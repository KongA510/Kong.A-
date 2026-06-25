using System.Collections.Concurrent;
using Aras.IOM;

namespace ArasToolkit.Services.Services;

/// <summary>
/// Aras 连接池 — 管理多个独立 HttpServerConnection + Innovator，
/// 供数据汇入多线程并发执行 AML 时使用。
/// 每个连接独立持有 Cookie/Session，线程间无竞态。
/// </summary>
public class ArasConnectionPool
{
    private readonly ConcurrentBag<PooledConnection> _pool = [];

    /// <summary>当前池大小（=线程数上限）</summary>
    public int PoolSize { get; private set; }

    /// <summary>池中可用连接数</summary>
    public int Available => _pool.Count;

    /// <summary>
    /// 登录 Aras 并创建 N 个独立连接放入池中。
    /// 先清空旧连接（如果有残留）。
    /// </summary>
    /// <param name="url">Aras 服务器 URL</param>
    /// <param name="database">数据库名</param>
    /// <param name="username">用户名</param>
    /// <param name="md5Password">MD5 小写十六进制密码</param>
    /// <param name="poolSize">连接池大小（=线程数上限，最大10）</param>
    public void Initialize(string url, string database, string username, string md5Password, int poolSize)
    {
        // 先清空旧连接（如果还有残留）
        Clear();
        poolSize = Math.Min(poolSize, 10); // 线程上限10
        PoolSize = poolSize;

        // IOM 的 IomFactory 不支持并发创建，串行逐个建立连接
        for (int i = 0; i < poolSize; i++)
        {
            HttpServerConnection conn = IomFactory.CreateHttpServerConnection(url, database, username, md5Password);
            var innovator = new Innovator(conn);
            _pool.Add(new PooledConnection(conn, innovator, i));
        }
    }

    /// <summary>
    /// 从池中租用一个连接（阻塞等待直到有可用连接）
    /// </summary>
    public PooledConnection Rent()
    {
        // ConcurrentBag.TryTake 是非阻塞的，使用自旋+短暂休眠实现阻塞等待
        PooledConnection? item;
        while (!_pool.TryTake(out item))
            Thread.Sleep(10);
        return item;
    }

    /// <summary>
    /// 归还连接到池中
    /// </summary>
    public void Return(PooledConnection item)
    {
        if (item != null)
            _pool.Add(item);
    }

    /// <summary>
    /// 清空连接池（注销所有连接）
    /// </summary>
    public void Clear()
    {
        while (_pool.TryTake(out var item))
        {
            try { item.Connection.Logout(); }
            catch { /* 静默处理注销异常 */ }
        }
        PoolSize = 0;
    }
}

/// <summary>
/// 池化连接 — 封装一个独立 Aras 连接及其标识
/// </summary>
public class PooledConnection
{
    public int Index { get; }
    public HttpServerConnection Connection { get; }
    public Innovator Innovator { get; }

    public PooledConnection(HttpServerConnection connection, Innovator innovator, int index)
    {
        Connection = connection;
        Innovator = innovator;
        Index = index;
    }
}
