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

template <typename T1, typename T2, typename T3>
struct VirtActionInvoker3
{
	typedef void (*Action)(void*, T1, T2, T3, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2, T3 p3)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		((Action)invokeData.methodPtr)(obj, p1, p2, p3, invokeData.method);
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

// System.Char[]
struct CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2;
// System.String
struct String_t;
// System.Void
struct Void_t22962CB4C05B1D89B55A6E1139F0E87A90987017;
// UnityEngine.GameObject
struct GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F;
// UnityEngine.ScriptableObject
struct ScriptableObject_tAB015486CEAB714DA0D5C1BA389B84FB90427734;
// UnityEngine.Sprite
struct Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198;
// UnityEngine.Sprite[]
struct SpriteU5BU5D_tF94AD07E062BC08ECD019A21E7A7B861654905F7;
// UnityEngine.Tilemaps.ITilemap
struct ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3;
// UnityEngine.Tilemaps.Tile
struct Tile_t275C7CE9C854F2912C851F345CCC00C45EDDE7AE;
// UnityEngine.Tilemaps.TileBase
struct TileBase_tD2158024AAA28EB0EC62F253FA1D1A76BC50F85E;
// UnityEngine.Tilemaps.Tilemap
struct Tilemap_t0F92148668211805A631B93488D4A629EC378B10;

IL2CPP_EXTERN_C RuntimeClass* ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C const uint32_t ITilemap_CreateInstance_m3DD29D9D393B923E0901DB830E0FE9170757AF9E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t Tile__ctor_mCF182CC986391B7BC36D0C543E7D422F492FA445_MetadataUsageId;

struct SpriteU5BU5D_tF94AD07E062BC08ECD019A21E7A7B861654905F7;

IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// <Module>
struct  U3CModuleU3E_t140A63E1069F1772D5238396CDA7CE5372253E44 
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

// UnityEngine.Tilemaps.ITilemap
struct  ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3  : public RuntimeObject
{
public:
	// UnityEngine.Tilemaps.Tilemap UnityEngine.Tilemaps.ITilemap::m_Tilemap
	Tilemap_t0F92148668211805A631B93488D4A629EC378B10 * ___m_Tilemap_1;

public:
	inline static int32_t get_offset_of_m_Tilemap_1() { return static_cast<int32_t>(offsetof(ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3, ___m_Tilemap_1)); }
	inline Tilemap_t0F92148668211805A631B93488D4A629EC378B10 * get_m_Tilemap_1() const { return ___m_Tilemap_1; }
	inline Tilemap_t0F92148668211805A631B93488D4A629EC378B10 ** get_address_of_m_Tilemap_1() { return &___m_Tilemap_1; }
	inline void set_m_Tilemap_1(Tilemap_t0F92148668211805A631B93488D4A629EC378B10 * value)
	{
		___m_Tilemap_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Tilemap_1), (void*)value);
	}
};

struct ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3_StaticFields
{
public:
	// UnityEngine.Tilemaps.ITilemap UnityEngine.Tilemaps.ITilemap::s_Instance
	ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 * ___s_Instance_0;

public:
	inline static int32_t get_offset_of_s_Instance_0() { return static_cast<int32_t>(offsetof(ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3_StaticFields, ___s_Instance_0)); }
	inline ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 * get_s_Instance_0() const { return ___s_Instance_0; }
	inline ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 ** get_address_of_s_Instance_0() { return &___s_Instance_0; }
	inline void set_s_Instance_0(ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 * value)
	{
		___s_Instance_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_Instance_0), (void*)value);
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


// UnityEngine.Color
struct  Color_t119BCA590009762C7223FDD3AF9706653AC84ED2 
{
public:
	// System.Single UnityEngine.Color::r
	float ___r_0;
	// System.Single UnityEngine.Color::g
	float ___g_1;
	// System.Single UnityEngine.Color::b
	float ___b_2;
	// System.Single UnityEngine.Color::a
	float ___a_3;

public:
	inline static int32_t get_offset_of_r_0() { return static_cast<int32_t>(offsetof(Color_t119BCA590009762C7223FDD3AF9706653AC84ED2, ___r_0)); }
	inline float get_r_0() const { return ___r_0; }
	inline float* get_address_of_r_0() { return &___r_0; }
	inline void set_r_0(float value)
	{
		___r_0 = value;
	}

	inline static int32_t get_offset_of_g_1() { return static_cast<int32_t>(offsetof(Color_t119BCA590009762C7223FDD3AF9706653AC84ED2, ___g_1)); }
	inline float get_g_1() const { return ___g_1; }
	inline float* get_address_of_g_1() { return &___g_1; }
	inline void set_g_1(float value)
	{
		___g_1 = value;
	}

	inline static int32_t get_offset_of_b_2() { return static_cast<int32_t>(offsetof(Color_t119BCA590009762C7223FDD3AF9706653AC84ED2, ___b_2)); }
	inline float get_b_2() const { return ___b_2; }
	inline float* get_address_of_b_2() { return &___b_2; }
	inline void set_b_2(float value)
	{
		___b_2 = value;
	}

	inline static int32_t get_offset_of_a_3() { return static_cast<int32_t>(offsetof(Color_t119BCA590009762C7223FDD3AF9706653AC84ED2, ___a_3)); }
	inline float get_a_3() const { return ___a_3; }
	inline float* get_address_of_a_3() { return &___a_3; }
	inline void set_a_3(float value)
	{
		___a_3 = value;
	}
};


// UnityEngine.Matrix4x4
struct  Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA 
{
public:
	// System.Single UnityEngine.Matrix4x4::m00
	float ___m00_0;
	// System.Single UnityEngine.Matrix4x4::m10
	float ___m10_1;
	// System.Single UnityEngine.Matrix4x4::m20
	float ___m20_2;
	// System.Single UnityEngine.Matrix4x4::m30
	float ___m30_3;
	// System.Single UnityEngine.Matrix4x4::m01
	float ___m01_4;
	// System.Single UnityEngine.Matrix4x4::m11
	float ___m11_5;
	// System.Single UnityEngine.Matrix4x4::m21
	float ___m21_6;
	// System.Single UnityEngine.Matrix4x4::m31
	float ___m31_7;
	// System.Single UnityEngine.Matrix4x4::m02
	float ___m02_8;
	// System.Single UnityEngine.Matrix4x4::m12
	float ___m12_9;
	// System.Single UnityEngine.Matrix4x4::m22
	float ___m22_10;
	// System.Single UnityEngine.Matrix4x4::m32
	float ___m32_11;
	// System.Single UnityEngine.Matrix4x4::m03
	float ___m03_12;
	// System.Single UnityEngine.Matrix4x4::m13
	float ___m13_13;
	// System.Single UnityEngine.Matrix4x4::m23
	float ___m23_14;
	// System.Single UnityEngine.Matrix4x4::m33
	float ___m33_15;

public:
	inline static int32_t get_offset_of_m00_0() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m00_0)); }
	inline float get_m00_0() const { return ___m00_0; }
	inline float* get_address_of_m00_0() { return &___m00_0; }
	inline void set_m00_0(float value)
	{
		___m00_0 = value;
	}

	inline static int32_t get_offset_of_m10_1() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m10_1)); }
	inline float get_m10_1() const { return ___m10_1; }
	inline float* get_address_of_m10_1() { return &___m10_1; }
	inline void set_m10_1(float value)
	{
		___m10_1 = value;
	}

	inline static int32_t get_offset_of_m20_2() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m20_2)); }
	inline float get_m20_2() const { return ___m20_2; }
	inline float* get_address_of_m20_2() { return &___m20_2; }
	inline void set_m20_2(float value)
	{
		___m20_2 = value;
	}

	inline static int32_t get_offset_of_m30_3() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m30_3)); }
	inline float get_m30_3() const { return ___m30_3; }
	inline float* get_address_of_m30_3() { return &___m30_3; }
	inline void set_m30_3(float value)
	{
		___m30_3 = value;
	}

	inline static int32_t get_offset_of_m01_4() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m01_4)); }
	inline float get_m01_4() const { return ___m01_4; }
	inline float* get_address_of_m01_4() { return &___m01_4; }
	inline void set_m01_4(float value)
	{
		___m01_4 = value;
	}

	inline static int32_t get_offset_of_m11_5() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m11_5)); }
	inline float get_m11_5() const { return ___m11_5; }
	inline float* get_address_of_m11_5() { return &___m11_5; }
	inline void set_m11_5(float value)
	{
		___m11_5 = value;
	}

	inline static int32_t get_offset_of_m21_6() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m21_6)); }
	inline float get_m21_6() const { return ___m21_6; }
	inline float* get_address_of_m21_6() { return &___m21_6; }
	inline void set_m21_6(float value)
	{
		___m21_6 = value;
	}

	inline static int32_t get_offset_of_m31_7() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m31_7)); }
	inline float get_m31_7() const { return ___m31_7; }
	inline float* get_address_of_m31_7() { return &___m31_7; }
	inline void set_m31_7(float value)
	{
		___m31_7 = value;
	}

	inline static int32_t get_offset_of_m02_8() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m02_8)); }
	inline float get_m02_8() const { return ___m02_8; }
	inline float* get_address_of_m02_8() { return &___m02_8; }
	inline void set_m02_8(float value)
	{
		___m02_8 = value;
	}

	inline static int32_t get_offset_of_m12_9() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m12_9)); }
	inline float get_m12_9() const { return ___m12_9; }
	inline float* get_address_of_m12_9() { return &___m12_9; }
	inline void set_m12_9(float value)
	{
		___m12_9 = value;
	}

	inline static int32_t get_offset_of_m22_10() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m22_10)); }
	inline float get_m22_10() const { return ___m22_10; }
	inline float* get_address_of_m22_10() { return &___m22_10; }
	inline void set_m22_10(float value)
	{
		___m22_10 = value;
	}

	inline static int32_t get_offset_of_m32_11() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m32_11)); }
	inline float get_m32_11() const { return ___m32_11; }
	inline float* get_address_of_m32_11() { return &___m32_11; }
	inline void set_m32_11(float value)
	{
		___m32_11 = value;
	}

	inline static int32_t get_offset_of_m03_12() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m03_12)); }
	inline float get_m03_12() const { return ___m03_12; }
	inline float* get_address_of_m03_12() { return &___m03_12; }
	inline void set_m03_12(float value)
	{
		___m03_12 = value;
	}

	inline static int32_t get_offset_of_m13_13() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m13_13)); }
	inline float get_m13_13() const { return ___m13_13; }
	inline float* get_address_of_m13_13() { return &___m13_13; }
	inline void set_m13_13(float value)
	{
		___m13_13 = value;
	}

	inline static int32_t get_offset_of_m23_14() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m23_14)); }
	inline float get_m23_14() const { return ___m23_14; }
	inline float* get_address_of_m23_14() { return &___m23_14; }
	inline void set_m23_14(float value)
	{
		___m23_14 = value;
	}

	inline static int32_t get_offset_of_m33_15() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA, ___m33_15)); }
	inline float get_m33_15() const { return ___m33_15; }
	inline float* get_address_of_m33_15() { return &___m33_15; }
	inline void set_m33_15(float value)
	{
		___m33_15 = value;
	}
};

struct Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA_StaticFields
{
public:
	// UnityEngine.Matrix4x4 UnityEngine.Matrix4x4::zeroMatrix
	Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  ___zeroMatrix_16;
	// UnityEngine.Matrix4x4 UnityEngine.Matrix4x4::identityMatrix
	Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  ___identityMatrix_17;

public:
	inline static int32_t get_offset_of_zeroMatrix_16() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA_StaticFields, ___zeroMatrix_16)); }
	inline Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  get_zeroMatrix_16() const { return ___zeroMatrix_16; }
	inline Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA * get_address_of_zeroMatrix_16() { return &___zeroMatrix_16; }
	inline void set_zeroMatrix_16(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  value)
	{
		___zeroMatrix_16 = value;
	}

	inline static int32_t get_offset_of_identityMatrix_17() { return static_cast<int32_t>(offsetof(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA_StaticFields, ___identityMatrix_17)); }
	inline Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  get_identityMatrix_17() const { return ___identityMatrix_17; }
	inline Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA * get_address_of_identityMatrix_17() { return &___identityMatrix_17; }
	inline void set_identityMatrix_17(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  value)
	{
		___identityMatrix_17 = value;
	}
};


// UnityEngine.Tilemaps.TileAnimationData
struct  TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8 
{
public:
	// UnityEngine.Sprite[] UnityEngine.Tilemaps.TileAnimationData::m_AnimatedSprites
	SpriteU5BU5D_tF94AD07E062BC08ECD019A21E7A7B861654905F7* ___m_AnimatedSprites_0;
	// System.Single UnityEngine.Tilemaps.TileAnimationData::m_AnimationSpeed
	float ___m_AnimationSpeed_1;
	// System.Single UnityEngine.Tilemaps.TileAnimationData::m_AnimationStartTime
	float ___m_AnimationStartTime_2;

public:
	inline static int32_t get_offset_of_m_AnimatedSprites_0() { return static_cast<int32_t>(offsetof(TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8, ___m_AnimatedSprites_0)); }
	inline SpriteU5BU5D_tF94AD07E062BC08ECD019A21E7A7B861654905F7* get_m_AnimatedSprites_0() const { return ___m_AnimatedSprites_0; }
	inline SpriteU5BU5D_tF94AD07E062BC08ECD019A21E7A7B861654905F7** get_address_of_m_AnimatedSprites_0() { return &___m_AnimatedSprites_0; }
	inline void set_m_AnimatedSprites_0(SpriteU5BU5D_tF94AD07E062BC08ECD019A21E7A7B861654905F7* value)
	{
		___m_AnimatedSprites_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_AnimatedSprites_0), (void*)value);
	}

	inline static int32_t get_offset_of_m_AnimationSpeed_1() { return static_cast<int32_t>(offsetof(TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8, ___m_AnimationSpeed_1)); }
	inline float get_m_AnimationSpeed_1() const { return ___m_AnimationSpeed_1; }
	inline float* get_address_of_m_AnimationSpeed_1() { return &___m_AnimationSpeed_1; }
	inline void set_m_AnimationSpeed_1(float value)
	{
		___m_AnimationSpeed_1 = value;
	}

	inline static int32_t get_offset_of_m_AnimationStartTime_2() { return static_cast<int32_t>(offsetof(TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8, ___m_AnimationStartTime_2)); }
	inline float get_m_AnimationStartTime_2() const { return ___m_AnimationStartTime_2; }
	inline float* get_address_of_m_AnimationStartTime_2() { return &___m_AnimationStartTime_2; }
	inline void set_m_AnimationStartTime_2(float value)
	{
		___m_AnimationStartTime_2 = value;
	}
};

// Native definition for P/Invoke marshalling of UnityEngine.Tilemaps.TileAnimationData
struct TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8_marshaled_pinvoke
{
	SpriteU5BU5D_tF94AD07E062BC08ECD019A21E7A7B861654905F7* ___m_AnimatedSprites_0;
	float ___m_AnimationSpeed_1;
	float ___m_AnimationStartTime_2;
};
// Native definition for COM marshalling of UnityEngine.Tilemaps.TileAnimationData
struct TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8_marshaled_com
{
	SpriteU5BU5D_tF94AD07E062BC08ECD019A21E7A7B861654905F7* ___m_AnimatedSprites_0;
	float ___m_AnimationSpeed_1;
	float ___m_AnimationStartTime_2;
};

// UnityEngine.Vector3Int
struct  Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 
{
public:
	// System.Int32 UnityEngine.Vector3Int::m_X
	int32_t ___m_X_0;
	// System.Int32 UnityEngine.Vector3Int::m_Y
	int32_t ___m_Y_1;
	// System.Int32 UnityEngine.Vector3Int::m_Z
	int32_t ___m_Z_2;

public:
	inline static int32_t get_offset_of_m_X_0() { return static_cast<int32_t>(offsetof(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4, ___m_X_0)); }
	inline int32_t get_m_X_0() const { return ___m_X_0; }
	inline int32_t* get_address_of_m_X_0() { return &___m_X_0; }
	inline void set_m_X_0(int32_t value)
	{
		___m_X_0 = value;
	}

	inline static int32_t get_offset_of_m_Y_1() { return static_cast<int32_t>(offsetof(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4, ___m_Y_1)); }
	inline int32_t get_m_Y_1() const { return ___m_Y_1; }
	inline int32_t* get_address_of_m_Y_1() { return &___m_Y_1; }
	inline void set_m_Y_1(int32_t value)
	{
		___m_Y_1 = value;
	}

	inline static int32_t get_offset_of_m_Z_2() { return static_cast<int32_t>(offsetof(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4, ___m_Z_2)); }
	inline int32_t get_m_Z_2() const { return ___m_Z_2; }
	inline int32_t* get_address_of_m_Z_2() { return &___m_Z_2; }
	inline void set_m_Z_2(int32_t value)
	{
		___m_Z_2 = value;
	}
};

struct Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4_StaticFields
{
public:
	// UnityEngine.Vector3Int UnityEngine.Vector3Int::s_Zero
	Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___s_Zero_3;
	// UnityEngine.Vector3Int UnityEngine.Vector3Int::s_One
	Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___s_One_4;
	// UnityEngine.Vector3Int UnityEngine.Vector3Int::s_Up
	Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___s_Up_5;
	// UnityEngine.Vector3Int UnityEngine.Vector3Int::s_Down
	Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___s_Down_6;
	// UnityEngine.Vector3Int UnityEngine.Vector3Int::s_Left
	Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___s_Left_7;
	// UnityEngine.Vector3Int UnityEngine.Vector3Int::s_Right
	Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___s_Right_8;

public:
	inline static int32_t get_offset_of_s_Zero_3() { return static_cast<int32_t>(offsetof(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4_StaticFields, ___s_Zero_3)); }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  get_s_Zero_3() const { return ___s_Zero_3; }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 * get_address_of_s_Zero_3() { return &___s_Zero_3; }
	inline void set_s_Zero_3(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  value)
	{
		___s_Zero_3 = value;
	}

	inline static int32_t get_offset_of_s_One_4() { return static_cast<int32_t>(offsetof(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4_StaticFields, ___s_One_4)); }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  get_s_One_4() const { return ___s_One_4; }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 * get_address_of_s_One_4() { return &___s_One_4; }
	inline void set_s_One_4(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  value)
	{
		___s_One_4 = value;
	}

	inline static int32_t get_offset_of_s_Up_5() { return static_cast<int32_t>(offsetof(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4_StaticFields, ___s_Up_5)); }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  get_s_Up_5() const { return ___s_Up_5; }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 * get_address_of_s_Up_5() { return &___s_Up_5; }
	inline void set_s_Up_5(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  value)
	{
		___s_Up_5 = value;
	}

	inline static int32_t get_offset_of_s_Down_6() { return static_cast<int32_t>(offsetof(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4_StaticFields, ___s_Down_6)); }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  get_s_Down_6() const { return ___s_Down_6; }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 * get_address_of_s_Down_6() { return &___s_Down_6; }
	inline void set_s_Down_6(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  value)
	{
		___s_Down_6 = value;
	}

	inline static int32_t get_offset_of_s_Left_7() { return static_cast<int32_t>(offsetof(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4_StaticFields, ___s_Left_7)); }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  get_s_Left_7() const { return ___s_Left_7; }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 * get_address_of_s_Left_7() { return &___s_Left_7; }
	inline void set_s_Left_7(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  value)
	{
		___s_Left_7 = value;
	}

	inline static int32_t get_offset_of_s_Right_8() { return static_cast<int32_t>(offsetof(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4_StaticFields, ___s_Right_8)); }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  get_s_Right_8() const { return ___s_Right_8; }
	inline Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 * get_address_of_s_Right_8() { return &___s_Right_8; }
	inline void set_s_Right_8(Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  value)
	{
		___s_Right_8 = value;
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

// UnityEngine.Tilemaps.Tile_ColliderType
struct  ColliderType_tCF48B308BF04CE1D262A726D500126E8C8859F2B 
{
public:
	// System.Int32 UnityEngine.Tilemaps.Tile_ColliderType::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(ColliderType_tCF48B308BF04CE1D262A726D500126E8C8859F2B, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.Tilemaps.TileFlags
struct  TileFlags_tBD601639E3CC4B4BA675548F3B8F17B709A974AF 
{
public:
	// System.Int32 UnityEngine.Tilemaps.TileFlags::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(TileFlags_tBD601639E3CC4B4BA675548F3B8F17B709A974AF, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
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

// UnityEngine.Sprite
struct  Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198  : public Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0
{
public:

public:
};


// UnityEngine.Tilemaps.TileData
struct  TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 
{
public:
	// UnityEngine.Sprite UnityEngine.Tilemaps.TileData::m_Sprite
	Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * ___m_Sprite_0;
	// UnityEngine.Color UnityEngine.Tilemaps.TileData::m_Color
	Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  ___m_Color_1;
	// UnityEngine.Matrix4x4 UnityEngine.Tilemaps.TileData::m_Transform
	Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  ___m_Transform_2;
	// UnityEngine.GameObject UnityEngine.Tilemaps.TileData::m_GameObject
	GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * ___m_GameObject_3;
	// UnityEngine.Tilemaps.TileFlags UnityEngine.Tilemaps.TileData::m_Flags
	int32_t ___m_Flags_4;
	// UnityEngine.Tilemaps.Tile_ColliderType UnityEngine.Tilemaps.TileData::m_ColliderType
	int32_t ___m_ColliderType_5;

public:
	inline static int32_t get_offset_of_m_Sprite_0() { return static_cast<int32_t>(offsetof(TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5, ___m_Sprite_0)); }
	inline Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * get_m_Sprite_0() const { return ___m_Sprite_0; }
	inline Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 ** get_address_of_m_Sprite_0() { return &___m_Sprite_0; }
	inline void set_m_Sprite_0(Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * value)
	{
		___m_Sprite_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Sprite_0), (void*)value);
	}

	inline static int32_t get_offset_of_m_Color_1() { return static_cast<int32_t>(offsetof(TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5, ___m_Color_1)); }
	inline Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  get_m_Color_1() const { return ___m_Color_1; }
	inline Color_t119BCA590009762C7223FDD3AF9706653AC84ED2 * get_address_of_m_Color_1() { return &___m_Color_1; }
	inline void set_m_Color_1(Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  value)
	{
		___m_Color_1 = value;
	}

	inline static int32_t get_offset_of_m_Transform_2() { return static_cast<int32_t>(offsetof(TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5, ___m_Transform_2)); }
	inline Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  get_m_Transform_2() const { return ___m_Transform_2; }
	inline Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA * get_address_of_m_Transform_2() { return &___m_Transform_2; }
	inline void set_m_Transform_2(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  value)
	{
		___m_Transform_2 = value;
	}

	inline static int32_t get_offset_of_m_GameObject_3() { return static_cast<int32_t>(offsetof(TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5, ___m_GameObject_3)); }
	inline GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * get_m_GameObject_3() const { return ___m_GameObject_3; }
	inline GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F ** get_address_of_m_GameObject_3() { return &___m_GameObject_3; }
	inline void set_m_GameObject_3(GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * value)
	{
		___m_GameObject_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_GameObject_3), (void*)value);
	}

	inline static int32_t get_offset_of_m_Flags_4() { return static_cast<int32_t>(offsetof(TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5, ___m_Flags_4)); }
	inline int32_t get_m_Flags_4() const { return ___m_Flags_4; }
	inline int32_t* get_address_of_m_Flags_4() { return &___m_Flags_4; }
	inline void set_m_Flags_4(int32_t value)
	{
		___m_Flags_4 = value;
	}

	inline static int32_t get_offset_of_m_ColliderType_5() { return static_cast<int32_t>(offsetof(TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5, ___m_ColliderType_5)); }
	inline int32_t get_m_ColliderType_5() const { return ___m_ColliderType_5; }
	inline int32_t* get_address_of_m_ColliderType_5() { return &___m_ColliderType_5; }
	inline void set_m_ColliderType_5(int32_t value)
	{
		___m_ColliderType_5 = value;
	}
};

// Native definition for P/Invoke marshalling of UnityEngine.Tilemaps.TileData
struct TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5_marshaled_pinvoke
{
	Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * ___m_Sprite_0;
	Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  ___m_Color_1;
	Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  ___m_Transform_2;
	GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * ___m_GameObject_3;
	int32_t ___m_Flags_4;
	int32_t ___m_ColliderType_5;
};
// Native definition for COM marshalling of UnityEngine.Tilemaps.TileData
struct TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5_marshaled_com
{
	Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * ___m_Sprite_0;
	Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  ___m_Color_1;
	Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  ___m_Transform_2;
	GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * ___m_GameObject_3;
	int32_t ___m_Flags_4;
	int32_t ___m_ColliderType_5;
};

// UnityEngine.Behaviour
struct  Behaviour_tBDC7E9C3C898AD8348891B82D3E345801D920CA8  : public Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621
{
public:

public:
};


// UnityEngine.Tilemaps.TileBase
struct  TileBase_tD2158024AAA28EB0EC62F253FA1D1A76BC50F85E  : public ScriptableObject_tAB015486CEAB714DA0D5C1BA389B84FB90427734
{
public:

public:
};


// UnityEngine.GridLayout
struct  GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9  : public Behaviour_tBDC7E9C3C898AD8348891B82D3E345801D920CA8
{
public:

public:
};


// UnityEngine.Tilemaps.Tile
struct  Tile_t275C7CE9C854F2912C851F345CCC00C45EDDE7AE  : public TileBase_tD2158024AAA28EB0EC62F253FA1D1A76BC50F85E
{
public:
	// UnityEngine.Sprite UnityEngine.Tilemaps.Tile::m_Sprite
	Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * ___m_Sprite_4;
	// UnityEngine.Color UnityEngine.Tilemaps.Tile::m_Color
	Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  ___m_Color_5;
	// UnityEngine.Matrix4x4 UnityEngine.Tilemaps.Tile::m_Transform
	Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  ___m_Transform_6;
	// UnityEngine.GameObject UnityEngine.Tilemaps.Tile::m_InstancedGameObject
	GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * ___m_InstancedGameObject_7;
	// UnityEngine.Tilemaps.TileFlags UnityEngine.Tilemaps.Tile::m_Flags
	int32_t ___m_Flags_8;
	// UnityEngine.Tilemaps.Tile_ColliderType UnityEngine.Tilemaps.Tile::m_ColliderType
	int32_t ___m_ColliderType_9;

public:
	inline static int32_t get_offset_of_m_Sprite_4() { return static_cast<int32_t>(offsetof(Tile_t275C7CE9C854F2912C851F345CCC00C45EDDE7AE, ___m_Sprite_4)); }
	inline Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * get_m_Sprite_4() const { return ___m_Sprite_4; }
	inline Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 ** get_address_of_m_Sprite_4() { return &___m_Sprite_4; }
	inline void set_m_Sprite_4(Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * value)
	{
		___m_Sprite_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Sprite_4), (void*)value);
	}

	inline static int32_t get_offset_of_m_Color_5() { return static_cast<int32_t>(offsetof(Tile_t275C7CE9C854F2912C851F345CCC00C45EDDE7AE, ___m_Color_5)); }
	inline Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  get_m_Color_5() const { return ___m_Color_5; }
	inline Color_t119BCA590009762C7223FDD3AF9706653AC84ED2 * get_address_of_m_Color_5() { return &___m_Color_5; }
	inline void set_m_Color_5(Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  value)
	{
		___m_Color_5 = value;
	}

	inline static int32_t get_offset_of_m_Transform_6() { return static_cast<int32_t>(offsetof(Tile_t275C7CE9C854F2912C851F345CCC00C45EDDE7AE, ___m_Transform_6)); }
	inline Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  get_m_Transform_6() const { return ___m_Transform_6; }
	inline Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA * get_address_of_m_Transform_6() { return &___m_Transform_6; }
	inline void set_m_Transform_6(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  value)
	{
		___m_Transform_6 = value;
	}

	inline static int32_t get_offset_of_m_InstancedGameObject_7() { return static_cast<int32_t>(offsetof(Tile_t275C7CE9C854F2912C851F345CCC00C45EDDE7AE, ___m_InstancedGameObject_7)); }
	inline GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * get_m_InstancedGameObject_7() const { return ___m_InstancedGameObject_7; }
	inline GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F ** get_address_of_m_InstancedGameObject_7() { return &___m_InstancedGameObject_7; }
	inline void set_m_InstancedGameObject_7(GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * value)
	{
		___m_InstancedGameObject_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_InstancedGameObject_7), (void*)value);
	}

	inline static int32_t get_offset_of_m_Flags_8() { return static_cast<int32_t>(offsetof(Tile_t275C7CE9C854F2912C851F345CCC00C45EDDE7AE, ___m_Flags_8)); }
	inline int32_t get_m_Flags_8() const { return ___m_Flags_8; }
	inline int32_t* get_address_of_m_Flags_8() { return &___m_Flags_8; }
	inline void set_m_Flags_8(int32_t value)
	{
		___m_Flags_8 = value;
	}

	inline static int32_t get_offset_of_m_ColliderType_9() { return static_cast<int32_t>(offsetof(Tile_t275C7CE9C854F2912C851F345CCC00C45EDDE7AE, ___m_ColliderType_9)); }
	inline int32_t get_m_ColliderType_9() const { return ___m_ColliderType_9; }
	inline int32_t* get_address_of_m_ColliderType_9() { return &___m_ColliderType_9; }
	inline void set_m_ColliderType_9(int32_t value)
	{
		___m_ColliderType_9 = value;
	}
};


// UnityEngine.Tilemaps.Tilemap
struct  Tilemap_t0F92148668211805A631B93488D4A629EC378B10  : public GridLayout_t271F88A0992C64FE8E229D03480E3862B22D57F9
{
public:

public:
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
// UnityEngine.Sprite[]
struct SpriteU5BU5D_tF94AD07E062BC08ECD019A21E7A7B861654905F7  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * m_Items[1];

public:
	inline Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};



// System.Void System.Object::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0 (RuntimeObject * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Tilemaps.Tilemap::RefreshTile(UnityEngine.Vector3Int)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Tilemap_RefreshTile_m9F32ACD729A4C9D843662E956B38BB91351E01EA (Tilemap_t0F92148668211805A631B93488D4A629EC378B10 * __this, Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___position0, const RuntimeMethod* method);
// System.Void UnityEngine.Tilemaps.ITilemap::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ITilemap__ctor_m35D698BF933B4EF8217C29617B9F81E0E8826B7A (ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 * __this, const RuntimeMethod* method);
// UnityEngine.Color UnityEngine.Color::get_white()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  Color_get_white_mE7F3AC4FF0D6F35E48049C73116A222CBE96D905 (const RuntimeMethod* method);
// UnityEngine.Matrix4x4 UnityEngine.Matrix4x4::get_identity()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  Matrix4x4_get_identity_mA0CECDE2A5E85CF014375084624F3770B5B7B79B (const RuntimeMethod* method);
// System.Void UnityEngine.Tilemaps.TileBase::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TileBase__ctor_m24A60EEED10D8C5667B24C8FB486087662417720 (TileBase_tD2158024AAA28EB0EC62F253FA1D1A76BC50F85E * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Tilemaps.TileData::set_sprite(UnityEngine.Sprite)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TileData_set_sprite_m8E47803AAA5DA51504EF24A57EC2432D53B9CCB0 (TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * __this, Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * ___value0, const RuntimeMethod* method);
// System.Void UnityEngine.Tilemaps.TileData::set_color(UnityEngine.Color)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TileData_set_color_m518FDC6D6A2287BB1C69BA55FE8C02977A55EA4C (TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * __this, Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  ___value0, const RuntimeMethod* method);
// System.Void UnityEngine.Tilemaps.TileData::set_transform(UnityEngine.Matrix4x4)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TileData_set_transform_m4F98BB7795C0837AE119FE7C629E14238184422A (TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * __this, Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  ___value0, const RuntimeMethod* method);
// System.Void UnityEngine.Tilemaps.TileData::set_gameObject(UnityEngine.GameObject)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TileData_set_gameObject_m1E8E839859E84C9E578E8C5F23CD343BD7933C49 (TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * __this, GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * ___value0, const RuntimeMethod* method);
// System.Void UnityEngine.Tilemaps.TileData::set_flags(UnityEngine.Tilemaps.TileFlags)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TileData_set_flags_m7817AC2D2183D25550281C3F868DAD2D4BCDB425 (TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * __this, int32_t ___value0, const RuntimeMethod* method);
// System.Void UnityEngine.Tilemaps.TileData::set_colliderType(UnityEngine.Tilemaps.Tile/ColliderType)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TileData_set_colliderType_mC23D9C79557D9C0D41F507D4F8B427B843E5C11B (TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * __this, int32_t ___value0, const RuntimeMethod* method);
// System.Void UnityEngine.ScriptableObject::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ScriptableObject__ctor_m6E2B3821A4A361556FC12E9B1C71E1D5DC002C5B (ScriptableObject_tAB015486CEAB714DA0D5C1BA389B84FB90427734 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Tilemaps.ITilemap::RefreshTile(UnityEngine.Vector3Int)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ITilemap_RefreshTile_mA824B41C2B9791C91FC5FB3AA93DDD478713564C (ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 * __this, Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___position0, const RuntimeMethod* method);
// System.Void UnityEngine.Tilemaps.Tilemap::RefreshTile_Injected(UnityEngine.Vector3Int&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Tilemap_RefreshTile_Injected_m71B01A4DDE057FE002AE5BEC5ACB38F04BC64792 (Tilemap_t0F92148668211805A631B93488D4A629EC378B10 * __this, Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 * ___position0, const RuntimeMethod* method);
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
// System.Void UnityEngine.Tilemaps.ITilemap::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ITilemap__ctor_m35D698BF933B4EF8217C29617B9F81E0E8826B7A (ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 * __this, const RuntimeMethod* method)
{
	{
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		return;
	}
}
// System.Void UnityEngine.Tilemaps.ITilemap::RefreshTile(UnityEngine.Vector3Int)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ITilemap_RefreshTile_mA824B41C2B9791C91FC5FB3AA93DDD478713564C (ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 * __this, Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___position0, const RuntimeMethod* method)
{
	{
		Tilemap_t0F92148668211805A631B93488D4A629EC378B10 * L_0 = __this->get_m_Tilemap_1();
		Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  L_1 = ___position0;
		NullCheck(L_0);
		Tilemap_RefreshTile_m9F32ACD729A4C9D843662E956B38BB91351E01EA(L_0, L_1, /*hidden argument*/NULL);
		return;
	}
}
// UnityEngine.Tilemaps.ITilemap UnityEngine.Tilemaps.ITilemap::CreateInstance()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 * ITilemap_CreateInstance_m3DD29D9D393B923E0901DB830E0FE9170757AF9E (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ITilemap_CreateInstance_m3DD29D9D393B923E0901DB830E0FE9170757AF9E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 * V_0 = NULL;
	{
		ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 * L_0 = (ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 *)il2cpp_codegen_object_new(ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3_il2cpp_TypeInfo_var);
		ITilemap__ctor_m35D698BF933B4EF8217C29617B9F81E0E8826B7A(L_0, /*hidden argument*/NULL);
		((ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3_StaticFields*)il2cpp_codegen_static_fields_for(ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3_il2cpp_TypeInfo_var))->set_s_Instance_0(L_0);
		ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 * L_1 = ((ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3_StaticFields*)il2cpp_codegen_static_fields_for(ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3_il2cpp_TypeInfo_var))->get_s_Instance_0();
		V_0 = L_1;
		goto IL_0016;
	}

IL_0016:
	{
		ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 * L_2 = V_0;
		return L_2;
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
// System.Void UnityEngine.Tilemaps.Tile::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Tile__ctor_mCF182CC986391B7BC36D0C543E7D422F492FA445 (Tile_t275C7CE9C854F2912C851F345CCC00C45EDDE7AE * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (Tile__ctor_mCF182CC986391B7BC36D0C543E7D422F492FA445_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  L_0 = Color_get_white_mE7F3AC4FF0D6F35E48049C73116A222CBE96D905(/*hidden argument*/NULL);
		__this->set_m_Color_5(L_0);
		IL2CPP_RUNTIME_CLASS_INIT(Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA_il2cpp_TypeInfo_var);
		Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  L_1 = Matrix4x4_get_identity_mA0CECDE2A5E85CF014375084624F3770B5B7B79B(/*hidden argument*/NULL);
		__this->set_m_Transform_6(L_1);
		__this->set_m_Flags_8(1);
		__this->set_m_ColliderType_9(1);
		TileBase__ctor_m24A60EEED10D8C5667B24C8FB486087662417720(__this, /*hidden argument*/NULL);
		return;
	}
}
// UnityEngine.Sprite UnityEngine.Tilemaps.Tile::get_sprite()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * Tile_get_sprite_m3D0A9B402A6296694D1448B7D54EA47F46F42D95 (Tile_t275C7CE9C854F2912C851F345CCC00C45EDDE7AE * __this, const RuntimeMethod* method)
{
	Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * V_0 = NULL;
	{
		Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * L_0 = __this->get_m_Sprite_4();
		V_0 = L_0;
		goto IL_000d;
	}

IL_000d:
	{
		Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * L_1 = V_0;
		return L_1;
	}
}
// System.Void UnityEngine.Tilemaps.Tile::set_sprite(UnityEngine.Sprite)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Tile_set_sprite_m3572C470BBE7214895DAD09702F4D52C178D1BC2 (Tile_t275C7CE9C854F2912C851F345CCC00C45EDDE7AE * __this, Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * ___value0, const RuntimeMethod* method)
{
	{
		Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * L_0 = ___value0;
		__this->set_m_Sprite_4(L_0);
		return;
	}
}
// UnityEngine.Color UnityEngine.Tilemaps.Tile::get_color()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  Tile_get_color_m347AC0D960A944E3B67E55EA28C7421297CE78DC (Tile_t275C7CE9C854F2912C851F345CCC00C45EDDE7AE * __this, const RuntimeMethod* method)
{
	Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  L_0 = __this->get_m_Color_5();
		V_0 = L_0;
		goto IL_000d;
	}

IL_000d:
	{
		Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  L_1 = V_0;
		return L_1;
	}
}
// System.Void UnityEngine.Tilemaps.Tile::set_color(UnityEngine.Color)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Tile_set_color_m7EBCE0611ADD33AE67B6F8077D515029EE4E3066 (Tile_t275C7CE9C854F2912C851F345CCC00C45EDDE7AE * __this, Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  ___value0, const RuntimeMethod* method)
{
	{
		Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  L_0 = ___value0;
		__this->set_m_Color_5(L_0);
		return;
	}
}
// UnityEngine.Matrix4x4 UnityEngine.Tilemaps.Tile::get_transform()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  Tile_get_transform_m2E9EB31510CA7CC58F7ECD4B8540E8755F87DAF5 (Tile_t275C7CE9C854F2912C851F345CCC00C45EDDE7AE * __this, const RuntimeMethod* method)
{
	Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  L_0 = __this->get_m_Transform_6();
		V_0 = L_0;
		goto IL_000d;
	}

IL_000d:
	{
		Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  L_1 = V_0;
		return L_1;
	}
}
// System.Void UnityEngine.Tilemaps.Tile::set_transform(UnityEngine.Matrix4x4)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Tile_set_transform_m7216085293F3BB15A279B766F59D7EE6DAE4B552 (Tile_t275C7CE9C854F2912C851F345CCC00C45EDDE7AE * __this, Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  ___value0, const RuntimeMethod* method)
{
	{
		Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  L_0 = ___value0;
		__this->set_m_Transform_6(L_0);
		return;
	}
}
// UnityEngine.GameObject UnityEngine.Tilemaps.Tile::get_gameObject()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * Tile_get_gameObject_m854EDF4B180447489889C5522D618221BE40F89B (Tile_t275C7CE9C854F2912C851F345CCC00C45EDDE7AE * __this, const RuntimeMethod* method)
{
	GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * V_0 = NULL;
	{
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_0 = __this->get_m_InstancedGameObject_7();
		V_0 = L_0;
		goto IL_000d;
	}

IL_000d:
	{
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_1 = V_0;
		return L_1;
	}
}
// System.Void UnityEngine.Tilemaps.Tile::set_gameObject(UnityEngine.GameObject)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Tile_set_gameObject_m97619192006CCECC0FFB3748EDE45D736F631390 (Tile_t275C7CE9C854F2912C851F345CCC00C45EDDE7AE * __this, GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * ___value0, const RuntimeMethod* method)
{
	{
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_0 = ___value0;
		__this->set_m_InstancedGameObject_7(L_0);
		return;
	}
}
// UnityEngine.Tilemaps.TileFlags UnityEngine.Tilemaps.Tile::get_flags()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Tile_get_flags_mE8AA55ED6BBB851E1ECCEC7182EAD676EEA55630 (Tile_t275C7CE9C854F2912C851F345CCC00C45EDDE7AE * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		int32_t L_0 = __this->get_m_Flags_8();
		V_0 = L_0;
		goto IL_000d;
	}

IL_000d:
	{
		int32_t L_1 = V_0;
		return L_1;
	}
}
// System.Void UnityEngine.Tilemaps.Tile::set_flags(UnityEngine.Tilemaps.TileFlags)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Tile_set_flags_mAD725C9CD9928F20D5C86BC1F3311769A0F3A517 (Tile_t275C7CE9C854F2912C851F345CCC00C45EDDE7AE * __this, int32_t ___value0, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___value0;
		__this->set_m_Flags_8(L_0);
		return;
	}
}
// UnityEngine.Tilemaps.Tile_ColliderType UnityEngine.Tilemaps.Tile::get_colliderType()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Tile_get_colliderType_m77DF59033D6AA8045E6BFBEDC420FA49DA369531 (Tile_t275C7CE9C854F2912C851F345CCC00C45EDDE7AE * __this, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
	{
		int32_t L_0 = __this->get_m_ColliderType_9();
		V_0 = L_0;
		goto IL_000d;
	}

IL_000d:
	{
		int32_t L_1 = V_0;
		return L_1;
	}
}
// System.Void UnityEngine.Tilemaps.Tile::set_colliderType(UnityEngine.Tilemaps.Tile_ColliderType)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Tile_set_colliderType_mEE9420C62F7500856CB48EA6717072C85326B852 (Tile_t275C7CE9C854F2912C851F345CCC00C45EDDE7AE * __this, int32_t ___value0, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___value0;
		__this->set_m_ColliderType_9(L_0);
		return;
	}
}
// System.Void UnityEngine.Tilemaps.Tile::GetTileData(UnityEngine.Vector3Int,UnityEngine.Tilemaps.ITilemap,UnityEngine.Tilemaps.TileData&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Tile_GetTileData_m370E5271B8270137DB62EF7890FA4AA7C7C71541 (Tile_t275C7CE9C854F2912C851F345CCC00C45EDDE7AE * __this, Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___position0, ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 * ___tilemap1, TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * ___tileData2, const RuntimeMethod* method)
{
	{
		TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * L_0 = ___tileData2;
		Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * L_1 = __this->get_m_Sprite_4();
		TileData_set_sprite_m8E47803AAA5DA51504EF24A57EC2432D53B9CCB0((TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 *)L_0, L_1, /*hidden argument*/NULL);
		TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * L_2 = ___tileData2;
		Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  L_3 = __this->get_m_Color_5();
		TileData_set_color_m518FDC6D6A2287BB1C69BA55FE8C02977A55EA4C((TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 *)L_2, L_3, /*hidden argument*/NULL);
		TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * L_4 = ___tileData2;
		Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  L_5 = __this->get_m_Transform_6();
		TileData_set_transform_m4F98BB7795C0837AE119FE7C629E14238184422A((TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 *)L_4, L_5, /*hidden argument*/NULL);
		TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * L_6 = ___tileData2;
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_7 = __this->get_m_InstancedGameObject_7();
		TileData_set_gameObject_m1E8E839859E84C9E578E8C5F23CD343BD7933C49((TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 *)L_6, L_7, /*hidden argument*/NULL);
		TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * L_8 = ___tileData2;
		int32_t L_9 = __this->get_m_Flags_8();
		TileData_set_flags_m7817AC2D2183D25550281C3F868DAD2D4BCDB425((TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 *)L_8, L_9, /*hidden argument*/NULL);
		TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * L_10 = ___tileData2;
		int32_t L_11 = __this->get_m_ColliderType_9();
		TileData_set_colliderType_mC23D9C79557D9C0D41F507D4F8B427B843E5C11B((TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 *)L_10, L_11, /*hidden argument*/NULL);
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
// Conversion methods for marshalling of: UnityEngine.Tilemaps.TileAnimationData
IL2CPP_EXTERN_C void TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8_marshal_pinvoke(const TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8& unmarshaled, TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8_marshaled_pinvoke& marshaled)
{
	Exception_t* ___m_AnimatedSprites_0Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'm_AnimatedSprites' of type 'TileAnimationData': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___m_AnimatedSprites_0Exception, NULL, NULL);
}
IL2CPP_EXTERN_C void TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8_marshal_pinvoke_back(const TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8_marshaled_pinvoke& marshaled, TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8& unmarshaled)
{
	Exception_t* ___m_AnimatedSprites_0Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'm_AnimatedSprites' of type 'TileAnimationData': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___m_AnimatedSprites_0Exception, NULL, NULL);
}
// Conversion method for clean up from marshalling of: UnityEngine.Tilemaps.TileAnimationData
IL2CPP_EXTERN_C void TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8_marshal_pinvoke_cleanup(TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8_marshaled_pinvoke& marshaled)
{
}
// Conversion methods for marshalling of: UnityEngine.Tilemaps.TileAnimationData
IL2CPP_EXTERN_C void TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8_marshal_com(const TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8& unmarshaled, TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8_marshaled_com& marshaled)
{
	Exception_t* ___m_AnimatedSprites_0Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'm_AnimatedSprites' of type 'TileAnimationData': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___m_AnimatedSprites_0Exception, NULL, NULL);
}
IL2CPP_EXTERN_C void TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8_marshal_com_back(const TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8_marshaled_com& marshaled, TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8& unmarshaled)
{
	Exception_t* ___m_AnimatedSprites_0Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'm_AnimatedSprites' of type 'TileAnimationData': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___m_AnimatedSprites_0Exception, NULL, NULL);
}
// Conversion method for clean up from marshalling of: UnityEngine.Tilemaps.TileAnimationData
IL2CPP_EXTERN_C void TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8_marshal_com_cleanup(TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8_marshaled_com& marshaled)
{
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void UnityEngine.Tilemaps.TileBase::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TileBase__ctor_m24A60EEED10D8C5667B24C8FB486087662417720 (TileBase_tD2158024AAA28EB0EC62F253FA1D1A76BC50F85E * __this, const RuntimeMethod* method)
{
	{
		ScriptableObject__ctor_m6E2B3821A4A361556FC12E9B1C71E1D5DC002C5B(__this, /*hidden argument*/NULL);
		return;
	}
}
// System.Void UnityEngine.Tilemaps.TileBase::RefreshTile(UnityEngine.Vector3Int,UnityEngine.Tilemaps.ITilemap)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TileBase_RefreshTile_m3094382B655073ED6F59FAC838C6BACEE05BE131 (TileBase_tD2158024AAA28EB0EC62F253FA1D1A76BC50F85E * __this, Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___position0, ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 * ___tilemap1, const RuntimeMethod* method)
{
	{
		ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 * L_0 = ___tilemap1;
		Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  L_1 = ___position0;
		NullCheck(L_0);
		ITilemap_RefreshTile_mA824B41C2B9791C91FC5FB3AA93DDD478713564C(L_0, L_1, /*hidden argument*/NULL);
		return;
	}
}
// System.Void UnityEngine.Tilemaps.TileBase::GetTileData(UnityEngine.Vector3Int,UnityEngine.Tilemaps.ITilemap,UnityEngine.Tilemaps.TileData&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TileBase_GetTileData_m3A1CE78996D8498F5EEB3FE0F16BF82BB903D331 (TileBase_tD2158024AAA28EB0EC62F253FA1D1A76BC50F85E * __this, Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___position0, ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 * ___tilemap1, TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * ___tileData2, const RuntimeMethod* method)
{
	{
		return;
	}
}
// UnityEngine.Tilemaps.TileData UnityEngine.Tilemaps.TileBase::GetTileDataNoRef(UnityEngine.Vector3Int,UnityEngine.Tilemaps.ITilemap)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5  TileBase_GetTileDataNoRef_m669665FCC7CE24E81D803944D1F319DDC72B889F (TileBase_tD2158024AAA28EB0EC62F253FA1D1A76BC50F85E * __this, Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___position0, ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 * ___tilemap1, const RuntimeMethod* method)
{
	TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5  V_0;
	memset((&V_0), 0, sizeof(V_0));
	TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5  V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		il2cpp_codegen_initobj((&V_0), sizeof(TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 ));
		Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  L_0 = ___position0;
		ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 * L_1 = ___tilemap1;
		VirtActionInvoker3< Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 , ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 *, TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * >::Invoke(5 /* System.Void UnityEngine.Tilemaps.TileBase::GetTileData(UnityEngine.Vector3Int,UnityEngine.Tilemaps.ITilemap,UnityEngine.Tilemaps.TileData&) */, __this, L_0, L_1, (TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 *)(&V_0));
		TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5  L_2 = V_0;
		V_1 = L_2;
		goto IL_001a;
	}

IL_001a:
	{
		TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5  L_3 = V_1;
		return L_3;
	}
}
// System.Boolean UnityEngine.Tilemaps.TileBase::GetTileAnimationData(UnityEngine.Vector3Int,UnityEngine.Tilemaps.ITilemap,UnityEngine.Tilemaps.TileAnimationData&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool TileBase_GetTileAnimationData_m674999CD0CD7208F960DDC3B618CD95D5CE2626F (TileBase_tD2158024AAA28EB0EC62F253FA1D1A76BC50F85E * __this, Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___position0, ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 * ___tilemap1, TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8 * ___tileAnimationData2, const RuntimeMethod* method)
{
	bool V_0 = false;
	{
		V_0 = (bool)0;
		goto IL_0008;
	}

IL_0008:
	{
		bool L_0 = V_0;
		return L_0;
	}
}
// UnityEngine.Tilemaps.TileAnimationData UnityEngine.Tilemaps.TileBase::GetTileAnimationDataNoRef(UnityEngine.Vector3Int,UnityEngine.Tilemaps.ITilemap)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8  TileBase_GetTileAnimationDataNoRef_mE10FEDA60DC93F3282482EB66BAE99236CACFBB3 (TileBase_tD2158024AAA28EB0EC62F253FA1D1A76BC50F85E * __this, Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___position0, ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 * ___tilemap1, const RuntimeMethod* method)
{
	TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8  V_0;
	memset((&V_0), 0, sizeof(V_0));
	TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8  V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		il2cpp_codegen_initobj((&V_0), sizeof(TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8 ));
		Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  L_0 = ___position0;
		ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 * L_1 = ___tilemap1;
		VirtFuncInvoker3< bool, Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 , ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 *, TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8 * >::Invoke(6 /* System.Boolean UnityEngine.Tilemaps.TileBase::GetTileAnimationData(UnityEngine.Vector3Int,UnityEngine.Tilemaps.ITilemap,UnityEngine.Tilemaps.TileAnimationData&) */, __this, L_0, L_1, (TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8 *)(&V_0));
		TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8  L_2 = V_0;
		V_1 = L_2;
		goto IL_001b;
	}

IL_001b:
	{
		TileAnimationData_t2A9C81AD1F3E916C2DE292A6F3953FC8C38EFDA8  L_3 = V_1;
		return L_3;
	}
}
// System.Boolean UnityEngine.Tilemaps.TileBase::StartUp(UnityEngine.Vector3Int,UnityEngine.Tilemaps.ITilemap,UnityEngine.GameObject)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool TileBase_StartUp_m17522434C955AB943A4C3C0AA0C701BBE39757E2 (TileBase_tD2158024AAA28EB0EC62F253FA1D1A76BC50F85E * __this, Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___position0, ITilemap_t784A442A8BD2283058F44E8C0FE5257168459BE3 * ___tilemap1, GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * ___go2, const RuntimeMethod* method)
{
	bool V_0 = false;
	{
		V_0 = (bool)0;
		goto IL_0008;
	}

IL_0008:
	{
		bool L_0 = V_0;
		return L_0;
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
// Conversion methods for marshalling of: UnityEngine.Tilemaps.TileData
IL2CPP_EXTERN_C void TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5_marshal_pinvoke(const TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5& unmarshaled, TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5_marshaled_pinvoke& marshaled)
{
	Exception_t* ___m_Sprite_0Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'm_Sprite' of type 'TileData': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___m_Sprite_0Exception, NULL, NULL);
}
IL2CPP_EXTERN_C void TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5_marshal_pinvoke_back(const TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5_marshaled_pinvoke& marshaled, TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5& unmarshaled)
{
	Exception_t* ___m_Sprite_0Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'm_Sprite' of type 'TileData': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___m_Sprite_0Exception, NULL, NULL);
}
// Conversion method for clean up from marshalling of: UnityEngine.Tilemaps.TileData
IL2CPP_EXTERN_C void TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5_marshal_pinvoke_cleanup(TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5_marshaled_pinvoke& marshaled)
{
}
// Conversion methods for marshalling of: UnityEngine.Tilemaps.TileData
IL2CPP_EXTERN_C void TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5_marshal_com(const TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5& unmarshaled, TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5_marshaled_com& marshaled)
{
	Exception_t* ___m_Sprite_0Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'm_Sprite' of type 'TileData': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___m_Sprite_0Exception, NULL, NULL);
}
IL2CPP_EXTERN_C void TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5_marshal_com_back(const TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5_marshaled_com& marshaled, TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5& unmarshaled)
{
	Exception_t* ___m_Sprite_0Exception = il2cpp_codegen_get_marshal_directive_exception("Cannot marshal field 'm_Sprite' of type 'TileData': Reference type field marshaling is not supported.");
	IL2CPP_RAISE_MANAGED_EXCEPTION(___m_Sprite_0Exception, NULL, NULL);
}
// Conversion method for clean up from marshalling of: UnityEngine.Tilemaps.TileData
IL2CPP_EXTERN_C void TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5_marshal_com_cleanup(TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5_marshaled_com& marshaled)
{
}
// System.Void UnityEngine.Tilemaps.TileData::set_sprite(UnityEngine.Sprite)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TileData_set_sprite_m8E47803AAA5DA51504EF24A57EC2432D53B9CCB0 (TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * __this, Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * ___value0, const RuntimeMethod* method)
{
	{
		Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * L_0 = ___value0;
		__this->set_m_Sprite_0(L_0);
		return;
	}
}
IL2CPP_EXTERN_C  void TileData_set_sprite_m8E47803AAA5DA51504EF24A57EC2432D53B9CCB0_AdjustorThunk (RuntimeObject * __this, Sprite_tCA09498D612D08DE668653AF1E9C12BF53434198 * ___value0, const RuntimeMethod* method)
{
	TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * _thisAdjusted = reinterpret_cast<TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 *>(__this + 1);
	TileData_set_sprite_m8E47803AAA5DA51504EF24A57EC2432D53B9CCB0(_thisAdjusted, ___value0, method);
}
// System.Void UnityEngine.Tilemaps.TileData::set_color(UnityEngine.Color)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TileData_set_color_m518FDC6D6A2287BB1C69BA55FE8C02977A55EA4C (TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * __this, Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  ___value0, const RuntimeMethod* method)
{
	{
		Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  L_0 = ___value0;
		__this->set_m_Color_1(L_0);
		return;
	}
}
IL2CPP_EXTERN_C  void TileData_set_color_m518FDC6D6A2287BB1C69BA55FE8C02977A55EA4C_AdjustorThunk (RuntimeObject * __this, Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  ___value0, const RuntimeMethod* method)
{
	TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * _thisAdjusted = reinterpret_cast<TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 *>(__this + 1);
	TileData_set_color_m518FDC6D6A2287BB1C69BA55FE8C02977A55EA4C(_thisAdjusted, ___value0, method);
}
// System.Void UnityEngine.Tilemaps.TileData::set_transform(UnityEngine.Matrix4x4)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TileData_set_transform_m4F98BB7795C0837AE119FE7C629E14238184422A (TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * __this, Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  ___value0, const RuntimeMethod* method)
{
	{
		Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  L_0 = ___value0;
		__this->set_m_Transform_2(L_0);
		return;
	}
}
IL2CPP_EXTERN_C  void TileData_set_transform_m4F98BB7795C0837AE119FE7C629E14238184422A_AdjustorThunk (RuntimeObject * __this, Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  ___value0, const RuntimeMethod* method)
{
	TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * _thisAdjusted = reinterpret_cast<TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 *>(__this + 1);
	TileData_set_transform_m4F98BB7795C0837AE119FE7C629E14238184422A(_thisAdjusted, ___value0, method);
}
// System.Void UnityEngine.Tilemaps.TileData::set_gameObject(UnityEngine.GameObject)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TileData_set_gameObject_m1E8E839859E84C9E578E8C5F23CD343BD7933C49 (TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * __this, GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * ___value0, const RuntimeMethod* method)
{
	{
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_0 = ___value0;
		__this->set_m_GameObject_3(L_0);
		return;
	}
}
IL2CPP_EXTERN_C  void TileData_set_gameObject_m1E8E839859E84C9E578E8C5F23CD343BD7933C49_AdjustorThunk (RuntimeObject * __this, GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * ___value0, const RuntimeMethod* method)
{
	TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * _thisAdjusted = reinterpret_cast<TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 *>(__this + 1);
	TileData_set_gameObject_m1E8E839859E84C9E578E8C5F23CD343BD7933C49(_thisAdjusted, ___value0, method);
}
// System.Void UnityEngine.Tilemaps.TileData::set_flags(UnityEngine.Tilemaps.TileFlags)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TileData_set_flags_m7817AC2D2183D25550281C3F868DAD2D4BCDB425 (TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * __this, int32_t ___value0, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___value0;
		__this->set_m_Flags_4(L_0);
		return;
	}
}
IL2CPP_EXTERN_C  void TileData_set_flags_m7817AC2D2183D25550281C3F868DAD2D4BCDB425_AdjustorThunk (RuntimeObject * __this, int32_t ___value0, const RuntimeMethod* method)
{
	TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * _thisAdjusted = reinterpret_cast<TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 *>(__this + 1);
	TileData_set_flags_m7817AC2D2183D25550281C3F868DAD2D4BCDB425(_thisAdjusted, ___value0, method);
}
// System.Void UnityEngine.Tilemaps.TileData::set_colliderType(UnityEngine.Tilemaps.Tile_ColliderType)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TileData_set_colliderType_mC23D9C79557D9C0D41F507D4F8B427B843E5C11B (TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * __this, int32_t ___value0, const RuntimeMethod* method)
{
	{
		int32_t L_0 = ___value0;
		__this->set_m_ColliderType_5(L_0);
		return;
	}
}
IL2CPP_EXTERN_C  void TileData_set_colliderType_mC23D9C79557D9C0D41F507D4F8B427B843E5C11B_AdjustorThunk (RuntimeObject * __this, int32_t ___value0, const RuntimeMethod* method)
{
	TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 * _thisAdjusted = reinterpret_cast<TileData_t8A50A35CAFD87C12E27D7E596D968C9114A4CBB5 *>(__this + 1);
	TileData_set_colliderType_mC23D9C79557D9C0D41F507D4F8B427B843E5C11B(_thisAdjusted, ___value0, method);
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
// System.Void UnityEngine.Tilemaps.Tilemap::RefreshTile(UnityEngine.Vector3Int)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Tilemap_RefreshTile_m9F32ACD729A4C9D843662E956B38BB91351E01EA (Tilemap_t0F92148668211805A631B93488D4A629EC378B10 * __this, Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4  ___position0, const RuntimeMethod* method)
{
	{
		Tilemap_RefreshTile_Injected_m71B01A4DDE057FE002AE5BEC5ACB38F04BC64792(__this, (Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 *)(&___position0), /*hidden argument*/NULL);
		return;
	}
}
// System.Void UnityEngine.Tilemaps.Tilemap::RefreshTile_Injected(UnityEngine.Vector3Int&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Tilemap_RefreshTile_Injected_m71B01A4DDE057FE002AE5BEC5ACB38F04BC64792 (Tilemap_t0F92148668211805A631B93488D4A629EC378B10 * __this, Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 * ___position0, const RuntimeMethod* method)
{
	typedef void (*Tilemap_RefreshTile_Injected_m71B01A4DDE057FE002AE5BEC5ACB38F04BC64792_ftn) (Tilemap_t0F92148668211805A631B93488D4A629EC378B10 *, Vector3Int_tA843C5F8C2EB42492786C5AF82C3E1F4929942B4 *);
	static Tilemap_RefreshTile_Injected_m71B01A4DDE057FE002AE5BEC5ACB38F04BC64792_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (Tilemap_RefreshTile_Injected_m71B01A4DDE057FE002AE5BEC5ACB38F04BC64792_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.Tilemaps.Tilemap::RefreshTile_Injected(UnityEngine.Vector3Int&)");
	_il2cpp_icall_func(__this, ___position0);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
