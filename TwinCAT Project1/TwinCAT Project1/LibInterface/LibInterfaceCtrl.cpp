// LibInterfaceCtrl.cpp : Implementation of CTcLibInterfaceCtrl
#include "TcPch.h"
#pragma hdrstop

#include "LibInterfaceW32.h"
#include "LibInterfaceCtrl.h"

/////////////////////////////////////////////////////////////////////////////
// CLibInterfaceCtrl

CLibInterfaceCtrl::CLibInterfaceCtrl() 
	: ITcOCFCtrlImpl<CLibInterfaceCtrl, CLibInterfaceClassFactory>() 
{
}

CLibInterfaceCtrl::~CLibInterfaceCtrl()
{
}

