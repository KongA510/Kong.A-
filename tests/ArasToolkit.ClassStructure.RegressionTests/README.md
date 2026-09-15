# 类结构汇入回归

运行：`dotnet run --project tests/ArasToolkit.ClassStructure.RegressionTests`

使用真实 R37 IOM 和替代传输层，不连接 Aras 或数据库。通过公开服务入口覆盖 Excel 解析、半角斜杠转换、转换后名称碰撞、十级路径、根 ID、XML Text 序列化、特殊字符、重复汇入、异常回读、取消和日志。

BL 实测背景和复现步骤见 `design/类结构汇入使用说明.md`。
