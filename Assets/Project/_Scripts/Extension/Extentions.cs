using System.Collections.Generic;
using UnityEngine;

public static class Extentions
{
    public static T GetRandom<T>(this List<T> list)
    {
        int index = Random.Range(0, list.Count);
        return  list[index];
    }
    
    public static T PullRandom<T>(this List<T> list)
    {
        int index = Random.Range(0, list.Count);
        T result = list[index];
        list.Remove(result);
        return result;
    }
    
    public static void Shuffle<T>(this List<T> list)
    {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    
    public static void ClearAll(this Transform transform, bool instant = false)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if(instant)
                Object.DestroyImmediate(transform.GetChild(i).gameObject);
            else
                Object.Destroy(transform.GetChild(i).gameObject);
        }
    }
}