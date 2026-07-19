const fs = require('fs');
const path = require('path');

// Load all translations from translations_all.json
const allTranslations = JSON.parse(fs.readFileSync(path.join(__dirname, 'translations_all.json'), 'utf-8'));

// Additional medium and long paragraph translations
const additionalTranslations = {
  // Medium paragraphs (30-200 chars)
  ".Creating a Relationship with Related Item": "创建带关联项目的关系",
  "An Innovator List which can hold more than one value.": "可以包含多个值的 Innovator 列表。",
  "A long string of letters and numbers up to 1GB in length.": "最长可达1GB的字母和数字字符串。",
  "Create the source ItemType and the related ItemType": "创建源 ItemType 和关联 ItemType",
  "From the source ItemType create and name the relationship.": "从源 ItemType 创建并命名关系。",
  "Claim, Edit, Done, and Unclaim": "认领、编辑、完成和取消认领",
  "determines where the Action appears in Menus": "确定操作在菜单中的显示位置",
  "specifies where the action runs": "指定操作的执行位置",
  "written code that the action calls": "操作调用的代码",
  "the method that runs once the action is completed": "操作完成后执行的方法",
  "Edit field properties, as needed.": "根据需要编辑字段属性。",
  "Use the same procedure to add documents to a Forum.": "使用相同的过程将文档添加到论坛。",
  "Properties of type Boolean (yes/no) display a checkbox.": "布尔类型（是/否）的属性显示为复选框。",
  "Properties of type List display a dropdown.": "列表类型的属性显示为下拉框。",
  "Displays Part ItemTypes which include a description.": "显示包含描述的零部件 ItemType。",
  "Click the 'Search' button in the search toolbar.": "点击搜索工具栏中的「搜索」按钮。",
  "Open Opens the form of the Item in a tear-off window": "在独立窗口中打开项目的表单",
  "Click to save and close the ItemType.": "点击保存并关闭 ItemType。",
  "Only one state per Life Cycle can be marked as Released.": "每个生命周期只能有一个状态标记为已发布。",
  "Click the Save Item button to save changes to the form.": "点击「保存项目」按钮保存表单更改。",
  "Click the Unlock button to unclaim the form.": "点击「解锁」按钮取消认领表单。",
  "Repeat steps 10-15 for each Form.": "对每个表单重复步骤10-15。",
  "Radius- add to the Circle subclass": "半径 - 添加到圆形子类",
  "Side Length- add to the Square subclass": "边长 - 添加到正方形子类",
  "Side 2 Length - add to the Rectangle subclass": "第二边长 - 添加到矩形子类",
  "Click Save and Done to save and unclaim the ItemType.": "点击「保存并完成」以保存并取消认领 ItemType。",
  "Click the Lock Item icon to claim the form for editing.": "点击锁定图标认领表单进行编辑。",
  "Place Classification and Color on the form, like this": "将分类和颜色放在表单上，如下所示：",
  "5. Click to save your changes.": "5. 点击保存更改。",
  "Configuring History for an ItemType (Administrators Only):": "配置 ItemType 的历史记录（仅限管理员）：",
  "History Based On Life Cycle State": "基于生命周期状态的历史",
  "In the Life Cycle Editor form, select a state in the Map.": "在生命周期编辑器表单中，选择映射中的一个状态。",
  "View an Item's History Log (All Users):": "查看项目的历史日志（所有用户）：",
  "Open the form for an Item Instance.": "打开项目实例的表单。",
  "The Item History Dialog appears.": "显示项目历史对话框。",
  "The date and time when the action was performed": "执行操作的日期和时间",
  "The first name, last name of the user": "用户的姓和名",
  "The current state of the Item.": "项目的当前状态。",
  "The Generation of the item after the action was performed.": "操作执行后项目的世代号。",
  "Creating a Custom History Template (Administrators Only):": "创建自定义历史模板（仅限管理员）：",
  "Search for the History Template ItemType.": "搜索历史模板 ItemType。",
  "Enter a name in the Name field (required).": "在名称字段中输入名称（必填）。",
  "Select the LifeCycle map that you want to use.": "选择您要使用的生命周期映射。",
  "Viewing an Item's History Log (All Users):": "查看项目的历史日志（所有用户）：",
  "1. Open the form for an Item Instance.": "1. 打开项目实例的表单。",
  "The Item History dialog box appears.": "显示项目历史对话框。",
  "The following properties appear in the Item History Dialog:": "以下属性显示在项目历史对话框中：",
  "The Generation of the item after the action was performed": "操作执行后项目的世代号",
  "The created_on_tick property for the item.": "项目的创建时间戳属性。",
  "Use the following procedure to create an email:": "使用以下步骤创建电子邮件：",
  "3. Enter a name for the email in the Name field.": "3. 在名称字段中输入电子邮件名称。",
  "The alias identity of the user": "用户的别名身份",
  "String containing today&apos;s date": "包含今天日期的字符串",
  "String containing the current time": "包含当前时间的字符串",
  "An alternate identity for receipt of notification": "接收通知的替代身份",
  "Displays the structure of the selected item.": "显示选定项目的结构。",
  "Displays the revisions of an item.": "显示项目的修订。",
  "Enables you to reset access rights for an item..": "允许您重置项目的访问权限。",
  "Enables you to create a LifeCycle map for an item.": "允许您为项目创建生命周期映射。",
  "Displays the information about the specified Item.": "显示指定项目的信息。",
  "Displays permissions information for the selected Item.": "显示选定项目的权限信息。",
  "ItemType History Configuration": "ItemType 历史配置",
  "Copies the selected item to the clipboard.": "将选定项目复制到剪贴板。",
  "Displays the selected item in the Clipboard.": "在剪贴板中显示选定项目。",
  "Promotes the selected item to the next stage.": "将选定项目提升到下一阶段。",
  "The copy also appears in the Search grid": "副本也会出现在搜索网格中：",
  "100 Brickstone Square, Suite 100": "100 Brickstone 广场，100号套房",
  "Note: The Role is optional and may be left blank.": "注意：角色是可选的，可以留空。",
  "1. Enter an Identity name into the search box.": "1. 在搜索框中输入身份名称。",
  "3. Enter the appropriate information in the fields.": "3. 在字段中输入适当的信息。",
  "4. Click and to save and unclaim the item.": "4. 点击保存并取消认领项目。",
  "Name - the name of the ItemType": "名称 - ItemType 的名称",
  "Notice that the described tabs appear on the New Part form.": "注意，所述选项卡出现在新建零部件表单上。",
  "Client Style - this tab is used to customize the UI.": "客户端样式 - 此选项卡用于自定义用户界面。",
  "Implements - this tab is used to add polymorphic ItemTypes.": "实现 - 此选项卡用于添加多态 ItemType。",
  "Click and to save and unclaim the file.": "点击保存并取消认领文件。",
  "Description- the description of the map": "描述 - 映射的描述",
  "Click and to save and unclaim the item.": "点击保存并取消认领项目。",
  "Right click on the state from which you wish to create a transition.": "右键点击您希望创建转换的状态。",
  "Right click on the J-100002 row and click Versions in the context menu that appe": "右键点击 J-100002 行，然后在出现的上下文菜单中点击「版本」。",
  "Right click on the Item Type and select Properties. You will see a dialog that d": "右键点击 ItemType 并选择「属性」。您将看到一个显示该 ItemType 所有属性的对话框。",
  "Once you save the item, the form and properties are refreshed to reflect the pro": "保存项目后，表单和属性会刷新以反映刚刚进行的更改。",
  "Once you select the SpecDocument, and save the item, then the Spec ID (the Forei": "选择 SpecDocument 并保存项目后，SpecID（外部属性）将自动填充，且为只读。",
  "Click on Date and Time to change the date and time, the time zone, or to add clo": "点击日期和时间以更改日期和时间、时区或添加时钟：",
  "Click on this link to configure e-mail, in order to notify specified identities ": "点击此链接配置电子邮件，以便在分配活动时通知指定的身份。",
  "If you specify Standard Mode, the item you select appears in a separate window.": "如果指定标准模式，您选择的项目将在独立窗口中显示。",
  "In the Navigation pane, select Administration --&gt; Forms. Click the Search For": "在导航窗格中，选择 管理 --&gt; 表单。点击「搜索表单」。",
  "In the Navigation pane, select Administration&gt;Forms&gt;Search Forms. Search f": "在导航窗格中，选择 管理&gt;表单&gt;搜索表单。搜索要修改的表单。",
  "Notice that the Color property is inherited by all subclasses, because it was de": "注意，颜色属性被所有子类继承，因为它是在父级别定义的。",
  "Notice that there are nine tabs defined, with the labels shown in the center col": "注意，定义了九个选项卡，标签显示在中心列中。",
  "If you are setting the E-Signature for the first time, leave the Old Signature f": "如果是首次设置电子签名，请将旧签名字段留空。",
  "If you are a member of the Managed By Identity, the Add and Delete Icons will be": "如果您是管理人身份的成员，添加和删除图标将处于活动状态。",
  "The following properties are displayed in the Item History Dialog:": "以下属性显示在项目历史对话框中：",
  "The following examples show how these properties work to together to create a se": "以下示例展示了这些属性如何协同工作以创建安全消息：",
  "Select one or more actions from the search dialog and click the green arrow.": "从搜索对话框中选择一个或多个操作，然后点击绿色箭头。",
  "Select one or more actions from the search dialog and click the toolbar button. ": "从搜索对话框中选择一个或多个操作，然后点击工具栏按钮。",
  "Search for the ItemType you wish to modify. In this example, we will use the Ite": "搜索您要修改的 ItemType。在此示例中，我们将使用 ItemType 来演示。",
  "Select the LifeCycle map that you want to use.": "选择您要使用的生命周期映射。",
  "Select the appropriate file and click Open. A message similar to the following a": "选择适当的文件并点击打开。将出现类似以下的消息。",
  "Displays the action that was performed on the item. See 'History Actions' (above": "显示对项目执行的操作。参见「历史操作」（上文）。",
  "Displays the structure of the selected item.": "显示选定项目的结构。",
  "Displays the revisions of an item.": "显示项目的修订信息。",
  "3. Enter a name for the email in the Name field.": "3. 在名称字段中输入电子邮件名称。",
  "3. Enter the appropriate information in the form fields and click the Save Item ": "3. 在表单字段中输入适当的信息，然后点击「保存项目」按钮。",
  "4. Click OK to save the changes in the Share Forum dialog box or click Cancel to": "4. 点击确定保存共享论坛对话框中的更改，或点击取消退出。",
  "Promotes the selected item to the next stage.": "将选定项目提升到下一阶段。",
  "Promotes the Item to the next available state, defined in the assigned Lifecycle": "将项目提升到分配的生命周期中定义的下一个可用状态。",
  "Promotes the Relationship Item and NOT the child or related Item.": "提升的是关系项目，而非子项目或关联项目。",
  "Name - the name of the revision.": "名称 - 修订版本的名称。",
  "Name - the name of the sequence item.": "名称 - 序列项目的名称。",
  "Enter the Name of the list and the Description.": "输入列表的名称和描述。",
  "Enter the Presentation Configuration name and click .": "输入显示配置名称并点击。",
  "Enter the vault name, URL pattern, and URL in the appropriate fields.": "在相应字段中输入保险库名称、URL 模式和 URL。",
  "Save, Unlock, and Close the list.": "保存、解锁并关闭列表。",
  "Save, Unlock, and Close the Permission.": "保存、解锁并关闭权限。",
  "Save, Unlock, and Close the relationship ItemType.": "保存、解锁并关闭关系 ItemType。",
  "2. Click the Navigate button to see the Navigate menu and select History.": "2. 点击「导航」按钮查看导航菜单，然后选择「历史」。",
  "2. Click the New Email button to create your own email. A row is added to the di": "2. 点击「新建电子邮件」按钮创建自己的电子邮件。将向显示网格添加一行。",
  "2. Click Create New List. A blank List window appears.": "2. 点击「创建新列表」。将出现空白列表窗口。",
  "Click to save and close the ItemType.": "点击保存并关闭 ItemType。",
  "Click to save and unclaim the item.": "点击保存并取消认领项目。",
  "Click to save and unclaim this item.": "点击保存并取消认领此项目。",
  "Enables you to retrieve/view item": "允许您检索/查看项目",
  "Enables you to edit existing item": "允许您编辑现有项目",
  "Enables user to create instances of the item": "允许用户创建项目的实例",
  "Click on Comment to save the markup comment.": "点击评论保存批注评论。",
  "Enables you to search Items with properties of value greater than the value spec": "允许您搜索属性值大于搜索条件中指定值的项目。",
  "Enables you to search for properties that explicitly have a NULL value in the da": "允许您搜索数据库中明确具有 NULL 值的属性。",
  "Enables you to search any properties that have a value that has been set.": "允许您搜索已设置值的任何属性。",
  "Properties of type Date display a Date Picker form when you click on the Criteri": "日期类型的属性在您点击条件时会显示日期选择器表单。",
  "Displays Part ItemTypes created after 1/1/2014 AND modified before 5/14/2014.": "显示在2014/1/1之后创建且在2014/5/14之前修改的零部件 ItemType。",
  "The Search dialog is displayed.": "显示搜索对话框。",
  "Specify the search criteria and click .": "指定搜索条件并点击。",
  "Select the Item from the grid and click .": "从网格中选择项目并点击。",
  "Inserts a radio button element": "插入单选按钮元素",
  "Toggles display of the Relationship tabs.": "切换关系选项卡的显示。",
  "Displays the Just Ask Innovator online help.": "显示「询问 Innovator」在线帮助。",
  "There are two tabs, Aliases and Read Vaults:": "有两个选项卡，别名和读取保险库：",
  "Deletes any selected notations.": "删除选定的批注。",
  "Switches from the Basic Toolbar to the Standard Toolbar.": "从基本工具栏切换到标准工具栏。",
  "Status displays the current promotion status.": "状态显示当前提升状态。",
  "Review displays a message in more detail.": "审核以更详细的格式显示消息。",
  "Allows you to turn Discussions on or off.": "允许您开启或关闭讨论功能。",
  "Allows you to select the search mode.": "允许您选择搜索模式。",
  "Removes the applied search criteria.": "删除已应用的搜索条件。",
  "Deletes the selected saved search from the main grid.": "从主网格中删除选定的保存搜索。",
  "Displays the previous page in the main grid.": "显示主网格中的上一页。",
  "Displays the next page in the main grid.": "显示主网格中的下一页。",
  "Making a Text Property Required": "将文本属性设为必填",
  "A field to group other fields together": "将其他字段分组在一起的字段",
  "A fields that enables the user to select an image": "允许用户选择图像的字段",
  "A filed that enables the user to classify the item": "允许用户对项目进行分类的字段",
  "6. Click to save and unclaim the list.": "6. 点击保存并取消认领列表。",
  "6. Click to save the ItemType.": "6. 点击保存 ItemType。",
  "6. Click to save and unclaim the ItemType.": "6. 点击保存并取消认领 ItemType。",
  "5. Click t osave and unclaim the item.": "5. 点击保存并取消认领项目。",
  "Click the Save, Unlock, CloseClick the": "点击保存、解锁、关闭",
  "Note: Messages of type \"History\" cannot be erased.": "注意：「历史」类型的消息无法删除。",
  "Six users have flagged the message but not the creator.": "六位用户标记了该消息，但不包括创建者。",
  "A Show replies link is displayed again below the message.": "消息下方会再次显示「显示回复」链接。",
  "Enables you create a new ItemType.": "允许您创建新的 ItemType。",
  "Add, Delete or Edit assignments": "添加、删除或编辑分配",
  "Enter color names, color values, and the sort order.": "输入颜色名称、颜色值和排序顺序。",
  "9. Click to save the item and unclaim it.": "9. 点击保存项目并取消认领。",
  "Click to open the item for editing.": "点击打开项目进行编辑。",
  "Click to save and close the item.": "点击保存并关闭项目。",
  "Create a new Forum and send messages from the Forum": "创建新论坛并从论坛发送消息",
  "Search messages based on Bookmarks": "基于书签搜索消息",
  "Follow messages from a particular User or a Forum.": "关注来自特定用户或论坛的消息。",
  "This includes the following panels:": "这包括以下面板：",
  "Enter information in the following fields:": "在以下字段中输入信息：",
  "SharePoint User - the account that you have designated for this library connecti": "SharePoint 用户 - 您为此库连接指定的帐户",
  "SharePoint User Password - password for the user above": "SharePoint 用户密码 - 上述用户的密码",
  "SharePoint User Domain - SharePoint may require that the user's domain is provid": "SharePoint 用户域 - SharePoint 可能要求提供用户的域",
  "Select an identity and click the Return Selected button to add it to the Replica": "选择一个身份并点击「返回所选」按钮将其添加到复制列表中。",
  "Select an activity in the Workflow Map Editor which will be used to initiate the": "在工作流映射编辑器中选择一个活动，用于启动子流程。",
  "You can specify the values of the Item&apos;s properties you want to conduct the": "您可以指定要执行搜索的项目属性值。",
  "You can click Make New Folder to create new folders on the client machine.": "您可以点击「新建文件夹」在客户端机器上创建新文件夹。",
  "Find the Database for the Innovator to be updated. Please be certain that this i": "找到要更新的 Innovator 数据库。请确保这是正确的数据库。",
  "Find the table called ItemType": "找到名为 ItemType 的表",
  "Open an ItemType in a Structure Browser. For detailed steps, refer to To open an": "在结构浏览器中打开 ItemType。有关详细步骤，请参阅在结构浏览器中打开 ItemType。",
  "Indicates one or more files to be viewed with the 3D viewer .": "指示一个或多个要使用3D查看器查看的文件。",
  "Indicates one or more files to be viewed with the PDF viewer .": "指示一个或多个要使用 PDF 查看器查看的文件。",
  "Indicates one or more files to be viewed with the Image viewer .": "指示一个或多个要使用图像查看器查看的文件。",
  "Pick two points to create a measurement.": "选择两个点创建测量。",
  "Pick two planar faces to create an angular measurement.": "选择两个平面创建角度测量。",
  "BLUE – Indicates no Item found with matching structure.": "蓝色 - 表示未找到匹配结构的项目。",
  "Members of this identity decide whether or not proposed changes to a software pr": "此身份的成员决定是否批准对软件产品的拟议更改。",
  "Members of this Identity have control over various aspects of the workflow of PR": "此身份的成员控制 PR 工作流的各个方面。",
  "Members of this Identity have control over some aspects of the workflow of the E": "此身份的成员控制 ECR 工作流的某些方面。"
};

// Merge all translations
const merged = { ...allTranslations, ...additionalTranslations };

// Read SQL file
const sql = fs.readFileSync(path.join(__dirname, 'INSERT_系统管理详解_full.sql'), 'utf-8');

// Replace English paragraphs with translations
let replaced = 0;
const result = sql.replace(/<Run xml:lang="zh-cn">([^<]+)<\/Run>/g, (match, text) => {
  // Skip if already has Chinese
  if (/[一-鿿]/.test(text)) return match;
  // Skip metadata
  if (text.startsWith('分类:') || text.startsWith('来源:')) return match;
  // Skip very short or non-English
  if (text.length < 4) return match;

  // Look up translation
  if (merged[text]) {
    replaced++;
    return `<Run xml:lang="zh-cn">${merged[text]}</Run>`;
  }

  // Try trimmed version
  const trimmed = text.trim();
  if (merged[trimmed]) {
    replaced++;
    return `<Run xml:lang="zh-cn">${merged[trimmed]}</Run>`;
  }

  return match;
});

fs.writeFileSync(path.join(__dirname, 'INSERT_系统管理详解_full.sql'), result);
console.log(`Replaced ${replaced} English paragraphs with Chinese translations.`);

// Count remaining English
const remaining = [...result.matchAll(/<Run xml:lang="zh-cn">([^<]+)<\/Run>/g)]
  .filter(m => !/[一-鿿]/.test(m[1]) && m[1].length > 4 && !m[1].startsWith('分类') && !m[1].startsWith('来源'))
  .length;
console.log(`Remaining English paragraphs: ${remaining}`);
