管理员共享查询回归验证，使用真实服务、WinUI ViewModel 和 EF Core 内存数据库，不连接业务数据库。

运行：

```powershell
dotnet run --project tests/ArasToolkit.SharedLibrary.RegressionTests
```

覆盖两个管理员读取全部 SQL/AML/XML 片段、代码主题和代码段，普通用户隔离，XML 格式化共享查询与 XML 比对原权限，非创建者编辑/删除/排序限制，误建副本拦截，以及本人 CRUD 与混合列表排序。
