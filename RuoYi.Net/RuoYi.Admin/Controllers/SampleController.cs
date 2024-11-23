using Microsoft.AspNetCore.RateLimiting;
using RuoYi.Framework.RateLimit;

namespace RuoYi.Admin;

/// <summary>
///   示例接口
/// </summary>
[ApiDescriptionSettings("Sample")]
public class SampleController : ControllerBase
{
  private readonly ILogger<SampleController> _logger;

  private readonly SystemService _systemService;

  public SampleController(ILogger<SampleController> logger, SystemService systemService)
  {
    _logger = logger;
    _systemService = systemService;
  }

  /// <summary>
  ///   限流
  ///   添加特性 [EnableRateLimiting(LimitType.Default)]
  /// </summary>
  [HttpGet("rateLimit")]
  [EnableRateLimiting(LimitType.Default)]
  public string RateLimit()
  {
    //return Guid.NewGuid().ToString();
    return "rateLimit";
  }

  /// <summary>
  ///   从库(slave) 用户查询
  ///   表 SysUser 的实体类上 添加特性 [Tenant(DataConstants.Slave)]
  /// </summary>
  [HttpGet("getWithPerminAndRole/{id}")]
  [AppAuthorize("system:dept:query")]
  [AppRoleAuthorize("admin")]
  public string GetWithPerminAndRole(long? id)
  {
    return id.ToString();
  }

  /// <summary>
  ///   IP限流
  ///   添加特性 [EnableRateLimiting(LimitType.IP)]
  /// </summary>
  [HttpGet("ipRateLimit")]
  [EnableRateLimiting(LimitType.IP)]
  public string IpRateLimit()
  {
    //return Guid.NewGuid().ToString();
    return "ipRateLimit";
  }
}
