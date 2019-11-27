#pragma once

#include "il2cpp-codegen-metadata.h"

#if RUNTIME_MONO
typedef MonoStringBuilder RuntimeStringBuilder;
#include "il2cpp-codegen-mono.h"
#else
struct Il2CppStringBuilder;
typedef Il2CppStringBuilder RuntimeStringBuilder;
#include "il2cpp-codegen-il2cpp.h"
#endif
