using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace SonarqubeTestLogger;

public static class ExtensionMethod
{
    public static string ToExtractNamespaceFromFQDN(this string fullyQualifiedName, int limit = 2)
    {
        if (string.IsNullOrEmpty(fullyQualifiedName))
            return fullyQualifiedName;

        var t = fullyQualifiedName.Split('.');

        if (t.Length == 1 || t.Length == limit)
            return t[0];

        string[] t1 = new string[t.Length - limit];

        Array.Copy(t, t1, t.Length - limit);

        return string.Join(".", t1);
    }

    public static string ToExtractClassFromFQDN(this string fullyQualifiedName, int limit = 1)
    {
        if (string.IsNullOrEmpty(fullyQualifiedName))
            return fullyQualifiedName;

        var t = fullyQualifiedName.Split('.');

        if (t.Length == 1 || t.Length == limit)
            return "";

        string[] t1 = new string[t.Length - limit];

        Array.Copy(t, t1, t.Length - limit);

        return t1[t1.Length - 1];
    }

    public static string ToExtractMethodFromFQDN(this string fullyQualifiedName, int limit = 0)
    {
        if (string.IsNullOrEmpty(fullyQualifiedName))
            return fullyQualifiedName;

        var t = fullyQualifiedName.Split('.');

        if (t.Length == 1 || t.Length == limit)
            return "";

        string[] t1 = new string[t.Length - limit];

        Array.Copy(t, t1, t.Length - limit);

        return t1[t1.Length - 1];
    }

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Func<T, bool> skip, Action<T, int> action, bool exit = false)
    {
        if (source is not null)
        {
            int i = 1;
            foreach (var item in source)
            {
                if (skip(item))
                {
                    action(item, i);
                    if (exit)
                        break;
                }

                i++;
            }
        }

        return source ?? [];
    }

    public static bool ContainsInPath(this string pathFile, string[] subdirectories)
    {
        foreach (var subdirectory in subdirectories)
        {
            if (pathFile.Contains($"{Path.DirectorySeparatorChar}{subdirectory}{Path.DirectorySeparatorChar}"))
                return true;
        }

        return false;
    }
}
