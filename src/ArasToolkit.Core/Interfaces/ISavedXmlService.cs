using ArasToolkit.Core.Entities;

namespace ArasToolkit.Core.Interfaces;

/// <summary>XML 格式化与比对工具共用的数据库存储服务。</summary>
public interface ISavedXmlService
{
    /// <summary>按最近保存时间倒序读取 XML；格式化页可请求管理员查询全部，默认仍按当前用户隔离。</summary>
    Task<List<SavedXml>> GetAllAsync(bool includeAllForAdmin = false);

    /// <summary>按名称新增或覆盖当前用户的 XML。</summary>
    Task<SavedXml> SaveAsync(string name, string xmlContent);

    /// <summary>删除当前用户指定的 XML。</summary>
    Task DeleteAsync(string id);
}
