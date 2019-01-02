using System;

namespace Pixie.Core
{
    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class AppStateTypeAttribute : Attribute { }
}