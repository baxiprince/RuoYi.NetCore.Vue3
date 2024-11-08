using Microsoft.Extensions.DependencyInjection;

namespace RuoYi.Framework.DependencyInjection;

/// <summary>
///   创建作用域静态类
/// </summary>
[SuppressSniffer]
public static class Scoped
{
  /// <summary>
  ///   创建一个作用域范围
  /// </summary>
  /// <param name="handler"></param>
  /// <param name="scopeFactory"></param>
  public static void Create(Action<IServiceScopeFactory, IServiceScope> handler, IServiceScopeFactory scopeFactory = default)
  {
    CreateAsync(async (fac, scope) =>
    {
      handler(fac, scope);
      await Task.CompletedTask;
    }, scopeFactory).GetAwaiter().GetResult();
  }

  /// <summary>
  ///   创建一个作用域范围（异步）
  /// </summary>
  /// <param name="handler"></param>
  /// <param name="scopeFactory"></param>
  public static async Task CreateAsync(Func<IServiceScopeFactory, IServiceScope, Task> handler,
    IServiceScopeFactory scopeFactory = default)
  {
    // 禁止空调用
    if (handler == null) throw new ArgumentNullException(nameof(handler));

    // 创建作用域
    var (scoped, serviceProvider) = CreateScope(ref scopeFactory);

    try
    {
      // 执行方法
      await handler(scopeFactory, scoped);
    }
    finally
    {
      // 释放
      scoped.Dispose();
      if (serviceProvider != null) await serviceProvider.DisposeAsync();
    }
  }

  /// <summary>
  ///   创建一个作用域
  /// </summary>
  /// <param name="scopeFactory"></param>
  /// <returns></returns>
  private static (IServiceScope scoped, ServiceProvider? undisposeServiceProvider) CreateScope(
    ref IServiceScopeFactory scopeFactory)
  {
    ServiceProvider undisposeServiceProvider = default;

    // 解析服务作用域工厂
    var scoped = scopeFactory.CreateScope();
    return (scoped, undisposeServiceProvider);
  }
}
