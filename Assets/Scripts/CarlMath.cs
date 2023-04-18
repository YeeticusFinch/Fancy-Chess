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

    public static List<int> halfway(List<int> a, List<int> b)
    {
        List<int> result = new List<int>();
        for (int i = 0; i < Mathf.Min(a.Count, b.Count); i++)
        {
            result.Add(a[i] + (b[i] - a[i]) / 2);
        }
        return result;
    }

    public static float angleDiff(float a, float b)
    {
        return AbsMin(AbsMin((a % 360) - (b % 360), ((a % 360) + 360) - (b % 360)), (a % 360) - ((b % 360) + 360));
    }

    public static float AbsMod(float a, float b)
    {
        return Mathf.Sign(a) * Mathf.Sign(b) * (a % b);
    }

    public static string ListAsString(List<int> l)
    {
        if (l == null) return null;
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

    public static bool equals(List<int> a, List<int> b)
    {
        for (int i = 0; i < Mathf.Min(a.Count, b.Count); i++)
            if (a[i] != b[i])
                return false;
        return true;
    }

    public static float AbsMin(float a, float b)
    {
        if (Mathf.Abs(a) < Mathf.Abs(b))
            return a;
        return b;
    }

    public static Vector3 MaxV(Vector3 a, Vector3 b)
    {
        if (a.magnitude > b.magnitude)
            return a;
        else return b;
        //return new Vector3(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
    }

    public static Vector3 MinV(Vector3 a, Vector3 b)
    {
        if (a.magnitude > b.magnitude)
            return b;
        else return a;
        //return new Vector3(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));
    }
}
