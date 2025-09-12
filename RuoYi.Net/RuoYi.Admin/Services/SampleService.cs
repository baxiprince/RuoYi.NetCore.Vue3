using SqlSugar;

namespace RuoYi.Admin.Services;

public class SampleService : ITransient
{
  private readonly ISqlSugarClient _db;

  public SampleService(ISqlSugarClient db)
  {
    _db = db;
  }

  public async Task<int> UpdateUserAsync()
  {
    var sql = "UPDATE sys_user SET remark = 'test' WHERE user_name = 'ry.net'";
    return await _db.Ado.ExecuteCommandAsync(sql);
  }
}
