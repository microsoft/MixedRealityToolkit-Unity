#include "il2cpp-config.h"

#ifndef _MSC_VER
# include <alloca.h>
#else
# include <malloc.h>
#endif


#include <cstring>
#include <string.h>
#include <stdio.h>
#include <cmath>
#include <limits>
#include <assert.h>
#include <stdint.h>

#include "codegen/il2cpp-codegen.h"
#include "il2cpp-object-internals.h"

template <typename R>
struct VirtFuncInvoker0
{
	typedef R (*Func)(void*, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		return ((Func)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename T1>
struct VirtActionInvoker1
{
	typedef void (*Action)(void*, T1, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		((Action)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};
struct VirtActionInvoker0
{
	typedef void (*Action)(void*, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		((Action)invokeData.methodPtr)(obj, invokeData.method);
	}
};
struct GenericVirtActionInvoker0
{
	typedef void (*Action)(void*, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename T1>
struct GenericVirtActionInvoker1
{
	typedef void (*Action)(void*, T1, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3>
struct InterfaceActionInvoker3
{
	typedef void (*Action)(void*, T1, T2, T3, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2, T3 p3)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3, typename T4>
struct InterfaceActionInvoker4
{
	typedef void (*Action)(void*, T1, T2, T3, T4, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, p4, invokeData.method);
	}
};
template <typename R>
struct InterfaceFuncInvoker0
{
	typedef R (*Func)(void*, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		return ((Func)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename T1, typename T2>
struct InterfaceActionInvoker2
{
	typedef void (*Action)(void*, T1, T2, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
	}
};
struct InterfaceActionInvoker0
{
	typedef void (*Action)(void*, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename T1>
struct InterfaceActionInvoker1
{
	typedef void (*Action)(void*, T1, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};
template <typename R, typename T1>
struct GenericInterfaceFuncInvoker1
{
	typedef R (*Func)(void*, T1, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};
struct GenericInterfaceActionInvoker0
{
	typedef void (*Action)(void*, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename T1>
struct GenericInterfaceActionInvoker1
{
	typedef void (*Action)(void*, T1, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};

// Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile
struct BaseMixedRealityProfile_tB4DC16619B37D298D22571CE017070A78EF826E8;
// Microsoft.MixedReality.Toolkit.Input.BaseController
struct BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B;
// Microsoft.MixedReality.Toolkit.Input.BaseHand
struct BaseHand_tB58ECFC99FBFD516BBAA0989004A10F687078F4B;
// Microsoft.MixedReality.Toolkit.Input.HandRay
struct HandRay_t9DAE3FE243DBED1BAA1B9A4F782C3F1C9E6AE285;
// Microsoft.MixedReality.Toolkit.Input.IMixedRealityControllerVisualizer
struct IMixedRealityControllerVisualizer_tF11B01C18D3E7D9443AFA3B890520D0F196716C5;
// Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource
struct IMixedRealityInputSource_tE0E928A8AFA1825E798A69EB5D4BE993B8227ED2;
// Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem
struct IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B;
// Microsoft.MixedReality.Toolkit.Input.KeyBinding/<>c__DisplayClass5_0
struct U3CU3Ec__DisplayClass5_0_t5532E81B72C939F27BA424481612158E32B0C681;
// Microsoft.MixedReality.Toolkit.Input.MixedRealityControllerMappingProfile
struct MixedRealityControllerMappingProfile_t254FF0C8E28DA6341E36D23A9DD832B11ACCE9DB;
// Microsoft.MixedReality.Toolkit.Input.MixedRealityControllerVisualizationProfile
struct MixedRealityControllerVisualizationProfile_tDF62A9AB730F36F2AF8454D32ECF008D046E899D;
// Microsoft.MixedReality.Toolkit.Input.MixedRealityGestureMapping[]
struct MixedRealityGestureMappingU5BU5D_t2F3D7B685E29F06002C6BD2EF99A97C8DF6BD874;
// Microsoft.MixedReality.Toolkit.Input.MixedRealityGesturesProfile
struct MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725;
// Microsoft.MixedReality.Toolkit.Input.MixedRealityHandTrackingProfile
struct MixedRealityHandTrackingProfile_tFA3A9118040918D9E221EEB94786E3A333A12E36;
// Microsoft.MixedReality.Toolkit.Input.MixedRealityInputActionRulesProfile
struct MixedRealityInputActionRulesProfile_t4CE915FD59B3CEE0DDE18E9BF5922E5628DCCD3A;
// Microsoft.MixedReality.Toolkit.Input.MixedRealityInputActionsProfile
struct MixedRealityInputActionsProfile_t5D05F56AB25081BDE6B4697C8DF609F2A458EA12;
// Microsoft.MixedReality.Toolkit.Input.MixedRealityInputDataProviderConfiguration[]
struct MixedRealityInputDataProviderConfigurationU5BU5D_t621D14E0DCEE987398F574E5335D55FDBDFBE426;
// Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile
struct MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977;
// Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSystemProfile
struct MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2;
// Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping
struct MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2;
// Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping[]
struct MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA;
// Microsoft.MixedReality.Toolkit.Input.MixedRealityPointerProfile
struct MixedRealityPointerProfile_t006E66A5D042614269E3195CA2042A3AC964671E;
// Microsoft.MixedReality.Toolkit.Input.MixedRealitySpeechCommandsProfile
struct MixedRealitySpeechCommandsProfile_t73EF505A304D3B94E486F30B64A9A002FBD858CD;
// Microsoft.MixedReality.Toolkit.Input.SimulatedArticulatedHand
struct SimulatedArticulatedHand_tE70788F371CF5A48A99B3DE695FFA7A0FEF6E2E9;
// Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand
struct SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB;
// Microsoft.MixedReality.Toolkit.Input.SimulatedHand
struct SimulatedHand_tFBAB6AD39E9B16E093E63E4D2A88EA5E3415437E;
// Microsoft.MixedReality.Toolkit.Input.SimulatedHandData
struct SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1;
// Microsoft.MixedReality.Toolkit.Input.SimulatedHandData/HandJointDataGenerator
struct HandJointDataGenerator_t70BF622884D5C475C85D34FDE76FD298FAC37955;
// Microsoft.MixedReality.Toolkit.Input.SimulatedHandUtils
struct SimulatedHandUtils_t112B94E0F721072169327F6020348A7BB791A465;
// Microsoft.MixedReality.Toolkit.Input.StabilizedRay
struct StabilizedRay_tCE887AC85F7E1C0B2EA6DFE158B4BA7E7440E048;
// Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose[]
struct MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD;
// Microsoft.MixedReality.Toolkit.Utilities.SystemType
struct SystemType_t9696BD865921F75894EBD4D6EF913158A8BF3432;
// System.Action`2<Microsoft.MixedReality.Toolkit.Input.KeyBinding/KeyType,System.Int32>
struct Action_2_t599C81CC1C0CDFE287E5D39D3EEB3130080399E8;
// System.Action`2<System.Int32Enum,System.Int32>
struct Action_2_t211F8AF4611284BBE3D0590121A0E0BF9FA7E614;
// System.AsyncCallback
struct AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4;
// System.Char[]
struct CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2;
// System.Collections.Generic.Dictionary`2/Entry<Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose>[]
struct EntryU5BU5D_tDCE92978401B6E88C4A837958998A6DD61C63CD2;
// System.Collections.Generic.Dictionary`2/Entry<System.Int32,System.Tuple`2<Microsoft.MixedReality.Toolkit.Input.KeyBinding/KeyType,System.Int32>>[]
struct EntryU5BU5D_tCC3FFE930F278956DCD8BE65A7737B74C9A4E9DE;
// System.Collections.Generic.Dictionary`2/Entry<System.Tuple`2<Microsoft.MixedReality.Toolkit.Input.KeyBinding/KeyType,System.Int32>,System.Int32>[]
struct EntryU5BU5D_tAE4BD942D1AA89BBDD0C5F4280EC85C33EB60DF3;
// System.Collections.Generic.Dictionary`2/KeyCollection<Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose>
struct KeyCollection_t9B3F4312810F6E987754407200C69F9F8620465A;
// System.Collections.Generic.Dictionary`2/KeyCollection<System.Int32,System.Tuple`2<Microsoft.MixedReality.Toolkit.Input.KeyBinding/KeyType,System.Int32>>
struct KeyCollection_t2B6B73D918507DC5D3BA824CBABBF5B54F1F0FEB;
// System.Collections.Generic.Dictionary`2/KeyCollection<System.Tuple`2<Microsoft.MixedReality.Toolkit.Input.KeyBinding/KeyType,System.Int32>,System.Int32>
struct KeyCollection_t1535CF17E8BED19B5FDC0C264527A148331019B1;
// System.Collections.Generic.Dictionary`2/ValueCollection<Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose>
struct ValueCollection_tC935A0CB8F2162DB8392B9187AB1302A3041AC0B;
// System.Collections.Generic.Dictionary`2/ValueCollection<System.Int32,System.Tuple`2<Microsoft.MixedReality.Toolkit.Input.KeyBinding/KeyType,System.Int32>>
struct ValueCollection_tAFA4AA41F3ED39FB195B3986B51B08ABDEBA68FC;
// System.Collections.Generic.Dictionary`2/ValueCollection<System.Tuple`2<Microsoft.MixedReality.Toolkit.Input.KeyBinding/KeyType,System.Int32>,System.Int32>
struct ValueCollection_t3C7008B2E602BA0404280CBD1E1A3D1E565EE9DC;
// System.Collections.Generic.Dictionary`2<Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose>
struct Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86;
// System.Collections.Generic.Dictionary`2<System.Int32,System.Object>
struct Dictionary_2_t03608389BB57475AA3F4B2B79D176A27807BC884;
// System.Collections.Generic.Dictionary`2<System.Int32,System.Tuple`2<Microsoft.MixedReality.Toolkit.Input.KeyBinding/KeyType,System.Int32>>
struct Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64;
// System.Collections.Generic.Dictionary`2<System.Int32Enum,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose>
struct Dictionary_2_t4594E9EA67CB7172740DF4116774A3B1432A9E97;
// System.Collections.Generic.Dictionary`2<System.Object,System.Int32>
struct Dictionary_2_t81923CE2A312318AE13F58085CCF7FA8D879B77A;
// System.Collections.Generic.Dictionary`2<System.Tuple`2<Microsoft.MixedReality.Toolkit.Input.KeyBinding/KeyType,System.Int32>,System.Int32>
struct Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB;
// System.Collections.Generic.IEnumerable`1<System.Int32>
struct IEnumerable_1_t1AE8F03F101BA7578AF3A97EF1EBE8DB5FF31215;
// System.Collections.Generic.IEqualityComparer`1<Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint>
struct IEqualityComparer_1_t223CDEA89E79F60CB0C83D846FA30D2BD466ADFA;
// System.Collections.Generic.IEqualityComparer`1<System.Int32>
struct IEqualityComparer_1_t7B82AA0F8B96BAAA21E36DDF7A1FE4348BDDBE95;
// System.Collections.Generic.IEqualityComparer`1<System.Tuple`2<Microsoft.MixedReality.Toolkit.Input.KeyBinding/KeyType,System.Int32>>
struct IEqualityComparer_1_tF172A04A51B57A5C4EE2B3B8992DBD8C240C9E6F;
// System.Collections.Generic.List`1<System.Globalization.CultureInfo>
struct List_1_t74F59DD36FAE0CFB087612565C42CAD359647832;
// System.Collections.Generic.List`1<System.Object>
struct List_1_t05CC3C859AB5E6024394EF9A42E3E696628CA02D;
// System.Collections.Generic.List`1<System.String>
struct List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3;
// System.Delegate
struct Delegate_t;
// System.DelegateData
struct DelegateData_t1BF9F691B56DAE5F8C28C5E084FDE94F15F27BBE;
// System.Delegate[]
struct DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86;
// System.IAsyncResult
struct IAsyncResult_t8E194308510B375B42432981AE5E7488C458D598;
// System.Int32[]
struct Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83;
// System.Object[]
struct ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A;
// System.Reflection.Binder
struct Binder_t4D5CB06963501D32847C057B57157D6DC49CA759;
// System.Reflection.MemberFilter
struct MemberFilter_t25C1BD92C42BE94426E300787C13C452CB89B381;
// System.Reflection.MethodInfo
struct MethodInfo_t;
// System.String
struct String_t;
// System.String[]
struct StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E;
// System.Tuple`2<Microsoft.MixedReality.Toolkit.Input.KeyBinding/KeyType,System.Int32>
struct Tuple_2_tFF0D9FEC0FEA81089BD6B1384583703BD0A104EE;
// System.Tuple`2<System.Int32Enum,System.Int32>
struct Tuple_2_t6013D918BF7AB88AC1206529AAB17213208F33F0;
// System.Type
struct Type_t;
// System.Type[]
struct TypeU5BU5D_t7FE623A666B49176DE123306221193E888A12F5F;
// System.Void
struct Void_t22962CB4C05B1D89B55A6E1139F0E87A90987017;
// UnityEngine.Camera
struct Camera_t48B2B9ECB3CE6108A98BF949A1CECF0FE3421F34;
// UnityEngine.Camera/CameraCallback
struct CameraCallback_t8BBB42AA08D7498DFC11F4128117055BC7F0B9D0;
// UnityEngine.Component
struct Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621;
// UnityEngine.GameObject
struct GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F;
// UnityEngine.Object
struct Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0;
// UnityEngine.Quaternion[]
struct QuaternionU5BU5D_t26EB10EEE89DD3EF913D52E8797FAB841F6F2AA3;
// UnityEngine.Transform
struct Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA;
// UnityEngine.Vector3[]
struct Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28;

IL2CPP_EXTERN_C RuntimeClass* Action_2_t599C81CC1C0CDFE287E5D39D3EEB3130080399E8_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Enum_t2AF27C02B8653AE29442467390005ABC74D8F521_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IInputSimulationService_t9AF3035C6487685E30A3E3ADB5E2D70DC2C3B443_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IMixedRealityDataProviderAccess_t8EDB3ADE5066213B543EB035F96F346DEF5FD94C_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* KeyCodeU5BU5D_tF4382F22534318B6E15A70B33AAF395B3D8D127F_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* KeyCode_tC93EA87C5A6901160B583ADFCD3EF6726570DC3C_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* KeyType_t63A0EC9B1C9653881B95DF409080C7FB24760D72_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Mathf_tFBDE6467D269BFE410605C7D806FD9991D4A89CB_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* MixedRealityServiceRegistry_t32DA3C08833DAE82817D72D1EE88363D3064D911_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* MouseButtonU5BU5D_t6CE0267665AAD6A7B40F7782DA60DD3810558E82_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* MouseButton_t4174FC057A73B1ECBC9603C3AF8AF87E964E719E_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SimulatedHandUtils_t112B94E0F721072169327F6020348A7BB791A465_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SimulatedHand_tFBAB6AD39E9B16E093E63E4D2A88EA5E3415437E_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Type_t_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* U3CU3Ec__DisplayClass5_0_t5532E81B72C939F27BA424481612158E32B0C681_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeField* U3CPrivateImplementationDetailsU3E_t5D7196C8D3A7E05A50169A365F5A7B3B92600D14____6AF7EBB4A5EF5D7478981B4AA0BAD37788AAB1ED_0_FieldInfo_var;
IL2CPP_EXTERN_C String_t* _stringLiteral04734178D407F1573AAACEB7E086B11BCFABD7FF;
IL2CPP_EXTERN_C String_t* _stringLiteral0F9D13B1C31A5F4C68D0EEA587D21588F757084E;
IL2CPP_EXTERN_C String_t* _stringLiteral1E88AB05D76FF253F292B74866D32460BB3836E2;
IL2CPP_EXTERN_C String_t* _stringLiteral294D359ECE148A430F19981912277E5154CA19E0;
IL2CPP_EXTERN_C String_t* _stringLiteral2FEED76F1368917E9E5273B5D3B77EC607649D4D;
IL2CPP_EXTERN_C String_t* _stringLiteral4B937CC841D82F8936CEF1EFB88708AB5B0F1EE5;
IL2CPP_EXTERN_C String_t* _stringLiteral4F57A1CE99E68A7B05C42D0A7EA0070EAFABD31C;
IL2CPP_EXTERN_C String_t* _stringLiteral561DDB78EA3339033D719AFAA6980160DC8D88CB;
IL2CPP_EXTERN_C String_t* _stringLiteral627A7387C8BDDC7ACFF00D342D3F799DC6C19A31;
IL2CPP_EXTERN_C String_t* _stringLiteral66654F3A427908EF2AB0102919620271D634DA8A;
IL2CPP_EXTERN_C String_t* _stringLiteral8598222918D3C6E513D63060CF55E2971DED729A;
IL2CPP_EXTERN_C String_t* _stringLiteral8B7970623A806CC748C1B218861BE920B011B98C;
IL2CPP_EXTERN_C String_t* _stringLiteralCECA32E904728D1645727CB2B9CDEAA153807D77;
IL2CPP_EXTERN_C String_t* _stringLiteralCF673A9C875D20DCDA8A5C0D7A2E5C60A940DB8E;
IL2CPP_EXTERN_C String_t* _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709;
IL2CPP_EXTERN_C String_t* _stringLiteralE705DD1D38D6989FA3B3CCE68EC8B3C54B31ECFC;
IL2CPP_EXTERN_C const RuntimeMethod* Action_2_Invoke_m0D15E6E36BD572A4DF315B9F04F30A0F0EFE31E5_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Action_2__ctor_mF9F632823062B05D3DA92A0649DC4EE862AE1C7A_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_Add_mF5D352A2DB17E5E4545D622A66744A4697ACC3D2_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_ContainsKey_m9123BEB1C67E91B9D1C87834EED0E4805EAB9389_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_TryGetValue_mEB4E22F5D5C93FBC06285B7EA9EDC0B6B73CF31D_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2__ctor_m2298C894CE2941227F176A13E8FF938BD954E63B_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2__ctor_m747FD3B997983E98D0914810BA2B843ED90D554B_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2__ctor_mD52EC03DD022577E1A73259E748910906383DA4E_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_get_Item_mAA87FA69922BAF6733C05E34A765031668FCABA6_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_set_Item_m3D5CB4BFE05FDFFBEFF66F28C80B6AF3A94ECBF5_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_set_Item_m71327547831A3689A4215232C29A1EBA103BE6DE_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2_set_Item_mA73F452CC26A09DD780D50EAE46E8684633BA15B_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Enumerable_Take_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_mCBED6C7F74DCC17FA9C923D11B6801F52FEEB61B_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* IMixedRealityDataProviderAccess_GetDataProvider_TisIInputSimulationService_t9AF3035C6487685E30A3E3ADB5E2D70DC2C3B443_m33255EF491AD44DA64F7825B26A7EEFE2BFAD51A_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1_Add_mA348FA1140766465189459D25B01EB179001DE83_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1_ToArray_m9DD19D800AE6D84ED0729D5D97CAF84DF317DD38_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* List_1__ctor_mDA22758D73530683C950C5CCF39BDB4E7E1F3F06_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* MixedRealityServiceRegistry_TryGetService_TisIMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_m11EAC52C13EC4EEBB2BC67A0F3F775159F619EAD_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Nullable_1_GetValueOrDefault_mE89BB8F302DF31EE202251F4746859285860B6B6_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Nullable_1__ctor_m11F9C228CFDF836DDFCD7880C09CB4098AB9D7F2_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Nullable_1_get_HasValue_mB664E2C41CADA8413EF8842E6601B8C696A7CE15_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Tuple_Create_TisKeyType_t63A0EC9B1C9653881B95DF409080C7FB24760D72_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_mA5D31171EBE5513EC23DF8E079EC60FE1EE2E658_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* U3CU3Ec__DisplayClass5_0_U3C_cctorU3Eb__0_m7589D4054CF6C9029801CCE9EC4CD741486AD169_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeType* KeyCode_tC93EA87C5A6901160B583ADFCD3EF6726570DC3C_0_0_0_var;
IL2CPP_EXTERN_C const RuntimeType* MouseButton_t4174FC057A73B1ECBC9603C3AF8AF87E964E719E_0_0_0_var;
IL2CPP_EXTERN_C const RuntimeType* TrackedHandJoint_tDE2FD40782A5B0C1D39386D6BF70D8A1CCF94E22_0_0_0_var;
IL2CPP_EXTERN_C const uint32_t KeyBinding_FromMouseButton_mC7479108FCC71C952AAB38A9526E2B82B71C8CD0_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t KeyBinding_ToString_mB8F2F02D75495579EEDDB8B27851E0BFC044B526_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t KeyBinding__cctor_m52B9381B882303097E8CC5BE8025234BAC0A75DA_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t MixedRealityInputAction_get_None_m0276CF8988B0670DCCE381865DD5190010A2A8BFMicrosoft_MixedReality_Toolkit_Services_InputSimulation_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t MixedRealityInputSimulationProfile__ctor_m9769DFD9BDD54BA2B6A190798622CEDC78EA2EEB_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t MixedRealityPose_get_ZeroIdentity_m80C016329EAADDC4EB8DFD80ED0CF614A5E547ADMicrosoft_MixedReality_Toolkit_Services_InputSimulation_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedArticulatedHand_UpdateInteractions_m982D348EDBBB3D148D95B9F7E4BF863AFB851DA9_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedArticulatedHand__ctor_m5518A9A451EE08DB313A88F7EDF1FCF72BFD5333_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedArticulatedHand_get_DefaultInteractions_mDE48166990BF99C0D3809DD299CDCC0FC06777B4_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedGestureHand_EnsureProfileSettings_m5FC39BD038B64363C40173D9E60B1BC1606C7A3A_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedGestureHand_TryCancelHold_m1F67089B7A138E396206FE8E7E0DAEECCE14BFBC_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedGestureHand_TryCancelManipulation_m774C717F6300ED032BD87747966E2EBFBE9F3159_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedGestureHand_TryCancelNavigation_m7F78258B782D49B12470728A9F18ECFE2C0138A5_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedGestureHand_TryCompleteHold_mA3B5BAB738C6425798C608310D7D59D6B6FCA1AC_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedGestureHand_TryCompleteManipulation_m7DD88EA40E108EB197BF22BD11460BF7A3DFBB18_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedGestureHand_TryCompleteNavigation_m725C944777267419341F15E256472663CBCE6AC8_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedGestureHand_TryCompleteSelect_m39126D98BA2E83C742CDA9EAEA81EB5128B541AC_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedGestureHand_TryStartHold_m72CBFF5CAEDDC55C9E865745A5DE4C34C1B2E234_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedGestureHand_TryStartManipulation_m0B58E7807CC8E31CE5F4817A99CC358085866A3E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedGestureHand_TryStartNavigation_m2F5F675D13ACB7225B7672755846459058BDF575_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedGestureHand_UpdateInteractions_m96F24F8AEC7B7EC9C96EAF20378C4BBF49B26DF8_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedGestureHand_UpdateManipulation_m7D7C54E9B0364BA9862D4326D9606FB6419CCBC3_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedGestureHand_UpdateNavigationRails_mDA8C27C354D28CD6BC7E7EB7E4A84A560D1B08A6_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedGestureHand_UpdateNavigation_mD504939EDF859CD568D6127F467D193ADF3ADFC0_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedGestureHand__ctor_m93581EB80551349B8F9FD7C292CBDBFA5243F97A_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedGestureHand_get_DefaultInteractions_m304D32B99A064523F1EC9DFD6873DEB55A56A8AF_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedGestureHand_get_navigationDelta_m0FD22233CFFA608F80B80E740D01DA6F8E22582A_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedHandData_Copy_m41ABA1DF6D6E58F82E3DF8D876F210F2D75BCC52_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedHandData__cctor_m9FF93A339C2E4BD70FD2048183E316BDEFD82849_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedHandData__ctor_mC0F48E57A15AA83EB147D0682EAFD4B9A13A74E3_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedHandData_get_InputSystem_m74B585679CB887A0A5722F761D09C8AC21A5E799_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedHandUtils_CalculateJointRotations_mA0A1808305AB3D8B589A08E42F9155739D9221AE_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedHandUtils_GetPalmForwardVector_m9E069A581F41648ADB1D947EDBB726BD867602F4_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedHandUtils_GetPalmRightVector_m9C646FB51F2C94823DC3EEE26383B22A88EA4301_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedHandUtils_GetPalmUpVector_mB1852A38F5919EC805FE801DB47DC6DA1E64CCD0_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedHandUtils__cctor_mE9EC43A15625808EECB51ECE0AA4C867F45C6733_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedHand_TryGetJoint_m14B9D4449933B89DB099541E2901B4017D613B64_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedHand_UpdateState_m76167DB74444C36B375258174DBB71C74806C7E7_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedHand__cctor_mD1BA38A6EB0C974530FDAEA1E4A70CE9C16F7B5A_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SimulatedHand__ctor_m93808D1348F3FB6FA63A335E89F47FB5345EE1C4_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t U3CU3Ec__DisplayClass5_0_U3C_cctorU3Eb__0_m7589D4054CF6C9029801CCE9EC4CD741486AD169_MetadataUsageId;
struct Delegate_t_marshaled_com;
struct Delegate_t_marshaled_pinvoke;

struct MouseButtonU5BU5D_t6CE0267665AAD6A7B40F7782DA60DD3810558E82;
struct MixedRealityGestureMappingU5BU5D_t2F3D7B685E29F06002C6BD2EF99A97C8DF6BD874;
struct MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA;
struct MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD;
struct DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86;
struct Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83;
struct StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E;
struct KeyCodeU5BU5D_tF4382F22534318B6E15A70B33AAF395B3D8D127F;
struct QuaternionU5BU5D_t26EB10EEE89DD3EF913D52E8797FAB841F6F2AA3;
struct Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28;

IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// <Module>
struct  U3CModuleU3E_t22A22D38C0EE8B824B145675D71FBBE1E3B07B69 
{
public:

public:
};


// System.Object


// Microsoft.MixedReality.Toolkit.Input.KeyBinding_<>c__DisplayClass5_0
struct  U3CU3Ec__DisplayClass5_0_t5532E81B72C939F27BA424481612158E32B0C681  : public RuntimeObject
{
public:
	// System.Collections.Generic.List`1<System.String> Microsoft.MixedReality.Toolkit.Input.KeyBinding_<>c__DisplayClass5_0::names
	List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3 * ___names_0;
	// System.Int32 Microsoft.MixedReality.Toolkit.Input.KeyBinding_<>c__DisplayClass5_0::index
	int32_t ___index_1;

public:
	inline static int32_t get_offset_of_names_0() { return static_cast<int32_t>(offsetof(U3CU3Ec__DisplayClass5_0_t5532E81B72C939F27BA424481612158E32B0C681, ___names_0)); }
	inline List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3 * get_names_0() const { return ___names_0; }
	inline List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3 ** get_address_of_names_0() { return &___names_0; }
	inline void set_names_0(List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3 * value)
	{
		___names_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___names_0), (void*)value);
	}

	inline static int32_t get_offset_of_index_1() { return static_cast<int32_t>(offsetof(U3CU3Ec__DisplayClass5_0_t5532E81B72C939F27BA424481612158E32B0C681, ___index_1)); }
	inline int32_t get_index_1() const { return ___index_1; }
	inline int32_t* get_address_of_index_1() { return &___index_1; }
	inline void set_index_1(int32_t value)
	{
		___index_1 = value;
	}
};


// Microsoft.MixedReality.Toolkit.Input.KeyInputSystem
struct  KeyInputSystem_t51B00541CF918BF3C5F238C506A643440D333BC3  : public RuntimeObject
{
public:

public:
};


// Microsoft.MixedReality.Toolkit.Input.SimulatedHandData
struct  SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1  : public RuntimeObject
{
public:
	// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedHandData::isTracked
	bool ___isTracked_1;
	// Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose[] Microsoft.MixedReality.Toolkit.Input.SimulatedHandData::joints
	MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD* ___joints_2;
	// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedHandData::isPinching
	bool ___isPinching_3;
	// Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem Microsoft.MixedReality.Toolkit.Input.SimulatedHandData::inputSystem
	RuntimeObject* ___inputSystem_4;

public:
	inline static int32_t get_offset_of_isTracked_1() { return static_cast<int32_t>(offsetof(SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1, ___isTracked_1)); }
	inline bool get_isTracked_1() const { return ___isTracked_1; }
	inline bool* get_address_of_isTracked_1() { return &___isTracked_1; }
	inline void set_isTracked_1(bool value)
	{
		___isTracked_1 = value;
	}

	inline static int32_t get_offset_of_joints_2() { return static_cast<int32_t>(offsetof(SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1, ___joints_2)); }
	inline MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD* get_joints_2() const { return ___joints_2; }
	inline MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD** get_address_of_joints_2() { return &___joints_2; }
	inline void set_joints_2(MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD* value)
	{
		___joints_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___joints_2), (void*)value);
	}

	inline static int32_t get_offset_of_isPinching_3() { return static_cast<int32_t>(offsetof(SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1, ___isPinching_3)); }
	inline bool get_isPinching_3() const { return ___isPinching_3; }
	inline bool* get_address_of_isPinching_3() { return &___isPinching_3; }
	inline void set_isPinching_3(bool value)
	{
		___isPinching_3 = value;
	}

	inline static int32_t get_offset_of_inputSystem_4() { return static_cast<int32_t>(offsetof(SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1, ___inputSystem_4)); }
	inline RuntimeObject* get_inputSystem_4() const { return ___inputSystem_4; }
	inline RuntimeObject** get_address_of_inputSystem_4() { return &___inputSystem_4; }
	inline void set_inputSystem_4(RuntimeObject* value)
	{
		___inputSystem_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___inputSystem_4), (void*)value);
	}
};

struct SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1_StaticFields
{
public:
	// System.Int32 Microsoft.MixedReality.Toolkit.Input.SimulatedHandData::jointCount
	int32_t ___jointCount_0;

public:
	inline static int32_t get_offset_of_jointCount_0() { return static_cast<int32_t>(offsetof(SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1_StaticFields, ___jointCount_0)); }
	inline int32_t get_jointCount_0() const { return ___jointCount_0; }
	inline int32_t* get_address_of_jointCount_0() { return &___jointCount_0; }
	inline void set_jointCount_0(int32_t value)
	{
		___jointCount_0 = value;
	}
};


// Microsoft.MixedReality.Toolkit.Input.SimulatedHandUtils
struct  SimulatedHandUtils_t112B94E0F721072169327F6020348A7BB791A465  : public RuntimeObject
{
public:

public:
};

struct SimulatedHandUtils_t112B94E0F721072169327F6020348A7BB791A465_StaticFields
{
public:
	// System.Int32 Microsoft.MixedReality.Toolkit.Input.SimulatedHandUtils::jointCount
	int32_t ___jointCount_0;

public:
	inline static int32_t get_offset_of_jointCount_0() { return static_cast<int32_t>(offsetof(SimulatedHandUtils_t112B94E0F721072169327F6020348A7BB791A465_StaticFields, ___jointCount_0)); }
	inline int32_t get_jointCount_0() const { return ___jointCount_0; }
	inline int32_t* get_address_of_jointCount_0() { return &___jointCount_0; }
	inline void set_jointCount_0(int32_t value)
	{
		___jointCount_0 = value;
	}
};

struct Il2CppArrayBounds;

// System.Array


// System.Collections.Generic.Dictionary`2<Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose>
struct  Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86  : public RuntimeObject
{
public:
	// System.Int32[] System.Collections.Generic.Dictionary`2::buckets
	Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* ___buckets_0;
	// System.Collections.Generic.Dictionary`2_Entry<TKey,TValue>[] System.Collections.Generic.Dictionary`2::entries
	EntryU5BU5D_tDCE92978401B6E88C4A837958998A6DD61C63CD2* ___entries_1;
	// System.Int32 System.Collections.Generic.Dictionary`2::count
	int32_t ___count_2;
	// System.Int32 System.Collections.Generic.Dictionary`2::version
	int32_t ___version_3;
	// System.Int32 System.Collections.Generic.Dictionary`2::freeList
	int32_t ___freeList_4;
	// System.Int32 System.Collections.Generic.Dictionary`2::freeCount
	int32_t ___freeCount_5;
	// System.Collections.Generic.IEqualityComparer`1<TKey> System.Collections.Generic.Dictionary`2::comparer
	RuntimeObject* ___comparer_6;
	// System.Collections.Generic.Dictionary`2_KeyCollection<TKey,TValue> System.Collections.Generic.Dictionary`2::keys
	KeyCollection_t9B3F4312810F6E987754407200C69F9F8620465A * ___keys_7;
	// System.Collections.Generic.Dictionary`2_ValueCollection<TKey,TValue> System.Collections.Generic.Dictionary`2::values
	ValueCollection_tC935A0CB8F2162DB8392B9187AB1302A3041AC0B * ___values_8;
	// System.Object System.Collections.Generic.Dictionary`2::_syncRoot
	RuntimeObject * ____syncRoot_9;

public:
	inline static int32_t get_offset_of_buckets_0() { return static_cast<int32_t>(offsetof(Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86, ___buckets_0)); }
	inline Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* get_buckets_0() const { return ___buckets_0; }
	inline Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83** get_address_of_buckets_0() { return &___buckets_0; }
	inline void set_buckets_0(Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* value)
	{
		___buckets_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___buckets_0), (void*)value);
	}

	inline static int32_t get_offset_of_entries_1() { return static_cast<int32_t>(offsetof(Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86, ___entries_1)); }
	inline EntryU5BU5D_tDCE92978401B6E88C4A837958998A6DD61C63CD2* get_entries_1() const { return ___entries_1; }
	inline EntryU5BU5D_tDCE92978401B6E88C4A837958998A6DD61C63CD2** get_address_of_entries_1() { return &___entries_1; }
	inline void set_entries_1(EntryU5BU5D_tDCE92978401B6E88C4A837958998A6DD61C63CD2* value)
	{
		___entries_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___entries_1), (void*)value);
	}

	inline static int32_t get_offset_of_count_2() { return static_cast<int32_t>(offsetof(Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86, ___count_2)); }
	inline int32_t get_count_2() const { return ___count_2; }
	inline int32_t* get_address_of_count_2() { return &___count_2; }
	inline void set_count_2(int32_t value)
	{
		___count_2 = value;
	}

	inline static int32_t get_offset_of_version_3() { return static_cast<int32_t>(offsetof(Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86, ___version_3)); }
	inline int32_t get_version_3() const { return ___version_3; }
	inline int32_t* get_address_of_version_3() { return &___version_3; }
	inline void set_version_3(int32_t value)
	{
		___version_3 = value;
	}

	inline static int32_t get_offset_of_freeList_4() { return static_cast<int32_t>(offsetof(Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86, ___freeList_4)); }
	inline int32_t get_freeList_4() const { return ___freeList_4; }
	inline int32_t* get_address_of_freeList_4() { return &___freeList_4; }
	inline void set_freeList_4(int32_t value)
	{
		___freeList_4 = value;
	}

	inline static int32_t get_offset_of_freeCount_5() { return static_cast<int32_t>(offsetof(Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86, ___freeCount_5)); }
	inline int32_t get_freeCount_5() const { return ___freeCount_5; }
	inline int32_t* get_address_of_freeCount_5() { return &___freeCount_5; }
	inline void set_freeCount_5(int32_t value)
	{
		___freeCount_5 = value;
	}

	inline static int32_t get_offset_of_comparer_6() { return static_cast<int32_t>(offsetof(Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86, ___comparer_6)); }
	inline RuntimeObject* get_comparer_6() const { return ___comparer_6; }
	inline RuntimeObject** get_address_of_comparer_6() { return &___comparer_6; }
	inline void set_comparer_6(RuntimeObject* value)
	{
		___comparer_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___comparer_6), (void*)value);
	}

	inline static int32_t get_offset_of_keys_7() { return static_cast<int32_t>(offsetof(Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86, ___keys_7)); }
	inline KeyCollection_t9B3F4312810F6E987754407200C69F9F8620465A * get_keys_7() const { return ___keys_7; }
	inline KeyCollection_t9B3F4312810F6E987754407200C69F9F8620465A ** get_address_of_keys_7() { return &___keys_7; }
	inline void set_keys_7(KeyCollection_t9B3F4312810F6E987754407200C69F9F8620465A * value)
	{
		___keys_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___keys_7), (void*)value);
	}

	inline static int32_t get_offset_of_values_8() { return static_cast<int32_t>(offsetof(Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86, ___values_8)); }
	inline ValueCollection_tC935A0CB8F2162DB8392B9187AB1302A3041AC0B * get_values_8() const { return ___values_8; }
	inline ValueCollection_tC935A0CB8F2162DB8392B9187AB1302A3041AC0B ** get_address_of_values_8() { return &___values_8; }
	inline void set_values_8(ValueCollection_tC935A0CB8F2162DB8392B9187AB1302A3041AC0B * value)
	{
		___values_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___values_8), (void*)value);
	}

	inline static int32_t get_offset_of__syncRoot_9() { return static_cast<int32_t>(offsetof(Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86, ____syncRoot_9)); }
	inline RuntimeObject * get__syncRoot_9() const { return ____syncRoot_9; }
	inline RuntimeObject ** get_address_of__syncRoot_9() { return &____syncRoot_9; }
	inline void set__syncRoot_9(RuntimeObject * value)
	{
		____syncRoot_9 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____syncRoot_9), (void*)value);
	}
};


// System.Collections.Generic.Dictionary`2<System.Int32,System.Tuple`2<Microsoft.MixedReality.Toolkit.Input.KeyBinding_KeyType,System.Int32>>
struct  Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64  : public RuntimeObject
{
public:
	// System.Int32[] System.Collections.Generic.Dictionary`2::buckets
	Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* ___buckets_0;
	// System.Collections.Generic.Dictionary`2_Entry<TKey,TValue>[] System.Collections.Generic.Dictionary`2::entries
	EntryU5BU5D_tCC3FFE930F278956DCD8BE65A7737B74C9A4E9DE* ___entries_1;
	// System.Int32 System.Collections.Generic.Dictionary`2::count
	int32_t ___count_2;
	// System.Int32 System.Collections.Generic.Dictionary`2::version
	int32_t ___version_3;
	// System.Int32 System.Collections.Generic.Dictionary`2::freeList
	int32_t ___freeList_4;
	// System.Int32 System.Collections.Generic.Dictionary`2::freeCount
	int32_t ___freeCount_5;
	// System.Collections.Generic.IEqualityComparer`1<TKey> System.Collections.Generic.Dictionary`2::comparer
	RuntimeObject* ___comparer_6;
	// System.Collections.Generic.Dictionary`2_KeyCollection<TKey,TValue> System.Collections.Generic.Dictionary`2::keys
	KeyCollection_t2B6B73D918507DC5D3BA824CBABBF5B54F1F0FEB * ___keys_7;
	// System.Collections.Generic.Dictionary`2_ValueCollection<TKey,TValue> System.Collections.Generic.Dictionary`2::values
	ValueCollection_tAFA4AA41F3ED39FB195B3986B51B08ABDEBA68FC * ___values_8;
	// System.Object System.Collections.Generic.Dictionary`2::_syncRoot
	RuntimeObject * ____syncRoot_9;

public:
	inline static int32_t get_offset_of_buckets_0() { return static_cast<int32_t>(offsetof(Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64, ___buckets_0)); }
	inline Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* get_buckets_0() const { return ___buckets_0; }
	inline Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83** get_address_of_buckets_0() { return &___buckets_0; }
	inline void set_buckets_0(Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* value)
	{
		___buckets_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___buckets_0), (void*)value);
	}

	inline static int32_t get_offset_of_entries_1() { return static_cast<int32_t>(offsetof(Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64, ___entries_1)); }
	inline EntryU5BU5D_tCC3FFE930F278956DCD8BE65A7737B74C9A4E9DE* get_entries_1() const { return ___entries_1; }
	inline EntryU5BU5D_tCC3FFE930F278956DCD8BE65A7737B74C9A4E9DE** get_address_of_entries_1() { return &___entries_1; }
	inline void set_entries_1(EntryU5BU5D_tCC3FFE930F278956DCD8BE65A7737B74C9A4E9DE* value)
	{
		___entries_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___entries_1), (void*)value);
	}

	inline static int32_t get_offset_of_count_2() { return static_cast<int32_t>(offsetof(Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64, ___count_2)); }
	inline int32_t get_count_2() const { return ___count_2; }
	inline int32_t* get_address_of_count_2() { return &___count_2; }
	inline void set_count_2(int32_t value)
	{
		___count_2 = value;
	}

	inline static int32_t get_offset_of_version_3() { return static_cast<int32_t>(offsetof(Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64, ___version_3)); }
	inline int32_t get_version_3() const { return ___version_3; }
	inline int32_t* get_address_of_version_3() { return &___version_3; }
	inline void set_version_3(int32_t value)
	{
		___version_3 = value;
	}

	inline static int32_t get_offset_of_freeList_4() { return static_cast<int32_t>(offsetof(Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64, ___freeList_4)); }
	inline int32_t get_freeList_4() const { return ___freeList_4; }
	inline int32_t* get_address_of_freeList_4() { return &___freeList_4; }
	inline void set_freeList_4(int32_t value)
	{
		___freeList_4 = value;
	}

	inline static int32_t get_offset_of_freeCount_5() { return static_cast<int32_t>(offsetof(Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64, ___freeCount_5)); }
	inline int32_t get_freeCount_5() const { return ___freeCount_5; }
	inline int32_t* get_address_of_freeCount_5() { return &___freeCount_5; }
	inline void set_freeCount_5(int32_t value)
	{
		___freeCount_5 = value;
	}

	inline static int32_t get_offset_of_comparer_6() { return static_cast<int32_t>(offsetof(Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64, ___comparer_6)); }
	inline RuntimeObject* get_comparer_6() const { return ___comparer_6; }
	inline RuntimeObject** get_address_of_comparer_6() { return &___comparer_6; }
	inline void set_comparer_6(RuntimeObject* value)
	{
		___comparer_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___comparer_6), (void*)value);
	}

	inline static int32_t get_offset_of_keys_7() { return static_cast<int32_t>(offsetof(Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64, ___keys_7)); }
	inline KeyCollection_t2B6B73D918507DC5D3BA824CBABBF5B54F1F0FEB * get_keys_7() const { return ___keys_7; }
	inline KeyCollection_t2B6B73D918507DC5D3BA824CBABBF5B54F1F0FEB ** get_address_of_keys_7() { return &___keys_7; }
	inline void set_keys_7(KeyCollection_t2B6B73D918507DC5D3BA824CBABBF5B54F1F0FEB * value)
	{
		___keys_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___keys_7), (void*)value);
	}

	inline static int32_t get_offset_of_values_8() { return static_cast<int32_t>(offsetof(Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64, ___values_8)); }
	inline ValueCollection_tAFA4AA41F3ED39FB195B3986B51B08ABDEBA68FC * get_values_8() const { return ___values_8; }
	inline ValueCollection_tAFA4AA41F3ED39FB195B3986B51B08ABDEBA68FC ** get_address_of_values_8() { return &___values_8; }
	inline void set_values_8(ValueCollection_tAFA4AA41F3ED39FB195B3986B51B08ABDEBA68FC * value)
	{
		___values_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___values_8), (void*)value);
	}

	inline static int32_t get_offset_of__syncRoot_9() { return static_cast<int32_t>(offsetof(Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64, ____syncRoot_9)); }
	inline RuntimeObject * get__syncRoot_9() const { return ____syncRoot_9; }
	inline RuntimeObject ** get_address_of__syncRoot_9() { return &____syncRoot_9; }
	inline void set__syncRoot_9(RuntimeObject * value)
	{
		____syncRoot_9 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____syncRoot_9), (void*)value);
	}
};


// System.Collections.Generic.Dictionary`2<System.Tuple`2<Microsoft.MixedReality.Toolkit.Input.KeyBinding_KeyType,System.Int32>,System.Int32>
struct  Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB  : public RuntimeObject
{
public:
	// System.Int32[] System.Collections.Generic.Dictionary`2::buckets
	Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* ___buckets_0;
	// System.Collections.Generic.Dictionary`2_Entry<TKey,TValue>[] System.Collections.Generic.Dictionary`2::entries
	EntryU5BU5D_tAE4BD942D1AA89BBDD0C5F4280EC85C33EB60DF3* ___entries_1;
	// System.Int32 System.Collections.Generic.Dictionary`2::count
	int32_t ___count_2;
	// System.Int32 System.Collections.Generic.Dictionary`2::version
	int32_t ___version_3;
	// System.Int32 System.Collections.Generic.Dictionary`2::freeList
	int32_t ___freeList_4;
	// System.Int32 System.Collections.Generic.Dictionary`2::freeCount
	int32_t ___freeCount_5;
	// System.Collections.Generic.IEqualityComparer`1<TKey> System.Collections.Generic.Dictionary`2::comparer
	RuntimeObject* ___comparer_6;
	// System.Collections.Generic.Dictionary`2_KeyCollection<TKey,TValue> System.Collections.Generic.Dictionary`2::keys
	KeyCollection_t1535CF17E8BED19B5FDC0C264527A148331019B1 * ___keys_7;
	// System.Collections.Generic.Dictionary`2_ValueCollection<TKey,TValue> System.Collections.Generic.Dictionary`2::values
	ValueCollection_t3C7008B2E602BA0404280CBD1E1A3D1E565EE9DC * ___values_8;
	// System.Object System.Collections.Generic.Dictionary`2::_syncRoot
	RuntimeObject * ____syncRoot_9;

public:
	inline static int32_t get_offset_of_buckets_0() { return static_cast<int32_t>(offsetof(Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB, ___buckets_0)); }
	inline Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* get_buckets_0() const { return ___buckets_0; }
	inline Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83** get_address_of_buckets_0() { return &___buckets_0; }
	inline void set_buckets_0(Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* value)
	{
		___buckets_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___buckets_0), (void*)value);
	}

	inline static int32_t get_offset_of_entries_1() { return static_cast<int32_t>(offsetof(Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB, ___entries_1)); }
	inline EntryU5BU5D_tAE4BD942D1AA89BBDD0C5F4280EC85C33EB60DF3* get_entries_1() const { return ___entries_1; }
	inline EntryU5BU5D_tAE4BD942D1AA89BBDD0C5F4280EC85C33EB60DF3** get_address_of_entries_1() { return &___entries_1; }
	inline void set_entries_1(EntryU5BU5D_tAE4BD942D1AA89BBDD0C5F4280EC85C33EB60DF3* value)
	{
		___entries_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___entries_1), (void*)value);
	}

	inline static int32_t get_offset_of_count_2() { return static_cast<int32_t>(offsetof(Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB, ___count_2)); }
	inline int32_t get_count_2() const { return ___count_2; }
	inline int32_t* get_address_of_count_2() { return &___count_2; }
	inline void set_count_2(int32_t value)
	{
		___count_2 = value;
	}

	inline static int32_t get_offset_of_version_3() { return static_cast<int32_t>(offsetof(Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB, ___version_3)); }
	inline int32_t get_version_3() const { return ___version_3; }
	inline int32_t* get_address_of_version_3() { return &___version_3; }
	inline void set_version_3(int32_t value)
	{
		___version_3 = value;
	}

	inline static int32_t get_offset_of_freeList_4() { return static_cast<int32_t>(offsetof(Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB, ___freeList_4)); }
	inline int32_t get_freeList_4() const { return ___freeList_4; }
	inline int32_t* get_address_of_freeList_4() { return &___freeList_4; }
	inline void set_freeList_4(int32_t value)
	{
		___freeList_4 = value;
	}

	inline static int32_t get_offset_of_freeCount_5() { return static_cast<int32_t>(offsetof(Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB, ___freeCount_5)); }
	inline int32_t get_freeCount_5() const { return ___freeCount_5; }
	inline int32_t* get_address_of_freeCount_5() { return &___freeCount_5; }
	inline void set_freeCount_5(int32_t value)
	{
		___freeCount_5 = value;
	}

	inline static int32_t get_offset_of_comparer_6() { return static_cast<int32_t>(offsetof(Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB, ___comparer_6)); }
	inline RuntimeObject* get_comparer_6() const { return ___comparer_6; }
	inline RuntimeObject** get_address_of_comparer_6() { return &___comparer_6; }
	inline void set_comparer_6(RuntimeObject* value)
	{
		___comparer_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___comparer_6), (void*)value);
	}

	inline static int32_t get_offset_of_keys_7() { return static_cast<int32_t>(offsetof(Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB, ___keys_7)); }
	inline KeyCollection_t1535CF17E8BED19B5FDC0C264527A148331019B1 * get_keys_7() const { return ___keys_7; }
	inline KeyCollection_t1535CF17E8BED19B5FDC0C264527A148331019B1 ** get_address_of_keys_7() { return &___keys_7; }
	inline void set_keys_7(KeyCollection_t1535CF17E8BED19B5FDC0C264527A148331019B1 * value)
	{
		___keys_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___keys_7), (void*)value);
	}

	inline static int32_t get_offset_of_values_8() { return static_cast<int32_t>(offsetof(Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB, ___values_8)); }
	inline ValueCollection_t3C7008B2E602BA0404280CBD1E1A3D1E565EE9DC * get_values_8() const { return ___values_8; }
	inline ValueCollection_t3C7008B2E602BA0404280CBD1E1A3D1E565EE9DC ** get_address_of_values_8() { return &___values_8; }
	inline void set_values_8(ValueCollection_t3C7008B2E602BA0404280CBD1E1A3D1E565EE9DC * value)
	{
		___values_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___values_8), (void*)value);
	}

	inline static int32_t get_offset_of__syncRoot_9() { return static_cast<int32_t>(offsetof(Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB, ____syncRoot_9)); }
	inline RuntimeObject * get__syncRoot_9() const { return ____syncRoot_9; }
	inline RuntimeObject ** get_address_of__syncRoot_9() { return &____syncRoot_9; }
	inline void set__syncRoot_9(RuntimeObject * value)
	{
		____syncRoot_9 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____syncRoot_9), (void*)value);
	}
};


// System.Collections.Generic.List`1<System.String>
struct  List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3  : public RuntimeObject
{
public:
	// T[] System.Collections.Generic.List`1::_items
	StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* ____items_1;
	// System.Int32 System.Collections.Generic.List`1::_size
	int32_t ____size_2;
	// System.Int32 System.Collections.Generic.List`1::_version
	int32_t ____version_3;
	// System.Object System.Collections.Generic.List`1::_syncRoot
	RuntimeObject * ____syncRoot_4;

public:
	inline static int32_t get_offset_of__items_1() { return static_cast<int32_t>(offsetof(List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3, ____items_1)); }
	inline StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* get__items_1() const { return ____items_1; }
	inline StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E** get_address_of__items_1() { return &____items_1; }
	inline void set__items_1(StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* value)
	{
		____items_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____items_1), (void*)value);
	}

	inline static int32_t get_offset_of__size_2() { return static_cast<int32_t>(offsetof(List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3, ____size_2)); }
	inline int32_t get__size_2() const { return ____size_2; }
	inline int32_t* get_address_of__size_2() { return &____size_2; }
	inline void set__size_2(int32_t value)
	{
		____size_2 = value;
	}

	inline static int32_t get_offset_of__version_3() { return static_cast<int32_t>(offsetof(List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3, ____version_3)); }
	inline int32_t get__version_3() const { return ____version_3; }
	inline int32_t* get_address_of__version_3() { return &____version_3; }
	inline void set__version_3(int32_t value)
	{
		____version_3 = value;
	}

	inline static int32_t get_offset_of__syncRoot_4() { return static_cast<int32_t>(offsetof(List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3, ____syncRoot_4)); }
	inline RuntimeObject * get__syncRoot_4() const { return ____syncRoot_4; }
	inline RuntimeObject ** get_address_of__syncRoot_4() { return &____syncRoot_4; }
	inline void set__syncRoot_4(RuntimeObject * value)
	{
		____syncRoot_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____syncRoot_4), (void*)value);
	}
};

struct List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3_StaticFields
{
public:
	// T[] System.Collections.Generic.List`1::_emptyArray
	StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* ____emptyArray_5;

public:
	inline static int32_t get_offset_of__emptyArray_5() { return static_cast<int32_t>(offsetof(List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3_StaticFields, ____emptyArray_5)); }
	inline StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* get__emptyArray_5() const { return ____emptyArray_5; }
	inline StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E** get_address_of__emptyArray_5() { return &____emptyArray_5; }
	inline void set__emptyArray_5(StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* value)
	{
		____emptyArray_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____emptyArray_5), (void*)value);
	}
};


// System.Reflection.MemberInfo
struct  MemberInfo_t  : public RuntimeObject
{
public:

public:
};


// System.String
struct  String_t  : public RuntimeObject
{
public:
	// System.Int32 System.String::m_stringLength
	int32_t ___m_stringLength_0;
	// System.Char System.String::m_firstChar
	Il2CppChar ___m_firstChar_1;

public:
	inline static int32_t get_offset_of_m_stringLength_0() { return static_cast<int32_t>(offsetof(String_t, ___m_stringLength_0)); }
	inline int32_t get_m_stringLength_0() const { return ___m_stringLength_0; }
	inline int32_t* get_address_of_m_stringLength_0() { return &___m_stringLength_0; }
	inline void set_m_stringLength_0(int32_t value)
	{
		___m_stringLength_0 = value;
	}

	inline static int32_t get_offset_of_m_firstChar_1() { return static_cast<int32_t>(offsetof(String_t, ___m_firstChar_1)); }
	inline Il2CppChar get_m_firstChar_1() const { return ___m_firstChar_1; }
	inline Il2CppChar* get_address_of_m_firstChar_1() { return &___m_firstChar_1; }
	inline void set_m_firstChar_1(Il2CppChar value)
	{
		___m_firstChar_1 = value;
	}
};

struct String_t_StaticFields
{
public:
	// System.String System.String::Empty
	String_t* ___Empty_5;

public:
	inline static int32_t get_offset_of_Empty_5() { return static_cast<int32_t>(offsetof(String_t_StaticFields, ___Empty_5)); }
	inline String_t* get_Empty_5() const { return ___Empty_5; }
	inline String_t** get_address_of_Empty_5() { return &___Empty_5; }
	inline void set_Empty_5(String_t* value)
	{
		___Empty_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___Empty_5), (void*)value);
	}
};


// System.ValueType
struct  ValueType_t4D0C27076F7C36E76190FB3328E232BCB1CD1FFF  : public RuntimeObject
{
public:

public:
};

// Native definition for P/Invoke marshalling of System.ValueType
struct ValueType_t4D0C27076F7C36E76190FB3328E232BCB1CD1FFF_marshaled_pinvoke
{
};
// Native definition for COM marshalling of System.ValueType
struct ValueType_t4D0C27076F7C36E76190FB3328E232BCB1CD1FFF_marshaled_com
{
};

// <PrivateImplementationDetails>___StaticArrayInitTypeSizeU3D20
struct  __StaticArrayInitTypeSizeU3D20_t61B73AC9C8C13E1C63E537737789BCB471C794DD 
{
public:
	union
	{
		struct
		{
			union
			{
			};
		};
		uint8_t __StaticArrayInitTypeSizeU3D20_t61B73AC9C8C13E1C63E537737789BCB471C794DD__padding[20];
	};

public:
};


// System.Boolean
struct  Boolean_tB53F6830F670160873277339AA58F15CAED4399C 
{
public:
	// System.Boolean System.Boolean::m_value
	bool ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Boolean_tB53F6830F670160873277339AA58F15CAED4399C, ___m_value_0)); }
	inline bool get_m_value_0() const { return ___m_value_0; }
	inline bool* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(bool value)
	{
		___m_value_0 = value;
	}
};

struct Boolean_tB53F6830F670160873277339AA58F15CAED4399C_StaticFields
{
public:
	// System.String System.Boolean::TrueString
	String_t* ___TrueString_5;
	// System.String System.Boolean::FalseString
	String_t* ___FalseString_6;

public:
	inline static int32_t get_offset_of_TrueString_5() { return static_cast<int32_t>(offsetof(Boolean_tB53F6830F670160873277339AA58F15CAED4399C_StaticFields, ___TrueString_5)); }
	inline String_t* get_TrueString_5() const { return ___TrueString_5; }
	inline String_t** get_address_of_TrueString_5() { return &___TrueString_5; }
	inline void set_TrueString_5(String_t* value)
	{
		___TrueString_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___TrueString_5), (void*)value);
	}

	inline static int32_t get_offset_of_FalseString_6() { return static_cast<int32_t>(offsetof(Boolean_tB53F6830F670160873277339AA58F15CAED4399C_StaticFields, ___FalseString_6)); }
	inline String_t* get_FalseString_6() const { return ___FalseString_6; }
	inline String_t** get_address_of_FalseString_6() { return &___FalseString_6; }
	inline void set_FalseString_6(String_t* value)
	{
		___FalseString_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___FalseString_6), (void*)value);
	}
};


// System.Enum
struct  Enum_t2AF27C02B8653AE29442467390005ABC74D8F521  : public ValueType_t4D0C27076F7C36E76190FB3328E232BCB1CD1FFF
{
public:

public:
};

struct Enum_t2AF27C02B8653AE29442467390005ABC74D8F521_StaticFields
{
public:
	// System.Char[] System.Enum::enumSeperatorCharArray
	CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* ___enumSeperatorCharArray_0;

public:
	inline static int32_t get_offset_of_enumSeperatorCharArray_0() { return static_cast<int32_t>(offsetof(Enum_t2AF27C02B8653AE29442467390005ABC74D8F521_StaticFields, ___enumSeperatorCharArray_0)); }
	inline CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* get_enumSeperatorCharArray_0() const { return ___enumSeperatorCharArray_0; }
	inline CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2** get_address_of_enumSeperatorCharArray_0() { return &___enumSeperatorCharArray_0; }
	inline void set_enumSeperatorCharArray_0(CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* value)
	{
		___enumSeperatorCharArray_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___enumSeperatorCharArray_0), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of System.Enum
struct Enum_t2AF27C02B8653AE29442467390005ABC74D8F521_marshaled_pinvoke
{
};
// Native definition for COM marshalling of System.Enum
struct Enum_t2AF27C02B8653AE29442467390005ABC74D8F521_marshaled_com
{
};

// System.Int32
struct  Int32_t585191389E07734F19F3156FF88FB3EF4800D102 
{
public:
	// System.Int32 System.Int32::m_value
	int32_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Int32_t585191389E07734F19F3156FF88FB3EF4800D102, ___m_value_0)); }
	inline int32_t get_m_value_0() const { return ___m_value_0; }
	inline int32_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(int32_t value)
	{
		___m_value_0 = value;
	}
};


// System.IntPtr
struct  IntPtr_t 
{
public:
	// System.Void* System.IntPtr::m_value
	void* ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(IntPtr_t, ___m_value_0)); }
	inline void* get_m_value_0() const { return ___m_value_0; }
	inline void** get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(void* value)
	{
		___m_value_0 = value;
	}
};

struct IntPtr_t_StaticFields
{
public:
	// System.IntPtr System.IntPtr::Zero
	intptr_t ___Zero_1;

public:
	inline static int32_t get_offset_of_Zero_1() { return static_cast<int32_t>(offsetof(IntPtr_t_StaticFields, ___Zero_1)); }
	inline intptr_t get_Zero_1() const { return ___Zero_1; }
	inline intptr_t* get_address_of_Zero_1() { return &___Zero_1; }
	inline void set_Zero_1(intptr_t value)
	{
		___Zero_1 = value;
	}
};


// System.Nullable`1<System.Int32>
struct  Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB 
{
public:
	// T System.Nullable`1::value
	int32_t ___value_0;
	// System.Boolean System.Nullable`1::has_value
	bool ___has_value_1;

public:
	inline static int32_t get_offset_of_value_0() { return static_cast<int32_t>(offsetof(Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB, ___value_0)); }
	inline int32_t get_value_0() const { return ___value_0; }
	inline int32_t* get_address_of_value_0() { return &___value_0; }
	inline void set_value_0(int32_t value)
	{
		___value_0 = value;
	}

	inline static int32_t get_offset_of_has_value_1() { return static_cast<int32_t>(offsetof(Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB, ___has_value_1)); }
	inline bool get_has_value_1() const { return ___has_value_1; }
	inline bool* get_address_of_has_value_1() { return &___has_value_1; }
	inline void set_has_value_1(bool value)
	{
		___has_value_1 = value;
	}
};


// System.Single
struct  Single_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1 
{
public:
	// System.Single System.Single::m_value
	float ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Single_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1, ___m_value_0)); }
	inline float get_m_value_0() const { return ___m_value_0; }
	inline float* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(float value)
	{
		___m_value_0 = value;
	}
};


// System.UInt32
struct  UInt32_t4980FA09003AFAAB5A6E361BA2748EA9A005709B 
{
public:
	// System.UInt32 System.UInt32::m_value
	uint32_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(UInt32_t4980FA09003AFAAB5A6E361BA2748EA9A005709B, ___m_value_0)); }
	inline uint32_t get_m_value_0() const { return ___m_value_0; }
	inline uint32_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(uint32_t value)
	{
		___m_value_0 = value;
	}
};


// System.Void
struct  Void_t22962CB4C05B1D89B55A6E1139F0E87A90987017 
{
public:
	union
	{
		struct
		{
		};
		uint8_t Void_t22962CB4C05B1D89B55A6E1139F0E87A90987017__padding[1];
	};

public:
};


// UnityEngine.Quaternion
struct  Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 
{
public:
	// System.Single UnityEngine.Quaternion::x
	float ___x_0;
	// System.Single UnityEngine.Quaternion::y
	float ___y_1;
	// System.Single UnityEngine.Quaternion::z
	float ___z_2;
	// System.Single UnityEngine.Quaternion::w
	float ___w_3;

public:
	inline static int32_t get_offset_of_x_0() { return static_cast<int32_t>(offsetof(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357, ___x_0)); }
	inline float get_x_0() const { return ___x_0; }
	inline float* get_address_of_x_0() { return &___x_0; }
	inline void set_x_0(float value)
	{
		___x_0 = value;
	}

	inline static int32_t get_offset_of_y_1() { return static_cast<int32_t>(offsetof(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357, ___y_1)); }
	inline float get_y_1() const { return ___y_1; }
	inline float* get_address_of_y_1() { return &___y_1; }
	inline void set_y_1(float value)
	{
		___y_1 = value;
	}

	inline static int32_t get_offset_of_z_2() { return static_cast<int32_t>(offsetof(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357, ___z_2)); }
	inline float get_z_2() const { return ___z_2; }
	inline float* get_address_of_z_2() { return &___z_2; }
	inline void set_z_2(float value)
	{
		___z_2 = value;
	}

	inline static int32_t get_offset_of_w_3() { return static_cast<int32_t>(offsetof(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357, ___w_3)); }
	inline float get_w_3() const { return ___w_3; }
	inline float* get_address_of_w_3() { return &___w_3; }
	inline void set_w_3(float value)
	{
		___w_3 = value;
	}
};

struct Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_StaticFields
{
public:
	// UnityEngine.Quaternion UnityEngine.Quaternion::identityQuaternion
	Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___identityQuaternion_4;

public:
	inline static int32_t get_offset_of_identityQuaternion_4() { return static_cast<int32_t>(offsetof(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_StaticFields, ___identityQuaternion_4)); }
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  get_identityQuaternion_4() const { return ___identityQuaternion_4; }
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 * get_address_of_identityQuaternion_4() { return &___identityQuaternion_4; }
	inline void set_identityQuaternion_4(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  value)
	{
		___identityQuaternion_4 = value;
	}
};


// UnityEngine.Vector2
struct  Vector2_tA85D2DD88578276CA8A8796756458277E72D073D 
{
public:
	// System.Single UnityEngine.Vector2::x
	float ___x_0;
	// System.Single UnityEngine.Vector2::y
	float ___y_1;

public:
	inline static int32_t get_offset_of_x_0() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D, ___x_0)); }
	inline float get_x_0() const { return ___x_0; }
	inline float* get_address_of_x_0() { return &___x_0; }
	inline void set_x_0(float value)
	{
		___x_0 = value;
	}

	inline static int32_t get_offset_of_y_1() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D, ___y_1)); }
	inline float get_y_1() const { return ___y_1; }
	inline float* get_address_of_y_1() { return &___y_1; }
	inline void set_y_1(float value)
	{
		___y_1 = value;
	}
};

struct Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields
{
public:
	// UnityEngine.Vector2 UnityEngine.Vector2::zeroVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___zeroVector_2;
	// UnityEngine.Vector2 UnityEngine.Vector2::oneVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___oneVector_3;
	// UnityEngine.Vector2 UnityEngine.Vector2::upVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___upVector_4;
	// UnityEngine.Vector2 UnityEngine.Vector2::downVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___downVector_5;
	// UnityEngine.Vector2 UnityEngine.Vector2::leftVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___leftVector_6;
	// UnityEngine.Vector2 UnityEngine.Vector2::rightVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___rightVector_7;
	// UnityEngine.Vector2 UnityEngine.Vector2::positiveInfinityVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___positiveInfinityVector_8;
	// UnityEngine.Vector2 UnityEngine.Vector2::negativeInfinityVector
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___negativeInfinityVector_9;

public:
	inline static int32_t get_offset_of_zeroVector_2() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___zeroVector_2)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_zeroVector_2() const { return ___zeroVector_2; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_zeroVector_2() { return &___zeroVector_2; }
	inline void set_zeroVector_2(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___zeroVector_2 = value;
	}

	inline static int32_t get_offset_of_oneVector_3() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___oneVector_3)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_oneVector_3() const { return ___oneVector_3; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_oneVector_3() { return &___oneVector_3; }
	inline void set_oneVector_3(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___oneVector_3 = value;
	}

	inline static int32_t get_offset_of_upVector_4() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___upVector_4)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_upVector_4() const { return ___upVector_4; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_upVector_4() { return &___upVector_4; }
	inline void set_upVector_4(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___upVector_4 = value;
	}

	inline static int32_t get_offset_of_downVector_5() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___downVector_5)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_downVector_5() const { return ___downVector_5; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_downVector_5() { return &___downVector_5; }
	inline void set_downVector_5(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___downVector_5 = value;
	}

	inline static int32_t get_offset_of_leftVector_6() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___leftVector_6)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_leftVector_6() const { return ___leftVector_6; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_leftVector_6() { return &___leftVector_6; }
	inline void set_leftVector_6(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___leftVector_6 = value;
	}

	inline static int32_t get_offset_of_rightVector_7() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___rightVector_7)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_rightVector_7() const { return ___rightVector_7; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_rightVector_7() { return &___rightVector_7; }
	inline void set_rightVector_7(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___rightVector_7 = value;
	}

	inline static int32_t get_offset_of_positiveInfinityVector_8() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___positiveInfinityVector_8)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_positiveInfinityVector_8() const { return ___positiveInfinityVector_8; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_positiveInfinityVector_8() { return &___positiveInfinityVector_8; }
	inline void set_positiveInfinityVector_8(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___positiveInfinityVector_8 = value;
	}

	inline static int32_t get_offset_of_negativeInfinityVector_9() { return static_cast<int32_t>(offsetof(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_StaticFields, ___negativeInfinityVector_9)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_negativeInfinityVector_9() const { return ___negativeInfinityVector_9; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_negativeInfinityVector_9() { return &___negativeInfinityVector_9; }
	inline void set_negativeInfinityVector_9(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___negativeInfinityVector_9 = value;
	}
};


// UnityEngine.Vector3
struct  Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 
{
public:
	// System.Single UnityEngine.Vector3::x
	float ___x_2;
	// System.Single UnityEngine.Vector3::y
	float ___y_3;
	// System.Single UnityEngine.Vector3::z
	float ___z_4;

public:
	inline static int32_t get_offset_of_x_2() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720, ___x_2)); }
	inline float get_x_2() const { return ___x_2; }
	inline float* get_address_of_x_2() { return &___x_2; }
	inline void set_x_2(float value)
	{
		___x_2 = value;
	}

	inline static int32_t get_offset_of_y_3() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720, ___y_3)); }
	inline float get_y_3() const { return ___y_3; }
	inline float* get_address_of_y_3() { return &___y_3; }
	inline void set_y_3(float value)
	{
		___y_3 = value;
	}

	inline static int32_t get_offset_of_z_4() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720, ___z_4)); }
	inline float get_z_4() const { return ___z_4; }
	inline float* get_address_of_z_4() { return &___z_4; }
	inline void set_z_4(float value)
	{
		___z_4 = value;
	}
};

struct Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields
{
public:
	// UnityEngine.Vector3 UnityEngine.Vector3::zeroVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___zeroVector_5;
	// UnityEngine.Vector3 UnityEngine.Vector3::oneVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___oneVector_6;
	// UnityEngine.Vector3 UnityEngine.Vector3::upVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___upVector_7;
	// UnityEngine.Vector3 UnityEngine.Vector3::downVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___downVector_8;
	// UnityEngine.Vector3 UnityEngine.Vector3::leftVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___leftVector_9;
	// UnityEngine.Vector3 UnityEngine.Vector3::rightVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___rightVector_10;
	// UnityEngine.Vector3 UnityEngine.Vector3::forwardVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___forwardVector_11;
	// UnityEngine.Vector3 UnityEngine.Vector3::backVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___backVector_12;
	// UnityEngine.Vector3 UnityEngine.Vector3::positiveInfinityVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___positiveInfinityVector_13;
	// UnityEngine.Vector3 UnityEngine.Vector3::negativeInfinityVector
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___negativeInfinityVector_14;

public:
	inline static int32_t get_offset_of_zeroVector_5() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___zeroVector_5)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_zeroVector_5() const { return ___zeroVector_5; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_zeroVector_5() { return &___zeroVector_5; }
	inline void set_zeroVector_5(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___zeroVector_5 = value;
	}

	inline static int32_t get_offset_of_oneVector_6() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___oneVector_6)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_oneVector_6() const { return ___oneVector_6; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_oneVector_6() { return &___oneVector_6; }
	inline void set_oneVector_6(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___oneVector_6 = value;
	}

	inline static int32_t get_offset_of_upVector_7() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___upVector_7)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_upVector_7() const { return ___upVector_7; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_upVector_7() { return &___upVector_7; }
	inline void set_upVector_7(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___upVector_7 = value;
	}

	inline static int32_t get_offset_of_downVector_8() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___downVector_8)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_downVector_8() const { return ___downVector_8; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_downVector_8() { return &___downVector_8; }
	inline void set_downVector_8(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___downVector_8 = value;
	}

	inline static int32_t get_offset_of_leftVector_9() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___leftVector_9)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_leftVector_9() const { return ___leftVector_9; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_leftVector_9() { return &___leftVector_9; }
	inline void set_leftVector_9(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___leftVector_9 = value;
	}

	inline static int32_t get_offset_of_rightVector_10() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___rightVector_10)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_rightVector_10() const { return ___rightVector_10; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_rightVector_10() { return &___rightVector_10; }
	inline void set_rightVector_10(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___rightVector_10 = value;
	}

	inline static int32_t get_offset_of_forwardVector_11() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___forwardVector_11)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_forwardVector_11() const { return ___forwardVector_11; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_forwardVector_11() { return &___forwardVector_11; }
	inline void set_forwardVector_11(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___forwardVector_11 = value;
	}

	inline static int32_t get_offset_of_backVector_12() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___backVector_12)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_backVector_12() const { return ___backVector_12; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_backVector_12() { return &___backVector_12; }
	inline void set_backVector_12(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___backVector_12 = value;
	}

	inline static int32_t get_offset_of_positiveInfinityVector_13() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___positiveInfinityVector_13)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_positiveInfinityVector_13() const { return ___positiveInfinityVector_13; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_positiveInfinityVector_13() { return &___positiveInfinityVector_13; }
	inline void set_positiveInfinityVector_13(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___positiveInfinityVector_13 = value;
	}

	inline static int32_t get_offset_of_negativeInfinityVector_14() { return static_cast<int32_t>(offsetof(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_StaticFields, ___negativeInfinityVector_14)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_negativeInfinityVector_14() const { return ___negativeInfinityVector_14; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_negativeInfinityVector_14() { return &___negativeInfinityVector_14; }
	inline void set_negativeInfinityVector_14(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___negativeInfinityVector_14 = value;
	}
};


// <PrivateImplementationDetails>
struct  U3CPrivateImplementationDetailsU3E_t5D7196C8D3A7E05A50169A365F5A7B3B92600D14  : public RuntimeObject
{
public:

public:
};

struct U3CPrivateImplementationDetailsU3E_t5D7196C8D3A7E05A50169A365F5A7B3B92600D14_StaticFields
{
public:
	// <PrivateImplementationDetails>___StaticArrayInitTypeSizeU3D20 <PrivateImplementationDetails>::6AF7EBB4A5EF5D7478981B4AA0BAD37788AAB1ED
	__StaticArrayInitTypeSizeU3D20_t61B73AC9C8C13E1C63E537737789BCB471C794DD  ___6AF7EBB4A5EF5D7478981B4AA0BAD37788AAB1ED_0;

public:
	inline static int32_t get_offset_of_U36AF7EBB4A5EF5D7478981B4AA0BAD37788AAB1ED_0() { return static_cast<int32_t>(offsetof(U3CPrivateImplementationDetailsU3E_t5D7196C8D3A7E05A50169A365F5A7B3B92600D14_StaticFields, ___6AF7EBB4A5EF5D7478981B4AA0BAD37788AAB1ED_0)); }
	inline __StaticArrayInitTypeSizeU3D20_t61B73AC9C8C13E1C63E537737789BCB471C794DD  get_U36AF7EBB4A5EF5D7478981B4AA0BAD37788AAB1ED_0() const { return ___6AF7EBB4A5EF5D7478981B4AA0BAD37788AAB1ED_0; }
	inline __StaticArrayInitTypeSizeU3D20_t61B73AC9C8C13E1C63E537737789BCB471C794DD * get_address_of_U36AF7EBB4A5EF5D7478981B4AA0BAD37788AAB1ED_0() { return &___6AF7EBB4A5EF5D7478981B4AA0BAD37788AAB1ED_0; }
	inline void set_U36AF7EBB4A5EF5D7478981B4AA0BAD37788AAB1ED_0(__StaticArrayInitTypeSizeU3D20_t61B73AC9C8C13E1C63E537737789BCB471C794DD  value)
	{
		___6AF7EBB4A5EF5D7478981B4AA0BAD37788AAB1ED_0 = value;
	}
};


// Microsoft.MixedReality.Toolkit.Input.DeviceInputType
struct  DeviceInputType_t358986D22B64DCDBE1EC628624BB92B25DED5E31 
{
public:
	// System.Int32 Microsoft.MixedReality.Toolkit.Input.DeviceInputType::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(DeviceInputType_t358986D22B64DCDBE1EC628624BB92B25DED5E31, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// Microsoft.MixedReality.Toolkit.Input.GestureInputType
struct  GestureInputType_tE0BF82A452F97F80C699F9D207127F34EEB261CF 
{
public:
	// System.Int32 Microsoft.MixedReality.Toolkit.Input.GestureInputType::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(GestureInputType_tE0BF82A452F97F80C699F9D207127F34EEB261CF, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// Microsoft.MixedReality.Toolkit.Input.HandSimulationMode
struct  HandSimulationMode_t832EE1D59F2C4C5A884C478C59FB38AB3DA9C762 
{
public:
	// System.Int32 Microsoft.MixedReality.Toolkit.Input.HandSimulationMode::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(HandSimulationMode_t832EE1D59F2C4C5A884C478C59FB38AB3DA9C762, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// Microsoft.MixedReality.Toolkit.Input.InputSimulationControlMode
struct  InputSimulationControlMode_t17D676A28E3E944B01D8DF7D018A8F0F17FD1648 
{
public:
	// System.Int32 Microsoft.MixedReality.Toolkit.Input.InputSimulationControlMode::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(InputSimulationControlMode_t17D676A28E3E944B01D8DF7D018A8F0F17FD1648, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// Microsoft.MixedReality.Toolkit.Input.KeyBinding_KeyType
struct  KeyType_t63A0EC9B1C9653881B95DF409080C7FB24760D72 
{
public:
	// System.Int32 Microsoft.MixedReality.Toolkit.Input.KeyBinding_KeyType::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(KeyType_t63A0EC9B1C9653881B95DF409080C7FB24760D72, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// Microsoft.MixedReality.Toolkit.Input.KeyBinding_MouseButton
struct  MouseButton_t4174FC057A73B1ECBC9603C3AF8AF87E964E719E 
{
public:
	// System.Int32 Microsoft.MixedReality.Toolkit.Input.KeyBinding_MouseButton::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(MouseButton_t4174FC057A73B1ECBC9603C3AF8AF87E964E719E, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// Microsoft.MixedReality.Toolkit.TrackingState
struct  TrackingState_tA4B3C624DF9D6B518A15D682BA0207573D1611EA 
{
public:
	// System.Int32 Microsoft.MixedReality.Toolkit.TrackingState::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(TrackingState_tA4B3C624DF9D6B518A15D682BA0207573D1611EA, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// Microsoft.MixedReality.Toolkit.Utilities.ArticulatedHandPose_GestureId
struct  GestureId_tC7E0E1660275BEE4341D718A2D819A1AEEB7BE62 
{
public:
	// System.Int32 Microsoft.MixedReality.Toolkit.Utilities.ArticulatedHandPose_GestureId::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(GestureId_tC7E0E1660275BEE4341D718A2D819A1AEEB7BE62, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// Microsoft.MixedReality.Toolkit.Utilities.AutoStartBehavior
struct  AutoStartBehavior_t9BBC9C0AE47250C9034F4B386E2D1C6BA21D5839 
{
public:
	// System.Int32 Microsoft.MixedReality.Toolkit.Utilities.AutoStartBehavior::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(AutoStartBehavior_t9BBC9C0AE47250C9034F4B386E2D1C6BA21D5839, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// Microsoft.MixedReality.Toolkit.Utilities.AxisType
struct  AxisType_t45CEF046648179DA1FDF98C495D40AA34823C164 
{
public:
	// System.Int32 Microsoft.MixedReality.Toolkit.Utilities.AxisType::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(AxisType_t45CEF046648179DA1FDF98C495D40AA34823C164, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// Microsoft.MixedReality.Toolkit.Utilities.Handedness
struct  Handedness_tA51C49CA286A1BC201E1680F521639E9AC1165AB 
{
public:
	// System.Byte Microsoft.MixedReality.Toolkit.Utilities.Handedness::value__
	uint8_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(Handedness_tA51C49CA286A1BC201E1680F521639E9AC1165AB, ___value___2)); }
	inline uint8_t get_value___2() const { return ___value___2; }
	inline uint8_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(uint8_t value)
	{
		___value___2 = value;
	}
};


// Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose
struct  MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 
{
public:
	// UnityEngine.Vector3 Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose::position
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___position_1;
	// UnityEngine.Quaternion Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose::rotation
	Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___rotation_2;

public:
	inline static int32_t get_offset_of_position_1() { return static_cast<int32_t>(offsetof(MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45, ___position_1)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_position_1() const { return ___position_1; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_position_1() { return &___position_1; }
	inline void set_position_1(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___position_1 = value;
	}

	inline static int32_t get_offset_of_rotation_2() { return static_cast<int32_t>(offsetof(MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45, ___rotation_2)); }
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  get_rotation_2() const { return ___rotation_2; }
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 * get_address_of_rotation_2() { return &___rotation_2; }
	inline void set_rotation_2(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  value)
	{
		___rotation_2 = value;
	}
};

struct MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45_StaticFields
{
public:
	// Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose::<ZeroIdentity>k__BackingField
	MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  ___U3CZeroIdentityU3Ek__BackingField_0;

public:
	inline static int32_t get_offset_of_U3CZeroIdentityU3Ek__BackingField_0() { return static_cast<int32_t>(offsetof(MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45_StaticFields, ___U3CZeroIdentityU3Ek__BackingField_0)); }
	inline MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  get_U3CZeroIdentityU3Ek__BackingField_0() const { return ___U3CZeroIdentityU3Ek__BackingField_0; }
	inline MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * get_address_of_U3CZeroIdentityU3Ek__BackingField_0() { return &___U3CZeroIdentityU3Ek__BackingField_0; }
	inline void set_U3CZeroIdentityU3Ek__BackingField_0(MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  value)
	{
		___U3CZeroIdentityU3Ek__BackingField_0 = value;
	}
};


// Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint
struct  TrackedHandJoint_tDE2FD40782A5B0C1D39386D6BF70D8A1CCF94E22 
{
public:
	// System.Int32 Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(TrackedHandJoint_tDE2FD40782A5B0C1D39386D6BF70D8A1CCF94E22, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// Microsoft.MixedReality.Toolkit.Windows.Input.WindowsGestureSettings
struct  WindowsGestureSettings_t1876E81B36888DFF13EDC8D13F0509B5253DD430 
{
public:
	// System.Int32 Microsoft.MixedReality.Toolkit.Windows.Input.WindowsGestureSettings::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(WindowsGestureSettings_t1876E81B36888DFF13EDC8D13F0509B5253DD430, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// System.Delegate
struct  Delegate_t  : public RuntimeObject
{
public:
	// System.IntPtr System.Delegate::method_ptr
	Il2CppMethodPointer ___method_ptr_0;
	// System.IntPtr System.Delegate::invoke_impl
	intptr_t ___invoke_impl_1;
	// System.Object System.Delegate::m_target
	RuntimeObject * ___m_target_2;
	// System.IntPtr System.Delegate::method
	intptr_t ___method_3;
	// System.IntPtr System.Delegate::delegate_trampoline
	intptr_t ___delegate_trampoline_4;
	// System.IntPtr System.Delegate::extra_arg
	intptr_t ___extra_arg_5;
	// System.IntPtr System.Delegate::method_code
	intptr_t ___method_code_6;
	// System.Reflection.MethodInfo System.Delegate::method_info
	MethodInfo_t * ___method_info_7;
	// System.Reflection.MethodInfo System.Delegate::original_method_info
	MethodInfo_t * ___original_method_info_8;
	// System.DelegateData System.Delegate::data
	DelegateData_t1BF9F691B56DAE5F8C28C5E084FDE94F15F27BBE * ___data_9;
	// System.Boolean System.Delegate::method_is_virtual
	bool ___method_is_virtual_10;

public:
	inline static int32_t get_offset_of_method_ptr_0() { return static_cast<int32_t>(offsetof(Delegate_t, ___method_ptr_0)); }
	inline Il2CppMethodPointer get_method_ptr_0() const { return ___method_ptr_0; }
	inline Il2CppMethodPointer* get_address_of_method_ptr_0() { return &___method_ptr_0; }
	inline void set_method_ptr_0(Il2CppMethodPointer value)
	{
		___method_ptr_0 = value;
	}

	inline static int32_t get_offset_of_invoke_impl_1() { return static_cast<int32_t>(offsetof(Delegate_t, ___invoke_impl_1)); }
	inline intptr_t get_invoke_impl_1() const { return ___invoke_impl_1; }
	inline intptr_t* get_address_of_invoke_impl_1() { return &___invoke_impl_1; }
	inline void set_invoke_impl_1(intptr_t value)
	{
		___invoke_impl_1 = value;
	}

	inline static int32_t get_offset_of_m_target_2() { return static_cast<int32_t>(offsetof(Delegate_t, ___m_target_2)); }
	inline RuntimeObject * get_m_target_2() const { return ___m_target_2; }
	inline RuntimeObject ** get_address_of_m_target_2() { return &___m_target_2; }
	inline void set_m_target_2(RuntimeObject * value)
	{
		___m_target_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_target_2), (void*)value);
	}

	inline static int32_t get_offset_of_method_3() { return static_cast<int32_t>(offsetof(Delegate_t, ___method_3)); }
	inline intptr_t get_method_3() const { return ___method_3; }
	inline intptr_t* get_address_of_method_3() { return &___method_3; }
	inline void set_method_3(intptr_t value)
	{
		___method_3 = value;
	}

	inline static int32_t get_offset_of_delegate_trampoline_4() { return static_cast<int32_t>(offsetof(Delegate_t, ___delegate_trampoline_4)); }
	inline intptr_t get_delegate_trampoline_4() const { return ___delegate_trampoline_4; }
	inline intptr_t* get_address_of_delegate_trampoline_4() { return &___delegate_trampoline_4; }
	inline void set_delegate_trampoline_4(intptr_t value)
	{
		___delegate_trampoline_4 = value;
	}

	inline static int32_t get_offset_of_extra_arg_5() { return static_cast<int32_t>(offsetof(Delegate_t, ___extra_arg_5)); }
	inline intptr_t get_extra_arg_5() const { return ___extra_arg_5; }
	inline intptr_t* get_address_of_extra_arg_5() { return &___extra_arg_5; }
	inline void set_extra_arg_5(intptr_t value)
	{
		___extra_arg_5 = value;
	}

	inline static int32_t get_offset_of_method_code_6() { return static_cast<int32_t>(offsetof(Delegate_t, ___method_code_6)); }
	inline intptr_t get_method_code_6() const { return ___method_code_6; }
	inline intptr_t* get_address_of_method_code_6() { return &___method_code_6; }
	inline void set_method_code_6(intptr_t value)
	{
		___method_code_6 = value;
	}

	inline static int32_t get_offset_of_method_info_7() { return static_cast<int32_t>(offsetof(Delegate_t, ___method_info_7)); }
	inline MethodInfo_t * get_method_info_7() const { return ___method_info_7; }
	inline MethodInfo_t ** get_address_of_method_info_7() { return &___method_info_7; }
	inline void set_method_info_7(MethodInfo_t * value)
	{
		___method_info_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___method_info_7), (void*)value);
	}

	inline static int32_t get_offset_of_original_method_info_8() { return static_cast<int32_t>(offsetof(Delegate_t, ___original_method_info_8)); }
	inline MethodInfo_t * get_original_method_info_8() const { return ___original_method_info_8; }
	inline MethodInfo_t ** get_address_of_original_method_info_8() { return &___original_method_info_8; }
	inline void set_original_method_info_8(MethodInfo_t * value)
	{
		___original_method_info_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___original_method_info_8), (void*)value);
	}

	inline static int32_t get_offset_of_data_9() { return static_cast<int32_t>(offsetof(Delegate_t, ___data_9)); }
	inline DelegateData_t1BF9F691B56DAE5F8C28C5E084FDE94F15F27BBE * get_data_9() const { return ___data_9; }
	inline DelegateData_t1BF9F691B56DAE5F8C28C5E084FDE94F15F27BBE ** get_address_of_data_9() { return &___data_9; }
	inline void set_data_9(DelegateData_t1BF9F691B56DAE5F8C28C5E084FDE94F15F27BBE * value)
	{
		___data_9 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___data_9), (void*)value);
	}

	inline static int32_t get_offset_of_method_is_virtual_10() { return static_cast<int32_t>(offsetof(Delegate_t, ___method_is_virtual_10)); }
	inline bool get_method_is_virtual_10() const { return ___method_is_virtual_10; }
	inline bool* get_address_of_method_is_virtual_10() { return &___method_is_virtual_10; }
	inline void set_method_is_virtual_10(bool value)
	{
		___method_is_virtual_10 = value;
	}
};

// Native definition for P/Invoke marshalling of System.Delegate
struct Delegate_t_marshaled_pinvoke
{
	intptr_t ___method_ptr_0;
	intptr_t ___invoke_impl_1;
	Il2CppIUnknown* ___m_target_2;
	intptr_t ___method_3;
	intptr_t ___delegate_trampoline_4;
	intptr_t ___extra_arg_5;
	intptr_t ___method_code_6;
	MethodInfo_t * ___method_info_7;
	MethodInfo_t * ___original_method_info_8;
	DelegateData_t1BF9F691B56DAE5F8C28C5E084FDE94F15F27BBE * ___data_9;
	int32_t ___method_is_virtual_10;
};
// Native definition for COM marshalling of System.Delegate
struct Delegate_t_marshaled_com
{
	intptr_t ___method_ptr_0;
	intptr_t ___invoke_impl_1;
	Il2CppIUnknown* ___m_target_2;
	intptr_t ___method_3;
	intptr_t ___delegate_trampoline_4;
	intptr_t ___extra_arg_5;
	intptr_t ___method_code_6;
	MethodInfo_t * ___method_info_7;
	MethodInfo_t * ___original_method_info_8;
	DelegateData_t1BF9F691B56DAE5F8C28C5E084FDE94F15F27BBE * ___data_9;
	int32_t ___method_is_virtual_10;
};

// System.Int32Enum
struct  Int32Enum_t6312CE4586C17FE2E2E513D2E7655B574F10FDCD 
{
public:
	// System.Int32 System.Int32Enum::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(Int32Enum_t6312CE4586C17FE2E2E513D2E7655B574F10FDCD, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// System.Reflection.BindingFlags
struct  BindingFlags_tE35C91D046E63A1B92BB9AB909FCF9DA84379ED0 
{
public:
	// System.Int32 System.Reflection.BindingFlags::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(BindingFlags_tE35C91D046E63A1B92BB9AB909FCF9DA84379ED0, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// System.RuntimeFieldHandle
struct  RuntimeFieldHandle_t844BDF00E8E6FE69D9AEAA7657F09018B864F4EF 
{
public:
	// System.IntPtr System.RuntimeFieldHandle::value
	intptr_t ___value_0;

public:
	inline static int32_t get_offset_of_value_0() { return static_cast<int32_t>(offsetof(RuntimeFieldHandle_t844BDF00E8E6FE69D9AEAA7657F09018B864F4EF, ___value_0)); }
	inline intptr_t get_value_0() const { return ___value_0; }
	inline intptr_t* get_address_of_value_0() { return &___value_0; }
	inline void set_value_0(intptr_t value)
	{
		___value_0 = value;
	}
};


// System.RuntimeTypeHandle
struct  RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D 
{
public:
	// System.IntPtr System.RuntimeTypeHandle::value
	intptr_t ___value_0;

public:
	inline static int32_t get_offset_of_value_0() { return static_cast<int32_t>(offsetof(RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D, ___value_0)); }
	inline intptr_t get_value_0() const { return ___value_0; }
	inline intptr_t* get_address_of_value_0() { return &___value_0; }
	inline void set_value_0(intptr_t value)
	{
		___value_0 = value;
	}
};


// UnityEngine.KeyCode
struct  KeyCode_tC93EA87C5A6901160B583ADFCD3EF6726570DC3C 
{
public:
	// System.Int32 UnityEngine.KeyCode::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(KeyCode_tC93EA87C5A6901160B583ADFCD3EF6726570DC3C, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.Object
struct  Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0  : public RuntimeObject
{
public:
	// System.IntPtr UnityEngine.Object::m_CachedPtr
	intptr_t ___m_CachedPtr_0;

public:
	inline static int32_t get_offset_of_m_CachedPtr_0() { return static_cast<int32_t>(offsetof(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0, ___m_CachedPtr_0)); }
	inline intptr_t get_m_CachedPtr_0() const { return ___m_CachedPtr_0; }
	inline intptr_t* get_address_of_m_CachedPtr_0() { return &___m_CachedPtr_0; }
	inline void set_m_CachedPtr_0(intptr_t value)
	{
		___m_CachedPtr_0 = value;
	}
};

struct Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_StaticFields
{
public:
	// System.Int32 UnityEngine.Object::OffsetOfInstanceIDInCPlusPlusObject
	int32_t ___OffsetOfInstanceIDInCPlusPlusObject_1;

public:
	inline static int32_t get_offset_of_OffsetOfInstanceIDInCPlusPlusObject_1() { return static_cast<int32_t>(offsetof(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_StaticFields, ___OffsetOfInstanceIDInCPlusPlusObject_1)); }
	inline int32_t get_OffsetOfInstanceIDInCPlusPlusObject_1() const { return ___OffsetOfInstanceIDInCPlusPlusObject_1; }
	inline int32_t* get_address_of_OffsetOfInstanceIDInCPlusPlusObject_1() { return &___OffsetOfInstanceIDInCPlusPlusObject_1; }
	inline void set_OffsetOfInstanceIDInCPlusPlusObject_1(int32_t value)
	{
		___OffsetOfInstanceIDInCPlusPlusObject_1 = value;
	}
};

// Native definition for P/Invoke marshalling of UnityEngine.Object
struct Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_marshaled_pinvoke
{
	intptr_t ___m_CachedPtr_0;
};
// Native definition for COM marshalling of UnityEngine.Object
struct Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_marshaled_com
{
	intptr_t ___m_CachedPtr_0;
};

// UnityEngine.Ray
struct  Ray_tE2163D4CB3E6B267E29F8ABE41684490E4A614B2 
{
public:
	// UnityEngine.Vector3 UnityEngine.Ray::m_Origin
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___m_Origin_0;
	// UnityEngine.Vector3 UnityEngine.Ray::m_Direction
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___m_Direction_1;

public:
	inline static int32_t get_offset_of_m_Origin_0() { return static_cast<int32_t>(offsetof(Ray_tE2163D4CB3E6B267E29F8ABE41684490E4A614B2, ___m_Origin_0)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_m_Origin_0() const { return ___m_Origin_0; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_m_Origin_0() { return &___m_Origin_0; }
	inline void set_m_Origin_0(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___m_Origin_0 = value;
	}

	inline static int32_t get_offset_of_m_Direction_1() { return static_cast<int32_t>(offsetof(Ray_tE2163D4CB3E6B267E29F8ABE41684490E4A614B2, ___m_Direction_1)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_m_Direction_1() const { return ___m_Direction_1; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_m_Direction_1() { return &___m_Direction_1; }
	inline void set_m_Direction_1(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___m_Direction_1 = value;
	}
};


// Microsoft.MixedReality.Toolkit.Input.BaseController
struct  BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B  : public RuntimeObject
{
public:
	// Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem Microsoft.MixedReality.Toolkit.Input.BaseController::inputSystem
	RuntimeObject* ___inputSystem_0;
	// Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping[] Microsoft.MixedReality.Toolkit.Input.BaseController::<DefaultInteractions>k__BackingField
	MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* ___U3CDefaultInteractionsU3Ek__BackingField_1;
	// Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping[] Microsoft.MixedReality.Toolkit.Input.BaseController::<DefaultLeftHandedInteractions>k__BackingField
	MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* ___U3CDefaultLeftHandedInteractionsU3Ek__BackingField_2;
	// Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping[] Microsoft.MixedReality.Toolkit.Input.BaseController::<DefaultRightHandedInteractions>k__BackingField
	MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* ___U3CDefaultRightHandedInteractionsU3Ek__BackingField_3;
	// System.Boolean Microsoft.MixedReality.Toolkit.Input.BaseController::<Enabled>k__BackingField
	bool ___U3CEnabledU3Ek__BackingField_4;
	// Microsoft.MixedReality.Toolkit.TrackingState Microsoft.MixedReality.Toolkit.Input.BaseController::<TrackingState>k__BackingField
	int32_t ___U3CTrackingStateU3Ek__BackingField_5;
	// Microsoft.MixedReality.Toolkit.Utilities.Handedness Microsoft.MixedReality.Toolkit.Input.BaseController::<ControllerHandedness>k__BackingField
	uint8_t ___U3CControllerHandednessU3Ek__BackingField_6;
	// Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource Microsoft.MixedReality.Toolkit.Input.BaseController::<InputSource>k__BackingField
	RuntimeObject* ___U3CInputSourceU3Ek__BackingField_7;
	// Microsoft.MixedReality.Toolkit.Input.IMixedRealityControllerVisualizer Microsoft.MixedReality.Toolkit.Input.BaseController::<Visualizer>k__BackingField
	RuntimeObject* ___U3CVisualizerU3Ek__BackingField_8;
	// System.Boolean Microsoft.MixedReality.Toolkit.Input.BaseController::<IsPositionAvailable>k__BackingField
	bool ___U3CIsPositionAvailableU3Ek__BackingField_9;
	// System.Boolean Microsoft.MixedReality.Toolkit.Input.BaseController::<IsPositionApproximate>k__BackingField
	bool ___U3CIsPositionApproximateU3Ek__BackingField_10;
	// System.Boolean Microsoft.MixedReality.Toolkit.Input.BaseController::<IsRotationAvailable>k__BackingField
	bool ___U3CIsRotationAvailableU3Ek__BackingField_11;
	// Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping[] Microsoft.MixedReality.Toolkit.Input.BaseController::<Interactions>k__BackingField
	MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* ___U3CInteractionsU3Ek__BackingField_12;
	// UnityEngine.Vector3 Microsoft.MixedReality.Toolkit.Input.BaseController::<AngularVelocity>k__BackingField
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___U3CAngularVelocityU3Ek__BackingField_13;
	// UnityEngine.Vector3 Microsoft.MixedReality.Toolkit.Input.BaseController::<Velocity>k__BackingField
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___U3CVelocityU3Ek__BackingField_14;

public:
	inline static int32_t get_offset_of_inputSystem_0() { return static_cast<int32_t>(offsetof(BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B, ___inputSystem_0)); }
	inline RuntimeObject* get_inputSystem_0() const { return ___inputSystem_0; }
	inline RuntimeObject** get_address_of_inputSystem_0() { return &___inputSystem_0; }
	inline void set_inputSystem_0(RuntimeObject* value)
	{
		___inputSystem_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___inputSystem_0), (void*)value);
	}

	inline static int32_t get_offset_of_U3CDefaultInteractionsU3Ek__BackingField_1() { return static_cast<int32_t>(offsetof(BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B, ___U3CDefaultInteractionsU3Ek__BackingField_1)); }
	inline MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* get_U3CDefaultInteractionsU3Ek__BackingField_1() const { return ___U3CDefaultInteractionsU3Ek__BackingField_1; }
	inline MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA** get_address_of_U3CDefaultInteractionsU3Ek__BackingField_1() { return &___U3CDefaultInteractionsU3Ek__BackingField_1; }
	inline void set_U3CDefaultInteractionsU3Ek__BackingField_1(MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* value)
	{
		___U3CDefaultInteractionsU3Ek__BackingField_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CDefaultInteractionsU3Ek__BackingField_1), (void*)value);
	}

	inline static int32_t get_offset_of_U3CDefaultLeftHandedInteractionsU3Ek__BackingField_2() { return static_cast<int32_t>(offsetof(BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B, ___U3CDefaultLeftHandedInteractionsU3Ek__BackingField_2)); }
	inline MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* get_U3CDefaultLeftHandedInteractionsU3Ek__BackingField_2() const { return ___U3CDefaultLeftHandedInteractionsU3Ek__BackingField_2; }
	inline MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA** get_address_of_U3CDefaultLeftHandedInteractionsU3Ek__BackingField_2() { return &___U3CDefaultLeftHandedInteractionsU3Ek__BackingField_2; }
	inline void set_U3CDefaultLeftHandedInteractionsU3Ek__BackingField_2(MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* value)
	{
		___U3CDefaultLeftHandedInteractionsU3Ek__BackingField_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CDefaultLeftHandedInteractionsU3Ek__BackingField_2), (void*)value);
	}

	inline static int32_t get_offset_of_U3CDefaultRightHandedInteractionsU3Ek__BackingField_3() { return static_cast<int32_t>(offsetof(BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B, ___U3CDefaultRightHandedInteractionsU3Ek__BackingField_3)); }
	inline MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* get_U3CDefaultRightHandedInteractionsU3Ek__BackingField_3() const { return ___U3CDefaultRightHandedInteractionsU3Ek__BackingField_3; }
	inline MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA** get_address_of_U3CDefaultRightHandedInteractionsU3Ek__BackingField_3() { return &___U3CDefaultRightHandedInteractionsU3Ek__BackingField_3; }
	inline void set_U3CDefaultRightHandedInteractionsU3Ek__BackingField_3(MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* value)
	{
		___U3CDefaultRightHandedInteractionsU3Ek__BackingField_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CDefaultRightHandedInteractionsU3Ek__BackingField_3), (void*)value);
	}

	inline static int32_t get_offset_of_U3CEnabledU3Ek__BackingField_4() { return static_cast<int32_t>(offsetof(BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B, ___U3CEnabledU3Ek__BackingField_4)); }
	inline bool get_U3CEnabledU3Ek__BackingField_4() const { return ___U3CEnabledU3Ek__BackingField_4; }
	inline bool* get_address_of_U3CEnabledU3Ek__BackingField_4() { return &___U3CEnabledU3Ek__BackingField_4; }
	inline void set_U3CEnabledU3Ek__BackingField_4(bool value)
	{
		___U3CEnabledU3Ek__BackingField_4 = value;
	}

	inline static int32_t get_offset_of_U3CTrackingStateU3Ek__BackingField_5() { return static_cast<int32_t>(offsetof(BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B, ___U3CTrackingStateU3Ek__BackingField_5)); }
	inline int32_t get_U3CTrackingStateU3Ek__BackingField_5() const { return ___U3CTrackingStateU3Ek__BackingField_5; }
	inline int32_t* get_address_of_U3CTrackingStateU3Ek__BackingField_5() { return &___U3CTrackingStateU3Ek__BackingField_5; }
	inline void set_U3CTrackingStateU3Ek__BackingField_5(int32_t value)
	{
		___U3CTrackingStateU3Ek__BackingField_5 = value;
	}

	inline static int32_t get_offset_of_U3CControllerHandednessU3Ek__BackingField_6() { return static_cast<int32_t>(offsetof(BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B, ___U3CControllerHandednessU3Ek__BackingField_6)); }
	inline uint8_t get_U3CControllerHandednessU3Ek__BackingField_6() const { return ___U3CControllerHandednessU3Ek__BackingField_6; }
	inline uint8_t* get_address_of_U3CControllerHandednessU3Ek__BackingField_6() { return &___U3CControllerHandednessU3Ek__BackingField_6; }
	inline void set_U3CControllerHandednessU3Ek__BackingField_6(uint8_t value)
	{
		___U3CControllerHandednessU3Ek__BackingField_6 = value;
	}

	inline static int32_t get_offset_of_U3CInputSourceU3Ek__BackingField_7() { return static_cast<int32_t>(offsetof(BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B, ___U3CInputSourceU3Ek__BackingField_7)); }
	inline RuntimeObject* get_U3CInputSourceU3Ek__BackingField_7() const { return ___U3CInputSourceU3Ek__BackingField_7; }
	inline RuntimeObject** get_address_of_U3CInputSourceU3Ek__BackingField_7() { return &___U3CInputSourceU3Ek__BackingField_7; }
	inline void set_U3CInputSourceU3Ek__BackingField_7(RuntimeObject* value)
	{
		___U3CInputSourceU3Ek__BackingField_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CInputSourceU3Ek__BackingField_7), (void*)value);
	}

	inline static int32_t get_offset_of_U3CVisualizerU3Ek__BackingField_8() { return static_cast<int32_t>(offsetof(BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B, ___U3CVisualizerU3Ek__BackingField_8)); }
	inline RuntimeObject* get_U3CVisualizerU3Ek__BackingField_8() const { return ___U3CVisualizerU3Ek__BackingField_8; }
	inline RuntimeObject** get_address_of_U3CVisualizerU3Ek__BackingField_8() { return &___U3CVisualizerU3Ek__BackingField_8; }
	inline void set_U3CVisualizerU3Ek__BackingField_8(RuntimeObject* value)
	{
		___U3CVisualizerU3Ek__BackingField_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CVisualizerU3Ek__BackingField_8), (void*)value);
	}

	inline static int32_t get_offset_of_U3CIsPositionAvailableU3Ek__BackingField_9() { return static_cast<int32_t>(offsetof(BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B, ___U3CIsPositionAvailableU3Ek__BackingField_9)); }
	inline bool get_U3CIsPositionAvailableU3Ek__BackingField_9() const { return ___U3CIsPositionAvailableU3Ek__BackingField_9; }
	inline bool* get_address_of_U3CIsPositionAvailableU3Ek__BackingField_9() { return &___U3CIsPositionAvailableU3Ek__BackingField_9; }
	inline void set_U3CIsPositionAvailableU3Ek__BackingField_9(bool value)
	{
		___U3CIsPositionAvailableU3Ek__BackingField_9 = value;
	}

	inline static int32_t get_offset_of_U3CIsPositionApproximateU3Ek__BackingField_10() { return static_cast<int32_t>(offsetof(BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B, ___U3CIsPositionApproximateU3Ek__BackingField_10)); }
	inline bool get_U3CIsPositionApproximateU3Ek__BackingField_10() const { return ___U3CIsPositionApproximateU3Ek__BackingField_10; }
	inline bool* get_address_of_U3CIsPositionApproximateU3Ek__BackingField_10() { return &___U3CIsPositionApproximateU3Ek__BackingField_10; }
	inline void set_U3CIsPositionApproximateU3Ek__BackingField_10(bool value)
	{
		___U3CIsPositionApproximateU3Ek__BackingField_10 = value;
	}

	inline static int32_t get_offset_of_U3CIsRotationAvailableU3Ek__BackingField_11() { return static_cast<int32_t>(offsetof(BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B, ___U3CIsRotationAvailableU3Ek__BackingField_11)); }
	inline bool get_U3CIsRotationAvailableU3Ek__BackingField_11() const { return ___U3CIsRotationAvailableU3Ek__BackingField_11; }
	inline bool* get_address_of_U3CIsRotationAvailableU3Ek__BackingField_11() { return &___U3CIsRotationAvailableU3Ek__BackingField_11; }
	inline void set_U3CIsRotationAvailableU3Ek__BackingField_11(bool value)
	{
		___U3CIsRotationAvailableU3Ek__BackingField_11 = value;
	}

	inline static int32_t get_offset_of_U3CInteractionsU3Ek__BackingField_12() { return static_cast<int32_t>(offsetof(BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B, ___U3CInteractionsU3Ek__BackingField_12)); }
	inline MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* get_U3CInteractionsU3Ek__BackingField_12() const { return ___U3CInteractionsU3Ek__BackingField_12; }
	inline MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA** get_address_of_U3CInteractionsU3Ek__BackingField_12() { return &___U3CInteractionsU3Ek__BackingField_12; }
	inline void set_U3CInteractionsU3Ek__BackingField_12(MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* value)
	{
		___U3CInteractionsU3Ek__BackingField_12 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CInteractionsU3Ek__BackingField_12), (void*)value);
	}

	inline static int32_t get_offset_of_U3CAngularVelocityU3Ek__BackingField_13() { return static_cast<int32_t>(offsetof(BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B, ___U3CAngularVelocityU3Ek__BackingField_13)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_U3CAngularVelocityU3Ek__BackingField_13() const { return ___U3CAngularVelocityU3Ek__BackingField_13; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_U3CAngularVelocityU3Ek__BackingField_13() { return &___U3CAngularVelocityU3Ek__BackingField_13; }
	inline void set_U3CAngularVelocityU3Ek__BackingField_13(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___U3CAngularVelocityU3Ek__BackingField_13 = value;
	}

	inline static int32_t get_offset_of_U3CVelocityU3Ek__BackingField_14() { return static_cast<int32_t>(offsetof(BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B, ___U3CVelocityU3Ek__BackingField_14)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_U3CVelocityU3Ek__BackingField_14() const { return ___U3CVelocityU3Ek__BackingField_14; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_U3CVelocityU3Ek__BackingField_14() { return &___U3CVelocityU3Ek__BackingField_14; }
	inline void set_U3CVelocityU3Ek__BackingField_14(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___U3CVelocityU3Ek__BackingField_14 = value;
	}
};


// Microsoft.MixedReality.Toolkit.Input.HandRay
struct  HandRay_t9DAE3FE243DBED1BAA1B9A4F782C3F1C9E6AE285  : public RuntimeObject
{
public:
	// UnityEngine.Ray Microsoft.MixedReality.Toolkit.Input.HandRay::ray
	Ray_tE2163D4CB3E6B267E29F8ABE41684490E4A614B2  ___ray_0;
	// System.Single Microsoft.MixedReality.Toolkit.Input.HandRay::CursorBeamBackwardTolerance
	float ___CursorBeamBackwardTolerance_10;
	// System.Single Microsoft.MixedReality.Toolkit.Input.HandRay::CursorBeamUpTolerance
	float ___CursorBeamUpTolerance_11;
	// Microsoft.MixedReality.Toolkit.Input.StabilizedRay Microsoft.MixedReality.Toolkit.Input.HandRay::stabilizedRay
	StabilizedRay_tCE887AC85F7E1C0B2EA6DFE158B4BA7E7440E048 * ___stabilizedRay_13;
	// UnityEngine.Vector3 Microsoft.MixedReality.Toolkit.Input.HandRay::palmNormal
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___palmNormal_14;
	// UnityEngine.Vector3 Microsoft.MixedReality.Toolkit.Input.HandRay::headForward
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___headForward_15;

public:
	inline static int32_t get_offset_of_ray_0() { return static_cast<int32_t>(offsetof(HandRay_t9DAE3FE243DBED1BAA1B9A4F782C3F1C9E6AE285, ___ray_0)); }
	inline Ray_tE2163D4CB3E6B267E29F8ABE41684490E4A614B2  get_ray_0() const { return ___ray_0; }
	inline Ray_tE2163D4CB3E6B267E29F8ABE41684490E4A614B2 * get_address_of_ray_0() { return &___ray_0; }
	inline void set_ray_0(Ray_tE2163D4CB3E6B267E29F8ABE41684490E4A614B2  value)
	{
		___ray_0 = value;
	}

	inline static int32_t get_offset_of_CursorBeamBackwardTolerance_10() { return static_cast<int32_t>(offsetof(HandRay_t9DAE3FE243DBED1BAA1B9A4F782C3F1C9E6AE285, ___CursorBeamBackwardTolerance_10)); }
	inline float get_CursorBeamBackwardTolerance_10() const { return ___CursorBeamBackwardTolerance_10; }
	inline float* get_address_of_CursorBeamBackwardTolerance_10() { return &___CursorBeamBackwardTolerance_10; }
	inline void set_CursorBeamBackwardTolerance_10(float value)
	{
		___CursorBeamBackwardTolerance_10 = value;
	}

	inline static int32_t get_offset_of_CursorBeamUpTolerance_11() { return static_cast<int32_t>(offsetof(HandRay_t9DAE3FE243DBED1BAA1B9A4F782C3F1C9E6AE285, ___CursorBeamUpTolerance_11)); }
	inline float get_CursorBeamUpTolerance_11() const { return ___CursorBeamUpTolerance_11; }
	inline float* get_address_of_CursorBeamUpTolerance_11() { return &___CursorBeamUpTolerance_11; }
	inline void set_CursorBeamUpTolerance_11(float value)
	{
		___CursorBeamUpTolerance_11 = value;
	}

	inline static int32_t get_offset_of_stabilizedRay_13() { return static_cast<int32_t>(offsetof(HandRay_t9DAE3FE243DBED1BAA1B9A4F782C3F1C9E6AE285, ___stabilizedRay_13)); }
	inline StabilizedRay_tCE887AC85F7E1C0B2EA6DFE158B4BA7E7440E048 * get_stabilizedRay_13() const { return ___stabilizedRay_13; }
	inline StabilizedRay_tCE887AC85F7E1C0B2EA6DFE158B4BA7E7440E048 ** get_address_of_stabilizedRay_13() { return &___stabilizedRay_13; }
	inline void set_stabilizedRay_13(StabilizedRay_tCE887AC85F7E1C0B2EA6DFE158B4BA7E7440E048 * value)
	{
		___stabilizedRay_13 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___stabilizedRay_13), (void*)value);
	}

	inline static int32_t get_offset_of_palmNormal_14() { return static_cast<int32_t>(offsetof(HandRay_t9DAE3FE243DBED1BAA1B9A4F782C3F1C9E6AE285, ___palmNormal_14)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_palmNormal_14() const { return ___palmNormal_14; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_palmNormal_14() { return &___palmNormal_14; }
	inline void set_palmNormal_14(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___palmNormal_14 = value;
	}

	inline static int32_t get_offset_of_headForward_15() { return static_cast<int32_t>(offsetof(HandRay_t9DAE3FE243DBED1BAA1B9A4F782C3F1C9E6AE285, ___headForward_15)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_headForward_15() const { return ___headForward_15; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_headForward_15() { return &___headForward_15; }
	inline void set_headForward_15(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___headForward_15 = value;
	}
};


// Microsoft.MixedReality.Toolkit.Input.KeyBinding
struct  KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 
{
public:
	// Microsoft.MixedReality.Toolkit.Input.KeyBinding_KeyType Microsoft.MixedReality.Toolkit.Input.KeyBinding::bindingType
	int32_t ___bindingType_3;
	// System.Int32 Microsoft.MixedReality.Toolkit.Input.KeyBinding::code
	int32_t ___code_4;

public:
	inline static int32_t get_offset_of_bindingType_3() { return static_cast<int32_t>(offsetof(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79, ___bindingType_3)); }
	inline int32_t get_bindingType_3() const { return ___bindingType_3; }
	inline int32_t* get_address_of_bindingType_3() { return &___bindingType_3; }
	inline void set_bindingType_3(int32_t value)
	{
		___bindingType_3 = value;
	}

	inline static int32_t get_offset_of_code_4() { return static_cast<int32_t>(offsetof(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79, ___code_4)); }
	inline int32_t get_code_4() const { return ___code_4; }
	inline int32_t* get_address_of_code_4() { return &___code_4; }
	inline void set_code_4(int32_t value)
	{
		___code_4 = value;
	}
};

struct KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79_StaticFields
{
public:
	// System.String[] Microsoft.MixedReality.Toolkit.Input.KeyBinding::AllCodeNames
	StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* ___AllCodeNames_0;
	// System.Collections.Generic.Dictionary`2<System.Tuple`2<Microsoft.MixedReality.Toolkit.Input.KeyBinding_KeyType,System.Int32>,System.Int32> Microsoft.MixedReality.Toolkit.Input.KeyBinding::KeyBindingToEnumMap
	Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB * ___KeyBindingToEnumMap_1;
	// System.Collections.Generic.Dictionary`2<System.Int32,System.Tuple`2<Microsoft.MixedReality.Toolkit.Input.KeyBinding_KeyType,System.Int32>> Microsoft.MixedReality.Toolkit.Input.KeyBinding::EnumToKeyBindingMap
	Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64 * ___EnumToKeyBindingMap_2;

public:
	inline static int32_t get_offset_of_AllCodeNames_0() { return static_cast<int32_t>(offsetof(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79_StaticFields, ___AllCodeNames_0)); }
	inline StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* get_AllCodeNames_0() const { return ___AllCodeNames_0; }
	inline StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E** get_address_of_AllCodeNames_0() { return &___AllCodeNames_0; }
	inline void set_AllCodeNames_0(StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* value)
	{
		___AllCodeNames_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___AllCodeNames_0), (void*)value);
	}

	inline static int32_t get_offset_of_KeyBindingToEnumMap_1() { return static_cast<int32_t>(offsetof(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79_StaticFields, ___KeyBindingToEnumMap_1)); }
	inline Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB * get_KeyBindingToEnumMap_1() const { return ___KeyBindingToEnumMap_1; }
	inline Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB ** get_address_of_KeyBindingToEnumMap_1() { return &___KeyBindingToEnumMap_1; }
	inline void set_KeyBindingToEnumMap_1(Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB * value)
	{
		___KeyBindingToEnumMap_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___KeyBindingToEnumMap_1), (void*)value);
	}

	inline static int32_t get_offset_of_EnumToKeyBindingMap_2() { return static_cast<int32_t>(offsetof(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79_StaticFields, ___EnumToKeyBindingMap_2)); }
	inline Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64 * get_EnumToKeyBindingMap_2() const { return ___EnumToKeyBindingMap_2; }
	inline Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64 ** get_address_of_EnumToKeyBindingMap_2() { return &___EnumToKeyBindingMap_2; }
	inline void set_EnumToKeyBindingMap_2(Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64 * value)
	{
		___EnumToKeyBindingMap_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___EnumToKeyBindingMap_2), (void*)value);
	}
};


// Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction
struct  MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073 
{
public:
	// System.UInt32 Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction::id
	uint32_t ___id_1;
	// System.String Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction::description
	String_t* ___description_2;
	// Microsoft.MixedReality.Toolkit.Utilities.AxisType Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction::axisConstraint
	int32_t ___axisConstraint_3;

public:
	inline static int32_t get_offset_of_id_1() { return static_cast<int32_t>(offsetof(MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073, ___id_1)); }
	inline uint32_t get_id_1() const { return ___id_1; }
	inline uint32_t* get_address_of_id_1() { return &___id_1; }
	inline void set_id_1(uint32_t value)
	{
		___id_1 = value;
	}

	inline static int32_t get_offset_of_description_2() { return static_cast<int32_t>(offsetof(MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073, ___description_2)); }
	inline String_t* get_description_2() const { return ___description_2; }
	inline String_t** get_address_of_description_2() { return &___description_2; }
	inline void set_description_2(String_t* value)
	{
		___description_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___description_2), (void*)value);
	}

	inline static int32_t get_offset_of_axisConstraint_3() { return static_cast<int32_t>(offsetof(MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073, ___axisConstraint_3)); }
	inline int32_t get_axisConstraint_3() const { return ___axisConstraint_3; }
	inline int32_t* get_address_of_axisConstraint_3() { return &___axisConstraint_3; }
	inline void set_axisConstraint_3(int32_t value)
	{
		___axisConstraint_3 = value;
	}
};

struct MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073_StaticFields
{
public:
	// Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction::<None>k__BackingField
	MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  ___U3CNoneU3Ek__BackingField_0;

public:
	inline static int32_t get_offset_of_U3CNoneU3Ek__BackingField_0() { return static_cast<int32_t>(offsetof(MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073_StaticFields, ___U3CNoneU3Ek__BackingField_0)); }
	inline MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  get_U3CNoneU3Ek__BackingField_0() const { return ___U3CNoneU3Ek__BackingField_0; }
	inline MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073 * get_address_of_U3CNoneU3Ek__BackingField_0() { return &___U3CNoneU3Ek__BackingField_0; }
	inline void set_U3CNoneU3Ek__BackingField_0(MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  value)
	{
		___U3CNoneU3Ek__BackingField_0 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___U3CNoneU3Ek__BackingField_0))->___description_2), (void*)NULL);
	}
};

// Native definition for P/Invoke marshalling of Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction
struct MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073_marshaled_pinvoke
{
	uint32_t ___id_1;
	char* ___description_2;
	int32_t ___axisConstraint_3;
};
// Native definition for COM marshalling of Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction
struct MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073_marshaled_com
{
	uint32_t ___id_1;
	Il2CppChar* ___description_2;
	int32_t ___axisConstraint_3;
};

// System.MulticastDelegate
struct  MulticastDelegate_t  : public Delegate_t
{
public:
	// System.Delegate[] System.MulticastDelegate::delegates
	DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86* ___delegates_11;

public:
	inline static int32_t get_offset_of_delegates_11() { return static_cast<int32_t>(offsetof(MulticastDelegate_t, ___delegates_11)); }
	inline DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86* get_delegates_11() const { return ___delegates_11; }
	inline DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86** get_address_of_delegates_11() { return &___delegates_11; }
	inline void set_delegates_11(DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86* value)
	{
		___delegates_11 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___delegates_11), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of System.MulticastDelegate
struct MulticastDelegate_t_marshaled_pinvoke : public Delegate_t_marshaled_pinvoke
{
	Delegate_t_marshaled_pinvoke** ___delegates_11;
};
// Native definition for COM marshalling of System.MulticastDelegate
struct MulticastDelegate_t_marshaled_com : public Delegate_t_marshaled_com
{
	Delegate_t_marshaled_com** ___delegates_11;
};

// System.Tuple`2<Microsoft.MixedReality.Toolkit.Input.KeyBinding_KeyType,System.Int32>
struct  Tuple_2_tFF0D9FEC0FEA81089BD6B1384583703BD0A104EE  : public RuntimeObject
{
public:
	// T1 System.Tuple`2::m_Item1
	int32_t ___m_Item1_0;
	// T2 System.Tuple`2::m_Item2
	int32_t ___m_Item2_1;

public:
	inline static int32_t get_offset_of_m_Item1_0() { return static_cast<int32_t>(offsetof(Tuple_2_tFF0D9FEC0FEA81089BD6B1384583703BD0A104EE, ___m_Item1_0)); }
	inline int32_t get_m_Item1_0() const { return ___m_Item1_0; }
	inline int32_t* get_address_of_m_Item1_0() { return &___m_Item1_0; }
	inline void set_m_Item1_0(int32_t value)
	{
		___m_Item1_0 = value;
	}

	inline static int32_t get_offset_of_m_Item2_1() { return static_cast<int32_t>(offsetof(Tuple_2_tFF0D9FEC0FEA81089BD6B1384583703BD0A104EE, ___m_Item2_1)); }
	inline int32_t get_m_Item2_1() const { return ___m_Item2_1; }
	inline int32_t* get_address_of_m_Item2_1() { return &___m_Item2_1; }
	inline void set_m_Item2_1(int32_t value)
	{
		___m_Item2_1 = value;
	}
};


// System.Type
struct  Type_t  : public MemberInfo_t
{
public:
	// System.RuntimeTypeHandle System.Type::_impl
	RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  ____impl_9;

public:
	inline static int32_t get_offset_of__impl_9() { return static_cast<int32_t>(offsetof(Type_t, ____impl_9)); }
	inline RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  get__impl_9() const { return ____impl_9; }
	inline RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D * get_address_of__impl_9() { return &____impl_9; }
	inline void set__impl_9(RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  value)
	{
		____impl_9 = value;
	}
};

struct Type_t_StaticFields
{
public:
	// System.Reflection.MemberFilter System.Type::FilterAttribute
	MemberFilter_t25C1BD92C42BE94426E300787C13C452CB89B381 * ___FilterAttribute_0;
	// System.Reflection.MemberFilter System.Type::FilterName
	MemberFilter_t25C1BD92C42BE94426E300787C13C452CB89B381 * ___FilterName_1;
	// System.Reflection.MemberFilter System.Type::FilterNameIgnoreCase
	MemberFilter_t25C1BD92C42BE94426E300787C13C452CB89B381 * ___FilterNameIgnoreCase_2;
	// System.Object System.Type::Missing
	RuntimeObject * ___Missing_3;
	// System.Char System.Type::Delimiter
	Il2CppChar ___Delimiter_4;
	// System.Type[] System.Type::EmptyTypes
	TypeU5BU5D_t7FE623A666B49176DE123306221193E888A12F5F* ___EmptyTypes_5;
	// System.Reflection.Binder System.Type::defaultBinder
	Binder_t4D5CB06963501D32847C057B57157D6DC49CA759 * ___defaultBinder_6;

public:
	inline static int32_t get_offset_of_FilterAttribute_0() { return static_cast<int32_t>(offsetof(Type_t_StaticFields, ___FilterAttribute_0)); }
	inline MemberFilter_t25C1BD92C42BE94426E300787C13C452CB89B381 * get_FilterAttribute_0() const { return ___FilterAttribute_0; }
	inline MemberFilter_t25C1BD92C42BE94426E300787C13C452CB89B381 ** get_address_of_FilterAttribute_0() { return &___FilterAttribute_0; }
	inline void set_FilterAttribute_0(MemberFilter_t25C1BD92C42BE94426E300787C13C452CB89B381 * value)
	{
		___FilterAttribute_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___FilterAttribute_0), (void*)value);
	}

	inline static int32_t get_offset_of_FilterName_1() { return static_cast<int32_t>(offsetof(Type_t_StaticFields, ___FilterName_1)); }
	inline MemberFilter_t25C1BD92C42BE94426E300787C13C452CB89B381 * get_FilterName_1() const { return ___FilterName_1; }
	inline MemberFilter_t25C1BD92C42BE94426E300787C13C452CB89B381 ** get_address_of_FilterName_1() { return &___FilterName_1; }
	inline void set_FilterName_1(MemberFilter_t25C1BD92C42BE94426E300787C13C452CB89B381 * value)
	{
		___FilterName_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___FilterName_1), (void*)value);
	}

	inline static int32_t get_offset_of_FilterNameIgnoreCase_2() { return static_cast<int32_t>(offsetof(Type_t_StaticFields, ___FilterNameIgnoreCase_2)); }
	inline MemberFilter_t25C1BD92C42BE94426E300787C13C452CB89B381 * get_FilterNameIgnoreCase_2() const { return ___FilterNameIgnoreCase_2; }
	inline MemberFilter_t25C1BD92C42BE94426E300787C13C452CB89B381 ** get_address_of_FilterNameIgnoreCase_2() { return &___FilterNameIgnoreCase_2; }
	inline void set_FilterNameIgnoreCase_2(MemberFilter_t25C1BD92C42BE94426E300787C13C452CB89B381 * value)
	{
		___FilterNameIgnoreCase_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___FilterNameIgnoreCase_2), (void*)value);
	}

	inline static int32_t get_offset_of_Missing_3() { return static_cast<int32_t>(offsetof(Type_t_StaticFields, ___Missing_3)); }
	inline RuntimeObject * get_Missing_3() const { return ___Missing_3; }
	inline RuntimeObject ** get_address_of_Missing_3() { return &___Missing_3; }
	inline void set_Missing_3(RuntimeObject * value)
	{
		___Missing_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___Missing_3), (void*)value);
	}

	inline static int32_t get_offset_of_Delimiter_4() { return static_cast<int32_t>(offsetof(Type_t_StaticFields, ___Delimiter_4)); }
	inline Il2CppChar get_Delimiter_4() const { return ___Delimiter_4; }
	inline Il2CppChar* get_address_of_Delimiter_4() { return &___Delimiter_4; }
	inline void set_Delimiter_4(Il2CppChar value)
	{
		___Delimiter_4 = value;
	}

	inline static int32_t get_offset_of_EmptyTypes_5() { return static_cast<int32_t>(offsetof(Type_t_StaticFields, ___EmptyTypes_5)); }
	inline TypeU5BU5D_t7FE623A666B49176DE123306221193E888A12F5F* get_EmptyTypes_5() const { return ___EmptyTypes_5; }
	inline TypeU5BU5D_t7FE623A666B49176DE123306221193E888A12F5F** get_address_of_EmptyTypes_5() { return &___EmptyTypes_5; }
	inline void set_EmptyTypes_5(TypeU5BU5D_t7FE623A666B49176DE123306221193E888A12F5F* value)
	{
		___EmptyTypes_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___EmptyTypes_5), (void*)value);
	}

	inline static int32_t get_offset_of_defaultBinder_6() { return static_cast<int32_t>(offsetof(Type_t_StaticFields, ___defaultBinder_6)); }
	inline Binder_t4D5CB06963501D32847C057B57157D6DC49CA759 * get_defaultBinder_6() const { return ___defaultBinder_6; }
	inline Binder_t4D5CB06963501D32847C057B57157D6DC49CA759 ** get_address_of_defaultBinder_6() { return &___defaultBinder_6; }
	inline void set_defaultBinder_6(Binder_t4D5CB06963501D32847C057B57157D6DC49CA759 * value)
	{
		___defaultBinder_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___defaultBinder_6), (void*)value);
	}
};


// UnityEngine.Component
struct  Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621  : public Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0
{
public:

public:
};


// UnityEngine.GameObject
struct  GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F  : public Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0
{
public:

public:
};


// UnityEngine.ScriptableObject
struct  ScriptableObject_tAB015486CEAB714DA0D5C1BA389B84FB90427734  : public Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0
{
public:

public:
};

// Native definition for P/Invoke marshalling of UnityEngine.ScriptableObject
struct ScriptableObject_tAB015486CEAB714DA0D5C1BA389B84FB90427734_marshaled_pinvoke : public Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_marshaled_pinvoke
{
};
// Native definition for COM marshalling of UnityEngine.ScriptableObject
struct ScriptableObject_tAB015486CEAB714DA0D5C1BA389B84FB90427734_marshaled_com : public Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_marshaled_com
{
};

// Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile
struct  BaseMixedRealityProfile_tB4DC16619B37D298D22571CE017070A78EF826E8  : public ScriptableObject_tAB015486CEAB714DA0D5C1BA389B84FB90427734
{
public:
	// System.Boolean Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile::isCustomProfile
	bool ___isCustomProfile_4;

public:
	inline static int32_t get_offset_of_isCustomProfile_4() { return static_cast<int32_t>(offsetof(BaseMixedRealityProfile_tB4DC16619B37D298D22571CE017070A78EF826E8, ___isCustomProfile_4)); }
	inline bool get_isCustomProfile_4() const { return ___isCustomProfile_4; }
	inline bool* get_address_of_isCustomProfile_4() { return &___isCustomProfile_4; }
	inline void set_isCustomProfile_4(bool value)
	{
		___isCustomProfile_4 = value;
	}
};


// Microsoft.MixedReality.Toolkit.Input.BaseHand
struct  BaseHand_tB58ECFC99FBFD516BBAA0989004A10F687078F4B  : public BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B
{
public:
	// Microsoft.MixedReality.Toolkit.Input.HandRay Microsoft.MixedReality.Toolkit.Input.BaseHand::<HandRay>k__BackingField
	HandRay_t9DAE3FE243DBED1BAA1B9A4F782C3F1C9E6AE285 * ___U3CHandRayU3Ek__BackingField_15;
	// System.Single Microsoft.MixedReality.Toolkit.Input.BaseHand::deltaTimeStart
	float ___deltaTimeStart_16;
	// System.Int32 Microsoft.MixedReality.Toolkit.Input.BaseHand::frameOn
	int32_t ___frameOn_18;
	// UnityEngine.Vector3[] Microsoft.MixedReality.Toolkit.Input.BaseHand::velocityPositionsCache
	Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* ___velocityPositionsCache_19;
	// UnityEngine.Vector3[] Microsoft.MixedReality.Toolkit.Input.BaseHand::velocityNormalsCache
	Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* ___velocityNormalsCache_20;
	// UnityEngine.Vector3 Microsoft.MixedReality.Toolkit.Input.BaseHand::velocityPositionsSum
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___velocityPositionsSum_21;
	// UnityEngine.Vector3 Microsoft.MixedReality.Toolkit.Input.BaseHand::velocityNormalsSum
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___velocityNormalsSum_22;

public:
	inline static int32_t get_offset_of_U3CHandRayU3Ek__BackingField_15() { return static_cast<int32_t>(offsetof(BaseHand_tB58ECFC99FBFD516BBAA0989004A10F687078F4B, ___U3CHandRayU3Ek__BackingField_15)); }
	inline HandRay_t9DAE3FE243DBED1BAA1B9A4F782C3F1C9E6AE285 * get_U3CHandRayU3Ek__BackingField_15() const { return ___U3CHandRayU3Ek__BackingField_15; }
	inline HandRay_t9DAE3FE243DBED1BAA1B9A4F782C3F1C9E6AE285 ** get_address_of_U3CHandRayU3Ek__BackingField_15() { return &___U3CHandRayU3Ek__BackingField_15; }
	inline void set_U3CHandRayU3Ek__BackingField_15(HandRay_t9DAE3FE243DBED1BAA1B9A4F782C3F1C9E6AE285 * value)
	{
		___U3CHandRayU3Ek__BackingField_15 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CHandRayU3Ek__BackingField_15), (void*)value);
	}

	inline static int32_t get_offset_of_deltaTimeStart_16() { return static_cast<int32_t>(offsetof(BaseHand_tB58ECFC99FBFD516BBAA0989004A10F687078F4B, ___deltaTimeStart_16)); }
	inline float get_deltaTimeStart_16() const { return ___deltaTimeStart_16; }
	inline float* get_address_of_deltaTimeStart_16() { return &___deltaTimeStart_16; }
	inline void set_deltaTimeStart_16(float value)
	{
		___deltaTimeStart_16 = value;
	}

	inline static int32_t get_offset_of_frameOn_18() { return static_cast<int32_t>(offsetof(BaseHand_tB58ECFC99FBFD516BBAA0989004A10F687078F4B, ___frameOn_18)); }
	inline int32_t get_frameOn_18() const { return ___frameOn_18; }
	inline int32_t* get_address_of_frameOn_18() { return &___frameOn_18; }
	inline void set_frameOn_18(int32_t value)
	{
		___frameOn_18 = value;
	}

	inline static int32_t get_offset_of_velocityPositionsCache_19() { return static_cast<int32_t>(offsetof(BaseHand_tB58ECFC99FBFD516BBAA0989004A10F687078F4B, ___velocityPositionsCache_19)); }
	inline Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* get_velocityPositionsCache_19() const { return ___velocityPositionsCache_19; }
	inline Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28** get_address_of_velocityPositionsCache_19() { return &___velocityPositionsCache_19; }
	inline void set_velocityPositionsCache_19(Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* value)
	{
		___velocityPositionsCache_19 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___velocityPositionsCache_19), (void*)value);
	}

	inline static int32_t get_offset_of_velocityNormalsCache_20() { return static_cast<int32_t>(offsetof(BaseHand_tB58ECFC99FBFD516BBAA0989004A10F687078F4B, ___velocityNormalsCache_20)); }
	inline Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* get_velocityNormalsCache_20() const { return ___velocityNormalsCache_20; }
	inline Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28** get_address_of_velocityNormalsCache_20() { return &___velocityNormalsCache_20; }
	inline void set_velocityNormalsCache_20(Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* value)
	{
		___velocityNormalsCache_20 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___velocityNormalsCache_20), (void*)value);
	}

	inline static int32_t get_offset_of_velocityPositionsSum_21() { return static_cast<int32_t>(offsetof(BaseHand_tB58ECFC99FBFD516BBAA0989004A10F687078F4B, ___velocityPositionsSum_21)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_velocityPositionsSum_21() const { return ___velocityPositionsSum_21; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_velocityPositionsSum_21() { return &___velocityPositionsSum_21; }
	inline void set_velocityPositionsSum_21(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___velocityPositionsSum_21 = value;
	}

	inline static int32_t get_offset_of_velocityNormalsSum_22() { return static_cast<int32_t>(offsetof(BaseHand_tB58ECFC99FBFD516BBAA0989004A10F687078F4B, ___velocityNormalsSum_22)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_velocityNormalsSum_22() const { return ___velocityNormalsSum_22; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_velocityNormalsSum_22() { return &___velocityNormalsSum_22; }
	inline void set_velocityNormalsSum_22(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___velocityNormalsSum_22 = value;
	}
};


// Microsoft.MixedReality.Toolkit.Input.MixedRealityGestureMapping
struct  MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074 
{
public:
	// System.String Microsoft.MixedReality.Toolkit.Input.MixedRealityGestureMapping::description
	String_t* ___description_0;
	// Microsoft.MixedReality.Toolkit.Input.GestureInputType Microsoft.MixedReality.Toolkit.Input.MixedRealityGestureMapping::gestureType
	int32_t ___gestureType_1;
	// Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction Microsoft.MixedReality.Toolkit.Input.MixedRealityGestureMapping::action
	MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  ___action_2;

public:
	inline static int32_t get_offset_of_description_0() { return static_cast<int32_t>(offsetof(MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074, ___description_0)); }
	inline String_t* get_description_0() const { return ___description_0; }
	inline String_t** get_address_of_description_0() { return &___description_0; }
	inline void set_description_0(String_t* value)
	{
		___description_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___description_0), (void*)value);
	}

	inline static int32_t get_offset_of_gestureType_1() { return static_cast<int32_t>(offsetof(MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074, ___gestureType_1)); }
	inline int32_t get_gestureType_1() const { return ___gestureType_1; }
	inline int32_t* get_address_of_gestureType_1() { return &___gestureType_1; }
	inline void set_gestureType_1(int32_t value)
	{
		___gestureType_1 = value;
	}

	inline static int32_t get_offset_of_action_2() { return static_cast<int32_t>(offsetof(MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074, ___action_2)); }
	inline MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  get_action_2() const { return ___action_2; }
	inline MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073 * get_address_of_action_2() { return &___action_2; }
	inline void set_action_2(MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  value)
	{
		___action_2 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___action_2))->___description_2), (void*)NULL);
	}
};

// Native definition for P/Invoke marshalling of Microsoft.MixedReality.Toolkit.Input.MixedRealityGestureMapping
struct MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074_marshaled_pinvoke
{
	char* ___description_0;
	int32_t ___gestureType_1;
	MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073_marshaled_pinvoke ___action_2;
};
// Native definition for COM marshalling of Microsoft.MixedReality.Toolkit.Input.MixedRealityGestureMapping
struct MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074_marshaled_com
{
	Il2CppChar* ___description_0;
	int32_t ___gestureType_1;
	MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073_marshaled_com ___action_2;
};

// Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping
struct  MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2  : public RuntimeObject
{
public:
	// System.UInt32 Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::id
	uint32_t ___id_0;
	// System.String Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::description
	String_t* ___description_1;
	// Microsoft.MixedReality.Toolkit.Utilities.AxisType Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::axisType
	int32_t ___axisType_2;
	// Microsoft.MixedReality.Toolkit.Input.DeviceInputType Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::inputType
	int32_t ___inputType_3;
	// Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::inputAction
	MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  ___inputAction_4;
	// UnityEngine.KeyCode Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::keyCode
	int32_t ___keyCode_5;
	// System.String Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::axisCodeX
	String_t* ___axisCodeX_6;
	// System.String Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::axisCodeY
	String_t* ___axisCodeY_7;
	// System.Boolean Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::invertXAxis
	bool ___invertXAxis_8;
	// System.Boolean Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::invertYAxis
	bool ___invertYAxis_9;
	// System.Boolean Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::changed
	bool ___changed_10;
	// System.Object Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::rawData
	RuntimeObject * ___rawData_11;
	// System.Boolean Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::boolData
	bool ___boolData_12;
	// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::floatData
	float ___floatData_13;
	// UnityEngine.Vector2 Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::vector2Data
	Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___vector2Data_14;
	// UnityEngine.Vector3 Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::positionData
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___positionData_15;
	// UnityEngine.Quaternion Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::rotationData
	Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___rotationData_16;
	// Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::poseData
	MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  ___poseData_17;

public:
	inline static int32_t get_offset_of_id_0() { return static_cast<int32_t>(offsetof(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2, ___id_0)); }
	inline uint32_t get_id_0() const { return ___id_0; }
	inline uint32_t* get_address_of_id_0() { return &___id_0; }
	inline void set_id_0(uint32_t value)
	{
		___id_0 = value;
	}

	inline static int32_t get_offset_of_description_1() { return static_cast<int32_t>(offsetof(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2, ___description_1)); }
	inline String_t* get_description_1() const { return ___description_1; }
	inline String_t** get_address_of_description_1() { return &___description_1; }
	inline void set_description_1(String_t* value)
	{
		___description_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___description_1), (void*)value);
	}

	inline static int32_t get_offset_of_axisType_2() { return static_cast<int32_t>(offsetof(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2, ___axisType_2)); }
	inline int32_t get_axisType_2() const { return ___axisType_2; }
	inline int32_t* get_address_of_axisType_2() { return &___axisType_2; }
	inline void set_axisType_2(int32_t value)
	{
		___axisType_2 = value;
	}

	inline static int32_t get_offset_of_inputType_3() { return static_cast<int32_t>(offsetof(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2, ___inputType_3)); }
	inline int32_t get_inputType_3() const { return ___inputType_3; }
	inline int32_t* get_address_of_inputType_3() { return &___inputType_3; }
	inline void set_inputType_3(int32_t value)
	{
		___inputType_3 = value;
	}

	inline static int32_t get_offset_of_inputAction_4() { return static_cast<int32_t>(offsetof(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2, ___inputAction_4)); }
	inline MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  get_inputAction_4() const { return ___inputAction_4; }
	inline MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073 * get_address_of_inputAction_4() { return &___inputAction_4; }
	inline void set_inputAction_4(MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  value)
	{
		___inputAction_4 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___inputAction_4))->___description_2), (void*)NULL);
	}

	inline static int32_t get_offset_of_keyCode_5() { return static_cast<int32_t>(offsetof(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2, ___keyCode_5)); }
	inline int32_t get_keyCode_5() const { return ___keyCode_5; }
	inline int32_t* get_address_of_keyCode_5() { return &___keyCode_5; }
	inline void set_keyCode_5(int32_t value)
	{
		___keyCode_5 = value;
	}

	inline static int32_t get_offset_of_axisCodeX_6() { return static_cast<int32_t>(offsetof(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2, ___axisCodeX_6)); }
	inline String_t* get_axisCodeX_6() const { return ___axisCodeX_6; }
	inline String_t** get_address_of_axisCodeX_6() { return &___axisCodeX_6; }
	inline void set_axisCodeX_6(String_t* value)
	{
		___axisCodeX_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___axisCodeX_6), (void*)value);
	}

	inline static int32_t get_offset_of_axisCodeY_7() { return static_cast<int32_t>(offsetof(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2, ___axisCodeY_7)); }
	inline String_t* get_axisCodeY_7() const { return ___axisCodeY_7; }
	inline String_t** get_address_of_axisCodeY_7() { return &___axisCodeY_7; }
	inline void set_axisCodeY_7(String_t* value)
	{
		___axisCodeY_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___axisCodeY_7), (void*)value);
	}

	inline static int32_t get_offset_of_invertXAxis_8() { return static_cast<int32_t>(offsetof(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2, ___invertXAxis_8)); }
	inline bool get_invertXAxis_8() const { return ___invertXAxis_8; }
	inline bool* get_address_of_invertXAxis_8() { return &___invertXAxis_8; }
	inline void set_invertXAxis_8(bool value)
	{
		___invertXAxis_8 = value;
	}

	inline static int32_t get_offset_of_invertYAxis_9() { return static_cast<int32_t>(offsetof(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2, ___invertYAxis_9)); }
	inline bool get_invertYAxis_9() const { return ___invertYAxis_9; }
	inline bool* get_address_of_invertYAxis_9() { return &___invertYAxis_9; }
	inline void set_invertYAxis_9(bool value)
	{
		___invertYAxis_9 = value;
	}

	inline static int32_t get_offset_of_changed_10() { return static_cast<int32_t>(offsetof(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2, ___changed_10)); }
	inline bool get_changed_10() const { return ___changed_10; }
	inline bool* get_address_of_changed_10() { return &___changed_10; }
	inline void set_changed_10(bool value)
	{
		___changed_10 = value;
	}

	inline static int32_t get_offset_of_rawData_11() { return static_cast<int32_t>(offsetof(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2, ___rawData_11)); }
	inline RuntimeObject * get_rawData_11() const { return ___rawData_11; }
	inline RuntimeObject ** get_address_of_rawData_11() { return &___rawData_11; }
	inline void set_rawData_11(RuntimeObject * value)
	{
		___rawData_11 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___rawData_11), (void*)value);
	}

	inline static int32_t get_offset_of_boolData_12() { return static_cast<int32_t>(offsetof(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2, ___boolData_12)); }
	inline bool get_boolData_12() const { return ___boolData_12; }
	inline bool* get_address_of_boolData_12() { return &___boolData_12; }
	inline void set_boolData_12(bool value)
	{
		___boolData_12 = value;
	}

	inline static int32_t get_offset_of_floatData_13() { return static_cast<int32_t>(offsetof(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2, ___floatData_13)); }
	inline float get_floatData_13() const { return ___floatData_13; }
	inline float* get_address_of_floatData_13() { return &___floatData_13; }
	inline void set_floatData_13(float value)
	{
		___floatData_13 = value;
	}

	inline static int32_t get_offset_of_vector2Data_14() { return static_cast<int32_t>(offsetof(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2, ___vector2Data_14)); }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  get_vector2Data_14() const { return ___vector2Data_14; }
	inline Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * get_address_of_vector2Data_14() { return &___vector2Data_14; }
	inline void set_vector2Data_14(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  value)
	{
		___vector2Data_14 = value;
	}

	inline static int32_t get_offset_of_positionData_15() { return static_cast<int32_t>(offsetof(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2, ___positionData_15)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_positionData_15() const { return ___positionData_15; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_positionData_15() { return &___positionData_15; }
	inline void set_positionData_15(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___positionData_15 = value;
	}

	inline static int32_t get_offset_of_rotationData_16() { return static_cast<int32_t>(offsetof(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2, ___rotationData_16)); }
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  get_rotationData_16() const { return ___rotationData_16; }
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 * get_address_of_rotationData_16() { return &___rotationData_16; }
	inline void set_rotationData_16(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  value)
	{
		___rotationData_16 = value;
	}

	inline static int32_t get_offset_of_poseData_17() { return static_cast<int32_t>(offsetof(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2, ___poseData_17)); }
	inline MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  get_poseData_17() const { return ___poseData_17; }
	inline MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * get_address_of_poseData_17() { return &___poseData_17; }
	inline void set_poseData_17(MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  value)
	{
		___poseData_17 = value;
	}
};


// Microsoft.MixedReality.Toolkit.Input.SimulatedHandData_HandJointDataGenerator
struct  HandJointDataGenerator_t70BF622884D5C475C85D34FDE76FD298FAC37955  : public MulticastDelegate_t
{
public:

public:
};


// System.Action`2<Microsoft.MixedReality.Toolkit.Input.KeyBinding_KeyType,System.Int32>
struct  Action_2_t599C81CC1C0CDFE287E5D39D3EEB3130080399E8  : public MulticastDelegate_t
{
public:

public:
};


// System.AsyncCallback
struct  AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4  : public MulticastDelegate_t
{
public:

public:
};


// UnityEngine.Behaviour
struct  Behaviour_tBDC7E9C3C898AD8348891B82D3E345801D920CA8  : public Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621
{
public:

public:
};


// UnityEngine.Transform
struct  Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA  : public Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621
{
public:

public:
};


// Microsoft.MixedReality.Toolkit.Input.MixedRealityGesturesProfile
struct  MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725  : public BaseMixedRealityProfile_tB4DC16619B37D298D22571CE017070A78EF826E8
{
public:
	// Microsoft.MixedReality.Toolkit.Windows.Input.WindowsGestureSettings Microsoft.MixedReality.Toolkit.Input.MixedRealityGesturesProfile::manipulationGestures
	int32_t ___manipulationGestures_5;
	// Microsoft.MixedReality.Toolkit.Windows.Input.WindowsGestureSettings Microsoft.MixedReality.Toolkit.Input.MixedRealityGesturesProfile::navigationGestures
	int32_t ___navigationGestures_6;
	// System.Boolean Microsoft.MixedReality.Toolkit.Input.MixedRealityGesturesProfile::useRailsNavigation
	bool ___useRailsNavigation_7;
	// Microsoft.MixedReality.Toolkit.Windows.Input.WindowsGestureSettings Microsoft.MixedReality.Toolkit.Input.MixedRealityGesturesProfile::railsNavigationGestures
	int32_t ___railsNavigationGestures_8;
	// Microsoft.MixedReality.Toolkit.Utilities.AutoStartBehavior Microsoft.MixedReality.Toolkit.Input.MixedRealityGesturesProfile::windowsGestureAutoStart
	int32_t ___windowsGestureAutoStart_9;
	// Microsoft.MixedReality.Toolkit.Input.MixedRealityGestureMapping[] Microsoft.MixedReality.Toolkit.Input.MixedRealityGesturesProfile::gestures
	MixedRealityGestureMappingU5BU5D_t2F3D7B685E29F06002C6BD2EF99A97C8DF6BD874* ___gestures_10;

public:
	inline static int32_t get_offset_of_manipulationGestures_5() { return static_cast<int32_t>(offsetof(MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725, ___manipulationGestures_5)); }
	inline int32_t get_manipulationGestures_5() const { return ___manipulationGestures_5; }
	inline int32_t* get_address_of_manipulationGestures_5() { return &___manipulationGestures_5; }
	inline void set_manipulationGestures_5(int32_t value)
	{
		___manipulationGestures_5 = value;
	}

	inline static int32_t get_offset_of_navigationGestures_6() { return static_cast<int32_t>(offsetof(MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725, ___navigationGestures_6)); }
	inline int32_t get_navigationGestures_6() const { return ___navigationGestures_6; }
	inline int32_t* get_address_of_navigationGestures_6() { return &___navigationGestures_6; }
	inline void set_navigationGestures_6(int32_t value)
	{
		___navigationGestures_6 = value;
	}

	inline static int32_t get_offset_of_useRailsNavigation_7() { return static_cast<int32_t>(offsetof(MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725, ___useRailsNavigation_7)); }
	inline bool get_useRailsNavigation_7() const { return ___useRailsNavigation_7; }
	inline bool* get_address_of_useRailsNavigation_7() { return &___useRailsNavigation_7; }
	inline void set_useRailsNavigation_7(bool value)
	{
		___useRailsNavigation_7 = value;
	}

	inline static int32_t get_offset_of_railsNavigationGestures_8() { return static_cast<int32_t>(offsetof(MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725, ___railsNavigationGestures_8)); }
	inline int32_t get_railsNavigationGestures_8() const { return ___railsNavigationGestures_8; }
	inline int32_t* get_address_of_railsNavigationGestures_8() { return &___railsNavigationGestures_8; }
	inline void set_railsNavigationGestures_8(int32_t value)
	{
		___railsNavigationGestures_8 = value;
	}

	inline static int32_t get_offset_of_windowsGestureAutoStart_9() { return static_cast<int32_t>(offsetof(MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725, ___windowsGestureAutoStart_9)); }
	inline int32_t get_windowsGestureAutoStart_9() const { return ___windowsGestureAutoStart_9; }
	inline int32_t* get_address_of_windowsGestureAutoStart_9() { return &___windowsGestureAutoStart_9; }
	inline void set_windowsGestureAutoStart_9(int32_t value)
	{
		___windowsGestureAutoStart_9 = value;
	}

	inline static int32_t get_offset_of_gestures_10() { return static_cast<int32_t>(offsetof(MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725, ___gestures_10)); }
	inline MixedRealityGestureMappingU5BU5D_t2F3D7B685E29F06002C6BD2EF99A97C8DF6BD874* get_gestures_10() const { return ___gestures_10; }
	inline MixedRealityGestureMappingU5BU5D_t2F3D7B685E29F06002C6BD2EF99A97C8DF6BD874** get_address_of_gestures_10() { return &___gestures_10; }
	inline void set_gestures_10(MixedRealityGestureMappingU5BU5D_t2F3D7B685E29F06002C6BD2EF99A97C8DF6BD874* value)
	{
		___gestures_10 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___gestures_10), (void*)value);
	}
};


// Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile
struct  MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977  : public BaseMixedRealityProfile_tB4DC16619B37D298D22571CE017070A78EF826E8
{
public:
	// UnityEngine.GameObject Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::indicatorsPrefab
	GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * ___indicatorsPrefab_5;
	// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::mouseRotationSensitivity
	float ___mouseRotationSensitivity_6;
	// System.String Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::mouseX
	String_t* ___mouseX_7;
	// System.String Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::mouseY
	String_t* ___mouseY_8;
	// System.String Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::mouseScroll
	String_t* ___mouseScroll_9;
	// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::doublePressTime
	float ___doublePressTime_10;
	// System.Boolean Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::isCameraControlEnabled
	bool ___isCameraControlEnabled_11;
	// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::mouseLookSpeed
	float ___mouseLookSpeed_12;
	// Microsoft.MixedReality.Toolkit.Input.KeyBinding Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::mouseLookButton
	KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  ___mouseLookButton_13;
	// System.Boolean Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::mouseLookToggle
	bool ___mouseLookToggle_14;
	// System.Boolean Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::isControllerLookInverted
	bool ___isControllerLookInverted_15;
	// Microsoft.MixedReality.Toolkit.Input.InputSimulationControlMode Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::currentControlMode
	int32_t ___currentControlMode_16;
	// Microsoft.MixedReality.Toolkit.Input.KeyBinding Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::fastControlKey
	KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  ___fastControlKey_17;
	// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::controlSlowSpeed
	float ___controlSlowSpeed_18;
	// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::controlFastSpeed
	float ___controlFastSpeed_19;
	// System.String Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::moveHorizontal
	String_t* ___moveHorizontal_20;
	// System.String Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::moveVertical
	String_t* ___moveVertical_21;
	// System.String Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::moveUpDown
	String_t* ___moveUpDown_22;
	// System.String Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::lookHorizontal
	String_t* ___lookHorizontal_23;
	// System.String Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::lookVertical
	String_t* ___lookVertical_24;
	// System.Boolean Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::simulateEyePosition
	bool ___simulateEyePosition_25;
	// Microsoft.MixedReality.Toolkit.Input.HandSimulationMode Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::defaultHandSimulationMode
	int32_t ___defaultHandSimulationMode_26;
	// Microsoft.MixedReality.Toolkit.Input.KeyBinding Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::toggleLeftHandKey
	KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  ___toggleLeftHandKey_27;
	// Microsoft.MixedReality.Toolkit.Input.KeyBinding Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::toggleRightHandKey
	KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  ___toggleRightHandKey_28;
	// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::handHideTimeout
	float ___handHideTimeout_29;
	// Microsoft.MixedReality.Toolkit.Input.KeyBinding Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::leftHandManipulationKey
	KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  ___leftHandManipulationKey_30;
	// Microsoft.MixedReality.Toolkit.Input.KeyBinding Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::rightHandManipulationKey
	KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  ___rightHandManipulationKey_31;
	// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::mouseHandRotationSpeed
	float ___mouseHandRotationSpeed_32;
	// Microsoft.MixedReality.Toolkit.Input.KeyBinding Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::handRotateButton
	KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  ___handRotateButton_33;
	// Microsoft.MixedReality.Toolkit.Utilities.ArticulatedHandPose_GestureId Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::defaultHandGesture
	int32_t ___defaultHandGesture_34;
	// Microsoft.MixedReality.Toolkit.Utilities.ArticulatedHandPose_GestureId Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::leftMouseHandGesture
	int32_t ___leftMouseHandGesture_35;
	// Microsoft.MixedReality.Toolkit.Utilities.ArticulatedHandPose_GestureId Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::middleMouseHandGesture
	int32_t ___middleMouseHandGesture_36;
	// Microsoft.MixedReality.Toolkit.Utilities.ArticulatedHandPose_GestureId Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::rightMouseHandGesture
	int32_t ___rightMouseHandGesture_37;
	// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::handGestureAnimationSpeed
	float ___handGestureAnimationSpeed_38;
	// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::holdStartDuration
	float ___holdStartDuration_39;
	// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::navigationStartThreshold
	float ___navigationStartThreshold_40;
	// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::defaultHandDistance
	float ___defaultHandDistance_41;
	// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::handDepthMultiplier
	float ___handDepthMultiplier_42;
	// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::handJitterAmount
	float ___handJitterAmount_43;

public:
	inline static int32_t get_offset_of_indicatorsPrefab_5() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___indicatorsPrefab_5)); }
	inline GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * get_indicatorsPrefab_5() const { return ___indicatorsPrefab_5; }
	inline GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F ** get_address_of_indicatorsPrefab_5() { return &___indicatorsPrefab_5; }
	inline void set_indicatorsPrefab_5(GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * value)
	{
		___indicatorsPrefab_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___indicatorsPrefab_5), (void*)value);
	}

	inline static int32_t get_offset_of_mouseRotationSensitivity_6() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___mouseRotationSensitivity_6)); }
	inline float get_mouseRotationSensitivity_6() const { return ___mouseRotationSensitivity_6; }
	inline float* get_address_of_mouseRotationSensitivity_6() { return &___mouseRotationSensitivity_6; }
	inline void set_mouseRotationSensitivity_6(float value)
	{
		___mouseRotationSensitivity_6 = value;
	}

	inline static int32_t get_offset_of_mouseX_7() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___mouseX_7)); }
	inline String_t* get_mouseX_7() const { return ___mouseX_7; }
	inline String_t** get_address_of_mouseX_7() { return &___mouseX_7; }
	inline void set_mouseX_7(String_t* value)
	{
		___mouseX_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___mouseX_7), (void*)value);
	}

	inline static int32_t get_offset_of_mouseY_8() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___mouseY_8)); }
	inline String_t* get_mouseY_8() const { return ___mouseY_8; }
	inline String_t** get_address_of_mouseY_8() { return &___mouseY_8; }
	inline void set_mouseY_8(String_t* value)
	{
		___mouseY_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___mouseY_8), (void*)value);
	}

	inline static int32_t get_offset_of_mouseScroll_9() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___mouseScroll_9)); }
	inline String_t* get_mouseScroll_9() const { return ___mouseScroll_9; }
	inline String_t** get_address_of_mouseScroll_9() { return &___mouseScroll_9; }
	inline void set_mouseScroll_9(String_t* value)
	{
		___mouseScroll_9 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___mouseScroll_9), (void*)value);
	}

	inline static int32_t get_offset_of_doublePressTime_10() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___doublePressTime_10)); }
	inline float get_doublePressTime_10() const { return ___doublePressTime_10; }
	inline float* get_address_of_doublePressTime_10() { return &___doublePressTime_10; }
	inline void set_doublePressTime_10(float value)
	{
		___doublePressTime_10 = value;
	}

	inline static int32_t get_offset_of_isCameraControlEnabled_11() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___isCameraControlEnabled_11)); }
	inline bool get_isCameraControlEnabled_11() const { return ___isCameraControlEnabled_11; }
	inline bool* get_address_of_isCameraControlEnabled_11() { return &___isCameraControlEnabled_11; }
	inline void set_isCameraControlEnabled_11(bool value)
	{
		___isCameraControlEnabled_11 = value;
	}

	inline static int32_t get_offset_of_mouseLookSpeed_12() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___mouseLookSpeed_12)); }
	inline float get_mouseLookSpeed_12() const { return ___mouseLookSpeed_12; }
	inline float* get_address_of_mouseLookSpeed_12() { return &___mouseLookSpeed_12; }
	inline void set_mouseLookSpeed_12(float value)
	{
		___mouseLookSpeed_12 = value;
	}

	inline static int32_t get_offset_of_mouseLookButton_13() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___mouseLookButton_13)); }
	inline KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  get_mouseLookButton_13() const { return ___mouseLookButton_13; }
	inline KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 * get_address_of_mouseLookButton_13() { return &___mouseLookButton_13; }
	inline void set_mouseLookButton_13(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  value)
	{
		___mouseLookButton_13 = value;
	}

	inline static int32_t get_offset_of_mouseLookToggle_14() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___mouseLookToggle_14)); }
	inline bool get_mouseLookToggle_14() const { return ___mouseLookToggle_14; }
	inline bool* get_address_of_mouseLookToggle_14() { return &___mouseLookToggle_14; }
	inline void set_mouseLookToggle_14(bool value)
	{
		___mouseLookToggle_14 = value;
	}

	inline static int32_t get_offset_of_isControllerLookInverted_15() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___isControllerLookInverted_15)); }
	inline bool get_isControllerLookInverted_15() const { return ___isControllerLookInverted_15; }
	inline bool* get_address_of_isControllerLookInverted_15() { return &___isControllerLookInverted_15; }
	inline void set_isControllerLookInverted_15(bool value)
	{
		___isControllerLookInverted_15 = value;
	}

	inline static int32_t get_offset_of_currentControlMode_16() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___currentControlMode_16)); }
	inline int32_t get_currentControlMode_16() const { return ___currentControlMode_16; }
	inline int32_t* get_address_of_currentControlMode_16() { return &___currentControlMode_16; }
	inline void set_currentControlMode_16(int32_t value)
	{
		___currentControlMode_16 = value;
	}

	inline static int32_t get_offset_of_fastControlKey_17() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___fastControlKey_17)); }
	inline KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  get_fastControlKey_17() const { return ___fastControlKey_17; }
	inline KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 * get_address_of_fastControlKey_17() { return &___fastControlKey_17; }
	inline void set_fastControlKey_17(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  value)
	{
		___fastControlKey_17 = value;
	}

	inline static int32_t get_offset_of_controlSlowSpeed_18() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___controlSlowSpeed_18)); }
	inline float get_controlSlowSpeed_18() const { return ___controlSlowSpeed_18; }
	inline float* get_address_of_controlSlowSpeed_18() { return &___controlSlowSpeed_18; }
	inline void set_controlSlowSpeed_18(float value)
	{
		___controlSlowSpeed_18 = value;
	}

	inline static int32_t get_offset_of_controlFastSpeed_19() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___controlFastSpeed_19)); }
	inline float get_controlFastSpeed_19() const { return ___controlFastSpeed_19; }
	inline float* get_address_of_controlFastSpeed_19() { return &___controlFastSpeed_19; }
	inline void set_controlFastSpeed_19(float value)
	{
		___controlFastSpeed_19 = value;
	}

	inline static int32_t get_offset_of_moveHorizontal_20() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___moveHorizontal_20)); }
	inline String_t* get_moveHorizontal_20() const { return ___moveHorizontal_20; }
	inline String_t** get_address_of_moveHorizontal_20() { return &___moveHorizontal_20; }
	inline void set_moveHorizontal_20(String_t* value)
	{
		___moveHorizontal_20 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___moveHorizontal_20), (void*)value);
	}

	inline static int32_t get_offset_of_moveVertical_21() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___moveVertical_21)); }
	inline String_t* get_moveVertical_21() const { return ___moveVertical_21; }
	inline String_t** get_address_of_moveVertical_21() { return &___moveVertical_21; }
	inline void set_moveVertical_21(String_t* value)
	{
		___moveVertical_21 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___moveVertical_21), (void*)value);
	}

	inline static int32_t get_offset_of_moveUpDown_22() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___moveUpDown_22)); }
	inline String_t* get_moveUpDown_22() const { return ___moveUpDown_22; }
	inline String_t** get_address_of_moveUpDown_22() { return &___moveUpDown_22; }
	inline void set_moveUpDown_22(String_t* value)
	{
		___moveUpDown_22 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___moveUpDown_22), (void*)value);
	}

	inline static int32_t get_offset_of_lookHorizontal_23() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___lookHorizontal_23)); }
	inline String_t* get_lookHorizontal_23() const { return ___lookHorizontal_23; }
	inline String_t** get_address_of_lookHorizontal_23() { return &___lookHorizontal_23; }
	inline void set_lookHorizontal_23(String_t* value)
	{
		___lookHorizontal_23 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___lookHorizontal_23), (void*)value);
	}

	inline static int32_t get_offset_of_lookVertical_24() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___lookVertical_24)); }
	inline String_t* get_lookVertical_24() const { return ___lookVertical_24; }
	inline String_t** get_address_of_lookVertical_24() { return &___lookVertical_24; }
	inline void set_lookVertical_24(String_t* value)
	{
		___lookVertical_24 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___lookVertical_24), (void*)value);
	}

	inline static int32_t get_offset_of_simulateEyePosition_25() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___simulateEyePosition_25)); }
	inline bool get_simulateEyePosition_25() const { return ___simulateEyePosition_25; }
	inline bool* get_address_of_simulateEyePosition_25() { return &___simulateEyePosition_25; }
	inline void set_simulateEyePosition_25(bool value)
	{
		___simulateEyePosition_25 = value;
	}

	inline static int32_t get_offset_of_defaultHandSimulationMode_26() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___defaultHandSimulationMode_26)); }
	inline int32_t get_defaultHandSimulationMode_26() const { return ___defaultHandSimulationMode_26; }
	inline int32_t* get_address_of_defaultHandSimulationMode_26() { return &___defaultHandSimulationMode_26; }
	inline void set_defaultHandSimulationMode_26(int32_t value)
	{
		___defaultHandSimulationMode_26 = value;
	}

	inline static int32_t get_offset_of_toggleLeftHandKey_27() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___toggleLeftHandKey_27)); }
	inline KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  get_toggleLeftHandKey_27() const { return ___toggleLeftHandKey_27; }
	inline KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 * get_address_of_toggleLeftHandKey_27() { return &___toggleLeftHandKey_27; }
	inline void set_toggleLeftHandKey_27(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  value)
	{
		___toggleLeftHandKey_27 = value;
	}

	inline static int32_t get_offset_of_toggleRightHandKey_28() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___toggleRightHandKey_28)); }
	inline KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  get_toggleRightHandKey_28() const { return ___toggleRightHandKey_28; }
	inline KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 * get_address_of_toggleRightHandKey_28() { return &___toggleRightHandKey_28; }
	inline void set_toggleRightHandKey_28(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  value)
	{
		___toggleRightHandKey_28 = value;
	}

	inline static int32_t get_offset_of_handHideTimeout_29() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___handHideTimeout_29)); }
	inline float get_handHideTimeout_29() const { return ___handHideTimeout_29; }
	inline float* get_address_of_handHideTimeout_29() { return &___handHideTimeout_29; }
	inline void set_handHideTimeout_29(float value)
	{
		___handHideTimeout_29 = value;
	}

	inline static int32_t get_offset_of_leftHandManipulationKey_30() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___leftHandManipulationKey_30)); }
	inline KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  get_leftHandManipulationKey_30() const { return ___leftHandManipulationKey_30; }
	inline KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 * get_address_of_leftHandManipulationKey_30() { return &___leftHandManipulationKey_30; }
	inline void set_leftHandManipulationKey_30(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  value)
	{
		___leftHandManipulationKey_30 = value;
	}

	inline static int32_t get_offset_of_rightHandManipulationKey_31() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___rightHandManipulationKey_31)); }
	inline KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  get_rightHandManipulationKey_31() const { return ___rightHandManipulationKey_31; }
	inline KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 * get_address_of_rightHandManipulationKey_31() { return &___rightHandManipulationKey_31; }
	inline void set_rightHandManipulationKey_31(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  value)
	{
		___rightHandManipulationKey_31 = value;
	}

	inline static int32_t get_offset_of_mouseHandRotationSpeed_32() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___mouseHandRotationSpeed_32)); }
	inline float get_mouseHandRotationSpeed_32() const { return ___mouseHandRotationSpeed_32; }
	inline float* get_address_of_mouseHandRotationSpeed_32() { return &___mouseHandRotationSpeed_32; }
	inline void set_mouseHandRotationSpeed_32(float value)
	{
		___mouseHandRotationSpeed_32 = value;
	}

	inline static int32_t get_offset_of_handRotateButton_33() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___handRotateButton_33)); }
	inline KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  get_handRotateButton_33() const { return ___handRotateButton_33; }
	inline KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 * get_address_of_handRotateButton_33() { return &___handRotateButton_33; }
	inline void set_handRotateButton_33(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  value)
	{
		___handRotateButton_33 = value;
	}

	inline static int32_t get_offset_of_defaultHandGesture_34() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___defaultHandGesture_34)); }
	inline int32_t get_defaultHandGesture_34() const { return ___defaultHandGesture_34; }
	inline int32_t* get_address_of_defaultHandGesture_34() { return &___defaultHandGesture_34; }
	inline void set_defaultHandGesture_34(int32_t value)
	{
		___defaultHandGesture_34 = value;
	}

	inline static int32_t get_offset_of_leftMouseHandGesture_35() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___leftMouseHandGesture_35)); }
	inline int32_t get_leftMouseHandGesture_35() const { return ___leftMouseHandGesture_35; }
	inline int32_t* get_address_of_leftMouseHandGesture_35() { return &___leftMouseHandGesture_35; }
	inline void set_leftMouseHandGesture_35(int32_t value)
	{
		___leftMouseHandGesture_35 = value;
	}

	inline static int32_t get_offset_of_middleMouseHandGesture_36() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___middleMouseHandGesture_36)); }
	inline int32_t get_middleMouseHandGesture_36() const { return ___middleMouseHandGesture_36; }
	inline int32_t* get_address_of_middleMouseHandGesture_36() { return &___middleMouseHandGesture_36; }
	inline void set_middleMouseHandGesture_36(int32_t value)
	{
		___middleMouseHandGesture_36 = value;
	}

	inline static int32_t get_offset_of_rightMouseHandGesture_37() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___rightMouseHandGesture_37)); }
	inline int32_t get_rightMouseHandGesture_37() const { return ___rightMouseHandGesture_37; }
	inline int32_t* get_address_of_rightMouseHandGesture_37() { return &___rightMouseHandGesture_37; }
	inline void set_rightMouseHandGesture_37(int32_t value)
	{
		___rightMouseHandGesture_37 = value;
	}

	inline static int32_t get_offset_of_handGestureAnimationSpeed_38() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___handGestureAnimationSpeed_38)); }
	inline float get_handGestureAnimationSpeed_38() const { return ___handGestureAnimationSpeed_38; }
	inline float* get_address_of_handGestureAnimationSpeed_38() { return &___handGestureAnimationSpeed_38; }
	inline void set_handGestureAnimationSpeed_38(float value)
	{
		___handGestureAnimationSpeed_38 = value;
	}

	inline static int32_t get_offset_of_holdStartDuration_39() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___holdStartDuration_39)); }
	inline float get_holdStartDuration_39() const { return ___holdStartDuration_39; }
	inline float* get_address_of_holdStartDuration_39() { return &___holdStartDuration_39; }
	inline void set_holdStartDuration_39(float value)
	{
		___holdStartDuration_39 = value;
	}

	inline static int32_t get_offset_of_navigationStartThreshold_40() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___navigationStartThreshold_40)); }
	inline float get_navigationStartThreshold_40() const { return ___navigationStartThreshold_40; }
	inline float* get_address_of_navigationStartThreshold_40() { return &___navigationStartThreshold_40; }
	inline void set_navigationStartThreshold_40(float value)
	{
		___navigationStartThreshold_40 = value;
	}

	inline static int32_t get_offset_of_defaultHandDistance_41() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___defaultHandDistance_41)); }
	inline float get_defaultHandDistance_41() const { return ___defaultHandDistance_41; }
	inline float* get_address_of_defaultHandDistance_41() { return &___defaultHandDistance_41; }
	inline void set_defaultHandDistance_41(float value)
	{
		___defaultHandDistance_41 = value;
	}

	inline static int32_t get_offset_of_handDepthMultiplier_42() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___handDepthMultiplier_42)); }
	inline float get_handDepthMultiplier_42() const { return ___handDepthMultiplier_42; }
	inline float* get_address_of_handDepthMultiplier_42() { return &___handDepthMultiplier_42; }
	inline void set_handDepthMultiplier_42(float value)
	{
		___handDepthMultiplier_42 = value;
	}

	inline static int32_t get_offset_of_handJitterAmount_43() { return static_cast<int32_t>(offsetof(MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977, ___handJitterAmount_43)); }
	inline float get_handJitterAmount_43() const { return ___handJitterAmount_43; }
	inline float* get_address_of_handJitterAmount_43() { return &___handJitterAmount_43; }
	inline void set_handJitterAmount_43(float value)
	{
		___handJitterAmount_43 = value;
	}
};


// Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSystemProfile
struct  MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2  : public BaseMixedRealityProfile_tB4DC16619B37D298D22571CE017070A78EF826E8
{
public:
	// Microsoft.MixedReality.Toolkit.Input.MixedRealityInputDataProviderConfiguration[] Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSystemProfile::dataProviderConfigurations
	MixedRealityInputDataProviderConfigurationU5BU5D_t621D14E0DCEE987398F574E5335D55FDBDFBE426* ___dataProviderConfigurations_5;
	// Microsoft.MixedReality.Toolkit.Utilities.SystemType Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSystemProfile::focusProviderType
	SystemType_t9696BD865921F75894EBD4D6EF913158A8BF3432 * ___focusProviderType_6;
	// Microsoft.MixedReality.Toolkit.Utilities.SystemType Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSystemProfile::raycastProviderType
	SystemType_t9696BD865921F75894EBD4D6EF913158A8BF3432 * ___raycastProviderType_7;
	// System.Int32 Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSystemProfile::focusQueryBufferSize
	int32_t ___focusQueryBufferSize_8;
	// System.Boolean Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSystemProfile::focusIndividualCompoundCollider
	bool ___focusIndividualCompoundCollider_9;
	// Microsoft.MixedReality.Toolkit.Input.MixedRealityInputActionsProfile Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSystemProfile::inputActionsProfile
	MixedRealityInputActionsProfile_t5D05F56AB25081BDE6B4697C8DF609F2A458EA12 * ___inputActionsProfile_10;
	// Microsoft.MixedReality.Toolkit.Input.MixedRealityInputActionRulesProfile Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSystemProfile::inputActionRulesProfile
	MixedRealityInputActionRulesProfile_t4CE915FD59B3CEE0DDE18E9BF5922E5628DCCD3A * ___inputActionRulesProfile_11;
	// Microsoft.MixedReality.Toolkit.Input.MixedRealityPointerProfile Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSystemProfile::pointerProfile
	MixedRealityPointerProfile_t006E66A5D042614269E3195CA2042A3AC964671E * ___pointerProfile_12;
	// Microsoft.MixedReality.Toolkit.Input.MixedRealityGesturesProfile Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSystemProfile::gesturesProfile
	MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725 * ___gesturesProfile_13;
	// System.Collections.Generic.List`1<System.Globalization.CultureInfo> Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSystemProfile::supportedVoiceCultures
	List_1_t74F59DD36FAE0CFB087612565C42CAD359647832 * ___supportedVoiceCultures_14;
	// Microsoft.MixedReality.Toolkit.Input.MixedRealitySpeechCommandsProfile Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSystemProfile::speechCommandsProfile
	MixedRealitySpeechCommandsProfile_t73EF505A304D3B94E486F30B64A9A002FBD858CD * ___speechCommandsProfile_15;
	// System.Boolean Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSystemProfile::enableControllerMapping
	bool ___enableControllerMapping_16;
	// Microsoft.MixedReality.Toolkit.Input.MixedRealityControllerMappingProfile Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSystemProfile::controllerMappingProfile
	MixedRealityControllerMappingProfile_t254FF0C8E28DA6341E36D23A9DD832B11ACCE9DB * ___controllerMappingProfile_17;
	// Microsoft.MixedReality.Toolkit.Input.MixedRealityControllerVisualizationProfile Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSystemProfile::controllerVisualizationProfile
	MixedRealityControllerVisualizationProfile_tDF62A9AB730F36F2AF8454D32ECF008D046E899D * ___controllerVisualizationProfile_18;
	// Microsoft.MixedReality.Toolkit.Input.MixedRealityHandTrackingProfile Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSystemProfile::handTrackingProfile
	MixedRealityHandTrackingProfile_tFA3A9118040918D9E221EEB94786E3A333A12E36 * ___handTrackingProfile_19;

public:
	inline static int32_t get_offset_of_dataProviderConfigurations_5() { return static_cast<int32_t>(offsetof(MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2, ___dataProviderConfigurations_5)); }
	inline MixedRealityInputDataProviderConfigurationU5BU5D_t621D14E0DCEE987398F574E5335D55FDBDFBE426* get_dataProviderConfigurations_5() const { return ___dataProviderConfigurations_5; }
	inline MixedRealityInputDataProviderConfigurationU5BU5D_t621D14E0DCEE987398F574E5335D55FDBDFBE426** get_address_of_dataProviderConfigurations_5() { return &___dataProviderConfigurations_5; }
	inline void set_dataProviderConfigurations_5(MixedRealityInputDataProviderConfigurationU5BU5D_t621D14E0DCEE987398F574E5335D55FDBDFBE426* value)
	{
		___dataProviderConfigurations_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___dataProviderConfigurations_5), (void*)value);
	}

	inline static int32_t get_offset_of_focusProviderType_6() { return static_cast<int32_t>(offsetof(MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2, ___focusProviderType_6)); }
	inline SystemType_t9696BD865921F75894EBD4D6EF913158A8BF3432 * get_focusProviderType_6() const { return ___focusProviderType_6; }
	inline SystemType_t9696BD865921F75894EBD4D6EF913158A8BF3432 ** get_address_of_focusProviderType_6() { return &___focusProviderType_6; }
	inline void set_focusProviderType_6(SystemType_t9696BD865921F75894EBD4D6EF913158A8BF3432 * value)
	{
		___focusProviderType_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___focusProviderType_6), (void*)value);
	}

	inline static int32_t get_offset_of_raycastProviderType_7() { return static_cast<int32_t>(offsetof(MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2, ___raycastProviderType_7)); }
	inline SystemType_t9696BD865921F75894EBD4D6EF913158A8BF3432 * get_raycastProviderType_7() const { return ___raycastProviderType_7; }
	inline SystemType_t9696BD865921F75894EBD4D6EF913158A8BF3432 ** get_address_of_raycastProviderType_7() { return &___raycastProviderType_7; }
	inline void set_raycastProviderType_7(SystemType_t9696BD865921F75894EBD4D6EF913158A8BF3432 * value)
	{
		___raycastProviderType_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___raycastProviderType_7), (void*)value);
	}

	inline static int32_t get_offset_of_focusQueryBufferSize_8() { return static_cast<int32_t>(offsetof(MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2, ___focusQueryBufferSize_8)); }
	inline int32_t get_focusQueryBufferSize_8() const { return ___focusQueryBufferSize_8; }
	inline int32_t* get_address_of_focusQueryBufferSize_8() { return &___focusQueryBufferSize_8; }
	inline void set_focusQueryBufferSize_8(int32_t value)
	{
		___focusQueryBufferSize_8 = value;
	}

	inline static int32_t get_offset_of_focusIndividualCompoundCollider_9() { return static_cast<int32_t>(offsetof(MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2, ___focusIndividualCompoundCollider_9)); }
	inline bool get_focusIndividualCompoundCollider_9() const { return ___focusIndividualCompoundCollider_9; }
	inline bool* get_address_of_focusIndividualCompoundCollider_9() { return &___focusIndividualCompoundCollider_9; }
	inline void set_focusIndividualCompoundCollider_9(bool value)
	{
		___focusIndividualCompoundCollider_9 = value;
	}

	inline static int32_t get_offset_of_inputActionsProfile_10() { return static_cast<int32_t>(offsetof(MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2, ___inputActionsProfile_10)); }
	inline MixedRealityInputActionsProfile_t5D05F56AB25081BDE6B4697C8DF609F2A458EA12 * get_inputActionsProfile_10() const { return ___inputActionsProfile_10; }
	inline MixedRealityInputActionsProfile_t5D05F56AB25081BDE6B4697C8DF609F2A458EA12 ** get_address_of_inputActionsProfile_10() { return &___inputActionsProfile_10; }
	inline void set_inputActionsProfile_10(MixedRealityInputActionsProfile_t5D05F56AB25081BDE6B4697C8DF609F2A458EA12 * value)
	{
		___inputActionsProfile_10 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___inputActionsProfile_10), (void*)value);
	}

	inline static int32_t get_offset_of_inputActionRulesProfile_11() { return static_cast<int32_t>(offsetof(MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2, ___inputActionRulesProfile_11)); }
	inline MixedRealityInputActionRulesProfile_t4CE915FD59B3CEE0DDE18E9BF5922E5628DCCD3A * get_inputActionRulesProfile_11() const { return ___inputActionRulesProfile_11; }
	inline MixedRealityInputActionRulesProfile_t4CE915FD59B3CEE0DDE18E9BF5922E5628DCCD3A ** get_address_of_inputActionRulesProfile_11() { return &___inputActionRulesProfile_11; }
	inline void set_inputActionRulesProfile_11(MixedRealityInputActionRulesProfile_t4CE915FD59B3CEE0DDE18E9BF5922E5628DCCD3A * value)
	{
		___inputActionRulesProfile_11 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___inputActionRulesProfile_11), (void*)value);
	}

	inline static int32_t get_offset_of_pointerProfile_12() { return static_cast<int32_t>(offsetof(MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2, ___pointerProfile_12)); }
	inline MixedRealityPointerProfile_t006E66A5D042614269E3195CA2042A3AC964671E * get_pointerProfile_12() const { return ___pointerProfile_12; }
	inline MixedRealityPointerProfile_t006E66A5D042614269E3195CA2042A3AC964671E ** get_address_of_pointerProfile_12() { return &___pointerProfile_12; }
	inline void set_pointerProfile_12(MixedRealityPointerProfile_t006E66A5D042614269E3195CA2042A3AC964671E * value)
	{
		___pointerProfile_12 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___pointerProfile_12), (void*)value);
	}

	inline static int32_t get_offset_of_gesturesProfile_13() { return static_cast<int32_t>(offsetof(MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2, ___gesturesProfile_13)); }
	inline MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725 * get_gesturesProfile_13() const { return ___gesturesProfile_13; }
	inline MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725 ** get_address_of_gesturesProfile_13() { return &___gesturesProfile_13; }
	inline void set_gesturesProfile_13(MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725 * value)
	{
		___gesturesProfile_13 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___gesturesProfile_13), (void*)value);
	}

	inline static int32_t get_offset_of_supportedVoiceCultures_14() { return static_cast<int32_t>(offsetof(MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2, ___supportedVoiceCultures_14)); }
	inline List_1_t74F59DD36FAE0CFB087612565C42CAD359647832 * get_supportedVoiceCultures_14() const { return ___supportedVoiceCultures_14; }
	inline List_1_t74F59DD36FAE0CFB087612565C42CAD359647832 ** get_address_of_supportedVoiceCultures_14() { return &___supportedVoiceCultures_14; }
	inline void set_supportedVoiceCultures_14(List_1_t74F59DD36FAE0CFB087612565C42CAD359647832 * value)
	{
		___supportedVoiceCultures_14 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___supportedVoiceCultures_14), (void*)value);
	}

	inline static int32_t get_offset_of_speechCommandsProfile_15() { return static_cast<int32_t>(offsetof(MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2, ___speechCommandsProfile_15)); }
	inline MixedRealitySpeechCommandsProfile_t73EF505A304D3B94E486F30B64A9A002FBD858CD * get_speechCommandsProfile_15() const { return ___speechCommandsProfile_15; }
	inline MixedRealitySpeechCommandsProfile_t73EF505A304D3B94E486F30B64A9A002FBD858CD ** get_address_of_speechCommandsProfile_15() { return &___speechCommandsProfile_15; }
	inline void set_speechCommandsProfile_15(MixedRealitySpeechCommandsProfile_t73EF505A304D3B94E486F30B64A9A002FBD858CD * value)
	{
		___speechCommandsProfile_15 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___speechCommandsProfile_15), (void*)value);
	}

	inline static int32_t get_offset_of_enableControllerMapping_16() { return static_cast<int32_t>(offsetof(MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2, ___enableControllerMapping_16)); }
	inline bool get_enableControllerMapping_16() const { return ___enableControllerMapping_16; }
	inline bool* get_address_of_enableControllerMapping_16() { return &___enableControllerMapping_16; }
	inline void set_enableControllerMapping_16(bool value)
	{
		___enableControllerMapping_16 = value;
	}

	inline static int32_t get_offset_of_controllerMappingProfile_17() { return static_cast<int32_t>(offsetof(MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2, ___controllerMappingProfile_17)); }
	inline MixedRealityControllerMappingProfile_t254FF0C8E28DA6341E36D23A9DD832B11ACCE9DB * get_controllerMappingProfile_17() const { return ___controllerMappingProfile_17; }
	inline MixedRealityControllerMappingProfile_t254FF0C8E28DA6341E36D23A9DD832B11ACCE9DB ** get_address_of_controllerMappingProfile_17() { return &___controllerMappingProfile_17; }
	inline void set_controllerMappingProfile_17(MixedRealityControllerMappingProfile_t254FF0C8E28DA6341E36D23A9DD832B11ACCE9DB * value)
	{
		___controllerMappingProfile_17 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___controllerMappingProfile_17), (void*)value);
	}

	inline static int32_t get_offset_of_controllerVisualizationProfile_18() { return static_cast<int32_t>(offsetof(MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2, ___controllerVisualizationProfile_18)); }
	inline MixedRealityControllerVisualizationProfile_tDF62A9AB730F36F2AF8454D32ECF008D046E899D * get_controllerVisualizationProfile_18() const { return ___controllerVisualizationProfile_18; }
	inline MixedRealityControllerVisualizationProfile_tDF62A9AB730F36F2AF8454D32ECF008D046E899D ** get_address_of_controllerVisualizationProfile_18() { return &___controllerVisualizationProfile_18; }
	inline void set_controllerVisualizationProfile_18(MixedRealityControllerVisualizationProfile_tDF62A9AB730F36F2AF8454D32ECF008D046E899D * value)
	{
		___controllerVisualizationProfile_18 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___controllerVisualizationProfile_18), (void*)value);
	}

	inline static int32_t get_offset_of_handTrackingProfile_19() { return static_cast<int32_t>(offsetof(MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2, ___handTrackingProfile_19)); }
	inline MixedRealityHandTrackingProfile_tFA3A9118040918D9E221EEB94786E3A333A12E36 * get_handTrackingProfile_19() const { return ___handTrackingProfile_19; }
	inline MixedRealityHandTrackingProfile_tFA3A9118040918D9E221EEB94786E3A333A12E36 ** get_address_of_handTrackingProfile_19() { return &___handTrackingProfile_19; }
	inline void set_handTrackingProfile_19(MixedRealityHandTrackingProfile_tFA3A9118040918D9E221EEB94786E3A333A12E36 * value)
	{
		___handTrackingProfile_19 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___handTrackingProfile_19), (void*)value);
	}
};


// Microsoft.MixedReality.Toolkit.Input.SimulatedHand
struct  SimulatedHand_tFBAB6AD39E9B16E093E63E4D2A88EA5E3415437E  : public BaseHand_tB58ECFC99FBFD516BBAA0989004A10F687078F4B
{
public:
	// System.Collections.Generic.Dictionary`2<Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose> Microsoft.MixedReality.Toolkit.Input.SimulatedHand::jointPoses
	Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 * ___jointPoses_24;

public:
	inline static int32_t get_offset_of_jointPoses_24() { return static_cast<int32_t>(offsetof(SimulatedHand_tFBAB6AD39E9B16E093E63E4D2A88EA5E3415437E, ___jointPoses_24)); }
	inline Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 * get_jointPoses_24() const { return ___jointPoses_24; }
	inline Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 ** get_address_of_jointPoses_24() { return &___jointPoses_24; }
	inline void set_jointPoses_24(Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 * value)
	{
		___jointPoses_24 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___jointPoses_24), (void*)value);
	}
};

struct SimulatedHand_tFBAB6AD39E9B16E093E63E4D2A88EA5E3415437E_StaticFields
{
public:
	// System.Int32 Microsoft.MixedReality.Toolkit.Input.SimulatedHand::jointCount
	int32_t ___jointCount_23;

public:
	inline static int32_t get_offset_of_jointCount_23() { return static_cast<int32_t>(offsetof(SimulatedHand_tFBAB6AD39E9B16E093E63E4D2A88EA5E3415437E_StaticFields, ___jointCount_23)); }
	inline int32_t get_jointCount_23() const { return ___jointCount_23; }
	inline int32_t* get_address_of_jointCount_23() { return &___jointCount_23; }
	inline void set_jointCount_23(int32_t value)
	{
		___jointCount_23 = value;
	}
};


// UnityEngine.Camera
struct  Camera_t48B2B9ECB3CE6108A98BF949A1CECF0FE3421F34  : public Behaviour_tBDC7E9C3C898AD8348891B82D3E345801D920CA8
{
public:

public:
};

struct Camera_t48B2B9ECB3CE6108A98BF949A1CECF0FE3421F34_StaticFields
{
public:
	// UnityEngine.Camera_CameraCallback UnityEngine.Camera::onPreCull
	CameraCallback_t8BBB42AA08D7498DFC11F4128117055BC7F0B9D0 * ___onPreCull_4;
	// UnityEngine.Camera_CameraCallback UnityEngine.Camera::onPreRender
	CameraCallback_t8BBB42AA08D7498DFC11F4128117055BC7F0B9D0 * ___onPreRender_5;
	// UnityEngine.Camera_CameraCallback UnityEngine.Camera::onPostRender
	CameraCallback_t8BBB42AA08D7498DFC11F4128117055BC7F0B9D0 * ___onPostRender_6;

public:
	inline static int32_t get_offset_of_onPreCull_4() { return static_cast<int32_t>(offsetof(Camera_t48B2B9ECB3CE6108A98BF949A1CECF0FE3421F34_StaticFields, ___onPreCull_4)); }
	inline CameraCallback_t8BBB42AA08D7498DFC11F4128117055BC7F0B9D0 * get_onPreCull_4() const { return ___onPreCull_4; }
	inline CameraCallback_t8BBB42AA08D7498DFC11F4128117055BC7F0B9D0 ** get_address_of_onPreCull_4() { return &___onPreCull_4; }
	inline void set_onPreCull_4(CameraCallback_t8BBB42AA08D7498DFC11F4128117055BC7F0B9D0 * value)
	{
		___onPreCull_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___onPreCull_4), (void*)value);
	}

	inline static int32_t get_offset_of_onPreRender_5() { return static_cast<int32_t>(offsetof(Camera_t48B2B9ECB3CE6108A98BF949A1CECF0FE3421F34_StaticFields, ___onPreRender_5)); }
	inline CameraCallback_t8BBB42AA08D7498DFC11F4128117055BC7F0B9D0 * get_onPreRender_5() const { return ___onPreRender_5; }
	inline CameraCallback_t8BBB42AA08D7498DFC11F4128117055BC7F0B9D0 ** get_address_of_onPreRender_5() { return &___onPreRender_5; }
	inline void set_onPreRender_5(CameraCallback_t8BBB42AA08D7498DFC11F4128117055BC7F0B9D0 * value)
	{
		___onPreRender_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___onPreRender_5), (void*)value);
	}

	inline static int32_t get_offset_of_onPostRender_6() { return static_cast<int32_t>(offsetof(Camera_t48B2B9ECB3CE6108A98BF949A1CECF0FE3421F34_StaticFields, ___onPostRender_6)); }
	inline CameraCallback_t8BBB42AA08D7498DFC11F4128117055BC7F0B9D0 * get_onPostRender_6() const { return ___onPostRender_6; }
	inline CameraCallback_t8BBB42AA08D7498DFC11F4128117055BC7F0B9D0 ** get_address_of_onPostRender_6() { return &___onPostRender_6; }
	inline void set_onPostRender_6(CameraCallback_t8BBB42AA08D7498DFC11F4128117055BC7F0B9D0 * value)
	{
		___onPostRender_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___onPostRender_6), (void*)value);
	}
};


// Microsoft.MixedReality.Toolkit.Input.SimulatedArticulatedHand
struct  SimulatedArticulatedHand_tE70788F371CF5A48A99B3DE695FFA7A0FEF6E2E9  : public SimulatedHand_tFBAB6AD39E9B16E093E63E4D2A88EA5E3415437E
{
public:
	// UnityEngine.Vector3 Microsoft.MixedReality.Toolkit.Input.SimulatedArticulatedHand::currentPointerPosition
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___currentPointerPosition_25;
	// UnityEngine.Quaternion Microsoft.MixedReality.Toolkit.Input.SimulatedArticulatedHand::currentPointerRotation
	Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___currentPointerRotation_26;
	// Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose Microsoft.MixedReality.Toolkit.Input.SimulatedArticulatedHand::lastPointerPose
	MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  ___lastPointerPose_27;
	// Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose Microsoft.MixedReality.Toolkit.Input.SimulatedArticulatedHand::currentPointerPose
	MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  ___currentPointerPose_28;
	// Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose Microsoft.MixedReality.Toolkit.Input.SimulatedArticulatedHand::currentIndexPose
	MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  ___currentIndexPose_29;
	// Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose Microsoft.MixedReality.Toolkit.Input.SimulatedArticulatedHand::currentGripPose
	MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  ___currentGripPose_30;
	// Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose Microsoft.MixedReality.Toolkit.Input.SimulatedArticulatedHand::lastGripPose
	MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  ___lastGripPose_31;

public:
	inline static int32_t get_offset_of_currentPointerPosition_25() { return static_cast<int32_t>(offsetof(SimulatedArticulatedHand_tE70788F371CF5A48A99B3DE695FFA7A0FEF6E2E9, ___currentPointerPosition_25)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_currentPointerPosition_25() const { return ___currentPointerPosition_25; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_currentPointerPosition_25() { return &___currentPointerPosition_25; }
	inline void set_currentPointerPosition_25(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___currentPointerPosition_25 = value;
	}

	inline static int32_t get_offset_of_currentPointerRotation_26() { return static_cast<int32_t>(offsetof(SimulatedArticulatedHand_tE70788F371CF5A48A99B3DE695FFA7A0FEF6E2E9, ___currentPointerRotation_26)); }
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  get_currentPointerRotation_26() const { return ___currentPointerRotation_26; }
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 * get_address_of_currentPointerRotation_26() { return &___currentPointerRotation_26; }
	inline void set_currentPointerRotation_26(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  value)
	{
		___currentPointerRotation_26 = value;
	}

	inline static int32_t get_offset_of_lastPointerPose_27() { return static_cast<int32_t>(offsetof(SimulatedArticulatedHand_tE70788F371CF5A48A99B3DE695FFA7A0FEF6E2E9, ___lastPointerPose_27)); }
	inline MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  get_lastPointerPose_27() const { return ___lastPointerPose_27; }
	inline MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * get_address_of_lastPointerPose_27() { return &___lastPointerPose_27; }
	inline void set_lastPointerPose_27(MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  value)
	{
		___lastPointerPose_27 = value;
	}

	inline static int32_t get_offset_of_currentPointerPose_28() { return static_cast<int32_t>(offsetof(SimulatedArticulatedHand_tE70788F371CF5A48A99B3DE695FFA7A0FEF6E2E9, ___currentPointerPose_28)); }
	inline MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  get_currentPointerPose_28() const { return ___currentPointerPose_28; }
	inline MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * get_address_of_currentPointerPose_28() { return &___currentPointerPose_28; }
	inline void set_currentPointerPose_28(MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  value)
	{
		___currentPointerPose_28 = value;
	}

	inline static int32_t get_offset_of_currentIndexPose_29() { return static_cast<int32_t>(offsetof(SimulatedArticulatedHand_tE70788F371CF5A48A99B3DE695FFA7A0FEF6E2E9, ___currentIndexPose_29)); }
	inline MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  get_currentIndexPose_29() const { return ___currentIndexPose_29; }
	inline MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * get_address_of_currentIndexPose_29() { return &___currentIndexPose_29; }
	inline void set_currentIndexPose_29(MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  value)
	{
		___currentIndexPose_29 = value;
	}

	inline static int32_t get_offset_of_currentGripPose_30() { return static_cast<int32_t>(offsetof(SimulatedArticulatedHand_tE70788F371CF5A48A99B3DE695FFA7A0FEF6E2E9, ___currentGripPose_30)); }
	inline MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  get_currentGripPose_30() const { return ___currentGripPose_30; }
	inline MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * get_address_of_currentGripPose_30() { return &___currentGripPose_30; }
	inline void set_currentGripPose_30(MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  value)
	{
		___currentGripPose_30 = value;
	}

	inline static int32_t get_offset_of_lastGripPose_31() { return static_cast<int32_t>(offsetof(SimulatedArticulatedHand_tE70788F371CF5A48A99B3DE695FFA7A0FEF6E2E9, ___lastGripPose_31)); }
	inline MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  get_lastGripPose_31() const { return ___lastGripPose_31; }
	inline MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * get_address_of_lastGripPose_31() { return &___lastGripPose_31; }
	inline void set_lastGripPose_31(MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  value)
	{
		___lastGripPose_31 = value;
	}
};


// Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand
struct  SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB  : public SimulatedHand_tFBAB6AD39E9B16E093E63E4D2A88EA5E3415437E
{
public:
	// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::initializedFromProfile
	bool ___initializedFromProfile_25;
	// Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::holdAction
	MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  ___holdAction_26;
	// Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::navigationAction
	MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  ___navigationAction_27;
	// Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::manipulationAction
	MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  ___manipulationAction_28;
	// Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::selectAction
	MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  ___selectAction_29;
	// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::useRailsNavigation
	bool ___useRailsNavigation_30;
	// System.Single Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::holdStartDuration
	float ___holdStartDuration_31;
	// System.Single Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::navigationStartThreshold
	float ___navigationStartThreshold_32;
	// System.Single Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::SelectDownStartTime
	float ___SelectDownStartTime_33;
	// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::holdInProgress
	bool ___holdInProgress_34;
	// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::manipulationInProgress
	bool ___manipulationInProgress_35;
	// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::navigationInProgress
	bool ___navigationInProgress_36;
	// UnityEngine.Vector3 Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::currentRailsUsed
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___currentRailsUsed_37;
	// UnityEngine.Vector3 Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::currentPosition
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___currentPosition_38;
	// UnityEngine.Vector3 Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::cumulativeDelta
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___cumulativeDelta_39;
	// Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::currentGripPose
	MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  ___currentGripPose_40;

public:
	inline static int32_t get_offset_of_initializedFromProfile_25() { return static_cast<int32_t>(offsetof(SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB, ___initializedFromProfile_25)); }
	inline bool get_initializedFromProfile_25() const { return ___initializedFromProfile_25; }
	inline bool* get_address_of_initializedFromProfile_25() { return &___initializedFromProfile_25; }
	inline void set_initializedFromProfile_25(bool value)
	{
		___initializedFromProfile_25 = value;
	}

	inline static int32_t get_offset_of_holdAction_26() { return static_cast<int32_t>(offsetof(SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB, ___holdAction_26)); }
	inline MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  get_holdAction_26() const { return ___holdAction_26; }
	inline MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073 * get_address_of_holdAction_26() { return &___holdAction_26; }
	inline void set_holdAction_26(MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  value)
	{
		___holdAction_26 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___holdAction_26))->___description_2), (void*)NULL);
	}

	inline static int32_t get_offset_of_navigationAction_27() { return static_cast<int32_t>(offsetof(SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB, ___navigationAction_27)); }
	inline MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  get_navigationAction_27() const { return ___navigationAction_27; }
	inline MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073 * get_address_of_navigationAction_27() { return &___navigationAction_27; }
	inline void set_navigationAction_27(MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  value)
	{
		___navigationAction_27 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___navigationAction_27))->___description_2), (void*)NULL);
	}

	inline static int32_t get_offset_of_manipulationAction_28() { return static_cast<int32_t>(offsetof(SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB, ___manipulationAction_28)); }
	inline MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  get_manipulationAction_28() const { return ___manipulationAction_28; }
	inline MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073 * get_address_of_manipulationAction_28() { return &___manipulationAction_28; }
	inline void set_manipulationAction_28(MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  value)
	{
		___manipulationAction_28 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___manipulationAction_28))->___description_2), (void*)NULL);
	}

	inline static int32_t get_offset_of_selectAction_29() { return static_cast<int32_t>(offsetof(SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB, ___selectAction_29)); }
	inline MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  get_selectAction_29() const { return ___selectAction_29; }
	inline MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073 * get_address_of_selectAction_29() { return &___selectAction_29; }
	inline void set_selectAction_29(MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  value)
	{
		___selectAction_29 = value;
		Il2CppCodeGenWriteBarrier((void**)&(((&___selectAction_29))->___description_2), (void*)NULL);
	}

	inline static int32_t get_offset_of_useRailsNavigation_30() { return static_cast<int32_t>(offsetof(SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB, ___useRailsNavigation_30)); }
	inline bool get_useRailsNavigation_30() const { return ___useRailsNavigation_30; }
	inline bool* get_address_of_useRailsNavigation_30() { return &___useRailsNavigation_30; }
	inline void set_useRailsNavigation_30(bool value)
	{
		___useRailsNavigation_30 = value;
	}

	inline static int32_t get_offset_of_holdStartDuration_31() { return static_cast<int32_t>(offsetof(SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB, ___holdStartDuration_31)); }
	inline float get_holdStartDuration_31() const { return ___holdStartDuration_31; }
	inline float* get_address_of_holdStartDuration_31() { return &___holdStartDuration_31; }
	inline void set_holdStartDuration_31(float value)
	{
		___holdStartDuration_31 = value;
	}

	inline static int32_t get_offset_of_navigationStartThreshold_32() { return static_cast<int32_t>(offsetof(SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB, ___navigationStartThreshold_32)); }
	inline float get_navigationStartThreshold_32() const { return ___navigationStartThreshold_32; }
	inline float* get_address_of_navigationStartThreshold_32() { return &___navigationStartThreshold_32; }
	inline void set_navigationStartThreshold_32(float value)
	{
		___navigationStartThreshold_32 = value;
	}

	inline static int32_t get_offset_of_SelectDownStartTime_33() { return static_cast<int32_t>(offsetof(SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB, ___SelectDownStartTime_33)); }
	inline float get_SelectDownStartTime_33() const { return ___SelectDownStartTime_33; }
	inline float* get_address_of_SelectDownStartTime_33() { return &___SelectDownStartTime_33; }
	inline void set_SelectDownStartTime_33(float value)
	{
		___SelectDownStartTime_33 = value;
	}

	inline static int32_t get_offset_of_holdInProgress_34() { return static_cast<int32_t>(offsetof(SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB, ___holdInProgress_34)); }
	inline bool get_holdInProgress_34() const { return ___holdInProgress_34; }
	inline bool* get_address_of_holdInProgress_34() { return &___holdInProgress_34; }
	inline void set_holdInProgress_34(bool value)
	{
		___holdInProgress_34 = value;
	}

	inline static int32_t get_offset_of_manipulationInProgress_35() { return static_cast<int32_t>(offsetof(SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB, ___manipulationInProgress_35)); }
	inline bool get_manipulationInProgress_35() const { return ___manipulationInProgress_35; }
	inline bool* get_address_of_manipulationInProgress_35() { return &___manipulationInProgress_35; }
	inline void set_manipulationInProgress_35(bool value)
	{
		___manipulationInProgress_35 = value;
	}

	inline static int32_t get_offset_of_navigationInProgress_36() { return static_cast<int32_t>(offsetof(SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB, ___navigationInProgress_36)); }
	inline bool get_navigationInProgress_36() const { return ___navigationInProgress_36; }
	inline bool* get_address_of_navigationInProgress_36() { return &___navigationInProgress_36; }
	inline void set_navigationInProgress_36(bool value)
	{
		___navigationInProgress_36 = value;
	}

	inline static int32_t get_offset_of_currentRailsUsed_37() { return static_cast<int32_t>(offsetof(SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB, ___currentRailsUsed_37)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_currentRailsUsed_37() const { return ___currentRailsUsed_37; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_currentRailsUsed_37() { return &___currentRailsUsed_37; }
	inline void set_currentRailsUsed_37(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___currentRailsUsed_37 = value;
	}

	inline static int32_t get_offset_of_currentPosition_38() { return static_cast<int32_t>(offsetof(SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB, ___currentPosition_38)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_currentPosition_38() const { return ___currentPosition_38; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_currentPosition_38() { return &___currentPosition_38; }
	inline void set_currentPosition_38(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___currentPosition_38 = value;
	}

	inline static int32_t get_offset_of_cumulativeDelta_39() { return static_cast<int32_t>(offsetof(SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB, ___cumulativeDelta_39)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_cumulativeDelta_39() const { return ___cumulativeDelta_39; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_cumulativeDelta_39() { return &___cumulativeDelta_39; }
	inline void set_cumulativeDelta_39(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___cumulativeDelta_39 = value;
	}

	inline static int32_t get_offset_of_currentGripPose_40() { return static_cast<int32_t>(offsetof(SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB, ___currentGripPose_40)); }
	inline MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  get_currentGripPose_40() const { return ___currentGripPose_40; }
	inline MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * get_address_of_currentGripPose_40() { return &___currentGripPose_40; }
	inline void set_currentGripPose_40(MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  value)
	{
		___currentGripPose_40 = value;
	}
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
// UnityEngine.KeyCode[]
struct KeyCodeU5BU5D_tF4382F22534318B6E15A70B33AAF395B3D8D127F  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) int32_t m_Items[1];

public:
	inline int32_t GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline int32_t* GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, int32_t value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline int32_t GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline int32_t* GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, int32_t value)
	{
		m_Items[index] = value;
	}
};
// Microsoft.MixedReality.Toolkit.Input.KeyBinding_MouseButton[]
struct MouseButtonU5BU5D_t6CE0267665AAD6A7B40F7782DA60DD3810558E82  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) int32_t m_Items[1];

public:
	inline int32_t GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline int32_t* GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, int32_t value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline int32_t GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline int32_t* GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, int32_t value)
	{
		m_Items[index] = value;
	}
};
// System.String[]
struct StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) String_t* m_Items[1];

public:
	inline String_t* GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline String_t** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, String_t* value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline String_t* GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline String_t** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, String_t* value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};
// Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping[]
struct MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * m_Items[1];

public:
	inline MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};
// Microsoft.MixedReality.Toolkit.Input.MixedRealityGestureMapping[]
struct MixedRealityGestureMappingU5BU5D_t2F3D7B685E29F06002C6BD2EF99A97C8DF6BD874  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074  m_Items[1];

public:
	inline MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074  GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074 * GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074  value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)&((m_Items + index)->___description_0), (void*)NULL);
		#if IL2CPP_ENABLE_STRICT_WRITE_BARRIERS
		Il2CppCodeGenWriteBarrier((void**)&((&((m_Items + index)->___action_2))->___description_2), (void*)NULL);
		#endif
	}
	inline MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074  GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074 * GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074  value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)&((m_Items + index)->___description_0), (void*)NULL);
		#if IL2CPP_ENABLE_STRICT_WRITE_BARRIERS
		Il2CppCodeGenWriteBarrier((void**)&((&((m_Items + index)->___action_2))->___description_2), (void*)NULL);
		#endif
	}
};
// Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose[]
struct MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  m_Items[1];

public:
	inline MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  value)
	{
		m_Items[index] = value;
	}
};
// System.Delegate[]
struct DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) Delegate_t * m_Items[1];

public:
	inline Delegate_t * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline Delegate_t ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, Delegate_t * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline Delegate_t * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline Delegate_t ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, Delegate_t * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};
// UnityEngine.Vector3[]
struct Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  m_Items[1];

public:
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		m_Items[index] = value;
	}
};
// UnityEngine.Quaternion[]
struct QuaternionU5BU5D_t26EB10EEE89DD3EF913D52E8797FAB841F6F2AA3  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  m_Items[1];

public:
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 * GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 * GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  value)
	{
		m_Items[index] = value;
	}
};
// System.Int32[]
struct Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) int32_t m_Items[1];

public:
	inline int32_t GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline int32_t* GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, int32_t value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline int32_t GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline int32_t* GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, int32_t value)
	{
		m_Items[index] = value;
	}
};


// System.Void System.Collections.Generic.Dictionary`2<System.Object,System.Int32>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Dictionary_2__ctor_m56FBD260A4D190AD833E9B108B1E80A574AA62C4_gshared (Dictionary_2_t81923CE2A312318AE13F58085CCF7FA8D879B77A * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<System.Int32,System.Object>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Dictionary_2__ctor_m7D745ADE56151C2895459668F4A4242985E526D8_gshared (Dictionary_2_t03608389BB57475AA3F4B2B79D176A27807BC884 * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.List`1<System.Object>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void List_1__ctor_mC832F1AC0F814BAEB19175F5D7972A7507508BC3_gshared (List_1_t05CC3C859AB5E6024394EF9A42E3E696628CA02D * __this, const RuntimeMethod* method);
// System.Void System.Action`2<System.Int32Enum,System.Int32>::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Action_2__ctor_m95993AFCBE79972A2D915BBD691A9C7268107BA7_gshared (Action_2_t211F8AF4611284BBE3D0590121A0E0BF9FA7E614 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method);
// System.Void System.Action`2<System.Int32Enum,System.Int32>::Invoke(!0,!1)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Action_2_Invoke_m0D4C0925C5B10E919BC61BA0898B5ED5D188F8F2_gshared (Action_2_t211F8AF4611284BBE3D0590121A0E0BF9FA7E614 * __this, int32_t ___arg10, int32_t ___arg21, const RuntimeMethod* method);
// !0[] System.Collections.Generic.List`1<System.Object>::ToArray()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* List_1_ToArray_m801D4DEF3587F60F463F04EEABE5CBE711FE5612_gshared (List_1_t05CC3C859AB5E6024394EF9A42E3E696628CA02D * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.List`1<System.Object>::Add(!0)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void List_1_Add_m6930161974C7504C80F52EC379EF012387D43138_gshared (List_1_t05CC3C859AB5E6024394EF9A42E3E696628CA02D * __this, RuntimeObject * ___item0, const RuntimeMethod* method);
// System.Tuple`2<!!0,!!1> System.Tuple::Create<System.Int32Enum,System.Int32>(!!0,!!1)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Tuple_2_t6013D918BF7AB88AC1206529AAB17213208F33F0 * Tuple_Create_TisInt32Enum_t6312CE4586C17FE2E2E513D2E7655B574F10FDCD_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m4217E2530462EE46C5DCEF9F21AED4F307000848_gshared (int32_t ___item10, int32_t ___item21, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<System.Int32,System.Object>::set_Item(!0,!1)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Dictionary_2_set_Item_mF9A6FBE4006C89D15B8C88B2CB46E9B24D18B7FC_gshared (Dictionary_2_t03608389BB57475AA3F4B2B79D176A27807BC884 * __this, int32_t ___key0, RuntimeObject * ___value1, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<System.Object,System.Int32>::set_Item(!0,!1)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Dictionary_2_set_Item_mC87D8EECD8406043786CC95870458389CEF82CDF_gshared (Dictionary_2_t81923CE2A312318AE13F58085CCF7FA8D879B77A * __this, RuntimeObject * ___key0, int32_t ___value1, const RuntimeMethod* method);
// !1 System.Collections.Generic.Dictionary`2<System.Int32Enum,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose>::get_Item(!0)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  Dictionary_2_get_Item_mE6B9D39124056519428A572665E726815D5600EF_gshared (Dictionary_2_t4594E9EA67CB7172740DF4116774A3B1432A9E97 * __this, int32_t ___key0, const RuntimeMethod* method);
// System.Void System.Nullable`1<System.Int32>::.ctor(!0)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Nullable_1__ctor_m11F9C228CFDF836DDFCD7880C09CB4098AB9D7F2_gshared (Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB * __this, int32_t ___value0, const RuntimeMethod* method);
// !0 System.Nullable`1<System.Int32>::GetValueOrDefault()
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR int32_t Nullable_1_GetValueOrDefault_mE89BB8F302DF31EE202251F4746859285860B6B6_gshared_inline (Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB * __this, const RuntimeMethod* method);
// System.Boolean System.Nullable`1<System.Int32>::get_HasValue()
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR bool Nullable_1_get_HasValue_mB664E2C41CADA8413EF8842E6601B8C696A7CE15_gshared_inline (Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<System.Int32Enum,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Dictionary_2__ctor_m6F2ED586C8EC85B459FFCA36D05ABF98C1AA33B3_gshared (Dictionary_2_t4594E9EA67CB7172740DF4116774A3B1432A9E97 * __this, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.Dictionary`2<System.Int32Enum,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose>::TryGetValue(!0,!1&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Dictionary_2_TryGetValue_m3816E3065E00AF57E62424BB45AEA6000BD27F49_gshared (Dictionary_2_t4594E9EA67CB7172740DF4116774A3B1432A9E97 * __this, int32_t ___key0, MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * ___value1, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.Dictionary`2<System.Int32Enum,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose>::ContainsKey(!0)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Dictionary_2_ContainsKey_m83D33BC652DBE4549C5B2C4A1E51BDA96E1989C6_gshared (Dictionary_2_t4594E9EA67CB7172740DF4116774A3B1432A9E97 * __this, int32_t ___key0, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<System.Int32Enum,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose>::Add(!0,!1)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Dictionary_2_Add_mBB39FF6AADDEF60E949DF52642B7BA33E9CC5406_gshared (Dictionary_2_t4594E9EA67CB7172740DF4116774A3B1432A9E97 * __this, int32_t ___key0, MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  ___value1, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<System.Int32Enum,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose>::set_Item(!0,!1)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Dictionary_2_set_Item_m72BAB8E16164B0649C7EFF83BD5C1904748DC7F0_gshared (Dictionary_2_t4594E9EA67CB7172740DF4116774A3B1432A9E97 * __this, int32_t ___key0, MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  ___value1, const RuntimeMethod* method);
// System.Boolean Microsoft.MixedReality.Toolkit.MixedRealityServiceRegistry::TryGetService<System.Object>(!!0&,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool MixedRealityServiceRegistry_TryGetService_TisRuntimeObject_m2354211184CA13FEA1094444215C1DE746B56354_gshared (RuntimeObject ** ___serviceInstance0, String_t* ___name1, const RuntimeMethod* method);
// System.Collections.Generic.IEnumerable`1<!!0> System.Linq.Enumerable::Take<System.Int32>(System.Collections.Generic.IEnumerable`1<!!0>,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* Enumerable_Take_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_mCBED6C7F74DCC17FA9C923D11B6801F52FEEB61B_gshared (RuntimeObject* ___source0, int32_t ___count1, const RuntimeMethod* method);

// System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Type_t * Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6 (RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  ___handle0, const RuntimeMethod* method);
// System.Array System.Enum::GetValues(System.Type)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeArray * Enum_GetValues_m20F5C0B826344A499B1C23BB7A3B532017F0F30C (Type_t * ___enumType0, const RuntimeMethod* method);
// System.Void Microsoft.MixedReality.Toolkit.Input.KeyBinding/<>c__DisplayClass5_0::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass5_0__ctor_m765F71F1687CD4EA6EF78246D50807FA94326711 (U3CU3Ec__DisplayClass5_0_t5532E81B72C939F27BA424481612158E32B0C681 * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<System.Tuple`2<Microsoft.MixedReality.Toolkit.Input.KeyBinding/KeyType,System.Int32>,System.Int32>::.ctor()
inline void Dictionary_2__ctor_m747FD3B997983E98D0914810BA2B843ED90D554B (Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB * __this, const RuntimeMethod* method)
{
	((  void (*) (Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB *, const RuntimeMethod*))Dictionary_2__ctor_m56FBD260A4D190AD833E9B108B1E80A574AA62C4_gshared)(__this, method);
}
// System.Void System.Collections.Generic.Dictionary`2<System.Int32,System.Tuple`2<Microsoft.MixedReality.Toolkit.Input.KeyBinding/KeyType,System.Int32>>::.ctor()
inline void Dictionary_2__ctor_m2298C894CE2941227F176A13E8FF938BD954E63B (Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64 * __this, const RuntimeMethod* method)
{
	((  void (*) (Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64 *, const RuntimeMethod*))Dictionary_2__ctor_m7D745ADE56151C2895459668F4A4242985E526D8_gshared)(__this, method);
}
// System.Void System.Collections.Generic.List`1<System.String>::.ctor()
inline void List_1__ctor_mDA22758D73530683C950C5CCF39BDB4E7E1F3F06 (List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3 * __this, const RuntimeMethod* method)
{
	((  void (*) (List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3 *, const RuntimeMethod*))List_1__ctor_mC832F1AC0F814BAEB19175F5D7972A7507508BC3_gshared)(__this, method);
}
// System.Void System.Action`2<Microsoft.MixedReality.Toolkit.Input.KeyBinding/KeyType,System.Int32>::.ctor(System.Object,System.IntPtr)
inline void Action_2__ctor_mF9F632823062B05D3DA92A0649DC4EE862AE1C7A (Action_2_t599C81CC1C0CDFE287E5D39D3EEB3130080399E8 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	((  void (*) (Action_2_t599C81CC1C0CDFE287E5D39D3EEB3130080399E8 *, RuntimeObject *, intptr_t, const RuntimeMethod*))Action_2__ctor_m95993AFCBE79972A2D915BBD691A9C7268107BA7_gshared)(__this, ___object0, ___method1, method);
}
// System.Void System.Action`2<Microsoft.MixedReality.Toolkit.Input.KeyBinding/KeyType,System.Int32>::Invoke(!0,!1)
inline void Action_2_Invoke_m0D15E6E36BD572A4DF315B9F04F30A0F0EFE31E5 (Action_2_t599C81CC1C0CDFE287E5D39D3EEB3130080399E8 * __this, int32_t ___arg10, int32_t ___arg21, const RuntimeMethod* method)
{
	((  void (*) (Action_2_t599C81CC1C0CDFE287E5D39D3EEB3130080399E8 *, int32_t, int32_t, const RuntimeMethod*))Action_2_Invoke_m0D4C0925C5B10E919BC61BA0898B5ED5D188F8F2_gshared)(__this, ___arg10, ___arg21, method);
}
// !0[] System.Collections.Generic.List`1<System.String>::ToArray()
inline StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* List_1_ToArray_m9DD19D800AE6D84ED0729D5D97CAF84DF317DD38 (List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3 * __this, const RuntimeMethod* method)
{
	return ((  StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* (*) (List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3 *, const RuntimeMethod*))List_1_ToArray_m801D4DEF3587F60F463F04EEABE5CBE711FE5612_gshared)(__this, method);
}
// Microsoft.MixedReality.Toolkit.Input.KeyBinding/KeyType Microsoft.MixedReality.Toolkit.Input.KeyBinding::get_BindingType()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t KeyBinding_get_BindingType_mA6915A48809778FE77561961A250F3D5BEABFE91_inline (KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 * __this, const RuntimeMethod* method);
// System.String System.String::Concat(System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Concat_mB78D0094592718DA6D5DB6C712A9C225631666BE (String_t* ___str00, String_t* ___str11, const RuntimeMethod* method);
// System.String System.String::Concat(System.String,System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Concat_mF4626905368D6558695A823466A1AF65EADB9923 (String_t* ___str00, String_t* ___str11, String_t* ___str22, const RuntimeMethod* method);
// System.String Microsoft.MixedReality.Toolkit.Input.KeyBinding::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* KeyBinding_ToString_mB8F2F02D75495579EEDDB8B27851E0BFC044B526 (KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 * __this, const RuntimeMethod* method);
// System.Boolean Microsoft.MixedReality.Toolkit.Input.KeyBinding::TryGetKeyCode(UnityEngine.KeyCode&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool KeyBinding_TryGetKeyCode_m185188BD7AFC2303E3DE3BB2161E6280DB676382 (KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 * __this, int32_t* ___keyCode0, const RuntimeMethod* method);
// System.Boolean Microsoft.MixedReality.Toolkit.Input.KeyBinding::TryGetMouseButton(System.Int32&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool KeyBinding_TryGetMouseButton_m398435CC5A7F9427B8C7932A8714E496ED650DEC (KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 * __this, int32_t* ___mouseButton0, const RuntimeMethod* method);
// System.Boolean Microsoft.MixedReality.Toolkit.Input.KeyBinding::TryGetMouseButton(Microsoft.MixedReality.Toolkit.Input.KeyBinding/MouseButton&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool KeyBinding_TryGetMouseButton_mC9D3F31ECF45FC2649872D0FE531235E0C46F6A2 (KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 * __this, int32_t* ___mouseButton0, const RuntimeMethod* method);
// Microsoft.MixedReality.Toolkit.Input.KeyBinding Microsoft.MixedReality.Toolkit.Input.KeyBinding::FromMouseButton(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  KeyBinding_FromMouseButton_m9C1C18324382689D26647131D9C8CD2D71B71CF2 (int32_t ___mouseButton0, const RuntimeMethod* method);
// System.Void System.Object::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0 (RuntimeObject * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.List`1<System.String>::Add(!0)
inline void List_1_Add_mA348FA1140766465189459D25B01EB179001DE83 (List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3 * __this, String_t* ___item0, const RuntimeMethod* method)
{
	((  void (*) (List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3 *, String_t*, const RuntimeMethod*))List_1_Add_m6930161974C7504C80F52EC379EF012387D43138_gshared)(__this, ___item0, method);
}
// System.Tuple`2<!!0,!!1> System.Tuple::Create<Microsoft.MixedReality.Toolkit.Input.KeyBinding/KeyType,System.Int32>(!!0,!!1)
inline Tuple_2_tFF0D9FEC0FEA81089BD6B1384583703BD0A104EE * Tuple_Create_TisKeyType_t63A0EC9B1C9653881B95DF409080C7FB24760D72_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_mA5D31171EBE5513EC23DF8E079EC60FE1EE2E658 (int32_t ___item10, int32_t ___item21, const RuntimeMethod* method)
{
	return ((  Tuple_2_tFF0D9FEC0FEA81089BD6B1384583703BD0A104EE * (*) (int32_t, int32_t, const RuntimeMethod*))Tuple_Create_TisInt32Enum_t6312CE4586C17FE2E2E513D2E7655B574F10FDCD_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m4217E2530462EE46C5DCEF9F21AED4F307000848_gshared)(___item10, ___item21, method);
}
// System.Void System.Collections.Generic.Dictionary`2<System.Int32,System.Tuple`2<Microsoft.MixedReality.Toolkit.Input.KeyBinding/KeyType,System.Int32>>::set_Item(!0,!1)
inline void Dictionary_2_set_Item_m3D5CB4BFE05FDFFBEFF66F28C80B6AF3A94ECBF5 (Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64 * __this, int32_t ___key0, Tuple_2_tFF0D9FEC0FEA81089BD6B1384583703BD0A104EE * ___value1, const RuntimeMethod* method)
{
	((  void (*) (Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64 *, int32_t, Tuple_2_tFF0D9FEC0FEA81089BD6B1384583703BD0A104EE *, const RuntimeMethod*))Dictionary_2_set_Item_mF9A6FBE4006C89D15B8C88B2CB46E9B24D18B7FC_gshared)(__this, ___key0, ___value1, method);
}
// System.Void System.Collections.Generic.Dictionary`2<System.Tuple`2<Microsoft.MixedReality.Toolkit.Input.KeyBinding/KeyType,System.Int32>,System.Int32>::set_Item(!0,!1)
inline void Dictionary_2_set_Item_m71327547831A3689A4215232C29A1EBA103BE6DE (Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB * __this, Tuple_2_tFF0D9FEC0FEA81089BD6B1384583703BD0A104EE * ___key0, int32_t ___value1, const RuntimeMethod* method)
{
	((  void (*) (Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB *, Tuple_2_tFF0D9FEC0FEA81089BD6B1384583703BD0A104EE *, int32_t, const RuntimeMethod*))Dictionary_2_set_Item_mC87D8EECD8406043786CC95870458389CEF82CDF_gshared)(__this, ___key0, ___value1, method);
}
// System.Boolean UnityEngine.Input::GetMouseButton(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Input_GetMouseButton_m43C68DE93C7D990E875BA53C4DEC9CA6230C8B79 (int32_t ___button0, const RuntimeMethod* method);
// System.Boolean UnityEngine.Input::GetKey(UnityEngine.KeyCode)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Input_GetKey_m46AA83E14F9C3A75E06FE0A8C55740D47B2DB784 (int32_t ___key0, const RuntimeMethod* method);
// System.Boolean UnityEngine.Input::GetMouseButtonDown(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Input_GetMouseButtonDown_m5AD76E22AA839706219AD86A4E0BE5276AF8E28A (int32_t ___button0, const RuntimeMethod* method);
// System.Boolean UnityEngine.Input::GetKeyDown(UnityEngine.KeyCode)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Input_GetKeyDown_mEA57896808B6F484B12CD0AEEB83390A3CFCDBDC (int32_t ___key0, const RuntimeMethod* method);
// System.Boolean UnityEngine.Input::GetMouseButtonUp(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Input_GetMouseButtonUp_m4899272EB31D43EC4A3A1A115843CD3D9AA2C4EC (int32_t ___button0, const RuntimeMethod* method);
// System.Boolean UnityEngine.Input::GetKeyUp(UnityEngine.KeyCode)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Input_GetKeyUp_m5345ECFA25B7AC99D6D4223DA23BB9FB991B7193 (int32_t ___key0, const RuntimeMethod* method);
// Microsoft.MixedReality.Toolkit.Input.KeyBinding Microsoft.MixedReality.Toolkit.Input.KeyBinding::FromMouseButton(Microsoft.MixedReality.Toolkit.Input.KeyBinding/MouseButton)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  KeyBinding_FromMouseButton_mC7479108FCC71C952AAB38A9526E2B82B71C8CD0 (int32_t ___mouseButton0, const RuntimeMethod* method);
// Microsoft.MixedReality.Toolkit.Input.KeyBinding Microsoft.MixedReality.Toolkit.Input.KeyBinding::FromKey(UnityEngine.KeyCode)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  KeyBinding_FromKey_m4E6BB297D9741E6C9C1FD8CB946CB140C4FD1DE5 (int32_t ___keyCode0, const RuntimeMethod* method);
// System.Void Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BaseMixedRealityProfile__ctor_mC73E9360DB114F72FBC08703A0A9ABA78168B78A (BaseMixedRealityProfile_tB4DC16619B37D298D22571CE017070A78EF826E8 * __this, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Vector3::get_zero()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Vector3_get_zero_m3CDDCAE94581DF3BB16C4B40A100E28E9C6649C2 (const RuntimeMethod* method);
// UnityEngine.Quaternion UnityEngine.Quaternion::get_identity()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  Quaternion_get_identity_m548B37D80F2DEE60E41D1F09BF6889B557BE1A64 (const RuntimeMethod* method);
// Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose::get_ZeroIdentity()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  MixedRealityPose_get_ZeroIdentity_m80C016329EAADDC4EB8DFD80ED0CF614A5E547AD_inline (const RuntimeMethod* method);
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedHand::.ctor(Microsoft.MixedReality.Toolkit.TrackingState,Microsoft.MixedReality.Toolkit.Utilities.Handedness,Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource,Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedHand__ctor_m93808D1348F3FB6FA63A335E89F47FB5345EE1C4 (SimulatedHand_tFBAB6AD39E9B16E093E63E4D2A88EA5E3415437E * __this, int32_t ___trackingState0, uint8_t ___controllerHandedness1, RuntimeObject* ___inputSource2, MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* ___interactions3, const RuntimeMethod* method);
// Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction::get_None()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  MixedRealityInputAction_get_None_m0276CF8988B0670DCCE381865DD5190010A2A8BF_inline (const RuntimeMethod* method);
// System.Void Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::.ctor(System.UInt32,System.String,Microsoft.MixedReality.Toolkit.Utilities.AxisType,Microsoft.MixedReality.Toolkit.Input.DeviceInputType,Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction,UnityEngine.KeyCode,System.String,System.String,System.Boolean,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void MixedRealityInteractionMapping__ctor_m42FA7B2EF2BAA3804530651DFDF1145EEECE437F (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * __this, uint32_t ___id0, String_t* ___description1, int32_t ___axisType2, int32_t ___inputType3, MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  ___inputAction4, int32_t ___keyCode5, String_t* ___axisCodeX6, String_t* ___axisCodeY7, bool ___invertXAxis8, bool ___invertYAxis9, const RuntimeMethod* method);
// System.Void Microsoft.MixedReality.Toolkit.Input.BaseController::AssignControllerMappings(Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BaseController_AssignControllerMappings_mB58538C7085760171304343CFBD77E5D8F230054 (BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B * __this, MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* ___mappings0, const RuntimeMethod* method);
// !1 System.Collections.Generic.Dictionary`2<Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose>::get_Item(!0)
inline MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  Dictionary_2_get_Item_mAA87FA69922BAF6733C05E34A765031668FCABA6 (Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 * __this, int32_t ___key0, const RuntimeMethod* method)
{
	return ((  MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  (*) (Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 *, int32_t, const RuntimeMethod*))Dictionary_2_get_Item_mE6B9D39124056519428A572665E726815D5600EF_gshared)(__this, ___key0, method);
}
// UnityEngine.Vector3 Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose::get_Position()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  MixedRealityPose_get_Position_mF175BAE3270E5432E605BDD5FD1FA5F722B24AEE_inline (MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Vector3::op_Inequality(UnityEngine.Vector3,UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Vector3_op_Inequality_mFEEAA4C4BF743FB5B8A47FF4967A5E2C73273D6E (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___lhs0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___rhs1, const RuntimeMethod* method);
// System.Void Microsoft.MixedReality.Toolkit.Input.BaseController::set_IsRotationAvailable(System.Boolean)
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR void BaseController_set_IsRotationAvailable_m5259A799822AFD94A2BEE4B47F887A03158FE308_inline (BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B * __this, bool ___value0, const RuntimeMethod* method);
// System.Void Microsoft.MixedReality.Toolkit.Input.BaseController::set_IsPositionAvailable(System.Boolean)
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR void BaseController_set_IsPositionAvailable_m76D7FB5DBF945174A9D9B7A19123783742C6B57F_inline (BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B * __this, bool ___value0, const RuntimeMethod* method);
// System.Boolean Microsoft.MixedReality.Toolkit.Input.BaseController::get_IsPositionAvailable()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR bool BaseController_get_IsPositionAvailable_m3E2EB0D15AAADABB3D967535353AD53539677046_inline (BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B * __this, const RuntimeMethod* method);
// Microsoft.MixedReality.Toolkit.Input.HandRay Microsoft.MixedReality.Toolkit.Input.BaseHand::get_HandRay()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR HandRay_t9DAE3FE243DBED1BAA1B9A4F782C3F1C9E6AE285 * BaseHand_get_HandRay_mDB7145BE29023110AF5EC4037ABE75660776680F_inline (BaseHand_tB58ECFC99FBFD516BBAA0989004A10F687078F4B * __this, const RuntimeMethod* method);
// UnityEngine.Vector3 Microsoft.MixedReality.Toolkit.Input.BaseHand::GetPalmNormal()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  BaseHand_GetPalmNormal_mB5FF6D007531A6DD4C3E7632AF60DD2C586AA76B (BaseHand_tB58ECFC99FBFD516BBAA0989004A10F687078F4B * __this, const RuntimeMethod* method);
// UnityEngine.Camera Microsoft.MixedReality.Toolkit.Utilities.CameraCache::get_Main()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Camera_t48B2B9ECB3CE6108A98BF949A1CECF0FE3421F34 * CameraCache_get_Main_m23FB3162F6476988FEE59F829DEAF08702D81554 (const RuntimeMethod* method);
// UnityEngine.Transform UnityEngine.Component::get_transform()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9 (Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621 * __this, const RuntimeMethod* method);
// Microsoft.MixedReality.Toolkit.Utilities.Handedness Microsoft.MixedReality.Toolkit.Input.BaseController::get_ControllerHandedness()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR uint8_t BaseController_get_ControllerHandedness_mA18814111E1328E1C7C04C383CC44E8A2F8A995A_inline (BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B * __this, const RuntimeMethod* method);
// System.Void Microsoft.MixedReality.Toolkit.Input.HandRay::Update(UnityEngine.Vector3,UnityEngine.Vector3,UnityEngine.Transform,Microsoft.MixedReality.Toolkit.Utilities.Handedness)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void HandRay_Update_m2C7628B2A0B6F1EE9C20DE0E38CDD4854F70F149 (HandRay_t9DAE3FE243DBED1BAA1B9A4F782C3F1C9E6AE285 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___handPosition0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___palmNormal1, Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * ___headTransform2, uint8_t ___sourceHandedness3, const RuntimeMethod* method);
// UnityEngine.Ray Microsoft.MixedReality.Toolkit.Input.HandRay::get_Ray()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Ray_tE2163D4CB3E6B267E29F8ABE41684490E4A614B2  HandRay_get_Ray_mA5DDBC5EF46D813F75A3728882AE72F8A779C189 (HandRay_t9DAE3FE243DBED1BAA1B9A4F782C3F1C9E6AE285 * __this, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Ray::get_origin()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Ray_get_origin_m3773CA7B1E2F26F6F1447652B485D86C0BEC5187 (Ray_tE2163D4CB3E6B267E29F8ABE41684490E4A614B2 * __this, const RuntimeMethod* method);
// System.Void Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose::set_Position(UnityEngine.Vector3)
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR void MixedRealityPose_set_Position_m28EBD523337BC95684EFC016980F3862DE763759_inline (MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___value0, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Ray::get_direction()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Ray_get_direction_m9E6468CD87844B437FC4B93491E63D388322F76E (Ray_tE2163D4CB3E6B267E29F8ABE41684490E4A614B2 * __this, const RuntimeMethod* method);
// UnityEngine.Quaternion UnityEngine.Quaternion::LookRotation(UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  Quaternion_LookRotation_m465C08262650385D02ADDE78C9791AED47D2155F (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___forward0, const RuntimeMethod* method);
// System.Void Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose::set_Rotation(UnityEngine.Quaternion)
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR void MixedRealityPose_set_Rotation_m1AC620BE37B8F415170D725902EE1C3A92ECC19B_inline (MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * __this, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___value0, const RuntimeMethod* method);
// System.Boolean Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose::op_Inequality(Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool MixedRealityPose_op_Inequality_m85FF483B646A63C06AE543020D4F85257046AB3D (MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  ___left0, MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  ___right1, const RuntimeMethod* method);
// System.Boolean Microsoft.MixedReality.Toolkit.Input.BaseController::get_IsRotationAvailable()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR bool BaseController_get_IsRotationAvailable_m59D5E1DD267C83A3DB834096028590522C934868_inline (BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B * __this, const RuntimeMethod* method);
// Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem Microsoft.MixedReality.Toolkit.Input.BaseController::get_InputSystem()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365 (BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B * __this, const RuntimeMethod* method);
// Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource Microsoft.MixedReality.Toolkit.Input.BaseController::get_InputSource()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR RuntimeObject* BaseController_get_InputSource_m9F9D70F24AC4D5605665D31F6D8A6083A3CA1CFD_inline (BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B * __this, const RuntimeMethod* method);
// Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping[] Microsoft.MixedReality.Toolkit.Input.BaseController::get_Interactions()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline (BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B * __this, const RuntimeMethod* method);
// Microsoft.MixedReality.Toolkit.Input.DeviceInputType Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::get_InputType()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t MixedRealityInteractionMapping_get_InputType_mA8C027545479C380F87D72BDED734A9BDBFA40CD_inline (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * __this, const RuntimeMethod* method);
// System.Void Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::set_PoseData(Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void MixedRealityInteractionMapping_set_PoseData_mED53A7137722CE84DD3F8144D83C6E2F6B844287 (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * __this, MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  ___value0, const RuntimeMethod* method);
// System.Boolean Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::get_Changed()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool MixedRealityInteractionMapping_get_Changed_m70D15D24BDB909A6AA0E9C4DB393DAA25F84983F (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * __this, const RuntimeMethod* method);
// Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::get_MixedRealityInputAction()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  MixedRealityInteractionMapping_get_MixedRealityInputAction_mA22FF2AC6237AEF7B9EADF4461EB3B484CCB995E_inline (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * __this, const RuntimeMethod* method);
// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedHandData::get_IsPinching()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR bool SimulatedHandData_get_IsPinching_mB7C40888399E88C93E755FE89D50234CF5F5C981_inline (SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * __this, const RuntimeMethod* method);
// System.Void Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::set_BoolData(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void MixedRealityInteractionMapping_set_BoolData_mE86E7E665BCA02A2E69651A333993A51703F7D64 (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * __this, bool ___value0, const RuntimeMethod* method);
// System.Boolean Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping::get_BoolData()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR bool MixedRealityInteractionMapping_get_BoolData_mB42A4C428B73C25DC7FE9CAC463325E19255F71B_inline (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * __this, const RuntimeMethod* method);
// System.Void System.Nullable`1<System.Int32>::.ctor(!0)
inline void Nullable_1__ctor_m11F9C228CFDF836DDFCD7880C09CB4098AB9D7F2 (Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB * __this, int32_t ___value0, const RuntimeMethod* method)
{
	((  void (*) (Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB *, int32_t, const RuntimeMethod*))Nullable_1__ctor_m11F9C228CFDF836DDFCD7880C09CB4098AB9D7F2_gshared)(__this, ___value0, method);
}
// !0 System.Nullable`1<System.Int32>::GetValueOrDefault()
inline int32_t Nullable_1_GetValueOrDefault_mE89BB8F302DF31EE202251F4746859285860B6B6_inline (Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB * __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB *, const RuntimeMethod*))Nullable_1_GetValueOrDefault_mE89BB8F302DF31EE202251F4746859285860B6B6_gshared_inline)(__this, method);
}
// System.Boolean System.Nullable`1<System.Int32>::get_HasValue()
inline bool Nullable_1_get_HasValue_mB664E2C41CADA8413EF8842E6601B8C696A7CE15_inline (Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB * __this, const RuntimeMethod* method)
{
	return ((  bool (*) (Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB *, const RuntimeMethod*))Nullable_1_get_HasValue_mB664E2C41CADA8413EF8842E6601B8C696A7CE15_gshared_inline)(__this, method);
}
// System.Single UnityEngine.Mathf::Clamp(System.Single,System.Single,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float Mathf_Clamp_m033DD894F89E6DCCDAFC580091053059C86A4507 (float ___value0, float ___min1, float ___max2, const RuntimeMethod* method);
// System.Void UnityEngine.Vector3::.ctor(System.Single,System.Single,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Vector3__ctor_m08F61F548AA5836D8789843ACB4A81E4963D2EE1 (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * __this, float ___x0, float ___y1, float ___z2, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Vector3::get_one()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Vector3_get_one_mA11B83037CB269C6076CBCF754E24C8F3ACEC2AB (const RuntimeMethod* method);
// Microsoft.MixedReality.Toolkit.Input.MixedRealityGesturesProfile Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSystemProfile::get_GesturesProfile()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725 * MixedRealityInputSystemProfile_get_GesturesProfile_mA8F275BA8A5AE96D3A95350F698A7343D72E5129_inline (MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2 * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Object::op_Inequality(UnityEngine.Object,UnityEngine.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1 (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * ___x0, Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * ___y1, const RuntimeMethod* method);
// Microsoft.MixedReality.Toolkit.Input.MixedRealityGestureMapping[] Microsoft.MixedReality.Toolkit.Input.MixedRealityGesturesProfile::get_Gestures()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR MixedRealityGestureMappingU5BU5D_t2F3D7B685E29F06002C6BD2EF99A97C8DF6BD874* MixedRealityGesturesProfile_get_Gestures_mBAB7F3737E09478B3FA7F30ECAC24D6840E98580_inline (MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725 * __this, const RuntimeMethod* method);
// Microsoft.MixedReality.Toolkit.Input.GestureInputType Microsoft.MixedReality.Toolkit.Input.MixedRealityGestureMapping::get_GestureType()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t MixedRealityGestureMapping_get_GestureType_m6798792581776B818AF6A5307DD72D3425420C20_inline (MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074 * __this, const RuntimeMethod* method);
// Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction Microsoft.MixedReality.Toolkit.Input.MixedRealityGestureMapping::get_Action()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  MixedRealityGestureMapping_get_Action_mF225EE997BA38AFC7DCCA99F71434633FD683D82_inline (MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074 * __this, const RuntimeMethod* method);
// System.Boolean Microsoft.MixedReality.Toolkit.Input.MixedRealityGesturesProfile::get_UseRailsNavigation()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR bool MixedRealityGesturesProfile_get_UseRailsNavigation_mEAE6D30B9C69C0E5EA8115068FDA600F87CE02C6_inline (MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725 * __this, const RuntimeMethod* method);
// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_HoldStartDuration()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR float MixedRealityInputSimulationProfile_get_HoldStartDuration_mBC1A3E5C22D4854356392379561E246374610007_inline (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method);
// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_NavigationStartThreshold()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR float MixedRealityInputSimulationProfile_get_NavigationStartThreshold_m30BD08DA409E73AE42567F6420EB5E92DC7981E4_inline (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method);
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::EnsureProfileSettings()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedGestureHand_EnsureProfileSettings_m5FC39BD038B64363C40173D9E60B1BC1606C7A3A (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Vector3::op_Subtraction(UnityEngine.Vector3,UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Vector3_op_Subtraction_mF9846B723A5034F8B9F5F5DCB78E3D67649143D3 (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___a0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___b1, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Vector3::op_Addition(UnityEngine.Vector3,UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Vector3_op_Addition_m929F9C17E5D11B94D50B4AFF1D730B70CB59B50E (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___a0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___b1, const RuntimeMethod* method);
// System.Single UnityEngine.Time::get_time()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float Time_get_time_m7863349C8845BBA36629A2B3F8EF1C3BEA350FD8 (const RuntimeMethod* method);
// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::TryCompleteSelect()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SimulatedGestureHand_TryCompleteSelect_m39126D98BA2E83C742CDA9EAEA81EB5128B541AC (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method);
// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::TryCompleteHold()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SimulatedGestureHand_TryCompleteHold_mA3B5BAB738C6425798C608310D7D59D6B6FCA1AC (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method);
// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::TryCompleteManipulation()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SimulatedGestureHand_TryCompleteManipulation_m7DD88EA40E108EB197BF22BD11460BF7A3DFBB18 (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method);
// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::TryCompleteNavigation()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SimulatedGestureHand_TryCompleteNavigation_m725C944777267419341F15E256472663CBCE6AC8 (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method);
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::UpdateManipulation()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedGestureHand_UpdateManipulation_m7D7C54E9B0364BA9862D4326D9606FB6419CCBC3 (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method);
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::UpdateNavigation()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedGestureHand_UpdateNavigation_mD504939EDF859CD568D6127F467D193ADF3ADFC0 (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method);
// System.Single UnityEngine.Vector3::get_magnitude()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float Vector3_get_magnitude_m9A750659B60C5FE0C30438A7F9681775D5DB1274 (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * __this, const RuntimeMethod* method);
// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::TryCancelHold()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SimulatedGestureHand_TryCancelHold_m1F67089B7A138E396206FE8E7E0DAEECCE14BFBC (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method);
// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::TryStartNavigation()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SimulatedGestureHand_TryStartNavigation_m2F5F675D13ACB7225B7672755846459058BDF575 (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method);
// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::TryStartManipulation()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SimulatedGestureHand_TryStartManipulation_m0B58E7807CC8E31CE5F4817A99CC358085866A3E (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method);
// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::TryStartHold()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SimulatedGestureHand_TryStartHold_m72CBFF5CAEDDC55C9E865745A5DE4C34C1B2E234 (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method);
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::UpdateNavigationRails()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedGestureHand_UpdateNavigationRails_mDA8C27C354D28CD6BC7E7EB7E4A84A560D1B08A6 (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method);
// UnityEngine.Vector3 Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::get_navigationDelta()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  SimulatedGestureHand_get_navigationDelta_m0FD22233CFFA608F80B80E740D01DA6F8E22582A (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Vector3::op_Equality(UnityEngine.Vector3,UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Vector3_op_Equality_mA9E2F96E98E71AE7ACCE74766D700D41F0404806 (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___lhs0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___rhs1, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose>::.ctor()
inline void Dictionary_2__ctor_mD52EC03DD022577E1A73259E748910906383DA4E (Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 * __this, const RuntimeMethod* method)
{
	((  void (*) (Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 *, const RuntimeMethod*))Dictionary_2__ctor_m6F2ED586C8EC85B459FFCA36D05ABF98C1AA33B3_gshared)(__this, method);
}
// System.Void Microsoft.MixedReality.Toolkit.Input.BaseHand::.ctor(Microsoft.MixedReality.Toolkit.TrackingState,Microsoft.MixedReality.Toolkit.Utilities.Handedness,Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource,Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BaseHand__ctor_mD486A5087D9CF2CC6B1048F37EEAD182843CB503 (BaseHand_tB58ECFC99FBFD516BBAA0989004A10F687078F4B * __this, int32_t ___trackingState0, uint8_t ___controllerHandedness1, RuntimeObject* ___inputSource2, MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* ___interactions3, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.Dictionary`2<Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose>::TryGetValue(!0,!1&)
inline bool Dictionary_2_TryGetValue_mEB4E22F5D5C93FBC06285B7EA9EDC0B6B73CF31D (Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 * __this, int32_t ___key0, MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * ___value1, const RuntimeMethod* method)
{
	return ((  bool (*) (Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 *, int32_t, MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 *, const RuntimeMethod*))Dictionary_2_TryGetValue_m3816E3065E00AF57E62424BB45AEA6000BD27F49_gshared)(__this, ___key0, ___value1, method);
}
// System.Boolean System.Collections.Generic.Dictionary`2<Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose>::ContainsKey(!0)
inline bool Dictionary_2_ContainsKey_m9123BEB1C67E91B9D1C87834EED0E4805EAB9389 (Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 * __this, int32_t ___key0, const RuntimeMethod* method)
{
	return ((  bool (*) (Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 *, int32_t, const RuntimeMethod*))Dictionary_2_ContainsKey_m83D33BC652DBE4549C5B2C4A1E51BDA96E1989C6_gshared)(__this, ___key0, method);
}
// Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose[] Microsoft.MixedReality.Toolkit.Input.SimulatedHandData::get_Joints()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD* SimulatedHandData_get_Joints_m0137F96239589766E8132147EBBC5D1C24516B7C_inline (SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * __this, const RuntimeMethod* method);
// System.Void System.Collections.Generic.Dictionary`2<Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose>::Add(!0,!1)
inline void Dictionary_2_Add_mF5D352A2DB17E5E4545D622A66744A4697ACC3D2 (Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 * __this, int32_t ___key0, MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  ___value1, const RuntimeMethod* method)
{
	((  void (*) (Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 *, int32_t, MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 , const RuntimeMethod*))Dictionary_2_Add_mBB39FF6AADDEF60E949DF52642B7BA33E9CC5406_gshared)(__this, ___key0, ___value1, method);
}
// System.Void System.Collections.Generic.Dictionary`2<Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose>::set_Item(!0,!1)
inline void Dictionary_2_set_Item_mA73F452CC26A09DD780D50EAE46E8684633BA15B (Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 * __this, int32_t ___key0, MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  ___value1, const RuntimeMethod* method)
{
	((  void (*) (Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 *, int32_t, MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 , const RuntimeMethod*))Dictionary_2_set_Item_m72BAB8E16164B0649C7EFF83BD5C1904748DC7F0_gshared)(__this, ___key0, ___value1, method);
}
// System.Void Microsoft.MixedReality.Toolkit.Input.BaseHand::UpdateVelocity()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void BaseHand_UpdateVelocity_m2E2A6FE7655DBBE7E1BEBD9DAD7936B28DCEE484 (BaseHand_tB58ECFC99FBFD516BBAA0989004A10F687078F4B * __this, const RuntimeMethod* method);
// System.String[] System.Enum::GetNames(System.Type)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* Enum_GetNames_m9ECDF3E80A7A31075D7D2B2B362DDCC6150BC15C (Type_t * ___enumType0, const RuntimeMethod* method);
// System.Boolean Microsoft.MixedReality.Toolkit.MixedRealityServiceRegistry::TryGetService<Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem>(!!0&,System.String)
inline bool MixedRealityServiceRegistry_TryGetService_TisIMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_m11EAC52C13EC4EEBB2BC67A0F3F775159F619EAD (RuntimeObject** ___serviceInstance0, String_t* ___name1, const RuntimeMethod* method)
{
	return ((  bool (*) (RuntimeObject**, String_t*, const RuntimeMethod*))MixedRealityServiceRegistry_TryGetService_TisRuntimeObject_m2354211184CA13FEA1094444215C1DE746B56354_gshared)(___serviceInstance0, ___name1, method);
}
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedHandData/HandJointDataGenerator::Invoke(Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void HandJointDataGenerator_Invoke_m453D8F003A5B2375922D4E902074628FA4AAB4F2 (HandJointDataGenerator_t70BF622884D5C475C85D34FDE76FD298FAC37955 * __this, MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD* ___jointPoses0, const RuntimeMethod* method);
// System.Void System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(System.Array,System.RuntimeFieldHandle)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void RuntimeHelpers_InitializeArray_m29F50CDFEEE0AB868200291366253DD4737BC76A (RuntimeArray * ___array0, RuntimeFieldHandle_t844BDF00E8E6FE69D9AEAA7657F09018B864F4EF  ___fldHandle1, const RuntimeMethod* method);
// System.Collections.Generic.IEnumerable`1<!!0> System.Linq.Enumerable::Take<System.Int32>(System.Collections.Generic.IEnumerable`1<!!0>,System.Int32)
inline RuntimeObject* Enumerable_Take_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_mCBED6C7F74DCC17FA9C923D11B6801F52FEEB61B (RuntimeObject* ___source0, int32_t ___count1, const RuntimeMethod* method)
{
	return ((  RuntimeObject* (*) (RuntimeObject*, int32_t, const RuntimeMethod*))Enumerable_Take_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_mCBED6C7F74DCC17FA9C923D11B6801F52FEEB61B_gshared)(___source0, ___count1, method);
}
// System.Int32 System.Linq.Enumerable::Sum(System.Collections.Generic.IEnumerable`1<System.Int32>)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Enumerable_Sum_mA81913DBCF3086B4716F692F9DB797D7DD6B7583 (RuntimeObject* ___source0, const RuntimeMethod* method);
// UnityEngine.Vector3 Microsoft.MixedReality.Toolkit.Input.SimulatedHandUtils::GetPalmRightVector(Microsoft.MixedReality.Toolkit.Utilities.Handedness,UnityEngine.Vector3[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  SimulatedHandUtils_GetPalmRightVector_m9C646FB51F2C94823DC3EEE26383B22A88EA4301 (uint8_t ___handedness0, Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* ___jointPositions1, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Vector3::Cross(UnityEngine.Vector3,UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Vector3_Cross_m3E9DBC445228FDB850BDBB4B01D6F61AC0111887 (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___lhs0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___rhs1, const RuntimeMethod* method);
// UnityEngine.Quaternion UnityEngine.Quaternion::LookRotation(UnityEngine.Vector3,UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  Quaternion_LookRotation_m7BED8FBB457FF073F183AC7962264E5110794672 (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___forward0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___upwards1, const RuntimeMethod* method);
// UnityEngine.Quaternion UnityEngine.Quaternion::AngleAxis(System.Single,UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  Quaternion_AngleAxis_m07DACF59F0403451DABB9BC991C53EE3301E88B0 (float ___angle0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___axis1, const RuntimeMethod* method);
// UnityEngine.Quaternion UnityEngine.Quaternion::op_Multiply(UnityEngine.Quaternion,UnityEngine.Quaternion)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  Quaternion_op_Multiply_mDB9F738AA8160E3D85549F4FEDA23BC658B5A790 (Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___lhs0, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___rhs1, const RuntimeMethod* method);
// UnityEngine.Vector3 Microsoft.MixedReality.Toolkit.Input.SimulatedHandUtils::GetPalmForwardVector(UnityEngine.Vector3[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  SimulatedHandUtils_GetPalmForwardVector_m9E069A581F41648ADB1D947EDBB726BD867602F4 (Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* ___jointPositions0, const RuntimeMethod* method);
// UnityEngine.Vector3 Microsoft.MixedReality.Toolkit.Input.SimulatedHandUtils::GetPalmUpVector(Microsoft.MixedReality.Toolkit.Utilities.Handedness,UnityEngine.Vector3[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  SimulatedHandUtils_GetPalmUpVector_mB1852A38F5919EC805FE801DB47DC6DA1E64CCD0 (uint8_t ___handedness0, Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* ___jointPositions1, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Vector3::get_normalized()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Vector3_get_normalized_mE20796F1D2D36244FACD4D14DADB245BE579849B (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * __this, const RuntimeMethod* method);
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void Microsoft.MixedReality.Toolkit.Input.KeyBinding::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void KeyBinding__cctor_m52B9381B882303097E8CC5BE8025234BAC0A75DA (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (KeyBinding__cctor_m52B9381B882303097E8CC5BE8025234BAC0A75DA_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	KeyCodeU5BU5D_tF4382F22534318B6E15A70B33AAF395B3D8D127F* V_0 = NULL;
	U3CU3Ec__DisplayClass5_0_t5532E81B72C939F27BA424481612158E32B0C681 * V_1 = NULL;
	Action_2_t599C81CC1C0CDFE287E5D39D3EEB3130080399E8 * V_2 = NULL;
	MouseButtonU5BU5D_t6CE0267665AAD6A7B40F7782DA60DD3810558E82* V_3 = NULL;
	int32_t V_4 = 0;
	int32_t V_5 = 0;
	KeyCodeU5BU5D_tF4382F22534318B6E15A70B33AAF395B3D8D127F* V_6 = NULL;
	int32_t V_7 = 0;
	{
		// KeyCode[] KeyCodeValues = (KeyCode[])Enum.GetValues(typeof(KeyCode));
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_0 = { reinterpret_cast<intptr_t> (KeyCode_tC93EA87C5A6901160B583ADFCD3EF6726570DC3C_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_1 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_0, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Enum_t2AF27C02B8653AE29442467390005ABC74D8F521_il2cpp_TypeInfo_var);
		RuntimeArray * L_2 = Enum_GetValues_m20F5C0B826344A499B1C23BB7A3B532017F0F30C(L_1, /*hidden argument*/NULL);
		V_0 = ((KeyCodeU5BU5D_tF4382F22534318B6E15A70B33AAF395B3D8D127F*)Castclass((RuntimeObject*)L_2, KeyCodeU5BU5D_tF4382F22534318B6E15A70B33AAF395B3D8D127F_il2cpp_TypeInfo_var));
		// MouseButton[] MouseButtonValues = (MouseButton[])Enum.GetValues(typeof(MouseButton));
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_3 = { reinterpret_cast<intptr_t> (MouseButton_t4174FC057A73B1ECBC9603C3AF8AF87E964E719E_0_0_0_var) };
		Type_t * L_4 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_3, /*hidden argument*/NULL);
		RuntimeArray * L_5 = Enum_GetValues_m20F5C0B826344A499B1C23BB7A3B532017F0F30C(L_4, /*hidden argument*/NULL);
		U3CU3Ec__DisplayClass5_0_t5532E81B72C939F27BA424481612158E32B0C681 * L_6 = (U3CU3Ec__DisplayClass5_0_t5532E81B72C939F27BA424481612158E32B0C681 *)il2cpp_codegen_object_new(U3CU3Ec__DisplayClass5_0_t5532E81B72C939F27BA424481612158E32B0C681_il2cpp_TypeInfo_var);
		U3CU3Ec__DisplayClass5_0__ctor_m765F71F1687CD4EA6EF78246D50807FA94326711(L_6, /*hidden argument*/NULL);
		V_1 = L_6;
		// KeyBindingToEnumMap = new Dictionary<Tuple<KeyType, int>, int>();
		Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB * L_7 = (Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB *)il2cpp_codegen_object_new(Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB_il2cpp_TypeInfo_var);
		Dictionary_2__ctor_m747FD3B997983E98D0914810BA2B843ED90D554B(L_7, /*hidden argument*/Dictionary_2__ctor_m747FD3B997983E98D0914810BA2B843ED90D554B_RuntimeMethod_var);
		((KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79_StaticFields*)il2cpp_codegen_static_fields_for(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79_il2cpp_TypeInfo_var))->set_KeyBindingToEnumMap_1(L_7);
		// EnumToKeyBindingMap = new Dictionary<int, Tuple<KeyType, int>>();
		Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64 * L_8 = (Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64 *)il2cpp_codegen_object_new(Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64_il2cpp_TypeInfo_var);
		Dictionary_2__ctor_m2298C894CE2941227F176A13E8FF938BD954E63B(L_8, /*hidden argument*/Dictionary_2__ctor_m2298C894CE2941227F176A13E8FF938BD954E63B_RuntimeMethod_var);
		((KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79_StaticFields*)il2cpp_codegen_static_fields_for(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79_il2cpp_TypeInfo_var))->set_EnumToKeyBindingMap_2(L_8);
		// List<string> names = new List<string>();
		U3CU3Ec__DisplayClass5_0_t5532E81B72C939F27BA424481612158E32B0C681 * L_9 = V_1;
		List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3 * L_10 = (List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3 *)il2cpp_codegen_object_new(List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3_il2cpp_TypeInfo_var);
		List_1__ctor_mDA22758D73530683C950C5CCF39BDB4E7E1F3F06(L_10, /*hidden argument*/List_1__ctor_mDA22758D73530683C950C5CCF39BDB4E7E1F3F06_RuntimeMethod_var);
		NullCheck(L_9);
		L_9->set_names_0(L_10);
		// int index = 0;
		U3CU3Ec__DisplayClass5_0_t5532E81B72C939F27BA424481612158E32B0C681 * L_11 = V_1;
		NullCheck(L_11);
		L_11->set_index_1(0);
		// Action<KeyType, int> AddEnumValue = (bindingType, code) =>
		// {
		//     var kb = new KeyBinding() { bindingType=bindingType, code=code };
		//     names.Add(kb.ToString());
		//     EnumToKeyBindingMap[index] = Tuple.Create(bindingType, code);
		//     KeyBindingToEnumMap[Tuple.Create(bindingType, code)] = index;
		// 
		//     ++index;
		// };
		U3CU3Ec__DisplayClass5_0_t5532E81B72C939F27BA424481612158E32B0C681 * L_12 = V_1;
		Action_2_t599C81CC1C0CDFE287E5D39D3EEB3130080399E8 * L_13 = (Action_2_t599C81CC1C0CDFE287E5D39D3EEB3130080399E8 *)il2cpp_codegen_object_new(Action_2_t599C81CC1C0CDFE287E5D39D3EEB3130080399E8_il2cpp_TypeInfo_var);
		Action_2__ctor_mF9F632823062B05D3DA92A0649DC4EE862AE1C7A(L_13, L_12, (intptr_t)((intptr_t)U3CU3Ec__DisplayClass5_0_U3C_cctorU3Eb__0_m7589D4054CF6C9029801CCE9EC4CD741486AD169_RuntimeMethod_var), /*hidden argument*/Action_2__ctor_mF9F632823062B05D3DA92A0649DC4EE862AE1C7A_RuntimeMethod_var);
		V_2 = L_13;
		// AddEnumValue(KeyType.None, 0);
		Action_2_t599C81CC1C0CDFE287E5D39D3EEB3130080399E8 * L_14 = V_2;
		NullCheck(L_14);
		Action_2_Invoke_m0D15E6E36BD572A4DF315B9F04F30A0F0EFE31E5(L_14, 0, 0, /*hidden argument*/Action_2_Invoke_m0D15E6E36BD572A4DF315B9F04F30A0F0EFE31E5_RuntimeMethod_var);
		// foreach (MouseButton mb in MouseButtonValues)
		V_3 = ((MouseButtonU5BU5D_t6CE0267665AAD6A7B40F7782DA60DD3810558E82*)Castclass((RuntimeObject*)L_5, MouseButtonU5BU5D_t6CE0267665AAD6A7B40F7782DA60DD3810558E82_il2cpp_TypeInfo_var));
		V_4 = 0;
		goto IL_0085;
	}

IL_0070:
	{
		// foreach (MouseButton mb in MouseButtonValues)
		MouseButtonU5BU5D_t6CE0267665AAD6A7B40F7782DA60DD3810558E82* L_15 = V_3;
		int32_t L_16 = V_4;
		NullCheck(L_15);
		int32_t L_17 = L_16;
		int32_t L_18 = (int32_t)(L_15)->GetAt(static_cast<il2cpp_array_size_t>(L_17));
		V_5 = L_18;
		// AddEnumValue(KeyType.Mouse, (int)mb);
		Action_2_t599C81CC1C0CDFE287E5D39D3EEB3130080399E8 * L_19 = V_2;
		int32_t L_20 = V_5;
		NullCheck(L_19);
		Action_2_Invoke_m0D15E6E36BD572A4DF315B9F04F30A0F0EFE31E5(L_19, 1, L_20, /*hidden argument*/Action_2_Invoke_m0D15E6E36BD572A4DF315B9F04F30A0F0EFE31E5_RuntimeMethod_var);
		int32_t L_21 = V_4;
		V_4 = ((int32_t)il2cpp_codegen_add((int32_t)L_21, (int32_t)1));
	}

IL_0085:
	{
		// foreach (MouseButton mb in MouseButtonValues)
		int32_t L_22 = V_4;
		MouseButtonU5BU5D_t6CE0267665AAD6A7B40F7782DA60DD3810558E82* L_23 = V_3;
		NullCheck(L_23);
		if ((((int32_t)L_22) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_23)->max_length)))))))
		{
			goto IL_0070;
		}
	}
	{
		// foreach (KeyCode kc in KeyCodeValues)
		KeyCodeU5BU5D_tF4382F22534318B6E15A70B33AAF395B3D8D127F* L_24 = V_0;
		V_6 = L_24;
		V_4 = 0;
		goto IL_00aa;
	}

IL_0094:
	{
		// foreach (KeyCode kc in KeyCodeValues)
		KeyCodeU5BU5D_tF4382F22534318B6E15A70B33AAF395B3D8D127F* L_25 = V_6;
		int32_t L_26 = V_4;
		NullCheck(L_25);
		int32_t L_27 = L_26;
		int32_t L_28 = (int32_t)(L_25)->GetAt(static_cast<il2cpp_array_size_t>(L_27));
		V_7 = L_28;
		// AddEnumValue(KeyType.Key, (int)kc);
		Action_2_t599C81CC1C0CDFE287E5D39D3EEB3130080399E8 * L_29 = V_2;
		int32_t L_30 = V_7;
		NullCheck(L_29);
		Action_2_Invoke_m0D15E6E36BD572A4DF315B9F04F30A0F0EFE31E5(L_29, 2, L_30, /*hidden argument*/Action_2_Invoke_m0D15E6E36BD572A4DF315B9F04F30A0F0EFE31E5_RuntimeMethod_var);
		int32_t L_31 = V_4;
		V_4 = ((int32_t)il2cpp_codegen_add((int32_t)L_31, (int32_t)1));
	}

IL_00aa:
	{
		// foreach (KeyCode kc in KeyCodeValues)
		int32_t L_32 = V_4;
		KeyCodeU5BU5D_tF4382F22534318B6E15A70B33AAF395B3D8D127F* L_33 = V_6;
		NullCheck(L_33);
		if ((((int32_t)L_32) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_33)->max_length)))))))
		{
			goto IL_0094;
		}
	}
	{
		// AllCodeNames = names.ToArray();
		U3CU3Ec__DisplayClass5_0_t5532E81B72C939F27BA424481612158E32B0C681 * L_34 = V_1;
		NullCheck(L_34);
		List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3 * L_35 = L_34->get_names_0();
		NullCheck(L_35);
		StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* L_36 = List_1_ToArray_m9DD19D800AE6D84ED0729D5D97CAF84DF317DD38(L_35, /*hidden argument*/List_1_ToArray_m9DD19D800AE6D84ED0729D5D97CAF84DF317DD38_RuntimeMethod_var);
		((KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79_StaticFields*)il2cpp_codegen_static_fields_for(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79_il2cpp_TypeInfo_var))->set_AllCodeNames_0(L_36);
		// }
		return;
	}
}
// Microsoft.MixedReality.Toolkit.Input.KeyBinding_KeyType Microsoft.MixedReality.Toolkit.Input.KeyBinding::get_BindingType()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t KeyBinding_get_BindingType_mA6915A48809778FE77561961A250F3D5BEABFE91 (KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 * __this, const RuntimeMethod* method)
{
	{
		// public KeyType BindingType => bindingType;
		int32_t L_0 = __this->get_bindingType_3();
		return L_0;
	}
}
IL2CPP_EXTERN_C  int32_t KeyBinding_get_BindingType_mA6915A48809778FE77561961A250F3D5BEABFE91_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 * _thisAdjusted = reinterpret_cast<KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 *>(__this + 1);
	return KeyBinding_get_BindingType_mA6915A48809778FE77561961A250F3D5BEABFE91_inline(_thisAdjusted, method);
}
// System.String Microsoft.MixedReality.Toolkit.Input.KeyBinding::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* KeyBinding_ToString_mB8F2F02D75495579EEDDB8B27851E0BFC044B526 (KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (KeyBinding_ToString_mB8F2F02D75495579EEDDB8B27851E0BFC044B526_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	String_t* V_0 = NULL;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	{
		// string s = "";
		V_0 = _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709;
		// s += bindingType.ToString();
		String_t* L_0 = V_0;
		int32_t* L_1 = __this->get_address_of_bindingType_3();
		RuntimeObject * L_2 = Box(KeyType_t63A0EC9B1C9653881B95DF409080C7FB24760D72_il2cpp_TypeInfo_var, L_1);
		NullCheck(L_2);
		String_t* L_3 = VirtFuncInvoker0< String_t* >::Invoke(3 /* System.String System.Object::ToString() */, L_2);
		*L_1 = *(int32_t*)UnBox(L_2);
		String_t* L_4 = String_Concat_mB78D0094592718DA6D5DB6C712A9C225631666BE(L_0, L_3, /*hidden argument*/NULL);
		V_0 = L_4;
		// switch (bindingType)
		int32_t L_5 = __this->get_bindingType_3();
		V_1 = L_5;
		int32_t L_6 = V_1;
		if ((((int32_t)L_6) == ((int32_t)1)))
		{
			goto IL_004f;
		}
	}
	{
		int32_t L_7 = V_1;
		if ((!(((uint32_t)L_7) == ((uint32_t)2))))
		{
			goto IL_006f;
		}
	}
	{
		// s += ": " + ((KeyCode)code).ToString();
		String_t* L_8 = V_0;
		int32_t L_9 = __this->get_code_4();
		V_2 = L_9;
		RuntimeObject * L_10 = Box(KeyCode_tC93EA87C5A6901160B583ADFCD3EF6726570DC3C_il2cpp_TypeInfo_var, (&V_2));
		NullCheck(L_10);
		String_t* L_11 = VirtFuncInvoker0< String_t* >::Invoke(3 /* System.String System.Object::ToString() */, L_10);
		V_2 = *(int32_t*)UnBox(L_10);
		String_t* L_12 = String_Concat_mF4626905368D6558695A823466A1AF65EADB9923(L_8, _stringLiteralCECA32E904728D1645727CB2B9CDEAA153807D77, L_11, /*hidden argument*/NULL);
		V_0 = L_12;
		// break;
		goto IL_006f;
	}

IL_004f:
	{
		// s += ": " + ((MouseButton)code).ToString();
		String_t* L_13 = V_0;
		int32_t L_14 = __this->get_code_4();
		V_3 = L_14;
		RuntimeObject * L_15 = Box(MouseButton_t4174FC057A73B1ECBC9603C3AF8AF87E964E719E_il2cpp_TypeInfo_var, (&V_3));
		NullCheck(L_15);
		String_t* L_16 = VirtFuncInvoker0< String_t* >::Invoke(3 /* System.String System.Object::ToString() */, L_15);
		V_3 = *(int32_t*)UnBox(L_15);
		String_t* L_17 = String_Concat_mF4626905368D6558695A823466A1AF65EADB9923(L_13, _stringLiteralCECA32E904728D1645727CB2B9CDEAA153807D77, L_16, /*hidden argument*/NULL);
		V_0 = L_17;
	}

IL_006f:
	{
		// return s;
		String_t* L_18 = V_0;
		return L_18;
	}
}
IL2CPP_EXTERN_C  String_t* KeyBinding_ToString_mB8F2F02D75495579EEDDB8B27851E0BFC044B526_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 * _thisAdjusted = reinterpret_cast<KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 *>(__this + 1);
	return KeyBinding_ToString_mB8F2F02D75495579EEDDB8B27851E0BFC044B526(_thisAdjusted, method);
}
// System.Boolean Microsoft.MixedReality.Toolkit.Input.KeyBinding::TryGetKeyCode(UnityEngine.KeyCode&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool KeyBinding_TryGetKeyCode_m185188BD7AFC2303E3DE3BB2161E6280DB676382 (KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 * __this, int32_t* ___keyCode0, const RuntimeMethod* method)
{
	{
		// keyCode = (KeyCode)code;
		int32_t* L_0 = ___keyCode0;
		int32_t L_1 = __this->get_code_4();
		*((int32_t*)L_0) = (int32_t)L_1;
		// return bindingType == KeyType.Key;
		int32_t L_2 = __this->get_bindingType_3();
		return (bool)((((int32_t)L_2) == ((int32_t)2))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool KeyBinding_TryGetKeyCode_m185188BD7AFC2303E3DE3BB2161E6280DB676382_AdjustorThunk (RuntimeObject * __this, int32_t* ___keyCode0, const RuntimeMethod* method)
{
	KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 * _thisAdjusted = reinterpret_cast<KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 *>(__this + 1);
	return KeyBinding_TryGetKeyCode_m185188BD7AFC2303E3DE3BB2161E6280DB676382(_thisAdjusted, ___keyCode0, method);
}
// System.Boolean Microsoft.MixedReality.Toolkit.Input.KeyBinding::TryGetMouseButton(System.Int32&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool KeyBinding_TryGetMouseButton_m398435CC5A7F9427B8C7932A8714E496ED650DEC (KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 * __this, int32_t* ___mouseButton0, const RuntimeMethod* method)
{
	{
		// mouseButton = code;
		int32_t* L_0 = ___mouseButton0;
		int32_t L_1 = __this->get_code_4();
		*((int32_t*)L_0) = (int32_t)L_1;
		// return bindingType == KeyType.Mouse;
		int32_t L_2 = __this->get_bindingType_3();
		return (bool)((((int32_t)L_2) == ((int32_t)1))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool KeyBinding_TryGetMouseButton_m398435CC5A7F9427B8C7932A8714E496ED650DEC_AdjustorThunk (RuntimeObject * __this, int32_t* ___mouseButton0, const RuntimeMethod* method)
{
	KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 * _thisAdjusted = reinterpret_cast<KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 *>(__this + 1);
	return KeyBinding_TryGetMouseButton_m398435CC5A7F9427B8C7932A8714E496ED650DEC(_thisAdjusted, ___mouseButton0, method);
}
// System.Boolean Microsoft.MixedReality.Toolkit.Input.KeyBinding::TryGetMouseButton(Microsoft.MixedReality.Toolkit.Input.KeyBinding_MouseButton&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool KeyBinding_TryGetMouseButton_mC9D3F31ECF45FC2649872D0FE531235E0C46F6A2 (KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 * __this, int32_t* ___mouseButton0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		// if (TryGetMouseButton(out int iMouseButton))
		bool L_0 = KeyBinding_TryGetMouseButton_m398435CC5A7F9427B8C7932A8714E496ED650DEC((KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 *)__this, (int32_t*)(&V_0), /*hidden argument*/NULL);
		if (!L_0)
		{
			goto IL_000f;
		}
	}
	{
		// mouseButton = (MouseButton)iMouseButton;
		int32_t* L_1 = ___mouseButton0;
		int32_t L_2 = V_0;
		*((int32_t*)L_1) = (int32_t)L_2;
		// return true;
		return (bool)1;
	}

IL_000f:
	{
		// mouseButton = MouseButton.Left;
		int32_t* L_3 = ___mouseButton0;
		*((int32_t*)L_3) = (int32_t)0;
		// return false;
		return (bool)0;
	}
}
IL2CPP_EXTERN_C  bool KeyBinding_TryGetMouseButton_mC9D3F31ECF45FC2649872D0FE531235E0C46F6A2_AdjustorThunk (RuntimeObject * __this, int32_t* ___mouseButton0, const RuntimeMethod* method)
{
	KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 * _thisAdjusted = reinterpret_cast<KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 *>(__this + 1);
	return KeyBinding_TryGetMouseButton_mC9D3F31ECF45FC2649872D0FE531235E0C46F6A2(_thisAdjusted, ___mouseButton0, method);
}
// Microsoft.MixedReality.Toolkit.Input.KeyBinding Microsoft.MixedReality.Toolkit.Input.KeyBinding::Unbound()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  KeyBinding_Unbound_mF7F71D30B4BC4B0139821EC1F073851666D0E3D7 (const RuntimeMethod* method)
{
	KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// KeyBinding kb = new KeyBinding();
		il2cpp_codegen_initobj((&V_0), sizeof(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 ));
		// kb.bindingType = KeyType.None;
		(&V_0)->set_bindingType_3(0);
		// kb.code = 0;
		(&V_0)->set_code_4(0);
		// return kb;
		KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  L_0 = V_0;
		return L_0;
	}
}
// Microsoft.MixedReality.Toolkit.Input.KeyBinding Microsoft.MixedReality.Toolkit.Input.KeyBinding::FromKey(UnityEngine.KeyCode)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  KeyBinding_FromKey_m4E6BB297D9741E6C9C1FD8CB946CB140C4FD1DE5 (int32_t ___keyCode0, const RuntimeMethod* method)
{
	KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// KeyBinding kb = new KeyBinding();
		il2cpp_codegen_initobj((&V_0), sizeof(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 ));
		// kb.bindingType = KeyType.Key;
		(&V_0)->set_bindingType_3(2);
		// kb.code = (int)keyCode;
		int32_t L_0 = ___keyCode0;
		(&V_0)->set_code_4(L_0);
		// return kb;
		KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  L_1 = V_0;
		return L_1;
	}
}
// Microsoft.MixedReality.Toolkit.Input.KeyBinding Microsoft.MixedReality.Toolkit.Input.KeyBinding::FromMouseButton(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  KeyBinding_FromMouseButton_m9C1C18324382689D26647131D9C8CD2D71B71CF2 (int32_t ___mouseButton0, const RuntimeMethod* method)
{
	KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// KeyBinding kb = new KeyBinding();
		il2cpp_codegen_initobj((&V_0), sizeof(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 ));
		// kb.bindingType = KeyType.Mouse;
		(&V_0)->set_bindingType_3(1);
		// kb.code = mouseButton;
		int32_t L_0 = ___mouseButton0;
		(&V_0)->set_code_4(L_0);
		// return kb;
		KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  L_1 = V_0;
		return L_1;
	}
}
// Microsoft.MixedReality.Toolkit.Input.KeyBinding Microsoft.MixedReality.Toolkit.Input.KeyBinding::FromMouseButton(Microsoft.MixedReality.Toolkit.Input.KeyBinding_MouseButton)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  KeyBinding_FromMouseButton_mC7479108FCC71C952AAB38A9526E2B82B71C8CD0 (int32_t ___mouseButton0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (KeyBinding_FromMouseButton_mC7479108FCC71C952AAB38A9526E2B82B71C8CD0_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// return FromMouseButton((int)mouseButton);
		int32_t L_0 = ___mouseButton0;
		IL2CPP_RUNTIME_CLASS_INIT(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79_il2cpp_TypeInfo_var);
		KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  L_1 = KeyBinding_FromMouseButton_m9C1C18324382689D26647131D9C8CD2D71B71CF2(L_0, /*hidden argument*/NULL);
		return L_1;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void Microsoft.MixedReality.Toolkit.Input.KeyBinding_<>c__DisplayClass5_0::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass5_0__ctor_m765F71F1687CD4EA6EF78246D50807FA94326711 (U3CU3Ec__DisplayClass5_0_t5532E81B72C939F27BA424481612158E32B0C681 * __this, const RuntimeMethod* method)
{
	{
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		return;
	}
}
// System.Void Microsoft.MixedReality.Toolkit.Input.KeyBinding_<>c__DisplayClass5_0::<.cctor>b__0(Microsoft.MixedReality.Toolkit.Input.KeyBinding_KeyType,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CU3Ec__DisplayClass5_0_U3C_cctorU3Eb__0_m7589D4054CF6C9029801CCE9EC4CD741486AD169 (U3CU3Ec__DisplayClass5_0_t5532E81B72C939F27BA424481612158E32B0C681 * __this, int32_t ___bindingType0, int32_t ___code1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (U3CU3Ec__DisplayClass5_0_U3C_cctorU3Eb__0_m7589D4054CF6C9029801CCE9EC4CD741486AD169_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  V_0;
	memset((&V_0), 0, sizeof(V_0));
	KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  V_1;
	memset((&V_1), 0, sizeof(V_1));
	int32_t V_2 = 0;
	{
		// var kb = new KeyBinding() { bindingType=bindingType, code=code };
		il2cpp_codegen_initobj((&V_1), sizeof(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 ));
		int32_t L_0 = ___bindingType0;
		(&V_1)->set_bindingType_3(L_0);
		int32_t L_1 = ___code1;
		(&V_1)->set_code_4(L_1);
		KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  L_2 = V_1;
		V_0 = L_2;
		// names.Add(kb.ToString());
		List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3 * L_3 = __this->get_names_0();
		String_t* L_4 = KeyBinding_ToString_mB8F2F02D75495579EEDDB8B27851E0BFC044B526((KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 *)(&V_0), /*hidden argument*/NULL);
		NullCheck(L_3);
		List_1_Add_mA348FA1140766465189459D25B01EB179001DE83(L_3, L_4, /*hidden argument*/List_1_Add_mA348FA1140766465189459D25B01EB179001DE83_RuntimeMethod_var);
		// EnumToKeyBindingMap[index] = Tuple.Create(bindingType, code);
		IL2CPP_RUNTIME_CLASS_INIT(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79_il2cpp_TypeInfo_var);
		Dictionary_2_t851109C8EC3B462C09C470AA73AA5F6A82D61B64 * L_5 = ((KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79_StaticFields*)il2cpp_codegen_static_fields_for(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79_il2cpp_TypeInfo_var))->get_EnumToKeyBindingMap_2();
		int32_t L_6 = __this->get_index_1();
		int32_t L_7 = ___bindingType0;
		int32_t L_8 = ___code1;
		Tuple_2_tFF0D9FEC0FEA81089BD6B1384583703BD0A104EE * L_9 = Tuple_Create_TisKeyType_t63A0EC9B1C9653881B95DF409080C7FB24760D72_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_mA5D31171EBE5513EC23DF8E079EC60FE1EE2E658(L_7, L_8, /*hidden argument*/Tuple_Create_TisKeyType_t63A0EC9B1C9653881B95DF409080C7FB24760D72_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_mA5D31171EBE5513EC23DF8E079EC60FE1EE2E658_RuntimeMethod_var);
		NullCheck(L_5);
		Dictionary_2_set_Item_m3D5CB4BFE05FDFFBEFF66F28C80B6AF3A94ECBF5(L_5, L_6, L_9, /*hidden argument*/Dictionary_2_set_Item_m3D5CB4BFE05FDFFBEFF66F28C80B6AF3A94ECBF5_RuntimeMethod_var);
		// KeyBindingToEnumMap[Tuple.Create(bindingType, code)] = index;
		Dictionary_2_tCCE7E3DED5BB9D85ABD0F224C25BBC56DC6FB0CB * L_10 = ((KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79_StaticFields*)il2cpp_codegen_static_fields_for(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79_il2cpp_TypeInfo_var))->get_KeyBindingToEnumMap_1();
		int32_t L_11 = ___bindingType0;
		int32_t L_12 = ___code1;
		Tuple_2_tFF0D9FEC0FEA81089BD6B1384583703BD0A104EE * L_13 = Tuple_Create_TisKeyType_t63A0EC9B1C9653881B95DF409080C7FB24760D72_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_mA5D31171EBE5513EC23DF8E079EC60FE1EE2E658(L_11, L_12, /*hidden argument*/Tuple_Create_TisKeyType_t63A0EC9B1C9653881B95DF409080C7FB24760D72_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_mA5D31171EBE5513EC23DF8E079EC60FE1EE2E658_RuntimeMethod_var);
		int32_t L_14 = __this->get_index_1();
		NullCheck(L_10);
		Dictionary_2_set_Item_m71327547831A3689A4215232C29A1EBA103BE6DE(L_10, L_13, L_14, /*hidden argument*/Dictionary_2_set_Item_m71327547831A3689A4215232C29A1EBA103BE6DE_RuntimeMethod_var);
		// ++index;
		int32_t L_15 = __this->get_index_1();
		V_2 = ((int32_t)il2cpp_codegen_add((int32_t)L_15, (int32_t)1));
		int32_t L_16 = V_2;
		__this->set_index_1(L_16);
		// };
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Boolean Microsoft.MixedReality.Toolkit.Input.KeyInputSystem::GetKey(Microsoft.MixedReality.Toolkit.Input.KeyBinding)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool KeyInputSystem_GetKey_m2EE019355844DA29C17B8678F49586574D5A6D49 (KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  ___kb0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	{
		// if (kb.TryGetMouseButton(out int mouseButton))
		bool L_0 = KeyBinding_TryGetMouseButton_m398435CC5A7F9427B8C7932A8714E496ED650DEC((KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 *)(&___kb0), (int32_t*)(&V_0), /*hidden argument*/NULL);
		if (!L_0)
		{
			goto IL_0012;
		}
	}
	{
		// return UnityEngine.Input.GetMouseButton(mouseButton);
		int32_t L_1 = V_0;
		bool L_2 = Input_GetMouseButton_m43C68DE93C7D990E875BA53C4DEC9CA6230C8B79(L_1, /*hidden argument*/NULL);
		return L_2;
	}

IL_0012:
	{
		// if (kb.TryGetKeyCode(out KeyCode keyCode))
		bool L_3 = KeyBinding_TryGetKeyCode_m185188BD7AFC2303E3DE3BB2161E6280DB676382((KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 *)(&___kb0), (int32_t*)(&V_1), /*hidden argument*/NULL);
		if (!L_3)
		{
			goto IL_0024;
		}
	}
	{
		// return UnityEngine.Input.GetKey(keyCode);
		int32_t L_4 = V_1;
		bool L_5 = Input_GetKey_m46AA83E14F9C3A75E06FE0A8C55740D47B2DB784(L_4, /*hidden argument*/NULL);
		return L_5;
	}

IL_0024:
	{
		// return false;
		return (bool)0;
	}
}
// System.Boolean Microsoft.MixedReality.Toolkit.Input.KeyInputSystem::GetKeyDown(Microsoft.MixedReality.Toolkit.Input.KeyBinding)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool KeyInputSystem_GetKeyDown_mC60446A0EE3AD6F0C1E9A6CBCBDA6160133C3E6C (KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  ___kb0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	{
		// if (kb.TryGetMouseButton(out int mouseButton))
		bool L_0 = KeyBinding_TryGetMouseButton_m398435CC5A7F9427B8C7932A8714E496ED650DEC((KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 *)(&___kb0), (int32_t*)(&V_0), /*hidden argument*/NULL);
		if (!L_0)
		{
			goto IL_0012;
		}
	}
	{
		// return UnityEngine.Input.GetMouseButtonDown(mouseButton);
		int32_t L_1 = V_0;
		bool L_2 = Input_GetMouseButtonDown_m5AD76E22AA839706219AD86A4E0BE5276AF8E28A(L_1, /*hidden argument*/NULL);
		return L_2;
	}

IL_0012:
	{
		// if (kb.TryGetKeyCode(out KeyCode keyCode))
		bool L_3 = KeyBinding_TryGetKeyCode_m185188BD7AFC2303E3DE3BB2161E6280DB676382((KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 *)(&___kb0), (int32_t*)(&V_1), /*hidden argument*/NULL);
		if (!L_3)
		{
			goto IL_0024;
		}
	}
	{
		// return UnityEngine.Input.GetKeyDown(keyCode);
		int32_t L_4 = V_1;
		bool L_5 = Input_GetKeyDown_mEA57896808B6F484B12CD0AEEB83390A3CFCDBDC(L_4, /*hidden argument*/NULL);
		return L_5;
	}

IL_0024:
	{
		// return false;
		return (bool)0;
	}
}
// System.Boolean Microsoft.MixedReality.Toolkit.Input.KeyInputSystem::GetKeyUp(Microsoft.MixedReality.Toolkit.Input.KeyBinding)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool KeyInputSystem_GetKeyUp_mA01B73327240946CC4F0FBB48DB7E6216F01972A (KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  ___kb0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	{
		// if (kb.TryGetMouseButton(out int mouseButton))
		bool L_0 = KeyBinding_TryGetMouseButton_m398435CC5A7F9427B8C7932A8714E496ED650DEC((KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 *)(&___kb0), (int32_t*)(&V_0), /*hidden argument*/NULL);
		if (!L_0)
		{
			goto IL_0012;
		}
	}
	{
		// return UnityEngine.Input.GetMouseButtonUp(mouseButton);
		int32_t L_1 = V_0;
		bool L_2 = Input_GetMouseButtonUp_m4899272EB31D43EC4A3A1A115843CD3D9AA2C4EC(L_1, /*hidden argument*/NULL);
		return L_2;
	}

IL_0012:
	{
		// if (kb.TryGetKeyCode(out KeyCode keyCode))
		bool L_3 = KeyBinding_TryGetKeyCode_m185188BD7AFC2303E3DE3BB2161E6280DB676382((KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 *)(&___kb0), (int32_t*)(&V_1), /*hidden argument*/NULL);
		if (!L_3)
		{
			goto IL_0024;
		}
	}
	{
		// return UnityEngine.Input.GetKeyUp(keyCode);
		int32_t L_4 = V_1;
		bool L_5 = Input_GetKeyUp_m5345ECFA25B7AC99D6D4223DA23BB9FB991B7193(L_4, /*hidden argument*/NULL);
		return L_5;
	}

IL_0024:
	{
		// return false;
		return (bool)0;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// UnityEngine.GameObject Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_IndicatorsPrefab()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * MixedRealityInputSimulationProfile_get_IndicatorsPrefab_mA5A742CB26252926FAB1EFA932495D208292538F (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public GameObject IndicatorsPrefab => indicatorsPrefab;
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_0 = __this->get_indicatorsPrefab_5();
		return L_0;
	}
}
// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_MouseRotationSensitivity()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float MixedRealityInputSimulationProfile_get_MouseRotationSensitivity_mF845BEED2D0B763CE04CB80387448092D1E6A25D (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public float MouseRotationSensitivity => mouseRotationSensitivity;
		float L_0 = __this->get_mouseRotationSensitivity_6();
		return L_0;
	}
}
// System.String Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_MouseX()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* MixedRealityInputSimulationProfile_get_MouseX_m948C6DC5FA747EF496CF4D034559866E42208385 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public string MouseX => mouseX;
		String_t* L_0 = __this->get_mouseX_7();
		return L_0;
	}
}
// System.String Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_MouseY()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* MixedRealityInputSimulationProfile_get_MouseY_m09ED3578096DF704596D515302D86E585CFB5C02 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public string MouseY => mouseY;
		String_t* L_0 = __this->get_mouseY_8();
		return L_0;
	}
}
// System.String Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_MouseScroll()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* MixedRealityInputSimulationProfile_get_MouseScroll_m9A0148217817B75EC5AE9FE0640EBC70E379EB94 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public string MouseScroll => mouseScroll;
		String_t* L_0 = __this->get_mouseScroll_9();
		return L_0;
	}
}
// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_DoublePressTime()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float MixedRealityInputSimulationProfile_get_DoublePressTime_m6566FF721F580900057020119E5AA88F9875DFE8 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public float DoublePressTime => doublePressTime;
		float L_0 = __this->get_doublePressTime_10();
		return L_0;
	}
}
// System.Boolean Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_IsCameraControlEnabled()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool MixedRealityInputSimulationProfile_get_IsCameraControlEnabled_m4A126F4D0B5BACDD83B6D26532FE1A6ED1FBBC69 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public bool IsCameraControlEnabled => isCameraControlEnabled;
		bool L_0 = __this->get_isCameraControlEnabled_11();
		return L_0;
	}
}
// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_MouseLookSpeed()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float MixedRealityInputSimulationProfile_get_MouseLookSpeed_mBC42B1AA50D4CEFD639F5B691068731736766425 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public float MouseLookSpeed => mouseLookSpeed;
		float L_0 = __this->get_mouseLookSpeed_12();
		return L_0;
	}
}
// Microsoft.MixedReality.Toolkit.Input.KeyBinding Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_MouseLookButton()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  MixedRealityInputSimulationProfile_get_MouseLookButton_m802B81E2426D106617415BC82AB160F7C7C6C626 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public KeyBinding MouseLookButton => mouseLookButton;
		KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  L_0 = __this->get_mouseLookButton_13();
		return L_0;
	}
}
// System.Boolean Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_MouseLookToggle()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool MixedRealityInputSimulationProfile_get_MouseLookToggle_mAB5E86224A3E4712D165DB247C02B7C671F4B6E3 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public bool MouseLookToggle => mouseLookToggle;
		bool L_0 = __this->get_mouseLookToggle_14();
		return L_0;
	}
}
// System.Boolean Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_IsControllerLookInverted()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool MixedRealityInputSimulationProfile_get_IsControllerLookInverted_m1F1DDDC2A8E1523EAFCFA298669C7DB4F87AAC4D (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public bool IsControllerLookInverted => isControllerLookInverted;
		bool L_0 = __this->get_isControllerLookInverted_15();
		return L_0;
	}
}
// Microsoft.MixedReality.Toolkit.Input.InputSimulationControlMode Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_CurrentControlMode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t MixedRealityInputSimulationProfile_get_CurrentControlMode_m365A1934F1906A26E0865D061C759F74CD7F8BBB (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public InputSimulationControlMode CurrentControlMode => currentControlMode;
		int32_t L_0 = __this->get_currentControlMode_16();
		return L_0;
	}
}
// Microsoft.MixedReality.Toolkit.Input.KeyBinding Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_FastControlKey()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  MixedRealityInputSimulationProfile_get_FastControlKey_mC13B8B4DB11098D7D7B749712C486934A0AADCB1 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public KeyBinding FastControlKey => fastControlKey;
		KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  L_0 = __this->get_fastControlKey_17();
		return L_0;
	}
}
// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_ControlSlowSpeed()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float MixedRealityInputSimulationProfile_get_ControlSlowSpeed_m876415D8FBE9D75C3C183514450741EAF5F07A7B (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public float ControlSlowSpeed => controlSlowSpeed;
		float L_0 = __this->get_controlSlowSpeed_18();
		return L_0;
	}
}
// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_ControlFastSpeed()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float MixedRealityInputSimulationProfile_get_ControlFastSpeed_mFC4C7367E2EED39F2170D25353F140C624730606 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public float ControlFastSpeed => controlFastSpeed;
		float L_0 = __this->get_controlFastSpeed_19();
		return L_0;
	}
}
// System.String Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_MoveHorizontal()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* MixedRealityInputSimulationProfile_get_MoveHorizontal_m017554BDDC001B7EA768A8232E20D7458D583027 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public string MoveHorizontal => moveHorizontal;
		String_t* L_0 = __this->get_moveHorizontal_20();
		return L_0;
	}
}
// System.String Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_MoveVertical()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* MixedRealityInputSimulationProfile_get_MoveVertical_mD3951F2FDB8CEB301D4060806EC0CA6759896766 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public string MoveVertical => moveVertical;
		String_t* L_0 = __this->get_moveVertical_21();
		return L_0;
	}
}
// System.String Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_MoveUpDown()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* MixedRealityInputSimulationProfile_get_MoveUpDown_mC2CA55E02A0DEEFB179A7F78E529C0C2DAE2FC5C (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public string MoveUpDown => moveUpDown;
		String_t* L_0 = __this->get_moveUpDown_22();
		return L_0;
	}
}
// System.String Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_LookHorizontal()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* MixedRealityInputSimulationProfile_get_LookHorizontal_m42B3D6D256B7AAC814327C831B5D6AA1DF97E255 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public string LookHorizontal => lookHorizontal;
		String_t* L_0 = __this->get_lookHorizontal_23();
		return L_0;
	}
}
// System.String Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_LookVertical()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* MixedRealityInputSimulationProfile_get_LookVertical_m96BDB7F20C2470F9A83B319056DFAF7506DC42FE (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public string LookVertical => lookVertical;
		String_t* L_0 = __this->get_lookVertical_24();
		return L_0;
	}
}
// System.Boolean Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_SimulateEyePosition()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool MixedRealityInputSimulationProfile_get_SimulateEyePosition_m65F1A898A3FF7197331712C132AFF3C07F20848F (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public bool SimulateEyePosition => simulateEyePosition;
		bool L_0 = __this->get_simulateEyePosition_25();
		return L_0;
	}
}
// Microsoft.MixedReality.Toolkit.Input.HandSimulationMode Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_DefaultHandSimulationMode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t MixedRealityInputSimulationProfile_get_DefaultHandSimulationMode_mDFE400DEF2F624CDC95C751A868592551CF61601 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public HandSimulationMode DefaultHandSimulationMode => defaultHandSimulationMode;
		int32_t L_0 = __this->get_defaultHandSimulationMode_26();
		return L_0;
	}
}
// Microsoft.MixedReality.Toolkit.Input.KeyBinding Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_ToggleLeftHandKey()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  MixedRealityInputSimulationProfile_get_ToggleLeftHandKey_mCED4385DF475C0EE6B745179F093999CF83A6F0A (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public KeyBinding ToggleLeftHandKey => toggleLeftHandKey;
		KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  L_0 = __this->get_toggleLeftHandKey_27();
		return L_0;
	}
}
// Microsoft.MixedReality.Toolkit.Input.KeyBinding Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_ToggleRightHandKey()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  MixedRealityInputSimulationProfile_get_ToggleRightHandKey_mC2F372901568ED10228BEA9DB5DE15EC2ED0E82F (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public KeyBinding ToggleRightHandKey => toggleRightHandKey;
		KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  L_0 = __this->get_toggleRightHandKey_28();
		return L_0;
	}
}
// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_HandHideTimeout()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float MixedRealityInputSimulationProfile_get_HandHideTimeout_m68A2277521E6F5AB774F6C2B8C33308995704297 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public float HandHideTimeout => handHideTimeout;
		float L_0 = __this->get_handHideTimeout_29();
		return L_0;
	}
}
// Microsoft.MixedReality.Toolkit.Input.KeyBinding Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_LeftHandManipulationKey()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  MixedRealityInputSimulationProfile_get_LeftHandManipulationKey_mE0EF314F863B3E08284121956734B6B138628D6F (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public KeyBinding LeftHandManipulationKey => leftHandManipulationKey;
		KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  L_0 = __this->get_leftHandManipulationKey_30();
		return L_0;
	}
}
// Microsoft.MixedReality.Toolkit.Input.KeyBinding Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_RightHandManipulationKey()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  MixedRealityInputSimulationProfile_get_RightHandManipulationKey_m7F7C231E5A0A9089713A3886E183009A0AA750EC (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public KeyBinding RightHandManipulationKey => rightHandManipulationKey;
		KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  L_0 = __this->get_rightHandManipulationKey_31();
		return L_0;
	}
}
// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_MouseHandRotationSpeed()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float MixedRealityInputSimulationProfile_get_MouseHandRotationSpeed_m8C46C71287BF633CA982806BDBBCD2921912AA92 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public float MouseHandRotationSpeed => mouseHandRotationSpeed;
		float L_0 = __this->get_mouseHandRotationSpeed_32();
		return L_0;
	}
}
// Microsoft.MixedReality.Toolkit.Input.KeyBinding Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_HandRotateButton()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  MixedRealityInputSimulationProfile_get_HandRotateButton_m1EDB8CCD2179F5E3DFED5D8468EDBB7EA1692B07 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public KeyBinding HandRotateButton => handRotateButton;
		KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  L_0 = __this->get_handRotateButton_33();
		return L_0;
	}
}
// Microsoft.MixedReality.Toolkit.Utilities.ArticulatedHandPose_GestureId Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_DefaultHandGesture()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t MixedRealityInputSimulationProfile_get_DefaultHandGesture_mD3A6A7B18EE78308C4EB5A9E34D9E724D214923E (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public ArticulatedHandPose.GestureId DefaultHandGesture => defaultHandGesture;
		int32_t L_0 = __this->get_defaultHandGesture_34();
		return L_0;
	}
}
// Microsoft.MixedReality.Toolkit.Utilities.ArticulatedHandPose_GestureId Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_LeftMouseHandGesture()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t MixedRealityInputSimulationProfile_get_LeftMouseHandGesture_mEBCDA2D66B78F65369F66DE1153FEC879F096A82 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public ArticulatedHandPose.GestureId LeftMouseHandGesture => leftMouseHandGesture;
		int32_t L_0 = __this->get_leftMouseHandGesture_35();
		return L_0;
	}
}
// Microsoft.MixedReality.Toolkit.Utilities.ArticulatedHandPose_GestureId Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_MiddleMouseHandGesture()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t MixedRealityInputSimulationProfile_get_MiddleMouseHandGesture_mD1F5A9F51BCF91EC6D5F0AB990FD0183F0F0EE0C (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public ArticulatedHandPose.GestureId MiddleMouseHandGesture => middleMouseHandGesture;
		int32_t L_0 = __this->get_middleMouseHandGesture_36();
		return L_0;
	}
}
// Microsoft.MixedReality.Toolkit.Utilities.ArticulatedHandPose_GestureId Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_RightMouseHandGesture()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t MixedRealityInputSimulationProfile_get_RightMouseHandGesture_m73169C1AC874B34BE0ED76158486C07403F82432 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public ArticulatedHandPose.GestureId RightMouseHandGesture => rightMouseHandGesture;
		int32_t L_0 = __this->get_rightMouseHandGesture_37();
		return L_0;
	}
}
// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_HandGestureAnimationSpeed()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float MixedRealityInputSimulationProfile_get_HandGestureAnimationSpeed_m159042CFADEA1948914A6AD9D52193E9179B0AF5 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public float HandGestureAnimationSpeed => handGestureAnimationSpeed;
		float L_0 = __this->get_handGestureAnimationSpeed_38();
		return L_0;
	}
}
// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_HoldStartDuration()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float MixedRealityInputSimulationProfile_get_HoldStartDuration_mBC1A3E5C22D4854356392379561E246374610007 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public float HoldStartDuration => holdStartDuration;
		float L_0 = __this->get_holdStartDuration_39();
		return L_0;
	}
}
// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_NavigationStartThreshold()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float MixedRealityInputSimulationProfile_get_NavigationStartThreshold_m30BD08DA409E73AE42567F6420EB5E92DC7981E4 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public float NavigationStartThreshold => navigationStartThreshold;
		float L_0 = __this->get_navigationStartThreshold_40();
		return L_0;
	}
}
// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_DefaultHandDistance()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float MixedRealityInputSimulationProfile_get_DefaultHandDistance_m3D175B58CA9EBA30092EA2A68D01EA4B94489C4A (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public float DefaultHandDistance => defaultHandDistance;
		float L_0 = __this->get_defaultHandDistance_41();
		return L_0;
	}
}
// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_HandDepthMultiplier()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float MixedRealityInputSimulationProfile_get_HandDepthMultiplier_m6604B44E146EA94CBD6109337F0190E999766DA8 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public float HandDepthMultiplier => handDepthMultiplier;
		float L_0 = __this->get_handDepthMultiplier_42();
		return L_0;
	}
}
// System.Single Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::get_HandJitterAmount()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float MixedRealityInputSimulationProfile_get_HandJitterAmount_mDDF19A0974AFB60C2E91D129017A3D0805143F60 (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public float HandJitterAmount => handJitterAmount;
		float L_0 = __this->get_handJitterAmount_43();
		return L_0;
	}
}
// System.Void Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void MixedRealityInputSimulationProfile__ctor_m9769DFD9BDD54BA2B6A190798622CEDC78EA2EEB (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (MixedRealityInputSimulationProfile__ctor_m9769DFD9BDD54BA2B6A190798622CEDC78EA2EEB_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// private float mouseRotationSensitivity = 0.1f;
		__this->set_mouseRotationSensitivity_6((0.1f));
		// private string mouseX = "Mouse X";
		__this->set_mouseX_7(_stringLiteral294D359ECE148A430F19981912277E5154CA19E0);
		// private string mouseY = "Mouse Y";
		__this->set_mouseY_8(_stringLiteral1E88AB05D76FF253F292B74866D32460BB3836E2);
		// private string mouseScroll = "Mouse ScrollWheel";
		__this->set_mouseScroll_9(_stringLiteral627A7387C8BDDC7ACFF00D342D3F799DC6C19A31);
		// private float doublePressTime = 0.4f;
		__this->set_doublePressTime_10((0.4f));
		// private bool isCameraControlEnabled = true;
		__this->set_isCameraControlEnabled_11((bool)1);
		// private float mouseLookSpeed = 3.0f;
		__this->set_mouseLookSpeed_12((3.0f));
		// private KeyBinding mouseLookButton = KeyBinding.FromMouseButton(KeyBinding.MouseButton.Right);
		IL2CPP_RUNTIME_CLASS_INIT(KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79_il2cpp_TypeInfo_var);
		KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  L_0 = KeyBinding_FromMouseButton_mC7479108FCC71C952AAB38A9526E2B82B71C8CD0(1, /*hidden argument*/NULL);
		__this->set_mouseLookButton_13(L_0);
		// private bool isControllerLookInverted = true;
		__this->set_isControllerLookInverted_15((bool)1);
		// private KeyBinding fastControlKey = KeyBinding.FromKey(KeyCode.RightControl);
		KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  L_1 = KeyBinding_FromKey_m4E6BB297D9741E6C9C1FD8CB946CB140C4FD1DE5(((int32_t)305), /*hidden argument*/NULL);
		__this->set_fastControlKey_17(L_1);
		// private float controlSlowSpeed = 0.1f;
		__this->set_controlSlowSpeed_18((0.1f));
		// private float controlFastSpeed = 1.0f;
		__this->set_controlFastSpeed_19((1.0f));
		// private string moveHorizontal = "Horizontal";
		__this->set_moveHorizontal_20(_stringLiteral4F57A1CE99E68A7B05C42D0A7EA0070EAFABD31C);
		// private string moveVertical = "Vertical";
		__this->set_moveVertical_21(_stringLiteral4B937CC841D82F8936CEF1EFB88708AB5B0F1EE5);
		// private string moveUpDown = "UpDown";
		__this->set_moveUpDown_22(_stringLiteral2FEED76F1368917E9E5273B5D3B77EC607649D4D);
		// private string lookHorizontal = ControllerMappingLibrary.AXIS_4;
		__this->set_lookHorizontal_23(_stringLiteral8B7970623A806CC748C1B218861BE920B011B98C);
		// private string lookVertical = ControllerMappingLibrary.AXIS_5;
		__this->set_lookVertical_24(_stringLiteral04734178D407F1573AAACEB7E086B11BCFABD7FF);
		// private HandSimulationMode defaultHandSimulationMode = HandSimulationMode.Articulated;
		__this->set_defaultHandSimulationMode_26(2);
		// private KeyBinding toggleLeftHandKey = KeyBinding.FromKey(KeyCode.T);
		KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  L_2 = KeyBinding_FromKey_m4E6BB297D9741E6C9C1FD8CB946CB140C4FD1DE5(((int32_t)116), /*hidden argument*/NULL);
		__this->set_toggleLeftHandKey_27(L_2);
		// private KeyBinding toggleRightHandKey = KeyBinding.FromKey(KeyCode.Y);
		KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  L_3 = KeyBinding_FromKey_m4E6BB297D9741E6C9C1FD8CB946CB140C4FD1DE5(((int32_t)121), /*hidden argument*/NULL);
		__this->set_toggleRightHandKey_28(L_3);
		// private float handHideTimeout = 0.2f;
		__this->set_handHideTimeout_29((0.2f));
		// private KeyBinding leftHandManipulationKey = KeyBinding.FromKey(KeyCode.LeftShift);
		KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  L_4 = KeyBinding_FromKey_m4E6BB297D9741E6C9C1FD8CB946CB140C4FD1DE5(((int32_t)304), /*hidden argument*/NULL);
		__this->set_leftHandManipulationKey_30(L_4);
		// private KeyBinding rightHandManipulationKey = KeyBinding.FromKey(KeyCode.Space);
		KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  L_5 = KeyBinding_FromKey_m4E6BB297D9741E6C9C1FD8CB946CB140C4FD1DE5(((int32_t)32), /*hidden argument*/NULL);
		__this->set_rightHandManipulationKey_31(L_5);
		// private float mouseHandRotationSpeed = 6.0f;
		__this->set_mouseHandRotationSpeed_32((6.0f));
		// private KeyBinding handRotateButton = KeyBinding.FromKey(KeyCode.LeftControl);
		KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79  L_6 = KeyBinding_FromKey_m4E6BB297D9741E6C9C1FD8CB946CB140C4FD1DE5(((int32_t)306), /*hidden argument*/NULL);
		__this->set_handRotateButton_33(L_6);
		// private ArticulatedHandPose.GestureId defaultHandGesture = ArticulatedHandPose.GestureId.Open;
		__this->set_defaultHandGesture_34(2);
		// private ArticulatedHandPose.GestureId leftMouseHandGesture = ArticulatedHandPose.GestureId.Pinch;
		__this->set_leftMouseHandGesture_35(3);
		// private float handGestureAnimationSpeed = 8.0f;
		__this->set_handGestureAnimationSpeed_38((8.0f));
		// private float holdStartDuration = 0.5f;
		__this->set_holdStartDuration_39((0.5f));
		// private float navigationStartThreshold = 0.03f;
		__this->set_navigationStartThreshold_40((0.03f));
		// private float defaultHandDistance = 0.5f;
		__this->set_defaultHandDistance_41((0.5f));
		// private float handDepthMultiplier = 0.03f;
		__this->set_handDepthMultiplier_42((0.03f));
		BaseMixedRealityProfile__ctor_mC73E9360DB114F72FBC08703A0A9ABA78168B78A(__this, /*hidden argument*/NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Microsoft.MixedReality.Toolkit.Input.HandSimulationMode Microsoft.MixedReality.Toolkit.Input.SimulatedArticulatedHand::get_SimulationMode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t SimulatedArticulatedHand_get_SimulationMode_mF859372BC1E7EBA1FB344F3693308E1720483F94 (SimulatedArticulatedHand_tE70788F371CF5A48A99B3DE695FFA7A0FEF6E2E9 * __this, const RuntimeMethod* method)
{
	{
		// public override HandSimulationMode SimulationMode => HandSimulationMode.Articulated;
		return (int32_t)(2);
	}
}
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedArticulatedHand::.ctor(Microsoft.MixedReality.Toolkit.TrackingState,Microsoft.MixedReality.Toolkit.Utilities.Handedness,Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource,Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedArticulatedHand__ctor_m5518A9A451EE08DB313A88F7EDF1FCF72BFD5333 (SimulatedArticulatedHand_tE70788F371CF5A48A99B3DE695FFA7A0FEF6E2E9 * __this, int32_t ___trackingState0, uint8_t ___controllerHandedness1, RuntimeObject* ___inputSource2, MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* ___interactions3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedArticulatedHand__ctor_m5518A9A451EE08DB313A88F7EDF1FCF72BFD5333_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// private Vector3 currentPointerPosition = Vector3.zero;
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_0 = Vector3_get_zero_m3CDDCAE94581DF3BB16C4B40A100E28E9C6649C2(/*hidden argument*/NULL);
		__this->set_currentPointerPosition_25(L_0);
		// private Quaternion currentPointerRotation = Quaternion.identity;
		IL2CPP_RUNTIME_CLASS_INIT(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_il2cpp_TypeInfo_var);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_1 = Quaternion_get_identity_m548B37D80F2DEE60E41D1F09BF6889B557BE1A64(/*hidden argument*/NULL);
		__this->set_currentPointerRotation_26(L_1);
		// private MixedRealityPose lastPointerPose = MixedRealityPose.ZeroIdentity;
		IL2CPP_RUNTIME_CLASS_INIT(MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45_il2cpp_TypeInfo_var);
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_2 = MixedRealityPose_get_ZeroIdentity_m80C016329EAADDC4EB8DFD80ED0CF614A5E547AD_inline(/*hidden argument*/NULL);
		__this->set_lastPointerPose_27(L_2);
		// private MixedRealityPose currentPointerPose = MixedRealityPose.ZeroIdentity;
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_3 = MixedRealityPose_get_ZeroIdentity_m80C016329EAADDC4EB8DFD80ED0CF614A5E547AD_inline(/*hidden argument*/NULL);
		__this->set_currentPointerPose_28(L_3);
		// private MixedRealityPose currentIndexPose = MixedRealityPose.ZeroIdentity;
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_4 = MixedRealityPose_get_ZeroIdentity_m80C016329EAADDC4EB8DFD80ED0CF614A5E547AD_inline(/*hidden argument*/NULL);
		__this->set_currentIndexPose_29(L_4);
		// private MixedRealityPose currentGripPose = MixedRealityPose.ZeroIdentity;
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_5 = MixedRealityPose_get_ZeroIdentity_m80C016329EAADDC4EB8DFD80ED0CF614A5E547AD_inline(/*hidden argument*/NULL);
		__this->set_currentGripPose_30(L_5);
		// private MixedRealityPose lastGripPose = MixedRealityPose.ZeroIdentity;
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_6 = MixedRealityPose_get_ZeroIdentity_m80C016329EAADDC4EB8DFD80ED0CF614A5E547AD_inline(/*hidden argument*/NULL);
		__this->set_lastGripPose_31(L_6);
		// : base(trackingState, controllerHandedness, inputSource, interactions)
		int32_t L_7 = ___trackingState0;
		uint8_t L_8 = ___controllerHandedness1;
		RuntimeObject* L_9 = ___inputSource2;
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_10 = ___interactions3;
		IL2CPP_RUNTIME_CLASS_INIT(SimulatedHand_tFBAB6AD39E9B16E093E63E4D2A88EA5E3415437E_il2cpp_TypeInfo_var);
		SimulatedHand__ctor_m93808D1348F3FB6FA63A335E89F47FB5345EE1C4(__this, L_7, L_8, L_9, L_10, /*hidden argument*/NULL);
		// }
		return;
	}
}
// Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping[] Microsoft.MixedReality.Toolkit.Input.SimulatedArticulatedHand::get_DefaultInteractions()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* SimulatedArticulatedHand_get_DefaultInteractions_mDE48166990BF99C0D3809DD299CDCC0FC06777B4 (SimulatedArticulatedHand_tE70788F371CF5A48A99B3DE695FFA7A0FEF6E2E9 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedArticulatedHand_get_DefaultInteractions_mDE48166990BF99C0D3809DD299CDCC0FC06777B4_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
		// {
		//     new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
		//     new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip, MixedRealityInputAction.None),
		//     new MixedRealityInteractionMapping(2, "Select", AxisType.Digital, DeviceInputType.Select, MixedRealityInputAction.None),
		//     new MixedRealityInteractionMapping(3, "Grab", AxisType.SingleAxis, DeviceInputType.TriggerPress, MixedRealityInputAction.None),
		//     new MixedRealityInteractionMapping(4, "Index Finger Pose", AxisType.SixDof, DeviceInputType.IndexFinger, MixedRealityInputAction.None),
		// };
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_0 = (MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA*)(MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA*)SZArrayNew(MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA_il2cpp_TypeInfo_var, (uint32_t)5);
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_1 = L_0;
		IL2CPP_RUNTIME_CLASS_INIT(MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073_il2cpp_TypeInfo_var);
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_2 = MixedRealityInputAction_get_None_m0276CF8988B0670DCCE381865DD5190010A2A8BF_inline(/*hidden argument*/NULL);
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_3 = (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 *)il2cpp_codegen_object_new(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2_il2cpp_TypeInfo_var);
		MixedRealityInteractionMapping__ctor_m42FA7B2EF2BAA3804530651DFDF1145EEECE437F(L_3, 0, _stringLiteral0F9D13B1C31A5F4C68D0EEA587D21588F757084E, 7, 3, L_2, 0, _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709, _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709, (bool)0, (bool)0, /*hidden argument*/NULL);
		NullCheck(L_1);
		ArrayElementTypeCheck (L_1, L_3);
		(L_1)->SetAt(static_cast<il2cpp_array_size_t>(0), (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 *)L_3);
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_4 = L_1;
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_5 = MixedRealityInputAction_get_None_m0276CF8988B0670DCCE381865DD5190010A2A8BF_inline(/*hidden argument*/NULL);
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_6 = (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 *)il2cpp_codegen_object_new(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2_il2cpp_TypeInfo_var);
		MixedRealityInteractionMapping__ctor_m42FA7B2EF2BAA3804530651DFDF1145EEECE437F(L_6, 1, _stringLiteralE705DD1D38D6989FA3B3CCE68EC8B3C54B31ECFC, 7, ((int32_t)14), L_5, 0, _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709, _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709, (bool)0, (bool)0, /*hidden argument*/NULL);
		NullCheck(L_4);
		ArrayElementTypeCheck (L_4, L_6);
		(L_4)->SetAt(static_cast<il2cpp_array_size_t>(1), (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 *)L_6);
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_7 = L_4;
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_8 = MixedRealityInputAction_get_None_m0276CF8988B0670DCCE381865DD5190010A2A8BF_inline(/*hidden argument*/NULL);
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_9 = (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 *)il2cpp_codegen_object_new(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2_il2cpp_TypeInfo_var);
		MixedRealityInteractionMapping__ctor_m42FA7B2EF2BAA3804530651DFDF1145EEECE437F(L_9, 2, _stringLiteral8598222918D3C6E513D63060CF55E2971DED729A, 2, ((int32_t)25), L_8, 0, _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709, _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709, (bool)0, (bool)0, /*hidden argument*/NULL);
		NullCheck(L_7);
		ArrayElementTypeCheck (L_7, L_9);
		(L_7)->SetAt(static_cast<il2cpp_array_size_t>(2), (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 *)L_9);
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_10 = L_7;
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_11 = MixedRealityInputAction_get_None_m0276CF8988B0670DCCE381865DD5190010A2A8BF_inline(/*hidden argument*/NULL);
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_12 = (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 *)il2cpp_codegen_object_new(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2_il2cpp_TypeInfo_var);
		MixedRealityInteractionMapping__ctor_m42FA7B2EF2BAA3804530651DFDF1145EEECE437F(L_12, 3, _stringLiteralCF673A9C875D20DCDA8A5C0D7A2E5C60A940DB8E, 3, ((int32_t)13), L_11, 0, _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709, _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709, (bool)0, (bool)0, /*hidden argument*/NULL);
		NullCheck(L_10);
		ArrayElementTypeCheck (L_10, L_12);
		(L_10)->SetAt(static_cast<il2cpp_array_size_t>(3), (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 *)L_12);
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_13 = L_10;
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_14 = MixedRealityInputAction_get_None_m0276CF8988B0670DCCE381865DD5190010A2A8BF_inline(/*hidden argument*/NULL);
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_15 = (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 *)il2cpp_codegen_object_new(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2_il2cpp_TypeInfo_var);
		MixedRealityInteractionMapping__ctor_m42FA7B2EF2BAA3804530651DFDF1145EEECE437F(L_15, 4, _stringLiteral561DDB78EA3339033D719AFAA6980160DC8D88CB, 7, ((int32_t)33), L_14, 0, _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709, _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709, (bool)0, (bool)0, /*hidden argument*/NULL);
		NullCheck(L_13);
		ArrayElementTypeCheck (L_13, L_15);
		(L_13)->SetAt(static_cast<il2cpp_array_size_t>(4), (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 *)L_15);
		return L_13;
	}
}
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedArticulatedHand::SetupDefaultInteractions(Microsoft.MixedReality.Toolkit.Utilities.Handedness)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedArticulatedHand_SetupDefaultInteractions_m9F9F05A361C810DB38582F466CE28CAD1A4049F5 (SimulatedArticulatedHand_tE70788F371CF5A48A99B3DE695FFA7A0FEF6E2E9 * __this, uint8_t ___controllerHandedness0, const RuntimeMethod* method)
{
	{
		// AssignControllerMappings(DefaultInteractions);
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_0 = VirtFuncInvoker0< MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* >::Invoke(17 /* Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping[] Microsoft.MixedReality.Toolkit.Input.BaseController::get_DefaultInteractions() */, __this);
		BaseController_AssignControllerMappings_mB58538C7085760171304343CFBD77E5D8F230054(__this, L_0, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedArticulatedHand::UpdateInteractions(Microsoft.MixedReality.Toolkit.Input.SimulatedHandData)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedArticulatedHand_UpdateInteractions_m982D348EDBBB3D148D95B9F7E4BF863AFB851DA9 (SimulatedArticulatedHand_tE70788F371CF5A48A99B3DE695FFA7A0FEF6E2E9 * __this, SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * ___handData0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedArticulatedHand_UpdateInteractions_m982D348EDBBB3D148D95B9F7E4BF863AFB851DA9_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_0;
	memset((&V_0), 0, sizeof(V_0));
	MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  V_1;
	memset((&V_1), 0, sizeof(V_1));
	bool V_2 = false;
	Ray_tE2163D4CB3E6B267E29F8ABE41684490E4A614B2  V_3;
	memset((&V_3), 0, sizeof(V_3));
	int32_t V_4 = 0;
	int32_t V_5 = 0;
	Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB  V_6;
	memset((&V_6), 0, sizeof(V_6));
	Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB  V_7;
	memset((&V_7), 0, sizeof(V_7));
	RuntimeObject* G_B7_0 = NULL;
	RuntimeObject* G_B6_0 = NULL;
	RuntimeObject* G_B12_0 = NULL;
	RuntimeObject* G_B11_0 = NULL;
	RuntimeObject* G_B17_0 = NULL;
	RuntimeObject* G_B16_0 = NULL;
	RuntimeObject* G_B30_0 = NULL;
	RuntimeObject* G_B29_0 = NULL;
	RuntimeObject* G_B34_0 = NULL;
	RuntimeObject* G_B33_0 = NULL;
	RuntimeObject* G_B39_0 = NULL;
	RuntimeObject* G_B38_0 = NULL;
	RuntimeObject* G_B42_0 = NULL;
	RuntimeObject* G_B41_0 = NULL;
	RuntimeObject* G_B47_0 = NULL;
	RuntimeObject* G_B46_0 = NULL;
	RuntimeObject* G_B50_0 = NULL;
	RuntimeObject* G_B49_0 = NULL;
	RuntimeObject* G_B54_0 = NULL;
	RuntimeObject* G_B53_0 = NULL;
	MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* G_B58_0 = NULL;
	int32_t G_B58_1 = 0;
	MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* G_B57_0 = NULL;
	int32_t G_B57_1 = 0;
	Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB  G_B59_0;
	memset((&G_B59_0), 0, sizeof(G_B59_0));
	int32_t G_B59_1 = 0;
	{
		// lastPointerPose = currentPointerPose;
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_0 = __this->get_currentPointerPose_28();
		__this->set_lastPointerPose_27(L_0);
		// lastGripPose = currentGripPose;
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_1 = __this->get_currentGripPose_30();
		__this->set_lastGripPose_31(L_1);
		// Vector3 pointerPosition = jointPoses[TrackedHandJoint.IndexTip].Position;
		Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 * L_2 = ((SimulatedHand_tFBAB6AD39E9B16E093E63E4D2A88EA5E3415437E *)__this)->get_jointPoses_24();
		NullCheck(L_2);
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_3 = Dictionary_2_get_Item_mAA87FA69922BAF6733C05E34A765031668FCABA6(L_2, ((int32_t)11), /*hidden argument*/Dictionary_2_get_Item_mAA87FA69922BAF6733C05E34A765031668FCABA6_RuntimeMethod_var);
		V_1 = L_3;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_4 = MixedRealityPose_get_Position_mF175BAE3270E5432E605BDD5FD1FA5F722B24AEE_inline((MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 *)(&V_1), /*hidden argument*/NULL);
		V_0 = L_4;
		// IsPositionAvailable = IsRotationAvailable = pointerPosition != Vector3.zero;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_5 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_6 = Vector3_get_zero_m3CDDCAE94581DF3BB16C4B40A100E28E9C6649C2(/*hidden argument*/NULL);
		bool L_7 = Vector3_op_Inequality_mFEEAA4C4BF743FB5B8A47FF4967A5E2C73273D6E(L_5, L_6, /*hidden argument*/NULL);
		bool L_8 = L_7;
		V_2 = L_8;
		BaseController_set_IsRotationAvailable_m5259A799822AFD94A2BEE4B47F887A03158FE308_inline(__this, L_8, /*hidden argument*/NULL);
		bool L_9 = V_2;
		BaseController_set_IsPositionAvailable_m76D7FB5DBF945174A9D9B7A19123783742C6B57F_inline(__this, L_9, /*hidden argument*/NULL);
		// if (IsPositionAvailable)
		bool L_10 = BaseController_get_IsPositionAvailable_m3E2EB0D15AAADABB3D967535353AD53539677046_inline(__this, /*hidden argument*/NULL);
		if (!L_10)
		{
			goto IL_00cc;
		}
	}
	{
		// HandRay.Update(pointerPosition, GetPalmNormal(), CameraCache.Main.transform, ControllerHandedness);
		HandRay_t9DAE3FE243DBED1BAA1B9A4F782C3F1C9E6AE285 * L_11 = BaseHand_get_HandRay_mDB7145BE29023110AF5EC4037ABE75660776680F_inline(__this, /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_12 = V_0;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_13 = BaseHand_GetPalmNormal_mB5FF6D007531A6DD4C3E7632AF60DD2C586AA76B(__this, /*hidden argument*/NULL);
		Camera_t48B2B9ECB3CE6108A98BF949A1CECF0FE3421F34 * L_14 = CameraCache_get_Main_m23FB3162F6476988FEE59F829DEAF08702D81554(/*hidden argument*/NULL);
		NullCheck(L_14);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_15 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(L_14, /*hidden argument*/NULL);
		uint8_t L_16 = BaseController_get_ControllerHandedness_mA18814111E1328E1C7C04C383CC44E8A2F8A995A_inline(__this, /*hidden argument*/NULL);
		NullCheck(L_11);
		HandRay_Update_m2C7628B2A0B6F1EE9C20DE0E38CDD4854F70F149(L_11, L_12, L_13, L_15, L_16, /*hidden argument*/NULL);
		// Ray ray = HandRay.Ray;
		HandRay_t9DAE3FE243DBED1BAA1B9A4F782C3F1C9E6AE285 * L_17 = BaseHand_get_HandRay_mDB7145BE29023110AF5EC4037ABE75660776680F_inline(__this, /*hidden argument*/NULL);
		NullCheck(L_17);
		Ray_tE2163D4CB3E6B267E29F8ABE41684490E4A614B2  L_18 = HandRay_get_Ray_mA5DDBC5EF46D813F75A3728882AE72F8A779C189(L_17, /*hidden argument*/NULL);
		V_3 = L_18;
		// currentPointerPose.Position = ray.origin;
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * L_19 = __this->get_address_of_currentPointerPose_28();
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_20 = Ray_get_origin_m3773CA7B1E2F26F6F1447652B485D86C0BEC5187((Ray_tE2163D4CB3E6B267E29F8ABE41684490E4A614B2 *)(&V_3), /*hidden argument*/NULL);
		MixedRealityPose_set_Position_m28EBD523337BC95684EFC016980F3862DE763759_inline((MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 *)L_19, L_20, /*hidden argument*/NULL);
		// currentPointerPose.Rotation = Quaternion.LookRotation(ray.direction);
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * L_21 = __this->get_address_of_currentPointerPose_28();
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_22 = Ray_get_direction_m9E6468CD87844B437FC4B93491E63D388322F76E((Ray_tE2163D4CB3E6B267E29F8ABE41684490E4A614B2 *)(&V_3), /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_il2cpp_TypeInfo_var);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_23 = Quaternion_LookRotation_m465C08262650385D02ADDE78C9791AED47D2155F(L_22, /*hidden argument*/NULL);
		MixedRealityPose_set_Rotation_m1AC620BE37B8F415170D725902EE1C3A92ECC19B_inline((MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 *)L_21, L_23, /*hidden argument*/NULL);
		// currentGripPose = jointPoses[TrackedHandJoint.Palm];
		Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 * L_24 = ((SimulatedHand_tFBAB6AD39E9B16E093E63E4D2A88EA5E3415437E *)__this)->get_jointPoses_24();
		NullCheck(L_24);
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_25 = Dictionary_2_get_Item_mAA87FA69922BAF6733C05E34A765031668FCABA6(L_24, 2, /*hidden argument*/Dictionary_2_get_Item_mAA87FA69922BAF6733C05E34A765031668FCABA6_RuntimeMethod_var);
		__this->set_currentGripPose_30(L_25);
		// currentIndexPose = jointPoses[TrackedHandJoint.IndexTip];
		Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 * L_26 = ((SimulatedHand_tFBAB6AD39E9B16E093E63E4D2A88EA5E3415437E *)__this)->get_jointPoses_24();
		NullCheck(L_26);
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_27 = Dictionary_2_get_Item_mAA87FA69922BAF6733C05E34A765031668FCABA6(L_26, ((int32_t)11), /*hidden argument*/Dictionary_2_get_Item_mAA87FA69922BAF6733C05E34A765031668FCABA6_RuntimeMethod_var);
		__this->set_currentIndexPose_29(L_27);
	}

IL_00cc:
	{
		// if (lastGripPose != currentGripPose)
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_28 = __this->get_lastGripPose_31();
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_29 = __this->get_currentGripPose_30();
		IL2CPP_RUNTIME_CLASS_INIT(MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45_il2cpp_TypeInfo_var);
		bool L_30 = MixedRealityPose_op_Inequality_m85FF483B646A63C06AE543020D4F85257046AB3D(L_28, L_29, /*hidden argument*/NULL);
		if (!L_30)
		{
			goto IL_0170;
		}
	}
	{
		// if (IsPositionAvailable && IsRotationAvailable)
		bool L_31 = BaseController_get_IsPositionAvailable_m3E2EB0D15AAADABB3D967535353AD53539677046_inline(__this, /*hidden argument*/NULL);
		if (!L_31)
		{
			goto IL_0112;
		}
	}
	{
		bool L_32 = BaseController_get_IsRotationAvailable_m59D5E1DD267C83A3DB834096028590522C934868_inline(__this, /*hidden argument*/NULL);
		if (!L_32)
		{
			goto IL_0112;
		}
	}
	{
		// InputSystem?.RaiseSourcePoseChanged(InputSource, this, currentGripPose);
		RuntimeObject* L_33 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_34 = L_33;
		G_B6_0 = L_34;
		if (L_34)
		{
			G_B7_0 = L_34;
			goto IL_00fe;
		}
	}
	{
		goto IL_0170;
	}

IL_00fe:
	{
		RuntimeObject* L_35 = BaseController_get_InputSource_m9F9D70F24AC4D5605665D31F6D8A6083A3CA1CFD_inline(__this, /*hidden argument*/NULL);
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_36 = __this->get_currentGripPose_30();
		NullCheck(G_B7_0);
		InterfaceActionInvoker3< RuntimeObject*, RuntimeObject*, MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  >::Invoke(29 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaiseSourcePoseChanged(Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource,Microsoft.MixedReality.Toolkit.Input.IMixedRealityController,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B7_0, L_35, __this, L_36);
		// }
		goto IL_0170;
	}

IL_0112:
	{
		// else if (IsPositionAvailable && !IsRotationAvailable)
		bool L_37 = BaseController_get_IsPositionAvailable_m3E2EB0D15AAADABB3D967535353AD53539677046_inline(__this, /*hidden argument*/NULL);
		if (!L_37)
		{
			goto IL_0142;
		}
	}
	{
		bool L_38 = BaseController_get_IsRotationAvailable_m59D5E1DD267C83A3DB834096028590522C934868_inline(__this, /*hidden argument*/NULL);
		if (L_38)
		{
			goto IL_0142;
		}
	}
	{
		// InputSystem?.RaiseSourcePositionChanged(InputSource, this, currentPointerPosition);
		RuntimeObject* L_39 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_40 = L_39;
		G_B11_0 = L_40;
		if (L_40)
		{
			G_B12_0 = L_40;
			goto IL_012e;
		}
	}
	{
		goto IL_0170;
	}

IL_012e:
	{
		RuntimeObject* L_41 = BaseController_get_InputSource_m9F9D70F24AC4D5605665D31F6D8A6083A3CA1CFD_inline(__this, /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_42 = __this->get_currentPointerPosition_25();
		NullCheck(G_B12_0);
		InterfaceActionInvoker3< RuntimeObject*, RuntimeObject*, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  >::Invoke(27 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaiseSourcePositionChanged(Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource,Microsoft.MixedReality.Toolkit.Input.IMixedRealityController,UnityEngine.Vector3) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B12_0, L_41, __this, L_42);
		// }
		goto IL_0170;
	}

IL_0142:
	{
		// else if (!IsPositionAvailable && IsRotationAvailable)
		bool L_43 = BaseController_get_IsPositionAvailable_m3E2EB0D15AAADABB3D967535353AD53539677046_inline(__this, /*hidden argument*/NULL);
		if (L_43)
		{
			goto IL_0170;
		}
	}
	{
		bool L_44 = BaseController_get_IsRotationAvailable_m59D5E1DD267C83A3DB834096028590522C934868_inline(__this, /*hidden argument*/NULL);
		if (!L_44)
		{
			goto IL_0170;
		}
	}
	{
		// InputSystem?.RaiseSourceRotationChanged(InputSource, this, currentPointerRotation);
		RuntimeObject* L_45 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_46 = L_45;
		G_B16_0 = L_46;
		if (L_46)
		{
			G_B17_0 = L_46;
			goto IL_015e;
		}
	}
	{
		goto IL_0170;
	}

IL_015e:
	{
		RuntimeObject* L_47 = BaseController_get_InputSource_m9F9D70F24AC4D5605665D31F6D8A6083A3CA1CFD_inline(__this, /*hidden argument*/NULL);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_48 = __this->get_currentPointerRotation_26();
		NullCheck(G_B17_0);
		InterfaceActionInvoker3< RuntimeObject*, RuntimeObject*, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  >::Invoke(28 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaiseSourceRotationChanged(Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource,Microsoft.MixedReality.Toolkit.Input.IMixedRealityController,UnityEngine.Quaternion) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B17_0, L_47, __this, L_48);
	}

IL_0170:
	{
		// for (int i = 0; i < Interactions?.Length; i++)
		V_4 = 0;
		goto IL_040d;
	}

IL_0178:
	{
		// switch (Interactions[i].InputType)
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_49 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_50 = V_4;
		NullCheck(L_49);
		int32_t L_51 = L_50;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_52 = (L_49)->GetAt(static_cast<il2cpp_array_size_t>(L_51));
		NullCheck(L_52);
		int32_t L_53 = MixedRealityInteractionMapping_get_InputType_mA8C027545479C380F87D72BDED734A9BDBFA40CD_inline(L_52, /*hidden argument*/NULL);
		V_5 = L_53;
		int32_t L_54 = V_5;
		if ((((int32_t)L_54) > ((int32_t)((int32_t)13))))
		{
			goto IL_01a1;
		}
	}
	{
		int32_t L_55 = V_5;
		if ((((int32_t)L_55) == ((int32_t)3)))
		{
			goto IL_01be;
		}
	}
	{
		int32_t L_56 = V_5;
		if ((((int32_t)L_56) == ((int32_t)((int32_t)13))))
		{
			goto IL_031b;
		}
	}
	{
		goto IL_0407;
	}

IL_01a1:
	{
		int32_t L_57 = V_5;
		if ((((int32_t)L_57) == ((int32_t)((int32_t)14))))
		{
			goto IL_021e;
		}
	}
	{
		int32_t L_58 = V_5;
		if ((((int32_t)L_58) == ((int32_t)((int32_t)25))))
		{
			goto IL_027e;
		}
	}
	{
		int32_t L_59 = V_5;
		if ((((int32_t)L_59) == ((int32_t)((int32_t)33))))
		{
			goto IL_03b2;
		}
	}
	{
		goto IL_0407;
	}

IL_01be:
	{
		// Interactions[i].PoseData = currentPointerPose;
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_60 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_61 = V_4;
		NullCheck(L_60);
		int32_t L_62 = L_61;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_63 = (L_60)->GetAt(static_cast<il2cpp_array_size_t>(L_62));
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_64 = __this->get_currentPointerPose_28();
		NullCheck(L_63);
		MixedRealityInteractionMapping_set_PoseData_mED53A7137722CE84DD3F8144D83C6E2F6B844287(L_63, L_64, /*hidden argument*/NULL);
		// if (Interactions[i].Changed)
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_65 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_66 = V_4;
		NullCheck(L_65);
		int32_t L_67 = L_66;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_68 = (L_65)->GetAt(static_cast<il2cpp_array_size_t>(L_67));
		NullCheck(L_68);
		bool L_69 = MixedRealityInteractionMapping_get_Changed_m70D15D24BDB909A6AA0E9C4DB393DAA25F84983F(L_68, /*hidden argument*/NULL);
		if (!L_69)
		{
			goto IL_0407;
		}
	}
	{
		// InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, currentPointerPose);
		RuntimeObject* L_70 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_71 = L_70;
		G_B29_0 = L_71;
		if (L_71)
		{
			G_B30_0 = L_71;
			goto IL_01f4;
		}
	}
	{
		goto IL_0407;
	}

IL_01f4:
	{
		RuntimeObject* L_72 = BaseController_get_InputSource_m9F9D70F24AC4D5605665D31F6D8A6083A3CA1CFD_inline(__this, /*hidden argument*/NULL);
		uint8_t L_73 = BaseController_get_ControllerHandedness_mA18814111E1328E1C7C04C383CC44E8A2F8A995A_inline(__this, /*hidden argument*/NULL);
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_74 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_75 = V_4;
		NullCheck(L_74);
		int32_t L_76 = L_75;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_77 = (L_74)->GetAt(static_cast<il2cpp_array_size_t>(L_76));
		NullCheck(L_77);
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_78 = MixedRealityInteractionMapping_get_MixedRealityInputAction_mA22FF2AC6237AEF7B9EADF4461EB3B484CCB995E_inline(L_77, /*hidden argument*/NULL);
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_79 = __this->get_currentPointerPose_28();
		NullCheck(G_B30_0);
		InterfaceActionInvoker4< RuntimeObject*, uint8_t, MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073 , MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  >::Invoke(44 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaisePoseInputChanged(Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource,Microsoft.MixedReality.Toolkit.Utilities.Handedness,Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B30_0, L_72, L_73, L_78, L_79);
		// break;
		goto IL_0407;
	}

IL_021e:
	{
		// Interactions[i].PoseData = currentGripPose;
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_80 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_81 = V_4;
		NullCheck(L_80);
		int32_t L_82 = L_81;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_83 = (L_80)->GetAt(static_cast<il2cpp_array_size_t>(L_82));
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_84 = __this->get_currentGripPose_30();
		NullCheck(L_83);
		MixedRealityInteractionMapping_set_PoseData_mED53A7137722CE84DD3F8144D83C6E2F6B844287(L_83, L_84, /*hidden argument*/NULL);
		// if (Interactions[i].Changed)
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_85 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_86 = V_4;
		NullCheck(L_85);
		int32_t L_87 = L_86;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_88 = (L_85)->GetAt(static_cast<il2cpp_array_size_t>(L_87));
		NullCheck(L_88);
		bool L_89 = MixedRealityInteractionMapping_get_Changed_m70D15D24BDB909A6AA0E9C4DB393DAA25F84983F(L_88, /*hidden argument*/NULL);
		if (!L_89)
		{
			goto IL_0407;
		}
	}
	{
		// InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, currentGripPose);
		RuntimeObject* L_90 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_91 = L_90;
		G_B33_0 = L_91;
		if (L_91)
		{
			G_B34_0 = L_91;
			goto IL_0254;
		}
	}
	{
		goto IL_0407;
	}

IL_0254:
	{
		RuntimeObject* L_92 = BaseController_get_InputSource_m9F9D70F24AC4D5605665D31F6D8A6083A3CA1CFD_inline(__this, /*hidden argument*/NULL);
		uint8_t L_93 = BaseController_get_ControllerHandedness_mA18814111E1328E1C7C04C383CC44E8A2F8A995A_inline(__this, /*hidden argument*/NULL);
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_94 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_95 = V_4;
		NullCheck(L_94);
		int32_t L_96 = L_95;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_97 = (L_94)->GetAt(static_cast<il2cpp_array_size_t>(L_96));
		NullCheck(L_97);
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_98 = MixedRealityInteractionMapping_get_MixedRealityInputAction_mA22FF2AC6237AEF7B9EADF4461EB3B484CCB995E_inline(L_97, /*hidden argument*/NULL);
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_99 = __this->get_currentGripPose_30();
		NullCheck(G_B34_0);
		InterfaceActionInvoker4< RuntimeObject*, uint8_t, MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073 , MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  >::Invoke(44 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaisePoseInputChanged(Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource,Microsoft.MixedReality.Toolkit.Utilities.Handedness,Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B34_0, L_92, L_93, L_98, L_99);
		// break;
		goto IL_0407;
	}

IL_027e:
	{
		// Interactions[i].BoolData = handData.IsPinching;
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_100 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_101 = V_4;
		NullCheck(L_100);
		int32_t L_102 = L_101;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_103 = (L_100)->GetAt(static_cast<il2cpp_array_size_t>(L_102));
		SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * L_104 = ___handData0;
		NullCheck(L_104);
		bool L_105 = SimulatedHandData_get_IsPinching_mB7C40888399E88C93E755FE89D50234CF5F5C981_inline(L_104, /*hidden argument*/NULL);
		NullCheck(L_103);
		MixedRealityInteractionMapping_set_BoolData_mE86E7E665BCA02A2E69651A333993A51703F7D64(L_103, L_105, /*hidden argument*/NULL);
		// if (Interactions[i].Changed)
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_106 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_107 = V_4;
		NullCheck(L_106);
		int32_t L_108 = L_107;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_109 = (L_106)->GetAt(static_cast<il2cpp_array_size_t>(L_108));
		NullCheck(L_109);
		bool L_110 = MixedRealityInteractionMapping_get_Changed_m70D15D24BDB909A6AA0E9C4DB393DAA25F84983F(L_109, /*hidden argument*/NULL);
		if (!L_110)
		{
			goto IL_0407;
		}
	}
	{
		// if (Interactions[i].BoolData)
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_111 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_112 = V_4;
		NullCheck(L_111);
		int32_t L_113 = L_112;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_114 = (L_111)->GetAt(static_cast<il2cpp_array_size_t>(L_113));
		NullCheck(L_114);
		bool L_115 = MixedRealityInteractionMapping_get_BoolData_mB42A4C428B73C25DC7FE9CAC463325E19255F71B_inline(L_114, /*hidden argument*/NULL);
		if (!L_115)
		{
			goto IL_02e8;
		}
	}
	{
		// InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
		RuntimeObject* L_116 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_117 = L_116;
		G_B38_0 = L_117;
		if (L_117)
		{
			G_B39_0 = L_117;
			goto IL_02c4;
		}
	}
	{
		goto IL_0407;
	}

IL_02c4:
	{
		RuntimeObject* L_118 = BaseController_get_InputSource_m9F9D70F24AC4D5605665D31F6D8A6083A3CA1CFD_inline(__this, /*hidden argument*/NULL);
		uint8_t L_119 = BaseController_get_ControllerHandedness_mA18814111E1328E1C7C04C383CC44E8A2F8A995A_inline(__this, /*hidden argument*/NULL);
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_120 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_121 = V_4;
		NullCheck(L_120);
		int32_t L_122 = L_121;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_123 = (L_120)->GetAt(static_cast<il2cpp_array_size_t>(L_122));
		NullCheck(L_123);
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_124 = MixedRealityInteractionMapping_get_MixedRealityInputAction_mA22FF2AC6237AEF7B9EADF4461EB3B484CCB995E_inline(L_123, /*hidden argument*/NULL);
		NullCheck(G_B39_0);
		InterfaceActionInvoker3< RuntimeObject*, uint8_t, MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  >::Invoke(38 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaiseOnInputDown(Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource,Microsoft.MixedReality.Toolkit.Utilities.Handedness,Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B39_0, L_118, L_119, L_124);
		// }
		goto IL_0407;
	}

IL_02e8:
	{
		// InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
		RuntimeObject* L_125 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_126 = L_125;
		G_B41_0 = L_126;
		if (L_126)
		{
			G_B42_0 = L_126;
			goto IL_02f7;
		}
	}
	{
		goto IL_0407;
	}

IL_02f7:
	{
		RuntimeObject* L_127 = BaseController_get_InputSource_m9F9D70F24AC4D5605665D31F6D8A6083A3CA1CFD_inline(__this, /*hidden argument*/NULL);
		uint8_t L_128 = BaseController_get_ControllerHandedness_mA18814111E1328E1C7C04C383CC44E8A2F8A995A_inline(__this, /*hidden argument*/NULL);
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_129 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_130 = V_4;
		NullCheck(L_129);
		int32_t L_131 = L_130;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_132 = (L_129)->GetAt(static_cast<il2cpp_array_size_t>(L_131));
		NullCheck(L_132);
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_133 = MixedRealityInteractionMapping_get_MixedRealityInputAction_mA22FF2AC6237AEF7B9EADF4461EB3B484CCB995E_inline(L_132, /*hidden argument*/NULL);
		NullCheck(G_B42_0);
		InterfaceActionInvoker3< RuntimeObject*, uint8_t, MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  >::Invoke(39 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaiseOnInputUp(Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource,Microsoft.MixedReality.Toolkit.Utilities.Handedness,Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B42_0, L_127, L_128, L_133);
		// break;
		goto IL_0407;
	}

IL_031b:
	{
		// Interactions[i].BoolData = handData.IsPinching;
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_134 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_135 = V_4;
		NullCheck(L_134);
		int32_t L_136 = L_135;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_137 = (L_134)->GetAt(static_cast<il2cpp_array_size_t>(L_136));
		SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * L_138 = ___handData0;
		NullCheck(L_138);
		bool L_139 = SimulatedHandData_get_IsPinching_mB7C40888399E88C93E755FE89D50234CF5F5C981_inline(L_138, /*hidden argument*/NULL);
		NullCheck(L_137);
		MixedRealityInteractionMapping_set_BoolData_mE86E7E665BCA02A2E69651A333993A51703F7D64(L_137, L_139, /*hidden argument*/NULL);
		// if (Interactions[i].Changed)
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_140 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_141 = V_4;
		NullCheck(L_140);
		int32_t L_142 = L_141;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_143 = (L_140)->GetAt(static_cast<il2cpp_array_size_t>(L_142));
		NullCheck(L_143);
		bool L_144 = MixedRealityInteractionMapping_get_Changed_m70D15D24BDB909A6AA0E9C4DB393DAA25F84983F(L_143, /*hidden argument*/NULL);
		if (!L_144)
		{
			goto IL_0407;
		}
	}
	{
		// if (Interactions[i].BoolData)
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_145 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_146 = V_4;
		NullCheck(L_145);
		int32_t L_147 = L_146;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_148 = (L_145)->GetAt(static_cast<il2cpp_array_size_t>(L_147));
		NullCheck(L_148);
		bool L_149 = MixedRealityInteractionMapping_get_BoolData_mB42A4C428B73C25DC7FE9CAC463325E19255F71B_inline(L_148, /*hidden argument*/NULL);
		if (!L_149)
		{
			goto IL_0385;
		}
	}
	{
		// InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
		RuntimeObject* L_150 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_151 = L_150;
		G_B46_0 = L_151;
		if (L_151)
		{
			G_B47_0 = L_151;
			goto IL_0361;
		}
	}
	{
		goto IL_0407;
	}

IL_0361:
	{
		RuntimeObject* L_152 = BaseController_get_InputSource_m9F9D70F24AC4D5605665D31F6D8A6083A3CA1CFD_inline(__this, /*hidden argument*/NULL);
		uint8_t L_153 = BaseController_get_ControllerHandedness_mA18814111E1328E1C7C04C383CC44E8A2F8A995A_inline(__this, /*hidden argument*/NULL);
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_154 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_155 = V_4;
		NullCheck(L_154);
		int32_t L_156 = L_155;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_157 = (L_154)->GetAt(static_cast<il2cpp_array_size_t>(L_156));
		NullCheck(L_157);
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_158 = MixedRealityInteractionMapping_get_MixedRealityInputAction_mA22FF2AC6237AEF7B9EADF4461EB3B484CCB995E_inline(L_157, /*hidden argument*/NULL);
		NullCheck(G_B47_0);
		InterfaceActionInvoker3< RuntimeObject*, uint8_t, MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  >::Invoke(38 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaiseOnInputDown(Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource,Microsoft.MixedReality.Toolkit.Utilities.Handedness,Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B47_0, L_152, L_153, L_158);
		// }
		goto IL_0407;
	}

IL_0385:
	{
		// InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
		RuntimeObject* L_159 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_160 = L_159;
		G_B49_0 = L_160;
		if (L_160)
		{
			G_B50_0 = L_160;
			goto IL_0391;
		}
	}
	{
		goto IL_0407;
	}

IL_0391:
	{
		RuntimeObject* L_161 = BaseController_get_InputSource_m9F9D70F24AC4D5605665D31F6D8A6083A3CA1CFD_inline(__this, /*hidden argument*/NULL);
		uint8_t L_162 = BaseController_get_ControllerHandedness_mA18814111E1328E1C7C04C383CC44E8A2F8A995A_inline(__this, /*hidden argument*/NULL);
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_163 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_164 = V_4;
		NullCheck(L_163);
		int32_t L_165 = L_164;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_166 = (L_163)->GetAt(static_cast<il2cpp_array_size_t>(L_165));
		NullCheck(L_166);
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_167 = MixedRealityInteractionMapping_get_MixedRealityInputAction_mA22FF2AC6237AEF7B9EADF4461EB3B484CCB995E_inline(L_166, /*hidden argument*/NULL);
		NullCheck(G_B50_0);
		InterfaceActionInvoker3< RuntimeObject*, uint8_t, MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  >::Invoke(39 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaiseOnInputUp(Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource,Microsoft.MixedReality.Toolkit.Utilities.Handedness,Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B50_0, L_161, L_162, L_167);
		// break;
		goto IL_0407;
	}

IL_03b2:
	{
		// Interactions[i].PoseData = currentIndexPose;
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_168 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_169 = V_4;
		NullCheck(L_168);
		int32_t L_170 = L_169;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_171 = (L_168)->GetAt(static_cast<il2cpp_array_size_t>(L_170));
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_172 = __this->get_currentIndexPose_29();
		NullCheck(L_171);
		MixedRealityInteractionMapping_set_PoseData_mED53A7137722CE84DD3F8144D83C6E2F6B844287(L_171, L_172, /*hidden argument*/NULL);
		// if (Interactions[i].Changed)
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_173 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_174 = V_4;
		NullCheck(L_173);
		int32_t L_175 = L_174;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_176 = (L_173)->GetAt(static_cast<il2cpp_array_size_t>(L_175));
		NullCheck(L_176);
		bool L_177 = MixedRealityInteractionMapping_get_Changed_m70D15D24BDB909A6AA0E9C4DB393DAA25F84983F(L_176, /*hidden argument*/NULL);
		if (!L_177)
		{
			goto IL_0407;
		}
	}
	{
		// InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, currentIndexPose);
		RuntimeObject* L_178 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_179 = L_178;
		G_B53_0 = L_179;
		if (L_179)
		{
			G_B54_0 = L_179;
			goto IL_03e2;
		}
	}
	{
		goto IL_0407;
	}

IL_03e2:
	{
		RuntimeObject* L_180 = BaseController_get_InputSource_m9F9D70F24AC4D5605665D31F6D8A6083A3CA1CFD_inline(__this, /*hidden argument*/NULL);
		uint8_t L_181 = BaseController_get_ControllerHandedness_mA18814111E1328E1C7C04C383CC44E8A2F8A995A_inline(__this, /*hidden argument*/NULL);
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_182 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_183 = V_4;
		NullCheck(L_182);
		int32_t L_184 = L_183;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_185 = (L_182)->GetAt(static_cast<il2cpp_array_size_t>(L_184));
		NullCheck(L_185);
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_186 = MixedRealityInteractionMapping_get_MixedRealityInputAction_mA22FF2AC6237AEF7B9EADF4461EB3B484CCB995E_inline(L_185, /*hidden argument*/NULL);
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_187 = __this->get_currentIndexPose_29();
		NullCheck(G_B54_0);
		InterfaceActionInvoker4< RuntimeObject*, uint8_t, MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073 , MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  >::Invoke(44 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaisePoseInputChanged(Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource,Microsoft.MixedReality.Toolkit.Utilities.Handedness,Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B54_0, L_180, L_181, L_186, L_187);
	}

IL_0407:
	{
		// for (int i = 0; i < Interactions?.Length; i++)
		int32_t L_188 = V_4;
		V_4 = ((int32_t)il2cpp_codegen_add((int32_t)L_188, (int32_t)1));
	}

IL_040d:
	{
		// for (int i = 0; i < Interactions?.Length; i++)
		int32_t L_189 = V_4;
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_190 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_191 = L_190;
		G_B57_0 = L_191;
		G_B57_1 = L_189;
		if (L_191)
		{
			G_B58_0 = L_191;
			G_B58_1 = L_189;
			goto IL_0425;
		}
	}
	{
		il2cpp_codegen_initobj((&V_7), sizeof(Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB ));
		Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB  L_192 = V_7;
		G_B59_0 = L_192;
		G_B59_1 = G_B57_1;
		goto IL_042c;
	}

IL_0425:
	{
		NullCheck(G_B58_0);
		Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB  L_193;
		memset((&L_193), 0, sizeof(L_193));
		Nullable_1__ctor_m11F9C228CFDF836DDFCD7880C09CB4098AB9D7F2((&L_193), (((int32_t)((int32_t)(((RuntimeArray*)G_B58_0)->max_length)))), /*hidden argument*/Nullable_1__ctor_m11F9C228CFDF836DDFCD7880C09CB4098AB9D7F2_RuntimeMethod_var);
		G_B59_0 = L_193;
		G_B59_1 = G_B58_1;
	}

IL_042c:
	{
		V_6 = G_B59_0;
		int32_t L_194 = Nullable_1_GetValueOrDefault_mE89BB8F302DF31EE202251F4746859285860B6B6_inline((Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB *)(&V_6), /*hidden argument*/Nullable_1_GetValueOrDefault_mE89BB8F302DF31EE202251F4746859285860B6B6_RuntimeMethod_var);
		bool L_195 = Nullable_1_get_HasValue_mB664E2C41CADA8413EF8842E6601B8C696A7CE15_inline((Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB *)(&V_6), /*hidden argument*/Nullable_1_get_HasValue_mB664E2C41CADA8413EF8842E6601B8C696A7CE15_RuntimeMethod_var);
		if (((int32_t)((int32_t)((((int32_t)G_B59_1) < ((int32_t)L_194))? 1 : 0)&(int32_t)L_195)))
		{
			goto IL_0178;
		}
	}
	{
		// }
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Microsoft.MixedReality.Toolkit.Input.HandSimulationMode Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::get_SimulationMode()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t SimulatedGestureHand_get_SimulationMode_m3E7C9A35BFAA289E74524BCA142A3771A08B8ABB (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method)
{
	{
		// public override HandSimulationMode SimulationMode => HandSimulationMode.Gestures;
		return (int32_t)(1);
	}
}
// UnityEngine.Vector3 Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::get_navigationDelta()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  SimulatedGestureHand_get_navigationDelta_m0FD22233CFFA608F80B80E740D01DA6F8E22582A (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedGestureHand_get_navigationDelta_m0FD22233CFFA608F80B80E740D01DA6F8E22582A_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// private Vector3 navigationDelta => new Vector3(
		//     Mathf.Clamp(cumulativeDelta.x, -1.0f, 1.0f) * currentRailsUsed.x,
		//     Mathf.Clamp(cumulativeDelta.y, -1.0f, 1.0f) * currentRailsUsed.y,
		//     Mathf.Clamp(cumulativeDelta.z, -1.0f, 1.0f) * currentRailsUsed.z);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * L_0 = __this->get_address_of_cumulativeDelta_39();
		float L_1 = L_0->get_x_2();
		IL2CPP_RUNTIME_CLASS_INIT(Mathf_tFBDE6467D269BFE410605C7D806FD9991D4A89CB_il2cpp_TypeInfo_var);
		float L_2 = Mathf_Clamp_m033DD894F89E6DCCDAFC580091053059C86A4507(L_1, (-1.0f), (1.0f), /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * L_3 = __this->get_address_of_currentRailsUsed_37();
		float L_4 = L_3->get_x_2();
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * L_5 = __this->get_address_of_cumulativeDelta_39();
		float L_6 = L_5->get_y_3();
		float L_7 = Mathf_Clamp_m033DD894F89E6DCCDAFC580091053059C86A4507(L_6, (-1.0f), (1.0f), /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * L_8 = __this->get_address_of_currentRailsUsed_37();
		float L_9 = L_8->get_y_3();
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * L_10 = __this->get_address_of_cumulativeDelta_39();
		float L_11 = L_10->get_z_4();
		float L_12 = Mathf_Clamp_m033DD894F89E6DCCDAFC580091053059C86A4507(L_11, (-1.0f), (1.0f), /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * L_13 = __this->get_address_of_currentRailsUsed_37();
		float L_14 = L_13->get_z_4();
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_15;
		memset((&L_15), 0, sizeof(L_15));
		Vector3__ctor_m08F61F548AA5836D8789843ACB4A81E4963D2EE1((&L_15), ((float)il2cpp_codegen_multiply((float)L_2, (float)L_4)), ((float)il2cpp_codegen_multiply((float)L_7, (float)L_9)), ((float)il2cpp_codegen_multiply((float)L_12, (float)L_14)), /*hidden argument*/NULL);
		return L_15;
	}
}
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::.ctor(Microsoft.MixedReality.Toolkit.TrackingState,Microsoft.MixedReality.Toolkit.Utilities.Handedness,Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource,Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedGestureHand__ctor_m93581EB80551349B8F9FD7C292CBDBFA5243F97A (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, int32_t ___trackingState0, uint8_t ___controllerHandedness1, RuntimeObject* ___inputSource2, MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* ___interactions3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedGestureHand__ctor_m93581EB80551349B8F9FD7C292CBDBFA5243F97A_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// private MixedRealityInputAction holdAction = MixedRealityInputAction.None;
		IL2CPP_RUNTIME_CLASS_INIT(MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073_il2cpp_TypeInfo_var);
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_0 = MixedRealityInputAction_get_None_m0276CF8988B0670DCCE381865DD5190010A2A8BF_inline(/*hidden argument*/NULL);
		__this->set_holdAction_26(L_0);
		// private MixedRealityInputAction navigationAction = MixedRealityInputAction.None;
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_1 = MixedRealityInputAction_get_None_m0276CF8988B0670DCCE381865DD5190010A2A8BF_inline(/*hidden argument*/NULL);
		__this->set_navigationAction_27(L_1);
		// private MixedRealityInputAction manipulationAction = MixedRealityInputAction.None;
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_2 = MixedRealityInputAction_get_None_m0276CF8988B0670DCCE381865DD5190010A2A8BF_inline(/*hidden argument*/NULL);
		__this->set_manipulationAction_28(L_2);
		// private MixedRealityInputAction selectAction = MixedRealityInputAction.None;
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_3 = MixedRealityInputAction_get_None_m0276CF8988B0670DCCE381865DD5190010A2A8BF_inline(/*hidden argument*/NULL);
		__this->set_selectAction_29(L_3);
		// private Vector3 currentRailsUsed = Vector3.one;
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_4 = Vector3_get_one_mA11B83037CB269C6076CBCF754E24C8F3ACEC2AB(/*hidden argument*/NULL);
		__this->set_currentRailsUsed_37(L_4);
		// private Vector3 currentPosition = Vector3.zero;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_5 = Vector3_get_zero_m3CDDCAE94581DF3BB16C4B40A100E28E9C6649C2(/*hidden argument*/NULL);
		__this->set_currentPosition_38(L_5);
		// private Vector3 cumulativeDelta = Vector3.zero;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_6 = Vector3_get_zero_m3CDDCAE94581DF3BB16C4B40A100E28E9C6649C2(/*hidden argument*/NULL);
		__this->set_cumulativeDelta_39(L_6);
		// private MixedRealityPose currentGripPose = MixedRealityPose.ZeroIdentity;
		IL2CPP_RUNTIME_CLASS_INIT(MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45_il2cpp_TypeInfo_var);
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_7 = MixedRealityPose_get_ZeroIdentity_m80C016329EAADDC4EB8DFD80ED0CF614A5E547AD_inline(/*hidden argument*/NULL);
		__this->set_currentGripPose_40(L_7);
		// : base(trackingState, controllerHandedness, inputSource, interactions)
		int32_t L_8 = ___trackingState0;
		uint8_t L_9 = ___controllerHandedness1;
		RuntimeObject* L_10 = ___inputSource2;
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_11 = ___interactions3;
		IL2CPP_RUNTIME_CLASS_INIT(SimulatedHand_tFBAB6AD39E9B16E093E63E4D2A88EA5E3415437E_il2cpp_TypeInfo_var);
		SimulatedHand__ctor_m93808D1348F3FB6FA63A335E89F47FB5345EE1C4(__this, L_8, L_9, L_10, L_11, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::EnsureProfileSettings()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedGestureHand_EnsureProfileSettings_m5FC39BD038B64363C40173D9E60B1BC1606C7A3A (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedGestureHand_EnsureProfileSettings_m5FC39BD038B64363C40173D9E60B1BC1606C7A3A_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725 * V_0 = NULL;
	MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * V_1 = NULL;
	int32_t V_2 = 0;
	MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074  V_3;
	memset((&V_3), 0, sizeof(V_3));
	int32_t V_4 = 0;
	RuntimeObject* G_B4_0 = NULL;
	RuntimeObject* G_B3_0 = NULL;
	MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725 * G_B7_0 = NULL;
	MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2 * G_B6_0 = NULL;
	MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2 * G_B5_0 = NULL;
	RuntimeObject* G_B21_0 = NULL;
	RuntimeObject* G_B20_0 = NULL;
	MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * G_B22_0 = NULL;
	{
		// if (initializedFromProfile)
		bool L_0 = __this->get_initializedFromProfile_25();
		if (!L_0)
		{
			goto IL_0009;
		}
	}
	{
		// return;
		return;
	}

IL_0009:
	{
		// initializedFromProfile = true;
		__this->set_initializedFromProfile_25((bool)1);
		// var gestureProfile = InputSystem?.InputSystemProfile?.GesturesProfile;
		RuntimeObject* L_1 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_2 = L_1;
		G_B3_0 = L_2;
		if (L_2)
		{
			G_B4_0 = L_2;
			goto IL_001d;
		}
	}
	{
		G_B7_0 = ((MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725 *)(NULL));
		goto IL_002e;
	}

IL_001d:
	{
		NullCheck(G_B4_0);
		MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2 * L_3 = InterfaceFuncInvoker0< MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2 * >::Invoke(6 /* Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSystemProfile Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::get_InputSystemProfile() */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B4_0);
		MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2 * L_4 = L_3;
		G_B5_0 = L_4;
		if (L_4)
		{
			G_B6_0 = L_4;
			goto IL_0029;
		}
	}
	{
		G_B7_0 = ((MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725 *)(NULL));
		goto IL_002e;
	}

IL_0029:
	{
		NullCheck(G_B6_0);
		MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725 * L_5 = MixedRealityInputSystemProfile_get_GesturesProfile_mA8F275BA8A5AE96D3A95350F698A7343D72E5129_inline(G_B6_0, /*hidden argument*/NULL);
		G_B7_0 = L_5;
	}

IL_002e:
	{
		V_0 = G_B7_0;
		// if (gestureProfile != null)
		MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725 * L_6 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_7 = Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1(L_6, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_7)
		{
			goto IL_00c5;
		}
	}
	{
		// for (int i = 0; i < gestureProfile.Gestures.Length; i++)
		V_2 = 0;
		goto IL_00ae;
	}

IL_003f:
	{
		// var gesture = gestureProfile.Gestures[i];
		MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725 * L_8 = V_0;
		NullCheck(L_8);
		MixedRealityGestureMappingU5BU5D_t2F3D7B685E29F06002C6BD2EF99A97C8DF6BD874* L_9 = MixedRealityGesturesProfile_get_Gestures_mBAB7F3737E09478B3FA7F30ECAC24D6840E98580_inline(L_8, /*hidden argument*/NULL);
		int32_t L_10 = V_2;
		NullCheck(L_9);
		int32_t L_11 = L_10;
		MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074  L_12 = (L_9)->GetAt(static_cast<il2cpp_array_size_t>(L_11));
		V_3 = L_12;
		// switch (gesture.GestureType)
		int32_t L_13 = MixedRealityGestureMapping_get_GestureType_m6798792581776B818AF6A5307DD72D3425420C20_inline((MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074 *)(&V_3), /*hidden argument*/NULL);
		V_4 = L_13;
		int32_t L_14 = V_4;
		switch (((int32_t)il2cpp_codegen_subtract((int32_t)L_14, (int32_t)1)))
		{
			case 0:
			{
				goto IL_0070;
			}
			case 1:
			{
				goto IL_008e;
			}
			case 2:
			{
				goto IL_007f;
			}
			case 3:
			{
				goto IL_009d;
			}
		}
	}
	{
		goto IL_00aa;
	}

IL_0070:
	{
		// holdAction = gesture.Action;
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_15 = MixedRealityGestureMapping_get_Action_mF225EE997BA38AFC7DCCA99F71434633FD683D82_inline((MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074 *)(&V_3), /*hidden argument*/NULL);
		__this->set_holdAction_26(L_15);
		// break;
		goto IL_00aa;
	}

IL_007f:
	{
		// manipulationAction = gesture.Action;
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_16 = MixedRealityGestureMapping_get_Action_mF225EE997BA38AFC7DCCA99F71434633FD683D82_inline((MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074 *)(&V_3), /*hidden argument*/NULL);
		__this->set_manipulationAction_28(L_16);
		// break;
		goto IL_00aa;
	}

IL_008e:
	{
		// navigationAction = gesture.Action;
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_17 = MixedRealityGestureMapping_get_Action_mF225EE997BA38AFC7DCCA99F71434633FD683D82_inline((MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074 *)(&V_3), /*hidden argument*/NULL);
		__this->set_navigationAction_27(L_17);
		// break;
		goto IL_00aa;
	}

IL_009d:
	{
		// selectAction = gesture.Action;
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_18 = MixedRealityGestureMapping_get_Action_mF225EE997BA38AFC7DCCA99F71434633FD683D82_inline((MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074 *)(&V_3), /*hidden argument*/NULL);
		__this->set_selectAction_29(L_18);
	}

IL_00aa:
	{
		// for (int i = 0; i < gestureProfile.Gestures.Length; i++)
		int32_t L_19 = V_2;
		V_2 = ((int32_t)il2cpp_codegen_add((int32_t)L_19, (int32_t)1));
	}

IL_00ae:
	{
		// for (int i = 0; i < gestureProfile.Gestures.Length; i++)
		int32_t L_20 = V_2;
		MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725 * L_21 = V_0;
		NullCheck(L_21);
		MixedRealityGestureMappingU5BU5D_t2F3D7B685E29F06002C6BD2EF99A97C8DF6BD874* L_22 = MixedRealityGesturesProfile_get_Gestures_mBAB7F3737E09478B3FA7F30ECAC24D6840E98580_inline(L_21, /*hidden argument*/NULL);
		NullCheck(L_22);
		if ((((int32_t)L_20) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_22)->max_length)))))))
		{
			goto IL_003f;
		}
	}
	{
		// useRailsNavigation = gestureProfile.UseRailsNavigation;
		MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725 * L_23 = V_0;
		NullCheck(L_23);
		bool L_24 = MixedRealityGesturesProfile_get_UseRailsNavigation_mEAE6D30B9C69C0E5EA8115068FDA600F87CE02C6_inline(L_23, /*hidden argument*/NULL);
		__this->set_useRailsNavigation_30(L_24);
	}

IL_00c5:
	{
		// MixedRealityInputSimulationProfile inputSimProfile = null;
		V_1 = (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 *)NULL;
		// if (InputSystem != null)
		RuntimeObject* L_25 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		if (!L_25)
		{
			goto IL_00ed;
		}
	}
	{
		// inputSimProfile = (InputSystem as IMixedRealityDataProviderAccess).GetDataProvider<IInputSimulationService>()?.InputSimulationProfile;
		RuntimeObject* L_26 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		NullCheck(((RuntimeObject*)IsInst((RuntimeObject*)L_26, IMixedRealityDataProviderAccess_t8EDB3ADE5066213B543EB035F96F346DEF5FD94C_il2cpp_TypeInfo_var)));
		RuntimeObject* L_27 = GenericInterfaceFuncInvoker1< RuntimeObject*, String_t* >::Invoke(IMixedRealityDataProviderAccess_GetDataProvider_TisIInputSimulationService_t9AF3035C6487685E30A3E3ADB5E2D70DC2C3B443_m33255EF491AD44DA64F7825B26A7EEFE2BFAD51A_RuntimeMethod_var, ((RuntimeObject*)IsInst((RuntimeObject*)L_26, IMixedRealityDataProviderAccess_t8EDB3ADE5066213B543EB035F96F346DEF5FD94C_il2cpp_TypeInfo_var)), (String_t*)NULL);
		RuntimeObject* L_28 = L_27;
		G_B20_0 = L_28;
		if (L_28)
		{
			G_B21_0 = L_28;
			goto IL_00e7;
		}
	}
	{
		G_B22_0 = ((MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 *)(NULL));
		goto IL_00ec;
	}

IL_00e7:
	{
		NullCheck(G_B21_0);
		MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * L_29 = InterfaceFuncInvoker0< MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * >::Invoke(0 /* Microsoft.MixedReality.Toolkit.Input.MixedRealityInputSimulationProfile Microsoft.MixedReality.Toolkit.Input.IInputSimulationService::get_InputSimulationProfile() */, IInputSimulationService_t9AF3035C6487685E30A3E3ADB5E2D70DC2C3B443_il2cpp_TypeInfo_var, G_B21_0);
		G_B22_0 = L_29;
	}

IL_00ec:
	{
		V_1 = G_B22_0;
	}

IL_00ed:
	{
		// if (inputSimProfile != null)
		MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * L_30 = V_1;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_31 = Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1(L_30, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_31)
		{
			goto IL_010e;
		}
	}
	{
		// holdStartDuration = inputSimProfile.HoldStartDuration;
		MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * L_32 = V_1;
		NullCheck(L_32);
		float L_33 = MixedRealityInputSimulationProfile_get_HoldStartDuration_mBC1A3E5C22D4854356392379561E246374610007_inline(L_32, /*hidden argument*/NULL);
		__this->set_holdStartDuration_31(L_33);
		// navigationStartThreshold = inputSimProfile.NavigationStartThreshold;
		MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * L_34 = V_1;
		NullCheck(L_34);
		float L_35 = MixedRealityInputSimulationProfile_get_NavigationStartThreshold_m30BD08DA409E73AE42567F6420EB5E92DC7981E4_inline(L_34, /*hidden argument*/NULL);
		__this->set_navigationStartThreshold_32(L_35);
	}

IL_010e:
	{
		// }
		return;
	}
}
// Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping[] Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::get_DefaultInteractions()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* SimulatedGestureHand_get_DefaultInteractions_m304D32B99A064523F1EC9DFD6873DEB55A56A8AF (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedGestureHand_get_DefaultInteractions_m304D32B99A064523F1EC9DFD6873DEB55A56A8AF_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
		// {
		//     new MixedRealityInteractionMapping(0, "Select", AxisType.Digital, DeviceInputType.Select, MixedRealityInputAction.None),
		//     new MixedRealityInteractionMapping(1, "Grip Pose", AxisType.SixDof, DeviceInputType.SpatialGrip, MixedRealityInputAction.None),
		// };
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_0 = (MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA*)(MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA*)SZArrayNew(MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA_il2cpp_TypeInfo_var, (uint32_t)2);
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_1 = L_0;
		IL2CPP_RUNTIME_CLASS_INIT(MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073_il2cpp_TypeInfo_var);
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_2 = MixedRealityInputAction_get_None_m0276CF8988B0670DCCE381865DD5190010A2A8BF_inline(/*hidden argument*/NULL);
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_3 = (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 *)il2cpp_codegen_object_new(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2_il2cpp_TypeInfo_var);
		MixedRealityInteractionMapping__ctor_m42FA7B2EF2BAA3804530651DFDF1145EEECE437F(L_3, 0, _stringLiteral8598222918D3C6E513D63060CF55E2971DED729A, 2, ((int32_t)25), L_2, 0, _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709, _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709, (bool)0, (bool)0, /*hidden argument*/NULL);
		NullCheck(L_1);
		ArrayElementTypeCheck (L_1, L_3);
		(L_1)->SetAt(static_cast<il2cpp_array_size_t>(0), (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 *)L_3);
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_4 = L_1;
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_5 = MixedRealityInputAction_get_None_m0276CF8988B0670DCCE381865DD5190010A2A8BF_inline(/*hidden argument*/NULL);
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_6 = (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 *)il2cpp_codegen_object_new(MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2_il2cpp_TypeInfo_var);
		MixedRealityInteractionMapping__ctor_m42FA7B2EF2BAA3804530651DFDF1145EEECE437F(L_6, 1, _stringLiteral66654F3A427908EF2AB0102919620271D634DA8A, 7, ((int32_t)14), L_5, 0, _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709, _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709, (bool)0, (bool)0, /*hidden argument*/NULL);
		NullCheck(L_4);
		ArrayElementTypeCheck (L_4, L_6);
		(L_4)->SetAt(static_cast<il2cpp_array_size_t>(1), (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 *)L_6);
		return L_4;
	}
}
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::SetupDefaultInteractions(Microsoft.MixedReality.Toolkit.Utilities.Handedness)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedGestureHand_SetupDefaultInteractions_m43EB37ECD45A5DF02A51C70044EE4A423D45F0EA (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, uint8_t ___controllerHandedness0, const RuntimeMethod* method)
{
	{
		// AssignControllerMappings(DefaultInteractions);
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_0 = VirtFuncInvoker0< MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* >::Invoke(17 /* Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping[] Microsoft.MixedReality.Toolkit.Input.BaseController::get_DefaultInteractions() */, __this);
		BaseController_AssignControllerMappings_mB58538C7085760171304343CFBD77E5D8F230054(__this, L_0, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::UpdateInteractions(Microsoft.MixedReality.Toolkit.Input.SimulatedHandData)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedGestureHand_UpdateInteractions_m96F24F8AEC7B7EC9C96EAF20378C4BBF49B26DF8 (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * ___handData0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedGestureHand_UpdateInteractions_m96F24F8AEC7B7EC9C96EAF20378C4BBF49B26DF8_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_0;
	memset((&V_0), 0, sizeof(V_0));
	MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  V_1;
	memset((&V_1), 0, sizeof(V_1));
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB  V_4;
	memset((&V_4), 0, sizeof(V_4));
	Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB  V_5;
	memset((&V_5), 0, sizeof(V_5));
	RuntimeObject* G_B3_0 = NULL;
	RuntimeObject* G_B2_0 = NULL;
	RuntimeObject* G_B11_0 = NULL;
	RuntimeObject* G_B10_0 = NULL;
	RuntimeObject* G_B16_0 = NULL;
	RuntimeObject* G_B15_0 = NULL;
	RuntimeObject* G_B20_0 = NULL;
	RuntimeObject* G_B19_0 = NULL;
	MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* G_B34_0 = NULL;
	int32_t G_B34_1 = 0;
	MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* G_B33_0 = NULL;
	int32_t G_B33_1 = 0;
	Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB  G_B35_0;
	memset((&G_B35_0), 0, sizeof(G_B35_0));
	int32_t G_B35_1 = 0;
	{
		// EnsureProfileSettings();
		SimulatedGestureHand_EnsureProfileSettings_m5FC39BD038B64363C40173D9E60B1BC1606C7A3A(__this, /*hidden argument*/NULL);
		// Vector3 lastPosition = currentPosition;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_0 = __this->get_currentPosition_38();
		V_0 = L_0;
		// currentPosition = jointPoses[TrackedHandJoint.IndexTip].Position;
		Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 * L_1 = ((SimulatedHand_tFBAB6AD39E9B16E093E63E4D2A88EA5E3415437E *)__this)->get_jointPoses_24();
		NullCheck(L_1);
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_2 = Dictionary_2_get_Item_mAA87FA69922BAF6733C05E34A765031668FCABA6(L_1, ((int32_t)11), /*hidden argument*/Dictionary_2_get_Item_mAA87FA69922BAF6733C05E34A765031668FCABA6_RuntimeMethod_var);
		V_1 = L_2;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_3 = MixedRealityPose_get_Position_mF175BAE3270E5432E605BDD5FD1FA5F722B24AEE_inline((MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 *)(&V_1), /*hidden argument*/NULL);
		__this->set_currentPosition_38(L_3);
		// cumulativeDelta += currentPosition - lastPosition;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_4 = __this->get_cumulativeDelta_39();
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_5 = __this->get_currentPosition_38();
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_6 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_7 = Vector3_op_Subtraction_mF9846B723A5034F8B9F5F5DCB78E3D67649143D3(L_5, L_6, /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_8 = Vector3_op_Addition_m929F9C17E5D11B94D50B4AFF1D730B70CB59B50E(L_4, L_7, /*hidden argument*/NULL);
		__this->set_cumulativeDelta_39(L_8);
		// currentGripPose.Position = currentPosition;
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * L_9 = __this->get_address_of_currentGripPose_40();
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_10 = __this->get_currentPosition_38();
		MixedRealityPose_set_Position_m28EBD523337BC95684EFC016980F3862DE763759_inline((MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 *)L_9, L_10, /*hidden argument*/NULL);
		// if (lastPosition != currentPosition)
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_11 = V_0;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_12 = __this->get_currentPosition_38();
		bool L_13 = Vector3_op_Inequality_mFEEAA4C4BF743FB5B8A47FF4967A5E2C73273D6E(L_11, L_12, /*hidden argument*/NULL);
		if (!L_13)
		{
			goto IL_0082;
		}
	}
	{
		// InputSystem?.RaiseSourcePositionChanged(InputSource, this, currentPosition);
		RuntimeObject* L_14 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_15 = L_14;
		G_B2_0 = L_15;
		if (L_15)
		{
			G_B3_0 = L_15;
			goto IL_0070;
		}
	}
	{
		goto IL_0082;
	}

IL_0070:
	{
		RuntimeObject* L_16 = BaseController_get_InputSource_m9F9D70F24AC4D5605665D31F6D8A6083A3CA1CFD_inline(__this, /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_17 = __this->get_currentPosition_38();
		NullCheck(G_B3_0);
		InterfaceActionInvoker3< RuntimeObject*, RuntimeObject*, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  >::Invoke(27 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaiseSourcePositionChanged(Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource,Microsoft.MixedReality.Toolkit.Input.IMixedRealityController,UnityEngine.Vector3) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B3_0, L_16, __this, L_17);
	}

IL_0082:
	{
		// for (int i = 0; i < Interactions?.Length; i++)
		V_2 = 0;
		goto IL_0238;
	}

IL_0089:
	{
		// switch (Interactions[i].InputType)
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_18 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_19 = V_2;
		NullCheck(L_18);
		int32_t L_20 = L_19;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_21 = (L_18)->GetAt(static_cast<il2cpp_array_size_t>(L_20));
		NullCheck(L_21);
		int32_t L_22 = MixedRealityInteractionMapping_get_InputType_mA8C027545479C380F87D72BDED734A9BDBFA40CD_inline(L_21, /*hidden argument*/NULL);
		V_3 = L_22;
		int32_t L_23 = V_3;
		if ((((int32_t)L_23) == ((int32_t)((int32_t)14))))
		{
			goto IL_00a6;
		}
	}
	{
		int32_t L_24 = V_3;
		if ((((int32_t)L_24) == ((int32_t)((int32_t)25))))
		{
			goto IL_0103;
		}
	}
	{
		goto IL_0234;
	}

IL_00a6:
	{
		// Interactions[i].PoseData = currentGripPose;
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_25 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_26 = V_2;
		NullCheck(L_25);
		int32_t L_27 = L_26;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_28 = (L_25)->GetAt(static_cast<il2cpp_array_size_t>(L_27));
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_29 = __this->get_currentGripPose_40();
		NullCheck(L_28);
		MixedRealityInteractionMapping_set_PoseData_mED53A7137722CE84DD3F8144D83C6E2F6B844287(L_28, L_29, /*hidden argument*/NULL);
		// if (Interactions[i].Changed)
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_30 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_31 = V_2;
		NullCheck(L_30);
		int32_t L_32 = L_31;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_33 = (L_30)->GetAt(static_cast<il2cpp_array_size_t>(L_32));
		NullCheck(L_33);
		bool L_34 = MixedRealityInteractionMapping_get_Changed_m70D15D24BDB909A6AA0E9C4DB393DAA25F84983F(L_33, /*hidden argument*/NULL);
		if (!L_34)
		{
			goto IL_0234;
		}
	}
	{
		// InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, currentGripPose);
		RuntimeObject* L_35 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_36 = L_35;
		G_B10_0 = L_36;
		if (L_36)
		{
			G_B11_0 = L_36;
			goto IL_00da;
		}
	}
	{
		goto IL_0234;
	}

IL_00da:
	{
		RuntimeObject* L_37 = BaseController_get_InputSource_m9F9D70F24AC4D5605665D31F6D8A6083A3CA1CFD_inline(__this, /*hidden argument*/NULL);
		uint8_t L_38 = BaseController_get_ControllerHandedness_mA18814111E1328E1C7C04C383CC44E8A2F8A995A_inline(__this, /*hidden argument*/NULL);
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_39 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_40 = V_2;
		NullCheck(L_39);
		int32_t L_41 = L_40;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_42 = (L_39)->GetAt(static_cast<il2cpp_array_size_t>(L_41));
		NullCheck(L_42);
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_43 = MixedRealityInteractionMapping_get_MixedRealityInputAction_mA22FF2AC6237AEF7B9EADF4461EB3B484CCB995E_inline(L_42, /*hidden argument*/NULL);
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_44 = __this->get_currentGripPose_40();
		NullCheck(G_B11_0);
		InterfaceActionInvoker4< RuntimeObject*, uint8_t, MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073 , MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  >::Invoke(44 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaisePoseInputChanged(Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource,Microsoft.MixedReality.Toolkit.Utilities.Handedness,Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B11_0, L_37, L_38, L_43, L_44);
		// break;
		goto IL_0234;
	}

IL_0103:
	{
		// Interactions[i].BoolData = handData.IsPinching;
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_45 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_46 = V_2;
		NullCheck(L_45);
		int32_t L_47 = L_46;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_48 = (L_45)->GetAt(static_cast<il2cpp_array_size_t>(L_47));
		SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * L_49 = ___handData0;
		NullCheck(L_49);
		bool L_50 = SimulatedHandData_get_IsPinching_mB7C40888399E88C93E755FE89D50234CF5F5C981_inline(L_49, /*hidden argument*/NULL);
		NullCheck(L_48);
		MixedRealityInteractionMapping_set_BoolData_mE86E7E665BCA02A2E69651A333993A51703F7D64(L_48, L_50, /*hidden argument*/NULL);
		// if (Interactions[i].Changed)
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_51 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_52 = V_2;
		NullCheck(L_51);
		int32_t L_53 = L_52;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_54 = (L_51)->GetAt(static_cast<il2cpp_array_size_t>(L_53));
		NullCheck(L_54);
		bool L_55 = MixedRealityInteractionMapping_get_Changed_m70D15D24BDB909A6AA0E9C4DB393DAA25F84983F(L_54, /*hidden argument*/NULL);
		if (!L_55)
		{
			goto IL_01c4;
		}
	}
	{
		// if (Interactions[i].BoolData)
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_56 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_57 = V_2;
		NullCheck(L_56);
		int32_t L_58 = L_57;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_59 = (L_56)->GetAt(static_cast<il2cpp_array_size_t>(L_58));
		NullCheck(L_59);
		bool L_60 = MixedRealityInteractionMapping_get_BoolData_mB42A4C428B73C25DC7FE9CAC463325E19255F71B_inline(L_59, /*hidden argument*/NULL);
		if (!L_60)
		{
			goto IL_017c;
		}
	}
	{
		// InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
		RuntimeObject* L_61 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_62 = L_61;
		G_B15_0 = L_62;
		if (L_62)
		{
			G_B16_0 = L_62;
			goto IL_0143;
		}
	}
	{
		goto IL_0161;
	}

IL_0143:
	{
		RuntimeObject* L_63 = BaseController_get_InputSource_m9F9D70F24AC4D5605665D31F6D8A6083A3CA1CFD_inline(__this, /*hidden argument*/NULL);
		uint8_t L_64 = BaseController_get_ControllerHandedness_mA18814111E1328E1C7C04C383CC44E8A2F8A995A_inline(__this, /*hidden argument*/NULL);
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_65 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_66 = V_2;
		NullCheck(L_65);
		int32_t L_67 = L_66;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_68 = (L_65)->GetAt(static_cast<il2cpp_array_size_t>(L_67));
		NullCheck(L_68);
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_69 = MixedRealityInteractionMapping_get_MixedRealityInputAction_mA22FF2AC6237AEF7B9EADF4461EB3B484CCB995E_inline(L_68, /*hidden argument*/NULL);
		NullCheck(G_B16_0);
		InterfaceActionInvoker3< RuntimeObject*, uint8_t, MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  >::Invoke(38 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaiseOnInputDown(Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource,Microsoft.MixedReality.Toolkit.Utilities.Handedness,Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B16_0, L_63, L_64, L_69);
	}

IL_0161:
	{
		// SelectDownStartTime = Time.time;
		float L_70 = Time_get_time_m7863349C8845BBA36629A2B3F8EF1C3BEA350FD8(/*hidden argument*/NULL);
		__this->set_SelectDownStartTime_33(L_70);
		// cumulativeDelta = Vector3.zero;
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_71 = Vector3_get_zero_m3CDDCAE94581DF3BB16C4B40A100E28E9C6649C2(/*hidden argument*/NULL);
		__this->set_cumulativeDelta_39(L_71);
		// }
		goto IL_0234;
	}

IL_017c:
	{
		// InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);
		RuntimeObject* L_72 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_73 = L_72;
		G_B19_0 = L_73;
		if (L_73)
		{
			G_B20_0 = L_73;
			goto IL_0188;
		}
	}
	{
		goto IL_01a6;
	}

IL_0188:
	{
		RuntimeObject* L_74 = BaseController_get_InputSource_m9F9D70F24AC4D5605665D31F6D8A6083A3CA1CFD_inline(__this, /*hidden argument*/NULL);
		uint8_t L_75 = BaseController_get_ControllerHandedness_mA18814111E1328E1C7C04C383CC44E8A2F8A995A_inline(__this, /*hidden argument*/NULL);
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_76 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_77 = V_2;
		NullCheck(L_76);
		int32_t L_78 = L_77;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_79 = (L_76)->GetAt(static_cast<il2cpp_array_size_t>(L_78));
		NullCheck(L_79);
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_80 = MixedRealityInteractionMapping_get_MixedRealityInputAction_mA22FF2AC6237AEF7B9EADF4461EB3B484CCB995E_inline(L_79, /*hidden argument*/NULL);
		NullCheck(G_B20_0);
		InterfaceActionInvoker3< RuntimeObject*, uint8_t, MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  >::Invoke(39 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaiseOnInputUp(Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource,Microsoft.MixedReality.Toolkit.Utilities.Handedness,Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B20_0, L_74, L_75, L_80);
	}

IL_01a6:
	{
		// TryCompleteSelect();
		SimulatedGestureHand_TryCompleteSelect_m39126D98BA2E83C742CDA9EAEA81EB5128B541AC(__this, /*hidden argument*/NULL);
		// TryCompleteHold();
		SimulatedGestureHand_TryCompleteHold_mA3B5BAB738C6425798C608310D7D59D6B6FCA1AC(__this, /*hidden argument*/NULL);
		// TryCompleteManipulation();
		SimulatedGestureHand_TryCompleteManipulation_m7DD88EA40E108EB197BF22BD11460BF7A3DFBB18(__this, /*hidden argument*/NULL);
		// TryCompleteNavigation();
		SimulatedGestureHand_TryCompleteNavigation_m725C944777267419341F15E256472663CBCE6AC8(__this, /*hidden argument*/NULL);
		// }
		goto IL_0234;
	}

IL_01c4:
	{
		// else if (Interactions[i].BoolData)
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_81 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		int32_t L_82 = V_2;
		NullCheck(L_81);
		int32_t L_83 = L_82;
		MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * L_84 = (L_81)->GetAt(static_cast<il2cpp_array_size_t>(L_83));
		NullCheck(L_84);
		bool L_85 = MixedRealityInteractionMapping_get_BoolData_mB42A4C428B73C25DC7FE9CAC463325E19255F71B_inline(L_84, /*hidden argument*/NULL);
		if (!L_85)
		{
			goto IL_0234;
		}
	}
	{
		// if (manipulationInProgress)
		bool L_86 = __this->get_manipulationInProgress_35();
		if (!L_86)
		{
			goto IL_01e1;
		}
	}
	{
		// UpdateManipulation();
		SimulatedGestureHand_UpdateManipulation_m7D7C54E9B0364BA9862D4326D9606FB6419CCBC3(__this, /*hidden argument*/NULL);
	}

IL_01e1:
	{
		// if (navigationInProgress)
		bool L_87 = __this->get_navigationInProgress_36();
		if (!L_87)
		{
			goto IL_01ef;
		}
	}
	{
		// UpdateNavigation();
		SimulatedGestureHand_UpdateNavigation_mD504939EDF859CD568D6127F467D193ADF3ADFC0(__this, /*hidden argument*/NULL);
	}

IL_01ef:
	{
		// if (cumulativeDelta.magnitude > navigationStartThreshold)
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * L_88 = __this->get_address_of_cumulativeDelta_39();
		float L_89 = Vector3_get_magnitude_m9A750659B60C5FE0C30438A7F9681775D5DB1274((Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *)L_88, /*hidden argument*/NULL);
		float L_90 = __this->get_navigationStartThreshold_32();
		if ((!(((float)L_89) > ((float)L_90))))
		{
			goto IL_0219;
		}
	}
	{
		// TryCancelHold();
		SimulatedGestureHand_TryCancelHold_m1F67089B7A138E396206FE8E7E0DAEECCE14BFBC(__this, /*hidden argument*/NULL);
		// TryStartNavigation();
		SimulatedGestureHand_TryStartNavigation_m2F5F675D13ACB7225B7672755846459058BDF575(__this, /*hidden argument*/NULL);
		// TryStartManipulation();
		SimulatedGestureHand_TryStartManipulation_m0B58E7807CC8E31CE5F4817A99CC358085866A3E(__this, /*hidden argument*/NULL);
		// }
		goto IL_0234;
	}

IL_0219:
	{
		// else if (Time.time >= SelectDownStartTime + holdStartDuration)
		float L_91 = Time_get_time_m7863349C8845BBA36629A2B3F8EF1C3BEA350FD8(/*hidden argument*/NULL);
		float L_92 = __this->get_SelectDownStartTime_33();
		float L_93 = __this->get_holdStartDuration_31();
		if ((!(((float)L_91) >= ((float)((float)il2cpp_codegen_add((float)L_92, (float)L_93))))))
		{
			goto IL_0234;
		}
	}
	{
		// TryStartHold();
		SimulatedGestureHand_TryStartHold_m72CBFF5CAEDDC55C9E865745A5DE4C34C1B2E234(__this, /*hidden argument*/NULL);
	}

IL_0234:
	{
		// for (int i = 0; i < Interactions?.Length; i++)
		int32_t L_94 = V_2;
		V_2 = ((int32_t)il2cpp_codegen_add((int32_t)L_94, (int32_t)1));
	}

IL_0238:
	{
		// for (int i = 0; i < Interactions?.Length; i++)
		int32_t L_95 = V_2;
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_96 = BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline(__this, /*hidden argument*/NULL);
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_97 = L_96;
		G_B33_0 = L_97;
		G_B33_1 = L_95;
		if (L_97)
		{
			G_B34_0 = L_97;
			G_B34_1 = L_95;
			goto IL_024f;
		}
	}
	{
		il2cpp_codegen_initobj((&V_5), sizeof(Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB ));
		Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB  L_98 = V_5;
		G_B35_0 = L_98;
		G_B35_1 = G_B33_1;
		goto IL_0256;
	}

IL_024f:
	{
		NullCheck(G_B34_0);
		Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB  L_99;
		memset((&L_99), 0, sizeof(L_99));
		Nullable_1__ctor_m11F9C228CFDF836DDFCD7880C09CB4098AB9D7F2((&L_99), (((int32_t)((int32_t)(((RuntimeArray*)G_B34_0)->max_length)))), /*hidden argument*/Nullable_1__ctor_m11F9C228CFDF836DDFCD7880C09CB4098AB9D7F2_RuntimeMethod_var);
		G_B35_0 = L_99;
		G_B35_1 = G_B34_1;
	}

IL_0256:
	{
		V_4 = G_B35_0;
		int32_t L_100 = Nullable_1_GetValueOrDefault_mE89BB8F302DF31EE202251F4746859285860B6B6_inline((Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB *)(&V_4), /*hidden argument*/Nullable_1_GetValueOrDefault_mE89BB8F302DF31EE202251F4746859285860B6B6_RuntimeMethod_var);
		bool L_101 = Nullable_1_get_HasValue_mB664E2C41CADA8413EF8842E6601B8C696A7CE15_inline((Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB *)(&V_4), /*hidden argument*/Nullable_1_get_HasValue_mB664E2C41CADA8413EF8842E6601B8C696A7CE15_RuntimeMethod_var);
		if (((int32_t)((int32_t)((((int32_t)G_B35_1) < ((int32_t)L_100))? 1 : 0)&(int32_t)L_101)))
		{
			goto IL_0089;
		}
	}
	{
		// }
		return;
	}
}
// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::TryStartHold()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SimulatedGestureHand_TryStartHold_m72CBFF5CAEDDC55C9E865745A5DE4C34C1B2E234 (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedGestureHand_TryStartHold_m72CBFF5CAEDDC55C9E865745A5DE4C34C1B2E234_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject* G_B3_0 = NULL;
	RuntimeObject* G_B2_0 = NULL;
	{
		// if (!holdInProgress)
		bool L_0 = __this->get_holdInProgress_34();
		if (L_0)
		{
			goto IL_0029;
		}
	}
	{
		// InputSystem?.RaiseGestureStarted(this, holdAction);
		RuntimeObject* L_1 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_2 = L_1;
		G_B2_0 = L_2;
		if (L_2)
		{
			G_B3_0 = L_2;
			goto IL_0014;
		}
	}
	{
		goto IL_0020;
	}

IL_0014:
	{
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_3 = __this->get_holdAction_26();
		NullCheck(G_B3_0);
		InterfaceActionInvoker2< RuntimeObject*, MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  >::Invoke(45 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaiseGestureStarted(Microsoft.MixedReality.Toolkit.Input.IMixedRealityController,Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B3_0, __this, L_3);
	}

IL_0020:
	{
		// holdInProgress = true;
		__this->set_holdInProgress_34((bool)1);
		// return true;
		return (bool)1;
	}

IL_0029:
	{
		// return false;
		return (bool)0;
	}
}
// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::TryCompleteHold()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SimulatedGestureHand_TryCompleteHold_mA3B5BAB738C6425798C608310D7D59D6B6FCA1AC (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedGestureHand_TryCompleteHold_mA3B5BAB738C6425798C608310D7D59D6B6FCA1AC_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject* G_B3_0 = NULL;
	RuntimeObject* G_B2_0 = NULL;
	{
		// if (holdInProgress)
		bool L_0 = __this->get_holdInProgress_34();
		if (!L_0)
		{
			goto IL_0029;
		}
	}
	{
		// InputSystem?.RaiseGestureCompleted(this, holdAction);
		RuntimeObject* L_1 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_2 = L_1;
		G_B2_0 = L_2;
		if (L_2)
		{
			G_B3_0 = L_2;
			goto IL_0014;
		}
	}
	{
		goto IL_0020;
	}

IL_0014:
	{
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_3 = __this->get_holdAction_26();
		NullCheck(G_B3_0);
		InterfaceActionInvoker2< RuntimeObject*, MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  >::Invoke(51 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaiseGestureCompleted(Microsoft.MixedReality.Toolkit.Input.IMixedRealityController,Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B3_0, __this, L_3);
	}

IL_0020:
	{
		// holdInProgress = false;
		__this->set_holdInProgress_34((bool)0);
		// return true;
		return (bool)1;
	}

IL_0029:
	{
		// return false;
		return (bool)0;
	}
}
// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::TryCancelHold()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SimulatedGestureHand_TryCancelHold_m1F67089B7A138E396206FE8E7E0DAEECCE14BFBC (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedGestureHand_TryCancelHold_m1F67089B7A138E396206FE8E7E0DAEECCE14BFBC_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject* G_B3_0 = NULL;
	RuntimeObject* G_B2_0 = NULL;
	{
		// if (holdInProgress)
		bool L_0 = __this->get_holdInProgress_34();
		if (!L_0)
		{
			goto IL_0029;
		}
	}
	{
		// InputSystem?.RaiseGestureCanceled(this, holdAction);
		RuntimeObject* L_1 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_2 = L_1;
		G_B2_0 = L_2;
		if (L_2)
		{
			G_B3_0 = L_2;
			goto IL_0014;
		}
	}
	{
		goto IL_0020;
	}

IL_0014:
	{
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_3 = __this->get_holdAction_26();
		NullCheck(G_B3_0);
		InterfaceActionInvoker2< RuntimeObject*, MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  >::Invoke(56 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaiseGestureCanceled(Microsoft.MixedReality.Toolkit.Input.IMixedRealityController,Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B3_0, __this, L_3);
	}

IL_0020:
	{
		// holdInProgress = false;
		__this->set_holdInProgress_34((bool)0);
		// return true;
		return (bool)1;
	}

IL_0029:
	{
		// return false;
		return (bool)0;
	}
}
// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::TryStartManipulation()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SimulatedGestureHand_TryStartManipulation_m0B58E7807CC8E31CE5F4817A99CC358085866A3E (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedGestureHand_TryStartManipulation_m0B58E7807CC8E31CE5F4817A99CC358085866A3E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject* G_B3_0 = NULL;
	RuntimeObject* G_B2_0 = NULL;
	{
		// if (!manipulationInProgress)
		bool L_0 = __this->get_manipulationInProgress_35();
		if (L_0)
		{
			goto IL_0029;
		}
	}
	{
		// InputSystem?.RaiseGestureStarted(this, manipulationAction);
		RuntimeObject* L_1 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_2 = L_1;
		G_B2_0 = L_2;
		if (L_2)
		{
			G_B3_0 = L_2;
			goto IL_0014;
		}
	}
	{
		goto IL_0020;
	}

IL_0014:
	{
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_3 = __this->get_manipulationAction_28();
		NullCheck(G_B3_0);
		InterfaceActionInvoker2< RuntimeObject*, MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  >::Invoke(45 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaiseGestureStarted(Microsoft.MixedReality.Toolkit.Input.IMixedRealityController,Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B3_0, __this, L_3);
	}

IL_0020:
	{
		// manipulationInProgress = true;
		__this->set_manipulationInProgress_35((bool)1);
		// return true;
		return (bool)1;
	}

IL_0029:
	{
		// return false;
		return (bool)0;
	}
}
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::UpdateManipulation()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedGestureHand_UpdateManipulation_m7D7C54E9B0364BA9862D4326D9606FB6419CCBC3 (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedGestureHand_UpdateManipulation_m7D7C54E9B0364BA9862D4326D9606FB6419CCBC3_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject* G_B3_0 = NULL;
	RuntimeObject* G_B2_0 = NULL;
	{
		// if (manipulationInProgress)
		bool L_0 = __this->get_manipulationInProgress_35();
		if (!L_0)
		{
			goto IL_0025;
		}
	}
	{
		// InputSystem?.RaiseGestureUpdated(this, manipulationAction, cumulativeDelta);
		RuntimeObject* L_1 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_2 = L_1;
		G_B2_0 = L_2;
		if (L_2)
		{
			G_B3_0 = L_2;
			goto IL_0013;
		}
	}
	{
		return;
	}

IL_0013:
	{
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_3 = __this->get_manipulationAction_28();
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_4 = __this->get_cumulativeDelta_39();
		NullCheck(G_B3_0);
		InterfaceActionInvoker3< RuntimeObject*, MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073 , Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  >::Invoke(48 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaiseGestureUpdated(Microsoft.MixedReality.Toolkit.Input.IMixedRealityController,Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction,UnityEngine.Vector3) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B3_0, __this, L_3, L_4);
	}

IL_0025:
	{
		// }
		return;
	}
}
// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::TryCompleteManipulation()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SimulatedGestureHand_TryCompleteManipulation_m7DD88EA40E108EB197BF22BD11460BF7A3DFBB18 (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedGestureHand_TryCompleteManipulation_m7DD88EA40E108EB197BF22BD11460BF7A3DFBB18_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject* G_B3_0 = NULL;
	RuntimeObject* G_B2_0 = NULL;
	{
		// if (manipulationInProgress)
		bool L_0 = __this->get_manipulationInProgress_35();
		if (!L_0)
		{
			goto IL_002f;
		}
	}
	{
		// InputSystem?.RaiseGestureCompleted(this, manipulationAction, cumulativeDelta);
		RuntimeObject* L_1 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_2 = L_1;
		G_B2_0 = L_2;
		if (L_2)
		{
			G_B3_0 = L_2;
			goto IL_0014;
		}
	}
	{
		goto IL_0026;
	}

IL_0014:
	{
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_3 = __this->get_manipulationAction_28();
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_4 = __this->get_cumulativeDelta_39();
		NullCheck(G_B3_0);
		InterfaceActionInvoker3< RuntimeObject*, MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073 , Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  >::Invoke(53 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaiseGestureCompleted(Microsoft.MixedReality.Toolkit.Input.IMixedRealityController,Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction,UnityEngine.Vector3) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B3_0, __this, L_3, L_4);
	}

IL_0026:
	{
		// manipulationInProgress = false;
		__this->set_manipulationInProgress_35((bool)0);
		// return true;
		return (bool)1;
	}

IL_002f:
	{
		// return false;
		return (bool)0;
	}
}
// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::TryCancelManipulation()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SimulatedGestureHand_TryCancelManipulation_m774C717F6300ED032BD87747966E2EBFBE9F3159 (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedGestureHand_TryCancelManipulation_m774C717F6300ED032BD87747966E2EBFBE9F3159_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject* G_B3_0 = NULL;
	RuntimeObject* G_B2_0 = NULL;
	{
		// if (manipulationInProgress)
		bool L_0 = __this->get_manipulationInProgress_35();
		if (!L_0)
		{
			goto IL_0029;
		}
	}
	{
		// InputSystem?.RaiseGestureCanceled(this, manipulationAction);
		RuntimeObject* L_1 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_2 = L_1;
		G_B2_0 = L_2;
		if (L_2)
		{
			G_B3_0 = L_2;
			goto IL_0014;
		}
	}
	{
		goto IL_0020;
	}

IL_0014:
	{
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_3 = __this->get_manipulationAction_28();
		NullCheck(G_B3_0);
		InterfaceActionInvoker2< RuntimeObject*, MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  >::Invoke(56 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaiseGestureCanceled(Microsoft.MixedReality.Toolkit.Input.IMixedRealityController,Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B3_0, __this, L_3);
	}

IL_0020:
	{
		// manipulationInProgress = false;
		__this->set_manipulationInProgress_35((bool)0);
		// return true;
		return (bool)1;
	}

IL_0029:
	{
		// return false;
		return (bool)0;
	}
}
// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::TryCompleteSelect()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SimulatedGestureHand_TryCompleteSelect_m39126D98BA2E83C742CDA9EAEA81EB5128B541AC (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedGestureHand_TryCompleteSelect_m39126D98BA2E83C742CDA9EAEA81EB5128B541AC_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject* G_B4_0 = NULL;
	RuntimeObject* G_B3_0 = NULL;
	{
		// if (!manipulationInProgress && !holdInProgress)
		bool L_0 = __this->get_manipulationInProgress_35();
		if (L_0)
		{
			goto IL_002a;
		}
	}
	{
		bool L_1 = __this->get_holdInProgress_34();
		if (L_1)
		{
			goto IL_002a;
		}
	}
	{
		// InputSystem?.RaiseGestureCompleted(this, selectAction);
		RuntimeObject* L_2 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_3 = L_2;
		G_B3_0 = L_3;
		if (L_3)
		{
			G_B4_0 = L_3;
			goto IL_001c;
		}
	}
	{
		goto IL_0028;
	}

IL_001c:
	{
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_4 = __this->get_selectAction_29();
		NullCheck(G_B4_0);
		InterfaceActionInvoker2< RuntimeObject*, MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  >::Invoke(51 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaiseGestureCompleted(Microsoft.MixedReality.Toolkit.Input.IMixedRealityController,Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B4_0, __this, L_4);
	}

IL_0028:
	{
		// return true;
		return (bool)1;
	}

IL_002a:
	{
		// return false;
		return (bool)0;
	}
}
// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::TryStartNavigation()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SimulatedGestureHand_TryStartNavigation_m2F5F675D13ACB7225B7672755846459058BDF575 (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedGestureHand_TryStartNavigation_m2F5F675D13ACB7225B7672755846459058BDF575_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject* G_B3_0 = NULL;
	RuntimeObject* G_B2_0 = NULL;
	{
		// if (!navigationInProgress)
		bool L_0 = __this->get_navigationInProgress_36();
		if (L_0)
		{
			goto IL_003a;
		}
	}
	{
		// InputSystem?.RaiseGestureStarted(this, navigationAction);
		RuntimeObject* L_1 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_2 = L_1;
		G_B2_0 = L_2;
		if (L_2)
		{
			G_B3_0 = L_2;
			goto IL_0014;
		}
	}
	{
		goto IL_0020;
	}

IL_0014:
	{
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_3 = __this->get_navigationAction_27();
		NullCheck(G_B3_0);
		InterfaceActionInvoker2< RuntimeObject*, MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  >::Invoke(45 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaiseGestureStarted(Microsoft.MixedReality.Toolkit.Input.IMixedRealityController,Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B3_0, __this, L_3);
	}

IL_0020:
	{
		// navigationInProgress = true;
		__this->set_navigationInProgress_36((bool)1);
		// currentRailsUsed = Vector3.one;
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_4 = Vector3_get_one_mA11B83037CB269C6076CBCF754E24C8F3ACEC2AB(/*hidden argument*/NULL);
		__this->set_currentRailsUsed_37(L_4);
		// UpdateNavigationRails();
		SimulatedGestureHand_UpdateNavigationRails_mDA8C27C354D28CD6BC7E7EB7E4A84A560D1B08A6(__this, /*hidden argument*/NULL);
		// return true;
		return (bool)1;
	}

IL_003a:
	{
		// return false;
		return (bool)0;
	}
}
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::UpdateNavigation()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedGestureHand_UpdateNavigation_mD504939EDF859CD568D6127F467D193ADF3ADFC0 (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedGestureHand_UpdateNavigation_mD504939EDF859CD568D6127F467D193ADF3ADFC0_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject* G_B3_0 = NULL;
	RuntimeObject* G_B2_0 = NULL;
	{
		// if (navigationInProgress)
		bool L_0 = __this->get_navigationInProgress_36();
		if (!L_0)
		{
			goto IL_002b;
		}
	}
	{
		// UpdateNavigationRails();
		SimulatedGestureHand_UpdateNavigationRails_mDA8C27C354D28CD6BC7E7EB7E4A84A560D1B08A6(__this, /*hidden argument*/NULL);
		// InputSystem?.RaiseGestureUpdated(this, navigationAction, navigationDelta);
		RuntimeObject* L_1 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_2 = L_1;
		G_B2_0 = L_2;
		if (L_2)
		{
			G_B3_0 = L_2;
			goto IL_0019;
		}
	}
	{
		return;
	}

IL_0019:
	{
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_3 = __this->get_navigationAction_27();
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_4 = SimulatedGestureHand_get_navigationDelta_m0FD22233CFFA608F80B80E740D01DA6F8E22582A(__this, /*hidden argument*/NULL);
		NullCheck(G_B3_0);
		InterfaceActionInvoker3< RuntimeObject*, MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073 , Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  >::Invoke(48 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaiseGestureUpdated(Microsoft.MixedReality.Toolkit.Input.IMixedRealityController,Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction,UnityEngine.Vector3) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B3_0, __this, L_3, L_4);
	}

IL_002b:
	{
		// }
		return;
	}
}
// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::TryCompleteNavigation()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SimulatedGestureHand_TryCompleteNavigation_m725C944777267419341F15E256472663CBCE6AC8 (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedGestureHand_TryCompleteNavigation_m725C944777267419341F15E256472663CBCE6AC8_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject* G_B3_0 = NULL;
	RuntimeObject* G_B2_0 = NULL;
	{
		// if (navigationInProgress)
		bool L_0 = __this->get_navigationInProgress_36();
		if (!L_0)
		{
			goto IL_002f;
		}
	}
	{
		// InputSystem?.RaiseGestureCompleted(this, navigationAction, navigationDelta);
		RuntimeObject* L_1 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_2 = L_1;
		G_B2_0 = L_2;
		if (L_2)
		{
			G_B3_0 = L_2;
			goto IL_0014;
		}
	}
	{
		goto IL_0026;
	}

IL_0014:
	{
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_3 = __this->get_navigationAction_27();
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_4 = SimulatedGestureHand_get_navigationDelta_m0FD22233CFFA608F80B80E740D01DA6F8E22582A(__this, /*hidden argument*/NULL);
		NullCheck(G_B3_0);
		InterfaceActionInvoker3< RuntimeObject*, MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073 , Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  >::Invoke(53 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaiseGestureCompleted(Microsoft.MixedReality.Toolkit.Input.IMixedRealityController,Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction,UnityEngine.Vector3) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B3_0, __this, L_3, L_4);
	}

IL_0026:
	{
		// navigationInProgress = false;
		__this->set_navigationInProgress_36((bool)0);
		// return true;
		return (bool)1;
	}

IL_002f:
	{
		// return false;
		return (bool)0;
	}
}
// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::TryCancelNavigation()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SimulatedGestureHand_TryCancelNavigation_m7F78258B782D49B12470728A9F18ECFE2C0138A5 (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedGestureHand_TryCancelNavigation_m7F78258B782D49B12470728A9F18ECFE2C0138A5_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject* G_B3_0 = NULL;
	RuntimeObject* G_B2_0 = NULL;
	{
		// if (navigationInProgress)
		bool L_0 = __this->get_navigationInProgress_36();
		if (!L_0)
		{
			goto IL_0029;
		}
	}
	{
		// InputSystem?.RaiseGestureCanceled(this, navigationAction);
		RuntimeObject* L_1 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_2 = L_1;
		G_B2_0 = L_2;
		if (L_2)
		{
			G_B3_0 = L_2;
			goto IL_0014;
		}
	}
	{
		goto IL_0020;
	}

IL_0014:
	{
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_3 = __this->get_navigationAction_27();
		NullCheck(G_B3_0);
		InterfaceActionInvoker2< RuntimeObject*, MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  >::Invoke(56 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaiseGestureCanceled(Microsoft.MixedReality.Toolkit.Input.IMixedRealityController,Microsoft.MixedReality.Toolkit.Input.MixedRealityInputAction) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B3_0, __this, L_3);
	}

IL_0020:
	{
		// navigationInProgress = false;
		__this->set_navigationInProgress_36((bool)0);
		// return true;
		return (bool)1;
	}

IL_0029:
	{
		// return false;
		return (bool)0;
	}
}
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedGestureHand::UpdateNavigationRails()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedGestureHand_UpdateNavigationRails_mDA8C27C354D28CD6BC7E7EB7E4A84A560D1B08A6 (SimulatedGestureHand_t9A6617D8B7C1E31347E9B134A1D67AE017661EBB * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedGestureHand_UpdateNavigationRails_mDA8C27C354D28CD6BC7E7EB7E4A84A560D1B08A6_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (useRailsNavigation && currentRailsUsed == Vector3.one)
		bool L_0 = __this->get_useRailsNavigation_30();
		if (!L_0)
		{
			goto IL_00b8;
		}
	}
	{
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_1 = __this->get_currentRailsUsed_37();
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_2 = Vector3_get_one_mA11B83037CB269C6076CBCF754E24C8F3ACEC2AB(/*hidden argument*/NULL);
		bool L_3 = Vector3_op_Equality_mA9E2F96E98E71AE7ACCE74766D700D41F0404806(L_1, L_2, /*hidden argument*/NULL);
		if (!L_3)
		{
			goto IL_00b8;
		}
	}
	{
		// if (Mathf.Abs(cumulativeDelta.x) >= navigationStartThreshold)
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * L_4 = __this->get_address_of_cumulativeDelta_39();
		float L_5 = L_4->get_x_2();
		IL2CPP_RUNTIME_CLASS_INIT(Mathf_tFBDE6467D269BFE410605C7D806FD9991D4A89CB_il2cpp_TypeInfo_var);
		float L_6 = fabsf(L_5);
		float L_7 = __this->get_navigationStartThreshold_32();
		if ((!(((float)L_6) >= ((float)L_7))))
		{
			goto IL_0053;
		}
	}
	{
		// currentRailsUsed = new Vector3(1, 0, 0);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_8;
		memset((&L_8), 0, sizeof(L_8));
		Vector3__ctor_m08F61F548AA5836D8789843ACB4A81E4963D2EE1((&L_8), (1.0f), (0.0f), (0.0f), /*hidden argument*/NULL);
		__this->set_currentRailsUsed_37(L_8);
		// }
		return;
	}

IL_0053:
	{
		// else if (Mathf.Abs(cumulativeDelta.y) > navigationStartThreshold)
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * L_9 = __this->get_address_of_cumulativeDelta_39();
		float L_10 = L_9->get_y_3();
		IL2CPP_RUNTIME_CLASS_INIT(Mathf_tFBDE6467D269BFE410605C7D806FD9991D4A89CB_il2cpp_TypeInfo_var);
		float L_11 = fabsf(L_10);
		float L_12 = __this->get_navigationStartThreshold_32();
		if ((!(((float)L_11) > ((float)L_12))))
		{
			goto IL_0086;
		}
	}
	{
		// currentRailsUsed = new Vector3(0, 1, 0);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_13;
		memset((&L_13), 0, sizeof(L_13));
		Vector3__ctor_m08F61F548AA5836D8789843ACB4A81E4963D2EE1((&L_13), (0.0f), (1.0f), (0.0f), /*hidden argument*/NULL);
		__this->set_currentRailsUsed_37(L_13);
		// }
		return;
	}

IL_0086:
	{
		// else if (Mathf.Abs(cumulativeDelta.z) > navigationStartThreshold)
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * L_14 = __this->get_address_of_cumulativeDelta_39();
		float L_15 = L_14->get_z_4();
		IL2CPP_RUNTIME_CLASS_INIT(Mathf_tFBDE6467D269BFE410605C7D806FD9991D4A89CB_il2cpp_TypeInfo_var);
		float L_16 = fabsf(L_15);
		float L_17 = __this->get_navigationStartThreshold_32();
		if ((!(((float)L_16) > ((float)L_17))))
		{
			goto IL_00b8;
		}
	}
	{
		// currentRailsUsed = new Vector3(0, 0, 1);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_18;
		memset((&L_18), 0, sizeof(L_18));
		Vector3__ctor_m08F61F548AA5836D8789843ACB4A81E4963D2EE1((&L_18), (0.0f), (0.0f), (1.0f), /*hidden argument*/NULL);
		__this->set_currentRailsUsed_37(L_18);
	}

IL_00b8:
	{
		// }
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedHand::.ctor(Microsoft.MixedReality.Toolkit.TrackingState,Microsoft.MixedReality.Toolkit.Utilities.Handedness,Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource,Microsoft.MixedReality.Toolkit.Input.MixedRealityInteractionMapping[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedHand__ctor_m93808D1348F3FB6FA63A335E89F47FB5345EE1C4 (SimulatedHand_tFBAB6AD39E9B16E093E63E4D2A88EA5E3415437E * __this, int32_t ___trackingState0, uint8_t ___controllerHandedness1, RuntimeObject* ___inputSource2, MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* ___interactions3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedHand__ctor_m93808D1348F3FB6FA63A335E89F47FB5345EE1C4_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// protected readonly Dictionary<TrackedHandJoint, MixedRealityPose> jointPoses = new Dictionary<TrackedHandJoint, MixedRealityPose>();
		Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 * L_0 = (Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 *)il2cpp_codegen_object_new(Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86_il2cpp_TypeInfo_var);
		Dictionary_2__ctor_mD52EC03DD022577E1A73259E748910906383DA4E(L_0, /*hidden argument*/Dictionary_2__ctor_mD52EC03DD022577E1A73259E748910906383DA4E_RuntimeMethod_var);
		__this->set_jointPoses_24(L_0);
		// : base(trackingState, controllerHandedness, inputSource, interactions)
		int32_t L_1 = ___trackingState0;
		uint8_t L_2 = ___controllerHandedness1;
		RuntimeObject* L_3 = ___inputSource2;
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_4 = ___interactions3;
		BaseHand__ctor_mD486A5087D9CF2CC6B1048F37EEAD182843CB503(__this, L_1, L_2, L_3, L_4, /*hidden argument*/NULL);
		// {}
		return;
	}
}
// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedHand::TryGetJoint(Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SimulatedHand_TryGetJoint_m14B9D4449933B89DB099541E2901B4017D613B64 (SimulatedHand_tFBAB6AD39E9B16E093E63E4D2A88EA5E3415437E * __this, int32_t ___joint0, MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * ___pose1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedHand_TryGetJoint_m14B9D4449933B89DB099541E2901B4017D613B64_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// return jointPoses.TryGetValue(joint, out pose);
		Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 * L_0 = __this->get_jointPoses_24();
		int32_t L_1 = ___joint0;
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * L_2 = ___pose1;
		NullCheck(L_0);
		bool L_3 = Dictionary_2_TryGetValue_mEB4E22F5D5C93FBC06285B7EA9EDC0B6B73CF31D(L_0, L_1, (MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 *)L_2, /*hidden argument*/Dictionary_2_TryGetValue_mEB4E22F5D5C93FBC06285B7EA9EDC0B6B73CF31D_RuntimeMethod_var);
		return L_3;
	}
}
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedHand::UpdateState(Microsoft.MixedReality.Toolkit.Input.SimulatedHandData)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedHand_UpdateState_m76167DB74444C36B375258174DBB71C74806C7E7 (SimulatedHand_tFBAB6AD39E9B16E093E63E4D2A88EA5E3415437E * __this, SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * ___handData0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedHand_UpdateState_m76167DB74444C36B375258174DBB71C74806C7E7_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	RuntimeObject* G_B8_0 = NULL;
	RuntimeObject* G_B7_0 = NULL;
	{
		// for (int i = 0; i < jointCount; i++)
		V_0 = 0;
		goto IL_004a;
	}

IL_0004:
	{
		// TrackedHandJoint handJoint = (TrackedHandJoint)i;
		int32_t L_0 = V_0;
		V_1 = L_0;
		// if (!jointPoses.ContainsKey(handJoint))
		Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 * L_1 = __this->get_jointPoses_24();
		int32_t L_2 = V_1;
		NullCheck(L_1);
		bool L_3 = Dictionary_2_ContainsKey_m9123BEB1C67E91B9D1C87834EED0E4805EAB9389(L_1, L_2, /*hidden argument*/Dictionary_2_ContainsKey_m9123BEB1C67E91B9D1C87834EED0E4805EAB9389_RuntimeMethod_var);
		if (L_3)
		{
			goto IL_002e;
		}
	}
	{
		// jointPoses.Add(handJoint, handData.Joints[i]);
		Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 * L_4 = __this->get_jointPoses_24();
		int32_t L_5 = V_1;
		SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * L_6 = ___handData0;
		NullCheck(L_6);
		MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD* L_7 = SimulatedHandData_get_Joints_m0137F96239589766E8132147EBBC5D1C24516B7C_inline(L_6, /*hidden argument*/NULL);
		int32_t L_8 = V_0;
		NullCheck(L_7);
		int32_t L_9 = L_8;
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_10 = (L_7)->GetAt(static_cast<il2cpp_array_size_t>(L_9));
		NullCheck(L_4);
		Dictionary_2_Add_mF5D352A2DB17E5E4545D622A66744A4697ACC3D2(L_4, L_5, L_10, /*hidden argument*/Dictionary_2_Add_mF5D352A2DB17E5E4545D622A66744A4697ACC3D2_RuntimeMethod_var);
		// }
		goto IL_0046;
	}

IL_002e:
	{
		// jointPoses[handJoint] = handData.Joints[i];
		Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 * L_11 = __this->get_jointPoses_24();
		int32_t L_12 = V_1;
		SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * L_13 = ___handData0;
		NullCheck(L_13);
		MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD* L_14 = SimulatedHandData_get_Joints_m0137F96239589766E8132147EBBC5D1C24516B7C_inline(L_13, /*hidden argument*/NULL);
		int32_t L_15 = V_0;
		NullCheck(L_14);
		int32_t L_16 = L_15;
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_17 = (L_14)->GetAt(static_cast<il2cpp_array_size_t>(L_16));
		NullCheck(L_11);
		Dictionary_2_set_Item_mA73F452CC26A09DD780D50EAE46E8684633BA15B(L_11, L_12, L_17, /*hidden argument*/Dictionary_2_set_Item_mA73F452CC26A09DD780D50EAE46E8684633BA15B_RuntimeMethod_var);
	}

IL_0046:
	{
		// for (int i = 0; i < jointCount; i++)
		int32_t L_18 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_add((int32_t)L_18, (int32_t)1));
	}

IL_004a:
	{
		// for (int i = 0; i < jointCount; i++)
		int32_t L_19 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(SimulatedHand_tFBAB6AD39E9B16E093E63E4D2A88EA5E3415437E_il2cpp_TypeInfo_var);
		int32_t L_20 = ((SimulatedHand_tFBAB6AD39E9B16E093E63E4D2A88EA5E3415437E_StaticFields*)il2cpp_codegen_static_fields_for(SimulatedHand_tFBAB6AD39E9B16E093E63E4D2A88EA5E3415437E_il2cpp_TypeInfo_var))->get_jointCount_23();
		if ((((int32_t)L_19) < ((int32_t)L_20)))
		{
			goto IL_0004;
		}
	}
	{
		// InputSystem?.RaiseHandJointsUpdated(InputSource, ControllerHandedness, jointPoses);
		RuntimeObject* L_21 = BaseController_get_InputSystem_m49950F99CD27E15F1CA252ECFE568C8945145365(__this, /*hidden argument*/NULL);
		RuntimeObject* L_22 = L_21;
		G_B7_0 = L_22;
		if (L_22)
		{
			G_B8_0 = L_22;
			goto IL_005e;
		}
	}
	{
		goto IL_0075;
	}

IL_005e:
	{
		RuntimeObject* L_23 = BaseController_get_InputSource_m9F9D70F24AC4D5605665D31F6D8A6083A3CA1CFD_inline(__this, /*hidden argument*/NULL);
		uint8_t L_24 = BaseController_get_ControllerHandedness_mA18814111E1328E1C7C04C383CC44E8A2F8A995A_inline(__this, /*hidden argument*/NULL);
		Dictionary_2_tC314057363AB78F99AD807B804C5676B14530F86 * L_25 = __this->get_jointPoses_24();
		NullCheck(G_B8_0);
		InterfaceActionInvoker3< RuntimeObject*, uint8_t, RuntimeObject* >::Invoke(62 /* System.Void Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem::RaiseHandJointsUpdated(Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSource,Microsoft.MixedReality.Toolkit.Utilities.Handedness,System.Collections.Generic.IDictionary`2<Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint,Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose>) */, IMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_il2cpp_TypeInfo_var, G_B8_0, L_23, L_24, L_25);
	}

IL_0075:
	{
		// UpdateVelocity();
		BaseHand_UpdateVelocity_m2E2A6FE7655DBBE7E1BEBD9DAD7936B28DCEE484(__this, /*hidden argument*/NULL);
		// UpdateInteractions(handData);
		SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * L_26 = ___handData0;
		VirtActionInvoker1< SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * >::Invoke(26 /* System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedHand::UpdateInteractions(Microsoft.MixedReality.Toolkit.Input.SimulatedHandData) */, __this, L_26);
		// }
		return;
	}
}
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedHand::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedHand__cctor_mD1BA38A6EB0C974530FDAEA1E4A70CE9C16F7B5A (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedHand__cctor_mD1BA38A6EB0C974530FDAEA1E4A70CE9C16F7B5A_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// protected static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_0 = { reinterpret_cast<intptr_t> (TrackedHandJoint_tDE2FD40782A5B0C1D39386D6BF70D8A1CCF94E22_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_1 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_0, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Enum_t2AF27C02B8653AE29442467390005ABC74D8F521_il2cpp_TypeInfo_var);
		StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* L_2 = Enum_GetNames_m9ECDF3E80A7A31075D7D2B2B362DDCC6150BC15C(L_1, /*hidden argument*/NULL);
		NullCheck(L_2);
		((SimulatedHand_tFBAB6AD39E9B16E093E63E4D2A88EA5E3415437E_StaticFields*)il2cpp_codegen_static_fields_for(SimulatedHand_tFBAB6AD39E9B16E093E63E4D2A88EA5E3415437E_il2cpp_TypeInfo_var))->set_jointCount_23((((int32_t)((int32_t)(((RuntimeArray*)L_2)->max_length)))));
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedHandData::get_IsTracked()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SimulatedHandData_get_IsTracked_m44B1246872F6BE0B0A308EB2CC5259B6DFCF7FBF (SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * __this, const RuntimeMethod* method)
{
	{
		// public bool IsTracked => isTracked;
		bool L_0 = __this->get_isTracked_1();
		return L_0;
	}
}
// Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose[] Microsoft.MixedReality.Toolkit.Input.SimulatedHandData::get_Joints()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD* SimulatedHandData_get_Joints_m0137F96239589766E8132147EBBC5D1C24516B7C (SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * __this, const RuntimeMethod* method)
{
	{
		// public MixedRealityPose[] Joints => joints;
		MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD* L_0 = __this->get_joints_2();
		return L_0;
	}
}
// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedHandData::get_IsPinching()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SimulatedHandData_get_IsPinching_mB7C40888399E88C93E755FE89D50234CF5F5C981 (SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * __this, const RuntimeMethod* method)
{
	{
		// public bool IsPinching => isPinching;
		bool L_0 = __this->get_isPinching_3();
		return L_0;
	}
}
// Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem Microsoft.MixedReality.Toolkit.Input.SimulatedHandData::get_InputSystem()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* SimulatedHandData_get_InputSystem_m74B585679CB887A0A5722F761D09C8AC21A5E799 (SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedHandData_get_InputSystem_m74B585679CB887A0A5722F761D09C8AC21A5E799_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (inputSystem == null)
		RuntimeObject* L_0 = __this->get_inputSystem_4();
		if (L_0)
		{
			goto IL_0015;
		}
	}
	{
		// MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
		RuntimeObject** L_1 = __this->get_address_of_inputSystem_4();
		IL2CPP_RUNTIME_CLASS_INIT(MixedRealityServiceRegistry_t32DA3C08833DAE82817D72D1EE88363D3064D911_il2cpp_TypeInfo_var);
		MixedRealityServiceRegistry_TryGetService_TisIMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_m11EAC52C13EC4EEBB2BC67A0F3F775159F619EAD((RuntimeObject**)L_1, (String_t*)NULL, /*hidden argument*/MixedRealityServiceRegistry_TryGetService_TisIMixedRealityInputSystem_t5CCAA5BAD9D45403FCE5D1B3FEEB2E45BA65B22B_m11EAC52C13EC4EEBB2BC67A0F3F775159F619EAD_RuntimeMethod_var);
	}

IL_0015:
	{
		// return inputSystem;
		RuntimeObject* L_2 = __this->get_inputSystem_4();
		return L_2;
	}
}
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedHandData::Copy(Microsoft.MixedReality.Toolkit.Input.SimulatedHandData)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedHandData_Copy_m41ABA1DF6D6E58F82E3DF8D876F210F2D75BCC52 (SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * __this, SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * ___other0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedHandData_Copy_m41ABA1DF6D6E58F82E3DF8D876F210F2D75BCC52_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	{
		// isTracked = other.isTracked;
		SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * L_0 = ___other0;
		NullCheck(L_0);
		bool L_1 = L_0->get_isTracked_1();
		__this->set_isTracked_1(L_1);
		// isPinching = other.isPinching;
		SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * L_2 = ___other0;
		NullCheck(L_2);
		bool L_3 = L_2->get_isPinching_3();
		__this->set_isPinching_3(L_3);
		// for (int i = 0; i < jointCount; ++i)
		V_0 = 0;
		goto IL_0038;
	}

IL_001c:
	{
		// joints[i] = other.joints[i];
		MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD* L_4 = __this->get_joints_2();
		int32_t L_5 = V_0;
		SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * L_6 = ___other0;
		NullCheck(L_6);
		MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD* L_7 = L_6->get_joints_2();
		int32_t L_8 = V_0;
		NullCheck(L_7);
		int32_t L_9 = L_8;
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_10 = (L_7)->GetAt(static_cast<il2cpp_array_size_t>(L_9));
		NullCheck(L_4);
		(L_4)->SetAt(static_cast<il2cpp_array_size_t>(L_5), (MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 )L_10);
		// for (int i = 0; i < jointCount; ++i)
		int32_t L_11 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_add((int32_t)L_11, (int32_t)1));
	}

IL_0038:
	{
		// for (int i = 0; i < jointCount; ++i)
		int32_t L_12 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1_il2cpp_TypeInfo_var);
		int32_t L_13 = ((SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1_StaticFields*)il2cpp_codegen_static_fields_for(SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1_il2cpp_TypeInfo_var))->get_jointCount_0();
		if ((((int32_t)L_12) < ((int32_t)L_13)))
		{
			goto IL_001c;
		}
	}
	{
		// }
		return;
	}
}
// System.Boolean Microsoft.MixedReality.Toolkit.Input.SimulatedHandData::Update(System.Boolean,System.Boolean,Microsoft.MixedReality.Toolkit.Input.SimulatedHandData_HandJointDataGenerator)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SimulatedHandData_Update_m8F8FA53BE78C0B1B1B5AEDD04E81EE37283C2048 (SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * __this, bool ___isTrackedNew0, bool ___isPinchingNew1, HandJointDataGenerator_t70BF622884D5C475C85D34FDE76FD298FAC37955 * ___generator2, const RuntimeMethod* method)
{
	bool V_0 = false;
	{
		// bool handDataChanged = false;
		V_0 = (bool)0;
		// if (isTracked != isTrackedNew || isPinching != isPinchingNew)
		bool L_0 = __this->get_isTracked_1();
		bool L_1 = ___isTrackedNew0;
		if ((!(((uint32_t)L_0) == ((uint32_t)L_1))))
		{
			goto IL_0014;
		}
	}
	{
		bool L_2 = __this->get_isPinching_3();
		bool L_3 = ___isPinchingNew1;
		if ((((int32_t)L_2) == ((int32_t)L_3)))
		{
			goto IL_0024;
		}
	}

IL_0014:
	{
		// isTracked = isTrackedNew;
		bool L_4 = ___isTrackedNew0;
		__this->set_isTracked_1(L_4);
		// isPinching = isPinchingNew;
		bool L_5 = ___isPinchingNew1;
		__this->set_isPinching_3(L_5);
		// handDataChanged = true;
		V_0 = (bool)1;
	}

IL_0024:
	{
		// if (isTracked)
		bool L_6 = __this->get_isTracked_1();
		if (!L_6)
		{
			goto IL_003a;
		}
	}
	{
		// generator(Joints);
		HandJointDataGenerator_t70BF622884D5C475C85D34FDE76FD298FAC37955 * L_7 = ___generator2;
		MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD* L_8 = SimulatedHandData_get_Joints_m0137F96239589766E8132147EBBC5D1C24516B7C_inline(__this, /*hidden argument*/NULL);
		NullCheck(L_7);
		HandJointDataGenerator_Invoke_m453D8F003A5B2375922D4E902074628FA4AAB4F2(L_7, L_8, /*hidden argument*/NULL);
		// handDataChanged = true;
		V_0 = (bool)1;
	}

IL_003a:
	{
		// return handDataChanged;
		bool L_9 = V_0;
		return L_9;
	}
}
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedHandData::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedHandData__ctor_mC0F48E57A15AA83EB147D0682EAFD4B9A13A74E3 (SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedHandData__ctor_mC0F48E57A15AA83EB147D0682EAFD4B9A13A74E3_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// private MixedRealityPose[] joints = new MixedRealityPose[jointCount];
		IL2CPP_RUNTIME_CLASS_INIT(SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1_il2cpp_TypeInfo_var);
		int32_t L_0 = ((SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1_StaticFields*)il2cpp_codegen_static_fields_for(SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1_il2cpp_TypeInfo_var))->get_jointCount_0();
		MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD* L_1 = (MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD*)(MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD*)SZArrayNew(MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD_il2cpp_TypeInfo_var, (uint32_t)L_0);
		__this->set_joints_2(L_1);
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		return;
	}
}
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedHandData::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedHandData__cctor_m9FF93A339C2E4BD70FD2048183E316BDEFD82849 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedHandData__cctor_m9FF93A339C2E4BD70FD2048183E316BDEFD82849_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_0 = { reinterpret_cast<intptr_t> (TrackedHandJoint_tDE2FD40782A5B0C1D39386D6BF70D8A1CCF94E22_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_1 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_0, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Enum_t2AF27C02B8653AE29442467390005ABC74D8F521_il2cpp_TypeInfo_var);
		StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* L_2 = Enum_GetNames_m9ECDF3E80A7A31075D7D2B2B362DDCC6150BC15C(L_1, /*hidden argument*/NULL);
		NullCheck(L_2);
		((SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1_StaticFields*)il2cpp_codegen_static_fields_for(SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1_il2cpp_TypeInfo_var))->set_jointCount_0((((int32_t)((int32_t)(((RuntimeArray*)L_2)->max_length)))));
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
IL2CPP_EXTERN_C  void DelegatePInvokeWrapper_HandJointDataGenerator_t70BF622884D5C475C85D34FDE76FD298FAC37955 (HandJointDataGenerator_t70BF622884D5C475C85D34FDE76FD298FAC37955 * __this, MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD* ___jointPoses0, const RuntimeMethod* method)
{
	typedef void (DEFAULT_CALL *PInvokeFunc)(MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 *);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Marshaling of parameter '___jointPoses0' to native representation
	MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * ____jointPoses0_marshaled = NULL;
	if (___jointPoses0 != NULL)
	{
		____jointPoses0_marshaled = reinterpret_cast<MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 *>((___jointPoses0)->GetAddressAtUnchecked(0));
	}

	// Native function invocation
	il2cppPInvokeFunc(____jointPoses0_marshaled);

}
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedHandData_HandJointDataGenerator::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void HandJointDataGenerator__ctor_mB815FE73EC4C1E2EA223BA3380BC7817ACED0EB0 (HandJointDataGenerator_t70BF622884D5C475C85D34FDE76FD298FAC37955 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedHandData_HandJointDataGenerator::Invoke(Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void HandJointDataGenerator_Invoke_m453D8F003A5B2375922D4E902074628FA4AAB4F2 (HandJointDataGenerator_t70BF622884D5C475C85D34FDE76FD298FAC37955 * __this, MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD* ___jointPoses0, const RuntimeMethod* method)
{
	DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86* delegateArrayToInvoke = __this->get_delegates_11();
	Delegate_t** delegatesToInvoke;
	il2cpp_array_size_t length;
	if (delegateArrayToInvoke != NULL)
	{
		length = delegateArrayToInvoke->max_length;
		delegatesToInvoke = reinterpret_cast<Delegate_t**>(delegateArrayToInvoke->GetAddressAtUnchecked(0));
	}
	else
	{
		length = 1;
		delegatesToInvoke = reinterpret_cast<Delegate_t**>(&__this);
	}

	for (il2cpp_array_size_t i = 0; i < length; i++)
	{
		Delegate_t* currentDelegate = delegatesToInvoke[i];
		Il2CppMethodPointer targetMethodPointer = currentDelegate->get_method_ptr_0();
		RuntimeObject* targetThis = currentDelegate->get_m_target_2();
		RuntimeMethod* targetMethod = (RuntimeMethod*)(currentDelegate->get_method_3());
		if (!il2cpp_codegen_method_is_virtual(targetMethod))
		{
			il2cpp_codegen_raise_execution_engine_exception_if_method_is_not_found(targetMethod);
		}
		bool ___methodIsStatic = MethodIsStatic(targetMethod);
		int ___parameterCount = il2cpp_codegen_method_parameter_count(targetMethod);
		if (___methodIsStatic)
		{
			if (___parameterCount == 1)
			{
				// open
				typedef void (*FunctionPointerType) (MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD*, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___jointPoses0, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD*, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___jointPoses0, targetMethod);
			}
		}
		else if (___parameterCount != 1)
		{
			// open
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker0::Invoke(targetMethod, ___jointPoses0);
					else
						GenericVirtActionInvoker0::Invoke(targetMethod, ___jointPoses0);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker0::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), ___jointPoses0);
					else
						VirtActionInvoker0::Invoke(il2cpp_codegen_method_get_slot(targetMethod), ___jointPoses0);
				}
			}
			else
			{
				typedef void (*FunctionPointerType) (MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD*, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___jointPoses0, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD*, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___jointPoses0, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker1< MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD* >::Invoke(targetMethod, targetThis, ___jointPoses0);
					else
						GenericVirtActionInvoker1< MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD* >::Invoke(targetMethod, targetThis, ___jointPoses0);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker1< MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD* >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___jointPoses0);
					else
						VirtActionInvoker1< MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD* >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___jointPoses0);
				}
			}
			else
			{
				typedef void (*FunctionPointerType) (void*, MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD*, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___jointPoses0, targetMethod);
			}
		}
	}
}
// System.IAsyncResult Microsoft.MixedReality.Toolkit.Input.SimulatedHandData_HandJointDataGenerator::BeginInvoke(Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose[],System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* HandJointDataGenerator_BeginInvoke_mA4657EBE145331D04C470650EAF342F7C67A646F (HandJointDataGenerator_t70BF622884D5C475C85D34FDE76FD298FAC37955 * __this, MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD* ___jointPoses0, AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4 * ___callback1, RuntimeObject * ___object2, const RuntimeMethod* method)
{
	void *__d_args[2] = {0};
	__d_args[0] = ___jointPoses0;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback1, (RuntimeObject*)___object2);
}
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedHandData_HandJointDataGenerator::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void HandJointDataGenerator_EndInvoke_m47C19E7BD246FFCEDFC1D4E6903DB18FAF1002F5 (HandJointDataGenerator_t70BF622884D5C475C85D34FDE76FD298FAC37955 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
{
	il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___result0, 0);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedHandUtils::CalculateJointRotations(Microsoft.MixedReality.Toolkit.Utilities.Handedness,UnityEngine.Vector3[],UnityEngine.Quaternion[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedHandUtils_CalculateJointRotations_mA0A1808305AB3D8B589A08E42F9155739D9221AE (uint8_t ___handedness0, Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* ___jointPositions1, QuaternionU5BU5D_t26EB10EEE89DD3EF913D52E8797FAB841F6F2AA3* ___jointOrientationsOut2, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedHandUtils_CalculateJointRotations_mA0A1808305AB3D8B589A08E42F9155739D9221AE_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* V_0 = NULL;
	int32_t V_1 = 0;
	int32_t V_2 = 0;
	int32_t V_3 = 0;
	int32_t V_4 = 0;
	int32_t V_5 = 0;
	int32_t V_6 = 0;
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_7;
	memset((&V_7), 0, sizeof(V_7));
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_8;
	memset((&V_8), 0, sizeof(V_8));
	Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  V_9;
	memset((&V_9), 0, sizeof(V_9));
	int32_t G_B5_0 = 0;
	int32_t G_B11_0 = 0;
	{
		// int[] jointsPerFinger = { 4, 5, 5, 5, 5 }; // thumb, index, middle, right, pinky
		Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* L_0 = (Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83*)(Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83*)SZArrayNew(Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83_il2cpp_TypeInfo_var, (uint32_t)5);
		Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* L_1 = L_0;
		RuntimeFieldHandle_t844BDF00E8E6FE69D9AEAA7657F09018B864F4EF  L_2 = { reinterpret_cast<intptr_t> (U3CPrivateImplementationDetailsU3E_t5D7196C8D3A7E05A50169A365F5A7B3B92600D14____6AF7EBB4A5EF5D7478981B4AA0BAD37788AAB1ED_0_FieldInfo_var) };
		RuntimeHelpers_InitializeArray_m29F50CDFEEE0AB868200291366253DD4737BC76A((RuntimeArray *)(RuntimeArray *)L_1, L_2, /*hidden argument*/NULL);
		V_0 = L_1;
		// for (int fingerIndex = 0; fingerIndex < numFingers; fingerIndex++)
		V_1 = 0;
		goto IL_00e1;
	}

IL_0019:
	{
		// int jointsCurrentFinger = jointsPerFinger[fingerIndex];
		Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* L_3 = V_0;
		int32_t L_4 = V_1;
		NullCheck(L_3);
		int32_t L_5 = L_4;
		int32_t L_6 = (L_3)->GetAt(static_cast<il2cpp_array_size_t>(L_5));
		V_2 = L_6;
		// int lowIndex = (int)TrackedHandJoint.ThumbMetacarpalJoint + jointsPerFinger.Take(fingerIndex).Sum();
		Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* L_7 = V_0;
		int32_t L_8 = V_1;
		RuntimeObject* L_9 = Enumerable_Take_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_mCBED6C7F74DCC17FA9C923D11B6801F52FEEB61B((RuntimeObject*)(RuntimeObject*)L_7, L_8, /*hidden argument*/Enumerable_Take_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_mCBED6C7F74DCC17FA9C923D11B6801F52FEEB61B_RuntimeMethod_var);
		int32_t L_10 = Enumerable_Sum_mA81913DBCF3086B4716F692F9DB797D7DD6B7583(L_9, /*hidden argument*/NULL);
		V_3 = ((int32_t)il2cpp_codegen_add((int32_t)3, (int32_t)L_10));
		// int highIndex = lowIndex + jointsCurrentFinger - 1;
		int32_t L_11 = V_3;
		int32_t L_12 = V_2;
		V_4 = ((int32_t)il2cpp_codegen_subtract((int32_t)((int32_t)il2cpp_codegen_add((int32_t)L_11, (int32_t)L_12)), (int32_t)1));
		// for (int jointStartidx = lowIndex; jointStartidx <= highIndex; jointStartidx++)
		int32_t L_13 = V_3;
		V_5 = L_13;
		goto IL_00d4;
	}

IL_003b:
	{
		// int jointEndidx = jointStartidx == lowIndex ? (int)TrackedHandJoint.Wrist : jointStartidx - 1;
		int32_t L_14 = V_5;
		int32_t L_15 = V_3;
		if ((((int32_t)L_14) == ((int32_t)L_15)))
		{
			goto IL_0046;
		}
	}
	{
		int32_t L_16 = V_5;
		G_B5_0 = ((int32_t)il2cpp_codegen_subtract((int32_t)L_16, (int32_t)1));
		goto IL_0047;
	}

IL_0046:
	{
		G_B5_0 = 1;
	}

IL_0047:
	{
		V_6 = G_B5_0;
		// Vector3 boneForward = jointPositions[jointStartidx] - jointPositions[jointEndidx];
		Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* L_17 = ___jointPositions1;
		int32_t L_18 = V_5;
		NullCheck(L_17);
		int32_t L_19 = L_18;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_20 = (L_17)->GetAt(static_cast<il2cpp_array_size_t>(L_19));
		Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* L_21 = ___jointPositions1;
		int32_t L_22 = V_6;
		NullCheck(L_21);
		int32_t L_23 = L_22;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_24 = (L_21)->GetAt(static_cast<il2cpp_array_size_t>(L_23));
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_25 = Vector3_op_Subtraction_mF9846B723A5034F8B9F5F5DCB78E3D67649143D3(L_20, L_24, /*hidden argument*/NULL);
		V_7 = L_25;
		// Vector3 boneUp = Vector3.Cross(boneForward, GetPalmRightVector(handedness, jointPositions));
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_26 = V_7;
		uint8_t L_27 = ___handedness0;
		Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* L_28 = ___jointPositions1;
		IL2CPP_RUNTIME_CLASS_INIT(SimulatedHandUtils_t112B94E0F721072169327F6020348A7BB791A465_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_29 = SimulatedHandUtils_GetPalmRightVector_m9C646FB51F2C94823DC3EEE26383B22A88EA4301(L_27, L_28, /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_30 = Vector3_Cross_m3E9DBC445228FDB850BDBB4B01D6F61AC0111887(L_26, L_29, /*hidden argument*/NULL);
		V_8 = L_30;
		// if (boneForward.magnitude > float.Epsilon && boneUp.magnitude > float.Epsilon)
		float L_31 = Vector3_get_magnitude_m9A750659B60C5FE0C30438A7F9681775D5DB1274((Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *)(&V_7), /*hidden argument*/NULL);
		if ((!(((float)L_31) > ((float)(1.401298E-45f)))))
		{
			goto IL_00c1;
		}
	}
	{
		float L_32 = Vector3_get_magnitude_m9A750659B60C5FE0C30438A7F9681775D5DB1274((Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *)(&V_8), /*hidden argument*/NULL);
		if ((!(((float)L_32) > ((float)(1.401298E-45f)))))
		{
			goto IL_00c1;
		}
	}
	{
		// Quaternion jointRotation = Quaternion.LookRotation(boneForward, boneUp);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_33 = V_7;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_34 = V_8;
		IL2CPP_RUNTIME_CLASS_INIT(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_il2cpp_TypeInfo_var);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_35 = Quaternion_LookRotation_m7BED8FBB457FF073F183AC7962264E5110794672(L_33, L_34, /*hidden argument*/NULL);
		V_9 = L_35;
		// if (fingerIndex == 0)
		int32_t L_36 = V_1;
		if (L_36)
		{
			goto IL_00b5;
		}
	}
	{
		// Quaternion rotateThumb90 = Quaternion.AngleAxis(handedness == Handedness.Left ? -90 : 90, boneForward);
		uint8_t L_37 = ___handedness0;
		if ((((int32_t)L_37) == ((int32_t)1)))
		{
			goto IL_00a2;
		}
	}
	{
		G_B11_0 = ((int32_t)90);
		goto IL_00a4;
	}

IL_00a2:
	{
		G_B11_0 = ((int32_t)-90);
	}

IL_00a4:
	{
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_38 = V_7;
		IL2CPP_RUNTIME_CLASS_INIT(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_il2cpp_TypeInfo_var);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_39 = Quaternion_AngleAxis_m07DACF59F0403451DABB9BC991C53EE3301E88B0((((float)((float)G_B11_0))), L_38, /*hidden argument*/NULL);
		// jointRotation = rotateThumb90 * jointRotation;
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_40 = V_9;
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_41 = Quaternion_op_Multiply_mDB9F738AA8160E3D85549F4FEDA23BC658B5A790(L_39, L_40, /*hidden argument*/NULL);
		V_9 = L_41;
	}

IL_00b5:
	{
		// jointOrientationsOut[jointStartidx] = jointRotation;
		QuaternionU5BU5D_t26EB10EEE89DD3EF913D52E8797FAB841F6F2AA3* L_42 = ___jointOrientationsOut2;
		int32_t L_43 = V_5;
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_44 = V_9;
		NullCheck(L_42);
		(L_42)->SetAt(static_cast<il2cpp_array_size_t>(L_43), (Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 )L_44);
		// }
		goto IL_00ce;
	}

IL_00c1:
	{
		// jointOrientationsOut[jointStartidx] = Quaternion.identity;
		QuaternionU5BU5D_t26EB10EEE89DD3EF913D52E8797FAB841F6F2AA3* L_45 = ___jointOrientationsOut2;
		int32_t L_46 = V_5;
		IL2CPP_RUNTIME_CLASS_INIT(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_il2cpp_TypeInfo_var);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_47 = Quaternion_get_identity_m548B37D80F2DEE60E41D1F09BF6889B557BE1A64(/*hidden argument*/NULL);
		NullCheck(L_45);
		(L_45)->SetAt(static_cast<il2cpp_array_size_t>(L_46), (Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 )L_47);
	}

IL_00ce:
	{
		// for (int jointStartidx = lowIndex; jointStartidx <= highIndex; jointStartidx++)
		int32_t L_48 = V_5;
		V_5 = ((int32_t)il2cpp_codegen_add((int32_t)L_48, (int32_t)1));
	}

IL_00d4:
	{
		// for (int jointStartidx = lowIndex; jointStartidx <= highIndex; jointStartidx++)
		int32_t L_49 = V_5;
		int32_t L_50 = V_4;
		if ((((int32_t)L_49) <= ((int32_t)L_50)))
		{
			goto IL_003b;
		}
	}
	{
		// for (int fingerIndex = 0; fingerIndex < numFingers; fingerIndex++)
		int32_t L_51 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_51, (int32_t)1));
	}

IL_00e1:
	{
		// for (int fingerIndex = 0; fingerIndex < numFingers; fingerIndex++)
		int32_t L_52 = V_1;
		if ((((int32_t)L_52) < ((int32_t)5)))
		{
			goto IL_0019;
		}
	}
	{
		// jointOrientationsOut[(int)TrackedHandJoint.Palm] = Quaternion.LookRotation(GetPalmForwardVector(jointPositions), GetPalmUpVector(handedness, jointPositions));
		QuaternionU5BU5D_t26EB10EEE89DD3EF913D52E8797FAB841F6F2AA3* L_53 = ___jointOrientationsOut2;
		Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* L_54 = ___jointPositions1;
		IL2CPP_RUNTIME_CLASS_INIT(SimulatedHandUtils_t112B94E0F721072169327F6020348A7BB791A465_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_55 = SimulatedHandUtils_GetPalmForwardVector_m9E069A581F41648ADB1D947EDBB726BD867602F4(L_54, /*hidden argument*/NULL);
		uint8_t L_56 = ___handedness0;
		Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* L_57 = ___jointPositions1;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_58 = SimulatedHandUtils_GetPalmUpVector_mB1852A38F5919EC805FE801DB47DC6DA1E64CCD0(L_56, L_57, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_il2cpp_TypeInfo_var);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_59 = Quaternion_LookRotation_m7BED8FBB457FF073F183AC7962264E5110794672(L_55, L_58, /*hidden argument*/NULL);
		NullCheck(L_53);
		(L_53)->SetAt(static_cast<il2cpp_array_size_t>(2), (Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 )L_59);
		// }
		return;
	}
}
// UnityEngine.Vector3 Microsoft.MixedReality.Toolkit.Input.SimulatedHandUtils::GetPalmForwardVector(UnityEngine.Vector3[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  SimulatedHandUtils_GetPalmForwardVector_m9E069A581F41648ADB1D947EDBB726BD867602F4 (Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* ___jointPositions0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedHandUtils_GetPalmForwardVector_m9E069A581F41648ADB1D947EDBB726BD867602F4_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_0;
	memset((&V_0), 0, sizeof(V_0));
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		// Vector3 indexBase = jointPositions[(int)TrackedHandJoint.IndexKnuckle];
		Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* L_0 = ___jointPositions0;
		NullCheck(L_0);
		int32_t L_1 = 8;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_2 = (L_0)->GetAt(static_cast<il2cpp_array_size_t>(L_1));
		// Vector3 thumbMetaCarpal = jointPositions[(int)TrackedHandJoint.ThumbMetacarpalJoint];
		Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* L_3 = ___jointPositions0;
		NullCheck(L_3);
		int32_t L_4 = 3;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_5 = (L_3)->GetAt(static_cast<il2cpp_array_size_t>(L_4));
		V_0 = L_5;
		// Vector3 thumbMetaCarpalToIndex = indexBase - thumbMetaCarpal;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_6 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_7 = Vector3_op_Subtraction_mF9846B723A5034F8B9F5F5DCB78E3D67649143D3(L_2, L_6, /*hidden argument*/NULL);
		V_1 = L_7;
		// return thumbMetaCarpalToIndex.normalized;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_8 = Vector3_get_normalized_mE20796F1D2D36244FACD4D14DADB245BE579849B((Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *)(&V_1), /*hidden argument*/NULL);
		return L_8;
	}
}
// UnityEngine.Vector3 Microsoft.MixedReality.Toolkit.Input.SimulatedHandUtils::GetPalmUpVector(Microsoft.MixedReality.Toolkit.Utilities.Handedness,UnityEngine.Vector3[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  SimulatedHandUtils_GetPalmUpVector_mB1852A38F5919EC805FE801DB47DC6DA1E64CCD0 (uint8_t ___handedness0, Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* ___jointPositions1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedHandUtils_GetPalmUpVector_mB1852A38F5919EC805FE801DB47DC6DA1E64CCD0_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_0;
	memset((&V_0), 0, sizeof(V_0));
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_1;
	memset((&V_1), 0, sizeof(V_1));
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_2;
	memset((&V_2), 0, sizeof(V_2));
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_3;
	memset((&V_3), 0, sizeof(V_3));
	{
		// Vector3 indexBase = jointPositions[(int)TrackedHandJoint.IndexKnuckle];
		Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* L_0 = ___jointPositions1;
		NullCheck(L_0);
		int32_t L_1 = 8;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_2 = (L_0)->GetAt(static_cast<il2cpp_array_size_t>(L_1));
		// Vector3 pinkyBase = jointPositions[(int)TrackedHandJoint.PinkyKnuckle];
		Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* L_3 = ___jointPositions1;
		NullCheck(L_3);
		int32_t L_4 = ((int32_t)23);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_5 = (L_3)->GetAt(static_cast<il2cpp_array_size_t>(L_4));
		// Vector3 ThumbMetaCarpal = jointPositions[(int)TrackedHandJoint.ThumbMetacarpalJoint];
		Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* L_6 = ___jointPositions1;
		NullCheck(L_6);
		int32_t L_7 = 3;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_8 = (L_6)->GetAt(static_cast<il2cpp_array_size_t>(L_7));
		V_0 = L_8;
		// Vector3 ThumbMetaCarpalToPinky = pinkyBase - ThumbMetaCarpal;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_9 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_10 = Vector3_op_Subtraction_mF9846B723A5034F8B9F5F5DCB78E3D67649143D3(L_5, L_9, /*hidden argument*/NULL);
		V_1 = L_10;
		// Vector3 ThumbMetaCarpalToIndex = indexBase - ThumbMetaCarpal;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_11 = V_0;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_12 = Vector3_op_Subtraction_mF9846B723A5034F8B9F5F5DCB78E3D67649143D3(L_2, L_11, /*hidden argument*/NULL);
		V_2 = L_12;
		// if (handedness == Handedness.Left)
		uint8_t L_13 = ___handedness0;
		if ((!(((uint32_t)L_13) == ((uint32_t)1))))
		{
			goto IL_0039;
		}
	}
	{
		// return Vector3.Cross(ThumbMetaCarpalToPinky, ThumbMetaCarpalToIndex).normalized;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_14 = V_1;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_15 = V_2;
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_16 = Vector3_Cross_m3E9DBC445228FDB850BDBB4B01D6F61AC0111887(L_14, L_15, /*hidden argument*/NULL);
		V_3 = L_16;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_17 = Vector3_get_normalized_mE20796F1D2D36244FACD4D14DADB245BE579849B((Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *)(&V_3), /*hidden argument*/NULL);
		return L_17;
	}

IL_0039:
	{
		// return Vector3.Cross(ThumbMetaCarpalToIndex, ThumbMetaCarpalToPinky).normalized;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_18 = V_2;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_19 = V_1;
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_20 = Vector3_Cross_m3E9DBC445228FDB850BDBB4B01D6F61AC0111887(L_18, L_19, /*hidden argument*/NULL);
		V_3 = L_20;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_21 = Vector3_get_normalized_mE20796F1D2D36244FACD4D14DADB245BE579849B((Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *)(&V_3), /*hidden argument*/NULL);
		return L_21;
	}
}
// UnityEngine.Vector3 Microsoft.MixedReality.Toolkit.Input.SimulatedHandUtils::GetPalmRightVector(Microsoft.MixedReality.Toolkit.Utilities.Handedness,UnityEngine.Vector3[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  SimulatedHandUtils_GetPalmRightVector_m9C646FB51F2C94823DC3EEE26383B22A88EA4301 (uint8_t ___handedness0, Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* ___jointPositions1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedHandUtils_GetPalmRightVector_m9C646FB51F2C94823DC3EEE26383B22A88EA4301_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_0;
	memset((&V_0), 0, sizeof(V_0));
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_1;
	memset((&V_1), 0, sizeof(V_1));
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_2;
	memset((&V_2), 0, sizeof(V_2));
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_3;
	memset((&V_3), 0, sizeof(V_3));
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_4;
	memset((&V_4), 0, sizeof(V_4));
	{
		// Vector3 indexBase = jointPositions[(int)TrackedHandJoint.IndexKnuckle];
		Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* L_0 = ___jointPositions1;
		NullCheck(L_0);
		int32_t L_1 = 8;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_2 = (L_0)->GetAt(static_cast<il2cpp_array_size_t>(L_1));
		// Vector3 pinkyBase = jointPositions[(int)TrackedHandJoint.PinkyKnuckle];
		Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* L_3 = ___jointPositions1;
		NullCheck(L_3);
		int32_t L_4 = ((int32_t)23);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_5 = (L_3)->GetAt(static_cast<il2cpp_array_size_t>(L_4));
		// Vector3 thumbMetaCarpal = jointPositions[(int)TrackedHandJoint.ThumbMetacarpalJoint];
		Vector3U5BU5D_tB9EC3346CC4A0EA5447D968E84A9AC1F6F372C28* L_6 = ___jointPositions1;
		NullCheck(L_6);
		int32_t L_7 = 3;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_8 = (L_6)->GetAt(static_cast<il2cpp_array_size_t>(L_7));
		V_0 = L_8;
		// Vector3 thumbMetaCarpalToPinky = pinkyBase - thumbMetaCarpal;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_9 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_10 = Vector3_op_Subtraction_mF9846B723A5034F8B9F5F5DCB78E3D67649143D3(L_5, L_9, /*hidden argument*/NULL);
		V_1 = L_10;
		// Vector3 thumbMetaCarpalToIndex = indexBase - thumbMetaCarpal;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_11 = V_0;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_12 = Vector3_op_Subtraction_mF9846B723A5034F8B9F5F5DCB78E3D67649143D3(L_2, L_11, /*hidden argument*/NULL);
		V_2 = L_12;
		// Vector3 thumbMetaCarpalUp = Vector3.zero;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_13 = Vector3_get_zero_m3CDDCAE94581DF3BB16C4B40A100E28E9C6649C2(/*hidden argument*/NULL);
		V_3 = L_13;
		// if (handedness == Handedness.Left)
		uint8_t L_14 = ___handedness0;
		if ((!(((uint32_t)L_14) == ((uint32_t)1))))
		{
			goto IL_0042;
		}
	}
	{
		// thumbMetaCarpalUp = Vector3.Cross(thumbMetaCarpalToPinky, thumbMetaCarpalToIndex).normalized;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_15 = V_1;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_16 = V_2;
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_17 = Vector3_Cross_m3E9DBC445228FDB850BDBB4B01D6F61AC0111887(L_15, L_16, /*hidden argument*/NULL);
		V_4 = L_17;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_18 = Vector3_get_normalized_mE20796F1D2D36244FACD4D14DADB245BE579849B((Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *)(&V_4), /*hidden argument*/NULL);
		V_3 = L_18;
		// }
		goto IL_0053;
	}

IL_0042:
	{
		// thumbMetaCarpalUp = Vector3.Cross(thumbMetaCarpalToIndex, thumbMetaCarpalToPinky).normalized;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_19 = V_2;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_20 = V_1;
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_21 = Vector3_Cross_m3E9DBC445228FDB850BDBB4B01D6F61AC0111887(L_19, L_20, /*hidden argument*/NULL);
		V_4 = L_21;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_22 = Vector3_get_normalized_mE20796F1D2D36244FACD4D14DADB245BE579849B((Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *)(&V_4), /*hidden argument*/NULL);
		V_3 = L_22;
	}

IL_0053:
	{
		// return Vector3.Cross(thumbMetaCarpalUp, thumbMetaCarpalToIndex).normalized;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_23 = V_3;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_24 = V_2;
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_25 = Vector3_Cross_m3E9DBC445228FDB850BDBB4B01D6F61AC0111887(L_23, L_24, /*hidden argument*/NULL);
		V_4 = L_25;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_26 = Vector3_get_normalized_mE20796F1D2D36244FACD4D14DADB245BE579849B((Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *)(&V_4), /*hidden argument*/NULL);
		return L_26;
	}
}
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedHandUtils::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedHandUtils__ctor_m8FE7B6098201AE8BB6E8337DCDAAA7663D64F06F (SimulatedHandUtils_t112B94E0F721072169327F6020348A7BB791A465 * __this, const RuntimeMethod* method)
{
	{
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		return;
	}
}
// System.Void Microsoft.MixedReality.Toolkit.Input.SimulatedHandUtils::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SimulatedHandUtils__cctor_mE9EC43A15625808EECB51ECE0AA4C867F45C6733 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SimulatedHandUtils__cctor_mE9EC43A15625808EECB51ECE0AA4C867F45C6733_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_0 = { reinterpret_cast<intptr_t> (TrackedHandJoint_tDE2FD40782A5B0C1D39386D6BF70D8A1CCF94E22_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_1 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_0, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Enum_t2AF27C02B8653AE29442467390005ABC74D8F521_il2cpp_TypeInfo_var);
		StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* L_2 = Enum_GetNames_m9ECDF3E80A7A31075D7D2B2B362DDCC6150BC15C(L_1, /*hidden argument*/NULL);
		NullCheck(L_2);
		((SimulatedHandUtils_t112B94E0F721072169327F6020348A7BB791A465_StaticFields*)il2cpp_codegen_static_fields_for(SimulatedHandUtils_t112B94E0F721072169327F6020348A7BB791A465_il2cpp_TypeInfo_var))->set_jointCount_0((((int32_t)((int32_t)(((RuntimeArray*)L_2)->max_length)))));
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t KeyBinding_get_BindingType_mA6915A48809778FE77561961A250F3D5BEABFE91_inline (KeyBinding_tB411D21A41BE54262ECD35999E2324DFF1C6ED79 * __this, const RuntimeMethod* method)
{
	{
		// public KeyType BindingType => bindingType;
		int32_t L_0 = __this->get_bindingType_3();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  MixedRealityPose_get_ZeroIdentity_m80C016329EAADDC4EB8DFD80ED0CF614A5E547AD_inline (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (MixedRealityPose_get_ZeroIdentity_m80C016329EAADDC4EB8DFD80ED0CF614A5E547ADMicrosoft_MixedReality_Toolkit_Services_InputSimulation_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public static MixedRealityPose ZeroIdentity { get; } = new MixedRealityPose(Vector3.zero, Quaternion.identity);
		IL2CPP_RUNTIME_CLASS_INIT(MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45_il2cpp_TypeInfo_var);
		MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45  L_0 = ((MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45_StaticFields*)il2cpp_codegen_static_fields_for(MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45_il2cpp_TypeInfo_var))->get_U3CZeroIdentityU3Ek__BackingField_0();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  MixedRealityInputAction_get_None_m0276CF8988B0670DCCE381865DD5190010A2A8BF_inline (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (MixedRealityInputAction_get_None_m0276CF8988B0670DCCE381865DD5190010A2A8BFMicrosoft_MixedReality_Toolkit_Services_InputSimulation_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public static MixedRealityInputAction None { get; } = new MixedRealityInputAction(0, "None");
		IL2CPP_RUNTIME_CLASS_INIT(MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073_il2cpp_TypeInfo_var);
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_0 = ((MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073_StaticFields*)il2cpp_codegen_static_fields_for(MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073_il2cpp_TypeInfo_var))->get_U3CNoneU3Ek__BackingField_0();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  MixedRealityPose_get_Position_mF175BAE3270E5432E605BDD5FD1FA5F722B24AEE_inline (MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * __this, const RuntimeMethod* method)
{
	{
		// public Vector3 Position { get { return position; } set { position = value; } }
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_0 = __this->get_position_1();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR void BaseController_set_IsRotationAvailable_m5259A799822AFD94A2BEE4B47F887A03158FE308_inline (BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B * __this, bool ___value0, const RuntimeMethod* method)
{
	{
		// public bool IsRotationAvailable { get; protected set; }
		bool L_0 = ___value0;
		__this->set_U3CIsRotationAvailableU3Ek__BackingField_11(L_0);
		return;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR void BaseController_set_IsPositionAvailable_m76D7FB5DBF945174A9D9B7A19123783742C6B57F_inline (BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B * __this, bool ___value0, const RuntimeMethod* method)
{
	{
		// public bool IsPositionAvailable { get; protected set; }
		bool L_0 = ___value0;
		__this->set_U3CIsPositionAvailableU3Ek__BackingField_9(L_0);
		return;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR bool BaseController_get_IsPositionAvailable_m3E2EB0D15AAADABB3D967535353AD53539677046_inline (BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B * __this, const RuntimeMethod* method)
{
	{
		// public bool IsPositionAvailable { get; protected set; }
		bool L_0 = __this->get_U3CIsPositionAvailableU3Ek__BackingField_9();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR HandRay_t9DAE3FE243DBED1BAA1B9A4F782C3F1C9E6AE285 * BaseHand_get_HandRay_mDB7145BE29023110AF5EC4037ABE75660776680F_inline (BaseHand_tB58ECFC99FBFD516BBAA0989004A10F687078F4B * __this, const RuntimeMethod* method)
{
	{
		// protected HandRay HandRay { get; } = new HandRay();
		HandRay_t9DAE3FE243DBED1BAA1B9A4F782C3F1C9E6AE285 * L_0 = __this->get_U3CHandRayU3Ek__BackingField_15();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR uint8_t BaseController_get_ControllerHandedness_mA18814111E1328E1C7C04C383CC44E8A2F8A995A_inline (BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B * __this, const RuntimeMethod* method)
{
	{
		// public Handedness ControllerHandedness { get; }
		uint8_t L_0 = __this->get_U3CControllerHandednessU3Ek__BackingField_6();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR void MixedRealityPose_set_Position_m28EBD523337BC95684EFC016980F3862DE763759_inline (MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___value0, const RuntimeMethod* method)
{
	{
		// public Vector3 Position { get { return position; } set { position = value; } }
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_0 = ___value0;
		__this->set_position_1(L_0);
		// public Vector3 Position { get { return position; } set { position = value; } }
		return;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR void MixedRealityPose_set_Rotation_m1AC620BE37B8F415170D725902EE1C3A92ECC19B_inline (MixedRealityPose_tB91C13927D4C609825580E7DACDB4A550F3F0F45 * __this, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___value0, const RuntimeMethod* method)
{
	{
		// public Quaternion Rotation { get { return rotation; } set { rotation = value; } }
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_0 = ___value0;
		__this->set_rotation_2(L_0);
		// public Quaternion Rotation { get { return rotation; } set { rotation = value; } }
		return;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR bool BaseController_get_IsRotationAvailable_m59D5E1DD267C83A3DB834096028590522C934868_inline (BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B * __this, const RuntimeMethod* method)
{
	{
		// public bool IsRotationAvailable { get; protected set; }
		bool L_0 = __this->get_U3CIsRotationAvailableU3Ek__BackingField_11();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR RuntimeObject* BaseController_get_InputSource_m9F9D70F24AC4D5605665D31F6D8A6083A3CA1CFD_inline (BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B * __this, const RuntimeMethod* method)
{
	{
		// public IMixedRealityInputSource InputSource { get; }
		RuntimeObject* L_0 = __this->get_U3CInputSourceU3Ek__BackingField_7();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* BaseController_get_Interactions_mC6BB2DCE6BB5806FB3AEA325A55FB53BD7D3C561_inline (BaseController_t3529EF2CB2E73206F555D8AF9468309DFF9B1E9B * __this, const RuntimeMethod* method)
{
	{
		// public MixedRealityInteractionMapping[] Interactions { get; private set; } = null;
		MixedRealityInteractionMappingU5BU5D_tA9021B8F5A4C53A970615CF32CF4B0992DEFB4FA* L_0 = __this->get_U3CInteractionsU3Ek__BackingField_12();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t MixedRealityInteractionMapping_get_InputType_mA8C027545479C380F87D72BDED734A9BDBFA40CD_inline (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * __this, const RuntimeMethod* method)
{
	{
		// public DeviceInputType InputType => inputType;
		int32_t L_0 = __this->get_inputType_3();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  MixedRealityInteractionMapping_get_MixedRealityInputAction_mA22FF2AC6237AEF7B9EADF4461EB3B484CCB995E_inline (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * __this, const RuntimeMethod* method)
{
	{
		// get { return inputAction; }
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_0 = __this->get_inputAction_4();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR bool SimulatedHandData_get_IsPinching_mB7C40888399E88C93E755FE89D50234CF5F5C981_inline (SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * __this, const RuntimeMethod* method)
{
	{
		// public bool IsPinching => isPinching;
		bool L_0 = __this->get_isPinching_3();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR bool MixedRealityInteractionMapping_get_BoolData_mB42A4C428B73C25DC7FE9CAC463325E19255F71B_inline (MixedRealityInteractionMapping_tF40535F5D25A7AEA688519D1A5674324B999CAE2 * __this, const RuntimeMethod* method)
{
	{
		// return boolData;
		bool L_0 = __this->get_boolData_12();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725 * MixedRealityInputSystemProfile_get_GesturesProfile_mA8F275BA8A5AE96D3A95350F698A7343D72E5129_inline (MixedRealityInputSystemProfile_tE6382BBDB73ACDFF6F3D0C3B4AD9B1B7F2D5BAC2 * __this, const RuntimeMethod* method)
{
	{
		// get { return gesturesProfile; }
		MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725 * L_0 = __this->get_gesturesProfile_13();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR MixedRealityGestureMappingU5BU5D_t2F3D7B685E29F06002C6BD2EF99A97C8DF6BD874* MixedRealityGesturesProfile_get_Gestures_mBAB7F3737E09478B3FA7F30ECAC24D6840E98580_inline (MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725 * __this, const RuntimeMethod* method)
{
	{
		// public MixedRealityGestureMapping[] Gestures => gestures;
		MixedRealityGestureMappingU5BU5D_t2F3D7B685E29F06002C6BD2EF99A97C8DF6BD874* L_0 = __this->get_gestures_10();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t MixedRealityGestureMapping_get_GestureType_m6798792581776B818AF6A5307DD72D3425420C20_inline (MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074 * __this, const RuntimeMethod* method)
{
	{
		// public GestureInputType GestureType => gestureType;
		int32_t L_0 = __this->get_gestureType_1();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  MixedRealityGestureMapping_get_Action_mF225EE997BA38AFC7DCCA99F71434633FD683D82_inline (MixedRealityGestureMapping_t765237603301D949A532A3533D70FB492A6E3074 * __this, const RuntimeMethod* method)
{
	{
		// public MixedRealityInputAction Action => action;
		MixedRealityInputAction_tF3298AB582C6E52C2107F4AC4E6E4381EA0A5073  L_0 = __this->get_action_2();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR bool MixedRealityGesturesProfile_get_UseRailsNavigation_mEAE6D30B9C69C0E5EA8115068FDA600F87CE02C6_inline (MixedRealityGesturesProfile_t9CC7974AD508EC596BC2FD0C5D3807CA076D7725 * __this, const RuntimeMethod* method)
{
	{
		// public bool UseRailsNavigation => useRailsNavigation;
		bool L_0 = __this->get_useRailsNavigation_7();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR float MixedRealityInputSimulationProfile_get_HoldStartDuration_mBC1A3E5C22D4854356392379561E246374610007_inline (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public float HoldStartDuration => holdStartDuration;
		float L_0 = __this->get_holdStartDuration_39();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR float MixedRealityInputSimulationProfile_get_NavigationStartThreshold_m30BD08DA409E73AE42567F6420EB5E92DC7981E4_inline (MixedRealityInputSimulationProfile_t752581F6963049D1D77F1BB3E533640527CD7977 * __this, const RuntimeMethod* method)
{
	{
		// public float NavigationStartThreshold => navigationStartThreshold;
		float L_0 = __this->get_navigationStartThreshold_40();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD* SimulatedHandData_get_Joints_m0137F96239589766E8132147EBBC5D1C24516B7C_inline (SimulatedHandData_t414B6A5A422CE06387BF5DB28CCAF451A21FCBA1 * __this, const RuntimeMethod* method)
{
	{
		// public MixedRealityPose[] Joints => joints;
		MixedRealityPoseU5BU5D_t9A8494A57EE87642D3A570AB9C476CE039C529BD* L_0 = __this->get_joints_2();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR int32_t Nullable_1_GetValueOrDefault_mE89BB8F302DF31EE202251F4746859285860B6B6_gshared_inline (Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB * __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = (int32_t)__this->get_value_0();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR bool Nullable_1_get_HasValue_mB664E2C41CADA8413EF8842E6601B8C696A7CE15_gshared_inline (Nullable_1_t0D03270832B3FFDDC0E7C2D89D4A0EA25376A1EB * __this, const RuntimeMethod* method)
{
	{
		bool L_0 = (bool)__this->get_has_value_1();
		return L_0;
	}
}
