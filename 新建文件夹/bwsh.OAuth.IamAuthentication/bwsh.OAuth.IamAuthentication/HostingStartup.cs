using Aras.OAuth.Server.Infrastructure;
using bwsh.OAuth.IamAuthentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

[assembly: HostingStartup(typeof(HostingStartup))]
namespace bwsh.OAuth.IamAuthentication
{
    public class HostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            // ============================================================
            // 注册 Forwarded Headers，解决 HTTPS 反向代理下 scheme 识别问题
            // 必须在宿主应用自己的 Configure 之前执行，以保证 UseForwardedHeaders
            // 在 UseAuthentication 之前生效
            // ============================================================
            builder.ConfigureServices((hostingContext, services) =>
            {
                Assembly pluginAssembly = typeof(HostingStartup).Assembly;
                IConfiguration configuration = hostingContext.Configuration.GetPluginConfiguration(pluginAssembly);
                var pluginOptions = new PluginOptions();
                configuration.Bind(pluginOptions);
                services.Configure<PluginOptions>(configuration);

                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;

                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOAuth<IamOptions, IamHandler>(OpenIdConnectDefaults.AuthenticationScheme, pluginOptions.DisplayName, options =>
                {
                    options.Database = pluginOptions.Database;
                    options.PlmSecret = pluginOptions.PlmSecret;
                 
                    options.AuthorizationEndpoint = pluginOptions.Instance;
                    options.TokenEndpoint = pluginOptions.TokenEndpoint;
                    options.UserInformationEndpoint = pluginOptions.UserInformationEndpoint;
                    options.CallbackPath = pluginOptions.CallbackPath;
                    options.ClientId = pluginOptions.ClientId;
                    options.ClientSecret = pluginOptions.ClientSecret;                    
                    options.Scope.Clear();
                    options.Scope.Add("user");
                    options.SaveTokens = true;
                    options.UsePkce = false;
                });

                // 配置 ForwardedHeadersOptions，信任反向代理转发的请求头
                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders =
                        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

                    // 清除默认的已知网络/代理列表。
                    // 当反向代理（nginx / IIS ARR）与 ASP.NET Core
                    // 运行在同一台机器时，ASP.NET Core 默认只信任
                    // 本地回环地址的代理，需要清除限制
                    options.KnownNetworks.Clear();
                    options.KnownProxies.Clear();
                });
            });

            // ============================================================
            // 在管道中注册 ForwardedHeaders 中间件（必须在 UseAuthentication 之前）
            // ============================================================
            builder.Configure((context, app) =>
            {
                app.UseForwardedHeaders();
            });

            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        }
    }
}
