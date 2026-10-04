using System.Reflection;
using CriusNyx.Util;

public static class ElementURL
{
  public static string? GetElementUrl(MemberInfo member, IDictionary<MemberInfo, string> memberToID)
  {
    if (member is Type type)
    {
      return memberToID.Safe(type);
    }
    else if (member.DeclaringType is Type declaringType)
    {
      if (
        memberToID.TryGetValue(declaringType, out var declaringTypeId)
        && memberToID.TryGetValue(member, out var memberId)
      )
      {
        return $"{declaringTypeId}#{memberId}";
      }
    }
    return null;
  }
}
