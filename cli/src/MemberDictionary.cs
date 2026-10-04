using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

public class MemberDictionary<Value> : IDictionary<MemberInfo, Value>
{
  Dictionary<string, Value> inner = new();

  private string Hash(MemberInfo member)
  {
    return MemberSignature.GetMemberSlug(member)!;
  }

  public Value this[MemberInfo key]
  {
    get => inner[Hash(key)!];
    set => inner[Hash(key)!] = value;
  }

  public ICollection<MemberInfo> Keys => throw new NotImplementedException();

  public ICollection<Value> Values => inner.Values;

  public int Count => inner.Count;

  public bool IsReadOnly => false;

  public void Add(MemberInfo key, Value value)
  {
    inner.Add(Hash(key)!, value);
  }

  public void Add(KeyValuePair<MemberInfo, Value> item)
  {
    Add(item.Key, item.Value);
  }

  public void Clear()
  {
    inner.Clear();
  }

  public bool Contains(KeyValuePair<MemberInfo, Value> item)
  {
    throw new NotImplementedException();
  }

  public bool ContainsKey(MemberInfo key)
  {
    return inner.ContainsKey(Hash(key)!);
  }

  public void CopyTo(KeyValuePair<MemberInfo, Value>[] array, int arrayIndex)
  {
    throw new NotImplementedException();
  }

  public IEnumerator<KeyValuePair<MemberInfo, Value>> GetEnumerator()
  {
    throw new NotImplementedException();
  }

  public bool Remove(MemberInfo key)
  {
    return inner.Remove(Hash(key)!);
  }

  public bool Remove(KeyValuePair<MemberInfo, Value> item)
  {
    throw new NotImplementedException();
  }

  public bool TryGetValue(MemberInfo key, [MaybeNullWhen(false)] out Value value)
  {
    return inner.TryGetValue(Hash(key), out value);
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }
}
