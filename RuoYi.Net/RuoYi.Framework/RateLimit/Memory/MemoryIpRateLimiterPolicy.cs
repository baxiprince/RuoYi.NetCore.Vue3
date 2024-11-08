using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using RuoYi.Framework.Utils;

namespace RuoYi.Framework.RateLimit.Memory;

public class MemoryIpRateLimiterPolicy : IRateLimiterPolicy<string>
{
  private readonly ILogger<MemoryIpRateLimiterPolicy> _logger;

  public MemoryIpRateLimiterPolicy(ILogger<MemoryIpRateLimiterPolicy> logger)
  {
    _logger = logger;
    OnRejected = (context, token) =>
    {
      context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
      context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");

      return ValueTask.CompletedTask;
    };
  }

  public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get; }

  public RateLimitPartition<string> GetPartition(HttpContext httpContext)
  {
    var ip = IpUtils.GetIpAddr();
    var rateLimitConfig = App.GetConfig<IpRateLimitConfig>(nameof(IpRateLimitConfig));
    var permitLimit = rateLimitConfig?.PermitLimit ?? 10; // 每个ip, 并发最大数量
    var window = rateLimitConfig?.Window ?? 1; // 默认一秒

    _logger.LogInformation($"IP: {ip} | PermitLimit: {permitLimit} | Window: {window}");

    // 固定窗口限流
    return RateLimitPartition.GetFixedWindowLimiter($"{LimitType.IP}_{ip}", key => new FixedWindowRateLimiterOptions
    {
      PermitLimit = permitLimit,
      Window = TimeSpan.FromSeconds(window)
    });
  }
}
