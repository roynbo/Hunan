///////////////////////////////////////////////////////////////////////////////
// LibInterfaceDriver.cpp
#include "TcPch.h"
#pragma hdrstop

#include "LibInterfaceDriver.h"
#include "LibInterfaceClassFactory.h"

DECLARE_GENERIC_DEVICE(LIBINTERFACEDRV)

IOSTATUS CLibInterfaceDriver::OnLoad( )
{
	TRACE(_T("CObjClassFactory::OnLoad()\n") );
	m_pObjClassFactory = new CLibInterfaceClassFactory();

	return IOSTATUS_SUCCESS;
}

VOID CLibInterfaceDriver::OnUnLoad( )
{
	delete m_pObjClassFactory;
}

unsigned long _cdecl CLibInterfaceDriver::LIBINTERFACEDRV_GetVersion( )
{
	return( (LIBINTERFACEDRV_Major << 8) | LIBINTERFACEDRV_Minor );
}

