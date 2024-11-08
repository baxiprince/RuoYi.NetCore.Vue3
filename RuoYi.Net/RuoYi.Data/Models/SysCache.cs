namespace RuoYi.Data.Models;

public class SysCache
{
  public SysCache()
  {
  }

  public SysCache(string cacheName, string remark)
  {
    CacheName = cacheName;
    Remark = remark;
  }

  public SysCache(string cacheName, string cacheKey, string cacheValue)
  {
    CacheName = cacheName;
    CacheKey = cacheKey;
    CacheValue = cacheValue;
  }

  /**
   * 缓存名称
   */
  public string? CacheName { get; set; } = string.Empty;

  /**
   * 缓存键名
   */
  public string? CacheKey { get; set; } = string.Empty;

  /**
   * 缓存内容
   */
  public string? CacheValue { get; set; } = string.Empty;

  /**
   * 备注
   */
  public string? Remark { get; set; } = string.Empty;
}
