using RuoYi.Generator.Dtos;

namespace RuoYi.Generator.RepoSql;

public class RepoSqlService : ITransient
{
  private readonly MySql _mySql;
  private readonly SqlServer _sqlServer;
  private readonly ISqlSugarRepository _sqlSugarRepository;

  public RepoSqlService(ISqlSugarRepository sqlSugarRepository, MySql mySql, SqlServer sqlServer)
  {
    _sqlSugarRepository = sqlSugarRepository;
    _mySql = mySql;
    _sqlServer = sqlServer;
  }

  public DbType GetDbType()
  {
    return _sqlSugarRepository.Context.CurrentConnectionConfig.DbType;
  }

  public IDbSql GetDbSql()
  {
    var dbType = GetDbType();
    return dbType switch
    {
      DbType.MySql => _mySql,
      DbType.SqlServer => _sqlServer,
      //DbType.Oracle => _oracle,
      _ => _mySql
    };
  }

  // Table 查询
  public SqlAndParameter GetDbTableListSqlAndParameter(GenTableDto dto)
  {
    return GetDbSql().GetDbTableListSqlAndParameter(dto);
  }

  // Table: 按名字查询
  public string GetDbTableListByNamesSql()
  {
    return GetDbSql().GetDbTableListByNamesSql();
  }

  // 列 按table名称查询
  public string GetDbTableColumnsByName()
  {
    return GetDbSql().GetDbTableColumnsByName();
  }
}
