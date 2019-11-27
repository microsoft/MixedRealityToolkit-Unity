#include "il2cpp-config.h"
#include <ctype.h>
#include "icalls/mscorlib/System/Convert.h"
#include "il2cpp-api.h"
#include "il2cpp-class-internals.h"
#include "il2cpp-object-internals.h"
#include "vm/Array.h"
#include "vm/Image.h"
#include "vm/Exception.h"

namespace il2cpp
{
namespace icalls
{
namespace mscorlib
{
namespace System
{
    Il2CppArray* Convert::InternalFromBase64String(Il2CppString* str, bool allowWhitespaceOnly)
    {
        return Base64ToByteArray(str->chars, str->length, allowWhitespaceOnly);
    }

    Il2CppArray* Convert::InternalFromBase64CharArray(Il2CppArray* chars, int32_t offset, int32_t length)
    {
        return Base64ToByteArray(il2cpp_array_addr(chars, Il2CppChar, offset), length, false);
    }

    Il2CppArray* Convert::Base64ToByteArray(Il2CppChar* start, int length, bool allowWhitespaceOnly)
    {
        int ignored;
        int i;
        Il2CppChar c;
        Il2CppChar last, prev_last, prev2_last;
        int olength;
        Il2CppArray *result;
        unsigned char *res_ptr;
        int a[4], b[4];

        const static unsigned char dbase64[] =
        {
            128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128,
            128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128,
            128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 128, 62, 128, 128, 128, 63,
            52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 128, 128, 128, 0, 128, 128,
            128, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
            15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 128, 128, 128, 128, 128,
            128, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
            41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51
        };

        ignored = 0;
        last = prev_last = 0, prev2_last = 0;
        for (i = 0; i < length; i++)
        {
            c = start[i];
            if (c >= sizeof(dbase64))
            {
                // The content of this exception message matches Mono, but differs from .Net.
                il2cpp_raise_exception(vm::Exception::GetFormatException("Invalid character found."));
            }
            else if (isspace(c))
            {
                ignored++;
            }
            else
            {
                prev2_last = prev_last;
                prev_last = last;
                last = c;
            }
        }

        olength = length - ignored;

        if (allowWhitespaceOnly && olength == 0)
            return vm::Array::New(il2cpp_defaults.byte_class, 0);

        if ((olength & 3) != 0 || olength <= 0)
        {
            // The content of this exception message matches Mono, but differs from .Net.
            il2cpp_raise_exception(vm::Exception::GetFormatException("Invalid length."));
        }

        if (prev2_last == '=')
        {
            // The content of this exception message matches Mono, but differs from .Net.
            il2cpp_raise_exception(vm::Exception::GetFormatException("Invalid format."));
        }

        olength = (olength * 3) / 4;
        if (last == '=')
            olength--;

        if (prev_last == '=')
            olength--;

        result = vm::Array::New(il2cpp_defaults.byte_class, olength);
        res_ptr = il2cpp_array_addr(result, unsigned char, 0);
        for (i = 0; i < length;)
        {
            int k;

            for (k = 0; k < 4 && i < length;)
            {
                c = start[i++];
                if (isspace(c))
                    continue;

                a[k] = (unsigned char)c;
                if (((b[k] = dbase64[c]) & 0x80) != 0)
                {
                    // The content of this exception message matches Mono, but differs from .Net.
                    il2cpp_raise_exception(vm::Exception::GetFormatException("Invalid character found."));
                }
                k++;
            }

            *res_ptr++ = (b[0] << 2) | (b[1] >> 4);
            if (a[2] != '=')
                *res_ptr++ = (b[1] << 4) | (b[2] >> 2);
            if (a[3] != '=')
                *res_ptr++ = (b[2] << 6) | b[3];

            while (i < length && isspace(start[i]))
                i++;
        }

        return result;
    }
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
