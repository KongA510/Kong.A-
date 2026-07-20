INSERT INTO [knowledge_entry]
  ([id], [title], [content], [plain_text_preview], [tags], [category],
   [pinned], [created_date], [modified_date], [creator_on], [user_id])
VALUES
  (LEFT(REPLACE(NEWID(), '-', ''), 12),
   N'创建表单',
   N'<FlowDocument PagePadding="5,0,5,0" AllowDrop="True" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
  <Paragraph><Run xml:lang="zh-cn" FontSize="20" FontWeight="Bold">创建表单</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn" Foreground="Gray">分类: 系统管理详解 | 来源: Creating_a_Form</Run></Paragraph>
  <Paragraph><Run xml:lang="zh-cn">在导航窗格中选择「管理」&gt;「表单」，将出现以下菜单：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/f4124098c4de.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">点击「新建表单」，将出现一个空表单：</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/383f5d419e21.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
  <Paragraph><Run xml:lang="zh-cn">在表单字段中输入相关信息，然后点击「保存项目」按钮。</Run></Paragraph>
<BlockUIContainer>
  <Image MaxWidth="600" Stretch="Uniform">
    <Image.Source>
      <BitmapImage UriSource="D:/git仓库/KnowledgeImages/29c7d82a1260.jpg" CacheOption="OnLoad" />
    </Image.Source>
  </Image>
</BlockUIContainer>
</FlowDocument>',
   N'在导航窗格中选择「管理」>「表单」，将出现以下菜单。点击「新建表单」，将出现一个空表单。在表单字段中输入相关信息，然后点击「保存项目」按钮。',
   NULL,
   N'系统管理详解',
   0,
   GETDATE(),
   NULL,
   GETDATE(),
   N'ae7cd0e8f4e8');
