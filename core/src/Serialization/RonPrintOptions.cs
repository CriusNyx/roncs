namespace RonCS;

/// <summary>
/// The print mode to use for Ron printing.
/// </summary>
public enum RonPrintMode
{
  /// <summary>
  /// The most compact ron string possible.
  /// </summary>
  Compact,

  /// <summary>
  /// Format ron with new lines and indent.
  /// </summary>
  Pretty,
}

/// <summary>
/// Ron print options.
/// </summary>
public class RonPrintOptions
{
  /// <summary>
  /// The indent to use for the Ron document.
  /// </summary>
  public string indent = "  ";

  /// <summary>
  /// The mode to use for Ron printing.
  /// </summary>
  public RonPrintMode mode;

  /// <summary>
  /// Create the default pretty print options.
  /// </summary>
  /// <returns></returns>
  public static RonPrintOptions Pretty()
  {
    return new RonPrintOptions { mode = RonPrintMode.Pretty };
  }

  /// <summary>
  /// Create the default compact print options.
  /// </summary>
  /// <returns></returns>
  public static RonPrintOptions Compact()
  {
    return new RonPrintOptions { mode = RonPrintMode.Compact };
  }
}
