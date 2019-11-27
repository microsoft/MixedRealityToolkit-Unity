#include "il2cpp-config.h"
#include "il2cpp-string-types.h"
#include "metadata/GenericMetadata.h"
#include "os/WindowsRuntime.h"
#include "utils/StringUtils.h"
#include "vm/Atomic.h"
#include "vm/Class.h"
#include "vm/COM.h"
#include "vm/ComObjectBase.h"
#include "vm/GenericClass.h"
#include "vm/MetadataCache.h"
#include "vm/WeakReference.h"

il2cpp_hresult_t STDCALL il2cpp::vm::ComObjectBase::GetIids(uint32_t* iidCount, Il2CppGuid** iids)
{
    *iidCount = 0;
    *iids = NULL;
    return IL2CPP_S_OK;
}

static inline bool CanPotentiallyBeBoxedToWindowsRuntime(const Il2CppClass* klass)
{
    if (il2cpp::vm::Class::IsInflated(klass))
        return false;

    if (il2cpp::vm::Class::IsValuetype(klass))
        return true;

    if (klass == il2cpp_defaults.string_class)
        return true;

    return false;
}

static inline const Il2CppClass* GetBoxedWindowsRuntimeClass(const Il2CppClass* typeDefinition, const Il2CppClass* genericArg)
{
    const Il2CppType* klass = &genericArg->byval_arg;
    const Il2CppGenericInst* inst = il2cpp::vm::MetadataCache::GetGenericInst(&klass, 1);
    Il2CppGenericClass* genericClass = il2cpp::metadata::GenericMetadata::GetGenericClass(typeDefinition, inst);
    return il2cpp::vm::GenericClass::GetClass(genericClass);
}

il2cpp_hresult_t STDCALL il2cpp::vm::ComObjectBase::GetRuntimeClassName(Il2CppHString* className)
{
    const Il2CppClass* objectClass = GetManagedObjectInline()->klass;
    if (il2cpp_defaults.ireference_class != NULL && CanPotentiallyBeBoxedToWindowsRuntime(objectClass))
    {
        // For value types/strings we're supposed to return the name of its boxed representation, i.e. Windows.Foundation.IReference`1<T>
        objectClass = GetBoxedWindowsRuntimeClass(il2cpp_defaults.ireference_class, objectClass);
    }
    else if (il2cpp_defaults.ireferencearray_class != NULL && objectClass->rank > 0)
    {
        // For arrays of value types/strings we're supposed to return the name of its boxed representation too, i.e. Windows.Foundation.IReferenceArray`1<T>
        const Il2CppClass* elementClass = objectClass->element_class;
        if (CanPotentiallyBeBoxedToWindowsRuntime(elementClass))
        {
            objectClass = GetBoxedWindowsRuntimeClass(il2cpp_defaults.ireferencearray_class, elementClass);
        }
        else if (elementClass == il2cpp_defaults.object_class || strcmp(elementClass->image->assembly->aname.name, "WindowsRuntimeMetadata") == 0)
        {
            // Object arrays can be boxed, but objects cannot, so we need to special case it
            // For object and WindowsRuntime classes arrays, we also return Windows.Foundation.IReferenceArray`1<Object>
            return os::WindowsRuntime::CreateHString(utils::StringView<Il2CppNativeChar>(IL2CPP_NATIVE_STRING("Windows.Foundation.IReferenceArray`1<Object>")), className);
        }
    }

    const char* name = MetadataCache::GetWindowsRuntimeClassName(objectClass);
    if (name == NULL)
    {
        *className = NULL;
        return IL2CPP_S_OK;
    }

    UTF16String nameUtf16 = utils::StringUtils::Utf8ToUtf16(name);
    return os::WindowsRuntime::CreateHString(utils::StringView<Il2CppChar>(nameUtf16.c_str(), nameUtf16.length()), className);
}

il2cpp_hresult_t STDCALL il2cpp::vm::ComObjectBase::GetTrustLevel(int32_t* trustLevel)
{
    *trustLevel = 0;
    return IL2CPP_S_OK;
}

Il2CppObject* STDCALL il2cpp::vm::ComObjectBase::GetManagedObject()
{
    return GetManagedObjectInline();
}

il2cpp_hresult_t STDCALL il2cpp::vm::ComObjectBase::GetUnmarshalClass(const Il2CppGuid& iid, void* object, uint32_t context, void* reserved, uint32_t flags, Il2CppGuid* clsid)
{
    Il2CppIMarshal* freeThreadedMarshaler;
    il2cpp_hresult_t hr = GetFreeThreadedMarshalerNoAddRef(&freeThreadedMarshaler);
    if (IL2CPP_HR_FAILED(hr))
        return hr;

    return freeThreadedMarshaler->GetUnmarshalClass(iid, object, context, reserved, flags, clsid);
}

il2cpp_hresult_t STDCALL il2cpp::vm::ComObjectBase::GetMarshalSizeMax(const Il2CppGuid& iid, void* object, uint32_t context, void* reserved, uint32_t flags, uint32_t* size)
{
    Il2CppIMarshal* freeThreadedMarshaler;
    il2cpp_hresult_t hr = GetFreeThreadedMarshalerNoAddRef(&freeThreadedMarshaler);
    if (IL2CPP_HR_FAILED(hr))
        return hr;

    return freeThreadedMarshaler->GetMarshalSizeMax(iid, object, context, reserved, flags, size);
}

il2cpp_hresult_t STDCALL il2cpp::vm::ComObjectBase::MarshalInterface(Il2CppIStream* stream, const Il2CppGuid& iid, void* object, uint32_t context, void* reserved, uint32_t flags)
{
    Il2CppIMarshal* freeThreadedMarshaler;
    il2cpp_hresult_t hr = GetFreeThreadedMarshalerNoAddRef(&freeThreadedMarshaler);
    if (IL2CPP_HR_FAILED(hr))
        return hr;

    return freeThreadedMarshaler->MarshalInterface(stream, iid, object, context, reserved, flags);
}

il2cpp_hresult_t STDCALL il2cpp::vm::ComObjectBase::UnmarshalInterface(Il2CppIStream* stream, const Il2CppGuid& iid, void** object)
{
    Il2CppIMarshal* freeThreadedMarshaler;
    il2cpp_hresult_t hr = GetFreeThreadedMarshalerNoAddRef(&freeThreadedMarshaler);
    if (IL2CPP_HR_FAILED(hr))
        return hr;

    return freeThreadedMarshaler->UnmarshalInterface(stream, iid, object);
}

il2cpp_hresult_t STDCALL il2cpp::vm::ComObjectBase::ReleaseMarshalData(Il2CppIStream* stream)
{
    Il2CppIMarshal* freeThreadedMarshaler;
    il2cpp_hresult_t hr = GetFreeThreadedMarshalerNoAddRef(&freeThreadedMarshaler);
    if (IL2CPP_HR_FAILED(hr))
        return hr;

    return freeThreadedMarshaler->ReleaseMarshalData(stream);
}

il2cpp_hresult_t STDCALL il2cpp::vm::ComObjectBase::DisconnectObject(uint32_t reserved)
{
    Il2CppIMarshal* freeThreadedMarshaler;
    il2cpp_hresult_t hr = GetFreeThreadedMarshalerNoAddRef(&freeThreadedMarshaler);
    if (IL2CPP_HR_FAILED(hr))
        return hr;

    return freeThreadedMarshaler->DisconnectObject(reserved);
}

il2cpp_hresult_t il2cpp::vm::ComObjectBase::GetFreeThreadedMarshalerNoAddRef(Il2CppIMarshal** destination)
{
    Il2CppIMarshal* freeThreadedMarshaler = m_FreeThreadedMarshaler;
    if (freeThreadedMarshaler == NULL)
    {
        // We don't really want to aggregate FTM, as then we'd have to store its IUnknown too
        // So we pass NULL as the first parameter
        Il2CppIUnknown* freeThreadedMarshalerUnknown;
        il2cpp_hresult_t hr = COM::CreateFreeThreadedMarshaler(NULL, &freeThreadedMarshalerUnknown);
        if (IL2CPP_HR_FAILED(hr))
            return hr;

        hr = freeThreadedMarshalerUnknown->QueryInterface(Il2CppIMarshal::IID, reinterpret_cast<void**>(&freeThreadedMarshaler));
        freeThreadedMarshalerUnknown->Release();
        if (IL2CPP_HR_FAILED(hr))
            return hr;

        if (Atomic::CompareExchangePointer<Il2CppIMarshal>(&m_FreeThreadedMarshaler, freeThreadedMarshaler, NULL) != NULL)
        {
            freeThreadedMarshaler->Release();
            freeThreadedMarshaler = m_FreeThreadedMarshaler;
        }
    }

    *destination = freeThreadedMarshaler;
    return IL2CPP_S_OK;
}

il2cpp_hresult_t STDCALL il2cpp::vm::ComObjectBase::GetWeakReference(Il2CppIWeakReference** weakReference)
{
    return WeakReference::Create(GetManagedObjectInline(), weakReference);
}
