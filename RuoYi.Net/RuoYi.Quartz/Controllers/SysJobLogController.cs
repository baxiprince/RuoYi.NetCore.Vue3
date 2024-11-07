
using Microsoft.AspNetCore.Http;
using RuoYi.Common.Enums;
using RuoYi.Common.Utils;
using RuoYi.Quartz.Services;

namespace RuoYi.Quartz.Controllers
{
    /// <summary>
    /// 定时任务调度日志表
    /// </summary>
    [ApiDescriptionSettings("Monitor")]
    [Route("monitor/jobLog")]
    public class SysJobLogController : ControllerBase
    {
        private readonly ILogger<SysJobLogController> _logger;
        private readonly SysJobLogService _sysJobLogService;

        public SysJobLogController(ILogger<SysJobLogController> logger,
            SysJobLogService sysJobLogService)
        {
            _logger = logger;
            _sysJobLogService = sysJobLogService;
        }

        /// <summary>
        /// 查询定时任务调度日志表列表
        /// </summary>
        [HttpGet("list")]
        [AppAuthorize("monitor:log:list")]
        public async Task<SqlSugarPagedList<SysJobLogDto>> GetSysJobLogPagedList([FromQuery] SysJobLogDto dto)
        {
           return await _sysJobLogService.GetDtoPagedListAsync(dto);
        }

        /// <summary>
        /// 获取 定时任务调度日志表 详细信息
        /// </summary>
        [HttpGet("")]
        [HttpGet("{id:long}")]
        [AppAuthorize("monitor:log:query")]
        public async Task<AjaxResult> Get(long id)
        {
            var data = await _sysJobLogService.GetDtoAsync(id);
            return AjaxResult.Success(data);
        }

        /// <summary>
        /// 新增 定时任务调度日志表
        /// </summary>
        [HttpPost("")]
        [AppAuthorize("monitor:log:add")]
        [TypeFilter(typeof(Framework.DataValidation.DataValidationFilter))]
        [System.Log(Title = "定时任务调度日志表", BusinessType = BusinessType.INSERT)]
        public async Task<AjaxResult> Add([FromBody] SysJobLogDto dto)
        {
            var data = await _sysJobLogService.InsertAsync(dto);
            return AjaxResult.Success(data);
        }

        /// <summary>
        /// 修改 定时任务调度日志表
        /// </summary>
        [HttpPut("")]
        [AppAuthorize("monitor:log:edit")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        [RuoYi.System.Log(Title = "定时任务调度日志表", BusinessType = BusinessType.UPDATE)]
        public async Task<AjaxResult> Edit([FromBody] SysJobLogDto dto)
        {
            var data = await _sysJobLogService.UpdateAsync(dto);
            return AjaxResult.Success(data);
        }

        /// <summary>
        /// 删除 定时任务调度日志表
        /// </summary>
        [HttpDelete("{ids}")]
        [AppAuthorize("monitor:log:remove")]
        [RuoYi.System.Log(Title = "定时任务调度日志表", BusinessType = BusinessType.DELETE)]
        public async Task<AjaxResult> Remove(string ids)
        {
            var idList = ids.SplitToList<long>();
            var data = await _sysJobLogService.DeleteAsync(idList);
            return AjaxResult.Success(data);
        }

        /// <summary>
        /// 删除 操作日志记录
        /// </summary>
        [HttpDelete("clean")]
        [AppAuthorize("monitor:log:remove")]
        [RuoYi.System.Log(Title = "操作日志", BusinessType = BusinessType.CLEAN)]
        public AjaxResult Clean()
        {
          _sysJobLogService.Clean();
          return AjaxResult.Success();
        }

        /// <summary>
        /// 导出 定时任务调度日志表
        /// </summary>
        [HttpPost("export")]
        [AppAuthorize("monitor:log:export")]
        [RuoYi.System.Log(Title = "定时任务调度日志表", BusinessType = BusinessType.EXPORT)]
        public async Task Export(SysJobLogDto dto)
        {
            var list = await _sysJobLogService.GetDtoListAsync(dto);
            await ExcelUtils.ExportAsync(App.HttpContext.Response, list);
        }
    }
}
