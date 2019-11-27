#include "il2cpp-config.h"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif



#include "codegen/il2cpp-codegen-metadata.h"





IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END




// 0x00000001 System.Void UnityEngineInternal.Input.NativeUpdateCallback::.ctor(System.Object,System.IntPtr)
extern void NativeUpdateCallback__ctor_m5582C8C51CCFC860BC0A2A6DC5C746A05AABA622 ();
// 0x00000002 System.Void UnityEngineInternal.Input.NativeUpdateCallback::Invoke(UnityEngineInternal.Input.NativeInputUpdateType,UnityEngineInternal.Input.NativeInputEventBuffer*)
extern void NativeUpdateCallback_Invoke_m0F3B8F934AB1C953397B0E2134D17A9AAEBD975E ();
// 0x00000003 System.IAsyncResult UnityEngineInternal.Input.NativeUpdateCallback::BeginInvoke(UnityEngineInternal.Input.NativeInputUpdateType,UnityEngineInternal.Input.NativeInputEventBuffer*,System.AsyncCallback,System.Object)
extern void NativeUpdateCallback_BeginInvoke_m79CEAA701133792644EE6B9C9F1B234541EA0D5C ();
// 0x00000004 System.Void UnityEngineInternal.Input.NativeUpdateCallback::EndInvoke(System.IAsyncResult)
extern void NativeUpdateCallback_EndInvoke_m58A8CC712FC274C7B81CAD4D3B0AB159D3525246 ();
// 0x00000005 System.Void UnityEngineInternal.Input.NativeInputSystem::.cctor()
extern void NativeInputSystem__cctor_m062AB23A2EB8CDEC8BEB378C4588038BD1614A11 ();
// 0x00000006 System.Void UnityEngineInternal.Input.NativeInputSystem::NotifyBeforeUpdate(UnityEngineInternal.Input.NativeInputUpdateType)
extern void NativeInputSystem_NotifyBeforeUpdate_m317D534DBDD42915CE638051B87E48B76ED28574 ();
// 0x00000007 System.Void UnityEngineInternal.Input.NativeInputSystem::NotifyUpdate(UnityEngineInternal.Input.NativeInputUpdateType,System.IntPtr)
extern void NativeInputSystem_NotifyUpdate_m9AAC791622BA97DC7807610DD08C54F6011834F2 ();
// 0x00000008 System.Void UnityEngineInternal.Input.NativeInputSystem::NotifyDeviceDiscovered(System.Int32,System.String)
extern void NativeInputSystem_NotifyDeviceDiscovered_m48C3818DDCFB1A2C0530B6CE1C78743ABA4614B0 ();
// 0x00000009 System.Void UnityEngineInternal.Input.NativeInputSystem::ShouldRunUpdate(UnityEngineInternal.Input.NativeInputUpdateType,System.Boolean&)
extern void NativeInputSystem_ShouldRunUpdate_m3232491D0E639A8C58910B2ECD2967BAC8AFF9C6 ();
// 0x0000000A System.Void UnityEngineInternal.Input.NativeInputSystem::set_hasDeviceDiscoveredCallback(System.Boolean)
extern void NativeInputSystem_set_hasDeviceDiscoveredCallback_mB5319D5A7C245D6F12C294928D4AF3B635FFE362 ();
static Il2CppMethodPointer s_methodPointers[10] = 
{
	NativeUpdateCallback__ctor_m5582C8C51CCFC860BC0A2A6DC5C746A05AABA622,
	NativeUpdateCallback_Invoke_m0F3B8F934AB1C953397B0E2134D17A9AAEBD975E,
	NativeUpdateCallback_BeginInvoke_m79CEAA701133792644EE6B9C9F1B234541EA0D5C,
	NativeUpdateCallback_EndInvoke_m58A8CC712FC274C7B81CAD4D3B0AB159D3525246,
	NativeInputSystem__cctor_m062AB23A2EB8CDEC8BEB378C4588038BD1614A11,
	NativeInputSystem_NotifyBeforeUpdate_m317D534DBDD42915CE638051B87E48B76ED28574,
	NativeInputSystem_NotifyUpdate_m9AAC791622BA97DC7807610DD08C54F6011834F2,
	NativeInputSystem_NotifyDeviceDiscovered_m48C3818DDCFB1A2C0530B6CE1C78743ABA4614B0,
	NativeInputSystem_ShouldRunUpdate_m3232491D0E639A8C58910B2ECD2967BAC8AFF9C6,
	NativeInputSystem_set_hasDeviceDiscoveredCallback_mB5319D5A7C245D6F12C294928D4AF3B635FFE362,
};
static const int32_t s_InvokerIndices[10] = 
{
	111,
	790,
	2320,
	26,
	3,
	140,
	1417,
	536,
	1429,
	814,
};
extern const Il2CppCodeGenModule g_UnityEngine_InputModuleCodeGenModule;
const Il2CppCodeGenModule g_UnityEngine_InputModuleCodeGenModule = 
{
	"UnityEngine.InputModule.dll",
	10,
	s_methodPointers,
	s_InvokerIndices,
	0,
	NULL,
	0,
	NULL,
	0,
	NULL,
	NULL,
};
