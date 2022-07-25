using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarlMath : MonoBehaviour
{
  
  public static List<int> add(List<int> a, List<int> b)
  {
    List<int> result = new List<int>();
    for (int i = 0; i < Mathf.Min(a.Count, b.Count); i++)
      result.Add(a[i]+b[i]);
    if (a.Count != b.Count)
      for (int i = Mathf.Min(a.Count, b.Count); i < Mathf.Max(a.Count, b.Count); i++)
        result.Add(i < a.Count ? a[i] : b[i]);
    return result;
  }

    public static string ListAsString(List<int> l)
    {
        string result = ":";
        foreach (int i in l) result += i+":";
        return result;
    }
  
    public static List<int> concat(List<int> a, List<int> b)
    {
        List<int> result = new List<int>(a);
        foreach (int e in b)
            result.Add(e);
        return result;
    }

    public static List<List<int>> concat(List<List<int>> a, List<List<int>> b)
    {
        List<List<int>> result = new List<List<int>>(a);
        foreach (List<int> e in b)
            result.Add(e);
        return result;
    }
}
