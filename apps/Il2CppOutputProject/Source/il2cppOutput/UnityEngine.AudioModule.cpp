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
struct VirtActionInvoker1
{
	typedef void (*Action)(void*, T1, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		((Action)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};
template <typename R, typename T1, typename T2, typename T3>
struct VirtFuncInvoker3
{
	typedef R (*Func)(void*, T1, T2, T3, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2, T3 p3)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, p3, invokeData.method);
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
template <typename R, typename T1, typename T2, typename T3>
struct GenericVirtFuncInvoker3
{
	typedef R (*Func)(void*, T1, T2, T3, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2, T3 p3)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, p3, invokeData.method);
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
template <typename R, typename T1, typename T2, typename T3>
struct InterfaceFuncInvoker3
{
	typedef R (*Func)(void*, T1, T2, T3, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2, T3 p3)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, p3, invokeData.method);
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
template <typename R, typename T1, typename T2, typename T3>
struct GenericInterfaceFuncInvoker3
{
	typedef R (*Func)(void*, T1, T2, T3, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2, T3 p3)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, p3, invokeData.method);
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

// System.ArgumentException
struct ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1;
// System.AsyncCallback
struct AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4;
// System.Collections.IDictionary
struct IDictionary_t1BD5C1546718A374EA8122FBD6C6EE45331E8CE7;
// System.Delegate
struct Delegate_t;
// System.DelegateData
struct DelegateData_t1BF9F691B56DAE5F8C28C5E084FDE94F15F27BBE;
// System.Delegate[]
struct DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86;
// System.Diagnostics.StackTrace[]
struct StackTraceU5BU5D_t855F09649EA34DEE7C1B6F088E0538E3CCC3F196;
// System.IAsyncResult
struct IAsyncResult_t8E194308510B375B42432981AE5E7488C458D598;
// System.IntPtr[]
struct IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD;
// System.Reflection.MethodInfo
struct MethodInfo_t;
// System.Runtime.Serialization.SafeSerializationManager
struct SafeSerializationManager_t4A754D86B0F784B18CBC36C073BA564BED109770;
// System.Single[]
struct SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5;
// System.String
struct String_t;
// System.Void
struct Void_t22962CB4C05B1D89B55A6E1139F0E87A90987017;
// UnityEngine.AudioClip
struct AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051;
// UnityEngine.AudioClip/PCMReaderCallback
struct PCMReaderCallback_t9B87AB13DCD37957B045554BF28A57697E6B8EFB;
// UnityEngine.AudioClip/PCMSetPositionCallback
struct PCMSetPositionCallback_t092ED33043C0279B5E4D343EBCBD516CEF260801;
// UnityEngine.AudioHighPassFilter
struct AudioHighPassFilter_tAF7C56386170F84139A84528E2EC43DBC9E2C473;
// UnityEngine.AudioLowPassFilter
struct AudioLowPassFilter_t97DD6F50E1F0D2D9404D8A28A97CA3D232BF44CE;
// UnityEngine.AudioSettings/AudioConfigurationChangeHandler
struct AudioConfigurationChangeHandler_t8E0E05D0198D95B5412DC716F87D97020EF54926;
// UnityEngine.AudioSource
struct AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C;
// UnityEngine.Experimental.Audio.AudioSampleProvider
struct AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913;
// UnityEngine.Experimental.Audio.AudioSampleProvider/ConsumeSampleFramesNativeFunction
struct ConsumeSampleFramesNativeFunction_tC1E0B1BFCF2C3D7F87D66FCFA2022369327D931D;
// UnityEngine.Experimental.Audio.AudioSampleProvider/SampleFramesHandler
struct SampleFramesHandler_t5179C92AFBB393A85144E9134A862C161726F6AF;
// UnityEngine.Object
struct Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0;

IL2CPP_EXTERN_C RuntimeClass* ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* AudioSettings_t77B5D69F704CF3B710B0B6970BB62A4BF25A5B31_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Boolean_tB53F6830F670160873277339AA58F15CAED4399C_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* GC_tC1D7BD74E8F44ECCEF5CD2B5D84BFF9AAE02D01D_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IntPtr_t_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* UInt32_t4980FA09003AFAAB5A6E361BA2748EA9A005709B_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C String_t* _stringLiteral2E4C82C746C3EC981DE6270E62A307A7F402A3C0;
IL2CPP_EXTERN_C String_t* _stringLiteral2F43397615CF615962F130AA6604F6D76A60AE5C;
IL2CPP_EXTERN_C String_t* _stringLiteral2F6210C7CB4E004CB3F104E1A7B40B6C6B7121FD;
IL2CPP_EXTERN_C String_t* _stringLiteral4CDCA392B9BF22E4728254EA8AABA605D8A96AC3;
IL2CPP_EXTERN_C String_t* _stringLiteral6E08EC217803485FD1258E704F681704792FDD20;
IL2CPP_EXTERN_C String_t* _stringLiteralB690CB7A4FD9F948BE23247E7090E398B232E4C1;
IL2CPP_EXTERN_C String_t* _stringLiteralE50B0C9845BE0262691292B3BC4739CAB8B57967;
IL2CPP_EXTERN_C const RuntimeMethod* Microphone_Start_mF756A7EBA3E62EF0D138A220482B725D16E96047_RuntimeMethod_var;
IL2CPP_EXTERN_C const uint32_t AudioClipPlayable_Equals_mEB47B5F2E1C643D403FA916C8961F117593DCFC0_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AudioClip__ctor_m52425138C3A036FC847A0E4C4ADA31CEF81CD10D_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AudioConfigurationChangeHandler_BeginInvoke_mB0B0ACF6281B999FA11037CA130CA3C72BEC7827_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AudioMixerPlayable_Equals_m6B84D1A5AEDEAAE12AEFB77319B2662506ABC9C4_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AudioSampleProvider_Dispose_mA11D514354EDA94E8B6AA83B726DF85169C8E29A_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AudioSettings_InvokeOnAudioConfigurationChanged_m8D251791C6A402B12E93C22F43475DE3033FC8E7_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AudioSource_PlayOneShot_mA65B809A4480039CD8337ABF45C0E57E137EED41_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t ConsumeSampleFramesNativeFunction_BeginInvoke_mCFD45D186107B1FD87B1A492904CBCF49DDA28CB_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Microphone_Start_mF756A7EBA3E62EF0D138A220482B725D16E96047_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t PCMSetPositionCallback_BeginInvoke_m88CDF70D75854621CA69ED3D53CD53B8206A5093_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SampleFramesHandler_BeginInvoke_mE516B77CCC50738663D10DDD2D7BDB4391FDFF92_MetadataUsageId;
struct Delegate_t_marshaled_com;
struct Delegate_t_marshaled_pinvoke;
struct Exception_t_marshaled_com;
struct Exception_t_marshaled_pinvoke;

struct DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86;
struct SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5;

IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// <Module>
struct  U3CModuleU3E_t2E16431D825A5D233BFED659B3C516DAB0AC0286 
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

// UnityEngine.AudioSettings
struct  AudioSettings_t77B5D69F704CF3B710B0B6970BB62A4BF25A5B31  : public RuntimeObject
{
public:

public:
};

struct AudioSettings_t77B5D69F704CF3B710B0B6970BB62A4BF25A5B31_StaticFields
{
public:
	// UnityEngine.AudioSettings_AudioConfigurationChangeHandler UnityEngine.AudioSettings::OnAudioConfigurationChanged
	AudioConfigurationChangeHandler_t8E0E05D0198D95B5412DC716F87D97020EF54926 * ___OnAudioConfigurationChanged_0;

public:
	inline static int32_t get_offset_of_OnAudioConfigurationChanged_0() { return static_cast<int32_t>(offsetof(AudioSettings_t77B5D69F704CF3B710B0B6970BB62A4BF25A5B31_StaticFields, ___OnAudioConfigurationChanged_0)); }
	inline AudioConfigurationChangeHandler_t8E0E05D0198D95B5412DC716F87D97020EF54926 * get_OnAudioConfigurationChanged_0() const { return ___OnAudioConfigurationChanged_0; }
	inline AudioConfigurationChangeHandler_t8E0E05D0198D95B5412DC716F87D97020EF54926 ** get_address_of_OnAudioConfigurationChanged_0() { return &___OnAudioConfigurationChanged_0; }
	inline void set_OnAudioConfigurationChanged_0(AudioConfigurationChangeHandler_t8E0E05D0198D95B5412DC716F87D97020EF54926 * value)
	{
		___OnAudioConfigurationChanged_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___OnAudioConfigurationChanged_0), (void*)value);
	}
};


// UnityEngine.Experimental.Audio.AudioSampleProvider
struct  AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913  : public RuntimeObject
{
public:
	// UnityEngine.Experimental.Audio.AudioSampleProvider_ConsumeSampleFramesNativeFunction UnityEngine.Experimental.Audio.AudioSampleProvider::m_ConsumeSampleFramesNativeFunction
	ConsumeSampleFramesNativeFunction_tC1E0B1BFCF2C3D7F87D66FCFA2022369327D931D * ___m_ConsumeSampleFramesNativeFunction_0;
	// System.UInt32 UnityEngine.Experimental.Audio.AudioSampleProvider::<id>k__BackingField
	uint32_t ___U3CidU3Ek__BackingField_1;
	// System.UInt16 UnityEngine.Experimental.Audio.AudioSampleProvider::<trackIndex>k__BackingField
	uint16_t ___U3CtrackIndexU3Ek__BackingField_2;
	// UnityEngine.Object UnityEngine.Experimental.Audio.AudioSampleProvider::<owner>k__BackingField
	Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * ___U3CownerU3Ek__BackingField_3;
	// System.UInt16 UnityEngine.Experimental.Audio.AudioSampleProvider::<channelCount>k__BackingField
	uint16_t ___U3CchannelCountU3Ek__BackingField_4;
	// System.UInt32 UnityEngine.Experimental.Audio.AudioSampleProvider::<sampleRate>k__BackingField
	uint32_t ___U3CsampleRateU3Ek__BackingField_5;
	// UnityEngine.Experimental.Audio.AudioSampleProvider_SampleFramesHandler UnityEngine.Experimental.Audio.AudioSampleProvider::sampleFramesAvailable
	SampleFramesHandler_t5179C92AFBB393A85144E9134A862C161726F6AF * ___sampleFramesAvailable_6;
	// UnityEngine.Experimental.Audio.AudioSampleProvider_SampleFramesHandler UnityEngine.Experimental.Audio.AudioSampleProvider::sampleFramesOverflow
	SampleFramesHandler_t5179C92AFBB393A85144E9134A862C161726F6AF * ___sampleFramesOverflow_7;

public:
	inline static int32_t get_offset_of_m_ConsumeSampleFramesNativeFunction_0() { return static_cast<int32_t>(offsetof(AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913, ___m_ConsumeSampleFramesNativeFunction_0)); }
	inline ConsumeSampleFramesNativeFunction_tC1E0B1BFCF2C3D7F87D66FCFA2022369327D931D * get_m_ConsumeSampleFramesNativeFunction_0() const { return ___m_ConsumeSampleFramesNativeFunction_0; }
	inline ConsumeSampleFramesNativeFunction_tC1E0B1BFCF2C3D7F87D66FCFA2022369327D931D ** get_address_of_m_ConsumeSampleFramesNativeFunction_0() { return &___m_ConsumeSampleFramesNativeFunction_0; }
	inline void set_m_ConsumeSampleFramesNativeFunction_0(ConsumeSampleFramesNativeFunction_tC1E0B1BFCF2C3D7F87D66FCFA2022369327D931D * value)
	{
		___m_ConsumeSampleFramesNativeFunction_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_ConsumeSampleFramesNativeFunction_0), (void*)value);
	}

	inline static int32_t get_offset_of_U3CidU3Ek__BackingField_1() { return static_cast<int32_t>(offsetof(AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913, ___U3CidU3Ek__BackingField_1)); }
	inline uint32_t get_U3CidU3Ek__BackingField_1() const { return ___U3CidU3Ek__BackingField_1; }
	inline uint32_t* get_address_of_U3CidU3Ek__BackingField_1() { return &___U3CidU3Ek__BackingField_1; }
	inline void set_U3CidU3Ek__BackingField_1(uint32_t value)
	{
		___U3CidU3Ek__BackingField_1 = value;
	}

	inline static int32_t get_offset_of_U3CtrackIndexU3Ek__BackingField_2() { return static_cast<int32_t>(offsetof(AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913, ___U3CtrackIndexU3Ek__BackingField_2)); }
	inline uint16_t get_U3CtrackIndexU3Ek__BackingField_2() const { return ___U3CtrackIndexU3Ek__BackingField_2; }
	inline uint16_t* get_address_of_U3CtrackIndexU3Ek__BackingField_2() { return &___U3CtrackIndexU3Ek__BackingField_2; }
	inline void set_U3CtrackIndexU3Ek__BackingField_2(uint16_t value)
	{
		___U3CtrackIndexU3Ek__BackingField_2 = value;
	}

	inline static int32_t get_offset_of_U3CownerU3Ek__BackingField_3() { return static_cast<int32_t>(offsetof(AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913, ___U3CownerU3Ek__BackingField_3)); }
	inline Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * get_U3CownerU3Ek__BackingField_3() const { return ___U3CownerU3Ek__BackingField_3; }
	inline Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 ** get_address_of_U3CownerU3Ek__BackingField_3() { return &___U3CownerU3Ek__BackingField_3; }
	inline void set_U3CownerU3Ek__BackingField_3(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * value)
	{
		___U3CownerU3Ek__BackingField_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___U3CownerU3Ek__BackingField_3), (void*)value);
	}

	inline static int32_t get_offset_of_U3CchannelCountU3Ek__BackingField_4() { return static_cast<int32_t>(offsetof(AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913, ___U3CchannelCountU3Ek__BackingField_4)); }
	inline uint16_t get_U3CchannelCountU3Ek__BackingField_4() const { return ___U3CchannelCountU3Ek__BackingField_4; }
	inline uint16_t* get_address_of_U3CchannelCountU3Ek__BackingField_4() { return &___U3CchannelCountU3Ek__BackingField_4; }
	inline void set_U3CchannelCountU3Ek__BackingField_4(uint16_t value)
	{
		___U3CchannelCountU3Ek__BackingField_4 = value;
	}

	inline static int32_t get_offset_of_U3CsampleRateU3Ek__BackingField_5() { return static_cast<int32_t>(offsetof(AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913, ___U3CsampleRateU3Ek__BackingField_5)); }
	inline uint32_t get_U3CsampleRateU3Ek__BackingField_5() const { return ___U3CsampleRateU3Ek__BackingField_5; }
	inline uint32_t* get_address_of_U3CsampleRateU3Ek__BackingField_5() { return &___U3CsampleRateU3Ek__BackingField_5; }
	inline void set_U3CsampleRateU3Ek__BackingField_5(uint32_t value)
	{
		___U3CsampleRateU3Ek__BackingField_5 = value;
	}

	inline static int32_t get_offset_of_sampleFramesAvailable_6() { return static_cast<int32_t>(offsetof(AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913, ___sampleFramesAvailable_6)); }
	inline SampleFramesHandler_t5179C92AFBB393A85144E9134A862C161726F6AF * get_sampleFramesAvailable_6() const { return ___sampleFramesAvailable_6; }
	inline SampleFramesHandler_t5179C92AFBB393A85144E9134A862C161726F6AF ** get_address_of_sampleFramesAvailable_6() { return &___sampleFramesAvailable_6; }
	inline void set_sampleFramesAvailable_6(SampleFramesHandler_t5179C92AFBB393A85144E9134A862C161726F6AF * value)
	{
		___sampleFramesAvailable_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___sampleFramesAvailable_6), (void*)value);
	}

	inline static int32_t get_offset_of_sampleFramesOverflow_7() { return static_cast<int32_t>(offsetof(AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913, ___sampleFramesOverflow_7)); }
	inline SampleFramesHandler_t5179C92AFBB393A85144E9134A862C161726F6AF * get_sampleFramesOverflow_7() const { return ___sampleFramesOverflow_7; }
	inline SampleFramesHandler_t5179C92AFBB393A85144E9134A862C161726F6AF ** get_address_of_sampleFramesOverflow_7() { return &___sampleFramesOverflow_7; }
	inline void set_sampleFramesOverflow_7(SampleFramesHandler_t5179C92AFBB393A85144E9134A862C161726F6AF * value)
	{
		___sampleFramesOverflow_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___sampleFramesOverflow_7), (void*)value);
	}
};


// UnityEngine.Microphone
struct  Microphone_t726BEF7F4F60B1E2C4C81333CD7C33D4987434E5  : public RuntimeObject
{
public:

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


// System.UInt64
struct  UInt64_tA02DF3B59C8FC4A849BD207DA11038CC64E4CB4E 
{
public:
	// System.UInt64 System.UInt64::m_value
	uint64_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(UInt64_tA02DF3B59C8FC4A849BD207DA11038CC64E4CB4E, ___m_value_0)); }
	inline uint64_t get_m_value_0() const { return ___m_value_0; }
	inline uint64_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(uint64_t value)
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

// System.Exception
struct  Exception_t  : public RuntimeObject
{
public:
	// System.String System.Exception::_className
	String_t* ____className_1;
	// System.String System.Exception::_message
	String_t* ____message_2;
	// System.Collections.IDictionary System.Exception::_data
	RuntimeObject* ____data_3;
	// System.Exception System.Exception::_innerException
	Exception_t * ____innerException_4;
	// System.String System.Exception::_helpURL
	String_t* ____helpURL_5;
	// System.Object System.Exception::_stackTrace
	RuntimeObject * ____stackTrace_6;
	// System.String System.Exception::_stackTraceString
	String_t* ____stackTraceString_7;
	// System.String System.Exception::_remoteStackTraceString
	String_t* ____remoteStackTraceString_8;
	// System.Int32 System.Exception::_remoteStackIndex
	int32_t ____remoteStackIndex_9;
	// System.Object System.Exception::_dynamicMethods
	RuntimeObject * ____dynamicMethods_10;
	// System.Int32 System.Exception::_HResult
	int32_t ____HResult_11;
	// System.String System.Exception::_source
	String_t* ____source_12;
	// System.Runtime.Serialization.SafeSerializationManager System.Exception::_safeSerializationManager
	SafeSerializationManager_t4A754D86B0F784B18CBC36C073BA564BED109770 * ____safeSerializationManager_13;
	// System.Diagnostics.StackTrace[] System.Exception::captured_traces
	StackTraceU5BU5D_t855F09649EA34DEE7C1B6F088E0538E3CCC3F196* ___captured_traces_14;
	// System.IntPtr[] System.Exception::native_trace_ips
	IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD* ___native_trace_ips_15;

public:
	inline static int32_t get_offset_of__className_1() { return static_cast<int32_t>(offsetof(Exception_t, ____className_1)); }
	inline String_t* get__className_1() const { return ____className_1; }
	inline String_t** get_address_of__className_1() { return &____className_1; }
	inline void set__className_1(String_t* value)
	{
		____className_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____className_1), (void*)value);
	}

	inline static int32_t get_offset_of__message_2() { return static_cast<int32_t>(offsetof(Exception_t, ____message_2)); }
	inline String_t* get__message_2() const { return ____message_2; }
	inline String_t** get_address_of__message_2() { return &____message_2; }
	inline void set__message_2(String_t* value)
	{
		____message_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____message_2), (void*)value);
	}

	inline static int32_t get_offset_of__data_3() { return static_cast<int32_t>(offsetof(Exception_t, ____data_3)); }
	inline RuntimeObject* get__data_3() const { return ____data_3; }
	inline RuntimeObject** get_address_of__data_3() { return &____data_3; }
	inline void set__data_3(RuntimeObject* value)
	{
		____data_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____data_3), (void*)value);
	}

	inline static int32_t get_offset_of__innerException_4() { return static_cast<int32_t>(offsetof(Exception_t, ____innerException_4)); }
	inline Exception_t * get__innerException_4() const { return ____innerException_4; }
	inline Exception_t ** get_address_of__innerException_4() { return &____innerException_4; }
	inline void set__innerException_4(Exception_t * value)
	{
		____innerException_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____innerException_4), (void*)value);
	}

	inline static int32_t get_offset_of__helpURL_5() { return static_cast<int32_t>(offsetof(Exception_t, ____helpURL_5)); }
	inline String_t* get__helpURL_5() const { return ____helpURL_5; }
	inline String_t** get_address_of__helpURL_5() { return &____helpURL_5; }
	inline void set__helpURL_5(String_t* value)
	{
		____helpURL_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____helpURL_5), (void*)value);
	}

	inline static int32_t get_offset_of__stackTrace_6() { return static_cast<int32_t>(offsetof(Exception_t, ____stackTrace_6)); }
	inline RuntimeObject * get__stackTrace_6() const { return ____stackTrace_6; }
	inline RuntimeObject ** get_address_of__stackTrace_6() { return &____stackTrace_6; }
	inline void set__stackTrace_6(RuntimeObject * value)
	{
		____stackTrace_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____stackTrace_6), (void*)value);
	}

	inline static int32_t get_offset_of__stackTraceString_7() { return static_cast<int32_t>(offsetof(Exception_t, ____stackTraceString_7)); }
	inline String_t* get__stackTraceString_7() const { return ____stackTraceString_7; }
	inline String_t** get_address_of__stackTraceString_7() { return &____stackTraceString_7; }
	inline void set__stackTraceString_7(String_t* value)
	{
		____stackTraceString_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____stackTraceString_7), (void*)value);
	}

	inline static int32_t get_offset_of__remoteStackTraceString_8() { return static_cast<int32_t>(offsetof(Exception_t, ____remoteStackTraceString_8)); }
	inline String_t* get__remoteStackTraceString_8() const { return ____remoteStackTraceString_8; }
	inline String_t** get_address_of__remoteStackTraceString_8() { return &____remoteStackTraceString_8; }
	inline void set__remoteStackTraceString_8(String_t* value)
	{
		____remoteStackTraceString_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____remoteStackTraceString_8), (void*)value);
	}

	inline static int32_t get_offset_of__remoteStackIndex_9() { return static_cast<int32_t>(offsetof(Exception_t, ____remoteStackIndex_9)); }
	inline int32_t get__remoteStackIndex_9() const { return ____remoteStackIndex_9; }
	inline int32_t* get_address_of__remoteStackIndex_9() { return &____remoteStackIndex_9; }
	inline void set__remoteStackIndex_9(int32_t value)
	{
		____remoteStackIndex_9 = value;
	}

	inline static int32_t get_offset_of__dynamicMethods_10() { return static_cast<int32_t>(offsetof(Exception_t, ____dynamicMethods_10)); }
	inline RuntimeObject * get__dynamicMethods_10() const { return ____dynamicMethods_10; }
	inline RuntimeObject ** get_address_of__dynamicMethods_10() { return &____dynamicMethods_10; }
	inline void set__dynamicMethods_10(RuntimeObject * value)
	{
		____dynamicMethods_10 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____dynamicMethods_10), (void*)value);
	}

	inline static int32_t get_offset_of__HResult_11() { return static_cast<int32_t>(offsetof(Exception_t, ____HResult_11)); }
	inline int32_t get__HResult_11() const { return ____HResult_11; }
	inline int32_t* get_address_of__HResult_11() { return &____HResult_11; }
	inline void set__HResult_11(int32_t value)
	{
		____HResult_11 = value;
	}

	inline static int32_t get_offset_of__source_12() { return static_cast<int32_t>(offsetof(Exception_t, ____source_12)); }
	inline String_t* get__source_12() const { return ____source_12; }
	inline String_t** get_address_of__source_12() { return &____source_12; }
	inline void set__source_12(String_t* value)
	{
		____source_12 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____source_12), (void*)value);
	}

	inline static int32_t get_offset_of__safeSerializationManager_13() { return static_cast<int32_t>(offsetof(Exception_t, ____safeSerializationManager_13)); }
	inline SafeSerializationManager_t4A754D86B0F784B18CBC36C073BA564BED109770 * get__safeSerializationManager_13() const { return ____safeSerializationManager_13; }
	inline SafeSerializationManager_t4A754D86B0F784B18CBC36C073BA564BED109770 ** get_address_of__safeSerializationManager_13() { return &____safeSerializationManager_13; }
	inline void set__safeSerializationManager_13(SafeSerializationManager_t4A754D86B0F784B18CBC36C073BA564BED109770 * value)
	{
		____safeSerializationManager_13 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____safeSerializationManager_13), (void*)value);
	}

	inline static int32_t get_offset_of_captured_traces_14() { return static_cast<int32_t>(offsetof(Exception_t, ___captured_traces_14)); }
	inline StackTraceU5BU5D_t855F09649EA34DEE7C1B6F088E0538E3CCC3F196* get_captured_traces_14() const { return ___captured_traces_14; }
	inline StackTraceU5BU5D_t855F09649EA34DEE7C1B6F088E0538E3CCC3F196** get_address_of_captured_traces_14() { return &___captured_traces_14; }
	inline void set_captured_traces_14(StackTraceU5BU5D_t855F09649EA34DEE7C1B6F088E0538E3CCC3F196* value)
	{
		___captured_traces_14 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___captured_traces_14), (void*)value);
	}

	inline static int32_t get_offset_of_native_trace_ips_15() { return static_cast<int32_t>(offsetof(Exception_t, ___native_trace_ips_15)); }
	inline IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD* get_native_trace_ips_15() const { return ___native_trace_ips_15; }
	inline IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD** get_address_of_native_trace_ips_15() { return &___native_trace_ips_15; }
	inline void set_native_trace_ips_15(IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD* value)
	{
		___native_trace_ips_15 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___native_trace_ips_15), (void*)value);
	}
};

struct Exception_t_StaticFields
{
public:
	// System.Object System.Exception::s_EDILock
	RuntimeObject * ___s_EDILock_0;

public:
	inline static int32_t get_offset_of_s_EDILock_0() { return static_cast<int32_t>(offsetof(Exception_t_StaticFields, ___s_EDILock_0)); }
	inline RuntimeObject * get_s_EDILock_0() const { return ___s_EDILock_0; }
	inline RuntimeObject ** get_address_of_s_EDILock_0() { return &___s_EDILock_0; }
	inline void set_s_EDILock_0(RuntimeObject * value)
	{
		___s_EDILock_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_EDILock_0), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of System.Exception
struct Exception_t_marshaled_pinvoke
{
	char* ____className_1;
	char* ____message_2;
	RuntimeObject* ____data_3;
	Exception_t_marshaled_pinvoke* ____innerException_4;
	char* ____helpURL_5;
	Il2CppIUnknown* ____stackTrace_6;
	char* ____stackTraceString_7;
	char* ____remoteStackTraceString_8;
	int32_t ____remoteStackIndex_9;
	Il2CppIUnknown* ____dynamicMethods_10;
	int32_t ____HResult_11;
	char* ____source_12;
	SafeSerializationManager_t4A754D86B0F784B18CBC36C073BA564BED109770 * ____safeSerializationManager_13;
	StackTraceU5BU5D_t855F09649EA34DEE7C1B6F088E0538E3CCC3F196* ___captured_traces_14;
	Il2CppSafeArray/*NONE*/* ___native_trace_ips_15;
};
// Native definition for COM marshalling of System.Exception
struct Exception_t_marshaled_com
{
	Il2CppChar* ____className_1;
	Il2CppChar* ____message_2;
	RuntimeObject* ____data_3;
	Exception_t_marshaled_com* ____innerException_4;
	Il2CppChar* ____helpURL_5;
	Il2CppIUnknown* ____stackTrace_6;
	Il2CppChar* ____stackTraceString_7;
	Il2CppChar* ____remoteStackTraceString_8;
	int32_t ____remoteStackIndex_9;
	Il2CppIUnknown* ____dynamicMethods_10;
	int32_t ____HResult_11;
	Il2CppChar* ____source_12;
	SafeSerializationManager_t4A754D86B0F784B18CBC36C073BA564BED109770 * ____safeSerializationManager_13;
	StackTraceU5BU5D_t855F09649EA34DEE7C1B6F088E0538E3CCC3F196* ___captured_traces_14;
	Il2CppSafeArray/*NONE*/* ___native_trace_ips_15;
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


// UnityEngine.Playables.PlayableOutputHandle
struct  PlayableOutputHandle_t0D0C9D8ACC1A4061BD4EAEB61F3EE0357052F922 
{
public:
	// System.IntPtr UnityEngine.Playables.PlayableOutputHandle::m_Handle
	intptr_t ___m_Handle_0;
	// System.UInt32 UnityEngine.Playables.PlayableOutputHandle::m_Version
	uint32_t ___m_Version_1;

public:
	inline static int32_t get_offset_of_m_Handle_0() { return static_cast<int32_t>(offsetof(PlayableOutputHandle_t0D0C9D8ACC1A4061BD4EAEB61F3EE0357052F922, ___m_Handle_0)); }
	inline intptr_t get_m_Handle_0() const { return ___m_Handle_0; }
	inline intptr_t* get_address_of_m_Handle_0() { return &___m_Handle_0; }
	inline void set_m_Handle_0(intptr_t value)
	{
		___m_Handle_0 = value;
	}

	inline static int32_t get_offset_of_m_Version_1() { return static_cast<int32_t>(offsetof(PlayableOutputHandle_t0D0C9D8ACC1A4061BD4EAEB61F3EE0357052F922, ___m_Version_1)); }
	inline uint32_t get_m_Version_1() const { return ___m_Version_1; }
	inline uint32_t* get_address_of_m_Version_1() { return &___m_Version_1; }
	inline void set_m_Version_1(uint32_t value)
	{
		___m_Version_1 = value;
	}
};

struct PlayableOutputHandle_t0D0C9D8ACC1A4061BD4EAEB61F3EE0357052F922_StaticFields
{
public:
	// UnityEngine.Playables.PlayableOutputHandle UnityEngine.Playables.PlayableOutputHandle::m_Null
	PlayableOutputHandle_t0D0C9D8ACC1A4061BD4EAEB61F3EE0357052F922  ___m_Null_2;

public:
	inline static int32_t get_offset_of_m_Null_2() { return static_cast<int32_t>(offsetof(PlayableOutputHandle_t0D0C9D8ACC1A4061BD4EAEB61F3EE0357052F922_StaticFields, ___m_Null_2)); }
	inline PlayableOutputHandle_t0D0C9D8ACC1A4061BD4EAEB61F3EE0357052F922  get_m_Null_2() const { return ___m_Null_2; }
	inline PlayableOutputHandle_t0D0C9D8ACC1A4061BD4EAEB61F3EE0357052F922 * get_address_of_m_Null_2() { return &___m_Null_2; }
	inline void set_m_Null_2(PlayableOutputHandle_t0D0C9D8ACC1A4061BD4EAEB61F3EE0357052F922  value)
	{
		___m_Null_2 = value;
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

// System.SystemException
struct  SystemException_t5380468142AA850BE4A341D7AF3EAB9C78746782  : public Exception_t
{
public:

public:
};


// UnityEngine.Audio.AudioClipPlayable
struct  AudioClipPlayable_t6094311F945E65BC29F85B23A81E8426D596553C 
{
public:
	// UnityEngine.Playables.PlayableHandle UnityEngine.Audio.AudioClipPlayable::m_Handle
	PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  ___m_Handle_0;

public:
	inline static int32_t get_offset_of_m_Handle_0() { return static_cast<int32_t>(offsetof(AudioClipPlayable_t6094311F945E65BC29F85B23A81E8426D596553C, ___m_Handle_0)); }
	inline PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  get_m_Handle_0() const { return ___m_Handle_0; }
	inline PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182 * get_address_of_m_Handle_0() { return &___m_Handle_0; }
	inline void set_m_Handle_0(PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  value)
	{
		___m_Handle_0 = value;
	}
};


// UnityEngine.Audio.AudioMixerPlayable
struct  AudioMixerPlayable_t2C445EB39F9111CCFF7E2E1F813B22007862FA9F 
{
public:
	// UnityEngine.Playables.PlayableHandle UnityEngine.Audio.AudioMixerPlayable::m_Handle
	PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  ___m_Handle_0;

public:
	inline static int32_t get_offset_of_m_Handle_0() { return static_cast<int32_t>(offsetof(AudioMixerPlayable_t2C445EB39F9111CCFF7E2E1F813B22007862FA9F, ___m_Handle_0)); }
	inline PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  get_m_Handle_0() const { return ___m_Handle_0; }
	inline PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182 * get_address_of_m_Handle_0() { return &___m_Handle_0; }
	inline void set_m_Handle_0(PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  value)
	{
		___m_Handle_0 = value;
	}
};


// UnityEngine.Audio.AudioPlayableOutput
struct  AudioPlayableOutput_tD2671908FEE2832112E8A3B611089A2558A4DA6B 
{
public:
	// UnityEngine.Playables.PlayableOutputHandle UnityEngine.Audio.AudioPlayableOutput::m_Handle
	PlayableOutputHandle_t0D0C9D8ACC1A4061BD4EAEB61F3EE0357052F922  ___m_Handle_0;

public:
	inline static int32_t get_offset_of_m_Handle_0() { return static_cast<int32_t>(offsetof(AudioPlayableOutput_tD2671908FEE2832112E8A3B611089A2558A4DA6B, ___m_Handle_0)); }
	inline PlayableOutputHandle_t0D0C9D8ACC1A4061BD4EAEB61F3EE0357052F922  get_m_Handle_0() const { return ___m_Handle_0; }
	inline PlayableOutputHandle_t0D0C9D8ACC1A4061BD4EAEB61F3EE0357052F922 * get_address_of_m_Handle_0() { return &___m_Handle_0; }
	inline void set_m_Handle_0(PlayableOutputHandle_t0D0C9D8ACC1A4061BD4EAEB61F3EE0357052F922  value)
	{
		___m_Handle_0 = value;
	}
};


// UnityEngine.AudioClip
struct  AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051  : public Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0
{
public:
	// UnityEngine.AudioClip_PCMReaderCallback UnityEngine.AudioClip::m_PCMReaderCallback
	PCMReaderCallback_t9B87AB13DCD37957B045554BF28A57697E6B8EFB * ___m_PCMReaderCallback_4;
	// UnityEngine.AudioClip_PCMSetPositionCallback UnityEngine.AudioClip::m_PCMSetPositionCallback
	PCMSetPositionCallback_t092ED33043C0279B5E4D343EBCBD516CEF260801 * ___m_PCMSetPositionCallback_5;

public:
	inline static int32_t get_offset_of_m_PCMReaderCallback_4() { return static_cast<int32_t>(offsetof(AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051, ___m_PCMReaderCallback_4)); }
	inline PCMReaderCallback_t9B87AB13DCD37957B045554BF28A57697E6B8EFB * get_m_PCMReaderCallback_4() const { return ___m_PCMReaderCallback_4; }
	inline PCMReaderCallback_t9B87AB13DCD37957B045554BF28A57697E6B8EFB ** get_address_of_m_PCMReaderCallback_4() { return &___m_PCMReaderCallback_4; }
	inline void set_m_PCMReaderCallback_4(PCMReaderCallback_t9B87AB13DCD37957B045554BF28A57697E6B8EFB * value)
	{
		___m_PCMReaderCallback_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_PCMReaderCallback_4), (void*)value);
	}

	inline static int32_t get_offset_of_m_PCMSetPositionCallback_5() { return static_cast<int32_t>(offsetof(AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051, ___m_PCMSetPositionCallback_5)); }
	inline PCMSetPositionCallback_t092ED33043C0279B5E4D343EBCBD516CEF260801 * get_m_PCMSetPositionCallback_5() const { return ___m_PCMSetPositionCallback_5; }
	inline PCMSetPositionCallback_t092ED33043C0279B5E4D343EBCBD516CEF260801 ** get_address_of_m_PCMSetPositionCallback_5() { return &___m_PCMSetPositionCallback_5; }
	inline void set_m_PCMSetPositionCallback_5(PCMSetPositionCallback_t092ED33043C0279B5E4D343EBCBD516CEF260801 * value)
	{
		___m_PCMSetPositionCallback_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_PCMSetPositionCallback_5), (void*)value);
	}
};


// UnityEngine.Component
struct  Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621  : public Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0
{
public:

public:
};


// System.ArgumentException
struct  ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1  : public SystemException_t5380468142AA850BE4A341D7AF3EAB9C78746782
{
public:
	// System.String System.ArgumentException::m_paramName
	String_t* ___m_paramName_17;

public:
	inline static int32_t get_offset_of_m_paramName_17() { return static_cast<int32_t>(offsetof(ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1, ___m_paramName_17)); }
	inline String_t* get_m_paramName_17() const { return ___m_paramName_17; }
	inline String_t** get_address_of_m_paramName_17() { return &___m_paramName_17; }
	inline void set_m_paramName_17(String_t* value)
	{
		___m_paramName_17 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_paramName_17), (void*)value);
	}
};


// System.AsyncCallback
struct  AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4  : public MulticastDelegate_t
{
public:

public:
};


// UnityEngine.AudioClip_PCMReaderCallback
struct  PCMReaderCallback_t9B87AB13DCD37957B045554BF28A57697E6B8EFB  : public MulticastDelegate_t
{
public:

public:
};


// UnityEngine.AudioClip_PCMSetPositionCallback
struct  PCMSetPositionCallback_t092ED33043C0279B5E4D343EBCBD516CEF260801  : public MulticastDelegate_t
{
public:

public:
};


// UnityEngine.AudioSettings_AudioConfigurationChangeHandler
struct  AudioConfigurationChangeHandler_t8E0E05D0198D95B5412DC716F87D97020EF54926  : public MulticastDelegate_t
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


// UnityEngine.Experimental.Audio.AudioSampleProvider_ConsumeSampleFramesNativeFunction
struct  ConsumeSampleFramesNativeFunction_tC1E0B1BFCF2C3D7F87D66FCFA2022369327D931D  : public MulticastDelegate_t
{
public:

public:
};


// UnityEngine.Experimental.Audio.AudioSampleProvider_SampleFramesHandler
struct  SampleFramesHandler_t5179C92AFBB393A85144E9134A862C161726F6AF  : public MulticastDelegate_t
{
public:

public:
};


// UnityEngine.AudioBehaviour
struct  AudioBehaviour_tC612EC4E17A648A5C568621F3FBF1DBD773C71C7  : public Behaviour_tBDC7E9C3C898AD8348891B82D3E345801D920CA8
{
public:

public:
};


// UnityEngine.AudioHighPassFilter
struct  AudioHighPassFilter_tAF7C56386170F84139A84528E2EC43DBC9E2C473  : public Behaviour_tBDC7E9C3C898AD8348891B82D3E345801D920CA8
{
public:

public:
};


// UnityEngine.AudioLowPassFilter
struct  AudioLowPassFilter_t97DD6F50E1F0D2D9404D8A28A97CA3D232BF44CE  : public Behaviour_tBDC7E9C3C898AD8348891B82D3E345801D920CA8
{
public:

public:
};


// UnityEngine.AudioListener
struct  AudioListener_tE3E1467B84A4AFD509947B44A7C8ACFB67FF2099  : public AudioBehaviour_tC612EC4E17A648A5C568621F3FBF1DBD773C71C7
{
public:

public:
};


// UnityEngine.AudioSource
struct  AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C  : public AudioBehaviour_tC612EC4E17A648A5C568621F3FBF1DBD773C71C7
{
public:

public:
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
// System.Single[]
struct SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) float m_Items[1];

public:
	inline float GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline float* GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, float value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline float GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline float* GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, float value)
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



// UnityEngine.Playables.PlayableHandle UnityEngine.Audio.AudioClipPlayable::GetHandle()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  AudioClipPlayable_GetHandle_mEE8F62E9DA2A0DDDB064A9AA2391909C425CB3B1 (AudioClipPlayable_t6094311F945E65BC29F85B23A81E8426D596553C * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Playables.PlayableHandle::op_Equality(UnityEngine.Playables.PlayableHandle,UnityEngine.Playables.PlayableHandle)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool PlayableHandle_op_Equality_mBA774AE123AF794A1EB55148206CDD52DAFA42DF (PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  ___x0, PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  ___y1, const RuntimeMethod* method);
// System.Boolean UnityEngine.Audio.AudioClipPlayable::Equals(UnityEngine.Audio.AudioClipPlayable)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool AudioClipPlayable_Equals_mEB47B5F2E1C643D403FA916C8961F117593DCFC0 (AudioClipPlayable_t6094311F945E65BC29F85B23A81E8426D596553C * __this, AudioClipPlayable_t6094311F945E65BC29F85B23A81E8426D596553C  ___other0, const RuntimeMethod* method);
// UnityEngine.Playables.PlayableHandle UnityEngine.Audio.AudioMixerPlayable::GetHandle()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  AudioMixerPlayable_GetHandle_mDBC7135DF653E0E19675B6694EA89958E609587D (AudioMixerPlayable_t2C445EB39F9111CCFF7E2E1F813B22007862FA9F * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Audio.AudioMixerPlayable::Equals(UnityEngine.Audio.AudioMixerPlayable)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool AudioMixerPlayable_Equals_m6B84D1A5AEDEAAE12AEFB77319B2662506ABC9C4 (AudioMixerPlayable_t2C445EB39F9111CCFF7E2E1F813B22007862FA9F * __this, AudioMixerPlayable_t2C445EB39F9111CCFF7E2E1F813B22007862FA9F  ___other0, const RuntimeMethod* method);
// System.Void UnityEngine.Object::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object__ctor_m091EBAEBC7919B0391ABDAFB7389ADC12206525B (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.AudioClip/PCMReaderCallback::Invoke(System.Single[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PCMReaderCallback_Invoke_m7B101820DB35BEFC8D2724DF96900367863B93B6 (PCMReaderCallback_t9B87AB13DCD37957B045554BF28A57697E6B8EFB * __this, SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* ___data0, const RuntimeMethod* method);
// System.Void UnityEngine.AudioClip/PCMSetPositionCallback::Invoke(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PCMSetPositionCallback_Invoke_m8EA4736B43191A8E6F95E1548AFF124519EC533C (PCMSetPositionCallback_t092ED33043C0279B5E4D343EBCBD516CEF260801 * __this, int32_t ___position0, const RuntimeMethod* method);
// System.Void UnityEngine.AudioSettings/AudioConfigurationChangeHandler::Invoke(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioConfigurationChangeHandler_Invoke_m62D72B397E1DC117C8C92A450D2C86C535A2BF49 (AudioConfigurationChangeHandler_t8E0E05D0198D95B5412DC716F87D97020EF54926 * __this, bool ___deviceWasChanged0, const RuntimeMethod* method);
// System.Void UnityEngine.AudioSource::SetPitch(UnityEngine.AudioSource,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSource_SetPitch_mB65EF59C56A49FD68CD3361B2C34E07F89F1244B (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C * ___source0, float ___pitch1, const RuntimeMethod* method);
// System.Void UnityEngine.AudioSource::PlayHelper(UnityEngine.AudioSource,System.UInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSource_PlayHelper_m361C17B583E05D2A5FA0F03BD7CD98D74FBF83AC (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C * ___source0, uint64_t ___delay1, const RuntimeMethod* method);
// System.Void UnityEngine.AudioSource::PlayOneShot(UnityEngine.AudioClip,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSource_PlayOneShot_mA65B809A4480039CD8337ABF45C0E57E137EED41 (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C * __this, AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * ___clip0, float ___volumeScale1, const RuntimeMethod* method);
// System.Boolean UnityEngine.Object::op_Equality(UnityEngine.Object,UnityEngine.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Object_op_Equality_mBC2401774F3BE33E8CF6F0A8148E66C95D6CFF1C (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * ___x0, Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * ___y1, const RuntimeMethod* method);
// System.Void UnityEngine.Debug::LogWarning(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Debug_LogWarning_m37338644DC81F640CCDFEAE35A223F0E965F0568 (RuntimeObject * ___message0, const RuntimeMethod* method);
// System.Void UnityEngine.AudioSource::PlayOneShotHelper(UnityEngine.AudioSource,UnityEngine.AudioClip,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSource_PlayOneShotHelper_mDAEDF5E0C56C665DE62CDF26E7B98149B4A71704 (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C * ___source0, AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * ___clip1, float ___volumeScale2, const RuntimeMethod* method);
// System.Void UnityEngine.Experimental.Audio.AudioSampleProvider::set_owner(UnityEngine.Object)
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR void AudioSampleProvider_set_owner_m21FC4073346BFBE33E2B1567ECEC8ECD373B3069_inline (AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 * __this, Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * ___value0, const RuntimeMethod* method);
// System.Void UnityEngine.Experimental.Audio.AudioSampleProvider::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSampleProvider_Dispose_mA11D514354EDA94E8B6AA83B726DF85169C8E29A (AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 * __this, const RuntimeMethod* method);
// System.Void System.Object::Finalize()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object_Finalize_m4015B7D3A44DE125C5FE34D7276CD4697C06F380 (RuntimeObject * __this, const RuntimeMethod* method);
// System.UInt32 UnityEngine.Experimental.Audio.AudioSampleProvider::get_id()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t AudioSampleProvider_get_id_m9E16910C6A2335F87E6B257D3059A8BBC8D7253A (AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Experimental.Audio.AudioSampleProvider::InternalSetScriptingPtr(System.UInt32,UnityEngine.Experimental.Audio.AudioSampleProvider)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSampleProvider_InternalSetScriptingPtr_m018841DA2C6D92E2322F1D041E4C88A054991AF9 (uint32_t ___providerId0, AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 * ___provider1, const RuntimeMethod* method);
// UnityEngine.Object UnityEngine.Experimental.Audio.AudioSampleProvider::get_owner()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * AudioSampleProvider_get_owner_m822057CE02CF8664D7BF569CDB41D8EB154251FE (AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Experimental.Audio.AudioSampleProvider::InternalRemove(System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSampleProvider_InternalRemove_m3CD8E2D57A6DC5522C71436A854926C346CEBF11 (uint32_t ___providerId0, const RuntimeMethod* method);
// System.Void UnityEngine.Experimental.Audio.AudioSampleProvider::set_id(System.UInt32)
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR void AudioSampleProvider_set_id_mB98E8435407CAD305BBEAA91B18CBC12562DFAF9_inline (AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 * __this, uint32_t ___value0, const RuntimeMethod* method);
// System.Void System.GC::SuppressFinalize(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GC_SuppressFinalize_m037319A9B95A5BA437E806DE592802225EE5B425 (RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.Void UnityEngine.Experimental.Audio.AudioSampleProvider/SampleFramesHandler::Invoke(UnityEngine.Experimental.Audio.AudioSampleProvider,System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SampleFramesHandler_Invoke_m52F0148F680B36E04A7F850E617FBEF1CA9809FD (SampleFramesHandler_t5179C92AFBB393A85144E9134A862C161726F6AF * __this, AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 * ___provider0, uint32_t ___sampleFrameCount1, const RuntimeMethod* method);
// System.Int32 UnityEngine.Microphone::GetMicrophoneDeviceIDFromName(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Microphone_GetMicrophoneDeviceIDFromName_m08C8735264E4D5FEAED224C8E7957CF5916D9A5E (String_t* ___name0, const RuntimeMethod* method);
// System.String System.String::Concat(System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Concat_mB78D0094592718DA6D5DB6C712A9C225631666BE (String_t* ___str00, String_t* ___str11, const RuntimeMethod* method);
// System.Void System.ArgumentException::.ctor(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ArgumentException__ctor_m9A85EF7FEFEC21DDD525A67E831D77278E5165B7 (ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1 * __this, String_t* ___message0, const RuntimeMethod* method);
// System.String System.String::Concat(System.Object,System.Object,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Concat_m2E1F71C491D2429CC80A28745488FEA947BB7AAC (RuntimeObject * ___arg00, RuntimeObject * ___arg11, RuntimeObject * ___arg22, const RuntimeMethod* method);
// UnityEngine.AudioClip UnityEngine.Microphone::StartRecord(System.Int32,System.Boolean,System.Single,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * Microphone_StartRecord_m88F707BD42E1494928AA8D2E915062826E9021E8 (int32_t ___deviceID0, bool ___loop1, float ___lengthSec2, int32_t ___frequency3, const RuntimeMethod* method);
// System.Void UnityEngine.Microphone::EndRecord(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Microphone_EndRecord_m6DDB48A30B1A34A6E996A1A1FC194DA184B46733 (int32_t ___deviceID0, const RuntimeMethod* method);
// System.Boolean UnityEngine.Microphone::IsRecording(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Microphone_IsRecording_m70F5B087E69C0077AA9238931776EDE123FCF122 (int32_t ___deviceID0, const RuntimeMethod* method);
// System.Void UnityEngine.Microphone::GetDeviceCaps(System.Int32,System.Int32&,System.Int32&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Microphone_GetDeviceCaps_m4592CD4A6371544156A042579626B9351DCD7E93 (int32_t ___deviceID0, int32_t* ___minFreq1, int32_t* ___maxFreq2, const RuntimeMethod* method);
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
// UnityEngine.Playables.PlayableHandle UnityEngine.Audio.AudioClipPlayable::GetHandle()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  AudioClipPlayable_GetHandle_mEE8F62E9DA2A0DDDB064A9AA2391909C425CB3B1 (AudioClipPlayable_t6094311F945E65BC29F85B23A81E8426D596553C * __this, const RuntimeMethod* method)
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
IL2CPP_EXTERN_C  PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  AudioClipPlayable_GetHandle_mEE8F62E9DA2A0DDDB064A9AA2391909C425CB3B1_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	AudioClipPlayable_t6094311F945E65BC29F85B23A81E8426D596553C * _thisAdjusted = reinterpret_cast<AudioClipPlayable_t6094311F945E65BC29F85B23A81E8426D596553C *>(__this + 1);
	return AudioClipPlayable_GetHandle_mEE8F62E9DA2A0DDDB064A9AA2391909C425CB3B1(_thisAdjusted, method);
}
// System.Boolean UnityEngine.Audio.AudioClipPlayable::Equals(UnityEngine.Audio.AudioClipPlayable)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool AudioClipPlayable_Equals_mEB47B5F2E1C643D403FA916C8961F117593DCFC0 (AudioClipPlayable_t6094311F945E65BC29F85B23A81E8426D596553C * __this, AudioClipPlayable_t6094311F945E65BC29F85B23A81E8426D596553C  ___other0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AudioClipPlayable_Equals_mEB47B5F2E1C643D403FA916C8961F117593DCFC0_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	{
		PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  L_0 = AudioClipPlayable_GetHandle_mEE8F62E9DA2A0DDDB064A9AA2391909C425CB3B1((AudioClipPlayable_t6094311F945E65BC29F85B23A81E8426D596553C *)__this, /*hidden argument*/NULL);
		PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  L_1 = AudioClipPlayable_GetHandle_mEE8F62E9DA2A0DDDB064A9AA2391909C425CB3B1((AudioClipPlayable_t6094311F945E65BC29F85B23A81E8426D596553C *)(&___other0), /*hidden argument*/NULL);
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
IL2CPP_EXTERN_C  bool AudioClipPlayable_Equals_mEB47B5F2E1C643D403FA916C8961F117593DCFC0_AdjustorThunk (RuntimeObject * __this, AudioClipPlayable_t6094311F945E65BC29F85B23A81E8426D596553C  ___other0, const RuntimeMethod* method)
{
	AudioClipPlayable_t6094311F945E65BC29F85B23A81E8426D596553C * _thisAdjusted = reinterpret_cast<AudioClipPlayable_t6094311F945E65BC29F85B23A81E8426D596553C *>(__this + 1);
	return AudioClipPlayable_Equals_mEB47B5F2E1C643D403FA916C8961F117593DCFC0(_thisAdjusted, ___other0, method);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// UnityEngine.Playables.PlayableHandle UnityEngine.Audio.AudioMixerPlayable::GetHandle()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  AudioMixerPlayable_GetHandle_mDBC7135DF653E0E19675B6694EA89958E609587D (AudioMixerPlayable_t2C445EB39F9111CCFF7E2E1F813B22007862FA9F * __this, const RuntimeMethod* method)
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
IL2CPP_EXTERN_C  PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  AudioMixerPlayable_GetHandle_mDBC7135DF653E0E19675B6694EA89958E609587D_AdjustorThunk (RuntimeObject * __this, const RuntimeMethod* method)
{
	AudioMixerPlayable_t2C445EB39F9111CCFF7E2E1F813B22007862FA9F * _thisAdjusted = reinterpret_cast<AudioMixerPlayable_t2C445EB39F9111CCFF7E2E1F813B22007862FA9F *>(__this + 1);
	return AudioMixerPlayable_GetHandle_mDBC7135DF653E0E19675B6694EA89958E609587D(_thisAdjusted, method);
}
// System.Boolean UnityEngine.Audio.AudioMixerPlayable::Equals(UnityEngine.Audio.AudioMixerPlayable)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool AudioMixerPlayable_Equals_m6B84D1A5AEDEAAE12AEFB77319B2662506ABC9C4 (AudioMixerPlayable_t2C445EB39F9111CCFF7E2E1F813B22007862FA9F * __this, AudioMixerPlayable_t2C445EB39F9111CCFF7E2E1F813B22007862FA9F  ___other0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AudioMixerPlayable_Equals_m6B84D1A5AEDEAAE12AEFB77319B2662506ABC9C4_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	bool V_0 = false;
	{
		PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  L_0 = AudioMixerPlayable_GetHandle_mDBC7135DF653E0E19675B6694EA89958E609587D((AudioMixerPlayable_t2C445EB39F9111CCFF7E2E1F813B22007862FA9F *)__this, /*hidden argument*/NULL);
		PlayableHandle_t9D3B4E540D4413CED81DDD6A24C5373BEFA1D182  L_1 = AudioMixerPlayable_GetHandle_mDBC7135DF653E0E19675B6694EA89958E609587D((AudioMixerPlayable_t2C445EB39F9111CCFF7E2E1F813B22007862FA9F *)(&___other0), /*hidden argument*/NULL);
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
IL2CPP_EXTERN_C  bool AudioMixerPlayable_Equals_m6B84D1A5AEDEAAE12AEFB77319B2662506ABC9C4_AdjustorThunk (RuntimeObject * __this, AudioMixerPlayable_t2C445EB39F9111CCFF7E2E1F813B22007862FA9F  ___other0, const RuntimeMethod* method)
{
	AudioMixerPlayable_t2C445EB39F9111CCFF7E2E1F813B22007862FA9F * _thisAdjusted = reinterpret_cast<AudioMixerPlayable_t2C445EB39F9111CCFF7E2E1F813B22007862FA9F *>(__this + 1);
	return AudioMixerPlayable_Equals_m6B84D1A5AEDEAAE12AEFB77319B2662506ABC9C4(_thisAdjusted, ___other0, method);
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
// System.Void UnityEngine.AudioClip::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioClip__ctor_m52425138C3A036FC847A0E4C4ADA31CEF81CD10D (AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AudioClip__ctor_m52425138C3A036FC847A0E4C4ADA31CEF81CD10D_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		__this->set_m_PCMReaderCallback_4((PCMReaderCallback_t9B87AB13DCD37957B045554BF28A57697E6B8EFB *)NULL);
		__this->set_m_PCMSetPositionCallback_5((PCMSetPositionCallback_t092ED33043C0279B5E4D343EBCBD516CEF260801 *)NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		Object__ctor_m091EBAEBC7919B0391ABDAFB7389ADC12206525B(__this, /*hidden argument*/NULL);
		return;
	}
}
// System.Single UnityEngine.AudioClip::get_length()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float AudioClip_get_length_mFF1E21363B1860453451C4DA1C1459E9B9504317 (AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * __this, const RuntimeMethod* method)
{
	typedef float (*AudioClip_get_length_mFF1E21363B1860453451C4DA1C1459E9B9504317_ftn) (AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 *);
	static AudioClip_get_length_mFF1E21363B1860453451C4DA1C1459E9B9504317_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AudioClip_get_length_mFF1E21363B1860453451C4DA1C1459E9B9504317_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AudioClip::get_length()");
	float retVal = _il2cpp_icall_func(__this);
	return retVal;
}
// System.Void UnityEngine.AudioClip::InvokePCMReaderCallback_Internal(System.Single[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioClip_InvokePCMReaderCallback_Internal_mF087FCAD425EAC299C1156BA809DC535D00757F9 (AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * __this, SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* ___data0, const RuntimeMethod* method)
{
	{
		PCMReaderCallback_t9B87AB13DCD37957B045554BF28A57697E6B8EFB * L_0 = __this->get_m_PCMReaderCallback_4();
		if (!L_0)
		{
			goto IL_0018;
		}
	}
	{
		PCMReaderCallback_t9B87AB13DCD37957B045554BF28A57697E6B8EFB * L_1 = __this->get_m_PCMReaderCallback_4();
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_2 = ___data0;
		NullCheck(L_1);
		PCMReaderCallback_Invoke_m7B101820DB35BEFC8D2724DF96900367863B93B6(L_1, L_2, /*hidden argument*/NULL);
	}

IL_0018:
	{
		return;
	}
}
// System.Void UnityEngine.AudioClip::InvokePCMSetPositionCallback_Internal(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioClip_InvokePCMSetPositionCallback_Internal_mBB8265A5BFF660F8AF39718DDB193319AB7EFA6F (AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * __this, int32_t ___position0, const RuntimeMethod* method)
{
	{
		PCMSetPositionCallback_t092ED33043C0279B5E4D343EBCBD516CEF260801 * L_0 = __this->get_m_PCMSetPositionCallback_5();
		if (!L_0)
		{
			goto IL_0018;
		}
	}
	{
		PCMSetPositionCallback_t092ED33043C0279B5E4D343EBCBD516CEF260801 * L_1 = __this->get_m_PCMSetPositionCallback_5();
		int32_t L_2 = ___position0;
		NullCheck(L_1);
		PCMSetPositionCallback_Invoke_m8EA4736B43191A8E6F95E1548AFF124519EC533C(L_1, L_2, /*hidden argument*/NULL);
	}

IL_0018:
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
IL2CPP_EXTERN_C  void DelegatePInvokeWrapper_PCMReaderCallback_t9B87AB13DCD37957B045554BF28A57697E6B8EFB (PCMReaderCallback_t9B87AB13DCD37957B045554BF28A57697E6B8EFB * __this, SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* ___data0, const RuntimeMethod* method)
{
	typedef void (DEFAULT_CALL *PInvokeFunc)(float*);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Marshaling of parameter '___data0' to native representation
	float* ____data0_marshaled = NULL;
	if (___data0 != NULL)
	{
		____data0_marshaled = reinterpret_cast<float*>((___data0)->GetAddressAtUnchecked(0));
	}

	// Native function invocation
	il2cppPInvokeFunc(____data0_marshaled);

}
// System.Void UnityEngine.AudioClip_PCMReaderCallback::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PCMReaderCallback__ctor_mF9EB2467704F5E13196BBA93F41FA275AC5432F6 (PCMReaderCallback_t9B87AB13DCD37957B045554BF28A57697E6B8EFB * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void UnityEngine.AudioClip_PCMReaderCallback::Invoke(System.Single[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PCMReaderCallback_Invoke_m7B101820DB35BEFC8D2724DF96900367863B93B6 (PCMReaderCallback_t9B87AB13DCD37957B045554BF28A57697E6B8EFB * __this, SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* ___data0, const RuntimeMethod* method)
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
				typedef void (*FunctionPointerType) (SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___data0, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___data0, targetMethod);
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
						GenericInterfaceActionInvoker0::Invoke(targetMethod, ___data0);
					else
						GenericVirtActionInvoker0::Invoke(targetMethod, ___data0);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker0::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), ___data0);
					else
						VirtActionInvoker0::Invoke(il2cpp_codegen_method_get_slot(targetMethod), ___data0);
				}
			}
			else
			{
				typedef void (*FunctionPointerType) (SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___data0, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___data0, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker1< SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* >::Invoke(targetMethod, targetThis, ___data0);
					else
						GenericVirtActionInvoker1< SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* >::Invoke(targetMethod, targetThis, ___data0);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker1< SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___data0);
					else
						VirtActionInvoker1< SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___data0);
				}
			}
			else
			{
				typedef void (*FunctionPointerType) (void*, SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___data0, targetMethod);
			}
		}
	}
}
// System.IAsyncResult UnityEngine.AudioClip_PCMReaderCallback::BeginInvoke(System.Single[],System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* PCMReaderCallback_BeginInvoke_m94035E11B2B9BD6114EF3D7F4B7E367572E7AE1F (PCMReaderCallback_t9B87AB13DCD37957B045554BF28A57697E6B8EFB * __this, SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* ___data0, AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4 * ___callback1, RuntimeObject * ___object2, const RuntimeMethod* method)
{
	void *__d_args[2] = {0};
	__d_args[0] = ___data0;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback1, (RuntimeObject*)___object2);
}
// System.Void UnityEngine.AudioClip_PCMReaderCallback::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PCMReaderCallback_EndInvoke_m6730FD7DFD7246F137C437BC470F995D6C75E15B (PCMReaderCallback_t9B87AB13DCD37957B045554BF28A57697E6B8EFB * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
IL2CPP_EXTERN_C  void DelegatePInvokeWrapper_PCMSetPositionCallback_t092ED33043C0279B5E4D343EBCBD516CEF260801 (PCMSetPositionCallback_t092ED33043C0279B5E4D343EBCBD516CEF260801 * __this, int32_t ___position0, const RuntimeMethod* method)
{
	typedef void (DEFAULT_CALL *PInvokeFunc)(int32_t);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	il2cppPInvokeFunc(___position0);

}
// System.Void UnityEngine.AudioClip_PCMSetPositionCallback::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PCMSetPositionCallback__ctor_m31EA578C3CCFDFC9335B8C67353878AEE4B3905F (PCMSetPositionCallback_t092ED33043C0279B5E4D343EBCBD516CEF260801 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void UnityEngine.AudioClip_PCMSetPositionCallback::Invoke(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PCMSetPositionCallback_Invoke_m8EA4736B43191A8E6F95E1548AFF124519EC533C (PCMSetPositionCallback_t092ED33043C0279B5E4D343EBCBD516CEF260801 * __this, int32_t ___position0, const RuntimeMethod* method)
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
				typedef void (*FunctionPointerType) (int32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___position0, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, int32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___position0, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (int32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___position0, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker1< int32_t >::Invoke(targetMethod, targetThis, ___position0);
					else
						GenericVirtActionInvoker1< int32_t >::Invoke(targetMethod, targetThis, ___position0);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker1< int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___position0);
					else
						VirtActionInvoker1< int32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___position0);
				}
			}
			else
			{
				typedef void (*FunctionPointerType) (void*, int32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___position0, targetMethod);
			}
		}
	}
}
// System.IAsyncResult UnityEngine.AudioClip_PCMSetPositionCallback::BeginInvoke(System.Int32,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* PCMSetPositionCallback_BeginInvoke_m88CDF70D75854621CA69ED3D53CD53B8206A5093 (PCMSetPositionCallback_t092ED33043C0279B5E4D343EBCBD516CEF260801 * __this, int32_t ___position0, AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4 * ___callback1, RuntimeObject * ___object2, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (PCMSetPositionCallback_BeginInvoke_m88CDF70D75854621CA69ED3D53CD53B8206A5093_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[2] = {0};
	__d_args[0] = Box(Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var, &___position0);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback1, (RuntimeObject*)___object2);
}
// System.Void UnityEngine.AudioClip_PCMSetPositionCallback::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PCMSetPositionCallback_EndInvoke_mB711E23CFD370348A1680B281A3DFE04F970792C (PCMSetPositionCallback_t092ED33043C0279B5E4D343EBCBD516CEF260801 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
// System.Single UnityEngine.AudioHighPassFilter::get_cutoffFrequency()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float AudioHighPassFilter_get_cutoffFrequency_m0733A348725BAAD6E72E4EB47C9900E4496FB917 (AudioHighPassFilter_tAF7C56386170F84139A84528E2EC43DBC9E2C473 * __this, const RuntimeMethod* method)
{
	typedef float (*AudioHighPassFilter_get_cutoffFrequency_m0733A348725BAAD6E72E4EB47C9900E4496FB917_ftn) (AudioHighPassFilter_tAF7C56386170F84139A84528E2EC43DBC9E2C473 *);
	static AudioHighPassFilter_get_cutoffFrequency_m0733A348725BAAD6E72E4EB47C9900E4496FB917_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AudioHighPassFilter_get_cutoffFrequency_m0733A348725BAAD6E72E4EB47C9900E4496FB917_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AudioHighPassFilter::get_cutoffFrequency()");
	float retVal = _il2cpp_icall_func(__this);
	return retVal;
}
// System.Void UnityEngine.AudioHighPassFilter::set_cutoffFrequency(System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioHighPassFilter_set_cutoffFrequency_m6D4DFA0033853F8C1E7B9B49153E12BFD1E4D35A (AudioHighPassFilter_tAF7C56386170F84139A84528E2EC43DBC9E2C473 * __this, float ___value0, const RuntimeMethod* method)
{
	typedef void (*AudioHighPassFilter_set_cutoffFrequency_m6D4DFA0033853F8C1E7B9B49153E12BFD1E4D35A_ftn) (AudioHighPassFilter_tAF7C56386170F84139A84528E2EC43DBC9E2C473 *, float);
	static AudioHighPassFilter_set_cutoffFrequency_m6D4DFA0033853F8C1E7B9B49153E12BFD1E4D35A_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AudioHighPassFilter_set_cutoffFrequency_m6D4DFA0033853F8C1E7B9B49153E12BFD1E4D35A_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AudioHighPassFilter::set_cutoffFrequency(System.Single)");
	_il2cpp_icall_func(__this, ___value0);
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
// System.Single UnityEngine.AudioLowPassFilter::get_cutoffFrequency()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float AudioLowPassFilter_get_cutoffFrequency_mC6A24AC159FC3BF1B52F19B24E04AF0712682433 (AudioLowPassFilter_t97DD6F50E1F0D2D9404D8A28A97CA3D232BF44CE * __this, const RuntimeMethod* method)
{
	typedef float (*AudioLowPassFilter_get_cutoffFrequency_mC6A24AC159FC3BF1B52F19B24E04AF0712682433_ftn) (AudioLowPassFilter_t97DD6F50E1F0D2D9404D8A28A97CA3D232BF44CE *);
	static AudioLowPassFilter_get_cutoffFrequency_mC6A24AC159FC3BF1B52F19B24E04AF0712682433_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AudioLowPassFilter_get_cutoffFrequency_mC6A24AC159FC3BF1B52F19B24E04AF0712682433_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AudioLowPassFilter::get_cutoffFrequency()");
	float retVal = _il2cpp_icall_func(__this);
	return retVal;
}
// System.Void UnityEngine.AudioLowPassFilter::set_cutoffFrequency(System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioLowPassFilter_set_cutoffFrequency_m8F9D885ADC292AF38492FE53A44CDB31D6BD0AF6 (AudioLowPassFilter_t97DD6F50E1F0D2D9404D8A28A97CA3D232BF44CE * __this, float ___value0, const RuntimeMethod* method)
{
	typedef void (*AudioLowPassFilter_set_cutoffFrequency_m8F9D885ADC292AF38492FE53A44CDB31D6BD0AF6_ftn) (AudioLowPassFilter_t97DD6F50E1F0D2D9404D8A28A97CA3D232BF44CE *, float);
	static AudioLowPassFilter_set_cutoffFrequency_m8F9D885ADC292AF38492FE53A44CDB31D6BD0AF6_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AudioLowPassFilter_set_cutoffFrequency_m8F9D885ADC292AF38492FE53A44CDB31D6BD0AF6_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AudioLowPassFilter::set_cutoffFrequency(System.Single)");
	_il2cpp_icall_func(__this, ___value0);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void UnityEngine.AudioSettings::InvokeOnAudioConfigurationChanged(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSettings_InvokeOnAudioConfigurationChanged_m8D251791C6A402B12E93C22F43475DE3033FC8E7 (bool ___deviceWasChanged0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AudioSettings_InvokeOnAudioConfigurationChanged_m8D251791C6A402B12E93C22F43475DE3033FC8E7_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		AudioConfigurationChangeHandler_t8E0E05D0198D95B5412DC716F87D97020EF54926 * L_0 = ((AudioSettings_t77B5D69F704CF3B710B0B6970BB62A4BF25A5B31_StaticFields*)il2cpp_codegen_static_fields_for(AudioSettings_t77B5D69F704CF3B710B0B6970BB62A4BF25A5B31_il2cpp_TypeInfo_var))->get_OnAudioConfigurationChanged_0();
		if (!L_0)
		{
			goto IL_0016;
		}
	}
	{
		AudioConfigurationChangeHandler_t8E0E05D0198D95B5412DC716F87D97020EF54926 * L_1 = ((AudioSettings_t77B5D69F704CF3B710B0B6970BB62A4BF25A5B31_StaticFields*)il2cpp_codegen_static_fields_for(AudioSettings_t77B5D69F704CF3B710B0B6970BB62A4BF25A5B31_il2cpp_TypeInfo_var))->get_OnAudioConfigurationChanged_0();
		bool L_2 = ___deviceWasChanged0;
		NullCheck(L_1);
		AudioConfigurationChangeHandler_Invoke_m62D72B397E1DC117C8C92A450D2C86C535A2BF49(L_1, L_2, /*hidden argument*/NULL);
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
IL2CPP_EXTERN_C  void DelegatePInvokeWrapper_AudioConfigurationChangeHandler_t8E0E05D0198D95B5412DC716F87D97020EF54926 (AudioConfigurationChangeHandler_t8E0E05D0198D95B5412DC716F87D97020EF54926 * __this, bool ___deviceWasChanged0, const RuntimeMethod* method)
{
	typedef void (DEFAULT_CALL *PInvokeFunc)(int32_t);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	il2cppPInvokeFunc(static_cast<int32_t>(___deviceWasChanged0));

}
// System.Void UnityEngine.AudioSettings_AudioConfigurationChangeHandler::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioConfigurationChangeHandler__ctor_mF9399769D5BB18D740774B9E3129958868BD6D9A (AudioConfigurationChangeHandler_t8E0E05D0198D95B5412DC716F87D97020EF54926 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void UnityEngine.AudioSettings_AudioConfigurationChangeHandler::Invoke(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioConfigurationChangeHandler_Invoke_m62D72B397E1DC117C8C92A450D2C86C535A2BF49 (AudioConfigurationChangeHandler_t8E0E05D0198D95B5412DC716F87D97020EF54926 * __this, bool ___deviceWasChanged0, const RuntimeMethod* method)
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
				typedef void (*FunctionPointerType) (bool, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___deviceWasChanged0, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, bool, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___deviceWasChanged0, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (bool, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___deviceWasChanged0, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker1< bool >::Invoke(targetMethod, targetThis, ___deviceWasChanged0);
					else
						GenericVirtActionInvoker1< bool >::Invoke(targetMethod, targetThis, ___deviceWasChanged0);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker1< bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___deviceWasChanged0);
					else
						VirtActionInvoker1< bool >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___deviceWasChanged0);
				}
			}
			else
			{
				typedef void (*FunctionPointerType) (void*, bool, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___deviceWasChanged0, targetMethod);
			}
		}
	}
}
// System.IAsyncResult UnityEngine.AudioSettings_AudioConfigurationChangeHandler::BeginInvoke(System.Boolean,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* AudioConfigurationChangeHandler_BeginInvoke_mB0B0ACF6281B999FA11037CA130CA3C72BEC7827 (AudioConfigurationChangeHandler_t8E0E05D0198D95B5412DC716F87D97020EF54926 * __this, bool ___deviceWasChanged0, AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4 * ___callback1, RuntimeObject * ___object2, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AudioConfigurationChangeHandler_BeginInvoke_mB0B0ACF6281B999FA11037CA130CA3C72BEC7827_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[2] = {0};
	__d_args[0] = Box(Boolean_tB53F6830F670160873277339AA58F15CAED4399C_il2cpp_TypeInfo_var, &___deviceWasChanged0);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback1, (RuntimeObject*)___object2);
}
// System.Void UnityEngine.AudioSettings_AudioConfigurationChangeHandler::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioConfigurationChangeHandler_EndInvoke_mBB53599C34E3944D3A1DD71EFD2D73AF105CF830 (AudioConfigurationChangeHandler_t8E0E05D0198D95B5412DC716F87D97020EF54926 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
// System.Void UnityEngine.AudioSource::SetPitch(UnityEngine.AudioSource,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSource_SetPitch_mB65EF59C56A49FD68CD3361B2C34E07F89F1244B (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C * ___source0, float ___pitch1, const RuntimeMethod* method)
{
	typedef void (*AudioSource_SetPitch_mB65EF59C56A49FD68CD3361B2C34E07F89F1244B_ftn) (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C *, float);
	static AudioSource_SetPitch_mB65EF59C56A49FD68CD3361B2C34E07F89F1244B_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AudioSource_SetPitch_mB65EF59C56A49FD68CD3361B2C34E07F89F1244B_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AudioSource::SetPitch(UnityEngine.AudioSource,System.Single)");
	_il2cpp_icall_func(___source0, ___pitch1);
}
// System.Void UnityEngine.AudioSource::PlayHelper(UnityEngine.AudioSource,System.UInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSource_PlayHelper_m361C17B583E05D2A5FA0F03BD7CD98D74FBF83AC (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C * ___source0, uint64_t ___delay1, const RuntimeMethod* method)
{
	typedef void (*AudioSource_PlayHelper_m361C17B583E05D2A5FA0F03BD7CD98D74FBF83AC_ftn) (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C *, uint64_t);
	static AudioSource_PlayHelper_m361C17B583E05D2A5FA0F03BD7CD98D74FBF83AC_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AudioSource_PlayHelper_m361C17B583E05D2A5FA0F03BD7CD98D74FBF83AC_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AudioSource::PlayHelper(UnityEngine.AudioSource,System.UInt64)");
	_il2cpp_icall_func(___source0, ___delay1);
}
// System.Void UnityEngine.AudioSource::PlayOneShotHelper(UnityEngine.AudioSource,UnityEngine.AudioClip,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSource_PlayOneShotHelper_mDAEDF5E0C56C665DE62CDF26E7B98149B4A71704 (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C * ___source0, AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * ___clip1, float ___volumeScale2, const RuntimeMethod* method)
{
	typedef void (*AudioSource_PlayOneShotHelper_mDAEDF5E0C56C665DE62CDF26E7B98149B4A71704_ftn) (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C *, AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 *, float);
	static AudioSource_PlayOneShotHelper_mDAEDF5E0C56C665DE62CDF26E7B98149B4A71704_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AudioSource_PlayOneShotHelper_mDAEDF5E0C56C665DE62CDF26E7B98149B4A71704_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AudioSource::PlayOneShotHelper(UnityEngine.AudioSource,UnityEngine.AudioClip,System.Single)");
	_il2cpp_icall_func(___source0, ___clip1, ___volumeScale2);
}
// System.Single UnityEngine.AudioSource::get_volume()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float AudioSource_get_volume_mBD65DB423F0520CDCB935CC593565343965A4CB0 (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C * __this, const RuntimeMethod* method)
{
	typedef float (*AudioSource_get_volume_mBD65DB423F0520CDCB935CC593565343965A4CB0_ftn) (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C *);
	static AudioSource_get_volume_mBD65DB423F0520CDCB935CC593565343965A4CB0_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AudioSource_get_volume_mBD65DB423F0520CDCB935CC593565343965A4CB0_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AudioSource::get_volume()");
	float retVal = _il2cpp_icall_func(__this);
	return retVal;
}
// System.Void UnityEngine.AudioSource::set_volume(System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSource_set_volume_mF1757D70EE113871724334D13F70EF1ED033BA06 (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C * __this, float ___value0, const RuntimeMethod* method)
{
	typedef void (*AudioSource_set_volume_mF1757D70EE113871724334D13F70EF1ED033BA06_ftn) (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C *, float);
	static AudioSource_set_volume_mF1757D70EE113871724334D13F70EF1ED033BA06_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AudioSource_set_volume_mF1757D70EE113871724334D13F70EF1ED033BA06_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AudioSource::set_volume(System.Single)");
	_il2cpp_icall_func(__this, ___value0);
}
// System.Void UnityEngine.AudioSource::set_pitch(System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSource_set_pitch_mAB8F8CDB21A3139D3471784FEE9DBFA4CEDEE2E0 (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C * __this, float ___value0, const RuntimeMethod* method)
{
	{
		float L_0 = ___value0;
		AudioSource_SetPitch_mB65EF59C56A49FD68CD3361B2C34E07F89F1244B(__this, L_0, /*hidden argument*/NULL);
		return;
	}
}
// UnityEngine.AudioClip UnityEngine.AudioSource::get_clip()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * AudioSource_get_clip_m773ECEF5566EA64C74E316D7EF1A63AA01604643 (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C * __this, const RuntimeMethod* method)
{
	typedef AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * (*AudioSource_get_clip_m773ECEF5566EA64C74E316D7EF1A63AA01604643_ftn) (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C *);
	static AudioSource_get_clip_m773ECEF5566EA64C74E316D7EF1A63AA01604643_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AudioSource_get_clip_m773ECEF5566EA64C74E316D7EF1A63AA01604643_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AudioSource::get_clip()");
	AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * retVal = _il2cpp_icall_func(__this);
	return retVal;
}
// System.Void UnityEngine.AudioSource::set_clip(UnityEngine.AudioClip)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSource_set_clip_mF574231E0B749E0167CAF9E4FCBA06BAA0F9ED9B (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C * __this, AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * ___value0, const RuntimeMethod* method)
{
	typedef void (*AudioSource_set_clip_mF574231E0B749E0167CAF9E4FCBA06BAA0F9ED9B_ftn) (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C *, AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 *);
	static AudioSource_set_clip_mF574231E0B749E0167CAF9E4FCBA06BAA0F9ED9B_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AudioSource_set_clip_mF574231E0B749E0167CAF9E4FCBA06BAA0F9ED9B_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AudioSource::set_clip(UnityEngine.AudioClip)");
	_il2cpp_icall_func(__this, ___value0);
}
// System.Void UnityEngine.AudioSource::Play()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSource_Play_m0BA206481892AA4AF7DB2900A0B0805076516164 (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C * __this, const RuntimeMethod* method)
{
	{
		AudioSource_PlayHelper_m361C17B583E05D2A5FA0F03BD7CD98D74FBF83AC(__this, (((int64_t)((int64_t)0))), /*hidden argument*/NULL);
		return;
	}
}
// System.Void UnityEngine.AudioSource::PlayOneShot(UnityEngine.AudioClip)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSource_PlayOneShot_mFD68566752A61B9C54843650A5C6075DBBFC56CD (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C * __this, AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * ___clip0, const RuntimeMethod* method)
{
	{
		AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * L_0 = ___clip0;
		AudioSource_PlayOneShot_mA65B809A4480039CD8337ABF45C0E57E137EED41(__this, L_0, (1.0f), /*hidden argument*/NULL);
		return;
	}
}
// System.Void UnityEngine.AudioSource::PlayOneShot(UnityEngine.AudioClip,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSource_PlayOneShot_mA65B809A4480039CD8337ABF45C0E57E137EED41 (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C * __this, AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * ___clip0, float ___volumeScale1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AudioSource_PlayOneShot_mA65B809A4480039CD8337ABF45C0E57E137EED41_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * L_0 = ___clip0;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_1 = Object_op_Equality_mBC2401774F3BE33E8CF6F0A8148E66C95D6CFF1C(L_0, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_001d;
		}
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_LogWarning_m37338644DC81F640CCDFEAE35A223F0E965F0568(_stringLiteral2F43397615CF615962F130AA6604F6D76A60AE5C, /*hidden argument*/NULL);
		goto IL_0025;
	}

IL_001d:
	{
		AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * L_2 = ___clip0;
		float L_3 = ___volumeScale1;
		AudioSource_PlayOneShotHelper_mDAEDF5E0C56C665DE62CDF26E7B98149B4A71704(__this, L_2, L_3, /*hidden argument*/NULL);
	}

IL_0025:
	{
		return;
	}
}
// System.Void UnityEngine.AudioSource::set_playOnAwake(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSource_set_playOnAwake_m5E4C76260D66898EEFEB20E4F42B6249AACB4128 (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C * __this, bool ___value0, const RuntimeMethod* method)
{
	typedef void (*AudioSource_set_playOnAwake_m5E4C76260D66898EEFEB20E4F42B6249AACB4128_ftn) (AudioSource_t5196F862B4E60F404613361C90D87FBDD041E93C *, bool);
	static AudioSource_set_playOnAwake_m5E4C76260D66898EEFEB20E4F42B6249AACB4128_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AudioSource_set_playOnAwake_m5E4C76260D66898EEFEB20E4F42B6249AACB4128_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AudioSource::set_playOnAwake(System.Boolean)");
	_il2cpp_icall_func(__this, ___value0);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void UnityEngine.Experimental.Audio.AudioSampleProvider::Finalize()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSampleProvider_Finalize_mACCDEE1F5F0F602DC35AE68875EB8DB16830544C (AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 * __this, const RuntimeMethod* method)
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
		AudioSampleProvider_set_owner_m21FC4073346BFBE33E2B1567ECEC8ECD373B3069_inline(__this, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		AudioSampleProvider_Dispose_mA11D514354EDA94E8B6AA83B726DF85169C8E29A(__this, /*hidden argument*/NULL);
		IL2CPP_LEAVE(0x1A, FINALLY_0013);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0013;
	}

FINALLY_0013:
	{ // begin finally (depth: 1)
		Object_Finalize_m4015B7D3A44DE125C5FE34D7276CD4697C06F380(__this, /*hidden argument*/NULL);
		IL2CPP_END_FINALLY(19)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(19)
	{
		IL2CPP_JUMP_TBL(0x1A, IL_001a)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_001a:
	{
		return;
	}
}
// System.Void UnityEngine.Experimental.Audio.AudioSampleProvider::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSampleProvider_Dispose_mA11D514354EDA94E8B6AA83B726DF85169C8E29A (AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AudioSampleProvider_Dispose_mA11D514354EDA94E8B6AA83B726DF85169C8E29A_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		uint32_t L_0 = AudioSampleProvider_get_id_m9E16910C6A2335F87E6B257D3059A8BBC8D7253A(__this, /*hidden argument*/NULL);
		if (!L_0)
		{
			goto IL_003d;
		}
	}
	{
		uint32_t L_1 = AudioSampleProvider_get_id_m9E16910C6A2335F87E6B257D3059A8BBC8D7253A(__this, /*hidden argument*/NULL);
		AudioSampleProvider_InternalSetScriptingPtr_m018841DA2C6D92E2322F1D041E4C88A054991AF9(L_1, (AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 *)NULL, /*hidden argument*/NULL);
		Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * L_2 = AudioSampleProvider_get_owner_m822057CE02CF8664D7BF569CDB41D8EB154251FE(__this, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_3 = Object_op_Equality_mBC2401774F3BE33E8CF6F0A8148E66C95D6CFF1C(L_2, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_3)
		{
			goto IL_0035;
		}
	}
	{
		uint32_t L_4 = AudioSampleProvider_get_id_m9E16910C6A2335F87E6B257D3059A8BBC8D7253A(__this, /*hidden argument*/NULL);
		AudioSampleProvider_InternalRemove_m3CD8E2D57A6DC5522C71436A854926C346CEBF11(L_4, /*hidden argument*/NULL);
	}

IL_0035:
	{
		AudioSampleProvider_set_id_mB98E8435407CAD305BBEAA91B18CBC12562DFAF9_inline(__this, 0, /*hidden argument*/NULL);
	}

IL_003d:
	{
		IL2CPP_RUNTIME_CLASS_INIT(GC_tC1D7BD74E8F44ECCEF5CD2B5D84BFF9AAE02D01D_il2cpp_TypeInfo_var);
		GC_SuppressFinalize_m037319A9B95A5BA437E806DE592802225EE5B425(__this, /*hidden argument*/NULL);
		return;
	}
}
// System.UInt32 UnityEngine.Experimental.Audio.AudioSampleProvider::get_id()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t AudioSampleProvider_get_id_m9E16910C6A2335F87E6B257D3059A8BBC8D7253A (AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 * __this, const RuntimeMethod* method)
{
	uint32_t V_0 = 0;
	{
		uint32_t L_0 = __this->get_U3CidU3Ek__BackingField_1();
		V_0 = L_0;
		goto IL_000c;
	}

IL_000c:
	{
		uint32_t L_1 = V_0;
		return L_1;
	}
}
// System.Void UnityEngine.Experimental.Audio.AudioSampleProvider::set_id(System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSampleProvider_set_id_mB98E8435407CAD305BBEAA91B18CBC12562DFAF9 (AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 * __this, uint32_t ___value0, const RuntimeMethod* method)
{
	{
		uint32_t L_0 = ___value0;
		__this->set_U3CidU3Ek__BackingField_1(L_0);
		return;
	}
}
// UnityEngine.Object UnityEngine.Experimental.Audio.AudioSampleProvider::get_owner()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * AudioSampleProvider_get_owner_m822057CE02CF8664D7BF569CDB41D8EB154251FE (AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 * __this, const RuntimeMethod* method)
{
	Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * V_0 = NULL;
	{
		Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * L_0 = __this->get_U3CownerU3Ek__BackingField_3();
		V_0 = L_0;
		goto IL_000c;
	}

IL_000c:
	{
		Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * L_1 = V_0;
		return L_1;
	}
}
// System.Void UnityEngine.Experimental.Audio.AudioSampleProvider::set_owner(UnityEngine.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSampleProvider_set_owner_m21FC4073346BFBE33E2B1567ECEC8ECD373B3069 (AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 * __this, Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * ___value0, const RuntimeMethod* method)
{
	{
		Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * L_0 = ___value0;
		__this->set_U3CownerU3Ek__BackingField_3(L_0);
		return;
	}
}
// System.Void UnityEngine.Experimental.Audio.AudioSampleProvider::InvokeSampleFramesAvailable(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSampleProvider_InvokeSampleFramesAvailable_m7604AAF1AC01473A29DCDAD1AEC06165504BE832 (AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 * __this, int32_t ___sampleFrameCount0, const RuntimeMethod* method)
{
	{
		SampleFramesHandler_t5179C92AFBB393A85144E9134A862C161726F6AF * L_0 = __this->get_sampleFramesAvailable_6();
		if (!L_0)
		{
			goto IL_0019;
		}
	}
	{
		SampleFramesHandler_t5179C92AFBB393A85144E9134A862C161726F6AF * L_1 = __this->get_sampleFramesAvailable_6();
		int32_t L_2 = ___sampleFrameCount0;
		NullCheck(L_1);
		SampleFramesHandler_Invoke_m52F0148F680B36E04A7F850E617FBEF1CA9809FD(L_1, __this, L_2, /*hidden argument*/NULL);
	}

IL_0019:
	{
		return;
	}
}
// System.Void UnityEngine.Experimental.Audio.AudioSampleProvider::InvokeSampleFramesOverflow(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSampleProvider_InvokeSampleFramesOverflow_mC81A014388E535569EF02E3DA6B9831B0FB8A8D4 (AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 * __this, int32_t ___droppedSampleFrameCount0, const RuntimeMethod* method)
{
	{
		SampleFramesHandler_t5179C92AFBB393A85144E9134A862C161726F6AF * L_0 = __this->get_sampleFramesOverflow_7();
		if (!L_0)
		{
			goto IL_0019;
		}
	}
	{
		SampleFramesHandler_t5179C92AFBB393A85144E9134A862C161726F6AF * L_1 = __this->get_sampleFramesOverflow_7();
		int32_t L_2 = ___droppedSampleFrameCount0;
		NullCheck(L_1);
		SampleFramesHandler_Invoke_m52F0148F680B36E04A7F850E617FBEF1CA9809FD(L_1, __this, L_2, /*hidden argument*/NULL);
	}

IL_0019:
	{
		return;
	}
}
// System.Void UnityEngine.Experimental.Audio.AudioSampleProvider::InternalRemove(System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSampleProvider_InternalRemove_m3CD8E2D57A6DC5522C71436A854926C346CEBF11 (uint32_t ___providerId0, const RuntimeMethod* method)
{
	typedef void (*AudioSampleProvider_InternalRemove_m3CD8E2D57A6DC5522C71436A854926C346CEBF11_ftn) (uint32_t);
	static AudioSampleProvider_InternalRemove_m3CD8E2D57A6DC5522C71436A854926C346CEBF11_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AudioSampleProvider_InternalRemove_m3CD8E2D57A6DC5522C71436A854926C346CEBF11_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.Experimental.Audio.AudioSampleProvider::InternalRemove(System.UInt32)");
	_il2cpp_icall_func(___providerId0);
}
// System.Void UnityEngine.Experimental.Audio.AudioSampleProvider::InternalSetScriptingPtr(System.UInt32,UnityEngine.Experimental.Audio.AudioSampleProvider)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AudioSampleProvider_InternalSetScriptingPtr_m018841DA2C6D92E2322F1D041E4C88A054991AF9 (uint32_t ___providerId0, AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 * ___provider1, const RuntimeMethod* method)
{
	typedef void (*AudioSampleProvider_InternalSetScriptingPtr_m018841DA2C6D92E2322F1D041E4C88A054991AF9_ftn) (uint32_t, AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 *);
	static AudioSampleProvider_InternalSetScriptingPtr_m018841DA2C6D92E2322F1D041E4C88A054991AF9_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AudioSampleProvider_InternalSetScriptingPtr_m018841DA2C6D92E2322F1D041E4C88A054991AF9_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.Experimental.Audio.AudioSampleProvider::InternalSetScriptingPtr(System.UInt32,UnityEngine.Experimental.Audio.AudioSampleProvider)");
	_il2cpp_icall_func(___providerId0, ___provider1);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
IL2CPP_EXTERN_C  uint32_t DelegatePInvokeWrapper_ConsumeSampleFramesNativeFunction_tC1E0B1BFCF2C3D7F87D66FCFA2022369327D931D (ConsumeSampleFramesNativeFunction_tC1E0B1BFCF2C3D7F87D66FCFA2022369327D931D * __this, uint32_t ___providerId0, intptr_t ___interleavedSampleFrames1, uint32_t ___sampleFrameCount2, const RuntimeMethod* method)
{
	typedef uint32_t (CDECL *PInvokeFunc)(uint32_t, intptr_t, uint32_t);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	uint32_t returnValue = il2cppPInvokeFunc(___providerId0, ___interleavedSampleFrames1, ___sampleFrameCount2);

	return returnValue;
}
// System.Void UnityEngine.Experimental.Audio.AudioSampleProvider_ConsumeSampleFramesNativeFunction::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ConsumeSampleFramesNativeFunction__ctor_m10D2C92ADC2CC0F2731CC790036CF84E7E6E97F6 (ConsumeSampleFramesNativeFunction_tC1E0B1BFCF2C3D7F87D66FCFA2022369327D931D * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.UInt32 UnityEngine.Experimental.Audio.AudioSampleProvider_ConsumeSampleFramesNativeFunction::Invoke(System.UInt32,System.IntPtr,System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t ConsumeSampleFramesNativeFunction_Invoke_mA644FB7343047BEC754D81AC2AFABDC300DF5D74 (ConsumeSampleFramesNativeFunction_tC1E0B1BFCF2C3D7F87D66FCFA2022369327D931D * __this, uint32_t ___providerId0, intptr_t ___interleavedSampleFrames1, uint32_t ___sampleFrameCount2, const RuntimeMethod* method)
{
	uint32_t result = 0;
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
			if (___parameterCount == 3)
			{
				// open
				typedef uint32_t (*FunctionPointerType) (uint32_t, intptr_t, uint32_t, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(___providerId0, ___interleavedSampleFrames1, ___sampleFrameCount2, targetMethod);
			}
			else
			{
				// closed
				typedef uint32_t (*FunctionPointerType) (void*, uint32_t, intptr_t, uint32_t, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___providerId0, ___interleavedSampleFrames1, ___sampleFrameCount2, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef uint32_t (*FunctionPointerType) (uint32_t, intptr_t, uint32_t, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)(___providerId0, ___interleavedSampleFrames1, ___sampleFrameCount2, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = GenericInterfaceFuncInvoker3< uint32_t, uint32_t, intptr_t, uint32_t >::Invoke(targetMethod, targetThis, ___providerId0, ___interleavedSampleFrames1, ___sampleFrameCount2);
					else
						result = GenericVirtFuncInvoker3< uint32_t, uint32_t, intptr_t, uint32_t >::Invoke(targetMethod, targetThis, ___providerId0, ___interleavedSampleFrames1, ___sampleFrameCount2);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = InterfaceFuncInvoker3< uint32_t, uint32_t, intptr_t, uint32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___providerId0, ___interleavedSampleFrames1, ___sampleFrameCount2);
					else
						result = VirtFuncInvoker3< uint32_t, uint32_t, intptr_t, uint32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___providerId0, ___interleavedSampleFrames1, ___sampleFrameCount2);
				}
			}
			else
			{
				typedef uint32_t (*FunctionPointerType) (void*, uint32_t, intptr_t, uint32_t, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___providerId0, ___interleavedSampleFrames1, ___sampleFrameCount2, targetMethod);
			}
		}
	}
	return result;
}
// System.IAsyncResult UnityEngine.Experimental.Audio.AudioSampleProvider_ConsumeSampleFramesNativeFunction::BeginInvoke(System.UInt32,System.IntPtr,System.UInt32,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* ConsumeSampleFramesNativeFunction_BeginInvoke_mCFD45D186107B1FD87B1A492904CBCF49DDA28CB (ConsumeSampleFramesNativeFunction_tC1E0B1BFCF2C3D7F87D66FCFA2022369327D931D * __this, uint32_t ___providerId0, intptr_t ___interleavedSampleFrames1, uint32_t ___sampleFrameCount2, AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4 * ___callback3, RuntimeObject * ___object4, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ConsumeSampleFramesNativeFunction_BeginInvoke_mCFD45D186107B1FD87B1A492904CBCF49DDA28CB_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[4] = {0};
	__d_args[0] = Box(UInt32_t4980FA09003AFAAB5A6E361BA2748EA9A005709B_il2cpp_TypeInfo_var, &___providerId0);
	__d_args[1] = Box(IntPtr_t_il2cpp_TypeInfo_var, &___interleavedSampleFrames1);
	__d_args[2] = Box(UInt32_t4980FA09003AFAAB5A6E361BA2748EA9A005709B_il2cpp_TypeInfo_var, &___sampleFrameCount2);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback3, (RuntimeObject*)___object4);
}
// System.UInt32 UnityEngine.Experimental.Audio.AudioSampleProvider_ConsumeSampleFramesNativeFunction::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t ConsumeSampleFramesNativeFunction_EndInvoke_m4F91AA6FC2DFEF1495E3BA8D7BF1098C9E043711 (ConsumeSampleFramesNativeFunction_tC1E0B1BFCF2C3D7F87D66FCFA2022369327D931D * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
{
	RuntimeObject *__result = il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___result0, 0);
	return *(uint32_t*)UnBox ((RuntimeObject*)__result);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void UnityEngine.Experimental.Audio.AudioSampleProvider_SampleFramesHandler::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SampleFramesHandler__ctor_mFDA0769E55F136D1B8EC8AA4B40EF43069934EB5 (SampleFramesHandler_t5179C92AFBB393A85144E9134A862C161726F6AF * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void UnityEngine.Experimental.Audio.AudioSampleProvider_SampleFramesHandler::Invoke(UnityEngine.Experimental.Audio.AudioSampleProvider,System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SampleFramesHandler_Invoke_m52F0148F680B36E04A7F850E617FBEF1CA9809FD (SampleFramesHandler_t5179C92AFBB393A85144E9134A862C161726F6AF * __this, AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 * ___provider0, uint32_t ___sampleFrameCount1, const RuntimeMethod* method)
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
				typedef void (*FunctionPointerType) (AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 *, uint32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___provider0, ___sampleFrameCount1, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 *, uint32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___provider0, ___sampleFrameCount1, targetMethod);
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
						GenericInterfaceActionInvoker1< uint32_t >::Invoke(targetMethod, ___provider0, ___sampleFrameCount1);
					else
						GenericVirtActionInvoker1< uint32_t >::Invoke(targetMethod, ___provider0, ___sampleFrameCount1);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker1< uint32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), ___provider0, ___sampleFrameCount1);
					else
						VirtActionInvoker1< uint32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), ___provider0, ___sampleFrameCount1);
				}
			}
			else
			{
				typedef void (*FunctionPointerType) (AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 *, uint32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___provider0, ___sampleFrameCount1, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 *, uint32_t, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___provider0, ___sampleFrameCount1, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker2< AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 *, uint32_t >::Invoke(targetMethod, targetThis, ___provider0, ___sampleFrameCount1);
					else
						GenericVirtActionInvoker2< AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 *, uint32_t >::Invoke(targetMethod, targetThis, ___provider0, ___sampleFrameCount1);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker2< AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 *, uint32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___provider0, ___sampleFrameCount1);
					else
						VirtActionInvoker2< AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 *, uint32_t >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___provider0, ___sampleFrameCount1);
				}
			}
			else
			{
				typedef void (*FunctionPointerType) (void*, AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 *, uint32_t, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___provider0, ___sampleFrameCount1, targetMethod);
			}
		}
	}
}
// System.IAsyncResult UnityEngine.Experimental.Audio.AudioSampleProvider_SampleFramesHandler::BeginInvoke(UnityEngine.Experimental.Audio.AudioSampleProvider,System.UInt32,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* SampleFramesHandler_BeginInvoke_mE516B77CCC50738663D10DDD2D7BDB4391FDFF92 (SampleFramesHandler_t5179C92AFBB393A85144E9134A862C161726F6AF * __this, AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 * ___provider0, uint32_t ___sampleFrameCount1, AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4 * ___callback2, RuntimeObject * ___object3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SampleFramesHandler_BeginInvoke_mE516B77CCC50738663D10DDD2D7BDB4391FDFF92_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[3] = {0};
	__d_args[0] = ___provider0;
	__d_args[1] = Box(UInt32_t4980FA09003AFAAB5A6E361BA2748EA9A005709B_il2cpp_TypeInfo_var, &___sampleFrameCount1);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback2, (RuntimeObject*)___object3);
}
// System.Void UnityEngine.Experimental.Audio.AudioSampleProvider_SampleFramesHandler::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SampleFramesHandler_EndInvoke_mF5305B3BA179CE3C49836790DE3FEB02EB088D28 (SampleFramesHandler_t5179C92AFBB393A85144E9134A862C161726F6AF * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
// System.Int32 UnityEngine.Microphone::GetMicrophoneDeviceIDFromName(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Microphone_GetMicrophoneDeviceIDFromName_m08C8735264E4D5FEAED224C8E7957CF5916D9A5E (String_t* ___name0, const RuntimeMethod* method)
{
	typedef int32_t (*Microphone_GetMicrophoneDeviceIDFromName_m08C8735264E4D5FEAED224C8E7957CF5916D9A5E_ftn) (String_t*);
	static Microphone_GetMicrophoneDeviceIDFromName_m08C8735264E4D5FEAED224C8E7957CF5916D9A5E_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (Microphone_GetMicrophoneDeviceIDFromName_m08C8735264E4D5FEAED224C8E7957CF5916D9A5E_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.Microphone::GetMicrophoneDeviceIDFromName(System.String)");
	int32_t retVal = _il2cpp_icall_func(___name0);
	return retVal;
}
// UnityEngine.AudioClip UnityEngine.Microphone::StartRecord(System.Int32,System.Boolean,System.Single,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * Microphone_StartRecord_m88F707BD42E1494928AA8D2E915062826E9021E8 (int32_t ___deviceID0, bool ___loop1, float ___lengthSec2, int32_t ___frequency3, const RuntimeMethod* method)
{
	typedef AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * (*Microphone_StartRecord_m88F707BD42E1494928AA8D2E915062826E9021E8_ftn) (int32_t, bool, float, int32_t);
	static Microphone_StartRecord_m88F707BD42E1494928AA8D2E915062826E9021E8_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (Microphone_StartRecord_m88F707BD42E1494928AA8D2E915062826E9021E8_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.Microphone::StartRecord(System.Int32,System.Boolean,System.Single,System.Int32)");
	AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * retVal = _il2cpp_icall_func(___deviceID0, ___loop1, ___lengthSec2, ___frequency3);
	return retVal;
}
// System.Void UnityEngine.Microphone::EndRecord(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Microphone_EndRecord_m6DDB48A30B1A34A6E996A1A1FC194DA184B46733 (int32_t ___deviceID0, const RuntimeMethod* method)
{
	typedef void (*Microphone_EndRecord_m6DDB48A30B1A34A6E996A1A1FC194DA184B46733_ftn) (int32_t);
	static Microphone_EndRecord_m6DDB48A30B1A34A6E996A1A1FC194DA184B46733_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (Microphone_EndRecord_m6DDB48A30B1A34A6E996A1A1FC194DA184B46733_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.Microphone::EndRecord(System.Int32)");
	_il2cpp_icall_func(___deviceID0);
}
// System.Boolean UnityEngine.Microphone::IsRecording(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Microphone_IsRecording_m70F5B087E69C0077AA9238931776EDE123FCF122 (int32_t ___deviceID0, const RuntimeMethod* method)
{
	typedef bool (*Microphone_IsRecording_m70F5B087E69C0077AA9238931776EDE123FCF122_ftn) (int32_t);
	static Microphone_IsRecording_m70F5B087E69C0077AA9238931776EDE123FCF122_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (Microphone_IsRecording_m70F5B087E69C0077AA9238931776EDE123FCF122_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.Microphone::IsRecording(System.Int32)");
	bool retVal = _il2cpp_icall_func(___deviceID0);
	return retVal;
}
// System.Void UnityEngine.Microphone::GetDeviceCaps(System.Int32,System.Int32&,System.Int32&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Microphone_GetDeviceCaps_m4592CD4A6371544156A042579626B9351DCD7E93 (int32_t ___deviceID0, int32_t* ___minFreq1, int32_t* ___maxFreq2, const RuntimeMethod* method)
{
	typedef void (*Microphone_GetDeviceCaps_m4592CD4A6371544156A042579626B9351DCD7E93_ftn) (int32_t, int32_t*, int32_t*);
	static Microphone_GetDeviceCaps_m4592CD4A6371544156A042579626B9351DCD7E93_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (Microphone_GetDeviceCaps_m4592CD4A6371544156A042579626B9351DCD7E93_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.Microphone::GetDeviceCaps(System.Int32,System.Int32&,System.Int32&)");
	_il2cpp_icall_func(___deviceID0, ___minFreq1, ___maxFreq2);
}
// UnityEngine.AudioClip UnityEngine.Microphone::Start(System.String,System.Boolean,System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * Microphone_Start_mF756A7EBA3E62EF0D138A220482B725D16E96047 (String_t* ___deviceName0, bool ___loop1, int32_t ___lengthSec2, int32_t ___frequency3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Microphone_Start_mF756A7EBA3E62EF0D138A220482B725D16E96047_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * V_1 = NULL;
	{
		String_t* L_0 = ___deviceName0;
		int32_t L_1 = Microphone_GetMicrophoneDeviceIDFromName_m08C8735264E4D5FEAED224C8E7957CF5916D9A5E(L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		int32_t L_2 = V_0;
		if ((!(((uint32_t)L_2) == ((uint32_t)(-1)))))
		{
			goto IL_0020;
		}
	}
	{
		String_t* L_3 = ___deviceName0;
		String_t* L_4 = String_Concat_mB78D0094592718DA6D5DB6C712A9C225631666BE(_stringLiteralB690CB7A4FD9F948BE23247E7090E398B232E4C1, L_3, /*hidden argument*/NULL);
		ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1 * L_5 = (ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1 *)il2cpp_codegen_object_new(ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1_il2cpp_TypeInfo_var);
		ArgumentException__ctor_m9A85EF7FEFEC21DDD525A67E831D77278E5165B7(L_5, L_4, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_5, NULL, Microphone_Start_mF756A7EBA3E62EF0D138A220482B725D16E96047_RuntimeMethod_var);
	}

IL_0020:
	{
		int32_t L_6 = ___lengthSec2;
		if ((((int32_t)L_6) > ((int32_t)0)))
		{
			goto IL_0042;
		}
	}
	{
		int32_t L_7 = ___lengthSec2;
		int32_t L_8 = L_7;
		RuntimeObject * L_9 = Box(Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var, &L_8);
		String_t* L_10 = String_Concat_m2E1F71C491D2429CC80A28745488FEA947BB7AAC(_stringLiteral2E4C82C746C3EC981DE6270E62A307A7F402A3C0, L_9, _stringLiteralE50B0C9845BE0262691292B3BC4739CAB8B57967, /*hidden argument*/NULL);
		ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1 * L_11 = (ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1 *)il2cpp_codegen_object_new(ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1_il2cpp_TypeInfo_var);
		ArgumentException__ctor_m9A85EF7FEFEC21DDD525A67E831D77278E5165B7(L_11, L_10, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_11, NULL, Microphone_Start_mF756A7EBA3E62EF0D138A220482B725D16E96047_RuntimeMethod_var);
	}

IL_0042:
	{
		int32_t L_12 = ___lengthSec2;
		if ((((int32_t)L_12) <= ((int32_t)((int32_t)3600))))
		{
			goto IL_0068;
		}
	}
	{
		int32_t L_13 = ___lengthSec2;
		int32_t L_14 = L_13;
		RuntimeObject * L_15 = Box(Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var, &L_14);
		String_t* L_16 = String_Concat_m2E1F71C491D2429CC80A28745488FEA947BB7AAC(_stringLiteral6E08EC217803485FD1258E704F681704792FDD20, L_15, _stringLiteralE50B0C9845BE0262691292B3BC4739CAB8B57967, /*hidden argument*/NULL);
		ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1 * L_17 = (ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1 *)il2cpp_codegen_object_new(ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1_il2cpp_TypeInfo_var);
		ArgumentException__ctor_m9A85EF7FEFEC21DDD525A67E831D77278E5165B7(L_17, L_16, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_17, NULL, Microphone_Start_mF756A7EBA3E62EF0D138A220482B725D16E96047_RuntimeMethod_var);
	}

IL_0068:
	{
		int32_t L_18 = ___frequency3;
		if ((((int32_t)L_18) > ((int32_t)0)))
		{
			goto IL_008a;
		}
	}
	{
		int32_t L_19 = ___frequency3;
		int32_t L_20 = L_19;
		RuntimeObject * L_21 = Box(Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var, &L_20);
		String_t* L_22 = String_Concat_m2E1F71C491D2429CC80A28745488FEA947BB7AAC(_stringLiteral4CDCA392B9BF22E4728254EA8AABA605D8A96AC3, L_21, _stringLiteral2F6210C7CB4E004CB3F104E1A7B40B6C6B7121FD, /*hidden argument*/NULL);
		ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1 * L_23 = (ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1 *)il2cpp_codegen_object_new(ArgumentException_tEDCD16F20A09ECE461C3DA766C16EDA8864057D1_il2cpp_TypeInfo_var);
		ArgumentException__ctor_m9A85EF7FEFEC21DDD525A67E831D77278E5165B7(L_23, L_22, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_23, NULL, Microphone_Start_mF756A7EBA3E62EF0D138A220482B725D16E96047_RuntimeMethod_var);
	}

IL_008a:
	{
		int32_t L_24 = V_0;
		bool L_25 = ___loop1;
		int32_t L_26 = ___lengthSec2;
		int32_t L_27 = ___frequency3;
		AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * L_28 = Microphone_StartRecord_m88F707BD42E1494928AA8D2E915062826E9021E8(L_24, L_25, (((float)((float)L_26))), L_27, /*hidden argument*/NULL);
		V_1 = L_28;
		goto IL_009a;
	}

IL_009a:
	{
		AudioClip_tCC3C35F579203CE2601243585AB3D6953C3BA051 * L_29 = V_1;
		return L_29;
	}
}
// System.Void UnityEngine.Microphone::End(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Microphone_End_m2E3D0E4890AE014AF687987F6160CA3D5ACDC29F (String_t* ___deviceName0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		String_t* L_0 = ___deviceName0;
		int32_t L_1 = Microphone_GetMicrophoneDeviceIDFromName_m08C8735264E4D5FEAED224C8E7957CF5916D9A5E(L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		int32_t L_2 = V_0;
		if ((!(((uint32_t)L_2) == ((uint32_t)(-1)))))
		{
			goto IL_0014;
		}
	}
	{
		goto IL_001a;
	}

IL_0014:
	{
		int32_t L_3 = V_0;
		Microphone_EndRecord_m6DDB48A30B1A34A6E996A1A1FC194DA184B46733(L_3, /*hidden argument*/NULL);
	}

IL_001a:
	{
		return;
	}
}
// System.Boolean UnityEngine.Microphone::IsRecording(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Microphone_IsRecording_m2E3373AD36865B7261BBA5E9140271A08E0FA004 (String_t* ___deviceName0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	bool V_1 = false;
	{
		String_t* L_0 = ___deviceName0;
		int32_t L_1 = Microphone_GetMicrophoneDeviceIDFromName_m08C8735264E4D5FEAED224C8E7957CF5916D9A5E(L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		int32_t L_2 = V_0;
		if ((!(((uint32_t)L_2) == ((uint32_t)(-1)))))
		{
			goto IL_0016;
		}
	}
	{
		V_1 = (bool)0;
		goto IL_0022;
	}

IL_0016:
	{
		int32_t L_3 = V_0;
		bool L_4 = Microphone_IsRecording_m70F5B087E69C0077AA9238931776EDE123FCF122(L_3, /*hidden argument*/NULL);
		V_1 = L_4;
		goto IL_0022;
	}

IL_0022:
	{
		bool L_5 = V_1;
		return L_5;
	}
}
// System.Void UnityEngine.Microphone::GetDeviceCaps(System.String,System.Int32&,System.Int32&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Microphone_GetDeviceCaps_mF079FFC698AE94F132D4E0AD072498F6937BAF6B (String_t* ___deviceName0, int32_t* ___minFreq1, int32_t* ___maxFreq2, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		int32_t* L_0 = ___minFreq1;
		*((int32_t*)L_0) = (int32_t)0;
		int32_t* L_1 = ___maxFreq2;
		*((int32_t*)L_1) = (int32_t)0;
		String_t* L_2 = ___deviceName0;
		int32_t L_3 = Microphone_GetMicrophoneDeviceIDFromName_m08C8735264E4D5FEAED224C8E7957CF5916D9A5E(L_2, /*hidden argument*/NULL);
		V_0 = L_3;
		int32_t L_4 = V_0;
		if ((!(((uint32_t)L_4) == ((uint32_t)(-1)))))
		{
			goto IL_001a;
		}
	}
	{
		goto IL_0022;
	}

IL_001a:
	{
		int32_t L_5 = V_0;
		int32_t* L_6 = ___minFreq1;
		int32_t* L_7 = ___maxFreq2;
		Microphone_GetDeviceCaps_m4592CD4A6371544156A042579626B9351DCD7E93(L_5, (int32_t*)L_6, (int32_t*)L_7, /*hidden argument*/NULL);
	}

IL_0022:
	{
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR void AudioSampleProvider_set_owner_m21FC4073346BFBE33E2B1567ECEC8ECD373B3069_inline (AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 * __this, Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * ___value0, const RuntimeMethod* method)
{
	{
		Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * L_0 = ___value0;
		__this->set_U3CownerU3Ek__BackingField_3(L_0);
		return;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR void AudioSampleProvider_set_id_mB98E8435407CAD305BBEAA91B18CBC12562DFAF9_inline (AudioSampleProvider_tD5B209D07C5F1D4714F92069F4071068B9BC6913 * __this, uint32_t ___value0, const RuntimeMethod* method)
{
	{
		uint32_t L_0 = ___value0;
		__this->set_U3CidU3Ek__BackingField_1(L_0);
		return;
	}
}
