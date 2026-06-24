using ArasToolkit.Core.Interfaces;
using ArasToolkit.Services.Data;
using ArasToolkit.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ArasToolkit.Services;

/// <summary>
/// 服务层依赖注入扩展
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 注册所有服务层服务
    /// </summary>
    public static IServiceCollection AddArasToolkitServices(this IServiceCollection services)
    {
        // 注册 EF Core DbContext 工厂（单例工厂，每次操作创建新的短生命周期 DbContext）
        services.AddDbContextFactory<ArasToolkitDbContext>(options =>
        {
            // 连接字符串在 ArasToolkitDbContext.OnConfiguring 中从 DBSeeting.json 读取
        });

        // 注册应用用户服务
        services.AddSingleton<IAppUserService, AppUserService>();

        // 注册操作日志服务（单例）
        services.AddSingleton<IOperationLogService, OperationLogService>();

        // 注册配置服务（单例）
        services.AddSingleton<IConfigService, ConfigService>();

        // 注册Aras连接服务（单例，全局共享）
        services.AddSingleton<IArasConnectionService, ArasConnectionService>();

        // 注册登录服务
        services.AddTransient<ILoginService, LoginService>();

        // 注册Excel服务
        services.AddTransient<IExcelService, ExcelService>();

        // 注册更新日志服务（单例）
        services.AddSingleton<IChangelogService, ChangelogService>();

        // 注册错误日志服务（单例）
        services.AddSingleton<IErrorLogService, ErrorLogService>();

        // 注册待办项目服务（单例，通过 IDbContextFactory 创建 DbContext）
        services.AddSingleton<ITodoService, TodoService>();

        // 注册图表报表服务（单例）
        services.AddSingleton<IChartService, ChartService>();

        return services;
    }
}
