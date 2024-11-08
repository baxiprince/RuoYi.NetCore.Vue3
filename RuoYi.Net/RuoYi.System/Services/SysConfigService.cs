using RuoYi.Common.Constants;
using RuoYi.Framework.Cache;
using RuoYi.Framework.Exceptions;
using RuoYi.System.Repositories;

namespace RuoYi.System.Services;

/// <summary>
///   参数配置表 Service
///   author ruoyi
///   date   2023-08-21 14:40:22
/// </summary>
public class SysConfigService : BaseService<SysConfig, SysConfigDto>, ITransient
{
  private readonly ICache _cache;
  private readonly ILogger<SysConfigService> _logger;
  private readonly SysConfigRepository _sysConfigRepository;

  public SysConfigService(ILogger<SysConfigService> logger,
    ICache cache,
    SysConfigRepository sysConfigRepository)
  {
    _logger = logger;
    _cache = cache;
    _sysConfigRepository = sysConfigRepository;
    BaseRepo = sysConfigRepository;
  }

  /// <summary>
  ///   查询 参数配置表 详情
  /// </summary>
  public async Task<SysConfig> GetAsync(int id)
  {
    var entity = await base.FirstOrDefaultAsync(e => e.ConfigId == id);
    return entity;
  }

  /// <summary>
  ///   根据键名查询参数配置信息
  /// </summary>
  /// <param name="configKey">参数key</param>
  /// <returns>参数键值</returns>
  public string SelectConfigByKey(string configKey)
  {
    var configValue = _cache.GetString(GetCacheKey(configKey));
    if (!string.IsNullOrEmpty(configValue)) return configValue;
    var config = _sysConfigRepository.FirstOrDefault(e => e.ConfigKey == configKey);
    if (config != null)
    {
      _cache.SetString(GetCacheKey(configKey), config.ConfigValue ?? "");
      return config.ConfigValue ?? "";
    }

    return string.Empty;
  }

  /// <summary>
  ///   设置cache key
  /// </summary>
  /// <param name="configKey">configKey 参数键</param>
  private string GetCacheKey(string configKey)
  {
    return CacheConstants.SYS_CONFIG_KEY + configKey;
  }

  /// <summary>
  ///   获取验证码开关
  /// </summary>
  public bool IsCaptchaEnabled()
  {
    var captchaEnabled = SelectConfigByKey("sys.account.captchaEnabled");
    if (string.IsNullOrEmpty(captchaEnabled)) return true;
    return Convert.ToBoolean(captchaEnabled);
  }

  /// <summary>
  ///   新增参数配置
  /// </summary>
  public async Task<bool> InsertConfigAsync(SysConfigDto config)
  {
    var success = await _sysConfigRepository.InsertAsync(config);
    if (success) await _cache.SetStringAsync(GetCacheKey(config.ConfigKey!), config.ConfigValue!);
    return success;
  }

  /// <summary>
  ///   修改参数配置
  /// </summary>
  public async Task<int> UpdateConfigAsync(SysConfigDto config)
  {
    var temp = await GetAsync(config.ConfigId ?? 0);
    if (!temp.ConfigKey!.Equals(config.ConfigKey)) _cache.Remove(GetCacheKey(temp.ConfigKey));

    var row = await _sysConfigRepository.UpdateAsync(config);
    if (row > 0) await _cache.SetStringAsync(GetCacheKey(config.ConfigKey!), config.ConfigValue!);
    return row;
  }

  /// <summary>
  ///   批量删除参数信息
  /// </summary>
  /// <param name="configIds">需要删除的参数ID</param>
  public async Task DeleteConfigByIdsAsync(int[] configIds)
  {
    foreach (var configId in configIds)
    {
      var config = await GetAsync(configId);
      if (StringUtils.Equals(UserConstants.YES, config.ConfigType)) throw new ServiceException($"内置参数【{config.ConfigKey}】不能删除 ");
      await _sysConfigRepository.DeleteAsync(configId);
      await _cache.RemoveAsync(GetCacheKey(config.ConfigKey!));
    }
  }

  /// <summary>
  ///   重置参数缓存数据
  /// </summary>
  public void ResetConfigCache()
  {
    ClearConfigCache();
    LoadingConfigCache();
  }

  /// <summary>
  ///   清空参数缓存数据
  /// </summary>
  public void ClearConfigCache()
  {
    _cache.RemoveByPattern(CacheConstants.SYS_CONFIG_KEY + "*");
  }

  /// <summary>
  ///   加载参数缓存数据
  /// </summary>
  public void LoadingConfigCache()
  {
    var configsList = _sysConfigRepository.GetList(new SysConfigDto());
    foreach (var config in configsList) _cache.SetString(GetCacheKey(config.ConfigKey!), config.ConfigValue!);
  }

  /// <summary>
  ///   校验参数键名是否唯一
  /// </summary>
  /// <param name="config">参数配置信息</param>
  public bool CheckConfigKeyUnique(SysConfigDto config)
  {
    var configId = config.ConfigId ?? 0;
    var info = _sysConfigRepository.CheckConfigKeyUnique(config.ConfigKey!);
    if (info != null && info.ConfigId != configId) return UserConstants.NOT_UNIQUE;
    return UserConstants.UNIQUE;
  }
}
