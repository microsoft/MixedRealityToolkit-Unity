#include "il2cpp-config.h"
#include "MetadataCache.h"

#include <map>
#include <limits>
#include "il2cpp-class-internals.h"
#include "il2cpp-tabledefs.h"
#include "il2cpp-runtime-stats.h"
#include "gc/GarbageCollector.h"
#include "metadata/ArrayMetadata.h"
#include "metadata/GenericMetadata.h"
#include "metadata/GenericMethod.h"
#include "metadata/Il2CppTypeCompare.h"
#include "metadata/Il2CppTypeHash.h"
#include "metadata/Il2CppTypeVector.h"
#include "metadata/Il2CppGenericContextCompare.h"
#include "metadata/Il2CppGenericContextHash.h"
#include "metadata/Il2CppGenericInstCompare.h"
#include "metadata/Il2CppGenericInstHash.h"
#include "metadata/Il2CppGenericMethodCompare.h"
#include "metadata/Il2CppGenericMethodHash.h"
#include "metadata/Il2CppSignatureCompare.h"
#include "metadata/Il2CppSignatureHash.h"
#include "os/Atomic.h"
#include "os/Mutex.h"
#include "utils/CallOnce.h"
#include "utils/Collections.h"
#include "utils/HashUtils.h"
#include "utils/Il2CppHashMap.h"
#include "utils/Il2CppHashSet.h"
#include "utils/Memory.h"
#include "utils/StringUtils.h"
#include "utils/PathUtils.h"
#include "vm/Assembly.h"
#include "vm/Class.h"
#include "vm/ClassInlines.h"
#include "vm/GenericClass.h"
#include "vm/MetadataAlloc.h"
#include "vm/MetadataLoader.h"
#include "vm/MetadataLock.h"
#include "vm/Method.h"
#include "vm/Object.h"
#include "vm/String.h"
#include "vm/Type.h"
#include "mono-runtime/il2cpp-mapping.h"
#include "vm-utils/NativeSymbol.h"
#include "vm-utils/VmStringUtils.h"

typedef std::map<Il2CppClass*, Il2CppClass*> PointerTypeMap;
typedef Il2CppHashMap<const char*, Il2CppClass*, il2cpp::utils::StringUtils::StringHasher<const char*>, il2cpp::utils::VmStringUtils::CaseSensitiveComparer> WindowsRuntimeTypeNameToClassMap;
typedef Il2CppHashMap<const Il2CppClass*, const char*, il2cpp::utils::PointerHash<Il2CppClass> > ClassToWindowsRuntimeTypeNameMap;

typedef Il2CppHashSet<const Il2CppGenericMethod*, il2cpp::metadata::Il2CppGenericMethodHash, il2cpp::metadata::Il2CppGenericMethodCompare> Il2CppGenericMethodSet;
typedef Il2CppGenericMethodSet::const_iterator Il2CppGenericMethodSetIter;
static Il2CppGenericMethodSet s_GenericMethodSet;

struct Il2CppMetadataCache
{
    il2cpp::os::FastMutex m_CacheMutex;
    PointerTypeMap m_PointerTypes;
};

static Il2CppMetadataCache s_MetadataCache;
static Il2CppClass** s_TypeInfoTable = NULL;
static Il2CppClass** s_TypeInfoDefinitionTable = NULL;
static const MethodInfo** s_MethodInfoDefinitionTable = NULL;
static Il2CppString** s_StringLiteralTable = NULL;
static const Il2CppGenericMethod** s_GenericMethodTable = NULL;
static int32_t s_ImagesCount = 0;
static Il2CppImage* s_ImagesTable = NULL;
static int32_t s_AssembliesCount = 0;
static Il2CppAssembly* s_AssembliesTable = NULL;


typedef Il2CppHashSet<const Il2CppGenericInst*, il2cpp::metadata::Il2CppGenericInstHash, il2cpp::metadata::Il2CppGenericInstCompare> Il2CppGenericInstSet;
static Il2CppGenericInstSet s_GenericInstSet;

typedef Il2CppHashMap<const Il2CppGenericMethod*, const Il2CppGenericMethodIndices*, il2cpp::metadata::Il2CppGenericMethodHash, il2cpp::metadata::Il2CppGenericMethodCompare> Il2CppMethodTableMap;
typedef Il2CppMethodTableMap::const_iterator Il2CppMethodTableMapIter;
static Il2CppMethodTableMap s_MethodTableMap;

typedef Il2CppHashMap<il2cpp::utils::dynamic_array<const Il2CppType*>, Il2CppMethodPointer, il2cpp::metadata::Il2CppSignatureHash, il2cpp::metadata::Il2CppSignatureCompare> Il2CppUnresolvedSignatureMap;
typedef Il2CppUnresolvedSignatureMap::const_iterator Il2CppUnresolvedSignatureMapIter;
static Il2CppUnresolvedSignatureMap *s_pUnresolvedSignatureMap;

typedef Il2CppHashMap<FieldInfo*, int32_t, il2cpp::utils::PointerHash<FieldInfo> > Il2CppThreadLocalStaticOffsetHashMap;
typedef Il2CppThreadLocalStaticOffsetHashMap::iterator Il2CppThreadLocalStaticOffsetHashMapIter;
static Il2CppThreadLocalStaticOffsetHashMap s_ThreadLocalStaticOffsetMap;

static const Il2CppCodeRegistration * s_Il2CppCodeRegistration;
static const Il2CppMetadataRegistration * s_Il2CppMetadataRegistration;
static const Il2CppCodeGenOptions* s_Il2CppCodeGenOptions;
static CustomAttributesCache** s_CustomAttributesCaches;

static WindowsRuntimeTypeNameToClassMap s_WindowsRuntimeTypeNameToClassMap;
static ClassToWindowsRuntimeTypeNameMap s_ClassToWindowsRuntimeTypeNameMap;

struct InteropDataToTypeConverter
{
    inline const Il2CppType* operator()(const Il2CppInteropData& interopData) const
    {
        return interopData.type;
    }
};

typedef il2cpp::utils::collections::ArrayValueMap<const Il2CppType*, Il2CppInteropData, InteropDataToTypeConverter, il2cpp::metadata::Il2CppTypeLess, il2cpp::metadata::Il2CppTypeEqualityComparer> InteropDataMap;
static InteropDataMap s_InteropData;

template<typename K, typename V>
struct PairToKeyConverter
{
    inline const K& operator()(const std::pair<K, V>& pair) const
    {
        return pair.first;
    }
};

typedef il2cpp::utils::collections::ArrayValueMap<const Il2CppGuid*, std::pair<const Il2CppGuid*, Il2CppClass*>, PairToKeyConverter<const Il2CppGuid*, Il2CppClass*> > GuidToClassMap;
static GuidToClassMap s_GuidToNonImportClassMap;

template<typename T>
static T MetadataOffset(void* metadata, size_t sectionOffset, size_t itemIndex)
{
    return reinterpret_cast<T>(reinterpret_cast<uint8_t*>(metadata) + sectionOffset) + itemIndex;
}

void il2cpp::vm::MetadataCache::Register(const Il2CppCodeRegistration* const codeRegistration, const Il2CppMetadataRegistration* const metadataRegistration, const Il2CppCodeGenOptions* const codeGenOptions)
{
    s_Il2CppCodeRegistration = codeRegistration;
    s_Il2CppMetadataRegistration = metadataRegistration;
    s_Il2CppCodeGenOptions = codeGenOptions;

    for (int32_t j = 0; j < metadataRegistration->genericClassesCount; j++)
        if (metadataRegistration->genericClasses[j]->typeDefinitionIndex != kTypeIndexInvalid)
            il2cpp::metadata::GenericMetadata::RegisterGenericClass(metadataRegistration->genericClasses[j]);

    for (int32_t i = 0; i < metadataRegistration->genericInstsCount; i++)
        s_GenericInstSet.insert(metadataRegistration->genericInsts[i]);

    s_InteropData.assign_external(codeRegistration->interopData, codeRegistration->interopDataCount);
}

static void* s_GlobalMetadata;
static const Il2CppGlobalMetadataHeader* s_GlobalMetadataHeader;

bool il2cpp::vm::MetadataCache::Initialize()
{
    s_GlobalMetadata = vm::MetadataLoader::LoadMetadataFile("global-metadata.dat");
    if (!s_GlobalMetadata)
        return false;

    s_GlobalMetadataHeader = (const Il2CppGlobalMetadataHeader*)s_GlobalMetadata;
    IL2CPP_ASSERT(s_GlobalMetadataHeader->sanity == 0xFAB11BAF);
    IL2CPP_ASSERT(s_GlobalMetadataHeader->version == 24);

    // Pre-allocate these arrays so we don't need to lock when reading later.
    // These arrays hold the runtime metadata representation for metadata explicitly
    // referenced during conversion. There is a corresponding table of same size
    // in the converted metadata, giving a description of runtime metadata to construct.
    s_TypeInfoTable = (Il2CppClass**)IL2CPP_CALLOC(s_Il2CppMetadataRegistration->typesCount, sizeof(Il2CppClass*));
    s_TypeInfoDefinitionTable = (Il2CppClass**)IL2CPP_CALLOC(s_GlobalMetadataHeader->typeDefinitionsCount / sizeof(Il2CppTypeDefinition), sizeof(Il2CppClass*));
    s_MethodInfoDefinitionTable = (const MethodInfo**)IL2CPP_CALLOC(s_GlobalMetadataHeader->methodsCount / sizeof(Il2CppMethodDefinition), sizeof(MethodInfo*));
    s_GenericMethodTable = (const Il2CppGenericMethod**)IL2CPP_CALLOC(s_Il2CppMetadataRegistration->methodSpecsCount, sizeof(Il2CppGenericMethod*));
    s_ImagesCount = s_GlobalMetadataHeader->imagesCount / sizeof(Il2CppImageDefinition);
    s_ImagesTable = (Il2CppImage*)IL2CPP_CALLOC(s_ImagesCount, sizeof(Il2CppImage));
    s_AssembliesCount = s_GlobalMetadataHeader->assembliesCount / sizeof(Il2CppAssemblyDefinition);
    s_AssembliesTable = (Il2CppAssembly*)IL2CPP_CALLOC(s_AssembliesCount, sizeof(Il2CppAssembly));

    // setup all the Il2CppImages. There are not many and it avoid locks later on
    const Il2CppImageDefinition* imagesDefinitions = (const Il2CppImageDefinition*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->imagesOffset);
    for (int32_t imageIndex = 0; imageIndex < s_ImagesCount; imageIndex++)
    {
        const Il2CppImageDefinition* imageDefinition = imagesDefinitions + imageIndex;
        Il2CppImage* image = s_ImagesTable + imageIndex;
        image->name = GetStringFromIndex(imageDefinition->nameIndex);

        std::string nameNoExt = il2cpp::utils::PathUtils::PathNoExtension(image->name);
        image->nameNoExt = (char*)IL2CPP_CALLOC(nameNoExt.size() + 1, sizeof(char));
        strcpy(const_cast<char*>(image->nameNoExt), nameNoExt.c_str());

        image->assembly = const_cast<Il2CppAssembly*>(GetAssemblyFromIndex(imageDefinition->assemblyIndex));
        image->typeStart = imageDefinition->typeStart;
        image->typeCount = imageDefinition->typeCount;
        image->exportedTypeStart = imageDefinition->exportedTypeStart;
        image->exportedTypeCount = imageDefinition->exportedTypeCount;
        image->entryPointIndex = imageDefinition->entryPointIndex;
        image->token = imageDefinition->token;
        image->customAttributeStart = imageDefinition->customAttributeStart;
        image->customAttributeCount = imageDefinition->customAttributeCount;
        for (uint32_t codeGenModuleIndex = 0; codeGenModuleIndex < s_Il2CppCodeRegistration->codeGenModulesCount; ++codeGenModuleIndex)
        {
            if (strcmp(image->name, s_Il2CppCodeRegistration->codeGenModules[codeGenModuleIndex]->moduleName) == 0)
                image->codeGenModule = s_Il2CppCodeRegistration->codeGenModules[codeGenModuleIndex];
        }
        IL2CPP_ASSERT(image->codeGenModule);
        image->dynamic = false;
    }

    // setup all the Il2CppAssemblies.
    const Il2CppAssemblyDefinition* assemblyDefinitions = (const Il2CppAssemblyDefinition*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->assembliesOffset);
    for (int32_t assemblyIndex = 0; assemblyIndex < s_ImagesCount; assemblyIndex++)
    {
        const Il2CppAssemblyDefinition* assemblyDefinition = assemblyDefinitions + assemblyIndex;
        Il2CppAssembly* assembly = s_AssembliesTable + assemblyIndex;

        assembly->image = il2cpp::vm::MetadataCache::GetImageFromIndex(assemblyDefinition->imageIndex);
        assembly->token = assemblyDefinition->token;
        assembly->referencedAssemblyStart = assemblyDefinition->referencedAssemblyStart;
        assembly->referencedAssemblyCount = assemblyDefinition->referencedAssemblyCount;

        Il2CppAssemblyName* assemblyName = &assembly->aname;
        const Il2CppAssemblyNameDefinition* assemblyNameDefinition = &assemblyDefinition->aname;

        assemblyName->name = GetStringFromIndex(assemblyNameDefinition->nameIndex);
        assemblyName->culture = GetStringFromIndex(assemblyNameDefinition->cultureIndex);
        assemblyName->hash_value = GetStringFromIndex(assemblyNameDefinition->hashValueIndex);
        assemblyName->public_key = GetStringFromIndex(assemblyNameDefinition->publicKeyIndex);
        if (strcmp(assemblyName->public_key, "NULL") == 0)
            assemblyName->public_key = NULL;
        assemblyName->hash_alg = assemblyNameDefinition->hash_alg;
        assemblyName->hash_len = assemblyNameDefinition->hash_len;
        assemblyName->flags = assemblyNameDefinition->flags;
        assemblyName->major = assemblyNameDefinition->major;
        assemblyName->minor = assemblyNameDefinition->minor;
        assemblyName->build = assemblyNameDefinition->build;
        assemblyName->revision = assemblyNameDefinition->revision;
        memcpy(assemblyName->public_key_token, assemblyNameDefinition->public_key_token, sizeof(assemblyNameDefinition->public_key_token));

        il2cpp::vm::Assembly::Register(assembly);
    }

    InitializeUnresolvedSignatureTable();

#if IL2CPP_ENABLE_NATIVE_STACKTRACES
    std::vector<MethodDefinitionKey> managedMethods;


    const Il2CppTypeDefinition* typeDefinitions = (const Il2CppTypeDefinition*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->typeDefinitionsOffset);
    for (int32_t i = 0; i < s_AssembliesCount; i++)
    {
        const Il2CppImage* image = s_AssembliesTable[i].image;

        for (size_t j = 0; j < image->typeCount; j++)
        {
            const Il2CppTypeDefinition* type = typeDefinitions + image->typeStart + j;

            for (uint16_t u = 0; u < type->method_count; u++)
            {
                const Il2CppMethodDefinition* methodDefinition = GetMethodDefinitionFromIndex(type->methodStart + u);
                MethodDefinitionKey currentMethodList;
                currentMethodList.methodIndex = type->methodStart + u;
                currentMethodList.method = GetMethodPointer(image, methodDefinition->token);
                if (currentMethodList.method)
                    managedMethods.push_back(currentMethodList);
            }
        }
    }

    for (int32_t i = 0; i < s_Il2CppMetadataRegistration->genericMethodTableCount; i++)
    {
        const Il2CppGenericMethodFunctionsDefinitions* genericMethodIndices = s_Il2CppMetadataRegistration->genericMethodTable + i;

        MethodDefinitionKey currentMethodList;

        GenericMethodIndex genericMethodIndex = genericMethodIndices->genericMethodIndex;

        IL2CPP_ASSERT(genericMethodIndex < s_Il2CppMetadataRegistration->methodSpecsCount);
        const Il2CppMethodSpec* methodSpec = s_Il2CppMetadataRegistration->methodSpecs + genericMethodIndex;

        currentMethodList.methodIndex = methodSpec->methodDefinitionIndex;

        IL2CPP_ASSERT(genericMethodIndices->indices.methodIndex < static_cast<int32_t>(s_Il2CppCodeRegistration->genericMethodPointersCount));
        currentMethodList.method = s_Il2CppCodeRegistration->genericMethodPointers[genericMethodIndices->indices.methodIndex];

        managedMethods.push_back(currentMethodList);
    }

    il2cpp::utils::NativeSymbol::RegisterMethods(managedMethods);
#endif
    return true;
}

void il2cpp::vm::MetadataCache::InitializeStringLiteralTable()
{
    s_StringLiteralTable = (Il2CppString**)il2cpp::gc::GarbageCollector::AllocateFixed(s_GlobalMetadataHeader->stringLiteralCount / sizeof(Il2CppStringLiteral) * sizeof(Il2CppString*), NULL);
}

void il2cpp::vm::MetadataCache::InitializeGenericMethodTable()
{
    for (int32_t i = 0; i < s_Il2CppMetadataRegistration->genericMethodTableCount; i++)
    {
        const Il2CppGenericMethodFunctionsDefinitions* genericMethodIndices = s_Il2CppMetadataRegistration->genericMethodTable + i;
        const Il2CppGenericMethod* genericMethod = GetGenericMethodFromIndex(genericMethodIndices->genericMethodIndex);
        s_MethodTableMap.insert(std::make_pair(genericMethod, &genericMethodIndices->indices));
    }
}

void il2cpp::vm::MetadataCache::InitializeWindowsRuntimeTypeNamesTables()
{
    int32_t typeCount = s_GlobalMetadataHeader->windowsRuntimeTypeNamesSize / sizeof(Il2CppWindowsRuntimeTypeNamePair);
    const Il2CppWindowsRuntimeTypeNamePair* windowsRuntimeTypeNames = MetadataOffset<Il2CppWindowsRuntimeTypeNamePair*>(s_GlobalMetadata, s_GlobalMetadataHeader->windowsRuntimeTypeNamesOffset, 0);

    for (int32_t i = 0; i < typeCount; i++)
    {
        Il2CppWindowsRuntimeTypeNamePair typeNamePair = windowsRuntimeTypeNames[i];
        const char* name = GetStringFromIndex(typeNamePair.nameIndex);
        const Il2CppType* type = GetIl2CppTypeFromIndex(typeNamePair.typeIndex);
        Il2CppClass* klass = il2cpp::vm::Class::FromIl2CppType(type);

        if (!Class::IsNullable(klass))
        {
            // Don't add nullable types to name -> klass map because IReference`1<T> and Nullable`1<T>
            // share windows runtime type names, and that would cause a collision.
            s_WindowsRuntimeTypeNameToClassMap.insert(std::make_pair(name, klass));
        }

        s_ClassToWindowsRuntimeTypeNameMap.insert(std::make_pair(klass, name));
    }
}

void il2cpp::vm::MetadataCache::InitializeGuidToClassTable()
{
    Il2CppInteropData* interopData = s_Il2CppCodeRegistration->interopData;
    uint32_t interopDataCount = s_Il2CppCodeRegistration->interopDataCount;
    std::vector<std::pair<const Il2CppGuid*, Il2CppClass*> > guidToNonImportClassMap;
    guidToNonImportClassMap.reserve(interopDataCount);

    for (uint32_t i = 0; i < interopDataCount; i++)
    {
        // It's important to check for non-import types because type projections will have identical GUIDs (e.g. IEnumerable<T> and IIterable<T>)
        if (interopData[i].guid != NULL)
        {
            Il2CppClass* klass = Class::FromIl2CppType(interopData[i].type);
            if (!klass->is_import_or_windows_runtime)
                guidToNonImportClassMap.push_back(std::make_pair(interopData[i].guid, klass));
        }
    }

    s_GuidToNonImportClassMap.assign(guidToNonImportClassMap);
}

// this is called later in the intialization cycle with more systems setup like GC
void il2cpp::vm::MetadataCache::InitializeGCSafe()
{
    InitializeStringLiteralTable();
    InitializeGenericMethodTable();
    InitializeWindowsRuntimeTypeNamesTables();
    InitializeGuidToClassTable();
}

void il2cpp::vm::MetadataCache::InitializeUnresolvedSignatureTable()
{
    s_pUnresolvedSignatureMap = new Il2CppUnresolvedSignatureMap();

    for (uint32_t i = 0; i < s_Il2CppCodeRegistration->unresolvedVirtualCallCount; ++i)
    {
        const Il2CppRange* range = MetadataOffset<Il2CppRange*>(s_GlobalMetadata, s_GlobalMetadataHeader->unresolvedVirtualCallParameterRangesOffset, i);
        il2cpp::utils::dynamic_array<const Il2CppType*> signature;

        for (int j = 0; j < range->length; ++j)
        {
            TypeIndex typeIndex = *MetadataOffset<TypeIndex*>(s_GlobalMetadata, s_GlobalMetadataHeader->unresolvedVirtualCallParameterTypesOffset, range->start + j);
            const Il2CppType* type = il2cpp::vm::MetadataCache::GetIl2CppTypeFromIndex(typeIndex);
            signature.push_back(type);
        }

        (*s_pUnresolvedSignatureMap)[signature] = s_Il2CppCodeRegistration->unresolvedVirtualCallPointers[i];
    }
}

Il2CppClass* il2cpp::vm::MetadataCache::GetGenericInstanceType(Il2CppClass* genericTypeDefinition, const il2cpp::metadata::Il2CppTypeVector& genericArgumentTypes)
{
    const Il2CppGenericInst* inst = il2cpp::vm::MetadataCache::GetGenericInst(genericArgumentTypes);
    Il2CppGenericClass* genericClass = il2cpp::metadata::GenericMetadata::GetGenericClass(genericTypeDefinition, inst);
    return il2cpp::vm::GenericClass::GetClass(genericClass);
}

const MethodInfo* il2cpp::vm::MetadataCache::GetGenericInstanceMethod(const MethodInfo* genericMethodDefinition, const Il2CppGenericContext* context)
{
    const MethodInfo* method = genericMethodDefinition;
    const Il2CppGenericInst* classInst = context->class_inst;
    const Il2CppGenericInst* methodInst = context->method_inst;
    if (genericMethodDefinition->is_inflated)
    {
        IL2CPP_ASSERT(genericMethodDefinition->klass->generic_class);
        classInst = genericMethodDefinition->klass->generic_class->context.class_inst;
        method = genericMethodDefinition->genericMethod->methodDefinition;
    }

    const Il2CppGenericMethod* gmethod = GetGenericMethod(method, classInst, methodInst);
    return il2cpp::metadata::GenericMethod::GetMethod(gmethod);
}

const MethodInfo* il2cpp::vm::MetadataCache::GetGenericInstanceMethod(const MethodInfo* genericMethodDefinition, const il2cpp::metadata::Il2CppTypeVector& genericArgumentTypes)
{
    Il2CppGenericContext context = { NULL, GetGenericInst(genericArgumentTypes) };

    return GetGenericInstanceMethod(genericMethodDefinition, &context);
}

const Il2CppGenericContext* il2cpp::vm::MetadataCache::GetMethodGenericContext(const MethodInfo* method)
{
    if (!method->is_inflated)
    {
        IL2CPP_NOT_IMPLEMENTED(Image::GetMethodGenericContext);
        return NULL;
    }

    return &method->genericMethod->context;
}

const MethodInfo* il2cpp::vm::MetadataCache::GetGenericMethodDefinition(const MethodInfo* method)
{
    if (!method->is_inflated)
    {
        IL2CPP_NOT_IMPLEMENTED(Image::GetGenericMethodDefinition);
        return NULL;
    }

    return method->genericMethod->methodDefinition;
}

const Il2CppGenericContainer* il2cpp::vm::MetadataCache::GetMethodGenericContainer(const MethodInfo* method)
{
    return method->genericContainer;
}

Il2CppClass* il2cpp::vm::MetadataCache::GetPointerType(Il2CppClass* type)
{
    il2cpp::os::FastAutoLock lock(&s_MetadataCache.m_CacheMutex);

    PointerTypeMap::const_iterator i = s_MetadataCache.m_PointerTypes.find(type);
    if (i == s_MetadataCache.m_PointerTypes.end())
        return NULL;

    return i->second;
}

Il2CppClass* il2cpp::vm::MetadataCache::GetWindowsRuntimeClass(const char* fullName)
{
    WindowsRuntimeTypeNameToClassMap::iterator it = s_WindowsRuntimeTypeNameToClassMap.find(fullName);
    if (it != s_WindowsRuntimeTypeNameToClassMap.end())
        return it->second;

    return NULL;
}

const char* il2cpp::vm::MetadataCache::GetWindowsRuntimeClassName(const Il2CppClass* klass)
{
    ClassToWindowsRuntimeTypeNameMap::iterator it = s_ClassToWindowsRuntimeTypeNameMap.find(klass);
    if (it != s_ClassToWindowsRuntimeTypeNameMap.end())
        return it->second;

    return NULL;
}

Il2CppClass* il2cpp::vm::MetadataCache::GetClassForGuid(const Il2CppGuid* guid)
{
    IL2CPP_ASSERT(guid != NULL);

    GuidToClassMap::iterator it = s_GuidToNonImportClassMap.find_first(guid);
    if (it != s_GuidToNonImportClassMap.end())
        return it->second;

    return NULL;
}

void il2cpp::vm::MetadataCache::AddPointerType(Il2CppClass* type, Il2CppClass* pointerType)
{
    il2cpp::os::FastAutoLock lock(&s_MetadataCache.m_CacheMutex);
    s_MetadataCache.m_PointerTypes.insert(std::make_pair(type, pointerType));
}

const Il2CppGenericInst* il2cpp::vm::MetadataCache::GetGenericInst(const Il2CppType* const* types, uint32_t typeCount)
{
    // temporary inst to lookup a permanent one that may already exist
    Il2CppGenericInst inst;
    inst.type_argc = typeCount;
    inst.type_argv = (const Il2CppType**)alloca(inst.type_argc * sizeof(Il2CppType*));

    size_t index = 0;
    const Il2CppType* const* typesEnd = types + typeCount;
    for (const Il2CppType* const* iter = types; iter != typesEnd; ++iter, ++index)
        inst.type_argv[index] = *iter;

    il2cpp::os::FastAutoLock lock(&s_MetadataCache.m_CacheMutex);
    Il2CppGenericInstSet::const_iterator iter = s_GenericInstSet.find(&inst);
    if (iter != s_GenericInstSet.end())
        return *iter;

    Il2CppGenericInst* newInst = (Il2CppGenericInst*)IL2CPP_MALLOC(sizeof(Il2CppGenericInst));
    newInst->type_argc = typeCount;
    newInst->type_argv = (const Il2CppType**)IL2CPP_MALLOC(newInst->type_argc * sizeof(Il2CppType*));

    index = 0;
    for (const Il2CppType* const* iter = types; iter != typesEnd; ++iter, ++index)
        newInst->type_argv[index] = *iter;

    s_GenericInstSet.insert(newInst);

    ++il2cpp_runtime_stats.generic_instance_count;

    return newInst;
}

const Il2CppGenericInst* il2cpp::vm::MetadataCache::GetGenericInst(const il2cpp::metadata::Il2CppTypeVector& types)
{
    return GetGenericInst(&types[0], static_cast<uint32_t>(types.size()));
}

static il2cpp::os::FastMutex s_GenericMethodMutex;
const Il2CppGenericMethod* il2cpp::vm::MetadataCache::GetGenericMethod(const MethodInfo* methodDefinition, const Il2CppGenericInst* classInst, const Il2CppGenericInst* methodInst)
{
    Il2CppGenericMethod method = { 0 };
    method.methodDefinition = methodDefinition;
    method.context.class_inst = classInst;
    method.context.method_inst = methodInst;

    il2cpp::os::FastAutoLock lock(&s_GenericMethodMutex);
    Il2CppGenericMethodSet::const_iterator iter = s_GenericMethodSet.find(&method);
    if (iter != s_GenericMethodSet.end())
        return *iter;

    Il2CppGenericMethod* newMethod = MetadataAllocGenericMethod();
    newMethod->methodDefinition = methodDefinition;
    newMethod->context.class_inst = classInst;
    newMethod->context.method_inst = methodInst;

    s_GenericMethodSet.insert(newMethod);

    return newMethod;
}

static bool IsShareableEnum(const Il2CppType* type)
{
    // Base case for recursion - we've found an enum.
    if (il2cpp::vm::Type::IsEnum(type))
        return true;

    if (il2cpp::vm::Type::IsGenericInstance(type))
    {
        // Recursive case - look "inside" the generic instance type to see if this is a nested enum.
        Il2CppClass* definition = il2cpp::vm::GenericClass::GetTypeDefinition(type->data.generic_class);
        return IsShareableEnum(il2cpp::vm::Class::GetType(definition));
    }

    // Base case for recurion - this is not an enum or a generic instance type.
    return false;
}

// this logic must match the C# logic in GenericSharingAnalysis.GetSharedTypeForGenericParameter
static const Il2CppGenericInst* GetSharedInst(const Il2CppGenericInst* inst)
{
    if (inst == NULL)
        return NULL;

    il2cpp::metadata::Il2CppTypeVector types;
    for (uint32_t i = 0; i < inst->type_argc; ++i)
    {
        if (il2cpp::vm::Type::IsReference(inst->type_argv[i]))
            types.push_back(&il2cpp_defaults.object_class->byval_arg);
        else
        {
            const Il2CppType* type = inst->type_argv[i];
#if NET_4_0
            if (s_Il2CppCodeGenOptions->enablePrimitiveValueTypeGenericSharing)
            {
                if (IsShareableEnum(type))
                {
                    const Il2CppType* underlyingType = il2cpp::vm::Type::GetUnderlyingType(type);
                    switch (underlyingType->type)
                    {
                        case IL2CPP_TYPE_I1:
                            type = &il2cpp_defaults.sbyte_shared_enum->byval_arg;
                            break;
                        case IL2CPP_TYPE_I2:
                            type = &il2cpp_defaults.int16_shared_enum->byval_arg;
                            break;
                        case IL2CPP_TYPE_I4:
                            type = &il2cpp_defaults.int32_shared_enum->byval_arg;
                            break;
                        case IL2CPP_TYPE_I8:
                            type = &il2cpp_defaults.int64_shared_enum->byval_arg;
                            break;
                        case IL2CPP_TYPE_U1:
                            type = &il2cpp_defaults.byte_shared_enum->byval_arg;
                            break;
                        case IL2CPP_TYPE_U2:
                            type = &il2cpp_defaults.uint16_shared_enum->byval_arg;
                            break;
                        case IL2CPP_TYPE_U4:
                            type = &il2cpp_defaults.uint32_shared_enum->byval_arg;
                            break;
                        case IL2CPP_TYPE_U8:
                            type = &il2cpp_defaults.uint64_shared_enum->byval_arg;
                            break;
                        default:
                            IL2CPP_ASSERT(0 && "Invalid enum underlying type");
                            break;
                    }
                }
            }
#endif

            if (il2cpp::vm::Type::IsGenericInstance(type))
            {
                const Il2CppGenericInst* sharedInst = GetSharedInst(type->data.generic_class->context.class_inst);
                Il2CppGenericClass* gklass = il2cpp::metadata::GenericMetadata::GetGenericClass(type->data.generic_class->typeDefinitionIndex, sharedInst);
                Il2CppClass* klass = il2cpp::vm::GenericClass::GetClass(gklass);
                type = &klass->byval_arg;
            }
            types.push_back(type);
        }
    }

    const Il2CppGenericInst* sharedInst = il2cpp::vm::MetadataCache::GetGenericInst(types);

    return sharedInst;
}

InvokerMethod il2cpp::vm::MetadataCache::GetInvokerMethodPointer(const MethodInfo* methodDefinition, const Il2CppGenericContext* context)
{
    Il2CppGenericMethod method = { 0 };
    method.methodDefinition = const_cast<MethodInfo*>(methodDefinition);
    method.context.class_inst = context->class_inst;
    method.context.method_inst = context->method_inst;

    Il2CppMethodTableMapIter iter = s_MethodTableMap.find(&method);
    if (iter != s_MethodTableMap.end())
    {
        IL2CPP_ASSERT(iter->second->invokerIndex >= 0);
        if (static_cast<uint32_t>(iter->second->invokerIndex) < s_Il2CppCodeRegistration->invokerPointersCount)
            return s_Il2CppCodeRegistration->invokerPointers[iter->second->invokerIndex];
        return NULL;
    }
    // get the shared version if it exists
    method.context.class_inst = GetSharedInst(context->class_inst);
    method.context.method_inst = GetSharedInst(context->method_inst);

    iter = s_MethodTableMap.find(&method);
    if (iter != s_MethodTableMap.end())
    {
        IL2CPP_ASSERT(iter->second->invokerIndex >= 0);
        if (static_cast<uint32_t>(iter->second->invokerIndex) < s_Il2CppCodeRegistration->invokerPointersCount)
            return s_Il2CppCodeRegistration->invokerPointers[iter->second->invokerIndex];
        return NULL;
    }

    return NULL;
}

Il2CppMethodPointer il2cpp::vm::MetadataCache::GetMethodPointer(const MethodInfo* methodDefinition, const Il2CppGenericContext* context)
{
    Il2CppGenericMethod method = { 0 };
    method.methodDefinition = const_cast<MethodInfo*>(methodDefinition);
    method.context.class_inst = context->class_inst;
    method.context.method_inst = context->method_inst;

    Il2CppMethodTableMapIter iter = s_MethodTableMap.find(&method);
    if (iter != s_MethodTableMap.end())
    {
        IL2CPP_ASSERT(iter->second->invokerIndex >= 0);
        if (static_cast<uint32_t>(iter->second->methodIndex) < s_Il2CppCodeRegistration->genericMethodPointersCount)
            return s_Il2CppCodeRegistration->genericMethodPointers[iter->second->methodIndex];
        return NULL;
    }

    method.context.class_inst = GetSharedInst(context->class_inst);
    method.context.method_inst = GetSharedInst(context->method_inst);

    iter = s_MethodTableMap.find(&method);
    if (iter != s_MethodTableMap.end())
    {
        IL2CPP_ASSERT(iter->second->invokerIndex >= 0);
        if (static_cast<uint32_t>(iter->second->methodIndex) < s_Il2CppCodeRegistration->genericMethodPointersCount)
            return s_Il2CppCodeRegistration->genericMethodPointers[iter->second->methodIndex];
        return NULL;
    }

    return NULL;
}

Il2CppClass* il2cpp::vm::MetadataCache::GetTypeInfoFromTypeIndex(TypeIndex index)
{
    if (index == kTypeIndexInvalid)
        return NULL;

    IL2CPP_ASSERT(index < s_Il2CppMetadataRegistration->typesCount && "Invalid type index ");

    if (s_TypeInfoTable[index])
        return s_TypeInfoTable[index];

    const Il2CppType* type = s_Il2CppMetadataRegistration->types[index];
    Il2CppClass *klass = il2cpp::vm::Class::FromIl2CppType(type);
    il2cpp::vm::ClassInlines::InitFromCodegen(klass);
    s_TypeInfoTable[index] = klass;

    return s_TypeInfoTable[index];
}

const Il2CppType* il2cpp::vm::MetadataCache::GetIl2CppTypeFromIndex(TypeIndex index)
{
    if (index == kTypeIndexInvalid)
        return NULL;

    IL2CPP_ASSERT(index < s_Il2CppMetadataRegistration->typesCount && "Invalid type index ");

    return s_Il2CppMetadataRegistration->types[index];
}

const MethodInfo* il2cpp::vm::MetadataCache::GetMethodInfoFromIndex(EncodedMethodIndex methodIndex)
{
    uint32_t index = GetDecodedMethodIndex(methodIndex);

    if (index == 0)
        return NULL;

    if (GetEncodedIndexType(methodIndex) == kIl2CppMetadataUsageMethodRef)
        return il2cpp::metadata::GenericMethod::GetMethod(GetGenericMethodFromIndex(index));
    else
        return il2cpp::vm::MetadataCache::GetMethodInfoFromMethodDefinitionIndex(index);
}

const Il2CppGenericMethod* il2cpp::vm::MetadataCache::GetGenericMethodFromIndex(GenericMethodIndex index)
{
    IL2CPP_ASSERT(index < s_Il2CppMetadataRegistration->methodSpecsCount);
    if (s_GenericMethodTable[index])
        return s_GenericMethodTable[index];

    const Il2CppMethodSpec* methodSpec = s_Il2CppMetadataRegistration->methodSpecs + index;
    const MethodInfo* methodDefinition = GetMethodInfoFromMethodDefinitionIndex(methodSpec->methodDefinitionIndex);
    const Il2CppGenericInst* classInst = NULL;
    const Il2CppGenericInst* methodInst = NULL;
    if (methodSpec->classIndexIndex != -1)
    {
        IL2CPP_ASSERT(methodSpec->classIndexIndex < s_Il2CppMetadataRegistration->genericInstsCount);
        classInst = s_Il2CppMetadataRegistration->genericInsts[methodSpec->classIndexIndex];
    }
    if (methodSpec->methodIndexIndex != -1)
    {
        IL2CPP_ASSERT(methodSpec->methodIndexIndex < s_Il2CppMetadataRegistration->genericInstsCount);
        methodInst = s_Il2CppMetadataRegistration->genericInsts[methodSpec->methodIndexIndex];
    }
    s_GenericMethodTable[index] = GetGenericMethod(methodDefinition, classInst, methodInst);

    return s_GenericMethodTable[index];
}

Il2CppMethodPointer il2cpp::vm::MetadataCache::GetMethodPointer(const Il2CppImage* image, uint32_t token)
{
    uint32_t rid = GetTokenRowId(token);
    uint32_t table =  GetTokenType(token);
    if (rid == 0)
        return NULL;

    IL2CPP_ASSERT(rid <= image->codeGenModule->methodPointerCount);

    return image->codeGenModule->methodPointers[rid - 1];
}

InvokerMethod il2cpp::vm::MetadataCache::GetMethodInvoker(const Il2CppImage* image, uint32_t token)
{
    uint32_t rid = GetTokenRowId(token);
    uint32_t table = GetTokenType(token);
    if (rid == 0)
        return NULL;

    int32_t index = image->codeGenModule->invokerIndices[rid - 1];

    if (index == kMethodIndexInvalid)
        return NULL;

    IL2CPP_ASSERT(index >= 0 && static_cast<uint32_t>(index) < s_Il2CppCodeRegistration->invokerPointersCount);
    return s_Il2CppCodeRegistration->invokerPointers[index];
}

const Il2CppInteropData* il2cpp::vm::MetadataCache::GetInteropDataForType(const Il2CppType* type)
{
    IL2CPP_ASSERT(type != NULL);
    InteropDataMap::iterator interopData = s_InteropData.find_first(type);
    if (interopData == s_InteropData.end())
        return NULL;

    return interopData;
}

static int CompareIl2CppTokenIndexPair(const void* pkey, const void* pelem)
{
    return (int)(((Il2CppTokenIndexPair*)pkey)->token - ((Il2CppTokenIndexPair*)pelem)->token);
}

Il2CppMethodPointer il2cpp::vm::MetadataCache::GetReversePInvokeWrapper(const Il2CppImage* image, uint32_t token)
{
    if (image->codeGenModule->reversePInvokeWrapperCount == 0)
        return NULL;

    Il2CppTokenIndexPair key;
    memset(&key, 0, sizeof(Il2CppTokenIndexPair));
    key.token = token;

    const Il2CppTokenIndexPair* res = (const Il2CppTokenIndexPair*)bsearch(&key, image->codeGenModule->reversePInvokeWrapperIndices, image->codeGenModule->reversePInvokeWrapperCount, sizeof(Il2CppTokenIndexPair), CompareIl2CppTokenIndexPair);

    if (res == NULL)
        return NULL;

    uint32_t index = res->index;

    IL2CPP_ASSERT(index >= 0 && static_cast<uint32_t>(index) < s_Il2CppCodeRegistration->reversePInvokeWrapperCount);
    return s_Il2CppCodeRegistration->reversePInvokeWrappers[index];
}

static const Il2CppType* GetReducedType(const Il2CppType* type)
{
    if (type->byref)
        return &il2cpp_defaults.object_class->byval_arg;

    if (il2cpp::vm::Type::IsEnum(type))
        type = il2cpp::vm::Type::GetUnderlyingType(type);

    switch (type->type)
    {
        case IL2CPP_TYPE_BOOLEAN:
            return &il2cpp_defaults.sbyte_class->byval_arg;
        case IL2CPP_TYPE_CHAR:
            return &il2cpp_defaults.int16_class->byval_arg;
        case IL2CPP_TYPE_BYREF:
        case IL2CPP_TYPE_CLASS:
        case IL2CPP_TYPE_OBJECT:
        case IL2CPP_TYPE_STRING:
        case IL2CPP_TYPE_ARRAY:
        case IL2CPP_TYPE_SZARRAY:
            return &il2cpp_defaults.object_class->byval_arg;
        case IL2CPP_TYPE_GENERICINST:
            if (il2cpp::vm::Type::GenericInstIsValuetype(type))
                return type;
            else
                return &il2cpp_defaults.object_class->byval_arg;
        default:
            return type;
    }
}

Il2CppMethodPointer il2cpp::vm::MetadataCache::GetUnresolvedVirtualCallStub(const MethodInfo* method)
{
    il2cpp::utils::dynamic_array<const Il2CppType*> signature;

    signature.push_back(GetReducedType(method->return_type));
    for (int i = 0; i < method->parameters_count; ++i)
        signature.push_back(GetReducedType(method->parameters[i].parameter_type));

    Il2CppUnresolvedSignatureMapIter it = s_pUnresolvedSignatureMap->find(signature);
    if (it != s_pUnresolvedSignatureMap->end())
        return it->second;

    return NULL;
}

static const Il2CppImage* GetImageForTypeDefinitionIndex(TypeDefinitionIndex index)
{
    for (int32_t imageIndex = 0; imageIndex < s_ImagesCount; imageIndex++)
    {
        const Il2CppImage* image = s_ImagesTable + imageIndex;
        IL2CPP_ASSERT(index >= 0);
        if (index >= image->typeStart && static_cast<uint32_t>(index) < (image->typeStart + image->typeCount))
            return image;
    }

    IL2CPP_ASSERT(0 && "Failed to find owning image for type");
    return NULL;
}

enum PackingSize
{
    Zero,
    One,
    Two,
    Four,
    Eight,
    Sixteen,
    ThirtyTwo,
    SixtyFour,
    OneHundredTwentyEight
};

static uint8_t ConvertPackingSizeEnumToValue(PackingSize packingSize)
{
    switch (packingSize)
    {
        case Zero:
            return 0;
        case One:
            return 1;
        case Two:
            return 2;
        case Four:
            return 4;
        case Eight:
            return 8;
        case Sixteen:
            return 16;
        case ThirtyTwo:
            return 32;
        case SixtyFour:
            return 64;
        case OneHundredTwentyEight:
            return 128;
        default:
            Assert(0 && "Invalid packing size!");
            return 0;
    }
}

static const int kBitIsValueType = 1;
static const int kBitIsEnum = 2;
static const int kBitHasFinalizer = 3;
static const int kBitHasStaticConstructor = 4;
static const int kBitIsBlittable = 5;
static const int kBitIsImportOrWindowsRuntime = 6;
static const int kPackingSize = 7; // This uses 4 bits from bit 7 to bit 10

static Il2CppClass* FromTypeDefinition(TypeDefinitionIndex index)
{
    IL2CPP_ASSERT(index >= 0 && static_cast<uint32_t>(index) < s_GlobalMetadataHeader->typeDefinitionsCount / sizeof(Il2CppTypeDefinition));
    const Il2CppTypeDefinition* typeDefinitions = (const Il2CppTypeDefinition*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->typeDefinitionsOffset);
    const Il2CppTypeDefinition* typeDefinition = typeDefinitions + index;
    const Il2CppTypeDefinitionSizes* typeDefinitionSizes = s_Il2CppMetadataRegistration->typeDefinitionsSizes[index];
    Il2CppClass* typeInfo = (Il2CppClass*)IL2CPP_CALLOC(1, sizeof(Il2CppClass) + (sizeof(VirtualInvokeData) * typeDefinition->vtable_count));
    typeInfo->klass = typeInfo;
    typeInfo->image = GetImageForTypeDefinitionIndex(index);
    typeInfo->name = il2cpp::vm::MetadataCache::GetStringFromIndex(typeDefinition->nameIndex);
    typeInfo->namespaze = il2cpp::vm::MetadataCache::GetStringFromIndex(typeDefinition->namespaceIndex);
    typeInfo->byval_arg = *il2cpp::vm::MetadataCache::GetIl2CppTypeFromIndex(typeDefinition->byvalTypeIndex);
    typeInfo->this_arg = *il2cpp::vm::MetadataCache::GetIl2CppTypeFromIndex(typeDefinition->byrefTypeIndex);
    typeInfo->typeDefinition = typeDefinition;
    typeInfo->genericContainerIndex = typeDefinition->genericContainerIndex;
    typeInfo->instance_size = typeDefinitionSizes->instance_size;
    typeInfo->actualSize = typeDefinitionSizes->instance_size; // actualySize is instance_size for compiler generated values
    typeInfo->native_size = typeDefinitionSizes->native_size;
    typeInfo->static_fields_size = typeDefinitionSizes->static_fields_size;
    typeInfo->thread_static_fields_size = typeDefinitionSizes->thread_static_fields_size;
    typeInfo->thread_static_fields_offset = -1;
    typeInfo->flags = typeDefinition->flags;
    typeInfo->valuetype = (typeDefinition->bitfield >> (kBitIsValueType - 1)) & 0x1;
    typeInfo->enumtype = (typeDefinition->bitfield >> (kBitIsEnum - 1)) & 0x1;
    typeInfo->is_generic = typeDefinition->genericContainerIndex != kGenericContainerIndexInvalid; // generic if we have a generic container
    typeInfo->has_finalize = (typeDefinition->bitfield >> (kBitHasFinalizer - 1)) & 0x1;
    typeInfo->has_cctor = (typeDefinition->bitfield >> (kBitHasStaticConstructor - 1)) & 0x1;
    typeInfo->is_blittable = (typeDefinition->bitfield >> (kBitIsBlittable - 1)) & 0x1;
    typeInfo->is_import_or_windows_runtime = (typeDefinition->bitfield >> (kBitIsImportOrWindowsRuntime - 1)) & 0x1;
    typeInfo->packingSize = ConvertPackingSizeEnumToValue(static_cast<PackingSize>((typeDefinition->bitfield >> (kPackingSize - 1)) & 0xF));
    typeInfo->method_count = typeDefinition->method_count;
    typeInfo->property_count = typeDefinition->property_count;
    typeInfo->field_count = typeDefinition->field_count;
    typeInfo->event_count = typeDefinition->event_count;
    typeInfo->nested_type_count = typeDefinition->nested_type_count;
    typeInfo->vtable_count = typeDefinition->vtable_count;
    typeInfo->interfaces_count = typeDefinition->interfaces_count;
    typeInfo->interface_offsets_count = typeDefinition->interface_offsets_count;
    typeInfo->token = typeDefinition->token;
    typeInfo->interopData = il2cpp::vm::MetadataCache::GetInteropDataForType(&typeInfo->byval_arg);

    if (typeDefinition->parentIndex != kTypeIndexInvalid)
        typeInfo->parent = il2cpp::vm::Class::FromIl2CppType(il2cpp::vm::MetadataCache::GetIl2CppTypeFromIndex(typeDefinition->parentIndex));

    if (typeDefinition->declaringTypeIndex != kTypeIndexInvalid)
        typeInfo->declaringType = il2cpp::vm::Class::FromIl2CppType(il2cpp::vm::MetadataCache::GetIl2CppTypeFromIndex(typeDefinition->declaringTypeIndex));

    typeInfo->castClass = typeInfo->element_class = typeInfo;
    if (typeInfo->enumtype)
        typeInfo->castClass = typeInfo->element_class = il2cpp::vm::Class::FromIl2CppType(il2cpp::vm::MetadataCache::GetIl2CppTypeFromIndex(typeDefinition->elementTypeIndex));

    return typeInfo;
}

const Il2CppAssembly* il2cpp::vm::MetadataCache::GetAssemblyFromIndex(AssemblyIndex index)
{
    if (index == kGenericContainerIndexInvalid)
        return NULL;

    IL2CPP_ASSERT(index <= s_AssembliesCount);
    return s_AssembliesTable + index;
}

const Il2CppAssembly* il2cpp::vm::MetadataCache::GetAssemblyByName(const char* nameToFind)
{
    for (int i = 0; i < s_AssembliesCount; i++)
    {
        const Il2CppAssembly* assembly = s_AssembliesTable + i;

        const char* assemblyName = assembly->aname.name;

        if (strcmp(assemblyName, nameToFind) == 0)
            return assembly;
    }

    return NULL;
}

Il2CppImage* il2cpp::vm::MetadataCache::GetImageFromIndex(ImageIndex index)
{
    if (index == kGenericContainerIndexInvalid)
        return NULL;

    IL2CPP_ASSERT(index <= s_ImagesCount);
    return s_ImagesTable + index;
}

Il2CppClass* il2cpp::vm::MetadataCache::GetTypeInfoFromTypeDefinitionIndex(TypeDefinitionIndex index)
{
    if (index == kTypeIndexInvalid)
        return NULL;

    IL2CPP_ASSERT(index >= 0 && static_cast<uint32_t>(index) < s_GlobalMetadataHeader->typeDefinitionsCount / sizeof(Il2CppTypeDefinition));

    if (!s_TypeInfoDefinitionTable[index])
    {
        // we need to use the metadata lock, since we may need to retrieve other Il2CppClass's when setting. Our parent may be a generic instance for example
        il2cpp::os::FastAutoLock lock(&g_MetadataLock);
        // double checked locking
        if (!s_TypeInfoDefinitionTable[index])
            s_TypeInfoDefinitionTable[index] = FromTypeDefinition(index);
    }

    return s_TypeInfoDefinitionTable[index];
}

const Il2CppTypeDefinition* il2cpp::vm::MetadataCache::GetTypeDefinitionFromIndex(TypeDefinitionIndex index)
{
    if (index == kTypeDefinitionIndexInvalid)
        return NULL;

    IL2CPP_ASSERT(index >= 0 && static_cast<uint32_t>(index) < s_GlobalMetadataHeader->typeDefinitionsCount / sizeof(Il2CppTypeDefinition));
    const Il2CppTypeDefinition* typeDefinitions = (const Il2CppTypeDefinition*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->typeDefinitionsOffset);
    return typeDefinitions + index;
}

TypeDefinitionIndex il2cpp::vm::MetadataCache::GetExportedTypeFromIndex(TypeDefinitionIndex index)
{
    if (index == kTypeDefinitionIndexInvalid)
        return kTypeDefinitionIndexInvalid;

    IL2CPP_ASSERT(index >= 0 && static_cast<uint32_t>(index) < s_GlobalMetadataHeader->exportedTypeDefinitionsCount / sizeof(TypeDefinitionIndex));
    TypeDefinitionIndex* exportedTypes = (TypeDefinitionIndex*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->exportedTypeDefinitionsOffset);
    return *(exportedTypes + index);
}

const Il2CppGenericContainer* il2cpp::vm::MetadataCache::GetGenericContainerFromIndex(GenericContainerIndex index)
{
    if (index == kGenericContainerIndexInvalid)
        return NULL;

    IL2CPP_ASSERT(index >= 0 && static_cast<uint32_t>(index) <= s_GlobalMetadataHeader->genericContainersCount / sizeof(Il2CppGenericContainer));
    const Il2CppGenericContainer* genericContainers = (const Il2CppGenericContainer*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->genericContainersOffset);
    return genericContainers + index;
}

const Il2CppGenericParameter* il2cpp::vm::MetadataCache::GetGenericParameterFromIndex(GenericParameterIndex index)
{
    if (index == kGenericParameterIndexInvalid)
        return NULL;

    IL2CPP_ASSERT(index >= 0 && static_cast<uint32_t>(index) <= s_GlobalMetadataHeader->genericParametersCount / sizeof(Il2CppGenericParameter));
    const Il2CppGenericParameter* genericParameters = (const Il2CppGenericParameter*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->genericParametersOffset);
    return genericParameters + index;
}

const Il2CppType* il2cpp::vm::MetadataCache::GetGenericParameterConstraintFromIndex(GenericParameterConstraintIndex index)
{
    IL2CPP_ASSERT(index >= 0 && static_cast<uint32_t>(index) <= s_GlobalMetadataHeader->genericParameterConstraintsCount / sizeof(TypeIndex));
    const TypeIndex* constraintIndices = (const TypeIndex*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->genericParameterConstraintsOffset);

    return GetIl2CppTypeFromIndex(constraintIndices[index]);
}

Il2CppClass* il2cpp::vm::MetadataCache::GetNestedTypeFromIndex(NestedTypeIndex index)
{
    IL2CPP_ASSERT(index >= 0 && static_cast<uint32_t>(index) <= s_GlobalMetadataHeader->nestedTypesCount / sizeof(TypeDefinitionIndex));
    const TypeDefinitionIndex* nestedTypeIndices = (const TypeDefinitionIndex*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->nestedTypesOffset);

    return GetTypeInfoFromTypeDefinitionIndex(nestedTypeIndices[index]);
}

const Il2CppType* il2cpp::vm::MetadataCache::GetInterfaceFromIndex(InterfacesIndex index)
{
    IL2CPP_ASSERT(index >= 0 && static_cast<uint32_t>(index) <= s_GlobalMetadataHeader->interfacesCount / sizeof(TypeIndex));
    const TypeIndex* interfaceIndices = (const TypeIndex*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->interfacesOffset);

    return GetIl2CppTypeFromIndex(interfaceIndices[index]);
}

EncodedMethodIndex il2cpp::vm::MetadataCache::GetVTableMethodFromIndex(VTableIndex index)
{
    IL2CPP_ASSERT(index >= 0 && static_cast<uint32_t>(index) <= s_GlobalMetadataHeader->vtableMethodsCount / sizeof(EncodedMethodIndex));
    const EncodedMethodIndex* methodReferences = (const EncodedMethodIndex*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->vtableMethodsOffset);

    return methodReferences[index];
}

Il2CppInterfaceOffsetPair il2cpp::vm::MetadataCache::GetInterfaceOffsetIndex(InterfaceOffsetIndex index)
{
    IL2CPP_ASSERT(index >= 0 && static_cast<uint32_t>(index) <= s_GlobalMetadataHeader->interfaceOffsetsCount / sizeof(Il2CppInterfaceOffsetPair));
    const Il2CppInterfaceOffsetPair* interfaceOffsets = (const Il2CppInterfaceOffsetPair*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->interfaceOffsetsOffset);

    return interfaceOffsets[index];
}

static int CompareIl2CppTokenRangePair(const void* pkey, const void* pelem)
{
    return (int)(((Il2CppTokenRangePair*)pkey)->token - ((Il2CppTokenRangePair*)pelem)->token);
}

il2cpp::vm::RGCTXCollection il2cpp::vm::MetadataCache::GetRGCTXs(const Il2CppImage* image, uint32_t token)
{
    il2cpp::vm::RGCTXCollection collection = { 0, NULL };
    if (image->codeGenModule->rgctxRangesCount == 0)
        return collection;

    Il2CppTokenRangePair key;
    memset(&key, 0, sizeof(Il2CppTokenRangePair));
    key.token = token;

    const Il2CppTokenRangePair* res = (const Il2CppTokenRangePair*)bsearch(&key, image->codeGenModule->rgctxRanges, image->codeGenModule->rgctxRangesCount, sizeof(Il2CppTokenRangePair), CompareIl2CppTokenRangePair);

    if (res == NULL)
        return collection;

    collection.count = res->range.length;
    collection.items = image->codeGenModule->rgctxs + res->range.start;

    return collection;
}

const Il2CppEventDefinition* il2cpp::vm::MetadataCache::GetEventDefinitionFromIndex(EventIndex index)
{
    IL2CPP_ASSERT(index >= 0 && static_cast<uint32_t>(index) <= s_GlobalMetadataHeader->eventsCount / sizeof(Il2CppEventDefinition));
    const Il2CppEventDefinition* events = (const Il2CppEventDefinition*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->eventsOffset);
    return events + index;
}

const Il2CppFieldDefinition* il2cpp::vm::MetadataCache::GetFieldDefinitionFromIndex(FieldIndex index)
{
    IL2CPP_ASSERT(index >= 0 && static_cast<uint32_t>(index) <= s_GlobalMetadataHeader->fieldsCount / sizeof(Il2CppFieldDefinition));
    const Il2CppFieldDefinition* fields = (const Il2CppFieldDefinition*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->fieldsOffset);
    return fields + index;
}

const Il2CppFieldDefaultValue* il2cpp::vm::MetadataCache::GetFieldDefaultValueFromIndex(FieldIndex index)
{
    IL2CPP_ASSERT(index >= 0 && static_cast<uint32_t>(index) <= s_GlobalMetadataHeader->fieldDefaultValuesCount / sizeof(Il2CppFieldDefaultValue));
    const Il2CppFieldDefaultValue* defaultValues = (const Il2CppFieldDefaultValue*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->fieldDefaultValuesOffset);
    return defaultValues + index;
}

const uint8_t* il2cpp::vm::MetadataCache::GetFieldDefaultValueDataFromIndex(FieldIndex index)
{
    IL2CPP_ASSERT(index >= 0 && static_cast<uint32_t>(index) <= s_GlobalMetadataHeader->fieldAndParameterDefaultValueDataCount / sizeof(uint8_t));
    const uint8_t* defaultValuesData = (const uint8_t*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->fieldAndParameterDefaultValueDataOffset);
    return defaultValuesData + index;
}

const Il2CppFieldDefaultValue* il2cpp::vm::MetadataCache::GetFieldDefaultValueForField(const FieldInfo* field)
{
    Il2CppClass* parent = field->parent;
    size_t fieldIndex = (field - parent->fields);
    if (il2cpp::vm::Type::IsGenericInstance(&parent->byval_arg))
        fieldIndex += il2cpp::vm::GenericClass::GetTypeDefinition(parent->generic_class)->typeDefinition->fieldStart;
    else
        fieldIndex += parent->typeDefinition->fieldStart;
    const Il2CppFieldDefaultValue *start = (const Il2CppFieldDefaultValue*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->fieldDefaultValuesOffset);
    const Il2CppFieldDefaultValue *entry = start;
    while (entry < start + s_GlobalMetadataHeader->fieldDefaultValuesCount)
    {
        if (fieldIndex == entry->fieldIndex)
        {
            return entry;
        }
        entry++;
    }
    IL2CPP_ASSERT(0);
    return NULL;
}

const Il2CppParameterDefaultValue * il2cpp::vm::MetadataCache::GetParameterDefaultValueForParameter(const MethodInfo* method, const ParameterInfo* parameter)
{
    if (Method::IsGenericInstance(method))
        method = GetGenericMethodDefinition(method);

    IL2CPP_ASSERT(!Method::IsGenericInstance(method));

    if (method->methodDefinition == NULL)
        return NULL;

    size_t parameterIndex = method->methodDefinition->parameterStart + parameter->position;
    const Il2CppParameterDefaultValue *start = (const Il2CppParameterDefaultValue*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->parameterDefaultValuesOffset);
    const Il2CppParameterDefaultValue *entry = start;
    while (entry < start + s_GlobalMetadataHeader->parameterDefaultValuesCount)
    {
        if (parameterIndex == entry->parameterIndex)
            return entry;
        entry++;
    }

    return NULL;
}

const uint8_t* il2cpp::vm::MetadataCache::GetParameterDefaultValueDataFromIndex(ParameterIndex index)
{
    IL2CPP_ASSERT(index >= 0 && static_cast<uint32_t>(index) <= s_GlobalMetadataHeader->fieldAndParameterDefaultValueDataCount / sizeof(uint8_t));
    const uint8_t* defaultValuesData = (const uint8_t*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->fieldAndParameterDefaultValueDataOffset);
    return defaultValuesData + index;
}

int il2cpp::vm::MetadataCache::GetFieldMarshaledSizeForField(const FieldInfo* field)
{
    Il2CppClass* parent = field->parent;
    size_t fieldIndex = (field - parent->fields);
    fieldIndex += parent->typeDefinition->fieldStart;
    const Il2CppFieldMarshaledSize *start = (const Il2CppFieldMarshaledSize*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->fieldMarshaledSizesOffset);
    const Il2CppFieldMarshaledSize *entry = start;
    while ((intptr_t)entry < (intptr_t)start + s_GlobalMetadataHeader->fieldMarshaledSizesCount)
    {
        if (fieldIndex == entry->fieldIndex)
            return entry->size;
        entry++;
    }

    return -1;
}

const Il2CppMethodDefinition* il2cpp::vm::MetadataCache::GetMethodDefinitionFromIndex(MethodIndex index)
{
    IL2CPP_ASSERT(index >= 0 && static_cast<uint32_t>(index) <= s_GlobalMetadataHeader->methodsCount / sizeof(Il2CppMethodDefinition));
    const Il2CppMethodDefinition* methods = (const Il2CppMethodDefinition*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->methodsOffset);
    return methods + index;
}

const MethodInfo* il2cpp::vm::MetadataCache::GetMethodInfoFromMethodDefinitionIndex(MethodIndex index)
{
    IL2CPP_ASSERT(index >= 0 && static_cast<uint32_t>(index) <= s_GlobalMetadataHeader->methodsCount / sizeof(Il2CppMethodDefinition));

    if (!s_MethodInfoDefinitionTable[index])
    {
        const Il2CppMethodDefinition* methodDefinition = GetMethodDefinitionFromIndex(index);
        Il2CppClass* typeInfo = GetTypeInfoFromTypeDefinitionIndex(methodDefinition->declaringType);
        il2cpp::vm::Class::SetupMethods(typeInfo);
        s_MethodInfoDefinitionTable[index] = typeInfo->methods[index - typeInfo->typeDefinition->methodStart];
    }

    return s_MethodInfoDefinitionTable[index];
}

const Il2CppPropertyDefinition* il2cpp::vm::MetadataCache::GetPropertyDefinitionFromIndex(PropertyIndex index)
{
    IL2CPP_ASSERT(index >= 0 && static_cast<uint32_t>(index) <= s_GlobalMetadataHeader->propertiesCount / sizeof(Il2CppPropertyDefinition));
    const Il2CppPropertyDefinition* properties = (const Il2CppPropertyDefinition*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->propertiesOffset);
    return properties + index;
}

const Il2CppParameterDefinition* il2cpp::vm::MetadataCache::GetParameterDefinitionFromIndex(ParameterIndex index)
{
    IL2CPP_ASSERT(index >= 0 && static_cast<uint32_t>(index) <= s_GlobalMetadataHeader->parametersCount / sizeof(Il2CppParameterDefinition));
    const Il2CppParameterDefinition* parameters = (const Il2CppParameterDefinition*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->parametersOffset);
    return parameters + index;
}

int32_t il2cpp::vm::MetadataCache::GetFieldOffsetFromIndexLocked(TypeIndex typeIndex, int32_t fieldIndexInType, FieldInfo* field, const il2cpp::os::FastAutoLock& lock)
{
    IL2CPP_ASSERT(typeIndex <= s_Il2CppMetadataRegistration->typeDefinitionsSizesCount);
    int32_t offset = s_Il2CppMetadataRegistration->fieldOffsets[typeIndex][fieldIndexInType];
    if (offset < 0)
    {
        AddThreadLocalStaticOffsetForFieldLocked(field, offset & ~THREAD_LOCAL_STATIC_MASK, lock);
        return THREAD_STATIC_FIELD_OFFSET;
    }
    return offset;
}

void il2cpp::vm::MetadataCache::AddThreadLocalStaticOffsetForFieldLocked(FieldInfo* field, int32_t offset, const il2cpp::os::FastAutoLock& lock)
{
    s_ThreadLocalStaticOffsetMap.add(field, offset);
}

int32_t il2cpp::vm::MetadataCache::GetThreadLocalStaticOffsetForField(FieldInfo* field)
{
    IL2CPP_ASSERT(field->offset == THREAD_STATIC_FIELD_OFFSET);

    il2cpp::os::FastAutoLock lock(&g_MetadataLock);
    Il2CppThreadLocalStaticOffsetHashMapIter iter = s_ThreadLocalStaticOffsetMap.find(field);
    IL2CPP_ASSERT(iter != s_ThreadLocalStaticOffsetMap.end());
    return iter->second;
}

int32_t il2cpp::vm::MetadataCache::GetReferenceAssemblyIndexIntoAssemblyTable(int32_t referencedAssemblyTableIndex)
{
    IL2CPP_ASSERT(referencedAssemblyTableIndex >= 0 && static_cast<uint32_t>(referencedAssemblyTableIndex) <= s_GlobalMetadataHeader->referencedAssembliesCount / sizeof(int32_t));
    const int32_t* referenceAssemblyIndicies = (const int32_t*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->referencedAssembliesOffset);
    return referenceAssemblyIndicies[referencedAssemblyTableIndex];
}

const TypeDefinitionIndex il2cpp::vm::MetadataCache::GetIndexForTypeDefinition(const Il2CppClass* typeDefinition)
{
    IL2CPP_ASSERT(typeDefinition->typeDefinition);
    const Il2CppTypeDefinition* typeDefinitions = (const Il2CppTypeDefinition*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->typeDefinitionsOffset);

    IL2CPP_ASSERT(typeDefinition->typeDefinition >= typeDefinitions && typeDefinition->typeDefinition < typeDefinitions + s_GlobalMetadataHeader->typeDefinitionsCount);

    ptrdiff_t index = typeDefinition->typeDefinition - typeDefinitions;
    IL2CPP_ASSERT(index <= std::numeric_limits<TypeDefinitionIndex>::max());
    return static_cast<TypeDefinitionIndex>(index);
}

const GenericParameterIndex il2cpp::vm::MetadataCache::GetIndexForGenericParameter(const Il2CppGenericParameter* genericParameter)
{
    const Il2CppGenericParameter* genericParameters = (const Il2CppGenericParameter*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->genericParametersOffset);

    IL2CPP_ASSERT(genericParameter >= genericParameters && genericParameter < genericParameters + s_GlobalMetadataHeader->genericParametersCount);

    ptrdiff_t index = genericParameter - genericParameters;
    IL2CPP_ASSERT(index <= std::numeric_limits<GenericParameterIndex>::max());
    return static_cast<GenericParameterIndex>(index);
}

const MethodIndex il2cpp::vm::MetadataCache::GetIndexForMethodDefinition(const MethodInfo* method)
{
    IL2CPP_ASSERT(!method->is_inflated);
    const Il2CppMethodDefinition* methodDefinitions = (const Il2CppMethodDefinition*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->methodsOffset);

    IL2CPP_ASSERT(method->methodDefinition >= methodDefinitions && method->methodDefinition < methodDefinitions + s_GlobalMetadataHeader->methodsCount);

    ptrdiff_t index = method->methodDefinition - methodDefinitions;
    IL2CPP_ASSERT(index <= std::numeric_limits<MethodIndex>::max());
    return static_cast<MethodIndex>(index);
}

static il2cpp::utils::OnceFlag s_CustomAttributesOnceFlag;

static void InitializeCustomAttributesCaches(void* arg)
{
    s_CustomAttributesCaches = (CustomAttributesCache**)IL2CPP_CALLOC(s_Il2CppCodeRegistration->customAttributeCount, sizeof(CustomAttributesCache*));
}

CustomAttributesCache* il2cpp::vm::MetadataCache::GenerateCustomAttributesCache(CustomAttributeIndex index)
{
    if (index == kCustomAttributeIndexInvalid)
        return NULL;

    IL2CPP_ASSERT(index >= 0 && index < s_Il2CppCodeRegistration->customAttributeCount);
    IL2CPP_ASSERT(index >= 0 && index < static_cast<int32_t>(s_GlobalMetadataHeader->attributesInfoCount / sizeof(Il2CppCustomAttributeTypeRange)));

    il2cpp::utils::CallOnce(s_CustomAttributesOnceFlag, &InitializeCustomAttributesCaches, NULL);

    // use atomics rather than a Mutex here to avoid deadlock. The attribute generators call arbitrary managed code
    CustomAttributesCache* cache = il2cpp::os::Atomic::ReadPointer(&s_CustomAttributesCaches[index]);
    if (cache == NULL)
    {
        const Il2CppCustomAttributeTypeRange* attributeTypeRange = MetadataOffset<const Il2CppCustomAttributeTypeRange*>(s_GlobalMetadata, s_GlobalMetadataHeader->attributesInfoOffset, index);

        cache = (CustomAttributesCache*)IL2CPP_CALLOC(1, sizeof(CustomAttributesCache));
        cache->count = attributeTypeRange->count;
        cache->attributes = (Il2CppObject**)il2cpp::gc::GarbageCollector::AllocateFixed(sizeof(Il2CppObject *) * cache->count, 0);

        for (int32_t i = 0; i < attributeTypeRange->count; i++)
        {
            IL2CPP_ASSERT(attributeTypeRange->start + i < s_GlobalMetadataHeader->attributeTypesCount);
            TypeIndex typeIndex = *MetadataOffset<TypeIndex*>(s_GlobalMetadata, s_GlobalMetadataHeader->attributeTypesOffset, attributeTypeRange->start + i);
            cache->attributes[i] = il2cpp::vm::Object::New(GetTypeInfoFromTypeIndex(typeIndex));
            il2cpp::gc::GarbageCollector::SetWriteBarrier((void**)cache->attributes + i);
        }

        // generated code calls the attribute constructor and sets any fields/properties
        s_Il2CppCodeRegistration->customAttributeGenerators[index](cache);

        CustomAttributesCache* original = il2cpp::os::Atomic::CompareExchangePointer(&s_CustomAttributesCaches[index], cache, (CustomAttributesCache*)NULL);
        if (original)
        {
            // A non-NULL return value indicates some other thread already generated this cache.
            // We need to cleanup the resources we allocated
            il2cpp::gc::GarbageCollector::FreeFixed(cache->attributes);
            IL2CPP_FREE(cache);

            cache = original;
        }
    }

    return cache;
}

static int CompareTokens(const void* pkey, const void* pelem)
{
    return (int)(((Il2CppCustomAttributeTypeRange*)pkey)->token - ((Il2CppCustomAttributeTypeRange*)pelem)->token);
}

CustomAttributeIndex il2cpp::vm::MetadataCache::GetCustomAttributeIndex(const Il2CppImage* image, uint32_t token)
{
    const Il2CppCustomAttributeTypeRange* attributeTypeRange = MetadataOffset<const Il2CppCustomAttributeTypeRange*>(s_GlobalMetadata, s_GlobalMetadataHeader->attributesInfoOffset, 0);

    Il2CppCustomAttributeTypeRange key;
    memset(&key, 0, sizeof(Il2CppCustomAttributeTypeRange));
    key.token = token;

    const Il2CppCustomAttributeTypeRange* res = (const Il2CppCustomAttributeTypeRange*)bsearch(&key, attributeTypeRange + image->customAttributeStart, image->customAttributeCount, sizeof(Il2CppCustomAttributeTypeRange), CompareTokens);

    if (res == NULL)
        return kCustomAttributeIndexInvalid;

    CustomAttributeIndex index = (CustomAttributeIndex)(res - attributeTypeRange);

    IL2CPP_ASSERT(index >= 0 && index < static_cast<int32_t>(s_GlobalMetadataHeader->attributesInfoCount / sizeof(Il2CppCustomAttributeTypeRange)));

    return index;
}

CustomAttributesCache* il2cpp::vm::MetadataCache::GenerateCustomAttributesCache(const Il2CppImage* image, uint32_t token)
{
    return GenerateCustomAttributesCache(GetCustomAttributeIndex(image, token));
}

bool il2cpp::vm::MetadataCache::HasAttribute(CustomAttributeIndex index, Il2CppClass* attribute)
{
    if (index == kCustomAttributeIndexInvalid)
        return false;

    IL2CPP_ASSERT(attribute);

    const Il2CppCustomAttributeTypeRange* attributeTypeRange = MetadataOffset<const Il2CppCustomAttributeTypeRange*>(s_GlobalMetadata, s_GlobalMetadataHeader->attributesInfoOffset, index);

    for (int32_t i = 0; i < attributeTypeRange->count; i++)
    {
        IL2CPP_ASSERT(attributeTypeRange->start + i < s_GlobalMetadataHeader->attributeTypesCount);
        TypeIndex typeIndex = *MetadataOffset<TypeIndex*>(s_GlobalMetadata, s_GlobalMetadataHeader->attributeTypesOffset, attributeTypeRange->start + i);
        Il2CppClass* klass = GetTypeInfoFromTypeIndex(typeIndex);

        if (il2cpp::vm::Class::HasParent(klass, attribute) || (il2cpp::vm::Class::IsInterface(attribute) && il2cpp::vm::Class::IsAssignableFrom(attribute, klass)))
            return true;
    }

    return false;
}

bool il2cpp::vm::MetadataCache::HasAttribute(const Il2CppImage* image, uint32_t token, Il2CppClass* attribute)
{
    return HasAttribute(GetCustomAttributeIndex(image, token), attribute);
}

Il2CppString* il2cpp::vm::MetadataCache::GetStringLiteralFromIndex(StringLiteralIndex index)
{
    if (index == kStringLiteralIndexInvalid)
        return NULL;

    IL2CPP_ASSERT(index >= 0 && static_cast<uint32_t>(index) < s_GlobalMetadataHeader->stringLiteralCount / sizeof(Il2CppStringLiteral) && "Invalid string literal index ");

    if (s_StringLiteralTable[index])
        return s_StringLiteralTable[index];

    const Il2CppStringLiteral* stringLiteral = (const Il2CppStringLiteral*)((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->stringLiteralOffset) + index;
    s_StringLiteralTable[index] = String::NewLen((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->stringLiteralDataOffset + stringLiteral->dataIndex, stringLiteral->length);
    il2cpp::gc::GarbageCollector::SetWriteBarrier((void**)s_StringLiteralTable + index);

    return s_StringLiteralTable[index];
}

const char* il2cpp::vm::MetadataCache::GetStringFromIndex(StringIndex index)
{
    IL2CPP_ASSERT(index <= s_GlobalMetadataHeader->stringCount);
    const char* strings = ((const char*)s_GlobalMetadata + s_GlobalMetadataHeader->stringOffset) + index;
    return strings;
}

FieldInfo* il2cpp::vm::MetadataCache::GetFieldInfoFromIndex(EncodedMethodIndex index)
{
    IL2CPP_ASSERT(s_GlobalMetadataHeader->fieldRefsCount >= 0 && index <= static_cast<uint32_t>(s_GlobalMetadataHeader->fieldRefsCount));

    const Il2CppFieldRef* fieldRef = MetadataOffset<const Il2CppFieldRef*>(s_GlobalMetadata, s_GlobalMetadataHeader->fieldRefsOffset, index);
    Il2CppClass* typeInfo = GetTypeInfoFromTypeIndex(fieldRef->typeIndex);
    return typeInfo->fields + fieldRef->fieldIndex;
}

static bool IsMatchingUsage(Il2CppMetadataUsage usage, const il2cpp::utils::dynamic_array<Il2CppMetadataUsage>& expectedUsages)
{
    if (expectedUsages.empty())
        return true;

    size_t numberOfExpectedUsages = expectedUsages.size();
    for (size_t i = 0; i < numberOfExpectedUsages; ++i)
    {
        if (expectedUsages[i] == usage)
            return true;
    }

    return false;
}

// This method can be called from multiple threads, so it does have a data race. However, each
// thread is reading from the same read-only metadata, so each thread will set the same values.
// Therefore, we can safely ignore thread sanitizer issues in this method.
void il2cpp::vm::MetadataCache::IntializeMethodMetadataRange(uint32_t start, uint32_t count, const il2cpp::utils::dynamic_array<Il2CppMetadataUsage>& expectedUsages) IL2CPP_DISABLE_TSAN
{
    for (uint32_t i = 0; i < count; i++)
    {
        uint32_t offset = start + i;
        IL2CPP_ASSERT(s_GlobalMetadataHeader->metadataUsagePairsCount >= 0 && offset <= static_cast<uint32_t>(s_GlobalMetadataHeader->metadataUsagePairsCount));
        const Il2CppMetadataUsagePair* metadataUsagePairs = MetadataOffset<const Il2CppMetadataUsagePair*>(s_GlobalMetadata, s_GlobalMetadataHeader->metadataUsagePairsOffset, offset);
        uint32_t destinationIndex = metadataUsagePairs->destinationIndex;
        uint32_t encodedSourceIndex = metadataUsagePairs->encodedSourceIndex;

        Il2CppMetadataUsage usage = GetEncodedIndexType(encodedSourceIndex);
        if (IsMatchingUsage(usage, expectedUsages))
        {
            uint32_t decodedIndex = GetDecodedMethodIndex(encodedSourceIndex);
            switch (usage)
            {
                case kIl2CppMetadataUsageTypeInfo:
                    *s_Il2CppMetadataRegistration->metadataUsages[destinationIndex] = GetTypeInfoFromTypeIndex(decodedIndex);
                    break;
                case kIl2CppMetadataUsageIl2CppType:
                    *s_Il2CppMetadataRegistration->metadataUsages[destinationIndex] = const_cast<Il2CppType*>(GetIl2CppTypeFromIndex(decodedIndex));
                    break;
                case kIl2CppMetadataUsageMethodDef:
                case kIl2CppMetadataUsageMethodRef:
                    *s_Il2CppMetadataRegistration->metadataUsages[destinationIndex] = const_cast<MethodInfo*>(GetMethodInfoFromIndex(encodedSourceIndex));
                    break;
                case kIl2CppMetadataUsageFieldInfo:
                    *s_Il2CppMetadataRegistration->metadataUsages[destinationIndex] = GetFieldInfoFromIndex(decodedIndex);
                    break;
                case kIl2CppMetadataUsageStringLiteral:
                    *s_Il2CppMetadataRegistration->metadataUsages[destinationIndex] = GetStringLiteralFromIndex(decodedIndex);
                    break;
                default:
                    IL2CPP_NOT_IMPLEMENTED(il2cpp::vm::MetadataCache::InitializeMethodMetadata);
                    break;
            }
        }
    }
}

void il2cpp::vm::MetadataCache::InitializeAllMethodMetadata()
{
    il2cpp::utils::dynamic_array<Il2CppMetadataUsage> onlyAcceptMethodUsages;
    onlyAcceptMethodUsages.push_back(kIl2CppMetadataUsageMethodDef);
    onlyAcceptMethodUsages.push_back(kIl2CppMetadataUsageMethodRef);
    onlyAcceptMethodUsages.push_back(kIl2CppMetadataUsageTypeInfo);
    IntializeMethodMetadataRange(0, s_GlobalMetadataHeader->metadataUsagePairsCount / sizeof(Il2CppMetadataUsagePair), onlyAcceptMethodUsages);
}

void il2cpp::vm::MetadataCache::InitializeMethodMetadata(uint32_t index)
{
    IL2CPP_ASSERT(s_GlobalMetadataHeader->metadataUsageListsCount >= 0 && index <= static_cast<uint32_t>(s_GlobalMetadataHeader->metadataUsageListsCount));

    const Il2CppMetadataUsageList* metadataUsageLists = MetadataOffset<const Il2CppMetadataUsageList*>(s_GlobalMetadata, s_GlobalMetadataHeader->metadataUsageListsOffset, index);

    uint32_t start = metadataUsageLists->start;
    uint32_t count = metadataUsageLists->count;

    il2cpp::utils::dynamic_array<Il2CppMetadataUsage> acceptAllUsages;
    IntializeMethodMetadataRange(start, count, acceptAllUsages);
}

void il2cpp::vm::MetadataCache::WalkPointerTypes(WalkTypesCallback callback, void* context)
{
    il2cpp::os::FastAutoLock lock(&s_MetadataCache.m_CacheMutex);
    for (PointerTypeMap::iterator it = s_MetadataCache.m_PointerTypes.begin(); it != s_MetadataCache.m_PointerTypes.end(); it++)
    {
        callback(it->second, context);
    }
}
