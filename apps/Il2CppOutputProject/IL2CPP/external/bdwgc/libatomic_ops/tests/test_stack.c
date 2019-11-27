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

#include <stdio.h>

#if defined(__vxworks)

  int main(void)
  {
    printf("test skipped\n");
    return 0;
  }

#else

#if ((defined(_WIN32) && !defined(__CYGWIN32__) && !defined(__CYGWIN__)) \
     || defined(_MSC_VER) || defined(_WIN32_WINCE)) \
    && !defined(AO_USE_WIN32_PTHREADS)
# define USE_WINTHREADS
#endif

#ifdef USE_WINTHREADS
# include <windows.h>
#else
# include <pthread.h>
#endif

#include <stdlib.h>

#include "atomic_ops_stack.h" /* includes atomic_ops.h as well */

#if (defined(_WIN32_WCE) || defined(__MINGW32CE__)) && !defined(abort)
# define abort() _exit(-1) /* there is no abort() in WinCE */
#endif

#ifndef MAX_NTHREADS
# define MAX_NTHREADS 100
#endif

#ifdef NO_TIMES
# define get_msecs() 0
#elif defined(USE_WINTHREADS) || defined(AO_USE_WIN32_PTHREADS)
# include <sys/timeb.h>
  long long get_msecs(void)
  {
    struct timeb tb;

    ftime(&tb);
    return (long long)tb.time * 1000 + tb.millitm;
  }
#else /* Unix */
# include <time.h>
# include <sys/time.h>
  /* Need 64-bit long long support */
  long long get_msecs(void)
  {
    struct timeval tv;

    gettimeofday(&tv, 0);
    return (long long)tv.tv_sec * 1000 + tv.tv_usec/1000;
  }
#endif /* !NO_TIMES */

typedef struct le {
  AO_t next;
  int data;
} list_element;

AO_stack_t the_list = AO_STACK_INITIALIZER;

void add_elements(int n)
{
  list_element * le;
  if (n == 0) return;
  add_elements(n-1);
  le = malloc(sizeof(list_element));
  if (le == 0)
    {
      fprintf(stderr, "Out of memory\n");
      exit(2);
    }
  le -> data = n;
  AO_stack_push(&the_list, (AO_t *)le);
}

void print_list(void)
{
  list_element *p;

  for (p = (list_element *)AO_REAL_HEAD_PTR(the_list);
       p != 0;
       p = (list_element *)AO_REAL_NEXT_PTR(p -> next))
    printf("%d\n", p -> data);
}

static char marks[MAX_NTHREADS * (MAX_NTHREADS + 1) / 2 + 1];

void check_list(int n)
{
  list_element *p;
  int i;

  for (i = 1; i <= n; ++i) marks[i] = 0;

  for (p = (list_element *)AO_REAL_HEAD_PTR(the_list);
       p != 0;
       p = (list_element *)AO_REAL_NEXT_PTR(p -> next))
    {
      i = p -> data;
      if (i > n || i <= 0)
        {
          fprintf(stderr, "Found erroneous list element %d\n", i);
          abort();
        }
      if (marks[i] != 0)
        {
          fprintf(stderr, "Found duplicate list element %d\n", i);
          abort();
        }
      marks[i] = 1;
    }

  for (i = 1; i <= n; ++i)
    if (marks[i] != 1)
      {
        fprintf(stderr, "Missing list element %d\n", i);
        abort();
      }
}

volatile AO_t ops_performed = 0;

#ifndef LIMIT
        /* Total number of push/pop ops in all threads per test.    */
# ifdef AO_USE_PTHREAD_DEFS
#   define LIMIT 20000
# else
#   define LIMIT 1000000
# endif
#endif

#ifdef AO_HAVE_fetch_and_add
# define fetch_and_add(addr, val) AO_fetch_and_add(addr, val)
#else
  /* Fake it.  This is really quite unacceptable for timing */
  /* purposes.  But as a correctness test, it should be OK. */
  AO_INLINE AO_t fetch_and_add(volatile AO_t * addr, AO_t val)
  {
    AO_t result = AO_load(addr);
    AO_store(addr, result + val);
    return result;
  }
#endif

#ifdef USE_WINTHREADS
  DWORD WINAPI run_one_test(LPVOID arg)
#else
  void * run_one_test(void * arg)
#endif
{
  list_element * t[MAX_NTHREADS + 1];
  int index = (int)(size_t)arg;
  int i;
  int j = 0;

# ifdef VERBOSE
    printf("starting thread %d\n", index);
# endif
  while (fetch_and_add(&ops_performed, index + 1) + index + 1 < LIMIT)
    {
      for (i = 0; i < index + 1; ++i)
        {
          t[i] = (list_element *)AO_stack_pop(&the_list);
          if (0 == t[i])
            {
              fprintf(stderr, "FAILED\n");
              abort();
            }
        }
      for (i = 0; i < index + 1; ++i)
        {
          AO_stack_push(&the_list, (AO_t *)t[i]);
        }
      j += (index + 1);
    }
# ifdef VERBOSE
    printf("finished thread %d: %d total ops\n", index, j);
# endif
  return 0;
}

#ifndef N_EXPERIMENTS
# define N_EXPERIMENTS 1
#endif

unsigned long times[MAX_NTHREADS + 1][N_EXPERIMENTS];

int main(int argc, char **argv)
{
  int nthreads;
  int max_nthreads;
  int exper_n;

  if (1 == argc)
    max_nthreads = 4;
  else if (2 == argc)
    {
      max_nthreads = atoi(argv[1]);
      if (max_nthreads < 1 || max_nthreads > MAX_NTHREADS)
        {
          fprintf(stderr, "Invalid max # of threads argument\n");
          exit(1);
        }
    }
  else
    {
      fprintf(stderr, "Usage: %s [max # of threads]\n", argv[0]);
      exit(1);
    }
  for (exper_n = 0; exper_n < N_EXPERIMENTS; ++ exper_n)
    for (nthreads = 1; nthreads <= max_nthreads; ++nthreads)
      {
        int i;
#       ifdef USE_WINTHREADS
          DWORD thread_id;
          HANDLE thread[MAX_NTHREADS];
#       else
          pthread_t thread[MAX_NTHREADS];
#       endif
        int list_length = nthreads*(nthreads+1)/2;
        long long start_time;
        list_element * le;

#       ifdef VERBOSE
          printf("Before add_elements: exper_n=%d, nthreads=%d,"
                 " max_nthreads=%d, list_length=%d\n",
                 exper_n, nthreads, max_nthreads, list_length);
#       endif
        add_elements(list_length);
#       ifdef VERBOSE
          printf("Initial list (nthreads = %d):\n", nthreads);
          print_list();
#       endif
        ops_performed = 0;
        start_time = get_msecs();
        for (i = 1; i < nthreads; ++i) {
          int code;

#         ifdef USE_WINTHREADS
            thread[i] = CreateThread(NULL, 0, run_one_test, (LPVOID)(size_t)i,
                                     0, &thread_id);
            code = thread[i] != NULL ? 0 : (int)GetLastError();
#         else
            code = pthread_create(&thread[i], 0, run_one_test,
                                  (void *)(size_t)i);
#         endif
          if (code != 0) {
            fprintf(stderr, "Thread creation failed %u\n", (unsigned)code);
            exit(3);
          }
        }
        /* We use the main thread to run one test.  This allows gprof   */
        /* profiling to work, for example.                              */
        run_one_test(0);
        for (i = 1; i < nthreads; ++i) {
          int code;

#         ifdef USE_WINTHREADS
            code = WaitForSingleObject(thread[i], INFINITE) == WAIT_OBJECT_0 ?
                        0 : (int)GetLastError();
#         else
            code = pthread_join(thread[i], 0);
#         endif
          if (code != 0) {
            fprintf(stderr, "Thread join failed %u\n", (unsigned)code);
            abort();
          }
        }
        times[nthreads][exper_n] = (unsigned long)(get_msecs() - start_time);
  #     ifdef VERBOSE
          printf("%d %lu\n", nthreads,
                 (unsigned long)(get_msecs() - start_time));
          printf("final list (should be reordered initial list):\n");
          print_list();
  #     endif
        check_list(list_length);
        while ((le = (list_element *)AO_stack_pop(&the_list)) != 0)
          free(le);
      }
    for (nthreads = 1; nthreads <= max_nthreads; ++nthreads)
      {
        unsigned long sum = 0;

        printf("About %d pushes + %d pops in %d threads:",
               LIMIT, LIMIT, nthreads);
        for (exper_n = 0; exper_n < N_EXPERIMENTS; ++exper_n)
          {
#           if defined(VERBOSE)
              printf(" [%lu]", times[nthreads][exper_n]);
#           endif
            sum += times[nthreads][exper_n];
          }
#     ifndef NO_TIMES
        printf(" %lu msecs\n", (sum + N_EXPERIMENTS/2)/N_EXPERIMENTS);
#     else
        printf(" completed\n");
#     endif
      }
  return 0;
}

#endif
