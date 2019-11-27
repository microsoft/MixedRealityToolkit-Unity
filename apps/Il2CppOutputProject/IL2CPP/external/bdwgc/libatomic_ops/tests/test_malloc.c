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

#include "run_parallel.h"

#include <stdlib.h>
#include <stdio.h>
#include "atomic_ops_malloc.h"

#ifndef MAX_NTHREADS
# define MAX_NTHREADS 100
#endif

#ifndef DEFAULT_NTHREADS
# ifdef HAVE_MMAP
#   define DEFAULT_NTHREADS 10
# else
#   define DEFAULT_NTHREADS 3
# endif
#endif

#ifndef N_REVERSALS
# ifdef AO_USE_PTHREAD_DEFS
#   define N_REVERSALS 4
# else
#   define N_REVERSALS 1000 /* must be even */
# endif
#endif

#ifndef LIST_LENGTH
# ifdef HAVE_MMAP
#   define LIST_LENGTH 1000
# else
#   define LIST_LENGTH 100
# endif
#endif

#ifndef LARGE_OBJ_SIZE
# ifdef HAVE_MMAP
#   define LARGE_OBJ_SIZE 200000
# else
#   define LARGE_OBJ_SIZE 20000
# endif
#endif

#ifdef USE_STANDARD_MALLOC
# define AO_malloc(n) malloc(n)
# define AO_free(p) free(p)
# define AO_malloc_enable_mmap()
#endif

typedef struct list_node {
        struct list_node *next;
        int data;
} ln;

ln *cons(int d, ln *tail)
{
  static size_t extra = 0;
  size_t my_extra = extra;
  ln *result;
  int * extras;
  unsigned i;

  if (my_extra > 100)
    extra = my_extra = 0;
  else
    ++extra;
  result = AO_malloc(sizeof(ln) + sizeof(int)*my_extra);
  if (result == 0)
    {
      fprintf(stderr, "Out of memory\n");
        /* Normal for more than about 10 threads without mmap? */
      exit(2);
    }

  result -> data = d;
  result -> next = tail;
  extras = (int *)(result+1);
  for (i = 0; i < my_extra; ++i) extras[i] = 42;
  return result;
}

void print_list(ln *l)
{
  ln *p;

  for (p = l; p != 0; p = p -> next)
    {
      printf("%d, ", p -> data);
    }
  printf("\n");
}

/* Check that l contains numbers from m to n inclusive in ascending order */
void check_list(ln *l, int m, int n)
{
  ln *p;
  int i;

  for (p = l, i = m; p != 0 && i <= n; p = p -> next, ++i)
    {
      if (i != p -> data)
        {
          fprintf(stderr, "Found %d, expected %d\n", p -> data, i);
          abort();
        }
    }
  if (i <= n)
    {
      fprintf(stderr, "Number not found: %d\n", i);
      abort();
    }
  if (p != 0)
    {
      fprintf(stderr, "Found unexpected number: %d\n", i);
      abort();
    }
}

/* Create a list of integers from m to n */
ln *
make_list(int m, int n)
{
  if (m > n) return 0;
  return cons(m, make_list(m+1, n));
}

/* Reverse list x, and concatenate it to y, deallocating no longer needed */
/* nodes in x.                                                            */
ln *
reverse(ln *x, ln *y)
{
  ln * result;

  if (x == 0) return y;
  result = reverse(x -> next, cons(x -> data, y));
  AO_free(x);
  return result;
}

int dummy_test(void) { return 1; }

void * run_one_test(void * arg) {
  ln * x = make_list(1, LIST_LENGTH);
  int i;
  char *p = AO_malloc(LARGE_OBJ_SIZE);
  char *q;

  if (0 == p) {
#   ifdef HAVE_MMAP
      fprintf(stderr, "AO_malloc(%d) failed\n", LARGE_OBJ_SIZE);
      abort();
#   else
      fprintf(stderr, "AO_malloc(%d) failed: This is normal without mmap\n",
              LARGE_OBJ_SIZE);
#   endif
  } else {
    p[0] = p[LARGE_OBJ_SIZE/2] = p[LARGE_OBJ_SIZE-1] = 'a';
    q = AO_malloc(LARGE_OBJ_SIZE);
    if (q == 0)
      {
        fprintf(stderr, "Out of memory\n");
          /* Normal for more than about 10 threads without mmap? */
        exit(2);
      }
    q[0] = q[LARGE_OBJ_SIZE/2] = q[LARGE_OBJ_SIZE-1] = 'b';
    if (p[0] != 'a' || p[LARGE_OBJ_SIZE/2] != 'a'
        || p[LARGE_OBJ_SIZE-1] != 'a') {
      fprintf(stderr, "First large allocation smashed\n");
      abort();
    }
    AO_free(p);
    if (q[0] != 'b' || q[LARGE_OBJ_SIZE/2] != 'b'
        || q[LARGE_OBJ_SIZE-1] != 'b') {
      fprintf(stderr, "Second large allocation smashed\n");
      abort();
    }
    AO_free(q);
  }
# ifdef DEBUG_RUN_ONE_TEST
    x = reverse(x, 0);
    print_list(x);
    x = reverse(x, 0);
    print_list(x);
# endif
  for (i = 0; i < N_REVERSALS; ++i) {
    x = reverse(x, 0);
  }
  check_list(x, 1, LIST_LENGTH);
  return arg; /* use arg to suppress compiler warning */
}

int main(int argc, char **argv) {
    int nthreads;

    if (1 == argc) {
      nthreads = DEFAULT_NTHREADS;
    } else if (2 == argc) {
      nthreads = atoi(argv[1]);
      if (nthreads < 1 || nthreads > MAX_NTHREADS) {
        fprintf(stderr, "Invalid # of threads argument\n");
        exit(1);
      }
    } else {
      fprintf(stderr, "Usage: %s [# of threads]\n", argv[0]);
      exit(1);
    }
    printf("Performing %d reversals of %d element lists in %d threads\n",
           N_REVERSALS, LIST_LENGTH, nthreads);
    AO_malloc_enable_mmap();
    run_parallel(nthreads, run_one_test, dummy_test, "AO_malloc/AO_free");
    return 0;
}
