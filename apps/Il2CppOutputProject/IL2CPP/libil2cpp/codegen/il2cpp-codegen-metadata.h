#pragma once

typedef struct String_t String_t;
struct Type_t;
struct Exception_t;
struct StringBuilder_t;
struct MulticastDelegate_t;
struct MethodBase_t;
struct Assembly_t;

#include "il2cpp-class-internals.h"

#if RUNTIME_MONO
IL2CPP_EXTERN_C_BEGIN
#include <mono/metadata/class.h>
#include <mono/metadata/image.h>
#include <mono/metadata/metadata.h>
#include <mono/metadata/object.h>
#include <mono/metadata/object-internals.h>
IL2CPP_EXTERN_C_END

typedef MonoClass RuntimeClass;
typedef MonoMethod RuntimeMethod;
typedef MonoClassField RuntimeField;
typedef MonoType RuntimeType;
typedef MonoObject RuntimeObject;
typedef MonoImage RuntimeImage;
typedef MonoException RuntimeException;
typedef MonoArray RuntimeArray;
typedef MonoAssembly RuntimeAssembly;
typedef MonoString RuntimeString;
typedef MonoDelegate RuntimeDelegate;

#else

struct TypeInfo;
struct MethodInfo;
struct FieldInfo;
struct Il2CppType;
typedef Il2CppClass RuntimeClass;
typedef MethodInfo RuntimeMethod;
typedef FieldInfo RuntimeField;
typedef Il2CppType RuntimeType;
typedef Il2CppObject RuntimeObject;
typedef Il2CppImage RuntimeImage;
typedef Il2CppException RuntimeException;
typedef Il2CppArray RuntimeArray;
typedef Il2CppAssembly RuntimeAssembly;
typedef Il2CppString RuntimeString;
typedef Il2CppDelegate RuntimeDelegate;
#endif
