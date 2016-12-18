using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Utils {
  public static class CollectionUtils {
    public static Dictionary<TKey, TValue> DictionaryFromLists<TKey, TValue>(IList<TKey> keys, IList<TValue> values) {
      if ((keys == null) || (values == null)) {
        return new Dictionary<TKey, TValue>();
      }
      // Both lists SHOULD be the same size. Clamp to the bounds of the smallest list.
      Debug.AssertFormat(keys.Count == values.Count, "Key count ({0}) should be the same as value count ({1}).", keys.Count, values.Count);
      int bounds = Math.Min(keys.Count, values.Count);

      Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();
      for (int i = 0; i < bounds; i++) {
        dict[keys[i]] = values[i];
      }
      return dict;
    }

    public static T RandomItemFromCollection<T>(IList<T> list) {
      if ((list == null) || (list.Count == 0)) {
        return default(T);
      }

      int index = Random.Range(0, list.Count);
      return list[index];
    }

    public static List<TValue> ProduceListForKey<TKey, TValue>(Dictionary<TKey, List<TValue>> dictionary, TKey key) {
      if (dictionary == null) {
        return new List<TValue>();
      }
      List<TValue> list;
      dictionary.TryGetValue(key, out list);
      if (list == null) {
        list = new List<TValue>();
        dictionary.Add(key, list);
      }
      return list;
    }

    public static IEnumerator<T> LoopingEnumerator<T>(ICollection<T> collection) {
      while (true) {
        if (collection.Count == 0) {
          yield break;
        }
        foreach (T item in collection) {
          yield return item;
        }
      }
    }

    public static IEnumerator<T> BagRandomizer<T>(ICollection<T> collection) {
      if ((collection == null) || (collection.Count == 0)) {
        yield break;
      }
      while (true) {
        List<T> bag = new List<T>(collection);
        Shuffle(bag);
        foreach (T item in bag) {
          yield return item;
        }
      }
    }

    public static IEnumerator<T> Randomizer<T>(IList<T> list) {
      if ((list == null) || (list.Count == 0)) {
        yield break;
      }
      while (true) {
        yield return RandomItemFromCollection(list);
      }
    }

    public static void Shuffle<T>(IList<T> list) {
      if (list == null) {
        return;
      }
      // Fisher-Yates/Durstenfeld in-place shuffle
      // https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle#The_modern_algorithm
      for (int i = list.Count - 1; i >= 1; i--) {
        // Note that Range() is max *exclusive*, hence the i+1
        int j = Random.Range(0, i + 1);
        T temp = list[j];
        list[j] = list[i];
        list[i] = temp;
      }
    }

    public static void ShuffleInto<T>(IEnumerable<T> source, IList<T> destination) {
      Debug.Assert(destination != null, "Destination cannot be null");
      if (source == null) {
        return;
      }

      foreach (T item in source) {
        ShuffleInto(item, destination);
      }
    }

    public static void ShuffleInto<T>(T item, IList<T> destination) {
      Debug.Assert(destination != null, "Destination cannot be null");
      if (item == null) {
        return;
      }

      // Fisher-Yates 'inside-out' shuffle
      // https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle#The_.22inside-out.22_algorithm
      int j = Random.Range(0, destination.Count + 1);
      if (j == destination.Count) {
        destination.Add(item);
      } else {
        destination.Add(destination[j]);
        destination[j] = item;
      }
    }

    public static IList<T> ShuffleToNewList<T>(IEnumerable<T> source) {
      IList<T> destination = new List<T>();
      ShuffleInto(source, destination);
      return destination;
    }

    public static void PruneDestroyed<T>(ICollection<T> collection) where T : Object {
      if (collection == null) {
        return;
      }
      List<T> toPrune = null;
      foreach (T item in collection) {
        if (item == null) {
          if (toPrune == null) {
            toPrune = new List<T>();
          }
          toPrune.Add(item);
        }
      }
      if (toPrune != null) {
        foreach (T item in toPrune) {
          collection.Remove(item);
        }
      }
    }

    public static void PruneDestroyed<TKey, TValue>(IDictionary<TKey, TValue> dictionary) where TKey : Object {
      if (dictionary == null) {
        return;
      }
      List<TKey> toPrune = null;
      foreach (KeyValuePair<TKey, TValue> kvp in dictionary) {
        TKey item = kvp.Key;
        if (item == null) {
          if (toPrune == null) {
            toPrune = new List<TKey>();
          }
          toPrune.Add(item);
        }
      }
      if (toPrune != null) {
        foreach (TKey item in toPrune) {
          dictionary.Remove(item);
        }
      }
    }

    public static T FindHighestScore<T>(IEnumerable<T> collection, Func<T, float> scoreFunc) where T : class {
      T best = null;
      float bestScore = float.MinValue;
      foreach (T candidate in collection) {
        float score = scoreFunc(candidate);
        if ((best == null) || (score > bestScore)) {
          best = candidate;
          bestScore = score;
        }
      }
      return best;
    }
  }
}

// Conforms to the non-Extension portion of IReadOnlyCollection from .NET framework 4.5
// https://msdn.microsoft.com/en-us/library/hh881542(v=vs.110).aspx
public interface IReadOnlyCollection<T> : IEnumerable<T>, IEnumerable {
  int Count { get; }
}

public interface IReadOnlySet<T> : IReadOnlyCollection<T> {
  bool Contains(T item);

  void CopyTo(T[] array);

  void CopyTo(T[] array, int arrayIndex);

  void CopyTo(T[] array, int arrayIndex, int count);

  bool IsProperSubsetOf(IEnumerable<T> other);

  bool IsProperSupersetOf(IEnumerable<T> other);

  bool IsSubsetOf(IEnumerable<T> other);

  bool IsSupersetOf(IEnumerable<T> other);

  bool Overlaps(IEnumerable<T> other);

  bool SetEquals(IEnumerable<T> other);
}

public class ReadOnlySetWrapper<T> : IReadOnlySet<T> {
  private HashSet<T> m_BackingSet;

  private ReadOnlySetWrapper() {}

  public ReadOnlySetWrapper(HashSet<T> backingSet) {
    m_BackingSet = backingSet;
  }

  public bool Contains(T item) {
    return m_BackingSet.Contains(item);
  }

  public void CopyTo(T[] array) {
    m_BackingSet.CopyTo(array);
  }

  public void CopyTo(T[] array, int arrayIndex) {
    m_BackingSet.CopyTo(array, arrayIndex);
  }

  public void CopyTo(T[] array, int arrayIndex, int count) {
    m_BackingSet.CopyTo(array, arrayIndex, count);
  }

  public int Count {
    get {
      return m_BackingSet.Count;
    }
  }

  public IEnumerator<T> GetEnumerator() {
    return m_BackingSet.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() {
    return m_BackingSet.GetEnumerator();
  }

  public bool IsProperSubsetOf(IEnumerable<T> other) {
    return m_BackingSet.IsProperSubsetOf(other);
  }

  public bool IsProperSupersetOf(IEnumerable<T> other) {
    return m_BackingSet.IsProperSupersetOf(other);
  }

  public bool IsSubsetOf(IEnumerable<T> other) {
    return m_BackingSet.IsSubsetOf(other);
  }

  public bool IsSupersetOf(IEnumerable<T> other) {
    return m_BackingSet.IsSupersetOf(other);
  }

  public bool Overlaps(IEnumerable<T> other) {
    return m_BackingSet.Overlaps(other);
  }

  public bool SetEquals(IEnumerable<T> other) {
    return m_BackingSet.SetEquals(other);
  }
}

public class TwoWayDictionary<T> : IDictionary<T, T> {
  private Dictionary<T, T> forward = new Dictionary<T, T>();
  private Dictionary<T, T> reverse = new Dictionary<T, T>();

  public void Add(T key, T value) {
    Remove(key);
    Remove(value);
    forward.Add(key, value);
    reverse.Add(value, key);
  }

  public bool ContainsKey(T key) {
    return forward.ContainsKey(key) || reverse.ContainsKey(key);
  }

  public bool Remove(T key) {
    if (forward.ContainsKey(key)) {
      T value = forward[key];
      reverse.Remove(value);
      forward.Remove(key);
      return true;
    } else if (reverse.ContainsKey(key)) {
      T value = reverse[key];
      forward.Remove(value);
      reverse.Remove(key);
      return true;
    } else {
      return false;
    }
  }

  public bool TryGetValue(T key, out T value) {
    if (forward.TryGetValue(key, out value)) {
      return true;
    } else if (reverse.TryGetValue(key, out value)) {
      return true;
    } else {
      value = default(T);
      return false;
    }
  }

  public T this[T key] {
    get {
      T value;
      if (forward.TryGetValue(key, out value)) {
        return value;
      } else if (reverse.TryGetValue(key, out value)) {
        return value;
      } else {
        throw new KeyNotFoundException();
      }
    }
    set {
      Add(key, value);
    }
  }

  public ICollection<T> Keys {
    get {
      HashSet<T> keys = new HashSet<T>(forward.Keys);
      keys.UnionWith(reverse.Keys);
      return keys;
    }
  }

  public ICollection<T> Values {
    get {
      HashSet<T> values = new HashSet<T>(forward.Values);
      values.UnionWith(reverse.Values);
      return values;
    }
  }

  IEnumerator IEnumerable.GetEnumerator() {
    return GetEnumerator();
  }

  public IEnumerator<KeyValuePair<T, T>> GetEnumerator() {
    foreach (KeyValuePair<T, T> pair in forward) {
      yield return pair;
    }
    foreach (KeyValuePair<T, T> pair in reverse) {
      yield return pair;
    }
  }

  public void Add(KeyValuePair<T, T> item) {
    Add(item.Key, item.Value);
  }

  public void Clear() {
    forward.Clear();
    reverse.Clear();
  }

  public bool Contains(KeyValuePair<T, T> item) {
    T testValue;
    if ((TryGetValue(item.Key, out testValue) == true) && item.Value.Equals(testValue)) {
      return true;
    } else {
      return false;
    }
  }

  public void CopyTo(KeyValuePair<T, T>[] array, int arrayIndex) {
    throw new NotImplementedException();
  }

  public bool Remove(KeyValuePair<T, T> item) {
    T testValue;
    if ((TryGetValue(item.Key, out testValue) == true) && item.Value.Equals(testValue)) {
      return Remove(item.Key);
    } else {
      return false;
    }
  }

  public int Count {
    get {
      return forward.Count;
    }
  }

  public bool IsReadOnly {
    get {
      return false;
    }
  }
}