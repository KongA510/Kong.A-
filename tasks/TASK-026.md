---
id: TASK-026
priority: P1
type: fix
created: 2026-06-25
source: Claude Code
status: pending
---

# 修复 StringToVisibilityConverter 不支持 ConverterParameter=Invert

## 问题描述

`DataImportView.xaml:109` 行：

```xml
<TextBlock Text="请先加载预览" FontSize="14"
    Foreground="{StaticResource TextSecondaryBrush}"
    HorizontalAlignment="Center" VerticalAlignment="Center"
    Visibility="{Binding PreviewData, Converter={StaticResource StringToVisibility}, 
                 ConverterParameter=Invert}" />
```

- `PreviewData` 类型为 `DataTable?`（非 string），`StringToVisibilityConverter.Convert` 对非 string 类型默认返回 Visibility.Visible（因为 `value?.ToString()` 返回类型名而非空字符串）
- **该转换器不支持 `ConverterParameter=Invert`**，`Invert` 参数被完全忽略
- 后果：PreviewData 为 null 时占位文本 Collapsed（期待 Visible → 无提示）；有数据时占位文本反而显示（遮盖 DataGrid）

## 涉及文件

| 文件 | 说明 |
|------|------|
| `src/ArasToolkit.App/Converters/Converters.cs` :35-46 | StringToVisibilityConverter — 需添加 Invert 参数支持 + 非字符串类型处理 |
| `src/ArasToolkit.App/Views/DataImportView.xaml` :109 | 占位文本绑定 — 需确认修复后行为正确 |

## 修复方案

修改 `StringToVisibilityConverter.Convert` 方法：

```csharp
public class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isEmpty;

        if (value is string s)
            isEmpty = string.IsNullOrEmpty(s);
        else
            isEmpty = value == null;

        bool invert = parameter?.ToString() == "Invert";
        return (isEmpty ^ invert) ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

### 改动点

1. 区分 string 类型和 null 检查 — 对于 DataTable 等非 string 类型，只检查是否为 null
2. 添加 `Invert` 参数支持 — 与 `BoolToVisibilityConverter` 保持一致的参数约定

### 修复后行为

- `PreviewData == null`, `ConverterParameter=Invert` → **Visible**（显示"请先加载预览"）
- `PreviewData != null`, `ConverterParameter=Invert` → **Collapsed**（隐藏占位文本）
- 不带 Invert 参数的行为不变（向后兼容）

## 联动检查清单

- [x] Converters.cs — StringToVisibilityConverter 修改
- [x] DataImportView.xaml — 验证占位文本行为（无需修改 XAML 本身）
- [x] 其他使用 StringToVisibilityConverter 的地方不受影响（均未使用 Invert 参数）

## 编译验证

- [ ] `dotnet build ArasToolkit.slnx` 通过
- [ ] 无新增 Warning
- [ ] 数据汇入页面：未加载预览时显示"请先加载预览"，加载后隐藏

## 修复结果（Codex 填写）

- 修复状态: [success / partial / failed]
- 编译结果: [pass / fail]
- 备注: [Codex 填写的修复说明]


## Git 提交规范

```
修复完成后执行:
1. git add -A && git status  (检查变更文件)
2. git commit -m "fix: StringToVisibilityConverter 添加 Invert 参数支持 + 非字符串 null 检查"
3. git push origin master --force  (推送前确保 develop 已备份)
```
