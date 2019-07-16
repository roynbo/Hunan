///////////////////////////////////////////////////////////////////////////////
// LibInterfaceDriver.h

#ifndef __LIBINTERFACEDRIVER_H__
#define __LIBINTERFACEDRIVER_H__

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "TcBase.h"

#define LIBINTERFACEDRV_NAME        "LIBINTERFACE"
#define LIBINTERFACEDRV_Major       1
#define LIBINTERFACEDRV_Minor       0

#define DEVICE_CLASS CLibInterfaceDriver

#include "ObjDriver.h"

class CLibInterfaceDriver : public CObjDriver
{
public:
	virtual IOSTATUS	OnLoad();
	virtual VOID		OnUnLoad();

	//////////////////////////////////////////////////////
	// VxD-Services exported by this driver
	static unsigned long	_cdecl LIBINTERFACEDRV_GetVersion();
	//////////////////////////////////////////////////////
	
};

Begin_VxD_Service_Table(LIBINTERFACEDRV)
	VxD_Service( LIBINTERFACEDRV_GetVersion )
End_VxD_Service_Table


#endif // ifndef __LIBINTERFACEDRIVER_H__