#ifndef _THREAD_X_  
#define _THREAD_X_  
/*
*********************************************************************************************************
*
*                                     COMMON TASK AND SEMAPHORE
*
* Filename      : ThreadX.h
* Version       : V1.0
* Programmer(s) : xclyfe
*********************************************************************************************************
*/
/*
*********************************************************************************************************
*                                        INCLUDE FILES
*********************************************************************************************************
*/
#include <stdio.h>  
#include <string>  
#include <windows.h>  
#include <process.h>  
using namespace std;

class ThreadX
{
public:
	string threadName;
protected:
	HANDLE m_handle;
	unsigned int m_threadId;
	string m_threadName;

public:
	ThreadX();
	virtual ~ThreadX();
	virtual void Start(bool bSuspend = false);

	string GetThreadName();
	void SetThreadName(string threadName);
	unsigned int GetThreadId();
	HANDLE GetThreadHandle();
public:
	unsigned long ResumeThreadX();
	unsigned long SuspendThreadX();
	int TerminateThreadX(unsigned long dwExitCode);
	int SetThreadPriorityX(int nPriority);
	int GetThreadPriorityX();
	void ExitThreadX(unsigned long dwExitCode);
	unsigned long WaitForSingleObjectX(unsigned long dwMilliseconds);
protected:
	static unsigned __stdcall ThreadStaticEntryPoint(void * pThis);
	static HANDLE BeginThreadX(void * _Security, unsigned _StackSize, void * _ArgList, unsigned _InitFlag, unsigned * _ThrdAddr);
	static HANDLE BeginThreadXDefault(void * _ArgList, unsigned int * threadId, bool bSuspend = false);
	virtual void ThreadEntryPoint() = 0;
};

#endif//_THREAD_X_  