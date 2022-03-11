using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtension
{
    public static void Replace<T>(this List<T> current, int startIndex, T[] collection)
    {
        for (int i = 0; i < collection.Length; i++)
            current[startIndex + i] = collection[i];
    }
}
