using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class MyExtension {

  public static TSource WhichMin<TSource, TResult>(this IEnumerable<TSource> srcs, System.Func<TSource, TResult> selector)
      where TResult: IComparable
  {
    IEnumerator<TSource> iter = srcs.GetEnumerator();
    iter.MoveNext();

    TSource minItem = iter.Current;
    TResult minVal  = selector(minItem);

    while (iter.MoveNext()) {
      TSource item = iter.Current;
      TResult val  = selector(item);

      if (val.CompareTo(minVal) < 0) {
        minVal  = val;
        minItem = item;
      }
    }

    return minItem;
  }

}
