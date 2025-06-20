namespace RuoYi.System.Repositories;

/// <summary>
///   部门表 Repository
///   author ruoyi
///   date   2023-09-04 17:49:57
/// </summary>
public class SysDeptRepository : BaseRepository<SysDept, SysDeptDto>
{
  public SysDeptRepository(ISqlSugarRepository<SysDept> sqlSugarRepository)
  {
    Repo = sqlSugarRepository;
  }

  public override ISugarQueryable<SysDept> Queryable(SysDeptDto dto)
  {
    return Repo.AsQueryable()
        .Where(d => d.DelFlag == DelFlag.No)
        .WhereIF(dto.DeptId > 0, d => d.DeptId == dto.DeptId)
        .WhereIF(dto.ParentId > 0, d => d.ParentId == dto.ParentId)
        .WhereIF(!string.IsNullOrEmpty(dto.DelFlag), d => d.DelFlag == dto.DelFlag)
        .WhereIF(!string.IsNullOrEmpty(dto.DeptName), d => d.DeptName!.Contains(dto.DeptName!))
        .WhereIF(!string.IsNullOrEmpty(dto.Status), d => d.Status == dto.Status)
      ;
  }

  public override ISugarQueryable<SysDeptDto> DtoQueryable(SysDeptDto dto)
  {
    return Repo.AsQueryable()
      .LeftJoin<SysRoleDept>((d, rd) => d.DeptId == rd.DeptId)
      .Where(d => d.DelFlag == DelFlag.No)
      .WhereIF(dto.DeptId > 0, d => d.DeptId == dto.DeptId)
      .WhereIF(dto.ParentId > 0, d => d.ParentId == dto.ParentId)
      .WhereIF(dto.ParentIds!.IsNotEmpty(), d => dto.ParentIds!.Contains(d.ParentId))
      .WhereIF(!string.IsNullOrEmpty(dto.DelFlag), d => d.DelFlag == dto.DelFlag)
      .WhereIF(!string.IsNullOrEmpty(dto.Status), d => d.Status == dto.Status)
      .WhereIF(!string.IsNullOrEmpty(dto.DeptName), d => d.DeptName!.Contains(dto.DeptName!))
      // and d.dept_id not in (select d.parent_id from sys_dept d inner join sys_role_dept rd on d.dept_id = rd.dept_id and rd.role_id = #{roleId})
      .WhereIF(dto.DeptCheckStrictly ?? false, d => d.DeptId !=
                                                    SqlFunc.Subqueryable<SysDept>()
                                                      .InnerJoin<SysRoleDept>((d1, rd1) => d1.DeptId == rd1.DeptId)
                                                      .Where((d1, rd1) => rd1.RoleId == dto.RoleId)
                                                      .GroupBy(d1 => d1.ParentId)
                                                      .Select(d1 => d1.ParentId))
      .Select(d => new SysDeptDto
        {
          DeptId = d.DeptId
        },
        true);
  }

  // dtos 关联表数据
  protected override async Task FillRelatedDataAsync(IEnumerable<SysDeptDto> dtos)
  {
    if (dtos.IsEmpty()) return;

    // 关联表处理
    var parentIds = dtos.Where(d => d.ParentId.HasValue).Select(d => d.ParentId!.Value).Distinct().ToList();
    var parentDepts = await DtoQueryable(new SysDeptDto { ParentIds = parentIds }).ToListAsync();
    foreach (var dto in dtos) dto.ParentName = parentDepts.FirstOrDefault(p => p.DeptId == dto.ParentId)?.DeptName;
  }

  public async Task<List<SysDept>> GetDeptListAsync(SysDeptDto dto)
  {
    dto.DelFlag = DelFlag.No;

    return await GetListAsync(dto);
  }

  /// <summary>
  ///   根据角色ID查询部门树信息
  /// </summary>
  public async Task<List<long>> GetDeptListByRoleIdAsync(long roleId, bool isDeptCheckStrictly)
  {
    //var query = new SysDeptDto { RoleId = roleId, DeptCheckStrictly = isDeptCheckStrictly };
    //var list = await GetDtoListAsync(query);
    //return list.Where(d => d.DeptId.HasValue).Select(d => d.DeptId!.Value).Distinct().ToList();

    var query = Repo.Context.Queryable<SysDept>()
      .LeftJoin<SysRoleDept>((d, rd) => d.DeptId == rd.DeptId)
      .Where((d, rd) => rd.RoleId == roleId);

    if (isDeptCheckStrictly)
    {
      // 使用原生 SQL 条件
      query = query.Where($"""
                           d.dept_id NOT IN (
                               SELECT d2.parent_id
                               FROM sys_dept d2
                               INNER JOIN sys_role_dept rd2 ON d2.dept_id = rd2.dept_id
                               WHERE rd2.role_id = {roleId}
                           )
                           """);
    }

    return await query
      .OrderBy(d => d.ParentId)
      .OrderBy(d => d.OrderNum)
      .Select(d => d.DeptId)
      .ToListAsync();

  }

  /// <summary>
  ///   根据ID查询所有子部门（正常状态）
  /// </summary>
  /// <param name="deptId">部门ID</param>
  public async Task<int> CountNormalChildrenDeptByIdAsync(long deptId)
  {
    return await CountAsync(d => d.DelFlag == DelFlag.No && d.Status == "0" && SqlFunc.SplitIn(d.Ancestors, deptId.ToString()));
  }

  /// <summary>
  ///   根据ID查询所有子部门
  /// </summary>
  /// <param name="deptId">部门ID</param>
  /// <returns></returns>
  public async Task<List<SysDept>> GetChildrenDeptByIdAsync(long deptId)
  {
    var queryable = Repo.AsQueryable()
      .Where(d => SqlFunc.SplitIn(d.Ancestors, deptId.ToString()));
    return await queryable.ToListAsync();
  }

  /// <summary>
  ///   修改所在部门正常状态
  /// </summary>
  /// <param name="deptIds">部门ID组</param>
  /// <returns></returns>
  public async Task<int> UpdateDeptStatusNormalAsync(IEnumerable<long> deptIds)
  {
    return await Updateable()
      .SetColumns(col => col.Status == Status.Enabled)
      .Where(col => deptIds.Contains(col.DeptId))
      .ExecuteCommandAsync();
  }

  /// <summary>
  ///   是否存在子节点
  /// </summary>
  /// <returns></returns>
  public async Task<bool> HasChildByDeptIdAsync(long parentDeptId)
  {
    var query = new SysDeptDto { DelFlag = DelFlag.No, ParentId = parentDeptId };
    return await AnyAsync(query);
  }

  /// <summary>
  ///   删除部门管理信息
  /// </summary>
  public async Task<int> DeleteDeptByIdAsync(long deptId)
  {
    return await Updateable()
      .SetColumns(col => col.DelFlag == DelFlag.Yes)
      .Where(col => col.DeptId == deptId)
      .ExecuteCommandAsync();
  }
}
