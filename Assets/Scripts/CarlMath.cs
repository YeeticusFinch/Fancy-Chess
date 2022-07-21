using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
  
  public static List<int> add(List<int> a, List<int> b)
  {
    List<int> result = new List<int>();
    for (int i = 0; i < Math.Min(a.size(), b.size()); i++)
      result.add(a[i]+b[i]);
    if (a.size() != b.size())
      for (int i = Math.Min(a.size(), b.size()); i < Math.Max(a.size(), b.size()); i++)
        result.add(i < a.size() ? a[i] : b[i]);
    return result;
  }
  
}
