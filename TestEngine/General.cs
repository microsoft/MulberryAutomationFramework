// <copyright file="General.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>


namespace AutomationFramework
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// class containing general methods being used in the framework.
    /// </summary>
    public static class General
    {
        public static List<Assembly> RefAssemblyLookupList { get; set; }

        public static void AddParameters<K, V>(this Dictionary<K, V> parametersDictionary, K key, V value)
        {

            if (parametersDictionary == null) throw new ArgumentNullException("NULL Value in Dictionary Parameter !");

            if (parametersDictionary.ContainsKey(key))
            {
                parametersDictionary[key] = value;
            }
            else
            {
                parametersDictionary.Add(key, value);
            }
        }


        public static System.Type GetClassType(string className)
        {
            bool foundClassType = false;
            Type returnClassType = null;
            List<string> search4ClassNames = new List<string>();

            search4ClassNames.Add(className);


            try
            {
                Type qualifiedCompType = Type.GetType(className);
                if (qualifiedCompType != null)
                    return qualifiedCompType;
            }
            catch { }


            foreach (Assembly seekAssembly in RefAssemblyLookupList)
            {
                #region Searching in one assembly
                foreach (string localClassName in search4ClassNames)
                {
                    try
                    {
                        returnClassType = GetClassTypeFromAssembly(seekAssembly, localClassName);
                        if (returnClassType == null) continue;

                        foundClassType = true;
                        break;
                    }
                    catch { }
                }
                #endregion
                if (foundClassType) return returnClassType;
            }

            return null;
        }


        public static Type GetClassTypeFromAssembly(Assembly assemblyName, string className)
        {
            //Try/catch is handled in the calling function.  Do not implement.
            Type[] allClassesTypes = assemblyName.GetTypes();

            foreach (Type t in allClassesTypes)
            {
                if (t.Name.ToLower() == className.ToLower() || string.CompareOrdinal(t.FullName.ToLower(), className.ToLower()) == 0)
                    return t;
            }
            throw new Exception("Type Not found !");

        }

    }
}
