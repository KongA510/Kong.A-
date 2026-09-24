using System.Xml.Linq;
using Aras.IOM;
using ArasToolkit.Core.Entities;
using ArasToolkit.Core.Interfaces;
using ArasToolkit.Core.Models;
using ArasToolkit.Services.Data;
using Microsoft.EntityFrameworkCore;

namespace ArasToolkit.Services.Services;

public sealed class ArasMigrationSessionFactory(
    IDbContextFactory<ArasToolkitDbContext> dbFactory,
    IErrorLogService errors) : IArasMigrationSessionFactory
{
    public async Task<List<MigrationEndpoint>> GetConnectionsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
            var user = CurrentUserContext.Current?.Id ?? throw new InvalidOperationException("请先登录工具箱。");
            var rows = await db.ArasLoginConfigs.AsNoTracking().Where(x => x.UserId == user)
                .OrderByDescending(x => x.CreatorOn).ToListAsync(cancellationToken);
            return rows.Select(Endpoint).ToList();
        }
        catch (Exception ex)
        {
            await errors.LogErrorAsync("导包-读取连接", ex.Message, ErrorLog.LevelP0);
            throw;
        }
    }

    public async Task<MigrationCredential> GetCredentialAsync(string connectionId, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
            var user = CurrentUserContext.Current?.Id ?? throw new InvalidOperationException("请先登录工具箱。");
            var row = await db.ArasLoginConfigs.AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == connectionId && x.UserId == user, cancellationToken)
                ?? throw new InvalidOperationException("连接不存在或不属于当前用户。");
            return new MigrationCredential { Endpoint = Endpoint(row), Md5Password = row.Md5Password };
        }
        catch (Exception ex)
        {
            await errors.LogErrorAsync("导包-连接凭据", ex.Message, ErrorLog.LevelP0);
            throw;
        }
    }

    public async Task<IArasMigrationSession> OpenAsync(string connectionId, CancellationToken cancellationToken = default)
    {
        var credential = await GetCredentialAsync(connectionId, cancellationToken);
        HttpServerConnection? connection = null;
        try
        {
            var session = await Task.Run(() =>
            {
                // Each session owns its connection; never update IArasConnectionService or its pool.
                connection = IomFactory.CreateHttpServerConnection(credential.Endpoint.Url,
                    credential.Endpoint.Database, credential.Endpoint.Username, credential.Md5Password);
                connection.Timeout = 120000;
                var login = connection.Login();
                if (login.isError()) throw new InvalidOperationException("Aras 登录失败：" + login.getErrorString());
                return new Session(credential.Endpoint, connection, login.getInnovator(), errors);
            }, cancellationToken);
            var version = PackageXml.Items(await session.QueryAsync("<AML><Item type=\"Variable\" action=\"get\" select=\"name,value\"><name condition=\"in\">'VersionMajor','VersionMinor','VersionServiceUpdate','VersionBuild'</name></Item></AML>", cancellationToken))
                .ToDictionary(x => (string?)x.Element("name") ?? "", x => (string?)x.Element("value") ?? "");
            session.Endpoint.Version = string.Join(".", new[] { "VersionMajor", "VersionMinor", "VersionServiceUpdate", "VersionBuild" }
                .Select(k => version.GetValueOrDefault(k, "?")));
            return session;
        }
        catch (Exception ex)
        {
            await errors.LogErrorAsync("导包-建立连接", PackageXml.Redact(ex.Message, credential.Md5Password), ErrorLog.LevelP0);
            if (connection != null)
            {
                try { connection.Logout(); }
                catch (Exception logoutError) { await errors.LogErrorAsync("导包-释放失败连接", PackageXml.Redact(logoutError.Message, credential.Md5Password), ErrorLog.LevelP1); }
            }
            if (ex is OperationCanceledException) throw;
            throw new InvalidOperationException(PackageXml.Redact(ex.Message, credential.Md5Password));
        }
        finally { credential.Md5Password = ""; }
    }

    private static MigrationEndpoint Endpoint(ArasLoginConfig config) => new()
    {
        Id = config.Id, Url = config.Url, Database = config.DatabaseName, Username = config.Username
    };

    private sealed class Session(MigrationEndpoint endpoint, HttpServerConnection connection, Innovator innovator,
        IErrorLogService errors) : IArasMigrationSession
    {
        private readonly SemaphoreSlim _gate = new(1, 1);
        public MigrationEndpoint Endpoint { get; } = endpoint;
        public async Task<string> QueryAsync(string aml, CancellationToken cancellationToken = default)
        {
            PackageXml.ValidateReadOnly(aml);
            await _gate.WaitAsync(cancellationToken);
            try
            {
                return await Task.Run(() =>
                {
                    var item = innovator.applyAML(aml);
                    if (item == null) throw new InvalidOperationException("Aras 查询没有返回结果。");
                    if (item.isError())
                    {
                        if (item.getErrorCode() == "0") return "<Result />";
                        throw new InvalidOperationException(item.getErrorString());
                    }
                    return item.ToString();
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                await errors.LogErrorAsync("导包-元数据查询", ex.Message, ErrorLog.LevelP0);
                throw;
            }
            finally { _gate.Release(); }
        }

        public async ValueTask DisposeAsync()
        {
            await _gate.WaitAsync();
            try { await Task.Run(() => connection.Logout()); }
            catch (Exception ex) { await errors.LogErrorAsync("导包-关闭连接", ex.Message, ErrorLog.LevelP1); }
            finally { _gate.Release(); _gate.Dispose(); }
        }
    }
}
