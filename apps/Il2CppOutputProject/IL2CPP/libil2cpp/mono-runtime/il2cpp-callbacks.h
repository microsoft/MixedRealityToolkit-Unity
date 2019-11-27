#pragma once

#if RUNTIME_MONO

#if _MSC_VER
typedef wchar_t Il2CppChar;
#elif __has_feature(cxx_unicode_literals)
typedef char16_t Il2CppChar;
#else
typedef uint16_t Il2CppChar;
#endif

void il2cpp_install_callbacks();
void il2cpp_mono_runtime_init();
void il2cpp_mono_set_config_utf16(const Il2CppChar* executablePath);
void il2cpp_mono_set_config(const char* executablePath);
void il2cpp_mono_set_commandline_arguments_utf16(int argc, const Il2CppChar* const* argv);
void il2cpp_mono_set_commandline_arguments(int argc, const char* const* argv);
void il2cpp_mono_initialize_metadata();

#endif
