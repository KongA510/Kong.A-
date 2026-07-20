const fs = require('fs');

// Short word/phrase translations
const shortDict = {
  "Item": "项目", "List": "列表", "Text": "文本", "Body": "正文",
  "like": "包含", "null": "空值", "Rows": "行", "Open": "打开",
  "Label": "标签", "When": "当", "Event": "事件", "On Due": "到期时",
  "ECR-1000": "ECR-1000", "Claim": "认领", "Name": "名称",
  "Can Add": "可添加", "HTML": "HTML", "Color": "颜色",
  "Help": "帮助", "Done": "完成", "Share": "分享", "More": "更多",
  "Print": "打印", "World": "全局", "Aras PLM": "Aras PLM",
  "Owner": "所有者", "Icon": "图标", "View": "视图",
  "Resets to previous view": "恢复为上一视图", "Find": "查找",
  "Walk": "步行", "Role": "角色", ".text": "文本",
  "&lt; AML&gt;": "<AML>",
  "AML&gt;": "AML>",
  "&lt;": "<", "&lt;=": "<=", "&gt;": ">",
  "xItemTypeAllowedProperty": "xItemTypeAllowedProperty"
};

// Medium phrase translations
const mediumDict = {
  "Other Properties display a blank text box, in which wild card searches are allowed so long as &apos;like&apos; is specified as the Operator.": "其他属性显示一个空白文本框，只要将操作符指定为「包含」，就允许进行通配符搜索。",
  "Delete any criteria that is not required by using the right-mouse button and selecting Delete Criteria from the context menu.": "使用鼠标右键并从上下文菜单中选择「删除条件」来删除不需要的条件。",
  "For each criteria row, select: an ItemType a Property an Operation a Criteria value": "对于每个条件行，选择：一个 ItemType、一个属性、一个操作、一个条件值",
  "Click the 'Search' button in the search toolbar.": "点击搜索工具栏中的「搜索」按钮。",
  "Use the 'Stop Search' and 'Clear Search' buttons in the search toolbar as required.": "根据需要使用搜索工具栏中的「停止搜索」和「清除搜索」按钮。",
  "Share... Changes sharing for the Forum. Opens the Share Forum dialog.": "分享... 更改论坛的共享设置。打开共享论坛对话框。",
  "Float - The source item (in the current state) points to the latest generation of the related item.": "浮动 - 源项目（在当前状态）指向关联项目的最新世代号。",
  "Fixed - The source item (in the current state) points to a specified generation of the related item.": "固定 - 源项目（在当前状态）指向关联项目的指定世代号。",
  "Hard Fixed - The behavior is Fixed, as described above, and cannot be changed by any subsequent states in this life cycle.": "硬固定 - 行为为固定（如上所述），不能被此生命周期中的后续状态更改。",
  "Hard Float - The behavior is Float, as described above, and cannot be changed by any subsequent states in this life cycle.": "硬浮动 - 行为为浮动（如上所述），不能被此生命周期中的后续状态更改。",
  "Note: When creating a new item, the generic form will appear until the item is saved:": "注意：创建新项目时，在项目保存之前将显示通用表单：",
  "There may be more than one choice available for a given user/group depending on configuration.": "根据配置，给定用户/组可能有多个选择可用。",
  "Make sure you have the TOC Access, Permissions, and Can Add set as desired. (see Permissions)": "请确保已按需设置了目录访问、权限和可添加。（请参阅权限）",
  "Place Classification and Color on the form, like this:": "将分类和颜色放在表单上，如下所示：",
  "Click on the History Template Action relationship tab in the lower portion of the form.": "点击表单下部的历史模板操作关系选项卡。",
  "View an Item's History Log (All Users):": "查看项目的历史日志（所有用户）：",
  "In the Form's Main Menu, click the Navigate button and select History from the dropdown menu:": "在表单的主菜单中，点击「导航」按钮并从下拉菜单中选择「历史」：",
  "A Comment entered by the user when the item is promoted (life cycle)": "用户在项目提升（生命周期）时输入的评论",
  "The Major Revision of the item after the action was performed.": "操作执行后项目的主修订号。",
  "Navigate to ItemTypes under the Administration folder in the TOC.": "导航到目录中管理文件夹下的 ItemTypes。",
  "Viewing an Item's History Log (All Users):": "查看项目的历史日志（所有用户）：",
  "The Major Revision of the item after the action was performed": "操作执行后项目的主修订号",
  "6. Select the identities that you want to receive the email and click to add them.": "6. 选择您希望接收电子邮件的身份并点击添加。",
  "5. Select user names to associate with the email and click .": "5. 选择要与电子邮件关联的用户名并点击。",
  "The recipients of the notification. The possible choices are:": "通知的接收者。可选选项有：",
  "Alternate - the identity defined in the alternate field (see below)": "替代 - 替代字段中定义的身份（见下文）",
  "So, if we wanted to access the value of the state property of the ECR, we would write:": "因此，如果我们想访问 ECR 状态属性的值，可以写为：",
  "And if we wanted to access the name of the Activity, we would write:": "如果我们想访问活动的名称，可以写为：",
  "Double click the ECN sequence in the Search grid and click . The ECN Sequence form appears:": "双击搜索网格中的 ECN 序列并点击。将出现 ECN 序列表单：",
  "Displays a tree structure that shows the relationships between items.": "显示展示项目之间关系的树形结构。",
  "Displays information about properties and actions associated with the Item.": "显示与项目关联的属性和操作的信息。",
  "Locks the related (child) Item and prevents other users from submitting changes.": "锁定关联（子）项目并阻止其他用户提交更改。",
  "The copy also appears in the Search grid:": "副本也会出现在搜索网格中：",
  "2. Select the identities that you want to share your forum with.": "2. 选择您希望共享论坛的身份。",
  "3. Click OK to save the changes or Cancel to exit from the Share with dialog box.": "3. 点击确定保存更改，或点击取消退出共享对话框。",
  "Click . The file is locked and appears in the default page in the main window.": "点击。文件被锁定并显示在主窗口的默认页面中。",
  "Click the Run Search button to see a list of available viewers.": "点击「执行搜索」按钮查看可用查看器列表。",
  "Select a Viewer from the list and click the Return Selected button.": "从列表中选择一个查看器并点击「返回所选」按钮。",
  "3. Enter the appropriate information in the form fields and click the Save Item button .": "3. 在表单字段中输入适当的信息，然后点击「保存项目」按钮。",
  "1. In the Navigation pane, select Administration&gt;Lists The following menu appears:": "1. 在导航窗格中，选择 管理&gt;列表。将出现以下菜单：",
  "3. Enter the Name of the list and the Description.": "3. 输入列表的名称和描述。",
  "4. Click the Add Row icon to add a row on the Values tab.": "4. 点击「添加行」图标在值选项卡上添加行。",
  "Value - the actual values that the property will have assigned to it, once selected by the user.": "值 - 用户选择后属性将被分配的实际值。",
  "Sort Order - the order in which these entries will appear in the drop-down box (top to bottom).": "排序顺序 - 这些条目在下拉框中出现的顺序（从上到下）。",
  "Properties - to enter new properties for an ItemType, see Entering ItemType Properties.": "属性 - 为 ItemType 输入新属性，请参阅输入 ItemType 属性。",
  "Relationship Types - to define relationships for an ItemType, see About Relationships.": "关系类型 - 为 ItemType 定义关系，请参阅关于关系。",
  "Life Cycles - Enable different Life Cycle Maps to be selected for different Classifications.": "生命周期 - 允许为不同的分类选择不同的生命周期映射。",
  "Inherited Server Events - this tab is used to add inherited server events to the ItemType.": "继承的服务器事件 - 此选项卡用于向 ItemType 添加继承的服务器事件。",
  "xProperties - This tab is used to add xProperties to the ItemType.": "xProperties - 此选项卡用于向 ItemType 添加 xProperties。",
  "Notice how ineligible Class choices are grayed out and not selectable in the case of an Aesthetic Group member in the window below.": "注意在下面的窗口中，不合格的类选项如何被灰显且不可选择（以美学组成员为例）。",
  "This user belongs to the Aesthetics Group thus is only authorized to select the Aesthetic Class Type for Inspection Results:": "此用户属于美学组，因此仅被授权为检查结果选择美学类类型：",
  "Once you save the item, the form and properties are refreshed to reflect the properties and form configured for the Class Type:": "保存项目后，表单和属性会刷新以反映为类类型配置的属性和表单：",
  "Set as Default Sets the default Bookmark that will be used when the user opens My Discussions in the future (is set to \"All Messages\" by default)": "设为默认 - 设置用户将来打开「我的讨论」时使用的默认书签（默认设为「所有消息」）",
  "Remove If a Bookmark is selected, it removes it from Bookmarks If an item or saved search inside a Forum is selected, it removes it from the Forum": "移除 - 如果选中书签则从书签中移除。如果选中论坛内的项目或保存搜索则从论坛中移除。",
  "Click on this link to configure e-mail, in order to notify specified identities that the item has entered this state. See Configuring Email.": "点击此链接配置电子邮件，以便在项目进入此状态时通知指定的身份。请参阅配置电子邮件。",
  "● ItemType Permissions Report summarizes all the Permissions showing who can get, update, delete and change permissions.": "● ItemType 权限报告汇总了所有权限，显示谁可以获取、更新、删除和更改权限。",
};

// Read SQL
const sql = fs.readFileSync('INSERT_系统管理详解_full.sql', 'utf-8');

// Apply short dict
let replaced = 0;
let result = sql.replace(/<Run xml:lang="zh-cn">([^<]+)<\/Run>/g, (match, text) => {
  if (/[一-鿿]/.test(text)) return match;
  if (text.startsWith('分类:') || text.startsWith('来源:')) return match;
  if (text.length < 2) return match;

  const trimmed = text.trim();

  // Check short dict
  if (shortDict[trimmed]) {
    replaced++;
    return `<Run xml:lang="zh-cn">${shortDict[trimmed]}</Run>`;
  }

  // Check medium dict
  if (mediumDict[trimmed] || mediumDict[text]) {
    replaced++;
    return `<Run xml:lang="zh-cn">${mediumDict[trimmed] || mediumDict[text]}</Run>`;
  }

  return match;
});

fs.writeFileSync('INSERT_系统管理详解_full.sql', result);
console.log(`Replaced ${replaced} more paragraphs.`);

// Count remaining
const remaining = [...result.matchAll(/<Run xml:lang="zh-cn">([^<]+)<\/Run>/g)]
  .filter(m => !/[一-鿿]/.test(m[1]) && m[1].length > 3 && !m[1].startsWith('分类') && !m[1].startsWith('来源'));
const uniqueRemaining = [...new Set(remaining.map(m => m[1]))];
console.log(`Remaining unique English: ${uniqueRemaining.length}`);
fs.writeFileSync('remaining_english3.json', JSON.stringify(uniqueRemaining, null, 2));
