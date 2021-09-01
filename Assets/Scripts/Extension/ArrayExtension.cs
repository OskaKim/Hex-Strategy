using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtension
{
    public static void ForEach<T>(this T[] array, Action<T> action)
    {
        if (action == null) { throw new ArgumentNullException(nameof(action)); }

        for (int i = 0; i < array.Length; i++)
        {
            action?.Invoke(array[i]);
        }
    }

    public static void ForEach<T>(this T[] array, RefAction<T> action)
    {
        if (action == null) { throw new ArgumentNullException(nameof(action)); }

        for (int i = 0; i < array.Length; i++)
        {
            action?.Invoke(ref array[i]);
        }
    }

    public static void ForEach<T>(this T[] array, Func<T, T> action, int? a)
    {
        if (action == null) { throw new ArgumentNullException(nameof(action)); }

        for (int i = 0; i < array.Length; i++)
        {
            array[i] = action.Invoke(array[i]);
        }
    }
}

public delegate void RefAction<T>(ref T item);