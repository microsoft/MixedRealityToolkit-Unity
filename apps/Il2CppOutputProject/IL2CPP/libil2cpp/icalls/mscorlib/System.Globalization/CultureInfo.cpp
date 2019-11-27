#include "il2cpp-config.h"
#include <string>
#include <algorithm>
#include "il2cpp-api.h"
#include "il2cpp-object-internals.h"
#include "il2cpp-class-internals.h"
#include "utils/Memory.h"
#include "utils/StringUtils.h"
#include "vm/Array.h"
#include "vm/Exception.h"
#include "vm/String.h"
#include "os/Locale.h"
#include "icalls/mscorlib/System.Globalization/CultureInfo.h"
#include "CultureInfoInternals.h"
#include "CultureInfoTables.h"


/*
* The following methods is modified from the ICU source code. (http://oss.software.ibm.com/icu)
* Copyright (c) 1995-2003 International Business Machines Corporation and others
* All rights reserved.
*/
static std::string get_current_locale_name(void)
{
    char* locale;
    char* corrected = NULL;
    const char* p;

    std::string locale_str = il2cpp::os::Locale::GetLocale();

    if (locale_str.empty())
        return std::string();

    locale = il2cpp::utils::StringUtils::StringDuplicate(locale_str.c_str());

    if ((p = strchr(locale, '.')) != NULL)
    {
        /* assume new locale can't be larger than old one? */
        corrected = (char*)IL2CPP_MALLOC(strlen(locale));
        strncpy(corrected, locale, p - locale);
        corrected[p - locale] = 0;

        /* do not copy after the @ */
        if ((p = strchr(corrected, '@')) != NULL)
            corrected[p - corrected] = 0;
    }

    /* Note that we scan the *uncorrected* ID. */
    if ((p = strrchr(locale, '@')) != NULL)
    {
        /*
        * Mono we doesn't handle the '@' modifier because it does
        * not have any cultures that use it. Just trim it
        * off of the end of the name.
        */

        if (corrected == NULL)
        {
            corrected = (char*)IL2CPP_MALLOC(strlen(locale));
            strncpy(corrected, locale, p - locale);
            corrected[p - locale] = 0;
        }
    }

    if (corrected == NULL)
        corrected = locale;
    else
        IL2CPP_FREE(locale);

    char* c;
    if ((c = strchr(corrected, '_')) != NULL)
        *c = '-';

    std::string result(corrected);
    IL2CPP_FREE(corrected);

    std::transform(result.begin(), result.end(), result.begin(), ::tolower);

    return result;
}

namespace il2cpp
{
namespace icalls
{
namespace mscorlib
{
namespace System
{
namespace Globalization
{
    static Il2CppArray* culture_info_create_names_array_idx(const uint16_t* names, int max)
    {
        if (names == NULL)
            return NULL;

        int len = 0;
        for (int i = 0; i < max; i++)
        {
            if (names[i] == 0)
                break;
            len++;
        }

        Il2CppArray* ret = il2cpp_array_new_specific(il2cpp_array_class_get(il2cpp_defaults.string_class, 1), len);

        for (int i = 0; i < len; i++)
#if !NET_4_0
            il2cpp_array_setref(ret, i, il2cpp_string_new(idx2string(names[i])));
#else
            il2cpp_array_setref(ret, i, il2cpp_string_new(dtidx2string(names[i])));
#endif

        return ret;
    }

    static bool construct_culture(Il2CppCultureInfo* cultureInfo, const CultureInfoEntry *ci)
    {
        cultureInfo->lcid = ci->lcid;
        IL2CPP_OBJECT_SETREF(cultureInfo, name, il2cpp_string_new(idx2string(ci->name)));
#if !NET_4_0
        IL2CPP_OBJECT_SETREF(cultureInfo, icu_name, il2cpp_string_new(idx2string(ci->icu_name)));
        IL2CPP_OBJECT_SETREF(cultureInfo, displayname, il2cpp_string_new(idx2string(ci->displayname)));
#endif
        IL2CPP_OBJECT_SETREF(cultureInfo, englishname, il2cpp_string_new(idx2string(ci->englishname)));
        IL2CPP_OBJECT_SETREF(cultureInfo, nativename, il2cpp_string_new(idx2string(ci->nativename)));
        IL2CPP_OBJECT_SETREF(cultureInfo, win3lang, il2cpp_string_new(idx2string(ci->win3lang)));
        IL2CPP_OBJECT_SETREF(cultureInfo, iso3lang, il2cpp_string_new(idx2string(ci->iso3lang)));
        IL2CPP_OBJECT_SETREF(cultureInfo, iso2lang, il2cpp_string_new(idx2string(ci->iso2lang)));

#if !NET_4_0
        IL2CPP_OBJECT_SETREF(cultureInfo, territory, il2cpp_string_new(idx2string(ci->territory)));
#else
        // It's null for neutral cultures
        if (ci->territory > 0)
            IL2CPP_OBJECT_SETREF(cultureInfo, territory, il2cpp_string_new(idx2string(ci->territory)));
#endif
        cultureInfo->parent_lcid = ci->parent_lcid;
#if !NET_4_0
        cultureInfo->specific_lcid = ci->specific_lcid;
        cultureInfo->calendar_data = ci->calendar_data;
#endif
        cultureInfo->datetime_index = ci->datetime_format_index;
        cultureInfo->number_index = ci->number_format_index;
        cultureInfo->text_info_data = &ci->text_info;

#if NET_4_0
        IL2CPP_OBJECT_SETREF(cultureInfo, native_calendar_names, culture_info_create_names_array_idx(ci->native_calendar_names, NUM_CALENDARS));
        cultureInfo->default_calendar_type = ci->calendar_type;
#endif

        return true;
    }

    static int culture_info_culture_name_locator(const void *a, const void *b)
    {
        const char* aa = (const char*)a;
        const CultureInfoNameEntry* bb = (const CultureInfoNameEntry*)b;
        int ret;

        ret = strcmp(aa, idx2string(bb->name));

        return ret;
    }

    static int culture_lcid_locator(const void *a, const void *b)
    {
        const CultureInfoEntry *aa = (const CultureInfoEntry*)a;
        const CultureInfoEntry *bb = (const CultureInfoEntry*)b;

        return (aa->lcid - bb->lcid);
    }

    static const CultureInfoEntry* culture_info_entry_from_lcid(int lcid)
    {
        CultureInfoEntry key;
        key.lcid = lcid;
        return (const CultureInfoEntry*)bsearch(&key, culture_entries, NUM_CULTURE_ENTRIES, sizeof(CultureInfoEntry), culture_lcid_locator);
    }

#if !NET_4_0
    static bool construct_culture_from_specific_name(Il2CppCultureInfo* cultureInfo, const char *name)
    {
        const CultureInfoNameEntry* ne = (const CultureInfoNameEntry*)bsearch(name, culture_name_entries, NUM_CULTURE_ENTRIES, sizeof(CultureInfoNameEntry), culture_info_culture_name_locator);

        if (ne == NULL)
            return false;

        const CultureInfoEntry* entry = &culture_entries[ne->culture_entry_index];

        /* try avoiding another lookup, often the culture is its own specific culture */
        if (entry->lcid != entry->specific_lcid)
            entry = culture_info_entry_from_lcid(entry->specific_lcid);

        if (entry)
            return construct_culture(cultureInfo, entry);
        else
            return false;
    }

#endif

    static Il2CppArray* culture_info_create_group_sizes_array(const int *gs, int ml)
    {
        int i, len = 0;

        for (i = 0; i < ml; i++)
        {
            if (gs[i] == -1)
                break;
            len++;
        }

        Il2CppArray* ret = il2cpp_array_new_specific(il2cpp_array_class_get(il2cpp_defaults.int32_class, 1), len);

        for (i = 0; i < len; i++)
            il2cpp_array_set(ret, int32_t, i, gs[i]);

        return ret;
    }

#if !NET_4_0
    void CultureInfo::construct_datetime_format(Il2CppCultureInfo* cultureInfo)
    {
        IL2CPP_ASSERT(cultureInfo->datetime_index >= 0);

        Il2CppDateTimeFormatInfo* datetime = cultureInfo->datetime_format;
        const DateTimeFormatEntry* dfe = &datetime_format_entries[cultureInfo->datetime_index];

        datetime->readOnly = cultureInfo->is_read_only;
        IL2CPP_OBJECT_SETREF(datetime, AbbreviatedDayNames, culture_info_create_names_array_idx(dfe->abbreviated_day_names, NUM_DAYS));
        IL2CPP_OBJECT_SETREF(datetime, AbbreviatedMonthNames, culture_info_create_names_array_idx(dfe->abbreviated_month_names, NUM_MONTHS));
        IL2CPP_OBJECT_SETREF(datetime, AMDesignator, il2cpp_string_new(idx2string(dfe->am_designator)));
        datetime->CalendarWeekRule = dfe->calendar_week_rule;
        IL2CPP_OBJECT_SETREF(datetime, DateSeparator, il2cpp_string_new(idx2string(dfe->date_separator)));
        IL2CPP_OBJECT_SETREF(datetime, DayNames, culture_info_create_names_array_idx(dfe->day_names, NUM_DAYS));
        datetime->FirstDayOfWeek = dfe->first_day_of_week;
        IL2CPP_OBJECT_SETREF(datetime, FullDateTimePattern, il2cpp_string_new(idx2string(dfe->full_date_time_pattern)));
        IL2CPP_OBJECT_SETREF(datetime, LongDatePattern, il2cpp_string_new(idx2string(dfe->long_date_pattern)));
        IL2CPP_OBJECT_SETREF(datetime, LongTimePattern, il2cpp_string_new(idx2string(dfe->long_time_pattern)));
        IL2CPP_OBJECT_SETREF(datetime, MonthDayPattern, il2cpp_string_new(idx2string(dfe->month_day_pattern)));
        IL2CPP_OBJECT_SETREF(datetime, MonthNames, culture_info_create_names_array_idx(dfe->month_names, NUM_MONTHS));
        IL2CPP_OBJECT_SETREF(datetime, PMDesignator, il2cpp_string_new(idx2string(dfe->pm_designator)));
        IL2CPP_OBJECT_SETREF(datetime, ShortDatePattern, il2cpp_string_new(idx2string(dfe->short_date_pattern)));
        IL2CPP_OBJECT_SETREF(datetime, ShortTimePattern, il2cpp_string_new(idx2string(dfe->short_time_pattern)));
        IL2CPP_OBJECT_SETREF(datetime, TimeSeparator, il2cpp_string_new(idx2string(dfe->time_separator)));
        IL2CPP_OBJECT_SETREF(datetime, YearMonthPattern, il2cpp_string_new(idx2string(dfe->year_month_pattern)));
        IL2CPP_OBJECT_SETREF(datetime, ShortDatePatterns, culture_info_create_names_array_idx(dfe->short_date_patterns, NUM_SHORT_DATE_PATTERNS));
        IL2CPP_OBJECT_SETREF(datetime, LongDatePatterns, culture_info_create_names_array_idx(dfe->long_date_patterns, NUM_LONG_DATE_PATTERNS));
        IL2CPP_OBJECT_SETREF(datetime, ShortTimePatterns, culture_info_create_names_array_idx(dfe->short_time_patterns, NUM_SHORT_TIME_PATTERNS));
        IL2CPP_OBJECT_SETREF(datetime, LongTimePatterns, culture_info_create_names_array_idx(dfe->long_time_patterns, NUM_LONG_TIME_PATTERNS));
    }

    bool CultureInfo::construct_internal_locale_from_current_locale(Il2CppCultureInfo* cultureInfo)
    {
        std::string locale = ::get_current_locale_name();
        if (locale.empty())
            return false;

        bool status = construct_culture_from_specific_name(cultureInfo, locale.c_str());
        cultureInfo->is_read_only = true;
        cultureInfo->use_user_override = true;

        return status;
    }

#endif

    bool CultureInfo::construct_internal_locale_from_lcid(Il2CppCultureInfo* cultureInfo, int lcid)
    {
        const CultureInfoEntry* ci = culture_info_entry_from_lcid(lcid);
        if (ci == NULL)
            return false;

        return construct_culture(cultureInfo, ci);
    }

    bool CultureInfo::construct_internal_locale_from_name(Il2CppCultureInfo* cultureInfo, Il2CppString* name)
    {
        std::string cultureName = il2cpp::utils::StringUtils::Utf16ToUtf8(name->chars);
        const CultureInfoNameEntry* ne = (const CultureInfoNameEntry*)bsearch(cultureName.c_str(), culture_name_entries, NUM_CULTURE_ENTRIES, sizeof(CultureInfoNameEntry), culture_info_culture_name_locator);

        if (ne == NULL)
            return false;

        return construct_culture(cultureInfo, &culture_entries[ne->culture_entry_index]);
    }

#if !NET_4_0
    void CultureInfo::construct_number_format(Il2CppCultureInfo* cultureInfo)
    {
        IL2CPP_ASSERT(cultureInfo->number_format != 0);

        if (cultureInfo->number_index < 0)
            return;

        Il2CppNumberFormatInfo* number = cultureInfo->number_format;
        const NumberFormatEntry* nfe = &number_format_entries[cultureInfo->number_index];

        number->readOnly = cultureInfo->is_read_only;
        number->currencyDecimalDigits = nfe->currency_decimal_digits;
        IL2CPP_OBJECT_SETREF(number, currencyDecimalSeparator, il2cpp_string_new(idx2string(nfe->currency_decimal_separator)));
        IL2CPP_OBJECT_SETREF(number, currencyGroupSeparator, il2cpp_string_new(idx2string(nfe->currency_group_separator)));
        IL2CPP_OBJECT_SETREF(number, currencyGroupSizes, culture_info_create_group_sizes_array(nfe->currency_group_sizes, GROUP_SIZE));
        number->currencyNegativePattern = nfe->currency_negative_pattern;
        number->currencyPositivePattern = nfe->currency_positive_pattern;
        IL2CPP_OBJECT_SETREF(number, currencySymbol, il2cpp_string_new(idx2string(nfe->currency_symbol)));
        IL2CPP_OBJECT_SETREF(number, naNSymbol, il2cpp_string_new(idx2string(nfe->nan_symbol)));
        IL2CPP_OBJECT_SETREF(number, negativeInfinitySymbol, il2cpp_string_new(idx2string(nfe->negative_infinity_symbol)));
        IL2CPP_OBJECT_SETREF(number, negativeSign, il2cpp_string_new(idx2string(nfe->negative_sign)));
        number->numberDecimalDigits = nfe->number_decimal_digits;
        IL2CPP_OBJECT_SETREF(number, numberDecimalSeparator, il2cpp_string_new(idx2string(nfe->number_decimal_separator)));
        IL2CPP_OBJECT_SETREF(number, numberGroupSeparator, il2cpp_string_new(idx2string(nfe->number_group_separator)));
        IL2CPP_OBJECT_SETREF(number, numberGroupSizes, culture_info_create_group_sizes_array(nfe->number_group_sizes, GROUP_SIZE));
        number->numberNegativePattern = nfe->number_negative_pattern;
        number->percentDecimalDigits = nfe->percent_decimal_digits;
        IL2CPP_OBJECT_SETREF(number, percentDecimalSeparator, il2cpp_string_new(idx2string(nfe->percent_decimal_separator)));
        IL2CPP_OBJECT_SETREF(number, percentGroupSeparator, il2cpp_string_new(idx2string(nfe->percent_group_separator)));
        IL2CPP_OBJECT_SETREF(number, percentGroupSizes, culture_info_create_group_sizes_array(nfe->percent_group_sizes, GROUP_SIZE));
        number->percentNegativePattern = nfe->percent_negative_pattern;
        number->percentPositivePattern = nfe->percent_positive_pattern;
        IL2CPP_OBJECT_SETREF(number, percentSymbol, il2cpp_string_new(idx2string(nfe->percent_symbol)));
        IL2CPP_OBJECT_SETREF(number, perMilleSymbol, il2cpp_string_new(idx2string(nfe->per_mille_symbol)));
        IL2CPP_OBJECT_SETREF(number, positiveInfinitySymbol, il2cpp_string_new(idx2string(nfe->positive_infinity_symbol)));
        IL2CPP_OBJECT_SETREF(number, positiveSign, il2cpp_string_new(idx2string(nfe->positive_sign)));
    }

    bool CultureInfo::construct_internal_locale_from_specific_name(Il2CppCultureInfo* cultureInfo, Il2CppString* name)
    {
        std::string cultureName = il2cpp::utils::StringUtils::Utf16ToUtf8(name->chars);
        return construct_culture_from_specific_name(cultureInfo, cultureName.c_str());
    }

#endif

    static bool IsMatchingCultureInfoEntry(const CultureInfoEntry& entry, bool neutral, bool specific, bool installed)
    {
#if !NET_4_0
        const bool isNeutral = ((entry.lcid & 0xff00) == 0) || (entry.specific_lcid == 0);
#else
        const bool isNeutral = entry.territory == 0;
#endif
        return ((neutral && isNeutral) || (specific && !isNeutral));
    }

    Il2CppArray* CultureInfo::internal_get_cultures(bool neutral, bool specific, bool installed)
    {
        // Count culture infos that match.
        int numMatchingCultures = 0;
        for (int i = 0; i < NUM_CULTURE_ENTRIES; ++i)
        {
            const CultureInfoEntry& entry = culture_entries[i];
            if (IsMatchingCultureInfoEntry(entry, neutral, specific, installed))
                ++numMatchingCultures;
        }

        if (neutral)
            ++numMatchingCultures;

        // Allocate result array.
        Il2CppClass* cultureInfoClass = il2cpp_defaults.culture_info;
        Il2CppArray* array = il2cpp_array_new(cultureInfoClass, numMatchingCultures);

        int index = 0;

        // InvariantCulture is not in culture table. We reserve the first
        // array element for it.
        if (neutral)
            il2cpp_array_setref(array, index++, NULL);

        // Populate CultureInfo entries.
        for (int i = 0; i < NUM_CULTURE_ENTRIES; ++i)
        {
            const CultureInfoEntry& entry = culture_entries[i];
            if (!IsMatchingCultureInfoEntry(entry, neutral, specific, installed))
                continue;

            Il2CppCultureInfo* info = reinterpret_cast<Il2CppCultureInfo*>(il2cpp_object_new(cultureInfoClass));
            construct_culture(info, &entry);

            il2cpp_array_setref(array, index++, info);
        }

        return array;
    }

    bool CultureInfo::internal_is_lcid_neutral(int32_t lcid, bool* is_neutral)
    {
        NOT_SUPPORTED_IL2CPP(CultureInfo::internal_is_lcid_neutral, "This icall is not supported by il2cpp.");

        return false;
    }

    Il2CppString* CultureInfo::get_current_locale_name()
    {
        return vm::String::New(::get_current_locale_name().c_str());
    }
} /* namespace Globalization */
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
