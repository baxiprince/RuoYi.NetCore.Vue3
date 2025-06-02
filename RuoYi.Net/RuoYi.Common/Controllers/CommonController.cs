using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RuoYi.Common.Enums;
using RuoYi.Common.Files;
using RuoYi.Common.Utils;
using RuoYi.Data.Dtos;
using RuoYi.Framework;
using RuoYi.Framework.DataValidation;
using Swashbuckle.AspNetCore.Annotations;

namespace RuoYi.Common.Controllers;

/// <summary>
///   通用方法
/// </summary>
[ApiDescriptionSettings("Common")]
[Route("common")]
public class CommonController : ControllerBase
{
  private readonly ILogger<CommonController> _logger;


  public CommonController(ILogger<CommonController> logger)
  {
    _logger = logger;
  }

  /// <summary>
  ///   上传文件
  /// </summary>
  [HttpPost("upload")]
  [SwaggerIgnore]
  public async Task<AjaxResult> UploadFile([FromForm(Name = "file")] IFormFile file)
  {
    if (file != null)
    {
      var filePath = await FileUploadUtils.UploadAsync(file, RyApp.RuoYiConfig.UploadPath, MimeTypeUtils.ZIP_EXE_EXTENSION);
      var ajax = AjaxResult.Success();
      ajax.Add("url", filePath);
      ajax.Add("fileName", filePath);
      ajax.Add("newFileName", Path.GetFileName(filePath));
      ajax.Add("originalFilename", file.FileName);
      return ajax;
    }
    return AjaxResult.Error("上传文件异常，请联系管理员");
  }
}
