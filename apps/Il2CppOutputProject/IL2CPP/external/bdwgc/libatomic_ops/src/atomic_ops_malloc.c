/*
 * Copyright (c) 2005 Hewlett-Packard Development Company, L.P.
 *
 * This file may be redistributed and/or modified under the
 * terms of the GNU General Public License as published by the Free Software
 * Foundation; either version 2, or (at your option) any later version.
 *
 * It is distributed in the hope that it will be useful, but WITHOUT ANY
 * WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.  See the GNU General Public License in the
 * file COPYING for more details.
 */

#if defined(HAVE_CONFIG_H)
# include "config.h"
#endif

#define AO_REQUIRE_CAS
#include "atomic_ops_malloc.h"

#include <string.h>     /* for ffs, which is assumed reentrant. */
#include <stdlib.h>
#include <assert.h>

#ifdef AO_TRACE_MALLOC
# include <stdio.h>
# include <pthread.h>
#endif

#if (defined(_WIN32_WCE) || defined(__MINGW32CE__)) && !defined(abort)
# define abort() _exit(-1) /* there is no abort() in WinCE */
#endif

/*
 * We round up each allocation request to the next power of two
 * minus one word.
 * We keep one stack of free objects for each size.  Each object
 * has an initial word (offset -sizeof(AO_t) from the visible pointer)
 * which contains either
 *      The binary log of the object size in bytes (small objects)
 *      The object size (a multiple of CHUNK_SIZE) for large objects.
 * The second case only arises if mmap-based allocation is supported.
 * We align the user-visible part of each object on a GRANULARITY
 * byte boundary.  That means that the actual (hidden) start of
 * the object starts a word before this boundary.
 */

#ifndef LOG_MAX_SIZE
# define LOG_MAX_SIZE 16
        /* We assume that 2**LOG_MAX_SIZE is a multiple of page size. */
#endif

#ifndef ALIGNMENT
# define ALIGNMENT 16
        /* Assumed to be at least sizeof(AO_t).         */
#endif

#define CHUNK_SIZE (1 << LOG_MAX_SIZE)

#ifndef AO_INITIAL_HEAP_SIZE
#  define AO_INITIAL_HEAP_SIZE (2*(LOG_MAX_SIZE+1)*CHUNK_SIZE)
#endif

char AO_initial_heap[AO_INITIAL_HEAP_SIZE];

static volatile AO_t initial_heap_ptr = (AO_t)AO_initial_heap;

#if defined(HAVE_MMAP)

#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>
#include <sys/mman.h>

#if defined(MAP_ANONYMOUS) || defined(MAP_ANON)
# define USE_MMAP_ANON
#endif

#ifdef USE_MMAP_FIXED
# define GC_MMAP_FLAGS (MAP_FIXED | MAP_PRIVATE)
        /* Seems to yield better performance on Solaris 2, but can      */
        /* be unreliable if something is already mapped at the address. */
#else
# define GC_MMAP_FLAGS MAP_PRIVATE
#endif

#ifdef USE_MMAP_ANON
# ifdef MAP_ANONYMOUS
#   define OPT_MAP_ANON MAP_ANONYMOUS
# else
#   define OPT_MAP_ANON MAP_ANON
# endif
#else
# define OPT_MAP_ANON 0
#endif

static volatile AO_t mmap_enabled = 0;

void
AO_malloc_enable_mmap(void)
{
# if defined(__sun)
    AO_store_release(&mmap_enabled, 1);
            /* Workaround for Sun CC */
# else
    AO_store(&mmap_enabled, 1);
# endif
}

static char *get_mmaped(size_t sz)
{
  char * result;
# ifdef USE_MMAP_ANON
#   define zero_fd -1
# else
    int zero_fd;
# endif

  assert(!(sz & (CHUNK_SIZE - 1)));
  if (!mmap_enabled)
    return 0;

# ifndef USE_MMAP_ANON
    zero_fd = open("/dev/zero", O_RDONLY);
    if (zero_fd == -1)
      return 0;
# endif
  result = mmap(0, sz, PROT_READ | PROT_WRITE,
                GC_MMAP_FLAGS | OPT_MAP_ANON, zero_fd, 0/* offset */);
# ifndef USE_MMAP_ANON
    close(zero_fd);
# endif
  if (result == MAP_FAILED)
    result = 0;
  return result;
}

/* Allocate an object of size (incl. header) of size > CHUNK_SIZE.      */
/* sz includes space for an AO_t-sized header.                          */
static char *
AO_malloc_large(size_t sz)
{
 char * result;
 /* The header will force us to waste ALIGNMENT bytes, incl. header.    */
   sz += ALIGNMENT;
 /* Round to multiple of CHUNK_SIZE.    */
   sz = (sz + CHUNK_SIZE - 1) & ~(CHUNK_SIZE - 1);
 result = get_mmaped(sz);
 if (result == 0) return 0;
 result += ALIGNMENT;
 ((AO_t *)result)[-1] = (AO_t)sz;
 return result;
}

static void
AO_free_large(char * p)
{
  AO_t sz = ((AO_t *)p)[-1];
  if (munmap(p - ALIGNMENT, (size_t)sz) != 0)
    abort();  /* Programmer error.  Not really async-signal-safe, but ... */
}


#else /*  No MMAP */

void
AO_malloc_enable_mmap(void)
{
}

#define get_mmaped(sz) ((char*)0)
#define AO_malloc_large(sz) ((char*)0)
#define AO_free_large(p) abort()
                /* Programmer error.  Not really async-signal-safe, but ... */

#endif /* No MMAP */

static char *
get_chunk(void)
{
  char *my_chunk_ptr;

  for (;;) {
    char *initial_ptr = (char *)AO_load(&initial_heap_ptr);

    my_chunk_ptr = (char *)(((AO_t)initial_ptr + (ALIGNMENT - 1))
                            & ~(ALIGNMENT - 1));
    if (initial_ptr != my_chunk_ptr)
      {
        /* Align correctly.  If this fails, someone else did it for us. */
        (void)AO_compare_and_swap_acquire(&initial_heap_ptr,
                                    (AO_t)initial_ptr, (AO_t)my_chunk_ptr);
      }

    if (my_chunk_ptr - AO_initial_heap > AO_INITIAL_HEAP_SIZE - CHUNK_SIZE)
      break;
    if (AO_compare_and_swap(&initial_heap_ptr, (AO_t)my_chunk_ptr,
                            (AO_t)(my_chunk_ptr + CHUNK_SIZE))) {
      return my_chunk_ptr;
    }
  }

  /* We failed.  The initial heap is used up.   */
  my_chunk_ptr = get_mmaped(CHUNK_SIZE);
  assert (!((AO_t)my_chunk_ptr & (ALIGNMENT-1)));
  return my_chunk_ptr;
}

/* Object free lists.  Ith entry corresponds to objects         */
/* of total size 2**i bytes.                                    */
AO_stack_t AO_free_list[LOG_MAX_SIZE+1];

/* Break up the chunk, and add it to the object free list for   */
/* the given size.  We have exclusive access to chunk.          */
static void add_chunk_as(void * chunk, unsigned log_sz)
{
  size_t ofs, limit;
  size_t sz = (size_t)1 << log_sz;

  assert (CHUNK_SIZE >= sz);
  limit = (size_t)CHUNK_SIZE - sz;
  for (ofs = ALIGNMENT - sizeof(AO_t); ofs <= limit; ofs += sz) {
    AO_stack_push(&AO_free_list[log_sz], (AO_t *)((char *)chunk + ofs));
  }
}

static const int msbs[16] = {0, 1, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4};

/* Return the position of the most significant set bit in the   */
/* argument.                                                    */
/* We follow the conventions of ffs(), i.e. the least           */
/* significant bit is number one.                               */
static int msb(size_t s)
{
  int result = 0;
  int v;
  if ((s & 0xff) != s) {
    /* The following is a tricky code ought to be equivalent to         */
    /* "(v = s >> 32) != 0" but suppresses warnings on 32-bit arch's.   */
    if (sizeof(size_t) > 4 && (v = s >> (sizeof(size_t) > 4 ? 32 : 0)) != 0)
      {
        s = v;
        result += 32;
      }
    if ((s >> 16) != 0)
      {
        s >>= 16;
        result += 16;
      }
    if ((s >> 8) != 0)
      {
        s >>= 8;
        result += 8;
      }
  }
  if (s > 15)
    {
      s >>= 4;
      result += 4;
    }
  result += msbs[s];
  return result;
}

void *
AO_malloc(size_t sz)
{
  AO_t *result;
  int log_sz;

  if (sz > CHUNK_SIZE)
    return AO_malloc_large(sz);
  log_sz = msb(sz + (sizeof(AO_t) - 1));
  result = AO_stack_pop(AO_free_list+log_sz);
  while (0 == result) {
    void * chunk = get_chunk();
    if (0 == chunk) return 0;
    add_chunk_as(chunk, log_sz);
    result = AO_stack_pop(AO_free_list+log_sz);
  }
  *result = log_sz;
# ifdef AO_TRACE_MALLOC
    fprintf(stderr, "%x: AO_malloc(%lu) = %p\n",
                    (int)pthread_self(), (unsigned long)sz, result+1);
# endif
  return result + 1;
}

void
AO_free(void *p)
{
  char *base = (char *)p - sizeof(AO_t);
  int log_sz;

  if (0 == p) return;
  log_sz = (int)(*(AO_t *)base);
# ifdef AO_TRACE_MALLOC
    fprintf(stderr, "%x: AO_free(%p sz:%lu)\n", (int)pthread_self(), p,
            (unsigned long)(log_sz > LOG_MAX_SIZE? log_sz : (1 << log_sz)));
# endif
  if (log_sz > LOG_MAX_SIZE)
    AO_free_large(p);
  else
    AO_stack_push(AO_free_list+log_sz, (AO_t *)base);
}
