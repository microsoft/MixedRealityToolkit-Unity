#pragma once

// Unity native plugin API
// Compatible with C99

#if defined(__CYGWIN32__)
	#define UNITY_INTERFACE_API __stdcall
	#define UNITY_INTERFACE_EXPORT __declspec(dllexport)
#elif defined(WIN32) || defined(_WIN32) || defined(__WIN32__) || defined(_WIN64) || defined(WINAPI_FAMILY)
	#define UNITY_INTERFACE_API __stdcall
	#define UNITY_INTERFACE_EXPORT __declspec(dllexport)
#elif defined(__MACH__) || defined(__ANDROID__) || defined(__linux__) || defined(__QNX__)
	#define UNITY_INTERFACE_API
	#define UNITY_INTERFACE_EXPORT
#else
	#define UNITY_INTERFACE_API
	#define UNITY_INTERFACE_EXPORT
#endif



// Unity Interface GUID
// Ensures cross plugin uniqueness.
//
// Template specialization is used to produce a means of looking up a GUID from it's payload type at compile time.
// The net result should compile down to passing around the GUID.
//
// UNITY_REGISTER_INTERFACE_GUID should be placed in the header file of any payload definition outside of all namespaces.
// The payload structure and the registration GUID are all that is required to expose the interface to other systems.
struct UnityInterfaceGUID
{
#ifdef __cplusplus
	UnityInterfaceGUID(unsigned long long high, unsigned long long low)
	: m_GUIDHigh(high)
	, m_GUIDLow(low)
	{
	}

	UnityInterfaceGUID(const UnityInterfaceGUID& other)
	{
		m_GUIDHigh = other.m_GUIDHigh;
		m_GUIDLow  = other.m_GUIDLow;
	}

	UnityInterfaceGUID& operator=(const UnityInterfaceGUID& other)
	{
		m_GUIDHigh = other.m_GUIDHigh;
		m_GUIDLow  = other.m_GUIDLow;
		return *this;
	}

	bool Equals(const UnityInterfaceGUID& other)   const { return m_GUIDHigh == other.m_GUIDHigh && m_GUIDLow == other.m_GUIDLow; }
	bool LessThan(const UnityInterfaceGUID& other) const { return m_GUIDHigh < other.m_GUIDHigh || (m_GUIDHigh == other.m_GUIDHigh && m_GUIDLow < other.m_GUIDLow); }
#endif
	unsigned long long m_GUIDHigh;
	unsigned long long m_GUIDLow;
};
#ifdef __cplusplus
inline bool operator==(const UnityInterfaceGUID& left, const UnityInterfaceGUID& right) { return left.Equals(right); }
inline bool operator!=(const UnityInterfaceGUID& left, const UnityInterfaceGUID& right) { return !left.Equals(right); }
inline bool operator< (const UnityInterfaceGUID& left, const UnityInterfaceGUID& right) { return left.LessThan(right); }
inline bool operator> (const UnityInterfaceGUID& left, const UnityInterfaceGUID& right) { return right.LessThan(left); }
inline bool operator>=(const UnityInterfaceGUID& left, const UnityInterfaceGUID& right) { return !operator< (left,right); }
inline bool operator<=(const UnityInterfaceGUID& left, const UnityInterfaceGUID& right) { return !operator> (left,right); }
#else
typedef struct UnityInterfaceGUID UnityInterfaceGUID;
#endif



#define UNITY_GET_INTERFACE_GUID(TYPE) TYPE##_GUID
#define UNITY_GET_INTERFACE(INTERFACES, TYPE) (TYPE*)INTERFACES->GetInterface(UNITY_GET_INTERFACE_GUID(TYPE));

#ifdef __cplusplus
	#define UNITY_DECLARE_INTERFACE(NAME) \
	struct NAME : IUnityInterface

	template<typename TYPE>                                        \
	inline const UnityInterfaceGUID GetUnityInterfaceGUID();       \

	#define UNITY_REGISTER_INTERFACE_GUID(HASHH, HASHL, TYPE)      \
	const UnityInterfaceGUID TYPE##_GUID(HASHH, HASHL);            \
	template<>                                                     \
	inline const UnityInterfaceGUID GetUnityInterfaceGUID<TYPE>()  \
	{                                                              \
	    return UNITY_GET_INTERFACE_GUID(TYPE);                     \
	}
#else
	#define UNITY_DECLARE_INTERFACE(NAME) \
	typedef struct NAME NAME;             \
	struct NAME

	#define UNITY_REGISTER_INTERFACE_GUID(HASHH, HASHL, TYPE) \
	const UnityInterfaceGUID TYPE##_GUID = {HASHH, HASHL};
#endif



#ifdef __cplusplus
struct IUnityInterface
{
};
#else
typedef void IUnityInterface;
#endif



typedef struct IUnityInterfaces
{
	// Returns an interface matching the guid.
	// Returns nullptr if the given interface is unavailable in the active Unity runtime.
	IUnityInterface* (UNITY_INTERFACE_API * GetInterface)(UnityInterfaceGUID guid);

	// Registers a new interface.
	void (UNITY_INTERFACE_API * RegisterInterface)(UnityInterfaceGUID guid, IUnityInterface* ptr);

#ifdef __cplusplus
	// Helper for GetInterface.
	template <typename INTERFACE>
	INTERFACE* Get()
	{
		return static_cast<INTERFACE*>(GetInterface(GetUnityInterfaceGUID<INTERFACE>()));
	}

	// Helper for RegisterInterface.
	template <typename INTERFACE>
	void Register(IUnityInterface* ptr)
	{
		RegisterInterface(GetUnityInterfaceGUID<INTERFACE>(), ptr);
	}
#endif
} IUnityInterfaces;



#ifdef __cplusplus
extern "C" {
#endif

// If exported by a plugin, this function will be called when the plugin is loaded.
void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces* unityInterfaces);
// If exported by a plugin, this function will be called when the plugin is about to be unloaded.
void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload();

#ifdef __cplusplus
}
#endif
