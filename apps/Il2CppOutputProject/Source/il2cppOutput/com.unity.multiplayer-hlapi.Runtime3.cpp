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
template <typename R, typename T1, typename T2, typename T3, typename T4, typename T5>
struct VirtFuncInvoker5
{
	typedef R (*Func)(void*, T1, T2, T3, T4, T5, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, p3, p4, p5, invokeData.method);
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
template <typename R, typename T1, typename T2>
struct GenericVirtFuncInvoker2
{
	typedef R (*Func)(void*, T1, T2, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_virtual_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
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
struct InterfaceFuncInvoker3
{
	typedef R (*Func)(void*, T1, T2, T3, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2, T3 p3)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, p3, invokeData.method);
	}
};
template <typename R, typename T1, typename T2>
struct InterfaceFuncInvoker2
{
	typedef R (*Func)(void*, T1, T2, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
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
template <typename R, typename T1, typename T2>
struct GenericInterfaceFuncInvoker2
{
	typedef R (*Func)(void*, T1, T2, const RuntimeMethod*);

	static inline R Invoke (const RuntimeMethod* method, RuntimeObject* obj, T1 p1, T2 p2)
	{
		VirtualInvokeData invokeData;
		il2cpp_codegen_get_generic_interface_invoke_data(method, obj, &invokeData);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
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

// System.AsyncCallback
struct AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4;
// System.Attribute
struct Attribute_tF048C13FB3C8CFCC53F82290E4A3F621089F9A74;
// System.Byte[]
struct ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821;
// System.Char[]
struct CharU5BU5D_t4CC6ABF0AD71BEC97E3C2F1E9C5677E46D3A75C2;
// System.Collections.Generic.Dictionary`2<System.Int16,UnityEngine.Networking.NetworkConnection/PacketStat>
struct Dictionary_2_t3C696EAE739BB0B87CB145AEF2D2B55EA1CAE88F;
// System.Collections.Generic.Dictionary`2<System.Int16,UnityEngine.Networking.NetworkMessageDelegate>
struct Dictionary_2_t519615383E326CAA4218E3A39FB706EE903B11C8;
// System.Collections.Generic.Dictionary`2<System.Int32,UnityEngine.Networking.NetworkBehaviour/Invoker>
struct Dictionary_2_tCB6A26454DC24D4ED3A427AD6A6B9ADDA3A74D0D;
// System.Collections.Generic.Dictionary`2<UnityEngine.Networking.NetworkSceneId,UnityEngine.Networking.NetworkIdentity>
struct Dictionary_2_t7BABA42F397000124B62D0DE8AC6226B5276B2CA;
// System.Collections.Generic.HashSet`1/Slot<UnityEngine.Networking.NetworkInstanceId>[]
struct SlotU5BU5D_t971A4EBC1B2F2C5607B8B63726102B5989FF8B4A;
// System.Collections.Generic.HashSet`1<System.Int32>
struct HashSet_1_tC4214D83D479652EF2A07346543F228C3C0A8D74;
// System.Collections.Generic.HashSet`1<UnityEngine.Networking.NetworkIdentity>
struct HashSet_1_tAFF21BA556217C09A0897CBE50F53A1AD6C24EC1;
// System.Collections.Generic.HashSet`1<UnityEngine.Networking.NetworkInstanceId>
struct HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990;
// System.Collections.Generic.IEqualityComparer`1<UnityEngine.Networking.NetworkInstanceId>
struct IEqualityComparer_1_tD9C10E2393CE40CDC1E9BDDD6BB2CF30D2451214;
// System.Collections.Generic.List`1<System.Boolean>
struct List_1_tCF6613377FD07378DDA05A5BC95C5EF4A07B3E75;
// System.Collections.Generic.List`1<System.Int32>
struct List_1_tE1526161A558A17A39A8B69D8EEF3801393B6226;
// System.Collections.Generic.List`1<System.Single>
struct List_1_t8980FA0E6CB3848F706C43D859930435C34BCC37;
// System.Collections.Generic.List`1<System.String>
struct List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3;
// System.Collections.Generic.List`1<System.UInt32>
struct List_1_t49B315A213A231954A3718D77EE3A2AFF443C38E;
// System.Collections.Generic.List`1<UnityEngine.Networking.ClientScene/PendingOwner>
struct List_1_t93B3F1949B711B014F8D6B02F94C18FA9A0B4EC0;
// System.Collections.Generic.List`1<UnityEngine.Networking.LocalClient/InternalMsg>
struct List_1_tD68CD4018F6A1BB25DFAABF5C75012912E867F6A;
// System.Collections.Generic.List`1<UnityEngine.Networking.NetworkClient>
struct List_1_t7816E78619327B971A54376C3C9CDD6E84077D6D;
// System.Collections.Generic.List`1<UnityEngine.Networking.NetworkConnection>
struct List_1_t8B02DD1F0211D3E19F6A6E0204AF7D7537912362;
// System.Collections.Generic.List`1<UnityEngine.Networking.PlayerController>
struct List_1_t44D1B61364FCFEF62067A4726A735856DFDFD545;
// System.Collections.Generic.Stack`1<UnityEngine.Networking.LocalClient/InternalMsg>
struct Stack_1_t9C08B2D567DCAE884CE2FD4DE45BA3F7BD6598E4;
// System.Collections.Hashtable
struct Hashtable_t978F65B8006C8F5504B286526AEC6608FF983FC9;
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
// System.Globalization.CodePageDataItem
struct CodePageDataItem_t6E34BEE9CCCBB35C88D714664633AF6E5F5671FB;
// System.IAsyncResult
struct IAsyncResult_t8E194308510B375B42432981AE5E7488C458D598;
// System.IndexOutOfRangeException
struct IndexOutOfRangeException_tEC7665FC66525AB6A6916A7EB505E5591683F0CF;
// System.Int32[]
struct Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83;
// System.IntPtr[]
struct IntPtrU5BU5D_t4DC01DCB9A6DF6C9792A6513595D7A11E637DCDD;
// System.Net.EndPoint
struct EndPoint_tD87FCEF2780A951E8CE8D808C345FBF2C088D980;
// System.Object[]
struct ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A;
// System.Reflection.MethodInfo
struct MethodInfo_t;
// System.Runtime.Serialization.SafeSerializationManager
struct SafeSerializationManager_t4A754D86B0F784B18CBC36C073BA564BED109770;
// System.Runtime.Serialization.SerializationInfo
struct SerializationInfo_t1BB80E9C9DEA52DBF464487234B045E2930ADA26;
// System.String
struct String_t;
// System.Text.DecoderFallback
struct DecoderFallback_t128445EB7676870485230893338EF044F6B72F60;
// System.Text.EncoderFallback
struct EncoderFallback_tDE342346D01608628F1BCEBB652D31009852CF63;
// System.Text.Encoding
struct Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4;
// System.Text.UTF8Encoding
struct UTF8Encoding_t77ED103B749A387EF072C3429F48C91D12CA08DE;
// System.Type
struct Type_t;
// System.UInt32[]
struct UInt32U5BU5D_t9AA834AF2940E75BBF8E3F08FF0D20D266DB71CB;
// System.Void
struct Void_t22962CB4C05B1D89B55A6E1139F0E87A90987017;
// UnityEngine.CharacterController
struct CharacterController_t0ED98F461DBB7AC5B189C190153D83D5888BF93E;
// UnityEngine.Component
struct Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621;
// UnityEngine.GameObject
struct GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F;
// UnityEngine.Material
struct Material_tF7DB3BF0C24DEC2FE0CB51E5DF5053D5223C8598;
// UnityEngine.Networking.ChannelBuffer[]
struct ChannelBufferU5BU5D_t75CDA99AB4F27F49A1DAA287CF43B1132505E6FA;
// UnityEngine.Networking.HostTopology
struct HostTopology_tD01D253330A0DAA736EDFC67EE9585C363FA9B0E;
// UnityEngine.Networking.LocalClient
struct LocalClient_tCEC0096B13C433140FD4C09424CE345B28FE3C86;
// UnityEngine.Networking.MessageBase
struct MessageBase_t2EA42B01AD6A5F36EAF84BE623801951B9F55416;
// UnityEngine.Networking.NetBuffer
struct NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C;
// UnityEngine.Networking.NetworkBehaviour
struct NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C;
// UnityEngine.Networking.NetworkBehaviour[]
struct NetworkBehaviourU5BU5D_tA321D64478B9213228935C52651EBFA3E352C7CB;
// UnityEngine.Networking.NetworkConnection
struct NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA;
// UnityEngine.Networking.NetworkIdentity
struct NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B;
// UnityEngine.Networking.NetworkIdentity/ClientAuthorityCallback
struct ClientAuthorityCallback_tB6533BDCE069DE0B5628A9BEE08EDCC76F373644;
// UnityEngine.Networking.NetworkMessage
struct NetworkMessage_tCD66E2AE395A185EFE622EBB5497C95F6754685C;
// UnityEngine.Networking.NetworkMessageHandlers
struct NetworkMessageHandlers_tA7BB2E51BDBD8ECE976AD44F1B634F40EA9807D4;
// UnityEngine.Networking.NetworkReader
struct NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12;
// UnityEngine.Networking.NetworkScene
struct NetworkScene_t67A8AC9779C203B146A8723FA561736890CA9A40;
// UnityEngine.Networking.NetworkServer
struct NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1;
// UnityEngine.Networking.NetworkServer/ServerSimpleWrapper
struct ServerSimpleWrapper_t1ECF42A66748FA970402440F00E743DB5E2AAA32;
// UnityEngine.Networking.NetworkSystem.CRCMessage
struct CRCMessage_t7F44D52B267C35387F0D7AD0D9098D579ECF61FA;
// UnityEngine.Networking.NetworkSystem.ClientAuthorityMessage
struct ClientAuthorityMessage_t3236F2A4C2A172651CCA0E9807EA8FB14D1E5E21;
// UnityEngine.Networking.NetworkSystem.ObjectDestroyMessage
struct ObjectDestroyMessage_tDABDFFAAF87735B56D448C9DC817E73D8DF8BB07;
// UnityEngine.Networking.NetworkSystem.ObjectSpawnFinishedMessage
struct ObjectSpawnFinishedMessage_t02EF525CD1734EDEA77DA073E728E6FBD5E7C550;
// UnityEngine.Networking.NetworkSystem.ObjectSpawnMessage
struct ObjectSpawnMessage_t5BC8D432216492084C7D2DC082BB8D2A81EE9E33;
// UnityEngine.Networking.NetworkSystem.ObjectSpawnSceneMessage
struct ObjectSpawnSceneMessage_tA5AF2D7F8B73A6C29D346E11D82B0CE86F7B28FC;
// UnityEngine.Networking.NetworkSystem.OwnerMessage
struct OwnerMessage_tB0123F3077643618B07980F6ACB02D4BC4C9E887;
// UnityEngine.Networking.NetworkSystem.PeerInfoMessage[]
struct PeerInfoMessageU5BU5D_t6AD51F1C65B2BBE6A626AB37377689360E088984;
// UnityEngine.Networking.NetworkSystem.RemovePlayerMessage
struct RemovePlayerMessage_t51B0D9BCA3C2B4FD772A2972588CC0915FD4CEBF;
// UnityEngine.Networking.NetworkTransform
struct NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F;
// UnityEngine.Networking.NetworkTransform/ClientMoveCallback2D
struct ClientMoveCallback2D_tDFAD7DD6998C835AD2376F25136794AB12BA81A2;
// UnityEngine.Networking.NetworkTransform/ClientMoveCallback3D
struct ClientMoveCallback3D_t8B3ABB4B5CD7D938193C0EB61634DC9424916A45;
// UnityEngine.Networking.NetworkTransformChild
struct NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E;
// UnityEngine.Networking.NetworkTransformChild[]
struct NetworkTransformChildU5BU5D_tAE50AD052E14730045D7CF7E715FBC48ADEDB6C1;
// UnityEngine.Networking.NetworkTransformVisualizer
struct NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386;
// UnityEngine.Networking.NetworkWriter
struct NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030;
// UnityEngine.Networking.PlayerController
struct PlayerController_tAB80FD64EAB9692832107A2D107D39D078CE7C5E;
// UnityEngine.Networking.ServerAttribute
struct ServerAttribute_tBEAD82CF18B52F903FB105CC54E39C66B82E079D;
// UnityEngine.Networking.ServerCallbackAttribute
struct ServerCallbackAttribute_tD2D226910AED65FFCB395293D6A4235FE08BCF0F;
// UnityEngine.Networking.SpawnDelegate
struct SpawnDelegate_t4CB00A9006B512E467753C6CC752E29FA2EBC87F;
// UnityEngine.Networking.SyncEventAttribute
struct SyncEventAttribute_t32B6E9C1595BB49337BC42619BB697C84790630E;
// UnityEngine.Networking.SyncListBool
struct SyncListBool_t4530597403BBB668F776B32DE46A1A91623EE054;
// UnityEngine.Networking.SyncListFloat
struct SyncListFloat_tC8F12C17B783518D34953712B51249276C506922;
// UnityEngine.Networking.SyncListInt
struct SyncListInt_t6D5125D26D629A9DB1325266BCFCDF2FC86FD9C6;
// UnityEngine.Networking.SyncListString
struct SyncListString_t26D5186F91FB985D01BDC6CC5B4C7C13FA3740CD;
// UnityEngine.Networking.SyncListUInt
struct SyncListUInt_tF223A88F804D7F2819F5F610669176CE6E93A0E6;
// UnityEngine.Networking.SyncList`1/SyncListChanged<System.Boolean>
struct SyncListChanged_t5156E5B2411DE07D5AEA8F98B87E4FBD1E626D6E;
// UnityEngine.Networking.SyncList`1/SyncListChanged<System.Int32>
struct SyncListChanged_t1A1EBC018732CBF3B1F86CE21BE59B5107844339;
// UnityEngine.Networking.SyncList`1/SyncListChanged<System.Single>
struct SyncListChanged_t63BE6CD22C2B89F865722BF980F4351DF0EC68D6;
// UnityEngine.Networking.SyncList`1/SyncListChanged<System.String>
struct SyncListChanged_tF0BC42132992DEBFE9981ED50FE321BAA43CA3B5;
// UnityEngine.Networking.SyncList`1/SyncListChanged<System.UInt32>
struct SyncListChanged_t1804C1A09093327076629C515EC006A30F38C1B8;
// UnityEngine.Networking.SyncList`1<System.Boolean>
struct SyncList_1_tDDF00E08E649A86264E50205CB99495D1AD2E8D0;
// UnityEngine.Networking.SyncList`1<System.Int32>
struct SyncList_1_t8595ACD08C8686AC9547A225ACF209C171FF0665;
// UnityEngine.Networking.SyncList`1<System.Object>
struct SyncList_1_tDC9BD47B0C55962FA07DEC77A578A1F5231B0238;
// UnityEngine.Networking.SyncList`1<System.Single>
struct SyncList_1_tDEB03E3C5252571915662095C7060998910FD0A2;
// UnityEngine.Networking.SyncList`1<System.String>
struct SyncList_1_t4C2B8FAE3D901E39D62A3678DF05B8473364ED04;
// UnityEngine.Networking.SyncList`1<System.UInt32>
struct SyncList_1_t6931FBA5633802C1CAE04093D0C6D9C88EE66627;
// UnityEngine.Networking.SyncVarAttribute
struct SyncVarAttribute_tD57FE395DED8D547F0200B7F50F36DFA27C6BF3A;
// UnityEngine.Networking.TargetRpcAttribute
struct TargetRpcAttribute_t7B515CB5DD6D609483DFC4ACC89D00B00C9EAE03;
// UnityEngine.Networking.ULocalConnectionToClient
struct ULocalConnectionToClient_t7AF7EBF2BEC3714F75EF894035BFAE9E6F9561A8;
// UnityEngine.Networking.ULocalConnectionToServer
struct ULocalConnectionToServer_tE6E34057F329C3E0E703C6F095DF82B0270557B8;
// UnityEngine.Networking.UnSpawnDelegate
struct UnSpawnDelegate_tDC1AD5AA3602EB703F4FA34792B4D4075582AE19;
// UnityEngine.Object
struct Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0;
// UnityEngine.Rigidbody
struct Rigidbody_tE0A58EE5A1F7DC908EFFB4F0D795AC9552A750A5;
// UnityEngine.Rigidbody2D
struct Rigidbody2D_tBDC6900A76D3C47E291446FF008D02B817C81CDE;
// UnityEngine.Shader
struct Shader_tE2731FF351B74AB4186897484FB01E000C1160CA;
// UnityEngine.Transform
struct Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA;

IL2CPP_EXTERN_C RuntimeClass* ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* ClientScene_t0A10B1F436A5AA8D5FC9B18C9ED0B32008809A3E_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IndexOutOfRangeException_tEC7665FC66525AB6A6916A7EB505E5591683F0CF_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Int16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* LogFilter_t5202A297E770086F7954B8D6703BAC03C22654ED_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Material_tF7DB3BF0C24DEC2FE0CB51E5DF5053D5223C8598_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Single_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SyncListBool_t4530597403BBB668F776B32DE46A1A91623EE054_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SyncListFloat_tC8F12C17B783518D34953712B51249276C506922_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SyncListInt_t6D5125D26D629A9DB1325266BCFCDF2FC86FD9C6_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SyncListString_t26D5186F91FB985D01BDC6CC5B4C7C13FA3740CD_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* SyncListUInt_tF223A88F804D7F2819F5F610669176CE6E93A0E6_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* UTF8Encoding_t77ED103B749A387EF072C3429F48C91D12CA08DE_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C String_t* _stringLiteral0C84AC39FC120C1E579C09FEAA062CA04994E08F;
IL2CPP_EXTERN_C String_t* _stringLiteral13F4B303180B12CAF8F069E93D7C4FAF969D3BC4;
IL2CPP_EXTERN_C String_t* _stringLiteral1CDFEAB29C4AD9DAC96B9B86A0440B7DCCACBA06;
IL2CPP_EXTERN_C String_t* _stringLiteral1FB72BA5614BA625FBF384ACEA5077F842DEAC45;
IL2CPP_EXTERN_C String_t* _stringLiteral23DD7FD333862ABC4A00FCC16019AD77994EB92C;
IL2CPP_EXTERN_C String_t* _stringLiteral2BE88CA4242C76E8253AC62474851065032D6833;
IL2CPP_EXTERN_C String_t* _stringLiteral2DECCA3D4BB2505D46E36DBC5737FDD9004B2564;
IL2CPP_EXTERN_C String_t* _stringLiteral31DDD762A7C4C86F022297D9A759C2F1C7D04EAC;
IL2CPP_EXTERN_C String_t* _stringLiteral43AB5B0D093A407E1568E3E17AAF14DC10D1DE88;
IL2CPP_EXTERN_C String_t* _stringLiteral4C3023713E64D2BEA428CD3A86A6DE84754220DE;
IL2CPP_EXTERN_C String_t* _stringLiteral5902A7A78A53DCB1D1016E9500F2A3343AA637C9;
IL2CPP_EXTERN_C String_t* _stringLiteral5A7F8C5B28DA7B5648FACC6874DF9F2B9D9823C9;
IL2CPP_EXTERN_C String_t* _stringLiteral8A90E4187AA462FD7BCD9E2521B1C4F09372DBAF;
IL2CPP_EXTERN_C String_t* _stringLiteral8FBB472DE655009001FA670F6146ACB177ACBCDC;
IL2CPP_EXTERN_C String_t* _stringLiteral994F759C33CC2E33C8D1EA34D1B4D05FF92E9F57;
IL2CPP_EXTERN_C String_t* _stringLiteralA45E7DA12B87A6DBEDA68DC73471E87E66E9C2E1;
IL2CPP_EXTERN_C String_t* _stringLiteralBF23D347C24FDA3FCBB189660BEF497CF90D2A71;
IL2CPP_EXTERN_C String_t* _stringLiteralCDC3766C1C256EF634E93723D6A50C0DDD213BC4;
IL2CPP_EXTERN_C String_t* _stringLiteralD19C1CDB8E147A59F990BE7BC80967AB61A50F80;
IL2CPP_EXTERN_C String_t* _stringLiteralD48C67736A90281297DD96BF118099E6CB6939B8;
IL2CPP_EXTERN_C String_t* _stringLiteralD4C310BD45DF218471473B38695DF6F80D91CA0F;
IL2CPP_EXTERN_C String_t* _stringLiteralD57E846CA38774E0B2EFB225D0914EE255C71B37;
IL2CPP_EXTERN_C String_t* _stringLiteralDC5AFF0CACCF4051C4542D7C075A52E0451162CF;
IL2CPP_EXTERN_C String_t* _stringLiteralF11AF337B3340D92B47E93D08CB0B65A6AE686F5;
IL2CPP_EXTERN_C String_t* _stringLiteralF344BFD3902B3288B6186257324DCFF0BBD317C1;
IL2CPP_EXTERN_C const RuntimeMethod* ArraySegment_1_get_Array_m41D93EFB7EAB3081C0A27ED9891E7177F5F361B7_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* ArraySegment_1_get_Count_m02387DADA172F909FD346559D93990E990E05352_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Component_GetComponent_TisNetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F_mC1DB4A13BBC41101231C90CD393292630350975B_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Component_GetComponents_TisNetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E_mD3E94B5EC8B4D6678D6CE5FDFBA6502236E701C0_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* GameObject_GetComponent_TisNetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B_m818B3B379B25E13EF0599E7709067A3E3F4B50FD_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* GameObject_GetComponent_TisNetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F_mB885510CB2C4A1A57D2A42B4AE68A09AAA1DD79A_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* GameObject_GetComponent_TisRigidbody2D_tBDC6900A76D3C47E291446FF008D02B817C81CDE_mDDB82F02C3053DCC0D60C420752A11EC11CBACC0_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* GameObject_GetComponent_TisRigidbody_tE0A58EE5A1F7DC908EFFB4F0D795AC9552A750A5_m31F97A6E057858450728C32EE09647374FA10903_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* GameObject_GetComponents_TisNetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E_mAE4B663ABA411E0016C4E260112D19FB99B8889F_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* HashSet_1_Contains_m68D1EC086CFCC7E6FBE6B1C66DDFF3D1DC62695C_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* NetworkWriter_Write_m856F6DD1E132E2C68BA9D7D36A5ED5EAA1D108F4_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Object_Instantiate_TisGameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F_m4F397BCC6697902B40033E61129D4EA6FE93570F_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1_AddInternal_m02EF37FDD57B236ED985C01D53E7E181843A33D8_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1_AddInternal_m84938C896AA1F3EED3568ECB90FED244DF2617B2_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1_AddInternal_m93CDCB4D3061B2F4CF88B74DEABE1C06D4AED23C_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1_AddInternal_m977B3CE5458FB772939C4CDB6612918FFC0BD427_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1_AddInternal_mC17F547D0099E43ACAA4C5FD21D63DDE456602A6_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1_Clear_m00C3496EAD8E618F4C20CA6F618373D4564CEB58_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1_Clear_m13160DF80DA71AAF005006E14C5C8985DBF15EB5_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1_Clear_mAF7EFFA62345875E1C183F7D3A09A57A0E05E97B_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1_Clear_mC367BED8954C65BFA956C2A66885A8FA241443E0_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1_Clear_mF8FAE0172014F355D0C66600D8607442BF9A03B3_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1__ctor_m1BB28896D4C843EEF83232CE6648F916429D54E3_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1__ctor_m6E3A6F39EE2A332965D0B912FD1662297B44B901_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1__ctor_mACF8E6F1689E85F8D9D88F6B2366C1A08D6F853E_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1__ctor_mBBB1AF24E09B273530603FA90034B2B830E2460C_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1__ctor_mF18B74E2EF8296E263BCEBAB8C8DE0EA78F8BAFC_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1_get_Count_m29E32BA907E6C50793D6A2D30D22A8D052A978B8_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1_get_Count_m641E2517509914AAC0415508A728F40A914318C4_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1_get_Count_m7E687EFF75167B5EB639F273102ED345B8CB905B_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1_get_Count_m9EBDDB18AA65B4522E066D29FE2ECD9980BDEAD9_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1_get_Count_mCC0838D9ED25E463384E4852839E47B100C99577_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1_get_Item_m0578989F729AF1CD8C5F378289B5DF1FA830AE16_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1_get_Item_m0EEA26E6C3ED4695254E4D9AC8243023AE227A48_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1_get_Item_m70C832E1FED3E2D52297C7B6EF187700309BF7D4_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1_get_Item_mA89484861CD0098C5FC7466F93F18C4EE231C55F_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* SyncList_1_get_Item_mC1369C43D41DC4C7863526B187E820DD7DA3709D_RuntimeMethod_var;
IL2CPP_EXTERN_C const uint32_t ClientMoveCallback2D_BeginInvoke_m50707DD51E2F1CD304B24C934B4C8193E503F56F_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t ClientMoveCallback3D_BeginInvoke_m8122BB9D2196D4763C0E15CB1838D74254B04256_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t ClientScene_get_readyConnection_mACB67AD0151B2507CF8BD5D7D8B806C470E49998com_unity_multiplayerU2Dhlapi_Runtime3_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkClient_get_active_m31953DC487641BC5D9BEB0EB4DE32462AC4A8BD1com_unity_multiplayerU2Dhlapi_Runtime3_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkServer_get_active_m3FAC75ABF32D586F6C8DB6B4237DC40300FB2257com_unity_multiplayerU2Dhlapi_Runtime3_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkTransformChild_Awake_m382F64641C54A3131E37EA1C6959C0A3B757A46B_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkTransformChild_FixedUpdateClient_mA050F9F38ED8D506D572DC21814309EDBF982B99_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkTransformChild_FixedUpdateServer_mC12D2E9DD8B1AE9D8222581DAE9F972F29102FE2_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkTransformChild_HandleChildTransform_m786648BD699A0A3BF0CCE37E18A16A8FB5673269_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkTransformChild_HasMoved_m6B10F1CD5A72301C5E26797F4105EAE2693971B4_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkTransformChild_OnDeserialize_mE19F3EC01F32BFB2BB0D5DAF9FD97564F1935891_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkTransformChild_OnValidate_mB8E287CF434D44F97FCBF26CE7D72BD84EA592CF_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkTransformChild_SendTransform_m5E5A962A5EB1E18C5C6175873A55AE8E73C53AD9_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkTransformChild_UnserializeModeTransform_mBD3AB1CDF0F5F3D5FEA1AB49C0C53F7B24A62B39_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkTransformChild_Update_m43F18B19328E7508C0AD3A754AFBB154617D8F6F_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkTransformChild__ctor_m45FC783409BB7C8ACF58B05DCC2C6EB5C46B966F_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkTransformVisualizer_CreateLineMaterial_m7939398CB6B61BFD7D237E3685D50000C7A41B89_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkTransformVisualizer_DrawRotationInterpolation_m61EA01F463B4524948B44B9142347C44B9C5A0B0_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkTransformVisualizer_FixedUpdate_m4E2CC9B289A2C7C9CBF8E6DDBACD9C717454A8ED_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkTransformVisualizer_OnDestroy_m928CDE56238148D2AB9AEE53A1B536E8B4C475E8_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkTransformVisualizer_OnRenderObject_m9DCA4436C234C4B2EBEB4CECEC40B4E628EAF12F_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkTransformVisualizer_OnStartClient_m6C93CA459FFB3A1E142B8EDDE4F50E3B1184F66F_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkTransformVisualizer_OnStartLocalPlayer_m0F01BE0FFDFC5EE5BE5237CF0DD1DC13689786AA_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkTransformVisualizer__ctor_mF6B5C3D5D9432CDA3B546D0261830BCFB8C6909A_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkWriter_AsArray_mE90AC762796F17DD398523A8C230DD9B2E2373D5_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkWriter_ToArray_mA27B02013E80F7673A07A60EE0DE2A657F9E05A8_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkWriter_WriteBytesAndSize_mC601A1BBC88D92522B7997698041ECEDF895A71E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkWriter_WriteBytesFull_m99C689920FA25E82972668235E7C776B78D1217E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkWriter_Write_m32B91CD67215B848269156BE15AC04FB5834F0C6_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkWriter_Write_m474C374D05DAAE6A0A4111512CA3B9E821A0A3EE_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkWriter_Write_m77E79DF810798F97CB4B0193DEDE1D7EAED9E176_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkWriter_Write_m856F6DD1E132E2C68BA9D7D36A5ED5EAA1D108F4_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkWriter_Write_m909A09F9662D8CB46D80D6155630A321EFED99E1_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkWriter_Write_mA29B40D65C79F09452E44ACE95D7882D48BC2631_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkWriter_Write_mAEB6BA4ED3581931DF47C9C32756693014CEB796_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkWriter_Write_mD3CD61845FC639033FBD63A6AC70260B113A0C7E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkWriter__ctor_m43E453A4A5244815EC8D906B22E5D85FB7535D33_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t NetworkWriter__ctor_m99A86656D77FE374861A287BBA85CD63C26FB6FC_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t PlayerController_ToString_m1E3830029B488BE2089DF2630ED9661C22649F19_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t PlayerController__ctor_m9FF8174A299E9211F8D4EAAF8BA1C7EFE098D29C_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SpawnDelegate_BeginInvoke_m2C90BEE3D708A1F55BFF6E06B0D2DB011BE44DF3_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SyncListBool_ReadInstance_mA961F477308AD365BA3F7140CA4ABFC60CBE16F4_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SyncListBool_ReadReference_m80A4B63470AE55EC05DF055CCE6C6D1C9CD0DA16_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SyncListBool_WriteInstance_m5A8501D2567B9E44E86A0C48E390EE625F7E98CA_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SyncListBool__ctor_m2BF7E2F5C16E5798F3DAD0B7C75DC606B00FF94C_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SyncListFloat_ReadInstance_mB2A1ABC0F72AE1A31BC36DF6F23EAF05962639F1_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SyncListFloat_ReadReference_m54B4A4D3721639E3021DA90C770CFED61A61377E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SyncListFloat_WriteInstance_m3309F0578D122C45D2F3881C383D53626DF1986E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SyncListFloat__ctor_m68F03DF4317EADAA861FA0D251C797FD7CFA28ED_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SyncListInt_ReadInstance_m03CA3005B2153B13442ABF9CFB75B8D8B1B8A7A0_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SyncListInt_ReadReference_m3CC83A8EB36BA8DC1874FC2B781DED8877EBD188_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SyncListInt_WriteInstance_m5C61B3494D72E98F5D6A03F7917F8553CD5CFAF4_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SyncListInt__ctor_m9A8426FDD81908FDA8B94E67751AB67D0C52D90A_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SyncListString_ReadInstance_m2EC7D4993F62FF87C40BB3112B04D2DA4D02C079_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SyncListString_ReadReference_m0900B20871BBE6147CF5EA035D3BD48A8415AAC4_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SyncListString_WriteInstance_m6FCCCB6C180BB58E59A883B568906A4072EBB2F2_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SyncListString__ctor_m50D229AE4F36D878B3FBB78517104B1D34BA3F38_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SyncListUInt_ReadInstance_m8477E9A83BCCED58B06B8AEF6CBFEA2BFE25292E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SyncListUInt_ReadReference_m3CDD33D651FD933EBDCFA62C19D3C37C268BB18A_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SyncListUInt_WriteInstance_m2C862802FBB8C20BE3E0B06434F89BF457887C9E_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t SyncListUInt__ctor_mBAD30E72F2FB4BFA239B5DDABCCFC0DEEFD918AC_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t ULocalConnectionToClient__ctor_mA475E50C32BC0BDEF1B6B574226B539007AA56ED_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t ULocalConnectionToServer_SendBytes_mB46A168719C69599AC91617C61D15D495CE498E6_MetadataUsageId;
IL2CPP_EXTERN_C const uint32_t ULocalConnectionToServer__ctor_mC9A4D762519369638D6A6ED1A27C077D17E36CFA_MetadataUsageId;
struct Delegate_t_marshaled_com;
struct Delegate_t_marshaled_pinvoke;
struct Exception_t_marshaled_com;
struct Exception_t_marshaled_pinvoke;

struct ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821;
struct DelegateU5BU5D_tDFCDEE2A6322F96C0FE49AF47E9ADB8C4B294E86;
struct Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83;
struct ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A;
struct NetworkTransformChildU5BU5D_tAE50AD052E14730045D7CF7E715FBC48ADEDB6C1;

IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif

// System.Object

struct Il2CppArrayBounds;

// System.Array


// System.Attribute
struct  Attribute_tF048C13FB3C8CFCC53F82290E4A3F621089F9A74  : public RuntimeObject
{
public:

public:
};


// System.Collections.Generic.HashSet`1<UnityEngine.Networking.NetworkInstanceId>
struct  HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990  : public RuntimeObject
{
public:
	// System.Int32[] System.Collections.Generic.HashSet`1::_buckets
	Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* ____buckets_7;
	// System.Collections.Generic.HashSet`1_Slot<T>[] System.Collections.Generic.HashSet`1::_slots
	SlotU5BU5D_t971A4EBC1B2F2C5607B8B63726102B5989FF8B4A* ____slots_8;
	// System.Int32 System.Collections.Generic.HashSet`1::_count
	int32_t ____count_9;
	// System.Int32 System.Collections.Generic.HashSet`1::_lastIndex
	int32_t ____lastIndex_10;
	// System.Int32 System.Collections.Generic.HashSet`1::_freeList
	int32_t ____freeList_11;
	// System.Collections.Generic.IEqualityComparer`1<T> System.Collections.Generic.HashSet`1::_comparer
	RuntimeObject* ____comparer_12;
	// System.Int32 System.Collections.Generic.HashSet`1::_version
	int32_t ____version_13;
	// System.Runtime.Serialization.SerializationInfo System.Collections.Generic.HashSet`1::_siInfo
	SerializationInfo_t1BB80E9C9DEA52DBF464487234B045E2930ADA26 * ____siInfo_14;

public:
	inline static int32_t get_offset_of__buckets_7() { return static_cast<int32_t>(offsetof(HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990, ____buckets_7)); }
	inline Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* get__buckets_7() const { return ____buckets_7; }
	inline Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83** get_address_of__buckets_7() { return &____buckets_7; }
	inline void set__buckets_7(Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* value)
	{
		____buckets_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____buckets_7), (void*)value);
	}

	inline static int32_t get_offset_of__slots_8() { return static_cast<int32_t>(offsetof(HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990, ____slots_8)); }
	inline SlotU5BU5D_t971A4EBC1B2F2C5607B8B63726102B5989FF8B4A* get__slots_8() const { return ____slots_8; }
	inline SlotU5BU5D_t971A4EBC1B2F2C5607B8B63726102B5989FF8B4A** get_address_of__slots_8() { return &____slots_8; }
	inline void set__slots_8(SlotU5BU5D_t971A4EBC1B2F2C5607B8B63726102B5989FF8B4A* value)
	{
		____slots_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____slots_8), (void*)value);
	}

	inline static int32_t get_offset_of__count_9() { return static_cast<int32_t>(offsetof(HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990, ____count_9)); }
	inline int32_t get__count_9() const { return ____count_9; }
	inline int32_t* get_address_of__count_9() { return &____count_9; }
	inline void set__count_9(int32_t value)
	{
		____count_9 = value;
	}

	inline static int32_t get_offset_of__lastIndex_10() { return static_cast<int32_t>(offsetof(HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990, ____lastIndex_10)); }
	inline int32_t get__lastIndex_10() const { return ____lastIndex_10; }
	inline int32_t* get_address_of__lastIndex_10() { return &____lastIndex_10; }
	inline void set__lastIndex_10(int32_t value)
	{
		____lastIndex_10 = value;
	}

	inline static int32_t get_offset_of__freeList_11() { return static_cast<int32_t>(offsetof(HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990, ____freeList_11)); }
	inline int32_t get__freeList_11() const { return ____freeList_11; }
	inline int32_t* get_address_of__freeList_11() { return &____freeList_11; }
	inline void set__freeList_11(int32_t value)
	{
		____freeList_11 = value;
	}

	inline static int32_t get_offset_of__comparer_12() { return static_cast<int32_t>(offsetof(HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990, ____comparer_12)); }
	inline RuntimeObject* get__comparer_12() const { return ____comparer_12; }
	inline RuntimeObject** get_address_of__comparer_12() { return &____comparer_12; }
	inline void set__comparer_12(RuntimeObject* value)
	{
		____comparer_12 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____comparer_12), (void*)value);
	}

	inline static int32_t get_offset_of__version_13() { return static_cast<int32_t>(offsetof(HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990, ____version_13)); }
	inline int32_t get__version_13() const { return ____version_13; }
	inline int32_t* get_address_of__version_13() { return &____version_13; }
	inline void set__version_13(int32_t value)
	{
		____version_13 = value;
	}

	inline static int32_t get_offset_of__siInfo_14() { return static_cast<int32_t>(offsetof(HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990, ____siInfo_14)); }
	inline SerializationInfo_t1BB80E9C9DEA52DBF464487234B045E2930ADA26 * get__siInfo_14() const { return ____siInfo_14; }
	inline SerializationInfo_t1BB80E9C9DEA52DBF464487234B045E2930ADA26 ** get_address_of__siInfo_14() { return &____siInfo_14; }
	inline void set__siInfo_14(SerializationInfo_t1BB80E9C9DEA52DBF464487234B045E2930ADA26 * value)
	{
		____siInfo_14 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____siInfo_14), (void*)value);
	}
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


// System.Text.Encoding
struct  Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4  : public RuntimeObject
{
public:
	// System.Int32 System.Text.Encoding::m_codePage
	int32_t ___m_codePage_55;
	// System.Globalization.CodePageDataItem System.Text.Encoding::dataItem
	CodePageDataItem_t6E34BEE9CCCBB35C88D714664633AF6E5F5671FB * ___dataItem_56;
	// System.Boolean System.Text.Encoding::m_deserializedFromEverett
	bool ___m_deserializedFromEverett_57;
	// System.Boolean System.Text.Encoding::m_isReadOnly
	bool ___m_isReadOnly_58;
	// System.Text.EncoderFallback System.Text.Encoding::encoderFallback
	EncoderFallback_tDE342346D01608628F1BCEBB652D31009852CF63 * ___encoderFallback_59;
	// System.Text.DecoderFallback System.Text.Encoding::decoderFallback
	DecoderFallback_t128445EB7676870485230893338EF044F6B72F60 * ___decoderFallback_60;

public:
	inline static int32_t get_offset_of_m_codePage_55() { return static_cast<int32_t>(offsetof(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4, ___m_codePage_55)); }
	inline int32_t get_m_codePage_55() const { return ___m_codePage_55; }
	inline int32_t* get_address_of_m_codePage_55() { return &___m_codePage_55; }
	inline void set_m_codePage_55(int32_t value)
	{
		___m_codePage_55 = value;
	}

	inline static int32_t get_offset_of_dataItem_56() { return static_cast<int32_t>(offsetof(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4, ___dataItem_56)); }
	inline CodePageDataItem_t6E34BEE9CCCBB35C88D714664633AF6E5F5671FB * get_dataItem_56() const { return ___dataItem_56; }
	inline CodePageDataItem_t6E34BEE9CCCBB35C88D714664633AF6E5F5671FB ** get_address_of_dataItem_56() { return &___dataItem_56; }
	inline void set_dataItem_56(CodePageDataItem_t6E34BEE9CCCBB35C88D714664633AF6E5F5671FB * value)
	{
		___dataItem_56 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___dataItem_56), (void*)value);
	}

	inline static int32_t get_offset_of_m_deserializedFromEverett_57() { return static_cast<int32_t>(offsetof(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4, ___m_deserializedFromEverett_57)); }
	inline bool get_m_deserializedFromEverett_57() const { return ___m_deserializedFromEverett_57; }
	inline bool* get_address_of_m_deserializedFromEverett_57() { return &___m_deserializedFromEverett_57; }
	inline void set_m_deserializedFromEverett_57(bool value)
	{
		___m_deserializedFromEverett_57 = value;
	}

	inline static int32_t get_offset_of_m_isReadOnly_58() { return static_cast<int32_t>(offsetof(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4, ___m_isReadOnly_58)); }
	inline bool get_m_isReadOnly_58() const { return ___m_isReadOnly_58; }
	inline bool* get_address_of_m_isReadOnly_58() { return &___m_isReadOnly_58; }
	inline void set_m_isReadOnly_58(bool value)
	{
		___m_isReadOnly_58 = value;
	}

	inline static int32_t get_offset_of_encoderFallback_59() { return static_cast<int32_t>(offsetof(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4, ___encoderFallback_59)); }
	inline EncoderFallback_tDE342346D01608628F1BCEBB652D31009852CF63 * get_encoderFallback_59() const { return ___encoderFallback_59; }
	inline EncoderFallback_tDE342346D01608628F1BCEBB652D31009852CF63 ** get_address_of_encoderFallback_59() { return &___encoderFallback_59; }
	inline void set_encoderFallback_59(EncoderFallback_tDE342346D01608628F1BCEBB652D31009852CF63 * value)
	{
		___encoderFallback_59 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___encoderFallback_59), (void*)value);
	}

	inline static int32_t get_offset_of_decoderFallback_60() { return static_cast<int32_t>(offsetof(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4, ___decoderFallback_60)); }
	inline DecoderFallback_t128445EB7676870485230893338EF044F6B72F60 * get_decoderFallback_60() const { return ___decoderFallback_60; }
	inline DecoderFallback_t128445EB7676870485230893338EF044F6B72F60 ** get_address_of_decoderFallback_60() { return &___decoderFallback_60; }
	inline void set_decoderFallback_60(DecoderFallback_t128445EB7676870485230893338EF044F6B72F60 * value)
	{
		___decoderFallback_60 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___decoderFallback_60), (void*)value);
	}
};

struct Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4_StaticFields
{
public:
	// System.Text.Encoding modreq(System.Runtime.CompilerServices.IsVolatile) System.Text.Encoding::defaultEncoding
	Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * ___defaultEncoding_0;
	// System.Text.Encoding modreq(System.Runtime.CompilerServices.IsVolatile) System.Text.Encoding::unicodeEncoding
	Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * ___unicodeEncoding_1;
	// System.Text.Encoding modreq(System.Runtime.CompilerServices.IsVolatile) System.Text.Encoding::bigEndianUnicode
	Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * ___bigEndianUnicode_2;
	// System.Text.Encoding modreq(System.Runtime.CompilerServices.IsVolatile) System.Text.Encoding::utf7Encoding
	Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * ___utf7Encoding_3;
	// System.Text.Encoding modreq(System.Runtime.CompilerServices.IsVolatile) System.Text.Encoding::utf8Encoding
	Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * ___utf8Encoding_4;
	// System.Text.Encoding modreq(System.Runtime.CompilerServices.IsVolatile) System.Text.Encoding::utf32Encoding
	Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * ___utf32Encoding_5;
	// System.Text.Encoding modreq(System.Runtime.CompilerServices.IsVolatile) System.Text.Encoding::asciiEncoding
	Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * ___asciiEncoding_6;
	// System.Text.Encoding modreq(System.Runtime.CompilerServices.IsVolatile) System.Text.Encoding::latin1Encoding
	Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * ___latin1Encoding_7;
	// System.Collections.Hashtable modreq(System.Runtime.CompilerServices.IsVolatile) System.Text.Encoding::encodings
	Hashtable_t978F65B8006C8F5504B286526AEC6608FF983FC9 * ___encodings_8;
	// System.Object System.Text.Encoding::s_InternalSyncObject
	RuntimeObject * ___s_InternalSyncObject_61;

public:
	inline static int32_t get_offset_of_defaultEncoding_0() { return static_cast<int32_t>(offsetof(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4_StaticFields, ___defaultEncoding_0)); }
	inline Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * get_defaultEncoding_0() const { return ___defaultEncoding_0; }
	inline Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 ** get_address_of_defaultEncoding_0() { return &___defaultEncoding_0; }
	inline void set_defaultEncoding_0(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * value)
	{
		___defaultEncoding_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___defaultEncoding_0), (void*)value);
	}

	inline static int32_t get_offset_of_unicodeEncoding_1() { return static_cast<int32_t>(offsetof(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4_StaticFields, ___unicodeEncoding_1)); }
	inline Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * get_unicodeEncoding_1() const { return ___unicodeEncoding_1; }
	inline Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 ** get_address_of_unicodeEncoding_1() { return &___unicodeEncoding_1; }
	inline void set_unicodeEncoding_1(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * value)
	{
		___unicodeEncoding_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___unicodeEncoding_1), (void*)value);
	}

	inline static int32_t get_offset_of_bigEndianUnicode_2() { return static_cast<int32_t>(offsetof(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4_StaticFields, ___bigEndianUnicode_2)); }
	inline Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * get_bigEndianUnicode_2() const { return ___bigEndianUnicode_2; }
	inline Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 ** get_address_of_bigEndianUnicode_2() { return &___bigEndianUnicode_2; }
	inline void set_bigEndianUnicode_2(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * value)
	{
		___bigEndianUnicode_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___bigEndianUnicode_2), (void*)value);
	}

	inline static int32_t get_offset_of_utf7Encoding_3() { return static_cast<int32_t>(offsetof(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4_StaticFields, ___utf7Encoding_3)); }
	inline Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * get_utf7Encoding_3() const { return ___utf7Encoding_3; }
	inline Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 ** get_address_of_utf7Encoding_3() { return &___utf7Encoding_3; }
	inline void set_utf7Encoding_3(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * value)
	{
		___utf7Encoding_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___utf7Encoding_3), (void*)value);
	}

	inline static int32_t get_offset_of_utf8Encoding_4() { return static_cast<int32_t>(offsetof(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4_StaticFields, ___utf8Encoding_4)); }
	inline Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * get_utf8Encoding_4() const { return ___utf8Encoding_4; }
	inline Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 ** get_address_of_utf8Encoding_4() { return &___utf8Encoding_4; }
	inline void set_utf8Encoding_4(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * value)
	{
		___utf8Encoding_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___utf8Encoding_4), (void*)value);
	}

	inline static int32_t get_offset_of_utf32Encoding_5() { return static_cast<int32_t>(offsetof(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4_StaticFields, ___utf32Encoding_5)); }
	inline Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * get_utf32Encoding_5() const { return ___utf32Encoding_5; }
	inline Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 ** get_address_of_utf32Encoding_5() { return &___utf32Encoding_5; }
	inline void set_utf32Encoding_5(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * value)
	{
		___utf32Encoding_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___utf32Encoding_5), (void*)value);
	}

	inline static int32_t get_offset_of_asciiEncoding_6() { return static_cast<int32_t>(offsetof(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4_StaticFields, ___asciiEncoding_6)); }
	inline Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * get_asciiEncoding_6() const { return ___asciiEncoding_6; }
	inline Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 ** get_address_of_asciiEncoding_6() { return &___asciiEncoding_6; }
	inline void set_asciiEncoding_6(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * value)
	{
		___asciiEncoding_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___asciiEncoding_6), (void*)value);
	}

	inline static int32_t get_offset_of_latin1Encoding_7() { return static_cast<int32_t>(offsetof(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4_StaticFields, ___latin1Encoding_7)); }
	inline Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * get_latin1Encoding_7() const { return ___latin1Encoding_7; }
	inline Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 ** get_address_of_latin1Encoding_7() { return &___latin1Encoding_7; }
	inline void set_latin1Encoding_7(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * value)
	{
		___latin1Encoding_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___latin1Encoding_7), (void*)value);
	}

	inline static int32_t get_offset_of_encodings_8() { return static_cast<int32_t>(offsetof(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4_StaticFields, ___encodings_8)); }
	inline Hashtable_t978F65B8006C8F5504B286526AEC6608FF983FC9 * get_encodings_8() const { return ___encodings_8; }
	inline Hashtable_t978F65B8006C8F5504B286526AEC6608FF983FC9 ** get_address_of_encodings_8() { return &___encodings_8; }
	inline void set_encodings_8(Hashtable_t978F65B8006C8F5504B286526AEC6608FF983FC9 * value)
	{
		___encodings_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___encodings_8), (void*)value);
	}

	inline static int32_t get_offset_of_s_InternalSyncObject_61() { return static_cast<int32_t>(offsetof(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4_StaticFields, ___s_InternalSyncObject_61)); }
	inline RuntimeObject * get_s_InternalSyncObject_61() const { return ___s_InternalSyncObject_61; }
	inline RuntimeObject ** get_address_of_s_InternalSyncObject_61() { return &___s_InternalSyncObject_61; }
	inline void set_s_InternalSyncObject_61(RuntimeObject * value)
	{
		___s_InternalSyncObject_61 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_InternalSyncObject_61), (void*)value);
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

// UnityEngine.Networking.ClientScene
struct  ClientScene_t0A10B1F436A5AA8D5FC9B18C9ED0B32008809A3E  : public RuntimeObject
{
public:

public:
};

struct ClientScene_t0A10B1F436A5AA8D5FC9B18C9ED0B32008809A3E_StaticFields
{
public:
	// System.Collections.Generic.List`1<UnityEngine.Networking.PlayerController> UnityEngine.Networking.ClientScene::s_LocalPlayers
	List_1_t44D1B61364FCFEF62067A4726A735856DFDFD545 * ___s_LocalPlayers_0;
	// UnityEngine.Networking.NetworkConnection UnityEngine.Networking.ClientScene::s_ReadyConnection
	NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * ___s_ReadyConnection_1;
	// System.Collections.Generic.Dictionary`2<UnityEngine.Networking.NetworkSceneId,UnityEngine.Networking.NetworkIdentity> UnityEngine.Networking.ClientScene::s_SpawnableObjects
	Dictionary_2_t7BABA42F397000124B62D0DE8AC6226B5276B2CA * ___s_SpawnableObjects_2;
	// System.Boolean UnityEngine.Networking.ClientScene::s_IsReady
	bool ___s_IsReady_3;
	// System.Boolean UnityEngine.Networking.ClientScene::s_IsSpawnFinished
	bool ___s_IsSpawnFinished_4;
	// UnityEngine.Networking.NetworkScene UnityEngine.Networking.ClientScene::s_NetworkScene
	NetworkScene_t67A8AC9779C203B146A8723FA561736890CA9A40 * ___s_NetworkScene_5;
	// UnityEngine.Networking.NetworkSystem.ObjectSpawnSceneMessage UnityEngine.Networking.ClientScene::s_ObjectSpawnSceneMessage
	ObjectSpawnSceneMessage_tA5AF2D7F8B73A6C29D346E11D82B0CE86F7B28FC * ___s_ObjectSpawnSceneMessage_6;
	// UnityEngine.Networking.NetworkSystem.ObjectSpawnFinishedMessage UnityEngine.Networking.ClientScene::s_ObjectSpawnFinishedMessage
	ObjectSpawnFinishedMessage_t02EF525CD1734EDEA77DA073E728E6FBD5E7C550 * ___s_ObjectSpawnFinishedMessage_7;
	// UnityEngine.Networking.NetworkSystem.ObjectDestroyMessage UnityEngine.Networking.ClientScene::s_ObjectDestroyMessage
	ObjectDestroyMessage_tDABDFFAAF87735B56D448C9DC817E73D8DF8BB07 * ___s_ObjectDestroyMessage_8;
	// UnityEngine.Networking.NetworkSystem.ObjectSpawnMessage UnityEngine.Networking.ClientScene::s_ObjectSpawnMessage
	ObjectSpawnMessage_t5BC8D432216492084C7D2DC082BB8D2A81EE9E33 * ___s_ObjectSpawnMessage_9;
	// UnityEngine.Networking.NetworkSystem.OwnerMessage UnityEngine.Networking.ClientScene::s_OwnerMessage
	OwnerMessage_tB0123F3077643618B07980F6ACB02D4BC4C9E887 * ___s_OwnerMessage_10;
	// UnityEngine.Networking.NetworkSystem.ClientAuthorityMessage UnityEngine.Networking.ClientScene::s_ClientAuthorityMessage
	ClientAuthorityMessage_t3236F2A4C2A172651CCA0E9807EA8FB14D1E5E21 * ___s_ClientAuthorityMessage_11;
	// System.Int32 UnityEngine.Networking.ClientScene::s_ReconnectId
	int32_t ___s_ReconnectId_14;
	// UnityEngine.Networking.NetworkSystem.PeerInfoMessage[] UnityEngine.Networking.ClientScene::s_Peers
	PeerInfoMessageU5BU5D_t6AD51F1C65B2BBE6A626AB37377689360E088984* ___s_Peers_15;
	// System.Collections.Generic.List`1<UnityEngine.Networking.ClientScene_PendingOwner> UnityEngine.Networking.ClientScene::s_PendingOwnerIds
	List_1_t93B3F1949B711B014F8D6B02F94C18FA9A0B4EC0 * ___s_PendingOwnerIds_16;

public:
	inline static int32_t get_offset_of_s_LocalPlayers_0() { return static_cast<int32_t>(offsetof(ClientScene_t0A10B1F436A5AA8D5FC9B18C9ED0B32008809A3E_StaticFields, ___s_LocalPlayers_0)); }
	inline List_1_t44D1B61364FCFEF62067A4726A735856DFDFD545 * get_s_LocalPlayers_0() const { return ___s_LocalPlayers_0; }
	inline List_1_t44D1B61364FCFEF62067A4726A735856DFDFD545 ** get_address_of_s_LocalPlayers_0() { return &___s_LocalPlayers_0; }
	inline void set_s_LocalPlayers_0(List_1_t44D1B61364FCFEF62067A4726A735856DFDFD545 * value)
	{
		___s_LocalPlayers_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_LocalPlayers_0), (void*)value);
	}

	inline static int32_t get_offset_of_s_ReadyConnection_1() { return static_cast<int32_t>(offsetof(ClientScene_t0A10B1F436A5AA8D5FC9B18C9ED0B32008809A3E_StaticFields, ___s_ReadyConnection_1)); }
	inline NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * get_s_ReadyConnection_1() const { return ___s_ReadyConnection_1; }
	inline NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA ** get_address_of_s_ReadyConnection_1() { return &___s_ReadyConnection_1; }
	inline void set_s_ReadyConnection_1(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * value)
	{
		___s_ReadyConnection_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_ReadyConnection_1), (void*)value);
	}

	inline static int32_t get_offset_of_s_SpawnableObjects_2() { return static_cast<int32_t>(offsetof(ClientScene_t0A10B1F436A5AA8D5FC9B18C9ED0B32008809A3E_StaticFields, ___s_SpawnableObjects_2)); }
	inline Dictionary_2_t7BABA42F397000124B62D0DE8AC6226B5276B2CA * get_s_SpawnableObjects_2() const { return ___s_SpawnableObjects_2; }
	inline Dictionary_2_t7BABA42F397000124B62D0DE8AC6226B5276B2CA ** get_address_of_s_SpawnableObjects_2() { return &___s_SpawnableObjects_2; }
	inline void set_s_SpawnableObjects_2(Dictionary_2_t7BABA42F397000124B62D0DE8AC6226B5276B2CA * value)
	{
		___s_SpawnableObjects_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_SpawnableObjects_2), (void*)value);
	}

	inline static int32_t get_offset_of_s_IsReady_3() { return static_cast<int32_t>(offsetof(ClientScene_t0A10B1F436A5AA8D5FC9B18C9ED0B32008809A3E_StaticFields, ___s_IsReady_3)); }
	inline bool get_s_IsReady_3() const { return ___s_IsReady_3; }
	inline bool* get_address_of_s_IsReady_3() { return &___s_IsReady_3; }
	inline void set_s_IsReady_3(bool value)
	{
		___s_IsReady_3 = value;
	}

	inline static int32_t get_offset_of_s_IsSpawnFinished_4() { return static_cast<int32_t>(offsetof(ClientScene_t0A10B1F436A5AA8D5FC9B18C9ED0B32008809A3E_StaticFields, ___s_IsSpawnFinished_4)); }
	inline bool get_s_IsSpawnFinished_4() const { return ___s_IsSpawnFinished_4; }
	inline bool* get_address_of_s_IsSpawnFinished_4() { return &___s_IsSpawnFinished_4; }
	inline void set_s_IsSpawnFinished_4(bool value)
	{
		___s_IsSpawnFinished_4 = value;
	}

	inline static int32_t get_offset_of_s_NetworkScene_5() { return static_cast<int32_t>(offsetof(ClientScene_t0A10B1F436A5AA8D5FC9B18C9ED0B32008809A3E_StaticFields, ___s_NetworkScene_5)); }
	inline NetworkScene_t67A8AC9779C203B146A8723FA561736890CA9A40 * get_s_NetworkScene_5() const { return ___s_NetworkScene_5; }
	inline NetworkScene_t67A8AC9779C203B146A8723FA561736890CA9A40 ** get_address_of_s_NetworkScene_5() { return &___s_NetworkScene_5; }
	inline void set_s_NetworkScene_5(NetworkScene_t67A8AC9779C203B146A8723FA561736890CA9A40 * value)
	{
		___s_NetworkScene_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_NetworkScene_5), (void*)value);
	}

	inline static int32_t get_offset_of_s_ObjectSpawnSceneMessage_6() { return static_cast<int32_t>(offsetof(ClientScene_t0A10B1F436A5AA8D5FC9B18C9ED0B32008809A3E_StaticFields, ___s_ObjectSpawnSceneMessage_6)); }
	inline ObjectSpawnSceneMessage_tA5AF2D7F8B73A6C29D346E11D82B0CE86F7B28FC * get_s_ObjectSpawnSceneMessage_6() const { return ___s_ObjectSpawnSceneMessage_6; }
	inline ObjectSpawnSceneMessage_tA5AF2D7F8B73A6C29D346E11D82B0CE86F7B28FC ** get_address_of_s_ObjectSpawnSceneMessage_6() { return &___s_ObjectSpawnSceneMessage_6; }
	inline void set_s_ObjectSpawnSceneMessage_6(ObjectSpawnSceneMessage_tA5AF2D7F8B73A6C29D346E11D82B0CE86F7B28FC * value)
	{
		___s_ObjectSpawnSceneMessage_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_ObjectSpawnSceneMessage_6), (void*)value);
	}

	inline static int32_t get_offset_of_s_ObjectSpawnFinishedMessage_7() { return static_cast<int32_t>(offsetof(ClientScene_t0A10B1F436A5AA8D5FC9B18C9ED0B32008809A3E_StaticFields, ___s_ObjectSpawnFinishedMessage_7)); }
	inline ObjectSpawnFinishedMessage_t02EF525CD1734EDEA77DA073E728E6FBD5E7C550 * get_s_ObjectSpawnFinishedMessage_7() const { return ___s_ObjectSpawnFinishedMessage_7; }
	inline ObjectSpawnFinishedMessage_t02EF525CD1734EDEA77DA073E728E6FBD5E7C550 ** get_address_of_s_ObjectSpawnFinishedMessage_7() { return &___s_ObjectSpawnFinishedMessage_7; }
	inline void set_s_ObjectSpawnFinishedMessage_7(ObjectSpawnFinishedMessage_t02EF525CD1734EDEA77DA073E728E6FBD5E7C550 * value)
	{
		___s_ObjectSpawnFinishedMessage_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_ObjectSpawnFinishedMessage_7), (void*)value);
	}

	inline static int32_t get_offset_of_s_ObjectDestroyMessage_8() { return static_cast<int32_t>(offsetof(ClientScene_t0A10B1F436A5AA8D5FC9B18C9ED0B32008809A3E_StaticFields, ___s_ObjectDestroyMessage_8)); }
	inline ObjectDestroyMessage_tDABDFFAAF87735B56D448C9DC817E73D8DF8BB07 * get_s_ObjectDestroyMessage_8() const { return ___s_ObjectDestroyMessage_8; }
	inline ObjectDestroyMessage_tDABDFFAAF87735B56D448C9DC817E73D8DF8BB07 ** get_address_of_s_ObjectDestroyMessage_8() { return &___s_ObjectDestroyMessage_8; }
	inline void set_s_ObjectDestroyMessage_8(ObjectDestroyMessage_tDABDFFAAF87735B56D448C9DC817E73D8DF8BB07 * value)
	{
		___s_ObjectDestroyMessage_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_ObjectDestroyMessage_8), (void*)value);
	}

	inline static int32_t get_offset_of_s_ObjectSpawnMessage_9() { return static_cast<int32_t>(offsetof(ClientScene_t0A10B1F436A5AA8D5FC9B18C9ED0B32008809A3E_StaticFields, ___s_ObjectSpawnMessage_9)); }
	inline ObjectSpawnMessage_t5BC8D432216492084C7D2DC082BB8D2A81EE9E33 * get_s_ObjectSpawnMessage_9() const { return ___s_ObjectSpawnMessage_9; }
	inline ObjectSpawnMessage_t5BC8D432216492084C7D2DC082BB8D2A81EE9E33 ** get_address_of_s_ObjectSpawnMessage_9() { return &___s_ObjectSpawnMessage_9; }
	inline void set_s_ObjectSpawnMessage_9(ObjectSpawnMessage_t5BC8D432216492084C7D2DC082BB8D2A81EE9E33 * value)
	{
		___s_ObjectSpawnMessage_9 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_ObjectSpawnMessage_9), (void*)value);
	}

	inline static int32_t get_offset_of_s_OwnerMessage_10() { return static_cast<int32_t>(offsetof(ClientScene_t0A10B1F436A5AA8D5FC9B18C9ED0B32008809A3E_StaticFields, ___s_OwnerMessage_10)); }
	inline OwnerMessage_tB0123F3077643618B07980F6ACB02D4BC4C9E887 * get_s_OwnerMessage_10() const { return ___s_OwnerMessage_10; }
	inline OwnerMessage_tB0123F3077643618B07980F6ACB02D4BC4C9E887 ** get_address_of_s_OwnerMessage_10() { return &___s_OwnerMessage_10; }
	inline void set_s_OwnerMessage_10(OwnerMessage_tB0123F3077643618B07980F6ACB02D4BC4C9E887 * value)
	{
		___s_OwnerMessage_10 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_OwnerMessage_10), (void*)value);
	}

	inline static int32_t get_offset_of_s_ClientAuthorityMessage_11() { return static_cast<int32_t>(offsetof(ClientScene_t0A10B1F436A5AA8D5FC9B18C9ED0B32008809A3E_StaticFields, ___s_ClientAuthorityMessage_11)); }
	inline ClientAuthorityMessage_t3236F2A4C2A172651CCA0E9807EA8FB14D1E5E21 * get_s_ClientAuthorityMessage_11() const { return ___s_ClientAuthorityMessage_11; }
	inline ClientAuthorityMessage_t3236F2A4C2A172651CCA0E9807EA8FB14D1E5E21 ** get_address_of_s_ClientAuthorityMessage_11() { return &___s_ClientAuthorityMessage_11; }
	inline void set_s_ClientAuthorityMessage_11(ClientAuthorityMessage_t3236F2A4C2A172651CCA0E9807EA8FB14D1E5E21 * value)
	{
		___s_ClientAuthorityMessage_11 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_ClientAuthorityMessage_11), (void*)value);
	}

	inline static int32_t get_offset_of_s_ReconnectId_14() { return static_cast<int32_t>(offsetof(ClientScene_t0A10B1F436A5AA8D5FC9B18C9ED0B32008809A3E_StaticFields, ___s_ReconnectId_14)); }
	inline int32_t get_s_ReconnectId_14() const { return ___s_ReconnectId_14; }
	inline int32_t* get_address_of_s_ReconnectId_14() { return &___s_ReconnectId_14; }
	inline void set_s_ReconnectId_14(int32_t value)
	{
		___s_ReconnectId_14 = value;
	}

	inline static int32_t get_offset_of_s_Peers_15() { return static_cast<int32_t>(offsetof(ClientScene_t0A10B1F436A5AA8D5FC9B18C9ED0B32008809A3E_StaticFields, ___s_Peers_15)); }
	inline PeerInfoMessageU5BU5D_t6AD51F1C65B2BBE6A626AB37377689360E088984* get_s_Peers_15() const { return ___s_Peers_15; }
	inline PeerInfoMessageU5BU5D_t6AD51F1C65B2BBE6A626AB37377689360E088984** get_address_of_s_Peers_15() { return &___s_Peers_15; }
	inline void set_s_Peers_15(PeerInfoMessageU5BU5D_t6AD51F1C65B2BBE6A626AB37377689360E088984* value)
	{
		___s_Peers_15 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_Peers_15), (void*)value);
	}

	inline static int32_t get_offset_of_s_PendingOwnerIds_16() { return static_cast<int32_t>(offsetof(ClientScene_t0A10B1F436A5AA8D5FC9B18C9ED0B32008809A3E_StaticFields, ___s_PendingOwnerIds_16)); }
	inline List_1_t93B3F1949B711B014F8D6B02F94C18FA9A0B4EC0 * get_s_PendingOwnerIds_16() const { return ___s_PendingOwnerIds_16; }
	inline List_1_t93B3F1949B711B014F8D6B02F94C18FA9A0B4EC0 ** get_address_of_s_PendingOwnerIds_16() { return &___s_PendingOwnerIds_16; }
	inline void set_s_PendingOwnerIds_16(List_1_t93B3F1949B711B014F8D6B02F94C18FA9A0B4EC0 * value)
	{
		___s_PendingOwnerIds_16 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_PendingOwnerIds_16), (void*)value);
	}
};


// UnityEngine.Networking.MessageBase
struct  MessageBase_t2EA42B01AD6A5F36EAF84BE623801951B9F55416  : public RuntimeObject
{
public:

public:
};


// UnityEngine.Networking.NetBuffer
struct  NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C  : public RuntimeObject
{
public:
	// System.Byte[] UnityEngine.Networking.NetBuffer::m_Buffer
	ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___m_Buffer_0;
	// System.UInt32 UnityEngine.Networking.NetBuffer::m_Pos
	uint32_t ___m_Pos_1;

public:
	inline static int32_t get_offset_of_m_Buffer_0() { return static_cast<int32_t>(offsetof(NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C, ___m_Buffer_0)); }
	inline ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* get_m_Buffer_0() const { return ___m_Buffer_0; }
	inline ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821** get_address_of_m_Buffer_0() { return &___m_Buffer_0; }
	inline void set_m_Buffer_0(ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* value)
	{
		___m_Buffer_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Buffer_0), (void*)value);
	}

	inline static int32_t get_offset_of_m_Pos_1() { return static_cast<int32_t>(offsetof(NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C, ___m_Pos_1)); }
	inline uint32_t get_m_Pos_1() const { return ___m_Pos_1; }
	inline uint32_t* get_address_of_m_Pos_1() { return &___m_Pos_1; }
	inline void set_m_Pos_1(uint32_t value)
	{
		___m_Pos_1 = value;
	}
};


// UnityEngine.Networking.NetworkMessage
struct  NetworkMessage_tCD66E2AE395A185EFE622EBB5497C95F6754685C  : public RuntimeObject
{
public:
	// System.Int16 UnityEngine.Networking.NetworkMessage::msgType
	int16_t ___msgType_1;
	// UnityEngine.Networking.NetworkConnection UnityEngine.Networking.NetworkMessage::conn
	NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * ___conn_2;
	// UnityEngine.Networking.NetworkReader UnityEngine.Networking.NetworkMessage::reader
	NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * ___reader_3;
	// System.Int32 UnityEngine.Networking.NetworkMessage::channelId
	int32_t ___channelId_4;

public:
	inline static int32_t get_offset_of_msgType_1() { return static_cast<int32_t>(offsetof(NetworkMessage_tCD66E2AE395A185EFE622EBB5497C95F6754685C, ___msgType_1)); }
	inline int16_t get_msgType_1() const { return ___msgType_1; }
	inline int16_t* get_address_of_msgType_1() { return &___msgType_1; }
	inline void set_msgType_1(int16_t value)
	{
		___msgType_1 = value;
	}

	inline static int32_t get_offset_of_conn_2() { return static_cast<int32_t>(offsetof(NetworkMessage_tCD66E2AE395A185EFE622EBB5497C95F6754685C, ___conn_2)); }
	inline NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * get_conn_2() const { return ___conn_2; }
	inline NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA ** get_address_of_conn_2() { return &___conn_2; }
	inline void set_conn_2(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * value)
	{
		___conn_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___conn_2), (void*)value);
	}

	inline static int32_t get_offset_of_reader_3() { return static_cast<int32_t>(offsetof(NetworkMessage_tCD66E2AE395A185EFE622EBB5497C95F6754685C, ___reader_3)); }
	inline NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * get_reader_3() const { return ___reader_3; }
	inline NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 ** get_address_of_reader_3() { return &___reader_3; }
	inline void set_reader_3(NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * value)
	{
		___reader_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___reader_3), (void*)value);
	}

	inline static int32_t get_offset_of_channelId_4() { return static_cast<int32_t>(offsetof(NetworkMessage_tCD66E2AE395A185EFE622EBB5497C95F6754685C, ___channelId_4)); }
	inline int32_t get_channelId_4() const { return ___channelId_4; }
	inline int32_t* get_address_of_channelId_4() { return &___channelId_4; }
	inline void set_channelId_4(int32_t value)
	{
		___channelId_4 = value;
	}
};


// UnityEngine.Networking.NetworkReader
struct  NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12  : public RuntimeObject
{
public:
	// UnityEngine.Networking.NetBuffer UnityEngine.Networking.NetworkReader::m_buf
	NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * ___m_buf_0;

public:
	inline static int32_t get_offset_of_m_buf_0() { return static_cast<int32_t>(offsetof(NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12, ___m_buf_0)); }
	inline NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * get_m_buf_0() const { return ___m_buf_0; }
	inline NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C ** get_address_of_m_buf_0() { return &___m_buf_0; }
	inline void set_m_buf_0(NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * value)
	{
		___m_buf_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_buf_0), (void*)value);
	}
};

struct NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12_StaticFields
{
public:
	// System.Byte[] UnityEngine.Networking.NetworkReader::s_StringReaderBuffer
	ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___s_StringReaderBuffer_3;
	// System.Text.Encoding UnityEngine.Networking.NetworkReader::s_Encoding
	Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * ___s_Encoding_4;

public:
	inline static int32_t get_offset_of_s_StringReaderBuffer_3() { return static_cast<int32_t>(offsetof(NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12_StaticFields, ___s_StringReaderBuffer_3)); }
	inline ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* get_s_StringReaderBuffer_3() const { return ___s_StringReaderBuffer_3; }
	inline ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821** get_address_of_s_StringReaderBuffer_3() { return &___s_StringReaderBuffer_3; }
	inline void set_s_StringReaderBuffer_3(ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* value)
	{
		___s_StringReaderBuffer_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_StringReaderBuffer_3), (void*)value);
	}

	inline static int32_t get_offset_of_s_Encoding_4() { return static_cast<int32_t>(offsetof(NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12_StaticFields, ___s_Encoding_4)); }
	inline Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * get_s_Encoding_4() const { return ___s_Encoding_4; }
	inline Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 ** get_address_of_s_Encoding_4() { return &___s_Encoding_4; }
	inline void set_s_Encoding_4(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * value)
	{
		___s_Encoding_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_Encoding_4), (void*)value);
	}
};


// UnityEngine.Networking.NetworkServer
struct  NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1  : public RuntimeObject
{
public:
	// System.Boolean UnityEngine.Networking.NetworkServer::m_LocalClientActive
	bool ___m_LocalClientActive_4;
	// System.Collections.Generic.List`1<UnityEngine.Networking.NetworkConnection> UnityEngine.Networking.NetworkServer::m_LocalConnectionsFakeList
	List_1_t8B02DD1F0211D3E19F6A6E0204AF7D7537912362 * ___m_LocalConnectionsFakeList_5;
	// UnityEngine.Networking.ULocalConnectionToClient UnityEngine.Networking.NetworkServer::m_LocalConnection
	ULocalConnectionToClient_t7AF7EBF2BEC3714F75EF894035BFAE9E6F9561A8 * ___m_LocalConnection_6;
	// UnityEngine.Networking.NetworkScene UnityEngine.Networking.NetworkServer::m_NetworkScene
	NetworkScene_t67A8AC9779C203B146A8723FA561736890CA9A40 * ___m_NetworkScene_7;
	// System.Collections.Generic.HashSet`1<System.Int32> UnityEngine.Networking.NetworkServer::m_ExternalConnections
	HashSet_1_tC4214D83D479652EF2A07346543F228C3C0A8D74 * ___m_ExternalConnections_8;
	// UnityEngine.Networking.NetworkServer_ServerSimpleWrapper UnityEngine.Networking.NetworkServer::m_SimpleServerSimple
	ServerSimpleWrapper_t1ECF42A66748FA970402440F00E743DB5E2AAA32 * ___m_SimpleServerSimple_9;
	// System.Single UnityEngine.Networking.NetworkServer::m_MaxDelay
	float ___m_MaxDelay_10;
	// System.Collections.Generic.HashSet`1<UnityEngine.Networking.NetworkInstanceId> UnityEngine.Networking.NetworkServer::m_RemoveList
	HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990 * ___m_RemoveList_11;
	// System.Int32 UnityEngine.Networking.NetworkServer::m_RemoveListCount
	int32_t ___m_RemoveListCount_12;

public:
	inline static int32_t get_offset_of_m_LocalClientActive_4() { return static_cast<int32_t>(offsetof(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1, ___m_LocalClientActive_4)); }
	inline bool get_m_LocalClientActive_4() const { return ___m_LocalClientActive_4; }
	inline bool* get_address_of_m_LocalClientActive_4() { return &___m_LocalClientActive_4; }
	inline void set_m_LocalClientActive_4(bool value)
	{
		___m_LocalClientActive_4 = value;
	}

	inline static int32_t get_offset_of_m_LocalConnectionsFakeList_5() { return static_cast<int32_t>(offsetof(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1, ___m_LocalConnectionsFakeList_5)); }
	inline List_1_t8B02DD1F0211D3E19F6A6E0204AF7D7537912362 * get_m_LocalConnectionsFakeList_5() const { return ___m_LocalConnectionsFakeList_5; }
	inline List_1_t8B02DD1F0211D3E19F6A6E0204AF7D7537912362 ** get_address_of_m_LocalConnectionsFakeList_5() { return &___m_LocalConnectionsFakeList_5; }
	inline void set_m_LocalConnectionsFakeList_5(List_1_t8B02DD1F0211D3E19F6A6E0204AF7D7537912362 * value)
	{
		___m_LocalConnectionsFakeList_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_LocalConnectionsFakeList_5), (void*)value);
	}

	inline static int32_t get_offset_of_m_LocalConnection_6() { return static_cast<int32_t>(offsetof(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1, ___m_LocalConnection_6)); }
	inline ULocalConnectionToClient_t7AF7EBF2BEC3714F75EF894035BFAE9E6F9561A8 * get_m_LocalConnection_6() const { return ___m_LocalConnection_6; }
	inline ULocalConnectionToClient_t7AF7EBF2BEC3714F75EF894035BFAE9E6F9561A8 ** get_address_of_m_LocalConnection_6() { return &___m_LocalConnection_6; }
	inline void set_m_LocalConnection_6(ULocalConnectionToClient_t7AF7EBF2BEC3714F75EF894035BFAE9E6F9561A8 * value)
	{
		___m_LocalConnection_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_LocalConnection_6), (void*)value);
	}

	inline static int32_t get_offset_of_m_NetworkScene_7() { return static_cast<int32_t>(offsetof(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1, ___m_NetworkScene_7)); }
	inline NetworkScene_t67A8AC9779C203B146A8723FA561736890CA9A40 * get_m_NetworkScene_7() const { return ___m_NetworkScene_7; }
	inline NetworkScene_t67A8AC9779C203B146A8723FA561736890CA9A40 ** get_address_of_m_NetworkScene_7() { return &___m_NetworkScene_7; }
	inline void set_m_NetworkScene_7(NetworkScene_t67A8AC9779C203B146A8723FA561736890CA9A40 * value)
	{
		___m_NetworkScene_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_NetworkScene_7), (void*)value);
	}

	inline static int32_t get_offset_of_m_ExternalConnections_8() { return static_cast<int32_t>(offsetof(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1, ___m_ExternalConnections_8)); }
	inline HashSet_1_tC4214D83D479652EF2A07346543F228C3C0A8D74 * get_m_ExternalConnections_8() const { return ___m_ExternalConnections_8; }
	inline HashSet_1_tC4214D83D479652EF2A07346543F228C3C0A8D74 ** get_address_of_m_ExternalConnections_8() { return &___m_ExternalConnections_8; }
	inline void set_m_ExternalConnections_8(HashSet_1_tC4214D83D479652EF2A07346543F228C3C0A8D74 * value)
	{
		___m_ExternalConnections_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_ExternalConnections_8), (void*)value);
	}

	inline static int32_t get_offset_of_m_SimpleServerSimple_9() { return static_cast<int32_t>(offsetof(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1, ___m_SimpleServerSimple_9)); }
	inline ServerSimpleWrapper_t1ECF42A66748FA970402440F00E743DB5E2AAA32 * get_m_SimpleServerSimple_9() const { return ___m_SimpleServerSimple_9; }
	inline ServerSimpleWrapper_t1ECF42A66748FA970402440F00E743DB5E2AAA32 ** get_address_of_m_SimpleServerSimple_9() { return &___m_SimpleServerSimple_9; }
	inline void set_m_SimpleServerSimple_9(ServerSimpleWrapper_t1ECF42A66748FA970402440F00E743DB5E2AAA32 * value)
	{
		___m_SimpleServerSimple_9 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_SimpleServerSimple_9), (void*)value);
	}

	inline static int32_t get_offset_of_m_MaxDelay_10() { return static_cast<int32_t>(offsetof(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1, ___m_MaxDelay_10)); }
	inline float get_m_MaxDelay_10() const { return ___m_MaxDelay_10; }
	inline float* get_address_of_m_MaxDelay_10() { return &___m_MaxDelay_10; }
	inline void set_m_MaxDelay_10(float value)
	{
		___m_MaxDelay_10 = value;
	}

	inline static int32_t get_offset_of_m_RemoveList_11() { return static_cast<int32_t>(offsetof(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1, ___m_RemoveList_11)); }
	inline HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990 * get_m_RemoveList_11() const { return ___m_RemoveList_11; }
	inline HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990 ** get_address_of_m_RemoveList_11() { return &___m_RemoveList_11; }
	inline void set_m_RemoveList_11(HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990 * value)
	{
		___m_RemoveList_11 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_RemoveList_11), (void*)value);
	}

	inline static int32_t get_offset_of_m_RemoveListCount_12() { return static_cast<int32_t>(offsetof(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1, ___m_RemoveListCount_12)); }
	inline int32_t get_m_RemoveListCount_12() const { return ___m_RemoveListCount_12; }
	inline int32_t* get_address_of_m_RemoveListCount_12() { return &___m_RemoveListCount_12; }
	inline void set_m_RemoveListCount_12(int32_t value)
	{
		___m_RemoveListCount_12 = value;
	}
};

struct NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1_StaticFields
{
public:
	// System.Boolean UnityEngine.Networking.NetworkServer::s_Active
	bool ___s_Active_0;
	// UnityEngine.Networking.NetworkServer modreq(System.Runtime.CompilerServices.IsVolatile) UnityEngine.Networking.NetworkServer::s_Instance
	NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1 * ___s_Instance_1;
	// System.Object UnityEngine.Networking.NetworkServer::s_Sync
	RuntimeObject * ___s_Sync_2;
	// System.Boolean UnityEngine.Networking.NetworkServer::m_DontListen
	bool ___m_DontListen_3;
	// System.UInt16 UnityEngine.Networking.NetworkServer::maxPacketSize
	uint16_t ___maxPacketSize_14;
	// UnityEngine.Networking.NetworkSystem.RemovePlayerMessage UnityEngine.Networking.NetworkServer::s_RemovePlayerMessage
	RemovePlayerMessage_t51B0D9BCA3C2B4FD772A2972588CC0915FD4CEBF * ___s_RemovePlayerMessage_15;

public:
	inline static int32_t get_offset_of_s_Active_0() { return static_cast<int32_t>(offsetof(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1_StaticFields, ___s_Active_0)); }
	inline bool get_s_Active_0() const { return ___s_Active_0; }
	inline bool* get_address_of_s_Active_0() { return &___s_Active_0; }
	inline void set_s_Active_0(bool value)
	{
		___s_Active_0 = value;
	}

	inline static int32_t get_offset_of_s_Instance_1() { return static_cast<int32_t>(offsetof(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1_StaticFields, ___s_Instance_1)); }
	inline NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1 * get_s_Instance_1() const { return ___s_Instance_1; }
	inline NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1 ** get_address_of_s_Instance_1() { return &___s_Instance_1; }
	inline void set_s_Instance_1(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1 * value)
	{
		___s_Instance_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_Instance_1), (void*)value);
	}

	inline static int32_t get_offset_of_s_Sync_2() { return static_cast<int32_t>(offsetof(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1_StaticFields, ___s_Sync_2)); }
	inline RuntimeObject * get_s_Sync_2() const { return ___s_Sync_2; }
	inline RuntimeObject ** get_address_of_s_Sync_2() { return &___s_Sync_2; }
	inline void set_s_Sync_2(RuntimeObject * value)
	{
		___s_Sync_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_Sync_2), (void*)value);
	}

	inline static int32_t get_offset_of_m_DontListen_3() { return static_cast<int32_t>(offsetof(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1_StaticFields, ___m_DontListen_3)); }
	inline bool get_m_DontListen_3() const { return ___m_DontListen_3; }
	inline bool* get_address_of_m_DontListen_3() { return &___m_DontListen_3; }
	inline void set_m_DontListen_3(bool value)
	{
		___m_DontListen_3 = value;
	}

	inline static int32_t get_offset_of_maxPacketSize_14() { return static_cast<int32_t>(offsetof(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1_StaticFields, ___maxPacketSize_14)); }
	inline uint16_t get_maxPacketSize_14() const { return ___maxPacketSize_14; }
	inline uint16_t* get_address_of_maxPacketSize_14() { return &___maxPacketSize_14; }
	inline void set_maxPacketSize_14(uint16_t value)
	{
		___maxPacketSize_14 = value;
	}

	inline static int32_t get_offset_of_s_RemovePlayerMessage_15() { return static_cast<int32_t>(offsetof(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1_StaticFields, ___s_RemovePlayerMessage_15)); }
	inline RemovePlayerMessage_t51B0D9BCA3C2B4FD772A2972588CC0915FD4CEBF * get_s_RemovePlayerMessage_15() const { return ___s_RemovePlayerMessage_15; }
	inline RemovePlayerMessage_t51B0D9BCA3C2B4FD772A2972588CC0915FD4CEBF ** get_address_of_s_RemovePlayerMessage_15() { return &___s_RemovePlayerMessage_15; }
	inline void set_s_RemovePlayerMessage_15(RemovePlayerMessage_t51B0D9BCA3C2B4FD772A2972588CC0915FD4CEBF * value)
	{
		___s_RemovePlayerMessage_15 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_RemovePlayerMessage_15), (void*)value);
	}
};


// UnityEngine.Networking.PlayerController
struct  PlayerController_tAB80FD64EAB9692832107A2D107D39D078CE7C5E  : public RuntimeObject
{
public:
	// System.Int16 UnityEngine.Networking.PlayerController::playerControllerId
	int16_t ___playerControllerId_1;
	// UnityEngine.Networking.NetworkIdentity UnityEngine.Networking.PlayerController::unetView
	NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * ___unetView_2;
	// UnityEngine.GameObject UnityEngine.Networking.PlayerController::gameObject
	GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * ___gameObject_3;

public:
	inline static int32_t get_offset_of_playerControllerId_1() { return static_cast<int32_t>(offsetof(PlayerController_tAB80FD64EAB9692832107A2D107D39D078CE7C5E, ___playerControllerId_1)); }
	inline int16_t get_playerControllerId_1() const { return ___playerControllerId_1; }
	inline int16_t* get_address_of_playerControllerId_1() { return &___playerControllerId_1; }
	inline void set_playerControllerId_1(int16_t value)
	{
		___playerControllerId_1 = value;
	}

	inline static int32_t get_offset_of_unetView_2() { return static_cast<int32_t>(offsetof(PlayerController_tAB80FD64EAB9692832107A2D107D39D078CE7C5E, ___unetView_2)); }
	inline NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * get_unetView_2() const { return ___unetView_2; }
	inline NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B ** get_address_of_unetView_2() { return &___unetView_2; }
	inline void set_unetView_2(NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * value)
	{
		___unetView_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___unetView_2), (void*)value);
	}

	inline static int32_t get_offset_of_gameObject_3() { return static_cast<int32_t>(offsetof(PlayerController_tAB80FD64EAB9692832107A2D107D39D078CE7C5E, ___gameObject_3)); }
	inline GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * get_gameObject_3() const { return ___gameObject_3; }
	inline GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F ** get_address_of_gameObject_3() { return &___gameObject_3; }
	inline void set_gameObject_3(GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * value)
	{
		___gameObject_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___gameObject_3), (void*)value);
	}
};


// UnityEngine.Networking.SyncList`1<System.Boolean>
struct  SyncList_1_tDDF00E08E649A86264E50205CB99495D1AD2E8D0  : public RuntimeObject
{
public:
	// System.Collections.Generic.List`1<T> UnityEngine.Networking.SyncList`1::m_Objects
	List_1_tCF6613377FD07378DDA05A5BC95C5EF4A07B3E75 * ___m_Objects_0;
	// UnityEngine.Networking.NetworkBehaviour UnityEngine.Networking.SyncList`1::m_Behaviour
	NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * ___m_Behaviour_1;
	// System.Int32 UnityEngine.Networking.SyncList`1::m_CmdHash
	int32_t ___m_CmdHash_2;
	// UnityEngine.Networking.SyncList`1_SyncListChanged<T> UnityEngine.Networking.SyncList`1::m_Callback
	SyncListChanged_t5156E5B2411DE07D5AEA8F98B87E4FBD1E626D6E * ___m_Callback_3;

public:
	inline static int32_t get_offset_of_m_Objects_0() { return static_cast<int32_t>(offsetof(SyncList_1_tDDF00E08E649A86264E50205CB99495D1AD2E8D0, ___m_Objects_0)); }
	inline List_1_tCF6613377FD07378DDA05A5BC95C5EF4A07B3E75 * get_m_Objects_0() const { return ___m_Objects_0; }
	inline List_1_tCF6613377FD07378DDA05A5BC95C5EF4A07B3E75 ** get_address_of_m_Objects_0() { return &___m_Objects_0; }
	inline void set_m_Objects_0(List_1_tCF6613377FD07378DDA05A5BC95C5EF4A07B3E75 * value)
	{
		___m_Objects_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Objects_0), (void*)value);
	}

	inline static int32_t get_offset_of_m_Behaviour_1() { return static_cast<int32_t>(offsetof(SyncList_1_tDDF00E08E649A86264E50205CB99495D1AD2E8D0, ___m_Behaviour_1)); }
	inline NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * get_m_Behaviour_1() const { return ___m_Behaviour_1; }
	inline NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C ** get_address_of_m_Behaviour_1() { return &___m_Behaviour_1; }
	inline void set_m_Behaviour_1(NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * value)
	{
		___m_Behaviour_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Behaviour_1), (void*)value);
	}

	inline static int32_t get_offset_of_m_CmdHash_2() { return static_cast<int32_t>(offsetof(SyncList_1_tDDF00E08E649A86264E50205CB99495D1AD2E8D0, ___m_CmdHash_2)); }
	inline int32_t get_m_CmdHash_2() const { return ___m_CmdHash_2; }
	inline int32_t* get_address_of_m_CmdHash_2() { return &___m_CmdHash_2; }
	inline void set_m_CmdHash_2(int32_t value)
	{
		___m_CmdHash_2 = value;
	}

	inline static int32_t get_offset_of_m_Callback_3() { return static_cast<int32_t>(offsetof(SyncList_1_tDDF00E08E649A86264E50205CB99495D1AD2E8D0, ___m_Callback_3)); }
	inline SyncListChanged_t5156E5B2411DE07D5AEA8F98B87E4FBD1E626D6E * get_m_Callback_3() const { return ___m_Callback_3; }
	inline SyncListChanged_t5156E5B2411DE07D5AEA8F98B87E4FBD1E626D6E ** get_address_of_m_Callback_3() { return &___m_Callback_3; }
	inline void set_m_Callback_3(SyncListChanged_t5156E5B2411DE07D5AEA8F98B87E4FBD1E626D6E * value)
	{
		___m_Callback_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Callback_3), (void*)value);
	}
};


// UnityEngine.Networking.SyncList`1<System.Int32>
struct  SyncList_1_t8595ACD08C8686AC9547A225ACF209C171FF0665  : public RuntimeObject
{
public:
	// System.Collections.Generic.List`1<T> UnityEngine.Networking.SyncList`1::m_Objects
	List_1_tE1526161A558A17A39A8B69D8EEF3801393B6226 * ___m_Objects_0;
	// UnityEngine.Networking.NetworkBehaviour UnityEngine.Networking.SyncList`1::m_Behaviour
	NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * ___m_Behaviour_1;
	// System.Int32 UnityEngine.Networking.SyncList`1::m_CmdHash
	int32_t ___m_CmdHash_2;
	// UnityEngine.Networking.SyncList`1_SyncListChanged<T> UnityEngine.Networking.SyncList`1::m_Callback
	SyncListChanged_t1A1EBC018732CBF3B1F86CE21BE59B5107844339 * ___m_Callback_3;

public:
	inline static int32_t get_offset_of_m_Objects_0() { return static_cast<int32_t>(offsetof(SyncList_1_t8595ACD08C8686AC9547A225ACF209C171FF0665, ___m_Objects_0)); }
	inline List_1_tE1526161A558A17A39A8B69D8EEF3801393B6226 * get_m_Objects_0() const { return ___m_Objects_0; }
	inline List_1_tE1526161A558A17A39A8B69D8EEF3801393B6226 ** get_address_of_m_Objects_0() { return &___m_Objects_0; }
	inline void set_m_Objects_0(List_1_tE1526161A558A17A39A8B69D8EEF3801393B6226 * value)
	{
		___m_Objects_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Objects_0), (void*)value);
	}

	inline static int32_t get_offset_of_m_Behaviour_1() { return static_cast<int32_t>(offsetof(SyncList_1_t8595ACD08C8686AC9547A225ACF209C171FF0665, ___m_Behaviour_1)); }
	inline NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * get_m_Behaviour_1() const { return ___m_Behaviour_1; }
	inline NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C ** get_address_of_m_Behaviour_1() { return &___m_Behaviour_1; }
	inline void set_m_Behaviour_1(NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * value)
	{
		___m_Behaviour_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Behaviour_1), (void*)value);
	}

	inline static int32_t get_offset_of_m_CmdHash_2() { return static_cast<int32_t>(offsetof(SyncList_1_t8595ACD08C8686AC9547A225ACF209C171FF0665, ___m_CmdHash_2)); }
	inline int32_t get_m_CmdHash_2() const { return ___m_CmdHash_2; }
	inline int32_t* get_address_of_m_CmdHash_2() { return &___m_CmdHash_2; }
	inline void set_m_CmdHash_2(int32_t value)
	{
		___m_CmdHash_2 = value;
	}

	inline static int32_t get_offset_of_m_Callback_3() { return static_cast<int32_t>(offsetof(SyncList_1_t8595ACD08C8686AC9547A225ACF209C171FF0665, ___m_Callback_3)); }
	inline SyncListChanged_t1A1EBC018732CBF3B1F86CE21BE59B5107844339 * get_m_Callback_3() const { return ___m_Callback_3; }
	inline SyncListChanged_t1A1EBC018732CBF3B1F86CE21BE59B5107844339 ** get_address_of_m_Callback_3() { return &___m_Callback_3; }
	inline void set_m_Callback_3(SyncListChanged_t1A1EBC018732CBF3B1F86CE21BE59B5107844339 * value)
	{
		___m_Callback_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Callback_3), (void*)value);
	}
};


// UnityEngine.Networking.SyncList`1<System.Single>
struct  SyncList_1_tDEB03E3C5252571915662095C7060998910FD0A2  : public RuntimeObject
{
public:
	// System.Collections.Generic.List`1<T> UnityEngine.Networking.SyncList`1::m_Objects
	List_1_t8980FA0E6CB3848F706C43D859930435C34BCC37 * ___m_Objects_0;
	// UnityEngine.Networking.NetworkBehaviour UnityEngine.Networking.SyncList`1::m_Behaviour
	NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * ___m_Behaviour_1;
	// System.Int32 UnityEngine.Networking.SyncList`1::m_CmdHash
	int32_t ___m_CmdHash_2;
	// UnityEngine.Networking.SyncList`1_SyncListChanged<T> UnityEngine.Networking.SyncList`1::m_Callback
	SyncListChanged_t63BE6CD22C2B89F865722BF980F4351DF0EC68D6 * ___m_Callback_3;

public:
	inline static int32_t get_offset_of_m_Objects_0() { return static_cast<int32_t>(offsetof(SyncList_1_tDEB03E3C5252571915662095C7060998910FD0A2, ___m_Objects_0)); }
	inline List_1_t8980FA0E6CB3848F706C43D859930435C34BCC37 * get_m_Objects_0() const { return ___m_Objects_0; }
	inline List_1_t8980FA0E6CB3848F706C43D859930435C34BCC37 ** get_address_of_m_Objects_0() { return &___m_Objects_0; }
	inline void set_m_Objects_0(List_1_t8980FA0E6CB3848F706C43D859930435C34BCC37 * value)
	{
		___m_Objects_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Objects_0), (void*)value);
	}

	inline static int32_t get_offset_of_m_Behaviour_1() { return static_cast<int32_t>(offsetof(SyncList_1_tDEB03E3C5252571915662095C7060998910FD0A2, ___m_Behaviour_1)); }
	inline NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * get_m_Behaviour_1() const { return ___m_Behaviour_1; }
	inline NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C ** get_address_of_m_Behaviour_1() { return &___m_Behaviour_1; }
	inline void set_m_Behaviour_1(NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * value)
	{
		___m_Behaviour_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Behaviour_1), (void*)value);
	}

	inline static int32_t get_offset_of_m_CmdHash_2() { return static_cast<int32_t>(offsetof(SyncList_1_tDEB03E3C5252571915662095C7060998910FD0A2, ___m_CmdHash_2)); }
	inline int32_t get_m_CmdHash_2() const { return ___m_CmdHash_2; }
	inline int32_t* get_address_of_m_CmdHash_2() { return &___m_CmdHash_2; }
	inline void set_m_CmdHash_2(int32_t value)
	{
		___m_CmdHash_2 = value;
	}

	inline static int32_t get_offset_of_m_Callback_3() { return static_cast<int32_t>(offsetof(SyncList_1_tDEB03E3C5252571915662095C7060998910FD0A2, ___m_Callback_3)); }
	inline SyncListChanged_t63BE6CD22C2B89F865722BF980F4351DF0EC68D6 * get_m_Callback_3() const { return ___m_Callback_3; }
	inline SyncListChanged_t63BE6CD22C2B89F865722BF980F4351DF0EC68D6 ** get_address_of_m_Callback_3() { return &___m_Callback_3; }
	inline void set_m_Callback_3(SyncListChanged_t63BE6CD22C2B89F865722BF980F4351DF0EC68D6 * value)
	{
		___m_Callback_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Callback_3), (void*)value);
	}
};


// UnityEngine.Networking.SyncList`1<System.String>
struct  SyncList_1_t4C2B8FAE3D901E39D62A3678DF05B8473364ED04  : public RuntimeObject
{
public:
	// System.Collections.Generic.List`1<T> UnityEngine.Networking.SyncList`1::m_Objects
	List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3 * ___m_Objects_0;
	// UnityEngine.Networking.NetworkBehaviour UnityEngine.Networking.SyncList`1::m_Behaviour
	NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * ___m_Behaviour_1;
	// System.Int32 UnityEngine.Networking.SyncList`1::m_CmdHash
	int32_t ___m_CmdHash_2;
	// UnityEngine.Networking.SyncList`1_SyncListChanged<T> UnityEngine.Networking.SyncList`1::m_Callback
	SyncListChanged_tF0BC42132992DEBFE9981ED50FE321BAA43CA3B5 * ___m_Callback_3;

public:
	inline static int32_t get_offset_of_m_Objects_0() { return static_cast<int32_t>(offsetof(SyncList_1_t4C2B8FAE3D901E39D62A3678DF05B8473364ED04, ___m_Objects_0)); }
	inline List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3 * get_m_Objects_0() const { return ___m_Objects_0; }
	inline List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3 ** get_address_of_m_Objects_0() { return &___m_Objects_0; }
	inline void set_m_Objects_0(List_1_tE8032E48C661C350FF9550E9063D595C0AB25CD3 * value)
	{
		___m_Objects_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Objects_0), (void*)value);
	}

	inline static int32_t get_offset_of_m_Behaviour_1() { return static_cast<int32_t>(offsetof(SyncList_1_t4C2B8FAE3D901E39D62A3678DF05B8473364ED04, ___m_Behaviour_1)); }
	inline NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * get_m_Behaviour_1() const { return ___m_Behaviour_1; }
	inline NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C ** get_address_of_m_Behaviour_1() { return &___m_Behaviour_1; }
	inline void set_m_Behaviour_1(NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * value)
	{
		___m_Behaviour_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Behaviour_1), (void*)value);
	}

	inline static int32_t get_offset_of_m_CmdHash_2() { return static_cast<int32_t>(offsetof(SyncList_1_t4C2B8FAE3D901E39D62A3678DF05B8473364ED04, ___m_CmdHash_2)); }
	inline int32_t get_m_CmdHash_2() const { return ___m_CmdHash_2; }
	inline int32_t* get_address_of_m_CmdHash_2() { return &___m_CmdHash_2; }
	inline void set_m_CmdHash_2(int32_t value)
	{
		___m_CmdHash_2 = value;
	}

	inline static int32_t get_offset_of_m_Callback_3() { return static_cast<int32_t>(offsetof(SyncList_1_t4C2B8FAE3D901E39D62A3678DF05B8473364ED04, ___m_Callback_3)); }
	inline SyncListChanged_tF0BC42132992DEBFE9981ED50FE321BAA43CA3B5 * get_m_Callback_3() const { return ___m_Callback_3; }
	inline SyncListChanged_tF0BC42132992DEBFE9981ED50FE321BAA43CA3B5 ** get_address_of_m_Callback_3() { return &___m_Callback_3; }
	inline void set_m_Callback_3(SyncListChanged_tF0BC42132992DEBFE9981ED50FE321BAA43CA3B5 * value)
	{
		___m_Callback_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Callback_3), (void*)value);
	}
};


// UnityEngine.Networking.SyncList`1<System.UInt32>
struct  SyncList_1_t6931FBA5633802C1CAE04093D0C6D9C88EE66627  : public RuntimeObject
{
public:
	// System.Collections.Generic.List`1<T> UnityEngine.Networking.SyncList`1::m_Objects
	List_1_t49B315A213A231954A3718D77EE3A2AFF443C38E * ___m_Objects_0;
	// UnityEngine.Networking.NetworkBehaviour UnityEngine.Networking.SyncList`1::m_Behaviour
	NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * ___m_Behaviour_1;
	// System.Int32 UnityEngine.Networking.SyncList`1::m_CmdHash
	int32_t ___m_CmdHash_2;
	// UnityEngine.Networking.SyncList`1_SyncListChanged<T> UnityEngine.Networking.SyncList`1::m_Callback
	SyncListChanged_t1804C1A09093327076629C515EC006A30F38C1B8 * ___m_Callback_3;

public:
	inline static int32_t get_offset_of_m_Objects_0() { return static_cast<int32_t>(offsetof(SyncList_1_t6931FBA5633802C1CAE04093D0C6D9C88EE66627, ___m_Objects_0)); }
	inline List_1_t49B315A213A231954A3718D77EE3A2AFF443C38E * get_m_Objects_0() const { return ___m_Objects_0; }
	inline List_1_t49B315A213A231954A3718D77EE3A2AFF443C38E ** get_address_of_m_Objects_0() { return &___m_Objects_0; }
	inline void set_m_Objects_0(List_1_t49B315A213A231954A3718D77EE3A2AFF443C38E * value)
	{
		___m_Objects_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Objects_0), (void*)value);
	}

	inline static int32_t get_offset_of_m_Behaviour_1() { return static_cast<int32_t>(offsetof(SyncList_1_t6931FBA5633802C1CAE04093D0C6D9C88EE66627, ___m_Behaviour_1)); }
	inline NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * get_m_Behaviour_1() const { return ___m_Behaviour_1; }
	inline NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C ** get_address_of_m_Behaviour_1() { return &___m_Behaviour_1; }
	inline void set_m_Behaviour_1(NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * value)
	{
		___m_Behaviour_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Behaviour_1), (void*)value);
	}

	inline static int32_t get_offset_of_m_CmdHash_2() { return static_cast<int32_t>(offsetof(SyncList_1_t6931FBA5633802C1CAE04093D0C6D9C88EE66627, ___m_CmdHash_2)); }
	inline int32_t get_m_CmdHash_2() const { return ___m_CmdHash_2; }
	inline int32_t* get_address_of_m_CmdHash_2() { return &___m_CmdHash_2; }
	inline void set_m_CmdHash_2(int32_t value)
	{
		___m_CmdHash_2 = value;
	}

	inline static int32_t get_offset_of_m_Callback_3() { return static_cast<int32_t>(offsetof(SyncList_1_t6931FBA5633802C1CAE04093D0C6D9C88EE66627, ___m_Callback_3)); }
	inline SyncListChanged_t1804C1A09093327076629C515EC006A30F38C1B8 * get_m_Callback_3() const { return ___m_Callback_3; }
	inline SyncListChanged_t1804C1A09093327076629C515EC006A30F38C1B8 ** get_address_of_m_Callback_3() { return &___m_Callback_3; }
	inline void set_m_Callback_3(SyncListChanged_t1804C1A09093327076629C515EC006A30F38C1B8 * value)
	{
		___m_Callback_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Callback_3), (void*)value);
	}
};


// System.ArraySegment`1<System.Byte>
struct  ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA 
{
public:
	// T[] System.ArraySegment`1::_array
	ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ____array_0;
	// System.Int32 System.ArraySegment`1::_offset
	int32_t ____offset_1;
	// System.Int32 System.ArraySegment`1::_count
	int32_t ____count_2;

public:
	inline static int32_t get_offset_of__array_0() { return static_cast<int32_t>(offsetof(ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA, ____array_0)); }
	inline ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* get__array_0() const { return ____array_0; }
	inline ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821** get_address_of__array_0() { return &____array_0; }
	inline void set__array_0(ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* value)
	{
		____array_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&____array_0), (void*)value);
	}

	inline static int32_t get_offset_of__offset_1() { return static_cast<int32_t>(offsetof(ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA, ____offset_1)); }
	inline int32_t get__offset_1() const { return ____offset_1; }
	inline int32_t* get_address_of__offset_1() { return &____offset_1; }
	inline void set__offset_1(int32_t value)
	{
		____offset_1 = value;
	}

	inline static int32_t get_offset_of__count_2() { return static_cast<int32_t>(offsetof(ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA, ____count_2)); }
	inline int32_t get__count_2() const { return ____count_2; }
	inline int32_t* get_address_of__count_2() { return &____count_2; }
	inline void set__count_2(int32_t value)
	{
		____count_2 = value;
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


// System.Decimal
struct  Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8 
{
public:
	// System.Int32 System.Decimal::flags
	int32_t ___flags_14;
	// System.Int32 System.Decimal::hi
	int32_t ___hi_15;
	// System.Int32 System.Decimal::lo
	int32_t ___lo_16;
	// System.Int32 System.Decimal::mid
	int32_t ___mid_17;

public:
	inline static int32_t get_offset_of_flags_14() { return static_cast<int32_t>(offsetof(Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8, ___flags_14)); }
	inline int32_t get_flags_14() const { return ___flags_14; }
	inline int32_t* get_address_of_flags_14() { return &___flags_14; }
	inline void set_flags_14(int32_t value)
	{
		___flags_14 = value;
	}

	inline static int32_t get_offset_of_hi_15() { return static_cast<int32_t>(offsetof(Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8, ___hi_15)); }
	inline int32_t get_hi_15() const { return ___hi_15; }
	inline int32_t* get_address_of_hi_15() { return &___hi_15; }
	inline void set_hi_15(int32_t value)
	{
		___hi_15 = value;
	}

	inline static int32_t get_offset_of_lo_16() { return static_cast<int32_t>(offsetof(Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8, ___lo_16)); }
	inline int32_t get_lo_16() const { return ___lo_16; }
	inline int32_t* get_address_of_lo_16() { return &___lo_16; }
	inline void set_lo_16(int32_t value)
	{
		___lo_16 = value;
	}

	inline static int32_t get_offset_of_mid_17() { return static_cast<int32_t>(offsetof(Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8, ___mid_17)); }
	inline int32_t get_mid_17() const { return ___mid_17; }
	inline int32_t* get_address_of_mid_17() { return &___mid_17; }
	inline void set_mid_17(int32_t value)
	{
		___mid_17 = value;
	}
};

struct Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8_StaticFields
{
public:
	// System.UInt32[] System.Decimal::Powers10
	UInt32U5BU5D_t9AA834AF2940E75BBF8E3F08FF0D20D266DB71CB* ___Powers10_6;
	// System.Decimal System.Decimal::Zero
	Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  ___Zero_7;
	// System.Decimal System.Decimal::One
	Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  ___One_8;
	// System.Decimal System.Decimal::MinusOne
	Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  ___MinusOne_9;
	// System.Decimal System.Decimal::MaxValue
	Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  ___MaxValue_10;
	// System.Decimal System.Decimal::MinValue
	Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  ___MinValue_11;
	// System.Decimal System.Decimal::NearNegativeZero
	Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  ___NearNegativeZero_12;
	// System.Decimal System.Decimal::NearPositiveZero
	Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  ___NearPositiveZero_13;

public:
	inline static int32_t get_offset_of_Powers10_6() { return static_cast<int32_t>(offsetof(Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8_StaticFields, ___Powers10_6)); }
	inline UInt32U5BU5D_t9AA834AF2940E75BBF8E3F08FF0D20D266DB71CB* get_Powers10_6() const { return ___Powers10_6; }
	inline UInt32U5BU5D_t9AA834AF2940E75BBF8E3F08FF0D20D266DB71CB** get_address_of_Powers10_6() { return &___Powers10_6; }
	inline void set_Powers10_6(UInt32U5BU5D_t9AA834AF2940E75BBF8E3F08FF0D20D266DB71CB* value)
	{
		___Powers10_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___Powers10_6), (void*)value);
	}

	inline static int32_t get_offset_of_Zero_7() { return static_cast<int32_t>(offsetof(Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8_StaticFields, ___Zero_7)); }
	inline Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  get_Zero_7() const { return ___Zero_7; }
	inline Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8 * get_address_of_Zero_7() { return &___Zero_7; }
	inline void set_Zero_7(Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  value)
	{
		___Zero_7 = value;
	}

	inline static int32_t get_offset_of_One_8() { return static_cast<int32_t>(offsetof(Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8_StaticFields, ___One_8)); }
	inline Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  get_One_8() const { return ___One_8; }
	inline Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8 * get_address_of_One_8() { return &___One_8; }
	inline void set_One_8(Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  value)
	{
		___One_8 = value;
	}

	inline static int32_t get_offset_of_MinusOne_9() { return static_cast<int32_t>(offsetof(Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8_StaticFields, ___MinusOne_9)); }
	inline Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  get_MinusOne_9() const { return ___MinusOne_9; }
	inline Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8 * get_address_of_MinusOne_9() { return &___MinusOne_9; }
	inline void set_MinusOne_9(Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  value)
	{
		___MinusOne_9 = value;
	}

	inline static int32_t get_offset_of_MaxValue_10() { return static_cast<int32_t>(offsetof(Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8_StaticFields, ___MaxValue_10)); }
	inline Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  get_MaxValue_10() const { return ___MaxValue_10; }
	inline Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8 * get_address_of_MaxValue_10() { return &___MaxValue_10; }
	inline void set_MaxValue_10(Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  value)
	{
		___MaxValue_10 = value;
	}

	inline static int32_t get_offset_of_MinValue_11() { return static_cast<int32_t>(offsetof(Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8_StaticFields, ___MinValue_11)); }
	inline Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  get_MinValue_11() const { return ___MinValue_11; }
	inline Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8 * get_address_of_MinValue_11() { return &___MinValue_11; }
	inline void set_MinValue_11(Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  value)
	{
		___MinValue_11 = value;
	}

	inline static int32_t get_offset_of_NearNegativeZero_12() { return static_cast<int32_t>(offsetof(Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8_StaticFields, ___NearNegativeZero_12)); }
	inline Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  get_NearNegativeZero_12() const { return ___NearNegativeZero_12; }
	inline Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8 * get_address_of_NearNegativeZero_12() { return &___NearNegativeZero_12; }
	inline void set_NearNegativeZero_12(Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  value)
	{
		___NearNegativeZero_12 = value;
	}

	inline static int32_t get_offset_of_NearPositiveZero_13() { return static_cast<int32_t>(offsetof(Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8_StaticFields, ___NearPositiveZero_13)); }
	inline Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  get_NearPositiveZero_13() const { return ___NearPositiveZero_13; }
	inline Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8 * get_address_of_NearPositiveZero_13() { return &___NearPositiveZero_13; }
	inline void set_NearPositiveZero_13(Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  value)
	{
		___NearPositiveZero_13 = value;
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


// System.Text.UTF8Encoding
struct  UTF8Encoding_t77ED103B749A387EF072C3429F48C91D12CA08DE  : public Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4
{
public:
	// System.Boolean System.Text.UTF8Encoding::emitUTF8Identifier
	bool ___emitUTF8Identifier_62;
	// System.Boolean System.Text.UTF8Encoding::isThrowException
	bool ___isThrowException_63;

public:
	inline static int32_t get_offset_of_emitUTF8Identifier_62() { return static_cast<int32_t>(offsetof(UTF8Encoding_t77ED103B749A387EF072C3429F48C91D12CA08DE, ___emitUTF8Identifier_62)); }
	inline bool get_emitUTF8Identifier_62() const { return ___emitUTF8Identifier_62; }
	inline bool* get_address_of_emitUTF8Identifier_62() { return &___emitUTF8Identifier_62; }
	inline void set_emitUTF8Identifier_62(bool value)
	{
		___emitUTF8Identifier_62 = value;
	}

	inline static int32_t get_offset_of_isThrowException_63() { return static_cast<int32_t>(offsetof(UTF8Encoding_t77ED103B749A387EF072C3429F48C91D12CA08DE, ___isThrowException_63)); }
	inline bool get_isThrowException_63() const { return ___isThrowException_63; }
	inline bool* get_address_of_isThrowException_63() { return &___isThrowException_63; }
	inline void set_isThrowException_63(bool value)
	{
		___isThrowException_63 = value;
	}
};


// System.UInt16
struct  UInt16_tAE45CEF73BF720100519F6867F32145D075F928E 
{
public:
	// System.UInt16 System.UInt16::m_value
	uint16_t ___m_value_0;

public:
	inline static int32_t get_offset_of_m_value_0() { return static_cast<int32_t>(offsetof(UInt16_tAE45CEF73BF720100519F6867F32145D075F928E, ___m_value_0)); }
	inline uint16_t get_m_value_0() const { return ___m_value_0; }
	inline uint16_t* get_address_of_m_value_0() { return &___m_value_0; }
	inline void set_m_value_0(uint16_t value)
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


// UnityEngine.Color32
struct  Color32_t23ABC4AE0E0BDFD2E22EE1FA0DA3904FFE5F6E23 
{
public:
	union
	{
		#pragma pack(push, tp, 1)
		struct
		{
			// System.Int32 UnityEngine.Color32::rgba
			int32_t ___rgba_0;
		};
		#pragma pack(pop, tp)
		struct
		{
			int32_t ___rgba_0_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			// System.Byte UnityEngine.Color32::r
			uint8_t ___r_1;
		};
		#pragma pack(pop, tp)
		struct
		{
			uint8_t ___r_1_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			char ___g_2_OffsetPadding[1];
			// System.Byte UnityEngine.Color32::g
			uint8_t ___g_2;
		};
		#pragma pack(pop, tp)
		struct
		{
			char ___g_2_OffsetPadding_forAlignmentOnly[1];
			uint8_t ___g_2_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			char ___b_3_OffsetPadding[2];
			// System.Byte UnityEngine.Color32::b
			uint8_t ___b_3;
		};
		#pragma pack(pop, tp)
		struct
		{
			char ___b_3_OffsetPadding_forAlignmentOnly[2];
			uint8_t ___b_3_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			char ___a_4_OffsetPadding[3];
			// System.Byte UnityEngine.Color32::a
			uint8_t ___a_4;
		};
		#pragma pack(pop, tp)
		struct
		{
			char ___a_4_OffsetPadding_forAlignmentOnly[3];
			uint8_t ___a_4_forAlignmentOnly;
		};
	};

public:
	inline static int32_t get_offset_of_rgba_0() { return static_cast<int32_t>(offsetof(Color32_t23ABC4AE0E0BDFD2E22EE1FA0DA3904FFE5F6E23, ___rgba_0)); }
	inline int32_t get_rgba_0() const { return ___rgba_0; }
	inline int32_t* get_address_of_rgba_0() { return &___rgba_0; }
	inline void set_rgba_0(int32_t value)
	{
		___rgba_0 = value;
	}

	inline static int32_t get_offset_of_r_1() { return static_cast<int32_t>(offsetof(Color32_t23ABC4AE0E0BDFD2E22EE1FA0DA3904FFE5F6E23, ___r_1)); }
	inline uint8_t get_r_1() const { return ___r_1; }
	inline uint8_t* get_address_of_r_1() { return &___r_1; }
	inline void set_r_1(uint8_t value)
	{
		___r_1 = value;
	}

	inline static int32_t get_offset_of_g_2() { return static_cast<int32_t>(offsetof(Color32_t23ABC4AE0E0BDFD2E22EE1FA0DA3904FFE5F6E23, ___g_2)); }
	inline uint8_t get_g_2() const { return ___g_2; }
	inline uint8_t* get_address_of_g_2() { return &___g_2; }
	inline void set_g_2(uint8_t value)
	{
		___g_2 = value;
	}

	inline static int32_t get_offset_of_b_3() { return static_cast<int32_t>(offsetof(Color32_t23ABC4AE0E0BDFD2E22EE1FA0DA3904FFE5F6E23, ___b_3)); }
	inline uint8_t get_b_3() const { return ___b_3; }
	inline uint8_t* get_address_of_b_3() { return &___b_3; }
	inline void set_b_3(uint8_t value)
	{
		___b_3 = value;
	}

	inline static int32_t get_offset_of_a_4() { return static_cast<int32_t>(offsetof(Color32_t23ABC4AE0E0BDFD2E22EE1FA0DA3904FFE5F6E23, ___a_4)); }
	inline uint8_t get_a_4() const { return ___a_4; }
	inline uint8_t* get_address_of_a_4() { return &___a_4; }
	inline void set_a_4(uint8_t value)
	{
		___a_4 = value;
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


// UnityEngine.Networking.NetworkHash128
struct  NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C 
{
public:
	// System.Byte UnityEngine.Networking.NetworkHash128::i0
	uint8_t ___i0_0;
	// System.Byte UnityEngine.Networking.NetworkHash128::i1
	uint8_t ___i1_1;
	// System.Byte UnityEngine.Networking.NetworkHash128::i2
	uint8_t ___i2_2;
	// System.Byte UnityEngine.Networking.NetworkHash128::i3
	uint8_t ___i3_3;
	// System.Byte UnityEngine.Networking.NetworkHash128::i4
	uint8_t ___i4_4;
	// System.Byte UnityEngine.Networking.NetworkHash128::i5
	uint8_t ___i5_5;
	// System.Byte UnityEngine.Networking.NetworkHash128::i6
	uint8_t ___i6_6;
	// System.Byte UnityEngine.Networking.NetworkHash128::i7
	uint8_t ___i7_7;
	// System.Byte UnityEngine.Networking.NetworkHash128::i8
	uint8_t ___i8_8;
	// System.Byte UnityEngine.Networking.NetworkHash128::i9
	uint8_t ___i9_9;
	// System.Byte UnityEngine.Networking.NetworkHash128::i10
	uint8_t ___i10_10;
	// System.Byte UnityEngine.Networking.NetworkHash128::i11
	uint8_t ___i11_11;
	// System.Byte UnityEngine.Networking.NetworkHash128::i12
	uint8_t ___i12_12;
	// System.Byte UnityEngine.Networking.NetworkHash128::i13
	uint8_t ___i13_13;
	// System.Byte UnityEngine.Networking.NetworkHash128::i14
	uint8_t ___i14_14;
	// System.Byte UnityEngine.Networking.NetworkHash128::i15
	uint8_t ___i15_15;

public:
	inline static int32_t get_offset_of_i0_0() { return static_cast<int32_t>(offsetof(NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C, ___i0_0)); }
	inline uint8_t get_i0_0() const { return ___i0_0; }
	inline uint8_t* get_address_of_i0_0() { return &___i0_0; }
	inline void set_i0_0(uint8_t value)
	{
		___i0_0 = value;
	}

	inline static int32_t get_offset_of_i1_1() { return static_cast<int32_t>(offsetof(NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C, ___i1_1)); }
	inline uint8_t get_i1_1() const { return ___i1_1; }
	inline uint8_t* get_address_of_i1_1() { return &___i1_1; }
	inline void set_i1_1(uint8_t value)
	{
		___i1_1 = value;
	}

	inline static int32_t get_offset_of_i2_2() { return static_cast<int32_t>(offsetof(NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C, ___i2_2)); }
	inline uint8_t get_i2_2() const { return ___i2_2; }
	inline uint8_t* get_address_of_i2_2() { return &___i2_2; }
	inline void set_i2_2(uint8_t value)
	{
		___i2_2 = value;
	}

	inline static int32_t get_offset_of_i3_3() { return static_cast<int32_t>(offsetof(NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C, ___i3_3)); }
	inline uint8_t get_i3_3() const { return ___i3_3; }
	inline uint8_t* get_address_of_i3_3() { return &___i3_3; }
	inline void set_i3_3(uint8_t value)
	{
		___i3_3 = value;
	}

	inline static int32_t get_offset_of_i4_4() { return static_cast<int32_t>(offsetof(NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C, ___i4_4)); }
	inline uint8_t get_i4_4() const { return ___i4_4; }
	inline uint8_t* get_address_of_i4_4() { return &___i4_4; }
	inline void set_i4_4(uint8_t value)
	{
		___i4_4 = value;
	}

	inline static int32_t get_offset_of_i5_5() { return static_cast<int32_t>(offsetof(NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C, ___i5_5)); }
	inline uint8_t get_i5_5() const { return ___i5_5; }
	inline uint8_t* get_address_of_i5_5() { return &___i5_5; }
	inline void set_i5_5(uint8_t value)
	{
		___i5_5 = value;
	}

	inline static int32_t get_offset_of_i6_6() { return static_cast<int32_t>(offsetof(NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C, ___i6_6)); }
	inline uint8_t get_i6_6() const { return ___i6_6; }
	inline uint8_t* get_address_of_i6_6() { return &___i6_6; }
	inline void set_i6_6(uint8_t value)
	{
		___i6_6 = value;
	}

	inline static int32_t get_offset_of_i7_7() { return static_cast<int32_t>(offsetof(NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C, ___i7_7)); }
	inline uint8_t get_i7_7() const { return ___i7_7; }
	inline uint8_t* get_address_of_i7_7() { return &___i7_7; }
	inline void set_i7_7(uint8_t value)
	{
		___i7_7 = value;
	}

	inline static int32_t get_offset_of_i8_8() { return static_cast<int32_t>(offsetof(NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C, ___i8_8)); }
	inline uint8_t get_i8_8() const { return ___i8_8; }
	inline uint8_t* get_address_of_i8_8() { return &___i8_8; }
	inline void set_i8_8(uint8_t value)
	{
		___i8_8 = value;
	}

	inline static int32_t get_offset_of_i9_9() { return static_cast<int32_t>(offsetof(NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C, ___i9_9)); }
	inline uint8_t get_i9_9() const { return ___i9_9; }
	inline uint8_t* get_address_of_i9_9() { return &___i9_9; }
	inline void set_i9_9(uint8_t value)
	{
		___i9_9 = value;
	}

	inline static int32_t get_offset_of_i10_10() { return static_cast<int32_t>(offsetof(NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C, ___i10_10)); }
	inline uint8_t get_i10_10() const { return ___i10_10; }
	inline uint8_t* get_address_of_i10_10() { return &___i10_10; }
	inline void set_i10_10(uint8_t value)
	{
		___i10_10 = value;
	}

	inline static int32_t get_offset_of_i11_11() { return static_cast<int32_t>(offsetof(NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C, ___i11_11)); }
	inline uint8_t get_i11_11() const { return ___i11_11; }
	inline uint8_t* get_address_of_i11_11() { return &___i11_11; }
	inline void set_i11_11(uint8_t value)
	{
		___i11_11 = value;
	}

	inline static int32_t get_offset_of_i12_12() { return static_cast<int32_t>(offsetof(NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C, ___i12_12)); }
	inline uint8_t get_i12_12() const { return ___i12_12; }
	inline uint8_t* get_address_of_i12_12() { return &___i12_12; }
	inline void set_i12_12(uint8_t value)
	{
		___i12_12 = value;
	}

	inline static int32_t get_offset_of_i13_13() { return static_cast<int32_t>(offsetof(NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C, ___i13_13)); }
	inline uint8_t get_i13_13() const { return ___i13_13; }
	inline uint8_t* get_address_of_i13_13() { return &___i13_13; }
	inline void set_i13_13(uint8_t value)
	{
		___i13_13 = value;
	}

	inline static int32_t get_offset_of_i14_14() { return static_cast<int32_t>(offsetof(NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C, ___i14_14)); }
	inline uint8_t get_i14_14() const { return ___i14_14; }
	inline uint8_t* get_address_of_i14_14() { return &___i14_14; }
	inline void set_i14_14(uint8_t value)
	{
		___i14_14 = value;
	}

	inline static int32_t get_offset_of_i15_15() { return static_cast<int32_t>(offsetof(NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C, ___i15_15)); }
	inline uint8_t get_i15_15() const { return ___i15_15; }
	inline uint8_t* get_address_of_i15_15() { return &___i15_15; }
	inline void set_i15_15(uint8_t value)
	{
		___i15_15 = value;
	}
};


// UnityEngine.Networking.NetworkInstanceId
struct  NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615 
{
public:
	// System.UInt32 UnityEngine.Networking.NetworkInstanceId::m_Value
	uint32_t ___m_Value_0;

public:
	inline static int32_t get_offset_of_m_Value_0() { return static_cast<int32_t>(offsetof(NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615, ___m_Value_0)); }
	inline uint32_t get_m_Value_0() const { return ___m_Value_0; }
	inline uint32_t* get_address_of_m_Value_0() { return &___m_Value_0; }
	inline void set_m_Value_0(uint32_t value)
	{
		___m_Value_0 = value;
	}
};

struct NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615_StaticFields
{
public:
	// UnityEngine.Networking.NetworkInstanceId UnityEngine.Networking.NetworkInstanceId::Invalid
	NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  ___Invalid_1;
	// UnityEngine.Networking.NetworkInstanceId UnityEngine.Networking.NetworkInstanceId::Zero
	NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  ___Zero_2;

public:
	inline static int32_t get_offset_of_Invalid_1() { return static_cast<int32_t>(offsetof(NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615_StaticFields, ___Invalid_1)); }
	inline NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  get_Invalid_1() const { return ___Invalid_1; }
	inline NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615 * get_address_of_Invalid_1() { return &___Invalid_1; }
	inline void set_Invalid_1(NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  value)
	{
		___Invalid_1 = value;
	}

	inline static int32_t get_offset_of_Zero_2() { return static_cast<int32_t>(offsetof(NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615_StaticFields, ___Zero_2)); }
	inline NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  get_Zero_2() const { return ___Zero_2; }
	inline NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615 * get_address_of_Zero_2() { return &___Zero_2; }
	inline void set_Zero_2(NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  value)
	{
		___Zero_2 = value;
	}
};


// UnityEngine.Networking.NetworkSceneId
struct  NetworkSceneId_t462EC62A23A1B7AF60637C48CD916A09BC493340 
{
public:
	// System.UInt32 UnityEngine.Networking.NetworkSceneId::m_Value
	uint32_t ___m_Value_0;

public:
	inline static int32_t get_offset_of_m_Value_0() { return static_cast<int32_t>(offsetof(NetworkSceneId_t462EC62A23A1B7AF60637C48CD916A09BC493340, ___m_Value_0)); }
	inline uint32_t get_m_Value_0() const { return ___m_Value_0; }
	inline uint32_t* get_address_of_m_Value_0() { return &___m_Value_0; }
	inline void set_m_Value_0(uint32_t value)
	{
		___m_Value_0 = value;
	}
};


// UnityEngine.Networking.ServerAttribute
struct  ServerAttribute_tBEAD82CF18B52F903FB105CC54E39C66B82E079D  : public Attribute_tF048C13FB3C8CFCC53F82290E4A3F621089F9A74
{
public:

public:
};


// UnityEngine.Networking.ServerCallbackAttribute
struct  ServerCallbackAttribute_tD2D226910AED65FFCB395293D6A4235FE08BCF0F  : public Attribute_tF048C13FB3C8CFCC53F82290E4A3F621089F9A74
{
public:

public:
};


// UnityEngine.Networking.SyncEventAttribute
struct  SyncEventAttribute_t32B6E9C1595BB49337BC42619BB697C84790630E  : public Attribute_tF048C13FB3C8CFCC53F82290E4A3F621089F9A74
{
public:
	// System.Int32 UnityEngine.Networking.SyncEventAttribute::channel
	int32_t ___channel_0;

public:
	inline static int32_t get_offset_of_channel_0() { return static_cast<int32_t>(offsetof(SyncEventAttribute_t32B6E9C1595BB49337BC42619BB697C84790630E, ___channel_0)); }
	inline int32_t get_channel_0() const { return ___channel_0; }
	inline int32_t* get_address_of_channel_0() { return &___channel_0; }
	inline void set_channel_0(int32_t value)
	{
		___channel_0 = value;
	}
};


// UnityEngine.Networking.SyncListBool
struct  SyncListBool_t4530597403BBB668F776B32DE46A1A91623EE054  : public SyncList_1_tDDF00E08E649A86264E50205CB99495D1AD2E8D0
{
public:

public:
};


// UnityEngine.Networking.SyncListFloat
struct  SyncListFloat_tC8F12C17B783518D34953712B51249276C506922  : public SyncList_1_tDEB03E3C5252571915662095C7060998910FD0A2
{
public:

public:
};


// UnityEngine.Networking.SyncListInt
struct  SyncListInt_t6D5125D26D629A9DB1325266BCFCDF2FC86FD9C6  : public SyncList_1_t8595ACD08C8686AC9547A225ACF209C171FF0665
{
public:

public:
};


// UnityEngine.Networking.SyncListString
struct  SyncListString_t26D5186F91FB985D01BDC6CC5B4C7C13FA3740CD  : public SyncList_1_t4C2B8FAE3D901E39D62A3678DF05B8473364ED04
{
public:

public:
};


// UnityEngine.Networking.SyncListUInt
struct  SyncListUInt_tF223A88F804D7F2819F5F610669176CE6E93A0E6  : public SyncList_1_t6931FBA5633802C1CAE04093D0C6D9C88EE66627
{
public:

public:
};


// UnityEngine.Networking.SyncVarAttribute
struct  SyncVarAttribute_tD57FE395DED8D547F0200B7F50F36DFA27C6BF3A  : public Attribute_tF048C13FB3C8CFCC53F82290E4A3F621089F9A74
{
public:
	// System.String UnityEngine.Networking.SyncVarAttribute::hook
	String_t* ___hook_0;

public:
	inline static int32_t get_offset_of_hook_0() { return static_cast<int32_t>(offsetof(SyncVarAttribute_tD57FE395DED8D547F0200B7F50F36DFA27C6BF3A, ___hook_0)); }
	inline String_t* get_hook_0() const { return ___hook_0; }
	inline String_t** get_address_of_hook_0() { return &___hook_0; }
	inline void set_hook_0(String_t* value)
	{
		___hook_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___hook_0), (void*)value);
	}
};


// UnityEngine.Networking.TargetRpcAttribute
struct  TargetRpcAttribute_t7B515CB5DD6D609483DFC4ACC89D00B00C9EAE03  : public Attribute_tF048C13FB3C8CFCC53F82290E4A3F621089F9A74
{
public:
	// System.Int32 UnityEngine.Networking.TargetRpcAttribute::channel
	int32_t ___channel_0;

public:
	inline static int32_t get_offset_of_channel_0() { return static_cast<int32_t>(offsetof(TargetRpcAttribute_t7B515CB5DD6D609483DFC4ACC89D00B00C9EAE03, ___channel_0)); }
	inline int32_t get_channel_0() const { return ___channel_0; }
	inline int32_t* get_address_of_channel_0() { return &___channel_0; }
	inline void set_channel_0(int32_t value)
	{
		___channel_0 = value;
	}
};


// UnityEngine.Networking.UIntFloat
struct  UIntFloat_tFF4D5273EEDE59506E38E1C3A3932139C4EACBE0 
{
public:
	union
	{
		#pragma pack(push, tp, 1)
		struct
		{
			// System.Single UnityEngine.Networking.UIntFloat::floatValue
			float ___floatValue_0;
		};
		#pragma pack(pop, tp)
		struct
		{
			float ___floatValue_0_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			// System.UInt32 UnityEngine.Networking.UIntFloat::intValue
			uint32_t ___intValue_1;
		};
		#pragma pack(pop, tp)
		struct
		{
			uint32_t ___intValue_1_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			// System.Double UnityEngine.Networking.UIntFloat::doubleValue
			double ___doubleValue_2;
		};
		#pragma pack(pop, tp)
		struct
		{
			double ___doubleValue_2_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			// System.UInt64 UnityEngine.Networking.UIntFloat::longValue
			uint64_t ___longValue_3;
		};
		#pragma pack(pop, tp)
		struct
		{
			uint64_t ___longValue_3_forAlignmentOnly;
		};
	};

public:
	inline static int32_t get_offset_of_floatValue_0() { return static_cast<int32_t>(offsetof(UIntFloat_tFF4D5273EEDE59506E38E1C3A3932139C4EACBE0, ___floatValue_0)); }
	inline float get_floatValue_0() const { return ___floatValue_0; }
	inline float* get_address_of_floatValue_0() { return &___floatValue_0; }
	inline void set_floatValue_0(float value)
	{
		___floatValue_0 = value;
	}

	inline static int32_t get_offset_of_intValue_1() { return static_cast<int32_t>(offsetof(UIntFloat_tFF4D5273EEDE59506E38E1C3A3932139C4EACBE0, ___intValue_1)); }
	inline uint32_t get_intValue_1() const { return ___intValue_1; }
	inline uint32_t* get_address_of_intValue_1() { return &___intValue_1; }
	inline void set_intValue_1(uint32_t value)
	{
		___intValue_1 = value;
	}

	inline static int32_t get_offset_of_doubleValue_2() { return static_cast<int32_t>(offsetof(UIntFloat_tFF4D5273EEDE59506E38E1C3A3932139C4EACBE0, ___doubleValue_2)); }
	inline double get_doubleValue_2() const { return ___doubleValue_2; }
	inline double* get_address_of_doubleValue_2() { return &___doubleValue_2; }
	inline void set_doubleValue_2(double value)
	{
		___doubleValue_2 = value;
	}

	inline static int32_t get_offset_of_longValue_3() { return static_cast<int32_t>(offsetof(UIntFloat_tFF4D5273EEDE59506E38E1C3A3932139C4EACBE0, ___longValue_3)); }
	inline uint64_t get_longValue_3() const { return ___longValue_3; }
	inline uint64_t* get_address_of_longValue_3() { return &___longValue_3; }
	inline void set_longValue_3(uint64_t value)
	{
		___longValue_3 = value;
	}
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


// UnityEngine.Rect
struct  Rect_t35B976DE901B5423C11705E156938EA27AB402CE 
{
public:
	// System.Single UnityEngine.Rect::m_XMin
	float ___m_XMin_0;
	// System.Single UnityEngine.Rect::m_YMin
	float ___m_YMin_1;
	// System.Single UnityEngine.Rect::m_Width
	float ___m_Width_2;
	// System.Single UnityEngine.Rect::m_Height
	float ___m_Height_3;

public:
	inline static int32_t get_offset_of_m_XMin_0() { return static_cast<int32_t>(offsetof(Rect_t35B976DE901B5423C11705E156938EA27AB402CE, ___m_XMin_0)); }
	inline float get_m_XMin_0() const { return ___m_XMin_0; }
	inline float* get_address_of_m_XMin_0() { return &___m_XMin_0; }
	inline void set_m_XMin_0(float value)
	{
		___m_XMin_0 = value;
	}

	inline static int32_t get_offset_of_m_YMin_1() { return static_cast<int32_t>(offsetof(Rect_t35B976DE901B5423C11705E156938EA27AB402CE, ___m_YMin_1)); }
	inline float get_m_YMin_1() const { return ___m_YMin_1; }
	inline float* get_address_of_m_YMin_1() { return &___m_YMin_1; }
	inline void set_m_YMin_1(float value)
	{
		___m_YMin_1 = value;
	}

	inline static int32_t get_offset_of_m_Width_2() { return static_cast<int32_t>(offsetof(Rect_t35B976DE901B5423C11705E156938EA27AB402CE, ___m_Width_2)); }
	inline float get_m_Width_2() const { return ___m_Width_2; }
	inline float* get_address_of_m_Width_2() { return &___m_Width_2; }
	inline void set_m_Width_2(float value)
	{
		___m_Width_2 = value;
	}

	inline static int32_t get_offset_of_m_Height_3() { return static_cast<int32_t>(offsetof(Rect_t35B976DE901B5423C11705E156938EA27AB402CE, ___m_Height_3)); }
	inline float get_m_Height_3() const { return ___m_Height_3; }
	inline float* get_address_of_m_Height_3() { return &___m_Height_3; }
	inline void set_m_Height_3(float value)
	{
		___m_Height_3 = value;
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


// UnityEngine.Vector4
struct  Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E 
{
public:
	// System.Single UnityEngine.Vector4::x
	float ___x_1;
	// System.Single UnityEngine.Vector4::y
	float ___y_2;
	// System.Single UnityEngine.Vector4::z
	float ___z_3;
	// System.Single UnityEngine.Vector4::w
	float ___w_4;

public:
	inline static int32_t get_offset_of_x_1() { return static_cast<int32_t>(offsetof(Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E, ___x_1)); }
	inline float get_x_1() const { return ___x_1; }
	inline float* get_address_of_x_1() { return &___x_1; }
	inline void set_x_1(float value)
	{
		___x_1 = value;
	}

	inline static int32_t get_offset_of_y_2() { return static_cast<int32_t>(offsetof(Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E, ___y_2)); }
	inline float get_y_2() const { return ___y_2; }
	inline float* get_address_of_y_2() { return &___y_2; }
	inline void set_y_2(float value)
	{
		___y_2 = value;
	}

	inline static int32_t get_offset_of_z_3() { return static_cast<int32_t>(offsetof(Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E, ___z_3)); }
	inline float get_z_3() const { return ___z_3; }
	inline float* get_address_of_z_3() { return &___z_3; }
	inline void set_z_3(float value)
	{
		___z_3 = value;
	}

	inline static int32_t get_offset_of_w_4() { return static_cast<int32_t>(offsetof(Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E, ___w_4)); }
	inline float get_w_4() const { return ___w_4; }
	inline float* get_address_of_w_4() { return &___w_4; }
	inline void set_w_4(float value)
	{
		___w_4 = value;
	}
};

struct Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E_StaticFields
{
public:
	// UnityEngine.Vector4 UnityEngine.Vector4::zeroVector
	Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E  ___zeroVector_5;
	// UnityEngine.Vector4 UnityEngine.Vector4::oneVector
	Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E  ___oneVector_6;
	// UnityEngine.Vector4 UnityEngine.Vector4::positiveInfinityVector
	Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E  ___positiveInfinityVector_7;
	// UnityEngine.Vector4 UnityEngine.Vector4::negativeInfinityVector
	Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E  ___negativeInfinityVector_8;

public:
	inline static int32_t get_offset_of_zeroVector_5() { return static_cast<int32_t>(offsetof(Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E_StaticFields, ___zeroVector_5)); }
	inline Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E  get_zeroVector_5() const { return ___zeroVector_5; }
	inline Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E * get_address_of_zeroVector_5() { return &___zeroVector_5; }
	inline void set_zeroVector_5(Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E  value)
	{
		___zeroVector_5 = value;
	}

	inline static int32_t get_offset_of_oneVector_6() { return static_cast<int32_t>(offsetof(Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E_StaticFields, ___oneVector_6)); }
	inline Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E  get_oneVector_6() const { return ___oneVector_6; }
	inline Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E * get_address_of_oneVector_6() { return &___oneVector_6; }
	inline void set_oneVector_6(Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E  value)
	{
		___oneVector_6 = value;
	}

	inline static int32_t get_offset_of_positiveInfinityVector_7() { return static_cast<int32_t>(offsetof(Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E_StaticFields, ___positiveInfinityVector_7)); }
	inline Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E  get_positiveInfinityVector_7() const { return ___positiveInfinityVector_7; }
	inline Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E * get_address_of_positiveInfinityVector_7() { return &___positiveInfinityVector_7; }
	inline void set_positiveInfinityVector_7(Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E  value)
	{
		___positiveInfinityVector_7 = value;
	}

	inline static int32_t get_offset_of_negativeInfinityVector_8() { return static_cast<int32_t>(offsetof(Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E_StaticFields, ___negativeInfinityVector_8)); }
	inline Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E  get_negativeInfinityVector_8() const { return ___negativeInfinityVector_8; }
	inline Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E * get_address_of_negativeInfinityVector_8() { return &___negativeInfinityVector_8; }
	inline void set_negativeInfinityVector_8(Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E  value)
	{
		___negativeInfinityVector_8 = value;
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

// UnityEngine.HideFlags
struct  HideFlags_t30B57DC00548E963A569318C8F4A4123E7447E37 
{
public:
	// System.Int32 UnityEngine.HideFlags::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(HideFlags_t30B57DC00548E963A569318C8F4A4123E7447E37, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.Networking.NetworkClient_ConnectState
struct  ConnectState_t36DA0E4226FF2170489050E79863831D4DBCB31A 
{
public:
	// System.Int32 UnityEngine.Networking.NetworkClient_ConnectState::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(ConnectState_t36DA0E4226FF2170489050E79863831D4DBCB31A, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.Networking.NetworkError
struct  NetworkError_t2F4C5EEB3EF2313DB6E035334EC2D73885BDEDEC 
{
public:
	// System.Int32 UnityEngine.Networking.NetworkError::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(NetworkError_t2F4C5EEB3EF2313DB6E035334EC2D73885BDEDEC, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.Networking.NetworkTransform_AxisSyncMode
struct  AxisSyncMode_t7B67E618C9CD1CB4220FD948F48C726FABC17180 
{
public:
	// System.Int32 UnityEngine.Networking.NetworkTransform_AxisSyncMode::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(AxisSyncMode_t7B67E618C9CD1CB4220FD948F48C726FABC17180, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.Networking.NetworkTransform_CompressionSyncMode
struct  CompressionSyncMode_tD1EE574334396715F61215037D3E1AB774643637 
{
public:
	// System.Int32 UnityEngine.Networking.NetworkTransform_CompressionSyncMode::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(CompressionSyncMode_tD1EE574334396715F61215037D3E1AB774643637, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.Networking.NetworkTransform_TransformSyncMode
struct  TransformSyncMode_t9BD7164F921F880A2F0B9D8A7F9EB91DD5475637 
{
public:
	// System.Int32 UnityEngine.Networking.NetworkTransform_TransformSyncMode::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(TransformSyncMode_t9BD7164F921F880A2F0B9D8A7F9EB91DD5475637, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.Networking.NetworkWriter
struct  NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030  : public RuntimeObject
{
public:
	// UnityEngine.Networking.NetBuffer UnityEngine.Networking.NetworkWriter::m_Buffer
	NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * ___m_Buffer_1;

public:
	inline static int32_t get_offset_of_m_Buffer_1() { return static_cast<int32_t>(offsetof(NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030, ___m_Buffer_1)); }
	inline NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * get_m_Buffer_1() const { return ___m_Buffer_1; }
	inline NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C ** get_address_of_m_Buffer_1() { return &___m_Buffer_1; }
	inline void set_m_Buffer_1(NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * value)
	{
		___m_Buffer_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Buffer_1), (void*)value);
	}
};

struct NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_StaticFields
{
public:
	// System.Text.Encoding UnityEngine.Networking.NetworkWriter::s_Encoding
	Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * ___s_Encoding_2;
	// System.Byte[] UnityEngine.Networking.NetworkWriter::s_StringWriteBuffer
	ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___s_StringWriteBuffer_3;
	// UnityEngine.Networking.UIntFloat UnityEngine.Networking.NetworkWriter::s_FloatConverter
	UIntFloat_tFF4D5273EEDE59506E38E1C3A3932139C4EACBE0  ___s_FloatConverter_4;

public:
	inline static int32_t get_offset_of_s_Encoding_2() { return static_cast<int32_t>(offsetof(NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_StaticFields, ___s_Encoding_2)); }
	inline Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * get_s_Encoding_2() const { return ___s_Encoding_2; }
	inline Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 ** get_address_of_s_Encoding_2() { return &___s_Encoding_2; }
	inline void set_s_Encoding_2(Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * value)
	{
		___s_Encoding_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_Encoding_2), (void*)value);
	}

	inline static int32_t get_offset_of_s_StringWriteBuffer_3() { return static_cast<int32_t>(offsetof(NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_StaticFields, ___s_StringWriteBuffer_3)); }
	inline ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* get_s_StringWriteBuffer_3() const { return ___s_StringWriteBuffer_3; }
	inline ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821** get_address_of_s_StringWriteBuffer_3() { return &___s_StringWriteBuffer_3; }
	inline void set_s_StringWriteBuffer_3(ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* value)
	{
		___s_StringWriteBuffer_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_StringWriteBuffer_3), (void*)value);
	}

	inline static int32_t get_offset_of_s_FloatConverter_4() { return static_cast<int32_t>(offsetof(NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_StaticFields, ___s_FloatConverter_4)); }
	inline UIntFloat_tFF4D5273EEDE59506E38E1C3A3932139C4EACBE0  get_s_FloatConverter_4() const { return ___s_FloatConverter_4; }
	inline UIntFloat_tFF4D5273EEDE59506E38E1C3A3932139C4EACBE0 * get_address_of_s_FloatConverter_4() { return &___s_FloatConverter_4; }
	inline void set_s_FloatConverter_4(UIntFloat_tFF4D5273EEDE59506E38E1C3A3932139C4EACBE0  value)
	{
		___s_FloatConverter_4 = value;
	}
};


// UnityEngine.Networking.PlayerSpawnMethod
struct  PlayerSpawnMethod_t725CDF579B025362E6E2A5B555009ECDC185AA1E 
{
public:
	// System.Int32 UnityEngine.Networking.PlayerSpawnMethod::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(PlayerSpawnMethod_t725CDF579B025362E6E2A5B555009ECDC185AA1E, ___value___2)); }
	inline int32_t get_value___2() const { return ___value___2; }
	inline int32_t* get_address_of_value___2() { return &___value___2; }
	inline void set_value___2(int32_t value)
	{
		___value___2 = value;
	}
};


// UnityEngine.Networking.UIntDecimal
struct  UIntDecimal_t7F0C0B7CE4190086DE8B448A03A75C133BA118BA 
{
public:
	union
	{
		#pragma pack(push, tp, 1)
		struct
		{
			// System.UInt64 UnityEngine.Networking.UIntDecimal::longValue1
			uint64_t ___longValue1_0;
		};
		#pragma pack(pop, tp)
		struct
		{
			uint64_t ___longValue1_0_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			char ___longValue2_1_OffsetPadding[8];
			// System.UInt64 UnityEngine.Networking.UIntDecimal::longValue2
			uint64_t ___longValue2_1;
		};
		#pragma pack(pop, tp)
		struct
		{
			char ___longValue2_1_OffsetPadding_forAlignmentOnly[8];
			uint64_t ___longValue2_1_forAlignmentOnly;
		};
		#pragma pack(push, tp, 1)
		struct
		{
			// System.Decimal UnityEngine.Networking.UIntDecimal::decimalValue
			Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  ___decimalValue_2;
		};
		#pragma pack(pop, tp)
		struct
		{
			Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  ___decimalValue_2_forAlignmentOnly;
		};
	};

public:
	inline static int32_t get_offset_of_longValue1_0() { return static_cast<int32_t>(offsetof(UIntDecimal_t7F0C0B7CE4190086DE8B448A03A75C133BA118BA, ___longValue1_0)); }
	inline uint64_t get_longValue1_0() const { return ___longValue1_0; }
	inline uint64_t* get_address_of_longValue1_0() { return &___longValue1_0; }
	inline void set_longValue1_0(uint64_t value)
	{
		___longValue1_0 = value;
	}

	inline static int32_t get_offset_of_longValue2_1() { return static_cast<int32_t>(offsetof(UIntDecimal_t7F0C0B7CE4190086DE8B448A03A75C133BA118BA, ___longValue2_1)); }
	inline uint64_t get_longValue2_1() const { return ___longValue2_1; }
	inline uint64_t* get_address_of_longValue2_1() { return &___longValue2_1; }
	inline void set_longValue2_1(uint64_t value)
	{
		___longValue2_1 = value;
	}

	inline static int32_t get_offset_of_decimalValue_2() { return static_cast<int32_t>(offsetof(UIntDecimal_t7F0C0B7CE4190086DE8B448A03A75C133BA118BA, ___decimalValue_2)); }
	inline Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  get_decimalValue_2() const { return ___decimalValue_2; }
	inline Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8 * get_address_of_decimalValue_2() { return &___decimalValue_2; }
	inline void set_decimalValue_2(Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  value)
	{
		___decimalValue_2 = value;
	}
};


// UnityEngine.Networking.Version
struct  Version_tD63B9838033C89FE5C3252E9CB83CA44B92D63CD 
{
public:
	// System.Int32 UnityEngine.Networking.Version::value__
	int32_t ___value___2;

public:
	inline static int32_t get_offset_of_value___2() { return static_cast<int32_t>(offsetof(Version_tD63B9838033C89FE5C3252E9CB83CA44B92D63CD, ___value___2)); }
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

// UnityEngine.Plane
struct  Plane_t0903921088DEEDE1BCDEA5BF279EDBCFC9679AED 
{
public:
	// UnityEngine.Vector3 UnityEngine.Plane::m_Normal
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___m_Normal_0;
	// System.Single UnityEngine.Plane::m_Distance
	float ___m_Distance_1;

public:
	inline static int32_t get_offset_of_m_Normal_0() { return static_cast<int32_t>(offsetof(Plane_t0903921088DEEDE1BCDEA5BF279EDBCFC9679AED, ___m_Normal_0)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_m_Normal_0() const { return ___m_Normal_0; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_m_Normal_0() { return &___m_Normal_0; }
	inline void set_m_Normal_0(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___m_Normal_0 = value;
	}

	inline static int32_t get_offset_of_m_Distance_1() { return static_cast<int32_t>(offsetof(Plane_t0903921088DEEDE1BCDEA5BF279EDBCFC9679AED, ___m_Distance_1)); }
	inline float get_m_Distance_1() const { return ___m_Distance_1; }
	inline float* get_address_of_m_Distance_1() { return &___m_Distance_1; }
	inline void set_m_Distance_1(float value)
	{
		___m_Distance_1 = value;
	}
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


// UnityEngine.Material
struct  Material_tF7DB3BF0C24DEC2FE0CB51E5DF5053D5223C8598  : public Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0
{
public:

public:
};


// UnityEngine.Networking.NetworkClient
struct  NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F  : public RuntimeObject
{
public:
	// System.Type UnityEngine.Networking.NetworkClient::m_NetworkConnectionClass
	Type_t * ___m_NetworkConnectionClass_0;
	// UnityEngine.Networking.HostTopology UnityEngine.Networking.NetworkClient::m_HostTopology
	HostTopology_tD01D253330A0DAA736EDFC67EE9585C363FA9B0E * ___m_HostTopology_4;
	// System.Int32 UnityEngine.Networking.NetworkClient::m_HostPort
	int32_t ___m_HostPort_5;
	// System.Boolean UnityEngine.Networking.NetworkClient::m_UseSimulator
	bool ___m_UseSimulator_6;
	// System.Int32 UnityEngine.Networking.NetworkClient::m_SimulatedLatency
	int32_t ___m_SimulatedLatency_7;
	// System.Single UnityEngine.Networking.NetworkClient::m_PacketLoss
	float ___m_PacketLoss_8;
	// System.String UnityEngine.Networking.NetworkClient::m_ServerIp
	String_t* ___m_ServerIp_9;
	// System.Int32 UnityEngine.Networking.NetworkClient::m_ServerPort
	int32_t ___m_ServerPort_10;
	// System.Int32 UnityEngine.Networking.NetworkClient::m_ClientId
	int32_t ___m_ClientId_11;
	// System.Int32 UnityEngine.Networking.NetworkClient::m_ClientConnectionId
	int32_t ___m_ClientConnectionId_12;
	// System.Int32 UnityEngine.Networking.NetworkClient::m_StatResetTime
	int32_t ___m_StatResetTime_13;
	// System.Net.EndPoint UnityEngine.Networking.NetworkClient::m_RemoteEndPoint
	EndPoint_tD87FCEF2780A951E8CE8D808C345FBF2C088D980 * ___m_RemoteEndPoint_14;
	// UnityEngine.Networking.NetworkMessageHandlers UnityEngine.Networking.NetworkClient::m_MessageHandlers
	NetworkMessageHandlers_tA7BB2E51BDBD8ECE976AD44F1B634F40EA9807D4 * ___m_MessageHandlers_16;
	// UnityEngine.Networking.NetworkConnection UnityEngine.Networking.NetworkClient::m_Connection
	NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * ___m_Connection_17;
	// System.Byte[] UnityEngine.Networking.NetworkClient::m_MsgBuffer
	ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___m_MsgBuffer_18;
	// UnityEngine.Networking.NetworkReader UnityEngine.Networking.NetworkClient::m_MsgReader
	NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * ___m_MsgReader_19;
	// UnityEngine.Networking.NetworkClient_ConnectState UnityEngine.Networking.NetworkClient::m_AsyncConnect
	int32_t ___m_AsyncConnect_20;
	// System.String UnityEngine.Networking.NetworkClient::m_RequestedServerHost
	String_t* ___m_RequestedServerHost_21;

public:
	inline static int32_t get_offset_of_m_NetworkConnectionClass_0() { return static_cast<int32_t>(offsetof(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F, ___m_NetworkConnectionClass_0)); }
	inline Type_t * get_m_NetworkConnectionClass_0() const { return ___m_NetworkConnectionClass_0; }
	inline Type_t ** get_address_of_m_NetworkConnectionClass_0() { return &___m_NetworkConnectionClass_0; }
	inline void set_m_NetworkConnectionClass_0(Type_t * value)
	{
		___m_NetworkConnectionClass_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_NetworkConnectionClass_0), (void*)value);
	}

	inline static int32_t get_offset_of_m_HostTopology_4() { return static_cast<int32_t>(offsetof(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F, ___m_HostTopology_4)); }
	inline HostTopology_tD01D253330A0DAA736EDFC67EE9585C363FA9B0E * get_m_HostTopology_4() const { return ___m_HostTopology_4; }
	inline HostTopology_tD01D253330A0DAA736EDFC67EE9585C363FA9B0E ** get_address_of_m_HostTopology_4() { return &___m_HostTopology_4; }
	inline void set_m_HostTopology_4(HostTopology_tD01D253330A0DAA736EDFC67EE9585C363FA9B0E * value)
	{
		___m_HostTopology_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_HostTopology_4), (void*)value);
	}

	inline static int32_t get_offset_of_m_HostPort_5() { return static_cast<int32_t>(offsetof(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F, ___m_HostPort_5)); }
	inline int32_t get_m_HostPort_5() const { return ___m_HostPort_5; }
	inline int32_t* get_address_of_m_HostPort_5() { return &___m_HostPort_5; }
	inline void set_m_HostPort_5(int32_t value)
	{
		___m_HostPort_5 = value;
	}

	inline static int32_t get_offset_of_m_UseSimulator_6() { return static_cast<int32_t>(offsetof(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F, ___m_UseSimulator_6)); }
	inline bool get_m_UseSimulator_6() const { return ___m_UseSimulator_6; }
	inline bool* get_address_of_m_UseSimulator_6() { return &___m_UseSimulator_6; }
	inline void set_m_UseSimulator_6(bool value)
	{
		___m_UseSimulator_6 = value;
	}

	inline static int32_t get_offset_of_m_SimulatedLatency_7() { return static_cast<int32_t>(offsetof(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F, ___m_SimulatedLatency_7)); }
	inline int32_t get_m_SimulatedLatency_7() const { return ___m_SimulatedLatency_7; }
	inline int32_t* get_address_of_m_SimulatedLatency_7() { return &___m_SimulatedLatency_7; }
	inline void set_m_SimulatedLatency_7(int32_t value)
	{
		___m_SimulatedLatency_7 = value;
	}

	inline static int32_t get_offset_of_m_PacketLoss_8() { return static_cast<int32_t>(offsetof(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F, ___m_PacketLoss_8)); }
	inline float get_m_PacketLoss_8() const { return ___m_PacketLoss_8; }
	inline float* get_address_of_m_PacketLoss_8() { return &___m_PacketLoss_8; }
	inline void set_m_PacketLoss_8(float value)
	{
		___m_PacketLoss_8 = value;
	}

	inline static int32_t get_offset_of_m_ServerIp_9() { return static_cast<int32_t>(offsetof(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F, ___m_ServerIp_9)); }
	inline String_t* get_m_ServerIp_9() const { return ___m_ServerIp_9; }
	inline String_t** get_address_of_m_ServerIp_9() { return &___m_ServerIp_9; }
	inline void set_m_ServerIp_9(String_t* value)
	{
		___m_ServerIp_9 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_ServerIp_9), (void*)value);
	}

	inline static int32_t get_offset_of_m_ServerPort_10() { return static_cast<int32_t>(offsetof(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F, ___m_ServerPort_10)); }
	inline int32_t get_m_ServerPort_10() const { return ___m_ServerPort_10; }
	inline int32_t* get_address_of_m_ServerPort_10() { return &___m_ServerPort_10; }
	inline void set_m_ServerPort_10(int32_t value)
	{
		___m_ServerPort_10 = value;
	}

	inline static int32_t get_offset_of_m_ClientId_11() { return static_cast<int32_t>(offsetof(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F, ___m_ClientId_11)); }
	inline int32_t get_m_ClientId_11() const { return ___m_ClientId_11; }
	inline int32_t* get_address_of_m_ClientId_11() { return &___m_ClientId_11; }
	inline void set_m_ClientId_11(int32_t value)
	{
		___m_ClientId_11 = value;
	}

	inline static int32_t get_offset_of_m_ClientConnectionId_12() { return static_cast<int32_t>(offsetof(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F, ___m_ClientConnectionId_12)); }
	inline int32_t get_m_ClientConnectionId_12() const { return ___m_ClientConnectionId_12; }
	inline int32_t* get_address_of_m_ClientConnectionId_12() { return &___m_ClientConnectionId_12; }
	inline void set_m_ClientConnectionId_12(int32_t value)
	{
		___m_ClientConnectionId_12 = value;
	}

	inline static int32_t get_offset_of_m_StatResetTime_13() { return static_cast<int32_t>(offsetof(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F, ___m_StatResetTime_13)); }
	inline int32_t get_m_StatResetTime_13() const { return ___m_StatResetTime_13; }
	inline int32_t* get_address_of_m_StatResetTime_13() { return &___m_StatResetTime_13; }
	inline void set_m_StatResetTime_13(int32_t value)
	{
		___m_StatResetTime_13 = value;
	}

	inline static int32_t get_offset_of_m_RemoteEndPoint_14() { return static_cast<int32_t>(offsetof(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F, ___m_RemoteEndPoint_14)); }
	inline EndPoint_tD87FCEF2780A951E8CE8D808C345FBF2C088D980 * get_m_RemoteEndPoint_14() const { return ___m_RemoteEndPoint_14; }
	inline EndPoint_tD87FCEF2780A951E8CE8D808C345FBF2C088D980 ** get_address_of_m_RemoteEndPoint_14() { return &___m_RemoteEndPoint_14; }
	inline void set_m_RemoteEndPoint_14(EndPoint_tD87FCEF2780A951E8CE8D808C345FBF2C088D980 * value)
	{
		___m_RemoteEndPoint_14 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_RemoteEndPoint_14), (void*)value);
	}

	inline static int32_t get_offset_of_m_MessageHandlers_16() { return static_cast<int32_t>(offsetof(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F, ___m_MessageHandlers_16)); }
	inline NetworkMessageHandlers_tA7BB2E51BDBD8ECE976AD44F1B634F40EA9807D4 * get_m_MessageHandlers_16() const { return ___m_MessageHandlers_16; }
	inline NetworkMessageHandlers_tA7BB2E51BDBD8ECE976AD44F1B634F40EA9807D4 ** get_address_of_m_MessageHandlers_16() { return &___m_MessageHandlers_16; }
	inline void set_m_MessageHandlers_16(NetworkMessageHandlers_tA7BB2E51BDBD8ECE976AD44F1B634F40EA9807D4 * value)
	{
		___m_MessageHandlers_16 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_MessageHandlers_16), (void*)value);
	}

	inline static int32_t get_offset_of_m_Connection_17() { return static_cast<int32_t>(offsetof(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F, ___m_Connection_17)); }
	inline NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * get_m_Connection_17() const { return ___m_Connection_17; }
	inline NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA ** get_address_of_m_Connection_17() { return &___m_Connection_17; }
	inline void set_m_Connection_17(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * value)
	{
		___m_Connection_17 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Connection_17), (void*)value);
	}

	inline static int32_t get_offset_of_m_MsgBuffer_18() { return static_cast<int32_t>(offsetof(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F, ___m_MsgBuffer_18)); }
	inline ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* get_m_MsgBuffer_18() const { return ___m_MsgBuffer_18; }
	inline ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821** get_address_of_m_MsgBuffer_18() { return &___m_MsgBuffer_18; }
	inline void set_m_MsgBuffer_18(ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* value)
	{
		___m_MsgBuffer_18 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_MsgBuffer_18), (void*)value);
	}

	inline static int32_t get_offset_of_m_MsgReader_19() { return static_cast<int32_t>(offsetof(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F, ___m_MsgReader_19)); }
	inline NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * get_m_MsgReader_19() const { return ___m_MsgReader_19; }
	inline NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 ** get_address_of_m_MsgReader_19() { return &___m_MsgReader_19; }
	inline void set_m_MsgReader_19(NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * value)
	{
		___m_MsgReader_19 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_MsgReader_19), (void*)value);
	}

	inline static int32_t get_offset_of_m_AsyncConnect_20() { return static_cast<int32_t>(offsetof(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F, ___m_AsyncConnect_20)); }
	inline int32_t get_m_AsyncConnect_20() const { return ___m_AsyncConnect_20; }
	inline int32_t* get_address_of_m_AsyncConnect_20() { return &___m_AsyncConnect_20; }
	inline void set_m_AsyncConnect_20(int32_t value)
	{
		___m_AsyncConnect_20 = value;
	}

	inline static int32_t get_offset_of_m_RequestedServerHost_21() { return static_cast<int32_t>(offsetof(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F, ___m_RequestedServerHost_21)); }
	inline String_t* get_m_RequestedServerHost_21() const { return ___m_RequestedServerHost_21; }
	inline String_t** get_address_of_m_RequestedServerHost_21() { return &___m_RequestedServerHost_21; }
	inline void set_m_RequestedServerHost_21(String_t* value)
	{
		___m_RequestedServerHost_21 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_RequestedServerHost_21), (void*)value);
	}
};

struct NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F_StaticFields
{
public:
	// System.Collections.Generic.List`1<UnityEngine.Networking.NetworkClient> UnityEngine.Networking.NetworkClient::s_Clients
	List_1_t7816E78619327B971A54376C3C9CDD6E84077D6D * ___s_Clients_2;
	// System.Boolean UnityEngine.Networking.NetworkClient::s_IsActive
	bool ___s_IsActive_3;
	// UnityEngine.Networking.NetworkSystem.CRCMessage UnityEngine.Networking.NetworkClient::s_CRCMessage
	CRCMessage_t7F44D52B267C35387F0D7AD0D9098D579ECF61FA * ___s_CRCMessage_15;

public:
	inline static int32_t get_offset_of_s_Clients_2() { return static_cast<int32_t>(offsetof(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F_StaticFields, ___s_Clients_2)); }
	inline List_1_t7816E78619327B971A54376C3C9CDD6E84077D6D * get_s_Clients_2() const { return ___s_Clients_2; }
	inline List_1_t7816E78619327B971A54376C3C9CDD6E84077D6D ** get_address_of_s_Clients_2() { return &___s_Clients_2; }
	inline void set_s_Clients_2(List_1_t7816E78619327B971A54376C3C9CDD6E84077D6D * value)
	{
		___s_Clients_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_Clients_2), (void*)value);
	}

	inline static int32_t get_offset_of_s_IsActive_3() { return static_cast<int32_t>(offsetof(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F_StaticFields, ___s_IsActive_3)); }
	inline bool get_s_IsActive_3() const { return ___s_IsActive_3; }
	inline bool* get_address_of_s_IsActive_3() { return &___s_IsActive_3; }
	inline void set_s_IsActive_3(bool value)
	{
		___s_IsActive_3 = value;
	}

	inline static int32_t get_offset_of_s_CRCMessage_15() { return static_cast<int32_t>(offsetof(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F_StaticFields, ___s_CRCMessage_15)); }
	inline CRCMessage_t7F44D52B267C35387F0D7AD0D9098D579ECF61FA * get_s_CRCMessage_15() const { return ___s_CRCMessage_15; }
	inline CRCMessage_t7F44D52B267C35387F0D7AD0D9098D579ECF61FA ** get_address_of_s_CRCMessage_15() { return &___s_CRCMessage_15; }
	inline void set_s_CRCMessage_15(CRCMessage_t7F44D52B267C35387F0D7AD0D9098D579ECF61FA * value)
	{
		___s_CRCMessage_15 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_CRCMessage_15), (void*)value);
	}
};


// UnityEngine.Networking.NetworkConnection
struct  NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA  : public RuntimeObject
{
public:
	// UnityEngine.Networking.ChannelBuffer[] UnityEngine.Networking.NetworkConnection::m_Channels
	ChannelBufferU5BU5D_t75CDA99AB4F27F49A1DAA287CF43B1132505E6FA* ___m_Channels_0;
	// System.Collections.Generic.List`1<UnityEngine.Networking.PlayerController> UnityEngine.Networking.NetworkConnection::m_PlayerControllers
	List_1_t44D1B61364FCFEF62067A4726A735856DFDFD545 * ___m_PlayerControllers_1;
	// UnityEngine.Networking.NetworkMessage UnityEngine.Networking.NetworkConnection::m_NetMsg
	NetworkMessage_tCD66E2AE395A185EFE622EBB5497C95F6754685C * ___m_NetMsg_2;
	// System.Collections.Generic.HashSet`1<UnityEngine.Networking.NetworkIdentity> UnityEngine.Networking.NetworkConnection::m_VisList
	HashSet_1_tAFF21BA556217C09A0897CBE50F53A1AD6C24EC1 * ___m_VisList_3;
	// UnityEngine.Networking.NetworkWriter UnityEngine.Networking.NetworkConnection::m_Writer
	NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * ___m_Writer_4;
	// System.Collections.Generic.Dictionary`2<System.Int16,UnityEngine.Networking.NetworkMessageDelegate> UnityEngine.Networking.NetworkConnection::m_MessageHandlersDict
	Dictionary_2_t519615383E326CAA4218E3A39FB706EE903B11C8 * ___m_MessageHandlersDict_5;
	// UnityEngine.Networking.NetworkMessageHandlers UnityEngine.Networking.NetworkConnection::m_MessageHandlers
	NetworkMessageHandlers_tA7BB2E51BDBD8ECE976AD44F1B634F40EA9807D4 * ___m_MessageHandlers_6;
	// System.Collections.Generic.HashSet`1<UnityEngine.Networking.NetworkInstanceId> UnityEngine.Networking.NetworkConnection::m_ClientOwnedObjects
	HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990 * ___m_ClientOwnedObjects_7;
	// UnityEngine.Networking.NetworkMessage UnityEngine.Networking.NetworkConnection::m_MessageInfo
	NetworkMessage_tCD66E2AE395A185EFE622EBB5497C95F6754685C * ___m_MessageInfo_8;
	// UnityEngine.Networking.NetworkError UnityEngine.Networking.NetworkConnection::error
	int32_t ___error_10;
	// System.Int32 UnityEngine.Networking.NetworkConnection::hostId
	int32_t ___hostId_11;
	// System.Int32 UnityEngine.Networking.NetworkConnection::connectionId
	int32_t ___connectionId_12;
	// System.Boolean UnityEngine.Networking.NetworkConnection::isReady
	bool ___isReady_13;
	// System.String UnityEngine.Networking.NetworkConnection::address
	String_t* ___address_14;
	// System.Single UnityEngine.Networking.NetworkConnection::lastMessageTime
	float ___lastMessageTime_15;
	// System.Boolean UnityEngine.Networking.NetworkConnection::logNetworkMessages
	bool ___logNetworkMessages_16;
	// System.Collections.Generic.Dictionary`2<System.Int16,UnityEngine.Networking.NetworkConnection_PacketStat> UnityEngine.Networking.NetworkConnection::m_PacketStats
	Dictionary_2_t3C696EAE739BB0B87CB145AEF2D2B55EA1CAE88F * ___m_PacketStats_17;
	// System.Boolean UnityEngine.Networking.NetworkConnection::m_Disposed
	bool ___m_Disposed_18;

public:
	inline static int32_t get_offset_of_m_Channels_0() { return static_cast<int32_t>(offsetof(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA, ___m_Channels_0)); }
	inline ChannelBufferU5BU5D_t75CDA99AB4F27F49A1DAA287CF43B1132505E6FA* get_m_Channels_0() const { return ___m_Channels_0; }
	inline ChannelBufferU5BU5D_t75CDA99AB4F27F49A1DAA287CF43B1132505E6FA** get_address_of_m_Channels_0() { return &___m_Channels_0; }
	inline void set_m_Channels_0(ChannelBufferU5BU5D_t75CDA99AB4F27F49A1DAA287CF43B1132505E6FA* value)
	{
		___m_Channels_0 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Channels_0), (void*)value);
	}

	inline static int32_t get_offset_of_m_PlayerControllers_1() { return static_cast<int32_t>(offsetof(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA, ___m_PlayerControllers_1)); }
	inline List_1_t44D1B61364FCFEF62067A4726A735856DFDFD545 * get_m_PlayerControllers_1() const { return ___m_PlayerControllers_1; }
	inline List_1_t44D1B61364FCFEF62067A4726A735856DFDFD545 ** get_address_of_m_PlayerControllers_1() { return &___m_PlayerControllers_1; }
	inline void set_m_PlayerControllers_1(List_1_t44D1B61364FCFEF62067A4726A735856DFDFD545 * value)
	{
		___m_PlayerControllers_1 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_PlayerControllers_1), (void*)value);
	}

	inline static int32_t get_offset_of_m_NetMsg_2() { return static_cast<int32_t>(offsetof(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA, ___m_NetMsg_2)); }
	inline NetworkMessage_tCD66E2AE395A185EFE622EBB5497C95F6754685C * get_m_NetMsg_2() const { return ___m_NetMsg_2; }
	inline NetworkMessage_tCD66E2AE395A185EFE622EBB5497C95F6754685C ** get_address_of_m_NetMsg_2() { return &___m_NetMsg_2; }
	inline void set_m_NetMsg_2(NetworkMessage_tCD66E2AE395A185EFE622EBB5497C95F6754685C * value)
	{
		___m_NetMsg_2 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_NetMsg_2), (void*)value);
	}

	inline static int32_t get_offset_of_m_VisList_3() { return static_cast<int32_t>(offsetof(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA, ___m_VisList_3)); }
	inline HashSet_1_tAFF21BA556217C09A0897CBE50F53A1AD6C24EC1 * get_m_VisList_3() const { return ___m_VisList_3; }
	inline HashSet_1_tAFF21BA556217C09A0897CBE50F53A1AD6C24EC1 ** get_address_of_m_VisList_3() { return &___m_VisList_3; }
	inline void set_m_VisList_3(HashSet_1_tAFF21BA556217C09A0897CBE50F53A1AD6C24EC1 * value)
	{
		___m_VisList_3 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_VisList_3), (void*)value);
	}

	inline static int32_t get_offset_of_m_Writer_4() { return static_cast<int32_t>(offsetof(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA, ___m_Writer_4)); }
	inline NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * get_m_Writer_4() const { return ___m_Writer_4; }
	inline NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 ** get_address_of_m_Writer_4() { return &___m_Writer_4; }
	inline void set_m_Writer_4(NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * value)
	{
		___m_Writer_4 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Writer_4), (void*)value);
	}

	inline static int32_t get_offset_of_m_MessageHandlersDict_5() { return static_cast<int32_t>(offsetof(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA, ___m_MessageHandlersDict_5)); }
	inline Dictionary_2_t519615383E326CAA4218E3A39FB706EE903B11C8 * get_m_MessageHandlersDict_5() const { return ___m_MessageHandlersDict_5; }
	inline Dictionary_2_t519615383E326CAA4218E3A39FB706EE903B11C8 ** get_address_of_m_MessageHandlersDict_5() { return &___m_MessageHandlersDict_5; }
	inline void set_m_MessageHandlersDict_5(Dictionary_2_t519615383E326CAA4218E3A39FB706EE903B11C8 * value)
	{
		___m_MessageHandlersDict_5 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_MessageHandlersDict_5), (void*)value);
	}

	inline static int32_t get_offset_of_m_MessageHandlers_6() { return static_cast<int32_t>(offsetof(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA, ___m_MessageHandlers_6)); }
	inline NetworkMessageHandlers_tA7BB2E51BDBD8ECE976AD44F1B634F40EA9807D4 * get_m_MessageHandlers_6() const { return ___m_MessageHandlers_6; }
	inline NetworkMessageHandlers_tA7BB2E51BDBD8ECE976AD44F1B634F40EA9807D4 ** get_address_of_m_MessageHandlers_6() { return &___m_MessageHandlers_6; }
	inline void set_m_MessageHandlers_6(NetworkMessageHandlers_tA7BB2E51BDBD8ECE976AD44F1B634F40EA9807D4 * value)
	{
		___m_MessageHandlers_6 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_MessageHandlers_6), (void*)value);
	}

	inline static int32_t get_offset_of_m_ClientOwnedObjects_7() { return static_cast<int32_t>(offsetof(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA, ___m_ClientOwnedObjects_7)); }
	inline HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990 * get_m_ClientOwnedObjects_7() const { return ___m_ClientOwnedObjects_7; }
	inline HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990 ** get_address_of_m_ClientOwnedObjects_7() { return &___m_ClientOwnedObjects_7; }
	inline void set_m_ClientOwnedObjects_7(HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990 * value)
	{
		___m_ClientOwnedObjects_7 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_ClientOwnedObjects_7), (void*)value);
	}

	inline static int32_t get_offset_of_m_MessageInfo_8() { return static_cast<int32_t>(offsetof(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA, ___m_MessageInfo_8)); }
	inline NetworkMessage_tCD66E2AE395A185EFE622EBB5497C95F6754685C * get_m_MessageInfo_8() const { return ___m_MessageInfo_8; }
	inline NetworkMessage_tCD66E2AE395A185EFE622EBB5497C95F6754685C ** get_address_of_m_MessageInfo_8() { return &___m_MessageInfo_8; }
	inline void set_m_MessageInfo_8(NetworkMessage_tCD66E2AE395A185EFE622EBB5497C95F6754685C * value)
	{
		___m_MessageInfo_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_MessageInfo_8), (void*)value);
	}

	inline static int32_t get_offset_of_error_10() { return static_cast<int32_t>(offsetof(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA, ___error_10)); }
	inline int32_t get_error_10() const { return ___error_10; }
	inline int32_t* get_address_of_error_10() { return &___error_10; }
	inline void set_error_10(int32_t value)
	{
		___error_10 = value;
	}

	inline static int32_t get_offset_of_hostId_11() { return static_cast<int32_t>(offsetof(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA, ___hostId_11)); }
	inline int32_t get_hostId_11() const { return ___hostId_11; }
	inline int32_t* get_address_of_hostId_11() { return &___hostId_11; }
	inline void set_hostId_11(int32_t value)
	{
		___hostId_11 = value;
	}

	inline static int32_t get_offset_of_connectionId_12() { return static_cast<int32_t>(offsetof(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA, ___connectionId_12)); }
	inline int32_t get_connectionId_12() const { return ___connectionId_12; }
	inline int32_t* get_address_of_connectionId_12() { return &___connectionId_12; }
	inline void set_connectionId_12(int32_t value)
	{
		___connectionId_12 = value;
	}

	inline static int32_t get_offset_of_isReady_13() { return static_cast<int32_t>(offsetof(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA, ___isReady_13)); }
	inline bool get_isReady_13() const { return ___isReady_13; }
	inline bool* get_address_of_isReady_13() { return &___isReady_13; }
	inline void set_isReady_13(bool value)
	{
		___isReady_13 = value;
	}

	inline static int32_t get_offset_of_address_14() { return static_cast<int32_t>(offsetof(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA, ___address_14)); }
	inline String_t* get_address_14() const { return ___address_14; }
	inline String_t** get_address_of_address_14() { return &___address_14; }
	inline void set_address_14(String_t* value)
	{
		___address_14 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___address_14), (void*)value);
	}

	inline static int32_t get_offset_of_lastMessageTime_15() { return static_cast<int32_t>(offsetof(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA, ___lastMessageTime_15)); }
	inline float get_lastMessageTime_15() const { return ___lastMessageTime_15; }
	inline float* get_address_of_lastMessageTime_15() { return &___lastMessageTime_15; }
	inline void set_lastMessageTime_15(float value)
	{
		___lastMessageTime_15 = value;
	}

	inline static int32_t get_offset_of_logNetworkMessages_16() { return static_cast<int32_t>(offsetof(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA, ___logNetworkMessages_16)); }
	inline bool get_logNetworkMessages_16() const { return ___logNetworkMessages_16; }
	inline bool* get_address_of_logNetworkMessages_16() { return &___logNetworkMessages_16; }
	inline void set_logNetworkMessages_16(bool value)
	{
		___logNetworkMessages_16 = value;
	}

	inline static int32_t get_offset_of_m_PacketStats_17() { return static_cast<int32_t>(offsetof(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA, ___m_PacketStats_17)); }
	inline Dictionary_2_t3C696EAE739BB0B87CB145AEF2D2B55EA1CAE88F * get_m_PacketStats_17() const { return ___m_PacketStats_17; }
	inline Dictionary_2_t3C696EAE739BB0B87CB145AEF2D2B55EA1CAE88F ** get_address_of_m_PacketStats_17() { return &___m_PacketStats_17; }
	inline void set_m_PacketStats_17(Dictionary_2_t3C696EAE739BB0B87CB145AEF2D2B55EA1CAE88F * value)
	{
		___m_PacketStats_17 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_PacketStats_17), (void*)value);
	}

	inline static int32_t get_offset_of_m_Disposed_18() { return static_cast<int32_t>(offsetof(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA, ___m_Disposed_18)); }
	inline bool get_m_Disposed_18() const { return ___m_Disposed_18; }
	inline bool* get_address_of_m_Disposed_18() { return &___m_Disposed_18; }
	inline void set_m_Disposed_18(bool value)
	{
		___m_Disposed_18 = value;
	}
};


// UnityEngine.Shader
struct  Shader_tE2731FF351B74AB4186897484FB01E000C1160CA  : public Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0
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


// System.IndexOutOfRangeException
struct  IndexOutOfRangeException_tEC7665FC66525AB6A6916A7EB505E5591683F0CF  : public SystemException_t5380468142AA850BE4A341D7AF3EAB9C78746782
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


// UnityEngine.Networking.LocalClient
struct  LocalClient_tCEC0096B13C433140FD4C09424CE345B28FE3C86  : public NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F
{
public:
	// System.Collections.Generic.List`1<UnityEngine.Networking.LocalClient_InternalMsg> UnityEngine.Networking.LocalClient::m_InternalMsgs
	List_1_tD68CD4018F6A1BB25DFAABF5C75012912E867F6A * ___m_InternalMsgs_23;
	// System.Collections.Generic.List`1<UnityEngine.Networking.LocalClient_InternalMsg> UnityEngine.Networking.LocalClient::m_InternalMsgs2
	List_1_tD68CD4018F6A1BB25DFAABF5C75012912E867F6A * ___m_InternalMsgs2_24;
	// System.Collections.Generic.Stack`1<UnityEngine.Networking.LocalClient_InternalMsg> UnityEngine.Networking.LocalClient::m_FreeMessages
	Stack_1_t9C08B2D567DCAE884CE2FD4DE45BA3F7BD6598E4 * ___m_FreeMessages_25;
	// UnityEngine.Networking.NetworkServer UnityEngine.Networking.LocalClient::m_LocalServer
	NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1 * ___m_LocalServer_26;
	// System.Boolean UnityEngine.Networking.LocalClient::m_Connected
	bool ___m_Connected_27;
	// UnityEngine.Networking.NetworkMessage UnityEngine.Networking.LocalClient::s_InternalMessage
	NetworkMessage_tCD66E2AE395A185EFE622EBB5497C95F6754685C * ___s_InternalMessage_28;

public:
	inline static int32_t get_offset_of_m_InternalMsgs_23() { return static_cast<int32_t>(offsetof(LocalClient_tCEC0096B13C433140FD4C09424CE345B28FE3C86, ___m_InternalMsgs_23)); }
	inline List_1_tD68CD4018F6A1BB25DFAABF5C75012912E867F6A * get_m_InternalMsgs_23() const { return ___m_InternalMsgs_23; }
	inline List_1_tD68CD4018F6A1BB25DFAABF5C75012912E867F6A ** get_address_of_m_InternalMsgs_23() { return &___m_InternalMsgs_23; }
	inline void set_m_InternalMsgs_23(List_1_tD68CD4018F6A1BB25DFAABF5C75012912E867F6A * value)
	{
		___m_InternalMsgs_23 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_InternalMsgs_23), (void*)value);
	}

	inline static int32_t get_offset_of_m_InternalMsgs2_24() { return static_cast<int32_t>(offsetof(LocalClient_tCEC0096B13C433140FD4C09424CE345B28FE3C86, ___m_InternalMsgs2_24)); }
	inline List_1_tD68CD4018F6A1BB25DFAABF5C75012912E867F6A * get_m_InternalMsgs2_24() const { return ___m_InternalMsgs2_24; }
	inline List_1_tD68CD4018F6A1BB25DFAABF5C75012912E867F6A ** get_address_of_m_InternalMsgs2_24() { return &___m_InternalMsgs2_24; }
	inline void set_m_InternalMsgs2_24(List_1_tD68CD4018F6A1BB25DFAABF5C75012912E867F6A * value)
	{
		___m_InternalMsgs2_24 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_InternalMsgs2_24), (void*)value);
	}

	inline static int32_t get_offset_of_m_FreeMessages_25() { return static_cast<int32_t>(offsetof(LocalClient_tCEC0096B13C433140FD4C09424CE345B28FE3C86, ___m_FreeMessages_25)); }
	inline Stack_1_t9C08B2D567DCAE884CE2FD4DE45BA3F7BD6598E4 * get_m_FreeMessages_25() const { return ___m_FreeMessages_25; }
	inline Stack_1_t9C08B2D567DCAE884CE2FD4DE45BA3F7BD6598E4 ** get_address_of_m_FreeMessages_25() { return &___m_FreeMessages_25; }
	inline void set_m_FreeMessages_25(Stack_1_t9C08B2D567DCAE884CE2FD4DE45BA3F7BD6598E4 * value)
	{
		___m_FreeMessages_25 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_FreeMessages_25), (void*)value);
	}

	inline static int32_t get_offset_of_m_LocalServer_26() { return static_cast<int32_t>(offsetof(LocalClient_tCEC0096B13C433140FD4C09424CE345B28FE3C86, ___m_LocalServer_26)); }
	inline NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1 * get_m_LocalServer_26() const { return ___m_LocalServer_26; }
	inline NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1 ** get_address_of_m_LocalServer_26() { return &___m_LocalServer_26; }
	inline void set_m_LocalServer_26(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1 * value)
	{
		___m_LocalServer_26 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_LocalServer_26), (void*)value);
	}

	inline static int32_t get_offset_of_m_Connected_27() { return static_cast<int32_t>(offsetof(LocalClient_tCEC0096B13C433140FD4C09424CE345B28FE3C86, ___m_Connected_27)); }
	inline bool get_m_Connected_27() const { return ___m_Connected_27; }
	inline bool* get_address_of_m_Connected_27() { return &___m_Connected_27; }
	inline void set_m_Connected_27(bool value)
	{
		___m_Connected_27 = value;
	}

	inline static int32_t get_offset_of_s_InternalMessage_28() { return static_cast<int32_t>(offsetof(LocalClient_tCEC0096B13C433140FD4C09424CE345B28FE3C86, ___s_InternalMessage_28)); }
	inline NetworkMessage_tCD66E2AE395A185EFE622EBB5497C95F6754685C * get_s_InternalMessage_28() const { return ___s_InternalMessage_28; }
	inline NetworkMessage_tCD66E2AE395A185EFE622EBB5497C95F6754685C ** get_address_of_s_InternalMessage_28() { return &___s_InternalMessage_28; }
	inline void set_s_InternalMessage_28(NetworkMessage_tCD66E2AE395A185EFE622EBB5497C95F6754685C * value)
	{
		___s_InternalMessage_28 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_InternalMessage_28), (void*)value);
	}
};


// UnityEngine.Networking.NetworkTransform_ClientMoveCallback2D
struct  ClientMoveCallback2D_tDFAD7DD6998C835AD2376F25136794AB12BA81A2  : public MulticastDelegate_t
{
public:

public:
};


// UnityEngine.Networking.NetworkTransform_ClientMoveCallback3D
struct  ClientMoveCallback3D_t8B3ABB4B5CD7D938193C0EB61634DC9424916A45  : public MulticastDelegate_t
{
public:

public:
};


// UnityEngine.Networking.SpawnDelegate
struct  SpawnDelegate_t4CB00A9006B512E467753C6CC752E29FA2EBC87F  : public MulticastDelegate_t
{
public:

public:
};


// UnityEngine.Networking.ULocalConnectionToClient
struct  ULocalConnectionToClient_t7AF7EBF2BEC3714F75EF894035BFAE9E6F9561A8  : public NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA
{
public:
	// UnityEngine.Networking.LocalClient UnityEngine.Networking.ULocalConnectionToClient::m_LocalClient
	LocalClient_tCEC0096B13C433140FD4C09424CE345B28FE3C86 * ___m_LocalClient_19;

public:
	inline static int32_t get_offset_of_m_LocalClient_19() { return static_cast<int32_t>(offsetof(ULocalConnectionToClient_t7AF7EBF2BEC3714F75EF894035BFAE9E6F9561A8, ___m_LocalClient_19)); }
	inline LocalClient_tCEC0096B13C433140FD4C09424CE345B28FE3C86 * get_m_LocalClient_19() const { return ___m_LocalClient_19; }
	inline LocalClient_tCEC0096B13C433140FD4C09424CE345B28FE3C86 ** get_address_of_m_LocalClient_19() { return &___m_LocalClient_19; }
	inline void set_m_LocalClient_19(LocalClient_tCEC0096B13C433140FD4C09424CE345B28FE3C86 * value)
	{
		___m_LocalClient_19 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_LocalClient_19), (void*)value);
	}
};


// UnityEngine.Networking.ULocalConnectionToServer
struct  ULocalConnectionToServer_tE6E34057F329C3E0E703C6F095DF82B0270557B8  : public NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA
{
public:
	// UnityEngine.Networking.NetworkServer UnityEngine.Networking.ULocalConnectionToServer::m_LocalServer
	NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1 * ___m_LocalServer_19;

public:
	inline static int32_t get_offset_of_m_LocalServer_19() { return static_cast<int32_t>(offsetof(ULocalConnectionToServer_tE6E34057F329C3E0E703C6F095DF82B0270557B8, ___m_LocalServer_19)); }
	inline NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1 * get_m_LocalServer_19() const { return ___m_LocalServer_19; }
	inline NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1 ** get_address_of_m_LocalServer_19() { return &___m_LocalServer_19; }
	inline void set_m_LocalServer_19(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1 * value)
	{
		___m_LocalServer_19 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_LocalServer_19), (void*)value);
	}
};


// UnityEngine.Networking.UnSpawnDelegate
struct  UnSpawnDelegate_tDC1AD5AA3602EB703F4FA34792B4D4075582AE19  : public MulticastDelegate_t
{
public:

public:
};


// UnityEngine.Rigidbody
struct  Rigidbody_tE0A58EE5A1F7DC908EFFB4F0D795AC9552A750A5  : public Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621
{
public:

public:
};


// UnityEngine.Rigidbody2D
struct  Rigidbody2D_tBDC6900A76D3C47E291446FF008D02B817C81CDE  : public Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621
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


// UnityEngine.MonoBehaviour
struct  MonoBehaviour_t4A60845CF505405AF8BE8C61CC07F75CADEF6429  : public Behaviour_tBDC7E9C3C898AD8348891B82D3E345801D920CA8
{
public:

public:
};


// UnityEngine.Networking.NetworkBehaviour
struct  NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C  : public MonoBehaviour_t4A60845CF505405AF8BE8C61CC07F75CADEF6429
{
public:
	// System.UInt32 UnityEngine.Networking.NetworkBehaviour::m_SyncVarDirtyBits
	uint32_t ___m_SyncVarDirtyBits_4;
	// System.Single UnityEngine.Networking.NetworkBehaviour::m_LastSendTime
	float ___m_LastSendTime_5;
	// System.Boolean UnityEngine.Networking.NetworkBehaviour::m_SyncVarGuard
	bool ___m_SyncVarGuard_6;
	// UnityEngine.Networking.NetworkIdentity UnityEngine.Networking.NetworkBehaviour::m_MyView
	NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * ___m_MyView_8;

public:
	inline static int32_t get_offset_of_m_SyncVarDirtyBits_4() { return static_cast<int32_t>(offsetof(NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C, ___m_SyncVarDirtyBits_4)); }
	inline uint32_t get_m_SyncVarDirtyBits_4() const { return ___m_SyncVarDirtyBits_4; }
	inline uint32_t* get_address_of_m_SyncVarDirtyBits_4() { return &___m_SyncVarDirtyBits_4; }
	inline void set_m_SyncVarDirtyBits_4(uint32_t value)
	{
		___m_SyncVarDirtyBits_4 = value;
	}

	inline static int32_t get_offset_of_m_LastSendTime_5() { return static_cast<int32_t>(offsetof(NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C, ___m_LastSendTime_5)); }
	inline float get_m_LastSendTime_5() const { return ___m_LastSendTime_5; }
	inline float* get_address_of_m_LastSendTime_5() { return &___m_LastSendTime_5; }
	inline void set_m_LastSendTime_5(float value)
	{
		___m_LastSendTime_5 = value;
	}

	inline static int32_t get_offset_of_m_SyncVarGuard_6() { return static_cast<int32_t>(offsetof(NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C, ___m_SyncVarGuard_6)); }
	inline bool get_m_SyncVarGuard_6() const { return ___m_SyncVarGuard_6; }
	inline bool* get_address_of_m_SyncVarGuard_6() { return &___m_SyncVarGuard_6; }
	inline void set_m_SyncVarGuard_6(bool value)
	{
		___m_SyncVarGuard_6 = value;
	}

	inline static int32_t get_offset_of_m_MyView_8() { return static_cast<int32_t>(offsetof(NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C, ___m_MyView_8)); }
	inline NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * get_m_MyView_8() const { return ___m_MyView_8; }
	inline NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B ** get_address_of_m_MyView_8() { return &___m_MyView_8; }
	inline void set_m_MyView_8(NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * value)
	{
		___m_MyView_8 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_MyView_8), (void*)value);
	}
};

struct NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C_StaticFields
{
public:
	// System.Collections.Generic.Dictionary`2<System.Int32,UnityEngine.Networking.NetworkBehaviour_Invoker> UnityEngine.Networking.NetworkBehaviour::s_CmdHandlerDelegates
	Dictionary_2_tCB6A26454DC24D4ED3A427AD6A6B9ADDA3A74D0D * ___s_CmdHandlerDelegates_9;

public:
	inline static int32_t get_offset_of_s_CmdHandlerDelegates_9() { return static_cast<int32_t>(offsetof(NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C_StaticFields, ___s_CmdHandlerDelegates_9)); }
	inline Dictionary_2_tCB6A26454DC24D4ED3A427AD6A6B9ADDA3A74D0D * get_s_CmdHandlerDelegates_9() const { return ___s_CmdHandlerDelegates_9; }
	inline Dictionary_2_tCB6A26454DC24D4ED3A427AD6A6B9ADDA3A74D0D ** get_address_of_s_CmdHandlerDelegates_9() { return &___s_CmdHandlerDelegates_9; }
	inline void set_s_CmdHandlerDelegates_9(Dictionary_2_tCB6A26454DC24D4ED3A427AD6A6B9ADDA3A74D0D * value)
	{
		___s_CmdHandlerDelegates_9 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_CmdHandlerDelegates_9), (void*)value);
	}
};


// UnityEngine.Networking.NetworkIdentity
struct  NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B  : public MonoBehaviour_t4A60845CF505405AF8BE8C61CC07F75CADEF6429
{
public:
	// UnityEngine.Networking.NetworkSceneId UnityEngine.Networking.NetworkIdentity::m_SceneId
	NetworkSceneId_t462EC62A23A1B7AF60637C48CD916A09BC493340  ___m_SceneId_4;
	// UnityEngine.Networking.NetworkHash128 UnityEngine.Networking.NetworkIdentity::m_AssetId
	NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  ___m_AssetId_5;
	// System.Boolean UnityEngine.Networking.NetworkIdentity::m_ServerOnly
	bool ___m_ServerOnly_6;
	// System.Boolean UnityEngine.Networking.NetworkIdentity::m_LocalPlayerAuthority
	bool ___m_LocalPlayerAuthority_7;
	// System.Boolean UnityEngine.Networking.NetworkIdentity::m_IsClient
	bool ___m_IsClient_8;
	// System.Boolean UnityEngine.Networking.NetworkIdentity::m_IsServer
	bool ___m_IsServer_9;
	// System.Boolean UnityEngine.Networking.NetworkIdentity::m_HasAuthority
	bool ___m_HasAuthority_10;
	// UnityEngine.Networking.NetworkInstanceId UnityEngine.Networking.NetworkIdentity::m_NetId
	NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  ___m_NetId_11;
	// System.Boolean UnityEngine.Networking.NetworkIdentity::m_IsLocalPlayer
	bool ___m_IsLocalPlayer_12;
	// UnityEngine.Networking.NetworkConnection UnityEngine.Networking.NetworkIdentity::m_ConnectionToServer
	NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * ___m_ConnectionToServer_13;
	// UnityEngine.Networking.NetworkConnection UnityEngine.Networking.NetworkIdentity::m_ConnectionToClient
	NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * ___m_ConnectionToClient_14;
	// System.Int16 UnityEngine.Networking.NetworkIdentity::m_PlayerId
	int16_t ___m_PlayerId_15;
	// UnityEngine.Networking.NetworkBehaviour[] UnityEngine.Networking.NetworkIdentity::m_NetworkBehaviours
	NetworkBehaviourU5BU5D_tA321D64478B9213228935C52651EBFA3E352C7CB* ___m_NetworkBehaviours_16;
	// System.Collections.Generic.HashSet`1<System.Int32> UnityEngine.Networking.NetworkIdentity::m_ObserverConnections
	HashSet_1_tC4214D83D479652EF2A07346543F228C3C0A8D74 * ___m_ObserverConnections_17;
	// System.Collections.Generic.List`1<UnityEngine.Networking.NetworkConnection> UnityEngine.Networking.NetworkIdentity::m_Observers
	List_1_t8B02DD1F0211D3E19F6A6E0204AF7D7537912362 * ___m_Observers_18;
	// UnityEngine.Networking.NetworkConnection UnityEngine.Networking.NetworkIdentity::m_ClientAuthorityOwner
	NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * ___m_ClientAuthorityOwner_19;
	// System.Boolean UnityEngine.Networking.NetworkIdentity::m_Reset
	bool ___m_Reset_20;

public:
	inline static int32_t get_offset_of_m_SceneId_4() { return static_cast<int32_t>(offsetof(NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B, ___m_SceneId_4)); }
	inline NetworkSceneId_t462EC62A23A1B7AF60637C48CD916A09BC493340  get_m_SceneId_4() const { return ___m_SceneId_4; }
	inline NetworkSceneId_t462EC62A23A1B7AF60637C48CD916A09BC493340 * get_address_of_m_SceneId_4() { return &___m_SceneId_4; }
	inline void set_m_SceneId_4(NetworkSceneId_t462EC62A23A1B7AF60637C48CD916A09BC493340  value)
	{
		___m_SceneId_4 = value;
	}

	inline static int32_t get_offset_of_m_AssetId_5() { return static_cast<int32_t>(offsetof(NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B, ___m_AssetId_5)); }
	inline NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  get_m_AssetId_5() const { return ___m_AssetId_5; }
	inline NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C * get_address_of_m_AssetId_5() { return &___m_AssetId_5; }
	inline void set_m_AssetId_5(NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  value)
	{
		___m_AssetId_5 = value;
	}

	inline static int32_t get_offset_of_m_ServerOnly_6() { return static_cast<int32_t>(offsetof(NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B, ___m_ServerOnly_6)); }
	inline bool get_m_ServerOnly_6() const { return ___m_ServerOnly_6; }
	inline bool* get_address_of_m_ServerOnly_6() { return &___m_ServerOnly_6; }
	inline void set_m_ServerOnly_6(bool value)
	{
		___m_ServerOnly_6 = value;
	}

	inline static int32_t get_offset_of_m_LocalPlayerAuthority_7() { return static_cast<int32_t>(offsetof(NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B, ___m_LocalPlayerAuthority_7)); }
	inline bool get_m_LocalPlayerAuthority_7() const { return ___m_LocalPlayerAuthority_7; }
	inline bool* get_address_of_m_LocalPlayerAuthority_7() { return &___m_LocalPlayerAuthority_7; }
	inline void set_m_LocalPlayerAuthority_7(bool value)
	{
		___m_LocalPlayerAuthority_7 = value;
	}

	inline static int32_t get_offset_of_m_IsClient_8() { return static_cast<int32_t>(offsetof(NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B, ___m_IsClient_8)); }
	inline bool get_m_IsClient_8() const { return ___m_IsClient_8; }
	inline bool* get_address_of_m_IsClient_8() { return &___m_IsClient_8; }
	inline void set_m_IsClient_8(bool value)
	{
		___m_IsClient_8 = value;
	}

	inline static int32_t get_offset_of_m_IsServer_9() { return static_cast<int32_t>(offsetof(NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B, ___m_IsServer_9)); }
	inline bool get_m_IsServer_9() const { return ___m_IsServer_9; }
	inline bool* get_address_of_m_IsServer_9() { return &___m_IsServer_9; }
	inline void set_m_IsServer_9(bool value)
	{
		___m_IsServer_9 = value;
	}

	inline static int32_t get_offset_of_m_HasAuthority_10() { return static_cast<int32_t>(offsetof(NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B, ___m_HasAuthority_10)); }
	inline bool get_m_HasAuthority_10() const { return ___m_HasAuthority_10; }
	inline bool* get_address_of_m_HasAuthority_10() { return &___m_HasAuthority_10; }
	inline void set_m_HasAuthority_10(bool value)
	{
		___m_HasAuthority_10 = value;
	}

	inline static int32_t get_offset_of_m_NetId_11() { return static_cast<int32_t>(offsetof(NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B, ___m_NetId_11)); }
	inline NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  get_m_NetId_11() const { return ___m_NetId_11; }
	inline NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615 * get_address_of_m_NetId_11() { return &___m_NetId_11; }
	inline void set_m_NetId_11(NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  value)
	{
		___m_NetId_11 = value;
	}

	inline static int32_t get_offset_of_m_IsLocalPlayer_12() { return static_cast<int32_t>(offsetof(NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B, ___m_IsLocalPlayer_12)); }
	inline bool get_m_IsLocalPlayer_12() const { return ___m_IsLocalPlayer_12; }
	inline bool* get_address_of_m_IsLocalPlayer_12() { return &___m_IsLocalPlayer_12; }
	inline void set_m_IsLocalPlayer_12(bool value)
	{
		___m_IsLocalPlayer_12 = value;
	}

	inline static int32_t get_offset_of_m_ConnectionToServer_13() { return static_cast<int32_t>(offsetof(NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B, ___m_ConnectionToServer_13)); }
	inline NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * get_m_ConnectionToServer_13() const { return ___m_ConnectionToServer_13; }
	inline NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA ** get_address_of_m_ConnectionToServer_13() { return &___m_ConnectionToServer_13; }
	inline void set_m_ConnectionToServer_13(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * value)
	{
		___m_ConnectionToServer_13 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_ConnectionToServer_13), (void*)value);
	}

	inline static int32_t get_offset_of_m_ConnectionToClient_14() { return static_cast<int32_t>(offsetof(NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B, ___m_ConnectionToClient_14)); }
	inline NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * get_m_ConnectionToClient_14() const { return ___m_ConnectionToClient_14; }
	inline NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA ** get_address_of_m_ConnectionToClient_14() { return &___m_ConnectionToClient_14; }
	inline void set_m_ConnectionToClient_14(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * value)
	{
		___m_ConnectionToClient_14 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_ConnectionToClient_14), (void*)value);
	}

	inline static int32_t get_offset_of_m_PlayerId_15() { return static_cast<int32_t>(offsetof(NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B, ___m_PlayerId_15)); }
	inline int16_t get_m_PlayerId_15() const { return ___m_PlayerId_15; }
	inline int16_t* get_address_of_m_PlayerId_15() { return &___m_PlayerId_15; }
	inline void set_m_PlayerId_15(int16_t value)
	{
		___m_PlayerId_15 = value;
	}

	inline static int32_t get_offset_of_m_NetworkBehaviours_16() { return static_cast<int32_t>(offsetof(NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B, ___m_NetworkBehaviours_16)); }
	inline NetworkBehaviourU5BU5D_tA321D64478B9213228935C52651EBFA3E352C7CB* get_m_NetworkBehaviours_16() const { return ___m_NetworkBehaviours_16; }
	inline NetworkBehaviourU5BU5D_tA321D64478B9213228935C52651EBFA3E352C7CB** get_address_of_m_NetworkBehaviours_16() { return &___m_NetworkBehaviours_16; }
	inline void set_m_NetworkBehaviours_16(NetworkBehaviourU5BU5D_tA321D64478B9213228935C52651EBFA3E352C7CB* value)
	{
		___m_NetworkBehaviours_16 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_NetworkBehaviours_16), (void*)value);
	}

	inline static int32_t get_offset_of_m_ObserverConnections_17() { return static_cast<int32_t>(offsetof(NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B, ___m_ObserverConnections_17)); }
	inline HashSet_1_tC4214D83D479652EF2A07346543F228C3C0A8D74 * get_m_ObserverConnections_17() const { return ___m_ObserverConnections_17; }
	inline HashSet_1_tC4214D83D479652EF2A07346543F228C3C0A8D74 ** get_address_of_m_ObserverConnections_17() { return &___m_ObserverConnections_17; }
	inline void set_m_ObserverConnections_17(HashSet_1_tC4214D83D479652EF2A07346543F228C3C0A8D74 * value)
	{
		___m_ObserverConnections_17 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_ObserverConnections_17), (void*)value);
	}

	inline static int32_t get_offset_of_m_Observers_18() { return static_cast<int32_t>(offsetof(NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B, ___m_Observers_18)); }
	inline List_1_t8B02DD1F0211D3E19F6A6E0204AF7D7537912362 * get_m_Observers_18() const { return ___m_Observers_18; }
	inline List_1_t8B02DD1F0211D3E19F6A6E0204AF7D7537912362 ** get_address_of_m_Observers_18() { return &___m_Observers_18; }
	inline void set_m_Observers_18(List_1_t8B02DD1F0211D3E19F6A6E0204AF7D7537912362 * value)
	{
		___m_Observers_18 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Observers_18), (void*)value);
	}

	inline static int32_t get_offset_of_m_ClientAuthorityOwner_19() { return static_cast<int32_t>(offsetof(NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B, ___m_ClientAuthorityOwner_19)); }
	inline NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * get_m_ClientAuthorityOwner_19() const { return ___m_ClientAuthorityOwner_19; }
	inline NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA ** get_address_of_m_ClientAuthorityOwner_19() { return &___m_ClientAuthorityOwner_19; }
	inline void set_m_ClientAuthorityOwner_19(NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * value)
	{
		___m_ClientAuthorityOwner_19 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_ClientAuthorityOwner_19), (void*)value);
	}

	inline static int32_t get_offset_of_m_Reset_20() { return static_cast<int32_t>(offsetof(NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B, ___m_Reset_20)); }
	inline bool get_m_Reset_20() const { return ___m_Reset_20; }
	inline bool* get_address_of_m_Reset_20() { return &___m_Reset_20; }
	inline void set_m_Reset_20(bool value)
	{
		___m_Reset_20 = value;
	}
};

struct NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B_StaticFields
{
public:
	// System.UInt32 UnityEngine.Networking.NetworkIdentity::s_NextNetworkId
	uint32_t ___s_NextNetworkId_21;
	// UnityEngine.Networking.NetworkWriter UnityEngine.Networking.NetworkIdentity::s_UpdateWriter
	NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * ___s_UpdateWriter_22;
	// UnityEngine.Networking.NetworkIdentity_ClientAuthorityCallback UnityEngine.Networking.NetworkIdentity::clientAuthorityCallback
	ClientAuthorityCallback_tB6533BDCE069DE0B5628A9BEE08EDCC76F373644 * ___clientAuthorityCallback_23;

public:
	inline static int32_t get_offset_of_s_NextNetworkId_21() { return static_cast<int32_t>(offsetof(NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B_StaticFields, ___s_NextNetworkId_21)); }
	inline uint32_t get_s_NextNetworkId_21() const { return ___s_NextNetworkId_21; }
	inline uint32_t* get_address_of_s_NextNetworkId_21() { return &___s_NextNetworkId_21; }
	inline void set_s_NextNetworkId_21(uint32_t value)
	{
		___s_NextNetworkId_21 = value;
	}

	inline static int32_t get_offset_of_s_UpdateWriter_22() { return static_cast<int32_t>(offsetof(NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B_StaticFields, ___s_UpdateWriter_22)); }
	inline NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * get_s_UpdateWriter_22() const { return ___s_UpdateWriter_22; }
	inline NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 ** get_address_of_s_UpdateWriter_22() { return &___s_UpdateWriter_22; }
	inline void set_s_UpdateWriter_22(NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * value)
	{
		___s_UpdateWriter_22 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_UpdateWriter_22), (void*)value);
	}

	inline static int32_t get_offset_of_clientAuthorityCallback_23() { return static_cast<int32_t>(offsetof(NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B_StaticFields, ___clientAuthorityCallback_23)); }
	inline ClientAuthorityCallback_tB6533BDCE069DE0B5628A9BEE08EDCC76F373644 * get_clientAuthorityCallback_23() const { return ___clientAuthorityCallback_23; }
	inline ClientAuthorityCallback_tB6533BDCE069DE0B5628A9BEE08EDCC76F373644 ** get_address_of_clientAuthorityCallback_23() { return &___clientAuthorityCallback_23; }
	inline void set_clientAuthorityCallback_23(ClientAuthorityCallback_tB6533BDCE069DE0B5628A9BEE08EDCC76F373644 * value)
	{
		___clientAuthorityCallback_23 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___clientAuthorityCallback_23), (void*)value);
	}
};


// UnityEngine.Networking.NetworkTransform
struct  NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F  : public NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C
{
public:
	// UnityEngine.Networking.NetworkTransform_TransformSyncMode UnityEngine.Networking.NetworkTransform::m_TransformSyncMode
	int32_t ___m_TransformSyncMode_10;
	// System.Single UnityEngine.Networking.NetworkTransform::m_SendInterval
	float ___m_SendInterval_11;
	// UnityEngine.Networking.NetworkTransform_AxisSyncMode UnityEngine.Networking.NetworkTransform::m_SyncRotationAxis
	int32_t ___m_SyncRotationAxis_12;
	// UnityEngine.Networking.NetworkTransform_CompressionSyncMode UnityEngine.Networking.NetworkTransform::m_RotationSyncCompression
	int32_t ___m_RotationSyncCompression_13;
	// System.Boolean UnityEngine.Networking.NetworkTransform::m_SyncSpin
	bool ___m_SyncSpin_14;
	// System.Single UnityEngine.Networking.NetworkTransform::m_MovementTheshold
	float ___m_MovementTheshold_15;
	// System.Single UnityEngine.Networking.NetworkTransform::m_VelocityThreshold
	float ___m_VelocityThreshold_16;
	// System.Single UnityEngine.Networking.NetworkTransform::m_SnapThreshold
	float ___m_SnapThreshold_17;
	// System.Single UnityEngine.Networking.NetworkTransform::m_InterpolateRotation
	float ___m_InterpolateRotation_18;
	// System.Single UnityEngine.Networking.NetworkTransform::m_InterpolateMovement
	float ___m_InterpolateMovement_19;
	// UnityEngine.Networking.NetworkTransform_ClientMoveCallback3D UnityEngine.Networking.NetworkTransform::m_ClientMoveCallback3D
	ClientMoveCallback3D_t8B3ABB4B5CD7D938193C0EB61634DC9424916A45 * ___m_ClientMoveCallback3D_20;
	// UnityEngine.Networking.NetworkTransform_ClientMoveCallback2D UnityEngine.Networking.NetworkTransform::m_ClientMoveCallback2D
	ClientMoveCallback2D_tDFAD7DD6998C835AD2376F25136794AB12BA81A2 * ___m_ClientMoveCallback2D_21;
	// UnityEngine.Rigidbody UnityEngine.Networking.NetworkTransform::m_RigidBody3D
	Rigidbody_tE0A58EE5A1F7DC908EFFB4F0D795AC9552A750A5 * ___m_RigidBody3D_22;
	// UnityEngine.Rigidbody2D UnityEngine.Networking.NetworkTransform::m_RigidBody2D
	Rigidbody2D_tBDC6900A76D3C47E291446FF008D02B817C81CDE * ___m_RigidBody2D_23;
	// UnityEngine.CharacterController UnityEngine.Networking.NetworkTransform::m_CharacterController
	CharacterController_t0ED98F461DBB7AC5B189C190153D83D5888BF93E * ___m_CharacterController_24;
	// System.Boolean UnityEngine.Networking.NetworkTransform::m_Grounded
	bool ___m_Grounded_25;
	// UnityEngine.Vector3 UnityEngine.Networking.NetworkTransform::m_TargetSyncPosition
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___m_TargetSyncPosition_26;
	// UnityEngine.Vector3 UnityEngine.Networking.NetworkTransform::m_TargetSyncVelocity
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___m_TargetSyncVelocity_27;
	// UnityEngine.Vector3 UnityEngine.Networking.NetworkTransform::m_FixedPosDiff
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___m_FixedPosDiff_28;
	// UnityEngine.Quaternion UnityEngine.Networking.NetworkTransform::m_TargetSyncRotation3D
	Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___m_TargetSyncRotation3D_29;
	// UnityEngine.Vector3 UnityEngine.Networking.NetworkTransform::m_TargetSyncAngularVelocity3D
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___m_TargetSyncAngularVelocity3D_30;
	// System.Single UnityEngine.Networking.NetworkTransform::m_TargetSyncRotation2D
	float ___m_TargetSyncRotation2D_31;
	// System.Single UnityEngine.Networking.NetworkTransform::m_TargetSyncAngularVelocity2D
	float ___m_TargetSyncAngularVelocity2D_32;
	// System.Single UnityEngine.Networking.NetworkTransform::m_LastClientSyncTime
	float ___m_LastClientSyncTime_33;
	// System.Single UnityEngine.Networking.NetworkTransform::m_LastClientSendTime
	float ___m_LastClientSendTime_34;
	// UnityEngine.Vector3 UnityEngine.Networking.NetworkTransform::m_PrevPosition
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___m_PrevPosition_35;
	// UnityEngine.Quaternion UnityEngine.Networking.NetworkTransform::m_PrevRotation
	Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___m_PrevRotation_36;
	// System.Single UnityEngine.Networking.NetworkTransform::m_PrevRotation2D
	float ___m_PrevRotation2D_37;
	// System.Single UnityEngine.Networking.NetworkTransform::m_PrevVelocity
	float ___m_PrevVelocity_38;
	// UnityEngine.Networking.NetworkWriter UnityEngine.Networking.NetworkTransform::m_LocalTransformWriter
	NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * ___m_LocalTransformWriter_43;

public:
	inline static int32_t get_offset_of_m_TransformSyncMode_10() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_TransformSyncMode_10)); }
	inline int32_t get_m_TransformSyncMode_10() const { return ___m_TransformSyncMode_10; }
	inline int32_t* get_address_of_m_TransformSyncMode_10() { return &___m_TransformSyncMode_10; }
	inline void set_m_TransformSyncMode_10(int32_t value)
	{
		___m_TransformSyncMode_10 = value;
	}

	inline static int32_t get_offset_of_m_SendInterval_11() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_SendInterval_11)); }
	inline float get_m_SendInterval_11() const { return ___m_SendInterval_11; }
	inline float* get_address_of_m_SendInterval_11() { return &___m_SendInterval_11; }
	inline void set_m_SendInterval_11(float value)
	{
		___m_SendInterval_11 = value;
	}

	inline static int32_t get_offset_of_m_SyncRotationAxis_12() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_SyncRotationAxis_12)); }
	inline int32_t get_m_SyncRotationAxis_12() const { return ___m_SyncRotationAxis_12; }
	inline int32_t* get_address_of_m_SyncRotationAxis_12() { return &___m_SyncRotationAxis_12; }
	inline void set_m_SyncRotationAxis_12(int32_t value)
	{
		___m_SyncRotationAxis_12 = value;
	}

	inline static int32_t get_offset_of_m_RotationSyncCompression_13() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_RotationSyncCompression_13)); }
	inline int32_t get_m_RotationSyncCompression_13() const { return ___m_RotationSyncCompression_13; }
	inline int32_t* get_address_of_m_RotationSyncCompression_13() { return &___m_RotationSyncCompression_13; }
	inline void set_m_RotationSyncCompression_13(int32_t value)
	{
		___m_RotationSyncCompression_13 = value;
	}

	inline static int32_t get_offset_of_m_SyncSpin_14() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_SyncSpin_14)); }
	inline bool get_m_SyncSpin_14() const { return ___m_SyncSpin_14; }
	inline bool* get_address_of_m_SyncSpin_14() { return &___m_SyncSpin_14; }
	inline void set_m_SyncSpin_14(bool value)
	{
		___m_SyncSpin_14 = value;
	}

	inline static int32_t get_offset_of_m_MovementTheshold_15() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_MovementTheshold_15)); }
	inline float get_m_MovementTheshold_15() const { return ___m_MovementTheshold_15; }
	inline float* get_address_of_m_MovementTheshold_15() { return &___m_MovementTheshold_15; }
	inline void set_m_MovementTheshold_15(float value)
	{
		___m_MovementTheshold_15 = value;
	}

	inline static int32_t get_offset_of_m_VelocityThreshold_16() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_VelocityThreshold_16)); }
	inline float get_m_VelocityThreshold_16() const { return ___m_VelocityThreshold_16; }
	inline float* get_address_of_m_VelocityThreshold_16() { return &___m_VelocityThreshold_16; }
	inline void set_m_VelocityThreshold_16(float value)
	{
		___m_VelocityThreshold_16 = value;
	}

	inline static int32_t get_offset_of_m_SnapThreshold_17() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_SnapThreshold_17)); }
	inline float get_m_SnapThreshold_17() const { return ___m_SnapThreshold_17; }
	inline float* get_address_of_m_SnapThreshold_17() { return &___m_SnapThreshold_17; }
	inline void set_m_SnapThreshold_17(float value)
	{
		___m_SnapThreshold_17 = value;
	}

	inline static int32_t get_offset_of_m_InterpolateRotation_18() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_InterpolateRotation_18)); }
	inline float get_m_InterpolateRotation_18() const { return ___m_InterpolateRotation_18; }
	inline float* get_address_of_m_InterpolateRotation_18() { return &___m_InterpolateRotation_18; }
	inline void set_m_InterpolateRotation_18(float value)
	{
		___m_InterpolateRotation_18 = value;
	}

	inline static int32_t get_offset_of_m_InterpolateMovement_19() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_InterpolateMovement_19)); }
	inline float get_m_InterpolateMovement_19() const { return ___m_InterpolateMovement_19; }
	inline float* get_address_of_m_InterpolateMovement_19() { return &___m_InterpolateMovement_19; }
	inline void set_m_InterpolateMovement_19(float value)
	{
		___m_InterpolateMovement_19 = value;
	}

	inline static int32_t get_offset_of_m_ClientMoveCallback3D_20() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_ClientMoveCallback3D_20)); }
	inline ClientMoveCallback3D_t8B3ABB4B5CD7D938193C0EB61634DC9424916A45 * get_m_ClientMoveCallback3D_20() const { return ___m_ClientMoveCallback3D_20; }
	inline ClientMoveCallback3D_t8B3ABB4B5CD7D938193C0EB61634DC9424916A45 ** get_address_of_m_ClientMoveCallback3D_20() { return &___m_ClientMoveCallback3D_20; }
	inline void set_m_ClientMoveCallback3D_20(ClientMoveCallback3D_t8B3ABB4B5CD7D938193C0EB61634DC9424916A45 * value)
	{
		___m_ClientMoveCallback3D_20 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_ClientMoveCallback3D_20), (void*)value);
	}

	inline static int32_t get_offset_of_m_ClientMoveCallback2D_21() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_ClientMoveCallback2D_21)); }
	inline ClientMoveCallback2D_tDFAD7DD6998C835AD2376F25136794AB12BA81A2 * get_m_ClientMoveCallback2D_21() const { return ___m_ClientMoveCallback2D_21; }
	inline ClientMoveCallback2D_tDFAD7DD6998C835AD2376F25136794AB12BA81A2 ** get_address_of_m_ClientMoveCallback2D_21() { return &___m_ClientMoveCallback2D_21; }
	inline void set_m_ClientMoveCallback2D_21(ClientMoveCallback2D_tDFAD7DD6998C835AD2376F25136794AB12BA81A2 * value)
	{
		___m_ClientMoveCallback2D_21 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_ClientMoveCallback2D_21), (void*)value);
	}

	inline static int32_t get_offset_of_m_RigidBody3D_22() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_RigidBody3D_22)); }
	inline Rigidbody_tE0A58EE5A1F7DC908EFFB4F0D795AC9552A750A5 * get_m_RigidBody3D_22() const { return ___m_RigidBody3D_22; }
	inline Rigidbody_tE0A58EE5A1F7DC908EFFB4F0D795AC9552A750A5 ** get_address_of_m_RigidBody3D_22() { return &___m_RigidBody3D_22; }
	inline void set_m_RigidBody3D_22(Rigidbody_tE0A58EE5A1F7DC908EFFB4F0D795AC9552A750A5 * value)
	{
		___m_RigidBody3D_22 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_RigidBody3D_22), (void*)value);
	}

	inline static int32_t get_offset_of_m_RigidBody2D_23() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_RigidBody2D_23)); }
	inline Rigidbody2D_tBDC6900A76D3C47E291446FF008D02B817C81CDE * get_m_RigidBody2D_23() const { return ___m_RigidBody2D_23; }
	inline Rigidbody2D_tBDC6900A76D3C47E291446FF008D02B817C81CDE ** get_address_of_m_RigidBody2D_23() { return &___m_RigidBody2D_23; }
	inline void set_m_RigidBody2D_23(Rigidbody2D_tBDC6900A76D3C47E291446FF008D02B817C81CDE * value)
	{
		___m_RigidBody2D_23 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_RigidBody2D_23), (void*)value);
	}

	inline static int32_t get_offset_of_m_CharacterController_24() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_CharacterController_24)); }
	inline CharacterController_t0ED98F461DBB7AC5B189C190153D83D5888BF93E * get_m_CharacterController_24() const { return ___m_CharacterController_24; }
	inline CharacterController_t0ED98F461DBB7AC5B189C190153D83D5888BF93E ** get_address_of_m_CharacterController_24() { return &___m_CharacterController_24; }
	inline void set_m_CharacterController_24(CharacterController_t0ED98F461DBB7AC5B189C190153D83D5888BF93E * value)
	{
		___m_CharacterController_24 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_CharacterController_24), (void*)value);
	}

	inline static int32_t get_offset_of_m_Grounded_25() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_Grounded_25)); }
	inline bool get_m_Grounded_25() const { return ___m_Grounded_25; }
	inline bool* get_address_of_m_Grounded_25() { return &___m_Grounded_25; }
	inline void set_m_Grounded_25(bool value)
	{
		___m_Grounded_25 = value;
	}

	inline static int32_t get_offset_of_m_TargetSyncPosition_26() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_TargetSyncPosition_26)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_m_TargetSyncPosition_26() const { return ___m_TargetSyncPosition_26; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_m_TargetSyncPosition_26() { return &___m_TargetSyncPosition_26; }
	inline void set_m_TargetSyncPosition_26(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___m_TargetSyncPosition_26 = value;
	}

	inline static int32_t get_offset_of_m_TargetSyncVelocity_27() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_TargetSyncVelocity_27)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_m_TargetSyncVelocity_27() const { return ___m_TargetSyncVelocity_27; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_m_TargetSyncVelocity_27() { return &___m_TargetSyncVelocity_27; }
	inline void set_m_TargetSyncVelocity_27(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___m_TargetSyncVelocity_27 = value;
	}

	inline static int32_t get_offset_of_m_FixedPosDiff_28() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_FixedPosDiff_28)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_m_FixedPosDiff_28() const { return ___m_FixedPosDiff_28; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_m_FixedPosDiff_28() { return &___m_FixedPosDiff_28; }
	inline void set_m_FixedPosDiff_28(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___m_FixedPosDiff_28 = value;
	}

	inline static int32_t get_offset_of_m_TargetSyncRotation3D_29() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_TargetSyncRotation3D_29)); }
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  get_m_TargetSyncRotation3D_29() const { return ___m_TargetSyncRotation3D_29; }
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 * get_address_of_m_TargetSyncRotation3D_29() { return &___m_TargetSyncRotation3D_29; }
	inline void set_m_TargetSyncRotation3D_29(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  value)
	{
		___m_TargetSyncRotation3D_29 = value;
	}

	inline static int32_t get_offset_of_m_TargetSyncAngularVelocity3D_30() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_TargetSyncAngularVelocity3D_30)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_m_TargetSyncAngularVelocity3D_30() const { return ___m_TargetSyncAngularVelocity3D_30; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_m_TargetSyncAngularVelocity3D_30() { return &___m_TargetSyncAngularVelocity3D_30; }
	inline void set_m_TargetSyncAngularVelocity3D_30(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___m_TargetSyncAngularVelocity3D_30 = value;
	}

	inline static int32_t get_offset_of_m_TargetSyncRotation2D_31() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_TargetSyncRotation2D_31)); }
	inline float get_m_TargetSyncRotation2D_31() const { return ___m_TargetSyncRotation2D_31; }
	inline float* get_address_of_m_TargetSyncRotation2D_31() { return &___m_TargetSyncRotation2D_31; }
	inline void set_m_TargetSyncRotation2D_31(float value)
	{
		___m_TargetSyncRotation2D_31 = value;
	}

	inline static int32_t get_offset_of_m_TargetSyncAngularVelocity2D_32() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_TargetSyncAngularVelocity2D_32)); }
	inline float get_m_TargetSyncAngularVelocity2D_32() const { return ___m_TargetSyncAngularVelocity2D_32; }
	inline float* get_address_of_m_TargetSyncAngularVelocity2D_32() { return &___m_TargetSyncAngularVelocity2D_32; }
	inline void set_m_TargetSyncAngularVelocity2D_32(float value)
	{
		___m_TargetSyncAngularVelocity2D_32 = value;
	}

	inline static int32_t get_offset_of_m_LastClientSyncTime_33() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_LastClientSyncTime_33)); }
	inline float get_m_LastClientSyncTime_33() const { return ___m_LastClientSyncTime_33; }
	inline float* get_address_of_m_LastClientSyncTime_33() { return &___m_LastClientSyncTime_33; }
	inline void set_m_LastClientSyncTime_33(float value)
	{
		___m_LastClientSyncTime_33 = value;
	}

	inline static int32_t get_offset_of_m_LastClientSendTime_34() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_LastClientSendTime_34)); }
	inline float get_m_LastClientSendTime_34() const { return ___m_LastClientSendTime_34; }
	inline float* get_address_of_m_LastClientSendTime_34() { return &___m_LastClientSendTime_34; }
	inline void set_m_LastClientSendTime_34(float value)
	{
		___m_LastClientSendTime_34 = value;
	}

	inline static int32_t get_offset_of_m_PrevPosition_35() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_PrevPosition_35)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_m_PrevPosition_35() const { return ___m_PrevPosition_35; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_m_PrevPosition_35() { return &___m_PrevPosition_35; }
	inline void set_m_PrevPosition_35(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___m_PrevPosition_35 = value;
	}

	inline static int32_t get_offset_of_m_PrevRotation_36() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_PrevRotation_36)); }
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  get_m_PrevRotation_36() const { return ___m_PrevRotation_36; }
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 * get_address_of_m_PrevRotation_36() { return &___m_PrevRotation_36; }
	inline void set_m_PrevRotation_36(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  value)
	{
		___m_PrevRotation_36 = value;
	}

	inline static int32_t get_offset_of_m_PrevRotation2D_37() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_PrevRotation2D_37)); }
	inline float get_m_PrevRotation2D_37() const { return ___m_PrevRotation2D_37; }
	inline float* get_address_of_m_PrevRotation2D_37() { return &___m_PrevRotation2D_37; }
	inline void set_m_PrevRotation2D_37(float value)
	{
		___m_PrevRotation2D_37 = value;
	}

	inline static int32_t get_offset_of_m_PrevVelocity_38() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_PrevVelocity_38)); }
	inline float get_m_PrevVelocity_38() const { return ___m_PrevVelocity_38; }
	inline float* get_address_of_m_PrevVelocity_38() { return &___m_PrevVelocity_38; }
	inline void set_m_PrevVelocity_38(float value)
	{
		___m_PrevVelocity_38 = value;
	}

	inline static int32_t get_offset_of_m_LocalTransformWriter_43() { return static_cast<int32_t>(offsetof(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F, ___m_LocalTransformWriter_43)); }
	inline NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * get_m_LocalTransformWriter_43() const { return ___m_LocalTransformWriter_43; }
	inline NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 ** get_address_of_m_LocalTransformWriter_43() { return &___m_LocalTransformWriter_43; }
	inline void set_m_LocalTransformWriter_43(NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * value)
	{
		___m_LocalTransformWriter_43 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_LocalTransformWriter_43), (void*)value);
	}
};


// UnityEngine.Networking.NetworkTransformChild
struct  NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E  : public NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C
{
public:
	// UnityEngine.Transform UnityEngine.Networking.NetworkTransformChild::m_Target
	Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * ___m_Target_10;
	// System.UInt32 UnityEngine.Networking.NetworkTransformChild::m_ChildIndex
	uint32_t ___m_ChildIndex_11;
	// UnityEngine.Networking.NetworkTransform UnityEngine.Networking.NetworkTransformChild::m_Root
	NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * ___m_Root_12;
	// System.Single UnityEngine.Networking.NetworkTransformChild::m_SendInterval
	float ___m_SendInterval_13;
	// UnityEngine.Networking.NetworkTransform_AxisSyncMode UnityEngine.Networking.NetworkTransformChild::m_SyncRotationAxis
	int32_t ___m_SyncRotationAxis_14;
	// UnityEngine.Networking.NetworkTransform_CompressionSyncMode UnityEngine.Networking.NetworkTransformChild::m_RotationSyncCompression
	int32_t ___m_RotationSyncCompression_15;
	// System.Single UnityEngine.Networking.NetworkTransformChild::m_MovementThreshold
	float ___m_MovementThreshold_16;
	// System.Single UnityEngine.Networking.NetworkTransformChild::m_InterpolateRotation
	float ___m_InterpolateRotation_17;
	// System.Single UnityEngine.Networking.NetworkTransformChild::m_InterpolateMovement
	float ___m_InterpolateMovement_18;
	// UnityEngine.Networking.NetworkTransform_ClientMoveCallback3D UnityEngine.Networking.NetworkTransformChild::m_ClientMoveCallback3D
	ClientMoveCallback3D_t8B3ABB4B5CD7D938193C0EB61634DC9424916A45 * ___m_ClientMoveCallback3D_19;
	// UnityEngine.Vector3 UnityEngine.Networking.NetworkTransformChild::m_TargetSyncPosition
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___m_TargetSyncPosition_20;
	// UnityEngine.Quaternion UnityEngine.Networking.NetworkTransformChild::m_TargetSyncRotation3D
	Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___m_TargetSyncRotation3D_21;
	// System.Single UnityEngine.Networking.NetworkTransformChild::m_LastClientSyncTime
	float ___m_LastClientSyncTime_22;
	// System.Single UnityEngine.Networking.NetworkTransformChild::m_LastClientSendTime
	float ___m_LastClientSendTime_23;
	// UnityEngine.Vector3 UnityEngine.Networking.NetworkTransformChild::m_PrevPosition
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___m_PrevPosition_24;
	// UnityEngine.Quaternion UnityEngine.Networking.NetworkTransformChild::m_PrevRotation
	Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___m_PrevRotation_25;
	// UnityEngine.Networking.NetworkWriter UnityEngine.Networking.NetworkTransformChild::m_LocalTransformWriter
	NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * ___m_LocalTransformWriter_28;

public:
	inline static int32_t get_offset_of_m_Target_10() { return static_cast<int32_t>(offsetof(NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E, ___m_Target_10)); }
	inline Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * get_m_Target_10() const { return ___m_Target_10; }
	inline Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA ** get_address_of_m_Target_10() { return &___m_Target_10; }
	inline void set_m_Target_10(Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * value)
	{
		___m_Target_10 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Target_10), (void*)value);
	}

	inline static int32_t get_offset_of_m_ChildIndex_11() { return static_cast<int32_t>(offsetof(NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E, ___m_ChildIndex_11)); }
	inline uint32_t get_m_ChildIndex_11() const { return ___m_ChildIndex_11; }
	inline uint32_t* get_address_of_m_ChildIndex_11() { return &___m_ChildIndex_11; }
	inline void set_m_ChildIndex_11(uint32_t value)
	{
		___m_ChildIndex_11 = value;
	}

	inline static int32_t get_offset_of_m_Root_12() { return static_cast<int32_t>(offsetof(NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E, ___m_Root_12)); }
	inline NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * get_m_Root_12() const { return ___m_Root_12; }
	inline NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F ** get_address_of_m_Root_12() { return &___m_Root_12; }
	inline void set_m_Root_12(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * value)
	{
		___m_Root_12 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Root_12), (void*)value);
	}

	inline static int32_t get_offset_of_m_SendInterval_13() { return static_cast<int32_t>(offsetof(NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E, ___m_SendInterval_13)); }
	inline float get_m_SendInterval_13() const { return ___m_SendInterval_13; }
	inline float* get_address_of_m_SendInterval_13() { return &___m_SendInterval_13; }
	inline void set_m_SendInterval_13(float value)
	{
		___m_SendInterval_13 = value;
	}

	inline static int32_t get_offset_of_m_SyncRotationAxis_14() { return static_cast<int32_t>(offsetof(NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E, ___m_SyncRotationAxis_14)); }
	inline int32_t get_m_SyncRotationAxis_14() const { return ___m_SyncRotationAxis_14; }
	inline int32_t* get_address_of_m_SyncRotationAxis_14() { return &___m_SyncRotationAxis_14; }
	inline void set_m_SyncRotationAxis_14(int32_t value)
	{
		___m_SyncRotationAxis_14 = value;
	}

	inline static int32_t get_offset_of_m_RotationSyncCompression_15() { return static_cast<int32_t>(offsetof(NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E, ___m_RotationSyncCompression_15)); }
	inline int32_t get_m_RotationSyncCompression_15() const { return ___m_RotationSyncCompression_15; }
	inline int32_t* get_address_of_m_RotationSyncCompression_15() { return &___m_RotationSyncCompression_15; }
	inline void set_m_RotationSyncCompression_15(int32_t value)
	{
		___m_RotationSyncCompression_15 = value;
	}

	inline static int32_t get_offset_of_m_MovementThreshold_16() { return static_cast<int32_t>(offsetof(NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E, ___m_MovementThreshold_16)); }
	inline float get_m_MovementThreshold_16() const { return ___m_MovementThreshold_16; }
	inline float* get_address_of_m_MovementThreshold_16() { return &___m_MovementThreshold_16; }
	inline void set_m_MovementThreshold_16(float value)
	{
		___m_MovementThreshold_16 = value;
	}

	inline static int32_t get_offset_of_m_InterpolateRotation_17() { return static_cast<int32_t>(offsetof(NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E, ___m_InterpolateRotation_17)); }
	inline float get_m_InterpolateRotation_17() const { return ___m_InterpolateRotation_17; }
	inline float* get_address_of_m_InterpolateRotation_17() { return &___m_InterpolateRotation_17; }
	inline void set_m_InterpolateRotation_17(float value)
	{
		___m_InterpolateRotation_17 = value;
	}

	inline static int32_t get_offset_of_m_InterpolateMovement_18() { return static_cast<int32_t>(offsetof(NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E, ___m_InterpolateMovement_18)); }
	inline float get_m_InterpolateMovement_18() const { return ___m_InterpolateMovement_18; }
	inline float* get_address_of_m_InterpolateMovement_18() { return &___m_InterpolateMovement_18; }
	inline void set_m_InterpolateMovement_18(float value)
	{
		___m_InterpolateMovement_18 = value;
	}

	inline static int32_t get_offset_of_m_ClientMoveCallback3D_19() { return static_cast<int32_t>(offsetof(NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E, ___m_ClientMoveCallback3D_19)); }
	inline ClientMoveCallback3D_t8B3ABB4B5CD7D938193C0EB61634DC9424916A45 * get_m_ClientMoveCallback3D_19() const { return ___m_ClientMoveCallback3D_19; }
	inline ClientMoveCallback3D_t8B3ABB4B5CD7D938193C0EB61634DC9424916A45 ** get_address_of_m_ClientMoveCallback3D_19() { return &___m_ClientMoveCallback3D_19; }
	inline void set_m_ClientMoveCallback3D_19(ClientMoveCallback3D_t8B3ABB4B5CD7D938193C0EB61634DC9424916A45 * value)
	{
		___m_ClientMoveCallback3D_19 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_ClientMoveCallback3D_19), (void*)value);
	}

	inline static int32_t get_offset_of_m_TargetSyncPosition_20() { return static_cast<int32_t>(offsetof(NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E, ___m_TargetSyncPosition_20)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_m_TargetSyncPosition_20() const { return ___m_TargetSyncPosition_20; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_m_TargetSyncPosition_20() { return &___m_TargetSyncPosition_20; }
	inline void set_m_TargetSyncPosition_20(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___m_TargetSyncPosition_20 = value;
	}

	inline static int32_t get_offset_of_m_TargetSyncRotation3D_21() { return static_cast<int32_t>(offsetof(NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E, ___m_TargetSyncRotation3D_21)); }
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  get_m_TargetSyncRotation3D_21() const { return ___m_TargetSyncRotation3D_21; }
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 * get_address_of_m_TargetSyncRotation3D_21() { return &___m_TargetSyncRotation3D_21; }
	inline void set_m_TargetSyncRotation3D_21(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  value)
	{
		___m_TargetSyncRotation3D_21 = value;
	}

	inline static int32_t get_offset_of_m_LastClientSyncTime_22() { return static_cast<int32_t>(offsetof(NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E, ___m_LastClientSyncTime_22)); }
	inline float get_m_LastClientSyncTime_22() const { return ___m_LastClientSyncTime_22; }
	inline float* get_address_of_m_LastClientSyncTime_22() { return &___m_LastClientSyncTime_22; }
	inline void set_m_LastClientSyncTime_22(float value)
	{
		___m_LastClientSyncTime_22 = value;
	}

	inline static int32_t get_offset_of_m_LastClientSendTime_23() { return static_cast<int32_t>(offsetof(NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E, ___m_LastClientSendTime_23)); }
	inline float get_m_LastClientSendTime_23() const { return ___m_LastClientSendTime_23; }
	inline float* get_address_of_m_LastClientSendTime_23() { return &___m_LastClientSendTime_23; }
	inline void set_m_LastClientSendTime_23(float value)
	{
		___m_LastClientSendTime_23 = value;
	}

	inline static int32_t get_offset_of_m_PrevPosition_24() { return static_cast<int32_t>(offsetof(NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E, ___m_PrevPosition_24)); }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  get_m_PrevPosition_24() const { return ___m_PrevPosition_24; }
	inline Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * get_address_of_m_PrevPosition_24() { return &___m_PrevPosition_24; }
	inline void set_m_PrevPosition_24(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  value)
	{
		___m_PrevPosition_24 = value;
	}

	inline static int32_t get_offset_of_m_PrevRotation_25() { return static_cast<int32_t>(offsetof(NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E, ___m_PrevRotation_25)); }
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  get_m_PrevRotation_25() const { return ___m_PrevRotation_25; }
	inline Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 * get_address_of_m_PrevRotation_25() { return &___m_PrevRotation_25; }
	inline void set_m_PrevRotation_25(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  value)
	{
		___m_PrevRotation_25 = value;
	}

	inline static int32_t get_offset_of_m_LocalTransformWriter_28() { return static_cast<int32_t>(offsetof(NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E, ___m_LocalTransformWriter_28)); }
	inline NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * get_m_LocalTransformWriter_28() const { return ___m_LocalTransformWriter_28; }
	inline NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 ** get_address_of_m_LocalTransformWriter_28() { return &___m_LocalTransformWriter_28; }
	inline void set_m_LocalTransformWriter_28(NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * value)
	{
		___m_LocalTransformWriter_28 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_LocalTransformWriter_28), (void*)value);
	}
};


// UnityEngine.Networking.NetworkTransformVisualizer
struct  NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386  : public NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C
{
public:
	// UnityEngine.GameObject UnityEngine.Networking.NetworkTransformVisualizer::m_VisualizerPrefab
	GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * ___m_VisualizerPrefab_10;
	// UnityEngine.Networking.NetworkTransform UnityEngine.Networking.NetworkTransformVisualizer::m_NetworkTransform
	NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * ___m_NetworkTransform_11;
	// UnityEngine.GameObject UnityEngine.Networking.NetworkTransformVisualizer::m_Visualizer
	GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * ___m_Visualizer_12;

public:
	inline static int32_t get_offset_of_m_VisualizerPrefab_10() { return static_cast<int32_t>(offsetof(NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386, ___m_VisualizerPrefab_10)); }
	inline GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * get_m_VisualizerPrefab_10() const { return ___m_VisualizerPrefab_10; }
	inline GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F ** get_address_of_m_VisualizerPrefab_10() { return &___m_VisualizerPrefab_10; }
	inline void set_m_VisualizerPrefab_10(GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * value)
	{
		___m_VisualizerPrefab_10 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_VisualizerPrefab_10), (void*)value);
	}

	inline static int32_t get_offset_of_m_NetworkTransform_11() { return static_cast<int32_t>(offsetof(NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386, ___m_NetworkTransform_11)); }
	inline NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * get_m_NetworkTransform_11() const { return ___m_NetworkTransform_11; }
	inline NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F ** get_address_of_m_NetworkTransform_11() { return &___m_NetworkTransform_11; }
	inline void set_m_NetworkTransform_11(NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * value)
	{
		___m_NetworkTransform_11 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_NetworkTransform_11), (void*)value);
	}

	inline static int32_t get_offset_of_m_Visualizer_12() { return static_cast<int32_t>(offsetof(NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386, ___m_Visualizer_12)); }
	inline GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * get_m_Visualizer_12() const { return ___m_Visualizer_12; }
	inline GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F ** get_address_of_m_Visualizer_12() { return &___m_Visualizer_12; }
	inline void set_m_Visualizer_12(GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * value)
	{
		___m_Visualizer_12 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___m_Visualizer_12), (void*)value);
	}
};

struct NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386_StaticFields
{
public:
	// UnityEngine.Material UnityEngine.Networking.NetworkTransformVisualizer::s_LineMaterial
	Material_tF7DB3BF0C24DEC2FE0CB51E5DF5053D5223C8598 * ___s_LineMaterial_13;

public:
	inline static int32_t get_offset_of_s_LineMaterial_13() { return static_cast<int32_t>(offsetof(NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386_StaticFields, ___s_LineMaterial_13)); }
	inline Material_tF7DB3BF0C24DEC2FE0CB51E5DF5053D5223C8598 * get_s_LineMaterial_13() const { return ___s_LineMaterial_13; }
	inline Material_tF7DB3BF0C24DEC2FE0CB51E5DF5053D5223C8598 ** get_address_of_s_LineMaterial_13() { return &___s_LineMaterial_13; }
	inline void set_s_LineMaterial_13(Material_tF7DB3BF0C24DEC2FE0CB51E5DF5053D5223C8598 * value)
	{
		___s_LineMaterial_13 = value;
		Il2CppCodeGenWriteBarrier((void**)(&___s_LineMaterial_13), (void*)value);
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
// UnityEngine.Networking.NetworkTransformChild[]
struct NetworkTransformChildU5BU5D_tAE50AD052E14730045D7CF7E715FBC48ADEDB6C1  : public RuntimeArray
{
public:
	ALIGN_FIELD (8) NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * m_Items[1];

public:
	inline NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * GetAt(il2cpp_array_size_t index) const
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items[index];
	}
	inline NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E ** GetAddressAt(il2cpp_array_size_t index)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		return m_Items + index;
	}
	inline void SetAt(il2cpp_array_size_t index, NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * value)
	{
		IL2CPP_ARRAY_BOUNDS_CHECK(index, (uint32_t)(this)->max_length);
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
	}
	inline NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * GetAtUnchecked(il2cpp_array_size_t index) const
	{
		return m_Items[index];
	}
	inline NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E ** GetAddressAtUnchecked(il2cpp_array_size_t index)
	{
		return m_Items + index;
	}
	inline void SetAtUnchecked(il2cpp_array_size_t index, NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * value)
	{
		m_Items[index] = value;
		Il2CppCodeGenWriteBarrier((void**)m_Items + index, (void*)value);
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


// !!0 UnityEngine.GameObject::GetComponent<System.Object>()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * GameObject_GetComponent_TisRuntimeObject_mE03C66715289D7957CA068A675826B7EE0887BE3_gshared (GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * __this, const RuntimeMethod* method);
// !!0[] UnityEngine.Component::GetComponents<System.Object>()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* Component_GetComponents_TisRuntimeObject_m1B7342AF989DE9DCE4CED42BF55A0AC6FFCBF6C6_gshared (Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621 * __this, const RuntimeMethod* method);
// !!0[] UnityEngine.GameObject::GetComponents<System.Object>()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* GameObject_GetComponents_TisRuntimeObject_mAB5B62A0C9EF4405B4E20D13F3CD7BC06A96FD40_gshared (GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * __this, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.HashSet`1<UnityEngine.Networking.NetworkInstanceId>::Contains(!0)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool HashSet_1_Contains_m68D1EC086CFCC7E6FBE6B1C66DDFF3D1DC62695C_gshared (HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990 * __this, NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  ___item0, const RuntimeMethod* method);
// !!0 UnityEngine.Component::GetComponent<System.Object>()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * Component_GetComponent_TisRuntimeObject_m15E3130603CE5400743CCCDEE7600FB9EEFAE5C0_gshared (Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621 * __this, const RuntimeMethod* method);
// !!0 UnityEngine.Object::Instantiate<System.Object>(!!0,UnityEngine.Vector3,UnityEngine.Quaternion)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * Object_Instantiate_TisRuntimeObject_mFE9C42D5336D4F9EFF8CD96E2A26962EFF523947_gshared (RuntimeObject * ___original0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___position1, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___rotation2, const RuntimeMethod* method);
// System.Int32 System.ArraySegment`1<System.Byte>::get_Count()
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR int32_t ArraySegment_1_get_Count_m02387DADA172F909FD346559D93990E990E05352_gshared_inline (ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA * __this, const RuntimeMethod* method);
// !0[] System.ArraySegment`1<System.Byte>::get_Array()
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ArraySegment_1_get_Array_m41D93EFB7EAB3081C0A27ED9891E7177F5F361B7_gshared_inline (ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.SyncList`1<System.Boolean>::AddInternal(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncList_1_AddInternal_m977B3CE5458FB772939C4CDB6612918FFC0BD427_gshared (SyncList_1_tDDF00E08E649A86264E50205CB99495D1AD2E8D0 * __this, bool ___item0, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.SyncList`1<System.Boolean>::Clear()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncList_1_Clear_mC367BED8954C65BFA956C2A66885A8FA241443E0_gshared (SyncList_1_tDDF00E08E649A86264E50205CB99495D1AD2E8D0 * __this, const RuntimeMethod* method);
// System.Int32 UnityEngine.Networking.SyncList`1<System.Boolean>::get_Count()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t SyncList_1_get_Count_m9EBDDB18AA65B4522E066D29FE2ECD9980BDEAD9_gshared (SyncList_1_tDDF00E08E649A86264E50205CB99495D1AD2E8D0 * __this, const RuntimeMethod* method);
// T UnityEngine.Networking.SyncList`1<System.Boolean>::get_Item(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SyncList_1_get_Item_m0EEA26E6C3ED4695254E4D9AC8243023AE227A48_gshared (SyncList_1_tDDF00E08E649A86264E50205CB99495D1AD2E8D0 * __this, int32_t ___i0, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.SyncList`1<System.Boolean>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncList_1__ctor_m1BB28896D4C843EEF83232CE6648F916429D54E3_gshared (SyncList_1_tDDF00E08E649A86264E50205CB99495D1AD2E8D0 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.SyncList`1<System.Single>::AddInternal(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncList_1_AddInternal_mC17F547D0099E43ACAA4C5FD21D63DDE456602A6_gshared (SyncList_1_tDEB03E3C5252571915662095C7060998910FD0A2 * __this, float ___item0, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.SyncList`1<System.Single>::Clear()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncList_1_Clear_m13160DF80DA71AAF005006E14C5C8985DBF15EB5_gshared (SyncList_1_tDEB03E3C5252571915662095C7060998910FD0A2 * __this, const RuntimeMethod* method);
// System.Int32 UnityEngine.Networking.SyncList`1<System.Single>::get_Count()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t SyncList_1_get_Count_mCC0838D9ED25E463384E4852839E47B100C99577_gshared (SyncList_1_tDEB03E3C5252571915662095C7060998910FD0A2 * __this, const RuntimeMethod* method);
// T UnityEngine.Networking.SyncList`1<System.Single>::get_Item(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float SyncList_1_get_Item_m70C832E1FED3E2D52297C7B6EF187700309BF7D4_gshared (SyncList_1_tDEB03E3C5252571915662095C7060998910FD0A2 * __this, int32_t ___i0, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.SyncList`1<System.Single>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncList_1__ctor_mACF8E6F1689E85F8D9D88F6B2366C1A08D6F853E_gshared (SyncList_1_tDEB03E3C5252571915662095C7060998910FD0A2 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.SyncList`1<System.Int32>::AddInternal(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncList_1_AddInternal_m93CDCB4D3061B2F4CF88B74DEABE1C06D4AED23C_gshared (SyncList_1_t8595ACD08C8686AC9547A225ACF209C171FF0665 * __this, int32_t ___item0, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.SyncList`1<System.Int32>::Clear()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncList_1_Clear_mF8FAE0172014F355D0C66600D8607442BF9A03B3_gshared (SyncList_1_t8595ACD08C8686AC9547A225ACF209C171FF0665 * __this, const RuntimeMethod* method);
// System.Int32 UnityEngine.Networking.SyncList`1<System.Int32>::get_Count()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t SyncList_1_get_Count_m7E687EFF75167B5EB639F273102ED345B8CB905B_gshared (SyncList_1_t8595ACD08C8686AC9547A225ACF209C171FF0665 * __this, const RuntimeMethod* method);
// T UnityEngine.Networking.SyncList`1<System.Int32>::get_Item(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t SyncList_1_get_Item_mA89484861CD0098C5FC7466F93F18C4EE231C55F_gshared (SyncList_1_t8595ACD08C8686AC9547A225ACF209C171FF0665 * __this, int32_t ___i0, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.SyncList`1<System.Int32>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncList_1__ctor_m6E3A6F39EE2A332965D0B912FD1662297B44B901_gshared (SyncList_1_t8595ACD08C8686AC9547A225ACF209C171FF0665 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.SyncList`1<System.Object>::AddInternal(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncList_1_AddInternal_m366BD82F1FADDBAA377373315B6905923C1EA438_gshared (SyncList_1_tDC9BD47B0C55962FA07DEC77A578A1F5231B0238 * __this, RuntimeObject * ___item0, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.SyncList`1<System.Object>::Clear()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncList_1_Clear_m6FF0B87F4C015BF2212C65CA18C0DA9A4A5A24D3_gshared (SyncList_1_tDC9BD47B0C55962FA07DEC77A578A1F5231B0238 * __this, const RuntimeMethod* method);
// System.Int32 UnityEngine.Networking.SyncList`1<System.Object>::get_Count()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t SyncList_1_get_Count_mACAE966F2F87AF836A946945D131D0EB48FB9F2B_gshared (SyncList_1_tDC9BD47B0C55962FA07DEC77A578A1F5231B0238 * __this, const RuntimeMethod* method);
// T UnityEngine.Networking.SyncList`1<System.Object>::get_Item(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject * SyncList_1_get_Item_m31396929FCFEC092BC74AD65E682CBFDCA7A6AA2_gshared (SyncList_1_tDC9BD47B0C55962FA07DEC77A578A1F5231B0238 * __this, int32_t ___i0, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.SyncList`1<System.Object>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncList_1__ctor_mEFD8368B5D9178E9DA189A23F22FD902E193C791_gshared (SyncList_1_tDC9BD47B0C55962FA07DEC77A578A1F5231B0238 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.SyncList`1<System.UInt32>::AddInternal(T)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncList_1_AddInternal_m84938C896AA1F3EED3568ECB90FED244DF2617B2_gshared (SyncList_1_t6931FBA5633802C1CAE04093D0C6D9C88EE66627 * __this, uint32_t ___item0, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.SyncList`1<System.UInt32>::Clear()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncList_1_Clear_m00C3496EAD8E618F4C20CA6F618373D4564CEB58_gshared (SyncList_1_t6931FBA5633802C1CAE04093D0C6D9C88EE66627 * __this, const RuntimeMethod* method);
// System.Int32 UnityEngine.Networking.SyncList`1<System.UInt32>::get_Count()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t SyncList_1_get_Count_m29E32BA907E6C50793D6A2D30D22A8D052A978B8_gshared (SyncList_1_t6931FBA5633802C1CAE04093D0C6D9C88EE66627 * __this, const RuntimeMethod* method);
// T UnityEngine.Networking.SyncList`1<System.UInt32>::get_Item(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t SyncList_1_get_Item_mC1369C43D41DC4C7863526B187E820DD7DA3709D_gshared (SyncList_1_t6931FBA5633802C1CAE04093D0C6D9C88EE66627 * __this, int32_t ___i0, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.SyncList`1<System.UInt32>::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncList_1__ctor_mF18B74E2EF8296E263BCEBAB8C8DE0EA78F8BAFC_gshared (SyncList_1_t6931FBA5633802C1CAE04093D0C6D9C88EE66627 * __this, const RuntimeMethod* method);

// System.Void UnityEngine.Networking.NetworkTransformChild::OnValidate()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_OnValidate_mB8E287CF434D44F97FCBF26CE7D72BD84EA592CF (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Object::op_Inequality(UnityEngine.Object,UnityEngine.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1 (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * ___x0, Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * ___y1, const RuntimeMethod* method);
// UnityEngine.Transform UnityEngine.Transform::get_parent()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * Transform_get_parent_m8FA24E38A1FA29D90CBF3CDC9F9F017C65BB3403 (Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Object::op_Equality(UnityEngine.Object,UnityEngine.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Object_op_Equality_mBC2401774F3BE33E8CF6F0A8148E66C95D6CFF1C (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * ___x0, Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * ___y1, const RuntimeMethod* method);
// System.Boolean UnityEngine.Networking.LogFilter::get_logError()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool LogFilter_get_logError_mD404500EEB2968A3CF190DB1EB6CA9A26135A21F (const RuntimeMethod* method);
// System.Void UnityEngine.Debug::LogError(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Debug_LogError_m3BCF9B78263152261565DCA9DB7D55F0C391ED29 (RuntimeObject * ___message0, const RuntimeMethod* method);
// UnityEngine.GameObject UnityEngine.Component::get_gameObject()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * Component_get_gameObject_m0B0570BA8DDD3CD78A9DB568EA18D7317686603C (Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621 * __this, const RuntimeMethod* method);
// !!0 UnityEngine.GameObject::GetComponent<UnityEngine.Networking.NetworkTransform>()
inline NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * GameObject_GetComponent_TisNetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F_mB885510CB2C4A1A57D2A42B4AE68A09AAA1DD79A (GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * __this, const RuntimeMethod* method)
{
	return ((  NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * (*) (GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F *, const RuntimeMethod*))GameObject_GetComponent_TisRuntimeObject_mE03C66715289D7957CA068A675826B7EE0887BE3_gshared)(__this, method);
}
// !!0[] UnityEngine.Component::GetComponents<UnityEngine.Networking.NetworkTransformChild>()
inline NetworkTransformChildU5BU5D_tAE50AD052E14730045D7CF7E715FBC48ADEDB6C1* Component_GetComponents_TisNetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E_mD3E94B5EC8B4D6678D6CE5FDFBA6502236E701C0 (Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621 * __this, const RuntimeMethod* method)
{
	return ((  NetworkTransformChildU5BU5D_tAE50AD052E14730045D7CF7E715FBC48ADEDB6C1* (*) (Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621 *, const RuntimeMethod*))Component_GetComponents_TisRuntimeObject_m1B7342AF989DE9DCE4CED42BF55A0AC6FFCBF6C6_gshared)(__this, method);
}
// System.Single UnityEngine.Networking.NetworkTransformChild::get_movementThreshold()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR float NetworkTransformChild_get_movementThreshold_m9BED81E541443BA95A2DDDF7465386F0E4F5639A_inline (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkTransformChild::set_movementThreshold(System.Single)
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR void NetworkTransformChild_set_movementThreshold_m93D1B2916BC9B686B9F40C755A0AADCE6E54AB93_inline (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, float ___value0, const RuntimeMethod* method);
// System.Single UnityEngine.Networking.NetworkTransformChild::get_interpolateRotation()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR float NetworkTransformChild_get_interpolateRotation_m9169822905990E0E3C3531F5812DC25FBE4C06BA_inline (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkTransformChild::set_interpolateRotation(System.Single)
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR void NetworkTransformChild_set_interpolateRotation_mF72DC382026B2C763A63F0FB8D565275ECAEC4DF_inline (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, float ___value0, const RuntimeMethod* method);
// System.Single UnityEngine.Networking.NetworkTransformChild::get_interpolateMovement()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR float NetworkTransformChild_get_interpolateMovement_m27491C1C3805AE420F584C937DC7DDFDB4A98E74_inline (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkTransformChild::set_interpolateMovement(System.Single)
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR void NetworkTransformChild_set_interpolateMovement_m94A0DB8134ACE98F427C12CFA1057CAA9370CF44_inline (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, float ___value0, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Transform::get_localPosition()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Transform_get_localPosition_m812D43318E05BDCB78310EB7308785A13D85EFD8 (Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * __this, const RuntimeMethod* method);
// UnityEngine.Quaternion UnityEngine.Transform::get_localRotation()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  Transform_get_localRotation_mEDA319E1B42EF12A19A95AC0824345B6574863FE (Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Networking.NetworkBehaviour::get_localPlayerAuthority()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool NetworkBehaviour_get_localPlayerAuthority_m73DEE3D9A2E9916520CBDBA1B11888DAEA24B415 (NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkWriter::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter__ctor_m43E453A4A5244815EC8D906B22E5D85FB7535D33 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, const RuntimeMethod* method);
// System.UInt32 UnityEngine.Networking.NetworkBehaviour::get_syncVarDirtyBits()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR uint32_t NetworkBehaviour_get_syncVarDirtyBits_mD53C3F852C533A88A2312E7AFF9883658DDEEB0C_inline (NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkWriter::WritePackedUInt32(System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_WritePackedUInt32_m99DCA40833B068CB958663A5B583BC8D2051B12F (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, uint32_t ___value0, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkTransformChild::SerializeModeTransform(UnityEngine.Networking.NetworkWriter)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_SerializeModeTransform_mCC0568E0CA6EC5192285EB962D141E4B056C6903 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * ___writer0, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkWriter::Write(UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m11CA4683BE86268158E1F949E620C1BF9D69884F (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___value0, const RuntimeMethod* method);
// UnityEngine.Networking.NetworkTransform/AxisSyncMode UnityEngine.Networking.NetworkTransformChild::get_syncRotationAxis()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t NetworkTransformChild_get_syncRotationAxis_m12199E8BCADC5098C84E6F2E6A8534424FD80979_inline (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method);
// UnityEngine.Networking.NetworkTransform/CompressionSyncMode UnityEngine.Networking.NetworkTransformChild::get_rotationSyncCompression()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t NetworkTransformChild_get_rotationSyncCompression_m014CA812E4BB0DBF2DF856CB96E40FBED022239B_inline (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkTransform::SerializeRotation3D(UnityEngine.Networking.NetworkWriter,UnityEngine.Quaternion,UnityEngine.Networking.NetworkTransform/AxisSyncMode,UnityEngine.Networking.NetworkTransform/CompressionSyncMode)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransform_SerializeRotation3D_m709105872FF5E4CA551590B97506834348060215 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * ___writer0, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___rot1, int32_t ___mode2, int32_t ___compression3, const RuntimeMethod* method);
// System.Boolean UnityEngine.Networking.NetworkBehaviour::get_isServer()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool NetworkBehaviour_get_isServer_m3366F78A4D83ECE0798B276F2E9EF1FEEC8E2D79 (NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Networking.NetworkServer::get_localClientActive()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool NetworkServer_get_localClientActive_mB6EDFFE4FCDAD0215974EE9F24E4E38D1257BF02 (const RuntimeMethod* method);
// System.UInt32 UnityEngine.Networking.NetworkReader::ReadPackedUInt32()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t NetworkReader_ReadPackedUInt32_mB0E5BF11AEAD652C88548BD93556D780A4E3F46B (NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkTransformChild::UnserializeModeTransform(UnityEngine.Networking.NetworkReader,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_UnserializeModeTransform_mBD3AB1CDF0F5F3D5FEA1AB49C0C53F7B24A62B39 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * ___reader0, bool ___initialState1, const RuntimeMethod* method);
// System.Single UnityEngine.Time::get_time()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float Time_get_time_m7863349C8845BBA36629A2B3F8EF1C3BEA350FD8 (const RuntimeMethod* method);
// System.Boolean UnityEngine.Networking.NetworkBehaviour::get_hasAuthority()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool NetworkBehaviour_get_hasAuthority_m20156D4B7D1F4097FFEAEFB2D0EAE8F95FF0B798 (NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * __this, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Networking.NetworkReader::ReadVector3()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  NetworkReader_ReadVector3_m8067F9687AEA7DD9FAC65E4550A441E8C7402314 (NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * __this, const RuntimeMethod* method);
// UnityEngine.Quaternion UnityEngine.Networking.NetworkTransform::UnserializeRotation3D(UnityEngine.Networking.NetworkReader,UnityEngine.Networking.NetworkTransform/AxisSyncMode,UnityEngine.Networking.NetworkTransform/CompressionSyncMode)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  NetworkTransform_UnserializeRotation3D_m6E8B8C1812E6FA2EBD24EFD6A8B75DC04439759E (NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * ___reader0, int32_t ___mode1, int32_t ___compression2, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Vector3::get_zero()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Vector3_get_zero_m3CDDCAE94581DF3BB16C4B40A100E28E9C6649C2 (const RuntimeMethod* method);
// UnityEngine.Quaternion UnityEngine.Quaternion::get_identity()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  Quaternion_get_identity_m548B37D80F2DEE60E41D1F09BF6889B557BE1A64 (const RuntimeMethod* method);
// System.Boolean UnityEngine.Networking.NetworkTransform/ClientMoveCallback3D::Invoke(UnityEngine.Vector3&,UnityEngine.Vector3&,UnityEngine.Quaternion&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ClientMoveCallback3D_Invoke_m4F4CED5C02FAFD6145BBA95A6B5261ACB9E0B19C (ClientMoveCallback3D_t8B3ABB4B5CD7D938193C0EB61634DC9424916A45 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___position0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___velocity1, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 * ___rotation2, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkTransformChild::FixedUpdateServer()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_FixedUpdateServer_mC12D2E9DD8B1AE9D8222581DAE9F972F29102FE2 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Networking.NetworkBehaviour::get_isClient()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool NetworkBehaviour_get_isClient_mB7B109ADAF27B23B3D58E2369CBD11B1471C9148 (NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkTransformChild::FixedUpdateClient()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_FixedUpdateClient_mA050F9F38ED8D506D572DC21814309EDBF982B99 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Networking.NetworkServer::get_active()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR bool NetworkServer_get_active_m3FAC75ABF32D586F6C8DB6B4237DC40300FB2257_inline (const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Vector3::op_Subtraction(UnityEngine.Vector3,UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Vector3_op_Subtraction_mF9846B723A5034F8B9F5F5DCB78E3D67649143D3 (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___a0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___b1, const RuntimeMethod* method);
// System.Single UnityEngine.Vector3::get_sqrMagnitude()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float Vector3_get_sqrMagnitude_m1C6E190B4A933A183B308736DEC0DD64B0588968 (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * __this, const RuntimeMethod* method);
// System.Single UnityEngine.Quaternion::Angle(UnityEngine.Quaternion,UnityEngine.Quaternion)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float Quaternion_Angle_m09599D660B724D330E5C7FE2FB1C8716161B3DD1 (Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___a0, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___b1, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkBehaviour::SetDirtyBit(System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkBehaviour_SetDirtyBit_m474FBAD852378B9657C96EDD3E72BDDFD7E893DF (NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * __this, uint32_t ___dirtyBit0, const RuntimeMethod* method);
// System.Boolean UnityEngine.Networking.NetworkClient::get_active()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR bool NetworkClient_get_active_m31953DC487641BC5D9BEB0EB4DE32462AC4A8BD1_inline (const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Vector3::Lerp(UnityEngine.Vector3,UnityEngine.Vector3,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Vector3_Lerp_m5BA75496B803820CC64079383956D73C6FD4A8A1 (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___a0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___b1, float ___t2, const RuntimeMethod* method);
// System.Void UnityEngine.Transform::set_localPosition(UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Transform_set_localPosition_m275F5550DD939F83AFEB5E8D681131172E2E1728 (Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___value0, const RuntimeMethod* method);
// UnityEngine.Quaternion UnityEngine.Quaternion::Slerp(UnityEngine.Quaternion,UnityEngine.Quaternion,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  Quaternion_Slerp_m56DE173C3520C83DF3F1C6EDFA82FF88A2C9E756 (Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___a0, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___b1, float ___t2, const RuntimeMethod* method);
// System.Void UnityEngine.Transform::set_localRotation(UnityEngine.Quaternion)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Transform_set_localRotation_mE2BECB0954FFC1D93FB631600D9A9BEFF41D9C8A (Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * __this, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___value0, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkTransformChild::SendTransform()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_SendTransform_m5E5A962A5EB1E18C5C6175873A55AE8E73C53AD9 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Networking.NetworkTransformChild::HasMoved()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool NetworkTransformChild_HasMoved_m6B10F1CD5A72301C5E26797F4105EAE2693971B4 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method);
// UnityEngine.Networking.NetworkConnection UnityEngine.Networking.ClientScene::get_readyConnection()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * ClientScene_get_readyConnection_mACB67AD0151B2507CF8BD5D7D8B806C470E49998_inline (const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkWriter::StartMessage(System.Int16)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_StartMessage_mD4F5BFA7ECA40EEA4AC721A1E357C3C8A09CE218 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, int16_t ___msgType0, const RuntimeMethod* method);
// UnityEngine.Networking.NetworkInstanceId UnityEngine.Networking.NetworkBehaviour::get_netId()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  NetworkBehaviour_get_netId_m33EAF782A985004BBEEB6AE5CD30A2C8F4E35564 (NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkWriter::Write(UnityEngine.Networking.NetworkInstanceId)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m327AAC971B7DA22E82661AD419E4D5EEC6CCAFBF (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  ___value0, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkWriter::FinishMessage()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_FinishMessage_mDA9E66815E448F635B2394A35DDCA3EC040B0590 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, const RuntimeMethod* method);
// UnityEngine.Networking.NetworkInstanceId UnityEngine.Networking.NetworkReader::ReadNetworkId()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  NetworkReader_ReadNetworkId_m68B53D6FD5C9BF5DCEA1E114630D5256A331E7FE (NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * __this, const RuntimeMethod* method);
// UnityEngine.GameObject UnityEngine.Networking.NetworkServer::FindLocalObject(UnityEngine.Networking.NetworkInstanceId)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * NetworkServer_FindLocalObject_m0EA227D12590A2EE92F6B029C888AE46C560FB77 (NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  ___netId0, const RuntimeMethod* method);
// !!0[] UnityEngine.GameObject::GetComponents<UnityEngine.Networking.NetworkTransformChild>()
inline NetworkTransformChildU5BU5D_tAE50AD052E14730045D7CF7E715FBC48ADEDB6C1* GameObject_GetComponents_TisNetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E_mAE4B663ABA411E0016C4E260112D19FB99B8889F (GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * __this, const RuntimeMethod* method)
{
	return ((  NetworkTransformChildU5BU5D_tAE50AD052E14730045D7CF7E715FBC48ADEDB6C1* (*) (GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F *, const RuntimeMethod*))GameObject_GetComponents_TisRuntimeObject_mAB5B62A0C9EF4405B4E20D13F3CD7BC06A96FD40_gshared)(__this, method);
}
// System.Collections.Generic.HashSet`1<UnityEngine.Networking.NetworkInstanceId> UnityEngine.Networking.NetworkConnection::get_clientOwnedObjects()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990 * NetworkConnection_get_clientOwnedObjects_m0CC0D90CD318855211AA194D67DB4A07E4694D22_inline (NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * __this, const RuntimeMethod* method);
// System.Boolean System.Collections.Generic.HashSet`1<UnityEngine.Networking.NetworkInstanceId>::Contains(!0)
inline bool HashSet_1_Contains_m68D1EC086CFCC7E6FBE6B1C66DDFF3D1DC62695C (HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990 * __this, NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  ___item0, const RuntimeMethod* method)
{
	return ((  bool (*) (HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990 *, NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615 , const RuntimeMethod*))HashSet_1_Contains_m68D1EC086CFCC7E6FBE6B1C66DDFF3D1DC62695C_gshared)(__this, ___item0, method);
}
// System.Boolean UnityEngine.Networking.LogFilter::get_logWarn()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool LogFilter_get_logWarn_m68D69BE30614BF75FF942A304F2C453298667AFD (const RuntimeMethod* method);
// System.String System.String::Concat(System.Object,System.Object,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Concat_m2E1F71C491D2429CC80A28745488FEA947BB7AAC (RuntimeObject * ___arg00, RuntimeObject * ___arg11, RuntimeObject * ___arg22, const RuntimeMethod* method);
// System.Void UnityEngine.Debug::LogWarning(System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Debug_LogWarning_m37338644DC81F640CCDFEAE35A223F0E965F0568 (RuntimeObject * ___message0, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkBehaviour::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkBehaviour__ctor_m37D8F4B6AD273AFBE5507BB02D956282684A0B78 (NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * __this, const RuntimeMethod* method);
// !!0 UnityEngine.Component::GetComponent<UnityEngine.Networking.NetworkTransform>()
inline NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * Component_GetComponent_TisNetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F_mC1DB4A13BBC41101231C90CD393292630350975B (Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621 * __this, const RuntimeMethod* method)
{
	return ((  NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * (*) (Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621 *, const RuntimeMethod*))Component_GetComponent_TisRuntimeObject_m15E3130603CE5400743CCCDEE7600FB9EEFAE5C0_gshared)(__this, method);
}
// System.Void UnityEngine.Networking.NetworkTransformVisualizer::CreateLineMaterial()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformVisualizer_CreateLineMaterial_m7939398CB6B61BFD7D237E3685D50000C7A41B89 (const RuntimeMethod* method);
// UnityEngine.Transform UnityEngine.Component::get_transform()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9 (Component_t05064EF382ABCAF4B8C94F8A350EA85184C26621 * __this, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Transform::get_position()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294 (Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * __this, const RuntimeMethod* method);
// !!0 UnityEngine.Object::Instantiate<UnityEngine.GameObject>(!!0,UnityEngine.Vector3,UnityEngine.Quaternion)
inline GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * Object_Instantiate_TisGameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F_m4F397BCC6697902B40033E61129D4EA6FE93570F (GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * ___original0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___position1, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___rotation2, const RuntimeMethod* method)
{
	return ((  GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * (*) (GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 , Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 , const RuntimeMethod*))Object_Instantiate_TisRuntimeObject_mFE9C42D5336D4F9EFF8CD96E2A26962EFF523947_gshared)(___original0, ___position1, ___rotation2, method);
}
// System.Void UnityEngine.Object::Destroy(UnityEngine.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object_Destroy_m23B4562495BA35A74266D4372D45368F8C05109A (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * ___obj0, const RuntimeMethod* method);
// UnityEngine.Transform UnityEngine.GameObject::get_transform()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * GameObject_get_transform_mA5C38857137F137CB96C69FAA624199EB1C2FB2C (GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * __this, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Networking.NetworkTransform::get_targetSyncPosition()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  NetworkTransform_get_targetSyncPosition_m8D2DCE0C4C4EDE2729E3323218669E433952A446_inline (NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Transform::set_position(UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Transform_set_position_mDA89E4893F14ECA5CBEEE7FB80A5BF7C1B8EA6DC (Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___value0, const RuntimeMethod* method);
// UnityEngine.Rigidbody UnityEngine.Networking.NetworkTransform::get_rigidbody3D()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR Rigidbody_tE0A58EE5A1F7DC908EFFB4F0D795AC9552A750A5 * NetworkTransform_get_rigidbody3D_m2F059AC7FE4AE29073DA4FB4D6D9719A35245DEB_inline (NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * __this, const RuntimeMethod* method);
// !!0 UnityEngine.GameObject::GetComponent<UnityEngine.Rigidbody>()
inline Rigidbody_tE0A58EE5A1F7DC908EFFB4F0D795AC9552A750A5 * GameObject_GetComponent_TisRigidbody_tE0A58EE5A1F7DC908EFFB4F0D795AC9552A750A5_m31F97A6E057858450728C32EE09647374FA10903 (GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * __this, const RuntimeMethod* method)
{
	return ((  Rigidbody_tE0A58EE5A1F7DC908EFFB4F0D795AC9552A750A5 * (*) (GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F *, const RuntimeMethod*))GameObject_GetComponent_TisRuntimeObject_mE03C66715289D7957CA068A675826B7EE0887BE3_gshared)(__this, method);
}
// UnityEngine.Vector3 UnityEngine.Networking.NetworkTransform::get_targetSyncVelocity()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  NetworkTransform_get_targetSyncVelocity_m7C47913B3EBFDC866349F5C091C439D255B75CFB_inline (NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Rigidbody::set_velocity(UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Rigidbody_set_velocity_m8D129E88E62AD02AB81CFC8BE694C4A5A2B2B380 (Rigidbody_tE0A58EE5A1F7DC908EFFB4F0D795AC9552A750A5 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___value0, const RuntimeMethod* method);
// UnityEngine.Rigidbody2D UnityEngine.Networking.NetworkTransform::get_rigidbody2D()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR Rigidbody2D_tBDC6900A76D3C47E291446FF008D02B817C81CDE * NetworkTransform_get_rigidbody2D_mC7614E0AE776DEE2D14FCC7E41D90CD5D498F765_inline (NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * __this, const RuntimeMethod* method);
// !!0 UnityEngine.GameObject::GetComponent<UnityEngine.Rigidbody2D>()
inline Rigidbody2D_tBDC6900A76D3C47E291446FF008D02B817C81CDE * GameObject_GetComponent_TisRigidbody2D_tBDC6900A76D3C47E291446FF008D02B817C81CDE_mDDB82F02C3053DCC0D60C420752A11EC11CBACC0 (GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * __this, const RuntimeMethod* method)
{
	return ((  Rigidbody2D_tBDC6900A76D3C47E291446FF008D02B817C81CDE * (*) (GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F *, const RuntimeMethod*))GameObject_GetComponent_TisRuntimeObject_mE03C66715289D7957CA068A675826B7EE0887BE3_gshared)(__this, method);
}
// UnityEngine.Vector2 UnityEngine.Vector2::op_Implicit(UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  Vector2_op_Implicit_mEA1F75961E3D368418BA8CEB9C40E55C25BA3C28 (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___v0, const RuntimeMethod* method);
// System.Void UnityEngine.Rigidbody2D::set_velocity(UnityEngine.Vector2)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Rigidbody2D_set_velocity_mE0DBCE5B683024B106C2AB6943BBA550B5BD0B83 (Rigidbody2D_tBDC6900A76D3C47E291446FF008D02B817C81CDE * __this, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___value0, const RuntimeMethod* method);
// UnityEngine.Quaternion UnityEngine.Networking.NetworkTransform::get_targetSyncRotation3D()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  NetworkTransform_get_targetSyncRotation3D_m6418875DB7CC2500B5E0778D6BC890D2583B4DF8_inline (NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * __this, const RuntimeMethod* method);
// System.Single UnityEngine.Networking.NetworkTransform::get_targetSyncRotation2D()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR float NetworkTransform_get_targetSyncRotation2D_mE1F4E6611853B634322EE9EF4517E7E2AF169BEA_inline (NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * __this, const RuntimeMethod* method);
// UnityEngine.Quaternion UnityEngine.Quaternion::Euler(System.Single,System.Single,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  Quaternion_Euler_m537DD6CEAE0AD4274D8A84414C24C30730427D05 (float ___x0, float ___y1, float ___z2, const RuntimeMethod* method);
// System.Void UnityEngine.Transform::set_rotation(UnityEngine.Quaternion)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Transform_set_rotation_m429694E264117C6DC682EC6AF45C7864E5155935 (Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * __this, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___value0, const RuntimeMethod* method);
// System.Single UnityEngine.Networking.NetworkTransform::get_lastSyncTime()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR float NetworkTransform_get_lastSyncTime_mD8AEBC7EDA370ACB0A222BF622BD95C54EBD6C9E_inline (NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Material::SetPass(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Material_SetPass_m4BE0A8FCBF158C83522AA2F69118A2FE33683918 (Material_tF7DB3BF0C24DEC2FE0CB51E5DF5053D5223C8598 * __this, int32_t ___pass0, const RuntimeMethod* method);
// System.Void UnityEngine.GL::Begin(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GL_Begin_m9A48BD6A2DA850D54250EF638DF5EC61F83E293C (int32_t ___mode0, const RuntimeMethod* method);
// UnityEngine.Color UnityEngine.Color::get_white()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  Color_get_white_mE7F3AC4FF0D6F35E48049C73116A222CBE96D905 (const RuntimeMethod* method);
// System.Void UnityEngine.GL::Color(UnityEngine.Color)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GL_Color_m6F50BBCC316C56A746CDF224DE1A27FEEB359D8E (Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  ___c0, const RuntimeMethod* method);
// System.Void UnityEngine.GL::Vertex3(System.Single,System.Single,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GL_Vertex3_mE94809C1522CE96DF4C6CD218B1A26D5E60A114E (float ___x0, float ___y1, float ___z2, const RuntimeMethod* method);
// System.Void UnityEngine.GL::End()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void GL_End_m7EDEB843BD9F7E00BD838FDE074B4688C55C0755 (const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkTransformVisualizer::DrawRotationInterpolation()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformVisualizer_DrawRotationInterpolation_m61EA01F463B4524948B44B9142347C44B9C5A0B0 (NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386 * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Quaternion::op_Equality(UnityEngine.Quaternion,UnityEngine.Quaternion)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Quaternion_op_Equality_m0DBCE8FE48EEF2D7C79741E498BFFB984DF4956F (Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___lhs0, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___rhs1, const RuntimeMethod* method);
// UnityEngine.Color UnityEngine.Color::get_yellow()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  Color_get_yellow_mC8BD62CCC364EA5FC4273D4C2E116D0E2DE135AE (const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Transform::get_right()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Transform_get_right_mC32CE648E98D3D4F62F897A2751EE567C7C0CFB0 (Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * __this, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Vector3::op_Addition(UnityEngine.Vector3,UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Vector3_op_Addition_m929F9C17E5D11B94D50B4AFF1D730B70CB59B50E (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___a0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___b1, const RuntimeMethod* method);
// UnityEngine.Color UnityEngine.Color::get_green()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  Color_get_green_mD53D8F980E92A0755759FBB2981E3DDEFCD084C0 (const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Vector3::get_right()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Vector3_get_right_m6DD9559CA0C75BBA42D9140021C4C2A9AAA9B3F5 (const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Quaternion::op_Multiply(UnityEngine.Quaternion,UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Quaternion_op_Multiply_mD5999DE317D808808B72E58E7A978C4C0995879C (Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___rotation0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___point1, const RuntimeMethod* method);
// System.Boolean UnityEngine.Object::op_Implicit(UnityEngine.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool Object_op_Implicit_m8B2A44B4B1406ED346D1AE6D962294FD58D0D534 (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * ___exists0, const RuntimeMethod* method);
// UnityEngine.Shader UnityEngine.Shader::Find(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Shader_tE2731FF351B74AB4186897484FB01E000C1160CA * Shader_Find_m755654AA68D1C663A3E20A10E00CDC10F96C962B (String_t* ___name0, const RuntimeMethod* method);
// System.Void UnityEngine.Material::.ctor(UnityEngine.Shader)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Material__ctor_m81E76B5C1316004F25D4FE9CEC0E78A7428DABA8 (Material_tF7DB3BF0C24DEC2FE0CB51E5DF5053D5223C8598 * __this, Shader_tE2731FF351B74AB4186897484FB01E000C1160CA * ___shader0, const RuntimeMethod* method);
// System.Void UnityEngine.Object::set_hideFlags(UnityEngine.HideFlags)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object_set_hideFlags_mB0B45A19A5871EF407D7B09E0EB76003496BA4F0 (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * __this, int32_t ___value0, const RuntimeMethod* method);
// System.Void UnityEngine.Material::SetInt(System.String,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Material_SetInt_m1FCBDBB985E6A299AE11C3D8AF29BB4D7C7DF278 (Material_tF7DB3BF0C24DEC2FE0CB51E5DF5053D5223C8598 * __this, String_t* ___name0, int32_t ___value1, const RuntimeMethod* method);
// System.Void System.Object::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0 (RuntimeObject * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetBuffer::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetBuffer__ctor_m2E59DFECCECE03A1FEC0A37B544DF3C75E4137DD (NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * __this, const RuntimeMethod* method);
// System.Void System.Text.UTF8Encoding::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UTF8Encoding__ctor_m999E138A2E4C290F8A97866714EE53D58C931488 (UTF8Encoding_t77ED103B749A387EF072C3429F48C91D12CA08DE * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetBuffer::.ctor(System.Byte[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetBuffer__ctor_m5AE89C6DC720184249448D73CF59ACC7B58E3CBF (NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * __this, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___buffer0, const RuntimeMethod* method);
// System.UInt32 UnityEngine.Networking.NetBuffer::get_Position()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR uint32_t NetBuffer_get_Position_m1F0C4B8C3EDCCB0D65CE51B4709FDAF2017938AB_inline (NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * __this, const RuntimeMethod* method);
// System.ArraySegment`1<System.Byte> UnityEngine.Networking.NetBuffer::AsArraySegment()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA  NetBuffer_AsArraySegment_mF6086E61EC8BCA66D2AD8DE5F271075E8BDF47EC (NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * __this, const RuntimeMethod* method);
// System.Int32 System.ArraySegment`1<System.Byte>::get_Count()
inline int32_t ArraySegment_1_get_Count_m02387DADA172F909FD346559D93990E990E05352_inline (ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA * __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA *, const RuntimeMethod*))ArraySegment_1_get_Count_m02387DADA172F909FD346559D93990E990E05352_gshared_inline)(__this, method);
}
// !0[] System.ArraySegment`1<System.Byte>::get_Array()
inline ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ArraySegment_1_get_Array_m41D93EFB7EAB3081C0A27ED9891E7177F5F361B7_inline (ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA * __this, const RuntimeMethod* method)
{
	return ((  ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* (*) (ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA *, const RuntimeMethod*))ArraySegment_1_get_Array_m41D93EFB7EAB3081C0A27ED9891E7177F5F361B7_gshared_inline)(__this, method);
}
// System.Void System.Array::Copy(System.Array,System.Array,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Array_Copy_m2D96731C600DE8A167348CA8BA796344E64F7434 (RuntimeArray * ___sourceArray0, RuntimeArray * ___destinationArray1, int32_t ___length2, const RuntimeMethod* method);
// System.ArraySegment`1<System.Byte> UnityEngine.Networking.NetworkWriter::AsArraySegment()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA  NetworkWriter_AsArraySegment_m4CF129BE51C5B5F2E1BD5EB4AA5D8B70E06E4A97 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.Byte)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, uint8_t ___value0, const RuntimeMethod* method);
// System.UInt32 UnityEngine.Networking.NetworkInstanceId::get_Value()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR uint32_t NetworkInstanceId_get_Value_m63FB00D0A8272D39B6C7F7C490A8190F0E95F67F_inline (NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615 * __this, const RuntimeMethod* method);
// System.UInt32 UnityEngine.Networking.NetworkSceneId::get_Value()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR uint32_t NetworkSceneId_get_Value_m917E56DBEDC97969F7AC83B42A1F53C21DC1A9A3_inline (NetworkSceneId_t462EC62A23A1B7AF60637C48CD916A09BC493340 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetBuffer::WriteByte(System.Byte)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetBuffer_WriteByte_m8F13CD997A9D3C72EEF9FB9B40E9F088FFC8FD20 (NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * __this, uint8_t ___value0, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetBuffer::WriteByte2(System.Byte,System.Byte)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetBuffer_WriteByte2_m214A4267B67CD5BC4BF3F74EEC256774E2E5FB55 (NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * __this, uint8_t ___value00, uint8_t ___value11, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetBuffer::WriteByte4(System.Byte,System.Byte,System.Byte,System.Byte)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetBuffer_WriteByte4_m9E62F33A8B124C327F187FAA1E1C275FA8CFB958 (NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * __this, uint8_t ___value00, uint8_t ___value11, uint8_t ___value22, uint8_t ___value33, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetBuffer::WriteByte8(System.Byte,System.Byte,System.Byte,System.Byte,System.Byte,System.Byte,System.Byte,System.Byte)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetBuffer_WriteByte8_m3F07A209557DFD1C4956AD51776C5695B4967012 (NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * __this, uint8_t ___value00, uint8_t ___value11, uint8_t ___value22, uint8_t ___value33, uint8_t ___value44, uint8_t ___value55, uint8_t ___value66, uint8_t ___value77, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_mC08A0A307CE86A9CE57B009A5656C1419C824A8F (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, uint32_t ___value0, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.UInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_mD1A7B0686E93732F4086FE17AAE75596E55F5946 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, uint64_t ___value0, const RuntimeMethod* method);
// System.Int32[] System.Decimal::GetBits(System.Decimal)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* Decimal_GetBits_m581C2DB9823AC9CD84817738A740E8A7D39609BF (Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  ___d0, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_mDDA79C3C63ED882F1895E9D71DB483284CBE9609 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, int32_t ___value0, const RuntimeMethod* method);
// System.Int32 System.String::get_Length()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t String_get_Length_mD48C8A16A5CF1914F330DCE82D9BE15C3BEDD018_inline (String_t* __this, const RuntimeMethod* method);
// System.String System.String::Concat(System.Object,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Concat_mBB19C73816BDD1C3519F248E1ADC8E11A6FDB495 (RuntimeObject * ___arg00, RuntimeObject * ___arg11, const RuntimeMethod* method);
// System.Void System.IndexOutOfRangeException::.ctor(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void IndexOutOfRangeException__ctor_mCCE2EFF47A0ACB4B2636F63140F94FCEA71A9BCA (IndexOutOfRangeException_tEC7665FC66525AB6A6916A7EB505E5591683F0CF * __this, String_t* ___message0, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.UInt16)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_mA00075C21036F9B7D020332BA99CCB2687D1C835 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, uint16_t ___value0, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetBuffer::WriteBytes(System.Byte[],System.UInt16)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetBuffer_WriteBytes_m899E9F103BFA5827EFA91A6E3B17F0E12B78F94F (NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * __this, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___buffer0, uint16_t ___count1, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetBuffer::WriteBytesAtOffset(System.Byte[],System.UInt16,System.UInt16)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetBuffer_WriteBytesAtOffset_m812B345F53E3717A654F972244E65B535651676F (NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * __this, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___buffer0, uint16_t ___targetOffset1, uint16_t ___count2, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, float ___value0, const RuntimeMethod* method);
// System.Single UnityEngine.Rect::get_xMin()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float Rect_get_xMin_mFDFA74F66595FD2B8CE360183D1A92B575F0A76E (Rect_t35B976DE901B5423C11705E156938EA27AB402CE * __this, const RuntimeMethod* method);
// System.Single UnityEngine.Rect::get_yMin()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float Rect_get_yMin_m31EDC3262BE39D2F6464B15397F882237E6158C3 (Rect_t35B976DE901B5423C11705E156938EA27AB402CE * __this, const RuntimeMethod* method);
// System.Single UnityEngine.Rect::get_width()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float Rect_get_width_m54FF69FC2C086E2DC349ED091FD0D6576BFB1484 (Rect_t35B976DE901B5423C11705E156938EA27AB402CE * __this, const RuntimeMethod* method);
// System.Single UnityEngine.Rect::get_height()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float Rect_get_height_m088C36990E0A255C5D7DCE36575DCE23ABB364B5 (Rect_t35B976DE901B5423C11705E156938EA27AB402CE * __this, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Plane::get_normal()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Plane_get_normal_m203D43F51C449990214D04F332E8261295162E84 (Plane_t0903921088DEEDE1BCDEA5BF279EDBCFC9679AED * __this, const RuntimeMethod* method);
// System.Single UnityEngine.Plane::get_distance()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float Plane_get_distance_m5358B80C35E1E295C0133E7DC6449BB09C456DEE (Plane_t0903921088DEEDE1BCDEA5BF279EDBCFC9679AED * __this, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Ray::get_direction()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Ray_get_direction_m9E6468CD87844B437FC4B93491E63D388322F76E (Ray_tE2163D4CB3E6B267E29F8ABE41684490E4A614B2 * __this, const RuntimeMethod* method);
// UnityEngine.Vector3 UnityEngine.Ray::get_origin()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  Ray_get_origin_m3773CA7B1E2F26F6F1447652B485D86C0BEC5187 (Ray_tE2163D4CB3E6B267E29F8ABE41684490E4A614B2 * __this, const RuntimeMethod* method);
// UnityEngine.Networking.NetworkInstanceId UnityEngine.Networking.NetworkIdentity::get_netId()
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  NetworkIdentity_get_netId_m22EB7CD04E2633FFAF99093749F79816B2BC9F28_inline (NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * __this, const RuntimeMethod* method);
// !!0 UnityEngine.GameObject::GetComponent<UnityEngine.Networking.NetworkIdentity>()
inline NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * GameObject_GetComponent_TisNetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B_m818B3B379B25E13EF0599E7709067A3E3F4B50FD (GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * __this, const RuntimeMethod* method)
{
	return ((  NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * (*) (GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F *, const RuntimeMethod*))GameObject_GetComponent_TisRuntimeObject_mE03C66715289D7957CA068A675826B7EE0887BE3_gshared)(__this, method);
}
// System.Void UnityEngine.Networking.NetBuffer::SeekZero()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetBuffer_SeekZero_mDFE0EB8B9FD542812FEC8935D1E767A690C6CE1E (NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkWriter::SeekZero()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_SeekZero_m14C6B4B8929557795BB4DC4D4CFADFBE3D10EA87 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.Int16)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m9292C4A6802A8A84548CE8FC02CF90DB05720C2E (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, int16_t ___value0, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetBuffer::FinishMessage()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetBuffer_FinishMessage_m75D5A784D18C0356FBF7E8FED96B7225F41D1E6D (NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * __this, const RuntimeMethod* method);
// System.String UnityEngine.Networking.NetworkInstanceId::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* NetworkInstanceId_ToString_m7550B88A961DA2A10D73F0E7D9739BD8715415ED (NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615 * __this, const RuntimeMethod* method);
// System.String UnityEngine.Object::get_name()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* Object_get_name_mA2D400141CB3C991C87A2556429781DE961A83CE (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 * __this, const RuntimeMethod* method);
// System.String System.String::Format(System.String,System.Object[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Format_mA3AC3FE7B23D97F3A5BAA082D25B0E01B341A865 (String_t* ___format0, ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* ___args1, const RuntimeMethod* method);
// System.Void System.Attribute::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Attribute__ctor_m45CAD4B01265CC84CC5A84F62EE2DBE85DE89EC0 (Attribute_tF048C13FB3C8CFCC53F82290E4A3F621089F9A74 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m68E1030824D76CD6B46468FDC290B55C11D944C5 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, bool ___value0, const RuntimeMethod* method);
// System.Boolean UnityEngine.Networking.NetworkReader::ReadBoolean()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool NetworkReader_ReadBoolean_m6B4DCD23E4E794EEEA321B677BAE88E78A483CDF (NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * __this, const RuntimeMethod* method);
// System.UInt16 UnityEngine.Networking.NetworkReader::ReadUInt16()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint16_t NetworkReader_ReadUInt16_m736BE183C9CBBB8A74C74038285C148746C2322F (NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.SyncListBool::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListBool__ctor_m2BF7E2F5C16E5798F3DAD0B7C75DC606B00FF94C (SyncListBool_t4530597403BBB668F776B32DE46A1A91623EE054 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.SyncList`1<System.Boolean>::AddInternal(T)
inline void SyncList_1_AddInternal_m977B3CE5458FB772939C4CDB6612918FFC0BD427 (SyncList_1_tDDF00E08E649A86264E50205CB99495D1AD2E8D0 * __this, bool ___item0, const RuntimeMethod* method)
{
	((  void (*) (SyncList_1_tDDF00E08E649A86264E50205CB99495D1AD2E8D0 *, bool, const RuntimeMethod*))SyncList_1_AddInternal_m977B3CE5458FB772939C4CDB6612918FFC0BD427_gshared)(__this, ___item0, method);
}
// System.Void UnityEngine.Networking.SyncList`1<System.Boolean>::Clear()
inline void SyncList_1_Clear_mC367BED8954C65BFA956C2A66885A8FA241443E0 (SyncList_1_tDDF00E08E649A86264E50205CB99495D1AD2E8D0 * __this, const RuntimeMethod* method)
{
	((  void (*) (SyncList_1_tDDF00E08E649A86264E50205CB99495D1AD2E8D0 *, const RuntimeMethod*))SyncList_1_Clear_mC367BED8954C65BFA956C2A66885A8FA241443E0_gshared)(__this, method);
}
// System.Int32 UnityEngine.Networking.SyncList`1<System.Boolean>::get_Count()
inline int32_t SyncList_1_get_Count_m9EBDDB18AA65B4522E066D29FE2ECD9980BDEAD9 (SyncList_1_tDDF00E08E649A86264E50205CB99495D1AD2E8D0 * __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (SyncList_1_tDDF00E08E649A86264E50205CB99495D1AD2E8D0 *, const RuntimeMethod*))SyncList_1_get_Count_m9EBDDB18AA65B4522E066D29FE2ECD9980BDEAD9_gshared)(__this, method);
}
// T UnityEngine.Networking.SyncList`1<System.Boolean>::get_Item(System.Int32)
inline bool SyncList_1_get_Item_m0EEA26E6C3ED4695254E4D9AC8243023AE227A48 (SyncList_1_tDDF00E08E649A86264E50205CB99495D1AD2E8D0 * __this, int32_t ___i0, const RuntimeMethod* method)
{
	return ((  bool (*) (SyncList_1_tDDF00E08E649A86264E50205CB99495D1AD2E8D0 *, int32_t, const RuntimeMethod*))SyncList_1_get_Item_m0EEA26E6C3ED4695254E4D9AC8243023AE227A48_gshared)(__this, ___i0, method);
}
// System.Void UnityEngine.Networking.SyncList`1<System.Boolean>::.ctor()
inline void SyncList_1__ctor_m1BB28896D4C843EEF83232CE6648F916429D54E3 (SyncList_1_tDDF00E08E649A86264E50205CB99495D1AD2E8D0 * __this, const RuntimeMethod* method)
{
	((  void (*) (SyncList_1_tDDF00E08E649A86264E50205CB99495D1AD2E8D0 *, const RuntimeMethod*))SyncList_1__ctor_m1BB28896D4C843EEF83232CE6648F916429D54E3_gshared)(__this, method);
}
// System.Single UnityEngine.Networking.NetworkReader::ReadSingle()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float NetworkReader_ReadSingle_mA5EE4F2C6A2FE9AA84AFC4FA0705B8CDAA7A4AAF (NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.SyncListFloat::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListFloat__ctor_m68F03DF4317EADAA861FA0D251C797FD7CFA28ED (SyncListFloat_tC8F12C17B783518D34953712B51249276C506922 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.SyncList`1<System.Single>::AddInternal(T)
inline void SyncList_1_AddInternal_mC17F547D0099E43ACAA4C5FD21D63DDE456602A6 (SyncList_1_tDEB03E3C5252571915662095C7060998910FD0A2 * __this, float ___item0, const RuntimeMethod* method)
{
	((  void (*) (SyncList_1_tDEB03E3C5252571915662095C7060998910FD0A2 *, float, const RuntimeMethod*))SyncList_1_AddInternal_mC17F547D0099E43ACAA4C5FD21D63DDE456602A6_gshared)(__this, ___item0, method);
}
// System.Void UnityEngine.Networking.SyncList`1<System.Single>::Clear()
inline void SyncList_1_Clear_m13160DF80DA71AAF005006E14C5C8985DBF15EB5 (SyncList_1_tDEB03E3C5252571915662095C7060998910FD0A2 * __this, const RuntimeMethod* method)
{
	((  void (*) (SyncList_1_tDEB03E3C5252571915662095C7060998910FD0A2 *, const RuntimeMethod*))SyncList_1_Clear_m13160DF80DA71AAF005006E14C5C8985DBF15EB5_gshared)(__this, method);
}
// System.Int32 UnityEngine.Networking.SyncList`1<System.Single>::get_Count()
inline int32_t SyncList_1_get_Count_mCC0838D9ED25E463384E4852839E47B100C99577 (SyncList_1_tDEB03E3C5252571915662095C7060998910FD0A2 * __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (SyncList_1_tDEB03E3C5252571915662095C7060998910FD0A2 *, const RuntimeMethod*))SyncList_1_get_Count_mCC0838D9ED25E463384E4852839E47B100C99577_gshared)(__this, method);
}
// T UnityEngine.Networking.SyncList`1<System.Single>::get_Item(System.Int32)
inline float SyncList_1_get_Item_m70C832E1FED3E2D52297C7B6EF187700309BF7D4 (SyncList_1_tDEB03E3C5252571915662095C7060998910FD0A2 * __this, int32_t ___i0, const RuntimeMethod* method)
{
	return ((  float (*) (SyncList_1_tDEB03E3C5252571915662095C7060998910FD0A2 *, int32_t, const RuntimeMethod*))SyncList_1_get_Item_m70C832E1FED3E2D52297C7B6EF187700309BF7D4_gshared)(__this, ___i0, method);
}
// System.Void UnityEngine.Networking.SyncList`1<System.Single>::.ctor()
inline void SyncList_1__ctor_mACF8E6F1689E85F8D9D88F6B2366C1A08D6F853E (SyncList_1_tDEB03E3C5252571915662095C7060998910FD0A2 * __this, const RuntimeMethod* method)
{
	((  void (*) (SyncList_1_tDEB03E3C5252571915662095C7060998910FD0A2 *, const RuntimeMethod*))SyncList_1__ctor_mACF8E6F1689E85F8D9D88F6B2366C1A08D6F853E_gshared)(__this, method);
}
// System.Void UnityEngine.Networking.SyncListInt::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListInt__ctor_m9A8426FDD81908FDA8B94E67751AB67D0C52D90A (SyncListInt_t6D5125D26D629A9DB1325266BCFCDF2FC86FD9C6 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.SyncList`1<System.Int32>::AddInternal(T)
inline void SyncList_1_AddInternal_m93CDCB4D3061B2F4CF88B74DEABE1C06D4AED23C (SyncList_1_t8595ACD08C8686AC9547A225ACF209C171FF0665 * __this, int32_t ___item0, const RuntimeMethod* method)
{
	((  void (*) (SyncList_1_t8595ACD08C8686AC9547A225ACF209C171FF0665 *, int32_t, const RuntimeMethod*))SyncList_1_AddInternal_m93CDCB4D3061B2F4CF88B74DEABE1C06D4AED23C_gshared)(__this, ___item0, method);
}
// System.Void UnityEngine.Networking.SyncList`1<System.Int32>::Clear()
inline void SyncList_1_Clear_mF8FAE0172014F355D0C66600D8607442BF9A03B3 (SyncList_1_t8595ACD08C8686AC9547A225ACF209C171FF0665 * __this, const RuntimeMethod* method)
{
	((  void (*) (SyncList_1_t8595ACD08C8686AC9547A225ACF209C171FF0665 *, const RuntimeMethod*))SyncList_1_Clear_mF8FAE0172014F355D0C66600D8607442BF9A03B3_gshared)(__this, method);
}
// System.Int32 UnityEngine.Networking.SyncList`1<System.Int32>::get_Count()
inline int32_t SyncList_1_get_Count_m7E687EFF75167B5EB639F273102ED345B8CB905B (SyncList_1_t8595ACD08C8686AC9547A225ACF209C171FF0665 * __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (SyncList_1_t8595ACD08C8686AC9547A225ACF209C171FF0665 *, const RuntimeMethod*))SyncList_1_get_Count_m7E687EFF75167B5EB639F273102ED345B8CB905B_gshared)(__this, method);
}
// T UnityEngine.Networking.SyncList`1<System.Int32>::get_Item(System.Int32)
inline int32_t SyncList_1_get_Item_mA89484861CD0098C5FC7466F93F18C4EE231C55F (SyncList_1_t8595ACD08C8686AC9547A225ACF209C171FF0665 * __this, int32_t ___i0, const RuntimeMethod* method)
{
	return ((  int32_t (*) (SyncList_1_t8595ACD08C8686AC9547A225ACF209C171FF0665 *, int32_t, const RuntimeMethod*))SyncList_1_get_Item_mA89484861CD0098C5FC7466F93F18C4EE231C55F_gshared)(__this, ___i0, method);
}
// System.Void UnityEngine.Networking.SyncList`1<System.Int32>::.ctor()
inline void SyncList_1__ctor_m6E3A6F39EE2A332965D0B912FD1662297B44B901 (SyncList_1_t8595ACD08C8686AC9547A225ACF209C171FF0665 * __this, const RuntimeMethod* method)
{
	((  void (*) (SyncList_1_t8595ACD08C8686AC9547A225ACF209C171FF0665 *, const RuntimeMethod*))SyncList_1__ctor_m6E3A6F39EE2A332965D0B912FD1662297B44B901_gshared)(__this, method);
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m856F6DD1E132E2C68BA9D7D36A5ED5EAA1D108F4 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, String_t* ___value0, const RuntimeMethod* method);
// System.String UnityEngine.Networking.NetworkReader::ReadString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* NetworkReader_ReadString_mF004D69C1AE3038215701A8E43973D1FA7BDB364 (NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.SyncListString::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListString__ctor_m50D229AE4F36D878B3FBB78517104B1D34BA3F38 (SyncListString_t26D5186F91FB985D01BDC6CC5B4C7C13FA3740CD * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.SyncList`1<System.String>::AddInternal(T)
inline void SyncList_1_AddInternal_m02EF37FDD57B236ED985C01D53E7E181843A33D8 (SyncList_1_t4C2B8FAE3D901E39D62A3678DF05B8473364ED04 * __this, String_t* ___item0, const RuntimeMethod* method)
{
	((  void (*) (SyncList_1_t4C2B8FAE3D901E39D62A3678DF05B8473364ED04 *, String_t*, const RuntimeMethod*))SyncList_1_AddInternal_m366BD82F1FADDBAA377373315B6905923C1EA438_gshared)(__this, ___item0, method);
}
// System.Void UnityEngine.Networking.SyncList`1<System.String>::Clear()
inline void SyncList_1_Clear_mAF7EFFA62345875E1C183F7D3A09A57A0E05E97B (SyncList_1_t4C2B8FAE3D901E39D62A3678DF05B8473364ED04 * __this, const RuntimeMethod* method)
{
	((  void (*) (SyncList_1_t4C2B8FAE3D901E39D62A3678DF05B8473364ED04 *, const RuntimeMethod*))SyncList_1_Clear_m6FF0B87F4C015BF2212C65CA18C0DA9A4A5A24D3_gshared)(__this, method);
}
// System.Int32 UnityEngine.Networking.SyncList`1<System.String>::get_Count()
inline int32_t SyncList_1_get_Count_m641E2517509914AAC0415508A728F40A914318C4 (SyncList_1_t4C2B8FAE3D901E39D62A3678DF05B8473364ED04 * __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (SyncList_1_t4C2B8FAE3D901E39D62A3678DF05B8473364ED04 *, const RuntimeMethod*))SyncList_1_get_Count_mACAE966F2F87AF836A946945D131D0EB48FB9F2B_gshared)(__this, method);
}
// T UnityEngine.Networking.SyncList`1<System.String>::get_Item(System.Int32)
inline String_t* SyncList_1_get_Item_m0578989F729AF1CD8C5F378289B5DF1FA830AE16 (SyncList_1_t4C2B8FAE3D901E39D62A3678DF05B8473364ED04 * __this, int32_t ___i0, const RuntimeMethod* method)
{
	return ((  String_t* (*) (SyncList_1_t4C2B8FAE3D901E39D62A3678DF05B8473364ED04 *, int32_t, const RuntimeMethod*))SyncList_1_get_Item_m31396929FCFEC092BC74AD65E682CBFDCA7A6AA2_gshared)(__this, ___i0, method);
}
// System.Void UnityEngine.Networking.SyncList`1<System.String>::.ctor()
inline void SyncList_1__ctor_mBBB1AF24E09B273530603FA90034B2B830E2460C (SyncList_1_t4C2B8FAE3D901E39D62A3678DF05B8473364ED04 * __this, const RuntimeMethod* method)
{
	((  void (*) (SyncList_1_t4C2B8FAE3D901E39D62A3678DF05B8473364ED04 *, const RuntimeMethod*))SyncList_1__ctor_mEFD8368B5D9178E9DA189A23F22FD902E193C791_gshared)(__this, method);
}
// System.Void UnityEngine.Networking.SyncListUInt::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListUInt__ctor_mBAD30E72F2FB4BFA239B5DDABCCFC0DEEFD918AC (SyncListUInt_tF223A88F804D7F2819F5F610669176CE6E93A0E6 * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.SyncList`1<System.UInt32>::AddInternal(T)
inline void SyncList_1_AddInternal_m84938C896AA1F3EED3568ECB90FED244DF2617B2 (SyncList_1_t6931FBA5633802C1CAE04093D0C6D9C88EE66627 * __this, uint32_t ___item0, const RuntimeMethod* method)
{
	((  void (*) (SyncList_1_t6931FBA5633802C1CAE04093D0C6D9C88EE66627 *, uint32_t, const RuntimeMethod*))SyncList_1_AddInternal_m84938C896AA1F3EED3568ECB90FED244DF2617B2_gshared)(__this, ___item0, method);
}
// System.Void UnityEngine.Networking.SyncList`1<System.UInt32>::Clear()
inline void SyncList_1_Clear_m00C3496EAD8E618F4C20CA6F618373D4564CEB58 (SyncList_1_t6931FBA5633802C1CAE04093D0C6D9C88EE66627 * __this, const RuntimeMethod* method)
{
	((  void (*) (SyncList_1_t6931FBA5633802C1CAE04093D0C6D9C88EE66627 *, const RuntimeMethod*))SyncList_1_Clear_m00C3496EAD8E618F4C20CA6F618373D4564CEB58_gshared)(__this, method);
}
// System.Int32 UnityEngine.Networking.SyncList`1<System.UInt32>::get_Count()
inline int32_t SyncList_1_get_Count_m29E32BA907E6C50793D6A2D30D22A8D052A978B8 (SyncList_1_t6931FBA5633802C1CAE04093D0C6D9C88EE66627 * __this, const RuntimeMethod* method)
{
	return ((  int32_t (*) (SyncList_1_t6931FBA5633802C1CAE04093D0C6D9C88EE66627 *, const RuntimeMethod*))SyncList_1_get_Count_m29E32BA907E6C50793D6A2D30D22A8D052A978B8_gshared)(__this, method);
}
// T UnityEngine.Networking.SyncList`1<System.UInt32>::get_Item(System.Int32)
inline uint32_t SyncList_1_get_Item_mC1369C43D41DC4C7863526B187E820DD7DA3709D (SyncList_1_t6931FBA5633802C1CAE04093D0C6D9C88EE66627 * __this, int32_t ___i0, const RuntimeMethod* method)
{
	return ((  uint32_t (*) (SyncList_1_t6931FBA5633802C1CAE04093D0C6D9C88EE66627 *, int32_t, const RuntimeMethod*))SyncList_1_get_Item_mC1369C43D41DC4C7863526B187E820DD7DA3709D_gshared)(__this, ___i0, method);
}
// System.Void UnityEngine.Networking.SyncList`1<System.UInt32>::.ctor()
inline void SyncList_1__ctor_mF18B74E2EF8296E263BCEBAB8C8DE0EA78F8BAFC (SyncList_1_t6931FBA5633802C1CAE04093D0C6D9C88EE66627 * __this, const RuntimeMethod* method)
{
	((  void (*) (SyncList_1_t6931FBA5633802C1CAE04093D0C6D9C88EE66627 *, const RuntimeMethod*))SyncList_1__ctor_mF18B74E2EF8296E263BCEBAB8C8DE0EA78F8BAFC_gshared)(__this, method);
}
// System.Void UnityEngine.Networking.NetworkConnection::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkConnection__ctor_mDD96E228FE96C836C690ADBFDC26C3FFDA31CEC9 (NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * __this, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.LocalClient::InvokeHandlerOnClient(System.Int16,UnityEngine.Networking.MessageBase,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void LocalClient_InvokeHandlerOnClient_mA31AB4C0AAE4A3B392F5D843904B1EBF614646ED (LocalClient_tCEC0096B13C433140FD4C09424CE345B28FE3C86 * __this, int16_t ___msgType0, MessageBase_t2EA42B01AD6A5F36EAF84BE623801951B9F55416 * ___msg1, int32_t ___channelId2, const RuntimeMethod* method);
// System.Void UnityEngine.Networking.LocalClient::InvokeBytesOnClient(System.Byte[],System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void LocalClient_InvokeBytesOnClient_m1EFDE0A0688D78F165E5340E2C399CC47269866E (LocalClient_tCEC0096B13C433140FD4C09424CE345B28FE3C86 * __this, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___buffer0, int32_t ___channelId1, const RuntimeMethod* method);
// System.Byte[] UnityEngine.Networking.NetworkWriter::AsArray()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* NetworkWriter_AsArray_mE90AC762796F17DD398523A8C230DD9B2E2373D5 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, const RuntimeMethod* method);
// System.Boolean UnityEngine.Networking.NetworkServer::InvokeHandlerOnServer(UnityEngine.Networking.ULocalConnectionToServer,System.Int16,UnityEngine.Networking.MessageBase,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool NetworkServer_InvokeHandlerOnServer_m24971E0FB8CA3BD5D9557EC82349C19B5D369CE2 (NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1 * __this, ULocalConnectionToServer_tE6E34057F329C3E0E703C6F095DF82B0270557B8 * ___conn0, int16_t ___msgType1, MessageBase_t2EA42B01AD6A5F36EAF84BE623801951B9F55416 * ___msg2, int32_t ___channelId3, const RuntimeMethod* method);
// System.Boolean UnityEngine.Networking.NetworkServer::InvokeBytes(UnityEngine.Networking.ULocalConnectionToServer,System.Byte[],System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool NetworkServer_InvokeBytes_mE35B20779E0DA27C27860D3F7EB24FC7D2E35754 (NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1 * __this, ULocalConnectionToServer_tE6E34057F329C3E0E703C6F095DF82B0270557B8 * ___conn0, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___buffer1, int32_t ___numBytes2, int32_t ___channelId3, const RuntimeMethod* method);
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
IL2CPP_EXTERN_C  bool DelegatePInvokeWrapper_ClientMoveCallback2D_tDFAD7DD6998C835AD2376F25136794AB12BA81A2 (ClientMoveCallback2D_tDFAD7DD6998C835AD2376F25136794AB12BA81A2 * __this, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * ___position0, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * ___velocity1, float* ___rotation2, const RuntimeMethod* method)
{
	typedef int32_t (DEFAULT_CALL *PInvokeFunc)(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D *, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D *, float*);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	int32_t returnValue = il2cppPInvokeFunc(___position0, ___velocity1, ___rotation2);

	return static_cast<bool>(returnValue);
}
// System.Void UnityEngine.Networking.NetworkTransform_ClientMoveCallback2D::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ClientMoveCallback2D__ctor_m12E0E657DAB0B35C096BB9FFBF576DD495ECBCD5 (ClientMoveCallback2D_tDFAD7DD6998C835AD2376F25136794AB12BA81A2 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Boolean UnityEngine.Networking.NetworkTransform_ClientMoveCallback2D::Invoke(UnityEngine.Vector2&,UnityEngine.Vector2&,System.Single&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ClientMoveCallback2D_Invoke_mA91CCED4BBE5E7130F5E393EB59369E73F6F7C40 (ClientMoveCallback2D_tDFAD7DD6998C835AD2376F25136794AB12BA81A2 * __this, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * ___position0, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * ___velocity1, float* ___rotation2, const RuntimeMethod* method)
{
	bool result = false;
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
				typedef bool (*FunctionPointerType) (Vector2_tA85D2DD88578276CA8A8796756458277E72D073D *, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D *, float*, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(___position0, ___velocity1, ___rotation2, targetMethod);
			}
			else
			{
				// closed
				typedef bool (*FunctionPointerType) (void*, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D *, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D *, float*, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___position0, ___velocity1, ___rotation2, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef bool (*FunctionPointerType) (Vector2_tA85D2DD88578276CA8A8796756458277E72D073D *, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D *, float*, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)(___position0, ___velocity1, ___rotation2, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = GenericInterfaceFuncInvoker3< bool, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D *, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D *, float* >::Invoke(targetMethod, targetThis, ___position0, ___velocity1, ___rotation2);
					else
						result = GenericVirtFuncInvoker3< bool, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D *, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D *, float* >::Invoke(targetMethod, targetThis, ___position0, ___velocity1, ___rotation2);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = InterfaceFuncInvoker3< bool, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D *, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D *, float* >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___position0, ___velocity1, ___rotation2);
					else
						result = VirtFuncInvoker3< bool, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D *, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D *, float* >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___position0, ___velocity1, ___rotation2);
				}
			}
			else
			{
				typedef bool (*FunctionPointerType) (void*, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D *, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D *, float*, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___position0, ___velocity1, ___rotation2, targetMethod);
			}
		}
	}
	return result;
}
// System.IAsyncResult UnityEngine.Networking.NetworkTransform_ClientMoveCallback2D::BeginInvoke(UnityEngine.Vector2&,UnityEngine.Vector2&,System.Single&,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* ClientMoveCallback2D_BeginInvoke_m50707DD51E2F1CD304B24C934B4C8193E503F56F (ClientMoveCallback2D_tDFAD7DD6998C835AD2376F25136794AB12BA81A2 * __this, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * ___position0, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * ___velocity1, float* ___rotation2, AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4 * ___callback3, RuntimeObject * ___object4, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ClientMoveCallback2D_BeginInvoke_m50707DD51E2F1CD304B24C934B4C8193E503F56F_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[4] = {0};
	__d_args[0] = Box(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_il2cpp_TypeInfo_var, &*___position0);
	__d_args[1] = Box(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_il2cpp_TypeInfo_var, &*___velocity1);
	__d_args[2] = Box(Single_tDDDA9169C4E4E308AC6D7A824F9B28DC82204AE1_il2cpp_TypeInfo_var, &*___rotation2);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback3, (RuntimeObject*)___object4);
}
// System.Boolean UnityEngine.Networking.NetworkTransform_ClientMoveCallback2D::EndInvoke(UnityEngine.Vector2&,UnityEngine.Vector2&,System.Single&,System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ClientMoveCallback2D_EndInvoke_m14894EBE5F801D3F0113111AD5DCB77374D4C3EF (ClientMoveCallback2D_tDFAD7DD6998C835AD2376F25136794AB12BA81A2 * __this, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * ___position0, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D * ___velocity1, float* ___rotation2, RuntimeObject* ___result3, const RuntimeMethod* method)
{
	void* ___out_args[] = {
	___position0,
	___velocity1,
	___rotation2,
	};
	RuntimeObject *__result = il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___result3, ___out_args);
	return *(bool*)UnBox ((RuntimeObject*)__result);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
IL2CPP_EXTERN_C  bool DelegatePInvokeWrapper_ClientMoveCallback3D_t8B3ABB4B5CD7D938193C0EB61634DC9424916A45 (ClientMoveCallback3D_t8B3ABB4B5CD7D938193C0EB61634DC9424916A45 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___position0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___velocity1, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 * ___rotation2, const RuntimeMethod* method)
{
	typedef int32_t (DEFAULT_CALL *PInvokeFunc)(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 *);
	PInvokeFunc il2cppPInvokeFunc = reinterpret_cast<PInvokeFunc>(il2cpp_codegen_get_method_pointer(((RuntimeDelegate*)__this)->method));

	// Native function invocation
	int32_t returnValue = il2cppPInvokeFunc(___position0, ___velocity1, ___rotation2);

	return static_cast<bool>(returnValue);
}
// System.Void UnityEngine.Networking.NetworkTransform_ClientMoveCallback3D::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ClientMoveCallback3D__ctor_m254D5CACD1F62E7BB342E734BD2B8C93764C8A38 (ClientMoveCallback3D_t8B3ABB4B5CD7D938193C0EB61634DC9424916A45 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Boolean UnityEngine.Networking.NetworkTransform_ClientMoveCallback3D::Invoke(UnityEngine.Vector3&,UnityEngine.Vector3&,UnityEngine.Quaternion&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ClientMoveCallback3D_Invoke_m4F4CED5C02FAFD6145BBA95A6B5261ACB9E0B19C (ClientMoveCallback3D_t8B3ABB4B5CD7D938193C0EB61634DC9424916A45 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___position0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___velocity1, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 * ___rotation2, const RuntimeMethod* method)
{
	bool result = false;
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
				typedef bool (*FunctionPointerType) (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 *, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(___position0, ___velocity1, ___rotation2, targetMethod);
			}
			else
			{
				// closed
				typedef bool (*FunctionPointerType) (void*, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 *, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___position0, ___velocity1, ___rotation2, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef bool (*FunctionPointerType) (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 *, const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)(___position0, ___velocity1, ___rotation2, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = GenericInterfaceFuncInvoker3< bool, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 * >::Invoke(targetMethod, targetThis, ___position0, ___velocity1, ___rotation2);
					else
						result = GenericVirtFuncInvoker3< bool, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 * >::Invoke(targetMethod, targetThis, ___position0, ___velocity1, ___rotation2);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = InterfaceFuncInvoker3< bool, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 * >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___position0, ___velocity1, ___rotation2);
					else
						result = VirtFuncInvoker3< bool, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 * >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___position0, ___velocity1, ___rotation2);
				}
			}
			else
			{
				typedef bool (*FunctionPointerType) (void*, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 *, const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___position0, ___velocity1, ___rotation2, targetMethod);
			}
		}
	}
	return result;
}
// System.IAsyncResult UnityEngine.Networking.NetworkTransform_ClientMoveCallback3D::BeginInvoke(UnityEngine.Vector3&,UnityEngine.Vector3&,UnityEngine.Quaternion&,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* ClientMoveCallback3D_BeginInvoke_m8122BB9D2196D4763C0E15CB1838D74254B04256 (ClientMoveCallback3D_t8B3ABB4B5CD7D938193C0EB61634DC9424916A45 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___position0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___velocity1, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 * ___rotation2, AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4 * ___callback3, RuntimeObject * ___object4, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ClientMoveCallback3D_BeginInvoke_m8122BB9D2196D4763C0E15CB1838D74254B04256_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[4] = {0};
	__d_args[0] = Box(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var, &*___position0);
	__d_args[1] = Box(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var, &*___velocity1);
	__d_args[2] = Box(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_il2cpp_TypeInfo_var, &*___rotation2);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback3, (RuntimeObject*)___object4);
}
// System.Boolean UnityEngine.Networking.NetworkTransform_ClientMoveCallback3D::EndInvoke(UnityEngine.Vector3&,UnityEngine.Vector3&,UnityEngine.Quaternion&,System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ClientMoveCallback3D_EndInvoke_mDDBA2EA07D2B29F5A71BD8449B033FF6D93DBDD0 (ClientMoveCallback3D_t8B3ABB4B5CD7D938193C0EB61634DC9424916A45 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___position0, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 * ___velocity1, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 * ___rotation2, RuntimeObject* ___result3, const RuntimeMethod* method)
{
	void* ___out_args[] = {
	___position0,
	___velocity1,
	___rotation2,
	};
	RuntimeObject *__result = il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___result3, ___out_args);
	return *(bool*)UnBox ((RuntimeObject*)__result);
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
// UnityEngine.Transform UnityEngine.Networking.NetworkTransformChild::get_target()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * NetworkTransformChild_get_target_mECE8252ECC8DA8792F957AA97012E0187F7D6A9B (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	{
		// public Transform                            target { get {return m_Target; } set { m_Target = value; OnValidate(); } }
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_0 = __this->get_m_Target_10();
		return L_0;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformChild::set_target(UnityEngine.Transform)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_set_target_m40D41F8E748BBC730F9691A7C098C5D81119752F (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * ___value0, const RuntimeMethod* method)
{
	{
		// public Transform                            target { get {return m_Target; } set { m_Target = value; OnValidate(); } }
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_0 = ___value0;
		__this->set_m_Target_10(L_0);
		// public Transform                            target { get {return m_Target; } set { m_Target = value; OnValidate(); } }
		NetworkTransformChild_OnValidate_mB8E287CF434D44F97FCBF26CE7D72BD84EA592CF(__this, /*hidden argument*/NULL);
		// public Transform                            target { get {return m_Target; } set { m_Target = value; OnValidate(); } }
		return;
	}
}
// System.UInt32 UnityEngine.Networking.NetworkTransformChild::get_childIndex()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t NetworkTransformChild_get_childIndex_m01E20EEF0C11EB925A3EABF78ACABC94B7DED540 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	{
		// public uint                                 childIndex { get { return m_ChildIndex; }}
		uint32_t L_0 = __this->get_m_ChildIndex_11();
		return L_0;
	}
}
// System.Single UnityEngine.Networking.NetworkTransformChild::get_sendInterval()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float NetworkTransformChild_get_sendInterval_mAB4E0D114C9B54EB66341631A4FB615B9CA45ABB (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	{
		// public float                                sendInterval { get { return m_SendInterval; } set { m_SendInterval = value; } }
		float L_0 = __this->get_m_SendInterval_13();
		return L_0;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformChild::set_sendInterval(System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_set_sendInterval_m169E479BC9F265CFA3663B2A1AA41822B28786B7 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, float ___value0, const RuntimeMethod* method)
{
	{
		// public float                                sendInterval { get { return m_SendInterval; } set { m_SendInterval = value; } }
		float L_0 = ___value0;
		__this->set_m_SendInterval_13(L_0);
		// public float                                sendInterval { get { return m_SendInterval; } set { m_SendInterval = value; } }
		return;
	}
}
// UnityEngine.Networking.NetworkTransform_AxisSyncMode UnityEngine.Networking.NetworkTransformChild::get_syncRotationAxis()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t NetworkTransformChild_get_syncRotationAxis_m12199E8BCADC5098C84E6F2E6A8534424FD80979 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	{
		// public NetworkTransform.AxisSyncMode        syncRotationAxis { get { return m_SyncRotationAxis; } set { m_SyncRotationAxis = value; } }
		int32_t L_0 = __this->get_m_SyncRotationAxis_14();
		return L_0;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformChild::set_syncRotationAxis(UnityEngine.Networking.NetworkTransform_AxisSyncMode)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_set_syncRotationAxis_mD8B796BD71A87C4B228DAFCF2BB8CEEF0DAF6206 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, int32_t ___value0, const RuntimeMethod* method)
{
	{
		// public NetworkTransform.AxisSyncMode        syncRotationAxis { get { return m_SyncRotationAxis; } set { m_SyncRotationAxis = value; } }
		int32_t L_0 = ___value0;
		__this->set_m_SyncRotationAxis_14(L_0);
		// public NetworkTransform.AxisSyncMode        syncRotationAxis { get { return m_SyncRotationAxis; } set { m_SyncRotationAxis = value; } }
		return;
	}
}
// UnityEngine.Networking.NetworkTransform_CompressionSyncMode UnityEngine.Networking.NetworkTransformChild::get_rotationSyncCompression()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t NetworkTransformChild_get_rotationSyncCompression_m014CA812E4BB0DBF2DF856CB96E40FBED022239B (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	{
		// public NetworkTransform.CompressionSyncMode rotationSyncCompression { get { return m_RotationSyncCompression; } set { m_RotationSyncCompression = value; } }
		int32_t L_0 = __this->get_m_RotationSyncCompression_15();
		return L_0;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformChild::set_rotationSyncCompression(UnityEngine.Networking.NetworkTransform_CompressionSyncMode)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_set_rotationSyncCompression_mD271CD2D318F5C170714180B03D96700AFD05BDD (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, int32_t ___value0, const RuntimeMethod* method)
{
	{
		// public NetworkTransform.CompressionSyncMode rotationSyncCompression { get { return m_RotationSyncCompression; } set { m_RotationSyncCompression = value; } }
		int32_t L_0 = ___value0;
		__this->set_m_RotationSyncCompression_15(L_0);
		// public NetworkTransform.CompressionSyncMode rotationSyncCompression { get { return m_RotationSyncCompression; } set { m_RotationSyncCompression = value; } }
		return;
	}
}
// System.Single UnityEngine.Networking.NetworkTransformChild::get_movementThreshold()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float NetworkTransformChild_get_movementThreshold_m9BED81E541443BA95A2DDDF7465386F0E4F5639A (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	{
		// public float                                movementThreshold { get { return m_MovementThreshold; } set { m_MovementThreshold = value; } }
		float L_0 = __this->get_m_MovementThreshold_16();
		return L_0;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformChild::set_movementThreshold(System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_set_movementThreshold_m93D1B2916BC9B686B9F40C755A0AADCE6E54AB93 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, float ___value0, const RuntimeMethod* method)
{
	{
		// public float                                movementThreshold { get { return m_MovementThreshold; } set { m_MovementThreshold = value; } }
		float L_0 = ___value0;
		__this->set_m_MovementThreshold_16(L_0);
		// public float                                movementThreshold { get { return m_MovementThreshold; } set { m_MovementThreshold = value; } }
		return;
	}
}
// System.Single UnityEngine.Networking.NetworkTransformChild::get_interpolateRotation()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float NetworkTransformChild_get_interpolateRotation_m9169822905990E0E3C3531F5812DC25FBE4C06BA (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	{
		// public float                                interpolateRotation { get { return m_InterpolateRotation; } set { m_InterpolateRotation = value; } }
		float L_0 = __this->get_m_InterpolateRotation_17();
		return L_0;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformChild::set_interpolateRotation(System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_set_interpolateRotation_mF72DC382026B2C763A63F0FB8D565275ECAEC4DF (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, float ___value0, const RuntimeMethod* method)
{
	{
		// public float                                interpolateRotation { get { return m_InterpolateRotation; } set { m_InterpolateRotation = value; } }
		float L_0 = ___value0;
		__this->set_m_InterpolateRotation_17(L_0);
		// public float                                interpolateRotation { get { return m_InterpolateRotation; } set { m_InterpolateRotation = value; } }
		return;
	}
}
// System.Single UnityEngine.Networking.NetworkTransformChild::get_interpolateMovement()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float NetworkTransformChild_get_interpolateMovement_m27491C1C3805AE420F584C937DC7DDFDB4A98E74 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	{
		// public float                                interpolateMovement { get { return m_InterpolateMovement; } set { m_InterpolateMovement = value; } }
		float L_0 = __this->get_m_InterpolateMovement_18();
		return L_0;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformChild::set_interpolateMovement(System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_set_interpolateMovement_m94A0DB8134ACE98F427C12CFA1057CAA9370CF44 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, float ___value0, const RuntimeMethod* method)
{
	{
		// public float                                interpolateMovement { get { return m_InterpolateMovement; } set { m_InterpolateMovement = value; } }
		float L_0 = ___value0;
		__this->set_m_InterpolateMovement_18(L_0);
		// public float                                interpolateMovement { get { return m_InterpolateMovement; } set { m_InterpolateMovement = value; } }
		return;
	}
}
// UnityEngine.Networking.NetworkTransform_ClientMoveCallback3D UnityEngine.Networking.NetworkTransformChild::get_clientMoveCallback3D()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ClientMoveCallback3D_t8B3ABB4B5CD7D938193C0EB61634DC9424916A45 * NetworkTransformChild_get_clientMoveCallback3D_mACEA2B7D876C9CBBFF7E64400350489049C1C187 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	{
		// public NetworkTransform.ClientMoveCallback3D clientMoveCallback3D { get { return m_ClientMoveCallback3D; } set { m_ClientMoveCallback3D = value; } }
		ClientMoveCallback3D_t8B3ABB4B5CD7D938193C0EB61634DC9424916A45 * L_0 = __this->get_m_ClientMoveCallback3D_19();
		return L_0;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformChild::set_clientMoveCallback3D(UnityEngine.Networking.NetworkTransform_ClientMoveCallback3D)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_set_clientMoveCallback3D_mDFC5581D914F9A1DDBDAAF6DECC6C27401360735 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, ClientMoveCallback3D_t8B3ABB4B5CD7D938193C0EB61634DC9424916A45 * ___value0, const RuntimeMethod* method)
{
	{
		// public NetworkTransform.ClientMoveCallback3D clientMoveCallback3D { get { return m_ClientMoveCallback3D; } set { m_ClientMoveCallback3D = value; } }
		ClientMoveCallback3D_t8B3ABB4B5CD7D938193C0EB61634DC9424916A45 * L_0 = ___value0;
		__this->set_m_ClientMoveCallback3D_19(L_0);
		// public NetworkTransform.ClientMoveCallback3D clientMoveCallback3D { get { return m_ClientMoveCallback3D; } set { m_ClientMoveCallback3D = value; } }
		return;
	}
}
// System.Single UnityEngine.Networking.NetworkTransformChild::get_lastSyncTime()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float NetworkTransformChild_get_lastSyncTime_m1B15088D066B7F380142A797492A019D3E2FA69E (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	{
		// public float                lastSyncTime { get { return m_LastClientSyncTime; } }
		float L_0 = __this->get_m_LastClientSyncTime_22();
		return L_0;
	}
}
// UnityEngine.Vector3 UnityEngine.Networking.NetworkTransformChild::get_targetSyncPosition()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  NetworkTransformChild_get_targetSyncPosition_m563FF25FFDE203605EC178FA1E42B688B5FB4E19 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	{
		// public Vector3              targetSyncPosition { get { return m_TargetSyncPosition; } }
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_0 = __this->get_m_TargetSyncPosition_20();
		return L_0;
	}
}
// UnityEngine.Quaternion UnityEngine.Networking.NetworkTransformChild::get_targetSyncRotation3D()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  NetworkTransformChild_get_targetSyncRotation3D_m32DDD3C4946E84C0A399B848A8D56EA85AFDA1D0 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	{
		// public Quaternion           targetSyncRotation3D { get { return m_TargetSyncRotation3D; } }
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_0 = __this->get_m_TargetSyncRotation3D_21();
		return L_0;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformChild::OnValidate()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_OnValidate_mB8E287CF434D44F97FCBF26CE7D72BD84EA592CF (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkTransformChild_OnValidate_mB8E287CF434D44F97FCBF26CE7D72BD84EA592CF_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * V_0 = NULL;
	NetworkTransformChildU5BU5D_tAE50AD052E14730045D7CF7E715FBC48ADEDB6C1* V_1 = NULL;
	uint32_t V_2 = 0;
	{
		// if (m_Target != null)
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_0 = __this->get_m_Target_10();
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_1 = Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1(L_0, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_0089;
		}
	}
	{
		// Transform parent = m_Target.parent;
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_2 = __this->get_m_Target_10();
		NullCheck(L_2);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_3 = Transform_get_parent_m8FA24E38A1FA29D90CBF3CDC9F9F017C65BB3403(L_2, /*hidden argument*/NULL);
		V_0 = L_3;
		// if (parent == null)
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_4 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_5 = Object_op_Equality_mBC2401774F3BE33E8CF6F0A8148E66C95D6CFF1C(L_4, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_5)
		{
			goto IL_0043;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("NetworkTransformChild target cannot be the root transform."); }
		IL2CPP_RUNTIME_CLASS_INIT(LogFilter_t5202A297E770086F7954B8D6703BAC03C22654ED_il2cpp_TypeInfo_var);
		bool L_6 = LogFilter_get_logError_mD404500EEB2968A3CF190DB1EB6CA9A26135A21F(/*hidden argument*/NULL);
		if (!L_6)
		{
			goto IL_0034;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("NetworkTransformChild target cannot be the root transform."); }
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_LogError_m3BCF9B78263152261565DCA9DB7D55F0C391ED29(_stringLiteral5A7F8C5B28DA7B5648FACC6874DF9F2B9D9823C9, /*hidden argument*/NULL);
	}

IL_0034:
	{
		// m_Target = null;
		__this->set_m_Target_10((Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA *)NULL);
		// return;
		return;
	}

IL_003c:
	{
		// parent = parent.parent;
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_7 = V_0;
		NullCheck(L_7);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_8 = Transform_get_parent_m8FA24E38A1FA29D90CBF3CDC9F9F017C65BB3403(L_7, /*hidden argument*/NULL);
		V_0 = L_8;
	}

IL_0043:
	{
		// while (parent.parent != null)
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_9 = V_0;
		NullCheck(L_9);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_10 = Transform_get_parent_m8FA24E38A1FA29D90CBF3CDC9F9F017C65BB3403(L_9, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_11 = Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1(L_10, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (L_11)
		{
			goto IL_003c;
		}
	}
	{
		// m_Root = parent.gameObject.GetComponent<NetworkTransform>();
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_12 = V_0;
		NullCheck(L_12);
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_13 = Component_get_gameObject_m0B0570BA8DDD3CD78A9DB568EA18D7317686603C(L_12, /*hidden argument*/NULL);
		NullCheck(L_13);
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_14 = GameObject_GetComponent_TisNetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F_mB885510CB2C4A1A57D2A42B4AE68A09AAA1DD79A(L_13, /*hidden argument*/GameObject_GetComponent_TisNetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F_mB885510CB2C4A1A57D2A42B4AE68A09AAA1DD79A_RuntimeMethod_var);
		__this->set_m_Root_12(L_14);
		// if (m_Root == null)
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_15 = __this->get_m_Root_12();
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_16 = Object_op_Equality_mBC2401774F3BE33E8CF6F0A8148E66C95D6CFF1C(L_15, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_16)
		{
			goto IL_0089;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("NetworkTransformChild root must have NetworkTransform"); }
		IL2CPP_RUNTIME_CLASS_INIT(LogFilter_t5202A297E770086F7954B8D6703BAC03C22654ED_il2cpp_TypeInfo_var);
		bool L_17 = LogFilter_get_logError_mD404500EEB2968A3CF190DB1EB6CA9A26135A21F(/*hidden argument*/NULL);
		if (!L_17)
		{
			goto IL_0081;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("NetworkTransformChild root must have NetworkTransform"); }
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_LogError_m3BCF9B78263152261565DCA9DB7D55F0C391ED29(_stringLiteralD57E846CA38774E0B2EFB225D0914EE255C71B37, /*hidden argument*/NULL);
	}

IL_0081:
	{
		// m_Target = null;
		__this->set_m_Target_10((Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA *)NULL);
		// return;
		return;
	}

IL_0089:
	{
		// if (m_Root != null)
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_18 = __this->get_m_Root_12();
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_19 = Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1(L_18, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_19)
		{
			goto IL_00ef;
		}
	}
	{
		// m_ChildIndex = UInt32.MaxValue;
		__this->set_m_ChildIndex_11((-1));
		// var childTransforms = m_Root.GetComponents<NetworkTransformChild>();
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_20 = __this->get_m_Root_12();
		NullCheck(L_20);
		NetworkTransformChildU5BU5D_tAE50AD052E14730045D7CF7E715FBC48ADEDB6C1* L_21 = Component_GetComponents_TisNetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E_mD3E94B5EC8B4D6678D6CE5FDFBA6502236E701C0(L_20, /*hidden argument*/Component_GetComponents_TisNetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E_mD3E94B5EC8B4D6678D6CE5FDFBA6502236E701C0_RuntimeMethod_var);
		V_1 = L_21;
		// for (uint i = 0; i < childTransforms.Length; i++)
		V_2 = 0;
		goto IL_00c6;
	}

IL_00ae:
	{
		// if (childTransforms[i] == this)
		NetworkTransformChildU5BU5D_tAE50AD052E14730045D7CF7E715FBC48ADEDB6C1* L_22 = V_1;
		uint32_t L_23 = V_2;
		NullCheck(L_22);
		uint32_t L_24 = L_23;
		NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * L_25 = (L_22)->GetAt(static_cast<il2cpp_array_size_t>(L_24));
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_26 = Object_op_Equality_mBC2401774F3BE33E8CF6F0A8148E66C95D6CFF1C(L_25, __this, /*hidden argument*/NULL);
		if (!L_26)
		{
			goto IL_00c2;
		}
	}
	{
		// m_ChildIndex = i;
		uint32_t L_27 = V_2;
		__this->set_m_ChildIndex_11(L_27);
		// break;
		goto IL_00ce;
	}

IL_00c2:
	{
		// for (uint i = 0; i < childTransforms.Length; i++)
		uint32_t L_28 = V_2;
		V_2 = ((int32_t)il2cpp_codegen_add((int32_t)L_28, (int32_t)1));
	}

IL_00c6:
	{
		// for (uint i = 0; i < childTransforms.Length; i++)
		uint32_t L_29 = V_2;
		NetworkTransformChildU5BU5D_tAE50AD052E14730045D7CF7E715FBC48ADEDB6C1* L_30 = V_1;
		NullCheck(L_30);
		if ((((int64_t)(((int64_t)((uint64_t)L_29)))) < ((int64_t)(((int64_t)((int64_t)(((int32_t)((int32_t)(((RuntimeArray*)L_30)->max_length))))))))))
		{
			goto IL_00ae;
		}
	}

IL_00ce:
	{
		// if (m_ChildIndex == UInt32.MaxValue)
		uint32_t L_31 = __this->get_m_ChildIndex_11();
		if ((!(((uint32_t)L_31) == ((uint32_t)(-1)))))
		{
			goto IL_00ef;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("NetworkTransformChild component must be a child in the same hierarchy"); }
		IL2CPP_RUNTIME_CLASS_INIT(LogFilter_t5202A297E770086F7954B8D6703BAC03C22654ED_il2cpp_TypeInfo_var);
		bool L_32 = LogFilter_get_logError_mD404500EEB2968A3CF190DB1EB6CA9A26135A21F(/*hidden argument*/NULL);
		if (!L_32)
		{
			goto IL_00e8;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("NetworkTransformChild component must be a child in the same hierarchy"); }
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_LogError_m3BCF9B78263152261565DCA9DB7D55F0C391ED29(_stringLiteralF344BFD3902B3288B6186257324DCFF0BBD317C1, /*hidden argument*/NULL);
	}

IL_00e8:
	{
		// m_Target = null;
		__this->set_m_Target_10((Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA *)NULL);
	}

IL_00ef:
	{
		// if (m_SendInterval < 0)
		float L_33 = __this->get_m_SendInterval_13();
		if ((!(((float)L_33) < ((float)(0.0f)))))
		{
			goto IL_0107;
		}
	}
	{
		// m_SendInterval = 0;
		__this->set_m_SendInterval_13((0.0f));
	}

IL_0107:
	{
		// if (m_SyncRotationAxis < NetworkTransform.AxisSyncMode.None || m_SyncRotationAxis > NetworkTransform.AxisSyncMode.AxisXYZ)
		int32_t L_34 = __this->get_m_SyncRotationAxis_14();
		if ((((int32_t)L_34) < ((int32_t)0)))
		{
			goto IL_0119;
		}
	}
	{
		int32_t L_35 = __this->get_m_SyncRotationAxis_14();
		if ((((int32_t)L_35) <= ((int32_t)7)))
		{
			goto IL_0120;
		}
	}

IL_0119:
	{
		// m_SyncRotationAxis = NetworkTransform.AxisSyncMode.None;
		__this->set_m_SyncRotationAxis_14(0);
	}

IL_0120:
	{
		// if (movementThreshold < 0)
		float L_36 = NetworkTransformChild_get_movementThreshold_m9BED81E541443BA95A2DDDF7465386F0E4F5639A_inline(__this, /*hidden argument*/NULL);
		if ((!(((float)L_36) < ((float)(0.0f)))))
		{
			goto IL_0138;
		}
	}
	{
		// movementThreshold = 0.00f;
		NetworkTransformChild_set_movementThreshold_m93D1B2916BC9B686B9F40C755A0AADCE6E54AB93_inline(__this, (0.0f), /*hidden argument*/NULL);
	}

IL_0138:
	{
		// if (interpolateRotation < 0)
		float L_37 = NetworkTransformChild_get_interpolateRotation_m9169822905990E0E3C3531F5812DC25FBE4C06BA_inline(__this, /*hidden argument*/NULL);
		if ((!(((float)L_37) < ((float)(0.0f)))))
		{
			goto IL_0150;
		}
	}
	{
		// interpolateRotation = 0.01f;
		NetworkTransformChild_set_interpolateRotation_mF72DC382026B2C763A63F0FB8D565275ECAEC4DF_inline(__this, (0.01f), /*hidden argument*/NULL);
	}

IL_0150:
	{
		// if (interpolateRotation > 1.0f)
		float L_38 = NetworkTransformChild_get_interpolateRotation_m9169822905990E0E3C3531F5812DC25FBE4C06BA_inline(__this, /*hidden argument*/NULL);
		if ((!(((float)L_38) > ((float)(1.0f)))))
		{
			goto IL_0168;
		}
	}
	{
		// interpolateRotation = 1.0f;
		NetworkTransformChild_set_interpolateRotation_mF72DC382026B2C763A63F0FB8D565275ECAEC4DF_inline(__this, (1.0f), /*hidden argument*/NULL);
	}

IL_0168:
	{
		// if (interpolateMovement < 0)
		float L_39 = NetworkTransformChild_get_interpolateMovement_m27491C1C3805AE420F584C937DC7DDFDB4A98E74_inline(__this, /*hidden argument*/NULL);
		if ((!(((float)L_39) < ((float)(0.0f)))))
		{
			goto IL_0180;
		}
	}
	{
		// interpolateMovement  = 0.01f;
		NetworkTransformChild_set_interpolateMovement_m94A0DB8134ACE98F427C12CFA1057CAA9370CF44_inline(__this, (0.01f), /*hidden argument*/NULL);
	}

IL_0180:
	{
		// if (interpolateMovement > 1.0f)
		float L_40 = NetworkTransformChild_get_interpolateMovement_m27491C1C3805AE420F584C937DC7DDFDB4A98E74_inline(__this, /*hidden argument*/NULL);
		if ((!(((float)L_40) > ((float)(1.0f)))))
		{
			goto IL_0198;
		}
	}
	{
		// interpolateMovement = 1.0f;
		NetworkTransformChild_set_interpolateMovement_m94A0DB8134ACE98F427C12CFA1057CAA9370CF44_inline(__this, (1.0f), /*hidden argument*/NULL);
	}

IL_0198:
	{
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformChild::Awake()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_Awake_m382F64641C54A3131E37EA1C6959C0A3B757A46B (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkTransformChild_Awake_m382F64641C54A3131E37EA1C6959C0A3B757A46B_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// m_PrevPosition = m_Target.localPosition;
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_0 = __this->get_m_Target_10();
		NullCheck(L_0);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_1 = Transform_get_localPosition_m812D43318E05BDCB78310EB7308785A13D85EFD8(L_0, /*hidden argument*/NULL);
		__this->set_m_PrevPosition_24(L_1);
		// m_PrevRotation = m_Target.localRotation;
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_2 = __this->get_m_Target_10();
		NullCheck(L_2);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_3 = Transform_get_localRotation_mEDA319E1B42EF12A19A95AC0824345B6574863FE(L_2, /*hidden argument*/NULL);
		__this->set_m_PrevRotation_25(L_3);
		// if (localPlayerAuthority)
		bool L_4 = NetworkBehaviour_get_localPlayerAuthority_m73DEE3D9A2E9916520CBDBA1B11888DAEA24B415(__this, /*hidden argument*/NULL);
		if (!L_4)
		{
			goto IL_0035;
		}
	}
	{
		// m_LocalTransformWriter = new NetworkWriter();
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_5 = (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 *)il2cpp_codegen_object_new(NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_il2cpp_TypeInfo_var);
		NetworkWriter__ctor_m43E453A4A5244815EC8D906B22E5D85FB7535D33(L_5, /*hidden argument*/NULL);
		__this->set_m_LocalTransformWriter_28(L_5);
	}

IL_0035:
	{
		// }
		return;
	}
}
// System.Boolean UnityEngine.Networking.NetworkTransformChild::OnSerialize(UnityEngine.Networking.NetworkWriter,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool NetworkTransformChild_OnSerialize_m871E33349FBE51FB6C5EEE975F59757EC4783412 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * ___writer0, bool ___initialState1, const RuntimeMethod* method)
{
	{
		// if (initialState)
		bool L_0 = ___initialState1;
		if (L_0)
		{
			goto IL_001b;
		}
	}
	{
		// else if (syncVarDirtyBits == 0)
		uint32_t L_1 = NetworkBehaviour_get_syncVarDirtyBits_mD53C3F852C533A88A2312E7AFF9883658DDEEB0C_inline(__this, /*hidden argument*/NULL);
		if (L_1)
		{
			goto IL_0014;
		}
	}
	{
		// writer.WritePackedUInt32(0);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_2 = ___writer0;
		NullCheck(L_2);
		NetworkWriter_WritePackedUInt32_m99DCA40833B068CB958663A5B583BC8D2051B12F(L_2, 0, /*hidden argument*/NULL);
		// return false;
		return (bool)0;
	}

IL_0014:
	{
		// writer.WritePackedUInt32(1);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_3 = ___writer0;
		NullCheck(L_3);
		NetworkWriter_WritePackedUInt32_m99DCA40833B068CB958663A5B583BC8D2051B12F(L_3, 1, /*hidden argument*/NULL);
	}

IL_001b:
	{
		// SerializeModeTransform(writer);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_4 = ___writer0;
		NetworkTransformChild_SerializeModeTransform_mCC0568E0CA6EC5192285EB962D141E4B056C6903(__this, L_4, /*hidden argument*/NULL);
		// return true;
		return (bool)1;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformChild::SerializeModeTransform(UnityEngine.Networking.NetworkWriter)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_SerializeModeTransform_mCC0568E0CA6EC5192285EB962D141E4B056C6903 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * ___writer0, const RuntimeMethod* method)
{
	{
		// writer.Write(m_Target.localPosition);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_0 = ___writer0;
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_1 = __this->get_m_Target_10();
		NullCheck(L_1);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_2 = Transform_get_localPosition_m812D43318E05BDCB78310EB7308785A13D85EFD8(L_1, /*hidden argument*/NULL);
		NullCheck(L_0);
		NetworkWriter_Write_m11CA4683BE86268158E1F949E620C1BF9D69884F(L_0, L_2, /*hidden argument*/NULL);
		// if (m_SyncRotationAxis != NetworkTransform.AxisSyncMode.None)
		int32_t L_3 = __this->get_m_SyncRotationAxis_14();
		if (!L_3)
		{
			goto IL_0036;
		}
	}
	{
		// NetworkTransform.SerializeRotation3D(writer, m_Target.localRotation, syncRotationAxis, rotationSyncCompression);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_4 = ___writer0;
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_5 = __this->get_m_Target_10();
		NullCheck(L_5);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_6 = Transform_get_localRotation_mEDA319E1B42EF12A19A95AC0824345B6574863FE(L_5, /*hidden argument*/NULL);
		int32_t L_7 = NetworkTransformChild_get_syncRotationAxis_m12199E8BCADC5098C84E6F2E6A8534424FD80979_inline(__this, /*hidden argument*/NULL);
		int32_t L_8 = NetworkTransformChild_get_rotationSyncCompression_m014CA812E4BB0DBF2DF856CB96E40FBED022239B_inline(__this, /*hidden argument*/NULL);
		NetworkTransform_SerializeRotation3D_m709105872FF5E4CA551590B97506834348060215(L_4, L_6, L_7, L_8, /*hidden argument*/NULL);
	}

IL_0036:
	{
		// m_PrevPosition = m_Target.localPosition;
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_9 = __this->get_m_Target_10();
		NullCheck(L_9);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_10 = Transform_get_localPosition_m812D43318E05BDCB78310EB7308785A13D85EFD8(L_9, /*hidden argument*/NULL);
		__this->set_m_PrevPosition_24(L_10);
		// m_PrevRotation = m_Target.localRotation;
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_11 = __this->get_m_Target_10();
		NullCheck(L_11);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_12 = Transform_get_localRotation_mEDA319E1B42EF12A19A95AC0824345B6574863FE(L_11, /*hidden argument*/NULL);
		__this->set_m_PrevRotation_25(L_12);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformChild::OnDeserialize(UnityEngine.Networking.NetworkReader,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_OnDeserialize_mE19F3EC01F32BFB2BB0D5DAF9FD97564F1935891 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * ___reader0, bool ___initialState1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkTransformChild_OnDeserialize_mE19F3EC01F32BFB2BB0D5DAF9FD97564F1935891_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (isServer && NetworkServer.localClientActive)
		bool L_0 = NetworkBehaviour_get_isServer_m3366F78A4D83ECE0798B276F2E9EF1FEEC8E2D79(__this, /*hidden argument*/NULL);
		if (!L_0)
		{
			goto IL_0010;
		}
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1_il2cpp_TypeInfo_var);
		bool L_1 = NetworkServer_get_localClientActive_mB6EDFFE4FCDAD0215974EE9F24E4E38D1257BF02(/*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_0010;
		}
	}
	{
		// return;
		return;
	}

IL_0010:
	{
		// if (!initialState)
		bool L_2 = ___initialState1;
		if (L_2)
		{
			goto IL_001c;
		}
	}
	{
		// if (reader.ReadPackedUInt32() == 0)
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_3 = ___reader0;
		NullCheck(L_3);
		uint32_t L_4 = NetworkReader_ReadPackedUInt32_mB0E5BF11AEAD652C88548BD93556D780A4E3F46B(L_3, /*hidden argument*/NULL);
		if (L_4)
		{
			goto IL_001c;
		}
	}
	{
		// return;
		return;
	}

IL_001c:
	{
		// UnserializeModeTransform(reader, initialState);
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_5 = ___reader0;
		bool L_6 = ___initialState1;
		NetworkTransformChild_UnserializeModeTransform_mBD3AB1CDF0F5F3D5FEA1AB49C0C53F7B24A62B39(__this, L_5, L_6, /*hidden argument*/NULL);
		// m_LastClientSyncTime = Time.time;
		float L_7 = Time_get_time_m7863349C8845BBA36629A2B3F8EF1C3BEA350FD8(/*hidden argument*/NULL);
		__this->set_m_LastClientSyncTime_22(L_7);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformChild::UnserializeModeTransform(UnityEngine.Networking.NetworkReader,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_UnserializeModeTransform_mBD3AB1CDF0F5F3D5FEA1AB49C0C53F7B24A62B39 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * ___reader0, bool ___initialState1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkTransformChild_UnserializeModeTransform_mBD3AB1CDF0F5F3D5FEA1AB49C0C53F7B24A62B39_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_0;
	memset((&V_0), 0, sizeof(V_0));
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_1;
	memset((&V_1), 0, sizeof(V_1));
	Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  V_2;
	memset((&V_2), 0, sizeof(V_2));
	{
		// if (hasAuthority)
		bool L_0 = NetworkBehaviour_get_hasAuthority_m20156D4B7D1F4097FFEAEFB2D0EAE8F95FF0B798(__this, /*hidden argument*/NULL);
		if (!L_0)
		{
			goto IL_002b;
		}
	}
	{
		// reader.ReadVector3();
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_1 = ___reader0;
		NullCheck(L_1);
		NetworkReader_ReadVector3_m8067F9687AEA7DD9FAC65E4550A441E8C7402314(L_1, /*hidden argument*/NULL);
		// if (syncRotationAxis != NetworkTransform.AxisSyncMode.None)
		int32_t L_2 = NetworkTransformChild_get_syncRotationAxis_m12199E8BCADC5098C84E6F2E6A8534424FD80979_inline(__this, /*hidden argument*/NULL);
		if (!L_2)
		{
			goto IL_002a;
		}
	}
	{
		// NetworkTransform.UnserializeRotation3D(reader, syncRotationAxis, rotationSyncCompression);
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_3 = ___reader0;
		int32_t L_4 = NetworkTransformChild_get_syncRotationAxis_m12199E8BCADC5098C84E6F2E6A8534424FD80979_inline(__this, /*hidden argument*/NULL);
		int32_t L_5 = NetworkTransformChild_get_rotationSyncCompression_m014CA812E4BB0DBF2DF856CB96E40FBED022239B_inline(__this, /*hidden argument*/NULL);
		NetworkTransform_UnserializeRotation3D_m6E8B8C1812E6FA2EBD24EFD6A8B75DC04439759E(L_3, L_4, L_5, /*hidden argument*/NULL);
	}

IL_002a:
	{
		// return;
		return;
	}

IL_002b:
	{
		// if (isServer && m_ClientMoveCallback3D != null)
		bool L_6 = NetworkBehaviour_get_isServer_m3366F78A4D83ECE0798B276F2E9EF1FEEC8E2D79(__this, /*hidden argument*/NULL);
		if (!L_6)
		{
			goto IL_0094;
		}
	}
	{
		ClientMoveCallback3D_t8B3ABB4B5CD7D938193C0EB61634DC9424916A45 * L_7 = __this->get_m_ClientMoveCallback3D_19();
		if (!L_7)
		{
			goto IL_0094;
		}
	}
	{
		// var pos = reader.ReadVector3();
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_8 = ___reader0;
		NullCheck(L_8);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_9 = NetworkReader_ReadVector3_m8067F9687AEA7DD9FAC65E4550A441E8C7402314(L_8, /*hidden argument*/NULL);
		V_0 = L_9;
		// var vel = Vector3.zero;
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_10 = Vector3_get_zero_m3CDDCAE94581DF3BB16C4B40A100E28E9C6649C2(/*hidden argument*/NULL);
		V_1 = L_10;
		// var rot = Quaternion.identity;
		IL2CPP_RUNTIME_CLASS_INIT(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_il2cpp_TypeInfo_var);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_11 = Quaternion_get_identity_m548B37D80F2DEE60E41D1F09BF6889B557BE1A64(/*hidden argument*/NULL);
		V_2 = L_11;
		// if (syncRotationAxis != NetworkTransform.AxisSyncMode.None)
		int32_t L_12 = NetworkTransformChild_get_syncRotationAxis_m12199E8BCADC5098C84E6F2E6A8534424FD80979_inline(__this, /*hidden argument*/NULL);
		if (!L_12)
		{
			goto IL_0069;
		}
	}
	{
		// rot = NetworkTransform.UnserializeRotation3D(reader, syncRotationAxis, rotationSyncCompression);
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_13 = ___reader0;
		int32_t L_14 = NetworkTransformChild_get_syncRotationAxis_m12199E8BCADC5098C84E6F2E6A8534424FD80979_inline(__this, /*hidden argument*/NULL);
		int32_t L_15 = NetworkTransformChild_get_rotationSyncCompression_m014CA812E4BB0DBF2DF856CB96E40FBED022239B_inline(__this, /*hidden argument*/NULL);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_16 = NetworkTransform_UnserializeRotation3D_m6E8B8C1812E6FA2EBD24EFD6A8B75DC04439759E(L_13, L_14, L_15, /*hidden argument*/NULL);
		V_2 = L_16;
	}

IL_0069:
	{
		// if (m_ClientMoveCallback3D(ref pos, ref vel, ref rot))
		ClientMoveCallback3D_t8B3ABB4B5CD7D938193C0EB61634DC9424916A45 * L_17 = __this->get_m_ClientMoveCallback3D_19();
		NullCheck(L_17);
		bool L_18 = ClientMoveCallback3D_Invoke_m4F4CED5C02FAFD6145BBA95A6B5261ACB9E0B19C(L_17, (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *)(&V_0), (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *)(&V_1), (Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357 *)(&V_2), /*hidden argument*/NULL);
		if (!L_18)
		{
			goto IL_0093;
		}
	}
	{
		// m_TargetSyncPosition = pos;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_19 = V_0;
		__this->set_m_TargetSyncPosition_20(L_19);
		// if (syncRotationAxis != NetworkTransform.AxisSyncMode.None)
		int32_t L_20 = NetworkTransformChild_get_syncRotationAxis_m12199E8BCADC5098C84E6F2E6A8534424FD80979_inline(__this, /*hidden argument*/NULL);
		if (!L_20)
		{
			goto IL_00c0;
		}
	}
	{
		// m_TargetSyncRotation3D = rot;
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_21 = V_2;
		__this->set_m_TargetSyncRotation3D_21(L_21);
		// }
		return;
	}

IL_0093:
	{
		// return;
		return;
	}

IL_0094:
	{
		// m_TargetSyncPosition = reader.ReadVector3();
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_22 = ___reader0;
		NullCheck(L_22);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_23 = NetworkReader_ReadVector3_m8067F9687AEA7DD9FAC65E4550A441E8C7402314(L_22, /*hidden argument*/NULL);
		__this->set_m_TargetSyncPosition_20(L_23);
		// if (syncRotationAxis != NetworkTransform.AxisSyncMode.None)
		int32_t L_24 = NetworkTransformChild_get_syncRotationAxis_m12199E8BCADC5098C84E6F2E6A8534424FD80979_inline(__this, /*hidden argument*/NULL);
		if (!L_24)
		{
			goto IL_00c0;
		}
	}
	{
		// m_TargetSyncRotation3D = NetworkTransform.UnserializeRotation3D(reader, syncRotationAxis, rotationSyncCompression);
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_25 = ___reader0;
		int32_t L_26 = NetworkTransformChild_get_syncRotationAxis_m12199E8BCADC5098C84E6F2E6A8534424FD80979_inline(__this, /*hidden argument*/NULL);
		int32_t L_27 = NetworkTransformChild_get_rotationSyncCompression_m014CA812E4BB0DBF2DF856CB96E40FBED022239B_inline(__this, /*hidden argument*/NULL);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_28 = NetworkTransform_UnserializeRotation3D_m6E8B8C1812E6FA2EBD24EFD6A8B75DC04439759E(L_25, L_26, L_27, /*hidden argument*/NULL);
		__this->set_m_TargetSyncRotation3D_21(L_28);
	}

IL_00c0:
	{
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformChild::FixedUpdate()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_FixedUpdate_m9DB17965DBBCB8798EB039628062A419796768AA (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	{
		// if (isServer)
		bool L_0 = NetworkBehaviour_get_isServer_m3366F78A4D83ECE0798B276F2E9EF1FEEC8E2D79(__this, /*hidden argument*/NULL);
		if (!L_0)
		{
			goto IL_000e;
		}
	}
	{
		// FixedUpdateServer();
		NetworkTransformChild_FixedUpdateServer_mC12D2E9DD8B1AE9D8222581DAE9F972F29102FE2(__this, /*hidden argument*/NULL);
	}

IL_000e:
	{
		// if (isClient)
		bool L_1 = NetworkBehaviour_get_isClient_mB7B109ADAF27B23B3D58E2369CBD11B1471C9148(__this, /*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_001c;
		}
	}
	{
		// FixedUpdateClient();
		NetworkTransformChild_FixedUpdateClient_mA050F9F38ED8D506D572DC21814309EDBF982B99(__this, /*hidden argument*/NULL);
	}

IL_001c:
	{
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformChild::FixedUpdateServer()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_FixedUpdateServer_mC12D2E9DD8B1AE9D8222581DAE9F972F29102FE2 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkTransformChild_FixedUpdateServer_mC12D2E9DD8B1AE9D8222581DAE9F972F29102FE2_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// if (syncVarDirtyBits != 0)
		uint32_t L_0 = NetworkBehaviour_get_syncVarDirtyBits_mD53C3F852C533A88A2312E7AFF9883658DDEEB0C_inline(__this, /*hidden argument*/NULL);
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
		// if (!NetworkServer.active)
		IL2CPP_RUNTIME_CLASS_INIT(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1_il2cpp_TypeInfo_var);
		bool L_1 = NetworkServer_get_active_m3FAC75ABF32D586F6C8DB6B4237DC40300FB2257_inline(/*hidden argument*/NULL);
		if (L_1)
		{
			goto IL_0011;
		}
	}
	{
		// return;
		return;
	}

IL_0011:
	{
		// if (!isServer)
		bool L_2 = NetworkBehaviour_get_isServer_m3366F78A4D83ECE0798B276F2E9EF1FEEC8E2D79(__this, /*hidden argument*/NULL);
		if (L_2)
		{
			goto IL_001a;
		}
	}
	{
		// return;
		return;
	}

IL_001a:
	{
		// if (GetNetworkSendInterval() == 0)
		float L_3 = VirtFuncInvoker0< float >::Invoke(21 /* System.Single UnityEngine.Networking.NetworkBehaviour::GetNetworkSendInterval() */, __this);
		if ((!(((float)L_3) == ((float)(0.0f)))))
		{
			goto IL_0028;
		}
	}
	{
		// return;
		return;
	}

IL_0028:
	{
		// float distance = (m_Target.localPosition - m_PrevPosition).sqrMagnitude;
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_4 = __this->get_m_Target_10();
		NullCheck(L_4);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_5 = Transform_get_localPosition_m812D43318E05BDCB78310EB7308785A13D85EFD8(L_4, /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_6 = __this->get_m_PrevPosition_24();
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_7 = Vector3_op_Subtraction_mF9846B723A5034F8B9F5F5DCB78E3D67649143D3(L_5, L_6, /*hidden argument*/NULL);
		V_0 = L_7;
		float L_8 = Vector3_get_sqrMagnitude_m1C6E190B4A933A183B308736DEC0DD64B0588968((Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *)(&V_0), /*hidden argument*/NULL);
		// if (distance < movementThreshold)
		float L_9 = NetworkTransformChild_get_movementThreshold_m9BED81E541443BA95A2DDDF7465386F0E4F5639A_inline(__this, /*hidden argument*/NULL);
		if ((!(((float)L_8) < ((float)L_9))))
		{
			goto IL_006d;
		}
	}
	{
		// distance = Quaternion.Angle(m_PrevRotation, m_Target.localRotation);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_10 = __this->get_m_PrevRotation_25();
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_11 = __this->get_m_Target_10();
		NullCheck(L_11);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_12 = Transform_get_localRotation_mEDA319E1B42EF12A19A95AC0824345B6574863FE(L_11, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_il2cpp_TypeInfo_var);
		float L_13 = Quaternion_Angle_m09599D660B724D330E5C7FE2FB1C8716161B3DD1(L_10, L_12, /*hidden argument*/NULL);
		// if (distance < movementThreshold)
		float L_14 = NetworkTransformChild_get_movementThreshold_m9BED81E541443BA95A2DDDF7465386F0E4F5639A_inline(__this, /*hidden argument*/NULL);
		if ((!(((float)L_13) < ((float)L_14))))
		{
			goto IL_006d;
		}
	}
	{
		// return;
		return;
	}

IL_006d:
	{
		// SetDirtyBit(1);
		NetworkBehaviour_SetDirtyBit_m474FBAD852378B9657C96EDD3E72BDDFD7E893DF(__this, 1, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformChild::FixedUpdateClient()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_FixedUpdateClient_mA050F9F38ED8D506D572DC21814309EDBF982B99 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkTransformChild_FixedUpdateClient_mA050F9F38ED8D506D572DC21814309EDBF982B99_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (m_LastClientSyncTime == 0)
		float L_0 = __this->get_m_LastClientSyncTime_22();
		if ((!(((float)L_0) == ((float)(0.0f)))))
		{
			goto IL_000e;
		}
	}
	{
		// return;
		return;
	}

IL_000e:
	{
		// if (!NetworkServer.active && !NetworkClient.active)
		IL2CPP_RUNTIME_CLASS_INIT(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1_il2cpp_TypeInfo_var);
		bool L_1 = NetworkServer_get_active_m3FAC75ABF32D586F6C8DB6B4237DC40300FB2257_inline(/*hidden argument*/NULL);
		if (L_1)
		{
			goto IL_001d;
		}
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F_il2cpp_TypeInfo_var);
		bool L_2 = NetworkClient_get_active_m31953DC487641BC5D9BEB0EB4DE32462AC4A8BD1_inline(/*hidden argument*/NULL);
		if (L_2)
		{
			goto IL_001d;
		}
	}
	{
		// return;
		return;
	}

IL_001d:
	{
		// if (!isServer && !isClient)
		bool L_3 = NetworkBehaviour_get_isServer_m3366F78A4D83ECE0798B276F2E9EF1FEEC8E2D79(__this, /*hidden argument*/NULL);
		if (L_3)
		{
			goto IL_002e;
		}
	}
	{
		bool L_4 = NetworkBehaviour_get_isClient_mB7B109ADAF27B23B3D58E2369CBD11B1471C9148(__this, /*hidden argument*/NULL);
		if (L_4)
		{
			goto IL_002e;
		}
	}
	{
		// return;
		return;
	}

IL_002e:
	{
		// if (GetNetworkSendInterval() == 0)
		float L_5 = VirtFuncInvoker0< float >::Invoke(21 /* System.Single UnityEngine.Networking.NetworkBehaviour::GetNetworkSendInterval() */, __this);
		if ((!(((float)L_5) == ((float)(0.0f)))))
		{
			goto IL_003c;
		}
	}
	{
		// return;
		return;
	}

IL_003c:
	{
		// if (hasAuthority)
		bool L_6 = NetworkBehaviour_get_hasAuthority_m20156D4B7D1F4097FFEAEFB2D0EAE8F95FF0B798(__this, /*hidden argument*/NULL);
		if (!L_6)
		{
			goto IL_0045;
		}
	}
	{
		// return;
		return;
	}

IL_0045:
	{
		// if (m_LastClientSyncTime != 0)
		float L_7 = __this->get_m_LastClientSyncTime_22();
		if ((((float)L_7) == ((float)(0.0f))))
		{
			goto IL_00e2;
		}
	}
	{
		// if (m_InterpolateMovement > 0)
		float L_8 = __this->get_m_InterpolateMovement_18();
		if ((!(((float)L_8) > ((float)(0.0f)))))
		{
			goto IL_008b;
		}
	}
	{
		// m_Target.localPosition = Vector3.Lerp(m_Target.localPosition, m_TargetSyncPosition, m_InterpolateMovement);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_9 = __this->get_m_Target_10();
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_10 = __this->get_m_Target_10();
		NullCheck(L_10);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_11 = Transform_get_localPosition_m812D43318E05BDCB78310EB7308785A13D85EFD8(L_10, /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_12 = __this->get_m_TargetSyncPosition_20();
		float L_13 = __this->get_m_InterpolateMovement_18();
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_14 = Vector3_Lerp_m5BA75496B803820CC64079383956D73C6FD4A8A1(L_11, L_12, L_13, /*hidden argument*/NULL);
		NullCheck(L_9);
		Transform_set_localPosition_m275F5550DD939F83AFEB5E8D681131172E2E1728(L_9, L_14, /*hidden argument*/NULL);
		// }
		goto IL_009c;
	}

IL_008b:
	{
		// m_Target.localPosition = m_TargetSyncPosition;
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_15 = __this->get_m_Target_10();
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_16 = __this->get_m_TargetSyncPosition_20();
		NullCheck(L_15);
		Transform_set_localPosition_m275F5550DD939F83AFEB5E8D681131172E2E1728(L_15, L_16, /*hidden argument*/NULL);
	}

IL_009c:
	{
		// if (m_InterpolateRotation > 0)
		float L_17 = __this->get_m_InterpolateRotation_17();
		if ((!(((float)L_17) > ((float)(0.0f)))))
		{
			goto IL_00d1;
		}
	}
	{
		// m_Target.localRotation = Quaternion.Slerp(m_Target.localRotation, m_TargetSyncRotation3D, m_InterpolateRotation);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_18 = __this->get_m_Target_10();
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_19 = __this->get_m_Target_10();
		NullCheck(L_19);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_20 = Transform_get_localRotation_mEDA319E1B42EF12A19A95AC0824345B6574863FE(L_19, /*hidden argument*/NULL);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_21 = __this->get_m_TargetSyncRotation3D_21();
		float L_22 = __this->get_m_InterpolateRotation_17();
		IL2CPP_RUNTIME_CLASS_INIT(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_il2cpp_TypeInfo_var);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_23 = Quaternion_Slerp_m56DE173C3520C83DF3F1C6EDFA82FF88A2C9E756(L_20, L_21, L_22, /*hidden argument*/NULL);
		NullCheck(L_18);
		Transform_set_localRotation_mE2BECB0954FFC1D93FB631600D9A9BEFF41D9C8A(L_18, L_23, /*hidden argument*/NULL);
		// }
		return;
	}

IL_00d1:
	{
		// m_Target.localRotation = m_TargetSyncRotation3D;
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_24 = __this->get_m_Target_10();
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_25 = __this->get_m_TargetSyncRotation3D_21();
		NullCheck(L_24);
		Transform_set_localRotation_mE2BECB0954FFC1D93FB631600D9A9BEFF41D9C8A(L_24, L_25, /*hidden argument*/NULL);
	}

IL_00e2:
	{
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformChild::Update()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_Update_m43F18B19328E7508C0AD3A754AFBB154617D8F6F (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkTransformChild_Update_m43F18B19328E7508C0AD3A754AFBB154617D8F6F_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (!hasAuthority)
		bool L_0 = NetworkBehaviour_get_hasAuthority_m20156D4B7D1F4097FFEAEFB2D0EAE8F95FF0B798(__this, /*hidden argument*/NULL);
		if (L_0)
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
		// if (!localPlayerAuthority)
		bool L_1 = NetworkBehaviour_get_localPlayerAuthority_m73DEE3D9A2E9916520CBDBA1B11888DAEA24B415(__this, /*hidden argument*/NULL);
		if (L_1)
		{
			goto IL_0012;
		}
	}
	{
		// return;
		return;
	}

IL_0012:
	{
		// if (NetworkServer.active)
		IL2CPP_RUNTIME_CLASS_INIT(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1_il2cpp_TypeInfo_var);
		bool L_2 = NetworkServer_get_active_m3FAC75ABF32D586F6C8DB6B4237DC40300FB2257_inline(/*hidden argument*/NULL);
		if (!L_2)
		{
			goto IL_001a;
		}
	}
	{
		// return;
		return;
	}

IL_001a:
	{
		// if (Time.time - m_LastClientSendTime > GetNetworkSendInterval())
		float L_3 = Time_get_time_m7863349C8845BBA36629A2B3F8EF1C3BEA350FD8(/*hidden argument*/NULL);
		float L_4 = __this->get_m_LastClientSendTime_23();
		float L_5 = VirtFuncInvoker0< float >::Invoke(21 /* System.Single UnityEngine.Networking.NetworkBehaviour::GetNetworkSendInterval() */, __this);
		if ((!(((float)((float)il2cpp_codegen_subtract((float)L_3, (float)L_4))) > ((float)L_5))))
		{
			goto IL_003f;
		}
	}
	{
		// SendTransform();
		NetworkTransformChild_SendTransform_m5E5A962A5EB1E18C5C6175873A55AE8E73C53AD9(__this, /*hidden argument*/NULL);
		// m_LastClientSendTime = Time.time;
		float L_6 = Time_get_time_m7863349C8845BBA36629A2B3F8EF1C3BEA350FD8(/*hidden argument*/NULL);
		__this->set_m_LastClientSendTime_23(L_6);
	}

IL_003f:
	{
		// }
		return;
	}
}
// System.Boolean UnityEngine.Networking.NetworkTransformChild::HasMoved()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool NetworkTransformChild_HasMoved_m6B10F1CD5A72301C5E26797F4105EAE2693971B4 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkTransformChild_HasMoved_m6B10F1CD5A72301C5E26797F4105EAE2693971B4_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// diff = (m_Target.localPosition - m_PrevPosition).sqrMagnitude;
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_0 = __this->get_m_Target_10();
		NullCheck(L_0);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_1 = Transform_get_localPosition_m812D43318E05BDCB78310EB7308785A13D85EFD8(L_0, /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_2 = __this->get_m_PrevPosition_24();
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_3 = Vector3_op_Subtraction_mF9846B723A5034F8B9F5F5DCB78E3D67649143D3(L_1, L_2, /*hidden argument*/NULL);
		V_0 = L_3;
		float L_4 = Vector3_get_sqrMagnitude_m1C6E190B4A933A183B308736DEC0DD64B0588968((Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 *)(&V_0), /*hidden argument*/NULL);
		// if (diff > k_LocalMovementThreshold)
		if ((!(((float)L_4) > ((float)(1.0E-05f)))))
		{
			goto IL_0027;
		}
	}
	{
		// return true;
		return (bool)1;
	}

IL_0027:
	{
		// diff = Quaternion.Angle(m_Target.localRotation, m_PrevRotation);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_5 = __this->get_m_Target_10();
		NullCheck(L_5);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_6 = Transform_get_localRotation_mEDA319E1B42EF12A19A95AC0824345B6574863FE(L_5, /*hidden argument*/NULL);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_7 = __this->get_m_PrevRotation_25();
		IL2CPP_RUNTIME_CLASS_INIT(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_il2cpp_TypeInfo_var);
		float L_8 = Quaternion_Angle_m09599D660B724D330E5C7FE2FB1C8716161B3DD1(L_6, L_7, /*hidden argument*/NULL);
		// if (diff > k_LocalRotationThreshold)
		if ((!(((float)L_8) > ((float)(1.0E-05f)))))
		{
			goto IL_0046;
		}
	}
	{
		// return true;
		return (bool)1;
	}

IL_0046:
	{
		// return false;
		return (bool)0;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformChild::SendTransform()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_SendTransform_m5E5A962A5EB1E18C5C6175873A55AE8E73C53AD9 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkTransformChild_SendTransform_m5E5A962A5EB1E18C5C6175873A55AE8E73C53AD9_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (!HasMoved() || ClientScene.readyConnection == null)
		bool L_0 = NetworkTransformChild_HasMoved_m6B10F1CD5A72301C5E26797F4105EAE2693971B4(__this, /*hidden argument*/NULL);
		if (!L_0)
		{
			goto IL_000f;
		}
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(ClientScene_t0A10B1F436A5AA8D5FC9B18C9ED0B32008809A3E_il2cpp_TypeInfo_var);
		NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * L_1 = ClientScene_get_readyConnection_mACB67AD0151B2507CF8BD5D7D8B806C470E49998_inline(/*hidden argument*/NULL);
		if (L_1)
		{
			goto IL_0010;
		}
	}

IL_000f:
	{
		// return;
		return;
	}

IL_0010:
	{
		// m_LocalTransformWriter.StartMessage(MsgType.LocalChildTransform);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_2 = __this->get_m_LocalTransformWriter_28();
		NullCheck(L_2);
		NetworkWriter_StartMessage_mD4F5BFA7ECA40EEA4AC721A1E357C3C8A09CE218(L_2, (int16_t)((int32_t)16), /*hidden argument*/NULL);
		// m_LocalTransformWriter.Write(netId);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_3 = __this->get_m_LocalTransformWriter_28();
		NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  L_4 = NetworkBehaviour_get_netId_m33EAF782A985004BBEEB6AE5CD30A2C8F4E35564(__this, /*hidden argument*/NULL);
		NullCheck(L_3);
		NetworkWriter_Write_m327AAC971B7DA22E82661AD419E4D5EEC6CCAFBF(L_3, L_4, /*hidden argument*/NULL);
		// m_LocalTransformWriter.WritePackedUInt32(m_ChildIndex);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_5 = __this->get_m_LocalTransformWriter_28();
		uint32_t L_6 = __this->get_m_ChildIndex_11();
		NullCheck(L_5);
		NetworkWriter_WritePackedUInt32_m99DCA40833B068CB958663A5B583BC8D2051B12F(L_5, L_6, /*hidden argument*/NULL);
		// SerializeModeTransform(m_LocalTransformWriter);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_7 = __this->get_m_LocalTransformWriter_28();
		NetworkTransformChild_SerializeModeTransform_mCC0568E0CA6EC5192285EB962D141E4B056C6903(__this, L_7, /*hidden argument*/NULL);
		// m_PrevPosition = m_Target.localPosition;
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_8 = __this->get_m_Target_10();
		NullCheck(L_8);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_9 = Transform_get_localPosition_m812D43318E05BDCB78310EB7308785A13D85EFD8(L_8, /*hidden argument*/NULL);
		__this->set_m_PrevPosition_24(L_9);
		// m_PrevRotation = m_Target.localRotation;
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_10 = __this->get_m_Target_10();
		NullCheck(L_10);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_11 = Transform_get_localRotation_mEDA319E1B42EF12A19A95AC0824345B6574863FE(L_10, /*hidden argument*/NULL);
		__this->set_m_PrevRotation_25(L_11);
		// m_LocalTransformWriter.FinishMessage();
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_12 = __this->get_m_LocalTransformWriter_28();
		NullCheck(L_12);
		NetworkWriter_FinishMessage_mDA9E66815E448F635B2394A35DDCA3EC040B0590(L_12, /*hidden argument*/NULL);
		// ClientScene.readyConnection.SendWriter(m_LocalTransformWriter, GetNetworkChannel());
		IL2CPP_RUNTIME_CLASS_INIT(ClientScene_t0A10B1F436A5AA8D5FC9B18C9ED0B32008809A3E_il2cpp_TypeInfo_var);
		NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * L_13 = ClientScene_get_readyConnection_mACB67AD0151B2507CF8BD5D7D8B806C470E49998_inline(/*hidden argument*/NULL);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_14 = __this->get_m_LocalTransformWriter_28();
		int32_t L_15 = VirtFuncInvoker0< int32_t >::Invoke(20 /* System.Int32 UnityEngine.Networking.NetworkBehaviour::GetNetworkChannel() */, __this);
		NullCheck(L_13);
		VirtFuncInvoker2< bool, NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 *, int32_t >::Invoke(11 /* System.Boolean UnityEngine.Networking.NetworkConnection::SendWriter(UnityEngine.Networking.NetworkWriter,System.Int32) */, L_13, L_14, L_15);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformChild::HandleChildTransform(UnityEngine.Networking.NetworkMessage)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild_HandleChildTransform_m786648BD699A0A3BF0CCE37E18A16A8FB5673269 (NetworkMessage_tCD66E2AE395A185EFE622EBB5497C95F6754685C * ___netMsg0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkTransformChild_HandleChildTransform_m786648BD699A0A3BF0CCE37E18A16A8FB5673269_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  V_0;
	memset((&V_0), 0, sizeof(V_0));
	uint32_t V_1 = 0;
	GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * V_2 = NULL;
	NetworkTransformChildU5BU5D_tAE50AD052E14730045D7CF7E715FBC48ADEDB6C1* V_3 = NULL;
	NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * V_4 = NULL;
	{
		// NetworkInstanceId netId = netMsg.reader.ReadNetworkId();
		NetworkMessage_tCD66E2AE395A185EFE622EBB5497C95F6754685C * L_0 = ___netMsg0;
		NullCheck(L_0);
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_1 = L_0->get_reader_3();
		NullCheck(L_1);
		NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  L_2 = NetworkReader_ReadNetworkId_m68B53D6FD5C9BF5DCEA1E114630D5256A331E7FE(L_1, /*hidden argument*/NULL);
		V_0 = L_2;
		// uint childIndex = netMsg.reader.ReadPackedUInt32();
		NetworkMessage_tCD66E2AE395A185EFE622EBB5497C95F6754685C * L_3 = ___netMsg0;
		NullCheck(L_3);
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_4 = L_3->get_reader_3();
		NullCheck(L_4);
		uint32_t L_5 = NetworkReader_ReadPackedUInt32_mB0E5BF11AEAD652C88548BD93556D780A4E3F46B(L_4, /*hidden argument*/NULL);
		V_1 = L_5;
		// GameObject foundObj = NetworkServer.FindLocalObject(netId);
		NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  L_6 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1_il2cpp_TypeInfo_var);
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_7 = NetworkServer_FindLocalObject_m0EA227D12590A2EE92F6B029C888AE46C560FB77(L_6, /*hidden argument*/NULL);
		V_2 = L_7;
		// if (foundObj == null)
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_8 = V_2;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_9 = Object_op_Equality_mBC2401774F3BE33E8CF6F0A8148E66C95D6CFF1C(L_8, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_9)
		{
			goto IL_003a;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("Received NetworkTransformChild data for GameObject that doesn't exist"); }
		IL2CPP_RUNTIME_CLASS_INIT(LogFilter_t5202A297E770086F7954B8D6703BAC03C22654ED_il2cpp_TypeInfo_var);
		bool L_10 = LogFilter_get_logError_mD404500EEB2968A3CF190DB1EB6CA9A26135A21F(/*hidden argument*/NULL);
		if (!L_10)
		{
			goto IL_0039;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("Received NetworkTransformChild data for GameObject that doesn't exist"); }
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_LogError_m3BCF9B78263152261565DCA9DB7D55F0C391ED29(_stringLiteral43AB5B0D093A407E1568E3E17AAF14DC10D1DE88, /*hidden argument*/NULL);
	}

IL_0039:
	{
		// return;
		return;
	}

IL_003a:
	{
		// var children = foundObj.GetComponents<NetworkTransformChild>();
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_11 = V_2;
		NullCheck(L_11);
		NetworkTransformChildU5BU5D_tAE50AD052E14730045D7CF7E715FBC48ADEDB6C1* L_12 = GameObject_GetComponents_TisNetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E_mAE4B663ABA411E0016C4E260112D19FB99B8889F(L_11, /*hidden argument*/GameObject_GetComponents_TisNetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E_mAE4B663ABA411E0016C4E260112D19FB99B8889F_RuntimeMethod_var);
		V_3 = L_12;
		// if (children == null || children.Length == 0)
		NetworkTransformChildU5BU5D_tAE50AD052E14730045D7CF7E715FBC48ADEDB6C1* L_13 = V_3;
		if (!L_13)
		{
			goto IL_0048;
		}
	}
	{
		NetworkTransformChildU5BU5D_tAE50AD052E14730045D7CF7E715FBC48ADEDB6C1* L_14 = V_3;
		NullCheck(L_14);
		if ((((RuntimeArray*)L_14)->max_length))
		{
			goto IL_005a;
		}
	}

IL_0048:
	{
		// if (LogFilter.logError) { Debug.LogError("HandleChildTransform no children"); }
		IL2CPP_RUNTIME_CLASS_INIT(LogFilter_t5202A297E770086F7954B8D6703BAC03C22654ED_il2cpp_TypeInfo_var);
		bool L_15 = LogFilter_get_logError_mD404500EEB2968A3CF190DB1EB6CA9A26135A21F(/*hidden argument*/NULL);
		if (!L_15)
		{
			goto IL_0059;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("HandleChildTransform no children"); }
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_LogError_m3BCF9B78263152261565DCA9DB7D55F0C391ED29(_stringLiteralCDC3766C1C256EF634E93723D6A50C0DDD213BC4, /*hidden argument*/NULL);
	}

IL_0059:
	{
		// return;
		return;
	}

IL_005a:
	{
		// if (childIndex >= children.Length)
		uint32_t L_16 = V_1;
		NetworkTransformChildU5BU5D_tAE50AD052E14730045D7CF7E715FBC48ADEDB6C1* L_17 = V_3;
		NullCheck(L_17);
		if ((((int64_t)(((int64_t)((uint64_t)L_16)))) < ((int64_t)(((int64_t)((int64_t)(((int32_t)((int32_t)(((RuntimeArray*)L_17)->max_length))))))))))
		{
			goto IL_0074;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("HandleChildTransform childIndex invalid"); }
		IL2CPP_RUNTIME_CLASS_INIT(LogFilter_t5202A297E770086F7954B8D6703BAC03C22654ED_il2cpp_TypeInfo_var);
		bool L_18 = LogFilter_get_logError_mD404500EEB2968A3CF190DB1EB6CA9A26135A21F(/*hidden argument*/NULL);
		if (!L_18)
		{
			goto IL_0073;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("HandleChildTransform childIndex invalid"); }
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_LogError_m3BCF9B78263152261565DCA9DB7D55F0C391ED29(_stringLiteral8FBB472DE655009001FA670F6146ACB177ACBCDC, /*hidden argument*/NULL);
	}

IL_0073:
	{
		// return;
		return;
	}

IL_0074:
	{
		// NetworkTransformChild foundSync = children[childIndex];
		NetworkTransformChildU5BU5D_tAE50AD052E14730045D7CF7E715FBC48ADEDB6C1* L_19 = V_3;
		uint32_t L_20 = V_1;
		NullCheck(L_19);
		uint32_t L_21 = L_20;
		NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * L_22 = (L_19)->GetAt(static_cast<il2cpp_array_size_t>(L_21));
		V_4 = L_22;
		// if (foundSync == null)
		NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * L_23 = V_4;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_24 = Object_op_Equality_mBC2401774F3BE33E8CF6F0A8148E66C95D6CFF1C(L_23, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_24)
		{
			goto IL_0095;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("HandleChildTransform null target"); }
		IL2CPP_RUNTIME_CLASS_INIT(LogFilter_t5202A297E770086F7954B8D6703BAC03C22654ED_il2cpp_TypeInfo_var);
		bool L_25 = LogFilter_get_logError_mD404500EEB2968A3CF190DB1EB6CA9A26135A21F(/*hidden argument*/NULL);
		if (!L_25)
		{
			goto IL_0094;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("HandleChildTransform null target"); }
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_LogError_m3BCF9B78263152261565DCA9DB7D55F0C391ED29(_stringLiteral2DECCA3D4BB2505D46E36DBC5737FDD9004B2564, /*hidden argument*/NULL);
	}

IL_0094:
	{
		// return;
		return;
	}

IL_0095:
	{
		// if (!foundSync.localPlayerAuthority)
		NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * L_26 = V_4;
		NullCheck(L_26);
		bool L_27 = NetworkBehaviour_get_localPlayerAuthority_m73DEE3D9A2E9916520CBDBA1B11888DAEA24B415(L_26, /*hidden argument*/NULL);
		if (L_27)
		{
			goto IL_00b0;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("HandleChildTransform no localPlayerAuthority"); }
		IL2CPP_RUNTIME_CLASS_INIT(LogFilter_t5202A297E770086F7954B8D6703BAC03C22654ED_il2cpp_TypeInfo_var);
		bool L_28 = LogFilter_get_logError_mD404500EEB2968A3CF190DB1EB6CA9A26135A21F(/*hidden argument*/NULL);
		if (!L_28)
		{
			goto IL_00af;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("HandleChildTransform no localPlayerAuthority"); }
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_LogError_m3BCF9B78263152261565DCA9DB7D55F0C391ED29(_stringLiteral994F759C33CC2E33C8D1EA34D1B4D05FF92E9F57, /*hidden argument*/NULL);
	}

IL_00af:
	{
		// return;
		return;
	}

IL_00b0:
	{
		// if (!netMsg.conn.clientOwnedObjects.Contains(netId))
		NetworkMessage_tCD66E2AE395A185EFE622EBB5497C95F6754685C * L_29 = ___netMsg0;
		NullCheck(L_29);
		NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * L_30 = L_29->get_conn_2();
		NullCheck(L_30);
		HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990 * L_31 = NetworkConnection_get_clientOwnedObjects_m0CC0D90CD318855211AA194D67DB4A07E4694D22_inline(L_30, /*hidden argument*/NULL);
		NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  L_32 = V_0;
		NullCheck(L_31);
		bool L_33 = HashSet_1_Contains_m68D1EC086CFCC7E6FBE6B1C66DDFF3D1DC62695C(L_31, L_32, /*hidden argument*/HashSet_1_Contains_m68D1EC086CFCC7E6FBE6B1C66DDFF3D1DC62695C_RuntimeMethod_var);
		if (L_33)
		{
			goto IL_00e5;
		}
	}
	{
		// if (LogFilter.logWarn) { Debug.LogWarning("NetworkTransformChild netId:" + netId + " is not for a valid player"); }
		IL2CPP_RUNTIME_CLASS_INIT(LogFilter_t5202A297E770086F7954B8D6703BAC03C22654ED_il2cpp_TypeInfo_var);
		bool L_34 = LogFilter_get_logWarn_m68D69BE30614BF75FF942A304F2C453298667AFD(/*hidden argument*/NULL);
		if (!L_34)
		{
			goto IL_00e4;
		}
	}
	{
		// if (LogFilter.logWarn) { Debug.LogWarning("NetworkTransformChild netId:" + netId + " is not for a valid player"); }
		NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  L_35 = V_0;
		NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  L_36 = L_35;
		RuntimeObject * L_37 = Box(NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615_il2cpp_TypeInfo_var, &L_36);
		String_t* L_38 = String_Concat_m2E1F71C491D2429CC80A28745488FEA947BB7AAC(_stringLiteralBF23D347C24FDA3FCBB189660BEF497CF90D2A71, L_37, _stringLiteral23DD7FD333862ABC4A00FCC16019AD77994EB92C, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_LogWarning_m37338644DC81F640CCDFEAE35A223F0E965F0568(L_38, /*hidden argument*/NULL);
	}

IL_00e4:
	{
		// return;
		return;
	}

IL_00e5:
	{
		// foundSync.UnserializeModeTransform(netMsg.reader, false);
		NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * L_39 = V_4;
		NetworkMessage_tCD66E2AE395A185EFE622EBB5497C95F6754685C * L_40 = ___netMsg0;
		NullCheck(L_40);
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_41 = L_40->get_reader_3();
		NullCheck(L_39);
		NetworkTransformChild_UnserializeModeTransform_mBD3AB1CDF0F5F3D5FEA1AB49C0C53F7B24A62B39(L_39, L_41, (bool)0, /*hidden argument*/NULL);
		// foundSync.m_LastClientSyncTime = Time.time;
		NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * L_42 = V_4;
		float L_43 = Time_get_time_m7863349C8845BBA36629A2B3F8EF1C3BEA350FD8(/*hidden argument*/NULL);
		NullCheck(L_42);
		L_42->set_m_LastClientSyncTime_22(L_43);
		// if (!foundSync.isClient)
		NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * L_44 = V_4;
		NullCheck(L_44);
		bool L_45 = NetworkBehaviour_get_isClient_mB7B109ADAF27B23B3D58E2369CBD11B1471C9148(L_44, /*hidden argument*/NULL);
		if (L_45)
		{
			goto IL_012e;
		}
	}
	{
		// foundSync.m_Target.localPosition = foundSync.m_TargetSyncPosition;
		NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * L_46 = V_4;
		NullCheck(L_46);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_47 = L_46->get_m_Target_10();
		NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * L_48 = V_4;
		NullCheck(L_48);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_49 = L_48->get_m_TargetSyncPosition_20();
		NullCheck(L_47);
		Transform_set_localPosition_m275F5550DD939F83AFEB5E8D681131172E2E1728(L_47, L_49, /*hidden argument*/NULL);
		// foundSync.m_Target.localRotation = foundSync.m_TargetSyncRotation3D;
		NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * L_50 = V_4;
		NullCheck(L_50);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_51 = L_50->get_m_Target_10();
		NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * L_52 = V_4;
		NullCheck(L_52);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_53 = L_52->get_m_TargetSyncRotation3D_21();
		NullCheck(L_51);
		Transform_set_localRotation_mE2BECB0954FFC1D93FB631600D9A9BEFF41D9C8A(L_51, L_53, /*hidden argument*/NULL);
	}

IL_012e:
	{
		// }
		return;
	}
}
// System.Int32 UnityEngine.Networking.NetworkTransformChild::GetNetworkChannel()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t NetworkTransformChild_GetNetworkChannel_mE45193214CB75809C57199012693AC8547391C12 (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	{
		// return Channels.DefaultUnreliable;
		return 1;
	}
}
// System.Single UnityEngine.Networking.NetworkTransformChild::GetNetworkSendInterval()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float NetworkTransformChild_GetNetworkSendInterval_m8914285F35BFAC44F0F17C3815E501AFA3A00BDE (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	{
		// return m_SendInterval;
		float L_0 = __this->get_m_SendInterval_13();
		return L_0;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformChild::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformChild__ctor_m45FC783409BB7C8ACF58B05DCC2C6EB5C46B966F (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkTransformChild__ctor_m45FC783409BB7C8ACF58B05DCC2C6EB5C46B966F_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// [SerializeField] float                                  m_SendInterval = 0.1f;
		__this->set_m_SendInterval_13((0.1f));
		// [SerializeField] NetworkTransform.AxisSyncMode          m_SyncRotationAxis = NetworkTransform.AxisSyncMode.AxisXYZ;
		__this->set_m_SyncRotationAxis_14(7);
		// [SerializeField] float                                  m_MovementThreshold = 0.001f;
		__this->set_m_MovementThreshold_16((0.001f));
		// [SerializeField] float                                  m_InterpolateRotation = 0.5f;
		__this->set_m_InterpolateRotation_17((0.5f));
		// [SerializeField] float                                  m_InterpolateMovement = 0.5f;
		__this->set_m_InterpolateMovement_18((0.5f));
		IL2CPP_RUNTIME_CLASS_INIT(NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C_il2cpp_TypeInfo_var);
		NetworkBehaviour__ctor_m37D8F4B6AD273AFBE5507BB02D956282684A0B78(__this, /*hidden argument*/NULL);
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
// UnityEngine.GameObject UnityEngine.Networking.NetworkTransformVisualizer::get_visualizerPrefab()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * NetworkTransformVisualizer_get_visualizerPrefab_mD1DA91FEA40555A32D4099C73B5EB7E788B063DB (NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386 * __this, const RuntimeMethod* method)
{
	{
		// public GameObject visualizerPrefab { get { return m_VisualizerPrefab; } set { m_VisualizerPrefab = value; }}
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_0 = __this->get_m_VisualizerPrefab_10();
		return L_0;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformVisualizer::set_visualizerPrefab(UnityEngine.GameObject)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformVisualizer_set_visualizerPrefab_m0E8C9061432DBAE28070178D86D396B1B4349D01 (NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386 * __this, GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * ___value0, const RuntimeMethod* method)
{
	{
		// public GameObject visualizerPrefab { get { return m_VisualizerPrefab; } set { m_VisualizerPrefab = value; }}
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_0 = ___value0;
		__this->set_m_VisualizerPrefab_10(L_0);
		// public GameObject visualizerPrefab { get { return m_VisualizerPrefab; } set { m_VisualizerPrefab = value; }}
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformVisualizer::OnStartClient()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformVisualizer_OnStartClient_m6C93CA459FFB3A1E142B8EDDE4F50E3B1184F66F (NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkTransformVisualizer_OnStartClient_m6C93CA459FFB3A1E142B8EDDE4F50E3B1184F66F_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (m_VisualizerPrefab != null)
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_0 = __this->get_m_VisualizerPrefab_10();
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_1 = Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1(L_0, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_0040;
		}
	}
	{
		// m_NetworkTransform = GetComponent<NetworkTransform>();
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_2 = Component_GetComponent_TisNetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F_mC1DB4A13BBC41101231C90CD393292630350975B(__this, /*hidden argument*/Component_GetComponent_TisNetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F_mC1DB4A13BBC41101231C90CD393292630350975B_RuntimeMethod_var);
		__this->set_m_NetworkTransform_11(L_2);
		// CreateLineMaterial();
		NetworkTransformVisualizer_CreateLineMaterial_m7939398CB6B61BFD7D237E3685D50000C7A41B89(/*hidden argument*/NULL);
		// m_Visualizer = (GameObject)Instantiate(m_VisualizerPrefab, transform.position, Quaternion.identity);
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_3 = __this->get_m_VisualizerPrefab_10();
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_4 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(__this, /*hidden argument*/NULL);
		NullCheck(L_4);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_5 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_4, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_il2cpp_TypeInfo_var);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_6 = Quaternion_get_identity_m548B37D80F2DEE60E41D1F09BF6889B557BE1A64(/*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_7 = Object_Instantiate_TisGameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F_m4F397BCC6697902B40033E61129D4EA6FE93570F(L_3, L_5, L_6, /*hidden argument*/Object_Instantiate_TisGameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F_m4F397BCC6697902B40033E61129D4EA6FE93570F_RuntimeMethod_var);
		__this->set_m_Visualizer_12(L_7);
	}

IL_0040:
	{
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformVisualizer::OnStartLocalPlayer()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformVisualizer_OnStartLocalPlayer_m0F01BE0FFDFC5EE5BE5237CF0DD1DC13689786AA (NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkTransformVisualizer_OnStartLocalPlayer_m0F01BE0FFDFC5EE5BE5237CF0DD1DC13689786AA_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (m_Visualizer == null)
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_0 = __this->get_m_Visualizer_12();
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_1 = Object_op_Equality_mBC2401774F3BE33E8CF6F0A8148E66C95D6CFF1C(L_0, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_000f;
		}
	}
	{
		// return;
		return;
	}

IL_000f:
	{
		// if (m_NetworkTransform.localPlayerAuthority || isServer)
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_2 = __this->get_m_NetworkTransform_11();
		NullCheck(L_2);
		bool L_3 = NetworkBehaviour_get_localPlayerAuthority_m73DEE3D9A2E9916520CBDBA1B11888DAEA24B415(L_2, /*hidden argument*/NULL);
		if (L_3)
		{
			goto IL_0024;
		}
	}
	{
		bool L_4 = NetworkBehaviour_get_isServer_m3366F78A4D83ECE0798B276F2E9EF1FEEC8E2D79(__this, /*hidden argument*/NULL);
		if (!L_4)
		{
			goto IL_002f;
		}
	}

IL_0024:
	{
		// Destroy(m_Visualizer);
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_5 = __this->get_m_Visualizer_12();
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		Object_Destroy_m23B4562495BA35A74266D4372D45368F8C05109A(L_5, /*hidden argument*/NULL);
	}

IL_002f:
	{
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformVisualizer::OnDestroy()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformVisualizer_OnDestroy_m928CDE56238148D2AB9AEE53A1B536E8B4C475E8 (NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkTransformVisualizer_OnDestroy_m928CDE56238148D2AB9AEE53A1B536E8B4C475E8_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (m_Visualizer != null)
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_0 = __this->get_m_Visualizer_12();
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_1 = Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1(L_0, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_0019;
		}
	}
	{
		// Destroy(m_Visualizer);
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_2 = __this->get_m_Visualizer_12();
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		Object_Destroy_m23B4562495BA35A74266D4372D45368F8C05109A(L_2, /*hidden argument*/NULL);
	}

IL_0019:
	{
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformVisualizer::FixedUpdate()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformVisualizer_FixedUpdate_m4E2CC9B289A2C7C9CBF8E6DDBACD9C717454A8ED (NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkTransformVisualizer_FixedUpdate_m4E2CC9B289A2C7C9CBF8E6DDBACD9C717454A8ED_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// if (m_Visualizer == null)
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_0 = __this->get_m_Visualizer_12();
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_1 = Object_op_Equality_mBC2401774F3BE33E8CF6F0A8148E66C95D6CFF1C(L_0, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_000f;
		}
	}
	{
		// return;
		return;
	}

IL_000f:
	{
		// if (!NetworkServer.active && !NetworkClient.active)
		IL2CPP_RUNTIME_CLASS_INIT(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1_il2cpp_TypeInfo_var);
		bool L_2 = NetworkServer_get_active_m3FAC75ABF32D586F6C8DB6B4237DC40300FB2257_inline(/*hidden argument*/NULL);
		if (L_2)
		{
			goto IL_001e;
		}
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F_il2cpp_TypeInfo_var);
		bool L_3 = NetworkClient_get_active_m31953DC487641BC5D9BEB0EB4DE32462AC4A8BD1_inline(/*hidden argument*/NULL);
		if (L_3)
		{
			goto IL_001e;
		}
	}
	{
		// return;
		return;
	}

IL_001e:
	{
		// if (!isServer && !isClient)
		bool L_4 = NetworkBehaviour_get_isServer_m3366F78A4D83ECE0798B276F2E9EF1FEEC8E2D79(__this, /*hidden argument*/NULL);
		if (L_4)
		{
			goto IL_002f;
		}
	}
	{
		bool L_5 = NetworkBehaviour_get_isClient_mB7B109ADAF27B23B3D58E2369CBD11B1471C9148(__this, /*hidden argument*/NULL);
		if (L_5)
		{
			goto IL_002f;
		}
	}
	{
		// return;
		return;
	}

IL_002f:
	{
		// if (hasAuthority && m_NetworkTransform.localPlayerAuthority)
		bool L_6 = NetworkBehaviour_get_hasAuthority_m20156D4B7D1F4097FFEAEFB2D0EAE8F95FF0B798(__this, /*hidden argument*/NULL);
		if (!L_6)
		{
			goto IL_0045;
		}
	}
	{
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_7 = __this->get_m_NetworkTransform_11();
		NullCheck(L_7);
		bool L_8 = NetworkBehaviour_get_localPlayerAuthority_m73DEE3D9A2E9916520CBDBA1B11888DAEA24B415(L_7, /*hidden argument*/NULL);
		if (!L_8)
		{
			goto IL_0045;
		}
	}
	{
		// return;
		return;
	}

IL_0045:
	{
		// m_Visualizer.transform.position = m_NetworkTransform.targetSyncPosition;
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_9 = __this->get_m_Visualizer_12();
		NullCheck(L_9);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_10 = GameObject_get_transform_mA5C38857137F137CB96C69FAA624199EB1C2FB2C(L_9, /*hidden argument*/NULL);
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_11 = __this->get_m_NetworkTransform_11();
		NullCheck(L_11);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_12 = NetworkTransform_get_targetSyncPosition_m8D2DCE0C4C4EDE2729E3323218669E433952A446_inline(L_11, /*hidden argument*/NULL);
		NullCheck(L_10);
		Transform_set_position_mDA89E4893F14ECA5CBEEE7FB80A5BF7C1B8EA6DC(L_10, L_12, /*hidden argument*/NULL);
		// if (m_NetworkTransform.rigidbody3D != null && m_Visualizer.GetComponent<Rigidbody>() != null)
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_13 = __this->get_m_NetworkTransform_11();
		NullCheck(L_13);
		Rigidbody_tE0A58EE5A1F7DC908EFFB4F0D795AC9552A750A5 * L_14 = NetworkTransform_get_rigidbody3D_m2F059AC7FE4AE29073DA4FB4D6D9719A35245DEB_inline(L_13, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_15 = Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1(L_14, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_15)
		{
			goto IL_00a1;
		}
	}
	{
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_16 = __this->get_m_Visualizer_12();
		NullCheck(L_16);
		Rigidbody_tE0A58EE5A1F7DC908EFFB4F0D795AC9552A750A5 * L_17 = GameObject_GetComponent_TisRigidbody_tE0A58EE5A1F7DC908EFFB4F0D795AC9552A750A5_m31F97A6E057858450728C32EE09647374FA10903(L_16, /*hidden argument*/GameObject_GetComponent_TisRigidbody_tE0A58EE5A1F7DC908EFFB4F0D795AC9552A750A5_m31F97A6E057858450728C32EE09647374FA10903_RuntimeMethod_var);
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_18 = Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1(L_17, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_18)
		{
			goto IL_00a1;
		}
	}
	{
		// m_Visualizer.GetComponent<Rigidbody>().velocity = m_NetworkTransform.targetSyncVelocity;
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_19 = __this->get_m_Visualizer_12();
		NullCheck(L_19);
		Rigidbody_tE0A58EE5A1F7DC908EFFB4F0D795AC9552A750A5 * L_20 = GameObject_GetComponent_TisRigidbody_tE0A58EE5A1F7DC908EFFB4F0D795AC9552A750A5_m31F97A6E057858450728C32EE09647374FA10903(L_19, /*hidden argument*/GameObject_GetComponent_TisRigidbody_tE0A58EE5A1F7DC908EFFB4F0D795AC9552A750A5_m31F97A6E057858450728C32EE09647374FA10903_RuntimeMethod_var);
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_21 = __this->get_m_NetworkTransform_11();
		NullCheck(L_21);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_22 = NetworkTransform_get_targetSyncVelocity_m7C47913B3EBFDC866349F5C091C439D255B75CFB_inline(L_21, /*hidden argument*/NULL);
		NullCheck(L_20);
		Rigidbody_set_velocity_m8D129E88E62AD02AB81CFC8BE694C4A5A2B2B380(L_20, L_22, /*hidden argument*/NULL);
	}

IL_00a1:
	{
		// if (m_NetworkTransform.rigidbody2D != null && m_Visualizer.GetComponent<Rigidbody2D>() != null)
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_23 = __this->get_m_NetworkTransform_11();
		NullCheck(L_23);
		Rigidbody2D_tBDC6900A76D3C47E291446FF008D02B817C81CDE * L_24 = NetworkTransform_get_rigidbody2D_mC7614E0AE776DEE2D14FCC7E41D90CD5D498F765_inline(L_23, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_25 = Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1(L_24, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_25)
		{
			goto IL_00e7;
		}
	}
	{
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_26 = __this->get_m_Visualizer_12();
		NullCheck(L_26);
		Rigidbody2D_tBDC6900A76D3C47E291446FF008D02B817C81CDE * L_27 = GameObject_GetComponent_TisRigidbody2D_tBDC6900A76D3C47E291446FF008D02B817C81CDE_mDDB82F02C3053DCC0D60C420752A11EC11CBACC0(L_26, /*hidden argument*/GameObject_GetComponent_TisRigidbody2D_tBDC6900A76D3C47E291446FF008D02B817C81CDE_mDDB82F02C3053DCC0D60C420752A11EC11CBACC0_RuntimeMethod_var);
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_28 = Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1(L_27, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_28)
		{
			goto IL_00e7;
		}
	}
	{
		// m_Visualizer.GetComponent<Rigidbody2D>().velocity = m_NetworkTransform.targetSyncVelocity;
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_29 = __this->get_m_Visualizer_12();
		NullCheck(L_29);
		Rigidbody2D_tBDC6900A76D3C47E291446FF008D02B817C81CDE * L_30 = GameObject_GetComponent_TisRigidbody2D_tBDC6900A76D3C47E291446FF008D02B817C81CDE_mDDB82F02C3053DCC0D60C420752A11EC11CBACC0(L_29, /*hidden argument*/GameObject_GetComponent_TisRigidbody2D_tBDC6900A76D3C47E291446FF008D02B817C81CDE_mDDB82F02C3053DCC0D60C420752A11EC11CBACC0_RuntimeMethod_var);
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_31 = __this->get_m_NetworkTransform_11();
		NullCheck(L_31);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_32 = NetworkTransform_get_targetSyncVelocity_m7C47913B3EBFDC866349F5C091C439D255B75CFB_inline(L_31, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Vector2_tA85D2DD88578276CA8A8796756458277E72D073D_il2cpp_TypeInfo_var);
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_33 = Vector2_op_Implicit_mEA1F75961E3D368418BA8CEB9C40E55C25BA3C28(L_32, /*hidden argument*/NULL);
		NullCheck(L_30);
		Rigidbody2D_set_velocity_mE0DBCE5B683024B106C2AB6943BBA550B5BD0B83(L_30, L_33, /*hidden argument*/NULL);
	}

IL_00e7:
	{
		// Quaternion targetFacing = Quaternion.identity;
		IL2CPP_RUNTIME_CLASS_INIT(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_il2cpp_TypeInfo_var);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_34 = Quaternion_get_identity_m548B37D80F2DEE60E41D1F09BF6889B557BE1A64(/*hidden argument*/NULL);
		V_0 = L_34;
		// if (m_NetworkTransform.rigidbody3D != null)
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_35 = __this->get_m_NetworkTransform_11();
		NullCheck(L_35);
		Rigidbody_tE0A58EE5A1F7DC908EFFB4F0D795AC9552A750A5 * L_36 = NetworkTransform_get_rigidbody3D_m2F059AC7FE4AE29073DA4FB4D6D9719A35245DEB_inline(L_35, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_37 = Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1(L_36, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_37)
		{
			goto IL_010c;
		}
	}
	{
		// targetFacing = m_NetworkTransform.targetSyncRotation3D;
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_38 = __this->get_m_NetworkTransform_11();
		NullCheck(L_38);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_39 = NetworkTransform_get_targetSyncRotation3D_m6418875DB7CC2500B5E0778D6BC890D2583B4DF8_inline(L_38, /*hidden argument*/NULL);
		V_0 = L_39;
	}

IL_010c:
	{
		// if (m_NetworkTransform.rigidbody2D != null)
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_40 = __this->get_m_NetworkTransform_11();
		NullCheck(L_40);
		Rigidbody2D_tBDC6900A76D3C47E291446FF008D02B817C81CDE * L_41 = NetworkTransform_get_rigidbody2D_mC7614E0AE776DEE2D14FCC7E41D90CD5D498F765_inline(L_40, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_42 = Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1(L_41, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_42)
		{
			goto IL_013a;
		}
	}
	{
		// targetFacing = Quaternion.Euler(0, 0, m_NetworkTransform.targetSyncRotation2D);
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_43 = __this->get_m_NetworkTransform_11();
		NullCheck(L_43);
		float L_44 = NetworkTransform_get_targetSyncRotation2D_mE1F4E6611853B634322EE9EF4517E7E2AF169BEA_inline(L_43, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_il2cpp_TypeInfo_var);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_45 = Quaternion_Euler_m537DD6CEAE0AD4274D8A84414C24C30730427D05((0.0f), (0.0f), L_44, /*hidden argument*/NULL);
		V_0 = L_45;
	}

IL_013a:
	{
		// m_Visualizer.transform.rotation = targetFacing;
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_46 = __this->get_m_Visualizer_12();
		NullCheck(L_46);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_47 = GameObject_get_transform_mA5C38857137F137CB96C69FAA624199EB1C2FB2C(L_46, /*hidden argument*/NULL);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_48 = V_0;
		NullCheck(L_47);
		Transform_set_rotation_m429694E264117C6DC682EC6AF45C7864E5155935(L_47, L_48, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformVisualizer::OnRenderObject()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformVisualizer_OnRenderObject_m9DCA4436C234C4B2EBEB4CECEC40B4E628EAF12F (NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkTransformVisualizer_OnRenderObject_m9DCA4436C234C4B2EBEB4CECEC40B4E628EAF12F_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (m_Visualizer == null)
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_0 = __this->get_m_Visualizer_12();
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_1 = Object_op_Equality_mBC2401774F3BE33E8CF6F0A8148E66C95D6CFF1C(L_0, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_000f;
		}
	}
	{
		// return;
		return;
	}

IL_000f:
	{
		// if (m_NetworkTransform.localPlayerAuthority && hasAuthority)
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_2 = __this->get_m_NetworkTransform_11();
		NullCheck(L_2);
		bool L_3 = NetworkBehaviour_get_localPlayerAuthority_m73DEE3D9A2E9916520CBDBA1B11888DAEA24B415(L_2, /*hidden argument*/NULL);
		if (!L_3)
		{
			goto IL_0025;
		}
	}
	{
		bool L_4 = NetworkBehaviour_get_hasAuthority_m20156D4B7D1F4097FFEAEFB2D0EAE8F95FF0B798(__this, /*hidden argument*/NULL);
		if (!L_4)
		{
			goto IL_0025;
		}
	}
	{
		// return;
		return;
	}

IL_0025:
	{
		// if (m_NetworkTransform.lastSyncTime == 0)
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_5 = __this->get_m_NetworkTransform_11();
		NullCheck(L_5);
		float L_6 = NetworkTransform_get_lastSyncTime_mD8AEBC7EDA370ACB0A222BF622BD95C54EBD6C9E_inline(L_5, /*hidden argument*/NULL);
		if ((!(((float)L_6) == ((float)(0.0f)))))
		{
			goto IL_0038;
		}
	}
	{
		// return;
		return;
	}

IL_0038:
	{
		// s_LineMaterial.SetPass(0);
		Material_tF7DB3BF0C24DEC2FE0CB51E5DF5053D5223C8598 * L_7 = ((NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386_StaticFields*)il2cpp_codegen_static_fields_for(NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386_il2cpp_TypeInfo_var))->get_s_LineMaterial_13();
		NullCheck(L_7);
		Material_SetPass_m4BE0A8FCBF158C83522AA2F69118A2FE33683918(L_7, 0, /*hidden argument*/NULL);
		// GL.Begin(GL.LINES);
		GL_Begin_m9A48BD6A2DA850D54250EF638DF5EC61F83E293C(1, /*hidden argument*/NULL);
		// GL.Color(Color.white);
		Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  L_8 = Color_get_white_mE7F3AC4FF0D6F35E48049C73116A222CBE96D905(/*hidden argument*/NULL);
		GL_Color_m6F50BBCC316C56A746CDF224DE1A27FEEB359D8E(L_8, /*hidden argument*/NULL);
		// GL.Vertex3(transform.position.x, transform.position.y, transform.position.z);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_9 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(__this, /*hidden argument*/NULL);
		NullCheck(L_9);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_10 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_9, /*hidden argument*/NULL);
		float L_11 = L_10.get_x_2();
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_12 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(__this, /*hidden argument*/NULL);
		NullCheck(L_12);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_13 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_12, /*hidden argument*/NULL);
		float L_14 = L_13.get_y_3();
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_15 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(__this, /*hidden argument*/NULL);
		NullCheck(L_15);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_16 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_15, /*hidden argument*/NULL);
		float L_17 = L_16.get_z_4();
		GL_Vertex3_mE94809C1522CE96DF4C6CD218B1A26D5E60A114E(L_11, L_14, L_17, /*hidden argument*/NULL);
		// GL.Vertex3(m_NetworkTransform.targetSyncPosition.x, m_NetworkTransform.targetSyncPosition.y, m_NetworkTransform.targetSyncPosition.z);
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_18 = __this->get_m_NetworkTransform_11();
		NullCheck(L_18);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_19 = NetworkTransform_get_targetSyncPosition_m8D2DCE0C4C4EDE2729E3323218669E433952A446_inline(L_18, /*hidden argument*/NULL);
		float L_20 = L_19.get_x_2();
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_21 = __this->get_m_NetworkTransform_11();
		NullCheck(L_21);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_22 = NetworkTransform_get_targetSyncPosition_m8D2DCE0C4C4EDE2729E3323218669E433952A446_inline(L_21, /*hidden argument*/NULL);
		float L_23 = L_22.get_y_3();
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_24 = __this->get_m_NetworkTransform_11();
		NullCheck(L_24);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_25 = NetworkTransform_get_targetSyncPosition_m8D2DCE0C4C4EDE2729E3323218669E433952A446_inline(L_24, /*hidden argument*/NULL);
		float L_26 = L_25.get_z_4();
		GL_Vertex3_mE94809C1522CE96DF4C6CD218B1A26D5E60A114E(L_20, L_23, L_26, /*hidden argument*/NULL);
		// GL.End();
		GL_End_m7EDEB843BD9F7E00BD838FDE074B4688C55C0755(/*hidden argument*/NULL);
		// DrawRotationInterpolation();
		NetworkTransformVisualizer_DrawRotationInterpolation_m61EA01F463B4524948B44B9142347C44B9C5A0B0(__this, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformVisualizer::DrawRotationInterpolation()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformVisualizer_DrawRotationInterpolation_m61EA01F463B4524948B44B9142347C44B9C5A0B0 (NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkTransformVisualizer_DrawRotationInterpolation_m61EA01F463B4524948B44B9142347C44B9C5A0B0_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  V_0;
	memset((&V_0), 0, sizeof(V_0));
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_1;
	memset((&V_1), 0, sizeof(V_1));
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_2;
	memset((&V_2), 0, sizeof(V_2));
	Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  V_3;
	memset((&V_3), 0, sizeof(V_3));
	{
		// Quaternion targetFacing = Quaternion.identity;
		IL2CPP_RUNTIME_CLASS_INIT(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_il2cpp_TypeInfo_var);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_0 = Quaternion_get_identity_m548B37D80F2DEE60E41D1F09BF6889B557BE1A64(/*hidden argument*/NULL);
		V_0 = L_0;
		// if (m_NetworkTransform.rigidbody3D != null)
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_1 = __this->get_m_NetworkTransform_11();
		NullCheck(L_1);
		Rigidbody_tE0A58EE5A1F7DC908EFFB4F0D795AC9552A750A5 * L_2 = NetworkTransform_get_rigidbody3D_m2F059AC7FE4AE29073DA4FB4D6D9719A35245DEB_inline(L_1, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_3 = Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1(L_2, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_3)
		{
			goto IL_0025;
		}
	}
	{
		// targetFacing = m_NetworkTransform.targetSyncRotation3D;
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_4 = __this->get_m_NetworkTransform_11();
		NullCheck(L_4);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_5 = NetworkTransform_get_targetSyncRotation3D_m6418875DB7CC2500B5E0778D6BC890D2583B4DF8_inline(L_4, /*hidden argument*/NULL);
		V_0 = L_5;
	}

IL_0025:
	{
		// if (m_NetworkTransform.rigidbody2D != null)
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_6 = __this->get_m_NetworkTransform_11();
		NullCheck(L_6);
		Rigidbody2D_tBDC6900A76D3C47E291446FF008D02B817C81CDE * L_7 = NetworkTransform_get_rigidbody2D_mC7614E0AE776DEE2D14FCC7E41D90CD5D498F765_inline(L_6, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_8 = Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1(L_7, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_8)
		{
			goto IL_0053;
		}
	}
	{
		// targetFacing = Quaternion.Euler(0, 0, m_NetworkTransform.targetSyncRotation2D);
		NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * L_9 = __this->get_m_NetworkTransform_11();
		NullCheck(L_9);
		float L_10 = NetworkTransform_get_targetSyncRotation2D_mE1F4E6611853B634322EE9EF4517E7E2AF169BEA_inline(L_9, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_il2cpp_TypeInfo_var);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_11 = Quaternion_Euler_m537DD6CEAE0AD4274D8A84414C24C30730427D05((0.0f), (0.0f), L_10, /*hidden argument*/NULL);
		V_0 = L_11;
	}

IL_0053:
	{
		// if (targetFacing == Quaternion.identity)
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_12 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_il2cpp_TypeInfo_var);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_13 = Quaternion_get_identity_m548B37D80F2DEE60E41D1F09BF6889B557BE1A64(/*hidden argument*/NULL);
		bool L_14 = Quaternion_op_Equality_m0DBCE8FE48EEF2D7C79741E498BFFB984DF4956F(L_12, L_13, /*hidden argument*/NULL);
		if (!L_14)
		{
			goto IL_0061;
		}
	}
	{
		// return;
		return;
	}

IL_0061:
	{
		// GL.Begin(GL.LINES);
		GL_Begin_m9A48BD6A2DA850D54250EF638DF5EC61F83E293C(1, /*hidden argument*/NULL);
		// GL.Color(Color.yellow);
		Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  L_15 = Color_get_yellow_mC8BD62CCC364EA5FC4273D4C2E116D0E2DE135AE(/*hidden argument*/NULL);
		GL_Color_m6F50BBCC316C56A746CDF224DE1A27FEEB359D8E(L_15, /*hidden argument*/NULL);
		// GL.Vertex3(transform.position.x, transform.position.y, transform.position.z);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_16 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(__this, /*hidden argument*/NULL);
		NullCheck(L_16);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_17 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_16, /*hidden argument*/NULL);
		float L_18 = L_17.get_x_2();
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_19 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(__this, /*hidden argument*/NULL);
		NullCheck(L_19);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_20 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_19, /*hidden argument*/NULL);
		float L_21 = L_20.get_y_3();
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_22 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(__this, /*hidden argument*/NULL);
		NullCheck(L_22);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_23 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_22, /*hidden argument*/NULL);
		float L_24 = L_23.get_z_4();
		GL_Vertex3_mE94809C1522CE96DF4C6CD218B1A26D5E60A114E(L_18, L_21, L_24, /*hidden argument*/NULL);
		// Vector3 actualFront = transform.position + transform.right;
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_25 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(__this, /*hidden argument*/NULL);
		NullCheck(L_25);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_26 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_25, /*hidden argument*/NULL);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_27 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(__this, /*hidden argument*/NULL);
		NullCheck(L_27);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_28 = Transform_get_right_mC32CE648E98D3D4F62F897A2751EE567C7C0CFB0(L_27, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_29 = Vector3_op_Addition_m929F9C17E5D11B94D50B4AFF1D730B70CB59B50E(L_26, L_28, /*hidden argument*/NULL);
		V_1 = L_29;
		// GL.Vertex3(actualFront.x, actualFront.y, actualFront.z);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_30 = V_1;
		float L_31 = L_30.get_x_2();
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_32 = V_1;
		float L_33 = L_32.get_y_3();
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_34 = V_1;
		float L_35 = L_34.get_z_4();
		GL_Vertex3_mE94809C1522CE96DF4C6CD218B1A26D5E60A114E(L_31, L_33, L_35, /*hidden argument*/NULL);
		// GL.End();
		GL_End_m7EDEB843BD9F7E00BD838FDE074B4688C55C0755(/*hidden argument*/NULL);
		// GL.Begin(GL.LINES);
		GL_Begin_m9A48BD6A2DA850D54250EF638DF5EC61F83E293C(1, /*hidden argument*/NULL);
		// GL.Color(Color.green);
		Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  L_36 = Color_get_green_mD53D8F980E92A0755759FBB2981E3DDEFCD084C0(/*hidden argument*/NULL);
		GL_Color_m6F50BBCC316C56A746CDF224DE1A27FEEB359D8E(L_36, /*hidden argument*/NULL);
		// GL.Vertex3(transform.position.x, transform.position.y, transform.position.z);
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_37 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(__this, /*hidden argument*/NULL);
		NullCheck(L_37);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_38 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_37, /*hidden argument*/NULL);
		float L_39 = L_38.get_x_2();
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_40 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(__this, /*hidden argument*/NULL);
		NullCheck(L_40);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_41 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_40, /*hidden argument*/NULL);
		float L_42 = L_41.get_y_3();
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_43 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(__this, /*hidden argument*/NULL);
		NullCheck(L_43);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_44 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_43, /*hidden argument*/NULL);
		float L_45 = L_44.get_z_4();
		GL_Vertex3_mE94809C1522CE96DF4C6CD218B1A26D5E60A114E(L_39, L_42, L_45, /*hidden argument*/NULL);
		// Vector3 targetPositionOffset = (targetFacing * Vector3.right);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_46 = V_0;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_47 = Vector3_get_right_m6DD9559CA0C75BBA42D9140021C4C2A9AAA9B3F5(/*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357_il2cpp_TypeInfo_var);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_48 = Quaternion_op_Multiply_mD5999DE317D808808B72E58E7A978C4C0995879C(L_46, L_47, /*hidden argument*/NULL);
		V_2 = L_48;
		// Vector3 targetFront = transform.position + targetPositionOffset;
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_49 = Component_get_transform_m00F05BD782F920C301A7EBA480F3B7A904C07EC9(__this, /*hidden argument*/NULL);
		NullCheck(L_49);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_50 = Transform_get_position_mF54C3A064F7C8E24F1C56EE128728B2E4485E294(L_49, /*hidden argument*/NULL);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_51 = V_2;
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_52 = Vector3_op_Addition_m929F9C17E5D11B94D50B4AFF1D730B70CB59B50E(L_50, L_51, /*hidden argument*/NULL);
		V_3 = L_52;
		// GL.Vertex3(targetFront.x, targetFront.y, targetFront.z);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_53 = V_3;
		float L_54 = L_53.get_x_2();
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_55 = V_3;
		float L_56 = L_55.get_y_3();
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_57 = V_3;
		float L_58 = L_57.get_z_4();
		GL_Vertex3_mE94809C1522CE96DF4C6CD218B1A26D5E60A114E(L_54, L_56, L_58, /*hidden argument*/NULL);
		// GL.End();
		GL_End_m7EDEB843BD9F7E00BD838FDE074B4688C55C0755(/*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformVisualizer::CreateLineMaterial()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformVisualizer_CreateLineMaterial_m7939398CB6B61BFD7D237E3685D50000C7A41B89 (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkTransformVisualizer_CreateLineMaterial_m7939398CB6B61BFD7D237E3685D50000C7A41B89_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Shader_tE2731FF351B74AB4186897484FB01E000C1160CA * V_0 = NULL;
	{
		// if (s_LineMaterial)
		Material_tF7DB3BF0C24DEC2FE0CB51E5DF5053D5223C8598 * L_0 = ((NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386_StaticFields*)il2cpp_codegen_static_fields_for(NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386_il2cpp_TypeInfo_var))->get_s_LineMaterial_13();
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_1 = Object_op_Implicit_m8B2A44B4B1406ED346D1AE6D962294FD58D0D534(L_0, /*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_000d;
		}
	}
	{
		// return;
		return;
	}

IL_000d:
	{
		// var shader = Shader.Find("Hidden/Internal-Colored");
		Shader_tE2731FF351B74AB4186897484FB01E000C1160CA * L_2 = Shader_Find_m755654AA68D1C663A3E20A10E00CDC10F96C962B(_stringLiteralF11AF337B3340D92B47E93D08CB0B65A6AE686F5, /*hidden argument*/NULL);
		V_0 = L_2;
		// if (!shader)
		Shader_tE2731FF351B74AB4186897484FB01E000C1160CA * L_3 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_4 = Object_op_Implicit_m8B2A44B4B1406ED346D1AE6D962294FD58D0D534(L_3, /*hidden argument*/NULL);
		if (L_4)
		{
			goto IL_002b;
		}
	}
	{
		// Debug.LogWarning("Could not find Colored builtin shader");
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_LogWarning_m37338644DC81F640CCDFEAE35A223F0E965F0568(_stringLiteralD4C310BD45DF218471473B38695DF6F80D91CA0F, /*hidden argument*/NULL);
		// return;
		return;
	}

IL_002b:
	{
		// s_LineMaterial = new Material(shader);
		Shader_tE2731FF351B74AB4186897484FB01E000C1160CA * L_5 = V_0;
		Material_tF7DB3BF0C24DEC2FE0CB51E5DF5053D5223C8598 * L_6 = (Material_tF7DB3BF0C24DEC2FE0CB51E5DF5053D5223C8598 *)il2cpp_codegen_object_new(Material_tF7DB3BF0C24DEC2FE0CB51E5DF5053D5223C8598_il2cpp_TypeInfo_var);
		Material__ctor_m81E76B5C1316004F25D4FE9CEC0E78A7428DABA8(L_6, L_5, /*hidden argument*/NULL);
		((NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386_StaticFields*)il2cpp_codegen_static_fields_for(NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386_il2cpp_TypeInfo_var))->set_s_LineMaterial_13(L_6);
		// s_LineMaterial.hideFlags = HideFlags.HideAndDontSave;
		Material_tF7DB3BF0C24DEC2FE0CB51E5DF5053D5223C8598 * L_7 = ((NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386_StaticFields*)il2cpp_codegen_static_fields_for(NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386_il2cpp_TypeInfo_var))->get_s_LineMaterial_13();
		NullCheck(L_7);
		Object_set_hideFlags_mB0B45A19A5871EF407D7B09E0EB76003496BA4F0(L_7, ((int32_t)61), /*hidden argument*/NULL);
		// s_LineMaterial.SetInt("_ZWrite", 0);
		Material_tF7DB3BF0C24DEC2FE0CB51E5DF5053D5223C8598 * L_8 = ((NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386_StaticFields*)il2cpp_codegen_static_fields_for(NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386_il2cpp_TypeInfo_var))->get_s_LineMaterial_13();
		NullCheck(L_8);
		Material_SetInt_m1FCBDBB985E6A299AE11C3D8AF29BB4D7C7DF278(L_8, _stringLiteralD48C67736A90281297DD96BF118099E6CB6939B8, 0, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkTransformVisualizer::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkTransformVisualizer__ctor_mF6B5C3D5D9432CDA3B546D0261830BCFB8C6909A (NetworkTransformVisualizer_t526915A77613D12306A6A3D7C34BC1E13DFB5386 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkTransformVisualizer__ctor_mF6B5C3D5D9432CDA3B546D0261830BCFB8C6909A_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		IL2CPP_RUNTIME_CLASS_INIT(NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C_il2cpp_TypeInfo_var);
		NetworkBehaviour__ctor_m37D8F4B6AD273AFBE5507BB02D956282684A0B78(__this, /*hidden argument*/NULL);
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
// System.Void UnityEngine.Networking.NetworkWriter::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter__ctor_m43E453A4A5244815EC8D906B22E5D85FB7535D33 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkWriter__ctor_m43E453A4A5244815EC8D906B22E5D85FB7535D33_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public NetworkWriter()
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		// m_Buffer = new NetBuffer();
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_0 = (NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C *)il2cpp_codegen_object_new(NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C_il2cpp_TypeInfo_var);
		NetBuffer__ctor_m2E59DFECCECE03A1FEC0A37B544DF3C75E4137DD(L_0, /*hidden argument*/NULL);
		__this->set_m_Buffer_1(L_0);
		// if (s_Encoding == null)
		Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * L_1 = ((NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_StaticFields*)il2cpp_codegen_static_fields_for(NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_il2cpp_TypeInfo_var))->get_s_Encoding_2();
		if (L_1)
		{
			goto IL_0031;
		}
	}
	{
		// s_Encoding = new UTF8Encoding();
		UTF8Encoding_t77ED103B749A387EF072C3429F48C91D12CA08DE * L_2 = (UTF8Encoding_t77ED103B749A387EF072C3429F48C91D12CA08DE *)il2cpp_codegen_object_new(UTF8Encoding_t77ED103B749A387EF072C3429F48C91D12CA08DE_il2cpp_TypeInfo_var);
		UTF8Encoding__ctor_m999E138A2E4C290F8A97866714EE53D58C931488(L_2, /*hidden argument*/NULL);
		((NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_StaticFields*)il2cpp_codegen_static_fields_for(NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_il2cpp_TypeInfo_var))->set_s_Encoding_2(L_2);
		// s_StringWriteBuffer = new byte[k_MaxStringLength];
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_3 = (ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821*)(ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821*)SZArrayNew(ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821_il2cpp_TypeInfo_var, (uint32_t)((int32_t)32768));
		((NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_StaticFields*)il2cpp_codegen_static_fields_for(NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_il2cpp_TypeInfo_var))->set_s_StringWriteBuffer_3(L_3);
	}

IL_0031:
	{
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::.ctor(System.Byte[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter__ctor_m99A86656D77FE374861A287BBA85CD63C26FB6FC (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___buffer0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkWriter__ctor_m99A86656D77FE374861A287BBA85CD63C26FB6FC_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public NetworkWriter(byte[] buffer)
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		// m_Buffer = new NetBuffer(buffer);
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_0 = ___buffer0;
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_1 = (NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C *)il2cpp_codegen_object_new(NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C_il2cpp_TypeInfo_var);
		NetBuffer__ctor_m5AE89C6DC720184249448D73CF59ACC7B58E3CBF(L_1, L_0, /*hidden argument*/NULL);
		__this->set_m_Buffer_1(L_1);
		// if (s_Encoding == null)
		Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * L_2 = ((NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_StaticFields*)il2cpp_codegen_static_fields_for(NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_il2cpp_TypeInfo_var))->get_s_Encoding_2();
		if (L_2)
		{
			goto IL_0032;
		}
	}
	{
		// s_Encoding = new UTF8Encoding();
		UTF8Encoding_t77ED103B749A387EF072C3429F48C91D12CA08DE * L_3 = (UTF8Encoding_t77ED103B749A387EF072C3429F48C91D12CA08DE *)il2cpp_codegen_object_new(UTF8Encoding_t77ED103B749A387EF072C3429F48C91D12CA08DE_il2cpp_TypeInfo_var);
		UTF8Encoding__ctor_m999E138A2E4C290F8A97866714EE53D58C931488(L_3, /*hidden argument*/NULL);
		((NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_StaticFields*)il2cpp_codegen_static_fields_for(NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_il2cpp_TypeInfo_var))->set_s_Encoding_2(L_3);
		// s_StringWriteBuffer = new byte[k_MaxStringLength];
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_4 = (ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821*)(ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821*)SZArrayNew(ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821_il2cpp_TypeInfo_var, (uint32_t)((int32_t)32768));
		((NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_StaticFields*)il2cpp_codegen_static_fields_for(NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_il2cpp_TypeInfo_var))->set_s_StringWriteBuffer_3(L_4);
	}

IL_0032:
	{
		// }
		return;
	}
}
// System.Int16 UnityEngine.Networking.NetworkWriter::get_Position()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int16_t NetworkWriter_get_Position_m531CCC6DA8F6570F48B31A93B91822F51165E9F6 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, const RuntimeMethod* method)
{
	{
		// public short Position { get { return (short)m_Buffer.Position; } }
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_0 = __this->get_m_Buffer_1();
		NullCheck(L_0);
		uint32_t L_1 = NetBuffer_get_Position_m1F0C4B8C3EDCCB0D65CE51B4709FDAF2017938AB_inline(L_0, /*hidden argument*/NULL);
		return (((int16_t)((int16_t)L_1)));
	}
}
// System.Byte[] UnityEngine.Networking.NetworkWriter::ToArray()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* NetworkWriter_ToArray_mA27B02013E80F7673A07A60EE0DE2A657F9E05A8 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkWriter_ToArray_mA27B02013E80F7673A07A60EE0DE2A657F9E05A8_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* V_0 = NULL;
	ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA  V_1;
	memset((&V_1), 0, sizeof(V_1));
	{
		// var newArray = new byte[m_Buffer.AsArraySegment().Count];
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_0 = __this->get_m_Buffer_1();
		NullCheck(L_0);
		ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA  L_1 = NetBuffer_AsArraySegment_mF6086E61EC8BCA66D2AD8DE5F271075E8BDF47EC(L_0, /*hidden argument*/NULL);
		V_1 = L_1;
		int32_t L_2 = ArraySegment_1_get_Count_m02387DADA172F909FD346559D93990E990E05352_inline((ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA *)(&V_1), /*hidden argument*/ArraySegment_1_get_Count_m02387DADA172F909FD346559D93990E990E05352_RuntimeMethod_var);
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_3 = (ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821*)(ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821*)SZArrayNew(ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821_il2cpp_TypeInfo_var, (uint32_t)L_2);
		V_0 = L_3;
		// Array.Copy(m_Buffer.AsArraySegment().Array, newArray, m_Buffer.AsArraySegment().Count);
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_4 = __this->get_m_Buffer_1();
		NullCheck(L_4);
		ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA  L_5 = NetBuffer_AsArraySegment_mF6086E61EC8BCA66D2AD8DE5F271075E8BDF47EC(L_4, /*hidden argument*/NULL);
		V_1 = L_5;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_6 = ArraySegment_1_get_Array_m41D93EFB7EAB3081C0A27ED9891E7177F5F361B7_inline((ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA *)(&V_1), /*hidden argument*/ArraySegment_1_get_Array_m41D93EFB7EAB3081C0A27ED9891E7177F5F361B7_RuntimeMethod_var);
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_7 = V_0;
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_8 = __this->get_m_Buffer_1();
		NullCheck(L_8);
		ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA  L_9 = NetBuffer_AsArraySegment_mF6086E61EC8BCA66D2AD8DE5F271075E8BDF47EC(L_8, /*hidden argument*/NULL);
		V_1 = L_9;
		int32_t L_10 = ArraySegment_1_get_Count_m02387DADA172F909FD346559D93990E990E05352_inline((ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA *)(&V_1), /*hidden argument*/ArraySegment_1_get_Count_m02387DADA172F909FD346559D93990E990E05352_RuntimeMethod_var);
		Array_Copy_m2D96731C600DE8A167348CA8BA796344E64F7434((RuntimeArray *)(RuntimeArray *)L_6, (RuntimeArray *)(RuntimeArray *)L_7, L_10, /*hidden argument*/NULL);
		// return newArray;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_11 = V_0;
		return L_11;
	}
}
// System.Byte[] UnityEngine.Networking.NetworkWriter::AsArray()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* NetworkWriter_AsArray_mE90AC762796F17DD398523A8C230DD9B2E2373D5 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkWriter_AsArray_mE90AC762796F17DD398523A8C230DD9B2E2373D5_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA  V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		// return AsArraySegment().Array;
		ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA  L_0 = NetworkWriter_AsArraySegment_m4CF129BE51C5B5F2E1BD5EB4AA5D8B70E06E4A97(__this, /*hidden argument*/NULL);
		V_0 = L_0;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_1 = ArraySegment_1_get_Array_m41D93EFB7EAB3081C0A27ED9891E7177F5F361B7_inline((ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA *)(&V_0), /*hidden argument*/ArraySegment_1_get_Array_m41D93EFB7EAB3081C0A27ED9891E7177F5F361B7_RuntimeMethod_var);
		return L_1;
	}
}
// System.ArraySegment`1<System.Byte> UnityEngine.Networking.NetworkWriter::AsArraySegment()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA  NetworkWriter_AsArraySegment_m4CF129BE51C5B5F2E1BD5EB4AA5D8B70E06E4A97 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, const RuntimeMethod* method)
{
	{
		// return m_Buffer.AsArraySegment();
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_0 = __this->get_m_Buffer_1();
		NullCheck(L_0);
		ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA  L_1 = NetBuffer_AsArraySegment_mF6086E61EC8BCA66D2AD8DE5F271075E8BDF47EC(L_0, /*hidden argument*/NULL);
		return L_1;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::WritePackedUInt32(System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_WritePackedUInt32_m99DCA40833B068CB958663A5B583BC8D2051B12F (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, uint32_t ___value0, const RuntimeMethod* method)
{
	{
		// if (value <= 240)
		uint32_t L_0 = ___value0;
		if ((!(((uint32_t)L_0) <= ((uint32_t)((int32_t)240)))))
		{
			goto IL_0011;
		}
	}
	{
		// Write((byte)value);
		uint32_t L_1 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)L_1))), /*hidden argument*/NULL);
		// return;
		return;
	}

IL_0011:
	{
		// if (value <= 2287)
		uint32_t L_2 = ___value0;
		if ((!(((uint32_t)L_2) <= ((uint32_t)((int32_t)2287)))))
		{
			goto IL_0048;
		}
	}
	{
		// Write((byte)((value - 240) / 256 + 241));
		uint32_t L_3 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int32_t)il2cpp_codegen_add((int32_t)((int32_t)((uint32_t)(int32_t)((int32_t)il2cpp_codegen_subtract((int32_t)L_3, (int32_t)((int32_t)240)))/(uint32_t)(int32_t)((int32_t)256))), (int32_t)((int32_t)241)))))), /*hidden argument*/NULL);
		// Write((byte)((value - 240) % 256));
		uint32_t L_4 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int32_t)((uint32_t)(int32_t)((int32_t)il2cpp_codegen_subtract((int32_t)L_4, (int32_t)((int32_t)240)))%(uint32_t)(int32_t)((int32_t)256)))))), /*hidden argument*/NULL);
		// return;
		return;
	}

IL_0048:
	{
		// if (value <= 67823)
		uint32_t L_5 = ___value0;
		if ((!(((uint32_t)L_5) <= ((uint32_t)((int32_t)67823)))))
		{
			goto IL_0084;
		}
	}
	{
		// Write((byte)249);
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)((int32_t)249), /*hidden argument*/NULL);
		// Write((byte)((value - 2288) / 256));
		uint32_t L_6 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int32_t)((uint32_t)(int32_t)((int32_t)il2cpp_codegen_subtract((int32_t)L_6, (int32_t)((int32_t)2288)))/(uint32_t)(int32_t)((int32_t)256)))))), /*hidden argument*/NULL);
		// Write((byte)((value - 2288) % 256));
		uint32_t L_7 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int32_t)((uint32_t)(int32_t)((int32_t)il2cpp_codegen_subtract((int32_t)L_7, (int32_t)((int32_t)2288)))%(uint32_t)(int32_t)((int32_t)256)))))), /*hidden argument*/NULL);
		// return;
		return;
	}

IL_0084:
	{
		// if (value <= 16777215)
		uint32_t L_8 = ___value0;
		if ((!(((uint32_t)L_8) <= ((uint32_t)((int32_t)16777215)))))
		{
			goto IL_00c7;
		}
	}
	{
		// Write((byte)250);
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)((int32_t)250), /*hidden argument*/NULL);
		// Write((byte)(value & 0xFF));
		uint32_t L_9 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)L_9&(int32_t)((int32_t)255)))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 8) & 0xFF));
		uint32_t L_10 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)((int32_t)((uint32_t)L_10>>8))&(int32_t)((int32_t)255)))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 16) & 0xFF));
		uint32_t L_11 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)((int32_t)((uint32_t)L_11>>((int32_t)16)))&(int32_t)((int32_t)255)))))), /*hidden argument*/NULL);
		// return;
		return;
	}

IL_00c7:
	{
		// Write((byte)251);
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)((int32_t)251), /*hidden argument*/NULL);
		// Write((byte)(value & 0xFF));
		uint32_t L_12 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)L_12&(int32_t)((int32_t)255)))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 8) & 0xFF));
		uint32_t L_13 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)((int32_t)((uint32_t)L_13>>8))&(int32_t)((int32_t)255)))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 16) & 0xFF));
		uint32_t L_14 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)((int32_t)((uint32_t)L_14>>((int32_t)16)))&(int32_t)((int32_t)255)))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 24) & 0xFF));
		uint32_t L_15 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)((int32_t)((uint32_t)L_15>>((int32_t)24)))&(int32_t)((int32_t)255)))))), /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::WritePackedUInt64(System.UInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_WritePackedUInt64_mE3440C6795089D2D37453F493B1EF534ABCA0468 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, uint64_t ___value0, const RuntimeMethod* method)
{
	{
		// if (value <= 240)
		uint64_t L_0 = ___value0;
		if ((!(((uint64_t)L_0) <= ((uint64_t)(((int64_t)((int64_t)((int32_t)240))))))))
		{
			goto IL_0012;
		}
	}
	{
		// Write((byte)value);
		uint64_t L_1 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)L_1))), /*hidden argument*/NULL);
		// return;
		return;
	}

IL_0012:
	{
		// if (value <= 2287)
		uint64_t L_2 = ___value0;
		if ((!(((uint64_t)L_2) <= ((uint64_t)(((int64_t)((int64_t)((int32_t)2287))))))))
		{
			goto IL_004f;
		}
	}
	{
		// Write((byte)((value - 240) / 256 + 241));
		uint64_t L_3 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)il2cpp_codegen_add((int64_t)((int64_t)((uint64_t)(int64_t)((int64_t)il2cpp_codegen_subtract((int64_t)L_3, (int64_t)(((int64_t)((int64_t)((int32_t)240))))))/(uint64_t)(int64_t)(((int64_t)((int64_t)((int32_t)256)))))), (int64_t)(((int64_t)((int64_t)((int32_t)241))))))))), /*hidden argument*/NULL);
		// Write((byte)((value - 240) % 256));
		uint64_t L_4 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((uint64_t)(int64_t)((int64_t)il2cpp_codegen_subtract((int64_t)L_4, (int64_t)(((int64_t)((int64_t)((int32_t)240))))))%(uint64_t)(int64_t)(((int64_t)((int64_t)((int32_t)256))))))))), /*hidden argument*/NULL);
		// return;
		return;
	}

IL_004f:
	{
		// if (value <= 67823)
		uint64_t L_5 = ___value0;
		if ((!(((uint64_t)L_5) <= ((uint64_t)(((int64_t)((int64_t)((int32_t)67823))))))))
		{
			goto IL_0090;
		}
	}
	{
		// Write((byte)249);
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)((int32_t)249), /*hidden argument*/NULL);
		// Write((byte)((value - 2288) / 256));
		uint64_t L_6 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((uint64_t)(int64_t)((int64_t)il2cpp_codegen_subtract((int64_t)L_6, (int64_t)(((int64_t)((int64_t)((int32_t)2288))))))/(uint64_t)(int64_t)(((int64_t)((int64_t)((int32_t)256))))))))), /*hidden argument*/NULL);
		// Write((byte)((value - 2288) % 256));
		uint64_t L_7 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((uint64_t)(int64_t)((int64_t)il2cpp_codegen_subtract((int64_t)L_7, (int64_t)(((int64_t)((int64_t)((int32_t)2288))))))%(uint64_t)(int64_t)(((int64_t)((int64_t)((int32_t)256))))))))), /*hidden argument*/NULL);
		// return;
		return;
	}

IL_0090:
	{
		// if (value <= 16777215)
		uint64_t L_8 = ___value0;
		if ((!(((uint64_t)L_8) <= ((uint64_t)(((int64_t)((int64_t)((int32_t)16777215))))))))
		{
			goto IL_00d7;
		}
	}
	{
		// Write((byte)250);
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)((int32_t)250), /*hidden argument*/NULL);
		// Write((byte)(value & 0xFF));
		uint64_t L_9 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)L_9&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 8) & 0xFF));
		uint64_t L_10 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_10>>8))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 16) & 0xFF));
		uint64_t L_11 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_11>>((int32_t)16)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// return;
		return;
	}

IL_00d7:
	{
		// if (value <= 4294967295)
		uint64_t L_12 = ___value0;
		if ((!(((uint64_t)L_12) <= ((uint64_t)(((int64_t)((uint64_t)(((uint32_t)((uint32_t)(-1)))))))))))
		{
			goto IL_012c;
		}
	}
	{
		// Write((byte)251);
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)((int32_t)251), /*hidden argument*/NULL);
		// Write((byte)(value & 0xFF));
		uint64_t L_13 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)L_13&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 8) & 0xFF));
		uint64_t L_14 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_14>>8))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 16) & 0xFF));
		uint64_t L_15 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_15>>((int32_t)16)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 24) & 0xFF));
		uint64_t L_16 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_16>>((int32_t)24)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// return;
		return;
	}

IL_012c:
	{
		// if (value <= 1099511627775)
		uint64_t L_17 = ___value0;
		if ((!(((uint64_t)L_17) <= ((uint64_t)((int64_t)1099511627775LL)))))
		{
			goto IL_019a;
		}
	}
	{
		// Write((byte)252);
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)((int32_t)252), /*hidden argument*/NULL);
		// Write((byte)(value & 0xFF));
		uint64_t L_18 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)L_18&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 8) & 0xFF));
		uint64_t L_19 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_19>>8))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 16) & 0xFF));
		uint64_t L_20 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_20>>((int32_t)16)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 24) & 0xFF));
		uint64_t L_21 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_21>>((int32_t)24)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 32) & 0xFF));
		uint64_t L_22 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_22>>((int32_t)32)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// return;
		return;
	}

IL_019a:
	{
		// if (value <= 281474976710655)
		uint64_t L_23 = ___value0;
		if ((!(((uint64_t)L_23) <= ((uint64_t)((int64_t)281474976710655LL)))))
		{
			goto IL_021a;
		}
	}
	{
		// Write((byte)253);
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)((int32_t)253), /*hidden argument*/NULL);
		// Write((byte)(value & 0xFF));
		uint64_t L_24 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)L_24&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 8) & 0xFF));
		uint64_t L_25 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_25>>8))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 16) & 0xFF));
		uint64_t L_26 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_26>>((int32_t)16)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 24) & 0xFF));
		uint64_t L_27 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_27>>((int32_t)24)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 32) & 0xFF));
		uint64_t L_28 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_28>>((int32_t)32)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 40) & 0xFF));
		uint64_t L_29 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_29>>((int32_t)40)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// return;
		return;
	}

IL_021a:
	{
		// if (value <= 72057594037927935)
		uint64_t L_30 = ___value0;
		if ((!(((uint64_t)L_30) <= ((uint64_t)((int64_t)72057594037927935LL)))))
		{
			goto IL_02af;
		}
	}
	{
		// Write((byte)254);
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)((int32_t)254), /*hidden argument*/NULL);
		// Write((byte)(value & 0xFF));
		uint64_t L_31 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)L_31&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 8) & 0xFF));
		uint64_t L_32 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_32>>8))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 16) & 0xFF));
		uint64_t L_33 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_33>>((int32_t)16)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 24) & 0xFF));
		uint64_t L_34 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_34>>((int32_t)24)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 32) & 0xFF));
		uint64_t L_35 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_35>>((int32_t)32)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 40) & 0xFF));
		uint64_t L_36 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_36>>((int32_t)40)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 48) & 0xFF));
		uint64_t L_37 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_37>>((int32_t)48)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// return;
		return;
	}

IL_02af:
	{
		// Write((byte)255);
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)((int32_t)255), /*hidden argument*/NULL);
		// Write((byte)(value & 0xFF));
		uint64_t L_38 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)L_38&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 8) & 0xFF));
		uint64_t L_39 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_39>>8))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 16) & 0xFF));
		uint64_t L_40 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_40>>((int32_t)16)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 24) & 0xFF));
		uint64_t L_41 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_41>>((int32_t)24)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 32) & 0xFF));
		uint64_t L_42 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_42>>((int32_t)32)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 40) & 0xFF));
		uint64_t L_43 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_43>>((int32_t)40)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 48) & 0xFF));
		uint64_t L_44 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_44>>((int32_t)48)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// Write((byte)((value >> 56) & 0xFF));
		uint64_t L_45 = ___value0;
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_45>>((int32_t)56)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(UnityEngine.Networking.NetworkInstanceId)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m327AAC971B7DA22E82661AD419E4D5EEC6CCAFBF (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  ___value0, const RuntimeMethod* method)
{
	{
		// WritePackedUInt32(value.Value);
		uint32_t L_0 = NetworkInstanceId_get_Value_m63FB00D0A8272D39B6C7F7C490A8190F0E95F67F_inline((NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615 *)(&___value0), /*hidden argument*/NULL);
		NetworkWriter_WritePackedUInt32_m99DCA40833B068CB958663A5B583BC8D2051B12F(__this, L_0, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(UnityEngine.Networking.NetworkSceneId)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_mE3F4BE69BA4D730DD3367504B87D53DA23043C5C (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, NetworkSceneId_t462EC62A23A1B7AF60637C48CD916A09BC493340  ___value0, const RuntimeMethod* method)
{
	{
		// WritePackedUInt32(value.Value);
		uint32_t L_0 = NetworkSceneId_get_Value_m917E56DBEDC97969F7AC83B42A1F53C21DC1A9A3_inline((NetworkSceneId_t462EC62A23A1B7AF60637C48CD916A09BC493340 *)(&___value0), /*hidden argument*/NULL);
		NetworkWriter_WritePackedUInt32_m99DCA40833B068CB958663A5B583BC8D2051B12F(__this, L_0, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.Char)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_mCD57D699B4DF091355AE33FFE862FF48359C1A33 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, Il2CppChar ___value0, const RuntimeMethod* method)
{
	{
		// m_Buffer.WriteByte((byte)value);
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_0 = __this->get_m_Buffer_1();
		Il2CppChar L_1 = ___value0;
		NullCheck(L_0);
		NetBuffer_WriteByte_m8F13CD997A9D3C72EEF9FB9B40E9F088FFC8FD20(L_0, (uint8_t)(((int32_t)((uint8_t)L_1))), /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.Byte)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, uint8_t ___value0, const RuntimeMethod* method)
{
	{
		// m_Buffer.WriteByte(value);
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_0 = __this->get_m_Buffer_1();
		uint8_t L_1 = ___value0;
		NullCheck(L_0);
		NetBuffer_WriteByte_m8F13CD997A9D3C72EEF9FB9B40E9F088FFC8FD20(L_0, L_1, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.SByte)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m36C45702758AAE4AB12D56ADE9F62372CB22669D (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, int8_t ___value0, const RuntimeMethod* method)
{
	{
		// m_Buffer.WriteByte((byte)value);
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_0 = __this->get_m_Buffer_1();
		int8_t L_1 = ___value0;
		NullCheck(L_0);
		NetBuffer_WriteByte_m8F13CD997A9D3C72EEF9FB9B40E9F088FFC8FD20(L_0, (uint8_t)(((int32_t)((uint8_t)L_1))), /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.Int16)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m9292C4A6802A8A84548CE8FC02CF90DB05720C2E (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, int16_t ___value0, const RuntimeMethod* method)
{
	{
		// m_Buffer.WriteByte2((byte)(value & 0xff), (byte)((value >> 8) & 0xff));
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_0 = __this->get_m_Buffer_1();
		int16_t L_1 = ___value0;
		int16_t L_2 = ___value0;
		NullCheck(L_0);
		NetBuffer_WriteByte2_m214A4267B67CD5BC4BF3F74EEC256774E2E5FB55(L_0, (uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)L_1&(int32_t)((int32_t)255)))))), (uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)((int32_t)((int32_t)L_2>>(int32_t)8))&(int32_t)((int32_t)255)))))), /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.UInt16)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_mA00075C21036F9B7D020332BA99CCB2687D1C835 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, uint16_t ___value0, const RuntimeMethod* method)
{
	{
		// m_Buffer.WriteByte2((byte)(value & 0xff), (byte)((value >> 8) & 0xff));
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_0 = __this->get_m_Buffer_1();
		uint16_t L_1 = ___value0;
		uint16_t L_2 = ___value0;
		NullCheck(L_0);
		NetBuffer_WriteByte2_m214A4267B67CD5BC4BF3F74EEC256774E2E5FB55(L_0, (uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)L_1&(int32_t)((int32_t)255)))))), (uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)((int32_t)((int32_t)L_2>>(int32_t)8))&(int32_t)((int32_t)255)))))), /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_mDDA79C3C63ED882F1895E9D71DB483284CBE9609 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, int32_t ___value0, const RuntimeMethod* method)
{
	{
		// m_Buffer.WriteByte4(
		//     (byte)(value & 0xff),
		//     (byte)((value >> 8) & 0xff),
		//     (byte)((value >> 16) & 0xff),
		//     (byte)((value >> 24) & 0xff));
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_0 = __this->get_m_Buffer_1();
		int32_t L_1 = ___value0;
		int32_t L_2 = ___value0;
		int32_t L_3 = ___value0;
		int32_t L_4 = ___value0;
		NullCheck(L_0);
		NetBuffer_WriteByte4_m9E62F33A8B124C327F187FAA1E1C275FA8CFB958(L_0, (uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)L_1&(int32_t)((int32_t)255)))))), (uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)((int32_t)((int32_t)L_2>>(int32_t)8))&(int32_t)((int32_t)255)))))), (uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)((int32_t)((int32_t)L_3>>(int32_t)((int32_t)16)))&(int32_t)((int32_t)255)))))), (uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)((int32_t)((int32_t)L_4>>(int32_t)((int32_t)24)))&(int32_t)((int32_t)255)))))), /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_mC08A0A307CE86A9CE57B009A5656C1419C824A8F (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, uint32_t ___value0, const RuntimeMethod* method)
{
	{
		// m_Buffer.WriteByte4(
		//     (byte)(value & 0xff),
		//     (byte)((value >> 8) & 0xff),
		//     (byte)((value >> 16) & 0xff),
		//     (byte)((value >> 24) & 0xff));
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_0 = __this->get_m_Buffer_1();
		uint32_t L_1 = ___value0;
		uint32_t L_2 = ___value0;
		uint32_t L_3 = ___value0;
		uint32_t L_4 = ___value0;
		NullCheck(L_0);
		NetBuffer_WriteByte4_m9E62F33A8B124C327F187FAA1E1C275FA8CFB958(L_0, (uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)L_1&(int32_t)((int32_t)255)))))), (uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)((int32_t)((uint32_t)L_2>>8))&(int32_t)((int32_t)255)))))), (uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)((int32_t)((uint32_t)L_3>>((int32_t)16)))&(int32_t)((int32_t)255)))))), (uint8_t)(((int32_t)((uint8_t)((int32_t)((int32_t)((int32_t)((uint32_t)L_4>>((int32_t)24)))&(int32_t)((int32_t)255)))))), /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.Int64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m9E0F14167C1B80B0553BC8326CD77C690E8425CD (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, int64_t ___value0, const RuntimeMethod* method)
{
	{
		// m_Buffer.WriteByte8(
		//     (byte)(value & 0xff),
		//     (byte)((value >> 8) & 0xff),
		//     (byte)((value >> 16) & 0xff),
		//     (byte)((value >> 24) & 0xff),
		//     (byte)((value >> 32) & 0xff),
		//     (byte)((value >> 40) & 0xff),
		//     (byte)((value >> 48) & 0xff),
		//     (byte)((value >> 56) & 0xff));
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_0 = __this->get_m_Buffer_1();
		int64_t L_1 = ___value0;
		int64_t L_2 = ___value0;
		int64_t L_3 = ___value0;
		int64_t L_4 = ___value0;
		int64_t L_5 = ___value0;
		int64_t L_6 = ___value0;
		int64_t L_7 = ___value0;
		int64_t L_8 = ___value0;
		NullCheck(L_0);
		NetBuffer_WriteByte8_m3F07A209557DFD1C4956AD51776C5695B4967012(L_0, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)L_1&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((int64_t)L_2>>(int32_t)8))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((int64_t)L_3>>(int32_t)((int32_t)16)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((int64_t)L_4>>(int32_t)((int32_t)24)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((int64_t)L_5>>(int32_t)((int32_t)32)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((int64_t)L_6>>(int32_t)((int32_t)40)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((int64_t)L_7>>(int32_t)((int32_t)48)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((int64_t)L_8>>(int32_t)((int32_t)56)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.UInt64)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_mD1A7B0686E93732F4086FE17AAE75596E55F5946 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, uint64_t ___value0, const RuntimeMethod* method)
{
	{
		// m_Buffer.WriteByte8(
		//     (byte)(value & 0xff),
		//     (byte)((value >> 8) & 0xff),
		//     (byte)((value >> 16) & 0xff),
		//     (byte)((value >> 24) & 0xff),
		//     (byte)((value >> 32) & 0xff),
		//     (byte)((value >> 40) & 0xff),
		//     (byte)((value >> 48) & 0xff),
		//     (byte)((value >> 56) & 0xff));
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_0 = __this->get_m_Buffer_1();
		uint64_t L_1 = ___value0;
		uint64_t L_2 = ___value0;
		uint64_t L_3 = ___value0;
		uint64_t L_4 = ___value0;
		uint64_t L_5 = ___value0;
		uint64_t L_6 = ___value0;
		uint64_t L_7 = ___value0;
		uint64_t L_8 = ___value0;
		NullCheck(L_0);
		NetBuffer_WriteByte8_m3F07A209557DFD1C4956AD51776C5695B4967012(L_0, (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)L_1&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_2>>8))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_3>>((int32_t)16)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_4>>((int32_t)24)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_5>>((int32_t)32)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_6>>((int32_t)40)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_7>>((int32_t)48)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), (uint8_t)(((int32_t)((uint8_t)((int64_t)((int64_t)((int64_t)((uint64_t)L_8>>((int32_t)56)))&(int64_t)(((int64_t)((int64_t)((int32_t)255))))))))), /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, float ___value0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// s_FloatConverter.floatValue = value;
		float L_0 = ___value0;
		(((NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_StaticFields*)il2cpp_codegen_static_fields_for(NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_il2cpp_TypeInfo_var))->get_address_of_s_FloatConverter_4())->set_floatValue_0(L_0);
		// Write(s_FloatConverter.intValue);
		uint32_t L_1 = (((NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_StaticFields*)il2cpp_codegen_static_fields_for(NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_il2cpp_TypeInfo_var))->get_address_of_s_FloatConverter_4())->get_intValue_1();
		NetworkWriter_Write_mC08A0A307CE86A9CE57B009A5656C1419C824A8F(__this, L_1, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.Double)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_mD3CD61845FC639033FBD63A6AC70260B113A0C7E (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, double ___value0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkWriter_Write_mD3CD61845FC639033FBD63A6AC70260B113A0C7E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// s_FloatConverter.doubleValue = value;
		double L_0 = ___value0;
		(((NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_StaticFields*)il2cpp_codegen_static_fields_for(NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_il2cpp_TypeInfo_var))->get_address_of_s_FloatConverter_4())->set_doubleValue_2(L_0);
		// Write(s_FloatConverter.longValue);
		uint64_t L_1 = (((NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_StaticFields*)il2cpp_codegen_static_fields_for(NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_il2cpp_TypeInfo_var))->get_address_of_s_FloatConverter_4())->get_longValue_3();
		NetworkWriter_Write_mD1A7B0686E93732F4086FE17AAE75596E55F5946(__this, L_1, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.Decimal)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_mA29B40D65C79F09452E44ACE95D7882D48BC2631 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  ___value0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkWriter_Write_mA29B40D65C79F09452E44ACE95D7882D48BC2631_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* V_0 = NULL;
	{
		// Int32[] bits = decimal.GetBits(value);
		Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8  L_0 = ___value0;
		IL2CPP_RUNTIME_CLASS_INIT(Decimal_t44EE9DA309A1BF848308DE4DDFC070CAE6D95EE8_il2cpp_TypeInfo_var);
		Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* L_1 = Decimal_GetBits_m581C2DB9823AC9CD84817738A740E8A7D39609BF(L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		// Write(bits[0]);
		Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* L_2 = V_0;
		NullCheck(L_2);
		int32_t L_3 = 0;
		int32_t L_4 = (L_2)->GetAt(static_cast<il2cpp_array_size_t>(L_3));
		NetworkWriter_Write_mDDA79C3C63ED882F1895E9D71DB483284CBE9609(__this, L_4, /*hidden argument*/NULL);
		// Write(bits[1]);
		Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* L_5 = V_0;
		NullCheck(L_5);
		int32_t L_6 = 1;
		int32_t L_7 = (L_5)->GetAt(static_cast<il2cpp_array_size_t>(L_6));
		NetworkWriter_Write_mDDA79C3C63ED882F1895E9D71DB483284CBE9609(__this, L_7, /*hidden argument*/NULL);
		// Write(bits[2]);
		Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* L_8 = V_0;
		NullCheck(L_8);
		int32_t L_9 = 2;
		int32_t L_10 = (L_8)->GetAt(static_cast<il2cpp_array_size_t>(L_9));
		NetworkWriter_Write_mDDA79C3C63ED882F1895E9D71DB483284CBE9609(__this, L_10, /*hidden argument*/NULL);
		// Write(bits[3]);
		Int32U5BU5D_t2B9E4FDDDB9F0A00EC0AC631BA2DA915EB1ECF83* L_11 = V_0;
		NullCheck(L_11);
		int32_t L_12 = 3;
		int32_t L_13 = (L_11)->GetAt(static_cast<il2cpp_array_size_t>(L_12));
		NetworkWriter_Write_mDDA79C3C63ED882F1895E9D71DB483284CBE9609(__this, L_13, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m856F6DD1E132E2C68BA9D7D36A5ED5EAA1D108F4 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, String_t* ___value0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkWriter_Write_m856F6DD1E132E2C68BA9D7D36A5ED5EAA1D108F4_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	int32_t V_1 = 0;
	{
		// if (value == null)
		String_t* L_0 = ___value0;
		if (L_0)
		{
			goto IL_0011;
		}
	}
	{
		// m_Buffer.WriteByte2(0, 0);
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_1 = __this->get_m_Buffer_1();
		NullCheck(L_1);
		NetBuffer_WriteByte2_m214A4267B67CD5BC4BF3F74EEC256774E2E5FB55(L_1, (uint8_t)0, (uint8_t)0, /*hidden argument*/NULL);
		// return;
		return;
	}

IL_0011:
	{
		// int len = s_Encoding.GetByteCount(value);
		Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * L_2 = ((NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_StaticFields*)il2cpp_codegen_static_fields_for(NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_il2cpp_TypeInfo_var))->get_s_Encoding_2();
		String_t* L_3 = ___value0;
		NullCheck(L_2);
		int32_t L_4 = VirtFuncInvoker1< int32_t, String_t* >::Invoke(18 /* System.Int32 System.Text.Encoding::GetByteCount(System.String) */, L_2, L_3);
		V_0 = L_4;
		// if (len >= k_MaxStringLength)
		int32_t L_5 = V_0;
		if ((((int32_t)L_5) < ((int32_t)((int32_t)32768))))
		{
			goto IL_0040;
		}
	}
	{
		// throw new IndexOutOfRangeException("Serialize(string) too long: " + value.Length);
		String_t* L_6 = ___value0;
		NullCheck(L_6);
		int32_t L_7 = String_get_Length_mD48C8A16A5CF1914F330DCE82D9BE15C3BEDD018_inline(L_6, /*hidden argument*/NULL);
		int32_t L_8 = L_7;
		RuntimeObject * L_9 = Box(Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var, &L_8);
		String_t* L_10 = String_Concat_mBB19C73816BDD1C3519F248E1ADC8E11A6FDB495(_stringLiteral5902A7A78A53DCB1D1016E9500F2A3343AA637C9, L_9, /*hidden argument*/NULL);
		IndexOutOfRangeException_tEC7665FC66525AB6A6916A7EB505E5591683F0CF * L_11 = (IndexOutOfRangeException_tEC7665FC66525AB6A6916A7EB505E5591683F0CF *)il2cpp_codegen_object_new(IndexOutOfRangeException_tEC7665FC66525AB6A6916A7EB505E5591683F0CF_il2cpp_TypeInfo_var);
		IndexOutOfRangeException__ctor_mCCE2EFF47A0ACB4B2636F63140F94FCEA71A9BCA(L_11, L_10, /*hidden argument*/NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_11, NULL, NetworkWriter_Write_m856F6DD1E132E2C68BA9D7D36A5ED5EAA1D108F4_RuntimeMethod_var);
	}

IL_0040:
	{
		// Write((ushort)(len));
		int32_t L_12 = V_0;
		NetworkWriter_Write_mA00075C21036F9B7D020332BA99CCB2687D1C835(__this, (uint16_t)(((int32_t)((uint16_t)L_12))), /*hidden argument*/NULL);
		// int numBytes = s_Encoding.GetBytes(value, 0, value.Length, s_StringWriteBuffer, 0);
		Encoding_t7837A3C0F55EAE0E3959A53C6D6E88B113ED78A4 * L_13 = ((NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_StaticFields*)il2cpp_codegen_static_fields_for(NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_il2cpp_TypeInfo_var))->get_s_Encoding_2();
		String_t* L_14 = ___value0;
		String_t* L_15 = ___value0;
		NullCheck(L_15);
		int32_t L_16 = String_get_Length_mD48C8A16A5CF1914F330DCE82D9BE15C3BEDD018_inline(L_15, /*hidden argument*/NULL);
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_17 = ((NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_StaticFields*)il2cpp_codegen_static_fields_for(NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_il2cpp_TypeInfo_var))->get_s_StringWriteBuffer_3();
		NullCheck(L_13);
		int32_t L_18 = VirtFuncInvoker5< int32_t, String_t*, int32_t, int32_t, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821*, int32_t >::Invoke(26 /* System.Int32 System.Text.Encoding::GetBytes(System.String,System.Int32,System.Int32,System.Byte[],System.Int32) */, L_13, L_14, 0, L_16, L_17, 0);
		V_1 = L_18;
		// m_Buffer.WriteBytes(s_StringWriteBuffer, (ushort)numBytes);
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_19 = __this->get_m_Buffer_1();
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_20 = ((NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_StaticFields*)il2cpp_codegen_static_fields_for(NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030_il2cpp_TypeInfo_var))->get_s_StringWriteBuffer_3();
		int32_t L_21 = V_1;
		NullCheck(L_19);
		NetBuffer_WriteBytes_m899E9F103BFA5827EFA91A6E3B17F0E12B78F94F(L_19, L_20, (uint16_t)(((int32_t)((uint16_t)L_21))), /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m68E1030824D76CD6B46468FDC290B55C11D944C5 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, bool ___value0, const RuntimeMethod* method)
{
	{
		// if (value)
		bool L_0 = ___value0;
		if (!L_0)
		{
			goto IL_0010;
		}
	}
	{
		// m_Buffer.WriteByte(1);
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_1 = __this->get_m_Buffer_1();
		NullCheck(L_1);
		NetBuffer_WriteByte_m8F13CD997A9D3C72EEF9FB9B40E9F088FFC8FD20(L_1, (uint8_t)1, /*hidden argument*/NULL);
		return;
	}

IL_0010:
	{
		// m_Buffer.WriteByte(0);
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_2 = __this->get_m_Buffer_1();
		NullCheck(L_2);
		NetBuffer_WriteByte_m8F13CD997A9D3C72EEF9FB9B40E9F088FFC8FD20(L_2, (uint8_t)0, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.Byte[],System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_mAEB6BA4ED3581931DF47C9C32756693014CEB796 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___buffer0, int32_t ___count1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkWriter_Write_mAEB6BA4ED3581931DF47C9C32756693014CEB796_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (count > UInt16.MaxValue)
		int32_t L_0 = ___count1;
		if ((((int32_t)L_0) <= ((int32_t)((int32_t)65535))))
		{
			goto IL_002a;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("NetworkWriter Write: buffer is too large (" + count + ") bytes. The maximum buffer size is 64K bytes."); }
		IL2CPP_RUNTIME_CLASS_INIT(LogFilter_t5202A297E770086F7954B8D6703BAC03C22654ED_il2cpp_TypeInfo_var);
		bool L_1 = LogFilter_get_logError_mD404500EEB2968A3CF190DB1EB6CA9A26135A21F(/*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_0029;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("NetworkWriter Write: buffer is too large (" + count + ") bytes. The maximum buffer size is 64K bytes."); }
		int32_t L_2 = ___count1;
		int32_t L_3 = L_2;
		RuntimeObject * L_4 = Box(Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var, &L_3);
		String_t* L_5 = String_Concat_m2E1F71C491D2429CC80A28745488FEA947BB7AAC(_stringLiteralD19C1CDB8E147A59F990BE7BC80967AB61A50F80, L_4, _stringLiteral31DDD762A7C4C86F022297D9A759C2F1C7D04EAC, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_LogError_m3BCF9B78263152261565DCA9DB7D55F0C391ED29(L_5, /*hidden argument*/NULL);
	}

IL_0029:
	{
		// return;
		return;
	}

IL_002a:
	{
		// m_Buffer.WriteBytes(buffer, (UInt16)count);
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_6 = __this->get_m_Buffer_1();
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_7 = ___buffer0;
		int32_t L_8 = ___count1;
		NullCheck(L_6);
		NetBuffer_WriteBytes_m899E9F103BFA5827EFA91A6E3B17F0E12B78F94F(L_6, L_7, (uint16_t)(((int32_t)((uint16_t)L_8))), /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(System.Byte[],System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m32B91CD67215B848269156BE15AC04FB5834F0C6 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___buffer0, int32_t ___offset1, int32_t ___count2, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkWriter_Write_m32B91CD67215B848269156BE15AC04FB5834F0C6_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (count > UInt16.MaxValue)
		int32_t L_0 = ___count2;
		if ((((int32_t)L_0) <= ((int32_t)((int32_t)65535))))
		{
			goto IL_002a;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("NetworkWriter Write: buffer is too large (" + count + ") bytes. The maximum buffer size is 64K bytes."); }
		IL2CPP_RUNTIME_CLASS_INIT(LogFilter_t5202A297E770086F7954B8D6703BAC03C22654ED_il2cpp_TypeInfo_var);
		bool L_1 = LogFilter_get_logError_mD404500EEB2968A3CF190DB1EB6CA9A26135A21F(/*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_0029;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("NetworkWriter Write: buffer is too large (" + count + ") bytes. The maximum buffer size is 64K bytes."); }
		int32_t L_2 = ___count2;
		int32_t L_3 = L_2;
		RuntimeObject * L_4 = Box(Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var, &L_3);
		String_t* L_5 = String_Concat_m2E1F71C491D2429CC80A28745488FEA947BB7AAC(_stringLiteralD19C1CDB8E147A59F990BE7BC80967AB61A50F80, L_4, _stringLiteral31DDD762A7C4C86F022297D9A759C2F1C7D04EAC, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_LogError_m3BCF9B78263152261565DCA9DB7D55F0C391ED29(L_5, /*hidden argument*/NULL);
	}

IL_0029:
	{
		// return;
		return;
	}

IL_002a:
	{
		// m_Buffer.WriteBytesAtOffset(buffer, (ushort)offset, (ushort)count);
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_6 = __this->get_m_Buffer_1();
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_7 = ___buffer0;
		int32_t L_8 = ___offset1;
		int32_t L_9 = ___count2;
		NullCheck(L_6);
		NetBuffer_WriteBytesAtOffset_m812B345F53E3717A654F972244E65B535651676F(L_6, L_7, (uint16_t)(((int32_t)((uint16_t)L_8))), (uint16_t)(((int32_t)((uint16_t)L_9))), /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::WriteBytesAndSize(System.Byte[],System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_WriteBytesAndSize_mC601A1BBC88D92522B7997698041ECEDF895A71E (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___buffer0, int32_t ___count1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkWriter_WriteBytesAndSize_mC601A1BBC88D92522B7997698041ECEDF895A71E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (buffer == null || count == 0)
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_0 = ___buffer0;
		if (!L_0)
		{
			goto IL_0006;
		}
	}
	{
		int32_t L_1 = ___count1;
		if (L_1)
		{
			goto IL_000e;
		}
	}

IL_0006:
	{
		// Write((UInt16)0);
		NetworkWriter_Write_mA00075C21036F9B7D020332BA99CCB2687D1C835(__this, (uint16_t)0, /*hidden argument*/NULL);
		// return;
		return;
	}

IL_000e:
	{
		// if (count > UInt16.MaxValue)
		int32_t L_2 = ___count1;
		if ((((int32_t)L_2) <= ((int32_t)((int32_t)65535))))
		{
			goto IL_0038;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("NetworkWriter WriteBytesAndSize: buffer is too large (" + count + ") bytes. The maximum buffer size is 64K bytes."); }
		IL2CPP_RUNTIME_CLASS_INIT(LogFilter_t5202A297E770086F7954B8D6703BAC03C22654ED_il2cpp_TypeInfo_var);
		bool L_3 = LogFilter_get_logError_mD404500EEB2968A3CF190DB1EB6CA9A26135A21F(/*hidden argument*/NULL);
		if (!L_3)
		{
			goto IL_0037;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("NetworkWriter WriteBytesAndSize: buffer is too large (" + count + ") bytes. The maximum buffer size is 64K bytes."); }
		int32_t L_4 = ___count1;
		int32_t L_5 = L_4;
		RuntimeObject * L_6 = Box(Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var, &L_5);
		String_t* L_7 = String_Concat_m2E1F71C491D2429CC80A28745488FEA947BB7AAC(_stringLiteral4C3023713E64D2BEA428CD3A86A6DE84754220DE, L_6, _stringLiteral31DDD762A7C4C86F022297D9A759C2F1C7D04EAC, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_LogError_m3BCF9B78263152261565DCA9DB7D55F0C391ED29(L_7, /*hidden argument*/NULL);
	}

IL_0037:
	{
		// return;
		return;
	}

IL_0038:
	{
		// Write((UInt16)count);
		int32_t L_8 = ___count1;
		NetworkWriter_Write_mA00075C21036F9B7D020332BA99CCB2687D1C835(__this, (uint16_t)(((int32_t)((uint16_t)L_8))), /*hidden argument*/NULL);
		// m_Buffer.WriteBytes(buffer, (UInt16)count);
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_9 = __this->get_m_Buffer_1();
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_10 = ___buffer0;
		int32_t L_11 = ___count1;
		NullCheck(L_9);
		NetBuffer_WriteBytes_m899E9F103BFA5827EFA91A6E3B17F0E12B78F94F(L_9, L_10, (uint16_t)(((int32_t)((uint16_t)L_11))), /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::WriteBytesFull(System.Byte[])
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_WriteBytesFull_m99C689920FA25E82972668235E7C776B78D1217E (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___buffer0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkWriter_WriteBytesFull_m99C689920FA25E82972668235E7C776B78D1217E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (buffer == null)
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_0 = ___buffer0;
		if (L_0)
		{
			goto IL_000b;
		}
	}
	{
		// Write((UInt16)0);
		NetworkWriter_Write_mA00075C21036F9B7D020332BA99CCB2687D1C835(__this, (uint16_t)0, /*hidden argument*/NULL);
		// return;
		return;
	}

IL_000b:
	{
		// if (buffer.Length > UInt16.MaxValue)
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_1 = ___buffer0;
		NullCheck(L_1);
		if ((((int32_t)(((int32_t)((int32_t)(((RuntimeArray*)L_1)->max_length))))) <= ((int32_t)((int32_t)65535))))
		{
			goto IL_0039;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("NetworkWriter WriteBytes: buffer is too large (" + buffer.Length + ") bytes. The maximum buffer size is 64K bytes."); }
		IL2CPP_RUNTIME_CLASS_INIT(LogFilter_t5202A297E770086F7954B8D6703BAC03C22654ED_il2cpp_TypeInfo_var);
		bool L_2 = LogFilter_get_logError_mD404500EEB2968A3CF190DB1EB6CA9A26135A21F(/*hidden argument*/NULL);
		if (!L_2)
		{
			goto IL_0038;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("NetworkWriter WriteBytes: buffer is too large (" + buffer.Length + ") bytes. The maximum buffer size is 64K bytes."); }
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_3 = ___buffer0;
		NullCheck(L_3);
		int32_t L_4 = (((int32_t)((int32_t)(((RuntimeArray*)L_3)->max_length))));
		RuntimeObject * L_5 = Box(Int32_t585191389E07734F19F3156FF88FB3EF4800D102_il2cpp_TypeInfo_var, &L_4);
		String_t* L_6 = String_Concat_m2E1F71C491D2429CC80A28745488FEA947BB7AAC(_stringLiteralDC5AFF0CACCF4051C4542D7C075A52E0451162CF, L_5, _stringLiteral31DDD762A7C4C86F022297D9A759C2F1C7D04EAC, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_LogError_m3BCF9B78263152261565DCA9DB7D55F0C391ED29(L_6, /*hidden argument*/NULL);
	}

IL_0038:
	{
		// return;
		return;
	}

IL_0039:
	{
		// Write((UInt16)buffer.Length);
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_7 = ___buffer0;
		NullCheck(L_7);
		NetworkWriter_Write_mA00075C21036F9B7D020332BA99CCB2687D1C835(__this, (uint16_t)(((int32_t)((uint16_t)(((int32_t)((int32_t)(((RuntimeArray*)L_7)->max_length))))))), /*hidden argument*/NULL);
		// m_Buffer.WriteBytes(buffer, (UInt16)buffer.Length);
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_8 = __this->get_m_Buffer_1();
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_9 = ___buffer0;
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_10 = ___buffer0;
		NullCheck(L_10);
		NullCheck(L_8);
		NetBuffer_WriteBytes_m899E9F103BFA5827EFA91A6E3B17F0E12B78F94F(L_8, L_9, (uint16_t)(((int32_t)((uint16_t)(((int32_t)((int32_t)(((RuntimeArray*)L_10)->max_length))))))), /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(UnityEngine.Vector2)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m1F3C70C2AA1256C3C9991EC1FFC9D6AFBB83EF2F (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  ___value0, const RuntimeMethod* method)
{
	{
		// Write(value.x);
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_0 = ___value0;
		float L_1 = L_0.get_x_0();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_1, /*hidden argument*/NULL);
		// Write(value.y);
		Vector2_tA85D2DD88578276CA8A8796756458277E72D073D  L_2 = ___value0;
		float L_3 = L_2.get_y_1();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_3, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(UnityEngine.Vector3)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m11CA4683BE86268158E1F949E620C1BF9D69884F (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___value0, const RuntimeMethod* method)
{
	{
		// Write(value.x);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_0 = ___value0;
		float L_1 = L_0.get_x_2();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_1, /*hidden argument*/NULL);
		// Write(value.y);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_2 = ___value0;
		float L_3 = L_2.get_y_3();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_3, /*hidden argument*/NULL);
		// Write(value.z);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_4 = ___value0;
		float L_5 = L_4.get_z_4();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_5, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(UnityEngine.Vector4)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m4AC47A1A1FFF41D6E319D5CA897362F1B99B79AC (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E  ___value0, const RuntimeMethod* method)
{
	{
		// Write(value.x);
		Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E  L_0 = ___value0;
		float L_1 = L_0.get_x_1();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_1, /*hidden argument*/NULL);
		// Write(value.y);
		Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E  L_2 = ___value0;
		float L_3 = L_2.get_y_2();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_3, /*hidden argument*/NULL);
		// Write(value.z);
		Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E  L_4 = ___value0;
		float L_5 = L_4.get_z_3();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_5, /*hidden argument*/NULL);
		// Write(value.w);
		Vector4_tD148D6428C3F8FF6CD998F82090113C2B490B76E  L_6 = ___value0;
		float L_7 = L_6.get_w_4();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_7, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(UnityEngine.Color)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m1F808A8AA9D566BB3C96C3A2F729451C97ADFC8B (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  ___value0, const RuntimeMethod* method)
{
	{
		// Write(value.r);
		Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  L_0 = ___value0;
		float L_1 = L_0.get_r_0();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_1, /*hidden argument*/NULL);
		// Write(value.g);
		Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  L_2 = ___value0;
		float L_3 = L_2.get_g_1();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_3, /*hidden argument*/NULL);
		// Write(value.b);
		Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  L_4 = ___value0;
		float L_5 = L_4.get_b_2();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_5, /*hidden argument*/NULL);
		// Write(value.a);
		Color_t119BCA590009762C7223FDD3AF9706653AC84ED2  L_6 = ___value0;
		float L_7 = L_6.get_a_3();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_7, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(UnityEngine.Color32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m6025C4AF4E2EEBFF381B2358741CCC80D83A7D7D (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, Color32_t23ABC4AE0E0BDFD2E22EE1FA0DA3904FFE5F6E23  ___value0, const RuntimeMethod* method)
{
	{
		// Write(value.r);
		Color32_t23ABC4AE0E0BDFD2E22EE1FA0DA3904FFE5F6E23  L_0 = ___value0;
		uint8_t L_1 = L_0.get_r_1();
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, L_1, /*hidden argument*/NULL);
		// Write(value.g);
		Color32_t23ABC4AE0E0BDFD2E22EE1FA0DA3904FFE5F6E23  L_2 = ___value0;
		uint8_t L_3 = L_2.get_g_2();
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, L_3, /*hidden argument*/NULL);
		// Write(value.b);
		Color32_t23ABC4AE0E0BDFD2E22EE1FA0DA3904FFE5F6E23  L_4 = ___value0;
		uint8_t L_5 = L_4.get_b_3();
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, L_5, /*hidden argument*/NULL);
		// Write(value.a);
		Color32_t23ABC4AE0E0BDFD2E22EE1FA0DA3904FFE5F6E23  L_6 = ___value0;
		uint8_t L_7 = L_6.get_a_4();
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, L_7, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(UnityEngine.Quaternion)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m4D770502835706DC3F5F134F2D0E5039AF435566 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  ___value0, const RuntimeMethod* method)
{
	{
		// Write(value.x);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_0 = ___value0;
		float L_1 = L_0.get_x_0();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_1, /*hidden argument*/NULL);
		// Write(value.y);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_2 = ___value0;
		float L_3 = L_2.get_y_1();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_3, /*hidden argument*/NULL);
		// Write(value.z);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_4 = ___value0;
		float L_5 = L_4.get_z_2();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_5, /*hidden argument*/NULL);
		// Write(value.w);
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_6 = ___value0;
		float L_7 = L_6.get_w_3();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_7, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(UnityEngine.Rect)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m6E09EC8B1248172A79E236AB5D319DD497E68199 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, Rect_t35B976DE901B5423C11705E156938EA27AB402CE  ___value0, const RuntimeMethod* method)
{
	{
		// Write(value.xMin);
		float L_0 = Rect_get_xMin_mFDFA74F66595FD2B8CE360183D1A92B575F0A76E((Rect_t35B976DE901B5423C11705E156938EA27AB402CE *)(&___value0), /*hidden argument*/NULL);
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_0, /*hidden argument*/NULL);
		// Write(value.yMin);
		float L_1 = Rect_get_yMin_m31EDC3262BE39D2F6464B15397F882237E6158C3((Rect_t35B976DE901B5423C11705E156938EA27AB402CE *)(&___value0), /*hidden argument*/NULL);
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_1, /*hidden argument*/NULL);
		// Write(value.width);
		float L_2 = Rect_get_width_m54FF69FC2C086E2DC349ED091FD0D6576BFB1484((Rect_t35B976DE901B5423C11705E156938EA27AB402CE *)(&___value0), /*hidden argument*/NULL);
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_2, /*hidden argument*/NULL);
		// Write(value.height);
		float L_3 = Rect_get_height_m088C36990E0A255C5D7DCE36575DCE23ABB364B5((Rect_t35B976DE901B5423C11705E156938EA27AB402CE *)(&___value0), /*hidden argument*/NULL);
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_3, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(UnityEngine.Plane)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m127418F7E3D2E15D6E91D87C0F8DEB1EEEA5D1C3 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, Plane_t0903921088DEEDE1BCDEA5BF279EDBCFC9679AED  ___value0, const RuntimeMethod* method)
{
	{
		// Write(value.normal);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_0 = Plane_get_normal_m203D43F51C449990214D04F332E8261295162E84((Plane_t0903921088DEEDE1BCDEA5BF279EDBCFC9679AED *)(&___value0), /*hidden argument*/NULL);
		NetworkWriter_Write_m11CA4683BE86268158E1F949E620C1BF9D69884F(__this, L_0, /*hidden argument*/NULL);
		// Write(value.distance);
		float L_1 = Plane_get_distance_m5358B80C35E1E295C0133E7DC6449BB09C456DEE((Plane_t0903921088DEEDE1BCDEA5BF279EDBCFC9679AED *)(&___value0), /*hidden argument*/NULL);
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_1, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(UnityEngine.Ray)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_mC7410B2E86D87629F6D49E6B7EDF2BFAB2B9D4E6 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, Ray_tE2163D4CB3E6B267E29F8ABE41684490E4A614B2  ___value0, const RuntimeMethod* method)
{
	{
		// Write(value.direction);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_0 = Ray_get_direction_m9E6468CD87844B437FC4B93491E63D388322F76E((Ray_tE2163D4CB3E6B267E29F8ABE41684490E4A614B2 *)(&___value0), /*hidden argument*/NULL);
		NetworkWriter_Write_m11CA4683BE86268158E1F949E620C1BF9D69884F(__this, L_0, /*hidden argument*/NULL);
		// Write(value.origin);
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_1 = Ray_get_origin_m3773CA7B1E2F26F6F1447652B485D86C0BEC5187((Ray_tE2163D4CB3E6B267E29F8ABE41684490E4A614B2 *)(&___value0), /*hidden argument*/NULL);
		NetworkWriter_Write_m11CA4683BE86268158E1F949E620C1BF9D69884F(__this, L_1, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(UnityEngine.Matrix4x4)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_mB5A3EBB1E4720B64FB31DFD3CF8C7851D4919633 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  ___value0, const RuntimeMethod* method)
{
	{
		// Write(value.m00);
		Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  L_0 = ___value0;
		float L_1 = L_0.get_m00_0();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_1, /*hidden argument*/NULL);
		// Write(value.m01);
		Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  L_2 = ___value0;
		float L_3 = L_2.get_m01_4();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_3, /*hidden argument*/NULL);
		// Write(value.m02);
		Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  L_4 = ___value0;
		float L_5 = L_4.get_m02_8();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_5, /*hidden argument*/NULL);
		// Write(value.m03);
		Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  L_6 = ___value0;
		float L_7 = L_6.get_m03_12();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_7, /*hidden argument*/NULL);
		// Write(value.m10);
		Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  L_8 = ___value0;
		float L_9 = L_8.get_m10_1();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_9, /*hidden argument*/NULL);
		// Write(value.m11);
		Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  L_10 = ___value0;
		float L_11 = L_10.get_m11_5();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_11, /*hidden argument*/NULL);
		// Write(value.m12);
		Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  L_12 = ___value0;
		float L_13 = L_12.get_m12_9();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_13, /*hidden argument*/NULL);
		// Write(value.m13);
		Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  L_14 = ___value0;
		float L_15 = L_14.get_m13_13();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_15, /*hidden argument*/NULL);
		// Write(value.m20);
		Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  L_16 = ___value0;
		float L_17 = L_16.get_m20_2();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_17, /*hidden argument*/NULL);
		// Write(value.m21);
		Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  L_18 = ___value0;
		float L_19 = L_18.get_m21_6();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_19, /*hidden argument*/NULL);
		// Write(value.m22);
		Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  L_20 = ___value0;
		float L_21 = L_20.get_m22_10();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_21, /*hidden argument*/NULL);
		// Write(value.m23);
		Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  L_22 = ___value0;
		float L_23 = L_22.get_m23_14();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_23, /*hidden argument*/NULL);
		// Write(value.m30);
		Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  L_24 = ___value0;
		float L_25 = L_24.get_m30_3();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_25, /*hidden argument*/NULL);
		// Write(value.m31);
		Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  L_26 = ___value0;
		float L_27 = L_26.get_m31_7();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_27, /*hidden argument*/NULL);
		// Write(value.m32);
		Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  L_28 = ___value0;
		float L_29 = L_28.get_m32_11();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_29, /*hidden argument*/NULL);
		// Write(value.m33);
		Matrix4x4_t6BF60F70C9169DF14C9D2577672A44224B236ECA  L_30 = ___value0;
		float L_31 = L_30.get_m33_15();
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(__this, L_31, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(UnityEngine.Networking.NetworkHash128)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_mDDF80E4C0BCB2F9CA7028B16352072628FE5D909 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  ___value0, const RuntimeMethod* method)
{
	{
		// Write(value.i0);
		NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  L_0 = ___value0;
		uint8_t L_1 = L_0.get_i0_0();
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, L_1, /*hidden argument*/NULL);
		// Write(value.i1);
		NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  L_2 = ___value0;
		uint8_t L_3 = L_2.get_i1_1();
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, L_3, /*hidden argument*/NULL);
		// Write(value.i2);
		NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  L_4 = ___value0;
		uint8_t L_5 = L_4.get_i2_2();
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, L_5, /*hidden argument*/NULL);
		// Write(value.i3);
		NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  L_6 = ___value0;
		uint8_t L_7 = L_6.get_i3_3();
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, L_7, /*hidden argument*/NULL);
		// Write(value.i4);
		NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  L_8 = ___value0;
		uint8_t L_9 = L_8.get_i4_4();
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, L_9, /*hidden argument*/NULL);
		// Write(value.i5);
		NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  L_10 = ___value0;
		uint8_t L_11 = L_10.get_i5_5();
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, L_11, /*hidden argument*/NULL);
		// Write(value.i6);
		NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  L_12 = ___value0;
		uint8_t L_13 = L_12.get_i6_6();
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, L_13, /*hidden argument*/NULL);
		// Write(value.i7);
		NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  L_14 = ___value0;
		uint8_t L_15 = L_14.get_i7_7();
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, L_15, /*hidden argument*/NULL);
		// Write(value.i8);
		NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  L_16 = ___value0;
		uint8_t L_17 = L_16.get_i8_8();
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, L_17, /*hidden argument*/NULL);
		// Write(value.i9);
		NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  L_18 = ___value0;
		uint8_t L_19 = L_18.get_i9_9();
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, L_19, /*hidden argument*/NULL);
		// Write(value.i10);
		NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  L_20 = ___value0;
		uint8_t L_21 = L_20.get_i10_10();
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, L_21, /*hidden argument*/NULL);
		// Write(value.i11);
		NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  L_22 = ___value0;
		uint8_t L_23 = L_22.get_i11_11();
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, L_23, /*hidden argument*/NULL);
		// Write(value.i12);
		NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  L_24 = ___value0;
		uint8_t L_25 = L_24.get_i12_12();
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, L_25, /*hidden argument*/NULL);
		// Write(value.i13);
		NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  L_26 = ___value0;
		uint8_t L_27 = L_26.get_i13_13();
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, L_27, /*hidden argument*/NULL);
		// Write(value.i14);
		NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  L_28 = ___value0;
		uint8_t L_29 = L_28.get_i14_14();
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, L_29, /*hidden argument*/NULL);
		// Write(value.i15);
		NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  L_30 = ___value0;
		uint8_t L_31 = L_30.get_i15_15();
		NetworkWriter_Write_m99D6ACBCB1E5C94FD09AE9F50CFC993B18DEA183(__this, L_31, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(UnityEngine.Networking.NetworkIdentity)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m909A09F9662D8CB46D80D6155630A321EFED99E1 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * ___value0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkWriter_Write_m909A09F9662D8CB46D80D6155630A321EFED99E1_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (value == null)
		NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * L_0 = ___value0;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_1 = Object_op_Equality_mBC2401774F3BE33E8CF6F0A8148E66C95D6CFF1C(L_0, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_0011;
		}
	}
	{
		// WritePackedUInt32(0);
		NetworkWriter_WritePackedUInt32_m99DCA40833B068CB958663A5B583BC8D2051B12F(__this, 0, /*hidden argument*/NULL);
		// return;
		return;
	}

IL_0011:
	{
		// Write(value.netId);
		NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * L_2 = ___value0;
		NullCheck(L_2);
		NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  L_3 = NetworkIdentity_get_netId_m22EB7CD04E2633FFAF99093749F79816B2BC9F28_inline(L_2, /*hidden argument*/NULL);
		NetworkWriter_Write_m327AAC971B7DA22E82661AD419E4D5EEC6CCAFBF(__this, L_3, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(UnityEngine.Transform)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m77E79DF810798F97CB4B0193DEDE1D7EAED9E176 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * ___value0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkWriter_Write_m77E79DF810798F97CB4B0193DEDE1D7EAED9E176_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * V_0 = NULL;
	{
		// if (value == null || value.gameObject == null)
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_0 = ___value0;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_1 = Object_op_Equality_mBC2401774F3BE33E8CF6F0A8148E66C95D6CFF1C(L_0, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (L_1)
		{
			goto IL_0017;
		}
	}
	{
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_2 = ___value0;
		NullCheck(L_2);
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_3 = Component_get_gameObject_m0B0570BA8DDD3CD78A9DB568EA18D7317686603C(L_2, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_4 = Object_op_Equality_mBC2401774F3BE33E8CF6F0A8148E66C95D6CFF1C(L_3, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_4)
		{
			goto IL_001f;
		}
	}

IL_0017:
	{
		// WritePackedUInt32(0);
		NetworkWriter_WritePackedUInt32_m99DCA40833B068CB958663A5B583BC8D2051B12F(__this, 0, /*hidden argument*/NULL);
		// return;
		return;
	}

IL_001f:
	{
		// var uv = value.gameObject.GetComponent<NetworkIdentity>();
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_5 = ___value0;
		NullCheck(L_5);
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_6 = Component_get_gameObject_m0B0570BA8DDD3CD78A9DB568EA18D7317686603C(L_5, /*hidden argument*/NULL);
		NullCheck(L_6);
		NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * L_7 = GameObject_GetComponent_TisNetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B_m818B3B379B25E13EF0599E7709067A3E3F4B50FD(L_6, /*hidden argument*/GameObject_GetComponent_TisNetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B_m818B3B379B25E13EF0599E7709067A3E3F4B50FD_RuntimeMethod_var);
		V_0 = L_7;
		// if (uv != null)
		NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * L_8 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_9 = Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1(L_8, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_9)
		{
			goto IL_0041;
		}
	}
	{
		// Write(uv.netId);
		NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * L_10 = V_0;
		NullCheck(L_10);
		NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  L_11 = NetworkIdentity_get_netId_m22EB7CD04E2633FFAF99093749F79816B2BC9F28_inline(L_10, /*hidden argument*/NULL);
		NetworkWriter_Write_m327AAC971B7DA22E82661AD419E4D5EEC6CCAFBF(__this, L_11, /*hidden argument*/NULL);
		// }
		return;
	}

IL_0041:
	{
		// if (LogFilter.logWarn) { Debug.LogWarning("NetworkWriter " + value + " has no NetworkIdentity"); }
		IL2CPP_RUNTIME_CLASS_INIT(LogFilter_t5202A297E770086F7954B8D6703BAC03C22654ED_il2cpp_TypeInfo_var);
		bool L_12 = LogFilter_get_logWarn_m68D69BE30614BF75FF942A304F2C453298667AFD(/*hidden argument*/NULL);
		if (!L_12)
		{
			goto IL_005d;
		}
	}
	{
		// if (LogFilter.logWarn) { Debug.LogWarning("NetworkWriter " + value + " has no NetworkIdentity"); }
		Transform_tBB9E78A2766C3C83599A8F66EDE7D1FCAFC66EDA * L_13 = ___value0;
		String_t* L_14 = String_Concat_m2E1F71C491D2429CC80A28745488FEA947BB7AAC(_stringLiteralA45E7DA12B87A6DBEDA68DC73471E87E66E9C2E1, L_13, _stringLiteral1CDFEAB29C4AD9DAC96B9B86A0440B7DCCACBA06, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_LogWarning_m37338644DC81F640CCDFEAE35A223F0E965F0568(L_14, /*hidden argument*/NULL);
	}

IL_005d:
	{
		// WritePackedUInt32(0);
		NetworkWriter_WritePackedUInt32_m99DCA40833B068CB958663A5B583BC8D2051B12F(__this, 0, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(UnityEngine.GameObject)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m474C374D05DAAE6A0A4111512CA3B9E821A0A3EE (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * ___value0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkWriter_Write_m474C374D05DAAE6A0A4111512CA3B9E821A0A3EE_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * V_0 = NULL;
	{
		// if (value == null)
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_0 = ___value0;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_1 = Object_op_Equality_mBC2401774F3BE33E8CF6F0A8148E66C95D6CFF1C(L_0, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_0011;
		}
	}
	{
		// WritePackedUInt32(0);
		NetworkWriter_WritePackedUInt32_m99DCA40833B068CB958663A5B583BC8D2051B12F(__this, 0, /*hidden argument*/NULL);
		// return;
		return;
	}

IL_0011:
	{
		// var uv = value.GetComponent<NetworkIdentity>();
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_2 = ___value0;
		NullCheck(L_2);
		NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * L_3 = GameObject_GetComponent_TisNetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B_m818B3B379B25E13EF0599E7709067A3E3F4B50FD(L_2, /*hidden argument*/GameObject_GetComponent_TisNetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B_m818B3B379B25E13EF0599E7709067A3E3F4B50FD_RuntimeMethod_var);
		V_0 = L_3;
		// if (uv != null)
		NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * L_4 = V_0;
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_5 = Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1(L_4, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		if (!L_5)
		{
			goto IL_002e;
		}
	}
	{
		// Write(uv.netId);
		NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * L_6 = V_0;
		NullCheck(L_6);
		NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  L_7 = NetworkIdentity_get_netId_m22EB7CD04E2633FFAF99093749F79816B2BC9F28_inline(L_6, /*hidden argument*/NULL);
		NetworkWriter_Write_m327AAC971B7DA22E82661AD419E4D5EEC6CCAFBF(__this, L_7, /*hidden argument*/NULL);
		// }
		return;
	}

IL_002e:
	{
		// if (LogFilter.logWarn) { Debug.LogWarning("NetworkWriter " + value + " has no NetworkIdentity"); }
		IL2CPP_RUNTIME_CLASS_INIT(LogFilter_t5202A297E770086F7954B8D6703BAC03C22654ED_il2cpp_TypeInfo_var);
		bool L_8 = LogFilter_get_logWarn_m68D69BE30614BF75FF942A304F2C453298667AFD(/*hidden argument*/NULL);
		if (!L_8)
		{
			goto IL_004a;
		}
	}
	{
		// if (LogFilter.logWarn) { Debug.LogWarning("NetworkWriter " + value + " has no NetworkIdentity"); }
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_9 = ___value0;
		String_t* L_10 = String_Concat_m2E1F71C491D2429CC80A28745488FEA947BB7AAC(_stringLiteralA45E7DA12B87A6DBEDA68DC73471E87E66E9C2E1, L_9, _stringLiteral1CDFEAB29C4AD9DAC96B9B86A0440B7DCCACBA06, /*hidden argument*/NULL);
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_LogWarning_m37338644DC81F640CCDFEAE35A223F0E965F0568(L_10, /*hidden argument*/NULL);
	}

IL_004a:
	{
		// WritePackedUInt32(0);
		NetworkWriter_WritePackedUInt32_m99DCA40833B068CB958663A5B583BC8D2051B12F(__this, 0, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::Write(UnityEngine.Networking.MessageBase)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_Write_m9218FFD466269FA2D738894157700D6B8409B216 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, MessageBase_t2EA42B01AD6A5F36EAF84BE623801951B9F55416 * ___msg0, const RuntimeMethod* method)
{
	{
		// msg.Serialize(this);
		MessageBase_t2EA42B01AD6A5F36EAF84BE623801951B9F55416 * L_0 = ___msg0;
		NullCheck(L_0);
		VirtActionInvoker1< NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * >::Invoke(5 /* System.Void UnityEngine.Networking.MessageBase::Serialize(UnityEngine.Networking.NetworkWriter) */, L_0, __this);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::SeekZero()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_SeekZero_m14C6B4B8929557795BB4DC4D4CFADFBE3D10EA87 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, const RuntimeMethod* method)
{
	{
		// m_Buffer.SeekZero();
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_0 = __this->get_m_Buffer_1();
		NullCheck(L_0);
		NetBuffer_SeekZero_mDFE0EB8B9FD542812FEC8935D1E767A690C6CE1E(L_0, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::StartMessage(System.Int16)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_StartMessage_mD4F5BFA7ECA40EEA4AC721A1E357C3C8A09CE218 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, int16_t ___msgType0, const RuntimeMethod* method)
{
	{
		// SeekZero();
		NetworkWriter_SeekZero_m14C6B4B8929557795BB4DC4D4CFADFBE3D10EA87(__this, /*hidden argument*/NULL);
		// m_Buffer.WriteByte2(0, 0);
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_0 = __this->get_m_Buffer_1();
		NullCheck(L_0);
		NetBuffer_WriteByte2_m214A4267B67CD5BC4BF3F74EEC256774E2E5FB55(L_0, (uint8_t)0, (uint8_t)0, /*hidden argument*/NULL);
		// Write(msgType);
		int16_t L_1 = ___msgType0;
		NetworkWriter_Write_m9292C4A6802A8A84548CE8FC02CF90DB05720C2E(__this, L_1, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.NetworkWriter::FinishMessage()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void NetworkWriter_FinishMessage_mDA9E66815E448F635B2394A35DDCA3EC040B0590 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * __this, const RuntimeMethod* method)
{
	{
		// m_Buffer.FinishMessage();
		NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * L_0 = __this->get_m_Buffer_1();
		NullCheck(L_0);
		NetBuffer_FinishMessage_m75D5A784D18C0356FBF7E8FED96B7225F41D1E6D(L_0, /*hidden argument*/NULL);
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
// System.Void UnityEngine.Networking.PlayerController::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PlayerController__ctor_m81F20567B4C59352790048609083558A44935BC4 (PlayerController_tAB80FD64EAB9692832107A2D107D39D078CE7C5E * __this, const RuntimeMethod* method)
{
	{
		// public short playerControllerId = -1;
		__this->set_playerControllerId_1((int16_t)(-1));
		// public PlayerController()
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Boolean UnityEngine.Networking.PlayerController::get_IsValid()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool PlayerController_get_IsValid_m6D0B10FEEA50445C3E82BE909F4DEBA2108A5D68 (PlayerController_tAB80FD64EAB9692832107A2D107D39D078CE7C5E * __this, const RuntimeMethod* method)
{
	{
		// public bool IsValid { get { return playerControllerId != -1; } }
		int16_t L_0 = __this->get_playerControllerId_1();
		return (bool)((((int32_t)((((int32_t)L_0) == ((int32_t)(-1)))? 1 : 0)) == ((int32_t)0))? 1 : 0);
	}
}
// System.Void UnityEngine.Networking.PlayerController::.ctor(UnityEngine.GameObject,System.Int16)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void PlayerController__ctor_m9FF8174A299E9211F8D4EAAF8BA1C7EFE098D29C (PlayerController_tAB80FD64EAB9692832107A2D107D39D078CE7C5E * __this, GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * ___go0, int16_t ___playerControllerId1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (PlayerController__ctor_m9FF8174A299E9211F8D4EAAF8BA1C7EFE098D29C_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public short playerControllerId = -1;
		__this->set_playerControllerId_1((int16_t)(-1));
		// internal PlayerController(GameObject go, short playerControllerId)
		Object__ctor_m925ECA5E85CA100E3FB86A4F9E15C120E9A184C0(__this, /*hidden argument*/NULL);
		// gameObject = go;
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_0 = ___go0;
		__this->set_gameObject_3(L_0);
		// unetView = go.GetComponent<NetworkIdentity>();
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_1 = ___go0;
		NullCheck(L_1);
		NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * L_2 = GameObject_GetComponent_TisNetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B_m818B3B379B25E13EF0599E7709067A3E3F4B50FD(L_1, /*hidden argument*/GameObject_GetComponent_TisNetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B_m818B3B379B25E13EF0599E7709067A3E3F4B50FD_RuntimeMethod_var);
		__this->set_unetView_2(L_2);
		// this.playerControllerId = playerControllerId;
		int16_t L_3 = ___playerControllerId1;
		__this->set_playerControllerId_1(L_3);
		// }
		return;
	}
}
// System.String UnityEngine.Networking.PlayerController::ToString()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* PlayerController_ToString_m1E3830029B488BE2089DF2630ED9661C22649F19 (PlayerController_tAB80FD64EAB9692832107A2D107D39D078CE7C5E * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (PlayerController_ToString_m1E3830029B488BE2089DF2630ED9661C22649F19_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  V_0;
	memset((&V_0), 0, sizeof(V_0));
	int32_t G_B2_0 = 0;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* G_B2_1 = NULL;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* G_B2_2 = NULL;
	String_t* G_B2_3 = NULL;
	int32_t G_B1_0 = 0;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* G_B1_1 = NULL;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* G_B1_2 = NULL;
	String_t* G_B1_3 = NULL;
	String_t* G_B3_0 = NULL;
	int32_t G_B3_1 = 0;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* G_B3_2 = NULL;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* G_B3_3 = NULL;
	String_t* G_B3_4 = NULL;
	int32_t G_B5_0 = 0;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* G_B5_1 = NULL;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* G_B5_2 = NULL;
	String_t* G_B5_3 = NULL;
	int32_t G_B4_0 = 0;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* G_B4_1 = NULL;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* G_B4_2 = NULL;
	String_t* G_B4_3 = NULL;
	String_t* G_B6_0 = NULL;
	int32_t G_B6_1 = 0;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* G_B6_2 = NULL;
	ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* G_B6_3 = NULL;
	String_t* G_B6_4 = NULL;
	{
		// return string.Format("ID={0} NetworkIdentity NetID={1} Player={2}", new object[] { playerControllerId, (unetView != null ? unetView.netId.ToString() : "null"), (gameObject != null ? gameObject.name : "null") });
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_0 = (ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A*)SZArrayNew(ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A_il2cpp_TypeInfo_var, (uint32_t)3);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_1 = L_0;
		int16_t L_2 = __this->get_playerControllerId_1();
		int16_t L_3 = L_2;
		RuntimeObject * L_4 = Box(Int16_t823A20635DAF5A3D93A1E01CFBF3CBA27CF00B4D_il2cpp_TypeInfo_var, &L_3);
		NullCheck(L_1);
		ArrayElementTypeCheck (L_1, L_4);
		(L_1)->SetAt(static_cast<il2cpp_array_size_t>(0), (RuntimeObject *)L_4);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_5 = L_1;
		NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * L_6 = __this->get_unetView_2();
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_7 = Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1(L_6, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		G_B1_0 = 1;
		G_B1_1 = L_5;
		G_B1_2 = L_5;
		G_B1_3 = _stringLiteral1FB72BA5614BA625FBF384ACEA5077F842DEAC45;
		if (L_7)
		{
			G_B2_0 = 1;
			G_B2_1 = L_5;
			G_B2_2 = L_5;
			G_B2_3 = _stringLiteral1FB72BA5614BA625FBF384ACEA5077F842DEAC45;
			goto IL_0030;
		}
	}
	{
		G_B3_0 = _stringLiteral2BE88CA4242C76E8253AC62474851065032D6833;
		G_B3_1 = G_B1_0;
		G_B3_2 = G_B1_1;
		G_B3_3 = G_B1_2;
		G_B3_4 = G_B1_3;
		goto IL_0049;
	}

IL_0030:
	{
		NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * L_8 = __this->get_unetView_2();
		NullCheck(L_8);
		NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  L_9 = NetworkIdentity_get_netId_m22EB7CD04E2633FFAF99093749F79816B2BC9F28_inline(L_8, /*hidden argument*/NULL);
		V_0 = L_9;
		String_t* L_10 = NetworkInstanceId_ToString_m7550B88A961DA2A10D73F0E7D9739BD8715415ED((NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615 *)(&V_0), /*hidden argument*/NULL);
		G_B3_0 = L_10;
		G_B3_1 = G_B2_0;
		G_B3_2 = G_B2_1;
		G_B3_3 = G_B2_2;
		G_B3_4 = G_B2_3;
	}

IL_0049:
	{
		NullCheck(G_B3_2);
		ArrayElementTypeCheck (G_B3_2, G_B3_0);
		(G_B3_2)->SetAt(static_cast<il2cpp_array_size_t>(G_B3_1), (RuntimeObject *)G_B3_0);
		ObjectU5BU5D_t3C9242B5C88A48B2A5BD9FDA6CD0024E792AF08A* L_11 = G_B3_3;
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_12 = __this->get_gameObject_3();
		IL2CPP_RUNTIME_CLASS_INIT(Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0_il2cpp_TypeInfo_var);
		bool L_13 = Object_op_Inequality_m31EF58E217E8F4BDD3E409DEF79E1AEE95874FC1(L_12, (Object_tAE11E5E46CD5C37C9F3E8950C00CD8B45666A2D0 *)NULL, /*hidden argument*/NULL);
		G_B4_0 = 2;
		G_B4_1 = L_11;
		G_B4_2 = L_11;
		G_B4_3 = G_B3_4;
		if (L_13)
		{
			G_B5_0 = 2;
			G_B5_1 = L_11;
			G_B5_2 = L_11;
			G_B5_3 = G_B3_4;
			goto IL_0061;
		}
	}
	{
		G_B6_0 = _stringLiteral2BE88CA4242C76E8253AC62474851065032D6833;
		G_B6_1 = G_B4_0;
		G_B6_2 = G_B4_1;
		G_B6_3 = G_B4_2;
		G_B6_4 = G_B4_3;
		goto IL_006c;
	}

IL_0061:
	{
		GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * L_14 = __this->get_gameObject_3();
		NullCheck(L_14);
		String_t* L_15 = Object_get_name_mA2D400141CB3C991C87A2556429781DE961A83CE(L_14, /*hidden argument*/NULL);
		G_B6_0 = L_15;
		G_B6_1 = G_B5_0;
		G_B6_2 = G_B5_1;
		G_B6_3 = G_B5_2;
		G_B6_4 = G_B5_3;
	}

IL_006c:
	{
		NullCheck(G_B6_2);
		ArrayElementTypeCheck (G_B6_2, G_B6_0);
		(G_B6_2)->SetAt(static_cast<il2cpp_array_size_t>(G_B6_1), (RuntimeObject *)G_B6_0);
		String_t* L_16 = String_Format_mA3AC3FE7B23D97F3A5BAA082D25B0E01B341A865(G_B6_4, G_B6_3, /*hidden argument*/NULL);
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
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void UnityEngine.Networking.ServerAttribute::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ServerAttribute__ctor_mEA6DAE7D4DC430D74E064636C9DFEA1B328794F1 (ServerAttribute_tBEAD82CF18B52F903FB105CC54E39C66B82E079D * __this, const RuntimeMethod* method)
{
	{
		Attribute__ctor_m45CAD4B01265CC84CC5A84F62EE2DBE85DE89EC0(__this, /*hidden argument*/NULL);
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
// System.Void UnityEngine.Networking.ServerCallbackAttribute::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ServerCallbackAttribute__ctor_mF6FE36F59DD4CCB0DA741998AB90676EBFCFD678 (ServerCallbackAttribute_tD2D226910AED65FFCB395293D6A4235FE08BCF0F * __this, const RuntimeMethod* method)
{
	{
		Attribute__ctor_m45CAD4B01265CC84CC5A84F62EE2DBE85DE89EC0(__this, /*hidden argument*/NULL);
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
// System.Void UnityEngine.Networking.SpawnDelegate::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SpawnDelegate__ctor_m2253EBFB034B2E147DD9B2A6862B05BF6CD3F0F8 (SpawnDelegate_t4CB00A9006B512E467753C6CC752E29FA2EBC87F * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// UnityEngine.GameObject UnityEngine.Networking.SpawnDelegate::Invoke(UnityEngine.Vector3,UnityEngine.Networking.NetworkHash128)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * SpawnDelegate_Invoke_m1CCD07CF6BAAE9C583B6E12DC8E05FD4F0252A0F (SpawnDelegate_t4CB00A9006B512E467753C6CC752E29FA2EBC87F * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___position0, NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  ___assetId1, const RuntimeMethod* method)
{
	GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * result = NULL;
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
				typedef GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * (*FunctionPointerType) (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 , NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C , const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(___position0, ___assetId1, targetMethod);
			}
			else
			{
				// closed
				typedef GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * (*FunctionPointerType) (void*, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 , NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C , const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___position0, ___assetId1, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * (*FunctionPointerType) (Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 , NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C , const RuntimeMethod*);
					result = ((FunctionPointerType)targetMethodPointer)(___position0, ___assetId1, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = GenericInterfaceFuncInvoker2< GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 , NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  >::Invoke(targetMethod, targetThis, ___position0, ___assetId1);
					else
						result = GenericVirtFuncInvoker2< GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 , NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  >::Invoke(targetMethod, targetThis, ___position0, ___assetId1);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						result = InterfaceFuncInvoker2< GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 , NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___position0, ___assetId1);
					else
						result = VirtFuncInvoker2< GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F *, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 , NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___position0, ___assetId1);
				}
			}
			else
			{
				typedef GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * (*FunctionPointerType) (void*, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720 , NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C , const RuntimeMethod*);
				result = ((FunctionPointerType)targetMethodPointer)(targetThis, ___position0, ___assetId1, targetMethod);
			}
		}
	}
	return result;
}
// System.IAsyncResult UnityEngine.Networking.SpawnDelegate::BeginInvoke(UnityEngine.Vector3,UnityEngine.Networking.NetworkHash128,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* SpawnDelegate_BeginInvoke_m2C90BEE3D708A1F55BFF6E06B0D2DB011BE44DF3 (SpawnDelegate_t4CB00A9006B512E467753C6CC752E29FA2EBC87F * __this, Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  ___position0, NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C  ___assetId1, AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4 * ___callback2, RuntimeObject * ___object3, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SpawnDelegate_BeginInvoke_m2C90BEE3D708A1F55BFF6E06B0D2DB011BE44DF3_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	void *__d_args[3] = {0};
	__d_args[0] = Box(Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720_il2cpp_TypeInfo_var, &___position0);
	__d_args[1] = Box(NetworkHash128_t157C5C14B16832B67D8F519C11ABA013695AF28C_il2cpp_TypeInfo_var, &___assetId1);
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback2, (RuntimeObject*)___object3);
}
// UnityEngine.GameObject UnityEngine.Networking.SpawnDelegate::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * SpawnDelegate_EndInvoke_mD4624880E90D49A4D14EB60A0FD0BBB587D482E8 (SpawnDelegate_t4CB00A9006B512E467753C6CC752E29FA2EBC87F * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
{
	RuntimeObject *__result = il2cpp_codegen_delegate_end_invoke((Il2CppAsyncResult*) ___result0, 0);
	return (GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F *)__result;
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// System.Void UnityEngine.Networking.SyncEventAttribute::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncEventAttribute__ctor_mABE50405B03321CA30C8DCECD9238EDA12A56D16 (SyncEventAttribute_t32B6E9C1595BB49337BC42619BB697C84790630E * __this, const RuntimeMethod* method)
{
	{
		Attribute__ctor_m45CAD4B01265CC84CC5A84F62EE2DBE85DE89EC0(__this, /*hidden argument*/NULL);
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
// System.Void UnityEngine.Networking.SyncListBool::SerializeItem(UnityEngine.Networking.NetworkWriter,System.Boolean)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListBool_SerializeItem_mDE9DC2C7BCDF97B807CEBE5609D593E7E428D017 (SyncListBool_t4530597403BBB668F776B32DE46A1A91623EE054 * __this, NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * ___writer0, bool ___item1, const RuntimeMethod* method)
{
	{
		// writer.Write(item);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_0 = ___writer0;
		bool L_1 = ___item1;
		NullCheck(L_0);
		NetworkWriter_Write_m68E1030824D76CD6B46468FDC290B55C11D944C5(L_0, L_1, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Boolean UnityEngine.Networking.SyncListBool::DeserializeItem(UnityEngine.Networking.NetworkReader)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool SyncListBool_DeserializeItem_m61034D3F7D82F306DE3CE646B2FDC983AE707838 (SyncListBool_t4530597403BBB668F776B32DE46A1A91623EE054 * __this, NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * ___reader0, const RuntimeMethod* method)
{
	{
		// return reader.ReadBoolean();
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_0 = ___reader0;
		NullCheck(L_0);
		bool L_1 = NetworkReader_ReadBoolean_m6B4DCD23E4E794EEEA321B677BAE88E78A483CDF(L_0, /*hidden argument*/NULL);
		return L_1;
	}
}
// UnityEngine.Networking.SyncListBool UnityEngine.Networking.SyncListBool::ReadInstance(UnityEngine.Networking.NetworkReader)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SyncListBool_t4530597403BBB668F776B32DE46A1A91623EE054 * SyncListBool_ReadInstance_mA961F477308AD365BA3F7140CA4ABFC60CBE16F4 (NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * ___reader0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SyncListBool_ReadInstance_mA961F477308AD365BA3F7140CA4ABFC60CBE16F4_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	uint16_t V_0 = 0;
	SyncListBool_t4530597403BBB668F776B32DE46A1A91623EE054 * V_1 = NULL;
	uint16_t V_2 = 0;
	{
		// ushort count = reader.ReadUInt16();
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_0 = ___reader0;
		NullCheck(L_0);
		uint16_t L_1 = NetworkReader_ReadUInt16_m736BE183C9CBBB8A74C74038285C148746C2322F(L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		// var result = new SyncListBool();
		SyncListBool_t4530597403BBB668F776B32DE46A1A91623EE054 * L_2 = (SyncListBool_t4530597403BBB668F776B32DE46A1A91623EE054 *)il2cpp_codegen_object_new(SyncListBool_t4530597403BBB668F776B32DE46A1A91623EE054_il2cpp_TypeInfo_var);
		SyncListBool__ctor_m2BF7E2F5C16E5798F3DAD0B7C75DC606B00FF94C(L_2, /*hidden argument*/NULL);
		V_1 = L_2;
		// for (ushort i = 0; i < count; i++)
		V_2 = (uint16_t)0;
		goto IL_0022;
	}

IL_0011:
	{
		// result.AddInternal(reader.ReadBoolean());
		SyncListBool_t4530597403BBB668F776B32DE46A1A91623EE054 * L_3 = V_1;
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_4 = ___reader0;
		NullCheck(L_4);
		bool L_5 = NetworkReader_ReadBoolean_m6B4DCD23E4E794EEEA321B677BAE88E78A483CDF(L_4, /*hidden argument*/NULL);
		NullCheck(L_3);
		SyncList_1_AddInternal_m977B3CE5458FB772939C4CDB6612918FFC0BD427(L_3, L_5, /*hidden argument*/SyncList_1_AddInternal_m977B3CE5458FB772939C4CDB6612918FFC0BD427_RuntimeMethod_var);
		// for (ushort i = 0; i < count; i++)
		uint16_t L_6 = V_2;
		V_2 = (uint16_t)(((int32_t)((uint16_t)((int32_t)il2cpp_codegen_add((int32_t)L_6, (int32_t)1)))));
	}

IL_0022:
	{
		// for (ushort i = 0; i < count; i++)
		uint16_t L_7 = V_2;
		uint16_t L_8 = V_0;
		if ((((int32_t)L_7) < ((int32_t)L_8)))
		{
			goto IL_0011;
		}
	}
	{
		// return result;
		SyncListBool_t4530597403BBB668F776B32DE46A1A91623EE054 * L_9 = V_1;
		return L_9;
	}
}
// System.Void UnityEngine.Networking.SyncListBool::ReadReference(UnityEngine.Networking.NetworkReader,UnityEngine.Networking.SyncListBool)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListBool_ReadReference_m80A4B63470AE55EC05DF055CCE6C6D1C9CD0DA16 (NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * ___reader0, SyncListBool_t4530597403BBB668F776B32DE46A1A91623EE054 * ___syncList1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SyncListBool_ReadReference_m80A4B63470AE55EC05DF055CCE6C6D1C9CD0DA16_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	uint16_t V_0 = 0;
	uint16_t V_1 = 0;
	{
		// ushort count = reader.ReadUInt16();
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_0 = ___reader0;
		NullCheck(L_0);
		uint16_t L_1 = NetworkReader_ReadUInt16_m736BE183C9CBBB8A74C74038285C148746C2322F(L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		// syncList.Clear();
		SyncListBool_t4530597403BBB668F776B32DE46A1A91623EE054 * L_2 = ___syncList1;
		NullCheck(L_2);
		SyncList_1_Clear_mC367BED8954C65BFA956C2A66885A8FA241443E0(L_2, /*hidden argument*/SyncList_1_Clear_mC367BED8954C65BFA956C2A66885A8FA241443E0_RuntimeMethod_var);
		// for (ushort i = 0; i < count; i++)
		V_1 = (uint16_t)0;
		goto IL_0022;
	}

IL_0011:
	{
		// syncList.AddInternal(reader.ReadBoolean());
		SyncListBool_t4530597403BBB668F776B32DE46A1A91623EE054 * L_3 = ___syncList1;
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_4 = ___reader0;
		NullCheck(L_4);
		bool L_5 = NetworkReader_ReadBoolean_m6B4DCD23E4E794EEEA321B677BAE88E78A483CDF(L_4, /*hidden argument*/NULL);
		NullCheck(L_3);
		SyncList_1_AddInternal_m977B3CE5458FB772939C4CDB6612918FFC0BD427(L_3, L_5, /*hidden argument*/SyncList_1_AddInternal_m977B3CE5458FB772939C4CDB6612918FFC0BD427_RuntimeMethod_var);
		// for (ushort i = 0; i < count; i++)
		uint16_t L_6 = V_1;
		V_1 = (uint16_t)(((int32_t)((uint16_t)((int32_t)il2cpp_codegen_add((int32_t)L_6, (int32_t)1)))));
	}

IL_0022:
	{
		// for (ushort i = 0; i < count; i++)
		uint16_t L_7 = V_1;
		uint16_t L_8 = V_0;
		if ((((int32_t)L_7) < ((int32_t)L_8)))
		{
			goto IL_0011;
		}
	}
	{
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.SyncListBool::WriteInstance(UnityEngine.Networking.NetworkWriter,UnityEngine.Networking.SyncListBool)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListBool_WriteInstance_m5A8501D2567B9E44E86A0C48E390EE625F7E98CA (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * ___writer0, SyncListBool_t4530597403BBB668F776B32DE46A1A91623EE054 * ___items1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SyncListBool_WriteInstance_m5A8501D2567B9E44E86A0C48E390EE625F7E98CA_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	{
		// writer.Write((ushort)items.Count);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_0 = ___writer0;
		SyncListBool_t4530597403BBB668F776B32DE46A1A91623EE054 * L_1 = ___items1;
		NullCheck(L_1);
		int32_t L_2 = SyncList_1_get_Count_m9EBDDB18AA65B4522E066D29FE2ECD9980BDEAD9(L_1, /*hidden argument*/SyncList_1_get_Count_m9EBDDB18AA65B4522E066D29FE2ECD9980BDEAD9_RuntimeMethod_var);
		NullCheck(L_0);
		NetworkWriter_Write_mA00075C21036F9B7D020332BA99CCB2687D1C835(L_0, (uint16_t)(((int32_t)((uint16_t)L_2))), /*hidden argument*/NULL);
		// for (int i = 0; i < items.Count; i++)
		V_0 = 0;
		goto IL_0022;
	}

IL_0011:
	{
		// writer.Write(items[i]);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_3 = ___writer0;
		SyncListBool_t4530597403BBB668F776B32DE46A1A91623EE054 * L_4 = ___items1;
		int32_t L_5 = V_0;
		NullCheck(L_4);
		bool L_6 = SyncList_1_get_Item_m0EEA26E6C3ED4695254E4D9AC8243023AE227A48(L_4, L_5, /*hidden argument*/SyncList_1_get_Item_m0EEA26E6C3ED4695254E4D9AC8243023AE227A48_RuntimeMethod_var);
		NullCheck(L_3);
		NetworkWriter_Write_m68E1030824D76CD6B46468FDC290B55C11D944C5(L_3, L_6, /*hidden argument*/NULL);
		// for (int i = 0; i < items.Count; i++)
		int32_t L_7 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_add((int32_t)L_7, (int32_t)1));
	}

IL_0022:
	{
		// for (int i = 0; i < items.Count; i++)
		int32_t L_8 = V_0;
		SyncListBool_t4530597403BBB668F776B32DE46A1A91623EE054 * L_9 = ___items1;
		NullCheck(L_9);
		int32_t L_10 = SyncList_1_get_Count_m9EBDDB18AA65B4522E066D29FE2ECD9980BDEAD9(L_9, /*hidden argument*/SyncList_1_get_Count_m9EBDDB18AA65B4522E066D29FE2ECD9980BDEAD9_RuntimeMethod_var);
		if ((((int32_t)L_8) < ((int32_t)L_10)))
		{
			goto IL_0011;
		}
	}
	{
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.SyncListBool::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListBool__ctor_m2BF7E2F5C16E5798F3DAD0B7C75DC606B00FF94C (SyncListBool_t4530597403BBB668F776B32DE46A1A91623EE054 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SyncListBool__ctor_m2BF7E2F5C16E5798F3DAD0B7C75DC606B00FF94C_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		SyncList_1__ctor_m1BB28896D4C843EEF83232CE6648F916429D54E3(__this, /*hidden argument*/SyncList_1__ctor_m1BB28896D4C843EEF83232CE6648F916429D54E3_RuntimeMethod_var);
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
// System.Void UnityEngine.Networking.SyncListFloat::SerializeItem(UnityEngine.Networking.NetworkWriter,System.Single)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListFloat_SerializeItem_m7F8C579B21487BAF6186850CFEB055FDFD28B1F5 (SyncListFloat_tC8F12C17B783518D34953712B51249276C506922 * __this, NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * ___writer0, float ___item1, const RuntimeMethod* method)
{
	{
		// writer.Write(item);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_0 = ___writer0;
		float L_1 = ___item1;
		NullCheck(L_0);
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(L_0, L_1, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Single UnityEngine.Networking.SyncListFloat::DeserializeItem(UnityEngine.Networking.NetworkReader)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR float SyncListFloat_DeserializeItem_mB7FC064FD15E7176AA1F39793BEFA8DA1EA93F4B (SyncListFloat_tC8F12C17B783518D34953712B51249276C506922 * __this, NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * ___reader0, const RuntimeMethod* method)
{
	{
		// return reader.ReadSingle();
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_0 = ___reader0;
		NullCheck(L_0);
		float L_1 = NetworkReader_ReadSingle_mA5EE4F2C6A2FE9AA84AFC4FA0705B8CDAA7A4AAF(L_0, /*hidden argument*/NULL);
		return L_1;
	}
}
// UnityEngine.Networking.SyncListFloat UnityEngine.Networking.SyncListFloat::ReadInstance(UnityEngine.Networking.NetworkReader)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SyncListFloat_tC8F12C17B783518D34953712B51249276C506922 * SyncListFloat_ReadInstance_mB2A1ABC0F72AE1A31BC36DF6F23EAF05962639F1 (NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * ___reader0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SyncListFloat_ReadInstance_mB2A1ABC0F72AE1A31BC36DF6F23EAF05962639F1_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	uint16_t V_0 = 0;
	SyncListFloat_tC8F12C17B783518D34953712B51249276C506922 * V_1 = NULL;
	uint16_t V_2 = 0;
	{
		// ushort count = reader.ReadUInt16();
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_0 = ___reader0;
		NullCheck(L_0);
		uint16_t L_1 = NetworkReader_ReadUInt16_m736BE183C9CBBB8A74C74038285C148746C2322F(L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		// var result = new SyncListFloat();
		SyncListFloat_tC8F12C17B783518D34953712B51249276C506922 * L_2 = (SyncListFloat_tC8F12C17B783518D34953712B51249276C506922 *)il2cpp_codegen_object_new(SyncListFloat_tC8F12C17B783518D34953712B51249276C506922_il2cpp_TypeInfo_var);
		SyncListFloat__ctor_m68F03DF4317EADAA861FA0D251C797FD7CFA28ED(L_2, /*hidden argument*/NULL);
		V_1 = L_2;
		// for (ushort i = 0; i < count; i++)
		V_2 = (uint16_t)0;
		goto IL_0022;
	}

IL_0011:
	{
		// result.AddInternal(reader.ReadSingle());
		SyncListFloat_tC8F12C17B783518D34953712B51249276C506922 * L_3 = V_1;
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_4 = ___reader0;
		NullCheck(L_4);
		float L_5 = NetworkReader_ReadSingle_mA5EE4F2C6A2FE9AA84AFC4FA0705B8CDAA7A4AAF(L_4, /*hidden argument*/NULL);
		NullCheck(L_3);
		SyncList_1_AddInternal_mC17F547D0099E43ACAA4C5FD21D63DDE456602A6(L_3, L_5, /*hidden argument*/SyncList_1_AddInternal_mC17F547D0099E43ACAA4C5FD21D63DDE456602A6_RuntimeMethod_var);
		// for (ushort i = 0; i < count; i++)
		uint16_t L_6 = V_2;
		V_2 = (uint16_t)(((int32_t)((uint16_t)((int32_t)il2cpp_codegen_add((int32_t)L_6, (int32_t)1)))));
	}

IL_0022:
	{
		// for (ushort i = 0; i < count; i++)
		uint16_t L_7 = V_2;
		uint16_t L_8 = V_0;
		if ((((int32_t)L_7) < ((int32_t)L_8)))
		{
			goto IL_0011;
		}
	}
	{
		// return result;
		SyncListFloat_tC8F12C17B783518D34953712B51249276C506922 * L_9 = V_1;
		return L_9;
	}
}
// System.Void UnityEngine.Networking.SyncListFloat::ReadReference(UnityEngine.Networking.NetworkReader,UnityEngine.Networking.SyncListFloat)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListFloat_ReadReference_m54B4A4D3721639E3021DA90C770CFED61A61377E (NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * ___reader0, SyncListFloat_tC8F12C17B783518D34953712B51249276C506922 * ___syncList1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SyncListFloat_ReadReference_m54B4A4D3721639E3021DA90C770CFED61A61377E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	uint16_t V_0 = 0;
	uint16_t V_1 = 0;
	{
		// ushort count = reader.ReadUInt16();
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_0 = ___reader0;
		NullCheck(L_0);
		uint16_t L_1 = NetworkReader_ReadUInt16_m736BE183C9CBBB8A74C74038285C148746C2322F(L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		// syncList.Clear();
		SyncListFloat_tC8F12C17B783518D34953712B51249276C506922 * L_2 = ___syncList1;
		NullCheck(L_2);
		SyncList_1_Clear_m13160DF80DA71AAF005006E14C5C8985DBF15EB5(L_2, /*hidden argument*/SyncList_1_Clear_m13160DF80DA71AAF005006E14C5C8985DBF15EB5_RuntimeMethod_var);
		// for (ushort i = 0; i < count; i++)
		V_1 = (uint16_t)0;
		goto IL_0022;
	}

IL_0011:
	{
		// syncList.AddInternal(reader.ReadSingle());
		SyncListFloat_tC8F12C17B783518D34953712B51249276C506922 * L_3 = ___syncList1;
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_4 = ___reader0;
		NullCheck(L_4);
		float L_5 = NetworkReader_ReadSingle_mA5EE4F2C6A2FE9AA84AFC4FA0705B8CDAA7A4AAF(L_4, /*hidden argument*/NULL);
		NullCheck(L_3);
		SyncList_1_AddInternal_mC17F547D0099E43ACAA4C5FD21D63DDE456602A6(L_3, L_5, /*hidden argument*/SyncList_1_AddInternal_mC17F547D0099E43ACAA4C5FD21D63DDE456602A6_RuntimeMethod_var);
		// for (ushort i = 0; i < count; i++)
		uint16_t L_6 = V_1;
		V_1 = (uint16_t)(((int32_t)((uint16_t)((int32_t)il2cpp_codegen_add((int32_t)L_6, (int32_t)1)))));
	}

IL_0022:
	{
		// for (ushort i = 0; i < count; i++)
		uint16_t L_7 = V_1;
		uint16_t L_8 = V_0;
		if ((((int32_t)L_7) < ((int32_t)L_8)))
		{
			goto IL_0011;
		}
	}
	{
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.SyncListFloat::WriteInstance(UnityEngine.Networking.NetworkWriter,UnityEngine.Networking.SyncListFloat)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListFloat_WriteInstance_m3309F0578D122C45D2F3881C383D53626DF1986E (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * ___writer0, SyncListFloat_tC8F12C17B783518D34953712B51249276C506922 * ___items1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SyncListFloat_WriteInstance_m3309F0578D122C45D2F3881C383D53626DF1986E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	{
		// writer.Write((ushort)items.Count);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_0 = ___writer0;
		SyncListFloat_tC8F12C17B783518D34953712B51249276C506922 * L_1 = ___items1;
		NullCheck(L_1);
		int32_t L_2 = SyncList_1_get_Count_mCC0838D9ED25E463384E4852839E47B100C99577(L_1, /*hidden argument*/SyncList_1_get_Count_mCC0838D9ED25E463384E4852839E47B100C99577_RuntimeMethod_var);
		NullCheck(L_0);
		NetworkWriter_Write_mA00075C21036F9B7D020332BA99CCB2687D1C835(L_0, (uint16_t)(((int32_t)((uint16_t)L_2))), /*hidden argument*/NULL);
		// for (int i = 0; i < items.Count; i++)
		V_0 = 0;
		goto IL_0022;
	}

IL_0011:
	{
		// writer.Write(items[i]);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_3 = ___writer0;
		SyncListFloat_tC8F12C17B783518D34953712B51249276C506922 * L_4 = ___items1;
		int32_t L_5 = V_0;
		NullCheck(L_4);
		float L_6 = SyncList_1_get_Item_m70C832E1FED3E2D52297C7B6EF187700309BF7D4(L_4, L_5, /*hidden argument*/SyncList_1_get_Item_m70C832E1FED3E2D52297C7B6EF187700309BF7D4_RuntimeMethod_var);
		NullCheck(L_3);
		NetworkWriter_Write_m8D81ED6D6F371BE25A1A08BCE7A4A7673F561F6B(L_3, L_6, /*hidden argument*/NULL);
		// for (int i = 0; i < items.Count; i++)
		int32_t L_7 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_add((int32_t)L_7, (int32_t)1));
	}

IL_0022:
	{
		// for (int i = 0; i < items.Count; i++)
		int32_t L_8 = V_0;
		SyncListFloat_tC8F12C17B783518D34953712B51249276C506922 * L_9 = ___items1;
		NullCheck(L_9);
		int32_t L_10 = SyncList_1_get_Count_mCC0838D9ED25E463384E4852839E47B100C99577(L_9, /*hidden argument*/SyncList_1_get_Count_mCC0838D9ED25E463384E4852839E47B100C99577_RuntimeMethod_var);
		if ((((int32_t)L_8) < ((int32_t)L_10)))
		{
			goto IL_0011;
		}
	}
	{
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.SyncListFloat::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListFloat__ctor_m68F03DF4317EADAA861FA0D251C797FD7CFA28ED (SyncListFloat_tC8F12C17B783518D34953712B51249276C506922 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SyncListFloat__ctor_m68F03DF4317EADAA861FA0D251C797FD7CFA28ED_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		SyncList_1__ctor_mACF8E6F1689E85F8D9D88F6B2366C1A08D6F853E(__this, /*hidden argument*/SyncList_1__ctor_mACF8E6F1689E85F8D9D88F6B2366C1A08D6F853E_RuntimeMethod_var);
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
// System.Void UnityEngine.Networking.SyncListInt::SerializeItem(UnityEngine.Networking.NetworkWriter,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListInt_SerializeItem_mE18151CA974E644BD703D17CA7FB99AF33578454 (SyncListInt_t6D5125D26D629A9DB1325266BCFCDF2FC86FD9C6 * __this, NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * ___writer0, int32_t ___item1, const RuntimeMethod* method)
{
	{
		// writer.WritePackedUInt32((uint)item);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_0 = ___writer0;
		int32_t L_1 = ___item1;
		NullCheck(L_0);
		NetworkWriter_WritePackedUInt32_m99DCA40833B068CB958663A5B583BC8D2051B12F(L_0, L_1, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.Int32 UnityEngine.Networking.SyncListInt::DeserializeItem(UnityEngine.Networking.NetworkReader)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR int32_t SyncListInt_DeserializeItem_m81DB7DC5C8D2AC7A9EDA2DC4E323A071B67DD7B9 (SyncListInt_t6D5125D26D629A9DB1325266BCFCDF2FC86FD9C6 * __this, NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * ___reader0, const RuntimeMethod* method)
{
	{
		// return (int)reader.ReadPackedUInt32();
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_0 = ___reader0;
		NullCheck(L_0);
		uint32_t L_1 = NetworkReader_ReadPackedUInt32_mB0E5BF11AEAD652C88548BD93556D780A4E3F46B(L_0, /*hidden argument*/NULL);
		return L_1;
	}
}
// UnityEngine.Networking.SyncListInt UnityEngine.Networking.SyncListInt::ReadInstance(UnityEngine.Networking.NetworkReader)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SyncListInt_t6D5125D26D629A9DB1325266BCFCDF2FC86FD9C6 * SyncListInt_ReadInstance_m03CA3005B2153B13442ABF9CFB75B8D8B1B8A7A0 (NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * ___reader0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SyncListInt_ReadInstance_m03CA3005B2153B13442ABF9CFB75B8D8B1B8A7A0_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	uint16_t V_0 = 0;
	SyncListInt_t6D5125D26D629A9DB1325266BCFCDF2FC86FD9C6 * V_1 = NULL;
	uint16_t V_2 = 0;
	{
		// ushort count = reader.ReadUInt16();
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_0 = ___reader0;
		NullCheck(L_0);
		uint16_t L_1 = NetworkReader_ReadUInt16_m736BE183C9CBBB8A74C74038285C148746C2322F(L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		// var result = new SyncListInt();
		SyncListInt_t6D5125D26D629A9DB1325266BCFCDF2FC86FD9C6 * L_2 = (SyncListInt_t6D5125D26D629A9DB1325266BCFCDF2FC86FD9C6 *)il2cpp_codegen_object_new(SyncListInt_t6D5125D26D629A9DB1325266BCFCDF2FC86FD9C6_il2cpp_TypeInfo_var);
		SyncListInt__ctor_m9A8426FDD81908FDA8B94E67751AB67D0C52D90A(L_2, /*hidden argument*/NULL);
		V_1 = L_2;
		// for (ushort i = 0; i < count; i++)
		V_2 = (uint16_t)0;
		goto IL_0022;
	}

IL_0011:
	{
		// result.AddInternal((int)reader.ReadPackedUInt32());
		SyncListInt_t6D5125D26D629A9DB1325266BCFCDF2FC86FD9C6 * L_3 = V_1;
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_4 = ___reader0;
		NullCheck(L_4);
		uint32_t L_5 = NetworkReader_ReadPackedUInt32_mB0E5BF11AEAD652C88548BD93556D780A4E3F46B(L_4, /*hidden argument*/NULL);
		NullCheck(L_3);
		SyncList_1_AddInternal_m93CDCB4D3061B2F4CF88B74DEABE1C06D4AED23C(L_3, L_5, /*hidden argument*/SyncList_1_AddInternal_m93CDCB4D3061B2F4CF88B74DEABE1C06D4AED23C_RuntimeMethod_var);
		// for (ushort i = 0; i < count; i++)
		uint16_t L_6 = V_2;
		V_2 = (uint16_t)(((int32_t)((uint16_t)((int32_t)il2cpp_codegen_add((int32_t)L_6, (int32_t)1)))));
	}

IL_0022:
	{
		// for (ushort i = 0; i < count; i++)
		uint16_t L_7 = V_2;
		uint16_t L_8 = V_0;
		if ((((int32_t)L_7) < ((int32_t)L_8)))
		{
			goto IL_0011;
		}
	}
	{
		// return result;
		SyncListInt_t6D5125D26D629A9DB1325266BCFCDF2FC86FD9C6 * L_9 = V_1;
		return L_9;
	}
}
// System.Void UnityEngine.Networking.SyncListInt::ReadReference(UnityEngine.Networking.NetworkReader,UnityEngine.Networking.SyncListInt)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListInt_ReadReference_m3CC83A8EB36BA8DC1874FC2B781DED8877EBD188 (NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * ___reader0, SyncListInt_t6D5125D26D629A9DB1325266BCFCDF2FC86FD9C6 * ___syncList1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SyncListInt_ReadReference_m3CC83A8EB36BA8DC1874FC2B781DED8877EBD188_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	uint16_t V_0 = 0;
	uint16_t V_1 = 0;
	{
		// ushort count = reader.ReadUInt16();
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_0 = ___reader0;
		NullCheck(L_0);
		uint16_t L_1 = NetworkReader_ReadUInt16_m736BE183C9CBBB8A74C74038285C148746C2322F(L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		// syncList.Clear();
		SyncListInt_t6D5125D26D629A9DB1325266BCFCDF2FC86FD9C6 * L_2 = ___syncList1;
		NullCheck(L_2);
		SyncList_1_Clear_mF8FAE0172014F355D0C66600D8607442BF9A03B3(L_2, /*hidden argument*/SyncList_1_Clear_mF8FAE0172014F355D0C66600D8607442BF9A03B3_RuntimeMethod_var);
		// for (ushort i = 0; i < count; i++)
		V_1 = (uint16_t)0;
		goto IL_0022;
	}

IL_0011:
	{
		// syncList.AddInternal((int)reader.ReadPackedUInt32());
		SyncListInt_t6D5125D26D629A9DB1325266BCFCDF2FC86FD9C6 * L_3 = ___syncList1;
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_4 = ___reader0;
		NullCheck(L_4);
		uint32_t L_5 = NetworkReader_ReadPackedUInt32_mB0E5BF11AEAD652C88548BD93556D780A4E3F46B(L_4, /*hidden argument*/NULL);
		NullCheck(L_3);
		SyncList_1_AddInternal_m93CDCB4D3061B2F4CF88B74DEABE1C06D4AED23C(L_3, L_5, /*hidden argument*/SyncList_1_AddInternal_m93CDCB4D3061B2F4CF88B74DEABE1C06D4AED23C_RuntimeMethod_var);
		// for (ushort i = 0; i < count; i++)
		uint16_t L_6 = V_1;
		V_1 = (uint16_t)(((int32_t)((uint16_t)((int32_t)il2cpp_codegen_add((int32_t)L_6, (int32_t)1)))));
	}

IL_0022:
	{
		// for (ushort i = 0; i < count; i++)
		uint16_t L_7 = V_1;
		uint16_t L_8 = V_0;
		if ((((int32_t)L_7) < ((int32_t)L_8)))
		{
			goto IL_0011;
		}
	}
	{
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.SyncListInt::WriteInstance(UnityEngine.Networking.NetworkWriter,UnityEngine.Networking.SyncListInt)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListInt_WriteInstance_m5C61B3494D72E98F5D6A03F7917F8553CD5CFAF4 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * ___writer0, SyncListInt_t6D5125D26D629A9DB1325266BCFCDF2FC86FD9C6 * ___items1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SyncListInt_WriteInstance_m5C61B3494D72E98F5D6A03F7917F8553CD5CFAF4_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	{
		// writer.Write((ushort)items.Count);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_0 = ___writer0;
		SyncListInt_t6D5125D26D629A9DB1325266BCFCDF2FC86FD9C6 * L_1 = ___items1;
		NullCheck(L_1);
		int32_t L_2 = SyncList_1_get_Count_m7E687EFF75167B5EB639F273102ED345B8CB905B(L_1, /*hidden argument*/SyncList_1_get_Count_m7E687EFF75167B5EB639F273102ED345B8CB905B_RuntimeMethod_var);
		NullCheck(L_0);
		NetworkWriter_Write_mA00075C21036F9B7D020332BA99CCB2687D1C835(L_0, (uint16_t)(((int32_t)((uint16_t)L_2))), /*hidden argument*/NULL);
		// for (int i = 0; i < items.Count; i++)
		V_0 = 0;
		goto IL_0022;
	}

IL_0011:
	{
		// writer.WritePackedUInt32((uint)items[i]);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_3 = ___writer0;
		SyncListInt_t6D5125D26D629A9DB1325266BCFCDF2FC86FD9C6 * L_4 = ___items1;
		int32_t L_5 = V_0;
		NullCheck(L_4);
		int32_t L_6 = SyncList_1_get_Item_mA89484861CD0098C5FC7466F93F18C4EE231C55F(L_4, L_5, /*hidden argument*/SyncList_1_get_Item_mA89484861CD0098C5FC7466F93F18C4EE231C55F_RuntimeMethod_var);
		NullCheck(L_3);
		NetworkWriter_WritePackedUInt32_m99DCA40833B068CB958663A5B583BC8D2051B12F(L_3, L_6, /*hidden argument*/NULL);
		// for (int i = 0; i < items.Count; i++)
		int32_t L_7 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_add((int32_t)L_7, (int32_t)1));
	}

IL_0022:
	{
		// for (int i = 0; i < items.Count; i++)
		int32_t L_8 = V_0;
		SyncListInt_t6D5125D26D629A9DB1325266BCFCDF2FC86FD9C6 * L_9 = ___items1;
		NullCheck(L_9);
		int32_t L_10 = SyncList_1_get_Count_m7E687EFF75167B5EB639F273102ED345B8CB905B(L_9, /*hidden argument*/SyncList_1_get_Count_m7E687EFF75167B5EB639F273102ED345B8CB905B_RuntimeMethod_var);
		if ((((int32_t)L_8) < ((int32_t)L_10)))
		{
			goto IL_0011;
		}
	}
	{
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.SyncListInt::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListInt__ctor_m9A8426FDD81908FDA8B94E67751AB67D0C52D90A (SyncListInt_t6D5125D26D629A9DB1325266BCFCDF2FC86FD9C6 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SyncListInt__ctor_m9A8426FDD81908FDA8B94E67751AB67D0C52D90A_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		SyncList_1__ctor_m6E3A6F39EE2A332965D0B912FD1662297B44B901(__this, /*hidden argument*/SyncList_1__ctor_m6E3A6F39EE2A332965D0B912FD1662297B44B901_RuntimeMethod_var);
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
// System.Void UnityEngine.Networking.SyncListString::SerializeItem(UnityEngine.Networking.NetworkWriter,System.String)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListString_SerializeItem_m3F915193521F2D35206821CA40DB8272A084B9D4 (SyncListString_t26D5186F91FB985D01BDC6CC5B4C7C13FA3740CD * __this, NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * ___writer0, String_t* ___item1, const RuntimeMethod* method)
{
	{
		// writer.Write(item);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_0 = ___writer0;
		String_t* L_1 = ___item1;
		NullCheck(L_0);
		NetworkWriter_Write_m856F6DD1E132E2C68BA9D7D36A5ED5EAA1D108F4(L_0, L_1, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.String UnityEngine.Networking.SyncListString::DeserializeItem(UnityEngine.Networking.NetworkReader)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* SyncListString_DeserializeItem_mFA14C524369550C299AAF95A28045C91C3092BF3 (SyncListString_t26D5186F91FB985D01BDC6CC5B4C7C13FA3740CD * __this, NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * ___reader0, const RuntimeMethod* method)
{
	{
		// return reader.ReadString();
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_0 = ___reader0;
		NullCheck(L_0);
		String_t* L_1 = NetworkReader_ReadString_mF004D69C1AE3038215701A8E43973D1FA7BDB364(L_0, /*hidden argument*/NULL);
		return L_1;
	}
}
// UnityEngine.Networking.SyncListString UnityEngine.Networking.SyncListString::ReadInstance(UnityEngine.Networking.NetworkReader)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SyncListString_t26D5186F91FB985D01BDC6CC5B4C7C13FA3740CD * SyncListString_ReadInstance_m2EC7D4993F62FF87C40BB3112B04D2DA4D02C079 (NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * ___reader0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SyncListString_ReadInstance_m2EC7D4993F62FF87C40BB3112B04D2DA4D02C079_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	uint16_t V_0 = 0;
	SyncListString_t26D5186F91FB985D01BDC6CC5B4C7C13FA3740CD * V_1 = NULL;
	uint16_t V_2 = 0;
	{
		// ushort count = reader.ReadUInt16();
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_0 = ___reader0;
		NullCheck(L_0);
		uint16_t L_1 = NetworkReader_ReadUInt16_m736BE183C9CBBB8A74C74038285C148746C2322F(L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		// var result = new SyncListString();
		SyncListString_t26D5186F91FB985D01BDC6CC5B4C7C13FA3740CD * L_2 = (SyncListString_t26D5186F91FB985D01BDC6CC5B4C7C13FA3740CD *)il2cpp_codegen_object_new(SyncListString_t26D5186F91FB985D01BDC6CC5B4C7C13FA3740CD_il2cpp_TypeInfo_var);
		SyncListString__ctor_m50D229AE4F36D878B3FBB78517104B1D34BA3F38(L_2, /*hidden argument*/NULL);
		V_1 = L_2;
		// for (ushort i = 0; i < count; i++)
		V_2 = (uint16_t)0;
		goto IL_0022;
	}

IL_0011:
	{
		// result.AddInternal(reader.ReadString());
		SyncListString_t26D5186F91FB985D01BDC6CC5B4C7C13FA3740CD * L_3 = V_1;
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_4 = ___reader0;
		NullCheck(L_4);
		String_t* L_5 = NetworkReader_ReadString_mF004D69C1AE3038215701A8E43973D1FA7BDB364(L_4, /*hidden argument*/NULL);
		NullCheck(L_3);
		SyncList_1_AddInternal_m02EF37FDD57B236ED985C01D53E7E181843A33D8(L_3, L_5, /*hidden argument*/SyncList_1_AddInternal_m02EF37FDD57B236ED985C01D53E7E181843A33D8_RuntimeMethod_var);
		// for (ushort i = 0; i < count; i++)
		uint16_t L_6 = V_2;
		V_2 = (uint16_t)(((int32_t)((uint16_t)((int32_t)il2cpp_codegen_add((int32_t)L_6, (int32_t)1)))));
	}

IL_0022:
	{
		// for (ushort i = 0; i < count; i++)
		uint16_t L_7 = V_2;
		uint16_t L_8 = V_0;
		if ((((int32_t)L_7) < ((int32_t)L_8)))
		{
			goto IL_0011;
		}
	}
	{
		// return result;
		SyncListString_t26D5186F91FB985D01BDC6CC5B4C7C13FA3740CD * L_9 = V_1;
		return L_9;
	}
}
// System.Void UnityEngine.Networking.SyncListString::ReadReference(UnityEngine.Networking.NetworkReader,UnityEngine.Networking.SyncListString)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListString_ReadReference_m0900B20871BBE6147CF5EA035D3BD48A8415AAC4 (NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * ___reader0, SyncListString_t26D5186F91FB985D01BDC6CC5B4C7C13FA3740CD * ___syncList1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SyncListString_ReadReference_m0900B20871BBE6147CF5EA035D3BD48A8415AAC4_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	uint16_t V_0 = 0;
	uint16_t V_1 = 0;
	{
		// ushort count = reader.ReadUInt16();
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_0 = ___reader0;
		NullCheck(L_0);
		uint16_t L_1 = NetworkReader_ReadUInt16_m736BE183C9CBBB8A74C74038285C148746C2322F(L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		// syncList.Clear();
		SyncListString_t26D5186F91FB985D01BDC6CC5B4C7C13FA3740CD * L_2 = ___syncList1;
		NullCheck(L_2);
		SyncList_1_Clear_mAF7EFFA62345875E1C183F7D3A09A57A0E05E97B(L_2, /*hidden argument*/SyncList_1_Clear_mAF7EFFA62345875E1C183F7D3A09A57A0E05E97B_RuntimeMethod_var);
		// for (ushort i = 0; i < count; i++)
		V_1 = (uint16_t)0;
		goto IL_0022;
	}

IL_0011:
	{
		// syncList.AddInternal(reader.ReadString());
		SyncListString_t26D5186F91FB985D01BDC6CC5B4C7C13FA3740CD * L_3 = ___syncList1;
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_4 = ___reader0;
		NullCheck(L_4);
		String_t* L_5 = NetworkReader_ReadString_mF004D69C1AE3038215701A8E43973D1FA7BDB364(L_4, /*hidden argument*/NULL);
		NullCheck(L_3);
		SyncList_1_AddInternal_m02EF37FDD57B236ED985C01D53E7E181843A33D8(L_3, L_5, /*hidden argument*/SyncList_1_AddInternal_m02EF37FDD57B236ED985C01D53E7E181843A33D8_RuntimeMethod_var);
		// for (ushort i = 0; i < count; i++)
		uint16_t L_6 = V_1;
		V_1 = (uint16_t)(((int32_t)((uint16_t)((int32_t)il2cpp_codegen_add((int32_t)L_6, (int32_t)1)))));
	}

IL_0022:
	{
		// for (ushort i = 0; i < count; i++)
		uint16_t L_7 = V_1;
		uint16_t L_8 = V_0;
		if ((((int32_t)L_7) < ((int32_t)L_8)))
		{
			goto IL_0011;
		}
	}
	{
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.SyncListString::WriteInstance(UnityEngine.Networking.NetworkWriter,UnityEngine.Networking.SyncListString)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListString_WriteInstance_m6FCCCB6C180BB58E59A883B568906A4072EBB2F2 (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * ___writer0, SyncListString_t26D5186F91FB985D01BDC6CC5B4C7C13FA3740CD * ___items1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SyncListString_WriteInstance_m6FCCCB6C180BB58E59A883B568906A4072EBB2F2_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	{
		// writer.Write((ushort)items.Count);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_0 = ___writer0;
		SyncListString_t26D5186F91FB985D01BDC6CC5B4C7C13FA3740CD * L_1 = ___items1;
		NullCheck(L_1);
		int32_t L_2 = SyncList_1_get_Count_m641E2517509914AAC0415508A728F40A914318C4(L_1, /*hidden argument*/SyncList_1_get_Count_m641E2517509914AAC0415508A728F40A914318C4_RuntimeMethod_var);
		NullCheck(L_0);
		NetworkWriter_Write_mA00075C21036F9B7D020332BA99CCB2687D1C835(L_0, (uint16_t)(((int32_t)((uint16_t)L_2))), /*hidden argument*/NULL);
		// for (int i = 0; i < items.Count; i++)
		V_0 = 0;
		goto IL_0022;
	}

IL_0011:
	{
		// writer.Write(items[i]);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_3 = ___writer0;
		SyncListString_t26D5186F91FB985D01BDC6CC5B4C7C13FA3740CD * L_4 = ___items1;
		int32_t L_5 = V_0;
		NullCheck(L_4);
		String_t* L_6 = SyncList_1_get_Item_m0578989F729AF1CD8C5F378289B5DF1FA830AE16(L_4, L_5, /*hidden argument*/SyncList_1_get_Item_m0578989F729AF1CD8C5F378289B5DF1FA830AE16_RuntimeMethod_var);
		NullCheck(L_3);
		NetworkWriter_Write_m856F6DD1E132E2C68BA9D7D36A5ED5EAA1D108F4(L_3, L_6, /*hidden argument*/NULL);
		// for (int i = 0; i < items.Count; i++)
		int32_t L_7 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_add((int32_t)L_7, (int32_t)1));
	}

IL_0022:
	{
		// for (int i = 0; i < items.Count; i++)
		int32_t L_8 = V_0;
		SyncListString_t26D5186F91FB985D01BDC6CC5B4C7C13FA3740CD * L_9 = ___items1;
		NullCheck(L_9);
		int32_t L_10 = SyncList_1_get_Count_m641E2517509914AAC0415508A728F40A914318C4(L_9, /*hidden argument*/SyncList_1_get_Count_m641E2517509914AAC0415508A728F40A914318C4_RuntimeMethod_var);
		if ((((int32_t)L_8) < ((int32_t)L_10)))
		{
			goto IL_0011;
		}
	}
	{
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.SyncListString::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListString__ctor_m50D229AE4F36D878B3FBB78517104B1D34BA3F38 (SyncListString_t26D5186F91FB985D01BDC6CC5B4C7C13FA3740CD * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SyncListString__ctor_m50D229AE4F36D878B3FBB78517104B1D34BA3F38_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		SyncList_1__ctor_mBBB1AF24E09B273530603FA90034B2B830E2460C(__this, /*hidden argument*/SyncList_1__ctor_mBBB1AF24E09B273530603FA90034B2B830E2460C_RuntimeMethod_var);
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
// System.Void UnityEngine.Networking.SyncListUInt::SerializeItem(UnityEngine.Networking.NetworkWriter,System.UInt32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListUInt_SerializeItem_mF460132DABBBACC475678478FC14F106E8768B08 (SyncListUInt_tF223A88F804D7F2819F5F610669176CE6E93A0E6 * __this, NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * ___writer0, uint32_t ___item1, const RuntimeMethod* method)
{
	{
		// writer.WritePackedUInt32(item);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_0 = ___writer0;
		uint32_t L_1 = ___item1;
		NullCheck(L_0);
		NetworkWriter_WritePackedUInt32_m99DCA40833B068CB958663A5B583BC8D2051B12F(L_0, L_1, /*hidden argument*/NULL);
		// }
		return;
	}
}
// System.UInt32 UnityEngine.Networking.SyncListUInt::DeserializeItem(UnityEngine.Networking.NetworkReader)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t SyncListUInt_DeserializeItem_m7CFD393B723C31D6418E993D68E2A421384ED829 (SyncListUInt_tF223A88F804D7F2819F5F610669176CE6E93A0E6 * __this, NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * ___reader0, const RuntimeMethod* method)
{
	{
		// return reader.ReadPackedUInt32();
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_0 = ___reader0;
		NullCheck(L_0);
		uint32_t L_1 = NetworkReader_ReadPackedUInt32_mB0E5BF11AEAD652C88548BD93556D780A4E3F46B(L_0, /*hidden argument*/NULL);
		return L_1;
	}
}
// UnityEngine.Networking.SyncListUInt UnityEngine.Networking.SyncListUInt::ReadInstance(UnityEngine.Networking.NetworkReader)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR SyncListUInt_tF223A88F804D7F2819F5F610669176CE6E93A0E6 * SyncListUInt_ReadInstance_m8477E9A83BCCED58B06B8AEF6CBFEA2BFE25292E (NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * ___reader0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SyncListUInt_ReadInstance_m8477E9A83BCCED58B06B8AEF6CBFEA2BFE25292E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	uint16_t V_0 = 0;
	SyncListUInt_tF223A88F804D7F2819F5F610669176CE6E93A0E6 * V_1 = NULL;
	uint16_t V_2 = 0;
	{
		// ushort count = reader.ReadUInt16();
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_0 = ___reader0;
		NullCheck(L_0);
		uint16_t L_1 = NetworkReader_ReadUInt16_m736BE183C9CBBB8A74C74038285C148746C2322F(L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		// var result = new SyncListUInt();
		SyncListUInt_tF223A88F804D7F2819F5F610669176CE6E93A0E6 * L_2 = (SyncListUInt_tF223A88F804D7F2819F5F610669176CE6E93A0E6 *)il2cpp_codegen_object_new(SyncListUInt_tF223A88F804D7F2819F5F610669176CE6E93A0E6_il2cpp_TypeInfo_var);
		SyncListUInt__ctor_mBAD30E72F2FB4BFA239B5DDABCCFC0DEEFD918AC(L_2, /*hidden argument*/NULL);
		V_1 = L_2;
		// for (ushort i = 0; i < count; i++)
		V_2 = (uint16_t)0;
		goto IL_0022;
	}

IL_0011:
	{
		// result.AddInternal(reader.ReadPackedUInt32());
		SyncListUInt_tF223A88F804D7F2819F5F610669176CE6E93A0E6 * L_3 = V_1;
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_4 = ___reader0;
		NullCheck(L_4);
		uint32_t L_5 = NetworkReader_ReadPackedUInt32_mB0E5BF11AEAD652C88548BD93556D780A4E3F46B(L_4, /*hidden argument*/NULL);
		NullCheck(L_3);
		SyncList_1_AddInternal_m84938C896AA1F3EED3568ECB90FED244DF2617B2(L_3, L_5, /*hidden argument*/SyncList_1_AddInternal_m84938C896AA1F3EED3568ECB90FED244DF2617B2_RuntimeMethod_var);
		// for (ushort i = 0; i < count; i++)
		uint16_t L_6 = V_2;
		V_2 = (uint16_t)(((int32_t)((uint16_t)((int32_t)il2cpp_codegen_add((int32_t)L_6, (int32_t)1)))));
	}

IL_0022:
	{
		// for (ushort i = 0; i < count; i++)
		uint16_t L_7 = V_2;
		uint16_t L_8 = V_0;
		if ((((int32_t)L_7) < ((int32_t)L_8)))
		{
			goto IL_0011;
		}
	}
	{
		// return result;
		SyncListUInt_tF223A88F804D7F2819F5F610669176CE6E93A0E6 * L_9 = V_1;
		return L_9;
	}
}
// System.Void UnityEngine.Networking.SyncListUInt::ReadReference(UnityEngine.Networking.NetworkReader,UnityEngine.Networking.SyncListUInt)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListUInt_ReadReference_m3CDD33D651FD933EBDCFA62C19D3C37C268BB18A (NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * ___reader0, SyncListUInt_tF223A88F804D7F2819F5F610669176CE6E93A0E6 * ___syncList1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SyncListUInt_ReadReference_m3CDD33D651FD933EBDCFA62C19D3C37C268BB18A_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	uint16_t V_0 = 0;
	uint16_t V_1 = 0;
	{
		// ushort count = reader.ReadUInt16();
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_0 = ___reader0;
		NullCheck(L_0);
		uint16_t L_1 = NetworkReader_ReadUInt16_m736BE183C9CBBB8A74C74038285C148746C2322F(L_0, /*hidden argument*/NULL);
		V_0 = L_1;
		// syncList.Clear();
		SyncListUInt_tF223A88F804D7F2819F5F610669176CE6E93A0E6 * L_2 = ___syncList1;
		NullCheck(L_2);
		SyncList_1_Clear_m00C3496EAD8E618F4C20CA6F618373D4564CEB58(L_2, /*hidden argument*/SyncList_1_Clear_m00C3496EAD8E618F4C20CA6F618373D4564CEB58_RuntimeMethod_var);
		// for (ushort i = 0; i < count; i++)
		V_1 = (uint16_t)0;
		goto IL_0022;
	}

IL_0011:
	{
		// syncList.AddInternal(reader.ReadPackedUInt32());
		SyncListUInt_tF223A88F804D7F2819F5F610669176CE6E93A0E6 * L_3 = ___syncList1;
		NetworkReader_t7011A2F66F461EA5D4413F3979F1F3244D82FD12 * L_4 = ___reader0;
		NullCheck(L_4);
		uint32_t L_5 = NetworkReader_ReadPackedUInt32_mB0E5BF11AEAD652C88548BD93556D780A4E3F46B(L_4, /*hidden argument*/NULL);
		NullCheck(L_3);
		SyncList_1_AddInternal_m84938C896AA1F3EED3568ECB90FED244DF2617B2(L_3, L_5, /*hidden argument*/SyncList_1_AddInternal_m84938C896AA1F3EED3568ECB90FED244DF2617B2_RuntimeMethod_var);
		// for (ushort i = 0; i < count; i++)
		uint16_t L_6 = V_1;
		V_1 = (uint16_t)(((int32_t)((uint16_t)((int32_t)il2cpp_codegen_add((int32_t)L_6, (int32_t)1)))));
	}

IL_0022:
	{
		// for (ushort i = 0; i < count; i++)
		uint16_t L_7 = V_1;
		uint16_t L_8 = V_0;
		if ((((int32_t)L_7) < ((int32_t)L_8)))
		{
			goto IL_0011;
		}
	}
	{
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.SyncListUInt::WriteInstance(UnityEngine.Networking.NetworkWriter,UnityEngine.Networking.SyncListUInt)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListUInt_WriteInstance_m2C862802FBB8C20BE3E0B06434F89BF457887C9E (NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * ___writer0, SyncListUInt_tF223A88F804D7F2819F5F610669176CE6E93A0E6 * ___items1, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SyncListUInt_WriteInstance_m2C862802FBB8C20BE3E0B06434F89BF457887C9E_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	{
		// writer.Write((ushort)items.Count);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_0 = ___writer0;
		SyncListUInt_tF223A88F804D7F2819F5F610669176CE6E93A0E6 * L_1 = ___items1;
		NullCheck(L_1);
		int32_t L_2 = SyncList_1_get_Count_m29E32BA907E6C50793D6A2D30D22A8D052A978B8(L_1, /*hidden argument*/SyncList_1_get_Count_m29E32BA907E6C50793D6A2D30D22A8D052A978B8_RuntimeMethod_var);
		NullCheck(L_0);
		NetworkWriter_Write_mA00075C21036F9B7D020332BA99CCB2687D1C835(L_0, (uint16_t)(((int32_t)((uint16_t)L_2))), /*hidden argument*/NULL);
		// for (int i = 0; i < items.Count; i++)
		V_0 = 0;
		goto IL_0022;
	}

IL_0011:
	{
		// writer.WritePackedUInt32(items[i]);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_3 = ___writer0;
		SyncListUInt_tF223A88F804D7F2819F5F610669176CE6E93A0E6 * L_4 = ___items1;
		int32_t L_5 = V_0;
		NullCheck(L_4);
		uint32_t L_6 = SyncList_1_get_Item_mC1369C43D41DC4C7863526B187E820DD7DA3709D(L_4, L_5, /*hidden argument*/SyncList_1_get_Item_mC1369C43D41DC4C7863526B187E820DD7DA3709D_RuntimeMethod_var);
		NullCheck(L_3);
		NetworkWriter_WritePackedUInt32_m99DCA40833B068CB958663A5B583BC8D2051B12F(L_3, L_6, /*hidden argument*/NULL);
		// for (int i = 0; i < items.Count; i++)
		int32_t L_7 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_add((int32_t)L_7, (int32_t)1));
	}

IL_0022:
	{
		// for (int i = 0; i < items.Count; i++)
		int32_t L_8 = V_0;
		SyncListUInt_tF223A88F804D7F2819F5F610669176CE6E93A0E6 * L_9 = ___items1;
		NullCheck(L_9);
		int32_t L_10 = SyncList_1_get_Count_m29E32BA907E6C50793D6A2D30D22A8D052A978B8(L_9, /*hidden argument*/SyncList_1_get_Count_m29E32BA907E6C50793D6A2D30D22A8D052A978B8_RuntimeMethod_var);
		if ((((int32_t)L_8) < ((int32_t)L_10)))
		{
			goto IL_0011;
		}
	}
	{
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.SyncListUInt::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncListUInt__ctor_mBAD30E72F2FB4BFA239B5DDABCCFC0DEEFD918AC (SyncListUInt_tF223A88F804D7F2819F5F610669176CE6E93A0E6 * __this, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (SyncListUInt__ctor_mBAD30E72F2FB4BFA239B5DDABCCFC0DEEFD918AC_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		SyncList_1__ctor_mF18B74E2EF8296E263BCEBAB8C8DE0EA78F8BAFC(__this, /*hidden argument*/SyncList_1__ctor_mF18B74E2EF8296E263BCEBAB8C8DE0EA78F8BAFC_RuntimeMethod_var);
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
// System.Void UnityEngine.Networking.SyncVarAttribute::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SyncVarAttribute__ctor_mA97F5F18302F4C616AD111707C3F9E6C83AF86B3 (SyncVarAttribute_tD57FE395DED8D547F0200B7F50F36DFA27C6BF3A * __this, const RuntimeMethod* method)
{
	{
		Attribute__ctor_m45CAD4B01265CC84CC5A84F62EE2DBE85DE89EC0(__this, /*hidden argument*/NULL);
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
// System.Void UnityEngine.Networking.TargetRpcAttribute::.ctor()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void TargetRpcAttribute__ctor_m69283E8DDBCC7B13BC6164A0087E3D0EC2BDD4E8 (TargetRpcAttribute_t7B515CB5DD6D609483DFC4ACC89D00B00C9EAE03 * __this, const RuntimeMethod* method)
{
	{
		Attribute__ctor_m45CAD4B01265CC84CC5A84F62EE2DBE85DE89EC0(__this, /*hidden argument*/NULL);
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
// UnityEngine.Networking.LocalClient UnityEngine.Networking.ULocalConnectionToClient::get_localClient()
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR LocalClient_tCEC0096B13C433140FD4C09424CE345B28FE3C86 * ULocalConnectionToClient_get_localClient_mFA1151CD224CF848FF175CE075EBECAA82C118E4 (ULocalConnectionToClient_t7AF7EBF2BEC3714F75EF894035BFAE9E6F9561A8 * __this, const RuntimeMethod* method)
{
	{
		// public LocalClient localClient { get {  return m_LocalClient; } }
		LocalClient_tCEC0096B13C433140FD4C09424CE345B28FE3C86 * L_0 = __this->get_m_LocalClient_19();
		return L_0;
	}
}
// System.Void UnityEngine.Networking.ULocalConnectionToClient::.ctor(UnityEngine.Networking.LocalClient)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ULocalConnectionToClient__ctor_mA475E50C32BC0BDEF1B6B574226B539007AA56ED (ULocalConnectionToClient_t7AF7EBF2BEC3714F75EF894035BFAE9E6F9561A8 * __this, LocalClient_tCEC0096B13C433140FD4C09424CE345B28FE3C86 * ___localClient0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ULocalConnectionToClient__ctor_mA475E50C32BC0BDEF1B6B574226B539007AA56ED_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public ULocalConnectionToClient(LocalClient localClient)
		NetworkConnection__ctor_mDD96E228FE96C836C690ADBFDC26C3FFDA31CEC9(__this, /*hidden argument*/NULL);
		// address = "localClient";
		((NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA *)__this)->set_address_14(_stringLiteral0C84AC39FC120C1E579C09FEAA062CA04994E08F);
		// m_LocalClient = localClient;
		LocalClient_tCEC0096B13C433140FD4C09424CE345B28FE3C86 * L_0 = ___localClient0;
		__this->set_m_LocalClient_19(L_0);
		// }
		return;
	}
}
// System.Boolean UnityEngine.Networking.ULocalConnectionToClient::Send(System.Int16,UnityEngine.Networking.MessageBase)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ULocalConnectionToClient_Send_m048B6408A13F9DCE12CFC83E8AB202094D8751EA (ULocalConnectionToClient_t7AF7EBF2BEC3714F75EF894035BFAE9E6F9561A8 * __this, int16_t ___msgType0, MessageBase_t2EA42B01AD6A5F36EAF84BE623801951B9F55416 * ___msg1, const RuntimeMethod* method)
{
	{
		// m_LocalClient.InvokeHandlerOnClient(msgType, msg, Channels.DefaultReliable);
		LocalClient_tCEC0096B13C433140FD4C09424CE345B28FE3C86 * L_0 = __this->get_m_LocalClient_19();
		int16_t L_1 = ___msgType0;
		MessageBase_t2EA42B01AD6A5F36EAF84BE623801951B9F55416 * L_2 = ___msg1;
		NullCheck(L_0);
		LocalClient_InvokeHandlerOnClient_mA31AB4C0AAE4A3B392F5D843904B1EBF614646ED(L_0, L_1, L_2, 0, /*hidden argument*/NULL);
		// return true;
		return (bool)1;
	}
}
// System.Boolean UnityEngine.Networking.ULocalConnectionToClient::SendUnreliable(System.Int16,UnityEngine.Networking.MessageBase)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ULocalConnectionToClient_SendUnreliable_m2C449C01E2EA6E4238B8DD0B5027CFB8C76C3280 (ULocalConnectionToClient_t7AF7EBF2BEC3714F75EF894035BFAE9E6F9561A8 * __this, int16_t ___msgType0, MessageBase_t2EA42B01AD6A5F36EAF84BE623801951B9F55416 * ___msg1, const RuntimeMethod* method)
{
	{
		// m_LocalClient.InvokeHandlerOnClient(msgType, msg, Channels.DefaultUnreliable);
		LocalClient_tCEC0096B13C433140FD4C09424CE345B28FE3C86 * L_0 = __this->get_m_LocalClient_19();
		int16_t L_1 = ___msgType0;
		MessageBase_t2EA42B01AD6A5F36EAF84BE623801951B9F55416 * L_2 = ___msg1;
		NullCheck(L_0);
		LocalClient_InvokeHandlerOnClient_mA31AB4C0AAE4A3B392F5D843904B1EBF614646ED(L_0, L_1, L_2, 1, /*hidden argument*/NULL);
		// return true;
		return (bool)1;
	}
}
// System.Boolean UnityEngine.Networking.ULocalConnectionToClient::SendByChannel(System.Int16,UnityEngine.Networking.MessageBase,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ULocalConnectionToClient_SendByChannel_mD555BC6210CC4992D2BBCCB686FB14479805E3FB (ULocalConnectionToClient_t7AF7EBF2BEC3714F75EF894035BFAE9E6F9561A8 * __this, int16_t ___msgType0, MessageBase_t2EA42B01AD6A5F36EAF84BE623801951B9F55416 * ___msg1, int32_t ___channelId2, const RuntimeMethod* method)
{
	{
		// m_LocalClient.InvokeHandlerOnClient(msgType, msg, channelId);
		LocalClient_tCEC0096B13C433140FD4C09424CE345B28FE3C86 * L_0 = __this->get_m_LocalClient_19();
		int16_t L_1 = ___msgType0;
		MessageBase_t2EA42B01AD6A5F36EAF84BE623801951B9F55416 * L_2 = ___msg1;
		int32_t L_3 = ___channelId2;
		NullCheck(L_0);
		LocalClient_InvokeHandlerOnClient_mA31AB4C0AAE4A3B392F5D843904B1EBF614646ED(L_0, L_1, L_2, L_3, /*hidden argument*/NULL);
		// return true;
		return (bool)1;
	}
}
// System.Boolean UnityEngine.Networking.ULocalConnectionToClient::SendBytes(System.Byte[],System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ULocalConnectionToClient_SendBytes_m661178DF258F1A9276B9CB3856B5D701C0327C83 (ULocalConnectionToClient_t7AF7EBF2BEC3714F75EF894035BFAE9E6F9561A8 * __this, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___bytes0, int32_t ___numBytes1, int32_t ___channelId2, const RuntimeMethod* method)
{
	{
		// m_LocalClient.InvokeBytesOnClient(bytes, channelId);
		LocalClient_tCEC0096B13C433140FD4C09424CE345B28FE3C86 * L_0 = __this->get_m_LocalClient_19();
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_1 = ___bytes0;
		int32_t L_2 = ___channelId2;
		NullCheck(L_0);
		LocalClient_InvokeBytesOnClient_m1EFDE0A0688D78F165E5340E2C399CC47269866E(L_0, L_1, L_2, /*hidden argument*/NULL);
		// return true;
		return (bool)1;
	}
}
// System.Boolean UnityEngine.Networking.ULocalConnectionToClient::SendWriter(UnityEngine.Networking.NetworkWriter,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ULocalConnectionToClient_SendWriter_m3FD18A3FC1294C2F9C9D1C50904562B00C84BC03 (ULocalConnectionToClient_t7AF7EBF2BEC3714F75EF894035BFAE9E6F9561A8 * __this, NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * ___writer0, int32_t ___channelId1, const RuntimeMethod* method)
{
	{
		// m_LocalClient.InvokeBytesOnClient(writer.AsArray(), channelId);
		LocalClient_tCEC0096B13C433140FD4C09424CE345B28FE3C86 * L_0 = __this->get_m_LocalClient_19();
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_1 = ___writer0;
		NullCheck(L_1);
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_2 = NetworkWriter_AsArray_mE90AC762796F17DD398523A8C230DD9B2E2373D5(L_1, /*hidden argument*/NULL);
		int32_t L_3 = ___channelId1;
		NullCheck(L_0);
		LocalClient_InvokeBytesOnClient_m1EFDE0A0688D78F165E5340E2C399CC47269866E(L_0, L_2, L_3, /*hidden argument*/NULL);
		// return true;
		return (bool)1;
	}
}
// System.Void UnityEngine.Networking.ULocalConnectionToClient::GetStatsOut(System.Int32&,System.Int32&,System.Int32&,System.Int32&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ULocalConnectionToClient_GetStatsOut_mAEE75AE8166F78A6CDC77C74E114BAE4617FC17F (ULocalConnectionToClient_t7AF7EBF2BEC3714F75EF894035BFAE9E6F9561A8 * __this, int32_t* ___numMsgs0, int32_t* ___numBufferedMsgs1, int32_t* ___numBytes2, int32_t* ___lastBufferedPerSecond3, const RuntimeMethod* method)
{
	{
		// numMsgs = 0;
		int32_t* L_0 = ___numMsgs0;
		*((int32_t*)L_0) = (int32_t)0;
		// numBufferedMsgs = 0;
		int32_t* L_1 = ___numBufferedMsgs1;
		*((int32_t*)L_1) = (int32_t)0;
		// numBytes = 0;
		int32_t* L_2 = ___numBytes2;
		*((int32_t*)L_2) = (int32_t)0;
		// lastBufferedPerSecond = 0;
		int32_t* L_3 = ___lastBufferedPerSecond3;
		*((int32_t*)L_3) = (int32_t)0;
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.ULocalConnectionToClient::GetStatsIn(System.Int32&,System.Int32&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ULocalConnectionToClient_GetStatsIn_mA7E0251CFDA47E11660B58055784016A239F05E3 (ULocalConnectionToClient_t7AF7EBF2BEC3714F75EF894035BFAE9E6F9561A8 * __this, int32_t* ___numMsgs0, int32_t* ___numBytes1, const RuntimeMethod* method)
{
	{
		// numMsgs = 0;
		int32_t* L_0 = ___numMsgs0;
		*((int32_t*)L_0) = (int32_t)0;
		// numBytes = 0;
		int32_t* L_1 = ___numBytes1;
		*((int32_t*)L_1) = (int32_t)0;
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
// System.Void UnityEngine.Networking.ULocalConnectionToServer::.ctor(UnityEngine.Networking.NetworkServer)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ULocalConnectionToServer__ctor_mC9A4D762519369638D6A6ED1A27C077D17E36CFA (ULocalConnectionToServer_tE6E34057F329C3E0E703C6F095DF82B0270557B8 * __this, NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1 * ___localServer0, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ULocalConnectionToServer__ctor_mC9A4D762519369638D6A6ED1A27C077D17E36CFA_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public ULocalConnectionToServer(NetworkServer localServer)
		NetworkConnection__ctor_mDD96E228FE96C836C690ADBFDC26C3FFDA31CEC9(__this, /*hidden argument*/NULL);
		// address = "localServer";
		((NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA *)__this)->set_address_14(_stringLiteral8A90E4187AA462FD7BCD9E2521B1C4F09372DBAF);
		// m_LocalServer = localServer;
		NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1 * L_0 = ___localServer0;
		__this->set_m_LocalServer_19(L_0);
		// }
		return;
	}
}
// System.Boolean UnityEngine.Networking.ULocalConnectionToServer::Send(System.Int16,UnityEngine.Networking.MessageBase)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ULocalConnectionToServer_Send_m450408EE6C067B1D77422B22151A8E38286C5B53 (ULocalConnectionToServer_tE6E34057F329C3E0E703C6F095DF82B0270557B8 * __this, int16_t ___msgType0, MessageBase_t2EA42B01AD6A5F36EAF84BE623801951B9F55416 * ___msg1, const RuntimeMethod* method)
{
	{
		// return m_LocalServer.InvokeHandlerOnServer(this, msgType, msg, Channels.DefaultReliable);
		NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1 * L_0 = __this->get_m_LocalServer_19();
		int16_t L_1 = ___msgType0;
		MessageBase_t2EA42B01AD6A5F36EAF84BE623801951B9F55416 * L_2 = ___msg1;
		NullCheck(L_0);
		bool L_3 = NetworkServer_InvokeHandlerOnServer_m24971E0FB8CA3BD5D9557EC82349C19B5D369CE2(L_0, __this, L_1, L_2, 0, /*hidden argument*/NULL);
		return L_3;
	}
}
// System.Boolean UnityEngine.Networking.ULocalConnectionToServer::SendUnreliable(System.Int16,UnityEngine.Networking.MessageBase)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ULocalConnectionToServer_SendUnreliable_mE11D489E53632E3FBA9FFACDD54EEB56485DCEC8 (ULocalConnectionToServer_tE6E34057F329C3E0E703C6F095DF82B0270557B8 * __this, int16_t ___msgType0, MessageBase_t2EA42B01AD6A5F36EAF84BE623801951B9F55416 * ___msg1, const RuntimeMethod* method)
{
	{
		// return m_LocalServer.InvokeHandlerOnServer(this, msgType, msg, Channels.DefaultUnreliable);
		NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1 * L_0 = __this->get_m_LocalServer_19();
		int16_t L_1 = ___msgType0;
		MessageBase_t2EA42B01AD6A5F36EAF84BE623801951B9F55416 * L_2 = ___msg1;
		NullCheck(L_0);
		bool L_3 = NetworkServer_InvokeHandlerOnServer_m24971E0FB8CA3BD5D9557EC82349C19B5D369CE2(L_0, __this, L_1, L_2, 1, /*hidden argument*/NULL);
		return L_3;
	}
}
// System.Boolean UnityEngine.Networking.ULocalConnectionToServer::SendByChannel(System.Int16,UnityEngine.Networking.MessageBase,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ULocalConnectionToServer_SendByChannel_m90693B292279DE7B920F4A0E68F5B0F62A2DEF02 (ULocalConnectionToServer_tE6E34057F329C3E0E703C6F095DF82B0270557B8 * __this, int16_t ___msgType0, MessageBase_t2EA42B01AD6A5F36EAF84BE623801951B9F55416 * ___msg1, int32_t ___channelId2, const RuntimeMethod* method)
{
	{
		// return m_LocalServer.InvokeHandlerOnServer(this, msgType, msg, channelId);
		NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1 * L_0 = __this->get_m_LocalServer_19();
		int16_t L_1 = ___msgType0;
		MessageBase_t2EA42B01AD6A5F36EAF84BE623801951B9F55416 * L_2 = ___msg1;
		int32_t L_3 = ___channelId2;
		NullCheck(L_0);
		bool L_4 = NetworkServer_InvokeHandlerOnServer_m24971E0FB8CA3BD5D9557EC82349C19B5D369CE2(L_0, __this, L_1, L_2, L_3, /*hidden argument*/NULL);
		return L_4;
	}
}
// System.Boolean UnityEngine.Networking.ULocalConnectionToServer::SendBytes(System.Byte[],System.Int32,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ULocalConnectionToServer_SendBytes_mB46A168719C69599AC91617C61D15D495CE498E6 (ULocalConnectionToServer_tE6E34057F329C3E0E703C6F095DF82B0270557B8 * __this, ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ___bytes0, int32_t ___numBytes1, int32_t ___channelId2, const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ULocalConnectionToServer_SendBytes_mB46A168719C69599AC91617C61D15D495CE498E6_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// if (numBytes <= 0)
		int32_t L_0 = ___numBytes1;
		if ((((int32_t)L_0) > ((int32_t)0)))
		{
			goto IL_0017;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("LocalConnection:SendBytes cannot send zero bytes"); }
		IL2CPP_RUNTIME_CLASS_INIT(LogFilter_t5202A297E770086F7954B8D6703BAC03C22654ED_il2cpp_TypeInfo_var);
		bool L_1 = LogFilter_get_logError_mD404500EEB2968A3CF190DB1EB6CA9A26135A21F(/*hidden argument*/NULL);
		if (!L_1)
		{
			goto IL_0015;
		}
	}
	{
		// if (LogFilter.logError) { Debug.LogError("LocalConnection:SendBytes cannot send zero bytes"); }
		IL2CPP_RUNTIME_CLASS_INIT(Debug_t7B5FCB117E2FD63B6838BC52821B252E2BFB61C4_il2cpp_TypeInfo_var);
		Debug_LogError_m3BCF9B78263152261565DCA9DB7D55F0C391ED29(_stringLiteral13F4B303180B12CAF8F069E93D7C4FAF969D3BC4, /*hidden argument*/NULL);
	}

IL_0015:
	{
		// return false;
		return (bool)0;
	}

IL_0017:
	{
		// return m_LocalServer.InvokeBytes(this, bytes, numBytes, channelId);
		NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1 * L_2 = __this->get_m_LocalServer_19();
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_3 = ___bytes0;
		int32_t L_4 = ___numBytes1;
		int32_t L_5 = ___channelId2;
		NullCheck(L_2);
		bool L_6 = NetworkServer_InvokeBytes_mE35B20779E0DA27C27860D3F7EB24FC7D2E35754(L_2, __this, L_3, L_4, L_5, /*hidden argument*/NULL);
		return L_6;
	}
}
// System.Boolean UnityEngine.Networking.ULocalConnectionToServer::SendWriter(UnityEngine.Networking.NetworkWriter,System.Int32)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool ULocalConnectionToServer_SendWriter_m71F6D32058B38382426B0933E25E8121C8049C71 (ULocalConnectionToServer_tE6E34057F329C3E0E703C6F095DF82B0270557B8 * __this, NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * ___writer0, int32_t ___channelId1, const RuntimeMethod* method)
{
	{
		// return m_LocalServer.InvokeBytes(this, writer.AsArray(), (short)writer.AsArray().Length, channelId);
		NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1 * L_0 = __this->get_m_LocalServer_19();
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_1 = ___writer0;
		NullCheck(L_1);
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_2 = NetworkWriter_AsArray_mE90AC762796F17DD398523A8C230DD9B2E2373D5(L_1, /*hidden argument*/NULL);
		NetworkWriter_t9BE861BDE3F59F374D83A1E4CC697C73003FF030 * L_3 = ___writer0;
		NullCheck(L_3);
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_4 = NetworkWriter_AsArray_mE90AC762796F17DD398523A8C230DD9B2E2373D5(L_3, /*hidden argument*/NULL);
		NullCheck(L_4);
		int32_t L_5 = ___channelId1;
		NullCheck(L_0);
		bool L_6 = NetworkServer_InvokeBytes_mE35B20779E0DA27C27860D3F7EB24FC7D2E35754(L_0, __this, L_2, (((int16_t)((int16_t)(((int32_t)((int32_t)(((RuntimeArray*)L_4)->max_length))))))), L_5, /*hidden argument*/NULL);
		return L_6;
	}
}
// System.Void UnityEngine.Networking.ULocalConnectionToServer::GetStatsOut(System.Int32&,System.Int32&,System.Int32&,System.Int32&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ULocalConnectionToServer_GetStatsOut_m6A36C78F279B23D01DD653FB1F02FF45DB56B04F (ULocalConnectionToServer_tE6E34057F329C3E0E703C6F095DF82B0270557B8 * __this, int32_t* ___numMsgs0, int32_t* ___numBufferedMsgs1, int32_t* ___numBytes2, int32_t* ___lastBufferedPerSecond3, const RuntimeMethod* method)
{
	{
		// numMsgs = 0;
		int32_t* L_0 = ___numMsgs0;
		*((int32_t*)L_0) = (int32_t)0;
		// numBufferedMsgs = 0;
		int32_t* L_1 = ___numBufferedMsgs1;
		*((int32_t*)L_1) = (int32_t)0;
		// numBytes = 0;
		int32_t* L_2 = ___numBytes2;
		*((int32_t*)L_2) = (int32_t)0;
		// lastBufferedPerSecond = 0;
		int32_t* L_3 = ___lastBufferedPerSecond3;
		*((int32_t*)L_3) = (int32_t)0;
		// }
		return;
	}
}
// System.Void UnityEngine.Networking.ULocalConnectionToServer::GetStatsIn(System.Int32&,System.Int32&)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void ULocalConnectionToServer_GetStatsIn_m73A6766DA51E93B0634853DF9623CE162DE1EA78 (ULocalConnectionToServer_tE6E34057F329C3E0E703C6F095DF82B0270557B8 * __this, int32_t* ___numMsgs0, int32_t* ___numBytes1, const RuntimeMethod* method)
{
	{
		// numMsgs = 0;
		int32_t* L_0 = ___numMsgs0;
		*((int32_t*)L_0) = (int32_t)0;
		// numBytes = 0;
		int32_t* L_1 = ___numBytes1;
		*((int32_t*)L_1) = (int32_t)0;
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
// System.Void UnityEngine.Networking.UnSpawnDelegate::.ctor(System.Object,System.IntPtr)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnSpawnDelegate__ctor_mC7BE11555411F55D2A4D6EF099827AA1C951281C (UnSpawnDelegate_tDC1AD5AA3602EB703F4FA34792B4D4075582AE19 * __this, RuntimeObject * ___object0, intptr_t ___method1, const RuntimeMethod* method)
{
	__this->set_method_ptr_0(il2cpp_codegen_get_method_pointer((RuntimeMethod*)___method1));
	__this->set_method_3(___method1);
	__this->set_m_target_2(___object0);
}
// System.Void UnityEngine.Networking.UnSpawnDelegate::Invoke(UnityEngine.GameObject)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnSpawnDelegate_Invoke_m5892BE95FBD86A3455A00B8CA227726E4D78DA93 (UnSpawnDelegate_tDC1AD5AA3602EB703F4FA34792B4D4075582AE19 * __this, GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * ___spawned0, const RuntimeMethod* method)
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
				typedef void (*FunctionPointerType) (GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F *, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___spawned0, targetMethod);
			}
			else
			{
				// closed
				typedef void (*FunctionPointerType) (void*, GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F *, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___spawned0, targetMethod);
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
						GenericInterfaceActionInvoker0::Invoke(targetMethod, ___spawned0);
					else
						GenericVirtActionInvoker0::Invoke(targetMethod, ___spawned0);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker0::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), ___spawned0);
					else
						VirtActionInvoker0::Invoke(il2cpp_codegen_method_get_slot(targetMethod), ___spawned0);
				}
			}
			else
			{
				typedef void (*FunctionPointerType) (GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F *, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(___spawned0, targetMethod);
			}
		}
		else
		{
			// closed
			if (il2cpp_codegen_method_is_virtual(targetMethod) && !il2cpp_codegen_object_is_of_sealed_type(targetThis) && il2cpp_codegen_delegate_has_invoker((Il2CppDelegate*)__this))
			{
				if (targetThis == NULL)
				{
					typedef void (*FunctionPointerType) (GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F *, const RuntimeMethod*);
					((FunctionPointerType)targetMethodPointer)(___spawned0, targetMethod);
				}
				else if (il2cpp_codegen_method_is_generic_instance(targetMethod))
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						GenericInterfaceActionInvoker1< GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * >::Invoke(targetMethod, targetThis, ___spawned0);
					else
						GenericVirtActionInvoker1< GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * >::Invoke(targetMethod, targetThis, ___spawned0);
				}
				else
				{
					if (il2cpp_codegen_method_is_interface_method(targetMethod))
						InterfaceActionInvoker1< GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), il2cpp_codegen_method_get_declaring_type(targetMethod), targetThis, ___spawned0);
					else
						VirtActionInvoker1< GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * >::Invoke(il2cpp_codegen_method_get_slot(targetMethod), targetThis, ___spawned0);
				}
			}
			else
			{
				typedef void (*FunctionPointerType) (void*, GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F *, const RuntimeMethod*);
				((FunctionPointerType)targetMethodPointer)(targetThis, ___spawned0, targetMethod);
			}
		}
	}
}
// System.IAsyncResult UnityEngine.Networking.UnSpawnDelegate::BeginInvoke(UnityEngine.GameObject,System.AsyncCallback,System.Object)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* UnSpawnDelegate_BeginInvoke_mB494EA9386D52614D172803BBD524DEC822915E6 (UnSpawnDelegate_tDC1AD5AA3602EB703F4FA34792B4D4075582AE19 * __this, GameObject_tBD1244AD56B4E59AAD76E5E7C9282EC5CE434F0F * ___spawned0, AsyncCallback_t3F3DA3BEDAEE81DD1D24125DF8EB30E85EE14DA4 * ___callback1, RuntimeObject * ___object2, const RuntimeMethod* method)
{
	void *__d_args[2] = {0};
	__d_args[0] = ___spawned0;
	return (RuntimeObject*)il2cpp_codegen_delegate_begin_invoke((RuntimeDelegate*)__this, __d_args, (RuntimeDelegate*)___callback1, (RuntimeObject*)___object2);
}
// System.Void UnityEngine.Networking.UnSpawnDelegate::EndInvoke(System.IAsyncResult)
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void UnSpawnDelegate_EndInvoke_m2C73C6DB70A31F17CC4095648754253BF3601BA5 (UnSpawnDelegate_tDC1AD5AA3602EB703F4FA34792B4D4075582AE19 * __this, RuntimeObject* ___result0, const RuntimeMethod* method)
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
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR float NetworkTransformChild_get_movementThreshold_m9BED81E541443BA95A2DDDF7465386F0E4F5639A_inline (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	{
		// public float                                movementThreshold { get { return m_MovementThreshold; } set { m_MovementThreshold = value; } }
		float L_0 = __this->get_m_MovementThreshold_16();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR void NetworkTransformChild_set_movementThreshold_m93D1B2916BC9B686B9F40C755A0AADCE6E54AB93_inline (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, float ___value0, const RuntimeMethod* method)
{
	{
		// public float                                movementThreshold { get { return m_MovementThreshold; } set { m_MovementThreshold = value; } }
		float L_0 = ___value0;
		__this->set_m_MovementThreshold_16(L_0);
		// public float                                movementThreshold { get { return m_MovementThreshold; } set { m_MovementThreshold = value; } }
		return;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR float NetworkTransformChild_get_interpolateRotation_m9169822905990E0E3C3531F5812DC25FBE4C06BA_inline (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	{
		// public float                                interpolateRotation { get { return m_InterpolateRotation; } set { m_InterpolateRotation = value; } }
		float L_0 = __this->get_m_InterpolateRotation_17();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR void NetworkTransformChild_set_interpolateRotation_mF72DC382026B2C763A63F0FB8D565275ECAEC4DF_inline (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, float ___value0, const RuntimeMethod* method)
{
	{
		// public float                                interpolateRotation { get { return m_InterpolateRotation; } set { m_InterpolateRotation = value; } }
		float L_0 = ___value0;
		__this->set_m_InterpolateRotation_17(L_0);
		// public float                                interpolateRotation { get { return m_InterpolateRotation; } set { m_InterpolateRotation = value; } }
		return;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR float NetworkTransformChild_get_interpolateMovement_m27491C1C3805AE420F584C937DC7DDFDB4A98E74_inline (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	{
		// public float                                interpolateMovement { get { return m_InterpolateMovement; } set { m_InterpolateMovement = value; } }
		float L_0 = __this->get_m_InterpolateMovement_18();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR void NetworkTransformChild_set_interpolateMovement_m94A0DB8134ACE98F427C12CFA1057CAA9370CF44_inline (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, float ___value0, const RuntimeMethod* method)
{
	{
		// public float                                interpolateMovement { get { return m_InterpolateMovement; } set { m_InterpolateMovement = value; } }
		float L_0 = ___value0;
		__this->set_m_InterpolateMovement_18(L_0);
		// public float                                interpolateMovement { get { return m_InterpolateMovement; } set { m_InterpolateMovement = value; } }
		return;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR uint32_t NetworkBehaviour_get_syncVarDirtyBits_mD53C3F852C533A88A2312E7AFF9883658DDEEB0C_inline (NetworkBehaviour_tE0C48D0A9ED8AC3977CAEF5B8090089CD544D19C * __this, const RuntimeMethod* method)
{
	{
		// protected uint syncVarDirtyBits { get { return m_SyncVarDirtyBits; } }
		uint32_t L_0 = __this->get_m_SyncVarDirtyBits_4();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t NetworkTransformChild_get_syncRotationAxis_m12199E8BCADC5098C84E6F2E6A8534424FD80979_inline (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	{
		// public NetworkTransform.AxisSyncMode        syncRotationAxis { get { return m_SyncRotationAxis; } set { m_SyncRotationAxis = value; } }
		int32_t L_0 = __this->get_m_SyncRotationAxis_14();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t NetworkTransformChild_get_rotationSyncCompression_m014CA812E4BB0DBF2DF856CB96E40FBED022239B_inline (NetworkTransformChild_tFC794CABDCF9ABF9335CBB1E0B3489F96A60206E * __this, const RuntimeMethod* method)
{
	{
		// public NetworkTransform.CompressionSyncMode rotationSyncCompression { get { return m_RotationSyncCompression; } set { m_RotationSyncCompression = value; } }
		int32_t L_0 = __this->get_m_RotationSyncCompression_15();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR bool NetworkServer_get_active_m3FAC75ABF32D586F6C8DB6B4237DC40300FB2257_inline (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkServer_get_active_m3FAC75ABF32D586F6C8DB6B4237DC40300FB2257com_unity_multiplayerU2Dhlapi_Runtime3_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public static bool active { get { return s_Active; } }
		IL2CPP_RUNTIME_CLASS_INIT(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1_il2cpp_TypeInfo_var);
		bool L_0 = ((NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1_StaticFields*)il2cpp_codegen_static_fields_for(NetworkServer_tFD62C268FC6F01624A5989BFC1D0DD689A66B4A1_il2cpp_TypeInfo_var))->get_s_Active_0();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR bool NetworkClient_get_active_m31953DC487641BC5D9BEB0EB4DE32462AC4A8BD1_inline (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (NetworkClient_get_active_m31953DC487641BC5D9BEB0EB4DE32462AC4A8BD1com_unity_multiplayerU2Dhlapi_Runtime3_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public static bool active { get { return s_IsActive; } }
		IL2CPP_RUNTIME_CLASS_INIT(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F_il2cpp_TypeInfo_var);
		bool L_0 = ((NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F_StaticFields*)il2cpp_codegen_static_fields_for(NetworkClient_t33B95FF43955FEC9083CA7222A143777B8B79F0F_il2cpp_TypeInfo_var))->get_s_IsActive_3();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * ClientScene_get_readyConnection_mACB67AD0151B2507CF8BD5D7D8B806C470E49998_inline (const RuntimeMethod* method)
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_method (ClientScene_get_readyConnection_mACB67AD0151B2507CF8BD5D7D8B806C470E49998com_unity_multiplayerU2Dhlapi_Runtime3_MetadataUsageId);
		s_Il2CppMethodInitialized = true;
	}
	{
		// public static NetworkConnection readyConnection { get { return s_ReadyConnection; }}
		IL2CPP_RUNTIME_CLASS_INIT(ClientScene_t0A10B1F436A5AA8D5FC9B18C9ED0B32008809A3E_il2cpp_TypeInfo_var);
		NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * L_0 = ((ClientScene_t0A10B1F436A5AA8D5FC9B18C9ED0B32008809A3E_StaticFields*)il2cpp_codegen_static_fields_for(ClientScene_t0A10B1F436A5AA8D5FC9B18C9ED0B32008809A3E_il2cpp_TypeInfo_var))->get_s_ReadyConnection_1();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990 * NetworkConnection_get_clientOwnedObjects_m0CC0D90CD318855211AA194D67DB4A07E4694D22_inline (NetworkConnection_t56E90DAE06B07A4A3233611CC9C0CBCD0A1CAFBA * __this, const RuntimeMethod* method)
{
	{
		// public HashSet<NetworkInstanceId> clientOwnedObjects { get { return m_ClientOwnedObjects; } }
		HashSet_1_t5328A401EC9FEDAF4F16B55D2D8EAEB6EA33C990 * L_0 = __this->get_m_ClientOwnedObjects_7();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  NetworkTransform_get_targetSyncPosition_m8D2DCE0C4C4EDE2729E3323218669E433952A446_inline (NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * __this, const RuntimeMethod* method)
{
	{
		// public Vector3              targetSyncPosition { get { return m_TargetSyncPosition; } }
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_0 = __this->get_m_TargetSyncPosition_26();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR Rigidbody_tE0A58EE5A1F7DC908EFFB4F0D795AC9552A750A5 * NetworkTransform_get_rigidbody3D_m2F059AC7FE4AE29073DA4FB4D6D9719A35245DEB_inline (NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * __this, const RuntimeMethod* method)
{
	{
		// public Rigidbody            rigidbody3D { get { return m_RigidBody3D; } }
		Rigidbody_tE0A58EE5A1F7DC908EFFB4F0D795AC9552A750A5 * L_0 = __this->get_m_RigidBody3D_22();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  NetworkTransform_get_targetSyncVelocity_m7C47913B3EBFDC866349F5C091C439D255B75CFB_inline (NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * __this, const RuntimeMethod* method)
{
	{
		// public Vector3              targetSyncVelocity { get { return m_TargetSyncVelocity; } }
		Vector3_tDCF05E21F632FE2BA260C06E0D10CA81513E6720  L_0 = __this->get_m_TargetSyncVelocity_27();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR Rigidbody2D_tBDC6900A76D3C47E291446FF008D02B817C81CDE * NetworkTransform_get_rigidbody2D_mC7614E0AE776DEE2D14FCC7E41D90CD5D498F765_inline (NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * __this, const RuntimeMethod* method)
{
	{
		// new public Rigidbody2D          rigidbody2D { get { return m_RigidBody2D; } }
		Rigidbody2D_tBDC6900A76D3C47E291446FF008D02B817C81CDE * L_0 = __this->get_m_RigidBody2D_23();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  NetworkTransform_get_targetSyncRotation3D_m6418875DB7CC2500B5E0778D6BC890D2583B4DF8_inline (NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * __this, const RuntimeMethod* method)
{
	{
		// public Quaternion           targetSyncRotation3D { get { return m_TargetSyncRotation3D; } }
		Quaternion_t319F3319A7D43FFA5D819AD6C0A98851F0095357  L_0 = __this->get_m_TargetSyncRotation3D_29();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR float NetworkTransform_get_targetSyncRotation2D_mE1F4E6611853B634322EE9EF4517E7E2AF169BEA_inline (NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * __this, const RuntimeMethod* method)
{
	{
		// public float                targetSyncRotation2D { get { return m_TargetSyncRotation2D; } }
		float L_0 = __this->get_m_TargetSyncRotation2D_31();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR float NetworkTransform_get_lastSyncTime_mD8AEBC7EDA370ACB0A222BF622BD95C54EBD6C9E_inline (NetworkTransform_t45A27276B454DE644FF4F2CD0DDBAC26375E639F * __this, const RuntimeMethod* method)
{
	{
		// public float                lastSyncTime { get { return m_LastClientSyncTime; } }
		float L_0 = __this->get_m_LastClientSyncTime_33();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR uint32_t NetBuffer_get_Position_m1F0C4B8C3EDCCB0D65CE51B4709FDAF2017938AB_inline (NetBuffer_t2BA43CF3688776F372BECD54D28F90CB0559B36C * __this, const RuntimeMethod* method)
{
	{
		// public uint Position { get { return m_Pos; } }
		uint32_t L_0 = __this->get_m_Pos_1();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR uint32_t NetworkInstanceId_get_Value_m63FB00D0A8272D39B6C7F7C490A8190F0E95F67F_inline (NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615 * __this, const RuntimeMethod* method)
{
	{
		// public uint Value { get { return m_Value; } }
		uint32_t L_0 = __this->get_m_Value_0();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR uint32_t NetworkSceneId_get_Value_m917E56DBEDC97969F7AC83B42A1F53C21DC1A9A3_inline (NetworkSceneId_t462EC62A23A1B7AF60637C48CD916A09BC493340 * __this, const RuntimeMethod* method)
{
	{
		// public uint Value { get { return m_Value; } }
		uint32_t L_0 = __this->get_m_Value_0();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR int32_t String_get_Length_mD48C8A16A5CF1914F330DCE82D9BE15C3BEDD018_inline (String_t* __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = __this->get_m_stringLength_0();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline  IL2CPP_METHOD_ATTR NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  NetworkIdentity_get_netId_m22EB7CD04E2633FFAF99093749F79816B2BC9F28_inline (NetworkIdentity_t764E9C8A578DEF667FDCB3D1171A4B0DDF38069B * __this, const RuntimeMethod* method)
{
	{
		// public NetworkInstanceId netId { get { return m_NetId; } }
		NetworkInstanceId_tB6492FD2B3B2062582F787801BF7C0457271F615  L_0 = __this->get_m_NetId_11();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR int32_t ArraySegment_1_get_Count_m02387DADA172F909FD346559D93990E990E05352_gshared_inline (ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA * __this, const RuntimeMethod* method)
{
	{
		int32_t L_0 = (int32_t)__this->get__count_2();
		return L_0;
	}
}
IL2CPP_EXTERN_C inline IL2CPP_METHOD_ATTR ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* ArraySegment_1_get_Array_m41D93EFB7EAB3081C0A27ED9891E7177F5F361B7_gshared_inline (ArraySegment_1_t5B17204266E698CC035E2A7F6435A4F78286D0FA * __this, const RuntimeMethod* method)
{
	{
		ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821* L_0 = (ByteU5BU5D_tD06FDBE8142446525DF1C40351D523A228373821*)__this->get__array_0();
		return L_0;
	}
}
