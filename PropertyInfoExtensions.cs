﻿using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace JOS.InjectedAllowedTypes
{
    internal static class PropertyInfoExtensions
    {
        public static bool IsAutoGenerated(this PropertyInfo p)
        {
            if (p.GetGetMethod() != null && p.GetSetMethod() != null && p.GetGetMethod().GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length == 1)
                return p.GetSetMethod().GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length == 1;
            return false;
        }

        public static bool IsAutoVirtualPublic(this PropertyInfo self)
        {
            if (self == null)
                throw new ArgumentNullException("self");
            if (self.IsAutoGenerated())
                return (self.GetAccessors(true)).All(m =>
                {
                    if (m.IsVirtual)
                        return m.IsPublic;
                    return false;
                });
            return false;
        }
    }
}