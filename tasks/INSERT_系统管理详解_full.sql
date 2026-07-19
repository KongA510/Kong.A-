-- ============================================================
-- Aras Innovator 帮助文档 → 个人知识库 SQL
-- 生成日期: 2026-07-05
-- 条目数: 244
-- ============================================================

USE [Kong.A];
GO

-- [1/244] About_Actions.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('fd4fdc9081b9',
   N'关于操作（Actions）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于操作（Actions）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_Actions.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于操作</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作（Action）是一种可自定义的菜单项，选中时会调用一个方法（Method）。操作的名称即为菜单上显示的标签。每个操作都关联一个方法，当用户选择该操作时即会执行。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作可以定义为通用类型，也可以绑定到特定的项目类型（ItemType）或具体项目（Item）上。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">通用操作在菜单中始终可见。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">类型为「ItemType」和「Item」的操作，必须在操作选项卡上作为关系链接到对应的 ItemType。</Run></Paragraph>
</FlowDocument>',
   N'关于操作 操作（Action）是一种可自定义的菜单项，选中时会调用一个方法（Method）。操作的名称即为菜单上显示的标签。每个操作都关联一个方法，当用户选择该操作时即会执行。 操作可以定义为通用类型，也可以绑定到特定的项目类型（ItemType）或具体项目（Item）上。 通用操作在菜单中始终可见。 类型为「ItemType」和「Item」的操作，必须在操作选项卡上作为关系链接到对应的 Item',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [2/244] About_Configurable_Grids.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('f83f8415215c',
   N'关于可配置网格',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于可配置网格</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_Configurable_Grids.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于可配置网格</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可配置网格（Configurable Grid）是 Innovator 框架的一部分，用于质量规划和项目管理解决方案，也可用于自定义应用程序。它在目录（TOC）的「管理」类别下以「Grids」名称显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可配置网格本质上是一个客户端控件，通过 AML 查询从 Innovator 数据库中检索数据并填充到网格中。网格定义包含 AML 查询和 XPath 表达式用于获取数据、网格加载时执行的方法，以及列规格等设置。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">实现可配置网格是一项高级开发功能。</Run></Paragraph>
</FlowDocument>',
   N'关于可配置网格 可配置网格（Configurable Grid）是 Innovator 框架的一部分，用于质量规划和项目管理解决方案，也可用于自定义应用程序。它在目录（TOC）的「管理」类别下以「Grids」名称显示。 可配置网格本质上是一个客户端控件，通过 AML 查询从 Innovator 数据库中检索数据并填充到网格中。网格定义包含 AML 查询和 XPath 表达式用于获取数据、网格加载时',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [3/244] About_Configuration_Management.htm (2 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('9da5f6f3f9dc',
   N'关于配置管理',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于配置管理</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_Configuration_Management.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于配置管理</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">配置管理（Configuration Management）是通过管理组织的产品、设施和流程的需求（包括变更），并确保结果符合要求的过程。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Innovator 中的所有项目都具有生命周期，项目可以设置为可版本化。可版本化项目的变更通过版本（Versions）、修订（Revisions）及其相关行为来实现，这些都属于 Innovator 框架的一部分。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于版本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于修订</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">生命周期项目行为</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系类型项目行为</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">产品工程解决方案利用 Innovator 框架将配置管理应用于零部件（Part）和文档（Document）。类似的功能也可应用于其他 Innovator 项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择在主网格中显示的版本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">对于可版本化的项目，搜索工具栏允许用户选择要显示的版本。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Current — 当前版本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Latest — 所有版本中最新的版本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Released — 已发布状态中最新的版本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Effective — 在特定日期生效的版本。日期可以手动输入，也可以通过点击日历图标从日历中选择。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b221fd361b59.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9f3553b780f4.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'关于配置管理 配置管理（Configuration Management）是通过管理组织的产品、设施和流程的需求（包括变更），并确保结果符合要求的过程。 Innovator 中的所有项目都具有生命周期，项目可以设置为可版本化。可版本化项目的变更通过版本（Versions）、修订（Revisions）及其相关行为来实现，这些都属于 Innovator 框架的一部分。 关于版本 关于修订 生命周期项目',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [4/244] About_Copy_and_Paste.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('dddaf926ca5c',
   N'关于复制和粘贴',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于复制和粘贴</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_Copy_and_Paste.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于复制和粘贴</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">复制和粘贴功能可在 Innovator 12.0 中高效使用。项目可以从数据模型中复制并粘贴到兼容的项目中。此外，该功能还允许用户：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从关系网格中复制一个或多个（多选）项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将剪贴板内容粘贴到同一源项目或不同但同类型的源项目中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在弹出窗口中查看剪贴板内容，窗口会显示源项目、关系类型和网格中的关联项目信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">维护一个剪贴板，其中包含从不同源类型复制的项目，粘贴过程会自动判断粘贴的项目与目标源项目是否兼容。</Run></Paragraph>
</FlowDocument>',
   N'关于复制和粘贴 复制和粘贴功能可在 Innovator 12.0 中高效使用。项目可以从数据模型中复制并粘贴到兼容的项目中。此外，该功能还允许用户： 从关系网格中复制一个或多个（多选）项目。 将剪贴板内容粘贴到同一源项目或不同但同类型的源项目中。 在弹出窗口中查看剪贴板内容，窗口会显示源项目、关系类型和网格中的关联项目信息。 维护一个剪贴板，其中包含从不同源类型复制的项目，粘贴过程会自动判断粘贴的',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [5/244] About_Creating_Relationships.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('9c3a1e887ed8',
   N'创建关系',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建关系</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_Creating_Relationships.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建关系</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系（Relationship）将两个项目绑定在一起——源（父）项目和关联（子）项目，使源项目能够引用关联项目中保存的信息。创建关系时，从源项目出发，创建一个指向关联项目的链接。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">根据管理员对 ItemType 的配置，添加关联项目的选项可能是部分启用或全部启用。三种主要的关系类型如下：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建带关联项目的关系（选取关联或创建关联）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">添加关系时，如果只有两个选项：「选取关联」或「创建关联」，则必须添加关联项目。您可以选取现有项目（选取关联）或创建新项目（创建关联），但在这类情况下关联项目是必填的。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例：Part-BOM 关系。详细步骤请参阅「创建带关联项目的关系」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建关联项目可选的关系（选取关联、创建关联或无关联）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">添加关系时，如果三个选项都有：「选取关联」、「创建关联」或「无关联」，则关联项目是可选的。您可以添加关联项目（选取关联或创建关联），也可以选择不关联任何项目（无关联），仅在关系中添加信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例：Part-Alternate 关系。详细步骤请参阅「创建可选关联项目的关系」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建不能添加关联项目的关系（无关联）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">添加关系时，如果只有一个选项：「无关联」，则不能关联任何项目，只能在关系中添加信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例：List-Value 关系。详细步骤请参阅「创建无关联项目的关系」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当您添加关系时，如果只有两个选项：选择关联或创建关联，则必须添加关联项目。您可以选择现有项目（选择关联）或创建新项目（创建关联），但在这种情况下关联项目是必需的。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建带关联项目的关系</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例：Part-BOM 关系。有关详细步骤，请参阅创建带关联项目的关系。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建关联项目为可选的关系（选择关联、创建关联或无关联）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当您添加关系时，如果有全部三个选项：选择关联、创建关联或无关联，则添加关联项目是可选的。您可以添加关联项目（选择关联或创建关联），也可以选择不关联任何项目（无关联），仅添加关系信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例：Part-Alternate 关系。有关详细步骤，请参阅创建带可选关联项目的关系。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建不能添加关联项目的关系（无关联）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当您添加关系时，如果只有一个选项：无关联，则您不能关联任何关联项目，只能添加关系信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例：List-Value 关系。有关详细步骤，请参阅创建不能添加关联项目的关系</Run></Paragraph>
</FlowDocument>',
   N'创建关系 关系（Relationship）将两个项目绑定在一起——源（父）项目和关联（子）项目，使源项目能够引用关联项目中保存的信息。创建关系时，从源项目出发，创建一个指向关联项目的链接。 根据管理员对 ItemType 的配置，添加关联项目的选项可能是部分启用或全部启用。三种主要的关系类型如下： 创建带关联项目的关系（选取关联或创建关联） 添加关系时，如果只有两个选项：「选取关联」或「创建关联」',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [6/244] About_Data_Types.htm (2 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('9f14a540ee22',
   N'关于数据类型',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于数据类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_Data_Types.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于数据类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在配置 ItemType 的属性时，其中一列是「数据类型」（Data Type）。数据类型是 Innovator 中每个项目的每个属性的特性。下拉列表中可用的数据类型如下所述，我们将逐一详细介绍。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">布尔值（Boolean）— 允许 true 或 false 的回答，通常在表单上显示为复选框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">颜色（Color）— 十六进制颜色值，表单上自动关联颜色选择控件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">颜色列表（Color List）— 使用此属性前，必须已有可用的颜色列表作为数据源。如果指定颜色列表为数据类型，必须提供数据源。在表单上，该列表以下拉框形式显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">日期（Date）— 日期值，可从日期对话框中选择。客户端上日期的显示格式由用户的 Windows 区域设置决定。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">小数（Decimal）— 值具有精度（总位数）和小数位数（小数点后的位数）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">过滤列表（Filter List）— 经过筛选的值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">外部（Foreign）— 请参阅外部数据类型。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">浮点数（Float）— 使用科学计数法表示的值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">格式化文本（Formatted Text）— 带格式的文本，可通过「Innovator 格式化文本编辑器」编辑。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3ebb0af419c2.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/631320963155.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">图片（Image）— 图片，如图标等。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">整数（Integer）— 整数。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目（Item）— 另一个 Innovator 项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">列表（List）— 预定义的值集合，通常在表单上以下拉框显示。更多信息请参阅「创建列表」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">MD5 — 加密值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">多语言字符串（Multilingual String）— 包含多种语言值的字符串，请参阅「国际化」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">多值列表（MultiValue List）— 可包含多个值的 Innovator 列表。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">序列（Sequence）— 按指定规则递增和格式化的连续数字系列。更多信息请参阅「序列」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字符串（String）— 由字母和数字组成的字符串，长度由属性设定。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文本（Text）— 长字符串，最大长度可达 1GB。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">变量（Variable）— Innovator 变量。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">带格式的文本，可通过「Innovator 格式化文本编辑器」编辑</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图片</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图片，例如图标。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">整数</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">整数值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">另一个 Innovator 项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">列表</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">一组预定义的值，通常在表单上以下拉框形式显示。有关更多信息，请参阅创建列表。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">MD5</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">加密值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">多语言字符串</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">包含多种语言值的字符串，请参阅国际化</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">多值列表</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可以包含多个值的 Innovator 列表。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">序列</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">按序列属性中指定的格式递增和格式化的连续数字序列。有关更多信息，请参阅序列。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字符串</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">长度由属性选定的字母和数字字符串。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">最长可达1GB的字母和数字字符串。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">变量</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Innovator 变量。</Run></Paragraph>
</FlowDocument>',
   N'关于数据类型 在配置 ItemType 的属性时，其中一列是「数据类型」（Data Type）。数据类型是 Innovator 中每个项目的每个属性的特性。下拉列表中可用的数据类型如下所述，我们将逐一详细介绍。 布尔值（Boolean）— 允许 true 或 false 的回答，通常在表单上显示为复选框。 颜色（Color）— 十六进制颜色值，表单上自动关联颜色选择控件。 颜色列表（Color L',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [7/244] About_Files.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('30db324fb1ea',
   N'关于文件',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于文件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_Files.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于文件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文件（Files）ItemType 是用于管理 Innovator 保险柜（Vault）中文件存储的核心 ItemType。它记录每个存储文件的名称、所在保险柜、文件类型和签出用户。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">需要注意的是，不应直接访问文件 ItemType 作为向保险柜存储文件的方式。要将项目存入保险柜，用户应在需要关联文件的 ItemType 上添加一个数据类型为「Item」、数据源为「File」的属性。</Run></Paragraph>
</FlowDocument>',
   N'关于文件 文件（Files）ItemType 是用于管理 Innovator 保险柜（Vault）中文件存储的核心 ItemType。它记录每个存储文件的名称、所在保险柜、文件类型和签出用户。 需要注意的是，不应直接访问文件 ItemType 作为向保险柜存储文件的方式。要将项目存入保险柜，用户应在需要关联文件的 ItemType 上添加一个数据类型为「Item」、数据源为「File」的属性。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [8/244] About_FileTypes.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('7f756f9b5951',
   N'关于文件类型',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于文件类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_FileTypes.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于文件类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文件类型（FileType）定义了 Innovator 保险柜管理的文件的查看器处理规则。FileType 项目用于将文件扩展名与所需的 MIME 类型关联并传递给浏览器。浏览器使用 MIME 类型关联来启动与该文件类型关联的应用程序或插件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">此外，还可以通过「View With」选项卡将特定应用程序与文件扩展名显式关联。View With 选项卡可以将文件定向到特定的应用程序进行查看。</Run></Paragraph>
</FlowDocument>',
   N'关于文件类型 文件类型（FileType）定义了 Innovator 保险柜管理的文件的查看器处理规则。FileType 项目用于将文件扩展名与所需的 MIME 类型关联并传递给浏览器。浏览器使用 MIME 类型关联来启动与该文件类型关联的应用程序或插件。 此外，还可以通过「View With」选项卡将特定应用程序与文件扩展名显式关联。View With 选项卡可以将文件定向到特定的应用程序进行查',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [9/244] About_Forms.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('176ff5de9867',
   N'关于表单',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于表单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_Forms.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于表单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单（Form）是一个动态生成的页面，包含 ItemType 的属性信息，并作为终端用户的数据输入区域。表单由管理员设计，以预定布局呈现相关信息，并可通过控件和事件成为终端用户交互的基础。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">让我们以 Part ItemType 为例，了解表单的各个组成部分。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Part 项目窗口</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c0d62de5c61a.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">项目菜单（Item Menu）：项目菜单与主菜单类似，您可以执行各种通用任务，如新建项目、保存当前项目、认领/取消认领当前项目等。更多信息请参阅「项目菜单」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目工具栏（Item Toolbar）：项目工具栏显示常用任务的按钮列表，如添加、查看编辑、保存、复制并保存等。有关项目工具栏上各按钮的说明，请参阅「项目工具栏」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目信息（Item Information）：项目信息显示 ItemType 名称及其关联的图片。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单字段（Form Fields）：表单字段是用户的数据输入区域。管理员设计表单并决定为 ItemType 插入哪些字段。因此，根据 ItemType 及其管理员配置，表单中的字段可能有所不同。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选项卡视图（Tabbed View）：选项卡视图包含与项目关联的各个选项卡。您可以点击每个选项卡查看和访问其内容。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系工具栏（Relationship Toolbar）：关系工具栏提供执行各种任务的按钮，如添加关联项目、替换关联项目、提升关联项目等。有关关系工具栏上各按钮的信息，请参阅「关系工具栏」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系网格（Relationship Grid）：关系网格列出关联/子项目。网格包含关系项目属性和子项目属性。高亮显示的属性属于关系项目，未高亮的属性属于子项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系网格：关系网格列出了关联/子项目。网格包含关系项目属性和子项目属性。高亮属性属于关系项目，非高亮属性属于子项目。</Run></Paragraph>
</FlowDocument>',
   N'关于表单 表单（Form）是一个动态生成的页面，包含 ItemType 的属性信息，并作为终端用户的数据输入区域。表单由管理员设计，以预定布局呈现相关信息，并可通过控件和事件成为终端用户交互的基础。 让我们以 Part ItemType 为例，了解表单的各个组成部分。 Part 项目窗口 项目菜单（Item Menu）：项目菜单与主菜单类似，您可以执行各种通用任务，如新建项目、保存当前项目、认领/',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [10/244] About_Internationalization_and_Localization.htm (8 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('dde1f0791f06',
   N'关于 Innovator 的国际化和本地化',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于 Innovator 的国际化和本地化</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_Internationalization_and_Localization.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于 Innovator 的国际化和本地化</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从 9.0 版本开始，Innovator 支持国际化和本地化，具有以下特性：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">英语为默认语言</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可以使用语言包创建或添加其他语言</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">多语言字符串（Multilingual String）数据类型用于以用户选择的语言显示值</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">语言包根据客户端区域设置显示相应的菜单、标签和值，使用户能够使用已安装的语言导航和使用 Innovator</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Innovator 客户端会根据客户端区域设置自动格式化日期、时间和小数</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果设置了企业时区，企业时间将显示在 Innovator 客户端中，所有日期和时间都以企业时间显示</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">新增 ItemType</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">区域设置（Locale）— Innovator 区域设置 ItemType 代表 Innovator 实例支持的区域设置。它映射到 Windows 区域设置，如 English (United States) 或 German (Germany)，并具有标识该区域设置使用的 Innovator 语言及其表示方式的属性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">语言（Language）— Innovator 语言 ItemType 代表 Innovator 实例支持的语言。安装后 Innovator 仅包含默认语言英语，管理员可以添加其他语言。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">新增数据类型</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b6f7799ece70.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">多语言字符串（Multilingual String）— 多语言字符串类型的属性可以为 Innovator 中安装的每种语言设置一个值。多语言字符串的主要用途是让用户能够以多种语言导航和使用 Innovator。它用于菜单、表单和网格上的标签以及列表值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">多语言字符串不能标记为必填或设置默认值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">因此，多语言字符串主要用于元数据（关于数据的数据）。可以将多语言字符串用于实例数据，但这样做需要在业务流程中提供多语言值。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7338745fa1b6.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">编辑多语言字符串</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在使用多语言字符串且数据库中安装了其他语言的地方，具有更新权限的用户可以使用多语言输入对话框编辑任何语言的值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要从表单打开多语言输入对话框，请点击字段右侧的省略号按钮。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要从网格打开多语言输入对话框，请点击单元格进入编辑模式，然后按 F2。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">新增服务器变量</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">DefaultLanguage — 在分发数据库中，其值为「English」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">DefaultLocale — 在分发数据库中，其值为「en-US」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">CorporateTimeZone — 安装后必须创建，其值为 Windows 使用的时区键名之一。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">客户端设置</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ae50d08705d4.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Innovator 客户端使用两个标准 Windows 设置。这些设置在安装 Windows 时自动设定，可由企业 IT 团队控制或由个人 Windows 用户设置。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6d353f51f280.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">区域设置（Regional Settings Locale）— 打开控制面板，选择「时钟、语言和区域」。此设置定义小数、日期和时间的显示方式，Innovator 支持此 Windows 功能。可以选择并自定义区域设置，Innovator 不使用任何货币格式。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">时区（Time Zone）— 点击「日期和时间」以更改日期时间、时区或添加时钟。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">语言（Language）— 点击「添加语言」以向列表中添加其他语言。列表顶部的语言是您的主要语言。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有关管理国际化的更多信息，请查阅发布光盘上的文档。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">CorporateTimeZone，必须在安装后创建，并包含 Windows 使用的时区键名之一的值。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5e885a12910d.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">客户端设置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Innovator 客户端使用两个标准 Windows 设置。这些设置会在安装 Windows 时自动设置，可由企业 IT 团队控制或由个人 Windows 用户设置。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">区域设置语言环境</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开控制面板并选择时钟、语言和区域：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3a00982b3788.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">此设置定义小数、日期和时间的显示方式，Innovator 支持此 Windows 功能。可以选择一种文化，甚至可以根据需要自定义。Innovator 不使用任何货币格式。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">时区</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击日期和时间以更改日期和时间、时区或添加时钟：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3590f4f230f3.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">图6</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">语言</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「添加语言」以向列表添加不同的语言。出现在列表顶部的语言是您的主要语言。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/525734d21905.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">有关管理国际化的更多信息，请参阅发行版 CD 上的文档。</Run></Paragraph>
</FlowDocument>',
   N'关于 Innovator 的国际化和本地化 从 9.0 版本开始，Innovator 支持国际化和本地化，具有以下特性： 英语为默认语言 可以使用语言包创建或添加其他语言 多语言字符串（Multilingual String）数据类型用于以用户选择的语言显示值 语言包根据客户端区域设置显示相应的菜单、标签和值，使用户能够使用已安装的语言导航和使用 Innovator Innovator 客户端会根',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [11/244] About_ItemTypes.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('a5fd97e7b817',
   N'关于项目类型（ItemType）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于项目类型（ItemType）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_ItemTypes.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于项目类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目类型（ItemType）是 Aras Innovator 管理的业务对象。它是从中创建项目的模板或定义。用面向对象编程的术语来说，ItemType 类似于类定义，从中创建的项目就是类的实例。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Aras Innovator 中几乎一切都通过 ItemType 定义。ItemType 定义：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目的属性、表单或视图。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">与项目关联的生命周期。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">与项目关联的工作流。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">权限、关系、服务器和客户端方法以及要在项目上运行的事件等。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType 可以设计为仅包含名称等简单信息，也可以包含最复杂业务对象所需的全部复杂性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建 ItemType 时，您将看到一组头部属性和一组选项卡。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要了解头部属性，请从「创建 ItemType」开始。点击以下链接了解各选项卡的描述：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/54016a4e1ec1.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">● 属性（Properties）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">● 关系类型（RelationshipTypes）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">● 视图（Views）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">● 服务器事件（Server Events）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">● 操作（Actions）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">● 工作流（Workflows）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">● 目录访问（TOC Access）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">● 客户端事件（Client Events）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">● 可添加（Can Add）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">● 权限（Permissions）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">● 报表（Reports）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType 报表</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有两个有用的报表可以汇总 ItemType 定义。您可以在主网格中选中 ItemType 时从目录中选择「报表」，或在 ItemType 窗口中点击「报表」选项卡来访问它们。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">● ItemType 定义报表 — 汇总表单内容、所有非核心属性以及所有其他关系选项卡的内容。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">● ItemType 权限报表 — 汇总所有权限，显示谁可以获取、更新、删除和更改权限。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType 历史配置报表 — 汇总默认历史模板、所有跟踪属性和生命周期。如果未分配历史模板，将在弹出窗口中显示「此 ItemType 未定义历史模板」的消息。从菜单中选择报表，使用标准打印功能打印报表。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">● ItemType 权限报告汇总了所有权限，显示谁可以获取、更新、删除和更改权限。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType 历史配置报告汇总了默认历史模板、跟踪属性和生命周期。如果未分配历史模板，独立窗口中将显示「此 ItemType 未定义历史模板」消息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从菜单中选择一个报告。使用标准 Internet Explorer 打印功能打印报告。</Run></Paragraph>
</FlowDocument>',
   N'关于项目类型 项目类型（ItemType）是 Aras Innovator 管理的业务对象。它是从中创建项目的模板或定义。用面向对象编程的术语来说，ItemType 类似于类定义，从中创建的项目就是类的实例。 Aras Innovator 中几乎一切都通过 ItemType 定义。ItemType 定义： 项目的属性、表单或视图。 与项目关联的生命周期。 与项目关联的工作流。 权限、关系、服务器和',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [12/244] About_Lifecycles.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('998ffd6876ad',
   N'关于生命周期',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于生命周期</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_Lifecycles.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于生命周期</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">生命周期（Life Cycle）是项目实例在其存在期间经过的一系列状态（即阶段或关口）。大多数业务流程定义高级阶段来跟踪对象在生命周期中的进展。以下是一个生命周期示例：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">生命周期由状态（States）和转换（Transitions）组成。状态基本上是一系列操作和步骤，转换是不同状态之间的路径。让我们一起了解这个周期以获得基本理解。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/92ce7be8b244.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">当创建 ItemType Vendor 的实例（例如命名为 PartsVendor1）时，它会立即置于「初始」（Preliminary）状态。通常，为了成为「已批准」（Approved）的供应商，供应商必须满足一些公司特定的标准或经过特定流程。当标准满足后，相关成员可以将供应商提升到下一状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">作为一个非常强大的工具，可以在项目状态变更时激活和停用各种设置。例如，当采购身份的成员为特定零部件选择供应商时，只有处于「首选」（Preferred）状态的供应商才会出现在列表中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">不同的生命周期映射可以分配给不同的类别。请参阅「类别结构」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">不同的生命周期映射可以分配给不同的类。请参阅类结构</Run></Paragraph>
</FlowDocument>',
   N'关于生命周期 生命周期（Life Cycle）是项目实例在其存在期间经过的一系列状态（即阶段或关口）。大多数业务流程定义高级阶段来跟踪对象在生命周期中的进展。以下是一个生命周期示例： 生命周期由状态（States）和转换（Transitions）组成。状态基本上是一系列操作和步骤，转换是不同状态之间的路径。让我们一起了解这个周期以获得基本理解。 当创建 ItemType Vendor 的实例（例如',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [13/244] About_Methods.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('20a51d6825dc',
   N'关于方法（Method）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于方法（Method）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_Methods.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于方法</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">方法（Method）是执行业务逻辑的脚本，可以通过多种方式调用。方法项目具有名称、代码和类型属性。类型属性定义了代码编写的语言。操作（Action）是终端用户接口层，用于调用方法中的逻辑。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">开发者使用方法 ItemType 将业务逻辑存储在 Innovator 数据库中。方法可以在客户端或服务器端使用 Innovator API 实现。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">方法可用于客户端和服务器端调用，有以下几种方式：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">方法的一种用途是在客户端或服务器对项目执行某些操作之前和之后实现业务逻辑。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">例如，onBeforeAdd 服务器事件可以在服务器执行「add」操作之前调用方法，使用户能够在项目添加到数据库之前对其进行处理。这为应用程序程序员提供了在服务器操作项目之前与项目交互的手段。</Run></Paragraph>
</FlowDocument>',
   N'关于方法 方法（Method）是执行业务逻辑的脚本，可以通过多种方式调用。方法项目具有名称、代码和类型属性。类型属性定义了代码编写的语言。操作（Action）是终端用户接口层，用于调用方法中的逻辑。 开发者使用方法 ItemType 将业务逻辑存储在 Innovator 数据库中。方法可以在客户端或服务器端使用 Innovator API 实现。 方法可用于客户端和服务器端调用，有以下几种方式： ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [14/244] About_relationships.htm (4 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('9c211fa55c53',
   N'关于关系',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于关系</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_relationships.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于关系</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Aras Innovator 中一切都是项目。关系（Relationship）将两个项目绑定在一起——源项目和关联项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系有一个源项目和一个可选的关联项目。当未指定关联项目时，该关系称为空关系（Null Relationship）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系项目包含以下标准属性：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系的源（父）项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系的关联（子）项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例：假设有一个门（Door）项目。门可能需要其他项目，如铆钉（Rivet）。根据门的类型，每扇门所需的铆钉数量不同。一扇大门可能需要 432 个铆钉，而一扇小门可能需要 382 个铆钉。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当您绑定门项目和铆钉项目时，关系项目存储有关这两个项目之间连接的信息（如数量）。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/74d38055ac18.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">当数据库中的两个项目实例建立关系时，还会创建一个关系项目实例，该实例可以为该特定关系保存不同的属性值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">相关主题：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">父子关系</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建关系</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">上下文菜单（关联项目）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">复制和粘贴项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Innovator 有三种典型的关系类型模式，但也有其他模式，如关系的关系。以下图表展示了典型的关系模式：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">直接关系类型（Direct Relationship Type）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图 1 展示了最常见的关系类型，其中源/父项目（Part）通过关系（Part Document）连接到独立的关联/子项目（Document）。直接关系类型是最常用的模式，最好描述为具有源 ItemType 和关联 ItemType。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">循环关系类型（Circular Relationship Type）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">循环关系类型只是引用自身，如标准的 Innovator Part ItemType。图 2 展示了循环关系，其中源/父项目（Part）通过关系（Part BOM）连接到与源项目相同 ItemType 的关联/子项目。在物料清单（BOM）层级中，零部件通过 Part BOM 关系类型连接到其他零部件。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/558569353b90.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">空关系类型（Null Relationship Type）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图 3 展示了「空」关系，即没有关联项目。当需要跟踪仅与源项目相关且不会被重用的项目（值、评论等）时，此模式很有用。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">空关系通常只有可见属性，因为它们不引用任何关联 ItemType。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">空关系在需要为用户提供向项目添加多条相同信息的能力，但不需要或没有另一个 ItemType 来建立连接时很有用。在上面的示例中，空关系用于定义用户向项目添加多个地址的能力。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/cc3352e26ee3.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">图2</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">空关系类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图3显示了一个「空」关系，其中没有关联项目。此模式适用于需要跟踪仅属于源项目且不会被重用的项目（值、评论等）的情况。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b5f2e382f89c.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">图3</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">空关系通常只有可见属性，因为它们不引用关联的 ItemType。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">NULL relationships are useful when you need to provide the ability for a user to add more than one piece of the same information to an Item, but do not need or don’t have another ItemType to make a connection. In the example above, a NULL relationship is used to define the ability for a User to add multiple remarks to an Item. A &apos;remarks&apos; String property can be assigned to the Relationship Item. Each time a User defines a new remark an additional Relationship Item is created. No additional related item is necessary.</Run></Paragraph>
</FlowDocument>',
   N'关于关系 Aras Innovator 中一切都是项目。关系（Relationship）将两个项目绑定在一起——源项目和关联项目。 关系有一个源项目和一个可选的关联项目。当未指定关联项目时，该关系称为空关系（Null Relationship）。 关系项目包含以下标准属性： 关系的源（父）项目 关系的关联（子）项目 示例：假设有一个门（Door）项目。门可能需要其他项目，如铆钉（Rivet）。根据',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [15/244] About_RelationshipTypes.htm (4 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('f213ee50b44c',
   N'关于关系（续）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于关系（续）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_RelationshipTypes.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于关系</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系（Relationship）实现了 ItemType 之间的数据绑定。它们定义了源 ItemType 和关联 ItemType。这个概念可以这样表示：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType1 和 ItemType2 通过 Relationship1 连接。每个关系由两个独立的项目组成：关系类型（RelationshipType）和关系 ItemType。关系 ItemType 可以与其他项目或关系建立其他关系。这种表示方式提供了最大的灵活性。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5e23f2370c67.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">源 ItemType 可以访问关联 ItemType 的所有属性。例如，如果定义了一个名为 Documents 的关系类型，源为 Part ItemType，关联为 Document ItemType，那么特定的 Part 实例就可以访问关联 Document 实例的所有属性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">源 ItemType 可以定义多个关系。每个定义的关系在项目实例中都有自己的选项卡。例如，Part ItemType 定义了 9 个关系类型。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">作为此 ItemType 定义的结果，Part 实例表单为每个定义的关系都有一个选项卡。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开源项目实例的关系选项卡后，您可以根据关系定义选取或创建关联项目。所有关联项目将列在相应关系选项卡下的关系网格中。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/54016a4e1ec1.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">以下主题描述了如何创建关系、设置其属性以及定义其行为。步骤如下：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建源 ItemType 和关联 ItemType</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/90a433603558.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">从源 ItemType 创建并命名关系</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">修改关系类型项目 — 修改关系本身的行为</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/cffe1f10e77c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">修改关系 ItemType — 为关系本身添加和编辑属性（如数量）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建源 ItemType 和关联 ItemType</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从源 ItemType 创建并命名关系。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">修改关系类型项目 - 修改关系本身的行为</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">修改关系 ItemType - 添加和编辑关系本身的属性（例如数量）。</Run></Paragraph>
</FlowDocument>',
   N'关于关系 关系（Relationship）实现了 ItemType 之间的数据绑定。它们定义了源 ItemType 和关联 ItemType。这个概念可以这样表示： ItemType1 和 ItemType2 通过 Relationship1 连接。每个关系由两个独立的项目组成：关系类型（RelationshipType）和关系 ItemType。关系 ItemType 可以与其他项目或关系建立其',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [16/244] About_Revisions.htm (4 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('f78c94f7c26f',
   N'关于修订',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于修订</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_Revisions.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于修订</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">修订（Revisions）是一个符号列表或系列，用于表示可版本化项目的主要修订。所有可版本化项目会自动获得一个世代号（generation number）。每次对项目进行更改时（包括锁定项目、更新项目和提升项目状态），该数字会自动递增。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">这是一个名为 Test 的 ItemType 定义。注意此 ItemType 定义的修订名为 PartsRevision。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">PartsRevision 修订如下所示：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/13788cbdd1ec.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">注意修订只有两个属性——名称和实际的修订列表。创建此列表时，选择能表示递进关系且易于识别先后顺序的名称。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">这是 Test 的一个实例，名为 TestPart。注意其初始修订为 PartA。同时注意此项目处于已发布（Released）生命周期状态，这意味着下次更新并保存项目时，它不仅会有新的世代号，还会有新的主要修订号。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/213c89232852.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">让我们再次编辑并保存项目。结果如下：注意修订系列中的下一个元素被用于递增主要修订号。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">这是 Test 的一个实例，名为 TestPart。注意其初始修订为 PartA。还要注意此项目处于已发布生命周期状态。这意味着下次我们更新并保存项目时，它不仅应该有一个新的世代号，还应该有一个新的主修订号。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/8410dd7c2e0f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">让我们再次编辑并保存项目。以下是结果。注意修订序列中的下一个元素用于递增主修订号。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/2ea2555278f7.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'关于修订 修订（Revisions）是一个符号列表或系列，用于表示可版本化项目的主要修订。所有可版本化项目会自动获得一个世代号（generation number）。每次对项目进行更改时（包括锁定项目、更新项目和提升项目状态），该数字会自动递增。 这是一个名为 Test 的 ItemType 定义。注意此 ItemType 定义的修订名为 PartsRevision。 PartsRevision ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [17/244] About_Sequences.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('6a203d59e71e',
   N'关于序列',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于序列</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_Sequences.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于序列</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">为了实现自动编号，Aras Innovator 为管理员提供了一个名为序列（Sequences）的功能。管理员可以定义序列并将其应用为 ItemType 的属性。序列将遵循定义的模式并自动递增。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有关创建或修改序列的说明，请点击此处。</Run></Paragraph>
</FlowDocument>',
   N'关于序列 为了实现自动编号，Aras Innovator 为管理员提供了一个名为序列（Sequences）的功能。管理员可以定义序列并将其应用为 ItemType 的属性。序列将遵循定义的模式并自动递增。 有关创建或修改序列的说明，请点击此处。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [18/244] About_TOC_Access.htm (4 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('c54da916e491',
   N'关于目录访问',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于目录访问</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_TOC_Access.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于目录访问</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">目录（TOC）访问控制主树形视图中 ItemType 的访问权限。要访问目录编辑器，请转到 管理 &gt; 配置 &gt; 目录编辑器。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">目录编辑器允许管理员创建新的目录访问条目，以及编辑和删除现有条目。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/88a004005535.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">创建新目录条目时，需要填写以下信息：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType — 指定应出现在目录中的 ItemType。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/69b8e3280bba.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">图标（Icon）— 选择应在目录条目旁显示的图片。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标签（Label）— 指定目录条目旁显示的标签。此字段支持多语言条目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">访问（Access）— 指定应看到此目录条目的身份。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：可以为同一 ItemType 授予多个访问条目，并在每个条目中指定不同的图标、标签和身份。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以下按钮用于控制目录条目：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">目录编辑器按钮说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">按钮</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6ba040956298.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">目录编辑器支持拖放操作来移动条目，以及缩进和取消缩进类别。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要预览特定用户组的目录外观，请点击「View As」按钮并输入一个身份（如 All Employees）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">描述</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">目录编辑器支持拖放操作来移动条目，以及缩进和取消缩进类别。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要预览特定组的目录外观，请点击「以...查看」按钮并输入身份，例如所有员工。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6b93b2e62c2f.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'关于目录访问 目录（TOC）访问控制主树形视图中 ItemType 的访问权限。要访问目录编辑器，请转到 管理 > 配置 > 目录编辑器。 目录编辑器允许管理员创建新的目录访问条目，以及编辑和删除现有条目。 创建新目录条目时，需要填写以下信息： ItemType — 指定应出现在目录中的 ItemType。 图标（Icon）— 选择应在目录条目旁显示的图片。 标签（Label）— 指定目录条目旁显',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [19/244] About_Variables.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('a169ca8aa4b2',
   N'关于变量',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于变量</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_Variables.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于变量</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">变量（Variable）项目用于在数据库中存储系统级变量。这些变量用于控制系统中的某些全局逻辑和行为。管理员可以定义额外的变量用于自己的业务逻辑。</Run></Paragraph>
</FlowDocument>',
   N'关于变量 变量（Variable）项目用于在数据库中存储系统级变量。这些变量用于控制系统中的某些全局逻辑和行为。管理员可以定义额外的变量用于自己的业务逻辑。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [20/244] About_Vaults.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('43140e972495',
   N'关于保险柜',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于保险柜</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_Vaults.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于保险柜</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Innovator 架构支持保险柜（Vault）功能，用于安全存储文件。Innovator 使用其数据库管理保险柜中存储文件的元数据。保险柜没有自己的数据库或登录凭据。物理文件（如文档、图纸或 CAD 对象）由保险柜管理。</Run></Paragraph>
</FlowDocument>',
   N'关于保险柜 Innovator 架构支持保险柜（Vault）功能，用于安全存储文件。Innovator 使用其数据库管理保险柜中存储文件的元数据。保险柜没有自己的数据库或登录凭据。物理文件（如文档、图纸或 CAD 对象）由保险柜管理。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [21/244] About_Versions.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('e2b652faeee9',
   N'关于版本',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于版本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_Versions.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于版本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目的业务规则行为由其 ItemType 定义的默认属性和存储在项目实例上的生命周期状态属性建立。当项目实例在业务生命周期的状态之间移动时，可能会经历对其元数据或修订的更改（版本化）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">版本化可以作为自动或手动流程启动。选择自动流程时，每次项目更改（即认领、编辑和取消认领）时世代号（Generation）会递增。项目的生命周期会自动重置到起始状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">附加逻辑已纳入以控制主要修订号（Major_Rev）的递增。当项目实例的生命周期状态为发布状态时，对该项目的任何后续更改都会导致新的 Major_Rev，同时递增 Generation 并将生命周期重置到起始状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Major_Rev（主要修订）— 版本字段，主要递增量，由业务规则控制，在项目处于发布状态时进行更改时版本号递增。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Minor_Rev（次要修订）— 修订字段，次要递增量，由业务规则控制，其行为可在应用程序级别和通过定制来定义。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Generation（世代号）— 整数字段，是计算更改次数的序列号。每当项目被锁定和保存时，世代号由 Innovator 代码流程和规则自动递增。当版本化设置为自动时，每次项目被取消认领时都会创建新的世代号。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Number（编号）— 关键字段（如零部件编号、文档编号），由业务规则控制。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ID — 内部唯一实例标识符（终端用户不可见）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Lifecycle（生命周期）— 用户定义的状态序列和状态之间的转换，用作业务规则的主要驱动器。所有项目都被定义为处于一个特定生命周期中的一个状态。状态名称本身没有意义；每个状态定义上的标志决定行为。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">生命周期起始状态 — 生命周期中被特别指定为起始点的状态。创建新项目时，其状态自动设置为起始状态。版本化时，每当版本（即世代号）更改，项目将被重置回生命周期起始状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">生命周期发布状态 — 生命周期中被特别标记为发布状态的状态。一个生命周期中可以有多个发布状态。当处于发布状态的项目被版本化时，其 Major_Rev 递增，被重置回起始状态，世代号递增。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Promote（提升）— 通过分配同一生命周期中通过转换连接的另一个状态来更改项目状态的操作。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Claim（认领）、Edit（编辑）、Done（完成）和 Unclaim（取消认领）— 一组用于启动和保存项目更改的操作，在选择自动版本化时会导致版本化。这些操作影响可版本化项目上的 Generation 值。如果项目在发布后被编辑，这些操作也会更改 Major_Rev 属性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在需要对自动版本化的项目进行多次编辑的情况下，用户可以认领项目以防止多次版本被应用。项目被认领期间，Generation 只会递增一次，直到用户决定取消认领。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">生命周期</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">客户定义的状态序列和状态之间的转换，用作业务规则的主要驱动因素。所有项目都定义在一个指定生命周期中的一个状态中。状态名称不重要；每个状态定义上的标志决定行为。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">生命周期起始状态</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">生命周期中被指定为起始点的状态。创建新项目时，其状态自动设置为起始状态。进行版本控制时，每当项目的版本（即世代号）更改时，项目将被重置为其生命周期起始状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">生命周期发布状态</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">生命周期中被标记为发布状态的状态。一个生命周期中可以有多个发布状态。当处于发布状态的项目被版本化时，其主修订号递增，重置为生命周期起始状态，世代号递增。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">提升</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">通过分配同一生命周期中通过转换连接的另一个状态来更改项目状态的操作。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">认领、编辑、完成和取消认领</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用于启动和保存项目更改的一组操作，当选择自动版本控制时会导致版本化。这些操作影响可版本化项目上的世代号值。如果项目在发布后被编辑，这些操作还会更改主修订号属性。次修订号属性的行为由每个组织实现的自定义逻辑控制。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">对于需要多次编辑的自动版本化项目，用户可以认领项目以防止应用多个版本。项目被认领期间，世代号只会递增一次，直到用户决定取消认领。</Run></Paragraph>
</FlowDocument>',
   N'关于版本 项目的业务规则行为由其 ItemType 定义的默认属性和存储在项目实例上的生命周期状态属性建立。当项目实例在业务生命周期的状态之间移动时，可能会经历对其元数据或修订的更改（版本化）。 版本化可以作为自动或手动流程启动。选择自动流程时，每次项目更改（即认领、编辑和取消认领）时世代号（Generation）会递增。项目的生命周期会自动重置到起始状态。 附加逻辑已纳入以控制主要修订号（Maj',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [22/244] About_Viewers.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('1bfdc8cb603a',
   N'关于查看器',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于查看器</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_Viewers.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于查看器</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">查看器（Viewer）定义了在 Web 浏览器中启动以查看 Innovator 保险柜中存储文件的应用程序。查看器 URL 链接到一个包含查看器启动说明的 HTML 文件。查看器可以在 View With 关系选项卡下关联到 FileType 项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当打开文件进行查看时，查看器被启动并将文件 URL 作为输入传递。当没有查看器与 FileType 关联时，Web 浏览器将尝试使用 MIME 类型或插件控制的关联来打开文件。</Run></Paragraph>
</FlowDocument>',
   N'关于查看器 查看器（Viewer）定义了在 Web 浏览器中启动以查看 Innovator 保险柜中存储文件的应用程序。查看器 URL 链接到一个包含查看器启动说明的 HTML 文件。查看器可以在 View With 关系选项卡下关联到 FileType 项目。 当打开文件进行查看时，查看器被启动并将文件 URL 作为输入传递。当没有查看器与 FileType 关联时，Web 浏览器将尝试使用 M',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [23/244] About_View_With.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('cb68fd25c4af',
   N'关于 View With',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于 View With</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_View_With.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于 View With</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">View With ItemType 是用于将文件类型与被调用的查看器关联的核心关系类型。当指定了 View With 关系时，将使用指定的应用程序代替 FileType 项目中指定的 MIME 类型。如果未指定 View With 关系，将使用默认的 MIME 类型关联。</Run></Paragraph>
</FlowDocument>',
   N'关于 View With View With ItemType 是用于将文件类型与被调用的查看器关联的核心关系类型。当指定了 View With 关系时，将使用指定的应用程序代替 FileType 项目中指定的 MIME 类型。如果未指定 View With 关系，将使用默认的 MIME 类型关联。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [24/244] About_Workflows.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('8c19764cea60',
   N'关于工作流',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于工作流</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: About_Workflows.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于工作流</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工作流（Workflow）是表示业务流程的已定义活动序列，例如 ECN 审批流程。工作流通常包含分支和并行活动，并嵌入程序规则。在 Innovator 中，工作流映射（Workflow Map）表示业务流程的模板，可以附加到一个或多个 ItemType。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">让我们查看一个工作流映射并讨论其不同组成部分。以下截图显示了一个分为 3 部分的表单：顶部是图形化的工作流设计器（即图表），其中图表表示活动模板（Activity Templates）和连接它们的工作流映射路径。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如前所述，工作流映射由活动（Activities）和路径（Paths）组成。每个活动代表必须执行的一个工作单元，包含任务列表、分配给负责这些任务的身份的通知，以及任何变量（如花费的小时数）。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0b7bdee26e1f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">当活动被激活时，被分配的身份（即受让人）会收到通知，活动出现在他们的收件箱（InBasket）中。然后受让人打开所谓的活动完成工作表（Activity Completion Worksheet），该工作表引导他们完成待办任务列表。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当活动被激活时，分配给它的身份（即受让人）会收到通知，活动出现在其收件箱中。受让人随后打开活动完成工作表，该工作表引导他们完成任务列表以及在活动完成之前需要执行的任何其他工作。此外，受让人将有机会选择活动的退出路径或投票。根据此投票结果，将选择下一个要激活的活动。</Run></Paragraph>
</FlowDocument>',
   N'关于工作流 工作流（Workflow）是表示业务流程的已定义活动序列，例如 ECN 审批流程。工作流通常包含分支和并行活动，并嵌入程序规则。在 Innovator 中，工作流映射（Workflow Map）表示业务流程的模板，可以附加到一个或多个 ItemType。 让我们查看一个工作流映射并讨论其不同组成部分。以下截图显示了一个分为 3 部分的表单：顶部是图形化的工作流设计器（即图表），其中图表',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [25/244] Actions_menu.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('78543350ca3b',
   N'操作菜单',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">操作菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Actions_menu.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作菜单（Actions Menu）是基于 Action ItemType 的动态菜单。操作菜单中显示的菜单选项取决于登录用户的访问权限。访问权限由管理员配置。</Run></Paragraph>
</FlowDocument>',
   N'操作菜单 操作菜单（Actions Menu）是基于 Action ItemType 的动态菜单。操作菜单中显示的菜单选项取决于登录用户的访问权限。访问权限由管理员配置。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [26/244] Actions_Menu_(Item_Window).htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('356e0a2d90b6',
   N'操作菜单（项目窗口）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">操作菜单（项目窗口）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Actions_Menu_(Item_Window).htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作菜单（项目窗口）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作菜单是基于 Action ItemType 的动态菜单。操作菜单中显示的菜单选项取决于登录用户的访问权限。访问权限由管理员配置。</Run></Paragraph>
</FlowDocument>',
   N'操作菜单（项目窗口） 操作菜单是基于 Action ItemType 的动态菜单。操作菜单中显示的菜单选项取决于登录用户的访问权限。访问权限由管理员配置。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [27/244] Action_Properties.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('ff37f773cc79',
   N'操作属性',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">操作属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Action_Properties.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">必填</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Name（名称）— 操作的名称，必填。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Label（标签）— 在操作菜单中显示的标签，多语言字符串，必填。请参阅「国际化」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Type（类型）— 决定操作出现在哪些菜单中，必填。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Location（位置）— 指定操作在哪里运行，必填。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Method（方法）— 操作调用的已编写代码，必填。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Target（目标）— 操作运行的位置，必填。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">On Complete（完成后）— 操作完成后运行的方法，选填。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Body（主体）— 用于指定 AML 的区域，选填。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">确定操作在菜单中的显示位置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">位置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Y</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">指定操作的执行位置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">方法</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Y</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作调用的代码</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">目标</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Y</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作的执行位置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">完成时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">N</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作完成后执行的方法</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">正文</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">N</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用于指定 AML 的位置</Run></Paragraph>
</FlowDocument>',
   N'操作属性 属性名称 必填 说明 Name（名称）— 操作的名称，必填。 Label（标签）— 在操作菜单中显示的标签，多语言字符串，必填。请参阅「国际化」。 Type（类型）— 决定操作出现在哪些菜单中，必填。 Location（位置）— 指定操作在哪里运行，必填。 Method（方法）— 操作调用的已编写代码，必填。 Target（目标）— 操作运行的位置，必填。 On Complete（完成后',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [28/244] Adding_Additional_Fields_to_a_Form_Layout.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('1ed544b04c46',
   N'向表单布局添加字段',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">向表单布局添加字段</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Adding_Additional_Fields_to_a_Form_Layout.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">向表单布局添加字段</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从表单布局工具栏点击下拉菜单，访问未使用属性列表。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择要添加到表单布局的属性。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c000b91cf621.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击并将属性拖动到布局框架中的所需位置。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">根据需要编辑字段属性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">根据需要编辑字段属性。</Run></Paragraph>
</FlowDocument>',
   N'向表单布局添加字段 从表单布局工具栏点击下拉菜单，访问未使用属性列表。 选择要添加到表单布局的属性。 点击并将属性拖动到布局框架中的所需位置。 根据需要编辑字段属性。 Edit field properties, as needed.',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [29/244] Adding_a_Viewer.htm (5 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('7e35dcce2254',
   N'添加查看器',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">添加查看器</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Adding_a_Viewer.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">添加查看器</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在目录中展开「管理」文件夹。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">展开「文件处理」并选择「查看器」。查看器窗口将出现。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「新建查看器」。将出现以下窗口。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c229ad697e6d.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">按查看器属性主题中的说明填写字段。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击保存并解锁项目。新操作现在将显示在主窗口的默认页面中。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7094e27e55e9.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">按照查看器属性主题中的说明填写字段。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击保存并解锁项目。新操作现在显示在主窗口的默认页面中。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ba507963f59.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7808f6238b92.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'添加查看器 在目录中展开「管理」文件夹。 展开「文件处理」并选择「查看器」。查看器窗口将出现。 点击「新建查看器」。将出现以下窗口。 按查看器属性主题中的说明填写字段。 点击保存并解锁项目。新操作现在将显示在主窗口的默认页面中。 Populate fields as indicated in the Viewer Properties topic. Click and to save and un',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [30/244] Adding_Elements_to_a_Form_Layout.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('5e15fb61672a',
   N'向表单布局添加元素',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">向表单布局添加元素</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Adding_Elements_to_a_Form_Layout.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">向表单布局添加元素</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户通过称为字段的命名元素与表单交互。字段显式绑定到通过表单的 ItemType 引用提供的属性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">这些字段可以在表单布局区域中进行逻辑分组和排列，支持自定义位置、控件选择（如下拉框/组合框、文本区域）、标签、字体等。控件的初始值由绑定到控件的属性的「默认值」决定。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择表单后，Innovator 工作区显示表单编辑工具。水平选项卡允许访问和修改表单属性。下方框架显示表单布局。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当新 ItemType 首次保存时，会自动创建新的表单模板。默认情况下，ItemType 属性显示在表单布局中。当向 ItemType 添加额外属性时，必须手动将相关字段添加到 ItemType 表单中。</Run></Paragraph>
</FlowDocument>',
   N'向表单布局添加元素 用户通过称为字段的命名元素与表单交互。字段显式绑定到通过表单的 ItemType 引用提供的属性。 这些字段可以在表单布局区域中进行逻辑分组和排列，支持自定义位置、控件选择（如下拉框/组合框、文本区域）、标签、字体等。控件的初始值由绑定到控件的属性的「默认值」决定。 选择表单后，Innovator 工作区显示表单编辑工具。水平选项卡允许访问和修改表单属性。下方框架显示表单布局。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [31/244] Adding_Items_to_Bookmarks.htm (2 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('8832c4e63526',
   N'将项目添加到书签',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">将项目添加到书签</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Adding_Items_to_Bookmarks.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将项目添加到书签</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">任何启用了安全社交功能的项目都可以添加到书签中的两个位置：书签组中的项目或论坛。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要将项目添加到书签，请执行以下操作：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">右键点击要添加的启用了 SSVC 的项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在上下文菜单中点击「添加项目到书签」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">这将打开「添加项目到书签」对话框。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6459c5d1495a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">选择以下选项之一：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目书签（Item Bookmarks）— 将所选项目添加到您的书签列表。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/33c5b0d3ce06.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">创建新论坛（Create New Forum）— 选择此选项访问创建新论坛对话框。创建新论坛时，所选项目将被放置在其中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">书签（Bookmarks）— 选择此选项将项目添加到书签中的项目组。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">第三个选项是用户拥有的现有论坛。选择此选项将把所选项目添加到该论坛。用户无法将项目添加到不属于自己的论坛中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「确定」保存更改，或点击「取消」退出对话框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">书签 - 选择此选项将把项目添加到书签中的项目组。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">第三个选项是用户拥有的现有论坛。选择此选项会将选定项目添加到该论坛中。用户无法将项目添加到他们不拥有的论坛中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击确定保存更改，或点击取消退出对话框。</Run></Paragraph>
</FlowDocument>',
   N'将项目添加到书签 任何启用了安全社交功能的项目都可以添加到书签中的两个位置：书签组中的项目或论坛。 要将项目添加到书签，请执行以下操作： 右键点击要添加的启用了 SSVC 的项目。 在上下文菜单中点击「添加项目到书签」。 这将打开「添加项目到书签」对话框。 选择以下选项之一： 项目书签（Item Bookmarks）— 将所选项目添加到您的书签列表。 创建新论坛（Create New Forum）',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [32/244] Adding_saved_searches_to_Forums.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('c5411759a707',
   N'将保存的搜索添加到论坛',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">将保存的搜索添加到论坛</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Adding_saved_searches_to_Forums.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将保存的搜索添加到论坛</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存的搜索（Saved Searches）可以添加到论坛中以驱动动态内容。任何针对启用了 SSVC 的项目的保存搜索都可以添加到书签中的论坛。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：此功能仅限管理员创建的保存搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用以下步骤：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1. 右键点击保存搜索的上下文菜单，点击「添加到论坛」。将出现「添加搜索到论坛」对话框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2. 选择以下选项之一：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">a. 创建新论坛 — 选择此选项打开创建新论坛对话框。创建论坛时，指定的项目将被添加到其中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">b. 选择您拥有的现有论坛，将指定的保存搜索添加到该论坛。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：您无法将搜索添加到他人拥有的论坛。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">3. 点击「保存并完成」。与论坛关联的保存搜索将出现在书签选项卡中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用相同的过程将文档添加到论坛。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/dc0615af58ef.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">使用相同的过程将文档添加到论坛。</Run></Paragraph>
</FlowDocument>',
   N'将保存的搜索添加到论坛 保存的搜索（Saved Searches）可以添加到论坛中以驱动动态内容。任何针对启用了 SSVC 的项目的保存搜索都可以添加到书签中的论坛。 注意：此功能仅限管理员创建的保存搜索。 使用以下步骤： 1. 右键点击保存搜索的上下文菜单，点击「添加到论坛」。将出现「添加搜索到论坛」对话框。 2. 选择以下选项之一： a. 创建新论坛 — 选择此选项打开创建新论坛对话框。创建论',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [33/244] advancedsearch.htm (3 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('0f8f95ada6dd',
   N'高级搜索',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">高级搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: advancedsearch.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高级搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高级搜索模式是一个向导，允许您使用简单搜索中无法使用的属性和搜索条件执行搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高级搜索允许您：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在关系以及源项目上搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在主网格中未显示的属性上搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用「大于」或「不等于」等运算符代替「等于」加通配符</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高级搜索栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高级搜索有一个额外的按钮。将鼠标悬停在按钮上会显示「添加条件」的工具提示。点击此按钮为高级搜索输入额外的搜索条件。右键点击并从上下文菜单中选择「删除条件」可删除搜索条件。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a2bd53d3bfd7.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">列说明：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/32ce6b8a0f4a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">ItemType — 在此列中选择源 ItemType 或关系。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Property — 选择用作搜索条件的属性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Operation — 选择适合该属性的运算符。以下是运算符及其功能：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">=（等于）— 搜索属性值与指定条件相同的项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">≠（不等于）— 搜索属性值与指定条件不同的所有项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">&lt;（小于）— 搜索属性值小于指定条件的项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">&lt;=（小于等于）— 搜索属性值小于或等于指定条件的项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">&gt;（大于）— 搜索属性值大于指定条件的项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">&gt;=（大于等于）— 搜索属性值大于或等于指定条件的项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">like — 允许在搜索条件中使用通配符「%」和「*」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">not like — 反转搜索条件，允许使用通配符。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">null — 搜索数据库中明确具有 NULL 值的属性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">not null — 搜索已设置值的任何属性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Criteria（条件）— 显示所选属性的适当格式：布尔属性显示复选框，列表属性显示下拉框，日期属性显示日期选择器，其他属性显示文本框（在指定 like 运算符时允许通配符搜索）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">行（Rows）— 每行是搜索的一个 AND 条件。高级搜索过滤器中可以有多行。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高级搜索示例</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例 1 — ItemType: Part, Property: Description, Operation: not null — 显示包含描述的 Part 项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例 2 — 添加条件：created_on &gt; 02/02/2014 — 显示包含描述且在指定日期之后创建的 Part 项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例 3 — 条件：created_on &gt; 1/1/2014 AND modified_on &lt; 05/14/2014 — 显示在 2014年1月1日之后创建且在 2014年5月14日之前修改的 Part 项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建高级搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击搜索工具栏中的「添加条件」按钮添加所有要使用的条件。使用右键菜单中的「删除条件」删除不需要的条件。对每个条件行，选择：ItemType、属性、运算符和条件值。点击搜索工具栏中的「搜索」按钮。根据需要使用「停止搜索」和「清除搜索」按钮。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您搜索属性值大于搜索条件中指定值的项目。示例：使用成本 &gt; 3000 返回成本大于3000的项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">大于或等于</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">大于或等于</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您搜索属性值大于或等于搜索条件中指定值的项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">包含</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Enables you to use wildcards &apos;%&apos; and &apos;*&quot; in the search condition. Example: Using Part Number like BR* returns all Items with part numbers beginning with BR.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">不包含</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Inverts the search condition you specify and allows the use of wildcards &apos;%&apos; and &apos;*&quot; in the search condition. Example: Using Part Number not like BR* returns all Items except those whose part numbers begin with BR.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">空值</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Enables you to search for properties that explicitly have a NULL value in the database. Example: Using Type null returns all Items for whom the Type (property has no value) is not specified.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">不为空</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Enables you to search any properties that have a value that has been set. Example: Using Type not null returns all Items for whom Type (property has some value) is specified.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Criteria: displays the appropriate format for the selected Property: Properties of type Boolean (yes/no) display a checkbox. Properties of type List display a dropdown. Properties of type Date display a Date Picker form when you click on the Criteria. Other Properties display a blank text box, in which wild card searches are allowed so long as &apos;like&apos; is specified as the Operator.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">布尔类型（是/否）的属性显示为复选框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">列表类型的属性显示为下拉框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">日期类型的属性在您点击条件时会显示日期选择器表单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">其他属性显示一个空白文本框，只要将操作符指定为「包含」，就允许进行通配符搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">行</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Each row is an AND condition for the search. There can be many rows in the advanced search filter. A scroll bar is available to move up and down the list.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高级搜索示例</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以零部件 ItemType 为例</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例1</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType = 零部件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性 = 描述</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作 = 不为空</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示包含描述的零部件 ItemType。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例2</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType = 零部件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性 = 描述</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作 = 不为空</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">AND</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType = 零部件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性 = 创建时间</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作 = 大于</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">条件 = 2014/02/02</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示包含描述且在指定日期之后创建的零部件 ItemType。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例3</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType = 零部件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性 = 创建时间</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作 = 大于</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">条件 = 2014/01/01</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">AND</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType = 零部件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性 = 修改时间</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作 = 小于</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">条件 = 2014/05/14</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示在2014/1/1之后创建且在2014/5/14之前修改的零部件 ItemType。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建高级搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击搜索工具栏中的「添加条件」按钮以使用所有条件。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/32ce6b8a0f4a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">使用鼠标右键并从上下文菜单中选择「删除条件」来删除不需要的条件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">对于每个条件行，选择：一个 ItemType、一个属性、一个操作、一个条件值</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">一个 ItemType</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">一个属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">一个操作</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">一个条件值</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click the ‘Search’ button in the search toolbar.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Use the ‘Stop Search’ and ‘Clear Search’ buttons in the search toolbar as required.</Run></Paragraph>
</FlowDocument>',
   N'高级搜索 高级搜索模式是一个向导，允许您使用简单搜索中无法使用的属性和搜索条件执行搜索。 高级搜索允许您： 在关系以及源项目上搜索 在主网格中未显示的属性上搜索 使用「大于」或「不等于」等运算符代替「等于」加通配符 高级搜索栏 高级搜索有一个额外的按钮。将鼠标悬停在按钮上会显示「添加条件」的工具提示。点击此按钮为高级搜索输入额外的搜索条件。右键点击并从上下文菜单中选择「删除条件」可删除搜索条件。 ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [34/244] amlsearch.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('630f88ef8ecc',
   N'AML 搜索',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">AML 搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: amlsearch.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">AML 搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">这是一种原始搜索模式，使用 Aras 标记语言（AML）。具有 AML 知识的用户可以使用此搜索模式。它允许所有用户进行复杂搜索，以及使用其他高级用户、管理员或开发者准备并作为保存搜索共享的搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">AML 搜索可以执行其他搜索模式无法完成的搜索，包括：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在关系和关联项目中搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在目录中关系项目的源或父项目中搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">同时在多个层级使用逻辑</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：如果您在简单搜索模式下开始构建搜索，然后切换到 AML 模式，搜索模式中的所有条件将被转换为 AML。这可以节省从头编写搜索条件的时间。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7b6d36291ee2.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">有关 AML 的更多信息，请参阅 Aras 网站文档部分的程序员指南。您也可以参加 Aras 提供的培训。有关培训详情，请参阅 Aras 网站的培训部分。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">For more information on AML, you can refer to the Programmer&apos;s Guide available in the Documentation section of the Aras web-site. You can also attend training provided by Aras. For details on the training, refer to the Training section on the Aras web-site.</Run></Paragraph>
</FlowDocument>',
   N'AML 搜索 这是一种原始搜索模式，使用 Aras 标记语言（AML）。具有 AML 知识的用户可以使用此搜索模式。它允许所有用户进行复杂搜索，以及使用其他高级用户、管理员或开发者准备并作为保存搜索共享的搜索。 AML 搜索可以执行其他搜索模式无法完成的搜索，包括： 在关系和关联项目中搜索 在目录中关系项目的源或父项目中搜索 同时在多个层级使用逻辑 注意：如果您在简单搜索模式下开始构建搜索，然后切',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [35/244] Attaching_a_Workflow_to_an_Item.htm (3 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('57233b6a97bb',
   N'将工作流附加到项目',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">将工作流附加到项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Attaching_a_Workflow_to_an_Item.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将工作流附加到项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">目前，工作流通过在 ItemType 定义的工作流选项卡中指定工作流来附加到项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从导航窗格，选择 管理 → ItemTypes，选择要添加工作流的 ItemType 并打开编辑。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3215685075c3.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">选择「工作流」选项卡。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「选择项目」按钮。工作流搜索对话框将出现。选择要添加的工作流，然后点击「返回所选」按钮。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果希望在创建新项目实例时自动启动此工作流，请勾选「默认」复选框。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/81d30419a164.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ebc292158fb2.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">与项目关联的所有工作流都必须列在此选项卡上。例如，如果默认工作流的某个活动有子流程，它必须列在此处（但不标记为默认）。如果您有希望以编程方式激活的工作流，也必须在此处列出。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">All workflows associated with the item must be listed on this tab. For example, if you have a subflow for one of the Default Workflow&apos;s activities, it must be listed here (but not marked as a Default). If you have a workflow that you wish to activate programmatically, you must also list it here, with the Default box unchecked.</Run></Paragraph>
</FlowDocument>',
   N'将工作流附加到项目 目前，工作流通过在 ItemType 定义的工作流选项卡中指定工作流来附加到项目。 从导航窗格，选择 管理 → ItemTypes，选择要添加工作流的 ItemType 并打开编辑。 选择「工作流」选项卡。 点击「选择项目」按钮。工作流搜索对话框将出现。选择要添加的工作流，然后点击「返回所选」按钮。 如果希望在创建新项目实例时自动启动此工作流，请勾选「默认」复选框。 与项目关联',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [36/244] Automatic_Versions.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('848740d3e366',
   N'自动版本',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">自动版本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Automatic_Versions.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">自动版本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择自动流程时，每次项目更改（即认领、编辑和取消认领）时世代号（Generation）会递增。此外，项目的生命周期会自动重置到起始状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用自动版本化的项目生命周期行为示例（例如零部件 Part）：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/98cd4c3cb397.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'自动版本 选择自动流程时，每次项目更改（即认领、编辑和取消认领）时世代号（Generation）会递增。此外，项目的生命周期会自动重置到起始状态。 使用自动版本化的项目生命周期行为示例（例如零部件 Part）：',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [37/244] Bookmarks.htm (2 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('bc73f3d64ba8',
   N'书签',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">书签</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Bookmarks.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">书签</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">书签（Bookmarks）代表您正在关注的社交内容。所选书签的讨论内容显示在讨论面板中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">默认书签</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6459c5d1495a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">默认书签自动设置并对所有用户可用：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">所有书签（All Bookmarks）— 这是打开「我的讨论」时的默认书签，一次显示所有书签的聚合内容。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">来自我的消息（Messages from me）— 您创建的所有消息（不包括用户间接创建的历史消息）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">这些书签始终按此顺序显示在顶部且不能折叠。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">书签组</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户可以创建/添加的书签分为 4 个书签组：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">人员（People）— 代表指定人员创建的所有消息的书签（不包括用户间接创建的历史消息）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">论坛（Forums）— 代表论坛的书签。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目（Items）— 代表单个项目的书签。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">每个组在书签中都有一个标题，允许显示或折叠该组。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">书签搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索书签字段出现在书签面板顶部。在此字段中输入搜索条件并点击图标。搜索通过在书签名称中搜索输入的字符串来完成。当搜索结果生效时，只有匹配搜索条件的书签会显示在讨论面板中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">书签的上下文菜单功能</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">书签具有以下可用的上下文菜单命令：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/29fbcf614b59.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">设为默认（Set as Default）— 设置用户将来打开「我的讨论」时使用的默认书签（默认为「所有消息」）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">分享（Share）— 更改论坛的共享设置，打开分享论坛对话框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开（Open）— 在弹出窗口中打开项目表单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">移除（Remove）— 如果选中书签则从书签中移除；如果选中论坛内的项目或保存搜索则从论坛中移除。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">上下文菜单选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">描述</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在默认组中</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设为默认</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Set as Default Sets the default Bookmark that will be used when the user opens My Discussions in the future (is set to “All Messages” by default)</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">分享... 更改论坛的共享设置。打开共享论坛对话框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在独立窗口中打开项目的表单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">移除 - 如果选中书签则从书签中移除。如果选中论坛内的项目或保存搜索则从论坛中移除。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息来自</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">移除</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设为默认</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">论坛</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">分享...</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">移除</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设为默认</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">论坛内的项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">移除</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设为默认</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">论坛内的保存搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">移除</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目（在项目组中）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">移除</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设为默认</Run></Paragraph>
</FlowDocument>',
   N'书签 书签（Bookmarks）代表您正在关注的社交内容。所选书签的讨论内容显示在讨论面板中。 默认书签 默认书签自动设置并对所有用户可用： 所有书签（All Bookmarks）— 这是打开「我的讨论」时的默认书签，一次显示所有书签的聚合内容。 来自我的消息（Messages from me）— 您创建的所有消息（不包括用户间接创建的历史消息）。 这些书签始终按此顺序显示在顶部且不能折叠。 书签',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [38/244] Bookmarks_User_Interface.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('bc7abb5394bd',
   N'书签面板',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">书签面板</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Bookmarks_User_Interface.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">书签面板</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">书签面板显示在「我的讨论」主窗口的右侧。书签的基本操作是，如果用户点击任何书签使其激活，关联的消息集将出现在主讨论面板中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要了解更多关于书签面板的信息，请参阅以下主题：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e224707c54ba.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">书签</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将项目添加到书签</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">论坛</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关注</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关注</Run></Paragraph>
</FlowDocument>',
   N'书签面板 书签面板显示在「我的讨论」主窗口的右侧。书签的基本操作是，如果用户点击任何书签使其激活，关联的消息集将出现在主讨论面板中。 要了解更多关于书签面板的信息，请参阅以下主题： 书签 将项目添加到书签 论坛 关注 Follow',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [39/244] Can_Add_Permission.htm (6 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('97114832f503',
   N'可添加权限',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">可添加权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Can_Add_Permission.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可添加权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可添加权限（Can Add Permission）允许特定身份创建或添加 ItemType 的实例。与目录访问权限一样，此权限直接添加到 ItemType 上。没有它，任何人都无法创建或添加实例。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设置可添加权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中，选择 管理 &gt; ItemTypes，将出现以下菜单。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/32c5415833d5.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击「搜索 ItemTypes」搜索要编辑的 ItemType。在此示例中，我们使用名为 Sample 的 ItemType。打开 ItemType 进行编辑并选择「Can Add」选项卡。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ee7dec23fd94.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击「选择项目」按钮可以选取一个现有身份，您将为其添加 ItemType 的实例。如果选择「创建项目」，可以当场创建一个身份并授予其创建实例的访问权限。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/81d30419a164.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/93ef1556cc54.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">选择「选择项目」。身份搜索对话框将出现。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择要授予可添加权限的身份，然后点击绿色勾选图标。Can Add 选项卡上将出现一个新行，包含您刚选择的身份名称。在上面的示例中，我们选择了 World 身份，即所有人。点击保存并关闭 ItemType。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5fdce21163ef.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击保存并关闭 ItemType。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'可添加权限 可添加权限（Can Add Permission）允许特定身份创建或添加 ItemType 的实例。与目录访问权限一样，此权限直接添加到 ItemType 上。没有它，任何人都无法创建或添加实例。 设置可添加权限 在导航窗格中，选择 管理 > ItemTypes，将出现以下菜单。 点击「搜索 ItemTypes」搜索要编辑的 ItemType。在此示例中，我们使用名为 Sample 的',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [40/244] Changing_password.htm (2 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('8f55cb2f59f4',
   N'更改密码',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">更改密码</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Changing_password.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">更改密码</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要访问 Aras Innovator，用户必须在系统中定义凭据（用户名和密码）。您可以使用以下步骤更改密码：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">登录 Aras Innovator。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击窗口右上角的用户菜单按钮。用户菜单将出现。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9ca33070eda9.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">选择 首选项 &gt; 更改密码。更改密码对话框将出现。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7767816692fb.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">在「旧密码」字段中输入当前密码。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在「新密码」字段中输入新密码。在「确认新密码」字段中重新输入新密码进行确认。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「确定」。</Run></Paragraph>
</FlowDocument>',
   N'更改密码 要访问 Aras Innovator，用户必须在系统中定义凭据（用户名和密码）。您可以使用以下步骤更改密码： 登录 Aras Innovator。 点击窗口右上角的用户菜单按钮。用户菜单将出现。 选择 首选项 > 更改密码。更改密码对话框将出现。 在「旧密码」字段中输入当前密码。 在「新密码」字段中输入新密码。在「确认新密码」字段中重新输入新密码进行确认。 点击「确定」。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [41/244] Checking_In.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('6eceb66eb7b8',
   N'签入和签出',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">签入和签出</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Checking_In.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">签入和签出</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">所有文件访问通过内置的文件签入和签出控制进行管理。用户将文件存储在保险柜中后，文档必须从工作区签出才能进行任何更改。工作目录位置由用户设置确定。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要签出文件进行编辑，该文件不能被其他用户锁定。文件签出时会自动锁定，其他用户无法签出或签入该文件。编辑完成后，用户可以签入文件。</Run></Paragraph>
</FlowDocument>',
   N'签入和签出 所有文件访问通过内置的文件签入和签出控制进行管理。用户将文件存储在保险柜中后，文档必须从工作区签出才能进行任何更改。工作目录位置由用户设置确定。 要签出文件进行编辑，该文件不能被其他用户锁定。文件签出时会自动锁定，其他用户无法签出或签入该文件。编辑完成后，用户可以签入文件。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [42/244] creating_a_life_cycle_map.htm (6 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('d3857863d891',
   N'创建生命周期映射',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建生命周期映射</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: creating_a_life_cycle_map.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建生命周期映射</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">生命周期映射（Life Cycle Map）可以独立于任何 ItemType 创建，之后可以附加到 ItemType。实际上，多个 ItemType 可以使用同一个生命周期映射。此外，一个 ItemType 可以包含多个生命周期映射。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建生命周期映射的步骤：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中，选择 管理 &gt; 生命周期映射，将出现以下菜单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「新建生命周期映射」。将出现空白的生命周期映射。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6b3eeef39a14.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">填写生命周期映射属性：名称（Name）— 映射的名称；描述（Description）— 映射的描述。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">通过在映射画布上右键点击并从弹出菜单中选择「添加状态」来创建新状态。新创建的状态默认保持选中。每个生命周期状态具有以下属性：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/92ce7be8b244.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">状态属性说明：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Name（名称）— 状态的名称。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Image（图片）— 点击「选择图片」链接访问图片浏览器对话框，为状态选择图标。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Released（已发布）— 此属性仅在此生命周期映射关联的 ItemType 可版本化时适用。布尔值，表示此状态为已发布状态。处于已发布状态的项目在下次「锁定/解锁/编辑」序列时会被移回起始状态，且主要修订号递增。每个生命周期只能有一个状态标记为已发布。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Not Lockable（不可锁定）— 布尔值，设为 true 时，处于此状态的项目不能被编辑。通常在状态为已发布时设置为 true。要编辑项目，必须将其提升到新状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Item Behavior（项目行为）— 仅适用于通过此生命周期状态的项目包含一个或多个与可版本化项目的关系时。有四种类型：Fixed（固定）— 源项目指向关联项目的指定世代；Float（浮动）— 源项目指向关联项目的最新世代；Hard Fixed（硬固定）— 固定行为且不能被后续状态更改；Hard Float（硬浮动）— 浮动行为且不能被后续状态更改。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">State Permissions（状态权限）— 可以为每个状态定义一组权限，控制项目处于此状态时的访问权限。状态权限会覆盖项目上设置的任何其他权限。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Configure E-Mail（配置电子邮件）— 点击此链接配置电子邮件，以通知指定身份该项目已进入此状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Workflow（工作流）— 点击工作流字段右侧的按钮，从弹出搜索对话框中选择允许的工作流。所选工作流将在生命周期状态激活时启动。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Label（标签）— 定义系统中为此生命周期状态显示的标签。如果配置了多语言支持，此字段允许以多种语言定义标签。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ebc292158fb2.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">History Template（历史模板）— 点击历史模板字段右侧的按钮，从弹出搜索对话框中选择历史模板。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建转换（Transition）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">右键点击要从中创建转换的状态。从弹出菜单中选择「添加转换」。转换从指定状态创建并连接到鼠标。将鼠标拖动到要连接的状态并点击完成连接。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">每个生命周期只能有一个状态标记为已发布。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">不可锁定</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">A Boolean; when set to true, the item in this state cannot be edited. Typically this property is set to true when the status of the state is Released. In order to edit the item, it has to be promoted to a new state, which would typically (depending on the life cycle) create a new version.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目行为</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Applicable only if the item going through this life cycle state contains one or more relationships with versionable items. There are four types of Item Behavior, and they are described below. Be aware that the Behavior set by the RelationshipType influences the Behavior set by the state as well. To see a full explanation of how these behaviors interact together, see Item Behavior.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">固定 - 源项目（在当前状态）指向关联项目的指定世代号。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">浮动 - 源项目（在当前状态）指向关联项目的最新世代号。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">硬固定 - 行为为固定（如上所述），不能被此生命周期中的后续状态更改。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">硬浮动 - 行为为浮动（如上所述），不能被此生命周期中的后续状态更改。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">状态权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">A set of permissions may be defined for each state, which will govern the access to the item while it is in this state (see Permissions). The state permissions override any other permissions set on the Item. Promoting to a state changes the Item permissions to match the state permissions. If no state permissions are set, then the Item permissions are not changed.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">For an example of state permissions, look at the default Part Life Cycle map. While a Part is in the Preliminary state All Employees can get and update it. However, once the Part enters the Review state, only members of the Aras PLM and the Administrators identities can update it.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">配置电子邮件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击此链接配置电子邮件，以便在项目进入此状态时通知指定的身份。请参阅配置电子邮件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工作流</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click the button to the right of the Workflow field. Select an allowed Workflow from the pop-up search dialog. The selected Workflow will be initiated when the Life Cycle State is activated. Note: An allowed Workflow is one that is related to the same ItemType to which the Life Cycle is also related.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f29be4d0d817.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">标签</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">This field defines the label that is displayed within the system for this Life Cycle State. If Aras Innovator is configured for Multilingual support, then this field allows the label to be defined in multiple languages. In order to set a multilingual value for this field, click the button to the right of the Label field. The system will pop-up a dialog for inputting a value in different languages. Refer to the help topics on Internationalization for more information.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f29be4d0d817.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">历史模板</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click the button to the right of the History Template field. Select a History Template from the pop-up search dialog. See the help topic on Configurable History for more information</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f29be4d0d817.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">创建转换</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">右键点击您希望创建转换的状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Select Add Transition from the popup menu. A transition is created from the specified state and connected to the mouse. Drag the mouse to the state which you want to connect, and click to connect the transition.</Run></Paragraph>
</FlowDocument>',
   N'创建生命周期映射 生命周期映射（Life Cycle Map）可以独立于任何 ItemType 创建，之后可以附加到 ItemType。实际上，多个 ItemType 可以使用同一个生命周期映射。此外，一个 ItemType 可以包含多个生命周期映射。 创建生命周期映射的步骤： 在导航窗格中，选择 管理 > 生命周期映射，将出现以下菜单。 点击「新建生命周期映射」。将出现空白的生命周期映射。 填写',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [43/244] class can add.htm (3 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('9144ccb92440',
   N'类别结构的可添加权限',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">类别结构的可添加权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: class can add.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">类别结构的可添加权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">简介</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">类别结构（Class Structure）允许将单个 Aras Innovator ItemType 专门化为类和子类，用于组织属性、启动不同的输入表单，以及通过 Can Add 设置为 ItemType 的实例设置权限。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">例如，名为 Inspection Result 的 ItemType 可以分类为 Aesthetic、Packaging、Tolerance、Product 和 Software 类。使用分类功能，不同的组和身份可以通过不同的方式使用同一个 Inspection Result 项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在本帮助页面中，我们了解如何授予特定组/身份创建（即可添加）Inspection Result 项目的权利。有关使用基于类的属性和表单的详细信息，请参阅相关的类别特定属性和类别特定表单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">定义 ItemType 的分类特定创建权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">为了解释分类特定创建/可添加权限在 Aras Innovator 中的工作方式，我们将使用一个名为 Inspection Result 的理论 ItemType。假设 Aesthetic、Packaging、Tolerance、Product 和 Software 类别已经创建。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">添加无类别限制的创建权限</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/14ae032ac217.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">创建权利在 ItemType 窗口的 Can Add 选项卡下分配。在全新的 ItemType 中进行任何分配之前，没有组或个人身份可以创建实例。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">检查员（Inspectors）组身份已添加到 Can Add 网格中，允许检查员组成员在数据库中创建记录。将分类列留空，组成员可以创建任何类类型的检查结果。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将组/身份限制为特定分类类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在我们的用例中，还有两个额外的组参与检查——Aesthetics 和 Packaging。我们需要允许这些组创建 Inspection Results，但只允许他们创建适当类类型的项目。这通过在 Can Add 网格中指定分类路径来完成。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">三个额外的组已添加到 Can Add 选项卡，每个行分配了正确的分类。保存后，这些组的成员将只能创建（或选择）配置的分类路径类型。确保勾选 Can Add 复选框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">再次提醒，如果未指定分类，该组/身份将能够创建任何类型的结果。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">分类特定「可添加」权利的执行方式</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当创建项目时，创建 Inspection Result 的用户在分类选择窗口中只会获得合格的选择。不合格的类选择会被灰显且不可选择。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：创建新项目时，在项目保存之前会显示通用表单。保存项目后，表单和属性会刷新以反映为该类类型配置的属性和表单。根据配置，给定用户/组可能有多个选择可用。由于用户可能属于多个组并具有多个分类权限，因此无法按用户自动选择分类。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">When items (i.e. Inspection Results) are created, the user creating the Inspection Result will be given only the eligible choices in the Classification selection window. In this way, each group ‘Can Add’ only the classifications that the administrator configured for the business rules in place.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意在下面的窗口中，不合格的类选项如何被灰显且不可选择（以美学组成员为例）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">此用户属于美学组，因此仅被授权为检查结果选择美学类类型：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/455b074feeab.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">注意：创建新项目时，在项目保存之前将显示通用表单：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3f7bb6079c85.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">保存项目后，表单和属性会刷新以反映为类类型配置的属性和表单：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">根据配置，给定用户/组可能有多个选择可用。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Because a user may belong to multiple groups and have multiple classification rights, it&apos;s not possible to automatically select a classification by user</Run></Paragraph>
</FlowDocument>',
   N'类别结构的可添加权限 简介 类别结构（Class Structure）允许将单个 Aras Innovator ItemType 专门化为类和子类，用于组织属性、启动不同的输入表单，以及通过 Can Add 设置为 ItemType 的实例设置权限。 例如，名为 Inspection Result 的 ItemType 可以分类为 Aesthetic、Packaging、Tolerance、Pro',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [44/244] class_specific_forms.htm (8 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('72dddc773f4a',
   N'分类特定表单',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">分类特定表单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: class_specific_forms.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">分类特定表单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可以为特定项目分类创建唯一表单，在选择分类时自动显示。Aras Innovator 检查项目可用的视图，查找与用户身份和查看模式匹配的视图。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要将类特定表单分配给特定子类：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">前提条件：如果尚未完成，请创建 ItemType、其分类结构和任何类特定属性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">编辑 ItemType。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「Views」选项卡。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「创建项目」按钮。这会在视图网格中创建新行。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6071d645e0dd.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">输入表单名称，选择表单类型（通常为 Default），并选择组身份（F2 打开搜索对话框）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「类别结构」按钮。ItemType 的类别结构将出现。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/1bc418b5e652.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">导航到并选择树中最低级别的分类叶子节点，点击工具栏上的「返回所选」图标。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">输入显示优先级。如果多个视图满足所有条件，数值最小的将被使用。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/82e50a57a10a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">对每个分类叶子节点重复步骤 3-6。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ebc292158fb2.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">保存 ItemType。这将表单链接到具有特定分类的 ItemType。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存 ItemType 后，双击新的视图行打开表单编辑器。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「锁定项目」按钮认领表单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">根据需要编辑表单。您可以从「未使用属性」列表中选择分类属性和类特定属性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「保存项目」按钮保存更改。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3f50a6ce89bb.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击「解锁」按钮取消认领表单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">对每个表单重复步骤 10-15。通过创建该 ItemType 的新项目并逐一选择每个叶子分类来测试行为。表单应在分类更改后立即切换。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/28f28223ac7e.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击「保存项目」按钮保存表单更改。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/29c7d82a1260.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击「解锁」按钮取消认领表单。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/8c97e8daf10e.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">对每个表单重复步骤10-15。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Test the behavior by creating a new Item of that ItemType, and one by one selecting each leaf classification. The form should immediately switch after the classification is changed.</Run></Paragraph>
</FlowDocument>',
   N'分类特定表单 可以为特定项目分类创建唯一表单，在选择分类时自动显示。Aras Innovator 检查项目可用的视图，查找与用户身份和查看模式匹配的视图。 要将类特定表单分配给特定子类： 前提条件：如果尚未完成，请创建 ItemType、其分类结构和任何类特定属性。 编辑 ItemType。 点击「Views」选项卡。 点击「创建项目」按钮。这会在视图网格中创建新行。 输入表单名称，选择表单类型（',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [45/244] Class_Structure.htm (13 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('63dfd8b2cf4c',
   N'类别结构',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">类别结构</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Class_Structure.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">类别结构</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">每个 ItemType 可以包含一个类别结构，其中包含多个级别的子类。每个子类可以有自己的类特定属性和表单。每个子类继承其父类的属性，也可以拥有自己的属性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建类别结构：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中，选择 管理 &gt; ItemTypes。打开一个 ItemType 并点击「类别结构」按钮打开类别结构对话框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：父类（即 ItemType 中定义的类）会自动显示在对话框中。右键点击对话框中的任何项目可弹出菜单，允许用户添加子类。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ad0715cd91d8.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">右键点击 Geometric Shape 项目访问上下文菜单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择「添加子类」。出现一个空字段，将颜色添加到类别结构中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击绿色勾选保存类别结构。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f842ad785ff2.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">现在有了类别结构，我们可以定义类特定属性。每个子类继承所有父类的所有属性。例如，定义正方形只需要一条边的长度，但定义矩形需要两条边。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建类特定属性：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a38dcdc951b8.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">在 ItemType 表单上，选择「属性」选项卡，搜索所有非隐藏属性。点击「新建属性」按钮添加以下属性，特别注意 Class Path 列。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/370db5d605f2.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Color（颜色）— 添加到 GeometricShape 父类。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Radius（半径）— 添加到 Circle 子类。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Side Length（边长）— 添加到 Square 子类。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3dcf870acda9.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Side 2 Length（第二边长）— 添加到 Rectangle 子类。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e0e6a23fe984.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/370db5d605f2.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">确保目录访问、权限和 Can Add 已按需设置。点击「保存并完成」保存 ItemType。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4878b36802fb.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中，选择 管理 &gt; 表单 &gt; 搜索表单。搜索 GeometricShape 表单并创建新实例。注意 Color 属性被所有子类继承，因为它定义在 GeometricShape 父级。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">半径 - 添加到圆形子类</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">边长 - 添加到正方形子类</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">第二边长 - 添加到矩形子类</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">请确保已按需设置了目录访问、权限和可添加。（请参阅权限）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「保存并完成」以保存并取消认领 ItemType。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中，选择 管理 --&gt; 表单。点击「搜索表单」按钮搜索 GeometricShape 表单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击锁定图标认领表单进行编辑。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3f50a6ce89bb.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">将分类和颜色放在表单上，如下所示：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/cea94cc277c8.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击保存表单。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/29c7d82a1260.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/8c97e8daf10e.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中，选择 管理&gt;表单&gt;搜索表单。搜索 GeometricShape 表单并创建新实例。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d9c30dfa2e3a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">注意，颜色属性被所有子类继承，因为它是在 GeometricShape 父级别定义的。</Run></Paragraph>
</FlowDocument>',
   N'类别结构 每个 ItemType 可以包含一个类别结构，其中包含多个级别的子类。每个子类可以有自己的类特定属性和表单。每个子类继承其父类的属性，也可以拥有自己的属性。 创建类别结构： 在导航窗格中，选择 管理 > ItemTypes。打开一个 ItemType 并点击「类别结构」按钮打开类别结构对话框。 注意：父类（即 ItemType 中定义的类）会自动显示在对话框中。右键点击对话框中的任何项目',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [46/244] Client_Display_Mode.htm (4 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('4a3112e975f1',
   N'设置客户端显示模式',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">设置客户端显示模式</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Client_Display_Mode.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设置客户端显示模式</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用以下步骤更改 Aras Innovator 中信息的显示方式：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1. 选择 管理 &gt; 变量 &gt; 搜索变量。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2. 在名称字段中输入 C* 并点击搜索。将出现变量列表。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9bcf17c7b30c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">3. 双击 Client Display Mode。将出现关联的变量窗口。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">4. 点击编辑将 Value 和 Default Value 字段从 Tabs（选项卡模式）更改为 Windows（标准模式）。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/657ab0f25878.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">选项卡模式如下所示。如果指定标准模式，所选项目将出现在单独的窗口中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">5. 点击保存更改。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/fe472b24b1a1.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">如果指定标准模式，您选择的项目将在独立窗口中显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">5. 点击保存更改。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'设置客户端显示模式 使用以下步骤更改 Aras Innovator 中信息的显示方式： 1. 选择 管理 > 变量 > 搜索变量。 2. 在名称字段中输入 C* 并点击搜索。将出现变量列表。 3. 双击 Client Display Mode。将出现关联的变量窗口。 4. 点击编辑将 Value 和 Default Value 字段从 Tabs（选项卡模式）更改为 Windows（标准模式）。 选',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [47/244] Client_Display_TOC.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('9128cd3b7faa',
   N'客户端显示模式',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">客户端显示模式</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Client_Display_TOC.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">客户端显示模式</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您可以使用 Client Display Mode 变量更改 Aras Innovator 中窗口的显示方式。更多信息请参阅：使用客户端显示模式。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用客户端显示模式</Run></Paragraph>
</FlowDocument>',
   N'客户端显示模式 您可以使用 Client Display Mode 变量更改 Aras Innovator 中窗口的显示方式。更多信息请参阅：使用客户端显示模式。 Using Client Display Mode',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [48/244] Common_User_Actions.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('f3d93d60cf0d',
   N'常见用户操作',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">常见用户操作</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Common_User_Actions.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">常见用户操作</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">常见的终端用户操作如下：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">认领或取消认领项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">更改密码</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建电子签名</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在生命周期中提升项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用位置</Run></Paragraph>
</FlowDocument>',
   N'常见用户操作 常见的终端用户操作如下： 认领或取消认领项目 更改密码 创建电子签名 在生命周期中提升项目 使用位置',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [49/244] configurable_history.html (8 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('6a67eccd86b6',
   N'可配置历史跟踪',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">可配置历史跟踪</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: configurable_history.html</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可配置历史跟踪</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Aras Innovator 提供跟踪项目历史并记录自创建以来所做更改的能力。它包含一组操作（Actions），可用于确定哪些事件被捕获在历史日志中。历史可以在 ItemType 级别或属性级别进行配置。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">需要注意的是，历史默认未启用，必须为每个需要跟踪历史的 ItemType 进行配置。这可以防止数据库因不必要的数据而承受压力。此外，如果现有 ItemType 启用了历史记录，则不会删除历史。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">历史操作</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">只有特定操作会触发系统写入历史记录。这些操作是：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Add（添加）— 新项目添加到数据库时记录，不存储评论。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Update（更新）— 项目更改保存或项目手动版本化时记录，不存储评论。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Promote（提升）— 项目提升到新状态时记录。包含在提升调用中时会存储评论，通常在生命周期转换上启用了 get_comment 且用户在提示时输入评论。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Reset Life Cycle State（重置生命周期状态）— 调用 resetLifecycle 服务器操作时记录，通常由管理员使用 Tools-&gt;Admin 菜单执行。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">SetDefaultLifecycle — 调用 setDefaultLifecycle 服务器操作时记录，存储反映项目新状态的评论。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Delete（删除）— 项目从数据库删除或项目世代被清除时记录。注意：项目在 Innovator 中删除时，其历史记录不会从数据库中删除。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">FormView — 项目表单打开时记录。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">FormPrint — 表单的可打印视图打开时记录。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">File Download（文件下载）— 文件下载时记录。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">历史模板</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">历史模板用于定义将触发系统存储历史记录的操作集以及条目中包含的特定文本。Aras Innovator 提供默认历史模板，可以进行自定义。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建自定义历史模板的步骤（仅管理员）：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">编辑历史模板 ItemType：Name = Administrators，Category = Administration。保存、解锁并关闭。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">导航到目录中管理文件夹下的历史模板。点击「新建历史模板」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在名称字段中输入模板名称（必填）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择 Property_history 复选框以存储引用此模板的 ItemType 的属性历史。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击历史模板操作关系选项卡。系统将所选操作记录作为新行填充到历史模板操作关系网格中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">为 ItemType 配置历史（仅管理员）：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在 ItemType 定义表单中，通过点击省略号选择历史模板。如果所选历史模板的「存储属性历史」设为 true，则启用了 Track History 属性的任何属性都将作为历史日志的一部分被跟踪。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">基于生命周期状态的历史</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">组织想要跟踪的历史内容可能取决于项目的生命周期状态。例如，当文档处于「初始」或「草稿」状态时，历史更改不如文档处于「已发布」状态时重要。在生命周期编辑器表单中，选择一个状态并选择历史模板。此历史模板将覆盖 ItemType 定义中指定的模板。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">查看项目历史日志（所有用户）：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开项目实例的表单。在表单主菜单中，点击「导航」按钮并从下拉菜单中选择「历史」。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/1b9b32ced315.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">项目历史对话框显示以下属性：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">When（时间）— 执行操作的日期和时间。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Who（操作人）— 用户的名字和姓氏。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Action（操作）— 对项目执行的操作。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/bc9cfb88b8d6.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Comment（评论）— 系统生成的评论或用户在项目提升时输入的评论。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Item State（项目状态）— 项目的当前状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Rev（修订）— 操作后项目的主要修订。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Gen（世代）— 操作后项目的世代号。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Select the Property_history checkbox to store histories for any ItemTypes referencing the template that have properties with history enabled. Deselect the checkbox if history should not be stored for any ItemTypes referencing this template that have properties with history enabled. In this case, the template setting overrides the property level history settings in the ItemType definition.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击表单下部的历史模板操作关系选项卡。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">系统将选定的操作记录作为新行填充到历史模板操作关系网格中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">配置 ItemType 的历史记录（仅限管理员）：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/94f32f731a9f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">基于生命周期状态的历史</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">There may be cases when what an organization wants to track for history is dependent upon the Item’s Life Cycle state. For example, when a Document is in the ‘Preliminary’ or ‘Draft’ state, history changes are not as important as when a document is in a ‘Released’ state. Therefore, Aras Innovator provides the ability to override the History Template specified in the History Template for certain states in the ItemType’s Life Cycle.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在生命周期编辑器表单中，选择映射中的一个状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Select a History Template by clicking the in the upper portion of the form (to the right of the History Template field). The system will launch a search dialog populated with available History Templates. Select the appropriate Template from the search dialog.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/346c536f8ef3.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0dd0b67cce07.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">This History Template will now override the History Template specified in the ItemType definition for each item instance that is in this Life Cycle state.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">View an Item’s History Log (All Users):</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开项目实例的表单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">In the Form’s Main Menu, click the Navigate button and select History from the dropdown menu:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b74123429e63.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">显示项目历史对话框。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/88f1a0856761.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">以下属性显示在项目历史对话框中：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">描述</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">执行操作的日期和时间</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Who</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户的姓和名</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Indicates the action that was performed on the item. See ‘History Actions’ (above) for the list of potential actions</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">评论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">A system generated comment based on what is defined in the ‘History Template Action’. For example, if a property is changed, the comment may include the property’s old value and the new value.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">OR</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户在项目提升（生命周期）时输入的评论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目状态</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目的当前状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Rev</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作执行后项目的主修订号。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Gen</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作执行后项目的世代号。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">In the ItemType definition form, select a History Template by clicking the ellipses in the upper portion of the form (to the right of the History Template field). The system launches a search dialog populated with available History Templates. Select the appropriate Template from the search dialog.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b135c30b0125.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">If the selected History Template has ‘Store Property History’ set to true (checked), then any properties with the Track History attribute enabled will be tracked as part of the history log. To enable history for a property, set the value of the Track History Attribute flag to true (checked).</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">导航到目录中管理文件夹下的 ItemTypes。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click the Select Items button on the relationship toolbar (note that Select Items is the only action available). The History Action search dialog appears.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从搜索对话框中选择一个或多个操作，然后点击绿色箭头。</Run></Paragraph>
</FlowDocument>',
   N'可配置历史跟踪 Aras Innovator 提供跟踪项目历史并记录自创建以来所做更改的能力。它包含一组操作（Actions），可用于确定哪些事件被捕获在历史日志中。历史可以在 ItemType 级别或属性级别进行配置。 需要注意的是，历史默认未启用，必须为每个需要跟踪历史的 ItemType 进行配置。这可以防止数据库因不必要的数据而承受压力。此外，如果现有 ItemType 启用了历史记录，则',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [50/244] configurable_history.htm (13 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('996a9e581612',
   N'可配置历史跟踪（续）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">可配置历史跟踪（续）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: configurable_history.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可配置历史跟踪</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Aras Innovator 提供跟踪项目历史并记录自创建以来所做更改的能力。它包含一组操作，可用于确定历史日志中捕获的事件。历史可以在 ItemType 级别或属性级别进行配置。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">需要注意的是，历史默认未启用，必须为每个 ItemType 配置。此外，如果现有 ItemType 启用了历史记录，则不会删除历史。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">历史操作</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">只有特定操作会触发系统写入历史记录：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Add（添加）— 新项目添加到数据库时记录。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Update（更新）— 项目更改保存或手动版本化时记录。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Promote（提升）— 项目提升到新状态时记录。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Reset Life Cycle State（重置生命周期状态）— 调用 resetLifecycle 服务器操作时记录。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">SetDefaultLifecycle — 调用 setDefaultLifecycle 服务器操作时记录。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Delete（删除）— 项目删除或世代清除时记录。项目删除时历史不会被删除。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">FormView — 表单打开时记录。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">FormPrint — 可打印视图打开时记录。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">历史模板</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">历史模板用于定义触发历史记录的操作集和特定文本。Aras Innovator 提供可自定义的默认历史模板。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建自定义历史模板（仅管理员）：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中，选择 管理 &gt; ItemTypes &gt; 搜索 ItemTypes。搜索历史模板 ItemType。编辑历史模板 ItemType 并添加 TOC Access：Name = Administrators，Category = Administration。解锁并关闭。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中，选择 管理 &gt; 历史模板 &gt; 新建历史模板。在名称字段中输入名称（必填）。确定是否允许保存属性级别历史。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击历史模板操作关系选项卡上的「添加历史操作」图标。选择一个或多个操作。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">为 ItemType 配置历史（仅管理员）：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在 ItemType 定义表单中，选择历史模板。如果启用了「存储属性历史」，则启用了 Track History 属性的属性将被跟踪。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">基于生命周期状态的历史 — 在生命周期编辑器中选择状态并分配历史模板，该模板将覆盖 ItemType 定义中指定的模板。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">查看项目历史日志（所有用户）：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开项目实例表单，点击「导航」按钮选择「历史」。项目历史对话框显示：When（时间）、Who（操作人）、Action（操作）、Comment（评论）、Item State（状态）、Rev（修订）、Gen（世代）、Created_on_tick。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">History Templates are used to define the set of actions that will trigger a history record to be stored by the system and the specific text that will be included in the entry. Aras Innovator provides a Default History Template (shown, below) that can be used for configuring history for an ItemType (note: The Default History Template is read-only). Alternatively, custom Templates may be created by an Administrator.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">默认历史模板：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/34f28efcb1ad.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">创建自定义历史模板（仅限管理员）：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中，选择 管理&gt;ItemTypes&gt;搜索 ItemTypes。将出现搜索网格。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索历史模板 ItemType。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">编辑历史模板 ItemType 并添加目录访问：名称 = 管理员，类别 = 管理</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">名称 = 管理员</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">类别 = 管理</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击取消认领并关闭历史模板 ItemType。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ba507963f59.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中，选择 管理&gt;历史模板&gt;创建新历史模板。将出现空白历史模板表单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在名称字段中输入名称（必填）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Determine if property level history is allowed to be saved: Set Property_History = true (checked) if history will be stored for any ItemTypes referencing this Template which have properties with history enabled. Set Property_History = false (unchecked) if history is not to be stored for any ItemTypes referencing this Template which have properties with history enabled. In this case, the template setting overrides the property level history settings in the ItemType definition.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Set Property_History = true (checked) if history will be stored for any ItemTypes referencing this Template which have properties with history enabled.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Set Property_History = false (unchecked) if history is not to be stored for any ItemTypes referencing this Template which have properties with history enabled. In this case, the template setting overrides the property level history settings in the ItemType definition.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click the Add History Actions icon on the History Template Action relationship tab. The History Action search dialog appears. It lists the actions available in the system.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/36d562c94ed3.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Select one or more actions from the search dialog and click the toolbar button. The actions you selected appear on the History Actions tab.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/11d26c325a74.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">配置 ItemType 的历史记录（仅限管理员）：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">In the ItemType definition form, select a History Template by clicking the in the upper portion of the form (to the right of the History Template field). The system will launch a search dialog populated with available History Templates. Select the appropriate Template from the search dialog.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b135c30b0125.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/27bf5fae6e11.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">If the selected History Template has &apos;Store Property History&apos; set to true (checked), then any properties with the Track History attribute enabled will be tracked as part of the history log. Set the value of the Track History Attribute flag to true (checked) to enable history for a property.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b3ca4660806c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">基于生命周期状态的历史</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">There may be cases when what an organization wants to track for history is dependent upon the Item’s Life Cycle state. For example, when a Document is in the ‘Preliminary’ or ‘Draft’ state, history changes are not as important as when a document is in a ‘Released’ state. Therefore, Innovator provides the ability to override the specified History Templatefor certain states in the ItemType’s Life Cycle.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在生命周期编辑器表单中，选择映射中的一个状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Select a History Template by clicking the in the the History Template field. The History Template search dialog box appears.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b135c30b0125.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">选择您要使用的生命周期映射。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d0375cedeb0b.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">This History Template you select overrides the History Template specified in the ItemType definition for each item instance that is in this Life Cycle state.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Viewing an Item’s History Log (All Users):</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1. 打开项目实例的表单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2. 点击「导航」按钮查看导航菜单，然后选择「历史」。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/59eb9549a5fc.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/05f84e096bf4.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">显示项目历史对话框。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/88f1a0856761.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">以下属性显示在项目历史对话框中：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">描述</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">执行操作的日期和时间</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Who</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户的姓和名</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Displays the action that was performed on the item. See ‘History Actions’ (above) for the list of potential actions</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">评论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">A system generated comment based on what is defined in the ‘History Template Action’. For example, if a property is changed, the comment may include the property’s old value and the new value.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">OR</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户在项目提升（生命周期）时输入的评论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目状态</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目的当前状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Rev</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作执行后项目的主修订号</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Gen</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作执行后项目的世代号</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建时间戳</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目的创建时间戳属性。</Run></Paragraph>
</FlowDocument>',
   N'可配置历史跟踪 Aras Innovator 提供跟踪项目历史并记录自创建以来所做更改的能力。它包含一组操作，可用于确定历史日志中捕获的事件。历史可以在 ItemType 级别或属性级别进行配置。 需要注意的是，历史默认未启用，必须为每个 ItemType 配置。此外，如果现有 ItemType 启用了历史记录，则不会删除历史。 历史操作 只有特定操作会触发系统写入历史记录： Add（添加）— 新',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [51/244] Configuring_Email.htm (12 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('a5167197ff1d',
   N'配置电子邮件',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">配置电子邮件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Configuring_Email.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">配置电子邮件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">电子邮件用于通知所选身份项目已进入特定生命周期状态或正在状态之间转换。对于状态和转换，配置对话框是相同的。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">对话框的顶部允许指定消息本身。您可以指定多条消息，因为您可能希望向不同的身份发送不同的消息。底部指定哪些身份接收消息。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6b9510021cb3.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">选择已配置的电子邮件：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1. 在生命周期映射中点击「配置电子邮件」链接。配置电子邮件对话框将出现。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2. 点击「添加电子邮件消息」按钮。电子邮件消息搜索对话框将出现。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">3. 点击「运行搜索」按钮查看之前配置的电子邮件列表。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/36d562c94ed3.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">4. 选择消息类型并确认。预配置的电子邮件将添加到列表中。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3dcf870acda9.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">属性说明：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b9f8bd40d6c9.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Name（名称）— 电子邮件消息的名称。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ad42022d1a90.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">From User（发件人）— 消息发送的身份。如果未指定，将使用配置电子邮件通知的身份。F2 打开搜索对话框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Subject（主题）— 电子邮件的主题行。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Body Plain（纯文本正文）— 消息的纯文本版本。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Body HTML（HTML 正文）— 消息的 HTML 版本。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Query String（查询字符串）— 如果需要编写复杂通知，可以编写查询从数据库请求数据。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Of Itemtype — 使电子邮件消息特定于正在提升或转换的 ItemType。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在对话框的身份部分点击「添加身份」指定接收电子邮件的现有身份。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建已配置的电子邮件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1. 点击生命周期映射中的「配置电子邮件」链接。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2. 点击「新建电子邮件」按钮创建自己的电子邮件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">3. 在名称字段中输入电子邮件名称。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">4. 双击发件人单元格并点击省略号按钮选择关联的用户。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">5. 选择要关联的用户名。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在电子邮件文本中使用变量</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可以在电子邮件文本中使用变量来标识具体项目及其属性值，使消息更具体。语法：${属性名称}。系统变量包括：$[USER]（当前登录名）、$[ALIAS]（用户别名身份）、$[DATE]（今天的日期）、$[TIME]（当前时间）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例：主题行中使用 ${name} 和 ${state} 变量。请确保使用属性名称而不是标签。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Because the same life cycle map can be used for many different ItemTypes, this field is used to make the e-mail message specific to a particular ItemType that is being promoted or transitioned. If no ItemType is specified in this field, this message will go out for every ItemType that has this life cycle and that is promoted or transitioned to the specific state/transition where this e-mail message is configured.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click the Add Identities button in the Identities section of the dialog to specify existing identities that will receive the e-mail (make certain that those identities have a valid e-mail address). Click the New Identity to add an identity. The Identity search dialog appears.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ce9fb703dda7.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">6. 选择您希望接收电子邮件的身份并点击添加。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ad42022d1a90.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">创建配置的电子邮件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用以下步骤创建电子邮件：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1. Click on the Configure Email link in the LifeCycle Map that you have created/selected. The Configure E-Mail dalog appears.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2. Click the New Email button to create your own email. A row is added to the dialog box.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e6e5913a54af.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/acb0610eb761.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">3. 在名称字段中输入电子邮件名称。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">4. Double-click the From User cell and click the ellipses button to select the users to associate with the email. The User search dialog appears.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b135c30b0125.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">5. 选择要与电子邮件关联的用户名并点击。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ad42022d1a90.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">在电子邮件正文中使用变量</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">You can use variables in the e-mail text to identify the exact item and its property values, making the e-mail message more specific. You can insert any item property using the syntax ${property name}, including system properties. There are also the following runtime variables supported in Innovator:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">变量</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">定义</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">$[用户]</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当前登录用户名</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">$[别名]</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户的别名身份</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">$[日期]</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">包含今天日期的字符串</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">$[时间]</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">包含当前时间的字符串</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Here is an example. We have an item with the following properties: name, part_number, and color. It also has the usual system properties, and we will use the property state to indicate which life cycle state the item is entering. An e-mail message could be configured as shown below. If you right click on the e-mail row and select View &quot;Email Message&quot; from the pop-up menu, you will see the Email message in a separate window, as shown here.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/2ae82e15ecbf.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Notice the use of two variables in the subject line - ${name} and ${state}. Please make sure that you use the property names, not labels. The Body of the message is using the additional property ${part_number}.</Run></Paragraph>
</FlowDocument>',
   N'配置电子邮件 电子邮件用于通知所选身份项目已进入特定生命周期状态或正在状态之间转换。对于状态和转换，配置对话框是相同的。 对话框的顶部允许指定消息本身。您可以指定多条消息，因为您可能希望向不同的身份发送不同的消息。底部指定哪些身份接收消息。 选择已配置的电子邮件： 1. 在生命周期映射中点击「配置电子邮件」链接。配置电子邮件对话框将出现。 2. 点击「添加电子邮件消息」按钮。电子邮件消息搜索对话框',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [52/244] Configuring_Notifications.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('9a8c2366be9b',
   N'配置通知',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">配置通知</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Configuring_Notifications.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">配置通知</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">通知允许配置电子邮件消息，从选定身份自动发送到选定身份。通知由选定事件触发，如 On Activate（活动激活时）或 On Delegate（委托时）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">通知配置属性：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4807947efbae.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Name（名称）— 通知本身的名称。可以重复使用，名称应该是描述此电子邮件提供什么信息的良好标识符。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">From User（发件人）— 发送通知的电子邮件地址所属的身份。F2 打开身份搜索对话框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Subject（主题）— 电子邮件主题行，可以使用变量。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Body Plain（纯文本正文）— 纯文本消息，可以使用变量。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Body HTML（HTML 正文）— HTML 消息，可以使用变量。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Query String（查询字符串）— 用于编写复杂通知的查询。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Event（事件）— 电子邮件在所选事件时自动发送。可选事件：On Activate、On Assign、On Refuse、On Delegate、On Vote、On Remind、On Due、On Escalate、On Close。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Target（目标）— 通知的接收者。可选项：All Assignments（所有受让人）、Open Assignments（未完成的受让人）、Closed Assignments（已完成的受让人）、From Identity（委托/升级来源身份）、To Identity（委托/升级目标身份）、Alternate（替代身份）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Alternate（替代）— 用于接收通知的替代身份。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在电子邮件文本中使用变量 — 访问工作流消息中项目的属性值：${Item[@type=&apos;ItemType_name&apos;]/property_name}。系统变量：$[USER]、$[ALIAS]、$[DATE]、$[TIME]。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Plain text version of the message to be sent (for recipients who do not accept HTML). The body of the e-mail can use variables in its text. See Using Variables for more information.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">HTML 正文</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">HTML version of the message to be sent. The body of the e-mail can use variables in its text. See Using Variables for more information.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">查询字符串</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">If you wish to write complex notifications, that require data from various items in Innovator, you can write a query to request this data from the database. The retrieved data can then be referenced in the body of the e-mail message. For example, if you wish to show values of a related item in the e-mail message, you will need to write a query to retrieve it. To learn how to write these queries, please refer to the Advanced Programming course, or contact your Aras Consultant.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">事件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The e-mail message is automatically sent on the selected event. The list of events to choose from are:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">激活时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">分配时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">拒绝时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">委派时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">投票时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">提醒时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">到期时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">升级时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关闭时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">For example, if you wish to send notification to the assignees that a task which is assigned to them is active and in their InBasket, you would select On Activate for the Event, and All Assignments as Target.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">目标</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">通知的接收者。可选选项有：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">All Assignments - All assignees listed in the Assignments tab for this activity. If there is an assignee which is a group identity, all members of that identity will receive this notification.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Open Assignments - All assignees that have not completed the activity. Typically this Target is used with the On Remind event.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Closed Assignments - All assignees that have completed the activity. Typically this Target would be used with the On Escalate event to inform</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">From Identity - Used only for escalation or delegation, this identity is the one from which the assignment is being delegated or escalated.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">To Identity - Used only for escalation or delegation, this identity is the one being delegated to or escalated to.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">替代 - 替代字段中定义的身份（见下文）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">替代</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">接收通知的替代身份</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在电子邮件正文中使用变量</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">You can use variables in the e-mail text to identify the exact item and its property values, making the e-mail message more specific. Let&apos;s take a look at the XML for the Workflow Item, it looks something like:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn"><AML></Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ECR-1000</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">已发布</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">对我投票</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">AML></Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">因此，如果我们想访问 ECR 状态属性的值，可以写为：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">${Item[@type=&apos;ECR&apos;]/state}</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果我们想访问活动的名称，可以写为：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">${Item[@type=&apos;Activity&apos;]/name}</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">So, in general, the format for accessing a property value of an item within a workflow message, would be:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">${Item[@type=&apos;ItemType_name&apos;]/property_name}</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">where ItemType_name is the name of the ItemType whose property value you want to access, and property_name is the name of the property.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Innovator also supports the following runtime system variables that can be used inside the notification fields:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">变量</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">定义</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">$[用户]</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当前登录用户名</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">$[别名]</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户的别名身份</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">$[日期]</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">包含今天日期的字符串</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">$[时间]</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">包含当前时间的字符串</Run></Paragraph>
</FlowDocument>',
   N'配置通知 通知允许配置电子邮件消息，从选定身份自动发送到选定身份。通知由选定事件触发，如 On Activate（活动激活时）或 On Delegate（委托时）。 通知配置属性： Name（名称）— 通知本身的名称。可以重复使用，名称应该是描述此电子邮件提供什么信息的良好标识符。 From User（发件人）— 发送通知的电子邮件地址所属的身份。F2 打开身份搜索对话框。 Subject（主题）',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [53/244] configuring_sequences.htm (6 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('5538e40338fd',
   N'配置序列',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">配置序列</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: configuring_sequences.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">配置序列</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">序列（Sequences）用于自动编号 PR、ECR、ECN 或 Innovator 中的任何其他选定项目。顺序编号是零部件和文档的选项。本节展示如何配置序列外观以符合您的编号需求。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">查看预定义序列：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中选择 管理 &gt; 序列 &gt; 搜索序列。搜索网格显示现有序列列表。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性说明：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9bcf17c7b30c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Name（名称）— 序列项的名称。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f3b699840cc7.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Prefix（前缀）— 生成编号的字母数字前缀。例如，默认所有 ECN 将有编号：ECN-10000x。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Current Value（当前值）— 序列的当前值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Suffix（后缀）— 类似于前缀，但添加到编号末尾。例如，如果指定后缀 -GTX，ECN 编号将为：ECN-100001-GTX。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Pad With（填充字符）— 指定填充所需空格数的字符。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Pad To（填充到）— 编号的总字符数，不包括前缀和后缀。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Initial Value（初始值）— 编号与前后编号之间的递增量。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">配置序列（以 ECN 序列为例）：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在搜索网格中双击 ECN 序列并点击编辑。ECN 序列表单将出现。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例 1 — 如果输入上述值，ECN 编号将为：ECN-xxxx100002、ECN-xxxx100003、ECN-xxxx100004 等。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例 2 — 如果输入上述值，ECN 编号将为：ECN-00001020xpj、ECN-00001030xpj、ECN-00001040xpj 等。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">双击搜索网格中的 ECN 序列并点击。将出现 ECN 序列表单：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/eaf68ab55cfa.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/df8c0d6c0cd6.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The following examples show how these properties work to together to create a sequence configuration.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例1：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/dd86e39b7483.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">If the above values are entered, then the ECN numbers would be as follows: ECN-xxxx100002, ECN-xxxx100003, ECN-xxxx100004, etc.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例2：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c283d1b18e3d.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">If the above values are entered, then the ECN numbers would be as follows: ECN-00001020xpj, ECN-00001030xpj, ECN-00001040xpj, etc.</Run></Paragraph>
</FlowDocument>',
   N'配置序列 序列（Sequences）用于自动编号 PR、ECR、ECN 或 Innovator 中的任何其他选定项目。顺序编号是零部件和文档的选项。本节展示如何配置序列外观以符合您的编号需求。 查看预定义序列： 在导航窗格中选择 管理 > 序列 > 搜索序列。搜索网格显示现有序列列表。 属性说明： Name（名称）— 序列项的名称。 Prefix（前缀）— 生成编号的字母数字前缀。例如，默认所有 ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [54/244] Context_menu.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('d86a36e0c328',
   N'上下文菜单（右键菜单）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">上下文菜单（右键菜单）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Context_menu.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">上下文菜单（右键菜单）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在主网格中对项目使用鼠标右键（右键点击）时，会出现上下文菜单。上下文中包含的项目可能因网格类型而异。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">右键点击显示的选项取决于主网格中选择的项目。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/42eb773f55f5.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Open（打开）— 打开项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Claim（认领）— 锁定项目进行编辑。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Promote（提升）— 提供将项目提升到已分配生命周期映射中定义的下一个可用状态的选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Properties（属性）— 显示项目的通用属性，如创建日期、修改日期、创建者、修改者等。还提供复制项目 32 位唯一 ID 的选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Save As（另存为）— 创建项目的副本。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Delete All Versions（删除所有版本）— 删除所有版本。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Structure Browser（结构浏览器）— 显示所选项目的结构。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Where Used（使用位置）— 显示项目之间关系的树形结构。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Revisions（修订）— 显示项目的修订版本。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Add to Package Definition（添加到包定义）— 允许选择包定义将项目添加到其中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Reset Lifecycle State（重置生命周期状态）— 重置生命周期状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Reset Item Access（重置项目访问）— 重置项目的访问权限。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Set Default Life Cycle（设置默认生命周期）— 为项目创建生命周期映射。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType Definition Report（ItemType 定义报表）— 显示指定项目的信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType Permissions Report（ItemType 权限报表）— 显示所选项目的权限信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType History Configuration（ItemType 历史配置）— 显示与项目关联的属性和操作信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示选定项目的结构。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用位置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示展示项目之间关系的树形结构。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">修订</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示项目的修订信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">添加到包定义</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Enables you to select a package definition to add the item to. You can also create a new package definition for a particular item.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">重置生命周期状态</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Logged when the &quot;reset_lifecycle&quot; server action is called. This is normally done by an administrator. No comment is stored.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">重置项目访问权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您重置项目的访问权限。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设置默认生命周期</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您为项目创建生命周期映射。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType 定义报告</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示指定项目的信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType 权限报告</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示选定项目的权限信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType 历史配置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示与项目关联的属性和操作的信息。</Run></Paragraph>
</FlowDocument>',
   N'上下文菜单（右键菜单） 在主网格中对项目使用鼠标右键（右键点击）时，会出现上下文菜单。上下文中包含的项目可能因网格类型而异。 右键点击显示的选项取决于主网格中选择的项目。 Open（打开）— 打开项目。 Claim（认领）— 锁定项目进行编辑。 Promote（提升）— 提供将项目提升到已分配生命周期映射中定义的下一个可用状态的选项。 Properties（属性）— 显示项目的通用属性，如创建日期',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [55/244] Context_menu_related_item.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('075277189856',
   N'上下文菜单（关联项目）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">上下文菜单（关联项目）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Context_menu_related_item.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">上下文菜单（关联项目）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在关系网格中对关联项目使用鼠标右键时，上下文菜单以弹出形式显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：右键点击显示的选项取决于关系网格中选择的 ItemType。File ItemType 的选项可能包含与文件签入/签出相关的额外选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Open（打开）— 仅在关系项目有定义的表单时打开关系项目的项目窗口。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Edit（编辑）— 仅在关联项目已添加到关系中时，以编辑模式认领关联项目。绿色标志出现在关系网格中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Replace（替换）— 用指定的项目替换关系网格中的所选项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Claim（认领）— 锁定关联项目并防止其他用户提交更改。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Actions（操作）— 打开搜索窗口搜索项目并替换当前关联项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Relationship Types（关系类型）— 打开另一个上下文菜单，允许选择：打开所选项目、复制所选行、粘贴所选行、将所选项目复制到剪贴板、在剪贴板中显示所选项目、将所选项目提升到下一阶段。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：上下文菜单包含标准选项和操作。管理员定义关系的可用操作。显示的上下文菜单选项取决于登录用户的访问权限。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">For example, in Part ItemType, clicking Edit in the relationship grid displays the related (child) Part Item.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">替换</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Replaces the selected Item in the Relationship grid with the item you specify. The name of the replacement item appears in the Name column.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">认领</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">锁定关联（子）项目并阻止其他用户提交更改。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Opens the Search window to search an Item and replaces the current related/child Item with the Item selected from the Search window.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Opens another context menu that enables you to select one of the following options:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开选定项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">复制选定行。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">粘贴选定行。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将选定项目复制到剪贴板。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在剪贴板中显示选定项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将选定项目提升到下一阶段。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Note: The context menu comprises standard options (covered in table) as well as Actions. The administrator defines the available Actions for a relationship. The context menu options for the various Actions to be displayed depend on the logged in user&apos;s access rights. The access rights are configured by the administrator.</Run></Paragraph>
</FlowDocument>',
   N'上下文菜单（关联项目） 在关系网格中对关联项目使用鼠标右键时，上下文菜单以弹出形式显示。 注意：右键点击显示的选项取决于关系网格中选择的 ItemType。File ItemType 的选项可能包含与文件签入/签出相关的额外选项。 Open（打开）— 仅在关系项目有定义的表单时打开关系项目的项目窗口。 Edit（编辑）— 仅在关联项目已添加到关系中时，以编辑模式认领关联项目。绿色标志出现在关系网格',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [56/244] Copying_an_Item.htm (3 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('abfd6de5090d',
   N'复制和粘贴关系项目',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">复制和粘贴关系项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Copying_an_Item.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">复制和粘贴关系项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您可以使用复制和粘贴功能创建关系项目的多个实例。在此示例中，我们使用 Part ItemType 作为源项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建关系项目的副本：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">登录 Aras Innovator。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择 设计 &gt; 零部件 &gt; 搜索零部件。搜索网格将出现。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">右键点击要添加关系的源（父）项目。零部件项目窗口将出现。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在关系网格中，右键点击要复制的项目。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d12e3fe9215c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">从上下文菜单中选择「另存为」。将出现项目的副本。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：只能复制已保存到服务器的项目。如果尝试复制未保存的项目，将出现错误。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3dcedf793273.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Note: You can only copy Items that are saved to the Server. If you try to copy items that have not been saved, an error appears.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">副本也会出现在搜索网格中：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4f16b7d8d5d9.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'复制和粘贴关系项目 您可以使用复制和粘贴功能创建关系项目的多个实例。在此示例中，我们使用 Part ItemType 作为源项目。 创建关系项目的副本： 登录 Aras Innovator。 选择 设计 > 零部件 > 搜索零部件。搜索网格将出现。 右键点击要添加关系的源（父）项目。零部件项目窗口将出现。 在关系网格中，右键点击要复制的项目。 从上下文菜单中选择「另存为」。将出现项目的副本。 注意',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [57/244] Copyright_Information.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('a3a4eef9fe8b',
   N'版权信息',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">版权信息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Copyright_Information.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">版权信息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">权利声明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">版权所有 © 2025 Aras Corporation 及/或其附属公司。保留所有权利。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">本文档受美国和国际版权法和公约的保护。本文档不得修改、更改、复制或传输，除非获得 Aras Corporation 的明确书面许可。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Aras Innovator、Aras 和 Aras Corp「A」标志是 Aras Corporation 在美国和其他国家/地区的注册商标。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">本文中引用的所有其他商标均为其各自所有者的财产。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">责任声明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">本文档仅供参考，其内容如有更改，恕不另行通知。本文档中的信息按「原样」分发，不提供任何明示或暗示的保证。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Aras 公司</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">100 Brickstone 广场，100号套房</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">安多弗，马萨诸塞州 01810</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">电话：978-806-9400</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">网站：https://www.aras.com</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">网站：https://www.aras.com</Run></Paragraph>
</FlowDocument>',
   N'版权信息 权利声明 版权所有 © 2025 Aras Corporation 及/或其附属公司。保留所有权利。 本文档受美国和国际版权法和公约的保护。本文档不得修改、更改、复制或传输，除非获得 Aras Corporation 的明确书面许可。 Aras Innovator、Aras 和 Aras Corp「A」标志是 Aras Corporation 在美国和其他国家/地区的注册商标。 本文中引',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [58/244] create_a_team.htm (5 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('7016d97dd0b4',
   N'创建团队（仅管理员）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建团队（仅管理员）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: create_a_team.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建团队（仅管理员）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中，选择 管理 &gt; 团队 &gt; 新建团队。新团队窗口将出现。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在名称字段中输入团队名称（必填）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在描述字段中输入团队的简要描述。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「添加身份」按钮。身份搜索对话框将出现。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/36d562c94ed3.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击搜索查看身份列表。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5da5ed34617a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">从搜索对话框中选择一个或多个身份并确认。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ad42022d1a90.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">系统将所选身份记录作为新行填充到团队成员关系网格中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要为团队成员分配角色（可选），双击关系网格中的角色字段并点击省略号按钮。身份搜索对话框将出现。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b135c30b0125.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">注意：搜索已预过滤为 IS_ALIAS=&apos;0&apos; 和 Classification=&apos;Team&apos;，用户无法更改。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从搜索对话框中选择角色并确认。角色是可选的，可以留空。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ad42022d1a90.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">注意：角色是可选的，可以留空。</Run></Paragraph>
</FlowDocument>',
   N'创建团队（仅管理员） 在导航窗格中，选择 管理 > 团队 > 新建团队。新团队窗口将出现。 在名称字段中输入团队名称（必填）。 在描述字段中输入团队的简要描述。 点击「添加身份」按钮。身份搜索对话框将出现。 点击搜索查看身份列表。 从搜索对话框中选择一个或多个身份并确认。 系统将所选身份记录作为新行填充到团队成员关系网格中。 要为团队成员分配角色（可选），双击关系网格中的角色字段并点击省略号按钮。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [59/244] Create_Forum.htm (4 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('06c689a2f8c8',
   N'添加论坛',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">添加论坛</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Create_Forum.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">添加论坛</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击书签面板中论坛部分的「添加论坛」按钮创建新论坛。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建新论坛的步骤：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击书签面板中的「添加论坛」按钮打开「创建新论坛」对话框。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/98dd55d4667e.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">在文本框中输入论坛名称。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c3d3478009a7.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">按回车键创建论坛。此论坛将添加到书签中，默认对所有人可用。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要设置不同的权限或共享设置，右键点击论坛并选择「分享」。这将打开分享论坛对话框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Public（公开）— 使论坛对所有人可用。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Private（私有）— 创建仅创建者可见的私有论坛。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f42d50b2716a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Shared with Identities（与身份共享）— 创建要共享论坛的身份列表。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建共享身份论坛：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1. 在搜索框中输入身份名称。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2. 选择要共享论坛的身份。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">3. 点击确定保存更改。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">4. 在分享论坛对话框中点击确定保存更改。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1. 在搜索框中输入身份名称。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7a9bd4cbbdcc.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">2. 选择您希望共享论坛的身份。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">3. 点击确定保存更改，或点击取消退出共享对话框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">4. Click OK to save the changes in the Share Forum dialog box or click Cancel to exit from this dialog box.</Run></Paragraph>
</FlowDocument>',
   N'添加论坛 点击书签面板中论坛部分的「添加论坛」按钮创建新论坛。 创建新论坛的步骤： 点击书签面板中的「添加论坛」按钮打开「创建新论坛」对话框。 在文本框中输入论坛名称。 按回车键创建论坛。此论坛将添加到书签中，默认对所有人可用。 要设置不同的权限或共享设置，右键点击论坛并选择「分享」。这将打开分享论坛对话框。 Public（公开）— 使论坛对所有人可用。 Private（私有）— 创建仅创建者可见',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [60/244] Create_Items_to_be_Related.htm (2 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('6ce840588fe4',
   N'创建要关联的 ItemType',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建要关联的 ItemType</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Create_Items_to_be_Related.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建要关联的 ItemType</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在开始创建关系之前，需要定义要关联的 ItemType。在以下示例中，我们将定义两个 ItemType：Housekeeping Planner（管家计划器）和 Contractor（承包商）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意这是一个非常基本的定义，有五个已定义的属性。将与 HousePlanner 关联的 ItemType 是 Contractor。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f81ce6dee296.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Contractor 的定义如下：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意 Contractor 也有不同的属性，如类型（由选项列表控制）、地址、名称、电话号码和可靠性评级。现在已经定义了两个要关联的 ItemType，我们可以继续定义关系。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ebe5b13c1921.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Notice that Contractor also has different properties, such as its type, controlled by a list of options, its address, name, phone number, and a reliability rating. Now that we have defined two ItemTypes to be related, we can continue to define the relationship itself.</Run></Paragraph>
</FlowDocument>',
   N'创建要关联的 ItemType 在开始创建关系之前，需要定义要关联的 ItemType。在以下示例中，我们将定义两个 ItemType：Housekeeping Planner（管家计划器）和 Contractor（承包商）。 注意这是一个非常基本的定义，有五个已定义的属性。将与 HousePlanner 关联的 ItemType 是 Contractor。 Contractor 的定义如下： 注',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [61/244] Creating_an_Action.htm (4 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('fdcbe39d0608',
   N'创建操作',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建操作</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Creating_an_Action.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建操作</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1. 在导航窗格中，选择 管理 &gt; 操作，将出现以下菜单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2. 选择「新建操作」。将出现空白操作窗口。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7645ed490026.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">3. 在字段中输入适当的信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">4. 点击保存并解锁项目。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ba8a76b328da.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">3. 在字段中输入适当的信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">4. 点击保存并取消认领项目。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ba507963f59.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'创建操作 1. 在导航窗格中，选择 管理 > 操作，将出现以下菜单。 2. 选择「新建操作」。将出现空白操作窗口。 3. 在字段中输入适当的信息。 4. 点击保存并解锁项目。 3. Enter the appropriate information in the fields. 4. Click and to save and unclaim the item.',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [62/244] Creating_an_ItemType.htm (23 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('61740a6ba863',
   N'创建项目类型',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建项目类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Creating_an_ItemType.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建项目类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Innovator 中的每个项目都是 ItemType 的实例。创建 ItemType 包括填写其头部属性和选项卡属性。某些属性可能导致复杂的结构。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建 ItemType 的步骤：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1. 从导航窗格，选择 管理 &gt; ItemTypes，将出现以下菜单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2. 点击「新建 ItemType」。将出现空白 ItemType 窗口。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3894b1fba77d.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">让我们先查看头部属性。以下是头部属性及其说明列表：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/1850b5da53dd.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Let&apos;s look at the header properties first. Here is a list of the header properties and their explanations:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">名称 - ItemType 的名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Singular Label - the label of the ItemType that appears in an Item window. It serves as a general reference to a single instance of the ItemType.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Plural Label - the label of the ItemType that appears in the TOC and serves as a general reference to multiple instances of the ItemType.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Class Structure - click if you wish to design a class structure for this item. The Class Structure dialog appears. Right-click on any item in the structure to access the pop-up menu:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/bd02ef5e2b42.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Right-clicking on any item in the structure, brings up this pop-up menu. You can create subclasses as many levels deep as required. Then, back on the ItemType form, you can create class specific properties for each subclass, as well as designate different forms for each subclass. For more information on how these structures work, please refer to Class Structure.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Small Icon - Click on the link. The Image Browser dialog appears. Click the Images folder and select the icon of your choice. Click the Return Selected icon to complete your selection. Notice that the link is replaced by the icon that you chose. The small icon will appear next to the ItemType label in the TOC.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/370db5d605f2.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e9ef515a3b20.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Large Icon - Click on the link. The Image Browser dialog appears. Select the icon of your choice, and click the Return Selected icon to complete your selection. Notice that the link is replaced by the icon that you chose. The large icon will appear on the ItemType form used to create instances of the ItemType.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/370db5d605f2.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Default Life Cycle- every item in Innovator may have an associated Life Cycle. To understand how to create Life Cycles, what they mean, and what options are available, please refer to Life Cycles.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">3. If you have created a Life Cycle that you wish to assign to this ItemType, select the Life Cycle tab.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/8d265150f8a9.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">4. Click the Select Items icon . The Life Cycle Map search dialog appears. Click the Run Search icon.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/81d30419a164.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3dcf870acda9.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c60e72be864b.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">5. Select the Life Cycle you want to assign to this ItemType, then click the Return Selected icon to complete the selection.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/370db5d605f2.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">History Template - Defines what history will be saved for the ItemType. For more information, see Configurable History</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Default Structure View - Defines whether the item form displays tabs or not. For each relationship described in the ItemType, a tab is displayed on the item form. For example, let&apos;s look at the ItemType definition for Parts:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/50d3a3b15f01.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Notice that there are nine tabs defined, with the labels shown in the center column. If we were to create a new Part, the following form appears:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/718b485c91df.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">注意，所述选项卡出现在新建零部件表单上。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">To have the tabs appear on the form, select Tabs On from the dropdown in the Default Structure View property in the ItemType definition. If you wish to hide the tabs, change the Default Structure View to Tabs Off.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The Default Structure View controls the tab setting for all instances of this ItemType; however, there is also a way to control the structure view on an individual basis for each form, simply by selecting the Hide Tabs icon directly on the form:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/cf47643a71f6.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c2789b417de5.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">If the button is selected when the form appears, and you wish to see the tabs, simply click the icon. Similarly, if you do not wish to see the tabs and they appear on the form, simply select the icon again.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Versioning- controls the different possible settings of version control for the item instances created from the ItemType definition:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3920166bdd8e.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Versionable - when checked, this setting indicates that each time the item is edited a new generation of the item is created. All generations of the item are stored in the database, and can be referenced as necessary. Click the More button and select Properties from the menu to view the item&apos;s Revision and Generation information:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/eeb4e2a96040.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/8b447555c3a0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Notice that for each selected item in the list, the last group of three properties shows the Major Rev and the Generation.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Manual Versioning - when checked this setting indicates that the version increment will take place manually, when initiated by the user. Each version, however, will be saved in the database.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Revisions - an Item that lists the Revision sequence. The default is each letter of the alphabet in order. To create a different sequence for revisions, see Revisions.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Search - defines parameters which govern the appearance of the list page when the Item is selected in the main tree view, or TOC.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b35756db6bd2.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Auto Search - a boolean to indicate if the search should be invoked when the user clicks on the defined ItemType in the TOC. For example, if you go to TOC --&gt; Administration --&gt; Charts, you will see that the Charts list will automatically populate. In the Charts ItemType definition, the Auto Search option is set to true.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Default Page Size - if you selected to do Auto Search, you can specify the default page size, which means how many items will appear on a page. Typically this number is set to 25. If left blank, the page size equals the Max Records field.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Max Records - Specify the maximum number of records for the search. If there are numerous instances, limiting the search each time this page is opened, improves performance.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Implementation Type - specifies which type of item is being defined, Single Item or Poly Item. If Poly Item is specified, and the ItemType is saved, it cannot revert back to the Single Item.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7eb0924497b9.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Single Item - the typical implementation where the ItemType represents a definition of a class of items. The Single Item is the Implementation Type assumed in this section.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Poly Item - the wrapper item which really points to any number of specified Single Items. For example, in Project Management there is a Poly Item called the Deliverable which can be any number of things, such as a Document, an ECN, a Part, a DFMEA, etc., the idea being that it is the item being delivered as a result of a scheduled activity. Please refer to Poly Items for more information.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ae5ddf8baab2.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Unlock on Logout - a Boolean to indicate if the item is to be unlocked when the user that locked it logs out.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Dependent - a Boolean to indicate that this item depends on another Item, and is meaningless without the parent item. For example, documentation for a particular part is meaningless if the part is deleted. So, if the item is PartDocumentation, it would probably be dependent on the Part.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Is Relationship - a Boolean to indicate if this ItemType is a relationship. See Relationships for further instructions on creating relationships.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Enforce Discovery - If checked, discovery access is enabled, otherwise discovery access is disabled, see Discovery Privilege</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Use Src Access - applies to Relationship ItemTypes, when checked this means that the permissions to the related items will be the same as on the parent or source item.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Allow Private Permissions - a Boolean to indicate if the access to a single item instance can be changed by an identity in the current set of permissions. For example, if an identity (such as an auditor) needs to be added to the permissions set for a particular Project instance.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The next step is to enter the information for the different tabs in the ItemType. Below is a list of these tabs and links to the pages which describe each one.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b7b7a8c08e60.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">属性 - 为 ItemType 输入新属性，请参阅输入 ItemType 属性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系类型 - 为 ItemType 定义关系，请参阅关于关系。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Views - to select the Forms used for the ItemType, Note that different Forms can be used for Viewing and Editing, and for different Classifications, in each case for different Identities</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Server Events - this tab holds a list of server-side methods that could be triggered by a number of different events, such as lock and unlock. The discussion of these methods is beyond the scope of this manual. To write these methods, one must have a solid knowledge of programming as taught in the Aras Advanced Programming Training. The properties that can be set for each method, such as the RunAs User, are also fully explained and taught in this course.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Actions - Enable users to select custom behavior implemented in Methods, from the Main or Item Window Menus</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">生命周期 - 允许为不同的分类选择不同的生命周期映射。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Workflows - this tab lists all the workflows associated with this item, whether the workflow is started programmatically, or is a subflow of another workflow, or is the default workflow started automatically on instance creation, it must be listed here. To learn more about workflows, how they work and how to create them, please see Workflows.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">TOC Access - the information under this tab describes where the new Item will appear on the TOC and which identities will be able to see it and access it. See TOC Access Permission for more information.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">TOC View - the information under this tab may alter what the user sees when they select an Item from the TOC. These settings are identity based, so a different view may be created for different users.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Client Events - this tab holds a list of client-side methods that are triggered with three different events: OnBeforeNew, OnNew, and OnAfterNew. These events have to do with instance creation. So, for example, when a new instance is created, you might want to run a method to populate certain properties, prior to the user seeing the item form for the first time.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The other properties of this tab, such as the RunAs User and Password are explained fully in the Aras Advanced Programming Training. We strongly suggest that you take this class prior to writing methods or designing complex customizations.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/124d900f3171.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">xItemTypeAllowedProperty</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Can Add - this tab contains a list of identities which can add the new instances of this item type. See Can Add for more informations.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Permissions - this tab contains the permissions for this item, i.e. which identity can see the items, edit them, and delete them. See Permissions for more details.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">报告 -</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Polysources - this tab is used for the creation of Poly Items, to specify the source ItemTypes which are combined to form this PolyItem. See Creating a Poly Item ItemType for more information.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">客户端样式 - 此选项卡用于自定义用户界面。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Secure Social - this tab is used to specify to make the ItemType usable for secure social activities such as discussion boards and bookmarks.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">实现 - 此选项卡用于添加多态 ItemType。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">继承的服务器事件 - 此选项卡用于向 ItemType 添加继承的服务器事件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">xProperties - 此选项卡用于向 ItemType 添加 xProperties。</Run></Paragraph>
</FlowDocument>',
   N'创建项目类型 Innovator 中的每个项目都是 ItemType 的实例。创建 ItemType 包括填写其头部属性和选项卡属性。某些属性可能导致复杂的结构。 创建 ItemType 的步骤： 1. 从导航窗格，选择 管理 > ItemTypes，将出现以下菜单。 2. 点击「新建 ItemType」。将出现空白 ItemType 窗口。 让我们先查看头部属性。以下是头部属性及其说明列表： L',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [63/244] Creating_a_File.htm (3 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('4931c7638f4e',
   N'添加文件',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">添加文件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Creating_a_File.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">添加文件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：以下步骤仅供管理员使用，用于测试目的。文件项目应从父项目创建。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中选择 管理 &gt; 文件处理 &gt; 文件，将出现以下菜单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「新建文件」。将出现打开对话框。选择适当的文件并点击打开。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a59db2ff5d5b.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击保存。文件被锁定并出现在主窗口的默认页面中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Select the appropriate file and click Open. A message similar to the following appears while the file is being loaded.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f5b3f2289186.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击。文件被锁定并显示在主窗口的默认页面中。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'添加文件 注意：以下步骤仅供管理员使用，用于测试目的。文件项目应从父项目创建。 在导航窗格中选择 管理 > 文件处理 > 文件，将出现以下菜单。 点击「新建文件」。将出现打开对话框。选择适当的文件并点击打开。 点击保存。文件被锁定并出现在主窗口的默认页面中。 Select the appropriate file and click Open. A message similar to the f',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [64/244] Creating_a_FileType.htm (7 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('1384bfca4b9f',
   N'创建文件类型',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建文件类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Creating_a_FileType.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建文件类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中选择 管理 &gt; 文件处理 &gt; FileTypes，将出现以下菜单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「新建 FileType」。将出现新 FileType 窗口。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0d976b738b85.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">在相应字段中输入文件类型信息。扩展名（Ext.）和 MIME 类型字段为必填。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击 View With 选项卡上的「选择项目」按钮，将现有查看器与文件关联。查看器搜索对话框将出现。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a98436813226.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击「运行搜索」查看可用查看器列表。选择查看器并确认。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click the Select Items button on the View With tab to associate an existing Viewer with the file. The Viewer Search dialog box appears.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/81d30419a164.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击「执行搜索」按钮查看可用查看器列表。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3dcf870acda9.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">从列表中选择一个查看器并点击「返回所选」按钮。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6a56781729d6.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击保存并取消认领文件。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ba507963f59.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'创建文件类型 在导航窗格中选择 管理 > 文件处理 > FileTypes，将出现以下菜单。 点击「新建 FileType」。将出现新 FileType 窗口。 在相应字段中输入文件类型信息。扩展名（Ext.）和 MIME 类型字段为必填。 点击 View With 选项卡上的「选择项目」按钮，将现有查看器与文件关联。查看器搜索对话框将出现。 点击「运行搜索」查看可用查看器列表。选择查看器并确认。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [65/244] Creating_a_Form.htm (3 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('b8b9e85e9fb3',
   N'创建表单',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建表单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Creating_a_Form.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建表单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1. 在导航窗格中选择 管理 &gt; 表单，将出现以下菜单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2. 点击「新建表单」。将出现空表单。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f4124098c4de.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">3. 在表单字段中输入适当的信息，然后点击「保存项目」按钮。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/383f5d419e21.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">3. 在表单字段中输入适当的信息，然后点击「保存项目」按钮。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/29c7d82a1260.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'创建表单 1. 在导航窗格中选择 管理 > 表单，将出现以下菜单。 2. 点击「新建表单」。将出现空表单。 3. 在表单字段中输入适当的信息，然后点击「保存项目」按钮。 3. Enter the appropriate information in the form fields and click the Save Item button .',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [66/244] creating_a_life_cycle_map.htm (2 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('106007be71a1',
   N'创建生命周期映射',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建生命周期映射</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: creating_a_life_cycle_map.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建生命周期映射</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">生命周期映射可以独立于任何 ItemType 创建，之后可以附加到 ItemType。多个 ItemType 可以使用同一个生命周期映射。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建步骤：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中选择 管理 &gt; 生命周期映射，将出现以下菜单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「新建生命周期映射」。将出现空白生命周期映射。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6b3eeef39a14.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">填写属性：名称和描述。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4e5e456ee475.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Fill in the Life Cycle Map properties: Name - the name of the map Description- the description of the map</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">名称 - 映射名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">描述 - 映射的描述</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Create a new State by right-clicking anywhere on the map canvas and selecting Add State from the popup menu. A new State is created, and remains selected by default. Each life cycle state has the following properties:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">状态属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">定义</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">状态名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图片</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click the Select an image link to access the Image Browser dialog. Select an icon for the state, then click the Return Selected button to complete your selection.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">已发布</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">This property applies only if this Life Cycle map is associated with an ItemType that is versionable.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">A Boolean to indicate that the status of this state is Released . Items in a state with a Released status are moved back to the Start state upon the next &quot;Lock/Unlock/Edit&quot; sequence, and the Major Rev property is incremented.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">每个生命周期只能有一个状态标记为已发布。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">不可锁定</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">A Boolean; when set to true, the item in this state cannot be edited. Typically this property is set to true when the status of the state is Released. In order to edit the item, it has to be promoted to a new state, which would typically (depending on the life cycle) create a new version.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目行为</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Applicable only if the item going through this life cycle state contains one or more relationships with versionable items. There are four types of Item Behavior, and they are described below. Be aware that the Behavior set by the RelationshipType influences the Behavior set by the state as well. To see a full explanation of how these behaviors interact together, see Item Behavior.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">固定 - 源项目（在当前状态）指向关联项目的指定世代号。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">浮动 - 源项目（在当前状态）指向关联项目的最新世代号。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">硬固定 - 行为为固定（如上所述），不能被此生命周期中的后续状态更改。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">硬浮动 - 行为为浮动（如上所述），不能被此生命周期中的后续状态更改。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">状态权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">A set of permissions may be defined for each state, which will govern the access to the item while it is in this state (see Permissions). The state permissions override any other permissions set on the Item. Promoting to a state changes the Item permissions to match the state permissions. If no state permissions are set, then the Item permissions are not changed.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">For an example of state permissions, look at the default Part Life Cycle map. While a Part is in the Preliminary state All Employees can get and update it. However, once the Part enters the Review state, only members of the Aras PLM and the Administrators identities can update it.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">配置电子邮件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击此链接配置电子邮件，以便在项目进入此状态时通知指定的身份。请参阅配置电子邮件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工作流</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click the button to the right of the Workflow field. Select an allowed Workflow from the pop-up search dialog. The selected Workflow will be initiated when the Life Cycle State is activated. Note: An allowed Workflow is one that is related to the same ItemType to which the Life Cycle is also related.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标签</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">This field defines the label that is displayed within the system for this Life Cycle State. If Innovator is configured for Multilingual support, then this field allows the label to be defined in multiple languages. In order to set a multilingual value for this field, click the button to the right of the Label field. The system will pop-up a dialog for inputting a value in different languages. Refer to the help topics on Internationalization for more information.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">历史模板</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click the button to the right of the History Template field. Select a History Template from the pop-up search dialog. See the help topic on Configurable History for more information</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建转换</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">右键点击您希望创建转换的状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Select Add Transition from the popup menu. A transition is created from the specified state and connected to the mouse. Drag the mouse to the state which you want to connect, and click to connect the transition.</Run></Paragraph>
</FlowDocument>',
   N'创建生命周期映射 生命周期映射可以独立于任何 ItemType 创建，之后可以附加到 ItemType。多个 ItemType 可以使用同一个生命周期映射。 创建步骤： 在导航窗格中选择 管理 > 生命周期映射，将出现以下菜单。 点击「新建生命周期映射」。将出现空白生命周期映射。 填写属性：名称和描述。 Fill in the Life Cycle Map properties: Name - t',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [67/244] Creating_a_List.htm (8 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('f976bc93497e',
   N'创建列表',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建列表</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Creating_a_List.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建列表</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">列表（List）是一种数据类型，包含一系列相似的值，如颜色或制造商名称。当列表用作 ItemType 属性的数据类型时，默认以下拉框形式显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：数据类型设置为颜色列表，数据源指向下面显示的红色色调列表。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c2bd3b03cc52.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">创建列表的步骤：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1. 在导航窗格中选择 管理 &gt; 列表，将出现以下菜单。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b93ceabec29a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">创建列表：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1. 在导航窗格中，选择 管理&gt;列表。将出现以下菜单：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e6c4415e1ade.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">2. 点击「创建新列表」。将出现空白列表窗口。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9c05139fd87a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">3. 输入列表的名称和描述。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">4. 点击「添加行」图标在值选项卡上添加行。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/09fc4497a977.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b625b9f848c3.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">5. Enter the Label, Value, and Sort Order of the entry. If you are doing a color list, you may want to refer to http://www.lynda.com/hex.html# web site for the hex numbers of colors. Remember that these entries will represent possible property values for a specific ItemType. Label, Value, and Sort Order control how the drop-down box looks on the form. Here is an example:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/631320963155.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Label - displayed in a drop-down box, on the form of the item that uses this list as a property for the user to select. If additional languages are installed in a database Labels for each of the additional languages should be added for Lists that are created or modified. See Internationalization.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">值 - 用户选择后属性将被分配的实际值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Sort Order - the order in which these entries will appear in the drop-down box (top to bottom).Sort Order</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">6. 点击保存并取消认领列表。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">.</Run></Paragraph>
</FlowDocument>',
   N'创建列表 列表（List）是一种数据类型，包含一系列相似的值，如颜色或制造商名称。当列表用作 ItemType 属性的数据类型时，默认以下拉框形式显示。 注意：数据类型设置为颜色列表，数据源指向下面显示的红色色调列表。 创建列表的步骤： 1. 在导航窗格中选择 管理 > 列表，将出现以下菜单。 To create a list: 1. In the Navigation pane, select ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [68/244] Creating_a_List1.htm (8 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('e2ec42a71e8b',
   N'创建列表（续）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建列表（续）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Creating_a_List1.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建列表</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">列表是一种数据类型，包含一系列相似的值。当列表用作属性的数据类型时，以下拉框形式显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：数据类型设置为颜色列表，数据源指向红色色调列表。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/944018860a39.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">创建列表的步骤：从目录中，选择 管理 &gt; 列表，选择新建项目图标。新列表表单将显示。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f8d0a396a1e2.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">创建列表：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">From TOC, Administration --&gt; Lists, select the New Item icon . The new list form will be displayed.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7d45a9287528.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/00230b7308ab.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">输入列表的名称和描述。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click the New Item icon in the list toolbar, next to the Actions box. This creates a new line in the list members grid:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7d45a9287528.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/58ffaa88fc83.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Enter the Label, Value, and Sort Order of the entry. If you are doing a color list, you may want to refer to http://www.lynda.com/hex.html# web site for the hex numbers of colors. Remember that these entries will represent possible property values for a specific ItemType. Label, Value, and Sort Order control how the drop-down box looks on the form. Here is an example:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/631320963155.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Labels - displayed in a drop-down box, on the form of the item that uses this list as a property for the user to select. If additional languages are installed in a database Labels for each of the additional languages should be added for Lists that are created or modified. See Internationalization.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Values - the actual values that the property will have assigned to it, once selected by the user.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">排序顺序 - 这些条目在下拉框中出现的顺序（从上到下）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存、解锁并关闭列表。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/370db5d605f2.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'创建列表 列表是一种数据类型，包含一系列相似的值。当列表用作属性的数据类型时，以下拉框形式显示。 注意：数据类型设置为颜色列表，数据源指向红色色调列表。 创建列表的步骤：从目录中，选择 管理 > 列表，选择新建项目图标。新列表表单将显示。 To create a list: From TOC, Administration --> Lists, select the New Item icon .',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [69/244] Creating_a_Markup_Comment.htm (5 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('c8053bd29bd2',
   N'创建快照评论',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建快照评论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Creating_a_Markup_Comment.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建快照评论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">快照评论是包含文件快照和文本评论的评论。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：快照评论可以在有或无批注的情况下创建。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建快照评论的步骤：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击项目弹出窗口侧边栏中的查看器或其关联文件，以查看模式显示文件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用查看器工具栏将文件调整到所需的查看状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">通过点击批注工具栏切换到批注模式。这将冻结当前查看状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">确保「包含图片」复选框已开启。现在用户创建的消息将包含查看器中当前显示的图片。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/27225ddd1e4e.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">In the markup mode, create any markup notations (if desired) using the markup toolbar. For more information on the toolbar options, refer to Markup Toolbar.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f68eeb187c4b.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Enter a text comment (if desired) in the discussion toolbar.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击评论保存批注评论。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The new message which includes a thumbnail of the image with the markups is displayed in the discussion panel. The Include Image checkbox is removed and the viewer returns to view mode.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/20fbd06728c6.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">If the user decides not to save the markup notations, then he can click on the View button at anytime to exit from the markup mode.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">缩略图显示</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The thumbnail of the markup image is displayed in the discussion panel along with the comment. This can be expanded and collapsed to view the markups more clearly.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click on the &quot;expand/collapse&quot; icon in the upper left hand corner to expand or collapse the thumbnail in a snapshot style within the discussion panel.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c44fbc0eb0b2.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Clicking on the thumbnail itself displays the markup image in the viewer.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/aec4d7aa4cae.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'创建快照评论 快照评论是包含文件快照和文本评论的评论。 注意：快照评论可以在有或无批注的情况下创建。 创建快照评论的步骤： 点击项目弹出窗口侧边栏中的查看器或其关联文件，以查看模式显示文件。 使用查看器工具栏将文件调整到所需的查看状态。 通过点击批注工具栏切换到批注模式。这将冻结当前查看状态。 确保「包含图片」复选框已开启。现在用户创建的消息将包含查看器中当前显示的图片。 In the marku',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [70/244] Creating_a_Method.htm (4 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('9a9ea73a6592',
   N'创建方法',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建方法</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Creating_a_Method.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建方法</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中点击 管理 &gt; 方法，将出现以下菜单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「新建方法」。方法窗口将出现。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4d8e2251a647.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">有关使用和创建方法的信息，请参阅 Aras Innovator 程序员指南。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击保存并解锁方法。新操作现在出现在主窗口的默认页面中。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/23666cc15d94.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">For information about using and creating methods, refer to the Aras Innovator Programmer&apos;s Guide.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click the icon to save and unclaim the method. The new action now appears in the default page in the main window.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/192f3e52bb23.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'创建方法 在导航窗格中点击 管理 > 方法，将出现以下菜单。 点击「新建方法」。方法窗口将出现。 有关使用和创建方法的信息，请参阅 Aras Innovator 程序员指南。 点击保存并解锁方法。新操作现在出现在主窗口的默认页面中。 For information about using and creating methods, refer to the Aras Innovator Progr',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [71/244] creating_a_new_item.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('fafb6f69a761',
   N'通过多态项目创建新项目',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">通过多态项目创建新项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: creating_a_new_item.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">通过多态项目创建新项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当关联项目使用多态项目（Poly Item）作为源时，用户在创建新关联项目时将看到额外的对话框。创建新关联项目时，必须首先选择要使用的多态源类型。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择项目类型后，其余创建过程应该很熟悉。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/8924ae101053.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Select the Type of Item, and the rest of the process of Item creation should be familiar.</Run></Paragraph>
</FlowDocument>',
   N'通过多态项目创建新项目 当关联项目使用多态项目（Poly Item）作为源时，用户在创建新关联项目时将看到额外的对话框。创建新关联项目时，必须首先选择要使用的多态源类型。 选择项目类型后，其余创建过程应该很熟悉。 Select the Type of Item, and the rest of the process of Item creation should be familiar.',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [72/244] creating_a_permission.htm (7 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('95ac2a33fa4a',
   N'创建权限',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: creating_a_permission.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您可以创建权限然后分配给 ItemType，也可以直接从 ItemType 的权限选项卡创建权限。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建权限的步骤：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在目录中选择 管理 &gt; 权限 &gt; 新建权限，将出现以下窗口。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">输入权限名称。名称通常描述性地说明正在创建的权限类型或权限将应用到的项目。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/709ff953dd0f.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">下一步是指定访问权限。请注意，Can Add 权限和 TOC Access 权限不在此处的权限网格中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The next step is to specify the access rights. The access settings are described in the table below. Notice that the Can Add Permission as well as the TOC Access permission are not available within the Permission item. These two are set from the ItemType directly. Refer to Can Add and TOC Access respectively to learn how to set these permissions.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">特权</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">定义</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Get</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您检索/查看项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">更新</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您编辑现有项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您删除项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可以发现</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Enables you to learn if an item exists. Refer to the Discovery Privilege</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可以更改访问权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Enables you to change access settings for a created instance of the item</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示权限警告</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Works with Can Discover to determine if a warning is displayed to the user when items with out discover privilege should be returned in a search. If this is set, a warning will be displayed other wise the user will not know if items are missing due to insufficient privilege</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">目录访问</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Enables the user to have access to the item through the TOC (table of contents)</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可添加</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许用户创建项目的实例</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">输入访问值：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Notice the drop-down box next to the Actions field. You can select either Pick Related or Create Related. Choosing Pick Related brings up the Identity Search dialog box. Selecting Create Related enables you to create a new identity. This example uses the Pick Related option.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/503fda9cfc3a.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Select an identity from the list and click the green arrow to add it to the Access tab. The selected Identity appears on the Access tab of the Permissions item. Select the privileges that you wish this identity to have by checking the boxes of the corresponding privileges.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">If you selected Create Related from the drop down box, a new empty line will be created for you. You can type in the name of a new identity, that you will be creating on a fly, and add the privileges that you want this new identity to have.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/35f5bfc7dd11.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">保存、解锁并关闭权限。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">You can set a permission to give different privileges to many different identities. Don&apos;t forget that identities can be groups and individuals, and if an individual identity is contained in more than one group identity, its privileges are cumulative.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">To connect a permission to an ItemType:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1. From TOC, Administration, select ItemTypes and find the ItemType to which you want to assign Permissions.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2. Open the ItemType for edit, and select the Permissions tab.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">3. If you have already created a permission, then click the Select Items button , select the appropriate permission from the Permission dialog that appears and click the Return Selected icon .</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/81d30419a164.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6a56781729d6.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">4. If you have not yet created a permission, you can create one on the fly, by choosing the Create Item icon on the Permissions tab. You will be guided through the same steps outlined earlier, except that the permission will already be assigned to the ItemType.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/93ef1556cc54.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">6. 点击保存 ItemType。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'创建权限 您可以创建权限然后分配给 ItemType，也可以直接从 ItemType 的权限选项卡创建权限。 创建权限的步骤： 在目录中选择 管理 > 权限 > 新建权限，将出现以下窗口。 输入权限名称。名称通常描述性地说明正在创建的权限类型或权限将应用到的项目。 下一步是指定访问权限。请注意，Can Add 权限和 TOC Access 权限不在此处的权限网格中。 The next step i',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [73/244] creating_a_poly_item_item_type.htm (8 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('82222bf35ef8',
   N'创建多态项目 ItemType',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建多态项目 ItemType</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: creating_a_poly_item_item_type.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建多态项目 ItemType</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用以下步骤：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中点击 管理 &gt; ItemType，将出现以下菜单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「新建 ItemType」。将出现空白 ItemType 窗口。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/32c5415833d5.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">为新 ItemType 输入头部属性数据，实现类型选择 Poly Item。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ac790ba9d124.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Enter data for the header properties of your new Item Type, selecting Poly Item for the implementation type.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ce6dafeed92b.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">You will receive the following warning.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d7a695149aec.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Note that many of the relationship tabs are disabled when selecting the Poly Item type</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击确定。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click the “Poly Sources” relationship tab</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5c95960bc3a6.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Select existing ItemTypes that the Poly Item will support. For example, the following could be a list for the Deliverable Item Type.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3f0679208975.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Select the Properties tab. Enter any common property names that you would like to display in selection search item browser grids. If a property name is added that does not exist in all the Poly Sources, then an error similar to the following will be thrown when saving the ItemType.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b736033bfda5.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击保存并取消认领项目。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'创建多态项目 ItemType 使用以下步骤： 在导航窗格中点击 管理 > ItemType，将出现以下菜单。 点击「新建 ItemType」。将出现空白 ItemType 窗口。 为新 ItemType 输入头部属性数据，实现类型选择 Poly Item。 Enter data for the header properties of your new Item Type, selecting ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [74/244] Creating_a_Revsion.htm (4 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('caa9c5058fa4',
   N'创建修订',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建修订</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Creating_a_Revsion.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建修订</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建新修订：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从导航窗格，选择 管理 &gt; 修订，将出现以下菜单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「新建修订」。将出现空白修订窗口。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6302bd786f94.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">填写以下属性：名称（修订的名称）、修订（要使用的新修订名称系列，这些名称应该是连续的且容易识别顺序）。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c2f8bb0b45e5.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c2f8bb0b45e5.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Fill out the following properties: Name - the name of the revision. Revisions - the series of new revision names to be used. These names should be sequential and easily recognized in terms of order and increment.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">名称 - 修订版本的名称。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Revisions - the series of new revision names to be used. These names should be sequential and easily recognized in terms of order and increment.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击保存并取消认领此项目。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'创建修订 创建新修订： 从导航窗格，选择 管理 > 修订，将出现以下菜单。 点击「新建修订」。将出现空白修订窗口。 填写以下属性：名称（修订的名称）、修订（要使用的新修订名称系列，这些名称应该是连续的且容易识别顺序）。 Fill out the following properties: Name - the name of the revision. Revisions - the series',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [75/244] Creating_a_Sequence.htm (5 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('bc1298aa4329',
   N'创建序列',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建序列</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Creating_a_Sequence.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建序列</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">序列用于自动编号，可用于为文档、零部件、项目等创建全面的唯一标识符。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">查看预定义序列：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中，选择 管理 &gt; 序列，将出现以下菜单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「搜索序列」查看现有序列列表。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a389c358047c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">创建序列</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f3b699840cc7.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">创建序列</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click Create New Sequence. A blank Sequence window appears:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/2eb3550877d1.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">在以下字段中输入信息：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">名称 - 序列项目的名称。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Prefix - the alphanumeric prefix of the generated number.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Initial Value - used to set the Current Value on import if none exists in the imported data.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Current Value - the initial value of the sequence.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Suffix - similar to Prefix, except added to the end of the number.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Pad With - specify the character to fill in the required number of spaces.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Pad To - the total number of characters for the number, excluding the prefix and the suffix.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Step - the increment by which the number differs from the previous and next.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The following examples show how these properties work to together to create a sequence configuration.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例1</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Example 1 generates the following sequence: {ECN-xxxx1002, ECN-xxxx1003, ECN-xxxx1004, ...)</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7f75287e36b2.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">示例2</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Example 2 generates the following sequence: {RPT-001010-sprt , RPT-001020-sprt, RPT-001030-sprt, ...)</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/556c75051d0e.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'创建序列 序列用于自动编号，可用于为文档、零部件、项目等创建全面的唯一标识符。 查看预定义序列： 在导航窗格中，选择 管理 > 序列，将出现以下菜单。 点击「搜索序列」查看现有序列列表。 创建序列 Creating a Sequence Click Create New Sequence. A blank Sequence window appears: Enter information in ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [76/244] Creating_a_Sidebar_Button.htm (15 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('a948d6ec7c1c',
   N'创建侧边栏按钮',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建侧边栏按钮</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Creating_a_Sidebar_Button.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建侧边栏按钮</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">完成图形导航视图的侧边栏创建后，使用以下步骤创建侧边栏按钮。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在 DemoIT_TOC_Configuration 项目的 Command Bar Section 选项卡中右键点击 MySidebar 并选择打开。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击 CommandBarItem 选项卡上的「新建命令栏」图标。选择项目类型对话框将出现。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/df9d2e9dfa0e.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">选择 Button 并确认。一行将添加到命令栏项目选项卡中。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e6e5913a54af.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6b4f78eae1e8.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Select Button and click . A row is added to the Command Bar Item tab.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ad42022d1a90.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7ef7338abbf7.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click the ellipses in the For Identity column and select World.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Select Add from the dropdown list in the Action field and enter a name for the button in the Name field.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/12f1c59b70ea.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Right-click on MyGV and click Open in the dropdown window. The MyGV item appears.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click the ellipses in the Click Method field. The Select Items dialog for Methods appears.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/fed91f9dfe48.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Select sidebar_default_gv_click and click .</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ad42022d1a90.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Select an image for the button by clicking the Select an image link, selecting an image in the Image Browser and clicking the green arrow .</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/8e9518bd87b6.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Add information about the Graph View definition ID in the Additional Data field by clicking the More button and selecting Properties from the dropdown menu. The Graph View Definition dialog appears:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/eeb4e2a96040.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f56bc573ae62.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click Copy ID and paste the information into the Additional Data field:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/07ca1bb340da.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click to save and unclaim the items.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">When you have finished creating the sidebar and sidebar button, the end user would see something like the following:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/bee2da1f2825.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Clicking the sidebar button displays the graph view that you configured in the button definition:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/2eeff4402251.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'创建侧边栏按钮 完成图形导航视图的侧边栏创建后，使用以下步骤创建侧边栏按钮。 在 DemoIT_TOC_Configuration 项目的 Command Bar Section 选项卡中右键点击 MySidebar 并选择打开。 点击 CommandBarItem 选项卡上的「新建命令栏」图标。选择项目类型对话框将出现。 选择 Button 并确认。一行将添加到命令栏项目选项卡中。 Select',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [77/244] Creating_a_Sidebar_for_a_Graph_Navigation_View.htm (7 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('0d9c979c5ccf',
   N'为图形导航视图创建侧边栏',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">为图形导航视图创建侧边栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Creating_a_Sidebar_for_a_Graph_Navigation_View.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">为图形导航视图创建侧边栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">管理员可以配置侧边栏和按钮，使用户能够通过点击按钮打开特定的图形导航视图。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建侧边栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择要用于配置侧边栏的 ItemType。以下示例使用 DemoIT ItemType。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击打开 ItemType 进行编辑。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/eaf68ab55cfa.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击「Client Style」选项卡和「新建演示配置」按钮创建演示配置。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e6e5913a54af.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">输入演示配置名称并确认。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/318e9eeabebd.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">输入显示配置名称并点击。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Double-click the Presentation Configuration name to view the item:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/19eb677f1e0a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click the ellipses in the Classification field and select Data Model from the Classification dialog box.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Specify the Name and Location of the Command Bar in the appropriate fields. You can also specify the sort order and an Identity to associate with the Presentation Configuration as shown in the following screenshot.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b17a67ef5ee5.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click to save the command bar information.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'为图形导航视图创建侧边栏 管理员可以配置侧边栏和按钮，使用户能够通过点击按钮打开特定的图形导航视图。 创建侧边栏 选择要用于配置侧边栏的 ItemType。以下示例使用 DemoIT ItemType。 点击打开 ItemType 进行编辑。 点击「Client Style」选项卡和「新建演示配置」按钮创建演示配置。 输入演示配置名称并确认。 Enter the Presentation Conf',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [78/244] Creating_a_Variable.htm (4 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('7ac0b2658ef7',
   N'创建变量',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建变量</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Creating_a_Variable.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建变量</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1. 在导航窗格中选择 管理 &gt; 变量，将出现以下菜单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2. 点击「新建变量」。将出现空白变量窗口。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d222855f6181.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">3. 在变量字段中输入适当的信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">4. 点击保存并解锁变量。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0c679f3b95a4.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">3. Enter the appropriate information in the Variable fields.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">4. Click and to save and unclaim the variable.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ba507963f59.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'创建变量 1. 在导航窗格中选择 管理 > 变量，将出现以下菜单。 2. 点击「新建变量」。将出现空白变量窗口。 3. 在变量字段中输入适当的信息。 4. 点击保存并解锁变量。 3. Enter the appropriate information in the Variable fields. 4. Click and to save and unclaim the variable.',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [79/244] Creating_a_Vault.htm (11 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('6f0c1d9e83ef',
   N'创建保险柜',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建保险柜</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Creating_a_Vault.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建保险柜</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">安装期间，Innovator 在 Innovator 服务器中创建默认文件保险柜目录（通常命名为 VaultServer），并创建提供对该目录访问的虚拟目录。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您可以在不同位置或不同服务器上建立额外的保险柜。每个保险柜位置通过唯一名称引用，并有自己的虚拟目录。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中选择 管理 &gt; 文件处理 &gt; 保险柜，将出现以下菜单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「新建保险柜」。将出现空白保险柜屏幕。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e35caaf8d426.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">在相应字段中输入保险柜名称、URL 模式和 URL。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5ca8ec972674.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">在相应字段中输入保险库名称、URL 模式和 URL。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click the Select Items button on the Replication Rules tab to add an identity. The Identity Search dialog appears:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/81d30419a164.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6e5ce8d9c2a5.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click the Search button to search for the appropriate identity.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5da5ed34617a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Select an identity and click the Return Selected button to add it to the Replication Rule tab, as shown in the following example:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6a56781729d6.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5f1309948abd.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click the Conversion Server Priority tab to add a conversion server to the vault.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click the Select Items button. The Conversion Server search dialog appears:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5ed90c74c35a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click the Run Search icon to see a list of available conversion servers.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Select the appropriate conversion server and click the Return Selected button to add it to the Conversion Server Priority tab, as shown in the following example:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6a56781729d6.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c3e49b57f789.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click to save and unclaim the vault.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'创建保险柜 安装期间，Innovator 在 Innovator 服务器中创建默认文件保险柜目录（通常命名为 VaultServer），并创建提供对该目录访问的虚拟目录。 您可以在不同位置或不同服务器上建立额外的保险柜。每个保险柜位置通过唯一名称引用，并有自己的虚拟目录。 在导航窗格中选择 管理 > 文件处理 > 保险柜，将出现以下菜单。 点击「新建保险柜」。将出现空白保险柜屏幕。 在相应字段中输',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [80/244] Creating_a_Version.htm (10 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('cbce0e9ba67a',
   N'创建版本',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建版本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Creating_a_Version.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建版本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建或编辑项目时，您就创建了该项目的版本。在此示例中，我们使用 Part J-100002。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中选择 设计 &gt; 零部件，将出现以下菜单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「搜索零部件」。零部件搜索网格将出现。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e98648c611da.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">右键点击 J-100002 行并从上下文菜单中点击「版本」。项目版本弹出框将出现。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e7f4edb2f221.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Right click on the J-100002 row and click Versions in the context menu that appears. The Item Versions pop-up box appears.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9b3845573b7b.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击打开项目进行编辑。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/eaf68ab55cfa.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click the Select Items button on the BOM tab. The Part search dialog appears.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/81d30419a164.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f67908a2709a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Select a part to add to the BOM and click the Return Selected button .</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6a56781729d6.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击保存并关闭项目。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click the Navigate button and select Versions from the context menu. The Item versions pop-up displays the new version information.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/59eb9549a5fc.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4b9814f84326.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'创建版本 创建或编辑项目时，您就创建了该项目的版本。在此示例中，我们使用 Part J-100002。 在导航窗格中选择 设计 > 零部件，将出现以下菜单。 点击「搜索零部件」。零部件搜索网格将出现。 右键点击 J-100002 行并从上下文菜单中点击「版本」。项目版本弹出框将出现。 Right click on the J-100002 row and click Versions in the',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [81/244] Creating_a_Workflow1.htm (4 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('b831d7a38ea9',
   N'创建工作流映射',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建工作流映射</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Creating_a_Workflow1.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建工作流映射</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建工作流映射包括创建活动并用路径将它们连接起来。在这里您将学习如何打开新的工作流映射以及如何创建活动和路径来填充它。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开新的工作流映射表单：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从导航窗格中，选择 管理 &gt; 工作流映射。将出现以下菜单：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「创建工作流映射」。将出现空白工作流映射窗口。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b456e9212745.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">工作流映射的头部属性如下：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">名称 - 工作流映射的名称</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a5686d16d2bc.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">标签 - 显示在项目上的工作流映射名称。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">描述 - 映射中描述的过程说明。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">流程负责人 - 负责此流程的身份，通常是组身份。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工作流映射本身有两个选项卡：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">活动 - 此选项卡将列出映射中创建的所有活动及其部分属性。创建、删除和修改这些活动的界面通过图形设计器完成。此选项卡上的信息仅作摘要。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">流程变量 - 需要添加到整个映射的变量列表。这些变量可以作为循环的属性以编程方式访问。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建活动或节点</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">右键点击映射背景，从弹出菜单中选择「添加活动」。一个新节点将放置在映射上并自动被选中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">按照「工作流活动」中的说明填写头部属性和选项卡。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建工作流路径</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">右键点击您希望路径开始的活动。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7a263388ab23.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">从弹出菜单中选择「添加路径」。路径矢量将连接到鼠标。指向路径将结束的活动，然后点击。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您应该看到路径连接了所选的活动。再次点击路径以选中它，注意底部两个窗格中的属性发生变化：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">活动选项卡列出了与映射关联的工作流活动。与活动关联的优先级编号指示活动的顺序。流程变量选项卡使您能够向工作流添加上下文信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">按照「工作流路径」中的说明和图示填写此路径的属性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">You should see the path connect the chosen activities. Click on the path again to select it, and notice the properties change in the bottom two panes:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5753e2148c7e.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The Activity tab lists the workflow activities associated with the map. The Priority numbers associated with the activites indicate the order of the activities. The Process variables tab enables you to add context information to a workflow.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Fill out the properties of this path as described and shown in Workflow Path.</Run></Paragraph>
</FlowDocument>',
   N'创建工作流映射 创建工作流映射包括创建活动并用路径将它们连接起来。在这里您将学习如何打开新的工作流映射以及如何创建活动和路径来填充它。 打开新的工作流映射表单： 从导航窗格中，选择 管理 > 工作流映射。将出现以下菜单： 点击「创建工作流映射」。将出现空白工作流映射窗口。 工作流映射的头部属性如下： 名称 - 工作流映射的名称 标签 - 显示在项目上的工作流映射名称。 描述 - 映射中描述的过程说',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [82/244] Creating_E-Signature.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('31206bde4448',
   N'创建电子签名',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建电子签名</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Creating_E-Signature.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建电子签名</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">电子签名与密码是分开的。电子签名是由用户维护的用户控制密码，而非 Innovator 管理员或 IT 密码。这使得每个用户可以完全控制自己的电子签名。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1. 登录 Aras Innovator。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2. 导航到 工具 --&gt; 首选项 --&gt; 更改电子签名。将出现更改电子签名对话框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">3. 在「旧密码」字段中输入当前电子签名。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7767816692fb.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">4. 在「新密码」和「确认新密码」字段中输入新密码。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">5. 点击确定。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果您是首次设置电子签名，请将旧签名字段留空。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">If you are setting the E-Signature for the first time, leave the Old Signature field blank.</Run></Paragraph>
</FlowDocument>',
   N'创建电子签名 电子签名与密码是分开的。电子签名是由用户维护的用户控制密码，而非 Innovator 管理员或 IT 密码。这使得每个用户可以完全控制自己的电子签名。 1. 登录 Aras Innovator。 2. 导航到 工具 --> 首选项 --> 更改电子签名。将出现更改电子签名对话框。 3. 在「旧密码」字段中输入当前电子签名。 4. 在「新密码」和「确认新密码」字段中输入新密码。 5. ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [83/244] Creating_No_Related_Relationship.htm (5 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('36bba2dc22db',
   N'创建无法添加关联项目的关系',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建无法添加关联项目的关系</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Creating_No_Related_Relationship.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建无法添加关联项目的关系</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系包含一个源项目和一个可选的关联项目。当未指定关联项目时，该关系称为空关系。在这种情况下，关系项目存储有关关系和源项目的更多信息。让我们从源（父）项目创建一个无关联的关系。因为我们创建了关系，所以在源（父）项目实例中有一个关系选项卡。但是，没有关联项目可以连接。在这种情况下，关系项目存储有关关系本身和源项目的更多信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">例如，让我们考虑列表 ItemType。列表 ItemType 存储您创建的列表项目的各种值。如果您创建一个名为「颜色」的新列表项目，该关系可以存储颜色的各种值，如红色、紫色等。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建无法添加关联项目的关系：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">登录 Aras Innovator。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从导航窗格中，选择 管理 &gt; 列表。将出现以下菜单：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「搜索列表」。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e6c4415e1ade.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">从主网格中，选择要添加关系的源（父）项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用鼠标右键（右键点击）打开上下文菜单并选择「打开」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将出现项目窗口。在我们的示例中，打开「颜色列表」项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在「值」选项卡上，点击关系工具栏以添加关系。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：「无关联」选项已被选中，且是添加关系的唯一可用选项。「无关联」选项不一定总是启用，这取决于管理员所做的 ItemType 配置。根据管理员的配置，一个或所有操作已启用并可用。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3ebb0af419c2.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">关系网格中将添加一个空白行。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/09fc4497a977.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">输入颜色名称、颜色值和排序顺序。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击保存并解锁项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">输入颜色名称、颜色值和排序顺序。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7669965dc022.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击保存并取消认领项目。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'创建无法添加关联项目的关系 关系包含一个源项目和一个可选的关联项目。当未指定关联项目时，该关系称为空关系。在这种情况下，关系项目存储有关关系和源项目的更多信息。让我们从源（父）项目创建一个无关联的关系。因为我们创建了关系，所以在源（父）项目实例中有一个关系选项卡。但是，没有关联项目可以连接。在这种情况下，关系项目存储有关关系本身和源项目的更多信息。 例如，让我们考虑列表 ItemType。列表 I',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [84/244] Defining_a_Relationship_ItemType.htm (6 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('e7092100992c',
   N'创建带有关联项目的关系',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建带有关联项目的关系</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Defining_a_Relationship_ItemType.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建带有关联项目的关系</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系绑定两个项目 - 源（父）项目和关联（子）项目，使源（父）项目能够引用关联项目中保存的信息。关系的创建是从源（父）项目开始，创建一个到关联（子）项目的链接。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在这种关系类型中，必须添加关联项目。您可以添加现有项目或创建新项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例：让我们以 Part ItemType 作为源项目定义一个 Part-BOM 关系。在我们的示例中，让我们向椅子项目（Part）添加关联（子）项目，如螺母、座垫、扶手。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建带有关联项目的关系：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">登录 Aras Innovator。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从目录中，导航到源（父）项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">让我们在此示例中将 Part ItemType 作为源（父）项目。从导航窗格中选择 设计 &gt; 零部件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从主网格中，选择要添加关系的源（父）项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在 BOM 选项卡上，点击（选择项目）。将出现搜索对话框：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/24b31b9000cc.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">选择要添加到源（父）项目（椅子）的源（父）项目。BOM 选项卡中将出现一个新行。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/81d30419a164.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">您需要为关联（子）项目属性提供值。在我们的示例中，序号属性是自动填充的。自动填充取决于系统属性。我们需要为以下属性提供值：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/8320f1a87adc.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">零部件编号</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">修订</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/cc4105504ad1.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">数量</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：每个项目的属性取决于 ItemType，并由管理员配置。因此，每个 ItemType 的属性可能不同，具体取决于管理员所做的配置。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击保存。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">数量</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Note: The Properties for each Item depend on the ItemType and are configured by the administrator. Thus, the properties for each ItemType may differ, depending on the configuration done by the administrator.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ba507963f59.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/418ab6b11615.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'创建带有关联项目的关系 关系绑定两个项目 - 源（父）项目和关联（子）项目，使源（父）项目能够引用关联项目中保存的信息。关系的创建是从源（父）项目开始，创建一个到关联（子）项目的链接。 在这种关系类型中，必须添加关联项目。您可以添加现有项目或创建新项目。 示例：让我们以 Part ItemType 作为源项目定义一个 Part-BOM 关系。在我们的示例中，让我们向椅子项目（Part）添加关联（子',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [85/244] Defining_NULL_Relationship.htm (9 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('7f3206f15d7d',
   N'创建带有可选关联项目的关系',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建带有可选关联项目的关系</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Defining_NULL_Relationship.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建带有可选关联项目的关系</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系包含一个源项目和一个可选的关联项目。当未指定关联项目时，该关系称为空关系，它在不关联关联项目的情况下存储有关源项目的更多信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">让我们看看 Part ItemType 的替代方案的可能用例。对于任何给定的零部件，可能有替代零部件可以替换它。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建带有可选关联项目的关系</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1. 登录 Aras Innovator。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2. 在导航窗格中，选择 管理 &gt; ItemTypes 并导航到源（父）项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">让我们在此示例中将 Part ItemType 作为源（父）项目。将出现以下菜单：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">3. 点击「搜索零部件」。从主网格中，选择要添加关系的源（父）项目。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e98648c611da.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">4. 使用鼠标右键（右键点击）打开上下文菜单并选择「编辑」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将出现项目窗口。在我们的示例中，让我们打开名称为「椅子」的 Part 项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">6. 点击「替代件」选项卡。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">对于任何给定的零部件，可能有替代零部件可以替换它。如果零部件1是零部件2的替代件，那么零部件1可以在每个装配体或使用零部件2的任何其他位置代替零部件2使用。对于无关联项目（空关系），在关系工具栏的操作中选择「无关联」。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0abe1c3b0846.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">「无关联」选项不一定总是启用，这取决于管理员所做的 ItemType 配置。根据管理员的配置，一个或所有操作已启用并可用。例如，要创建 Part-BOM 关系，它仅提供「创建关联」或「选择关联」两个选项，使得必须添加关联（子）项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">7. 点击关系工具栏以添加关系。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系网格中将添加一个空白行。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当您添加关系而不添加关联项目（使用「无关联」）时，关系网格会填充有关关系的信息。在图2中，图标表示关系中没有关联的项目。关系网格中属于关联项目的列为空白。属于关系项目的列已正确填充。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/93ef1556cc54.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">8. 要为行填充关联项目，点击。将出现搜索对话框。搜索要关联的现有项目并点击。有关如何搜索项目的信息，请参阅简单搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">行的值从您选择的项目中选取。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/114a78754453.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">关系项目的属性取决于管理员所做的配置。因此，每个 ItemType 的属性可能不同，具体取决于管理员所做的配置。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/93ef1556cc54.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">在关系网格中，第二行和第三行显示使用「选择关联」添加的关联项目的属性。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/81d30419a164.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/65c75ab80093.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">9. 点击保存项目并解锁。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The properties for the Relationship Item depend on the configuration done by the administrator. Thus, the properties for each ItemType may differ, depending on the configuration done by the administrator.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d6dc5bbd8629.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">In the relationship grid, the second and third row displays properties of the related Item added using Pick Related.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">9. 点击保存项目并取消认领。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'创建带有可选关联项目的关系 关系包含一个源项目和一个可选的关联项目。当未指定关联项目时，该关系称为空关系，它在不关联关联项目的情况下存储有关源项目的更多信息。 让我们看看 Part ItemType 的替代方案的可能用例。对于任何给定的零部件，可能有替代零部件可以替换它。 创建带有可选关联项目的关系 1. 登录 Aras Innovator。 2. 在导航窗格中，选择 管理 > ItemTypes',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [86/244] Deleting_an_Action.htm (6 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('873fd53ccd7c',
   N'删除操作',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">删除操作</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Deleting_an_Action.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除操作</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中，选择 管理 &gt; 操作。将出现以下菜单：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「搜索操作」。将出现搜索网格。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7645ed490026.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击查看可用操作列表。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">右键点击相应的操作以访问上下文菜单：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9bcf17c7b30c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">选择「删除所有版本」。将出现以下弹出窗口：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0adc3514bab7.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击确定。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：当确认弹出窗口后出现错误消息时，表示项目未被删除。错误消息中将显示 ItemType 对应的属性名称 [itemtype_name property_name}，必须从 ItemTypes 中删除该属性。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/034f156aac25.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Select Delete All Versions. The following pop-up window appears:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ae101ffe0b39.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击确定。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Note: When an error message appears after the confirmation pop-up window the item has not been deleted. The name of the property corresponding to the ItemType appears in the error message as [itemtype_name property_name} and must be deleted from ItemTypes.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7d330943e475.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'删除操作 在导航窗格中，选择 管理 > 操作。将出现以下菜单： 点击「搜索操作」。将出现搜索网格。 点击查看可用操作列表。 右键点击相应的操作以访问上下文菜单： 选择「删除所有版本」。将出现以下弹出窗口： 点击确定。 注意：当确认弹出窗口后出现错误消息时，表示项目未被删除。错误消息中将显示 ItemType 对应的属性名称 [itemtype_name property_name}，必须从 Ite',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [87/244] Deleting_a_FileType.htm (5 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('4423fd713b48',
   N'删除文件类型',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">删除文件类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Deleting_a_FileType.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除文件类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中，选择 管理 &gt; 文件处理。将出现以下菜单：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0d976b738b85.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击「搜索文件类型」。将出现搜索网格。点击查看文件类型列表。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9bcf17c7b30c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">右键点击要删除的文件类型以查看上下文菜单。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/bb6c9e220cea.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击「删除所有版本」。将出现类似以下的对话框：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击确定。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/034f156aac25.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click Delete All Versions. A dialog box similar to the following appears:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/75cb2b2dc5e1.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击确定。</Run></Paragraph>
</FlowDocument>',
   N'删除文件类型 在导航窗格中，选择 管理 > 文件处理。将出现以下菜单： 点击「搜索文件类型」。将出现搜索网格。点击查看文件类型列表。 右键点击要删除的文件类型以查看上下文菜单。 点击「删除所有版本」。将出现类似以下的对话框： 点击确定。 Click Delete All Versions. A dialog box similar to the following appears: Click O',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [88/244] Deleting_a_Form.htm (5 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('42d038f00a09',
   N'删除表单',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">删除表单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Deleting_a_Form.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除表单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从导航窗格中，选择 管理 &gt; 表单 &gt; 搜索表单。将出现搜索网格。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击查看表单列表。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9bcf17c7b30c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">右键点击网格中的表单行以查看上下文菜单。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e67509ffef2d.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击「删除所有版本」。将出现类似以下的对话框：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击确定。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/034f156aac25.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">注意：当弹出窗口后出现错误消息时，表示项目未被删除。错误消息中将显示 ItemType 对应的属性名称 [itemtype_name property_name}，必须从 ItemTypes 中删除该属性。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/75cb2b2dc5e1.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击确定。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Note: When an error message appears after the pop-up window it means that the item has not been deleted. The name of the property corresponding to the ItemType appears in the error message as [itemtype_name property_name} and must be deleted from ItemTypes.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/27ebf7aa1644.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'删除表单 从导航窗格中，选择 管理 > 表单 > 搜索表单。将出现搜索网格。 点击查看表单列表。 右键点击网格中的表单行以查看上下文菜单。 点击「删除所有版本」。将出现类似以下的对话框： 点击确定。 注意：当弹出窗口后出现错误消息时，表示项目未被删除。错误消息中将显示 ItemType 对应的属性名称 [itemtype_name property_name}，必须从 ItemTypes 中删除该',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [89/244] Deleting_a_Forum.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('a984c06691a7',
   N'删除论坛',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">删除论坛</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Deleting_a_Forum.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除论坛</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户可以从自己的书签中移除论坛。但用户无法移除管理员强制显示在其书签上的任何其他论坛。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要删除论坛，请右键点击要删除的论坛名称，然后从上下文菜单中选择「移除」选项。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/2c7fe6f9799e.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'删除论坛 用户可以从自己的书签中移除论坛。但用户无法移除管理员强制显示在其书签上的任何其他论坛。 要删除论坛，请右键点击要删除的论坛名称，然后从上下文菜单中选择「移除」选项。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [90/244] Deleting_a_Method.htm (5 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('811b9f7ea6b8',
   N'删除方法',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">删除方法</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Deleting_a_Method.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除方法</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中，选择 管理 &gt; 方法。将出现以下菜单：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「搜索方法」。将出现搜索网格。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4d8e2251a647.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击查看方法列表。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">右键点击要删除的方法。将出现以下上下文菜单：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9bcf17c7b30c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">选择「清除此版本」以删除方法的最新版本。选择「删除所有版本」以删除方法的所有版本。将出现类似以下的对话框：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/47ae1ff5fce0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击确定。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：当确认弹出窗口后出现错误消息时，表示项目未被删除。错误消息中将显示 ItemType 对应的属性名称 [itemtype_name property_name}，必须从 ItemTypes 中删除该属性。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/59314d884d24.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Select Purge This Version to delete the most recent version of the method. Select Delete All Versions to delete all the versions of a method. A dialog box similar to the following appears:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7c67c4b4ad42.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击确定。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Note: When an error message appears after the confirmation pop-up window the item has not been deleted. The name of the property corresponding to the ItemType appears in the error message as [itemtype_name property_name} and must be deleted from ItemTypes.</Run></Paragraph>
</FlowDocument>',
   N'删除方法 在导航窗格中，选择 管理 > 方法。将出现以下菜单： 点击「搜索方法」。将出现搜索网格。 点击查看方法列表。 右键点击要删除的方法。将出现以下上下文菜单： 选择「清除此版本」以删除方法的最新版本。选择「删除所有版本」以删除方法的所有版本。将出现类似以下的对话框： 点击确定。 注意：当确认弹出窗口后出现错误消息时，表示项目未被删除。错误消息中将显示 ItemType 对应的属性名称 [it',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [91/244] Deleting_a_Variable.htm (5 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('ad2bb3b8f020',
   N'删除变量',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">删除变量</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Deleting_a_Variable.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除变量</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从导航窗格中，选择 管理 &gt; 变量。将出现以下菜单：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「搜索变量」。将出现搜索网格。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d222855f6181.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击查看变量列表：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">右键点击要删除的变量。将出现以下上下文菜单：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9bcf17c7b30c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击「删除所有版本」。将出现类似以下的对话框：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b35c9605879f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击确定。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：当确认弹出窗口后出现错误消息时，表示项目未被删除。错误消息中将显示 ItemType 对应的属性名称 [itemtype_name property_name}，必须从 ItemTypes 中删除该属性。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5fe8e43d1afa.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click Delete All Versions. A dialog box similar to the following appears:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ef5b7a8ad0d9.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击确定。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Note: When an error message appears after the confirmation pop-up window the item has not been deleted. The name of the property corresponding to the ItemType appears in the error message as [itemtype_name property_name} and must be deleted from ItemTypes.</Run></Paragraph>
</FlowDocument>',
   N'删除变量 从导航窗格中，选择 管理 > 变量。将出现以下菜单： 点击「搜索变量」。将出现搜索网格。 点击查看变量列表： 右键点击要删除的变量。将出现以下上下文菜单： 点击「删除所有版本」。将出现类似以下的对话框： 点击确定。 注意：当确认弹出窗口后出现错误消息时，表示项目未被删除。错误消息中将显示 ItemType 对应的属性名称 [itemtype_name property_name}，必须从',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [92/244] Deleting_a_Version.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('82acc27576b3',
   N'删除版本',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">删除版本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Deleting_a_Version.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除版本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">请稍后回来查看！</Run></Paragraph>
</FlowDocument>',
   N'删除版本 请稍后回来查看！',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [93/244] Deleting_a_Viewer.htm (5 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('d514e21be2c4',
   N'删除查看器',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">删除查看器</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Deleting_a_Viewer.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除查看器</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中，选择 管理 &gt; 文件处理 &gt; 查看器。将出现以下菜单：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c229ad697e6d.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击「搜索查看器」。将出现搜索网格。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击查看可用查看器列表。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9bcf17c7b30c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">右键点击相应的查看器行。将出现以下上下文菜单。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3a7de56df31e.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击「删除所有版本」。将出现类似以下的弹出窗口：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/06eb7c0d59cc.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击确定。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：当确认弹出窗口后出现错误消息时，表示项目未被删除。错误消息中将显示 ItemType 对应的属性名称 [itemtype_name property_name]，必须从 ItemTypes 中删除该属性。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/cf5ca868737a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'删除查看器 在导航窗格中，选择 管理 > 文件处理 > 查看器。将出现以下菜单： 点击「搜索查看器」。将出现搜索网格。 点击查看可用查看器列表。 右键点击相应的查看器行。将出现以下上下文菜单。 点击「删除所有版本」。将出现类似以下的弹出窗口： 点击确定。 注意：当确认弹出窗口后出现错误消息时，表示项目未被删除。错误消息中将显示 ItemType 对应的属性名称 [itemtype_name pro',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [94/244] discovery.htm (5 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('f447a99ce3f4',
   N'关于发现权限',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于发现权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: discovery.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于发现权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">发现权限提供了进一步限制访问 Aras Innovator 数据库中项目的能力。发现权限决定身份是否被允许知道项目存在于 Aras Innovator 数据库中。此功能可用于限制开发或供应链合作伙伴仅发现 Aras Innovator 数据库中他们负责的项目。在这种情况下，即使其他项目匹配搜索条件，也只有设置了「可以发现」访问权限的项目才会在搜索中返回。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">发现和获取访问行为</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">访问类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Aras Innovator 行为</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">无获取或发现</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">（无访问权限）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">即使匹配搜索条件，项目也不会在主网格或关系网格中返回</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果设置了「显示权限警告」，且由于权限不足导致结果中缺少项目，状态栏将显示权限限制警告</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">仅发现访问</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当匹配搜索条件时，项目将在主网格和关系网格中返回</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户将被限制仅查看网格中返回的信息，无法打开表单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">主网格的锁定状态列将显示警告</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/64f8f503eb71.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">如果设置了「显示权限警告」，且由于权限不足导致结果中缺少项目，状态栏将显示权限限制警告</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9ba135b54438.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">获取访问权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">此处无变化</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户将被允许查看所有信息和打开表单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Aras Innovator 可以配置为提醒用户有因权限限制而未显示的项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">发现和获取访问场景</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以下场景演示了如何限制「所有供应商」身份发现初步零部件。在此示例中，「新零部件」和「已发布零部件」权限已按所示进行编辑。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">定义「获取」访问时的结果</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/66c577bc0e27.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">以「所有员工」身份成员登录时的搜索屏幕结果</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">此身份对这两个项目都有「获取」访问权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">未显示警告</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">仅定义「发现」访问时的结果</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以「所有供应商」身份成员登录时的搜索屏幕结果</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c82877847b00.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">此身份对其中一个项目有「发现」但没有「获取」访问权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">此身份对其中一个项目没有访问权限（未在结果中显示）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">第一列显示仅发现图标</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">状态栏显示警告，指示还有其他项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">「发现」权限的主要功能是允许用户搜索项目并查看有限信息，但无法查看项目表单中显示的详细信息。此功能为敏感信息提供了一定级别的访问控制。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Warnings are displayed in the status bar indicating that there are additional items</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4b9aa13d10f8.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The primary function of a “Discover” privilege is to allow the user to search for items and view limited information about them, but not view the detailed information that would be present on the item form. This feature provides a certain level of access control over sensitive information.</Run></Paragraph>
</FlowDocument>',
   N'关于发现权限 发现权限提供了进一步限制访问 Aras Innovator 数据库中项目的能力。发现权限决定身份是否被允许知道项目存在于 Aras Innovator 数据库中。此功能可用于限制开发或供应链合作伙伴仅发现 Aras Innovator 数据库中他们负责的项目。在这种情况下，即使其他项目匹配搜索条件，也只有设置了「可以发现」访问权限的项目才会在搜索中返回。 发现和获取访问行为 访问类型',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [95/244] discovery.htm (5 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('a6843f7c130e',
   N'关于发现权限（续）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于发现权限（续）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: discovery.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于发现权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">发现权限提供了进一步限制访问 Aras Innovator 数据库中项目的能力。发现权限决定身份是否被允许知道项目存在于 Aras Innovator 数据库中。此功能可用于限制开发或供应链合作伙伴仅发现 Aras Innovator 数据库中他们负责的项目。在这种情况下，即使其他项目匹配搜索条件，也只有设置了「可以发现」访问权限的项目才会在搜索中返回。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">发现和获取访问行为</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">访问类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Aras Innovator 行为</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">无获取或发现</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">（无访问权限）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">即使匹配搜索条件，项目也不会在主网格或关系网格中返回</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果设置了「显示权限警告」，且由于权限不足导致结果中缺少项目，状态栏将显示权限限制警告</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">仅发现访问</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当匹配搜索条件时，项目将在主网格和关系网格中返回</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户将被限制仅查看网格中返回的信息，无法打开表单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">主网格的锁定状态列将显示警告</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/64f8f503eb71.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">如果设置了「显示权限警告」，且由于权限不足导致结果中缺少项目，状态栏将显示权限限制警告</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9ba135b54438.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">获取访问权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">此处无变化</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户将被允许查看所有信息和打开表单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Aras Innovator 可以配置为提醒用户有因权限限制而未显示的项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">发现和获取访问场景</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以下场景演示了如何限制「所有供应商」身份发现初步零部件。在此示例中，「新零部件」和「已发布零部件」权限已按所示进行编辑。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">定义「获取」访问时的结果</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/143800500461.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">以「所有员工」身份成员登录时的搜索屏幕结果</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">此身份对这两个项目都有「获取」访问权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">未显示警告</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">仅定义「发现」访问时的结果</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以「所有供应商」身份成员登录时的搜索屏幕结果</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c82877847b00.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">此身份对其中一个项目有「发现」但没有「获取」访问权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">此身份对其中一个项目没有访问权限（未在结果中显示）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">第一列显示仅发现图标</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">状态栏显示警告，指示还有其他项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">「发现」权限的主要功能是允许用户搜索项目并查看有限信息，但无法查看项目表单中显示的详细信息。此功能为敏感信息提供了一定级别的访问控制。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Warnings are displayed in the status bar indicating that there are additional items</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4b9aa13d10f8.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The primary function of a “Discover” privilege is to allow the user to search for items and view limited information about them, but not view the detailed information that would be present on the item form. This feature provides a certain level of access control over sensitive information.</Run></Paragraph>
</FlowDocument>',
   N'关于发现权限 发现权限提供了进一步限制访问 Aras Innovator 数据库中项目的能力。发现权限决定身份是否被允许知道项目存在于 Aras Innovator 数据库中。此功能可用于限制开发或供应链合作伙伴仅发现 Aras Innovator 数据库中他们负责的项目。在这种情况下，即使其他项目匹配搜索条件，也只有设置了「可以发现」访问权限的项目才会在搜索中返回。 发现和获取访问行为 访问类型',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [96/244] Discussion_Panel.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('b95a1c9f9740',
   N'讨论面板',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">讨论面板</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Discussion_Panel.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">讨论面板</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当您点击项目窗口侧边栏上的「显示讨论面板」时，讨论面板将出现在窗口的右侧。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">讨论面板允许您查看现有消息并创建新的评论和回复。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要了解有关项目独立窗口中讨论面板功能的更多信息，请参阅以下主题：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ba1ff63e1921.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">讨论面板工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">讨论面板</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建评论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息功能</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息排序选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息搜索和过滤选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用 @提及</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用 @提及</Run></Paragraph>
</FlowDocument>',
   N'讨论面板 当您点击项目窗口侧边栏上的「显示讨论面板」时，讨论面板将出现在窗口的右侧。 讨论面板允许您查看现有消息并创建新的评论和回复。 要了解有关项目独立窗口中讨论面板功能的更多信息，请参阅以下主题： 讨论面板工具栏 讨论面板 创建评论 消息类型 消息功能 消息排序选项 消息搜索和过滤选项 使用 @提及 Using @mention',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [97/244] Discussion_Panel_TOC.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('8202dfc6b466',
   N'使用讨论面板',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">使用讨论面板</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Discussion_Panel_TOC.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用讨论面板</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您可以使用讨论面板向其他人发送评论、草图和快照。有关更多信息，请参阅以下内容：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">讨论面板</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">讨论面板工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用 @提及</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建文本评论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息功能</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息排序选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息搜索和过滤</Run></Paragraph>
</FlowDocument>',
   N'使用讨论面板 您可以使用讨论面板向其他人发送评论、草图和快照。有关更多信息，请参阅以下内容： 讨论面板 讨论面板工具栏 使用 @提及 创建文本评论 消息类型 消息功能 消息排序选项 消息搜索和过滤',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [98/244] Discussion_Panel_Toolbar.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('47cd92ab7d42',
   N'讨论面板工具栏',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">讨论面板工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Discussion_Panel_Toolbar.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">讨论面板工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">讨论面板上的工具栏有各种控件，用于输入评论和控制面板内消息的显示。工具栏各部分的功能说明如下：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">下表说明了讨论面板工具栏中各部分的属性</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e4fce63609ae.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">讨论工具栏的组成部分</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">添加评论/评论输入框</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在讨论面板中通过在评论输入框中输入文本并点击评论按钮来创建评论。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">评论将显示在讨论面板的内容区域中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">拖动手柄</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">拖动手柄使您可以方便地输入大量文本评论。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">评论输入框的右下角允许用户将框拖动到更大尺寸。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">附件下拉菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「附加」链接以将草图或图片附加到您的评论。此选项仅在查看器处于批注模式时显示。当表单/查看器处于查看模式时不会显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有关批注模式的更多详细信息，请参阅查看和批注模式。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息排序选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有三种消息排序选项：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">对话</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">主题日期</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息日期</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有关消息排序选项的更多详细信息，请参阅消息排序选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息显示选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">更改此选择器将更改讨论中所有评论的样式。有两种显示模式：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标准 - 标准模式是正常模式，也是默认选中的模式。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">审核 - 审核模式用于审核批注。在此模式下，所有批注消息以「快照样式」显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">基本搜索框</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在搜索框中输入任何单词即可开始搜索特定消息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有关基本搜索选项的更多详细信息，请参阅消息搜索和过滤选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高级搜索选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">提供高级消息搜索选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有关高级搜索选项的更多详细信息，请参阅消息搜索和过滤选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">分享评论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使您可以与单个人或一组人分享评论。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Enables you to share a comment with a single person or group of people.</Run></Paragraph>
</FlowDocument>',
   N'讨论面板工具栏 讨论面板上的工具栏有各种控件，用于输入评论和控制面板内消息的显示。工具栏各部分的功能说明如下： 下表说明了讨论面板工具栏中各部分的属性 讨论工具栏的组成部分 属性 添加评论/评论输入框 在讨论面板中通过在评论输入框中输入文本并点击评论按钮来创建评论。 评论将显示在讨论面板的内容区域中。 拖动手柄 拖动手柄使您可以方便地输入大量文本评论。 评论输入框的右下角允许用户将框拖动到更大尺寸',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [99/244] Display_Settings.htm (2 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('057d57e47eba',
   N'显示设置',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">显示设置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Display_Settings.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示设置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击显示图标以查看所选项目的表单或属性。点击预览，如下所示：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/222eac440b08.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">以下是预览选项：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/98ea055586bf.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">菜单选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在底部显示所选项目的基本显示</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在主搜索网格左侧显示所选项目属性的基本列表。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">隐藏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">隐藏所有预览，仅显示主搜索网格。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">「保存布局」选项将设置保存为您的首选项。下次打开相同的搜索网格时，显示设置将默认为保存的选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The Save Layout option saves the setting as your preference. The next time you open the same Search Grid, the Display Setting will default to the saved option.</Run></Paragraph>
</FlowDocument>',
   N'显示设置 点击显示图标以查看所选项目的表单或属性。点击预览，如下所示： 以下是预览选项： 菜单选项 说明 表单 在底部显示所选项目的基本显示 属性 在主搜索网格左侧显示所选项目属性的基本列表。 隐藏 隐藏所有预览，仅显示主搜索网格。 「保存布局」选项将设置保存为您的首选项。下次打开相同的搜索网格时，显示设置将默认为保存的选项。 The Save Layout option saves the se',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [100/244] Dynamic_Assigments.htm (3 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('1c19e24c1fe0',
   N'动态分配',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">动态分配</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Dynamic_Assigments.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">动态分配</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">动态分配必须在工作流映射中设置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择要为其实施动态分配的任何活动的管理者。只有管理者能够更改工作流流程的受让人</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择同一活动的角色，角色是一个身份；只有此身份的成员才能被分配到此活动</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工作流映射中所有将有动态分配的活动都必须设置管理者和角色。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">动态分配在工作流流程中执行</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9c80a553f743.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">要进行动态分配</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开附加了工作流流程的源项目，在此示例中为问题报告</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在项目窗口菜单中选择 视图 --&gt; 工作流</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将打开以下窗口，并锁定工作流流程</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择要更改分配的活动</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3718bfc8d1f8.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">如果您是管理者身份的成员，添加和删除图标将被启用</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">添加、删除或编辑分配</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/1afab33ed980.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">当您保存更改时，系统将验证以确保您选择的受让人是角色的成员</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">If you are a member of the Managed By Identity, the Add and Delete Icons will be enabled</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">添加、删除或编辑分配</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">When you save the changes they will be validated to ensure that the assignees you selected are members of the Role</Run></Paragraph>
</FlowDocument>',
   N'动态分配 动态分配必须在工作流映射中设置 选择要为其实施动态分配的任何活动的管理者。只有管理者能够更改工作流流程的受让人 选择同一活动的角色，角色是一个身份；只有此身份的成员才能被分配到此活动 工作流映射中所有将有动态分配的活动都必须设置管理者和角色。 动态分配在工作流流程中执行 要进行动态分配 打开附加了工作流流程的源项目，在此示例中为问题报告 在项目窗口菜单中选择 视图 --> 工作流 将打开',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [101/244] Edit_menu.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('7e861cd72cfe',
   N'编辑菜单',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">编辑菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Edit_menu.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">编辑菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">编辑菜单提供执行项目相关编辑任务的各种选项，如复制、粘贴、编辑、删除、解锁等。下表说明了编辑菜单中每个可用选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">编辑菜单中的某些选项可能显示为禁用状态。菜单选项在选择适当项目且登录用户具有足够访问权限时启用。访问权限由管理员配置。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">菜单选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">清除</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除所选项目的版本。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择「清除此版本」删除所选项目的版本。「删除所有版本」删除所选项目的所有版本。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">编辑</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以「编辑」模式打开所选项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">查看</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以「只读」模式打开所选项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">复制</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您复制所选的关系项目。有关更多信息，请参阅复制项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">粘贴</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您粘贴已复制的关系项目。有关更多信息，请参阅复制和粘贴关系项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择性粘贴</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您粘贴已复制的关系项目，并具有复制权限和/或创建子项目的功能。有关更多信息，请参阅使用选择性粘贴。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示剪贴板</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示已复制并在剪贴板上可用的关系项目列表。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">认领</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">锁定所选项目并阻止其他用户提交更改。有关更多信息，请参阅认领或取消认领项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">取消认领</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">解锁所选项目并允许任何授权用户锁定项目进行更改。有关更多信息，请参阅认领或取消认领项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">撤销更改</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将项目恢复为上次保存时的值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">提升</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将项目提升到已分配生命周期映射中定义的下一个状态。有关更多信息，请参阅在生命周期中提升项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">版本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示所选项目的版本历史。</Run></Paragraph>
</FlowDocument>',
   N'编辑菜单 编辑菜单提供执行项目相关编辑任务的各种选项，如复制、粘贴、编辑、删除、解锁等。下表说明了编辑菜单中每个可用选项。 编辑菜单中的某些选项可能显示为禁用状态。菜单选项在选择适当项目且登录用户具有足够访问权限时启用。访问权限由管理员配置。 菜单选项 说明 清除 删除所选项目的版本。 删除 选择「清除此版本」删除所选项目的版本。「删除所有版本」删除所选项目的所有版本。 编辑 以「编辑」模式打开所',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [102/244] Edit_Menu_(Item_Window).htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('80c14c9afaa5',
   N'编辑菜单（项目窗口）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">编辑菜单（项目窗口）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Edit_Menu_(Item_Window).htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">编辑菜单（项目窗口）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">编辑菜单提供执行项目相关编辑任务的各种选项，如复制、粘贴、编辑、删除、解锁等。下表说明了编辑菜单中每个可用选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">编辑菜单中的某些选项可能显示为禁用状态。菜单选项在选择适当项目且登录用户具有足够访问权限时启用。访问权限由管理员配置。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">菜单选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">清除此版本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除打开的项目的版本。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除所有版本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除打开的项目的所有版本。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">复制</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您复制所选的关系项目。有关更多信息，请参阅复制项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">粘贴</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您粘贴已复制的关系项目。有关更多信息，请参阅复制和粘贴关系项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择性粘贴</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您粘贴已复制的关系项目，并具有复制权限和/或创建子项目的功能。有关更多信息，请参阅使用选择性粘贴。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示剪贴板</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示已复制并在剪贴板上可用的关系项目列表。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">认领</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">锁定打开的项目并阻止其他用户提交更改。有关更多信息，请参阅认领或取消认领项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">取消认领</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">解锁打开的项目并允许任何授权用户锁定项目进行更改。有关更多信息，请参阅认领或取消认领项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">撤销更改</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将项目恢复为上次保存时的值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">提升</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将项目提升到已分配生命周期映射中定义的下一个状态。有关更多信息，请参阅在生命周期中提升项目。</Run></Paragraph>
</FlowDocument>',
   N'编辑菜单（项目窗口） 编辑菜单提供执行项目相关编辑任务的各种选项，如复制、粘贴、编辑、删除、解锁等。下表说明了编辑菜单中每个可用选项。 编辑菜单中的某些选项可能显示为禁用状态。菜单选项在选择适当项目且登录用户具有足够访问权限时启用。访问权限由管理员配置。 菜单选项 说明 清除此版本 删除打开的项目的版本。 删除所有版本 删除打开的项目的所有版本。 复制 允许您复制所选的关系项目。有关更多信息，请参',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [103/244] enable_the_team.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('345f3e3d98fe',
   N'为 ItemType 启用团队项目的步骤（仅限管理员）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">为 ItemType 启用团队项目的步骤（仅限管理员）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: enable_the_team.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">为 ItemType 启用团队项目的步骤（仅限管理员）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">这些步骤描述了在 ItemType 的表单上公开 team_id 系统属性。它们是通用的，适用于系统中的任何 ItemType。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">查看您希望为其提供用户使用团队项目能力的 ItemType 的表单（编辑模式）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将 team_id 系统属性添加到表单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存表单。用户现在可以将团队关联到 ItemType 的实例。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4510d6445306.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Save the form. Users may now associate a Team to instances of the ItemType.</Run></Paragraph>
</FlowDocument>',
   N'为 ItemType 启用团队项目的步骤（仅限管理员） 这些步骤描述了在 ItemType 的表单上公开 team_id 系统属性。它们是通用的，适用于系统中的任何 ItemType。 查看您希望为其提供用户使用团队项目能力的 ItemType 的表单（编辑模式）。 将 team_id 系统属性添加到表单。 保存表单。用户现在可以将团队关联到 ItemType 的实例。 Save the form',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [104/244] End_User.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('1e098edd0190',
   N'用户帮助',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">用户帮助</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: End_User.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户帮助</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">最终用户帮助部分面向 Aras Innovator 的所有用户。该帮助部分包括 Aras Innovator 的基本导航、各种菜单和工具栏选项、Aras Innovator 屏幕术语、如何执行各种搜索操作，以及项目窗口、表单、关系、结构浏览器和我的 Innovator 的基本信息。此部分还列出了最终用户可以执行的一些常见任务。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">导航</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目窗口</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">结构浏览器</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">我的 Innovator</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">常见用户操作</Run></Paragraph>
</FlowDocument>',
   N'用户帮助 最终用户帮助部分面向 Aras Innovator 的所有用户。该帮助部分包括 Aras Innovator 的基本导航、各种菜单和工具栏选项、Aras Innovator 屏幕术语、如何执行各种搜索操作，以及项目窗口、表单、关系、结构浏览器和我的 Innovator 的基本信息。此部分还列出了最终用户可以执行的一些常见任务。 导航 搜索 项目窗口 结构浏览器 我的 Innovator ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [105/244] searchtoolbar.htm (13 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('bcadca1220d4',
   N'搜索工具栏',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">搜索工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: searchtoolbar.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以下是 ItemType 搜索工具栏的示例。Aras Innovator 中的每个搜索工具栏都使用相同的按钮。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">下表描述了搜索工具栏中的按钮。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/00cc2ac18dd0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">按钮名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">按钮</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">执行搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">清除搜索条件</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9bcf17c7b30c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">删除已应用的搜索条件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索模式下拉菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您选择搜索模式。可用的不同搜索模式包括隐藏、简单、高级和 AML。有关每种搜索模式的更多信息，请参阅搜索模式。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9502b80cff55.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">保存的搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您选择保存的搜索条件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">仅当您在数据库中保存了搜索条件时，才会显示「保存的搜索」下拉菜单。有关更多信息，请参阅保存的搜索。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3474c06e6d3a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">添加条件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">提供额外的行以指定高级搜索的搜索条件和标准。「添加条件」按钮仅适用于高级搜索。有关更多信息，请参阅高级搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">每页结果数</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d25ddade175f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">允许您指定一页中显示的行数。如果留空，则表示一次显示全部。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">上一页</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示主网格中的上一页。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">下一页</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/32ce6b8a0f4a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">显示主网格中的下一页。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示总页数和结果数</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示当前页码以及搜索结果总数。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/544c311bc877.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">最大结果数</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">指定服务器返回的行数。当搜索后有大量项目需要返回时非常有用。例如，输入 1000 可将搜索结果限制为前 1000 行。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使您能够在搜索网格中预览 ItemType 表单和属性。点击「保存布局」以保存特定 ItemType 的布局。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d88de16eecd5.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">使您能够将搜索网格中的信息导出为 Excel、PDF 文件或 Word 文档。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">新建 ItemType</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使您能够创建新的 ItemType。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/67da8972ee0c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">有效性类型下拉菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">截至（日期）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有效性类型下拉菜单和日历仅用于可版本化项目，并在搜索网格上提供进一步过滤。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e35f53cc348c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">当前：显示截至今天存在的项目的最新（最高）世代号。日历被禁用，因为日期默认设置为今天。最新世代号通过 is_current 项目属性以编程方式标记。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">最新：显示截至日历中选定日期存在的项目的最新（最高）世代号，匹配提供的搜索条件。modified_on 日期用于比较。默认情况下，日历设置为今天。最新可能返回项目的非当前世代号；但这是非常罕见的情况。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9eebcdaf2443.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">已发布：显示截至日历中选定日期存在的项目的最新（最高）已发布世代号。最新已发布世代号基于发布日期。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有效：显示基于有效日期的最新有效世代号。当项目被标记为在特定日期有效时，意味着它在该日期投入使用。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/205de0f68c33.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">仅当您对所查找项目的世代号具有所需的访问权限时，搜索才会按有效性条件显示结果。否则，搜索不会返回所需结果。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Enables you to preview ItemType Forms and properties in the Search grid. Click Save Layout to save the layout for a particular ItemType.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Enables you to export an information in the search grid to Excel, a PDF file, or a Word document.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">新建 ItemType</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您创建新的 ItemType。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">生效类型下拉框</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">截至（日期）</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d764335a4471.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The Effectivity Type drop-down and Calendar are used only for versionable Items and provide further filtering on the search grid.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Current: Displays the latest (highest) generation of an Item that exists as of today. The calendar is disabled as the date is set to Today by default. The latest generation is flagged programmatically by is_current Item property.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Latest: Displays the latest (highest) generation of an Item that exists as of the date selected i n the calendar matching the search criteria provided . The modified_on date is used for the comparison. By default, the calendar is set to Today. The Latest may return a non-current generation of the Item; but, this would be a very unusual case.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Released: Displays the latest (highest) released generation of an Item that exists as of the date selected in the calendar. The latest released generation is based on the Released Date.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">. non-current generation of the ItemThe latest effective generation is based on the Effective Date and may return aWhen an Item is marked as Effective on a specific date, it means that it is put into use at that date. the date selected in the calendar. generation of an Item whose effective date is within latest (highest) Displays the: Effective</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The search displays the results as per the effectivity criteria only if you have required access rights to the generation of the Item you are looking for. Else, the search does not return the required result.</Run></Paragraph>
</FlowDocument>',
   N'搜索工具栏 以下是 ItemType 搜索工具栏的示例。Aras Innovator 中的每个搜索工具栏都使用相同的按钮。 下表描述了搜索工具栏中的按钮。 按钮名称 按钮 说明 搜索 执行搜索。 清除搜索条件 删除已应用的搜索条件。 搜索模式下拉菜单 允许您选择搜索模式。可用的不同搜索模式包括隐藏、简单、高级和 AML。有关每种搜索模式的更多信息，请参阅搜索模式。 保存的搜索 允许您选择保存的搜索',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [106/244] searchmenu.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('b62e4d236575',
   N'搜索选项',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">搜索选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: searchmenu.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索菜单提供执行搜索相关任务的各种选项。下表说明了搜索菜单中每个可用选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">执行搜索。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9bcf17c7b30c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">高级搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许搜索网格中未显示的属性。有关更多信息，请参阅高级搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">AML 搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许使用管理员创建的复杂搜索。有关更多信息，请参阅 AML 搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">隐藏搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">隐藏蓝色搜索栏并禁止您指定搜索条件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">简单搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您通过在蓝色搜索栏中为各种属性指定条件来执行搜索。有关更多信息，请参阅简单搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存搜索...</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您保存当前搜索条件并检索先前创建的搜索条件。有关更多信息，请参阅保存的搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除保存的搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从主网格中删除选定的保存搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用通配符</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许在搜索条件中使用通配符 * 和 %。默认情况下启用通配符的使用。如果禁用，则禁止使用通配符。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">追加结果</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您指定另一个搜索条件并将结果追加到主网格中已显示的项目。例如，您可以使用追加结果来查看具有多个特定零部件编号的项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">跨页排序</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">按 ItemType 中定义的序列返回多页排序结果。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Return multiple pages sorted as per sequence defined in ItemType.</Run></Paragraph>
</FlowDocument>',
   N'搜索选项 搜索菜单提供执行搜索相关任务的各种选项。下表说明了搜索菜单中每个可用选项。 搜索选项 说明 执行搜索。 高级搜索 允许搜索网格中未显示的属性。有关更多信息，请参阅高级搜索。 AML 搜索 允许使用管理员创建的复杂搜索。有关更多信息，请参阅 AML 搜索。 隐藏搜索 隐藏蓝色搜索栏并禁止您指定搜索条件。 简单搜索 允许您通过在蓝色搜索栏中为各种属性指定条件来执行搜索。有关更多信息，请参阅简',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [107/244] Simple_search.htm (5 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('2b0e6f5bb963',
   N'简单搜索',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">简单搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Simple_search.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">简单搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">简单搜索是最简单、最常用的搜索方式。默认情况下，搜索工具栏显示简单搜索。以下是一些增强搜索效果的提示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">简单搜索提示</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/608db3e78c11.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">使用 * 或 % 作为通配符。例如，名称列中的术语 &apos;a*b&apos; 表示以 &apos;a&apos; 开头并以 &apos;b&apos; 结尾的任何项目。如果「使用通配符」选项（来自搜索菜单）被禁用，则字符 * 或 % 将按原样识别，不被视为通配符。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">单独使用 * 来选择非空白值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用 | 在单个属性中使用多个条件。例如，如果您在类型列中输入 Material|Component，则所有属于 Material 或 Component 类型的项目将显示在搜索网格中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用 \\ 来搜索包含 \ 的项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：字符 | 和 \ 是保留字符，不能直接搜索。要搜索这些字符，需要在保留字符前放置转义字符 \。例如，要搜索行业 Aerospace\Defense，请在行业列中输入 Aerospace\\Defense。所有属于 Aerospace\Defense 行业的项目将出现在搜索网格中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用 @{n}（其中 n 是数字）来创建参数化搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">对包含复选框的列使用 0 或 1，0 表示未选中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">按键名搜索项目。例如，&apos;Adm*&apos; 查找名为 Administrators 的身份。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击蓝色搜索栏中最左侧的列以按认领状态搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击搜索工具栏或菜单上的按钮执行搜索。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/bb7432e237ab.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">示例：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3701f61868c3.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">在蓝色搜索栏的名称列中输入 &apos;RC*&apos; 以显示零部件编号以字母 &quot;RC&quot; 开头的项目。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9bcf17c7b30c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">搜索日期</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">简单搜索模式现在支持在日期属性上搜索的运算符，包括 &gt;、&gt;=、&lt;、&lt;= 和（范围）。搜索选项仅限于以下内容，其中 {Date} 表示标准短日期语法：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">{Date} 返回日期值与搜索值完全匹配的结果。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0155fd64a38b.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">&gt;={Date} 返回日期在指定日期之后（或等于）的结果。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">&lt;={Date} 返回日期在指定日期之前（或等于）的结果。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">{Date}...{Date} 返回值在两个值之间（包含）的结果。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：{Date} 文本可以手动输入或使用日期对话框选择。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">&lt;={Date} returns results where the date is before (or equal to) the specified date.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">{Date}...{Date} returns results where the value is between the two values (inclusive).</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Note: {Date} text can either be entered manually or by using the Date dialog.</Run></Paragraph>
</FlowDocument>',
   N'简单搜索 简单搜索是最简单、最常用的搜索方式。默认情况下，搜索工具栏显示简单搜索。以下是一些增强搜索效果的提示。 简单搜索提示 使用 * 或 % 作为通配符。例如，名称列中的术语 ''a*b'' 表示以 ''a'' 开头并以 ''b'' 结尾的任何项目。如果「使用通配符」选项（来自搜索菜单）被禁用，则字符 * 或 % 将按原样识别，不被视为通配符。 单独使用 * 来选择非空白值。 使用 | 在单个属性中使用多',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [108/244] Parameterized_Search.htm (4 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('a01892f0020c',
   N'参数化搜索',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">参数化搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Parameterized_Search.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">参数化搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用参数化搜索方法搜索具有一个或多个项目属性的项目，其中您搜索的属性保持不变，但要搜索的值不同。您可以预配置需要执行搜索的项目属性并保存搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您可以在任何搜索模式下使用参数化搜索，但它们在保存的搜索中最为有用。当主网格中显示具有大量属性的项目时，此搜索也很有用。在这种情况下，仅搜索少数属性会很不方便。您也可以使用参数化搜索将搜索限制为某些参数。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例：如果您经常搜索处于「已发布」状态的零部件，但搜索中包含的成本、修订或零部件编号各不相同，则可以使用参数化搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您可以使用语法 @{n}（其中 n 是数字）预定义要包含在搜索中的项目属性（参数）。可以有任意数量的参数，数字不必是连续的。您也可以在 @{n} 之前或之后使用通配符。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">语法 @{n} 是参数化搜索的保留语法。它始终被解释为参数；您不能搜索字符串 &apos;@{1}&apos;。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建和使用参数化搜索：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">登录 Aras Innovator。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在目录中，导航到要搜索的项目。例如，要搜索零部件，请导航到 设计 --&gt; 零部件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在搜索栏中，通过在列中指定 @{n} 来定义搜索中要使用的属性。在我们的示例中，让我们使用以下项目属性：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">零部件编号：@{1}*</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">状态：@{2}*</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">变更：@{3}</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">导航到 搜索 --&gt; 保存搜索 以保存参数化搜索。保存搜索是可选步骤。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ff77b04cdd7e.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击执行搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将出现「设置参数值」对话框。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9bcf17c7b30c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">图2</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">指定要搜索的值。让我们使用以下条件搜索零部件：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/dce44c5f430c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">零部件编号以 1000* 开头</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">处于已发布状态</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">未包含在变更管理中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击确定。主网格将显示与您的搜索条件匹配的项目列表。每次执行搜索时，都会出现一个显示您定义的项目属性的搜索对话框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您可以指定要搜索的项目属性的值。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/917df0369e71.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click OK. The main grid displays the list of items matching your search criteria. Every time you execute the search, a search dialog displaying the Item Properties you defined appears.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">You can specify the values of the Item&apos;s properties you want to conduct the search for.</Run></Paragraph>
</FlowDocument>',
   N'参数化搜索 使用参数化搜索方法搜索具有一个或多个项目属性的项目，其中您搜索的属性保持不变，但要搜索的值不同。您可以预配置需要执行搜索的项目属性并保存搜索。 您可以在任何搜索模式下使用参数化搜索，但它们在保存的搜索中最为有用。当主网格中显示具有大量属性的项目时，此搜索也很有用。在这种情况下，仅搜索少数属性会很不方便。您也可以使用参数化搜索将搜索限制为某些参数。 示例：如果您经常搜索处于「已发布」状态',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [109/244] savedsearch.htm (6 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('94ec94704d94',
   N'保存的搜索',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">保存的搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: savedsearch.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存的搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存的搜索 ItemType 使用户能够检索先前创建的搜索；它们可以通过多种方式使用。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户的保存搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">执行搜索后，您可以将其保存并命名，以便将来可以检索和重复使用（有或无修改）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">每当您执行搜索时，搜索模式和搜索词会自动保存。当您返回某个项目时，上次搜索会自动恢复。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">每个用户都可以执行自己创建的保存搜索，以及其他用户共享的搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在任何模式下进行搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「添加到收藏夹」按钮保存搜索。将出现收藏搜索对话框。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a620e17f3be9.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/bacc8a753fea.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">在「标签」字段中输入搜索名称。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「共享给：」字段中的省略号，与组织中的其他 Aras Innovator 用户共享搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">勾选「固定到快速访问」复选框以在目录中显示搜索的快捷方式图标。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d5f940a08d6b.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击保存。您可以通过在导航面板中选择 管理 &gt; 保存的搜索 来访问搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">检索/应用保存的搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">登录 Aras Innovator。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在目录中选择 管理 &gt; 保存的搜索。将出现保存的搜索网格。将出现保存的搜索菜单：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/8a63fe1232b7.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击「搜索保存的搜索」以查看保存的搜索列表。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">右键点击要执行的搜索，然后点击「运行」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除保存的搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">登录 Aras Innovator。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中选择 管理 &gt; 保存的搜索。将出现保存的搜索网格。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击查看保存的搜索列表。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9bcf17c7b30c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">右键点击要删除的搜索，从上下文菜单中选择「删除所有版本」。将出现类似以下的对话框。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a8edd37a5897.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击确定。</Run></Paragraph>
</FlowDocument>',
   N'保存的搜索 保存的搜索 ItemType 使用户能够检索先前创建的搜索；它们可以通过多种方式使用。 用户的保存搜索 执行搜索后，您可以将其保存并命名，以便将来可以检索和重复使用（有或无修改）。 每当您执行搜索时，搜索模式和搜索词会自动保存。当您返回某个项目时，上次搜索会自动恢复。 每个用户都可以执行自己创建的保存搜索，以及其他用户共享的搜索。 保存搜索 在任何模式下进行搜索。 点击「添加到收藏夹」',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [110/244] saved search administration.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('50175490cf46',
   N'保存的搜索管理',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">保存的搜索管理</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: saved search administration.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存的搜索管理</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存的搜索出现在目录（TOC）的管理类别中</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">主网格中显示的列有：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/acfe5364039a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">标签：保存的搜索的标签，显示在工具栏的保存搜索部分。对于自动保存的搜索，此列为空。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType：搜索适用的 ItemType 名称。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索模式：首次保存搜索时，此列显示用于创建搜索的搜索模式。可以通过点击列中的相应链接并编辑项目来更改。选择 No UI 模式会在您选择保存的搜索时隐藏搜索详细信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果所选搜索模式无法显示搜索条件（例如，如果简单搜索模式用于在 AML 模式下创建的搜索，但条件无法在简单搜索模式中表示），则使用能够表示条件的最简单模式。模式层次结构为简单、高级、AML。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">条件：所有搜索的条件以相同的格式显示，即 Aras 标记语言（AML），Aras Innovator 的基础语言。熟悉 AML 的管理员可以通过在主网格中显示保存的搜索时创建新项目来直接创建保存的搜索。有关 AML 的更多信息，请参阅程序员指南或 API 类。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">位置：显示搜索将在何处使用：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">主网格</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系网格搜索对话框</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">自动保存：此列中的复选标记表示搜索是自动保存的。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Owned_by_id：显示创建搜索的身份。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Managed_by_id：显示管理搜索的身份。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在 TOC 中显示：复选标记表示保存的搜索出现在导航窗格中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用主菜单保存主网格的搜索，使用项目窗口菜单保存关系网格的搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要为搜索对话框创建搜索，请执行以下操作之一：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">为主网格或关系网格创建搜索，然后编辑保存的搜索以更改位置。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">直接在管理树中创建保存的搜索：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">自动保存：当您启动搜索时，搜索会自动保存，此时 auto_saved 设置为 1。当从菜单保存搜索或从管理树创建搜索时，自动保存设置为 0。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">owned_by_id：自动设置为创建搜索的用户的别名身份，并拥有项目的获取访问权限。可以由管理者或管理员更改。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">managed_by_id：自动设置为创建搜索的用户的别名身份。管理者和管理员拥有项目的完全访问权限。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">managed_by_id: is automatically set to the alias Identity of the user who created the search. The manager and Administrators have full access to the Item</Run></Paragraph>
</FlowDocument>',
   N'保存的搜索管理 保存的搜索出现在目录（TOC）的管理类别中 主网格中显示的列有： 标签：保存的搜索的标签，显示在工具栏的保存搜索部分。对于自动保存的搜索，此列为空。 ItemType：搜索适用的 ItemType 名称。 搜索模式：首次保存搜索时，此列显示用于创建搜索的搜索模式。可以通过点击列中的相应链接并编辑项目来更改。选择 No UI 模式会在您选择保存的搜索时隐藏搜索详细信息。 如果所选搜索',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [111/244] searchmodesfilt.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('8c1e4acfb129',
   N'搜索模式',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">搜索模式</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: searchmodesfilt.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索模式</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您可以从搜索菜单和搜索工具栏中选择四种搜索模式，适用于主网格和关系网格。模式如下：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">隐藏：选择此模式时，搜索栏不显示。此模式禁止您更改搜索条件。选择其他搜索模式以指定新的搜索条件。隐藏搜索条件不会删除先前的搜索条件，只是隐藏搜索条件的显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">简单：此模式是最常用的搜索模式，也是最容易使用和理解的。蓝色搜索栏使您能够使用通配符输入搜索条件。有关更多信息，请参阅简单搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高级：此模式是一个向导，使您能够使用简单搜索中无法使用的属性和搜索条件执行搜索。有关更多信息，请参阅高级搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">AML：这是一种原始搜索模式，允许您使用属性和属性创建复杂搜索。有关更多信息，请参阅 AML 搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">相关主题</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高级搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">AML 搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">简单搜索</Run></Paragraph>
</FlowDocument>',
   N'搜索模式 您可以从搜索菜单和搜索工具栏中选择四种搜索模式，适用于主网格和关系网格。模式如下： 隐藏：选择此模式时，搜索栏不显示。此模式禁止您更改搜索条件。选择其他搜索模式以指定新的搜索条件。隐藏搜索条件不会删除先前的搜索条件，只是隐藏搜索条件的显示。 简单：此模式是最常用的搜索模式，也是最容易使用和理解的。蓝色搜索栏使您能够使用通配符输入搜索条件。有关更多信息，请参阅简单搜索。 高级：此模式是一个',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [112/244] amlsearch.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('d2202c83f5b8',
   N'AML 搜索',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">AML 搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: amlsearch.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">AML 搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">这是一种原始搜索模式，使用 Aras 标记语言（AML）。具有 AML 知识的用户可以使用此搜索模式。它允许所有用户进行复杂搜索，以及使用其他高级用户、管理员或开发者准备并作为保存搜索共享的搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">AML 搜索可以执行其他搜索模式无法完成的搜索，包括：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在关系和关联项目中搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在目录中关系项目的源或父项目中搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">同时在多个层级使用逻辑</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：如果您在简单搜索模式下开始构建搜索，然后切换到 AML 模式，搜索模式中的所有条件将被转换为 AML。这可以节省从头编写搜索条件的时间。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c0952e1246f2.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">有关 AML 的更多信息，您可以参阅 Aras 网站文档部分的程序员指南。您也可以参加 Aras 提供的培训。有关培训详情，请参阅 Aras 网站的培训部分。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">For more information on AML, you can refer to the Programmer&apos;s Guide available in the Documentation section of the Aras website. You can also attend training provided by Aras. For details on the training, refer to the Training section on the Aras website.</Run></Paragraph>
</FlowDocument>',
   N'AML 搜索 这是一种原始搜索模式，使用 Aras 标记语言（AML）。具有 AML 知识的用户可以使用此搜索模式。它允许所有用户进行复杂搜索，以及使用其他高级用户、管理员或开发者准备并作为保存搜索共享的搜索。 AML 搜索可以执行其他搜索模式无法完成的搜索，包括： 在关系和关联项目中搜索 在目录中关系项目的源或父项目中搜索 同时在多个层级使用逻辑 注意：如果您在简单搜索模式下开始构建搜索，然后切',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [113/244] advancedsearch.htm (3 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('bc6865a6a699',
   N'高级搜索',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">高级搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: advancedsearch.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高级搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高级搜索模式是一个向导，允许您使用简单搜索中无法使用的属性和搜索条件执行搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高级搜索允许您：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在关系以及源项目上搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在主网格中未显示的属性上搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用「大于」或「不等于」等运算符代替「等于」加通配符</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高级搜索栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高级搜索有一个额外的按钮。将鼠标悬停在按钮上会显示「添加条件」的工具提示。点击此按钮为高级搜索输入额外的搜索条件。右键点击并从上下文菜单中选择「删除条件」可删除搜索条件。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7d8604cafcc1.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">列说明：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/32ce6b8a0f4a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">ItemType — 在此列中选择源 ItemType 或关系。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性 — 选择用作搜索条件的属性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">运算符 — 选择适合该属性的运算符。下表描述了运算符及其功能：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">运算符</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">运算符名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">=</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">等于</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使您能够搜索属性值与搜索条件中指定值相同的项目。示例：使用零部件编号 = 301 返回零部件编号为 301 的项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">≠</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">不等于</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使您能够搜索属性值与搜索条件中指定值不同的所有项目。示例：使用零部件编号 ≠ 301 返回零部件编号不是 301 的所有项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn"><</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">小于</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使您能够搜索属性值小于搜索条件中指定值的项目。示例：使用成本 &lt; 3000 返回成本小于 3000 的项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn"><=</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">小于等于</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使您能够搜索属性值小于或等于搜索条件中指定值的项目。示例：使用成本 &lt;= 3000 返回成本小于或等于 3000 的项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">></Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">大于</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使您能够搜索属性值大于搜索条件中指定值的项目。示例：使用成本 &gt; 3000 返回成本大于 3000 的项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">大于或等于</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">大于等于</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使您能够搜索属性值大于或等于搜索条件中指定值的项目。示例：使用成本 &gt;= 3000 返回成本大于或等于 3000 的项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">包含</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使您能够在搜索条件中使用通配符 &apos;%&apos; 和 &apos;*&apos;。示例：使用零部件编号 like BR* 返回零部件编号以 BR 开头的所有项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">不包含</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">反转您指定的搜索条件，并允许在搜索条件中使用通配符 &apos;%&apos; 和 &apos;*&apos;。示例：使用零部件编号 not like BR* 返回零部件编号不以 BR 开头的所有项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">空值</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使您能够搜索数据库中明确具有 NULL 值的属性。示例：使用类型 null 返回类型（属性无值）未指定的所有项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">不为空</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使您能够搜索已设置值的任何属性。示例：使用类型 not null 返回类型（属性有值）已指定的所有项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">条件：显示所选属性的适当格式：布尔类型（是/否）属性显示复选框。列表类型属性显示下拉框。日期类型属性在您点击条件时显示日期选择器。其他属性显示空白文本框，只要将 &apos;like&apos; 指定为运算符，就允许通配符搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">布尔类型（是/否）属性显示复选框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">列表类型属性显示下拉框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">日期类型属性在您点击条件时显示日期选择器。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">其他属性显示空白文本框，只要将 &apos;like&apos; 指定为运算符，就允许通配符搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">行</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">每行是搜索的一个 AND 条件。高级搜索过滤器中可以有多行。有滚动条可以上下移动列表。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高级搜索示例</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">对于 Part ItemType</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例1</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType = 零部件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性= 描述</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">运算符= not null</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示包含描述的 Part ItemType。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例2</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType = 零部件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性= 描述</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">运算符= not null</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">且</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType = 零部件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性= created_on</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">运算符= &gt;</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">条件= 02/02/2014</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示包含描述且在指定日期之后创建的 Part ItemType。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例3</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType = 零部件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性= created_on</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">运算符= &gt;</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">条件= 1/1/2014</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">且</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType = 零部件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性= modified_on</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">运算符= &lt;</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">条件= 05/14/2014</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示在 2014年1月1日之后创建且在 2014年5月14日之前修改的 Part ItemType。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建高级搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击搜索工具栏中的「添加条件」按钮添加所有要使用的条件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用右键菜单中的「删除条件」删除不需要的条件。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/32ce6b8a0f4a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">对每个条件行，选择：一个 ItemType、一个属性、一个运算符和一个条件值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">一个 ItemType</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">一个属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">一个运算符</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">一个条件值</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击搜索工具栏中的「搜索」按钮。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">根据需要使用搜索工具栏中的「停止搜索」和「清除搜索」按钮。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Use the ‘Stop Search’ and ‘Clear Search’ buttons in the search toolbar as required.</Run></Paragraph>
</FlowDocument>',
   N'高级搜索 高级搜索模式是一个向导，允许您使用简单搜索中无法使用的属性和搜索条件执行搜索。 高级搜索允许您： 在关系以及源项目上搜索 在主网格中未显示的属性上搜索 使用「大于」或「不等于」等运算符代替「等于」加通配符 高级搜索栏 高级搜索有一个额外的按钮。将鼠标悬停在按钮上会显示「添加条件」的工具提示。点击此按钮为高级搜索输入额外的搜索条件。右键点击并从上下文菜单中选择「删除条件」可删除搜索条件。 ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [114/244] Entering_a_Comment.htm (5 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('ac0184204973',
   N'创建文本评论',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建文本评论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Entering_a_Comment.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建文本评论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户可以在项目/表单处于活动状态时在讨论面板中输入文本评论。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要输入评论，请执行以下操作：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击项目窗口右上角的讨论按钮以显示讨论面板。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/dfa10fd59570.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">在讨论面板的评论输入框中输入所需文本。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：使用拖动手柄可以方便地输入大量文本评论。该框只能向下拖动，同时保持相同的宽度。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击评论按钮发布评论。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：按 Enter 键不会发布评论，而是创建新行。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">一旦评论被输入，它将显示在讨论面板的内容区域中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息的组成部分</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ff477099b986.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">一旦消息显示在讨论面板区域中，其中就有具有特定属性的各种组件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">下表提供了在讨论面板中显示的消息的属性：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息组件</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/65ff5b20ca85.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">来源图标</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">指示消息来源是来自用户评论、回复还是自动的。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户名</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">指示创建评论的用户名称，无论是直接创建还是通过历史记录间接创建。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建时间</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">指示消息创建的时间。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">引用项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">指示消息相关的项目名称。它作为超链接打开该项目的独立窗口。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目信息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目信息显示消息创建时项目的基本项目信息。显示的字符串是引用项目的最多三个属性的组合：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目的当前修订</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目的当前世代号</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目的当前生命周期状态</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目图标</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">与引用项目对应的图标和颜色。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息文本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息的文本。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">缩略图</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">快照的缩略图（如果包含在评论中）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息功能</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可以在消息上执行的功能。有关消息功能的更多详细信息，请参阅消息功能。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息显示</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用滚动键上下滚动到显示消息的底部。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当所有消息都已显示后，将出现「没有更多消息」的消息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Use the scroll keys to scroll up and down to the bottom of the displayed messages.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/be7c5dc3c1b0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">When all the messages have been displayed, the message &quot;There are no more messages&quot; appears.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/8323741b4f6f.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'创建文本评论 用户可以在项目/表单处于活动状态时在讨论面板中输入文本评论。 要输入评论，请执行以下操作： 点击项目窗口右上角的讨论按钮以显示讨论面板。 在讨论面板的评论输入框中输入所需文本。 注意：使用拖动手柄可以方便地输入大量文本评论。该框只能向下拖动，同时保持相同的宽度。 点击评论按钮发布评论。 注意：按 Enter 键不会发布评论，而是创建新行。 一旦评论被输入，它将显示在讨论面板的内容区域',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [115/244] Entering_a_Reply.htm (5 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('9ee088fbbd2f',
   N'输入回复',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">输入回复</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Entering_a_Reply.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">输入回复</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户可以在讨论面板中回复评论或现有消息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要回复评论，请执行以下操作：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击消息下方的「回复」链接。这将打开一个回复框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在回复框中输入所需文本。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「添加回复」按钮发布回复。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">一旦回复发布，它将显示在相应消息的下方。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">回复消息以缩进方式显示，并带有箭头图标。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「隐藏回复」从消息列表中隐藏回复，点击「显示回复」再次显示回复。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示回复和隐藏回复</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9f81241703c5.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">如果消息包含超过三条回复（例如五条），点击「显示回复」链接最初将仅显示该消息的五条回复中的三条。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">链接现在变为「显示所有回复（7）」，点击它将显示该消息的所有回复。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">一旦所有回复都显示，将出现「隐藏回复」链接，点击它将隐藏该消息的所有回复。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/176f2fb46b5f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">在消息下方将再次显示「显示回复」链接。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：回复始终按「最新在底部」的顺序排序。当您点击回复消息上的「回复」链接时，您实际上是在回复原始（顶部）消息。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/eb8172d70a09.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Once all the replies are shown, a Hide Replies link appears and clicking on it will hide all the replies for that message.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/da12f251f82c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">消息下方会再次显示「显示回复」链接。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3125129ad5f2.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Note: The replies are always sorted in the order “newest at bottom&quot;. When you click the Reply link on a Reply message, you are actually replying to the original (top) message.</Run></Paragraph>
</FlowDocument>',
   N'输入回复 用户可以在讨论面板中回复评论或现有消息。 要回复评论，请执行以下操作： 点击消息下方的「回复」链接。这将打开一个回复框。 在回复框中输入所需文本。 点击「添加回复」按钮发布回复。 一旦回复发布，它将显示在相应消息的下方。 回复消息以缩进方式显示，并带有箭头图标。 点击「隐藏回复」从消息列表中隐藏回复，点击「显示回复」再次显示回复。 显示回复和隐藏回复 如果消息包含超过三条回复（例如五条）',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [116/244] Entering_ItemType_Properties.htm (13 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('6dd75279f8f5',
   N'输入 ItemType 属性',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">输入 ItemType 属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Entering_ItemType_Properties.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">输入 ItemType 属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType 中的属性选项卡包含项目的所有属性，包括系统属性。点击 ItemType 表单中的属性选项卡。您应该看到类似以下的内容：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当您创建新属性时，属性网格中会创建一个新行，然后需要由用户值填充。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d4ab4aea3179.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">创建属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开 ItemType 并点击。ItemType 被您认领，这意味着其他人无法编辑它。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击属性选项卡。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/eaf68ab55cfa.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击添加行图标。网格底部将添加一个新行。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">根据需要为每列输入值。有关每列指定内容的说明，请参见下表。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/09fc4497a977.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">列</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性的唯一内部名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标签</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在项目表单和网格上显示的属性名称标签</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">数据类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">确定属性的数据类型 - 如字符串、列表等。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">数据源</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果属性数据类型是列表、序列、项目或其他需要信息源来接收数据的数据类型，则在此处指定数据源。数据源的类型取决于数据类型。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">长度</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字符串数据类型的最大字符数。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">精度</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">对于数据类型为「小数」的属性，精度设置小数点左右的总位数。例如，对于精度设置为 5 的小数属性 A，A 的有效值可以是：12345、12.345、1234.5 等。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">小数位数</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设置数据类型为「小数」时小数点右侧的位数。例如，对于精度设置为 5 且小数位数设置为 2 的小数属性 A，有效值为：123.45、12.45、12.40。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">必填</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">布尔值。选中时，表示属性的值不能为空。对于数据类型为文本的属性，目前有特殊的变通方法来设置和重置必填值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">唯一</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果为 true，则该属性值在此 ItemType 的所有项目实例中必须唯一。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">索引</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果为 true，表示数据库表将在此属性上建立索引，以提高频繁搜索此特定属性的性能。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">联合</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">此属性的值将由自定义方法从 Innovator 外部提供。数据应与属性数据类型匹配以进行设置和显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">隐藏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果为 true，此属性将不会在项目的任何属性网格中显示。例如，从目录中选择 ItemType 时会显示其中一个网格。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以上您可以看到未隐藏的属性。其余的（如系统属性）不会出现在网格上</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">隐藏2</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/8fc26d5ac967.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">如果为 true，此属性将不会出现在关系网格中。例如，查看 Part ItemType。注意唯一未将 Hidden2 设置为 true 的属性（在 Hidden2 的搜索条件中输入 0 并搜索）是：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当 Part 实例被创建且 BOM 选项卡被公开时，注意只有这些属性（加上一些 BOM 关系属性，如序号、数量和参考标识符）是可见的：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">对齐</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">确定网格单元格中值的对齐方式。可能的值为左对齐、右对齐或居中。默认为左对齐。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/193ee2f65dab.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">宽度</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">确定属性网格中列的宽度（以像素为单位）。在以下示例中，长度设置为 30，其余为 100。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/755b9f57a483.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">排序顺序</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">整数值控制搜索网格显示中列的相对位置，值较小的从左到右排列。例如，GeometricShape 属性分类、颜色和半径的设置如下：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">结果主项目网格显示为：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">键名顺序</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">键名是系统消息中用于引用项目的名称，通常是项目的标签或名称。如果未指定 ItemType 的键名，将使用唯一的内部名称。您可以连接任意数量的属性值以有意义的方式标识项目。键名中使用的属性必须分配有顺序编号。例如：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/2ce491fe1ace.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">排序</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">控制主项目网格中显示的项目的顺序（从上到下），无论是手动还是自动搜索后。无论哪个属性的排序值最低，都是第一个控制顺序的。例如，如果您想按姓氏然后按名字对员工排序，姓氏在排序属性中的值为 1，名字的值为 2。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以下是按照上面设置的属性排序的几个员工的示例：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意姓氏是第一排序条件，然后在所有相同姓氏的成员中，Innovator 按名字排序（排序 = 20）。然后在相同姓氏和名字的成员中，Innovator 按社保号码排序。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b4d1e98c6fe9.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">默认值</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">未给出值时使用的属性默认值。空白表示空值。有时，为必填属性设置默认值是一个好主意。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">默认搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当用户进入主项目页面时，他们通常执行搜索，或为他们自动执行搜索（如果自动搜索属性被选中）。默认搜索属性的值作为搜索条件输入，显示在网格上方的蓝色行中。例如，在 GeometricShape 中，如果我们在名称属性的默认搜索列中输入 *cir*，则当执行搜索时，将显示名称中包含 &quot;cir&quot; 的所有项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">模式</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c050ace21992.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">这仅适用于字符串类型或过滤列表类型的属性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在以前的版本中，模式用于格式化日期类型的属性，但日期格式化现在是国际化和本地化的一部分。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4e3e38067160.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">如果属性是字符串类型，则模式将遵循基本正则表达式语法。这些模式用于服务器端的数据验证，应用于电话号码、电子邮件地址或社保号码等字段。例如，对于电话号码，您可以使用以下模式：\d{3}-\d{3}-\d{4}。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果属性是过滤列表类型，则模式是指向定义过滤器的属性的指针。有关更多信息，请参阅过滤列表。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">外部属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">此字段基本上回显为不同项目定义的属性的属性值。必须定义另一个类型为项目的属性（比如属性 A）。然后，外部属性可以引用属性 A 定义的项目的任何属性。此字段在保存 ItemType 定义时也会自动填充。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">例如，假设您想为 Part 创建一个 ItemType 定义。您想将工程规范与每个 Part 实例关联。因此，对于 Part ItemType 定义，您创建一个名为规范文档的属性，其类型为项目，数据源为文档。然后，您可以创建另一个名为规范 ID 的外部类型属性，其数据源将是规范文档的编号属性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目行为</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/53d5b5b2fbcd.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">对于数据类型为项目的属性，以及由属性指定的目标项目是可版本化的；项目行为指定固定或浮动。固定意味着此属性指向目标项目的特定世代号，即使创建了更新的世代号也是如此。浮动意味着此属性指向目标项目的最新世代号。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">类路径</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果项目具有类结构，您可以指定此属性属于哪个子类。所选子类以下的所有子类也将继承此属性。如果未指定子类，则假定该属性属于父 ItemType。有关更多信息，请参阅类结构。要插入属性的类路径，只需点击此单元格。类结构</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工具提示</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">帮助文本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">跟踪历史</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">This field basically echoes a property value of a property defined for a different Item. It is required that another property, say property A, of type Item is defined. Then, the Foreign Property, can reference any property of the item defined by property A. This field is also populated automatically when the ItemType definition is saved.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">For example, say you wanted to create an ItemType definition for a Part. You want to associate the engineering specification with each instance of Part. So, for the Part ItemType definition, you create a property, called Specification Document which is of type Item, and whose Data Source would be Document. Then, you can create another property, called Spec ID, of type Foreign, whose Data Source would point to the Name property of Document. Once you save the ItemType definition for Part, you should see name appear in the Foreign Property field. For more examples and definitions of how this works, see Foreign Data Type.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目行为</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">For properties where the data type = Item, and the target item specified by the property is versionable; Item Behavior specifies Fixed or Float. Fixed means that this property points to a specific generation of target item, even if newer ones are created. Float means that this property points to the latest generation of the target item.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">类路径</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">If the Item has a class structure, you can specify which subclass this property belongs to. All subclasses below the chosen subclass will inherit this property as well. If no subclass is specified, then it is assumed that the property belongs to the parent ItemType. For more information refer to Class Structure. To insert the class path for the property, simply click in this cell. The class structure dialog (shown below) is displayed. Select the subclass node to which this property belongs. Click the green check to complete your selection.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/370db5d605f2.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f08efc269e6d.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">工具提示</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">帮助文本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">跟踪历史</Run></Paragraph>
</FlowDocument>',
   N'输入 ItemType 属性 ItemType 中的属性选项卡包含项目的所有属性，包括系统属性。点击 ItemType 表单中的属性选项卡。您应该看到类似以下的内容： 当您创建新属性时，属性网格中会创建一个新行，然后需要由用户值填充。 创建属性 打开 ItemType 并点击。ItemType 被您认领，这意味着其他人无法编辑它。 点击属性选项卡。 点击添加行图标。网格底部将添加一个新行。 根据需',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [117/244] Erasing_a_Message.htm (3 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('c42e4bf519fb',
   N'删除消息',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">删除消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Erasing_a_Message.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">任何现有消息只能由消息的创建者或管理员删除。要删除消息，请执行以下操作：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1. 点击需要删除的消息下方的「删除」链接。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将出现类似以下的对话框。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ddb3d52bb041.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击确定删除消息，或点击取消退出此对话框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息被删除后，将在删除文本的位置显示新文本。消息的上下文为：消息已删除。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/8d9682645f7b.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">用户无法删除他们未创建的消息。其他用户创建的消息不包含删除链接。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：「历史记录」类型的消息无法删除。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7cf71d743f6f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">It is not possible for a user to erase a message they have not created. Messages created by other users do not contain the Erase link.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Note: Messages of type “History” cannot be erased.</Run></Paragraph>
</FlowDocument>',
   N'删除消息 任何现有消息只能由消息的创建者或管理员删除。要删除消息，请执行以下操作： 1. 点击需要删除的消息下方的「删除」链接。 将出现类似以下的对话框。 点击确定删除消息，或点击取消退出此对话框。 消息被删除后，将在删除文本的位置显示新文本。消息的上下文为：消息已删除。 用户无法删除他们未创建的消息。其他用户创建的消息不包含删除链接。 注意：「历史记录」类型的消息无法删除。 It is not ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [118/244] Events.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('3c6646d8eb7a',
   N'事件',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">事件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Events.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">事件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字段事件和表单事件使方法能够在客户端或服务器端基于用户操作执行。方法按方法部分中的描述存储在 Innovator 中。事件可以从用户操作（如鼠标事件（点击、移动））或页面操作（如加载或打印）触发。方法可以在客户端或服务器上执行，具体定义在事件的内部。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单中的事件触发器</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字段触发器</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单触发器</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">失去焦点时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打印后</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">值变更时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打印前</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">页面卸载前</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">双击时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">右键菜单时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">获得焦点时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">键盘按下时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选中时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">键盘抬起时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">加载时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">鼠标按下时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">鼠标悬停时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">鼠标移动时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">鼠标释放时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">窗口大小变更时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">上传时</Run></Paragraph>
</FlowDocument>',
   N'事件 字段事件和表单事件使方法能够在客户端或服务器端基于用户操作执行。方法按方法部分中的描述存储在 Innovator 中。事件可以从用户操作（如鼠标事件（点击、移动））或页面操作（如加载或打印）触发。方法可以在客户端或服务器上执行，具体定义在事件的内部。 表单中的事件触发器 字段触发器 表单触发器 OnBlur OnAfterPrint OnChange OnBeforePrint OnClic',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [119/244] Field_Border_Properties.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('87165269d49f',
   N'字段边框属性',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">字段边框属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Field_Border_Properties.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字段边框属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">值</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图例</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在标签上方插入文本，解释您想要的内容</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">边框宽度</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">值框边框的厚度</Run></Paragraph>
</FlowDocument>',
   N'字段边框属性 属性 值 图例 在标签上方插入文本，解释您想要的内容 边框宽度 值框边框的厚度',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [120/244] Field_Label_Properties.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('9b9312b457e8',
   N'字段标签属性',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">字段标签属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Field_Label_Properties.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字段标签属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">值</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标签</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户在表单上看到的标签。这是一个多语言字符串。参见国际化</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字体系列</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要显示的字体</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标签位置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标签将在值框中的位置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字体粗细</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">粗体或正常</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文本对齐</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">左对齐、右对齐或居中</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字体大小</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以磅为单位</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字体颜色</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用颜色选择器选择</Run></Paragraph>
</FlowDocument>',
   N'字段标签属性 属性 值 标签 用户在表单上看到的标签。这是一个多语言字符串。参见国际化 字体系列 要显示的字体 标签位置 标签将在值框中的位置 字体粗细 粗体或正常 文本对齐 左对齐、右对齐或居中 字体大小 以磅为单位 字体颜色 使用颜色选择器选择',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [121/244] Field_Physical_Properties.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('a7620a9b12da',
   N'字段物理属性',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">字段物理属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Field_Physical_Properties.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字段物理属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">定位</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">窗口中的绝对位置或相对于表单左上角的相对位置。建议使用绝对定位</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">X</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">水平位置（以像素为单位）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Y</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">垂直位置（以像素为单位）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Z 轴顺序</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">具有较高 Z-Index 的字段将绘制在具有较低 Z-Index 的字段之上</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可见</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字段是否可见</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">禁用</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字段是否可编辑</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Tab 索引</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果设置了 Tab 停止，Tab 键在字段之间移动的顺序</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Tab 停止</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字段是否可以使用 Tab 键选择</Run></Paragraph>
</FlowDocument>',
   N'字段物理属性 属性 说明 定位 窗口中的绝对位置或相对于表单左上角的相对位置。建议使用绝对定位 X 水平位置（以像素为单位） Y 垂直位置（以像素为单位） Z-Index 具有较高 Z-Index 的字段将绘制在具有较低 Z-Index 的字段之上 可见 字段是否可见 禁用 字段是否可编辑 Tab 索引 如果设置了 Tab 停止，Tab 键在字段之间移动的顺序 Tab 停止 字段是否可以使用 Ta',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [122/244] Field_Type_Properties.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('e90bc673a23d',
   N'字段类型属性',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">字段类型属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Field_Type_Properties.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字段类型属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">值</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">值框的名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示长度</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单正文</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字段类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">值框类型的下拉选择</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">数据源</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将从中获取数据的字段</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">背景颜色</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字段背景的颜色</Run></Paragraph>
</FlowDocument>',
   N'字段类型属性 属性 值 名称 值框的名称 显示长度 表单正文 字段类型 值框类型的下拉选择 数据源 将从中获取数据的字段 背景颜色 字段背景的颜色',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [123/244] FileType_Properties.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('06b2009ec0e2',
   N'文件类型属性',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">文件类型属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: FileType_Properties.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文件类型属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">必填</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">是</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文件类型的名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">扩展名（Ext.）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">是</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">扩展类型（.doc、.PDF、.txt 等）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">MIME 类型（内容类型）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">是</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文件将表示的正确 MIME 类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">描述</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">否</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">MIME 类型的描述</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">规则类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">否</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">定义从文件读取多少字节</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">模式</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">否</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">由规则类型定义的前 X 个字节的值，需要找到匹配项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">优先级</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">否</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">定义规则使用的顺序</Run></Paragraph>
</FlowDocument>',
   N'文件类型属性 属性名称 必填 说明 名称 是 文件类型的名称 扩展名（Ext.） 是 扩展类型（.doc、.PDF、.txt 等） MIME 类型（内容类型） 是 文件将表示的正确 MIME 类型 描述 否 MIME 类型的描述 规则类型 否 定义从文件读取多少字节 模式 否 由规则类型定义的前 X 个字节的值，需要找到匹配项 优先级 否 定义规则使用的顺序',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [124/244] File_menu.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('1b85178fa8a6',
   N'文件菜单',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">文件菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: File_menu.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文件菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文件菜单提供执行基本任务的各种选项，如创建、保存、打开、导出到 Microsoft Excel 或 Word 等。下表说明了文件菜单中每个可用选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文件菜单中的某些选项可能显示为禁用状态。菜单选项在选择适当项目且登录用户具有足够访问权限时启用。访问权限由管理员配置。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">菜单选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">新建</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建新项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将所选项目保存到数据库。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">另存为</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您创建所选项目的副本。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以只读模式打开所选文件项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在大多数情况下，此选项仅对管理员用户启用。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">下载</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将所选文件项目的副本下载到工作目录。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在大多数情况下，此选项仅对管理员用户启用。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">签入</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从工作目录签入文件并解锁。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">签出</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将当前文件签出到工作目录并锁定。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">签出到目录</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将当前文件签出到您指定的目录。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">撤销签出</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将文件恢复到签出之前的版本。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可打印视图</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将活动窗口打印到客户端打印驱动程序。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">导出到 Excel</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将主网格中列出的项目属性导出到 Microsoft Excel 工作表。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">导出到 Word</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将主网格中列出的项目属性导出到 Microsoft Word 文档。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注销</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注销 Aras Innovator 会话或关闭打开的窗口（仅适用于独立窗口）。</Run></Paragraph>
</FlowDocument>',
   N'文件菜单 文件菜单提供执行基本任务的各种选项，如创建、保存、打开、导出到 Microsoft Excel 或 Word 等。下表说明了文件菜单中每个可用选项。 文件菜单中的某些选项可能显示为禁用状态。菜单选项在选择适当项目且登录用户具有足够访问权限时启用。访问权限由管理员配置。 菜单选项 说明 新建 创建新项目。 保存 将所选项目保存到数据库。 另存为 允许您创建所选项目的副本。 打开 以只读模式',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [125/244] File_Menu_(Item_Window).htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('15b6c081f310',
   N'文件菜单（项目窗口）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">文件菜单（项目窗口）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: File_Menu_(Item_Window).htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文件菜单（项目窗口）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文件菜单提供执行基本任务的各种选项，如创建、保存、打开、导出到 Microsoft Excel 或 Word 等。下表说明了文件菜单中每个可用选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文件菜单中的某些选项可能显示为禁用状态。菜单选项在选择适当项目且登录用户具有足够访问权限时启用。访问权限由管理员配置。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">菜单选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">新建</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建新项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将打开的项目保存到数据库。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以只读模式打开所选文件项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在大多数情况下，此选项仅对管理员用户启用。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">下载</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将所选文件项目的副本下载到工作目录。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在大多数情况下，此选项仅对管理员用户启用。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">导出到 Excel</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将打开的项目的所有项目属性和关系属性导出到 Microsoft Excel 工作表。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">导出到 Word</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将打开的项目的所有项目属性和关系属性导出到 Microsoft Word 文档。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存、解锁并关闭</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存对项目所做的更改，解锁项目，然后关闭项目窗口。</Run></Paragraph>
</FlowDocument>',
   N'文件菜单（项目窗口） 文件菜单提供执行基本任务的各种选项，如创建、保存、打开、导出到 Microsoft Excel 或 Word 等。下表说明了文件菜单中每个可用选项。 文件菜单中的某些选项可能显示为禁用状态。菜单选项在选择适当项目且登录用户具有足够访问权限时启用。访问权限由管理员配置。 菜单选项 说明 新建 创建新项目。 保存 将打开的项目保存到数据库。 打开 以只读模式打开所选文件项目。 在',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [126/244] File_Properties.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('4ba57b630f30',
   N'文件属性',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">文件属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: File_Properties.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文件属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">必填</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文件名</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">是</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">存储在保险柜中的文件名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文件类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">是</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">由扩展名指示的文件类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文件大小</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">是</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文件大小（以字节为单位）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">签出路径</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">否</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当文件被签出进行编辑时，文件本地副本的位置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注释</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">否</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注释字段</Run></Paragraph>
</FlowDocument>',
   N'文件属性 属性名称 必填 说明 文件名 是 存储在保险柜中的文件名称 文件类型 是 由扩展名指示的文件类型 文件大小 是 文件大小（以字节为单位） 签出路径 否 当文件被签出进行编辑时，文件本地副本的位置 注释 否 注释字段',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [127/244] Flagging_a_Message.htm (5 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('a5b4f6d984d8',
   N'标记消息',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">标记消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Flagging_a_Message.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标记消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户可以在讨论面板中标记或取消标记任何消息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要标记消息，请点击「标记」链接。标记的颜色从灰色变为黄色，表示消息已被用户标记。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要取消标记消息，请点击「取消标记」链接。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击数字超链接以打开显示已标记消息的人员。如下图所示：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4d1af9479ba6.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">下表中的示例描述了标记功能的各种状态：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标记状态</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ceda206e2cd0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">含义</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">没有用户标记该消息，包括该消息的创建者。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">六位用户标记了该消息，但不包括创建者。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">包括创建者在内的十七位用户标记了该消息。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/904ad03f4010.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">注意：用户可以关注由特定用户标记的消息。有关更多信息，请点击关注。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ea1c8cb64cc2.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">六位用户标记了该消息，但不包括创建者。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7dc85d848f5f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Seventeen users including the creator have flagged the message.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Note: A user can follow messages that are flagged by specific users. For more information on this, click on Follow.</Run></Paragraph>
</FlowDocument>',
   N'标记消息 用户可以在讨论面板中标记或取消标记任何消息。 要标记消息，请点击「标记」链接。标记的颜色从灰色变为黄色，表示消息已被用户标记。 要取消标记消息，请点击「取消标记」链接。 点击数字超链接以打开显示已标记消息的人员。如下图所示： 下表中的示例描述了标记功能的各种状态： 标记状态 含义 没有用户标记该消息，包括该消息的创建者。 六位用户标记了该消息，但不包括创建者。 包括创建者在内的十七位用户',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [128/244] Follow.htm (3 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('e9d84d1e95f6',
   N'关注',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关注</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Follow.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关注</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">书签面板中的关注命令使您能够关注其他用户。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">为此，请在书签面板中的「来自消息」、「标记人」和「论坛」书签中添加新书签。您可以关注：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">特定用户创建的消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">特定用户标记的消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属于特定论坛的消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">添加新书签</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击书签面板中的「人员」按钮。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将出现关注对话框：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f380cd16cbc7.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">2. 开始输入您希望关注的人的姓名，然后在姓名出现时点击它。它将添加到您正在关注的人员列表中。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/2db83eb93e40.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">2. Start typing the name of the person you wish to follow and then click on the name when it comes up. It is added to the list of people you are following.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6bf5a6593cab.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'关注 书签面板中的关注命令使您能够关注其他用户。 为此，请在书签面板中的「来自消息」、「标记人」和「论坛」书签中添加新书签。您可以关注： 特定用户创建的消息 特定用户标记的消息 属于特定论坛的消息 添加新书签 点击书签面板中的「人员」按钮。 将出现关注对话框： 2. 开始输入您希望关注的人的姓名，然后在姓名出现时点击它。它将添加到您正在关注的人员列表中。 2. Start typing the n',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [129/244] Foreign_Property.htm (5 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('74c6540471be',
   N'外部数据类型',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">外部数据类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Foreign_Property.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">外部数据类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">外部数据类型用于将属于项目1的属性表示为项目2的属性。在下面的示例中，我们将创建一个新的 ItemType 定义，名为 TestPart，并为其赋予两个属性 - 第一个将是项目类型，将引用文档 ItemType；第二个将引用文档 ItemType 的一个属性，名为名称。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建外部数据类型：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中，点击 管理 &gt; ItemTypes。将出现以下菜单：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/32c5415833d5.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击「创建新 ItemType」。将出现类似以下的窗口：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ac790ba9d124.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">在适当字段中输入信息，如下截图所示：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">输入以下属性值：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/93e88419ee59.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">自动搜索 - 选中</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">TOC 访问 - 或您选择的任何其他类别和访问权限（参见 TOC 访问）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可添加 - 允许 World 身份添加（参见可添加）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">权限 - 允许 World 身份获取、更新、删除和更改访问权限（参见权限）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">添加名为 SpecDocument 的属性，将其设为数据类型为项目，数据源应设置为文档。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">添加另一个名为 SpecID 的属性，将其设为数据类型为外部。当您选择数据源列时，您将看到一个外部属性选择对话框，列出此新 ItemType 的所有项目类型属性（您只能选择已声明为属性的项目的属性）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择规范文档，因为这是我们想要访问其属性的项目，通过选择左侧的加号图标打开其属性列表。属性列表如下所示：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">通过双击选择名称属性。对话框将关闭，但它不会在任何地方显示属性名称。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存 ItemType 定义。您应该看到名称属性出现在 SpecID 属性行的外部属性列中。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3b9efcba9a6f.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">一旦您为 ItemType 创建了外部属性，让我们看看它在表单上的显示方式。很可能您根本不想让它出现在表单上，但如果出现，用户将无法访问它。以下是 TestPart 实例的外观：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">一旦您选择了 SpecDocument 并保存项目，那么 SpecID（外部属性）将自动填充，并且是只读的。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Once you have created a Foreign property for your ItemType, let&apos;s see how it appears on the form. Most likely you would not want it to appear on the form at all, but if you do, it will not be accessible by the user. Here is how the TestPart instance would look:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3f36a800dc43.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Once you select the SpecDocument, and save the item, then the Spec ID (the Foreign Property) is automatically filled in, and is read only.</Run></Paragraph>
</FlowDocument>',
   N'外部数据类型 外部数据类型用于将属于项目1的属性表示为项目2的属性。在下面的示例中，我们将创建一个新的 ItemType 定义，名为 TestPart，并为其赋予两个属性 - 第一个将是项目类型，将引用文档 ItemType；第二个将引用文档 ItemType 的一个属性，名为名称。 创建外部数据类型： 在导航窗格中，点击 管理 > ItemTypes。将出现以下菜单： 点击「创建新 ItemTy',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [130/244] Form_Body_Properties.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('a9d075c33ede',
   N'表单正文属性',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">表单正文属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Form_Body_Properties.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单正文属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">值</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">颜色</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单的背景颜色</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图像</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单背景的图像</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">平铺</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当图像需要平铺时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">附件</Run></Paragraph>
</FlowDocument>',
   N'表单正文属性 属性 值 颜色 表单的背景颜色 图像 表单背景的图像 平铺 当图像需要平铺时 附件',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [131/244] Form_Event_Properties.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('53f6b818aa4a',
   N'表单事件属性',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">表单事件属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Form_Event_Properties.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单事件属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">值</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">新建</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击新建图标</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关联现有</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击关联现有图标</Run></Paragraph>
</FlowDocument>',
   N'表单事件属性 属性 值 新建 点击新建图标 关联现有 点击关联现有图标',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [132/244] Form_Fields.htm (12 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('21f8ddd8c5b2',
   N'表单字段',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">表单字段</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Form_Fields.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单字段</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以下字段可在表单上使用：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字段类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">快速访问按钮</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标准文本字段</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">密码</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7bcd1f88c831.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">通过 MD5 隐藏文本的文本字段</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文本区域</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">带有滚动条的标准文本区域</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9bd2b962cd3b.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">复选框</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标准复选框</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">格式化文本</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e4a46284bfad.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">带有基本格式选项的文本区域（文本区域的替代方案）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标签</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标准标签字段</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/254cd0a54e7e.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">按钮</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标准按钮</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">单选按钮列表</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标准单选按钮集合（下拉列表的替代方案）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">下拉框</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标准下拉列表</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">列表框单选</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/25713ce99f06.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">带有滚动条的标准列表框（下拉列表的替代方案）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">列表框多选</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">带有滚动条的标准列表框，支持多选</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/45d4bf6824c5.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">复选框列表</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标准复选框集合（列表多选的替代方案）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">日期</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/160fa1551cf6.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">带有弹出日历的文本字段，用于选择日期</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">颜色</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">带有弹出颜色选择器的字段</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5124a76365cd.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">颜色列表</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">颜色的下拉列表</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">HTML</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ebf507b03e0d.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">可以包含自定义 HTML 代码（包括脚本）的字段</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">带有省略号按钮的文本字段，用于搜索另一个项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文件项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用户能够浏览和选择附件文件的字段</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9496476a7904.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">类结构</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用户能够对项目进行分类的字段</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图像</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/240fbc16d9aa.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">使用户能够选择图像的字段</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">多语言字符串</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用户能够输入不同语言的多个条目的文本字段</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">分组框</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将其他字段分组在一起的字段</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b4e6d9a192c8.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">A text field with an ellipsis button next to it, enabling a search for another item</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文件项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">A field that enables the user to browse and select an attachment file</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">类结构</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许用户对项目进行分类的字段</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图片</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许用户选择图像的字段</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">多语言字符串</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">A text field that enables the user to enter multiple entries in different languages</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">分组框</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将其他字段分组在一起的字段</Run></Paragraph>
</FlowDocument>',
   N'表单字段 以下字段可在表单上使用： 字段类型 说明 快速访问按钮 文本 标准文本字段 密码 通过 MD5 隐藏文本的文本字段 文本区域 带有滚动条的标准文本区域 复选框 标准复选框 格式化文本 带有基本格式选项的文本区域（文本区域的替代方案） 标签 标准标签字段 按钮 标准按钮 单选按钮列表 标准单选按钮集合（下拉列表的替代方案） 下拉框 标准下拉列表 列表框单选 带有滚动条的标准列表框（下拉列表',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [133/244] Form_field_types.htm (35 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('8ff538f82a25',
   N'表单字段类型',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">表单字段类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Form_field_types.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单字段类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单由各种类型的表单字段组成。表单字段构成用户的数据输入区域。有不同类型的字段。让我们以 CAD ItemType 表单为例来查看不同的字段类型。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字段类型</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/95fc4fce630c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">表单上的字段名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文本框：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文档编号</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/fb6ca2ffdffa.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">表单上浅蓝色的文本框表示要提供的信息对于 ItemType 是必填的。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">例如，在 CAD 表单中，文档编号信息是用户在保存表单之前必须提供的。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文本框：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">名称</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/8fdf9f73b5a2.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">版本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在文本框中输入信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图像选择器：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择图像</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/164d3584d5d2.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击选择图像。有关选择图像的更多信息，请参阅图像选择器。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">分类：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">类型</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5a05801cb61b.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">与当前表单关联的 ItemType 分类。有关如何选择分类的更多信息，请参阅类型-分类。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">下拉框：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创作工具</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a13933a6bbcd.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击查看列表选项并从列表中选择所需选项。您也可以在字段中输入字母以查看以该字母开头的条目列表：有关更多信息，请参阅使用自动输入。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文本区域：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f558c6d4984c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/dee4a50aeaaa.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">描述</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在文本区域中输入信息。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/76c4ab39ffe3.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">项目选择器：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">指定创建者</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">指定用户</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7dd3aba5a525.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击打开对话框，搜索然后选择项目。有关更多信息，请参阅选择项目。您也可以在字段中输入字母以查看以该字母开头的项目列表：有关更多信息，请参阅使用自动输入。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文件选择器：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">原始文件</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/805ddc74d192.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">可查看文件</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a6c92ec06603.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击文本框上传文件。有关更多信息，请参阅选择文件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击删除已上传的文件。点击签出已上传的文件。这两个按钮仅在文件上传后才可用。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">日期选择器：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">生效日期</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/8ef204a3a1bc.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/aea0abb1c64d.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击选择日期和时间。有关更多信息，请参阅日期对话框。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3f57e2837056.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">复选框：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标准</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/22ec00fd41f7.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">模板</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/46abe402522c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击选择复选框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图像选择器</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在表单中，点击「选择图像...」链接。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将显示图像浏览器对话框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择图像：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Innovator：点击从可用图像中选择图像。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">：以列表视图显示可用图标的列表及文件详细信息。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9afa301b6ec2.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">：显示所有可用图标。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">：确认图像选择并关闭图像选择器对话框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">：清除图像选择</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f5a569f089f2.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">外部文件：点击浏览外部图像并选择外部图像。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/225b42cd7534.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击确定。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/65c75ab80093.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">日期对话框</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f674b6fbc9ff.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">在项目表单中，点击日期选择器。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将显示日期对话框。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/65c75ab80093.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">默认情况下，选择当前日期和时间。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击日历中的所需日期以选择日期。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/22ec00fd41f7.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击清除日期选择。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">时间</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5087977eef9f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">无：选择以将 12:00:00 AM 配置为时间。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">HH:MM:SS 格式：选择以 HH:MM:SS 格式选择当前时间，其中 H=小时，M=分钟，S=秒。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">时区</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/87817f6eeb98.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">公司：选择已配置的公司时区。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">本地：选择本地客户端机器中配置的时间。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择文件/上传文件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在表单中，点击文件框。例如，在 CAD 表单中，点击原始文件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将显示打开对话框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择要上传的文件并点击打开。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选定的文件将上传到默认保险柜。文件上传后，将添加两个按钮。点击删除已上传的文件。点击签出已上传的文件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">类型-分类</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e938d5a7b61e.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">ItemType 可能包含子类结构，其中每个类可以引用其自己的特定属性、表单和生命周期。分类允许您在子类之间共享属性。如果您在父级别类定义属性，所有子类都将继承这些属性。这些子类中的每一个都可以有额外的特定属性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">例如，在 CAD 表单中，点击类型以查看 CAD ItemType 的类结构。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/975bb0d03518.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">将显示分类对话框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在此示例中，CAD ItemType 有一个定义的类结构。CAD 可以表示为机械或电子。由于每个子类共享公共属性，使用类结构可能比创建全新的 ItemType 更高效。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/8ef204a3a1bc.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/aea0abb1c64d.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">选择项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在表单中，点击项目选择器。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将显示搜索对话框。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d391f48b3efb.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">指定搜索条件并点击搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索网格显示与搜索条件匹配的项目列表。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f385102e1717.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">从网格中选择项目并点击确定。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在表单中，点击。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/96bd317e70dd.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">显示搜索对话框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">指定搜索条件并点击。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4e972777ad17.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The search grid displays list of Items that match the search criteria.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从网格中选择项目并点击。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/65c75ab80093.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'表单字段类型 表单由各种类型的表单字段组成。表单字段构成用户的数据输入区域。有不同类型的字段。让我们以 CAD ItemType 表单为例来查看不同的字段类型。 字段类型 表单上的字段名称 说明 文本框： 文档编号 表单上浅蓝色的文本框表示要提供的信息对于 ItemType 是必填的。 例如，在 CAD 表单中，文档编号信息是用户在保存表单之前必须提供的。 文本框： 名称 版本 在文本框中输入信息',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [134/244] Form_Layout_Designer_Toolbar_Icons.htm (13 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('dc84a3e2bbfd',
   N'表单布局设计器工具栏图标',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">表单布局设计器工具栏图标</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Form_Layout_Designer_Toolbar_Icons.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单布局设计器工具栏图标</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字段类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图标</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除活动的表单元素</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/af7f50b9aaf9.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">文本字段</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">插入新的文本字段元素</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">密码</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9b8e851fa329.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">插入新的隐藏字符文本字段元素</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文本区域</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">插入新的文本区域元素</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0e34bf50ca06.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">下拉列表</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">插入新的下拉列表元素</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">列表框</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3e74fb5cd1e4.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">插入新的列表框元素</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">多选列表框</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">插入新的多选列表框元素</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c2b449497f4c.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">复选框</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">插入复选框元素</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">单选按钮</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0afa1a9e2600.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">插入单选按钮元素</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">按钮</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">插入按钮元素</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/1acbf9065f37.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">日历</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">插入日历元素</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">颜色</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/45350f0638b9.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">插入颜色元素</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图像</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">插入图像元素</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/98ed752101aa.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">插入单选按钮元素</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">按钮</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/29281396ce2a.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">插入按钮元素</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">日历</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d623133df212.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">插入日历元素</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">颜色</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3a26c5f78914.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">插入颜色元素</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图片</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/05c7b28a8241.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">插入图像元素</Run></Paragraph>
</FlowDocument>',
   N'表单布局设计器工具栏图标 字段类型 图标 说明 删除 删除活动的表单元素 文本字段 插入新的文本字段元素 密码 插入新的隐藏字符文本字段元素 文本区域 插入新的文本区域元素 下拉列表 插入新的下拉列表元素 列表框 插入新的列表框元素 多选列表框 插入新的多选列表框元素 复选框 插入复选框元素 单选按钮 插入单选按钮元素 按钮 插入按钮元素 日历 插入日历元素 颜色 插入颜色元素 图像 插入图像元素',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [135/244] Form_Properties.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('8a44a9159844',
   N'表单属性',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">表单属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Form_Properties.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">必填</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">是</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单的名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">描述</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">否</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单的描述</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">宽度</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">否</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单打开时的宽度</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">样式表</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">否</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开表单时应用的样式表</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高度</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">否</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单打开时的高度</Run></Paragraph>
</FlowDocument>',
   N'表单属性 属性名称 必填 说明 名称 是 表单的名称 描述 否 表单的描述 宽度 否 表单打开时的宽度 样式表 否 打开表单时应用的样式表 高度 否 表单打开时的高度',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [136/244] Form_Property_Tabs.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('49c4e2b1f93d',
   N'表单属性选项卡',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">表单属性选项卡</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Form_Property_Tabs.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单属性选项卡</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字段属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单页面属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字段类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字段标签</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单正文</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字段物理</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单事件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字段边框</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单边框</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">字段事件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单事件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果您同时打开了多个表单项目，可以使用拖放重新排列选项卡。</Run></Paragraph>
</FlowDocument>',
   N'表单属性选项卡 字段属性 表单页面属性 字段类型 表单属性 字段标签 表单正文 字段物理 表单事件 字段边框 表单边框 字段事件 表单事件 如果您同时打开了多个表单项目，可以使用拖放重新排列选项卡。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [137/244] Forums.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('82921073ef2f',
   N'论坛',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">论坛</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Forums.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">论坛</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">论坛允许对任何主题进行安全的社交讨论。它们包含项目和保存的搜索。论坛不包含其他论坛，但包含对特定主题的讨论。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击论坛将显示来自所有包含项目的聚合消息。点击论坛内的某个项目或保存的搜索将仅显示该项目的消息内容。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4b56403803b7.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">个人</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">与特定用户和群组共享</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">对所有人可用</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要了解有关论坛的更多信息，请参阅以下主题：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">添加论坛</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将保存的搜索添加到论坛</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">论坛权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">论坛权限</Run></Paragraph>
</FlowDocument>',
   N'论坛 论坛允许对任何主题进行安全的社交讨论。它们包含项目和保存的搜索。论坛不包含其他论坛，但包含对特定主题的讨论。 点击论坛将显示来自所有包含项目的聚合消息。点击论坛内的某个项目或保存的搜索将仅显示该项目的消息内容。 个人 与特定用户和群组共享 对所有人可用 要了解有关论坛的更多信息，请参阅以下主题： 添加论坛 将保存的搜索添加到论坛 论坛权限 Forum Permissions',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [138/244] Fourm_Permissions.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('8ec5ff45a6bf',
   N'论坛权限',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">论坛权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Fourm_Permissions.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">论坛权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从论坛添加或移除项目，或设置论坛共享的能力仅限于论坛的所有者和管理员。其他有权访问论坛的用户可以看到论坛中的消息并向其发布消息，但他们无法更改论坛的项目内容或更改其共享设置。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户无法移除任何「默认组」书签 — 所有书签、来自我的消息和我的标记。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">没有最终用户功能来停用或删除论坛。用户可以从其书签中移除论坛。</Run></Paragraph>
</FlowDocument>',
   N'论坛权限 从论坛添加或移除项目，或设置论坛共享的能力仅限于论坛的所有者和管理员。其他有权访问论坛的用户可以看到论坛中的消息并向其发布消息，但他们无法更改论坛的项目内容或更改其共享设置。 用户无法移除任何「默认组」书签 — 所有书签、来自我的消息和我的标记。 没有最终用户功能来停用或删除论坛。用户可以从其书签中移除论坛。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [139/244] Grouping_Identities.htm (7 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('68f1f73a1248',
   N'创建和分组身份',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建和分组身份</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Grouping_Identities.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建和分组身份</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">身份，特别是组身份，在 Innovator 中有多种用途。以下是这些用途和目的的一些示例：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">定义和控制用户/组安全性，或特定组允许的权限。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">确定可以对项目执行的操作，如添加、更新、删除和/或发现权限。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">确定活动分配，例如工作流步骤中的审批流程。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可以提升生命周期状态并分配给工作流活动。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可以使用组身份创建多层层次结构访问。身份的所有成员继承该身份的访问特权。成员通过身份 ItemType 中的「成员」选项卡进行分配。用户别名身份可以属于多个组身份，在这种情况下，用户的访问权限或权限将是累积的。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">下表提供了典型身份列表。所有新用户都会自动添加到 World。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以下是创建新身份并添加其他身份作为其成员的方法：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从目录 --&gt; 管理 中，选择身份。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/033350318cd0.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击新建项目图标以显示身份表单：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">填写以下属性：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">a. 名称 - 身份的名称。这是必填值。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7d45a9287528.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">b. 描述 - 身份的描述，通常用于识别目的。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6eae81c33479.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">c. 是别名 - 指示此身份是否对应于特定的登录用户。组身份不是别名。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">d. 成员选项卡 - 列出此组身份的所有成员。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要向身份添加成员，有两个选择 - 添加现有身份，或在添加时创建新身份。每个选项都有其优势。如果您正在创建新结构（如管理用户中所述），那么当场创建新身份是一个好主意。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要添加新身份，请使用操作下拉框中的「创建关联」选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要添加现有成员，请使用操作下拉框中的「选择关联」选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">a. 点击操作下拉框旁边的新建项目图标。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">b. 如果创建新身份，只需输入新身份的名称。如果添加现有身份，将打开搜索对话框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">c. 选择要添加为成员的身份，然后点击保存、解锁并关闭图标退出对话框。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/26093e1e6b0f.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">To add existing members, use the Pick Related option from the Actions drop down box.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/93f5e96e767d.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">a. Click on the New Item icon next to the Action drop down box.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7d45a9287528.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">b. If creating a new identity, just type in the name of the new identity. If adding an existent identity, a search dialog will open.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">c. Select the identity or identities to add as members, and click the Save, Unlock and Close icon to exit the dialog.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/370db5d605f2.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'创建和分组身份 身份，特别是组身份，在 Innovator 中有多种用途。以下是这些用途和目的的一些示例： 定义和控制用户/组安全性，或特定组允许的权限。 确定可以对项目执行的操作，如添加、更新、删除和/或发现权限。 确定活动分配，例如工作流步骤中的审批流程。 可以提升生命周期状态并分配给工作流活动。 可以使用组身份创建多层层次结构访问。身份的所有成员继承该身份的访问特权。成员通过身份 ItemT',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [140/244] Help_Conventions.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('d13342bd7574',
   N'帮助约定',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">帮助约定</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Help_Conventions.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">帮助约定</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">下表突出显示了帮助中使用的约定：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">约定</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">粗体</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">强调菜单项、对话框、对话框元素和命令的名称。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例：点击确定。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">代码</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">代码示例以等宽字体显示。它可能代表您键入的文本或您读取的数据。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">黄色高亮</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以黄色高亮的代码提请注意内容中指示的代码。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">黄色高亮和红色文本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以黄色高亮的红色文本指示需要更改或替换的代码参数。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">斜体</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">引用其他文档。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注释包含其他有用信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">警告包含重要信息。请特别注意以这种方式突出显示的信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">提示包含 Aras 的建议。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">连续菜单选择</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">连续菜单选择可能在您将连续选择的项目之间出现 --&gt; 符号。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例：导航到 文件 --&gt; 保存 --&gt; 确定。</Run></Paragraph>
</FlowDocument>',
   N'帮助约定 下表突出显示了帮助中使用的约定： 约定 说明 粗体 强调菜单项、对话框、对话框元素和命令的名称。 示例：点击确定。 代码 代码示例以等宽字体显示。它可能代表您键入的文本或您读取的数据。 黄色高亮 以黄色高亮的代码提请注意内容中指示的代码。 黄色高亮和红色文本 以黄色高亮的红色文本指示需要更改或替换的代码参数。 斜体 引用其他文档。 注释包含其他有用信息。 警告包含重要信息。请特别注意以这',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [141/244] Help_menu.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('76858e9db0e5',
   N'帮助菜单',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">帮助菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Help_menu.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">帮助菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">帮助菜单提供查看在线帮助、产品版本和登录会话详细信息的选项。下表说明了帮助菜单中每个可用选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">菜单选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">询问 Innovator</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示「询问 Innovator」在线帮助。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于我的会话</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示登录用户名、数据库名称和正在访问的服务器名称等信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示产品版本和当前构建信息。</Run></Paragraph>
</FlowDocument>',
   N'帮助菜单 帮助菜单提供查看在线帮助、产品版本和登录会话详细信息的选项。下表说明了帮助菜单中每个可用选项。 菜单选项 说明 询问 Innovator 显示「询问 Innovator」在线帮助。 关于我的会话 显示登录用户名、数据库名称和正在访问的服务器名称等信息。 关于 显示产品版本和当前构建信息。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [142/244] Help_Menu_(Item_Window).htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('15dbdd029f02',
   N'帮助菜单（项目窗口）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">帮助菜单（项目窗口）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Help_Menu_(Item_Window).htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">帮助菜单（项目窗口）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">帮助菜单提供查看在线帮助和产品版本详细信息的选项。下表说明了帮助菜单中每个可用选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">菜单选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">询问 Innovator</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示「询问 Innovator」在线帮助。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示产品版本和当前构建信息。</Run></Paragraph>
</FlowDocument>',
   N'帮助菜单（项目窗口） 帮助菜单提供查看在线帮助和产品版本详细信息的选项。下表说明了帮助菜单中每个可用选项。 菜单选项 说明 询问 Innovator 显示「询问 Innovator」在线帮助。 关于 显示产品版本和当前构建信息。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [143/244] how_i8n_works_in_innovator.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('75c6d3cb5952',
   N'国际化和本地化行为',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">国际化和本地化行为</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: how_i8n_works_in_innovator.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">国际化和本地化行为</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当用户登录时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Aras Innovator 客户端检查客户端设置和服务器变量，并建立会话上下文，该上下文将持续到会话结束。Aras Innovator 确定客户端区域设置是否是数据库中的区域设置之一。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Aras Innovator 确定是否设置了公司时区：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果已设置，它会计算客户端时区之间的偏移量并采用客户端时区</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果未设置，到公司时区的偏移量设置为零并采用客户端时区</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果公司时间偏移量为零，Innovator 客户端中仅显示本地时间</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果公司时间偏移量不为零，Innovator 客户端中同时显示公司时间和本地时间</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当客户端请求信息时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">对于每个多语言字符串类型的值（最常见的是菜单、标签或列表值），Innovator 检查是否有上下文语言的可用值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果有，服务器返回上下文语言的值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果没有，服务器返回默认语言的值。请注意，如果未定义此值，它可能为空。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">对于每个日期或时间类型的值，服务器会根据会话公司时区调整值，该值可能为 0。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">对于每个日期、时间或小数类型的值，标准 Innovator 客户端使用客户端区域设置文化进行格式化（请注意，替代客户端需要执行此步骤以支持此功能）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果是，采用 Innovator 区域设置及其 Innovator 语言</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果不是，采用默认区域设置和语言</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有关管理国际化的更多信息，请参阅发行版 CD 上的文档。</Run></Paragraph>
</FlowDocument>',
   N'国际化和本地化行为 当用户登录时 Aras Innovator 客户端检查客户端设置和服务器变量，并建立会话上下文，该上下文将持续到会话结束。Aras Innovator 确定客户端区域设置是否是数据库中的区域设置之一。 Aras Innovator 确定是否设置了公司时区： 如果已设置，它会计算客户端时区之间的偏移量并采用客户端时区 如果未设置，到公司时区的偏移量设置为零并采用客户端时区 如果公',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [144/244] implementation_types.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('19f7911275e7',
   N'实现类型',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">实现类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: implementation_types.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">实现类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType 可以用两种不同的实现方式定义 - 单个项目和多态项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">单个项目</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/466740d74901.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">单个项目类型是最常见的实现方法。在单个项目中，每个 ItemType 表示一个单独的业务对象，并在关系数据库中表示为单个表。单个项目类型的示例包括 Part、Document、Project、DFMEA 等定义。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">多态项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">多态项目实现类型表示 ItemType 的多态类。多态项目 ItemType 可以表示其他 ItemType 的集合。多态（Poly Item）ItemType 通常在某个其他项目的属性可以引用多种可能类型的项目时使用 - 例如项目可交付物可能是文档、零部件或其他类型。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">多态项目类型具有一组有限的兼容关系，适用于 ItemType 定义。只有在 ItemType 表单中保持活动状态的选项卡才在多态项目类型中受支持。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The Poly Item type has a somewhat limited set of compatible relationships that are applicable in the ItemType definition. Only those Tabs that remain active in the ItemType form are supported in a Poly Item type.</Run></Paragraph>
</FlowDocument>',
   N'实现类型 ItemType 可以用两种不同的实现方式定义 - 单个项目和多态项目。 单个项目 单个项目类型是最常见的实现方法。在单个项目中，每个 ItemType 表示一个单独的业务对象，并在关系数据库中表示为单个表。单个项目类型的示例包括 Part、Document、Project、DFMEA 等定义。 多态项目 多态项目实现类型表示 ItemType 的多态类。多态项目 ItemType 可以',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [145/244] index.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('c14688d913d8',
   N'询问 Innovator',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">询问 Innovator</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: index.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">询问 Innovator</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">%</Run></Paragraph>
</FlowDocument>',
   N'询问 Innovator %',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [146/244] index1.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('2b41e030739b',
   N'询问 Innovator',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">询问 Innovator</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: index1.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">%</Run></Paragraph>
</FlowDocument>',
   N'%',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [147/244] about i18n.htm (8 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('c8004a69f207',
   N'关于 Innovator 的国际化和本地化',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于 Innovator 的国际化和本地化</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: about i18n.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于 Innovator 的国际化和本地化</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从 9.0 版本开始，Aras Innovator 支持以下功能的国际化和本地化：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">英语是默认语言</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可以使用语言包创建或添加其他语言</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">多语言字符串数据类型用于以多种语言显示值</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">语言包显示适合客户端区域设置的菜单、标签和值，使用户能够使用已安装的语言导航和使用 Innovator</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Aras Innovator 客户端根据客户端区域设置自动格式化日期、时间和小数</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果标识了公司时区，则在 Innovator 客户端中显示公司时间，所有日期和时间以公司时间显示</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">新 ItemType</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">区域设置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Aras Innovator 区域设置 ItemType 表示 Innovator 实例支持的区域设置，映射到 Windows 区域设置，如英语（美国）或德语（德国），并具有标识要用于该区域设置的 Aras Innovator 语言以及如何检索它的属性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">语言</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/eca4ac8a6efc.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Aras Innovator 语言 ItemType 表示 Innovator 实例支持的语言。安装后，Aras Innovator 仅包含默认语言英语，管理员可以添加其他语言。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">新数据类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">多语言字符串</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9bb2642a0dc3.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">多语言字符串类型的属性可以为 Aras Innovator 中安装的每种语言设置一个值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">多语言字符串的主要目的是使用户能够以多种语言导航和使用 Innovator。它们在 Aras Innovator 中用于菜单、表单和网格上的标签以及列表值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">它们不能标记为必填或具有默认值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">因此，多语言字符串主要用于元数据（关于数据的数据）。可以将多语言字符串用于实例数据，但这样做需要在多种语言中提供值的业务流程。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">编辑多语言字符串</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在使用多语言字符串的地方，如果数据库中安装了其他语言，具有更新访问权限的用户可以使用多语言输入对话框编辑任何语言的值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要从表单打开多语言输入对话框，请点击字段右侧的省略号。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要从网格打开多语言输入对话框，请点击单元格进入编辑模式，然后按 F2。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">新服务器变量</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ac543e2c7d8b.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">DefaultLanguage，在分发数据库中其值为 &apos;English&apos;</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6d353f51f280.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">DefaultLocale，在分发数据库中其值为 &apos;en-US&apos;</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">CorporateTimeZone，必须在安装后创建，并包含 Windows 使用的时区键名之一的值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">客户端设置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Innovator 客户端使用两个标准 Windows 设置。这些设置会在安装 Windows 时自动设置，可由企业 IT 团队控制或由个人 Windows 用户设置。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">区域设置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开控制面板并选择时钟、语言和区域：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/90eace1f1150.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">此设置定义小数、日期和时间的显示方式，Innovator 支持此 Windows 功能。可以选择一种文化，甚至可以根据需要自定义。Innovator 不使用任何货币格式。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">时区</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击日期和时间以更改日期和时间、时区或添加时钟：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图6</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">语言</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ecc1a6f8f355.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击「添加语言」以向列表添加不同的语言。出现在列表顶部的语言是您的主要语言。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有关管理国际化的更多信息，请参阅发行版 CD 上的文档。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击日期和时间以更改日期和时间、时区或添加时钟：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/18150a019d78.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">图6</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">语言</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「添加语言」以向列表添加不同的语言。出现在列表顶部的语言是您的主要语言。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/1c6ab55c361c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">有关管理国际化的更多信息，请参阅发行版 CD 上的文档。</Run></Paragraph>
</FlowDocument>',
   N'关于 Innovator 的国际化和本地化 从 9.0 版本开始，Aras Innovator 支持以下功能的国际化和本地化： 英语是默认语言 可以使用语言包创建或添加其他语言 多语言字符串数据类型用于以多种语言显示值 语言包显示适合客户端区域设置的菜单、标签和值，使用户能够使用已安装的语言导航和使用 Innovator Aras Innovator 客户端根据客户端区域设置自动格式化日期、时间和',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [148/244] how ii18n works in innovator.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('0631a81f45f0',
   N'国际化和本地化行为（续）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">国际化和本地化行为（续）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: how ii18n works in innovator.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">国际化和本地化行为</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当用户登录时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Aras Innovator 客户端检查客户端设置和服务器变量，并建立会话上下文，该上下文将持续到会话结束。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Aras Innovator 确定客户端区域设置是否是数据库中的区域设置之一：如果是，采用 Innovator 区域设置及其 Innovator 语言。如果不是，采用默认区域设置和语言。Aras Innovator 确定是否设置了公司时区：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果已设置，它会计算客户端时区之间的偏移量并采用客户端时区。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果未设置，到公司时区的偏移量设置为零并采用客户端时区。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果公司时间偏移量为零，Aras Innovator 客户端中仅显示本地时间。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果公司时间偏移量不为零，Aras Innovator 客户端中同时显示公司时间和本地时间。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当客户端请求信息时</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">对于每个多语言字符串类型的值（最常见的是菜单、标签或列表值），Innovator 检查是否有上下文语言的可用值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果有，服务器返回上下文语言的值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果没有，服务器返回默认语言的值，请注意如果未定义此值，它可能为空。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">对于每个日期或时间类型的值，服务器会根据会话公司时区调整值，该值可能为 0。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">对于每个日期、时间或小数类型的值，标准 Aras Innovator 客户端使用客户端区域设置文化进行格式化（请注意，替代客户端需要执行此步骤以支持此功能）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有关管理国际化的更多信息，请参阅发行版 CD 上的文档。</Run></Paragraph>
</FlowDocument>',
   N'国际化和本地化行为 当用户登录时 Aras Innovator 客户端检查客户端设置和服务器变量，并建立会话上下文，该上下文将持续到会话结束。 Aras Innovator 确定客户端区域设置是否是数据库中的区域设置之一：如果是，采用 Innovator 区域设置及其 Innovator 语言。如果不是，采用默认区域设置和语言。Aras Innovator 确定是否设置了公司时区： 如果已设置，它',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [149/244] Item_Behavior.htm (9 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('27ded21af664',
   N'项目行为',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">项目行为</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Item_Behavior.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目行为</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目行为是生命周期状态的一个属性，用于设置源项目（或父项目）与关联项目（或子项目）之间的连接行为。此属性仅在源项目和关联项目可版本化时适用。此外，请记住还有一个关系类型行为。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">硬固定和硬浮动的设置对于关系类型行为和生命周期状态行为具有不同的含义。下表说明了这些设置：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">硬浮动</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">硬固定</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">不能由生命周期状态设置修改的浮动行为</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">不能由生命周期状态设置修改的固定行为</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">生命周期状态</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在生命周期结束前有效的浮动行为，不能由后续生命周期状态更改</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在生命周期结束前有效的固定行为，不能由后续生命周期状态更改</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">此外，还需要考虑两个类别 - 如果子项目（或关联项目）先被版本化会发生什么，与父项目（或源项目）先被版本化的情况相对。让我们先看第一种情况。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当父项目（源项目）先被版本化时的行为</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">让我们首先考虑在生命周期状态上没有设置行为时会发生什么：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">请注意，在浮动关系类型行为下，当 Child2 被创建时，Parent1 仍然指向 Child1。原因是当父项目的新版本被创建时，之前的版本会被硬固定在其当前配置中。因此，当 Parent2 被创建时，Parent1 被指向 Child1。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a3920e994774.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">当生命周期行为设置为浮动时，它强制将定义为固定的关系类型也表现为浮动。参见下图。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当生命周期行为设置为固定时，它强制将定义为浮动的关系类型也表现为固定。参见下图。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当关系类型被定义为硬固定或硬浮动时，它会覆盖生命周期状态设置的任何行为。请注意下面两张图，显示了由关系类型定义的行为决定行为，而不考虑生命周期状态定义的行为。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/59d937042bda.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">如果关系类型行为未设置为硬，则可以被生命周期状态行为覆盖。下图显示了当父项目的一个世代号在一个状态中具有一种行为类型，而下一个状态在另一个生命周期状态中具有不同行为时会发生什么。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">到目前为止，我们一直以父项目或源项目先被版本化的假设来看待所有这些情况。现在，我们需要检查所有相同的情况，但子项目或关联项目先被版本化。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/047cda10c8ec.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">当子项目（关联项目）先被版本化时的行为</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如前所述，让我们首先考虑在没有设置生命周期状态行为时会发生什么。请注意，即使对于固定行为，Parent 2 也与 Child2 关联。这是由于以下规则：当源项目被版本化时，它会自动与关联项目的最新可用世代号关联。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/53eef01cc7e8.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">生命周期状态行为覆盖关系类型行为。如您在下面的图表中所见，在浮动生命周期状态下，固定关系表现为浮动；在固定生命周期状态下，浮动关系表现为固定。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当关系类型行为设置为硬固定或硬浮动时，它会覆盖生命周期状态行为。在下面的图表中，您可以看到硬固定行为表现为固定，而不考虑生命周期状态设置；硬浮动表现为浮动，而不考虑生命周期状态设置。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0405115db1c9.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">如果关系类型行为未设置为硬，则可以被生命周期状态行为覆盖。下图显示了当父项目的一个世代号在一个状态中具有一种行为类型，而下一个状态在另一个生命周期状态中具有不同行为时会发生什么。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">请记住，如果您将生命周期状态行为设置为硬固定或硬浮动，它将覆盖所有后续状态行为直到周期结束。如果您有一个生命周期映射，其中在发布之后有其他状态（如已淘汰或已取代），这将特别有用。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">As previously, let&apos;s consider first what happens when there is no life cycle state behavior set. Notice that even for Fixed behavior, Parent 2 is associated with Child2. This is true because of the following rule: When a source item is versioned, it is automatically associated with the latest available version of all its children. In this case, since Child 2 was previously created, instead of pointing to Child1, Parent 2 points to Child2.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c337927d6353.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The Life Cycle State behavior overrides the RelationshipType behavior. As you can see in the diagrams below, Fixed relationship acts like Float when in Float life cycle state, and the Float relationship acts as Fixed in the Fixed life cycle state.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6e596c819f0a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">When the RelationshipType Behavior is set to Hard Fixed or Hard Float, it overrides the Life Cycle State behavior. In the diagrams below you can see that the Hard Fixed behavior acts as Fixed, regardless of the life cycle state setting, and the Hard Float acts as Float regardless of the life cycle state setting.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3005f209220f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">If the RelationshipType Behavior is not set to Hard, it can be overwritten by the Life Cycle State Behavior. The figure below shows what happens when one generation of the parent is in one state, with one type of behavior, and the next state is in another life cycle state, with a different behavior.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f49d525c61f2.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Remember that if you set the Life Cycle state behavior to Hard Fixed or Hard Float, it will override all subsequent state behavior through the end of the cycle. This is particularly useful if you have a life cycle map with states that follow Released, such as Obsolete, or Superseded. Most likely once an item reaches a Released state you would want to Hard Fix its behavior, so that its configuration remained the same, even if the item is promoted to Obsolete or Superseded.</Run></Paragraph>
</FlowDocument>',
   N'项目行为 项目行为是生命周期状态的一个属性，用于设置源项目（或父项目）与关联项目（或子项目）之间的连接行为。此属性仅在源项目和关联项目可版本化时适用。此外，请记住还有一个关系类型行为。 硬固定和硬浮动的设置对于关系类型行为和生命周期状态行为具有不同的含义。下表说明了这些设置： 硬浮动 硬固定 关系类型 不能由生命周期状态设置修改的浮动行为 不能由生命周期状态设置修改的固定行为 生命周期状态 在生命',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [150/244] Item_Menu.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('a78077cdc90a',
   N'项目菜单',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">项目菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Item_Menu.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目菜单与主菜单类似，您可以执行各种通用任务，如创建新项目、保存当前项目、锁定/解锁当前项目等。点击链接查看每个菜单选项的更多信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文件菜单（项目窗口）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">编辑菜单（项目窗口）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">视图菜单（项目窗口）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索菜单（项目窗口）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作菜单（项目窗口）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">报告菜单（项目窗口）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">帮助菜单（项目窗口）</Run></Paragraph>
</FlowDocument>',
   N'项目菜单 项目菜单与主菜单类似，您可以执行各种通用任务，如创建新项目、保存当前项目、锁定/解锁当前项目等。点击链接查看每个菜单选项的更多信息。 文件菜单（项目窗口） 编辑菜单（项目窗口） 视图菜单（项目窗口） 搜索菜单（项目窗口） 操作菜单（项目窗口） 报告菜单（项目窗口） 帮助菜单（项目窗口）',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [151/244] Item_Toolbar.htm (15 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('33e3918330a4',
   N'项目工具栏',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">项目工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Item_Toolbar.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目窗口提供了一个带有按钮的工具栏，用于执行各种任务。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">下表描述了项目工具栏中的按钮。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/132c1f9365a1.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">按钮名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">按钮</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建新项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建新项目。按钮名称根据您要创建的项目类型而变化。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/33da8f7d9528.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">将打开的项目保存到服务器。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">更多</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「更多」按钮查看上下文菜单。从那里您可以选择「清除此版本」以删除项目的当前版本或「删除所有版本」。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ba507963f59.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">刷新</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">刷新并重新加载表单上的项目信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">导出到 Excel</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/eeb4e2a96040.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击「共享」按钮并从上下文菜单中选择「导出到 Excel」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">导出到 Word</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「共享」将打开项目的所有项目属性和关系属性导出到 Microsoft Word 文档。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0ae8439d0ca6.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">复制 ID</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从上下文菜单中选择「复制 ID」将 ID 复制到剪贴板。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">复制链接</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c9f33042dea3.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">从上下文菜单中选择「复制链接」将指定链接复制到剪贴板。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">复制链接到</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从上下文菜单中选择「复制链接到」将指定链接复制到最新版本或最新发布版。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c9f33042dea3.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">认领</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">认领项目（编辑模式）并阻止其他用户提交更改。有关更多信息，请参阅认领或取消认领项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">解锁</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">取消认领打开的项目并允许任何授权用户锁定项目进行更改。有关更多信息，请参阅认领或取消认领项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">撤销更改</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将项目恢复为上次保存时的值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">提升</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将项目提升到已分配生命周期映射中定义的下一个可用状态。有关提升项目的更多信息，请参阅在生命周期中提升项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">导航</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/dbc3664f0732.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击「导航」并从上下文菜单中选择「版本」。将显示所选项目的版本信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">隐藏选项卡视图中的选项卡</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">切换关系选项卡的显示。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/28ae4d7344ef.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">帮助</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示「询问 Innovator」在线帮助。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">完成</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/409bf1be3c37.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">保存对项目所做的更改，解锁项目，然后关闭项目窗口。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">提升</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/78b46b2ebb72.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Promotes the Item to the next available state, defined in the assigned Lifecycle map. For more information on promoting an Item, refer to Promoting an Item in Lifecycle.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">导航</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/59eb9549a5fc.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click Navigate and select Versions from the context menu. The version information for the selected item appears.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在选项卡视图中隐藏选项卡</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/57c1bdbee10a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">切换关系选项卡的显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">帮助</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/468a0252db0f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">显示「询问 Innovator」在线帮助。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">完成</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Saves the changes made to the Item, unlocks the Item, and then closes the Item window.</Run></Paragraph>
</FlowDocument>',
   N'项目工具栏 项目窗口提供了一个带有按钮的工具栏，用于执行各种任务。 下表描述了项目工具栏中的按钮。 按钮名称 按钮 说明 创建新项目 创建新项目。按钮名称根据您要创建的项目类型而变化。 保存 将打开的项目保存到服务器。 更多 点击「更多」按钮查看上下文菜单。从那里您可以选择「清除此版本」以删除项目的当前版本或「删除所有版本」。 刷新 刷新并重新加载表单上的项目信息。 导出到 Excel 点击「共享',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [152/244] Item_Window.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('9e4482760c91',
   N'项目窗口',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">项目窗口</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Item_Window.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目窗口</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目窗口（也称为独立窗口）包含有关项目的信息。当您编辑项目时，项目视图将出现在独立窗口中，基于为该 ItemType 创建的表单。独立窗口用于数据输入和查看主网格中未显示的详细信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目窗口主要由以下部分组成：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在表单中，您可以在下拉菜单或对话框中搜索值。只需在其中一个字段中输入字母即可查看以相同字母开头的选项列表。有关更多信息，请参阅使用自动输入。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">表单和关系选项卡因 ItemType 而异，因为它们是高度可定制的。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果项目的对应 ItemType 启用了社交内容，则项目窗口的用户界面将有一些额外的特殊属性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有关更多信息，请参阅项目窗口的安全社交可视化协作（SSVC）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果您同时打开了多个 ItemType 窗口，可以使用拖放重新排列选项卡。</Run></Paragraph>
</FlowDocument>',
   N'项目窗口 项目窗口（也称为独立窗口）包含有关项目的信息。当您编辑项目时，项目视图将出现在独立窗口中，基于为该 ItemType 创建的表单。独立窗口用于数据输入和查看主网格中未显示的详细信息。 项目窗口主要由以下部分组成： 表单 项目菜单 项目工具栏 关系 在表单中，您可以在下拉菜单或对话框中搜索值。只需在其中一个字段中输入字母即可查看以相同字母开头的选项列表。有关更多信息，请参阅使用自动输入。 ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [153/244] Item_Window_Buttons.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('d1fd3745c485',
   N'项目窗口按钮',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">项目窗口按钮</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Item_Window_Buttons.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目窗口按钮</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目窗口中出现的按钮提供了查看与项目关联的访问权限、项目历史、修订历史等选项。下表描述了按钮及其关联选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">某些选项可能显示为禁用状态。菜单选项在选择适当项目且登录用户具有足够访问权限时启用。访问权限由管理员配置。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击...</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">菜单选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
</FlowDocument>',
   N'项目窗口按钮 项目窗口中出现的按钮提供了查看与项目关联的访问权限、项目历史、修订历史等选项。下表描述了按钮及其关联选项。 某些选项可能显示为禁用状态。菜单选项在选择适当项目且登录用户具有足够访问权限时启用。访问权限由管理员配置。 点击... 菜单选项 说明',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [154/244] Locking_or_unlocking.htm (5 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('f52b8749609b',
   N'认领或取消认领项目',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">认领或取消认领项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Locking_or_unlocking.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">认领或取消认领项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果您具有所需的访问权限，可以认领或取消认领项目。当您认领项目时，它会阻止其他用户对其进行更改。取消认领项目后，其他用户将能够认领它并进行更改。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">认领项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从主网格中选择项目，然后选择以下选项之一：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">右键点击主网格中的项目并选择认领按钮。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开项目并从项目工具栏中选择认领按钮。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择认领选项意味着项目将在用户点击保存或完成按钮后自动取消认领，或在注销时取消认领。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/eaf68ab55cfa.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/8277a949a660.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">认领状态</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">下表列出了不同的认领状态及其说明：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图标</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">由您认领。其他用户被禁止为您认领的项目提交更改。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">由您认领，且项目已进行尚未保存或提交的更改。您需要保存项目以提交更改。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">由其他用户认领。您无法认领或编辑此项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">取消认领项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有多种选项可以取消认领项目。从主网格中选择由您认领的项目，然后执行以下操作之一：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您只能取消认领由您认领的项目。您无法取消认领带有红色标记（由其他用户认领）的项目。只有管理员可以覆盖其他用户的认领。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">右键点击主网格中的项目并点击取消认领按钮。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开项目并从项目工具栏中点击取消认领按钮。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果项目未被认领但处于编辑模式，请在项目工具栏中点击保存或完成按钮。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/8277a949a660.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'认领或取消认领项目 如果您具有所需的访问权限，可以认领或取消认领项目。当您认领项目时，它会阻止其他用户对其进行更改。取消认领项目后，其他用户将能够认领它并进行更改。 认领项目 从主网格中选择项目，然后选择以下选项之一： 右键点击主网格中的项目并选择认领按钮。 打开项目并从项目工具栏中选择认领按钮。 选择认领选项意味着项目将在用户点击保存或完成按钮后自动取消认领，或在注销时取消认领。 认领状态 下表',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [155/244] Logon_Users.htm (11 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('f0233cdd5fd0',
   N'登录用户',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">登录用户</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Logon_Users.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">登录用户</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要访问 Innovator，每个用户通常需要通过 Innovator 认证屏幕登录，该屏幕请求用户名和密码。用户名或登录名和密码定义了登录用户。在 Innovator 中，当创建登录用户时，会自动为用户分配关联的用户身份。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以下是此概念在 Innovator 中的工作原理。首先，管理员必须创建登录用户：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ca629830206.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">1. 在导航窗格中，选择 管理 &gt; 用户。将出现以下菜单：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2. 点击「搜索用户」以查看系统中已有哪些用户。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">3. 点击「创建新用户」。将出现以下对话框：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/1cbd1536c19d.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">4. 按照以下说明在对话框上填写以下属性：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">登录名 - 用户将在认证屏幕上输入以访问 Innovator 的登录用户名。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">密码 - 用户将在认证屏幕上输入以访问 Innovator 的密码。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/130f9c3cddec.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">确认密码 - 重新输入密码以确认。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">警告：登录名和密码区分大小写。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">登录已启用 - 如果用户具有登录权限，请选中此框。将此框留空将不允许用户通过认证屏幕访问 Innovator。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以下对话框属性是可选的，可以根据需要包含值：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">名字 - 用户的名字</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">姓氏 - 用户的姓氏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">公司名称 - 用户的公司名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">电子邮件 - 用户的电子邮件地址。如果用户要接收分配给他们的特定活动和操作项的通知并转发到他们的收件箱，此信息是必需的。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">电话 - 用户的电话号码</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">邮件地址 - 用户的邮件地址</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">传真 - 用户的传真号码</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">家庭电话 - 用户的家庭电话号码</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">手机 - 用户的手机号码</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">寻呼机 - 用户的寻呼机号码</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">员工编号 - 用户的员工编号。此信息通常被公司用于对员工活动进行指标分析。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">管理者 - 用户管理者的身份。此信息通常用于委派目的或甚至用于权限结构。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工作目录 - 用户访问的目录。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">起始页 -</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">默认保险柜 - 提供安全文件存储的保险柜。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">5. 从导航窗格中，选择 管理 &gt; 身份 并点击搜索以显示所有身份。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">组身份可以被分配特定的权限。现在此身份可以与其他身份分组以创建组身份。因此，对于每个登录用户，Innovator 会自动创建一个身份。您应该看到为上述用户自动创建了一个身份，此身份具有相同的名称。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">别名选项卡</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">查看与特定登录用户关联的身份的另一种方法是查看用户表单中别名选项卡下的列表。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5da5ed34617a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">1. 从导航窗格中，选择 管理 &gt; 用户。让我们打开之前创建的用户。注意对话框主工具栏中的隐藏选项卡箭头。如果您点击向下箭头，您将看到此用户的选项卡属性：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有两个选项卡，别名和读取保险柜：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/2342adc113d5.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">别名列出了与此登录用户别名（或关联）的所有 Innovator 身份。您也可以从此列表中手动添加或删除条目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">读取保险柜选项卡使您能够添加用于存储用户信息的保险柜。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2. 点击「选择项目」按钮以将保险柜与用户关联。将出现保险柜搜索对话框：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">3. 选择 Default 并点击「返回所选」按钮以添加它。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a65a99192645.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">有两个选项卡，别名和读取保险库：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e8406e231036.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The Alias tab lists all the Innovator Identities that are aliased to (or associated with) this Logon User. You can also manually add or delete entries from this list.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The Read Vaults tab enables you to add a vault to use to store user information.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/87453919fdea.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">2. Click the Select Items button to associate a vault with the user. The Vault Search dialog appears:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/81d30419a164.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/38019b732547.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">3. Select Default and click the Return Selected button to add it.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6a56781729d6.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'登录用户 要访问 Innovator，每个用户通常需要通过 Innovator 认证屏幕登录，该屏幕请求用户名和密码。用户名或登录名和密码定义了登录用户。在 Innovator 中，当创建登录用户时，会自动为用户分配关联的用户身份。 以下是此概念在 Innovator 中的工作原理。首先，管理员必须创建登录用户： 1. 在导航窗格中，选择 管理 > 用户。将出现以下菜单： 2. 点击「搜索用户」以',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [156/244] Main_menu.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('28ca335cfafa',
   N'主菜单',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">主菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Main_menu.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">主菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">主菜单提供以下选项。点击查看每个菜单的详细信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">编辑</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">视图</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">报告</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工具</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">帮助</Run></Paragraph>
</FlowDocument>',
   N'主菜单 主菜单提供以下选项。点击查看每个菜单的详细信息。 文件 编辑 视图 搜索 操作 报告 工具 帮助',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [157/244] Main_Toolbar.htm (14 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('6de0898fda68',
   N'主工具栏',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">主工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Main_Toolbar.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">主工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Aras Innovator 的用户界面在主窗口、独立窗口和其他对话框中提供工具栏图标。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">下表描述了主工具栏中的图标。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/62b2c839a4d2.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">按钮名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">按钮</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">编辑</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存对项目的更改。项目保持在编辑模式。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">完成</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ba507963f59.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">保存项目并解锁。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">放弃</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">放弃对项目的更改。项目返回到查看模式并被取消认领。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">刷新</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">刷新屏幕。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">提升</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/8277a949a660.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">允许您将项目提升到下一个级别。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">导航</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示以下菜单：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/96fc549148c5.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">报告</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示以下菜单：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">共享</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/33868a5cc35e.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">显示以下菜单：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">更多</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示以下菜单：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/59eb9549a5fc.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">显示以下菜单：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0b99dbcd96ab.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">报告</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6ee6a5835ccb.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">显示以下菜单：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/10b5550f5e0d.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">分享</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c9f33042dea3.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">显示以下菜单：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/30f85ebe81ba.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">更多</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/eeb4e2a96040.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">显示以下菜单：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/2a0abd1abaa4.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'主工具栏 Aras Innovator 的用户界面在主窗口、独立窗口和其他对话框中提供工具栏图标。 下表描述了主工具栏中的图标。 按钮名称 按钮 说明 编辑 保存 保存对项目的更改。项目保持在编辑模式。 完成 保存项目并解锁。 放弃 放弃对项目的更改。项目返回到查看模式并被取消认领。 刷新 刷新屏幕。 提升 允许您将项目提升到下一个级别。 导航 显示以下菜单： 报告 显示以下菜单： 共享 显示以下',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [158/244] Managing_Users.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('76fc12b344df',
   N'管理用户',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">管理用户</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Managing_Users.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">管理用户</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Aras Innovator 中的访问通过定义登录用户和身份来管理。登录用户是在 Aras Innovator 应用程序中具有登录权限的独特个体。每个登录用户属于由别名分配定义的唯一用户身份。用户身份随后被分配访问权限和权限。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">让我们查看下图了解示例结构。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">作为最佳实践，我们建议使用类似于构建公司组织图的过程来设置用户身份。以下是使此过程更容易的步骤列表。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6860d7cec9d4.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">为顶级部门标题创建身份，例如：工程、销售和市场。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">对于每个顶级标题，创建下属部门。例如，工程部可能有：开发、支持和质量保证。支持部可能进一步由：客户支持、文档和培训组成。确保将下属部门作为成员添加到其各自的组身份中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">一旦部门结构到位，创建个别用户身份并将其添加到各自的部门中。在 Innovator 中，一个用户身份可以属于多个组。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要了解创建用户并将其分配到组的过程，请点击以下链接：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">登录用户</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">分组身份</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">分组身份</Run></Paragraph>
</FlowDocument>',
   N'管理用户 Aras Innovator 中的访问通过定义登录用户和身份来管理。登录用户是在 Aras Innovator 应用程序中具有登录权限的独特个体。每个登录用户属于由别名分配定义的唯一用户身份。用户身份随后被分配访问权限和权限。 让我们查看下图了解示例结构。 作为最佳实践，我们建议使用类似于构建公司组织图的过程来设置用户身份。以下是使此过程更容易的步骤列表。 为顶级部门标题创建身份，例如：',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [159/244] Manual_Versions.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('8c3435bdf195',
   N'手动版本',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">手动版本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Manual_Versions.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">手动版本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">手动版本是管理员可以对项目执行的可选功能。当管理员定义 ItemType 并将其标记为可版本化时，创建新版本的操作可以由系统自动执行（通过自定义业务逻辑）或手动由用户执行。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">手动版本只能对当前未锁定的项目执行。</Run></Paragraph>
</FlowDocument>',
   N'手动版本 手动版本是管理员可以对项目执行的可选功能。当管理员定义 ItemType 并将其标记为可版本化时，创建新版本的操作可以由系统自动执行（通过自定义业务逻辑）或手动由用户执行。 手动版本只能对当前未锁定的项目执行。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [160/244] Markup_Messages.htm (11 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('0e761931f8dc',
   N'批注工具栏',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">批注工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Markup_Messages.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">批注工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户在批注模式下访问批注工具栏。用户可以在图片上绘制批注标记。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当批注模式处于活动状态时，以下工具栏和功能可用：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">基本工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标准工具栏</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b2b8b9141104.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">图标</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工具提示</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/65ccf4e9aa3f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">涂鸦</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开涂鸦调色板。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">小</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d8e3902e0dd5.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">中</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">大</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择活动大小</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5dc708739817.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">黄色</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">红色/粉色</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">绿色</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">蓝色</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">橙色</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">灰色</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">白色</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">黑色</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择活动颜色</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高亮</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开高亮调色板。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标签</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开标签调色板。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/19065e7e8534.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">选择任何标记以移动或删除它。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除任何选定的标记。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/26ca7cdc6abc.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">下载</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">下载文件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打印</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/87dddb57c335.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">打印文件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">切换到标准工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从基本工具栏切换到标准工具栏。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6fa8340f3299.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">删除</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除选定的批注。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/32bc0b1e30c0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">下载</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">下载文件。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4c3f64812716.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">打印</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打印文件。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/df6fed6cccfa.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">切换到标准工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从基本工具栏切换到标准工具栏。</Run></Paragraph>
</FlowDocument>',
   N'批注工具栏 用户在批注模式下访问批注工具栏。用户可以在图片上绘制批注标记。 当批注模式处于活动状态时，以下工具栏和功能可用： 基本工具栏 标准工具栏 图标 工具提示 说明 涂鸦 打开涂鸦调色板。 小 中 大 选择活动大小 黄色 红色/粉色 绿色 蓝色 橙色 灰色 白色 黑色 选择活动颜色 高亮 打开高亮调色板。 标签 打开标签调色板。 选择 选择任何标记以移动或删除它。 删除 删除任何选定的标记。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [161/244] MassPromote.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('6f9a371c05f1',
   N'关于批量提升',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于批量提升</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: MassPromote.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于批量提升</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Aras Innovator 中的批量提升功能使用户能够通过从搜索结果网格中选择多个项目并点击工具栏或上下文下拉菜单中的提升图标，一次性提升多个项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">批量提升窗口以新标签页（标签模式）或独立窗口（标准模式）显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：无论您选择何种类型的项目，提升都会启用。验证检查在批量提升窗口出现之前不会执行。使用批量提升时，您只能选择具有相同 ItemType 的多个项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">批量提升窗口</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">批量提升窗口由横幅、设置面板和网格组成。横幅显示所选项目数量及其 ItemType。设置面板显示以下模块：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/82858e77798b.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">生命周期映射仅当网格中出现的项目不共享相同的生命周期映射时显示。您需要为您要提升的项目选择一个生命周期。没有关联生命周期映射的项目将被视为无效。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">目标状态使您能够为正在提升的项目选择目标状态。项目将被提升到您选择的状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">评论仅当评论功能为与指定目标状态关联的一个或多个项目的特定生命周期转换启用时才显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">无效项目仅当在所选项目列表中检测到一个或多个无效项目时才显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">执行在项目之间的冲突解决后出现。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">状态显示当前提升状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">状态显示当前提升状态。</Run></Paragraph>
</FlowDocument>',
   N'关于批量提升 Aras Innovator 中的批量提升功能使用户能够通过从搜索结果网格中选择多个项目并点击工具栏或上下文下拉菜单中的提升图标，一次性提升多个项目。 批量提升窗口以新标签页（标签模式）或独立窗口（标准模式）显示。 注意：无论您选择何种类型的项目，提升都会启用。验证检查在批量提升窗口出现之前不会执行。使用批量提升时，您只能选择具有相同 ItemType 的多个项目。 批量提升窗口 批',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [162/244] Mass_Promote_-_Settings_Modules.htm (9 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('3fb68fbdeb52',
   N'批量提升 - 设置模块',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">批量提升 - 设置模块</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Mass_Promote_-_Settings_Modules.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">批量提升 - 设置模块</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用生命周期映射模块</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">生命周期映射模块使您能够：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择用于要提升的项目的生命周期映射。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">为您要提升的所有项目选择目标状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">输入将应用于所有提升项目的评论。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用无效项目模块从网格中移除无效项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用执行模块提升项目组。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">此模块仅当您选择的项目不共享相同的生命周期映射时才出现在批量提升窗口中。您只能提升共享相同生命周期映射的项目。使用您未选择的映射的项目将被标记为无效。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择生命周期映射</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用以下步骤：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「选择生命周期」下拉箭头查看生命周期映射列表。如果项目只关联一个映射，映射名称将显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择适当的生命周期映射。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/effde5fbf1a6.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">使用目标状态模块</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择目标状态</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">目标状态模块始终显示在窗口中，但在您选择要与先前选择的项目一起使用的生命周期映射之前处于禁用状态。一旦目标状态模块处于活动状态，使用以下步骤选择目标状态：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「目标状态」下拉箭头查看与您选择的生命周期映射关联的目标状态列表。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择适当的目标状态并点击「应用」。某些项目可能会变为无效，因为它们没有到您所选目标状态的路径，或者您没有适当的权限来提升它们。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用评论模块</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a0358439ff53.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">输入评论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">评论模块仅当评论功能为与正在提升的一个或多个项目关联的生命周期转换激活时才显示。在评论字段中输入您的评论，如下所示：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：您输入的评论适用于您选择提升的所有项目。您无法为单个项目输入单独的评论。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用无效项目模块</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">解决无效项目</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/37a6aea23ccb.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">当应用程序在您选择的列表中检测到一个或多个无效项目时，无效项目模块会出现。使用以下选项之一从列表中移除无效项目：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择「移除」单选按钮并点击「应用」以从网格中移除所有无效项目。状态模块将更新。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择「移动到另一个提升会话」单选按钮并点击「应用」。所有无效项目将从网格中移除，状态模块将更新。将打开一个新的批量提升窗口，显示无效项目列表。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：您可以通过在网格中手动选择单个项目，然后选择「从选择中移除」或「移动到新的批量提升」单选按钮来手动执行这些操作。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用执行模块</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">提升项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">执行模块使您能够提升所选项目。它始终显示在批量提升窗口中。根据当前状态，会激活不同的按钮，如下表所示。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5ebb9ccfae25.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">按钮</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">状态</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">操作</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">冲突未解决</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关闭批量提升窗口，不执行任何提升。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">执行进行中</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">暂停当前执行过程。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">执行进行中</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/21dfe10ff93e.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">启动所有选定项目的提升过程。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">执行完成</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存批量提升项目并关闭窗口。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">应用更改。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您可以选择移除无效项目或将它们移动到另一个提升级别，具体取决于您在「所有无效项目」模块中选择的单选按钮。当您做出选择时，「应用」按钮将被激活。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7f950ebe4b66.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">注意：当您点击「取消」按钮关闭批量提升窗口而不执行提升操作时，将出现包含警告提示的对话框。点击「是」关闭批量提升窗口。不会执行任何提升，所选项目保持不变。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用状态模块</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">状态模块显示批量提升过程的当前状态，如下截图所示。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7c2352601e72.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">下表描述了可以在字段中出现的状态消息：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">状态</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/33e71e80bf95.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">示例</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">解决冲突 &lt;n&gt; 个项目已发现</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目冲突未解决</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">解决冲突</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">114 个无效项目已发现</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">准备执行 所有项目有效</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d9db9a0b6dc1.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">项目冲突已解决；准备执行批量提升</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">准备执行 所有项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">已完成 失败</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">待处理</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">执行进行中</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1124 已完成 2 失败 30 已取消</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">已完成 失败 已取消</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">执行已取消</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1129 已完成 2 失败 30 已取消</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">已完成 失败</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">执行完成</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1159 已完成</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2 失败</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">执行进行中</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1124 Completed 2 Failed 30 Cancelled</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">已完成 失败 已取消</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">执行已取消</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1129 Completed 2 Failed 30 Cancelled</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">已完成 失败</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">执行完成</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1159 已完成</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2 失败</Run></Paragraph>
</FlowDocument>',
   N'批量提升 - 设置模块 使用生命周期映射模块 生命周期映射模块使您能够： 选择用于要提升的项目的生命周期映射。 为您要提升的所有项目选择目标状态。 输入将应用于所有提升项目的评论。 使用无效项目模块从网格中移除无效项目。 使用执行模块提升项目组。 此模块仅当您选择的项目不共享相同的生命周期映射时才出现在批量提升窗口中。您只能提升共享相同生命周期映射的项目。使用您未选择的映射的项目将被标记为无效。 ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [163/244] Mass_Promote_Grid.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('0e1d8e6f8a42',
   N'批量提升网格',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">批量提升网格</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Mass_Promote_Grid.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">批量提升网格</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">批量提升选项卡显示您选择要提升的项目。它由以下组件组成：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">横幅显示批量操作名称（批量提升）、所选项目数量及其 ItemType。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设置面板动态显示有关提升操作当前状态的信息。有关更多信息，请参阅批量提升设置模块。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工具栏显示主搜索网格和关系搜索网格中的标准图标。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">网格类似于 Innovator 中使用的搜索和关系网格，但它是专门为批量提升操作构建的。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">网格包含固定的 5 列：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">下表描述了列：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/82858e77798b.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">列标签必须是「项目」而不是「键名」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您只能在网格中进行简单搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">一旦您从主网格中选择了项目并填充了批量提升网格，您可以在开始提升过程之前添加和移除项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Once you have selected items from the main grid and populated the Mass Promote grid, you can add and remove items before you begin the promotion process.</Run></Paragraph>
</FlowDocument>',
   N'批量提升网格 批量提升选项卡显示您选择要提升的项目。它由以下组件组成： 横幅显示批量操作名称（批量提升）、所选项目数量及其 ItemType。 设置面板动态显示有关提升操作当前状态的信息。有关更多信息，请参阅批量提升设置模块。 工具栏显示主搜索网格和关系搜索网格中的标准图标。 网格类似于 Innovator 中使用的搜索和关系网格，但它是专门为批量提升操作构建的。 网格包含固定的 5 列： 下表描',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [164/244] Mass_Promote_TOC.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('2bc2da13e572',
   N'批量提升',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">批量提升</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Mass_Promote_TOC.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">批量提升</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">批量提升使您能够选择多个项目并一次性提升它们。有关更多信息，请参阅以下主题：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于批量提升</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">批量提升 - 设置模块</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">批量提升网格</Run></Paragraph>
</FlowDocument>',
   N'批量提升 批量提升使您能够选择多个项目并一次性提升它们。有关更多信息，请参阅以下主题： 关于批量提升 批量提升 - 设置模块 批量提升网格',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [165/244] Message_Components.htm (4 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('6d7e7c0fe4e7',
   N'消息的组成部分',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">消息的组成部分</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Message_Components.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息的组成部分</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">一旦消息显示在讨论面板区域中，其中就有具有特定属性的各种组件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息的组成部分如下所示：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">下表提供了消息的组成部分：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/652e90609362.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">消息组件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">来源图标</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">指示消息来源是来自用户评论还是自动的。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户名</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建评论的用户名称，无论是直接创建还是通过历史记录间接创建。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建时间</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息创建的时间。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">公告板项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">公告板项目显示消息相关的项目名称。公告板项目还作为超链接打开引用项目的独立窗口。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目信息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目信息显示消息创建时项目的基本项目信息。显示的字符串是安全消息的三个属性的组合，如下所示：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目的当前修订号</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目的当前世代号</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目的当前生命周期状态</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目图标</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">与公告板项目对应的图标和颜色。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息文本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息的文本。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">缩略图</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图像的缩略图（如果包含在评论中）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息功能</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可以在消息上执行的功能。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可以在消息上执行的功能，包括：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标记/取消标记 - 用户可以标记/取消标记消息。它还显示已标记/取消标记消息的用户数量。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d19eaf9cd541.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">回复 - 允许用户输入评论以回复此消息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">隐藏回复 - 允许用户隐藏回复消息的显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除 - 允许用户删除先前创建的消息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息显示</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示的消息数量由安全消息 ItemType 的默认页面大小属性控制。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用滚动键上下滚动到显示消息的底部。点击讨论面板页脚处的「显示更多消息」以显示更多消息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当所有消息都已显示后，讨论面板的页脚将显示「没有更多消息」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Use the scroll keys to scroll up and down to the bottom of the displayed messages. Click on &quot;Show more messages&quot; located at the footer of the discussion panel to display more messages.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/1160f97daa83.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">When all the messages have been displayed, the footer of the discussion panel indicates &quot;There are no more messages&quot;.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ebb541264aa6.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'消息的组成部分 一旦消息显示在讨论面板区域中，其中就有具有特定属性的各种组件。 消息的组成部分如下所示： 下表提供了消息的组成部分： 消息组件 属性 来源图标 指示消息来源是来自用户评论还是自动的。 用户名 创建评论的用户名称，无论是直接创建还是通过历史记录间接创建。 创建时间 消息创建的时间。 公告板项目 公告板项目显示消息相关的项目名称。公告板项目还作为超链接打开引用项目的独立窗口。 项目信息',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [166/244] Message_Functions.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('fb357c55d314',
   N'消息功能',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">消息功能</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Message_Functions.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息功能</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">一旦用户在评论框中输入评论，它将作为消息显示在项目独立窗口的讨论面板中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">每条消息的底部是消息功能。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有关消息功能的更多信息，请参阅以下主题：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/42f142ca474f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">标记消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">输入回复</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除消息</Run></Paragraph>
</FlowDocument>',
   N'消息功能 一旦用户在评论框中输入评论，它将作为消息显示在项目独立窗口的讨论面板中。 每条消息的底部是消息功能。 有关消息功能的更多信息，请参阅以下主题： 标记消息 输入回复 删除消息 Erasing a Message',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [167/244] Message_Search_and_Filtering_Options.htm (8 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('b3d4b45905aa',
   N'消息搜索和过滤选项',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">消息搜索和过滤选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Message_Search_and_Filtering_Options.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息搜索和过滤选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">讨论工具栏包含使用各种条件轻松搜索和过滤消息的选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在讨论工具栏中有两种执行消息搜索的方式。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">基本搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">基本搜索直接在工具栏中提供，仅搜索消息文本。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要执行消息的基本搜索，请执行以下操作：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在搜索输入字段中输入搜索文本。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击搜索图标执行搜索。讨论面板中将填充所有包含搜索文本的消息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">向下滚动到底部以查找讨论面板中显示的所有消息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高级搜索</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f90b30b78c41.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">高级搜索在您输入的过滤选项值上进行搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要执行消息的高级搜索，请执行以下操作：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击讨论工具栏中搜索输入框中的图标。这将在搜索框下方打开一个浮动窗口。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在任何这些过滤条件中输入值。选项在下表中描述：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0e6336172f0b.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">过滤条件</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/fcde360f5405.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">过滤选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息搜索基于消息类型：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">所有评论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">仅文本评论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">仅快照评论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">仅音频评论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">仅视频评论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">仅自动消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">来源</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息搜索基于消息来源：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">全部</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">仅此项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">来自</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">基于消息创建者的消息搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">时间范围</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">基于消息创建时间范围的消息搜索。有两个选项：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">「在过去」允许您指定一个数字和时间值（分钟、小时、天或月）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">「日期范围」选项使您能够搜索在特定时间段内创建的消息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当您点击搜索时，将出现类似以下的消息：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">修订</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/228b2bb146de.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">基于项目修订值的消息搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">状态</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ab212edbd62e.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">基于项目状态名称的消息搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击搜索执行搜索以显示过滤后的消息。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a8b74a7915d9.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击取消关闭搜索窗口。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">过滤指示</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当用户应用了搜索过滤器时，讨论面板清楚地指示这些是已应用的过滤器。这使用户能够理解他们不是在查看所有结果。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">对于消息的基本搜索，过滤指示如下：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">对于消息的高级搜索，过滤指示如下：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">讨论工具栏下方会出现一个黄色条，显示当前过滤条件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「显示所有消息」链接将讨论面板恢复到正常的未过滤状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">When the user has applied the search filters, the Discussion Panel clearly indicates that these are the filters that are applied. This allows the user to understand that they are not looking at all results.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">For a basic search on messages, the indication of filtering looks like:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4653119850ba.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">For an advanced search of messages, the indication of filtering looks like:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/02ccbc0d4dc1.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">A yellow bar appears below the Discussion toolbar displaying the current filtering criteria.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click the Show all messages link to return the Discussion Panel to its normal unfiltered state.</Run></Paragraph>
</FlowDocument>',
   N'消息搜索和过滤选项 讨论工具栏包含使用各种条件轻松搜索和过滤消息的选项。 在讨论工具栏中有两种执行消息搜索的方式。 基本搜索 基本搜索直接在工具栏中提供，仅搜索消息文本。 要执行消息的基本搜索，请执行以下操作： 在搜索输入字段中输入搜索文本。 点击搜索图标执行搜索。讨论面板中将填充所有包含搜索文本的消息。 向下滚动到底部以查找讨论面板中显示的所有消息。 高级搜索 高级搜索在您输入的过滤选项值上进行',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [168/244] Message_Sorting_Options.htm (3 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('7e3d3cf0faed',
   N'消息排序选项',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">消息排序选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Message_Sorting_Options.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息排序选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">讨论面板提供了一个选择器供用户选择排序选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有三种消息排序选项：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/bc2de55679f2.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">对话 - 这是默认排序模式。消息按发布时间排序。最新的消息在顶部。「最新」的确定基于消息本身的时间（如果没有回复）或最新回复的时间。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">主题日期 - 消息按日期排序，最新的在顶部。「最新」的确定仅基于消息本身的时间，不基于任何回复。消息以主题样式显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息日期 - 消息按日期排序，最新的在顶部。「最新」的确定仅基于消息本身的时间，不基于任何回复。消息以平面样式显示，所有回复列在同一级别。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有两种消息显示选项：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标准以标准格式显示消息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">审核以更详细的格式显示消息。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/110be63deb2b.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">审核以更详细的格式显示消息。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b6e25a3e979d.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'消息排序选项 讨论面板提供了一个选择器供用户选择排序选项。 有三种消息排序选项： 对话 - 这是默认排序模式。消息按发布时间排序。最新的消息在顶部。「最新」的确定基于消息本身的时间（如果没有回复）或最新回复的时间。 主题日期 - 消息按日期排序，最新的在顶部。「最新」的确定仅基于消息本身的时间，不基于任何回复。消息以主题样式显示。 消息日期 - 消息按日期排序，最新的在顶部。「最新」的确定仅基于消',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [169/244] Message_Types.htm (5 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('f12ff53de201',
   N'消息类型',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">消息类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Message_Types.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在讨论面板评论框中可以发布不同类型的消息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息分类</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文本消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">仅包含文本的消息评论/回复。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有关手动消息的更多信息，请参阅创建文本评论。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">快照消息</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/320f28bbce6f.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">包含快照的消息评论/回复。可能包含或不包含文本。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息中的缩略图可能包含或不包含批注。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有关快照消息的更多信息，请参阅创建快照评论。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">音频消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">包含音频的消息评论/回复。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0abd94d3cfa7.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">视频消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">包含视频的消息评论/回复。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">自动消息</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/2519b004d0f5.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">自动消息由项目的历史更新创建。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">这些消息类型和内容仅可由管理员配置。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ad878615c642.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">自动消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Automated messages are created by history updates to the item.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">These message types and content are configurable by admin only.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/136f6f7eec9c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'消息类型 在讨论面板评论框中可以发布不同类型的消息。 消息分类 说明 文本消息 仅包含文本的消息评论/回复。 有关手动消息的更多信息，请参阅创建文本评论。 快照消息 包含快照的消息评论/回复。可能包含或不包含文本。 消息中的缩略图可能包含或不包含批注。 有关快照消息的更多信息，请参阅创建快照评论。 音频消息 包含音频的消息评论/回复。 视频消息 包含视频的消息评论/回复。 自动消息 自动消息由项目',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [170/244] Method_Properties.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('6349ce97015d',
   N'方法属性',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">方法属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Method_Properties.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">方法属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">必填</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">是</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">方法的名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">方法类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">是</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">编写方法所用的语言</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">代码</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">否</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">输入方法代码的位置</Run></Paragraph>
</FlowDocument>',
   N'方法属性 属性名称 必填 说明 名称 是 方法的名称 方法类型 是 编写方法所用的语言 代码 否 输入方法代码的位置',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [171/244] Modifying_an_Action.htm (3 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('f6e21e9ea1ff',
   N'修改操作',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">修改操作</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Modifying_an_Action.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">修改操作</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在树视图中展开管理文件夹。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击操作节点以在主窗口中显示其默认页面。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从网格中选择一个操作行，然后点击主工具栏中的编辑项目。将显示对话窗口。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f7026e793cda.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">根据需要修改属性字段。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击保存、解锁、关闭。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ebc292158fb2.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b8d663ba4220.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'修改操作 在树视图中展开管理文件夹。 点击操作节点以在主窗口中显示其默认页面。 从网格中选择一个操作行，然后点击主工具栏中的编辑项目。将显示对话窗口。 根据需要修改属性字段。 点击保存、解锁、关闭。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [172/244] Modifying_a_FileType.htm (7 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('89550059ee77',
   N'修改文件类型',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">修改文件类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Modifying_a_FileType.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">修改文件类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从导航窗格中，选择 管理 &gt; 文件处理 &gt; 文件类型。将出现以下菜单：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2. 点击「搜索文件类型」。将出现文件类型搜索网格。点击查看文件类型：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5d6f5d1834f2.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">3. 双击搜索网格中的条目以查看文件类型：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9bcf17c7b30c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">4. 点击认领项目并根据需要修改属性字段。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0161f4cf8f02.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">5. 点击保存并解锁项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击保存、解锁、关闭</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/12579553dc9e.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">4. Click to claim the item and modify the property fields as needed.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/eaf68ab55cfa.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">5. 点击保存并取消认领项目。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击保存、解锁、关闭</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ebc292158fb2.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'修改文件类型 从导航窗格中，选择 管理 > 文件处理 > 文件类型。将出现以下菜单： 2. 点击「搜索文件类型」。将出现文件类型搜索网格。点击查看文件类型： 3. 双击搜索网格中的条目以查看文件类型： 4. 点击认领项目并根据需要修改属性字段。 5. 点击保存并解锁项目。 点击保存、解锁、关闭 4. Click to claim the item and modify the property f',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [173/244] Modifying_a_Form.htm (3 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('da1df8be5cbf',
   N'修改表单',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">修改表单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Modifying_a_Form.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">修改表单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在树视图中展开管理文件夹。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击表单节点以在主窗口中显示其默认页面。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从网格中选择一个表单行，然后点击主工具栏中的编辑项目。将显示对话窗口。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f7026e793cda.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">根据需要修改属性字段。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击保存、解锁、关闭。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ebc292158fb2.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/437e8f7803de.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'修改表单 在树视图中展开管理文件夹。 点击表单节点以在主窗口中显示其默认页面。 从网格中选择一个表单行，然后点击主工具栏中的编辑项目。将显示对话窗口。 根据需要修改属性字段。 点击保存、解锁、关闭。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [174/244] Modifying_a_Method.htm (3 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('f18bb17e4880',
   N'修改方法',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">修改方法</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Modifying_a_Method.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">修改方法</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在树视图中展开管理文件夹。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击方法节点以在主窗口中显示其默认页面。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从网格中选择一个方法行，然后点击主工具栏中的编辑项目。将显示对话窗口。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f7026e793cda.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">根据需要修改属性字段。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击保存、解锁、关闭。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ebc292158fb2.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/cac55f300f9a.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'修改方法 在树视图中展开管理文件夹。 点击方法节点以在主窗口中显示其默认页面。 从网格中选择一个方法行，然后点击主工具栏中的编辑项目。将显示对话窗口。 根据需要修改属性字段。 点击保存、解锁、关闭。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [175/244] Modifying_a_Variable.htm (3 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('f5a985cc3f76',
   N'修改变量',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">修改变量</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Modifying_a_Variable.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">修改变量</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在树视图中展开管理文件夹。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击变量节点以在主窗口中显示其默认页面。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从网格中选择一个变量行，然后点击主工具栏中的编辑项目。将显示对话窗口。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f7026e793cda.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">根据需要修改属性字段。有关变量属性的参考，请参阅变量属性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击保存、解锁、关闭。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ebc292158fb2.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d0fab90f9bb4.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'修改变量 在树视图中展开管理文件夹。 点击变量节点以在主窗口中显示其默认页面。 从网格中选择一个变量行，然后点击主工具栏中的编辑项目。将显示对话窗口。 根据需要修改属性字段。有关变量属性的参考，请参阅变量属性。 点击保存、解锁、关闭。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [176/244] Modifying_a_Version.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('1a0e47ac5d79',
   N'修改版本',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">修改版本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Modifying_a_Version.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">修改版本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在树视图中展开管理文件夹。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击版本节点以在主窗口中显示其默认页面。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从网格中选择一个版本行，然后点击主工具栏中的编辑项目图标。将显示版本 [版本名称] 对话窗口。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">根据需要修改属性字段。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击保存、解锁、关闭。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ebc292158fb2.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'修改版本 在树视图中展开管理文件夹。 点击版本节点以在主窗口中显示其默认页面。 从网格中选择一个版本行，然后点击主工具栏中的编辑项目图标。将显示版本 [版本名称] 对话窗口。 根据需要修改属性字段。 点击保存、解锁、关闭。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [177/244] Modifying_a_Viewer.htm (3 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('794de55ded1c',
   N'修改查看器',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">修改查看器</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Modifying_a_Viewer.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">修改查看器</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在树视图中展开管理文件夹。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">展开文件处理并选择查看器节点以在主窗口中显示其默认页面。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从网格中选择一个文件类型行，然后点击主工具栏中的编辑项目。将显示对话窗口。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f7026e793cda.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">根据需要修改属性字段。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击保存、解锁、关闭。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ebc292158fb2.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/168f95ba2b54.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'修改查看器 在树视图中展开管理文件夹。 展开文件处理并选择查看器节点以在主窗口中显示其默认页面。 从网格中选择一个文件类型行，然后点击主工具栏中的编辑项目。将显示对话窗口。 根据需要修改属性字段。 点击保存、解锁、关闭。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [178/244] Modifying_the_RelationshipType_Item.htm (11 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('1b120a448367',
   N'修改关系类型项目',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">修改关系类型项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Modifying_the_RelationshipType_Item.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">修改关系类型项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当从源项目创建关系时，自动创建的项目之一是关系类型项目。关系类型可以在目录中的管理文件夹下找到。但是，您不应在目录中创建关系类型。它们应在源 ItemType 中创建。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在我们的示例中，当我们从源 ItemType HousePlanner 创建到关联 ItemType Contractor 的关系时，我们将其称为 ContractorRel。因此，让我们检查 ContractorRel。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要编辑关系类型项目的属性：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从目录 --&gt; 管理 中，选择关系类型。搜索 ContractorRel 项目并打开进行编辑。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">让我们先探讨头部属性。请记住，此项目控制关系本身的行为。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b5131d577328.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">属性名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系的名称，在源 ItemType 中定义关系时给出</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标签</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">源项目实例中选项卡的标签。在此选项卡下，所有关联的项目实例（本例中为承包商）将显示在关系网格中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">描述</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系的描述。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">自动搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选中时，将自动使用关联的项目实例填充关系网格。请记住，关系网格位于源项目实例中的此关系选项卡下。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">排序顺序</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">确定选项卡在源项目显示中的相对位置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">默认页面大小</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设置源项目中此关系选项卡下关系网格中的页面大小</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">源 ItemType</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在关系中充当「父级」的 ItemType 的唯一名称。每个关系类型只能声明一个源，并在创建关系时自动设置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">全部隐藏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">此选项仅在未选择源项目时适用。在这种情况下，此关系使用所有 ItemType 作为源项目，并在所有 ItemType 与关联 ItemType 之间创建关系。在这种情况下，用户可以选择隐藏此关系。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关联 ItemType</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关联项目。在源 ItemType 中定义关系时会自动填充。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">最小出现次数</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设置关系网格的最小行数。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">上面的网格有 5 个项目。如果最小值设置为 6，您在保存上面显示的项目时会收到错误。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b5b829f17b5f.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">最大出现次数</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设置关系网格的最大行条目实例数。参见最小出现次数中显示的项目。它有 5 行。如果最大值设置为 4，您在保存该项目时会收到错误。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">行为</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">仅当关联项目可版本化时适用，并确定关联项目的哪个版本绑定到源项目，如下所示：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">固定 - 源始终指向关联项目的特定世代号。如果关联项目被修改，源与关联之间的关系项目不会更改。关联项目的新世代号将没有关系项目指向它。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">浮动 - 源始终指向关联项目的最新世代号。每当关联项目被修改时，源与关联之间的关系项目将被更新。关联项目的上一世代号将不再有关系指向它。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">硬固定 - 源始终指向关联项目的特定世代号。如果关联项目被修改，源与关联之间的关系项目不会更改。关联项目的新世代号将没有关系项目指向它。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">硬浮动 - 源始终指向关联项目的最新世代号。每当关联项目被修改时，源与关联之间的关系项目将被更新。关联项目的上一世代号将不再有关系指向它。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">网格视图</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系网格显示两种类型的属性 - 关联项目属性和关系本身的属性。网格视图的可能值为：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">右 - 将关联项目属性（列）放置在网格的右侧；关系属性列在这些列的左侧</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">左 - 将关联项目属性放置在网格的左侧，在所有关系属性列出之后</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">混合 - 列仅按每个属性的排序顺序值排序，因此关系属性和关联属性将混合在一起。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">网格视图还与 ItemType 上的排序属性一起工作，以确定行排序的属性优先级。在关系的情况下，关联项目可能有一个排序值，关系本身也可能有一个排序值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">右 - 关系 ItemType 排序优先</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">左 - 关联 ItemType 排序优先</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">混合 - 无优先级；排序值组合提供绝对排序序列。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">考虑以下示例。假设我们有一个源 Part，它与其他 Part 有 BOM 关系。Part ItemType 已定义为名称排序 = 10，零部件编号排序 = 20。在 BOM 关系本身上，有序列和数量属性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以下是父零部件的 BOM 选项卡在网格视图设置为左时的视图。请注意，关联项目属性列出现在左侧，而关系属性列随后。另请注意，列表首先按关联项目的名称属性排序（排序 = 10），然后按零部件编号排序（排序 = 20）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以下是父零部件的 BOM 选项卡在网格视图设置为右时的视图。请注意，关联项目属性列出现在右侧，在关系属性列之后。另请注意，零部件列表按数量排序，然后按序列排序，因为关系排序优先于关联 ItemType 排序。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以下是源或父零部件的 BOM 选项卡在网格视图设置为混合时的视图。请注意，列按每个属性的排序顺序指示的绝对顺序排列。行的顺序也由组合排序控制。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/bb0387cfe7f7.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">仅选择</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在关系网格中创建新的关联条目时，您只能从现有的关联项目实例中选择。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/8992b11bec9a.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">仅创建</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在关系网格中创建新的关联条目时，您只能创建关联项目的新实例。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d5fcb16cbd6f.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">选择和创建</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在关系网格中创建新的关联条目时，您可以从现有的关联项目实例中选择，也可以创建新实例来填充关系。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">需要关联</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选中时，「无关联」选项将从源实例的下拉框中排除。因此，在为源实例创建新的关系条目时，您只有两个选择：选择关联或创建关联。无论哪种方式，关联项目都是必需的。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开关联表单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果为 true，在关系网格中创建新的关联条目并创建关联项目的新实例时，关联项目的表单将自动打开以进行编辑。如果为 false，您只需将数据输入属性列中即可。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">复制权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">此选项仅在创建关联为 True 时相关（见下文）。此情况涉及以下事件：有一个名为 Part1 的 Part 实例，在其 BOM 中有一个名为 Bracket1 的 Bracket 实例。用户复制 Part1 的 BOM 并将其粘贴到另一个 Part 中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果原始实例 Bracket1 有私有权限，用户可以通过将此标志设置为 true 来选择复制这些特定权限：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f6d5420069fe.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">复制权限 = True - 原始零部件（Bracket1）的权限被新创建的零部件（Bracket1 的副本）引用</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">复制权限 = False - 新创建的零部件（Bracket1 的副本）将具有其 ItemType 定义上设置的默认权限。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建关联</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">此选项仅在复制和粘贴关系时相关。让我们以 Part 装配为例，在其 BOM 中有一个名为 Bracket 的零部件。如果您复制 Bracket 并将其粘贴到另一个父 Part 的 BOM 中，以下选项适用：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建关联 = False - 新的父级将引用与原始 Part 装配相同的 Bracket 实例</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/51ee012ecc2a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">创建关联 = True - 新的父级将引用新创建的 Bracket 实例，即 Bracket 的副本。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性输入完成后，下一步是向选项卡输入信息：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">网格事件 - 如果您希望运行客户端方法，由网格事件触发（如：OnSelectRow、OnInsertRow 和 OnDeleteRow），您可以在此处放置您的方法。如果您希望在某些列中填充数据，您可能需要考虑这些方法。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系视图 - 有时您可能不希望在点击源或父实例中的关系选项卡时看到基本关系网格。您可能希望看到网页或可配置网格。此选项卡是您定义这些视图的地方。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">让我们检查此选项卡的属性或列：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">名称 - 此视图有效的身份名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Create Related = True - The new parent will reference a newly created Bracket instance, Copy of Bracket.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/2c22894346c8.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Once the properties are entered, the next step is to enter information into the tabs:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Grid Events - If you wish to run client side methods, triggered on grid events such as: OnSelectRow, OnInsertRow, and OnDeleteRow, you would put your methods here. You might want to consider these methods if say you wish to populate some data into a relationship automatically when it is first created (OnInsertRow). For a great explanation on how to write client side method and how to set the other properties of this tab, such as RunAs User, please refer to the Aras Advanced Programming Training.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/2c603f62fddf.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Relationship View - Sometimes you may not wish to see the basic relationship grid when you click the relationship tab in the source or parent instance. You may perhaps wish to see a web page, or a configurable grid . This tab is the place where you would specify these settings. Also, these settings are identity based, so different users could be directed to different pages or grids. Here is an example of using a configurable grid instead of the basic relationship grid:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/bf6ff9d0bd43.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Let&apos;s examine the properties or columns of this tab:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Name - the name of the identity for which this view is valid</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Start Page - the http:// path to the web page to be displayed</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Parameters - the inputs (if any) that you wish to pass to the web page</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Grid - the reference (chosen from a selection dialog) to a configurable grid designed for this view</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Exclusion - Sometimes relationships may be mutually exclusive. For example, you may have a relationship such as a BOM for a part, and a relationship such as Vendors for a part. However, the part is either made in house, in which case you would need a BOM tab, or it is bought from a vendor, in which case you only need a Vendors tab. So, on the Exclusion tab you can specify a list of relationships that are mutually exclusive. In this example, in the RelationshipType for BOMRel, you would specify VendorsRel under the Exclusion Tab.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Hide In - When selected, this option will hide the relationship tab in all Source item instances. For example, if we edit the BOM relationship, and select Hide In Tab View, the BOM tab will be hidden in all instances.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/caabc9cb6f76.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'修改关系类型项目 当从源项目创建关系时，自动创建的项目之一是关系类型项目。关系类型可以在目录中的管理文件夹下找到。但是，您不应在目录中创建关系类型。它们应在源 ItemType 中创建。 在我们的示例中，当我们从源 ItemType HousePlanner 创建到关联 ItemType Contractor 的关系时，我们将其称为 ContractorRel。因此，让我们检查 Contracto',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [179/244] Modifying_the_relationship_ItemType.htm (4 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('d94790d91e72',
   N'我的讨论',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">我的讨论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Modifying_the_relationship_ItemType.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">我的讨论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">「我的讨论」功能提供了一个安全的社交讨论环境，用户可以在其中讨论任何项目或主题。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要了解更多关于「我的讨论」的信息，请参阅以下主题：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">书签</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">书签面板</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">讨论面板</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/db7dc80e4a76.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">消息类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">论坛</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用讨论面板</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许私有权限 -</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Usually, for every relationship we will want to add some properties that are specific to the relationship itself. For example, for a Part, in the BOM relationship we have a property called quantity. This property is placed on the relationship because it only makes sense in the context of both the source and the related parts. In our example of the ContractorRel, we will also add some relationship specific quantities as follows:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Add a property called ManHours Per Week. The value of this property will indicate how many hours of a particular contractor will be required for a particular property.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Add a property called Pay Rate. The value of this property will vary with each contractor and property. In other words, the same contractor may charge a different rate in two different properties because of the difficulty level.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Make sure that Hidden2 for both of the added properties is not checked. We do want these properties to show in the relationship grid. Also, uncheck Hidden2 for any system properties that you might wish to be displayed in the relationship grid.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5ecd07ceda56.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">保存、解锁并关闭关系 ItemType。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ebc292158fb2.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Create an instance of the HouseKeeper Item and add some contractors. You should see a form similar to the following:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d184d618b8ca.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Notice that the related item properties are all displayed on the left which is the default we set back in the RelationshipType configuration, followed by the relationship properties.</Run></Paragraph>
</FlowDocument>',
   N'我的讨论 「我的讨论」功能提供了一个安全的社交讨论环境，用户可以在其中讨论任何项目或主题。 要了解更多关于「我的讨论」的信息，请参阅以下主题： 书签 书签面板 讨论面板 消息类型 论坛 使用讨论面板 Allow Private Permissions - Usually, for every relationship we will want to add some properties that',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [180/244] My_Discussions.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('b279d0021da4',
   N'我的讨论',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">我的讨论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: My_Discussions.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">我的讨论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">「我的讨论」提供了一个安全的社交讨论环境，用户可以在其中讨论任何项目或主题。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">功能包括：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">书签面板 - 管理关注的讨论主题</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">讨论面板 - 查看和发布评论</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/08589e82c105.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">消息类型 - 支持文本、快照、音频和视频消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">论坛 - 创建和管理讨论论坛</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用讨论面板 - 在讨论面板中输入评论、回复消息、标记重要消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建新论坛并从论坛发送消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">基于书签搜索消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关注来自特定用户或论坛的消息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">这包括以下面板：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">讨论面板</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">书签面板</Run></Paragraph>
</FlowDocument>',
   N'我的讨论 「我的讨论」提供了一个安全的社交讨论环境，用户可以在其中讨论任何项目或主题。 功能包括： 书签面板 - 管理关注的讨论主题 讨论面板 - 查看和发布评论 消息类型 - 支持文本、快照、音频和视频消息 论坛 - 创建和管理讨论论坛 使用讨论面板 - 在讨论面板中输入评论、回复消息、标记重要消息 Create a new Forum and send messages from the Fo',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [181/244] My_Discussions_UI.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('a7c372ce5931',
   N'讨论面板（我的讨论）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">讨论面板（我的讨论）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: My_Discussions_UI.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">讨论面板</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">目录中我的 Innovator 部分中我的讨论的讨论面板如下所示：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">主窗口中我的讨论的讨论面板与项目独立窗口中的讨论面板几乎相同。有关讨论面板工具栏和消息组件的参考，请参阅讨论面板工具栏和消息的组成部分。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/43cab4e16dba.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">但是，它们的某些部分功能有细微差别。它们列在这里：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">评论输入框和评论按钮：仅在论坛和论坛内的项目或书签组中启用。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">附加链接：此链接永远不会出现在评论或回复框中，因为查看器永远不会在「我的讨论」主窗口中显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">引用项目上的超链接：消息的引用项目上的超链接将打开相应项目的独立窗口，具有引用中指定的正确修订和世代号。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">缩略图或快照的超链接：在查看器中显示快照图像的超链接将打开关联公告板项目的独立窗口，并在查看器中显示选定的快照。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Hyperlink from thumbnail or snapshot: The hyperlink to show the snapshot image in the viewer will open up the tear-off window of the associated Board item and shows the selected snapshot in the viewer window.</Run></Paragraph>
</FlowDocument>',
   N'讨论面板 目录中我的 Innovator 部分中我的讨论的讨论面板如下所示： 主窗口中我的讨论的讨论面板与项目独立窗口中的讨论面板几乎相同。有关讨论面板工具栏和消息组件的参考，请参阅讨论面板工具栏和消息的组成部分。 但是，它们的某些部分功能有细微差别。它们列在这里： 评论输入框和评论按钮：仅在论坛和论坛内的项目或书签组中启用。 附加链接：此链接永远不会出现在评论或回复框中，因为查看器永远不会在「我',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [182/244] My_inBasket.htm (5 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('d737bf5747c8',
   N'我的收件箱',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">我的收件箱</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: My_inBasket.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">我的收件箱</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">收件箱是从工作流和项目创建的分配的目的地。当项目启动工作流流程时，被分配的用户会在收件箱中收到有关待处理活动的通知。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要从目录访问收件箱，请导航到 我的 Innovator --&gt; 我的收件箱。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">收件箱显示受让人的工作流活动和项目活动。用户可以按每种活动类型过滤活动或待处理的分配。收收件箱还提供工作流和项目项目的链接。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/50c427c6daad.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">注意：项目活动仅在您安装了项目管理时才出现。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">按类型过滤</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">活动显示选项。两个选项都可以选中，它们不是互斥的分组。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工作流活动 - 切换所有工作流相关活动的显示</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目活动 - 切换所有项目活动的显示</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">按状态过滤</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户可以按活动或待处理状态过滤。这两个选择影响按类型过滤分组的显示，两者都可以选中，因为它们不是互斥的分组。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">活动 - 切换您当前活动的活动的显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">待处理 - 切换所有待处理的活动以提供未来工作的视图。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用上下文菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用上下文菜单（使用鼠标右键（右键点击）打开工作流活动），您可以执行以下操作：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">认领任务：锁定所选活动并打开关联项目进行编辑。搜索网格中将出现绿色标记图标，指示活动已被您认领。红色标记图标指示活动已被其他用户认领。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">活动可能被分配给组身份并发送给身份组的所有成员。许多成员收到工作要完成的通知，但未专门分配给一个用户。您需要认领活动才能处理它。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/dbc3664f0732.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/386f5bfc5361.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">如果您尝试在另一个用户锁定工作流活动完成工作表时更改它，将出现类似以下的警告对话框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">查看工作流：打开与所选活动关联的工作流。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">更新活动：打开工作流活动完成窗口，引导受让人完成任务列表以及在活动完成之前要执行的任何其他工作。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a05bbd7dcca0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">更新工作流活动</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1. 登录 Aras Innovator。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2. 导航到 我的 Innovator &gt; 我的收件箱。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">3. 从主网格中选择要更新的活动，点击鼠标右键（右键点击）并从出现的上下文菜单中选择「更新活动」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将显示工作流活动完成窗口。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">更新活动：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择您已完成的任务的「完成」复选框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：如果任务的「必填」复选框被选中，意味着您必须完成该任务。如果您尝试在未完成必填任务的情况下投票并完成工作流活动，将出现错误消息。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d1d7cdbe4e77.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">2. 通过从投票下拉菜单中选择适当的选项提交您的投票。投票选项基于工作流流程定义动态构建。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">3. 如果需要，输入您的密码和/或电子签名进行认证。根据管理员的配置，您可能需要提供密码或电子签名进行认证。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">4. 点击以下之一：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">完成：检查活动（确保所有必填任务已被选中、变量已输入等），将活动标记为完成，并继续工作流。保存更改：保存表单上输入的所有信息，但不处理活动。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">完成：检查活动（确保所有必填任务已被选中、变量已输入等），将活动标记为完成，并继续工作流。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存更改：保存表单上输入的所有信息，但不处理活动。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">此选项适用于具有长任务列表的活动，受让人希望跟踪已完成的内容。此外，如果活动被分配给组，则不同成员可以保存他们的更改。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">取消：关闭表单，不保存自打开或上次保存以来对其所做的任何更改。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Save Changes: Saves all information entered on form, but does not process the activity.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">This is a useful option for activities having a long list of tasks, and an assignee wishes to keep track of what has been completed. Also, if the activity is assigned to a group, then different members of the group may work on different tasks. Once a task is completed, marked as complete, and saved, all members of that group see this information on their Worksheets, thereby reducing duplication of effort.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Cancel: Closes the Form without saving any of changes made to it since it was opened or last saved.</Run></Paragraph>
</FlowDocument>',
   N'我的收件箱 收件箱是从工作流和项目创建的分配的目的地。当项目启动工作流流程时，被分配的用户会在收件箱中收到有关待处理活动的通知。 要从目录访问收件箱，请导航到 我的 Innovator --> 我的收件箱。 收件箱显示受让人的工作流活动和项目活动。用户可以按每种活动类型过滤活动或待处理的分配。收收件箱还提供工作流和项目项目的链接。 注意：项目活动仅在您安装了项目管理时才出现。 按类型过滤 活动显示',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [183/244] My_Innovator.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('370a1dc0b4d6',
   N'我的 Innovator',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">我的 Innovator</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: My_Innovator.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">我的 Innovator</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">产品中内置了一个标准界面，允许最终用户访问：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">我的讨论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">我的收件箱</Run></Paragraph>
</FlowDocument>',
   N'我的 Innovator 产品中内置了一个标准界面，允许最终用户访问： 我的讨论 我的收件箱',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [184/244] Navigation.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('641895917d75',
   N'我的 Innovator（续）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">我的 Innovator（续）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Navigation.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">我的 Innovator</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">产品中内置了一个标准界面，允许最终用户访问：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">我的讨论 - 在 Aras Innovator 中提供易于使用的社交协作和查看批注功能界面。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">我的收件箱 - 从工作流创建的分配的目的地。</Run></Paragraph>
</FlowDocument>',
   N'我的 Innovator 产品中内置了一个标准界面，允许最终用户访问： 我的讨论 - 在 Aras Innovator 中提供易于使用的社交协作和查看批注功能界面。 我的收件箱 - 从工作流创建的分配的目的地。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [185/244] New_Topic.htm (2 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('621d3016bd72',
   N'书签',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">书签</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: New_Topic.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">书签</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">书签代表您正在关注的社交内容。所选书签的讨论内容显示在讨论面板中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">默认书签</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/2a92e6f69c02.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">默认书签自动设置并对所有用户可用：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">所有书签：这是打开「我的讨论」时的默认书签，一次显示所有书签的聚合内容。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">来自我的消息：您创建的所有消息（不包括用户间接创建的历史消息）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">这些书签始终按此顺序显示在顶部且不能折叠。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">书签组</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户可以创建/添加的书签分为 4 个书签组：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">人员：代表指定人员创建的所有消息的书签（不包括用户间接创建的历史消息）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">论坛：代表论坛的书签。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目：代表单个项目的书签。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">每个组在书签中都有一个标题，允许显示或折叠该组。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">书签搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索书签字段出现在书签面板顶部。在此字段中输入搜索条件并点击图标。搜索通过在书签名称中搜索输入的字符串来完成。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当搜索结果生效时，只有匹配搜索条件的书签会显示在讨论面板中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">书签的上下文菜单功能</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7c16ff277649.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">书签具有以下可用的上下文菜单命令：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">书签类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">上下文菜单选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在默认组中</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设为默认</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设置用户将来打开「我的讨论」时使用的默认书签（默认为「所有消息」）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">分享...更改论坛的共享设置。打开分享论坛对话框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开在独立窗口中打开项目的表单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">移除如果选中书签则从书签中移除。如果选中论坛内的项目或保存搜索则从论坛中移除。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">来自我的消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">移除</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设为默认</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">论坛</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">分享...</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">移除</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设为默认</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">论坛内的项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">移除</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设为默认</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">论坛内的保存搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">移除</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目（在项目组中）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">移除</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设为默认</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">移除</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设为默认</Run></Paragraph>
</FlowDocument>',
   N'书签 书签代表您正在关注的社交内容。所选书签的讨论内容显示在讨论面板中。 默认书签 默认书签自动设置并对所有用户可用： 所有书签：这是打开「我的讨论」时的默认书签，一次显示所有书签的聚合内容。 来自我的消息：您创建的所有消息（不包括用户间接创建的历史消息）。 这些书签始终按此顺序显示在顶部且不能折叠。 书签组 用户可以创建/添加的书签分为 4 个书签组： 人员：代表指定人员创建的所有消息的书签（不',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [186/244] PackageDefinitions.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('e4a3ad00f95a',
   N'包定义',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">包定义</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: PackageDefinitions.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">包定义</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">包定义由管理员和开发者用于将 ItemType 定义从一个 Aras Innovator 数据库复制或传输到另一个数据库，以及将数据库升级到更高版本。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中，点击 管理 &gt; 配置 &gt; 包定义。将出现以下菜单：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">包定义由导出和导入工具使用。包定义和这些工具的使用在 Aras Innovator 12.0 - 包导入导出实用程序.pdf 中有文档说明，该文档可在发行版 CD 上获得。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/1cb4e9d235d9.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Package Definitions are used by Export, and Import Tools. The use of Package Definitions and these tools are documented in the Aras Innovator 12.0 - Package Import Export Utilities.pdf which is available on the release CD.</Run></Paragraph>
</FlowDocument>',
   N'包定义 包定义由管理员和开发者用于将 ItemType 定义从一个 Aras Innovator 数据库复制或传输到另一个数据库，以及将数据库升级到更高版本。 在导航窗格中，点击 管理 > 配置 > 包定义。将出现以下菜单： 包定义由导出和导入工具使用。包定义和这些工具的使用在 Aras Innovator 12.0 - 包导入导出实用程序.pdf 中有文档说明，该文档可在发行版 CD 上获得。 ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [187/244] Parameterized_Search.htm (4 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('dd04474c2409',
   N'参数化搜索（续）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">参数化搜索（续）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Parameterized_Search.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">参数化搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用参数化搜索方法搜索具有一个或多个项目属性的项目，其中您搜索的属性保持不变，但要搜索的值不同。您可以预配置需要执行搜索的项目属性并保存搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您可以在任何搜索模式下使用参数化搜索，但它们在保存的搜索中最为有用。当主网格中显示具有大量属性的项目时，此搜索也很有用。在这种情况下，仅搜索少数属性会很不方便。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例：如果您经常搜索处于「已发布」状态的零部件，但搜索中包含的成本、修订或零部件编号各不相同，则可以使用参数化搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您可以使用语法 @{n}（其中 n 是数字）预定义要包含在搜索中的项目属性（参数）。可以有任意数量的参数，数字不必是连续的。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">语法 @{n} 是参数化搜索的保留语法。它始终被解释为参数；您不能搜索字符串 &apos;@{1}&apos;。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建和使用参数化搜索：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">登录 Aras Innovator。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在目录中，导航到要搜索的项目。例如，要搜索零部件，请导航到 设计 --&gt; 零部件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在搜索栏中，通过在列中指定 @{n} 来定义搜索中要使用的属性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在我们的示例中，让我们使用以下项目属性：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">零部件编号：@{1}*</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">状态：@{2}*</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">变更：@{3}</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">导航到 搜索 --&gt; 保存搜索 以保存参数化搜索。保存搜索是可选步骤。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d0685feab876.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击执行搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将出现「设置参数值」对话框。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4e972777ad17.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">图2</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">指定要搜索的值。让我们使用以下条件搜索零部件：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/48b639bdca41.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">零部件编号以 1000* 开头</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">处于已发布状态</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">未包含在变更管理中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击确定。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">主网格将显示与您的搜索条件匹配的项目列表。每次执行搜索时，都会出现一个显示您定义的项目属性的搜索对话框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您可以指定要搜索的项目属性的值。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a8aabe00532d.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击确定。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The main grid displays the list of items matching your search criteria. Every time you execute the search, a search dialog displaying the Item Properties you defined appears.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">You can specify the values of the Item&apos;s properties you want to conduct the search for.</Run></Paragraph>
</FlowDocument>',
   N'参数化搜索 使用参数化搜索方法搜索具有一个或多个项目属性的项目，其中您搜索的属性保持不变，但要搜索的值不同。您可以预配置需要执行搜索的项目属性并保存搜索。 您可以在任何搜索模式下使用参数化搜索，但它们在保存的搜索中最为有用。当主网格中显示具有大量属性的项目时，此搜索也很有用。在这种情况下，仅搜索少数属性会很不方便。 示例：如果您经常搜索处于「已发布」状态的零部件，但搜索中包含的成本、修订或零部件编',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [188/244] about_permissions.htm (2 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('ec79f7d29fba',
   N'关于权限',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关于权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: about_permissions.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关于权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">就像 Aras Innovator 中几乎所有其他东西一样，权限是一个项目，因此具有某些属性。这些属性基本上保存了身份的名称以及该身份对该 ItemType 所拥有的权利和特权。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您可以看到它为许多不同的身份设置了某些权利。例如，「所有员工」身份具有获取或「查看」访问权限以及「可以发现」。这些权利或特权是自解释的。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b91cc0c92994.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">接下来，要授予 ItemType 所需的权限，您通过 ItemType 表单上的权限选项卡将 ItemType 与权限项目关联。以下是 Part ItemType 的视图，权限选项卡处于活动状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">请注意，分配给此项目的权限是我们上面检查的权限。另一个值得注意的选项卡是「可添加」选项卡，这是设置添加或创建项目的权限的地方。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建权限</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b7d192bf8a14.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">可添加权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可添加权限</Run></Paragraph>
</FlowDocument>',
   N'关于权限 就像 Aras Innovator 中几乎所有其他东西一样，权限是一个项目，因此具有某些属性。这些属性基本上保存了身份的名称以及该身份对该 ItemType 所拥有的权利和特权。 您可以看到它为许多不同的身份设置了某些权利。例如，「所有员工」身份具有获取或「查看」访问权限以及「可以发现」。这些权利或特权是自解释的。 接下来，要授予 ItemType 所需的权限，您通过 ItemType ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [189/244] creating_a_permission.htm (7 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('4c1fb1940986',
   N'创建权限（续）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建权限（续）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: creating_a_permission.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您可以创建权限然后分配给 ItemType，也可以直接从 ItemType 的权限选项卡创建权限。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建权限：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在目录中，选择 管理 -&gt; 权限 -&gt; 创建新权限。将出现以下窗口：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">输入权限名称。名称通常描述性地说明正在创建的权限类型或权限将应用到的项目。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7057585135a7.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">下一步是指定访问权限。访问设置在下表中描述。请注意，Can Add 权限和 TOC Access 权限不在此处的权限网格中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">特权</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">定义</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">获取</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使您能够检索/查看项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">更新</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使您能够编辑现有项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使您能够删除项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可以发现</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使您能够了解项目是否存在。参阅发现权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可以更改访问权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使您能够更改已创建项目的访问设置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示权限警告</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">与「可以发现」一起工作，决定当没有发现权限的项目应在搜索中返回时是否向用户显示警告。如果设置，将显示警告，否则不显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">TOC 访问</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用户能够通过目录（TOC）访问项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可添加</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用户能够创建项目的实例</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要输入访问值：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">请注意操作字段旁边的下拉框。您可以选择「选择关联」或「创建关联」。选择「选择关联」将弹出身份搜索对话框。选择「创建关联」使您能够当场创建一个身份。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从列表中选择一个身份并点击绿色箭头将其添加到访问选项卡。所选身份将出现在权限项目的访问选项卡上。选择您希望此身份拥有的特权。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果您从下拉框中选择了「创建关联」，将创建一个新的空行。您可以当场输入新身份的名称并添加特权。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/503fda9cfc3a.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">保存、解锁并关闭权限。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您可以设置一个权限，为许多不同的身份授予不同的特权。不要忘记身份可以是组和个人，如果个人身份包含在多个组中，则该个人的权限将是累积的。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将权限连接到 ItemType：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/35f5bfc7dd11.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">1. 从目录的管理中，选择 ItemTypes 并找到要分配权限的 ItemType。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2. 打开 ItemType 进行编辑，选择权限选项卡。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">3. 如果您已经创建了权限，请点击「选择项目」按钮，从出现的权限对话框中选择适当的权限，然后点击返回所选图标。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">4. 如果您尚未创建权限，可以通过在权限选项卡上选择「创建项目」图标来当场创建一个。您将按照前面概述的相同步骤进行操作。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">6. 点击保存 ItemType。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">3. If you have already created a permission, then click the Select Items button , select the appropriate permission from the Permission dialog that appears and click the Return Selected icon .</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/81d30419a164.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6a56781729d6.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">4. If you have not yet created a permission, you can create one on the fly, by choosing the Create Item icon on the Permissions tab. You will be guided through the same steps outlined earlier, except that the permission will already be assigned to the ItemType.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/93ef1556cc54.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">6. 点击保存 ItemType。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'创建权限 您可以创建权限然后分配给 ItemType，也可以直接从 ItemType 的权限选项卡创建权限。 创建权限： 在目录中，选择 管理 -> 权限 -> 创建新权限。将出现以下窗口： 输入权限名称。名称通常描述性地说明正在创建的权限类型或权限将应用到的项目。 下一步是指定访问权限。访问设置在下表中描述。请注意，Can Add 权限和 TOC Access 权限不在此处的权限网格中。 特权 ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [190/244] polyitem_setup.htm (4 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('866cd7dbabde',
   N'多态项目设置',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">多态项目设置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: polyitem_setup.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">多态项目设置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在项目管理中，可交付项目被定义为多态项目。由系统管理员决定哪些 ItemType 可以作为可交付物。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设置可交付 ItemType：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1. 在导航窗格中，选择 管理 &gt; ItemTypes。将出现以下菜单：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2. 点击「搜索 ItemTypes」并打开 Deliverable 进行编辑。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/32c5415833d5.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">3. 点击「多态源」选项卡。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">4. 点击「选择项目」按钮。将出现 ItemType 搜索对话框。选择将作为可交付物的任何 ItemType。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/db9cc72de491.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">5. 确保所选的 ItemType 设置了正确的权限，即项目管理和所有员工对每个这些 ItemType 都有「获取」访问权限。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">6. 点击保存并解锁 ItemType。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/81d30419a164.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">5. Make certain that the selected ItemTypes selected have the correct permissions set up, i.e. that Project Management and All Employees have &quot;get&quot; access to each of these ItemTypes.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">6. 点击保存并取消认领 ItemType。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'多态项目设置 在项目管理中，可交付项目被定义为多态项目。由系统管理员决定哪些 ItemType 可以作为可交付物。 设置可交付 ItemType： 1. 在导航窗格中，选择 管理 > ItemTypes。将出现以下菜单： 2. 点击「搜索 ItemTypes」并打开 Deliverable 进行编辑。 3. 点击「多态源」选项卡。 4. 点击「选择项目」按钮。将出现 ItemType 搜索对话框。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [191/244] Preferences.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('a219d23a2cf7',
   N'首选项',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">首选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Preferences.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">首选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">个别用户可以选择每个 ItemType 在标准用户界面中的显示方式。首选项在注销时存储在数据库中，并在用户再次登录时加载。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户可以通过从主菜单中选择 工具 --&gt; 首选项 --&gt; 重置用户首选项 来将其首选项重置为默认值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">管理员身份的成员可以在目录管理类别中查看和编辑首选项。World 身份也有项目管理的全局首选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在主网格中查看项目或在项目窗口的关系网格中查看相关项目时，可以进行以下更改：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可以通过右键点击列标题行并从上下文菜单中选择「隐藏列」或「插入列...」来隐藏或插入列。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可以通过拖动列标题之间的「分隔器」来调整列的大小。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可以通过将列标题向左或向右拖动来更改列的顺序。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可以设置搜索结果设置的默认值</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">页面大小</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">最大结果数（仅在主网格中）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">一个或多个列的过滤值</Run></Paragraph>
</FlowDocument>',
   N'首选项 个别用户可以选择每个 ItemType 在标准用户界面中的显示方式。首选项在注销时存储在数据库中，并在用户再次登录时加载。 用户可以通过从主菜单中选择 工具 --> 首选项 --> 重置用户首选项 来将其首选项重置为默认值。 管理员身份的成员可以在目录管理类别中查看和编辑首选项。World 身份也有项目管理的全局首选项。 在主网格中查看项目或在项目窗口的关系网格中查看相关项目时，可以进行以',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [192/244] Preferences_menu.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('2e2661a93ffe',
   N'首选项菜单',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">首选项菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Preferences_menu.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">首选项菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击用户菜单按钮将显示首选项菜单：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">下表说明了首选项菜单中每个可用选项。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/08e06afa24af.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">菜单选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用通配符</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您在搜索中使用通配符。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">更改密码</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您更改登录密码。有关如何更改密码的更多信息，请参阅更改密码。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">更改电子签名</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您更改电子签名。电子签名与密码分开。电子签名是由用户维护的用户控制密码，而非管理员或 IT 密码。这使每个用户可以完全控制其电子签名。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">讨论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您开启或关闭讨论。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您开启或关闭讨论功能。</Run></Paragraph>
</FlowDocument>',
   N'首选项菜单 点击用户菜单按钮将显示首选项菜单： 下表说明了首选项菜单中每个可用选项。 菜单选项 说明 使用通配符 允许您在搜索中使用通配符。 更改密码 允许您更改登录密码。有关如何更改密码的更多信息，请参阅更改密码。 更改电子签名 允许您更改电子签名。电子签名与密码分开。电子签名是由用户维护的用户控制密码，而非管理员或 IT 密码。这使每个用户可以完全控制其电子签名。 讨论 允许您开启或关闭讨论。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [193/244] Promoting_a_Lifecycle.htm (4 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('977dd95a54d7',
   N'在生命周期中提升项目',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">在生命周期中提升项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Promoting_a_Lifecycle.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在生命周期中提升项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">生命周期映射由管理员定义并与 ItemType 关联。这些映射为项目通过不同生命周期状态的进展提供规则。一个项目只能属于一个生命周期，但一个生命周期可以被多个项目共享。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果您尝试提升没有所需访问权限的项目，将显示错误消息「没有可用的提升」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要将项目提升到新的生命周期状态</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">登录 Aras Innovator。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从目录中，导航到项目。让我们考虑 Part ItemType。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">导航到 设计 --&gt; 零部件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从主网格中选择要提升的项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要提升项目，请使用以下选项之一：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击主工具栏上的提升。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/78b46b2ebb72.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">使用鼠标右键（右键点击）打开上下文菜单并选择「提升」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">或者，您可以打开 Part 项目窗口，点击项目工具栏上的提升或从项目菜单中选择 编辑 --&gt; 提升。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/78b46b2ebb72.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">注意：您无法提升已认领的项目。要提升项目，它应该是未认领的。工具栏上的提升按钮或菜单中的选项仅对未认领的项目启用。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将出现提升对话框，其中包含项目可以提升到的状态列表。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：提升对话框中显示的目标状态对每个生命周期映射都不同。生命周期映射由管理员配置。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a12281edcdf4.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">6. 选择所需的目标状态并点击确定。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">6. Select the required To State and click .</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/65c75ab80093.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'在生命周期中提升项目 生命周期映射由管理员定义并与 ItemType 关联。这些映射为项目通过不同生命周期状态的进展提供规则。一个项目只能属于一个生命周期，但一个生命周期可以被多个项目共享。 如果您尝试提升没有所需访问权限的项目，将显示错误消息「没有可用的提升」。 要将项目提升到新的生命周期状态 登录 Aras Innovator。 从目录中，导航到项目。让我们考虑 Part ItemType。 ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [194/244] Relationships_vs._Properties.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('b69b9294a268',
   N'关系与属性',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关系与属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Relationships_vs._Properties.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系与属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在所有数据建模的情况下，做同一件事有很多种方法，有些可能比其他的更高效，有些更灵活。例如，假设您希望访问另一个 ItemType 的属性值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系提供更大的灵活性，允许访问不仅仅是属性值</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系使一个 Item1 实例能够引用多个 Item2 实例</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">外部属性比关系更高效，不需要数据库中的两个额外表</Run></Paragraph>
</FlowDocument>',
   N'关系与属性 在所有数据建模的情况下，做同一件事有很多种方法，有些可能比其他的更高效，有些更灵活。例如，假设您希望访问另一个 ItemType 的属性值。 关系提供更大的灵活性，允许访问不仅仅是属性值 关系使一个 Item1 实例能够引用多个 Item2 实例 外部属性比关系更高效，不需要数据库中的两个额外表',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [195/244] RelationshipType_Item_Behavior.htm (2 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('83846ba1fca2',
   N'关系类型项目行为',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关系类型项目行为</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: RelationshipType_Item_Behavior.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系类型项目行为</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系类型中定义的属性之一是行为。虽然只有两个选择（浮动和固定），但根据此设置，可能发生许多变化。让我们探讨一下。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">第一种情况是 Parent1 与 Child1 相连。然后，Parent 被版本化为 Parent2。然后，Child 被版本化为 Child2。以下是关系连接的视图。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">第二种浮动情况应该提出一个问题：如果行为是浮动，为什么在 Child2 被创建后，Parent1 没有指向 Child2？这不符合浮动行为的定义吗？</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6cbf4a6345a0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">下一个情况类似，它开始于 Parent1 和 Child1 在创建时相关联。然后，子项目先被版本化以创建 Child2。之后，父项目被版本化为 Parent2。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在这种情况下，固定行为应该引起问题。为什么当行为是固定时，Parent2 与 Child2 关联？答案是因为另一个规则。当父项目的新世代号被创建时，它将自动与关联项目的最新可用世代号关联。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">另外两种行为 - 硬固定和硬浮动仅在与生命周期行为设置结合时才起作用。因此，我们将在生命周期的背景下探讨它们。有关详细示例，请参阅项目行为。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d12f4a613b37.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">In this case, the question should arise for the Fixed behavior. Why is Parent2 associated with Child2 when the behavior is fixed? The answer is because of another rule. When a new generation of the parent is created, it is automatically associated with the latest version of the children. So, if a later version of the child exists, even if the behavior is defined as fixed, it will automatically be associated with the new version of the parent.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The other two behaviors - Hard Fixed and Hard Float only play a role when combined with Life Cycle behavior settings. So, we will explore those in a context of a life cycle. See Item Behavior for an explanation of how life cycles can further change the behavior of related versionable items.</Run></Paragraph>
</FlowDocument>',
   N'关系类型项目行为 关系类型中定义的属性之一是行为。虽然只有两个选择（浮动和固定），但根据此设置，可能发生许多变化。让我们探讨一下。 第一种情况是 Parent1 与 Child1 相连。然后，Parent 被版本化为 Parent2。然后，Child 被版本化为 Child2。以下是关系连接的视图。 第二种浮动情况应该提出一个问题：如果行为是浮动，为什么在 Child2 被创建后，Parent1 ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [196/244] Relatioship_Toolbar.htm (16 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('cf54e93c5ad2',
   N'关系工具栏',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">关系工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Relatioship_Toolbar.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系工具栏包括各种按钮，用于在关系网格上执行各种任务。关系工具栏中的各种按钮列在下面：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">按钮名称</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/39fc77a8a122.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">按钮</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建新的关系项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">三个选项不一定总是启用，这取决于管理员所做的 ItemType 配置。根据管理员的配置，一个或所有操作已启用并可用。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/93ef1556cc54.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">选择项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开搜索对话框窗口以搜索关联项目，并用搜索到的项目替换所选项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除行</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除选定的关系。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/81d30419a164.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">运行搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">运行指定的搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">添加条件</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a8520640b00c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">当您选择高级搜索模式时，此按钮会出现。点击以添加搜索条件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">细化</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示与项目关联的属性和扩展属性列表。选择适当的属性和扩展属性用作条件。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3dcf870acda9.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">导出到 Excel</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从上下文菜单中选择「导出到 Excel」。将关系网格中列出的关联（子）项目的所有属性导出到 Microsoft Excel 工作表。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">导出到 Word</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/32ce6b8a0f4a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">从上下文菜单中选择「导出到 Word」。将关系网格中列出的关联（子）项目的所有属性导出到 Microsoft Word 文档。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">认领</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">锁定所选子（关联）项目并阻止其他用户提交更改。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/85f7827c5a4a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">取消认领</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">解锁所选子（关联）项目并允许任何授权用户锁定项目进行更改。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">提升</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c9f33042dea3.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">提供将关系项目提升到已分配生命周期映射中定义的下一个可用状态的选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">提升的是关系项目，而不是子项目或关联项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">运行搜索</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c9f33042dea3.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">执行搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">清除搜索条件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除已应用的搜索条件。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/dbc3664f0732.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">搜索模式下拉</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您选择搜索模式。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/28ae4d7344ef.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">启用选项卡红线视图。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">提升</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/33868a5cc35e.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Provides options to promote the relationship Item to the next available state, defined in the assigned Lifecycle map.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">提升的是关系项目，而非子项目或关联项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">执行搜索</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9bcf17c7b30c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">执行搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">清除搜索条件</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9502b80cff55.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">删除已应用的搜索条件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索模式下拉框</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3474c06e6d3a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">允许您选择搜索模式。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/222eac440b08.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">启用选项卡红线视图。</Run></Paragraph>
</FlowDocument>',
   N'关系工具栏 关系工具栏包括各种按钮，用于在关系网格上执行各种任务。关系工具栏中的各种按钮列在下面： 按钮名称 按钮 说明 创建项目 创建新的关系项目。 三个选项不一定总是启用，这取决于管理员所做的 ItemType 配置。根据管理员的配置，一个或所有操作已启用并可用。 选择项目 打开搜索对话框窗口以搜索关联项目，并用搜索到的项目替换所选项目。 删除行 删除选定的关系。 运行搜索 运行指定的搜索。 ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [197/244] Reports_Menu.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('218143297ee8',
   N'报告菜单',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">报告菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Reports_Menu.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">报告菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">报告菜单是基于报告 ItemType 的动态菜单。报告菜单中显示的菜单选项取决于登录用户的访问权限。访问权限由管理员配置。</Run></Paragraph>
</FlowDocument>',
   N'报告菜单 报告菜单是基于报告 ItemType 的动态菜单。报告菜单中显示的菜单选项取决于登录用户的访问权限。访问权限由管理员配置。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [198/244] Reports_Menu_(Item_Window).htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('3d030a5ada23',
   N'报告菜单（项目窗口）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">报告菜单（项目窗口）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Reports_Menu_(Item_Window).htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">报告菜单（项目窗口）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">报告菜单是基于报告 ItemType 的动态菜单。报告菜单中显示的菜单选项取决于登录用户的访问权限。访问权限由管理员配置。</Run></Paragraph>
</FlowDocument>',
   N'报告菜单（项目窗口） 报告菜单是基于报告 ItemType 的动态菜单。报告菜单中显示的菜单选项取决于登录用户的访问权限。访问权限由管理员配置。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [199/244] savedsearch.htm (6 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('ce42564427f6',
   N'保存的搜索（续）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">保存的搜索（续）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: savedsearch.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存的搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存的搜索 ItemType 使用户能够检索先前创建的搜索；它们可以通过多种方式使用。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户的保存搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">执行搜索后，您可以将其保存并命名，以便将来可以检索和重复使用（有或无修改）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">每当您执行搜索时，搜索模式和搜索词会自动保存。当您返回某个项目时，上次搜索会自动恢复。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">每个用户都可以执行自己创建的保存搜索，以及其他用户共享的搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在任何模式下进行搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「添加到收藏夹」按钮保存搜索。将出现「保存新的保存搜索」对话框。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a620e17f3be9.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9305c7c9a6a7.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">在「标签」字段中输入搜索名称。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「共享给：」字段中的省略号，与组织中的其他 Aras Innovator 用户共享搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">勾选「在 TOC 上显示」复选框以在目录中显示搜索的快捷方式图标。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击保存。您可以通过在导航面板中选择 管理 &gt; 保存的搜索 来访问搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">检索/应用保存的搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">登录 Aras Innovator。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择管理并在导航窗格中点击「保存的搜索」按钮。将出现保存的搜索网格。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5da5ed34617a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击查看保存的搜索列表。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9bcf17c7b30c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">右键点击要执行的搜索，然后点击「运行」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除保存的搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">登录 Aras Innovator。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中选择 管理 &gt; 保存的搜索。将出现保存的搜索网格。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击查看保存的搜索列表。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9bcf17c7b30c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">右键点击要删除的搜索，从上下文菜单中选择「删除所有版本」。将出现类似以下的对话框。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a8edd37a5897.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击确定。</Run></Paragraph>
</FlowDocument>',
   N'保存的搜索 保存的搜索 ItemType 使用户能够检索先前创建的搜索；它们可以通过多种方式使用。 用户的保存搜索 执行搜索后，您可以将其保存并命名，以便将来可以检索和重复使用（有或无修改）。 每当您执行搜索时，搜索模式和搜索词会自动保存。当您返回某个项目时，上次搜索会自动恢复。 每个用户都可以执行自己创建的保存搜索，以及其他用户共享的搜索。 保存搜索 在任何模式下进行搜索。 点击「添加到收藏夹」',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [200/244] Search.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('86f0810eb3d4',
   N'搜索',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Search.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索功能提供执行搜索的各种选项。您可以使用菜单选项、工具栏按钮和搜索模式来执行简单或复杂搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索模式</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">简单搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高级搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">AML 搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">参数化搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存的搜索</Run></Paragraph>
</FlowDocument>',
   N'搜索 搜索功能提供执行搜索的各种选项。您可以使用菜单选项、工具栏按钮和搜索模式来执行简单或复杂搜索。 搜索菜单 搜索工具栏 搜索模式 简单搜索 高级搜索 AML 搜索 参数化搜索 保存的搜索',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [201/244] searching_poly_items.htm (2 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('f2e0ebfa4ea6',
   N'搜索多态项目',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">搜索多态项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: searching_poly_items.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索多态项目</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">多态项目可以是另一个项目的属性数据源（例如项目管理中的可交付项）。您可以通过在搜索中选择所有项来搜索多态项目，该选项会返回所有可能的多态来源的数据。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">或者，您可以从下拉列表中选择特定的多态来源，将搜索范围限定为仅该 ItemType 的数据。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d54fa9fb0d0e.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">例如，在创建新项目或现有项目的可交付项时，「可交付项」字段将自动搜索所有已配置的多态来源，除非另有指定。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9fdfd604df6d.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'搜索多态项目 多态项目可以是另一个项目的属性数据源（例如项目管理中的可交付项）。您可以通过在搜索中选择所有项来搜索多态项目，该选项会返回所有可能的多态来源的数据。 或者，您可以从下拉列表中选择特定的多态来源，将搜索范围限定为仅该 ItemType 的数据。 例如，在创建新项目或现有项目的可交付项时，「可交付项」字段将自动搜索所有已配置的多态来源，除非另有指定。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [202/244] searchmenu.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('64f1bcfab0b4',
   N'搜索选项',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">搜索选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: searchmenu.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索菜单提供各种选项来执行与搜索相关的任务。下表说明了搜索菜单中每个可用选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">执行搜索。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9bcf17c7b30c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">隐藏搜索条件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">隐藏或显示搜索条件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存当前搜索。有关更多信息，请参阅保存的搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">添加到收藏夹</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将搜索添加到收藏夹。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">参数化搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">启动参数化搜索。有关更多信息，请参阅参数化搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">按属性搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">按属性定义和查看搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高级搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用各种操作符和属性按高级条件搜索。有关更多信息，请参阅高级搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从主网格中删除选定的保存搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用通配符</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Allows usage of wildcards * an % in the search criteria. Usage of wildcards is enabled by default. If disabled, usage of wildcards is prohibited.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">追加结果</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Allows you to specify another search criteria and append results to the items already displayed in the main grid. For example, you can use append results to view Items with multiple but specific part numbers.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">跨页排序</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Return multiple pages sorted as per sequence defined in ItemType.</Run></Paragraph>
</FlowDocument>',
   N'搜索选项 搜索菜单提供各种选项来执行与搜索相关的任务。下表说明了搜索菜单中每个可用选项。 搜索选项 说明 执行搜索。 隐藏搜索条件 隐藏或显示搜索条件。 保存搜索 保存当前搜索。有关更多信息，请参阅保存的搜索。 添加到收藏夹 将搜索添加到收藏夹。 参数化搜索 启动参数化搜索。有关更多信息，请参阅参数化搜索。 按属性搜索 按属性定义和查看搜索。 高级搜索 使用各种操作符和属性按高级条件搜索。有关更多',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [203/244] searchtoolbar.htm (13 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('0b3f10162939',
   N'搜索工具栏',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">搜索工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: searchtoolbar.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以下是 ItemType 搜索工具栏的示例。每个搜索工具栏都包含以下描述的按钮。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">下表描述了搜索工具栏中的按钮。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/12a3cb5a9e1d.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">按钮名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">按钮</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">执行搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">执行搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">清除搜索条件</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9bcf17c7b30c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">清除已应用的搜索条件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">添加到收藏夹</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将搜索添加到收藏夹。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9502b80cff55.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">搜索模式下拉</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您选择搜索模式。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择搜索条件</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3474c06e6d3a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">使您能够指定多个条件来精细化搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存搜索以供将来使用。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d25ddade175f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">搜索模板</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">此选项仅当保存的搜索可用时才显示。选择一个选项以将应用的搜索条件设置到当前网格。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">启用选项卡红线视图。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/32ce6b8a0f4a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Provides an additional row to specify search conditions and criteria for Advanced Search. The Add Criteria button is available only for Advanced Search. For more information, refer to Advanced Search.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">每页结果数</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/544c311bc877.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Allows you to specify the number of rows to be displayed in one page. If left blank, it implies to show all at once.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">上一页</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d88de16eecd5.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">显示主网格中的上一页。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">下一页</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/67da8972ee0c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">显示主网格中的下一页。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示总页数和结果数</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e35f53cc348c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Displays the current page number as well as the total number of search results.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9eebcdaf2443.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">最大结果数</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/205de0f68c33.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Specifies the number of rows to be returned by the server. It is useful, if there is a very large number of Items to be returned after a search. For example, inserting 1000 restricts the search results to the first 1000 rows.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">生效类型下拉框</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">截至（日期）</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d764335a4471.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The Effectivity Type drop-down and Calendar are used only for versionable Items and provide further filtering on the search grid.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Current: Displays the latest (highest) generation of an Item that exists as of today. The calendar is disabled as the date is set to Today by default. The latest generation is flagged programmatically by is_current Item property.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Latest: Displays the latest (highest) generation of an Item that exists as of the date selected i n the calendar matching the search criteria provided . The modified_on date is used for the comparison. By default, the calendar is set to Today. The Latest may return a non-current generation of the Item; but, this would be a very unusual case.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Released: Displays the latest (highest) released generation of an Item that exists as of the date selected in the calendar. The latest released generation is based on the Released Date.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">. non-current generation of the ItemThe latest effective generation is based on the Effective Date and may return aWhen an Item is marked as Effective on a specific date, it means that it is put into use at that date. the date selected in the calendar. generation of an Item whose effective date is within latest (highest) Displays the: Effective</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The search displays the results as per the effectivity criteria only if you have required access rights to the generation of the Item you are looking for. Else, the search does not return the required result.</Run></Paragraph>
</FlowDocument>',
   N'搜索工具栏 以下是 ItemType 搜索工具栏的示例。每个搜索工具栏都包含以下描述的按钮。 下表描述了搜索工具栏中的按钮。 按钮名称 按钮 说明 执行搜索 执行搜索 清除搜索条件 清除已应用的搜索条件。 添加到收藏夹 将搜索添加到收藏夹。 搜索模式下拉 允许您选择搜索模式。 选择搜索条件 使您能够指定多个条件来精细化搜索。 保存搜索 保存搜索以供将来使用。 搜索模板 此选项仅当保存的搜索可用时才',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [204/244] Search_Menu_(Item_Window).htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('583a8e545f17',
   N'搜索菜单（项目窗口）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">搜索菜单（项目窗口）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Search_Menu_(Item_Window).htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索菜单（项目窗口）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索菜单提供在项目窗口中执行搜索相关任务的各种选项。下表说明了搜索菜单中每个可用选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">菜单选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">执行搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">执行搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">隐藏搜索条件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">隐藏或显示搜索条件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存当前搜索。有关更多信息，请参阅保存的搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">添加到收藏夹</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将搜索添加到收藏夹。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高级搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用各种操作符和属性按高级条件搜索。有关更多信息，请参阅高级搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存搜索...</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Allows you to save the current search criteria and retrieve previously created search criteria. For more information, refer to Saved Searches.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除保存的搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Deletes the selected saved search from the relationship grid.</Run></Paragraph>
</FlowDocument>',
   N'搜索菜单（项目窗口） 搜索菜单提供在项目窗口中执行搜索相关任务的各种选项。下表说明了搜索菜单中每个可用选项。 菜单选项 说明 执行搜索 执行搜索。 隐藏搜索条件 隐藏或显示搜索条件。 保存搜索 保存当前搜索。有关更多信息，请参阅保存的搜索。 添加到收藏夹 将搜索添加到收藏夹。 高级搜索 使用各种操作符和属性按高级条件搜索。有关更多信息，请参阅高级搜索。 Save Search... Allows ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [205/244] search_modes.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('f54d6fc6a6cd',
   N'搜索模式',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">搜索模式</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: search_modes.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索模式</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">搜索菜单和搜索工具栏中有四种搜索模式可供选择：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">隐藏搜索条件：选择此模式时，搜索栏不会出现。这在您只是浏览一个项目而不想查看搜索条件时很有用。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：隐藏搜索条件不会移除或删除之前的搜索条件。它只是将它们隐藏起来。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">简单搜索：此模式是最常用的搜索模式，也是最容易执行的搜索。在此模式下，每个属性列标题下方有一个搜索输入框。在此模式下，搜索列按「和」执行。有关更多信息，请参阅简单搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高级搜索：此模式允许您指定搜索操作符并添加搜索条件。有关更多信息，请参阅高级搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">AML 搜索：此模式使您能够编写自己的 AML 查询来执行搜索。有关更多信息，请参阅 AML 搜索。</Run></Paragraph>
</FlowDocument>',
   N'搜索模式 搜索菜单和搜索工具栏中有四种搜索模式可供选择： 隐藏搜索条件：选择此模式时，搜索栏不会出现。这在您只是浏览一个项目而不想查看搜索条件时很有用。 注意：隐藏搜索条件不会移除或删除之前的搜索条件。它只是将它们隐藏起来。 简单搜索：此模式是最常用的搜索模式，也是最容易执行的搜索。在此模式下，每个属性列标题下方有一个搜索输入框。在此模式下，搜索列按「和」执行。有关更多信息，请参阅简单搜索。 高级',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [206/244] Secure_Messages.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('26f194883ec0',
   N'安全社交功能',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">安全社交功能</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Secure_Messages.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">安全社交功能</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">安全社交界面提供了跨所有 Aras Innovator ItemType 的线程讨论功能。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目独立窗口的安全社交用户界面如下所示：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要了解有关项目独立窗口中安全社交界面的更多信息，请参阅以下主题：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/403741287823.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">侧边栏及其属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">讨论面板</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建快照评论</Run></Paragraph>
</FlowDocument>',
   N'安全社交功能 安全社交界面提供了跨所有 Aras Innovator ItemType 的线程讨论功能。 项目独立窗口的安全社交用户界面如下所示： 要了解有关项目独立窗口中安全社交界面的更多信息，请参阅以下主题： 侧边栏及其属性 讨论面板 创建快照评论',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [207/244] Setting_Required_for_Tex.htm (22 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('d823e80af86c',
   N'设置文本必填属性',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">设置文本必填属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Setting_Required_for_Tex.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设置文本必填属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当处理数据类型为 Text 的属性时，更改 Required 属性的值需要几个额外的步骤。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">情况：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">一个 ItemType 有一个数据类型为 Text 的属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">此属性的 Required 属性已设置为 False</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">此属性的 Required 属性需要设置为 True</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要将 Text 类型属性的 Required 属性从 False 更改为 True：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1. 在设计器中打开 ItemType 并选择属性选项卡。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2. 右键点击所需的 Text 属性并选择「编辑」以打开属性对话框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">3. 将 Required 属性从 False 更改为 True。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">4. 点击「保存」并关闭属性对话框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将文本属性设为必填</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">This is the first starting point described above, where we have an ItemType with a Text property which we want to make Required.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将「必填」更改为「是」：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Notify users that they must log out of Innovator by certain time when you (the Innovator Admin) will be making this change. The change requires a modification to the database, and no one can be logged in at that time. If users ignore the notification, they will be disconnected from the database while it is being modified, and will lose their work.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Determine the Item Type ID and Property Name. In this step we will find the necessary data about the ItemType and the Property in order to change the Property to “Required”.Open a text file to temporarily hold required informationLog into Innovator as an administrator.From TOC select ItemTypes.Search for the ItemType you wish to modify. In this example, we will use the ItemType TextPropertyTest.Right click on the Item Type and select Properties. You will see a dialog that displays the items ID at the bottom.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Open a text file to temporarily hold required information</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Log into Innovator as an administrator.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从目录中选择 ItemTypes。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Search for the ItemType you wish to modify. In this example, we will use the ItemType TextPropertyTest.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Right click on the Item Type and select Properties. You will see a dialog that displays the items ID at the bottom.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ce53a994c28a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click on the Copy button on the dialog.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Paste this value into the text file created above.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Open the ItemType and search for the property to be modified. In our case, we will use the property test_property.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/279d572d3ebc.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Copy the name (not the label!) of the property to the text file created above. Your temporary text file should look similar to:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0c857335639c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Disable the Database connection In this step we will disconnect the database, so that no changes could be made to the table while this change is being performed.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">If any users are still logged into Innovator, they will be disconnected and will lose their work.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Open the InnovatorServerConfig.xml file</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Find the line for the database you wish to update. It should look similar to: &lt;DB-Connection id=&quot;SolutionsS71&quot; database=&quot;SolutionsS71&quot; server=&quot;localhost&quot; uid=&quot;innovator&quot; pwd=&quot;innovator&quot; dbType=&quot;SQL Server&quot; /&gt;</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Change the line by adding an x in front of the DB-Connection, like this: &lt;xDB-Connection id=&quot;SolutionsS71&quot; database=&quot;SolutionsS71&quot; server=&quot;localhost&quot; uid=&quot;innovator&quot; pwd=&quot;innovator&quot; dbType=&quot;SQL Server&quot; /&gt;</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存文件并关闭</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Determine the Table name of the ItemType being updated. Usually the table name would be the name of the ItemType. However, it is not always the case, so it’s safer to go through this step and retrieve the table name through a query.Open Enterprise ManagerFind the Database for the Innovator to be updated. Please be certain that this is the same database that you disconnected earlier from the InnovatorServerConfig.xml file.Find the table called ItemTypeHighlight the table and select from the main menu: Tools, SQL Query AnalyzerInput the following query:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开企业管理器</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Find the Database for the Innovator to be updated. Please be certain that this is the same database that you disconnected earlier from the InnovatorServerConfig.xml file.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">找到名为 ItemType 的表</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Highlight the table and select from the main menu: Tools, SQL Query Analyzer</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">输入以下查询：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">SELECT INSTANCE_DATA FROM innovator.ITEMTYPE WHERE ID = &apos;YOUR ITEM ID&apos;</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Where Your Item ID should be replaced with the number you copied into the text file from Step 1. Make sure that you do not delete the single quotes around that number. Here is what it should look like:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/1164d61abdea.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Run the query by hitting the green arrow icon .</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c282647ea909.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The result of the query will appear in the lower pane of the SQL Query Analyzer.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f250c6764dd6.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">It will show a table name of the ItemType. Copy this value and paste it into the text file we have been using to store temporary data. Your text file should look similar to:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7a048393f4a6.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Do not close the SQL Query Analyzer. It will be used in the next step.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Make all null vales non-null. When switching from a not-required to a required property, null values of this property are no longer accepted. Therefore, if instances of this ItemType exist, where the value of this property is null, these values must be changed.From the SQL Query Analyzer, input the following query: UPDATE innovator.TABLE-NAME SET Property-Name = &apos;Please Update This Field Value&apos; WHERE Property-Name is NULLWhere:innovator refers to the owner of the tableTABLE-NAME should be replaced by the name of the table copied in Step 4Property-Name should be replace by the name of the Property to be made Required and copied in Step 1. When entered, your query should look similar to:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">From the SQL Query Analyzer, input the following query: UPDATE innovator.TABLE-NAME SET Property-Name = &apos;Please Update This Field Value&apos; WHERE Property-Name is NULL</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Where:innovator refers to the owner of the tableTABLE-NAME should be replaced by the name of the table copied in Step 4Property-Name should be replace by the name of the Property to be made Required and copied in Step 1. When entered, your query should look similar to:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">innovator refers to the owner of the table</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">TABLE-NAME should be replaced by the name of the table copied in Step 4</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Property-Name should be replace by the name of the Property to be made Required and copied in Step 1. When entered, your query should look similar to:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/dc3322babe0d.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Run the query by hitting the green arrow icon .</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c282647ea909.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Do not close the SQL Query Analyzer window, we will need it again.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Change the Property’s attribute in the database In the database table, each property has an attribute called Allows Nulls. Since we are changing the property to be Required, it no longer allows null values, and therefore its attribute needs to change as well.Open Enterprise Manager, or go to it if already open.Find the Database for the Innovator to be updated. Please be certain that this is the same database that you disconnected earlier from the InnovatorServerConfig.xml file.Find the table retrieved in Step 3; the name of the table should be in your temporary text file.Right click on the table and select Design Table.Uncheck the Allows Nulls field for the Property you have changed (see your text file for the name of this property).</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Open Enterprise Manager, or go to it if already open.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Find the Database for the Innovator to be updated. Please be certain that this is the same database that you disconnected earlier from the InnovatorServerConfig.xml file.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Find the table retrieved in Step 3; the name of the table should be in your temporary text file.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Right click on the table and select Design Table.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Uncheck the Allows Nulls field for the Property you have changed (see your text file for the name of this property).</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9b30af7645f1.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click on the Save icon and close the window.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Set Required = True for the Property of the ItemType. In the definition of your ItemType, for this particular property you had the property attribute Required set to False. Now, we can set it to true.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ee52ca9f6b67.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">From the SQL Query Analyzer, run following query UPDATE innovator.PROPERTY SET IS_REQUIRED = &apos;1&apos; WHERE (SOURCE_ID = &apos;ItemType ID&apos;) AND (NAME = &apos;Property-Name&apos;)</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">条件：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType ID should be replace by the ItemType ID from your text file</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Property-Name should be replaced by the name of the property, also from your text file</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/53147870d6b9.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Close the Query Analyzer window.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Enable Database connectionOpen the InnovatorServerConfig.xml fileFind the line for the database updated in Step 3 It should look similar to: &lt;xDB-Connection id=&quot;SolutionsS71&quot; database=&quot;SolutionsS71&quot; server=&quot;localhost&quot; uid=&quot;innovator&quot; pwd=&quot;innovator&quot; dbType=&quot;SQL Server&quot; /&gt;Change the line by removing the x in front of the DB-Connection It should now look similar to: &lt;DB-Connection id=&quot;SolutionsS71&quot; database=&quot;SolutionsS71&quot; server=&quot;localhost&quot; uid=&quot;innovator&quot; pwd=&quot;innovator&quot; dbType=&quot;SQL Server&quot; /&gt;Save the file and close</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Open the InnovatorServerConfig.xml file</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Find the line for the database updated in Step 3 It should look similar to: &lt;xDB-Connection id=&quot;SolutionsS71&quot; database=&quot;SolutionsS71&quot; server=&quot;localhost&quot; uid=&quot;innovator&quot; pwd=&quot;innovator&quot; dbType=&quot;SQL Server&quot; /&gt;</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Change the line by removing the x in front of the DB-Connection It should now look similar to: &lt;DB-Connection id=&quot;SolutionsS71&quot; database=&quot;SolutionsS71&quot; server=&quot;localhost&quot; uid=&quot;innovator&quot; pwd=&quot;innovator&quot; dbType=&quot;SQL Server&quot; /&gt;</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存文件并关闭</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Step 8 – Restart IISFrom the Windows Start MenuSelect Start, Settings, Control PanelOpen Administrative ToolsOpen ServicesFind the World Wide Web Publishing serviceRight click on World Wide Web Publishing serviceSelect Restart</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从 Windows 开始菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Select Start, Settings, Control Panel</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开管理工具</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开服务</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Find the World Wide Web Publishing service</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Right click on World Wide Web Publishing service</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择重新启动</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Making a Text Property Not Required</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将「必填」更改为「否」：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Notify users that they must log out of Innovator by certain time when you (the Innovator Admin) will be making this change. The change requires a modification to the database, and no one can be logged in at that time. If users ignore the notification, they will be disconnected from the database while it is being modified, and will lose their work.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Determine the Item Type ID and Property Name. In this step we will find the necessary data about the ItemType and the Property in order to change the Property to “Required”.Open a text file to temporarily hold required informationLog into Innovator as an administrator.From TOC select ItemTypes.Search for the ItemType you wish to modify. In this example, we will use the ItemType TextPropertyTest.Right click on the Item Type and select Properties. You will see a dialog that displays the items ID at the bottom.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Open a text file to temporarily hold required information</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Log into Innovator as an administrator.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从目录中选择 ItemTypes。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Search for the ItemType you wish to modify. In this example, we will use the ItemType TextPropertyTest.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Right click on the Item Type and select Properties. You will see a dialog that displays the items ID at the bottom.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ce53a994c28a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click on the Copy button on the dialog.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Paste this value into the text file created above.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Open the ItemType and search for the property to be modified. In our case, we will use the property test_property.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ebbd793bfe13.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Copy the name (not the label!) of the property to the text file created above. Your temporary text file should look similar to:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0c857335639c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Disable the Database connection In this step we will disconnect the database, so that no changes could be made to the table while this change is being performed.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">If any users are still logged into Innovator, they will be disconnected and will lose their work.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Open the InnovatorServerConfig.xml file</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Find the line for the database you wish to update. It should look similar to: &lt;DB-Connection id=&quot;SolutionsS71&quot; database=&quot;SolutionsS71&quot; server=&quot;localhost&quot; uid=&quot;innovator&quot; pwd=&quot;innovator&quot; dbType=&quot;SQL Server&quot; /&gt;</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Change the line by adding an x in front of the DB-Connection, like this: &lt;xDB-Connection id=&quot;SolutionsS71&quot; database=&quot;SolutionsS71&quot; server=&quot;localhost&quot; uid=&quot;innovator&quot; pwd=&quot;innovator&quot; dbType=&quot;SQL Server&quot; /&gt;</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存文件并关闭</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Determine the Table name of the ItemType being updated. Usually the table name would be the name of the ItemType. However, it is not always the case, so it’s safer to go through this step and retrieve the table name through a query.Open Enterprise ManagerFind the Database for the Innovator to be updated. Please be certain that this is the same database that you disconnected earlier from the InnovatorServerConfig.xml file.Find the table called ItemTypeHighlight the table and select from the main menu: Tools, SQL Query AnalyzerInput the following query: SELECT INSTANCE_DATA FROM innovator.ITEMTYPE WHERE ID = &apos;YOUR ITEM ID&apos;</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开企业管理器</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Find the Database for the Innovator to be updated. Please be certain that this is the same database that you disconnected earlier from the InnovatorServerConfig.xml file.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">找到名为 ItemType 的表</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Highlight the table and select from the main menu: Tools, SQL Query Analyzer</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Input the following query: SELECT INSTANCE_DATA FROM innovator.ITEMTYPE WHERE ID = &apos;YOUR ITEM ID&apos;</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">其中，</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Your Item ID should be replaced with the number you copied into the text file from Step 1. Make sure that you do not delete the single quotes around that number. Here is what it should look like:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/1164d61abdea.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Run the query by hitting the green arrow icon .</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c282647ea909.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The result of the query will appear in the lower pane of the SQL Query Analyzer.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f250c6764dd6.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">It will show a table name of the ItemType.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Copy this value and paste it into the text file we have been using to store temporary data. Your text file should look similar to:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7a048393f4a6.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Do not close the SQL Query Analyzer. It will be used in the next step.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Change the Property’s attribute in the database In the database table, each property has an attribute called Allows Nulls. Since we are changing the property to be Not Required, it can allow null values, and therefore its attribute needs to change as well.Open Enterprise Manager, or go to it if already open.Find the Database for the Innovator to be updated. Please be certain that this is the same database that you disconnected earlier from the InnovatorServerConfig.xml file.Find the table retrieved in Step 3; the name of the table should be in your temporary text file.Right click on the table and select Design Table.Check the Allows Nulls field for the Property you have changed (see your text file for the name of this property).</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Open Enterprise Manager, or go to it if already open.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Find the Database for the Innovator to be updated. Please be certain that this is the same database that you disconnected earlier from the InnovatorServerConfig.xml file.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Find the table retrieved in Step 3; the name of the table should be in your temporary text file.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Right click on the table and select Design Table.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Check the Allows Nulls field for the Property you have changed (see your text file for the name of this property).</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/eebb2c574524.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click on the Save icon and close the window.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Set Required = False for the Property of the ItemType. In the definition of your ItemType, for this particular property you had the property attribute Required set to True. Now, we can set it to False.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/8bd0ec12fb52.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">From the SQL Query Analyzer, run following query UPDATE innovator.PROPERTY SET IS_REQUIRED = &apos;0&apos; WHERE (SOURCE_ID = &apos;ItemType ID&apos;) AND (NAME = &apos;Property-Name&apos;)</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">条件：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ItemType ID should be replace by the ItemType ID from your text file</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Property-Name should be replaced by the name of the property, also from your text file</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f823885e30e9.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Close the Query Analyzer window.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Enable Database connectionOpen the InnovatorServerConfig.xml fileFind the line for the database updated in Step 3 It should look similar to: &lt;xDB-Connection id=&quot;SolutionsS71&quot; database=&quot;SolutionsS71&quot; server=&quot;localhost&quot; uid=&quot;innovator&quot; pwd=&quot;innovator&quot; dbType=&quot;SQL Server&quot; /&gt;Change the line by removing the x in front of the DB-Connection It should now look similar to: &lt;DB-Connection id=&quot;SolutionsS71&quot; database=&quot;SolutionsS71&quot; server=&quot;localhost&quot; uid=&quot;innovator&quot; pwd=&quot;innovator&quot; dbType=&quot;SQL Server&quot; /&gt;Save the file and close</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Open the InnovatorServerConfig.xml file</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Find the line for the database updated in Step 3 It should look similar to: &lt;xDB-Connection id=&quot;SolutionsS71&quot; database=&quot;SolutionsS71&quot; server=&quot;localhost&quot; uid=&quot;innovator&quot; pwd=&quot;innovator&quot; dbType=&quot;SQL Server&quot; /&gt;</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Change the line by removing the x in front of the DB-Connection It should now look similar to: &lt;DB-Connection id=&quot;SolutionsS71&quot; database=&quot;SolutionsS71&quot; server=&quot;localhost&quot; uid=&quot;innovator&quot; pwd=&quot;innovator&quot; dbType=&quot;SQL Server&quot; /&gt;</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存文件并关闭</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Restart IISFrom the Windows Start MenuSelect Start, Settings, Control PanelOpen Administrative ToolsOpen ServicesFind the World Wide Web Publishing serviceRight click on World Wide Web Publishing serviceSelect Restart</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从 Windows 开始菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Select Start, Settings, Control Panel</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开管理工具</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开服务</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Find the World Wide Web Publishing service</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Right click on World Wide Web Publishing service</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择重新启动</Run></Paragraph>
</FlowDocument>',
   N'设置文本必填属性 当处理数据类型为 Text 的属性时，更改 Required 属性的值需要几个额外的步骤。 情况： 一个 ItemType 有一个数据类型为 Text 的属性 此属性的 Required 属性已设置为 False 此属性的 Required 属性需要设置为 True 要将 Text 类型属性的 Required 属性从 False 更改为 True： 1. 在设计器中打开 Ite',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [208/244] Setting_up_working_directory.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('2d31048f83e1',
   N'设置工作目录',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">设置工作目录</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Setting_up_working_directory.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设置工作目录</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工作目录是本地客户端机器上存储签入和签出文件的位置。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当您登录 Aras Innovator 时，系统会提示您设置工作目录。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果管理员已正确配置了最终用户的工作目录，将出现以下对话框：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果管理员未配置工作目录，将出现以下对话框：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：首次登录时，此窗口将显示两次。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要设置或更改工作目录</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1. 点击浏览以选择您希望文件下载到的目录。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2. 点击确定。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/35d4481b434f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">注意：您可以在登录后通过选择 工具 &gt; 首选项 &gt; 更改工作目录 来更改工作目录。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Select the required location for the working directory on the client machine.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您可以点击「新建文件夹」在客户端机器上创建新文件夹。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击确定。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The specified location is the working directory on the current client machine.</Run></Paragraph>
</FlowDocument>',
   N'设置工作目录 工作目录是本地客户端机器上存储签入和签出文件的位置。 当您登录 Aras Innovator 时，系统会提示您设置工作目录。 如果管理员已正确配置了最终用户的工作目录，将出现以下对话框： 如果管理员未配置工作目录，将出现以下对话框： 注意：首次登录时，此窗口将显示两次。 要设置或更改工作目录 1. 点击浏览以选择您希望文件下载到的目录。 2. 点击确定。 注意：您可以在登录后通过选择',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [209/244] sharepoint.htm (7 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('a3017bd31685',
   N'Innovator 与 SharePoint 集成',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">Innovator 与 SharePoint 集成</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: sharepoint.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Innovator 与 SharePoint 集成</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">简介</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/264f23f68aac.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Innovator 提供了使用 Web 服务直接访问 MS Sharepoint 文档的功能。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Innovator 遵循 SharePoint「库」的隐喻。SharePoint 库类似于 Innovator 的 ItemType。一个 Innovator ItemType 可以映射到一个 SharePoint 库。一个 Innovator Item 实例映射到该库中的一个 SharePoint 文档。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在此帮助文件中我们讨论：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在 Innovator 中定义 SharePoint 库</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建和存储 SharePoint 文档</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将 Innovator ItemType 关联到 SharePoint 库</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用 SharePoint 文档</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Using SharePoint Document Objects in Innovator</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Creating a Sharepoint Library Connection</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0de2b1372a48.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The first step is to link a SharePoint Library to Innovator. Each Innovator/Sharepoint connection is configured using a special Innovator item which resides under the Administration TOC folder &quot;SharePoint Libraries&quot;. As mentioned above, Sharepoint organizes its documents into Document Libraries. This is the same level used by Innovator to establish connections.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Select “New SharePoint Library Definition” from the menu. The various fields are discussed below.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">SharePoint Information and Credentials</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5791549a22d2.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">SharePoint Site URL - URL location of your Sharepoint server (e.g. http://yourdomain/Sharepoint )</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">SharePoint User - the account that you have designated for this library connection</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The SharePoint User serves as an agent for the Innovator connection. The Innovator administrator may choose from only those libraries that this account is authorized to access (see Library Selection below)</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">SharePoint 用户密码 - 上述用户的密码</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">SharePoint User Domain - SharePoint may require that the user’s domain is provided</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">库选择</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/231a6b97e013.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The &quot;Refresh List&quot; button queries for all accessible libraries for the SP user specified (above) and populates the pull-down menu just to its right.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Sharepoint Document Library Name - the Library that contains SharePoint documents available</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ID (read only, system generated)</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">If the specified SharePoint user (above) only has rights to the Engineering and Manufacturing Libraries, then these will be the only two available in the pulldown for this configuration. This is entirely up to your SharePoint implementation and Innovator will suggest connections accordingly.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Automatic Generation of ‘Reference’ ItemType</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Once you add a SharePoint Library connection, Innovator will create a ‘reference’ ItemType to represent the documents in that library. This itemtype acts as a reference to the actual document in SharePoint so that the “Download”, “Upload” and basic search grid functions are available in Innovator.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e20e83c68ed2.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The name and label information for this itemtype is entered in the next three fields on the SharePoint Library Definition form.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Generated Document Type Information</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e27ebab44a3f.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Name of Generated Document Type - Name of Generated Doc Item (e.g. “Test SP doc”)</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Label of generated item - label for this new itemtype (e.g. “Test SP doc”)</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Plural label - plural label for itemtype (e.g. “Test SP docs”)</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">This Innovator itemtype is automatically generated once saved. It will be named according to your inputs. This itemtype is used by Innovator to represent each document found in the SharePoint Library you connect to with this SharePoint Library Definition.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">You are now ready to check the connection!</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">测试连接</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Now you will be able to go to the Innovator TOC and open the SPDocuments Folder. Within it should be the Generated Document Items you specified above, however Innovator will populate the grid with all documents found in the Library you’re connected to for that reference Document ItemType.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/cf8bcd882738.png" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">If you do not see the SPDocuments Folder in the TOC, it’s possible that you do not have permission to Discover or Get the item. Check the itemtype definition as you would for any other Innovator itemtype.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">If you need additional customization to meet your specific business goals, Innovator’s open framework can support further refinements.</Run></Paragraph>
</FlowDocument>',
   N'Innovator 与 SharePoint 集成 简介 Innovator 提供了使用 Web 服务直接访问 MS Sharepoint 文档的功能。 Innovator 遵循 SharePoint「库」的隐喻。SharePoint 库类似于 Innovator 的 ItemType。一个 Innovator ItemType 可以映射到一个 SharePoint 库。一个 Innovator ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [210/244] Sidebar_and_its_Properties.htm (4 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('3f5378e172d1',
   N'侧边栏及其属性',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">侧边栏及其属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Sidebar_and_its_Properties.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">侧边栏及其属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当 ItemType 启用了安全社交功能时，该项目的独立窗口将显示一个侧边栏。所有与安全社交和可视化协作相关的控件都放在侧边栏中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">侧边栏的功能包括：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">讨论按钮 - 一个切换控件，用于显示/隐藏讨论面板。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c2eb03a00858.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">文件查看器 - 显示所选文件内容的查看区域。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">查看器工具栏 - 提供在查看器中导航、打印和创建批注的工具。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">快照/批注创建按钮 - 使用户能够创建和编辑评论的按钮。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">File Viewer Tabs - When the item has referenced files, the sidebar shows tabs for the appropriate files. Clicking on any of these tabs displays the selected file in the item window. Only one can file can be displayed at a time. When there is more than one file type available, clicking on the tab will open a drop-down list of all the available files of that type. The user can scroll down to select the desired file and open it in the viewer.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Item Keyed Name - The item&apos;s keyed name is displayed at the bottom of the sidebar.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">All Tabs - Provides the user with a drop down list of all the associated files from which the user selects the desired file or form to open it in the tear-off window.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">There are three kinds of File Viewer Tabs displayed on the sidebar:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">活动</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">非活动</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">指示</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/31840956dd4f.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">指示一个或多个要使用3D查看器查看的文件。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f5e119172e0f.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">指示一个或多个要使用 PDF 查看器查看的文件。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/af72273163c0.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">指示一个或多个要使用图像查看器查看的文件。</Run></Paragraph>
</FlowDocument>',
   N'侧边栏及其属性 当 ItemType 启用了安全社交功能时，该项目的独立窗口将显示一个侧边栏。所有与安全社交和可视化协作相关的控件都放在侧边栏中。 侧边栏的功能包括： 讨论按钮 - 一个切换控件，用于显示/隐藏讨论面板。 文件查看器 - 显示所选文件内容的查看区域。 查看器工具栏 - 提供在查看器中导航、打印和创建批注的工具。 快照/批注创建按钮 - 使用户能够创建和编辑评论的按钮。 File V',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [211/244] Simple_search.htm (5 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('aca552093c58',
   N'简单搜索',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">简单搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Simple_search.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">简单搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">简单搜索是最简单且最常用的搜索。默认情况下，搜索栏以简单搜索模式显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">简单搜索的技巧</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0ee7967ac578.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">使用 * 或 % 作为通配符。例如，名称列中的术语 &apos;a*b&apos; 表示名称以 a 开头、以 b 结尾的项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">单独使用 * 以选择非空值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在列标题下的搜索字段中输入搜索值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击执行搜索按钮以启动搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">主网格将列出与搜索条件匹配的项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可以通过将搜索值放在多列中来组合搜索。在简单搜索模式下，列搜索按「与」操作。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Use @{n} (where n is a number) to create Parameterized Searches.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Use 0 or 1 for columns with checkboxes, 0 means not-checked.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Search for Items by their keyed-name. For example, ‘Adm*’ finds the Identity named Administrators.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click in the left-most column in the blue search bar to search by claim status.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/bb7432e237ab.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3701f61868c3.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click the button on the Search toolbar or menu to execute the search.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9bcf17c7b30c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">示例：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Enter ‘RC*’ in the Name column of the blue search bar to display Items with part numbers beginning with the letters &quot;RC&quot;.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0155fd64a38b.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">搜索日期</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Simple search mode now supports operators for searching on Date properties including &gt;, &gt;=, &lt;, &lt;=, and (range). Search options are limited to the following where {Date} represents the standard Short Date syntax:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">{Date} returns results where the date value exactly matches the search value.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">&gt;={Date} returns results where the date is after (or equal to) the specified date.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">&lt;={Date} returns results where the date is before (or equal to) the specified date.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">{Date}...{Date} returns results where the value is between the two values (inclusive).</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Note: {Date} text can either be entered manually or by using the Date dialog.</Run></Paragraph>
</FlowDocument>',
   N'简单搜索 简单搜索是最简单且最常用的搜索。默认情况下，搜索栏以简单搜索模式显示。 简单搜索的技巧 使用 * 或 % 作为通配符。例如，名称列中的术语 ''a*b'' 表示名称以 a 开头、以 b 结尾的项目。 单独使用 * 以选择非空值。 在列标题下的搜索字段中输入搜索值。 点击执行搜索按钮以启动搜索。 主网格将列出与搜索条件匹配的项目。 可以通过将搜索值放在多列中来组合搜索。在简单搜索模式下，列搜索',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [212/244] Source_Item_-_Related_Item_Relationship.htm (2 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('cf211270d835',
   N'父子关系',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">父子关系</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Source_Item_-_Related_Item_Relationship.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">父子关系</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">下图突出显示了源（父级）- 关系 - 关联（子级）项目的各个部分：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系工具栏：关系工具栏包括在关系网格上执行各种任务的各种按钮。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/eca8438c7994.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">关系网格：关系网格列出了关联（子级）项目。网格列包括关联项目的属性以及关系本身的属性。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">例如，在此图中，序列、数量、引用设计等属性来自关系，而零部件编号、名称、修订等属性来自关联项目。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7ac6527365f8.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">For example, in this figure, properties such as Sequence, Quantity, Reference Designator are highlighted and hence belong to the Relationship Item; whereas, other properties such as Part Number, Revision, Name, Type, State, Unit, and others belong to the related (child) Item.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Note: The properties for each Item depend on each ItemType and are configured by the administrator. Thus, depending on the configuration done by the administrator, the properties for each ItemType could differ.</Run></Paragraph>
</FlowDocument>',
   N'父子关系 下图突出显示了源（父级）- 关系 - 关联（子级）项目的各个部分： 关系工具栏：关系工具栏包括在关系网格上执行各种任务的各种按钮。 关系网格：关系网格列出了关联（子级）项目。网格列包括关联项目的属性以及关系本身的属性。 例如，在此图中，序列、数量、引用设计等属性来自关系，而零部件编号、名称、修订等属性来自关联项目。 For example, in this figure, propert',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [213/244] Special_Identities.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('00dd86f15d9b',
   N'特殊身份',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">特殊身份</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Special_Identities.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">特殊身份</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Innovator 中保留了多个特殊身份用于特定目的。下表列出了这些特殊身份：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">身份名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">管理员</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">管理员身份。此组中的成员具有对所有 ItemType 的完全访问权限。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">全局</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">每个用户自动属于的组身份。新用户自动添加到 World。此组通常用于设置基本的默认权限。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可添加</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">控制用户是否可以创建新项目实例的特殊身份。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可发现</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">控制用户是否可以看到项目是否存在（但不一定能查看其详细信息）的特殊身份。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Aras PLM</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Internal identity for Aras PLM</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">认证管理员</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Grants rights to authenticated users as well as checking user account permissions to verify that these users have access to the appropriate resources</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">认证管理员组</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Identity for calling authentication methods</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">认证器</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">A means of verifying a user&apos;s identity</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">变更控制委员会（CCB）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Members of this identity decide whether or not proposed changes to a software project should be implemented.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">变更专家 I</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Members of this Identity have control over various aspects of the workflow of PRs, ECRs, and ECNs in the Product Engineering Solution</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">变更专家 II</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Members of this Identity have control over some aspects of the workflow of the ECNs in the Product Engineering Solution</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">变更专家 III</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Members of this Identity have control over the auditing actions in the workflow of the ECNs in the Product Engineering Solution</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">分类管理员</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">CM</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Configuration Management group, includes the Change Specialist I, II, and III identities</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">零部件工程</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Members of this identity have control over and access to Product Engineering items, such as Parts, Vendors, Manufacturers, Manufacturer Parts, etc.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">转换管理员</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">CRB</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Change Review Board - members of this Identity have control over different aspects of the workflow of the ECNs</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建者</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The identity that created the item</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">管理员</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Members of this identity have access to various Project Management items, such as the Project itself and its components - activities and WBS elements.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">所有者</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">This identity is used by the Product Engineering items to specify an Assigned Creator, or the identity responsible for the technical content and review of the item</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">流程规划管理员</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">质量规划管理员</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Members of this identity have full access to various Quality Planning Items, such as DFMEAs, Process Planner, Quality Planning Library, etc.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">质量规划</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Members of this identity have specific limited access to various Quality Planning items, such as DFMEAs, Process Planner, Quality Planning Library, etc.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">全局</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">All users are automatically added to the World identity</Run></Paragraph>
</FlowDocument>',
   N'特殊身份 Innovator 中保留了多个特殊身份用于特定目的。下表列出了这些特殊身份： 身份名称 说明 Administrators 管理员身份。此组中的成员具有对所有 ItemType 的完全访问权限。 World 每个用户自动属于的组身份。新用户自动添加到 World。此组通常用于设置基本的默认权限。 Can Add 控制用户是否可以创建新项目实例的特殊身份。 可发现 控制用户是否可以看到项',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [214/244] SSVC_in_Item_tear-off_windows.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('eddb79f7de09',
   N'项目窗口的安全社交可视化协作',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">项目窗口的安全社交可视化协作</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: SSVC_in_Item_tear-off_windows.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目窗口的安全社交可视化协作</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">安全社交和可视化协作（SSVC）的目的是为所有 Aras Innovator 项目提供易于使用的社交讨论和可视化协作功能。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">安全社交提供了一个强大、可配置的机制来聚合跨多个源的社交内容和文件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">例如，零部件可以包含来自相关文档的讨论内容和文件。一个项目的信息可以聚合来自其他相关项目的讨论内容。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">安全社交和可视化协作的功能可以从两个方面描述：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">社交讨论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可视化协作</Run></Paragraph>
</FlowDocument>',
   N'项目窗口的安全社交可视化协作 安全社交和可视化协作（SSVC）的目的是为所有 Aras Innovator 项目提供易于使用的社交讨论和可视化协作功能。 安全社交提供了一个强大、可配置的机制来聚合跨多个源的社交内容和文件。 例如，零部件可以包含来自相关文档的讨论内容和文件。一个项目的信息可以聚合来自其他相关项目的讨论内容。 安全社交和可视化协作的功能可以从两个方面描述： 社交讨论 可视化协作',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [215/244] Standard_Work_Procedure_for_Methods.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('50b44e07e4ae',
   N'方法的标准工作程序',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">方法的标准工作程序</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Standard_Work_Procedure_for_Methods.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">方法的标准工作程序</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">收集定义您将要处理的项目的文档</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Item/关系图（带有 ItemType 名称）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">每个 Item 的属性名称和数据类型列表</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">制定计划</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">客户端还是服务器</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如何调用</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">输入内容</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">期望的结果</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">编写方法</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">编写代码来完成计划。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从 ItemType 或表单中测试方法</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在 Innovator 客户端中通过菜单选项（操作）或表单事件调用方法。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">验证结果</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">请记住，代码的不同部分会产生不同的错误。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在 Innovator 中遇到问题时要检查的事情：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">检查方法代码（可能需要条件处理）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">检查项目或属性名称（区分大小写）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">检查方法类型（TSE/ASE）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">检查「适用于」字段是否正确关联</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">检查项目级事件的 Item Action（onBeforeAdd、onAfterUpdate 等）</Run></Paragraph>
</FlowDocument>',
   N'方法的标准工作程序 收集定义您将要处理的项目的文档 Item/关系图（带有 ItemType 名称） 每个 Item 的属性名称和数据类型列表 制定计划 客户端还是服务器 如何调用 输入内容 期望的结果 编写方法 编写代码来完成计划。 从 ItemType 或表单中测试方法 在 Innovator 客户端中通过菜单选项（操作）或表单事件调用方法。 验证结果 请记住，代码的不同部分会产生不同的错误。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [216/244] Structure_browser.htm (21 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('4fa1955f987c',
   N'结构浏览器',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">结构浏览器</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Structure_browser.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">结构浏览器</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">每个 Innovator ItemType 都可以包含一个或多个关系实例。结构浏览器提供了一个树形视图，用于查看 ItemType 之间的关系层次结构。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您可以通过右键点击主网格中的项目并从上下文菜单中选择「结构浏览器」来访问结构浏览器。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">结构浏览器窗口以单行显示所选项目及其 ItemType 名称。其所有关联项目显示在下一级层次结构中。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/dff7c47cf117.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">下表描述了结构浏览器工具栏中的图标。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/de7d6b1f8d36.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">图标</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">展开全部</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">展开树中所有节点。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">全部折叠</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">折叠树中所有节点。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d1270e5bc632.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在结构中搜索特定项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打印</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/95ae70e8279c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">打印结构视图。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关闭</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关闭结构浏览器。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/2c7cae80ac0c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Opens a search dialog with all prior generations of the Item is displayed. On selection, the two different versions of the same Item are displayed side with expanded structure. For example, a Part item can be compared to another version of the same Part.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Side-by-Side with other ItemType</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9496cc720430.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Opens a search dialog for selection of another Item that belongs to the same ItemType. On selection, the two different Items of the same ItemType are displayed side with expanded structure. For example, you can compare two different Part Items. A comparison of this type compares all relationships existing on each Item.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高亮</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a41d3fe65d5c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Expands entire structure and displays the differences between Items being compared using red, blue, and black colors.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">In the examples, we use a simple Bill of Material (BOM) to show the simplicity of the Structure Browser.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Opening an Item in a Structure Browser</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">登录 Aras Innovator。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">From the TOC, navigate to the Item.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Let us consider a Part ItemType. Navigate to Design--&gt; Parts.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">From the main grid, select the Item and use the right mouse button (right-click) to open the context menu.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择结构浏览器。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The Structure Browser window appears.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click to view relationships of the Part Item.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/618f8b87000a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ba2749ca5761.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ba2749ca5761.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Performing a Side-by-Side comparison with a different Item of the same Type</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Open an ItemType in a Structure Browser. For detailed steps, refer to To open an Item in a Structure Browser.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">In the Structure Browser window,click on the toolbar to search a Part for relationship comparison.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9496cc720430.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">In this example, we only compare the Part-BOM relationships, other relationships have been removed for clarity. A Search dialog box appears.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/01677376bc20.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Use the search toolbar to search for the required Part Item and then select the required Part Item from the grid.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click and select Structure Browser from the menu.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/59eb9549a5fc.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/291e126fe82b.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Each side of the Structure Browser is fully expandable.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Highlighting the differences in Side-by-Side comparison for Different Items (same ItemType)</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Open two different Items of the same Type in the Structure Browser window for side-by-side comparison. For detailed steps, refer to To do a Side-by-Side comparison with a different Item of same Type.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click on the Structure Browser toolbar to view color- coded differences between the Items being compared.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a41d3fe65d5c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ad88d1d81b2f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">A description of the Highlight color legend as shown in this example follows.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">BLACK – Indicates no difference between Items in structure or version/generation. Part-BOM relationship to the related Part Item BR-0301 is found to match in structure and version/generation.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Root Item types always appear black since the difference is known.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">RED – Indicates a difference in generation between two items of matching base number. The Part-BOM relationship between related Part Items BR-0101 is found to match in structure and base number, but generation differs between the two.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">BLUE – Indicates no Item found with matching structure. Part-BOM relationship to related Part Items California and BR-0201 are not found under BRAKE-02.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Close the Structure Browser window.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Performing a Side-by-Side comparison with another version of the same Item</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Open an ItemType and select Structure Browser from the menu. For detailed steps, refer to To open an Item in a Structure Browser.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">In the Structure Browser window,click on the toolbar to view the different versions of the selected Part Item. In this example, we only compare the Part-BOM relationships, other relationships have been removed for clarity. A Search dialog box is displayed that lists all versions of the Part Item to choose from.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5e4b4051caa7.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/40c0c5685a57.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click twice to select a version. In this example, version A.7 and Version A.2 are selected.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ff8bd349d8a0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Highlighting differences of the Side-by-Side comparison of different versions of the same Item</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Open two versions of the same Item in the Structure Browser window for side-by-side comparison. For detailed steps, refer to To do a Side-by-Side compare with another version of ItemType.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click on the Structure Browser toolbar to view color- coded differences between the Items being compared.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a41d3fe65d5c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0cdc031a3faa.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The Item version highlight comparison is confirming Part Item California was added to BRAKE-01 Revision A Version 3, but also that part item BR-0101 had a version change. This version change is more readily apparent when a highlight is run on a comparison. A brief description of the color legend as shown in this example follows.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">BLACK – Indicates no difference between items in structure or version/generation. Part BOM relationship to related part item BR-0301 is found to match in structure and version/generation.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Root Items always appear black since the difference is known.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">RED – Indicates a difference in generation between two items with matching structure. Part BOM relationship to related part item BR-0101 is found to match in structure, but generation differs between the two.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">BLUE – Indicates no Item found with matching structure. Part-BOM relationship to related Part Items California and BR-0201 are not found under BRAKE-01 -A.2.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Close the Structure Browser window.</Run></Paragraph>
</FlowDocument>',
   N'结构浏览器 每个 Innovator ItemType 都可以包含一个或多个关系实例。结构浏览器提供了一个树形视图，用于查看 ItemType 之间的关系层次结构。 您可以通过右键点击主网格中的项目并从上下文菜单中选择「结构浏览器」来访问结构浏览器。 结构浏览器窗口以单行显示所选项目及其 ItemType 名称。其所有关联项目显示在下一级层次结构中。 下表描述了结构浏览器工具栏中的图标。 图标 说',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [217/244] Structure_browser.htm (20 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('a0f187622046',
   N'结构浏览器（续）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">结构浏览器（续）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Structure_browser.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">结构浏览器</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">每个 Innovator ItemType 都可以包含一个或多个关系实例。结构浏览器提供了一个树形视图，用于查看 ItemType 之间的关系层次结构。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您可以通过右键点击主网格中的项目并从上下文菜单中选择「结构浏览器」来访问结构浏览器。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">结构浏览器窗口以单行显示所选项目及其 ItemType 名称。其所有关联项目显示在下一级层次结构中。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/847d7629b680.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">下表描述了结构浏览器工具栏中的图标。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/de7d6b1f8d36.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">图标</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">展开全部</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">展开树中所有节点。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">全部折叠</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">折叠树中所有节点。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d1270e5bc632.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在结构中搜索特定项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打印</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/95ae70e8279c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">打印结构视图。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关闭</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关闭结构浏览器。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/2c7cae80ac0c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Opens a search dialog with all prior generations of the Item is displayed. On selection, the two different versions of the same Item are displayed side with expanded structure. For example, a Part item can be compared to another version of the same Part.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Side-by-Side with other ItemType</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3f4004bade73.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Opens a search dialog for selection of another Item that belongs to the same ItemType. On selection, the two different Items of the same ItemType are displayed side with expanded structure. For example, you can compare two different Part Items. A comparison of this type compares all relationships existing on each Item.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">高亮</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a41d3fe65d5c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Expands entire structure and displays the differences between Items being compared using red, blue, and black colors.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">In the examples, we use a simple Bill of Material (BOM) to show the simplicity of the Structure Browser.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Opening an Item in a Structure Browser</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">登录 Aras Innovator。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">From the TOC, navigate to the Item.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Let us consider a Part ItemType. Navigate to Design--&gt; Parts.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">From the main grid, select the Item and use the right mouse button (right-click) to open the context menu.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择结构浏览器。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The Structure Browser window appears.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click to view relationships of the Part Item.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/618f8b87000a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6b11c5c3f963.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Performing a Side-by-Side comparison with a different Item of the same Type</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Open an Item in the Structure Browser. For detailed steps, refer to To open an Item in a Structure Browser.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">In the Structure Browser window,click on the toolbar to search a Part for relationship comparison. The Select Items dialog box appears.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/435468c42c3c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b55412b67cb2.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Select the item and Click to complete the selection.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ad42022d1a90.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/291e126fe82b.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Each side of the Structure Browser is fully expandable.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Highlighting the differences in Side-by-Side comparison for Different Items (same ItemType)</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Open two different Items of the same Type in the Structure Browser window for side-by-side comparison. For detailed steps, refer to To do a Side-by-Side comparison with a different Item of same Type.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click on the Structure Browser toolbar to view color- coded differences between the Items being compared.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a41d3fe65d5c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3cbee0278184.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">A description of the Highlight color legend as shown in this example follows.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">BLACK – Indicates no difference between Items in structure or version/generation. Part-BOM relationship to the related Part Item BR-0301 is found to match in structure and version/generation.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Root Item types always appear black since the difference is known.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">RED – Indicates a difference in generation between two items with matching base numbers. The relationship between related Part Items Simple Engine 2.0 - A.2 and Simple Engine 2.0 - A.1 match in structure and base number, but the generation differs between the two.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">BLUE – Indicates no Item found with matching structure. Part - Hybrid Engine - A.1 and Part - Simple Engine 2.0 - A.2 do not have matching structures.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Close the Structure Browser window.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Performing a Side-by-Side comparison with another version of the same Item</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Open an Item in a Structure Browser. For detailed steps, refer to To open an Item in a Structure Browser.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">In the Structure Browser window,click on the toolbar to view the different versions of the selected Part Item.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5e4b4051caa7.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">In this example, we only compare the Part-BOM relationships, other relationships have been removed for clarity.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">A Search dialog box is displayed that lists all versions of the Part Item to choose from.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9b3845573b7b.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click twice to select a version. In this example, version A.5 is selected.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/93fbe10ca766.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Highlighting differences of the Side-by-Side comparison of different versions of the same Item</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Open two versions of the same Item in the Structure Browser window for side-by-side comparison. For detailed steps, refer to To do a Side-by-Side compare with another version of ItemType.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click on the Structure Browser toolbar to view color- coded differences between the Items being compared.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a41d3fe65d5c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a11e89fa5b21.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The Item version highlight comparison is confirming Part Item N-10012 - A.1 was added to SEDAN - A.7, but also that part item Part - C-0001 - A.1 had a version change. This version change is more readily apparent when a highlight is run on a comparison. A brief description of the color legend as shown in this example follows.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">BLACK – Indicates no difference between items in structure or version/generation. Part BOM relationship to related part item BR-0301 is found to match in structure and version/generation.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Root Items always appear black since the difference is known.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">RED – Indicates a difference in generation between two items with matching structure. Part BOM relationship to related part item BR-0101 is found to match in structure, but generation differs between the two.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">BLUE – Indicates no Item found with matching structure. Part-BOM relationship to related Part Items California and BR-0201 are not found under BRAKE-01 -A.2.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Close the Structure Browser window.</Run></Paragraph>
</FlowDocument>',
   N'结构浏览器 每个 Innovator ItemType 都可以包含一个或多个关系实例。结构浏览器提供了一个树形视图，用于查看 ItemType 之间的关系层次结构。 您可以通过右键点击主网格中的项目并从上下文菜单中选择「结构浏览器」来访问结构浏览器。 结构浏览器窗口以单行显示所选项目及其 ItemType 名称。其所有关联项目显示在下一级层次结构中。 下表描述了结构浏览器工具栏中的图标。 图标 说',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [218/244] team_identity.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('70da6bf30a7d',
   N'团队身份',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">团队身份</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: team_identity.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">团队身份</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Innovator 提供了定义逻辑用户组（即「团队」）的功能，这些团队可以分配给工作流活动和生命周期状态转换。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">团队身份图</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在下图中，团队项目有三个成员（用户1、用户2和用户3）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当团队被分配到工作流活动时，所有三个用户都会收到活动的通知。当其中一个用户认领该活动时，其他用户将不再看到该活动。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c6bca8b47ceb.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">团队身份在权限和生命周期提升中的使用方式与常规组身份类似。</Run></Paragraph>
</FlowDocument>',
   N'团队身份 Innovator 提供了定义逻辑用户组（即「团队」）的功能，这些团队可以分配给工作流活动和生命周期状态转换。 团队身份图 在下图中，团队项目有三个成员（用户1、用户2和用户3）。 当团队被分配到工作流活动时，所有三个用户都会收到活动的通知。当其中一个用户认领该活动时，其他用户将不再看到该活动。 团队身份在权限和生命周期提升中的使用方式与常规组身份类似。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [219/244] Tools_menu.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('1833c014a766',
   N'工具菜单',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">工具菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Tools_menu.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工具菜单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工具菜单包含管理操作以及用户首选项设置。下表说明了工具菜单中每个可用选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">菜单选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">通配符</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">允许您在搜索中启用或禁用通配符。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">重置用户首选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将用户首选项重置为默认值。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">更改密码</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">更改登录密码。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">更改电子签名</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">更改电子签名。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">讨论</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">开启或关闭讨论功能。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">清除我的搜索</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">清除之前保存的搜索。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">更改工作目录</Run></Paragraph>
</FlowDocument>',
   N'工具菜单 工具菜单包含管理操作以及用户首选项设置。下表说明了工具菜单中每个可用选项。 菜单选项 说明 通配符 允许您在搜索中启用或禁用通配符。 重置用户首选项 将用户首选项重置为默认值。 更改密码 更改登录密码。 更改电子签名 更改电子签名。 讨论 开启或关闭讨论功能。 清除我的搜索 清除之前保存的搜索。 更改工作目录',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [220/244] To_Copy.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('8611e0cd82a9',
   N'复制操作',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">复制操作</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: To_Copy.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">复制操作</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">复制过程使用 Innovator 对象模型（IOM）来获取和显示复制到剪贴板的所选项目的数据。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">源 ID</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">源类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">源键名</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">复制功能的上下文菜单选项：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">复制行</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从主网格或关系网格复制选定的行。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">复制 ID</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将项目的唯一 ID 复制到剪贴板。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">复制链接</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将项目链接复制到剪贴板，可用于在新窗口中打开项目。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a31e2895b912.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'复制操作 复制过程使用 Innovator 对象模型（IOM）来获取和显示复制到剪贴板的所选项目的数据。 源 ID 源类型 源键名 复制功能的上下文菜单选项： 复制行 从主网格或关系网格复制选定的行。 复制 ID 将项目的唯一 ID 复制到剪贴板。 复制链接 将项目链接复制到剪贴板，可用于在新窗口中打开项目。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [221/244] To_Paste.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('be227b1a2e4a',
   N'粘贴操作',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">粘贴操作</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: To_Paste.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">粘贴操作</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">粘贴功能是客户端操作，用于将新的关系项目和数据添加到目标网格。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：数据库不会自动更新。有效的更新通过保存目标项目来实现。从主网格粘贴</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">右键点击网格并选择 {选项卡名称} &gt; 粘贴行。最后复制的数据将作为新行添加到网格中。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从关系网格粘贴</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开目标项目的关系选项卡。右键点击关系网格并选择「粘贴行」。被复制项目的关系和属性将作为新行添加到网格中。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d2384eb7de30.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">注意：粘贴功能会创建所选项目的副本，而不是引用。如果原始项目被修改，粘贴的项目不会受到影响。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Note: The node list is not considered compatible with any of the relationships in the node list that meet one of the following criteria: - The source and target share the same Relationships. - The source and the target do not share the same RelationshipType, but do share the same related types AND the related type of the target is !=NULL. A confirmation is displayed to the user.</Run></Paragraph>
</FlowDocument>',
   N'粘贴操作 粘贴功能是客户端操作，用于将新的关系项目和数据添加到目标网格。 注意：数据库不会自动更新。有效的更新通过保存目标项目来实现。从主网格粘贴 右键点击网格并选择 {选项卡名称} > 粘贴行。最后复制的数据将作为新行添加到网格中。 从关系网格粘贴 打开目标项目的关系选项卡。右键点击关系网格并选择「粘贴行」。被复制项目的关系和属性将作为新行添加到网格中。 注意：粘贴功能会创建所选项目的副本，而不',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [222/244] user_notifications.htm (2 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('94e14dd649d9',
   N'用户通知消息',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">用户通知消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: user_notifications.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户通知消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">管理员和其他授权人员可以创建弹出式和标准（非弹出式）通知消息，以便在用户登录 Innovator 时显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建用户通知消息：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/000c9139d725.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">前提条件：如果尚未创建，请先创建 ItemType 及其类结构。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在目录中，打开 管理 &gt; 通知消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击「创建新通知消息」。将出现通知消息表单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">填写以下字段：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">名称 - 通知消息的名称。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息 - 消息正文。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">登录弹出消息 - 如果希望消息在用户登录时以弹出窗口显示，请选中此框。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有效开始日期 - 消息的开始日期。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有效结束日期 - 消息的结束日期。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">点击保存。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Target – the identity to whom the message will be delivered.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Expiration Date – the date/time the message will no longer be displayed.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Type – Standard or Popup. Standard messages are displayed through the status bar. Popup messages each open in a new window when the messages are polled.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Show OK Button and OK Button Text – displays an OK button on the message window, with the button text as entered.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Priority – determines the order standard messages are listed in the status bar listing, and the order Popup messages are displayed.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Show Exit Button and Exit Button Text – displays an Exit button on the message window, with the button text as entered. When clicked, the exit button closes the Innovator application.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Acknowledge – Always or Once. Determines if the message remains in the listing (for standard messages), or pops up every polling interval (for Popup messages). If Once is selected, when the user clicks the OK button, the system stores the acknowledgement, so the message will no longer be displayed.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Style Css (for Standard Template messages) – optional formatting for the message window. The following styles are available for modification -</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">样式名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">描述</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标题</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标题文本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">文本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息文本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">通知确认按钮</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">确认按钮文本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">通知退出按钮</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">退出按钮文本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Style Width and Height (for non-Standard Template messages) – specifies the dimensions of the message window.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Custom HTML (for non-Standard Template messages) – specifies the entire content of the message window. Substitution for {TITLE}, {TEXT} and {URL} are supported.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Save/Unlock/Close the Notification Message. It will be displayed for the target users at the next polling interval.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Messages can be edited and deleted. Note that messages will not be delivered to any users who already acknowledged it, even if the Acknowledge property for the message is changed to “Always”. To re-deliver an acknowledged message, delete the message acknowledgement(s) for that message (see below).</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The following entries produce the message below –</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/bcdd67a327a3.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Viewing Notification Message Acknowledgements:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">An administrator may set the polling interval used by the system to check for new messages, and display pop-up messages.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">On the TOC, open Administration&gt;Notification Messages.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Select and view the message for which you want to review the Message Acknowledgements.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click the toolbar “Hide tabs” button on the tear-off form to deselect it, making the Message Acknowledgements tab visible.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Users who clicked the “OK” button will be listed on the tab.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The messages will not be re-delivered to any users who already acknowledged it. To re-deliver an acknowledged message, delete the message acknowledgement(s) for that message.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Modifying the Notification Message Polling Interval:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">An administrator may set the polling interval used by the system to check for new messages, and display pop-up messages.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Edit On the TOC, open Administration&gt;Variables</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Edit the Message_Check_Interval. The default value of 900000 (in milliseconds) is 15 minutes.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Updates will take affect when a user next logs in.</Run></Paragraph>
</FlowDocument>',
   N'用户通知消息 管理员和其他授权人员可以创建弹出式和标准（非弹出式）通知消息，以便在用户登录 Innovator 时显示。 创建用户通知消息： 前提条件：如果尚未创建，请先创建 ItemType 及其类结构。 在目录中，打开 管理 > 通知消息 点击「创建新通知消息」。将出现通知消息表单。 填写以下字段： 名称 - 通知消息的名称。 消息 - 消息正文。 登录弹出消息 - 如果希望消息在用户登录时以',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [223/244] Using_Split_Screen_Mode.htm (6 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('a48f216c38ab',
   N'使用分屏模式',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">使用分屏模式</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Using_Split_Screen_Mode.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用分屏模式</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">分屏模式使用户能够通过点击搜索工具栏中的分屏图标来启用或禁用并排视图。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以下示例显示了零部件搜索网格在左侧的分屏视图，而活动选项卡或窗口在右侧显示。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7d7a242b10fa.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">零部件搜索网格已停靠。右侧的选项卡已取消停靠，活动选项卡显示所选项目的详细信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用上下文菜单</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7db6775aa807.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">您还可以使用上下文菜单来控制分屏行为。右键点击主网格中的项目，然后从以下选项中选择：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在新窗口中打开 - 在独立窗口中打开项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在新标签中打开 - 在新标签中打开项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在分屏中打开 - 在分屏视图中打开项目。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/577332a32574.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The context menu lets you close a specific tab, selected tabs, tabs that appear on either the right or left side of the screen, or all tabs. Click Tear Off Tab close all tabs. Clicking Dock Tab enables you change a tab from undocked to docked.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Changing Split-Screen Proportions</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">You can change the proportions of the split pane view by clicking and dragging the pane divider.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/283e7ef75de7.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Hover over the pane divider. A double-headed arrow appears and the pane divider changes color to indicate that the pane proportions can be changed.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/1e9b3e6eefb0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Slide the divider to change the proportions.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9a354aa8603d.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'使用分屏模式 分屏模式使用户能够通过点击搜索工具栏中的分屏图标来启用或禁用并排视图。 以下示例显示了零部件搜索网格在左侧的分屏视图，而活动选项卡或窗口在右侧显示。 零部件搜索网格已停靠。右侧的选项卡已取消停靠，活动选项卡显示所选项目的详细信息。 使用上下文菜单 您还可以使用上下文菜单来控制分屏行为。右键点击主网格中的项目，然后从以下选项中选择： 在新窗口中打开 - 在独立窗口中打开项目。 在新标签',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [224/244] using_teams_in_life_cycles.htm (4 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('3b2f41ab4fc7',
   N'在生命周期中使用团队/角色',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">在生命周期中使用团队/角色</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: using_teams_in_life_cycles.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在生命周期中使用团队/角色（仅限管理员）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">支持在生命周期转换的分配中指定团队身份/角色。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">注意：必须为每个项目实例的 team_id 属性设置团队项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">步骤：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在现有生命周期映射的转换中查看（编辑模式），点击「新建分配」按钮以显示分配对话框。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f5186d00ca6f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c84f590d4ad3.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">从「身份」下拉列表中选择团队身份。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从「角色」下拉列表中选择一个角色。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ad42022d1a90.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击保存以将分配添加到转换。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7b4395fee7e3.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'在生命周期中使用团队/角色（仅限管理员） 支持在生命周期转换的分配中指定团队身份/角色。 注意：必须为每个项目实例的 team_id 属性设置团队项目。 步骤： 在现有生命周期映射的转换中查看（编辑模式），点击「新建分配」按钮以显示分配对话框。 从「身份」下拉列表中选择团队身份。 从「角色」下拉列表中选择一个角色。 点击保存以将分配添加到转换。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [225/244] using_teams_in_permissions.htm (8 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('fb53b75fd348',
   N'在权限中使用团队/角色',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">在权限中使用团队/角色</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: using_teams_in_permissions.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在权限中使用团队/角色（仅限管理员）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">支持在权限定义中指定团队身份/角色。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在此之前的示例中，「团队」的任何成员至少具有「获取」访问权限。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a0ab483bf701.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">注意：必须为每个项目实例的 team_id 属性设置团队项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">要设置使用团队/角色的权限：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在权限项的访问选项卡上，点击「添加」按钮。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从「身份」下拉列表中选择团队身份。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择「身份角色」选项。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ea2f74da5611.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">从「角色」下拉列表中选择适当的角色。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4c7e4108de6f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">设置此身份/角色组合应具有的访问权限。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5da5ed34617a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击保存。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/2fa41a5b79d4.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">从搜索对话框中选择一个或多个身份，然后点击。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ad42022d1a90.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The System populates the selected team identities (roles) in the relationship grid. These special team identities can be used in combination with other identities in defining the Permission item.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Set the remaining Access attributes (Get, Update, Delete, etc.) for each team identity:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b09c95a061a6.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click to save your changes and unclaim the permission.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4ddf0c24e4d0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'在权限中使用团队/角色（仅限管理员） 支持在权限定义中指定团队身份/角色。 在此之前的示例中，「团队」的任何成员至少具有「获取」访问权限。 注意：必须为每个项目实例的 team_id 属性设置团队项目。 要设置使用团队/角色的权限： 在权限项的访问选项卡上，点击「添加」按钮。 从「身份」下拉列表中选择团队身份。 选择「身份角色」选项。 从「角色」下拉列表中选择适当的角色。 设置此身份/角色组合应具',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [226/244] using_teams_in_workflow_activities.htm (4 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('a1f0c41c2f0a',
   N'在工作流活动中使用团队/角色',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">在工作流活动中使用团队/角色</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: using_teams_in_workflow_activities.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在工作流活动中使用团队/角色（仅限管理员）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">支持在工作流活动的分配中指定团队身份/角色。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工作流示例：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以下示例显示了系统在团队成员被分配到工作流活动时的处理方式：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">这些示例分配使用以下团队，该团队有两名成员（用户1和用户2），角色为批准者：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">示例 1 - 当分配被发送到团队和角色时，两个成员都会在收件箱中看到该活动。当其中一个成员认领任务时，另一个成员的收件箱中的活动将被移除。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/622157b7d596.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">示例 2 - 如果用户被分配到同一活动的多个团队/角色，该用户只会在收件箱中看到一次该活动。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">受让人</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">投票权重</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">必填</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户1</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">100</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">0</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户2</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">100</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">0</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ECN-10002 has a workflow map activity with Team as the only assignment. Voting Weight= 50, Required=0, For All Members=1. The resulting assignments for this activity in the workflow process would be:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">受让人</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">投票权重</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">必填</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户1</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">50</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">0</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户2</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">50</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">0</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ECN-10003 has a workflow map activity with Team as the only assignment. Voting Weight= 100, Required=1, For All Members=0. The resulting assignments for this activity in the workflow process would be:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">受让人</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">投票权重</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">必填</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户1</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">100</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户2</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">100</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">ECN-10004 has a workflow map activity with Team as the only assignment. Voting Weight= 100, Required=1, For All Members=1. The resulting assignments for this activity in the workflow process would be:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">受让人</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">投票权重</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">必填</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户1</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">50</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用户2</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">50</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">A team item must be set for the team_id property on each item instance to which the workflow applies. This provides the necessary team information for the system to utilize for activity assignments in the workflow process.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">步骤：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">While viewing an activity within an existing workflow map (edit mode), click the Add New Relationship button on the Assignments relationship toolbar. The System will launch a search dialog populated with the existing identities in the system.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Using the blue bar search, filter for identities with Is Alias=0 and Classification=Team. This will return the available system team role identities which can be used directly in the activity assignments.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/2fa41a5b79d4.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Select one or more identities from the search dialog and click . The System populates the selected team identities (roles) in the relationship grid. These special team identities can be used in combination with other identities in defining the Assignments for the workflow activity.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/ad42022d1a90.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Set the remaining Activities on the Activity tab.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/63b583001a23.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click the Process Variables tab and add the appropriate Workflow Map Variables.</Run></Paragraph>
</FlowDocument>',
   N'在工作流活动中使用团队/角色（仅限管理员） 支持在工作流活动的分配中指定团队身份/角色。 工作流示例： 以下示例显示了系统在团队成员被分配到工作流活动时的处理方式： 这些示例分配使用以下团队，该团队有两名成员（用户1和用户2），角色为批准者： 示例 1 - 当分配被发送到团队和角色时，两个成员都会在收件箱中看到该活动。当其中一个成员认领任务时，另一个成员的收件箱中的活动将被移除。 示例 2 - 如',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [227/244] Using_the_Discussion_Panel.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('1600a99fd8dd',
   N'讨论面板',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">讨论面板</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Using_the_Discussion_Panel.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">讨论面板</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">讨论面板使您能够在项目独立窗口的侧边栏中输入评论。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用讨论面板</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在讨论面板中使用 @提及</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">讨论面板工具栏</Run></Paragraph>
</FlowDocument>',
   N'讨论面板 讨论面板使您能够在项目独立窗口的侧边栏中输入评论。 使用讨论面板 在讨论面板中使用 @提及 讨论面板工具栏',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [228/244] Using_Type_Ahead.htm (3 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('6f29342e15e0',
   N'使用自动输入',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">使用自动输入</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Using_Type_Ahead.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用自动输入</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">自动输入功能使您能够自动填充表单中的某些字段。让我们以 CAD 表单中的「类型」字段为例：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">1. 在「类型」字段中输入字母 P。将出现一个以 P 开头的文档类型列表。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/991081354849.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">2. 从列表中选择所需选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您可以将自动输入功能与以下字段类型一起使用：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f6b3ec521948.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">下拉列表框</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目选择器（对话框搜索）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">输入值时，会出现一个以相同字母开头的项目列表。您也可以使用通配符 % 或 * 来扩展搜索。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/53640468218d.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'使用自动输入 自动输入功能使您能够自动填充表单中的某些字段。让我们以 CAD 表单中的「类型」字段为例： 1. 在「类型」字段中输入字母 P。将出现一个以 P 开头的文档类型列表。 2. 从列表中选择所需选项。 您可以将自动输入功能与以下字段类型一起使用： 下拉列表框 项目选择器（对话框搜索） 输入值时，会出现一个以相同字母开头的项目列表。您也可以使用通配符 % 或 * 来扩展搜索。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [229/244] Variable_Properties.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('ad5687cb0eed',
   N'变量属性',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">变量属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Variable_Properties.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">变量属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">必填</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">是</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">变量的名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">值</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">是</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">变量的值</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">描述</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">否</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">变量的描述</Run></Paragraph>
</FlowDocument>',
   N'变量属性 属性名称 必填 说明 名称 是 变量的名称 值 是 变量的值 描述 否 变量的描述',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [230/244] Vault_Storage_Locations.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('f834e27f72e6',
   N'保险库存储位置',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">保险库存储位置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Vault_Storage_Locations.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保险库存储位置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">存储文件的保险库位置由用户设置决定，该设置在用户登录 Innovator 时建立。</Run></Paragraph>
</FlowDocument>',
   N'保险库存储位置 存储文件的保险库位置由用户设置决定，该设置在用户登录 Innovator 时建立。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [231/244] Viewer.htm (58 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('0cdb41d0aad0',
   N'查看器工具栏',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">查看器工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Viewer.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">查看器工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">项目独立窗口中的侧边栏包含文件查看器的控件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">通过查看器工具栏控件可以使用以下工具栏：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">PDF 查看器工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图像查看器工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">2D 查看器工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">3D 查看器工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">批注工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">每个工具栏在查看器处于活动状态且查看相应文件类型时可用。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/470ac1ed460f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The PDF viewer advanced toolbar is as shown:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c151c7254fe8.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The available commands are as follows:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图标</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工具提示</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">描述</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d1de0a21d56f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">视图</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示 PDF 文件。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c2fc278f8c99.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">批注</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Enables you to annotate the file.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0710852c48e5.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">高亮</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Enables you to highlight portions of the file.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0008f1a80d22.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">标签</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Enables you to add a label to text or graphics in a file.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/54fe7f694539.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">选择</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Enables you to select items in a file.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a890e8659bea.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">删除</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">删除选定项目。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/32bc0b1e30c0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">下载</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">下载文件。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4c3f64812716.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">打印</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打印文件。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6e567ad06177.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">切换到基本工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Switches to the Basic toolbar.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/80304cc4a282.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">切换到标准工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Swtiches to the Standard toolbar</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c68413415140.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">放大</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">放大查看</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e729e9e5e84c.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">缩小</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">缩小查看</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/658190e9049e.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">重置视图</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">恢复为上一视图</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/afa12479abce.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">朝向面</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Shows the CAD Model oriented towards you showing the selected part surface.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0e83b0128f0f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">设置显示样式</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Selects the display style for the item.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6d5947226d61.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">测量</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Specifies the measuring method to use.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f3491e832449.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">爆炸视图</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Enables you to explode the drawing to view the individual components.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a7ab2c7a274c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">截面</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Enables you to create and view cross sections of a drawing.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/4a53b696ef2d.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">模型浏览器</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Enables you to view the model from different angles. There are two types of views: Standard and CAD.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e055b6b2bb05.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Product Manufacturing information</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Select this icon to view the associated product information.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a443f74beff3.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">配置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Displays a list of the possible viewing configurations for a CAD model.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/68b457ad03d6.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">适合宽度</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Fits page width to the viewer frame in the horizontal direction.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3fcb1a5ddeaf.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">适合高度</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Fits page height to the viewer frame in the vertical direction.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6f96ecb11c01.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">上一页</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Goes to the previous page (no effect if page 1 is current)</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/243cf99a2e0e.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">下一页</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Goes to the next page after the current one (no effect if last page is current)</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/661264e6fa64.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">跳转到页面</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开分页对话框：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0238ede68e86.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The current page number is shown.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Type in the desired number and hit the Enter key to display the page.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click the top and bottom arrow icons to open the first and last page respectively.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/46e0acf460c5.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">查找</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开查找对话框：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/14194814e1e6.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Type in the required search text and hit the Enter key to show the text.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click the right arrow to re-positions the file to show the next instance of that text string.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Click the left arrow to search backward in the file.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">If the Highlight All checkbox is checked, all instances of the text are instantly highlighted.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">If the Match Case checkbox is checked, the text search is performed as case-sensitive.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/de77e5ee36c9.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">缩放百分比</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开缩放对话框：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/8857e4357a27.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The zoom value is set to 100% by default.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Enter the desired value to zoom the page to that percent.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d2549b6a8860.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">顺时针旋转90度</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Rotate file content 90 degrees clockwise</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/9c77d941a83a.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">逆时针旋转90度</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Rotate file content 90 degrees counter-clockwise</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a598a993fbb6.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">展开高级工具</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">展开高级工具栏。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/7384e54cb989.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">关闭高级工具</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关闭高级工具栏。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图像查看器工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The Image viewer basic toolbar is as shown:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/470ac1ed460f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The Image viewer advanced toolbar is as shown:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/0b996b509799.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">3D 查看器工具栏</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The Image viewer basic toolbar is as shown:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/059f0390ed2c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The 3D viewer advanced toolbar is as shown:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/69ae49b35b8d.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The available commands are as follows:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图标</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工具提示</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">描述</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c68413415140.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">放大</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">放大</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e729e9e5e84c.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">缩小</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">缩小</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/734fd259c7c7.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">重置视图</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Resets the view back to default.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/785d0b2a7469.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">设置视图方向</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开视图方向选择器：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图标</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工具提示</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">描述</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">等轴测视图</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设置为等轴测视图。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">朝向面</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">User must pick a planar face, the view is then oriented perpendicular to it.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">左视图</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设置为左视图。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">右视图</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设置为右视图。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">底视图</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设置为底视图。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">前视图</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设置为前视图。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">后视图</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设置为后视图。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">顶视图</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设置为顶视图。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/eb7da526116c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">设置显示样式</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开显示样式选择器：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图标</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工具提示</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">描述</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">线框</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以线框模式显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">隐藏线</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以隐藏线模式显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">着色</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">以着色模式显示。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">着色线框</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Display as Wireframe on shaded.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/dcf2af985a84.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">测量</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">打开测量对话框：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/2aad1640b5e7.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">图标</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工具提示</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">描述</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/49a5634e9a2f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点到点测量</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择两个点创建测量。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5e0ff5c48e09.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">边测量</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Pick a single edge to create a measurement.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/bf7460b5881f.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">面间角度测量</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择两个平面创建角度测量。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b4ced07362e5.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Measure Distance Between Faces</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">选择两个平面创建距离测量。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/24d614f3881f.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">选择</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关闭测量。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/fd2772e1e601.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">设置旋转样式</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Opens the Rotation Style Selector:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图标</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工具提示</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">描述</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">步行</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">设置旋转样式为步行。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">转盘</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Set rotation style to Turntable.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">轨道相机</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Set rotation style to Orbit Camera.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/20b25d10befb.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">爆炸视图</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Opens the Exploded View dialog:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/61b457b8c056.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">截面</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Opens the Cross Section dialog:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/db9b321efae1.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">模型浏览器</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Opens the Model Browser dialog:</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/5dd8799aec71.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">This allows the user to interact with the structure of the 3D model.</Run></Paragraph>
</FlowDocument>',
   N'查看器工具栏 项目独立窗口中的侧边栏包含文件查看器的控件。 通过查看器工具栏控件可以使用以下工具栏： PDF 查看器工具栏 图像查看器工具栏 2D 查看器工具栏 3D 查看器工具栏 批注工具栏 每个工具栏在查看器处于活动状态且查看相应文件类型时可用。 The PDF viewer advanced toolbar is as shown: The available commands are as',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [232/244] Viewer_Properties.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('8ddedf62e4c1',
   N'查看器属性',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">查看器属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Viewer_Properties.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">查看器属性</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">必填</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">是</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">查看器的名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">描述</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">否</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">查看器的描述</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">查看器类型</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">是</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">用于显示文件的查看器类型</Run></Paragraph>
</FlowDocument>',
   N'查看器属性 属性名称 必填 说明 名称 是 查看器的名称 描述 否 查看器的描述 查看器类型 是 用于显示文件的查看器类型',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [233/244] Views.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('a7aa48283677',
   N'视图',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">视图</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Views.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">视图</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">视图是 ItemType 与表单之间的关系。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可以为添加、查看和编辑显示替代表单，这是在 ItemType 的视图定义中选择的。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">默认情况下，所有 ItemType 具有相同表单用于添加、查看和编辑操作。管理员可以为每个 ItemType 定义不同的表单，以提供不同的数据输入或查看体验。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/43f7959b9efc.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'视图 视图是 ItemType 与表单之间的关系。 可以为添加、查看和编辑显示替代表单，这是在 ItemType 的视图定义中选择的。 默认情况下，所有 ItemType 具有相同表单用于添加、查看和编辑操作。管理员可以为每个 ItemType 定义不同的表单，以提供不同的数据输入或查看体验。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [234/244] Views_Menu_(Item_Window).htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('b722fcf134be',
   N'视图菜单（项目窗口）',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">视图菜单（项目窗口）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Views_Menu_(Item_Window).htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">视图菜单（项目窗口）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">视图菜单提供查看与 ItemType 和项目关联的访问权限、历史记录等选项。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">视图菜单中的某些选项可能显示为禁用状态。菜单选项在选择适当项目且登录用户具有足够访问权限时启用。访问权限由管理员配置。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">菜单选项</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示历史记录</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示项目的修改历史记录。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示访问权限</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示项目的访问权限（权限）。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示版本</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示项目的所有版本。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示生命周期历史</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示项目的生命周期历史记录。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">关系视图</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">切换关系网格的视图模式。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示所有关系</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">显示所有关系选项卡。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">隐藏所有关系</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">隐藏所有关系选项卡。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工作流</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Displays the Workflow Process (if one exists) associated with the open Item.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">生命周期</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Displays the current state of the open Item in the LifeCycle Map.</Run></Paragraph>
</FlowDocument>',
   N'视图菜单（项目窗口） 视图菜单提供查看与 ItemType 和项目关联的访问权限、历史记录等选项。 视图菜单中的某些选项可能显示为禁用状态。菜单选项在选择适当项目且登录用户具有足够访问权限时启用。访问权限由管理员配置。 菜单选项 说明 显示历史记录 显示项目的修改历史记录。 显示访问权限 显示项目的访问权限（权限）。 显示版本 显示项目的所有版本。 显示生命周期历史 显示项目的生命周期历史记录。 ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [235/244] View_and_Markup_Toolbars.htm (3 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('ab543fb26b09',
   N'查看和批注模式',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">查看和批注模式</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: View_and_Markup_Toolbars.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">查看和批注模式</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当查看器在项目窗口中处于活动状态时，用户可以访问查看器和批注工具栏。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">一次只能有一个模式处于活动状态，活动模式由较大的按钮指示。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/fcf1a6f09d97.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">查看模式</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当用户点击查看器工具栏时，用户处于查看模式。在此模式下，用户可以：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">查看文件内容</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用鼠标平移、缩放和旋转文件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用键盘快捷键进行导航</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/8d54281acac4.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">打印文件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">下载文件</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">批注模式</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当用户点击批注工具栏时，用户处于批注模式。在此模式下，用户可以：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在文件上创建批注标记</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/bbd6f5d49eb0.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">使用绘图工具（涂鸦、高亮、标签）</Run></Paragraph>
</FlowDocument>',
   N'查看和批注模式 当查看器在项目窗口中处于活动状态时，用户可以访问查看器和批注工具栏。 一次只能有一个模式处于活动状态，活动模式由较大的按钮指示。 查看模式 当用户点击查看器工具栏时，用户处于查看模式。在此模式下，用户可以： 查看文件内容 使用鼠标平移、缩放和旋转文件 使用键盘快捷键进行导航 打印文件 下载文件 批注模式 当用户点击批注工具栏时，用户处于批注模式。在此模式下，用户可以： 在文件上创建',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [236/244] Visual_Colloboration_Functionality.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('26997a2cdc04',
   N'可视化协作功能',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">可视化协作功能</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Visual_Colloboration_Functionality.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可视化协作功能</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可视化协作在 Aras Innovator 中提供基于 Web 的查看和批注功能。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">有关更多信息，请参阅以下主题：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">查看和批注模式</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建快照评论</Run></Paragraph>
</FlowDocument>',
   N'可视化协作功能 可视化协作在 Aras Innovator 中提供基于 Web 的查看和批注功能。 有关更多信息，请参阅以下主题： 查看和批注模式 创建快照评论',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [237/244] Welcome.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('1beaff64a0c3',
   N'在线帮助中心',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">在线帮助中心</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Welcome.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在线帮助中心</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Aras Innovator 的在线知识中心</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Just Ask Innovator 代表了一个一站式学习中心，面向培训人员、最终用户和管理员。Just Ask Innovator 包含以下资源：</Run></Paragraph>
</FlowDocument>',
   N'在线帮助中心 Aras Innovator 的在线知识中心 Just Ask Innovator 代表了一个一站式学习中心，面向培训人员、最终用户和管理员。Just Ask Innovator 包含以下资源：',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [238/244] Where_used.htm (1 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('20a367ac4217',
   N'使用位置',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">使用位置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Where_used.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用位置</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">您可以使用上下文菜单中的「使用位置」选项来查看引用了某个项目的 ItemType 列表。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">让我们考虑一个场景，您想要替换或删除 CAD 文档引用的零部件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在做出此类决定之前，您可以查看「使用位置」树结构以防止错误，因为删除被另一个项目引用的项目可能会产生不良影响。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">使用位置功能</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">从主网格中选择一个项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">右键点击并从上下文菜单中选择「使用位置」。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">将显示使用位置树结构，显示引用了所选项目的所有项目。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">查看信息后，点击关闭。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/1237ecdf8f64.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The Tree structure displays the related Parts and products. Part C2951A-A1 is associated with four other Parts (C3801-80020-A.1, C9223-A1, and C9225-A.1). Part C9223-A.1 is also associated with Model 650A.1 and its associated Product. Part C9225-A.1 has a similar relationship with a Model and a Part. You can click twice on an Item to view the it.</Run></Paragraph>
</FlowDocument>',
   N'使用位置 您可以使用上下文菜单中的「使用位置」选项来查看引用了某个项目的 ItemType 列表。 让我们考虑一个场景，您想要替换或删除 CAD 文档引用的零部件。 在做出此类决定之前，您可以查看「使用位置」树结构以防止错误，因为删除被另一个项目引用的项目可能会产生不良影响。 使用位置功能 从主网格中选择一个项目。 右键点击并从上下文菜单中选择「使用位置」。 将显示使用位置树结构，显示引用了所选项',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [239/244] Who_Needs_to_Read_This_Guide.htm (0 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('a40e6cc5d6ee',
   N'谁需要阅读此帮助',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">谁需要阅读此帮助</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Who_Needs_to_Read_This_Guide.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">谁需要阅读此帮助</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Just Ask Innovator 在线帮助描述了如何设置、使用和维护 Aras Innovator 产品生命周期管理（PLM）软件。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">虽然 Aras Innovator 独特地结合了对象和业务规则定义以及部署在单一平台上的完整 PLM 应用程序，但某些读者可能只需要阅读与他们相关的部分。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">相关文档</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">订阅者：完整文档可在 CD 镜像中的版本发行版上获得。请咨询您的系统管理员或 Aras 联系人以获取更多信息。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">产品文档：产品文档（如 IOM API 参考、包定义参考等）可在版本 CD 上获得。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">http://www.aras.com/services/subscription.aspx.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Other Users: Open documentation is available at the following location on the Aras web-site:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">http://www.aras.com/support/documentation/.</Run></Paragraph>
</FlowDocument>',
   N'谁需要阅读此帮助 Just Ask Innovator 在线帮助描述了如何设置、使用和维护 Aras Innovator 产品生命周期管理（PLM）软件。 虽然 Aras Innovator 独特地结合了对象和业务规则定义以及部署在单一平台上的完整 PLM 应用程序，但某些读者可能只需要阅读与他们相关的部分。 相关文档 订阅者：完整文档可在 CD 镜像中的版本发行版上获得。请咨询您的系统管理员或 ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [240/244] workflow_promote.htm (7 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('31e9585b64e1',
   N'工作流提升',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">工作流提升</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: workflow_promote.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工作流提升</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Aras Innovator 提供了创建工作流的功能，以自动控制项目的生命周期状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在前面的示例中，一个 ItemType 的生命周期和工作流已定义。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d7815c9afdcf.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">重要工作流提升规则：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">受控项目的当前生命周期状态必须与已定义的工作流路径起始状态之一匹配。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工作流活动完成后，项目将自动提升到工作流路径定义的目标生命周期状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果工作流定义了多条路径，系统将根据活动投票结果确定使用哪条路径。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">投票选项由管理员在工作流定义中配置。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Steps to Configure a Workflow Promotion:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Select an activity in the Workflow Map Editor which will be used to initiate the workflow promotion based on a triggered event.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Select the Promotions relationship tab.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/c05b8ff23001.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Click the Add New Relationship button on the relationship toolbar.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">A search dialog is automatically launched by the system containing all Life Cycle Transitions currently defined in the system. Search for the appropriate Life Cycle Transition. Select the row in the dialog and click the toolbar button.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/11d26c325a74.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/57935f28e47b.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The selected Life Cycle Transition is added as a new row in the Promotions grid. The Event property value is set to ‘On Activate’ by default. This is the Activity event which, when triggered, will execute the lifecycle promotion. Set the Event to the desired value by clicking in the grid cell and selecting a value from the drop-down list.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/acabda6f9414.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">6. Select the ItemType that is associated with the lifecycle transition by clicking into the ItemType grid cell and then pressing F2 on the keyboard. A search dialog is automatically launched by the system containing all ItemTypes currently defined in the system. Search for the appropriate ItemType. Select the row in the dialog and click the toolbar button.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/11d26c325a74.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The selected ItemType must have both the current Workflow and Lifecycle (from chosen transition) specified in its ItemType definition</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a2d28cb2f2d1.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Additional Workflow Promotions may be added as necessary to the current Activity or other Activities in the Workflow.</Run></Paragraph>
</FlowDocument>',
   N'工作流提升 Aras Innovator 提供了创建工作流的功能，以自动控制项目的生命周期状态。 在前面的示例中，一个 ItemType 的生命周期和工作流已定义。 重要工作流提升规则： 受控项目的当前生命周期状态必须与已定义的工作流路径起始状态之一匹配。 工作流活动完成后，项目将自动提升到工作流路径定义的目标生命周期状态。 如果工作流定义了多条路径，系统将根据活动投票结果确定使用哪条路径。 投票',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [241/244] Workflow_Activity.htm (8 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('e405baa72613',
   N'工作流提升',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">工作流提升</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Workflow_Activity.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工作流提升</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Aras Innovator 提供了创建工作流的功能，以自动控制项目的生命周期状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在前面的示例中，一个 ItemType 的生命周期和工作流已定义。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/bf684fc218ed.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">重要工作流提升规则：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">受控项目的当前生命周期状态必须与已定义的工作流路径起始状态之一匹配。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工作流活动完成后，项目将自动提升到工作流路径定义的目标生命周期状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果工作流定义了多条路径，系统将根据活动投票结果确定使用哪条路径。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">投票选项由管理员在工作流定义中配置。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The name of the Activity Template</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标签</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Multilingual string used to identify the Activity in the user interface, see Internationalization</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">升级到</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">This is the Identity that replaces the assignee when this activity is overdue or for other reasons that require escalation. When there is no identity specified here, issues are escalated to the Process Owner. If no identity is specified there, they are escalated to the Creator of the workflow map.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">消息</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">A description of the work to be performed.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">预期持续时间</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The expected amount of calendar time until completion. The due date of the activity is calculated by adding the expected duration to the date the activity was activated.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">超时持续时间</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The amount of calendar time after the duedate before the activity is escalated.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">提醒间隔</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The amount of time between reminder messages calculated backwards from the Due Date of activity. So, if the Reminder Count = 3, and the Reminder Interval = 1, the reminder message will be sent 2 days before the due date, 1 day before the due date, and on the due date.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/fc8a130e8361.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">提醒次数</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The number of reminder messages to be sent. The Expected Duration must be greater than the Reminder Count * Reminder Interval.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">子流程</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The workflow map to be instantiated as a subflow. The activity will not be considered complete until that subflow is complete. The exit path from an activity that represents a subflow, is typically marked as Default Path.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">图标</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The icon to display in the graphical editor.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">管理人</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">An identity allowed to make on the spot changes to the activity once the Workflow is instantiated. For example, say you have a PR, and in a triage meeting specific employees are identified to work on it. The Managed By identity can then open the PR item, select Views menu, Workflow, and bring up the Workflow Process (the instance of the Workflow for this specific PR). From the Workflow Process the Managed By identity can edit assignments.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">角色</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">This property is used only at run time when changes are made to the instantiated workflow by the Managed By identity. Role is the group identity from which the Managed By identity can select individual identities for assignments. If an assignee is chosen that does not belong to the specified Role group identity, an error is thrown.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">起始活动</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Activities marked as Start Activities are activated when the workflow is started.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">结束活动</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Reaching an end activity causes the workflow to end.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">自动活动</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">An activity that requires no user interaction and completes automatically. For example, the Start activity is marked as automatic, and will allow the item to proceed to the next activity automatically, as long as its exit path is marked as Default. The same is true of an activity that contains a subflow. It must be marked as Automatic, and must have a Default exit path.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可以拒绝</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Allows the assignees to refuse this activity, and automatically escalates it. The escalation path is: Escalate To identity on the assignment, Escalate To identity on the Activity, Process Owner, Creator. When the activity is refused, Innovator will assign the next available identity on the escalation path.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">可以委派</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Allows the assignees to reassign this activity to another identity.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">合并委派</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">When the Can Delegate option is checked, it is possible that the same identity can be delegated to several times for the same activity. Consolidate Delegated = False forces the delegated to identity to vote once for each delegation. Consolidate Delegated = True allows the delegated to identity to vote once where the voting weight will be cumulative of all of the assigned delegations, specifically:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Consolidate Delegation = True: before identity votes, another delegated task is received. Still vote only once, cumulating the total weight of all the delegated tasks.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Consolidate Delegation = False: after identity votes, another delegated task is received. This new task will have the same vote and the new tasks&apos;s voting weight will be added to the cumulative Consolidated weight. Effectively this new delegation will never even appear in the InBasket; the voting will take place automatically.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">等待所有输入</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Does not activate the activity until all input paths have been completed. If not checked, as soon as any one input path is completed, this activity becomes active.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">等待所有投票</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Does not close the activity until all assignees have voted. Many times the outcome of the vote is obvious before all the votes are in, however this option will force the delay until all the assigned votes are gathered.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">活动选项卡</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The following is a list of tabs for the Workflow Activity.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The first tab for the Activity is called Tasks, and contains a checklist of all the tasks that need to be completed for this activity to be considered done. See Completion Worksheet to see how these tasks are displayed on the Activity Completion form for the assignee. The following is an example of a workflow map. The Collect Data activity has been selected. Notice that out of tasks, only one is required. Below is a list of properties that describe a task.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/3b20b3c5f4fe.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Sequence - The order number of the task. Determines the order in which tasks appear on the Completion Worksheet.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Required - if true, this task will require the assignee&apos;s sign-off when completed in order for the activity to be considered complete.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Description - the description of the task to be completed by the assignee.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">The next tab for the Activity is Notifications. Notification allows email messages to be configured to be sent automatically from chosen identities to chosen identities. Notifications are triggered by the selected events, such as On Active (when the activity becomes active), or On Delegate (when the activity is delegated). Notifications can also contain current instance data, such as the values of any specified properties of the ItemType instance with which this workflow is associated, or the properties of the workflow process itself. You can also include any other instance data, from other items, such as properties of a related item, or the name of the activity itself, by providing a query to retreive this information from the database. For more information, see Configring Notifications.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/6e66027ce1eb.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The Paths tab lists all the exit paths that flow out of the selected activity. This list is filled in automatically when the diagram is completed in the graphical map designer. The following is a view of the paths listed for the Review ECR activity. From this tab you can edit the paths, but you cannot delete them or create new ones. For a full explanation of the properties of the path, see Workflow Paths.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/49111b2a7e6a.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The Variables tab stores activity variables which can receive data from the assignees, or can be used for internal calculations based on activity properties. See how these variables appear on the Completion Worksheet to be completed by the assignee. The following is a list of properties by which a variable is defined.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/cc47c274a08c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Sequence - the sequence number of the variable determines the display order on the Activity Completion Worksheet where the assignee will be entering the value. If the variable is hidden, the sequence number is of no significance.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Name - the name of the variable.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Type - the data type of the variable.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Source - the data source for specific data types of the variable, such as a list or sequence.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Default Value - If the variable is required, it&apos;s a good idea to provide a default value.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Required - if true, the activity cannot complete without a valid value for the variable.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Hidden - if true, the variable will not show up on the Activity Completion Worksheet. If the variable is also required, the value must be set by internal code befrore the activity can be completed.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Assignments is the tab where those responsible for the activity are identified. Along with specifying the identity responsible for performing the tasks, here we set the options on how the next activity will be reached. The following is a workflow map where there are two exit paths from the Analyze and Sort activity - Test Data and Reject Data. Once the tasks of the Analyze and Sort activity are completed, each identity responsible for this activity will have to select the exit path, in this case either Test Data or Reject Data. The process of selecting the exit path is called &quot;voting&quot;. See Workflow Examples to understand how the different options work together.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/b72715928b8a.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">The columns listed here - Required, For All Members, and Voting Weight - combined with the Wait For All Inputs header property determine when the activity will be considered done, and also sets properties of the voting process.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/1c1871c52457.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Name - The identity responsible for the tasks of this activity, and also for voting on the exit workflow path out of this activity.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Required - When checked it means that members of this identity must complete this activity (finish the tasks and vote on the exit path from the activity), even if there have already been enough votes to determine the next activity.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">For All Members - this option applies only to group identities assigned to this activity; if false, only one member of the group identity has to complete and vote on the activity, which will then automatically remove this activity from the other group members&apos; In Basket. If true, all members have to complete and vote on the activity and will be assigned equal voting weight with the cumulative weight equal to the assigned voting weight.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Voting Weight - In order for a path to be selected it has to reach at least 100% vote, or more. Please refer to Examples to see how the voting weight and other options determine the exit path of the activity.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Escalate To - an identity that will receive this assignment if the activity is not completed by the due date + time out. If no identity is specified here, then the Activity Escalate To identity will receive this assignment.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Server Events is the last tab and it contains the methods that can be executed when triggered on the specified list of Server Events. The trigger events are: On Activate, On Assign, On Refuse, On Delegate, On Vote, On Remind, On Due, On Escalate, and On Close. The instruction on writing methods is beyond the scope of this manual. Please refer to the Advanced Programming Course, or contact your Aras Consultant if you wish to write methods.</Run></Paragraph>
</FlowDocument>',
   N'工作流提升 Aras Innovator 提供了创建工作流的功能，以自动控制项目的生命周期状态。 在前面的示例中，一个 ItemType 的生命周期和工作流已定义。 重要工作流提升规则： 受控项目的当前生命周期状态必须与已定义的工作流路径起始状态之一匹配。 工作流活动完成后，项目将自动提升到工作流路径定义的目标生命周期状态。 如果工作流定义了多条路径，系统将根据活动投票结果确定使用哪条路径。 投票',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [242/244] Workflow_Activity_Worksheet.htm (2 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('ce433c5f92ec',
   N'工作流活动',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">工作流活动</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Workflow_Activity_Worksheet.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工作流活动</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工作流活动由标题属性和选项卡描述。当创建新活动时，将自动分配一个默认表单。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">活动标题属性</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/a0ae9a8b7936.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">下表描述了活动标题属性：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">属性</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/878ec8805bea.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">说明</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">名称</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">活动的名称。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">标签</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在用户界面中标识活动的多语言字符串。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">描述</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">活动的描述。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">日程日期</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">活动的预定开始日期。</Run></Paragraph>
</FlowDocument>',
   N'工作流活动 工作流活动由标题属性和选项卡描述。当创建新活动时，将自动分配一个默认表单。 活动标题属性 下表描述了活动标题属性： 属性 说明 名称 活动的名称。 标签 在用户界面中标识活动的多语言字符串。 描述 活动的描述。 日程日期 活动的预定开始日期。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [243/244] Workflow_Examples.htm (5 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('c97a6e61f189',
   N'工作流活动工作表',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">工作流活动工作表</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Workflow_Examples.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工作流活动工作表</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当工作流活动变为活动状态时，它会立即发送给所有被分配的用户。收件箱中显示的活动将显示工作流图标。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">双击任何项目将打开该项目的完成工作表，该工作表通常由任务列表、投票选择以及任何需要填写的表单数据组成。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">完成工作表的不同区域：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/51476302d8d8.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">任务 - 列出活动「任务」选项卡中指定的所有任务。标记为必需的任务必须在投票之前完成。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">投票 - 提供从活动投票选项卡中定义的选项中进行选择的功能。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">投票按钮 - 根据投票选项进行投票或完成活动。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">保存更改 - 保存完成工作表中的当前信息但不投票。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">完成 - 保存更改并提交投票以继续工作流。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Let&apos;s take the same example we have above, but now, let&apos;s say that the Reject Data path is marked as Override.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/fb550fcf18c2.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">As soon as the Override path receives even one vote, no matter how small the percentage of the total vote weight that vote brings, the override path is executed. So, in this case, even though the Reject Data path receives just 10%, the Reject Data activity is activated.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">默认路径示例</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">In this example, we are again working with the Analyze and Sort activity, however, there are no longer any Override exit paths from this activity. Now, we have the Test Data path marked as the Default Path.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/e4746c44df3c.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Here again, the Change Analyst 1 votes Test Data, which gives it only 95%, and the Creator votes Reject Data, which receives 10%. If none of the paths are marked as Default or Override, this issue would be escalated. However, since we do have a Default Path, the default is chosen instead. So, in this case, the activity Test Data would become active next.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">必填示例</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">In this example we are again working with the Analyze and Sort activity. Notice however that now there are 3 Identities responsible for completing it - the Change Specialist I, the Creator, and the Inspectors. Notice that the Creator is marked as Required. This means that even if there is 100% vote weight on any one path, the Required identity must vote.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/d6639b2a0ca2.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">Let&apos;s assume that none of the paths are marked as Override or Default, and that the voting so far yields:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Data Analysis - Test Data (95 %)</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Testing and Quality - Test Data (95%)</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">This would give the Test Data path 190%, which is enough for that path to be selected. However, since the Creator has not yet voted, the activity does nothing and awaits that last input. If the Creator votes Reject Data, the Test Data activity is activated next. However, if the Reject Data was marked as Override, then that one last vote would change the whole picture and Reject Data would be activated instead.</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">等待所有投票示例</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">If the Wait For All Votes option is checked on the Activity, then no action will be taken until all the votes are in, even if there is 100% gathered on any particular path, or even if a vote is cast for an Override path.</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/713c52076642.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">In the above example, all 3 identities have to vote before the exit path is activated. Let&apos;s say that the Reject Data path is marked as Override. Then, the votes come in as follows:</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">Change Specialist I - Test Data (95%)</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">创建者 - 拒绝（10%）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">At this point the result is very clear. The Reject Data path will be selected. However, Aras Innovator will wait until the Inspector group votes, and then the Reject Data activity will be activated.</Run></Paragraph>
</FlowDocument>',
   N'工作流活动工作表 当工作流活动变为活动状态时，它会立即发送给所有被分配的用户。收件箱中显示的活动将显示工作流图标。 双击任何项目将打开该项目的完成工作表，该工作表通常由任务列表、投票选择以及任何需要填写的表单数据组成。 完成工作表的不同区域： 任务 - 列出活动「任务」选项卡中指定的所有任务。标记为必需的任务必须在投票之前完成。 投票 - 提供从活动投票选项卡中定义的选项中进行选择的功能。 投票按',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

-- [244/244] Workflow_Path.htm (2 imgs)
INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  ('d15ef6c5bdeb',
   N'工作流示例',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">工作流示例</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Workflow_Path.htm</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工作流示例</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">工作流活动和工作流路径中有几个属性可以影响工作流的行为。让我们通过几个示例来理解这些行为。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">升级示例：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/135cc29dc6a5.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">在此示例中，「分析和排序」活动被激活。注意分配给此活动的两个用户 - 变更专家 I 和变更专家 II。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">变更专家 I - 测试数据（95%）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">变更专家 II - 测试数据（70%）</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">假设您将必需完成百分比设置为 100%，将关键路径百分比设置为 90%。在这种情况下，当变更专家 I 进行投票时，将满足关键路径百分比（95% &gt; 90%），活动将提升到下一状态。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">如果关键路径百分比设置为 100%，则需要两位用户的投票才能完成活动。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/431bd081727b.gif" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">升级活动使用场景：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">当您想要设置一个系统，使得一些用户可以快速处理简单情况，但如果情况更复杂，需要额外的意见或批准时，此功能非常有用。</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">撤销示例：</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">撤销活动使工作流能够返回到之前的活动。当活动被撤销时，它会返回到原始活动。</Run></Paragraph>
</FlowDocument>',
   N'工作流示例 工作流活动和工作流路径中有几个属性可以影响工作流的行为。让我们通过几个示例来理解这些行为。 升级示例： 在此示例中，「分析和排序」活动被激活。注意分配给此活动的两个用户 - 变更专家 I 和变更专家 II。 变更专家 I - 测试数据（95%） 变更专家 II - 测试数据（70%） 假设您将必需完成百分比设置为 100%，将关键路径百分比设置为 90%。在这种情况下，当变更专家 I ',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');

