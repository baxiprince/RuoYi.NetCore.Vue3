namespace RuoYi.Data.Models;

/// <summary>
///   路由显示信息
/// </summary>
public class MetaVo
{
  public MetaVo()
  {
  }

  public MetaVo(string title, string icon)
  {
    Title = title;
    Icon = icon;
  }

  public MetaVo(string title, string icon, bool noCache)
  {
    Title = title;
    Icon = icon;
    NoCache = noCache;
  }

  public MetaVo(string title, string icon, string link)
  {
    Title = title;
    Icon = icon;
    Link = link;
  }

  public MetaVo(string title, string icon, bool noCache, string link)
  {
    Title = title;
    Icon = icon;
    NoCache = noCache;
    if (!string.IsNullOrEmpty(link) && link.StartsWith("http", StringComparison.OrdinalIgnoreCase)) Link = link;
  }

  /**
   * 设置该路由在侧边栏和面包屑中展示的名字
   */
  public string Title { get; set; }

  /**
   * 设置该路由的图标，对应路径src/assets/icons/svg
   */
  public string Icon { get; set; }

  /**
   * 设置为true，则不会被
   * <keep-alive>缓存
   */
  public bool NoCache { get; set; }

  /**
   * 内链地址（http(s)://开头）
   */
  public string Link { get; set; }
}
