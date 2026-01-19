using System.ComponentModel.DataAnnotations;

namespace RuoYi.Data.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class RyEmailAddressAttribute : DataTypeAttribute
{
  public RyEmailAddressAttribute() : base(DataType.EmailAddress)
  {
  }

  private static bool EnableFullDomainLiterals { get; } =
    AppContext.TryGetSwitch("System.Net.AllowFullDomainLiterals", out var enable) ? enable : false;

  public override bool IsValid(object? value)
  {
    if (value == null || (value is string && value.ToString() == "")) return true;

    if (!(value is string valueAsString)) return false;

    if (!EnableFullDomainLiterals && (valueAsString.Contains('\r') || valueAsString.Contains('\n'))) return false;

    // only return true if there is only 1 '@' character
    // and it is neither the first nor the last character
    var index = valueAsString.IndexOf('@');

    return
      index > 0 &&
      index != valueAsString.Length - 1 &&
      index == valueAsString.LastIndexOf('@');
  }
}
