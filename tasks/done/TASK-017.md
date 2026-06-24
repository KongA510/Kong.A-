---
id: TASK-017
priority: P0
type: bug
created: 2026-06-25
source: Claude Code (analysis)
status: done
---

# TASK-017: ArasLoginWindow crash on open + empty error log

## Symptom

Click "Aras Login" menu -> window crashes immediately. Error log file is empty.

## Root Cause

ArasLoginWindow.xaml lines 114/116/118 use `{StaticResource StringToVisibility}` and `{StaticResource BoolToVisibility}` converters. These were only defined locally in individual UserControl Resources (like DataImportView.xaml), never registered globally in App.xaml.

ArasLoginWindow is a `Window` (not a UserControl). During `InitializeComponent()` XAML parsing, `{StaticResource StringToVisibility}` cannot be resolved -> **XamlParseException** crashes immediately. Code never reaches ViewModel construction or ErrorLogService. That's why error log is empty - crash at WPF framework level, far before app-level exception handling.

## Fix

Register all three Converters globally in App.xaml:

1. Add namespace: `xmlns:converters="clr-namespace:ArasToolkit.App.Converters"`

2. Register in Application.Resources:
```xml
<converters:BoolToVisibilityConverter x:Key="BoolToVisibility" />
<converters:StringToVisibilityConverter x:Key="StringToVisibility" />
<converters:InverseBoolConverter x:Key="InverseBool" />
```

**Important**: The `<ResourceDictionary.MergedDictionaries>` element MUST be the FIRST child of `<ResourceDictionary>`. Put Converters and SolidColorBrush AFTER MergedDictionaries.

File: `src/ArasToolkit.App/App.xaml`

## Verification

- dotnet build ArasToolkit.slnx - 0 errors
- Launch app -> click "Aras Login" -> window opens normally
- Other views using same converters (DataImportView etc.) still work

---

> Note: This fix does not affect existing functionality. Converter x:Key names match all view references.

