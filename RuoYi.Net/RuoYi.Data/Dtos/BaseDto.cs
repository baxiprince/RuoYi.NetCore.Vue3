using Microsoft.AspNetCore.Mvc;
using RuoYi.Data.Attributes;

namespace RuoYi.Data.Dtos;

public class BaseDto
{
  /// <summary>
  ///   创建人
  /// </summary>
  [Excel(Name = "创建人")]
  public string? CreateBy { get; set; }

  /// <summary>
  ///   创建时间
  /// </summary>
  [Excel(Name = "创建时间")]
  public DateTime? CreateTime { get; set; }

  /// <summary>
  ///   更新人
  /// </summary>
  [Excel(Name = "更新人")]
  public string? UpdateBy { get; set; }

  /// <summary>
  ///   更新时间
  /// </summary>
  [Excel(Name = "更新时间")]
  public DateTime? UpdateTime { get; set; }

  /// <summary>
  ///   备注
  /// </summary>
  [Excel(Name = "备注")]
  public string? Remark { get; set; }

  /// <summary>
  ///   请求参数
  /// </summary>
  [FromQuery(Name = "")]
  public QueryParam Params { get; set; } = new();
}

public class QueryParam
{
  [FromQuery(Name = "params[beginTime]")]
  public DateTime? BeginTime { get; set; }

  [FromQuery(Name = "params[endTime]")] public DateTime? EndTime { get; set; }

  public string? DataScopeSql { get; set; }
}
