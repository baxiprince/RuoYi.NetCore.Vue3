using RuoYi.System.Repositories;

namespace RuoYi.System.Services;

/// <summary>
///   用户角色关联
/// </summary>
public class SysUserRoleService : BaseService<SysUserRole, SysUserRoleDto>, ITransient
{
  private readonly ILogger<SysUserRoleService> _logger;
  private readonly SysRoleRepository _sysRoleRepository;
  private readonly SysUserRoleRepository _userRoleRepository;

  public SysUserRoleService(ILogger<SysUserRoleService> logger,
    SysUserRoleRepository userRoleRepository, SysRoleRepository sysRoleRepository)
  {
    _logger = logger;
    _userRoleRepository = userRoleRepository;
    _sysRoleRepository = sysRoleRepository;
    BaseRepo = userRoleRepository;
  }

  /// <summary>
  ///   查询 用户与角色关联表 详情
  /// </summary>
  public async Task<List<string>> GetRoleKeyListAsync(long? id)
  {
    var roles = await _userRoleRepository.GetDtoListAsync(new SysUserRoleDto { UserId = id ?? 0 });
    List<string> result = [];
    if (roles.Count == 0) return result;
    result.AddRange(roles.Select(item => _sysRoleRepository.GetRoleById(item.RoleId)).Select(role => role.RoleKey));
    return result;
  }
}
