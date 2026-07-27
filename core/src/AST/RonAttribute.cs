namespace Ron;

public abstract class AttributeContent;

public class NoneAttributeContent : AttributeContent;

public class ValueAttributeContent : AttributeContent
{
  public string? Value;
}

public class ArgsAttributeContent : AttributeContent
{
  public IEnumerable<string>? Args;
}

public class RonAttribute
{
  public RonSpan? span;
  public Trivia? leading;
  public string? name;
  public AttributeContent? content;
}
