using RuoYi.Data;
using RuoYi.Data.Models;
using RuoYi.Framework.Cache;
using RuoYi.Framework.Utils;
using RuoYi.System.Services;

namespace RuoYi.System.Controllers;

/// <summary>
///   缓存监控
/// </summary>
[Route("monitor/cache")]
[ApiDescriptionSettings("Monitor")]
public class CacheController : ControllerBase
{
  private static readonly List<SysCache> Caches =
  [
    new(CacheConstants.LOGIN_TOKEN_KEY, "用户信息"),
    new(CacheConstants.SYS_CONFIG_KEY, "配置信息"),
    new(CacheConstants.SYS_DICT_KEY, "数据字典"),
    new(CacheConstants.CAPTCHA_CODE_KEY, "验证码"),
    //new SysCache(CacheConstants.REPEAT_SUBMIT_KEY, "防重提交"),
    //new SysCache(CacheConstants.RATE_LIMIT_KEY, "限流处理"),
    new(CacheConstants.PWD_ERR_CNT_KEY, "密码错误次数")
  ];

  private readonly ICache _cache;
  private readonly ILogger<SysOperLogController> _logger;
  private readonly ServerService _serverService;

  public CacheController(ILogger<SysOperLogController> logger,
    ICache cache,
    ServerService serverService)
  {
    _logger = logger;
    _cache = cache;
    _serverService = serverService;
  }

  /// <summary>
  ///   缓存监控
  /// </summary>
  [HttpGet("")]
  [AppAuthorize("monitor:cache:list")]
  public async Task<AjaxResult> GetCacheInfo()
  {
    var info = await _cache.GetDbInfoAsync();
    var commandStats = await _cache.GetDbInfoAsync("commandstats");
    var dbSize = await _cache.GetDbSize();

    var result = new Dictionary<string, object>();
    result.Add("info", info);
    result.Add("dbSize", dbSize);

    var pieList = new List<Dictionary<string, string>>();
    foreach (var key in commandStats.Keys)
    {
      var data = new Dictionary<string, string>();

      var property = commandStats[key];
      data.Add("name", StringUtils.StripStart(key, "cmdstat_"));
      data.Add("value", StringUtils.SubstringBetween(property, "calls=", ",usec"));
      pieList.Add(data);
    }

    result.Add("commandStats", pieList);

    return AjaxResult.Success(result);
  }

  /// <summary>
  ///   取缓存信息
  /// </summary>
  [HttpGet("getNames")]
  [AppAuthorize("monitor:cache:list")]
  public AjaxResult GetCacheNames()
  {
    return AjaxResult.Success(Caches);
  }

  /// <summary>
  ///   取缓存 key
  /// </summary>
  [HttpGet("getKeys/{cacheName}")]
  [AppAuthorize("monitor:cache:list")]
  public AjaxResult GetCacheKeys(string cacheName)
  {
    var cacheKeys = _cache.GetDbKeys(cacheName + "*");
    return AjaxResult.Success(cacheKeys);
  }

  /// <summary>
  ///   取缓存 值
  /// </summary>
  [HttpGet("getValue/{cacheName}/{cacheKey}")]
  [AppAuthorize("monitor:cache:list")]
  public AjaxResult GetCacheValue([FromRoute] string cacheName, [FromRoute] string cacheKey)
  {
    var cacheValue = _cache.GetString(cacheKey);
    var sysCache = new SysCache(cacheName, cacheKey, cacheValue);
    return AjaxResult.Success(sysCache);
  }

  /// <summary>
  ///   按名字删除缓存
  /// </summary>
  [HttpDelete("clearCacheName/{cacheName}")]
  [AppAuthorize("monitor:cache:list")]
  public AjaxResult ClearCacheName([FromRoute] string cacheName)
  {
    _cache.RemoveByPattern(cacheName + "*");
    return AjaxResult.Success();
  }

  /// <summary>
  ///   按key删除缓存
  /// </summary>
  [HttpDelete("clearCacheKey/{cacheKey}")]
  [AppAuthorize("monitor:cache:list")]
  public AjaxResult ClearCacheKey([FromRoute] string cacheKey)
  {
    _cache.Remove(cacheKey);
    return AjaxResult.Success();
  }

  /// <summary>
  ///   删除全部缓存
  /// </summary>
  [HttpDelete("clearCacheAll")]
  [AppAuthorize("monitor:cache:list")]
  public AjaxResult ClearCacheAll()
  {
    _cache.RemoveByPattern("*");
    return AjaxResult.Success();
  }
}
