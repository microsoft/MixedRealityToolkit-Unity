#include "il2cpp-config.h"
#include "vm/AssemblyName.h"
#include "vm/MetadataCache.h"
#include "vm/Runtime.h"
#include "il2cpp-tabledefs.h"
#include "il2cpp-class-internals.h"
#include "vm/Array.h"
#include "il2cpp-object-internals.h"
#include "vm/Reflection.h"
#include "vm/Type.h"
#include "vm/Object.h"
#include "utils/StringUtils.h"
#include "vm/String.h"
#include "vm/Class.h"
#include "mono-structs.h"

#include <vector>
#include <string>

namespace il2cpp
{
namespace vm
{
    static Il2CppObject* CreateVersion(uint32_t major, uint32_t minor, uint32_t build, uint32_t revision)
    {
        static const MethodInfo* versionContructor = NULL;
        if (!versionContructor)
            versionContructor = Class::GetMethodFromName(il2cpp_defaults.version, ".ctor", 4);

        Il2CppObject* version = Object::New(il2cpp_defaults.version);
        void* args[4] = { &major, &minor, &build, &revision };
        Runtime::Invoke(versionContructor, version, args, NULL);

        return version;
    }

    static Il2CppObject* CreateCulture(const char* cultureName)
    {
        static const MethodInfo* createCultureMethod = NULL;
        if (!createCultureMethod)
            createCultureMethod = Class::GetMethodFromName(il2cpp_defaults.culture_info, "CreateCulture", 2);

        bool reference = false;
        void* args[2];
        if (cultureName != NULL)
            args[0] = String::New(cultureName);
        else
            args[0] = String::New("neutral");
        args[1] = &reference;
        return Runtime::Invoke(createCultureMethod, NULL, args, NULL);
    }

    bool AssemblyName::ParseName(Il2CppReflectionAssemblyName* aname, std::string assemblyName)
    {
        il2cpp::vm::TypeNameParseInfo info;
        il2cpp::vm::TypeNameParser parser(assemblyName, info, false);

        if (!parser.ParseAssembly())
            return false;

        const il2cpp::vm::TypeNameParseInfo::AssemblyName& parsedName = info.assembly_name();
        IL2CPP_OBJECT_SETREF(aname, name, String::New(parsedName.name.c_str()));
        aname->major = parsedName.major;
        aname->minor = parsedName.minor;
        aname->build = parsedName.build;
        aname->revision = parsedName.revision;
        aname->flags = parsedName.flags;
        aname->hashalg = parsedName.hash_alg;

        IL2CPP_OBJECT_SETREF(aname, version, CreateVersion(parsedName.major, parsedName.minor, parsedName.build, parsedName.revision));
        IL2CPP_OBJECT_SETREF(aname, cultureInfo, CreateCulture(parsedName.culture.c_str()));

        if (parsedName.public_key_token[0])
        {
            IL2CPP_OBJECT_SETREF(aname, keyToken, Array::New(il2cpp_defaults.byte_class, kPublicKeyByteLength));
            char* p = il2cpp_array_addr(aname->keyToken, char, 0);

            char buf[2] = { 0 };
            for (int i = 0, j = 0; i < kPublicKeyByteLength; i++)
            {
                buf[0] = parsedName.public_key_token[j++];
                *p = (char)(strtol(buf, NULL, 16) << 4);
                buf[0] = parsedName.public_key_token[j++];
                *p |= (char)strtol(buf, NULL, 16);
                p++;
            }
        }
        else
            IL2CPP_OBJECT_SETREF(aname, keyToken, Array::New(il2cpp_defaults.byte_class, 0));

        return true;
    }

    static char HexValueToLowercaseAscii(uint8_t hexValue)
    {
        if (hexValue < 10)
            return char(hexValue + 48);

        return char(hexValue + 87);
    }

#if NET_4_0

    uint8_t* EncodeStringBlob(const char* original)
    {
        size_t stringLength = strlen(original);
        uint32_t sizeForLength;
        uint8_t encodedLength[4];

        if (stringLength < 0x80)
        {
            sizeForLength = 1;
            encodedLength[0] = static_cast<uint8_t>(stringLength);
        }
        else if (stringLength < 0x4000)
        {
            sizeForLength = 2;
            encodedLength[0] = static_cast<uint8_t>(stringLength >> 8) | 0x80;
            encodedLength[1] = static_cast<uint8_t>(stringLength & 0xFF);
        }
        else
        {
            sizeForLength = 4;
            encodedLength[0] = static_cast<uint8_t>(stringLength >> 24) | 0xC0;
            encodedLength[1] = static_cast<uint8_t>((stringLength >> 16) & 0xFF);
            encodedLength[2] = static_cast<uint8_t>((stringLength >> 8) & 0xFF);
            encodedLength[3] = static_cast<uint8_t>(stringLength & 0xFF);
        }

        uint8_t* result = static_cast<uint8_t*>(IL2CPP_MALLOC(stringLength + sizeForLength + 1));

        memcpy(result, encodedLength, sizeForLength);
        strncpy(reinterpret_cast<char*>(result + sizeForLength), original, stringLength + 1);
        return result;
    }

    void AssemblyName::FillNativeAssemblyName(const Il2CppAssemblyName& aname, Il2CppMonoAssemblyName* nativeName)
    {
        nativeName->name = il2cpp::utils::StringUtils::StringDuplicate(aname.name);
        nativeName->culture = il2cpp::utils::StringUtils::StringDuplicate(aname.culture);
        nativeName->hash_value = il2cpp::utils::StringUtils::StringDuplicate(aname.hash_value);
        nativeName->public_key = aname.public_key != NULL ? EncodeStringBlob(aname.public_key) : NULL;
        nativeName->hash_alg = aname.hash_alg;
        nativeName->hash_len = aname.hash_len;
        nativeName->flags = aname.flags;
        nativeName->major = aname.major;
        nativeName->minor = aname.minor;
        nativeName->build = aname.build;
        nativeName->revision = aname.revision;

        //Mono public key token is stored as hexadecimal characters
        if (aname.public_key_token[0])
        {
            int j = 0;
            for (int i = 0; i < kPublicKeyByteLength; ++i)
            {
                uint8_t value = aname.public_key_token[i];
                nativeName->public_key_token.padding[j++] = HexValueToLowercaseAscii((value & 0xF0) >> 4);
                nativeName->public_key_token.padding[j++] = HexValueToLowercaseAscii(value & 0x0F);
            }
        }
    }

#endif

    static std::string PublicKeyTokenToString(const uint8_t* public_key_token)
    {
        std::string result(kPublicKeyByteLength * 2, '0');
        for (int i = 0; i < kPublicKeyByteLength; ++i)
        {
            uint8_t hi = (public_key_token[i] & 0xF0) >> 4;
            uint8_t lo = public_key_token[i] & 0x0F;

            result[i * 2] = HexValueToLowercaseAscii(hi);
            result[i * 2 + 1] = HexValueToLowercaseAscii(lo);
        }

        return result;
    }

    std::string AssemblyName::AssemblyNameToString(const Il2CppAssemblyName& aname)
    {
        std::string name;

        char buffer[1024];

        name += aname.name;
        name += ", Version=";
        sprintf(buffer, "%d", aname.major);
        name += buffer;
        name += ".";
        sprintf(buffer, "%d", aname.minor);
        name += buffer;
        name += ".";
        sprintf(buffer, "%d", aname.build);
        name += buffer;
        name += ".";
        sprintf(buffer, "%d", aname.revision);
        name += buffer;
        name += ", Culture=";
        const char* culture = NULL;
        culture = aname.culture;
        name += (culture != NULL && strlen(culture) != 0 ? culture : "neutral");
        name += ", PublicKeyToken=";
        name += (aname.public_key_token[0] ? PublicKeyTokenToString(aname.public_key_token) : "null");
        name += ((aname.flags & ASSEMBLYREF_RETARGETABLE_FLAG) ? ", Retargetable=Yes" : "");

        if (strcmp(aname.name, "WindowsRuntimeMetadata") == 0)
            name += ", ContentType=WindowsRuntime";

        return name;
    }
} /* namespace vm */
} /* namespace il2cpp */
