/*
 * Copyright (c) 2003-2005 Hewlett-Packard Development Company, L.P.
 *
 * This file is covered by the GNU general public license, version 2.
 * see COPYING for details.
 */

#if defined(_MSC_VER) || \
    defined(_WIN32) && !defined(__CYGWIN32__) && !defined(__CYGWIN__) || \
    defined(_WIN32_WINCE)
#  define USE_WINTHREADS
#elif defined(__vxworks)
#  define USE_VXTHREADS
#else
#  define USE_PTHREADS
#endif

#include <stdlib.h>
#include <stdio.h>

#ifdef USE_PTHREADS
# include <pthread.h>
#endif

#ifdef USE_VXTHREADS
# include <vxworks.h>
# include <taskLib.h>
#endif

#ifdef USE_WINTHREADS
# include <windows.h>
#endif

#include "atomic_ops.h"

#if (defined(_WIN32_WCE) || defined(__MINGW32CE__)) && !defined(abort)
# define abort() _exit(-1) /* there is no abort() in WinCE */
#endif

#ifndef _WIN64
# define AO_PTRDIFF_T long
#elif defined(__int64)
# define AO_PTRDIFF_T __int64
#else
# define AO_PTRDIFF_T long long
#endif

typedef void * (* thr_func)(void *);

typedef int (* test_func)(void);    /* Returns != 0 on success */

void * run_parallel(int nthreads, thr_func f1, test_func t, const char *name);

#ifdef USE_PTHREADS
void * run_parallel(int nthreads, thr_func f1, test_func t, const char *name)
{
  pthread_attr_t attr;
  pthread_t thr[100];
  int i;
  int code;

  printf("Testing %s\n", name);
  if (nthreads > 100)
    {
      fprintf(stderr, "run_parallel: requested too many threads\n");
      abort();
    }

# ifdef _HPUX_SOURCE
   /* Default stack size is too small, especially with the 64 bit ABI */
   /* Increase it.                                                    */
    if (pthread_default_stacksize_np(1024*1024, 0) != 0)
      {
        fprintf(stderr, "pthread_default_stacksize_np failed. "
                        "OK after first call.\n");
      }
# endif

  pthread_attr_init(&attr);

  for (i = 0; i < nthreads; ++i)
    {
      if ((code = pthread_create(thr + i, &attr, f1, (void *)(long)i)) != 0)
      {
        fprintf(stderr, "pthread_create returned %d, thread %d\n", code, i);
        abort();
      }
    }
  for (i = 0; i < nthreads; ++i)
    {
      if ((code = pthread_join(thr[i], NULL)) != 0)
      {
        fprintf(stderr, "pthread_join returned %d, thread %d\n", code, i);
        abort();
      }
    }
  if (t())
    {
      printf("Succeeded\n");
    }
  else
    {
      fprintf(stderr, "Failed\n");
      abort();
    }
  return 0;
}
#endif /* USE_PTHREADS */

#ifdef USE_VXTHREADS
void * run_parallel(int nthreads, thr_func f1, test_func t, const char *name)
{
  int thr[100];
  int i;

  printf("Testing %s\n", name);
  if (nthreads > 100)
    {
      fprintf(stderr, "run_parallel: requested too many threads\n");
      taskSuspend(0);
    }

  for (i = 0; i < nthreads; ++i)
    {
      thr[i] = taskSpawn((char*) name, 180, 0, 32768, (FUNCPTR) f1, i,
                         1, 2, 3, 4, 5, 6, 7, 8, 9);
      if (thr[i] == ERROR)
      {
        fprintf(stderr, "taskSpawn failed with %d, thread %d\n",
                errno, i);
        taskSuspend(0);
      }
    }
  for (i = 0; i < nthreads; ++i)
    {
      while (taskIdVerify(thr[i]) == OK)
        taskDelay(60);
    }
  if (t())
    {
      printf("Succeeded\n");
    }
  else
    {
      fprintf(stderr, "Failed\n");
      taskSuspend(0);
    }
  return 0;
}
#endif /* USE_VXTHREADS */

#ifdef USE_WINTHREADS

struct tramp_args {
  thr_func fn;
  long arg;
};

DWORD WINAPI tramp(LPVOID param)
{
  struct tramp_args *args = (struct tramp_args *)param;

  return (DWORD)(AO_PTRDIFF_T)(*args->fn)((LPVOID)(AO_PTRDIFF_T)args->arg);
}

void * run_parallel(int nthreads, thr_func f1, test_func t, const char *name)
{
  HANDLE thr[100];
  struct tramp_args args[100];
  int i;
  DWORD code;

  printf("Testing %s\n", name);
  if (nthreads > 100)
    {
      fprintf(stderr, "run_parallel: requested too many threads\n");
      abort();
    }

  for (i = 0; i < nthreads; ++i)
    {
      args[i].fn = f1;
      args[i].arg = i;
      if ((thr[i] = CreateThread(NULL, 0, tramp, (LPVOID)(args+i), 0, NULL))
          == NULL)
        {
          fprintf(stderr, "CreateThread failed with %lu, thread %d\n",
                  (unsigned long)GetLastError(), i);
          abort();
        }
    }
  for (i = 0; i < nthreads; ++i)
    {
      if ((code = WaitForSingleObject(thr[i], INFINITE)) != WAIT_OBJECT_0)
      {
        fprintf(stderr, "WaitForSingleObject returned %lu, thread %d\n",
                (unsigned long)code, i);
        abort();
      }
    }
  if (t())
    {
      printf("Succeeded\n");
    }
  else
    {
      fprintf(stderr, "Failed\n");
      abort();
    }
  return 0;
}
#endif /* USE_WINTHREADS */
