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

template <typename T1, typename T2, typename T3, typename T4>
struct VirtActionInvoker4
{
	typedef void (*Action)(void*, T1, T2, T3, T4, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, p4, invokeData.method);
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
template <typename T1, typename T2, typename T3, typename T4>
struct GenericVirtActionInvoker4
{
	typedef void (*Action)(void*, T1, T2, T3, T4, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, p4, invokeData.method);
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
struct InterfaceActionInvoker0
{
	typedef void (*Action)(void*, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename T1, typename T2, typename T3, typename T4>
struct GenericInterfaceActionInvoker4
{
	typedef void (*Action)(void*, T1, T2, T3, T4, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, p4, invokeData.method);
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

// System.Action
struct Action_t591D2A86165F896B4B800BB5C25CE18672A55579;
// System.Action`1<System.Boolean>
struct Action_1_tAA0F894C98302D68F7D5034E8104E9AB4763CCAD;
// System.Action`3<System.Boolean,System.Boolean,System.Int32>
struct Action_3_tEE1FB0623176AFA5109FAA9BA7E82293445CAE1E;
// System.AsyncCallback
struct AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4;
// System.Char[]
struct CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2;
// System.Delegate
struct Delegate_t;
// System.DelegateData
struct DelegateData_t1BF9F691B56DAE5F8C28C5E084FDE94F15F27BBE;
// System.Delegate[]
struct DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86;
// System.IAsyncResult
struct IAsyncResult_t8E194308510B375B42432981AE5E7488C458D598;
// System.Reflection.MethodInfo
struct MethodInfo_t;
// System.String
struct String_t;
// System.Void
struct Void_t22962CB4C05B1D89B55A6E1139F0E87A90987017;
// UnityEngine.Analytics.AnalyticsSessionInfo/SessionStateChanged
struct SessionStateChanged_t9084549A636BD45086D66CC6765DA8C3DD31066F;
// UnityEngine.RemoteConfigSettings
struct RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A;
// UnityEngine.RemoteSettings/UpdatedEventHandler
struct UpdatedEventHandler_tB0230BC83686D7126AB4D3800A66351028CA514F;

IL2CPP_EXTERN_C RuntimeClass* Action_1_tAA0F894C98302D68F7D5034E8104E9AB4763CCAD_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* AnalyticsSessionInfo_tE075F764A74D2B095CFD57F3B179397F504B7D8C_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* AnalyticsSessionState_t61CA873937E9A3B881B71B32F518A954A4C8F267_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Boolean_tB53F6830F670160873277339AA58F15CAED4399C_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Int64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IntPtr_t_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* RemoteSettings_t3F7E07D15288B0DF84A4A32044592D8AFA6D36ED_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C const RuntimeMethod* Action_1_Invoke_m45E8F9900F9DB395C48A868A7C6A83BDD7FC692F_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Action_3_Invoke_m8A4240174E60397816F6EACC65FE1E27F4275FA5_RuntimeMethod_var;
IL2CPP_EXTERN_C const uint32_t AnalyticsSessionInfo_CallSessionStateChanged_mB5E3A42407B97BD8A6A4827FADDA91DB85B0F294_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t RemoteConfigSettings_Destroy_m91215D772DF190B8AD9A73C8165DEDBE129C7A10_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t RemoteConfigSettings_RemoteConfigSettingsUpdated_mF72E8C9F99B752FEA9EA388CCC1EFECC24733504_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A_com_FromNativeMethodDefinition_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A_pinvoke_FromNativeMethodDefinition_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t RemoteSettings_RemoteSettingsBeforeFetchFromServer_mBF1BEF2ACAFE13AE7B820FCA5CB7B81CF614EF11_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t RemoteSettings_RemoteSettingsUpdateCompleted_m6C6C3568C5A8F319CD06F8303111F06AB3718A02_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t RemoteSettings_RemoteSettingsUpdated_mD5427A08A622FDAA576A3D9CF05AD4C7CC6102F5_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SessionStateChanged_BeginInvoke_mD75E6832EC29F8B01CEEAAACD5DC54322F03D4B3_MetadataUsageId;
struct Delegate_t_marshaled_com;
struct Delegate_t_marshaled_pinvoke;

struct DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86;

IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// <Module>
struct  U3CModuleU3E_tCD4309F8DDA0F37A98DBCDFE49F6C8F300C242B0 
{
public:

public:
};


// System.Object

struct Il2CppArrayBounds;

// System.Array


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

// UnityEngine.Analytics.AnalyticsSessionInfo
struct  AnalyticsSessionInfo_tE075F764A74D2B095CFD57F3B179397F504B7D8C  : public RuntimeObject
{
public:

public:
};

struct AnalyticsSessionInfo_tE075F764A74D2B095CFD57F3B179397F504B7D8C_StaticFields
{
public:
	// UnityEngine.Analytics.AnalyticsSessionInfo_SessionStateChanged UnityEngine.Analytics.AnalyticsSessionInfo::sessionStateChanged
	SessionStateChanged_t9084549A636BD45086D66CC6765DA8C3DD31066F * ___sessionStateChanged_0;

public:
	inline static int32_t get_offset_of_sessionStateChanged_0() { return static_cast<int32_t>(offsetof(AnalyticsSessionInfo_tE075F764A74D2B095CFD57F3B179397F504B7D8C_StaticFields, ___sessionStateChanged_0)); }
	inline SessionStateChanged_t9084549A636BD45086D66CC6765DA8C3DD31066F * get_sessionStateChanged_0() const { return ___sessionStateChanged_0; }
	inline SessionStateChanged_t9084549A636BD45086D66CC6765DA8C3DD31066F ** get_address_of_sessionStateChanged_0() { return &___sessionStateChanged_0; }
	inline void set_sessionStateChanged_0(SessionStateChanged_t9084549A636BD45086D66CC6765DA8C3DD31066F * value)
	{
		___sessionStateChanged_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___sessionStateChanged_0), (void*)value);
	}
};


// UnityEngine.Analytics.ContinuousEvent
struct  ContinuousEvent_tBAB6336255F3FC327CBA03CE368CD4D8D027107A  : public RuntimeObject
{
public:

public:
};


// UnityEngine.RemoteConfigSettingsHelper
struct  RemoteConfigSettingsHelper_t2F0887933AC2724EEA806E32E9147A493C0A33BE  : public RuntimeObject
{
public:

public:
};


// UnityEngine.RemoteSettings
struct  RemoteSettings_t3F7E07D15288B0DF84A4A32044592D8AFA6D36ED  : public RuntimeObject
{
public:

public:
};

struct RemoteSettings_t3F7E07D15288B0DF84A4A32044592D8AFA6D36ED_StaticFields
{
public:
	// UnityEngine.RemoteSettings_UpdatedEventHandler UnityEngine.RemoteSettings::Updated
	UpdatedEventHandler_tB0230BC83686D7126AB4D3800A66351028CA514F * ___Updated_0;
	// System.Action UnityEngine.RemoteSettings::BeforeFetchFromServer
	Action_t591D2A86165F896B4B800BB5C25CE18672A55579 * ___BeforeFetchFromServer_1;
	// System.Action`3<System.Boolean,System.Boolean,System.Int32> UnityEngine.RemoteSettings::Completed
	Action_3_tEE1FB0623176AFA5109FAA9BA7E82293445CAE1E * ___Completed_2;

public:
	inline static int32_t get_offset_of_Updated_0() { return static_cast<int32_t>(offsetof(RemoteSettings_t3F7E07D15288B0DF84A4A32044592D8AFA6D36ED_StaticFields, ___Updated_0)); }
	inline UpdatedEventHandler_tB0230BC83686D7126AB4D3800A66351028CA514F * get_Updated_0() const { return ___Updated_0; }
	inline UpdatedEventHandler_tB0230BC83686D7126AB4D3800A66351028CA514F ** get_address_of_Updated_0() { return &___Updated_0; }
	inline void set_Updated_0(UpdatedEventHandler_tB0230BC83686D7126AB4D3800A66351028CA514F * value)
	{
		___Updated_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___Updated_0), (void*)value);
	}

	inline static int32_t get_offset_of_BeforeFetchFromServer_1() { return static_cast<int32_t>(offsetof(RemoteSettings_t3F7E07D15288B0DF84A4A32044592D8AFA6D36ED_StaticFields, ___BeforeFetchFromServer_1)); }
	inline Action_t591D2A86165F896B4B800BB5C25CE18672A55579 * get_BeforeFetchFromServer_1() const { return ___BeforeFetchFromServer_1; }
	inline Action_t591D2A86165F896B4B800BB5C25CE18672A55579 ** get_address_of_BeforeFetchFromServer_1() { return &___BeforeFetchFromServer_1; }
	inline void set_BeforeFetchFromServer_1(Action_t591D2A86165F896B4B800BB5C25CE18672A55579 * value)
	{
		___BeforeFetchFromServer_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___BeforeFetchFromServer_1), (void*)value);
	}

	inline static int32_t get_offset_of_Completed_2() { return static_cast<int32_t>(offsetof(RemoteSettings_t3F7E07D15288B0DF84A4A32044592D8AFA6D36ED_StaticFields, ___Completed_2)); }
	inline Action_3_tEE1FB0623176AFA5109FAA9BA7E82293445CAE1E * get_Completed_2() const { return ___Completed_2; }
	inline Action_3_tEE1FB0623176AFA5109FAA9BA7E82293445CAE1E ** get_address_of_Completed_2() { return &___Completed_2; }
	inline void set_Completed_2(Action_3_tEE1FB0623176AFA5109FAA9BA7E82293445CAE1E * value)
	{
		___Completed_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___Completed_2), (void*)value);
	}
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


// System.Int64
struct  Int64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436 
{
public:
	// System.Int64 System.Int64::m_value
	int64_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Int64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436, ___m_value_0)); }
	inline int64_t get_m_value_0() const { return ___m_value_0; }
	inline int64_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(int64_t value)
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

// UnityEngine.Analytics.AnalyticsSessionState
struct  AnalyticsSessionState_t61CA873937E9A3B881B71B32F518A954A4C8F267 
{
public:
	// System.Int32 UnityEngine.Analytics.AnalyticsSessionState::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(AnalyticsSessionState_t61CA873937E9A3B881B71B32F518A954A4C8F267, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.RemoteConfigSettings
struct  RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A  : public RuntimeObject
{
public:
	// System.IntPtr UnityEngine.RemoteConfigSettings::m_Ptr
	intptr_t ___m_Ptr_0;
	// System.Action`1<System.Boolean> UnityEngine.RemoteConfigSettings::Updated
	Action_1_tAA0F894C98302D68F7D5034E8104E9AB4763CCAD * ___Updated_1;

public:
	inline static int32_t get_offset_of_m_Ptr_0() { return static_cast<int32_t>(offsetof(RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A, ___m_Ptr_0)); }
	inline intptr_t get_m_Ptr_0() const { return ___m_Ptr_0; }
	inline intptr_t* get_address_of_m_Ptr_0() { return &___m_Ptr_0; }
	inline void set_m_Ptr_0(intptr_t value)
	{
		___m_Ptr_0 = value;
	}

	inline static int32_t get_offset_of_Updated_1() { return static_cast<int32_t>(offsetof(RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A, ___Updated_1)); }
	inline Action_1_tAA0F894C98302D68F7D5034E8104E9AB4763CCAD * get_Updated_1() const { return ___Updated_1; }
	inline Action_1_tAA0F894C98302D68F7D5034E8104E9AB4763CCAD ** get_address_of_Updated_1() { return &___Updated_1; }
	inline void set_Updated_1(Action_1_tAA0F894C98302D68F7D5034E8104E9AB4763CCAD * value)
	{
		___Updated_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___Updated_1), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of UnityEngine.RemoteConfigSettings
struct RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A_marshaled_pinvoke
{
	intptr_t ___m_Ptr_0;
	Il2CppMethodPointer ___Updated_1;
};
// Native definition for COM marshalling of UnityEngine.RemoteConfigSettings
struct RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A_marshaled_com
{
	intptr_t ___m_Ptr_0;
	Il2CppMethodPointer ___Updated_1;
};

// UnityEngine.RemoteConfigSettingsHelper_Tag
struct  Tag_t7FB12BCA4EA470318C147C1AED37575DEB552307 
{
public:
	// System.Int32 UnityEngine.RemoteConfigSettingsHelper_Tag::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(Tag_t7FB12BCA4EA470318C147C1AED37575DEB552307, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
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

// System.Action
struct  Action_t591D2A86165F896B4B800BB5C25CE18672A55579  : public MulticastDelegate_t
{
public:

public:
};


// System.Action`1<System.Boolean>
struct  Action_1_tAA0F894C98302D68F7D5034E8104E9AB4763CCAD  : public MulticastDelegate_t
{
public:

public:
};


// System.Action`3<System.Boolean,System.Boolean,System.Int32>
struct  Action_3_tEE1FB0623176AFA5109FAA9BA7E82293445CAE1E  : public MulticastDelegate_t
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


// UnityEngine.Analytics.AnalyticsSessionInfo_SessionStateChanged
struct  SessionStateChanged_t9084549A636BD45086D66CC6765DA8C3DD31066F  : public MulticastDelegate_t
{
public:

public:
};


// UnityEngine.RemoteSettings_UpdatedEventHandler
struct  UpdatedEventHandler_tB0230BC83686D7126AB4D3800A66351028CA514F  : public MulticastDelegate_t
{
public:

public:
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
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


// System.Void System.Action`1<System.Boolean>::Invoke(!0)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Action_1_Invoke_m45E8F9900F9DB395C48A868A7C6A83BDD7FC692F_gshared (Action_1_tAA0F894C98302D68F7D5034E8104E9AB4763CCAD * __this, bool ___obj0, const RuntimeMethod* method);
// System.Void System.Action`3<System.Boolean,System.Boolean,System.Int32>::Invoke(!0,!1,!2)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Action_3_Invoke_m8A4240174E60397816F6EACC65FE1E27F4275FA5_gshared (Action_3_tEE1FB0623176AFA5109FAA9BA7E82293445CAE1E * __this, bool ___arg10, bool ___arg21, int32_t ___arg32, const RuntimeMethod* method);

// System.Void UnityEngine.Analytics.AnalyticsSessionInfo/SessionStateChanged::Invoke(UnityEngine.Analytics.AnalyticsSessionState,System.Int64,System.Int64,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SessionStateChanged_Invoke_m04E35EE6755FE675385F783A6FA7096BACD96740 (SessionStateChanged_t9084549A636BD45086D66CC6765DA8C3DD31066F * __this, int32_t ___sessionState0, int64_t ___sessionId1, int64_t ___sessionElapsedTime2, bool ___sessionChanged3, const RuntimeMethod* method);
// System.Void UnityEngine.RemoteConfigSettings::Destroy()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void RemoteConfigSettings_Destroy_m91215D772DF190B8AD9A73C8165DEDBE129C7A10 (RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A * __this, const RuntimeMethod* method);
// System.Void System.Object::Finalize()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object_Finalize_m4015B7D3A44DE125C5FE34D7276CD4697C06F380 (RuntimeObject * __this, const RuntimeMethod* method);
// System.Boolean System.IntPtr::op_Inequality(System.IntPtr,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool IntPtr_op_Inequality_mB4886A806009EA825EFCC60CD2A7F6EB8E273A61 (intptr_t ___value10, intptr_t ___value21, const RuntimeMethod* method);
// System.Void UnityEngine.RemoteConfigSettings::Internal_Destroy(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void RemoteConfigSettings_Internal_Destroy_mC419232704AC40CF7B3C5FB17A8A3D9FD2604F66 (intptr_t ___ptr0, const RuntimeMethod* method);
// System.Void System.Action`1<System.Boolean>::Invoke(!0)
inline void Action_1_Invoke_m45E8F9900F9DB395C48A868A7C6A83BDD7FC692F (Action_1_tAA0F894C98302D68F7D5034E8104E9AB4763CCAD * __this, bool ___obj0, const RuntimeMethod* method)
{
	((  void (*) (Action_1_tAA0F894C98302D68F7D5034E8104E9AB4763CCAD *, bool, const RuntimeMethod*))Action_1_Invoke_m45E8F9900F9DB395C48A868A7C6A83BDD7FC692F_gshared)(__this, ___obj0, method);
}
// System.Void UnityEngine.RemoteSettings/UpdatedEventHandler::Invoke()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UpdatedEventHandler_Invoke_mD2D72D86708690251A8A947FECE091E8EE0107E7 (UpdatedEventHandler_tB0230BC83686D7126AB4D3800A66351028CA514F * __this, const RuntimeMethod* method);
// System.Void System.Action::Invoke()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Action_Invoke_mC8D676E5DDF967EC5D23DD0E96FB52AA499817FD (Action_t591D2A86165F896B4B800BB5C25CE18672A55579 * __this, const RuntimeMethod* method);
// System.Void System.Action`3<System.Boolean,System.Boolean,System.Int32>::Invoke(!0,!1,!2)
inline void Action_3_Invoke_m8A4240174E60397816F6EACC65FE1E27F4275FA5 (Action_3_tEE1FB0623176AFA5109FAA9BA7E82293445CAE1E * __this, bool ___arg10, bool ___arg21, int32_t ___arg32, const RuntimeMethod* method)
{
	((  void (*) (Action_3_tEE1FB0623176AFA5109FAA9BA7E82293445CAE1E *, bool, bool, int32_t, const RuntimeMethod*))Action_3_Invoke_m8A4240174E60397816F6EACC65FE1E27F4275FA5_gshared)(__this, ___arg10, ___arg21, ___arg32, method);
}
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
// System.Void UnityEngine.Analytics.AnalyticsSessionInfo::CallSessionStateChanged(UnityEngine.Analytics.AnalyticsSessionState,System.Int64,System.Int64,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AnalyticsSessionInfo_CallSessionStateChanged_mB5E3A42407B97BD8A6A4827FADDA91DB85B0F294 (int32_t ___sessionState0, int64_t ___sessionId1, int64_t ___sessionElapsedTime2, bool ___sessionChanged3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AnalyticsSessionInfo_CallSessionStateChanged_mB5E3A42407B97BD8A6A4827FADDA91DB85B0F294_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	SessionStateChanged_t9084549A636BD45086D66CC6765DA8C3DD31066F * V_0 = NULL;
	{
		SessionStateChanged_t9084549A636BD45086D66CC6765DA8C3DD31066F * L_0 = ((AnalyticsSessionInfo_tE075F764A74D2B095CFD57F3B179397F504B7D8C_StaticFields*)il2cpp_codegen_static_fields_for(AnalyticsSessionInfo_tE075F764A74D2B095CFD57F3B179397F504B7D8C_il2cpp_TypeInfo_var))->get_sessionStateChanged_0();
		V_0 = L_0;
		SessionStateChanged_t9084549A636BD45086D66CC6765DA8C3DD31066F * L_1 = V_0;
		if (!L_1)
		{
			goto IL_0017;
		}
	}
	{
		SessionStateChanged_t9084549A636BD45086D66CC6765DA8C3DD31066F * L_2 = V_0;
		int32_t L_3 = ___sessionState0;
		int64_t L_4 = ___sessionId1;
		int64_t L_5 = ___sessionElapsedTime2;
		bool L_6 = ___sessionChanged3;
		NullCheck(L_2);
		SessionStateChanged_Invoke_m04E35EE6755FE675385F783A6FA7096BACD96740(L_2, L_3, L_4, L_5, L_6, /*hidden argument*/NULL);
	}

IL_0017:
	{
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
IL2CPP_EXTERN_C  void DelegatePInvokeWrapper_SessionStateChanged_t9084549A636BD45086D66CC6765DA8C3DD31066F (SessionStateChanged_t9084549A636BD45086D66CC6765DA8C3DD31066F * __this, int32_t ___sessionState0, int64_t ___sessionId1, int64_t ___sessionElapsedTime2, bool ___sessionChanged3, const RuntimeMethod* method)
{
	typedef void (DEFAULT_CALL *PInvokeFunc)(int32_t, int64_t, int64_t, int32_t);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	il2cppPInvokeFunc(___sessionState0, ___sessionId1, ___sessionElapsedTime2, static_cast<int32_t>(___sessionChanged3));

}
// System.Void UnityEngine.Analytics.AnalyticsSessionInfo_SessionStateChanged::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SessionStateChanged__ctor_m596C4D6873E1F9841C454BA7542D468F53A2DB25 (SessionStateChanged_t9084549A636BD45086D66CC6765DA8C3DD31066F * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void UnityEngine.Analytics.AnalyticsSessionInfo_SessionStateChanged::Invoke(UnityEngine.Analytics.AnalyticsSessionState,System.Int64,System.Int64,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SessionStateChanged_Invoke_m04E35EE6755FE675385F783A6FA7096BACD96740 (SessionStateChanged_t9084549A636BD45086D66CC6765DA8C3DD31066F * __this, int32_t ___sessionState0, int64_t ___sessionId1, int64_t ___sessionElapsedTime2, bool ___sessionChanged3, const RuntimeMethod* method)
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
			if (___parameterCount == 4)
			{
				// open
				typedef void (*FunctionPointerType) (int32_t, int64_t, int64_t, bool, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___sessionState0, ___sessionId1, ___sessionElapsedTime2, ___sessionChanged3, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, int32_t, int64_t, int64_t, bool, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___sessionState0, ___sessionId1, ___sessionElapsedTime2, ___sessionChanged3, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (int32_t, int64_t, int64_t, bool, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___sessionState0, ___sessionId1, ___sessionElapsedTime2, ___sessionChanged3, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker4< int32_t, int64_t, int64_t, bool >::Invoke(targetMethod, targetThis, ___sessionState0, ___sessionId1, ___sessionElapsedTime2, ___sessionChanged3);
					else
						GenericVirtActionInvoker4< int32_t, int64_t, int64_t, bool >::Invoke(targetMethod, targetThis, ___sessionState0, ___sessionId1, ___sessionElapsedTime2, ___sessionChanged3);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker4< int32_t, int64_t, int64_t, bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___sessionState0, ___sessionId1, ___sessionElapsedTime2, ___sessionChanged3);
					else
						VirtActionInvoker4< int32_t, int64_t, int64_t, bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___sessionState0, ___sessionId1, ___sessionElapsedTime2, ___sessionChanged3);
				}
			}
			else
			{
				typedef void (*FunctionPointerType) (void*, int32_t, int64_t, int64_t, bool, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___sessionState0, ___sessionId1, ___sessionElapsedTime2, ___sessionChanged3, targetMethod);
			}
		}
	}
}
// System.IAsyncResult UnityEngine.Analytics.AnalyticsSessionInfo_SessionStateChanged::BeginInvoke(UnityEngine.Analytics.AnalyticsSessionState,System.Int64,System.Int64,System.Boolean,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* SessionStateChanged_BeginInvoke_mD75E6832EC29F8B01CEEAAACD5DC54322F03D4B3 (SessionStateChanged_t9084549A636BD45086D66CC6765DA8C3DD31066F * __this, int32_t ___sessionState0, int64_t ___sessionId1, int64_t ___sessionElapsedTime2, bool ___sessionChanged3, AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4 * ___callback4, RuntimeObject * ___object5, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SessionStateChanged_BeginInvoke_mD75E6832EC29F8B01CEEAAACD5DC54322F03D4B3_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[5] = {0};
	__d_args[0] = Box(AnalyticsSessionState_t61CA873937E9A3B881B71B32F518A954A4C8F267_il2cpp_TypeInfo_var, &___sessionState0);
	__d_args[1] = Box(Int64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436_il2cpp_TypeInfo_var, &___sessionId1);
	__d_args[2] = Box(Int64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436_il2cpp_TypeInfo_var, &___sessionElapsedTime2);
	__d_args[3] = Box(Boolean_tB53F6830F670160873277339AA58F15CAED4399C_il2cpp_TypeInfo_var, &___sessionChanged3);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback4, (RuntimeObject*)___object5);
}
// System.Void UnityEngine.Analytics.AnalyticsSessionInfo_SessionStateChanged::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SessionStateChanged_EndInvoke_m1BE07B322DE61DE095B2E2F0D99F41D125F95E42 (SessionStateChanged_t9084549A636BD45086D66CC6765DA8C3DD31066F * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
// Conversion methods for marshalling of: UnityEngine.RemoteConfigSettings
IL2CPP_EXTERN_C void RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A_marshal_pinvoke(const RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A& unmarshaled, RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A_marshaled_pinvoke& marshaled)
{
	marshaled.___m_Ptr_0 = unmarshaled.get_m_Ptr_0();
	marshaled.___Updated_1 = il2cpp_codegen_marshal_delegate(reinterpret_cast<MulticastDelegate_t*>(unmarshaled.get_Updated_1()));
}
IL2CPP_EXTERN_C void RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A_marshal_pinvoke_back(const RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A_marshaled_pinvoke& marshaled, RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A& unmarshaled)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A_pinvoke_FromNativeMethodDefinition_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t unmarshaled_m_Ptr_temp_0;
	memset((&unmarshaled_m_Ptr_temp_0), 0, sizeof(unmarshaled_m_Ptr_temp_0));
	unmarshaled_m_Ptr_temp_0 = marshaled.___m_Ptr_0;
	unmarshaled.set_m_Ptr_0(unmarshaled_m_Ptr_temp_0);
	unmarshaled.set_Updated_1(il2cpp_codegen_marshal_function_ptr_to_delegate<Action_1_tAA0F894C98302D68F7D5034E8104E9AB4763CCAD>(marshaled.___Updated_1, Action_1_tAA0F894C98302D68F7D5034E8104E9AB4763CCAD_il2cpp_TypeInfo_var));
}
// Conversion method for clean up from marshalling of: UnityEngine.RemoteConfigSettings
IL2CPP_EXTERN_C void RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A_marshal_pinvoke_cleanup(RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A_marshaled_pinvoke& marshaled)
{
}
// Conversion methods for marshalling of: UnityEngine.RemoteConfigSettings
IL2CPP_EXTERN_C void RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A_marshal_com(const RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A& unmarshaled, RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A_marshaled_com& marshaled)
{
	marshaled.___m_Ptr_0 = unmarshaled.get_m_Ptr_0();
	marshaled.___Updated_1 = il2cpp_codegen_marshal_delegate(reinterpret_cast<MulticastDelegate_t*>(unmarshaled.get_Updated_1()));
}
IL2CPP_EXTERN_C void RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A_marshal_com_back(const RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A_marshaled_com& marshaled, RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A& unmarshaled)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A_com_FromNativeMethodDefinition_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t unmarshaled_m_Ptr_temp_0;
	memset((&unmarshaled_m_Ptr_temp_0), 0, sizeof(unmarshaled_m_Ptr_temp_0));
	unmarshaled_m_Ptr_temp_0 = marshaled.___m_Ptr_0;
	unmarshaled.set_m_Ptr_0(unmarshaled_m_Ptr_temp_0);
	unmarshaled.set_Updated_1(il2cpp_codegen_marshal_function_ptr_to_delegate<Action_1_tAA0F894C98302D68F7D5034E8104E9AB4763CCAD>(marshaled.___Updated_1, Action_1_tAA0F894C98302D68F7D5034E8104E9AB4763CCAD_il2cpp_TypeInfo_var));
}
// Conversion method for clean up from marshalling of: UnityEngine.RemoteConfigSettings
IL2CPP_EXTERN_C void RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A_marshal_com_cleanup(RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A_marshaled_com& marshaled)
{
}
// System.Void UnityEngine.RemoteConfigSettings::Finalize()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void RemoteConfigSettings_Finalize_mCF0EFDB4164F476B8835E6B16DA29165536ADC9A (RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A * __this, const RuntimeMethod* method)
{
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 1);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	{
	}

IL_0001:
	try
	{ // begin try (depth: 1)
		RemoteConfigSettings_Destroy_m91215D772DF190B8AD9A73C8165DEDBE129C7A10(__this, /*hidden argument*/NULL);
		IL2CPP_LEAVE(0x13, FINALLY_000c);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000c;
	}

FINALLY_000c:
	{ // begin finally (depth: 1)
		Object_Finalize_m4015B7D3A44DE125C5FE34D7276CD4697C06F380(__this, /*hidden argument*/NULL);
		IL2CPP_END_FINALLY(12)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(12)
	{
		IL2CPP_JUMP_TBL(0x13, IL_0013)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0013:
	{
		return;
	}
}
// System.Void UnityEngine.RemoteConfigSettings::Destroy()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void RemoteConfigSettings_Destroy_m91215D772DF190B8AD9A73C8165DEDBE129C7A10 (RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (RemoteConfigSettings_Destroy_m91215D772DF190B8AD9A73C8165DEDBE129C7A10_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		intptr_t L_0 = __this->get_m_Ptr_0();
		bool L_1 = IntPtr_op_Inequality_mB4886A806009EA825EFCC60CD2A7F6EB8E273A61((intptr_t)L_0, (intptr_t)(0), /*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_002e;
		}
	}
	{
		intptr_t L_2 = __this->get_m_Ptr_0();
		RemoteConfigSettings_Internal_Destroy_mC419232704AC40CF7B3C5FB17A8A3D9FD2604F66((intptr_t)L_2, /*hidden argument*/NULL);
		__this->set_m_Ptr_0((intptr_t)(0));
	}

IL_002e:
	{
		return;
	}
}
// System.Void UnityEngine.RemoteConfigSettings::Internal_Destroy(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void RemoteConfigSettings_Internal_Destroy_mC419232704AC40CF7B3C5FB17A8A3D9FD2604F66 (intptr_t ___ptr0, const RuntimeMethod* method)
{
	typedef void (*RemoteConfigSettings_Internal_Destroy_mC419232704AC40CF7B3C5FB17A8A3D9FD2604F66_ftn) (intptr_t);
	static RemoteConfigSettings_Internal_Destroy_mC419232704AC40CF7B3C5FB17A8A3D9FD2604F66_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (RemoteConfigSettings_Internal_Destroy_mC419232704AC40CF7B3C5FB17A8A3D9FD2604F66_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.RemoteConfigSettings::Internal_Destroy(System.IntPtr)");
	_il2cpp_icall_func(___ptr0);
}
// System.Void UnityEngine.RemoteConfigSettings::RemoteConfigSettingsUpdated(UnityEngine.RemoteConfigSettings,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void RemoteConfigSettings_RemoteConfigSettingsUpdated_mF72E8C9F99B752FEA9EA388CCC1EFECC24733504 (RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A * ___rcs0, bool ___wasLastUpdatedFromServer1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (RemoteConfigSettings_RemoteConfigSettingsUpdated_mF72E8C9F99B752FEA9EA388CCC1EFECC24733504_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Action_1_tAA0F894C98302D68F7D5034E8104E9AB4763CCAD * V_0 = NULL;
	{
		RemoteConfigSettings_t97154F5546B47CE72257CC2F0B677BDF696AEC4A * L_0 = ___rcs0;
		NullCheck(L_0);
		Action_1_tAA0F894C98302D68F7D5034E8104E9AB4763CCAD * L_1 = L_0->get_Updated_1();
		V_0 = L_1;
		Action_1_tAA0F894C98302D68F7D5034E8104E9AB4763CCAD * L_2 = V_0;
		if (!L_2)
		{
			goto IL_0015;
		}
	}
	{
		Action_1_tAA0F894C98302D68F7D5034E8104E9AB4763CCAD * L_3 = V_0;
		bool L_4 = ___wasLastUpdatedFromServer1;
		NullCheck(L_3);
		Action_1_Invoke_m45E8F9900F9DB395C48A868A7C6A83BDD7FC692F(L_3, L_4, /*hidden argument*/Action_1_Invoke_m45E8F9900F9DB395C48A868A7C6A83BDD7FC692F_RuntimeMethod_var);
	}

IL_0015:
	{
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
// System.Void UnityEngine.RemoteSettings::RemoteSettingsUpdated(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void RemoteSettings_RemoteSettingsUpdated_mD5427A08A622FDAA576A3D9CF05AD4C7CC6102F5 (bool ___wasLastUpdatedFromServer0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (RemoteSettings_RemoteSettingsUpdated_mD5427A08A622FDAA576A3D9CF05AD4C7CC6102F5_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	UpdatedEventHandler_tB0230BC83686D7126AB4D3800A66351028CA514F * V_0 = NULL;
	{
		UpdatedEventHandler_tB0230BC83686D7126AB4D3800A66351028CA514F * L_0 = ((RemoteSettings_t3F7E07D15288B0DF84A4A32044592D8AFA6D36ED_StaticFields*)il2cpp_codegen_static_fields_for(RemoteSettings_t3F7E07D15288B0DF84A4A32044592D8AFA6D36ED_il2cpp_TypeInfo_var))->get_Updated_0();
		V_0 = L_0;
		UpdatedEventHandler_tB0230BC83686D7126AB4D3800A66351028CA514F * L_1 = V_0;
		if (!L_1)
		{
			goto IL_0013;
		}
	}
	{
		UpdatedEventHandler_tB0230BC83686D7126AB4D3800A66351028CA514F * L_2 = V_0;
		NullCheck(L_2);
		UpdatedEventHandler_Invoke_mD2D72D86708690251A8A947FECE091E8EE0107E7(L_2, /*hidden argument*/NULL);
	}

IL_0013:
	{
		return;
	}
}
// System.Void UnityEngine.RemoteSettings::RemoteSettingsBeforeFetchFromServer()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void RemoteSettings_RemoteSettingsBeforeFetchFromServer_mBF1BEF2ACAFE13AE7B820FCA5CB7B81CF614EF11 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (RemoteSettings_RemoteSettingsBeforeFetchFromServer_mBF1BEF2ACAFE13AE7B820FCA5CB7B81CF614EF11_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Action_t591D2A86165F896B4B800BB5C25CE18672A55579 * V_0 = NULL;
	{
		Action_t591D2A86165F896B4B800BB5C25CE18672A55579 * L_0 = ((RemoteSettings_t3F7E07D15288B0DF84A4A32044592D8AFA6D36ED_StaticFields*)il2cpp_codegen_static_fields_for(RemoteSettings_t3F7E07D15288B0DF84A4A32044592D8AFA6D36ED_il2cpp_TypeInfo_var))->get_BeforeFetchFromServer_1();
		V_0 = L_0;
		Action_t591D2A86165F896B4B800BB5C25CE18672A55579 * L_1 = V_0;
		if (!L_1)
		{
			goto IL_0013;
		}
	}
	{
		Action_t591D2A86165F896B4B800BB5C25CE18672A55579 * L_2 = V_0;
		NullCheck(L_2);
		Action_Invoke_mC8D676E5DDF967EC5D23DD0E96FB52AA499817FD(L_2, /*hidden argument*/NULL);
	}

IL_0013:
	{
		return;
	}
}
// System.Void UnityEngine.RemoteSettings::RemoteSettingsUpdateCompleted(System.Boolean,System.Boolean,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void RemoteSettings_RemoteSettingsUpdateCompleted_m6C6C3568C5A8F319CD06F8303111F06AB3718A02 (bool ___wasLastUpdatedFromServer0, bool ___settingsChanged1, int32_t ___response2, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (RemoteSettings_RemoteSettingsUpdateCompleted_m6C6C3568C5A8F319CD06F8303111F06AB3718A02_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Action_3_tEE1FB0623176AFA5109FAA9BA7E82293445CAE1E * V_0 = NULL;
	{
		Action_3_tEE1FB0623176AFA5109FAA9BA7E82293445CAE1E * L_0 = ((RemoteSettings_t3F7E07D15288B0DF84A4A32044592D8AFA6D36ED_StaticFields*)il2cpp_codegen_static_fields_for(RemoteSettings_t3F7E07D15288B0DF84A4A32044592D8AFA6D36ED_il2cpp_TypeInfo_var))->get_Completed_2();
		V_0 = L_0;
		Action_3_tEE1FB0623176AFA5109FAA9BA7E82293445CAE1E * L_1 = V_0;
		if (!L_1)
		{
			goto IL_0016;
		}
	}
	{
		Action_3_tEE1FB0623176AFA5109FAA9BA7E82293445CAE1E * L_2 = V_0;
		bool L_3 = ___wasLastUpdatedFromServer0;
		bool L_4 = ___settingsChanged1;
		int32_t L_5 = ___response2;
		NullCheck(L_2);
		Action_3_Invoke_m8A4240174E60397816F6EACC65FE1E27F4275FA5(L_2, L_3, L_4, L_5, /*hidden argument*/Action_3_Invoke_m8A4240174E60397816F6EACC65FE1E27F4275FA5_RuntimeMethod_var);
	}

IL_0016:
	{
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
IL2CPP_EXTERN_C  void DelegatePInvokeWrapper_UpdatedEventHandler_tB0230BC83686D7126AB4D3800A66351028CA514F (UpdatedEventHandler_tB0230BC83686D7126AB4D3800A66351028CA514F * __this, const RuntimeMethod* method)
{
	typedef void (DEFAULT_CALL *PInvokeFunc)();
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	il2cppPInvokeFunc();

}
// System.Void UnityEngine.RemoteSettings_UpdatedEventHandler::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UpdatedEventHandler__ctor_mB0CFE6A3B394C3858502E54A9CBEE97B40690DE7 (UpdatedEventHandler_tB0230BC83686D7126AB4D3800A66351028CA514F * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void UnityEngine.RemoteSettings_UpdatedEventHandler::Invoke()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UpdatedEventHandler_Invoke_mD2D72D86708690251A8A947FECE091E8EE0107E7 (UpdatedEventHandler_tB0230BC83686D7126AB4D3800A66351028CA514F * __this, const RuntimeMethod* method)
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
			if (___parameterCount == 0)
			{
				// open
				typedef void (*FunctionPointerType) (const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker0::Invoke(targetMethod, targetThis);
					else
						GenericVirtActionInvoker0::Invoke(targetMethod, targetThis);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker0::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis);
					else
						VirtActionInvoker0::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis);
				}
			}
			else
			{
				typedef void (*FunctionPointerType) (void*, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, targetMethod);
			}
		}
	}
}
// System.IAsyncResult UnityEngine.RemoteSettings_UpdatedEventHandler::BeginInvoke(System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* UpdatedEventHandler_BeginInvoke_m45D924AF948243AD8BD8D2E403DD57544E34507E (UpdatedEventHandler_tB0230BC83686D7126AB4D3800A66351028CA514F * __this, AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4 * ___callback0, RuntimeObject * ___object1, const RuntimeMethod* method)
{
	void *__d_args[1] = {0};
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback0, (RuntimeObject*)___object1);
}
// System.Void UnityEngine.RemoteSettings_UpdatedEventHandler::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UpdatedEventHandler_EndInvoke_m54BCADBF1EE00E9C6DFF9D59C578C3C244CA3EA5 (UpdatedEventHandler_tB0230BC83686D7126AB4D3800A66351028CA514F * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
{
	il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___result0, 0);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
