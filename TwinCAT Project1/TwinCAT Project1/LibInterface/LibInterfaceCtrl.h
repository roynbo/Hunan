///////////////////////////////////////////////////////////////////////////////
// LibInterfaceCtrl.h

#ifndef __LIBINTERFACECTRL_H__
#define __LIBINTERFACECTRL_H__

#include <atlbase.h>
#include <atlcom.h>

#define LIBINTERFACEDRV_NAME "LIBINTERFACE"

#include "resource.h"       // main symbols
#include "LibInterfaceW32.h"
#include "TcBase.h"
#include "LibInterfaceClassFactory.h"
#include "TcOCFCtrlImpl.h"

class CLibInterfaceCtrl 
	: public CComObjectRootEx<CComMultiThreadModel>
	, public CComCoClass<CLibInterfaceCtrl, &CLSID_LibInterfaceCtrl>
	, public ILibInterfaceCtrl
	, public ITcOCFCtrlImpl<CLibInterfaceCtrl, CLibInterfaceClassFactory>
{
public:
	CLibInterfaceCtrl();
	virtual ~CLibInterfaceCtrl();

DECLARE_REGISTRY_RESOURCEID(IDR_LIBINTERFACECTRL)
DECLARE_NOT_AGGREGATABLE(CLibInterfaceCtrl)

DECLARE_PROTECT_FINAL_CONSTRUCT()

BEGIN_COM_MAP(CLibInterfaceCtrl)
	COM_INTERFACE_ENTRY(ILibInterfaceCtrl)
	COM_INTERFACE_ENTRY(ITcCtrl)
	COM_INTERFACE_ENTRY(ITcCtrl2)
END_COM_MAP()

};

#endif // #ifndef __LIBINTERFACECTRL_H__
