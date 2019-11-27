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
template <typename T1, typename T2>
struct VirtActionInvoker2
{
	typedef void (*Action)(void*, T1, T2, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		((Action)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
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
template <typename T1, typename T2>
struct GenericVirtActionInvoker2
{
	typedef void (*Action)(void*, T1, T2, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
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
struct InterfaceActionInvoker1
{
	typedef void (*Action)(void*, T1, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, p1, invokeData.method);
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
template <typename T1, typename T2>
struct GenericInterfaceActionInvoker2
{
	typedef void (*Action)(void*, T1, T2, const RuntimeMethod*);

	static inline void Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		((Action)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
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
// UnityEngine.Video.VideoPlayer
struct VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2;
// UnityEngine.Video.VideoPlayer/ErrorEventHandler
struct ErrorEventHandler_tF5863946928B48BE13146ED5FF70AC92678FE222;
// UnityEngine.Video.VideoPlayer/EventHandler
struct EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308;
// UnityEngine.Video.VideoPlayer/FrameReadyEventHandler
struct FrameReadyEventHandler_t518B277D916AB292680CAA186BCDB3D3EF130422;
// UnityEngine.Video.VideoPlayer/TimeEventHandler
struct TimeEventHandler_tDD815DAABFADDD98C8993B2A97A2FCE858266BC1;

IL2CPP_EXTERN_C RuntimeClass* Double_t358B8F23BDC52A5DD700E727E204F9F7CDE12409_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Int64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C const uint32_t FrameReadyEventHandler_BeginInvoke_m5DA99DFE61C78E158FF79535447F7649FC09E5F1_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t TimeEventHandler_BeginInvoke_m184CF1FDB1D643F00FE3C60982ED62EC4888F21D_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t VideoClipPlayable_Equals_m2F07E6EBA96043274F2384F18E43B66035058BC2_MetadataUsageId;
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
struct  U3CModuleU3E_t064756C4EE8D64CEFE107E600CEBCB3F77894990 
{
public:

public:
};


// System.Object

struct Il2CppArrayBounds;

// System.Array


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


// System.Double
struct  Double_t358B8F23BDC52A5DD700E727E204F9F7CDE12409 
{
public:
	// System.Double System.Double::m_value
	double ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Double_t358B8F23BDC52A5DD700E727E204F9F7CDE12409, ___m_value_0)); }
	inline double get_m_value_0() const { return ___m_value_0; }
	inline double* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(double value)
	{
		___m_value_0 = value;
	}
};

struct Double_t358B8F23BDC52A5DD700E727E204F9F7CDE12409_StaticFields
{
public:
	// System.Double System.Double::NegativeZero
	double ___NegativeZero_7;

public:
	inline static int32_t get_offset_of_NegativeZero_7() { return static_cast<int32_t>(offsetof(Double_t358B8F23BDC52A5DD700E727E204F9F7CDE12409_StaticFields, ___NegativeZero_7)); }
	inline double get_NegativeZero_7() const { return ___NegativeZero_7; }
	inline double* get_address_of_NegativeZero_7() { return &___NegativeZero_7; }
	inline void set_NegativeZero_7(double value)
	{
		___NegativeZero_7 = value;
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

// UnityEngine.Playables.PlayableHandle
struct  PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182 
{
public:
	// System.IntPtr UnityEngine.Playables.PlayableHandle::m_Handle
	intptr_t ___m_Handle_0;
	// System.UInt32 UnityEngine.Playables.PlayableHandle::m_Version
	uint32_t ___m_Version_1;

public:
	inline static int32_t get_offset_of_m_Handle_0() { return static_cast<int32_t>(offsetof(PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182, ___m_Handle_0)); }
	inline intptr_t get_m_Handle_0() const { return ___m_Handle_0; }
	inline intptr_t* get_address_of_m_Handle_0() { return &___m_Handle_0; }
	inline void set_m_Handle_0(intptr_t value)
	{
		___m_Handle_0 = value;
	}

	inline static int32_t get_offset_of_m_Version_1() { return static_cast<int32_t>(offsetof(PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182, ___m_Version_1)); }
	inline uint32_t get_m_Version_1() const { return ___m_Version_1; }
	inline uint32_t* get_address_of_m_Version_1() { return &___m_Version_1; }
	inline void set_m_Version_1(uint32_t value)
	{
		___m_Version_1 = value;
	}
};

struct PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182_StaticFields
{
public:
	// UnityEngine.Playables.PlayableHandle UnityEngine.Playables.PlayableHandle::m_Null
	PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  ___m_Null_2;

public:
	inline static int32_t get_offset_of_m_Null_2() { return static_cast<int32_t>(offsetof(PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182_StaticFields, ___m_Null_2)); }
	inline PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  get_m_Null_2() const { return ___m_Null_2; }
	inline PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182 * get_address_of_m_Null_2() { return &___m_Null_2; }
	inline void set_m_Null_2(PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  value)
	{
		___m_Null_2 = value;
	}
};


// UnityEngine.Video.Video3DLayout
struct  Video3DLayout_t5F64D0CE5E9B37C2FCE67F397FA5CFE9C047E4A1 
{
public:
	// System.Int32 UnityEngine.Video.Video3DLayout::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(Video3DLayout_t5F64D0CE5E9B37C2FCE67F397FA5CFE9C047E4A1, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.Video.VideoAspectRatio
struct  VideoAspectRatio_t5739968D28C4F8F802B085E293F22110205B8379 
{
public:
	// System.Int32 UnityEngine.Video.VideoAspectRatio::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(VideoAspectRatio_t5739968D28C4F8F802B085E293F22110205B8379, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.Video.VideoAudioOutputMode
struct  VideoAudioOutputMode_t8CDE10B382F3C321345EC57C9164A9177139DC6F 
{
public:
	// System.Int32 UnityEngine.Video.VideoAudioOutputMode::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(VideoAudioOutputMode_t8CDE10B382F3C321345EC57C9164A9177139DC6F, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.Video.VideoRenderMode
struct  VideoRenderMode_t0DBAABB576FDA890C49C6AD3EE641623F93E9161 
{
public:
	// System.Int32 UnityEngine.Video.VideoRenderMode::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(VideoRenderMode_t0DBAABB576FDA890C49C6AD3EE641623F93E9161, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.Video.VideoSource
struct  VideoSource_t32501B57EA7F9CF835FBA8184C9AF427CBBEFD0A 
{
public:
	// System.Int32 UnityEngine.Video.VideoSource::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(VideoSource_t32501B57EA7F9CF835FBA8184C9AF427CBBEFD0A, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.Video.VideoTimeReference
struct  VideoTimeReference_t9EAEBD354AE5E56F0D0F36E73A428BB2A0B8B31B 
{
public:
	// System.Int32 UnityEngine.Video.VideoTimeReference::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(VideoTimeReference_t9EAEBD354AE5E56F0D0F36E73A428BB2A0B8B31B, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.Video.VideoTimeSource
struct  VideoTimeSource_t15F04FD6B3D75A8D98480E8B77117C0FF691BB77 
{
public:
	// System.Int32 UnityEngine.Video.VideoTimeSource::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(VideoTimeSource_t15F04FD6B3D75A8D98480E8B77117C0FF691BB77, ___value___2)); }
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

// UnityEngine.Component
struct  Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621  : public Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0
{
public:

public:
};


// UnityEngine.Experimental.Video.VideoClipPlayable
struct  VideoClipPlayable_t4B7997FDB02C74F9E88F37574F0F4F9DE08CCC12 
{
public:
	// UnityEngine.Playables.PlayableHandle UnityEngine.Experimental.Video.VideoClipPlayable::m_Handle
	PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  ___m_Handle_0;

public:
	inline static int32_t get_offset_of_m_Handle_0() { return static_cast<int32_t>(offsetof(VideoClipPlayable_t4B7997FDB02C74F9E88F37574F0F4F9DE08CCC12, ___m_Handle_0)); }
	inline PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  get_m_Handle_0() const { return ___m_Handle_0; }
	inline PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182 * get_address_of_m_Handle_0() { return &___m_Handle_0; }
	inline void set_m_Handle_0(PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  value)
	{
		___m_Handle_0 = value;
	}
};


// UnityEngine.Video.VideoClip
struct  VideoClip_tA4039CBBC6F9C3AD62B067964A6C20C6FB7376D5  : public Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0
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


// UnityEngine.Video.VideoPlayer_ErrorEventHandler
struct  ErrorEventHandler_tF5863946928B48BE13146ED5FF70AC92678FE222  : public MulticastDelegate_t
{
public:

public:
};


// UnityEngine.Video.VideoPlayer_EventHandler
struct  EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308  : public MulticastDelegate_t
{
public:

public:
};


// UnityEngine.Video.VideoPlayer_FrameReadyEventHandler
struct  FrameReadyEventHandler_t518B277D916AB292680CAA186BCDB3D3EF130422  : public MulticastDelegate_t
{
public:

public:
};


// UnityEngine.Video.VideoPlayer_TimeEventHandler
struct  TimeEventHandler_tDD815DAABFADDD98C8993B2A97A2FCE858266BC1  : public MulticastDelegate_t
{
public:

public:
};


// UnityEngine.Video.VideoPlayer
struct  VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2  : public Behaviour_tBDC7E9C3C898AD8348891B82D3E345801D920CA8
{
public:
	// UnityEngine.Video.VideoPlayer_EventHandler UnityEngine.Video.VideoPlayer::prepareCompleted
	EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * ___prepareCompleted_4;
	// UnityEngine.Video.VideoPlayer_EventHandler UnityEngine.Video.VideoPlayer::loopPointReached
	EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * ___loopPointReached_5;
	// UnityEngine.Video.VideoPlayer_EventHandler UnityEngine.Video.VideoPlayer::started
	EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * ___started_6;
	// UnityEngine.Video.VideoPlayer_EventHandler UnityEngine.Video.VideoPlayer::frameDropped
	EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * ___frameDropped_7;
	// UnityEngine.Video.VideoPlayer_ErrorEventHandler UnityEngine.Video.VideoPlayer::errorReceived
	ErrorEventHandler_tF5863946928B48BE13146ED5FF70AC92678FE222 * ___errorReceived_8;
	// UnityEngine.Video.VideoPlayer_EventHandler UnityEngine.Video.VideoPlayer::seekCompleted
	EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * ___seekCompleted_9;
	// UnityEngine.Video.VideoPlayer_TimeEventHandler UnityEngine.Video.VideoPlayer::clockResyncOccurred
	TimeEventHandler_tDD815DAABFADDD98C8993B2A97A2FCE858266BC1 * ___clockResyncOccurred_10;
	// UnityEngine.Video.VideoPlayer_FrameReadyEventHandler UnityEngine.Video.VideoPlayer::frameReady
	FrameReadyEventHandler_t518B277D916AB292680CAA186BCDB3D3EF130422 * ___frameReady_11;

public:
	inline static int32_t get_offset_of_prepareCompleted_4() { return static_cast<int32_t>(offsetof(VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2, ___prepareCompleted_4)); }
	inline EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * get_prepareCompleted_4() const { return ___prepareCompleted_4; }
	inline EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 ** get_address_of_prepareCompleted_4() { return &___prepareCompleted_4; }
	inline void set_prepareCompleted_4(EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * value)
	{
		___prepareCompleted_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___prepareCompleted_4), (void*)value);
	}

	inline static int32_t get_offset_of_loopPointReached_5() { return static_cast<int32_t>(offsetof(VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2, ___loopPointReached_5)); }
	inline EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * get_loopPointReached_5() const { return ___loopPointReached_5; }
	inline EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 ** get_address_of_loopPointReached_5() { return &___loopPointReached_5; }
	inline void set_loopPointReached_5(EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * value)
	{
		___loopPointReached_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___loopPointReached_5), (void*)value);
	}

	inline static int32_t get_offset_of_started_6() { return static_cast<int32_t>(offsetof(VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2, ___started_6)); }
	inline EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * get_started_6() const { return ___started_6; }
	inline EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 ** get_address_of_started_6() { return &___started_6; }
	inline void set_started_6(EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * value)
	{
		___started_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___started_6), (void*)value);
	}

	inline static int32_t get_offset_of_frameDropped_7() { return static_cast<int32_t>(offsetof(VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2, ___frameDropped_7)); }
	inline EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * get_frameDropped_7() const { return ___frameDropped_7; }
	inline EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 ** get_address_of_frameDropped_7() { return &___frameDropped_7; }
	inline void set_frameDropped_7(EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * value)
	{
		___frameDropped_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___frameDropped_7), (void*)value);
	}

	inline static int32_t get_offset_of_errorReceived_8() { return static_cast<int32_t>(offsetof(VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2, ___errorReceived_8)); }
	inline ErrorEventHandler_tF5863946928B48BE13146ED5FF70AC92678FE222 * get_errorReceived_8() const { return ___errorReceived_8; }
	inline ErrorEventHandler_tF5863946928B48BE13146ED5FF70AC92678FE222 ** get_address_of_errorReceived_8() { return &___errorReceived_8; }
	inline void set_errorReceived_8(ErrorEventHandler_tF5863946928B48BE13146ED5FF70AC92678FE222 * value)
	{
		___errorReceived_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___errorReceived_8), (void*)value);
	}

	inline static int32_t get_offset_of_seekCompleted_9() { return static_cast<int32_t>(offsetof(VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2, ___seekCompleted_9)); }
	inline EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * get_seekCompleted_9() const { return ___seekCompleted_9; }
	inline EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 ** get_address_of_seekCompleted_9() { return &___seekCompleted_9; }
	inline void set_seekCompleted_9(EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * value)
	{
		___seekCompleted_9 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___seekCompleted_9), (void*)value);
	}

	inline static int32_t get_offset_of_clockResyncOccurred_10() { return static_cast<int32_t>(offsetof(VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2, ___clockResyncOccurred_10)); }
	inline TimeEventHandler_tDD815DAABFADDD98C8993B2A97A2FCE858266BC1 * get_clockResyncOccurred_10() const { return ___clockResyncOccurred_10; }
	inline TimeEventHandler_tDD815DAABFADDD98C8993B2A97A2FCE858266BC1 ** get_address_of_clockResyncOccurred_10() { return &___clockResyncOccurred_10; }
	inline void set_clockResyncOccurred_10(TimeEventHandler_tDD815DAABFADDD98C8993B2A97A2FCE858266BC1 * value)
	{
		___clockResyncOccurred_10 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___clockResyncOccurred_10), (void*)value);
	}

	inline static int32_t get_offset_of_frameReady_11() { return static_cast<int32_t>(offsetof(VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2, ___frameReady_11)); }
	inline FrameReadyEventHandler_t518B277D916AB292680CAA186BCDB3D3EF130422 * get_frameReady_11() const { return ___frameReady_11; }
	inline FrameReadyEventHandler_t518B277D916AB292680CAA186BCDB3D3EF130422 ** get_address_of_frameReady_11() { return &___frameReady_11; }
	inline void set_frameReady_11(FrameReadyEventHandler_t518B277D916AB292680CAA186BCDB3D3EF130422 * value)
	{
		___frameReady_11 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___frameReady_11), (void*)value);
	}
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



// UnityEngine.Playables.PlayableHandle UnityEngine.Experimental.Video.VideoClipPlayable::GetHandle()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  VideoClipPlayable_GetHandle_m5B2EFD8CFE93DB4D2EE29A600E598233471DF242 (VideoClipPlayable_t4B7997FDB02C74F9E88F37574F0F4F9DE08CCC12 * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Playables.PlayableHandle::op_Equality(UnityEngine.Playables.PlayableHandle,UnityEngine.Playables.PlayableHandle)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool PlayableHandle_op_Equality_mBA774AE123AF794A1EB55148206CDD52DAFA42DF (PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  ___x0, PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  ___y1, const RuntimeMethod* method);
// System.Boolean UnityEngine.Experimental.Video.VideoClipPlayable::Equals(UnityEngine.Experimental.Video.VideoClipPlayable)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VideoClipPlayable_Equals_m2F07E6EBA96043274F2384F18E43B66035058BC2 (VideoClipPlayable_t4B7997FDB02C74F9E88F37574F0F4F9DE08CCC12 * __this, VideoClipPlayable_t4B7997FDB02C74F9E88F37574F0F4F9DE08CCC12  ___other0, const RuntimeMethod* method);
// System.Void UnityEngine.Video.VideoPlayer/EventHandler::Invoke(UnityEngine.Video.VideoPlayer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void EventHandler_Invoke_m137A7D976F198147AD939AEF51E157107A3B1FBC (EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * __this, VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * ___source0, const RuntimeMethod* method);
// System.Void UnityEngine.Video.VideoPlayer/FrameReadyEventHandler::Invoke(UnityEngine.Video.VideoPlayer,System.Int64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void FrameReadyEventHandler_Invoke_m88D0AC1BED08D66B6CFA18DA23C58D10795DDA70 (FrameReadyEventHandler_t518B277D916AB292680CAA186BCDB3D3EF130422 * __this, VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * ___source0, int64_t ___frameIdx1, const RuntimeMethod* method);
// System.Void UnityEngine.Video.VideoPlayer/ErrorEventHandler::Invoke(UnityEngine.Video.VideoPlayer,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ErrorEventHandler_Invoke_m0A812811B673439792D99C125EE4FFE5E358EF6C (ErrorEventHandler_tF5863946928B48BE13146ED5FF70AC92678FE222 * __this, VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * ___source0, String_t* ___message1, const RuntimeMethod* method);
// System.Void UnityEngine.Video.VideoPlayer/TimeEventHandler::Invoke(UnityEngine.Video.VideoPlayer,System.Double)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TimeEventHandler_Invoke_m278E51F2838EC435606BE1CB3AD0E881505FAE10 (TimeEventHandler_tDD815DAABFADDD98C8993B2A97A2FCE858266BC1 * __this, VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * ___source0, double ___seconds1, const RuntimeMethod* method);
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
// UnityEngine.Playables.PlayableHandle UnityEngine.Experimental.Video.VideoClipPlayable::GetHandle()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  VideoClipPlayable_GetHandle_m5B2EFD8CFE93DB4D2EE29A600E598233471DF242 (VideoClipPlayable_t4B7997FDB02C74F9E88F37574F0F4F9DE08CCC12 * __this, const RuntimeMethod* method)
{
	PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  L_0 = __this->get_m_Handle_0();
		V_0 = L_0;
		goto IL_000d;
	}

IL_000d:
	{
		PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  L_1 = V_0;
		return L_1;
	}
}
IL2CPP_EXTERN_C  PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  VideoClipPlayable_GetHandle_m5B2EFD8CFE93DB4D2EE29A600E598233471DF242_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	VideoClipPlayable_t4B7997FDB02C74F9E88F37574F0F4F9DE08CCC12 * _thisAdjusted = reinterpret_cast<VideoClipPlayable_t4B7997FDB02C74F9E88F37574F0F4F9DE08CCC12 *>(__this + 1);
	return VideoClipPlayable_GetHandle_m5B2EFD8CFE93DB4D2EE29A600E598233471DF242(_thisAdjusted, method);
}
// System.Boolean UnityEngine.Experimental.Video.VideoClipPlayable::Equals(UnityEngine.Experimental.Video.VideoClipPlayable)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool VideoClipPlayable_Equals_m2F07E6EBA96043274F2384F18E43B66035058BC2 (VideoClipPlayable_t4B7997FDB02C74F9E88F37574F0F4F9DE08CCC12 * __this, VideoClipPlayable_t4B7997FDB02C74F9E88F37574F0F4F9DE08CCC12  ___other0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (VideoClipPlayable_Equals_m2F07E6EBA96043274F2384F18E43B66035058BC2_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	{
		PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  L_0 = VideoClipPlayable_GetHandle_m5B2EFD8CFE93DB4D2EE29A600E598233471DF242((VideoClipPlayable_t4B7997FDB02C74F9E88F37574F0F4F9DE08CCC12 *)__this, /*hidden argument*/NULL);
		PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  L_1 = VideoClipPlayable_GetHandle_m5B2EFD8CFE93DB4D2EE29A600E598233471DF242((VideoClipPlayable_t4B7997FDB02C74F9E88F37574F0F4F9DE08CCC12 *)(&___other0), /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182_il2cpp_TypeInfo_var);
		bool L_2 = PlayableHandle_op_Equality_mBA774AE123AF794A1EB55148206CDD52DAFA42DF(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		goto IL_0019;
	}

IL_0019:
	{
		bool L_3 = V_0;
		return L_3;
	}
}
IL2CPP_EXTERN_C  bool VideoClipPlayable_Equals_m2F07E6EBA96043274F2384F18E43B66035058BC2_AdjustorThunk (RuntimeObject * __this, VideoClipPlayable_t4B7997FDB02C74F9E88F37574F0F4F9DE08CCC12  ___other0, const RuntimeMethod* method)
{
	VideoClipPlayable_t4B7997FDB02C74F9E88F37574F0F4F9DE08CCC12 * _thisAdjusted = reinterpret_cast<VideoClipPlayable_t4B7997FDB02C74F9E88F37574F0F4F9DE08CCC12 *>(__this + 1);
	return VideoClipPlayable_Equals_m2F07E6EBA96043274F2384F18E43B66035058BC2(_thisAdjusted, ___other0, method);
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
// System.Void UnityEngine.Video.VideoPlayer::InvokePrepareCompletedCallback_Internal(UnityEngine.Video.VideoPlayer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VideoPlayer_InvokePrepareCompletedCallback_Internal_m4CFD7054C97BE95CAC055CF18466E90D060E9B53 (VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * ___source0, const RuntimeMethod* method)
{
	{
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_0 = ___source0;
		NullCheck(L_0);
		EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * L_1 = L_0->get_prepareCompleted_4();
		if (!L_1)
		{
			goto IL_0018;
		}
	}
	{
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_2 = ___source0;
		NullCheck(L_2);
		EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * L_3 = L_2->get_prepareCompleted_4();
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_4 = ___source0;
		NullCheck(L_3);
		EventHandler_Invoke_m137A7D976F198147AD939AEF51E157107A3B1FBC(L_3, L_4, /*hidden argument*/NULL);
	}

IL_0018:
	{
		return;
	}
}
// System.Void UnityEngine.Video.VideoPlayer::InvokeFrameReadyCallback_Internal(UnityEngine.Video.VideoPlayer,System.Int64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VideoPlayer_InvokeFrameReadyCallback_Internal_m4F62FC3695CFC72045E3C90503D541AE6E023EE8 (VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * ___source0, int64_t ___frameIdx1, const RuntimeMethod* method)
{
	{
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_0 = ___source0;
		NullCheck(L_0);
		FrameReadyEventHandler_t518B277D916AB292680CAA186BCDB3D3EF130422 * L_1 = L_0->get_frameReady_11();
		if (!L_1)
		{
			goto IL_0019;
		}
	}
	{
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_2 = ___source0;
		NullCheck(L_2);
		FrameReadyEventHandler_t518B277D916AB292680CAA186BCDB3D3EF130422 * L_3 = L_2->get_frameReady_11();
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_4 = ___source0;
		int64_t L_5 = ___frameIdx1;
		NullCheck(L_3);
		FrameReadyEventHandler_Invoke_m88D0AC1BED08D66B6CFA18DA23C58D10795DDA70(L_3, L_4, L_5, /*hidden argument*/NULL);
	}

IL_0019:
	{
		return;
	}
}
// System.Void UnityEngine.Video.VideoPlayer::InvokeLoopPointReachedCallback_Internal(UnityEngine.Video.VideoPlayer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VideoPlayer_InvokeLoopPointReachedCallback_Internal_m1A07EB382FD3CE673FF171A11047BEC54A6BB9AB (VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * ___source0, const RuntimeMethod* method)
{
	{
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_0 = ___source0;
		NullCheck(L_0);
		EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * L_1 = L_0->get_loopPointReached_5();
		if (!L_1)
		{
			goto IL_0018;
		}
	}
	{
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_2 = ___source0;
		NullCheck(L_2);
		EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * L_3 = L_2->get_loopPointReached_5();
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_4 = ___source0;
		NullCheck(L_3);
		EventHandler_Invoke_m137A7D976F198147AD939AEF51E157107A3B1FBC(L_3, L_4, /*hidden argument*/NULL);
	}

IL_0018:
	{
		return;
	}
}
// System.Void UnityEngine.Video.VideoPlayer::InvokeStartedCallback_Internal(UnityEngine.Video.VideoPlayer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VideoPlayer_InvokeStartedCallback_Internal_m28D5BF153FC37959C225292BE859135FF778C8BB (VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * ___source0, const RuntimeMethod* method)
{
	{
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_0 = ___source0;
		NullCheck(L_0);
		EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * L_1 = L_0->get_started_6();
		if (!L_1)
		{
			goto IL_0018;
		}
	}
	{
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_2 = ___source0;
		NullCheck(L_2);
		EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * L_3 = L_2->get_started_6();
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_4 = ___source0;
		NullCheck(L_3);
		EventHandler_Invoke_m137A7D976F198147AD939AEF51E157107A3B1FBC(L_3, L_4, /*hidden argument*/NULL);
	}

IL_0018:
	{
		return;
	}
}
// System.Void UnityEngine.Video.VideoPlayer::InvokeFrameDroppedCallback_Internal(UnityEngine.Video.VideoPlayer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VideoPlayer_InvokeFrameDroppedCallback_Internal_m669EAEF06893B53351EE81C3858BD62661228C58 (VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * ___source0, const RuntimeMethod* method)
{
	{
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_0 = ___source0;
		NullCheck(L_0);
		EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * L_1 = L_0->get_frameDropped_7();
		if (!L_1)
		{
			goto IL_0018;
		}
	}
	{
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_2 = ___source0;
		NullCheck(L_2);
		EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * L_3 = L_2->get_frameDropped_7();
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_4 = ___source0;
		NullCheck(L_3);
		EventHandler_Invoke_m137A7D976F198147AD939AEF51E157107A3B1FBC(L_3, L_4, /*hidden argument*/NULL);
	}

IL_0018:
	{
		return;
	}
}
// System.Void UnityEngine.Video.VideoPlayer::InvokeErrorReceivedCallback_Internal(UnityEngine.Video.VideoPlayer,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VideoPlayer_InvokeErrorReceivedCallback_Internal_mF7849030756F4A2B5226437C165ECDFE6E52E385 (VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * ___source0, String_t* ___errorStr1, const RuntimeMethod* method)
{
	{
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_0 = ___source0;
		NullCheck(L_0);
		ErrorEventHandler_tF5863946928B48BE13146ED5FF70AC92678FE222 * L_1 = L_0->get_errorReceived_8();
		if (!L_1)
		{
			goto IL_0019;
		}
	}
	{
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_2 = ___source0;
		NullCheck(L_2);
		ErrorEventHandler_tF5863946928B48BE13146ED5FF70AC92678FE222 * L_3 = L_2->get_errorReceived_8();
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_4 = ___source0;
		String_t* L_5 = ___errorStr1;
		NullCheck(L_3);
		ErrorEventHandler_Invoke_m0A812811B673439792D99C125EE4FFE5E358EF6C(L_3, L_4, L_5, /*hidden argument*/NULL);
	}

IL_0019:
	{
		return;
	}
}
// System.Void UnityEngine.Video.VideoPlayer::InvokeSeekCompletedCallback_Internal(UnityEngine.Video.VideoPlayer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VideoPlayer_InvokeSeekCompletedCallback_Internal_mFFB686F5BF044F61495CCF2EFB3517857F98C078 (VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * ___source0, const RuntimeMethod* method)
{
	{
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_0 = ___source0;
		NullCheck(L_0);
		EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * L_1 = L_0->get_seekCompleted_9();
		if (!L_1)
		{
			goto IL_0018;
		}
	}
	{
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_2 = ___source0;
		NullCheck(L_2);
		EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * L_3 = L_2->get_seekCompleted_9();
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_4 = ___source0;
		NullCheck(L_3);
		EventHandler_Invoke_m137A7D976F198147AD939AEF51E157107A3B1FBC(L_3, L_4, /*hidden argument*/NULL);
	}

IL_0018:
	{
		return;
	}
}
// System.Void UnityEngine.Video.VideoPlayer::InvokeClockResyncOccurredCallback_Internal(UnityEngine.Video.VideoPlayer,System.Double)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void VideoPlayer_InvokeClockResyncOccurredCallback_Internal_m21D2B32DDE5ED48F6D6F2ECA9B7A0A2724AF9D6F (VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * ___source0, double ___seconds1, const RuntimeMethod* method)
{
	{
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_0 = ___source0;
		NullCheck(L_0);
		TimeEventHandler_tDD815DAABFADDD98C8993B2A97A2FCE858266BC1 * L_1 = L_0->get_clockResyncOccurred_10();
		if (!L_1)
		{
			goto IL_0019;
		}
	}
	{
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_2 = ___source0;
		NullCheck(L_2);
		TimeEventHandler_tDD815DAABFADDD98C8993B2A97A2FCE858266BC1 * L_3 = L_2->get_clockResyncOccurred_10();
		VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * L_4 = ___source0;
		double L_5 = ___seconds1;
		NullCheck(L_3);
		TimeEventHandler_Invoke_m278E51F2838EC435606BE1CB3AD0E881505FAE10(L_3, L_4, L_5, /*hidden argument*/NULL);
	}

IL_0019:
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
// System.Void UnityEngine.Video.VideoPlayer_ErrorEventHandler::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ErrorEventHandler__ctor_m9E9B3A7A439858703258976491E29057CB17F534 (ErrorEventHandler_tF5863946928B48BE13146ED5FF70AC92678FE222 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void UnityEngine.Video.VideoPlayer_ErrorEventHandler::Invoke(UnityEngine.Video.VideoPlayer,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ErrorEventHandler_Invoke_m0A812811B673439792D99C125EE4FFE5E358EF6C (ErrorEventHandler_tF5863946928B48BE13146ED5FF70AC92678FE222 * __this, VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * ___source0, String_t* ___message1, const RuntimeMethod* method)
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
			if (___parameterCount == 2)
			{
				// open
				typedef void (*FunctionPointerType) (VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, String_t*, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___source0, ___message1, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, String_t*, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___source0, ___message1, targetMethod);
			}
		}
		else if (___parameterCount != 2)
		{
			// open
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker1< String_t* >::Invoke(targetMethod, ___source0, ___message1);
					else
						GenericVirtActionInvoker1< String_t* >::Invoke(targetMethod, ___source0, ___message1);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker1< String_t* >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), ___source0, ___message1);
					else
						VirtActionInvoker1< String_t* >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), ___source0, ___message1);
				}
			}
			else
			{
				typedef void (*FunctionPointerType) (VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, String_t*, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___source0, ___message1, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, String_t*, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___source0, ___message1, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker2< VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, String_t* >::Invoke(targetMethod, targetThis, ___source0, ___message1);
					else
						GenericVirtActionInvoker2< VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, String_t* >::Invoke(targetMethod, targetThis, ___source0, ___message1);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker2< VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, String_t* >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___source0, ___message1);
					else
						VirtActionInvoker2< VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, String_t* >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___source0, ___message1);
				}
			}
			else
			{
				typedef void (*FunctionPointerType) (void*, VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, String_t*, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___source0, ___message1, targetMethod);
			}
		}
	}
}
// System.IAsyncResult UnityEngine.Video.VideoPlayer_ErrorEventHandler::BeginInvoke(UnityEngine.Video.VideoPlayer,System.String,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* ErrorEventHandler_BeginInvoke_mD4C6F60629C221D7702E40D460818B90032FBAA7 (ErrorEventHandler_tF5863946928B48BE13146ED5FF70AC92678FE222 * __this, VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * ___source0, String_t* ___message1, AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4 * ___callback2, RuntimeObject * ___object3, const RuntimeMethod* method)
{
	void *__d_args[3] = {0};
	__d_args[0] = ___source0;
	__d_args[1] = ___message1;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback2, (RuntimeObject*)___object3);
}
// System.Void UnityEngine.Video.VideoPlayer_ErrorEventHandler::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ErrorEventHandler_EndInvoke_m7ABF3F8E15D2EF4AE6961324B66208A7FD127295 (ErrorEventHandler_tF5863946928B48BE13146ED5FF70AC92678FE222 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
// System.Void UnityEngine.Video.VideoPlayer_EventHandler::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void EventHandler__ctor_mA31DCA369A8B7C473F6CE19F6B53D6F3FAF7D6A7 (EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void UnityEngine.Video.VideoPlayer_EventHandler::Invoke(UnityEngine.Video.VideoPlayer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void EventHandler_Invoke_m137A7D976F198147AD939AEF51E157107A3B1FBC (EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * __this, VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * ___source0, const RuntimeMethod* method)
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
				typedef void (*FunctionPointerType) (VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___source0, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___source0, targetMethod);
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
						GenericInterfaceActionInvoker0::Invoke(targetMethod, ___source0);
					else
						GenericVirtActionInvoker0::Invoke(targetMethod, ___source0);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker0::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), ___source0);
					else
						VirtActionInvoker0::Invoke(il2cpp_codegen_method_get_slot(targetMethod), ___source0);
				}
			}
			else
			{
				typedef void (*FunctionPointerType) (VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___source0, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___source0, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker1< VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * >::Invoke(targetMethod, targetThis, ___source0);
					else
						GenericVirtActionInvoker1< VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * >::Invoke(targetMethod, targetThis, ___source0);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker1< VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___source0);
					else
						VirtActionInvoker1< VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___source0);
				}
			}
			else
			{
				typedef void (*FunctionPointerType) (void*, VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___source0, targetMethod);
			}
		}
	}
}
// System.IAsyncResult UnityEngine.Video.VideoPlayer_EventHandler::BeginInvoke(UnityEngine.Video.VideoPlayer,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* EventHandler_BeginInvoke_mCA1B5193B15F3D56BCB40A8DDBA703724040F348 (EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * __this, VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * ___source0, AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4 * ___callback1, RuntimeObject * ___object2, const RuntimeMethod* method)
{
	void *__d_args[2] = {0};
	__d_args[0] = ___source0;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback1, (RuntimeObject*)___object2);
}
// System.Void UnityEngine.Video.VideoPlayer_EventHandler::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void EventHandler_EndInvoke_m95A975D9455F92F836E7E19BDB85538B1EBF4067 (EventHandler_t5069D72E1ED46BD04F19D8D4534811B95A8E2308 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
// System.Void UnityEngine.Video.VideoPlayer_FrameReadyEventHandler::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void FrameReadyEventHandler__ctor_m7DFDBF9203E8F9FC1093E1655C5E2695623D7E3D (FrameReadyEventHandler_t518B277D916AB292680CAA186BCDB3D3EF130422 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void UnityEngine.Video.VideoPlayer_FrameReadyEventHandler::Invoke(UnityEngine.Video.VideoPlayer,System.Int64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void FrameReadyEventHandler_Invoke_m88D0AC1BED08D66B6CFA18DA23C58D10795DDA70 (FrameReadyEventHandler_t518B277D916AB292680CAA186BCDB3D3EF130422 * __this, VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * ___source0, int64_t ___frameIdx1, const RuntimeMethod* method)
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
			if (___parameterCount == 2)
			{
				// open
				typedef void (*FunctionPointerType) (VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, int64_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___source0, ___frameIdx1, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, int64_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___source0, ___frameIdx1, targetMethod);
			}
		}
		else if (___parameterCount != 2)
		{
			// open
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker1< int64_t >::Invoke(targetMethod, ___source0, ___frameIdx1);
					else
						GenericVirtActionInvoker1< int64_t >::Invoke(targetMethod, ___source0, ___frameIdx1);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker1< int64_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), ___source0, ___frameIdx1);
					else
						VirtActionInvoker1< int64_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), ___source0, ___frameIdx1);
				}
			}
			else
			{
				typedef void (*FunctionPointerType) (VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, int64_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___source0, ___frameIdx1, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, int64_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___source0, ___frameIdx1, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker2< VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, int64_t >::Invoke(targetMethod, targetThis, ___source0, ___frameIdx1);
					else
						GenericVirtActionInvoker2< VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, int64_t >::Invoke(targetMethod, targetThis, ___source0, ___frameIdx1);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker2< VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, int64_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___source0, ___frameIdx1);
					else
						VirtActionInvoker2< VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, int64_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___source0, ___frameIdx1);
				}
			}
			else
			{
				typedef void (*FunctionPointerType) (void*, VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, int64_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___source0, ___frameIdx1, targetMethod);
			}
		}
	}
}
// System.IAsyncResult UnityEngine.Video.VideoPlayer_FrameReadyEventHandler::BeginInvoke(UnityEngine.Video.VideoPlayer,System.Int64,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* FrameReadyEventHandler_BeginInvoke_m5DA99DFE61C78E158FF79535447F7649FC09E5F1 (FrameReadyEventHandler_t518B277D916AB292680CAA186BCDB3D3EF130422 * __this, VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * ___source0, int64_t ___frameIdx1, AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4 * ___callback2, RuntimeObject * ___object3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (FrameReadyEventHandler_BeginInvoke_m5DA99DFE61C78E158FF79535447F7649FC09E5F1_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[3] = {0};
	__d_args[0] = ___source0;
	__d_args[1] = Box(Int64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436_il2cpp_TypeInfo_var, &___frameIdx1);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback2, (RuntimeObject*)___object3);
}
// System.Void UnityEngine.Video.VideoPlayer_FrameReadyEventHandler::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void FrameReadyEventHandler_EndInvoke_mC54DCEDB2F8CB30CBC6CD1590E2C08150E3E0CFF (FrameReadyEventHandler_t518B277D916AB292680CAA186BCDB3D3EF130422 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
// System.Void UnityEngine.Video.VideoPlayer_TimeEventHandler::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TimeEventHandler__ctor_mF41715E69B793B1C7DCA3A619CFB05097466523F (TimeEventHandler_tDD815DAABFADDD98C8993B2A97A2FCE858266BC1 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void UnityEngine.Video.VideoPlayer_TimeEventHandler::Invoke(UnityEngine.Video.VideoPlayer,System.Double)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TimeEventHandler_Invoke_m278E51F2838EC435606BE1CB3AD0E881505FAE10 (TimeEventHandler_tDD815DAABFADDD98C8993B2A97A2FCE858266BC1 * __this, VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * ___source0, double ___seconds1, const RuntimeMethod* method)
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
			if (___parameterCount == 2)
			{
				// open
				typedef void (*FunctionPointerType) (VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, double, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___source0, ___seconds1, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, double, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___source0, ___seconds1, targetMethod);
			}
		}
		else if (___parameterCount != 2)
		{
			// open
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker1< double >::Invoke(targetMethod, ___source0, ___seconds1);
					else
						GenericVirtActionInvoker1< double >::Invoke(targetMethod, ___source0, ___seconds1);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker1< double >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), ___source0, ___seconds1);
					else
						VirtActionInvoker1< double >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), ___source0, ___seconds1);
				}
			}
			else
			{
				typedef void (*FunctionPointerType) (VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, double, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___source0, ___seconds1, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, double, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___source0, ___seconds1, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker2< VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, double >::Invoke(targetMethod, targetThis, ___source0, ___seconds1);
					else
						GenericVirtActionInvoker2< VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, double >::Invoke(targetMethod, targetThis, ___source0, ___seconds1);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker2< VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, double >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___source0, ___seconds1);
					else
						VirtActionInvoker2< VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, double >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___source0, ___seconds1);
				}
			}
			else
			{
				typedef void (*FunctionPointerType) (void*, VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 *, double, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___source0, ___seconds1, targetMethod);
			}
		}
	}
}
// System.IAsyncResult UnityEngine.Video.VideoPlayer_TimeEventHandler::BeginInvoke(UnityEngine.Video.VideoPlayer,System.Double,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* TimeEventHandler_BeginInvoke_m184CF1FDB1D643F00FE3C60982ED62EC4888F21D (TimeEventHandler_tDD815DAABFADDD98C8993B2A97A2FCE858266BC1 * __this, VideoPlayer_tFC1C27AF83D59A5213B2AC561B43FD7E19FE02F2 * ___source0, double ___seconds1, AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4 * ___callback2, RuntimeObject * ___object3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (TimeEventHandler_BeginInvoke_m184CF1FDB1D643F00FE3C60982ED62EC4888F21D_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[3] = {0};
	__d_args[0] = ___source0;
	__d_args[1] = Box(Double_t358B8F23BDC52A5DD700E727E204F9F7CDE12409_il2cpp_TypeInfo_var, &___seconds1);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback2, (RuntimeObject*)___object3);
}
// System.Void UnityEngine.Video.VideoPlayer_TimeEventHandler::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TimeEventHandler_EndInvoke_m2759449ABAAE5711D31E414D5906AB042725F7AA (TimeEventHandler_tDD815DAABFADDD98C8993B2A97A2FCE858266BC1 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
