namespace RonCS;

public enum RonPrintMode
{
  Compact,
  Pretty,
}

public class RonPrintOptions
{
  public string indent = "  ";
  public RonPrintMode mode;

  public static RonPrintOptions Pretty()
  {
    return new RonPrintOptions { mode = RonPrintMode.Pretty };
  }

  public static RonPrintOptions Compact()
  {
    return new RonPrintOptions { mode = RonPrintMode.Compact };
  }
}
