using RuoYi.Common.Enums;
using RuoYi.Common.Files;
using RuoYi.Common.Utils;
using RuoYi.System.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace RuoYi.System.Controllers;

/// <summary>
///   个人信息 业务处理
/// </summary>
[ApiDescriptionSettings("System")]
[Route("system/user/profile")]
public class SysProfileController : ControllerBase
{
  private readonly ILogger<SysNoticeController> _logger;
  private readonly SysUserService _sysUserService;
  private readonly TokenService _tokenService;

  public SysProfileController(ILogger<SysNoticeController> logger,
    SysUserService sysUserService,
    TokenService tokenService)
  {
    _logger = logger;
    _sysUserService = sysUserService;
    _tokenService = tokenService;
  }

  [HttpGet("")]
  public AjaxResult GetProfile()
  {
    var isLogin = SecurityUtils.IsLogin();
    if (!isLogin) return AjaxResult.Error(401, "授权失败");
    var loginUser = SecurityUtils.GetLoginUser();
    var user = loginUser.User;
    var ajax = AjaxResult.Success(user);
    ajax.Add("roleGroup", _sysUserService.SelectUserRoleGroup(loginUser.UserName));
    ajax.Add("postGroup", _sysUserService.SelectUserPostGroup(loginUser.UserName));
    return ajax;
  }

  /// <summary>
  ///   修改用户
  /// </summary>
  [HttpPut("")]
  [Log(Title = "个人信息", BusinessType = BusinessType.UPDATE)]
  public async Task<AjaxResult> UpdateProfile([FromBody] SysUserDto user)
  {
    var isLogin = SecurityUtils.IsLogin();
    if (!isLogin) return AjaxResult.Error(401, "授权失败");
    var loginUser = SecurityUtils.GetLoginUser();
    var currentUser = loginUser.User;
    currentUser.NickName = user.NickName;
    currentUser.Email = user.Email;
    currentUser.Phonenumber = user.Phonenumber;
    currentUser.Sex = user.Sex;

    if (StringUtils.IsNotEmpty(user.Phonenumber) && !await _sysUserService.CheckPhoneUniqueAsync(currentUser))
      return AjaxResult.Error("修改用户'" + user.UserName + "'失败，手机号码已存在");
    if (StringUtils.IsNotEmpty(user.Email) && !await _sysUserService.CheckEmailUniqueAsync(currentUser))
      return AjaxResult.Error("修改用户'" + user.UserName + "'失败，邮箱账号已存在");
    if (await _sysUserService.UpdateUserProfileAsync(currentUser) > 0)
    {
      // 更新缓存用户信息
      _tokenService.SetLoginUser(loginUser);
      return AjaxResult.Success();
    }

    return AjaxResult.Error("修改个人信息异常，请联系管理员");
  }

  /// <summary>
  ///   重置密码
  /// </summary>
  [HttpPut("updatePwd")]
  [Log(Title = "重置密码", BusinessType = BusinessType.UPDATE)]
  public async Task<AjaxResult> UpdatePwd(string oldPassword, string newPassword)
  {
    var isLogin = SecurityUtils.IsLogin();
    if (!isLogin) return AjaxResult.Error(401, "授权失败");
    var loginUser = SecurityUtils.GetLoginUser();
    var userName = loginUser.UserName;
    var password = loginUser.Password;
    if (!SecurityUtils.MatchesPassword(oldPassword, password)) return AjaxResult.Error("修改密码失败，旧密码错误");
    if (SecurityUtils.MatchesPassword(newPassword, password)) return AjaxResult.Error("新密码不能与旧密码相同");
    if (await _sysUserService.ResetUserPwdAsync(userName, SecurityUtils.EncryptPassword(newPassword)) > 0)
    {
      // 更新缓存用户密码
      loginUser.User.Password = SecurityUtils.EncryptPassword(newPassword);
      _tokenService.SetLoginUser(loginUser);
      return AjaxResult.Success();
    }

    return AjaxResult.Error("修改密码异常，请联系管理员");
  }

  /// <summary>
  ///   头像上传
  /// </summary>
  [SwaggerIgnore] // Swagger 不支持 [FromForm] IFormFile
  [HttpPost("avatar")]
  [Log(Title = "用户头像", BusinessType = BusinessType.UPDATE)]
  public async Task<object> UploadAvatar([FromForm(Name = "avatarfile")] IFormFile file)
  {
    var isLogin = SecurityUtils.IsLogin();
    if (!isLogin) return AjaxResult.Error(401, "授权失败");
    if (file != null)
    {
      var loginUser = SecurityUtils.GetLoginUser();
      var avatar = await FileUploadUtils.UploadAsync(file, RyApp.RuoYiConfig.AvatarPath, MimeTypeUtils.IMAGE_EXTENSION);
      if (await _sysUserService.UpdateUserAvatar(loginUser.UserName, avatar))
      {
        // 更新缓存用户头像
        loginUser.User.Avatar = avatar;
        _tokenService.SetLoginUser(loginUser);

        var ajax = AjaxResult.Success();
        ajax.Add("imgUrl", avatar);
        return ajax;
      }
    }

    return AjaxResult.Error("上传图片异常，请联系管理员");
  }
}
