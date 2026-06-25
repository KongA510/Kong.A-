namespace ArasToolkit.Core.Interfaces;

/// <summary>
/// Aras 连接池接口 — 管理多个独立 Aras 连接，供多线程导入使用。
/// 池中连接在导入完成后归还复用，不自动销毁。
/// </summary>
public interface IArasConnectionPool
{
    /// <summary>当前池大小</summary>
    int PoolSize { get; }

    /// <summary>池中可用连接数</summary>
    int Available { get; }

    /// <summary>
    /// 使用当前 IArasConnectionService.CurrentConnection 的信息重新初始化连接池。
    /// 先清空旧池（Logout 旧连接），再创建新连接。poolSize=0 时仅清空。
    /// </summary>
    void Reinitialize(int poolSize);

    /// <summary>清空连接池，注销所有池中连接</summary>
    void Clear();
}
