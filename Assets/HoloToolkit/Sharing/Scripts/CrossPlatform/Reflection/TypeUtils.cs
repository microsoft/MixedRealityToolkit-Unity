using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

public static class TypeUtils
{
    public static Type GetBaseType(this Type type)
    {
#if UNITY_METRO && !UNITY_EDITOR
        return type.GetTypeInfo().BaseType;
#else
        return type.BaseType;
#endif
    }
}
