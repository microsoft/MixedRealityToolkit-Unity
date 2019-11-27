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
template <typename R, typename T1, typename T2>
struct VirtFuncInvoker2
{
	typedef R (*Func)(void*, T1, T2, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
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
template <typename R, typename T1>
struct VirtFuncInvoker1
{
	typedef R (*Func)(void*, T1, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		return ((Func)invokeData.methodPtr)(obj, p1, invokeData.method);
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
struct InterfaceActionInvoker0
{
	typedef void (*Action)(void*, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, invokeData.method);
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
// System.Boolean[]
struct BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040;
// System.Byte[]
struct ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821;
// System.Char[]
struct CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2;
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
// System.Double[]
struct DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D;
// System.Exception
struct Exception_t;
// System.IAsyncResult
struct IAsyncResult_t8E194308510B375B42432981AE5E7488C458D598;
// System.Int16[]
struct Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28;
// System.Int32[]
struct Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83;
// System.Int64[]
struct Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F;
// System.IntPtr[]
struct IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD;
// System.Object[]
struct ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A;
// System.Reflection.Binder
struct Binder_t4D5CB06963501D32847C057B57157D6DC49CA759;
// System.Reflection.MemberFilter
struct MemberFilter_t25C1BD92C42BE94426E300787C13C452CB89B381;
// System.Reflection.MethodBase
struct MethodBase_t;
// System.Reflection.MethodInfo
struct MethodInfo_t;
// System.Reflection.ParameterModifier[]
struct ParameterModifierU5BU5D_t63EC46F14F048DC9EF6BF1362E8AEBEA1A05A5EA;
// System.Reflection.TargetInvocationException
struct TargetInvocationException_t0DD35F6083E1D1E0509BF181A79C76D3339D89B8;
// System.Runtime.Serialization.SafeSerializationManager
struct SafeSerializationManager_t4A754D86B0F784B18CBC36C073BA564BED109770;
// System.SByte[]
struct SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889;
// System.Single[]
struct SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5;
// System.String
struct String_t;
// System.String[]
struct StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E;
// System.Text.StringBuilder
struct StringBuilder_t;
// System.Type
struct Type_t;
// System.Type[]
struct TypeU5BU5D_t7FE623A666B49176DE123306221193E888A12F5F;
// System.Void
struct Void_t22962CB4C05B1D89B55A6E1139F0E87A90987017;
// UnityEngine.AndroidJavaClass
struct AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE;
// UnityEngine.AndroidJavaException
struct AndroidJavaException_tC81E6FAAA4067CBA537727328D5D2DB14F5F5466;
// UnityEngine.AndroidJavaObject
struct AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D;
// UnityEngine.AndroidJavaObject[]
struct AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379;
// UnityEngine.AndroidJavaProxy
struct AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D;
// UnityEngine.AndroidJavaRunnable
struct AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02;
// UnityEngine.AndroidJavaRunnableProxy
struct AndroidJavaRunnableProxy_t3C66FEA8C2A903168F2902788AB8AB29CA923308;
// UnityEngine.GlobalJavaObjectRef
struct GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0;
// UnityEngine.jvalue[]
struct jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3;

IL2CPP_EXTERN_C RuntimeClass* AndroidJavaClassU5BU5D_t834F2CD8A8D5B7F304A36C401A57C24A723690C7_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* AndroidJavaException_tC81E6FAAA4067CBA537727328D5D2DB14F5F5466_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* AndroidJavaRunnableProxy_t3C66FEA8C2A903168F2902788AB8AB29CA923308_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Boolean_tB53F6830F670160873277339AA58F15CAED4399C_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Byte_tF87C579059BD4633E6840EBBBEEF899C6E33EF07_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Char_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Double_t358B8F23BDC52A5DD700E727E204F9F7CDE12409_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Exception_t_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* GC_tC1D7BD74E8F44ECCEF5CD2B5D84BFF9AAE02D01D_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IDisposable_t7218B22548186B208D65EA5B7870503810A2D15A_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Int16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Int64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IntPtr_t_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* RuntimeArray_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* RuntimeObject_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SByte_t9070AEA2966184235653CB9B4D33B149CDA831DF_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Single_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* StringBuilder_t_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* String_t_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* TargetInvocationException_t0DD35F6083E1D1E0509BF181A79C76D3339D89B8_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* TypeU5BU5D_t7FE623A666B49176DE123306221193E888A12F5F_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Type_t_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C String_t* _stringLiteral02AA629C8B16CD17A44F3A0EFEC2FEED43937642;
IL2CPP_EXTERN_C String_t* _stringLiteral04B84BF4B23F7D8F289DDA3DFB2F69943E579890;
IL2CPP_EXTERN_C String_t* _stringLiteral093986A34E3A8209AD12EC05E8E02A27BA4A9B4F;
IL2CPP_EXTERN_C String_t* _stringLiteral0EEE3EFAE974B24F801BA15D9AEC6ED2340751D1;
IL2CPP_EXTERN_C String_t* _stringLiteral169775A78ADEE2D403BC1F88A1C1760F11C0304D;
IL2CPP_EXTERN_C String_t* _stringLiteral1C0FE9678C38548F6401871A634364E47FE3612C;
IL2CPP_EXTERN_C String_t* _stringLiteral1C138278299F1B35865A79651A05DF52C0D74BB9;
IL2CPP_EXTERN_C String_t* _stringLiteral1D6BCB22DD39DE1A757738A79C87BE5519B16FDE;
IL2CPP_EXTERN_C String_t* _stringLiteral1D85A749A5C6FB273395A49AF6A07D9CF0C26A6D;
IL2CPP_EXTERN_C String_t* _stringLiteral1EB9481D15B52FEE57E5AC17CA684460A95993E5;
IL2CPP_EXTERN_C String_t* _stringLiteral1F3F048FA394EC532DD74D6A71C33F30C7F7C70C;
IL2CPP_EXTERN_C String_t* _stringLiteral1F96368984CDA7947E8A4265D0BD7C61AC6EB290;
IL2CPP_EXTERN_C String_t* _stringLiteral24D34050C2CD7CAF4904B270C05866CE090A90D7;
IL2CPP_EXTERN_C String_t* _stringLiteral28ED3A797DA3C48C309A4EF792147F3C56CFEC40;
IL2CPP_EXTERN_C String_t* _stringLiteral2A42CDB77001E84D751FD683B088BBF833EEE0B3;
IL2CPP_EXTERN_C String_t* _stringLiteral2D14AB97CC3DC294C51C0D6814F4EA45F4B4E312;
IL2CPP_EXTERN_C String_t* _stringLiteral319523E1002BB1B6AB63E268BEEA610E6C9D8EEC;
IL2CPP_EXTERN_C String_t* _stringLiteral32096C2E0EFF33D844EE6D675407ACE18289357D;
IL2CPP_EXTERN_C String_t* _stringLiteral327C3BC0993A6F3EF91265DAF24D8D1A4076818E;
IL2CPP_EXTERN_C String_t* _stringLiteral328CD2BEF0C16A2D306B28CD73848671CCC42AC2;
IL2CPP_EXTERN_C String_t* _stringLiteral3A52CE780950D4D969792A2559CD519D7EE8C727;
IL2CPP_EXTERN_C String_t* _stringLiteral3AD95ED806CF8C0E154B9D74C27CBE73D5918CFB;
IL2CPP_EXTERN_C String_t* _stringLiteral3E2B500817A96FA5BAECB12EAFF42205003D74E6;
IL2CPP_EXTERN_C String_t* _stringLiteral40AB7562969E4FED5261D3FBFA38AEA9397716D4;
IL2CPP_EXTERN_C String_t* _stringLiteral46F8AB7C0CFF9DF7CD124852E26022A6BF89E315;
IL2CPP_EXTERN_C String_t* _stringLiteral48647474B89FA8F56ED6BDA0F8148A17B51B97BD;
IL2CPP_EXTERN_C String_t* _stringLiteral4E156FB692586759BECC946B6A67CEC836B61DDA;
IL2CPP_EXTERN_C String_t* _stringLiteral4F05CBFCA4DFE76B99B142F609CDCF00D44FA247;
IL2CPP_EXTERN_C String_t* _stringLiteral50C9E8D5FC98727B4BBC93CF5D64A68DB647F04F;
IL2CPP_EXTERN_C String_t* _stringLiteral51750D533BBD6F70990AA487A711B4492A08F4BC;
IL2CPP_EXTERN_C String_t* _stringLiteral58668E7669FD564D99DB5D581FCDB6A5618440B5;
IL2CPP_EXTERN_C String_t* _stringLiteral59CA046CC86D6DAAA8DF1A535C94F9AD1834F7FD;
IL2CPP_EXTERN_C String_t* _stringLiteral5ABBEC2FB4C72453E6720E8AA22C1978B547A438;
IL2CPP_EXTERN_C String_t* _stringLiteral5BD9EA45F0B419AD93E447295BC0AA4D644CF1B4;
IL2CPP_EXTERN_C String_t* _stringLiteral5C10B5B2CD673A0616D529AA5234B12EE7153808;
IL2CPP_EXTERN_C String_t* _stringLiteral6226D4F452A659360FAAC6A6266D73ABC5BFC1FC;
IL2CPP_EXTERN_C String_t* _stringLiteral685E80366130387CB75C055248326976D16FDF8D;
IL2CPP_EXTERN_C String_t* _stringLiteral71FAFC4E2FC1E47E234762A96B80512B6B5534C2;
IL2CPP_EXTERN_C String_t* _stringLiteral760AB7ED1FC73BD5C47398584B149380AB0582EA;
IL2CPP_EXTERN_C String_t* _stringLiteral783923E57BA5E8F1044632C31FD806EE24814BB5;
IL2CPP_EXTERN_C String_t* _stringLiteral84B35CD832E694499CB991F7B38517E07CFC129A;
IL2CPP_EXTERN_C String_t* _stringLiteral8777D1BEFDBAE64EDD9D49FE596B0CC904692081;
IL2CPP_EXTERN_C String_t* _stringLiteral8CF1783FA99F62CA581F6FE8F3CD66B0F9AB9FC3;
IL2CPP_EXTERN_C String_t* _stringLiteral90481F4868A2DA6C9F737FD69686A6CE240E7093;
IL2CPP_EXTERN_C String_t* _stringLiteral909F99A779ADB66A76FC53AB56C7DD1CAF35D0FD;
IL2CPP_EXTERN_C String_t* _stringLiteral917103D252076DA908A549A26BE33C64ABBD0EAC;
IL2CPP_EXTERN_C String_t* _stringLiteral94A9D9512BA3D2F295C65A0B3119715C79E6CB75;
IL2CPP_EXTERN_C String_t* _stringLiteral9ACC977EC5E796EA5E374A6E64654E0222D407E2;
IL2CPP_EXTERN_C String_t* _stringLiteral9B23229A318A455BC6A6E6317B4E72BC31248A5A;
IL2CPP_EXTERN_C String_t* _stringLiteral9E753E685FCDC6208CD59CF2FF3FDCCEB33023DD;
IL2CPP_EXTERN_C String_t* _stringLiteral9EAF6B54917BA48016AC5209BC15F62D5445708E;
IL2CPP_EXTERN_C String_t* _stringLiteralA0BCA4B2E667DD10532EED8280DA58E7BE1A8B88;
IL2CPP_EXTERN_C String_t* _stringLiteralA0F4EA7D91495DF92BBAC2E2149DFB850FE81396;
IL2CPP_EXTERN_C String_t* _stringLiteralA4527A64BEC3DBF0AC08CE1BF9ABC796E5E364E9;
IL2CPP_EXTERN_C String_t* _stringLiteralA515F49F0F4C724D096B5DA7E31DFBB14FC018AC;
IL2CPP_EXTERN_C String_t* _stringLiteralA7EDC6086A91C13EEC0568F09CD6263D5A4CFFEC;
IL2CPP_EXTERN_C String_t* _stringLiteralAA3B42B3BA69D14FA1DA94B7DD8016010E8F6E0C;
IL2CPP_EXTERN_C String_t* _stringLiteralAE4F281DF5A5D0FF3CAD6371F76D5C29B6D953EC;
IL2CPP_EXTERN_C String_t* _stringLiteralB305182D3386ABDBE950B08CD45E46CB3C5E3D6F;
IL2CPP_EXTERN_C String_t* _stringLiteralB402D9DB865836815F1609AD99C0C12FA3DD8026;
IL2CPP_EXTERN_C String_t* _stringLiteralB505B482020D33F0BA0DA1BE632CEF3BC4E82948;
IL2CPP_EXTERN_C String_t* _stringLiteralB663A44D6624D0F6014A6C18E4A7FE0F5BED8FB9;
IL2CPP_EXTERN_C String_t* _stringLiteralB9922C62B93E7253F36B55E613AA39448D564BB9;
IL2CPP_EXTERN_C String_t* _stringLiteralBB589D0621E5472F470FA3425A234C74B1E202E8;
IL2CPP_EXTERN_C String_t* _stringLiteralBC3866F48E90715EBFCDCFC327E4131E3BC40FB1;
IL2CPP_EXTERN_C String_t* _stringLiteralBD119F910FA08AD4078969E4A551A13A7EA4D4BC;
IL2CPP_EXTERN_C String_t* _stringLiteralBD3027FA569EA15CA76D84DB21C67E2D514C1A5A;
IL2CPP_EXTERN_C String_t* _stringLiteralBD7E4A941C870AD23894466BB52628A9B488A1A2;
IL2CPP_EXTERN_C String_t* _stringLiteralBDB36BB22DEB169275B3094BA9005A29EEDDD195;
IL2CPP_EXTERN_C String_t* _stringLiteralC3BEC6BCBC9B9F04E60FCB1D9C9C1A37F3E12E93;
IL2CPP_EXTERN_C String_t* _stringLiteralC5299E0BD811F82870EFBBB00341CCDF263E242E;
IL2CPP_EXTERN_C String_t* _stringLiteralC9291E1B62F25E545BD2AC4DF55EB10099666DCD;
IL2CPP_EXTERN_C String_t* _stringLiteralCA73AB65568CD125C2D27A22BBD9E863C10B675D;
IL2CPP_EXTERN_C String_t* _stringLiteralD15A929AAC58DB1B939AAB2AEDA4342595D77F13;
IL2CPP_EXTERN_C String_t* _stringLiteralD160E0986ACA4714714A16F29EC605AF90BE704D;
IL2CPP_EXTERN_C String_t* _stringLiteralD53437218F50447640E8F502D360C117BA0C456F;
IL2CPP_EXTERN_C String_t* _stringLiteralD98507F786B7E8AA37C8E9EE1D0452E55E21A08D;
IL2CPP_EXTERN_C String_t* _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709;
IL2CPP_EXTERN_C String_t* _stringLiteralDCB8EAD2EF149C5622DFF6CF2BD5628BDC485847;
IL2CPP_EXTERN_C String_t* _stringLiteralDE5E596326DC7F422D1D5BFA854AA400BA53AE86;
IL2CPP_EXTERN_C String_t* _stringLiteralE2F1487C5A036945AF1009212074D1B8984E2994;
IL2CPP_EXTERN_C String_t* _stringLiteralE33E0E502D09092EA117BE8C27FB58B1DD3AA609;
IL2CPP_EXTERN_C String_t* _stringLiteralE46270F492F404A1D912A23E4DE44F3C7840F993;
IL2CPP_EXTERN_C String_t* _stringLiteralE69F20E9F683920D3FB4329ABD951E878B1F9372;
IL2CPP_EXTERN_C String_t* _stringLiteralE6A7F51D4599E77D3EE682C1208434F332D9BF8D;
IL2CPP_EXTERN_C String_t* _stringLiteralE7064F0B80F61DBC65915311032D27BAA569AE2A;
IL2CPP_EXTERN_C String_t* _stringLiteralE7CB2263F2E05D6BC8A91144ABC41E52B1F3CCFD;
IL2CPP_EXTERN_C String_t* _stringLiteralEA4172C398D2679B145B45C5FF1544AB767AA341;
IL2CPP_EXTERN_C String_t* _stringLiteralEDB1046E80D3EA42FA26944C690CF3EB80C9CC62;
IL2CPP_EXTERN_C String_t* _stringLiteralF57B2D312D9EFE8FE993C8EB1F3E19D41AD04030;
IL2CPP_EXTERN_C String_t* _stringLiteralF75D848FCD77B877799E37401451606B0778E2C5;
IL2CPP_EXTERN_C String_t* _stringLiteralF8576A7F9518C9A6463C6E1B2833EB06AE9664B4;
IL2CPP_EXTERN_C String_t* _stringLiteralFA98C1FD2CA6FC89B5ED010FD16AA461F50AFB3E;
IL2CPP_EXTERN_C String_t* _stringLiteralFB0418121C28FD390FBFDEEBF12570C86FC00B32;
IL2CPP_EXTERN_C String_t* _stringLiteralFF17B8589770AC5A0B97D99980EA610D3A07AC25;
IL2CPP_EXTERN_C const RuntimeMethod* AndroidJNIHelper_CreateJavaProxy_m29A8BD91809FF21642EA1319E5F097979EE8FA28_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* AndroidJavaClass__ctor_m44A6DEC0612D768E9947FFC1C2DA64D0605F34F1_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* AndroidJavaObject_CallStatic_TisAndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_m6CAE75FB51C5A02521C239A7232735573C51EAE7_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* AndroidJavaObject_CallStatic_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m90D39A3F3725F8BD3F782614FA0101D563DA9CCF_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* AndroidJavaObject_Call_TisAndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_m775AB90594C5F27D6099ED61119EF3608FD1001D_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* AndroidJavaObject_Call_TisBoolean_tB53F6830F670160873277339AA58F15CAED4399C_m57EE1ACB271D15DD0E2DDD6B28805C31799A0976_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* AndroidJavaObject_Call_TisChar_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9_m73C43D18BEC4AF2416AC8ADA8FA26712645A0EEA_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* AndroidJavaObject_Call_TisDouble_t358B8F23BDC52A5DD700E727E204F9F7CDE12409_mBDD67692E825B1F8834E22FC94628B9C6AE54C81_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* AndroidJavaObject_Call_TisInt16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D_mB51ADF5CFAE5278F11CECE74CC8ABAA9B45BB34F_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* AndroidJavaObject_Call_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_mF7220A3D48BA18737AA0C7DAF0828822275A69A6_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* AndroidJavaObject_Call_TisInt64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436_mCD42F5F94257CC748CBA517A16A7BCC707A0C440_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* AndroidJavaObject_Call_TisSByte_t9070AEA2966184235653CB9B4D33B149CDA831DF_m1DA87DAFADCDA8DE62A86D5C1F94DF60F2F54651_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* AndroidJavaObject_Call_TisSingle_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1_m241B6C5C3A0259B256071CA26CAFE3EF0F229DBA_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* AndroidJavaObject_Call_TisString_t_m5EAE53C9E2A8893FD8FEA710378D22C162A0FDEA_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* AndroidJavaObject__ctor_m22E1E2E5D9F3DA31FF7DFB1339AD3BB0C3813E80_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* AndroidJavaProxy_Invoke_m2A4BA59C6A517E0B692478676AA0A0A77980848E_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* _AndroidJNIHelper_Box_m67A2A786DCE5ADD2FAF4F27B7CA115C82A8768C2_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* _AndroidJNIHelper_ConvertToJNIArray_mBF20C1B6716BA00CA9C3825EA446B291E6D8EB20_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* _AndroidJNIHelper_CreateJNIArgArray_m9605B7C73D18B6A11264A61E33888374E1F283A9_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* _AndroidJNIHelper_GetConstructorID_m9A5019D80C0E776003ADFC0A54A879ECDC3B60D8_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* _AndroidJNIHelper_GetMethodID_m22C073C0BCB560A1AD9EE6158FF8314D291EF756_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* _AndroidJNIHelper_GetSignature_m090B053BFD9A6AC7BBD0F2BFAE56A8188CE4D80B_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* _AndroidJNIHelper_UnboxArray_m57E035906F4D79FCAC155162AC491BB7B575956D_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeType* AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE_0_0_0_var;
IL2CPP_EXTERN_C const RuntimeType* AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_0_0_0_var;
IL2CPP_EXTERN_C const RuntimeType* AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02_0_0_0_var;
IL2CPP_EXTERN_C const RuntimeType* Boolean_tB53F6830F670160873277339AA58F15CAED4399C_0_0_0_var;
IL2CPP_EXTERN_C const RuntimeType* Byte_tF87C579059BD4633E6840EBBBEEF899C6E33EF07_0_0_0_var;
IL2CPP_EXTERN_C const RuntimeType* Char_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9_0_0_0_var;
IL2CPP_EXTERN_C const RuntimeType* Double_t358B8F23BDC52A5DD700E727E204F9F7CDE12409_0_0_0_var;
IL2CPP_EXTERN_C const RuntimeType* Int16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D_0_0_0_var;
IL2CPP_EXTERN_C const RuntimeType* Int32_t585191389E07734F19F3156FF88FB3EF4800D102_0_0_0_var;
IL2CPP_EXTERN_C const RuntimeType* Int64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436_0_0_0_var;
IL2CPP_EXTERN_C const RuntimeType* RuntimeArray_0_0_0_var;
IL2CPP_EXTERN_C const RuntimeType* SByte_t9070AEA2966184235653CB9B4D33B149CDA831DF_0_0_0_var;
IL2CPP_EXTERN_C const RuntimeType* Single_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1_0_0_0_var;
IL2CPP_EXTERN_C const RuntimeType* String_t_0_0_0_var;
IL2CPP_EXTERN_C const uint32_t AndroidJNIHelper_CreateJavaProxy_m29A8BD91809FF21642EA1319E5F097979EE8FA28_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidJNISafe_DeleteGlobalRef_mE0C851F30E3481496C72814973B66161C486D8BA_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidJNISafe_DeleteLocalRef_m9632EA13BF03AEE43FC7713125962A4D0DFFADC7_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidJNISafe_DeleteWeakGlobalRef_mB338C2F7116360905B7F444BDB16CAB18B914ED3_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidJavaClass__AndroidJavaClass_mBF3C92E82722125793A66F20C92BAE17F0CB02D9_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidJavaClass__ctor_m44A6DEC0612D768E9947FFC1C2DA64D0605F34F1_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidJavaClass__ctor_mAE416E812DB3911279C0FE87A7760247CE1BBFA8_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidJavaException__ctor_m8E5216F0181090FB7A9016AED78B7935019791D8_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidJavaObject_AndroidJavaClassDeleteLocalRef_mD137411129D4E0B5AB858EAE367EBBA0E668D962_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidJavaObject_AndroidJavaObjectDeleteLocalRef_m0B0BCBDD56C299AC69938BDD4135E1B6EEAAC7EF_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidJavaObject_DebugPrint_m88F06202527BA5A2848C1533C8B396702D112531_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidJavaObject_Dispose_m02D1B6D8F3E902E5F0D181BF6C1753856B0DE144_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidJavaObject__AndroidJavaObject_m596F928EE49384D7C7455920BA6ADFB2D9540CFA_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidJavaObject__cctor_m46EF3B9E61C141E07E12762F96F777EA8D1A4629_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidJavaObject__ctor_m22E1E2E5D9F3DA31FF7DFB1339AD3BB0C3813E80_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidJavaProxy_GetProxyObject_m411DC59BF56152B6058ABF99BBC8B64C813EEF06_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidJavaProxy_GetRawProxy_mFE7D48E72D4744E260D3ACE6D777D072002BEA6C_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidJavaProxy_Invoke_m27ACB084BB434FFEA8A1FB687CCB332F4EB80B9B_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidJavaProxy_Invoke_m2A4BA59C6A517E0B692478676AA0A0A77980848E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidJavaProxy__cctor_mC5B6251AA25617F7CE1AD4DAD0BD2CCAC9636C9F_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidJavaProxy__ctor_m159565DEF4041D92C0763D1F4A0684140241CD9A_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidJavaProxy__ctor_m9A2D1F4BF0E7803070D68D3C386F4218D3BCAC0F_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidJavaRunnableProxy__ctor_m0D23BFCE5D99EA0AA56A5813B2E91BDDAD72C738_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidReflection_GetConstructorMember_mE78FA3844BBB2FE5A6D3A6719BE72BD33423F4C9_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidReflection_GetMethodMember_m0B7C41F91CA0414D70EDFF7853BA93B11157EB19_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidReflection_NewProxyInstance_mEE0634E1963302B17FBAED127B581BFE4D228A8C_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidReflection_SetNativeExceptionOnProxy_m025AFCDD8B6659D45FE3830E8AC154300DA19966_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t AndroidReflection__cctor_m328F9C260CA935498229C4D912C6B27618BEE8E6_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t GlobalJavaObjectRef_Dispose_mDCFD34D040E7B4ACE886336F3659316D1A45599F_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t GlobalJavaObjectRef__ctor_m5581A68DC5217545E13F48ACF2DAFD9DF30396BC_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _AndroidJNIHelper_Box_m67A2A786DCE5ADD2FAF4F27B7CA115C82A8768C2_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _AndroidJNIHelper_ConvertToJNIArray_mBF20C1B6716BA00CA9C3825EA446B291E6D8EB20_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _AndroidJNIHelper_CreateJNIArgArray_m9605B7C73D18B6A11264A61E33888374E1F283A9_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _AndroidJNIHelper_CreateJavaProxy_m8E6AAE823A5FB6D70B4655FA45203779946321ED_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _AndroidJNIHelper_CreateJavaRunnable_mC009CB98AF579A1DBECE07EE23A4F20B8E53BDF0_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _AndroidJNIHelper_DeleteJNIArgArray_mCD37E30D32E979ED19131F9DC77A8DDD69D2E1A5_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _AndroidJNIHelper_GetConstructorID_m9A5019D80C0E776003ADFC0A54A879ECDC3B60D8_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _AndroidJNIHelper_GetMethodIDFallback_m45AC36798A5258FE80A68A2453CE3C45792E2C95_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _AndroidJNIHelper_GetMethodID_m22C073C0BCB560A1AD9EE6158FF8314D291EF756_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _AndroidJNIHelper_GetSignature_m090B053BFD9A6AC7BBD0F2BFAE56A8188CE4D80B_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _AndroidJNIHelper_GetSignature_m737340340A8C978F7AABB80DA4E31A8E700C73DA_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _AndroidJNIHelper_InvokeJavaProxyMethod_mF3275AFDFED43C42616A997FC582F1F90888AB87_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _AndroidJNIHelper_UnboxArray_m57E035906F4D79FCAC155162AC491BB7B575956D_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t _AndroidJNIHelper_Unbox_m813AFB8DE2C2568B011C81ED3AC4D013F1E5B67E_MetadataUsageId;
struct Delegate_t_marshaled_com;
struct Delegate_t_marshaled_pinvoke;
struct Exception_t_marshaled_com;
struct Exception_t_marshaled_pinvoke;

struct BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040;
struct ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821;
struct CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2;
struct DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86;
struct DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D;
struct Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28;
struct Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83;
struct Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F;
struct IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD;
struct ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A;
struct ParameterModifierU5BU5D_t63EC46F14F048DC9EF6BF1362E8AEBEA1A05A5EA;
struct SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889;
struct SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5;
struct StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E;
struct TypeU5BU5D_t7FE623A666B49176DE123306221193E888A12F5F;
struct AndroidJavaClassU5BU5D_t834F2CD8A8D5B7F304A36C401A57C24A723690C7;
struct AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379;
struct jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3;

IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// <Module>
struct  U3CModuleU3E_t52064C5B3EF9B4F802BDE275FFC96D25852579DF 
{
public:

public:
};


// System.Object

struct Il2CppArrayBounds;

// System.Array


// System.Reflection.Binder
struct  Binder_t4D5CB06963501D32847C057B57157D6DC49CA759  : public RuntimeObject
{
public:

public:
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


// System.Text.StringBuilder
struct  StringBuilder_t  : public RuntimeObject
{
public:
	// System.Char[] System.Text.StringBuilder::m_ChunkChars
	CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* ___m_ChunkChars_0;
	// System.Text.StringBuilder System.Text.StringBuilder::m_ChunkPrevious
	StringBuilder_t * ___m_ChunkPrevious_1;
	// System.Int32 System.Text.StringBuilder::m_ChunkLength
	int32_t ___m_ChunkLength_2;
	// System.Int32 System.Text.StringBuilder::m_ChunkOffset
	int32_t ___m_ChunkOffset_3;
	// System.Int32 System.Text.StringBuilder::m_MaxCapacity
	int32_t ___m_MaxCapacity_4;

public:
	inline static int32_t get_offset_of_m_ChunkChars_0() { return static_cast<int32_t>(offsetof(StringBuilder_t, ___m_ChunkChars_0)); }
	inline CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* get_m_ChunkChars_0() const { return ___m_ChunkChars_0; }
	inline CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2** get_address_of_m_ChunkChars_0() { return &___m_ChunkChars_0; }
	inline void set_m_ChunkChars_0(CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* value)
	{
		___m_ChunkChars_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_ChunkChars_0), (void*)value);
	}

	inline static int32_t get_offset_of_m_ChunkPrevious_1() { return static_cast<int32_t>(offsetof(StringBuilder_t, ___m_ChunkPrevious_1)); }
	inline StringBuilder_t * get_m_ChunkPrevious_1() const { return ___m_ChunkPrevious_1; }
	inline StringBuilder_t ** get_address_of_m_ChunkPrevious_1() { return &___m_ChunkPrevious_1; }
	inline void set_m_ChunkPrevious_1(StringBuilder_t * value)
	{
		___m_ChunkPrevious_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_ChunkPrevious_1), (void*)value);
	}

	inline static int32_t get_offset_of_m_ChunkLength_2() { return static_cast<int32_t>(offsetof(StringBuilder_t, ___m_ChunkLength_2)); }
	inline int32_t get_m_ChunkLength_2() const { return ___m_ChunkLength_2; }
	inline int32_t* get_address_of_m_ChunkLength_2() { return &___m_ChunkLength_2; }
	inline void set_m_ChunkLength_2(int32_t value)
	{
		___m_ChunkLength_2 = value;
	}

	inline static int32_t get_offset_of_m_ChunkOffset_3() { return static_cast<int32_t>(offsetof(StringBuilder_t, ___m_ChunkOffset_3)); }
	inline int32_t get_m_ChunkOffset_3() const { return ___m_ChunkOffset_3; }
	inline int32_t* get_address_of_m_ChunkOffset_3() { return &___m_ChunkOffset_3; }
	inline void set_m_ChunkOffset_3(int32_t value)
	{
		___m_ChunkOffset_3 = value;
	}

	inline static int32_t get_offset_of_m_MaxCapacity_4() { return static_cast<int32_t>(offsetof(StringBuilder_t, ___m_MaxCapacity_4)); }
	inline int32_t get_m_MaxCapacity_4() const { return ___m_MaxCapacity_4; }
	inline int32_t* get_address_of_m_MaxCapacity_4() { return &___m_MaxCapacity_4; }
	inline void set_m_MaxCapacity_4(int32_t value)
	{
		___m_MaxCapacity_4 = value;
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

// UnityEngine.AndroidJNI
struct  AndroidJNI_t814303BD74C07C665C3974493C1FB82D7E8F3B8D  : public RuntimeObject
{
public:

public:
};


// UnityEngine.AndroidJNIHelper
struct  AndroidJNIHelper_t89C239287FDA47996B4DA74992B2E246E0B0A49C  : public RuntimeObject
{
public:

public:
};


// UnityEngine.AndroidJNISafe
struct  AndroidJNISafe_t74FA312E869F9253D7ED237B32C7F992A6C9ED4E  : public RuntimeObject
{
public:

public:
};


// UnityEngine.AndroidJavaObject
struct  AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D  : public RuntimeObject
{
public:
	// UnityEngine.GlobalJavaObjectRef UnityEngine.AndroidJavaObject::m_jobject
	GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * ___m_jobject_1;
	// UnityEngine.GlobalJavaObjectRef UnityEngine.AndroidJavaObject::m_jclass
	GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * ___m_jclass_2;

public:
	inline static int32_t get_offset_of_m_jobject_1() { return static_cast<int32_t>(offsetof(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D, ___m_jobject_1)); }
	inline GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * get_m_jobject_1() const { return ___m_jobject_1; }
	inline GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 ** get_address_of_m_jobject_1() { return &___m_jobject_1; }
	inline void set_m_jobject_1(GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * value)
	{
		___m_jobject_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_jobject_1), (void*)value);
	}

	inline static int32_t get_offset_of_m_jclass_2() { return static_cast<int32_t>(offsetof(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D, ___m_jclass_2)); }
	inline GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * get_m_jclass_2() const { return ___m_jclass_2; }
	inline GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 ** get_address_of_m_jclass_2() { return &___m_jclass_2; }
	inline void set_m_jclass_2(GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * value)
	{
		___m_jclass_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_jclass_2), (void*)value);
	}
};

struct AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_StaticFields
{
public:
	// System.Boolean UnityEngine.AndroidJavaObject::enableDebugPrints
	bool ___enableDebugPrints_0;

public:
	inline static int32_t get_offset_of_enableDebugPrints_0() { return static_cast<int32_t>(offsetof(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_StaticFields, ___enableDebugPrints_0)); }
	inline bool get_enableDebugPrints_0() const { return ___enableDebugPrints_0; }
	inline bool* get_address_of_enableDebugPrints_0() { return &___enableDebugPrints_0; }
	inline void set_enableDebugPrints_0(bool value)
	{
		___enableDebugPrints_0 = value;
	}
};


// UnityEngine._AndroidJNIHelper
struct  _AndroidJNIHelper_t2104367336A4127C97F0F63CEF27E27792E7AA73  : public RuntimeObject
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


// System.Byte
struct  Byte_tF87C579059BD4633E6840EBBBEEF899C6E33EF07 
{
public:
	// System.Byte System.Byte::m_value
	uint8_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Byte_tF87C579059BD4633E6840EBBBEEF899C6E33EF07, ___m_value_0)); }
	inline uint8_t get_m_value_0() const { return ___m_value_0; }
	inline uint8_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(uint8_t value)
	{
		___m_value_0 = value;
	}
};


// System.Char
struct  Char_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9 
{
public:
	// System.Char System.Char::m_value
	Il2CppChar ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Char_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9, ___m_value_0)); }
	inline Il2CppChar get_m_value_0() const { return ___m_value_0; }
	inline Il2CppChar* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(Il2CppChar value)
	{
		___m_value_0 = value;
	}
};

struct Char_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9_StaticFields
{
public:
	// System.Byte[] System.Char::categoryForLatin1
	ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___categoryForLatin1_3;

public:
	inline static int32_t get_offset_of_categoryForLatin1_3() { return static_cast<int32_t>(offsetof(Char_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9_StaticFields, ___categoryForLatin1_3)); }
	inline ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* get_categoryForLatin1_3() const { return ___categoryForLatin1_3; }
	inline ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821** get_address_of_categoryForLatin1_3() { return &___categoryForLatin1_3; }
	inline void set_categoryForLatin1_3(ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* value)
	{
		___categoryForLatin1_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___categoryForLatin1_3), (void*)value);
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

// System.Int16
struct  Int16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D 
{
public:
	// System.Int16 System.Int16::m_value
	int16_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(Int16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D, ___m_value_0)); }
	inline int16_t get_m_value_0() const { return ___m_value_0; }
	inline int16_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(int16_t value)
	{
		___m_value_0 = value;
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


// System.Reflection.MethodBase
struct  MethodBase_t  : public MemberInfo_t
{
public:

public:
};


// System.Reflection.ParameterModifier
struct  ParameterModifier_t7BEFF7C52C8D7CD73D787BDAE6A1A50196204E3E 
{
public:
	// System.Boolean[] System.Reflection.ParameterModifier::_byRef
	BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040* ____byRef_0;

public:
	inline static int32_t get_offset_of__byRef_0() { return static_cast<int32_t>(offsetof(ParameterModifier_t7BEFF7C52C8D7CD73D787BDAE6A1A50196204E3E, ____byRef_0)); }
	inline BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040* get__byRef_0() const { return ____byRef_0; }
	inline BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040** get_address_of__byRef_0() { return &____byRef_0; }
	inline void set__byRef_0(BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040* value)
	{
		____byRef_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____byRef_0), (void*)value);
	}
};

// Native definition for P/Invoke marshalling of System.Reflection.ParameterModifier
struct ParameterModifier_t7BEFF7C52C8D7CD73D787BDAE6A1A50196204E3E_marshaled_pinvoke
{
	int32_t* ____byRef_0;
};
// Native definition for COM marshalling of System.Reflection.ParameterModifier
struct ParameterModifier_t7BEFF7C52C8D7CD73D787BDAE6A1A50196204E3E_marshaled_com
{
	int32_t* ____byRef_0;
};

// System.Runtime.InteropServices.GCHandle
struct  GCHandle_t39FAEE3EA592432C93B574A31DD83B87F1847DE3 
{
public:
	// System.Int32 System.Runtime.InteropServices.GCHandle::handle
	int32_t ___handle_0;

public:
	inline static int32_t get_offset_of_handle_0() { return static_cast<int32_t>(offsetof(GCHandle_t39FAEE3EA592432C93B574A31DD83B87F1847DE3, ___handle_0)); }
	inline int32_t get_handle_0() const { return ___handle_0; }
	inline int32_t* get_address_of_handle_0() { return &___handle_0; }
	inline void set_handle_0(int32_t value)
	{
		___handle_0 = value;
	}
};


// System.SByte
struct  SByte_t9070AEA2966184235653CB9B4D33B149CDA831DF 
{
public:
	// System.SByte System.SByte::m_value
	int8_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(SByte_t9070AEA2966184235653CB9B4D33B149CDA831DF, ___m_value_0)); }
	inline int8_t get_m_value_0() const { return ___m_value_0; }
	inline int8_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(int8_t value)
	{
		___m_value_0 = value;
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


// UnityEngine.AndroidJavaClass
struct  AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE  : public AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D
{
public:

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


// System.Reflection.MethodInfo
struct  MethodInfo_t  : public MethodBase_t
{
public:

public:
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


// UnityEngine.AndroidJavaProxy
struct  AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D  : public RuntimeObject
{
public:
	// UnityEngine.AndroidJavaClass UnityEngine.AndroidJavaProxy::javaInterface
	AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * ___javaInterface_0;
	// System.IntPtr UnityEngine.AndroidJavaProxy::proxyObject
	intptr_t ___proxyObject_1;

public:
	inline static int32_t get_offset_of_javaInterface_0() { return static_cast<int32_t>(offsetof(AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D, ___javaInterface_0)); }
	inline AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * get_javaInterface_0() const { return ___javaInterface_0; }
	inline AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE ** get_address_of_javaInterface_0() { return &___javaInterface_0; }
	inline void set_javaInterface_0(AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * value)
	{
		___javaInterface_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___javaInterface_0), (void*)value);
	}

	inline static int32_t get_offset_of_proxyObject_1() { return static_cast<int32_t>(offsetof(AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D, ___proxyObject_1)); }
	inline intptr_t get_proxyObject_1() const { return ___proxyObject_1; }
	inline intptr_t* get_address_of_proxyObject_1() { return &___proxyObject_1; }
	inline void set_proxyObject_1(intptr_t value)
	{
		___proxyObject_1 = value;
	}
};

struct AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D_StaticFields
{
public:
	// UnityEngine.GlobalJavaObjectRef UnityEngine.AndroidJavaProxy::s_JavaLangSystemClass
	GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * ___s_JavaLangSystemClass_2;
	// System.IntPtr UnityEngine.AndroidJavaProxy::s_HashCodeMethodID
	intptr_t ___s_HashCodeMethodID_3;

public:
	inline static int32_t get_offset_of_s_JavaLangSystemClass_2() { return static_cast<int32_t>(offsetof(AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D_StaticFields, ___s_JavaLangSystemClass_2)); }
	inline GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * get_s_JavaLangSystemClass_2() const { return ___s_JavaLangSystemClass_2; }
	inline GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 ** get_address_of_s_JavaLangSystemClass_2() { return &___s_JavaLangSystemClass_2; }
	inline void set_s_JavaLangSystemClass_2(GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * value)
	{
		___s_JavaLangSystemClass_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_JavaLangSystemClass_2), (void*)value);
	}

	inline static int32_t get_offset_of_s_HashCodeMethodID_3() { return static_cast<int32_t>(offsetof(AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D_StaticFields, ___s_HashCodeMethodID_3)); }
	inline intptr_t get_s_HashCodeMethodID_3() const { return ___s_HashCodeMethodID_3; }
	inline intptr_t* get_address_of_s_HashCodeMethodID_3() { return &___s_HashCodeMethodID_3; }
	inline void set_s_HashCodeMethodID_3(intptr_t value)
	{
		___s_HashCodeMethodID_3 = value;
	}
};


// UnityEngine.AndroidReflection
struct  AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533  : public RuntimeObject
{
public:

public:
};

struct AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_StaticFields
{
public:
	// UnityEngine.GlobalJavaObjectRef UnityEngine.AndroidReflection::s_ReflectionHelperClass
	GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * ___s_ReflectionHelperClass_0;
	// System.IntPtr UnityEngine.AndroidReflection::s_ReflectionHelperGetConstructorID
	intptr_t ___s_ReflectionHelperGetConstructorID_1;
	// System.IntPtr UnityEngine.AndroidReflection::s_ReflectionHelperGetMethodID
	intptr_t ___s_ReflectionHelperGetMethodID_2;
	// System.IntPtr UnityEngine.AndroidReflection::s_ReflectionHelperGetFieldID
	intptr_t ___s_ReflectionHelperGetFieldID_3;
	// System.IntPtr UnityEngine.AndroidReflection::s_ReflectionHelperNewProxyInstance
	intptr_t ___s_ReflectionHelperNewProxyInstance_4;
	// System.IntPtr UnityEngine.AndroidReflection::s_ReflectionHelperSetNativeExceptionOnProxy
	intptr_t ___s_ReflectionHelperSetNativeExceptionOnProxy_5;

public:
	inline static int32_t get_offset_of_s_ReflectionHelperClass_0() { return static_cast<int32_t>(offsetof(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_StaticFields, ___s_ReflectionHelperClass_0)); }
	inline GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * get_s_ReflectionHelperClass_0() const { return ___s_ReflectionHelperClass_0; }
	inline GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 ** get_address_of_s_ReflectionHelperClass_0() { return &___s_ReflectionHelperClass_0; }
	inline void set_s_ReflectionHelperClass_0(GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * value)
	{
		___s_ReflectionHelperClass_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_ReflectionHelperClass_0), (void*)value);
	}

	inline static int32_t get_offset_of_s_ReflectionHelperGetConstructorID_1() { return static_cast<int32_t>(offsetof(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_StaticFields, ___s_ReflectionHelperGetConstructorID_1)); }
	inline intptr_t get_s_ReflectionHelperGetConstructorID_1() const { return ___s_ReflectionHelperGetConstructorID_1; }
	inline intptr_t* get_address_of_s_ReflectionHelperGetConstructorID_1() { return &___s_ReflectionHelperGetConstructorID_1; }
	inline void set_s_ReflectionHelperGetConstructorID_1(intptr_t value)
	{
		___s_ReflectionHelperGetConstructorID_1 = value;
	}

	inline static int32_t get_offset_of_s_ReflectionHelperGetMethodID_2() { return static_cast<int32_t>(offsetof(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_StaticFields, ___s_ReflectionHelperGetMethodID_2)); }
	inline intptr_t get_s_ReflectionHelperGetMethodID_2() const { return ___s_ReflectionHelperGetMethodID_2; }
	inline intptr_t* get_address_of_s_ReflectionHelperGetMethodID_2() { return &___s_ReflectionHelperGetMethodID_2; }
	inline void set_s_ReflectionHelperGetMethodID_2(intptr_t value)
	{
		___s_ReflectionHelperGetMethodID_2 = value;
	}

	inline static int32_t get_offset_of_s_ReflectionHelperGetFieldID_3() { return static_cast<int32_t>(offsetof(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_StaticFields, ___s_ReflectionHelperGetFieldID_3)); }
	inline intptr_t get_s_ReflectionHelperGetFieldID_3() const { return ___s_ReflectionHelperGetFieldID_3; }
	inline intptr_t* get_address_of_s_ReflectionHelperGetFieldID_3() { return &___s_ReflectionHelperGetFieldID_3; }
	inline void set_s_ReflectionHelperGetFieldID_3(intptr_t value)
	{
		___s_ReflectionHelperGetFieldID_3 = value;
	}

	inline static int32_t get_offset_of_s_ReflectionHelperNewProxyInstance_4() { return static_cast<int32_t>(offsetof(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_StaticFields, ___s_ReflectionHelperNewProxyInstance_4)); }
	inline intptr_t get_s_ReflectionHelperNewProxyInstance_4() const { return ___s_ReflectionHelperNewProxyInstance_4; }
	inline intptr_t* get_address_of_s_ReflectionHelperNewProxyInstance_4() { return &___s_ReflectionHelperNewProxyInstance_4; }
	inline void set_s_ReflectionHelperNewProxyInstance_4(intptr_t value)
	{
		___s_ReflectionHelperNewProxyInstance_4 = value;
	}

	inline static int32_t get_offset_of_s_ReflectionHelperSetNativeExceptionOnProxy_5() { return static_cast<int32_t>(offsetof(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_StaticFields, ___s_ReflectionHelperSetNativeExceptionOnProxy_5)); }
	inline intptr_t get_s_ReflectionHelperSetNativeExceptionOnProxy_5() const { return ___s_ReflectionHelperSetNativeExceptionOnProxy_5; }
	inline intptr_t* get_address_of_s_ReflectionHelperSetNativeExceptionOnProxy_5() { return &___s_ReflectionHelperSetNativeExceptionOnProxy_5; }
	inline void set_s_ReflectionHelperSetNativeExceptionOnProxy_5(intptr_t value)
	{
		___s_ReflectionHelperSetNativeExceptionOnProxy_5 = value;
	}
};


// UnityEngine.GlobalJavaObjectRef
struct  GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0  : public RuntimeObject
{
public:
	// System.Boolean UnityEngine.GlobalJavaObjectRef::m_disposed
	bool ___m_disposed_0;
	// System.IntPtr UnityEngine.GlobalJavaObjectRef::m_jobject
	intptr_t ___m_jobject_1;

public:
	inline static int32_t get_offset_of_m_disposed_0() { return static_cast<int32_t>(offsetof(GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0, ___m_disposed_0)); }
	inline bool get_m_disposed_0() const { return ___m_disposed_0; }
	inline bool* get_address_of_m_disposed_0() { return &___m_disposed_0; }
	inline void set_m_disposed_0(bool value)
	{
		___m_disposed_0 = value;
	}

	inline static int32_t get_offset_of_m_jobject_1() { return static_cast<int32_t>(offsetof(GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0, ___m_jobject_1)); }
	inline intptr_t get_m_jobject_1() const { return ___m_jobject_1; }
	inline intptr_t* get_address_of_m_jobject_1() { return &___m_jobject_1; }
	inline void set_m_jobject_1(intptr_t value)
	{
		___m_jobject_1 = value;
	}
};


// UnityEngine.jvalue
struct  jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37 
{
public:
	union
	{
		#pragma pack(push, tp, 1)
		struct
		{
			// System.Boolean UnityEngine.jvalue::z
			bool ___z_0;
		};
		#pragma pack(pop, tp)
		struct
		{
			bool ___z_0_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			// System.SByte UnityEngine.jvalue::b
			int8_t ___b_1;
		};
		#pragma pack(pop, tp)
		struct
		{
			int8_t ___b_1_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			// System.Char UnityEngine.jvalue::c
			Il2CppChar ___c_2;
		};
		#pragma pack(pop, tp)
		struct
		{
			Il2CppChar ___c_2_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			// System.Int16 UnityEngine.jvalue::s
			int16_t ___s_3;
		};
		#pragma pack(pop, tp)
		struct
		{
			int16_t ___s_3_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			// System.Int32 UnityEngine.jvalue::i
			int32_t ___i_4;
		};
		#pragma pack(pop, tp)
		struct
		{
			int32_t ___i_4_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			// System.Int64 UnityEngine.jvalue::j
			int64_t ___j_5;
		};
		#pragma pack(pop, tp)
		struct
		{
			int64_t ___j_5_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			// System.Single UnityEngine.jvalue::f
			float ___f_6;
		};
		#pragma pack(pop, tp)
		struct
		{
			float ___f_6_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			// System.Double UnityEngine.jvalue::d
			double ___d_7;
		};
		#pragma pack(pop, tp)
		struct
		{
			double ___d_7_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			// System.IntPtr UnityEngine.jvalue::l
			intptr_t ___l_8;
		};
		#pragma pack(pop, tp)
		struct
		{
			intptr_t ___l_8_forAlignmentOnly;
		};
	};

public:
	inline static int32_t get_offset_of_z_0() { return static_cast<int32_t>(offsetof(jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37, ___z_0)); }
	inline bool get_z_0() const { return ___z_0; }
	inline bool* get_address_of_z_0() { return &___z_0; }
	inline void set_z_0(bool value)
	{
		___z_0 = value;
	}

	inline static int32_t get_offset_of_b_1() { return static_cast<int32_t>(offsetof(jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37, ___b_1)); }
	inline int8_t get_b_1() const { return ___b_1; }
	inline int8_t* get_address_of_b_1() { return &___b_1; }
	inline void set_b_1(int8_t value)
	{
		___b_1 = value;
	}

	inline static int32_t get_offset_of_c_2() { return static_cast<int32_t>(offsetof(jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37, ___c_2)); }
	inline Il2CppChar get_c_2() const { return ___c_2; }
	inline Il2CppChar* get_address_of_c_2() { return &___c_2; }
	inline void set_c_2(Il2CppChar value)
	{
		___c_2 = value;
	}

	inline static int32_t get_offset_of_s_3() { return static_cast<int32_t>(offsetof(jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37, ___s_3)); }
	inline int16_t get_s_3() const { return ___s_3; }
	inline int16_t* get_address_of_s_3() { return &___s_3; }
	inline void set_s_3(int16_t value)
	{
		___s_3 = value;
	}

	inline static int32_t get_offset_of_i_4() { return static_cast<int32_t>(offsetof(jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37, ___i_4)); }
	inline int32_t get_i_4() const { return ___i_4; }
	inline int32_t* get_address_of_i_4() { return &___i_4; }
	inline void set_i_4(int32_t value)
	{
		___i_4 = value;
	}

	inline static int32_t get_offset_of_j_5() { return static_cast<int32_t>(offsetof(jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37, ___j_5)); }
	inline int64_t get_j_5() const { return ___j_5; }
	inline int64_t* get_address_of_j_5() { return &___j_5; }
	inline void set_j_5(int64_t value)
	{
		___j_5 = value;
	}

	inline static int32_t get_offset_of_f_6() { return static_cast<int32_t>(offsetof(jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37, ___f_6)); }
	inline float get_f_6() const { return ___f_6; }
	inline float* get_address_of_f_6() { return &___f_6; }
	inline void set_f_6(float value)
	{
		___f_6 = value;
	}

	inline static int32_t get_offset_of_d_7() { return static_cast<int32_t>(offsetof(jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37, ___d_7)); }
	inline double get_d_7() const { return ___d_7; }
	inline double* get_address_of_d_7() { return &___d_7; }
	inline void set_d_7(double value)
	{
		___d_7 = value;
	}

	inline static int32_t get_offset_of_l_8() { return static_cast<int32_t>(offsetof(jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37, ___l_8)); }
	inline intptr_t get_l_8() const { return ___l_8; }
	inline intptr_t* get_address_of_l_8() { return &___l_8; }
	inline void set_l_8(intptr_t value)
	{
		___l_8 = value;
	}
};

// Native definition for P/Invoke marshalling of UnityEngine.jvalue
struct jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37_marshaled_pinvoke
{
	union
	{
		#pragma pack(push, tp, 1)
		struct
		{
			int32_t ___z_0;
		};
		#pragma pack(pop, tp)
		struct
		{
			int32_t ___z_0_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			int8_t ___b_1;
		};
		#pragma pack(pop, tp)
		struct
		{
			int8_t ___b_1_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			uint8_t ___c_2;
		};
		#pragma pack(pop, tp)
		struct
		{
			uint8_t ___c_2_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			int16_t ___s_3;
		};
		#pragma pack(pop, tp)
		struct
		{
			int16_t ___s_3_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			int32_t ___i_4;
		};
		#pragma pack(pop, tp)
		struct
		{
			int32_t ___i_4_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			int64_t ___j_5;
		};
		#pragma pack(pop, tp)
		struct
		{
			int64_t ___j_5_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			float ___f_6;
		};
		#pragma pack(pop, tp)
		struct
		{
			float ___f_6_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			double ___d_7;
		};
		#pragma pack(pop, tp)
		struct
		{
			double ___d_7_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			intptr_t ___l_8;
		};
		#pragma pack(pop, tp)
		struct
		{
			intptr_t ___l_8_forAlignmentOnly;
		};
	};
};
// Native definition for COM marshalling of UnityEngine.jvalue
struct jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37_marshaled_com
{
	union
	{
		#pragma pack(push, tp, 1)
		struct
		{
			int32_t ___z_0;
		};
		#pragma pack(pop, tp)
		struct
		{
			int32_t ___z_0_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			int8_t ___b_1;
		};
		#pragma pack(pop, tp)
		struct
		{
			int8_t ___b_1_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			uint8_t ___c_2;
		};
		#pragma pack(pop, tp)
		struct
		{
			uint8_t ___c_2_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			int16_t ___s_3;
		};
		#pragma pack(pop, tp)
		struct
		{
			int16_t ___s_3_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			int32_t ___i_4;
		};
		#pragma pack(pop, tp)
		struct
		{
			int32_t ___i_4_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			int64_t ___j_5;
		};
		#pragma pack(pop, tp)
		struct
		{
			int64_t ___j_5_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			float ___f_6;
		};
		#pragma pack(pop, tp)
		struct
		{
			float ___f_6_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			double ___d_7;
		};
		#pragma pack(pop, tp)
		struct
		{
			double ___d_7_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			intptr_t ___l_8;
		};
		#pragma pack(pop, tp)
		struct
		{
			intptr_t ___l_8_forAlignmentOnly;
		};
	};
};

// System.ApplicationException
struct  ApplicationException_t664823C3E0D3E1E7C7FA1C0DB4E19E98E9811C74  : public Exception_t
{
public:

public:
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


// UnityEngine.AndroidJavaException
struct  AndroidJavaException_tC81E6FAAA4067CBA537727328D5D2DB14F5F5466  : public Exception_t
{
public:
	// System.String UnityEngine.AndroidJavaException::mJavaStackTrace
	String_t* ___mJavaStackTrace_17;

public:
	inline static int32_t get_offset_of_mJavaStackTrace_17() { return static_cast<int32_t>(offsetof(AndroidJavaException_tC81E6FAAA4067CBA537727328D5D2DB14F5F5466, ___mJavaStackTrace_17)); }
	inline String_t* get_mJavaStackTrace_17() const { return ___mJavaStackTrace_17; }
	inline String_t** get_address_of_mJavaStackTrace_17() { return &___mJavaStackTrace_17; }
	inline void set_mJavaStackTrace_17(String_t* value)
	{
		___mJavaStackTrace_17 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___mJavaStackTrace_17), (void*)value);
	}
};


// UnityEngine.AndroidJavaRunnableProxy
struct  AndroidJavaRunnableProxy_t3C66FEA8C2A903168F2902788AB8AB29CA923308  : public AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D
{
public:
	// UnityEngine.AndroidJavaRunnable UnityEngine.AndroidJavaRunnableProxy::mRunnable
	AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 * ___mRunnable_4;

public:
	inline static int32_t get_offset_of_mRunnable_4() { return static_cast<int32_t>(offsetof(AndroidJavaRunnableProxy_t3C66FEA8C2A903168F2902788AB8AB29CA923308, ___mRunnable_4)); }
	inline AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 * get_mRunnable_4() const { return ___mRunnable_4; }
	inline AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 ** get_address_of_mRunnable_4() { return &___mRunnable_4; }
	inline void set_mRunnable_4(AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 * value)
	{
		___mRunnable_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___mRunnable_4), (void*)value);
	}
};


// System.AsyncCallback
struct  AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4  : public MulticastDelegate_t
{
public:

public:
};


// System.Reflection.TargetInvocationException
struct  TargetInvocationException_t0DD35F6083E1D1E0509BF181A79C76D3339D89B8  : public ApplicationException_t664823C3E0D3E1E7C7FA1C0DB4E19E98E9811C74
{
public:

public:
};


// UnityEngine.AndroidJavaRunnable
struct  AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02  : public MulticastDelegate_t
{
public:

public:
};

#ifdef __clang__
#pragma clang diagnostic pop
#endif
// UnityEngine.jvalue[]
struct jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37  m_Items[1];

public:
	inline jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37  GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37 * GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37  value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37  GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37 * GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37  value)
	{
		m_Items[index] = value;
	}
};
// System.Boolean[]
struct BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) bool m_Items[1];

public:
	inline bool GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline bool* GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, bool value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline bool GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline bool* GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, bool value)
	{
		m_Items[index] = value;
	}
};
// System.Byte[]
struct ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) uint8_t m_Items[1];

public:
	inline uint8_t GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline uint8_t* GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, uint8_t value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline uint8_t GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline uint8_t* GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, uint8_t value)
	{
		m_Items[index] = value;
	}
};
// System.SByte[]
struct SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) int8_t m_Items[1];

public:
	inline int8_t GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline int8_t* GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, int8_t value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline int8_t GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline int8_t* GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, int8_t value)
	{
		m_Items[index] = value;
	}
};
// System.Char[]
struct CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) Il2CppChar m_Items[1];

public:
	inline Il2CppChar GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline Il2CppChar* GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, Il2CppChar value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline Il2CppChar GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline Il2CppChar* GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, Il2CppChar value)
	{
		m_Items[index] = value;
	}
};
// System.Int16[]
struct Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) int16_t m_Items[1];

public:
	inline int16_t GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline int16_t* GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, int16_t value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline int16_t GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline int16_t* GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, int16_t value)
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
// System.Int64[]
struct Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) int64_t m_Items[1];

public:
	inline int64_t GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline int64_t* GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, int64_t value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline int64_t GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline int64_t* GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, int64_t value)
	{
		m_Items[index] = value;
	}
};
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
// System.Double[]
struct DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) double m_Items[1];

public:
	inline double GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline double* GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, double value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline double GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline double* GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, double value)
	{
		m_Items[index] = value;
	}
};
// System.IntPtr[]
struct IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) intptr_t m_Items[1];

public:
	inline intptr_t GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline intptr_t* GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, intptr_t value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
	}
	inline intptr_t GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline intptr_t* GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, intptr_t value)
	{
		m_Items[index] = value;
	}
};
// System.Object[]
struct ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) RuntimeObject * m_Items[1];

public:
	inline RuntimeObject * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline RuntimeObject ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, RuntimeObject * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline RuntimeObject * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline RuntimeObject ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, RuntimeObject * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};
// System.Type[]
struct TypeU5BU5D_t7FE623A666B49176DE123306221193E888A12F5F  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) Type_t * m_Items[1];

public:
	inline Type_t * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline Type_t ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, Type_t * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline Type_t * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline Type_t ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, Type_t * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
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
// System.Reflection.ParameterModifier[]
struct ParameterModifierU5BU5D_t63EC46F14F048DC9EF6BF1362E8AEBEA1A05A5EA  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) ParameterModifier_t7BEFF7C52C8D7CD73D787BDAE6A1A50196204E3E  m_Items[1];

public:
	inline ParameterModifier_t7BEFF7C52C8D7CD73D787BDAE6A1A50196204E3E  GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline ParameterModifier_t7BEFF7C52C8D7CD73D787BDAE6A1A50196204E3E * GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, ParameterModifier_t7BEFF7C52C8D7CD73D787BDAE6A1A50196204E3E  value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)&((m_Items + index)->____byRef_0), (void*)NULL);
	}
	inline ParameterModifier_t7BEFF7C52C8D7CD73D787BDAE6A1A50196204E3E  GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline ParameterModifier_t7BEFF7C52C8D7CD73D787BDAE6A1A50196204E3E * GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, ParameterModifier_t7BEFF7C52C8D7CD73D787BDAE6A1A50196204E3E  value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)&((m_Items + index)->____byRef_0), (void*)NULL);
	}
};
// UnityEngine.AndroidJavaObject[]
struct AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * m_Items[1];

public:
	inline AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
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
// UnityEngine.AndroidJavaClass[]
struct AndroidJavaClassU5BU5D_t834F2CD8A8D5B7F304A36C401A57C24A723690C7  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * m_Items[1];

public:
	inline AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
};


// ReturnType UnityEngine.AndroidJavaObject::Call<System.Object>(System.String,System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * AndroidJavaObject_Call_TisRuntimeObject_m38064E69DD787BA971B0757788FD11E7239A03B7_gshared (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method);
// ReturnType UnityEngine.AndroidJavaObject::CallStatic<System.Int32>(System.String,System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t AndroidJavaObject_CallStatic_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m90D39A3F3725F8BD3F782614FA0101D563DA9CCF_gshared (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method);
// ReturnType UnityEngine.AndroidJavaObject::Call<System.Boolean>(System.String,System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool AndroidJavaObject_Call_TisBoolean_tB53F6830F670160873277339AA58F15CAED4399C_m57EE1ACB271D15DD0E2DDD6B28805C31799A0976_gshared (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method);
// ReturnType UnityEngine.AndroidJavaObject::CallStatic<System.Object>(System.String,System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * AndroidJavaObject_CallStatic_TisRuntimeObject_mC00F70734976E6B3DD8281EB6EBC457B19762E9F_gshared (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method);
// ReturnType UnityEngine.AndroidJavaObject::Call<System.Int32>(System.String,System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t AndroidJavaObject_Call_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_mF7220A3D48BA18737AA0C7DAF0828822275A69A6_gshared (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method);
// ReturnType UnityEngine.AndroidJavaObject::Call<System.SByte>(System.String,System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int8_t AndroidJavaObject_Call_TisSByte_t9070AEA2966184235653CB9B4D33B149CDA831DF_m1DA87DAFADCDA8DE62A86D5C1F94DF60F2F54651_gshared (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method);
// ReturnType UnityEngine.AndroidJavaObject::Call<System.Int16>(System.String,System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int16_t AndroidJavaObject_Call_TisInt16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D_mB51ADF5CFAE5278F11CECE74CC8ABAA9B45BB34F_gshared (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method);
// ReturnType UnityEngine.AndroidJavaObject::Call<System.Int64>(System.String,System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t AndroidJavaObject_Call_TisInt64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436_mCD42F5F94257CC748CBA517A16A7BCC707A0C440_gshared (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method);
// ReturnType UnityEngine.AndroidJavaObject::Call<System.Single>(System.String,System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float AndroidJavaObject_Call_TisSingle_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1_m241B6C5C3A0259B256071CA26CAFE3EF0F229DBA_gshared (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method);
// ReturnType UnityEngine.AndroidJavaObject::Call<System.Double>(System.String,System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR double AndroidJavaObject_Call_TisDouble_t358B8F23BDC52A5DD700E727E204F9F7CDE12409_mBDD67692E825B1F8834E22FC94628B9C6AE54C81_gshared (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method);
// ReturnType UnityEngine.AndroidJavaObject::Call<System.Char>(System.String,System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Il2CppChar AndroidJavaObject_Call_TisChar_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9_m73C43D18BEC4AF2416AC8ADA8FA26712645A0EEA_gshared (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method);

// System.IntPtr UnityEngine.AndroidJNI::NewStringFromStr(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_NewStringFromStr_m01AAA91EC40C908302162C5653D6AFEFC384BBA9 (String_t* ___chars0, const RuntimeMethod* method);
// System.IntPtr UnityEngine._AndroidJNIHelper::GetConstructorID(System.IntPtr,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t _AndroidJNIHelper_GetConstructorID_m9A5019D80C0E776003ADFC0A54A879ECDC3B60D8 (intptr_t ___jclass0, String_t* ___signature1, const RuntimeMethod* method);
// System.IntPtr UnityEngine._AndroidJNIHelper::GetMethodID(System.IntPtr,System.String,System.String,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t _AndroidJNIHelper_GetMethodID_m22C073C0BCB560A1AD9EE6158FF8314D291EF756 (intptr_t ___jclass0, String_t* ___methodName1, String_t* ___signature2, bool ___isStatic3, const RuntimeMethod* method);
// System.IntPtr UnityEngine._AndroidJNIHelper::CreateJavaRunnable(UnityEngine.AndroidJavaRunnable)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t _AndroidJNIHelper_CreateJavaRunnable_mC009CB98AF579A1DBECE07EE23A4F20B8E53BDF0 (AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 * ___jrunnable0, const RuntimeMethod* method);
// System.Runtime.InteropServices.GCHandle System.Runtime.InteropServices.GCHandle::Alloc(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR GCHandle_t39FAEE3EA592432C93B574A31DD83B87F1847DE3  GCHandle_Alloc_m5BF9DC23B533B904BFEA61136B92916683B46B0F (RuntimeObject * ___value0, const RuntimeMethod* method);
// System.IntPtr System.Runtime.InteropServices.GCHandle::ToIntPtr(System.Runtime.InteropServices.GCHandle)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t GCHandle_ToIntPtr_m8CF7D07846B0C741B04A2A4E5E9B5D505F4B3CCE (GCHandle_t39FAEE3EA592432C93B574A31DD83B87F1847DE3  ___value0, const RuntimeMethod* method);
// System.IntPtr UnityEngine._AndroidJNIHelper::CreateJavaProxy(System.IntPtr,UnityEngine.AndroidJavaProxy)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t _AndroidJNIHelper_CreateJavaProxy_m8E6AAE823A5FB6D70B4655FA45203779946321ED (intptr_t ___delegateHandle0, AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D * ___proxy1, const RuntimeMethod* method);
// System.Void System.Runtime.InteropServices.GCHandle::Free()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GCHandle_Free_m392ECC9B1058E35A0FD5CF21A65F212873FC26F0 (GCHandle_t39FAEE3EA592432C93B574A31DD83B87F1847DE3 * __this, const RuntimeMethod* method);
// UnityEngine.jvalue[] UnityEngine._AndroidJNIHelper::CreateJNIArgArray(System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* _AndroidJNIHelper_CreateJNIArgArray_m9605B7C73D18B6A11264A61E33888374E1F283A9 (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args0, const RuntimeMethod* method);
// System.Void UnityEngine._AndroidJNIHelper::DeleteJNIArgArray(System.Object[],UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _AndroidJNIHelper_DeleteJNIArgArray_mCD37E30D32E979ED19131F9DC77A8DDD69D2E1A5 (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args0, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___jniArgs1, const RuntimeMethod* method);
// System.IntPtr UnityEngine._AndroidJNIHelper::GetConstructorID(System.IntPtr,System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t _AndroidJNIHelper_GetConstructorID_m1982E4290531BD8134C7B5EDF918B87466284D77 (intptr_t ___jclass0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::ExceptionOccurred()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_ExceptionOccurred_mC2EC654C42E285C9E141393BDA41A4D8BC56FECD (const RuntimeMethod* method);
// System.Boolean System.IntPtr::op_Inequality(System.IntPtr,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool IntPtr_op_Inequality_mB4886A806009EA825EFCC60CD2A7F6EB8E273A61 (intptr_t ___value10, intptr_t ___value21, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJNI::ExceptionClear()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNI_ExceptionClear_m339CEFB228B0F08EBA289AED25464FF0D80B9936 (const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::FindClass(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_FindClass_m07E2127D59F7EC97A06B5350699033448BD40CED (String_t* ___name0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::GetMethodID(System.IntPtr,System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_GetMethodID_m4D7386D69FFEF80467F1804447C094B59385AF0C (intptr_t ___clazz0, String_t* ___name1, String_t* ___sig2, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::GetStaticMethodID(System.IntPtr,System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_GetStaticMethodID_m135C9DEFFC207E509C001370C227F6E217FD9A1C (intptr_t ___clazz0, String_t* ___name1, String_t* ___sig2, const RuntimeMethod* method);
// System.String UnityEngine.AndroidJNI::CallStringMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* AndroidJNI_CallStringMethod_m3322E22FCA053618D794A9F3D00CFA1368F10AA9 (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.String UnityEngine.AndroidJNI::CallStaticStringMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* AndroidJNI_CallStaticStringMethod_m7502E60348B62159AE2F0C06D3D663E6E1F28116 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJavaException::.ctor(System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaException__ctor_m8E5216F0181090FB7A9016AED78B7935019791D8 (AndroidJavaException_tC81E6FAAA4067CBA537727328D5D2DB14F5F5466 * __this, String_t* ___message0, String_t* ___javaStackTrace1, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJNISafe::DeleteLocalRef(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNISafe_DeleteLocalRef_m9632EA13BF03AEE43FC7713125962A4D0DFFADC7 (intptr_t ___localref0, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJNI::DeleteGlobalRef(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNI_DeleteGlobalRef_mC800FCE93424A8778220806C3FE3497E21E94333 (intptr_t ___obj0, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJNI::DeleteWeakGlobalRef(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNI_DeleteWeakGlobalRef_m07AE954A94CDB58980A3CBA36E0E8F236BE01C75 (intptr_t ___obj0, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJNI::DeleteLocalRef(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNI_DeleteLocalRef_m5A7291640D0BB0F2A484C729CEDBF43F92B7941A (intptr_t ___obj0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::NewString(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_NewString_m4B505016C60A4B2602F2037983367C2DB52A8BE2 (String_t* ___chars0, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJNISafe::CheckException()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C (const RuntimeMethod* method);
// System.String UnityEngine.AndroidJNI::GetStringChars(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* AndroidJNI_GetStringChars_m1C44DAAF9B7AA8E9586F1CD236E825B07741A268 (intptr_t ___str0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::GetObjectClass(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_GetObjectClass_mA9719B0A6734C4ED55B60B129A9D51F7B8A3B4A6 (intptr_t ___obj0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::FromReflectedMethod(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_FromReflectedMethod_m5F01D9D2E6FDB25E9DF3B8804FC6A536C71F84B9 (intptr_t ___refMethod0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::NewObject(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_NewObject_mA1E19D3C530766C0E9F3196CB23A4C9E7795689B (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJNI::CallStaticVoidMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNI_CallStaticVoidMethod_m973B08F0CE8068F0AC8A8FF85F0C63FD5AC3EAFA (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::CallStaticObjectMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_CallStaticObjectMethod_m8540B678387A3DE6F1F702CF3053826962F569C0 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.Char UnityEngine.AndroidJNI::CallStaticCharMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Il2CppChar AndroidJNI_CallStaticCharMethod_m03968EDD820122C5AA74D396578D5C8F747DE8B9 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.Double UnityEngine.AndroidJNI::CallStaticDoubleMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR double AndroidJNI_CallStaticDoubleMethod_mB27665BD677D31470812D5E4FA466259D18D8D67 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.Single UnityEngine.AndroidJNI::CallStaticFloatMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float AndroidJNI_CallStaticFloatMethod_m22FE454F030F117CFA7CE8F8CE55A4DD9EB226DD (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.Int64 UnityEngine.AndroidJNI::CallStaticLongMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t AndroidJNI_CallStaticLongMethod_mACA1CFC943C54BB656D065AB6EF0A78FE3EEC014 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.Int16 UnityEngine.AndroidJNI::CallStaticShortMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int16_t AndroidJNI_CallStaticShortMethod_m1BC0BA260F59800529D511D0E51B501165056F3F (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.SByte UnityEngine.AndroidJNI::CallStaticSByteMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int8_t AndroidJNI_CallStaticSByteMethod_m637357610E5ECF91256FD6EFA48468D276395F46 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.Boolean UnityEngine.AndroidJNI::CallStaticBooleanMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool AndroidJNI_CallStaticBooleanMethod_mA5C4F5D3A724351C0DB569E863F070493E86069F (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.Int32 UnityEngine.AndroidJNI::CallStaticIntMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t AndroidJNI_CallStaticIntMethod_mC112D86B8844819C4D02AA8136BCF8C673B59FF0 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::CallObjectMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_CallObjectMethod_m953C16AD55D061D331B16060D9C2E7BEFFC34BB0 (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.Char UnityEngine.AndroidJNI::CallCharMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Il2CppChar AndroidJNI_CallCharMethod_mC5FEB28906B1F004D5EAE36363C2F2B32B4D25FD (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.Double UnityEngine.AndroidJNI::CallDoubleMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR double AndroidJNI_CallDoubleMethod_m391E75D42B6B445B80D751F56440DDE1C20A79EE (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.Single UnityEngine.AndroidJNI::CallFloatMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float AndroidJNI_CallFloatMethod_mFDB1FC58B999500B822E336ABB60408463FD9BAF (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.Int64 UnityEngine.AndroidJNI::CallLongMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t AndroidJNI_CallLongMethod_mF2B511CFE25949D688142C6A8A11973C22BE1AFC (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.Int16 UnityEngine.AndroidJNI::CallShortMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int16_t AndroidJNI_CallShortMethod_m1402B57DDA2B128398A7A911CDB24E06ED376D51 (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.SByte UnityEngine.AndroidJNI::CallSByteMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int8_t AndroidJNI_CallSByteMethod_m34A084018795E6E5847305390565A2A494AD2422 (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.Boolean UnityEngine.AndroidJNI::CallBooleanMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool AndroidJNI_CallBooleanMethod_mAE45802EE32D57194B47BC62E0AD9F8C56C41800 (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.Int32 UnityEngine.AndroidJNI::CallIntMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t AndroidJNI_CallIntMethod_m83AA9264B8978F8D42B4B5239CEDA616AD6FE047 (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.Char[] UnityEngine.AndroidJNI::FromCharArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* AndroidJNI_FromCharArray_mB24FA47F69D0B382F0D3F5F4B62F9B6F14F52842 (intptr_t ___array0, const RuntimeMethod* method);
// System.Double[] UnityEngine.AndroidJNI::FromDoubleArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* AndroidJNI_FromDoubleArray_m0994CF71AF7314249C12F3070FC50E048446D63E (intptr_t ___array0, const RuntimeMethod* method);
// System.Single[] UnityEngine.AndroidJNI::FromFloatArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* AndroidJNI_FromFloatArray_m5B41CA3BE4AAB40310042C0CFA624BFDBF1E15CB (intptr_t ___array0, const RuntimeMethod* method);
// System.Int64[] UnityEngine.AndroidJNI::FromLongArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F* AndroidJNI_FromLongArray_m5EDB9FD73EBB1F49486524B6A62B644D171A3CA4 (intptr_t ___array0, const RuntimeMethod* method);
// System.Int16[] UnityEngine.AndroidJNI::FromShortArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28* AndroidJNI_FromShortArray_m1084FF60F463C8EB3890406EEDBB9F1DFC80116B (intptr_t ___array0, const RuntimeMethod* method);
// System.Byte[] UnityEngine.AndroidJNI::FromByteArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* AndroidJNI_FromByteArray_mB1B0AC781BA50C8AE7F9A6B8660B7C3F6D7DDE02 (intptr_t ___array0, const RuntimeMethod* method);
// System.SByte[] UnityEngine.AndroidJNI::FromSByteArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889* AndroidJNI_FromSByteArray_m15A1A9366FC6A1952DA42809D8EEF59678ABF69E (intptr_t ___array0, const RuntimeMethod* method);
// System.Boolean[] UnityEngine.AndroidJNI::FromBooleanArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040* AndroidJNI_FromBooleanArray_mA5AF86E8FDA0D4B7CCA395E708527E2A1073AA86 (intptr_t ___array0, const RuntimeMethod* method);
// System.Int32[] UnityEngine.AndroidJNI::FromIntArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* AndroidJNI_FromIntArray_mD538A30307431BC4BEC75F3709701742131FE6F8 (intptr_t ___array0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::ToObjectArray(System.IntPtr[],System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_ToObjectArray_m0614CB442A041E1EE108ADF05676C001710EC33A (IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD* ___array0, intptr_t ___arrayClass1, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::ToCharArray(System.Char[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_ToCharArray_m2052C19FC000D01BA74DDAA7AC5EF8D4D13D1F6A (CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* ___array0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::ToDoubleArray(System.Double[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_ToDoubleArray_mB04386ABEC07D54732102A858B7F5250B49601CE (DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* ___array0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::ToFloatArray(System.Single[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_ToFloatArray_m684CAD369A3BDCE75B31FCC68F8CF7A1293A4533 (SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* ___array0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::ToLongArray(System.Int64[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_ToLongArray_mFAAAB30B9A9944A7D6A590ADE0ACB50A11656928 (Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F* ___array0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::ToShortArray(System.Int16[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_ToShortArray_m7FCED435AE3ACC7808F3CB9F9C5E8E16B616A316 (Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28* ___array0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::ToByteArray(System.Byte[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_ToByteArray_m57A1B1DD05FCA40796E0CFAA8297528E807CB5F4 (ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___array0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::ToSByteArray(System.SByte[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_ToSByteArray_mB78915C5C2948F80376765449650782802E03707 (SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889* ___array0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::ToBooleanArray(System.Boolean[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_ToBooleanArray_m7BEE0A1FEC1AAB4A244716CD93ABB456DC8E28C2 (BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040* ___array0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::ToIntArray(System.Int32[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_ToIntArray_mB69CEC2992884ADC394A9A7E604967B7B57651A9 (Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* ___array0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::GetObjectArrayElement(System.IntPtr,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_GetObjectArrayElement_m104E43629B8731ACAF53A5D351CCB19398A75648 (intptr_t ___array0, int32_t ___index1, const RuntimeMethod* method);
// System.Int32 UnityEngine.AndroidJNI::GetArrayLength(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t AndroidJNI_GetArrayLength_m3DD9BD96B89F86A4F8AAB10147CAADB951E49936 (intptr_t ___array0, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJavaObject::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaObject__ctor_m4C0CDAB96B807BB04E2C43609F16865034A60001 (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJavaClass::_AndroidJavaClass(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaClass__AndroidJavaClass_mBF3C92E82722125793A66F20C92BAE17F0CB02D9 (AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * __this, String_t* ___className0, const RuntimeMethod* method);
// System.Boolean System.IntPtr::op_Equality(System.IntPtr,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool IntPtr_op_Equality_mEE8D9FD2DFE312BBAA8B4ED3BF7976B3142A5934 (intptr_t ___value10, intptr_t ___value21, const RuntimeMethod* method);
// System.Void System.Exception::.ctor(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Exception__ctor_m89BADFF36C3B170013878726E07729D51AA9FBE0 (Exception_t * __this, String_t* ___message0, const RuntimeMethod* method);
// System.Void UnityEngine.GlobalJavaObjectRef::.ctor(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GlobalJavaObjectRef__ctor_m5581A68DC5217545E13F48ACF2DAFD9DF30396BC (GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * __this, intptr_t ___jobject0, const RuntimeMethod* method);
// System.String System.String::Concat(System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Concat_mB78D0094592718DA6D5DB6C712A9C225631666BE (String_t* ___str00, String_t* ___str11, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJavaObject::DebugPrint(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaObject_DebugPrint_m88F06202527BA5A2848C1533C8B396702D112531 (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___msg0, const RuntimeMethod* method);
// System.String System.String::Replace(System.Char,System.Char)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Replace_m276641366A463205C185A9B3DC0E24ECB95122C9 (String_t* __this, Il2CppChar ___oldChar0, Il2CppChar ___newChar1, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNISafe::FindClass(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_FindClass_mE58501828AA09ADC26347853AFE6D025845D487C (String_t* ___name0, const RuntimeMethod* method);
// System.String System.Exception::get_StackTrace()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* Exception_get_StackTrace_mF54AABBF2569597935F88AAF7BCD29C6639F8306 (Exception_t * __this, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJavaObject::_AndroidJavaObject(System.String,System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaObject__AndroidJavaObject_m596F928EE49384D7C7455920BA6ADFB2D9540CFA (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___className0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNISafe::GetObjectClass(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_GetObjectClass_mB36866622A9FD487DCA6926F63038E5584B35BFB (intptr_t ___ptr0, const RuntimeMethod* method);
// System.Void System.Object::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0 (RuntimeObject * __this, const RuntimeMethod* method);
// System.Void System.GC::SuppressFinalize(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GC_SuppressFinalize_m037319A9B95A5BA437E806DE592802225EE5B425 (RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJavaObject::_GetRawObject()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJavaObject__GetRawObject_m4B415E770E265AE32F5523DF0E627626F77E572F (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJavaObject::_GetRawClass()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJavaObject__GetRawClass_m1B3729CDBBC212E0C706256FF16D2F437F618435 (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Debug::Log(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Debug_Log_m4B7C70BAFD477C6BDB59C88A0934F0B018D03708 (RuntimeObject * ___message0, const RuntimeMethod* method);
// UnityEngine.jvalue[] UnityEngine.AndroidJNIHelper::CreateJNIArgArray(System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* AndroidJNIHelper_CreateJNIArgArray_mAA5972FD580D58FA3D30B4E97B9837B439231F34 (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.GlobalJavaObjectRef::op_Implicit(UnityEngine.GlobalJavaObjectRef)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t GlobalJavaObjectRef_op_Implicit_m1F52DE72C8F8B11E651F8B31879ED5AFD413EDFF (GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * ___obj0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNIHelper::GetConstructorID(System.IntPtr,System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNIHelper_GetConstructorID_m2756A393612A1CF86E3E73109E2268D9933F9F1E (intptr_t ___jclass0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNISafe::NewObject(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_NewObject_m78BDA85E651167163148C9B39DEA8CE831EB1DB0 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJNIHelper::DeleteJNIArgArray(System.Object[],UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNIHelper_DeleteJNIArgArray_mEDFD8275CF10A3E0777350597633378776673784 (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args0, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___jniArgs1, const RuntimeMethod* method);
// System.Void System.Object::Finalize()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object_Finalize_m4015B7D3A44DE125C5FE34D7276CD4697C06F380 (RuntimeObject * __this, const RuntimeMethod* method);
// System.Void UnityEngine.GlobalJavaObjectRef::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GlobalJavaObjectRef_Dispose_mDCFD34D040E7B4ACE886336F3659316D1A45599F (GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJavaObject::.ctor(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaObject__ctor_m22E1E2E5D9F3DA31FF7DFB1339AD3BB0C3813E80 (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, intptr_t ___jobject0, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJavaClass::.ctor(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaClass__ctor_m44A6DEC0612D768E9947FFC1C2DA64D0605F34F1 (AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * __this, intptr_t ___jclass0, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJavaClass::.ctor(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaClass__ctor_mAE416E812DB3911279C0FE87A7760247CE1BBFA8 (AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * __this, String_t* ___className0, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJavaProxy::.ctor(UnityEngine.AndroidJavaClass)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaProxy__ctor_m9A2D1F4BF0E7803070D68D3C386F4218D3BCAC0F (AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D * __this, AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * ___javaInterface0, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJNISafe::DeleteWeakGlobalRef(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNISafe_DeleteWeakGlobalRef_mB338C2F7116360905B7F444BDB16CAB18B914ED3 (intptr_t ___globalref0, const RuntimeMethod* method);
// System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Type_t * Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6 (RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  ___handle0, const RuntimeMethod* method);
// System.Type System.Object::GetType()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Type_t * Object_GetType_m2E0B62414ECCAA3094B703790CE88CBB2F83EA60 (RuntimeObject * __this, const RuntimeMethod* method);
// System.Reflection.MethodInfo System.Type::GetMethod(System.String,System.Reflection.BindingFlags,System.Reflection.Binder,System.Type[],System.Reflection.ParameterModifier[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR MethodInfo_t * Type_GetMethod_m694F07057F23808980BF6B1637544F34852759FA (Type_t * __this, String_t* ___name0, int32_t ___bindingAttr1, Binder_t4D5CB06963501D32847C057B57157D6DC49CA759 * ___binder2, TypeU5BU5D_t7FE623A666B49176DE123306221193E888A12F5F* ___types3, ParameterModifierU5BU5D_t63EC46F14F048DC9EF6BF1362E8AEBEA1A05A5EA* ___modifiers4, const RuntimeMethod* method);
// System.Object System.Reflection.MethodBase::Invoke(System.Object,System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * MethodBase_Invoke_m471794D56262D9DB5B5A324883030AB16BD39674 (MethodBase_t * __this, RuntimeObject * ___obj0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___parameters1, const RuntimeMethod* method);
// UnityEngine.AndroidJavaObject UnityEngine._AndroidJNIHelper::Box(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * _AndroidJNIHelper_Box_m67A2A786DCE5ADD2FAF4F27B7CA115C82A8768C2 (RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.Exception System.Exception::get_InnerException()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR Exception_t * Exception_get_InnerException_mCB68CC8CBF2540EF381CB17A4E4E3F6D0E33453F_inline (Exception_t * __this, const RuntimeMethod* method);
// System.String System.String::Join(System.String,System.String[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Join_m49371BED70248F0FCE970CB4F2E39E9A688AAFA4 (String_t* ___separator0, StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* ___value1, const RuntimeMethod* method);
// System.String System.String::Concat(System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Concat_mB7BA84F13912303B2E5E40FBF0109E1A328ACA07 (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args0, const RuntimeMethod* method);
// System.Void System.Reflection.TargetInvocationException::.ctor(System.String,System.Exception)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TargetInvocationException__ctor_mBCC339AE7AC683564DA27A950A92463915B71F00 (TargetInvocationException_t0DD35F6083E1D1E0509BF181A79C76D3339D89B8 * __this, String_t* ___message0, Exception_t * ___inner1, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJavaProxy::GetRawProxy()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJavaProxy_GetRawProxy_mFE7D48E72D4744E260D3ACE6D777D072002BEA6C (AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D * __this, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidReflection::SetNativeExceptionOnProxy(System.IntPtr,System.Exception,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidReflection_SetNativeExceptionOnProxy_m025AFCDD8B6659D45FE3830E8AC154300DA19966 (intptr_t ___proxy0, Exception_t * ___e1, bool ___methodNotFound2, const RuntimeMethod* method);
// System.Object UnityEngine._AndroidJNIHelper::Unbox(UnityEngine.AndroidJavaObject)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * _AndroidJNIHelper_Unbox_m813AFB8DE2C2568B011C81ED3AC4D013F1E5B67E (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * ___obj0, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJavaObject::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaObject_Dispose_m02D1B6D8F3E902E5F0D181BF6C1753856B0DE144 (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, const RuntimeMethod* method);
// UnityEngine.AndroidJavaObject UnityEngine.AndroidJavaObject::AndroidJavaObjectDeleteLocalRef(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * AndroidJavaObject_AndroidJavaObjectDeleteLocalRef_m0B0BCBDD56C299AC69938BDD4135E1B6EEAAC7EF (intptr_t ___jobject0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::NewLocalRef(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_NewLocalRef_m22674FDA13C73173E0ECB3F59DE15CBDAD4CD039 (intptr_t ___obj0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNIHelper::CreateJavaProxy(UnityEngine.AndroidJavaProxy)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNIHelper_CreateJavaProxy_m29A8BD91809FF21642EA1319E5F097979EE8FA28 (AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D * ___proxy0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::NewWeakGlobalRef(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_NewWeakGlobalRef_m907BCFA1475E108FBBD02A8A425929EC859D0E8C (intptr_t ___obj0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNIHelper::GetMethodID(System.IntPtr,System.String,System.String,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNIHelper_GetMethodID_mD3057EDF00D6BBB3E89116EE05F68D0731AD9E43 (intptr_t ___javaClass0, String_t* ___methodName1, String_t* ___signature2, bool ___isStatic3, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJavaProxy::.ctor(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaProxy__ctor_m159565DEF4041D92C0763D1F4A0684140241CD9A (AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D * __this, String_t* ___javaInterface0, const RuntimeMethod* method);
// System.Boolean System.Type::get_IsPrimitive()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Type_get_IsPrimitive_m8E39430EE4B70E1AE690B51E9BE681C7758DFF5A (Type_t * __this, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNISafe::GetStaticMethodID(System.IntPtr,System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_GetStaticMethodID_m4DCBC629048509F8E8566998CDA8F1AB9EAD6A50 (intptr_t ___clazz0, String_t* ___name1, String_t* ___sig2, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNISafe::NewString(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_NewString_mD1D954E0EE5A8F135B19EE67E8FF2A4E1A6CA97F (String_t* ___chars0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNISafe::CallStaticObjectMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_CallStaticObjectMethod_m11EDE005224D5A6833BFF896906397D24E19D440 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.Int64 System.IntPtr::ToInt64()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t IntPtr_ToInt64_mDD00D5F4AD380F40D31B60E9C57843CC3C12BD6B (intptr_t* __this, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJNISafe::CallStaticVoidMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNISafe_CallStaticVoidMethod_mC0BC9FA7E2FB69027E1F55E8810C6F619BCD7D59 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidReflection::GetStaticMethodID(System.String,System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidReflection_GetStaticMethodID_m1D6770C9A0BC1AA47FDA330B92743324C0441B29 (String_t* ___clazz0, String_t* ___methodName1, String_t* ___signature2, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::NewGlobalRef(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_NewGlobalRef_m1F7D16F896A4153CC36ADBACFD740D6453E2AB54 (intptr_t ___obj0, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJNISafe::DeleteGlobalRef(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNISafe_DeleteGlobalRef_mE0C851F30E3481496C72814973B66161C486D8BA (intptr_t ___globalref0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJavaObject::GetRawClass()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJavaObject_GetRawClass_m28BFE7AD6A4FFCB45929D9D1A0F8D792C3974774 (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidReflection::NewProxyInstance(System.IntPtr,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidReflection_NewProxyInstance_mEE0634E1963302B17FBAED127B581BFE4D228A8C (intptr_t ___delegateHandle0, intptr_t ___interfaze1, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJavaRunnableProxy::.ctor(UnityEngine.AndroidJavaRunnable)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaRunnableProxy__ctor_m0D23BFCE5D99EA0AA56A5813B2E91BDDAD72C738 (AndroidJavaRunnableProxy_t3C66FEA8C2A903168F2902788AB8AB29CA923308 * __this, AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 * ___runnable0, const RuntimeMethod* method);
// System.Int32 UnityEngine.AndroidJNISafe::GetArrayLength(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t AndroidJNISafe_GetArrayLength_m11614663772194842C0D75FB8C6FBDB92F8DEE05 (intptr_t ___array0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNISafe::GetObjectArrayElement(System.IntPtr,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_GetObjectArrayElement_mA87BFEFBCE1C7D1B5B817CCCB5D4B7F009FD37BD (intptr_t ___array0, int32_t ___index1, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJavaObject::GetRawObject()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJavaObject_GetRawObject_mCEB7EEC51D62A3E4F0D6F62C08CBEF008B556F3D (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, const RuntimeMethod* method);
// System.Int32 System.Array::GetLength(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t Array_GetLength_m318900B10C3A93A30ABDC67DE161C8F6ABA4D359 (RuntimeArray * __this, int32_t ___dimension0, const RuntimeMethod* method);
// System.Boolean UnityEngine.AndroidReflection::IsPrimitive(System.Type)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool AndroidReflection_IsPrimitive_m4C75B1AAEDD3FA0F73AFBC83CB374D3D8A9A3749 (Type_t * ___t0, const RuntimeMethod* method);
// System.Void UnityEngine.Debug::LogWarning(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Debug_LogWarning_m37338644DC81F640CCDFEAE35A223F0E965F0568 (RuntimeObject * ___message0, const RuntimeMethod* method);
// System.IntPtr UnityEngine._AndroidJNIHelper::ConvertToJNIArray(System.Array)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t _AndroidJNIHelper_ConvertToJNIArray_mBF20C1B6716BA00CA9C3825EA446B291E6D8EB20 (RuntimeArray * ___array0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNIHelper::CreateJavaRunnable(UnityEngine.AndroidJavaRunnable)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNIHelper_CreateJavaRunnable_mA6C7A0E1BEF771970126D0FB21FF6E95CF569ED8 (AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 * ___jrunnable0, const RuntimeMethod* method);
// System.String System.String::Concat(System.Object,System.Object,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Concat_m2E1F71C491D2429CC80A28745488FEA947BB7AAC (RuntimeObject * ___arg00, RuntimeObject * ___arg11, RuntimeObject * ___arg22, const RuntimeMethod* method);
// ReturnType UnityEngine.AndroidJavaObject::Call<UnityEngine.AndroidJavaObject>(System.String,System.Object[])
inline AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * AndroidJavaObject_Call_TisAndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_m775AB90594C5F27D6099ED61119EF3608FD1001D (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method)
{
	return ((  AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * (*) (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *, String_t*, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*, const RuntimeMethod*))AndroidJavaObject_Call_TisRuntimeObject_m38064E69DD787BA971B0757788FD11E7239A03B7_gshared)(__this, ___methodName0, ___args1, method);
}
// ReturnType UnityEngine.AndroidJavaObject::Call<System.String>(System.String,System.Object[])
inline String_t* AndroidJavaObject_Call_TisString_t_m5EAE53C9E2A8893FD8FEA710378D22C162A0FDEA (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method)
{
	return ((  String_t* (*) (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *, String_t*, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*, const RuntimeMethod*))AndroidJavaObject_Call_TisRuntimeObject_m38064E69DD787BA971B0757788FD11E7239A03B7_gshared)(__this, ___methodName0, ___args1, method);
}
// ReturnType UnityEngine.AndroidJavaObject::CallStatic<System.Int32>(System.String,System.Object[])
inline int32_t AndroidJavaObject_CallStatic_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m90D39A3F3725F8BD3F782614FA0101D563DA9CCF (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method)
{
	return ((  int32_t (*) (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *, String_t*, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*, const RuntimeMethod*))AndroidJavaObject_CallStatic_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m90D39A3F3725F8BD3F782614FA0101D563DA9CCF_gshared)(__this, ___methodName0, ___args1, method);
}
// ReturnType UnityEngine.AndroidJavaObject::Call<System.Boolean>(System.String,System.Object[])
inline bool AndroidJavaObject_Call_TisBoolean_tB53F6830F670160873277339AA58F15CAED4399C_m57EE1ACB271D15DD0E2DDD6B28805C31799A0976 (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method)
{
	return ((  bool (*) (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *, String_t*, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*, const RuntimeMethod*))AndroidJavaObject_Call_TisBoolean_tB53F6830F670160873277339AA58F15CAED4399C_m57EE1ACB271D15DD0E2DDD6B28805C31799A0976_gshared)(__this, ___methodName0, ___args1, method);
}
// System.Boolean System.String::op_Equality(System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE (String_t* ___a0, String_t* ___b1, const RuntimeMethod* method);
// System.String System.String::Concat(System.String,System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Concat_mF4626905368D6558695A823466A1AF65EADB9923 (String_t* ___str00, String_t* ___str11, String_t* ___str22, const RuntimeMethod* method);
// ReturnType UnityEngine.AndroidJavaObject::CallStatic<UnityEngine.AndroidJavaObject>(System.String,System.Object[])
inline AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * AndroidJavaObject_CallStatic_TisAndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_m6CAE75FB51C5A02521C239A7232735573C51EAE7 (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method)
{
	return ((  AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * (*) (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *, String_t*, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*, const RuntimeMethod*))AndroidJavaObject_CallStatic_TisRuntimeObject_mC00F70734976E6B3DD8281EB6EBC457B19762E9F_gshared)(__this, ___methodName0, ___args1, method);
}
// System.Void System.Array::SetValue(System.Object,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Array_SetValue_m3C6811CE9C45D1E461404B5D2FBD4EC1A054FDCA (RuntimeArray * __this, RuntimeObject * ___value0, int32_t ___index1, const RuntimeMethod* method);
// ReturnType UnityEngine.AndroidJavaObject::Call<System.Int32>(System.String,System.Object[])
inline int32_t AndroidJavaObject_Call_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_mF7220A3D48BA18737AA0C7DAF0828822275A69A6 (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method)
{
	return ((  int32_t (*) (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *, String_t*, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*, const RuntimeMethod*))AndroidJavaObject_Call_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_mF7220A3D48BA18737AA0C7DAF0828822275A69A6_gshared)(__this, ___methodName0, ___args1, method);
}
// ReturnType UnityEngine.AndroidJavaObject::Call<System.SByte>(System.String,System.Object[])
inline int8_t AndroidJavaObject_Call_TisSByte_t9070AEA2966184235653CB9B4D33B149CDA831DF_m1DA87DAFADCDA8DE62A86D5C1F94DF60F2F54651 (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method)
{
	return ((  int8_t (*) (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *, String_t*, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*, const RuntimeMethod*))AndroidJavaObject_Call_TisSByte_t9070AEA2966184235653CB9B4D33B149CDA831DF_m1DA87DAFADCDA8DE62A86D5C1F94DF60F2F54651_gshared)(__this, ___methodName0, ___args1, method);
}
// ReturnType UnityEngine.AndroidJavaObject::Call<System.Int16>(System.String,System.Object[])
inline int16_t AndroidJavaObject_Call_TisInt16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D_mB51ADF5CFAE5278F11CECE74CC8ABAA9B45BB34F (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method)
{
	return ((  int16_t (*) (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *, String_t*, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*, const RuntimeMethod*))AndroidJavaObject_Call_TisInt16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D_mB51ADF5CFAE5278F11CECE74CC8ABAA9B45BB34F_gshared)(__this, ___methodName0, ___args1, method);
}
// ReturnType UnityEngine.AndroidJavaObject::Call<System.Int64>(System.String,System.Object[])
inline int64_t AndroidJavaObject_Call_TisInt64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436_mCD42F5F94257CC748CBA517A16A7BCC707A0C440 (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method)
{
	return ((  int64_t (*) (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *, String_t*, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*, const RuntimeMethod*))AndroidJavaObject_Call_TisInt64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436_mCD42F5F94257CC748CBA517A16A7BCC707A0C440_gshared)(__this, ___methodName0, ___args1, method);
}
// ReturnType UnityEngine.AndroidJavaObject::Call<System.Single>(System.String,System.Object[])
inline float AndroidJavaObject_Call_TisSingle_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1_m241B6C5C3A0259B256071CA26CAFE3EF0F229DBA (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method)
{
	return ((  float (*) (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *, String_t*, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*, const RuntimeMethod*))AndroidJavaObject_Call_TisSingle_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1_m241B6C5C3A0259B256071CA26CAFE3EF0F229DBA_gshared)(__this, ___methodName0, ___args1, method);
}
// ReturnType UnityEngine.AndroidJavaObject::Call<System.Double>(System.String,System.Object[])
inline double AndroidJavaObject_Call_TisDouble_t358B8F23BDC52A5DD700E727E204F9F7CDE12409_mBDD67692E825B1F8834E22FC94628B9C6AE54C81 (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method)
{
	return ((  double (*) (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *, String_t*, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*, const RuntimeMethod*))AndroidJavaObject_Call_TisDouble_t358B8F23BDC52A5DD700E727E204F9F7CDE12409_mBDD67692E825B1F8834E22FC94628B9C6AE54C81_gshared)(__this, ___methodName0, ___args1, method);
}
// ReturnType UnityEngine.AndroidJavaObject::Call<System.Char>(System.String,System.Object[])
inline Il2CppChar AndroidJavaObject_Call_TisChar_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9_m73C43D18BEC4AF2416AC8ADA8FA26712645A0EEA (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method)
{
	return ((  Il2CppChar (*) (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *, String_t*, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*, const RuntimeMethod*))AndroidJavaObject_Call_TisChar_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9_m73C43D18BEC4AF2416AC8ADA8FA26712645A0EEA_gshared)(__this, ___methodName0, ___args1, method);
}
// System.Object UnityEngine._AndroidJNIHelper::UnboxArray(UnityEngine.AndroidJavaObject)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * _AndroidJNIHelper_UnboxArray_m57E035906F4D79FCAC155162AC491BB7B575956D (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * ___obj0, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJavaObject::.ctor(System.String,System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaObject__ctor_m39462EAD9AD82CBD90DEB4B7127F3D6C87A02BFA (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___className0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method);
// UnityEngine.AndroidJavaObject UnityEngine.AndroidJavaProxy::GetProxyObject()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * AndroidJavaProxy_GetProxyObject_m411DC59BF56152B6058ABF99BBC8B64C813EEF06 (AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D * __this, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNISafe::ToIntArray(System.Int32[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_ToIntArray_m324EDE9CCF1C9909444C40617BD3358172EFB874 (Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* ___array0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNISafe::ToBooleanArray(System.Boolean[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_ToBooleanArray_m1BCBD2041B6BFE6B91C1E3AD8C1133F791B70423 (BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040* ___array0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNISafe::ToByteArray(System.Byte[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_ToByteArray_m01C86D2FE9259F0888FA97B105FC741A0E2290D5 (ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___array0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNISafe::ToSByteArray(System.SByte[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_ToSByteArray_m5AE0F49EE17ABDCFBCDF619CBECD5DEF9961BDB8 (SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889* ___array0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNISafe::ToShortArray(System.Int16[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_ToShortArray_m7D79F918714300B5818C7C8646E4E1A48E056A07 (Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28* ___array0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNISafe::ToLongArray(System.Int64[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_ToLongArray_mD59D9304170DFB59B77342C994699BE445AF25D3 (Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F* ___array0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNISafe::ToFloatArray(System.Single[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_ToFloatArray_m8ACA5E42C6F32E7D851613AC129FB37AFC28EBFD (SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* ___array0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNISafe::ToDoubleArray(System.Double[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_ToDoubleArray_m9AE319DB92B91A255D2A0568D38B3B47CD0C69EB (DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* ___array0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNISafe::ToCharArray(System.Char[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_ToCharArray_m8AB18ECC188D1B8A15966FF3FBD7887CF35A5711 (CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* ___array0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNI::NewObjectArray(System.Int32,System.IntPtr,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_NewObjectArray_m49BBDBCC804A6799866B92D6E0DEA9A204B6BE43 (int32_t ___size0, intptr_t ___clazz1, intptr_t ___obj2, const RuntimeMethod* method);
// System.Void UnityEngine.AndroidJNI::SetObjectArrayElement(System.IntPtr,System.Int32,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNI_SetObjectArrayElement_m3CB77880BEEAA75E69813F5B193F07BDD8933418 (intptr_t ___array0, int32_t ___index1, intptr_t ___obj2, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNISafe::ToObjectArray(System.IntPtr[],System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_ToObjectArray_mB3A0EB74E8C47EB72667603D90A4DE2480E2AC63 (IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD* ___array0, intptr_t ___type1, const RuntimeMethod* method);
// System.String UnityEngine._AndroidJNIHelper::GetSignature(System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* _AndroidJNIHelper_GetSignature_m737340340A8C978F7AABB80DA4E31A8E700C73DA (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNIHelper::GetConstructorID(System.IntPtr,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNIHelper_GetConstructorID_m9978ECF944003B11786DDB1FDF0456CD89AF1180 (intptr_t ___javaClass0, String_t* ___signature1, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidReflection::GetConstructorMember(System.IntPtr,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidReflection_GetConstructorMember_mE78FA3844BBB2FE5A6D3A6719BE72BD33423F4C9 (intptr_t ___jclass0, String_t* ___signature1, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNISafe::FromReflectedMethod(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_FromReflectedMethod_m47AA20F4A2F8451B9BDCF8C6045802F04112F221 (intptr_t ___refMethod0, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidJNISafe::GetMethodID(System.IntPtr,System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_GetMethodID_m91CE11744503D04CD2AA8BAD99C914B1C2C6D494 (intptr_t ___obj0, String_t* ___name1, String_t* ___sig2, const RuntimeMethod* method);
// System.IntPtr UnityEngine.AndroidReflection::GetMethodMember(System.IntPtr,System.String,System.String,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidReflection_GetMethodMember_m0B7C41F91CA0414D70EDFF7853BA93B11157EB19 (intptr_t ___jclass0, String_t* ___methodName1, String_t* ___signature2, bool ___isStatic3, const RuntimeMethod* method);
// System.IntPtr UnityEngine._AndroidJNIHelper::GetMethodIDFallback(System.IntPtr,System.String,System.String,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t _AndroidJNIHelper_GetMethodIDFallback_m45AC36798A5258FE80A68A2453CE3C45792E2C95 (intptr_t ___jclass0, String_t* ___methodName1, String_t* ___signature2, bool ___isStatic3, const RuntimeMethod* method);
// System.Boolean UnityEngine.AndroidReflection::IsAssignableFrom(System.Type,System.Type)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool AndroidReflection_IsAssignableFrom_m000432044555172C9399EB05A11AA35BFAF790FD (Type_t * ___t0, Type_t * ___from1, const RuntimeMethod* method);
// System.Void System.Text.StringBuilder::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void StringBuilder__ctor_mF928376F82E8C8FF3C11842C562DB8CF28B2735E (StringBuilder_t * __this, const RuntimeMethod* method);
// System.Text.StringBuilder System.Text.StringBuilder::Append(System.Char)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR StringBuilder_t * StringBuilder_Append_m05C12F58ADC2D807613A9301DF438CB3CD09B75A (StringBuilder_t * __this, Il2CppChar ___value0, const RuntimeMethod* method);
// System.String UnityEngine._AndroidJNIHelper::GetSignature(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* _AndroidJNIHelper_GetSignature_m090B053BFD9A6AC7BBD0F2BFAE56A8188CE4D80B (RuntimeObject * ___obj0, const RuntimeMethod* method);
// System.Text.StringBuilder System.Text.StringBuilder::Append(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR StringBuilder_t * StringBuilder_Append_mDBB8CCBB7750C67BE2F2D92F47E6C0FA42793260 (StringBuilder_t * __this, String_t* ___value0, const RuntimeMethod* method);
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
// System.IntPtr UnityEngine.AndroidJNI::FindClass(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_FindClass_m07E2127D59F7EC97A06B5350699033448BD40CED (String_t* ___name0, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_FindClass_m07E2127D59F7EC97A06B5350699033448BD40CED_ftn) (String_t*);
	static AndroidJNI_FindClass_m07E2127D59F7EC97A06B5350699033448BD40CED_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_FindClass_m07E2127D59F7EC97A06B5350699033448BD40CED_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::FindClass(System.String)");
	intptr_t retVal = _il2cpp_icall_func(___name0);
	return retVal;
}
// System.IntPtr UnityEngine.AndroidJNI::FromReflectedMethod(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_FromReflectedMethod_m5F01D9D2E6FDB25E9DF3B8804FC6A536C71F84B9 (intptr_t ___refMethod0, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_FromReflectedMethod_m5F01D9D2E6FDB25E9DF3B8804FC6A536C71F84B9_ftn) (intptr_t);
	static AndroidJNI_FromReflectedMethod_m5F01D9D2E6FDB25E9DF3B8804FC6A536C71F84B9_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_FromReflectedMethod_m5F01D9D2E6FDB25E9DF3B8804FC6A536C71F84B9_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::FromReflectedMethod(System.IntPtr)");
	intptr_t retVal = _il2cpp_icall_func(___refMethod0);
	return retVal;
}
// System.IntPtr UnityEngine.AndroidJNI::ExceptionOccurred()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_ExceptionOccurred_mC2EC654C42E285C9E141393BDA41A4D8BC56FECD (const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_ExceptionOccurred_mC2EC654C42E285C9E141393BDA41A4D8BC56FECD_ftn) ();
	static AndroidJNI_ExceptionOccurred_mC2EC654C42E285C9E141393BDA41A4D8BC56FECD_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_ExceptionOccurred_mC2EC654C42E285C9E141393BDA41A4D8BC56FECD_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::ExceptionOccurred()");
	intptr_t retVal = _il2cpp_icall_func();
	return retVal;
}
// System.Void UnityEngine.AndroidJNI::ExceptionClear()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNI_ExceptionClear_m339CEFB228B0F08EBA289AED25464FF0D80B9936 (const RuntimeMethod* method)
{
	typedef void (*AndroidJNI_ExceptionClear_m339CEFB228B0F08EBA289AED25464FF0D80B9936_ftn) ();
	static AndroidJNI_ExceptionClear_m339CEFB228B0F08EBA289AED25464FF0D80B9936_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_ExceptionClear_m339CEFB228B0F08EBA289AED25464FF0D80B9936_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::ExceptionClear()");
	_il2cpp_icall_func();
}
// System.IntPtr UnityEngine.AndroidJNI::NewGlobalRef(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_NewGlobalRef_m1F7D16F896A4153CC36ADBACFD740D6453E2AB54 (intptr_t ___obj0, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_NewGlobalRef_m1F7D16F896A4153CC36ADBACFD740D6453E2AB54_ftn) (intptr_t);
	static AndroidJNI_NewGlobalRef_m1F7D16F896A4153CC36ADBACFD740D6453E2AB54_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_NewGlobalRef_m1F7D16F896A4153CC36ADBACFD740D6453E2AB54_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::NewGlobalRef(System.IntPtr)");
	intptr_t retVal = _il2cpp_icall_func(___obj0);
	return retVal;
}
// System.Void UnityEngine.AndroidJNI::DeleteGlobalRef(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNI_DeleteGlobalRef_mC800FCE93424A8778220806C3FE3497E21E94333 (intptr_t ___obj0, const RuntimeMethod* method)
{
	typedef void (*AndroidJNI_DeleteGlobalRef_mC800FCE93424A8778220806C3FE3497E21E94333_ftn) (intptr_t);
	static AndroidJNI_DeleteGlobalRef_mC800FCE93424A8778220806C3FE3497E21E94333_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_DeleteGlobalRef_mC800FCE93424A8778220806C3FE3497E21E94333_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::DeleteGlobalRef(System.IntPtr)");
	_il2cpp_icall_func(___obj0);
}
// System.IntPtr UnityEngine.AndroidJNI::NewWeakGlobalRef(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_NewWeakGlobalRef_m907BCFA1475E108FBBD02A8A425929EC859D0E8C (intptr_t ___obj0, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_NewWeakGlobalRef_m907BCFA1475E108FBBD02A8A425929EC859D0E8C_ftn) (intptr_t);
	static AndroidJNI_NewWeakGlobalRef_m907BCFA1475E108FBBD02A8A425929EC859D0E8C_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_NewWeakGlobalRef_m907BCFA1475E108FBBD02A8A425929EC859D0E8C_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::NewWeakGlobalRef(System.IntPtr)");
	intptr_t retVal = _il2cpp_icall_func(___obj0);
	return retVal;
}
// System.Void UnityEngine.AndroidJNI::DeleteWeakGlobalRef(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNI_DeleteWeakGlobalRef_m07AE954A94CDB58980A3CBA36E0E8F236BE01C75 (intptr_t ___obj0, const RuntimeMethod* method)
{
	typedef void (*AndroidJNI_DeleteWeakGlobalRef_m07AE954A94CDB58980A3CBA36E0E8F236BE01C75_ftn) (intptr_t);
	static AndroidJNI_DeleteWeakGlobalRef_m07AE954A94CDB58980A3CBA36E0E8F236BE01C75_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_DeleteWeakGlobalRef_m07AE954A94CDB58980A3CBA36E0E8F236BE01C75_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::DeleteWeakGlobalRef(System.IntPtr)");
	_il2cpp_icall_func(___obj0);
}
// System.IntPtr UnityEngine.AndroidJNI::NewLocalRef(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_NewLocalRef_m22674FDA13C73173E0ECB3F59DE15CBDAD4CD039 (intptr_t ___obj0, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_NewLocalRef_m22674FDA13C73173E0ECB3F59DE15CBDAD4CD039_ftn) (intptr_t);
	static AndroidJNI_NewLocalRef_m22674FDA13C73173E0ECB3F59DE15CBDAD4CD039_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_NewLocalRef_m22674FDA13C73173E0ECB3F59DE15CBDAD4CD039_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::NewLocalRef(System.IntPtr)");
	intptr_t retVal = _il2cpp_icall_func(___obj0);
	return retVal;
}
// System.Void UnityEngine.AndroidJNI::DeleteLocalRef(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNI_DeleteLocalRef_m5A7291640D0BB0F2A484C729CEDBF43F92B7941A (intptr_t ___obj0, const RuntimeMethod* method)
{
	typedef void (*AndroidJNI_DeleteLocalRef_m5A7291640D0BB0F2A484C729CEDBF43F92B7941A_ftn) (intptr_t);
	static AndroidJNI_DeleteLocalRef_m5A7291640D0BB0F2A484C729CEDBF43F92B7941A_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_DeleteLocalRef_m5A7291640D0BB0F2A484C729CEDBF43F92B7941A_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::DeleteLocalRef(System.IntPtr)");
	_il2cpp_icall_func(___obj0);
}
// System.IntPtr UnityEngine.AndroidJNI::NewObject(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_NewObject_mA1E19D3C530766C0E9F3196CB23A4C9E7795689B (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_NewObject_mA1E19D3C530766C0E9F3196CB23A4C9E7795689B_ftn) (intptr_t, intptr_t, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*);
	static AndroidJNI_NewObject_mA1E19D3C530766C0E9F3196CB23A4C9E7795689B_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_NewObject_mA1E19D3C530766C0E9F3196CB23A4C9E7795689B_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::NewObject(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])");
	intptr_t retVal = _il2cpp_icall_func(___clazz0, ___methodID1, ___args2);
	return retVal;
}
// System.IntPtr UnityEngine.AndroidJNI::GetObjectClass(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_GetObjectClass_mA9719B0A6734C4ED55B60B129A9D51F7B8A3B4A6 (intptr_t ___obj0, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_GetObjectClass_mA9719B0A6734C4ED55B60B129A9D51F7B8A3B4A6_ftn) (intptr_t);
	static AndroidJNI_GetObjectClass_mA9719B0A6734C4ED55B60B129A9D51F7B8A3B4A6_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_GetObjectClass_mA9719B0A6734C4ED55B60B129A9D51F7B8A3B4A6_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::GetObjectClass(System.IntPtr)");
	intptr_t retVal = _il2cpp_icall_func(___obj0);
	return retVal;
}
// System.IntPtr UnityEngine.AndroidJNI::GetMethodID(System.IntPtr,System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_GetMethodID_m4D7386D69FFEF80467F1804447C094B59385AF0C (intptr_t ___clazz0, String_t* ___name1, String_t* ___sig2, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_GetMethodID_m4D7386D69FFEF80467F1804447C094B59385AF0C_ftn) (intptr_t, String_t*, String_t*);
	static AndroidJNI_GetMethodID_m4D7386D69FFEF80467F1804447C094B59385AF0C_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_GetMethodID_m4D7386D69FFEF80467F1804447C094B59385AF0C_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::GetMethodID(System.IntPtr,System.String,System.String)");
	intptr_t retVal = _il2cpp_icall_func(___clazz0, ___name1, ___sig2);
	return retVal;
}
// System.IntPtr UnityEngine.AndroidJNI::GetStaticMethodID(System.IntPtr,System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_GetStaticMethodID_m135C9DEFFC207E509C001370C227F6E217FD9A1C (intptr_t ___clazz0, String_t* ___name1, String_t* ___sig2, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_GetStaticMethodID_m135C9DEFFC207E509C001370C227F6E217FD9A1C_ftn) (intptr_t, String_t*, String_t*);
	static AndroidJNI_GetStaticMethodID_m135C9DEFFC207E509C001370C227F6E217FD9A1C_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_GetStaticMethodID_m135C9DEFFC207E509C001370C227F6E217FD9A1C_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::GetStaticMethodID(System.IntPtr,System.String,System.String)");
	intptr_t retVal = _il2cpp_icall_func(___clazz0, ___name1, ___sig2);
	return retVal;
}
// System.IntPtr UnityEngine.AndroidJNI::NewString(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_NewString_m4B505016C60A4B2602F2037983367C2DB52A8BE2 (String_t* ___chars0, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		String_t* L_0 = ___chars0;
		intptr_t L_1 = AndroidJNI_NewStringFromStr_m01AAA91EC40C908302162C5653D6AFEFC384BBA9(L_0, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_1;
		goto IL_000d;
	}

IL_000d:
	{
		intptr_t L_2 = V_0;
		return (intptr_t)L_2;
	}
}
// System.IntPtr UnityEngine.AndroidJNI::NewStringFromStr(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_NewStringFromStr_m01AAA91EC40C908302162C5653D6AFEFC384BBA9 (String_t* ___chars0, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_NewStringFromStr_m01AAA91EC40C908302162C5653D6AFEFC384BBA9_ftn) (String_t*);
	static AndroidJNI_NewStringFromStr_m01AAA91EC40C908302162C5653D6AFEFC384BBA9_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_NewStringFromStr_m01AAA91EC40C908302162C5653D6AFEFC384BBA9_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::NewStringFromStr(System.String)");
	intptr_t retVal = _il2cpp_icall_func(___chars0);
	return retVal;
}
// System.String UnityEngine.AndroidJNI::GetStringChars(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* AndroidJNI_GetStringChars_m1C44DAAF9B7AA8E9586F1CD236E825B07741A268 (intptr_t ___str0, const RuntimeMethod* method)
{
	typedef String_t* (*AndroidJNI_GetStringChars_m1C44DAAF9B7AA8E9586F1CD236E825B07741A268_ftn) (intptr_t);
	static AndroidJNI_GetStringChars_m1C44DAAF9B7AA8E9586F1CD236E825B07741A268_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_GetStringChars_m1C44DAAF9B7AA8E9586F1CD236E825B07741A268_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::GetStringChars(System.IntPtr)");
	String_t* retVal = _il2cpp_icall_func(___str0);
	return retVal;
}
// System.String UnityEngine.AndroidJNI::CallStringMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* AndroidJNI_CallStringMethod_m3322E22FCA053618D794A9F3D00CFA1368F10AA9 (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	typedef String_t* (*AndroidJNI_CallStringMethod_m3322E22FCA053618D794A9F3D00CFA1368F10AA9_ftn) (intptr_t, intptr_t, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*);
	static AndroidJNI_CallStringMethod_m3322E22FCA053618D794A9F3D00CFA1368F10AA9_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_CallStringMethod_m3322E22FCA053618D794A9F3D00CFA1368F10AA9_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::CallStringMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])");
	String_t* retVal = _il2cpp_icall_func(___obj0, ___methodID1, ___args2);
	return retVal;
}
// System.IntPtr UnityEngine.AndroidJNI::CallObjectMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_CallObjectMethod_m953C16AD55D061D331B16060D9C2E7BEFFC34BB0 (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_CallObjectMethod_m953C16AD55D061D331B16060D9C2E7BEFFC34BB0_ftn) (intptr_t, intptr_t, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*);
	static AndroidJNI_CallObjectMethod_m953C16AD55D061D331B16060D9C2E7BEFFC34BB0_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_CallObjectMethod_m953C16AD55D061D331B16060D9C2E7BEFFC34BB0_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::CallObjectMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])");
	intptr_t retVal = _il2cpp_icall_func(___obj0, ___methodID1, ___args2);
	return retVal;
}
// System.Int32 UnityEngine.AndroidJNI::CallIntMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t AndroidJNI_CallIntMethod_m83AA9264B8978F8D42B4B5239CEDA616AD6FE047 (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	typedef int32_t (*AndroidJNI_CallIntMethod_m83AA9264B8978F8D42B4B5239CEDA616AD6FE047_ftn) (intptr_t, intptr_t, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*);
	static AndroidJNI_CallIntMethod_m83AA9264B8978F8D42B4B5239CEDA616AD6FE047_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_CallIntMethod_m83AA9264B8978F8D42B4B5239CEDA616AD6FE047_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::CallIntMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])");
	int32_t retVal = _il2cpp_icall_func(___obj0, ___methodID1, ___args2);
	return retVal;
}
// System.Boolean UnityEngine.AndroidJNI::CallBooleanMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool AndroidJNI_CallBooleanMethod_mAE45802EE32D57194B47BC62E0AD9F8C56C41800 (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	typedef bool (*AndroidJNI_CallBooleanMethod_mAE45802EE32D57194B47BC62E0AD9F8C56C41800_ftn) (intptr_t, intptr_t, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*);
	static AndroidJNI_CallBooleanMethod_mAE45802EE32D57194B47BC62E0AD9F8C56C41800_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_CallBooleanMethod_mAE45802EE32D57194B47BC62E0AD9F8C56C41800_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::CallBooleanMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])");
	bool retVal = _il2cpp_icall_func(___obj0, ___methodID1, ___args2);
	return retVal;
}
// System.Int16 UnityEngine.AndroidJNI::CallShortMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int16_t AndroidJNI_CallShortMethod_m1402B57DDA2B128398A7A911CDB24E06ED376D51 (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	typedef int16_t (*AndroidJNI_CallShortMethod_m1402B57DDA2B128398A7A911CDB24E06ED376D51_ftn) (intptr_t, intptr_t, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*);
	static AndroidJNI_CallShortMethod_m1402B57DDA2B128398A7A911CDB24E06ED376D51_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_CallShortMethod_m1402B57DDA2B128398A7A911CDB24E06ED376D51_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::CallShortMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])");
	int16_t retVal = _il2cpp_icall_func(___obj0, ___methodID1, ___args2);
	return retVal;
}
// System.SByte UnityEngine.AndroidJNI::CallSByteMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int8_t AndroidJNI_CallSByteMethod_m34A084018795E6E5847305390565A2A494AD2422 (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	typedef int8_t (*AndroidJNI_CallSByteMethod_m34A084018795E6E5847305390565A2A494AD2422_ftn) (intptr_t, intptr_t, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*);
	static AndroidJNI_CallSByteMethod_m34A084018795E6E5847305390565A2A494AD2422_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_CallSByteMethod_m34A084018795E6E5847305390565A2A494AD2422_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::CallSByteMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])");
	int8_t retVal = _il2cpp_icall_func(___obj0, ___methodID1, ___args2);
	return retVal;
}
// System.Char UnityEngine.AndroidJNI::CallCharMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Il2CppChar AndroidJNI_CallCharMethod_mC5FEB28906B1F004D5EAE36363C2F2B32B4D25FD (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	typedef Il2CppChar (*AndroidJNI_CallCharMethod_mC5FEB28906B1F004D5EAE36363C2F2B32B4D25FD_ftn) (intptr_t, intptr_t, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*);
	static AndroidJNI_CallCharMethod_mC5FEB28906B1F004D5EAE36363C2F2B32B4D25FD_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_CallCharMethod_mC5FEB28906B1F004D5EAE36363C2F2B32B4D25FD_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::CallCharMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])");
	Il2CppChar retVal = _il2cpp_icall_func(___obj0, ___methodID1, ___args2);
	return retVal;
}
// System.Single UnityEngine.AndroidJNI::CallFloatMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float AndroidJNI_CallFloatMethod_mFDB1FC58B999500B822E336ABB60408463FD9BAF (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	typedef float (*AndroidJNI_CallFloatMethod_mFDB1FC58B999500B822E336ABB60408463FD9BAF_ftn) (intptr_t, intptr_t, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*);
	static AndroidJNI_CallFloatMethod_mFDB1FC58B999500B822E336ABB60408463FD9BAF_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_CallFloatMethod_mFDB1FC58B999500B822E336ABB60408463FD9BAF_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::CallFloatMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])");
	float retVal = _il2cpp_icall_func(___obj0, ___methodID1, ___args2);
	return retVal;
}
// System.Double UnityEngine.AndroidJNI::CallDoubleMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR double AndroidJNI_CallDoubleMethod_m391E75D42B6B445B80D751F56440DDE1C20A79EE (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	typedef double (*AndroidJNI_CallDoubleMethod_m391E75D42B6B445B80D751F56440DDE1C20A79EE_ftn) (intptr_t, intptr_t, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*);
	static AndroidJNI_CallDoubleMethod_m391E75D42B6B445B80D751F56440DDE1C20A79EE_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_CallDoubleMethod_m391E75D42B6B445B80D751F56440DDE1C20A79EE_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::CallDoubleMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])");
	double retVal = _il2cpp_icall_func(___obj0, ___methodID1, ___args2);
	return retVal;
}
// System.Int64 UnityEngine.AndroidJNI::CallLongMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t AndroidJNI_CallLongMethod_mF2B511CFE25949D688142C6A8A11973C22BE1AFC (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	typedef int64_t (*AndroidJNI_CallLongMethod_mF2B511CFE25949D688142C6A8A11973C22BE1AFC_ftn) (intptr_t, intptr_t, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*);
	static AndroidJNI_CallLongMethod_mF2B511CFE25949D688142C6A8A11973C22BE1AFC_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_CallLongMethod_mF2B511CFE25949D688142C6A8A11973C22BE1AFC_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::CallLongMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])");
	int64_t retVal = _il2cpp_icall_func(___obj0, ___methodID1, ___args2);
	return retVal;
}
// System.String UnityEngine.AndroidJNI::CallStaticStringMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* AndroidJNI_CallStaticStringMethod_m7502E60348B62159AE2F0C06D3D663E6E1F28116 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	typedef String_t* (*AndroidJNI_CallStaticStringMethod_m7502E60348B62159AE2F0C06D3D663E6E1F28116_ftn) (intptr_t, intptr_t, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*);
	static AndroidJNI_CallStaticStringMethod_m7502E60348B62159AE2F0C06D3D663E6E1F28116_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_CallStaticStringMethod_m7502E60348B62159AE2F0C06D3D663E6E1F28116_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::CallStaticStringMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])");
	String_t* retVal = _il2cpp_icall_func(___clazz0, ___methodID1, ___args2);
	return retVal;
}
// System.IntPtr UnityEngine.AndroidJNI::CallStaticObjectMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_CallStaticObjectMethod_m8540B678387A3DE6F1F702CF3053826962F569C0 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_CallStaticObjectMethod_m8540B678387A3DE6F1F702CF3053826962F569C0_ftn) (intptr_t, intptr_t, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*);
	static AndroidJNI_CallStaticObjectMethod_m8540B678387A3DE6F1F702CF3053826962F569C0_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_CallStaticObjectMethod_m8540B678387A3DE6F1F702CF3053826962F569C0_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::CallStaticObjectMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])");
	intptr_t retVal = _il2cpp_icall_func(___clazz0, ___methodID1, ___args2);
	return retVal;
}
// System.Int32 UnityEngine.AndroidJNI::CallStaticIntMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t AndroidJNI_CallStaticIntMethod_mC112D86B8844819C4D02AA8136BCF8C673B59FF0 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	typedef int32_t (*AndroidJNI_CallStaticIntMethod_mC112D86B8844819C4D02AA8136BCF8C673B59FF0_ftn) (intptr_t, intptr_t, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*);
	static AndroidJNI_CallStaticIntMethod_mC112D86B8844819C4D02AA8136BCF8C673B59FF0_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_CallStaticIntMethod_mC112D86B8844819C4D02AA8136BCF8C673B59FF0_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::CallStaticIntMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])");
	int32_t retVal = _il2cpp_icall_func(___clazz0, ___methodID1, ___args2);
	return retVal;
}
// System.Boolean UnityEngine.AndroidJNI::CallStaticBooleanMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool AndroidJNI_CallStaticBooleanMethod_mA5C4F5D3A724351C0DB569E863F070493E86069F (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	typedef bool (*AndroidJNI_CallStaticBooleanMethod_mA5C4F5D3A724351C0DB569E863F070493E86069F_ftn) (intptr_t, intptr_t, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*);
	static AndroidJNI_CallStaticBooleanMethod_mA5C4F5D3A724351C0DB569E863F070493E86069F_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_CallStaticBooleanMethod_mA5C4F5D3A724351C0DB569E863F070493E86069F_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::CallStaticBooleanMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])");
	bool retVal = _il2cpp_icall_func(___clazz0, ___methodID1, ___args2);
	return retVal;
}
// System.Int16 UnityEngine.AndroidJNI::CallStaticShortMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int16_t AndroidJNI_CallStaticShortMethod_m1BC0BA260F59800529D511D0E51B501165056F3F (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	typedef int16_t (*AndroidJNI_CallStaticShortMethod_m1BC0BA260F59800529D511D0E51B501165056F3F_ftn) (intptr_t, intptr_t, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*);
	static AndroidJNI_CallStaticShortMethod_m1BC0BA260F59800529D511D0E51B501165056F3F_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_CallStaticShortMethod_m1BC0BA260F59800529D511D0E51B501165056F3F_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::CallStaticShortMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])");
	int16_t retVal = _il2cpp_icall_func(___clazz0, ___methodID1, ___args2);
	return retVal;
}
// System.SByte UnityEngine.AndroidJNI::CallStaticSByteMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int8_t AndroidJNI_CallStaticSByteMethod_m637357610E5ECF91256FD6EFA48468D276395F46 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	typedef int8_t (*AndroidJNI_CallStaticSByteMethod_m637357610E5ECF91256FD6EFA48468D276395F46_ftn) (intptr_t, intptr_t, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*);
	static AndroidJNI_CallStaticSByteMethod_m637357610E5ECF91256FD6EFA48468D276395F46_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_CallStaticSByteMethod_m637357610E5ECF91256FD6EFA48468D276395F46_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::CallStaticSByteMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])");
	int8_t retVal = _il2cpp_icall_func(___clazz0, ___methodID1, ___args2);
	return retVal;
}
// System.Char UnityEngine.AndroidJNI::CallStaticCharMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Il2CppChar AndroidJNI_CallStaticCharMethod_m03968EDD820122C5AA74D396578D5C8F747DE8B9 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	typedef Il2CppChar (*AndroidJNI_CallStaticCharMethod_m03968EDD820122C5AA74D396578D5C8F747DE8B9_ftn) (intptr_t, intptr_t, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*);
	static AndroidJNI_CallStaticCharMethod_m03968EDD820122C5AA74D396578D5C8F747DE8B9_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_CallStaticCharMethod_m03968EDD820122C5AA74D396578D5C8F747DE8B9_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::CallStaticCharMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])");
	Il2CppChar retVal = _il2cpp_icall_func(___clazz0, ___methodID1, ___args2);
	return retVal;
}
// System.Single UnityEngine.AndroidJNI::CallStaticFloatMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float AndroidJNI_CallStaticFloatMethod_m22FE454F030F117CFA7CE8F8CE55A4DD9EB226DD (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	typedef float (*AndroidJNI_CallStaticFloatMethod_m22FE454F030F117CFA7CE8F8CE55A4DD9EB226DD_ftn) (intptr_t, intptr_t, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*);
	static AndroidJNI_CallStaticFloatMethod_m22FE454F030F117CFA7CE8F8CE55A4DD9EB226DD_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_CallStaticFloatMethod_m22FE454F030F117CFA7CE8F8CE55A4DD9EB226DD_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::CallStaticFloatMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])");
	float retVal = _il2cpp_icall_func(___clazz0, ___methodID1, ___args2);
	return retVal;
}
// System.Double UnityEngine.AndroidJNI::CallStaticDoubleMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR double AndroidJNI_CallStaticDoubleMethod_mB27665BD677D31470812D5E4FA466259D18D8D67 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	typedef double (*AndroidJNI_CallStaticDoubleMethod_mB27665BD677D31470812D5E4FA466259D18D8D67_ftn) (intptr_t, intptr_t, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*);
	static AndroidJNI_CallStaticDoubleMethod_mB27665BD677D31470812D5E4FA466259D18D8D67_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_CallStaticDoubleMethod_mB27665BD677D31470812D5E4FA466259D18D8D67_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::CallStaticDoubleMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])");
	double retVal = _il2cpp_icall_func(___clazz0, ___methodID1, ___args2);
	return retVal;
}
// System.Int64 UnityEngine.AndroidJNI::CallStaticLongMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t AndroidJNI_CallStaticLongMethod_mACA1CFC943C54BB656D065AB6EF0A78FE3EEC014 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	typedef int64_t (*AndroidJNI_CallStaticLongMethod_mACA1CFC943C54BB656D065AB6EF0A78FE3EEC014_ftn) (intptr_t, intptr_t, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*);
	static AndroidJNI_CallStaticLongMethod_mACA1CFC943C54BB656D065AB6EF0A78FE3EEC014_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_CallStaticLongMethod_mACA1CFC943C54BB656D065AB6EF0A78FE3EEC014_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::CallStaticLongMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])");
	int64_t retVal = _il2cpp_icall_func(___clazz0, ___methodID1, ___args2);
	return retVal;
}
// System.Void UnityEngine.AndroidJNI::CallStaticVoidMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNI_CallStaticVoidMethod_m973B08F0CE8068F0AC8A8FF85F0C63FD5AC3EAFA (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	typedef void (*AndroidJNI_CallStaticVoidMethod_m973B08F0CE8068F0AC8A8FF85F0C63FD5AC3EAFA_ftn) (intptr_t, intptr_t, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*);
	static AndroidJNI_CallStaticVoidMethod_m973B08F0CE8068F0AC8A8FF85F0C63FD5AC3EAFA_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_CallStaticVoidMethod_m973B08F0CE8068F0AC8A8FF85F0C63FD5AC3EAFA_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::CallStaticVoidMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])");
	_il2cpp_icall_func(___clazz0, ___methodID1, ___args2);
}
// System.IntPtr UnityEngine.AndroidJNI::ToBooleanArray(System.Boolean[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_ToBooleanArray_m7BEE0A1FEC1AAB4A244716CD93ABB456DC8E28C2 (BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040* ___array0, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_ToBooleanArray_m7BEE0A1FEC1AAB4A244716CD93ABB456DC8E28C2_ftn) (BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040*);
	static AndroidJNI_ToBooleanArray_m7BEE0A1FEC1AAB4A244716CD93ABB456DC8E28C2_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_ToBooleanArray_m7BEE0A1FEC1AAB4A244716CD93ABB456DC8E28C2_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::ToBooleanArray(System.Boolean[])");
	intptr_t retVal = _il2cpp_icall_func(___array0);
	return retVal;
}
// System.IntPtr UnityEngine.AndroidJNI::ToByteArray(System.Byte[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_ToByteArray_m57A1B1DD05FCA40796E0CFAA8297528E807CB5F4 (ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___array0, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_ToByteArray_m57A1B1DD05FCA40796E0CFAA8297528E807CB5F4_ftn) (ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821*);
	static AndroidJNI_ToByteArray_m57A1B1DD05FCA40796E0CFAA8297528E807CB5F4_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_ToByteArray_m57A1B1DD05FCA40796E0CFAA8297528E807CB5F4_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::ToByteArray(System.Byte[])");
	intptr_t retVal = _il2cpp_icall_func(___array0);
	return retVal;
}
// System.IntPtr UnityEngine.AndroidJNI::ToSByteArray(System.SByte[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_ToSByteArray_mB78915C5C2948F80376765449650782802E03707 (SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889* ___array0, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_ToSByteArray_mB78915C5C2948F80376765449650782802E03707_ftn) (SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889*);
	static AndroidJNI_ToSByteArray_mB78915C5C2948F80376765449650782802E03707_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_ToSByteArray_mB78915C5C2948F80376765449650782802E03707_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::ToSByteArray(System.SByte[])");
	intptr_t retVal = _il2cpp_icall_func(___array0);
	return retVal;
}
// System.IntPtr UnityEngine.AndroidJNI::ToCharArray(System.Char[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_ToCharArray_m2052C19FC000D01BA74DDAA7AC5EF8D4D13D1F6A (CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* ___array0, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_ToCharArray_m2052C19FC000D01BA74DDAA7AC5EF8D4D13D1F6A_ftn) (CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2*);
	static AndroidJNI_ToCharArray_m2052C19FC000D01BA74DDAA7AC5EF8D4D13D1F6A_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_ToCharArray_m2052C19FC000D01BA74DDAA7AC5EF8D4D13D1F6A_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::ToCharArray(System.Char[])");
	intptr_t retVal = _il2cpp_icall_func(___array0);
	return retVal;
}
// System.IntPtr UnityEngine.AndroidJNI::ToShortArray(System.Int16[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_ToShortArray_m7FCED435AE3ACC7808F3CB9F9C5E8E16B616A316 (Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28* ___array0, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_ToShortArray_m7FCED435AE3ACC7808F3CB9F9C5E8E16B616A316_ftn) (Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28*);
	static AndroidJNI_ToShortArray_m7FCED435AE3ACC7808F3CB9F9C5E8E16B616A316_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_ToShortArray_m7FCED435AE3ACC7808F3CB9F9C5E8E16B616A316_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::ToShortArray(System.Int16[])");
	intptr_t retVal = _il2cpp_icall_func(___array0);
	return retVal;
}
// System.IntPtr UnityEngine.AndroidJNI::ToIntArray(System.Int32[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_ToIntArray_mB69CEC2992884ADC394A9A7E604967B7B57651A9 (Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* ___array0, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_ToIntArray_mB69CEC2992884ADC394A9A7E604967B7B57651A9_ftn) (Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83*);
	static AndroidJNI_ToIntArray_mB69CEC2992884ADC394A9A7E604967B7B57651A9_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_ToIntArray_mB69CEC2992884ADC394A9A7E604967B7B57651A9_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::ToIntArray(System.Int32[])");
	intptr_t retVal = _il2cpp_icall_func(___array0);
	return retVal;
}
// System.IntPtr UnityEngine.AndroidJNI::ToLongArray(System.Int64[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_ToLongArray_mFAAAB30B9A9944A7D6A590ADE0ACB50A11656928 (Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F* ___array0, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_ToLongArray_mFAAAB30B9A9944A7D6A590ADE0ACB50A11656928_ftn) (Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F*);
	static AndroidJNI_ToLongArray_mFAAAB30B9A9944A7D6A590ADE0ACB50A11656928_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_ToLongArray_mFAAAB30B9A9944A7D6A590ADE0ACB50A11656928_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::ToLongArray(System.Int64[])");
	intptr_t retVal = _il2cpp_icall_func(___array0);
	return retVal;
}
// System.IntPtr UnityEngine.AndroidJNI::ToFloatArray(System.Single[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_ToFloatArray_m684CAD369A3BDCE75B31FCC68F8CF7A1293A4533 (SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* ___array0, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_ToFloatArray_m684CAD369A3BDCE75B31FCC68F8CF7A1293A4533_ftn) (SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*);
	static AndroidJNI_ToFloatArray_m684CAD369A3BDCE75B31FCC68F8CF7A1293A4533_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_ToFloatArray_m684CAD369A3BDCE75B31FCC68F8CF7A1293A4533_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::ToFloatArray(System.Single[])");
	intptr_t retVal = _il2cpp_icall_func(___array0);
	return retVal;
}
// System.IntPtr UnityEngine.AndroidJNI::ToDoubleArray(System.Double[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_ToDoubleArray_mB04386ABEC07D54732102A858B7F5250B49601CE (DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* ___array0, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_ToDoubleArray_mB04386ABEC07D54732102A858B7F5250B49601CE_ftn) (DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D*);
	static AndroidJNI_ToDoubleArray_mB04386ABEC07D54732102A858B7F5250B49601CE_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_ToDoubleArray_mB04386ABEC07D54732102A858B7F5250B49601CE_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::ToDoubleArray(System.Double[])");
	intptr_t retVal = _il2cpp_icall_func(___array0);
	return retVal;
}
// System.IntPtr UnityEngine.AndroidJNI::ToObjectArray(System.IntPtr[],System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_ToObjectArray_m0614CB442A041E1EE108ADF05676C001710EC33A (IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD* ___array0, intptr_t ___arrayClass1, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_ToObjectArray_m0614CB442A041E1EE108ADF05676C001710EC33A_ftn) (IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD*, intptr_t);
	static AndroidJNI_ToObjectArray_m0614CB442A041E1EE108ADF05676C001710EC33A_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_ToObjectArray_m0614CB442A041E1EE108ADF05676C001710EC33A_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::ToObjectArray(System.IntPtr[],System.IntPtr)");
	intptr_t retVal = _il2cpp_icall_func(___array0, ___arrayClass1);
	return retVal;
}
// System.Boolean[] UnityEngine.AndroidJNI::FromBooleanArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040* AndroidJNI_FromBooleanArray_mA5AF86E8FDA0D4B7CCA395E708527E2A1073AA86 (intptr_t ___array0, const RuntimeMethod* method)
{
	typedef BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040* (*AndroidJNI_FromBooleanArray_mA5AF86E8FDA0D4B7CCA395E708527E2A1073AA86_ftn) (intptr_t);
	static AndroidJNI_FromBooleanArray_mA5AF86E8FDA0D4B7CCA395E708527E2A1073AA86_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_FromBooleanArray_mA5AF86E8FDA0D4B7CCA395E708527E2A1073AA86_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::FromBooleanArray(System.IntPtr)");
	BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040* retVal = _il2cpp_icall_func(___array0);
	return retVal;
}
// System.Byte[] UnityEngine.AndroidJNI::FromByteArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* AndroidJNI_FromByteArray_mB1B0AC781BA50C8AE7F9A6B8660B7C3F6D7DDE02 (intptr_t ___array0, const RuntimeMethod* method)
{
	typedef ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* (*AndroidJNI_FromByteArray_mB1B0AC781BA50C8AE7F9A6B8660B7C3F6D7DDE02_ftn) (intptr_t);
	static AndroidJNI_FromByteArray_mB1B0AC781BA50C8AE7F9A6B8660B7C3F6D7DDE02_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_FromByteArray_mB1B0AC781BA50C8AE7F9A6B8660B7C3F6D7DDE02_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::FromByteArray(System.IntPtr)");
	ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* retVal = _il2cpp_icall_func(___array0);
	return retVal;
}
// System.SByte[] UnityEngine.AndroidJNI::FromSByteArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889* AndroidJNI_FromSByteArray_m15A1A9366FC6A1952DA42809D8EEF59678ABF69E (intptr_t ___array0, const RuntimeMethod* method)
{
	typedef SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889* (*AndroidJNI_FromSByteArray_m15A1A9366FC6A1952DA42809D8EEF59678ABF69E_ftn) (intptr_t);
	static AndroidJNI_FromSByteArray_m15A1A9366FC6A1952DA42809D8EEF59678ABF69E_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_FromSByteArray_m15A1A9366FC6A1952DA42809D8EEF59678ABF69E_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::FromSByteArray(System.IntPtr)");
	SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889* retVal = _il2cpp_icall_func(___array0);
	return retVal;
}
// System.Char[] UnityEngine.AndroidJNI::FromCharArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* AndroidJNI_FromCharArray_mB24FA47F69D0B382F0D3F5F4B62F9B6F14F52842 (intptr_t ___array0, const RuntimeMethod* method)
{
	typedef CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* (*AndroidJNI_FromCharArray_mB24FA47F69D0B382F0D3F5F4B62F9B6F14F52842_ftn) (intptr_t);
	static AndroidJNI_FromCharArray_mB24FA47F69D0B382F0D3F5F4B62F9B6F14F52842_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_FromCharArray_mB24FA47F69D0B382F0D3F5F4B62F9B6F14F52842_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::FromCharArray(System.IntPtr)");
	CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* retVal = _il2cpp_icall_func(___array0);
	return retVal;
}
// System.Int16[] UnityEngine.AndroidJNI::FromShortArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28* AndroidJNI_FromShortArray_m1084FF60F463C8EB3890406EEDBB9F1DFC80116B (intptr_t ___array0, const RuntimeMethod* method)
{
	typedef Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28* (*AndroidJNI_FromShortArray_m1084FF60F463C8EB3890406EEDBB9F1DFC80116B_ftn) (intptr_t);
	static AndroidJNI_FromShortArray_m1084FF60F463C8EB3890406EEDBB9F1DFC80116B_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_FromShortArray_m1084FF60F463C8EB3890406EEDBB9F1DFC80116B_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::FromShortArray(System.IntPtr)");
	Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28* retVal = _il2cpp_icall_func(___array0);
	return retVal;
}
// System.Int32[] UnityEngine.AndroidJNI::FromIntArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* AndroidJNI_FromIntArray_mD538A30307431BC4BEC75F3709701742131FE6F8 (intptr_t ___array0, const RuntimeMethod* method)
{
	typedef Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* (*AndroidJNI_FromIntArray_mD538A30307431BC4BEC75F3709701742131FE6F8_ftn) (intptr_t);
	static AndroidJNI_FromIntArray_mD538A30307431BC4BEC75F3709701742131FE6F8_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_FromIntArray_mD538A30307431BC4BEC75F3709701742131FE6F8_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::FromIntArray(System.IntPtr)");
	Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* retVal = _il2cpp_icall_func(___array0);
	return retVal;
}
// System.Int64[] UnityEngine.AndroidJNI::FromLongArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F* AndroidJNI_FromLongArray_m5EDB9FD73EBB1F49486524B6A62B644D171A3CA4 (intptr_t ___array0, const RuntimeMethod* method)
{
	typedef Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F* (*AndroidJNI_FromLongArray_m5EDB9FD73EBB1F49486524B6A62B644D171A3CA4_ftn) (intptr_t);
	static AndroidJNI_FromLongArray_m5EDB9FD73EBB1F49486524B6A62B644D171A3CA4_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_FromLongArray_m5EDB9FD73EBB1F49486524B6A62B644D171A3CA4_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::FromLongArray(System.IntPtr)");
	Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F* retVal = _il2cpp_icall_func(___array0);
	return retVal;
}
// System.Single[] UnityEngine.AndroidJNI::FromFloatArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* AndroidJNI_FromFloatArray_m5B41CA3BE4AAB40310042C0CFA624BFDBF1E15CB (intptr_t ___array0, const RuntimeMethod* method)
{
	typedef SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* (*AndroidJNI_FromFloatArray_m5B41CA3BE4AAB40310042C0CFA624BFDBF1E15CB_ftn) (intptr_t);
	static AndroidJNI_FromFloatArray_m5B41CA3BE4AAB40310042C0CFA624BFDBF1E15CB_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_FromFloatArray_m5B41CA3BE4AAB40310042C0CFA624BFDBF1E15CB_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::FromFloatArray(System.IntPtr)");
	SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* retVal = _il2cpp_icall_func(___array0);
	return retVal;
}
// System.Double[] UnityEngine.AndroidJNI::FromDoubleArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* AndroidJNI_FromDoubleArray_m0994CF71AF7314249C12F3070FC50E048446D63E (intptr_t ___array0, const RuntimeMethod* method)
{
	typedef DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* (*AndroidJNI_FromDoubleArray_m0994CF71AF7314249C12F3070FC50E048446D63E_ftn) (intptr_t);
	static AndroidJNI_FromDoubleArray_m0994CF71AF7314249C12F3070FC50E048446D63E_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_FromDoubleArray_m0994CF71AF7314249C12F3070FC50E048446D63E_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::FromDoubleArray(System.IntPtr)");
	DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* retVal = _il2cpp_icall_func(___array0);
	return retVal;
}
// System.Int32 UnityEngine.AndroidJNI::GetArrayLength(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t AndroidJNI_GetArrayLength_m3DD9BD96B89F86A4F8AAB10147CAADB951E49936 (intptr_t ___array0, const RuntimeMethod* method)
{
	typedef int32_t (*AndroidJNI_GetArrayLength_m3DD9BD96B89F86A4F8AAB10147CAADB951E49936_ftn) (intptr_t);
	static AndroidJNI_GetArrayLength_m3DD9BD96B89F86A4F8AAB10147CAADB951E49936_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_GetArrayLength_m3DD9BD96B89F86A4F8AAB10147CAADB951E49936_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::GetArrayLength(System.IntPtr)");
	int32_t retVal = _il2cpp_icall_func(___array0);
	return retVal;
}
// System.IntPtr UnityEngine.AndroidJNI::NewObjectArray(System.Int32,System.IntPtr,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_NewObjectArray_m49BBDBCC804A6799866B92D6E0DEA9A204B6BE43 (int32_t ___size0, intptr_t ___clazz1, intptr_t ___obj2, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_NewObjectArray_m49BBDBCC804A6799866B92D6E0DEA9A204B6BE43_ftn) (int32_t, intptr_t, intptr_t);
	static AndroidJNI_NewObjectArray_m49BBDBCC804A6799866B92D6E0DEA9A204B6BE43_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_NewObjectArray_m49BBDBCC804A6799866B92D6E0DEA9A204B6BE43_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::NewObjectArray(System.Int32,System.IntPtr,System.IntPtr)");
	intptr_t retVal = _il2cpp_icall_func(___size0, ___clazz1, ___obj2);
	return retVal;
}
// System.IntPtr UnityEngine.AndroidJNI::GetObjectArrayElement(System.IntPtr,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNI_GetObjectArrayElement_m104E43629B8731ACAF53A5D351CCB19398A75648 (intptr_t ___array0, int32_t ___index1, const RuntimeMethod* method)
{
	typedef intptr_t (*AndroidJNI_GetObjectArrayElement_m104E43629B8731ACAF53A5D351CCB19398A75648_ftn) (intptr_t, int32_t);
	static AndroidJNI_GetObjectArrayElement_m104E43629B8731ACAF53A5D351CCB19398A75648_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_GetObjectArrayElement_m104E43629B8731ACAF53A5D351CCB19398A75648_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::GetObjectArrayElement(System.IntPtr,System.Int32)");
	intptr_t retVal = _il2cpp_icall_func(___array0, ___index1);
	return retVal;
}
// System.Void UnityEngine.AndroidJNI::SetObjectArrayElement(System.IntPtr,System.Int32,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNI_SetObjectArrayElement_m3CB77880BEEAA75E69813F5B193F07BDD8933418 (intptr_t ___array0, int32_t ___index1, intptr_t ___obj2, const RuntimeMethod* method)
{
	typedef void (*AndroidJNI_SetObjectArrayElement_m3CB77880BEEAA75E69813F5B193F07BDD8933418_ftn) (intptr_t, int32_t, intptr_t);
	static AndroidJNI_SetObjectArrayElement_m3CB77880BEEAA75E69813F5B193F07BDD8933418_ftn _il2cpp_icall_func;
	if (!_il2cpp_icall_func)
	_il2cpp_icall_func = (AndroidJNI_SetObjectArrayElement_m3CB77880BEEAA75E69813F5B193F07BDD8933418_ftn)il2cpp_codegen_resolve_icall ("UnityEngine.AndroidJNI::SetObjectArrayElement(System.IntPtr,System.Int32,System.IntPtr)");
	_il2cpp_icall_func(___array0, ___index1, ___obj2);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.IntPtr UnityEngine.AndroidJNIHelper::GetConstructorID(System.IntPtr,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNIHelper_GetConstructorID_m9978ECF944003B11786DDB1FDF0456CD89AF1180 (intptr_t ___javaClass0, String_t* ___signature1, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		intptr_t L_0 = ___javaClass0;
		String_t* L_1 = ___signature1;
		intptr_t L_2 = _AndroidJNIHelper_GetConstructorID_m9A5019D80C0E776003ADFC0A54A879ECDC3B60D8((intptr_t)L_0, L_1, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_2;
		goto IL_000e;
	}

IL_000e:
	{
		intptr_t L_3 = V_0;
		return (intptr_t)L_3;
	}
}
// System.IntPtr UnityEngine.AndroidJNIHelper::GetMethodID(System.IntPtr,System.String,System.String,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNIHelper_GetMethodID_mD3057EDF00D6BBB3E89116EE05F68D0731AD9E43 (intptr_t ___javaClass0, String_t* ___methodName1, String_t* ___signature2, bool ___isStatic3, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		intptr_t L_0 = ___javaClass0;
		String_t* L_1 = ___methodName1;
		String_t* L_2 = ___signature2;
		bool L_3 = ___isStatic3;
		intptr_t L_4 = _AndroidJNIHelper_GetMethodID_m22C073C0BCB560A1AD9EE6158FF8314D291EF756((intptr_t)L_0, L_1, L_2, L_3, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_4;
		goto IL_0010;
	}

IL_0010:
	{
		intptr_t L_5 = V_0;
		return (intptr_t)L_5;
	}
}
// System.IntPtr UnityEngine.AndroidJNIHelper::CreateJavaRunnable(UnityEngine.AndroidJavaRunnable)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNIHelper_CreateJavaRunnable_mA6C7A0E1BEF771970126D0FB21FF6E95CF569ED8 (AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 * ___jrunnable0, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 * L_0 = ___jrunnable0;
		intptr_t L_1 = _AndroidJNIHelper_CreateJavaRunnable_mC009CB98AF579A1DBECE07EE23A4F20B8E53BDF0(L_0, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_1;
		goto IL_000d;
	}

IL_000d:
	{
		intptr_t L_2 = V_0;
		return (intptr_t)L_2;
	}
}
// System.IntPtr UnityEngine.AndroidJNIHelper::CreateJavaProxy(UnityEngine.AndroidJavaProxy)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNIHelper_CreateJavaProxy_m29A8BD91809FF21642EA1319E5F097979EE8FA28 (AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D * ___proxy0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJNIHelper_CreateJavaProxy_m29A8BD91809FF21642EA1319E5F097979EE8FA28_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	GCHandle_t39FAEE3EA592432C93B574A31DD83B87F1847DE3  V_0;
	memset((&V_0), 0, sizeof(V_0));
	intptr_t V_1;
	memset((&V_1), 0, sizeof(V_1));
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 1);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	{
		AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D * L_0 = ___proxy0;
		GCHandle_t39FAEE3EA592432C93B574A31DD83B87F1847DE3  L_1 = GCHandle_Alloc_m5BF9DC23B533B904BFEA61136B92916683B46B0F(L_0, /*hidden argument*/NULL);
		V_0 = L_1;
	}

IL_0008:
	try
	{ // begin try (depth: 1)
		GCHandle_t39FAEE3EA592432C93B574A31DD83B87F1847DE3  L_2 = V_0;
		intptr_t L_3 = GCHandle_ToIntPtr_m8CF7D07846B0C741B04A2A4E5E9B5D505F4B3CCE(L_2, /*hidden argument*/NULL);
		AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D * L_4 = ___proxy0;
		intptr_t L_5 = _AndroidJNIHelper_CreateJavaProxy_m8E6AAE823A5FB6D70B4655FA45203779946321ED((intptr_t)L_3, L_4, /*hidden argument*/NULL);
		V_1 = (intptr_t)L_5;
		goto IL_0026;
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__exception_local = (Exception_t *)e.ex;
		if(il2cpp_codegen_class_is_assignable_from (RuntimeObject_il2cpp_TypeInfo_var, il2cpp_codegen_object_class(e.ex)))
			goto CATCH_001b;
		throw e;
	}

CATCH_001b:
	{ // begin catch(System.Object)
		GCHandle_Free_m392ECC9B1058E35A0FD5CF21A65F212873FC26F0((GCHandle_t39FAEE3EA592432C93B574A31DD83B87F1847DE3 *)(&V_0), /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(__exception_local, NULL, AndroidJNIHelper_CreateJavaProxy_m29A8BD91809FF21642EA1319E5F097979EE8FA28_RuntimeMethod_var);
	} // end catch (depth: 1)

IL_0026:
	{
		intptr_t L_6 = V_1;
		return (intptr_t)L_6;
	}
}
// UnityEngine.jvalue[] UnityEngine.AndroidJNIHelper::CreateJNIArgArray(System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* AndroidJNIHelper_CreateJNIArgArray_mAA5972FD580D58FA3D30B4E97B9837B439231F34 (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args0, const RuntimeMethod* method)
{
	jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* V_0 = NULL;
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_0 = ___args0;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_1 = _AndroidJNIHelper_CreateJNIArgArray_m9605B7C73D18B6A11264A61E33888374E1F283A9(L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		goto IL_000d;
	}

IL_000d:
	{
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = V_0;
		return L_2;
	}
}
// System.Void UnityEngine.AndroidJNIHelper::DeleteJNIArgArray(System.Object[],UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNIHelper_DeleteJNIArgArray_mEDFD8275CF10A3E0777350597633378776673784 (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args0, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___jniArgs1, const RuntimeMethod* method)
{
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_0 = ___args0;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_1 = ___jniArgs1;
		_AndroidJNIHelper_DeleteJNIArgArray_mCD37E30D32E979ED19131F9DC77A8DDD69D2E1A5(L_0, L_1, /*hidden argument*/NULL);
		return;
	}
}
// System.IntPtr UnityEngine.AndroidJNIHelper::GetConstructorID(System.IntPtr,System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNIHelper_GetConstructorID_m2756A393612A1CF86E3E73109E2268D9933F9F1E (intptr_t ___jclass0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		intptr_t L_0 = ___jclass0;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_1 = ___args1;
		intptr_t L_2 = _AndroidJNIHelper_GetConstructorID_m1982E4290531BD8134C7B5EDF918B87466284D77((intptr_t)L_0, L_1, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_2;
		goto IL_000e;
	}

IL_000e:
	{
		intptr_t L_3 = V_0;
		return (intptr_t)L_3;
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
// System.Void UnityEngine.AndroidJNISafe::CheckException()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	intptr_t V_1;
	memset((&V_1), 0, sizeof(V_1));
	intptr_t V_2;
	memset((&V_2), 0, sizeof(V_2));
	intptr_t V_3;
	memset((&V_3), 0, sizeof(V_3));
	intptr_t V_4;
	memset((&V_4), 0, sizeof(V_4));
	String_t* V_5 = NULL;
	jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* V_6 = NULL;
	String_t* V_7 = NULL;
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	{
		intptr_t L_0 = AndroidJNI_ExceptionOccurred_mC2EC654C42E285C9E141393BDA41A4D8BC56FECD(/*hidden argument*/NULL);
		V_0 = (intptr_t)L_0;
		intptr_t L_1 = V_0;
		bool L_2 = IntPtr_op_Inequality_mB4886A806009EA825EFCC60CD2A7F6EB8E273A61((intptr_t)L_1, (intptr_t)(0), /*hidden argument*/NULL);
		if (!L_2)
		{
			goto IL_00a7;
		}
	}
	{
		AndroidJNI_ExceptionClear_m339CEFB228B0F08EBA289AED25464FF0D80B9936(/*hidden argument*/NULL);
		intptr_t L_3 = AndroidJNI_FindClass_m07E2127D59F7EC97A06B5350699033448BD40CED(_stringLiteralA515F49F0F4C724D096B5DA7E31DFBB14FC018AC, /*hidden argument*/NULL);
		V_1 = (intptr_t)L_3;
		intptr_t L_4 = AndroidJNI_FindClass_m07E2127D59F7EC97A06B5350699033448BD40CED(_stringLiteral40AB7562969E4FED5261D3FBFA38AEA9397716D4, /*hidden argument*/NULL);
		V_2 = (intptr_t)L_4;
	}

IL_0033:
	try
	{ // begin try (depth: 1)
		intptr_t L_5 = V_1;
		intptr_t L_6 = AndroidJNI_GetMethodID_m4D7386D69FFEF80467F1804447C094B59385AF0C((intptr_t)L_5, _stringLiteralA7EDC6086A91C13EEC0568F09CD6263D5A4CFFEC, _stringLiteralFF17B8589770AC5A0B97D99980EA610D3A07AC25, /*hidden argument*/NULL);
		V_3 = (intptr_t)L_6;
		intptr_t L_7 = V_2;
		intptr_t L_8 = AndroidJNI_GetStaticMethodID_m135C9DEFFC207E509C001370C227F6E217FD9A1C((intptr_t)L_7, _stringLiteralE2F1487C5A036945AF1009212074D1B8984E2994, _stringLiteral4E156FB692586759BECC946B6A67CEC836B61DDA, /*hidden argument*/NULL);
		V_4 = (intptr_t)L_8;
		intptr_t L_9 = V_0;
		intptr_t L_10 = V_3;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_11 = (jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*)(jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*)SZArrayNew(jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3_il2cpp_TypeInfo_var, (uint32_t)0);
		String_t* L_12 = AndroidJNI_CallStringMethod_m3322E22FCA053618D794A9F3D00CFA1368F10AA9((intptr_t)L_9, (intptr_t)L_10, L_11, /*hidden argument*/NULL);
		V_5 = L_12;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_13 = (jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*)(jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*)SZArrayNew(jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3_il2cpp_TypeInfo_var, (uint32_t)1);
		V_6 = L_13;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_14 = V_6;
		NullCheck(L_14);
		intptr_t L_15 = V_0;
		((L_14)->GetAddressAt(static_cast<il2cpp_array_size_t>(0)))->set_l_8((intptr_t)L_15);
		intptr_t L_16 = V_2;
		intptr_t L_17 = V_4;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_18 = V_6;
		String_t* L_19 = AndroidJNI_CallStaticStringMethod_m7502E60348B62159AE2F0C06D3D663E6E1F28116((intptr_t)L_16, (intptr_t)L_17, L_18, /*hidden argument*/NULL);
		V_7 = L_19;
		String_t* L_20 = V_5;
		String_t* L_21 = V_7;
		AndroidJavaException_tC81E6FAAA4067CBA537727328D5D2DB14F5F5466 * L_22 = (AndroidJavaException_tC81E6FAAA4067CBA537727328D5D2DB14F5F5466 *)il2cpp_codegen_object_new(AndroidJavaException_tC81E6FAAA4067CBA537727328D5D2DB14F5F5466_il2cpp_TypeInfo_var);
		AndroidJavaException__ctor_m8E5216F0181090FB7A9016AED78B7935019791D8(L_22, L_20, L_21, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_22, NULL, AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C_RuntimeMethod_var);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0092;
	}

FINALLY_0092:
	{ // begin finally (depth: 1)
		intptr_t L_23 = V_0;
		AndroidJNISafe_DeleteLocalRef_m9632EA13BF03AEE43FC7713125962A4D0DFFADC7((intptr_t)L_23, /*hidden argument*/NULL);
		intptr_t L_24 = V_1;
		AndroidJNISafe_DeleteLocalRef_m9632EA13BF03AEE43FC7713125962A4D0DFFADC7((intptr_t)L_24, /*hidden argument*/NULL);
		intptr_t L_25 = V_2;
		AndroidJNISafe_DeleteLocalRef_m9632EA13BF03AEE43FC7713125962A4D0DFFADC7((intptr_t)L_25, /*hidden argument*/NULL);
		IL2CPP_END_FINALLY(146)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(146)
	{
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_00a7:
	{
		return;
	}
}
// System.Void UnityEngine.AndroidJNISafe::DeleteGlobalRef(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNISafe_DeleteGlobalRef_mE0C851F30E3481496C72814973B66161C486D8BA (intptr_t ___globalref0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJNISafe_DeleteGlobalRef_mE0C851F30E3481496C72814973B66161C486D8BA_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		intptr_t L_0 = ___globalref0;
		bool L_1 = IntPtr_op_Inequality_mB4886A806009EA825EFCC60CD2A7F6EB8E273A61((intptr_t)L_0, (intptr_t)(0), /*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_0017;
		}
	}
	{
		intptr_t L_2 = ___globalref0;
		AndroidJNI_DeleteGlobalRef_mC800FCE93424A8778220806C3FE3497E21E94333((intptr_t)L_2, /*hidden argument*/NULL);
	}

IL_0017:
	{
		return;
	}
}
// System.Void UnityEngine.AndroidJNISafe::DeleteWeakGlobalRef(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNISafe_DeleteWeakGlobalRef_mB338C2F7116360905B7F444BDB16CAB18B914ED3 (intptr_t ___globalref0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJNISafe_DeleteWeakGlobalRef_mB338C2F7116360905B7F444BDB16CAB18B914ED3_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		intptr_t L_0 = ___globalref0;
		bool L_1 = IntPtr_op_Inequality_mB4886A806009EA825EFCC60CD2A7F6EB8E273A61((intptr_t)L_0, (intptr_t)(0), /*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_0017;
		}
	}
	{
		intptr_t L_2 = ___globalref0;
		AndroidJNI_DeleteWeakGlobalRef_m07AE954A94CDB58980A3CBA36E0E8F236BE01C75((intptr_t)L_2, /*hidden argument*/NULL);
	}

IL_0017:
	{
		return;
	}
}
// System.Void UnityEngine.AndroidJNISafe::DeleteLocalRef(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNISafe_DeleteLocalRef_m9632EA13BF03AEE43FC7713125962A4D0DFFADC7 (intptr_t ___localref0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJNISafe_DeleteLocalRef_m9632EA13BF03AEE43FC7713125962A4D0DFFADC7_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		intptr_t L_0 = ___localref0;
		bool L_1 = IntPtr_op_Inequality_mB4886A806009EA825EFCC60CD2A7F6EB8E273A61((intptr_t)L_0, (intptr_t)(0), /*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_0017;
		}
	}
	{
		intptr_t L_2 = ___localref0;
		AndroidJNI_DeleteLocalRef_m5A7291640D0BB0F2A484C729CEDBF43F92B7941A((intptr_t)L_2, /*hidden argument*/NULL);
	}

IL_0017:
	{
		return;
	}
}
// System.IntPtr UnityEngine.AndroidJNISafe::NewString(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_NewString_mD1D954E0EE5A8F135B19EE67E8FF2A4E1A6CA97F (String_t* ___chars0, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
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
		String_t* L_0 = ___chars0;
		intptr_t L_1 = AndroidJNI_NewString_m4B505016C60A4B2602F2037983367C2DB52A8BE2(L_0, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		intptr_t L_2 = V_0;
		return (intptr_t)L_2;
	}
}
// System.String UnityEngine.AndroidJNISafe::GetStringChars(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* AndroidJNISafe_GetStringChars_m15C4A04998812B41DF6E67D7D2F9F270573847FE (intptr_t ___str0, const RuntimeMethod* method)
{
	String_t* V_0 = NULL;
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
		intptr_t L_0 = ___str0;
		String_t* L_1 = AndroidJNI_GetStringChars_m1C44DAAF9B7AA8E9586F1CD236E825B07741A268((intptr_t)L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		String_t* L_2 = V_0;
		return L_2;
	}
}
// System.IntPtr UnityEngine.AndroidJNISafe::GetObjectClass(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_GetObjectClass_mB36866622A9FD487DCA6926F63038E5584B35BFB (intptr_t ___ptr0, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
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
		intptr_t L_0 = ___ptr0;
		intptr_t L_1 = AndroidJNI_GetObjectClass_mA9719B0A6734C4ED55B60B129A9D51F7B8A3B4A6((intptr_t)L_0, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		intptr_t L_2 = V_0;
		return (intptr_t)L_2;
	}
}
// System.IntPtr UnityEngine.AndroidJNISafe::GetStaticMethodID(System.IntPtr,System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_GetStaticMethodID_m4DCBC629048509F8E8566998CDA8F1AB9EAD6A50 (intptr_t ___clazz0, String_t* ___name1, String_t* ___sig2, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
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
		intptr_t L_0 = ___clazz0;
		String_t* L_1 = ___name1;
		String_t* L_2 = ___sig2;
		intptr_t L_3 = AndroidJNI_GetStaticMethodID_m135C9DEFFC207E509C001370C227F6E217FD9A1C((intptr_t)L_0, L_1, L_2, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_3;
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		intptr_t L_4 = V_0;
		return (intptr_t)L_4;
	}
}
// System.IntPtr UnityEngine.AndroidJNISafe::GetMethodID(System.IntPtr,System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_GetMethodID_m91CE11744503D04CD2AA8BAD99C914B1C2C6D494 (intptr_t ___obj0, String_t* ___name1, String_t* ___sig2, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
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
		intptr_t L_0 = ___obj0;
		String_t* L_1 = ___name1;
		String_t* L_2 = ___sig2;
		intptr_t L_3 = AndroidJNI_GetMethodID_m4D7386D69FFEF80467F1804447C094B59385AF0C((intptr_t)L_0, L_1, L_2, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_3;
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		intptr_t L_4 = V_0;
		return (intptr_t)L_4;
	}
}
// System.IntPtr UnityEngine.AndroidJNISafe::FromReflectedMethod(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_FromReflectedMethod_m47AA20F4A2F8451B9BDCF8C6045802F04112F221 (intptr_t ___refMethod0, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
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
		intptr_t L_0 = ___refMethod0;
		intptr_t L_1 = AndroidJNI_FromReflectedMethod_m5F01D9D2E6FDB25E9DF3B8804FC6A536C71F84B9((intptr_t)L_0, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		intptr_t L_2 = V_0;
		return (intptr_t)L_2;
	}
}
// System.IntPtr UnityEngine.AndroidJNISafe::FindClass(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_FindClass_mE58501828AA09ADC26347853AFE6D025845D487C (String_t* ___name0, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
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
		String_t* L_0 = ___name0;
		intptr_t L_1 = AndroidJNI_FindClass_m07E2127D59F7EC97A06B5350699033448BD40CED(L_0, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		intptr_t L_2 = V_0;
		return (intptr_t)L_2;
	}
}
// System.IntPtr UnityEngine.AndroidJNISafe::NewObject(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_NewObject_m78BDA85E651167163148C9B39DEA8CE831EB1DB0 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
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
		intptr_t L_0 = ___clazz0;
		intptr_t L_1 = ___methodID1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = ___args2;
		intptr_t L_3 = AndroidJNI_NewObject_mA1E19D3C530766C0E9F3196CB23A4C9E7795689B((intptr_t)L_0, (intptr_t)L_1, L_2, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_3;
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		intptr_t L_4 = V_0;
		return (intptr_t)L_4;
	}
}
// System.Void UnityEngine.AndroidJNISafe::CallStaticVoidMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJNISafe_CallStaticVoidMethod_mC0BC9FA7E2FB69027E1F55E8810C6F619BCD7D59 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
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
		intptr_t L_0 = ___clazz0;
		intptr_t L_1 = ___methodID1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = ___args2;
		AndroidJNI_CallStaticVoidMethod_m973B08F0CE8068F0AC8A8FF85F0C63FD5AC3EAFA((intptr_t)L_0, (intptr_t)L_1, L_2, /*hidden argument*/NULL);
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		return;
	}
}
// System.IntPtr UnityEngine.AndroidJNISafe::CallStaticObjectMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_CallStaticObjectMethod_m11EDE005224D5A6833BFF896906397D24E19D440 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
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
		intptr_t L_0 = ___clazz0;
		intptr_t L_1 = ___methodID1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = ___args2;
		intptr_t L_3 = AndroidJNI_CallStaticObjectMethod_m8540B678387A3DE6F1F702CF3053826962F569C0((intptr_t)L_0, (intptr_t)L_1, L_2, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_3;
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		intptr_t L_4 = V_0;
		return (intptr_t)L_4;
	}
}
// System.String UnityEngine.AndroidJNISafe::CallStaticStringMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* AndroidJNISafe_CallStaticStringMethod_mBB43D0D0B7D7ED48C90F9D9FF583A629DC40EBA3 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	String_t* V_0 = NULL;
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
		intptr_t L_0 = ___clazz0;
		intptr_t L_1 = ___methodID1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = ___args2;
		String_t* L_3 = AndroidJNI_CallStaticStringMethod_m7502E60348B62159AE2F0C06D3D663E6E1F28116((intptr_t)L_0, (intptr_t)L_1, L_2, /*hidden argument*/NULL);
		V_0 = L_3;
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		String_t* L_4 = V_0;
		return L_4;
	}
}
// System.Char UnityEngine.AndroidJNISafe::CallStaticCharMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Il2CppChar AndroidJNISafe_CallStaticCharMethod_mC422B2FB9D7F13C0BEC8DAF00119B82FEA2854D9 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	Il2CppChar V_0 = 0x0;
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
		intptr_t L_0 = ___clazz0;
		intptr_t L_1 = ___methodID1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = ___args2;
		Il2CppChar L_3 = AndroidJNI_CallStaticCharMethod_m03968EDD820122C5AA74D396578D5C8F747DE8B9((intptr_t)L_0, (intptr_t)L_1, L_2, /*hidden argument*/NULL);
		V_0 = L_3;
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		Il2CppChar L_4 = V_0;
		return L_4;
	}
}
// System.Double UnityEngine.AndroidJNISafe::CallStaticDoubleMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR double AndroidJNISafe_CallStaticDoubleMethod_mC5A3C5AEEC15EB5D419E7B2B0A45DE2762310ABE (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	double V_0 = 0.0;
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
		intptr_t L_0 = ___clazz0;
		intptr_t L_1 = ___methodID1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = ___args2;
		double L_3 = AndroidJNI_CallStaticDoubleMethod_mB27665BD677D31470812D5E4FA466259D18D8D67((intptr_t)L_0, (intptr_t)L_1, L_2, /*hidden argument*/NULL);
		V_0 = L_3;
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		double L_4 = V_0;
		return L_4;
	}
}
// System.Single UnityEngine.AndroidJNISafe::CallStaticFloatMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float AndroidJNISafe_CallStaticFloatMethod_mA0AEAAA5ACCC7EB36F04616DCB2E09D29B6DED30 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	float V_0 = 0.0f;
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
		intptr_t L_0 = ___clazz0;
		intptr_t L_1 = ___methodID1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = ___args2;
		float L_3 = AndroidJNI_CallStaticFloatMethod_m22FE454F030F117CFA7CE8F8CE55A4DD9EB226DD((intptr_t)L_0, (intptr_t)L_1, L_2, /*hidden argument*/NULL);
		V_0 = L_3;
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		float L_4 = V_0;
		return L_4;
	}
}
// System.Int64 UnityEngine.AndroidJNISafe::CallStaticLongMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t AndroidJNISafe_CallStaticLongMethod_mDEA9005EBB9126BD13C56C1D4497C60863F1D00B (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	int64_t V_0 = 0;
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
		intptr_t L_0 = ___clazz0;
		intptr_t L_1 = ___methodID1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = ___args2;
		int64_t L_3 = AndroidJNI_CallStaticLongMethod_mACA1CFC943C54BB656D065AB6EF0A78FE3EEC014((intptr_t)L_0, (intptr_t)L_1, L_2, /*hidden argument*/NULL);
		V_0 = L_3;
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		int64_t L_4 = V_0;
		return L_4;
	}
}
// System.Int16 UnityEngine.AndroidJNISafe::CallStaticShortMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int16_t AndroidJNISafe_CallStaticShortMethod_m970528ACEB23F9AE4A38A9B223B825DF10A64F09 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	int16_t V_0 = 0;
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
		intptr_t L_0 = ___clazz0;
		intptr_t L_1 = ___methodID1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = ___args2;
		int16_t L_3 = AndroidJNI_CallStaticShortMethod_m1BC0BA260F59800529D511D0E51B501165056F3F((intptr_t)L_0, (intptr_t)L_1, L_2, /*hidden argument*/NULL);
		V_0 = L_3;
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		int16_t L_4 = V_0;
		return L_4;
	}
}
// System.SByte UnityEngine.AndroidJNISafe::CallStaticSByteMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int8_t AndroidJNISafe_CallStaticSByteMethod_m6F9A948F2EE6B668618D1B39FF3450368FA95010 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	int8_t V_0 = 0x0;
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
		intptr_t L_0 = ___clazz0;
		intptr_t L_1 = ___methodID1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = ___args2;
		int8_t L_3 = AndroidJNI_CallStaticSByteMethod_m637357610E5ECF91256FD6EFA48468D276395F46((intptr_t)L_0, (intptr_t)L_1, L_2, /*hidden argument*/NULL);
		V_0 = L_3;
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		int8_t L_4 = V_0;
		return L_4;
	}
}
// System.Boolean UnityEngine.AndroidJNISafe::CallStaticBooleanMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool AndroidJNISafe_CallStaticBooleanMethod_mD4AE550694EEC7859F137D0C60F0C94BD1092272 (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	bool V_0 = false;
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
		intptr_t L_0 = ___clazz0;
		intptr_t L_1 = ___methodID1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = ___args2;
		bool L_3 = AndroidJNI_CallStaticBooleanMethod_mA5C4F5D3A724351C0DB569E863F070493E86069F((intptr_t)L_0, (intptr_t)L_1, L_2, /*hidden argument*/NULL);
		V_0 = L_3;
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		bool L_4 = V_0;
		return L_4;
	}
}
// System.Int32 UnityEngine.AndroidJNISafe::CallStaticIntMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t AndroidJNISafe_CallStaticIntMethod_mBBD8501C4128A05B243DEDD7FC1473B7F8B6DFCA (intptr_t ___clazz0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
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
		intptr_t L_0 = ___clazz0;
		intptr_t L_1 = ___methodID1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = ___args2;
		int32_t L_3 = AndroidJNI_CallStaticIntMethod_mC112D86B8844819C4D02AA8136BCF8C673B59FF0((intptr_t)L_0, (intptr_t)L_1, L_2, /*hidden argument*/NULL);
		V_0 = L_3;
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		int32_t L_4 = V_0;
		return L_4;
	}
}
// System.IntPtr UnityEngine.AndroidJNISafe::CallObjectMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_CallObjectMethod_m4E9B0BCDACAF851BA170F85BA9F06727B6A3452B (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
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
		intptr_t L_0 = ___obj0;
		intptr_t L_1 = ___methodID1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = ___args2;
		intptr_t L_3 = AndroidJNI_CallObjectMethod_m953C16AD55D061D331B16060D9C2E7BEFFC34BB0((intptr_t)L_0, (intptr_t)L_1, L_2, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_3;
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		intptr_t L_4 = V_0;
		return (intptr_t)L_4;
	}
}
// System.String UnityEngine.AndroidJNISafe::CallStringMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* AndroidJNISafe_CallStringMethod_mF74DF782A0F41B0355910B4A6D1A88FFCA9E767D (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	String_t* V_0 = NULL;
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
		intptr_t L_0 = ___obj0;
		intptr_t L_1 = ___methodID1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = ___args2;
		String_t* L_3 = AndroidJNI_CallStringMethod_m3322E22FCA053618D794A9F3D00CFA1368F10AA9((intptr_t)L_0, (intptr_t)L_1, L_2, /*hidden argument*/NULL);
		V_0 = L_3;
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		String_t* L_4 = V_0;
		return L_4;
	}
}
// System.Char UnityEngine.AndroidJNISafe::CallCharMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Il2CppChar AndroidJNISafe_CallCharMethod_mCE65F1C456B282169DFCD5A7D87E4DF78EE89626 (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	Il2CppChar V_0 = 0x0;
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
		intptr_t L_0 = ___obj0;
		intptr_t L_1 = ___methodID1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = ___args2;
		Il2CppChar L_3 = AndroidJNI_CallCharMethod_mC5FEB28906B1F004D5EAE36363C2F2B32B4D25FD((intptr_t)L_0, (intptr_t)L_1, L_2, /*hidden argument*/NULL);
		V_0 = L_3;
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		Il2CppChar L_4 = V_0;
		return L_4;
	}
}
// System.Double UnityEngine.AndroidJNISafe::CallDoubleMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR double AndroidJNISafe_CallDoubleMethod_m47F889A5E70637CDF523C7A84CC7F657FBEB8427 (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	double V_0 = 0.0;
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
		intptr_t L_0 = ___obj0;
		intptr_t L_1 = ___methodID1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = ___args2;
		double L_3 = AndroidJNI_CallDoubleMethod_m391E75D42B6B445B80D751F56440DDE1C20A79EE((intptr_t)L_0, (intptr_t)L_1, L_2, /*hidden argument*/NULL);
		V_0 = L_3;
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		double L_4 = V_0;
		return L_4;
	}
}
// System.Single UnityEngine.AndroidJNISafe::CallFloatMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float AndroidJNISafe_CallFloatMethod_m74F15E4AE8B0341919AD470E0528599F3042E0D5 (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	float V_0 = 0.0f;
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
		intptr_t L_0 = ___obj0;
		intptr_t L_1 = ___methodID1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = ___args2;
		float L_3 = AndroidJNI_CallFloatMethod_mFDB1FC58B999500B822E336ABB60408463FD9BAF((intptr_t)L_0, (intptr_t)L_1, L_2, /*hidden argument*/NULL);
		V_0 = L_3;
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		float L_4 = V_0;
		return L_4;
	}
}
// System.Int64 UnityEngine.AndroidJNISafe::CallLongMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int64_t AndroidJNISafe_CallLongMethod_m30E44F8538D228134490B925FF35A2E8D194D0FC (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	int64_t V_0 = 0;
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
		intptr_t L_0 = ___obj0;
		intptr_t L_1 = ___methodID1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = ___args2;
		int64_t L_3 = AndroidJNI_CallLongMethod_mF2B511CFE25949D688142C6A8A11973C22BE1AFC((intptr_t)L_0, (intptr_t)L_1, L_2, /*hidden argument*/NULL);
		V_0 = L_3;
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		int64_t L_4 = V_0;
		return L_4;
	}
}
// System.Int16 UnityEngine.AndroidJNISafe::CallShortMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int16_t AndroidJNISafe_CallShortMethod_m0922D537A7A7C7576BA5CFA7359EEB1430B142B8 (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	int16_t V_0 = 0;
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
		intptr_t L_0 = ___obj0;
		intptr_t L_1 = ___methodID1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = ___args2;
		int16_t L_3 = AndroidJNI_CallShortMethod_m1402B57DDA2B128398A7A911CDB24E06ED376D51((intptr_t)L_0, (intptr_t)L_1, L_2, /*hidden argument*/NULL);
		V_0 = L_3;
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		int16_t L_4 = V_0;
		return L_4;
	}
}
// System.SByte UnityEngine.AndroidJNISafe::CallSByteMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int8_t AndroidJNISafe_CallSByteMethod_mBC18848E620817FD4BCD72EB66E5EFDE64B34AA8 (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	int8_t V_0 = 0x0;
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
		intptr_t L_0 = ___obj0;
		intptr_t L_1 = ___methodID1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = ___args2;
		int8_t L_3 = AndroidJNI_CallSByteMethod_m34A084018795E6E5847305390565A2A494AD2422((intptr_t)L_0, (intptr_t)L_1, L_2, /*hidden argument*/NULL);
		V_0 = L_3;
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		int8_t L_4 = V_0;
		return L_4;
	}
}
// System.Boolean UnityEngine.AndroidJNISafe::CallBooleanMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool AndroidJNISafe_CallBooleanMethod_mE15E3147C3BD2BE20EE4ACD537DFB1253254E743 (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	bool V_0 = false;
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
		intptr_t L_0 = ___obj0;
		intptr_t L_1 = ___methodID1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = ___args2;
		bool L_3 = AndroidJNI_CallBooleanMethod_mAE45802EE32D57194B47BC62E0AD9F8C56C41800((intptr_t)L_0, (intptr_t)L_1, L_2, /*hidden argument*/NULL);
		V_0 = L_3;
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		bool L_4 = V_0;
		return L_4;
	}
}
// System.Int32 UnityEngine.AndroidJNISafe::CallIntMethod(System.IntPtr,System.IntPtr,UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t AndroidJNISafe_CallIntMethod_m014D37C85659EDCDDFF9A4007ED1943981525E95 (intptr_t ___obj0, intptr_t ___methodID1, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___args2, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
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
		intptr_t L_0 = ___obj0;
		intptr_t L_1 = ___methodID1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = ___args2;
		int32_t L_3 = AndroidJNI_CallIntMethod_m83AA9264B8978F8D42B4B5239CEDA616AD6FE047((intptr_t)L_0, (intptr_t)L_1, L_2, /*hidden argument*/NULL);
		V_0 = L_3;
		IL2CPP_LEAVE(0x18, FINALLY_0010);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0010;
	}

FINALLY_0010:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(16)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(16)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		int32_t L_4 = V_0;
		return L_4;
	}
}
// System.Char[] UnityEngine.AndroidJNISafe::FromCharArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* AndroidJNISafe_FromCharArray_mDB6AE528FE52AC622EB833337F36AA93B5248E1B (intptr_t ___array0, const RuntimeMethod* method)
{
	CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* V_0 = NULL;
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
		intptr_t L_0 = ___array0;
		CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* L_1 = AndroidJNI_FromCharArray_mB24FA47F69D0B382F0D3F5F4B62F9B6F14F52842((intptr_t)L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* L_2 = V_0;
		return L_2;
	}
}
// System.Double[] UnityEngine.AndroidJNISafe::FromDoubleArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* AndroidJNISafe_FromDoubleArray_m10BE0E812ED3FC49D0FF7EFA7352F8EA026F824E (intptr_t ___array0, const RuntimeMethod* method)
{
	DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* V_0 = NULL;
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
		intptr_t L_0 = ___array0;
		DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* L_1 = AndroidJNI_FromDoubleArray_m0994CF71AF7314249C12F3070FC50E048446D63E((intptr_t)L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* L_2 = V_0;
		return L_2;
	}
}
// System.Single[] UnityEngine.AndroidJNISafe::FromFloatArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* AndroidJNISafe_FromFloatArray_m087EAD07306786A03F15756F9EC26CA2AB6B8BCB (intptr_t ___array0, const RuntimeMethod* method)
{
	SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* V_0 = NULL;
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
		intptr_t L_0 = ___array0;
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_1 = AndroidJNI_FromFloatArray_m5B41CA3BE4AAB40310042C0CFA624BFDBF1E15CB((intptr_t)L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_2 = V_0;
		return L_2;
	}
}
// System.Int64[] UnityEngine.AndroidJNISafe::FromLongArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F* AndroidJNISafe_FromLongArray_mDCCAE11E1BB9C72B1DCB0D5CB4D191922EB499C5 (intptr_t ___array0, const RuntimeMethod* method)
{
	Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F* V_0 = NULL;
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
		intptr_t L_0 = ___array0;
		Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F* L_1 = AndroidJNI_FromLongArray_m5EDB9FD73EBB1F49486524B6A62B644D171A3CA4((intptr_t)L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F* L_2 = V_0;
		return L_2;
	}
}
// System.Int16[] UnityEngine.AndroidJNISafe::FromShortArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28* AndroidJNISafe_FromShortArray_m05B4445B460FC16B41851A5C898123223C0B0024 (intptr_t ___array0, const RuntimeMethod* method)
{
	Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28* V_0 = NULL;
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
		intptr_t L_0 = ___array0;
		Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28* L_1 = AndroidJNI_FromShortArray_m1084FF60F463C8EB3890406EEDBB9F1DFC80116B((intptr_t)L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28* L_2 = V_0;
		return L_2;
	}
}
// System.Byte[] UnityEngine.AndroidJNISafe::FromByteArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* AndroidJNISafe_FromByteArray_m8182D68596E21605519D27197C4870DCAB9F6550 (intptr_t ___array0, const RuntimeMethod* method)
{
	ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* V_0 = NULL;
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
		intptr_t L_0 = ___array0;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_1 = AndroidJNI_FromByteArray_mB1B0AC781BA50C8AE7F9A6B8660B7C3F6D7DDE02((intptr_t)L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_2 = V_0;
		return L_2;
	}
}
// System.SByte[] UnityEngine.AndroidJNISafe::FromSByteArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889* AndroidJNISafe_FromSByteArray_m44649611607069754D9DD6A53B58C65AAE69C8E8 (intptr_t ___array0, const RuntimeMethod* method)
{
	SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889* V_0 = NULL;
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
		intptr_t L_0 = ___array0;
		SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889* L_1 = AndroidJNI_FromSByteArray_m15A1A9366FC6A1952DA42809D8EEF59678ABF69E((intptr_t)L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889* L_2 = V_0;
		return L_2;
	}
}
// System.Boolean[] UnityEngine.AndroidJNISafe::FromBooleanArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040* AndroidJNISafe_FromBooleanArray_m4CA0BE409AC39C391C4122A1DCE503B7EA87DC14 (intptr_t ___array0, const RuntimeMethod* method)
{
	BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040* V_0 = NULL;
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
		intptr_t L_0 = ___array0;
		BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040* L_1 = AndroidJNI_FromBooleanArray_mA5AF86E8FDA0D4B7CCA395E708527E2A1073AA86((intptr_t)L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040* L_2 = V_0;
		return L_2;
	}
}
// System.Int32[] UnityEngine.AndroidJNISafe::FromIntArray(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* AndroidJNISafe_FromIntArray_m5AB9419F8E92A4815A833006025ABD0039D6B353 (intptr_t ___array0, const RuntimeMethod* method)
{
	Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* V_0 = NULL;
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
		intptr_t L_0 = ___array0;
		Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* L_1 = AndroidJNI_FromIntArray_mD538A30307431BC4BEC75F3709701742131FE6F8((intptr_t)L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* L_2 = V_0;
		return L_2;
	}
}
// System.IntPtr UnityEngine.AndroidJNISafe::ToObjectArray(System.IntPtr[],System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_ToObjectArray_mB3A0EB74E8C47EB72667603D90A4DE2480E2AC63 (IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD* ___array0, intptr_t ___type1, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
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
		IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD* L_0 = ___array0;
		intptr_t L_1 = ___type1;
		intptr_t L_2 = AndroidJNI_ToObjectArray_m0614CB442A041E1EE108ADF05676C001710EC33A(L_0, (intptr_t)L_1, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_2;
		IL2CPP_LEAVE(0x17, FINALLY_000f);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000f;
	}

FINALLY_000f:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(15)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(15)
	{
		IL2CPP_JUMP_TBL(0x17, IL_0017)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0017:
	{
		intptr_t L_3 = V_0;
		return (intptr_t)L_3;
	}
}
// System.IntPtr UnityEngine.AndroidJNISafe::ToCharArray(System.Char[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_ToCharArray_m8AB18ECC188D1B8A15966FF3FBD7887CF35A5711 (CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* ___array0, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
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
		CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* L_0 = ___array0;
		intptr_t L_1 = AndroidJNI_ToCharArray_m2052C19FC000D01BA74DDAA7AC5EF8D4D13D1F6A(L_0, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		intptr_t L_2 = V_0;
		return (intptr_t)L_2;
	}
}
// System.IntPtr UnityEngine.AndroidJNISafe::ToDoubleArray(System.Double[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_ToDoubleArray_m9AE319DB92B91A255D2A0568D38B3B47CD0C69EB (DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* ___array0, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
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
		DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* L_0 = ___array0;
		intptr_t L_1 = AndroidJNI_ToDoubleArray_mB04386ABEC07D54732102A858B7F5250B49601CE(L_0, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		intptr_t L_2 = V_0;
		return (intptr_t)L_2;
	}
}
// System.IntPtr UnityEngine.AndroidJNISafe::ToFloatArray(System.Single[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_ToFloatArray_m8ACA5E42C6F32E7D851613AC129FB37AFC28EBFD (SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* ___array0, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
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
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_0 = ___array0;
		intptr_t L_1 = AndroidJNI_ToFloatArray_m684CAD369A3BDCE75B31FCC68F8CF7A1293A4533(L_0, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		intptr_t L_2 = V_0;
		return (intptr_t)L_2;
	}
}
// System.IntPtr UnityEngine.AndroidJNISafe::ToLongArray(System.Int64[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_ToLongArray_mD59D9304170DFB59B77342C994699BE445AF25D3 (Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F* ___array0, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
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
		Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F* L_0 = ___array0;
		intptr_t L_1 = AndroidJNI_ToLongArray_mFAAAB30B9A9944A7D6A590ADE0ACB50A11656928(L_0, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		intptr_t L_2 = V_0;
		return (intptr_t)L_2;
	}
}
// System.IntPtr UnityEngine.AndroidJNISafe::ToShortArray(System.Int16[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_ToShortArray_m7D79F918714300B5818C7C8646E4E1A48E056A07 (Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28* ___array0, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
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
		Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28* L_0 = ___array0;
		intptr_t L_1 = AndroidJNI_ToShortArray_m7FCED435AE3ACC7808F3CB9F9C5E8E16B616A316(L_0, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		intptr_t L_2 = V_0;
		return (intptr_t)L_2;
	}
}
// System.IntPtr UnityEngine.AndroidJNISafe::ToByteArray(System.Byte[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_ToByteArray_m01C86D2FE9259F0888FA97B105FC741A0E2290D5 (ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___array0, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
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
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_0 = ___array0;
		intptr_t L_1 = AndroidJNI_ToByteArray_m57A1B1DD05FCA40796E0CFAA8297528E807CB5F4(L_0, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		intptr_t L_2 = V_0;
		return (intptr_t)L_2;
	}
}
// System.IntPtr UnityEngine.AndroidJNISafe::ToSByteArray(System.SByte[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_ToSByteArray_m5AE0F49EE17ABDCFBCDF619CBECD5DEF9961BDB8 (SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889* ___array0, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
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
		SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889* L_0 = ___array0;
		intptr_t L_1 = AndroidJNI_ToSByteArray_mB78915C5C2948F80376765449650782802E03707(L_0, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		intptr_t L_2 = V_0;
		return (intptr_t)L_2;
	}
}
// System.IntPtr UnityEngine.AndroidJNISafe::ToBooleanArray(System.Boolean[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_ToBooleanArray_m1BCBD2041B6BFE6B91C1E3AD8C1133F791B70423 (BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040* ___array0, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
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
		BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040* L_0 = ___array0;
		intptr_t L_1 = AndroidJNI_ToBooleanArray_m7BEE0A1FEC1AAB4A244716CD93ABB456DC8E28C2(L_0, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		intptr_t L_2 = V_0;
		return (intptr_t)L_2;
	}
}
// System.IntPtr UnityEngine.AndroidJNISafe::ToIntArray(System.Int32[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_ToIntArray_m324EDE9CCF1C9909444C40617BD3358172EFB874 (Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* ___array0, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
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
		Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* L_0 = ___array0;
		intptr_t L_1 = AndroidJNI_ToIntArray_mB69CEC2992884ADC394A9A7E604967B7B57651A9(L_0, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		intptr_t L_2 = V_0;
		return (intptr_t)L_2;
	}
}
// System.IntPtr UnityEngine.AndroidJNISafe::GetObjectArrayElement(System.IntPtr,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJNISafe_GetObjectArrayElement_mA87BFEFBCE1C7D1B5B817CCCB5D4B7F009FD37BD (intptr_t ___array0, int32_t ___index1, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
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
		intptr_t L_0 = ___array0;
		int32_t L_1 = ___index1;
		intptr_t L_2 = AndroidJNI_GetObjectArrayElement_m104E43629B8731ACAF53A5D351CCB19398A75648((intptr_t)L_0, L_1, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_2;
		IL2CPP_LEAVE(0x17, FINALLY_000f);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000f;
	}

FINALLY_000f:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(15)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(15)
	{
		IL2CPP_JUMP_TBL(0x17, IL_0017)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0017:
	{
		intptr_t L_3 = V_0;
		return (intptr_t)L_3;
	}
}
// System.Int32 UnityEngine.AndroidJNISafe::GetArrayLength(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t AndroidJNISafe_GetArrayLength_m11614663772194842C0D75FB8C6FBDB92F8DEE05 (intptr_t ___array0, const RuntimeMethod* method)
{
	int32_t V_0 = 0;
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
		intptr_t L_0 = ___array0;
		int32_t L_1 = AndroidJNI_GetArrayLength_m3DD9BD96B89F86A4F8AAB10147CAADB951E49936((intptr_t)L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		IL2CPP_LEAVE(0x16, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		AndroidJNISafe_CheckException_m39B8553ABAD4AFD5D34089327D3179870E168B9C(/*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x16, IL_0016)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0016:
	{
		int32_t L_2 = V_0;
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
// System.Void UnityEngine.AndroidJavaClass::.ctor(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaClass__ctor_mAE416E812DB3911279C0FE87A7760247CE1BBFA8 (AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * __this, String_t* ___className0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJavaClass__ctor_mAE416E812DB3911279C0FE87A7760247CE1BBFA8_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var);
		AndroidJavaObject__ctor_m4C0CDAB96B807BB04E2C43609F16865034A60001(__this, /*hidden argument*/NULL);
		String_t* L_0 = ___className0;
		AndroidJavaClass__AndroidJavaClass_mBF3C92E82722125793A66F20C92BAE17F0CB02D9(__this, L_0, /*hidden argument*/NULL);
		return;
	}
}
// System.Void UnityEngine.AndroidJavaClass::.ctor(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaClass__ctor_m44A6DEC0612D768E9947FFC1C2DA64D0605F34F1 (AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * __this, intptr_t ___jclass0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJavaClass__ctor_m44A6DEC0612D768E9947FFC1C2DA64D0605F34F1_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var);
		AndroidJavaObject__ctor_m4C0CDAB96B807BB04E2C43609F16865034A60001(__this, /*hidden argument*/NULL);
		intptr_t L_0 = ___jclass0;
		bool L_1 = IntPtr_op_Equality_mEE8D9FD2DFE312BBAA8B4ED3BF7976B3142A5934((intptr_t)L_0, (intptr_t)(0), /*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_0023;
		}
	}
	{
		Exception_t * L_2 = (Exception_t *)il2cpp_codegen_object_new(Exception_t_il2cpp_TypeInfo_var);
		Exception__ctor_m89BADFF36C3B170013878726E07729D51AA9FBE0(L_2, _stringLiteral04B84BF4B23F7D8F289DDA3DFB2F69943E579890, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_2, NULL, AndroidJavaClass__ctor_m44A6DEC0612D768E9947FFC1C2DA64D0605F34F1_RuntimeMethod_var);
	}

IL_0023:
	{
		intptr_t L_3 = ___jclass0;
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_4 = (GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 *)il2cpp_codegen_object_new(GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0_il2cpp_TypeInfo_var);
		GlobalJavaObjectRef__ctor_m5581A68DC5217545E13F48ACF2DAFD9DF30396BC(L_4, (intptr_t)L_3, /*hidden argument*/NULL);
		((AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)__this)->set_m_jclass_2(L_4);
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_5 = (GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 *)il2cpp_codegen_object_new(GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0_il2cpp_TypeInfo_var);
		GlobalJavaObjectRef__ctor_m5581A68DC5217545E13F48ACF2DAFD9DF30396BC(L_5, (intptr_t)(0), /*hidden argument*/NULL);
		((AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)__this)->set_m_jobject_1(L_5);
		return;
	}
}
// System.Void UnityEngine.AndroidJavaClass::_AndroidJavaClass(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaClass__AndroidJavaClass_mBF3C92E82722125793A66F20C92BAE17F0CB02D9 (AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * __this, String_t* ___className0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJavaClass__AndroidJavaClass_mBF3C92E82722125793A66F20C92BAE17F0CB02D9_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		String_t* L_0 = ___className0;
		String_t* L_1 = String_Concat_mB78D0094592718DA6D5DB6C712A9C225631666BE(_stringLiteralE7CB2263F2E05D6BC8A91144ABC41E52B1F3CCFD, L_0, /*hidden argument*/NULL);
		AndroidJavaObject_DebugPrint_m88F06202527BA5A2848C1533C8B396702D112531(__this, L_1, /*hidden argument*/NULL);
		String_t* L_2 = ___className0;
		NullCheck(L_2);
		String_t* L_3 = String_Replace_m276641366A463205C185A9B3DC0E24ECB95122C9(L_2, ((int32_t)46), ((int32_t)47), /*hidden argument*/NULL);
		intptr_t L_4 = AndroidJNISafe_FindClass_mE58501828AA09ADC26347853AFE6D025845D487C(L_3, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_4;
		intptr_t L_5 = V_0;
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_6 = (GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 *)il2cpp_codegen_object_new(GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0_il2cpp_TypeInfo_var);
		GlobalJavaObjectRef__ctor_m5581A68DC5217545E13F48ACF2DAFD9DF30396BC(L_6, (intptr_t)L_5, /*hidden argument*/NULL);
		((AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)__this)->set_m_jclass_2(L_6);
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_7 = (GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 *)il2cpp_codegen_object_new(GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0_il2cpp_TypeInfo_var);
		GlobalJavaObjectRef__ctor_m5581A68DC5217545E13F48ACF2DAFD9DF30396BC(L_7, (intptr_t)(0), /*hidden argument*/NULL);
		((AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)__this)->set_m_jobject_1(L_7);
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
// System.Void UnityEngine.AndroidJavaException::.ctor(System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaException__ctor_m8E5216F0181090FB7A9016AED78B7935019791D8 (AndroidJavaException_tC81E6FAAA4067CBA537727328D5D2DB14F5F5466 * __this, String_t* ___message0, String_t* ___javaStackTrace1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJavaException__ctor_m8E5216F0181090FB7A9016AED78B7935019791D8_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		String_t* L_0 = ___message0;
		IL2CPP_RUNTIME_CLASS_INIT(Exception_t_il2cpp_TypeInfo_var);
		Exception__ctor_m89BADFF36C3B170013878726E07729D51AA9FBE0(__this, L_0, /*hidden argument*/NULL);
		String_t* L_1 = ___javaStackTrace1;
		__this->set_mJavaStackTrace_17(L_1);
		return;
	}
}
// System.String UnityEngine.AndroidJavaException::get_StackTrace()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* AndroidJavaException_get_StackTrace_m0CBD35F72DF136212F27E63C3BAF4FB7D956C710 (AndroidJavaException_tC81E6FAAA4067CBA537727328D5D2DB14F5F5466 * __this, const RuntimeMethod* method)
{
	String_t* V_0 = NULL;
	{
		String_t* L_0 = __this->get_mJavaStackTrace_17();
		String_t* L_1 = Exception_get_StackTrace_mF54AABBF2569597935F88AAF7BCD29C6639F8306(__this, /*hidden argument*/NULL);
		String_t* L_2 = String_Concat_mB78D0094592718DA6D5DB6C712A9C225631666BE(L_0, L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		goto IL_0018;
	}

IL_0018:
	{
		String_t* L_3 = V_0;
		return L_3;
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
// System.Void UnityEngine.AndroidJavaObject::.ctor(System.String,System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaObject__ctor_m39462EAD9AD82CBD90DEB4B7127F3D6C87A02BFA (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___className0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method)
{
	{
		AndroidJavaObject__ctor_m4C0CDAB96B807BB04E2C43609F16865034A60001(__this, /*hidden argument*/NULL);
		String_t* L_0 = ___className0;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_1 = ___args1;
		AndroidJavaObject__AndroidJavaObject_m596F928EE49384D7C7455920BA6ADFB2D9540CFA(__this, L_0, L_1, /*hidden argument*/NULL);
		return;
	}
}
// System.Void UnityEngine.AndroidJavaObject::.ctor(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaObject__ctor_m22E1E2E5D9F3DA31FF7DFB1339AD3BB0C3813E80 (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, intptr_t ___jobject0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJavaObject__ctor_m22E1E2E5D9F3DA31FF7DFB1339AD3BB0C3813E80_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		AndroidJavaObject__ctor_m4C0CDAB96B807BB04E2C43609F16865034A60001(__this, /*hidden argument*/NULL);
		intptr_t L_0 = ___jobject0;
		bool L_1 = IntPtr_op_Equality_mEE8D9FD2DFE312BBAA8B4ED3BF7976B3142A5934((intptr_t)L_0, (intptr_t)(0), /*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_0023;
		}
	}
	{
		Exception_t * L_2 = (Exception_t *)il2cpp_codegen_object_new(Exception_t_il2cpp_TypeInfo_var);
		Exception__ctor_m89BADFF36C3B170013878726E07729D51AA9FBE0(L_2, _stringLiteral1F96368984CDA7947E8A4265D0BD7C61AC6EB290, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_2, NULL, AndroidJavaObject__ctor_m22E1E2E5D9F3DA31FF7DFB1339AD3BB0C3813E80_RuntimeMethod_var);
	}

IL_0023:
	{
		intptr_t L_3 = ___jobject0;
		intptr_t L_4 = AndroidJNISafe_GetObjectClass_mB36866622A9FD487DCA6926F63038E5584B35BFB((intptr_t)L_3, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_4;
		intptr_t L_5 = ___jobject0;
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_6 = (GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 *)il2cpp_codegen_object_new(GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0_il2cpp_TypeInfo_var);
		GlobalJavaObjectRef__ctor_m5581A68DC5217545E13F48ACF2DAFD9DF30396BC(L_6, (intptr_t)L_5, /*hidden argument*/NULL);
		__this->set_m_jobject_1(L_6);
		intptr_t L_7 = V_0;
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_8 = (GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 *)il2cpp_codegen_object_new(GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0_il2cpp_TypeInfo_var);
		GlobalJavaObjectRef__ctor_m5581A68DC5217545E13F48ACF2DAFD9DF30396BC(L_8, (intptr_t)L_7, /*hidden argument*/NULL);
		__this->set_m_jclass_2(L_8);
		intptr_t L_9 = V_0;
		AndroidJNISafe_DeleteLocalRef_m9632EA13BF03AEE43FC7713125962A4D0DFFADC7((intptr_t)L_9, /*hidden argument*/NULL);
		return;
	}
}
// System.Void UnityEngine.AndroidJavaObject::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaObject__ctor_m4C0CDAB96B807BB04E2C43609F16865034A60001 (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, const RuntimeMethod* method)
{
	{
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		return;
	}
}
// System.Void UnityEngine.AndroidJavaObject::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaObject_Dispose_m02D1B6D8F3E902E5F0D181BF6C1753856B0DE144 (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJavaObject_Dispose_m02D1B6D8F3E902E5F0D181BF6C1753856B0DE144_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		VirtActionInvoker1< bool >::Invoke(5 /* System.Void UnityEngine.AndroidJavaObject::Dispose(System.Boolean) */, __this, (bool)1);
		IL2CPP_RUNTIME_CLASS_INIT(GC_tC1D7BD74E8F44ECCEF5CD2B5D84BFF9AAE02D01D_il2cpp_TypeInfo_var);
		GC_SuppressFinalize_m037319A9B95A5BA437E806DE592802225EE5B425(__this, /*hidden argument*/NULL);
		return;
	}
}
// System.IntPtr UnityEngine.AndroidJavaObject::GetRawObject()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJavaObject_GetRawObject_mCEB7EEC51D62A3E4F0D6F62C08CBEF008B556F3D (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		intptr_t L_0 = AndroidJavaObject__GetRawObject_m4B415E770E265AE32F5523DF0E627626F77E572F(__this, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_0;
		goto IL_000d;
	}

IL_000d:
	{
		intptr_t L_1 = V_0;
		return (intptr_t)L_1;
	}
}
// System.IntPtr UnityEngine.AndroidJavaObject::GetRawClass()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJavaObject_GetRawClass_m28BFE7AD6A4FFCB45929D9D1A0F8D792C3974774 (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		intptr_t L_0 = AndroidJavaObject__GetRawClass_m1B3729CDBBC212E0C706256FF16D2F437F618435(__this, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_0;
		goto IL_000d;
	}

IL_000d:
	{
		intptr_t L_1 = V_0;
		return (intptr_t)L_1;
	}
}
// System.Void UnityEngine.AndroidJavaObject::DebugPrint(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaObject_DebugPrint_m88F06202527BA5A2848C1533C8B396702D112531 (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___msg0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJavaObject_DebugPrint_m88F06202527BA5A2848C1533C8B396702D112531_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var);
		bool L_0 = ((AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_StaticFields*)il2cpp_codegen_static_fields_for(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var))->get_enableDebugPrints_0();
		if (L_0)
		{
			goto IL_0010;
		}
	}
	{
		goto IL_0016;
	}

IL_0010:
	{
		String_t* L_1 = ___msg0;
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_Log_m4B7C70BAFD477C6BDB59C88A0934F0B018D03708(L_1, /*hidden argument*/NULL);
	}

IL_0016:
	{
		return;
	}
}
// System.Void UnityEngine.AndroidJavaObject::_AndroidJavaObject(System.String,System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaObject__AndroidJavaObject_m596F928EE49384D7C7455920BA6ADFB2D9540CFA (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, String_t* ___className0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJavaObject__AndroidJavaObject_m596F928EE49384D7C7455920BA6ADFB2D9540CFA_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* V_1 = NULL;
	intptr_t V_2;
	memset((&V_2), 0, sizeof(V_2));
	intptr_t V_3;
	memset((&V_3), 0, sizeof(V_3));
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 1);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	{
		String_t* L_0 = ___className0;
		String_t* L_1 = String_Concat_mB78D0094592718DA6D5DB6C712A9C225631666BE(_stringLiteral1F3F048FA394EC532DD74D6A71C33F30C7F7C70C, L_0, /*hidden argument*/NULL);
		AndroidJavaObject_DebugPrint_m88F06202527BA5A2848C1533C8B396702D112531(__this, L_1, /*hidden argument*/NULL);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_2 = ___args1;
		if (L_2)
		{
			goto IL_0020;
		}
	}
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_3 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)1);
		___args1 = L_3;
	}

IL_0020:
	{
		String_t* L_4 = ___className0;
		NullCheck(L_4);
		String_t* L_5 = String_Replace_m276641366A463205C185A9B3DC0E24ECB95122C9(L_4, ((int32_t)46), ((int32_t)47), /*hidden argument*/NULL);
		intptr_t L_6 = AndroidJNISafe_FindClass_mE58501828AA09ADC26347853AFE6D025845D487C(L_5, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_6;
		intptr_t L_7 = V_0;
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_8 = (GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 *)il2cpp_codegen_object_new(GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0_il2cpp_TypeInfo_var);
		GlobalJavaObjectRef__ctor_m5581A68DC5217545E13F48ACF2DAFD9DF30396BC(L_8, (intptr_t)L_7, /*hidden argument*/NULL);
		__this->set_m_jclass_2(L_8);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_9 = ___args1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_10 = AndroidJNIHelper_CreateJNIArgArray_mAA5972FD580D58FA3D30B4E97B9837B439231F34(L_9, /*hidden argument*/NULL);
		V_1 = L_10;
	}

IL_0043:
	try
	{ // begin try (depth: 1)
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_11 = __this->get_m_jclass_2();
		intptr_t L_12 = GlobalJavaObjectRef_op_Implicit_m1F52DE72C8F8B11E651F8B31879ED5AFD413EDFF(L_11, /*hidden argument*/NULL);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_13 = ___args1;
		intptr_t L_14 = AndroidJNIHelper_GetConstructorID_m2756A393612A1CF86E3E73109E2268D9933F9F1E((intptr_t)L_12, L_13, /*hidden argument*/NULL);
		V_2 = (intptr_t)L_14;
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_15 = __this->get_m_jclass_2();
		intptr_t L_16 = GlobalJavaObjectRef_op_Implicit_m1F52DE72C8F8B11E651F8B31879ED5AFD413EDFF(L_15, /*hidden argument*/NULL);
		intptr_t L_17 = V_2;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_18 = V_1;
		intptr_t L_19 = AndroidJNISafe_NewObject_m78BDA85E651167163148C9B39DEA8CE831EB1DB0((intptr_t)L_16, (intptr_t)L_17, L_18, /*hidden argument*/NULL);
		V_3 = (intptr_t)L_19;
		intptr_t L_20 = V_3;
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_21 = (GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 *)il2cpp_codegen_object_new(GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0_il2cpp_TypeInfo_var);
		GlobalJavaObjectRef__ctor_m5581A68DC5217545E13F48ACF2DAFD9DF30396BC(L_21, (intptr_t)L_20, /*hidden argument*/NULL);
		__this->set_m_jobject_1(L_21);
		intptr_t L_22 = V_3;
		AndroidJNISafe_DeleteLocalRef_m9632EA13BF03AEE43FC7713125962A4D0DFFADC7((intptr_t)L_22, /*hidden argument*/NULL);
		IL2CPP_LEAVE(0x8B, FINALLY_0081);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0081;
	}

FINALLY_0081:
	{ // begin finally (depth: 1)
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_23 = ___args1;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_24 = V_1;
		AndroidJNIHelper_DeleteJNIArgArray_mEDFD8275CF10A3E0777350597633378776673784(L_23, L_24, /*hidden argument*/NULL);
		IL2CPP_END_FINALLY(129)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(129)
	{
		IL2CPP_JUMP_TBL(0x8B, IL_008b)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_008b:
	{
		return;
	}
}
// System.Void UnityEngine.AndroidJavaObject::Finalize()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaObject_Finalize_m834AA4594A7070A6DE1CA884752D2928ACAF2AF0 (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, const RuntimeMethod* method)
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
		VirtActionInvoker1< bool >::Invoke(5 /* System.Void UnityEngine.AndroidJavaObject::Dispose(System.Boolean) */, __this, (bool)1);
		IL2CPP_LEAVE(0x14, FINALLY_000d);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000d;
	}

FINALLY_000d:
	{ // begin finally (depth: 1)
		Object_Finalize_m4015B7D3A44DE125C5FE34D7276CD4697C06F380(__this, /*hidden argument*/NULL);
		IL2CPP_END_FINALLY(13)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(13)
	{
		IL2CPP_JUMP_TBL(0x14, IL_0014)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0014:
	{
		return;
	}
}
// System.Void UnityEngine.AndroidJavaObject::Dispose(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaObject_Dispose_m5F40DCA32137A2280BE224A63A89B8FE637619DA (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, bool ___disposing0, const RuntimeMethod* method)
{
	{
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_0 = __this->get_m_jobject_1();
		if (!L_0)
		{
			goto IL_0020;
		}
	}
	{
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_1 = __this->get_m_jobject_1();
		NullCheck(L_1);
		GlobalJavaObjectRef_Dispose_mDCFD34D040E7B4ACE886336F3659316D1A45599F(L_1, /*hidden argument*/NULL);
		__this->set_m_jobject_1((GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 *)NULL);
	}

IL_0020:
	{
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_2 = __this->get_m_jclass_2();
		if (!L_2)
		{
			goto IL_003f;
		}
	}
	{
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_3 = __this->get_m_jclass_2();
		NullCheck(L_3);
		GlobalJavaObjectRef_Dispose_mDCFD34D040E7B4ACE886336F3659316D1A45599F(L_3, /*hidden argument*/NULL);
		__this->set_m_jclass_2((GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 *)NULL);
	}

IL_003f:
	{
		return;
	}
}
// UnityEngine.AndroidJavaObject UnityEngine.AndroidJavaObject::AndroidJavaObjectDeleteLocalRef(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * AndroidJavaObject_AndroidJavaObjectDeleteLocalRef_m0B0BCBDD56C299AC69938BDD4135E1B6EEAAC7EF (intptr_t ___jobject0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJavaObject_AndroidJavaObjectDeleteLocalRef_m0B0BCBDD56C299AC69938BDD4135E1B6EEAAC7EF_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * V_0 = NULL;
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
		intptr_t L_0 = ___jobject0;
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_1 = (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)il2cpp_codegen_object_new(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var);
		AndroidJavaObject__ctor_m22E1E2E5D9F3DA31FF7DFB1339AD3BB0C3813E80(L_1, (intptr_t)L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		IL2CPP_LEAVE(0x17, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		intptr_t L_2 = ___jobject0;
		AndroidJNISafe_DeleteLocalRef_m9632EA13BF03AEE43FC7713125962A4D0DFFADC7((intptr_t)L_2, /*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x17, IL_0017)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0017:
	{
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_3 = V_0;
		return L_3;
	}
}
// UnityEngine.AndroidJavaClass UnityEngine.AndroidJavaObject::AndroidJavaClassDeleteLocalRef(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * AndroidJavaObject_AndroidJavaClassDeleteLocalRef_mD137411129D4E0B5AB858EAE367EBBA0E668D962 (intptr_t ___jclass0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJavaObject_AndroidJavaClassDeleteLocalRef_mD137411129D4E0B5AB858EAE367EBBA0E668D962_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * V_0 = NULL;
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
		intptr_t L_0 = ___jclass0;
		AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * L_1 = (AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE *)il2cpp_codegen_object_new(AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE_il2cpp_TypeInfo_var);
		AndroidJavaClass__ctor_m44A6DEC0612D768E9947FFC1C2DA64D0605F34F1(L_1, (intptr_t)L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		IL2CPP_LEAVE(0x17, FINALLY_000e);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_000e;
	}

FINALLY_000e:
	{ // begin finally (depth: 1)
		intptr_t L_2 = ___jclass0;
		AndroidJNISafe_DeleteLocalRef_m9632EA13BF03AEE43FC7713125962A4D0DFFADC7((intptr_t)L_2, /*hidden argument*/NULL);
		IL2CPP_END_FINALLY(14)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(14)
	{
		IL2CPP_JUMP_TBL(0x17, IL_0017)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0017:
	{
		AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * L_3 = V_0;
		return L_3;
	}
}
// System.IntPtr UnityEngine.AndroidJavaObject::_GetRawObject()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJavaObject__GetRawObject_m4B415E770E265AE32F5523DF0E627626F77E572F (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_0 = __this->get_m_jobject_1();
		intptr_t L_1 = GlobalJavaObjectRef_op_Implicit_m1F52DE72C8F8B11E651F8B31879ED5AFD413EDFF(L_0, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_1;
		goto IL_0012;
	}

IL_0012:
	{
		intptr_t L_2 = V_0;
		return (intptr_t)L_2;
	}
}
// System.IntPtr UnityEngine.AndroidJavaObject::_GetRawClass()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJavaObject__GetRawClass_m1B3729CDBBC212E0C706256FF16D2F437F618435 (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * __this, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_0 = __this->get_m_jclass_2();
		intptr_t L_1 = GlobalJavaObjectRef_op_Implicit_m1F52DE72C8F8B11E651F8B31879ED5AFD413EDFF(L_0, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_1;
		goto IL_0012;
	}

IL_0012:
	{
		intptr_t L_2 = V_0;
		return (intptr_t)L_2;
	}
}
// System.Void UnityEngine.AndroidJavaObject::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaObject__cctor_m46EF3B9E61C141E07E12762F96F777EA8D1A4629 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJavaObject__cctor_m46EF3B9E61C141E07E12762F96F777EA8D1A4629_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		((AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_StaticFields*)il2cpp_codegen_static_fields_for(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var))->set_enableDebugPrints_0((bool)0);
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
// System.Void UnityEngine.AndroidJavaProxy::.ctor(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaProxy__ctor_m159565DEF4041D92C0763D1F4A0684140241CD9A (AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D * __this, String_t* ___javaInterface0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJavaProxy__ctor_m159565DEF4041D92C0763D1F4A0684140241CD9A_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		String_t* L_0 = ___javaInterface0;
		AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * L_1 = (AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE *)il2cpp_codegen_object_new(AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE_il2cpp_TypeInfo_var);
		AndroidJavaClass__ctor_mAE416E812DB3911279C0FE87A7760247CE1BBFA8(L_1, L_0, /*hidden argument*/NULL);
		AndroidJavaProxy__ctor_m9A2D1F4BF0E7803070D68D3C386F4218D3BCAC0F(__this, L_1, /*hidden argument*/NULL);
		return;
	}
}
// System.Void UnityEngine.AndroidJavaProxy::.ctor(UnityEngine.AndroidJavaClass)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaProxy__ctor_m9A2D1F4BF0E7803070D68D3C386F4218D3BCAC0F (AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D * __this, AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * ___javaInterface0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJavaProxy__ctor_m9A2D1F4BF0E7803070D68D3C386F4218D3BCAC0F_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		__this->set_proxyObject_1((intptr_t)(0));
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * L_0 = ___javaInterface0;
		__this->set_javaInterface_0(L_0);
		return;
	}
}
// System.Void UnityEngine.AndroidJavaProxy::Finalize()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaProxy_Finalize_mB53473746276958436FE45332CD82C6847F14D73 (AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D * __this, const RuntimeMethod* method)
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
		intptr_t L_0 = __this->get_proxyObject_1();
		AndroidJNISafe_DeleteWeakGlobalRef_mB338C2F7116360905B7F444BDB16CAB18B914ED3((intptr_t)L_0, /*hidden argument*/NULL);
		IL2CPP_LEAVE(0x18, FINALLY_0011);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0011;
	}

FINALLY_0011:
	{ // begin finally (depth: 1)
		Object_Finalize_m4015B7D3A44DE125C5FE34D7276CD4697C06F380(__this, /*hidden argument*/NULL);
		IL2CPP_END_FINALLY(17)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(17)
	{
		IL2CPP_JUMP_TBL(0x18, IL_0018)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0018:
	{
		return;
	}
}
// UnityEngine.AndroidJavaObject UnityEngine.AndroidJavaProxy::Invoke(System.String,System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * AndroidJavaProxy_Invoke_m2A4BA59C6A517E0B692478676AA0A0A77980848E (AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D * __this, String_t* ___methodName0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJavaProxy_Invoke_m2A4BA59C6A517E0B692478676AA0A0A77980848E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Exception_t * V_0 = NULL;
	int32_t V_1 = 0;
	TypeU5BU5D_t7FE623A666B49176DE123306221193E888A12F5F* V_2 = NULL;
	int32_t V_3 = 0;
	MethodInfo_t * V_4 = NULL;
	AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * V_5 = NULL;
	TargetInvocationException_t0DD35F6083E1D1E0509BF181A79C76D3339D89B8 * V_6 = NULL;
	Exception_t * V_7 = NULL;
	StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* V_8 = NULL;
	int32_t V_9 = 0;
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 4);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	int32_t G_B3_0 = 0;
	TypeU5BU5D_t7FE623A666B49176DE123306221193E888A12F5F* G_B3_1 = NULL;
	int32_t G_B2_0 = 0;
	TypeU5BU5D_t7FE623A666B49176DE123306221193E888A12F5F* G_B2_1 = NULL;
	Type_t * G_B4_0 = NULL;
	int32_t G_B4_1 = 0;
	TypeU5BU5D_t7FE623A666B49176DE123306221193E888A12F5F* G_B4_2 = NULL;
	{
		V_0 = (Exception_t *)NULL;
		V_1 = ((int32_t)60);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_0 = ___args1;
		NullCheck(L_0);
		TypeU5BU5D_t7FE623A666B49176DE123306221193E888A12F5F* L_1 = (TypeU5BU5D_t7FE623A666B49176DE123306221193E888A12F5F*)(TypeU5BU5D_t7FE623A666B49176DE123306221193E888A12F5F*)SZArrayNew(TypeU5BU5D_t7FE623A666B49176DE123306221193E888A12F5F_il2cpp_TypeInfo_var, (uint32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_0)->max_length)))));
		V_2 = L_1;
		V_3 = 0;
		goto IL_003c;
	}

IL_0016:
	{
		TypeU5BU5D_t7FE623A666B49176DE123306221193E888A12F5F* L_2 = V_2;
		int32_t L_3 = V_3;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_4 = ___args1;
		int32_t L_5 = V_3;
		NullCheck(L_4);
		int32_t L_6 = L_5;
		RuntimeObject * L_7 = (L_4)->GetAt(static_cast<il2cpp_array_size_t>(L_6));
		G_B2_0 = L_3;
		G_B2_1 = L_2;
		if (L_7)
		{
			G_B3_0 = L_3;
			G_B3_1 = L_2;
			goto IL_002f;
		}
	}
	{
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_8 = { reinterpret_cast<intptr_t> (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_9 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_8, /*hidden argument*/NULL);
		G_B4_0 = L_9;
		G_B4_1 = G_B2_0;
		G_B4_2 = G_B2_1;
		goto IL_0037;
	}

IL_002f:
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_10 = ___args1;
		int32_t L_11 = V_3;
		NullCheck(L_10);
		int32_t L_12 = L_11;
		RuntimeObject * L_13 = (L_10)->GetAt(static_cast<il2cpp_array_size_t>(L_12));
		NullCheck(L_13);
		Type_t * L_14 = Object_GetType_m2E0B62414ECCAA3094B703790CE88CBB2F83EA60(L_13, /*hidden argument*/NULL);
		G_B4_0 = L_14;
		G_B4_1 = G_B3_0;
		G_B4_2 = G_B3_1;
	}

IL_0037:
	{
		NullCheck(G_B4_2);
		ArrayElementTypeCheck (G_B4_2, G_B4_0);
		(G_B4_2)->SetAt(static_cast<il2cpp_array_size_t>(G_B4_1), (Type_t *)G_B4_0);
		int32_t L_15 = V_3;
		V_3 = ((int32_t)il2cpp_codegen_add((int32_t)L_15, (int32_t)1));
	}

IL_003c:
	{
		int32_t L_16 = V_3;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_17 = ___args1;
		NullCheck(L_17);
		if ((((int32_t)L_16) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_17)->max_length)))))))
		{
			goto IL_0016;
		}
	}

IL_0045:
	try
	{ // begin try (depth: 1)
		{
			Type_t * L_18 = Object_GetType_m2E0B62414ECCAA3094B703790CE88CBB2F83EA60(__this, /*hidden argument*/NULL);
			String_t* L_19 = ___methodName0;
			int32_t L_20 = V_1;
			TypeU5BU5D_t7FE623A666B49176DE123306221193E888A12F5F* L_21 = V_2;
			NullCheck(L_18);
			MethodInfo_t * L_22 = Type_GetMethod_m694F07057F23808980BF6B1637544F34852759FA(L_18, L_19, L_20, (Binder_t4D5CB06963501D32847C057B57157D6DC49CA759 *)NULL, L_21, (ParameterModifierU5BU5D_t63EC46F14F048DC9EF6BF1362E8AEBEA1A05A5EA*)(ParameterModifierU5BU5D_t63EC46F14F048DC9EF6BF1362E8AEBEA1A05A5EA*)NULL, /*hidden argument*/NULL);
			V_4 = L_22;
			MethodInfo_t * L_23 = V_4;
			if (!L_23)
			{
				goto IL_0074;
			}
		}

IL_005f:
		{
			MethodInfo_t * L_24 = V_4;
			ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_25 = ___args1;
			NullCheck(L_24);
			RuntimeObject * L_26 = MethodBase_Invoke_m471794D56262D9DB5B5A324883030AB16BD39674(L_24, __this, L_25, /*hidden argument*/NULL);
			AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_27 = _AndroidJNIHelper_Box_m67A2A786DCE5ADD2FAF4F27B7CA115C82A8768C2(L_26, /*hidden argument*/NULL);
			V_5 = L_27;
			goto IL_0173;
		}

IL_0074:
		{
			goto IL_0097;
		}
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__exception_local = (Exception_t *)e.ex;
		if(il2cpp_codegen_class_is_assignable_from (TargetInvocationException_t0DD35F6083E1D1E0509BF181A79C76D3339D89B8_il2cpp_TypeInfo_var, il2cpp_codegen_object_class(e.ex)))
			goto CATCH_007a;
		if(il2cpp_codegen_class_is_assignable_from (Exception_t_il2cpp_TypeInfo_var, il2cpp_codegen_object_class(e.ex)))
			goto CATCH_008b;
		throw e;
	}

CATCH_007a:
	{ // begin catch(System.Reflection.TargetInvocationException)
		V_6 = ((TargetInvocationException_t0DD35F6083E1D1E0509BF181A79C76D3339D89B8 *)__exception_local);
		TargetInvocationException_t0DD35F6083E1D1E0509BF181A79C76D3339D89B8 * L_28 = V_6;
		NullCheck(L_28);
		Exception_t * L_29 = Exception_get_InnerException_mCB68CC8CBF2540EF381CB17A4E4E3F6D0E33453F_inline(L_28, /*hidden argument*/NULL);
		V_0 = L_29;
		goto IL_0097;
	} // end catch (depth: 1)

CATCH_008b:
	{ // begin catch(System.Exception)
		V_7 = ((Exception_t *)__exception_local);
		Exception_t * L_30 = V_7;
		V_0 = L_30;
		goto IL_0097;
	} // end catch (depth: 1)

IL_0097:
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_31 = ___args1;
		NullCheck(L_31);
		StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* L_32 = (StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E*)(StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E*)SZArrayNew(StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E_il2cpp_TypeInfo_var, (uint32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_31)->max_length)))));
		V_8 = L_32;
		V_9 = 0;
		goto IL_00bd;
	}

IL_00a9:
	{
		StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* L_33 = V_8;
		int32_t L_34 = V_9;
		TypeU5BU5D_t7FE623A666B49176DE123306221193E888A12F5F* L_35 = V_2;
		int32_t L_36 = V_9;
		NullCheck(L_35);
		int32_t L_37 = L_36;
		Type_t * L_38 = (L_35)->GetAt(static_cast<il2cpp_array_size_t>(L_37));
		NullCheck(L_38);
		String_t* L_39 = VirtFuncInvoker0< String_t* >::Invoke(3 /* System.String System.Object::ToString() */, L_38);
		NullCheck(L_33);
		ArrayElementTypeCheck (L_33, L_39);
		(L_33)->SetAt(static_cast<il2cpp_array_size_t>(L_34), (String_t*)L_39);
		int32_t L_40 = V_9;
		V_9 = ((int32_t)il2cpp_codegen_add((int32_t)L_40, (int32_t)1));
	}

IL_00bd:
	{
		int32_t L_41 = V_9;
		TypeU5BU5D_t7FE623A666B49176DE123306221193E888A12F5F* L_42 = V_2;
		NullCheck(L_42);
		if ((((int32_t)L_41) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_42)->max_length)))))))
		{
			goto IL_00a9;
		}
	}
	{
		Exception_t * L_43 = V_0;
		if (!L_43)
		{
			goto IL_0113;
		}
	}
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_44 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)6);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_45 = L_44;
		Type_t * L_46 = Object_GetType_m2E0B62414ECCAA3094B703790CE88CBB2F83EA60(__this, /*hidden argument*/NULL);
		NullCheck(L_45);
		ArrayElementTypeCheck (L_45, L_46);
		(L_45)->SetAt(static_cast<il2cpp_array_size_t>(0), (RuntimeObject *)L_46);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_47 = L_45;
		NullCheck(L_47);
		ArrayElementTypeCheck (L_47, _stringLiteral3A52CE780950D4D969792A2559CD519D7EE8C727);
		(L_47)->SetAt(static_cast<il2cpp_array_size_t>(1), (RuntimeObject *)_stringLiteral3A52CE780950D4D969792A2559CD519D7EE8C727);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_48 = L_47;
		String_t* L_49 = ___methodName0;
		NullCheck(L_48);
		ArrayElementTypeCheck (L_48, L_49);
		(L_48)->SetAt(static_cast<il2cpp_array_size_t>(2), (RuntimeObject *)L_49);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_50 = L_48;
		NullCheck(L_50);
		ArrayElementTypeCheck (L_50, _stringLiteral28ED3A797DA3C48C309A4EF792147F3C56CFEC40);
		(L_50)->SetAt(static_cast<il2cpp_array_size_t>(3), (RuntimeObject *)_stringLiteral28ED3A797DA3C48C309A4EF792147F3C56CFEC40);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_51 = L_50;
		StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* L_52 = V_8;
		String_t* L_53 = String_Join_m49371BED70248F0FCE970CB4F2E39E9A688AAFA4(_stringLiteral5C10B5B2CD673A0616D529AA5234B12EE7153808, L_52, /*hidden argument*/NULL);
		NullCheck(L_51);
		ArrayElementTypeCheck (L_51, L_53);
		(L_51)->SetAt(static_cast<il2cpp_array_size_t>(4), (RuntimeObject *)L_53);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_54 = L_51;
		NullCheck(L_54);
		ArrayElementTypeCheck (L_54, _stringLiteralE7064F0B80F61DBC65915311032D27BAA569AE2A);
		(L_54)->SetAt(static_cast<il2cpp_array_size_t>(5), (RuntimeObject *)_stringLiteralE7064F0B80F61DBC65915311032D27BAA569AE2A);
		String_t* L_55 = String_Concat_mB7BA84F13912303B2E5E40FBF0109E1A328ACA07(L_54, /*hidden argument*/NULL);
		Exception_t * L_56 = V_0;
		TargetInvocationException_t0DD35F6083E1D1E0509BF181A79C76D3339D89B8 * L_57 = (TargetInvocationException_t0DD35F6083E1D1E0509BF181A79C76D3339D89B8 *)il2cpp_codegen_object_new(TargetInvocationException_t0DD35F6083E1D1E0509BF181A79C76D3339D89B8_il2cpp_TypeInfo_var);
		TargetInvocationException__ctor_mBCC339AE7AC683564DA27A950A92463915B71F00(L_57, L_55, L_56, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_57, NULL, AndroidJavaProxy_Invoke_m2A4BA59C6A517E0B692478676AA0A0A77980848E_RuntimeMethod_var);
	}

IL_0113:
	{
		intptr_t L_58 = AndroidJavaProxy_GetRawProxy_mFE7D48E72D4744E260D3ACE6D777D072002BEA6C(__this, /*hidden argument*/NULL);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_59 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)7);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_60 = L_59;
		NullCheck(L_60);
		ArrayElementTypeCheck (L_60, _stringLiteralDCB8EAD2EF149C5622DFF6CF2BD5628BDC485847);
		(L_60)->SetAt(static_cast<il2cpp_array_size_t>(0), (RuntimeObject *)_stringLiteralDCB8EAD2EF149C5622DFF6CF2BD5628BDC485847);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_61 = L_60;
		Type_t * L_62 = Object_GetType_m2E0B62414ECCAA3094B703790CE88CBB2F83EA60(__this, /*hidden argument*/NULL);
		NullCheck(L_61);
		ArrayElementTypeCheck (L_61, L_62);
		(L_61)->SetAt(static_cast<il2cpp_array_size_t>(1), (RuntimeObject *)L_62);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_63 = L_61;
		NullCheck(L_63);
		ArrayElementTypeCheck (L_63, _stringLiteral3A52CE780950D4D969792A2559CD519D7EE8C727);
		(L_63)->SetAt(static_cast<il2cpp_array_size_t>(2), (RuntimeObject *)_stringLiteral3A52CE780950D4D969792A2559CD519D7EE8C727);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_64 = L_63;
		String_t* L_65 = ___methodName0;
		NullCheck(L_64);
		ArrayElementTypeCheck (L_64, L_65);
		(L_64)->SetAt(static_cast<il2cpp_array_size_t>(3), (RuntimeObject *)L_65);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_66 = L_64;
		NullCheck(L_66);
		ArrayElementTypeCheck (L_66, _stringLiteral28ED3A797DA3C48C309A4EF792147F3C56CFEC40);
		(L_66)->SetAt(static_cast<il2cpp_array_size_t>(4), (RuntimeObject *)_stringLiteral28ED3A797DA3C48C309A4EF792147F3C56CFEC40);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_67 = L_66;
		StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* L_68 = V_8;
		String_t* L_69 = String_Join_m49371BED70248F0FCE970CB4F2E39E9A688AAFA4(_stringLiteral5C10B5B2CD673A0616D529AA5234B12EE7153808, L_68, /*hidden argument*/NULL);
		NullCheck(L_67);
		ArrayElementTypeCheck (L_67, L_69);
		(L_67)->SetAt(static_cast<il2cpp_array_size_t>(5), (RuntimeObject *)L_69);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_70 = L_67;
		NullCheck(L_70);
		ArrayElementTypeCheck (L_70, _stringLiteralE7064F0B80F61DBC65915311032D27BAA569AE2A);
		(L_70)->SetAt(static_cast<il2cpp_array_size_t>(6), (RuntimeObject *)_stringLiteralE7064F0B80F61DBC65915311032D27BAA569AE2A);
		String_t* L_71 = String_Concat_mB7BA84F13912303B2E5E40FBF0109E1A328ACA07(L_70, /*hidden argument*/NULL);
		Exception_t * L_72 = (Exception_t *)il2cpp_codegen_object_new(Exception_t_il2cpp_TypeInfo_var);
		Exception__ctor_m89BADFF36C3B170013878726E07729D51AA9FBE0(L_72, L_71, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var);
		AndroidReflection_SetNativeExceptionOnProxy_m025AFCDD8B6659D45FE3830E8AC154300DA19966((intptr_t)L_58, L_72, (bool)1, /*hidden argument*/NULL);
		V_5 = (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)NULL;
		goto IL_0173;
	}

IL_0173:
	{
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_73 = V_5;
		return L_73;
	}
}
// UnityEngine.AndroidJavaObject UnityEngine.AndroidJavaProxy::Invoke(System.String,UnityEngine.AndroidJavaObject[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * AndroidJavaProxy_Invoke_m27ACB084BB434FFEA8A1FB687CCB332F4EB80B9B (AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D * __this, String_t* ___methodName0, AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379* ___javaArgs1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJavaProxy_Invoke_m27ACB084BB434FFEA8A1FB687CCB332F4EB80B9B_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* V_0 = NULL;
	int32_t V_1 = 0;
	AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * V_2 = NULL;
	{
		AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379* L_0 = ___javaArgs1;
		NullCheck(L_0);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_1 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_0)->max_length)))));
		V_0 = L_1;
		V_1 = 0;
		goto IL_0041;
	}

IL_0011:
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_2 = V_0;
		int32_t L_3 = V_1;
		AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379* L_4 = ___javaArgs1;
		int32_t L_5 = V_1;
		NullCheck(L_4);
		int32_t L_6 = L_5;
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_7 = (L_4)->GetAt(static_cast<il2cpp_array_size_t>(L_6));
		RuntimeObject * L_8 = _AndroidJNIHelper_Unbox_m813AFB8DE2C2568B011C81ED3AC4D013F1E5B67E(L_7, /*hidden argument*/NULL);
		NullCheck(L_2);
		ArrayElementTypeCheck (L_2, L_8);
		(L_2)->SetAt(static_cast<il2cpp_array_size_t>(L_3), (RuntimeObject *)L_8);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_9 = V_0;
		int32_t L_10 = V_1;
		NullCheck(L_9);
		int32_t L_11 = L_10;
		RuntimeObject * L_12 = (L_9)->GetAt(static_cast<il2cpp_array_size_t>(L_11));
		if (((AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)IsInstClass((RuntimeObject*)L_12, AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var)))
		{
			goto IL_003c;
		}
	}
	{
		AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379* L_13 = ___javaArgs1;
		int32_t L_14 = V_1;
		NullCheck(L_13);
		int32_t L_15 = L_14;
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_16 = (L_13)->GetAt(static_cast<il2cpp_array_size_t>(L_15));
		if (!L_16)
		{
			goto IL_003b;
		}
	}
	{
		AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379* L_17 = ___javaArgs1;
		int32_t L_18 = V_1;
		NullCheck(L_17);
		int32_t L_19 = L_18;
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_20 = (L_17)->GetAt(static_cast<il2cpp_array_size_t>(L_19));
		NullCheck(L_20);
		AndroidJavaObject_Dispose_m02D1B6D8F3E902E5F0D181BF6C1753856B0DE144(L_20, /*hidden argument*/NULL);
	}

IL_003b:
	{
	}

IL_003c:
	{
		int32_t L_21 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_21, (int32_t)1));
	}

IL_0041:
	{
		int32_t L_22 = V_1;
		AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379* L_23 = ___javaArgs1;
		NullCheck(L_23);
		if ((((int32_t)L_22) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_23)->max_length)))))))
		{
			goto IL_0011;
		}
	}
	{
		String_t* L_24 = ___methodName0;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_25 = V_0;
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_26 = VirtFuncInvoker2< AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *, String_t*, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* >::Invoke(4 /* UnityEngine.AndroidJavaObject UnityEngine.AndroidJavaProxy::Invoke(System.String,System.Object[]) */, __this, L_24, L_25);
		V_2 = L_26;
		goto IL_0058;
	}

IL_0058:
	{
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_27 = V_2;
		return L_27;
	}
}
// UnityEngine.AndroidJavaObject UnityEngine.AndroidJavaProxy::GetProxyObject()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * AndroidJavaProxy_GetProxyObject_m411DC59BF56152B6058ABF99BBC8B64C813EEF06 (AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJavaProxy_GetProxyObject_m411DC59BF56152B6058ABF99BBC8B64C813EEF06_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * V_0 = NULL;
	{
		intptr_t L_0 = AndroidJavaProxy_GetRawProxy_mFE7D48E72D4744E260D3ACE6D777D072002BEA6C(__this, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var);
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_1 = AndroidJavaObject_AndroidJavaObjectDeleteLocalRef_m0B0BCBDD56C299AC69938BDD4135E1B6EEAAC7EF((intptr_t)L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		goto IL_0012;
	}

IL_0012:
	{
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_2 = V_0;
		return L_2;
	}
}
// System.IntPtr UnityEngine.AndroidJavaProxy::GetRawProxy()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidJavaProxy_GetRawProxy_mFE7D48E72D4744E260D3ACE6D777D072002BEA6C (AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJavaProxy_GetRawProxy_mFE7D48E72D4744E260D3ACE6D777D072002BEA6C_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	intptr_t V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		V_0 = (intptr_t)(0);
		intptr_t L_0 = __this->get_proxyObject_1();
		bool L_1 = IntPtr_op_Inequality_mB4886A806009EA825EFCC60CD2A7F6EB8E273A61((intptr_t)L_0, (intptr_t)(0), /*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_0052;
		}
	}
	{
		intptr_t L_2 = __this->get_proxyObject_1();
		intptr_t L_3 = AndroidJNI_NewLocalRef_m22674FDA13C73173E0ECB3F59DE15CBDAD4CD039((intptr_t)L_2, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_3;
		intptr_t L_4 = V_0;
		bool L_5 = IntPtr_op_Equality_mEE8D9FD2DFE312BBAA8B4ED3BF7976B3142A5934((intptr_t)L_4, (intptr_t)(0), /*hidden argument*/NULL);
		if (!L_5)
		{
			goto IL_0051;
		}
	}
	{
		intptr_t L_6 = __this->get_proxyObject_1();
		AndroidJNI_DeleteWeakGlobalRef_m07AE954A94CDB58980A3CBA36E0E8F236BE01C75((intptr_t)L_6, /*hidden argument*/NULL);
		__this->set_proxyObject_1((intptr_t)(0));
	}

IL_0051:
	{
	}

IL_0052:
	{
		intptr_t L_7 = V_0;
		bool L_8 = IntPtr_op_Equality_mEE8D9FD2DFE312BBAA8B4ED3BF7976B3142A5934((intptr_t)L_7, (intptr_t)(0), /*hidden argument*/NULL);
		if (!L_8)
		{
			goto IL_0077;
		}
	}
	{
		intptr_t L_9 = AndroidJNIHelper_CreateJavaProxy_m29A8BD91809FF21642EA1319E5F097979EE8FA28(__this, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_9;
		intptr_t L_10 = V_0;
		intptr_t L_11 = AndroidJNI_NewWeakGlobalRef_m907BCFA1475E108FBBD02A8A425929EC859D0E8C((intptr_t)L_10, /*hidden argument*/NULL);
		__this->set_proxyObject_1((intptr_t)L_11);
	}

IL_0077:
	{
		intptr_t L_12 = V_0;
		V_1 = (intptr_t)L_12;
		goto IL_007e;
	}

IL_007e:
	{
		intptr_t L_13 = V_1;
		return (intptr_t)L_13;
	}
}
// System.Void UnityEngine.AndroidJavaProxy::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaProxy__cctor_mC5B6251AA25617F7CE1AD4DAD0BD2CCAC9636C9F (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJavaProxy__cctor_mC5B6251AA25617F7CE1AD4DAD0BD2CCAC9636C9F_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		intptr_t L_0 = AndroidJNISafe_FindClass_mE58501828AA09ADC26347853AFE6D025845D487C(_stringLiteral24D34050C2CD7CAF4904B270C05866CE090A90D7, /*hidden argument*/NULL);
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_1 = (GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 *)il2cpp_codegen_object_new(GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0_il2cpp_TypeInfo_var);
		GlobalJavaObjectRef__ctor_m5581A68DC5217545E13F48ACF2DAFD9DF30396BC(L_1, (intptr_t)L_0, /*hidden argument*/NULL);
		((AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D_StaticFields*)il2cpp_codegen_static_fields_for(AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D_il2cpp_TypeInfo_var))->set_s_JavaLangSystemClass_2(L_1);
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_2 = ((AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D_StaticFields*)il2cpp_codegen_static_fields_for(AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D_il2cpp_TypeInfo_var))->get_s_JavaLangSystemClass_2();
		intptr_t L_3 = GlobalJavaObjectRef_op_Implicit_m1F52DE72C8F8B11E651F8B31879ED5AFD413EDFF(L_2, /*hidden argument*/NULL);
		intptr_t L_4 = AndroidJNIHelper_GetMethodID_mD3057EDF00D6BBB3E89116EE05F68D0731AD9E43((intptr_t)L_3, _stringLiteral90481F4868A2DA6C9F737FD69686A6CE240E7093, _stringLiteral3AD95ED806CF8C0E154B9D74C27CBE73D5918CFB, (bool)1, /*hidden argument*/NULL);
		((AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D_StaticFields*)il2cpp_codegen_static_fields_for(AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D_il2cpp_TypeInfo_var))->set_s_HashCodeMethodID_3((intptr_t)L_4);
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
IL2CPP_EXTERN_C  void DelegatePInvokeWrapper_AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 (AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 * __this, const RuntimeMethod* method)
{
	typedef void (DEFAULT_CALL *PInvokeFunc)();
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	il2cppPInvokeFunc();

}
// System.Void UnityEngine.AndroidJavaRunnable::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaRunnable__ctor_mF5ED3CD9300D6D702748378A0E0633612866D052 (AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void UnityEngine.AndroidJavaRunnable::Invoke()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaRunnable_Invoke_m98A444239D449E61110C80E5F18D3C1D386FE79B (AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 * __this, const RuntimeMethod* method)
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
// System.IAsyncResult UnityEngine.AndroidJavaRunnable::BeginInvoke(System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* AndroidJavaRunnable_BeginInvoke_mC020AAC7B0B350E65DDA128222B54D5B10FE43ED (AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 * __this, AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4 * ___callback0, RuntimeObject * ___object1, const RuntimeMethod* method)
{
	void *__d_args[1] = {0};
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback0, (RuntimeObject*)___object1);
}
// System.Void UnityEngine.AndroidJavaRunnable::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaRunnable_EndInvoke_m4301C1FBD81C209F7056166325A35617DD10B5E9 (AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
// System.Void UnityEngine.AndroidJavaRunnableProxy::.ctor(UnityEngine.AndroidJavaRunnable)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidJavaRunnableProxy__ctor_m0D23BFCE5D99EA0AA56A5813B2E91BDDAD72C738 (AndroidJavaRunnableProxy_t3C66FEA8C2A903168F2902788AB8AB29CA923308 * __this, AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 * ___runnable0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidJavaRunnableProxy__ctor_m0D23BFCE5D99EA0AA56A5813B2E91BDDAD72C738_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D_il2cpp_TypeInfo_var);
		AndroidJavaProxy__ctor_m159565DEF4041D92C0763D1F4A0684140241CD9A(__this, _stringLiteralBC3866F48E90715EBFCDCFC327E4131E3BC40FB1, /*hidden argument*/NULL);
		AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 * L_0 = ___runnable0;
		__this->set_mRunnable_4(L_0);
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
// System.Boolean UnityEngine.AndroidReflection::IsPrimitive(System.Type)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool AndroidReflection_IsPrimitive_m4C75B1AAEDD3FA0F73AFBC83CB374D3D8A9A3749 (Type_t * ___t0, const RuntimeMethod* method)
{
	bool V_0 = false;
	{
		Type_t * L_0 = ___t0;
		NullCheck(L_0);
		bool L_1 = Type_get_IsPrimitive_m8E39430EE4B70E1AE690B51E9BE681C7758DFF5A(L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		goto IL_000d;
	}

IL_000d:
	{
		bool L_2 = V_0;
		return L_2;
	}
}
// System.Boolean UnityEngine.AndroidReflection::IsAssignableFrom(System.Type,System.Type)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool AndroidReflection_IsAssignableFrom_m000432044555172C9399EB05A11AA35BFAF790FD (Type_t * ___t0, Type_t * ___from1, const RuntimeMethod* method)
{
	bool V_0 = false;
	{
		Type_t * L_0 = ___t0;
		Type_t * L_1 = ___from1;
		NullCheck(L_0);
		bool L_2 = VirtFuncInvoker1< bool, Type_t * >::Invoke(110 /* System.Boolean System.Type::IsAssignableFrom(System.Type) */, L_0, L_1);
		V_0 = L_2;
		goto IL_000e;
	}

IL_000e:
	{
		bool L_3 = V_0;
		return L_3;
	}
}
// System.IntPtr UnityEngine.AndroidReflection::GetStaticMethodID(System.String,System.String,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidReflection_GetStaticMethodID_m1D6770C9A0BC1AA47FDA330B92743324C0441B29 (String_t* ___clazz0, String_t* ___methodName1, String_t* ___signature2, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	intptr_t V_1;
	memset((&V_1), 0, sizeof(V_1));
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 1);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	{
		String_t* L_0 = ___clazz0;
		intptr_t L_1 = AndroidJNISafe_FindClass_mE58501828AA09ADC26347853AFE6D025845D487C(L_0, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_1;
	}

IL_0008:
	try
	{ // begin try (depth: 1)
		intptr_t L_2 = V_0;
		String_t* L_3 = ___methodName1;
		String_t* L_4 = ___signature2;
		intptr_t L_5 = AndroidJNISafe_GetStaticMethodID_m4DCBC629048509F8E8566998CDA8F1AB9EAD6A50((intptr_t)L_2, L_3, L_4, /*hidden argument*/NULL);
		V_1 = (intptr_t)L_5;
		IL2CPP_LEAVE(0x20, FINALLY_0017);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0017;
	}

FINALLY_0017:
	{ // begin finally (depth: 1)
		intptr_t L_6 = V_0;
		AndroidJNISafe_DeleteLocalRef_m9632EA13BF03AEE43FC7713125962A4D0DFFADC7((intptr_t)L_6, /*hidden argument*/NULL);
		IL2CPP_END_FINALLY(23)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(23)
	{
		IL2CPP_JUMP_TBL(0x20, IL_0020)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0020:
	{
		intptr_t L_7 = V_1;
		return (intptr_t)L_7;
	}
}
// System.IntPtr UnityEngine.AndroidReflection::GetConstructorMember(System.IntPtr,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidReflection_GetConstructorMember_mE78FA3844BBB2FE5A6D3A6719BE72BD33423F4C9 (intptr_t ___jclass0, String_t* ___signature1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidReflection_GetConstructorMember_mE78FA3844BBB2FE5A6D3A6719BE72BD33423F4C9_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* V_0 = NULL;
	intptr_t V_1;
	memset((&V_1), 0, sizeof(V_1));
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 1);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	{
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_0 = (jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*)(jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*)SZArrayNew(jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3_il2cpp_TypeInfo_var, (uint32_t)2);
		V_0 = L_0;
	}

IL_0008:
	try
	{ // begin try (depth: 1)
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_1 = V_0;
		NullCheck(L_1);
		intptr_t L_2 = ___jclass0;
		((L_1)->GetAddressAt(static_cast<il2cpp_array_size_t>(0)))->set_l_8((intptr_t)L_2);
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_3 = V_0;
		NullCheck(L_3);
		String_t* L_4 = ___signature1;
		intptr_t L_5 = AndroidJNISafe_NewString_mD1D954E0EE5A8F135B19EE67E8FF2A4E1A6CA97F(L_4, /*hidden argument*/NULL);
		((L_3)->GetAddressAt(static_cast<il2cpp_array_size_t>(1)))->set_l_8((intptr_t)L_5);
		IL2CPP_RUNTIME_CLASS_INIT(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var);
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_6 = ((AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_StaticFields*)il2cpp_codegen_static_fields_for(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var))->get_s_ReflectionHelperClass_0();
		intptr_t L_7 = GlobalJavaObjectRef_op_Implicit_m1F52DE72C8F8B11E651F8B31879ED5AFD413EDFF(L_6, /*hidden argument*/NULL);
		intptr_t L_8 = ((AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_StaticFields*)il2cpp_codegen_static_fields_for(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var))->get_s_ReflectionHelperGetConstructorID_1();
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_9 = V_0;
		intptr_t L_10 = AndroidJNISafe_CallStaticObjectMethod_m11EDE005224D5A6833BFF896906397D24E19D440((intptr_t)L_7, (intptr_t)L_8, L_9, /*hidden argument*/NULL);
		V_1 = (intptr_t)L_10;
		IL2CPP_LEAVE(0x57, FINALLY_0043);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0043;
	}

FINALLY_0043:
	{ // begin finally (depth: 1)
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_11 = V_0;
		NullCheck(L_11);
		intptr_t L_12 = ((L_11)->GetAddressAt(static_cast<il2cpp_array_size_t>(1)))->get_l_8();
		AndroidJNISafe_DeleteLocalRef_m9632EA13BF03AEE43FC7713125962A4D0DFFADC7((intptr_t)L_12, /*hidden argument*/NULL);
		IL2CPP_END_FINALLY(67)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(67)
	{
		IL2CPP_JUMP_TBL(0x57, IL_0057)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0057:
	{
		intptr_t L_13 = V_1;
		return (intptr_t)L_13;
	}
}
// System.IntPtr UnityEngine.AndroidReflection::GetMethodMember(System.IntPtr,System.String,System.String,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidReflection_GetMethodMember_m0B7C41F91CA0414D70EDFF7853BA93B11157EB19 (intptr_t ___jclass0, String_t* ___methodName1, String_t* ___signature2, bool ___isStatic3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidReflection_GetMethodMember_m0B7C41F91CA0414D70EDFF7853BA93B11157EB19_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* V_0 = NULL;
	intptr_t V_1;
	memset((&V_1), 0, sizeof(V_1));
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 1);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	{
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_0 = (jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*)(jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*)SZArrayNew(jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3_il2cpp_TypeInfo_var, (uint32_t)4);
		V_0 = L_0;
	}

IL_0008:
	try
	{ // begin try (depth: 1)
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_1 = V_0;
		NullCheck(L_1);
		intptr_t L_2 = ___jclass0;
		((L_1)->GetAddressAt(static_cast<il2cpp_array_size_t>(0)))->set_l_8((intptr_t)L_2);
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_3 = V_0;
		NullCheck(L_3);
		String_t* L_4 = ___methodName1;
		intptr_t L_5 = AndroidJNISafe_NewString_mD1D954E0EE5A8F135B19EE67E8FF2A4E1A6CA97F(L_4, /*hidden argument*/NULL);
		((L_3)->GetAddressAt(static_cast<il2cpp_array_size_t>(1)))->set_l_8((intptr_t)L_5);
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_6 = V_0;
		NullCheck(L_6);
		String_t* L_7 = ___signature2;
		intptr_t L_8 = AndroidJNISafe_NewString_mD1D954E0EE5A8F135B19EE67E8FF2A4E1A6CA97F(L_7, /*hidden argument*/NULL);
		((L_6)->GetAddressAt(static_cast<il2cpp_array_size_t>(2)))->set_l_8((intptr_t)L_8);
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_9 = V_0;
		NullCheck(L_9);
		bool L_10 = ___isStatic3;
		((L_9)->GetAddressAt(static_cast<il2cpp_array_size_t>(3)))->set_z_0(L_10);
		IL2CPP_RUNTIME_CLASS_INIT(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var);
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_11 = ((AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_StaticFields*)il2cpp_codegen_static_fields_for(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var))->get_s_ReflectionHelperClass_0();
		intptr_t L_12 = GlobalJavaObjectRef_op_Implicit_m1F52DE72C8F8B11E651F8B31879ED5AFD413EDFF(L_11, /*hidden argument*/NULL);
		intptr_t L_13 = ((AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_StaticFields*)il2cpp_codegen_static_fields_for(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var))->get_s_ReflectionHelperGetMethodID_2();
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_14 = V_0;
		intptr_t L_15 = AndroidJNISafe_CallStaticObjectMethod_m11EDE005224D5A6833BFF896906397D24E19D440((intptr_t)L_12, (intptr_t)L_13, L_14, /*hidden argument*/NULL);
		V_1 = (intptr_t)L_15;
		IL2CPP_LEAVE(0x87, FINALLY_0062);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0062;
	}

FINALLY_0062:
	{ // begin finally (depth: 1)
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_16 = V_0;
		NullCheck(L_16);
		intptr_t L_17 = ((L_16)->GetAddressAt(static_cast<il2cpp_array_size_t>(1)))->get_l_8();
		AndroidJNISafe_DeleteLocalRef_m9632EA13BF03AEE43FC7713125962A4D0DFFADC7((intptr_t)L_17, /*hidden argument*/NULL);
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_18 = V_0;
		NullCheck(L_18);
		intptr_t L_19 = ((L_18)->GetAddressAt(static_cast<il2cpp_array_size_t>(2)))->get_l_8();
		AndroidJNISafe_DeleteLocalRef_m9632EA13BF03AEE43FC7713125962A4D0DFFADC7((intptr_t)L_19, /*hidden argument*/NULL);
		IL2CPP_END_FINALLY(98)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(98)
	{
		IL2CPP_JUMP_TBL(0x87, IL_0087)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0087:
	{
		intptr_t L_20 = V_1;
		return (intptr_t)L_20;
	}
}
// System.IntPtr UnityEngine.AndroidReflection::NewProxyInstance(System.IntPtr,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t AndroidReflection_NewProxyInstance_mEE0634E1963302B17FBAED127B581BFE4D228A8C (intptr_t ___delegateHandle0, intptr_t ___interfaze1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidReflection_NewProxyInstance_mEE0634E1963302B17FBAED127B581BFE4D228A8C_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* V_0 = NULL;
	intptr_t V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_0 = (jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*)(jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*)SZArrayNew(jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3_il2cpp_TypeInfo_var, (uint32_t)2);
		V_0 = L_0;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_1 = V_0;
		NullCheck(L_1);
		int64_t L_2 = IntPtr_ToInt64_mDD00D5F4AD380F40D31B60E9C57843CC3C12BD6B((intptr_t*)(&___delegateHandle0), /*hidden argument*/NULL);
		((L_1)->GetAddressAt(static_cast<il2cpp_array_size_t>(0)))->set_j_5(L_2);
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_3 = V_0;
		NullCheck(L_3);
		intptr_t L_4 = ___interfaze1;
		((L_3)->GetAddressAt(static_cast<il2cpp_array_size_t>(1)))->set_l_8((intptr_t)L_4);
		IL2CPP_RUNTIME_CLASS_INIT(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var);
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_5 = ((AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_StaticFields*)il2cpp_codegen_static_fields_for(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var))->get_s_ReflectionHelperClass_0();
		intptr_t L_6 = GlobalJavaObjectRef_op_Implicit_m1F52DE72C8F8B11E651F8B31879ED5AFD413EDFF(L_5, /*hidden argument*/NULL);
		intptr_t L_7 = ((AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_StaticFields*)il2cpp_codegen_static_fields_for(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var))->get_s_ReflectionHelperNewProxyInstance_4();
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_8 = V_0;
		intptr_t L_9 = AndroidJNISafe_CallStaticObjectMethod_m11EDE005224D5A6833BFF896906397D24E19D440((intptr_t)L_6, (intptr_t)L_7, L_8, /*hidden argument*/NULL);
		V_1 = (intptr_t)L_9;
		goto IL_0043;
	}

IL_0043:
	{
		intptr_t L_10 = V_1;
		return (intptr_t)L_10;
	}
}
// System.Void UnityEngine.AndroidReflection::SetNativeExceptionOnProxy(System.IntPtr,System.Exception,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidReflection_SetNativeExceptionOnProxy_m025AFCDD8B6659D45FE3830E8AC154300DA19966 (intptr_t ___proxy0, Exception_t * ___e1, bool ___methodNotFound2, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidReflection_SetNativeExceptionOnProxy_m025AFCDD8B6659D45FE3830E8AC154300DA19966_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* V_0 = NULL;
	intptr_t V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_0 = (jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*)(jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*)SZArrayNew(jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3_il2cpp_TypeInfo_var, (uint32_t)3);
		V_0 = L_0;
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_1 = V_0;
		NullCheck(L_1);
		intptr_t L_2 = ___proxy0;
		((L_1)->GetAddressAt(static_cast<il2cpp_array_size_t>(0)))->set_l_8((intptr_t)L_2);
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_3 = V_0;
		NullCheck(L_3);
		Exception_t * L_4 = ___e1;
		GCHandle_t39FAEE3EA592432C93B574A31DD83B87F1847DE3  L_5 = GCHandle_Alloc_m5BF9DC23B533B904BFEA61136B92916683B46B0F(L_4, /*hidden argument*/NULL);
		intptr_t L_6 = GCHandle_ToIntPtr_m8CF7D07846B0C741B04A2A4E5E9B5D505F4B3CCE(L_5, /*hidden argument*/NULL);
		V_1 = (intptr_t)L_6;
		int64_t L_7 = IntPtr_ToInt64_mDD00D5F4AD380F40D31B60E9C57843CC3C12BD6B((intptr_t*)(&V_1), /*hidden argument*/NULL);
		((L_3)->GetAddressAt(static_cast<il2cpp_array_size_t>(1)))->set_j_5(L_7);
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_8 = V_0;
		NullCheck(L_8);
		bool L_9 = ___methodNotFound2;
		((L_8)->GetAddressAt(static_cast<il2cpp_array_size_t>(2)))->set_z_0(L_9);
		IL2CPP_RUNTIME_CLASS_INIT(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var);
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_10 = ((AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_StaticFields*)il2cpp_codegen_static_fields_for(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var))->get_s_ReflectionHelperClass_0();
		intptr_t L_11 = GlobalJavaObjectRef_op_Implicit_m1F52DE72C8F8B11E651F8B31879ED5AFD413EDFF(L_10, /*hidden argument*/NULL);
		intptr_t L_12 = ((AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_StaticFields*)il2cpp_codegen_static_fields_for(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var))->get_s_ReflectionHelperSetNativeExceptionOnProxy_5();
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_13 = V_0;
		AndroidJNISafe_CallStaticVoidMethod_mC0BC9FA7E2FB69027E1F55E8810C6F619BCD7D59((intptr_t)L_11, (intptr_t)L_12, L_13, /*hidden argument*/NULL);
		return;
	}
}
// System.Void UnityEngine.AndroidReflection::.cctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AndroidReflection__cctor_m328F9C260CA935498229C4D912C6B27618BEE8E6 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (AndroidReflection__cctor_m328F9C260CA935498229C4D912C6B27618BEE8E6_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		intptr_t L_0 = AndroidJNISafe_FindClass_mE58501828AA09ADC26347853AFE6D025845D487C(_stringLiteral1D6BCB22DD39DE1A757738A79C87BE5519B16FDE, /*hidden argument*/NULL);
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_1 = (GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 *)il2cpp_codegen_object_new(GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0_il2cpp_TypeInfo_var);
		GlobalJavaObjectRef__ctor_m5581A68DC5217545E13F48ACF2DAFD9DF30396BC(L_1, (intptr_t)L_0, /*hidden argument*/NULL);
		((AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_StaticFields*)il2cpp_codegen_static_fields_for(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var))->set_s_ReflectionHelperClass_0(L_1);
		intptr_t L_2 = AndroidReflection_GetStaticMethodID_m1D6770C9A0BC1AA47FDA330B92743324C0441B29(_stringLiteral1D6BCB22DD39DE1A757738A79C87BE5519B16FDE, _stringLiteral1C0FE9678C38548F6401871A634364E47FE3612C, _stringLiteralB663A44D6624D0F6014A6C18E4A7FE0F5BED8FB9, /*hidden argument*/NULL);
		((AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_StaticFields*)il2cpp_codegen_static_fields_for(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var))->set_s_ReflectionHelperGetConstructorID_1((intptr_t)L_2);
		intptr_t L_3 = AndroidReflection_GetStaticMethodID_m1D6770C9A0BC1AA47FDA330B92743324C0441B29(_stringLiteral1D6BCB22DD39DE1A757738A79C87BE5519B16FDE, _stringLiteralA4527A64BEC3DBF0AC08CE1BF9ABC796E5E364E9, _stringLiteralEA4172C398D2679B145B45C5FF1544AB767AA341, /*hidden argument*/NULL);
		((AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_StaticFields*)il2cpp_codegen_static_fields_for(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var))->set_s_ReflectionHelperGetMethodID_2((intptr_t)L_3);
		intptr_t L_4 = AndroidReflection_GetStaticMethodID_m1D6770C9A0BC1AA47FDA330B92743324C0441B29(_stringLiteral1D6BCB22DD39DE1A757738A79C87BE5519B16FDE, _stringLiteral9B23229A318A455BC6A6E6317B4E72BC31248A5A, _stringLiteralB305182D3386ABDBE950B08CD45E46CB3C5E3D6F, /*hidden argument*/NULL);
		((AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_StaticFields*)il2cpp_codegen_static_fields_for(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var))->set_s_ReflectionHelperGetFieldID_3((intptr_t)L_4);
		intptr_t L_5 = AndroidReflection_GetStaticMethodID_m1D6770C9A0BC1AA47FDA330B92743324C0441B29(_stringLiteral1D6BCB22DD39DE1A757738A79C87BE5519B16FDE, _stringLiteral760AB7ED1FC73BD5C47398584B149380AB0582EA, _stringLiteralF8576A7F9518C9A6463C6E1B2833EB06AE9664B4, /*hidden argument*/NULL);
		((AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_StaticFields*)il2cpp_codegen_static_fields_for(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var))->set_s_ReflectionHelperNewProxyInstance_4((intptr_t)L_5);
		intptr_t L_6 = AndroidReflection_GetStaticMethodID_m1D6770C9A0BC1AA47FDA330B92743324C0441B29(_stringLiteral1D6BCB22DD39DE1A757738A79C87BE5519B16FDE, _stringLiteralB9922C62B93E7253F36B55E613AA39448D564BB9, _stringLiteral9ACC977EC5E796EA5E374A6E64654E0222D407E2, /*hidden argument*/NULL);
		((AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_StaticFields*)il2cpp_codegen_static_fields_for(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var))->set_s_ReflectionHelperSetNativeExceptionOnProxy_5((intptr_t)L_6);
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
// System.Void UnityEngine.GlobalJavaObjectRef::.ctor(System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GlobalJavaObjectRef__ctor_m5581A68DC5217545E13F48ACF2DAFD9DF30396BC (GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * __this, intptr_t ___jobject0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (GlobalJavaObjectRef__ctor_m5581A68DC5217545E13F48ACF2DAFD9DF30396BC_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * G_B2_0 = NULL;
	GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * G_B1_0 = NULL;
	intptr_t G_B3_0;
	memset((&G_B3_0), 0, sizeof(G_B3_0));
	GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * G_B3_1 = NULL;
	{
		__this->set_m_disposed_0((bool)0);
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		intptr_t L_0 = ___jobject0;
		bool L_1 = IntPtr_op_Equality_mEE8D9FD2DFE312BBAA8B4ED3BF7976B3142A5934((intptr_t)L_0, (intptr_t)(0), /*hidden argument*/NULL);
		G_B1_0 = __this;
		if (!L_1)
		{
			G_B2_0 = __this;
			goto IL_0029;
		}
	}
	{
		G_B3_0 = (0);
		G_B3_1 = G_B1_0;
		goto IL_002f;
	}

IL_0029:
	{
		intptr_t L_2 = ___jobject0;
		intptr_t L_3 = AndroidJNI_NewGlobalRef_m1F7D16F896A4153CC36ADBACFD740D6453E2AB54((intptr_t)L_2, /*hidden argument*/NULL);
		G_B3_0 = L_3;
		G_B3_1 = G_B2_0;
	}

IL_002f:
	{
		NullCheck(G_B3_1);
		G_B3_1->set_m_jobject_1((intptr_t)G_B3_0);
		return;
	}
}
// System.Void UnityEngine.GlobalJavaObjectRef::Finalize()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GlobalJavaObjectRef_Finalize_mAC26B588678FAB49013BFD07E5090E438E016F83 (GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * __this, const RuntimeMethod* method)
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
		GlobalJavaObjectRef_Dispose_mDCFD34D040E7B4ACE886336F3659316D1A45599F(__this, /*hidden argument*/NULL);
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
// System.IntPtr UnityEngine.GlobalJavaObjectRef::op_Implicit(UnityEngine.GlobalJavaObjectRef)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t GlobalJavaObjectRef_op_Implicit_m1F52DE72C8F8B11E651F8B31879ED5AFD413EDFF (GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * ___obj0, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * L_0 = ___obj0;
		NullCheck(L_0);
		intptr_t L_1 = L_0->get_m_jobject_1();
		V_0 = (intptr_t)L_1;
		goto IL_000d;
	}

IL_000d:
	{
		intptr_t L_2 = V_0;
		return (intptr_t)L_2;
	}
}
// System.Void UnityEngine.GlobalJavaObjectRef::Dispose()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GlobalJavaObjectRef_Dispose_mDCFD34D040E7B4ACE886336F3659316D1A45599F (GlobalJavaObjectRef_t2B9FA8DBBC53F0C0E0B57ACDC0FA9967CFB22DD0 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (GlobalJavaObjectRef_Dispose_mDCFD34D040E7B4ACE886336F3659316D1A45599F_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		bool L_0 = __this->get_m_disposed_0();
		if (!L_0)
		{
			goto IL_0011;
		}
	}
	{
		goto IL_003a;
	}

IL_0011:
	{
		__this->set_m_disposed_0((bool)1);
		intptr_t L_1 = __this->get_m_jobject_1();
		bool L_2 = IntPtr_op_Inequality_mB4886A806009EA825EFCC60CD2A7F6EB8E273A61((intptr_t)L_1, (intptr_t)(0), /*hidden argument*/NULL);
		if (!L_2)
		{
			goto IL_003a;
		}
	}
	{
		intptr_t L_3 = __this->get_m_jobject_1();
		AndroidJNISafe_DeleteGlobalRef_mE0C851F30E3481496C72814973B66161C486D8BA((intptr_t)L_3, /*hidden argument*/NULL);
	}

IL_003a:
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
// System.IntPtr UnityEngine._AndroidJNIHelper::CreateJavaProxy(System.IntPtr,UnityEngine.AndroidJavaProxy)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t _AndroidJNIHelper_CreateJavaProxy_m8E6AAE823A5FB6D70B4655FA45203779946321ED (intptr_t ___delegateHandle0, AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D * ___proxy1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_AndroidJNIHelper_CreateJavaProxy_m8E6AAE823A5FB6D70B4655FA45203779946321ED_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		intptr_t L_0 = ___delegateHandle0;
		AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D * L_1 = ___proxy1;
		NullCheck(L_1);
		AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * L_2 = L_1->get_javaInterface_0();
		NullCheck(L_2);
		intptr_t L_3 = AndroidJavaObject_GetRawClass_m28BFE7AD6A4FFCB45929D9D1A0F8D792C3974774(L_2, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var);
		intptr_t L_4 = AndroidReflection_NewProxyInstance_mEE0634E1963302B17FBAED127B581BFE4D228A8C((intptr_t)L_0, (intptr_t)L_3, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_4;
		goto IL_0018;
	}

IL_0018:
	{
		intptr_t L_5 = V_0;
		return (intptr_t)L_5;
	}
}
// System.IntPtr UnityEngine._AndroidJNIHelper::CreateJavaRunnable(UnityEngine.AndroidJavaRunnable)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t _AndroidJNIHelper_CreateJavaRunnable_mC009CB98AF579A1DBECE07EE23A4F20B8E53BDF0 (AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 * ___jrunnable0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_AndroidJNIHelper_CreateJavaRunnable_mC009CB98AF579A1DBECE07EE23A4F20B8E53BDF0_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 * L_0 = ___jrunnable0;
		AndroidJavaRunnableProxy_t3C66FEA8C2A903168F2902788AB8AB29CA923308 * L_1 = (AndroidJavaRunnableProxy_t3C66FEA8C2A903168F2902788AB8AB29CA923308 *)il2cpp_codegen_object_new(AndroidJavaRunnableProxy_t3C66FEA8C2A903168F2902788AB8AB29CA923308_il2cpp_TypeInfo_var);
		AndroidJavaRunnableProxy__ctor_m0D23BFCE5D99EA0AA56A5813B2E91BDDAD72C738(L_1, L_0, /*hidden argument*/NULL);
		intptr_t L_2 = AndroidJNIHelper_CreateJavaProxy_m29A8BD91809FF21642EA1319E5F097979EE8FA28(L_1, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_2;
		goto IL_0012;
	}

IL_0012:
	{
		intptr_t L_3 = V_0;
		return (intptr_t)L_3;
	}
}
// System.IntPtr UnityEngine._AndroidJNIHelper::InvokeJavaProxyMethod(UnityEngine.AndroidJavaProxy,System.IntPtr,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t _AndroidJNIHelper_InvokeJavaProxyMethod_mF3275AFDFED43C42616A997FC582F1F90888AB87 (AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D * ___proxy0, intptr_t ___jmethodName1, intptr_t ___jargs2, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_AndroidJNIHelper_InvokeJavaProxyMethod_mF3275AFDFED43C42616A997FC582F1F90888AB87_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379* V_1 = NULL;
	int32_t V_2 = 0;
	intptr_t V_3;
	memset((&V_3), 0, sizeof(V_3));
	AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * V_4 = NULL;
	intptr_t V_5;
	memset((&V_5), 0, sizeof(V_5));
	Exception_t * V_6 = NULL;
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 3);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	int32_t G_B6_0 = 0;
	AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379* G_B6_1 = NULL;
	int32_t G_B5_0 = 0;
	AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379* G_B5_1 = NULL;
	AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * G_B7_0 = NULL;
	int32_t G_B7_1 = 0;
	AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379* G_B7_2 = NULL;
	{
	}

IL_0001:
	try
	{ // begin try (depth: 1)
		{
			V_0 = 0;
			intptr_t L_0 = ___jargs2;
			bool L_1 = IntPtr_op_Inequality_mB4886A806009EA825EFCC60CD2A7F6EB8E273A61((intptr_t)L_0, (intptr_t)(0), /*hidden argument*/NULL);
			if (!L_1)
			{
				goto IL_001d;
			}
		}

IL_0014:
		{
			intptr_t L_2 = ___jargs2;
			int32_t L_3 = AndroidJNISafe_GetArrayLength_m11614663772194842C0D75FB8C6FBDB92F8DEE05((intptr_t)L_2, /*hidden argument*/NULL);
			V_0 = L_3;
		}

IL_001d:
		{
			int32_t L_4 = V_0;
			AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379* L_5 = (AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379*)(AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379*)SZArrayNew(AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379_il2cpp_TypeInfo_var, (uint32_t)L_4);
			V_1 = L_5;
			V_2 = 0;
			goto IL_0058;
		}

IL_002b:
		{
			intptr_t L_6 = ___jargs2;
			int32_t L_7 = V_2;
			intptr_t L_8 = AndroidJNISafe_GetObjectArrayElement_mA87BFEFBCE1C7D1B5B817CCCB5D4B7F009FD37BD((intptr_t)L_6, L_7, /*hidden argument*/NULL);
			V_3 = (intptr_t)L_8;
			AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379* L_9 = V_1;
			int32_t L_10 = V_2;
			intptr_t L_11 = V_3;
			bool L_12 = IntPtr_op_Inequality_mB4886A806009EA825EFCC60CD2A7F6EB8E273A61((intptr_t)L_11, (intptr_t)(0), /*hidden argument*/NULL);
			G_B5_0 = L_10;
			G_B5_1 = L_9;
			if (!L_12)
			{
				G_B6_0 = L_10;
				G_B6_1 = L_9;
				goto IL_0051;
			}
		}

IL_0046:
		{
			intptr_t L_13 = V_3;
			AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_14 = (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)il2cpp_codegen_object_new(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var);
			AndroidJavaObject__ctor_m22E1E2E5D9F3DA31FF7DFB1339AD3BB0C3813E80(L_14, (intptr_t)L_13, /*hidden argument*/NULL);
			G_B7_0 = L_14;
			G_B7_1 = G_B5_0;
			G_B7_2 = G_B5_1;
			goto IL_0052;
		}

IL_0051:
		{
			G_B7_0 = ((AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)(NULL));
			G_B7_1 = G_B6_0;
			G_B7_2 = G_B6_1;
		}

IL_0052:
		{
			NullCheck(G_B7_2);
			ArrayElementTypeCheck (G_B7_2, G_B7_0);
			(G_B7_2)->SetAt(static_cast<il2cpp_array_size_t>(G_B7_1), (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)G_B7_0);
			int32_t L_15 = V_2;
			V_2 = ((int32_t)il2cpp_codegen_add((int32_t)L_15, (int32_t)1));
		}

IL_0058:
		{
			int32_t L_16 = V_2;
			int32_t L_17 = V_0;
			if ((((int32_t)L_16) < ((int32_t)L_17)))
			{
				goto IL_002b;
			}
		}

IL_005f:
		{
			AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D * L_18 = ___proxy0;
			intptr_t L_19 = ___jmethodName1;
			String_t* L_20 = AndroidJNI_GetStringChars_m1C44DAAF9B7AA8E9586F1CD236E825B07741A268((intptr_t)L_19, /*hidden argument*/NULL);
			AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379* L_21 = V_1;
			NullCheck(L_18);
			AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_22 = VirtFuncInvoker2< AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *, String_t*, AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379* >::Invoke(5 /* UnityEngine.AndroidJavaObject UnityEngine.AndroidJavaProxy::Invoke(System.String,UnityEngine.AndroidJavaObject[]) */, L_18, L_20, L_21);
			V_4 = L_22;
		}

IL_006e:
		try
		{ // begin try (depth: 2)
			{
				AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_23 = V_4;
				if (L_23)
				{
					goto IL_0082;
				}
			}

IL_0076:
			{
				V_5 = (intptr_t)(0);
				IL2CPP_LEAVE(0xC1, FINALLY_0095);
			}

IL_0082:
			{
				AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_24 = V_4;
				NullCheck(L_24);
				intptr_t L_25 = AndroidJavaObject_GetRawObject_mCEB7EEC51D62A3E4F0D6F62C08CBEF008B556F3D(L_24, /*hidden argument*/NULL);
				intptr_t L_26 = AndroidJNI_NewLocalRef_m22674FDA13C73173E0ECB3F59DE15CBDAD4CD039((intptr_t)L_25, /*hidden argument*/NULL);
				V_5 = (intptr_t)L_26;
				IL2CPP_LEAVE(0xC1, FINALLY_0095);
			}
		} // end try (depth: 2)
		catch(Il2CppExceptionWrapper& e)
		{
			__last_unhandled_exception = (Exception_t *)e.ex;
			goto FINALLY_0095;
		}

FINALLY_0095:
		{ // begin finally (depth: 2)
			{
				AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_27 = V_4;
				if (!L_27)
				{
					goto IL_00a3;
				}
			}

IL_009c:
			{
				AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_28 = V_4;
				NullCheck(L_28);
				InterfaceActionInvoker0::Invoke(0 /* System.Void System.IDisposable::Dispose() */, IDisposable_t7218B22548186B208D65EA5B7870503810A2D15A_il2cpp_TypeInfo_var, L_28);
			}

IL_00a3:
			{
				IL2CPP_END_FINALLY(149)
			}
		} // end finally (depth: 2)
		IL2CPP_CLEANUP(149)
		{
			IL2CPP_JUMP_TBL(0xC1, IL_00c1)
			IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
		}
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__exception_local = (Exception_t *)e.ex;
		if(il2cpp_codegen_class_is_assignable_from (Exception_t_il2cpp_TypeInfo_var, il2cpp_codegen_object_class(e.ex)))
			goto CATCH_00a4;
		throw e;
	}

CATCH_00a4:
	{ // begin catch(System.Exception)
		V_6 = ((Exception_t *)__exception_local);
		AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D * L_29 = ___proxy0;
		NullCheck(L_29);
		intptr_t L_30 = AndroidJavaProxy_GetRawProxy_mFE7D48E72D4744E260D3ACE6D777D072002BEA6C(L_29, /*hidden argument*/NULL);
		Exception_t * L_31 = V_6;
		IL2CPP_RUNTIME_CLASS_INIT(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var);
		AndroidReflection_SetNativeExceptionOnProxy_m025AFCDD8B6659D45FE3830E8AC154300DA19966((intptr_t)L_30, L_31, (bool)0, /*hidden argument*/NULL);
		V_5 = (intptr_t)(0);
		goto IL_00c1;
	} // end catch (depth: 1)

IL_00c1:
	{
		intptr_t L_32 = V_5;
		return (intptr_t)L_32;
	}
}
// UnityEngine.jvalue[] UnityEngine._AndroidJNIHelper::CreateJNIArgArray(System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* _AndroidJNIHelper_CreateJNIArgArray_m9605B7C73D18B6A11264A61E33888374E1F283A9 (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_AndroidJNIHelper_CreateJNIArgArray_m9605B7C73D18B6A11264A61E33888374E1F283A9_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* V_0 = NULL;
	int32_t V_1 = 0;
	RuntimeObject * V_2 = NULL;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* V_3 = NULL;
	int32_t V_4 = 0;
	jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* V_5 = NULL;
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_0 = ___args0;
		NullCheck((RuntimeArray *)(RuntimeArray *)L_0);
		int32_t L_1 = Array_GetLength_m318900B10C3A93A30ABDC67DE161C8F6ABA4D359((RuntimeArray *)(RuntimeArray *)L_0, 0, /*hidden argument*/NULL);
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_2 = (jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*)(jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3*)SZArrayNew(jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3_il2cpp_TypeInfo_var, (uint32_t)L_1);
		V_0 = L_2;
		V_1 = 0;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_3 = ___args0;
		V_3 = L_3;
		V_4 = 0;
		goto IL_02ab;
	}

IL_001b:
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_4 = V_3;
		int32_t L_5 = V_4;
		NullCheck(L_4);
		int32_t L_6 = L_5;
		RuntimeObject * L_7 = (L_4)->GetAt(static_cast<il2cpp_array_size_t>(L_6));
		V_2 = L_7;
		RuntimeObject * L_8 = V_2;
		if (L_8)
		{
			goto IL_003d;
		}
	}
	{
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_9 = V_0;
		int32_t L_10 = V_1;
		NullCheck(L_9);
		((L_9)->GetAddressAt(static_cast<il2cpp_array_size_t>(L_10)))->set_l_8((intptr_t)(0));
		goto IL_02a0;
	}

IL_003d:
	{
		RuntimeObject * L_11 = V_2;
		NullCheck(L_11);
		Type_t * L_12 = Object_GetType_m2E0B62414ECCAA3094B703790CE88CBB2F83EA60(L_11, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var);
		bool L_13 = AndroidReflection_IsPrimitive_m4C75B1AAEDD3FA0F73AFBC83CB374D3D8A9A3749(L_12, /*hidden argument*/NULL);
		if (!L_13)
		{
			goto IL_018e;
		}
	}
	{
		RuntimeObject * L_14 = V_2;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_14, Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var)))
		{
			goto IL_0070;
		}
	}
	{
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_15 = V_0;
		int32_t L_16 = V_1;
		NullCheck(L_15);
		RuntimeObject * L_17 = V_2;
		((L_15)->GetAddressAt(static_cast<il2cpp_array_size_t>(L_16)))->set_i_4(((*(int32_t*)((int32_t*)UnBox(L_17, Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var)))));
		goto IL_0188;
	}

IL_0070:
	{
		RuntimeObject * L_18 = V_2;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_18, Boolean_tB53F6830F670160873277339AA58F15CAED4399C_il2cpp_TypeInfo_var)))
		{
			goto IL_0092;
		}
	}
	{
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_19 = V_0;
		int32_t L_20 = V_1;
		NullCheck(L_19);
		RuntimeObject * L_21 = V_2;
		((L_19)->GetAddressAt(static_cast<il2cpp_array_size_t>(L_20)))->set_z_0(((*(bool*)((bool*)UnBox(L_21, Boolean_tB53F6830F670160873277339AA58F15CAED4399C_il2cpp_TypeInfo_var)))));
		goto IL_0188;
	}

IL_0092:
	{
		RuntimeObject * L_22 = V_2;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_22, Byte_tF87C579059BD4633E6840EBBBEEF899C6E33EF07_il2cpp_TypeInfo_var)))
		{
			goto IL_00c1;
		}
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_LogWarning_m37338644DC81F640CCDFEAE35A223F0E965F0568(_stringLiteralC5299E0BD811F82870EFBBB00341CCDF263E242E, /*hidden argument*/NULL);
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_23 = V_0;
		int32_t L_24 = V_1;
		NullCheck(L_23);
		RuntimeObject * L_25 = V_2;
		((L_23)->GetAddressAt(static_cast<il2cpp_array_size_t>(L_24)))->set_b_1((((int8_t)((int8_t)((*(uint8_t*)((uint8_t*)UnBox(L_25, Byte_tF87C579059BD4633E6840EBBBEEF899C6E33EF07_il2cpp_TypeInfo_var))))))));
		goto IL_0188;
	}

IL_00c1:
	{
		RuntimeObject * L_26 = V_2;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_26, SByte_t9070AEA2966184235653CB9B4D33B149CDA831DF_il2cpp_TypeInfo_var)))
		{
			goto IL_00e3;
		}
	}
	{
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_27 = V_0;
		int32_t L_28 = V_1;
		NullCheck(L_27);
		RuntimeObject * L_29 = V_2;
		((L_27)->GetAddressAt(static_cast<il2cpp_array_size_t>(L_28)))->set_b_1(((*(int8_t*)((int8_t*)UnBox(L_29, SByte_t9070AEA2966184235653CB9B4D33B149CDA831DF_il2cpp_TypeInfo_var)))));
		goto IL_0188;
	}

IL_00e3:
	{
		RuntimeObject * L_30 = V_2;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_30, Int16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D_il2cpp_TypeInfo_var)))
		{
			goto IL_0105;
		}
	}
	{
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_31 = V_0;
		int32_t L_32 = V_1;
		NullCheck(L_31);
		RuntimeObject * L_33 = V_2;
		((L_31)->GetAddressAt(static_cast<il2cpp_array_size_t>(L_32)))->set_s_3(((*(int16_t*)((int16_t*)UnBox(L_33, Int16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D_il2cpp_TypeInfo_var)))));
		goto IL_0188;
	}

IL_0105:
	{
		RuntimeObject * L_34 = V_2;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_34, Int64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436_il2cpp_TypeInfo_var)))
		{
			goto IL_0127;
		}
	}
	{
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_35 = V_0;
		int32_t L_36 = V_1;
		NullCheck(L_35);
		RuntimeObject * L_37 = V_2;
		((L_35)->GetAddressAt(static_cast<il2cpp_array_size_t>(L_36)))->set_j_5(((*(int64_t*)((int64_t*)UnBox(L_37, Int64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436_il2cpp_TypeInfo_var)))));
		goto IL_0188;
	}

IL_0127:
	{
		RuntimeObject * L_38 = V_2;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_38, Single_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1_il2cpp_TypeInfo_var)))
		{
			goto IL_0149;
		}
	}
	{
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_39 = V_0;
		int32_t L_40 = V_1;
		NullCheck(L_39);
		RuntimeObject * L_41 = V_2;
		((L_39)->GetAddressAt(static_cast<il2cpp_array_size_t>(L_40)))->set_f_6(((*(float*)((float*)UnBox(L_41, Single_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1_il2cpp_TypeInfo_var)))));
		goto IL_0188;
	}

IL_0149:
	{
		RuntimeObject * L_42 = V_2;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_42, Double_t358B8F23BDC52A5DD700E727E204F9F7CDE12409_il2cpp_TypeInfo_var)))
		{
			goto IL_016b;
		}
	}
	{
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_43 = V_0;
		int32_t L_44 = V_1;
		NullCheck(L_43);
		RuntimeObject * L_45 = V_2;
		((L_43)->GetAddressAt(static_cast<il2cpp_array_size_t>(L_44)))->set_d_7(((*(double*)((double*)UnBox(L_45, Double_t358B8F23BDC52A5DD700E727E204F9F7CDE12409_il2cpp_TypeInfo_var)))));
		goto IL_0188;
	}

IL_016b:
	{
		RuntimeObject * L_46 = V_2;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_46, Char_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9_il2cpp_TypeInfo_var)))
		{
			goto IL_0188;
		}
	}
	{
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_47 = V_0;
		int32_t L_48 = V_1;
		NullCheck(L_47);
		RuntimeObject * L_49 = V_2;
		((L_47)->GetAddressAt(static_cast<il2cpp_array_size_t>(L_48)))->set_c_2(((*(Il2CppChar*)((Il2CppChar*)UnBox(L_49, Char_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9_il2cpp_TypeInfo_var)))));
	}

IL_0188:
	{
		goto IL_02a0;
	}

IL_018e:
	{
		RuntimeObject * L_50 = V_2;
		if (!((String_t*)IsInstSealed((RuntimeObject*)L_50, String_t_il2cpp_TypeInfo_var)))
		{
			goto IL_01b7;
		}
	}
	{
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_51 = V_0;
		int32_t L_52 = V_1;
		NullCheck(L_51);
		RuntimeObject * L_53 = V_2;
		intptr_t L_54 = AndroidJNISafe_NewString_mD1D954E0EE5A8F135B19EE67E8FF2A4E1A6CA97F(((String_t*)CastclassSealed((RuntimeObject*)L_53, String_t_il2cpp_TypeInfo_var)), /*hidden argument*/NULL);
		((L_51)->GetAddressAt(static_cast<il2cpp_array_size_t>(L_52)))->set_l_8((intptr_t)L_54);
		goto IL_02a0;
	}

IL_01b7:
	{
		RuntimeObject * L_55 = V_2;
		if (!((AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE *)IsInstClass((RuntimeObject*)L_55, AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE_il2cpp_TypeInfo_var)))
		{
			goto IL_01e0;
		}
	}
	{
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_56 = V_0;
		int32_t L_57 = V_1;
		NullCheck(L_56);
		RuntimeObject * L_58 = V_2;
		NullCheck(((AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE *)CastclassClass((RuntimeObject*)L_58, AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE_il2cpp_TypeInfo_var)));
		intptr_t L_59 = AndroidJavaObject_GetRawClass_m28BFE7AD6A4FFCB45929D9D1A0F8D792C3974774(((AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE *)CastclassClass((RuntimeObject*)L_58, AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE_il2cpp_TypeInfo_var)), /*hidden argument*/NULL);
		((L_56)->GetAddressAt(static_cast<il2cpp_array_size_t>(L_57)))->set_l_8((intptr_t)L_59);
		goto IL_02a0;
	}

IL_01e0:
	{
		RuntimeObject * L_60 = V_2;
		if (!((AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)IsInstClass((RuntimeObject*)L_60, AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var)))
		{
			goto IL_0209;
		}
	}
	{
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_61 = V_0;
		int32_t L_62 = V_1;
		NullCheck(L_61);
		RuntimeObject * L_63 = V_2;
		NullCheck(((AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)CastclassClass((RuntimeObject*)L_63, AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var)));
		intptr_t L_64 = AndroidJavaObject_GetRawObject_mCEB7EEC51D62A3E4F0D6F62C08CBEF008B556F3D(((AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)CastclassClass((RuntimeObject*)L_63, AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var)), /*hidden argument*/NULL);
		((L_61)->GetAddressAt(static_cast<il2cpp_array_size_t>(L_62)))->set_l_8((intptr_t)L_64);
		goto IL_02a0;
	}

IL_0209:
	{
		RuntimeObject * L_65 = V_2;
		if (!((RuntimeArray *)IsInstClass((RuntimeObject*)L_65, RuntimeArray_il2cpp_TypeInfo_var)))
		{
			goto IL_0232;
		}
	}
	{
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_66 = V_0;
		int32_t L_67 = V_1;
		NullCheck(L_66);
		RuntimeObject * L_68 = V_2;
		intptr_t L_69 = _AndroidJNIHelper_ConvertToJNIArray_mBF20C1B6716BA00CA9C3825EA446B291E6D8EB20(((RuntimeArray *)CastclassClass((RuntimeObject*)L_68, RuntimeArray_il2cpp_TypeInfo_var)), /*hidden argument*/NULL);
		((L_66)->GetAddressAt(static_cast<il2cpp_array_size_t>(L_67)))->set_l_8((intptr_t)L_69);
		goto IL_02a0;
	}

IL_0232:
	{
		RuntimeObject * L_70 = V_2;
		if (!((AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D *)IsInstClass((RuntimeObject*)L_70, AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D_il2cpp_TypeInfo_var)))
		{
			goto IL_025b;
		}
	}
	{
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_71 = V_0;
		int32_t L_72 = V_1;
		NullCheck(L_71);
		RuntimeObject * L_73 = V_2;
		NullCheck(((AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D *)CastclassClass((RuntimeObject*)L_73, AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D_il2cpp_TypeInfo_var)));
		intptr_t L_74 = AndroidJavaProxy_GetRawProxy_mFE7D48E72D4744E260D3ACE6D777D072002BEA6C(((AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D *)CastclassClass((RuntimeObject*)L_73, AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D_il2cpp_TypeInfo_var)), /*hidden argument*/NULL);
		((L_71)->GetAddressAt(static_cast<il2cpp_array_size_t>(L_72)))->set_l_8((intptr_t)L_74);
		goto IL_02a0;
	}

IL_025b:
	{
		RuntimeObject * L_75 = V_2;
		if (!((AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 *)IsInstSealed((RuntimeObject*)L_75, AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02_il2cpp_TypeInfo_var)))
		{
			goto IL_0284;
		}
	}
	{
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_76 = V_0;
		int32_t L_77 = V_1;
		NullCheck(L_76);
		RuntimeObject * L_78 = V_2;
		intptr_t L_79 = AndroidJNIHelper_CreateJavaRunnable_mA6C7A0E1BEF771970126D0FB21FF6E95CF569ED8(((AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 *)CastclassSealed((RuntimeObject*)L_78, AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02_il2cpp_TypeInfo_var)), /*hidden argument*/NULL);
		((L_76)->GetAddressAt(static_cast<il2cpp_array_size_t>(L_77)))->set_l_8((intptr_t)L_79);
		goto IL_02a0;
	}

IL_0284:
	{
		RuntimeObject * L_80 = V_2;
		NullCheck(L_80);
		Type_t * L_81 = Object_GetType_m2E0B62414ECCAA3094B703790CE88CBB2F83EA60(L_80, /*hidden argument*/NULL);
		String_t* L_82 = String_Concat_m2E1F71C491D2429CC80A28745488FEA947BB7AAC(_stringLiteral94A9D9512BA3D2F295C65A0B3119715C79E6CB75, L_81, _stringLiteralBB589D0621E5472F470FA3425A234C74B1E202E8, /*hidden argument*/NULL);
		Exception_t * L_83 = (Exception_t *)il2cpp_codegen_object_new(Exception_t_il2cpp_TypeInfo_var);
		Exception__ctor_m89BADFF36C3B170013878726E07729D51AA9FBE0(L_83, L_82, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_83, NULL, _AndroidJNIHelper_CreateJNIArgArray_m9605B7C73D18B6A11264A61E33888374E1F283A9_RuntimeMethod_var);
	}

IL_02a0:
	{
		int32_t L_84 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add((int32_t)L_84, (int32_t)1));
		int32_t L_85 = V_4;
		V_4 = ((int32_t)il2cpp_codegen_add((int32_t)L_85, (int32_t)1));
	}

IL_02ab:
	{
		int32_t L_86 = V_4;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_87 = V_3;
		NullCheck(L_87);
		if ((((int32_t)L_86) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_87)->max_length)))))))
		{
			goto IL_001b;
		}
	}
	{
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_88 = V_0;
		V_5 = L_88;
		goto IL_02bd;
	}

IL_02bd:
	{
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_89 = V_5;
		return L_89;
	}
}
// System.Object UnityEngine._AndroidJNIHelper::UnboxArray(UnityEngine.AndroidJavaObject)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * _AndroidJNIHelper_UnboxArray_m57E035906F4D79FCAC155162AC491BB7B575956D (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_AndroidJNIHelper_UnboxArray_m57E035906F4D79FCAC155162AC491BB7B575956D_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject * V_0 = NULL;
	AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * V_1 = NULL;
	AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * V_2 = NULL;
	AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * V_3 = NULL;
	String_t* V_4 = NULL;
	int32_t V_5 = 0;
	RuntimeArray * V_6 = NULL;
	int32_t V_7 = 0;
	{
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_0 = ___obj0;
		if (L_0)
		{
			goto IL_000e;
		}
	}
	{
		V_0 = NULL;
		goto IL_0226;
	}

IL_000e:
	{
		AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * L_1 = (AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE *)il2cpp_codegen_object_new(AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE_il2cpp_TypeInfo_var);
		AndroidJavaClass__ctor_mAE416E812DB3911279C0FE87A7760247CE1BBFA8(L_1, _stringLiteral6226D4F452A659360FAAC6A6266D73ABC5BFC1FC, /*hidden argument*/NULL);
		V_1 = L_1;
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_2 = ___obj0;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_3 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)0);
		NullCheck(L_2);
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_4 = AndroidJavaObject_Call_TisAndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_m775AB90594C5F27D6099ED61119EF3608FD1001D(L_2, _stringLiteral4F05CBFCA4DFE76B99B142F609CDCF00D44FA247, L_3, /*hidden argument*/AndroidJavaObject_Call_TisAndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_m775AB90594C5F27D6099ED61119EF3608FD1001D_RuntimeMethod_var);
		V_2 = L_4;
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_5 = V_2;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_6 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)0);
		NullCheck(L_5);
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_7 = AndroidJavaObject_Call_TisAndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_m775AB90594C5F27D6099ED61119EF3608FD1001D(L_5, _stringLiteral5ABBEC2FB4C72453E6720E8AA22C1978B547A438, L_6, /*hidden argument*/AndroidJavaObject_Call_TisAndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_m775AB90594C5F27D6099ED61119EF3608FD1001D_RuntimeMethod_var);
		V_3 = L_7;
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_8 = V_3;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_9 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)0);
		NullCheck(L_8);
		String_t* L_10 = AndroidJavaObject_Call_TisString_t_m5EAE53C9E2A8893FD8FEA710378D22C162A0FDEA(L_8, _stringLiteralFA98C1FD2CA6FC89B5ED010FD16AA461F50AFB3E, L_9, /*hidden argument*/AndroidJavaObject_Call_TisString_t_m5EAE53C9E2A8893FD8FEA710378D22C162A0FDEA_RuntimeMethod_var);
		V_4 = L_10;
		AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * L_11 = V_1;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_12 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)1);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_13 = L_12;
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_14 = ___obj0;
		NullCheck(L_13);
		ArrayElementTypeCheck (L_13, L_14);
		(L_13)->SetAt(static_cast<il2cpp_array_size_t>(0), (RuntimeObject *)L_14);
		NullCheck(L_11);
		int32_t L_15 = AndroidJavaObject_CallStatic_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m90D39A3F3725F8BD3F782614FA0101D563DA9CCF(L_11, _stringLiteralAA3B42B3BA69D14FA1DA94B7DD8016010E8F6E0C, L_13, /*hidden argument*/AndroidJavaObject_CallStatic_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_m90D39A3F3725F8BD3F782614FA0101D563DA9CCF_RuntimeMethod_var);
		V_5 = L_15;
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_16 = V_3;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_17 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)0);
		NullCheck(L_16);
		bool L_18 = AndroidJavaObject_Call_TisBoolean_tB53F6830F670160873277339AA58F15CAED4399C_m57EE1ACB271D15DD0E2DDD6B28805C31799A0976(L_16, _stringLiteralD53437218F50447640E8F502D360C117BA0C456F, L_17, /*hidden argument*/AndroidJavaObject_Call_TisBoolean_tB53F6830F670160873277339AA58F15CAED4399C_m57EE1ACB271D15DD0E2DDD6B28805C31799A0976_RuntimeMethod_var);
		if (!L_18)
		{
			goto IL_0193;
		}
	}
	{
		String_t* L_19 = V_4;
		bool L_20 = String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE(_stringLiteral46F8AB7C0CFF9DF7CD124852E26022A6BF89E315, L_19, /*hidden argument*/NULL);
		if (!L_20)
		{
			goto IL_009d;
		}
	}
	{
		int32_t L_21 = V_5;
		Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* L_22 = (Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83*)(Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83*)SZArrayNew(Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83_il2cpp_TypeInfo_var, (uint32_t)L_21);
		V_6 = (RuntimeArray *)L_22;
		goto IL_018d;
	}

IL_009d:
	{
		String_t* L_23 = V_4;
		bool L_24 = String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE(_stringLiteral48647474B89FA8F56ED6BDA0F8148A17B51B97BD, L_23, /*hidden argument*/NULL);
		if (!L_24)
		{
			goto IL_00bc;
		}
	}
	{
		int32_t L_25 = V_5;
		BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040* L_26 = (BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040*)(BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040*)SZArrayNew(BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040_il2cpp_TypeInfo_var, (uint32_t)L_25);
		V_6 = (RuntimeArray *)L_26;
		goto IL_018d;
	}

IL_00bc:
	{
		String_t* L_27 = V_4;
		bool L_28 = String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE(_stringLiteral8CF1783FA99F62CA581F6FE8F3CD66B0F9AB9FC3, L_27, /*hidden argument*/NULL);
		if (!L_28)
		{
			goto IL_00db;
		}
	}
	{
		int32_t L_29 = V_5;
		SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889* L_30 = (SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889*)(SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889*)SZArrayNew(SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889_il2cpp_TypeInfo_var, (uint32_t)L_29);
		V_6 = (RuntimeArray *)L_30;
		goto IL_018d;
	}

IL_00db:
	{
		String_t* L_31 = V_4;
		bool L_32 = String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE(_stringLiteralA0F4EA7D91495DF92BBAC2E2149DFB850FE81396, L_31, /*hidden argument*/NULL);
		if (!L_32)
		{
			goto IL_00fa;
		}
	}
	{
		int32_t L_33 = V_5;
		Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28* L_34 = (Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28*)(Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28*)SZArrayNew(Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28_il2cpp_TypeInfo_var, (uint32_t)L_33);
		V_6 = (RuntimeArray *)L_34;
		goto IL_018d;
	}

IL_00fa:
	{
		String_t* L_35 = V_4;
		bool L_36 = String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE(_stringLiteralBD3027FA569EA15CA76D84DB21C67E2D514C1A5A, L_35, /*hidden argument*/NULL);
		if (!L_36)
		{
			goto IL_0119;
		}
	}
	{
		int32_t L_37 = V_5;
		Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F* L_38 = (Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F*)(Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F*)SZArrayNew(Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F_il2cpp_TypeInfo_var, (uint32_t)L_37);
		V_6 = (RuntimeArray *)L_38;
		goto IL_018d;
	}

IL_0119:
	{
		String_t* L_39 = V_4;
		bool L_40 = String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE(_stringLiteral685E80366130387CB75C055248326976D16FDF8D, L_39, /*hidden argument*/NULL);
		if (!L_40)
		{
			goto IL_0138;
		}
	}
	{
		int32_t L_41 = V_5;
		SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5* L_42 = (SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*)(SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*)SZArrayNew(SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5_il2cpp_TypeInfo_var, (uint32_t)L_41);
		V_6 = (RuntimeArray *)L_42;
		goto IL_018d;
	}

IL_0138:
	{
		String_t* L_43 = V_4;
		bool L_44 = String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE(_stringLiteralBDB36BB22DEB169275B3094BA9005A29EEDDD195, L_43, /*hidden argument*/NULL);
		if (!L_44)
		{
			goto IL_0157;
		}
	}
	{
		int32_t L_45 = V_5;
		DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D* L_46 = (DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D*)(DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D*)SZArrayNew(DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D_il2cpp_TypeInfo_var, (uint32_t)L_45);
		V_6 = (RuntimeArray *)L_46;
		goto IL_018d;
	}

IL_0157:
	{
		String_t* L_47 = V_4;
		bool L_48 = String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE(_stringLiteral71FAFC4E2FC1E47E234762A96B80512B6B5534C2, L_47, /*hidden argument*/NULL);
		if (!L_48)
		{
			goto IL_0176;
		}
	}
	{
		int32_t L_49 = V_5;
		CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2* L_50 = (CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2*)(CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2*)SZArrayNew(CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2_il2cpp_TypeInfo_var, (uint32_t)L_49);
		V_6 = (RuntimeArray *)L_50;
		goto IL_018d;
	}

IL_0176:
	{
		String_t* L_51 = V_4;
		String_t* L_52 = String_Concat_mF4626905368D6558695A823466A1AF65EADB9923(_stringLiteral94A9D9512BA3D2F295C65A0B3119715C79E6CB75, L_51, _stringLiteralBB589D0621E5472F470FA3425A234C74B1E202E8, /*hidden argument*/NULL);
		Exception_t * L_53 = (Exception_t *)il2cpp_codegen_object_new(Exception_t_il2cpp_TypeInfo_var);
		Exception__ctor_m89BADFF36C3B170013878726E07729D51AA9FBE0(L_53, L_52, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_53, NULL, _AndroidJNIHelper_UnboxArray_m57E035906F4D79FCAC155162AC491BB7B575956D_RuntimeMethod_var);
	}

IL_018d:
	{
		goto IL_01da;
	}

IL_0193:
	{
		String_t* L_54 = V_4;
		bool L_55 = String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE(_stringLiteral169775A78ADEE2D403BC1F88A1C1760F11C0304D, L_54, /*hidden argument*/NULL);
		if (!L_55)
		{
			goto IL_01b2;
		}
	}
	{
		int32_t L_56 = V_5;
		StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* L_57 = (StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E*)(StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E*)SZArrayNew(StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E_il2cpp_TypeInfo_var, (uint32_t)L_56);
		V_6 = (RuntimeArray *)L_57;
		goto IL_01da;
	}

IL_01b2:
	{
		String_t* L_58 = V_4;
		bool L_59 = String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE(_stringLiteral328CD2BEF0C16A2D306B28CD73848671CCC42AC2, L_58, /*hidden argument*/NULL);
		if (!L_59)
		{
			goto IL_01d1;
		}
	}
	{
		int32_t L_60 = V_5;
		AndroidJavaClassU5BU5D_t834F2CD8A8D5B7F304A36C401A57C24A723690C7* L_61 = (AndroidJavaClassU5BU5D_t834F2CD8A8D5B7F304A36C401A57C24A723690C7*)(AndroidJavaClassU5BU5D_t834F2CD8A8D5B7F304A36C401A57C24A723690C7*)SZArrayNew(AndroidJavaClassU5BU5D_t834F2CD8A8D5B7F304A36C401A57C24A723690C7_il2cpp_TypeInfo_var, (uint32_t)L_60);
		V_6 = (RuntimeArray *)L_61;
		goto IL_01da;
	}

IL_01d1:
	{
		int32_t L_62 = V_5;
		AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379* L_63 = (AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379*)(AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379*)SZArrayNew(AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379_il2cpp_TypeInfo_var, (uint32_t)L_62);
		V_6 = (RuntimeArray *)L_63;
	}

IL_01da:
	{
		V_7 = 0;
		goto IL_0215;
	}

IL_01e2:
	{
		RuntimeArray * L_64 = V_6;
		AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * L_65 = V_1;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_66 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)2);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_67 = L_66;
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_68 = ___obj0;
		NullCheck(L_67);
		ArrayElementTypeCheck (L_67, L_68);
		(L_67)->SetAt(static_cast<il2cpp_array_size_t>(0), (RuntimeObject *)L_68);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_69 = L_67;
		int32_t L_70 = V_7;
		int32_t L_71 = L_70;
		RuntimeObject * L_72 = Box(Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var, &L_71);
		NullCheck(L_69);
		ArrayElementTypeCheck (L_69, L_72);
		(L_69)->SetAt(static_cast<il2cpp_array_size_t>(1), (RuntimeObject *)L_72);
		NullCheck(L_65);
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_73 = AndroidJavaObject_CallStatic_TisAndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_m6CAE75FB51C5A02521C239A7232735573C51EAE7(L_65, _stringLiteral783923E57BA5E8F1044632C31FD806EE24814BB5, L_69, /*hidden argument*/AndroidJavaObject_CallStatic_TisAndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_m6CAE75FB51C5A02521C239A7232735573C51EAE7_RuntimeMethod_var);
		RuntimeObject * L_74 = _AndroidJNIHelper_Unbox_m813AFB8DE2C2568B011C81ED3AC4D013F1E5B67E(L_73, /*hidden argument*/NULL);
		int32_t L_75 = V_7;
		NullCheck(L_64);
		Array_SetValue_m3C6811CE9C45D1E461404B5D2FBD4EC1A054FDCA(L_64, L_74, L_75, /*hidden argument*/NULL);
		int32_t L_76 = V_7;
		V_7 = ((int32_t)il2cpp_codegen_add((int32_t)L_76, (int32_t)1));
	}

IL_0215:
	{
		int32_t L_77 = V_7;
		int32_t L_78 = V_5;
		if ((((int32_t)L_77) < ((int32_t)L_78)))
		{
			goto IL_01e2;
		}
	}
	{
		RuntimeArray * L_79 = V_6;
		V_0 = L_79;
		goto IL_0226;
	}

IL_0226:
	{
		RuntimeObject * L_80 = V_0;
		return L_80;
	}
}
// System.Object UnityEngine._AndroidJNIHelper::Unbox(UnityEngine.AndroidJavaObject)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * _AndroidJNIHelper_Unbox_m813AFB8DE2C2568B011C81ED3AC4D013F1E5B67E (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_AndroidJNIHelper_Unbox_m813AFB8DE2C2568B011C81ED3AC4D013F1E5B67E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	RuntimeObject * V_0 = NULL;
	AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * V_1 = NULL;
	String_t* V_2 = NULL;
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 12);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	{
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_0 = ___obj0;
		if (L_0)
		{
			goto IL_000e;
		}
	}
	{
		V_0 = NULL;
		goto IL_0211;
	}

IL_000e:
	{
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_1 = ___obj0;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_2 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)0);
		NullCheck(L_1);
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_3 = AndroidJavaObject_Call_TisAndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_m775AB90594C5F27D6099ED61119EF3608FD1001D(L_1, _stringLiteral4F05CBFCA4DFE76B99B142F609CDCF00D44FA247, L_2, /*hidden argument*/AndroidJavaObject_Call_TisAndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_m775AB90594C5F27D6099ED61119EF3608FD1001D_RuntimeMethod_var);
		V_1 = L_3;
	}

IL_0020:
	try
	{ // begin try (depth: 1)
		{
			AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_4 = V_1;
			ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_5 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)0);
			NullCheck(L_4);
			String_t* L_6 = AndroidJavaObject_Call_TisString_t_m5EAE53C9E2A8893FD8FEA710378D22C162A0FDEA(L_4, _stringLiteralFA98C1FD2CA6FC89B5ED010FD16AA461F50AFB3E, L_5, /*hidden argument*/AndroidJavaObject_Call_TisString_t_m5EAE53C9E2A8893FD8FEA710378D22C162A0FDEA_RuntimeMethod_var);
			V_2 = L_6;
			String_t* L_7 = V_2;
			bool L_8 = String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE(_stringLiteral9EAF6B54917BA48016AC5209BC15F62D5445708E, L_7, /*hidden argument*/NULL);
			if (!L_8)
			{
				goto IL_005f;
			}
		}

IL_0043:
		{
			AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_9 = ___obj0;
			ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_10 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)0);
			NullCheck(L_9);
			int32_t L_11 = AndroidJavaObject_Call_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_mF7220A3D48BA18737AA0C7DAF0828822275A69A6(L_9, _stringLiteralA0BCA4B2E667DD10532EED8280DA58E7BE1A8B88, L_10, /*hidden argument*/AndroidJavaObject_Call_TisInt32_t585191389E07734F19F3156FF88FB3EF4800D102_mF7220A3D48BA18737AA0C7DAF0828822275A69A6_RuntimeMethod_var);
			int32_t L_12 = L_11;
			RuntimeObject * L_13 = Box(Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var, &L_12);
			V_0 = L_13;
			IL2CPP_LEAVE(0x211, FINALLY_0204);
		}

IL_005f:
		{
			String_t* L_14 = V_2;
			bool L_15 = String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE(_stringLiteralBD119F910FA08AD4078969E4A551A13A7EA4D4BC, L_14, /*hidden argument*/NULL);
			if (!L_15)
			{
				goto IL_008b;
			}
		}

IL_006f:
		{
			AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_16 = ___obj0;
			ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_17 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)0);
			NullCheck(L_16);
			bool L_18 = AndroidJavaObject_Call_TisBoolean_tB53F6830F670160873277339AA58F15CAED4399C_m57EE1ACB271D15DD0E2DDD6B28805C31799A0976(L_16, _stringLiteral1D85A749A5C6FB273395A49AF6A07D9CF0C26A6D, L_17, /*hidden argument*/AndroidJavaObject_Call_TisBoolean_tB53F6830F670160873277339AA58F15CAED4399C_m57EE1ACB271D15DD0E2DDD6B28805C31799A0976_RuntimeMethod_var);
			bool L_19 = L_18;
			RuntimeObject * L_20 = Box(Boolean_tB53F6830F670160873277339AA58F15CAED4399C_il2cpp_TypeInfo_var, &L_19);
			V_0 = L_20;
			IL2CPP_LEAVE(0x211, FINALLY_0204);
		}

IL_008b:
		{
			String_t* L_21 = V_2;
			bool L_22 = String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE(_stringLiteralBD7E4A941C870AD23894466BB52628A9B488A1A2, L_21, /*hidden argument*/NULL);
			if (!L_22)
			{
				goto IL_00b7;
			}
		}

IL_009b:
		{
			AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_23 = ___obj0;
			ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_24 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)0);
			NullCheck(L_23);
			int8_t L_25 = AndroidJavaObject_Call_TisSByte_t9070AEA2966184235653CB9B4D33B149CDA831DF_m1DA87DAFADCDA8DE62A86D5C1F94DF60F2F54651(L_23, _stringLiteral5BD9EA45F0B419AD93E447295BC0AA4D644CF1B4, L_24, /*hidden argument*/AndroidJavaObject_Call_TisSByte_t9070AEA2966184235653CB9B4D33B149CDA831DF_m1DA87DAFADCDA8DE62A86D5C1F94DF60F2F54651_RuntimeMethod_var);
			int8_t L_26 = L_25;
			RuntimeObject * L_27 = Box(SByte_t9070AEA2966184235653CB9B4D33B149CDA831DF_il2cpp_TypeInfo_var, &L_26);
			V_0 = L_27;
			IL2CPP_LEAVE(0x211, FINALLY_0204);
		}

IL_00b7:
		{
			String_t* L_28 = V_2;
			bool L_29 = String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE(_stringLiteralEDB1046E80D3EA42FA26944C690CF3EB80C9CC62, L_28, /*hidden argument*/NULL);
			if (!L_29)
			{
				goto IL_00e3;
			}
		}

IL_00c7:
		{
			AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_30 = ___obj0;
			ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_31 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)0);
			NullCheck(L_30);
			int16_t L_32 = AndroidJavaObject_Call_TisInt16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D_mB51ADF5CFAE5278F11CECE74CC8ABAA9B45BB34F(L_30, _stringLiteral1EB9481D15B52FEE57E5AC17CA684460A95993E5, L_31, /*hidden argument*/AndroidJavaObject_Call_TisInt16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D_mB51ADF5CFAE5278F11CECE74CC8ABAA9B45BB34F_RuntimeMethod_var);
			int16_t L_33 = L_32;
			RuntimeObject * L_34 = Box(Int16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D_il2cpp_TypeInfo_var, &L_33);
			V_0 = L_34;
			IL2CPP_LEAVE(0x211, FINALLY_0204);
		}

IL_00e3:
		{
			String_t* L_35 = V_2;
			bool L_36 = String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE(_stringLiteral59CA046CC86D6DAAA8DF1A535C94F9AD1834F7FD, L_35, /*hidden argument*/NULL);
			if (!L_36)
			{
				goto IL_010f;
			}
		}

IL_00f3:
		{
			AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_37 = ___obj0;
			ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_38 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)0);
			NullCheck(L_37);
			int64_t L_39 = AndroidJavaObject_Call_TisInt64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436_mCD42F5F94257CC748CBA517A16A7BCC707A0C440(L_37, _stringLiteral2A42CDB77001E84D751FD683B088BBF833EEE0B3, L_38, /*hidden argument*/AndroidJavaObject_Call_TisInt64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436_mCD42F5F94257CC748CBA517A16A7BCC707A0C440_RuntimeMethod_var);
			int64_t L_40 = L_39;
			RuntimeObject * L_41 = Box(Int64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436_il2cpp_TypeInfo_var, &L_40);
			V_0 = L_41;
			IL2CPP_LEAVE(0x211, FINALLY_0204);
		}

IL_010f:
		{
			String_t* L_42 = V_2;
			bool L_43 = String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE(_stringLiteralDE5E596326DC7F422D1D5BFA854AA400BA53AE86, L_42, /*hidden argument*/NULL);
			if (!L_43)
			{
				goto IL_013b;
			}
		}

IL_011f:
		{
			AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_44 = ___obj0;
			ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_45 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)0);
			NullCheck(L_44);
			float L_46 = AndroidJavaObject_Call_TisSingle_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1_m241B6C5C3A0259B256071CA26CAFE3EF0F229DBA(L_44, _stringLiteral51750D533BBD6F70990AA487A711B4492A08F4BC, L_45, /*hidden argument*/AndroidJavaObject_Call_TisSingle_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1_m241B6C5C3A0259B256071CA26CAFE3EF0F229DBA_RuntimeMethod_var);
			float L_47 = L_46;
			RuntimeObject * L_48 = Box(Single_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1_il2cpp_TypeInfo_var, &L_47);
			V_0 = L_48;
			IL2CPP_LEAVE(0x211, FINALLY_0204);
		}

IL_013b:
		{
			String_t* L_49 = V_2;
			bool L_50 = String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE(_stringLiteralE6A7F51D4599E77D3EE682C1208434F332D9BF8D, L_49, /*hidden argument*/NULL);
			if (!L_50)
			{
				goto IL_0167;
			}
		}

IL_014b:
		{
			AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_51 = ___obj0;
			ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_52 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)0);
			NullCheck(L_51);
			double L_53 = AndroidJavaObject_Call_TisDouble_t358B8F23BDC52A5DD700E727E204F9F7CDE12409_mBDD67692E825B1F8834E22FC94628B9C6AE54C81(L_51, _stringLiteral917103D252076DA908A549A26BE33C64ABBD0EAC, L_52, /*hidden argument*/AndroidJavaObject_Call_TisDouble_t358B8F23BDC52A5DD700E727E204F9F7CDE12409_mBDD67692E825B1F8834E22FC94628B9C6AE54C81_RuntimeMethod_var);
			double L_54 = L_53;
			RuntimeObject * L_55 = Box(Double_t358B8F23BDC52A5DD700E727E204F9F7CDE12409_il2cpp_TypeInfo_var, &L_54);
			V_0 = L_55;
			IL2CPP_LEAVE(0x211, FINALLY_0204);
		}

IL_0167:
		{
			String_t* L_56 = V_2;
			bool L_57 = String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE(_stringLiteralD15A929AAC58DB1B939AAB2AEDA4342595D77F13, L_56, /*hidden argument*/NULL);
			if (!L_57)
			{
				goto IL_0193;
			}
		}

IL_0177:
		{
			AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_58 = ___obj0;
			ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_59 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)0);
			NullCheck(L_58);
			Il2CppChar L_60 = AndroidJavaObject_Call_TisChar_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9_m73C43D18BEC4AF2416AC8ADA8FA26712645A0EEA(L_58, _stringLiteral319523E1002BB1B6AB63E268BEEA610E6C9D8EEC, L_59, /*hidden argument*/AndroidJavaObject_Call_TisChar_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9_m73C43D18BEC4AF2416AC8ADA8FA26712645A0EEA_RuntimeMethod_var);
			Il2CppChar L_61 = L_60;
			RuntimeObject * L_62 = Box(Char_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9_il2cpp_TypeInfo_var, &L_61);
			V_0 = L_62;
			IL2CPP_LEAVE(0x211, FINALLY_0204);
		}

IL_0193:
		{
			String_t* L_63 = V_2;
			bool L_64 = String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE(_stringLiteral169775A78ADEE2D403BC1F88A1C1760F11C0304D, L_63, /*hidden argument*/NULL);
			if (!L_64)
			{
				goto IL_01ba;
			}
		}

IL_01a3:
		{
			AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_65 = ___obj0;
			ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_66 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)0);
			NullCheck(L_65);
			String_t* L_67 = AndroidJavaObject_Call_TisString_t_m5EAE53C9E2A8893FD8FEA710378D22C162A0FDEA(L_65, _stringLiteralA7EDC6086A91C13EEC0568F09CD6263D5A4CFFEC, L_66, /*hidden argument*/AndroidJavaObject_Call_TisString_t_m5EAE53C9E2A8893FD8FEA710378D22C162A0FDEA_RuntimeMethod_var);
			V_0 = L_67;
			IL2CPP_LEAVE(0x211, FINALLY_0204);
		}

IL_01ba:
		{
			String_t* L_68 = V_2;
			bool L_69 = String_op_Equality_m139F0E4195AE2F856019E63B241F36F016997FCE(_stringLiteral328CD2BEF0C16A2D306B28CD73848671CCC42AC2, L_68, /*hidden argument*/NULL);
			if (!L_69)
			{
				goto IL_01db;
			}
		}

IL_01ca:
		{
			AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_70 = ___obj0;
			NullCheck(L_70);
			intptr_t L_71 = AndroidJavaObject_GetRawObject_mCEB7EEC51D62A3E4F0D6F62C08CBEF008B556F3D(L_70, /*hidden argument*/NULL);
			AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * L_72 = (AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE *)il2cpp_codegen_object_new(AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE_il2cpp_TypeInfo_var);
			AndroidJavaClass__ctor_m44A6DEC0612D768E9947FFC1C2DA64D0605F34F1(L_72, (intptr_t)L_71, /*hidden argument*/NULL);
			V_0 = L_72;
			IL2CPP_LEAVE(0x211, FINALLY_0204);
		}

IL_01db:
		{
			AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_73 = V_1;
			ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_74 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)0);
			NullCheck(L_73);
			bool L_75 = AndroidJavaObject_Call_TisBoolean_tB53F6830F670160873277339AA58F15CAED4399C_m57EE1ACB271D15DD0E2DDD6B28805C31799A0976(L_73, _stringLiteralE33E0E502D09092EA117BE8C27FB58B1DD3AA609, L_74, /*hidden argument*/AndroidJavaObject_Call_TisBoolean_tB53F6830F670160873277339AA58F15CAED4399C_m57EE1ACB271D15DD0E2DDD6B28805C31799A0976_RuntimeMethod_var);
			if (!L_75)
			{
				goto IL_01fd;
			}
		}

IL_01f1:
		{
			AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_76 = ___obj0;
			RuntimeObject * L_77 = _AndroidJNIHelper_UnboxArray_m57E035906F4D79FCAC155162AC491BB7B575956D(L_76, /*hidden argument*/NULL);
			V_0 = L_77;
			IL2CPP_LEAVE(0x211, FINALLY_0204);
		}

IL_01fd:
		{
			AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_78 = ___obj0;
			V_0 = L_78;
			IL2CPP_LEAVE(0x211, FINALLY_0204);
		}
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0204;
	}

FINALLY_0204:
	{ // begin finally (depth: 1)
		{
			AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_79 = V_1;
			if (!L_79)
			{
				goto IL_0210;
			}
		}

IL_020a:
		{
			AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_80 = V_1;
			NullCheck(L_80);
			InterfaceActionInvoker0::Invoke(0 /* System.Void System.IDisposable::Dispose() */, IDisposable_t7218B22548186B208D65EA5B7870503810A2D15A_il2cpp_TypeInfo_var, L_80);
		}

IL_0210:
		{
			IL2CPP_END_FINALLY(516)
		}
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(516)
	{
		IL2CPP_JUMP_TBL(0x211, IL_0211)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0211:
	{
		RuntimeObject * L_81 = V_0;
		return L_81;
	}
}
// UnityEngine.AndroidJavaObject UnityEngine._AndroidJNIHelper::Box(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * _AndroidJNIHelper_Box_m67A2A786DCE5ADD2FAF4F27B7CA115C82A8768C2 (RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_AndroidJNIHelper_Box_m67A2A786DCE5ADD2FAF4F27B7CA115C82A8768C2_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * V_0 = NULL;
	{
		RuntimeObject * L_0 = ___obj0;
		if (L_0)
		{
			goto IL_000e;
		}
	}
	{
		V_0 = (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)NULL;
		goto IL_02c3;
	}

IL_000e:
	{
		RuntimeObject * L_1 = ___obj0;
		NullCheck(L_1);
		Type_t * L_2 = Object_GetType_m2E0B62414ECCAA3094B703790CE88CBB2F83EA60(L_1, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var);
		bool L_3 = AndroidReflection_IsPrimitive_m4C75B1AAEDD3FA0F73AFBC83CB374D3D8A9A3749(L_2, /*hidden argument*/NULL);
		if (!L_3)
		{
			goto IL_01e1;
		}
	}
	{
		RuntimeObject * L_4 = ___obj0;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_4, Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var)))
		{
			goto IL_004e;
		}
	}
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_5 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)1);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_6 = L_5;
		RuntimeObject * L_7 = ___obj0;
		int32_t L_8 = ((*(int32_t*)((int32_t*)UnBox(L_7, Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var))));
		RuntimeObject * L_9 = Box(Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var, &L_8);
		NullCheck(L_6);
		ArrayElementTypeCheck (L_6, L_9);
		(L_6)->SetAt(static_cast<il2cpp_array_size_t>(0), (RuntimeObject *)L_9);
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_10 = (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)il2cpp_codegen_object_new(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var);
		AndroidJavaObject__ctor_m39462EAD9AD82CBD90DEB4B7127F3D6C87A02BFA(L_10, _stringLiteral9EAF6B54917BA48016AC5209BC15F62D5445708E, L_6, /*hidden argument*/NULL);
		V_0 = L_10;
		goto IL_02c3;
	}

IL_004e:
	{
		RuntimeObject * L_11 = ___obj0;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_11, Boolean_tB53F6830F670160873277339AA58F15CAED4399C_il2cpp_TypeInfo_var)))
		{
			goto IL_007d;
		}
	}
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_12 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)1);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_13 = L_12;
		RuntimeObject * L_14 = ___obj0;
		bool L_15 = ((*(bool*)((bool*)UnBox(L_14, Boolean_tB53F6830F670160873277339AA58F15CAED4399C_il2cpp_TypeInfo_var))));
		RuntimeObject * L_16 = Box(Boolean_tB53F6830F670160873277339AA58F15CAED4399C_il2cpp_TypeInfo_var, &L_15);
		NullCheck(L_13);
		ArrayElementTypeCheck (L_13, L_16);
		(L_13)->SetAt(static_cast<il2cpp_array_size_t>(0), (RuntimeObject *)L_16);
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_17 = (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)il2cpp_codegen_object_new(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var);
		AndroidJavaObject__ctor_m39462EAD9AD82CBD90DEB4B7127F3D6C87A02BFA(L_17, _stringLiteralBD119F910FA08AD4078969E4A551A13A7EA4D4BC, L_13, /*hidden argument*/NULL);
		V_0 = L_17;
		goto IL_02c3;
	}

IL_007d:
	{
		RuntimeObject * L_18 = ___obj0;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_18, Byte_tF87C579059BD4633E6840EBBBEEF899C6E33EF07_il2cpp_TypeInfo_var)))
		{
			goto IL_00ac;
		}
	}
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_19 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)1);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_20 = L_19;
		RuntimeObject * L_21 = ___obj0;
		int8_t L_22 = ((*(int8_t*)((int8_t*)UnBox(L_21, SByte_t9070AEA2966184235653CB9B4D33B149CDA831DF_il2cpp_TypeInfo_var))));
		RuntimeObject * L_23 = Box(SByte_t9070AEA2966184235653CB9B4D33B149CDA831DF_il2cpp_TypeInfo_var, &L_22);
		NullCheck(L_20);
		ArrayElementTypeCheck (L_20, L_23);
		(L_20)->SetAt(static_cast<il2cpp_array_size_t>(0), (RuntimeObject *)L_23);
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_24 = (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)il2cpp_codegen_object_new(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var);
		AndroidJavaObject__ctor_m39462EAD9AD82CBD90DEB4B7127F3D6C87A02BFA(L_24, _stringLiteralBD7E4A941C870AD23894466BB52628A9B488A1A2, L_20, /*hidden argument*/NULL);
		V_0 = L_24;
		goto IL_02c3;
	}

IL_00ac:
	{
		RuntimeObject * L_25 = ___obj0;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_25, SByte_t9070AEA2966184235653CB9B4D33B149CDA831DF_il2cpp_TypeInfo_var)))
		{
			goto IL_00db;
		}
	}
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_26 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)1);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_27 = L_26;
		RuntimeObject * L_28 = ___obj0;
		int8_t L_29 = ((*(int8_t*)((int8_t*)UnBox(L_28, SByte_t9070AEA2966184235653CB9B4D33B149CDA831DF_il2cpp_TypeInfo_var))));
		RuntimeObject * L_30 = Box(SByte_t9070AEA2966184235653CB9B4D33B149CDA831DF_il2cpp_TypeInfo_var, &L_29);
		NullCheck(L_27);
		ArrayElementTypeCheck (L_27, L_30);
		(L_27)->SetAt(static_cast<il2cpp_array_size_t>(0), (RuntimeObject *)L_30);
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_31 = (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)il2cpp_codegen_object_new(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var);
		AndroidJavaObject__ctor_m39462EAD9AD82CBD90DEB4B7127F3D6C87A02BFA(L_31, _stringLiteralBD7E4A941C870AD23894466BB52628A9B488A1A2, L_27, /*hidden argument*/NULL);
		V_0 = L_31;
		goto IL_02c3;
	}

IL_00db:
	{
		RuntimeObject * L_32 = ___obj0;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_32, Int16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D_il2cpp_TypeInfo_var)))
		{
			goto IL_010a;
		}
	}
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_33 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)1);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_34 = L_33;
		RuntimeObject * L_35 = ___obj0;
		int16_t L_36 = ((*(int16_t*)((int16_t*)UnBox(L_35, Int16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D_il2cpp_TypeInfo_var))));
		RuntimeObject * L_37 = Box(Int16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D_il2cpp_TypeInfo_var, &L_36);
		NullCheck(L_34);
		ArrayElementTypeCheck (L_34, L_37);
		(L_34)->SetAt(static_cast<il2cpp_array_size_t>(0), (RuntimeObject *)L_37);
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_38 = (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)il2cpp_codegen_object_new(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var);
		AndroidJavaObject__ctor_m39462EAD9AD82CBD90DEB4B7127F3D6C87A02BFA(L_38, _stringLiteralEDB1046E80D3EA42FA26944C690CF3EB80C9CC62, L_34, /*hidden argument*/NULL);
		V_0 = L_38;
		goto IL_02c3;
	}

IL_010a:
	{
		RuntimeObject * L_39 = ___obj0;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_39, Int64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436_il2cpp_TypeInfo_var)))
		{
			goto IL_0139;
		}
	}
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_40 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)1);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_41 = L_40;
		RuntimeObject * L_42 = ___obj0;
		int64_t L_43 = ((*(int64_t*)((int64_t*)UnBox(L_42, Int64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436_il2cpp_TypeInfo_var))));
		RuntimeObject * L_44 = Box(Int64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436_il2cpp_TypeInfo_var, &L_43);
		NullCheck(L_41);
		ArrayElementTypeCheck (L_41, L_44);
		(L_41)->SetAt(static_cast<il2cpp_array_size_t>(0), (RuntimeObject *)L_44);
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_45 = (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)il2cpp_codegen_object_new(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var);
		AndroidJavaObject__ctor_m39462EAD9AD82CBD90DEB4B7127F3D6C87A02BFA(L_45, _stringLiteral59CA046CC86D6DAAA8DF1A535C94F9AD1834F7FD, L_41, /*hidden argument*/NULL);
		V_0 = L_45;
		goto IL_02c3;
	}

IL_0139:
	{
		RuntimeObject * L_46 = ___obj0;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_46, Single_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1_il2cpp_TypeInfo_var)))
		{
			goto IL_0168;
		}
	}
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_47 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)1);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_48 = L_47;
		RuntimeObject * L_49 = ___obj0;
		float L_50 = ((*(float*)((float*)UnBox(L_49, Single_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1_il2cpp_TypeInfo_var))));
		RuntimeObject * L_51 = Box(Single_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1_il2cpp_TypeInfo_var, &L_50);
		NullCheck(L_48);
		ArrayElementTypeCheck (L_48, L_51);
		(L_48)->SetAt(static_cast<il2cpp_array_size_t>(0), (RuntimeObject *)L_51);
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_52 = (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)il2cpp_codegen_object_new(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var);
		AndroidJavaObject__ctor_m39462EAD9AD82CBD90DEB4B7127F3D6C87A02BFA(L_52, _stringLiteralDE5E596326DC7F422D1D5BFA854AA400BA53AE86, L_48, /*hidden argument*/NULL);
		V_0 = L_52;
		goto IL_02c3;
	}

IL_0168:
	{
		RuntimeObject * L_53 = ___obj0;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_53, Double_t358B8F23BDC52A5DD700E727E204F9F7CDE12409_il2cpp_TypeInfo_var)))
		{
			goto IL_0197;
		}
	}
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_54 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)1);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_55 = L_54;
		RuntimeObject * L_56 = ___obj0;
		double L_57 = ((*(double*)((double*)UnBox(L_56, Double_t358B8F23BDC52A5DD700E727E204F9F7CDE12409_il2cpp_TypeInfo_var))));
		RuntimeObject * L_58 = Box(Double_t358B8F23BDC52A5DD700E727E204F9F7CDE12409_il2cpp_TypeInfo_var, &L_57);
		NullCheck(L_55);
		ArrayElementTypeCheck (L_55, L_58);
		(L_55)->SetAt(static_cast<il2cpp_array_size_t>(0), (RuntimeObject *)L_58);
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_59 = (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)il2cpp_codegen_object_new(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var);
		AndroidJavaObject__ctor_m39462EAD9AD82CBD90DEB4B7127F3D6C87A02BFA(L_59, _stringLiteralE6A7F51D4599E77D3EE682C1208434F332D9BF8D, L_55, /*hidden argument*/NULL);
		V_0 = L_59;
		goto IL_02c3;
	}

IL_0197:
	{
		RuntimeObject * L_60 = ___obj0;
		if (!((RuntimeObject *)IsInstSealed((RuntimeObject*)L_60, Char_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9_il2cpp_TypeInfo_var)))
		{
			goto IL_01c6;
		}
	}
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_61 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)1);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_62 = L_61;
		RuntimeObject * L_63 = ___obj0;
		Il2CppChar L_64 = ((*(Il2CppChar*)((Il2CppChar*)UnBox(L_63, Char_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9_il2cpp_TypeInfo_var))));
		RuntimeObject * L_65 = Box(Char_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9_il2cpp_TypeInfo_var, &L_64);
		NullCheck(L_62);
		ArrayElementTypeCheck (L_62, L_65);
		(L_62)->SetAt(static_cast<il2cpp_array_size_t>(0), (RuntimeObject *)L_65);
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_66 = (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)il2cpp_codegen_object_new(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var);
		AndroidJavaObject__ctor_m39462EAD9AD82CBD90DEB4B7127F3D6C87A02BFA(L_66, _stringLiteralD15A929AAC58DB1B939AAB2AEDA4342595D77F13, L_62, /*hidden argument*/NULL);
		V_0 = L_66;
		goto IL_02c3;
	}

IL_01c6:
	{
		RuntimeObject * L_67 = ___obj0;
		NullCheck(L_67);
		Type_t * L_68 = Object_GetType_m2E0B62414ECCAA3094B703790CE88CBB2F83EA60(L_67, /*hidden argument*/NULL);
		String_t* L_69 = String_Concat_m2E1F71C491D2429CC80A28745488FEA947BB7AAC(_stringLiteral94A9D9512BA3D2F295C65A0B3119715C79E6CB75, L_68, _stringLiteralBB589D0621E5472F470FA3425A234C74B1E202E8, /*hidden argument*/NULL);
		Exception_t * L_70 = (Exception_t *)il2cpp_codegen_object_new(Exception_t_il2cpp_TypeInfo_var);
		Exception__ctor_m89BADFF36C3B170013878726E07729D51AA9FBE0(L_70, L_69, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_70, NULL, _AndroidJNIHelper_Box_m67A2A786DCE5ADD2FAF4F27B7CA115C82A8768C2_RuntimeMethod_var);
	}

IL_01e1:
	{
		RuntimeObject * L_71 = ___obj0;
		if (!((String_t*)IsInstSealed((RuntimeObject*)L_71, String_t_il2cpp_TypeInfo_var)))
		{
			goto IL_020c;
		}
	}
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_72 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)1);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_73 = L_72;
		RuntimeObject * L_74 = ___obj0;
		NullCheck(L_73);
		ArrayElementTypeCheck (L_73, ((String_t*)CastclassSealed((RuntimeObject*)L_74, String_t_il2cpp_TypeInfo_var)));
		(L_73)->SetAt(static_cast<il2cpp_array_size_t>(0), (RuntimeObject *)((String_t*)CastclassSealed((RuntimeObject*)L_74, String_t_il2cpp_TypeInfo_var)));
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_75 = (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)il2cpp_codegen_object_new(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var);
		AndroidJavaObject__ctor_m39462EAD9AD82CBD90DEB4B7127F3D6C87A02BFA(L_75, _stringLiteral169775A78ADEE2D403BC1F88A1C1760F11C0304D, L_73, /*hidden argument*/NULL);
		V_0 = L_75;
		goto IL_02c3;
	}

IL_020c:
	{
		RuntimeObject * L_76 = ___obj0;
		if (!((AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE *)IsInstClass((RuntimeObject*)L_76, AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE_il2cpp_TypeInfo_var)))
		{
			goto IL_022e;
		}
	}
	{
		RuntimeObject * L_77 = ___obj0;
		NullCheck(((AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE *)CastclassClass((RuntimeObject*)L_77, AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE_il2cpp_TypeInfo_var)));
		intptr_t L_78 = AndroidJavaObject_GetRawClass_m28BFE7AD6A4FFCB45929D9D1A0F8D792C3974774(((AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE *)CastclassClass((RuntimeObject*)L_77, AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE_il2cpp_TypeInfo_var)), /*hidden argument*/NULL);
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_79 = (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)il2cpp_codegen_object_new(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var);
		AndroidJavaObject__ctor_m22E1E2E5D9F3DA31FF7DFB1339AD3BB0C3813E80(L_79, (intptr_t)L_78, /*hidden argument*/NULL);
		V_0 = L_79;
		goto IL_02c3;
	}

IL_022e:
	{
		RuntimeObject * L_80 = ___obj0;
		if (!((AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)IsInstClass((RuntimeObject*)L_80, AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var)))
		{
			goto IL_0246;
		}
	}
	{
		RuntimeObject * L_81 = ___obj0;
		V_0 = ((AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)CastclassClass((RuntimeObject*)L_81, AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var));
		goto IL_02c3;
	}

IL_0246:
	{
		RuntimeObject * L_82 = ___obj0;
		if (!((RuntimeArray *)IsInstClass((RuntimeObject*)L_82, RuntimeArray_il2cpp_TypeInfo_var)))
		{
			goto IL_0268;
		}
	}
	{
		RuntimeObject * L_83 = ___obj0;
		intptr_t L_84 = _AndroidJNIHelper_ConvertToJNIArray_mBF20C1B6716BA00CA9C3825EA446B291E6D8EB20(((RuntimeArray *)CastclassClass((RuntimeObject*)L_83, RuntimeArray_il2cpp_TypeInfo_var)), /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var);
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_85 = AndroidJavaObject_AndroidJavaObjectDeleteLocalRef_m0B0BCBDD56C299AC69938BDD4135E1B6EEAAC7EF((intptr_t)L_84, /*hidden argument*/NULL);
		V_0 = L_85;
		goto IL_02c3;
	}

IL_0268:
	{
		RuntimeObject * L_86 = ___obj0;
		if (!((AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D *)IsInstClass((RuntimeObject*)L_86, AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D_il2cpp_TypeInfo_var)))
		{
			goto IL_0285;
		}
	}
	{
		RuntimeObject * L_87 = ___obj0;
		NullCheck(((AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D *)CastclassClass((RuntimeObject*)L_87, AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D_il2cpp_TypeInfo_var)));
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_88 = AndroidJavaProxy_GetProxyObject_m411DC59BF56152B6058ABF99BBC8B64C813EEF06(((AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D *)CastclassClass((RuntimeObject*)L_87, AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D_il2cpp_TypeInfo_var)), /*hidden argument*/NULL);
		V_0 = L_88;
		goto IL_02c3;
	}

IL_0285:
	{
		RuntimeObject * L_89 = ___obj0;
		if (!((AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 *)IsInstSealed((RuntimeObject*)L_89, AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02_il2cpp_TypeInfo_var)))
		{
			goto IL_02a7;
		}
	}
	{
		RuntimeObject * L_90 = ___obj0;
		intptr_t L_91 = AndroidJNIHelper_CreateJavaRunnable_mA6C7A0E1BEF771970126D0FB21FF6E95CF569ED8(((AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 *)CastclassSealed((RuntimeObject*)L_90, AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02_il2cpp_TypeInfo_var)), /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var);
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_92 = AndroidJavaObject_AndroidJavaObjectDeleteLocalRef_m0B0BCBDD56C299AC69938BDD4135E1B6EEAAC7EF((intptr_t)L_91, /*hidden argument*/NULL);
		V_0 = L_92;
		goto IL_02c3;
	}

IL_02a7:
	{
		RuntimeObject * L_93 = ___obj0;
		NullCheck(L_93);
		Type_t * L_94 = Object_GetType_m2E0B62414ECCAA3094B703790CE88CBB2F83EA60(L_93, /*hidden argument*/NULL);
		String_t* L_95 = String_Concat_m2E1F71C491D2429CC80A28745488FEA947BB7AAC(_stringLiteral94A9D9512BA3D2F295C65A0B3119715C79E6CB75, L_94, _stringLiteralBB589D0621E5472F470FA3425A234C74B1E202E8, /*hidden argument*/NULL);
		Exception_t * L_96 = (Exception_t *)il2cpp_codegen_object_new(Exception_t_il2cpp_TypeInfo_var);
		Exception__ctor_m89BADFF36C3B170013878726E07729D51AA9FBE0(L_96, L_95, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_96, NULL, _AndroidJNIHelper_Box_m67A2A786DCE5ADD2FAF4F27B7CA115C82A8768C2_RuntimeMethod_var);
	}

IL_02c3:
	{
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_97 = V_0;
		return L_97;
	}
}
// System.Void UnityEngine._AndroidJNIHelper::DeleteJNIArgArray(System.Object[],UnityEngine.jvalue[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void _AndroidJNIHelper_DeleteJNIArgArray_mCD37E30D32E979ED19131F9DC77A8DDD69D2E1A5 (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args0, jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* ___jniArgs1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_AndroidJNIHelper_DeleteJNIArgArray_mCD37E30D32E979ED19131F9DC77A8DDD69D2E1A5_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	RuntimeObject * V_1 = NULL;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* V_2 = NULL;
	int32_t V_3 = 0;
	{
		V_0 = 0;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_0 = ___args0;
		V_2 = L_0;
		V_3 = 0;
		goto IL_004d;
	}

IL_000d:
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_1 = V_2;
		int32_t L_2 = V_3;
		NullCheck(L_1);
		int32_t L_3 = L_2;
		RuntimeObject * L_4 = (L_1)->GetAt(static_cast<il2cpp_array_size_t>(L_3));
		V_1 = L_4;
		RuntimeObject * L_5 = V_1;
		if (((String_t*)IsInstSealed((RuntimeObject*)L_5, String_t_il2cpp_TypeInfo_var)))
		{
			goto IL_0033;
		}
	}
	{
		RuntimeObject * L_6 = V_1;
		if (((AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02 *)IsInstSealed((RuntimeObject*)L_6, AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02_il2cpp_TypeInfo_var)))
		{
			goto IL_0033;
		}
	}
	{
		RuntimeObject * L_7 = V_1;
		if (!((RuntimeArray *)IsInstClass((RuntimeObject*)L_7, RuntimeArray_il2cpp_TypeInfo_var)))
		{
			goto IL_0044;
		}
	}

IL_0033:
	{
		jvalueU5BU5D_t9AA52DD48CAF5296AE8A2F758A488A2B14B820E3* L_8 = ___jniArgs1;
		int32_t L_9 = V_0;
		NullCheck(L_8);
		intptr_t L_10 = ((L_8)->GetAddressAt(static_cast<il2cpp_array_size_t>(L_9)))->get_l_8();
		AndroidJNISafe_DeleteLocalRef_m9632EA13BF03AEE43FC7713125962A4D0DFFADC7((intptr_t)L_10, /*hidden argument*/NULL);
	}

IL_0044:
	{
		int32_t L_11 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_add((int32_t)L_11, (int32_t)1));
		int32_t L_12 = V_3;
		V_3 = ((int32_t)il2cpp_codegen_add((int32_t)L_12, (int32_t)1));
	}

IL_004d:
	{
		int32_t L_13 = V_3;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_14 = V_2;
		NullCheck(L_14);
		if ((((int32_t)L_13) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_14)->max_length)))))))
		{
			goto IL_000d;
		}
	}
	{
		return;
	}
}
// System.IntPtr UnityEngine._AndroidJNIHelper::ConvertToJNIArray(System.Array)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t _AndroidJNIHelper_ConvertToJNIArray_mBF20C1B6716BA00CA9C3825EA446B291E6D8EB20 (RuntimeArray * ___array0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_AndroidJNIHelper_ConvertToJNIArray_mBF20C1B6716BA00CA9C3825EA446B291E6D8EB20_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Type_t * V_0 = NULL;
	intptr_t V_1;
	memset((&V_1), 0, sizeof(V_1));
	StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* V_2 = NULL;
	int32_t V_3 = 0;
	intptr_t V_4;
	memset((&V_4), 0, sizeof(V_4));
	intptr_t V_5;
	memset((&V_5), 0, sizeof(V_5));
	int32_t V_6 = 0;
	intptr_t V_7;
	memset((&V_7), 0, sizeof(V_7));
	AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379* V_8 = NULL;
	int32_t V_9 = 0;
	IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD* V_10 = NULL;
	intptr_t V_11;
	memset((&V_11), 0, sizeof(V_11));
	intptr_t V_12;
	memset((&V_12), 0, sizeof(V_12));
	int32_t V_13 = 0;
	intptr_t V_14;
	memset((&V_14), 0, sizeof(V_14));
	intptr_t V_15;
	memset((&V_15), 0, sizeof(V_15));
	{
		RuntimeArray * L_0 = ___array0;
		NullCheck(L_0);
		Type_t * L_1 = Object_GetType_m2E0B62414ECCAA3094B703790CE88CBB2F83EA60(L_0, /*hidden argument*/NULL);
		NullCheck(L_1);
		Type_t * L_2 = VirtFuncInvoker0< Type_t * >::Invoke(98 /* System.Type System.Type::GetElementType() */, L_1);
		V_0 = L_2;
		Type_t * L_3 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var);
		bool L_4 = AndroidReflection_IsPrimitive_m4C75B1AAEDD3FA0F73AFBC83CB374D3D8A9A3749(L_3, /*hidden argument*/NULL);
		if (!L_4)
		{
			goto IL_0153;
		}
	}
	{
		Type_t * L_5 = V_0;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_6 = { reinterpret_cast<intptr_t> (Int32_t585191389E07734F19F3156FF88FB3EF4800D102_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_7 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_6, /*hidden argument*/NULL);
		if ((!(((RuntimeObject*)(Type_t *)L_5) == ((RuntimeObject*)(Type_t *)L_7))))
		{
			goto IL_003a;
		}
	}
	{
		RuntimeArray * L_8 = ___array0;
		intptr_t L_9 = AndroidJNISafe_ToIntArray_m324EDE9CCF1C9909444C40617BD3358172EFB874(((Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83*)Castclass((RuntimeObject*)L_8, Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83_il2cpp_TypeInfo_var)), /*hidden argument*/NULL);
		V_1 = (intptr_t)L_9;
		goto IL_02d9;
	}

IL_003a:
	{
		Type_t * L_10 = V_0;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_11 = { reinterpret_cast<intptr_t> (Boolean_tB53F6830F670160873277339AA58F15CAED4399C_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_12 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_11, /*hidden argument*/NULL);
		if ((!(((RuntimeObject*)(Type_t *)L_10) == ((RuntimeObject*)(Type_t *)L_12))))
		{
			goto IL_005b;
		}
	}
	{
		RuntimeArray * L_13 = ___array0;
		intptr_t L_14 = AndroidJNISafe_ToBooleanArray_m1BCBD2041B6BFE6B91C1E3AD8C1133F791B70423(((BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040*)Castclass((RuntimeObject*)L_13, BooleanU5BU5D_t192C7579715690E25BD5EFED47F3E0FC9DCB2040_il2cpp_TypeInfo_var)), /*hidden argument*/NULL);
		V_1 = (intptr_t)L_14;
		goto IL_02d9;
	}

IL_005b:
	{
		Type_t * L_15 = V_0;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_16 = { reinterpret_cast<intptr_t> (Byte_tF87C579059BD4633E6840EBBBEEF899C6E33EF07_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_17 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_16, /*hidden argument*/NULL);
		if ((!(((RuntimeObject*)(Type_t *)L_15) == ((RuntimeObject*)(Type_t *)L_17))))
		{
			goto IL_0087;
		}
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_LogWarning_m37338644DC81F640CCDFEAE35A223F0E965F0568(_stringLiteral0EEE3EFAE974B24F801BA15D9AEC6ED2340751D1, /*hidden argument*/NULL);
		RuntimeArray * L_18 = ___array0;
		intptr_t L_19 = AndroidJNISafe_ToByteArray_m01C86D2FE9259F0888FA97B105FC741A0E2290D5(((ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821*)Castclass((RuntimeObject*)L_18, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821_il2cpp_TypeInfo_var)), /*hidden argument*/NULL);
		V_1 = (intptr_t)L_19;
		goto IL_02d9;
	}

IL_0087:
	{
		Type_t * L_20 = V_0;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_21 = { reinterpret_cast<intptr_t> (SByte_t9070AEA2966184235653CB9B4D33B149CDA831DF_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_22 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_21, /*hidden argument*/NULL);
		if ((!(((RuntimeObject*)(Type_t *)L_20) == ((RuntimeObject*)(Type_t *)L_22))))
		{
			goto IL_00a8;
		}
	}
	{
		RuntimeArray * L_23 = ___array0;
		intptr_t L_24 = AndroidJNISafe_ToSByteArray_m5AE0F49EE17ABDCFBCDF619CBECD5DEF9961BDB8(((SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889*)Castclass((RuntimeObject*)L_23, SByteU5BU5D_t623D1F33C61DEAC564E2B0560E00F1E1364F7889_il2cpp_TypeInfo_var)), /*hidden argument*/NULL);
		V_1 = (intptr_t)L_24;
		goto IL_02d9;
	}

IL_00a8:
	{
		Type_t * L_25 = V_0;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_26 = { reinterpret_cast<intptr_t> (Int16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_27 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_26, /*hidden argument*/NULL);
		if ((!(((RuntimeObject*)(Type_t *)L_25) == ((RuntimeObject*)(Type_t *)L_27))))
		{
			goto IL_00c9;
		}
	}
	{
		RuntimeArray * L_28 = ___array0;
		intptr_t L_29 = AndroidJNISafe_ToShortArray_m7D79F918714300B5818C7C8646E4E1A48E056A07(((Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28*)Castclass((RuntimeObject*)L_28, Int16U5BU5D_tDA0F0B2730337F72E44DB024BE9818FA8EDE8D28_il2cpp_TypeInfo_var)), /*hidden argument*/NULL);
		V_1 = (intptr_t)L_29;
		goto IL_02d9;
	}

IL_00c9:
	{
		Type_t * L_30 = V_0;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_31 = { reinterpret_cast<intptr_t> (Int64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_32 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_31, /*hidden argument*/NULL);
		if ((!(((RuntimeObject*)(Type_t *)L_30) == ((RuntimeObject*)(Type_t *)L_32))))
		{
			goto IL_00ea;
		}
	}
	{
		RuntimeArray * L_33 = ___array0;
		intptr_t L_34 = AndroidJNISafe_ToLongArray_mD59D9304170DFB59B77342C994699BE445AF25D3(((Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F*)Castclass((RuntimeObject*)L_33, Int64U5BU5D_tE04A3DEF6AF1C852A43B98A24EFB715806B37F5F_il2cpp_TypeInfo_var)), /*hidden argument*/NULL);
		V_1 = (intptr_t)L_34;
		goto IL_02d9;
	}

IL_00ea:
	{
		Type_t * L_35 = V_0;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_36 = { reinterpret_cast<intptr_t> (Single_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_37 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_36, /*hidden argument*/NULL);
		if ((!(((RuntimeObject*)(Type_t *)L_35) == ((RuntimeObject*)(Type_t *)L_37))))
		{
			goto IL_010b;
		}
	}
	{
		RuntimeArray * L_38 = ___array0;
		intptr_t L_39 = AndroidJNISafe_ToFloatArray_m8ACA5E42C6F32E7D851613AC129FB37AFC28EBFD(((SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5*)Castclass((RuntimeObject*)L_38, SingleU5BU5D_tA7139B7CAA40EAEF9178E2C386C8A5993754FDD5_il2cpp_TypeInfo_var)), /*hidden argument*/NULL);
		V_1 = (intptr_t)L_39;
		goto IL_02d9;
	}

IL_010b:
	{
		Type_t * L_40 = V_0;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_41 = { reinterpret_cast<intptr_t> (Double_t358B8F23BDC52A5DD700E727E204F9F7CDE12409_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_42 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_41, /*hidden argument*/NULL);
		if ((!(((RuntimeObject*)(Type_t *)L_40) == ((RuntimeObject*)(Type_t *)L_42))))
		{
			goto IL_012c;
		}
	}
	{
		RuntimeArray * L_43 = ___array0;
		intptr_t L_44 = AndroidJNISafe_ToDoubleArray_m9AE319DB92B91A255D2A0568D38B3B47CD0C69EB(((DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D*)Castclass((RuntimeObject*)L_43, DoubleU5BU5D_tF9383437DDA9EAC9F60627E9E6E2045CF7CB182D_il2cpp_TypeInfo_var)), /*hidden argument*/NULL);
		V_1 = (intptr_t)L_44;
		goto IL_02d9;
	}

IL_012c:
	{
		Type_t * L_45 = V_0;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_46 = { reinterpret_cast<intptr_t> (Char_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_47 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_46, /*hidden argument*/NULL);
		if ((!(((RuntimeObject*)(Type_t *)L_45) == ((RuntimeObject*)(Type_t *)L_47))))
		{
			goto IL_014d;
		}
	}
	{
		RuntimeArray * L_48 = ___array0;
		intptr_t L_49 = AndroidJNISafe_ToCharArray_m8AB18ECC188D1B8A15966FF3FBD7887CF35A5711(((CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2*)Castclass((RuntimeObject*)L_48, CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2_il2cpp_TypeInfo_var)), /*hidden argument*/NULL);
		V_1 = (intptr_t)L_49;
		goto IL_02d9;
	}

IL_014d:
	{
		goto IL_02ce;
	}

IL_0153:
	{
		Type_t * L_50 = V_0;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_51 = { reinterpret_cast<intptr_t> (String_t_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_52 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_51, /*hidden argument*/NULL);
		if ((!(((RuntimeObject*)(Type_t *)L_50) == ((RuntimeObject*)(Type_t *)L_52))))
		{
			goto IL_01d2;
		}
	}
	{
		RuntimeArray * L_53 = ___array0;
		V_2 = ((StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E*)Castclass((RuntimeObject*)L_53, StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E_il2cpp_TypeInfo_var));
		RuntimeArray * L_54 = ___array0;
		NullCheck(L_54);
		int32_t L_55 = Array_GetLength_m318900B10C3A93A30ABDC67DE161C8F6ABA4D359(L_54, 0, /*hidden argument*/NULL);
		V_3 = L_55;
		intptr_t L_56 = AndroidJNISafe_FindClass_mE58501828AA09ADC26347853AFE6D025845D487C(_stringLiteralC9291E1B62F25E545BD2AC4DF55EB10099666DCD, /*hidden argument*/NULL);
		V_4 = (intptr_t)L_56;
		int32_t L_57 = V_3;
		intptr_t L_58 = V_4;
		intptr_t L_59 = AndroidJNI_NewObjectArray_m49BBDBCC804A6799866B92D6E0DEA9A204B6BE43(L_57, (intptr_t)L_58, (intptr_t)(0), /*hidden argument*/NULL);
		V_5 = (intptr_t)L_59;
		V_6 = 0;
		goto IL_01bb;
	}

IL_0196:
	{
		StringU5BU5D_t933FB07893230EA91C40FF900D5400665E87B14E* L_60 = V_2;
		int32_t L_61 = V_6;
		NullCheck(L_60);
		int32_t L_62 = L_61;
		String_t* L_63 = (L_60)->GetAt(static_cast<il2cpp_array_size_t>(L_62));
		intptr_t L_64 = AndroidJNISafe_NewString_mD1D954E0EE5A8F135B19EE67E8FF2A4E1A6CA97F(L_63, /*hidden argument*/NULL);
		V_7 = (intptr_t)L_64;
		intptr_t L_65 = V_5;
		int32_t L_66 = V_6;
		intptr_t L_67 = V_7;
		AndroidJNI_SetObjectArrayElement_m3CB77880BEEAA75E69813F5B193F07BDD8933418((intptr_t)L_65, L_66, (intptr_t)L_67, /*hidden argument*/NULL);
		intptr_t L_68 = V_7;
		AndroidJNISafe_DeleteLocalRef_m9632EA13BF03AEE43FC7713125962A4D0DFFADC7((intptr_t)L_68, /*hidden argument*/NULL);
		int32_t L_69 = V_6;
		V_6 = ((int32_t)il2cpp_codegen_add((int32_t)L_69, (int32_t)1));
	}

IL_01bb:
	{
		int32_t L_70 = V_6;
		int32_t L_71 = V_3;
		if ((((int32_t)L_70) < ((int32_t)L_71)))
		{
			goto IL_0196;
		}
	}
	{
		intptr_t L_72 = V_4;
		AndroidJNISafe_DeleteLocalRef_m9632EA13BF03AEE43FC7713125962A4D0DFFADC7((intptr_t)L_72, /*hidden argument*/NULL);
		intptr_t L_73 = V_5;
		V_1 = (intptr_t)L_73;
		goto IL_02d9;
	}

IL_01d2:
	{
		Type_t * L_74 = V_0;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_75 = { reinterpret_cast<intptr_t> (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_76 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_75, /*hidden argument*/NULL);
		if ((!(((RuntimeObject*)(Type_t *)L_74) == ((RuntimeObject*)(Type_t *)L_76))))
		{
			goto IL_02b7;
		}
	}
	{
		RuntimeArray * L_77 = ___array0;
		V_8 = ((AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379*)Castclass((RuntimeObject*)L_77, AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379_il2cpp_TypeInfo_var));
		RuntimeArray * L_78 = ___array0;
		NullCheck(L_78);
		int32_t L_79 = Array_GetLength_m318900B10C3A93A30ABDC67DE161C8F6ABA4D359(L_78, 0, /*hidden argument*/NULL);
		V_9 = L_79;
		int32_t L_80 = V_9;
		IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD* L_81 = (IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD*)(IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD*)SZArrayNew(IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD_il2cpp_TypeInfo_var, (uint32_t)L_80);
		V_10 = L_81;
		intptr_t L_82 = AndroidJNISafe_FindClass_mE58501828AA09ADC26347853AFE6D025845D487C(_stringLiteral1C138278299F1B35865A79651A05DF52C0D74BB9, /*hidden argument*/NULL);
		V_11 = (intptr_t)L_82;
		V_12 = (intptr_t)(0);
		V_13 = 0;
		goto IL_0294;
	}

IL_0218:
	{
		AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379* L_83 = V_8;
		int32_t L_84 = V_13;
		NullCheck(L_83);
		int32_t L_85 = L_84;
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_86 = (L_83)->GetAt(static_cast<il2cpp_array_size_t>(L_85));
		if (!L_86)
		{
			goto IL_027c;
		}
	}
	{
		IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD* L_87 = V_10;
		int32_t L_88 = V_13;
		NullCheck(L_87);
		AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379* L_89 = V_8;
		int32_t L_90 = V_13;
		NullCheck(L_89);
		int32_t L_91 = L_90;
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_92 = (L_89)->GetAt(static_cast<il2cpp_array_size_t>(L_91));
		NullCheck(L_92);
		intptr_t L_93 = AndroidJavaObject_GetRawObject_mCEB7EEC51D62A3E4F0D6F62C08CBEF008B556F3D(L_92, /*hidden argument*/NULL);
		*((intptr_t*)((L_87)->GetAddressAt(static_cast<il2cpp_array_size_t>(L_88)))) = (intptr_t)L_93;
		AndroidJavaObjectU5BU5D_t7C44610B692603ADE504A389C4362A53613B5379* L_94 = V_8;
		int32_t L_95 = V_13;
		NullCheck(L_94);
		int32_t L_96 = L_95;
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_97 = (L_94)->GetAt(static_cast<il2cpp_array_size_t>(L_96));
		NullCheck(L_97);
		intptr_t L_98 = AndroidJavaObject_GetRawClass_m28BFE7AD6A4FFCB45929D9D1A0F8D792C3974774(L_97, /*hidden argument*/NULL);
		V_14 = (intptr_t)L_98;
		intptr_t L_99 = V_12;
		intptr_t L_100 = V_14;
		bool L_101 = IntPtr_op_Inequality_mB4886A806009EA825EFCC60CD2A7F6EB8E273A61((intptr_t)L_99, (intptr_t)L_100, /*hidden argument*/NULL);
		if (!L_101)
		{
			goto IL_0276;
		}
	}
	{
		intptr_t L_102 = V_12;
		bool L_103 = IntPtr_op_Equality_mEE8D9FD2DFE312BBAA8B4ED3BF7976B3142A5934((intptr_t)L_102, (intptr_t)(0), /*hidden argument*/NULL);
		if (!L_103)
		{
			goto IL_026f;
		}
	}
	{
		intptr_t L_104 = V_14;
		V_12 = (intptr_t)L_104;
		goto IL_0275;
	}

IL_026f:
	{
		intptr_t L_105 = V_11;
		V_12 = (intptr_t)L_105;
	}

IL_0275:
	{
	}

IL_0276:
	{
		goto IL_028d;
	}

IL_027c:
	{
		IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD* L_106 = V_10;
		int32_t L_107 = V_13;
		NullCheck(L_106);
		*((intptr_t*)((L_106)->GetAddressAt(static_cast<il2cpp_array_size_t>(L_107)))) = (intptr_t)(0);
	}

IL_028d:
	{
		int32_t L_108 = V_13;
		V_13 = ((int32_t)il2cpp_codegen_add((int32_t)L_108, (int32_t)1));
	}

IL_0294:
	{
		int32_t L_109 = V_13;
		int32_t L_110 = V_9;
		if ((((int32_t)L_109) < ((int32_t)L_110)))
		{
			goto IL_0218;
		}
	}
	{
		IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD* L_111 = V_10;
		intptr_t L_112 = V_12;
		intptr_t L_113 = AndroidJNISafe_ToObjectArray_mB3A0EB74E8C47EB72667603D90A4DE2480E2AC63(L_111, (intptr_t)L_112, /*hidden argument*/NULL);
		V_15 = (intptr_t)L_113;
		intptr_t L_114 = V_11;
		AndroidJNISafe_DeleteLocalRef_m9632EA13BF03AEE43FC7713125962A4D0DFFADC7((intptr_t)L_114, /*hidden argument*/NULL);
		intptr_t L_115 = V_15;
		V_1 = (intptr_t)L_115;
		goto IL_02d9;
	}

IL_02b7:
	{
		Type_t * L_116 = V_0;
		String_t* L_117 = String_Concat_m2E1F71C491D2429CC80A28745488FEA947BB7AAC(_stringLiteralB505B482020D33F0BA0DA1BE632CEF3BC4E82948, L_116, _stringLiteralBB589D0621E5472F470FA3425A234C74B1E202E8, /*hidden argument*/NULL);
		Exception_t * L_118 = (Exception_t *)il2cpp_codegen_object_new(Exception_t_il2cpp_TypeInfo_var);
		Exception__ctor_m89BADFF36C3B170013878726E07729D51AA9FBE0(L_118, L_117, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_118, NULL, _AndroidJNIHelper_ConvertToJNIArray_mBF20C1B6716BA00CA9C3825EA446B291E6D8EB20_RuntimeMethod_var);
	}

IL_02ce:
	{
		V_1 = (intptr_t)(0);
		goto IL_02d9;
	}

IL_02d9:
	{
		intptr_t L_119 = V_1;
		return (intptr_t)L_119;
	}
}
// System.IntPtr UnityEngine._AndroidJNIHelper::GetConstructorID(System.IntPtr,System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t _AndroidJNIHelper_GetConstructorID_m1982E4290531BD8134C7B5EDF918B87466284D77 (intptr_t ___jclass0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method)
{
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		intptr_t L_0 = ___jclass0;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_1 = ___args1;
		String_t* L_2 = _AndroidJNIHelper_GetSignature_m737340340A8C978F7AABB80DA4E31A8E700C73DA(L_1, /*hidden argument*/NULL);
		intptr_t L_3 = AndroidJNIHelper_GetConstructorID_m9978ECF944003B11786DDB1FDF0456CD89AF1180((intptr_t)L_0, L_2, /*hidden argument*/NULL);
		V_0 = (intptr_t)L_3;
		goto IL_0013;
	}

IL_0013:
	{
		intptr_t L_4 = V_0;
		return (intptr_t)L_4;
	}
}
// System.IntPtr UnityEngine._AndroidJNIHelper::GetConstructorID(System.IntPtr,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t _AndroidJNIHelper_GetConstructorID_m9A5019D80C0E776003ADFC0A54A879ECDC3B60D8 (intptr_t ___jclass0, String_t* ___signature1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_AndroidJNIHelper_GetConstructorID_m9A5019D80C0E776003ADFC0A54A879ECDC3B60D8_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	intptr_t V_1;
	memset((&V_1), 0, sizeof(V_1));
	Exception_t * V_2 = NULL;
	intptr_t V_3;
	memset((&V_3), 0, sizeof(V_3));
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 2);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	{
		V_0 = (intptr_t)(0);
	}

IL_0007:
	try
	{ // begin try (depth: 1)
		try
		{ // begin try (depth: 2)
			intptr_t L_0 = ___jclass0;
			String_t* L_1 = ___signature1;
			IL2CPP_RUNTIME_CLASS_INIT(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var);
			intptr_t L_2 = AndroidReflection_GetConstructorMember_mE78FA3844BBB2FE5A6D3A6719BE72BD33423F4C9((intptr_t)L_0, L_1, /*hidden argument*/NULL);
			V_0 = (intptr_t)L_2;
			intptr_t L_3 = V_0;
			intptr_t L_4 = AndroidJNISafe_FromReflectedMethod_m47AA20F4A2F8451B9BDCF8C6045802F04112F221((intptr_t)L_3, /*hidden argument*/NULL);
			V_1 = (intptr_t)L_4;
			IL2CPP_LEAVE(0x4D, FINALLY_0044);
		} // end try (depth: 2)
		catch(Il2CppExceptionWrapper& e)
		{
			__exception_local = (Exception_t *)e.ex;
			if(il2cpp_codegen_class_is_assignable_from (Exception_t_il2cpp_TypeInfo_var, il2cpp_codegen_object_class(e.ex)))
				goto CATCH_001c;
			throw e;
		}

CATCH_001c:
		{ // begin catch(System.Exception)
			{
				V_2 = ((Exception_t *)__exception_local);
				intptr_t L_5 = ___jclass0;
				String_t* L_6 = ___signature1;
				intptr_t L_7 = AndroidJNISafe_GetMethodID_m91CE11744503D04CD2AA8BAD99C914B1C2C6D494((intptr_t)L_5, _stringLiteral9E753E685FCDC6208CD59CF2FF3FDCCEB33023DD, L_6, /*hidden argument*/NULL);
				V_3 = (intptr_t)L_7;
				intptr_t L_8 = V_3;
				bool L_9 = IntPtr_op_Inequality_mB4886A806009EA825EFCC60CD2A7F6EB8E273A61((intptr_t)L_8, (intptr_t)(0), /*hidden argument*/NULL);
				if (!L_9)
				{
					goto IL_0042;
				}
			}

IL_003b:
			{
				intptr_t L_10 = V_3;
				V_1 = (intptr_t)L_10;
				IL2CPP_LEAVE(0x4D, FINALLY_0044);
			}

IL_0042:
			{
				Exception_t * L_11 = V_2;
				IL2CPP_RAISE_MANAGED_EXCEPTION(L_11, NULL, _AndroidJNIHelper_GetConstructorID_m9A5019D80C0E776003ADFC0A54A879ECDC3B60D8_RuntimeMethod_var);
			}
		} // end catch (depth: 2)
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0044;
	}

FINALLY_0044:
	{ // begin finally (depth: 1)
		intptr_t L_12 = V_0;
		AndroidJNISafe_DeleteLocalRef_m9632EA13BF03AEE43FC7713125962A4D0DFFADC7((intptr_t)L_12, /*hidden argument*/NULL);
		IL2CPP_END_FINALLY(68)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(68)
	{
		IL2CPP_JUMP_TBL(0x4D, IL_004d)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_004d:
	{
		intptr_t L_13 = V_1;
		return (intptr_t)L_13;
	}
}
// System.IntPtr UnityEngine._AndroidJNIHelper::GetMethodID(System.IntPtr,System.String,System.String,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t _AndroidJNIHelper_GetMethodID_m22C073C0BCB560A1AD9EE6158FF8314D291EF756 (intptr_t ___jclass0, String_t* ___methodName1, String_t* ___signature2, bool ___isStatic3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_AndroidJNIHelper_GetMethodID_m22C073C0BCB560A1AD9EE6158FF8314D291EF756_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	intptr_t V_1;
	memset((&V_1), 0, sizeof(V_1));
	Exception_t * V_2 = NULL;
	intptr_t V_3;
	memset((&V_3), 0, sizeof(V_3));
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 2);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	{
		V_0 = (intptr_t)(0);
	}

IL_0007:
	try
	{ // begin try (depth: 1)
		try
		{ // begin try (depth: 2)
			intptr_t L_0 = ___jclass0;
			String_t* L_1 = ___methodName1;
			String_t* L_2 = ___signature2;
			bool L_3 = ___isStatic3;
			IL2CPP_RUNTIME_CLASS_INIT(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var);
			intptr_t L_4 = AndroidReflection_GetMethodMember_m0B7C41F91CA0414D70EDFF7853BA93B11157EB19((intptr_t)L_0, L_1, L_2, L_3, /*hidden argument*/NULL);
			V_0 = (intptr_t)L_4;
			intptr_t L_5 = V_0;
			intptr_t L_6 = AndroidJNISafe_FromReflectedMethod_m47AA20F4A2F8451B9BDCF8C6045802F04112F221((intptr_t)L_5, /*hidden argument*/NULL);
			V_1 = (intptr_t)L_6;
			IL2CPP_LEAVE(0x4C, FINALLY_0043);
		} // end try (depth: 2)
		catch(Il2CppExceptionWrapper& e)
		{
			__exception_local = (Exception_t *)e.ex;
			if(il2cpp_codegen_class_is_assignable_from (Exception_t_il2cpp_TypeInfo_var, il2cpp_codegen_object_class(e.ex)))
				goto CATCH_001e;
			throw e;
		}

CATCH_001e:
		{ // begin catch(System.Exception)
			{
				V_2 = ((Exception_t *)__exception_local);
				intptr_t L_7 = ___jclass0;
				String_t* L_8 = ___methodName1;
				String_t* L_9 = ___signature2;
				bool L_10 = ___isStatic3;
				intptr_t L_11 = _AndroidJNIHelper_GetMethodIDFallback_m45AC36798A5258FE80A68A2453CE3C45792E2C95((intptr_t)L_7, L_8, L_9, L_10, /*hidden argument*/NULL);
				V_3 = (intptr_t)L_11;
				intptr_t L_12 = V_3;
				bool L_13 = IntPtr_op_Inequality_mB4886A806009EA825EFCC60CD2A7F6EB8E273A61((intptr_t)L_12, (intptr_t)(0), /*hidden argument*/NULL);
				if (!L_13)
				{
					goto IL_0041;
				}
			}

IL_003a:
			{
				intptr_t L_14 = V_3;
				V_1 = (intptr_t)L_14;
				IL2CPP_LEAVE(0x4C, FINALLY_0043);
			}

IL_0041:
			{
				Exception_t * L_15 = V_2;
				IL2CPP_RAISE_MANAGED_EXCEPTION(L_15, NULL, _AndroidJNIHelper_GetMethodID_m22C073C0BCB560A1AD9EE6158FF8314D291EF756_RuntimeMethod_var);
			}
		} // end catch (depth: 2)
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0043;
	}

FINALLY_0043:
	{ // begin finally (depth: 1)
		intptr_t L_16 = V_0;
		AndroidJNISafe_DeleteLocalRef_m9632EA13BF03AEE43FC7713125962A4D0DFFADC7((intptr_t)L_16, /*hidden argument*/NULL);
		IL2CPP_END_FINALLY(67)
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(67)
	{
		IL2CPP_JUMP_TBL(0x4C, IL_004c)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_004c:
	{
		intptr_t L_17 = V_1;
		return (intptr_t)L_17;
	}
}
// System.IntPtr UnityEngine._AndroidJNIHelper::GetMethodIDFallback(System.IntPtr,System.String,System.String,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR intptr_t _AndroidJNIHelper_GetMethodIDFallback_m45AC36798A5258FE80A68A2453CE3C45792E2C95 (intptr_t ___jclass0, String_t* ___methodName1, String_t* ___signature2, bool ___isStatic3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_AndroidJNIHelper_GetMethodIDFallback_m45AC36798A5258FE80A68A2453CE3C45792E2C95_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	intptr_t V_0;
	memset((&V_0), 0, sizeof(V_0));
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 2);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	intptr_t G_B4_0;
	memset((&G_B4_0), 0, sizeof(G_B4_0));
	{
	}

IL_0001:
	try
	{ // begin try (depth: 1)
		{
			bool L_0 = ___isStatic3;
			if (!L_0)
			{
				goto IL_0015;
			}
		}

IL_0008:
		{
			intptr_t L_1 = ___jclass0;
			String_t* L_2 = ___methodName1;
			String_t* L_3 = ___signature2;
			intptr_t L_4 = AndroidJNISafe_GetStaticMethodID_m4DCBC629048509F8E8566998CDA8F1AB9EAD6A50((intptr_t)L_1, L_2, L_3, /*hidden argument*/NULL);
			G_B4_0 = L_4;
			goto IL_001d;
		}

IL_0015:
		{
			intptr_t L_5 = ___jclass0;
			String_t* L_6 = ___methodName1;
			String_t* L_7 = ___signature2;
			intptr_t L_8 = AndroidJNISafe_GetMethodID_m91CE11744503D04CD2AA8BAD99C914B1C2C6D494((intptr_t)L_5, L_6, L_7, /*hidden argument*/NULL);
			G_B4_0 = L_8;
		}

IL_001d:
		{
			V_0 = (intptr_t)G_B4_0;
			goto IL_0036;
		}
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__exception_local = (Exception_t *)e.ex;
		if(il2cpp_codegen_class_is_assignable_from (Exception_t_il2cpp_TypeInfo_var, il2cpp_codegen_object_class(e.ex)))
			goto CATCH_0023;
		throw e;
	}

CATCH_0023:
	{ // begin catch(System.Exception)
		goto IL_002b;
	} // end catch (depth: 1)

IL_002b:
	{
		V_0 = (intptr_t)(0);
		goto IL_0036;
	}

IL_0036:
	{
		intptr_t L_9 = V_0;
		return (intptr_t)L_9;
	}
}
// System.String UnityEngine._AndroidJNIHelper::GetSignature(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* _AndroidJNIHelper_GetSignature_m090B053BFD9A6AC7BBD0F2BFAE56A8188CE4D80B (RuntimeObject * ___obj0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_AndroidJNIHelper_GetSignature_m090B053BFD9A6AC7BBD0F2BFAE56A8188CE4D80B_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	String_t* V_0 = NULL;
	Type_t * V_1 = NULL;
	AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * V_2 = NULL;
	AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * V_3 = NULL;
	AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * V_4 = NULL;
	StringBuilder_t * V_5 = NULL;
	Exception_t * __last_unhandled_exception = 0;
	NO_UNUSED_WARNING (__last_unhandled_exception);
	Exception_t * __exception_local = 0;
	NO_UNUSED_WARNING (__exception_local);
	void* __leave_targets_storage = alloca(sizeof(int32_t) * 1);
	il2cpp::utils::LeaveTargetStack __leave_targets(__leave_targets_storage);
	NO_UNUSED_WARNING (__leave_targets);
	Type_t * G_B5_0 = NULL;
	int32_t G_B47_0 = 0;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* G_B47_1 = NULL;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* G_B47_2 = NULL;
	int32_t G_B46_0 = 0;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* G_B46_1 = NULL;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* G_B46_2 = NULL;
	String_t* G_B48_0 = NULL;
	int32_t G_B48_1 = 0;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* G_B48_2 = NULL;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* G_B48_3 = NULL;
	{
		RuntimeObject * L_0 = ___obj0;
		if (L_0)
		{
			goto IL_0012;
		}
	}
	{
		V_0 = _stringLiteralF75D848FCD77B877799E37401451606B0778E2C5;
		goto IL_0346;
	}

IL_0012:
	{
		RuntimeObject * L_1 = ___obj0;
		if (!((Type_t *)IsInstClass((RuntimeObject*)L_1, Type_t_il2cpp_TypeInfo_var)))
		{
			goto IL_0028;
		}
	}
	{
		RuntimeObject * L_2 = ___obj0;
		G_B5_0 = ((Type_t *)CastclassClass((RuntimeObject*)L_2, Type_t_il2cpp_TypeInfo_var));
		goto IL_002e;
	}

IL_0028:
	{
		RuntimeObject * L_3 = ___obj0;
		NullCheck(L_3);
		Type_t * L_4 = Object_GetType_m2E0B62414ECCAA3094B703790CE88CBB2F83EA60(L_3, /*hidden argument*/NULL);
		G_B5_0 = L_4;
	}

IL_002e:
	{
		V_1 = G_B5_0;
		Type_t * L_5 = V_1;
		IL2CPP_RUNTIME_CLASS_INIT(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var);
		bool L_6 = AndroidReflection_IsPrimitive_m4C75B1AAEDD3FA0F73AFBC83CB374D3D8A9A3749(L_5, /*hidden argument*/NULL);
		if (!L_6)
		{
			goto IL_016c;
		}
	}
	{
		Type_t * L_7 = V_1;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_8 = { reinterpret_cast<intptr_t> (Int32_t585191389E07734F19F3156FF88FB3EF4800D102_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_9 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_8, /*hidden argument*/NULL);
		NullCheck(L_7);
		bool L_10 = VirtFuncInvoker1< bool, Type_t * >::Invoke(113 /* System.Boolean System.Type::Equals(System.Type) */, L_7, L_9);
		if (!L_10)
		{
			goto IL_005b;
		}
	}
	{
		V_0 = _stringLiteralCA73AB65568CD125C2D27A22BBD9E863C10B675D;
		goto IL_0346;
	}

IL_005b:
	{
		Type_t * L_11 = V_1;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_12 = { reinterpret_cast<intptr_t> (Boolean_tB53F6830F670160873277339AA58F15CAED4399C_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_13 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_12, /*hidden argument*/NULL);
		NullCheck(L_11);
		bool L_14 = VirtFuncInvoker1< bool, Type_t * >::Invoke(113 /* System.Boolean System.Type::Equals(System.Type) */, L_11, L_13);
		if (!L_14)
		{
			goto IL_007b;
		}
	}
	{
		V_0 = _stringLiteral909F99A779ADB66A76FC53AB56C7DD1CAF35D0FD;
		goto IL_0346;
	}

IL_007b:
	{
		Type_t * L_15 = V_1;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_16 = { reinterpret_cast<intptr_t> (Byte_tF87C579059BD4633E6840EBBBEEF899C6E33EF07_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_17 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_16, /*hidden argument*/NULL);
		NullCheck(L_15);
		bool L_18 = VirtFuncInvoker1< bool, Type_t * >::Invoke(113 /* System.Boolean System.Type::Equals(System.Type) */, L_15, L_17);
		if (!L_18)
		{
			goto IL_00a6;
		}
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_LogWarning_m37338644DC81F640CCDFEAE35A223F0E965F0568(_stringLiteral093986A34E3A8209AD12EC05E8E02A27BA4A9B4F, /*hidden argument*/NULL);
		V_0 = _stringLiteralAE4F281DF5A5D0FF3CAD6371F76D5C29B6D953EC;
		goto IL_0346;
	}

IL_00a6:
	{
		Type_t * L_19 = V_1;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_20 = { reinterpret_cast<intptr_t> (SByte_t9070AEA2966184235653CB9B4D33B149CDA831DF_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_21 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_20, /*hidden argument*/NULL);
		NullCheck(L_19);
		bool L_22 = VirtFuncInvoker1< bool, Type_t * >::Invoke(113 /* System.Boolean System.Type::Equals(System.Type) */, L_19, L_21);
		if (!L_22)
		{
			goto IL_00c6;
		}
	}
	{
		V_0 = _stringLiteralAE4F281DF5A5D0FF3CAD6371F76D5C29B6D953EC;
		goto IL_0346;
	}

IL_00c6:
	{
		Type_t * L_23 = V_1;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_24 = { reinterpret_cast<intptr_t> (Int16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_25 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_24, /*hidden argument*/NULL);
		NullCheck(L_23);
		bool L_26 = VirtFuncInvoker1< bool, Type_t * >::Invoke(113 /* System.Boolean System.Type::Equals(System.Type) */, L_23, L_25);
		if (!L_26)
		{
			goto IL_00e6;
		}
	}
	{
		V_0 = _stringLiteral02AA629C8B16CD17A44F3A0EFEC2FEED43937642;
		goto IL_0346;
	}

IL_00e6:
	{
		Type_t * L_27 = V_1;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_28 = { reinterpret_cast<intptr_t> (Int64_t7A386C2FF7B0280A0F516992401DDFCF0FF7B436_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_29 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_28, /*hidden argument*/NULL);
		NullCheck(L_27);
		bool L_30 = VirtFuncInvoker1< bool, Type_t * >::Invoke(113 /* System.Boolean System.Type::Equals(System.Type) */, L_27, L_29);
		if (!L_30)
		{
			goto IL_0106;
		}
	}
	{
		V_0 = _stringLiteral58668E7669FD564D99DB5D581FCDB6A5618440B5;
		goto IL_0346;
	}

IL_0106:
	{
		Type_t * L_31 = V_1;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_32 = { reinterpret_cast<intptr_t> (Single_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_33 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_32, /*hidden argument*/NULL);
		NullCheck(L_31);
		bool L_34 = VirtFuncInvoker1< bool, Type_t * >::Invoke(113 /* System.Boolean System.Type::Equals(System.Type) */, L_31, L_33);
		if (!L_34)
		{
			goto IL_0126;
		}
	}
	{
		V_0 = _stringLiteralE69F20E9F683920D3FB4329ABD951E878B1F9372;
		goto IL_0346;
	}

IL_0126:
	{
		Type_t * L_35 = V_1;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_36 = { reinterpret_cast<intptr_t> (Double_t358B8F23BDC52A5DD700E727E204F9F7CDE12409_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_37 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_36, /*hidden argument*/NULL);
		NullCheck(L_35);
		bool L_38 = VirtFuncInvoker1< bool, Type_t * >::Invoke(113 /* System.Boolean System.Type::Equals(System.Type) */, L_35, L_37);
		if (!L_38)
		{
			goto IL_0146;
		}
	}
	{
		V_0 = _stringLiteral50C9E8D5FC98727B4BBC93CF5D64A68DB647F04F;
		goto IL_0346;
	}

IL_0146:
	{
		Type_t * L_39 = V_1;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_40 = { reinterpret_cast<intptr_t> (Char_tBF22D9FC341BE970735250BB6FF1A4A92BBA58B9_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_41 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_40, /*hidden argument*/NULL);
		NullCheck(L_39);
		bool L_42 = VirtFuncInvoker1< bool, Type_t * >::Invoke(113 /* System.Boolean System.Type::Equals(System.Type) */, L_39, L_41);
		if (!L_42)
		{
			goto IL_0166;
		}
	}
	{
		V_0 = _stringLiteral32096C2E0EFF33D844EE6D675407ACE18289357D;
		goto IL_0346;
	}

IL_0166:
	{
		goto IL_033b;
	}

IL_016c:
	{
		Type_t * L_43 = V_1;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_44 = { reinterpret_cast<intptr_t> (String_t_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_45 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_44, /*hidden argument*/NULL);
		NullCheck(L_43);
		bool L_46 = VirtFuncInvoker1< bool, Type_t * >::Invoke(113 /* System.Boolean System.Type::Equals(System.Type) */, L_43, L_45);
		if (!L_46)
		{
			goto IL_018d;
		}
	}
	{
		V_0 = _stringLiteral84B35CD832E694499CB991F7B38517E07CFC129A;
		goto IL_0346;
	}

IL_018d:
	{
		RuntimeObject * L_47 = ___obj0;
		if (!((AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D *)IsInstClass((RuntimeObject*)L_47, AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D_il2cpp_TypeInfo_var)))
		{
			goto IL_01d5;
		}
	}
	{
		RuntimeObject * L_48 = ___obj0;
		NullCheck(((AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D *)CastclassClass((RuntimeObject*)L_48, AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D_il2cpp_TypeInfo_var)));
		AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE * L_49 = ((AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D *)CastclassClass((RuntimeObject*)L_48, AndroidJavaProxy_t72F996A75B4B771B9572C3770CF2D2C7A5B4783D_il2cpp_TypeInfo_var))->get_javaInterface_0();
		NullCheck(L_49);
		intptr_t L_50 = AndroidJavaObject_GetRawClass_m28BFE7AD6A4FFCB45929D9D1A0F8D792C3974774(L_49, /*hidden argument*/NULL);
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_51 = (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)il2cpp_codegen_object_new(AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var);
		AndroidJavaObject__ctor_m22E1E2E5D9F3DA31FF7DFB1339AD3BB0C3813E80(L_51, (intptr_t)L_50, /*hidden argument*/NULL);
		V_2 = L_51;
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_52 = V_2;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_53 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)0);
		NullCheck(L_52);
		String_t* L_54 = AndroidJavaObject_Call_TisString_t_m5EAE53C9E2A8893FD8FEA710378D22C162A0FDEA(L_52, _stringLiteralFA98C1FD2CA6FC89B5ED010FD16AA461F50AFB3E, L_53, /*hidden argument*/AndroidJavaObject_Call_TisString_t_m5EAE53C9E2A8893FD8FEA710378D22C162A0FDEA_RuntimeMethod_var);
		String_t* L_55 = String_Concat_mF4626905368D6558695A823466A1AF65EADB9923(_stringLiteralD160E0986ACA4714714A16F29EC605AF90BE704D, L_54, _stringLiteral2D14AB97CC3DC294C51C0D6814F4EA45F4B4E312, /*hidden argument*/NULL);
		V_0 = L_55;
		goto IL_0346;
	}

IL_01d5:
	{
		Type_t * L_56 = V_1;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_57 = { reinterpret_cast<intptr_t> (AndroidJavaRunnable_tE8AD56646A51EED70E12A2D0A542AC934BD87C02_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_58 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_57, /*hidden argument*/NULL);
		NullCheck(L_56);
		bool L_59 = VirtFuncInvoker1< bool, Type_t * >::Invoke(113 /* System.Boolean System.Type::Equals(System.Type) */, L_56, L_58);
		if (!L_59)
		{
			goto IL_01f6;
		}
	}
	{
		V_0 = _stringLiteralE46270F492F404A1D912A23E4DE44F3C7840F993;
		goto IL_0346;
	}

IL_01f6:
	{
		Type_t * L_60 = V_1;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_61 = { reinterpret_cast<intptr_t> (AndroidJavaClass_t799D386229C77D27C7E129BEF7A79AFD426084EE_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_62 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_61, /*hidden argument*/NULL);
		NullCheck(L_60);
		bool L_63 = VirtFuncInvoker1< bool, Type_t * >::Invoke(113 /* System.Boolean System.Type::Equals(System.Type) */, L_60, L_62);
		if (!L_63)
		{
			goto IL_0217;
		}
	}
	{
		V_0 = _stringLiteralB402D9DB865836815F1609AD99C0C12FA3DD8026;
		goto IL_0346;
	}

IL_0217:
	{
		Type_t * L_64 = V_1;
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_65 = { reinterpret_cast<intptr_t> (AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_66 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_65, /*hidden argument*/NULL);
		NullCheck(L_64);
		bool L_67 = VirtFuncInvoker1< bool, Type_t * >::Invoke(113 /* System.Boolean System.Type::Equals(System.Type) */, L_64, L_66);
		if (!L_67)
		{
			goto IL_0291;
		}
	}
	{
		RuntimeObject * L_68 = ___obj0;
		Type_t * L_69 = V_1;
		if ((!(((RuntimeObject*)(RuntimeObject *)L_68) == ((RuntimeObject*)(Type_t *)L_69))))
		{
			goto IL_0240;
		}
	}
	{
		V_0 = _stringLiteralF75D848FCD77B877799E37401451606B0778E2C5;
		goto IL_0346;
	}

IL_0240:
	{
		RuntimeObject * L_70 = ___obj0;
		V_3 = ((AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D *)CastclassClass((RuntimeObject*)L_70, AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_il2cpp_TypeInfo_var));
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_71 = V_3;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_72 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)0);
		NullCheck(L_71);
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_73 = AndroidJavaObject_Call_TisAndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_m775AB90594C5F27D6099ED61119EF3608FD1001D(L_71, _stringLiteral4F05CBFCA4DFE76B99B142F609CDCF00D44FA247, L_72, /*hidden argument*/AndroidJavaObject_Call_TisAndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D_m775AB90594C5F27D6099ED61119EF3608FD1001D_RuntimeMethod_var);
		V_4 = L_73;
	}

IL_025a:
	try
	{ // begin try (depth: 1)
		AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_74 = V_4;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_75 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)0);
		NullCheck(L_74);
		String_t* L_76 = AndroidJavaObject_Call_TisString_t_m5EAE53C9E2A8893FD8FEA710378D22C162A0FDEA(L_74, _stringLiteralFA98C1FD2CA6FC89B5ED010FD16AA461F50AFB3E, L_75, /*hidden argument*/AndroidJavaObject_Call_TisString_t_m5EAE53C9E2A8893FD8FEA710378D22C162A0FDEA_RuntimeMethod_var);
		String_t* L_77 = String_Concat_mF4626905368D6558695A823466A1AF65EADB9923(_stringLiteralD160E0986ACA4714714A16F29EC605AF90BE704D, L_76, _stringLiteral2D14AB97CC3DC294C51C0D6814F4EA45F4B4E312, /*hidden argument*/NULL);
		V_0 = L_77;
		IL2CPP_LEAVE(0x346, FINALLY_0282);
	} // end try (depth: 1)
	catch(Il2CppExceptionWrapper& e)
	{
		__last_unhandled_exception = (Exception_t *)e.ex;
		goto FINALLY_0282;
	}

FINALLY_0282:
	{ // begin finally (depth: 1)
		{
			AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_78 = V_4;
			if (!L_78)
			{
				goto IL_0290;
			}
		}

IL_0289:
		{
			AndroidJavaObject_t31F4DD4D4523A77B8AF16FE422B7426248E3093D * L_79 = V_4;
			NullCheck(L_79);
			InterfaceActionInvoker0::Invoke(0 /* System.Void System.IDisposable::Dispose() */, IDisposable_t7218B22548186B208D65EA5B7870503810A2D15A_il2cpp_TypeInfo_var, L_79);
		}

IL_0290:
		{
			IL2CPP_END_FINALLY(642)
		}
	} // end finally (depth: 1)
	IL2CPP_CLEANUP(642)
	{
		IL2CPP_JUMP_TBL(0x346, IL_0346)
		IL2CPP_RETHROW_IF_UNHANDLED(Exception_t *)
	}

IL_0291:
	{
		RuntimeTypeHandle_t7B542280A22F0EC4EAC2061C29178845847A8B2D  L_80 = { reinterpret_cast<intptr_t> (RuntimeArray_0_0_0_var) };
		IL2CPP_RUNTIME_CLASS_INIT(Type_t_il2cpp_TypeInfo_var);
		Type_t * L_81 = Type_GetTypeFromHandle_m9DC58ADF0512987012A8A016FB64B068F3B1AFF6(L_80, /*hidden argument*/NULL);
		Type_t * L_82 = V_1;
		IL2CPP_RUNTIME_CLASS_INIT(AndroidReflection_t4C31ACF30004C6250A0141026ED26532553C4533_il2cpp_TypeInfo_var);
		bool L_83 = AndroidReflection_IsAssignableFrom_m000432044555172C9399EB05A11AA35BFAF790FD(L_81, L_82, /*hidden argument*/NULL);
		if (!L_83)
		{
			goto IL_02f0;
		}
	}
	{
		Type_t * L_84 = V_1;
		NullCheck(L_84);
		int32_t L_85 = VirtFuncInvoker0< int32_t >::Invoke(28 /* System.Int32 System.Type::GetArrayRank() */, L_84);
		if ((((int32_t)L_85) == ((int32_t)1)))
		{
			goto IL_02bf;
		}
	}
	{
		Exception_t * L_86 = (Exception_t *)il2cpp_codegen_object_new(Exception_t_il2cpp_TypeInfo_var);
		Exception__ctor_m89BADFF36C3B170013878726E07729D51AA9FBE0(L_86, _stringLiteralFB0418121C28FD390FBFDEEBF12570C86FC00B32, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_86, NULL, _AndroidJNIHelper_GetSignature_m090B053BFD9A6AC7BBD0F2BFAE56A8188CE4D80B_RuntimeMethod_var);
	}

IL_02bf:
	{
		StringBuilder_t * L_87 = (StringBuilder_t *)il2cpp_codegen_object_new(StringBuilder_t_il2cpp_TypeInfo_var);
		StringBuilder__ctor_mF928376F82E8C8FF3C11842C562DB8CF28B2735E(L_87, /*hidden argument*/NULL);
		V_5 = L_87;
		StringBuilder_t * L_88 = V_5;
		NullCheck(L_88);
		StringBuilder_Append_m05C12F58ADC2D807613A9301DF438CB3CD09B75A(L_88, ((int32_t)91), /*hidden argument*/NULL);
		StringBuilder_t * L_89 = V_5;
		Type_t * L_90 = V_1;
		NullCheck(L_90);
		Type_t * L_91 = VirtFuncInvoker0< Type_t * >::Invoke(98 /* System.Type System.Type::GetElementType() */, L_90);
		String_t* L_92 = _AndroidJNIHelper_GetSignature_m090B053BFD9A6AC7BBD0F2BFAE56A8188CE4D80B(L_91, /*hidden argument*/NULL);
		NullCheck(L_89);
		StringBuilder_Append_mDBB8CCBB7750C67BE2F2D92F47E6C0FA42793260(L_89, L_92, /*hidden argument*/NULL);
		StringBuilder_t * L_93 = V_5;
		NullCheck(L_93);
		String_t* L_94 = VirtFuncInvoker0< String_t* >::Invoke(3 /* System.String System.Object::ToString() */, L_93);
		V_0 = L_94;
		goto IL_0346;
	}

IL_02f0:
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_95 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)6);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_96 = L_95;
		NullCheck(L_96);
		ArrayElementTypeCheck (L_96, _stringLiteralD98507F786B7E8AA37C8E9EE1D0452E55E21A08D);
		(L_96)->SetAt(static_cast<il2cpp_array_size_t>(0), (RuntimeObject *)_stringLiteralD98507F786B7E8AA37C8E9EE1D0452E55E21A08D);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_97 = L_96;
		Type_t * L_98 = V_1;
		NullCheck(L_97);
		ArrayElementTypeCheck (L_97, L_98);
		(L_97)->SetAt(static_cast<il2cpp_array_size_t>(1), (RuntimeObject *)L_98);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_99 = L_97;
		NullCheck(L_99);
		ArrayElementTypeCheck (L_99, _stringLiteral8777D1BEFDBAE64EDD9D49FE596B0CC904692081);
		(L_99)->SetAt(static_cast<il2cpp_array_size_t>(2), (RuntimeObject *)_stringLiteral8777D1BEFDBAE64EDD9D49FE596B0CC904692081);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_100 = L_99;
		RuntimeObject * L_101 = ___obj0;
		NullCheck(L_100);
		ArrayElementTypeCheck (L_100, L_101);
		(L_100)->SetAt(static_cast<il2cpp_array_size_t>(3), (RuntimeObject *)L_101);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_102 = L_100;
		NullCheck(L_102);
		ArrayElementTypeCheck (L_102, _stringLiteral3E2B500817A96FA5BAECB12EAFF42205003D74E6);
		(L_102)->SetAt(static_cast<il2cpp_array_size_t>(4), (RuntimeObject *)_stringLiteral3E2B500817A96FA5BAECB12EAFF42205003D74E6);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_103 = L_102;
		Type_t * L_104 = V_1;
		RuntimeObject * L_105 = ___obj0;
		G_B46_0 = 5;
		G_B46_1 = L_103;
		G_B46_2 = L_103;
		if ((!(((RuntimeObject*)(Type_t *)L_104) == ((RuntimeObject*)(RuntimeObject *)L_105))))
		{
			G_B47_0 = 5;
			G_B47_1 = L_103;
			G_B47_2 = L_103;
			goto IL_032a;
		}
	}
	{
		G_B48_0 = _stringLiteralF57B2D312D9EFE8FE993C8EB1F3E19D41AD04030;
		G_B48_1 = G_B46_0;
		G_B48_2 = G_B46_1;
		G_B48_3 = G_B46_2;
		goto IL_032f;
	}

IL_032a:
	{
		G_B48_0 = _stringLiteralC3BEC6BCBC9B9F04E60FCB1D9C9C1A37F3E12E93;
		G_B48_1 = G_B47_0;
		G_B48_2 = G_B47_1;
		G_B48_3 = G_B47_2;
	}

IL_032f:
	{
		NullCheck(G_B48_2);
		ArrayElementTypeCheck (G_B48_2, G_B48_0);
		(G_B48_2)->SetAt(static_cast<il2cpp_array_size_t>(G_B48_1), (RuntimeObject *)G_B48_0);
		String_t* L_106 = String_Concat_mB7BA84F13912303B2E5E40FBF0109E1A328ACA07(G_B48_3, /*hidden argument*/NULL);
		Exception_t * L_107 = (Exception_t *)il2cpp_codegen_object_new(Exception_t_il2cpp_TypeInfo_var);
		Exception__ctor_m89BADFF36C3B170013878726E07729D51AA9FBE0(L_107, L_106, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_107, NULL, _AndroidJNIHelper_GetSignature_m090B053BFD9A6AC7BBD0F2BFAE56A8188CE4D80B_RuntimeMethod_var);
	}

IL_033b:
	{
		V_0 = _stringLiteralDA39A3EE5E6B4B0D3255BFEF95601890AFD80709;
		goto IL_0346;
	}

IL_0346:
	{
		String_t* L_108 = V_0;
		return L_108;
	}
}
// System.String UnityEngine._AndroidJNIHelper::GetSignature(System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* _AndroidJNIHelper_GetSignature_m737340340A8C978F7AABB80DA4E31A8E700C73DA (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (_AndroidJNIHelper_GetSignature_m737340340A8C978F7AABB80DA4E31A8E700C73DA_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	StringBuilder_t * V_0 = NULL;
	RuntimeObject * V_1 = NULL;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* V_2 = NULL;
	int32_t V_3 = 0;
	String_t* V_4 = NULL;
	{
		StringBuilder_t * L_0 = (StringBuilder_t *)il2cpp_codegen_object_new(StringBuilder_t_il2cpp_TypeInfo_var);
		StringBuilder__ctor_mF928376F82E8C8FF3C11842C562DB8CF28B2735E(L_0, /*hidden argument*/NULL);
		V_0 = L_0;
		StringBuilder_t * L_1 = V_0;
		NullCheck(L_1);
		StringBuilder_Append_m05C12F58ADC2D807613A9301DF438CB3CD09B75A(L_1, ((int32_t)40), /*hidden argument*/NULL);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_2 = ___args0;
		V_2 = L_2;
		V_3 = 0;
		goto IL_0031;
	}

IL_001a:
	{
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_3 = V_2;
		int32_t L_4 = V_3;
		NullCheck(L_3);
		int32_t L_5 = L_4;
		RuntimeObject * L_6 = (L_3)->GetAt(static_cast<il2cpp_array_size_t>(L_5));
		V_1 = L_6;
		StringBuilder_t * L_7 = V_0;
		RuntimeObject * L_8 = V_1;
		String_t* L_9 = _AndroidJNIHelper_GetSignature_m090B053BFD9A6AC7BBD0F2BFAE56A8188CE4D80B(L_8, /*hidden argument*/NULL);
		NullCheck(L_7);
		StringBuilder_Append_mDBB8CCBB7750C67BE2F2D92F47E6C0FA42793260(L_7, L_9, /*hidden argument*/NULL);
		int32_t L_10 = V_3;
		V_3 = ((int32_t)il2cpp_codegen_add((int32_t)L_10, (int32_t)1));
	}

IL_0031:
	{
		int32_t L_11 = V_3;
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_12 = V_2;
		NullCheck(L_12);
		if ((((int32_t)L_11) < ((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_12)->max_length)))))))
		{
			goto IL_001a;
		}
	}
	{
		StringBuilder_t * L_13 = V_0;
		NullCheck(L_13);
		StringBuilder_Append_mDBB8CCBB7750C67BE2F2D92F47E6C0FA42793260(L_13, _stringLiteral327C3BC0993A6F3EF91265DAF24D8D1A4076818E, /*hidden argument*/NULL);
		StringBuilder_t * L_14 = V_0;
		NullCheck(L_14);
		String_t* L_15 = VirtFuncInvoker0< String_t* >::Invoke(3 /* System.String System.Object::ToString() */, L_14);
		V_4 = L_15;
		goto IL_0053;
	}

IL_0053:
	{
		String_t* L_16 = V_4;
		return L_16;
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
// Conversion methods for marshalling of: UnityEngine.jvalue
IL2CPP_EXTERN_C void jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37_marshal_pinvoke(const jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37& unmarshaled, jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37_marshaled_pinvoke& marshaled)
{
	marshaled.___z_0 = static_cast<int32_t>(unmarshaled.get_z_0());
	marshaled.___b_1 = unmarshaled.get_b_1();
	marshaled.___c_2 = static_cast<uint8_t>(unmarshaled.get_c_2());
	marshaled.___s_3 = unmarshaled.get_s_3();
	marshaled.___i_4 = unmarshaled.get_i_4();
	marshaled.___j_5 = unmarshaled.get_j_5();
	marshaled.___f_6 = unmarshaled.get_f_6();
	marshaled.___d_7 = unmarshaled.get_d_7();
	marshaled.___l_8 = unmarshaled.get_l_8();
}
IL2CPP_EXTERN_C void jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37_marshal_pinvoke_back(const jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37_marshaled_pinvoke& marshaled, jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37& unmarshaled)
{
	bool unmarshaled_z_temp_0 = false;
	unmarshaled_z_temp_0 = static_cast<bool>(marshaled.___z_0);
	unmarshaled.set_z_0(unmarshaled_z_temp_0);
	int8_t unmarshaled_b_temp_1 = 0x0;
	unmarshaled_b_temp_1 = marshaled.___b_1;
	unmarshaled.set_b_1(unmarshaled_b_temp_1);
	Il2CppChar unmarshaled_c_temp_2 = 0x0;
	unmarshaled_c_temp_2 = static_cast<Il2CppChar>(marshaled.___c_2);
	unmarshaled.set_c_2(unmarshaled_c_temp_2);
	int16_t unmarshaled_s_temp_3 = 0;
	unmarshaled_s_temp_3 = marshaled.___s_3;
	unmarshaled.set_s_3(unmarshaled_s_temp_3);
	int32_t unmarshaled_i_temp_4 = 0;
	unmarshaled_i_temp_4 = marshaled.___i_4;
	unmarshaled.set_i_4(unmarshaled_i_temp_4);
	int64_t unmarshaled_j_temp_5 = 0;
	unmarshaled_j_temp_5 = marshaled.___j_5;
	unmarshaled.set_j_5(unmarshaled_j_temp_5);
	float unmarshaled_f_temp_6 = 0.0f;
	unmarshaled_f_temp_6 = marshaled.___f_6;
	unmarshaled.set_f_6(unmarshaled_f_temp_6);
	double unmarshaled_d_temp_7 = 0.0;
	unmarshaled_d_temp_7 = marshaled.___d_7;
	unmarshaled.set_d_7(unmarshaled_d_temp_7);
	intptr_t unmarshaled_l_temp_8;
	memset((&unmarshaled_l_temp_8), 0, sizeof(unmarshaled_l_temp_8));
	unmarshaled_l_temp_8 = marshaled.___l_8;
	unmarshaled.set_l_8(unmarshaled_l_temp_8);
}
// Conversion method for clean up from marshalling of: UnityEngine.jvalue
IL2CPP_EXTERN_C void jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37_marshal_pinvoke_cleanup(jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37_marshaled_pinvoke& marshaled)
{
}
// Conversion methods for marshalling of: UnityEngine.jvalue
IL2CPP_EXTERN_C void jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37_marshal_com(const jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37& unmarshaled, jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37_marshaled_com& marshaled)
{
	marshaled.___z_0 = static_cast<int32_t>(unmarshaled.get_z_0());
	marshaled.___b_1 = unmarshaled.get_b_1();
	marshaled.___c_2 = static_cast<uint8_t>(unmarshaled.get_c_2());
	marshaled.___s_3 = unmarshaled.get_s_3();
	marshaled.___i_4 = unmarshaled.get_i_4();
	marshaled.___j_5 = unmarshaled.get_j_5();
	marshaled.___f_6 = unmarshaled.get_f_6();
	marshaled.___d_7 = unmarshaled.get_d_7();
	marshaled.___l_8 = unmarshaled.get_l_8();
}
IL2CPP_EXTERN_C void jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37_marshal_com_back(const jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37_marshaled_com& marshaled, jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37& unmarshaled)
{
	bool unmarshaled_z_temp_0 = false;
	unmarshaled_z_temp_0 = static_cast<bool>(marshaled.___z_0);
	unmarshaled.set_z_0(unmarshaled_z_temp_0);
	int8_t unmarshaled_b_temp_1 = 0x0;
	unmarshaled_b_temp_1 = marshaled.___b_1;
	unmarshaled.set_b_1(unmarshaled_b_temp_1);
	Il2CppChar unmarshaled_c_temp_2 = 0x0;
	unmarshaled_c_temp_2 = static_cast<Il2CppChar>(marshaled.___c_2);
	unmarshaled.set_c_2(unmarshaled_c_temp_2);
	int16_t unmarshaled_s_temp_3 = 0;
	unmarshaled_s_temp_3 = marshaled.___s_3;
	unmarshaled.set_s_3(unmarshaled_s_temp_3);
	int32_t unmarshaled_i_temp_4 = 0;
	unmarshaled_i_temp_4 = marshaled.___i_4;
	unmarshaled.set_i_4(unmarshaled_i_temp_4);
	int64_t unmarshaled_j_temp_5 = 0;
	unmarshaled_j_temp_5 = marshaled.___j_5;
	unmarshaled.set_j_5(unmarshaled_j_temp_5);
	float unmarshaled_f_temp_6 = 0.0f;
	unmarshaled_f_temp_6 = marshaled.___f_6;
	unmarshaled.set_f_6(unmarshaled_f_temp_6);
	double unmarshaled_d_temp_7 = 0.0;
	unmarshaled_d_temp_7 = marshaled.___d_7;
	unmarshaled.set_d_7(unmarshaled_d_temp_7);
	intptr_t unmarshaled_l_temp_8;
	memset((&unmarshaled_l_temp_8), 0, sizeof(unmarshaled_l_temp_8));
	unmarshaled_l_temp_8 = marshaled.___l_8;
	unmarshaled.set_l_8(unmarshaled_l_temp_8);
}
// Conversion method for clean up from marshalling of: UnityEngine.jvalue
IL2CPP_EXTERN_C void jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37_marshal_com_cleanup(jvalue_t98310C8FA21DF12CBE79266684536EDE1B7F9C37_marshaled_com& marshaled)
{
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR Exception_t * Exception_get_InnerException_mCB68CC8CBF2540EF381CB17A4E4E3F6D0E33453F_inline (Exception_t * __this, const RuntimeMethod* method)
{
	{
		Exception_t * L_0 = __this->get__innerException_4();
		return L_0;
	}
}
