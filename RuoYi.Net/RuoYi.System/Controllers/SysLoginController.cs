using RuoYi.Common.Utils;
using RuoYi.Data.Models;
using RuoYi.System.Services;

namespace RuoYi.Admin;

/// <summary>
///   登录验证
/// </summary>
[ApiDescriptionSettings("System")]
public class SysLoginController : ControllerBase
{
  private readonly ILogger<SysLoginController> _logger;
  private readonly SysLogininforService _sysLogininforService;

  private readonly SysLoginService _sysLoginService;
  private readonly SysMenuService _sysMenuService;
  private readonly SysPermissionService _sysPermissionService;
  private readonly TokenService _tokenService;

  public SysLoginController(ILogger<SysLoginController> logger,
    TokenService tokenService,
    SysLoginService sysLoginService,
    SysPermissionService sysPermissionService,
    SysMenuService sysMenuService,
    SysLogininforService sysLogininforService)
  {
    _logger = logger;
    _tokenService = tokenService;
    _sysLoginService = sysLoginService;
    _sysPermissionService = sysPermissionService;
    _sysMenuService = sysMenuService;
    _sysLogininforService = sysLogininforService;
  }

  /// <summary>
  ///   登录验证
  /// </summary>
  /// <returns></returns>
  [HttpPost("/login")]
  public async Task<AjaxResult> Login([FromBody] LoginBody loginBody)
  {
    var ajax = AjaxResult.Success();
    // 生成令牌
    var token = await _sysLoginService.LoginAsync(loginBody.Username, loginBody.Password, loginBody.Code, loginBody.Uuid);
    ajax.Add(Constants.TOKEN, token);
    return ajax;
  }

  /// <summary>
  ///   退出
  /// </summary>
  [HttpPost("/logout")]
  public AjaxResult Logout()
  {
    var loginUser = _tokenService.GetLoginUser(App.HttpContext.Request);
    if (loginUser != null)
    {
      var userName = loginUser.UserName;
      // 删除用户缓存记录
      _tokenService.DelLoginUser(loginUser.Token);
      // 记录用户退出日志
      _ = Task.Factory.StartNew(async () => { await _sysLogininforService.AddAsync(userName, Constants.LOGOUT, "退出成功"); });
    }

    return AjaxResult.Success("退出成功");
  }

  /// <summary>
  ///   获取用户信息
  /// </summary>
  [HttpGet("/getInfo")]
  [AppAuthorize("*:*:*:*")]
  public async Task<AjaxResult> GetInfo()
  {
    var user = SecurityUtils.GetLoginUser().User;
    // 角色集合
    var roles = await _sysPermissionService.GetRolePermissionAsync(user);
    // 权限集合
    var permissions = _sysPermissionService.GetMenuPermission(user);

    var ajax = AjaxResult.Success();
    ajax.Add("user", user);
    ajax.Add("roles", roles);
    ajax.Add("permissions", permissions);
    return ajax;
  }

  /// <summary>
  ///   获取路由信息
  /// </summary>
  [HttpGet("/getRouters")]
  [AppAuthorize("*:*:*:*")]
  public AjaxResult GetRouters()
  {
    var userId = SecurityUtils.GetUserId();
    var menus = _sysMenuService.SelectMenuTreeByUserId(userId);
    var treeMenus = _sysMenuService.BuildMenus(menus);
    return AjaxResult.Success(treeMenus);
  }
}
