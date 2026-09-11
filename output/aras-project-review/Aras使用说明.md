# 项目立项评审表 · 整体宽度 900px

按原图保留固定标签、合并单元格及 53 个数据空位。本版将包含左右留白的整体横向宽度控制为 900px，适配笔记本较小的窗体区域。继续使用固定布局，保留 44px 起的行高及 30px 高的空位，便于放入属性字段。

## 尺寸

| 区域 | 宽度 |
| --- | --- |
| 每个基础列 | 147px |
| 六个基础列总宽 | 882px（包含折叠外边框实际约 883px） |
| 表格外层 HTML 字段 | 900px（包含左右各 8px 留白及边框余量） |
| 顶部信息区 | 标签 147 / 数据空位 294 / 标签 147 / 数据空位 294 |
| 评审区 | 类别 294 / 检查项 294 / 评审结论 147 / 问题说明 147 |
| 普通行高 | 44px 起；长内容可增高 |

正文由 18px 调整为 14px，标题由 32px 调整为 26px。147px 单元格内可放约 130px 宽的字段；294px 合并单元格内可放约 280px 宽的字段。

900px 指整体横向宽度；表单仍可纵向滚动。有效内容区至少有 900px 时可完整显示；若窗体本身更窄或存在额外 X 偏移、侧边留白，仍需为这些空间留出位置。

## 替换现有表格

1. 打开现有 HTML 字段的 **HTML Code**，用 `aras-project-review.txt` 的全部内容替换旧代码（包含 `<style>`）。HTML 片段与该 TXT 完全相同。
2. 在该 HTML 字段的 **Field CSS** 中删除旧的宽度设置（包括 1216px 的 width/min-width），改为 `Aras字段容器样式.css` 的内容：

```css
/* 粘贴到承载表格的 HTML 字段的 Field CSS，替换旧版宽度设置。 */
width: 900px !important;
min-width: 900px !important;
max-width: 900px !important;
height: auto !important;
min-height: 1600px;
overflow: visible !important;
padding: 0 !important;
box-sizing: border-box;
```

3. 如果设计器有 Width/宽度设置，也设为 `900`，位置 X/Y 按当前窗体保留。
4. 如果表格下半部仍被截断，增加 Form 的设计高度，让内容及字段都处于可滚动区域内（本表格自身约 1600px 高，还需加上顶部 Y 偏移）。单纯增加内部表格宽度不能解除祖先容器的 `overflow:hidden` 裁切；此时需调整对应容器的滚动/高度设置。
5. 打开 `preview.html` 预览本版固定宽度布局。不要把预览文件的 `<html>`、`<head>`、`<body>` 一起贴入 Aras 字段。

上面的 Field CSS 只用于承载表格的 HTML 字段，不要应用到所有属性字段。

## 放入属性字段

每个空位保留独立编号，例如：

```html
<div id="apr-project-name" class="apr-slot" data-slot="project-name">
  <!-- 项目名称：在此放入对应属性字段 -->
</div>
```

- `id`、`data-slot` 是模板位置编号，不是 Aras 属性绑定语法；不会自动读写或保存数据。
- 可把已有的、经过属性绑定的控件放入对应空位。评审结论和问题说明均留空，方便选择实际字段类型。
- 若通过 Classic Form 设计器的 X/Y 坐标摆放原生字段，请以本版固定尺寸重新对齐。窗口变窄时表格不会收缩；后续改动行高、字号或内容换行仍可能影响下方字段的坐标。
- 表格不会自行移动原生字段。如果需要控件跟随内容增高，应将真实控件接入相应空位，并保留原有绑定与事件。
- 如需加高，修改 CSS 中 `--apr-row-height: 44px`、`--apr-field-height: 30px`。
- 如需改为其他列宽，同步修改 6 个 `<col>`、表格 width/min-width、外层 width/min-width/flex 以及 Field CSS；外层宽度等于六列总宽加 16px 留白及折叠边框余量，最终不得超过 900px。
- 固定编号 `JSAB-TD-FR-22016` 和版本 `A.0` 可按实际需求修改。

## 空位对照

| 空位 id | 对应内容 |
| --- | --- |
| `apr-review-no` | 编号 |
| `apr-project-name` | 项目名称 |
| `apr-stage-name` | 阶段名称 |
| `apr-project-owner` | 项目负责人 |
| `apr-host-department` | 主持部门 |
| `apr-review-location` | 评审地点 |
| `apr-review-date` | 评审日期 |
| `apr-g1-r1-result` | 功能和性能要求 / 是否充分 / 评审结论 |
| `apr-g1-r1-issue` | 功能和性能要求 / 是否充分 / 存在的问题或不足 |
| `apr-g1-r2-result` | 功能和性能要求 / 是否适宜 / 评审结论 |
| `apr-g1-r2-issue` | 功能和性能要求 / 是否适宜 / 存在的问题或不足 |
| `apr-g1-r3-result` | 功能和性能要求 / 要求是否完整 / 评审结论 |
| `apr-g1-r3-issue` | 功能和性能要求 / 要求是否完整 / 存在的问题或不足 |
| `apr-g1-r4-result` | 功能和性能要求 / 要求是否清楚 / 评审结论 |
| `apr-g1-r4-issue` | 功能和性能要求 / 要求是否清楚 / 存在的问题或不足 |
| `apr-g1-r5-result` | 功能和性能要求 / 所有要求是否没有相矛盾 / 评审结论 |
| `apr-g1-r5-issue` | 功能和性能要求 / 所有要求是否没有相矛盾 / 存在的问题或不足 |
| `apr-g2-r1-result` | 法律法规要求 / 是否充分 / 评审结论 |
| `apr-g2-r1-issue` | 法律法规要求 / 是否充分 / 存在的问题或不足 |
| `apr-g2-r2-result` | 法律法规要求 / 是否适宜 / 评审结论 |
| `apr-g2-r2-issue` | 法律法规要求 / 是否适宜 / 存在的问题或不足 |
| `apr-g2-r3-result` | 法律法规要求 / 要求是否完整 / 评审结论 |
| `apr-g2-r3-issue` | 法律法规要求 / 要求是否完整 / 存在的问题或不足 |
| `apr-g2-r4-result` | 法律法规要求 / 要求是否清楚 / 评审结论 |
| `apr-g2-r4-issue` | 法律法规要求 / 要求是否清楚 / 存在的问题或不足 |
| `apr-g2-r5-result` | 法律法规要求 / 所有要求是否没有相矛盾 / 评审结论 |
| `apr-g2-r5-issue` | 法律法规要求 / 所有要求是否没有相矛盾 / 存在的问题或不足 |
| `apr-g3-r1-result` | 以往可利用的设计信息 / 是否充分 / 评审结论 |
| `apr-g3-r1-issue` | 以往可利用的设计信息 / 是否充分 / 存在的问题或不足 |
| `apr-g3-r2-result` | 以往可利用的设计信息 / 是否适宜 / 评审结论 |
| `apr-g3-r2-issue` | 以往可利用的设计信息 / 是否适宜 / 存在的问题或不足 |
| `apr-g3-r3-result` | 以往可利用的设计信息 / 要求是否完整 / 评审结论 |
| `apr-g3-r3-issue` | 以往可利用的设计信息 / 要求是否完整 / 存在的问题或不足 |
| `apr-g3-r4-result` | 以往可利用的设计信息 / 要求是否清楚 / 评审结论 |
| `apr-g3-r4-issue` | 以往可利用的设计信息 / 要求是否清楚 / 存在的问题或不足 |
| `apr-g3-r5-result` | 以往可利用的设计信息 / 所有要求是否没有相矛盾 / 评审结论 |
| `apr-g3-r5-issue` | 以往可利用的设计信息 / 所有要求是否没有相矛盾 / 存在的问题或不足 |
| `apr-g4-r1-result` | 设计开发其他要求 / 是否充分 / 评审结论 |
| `apr-g4-r1-issue` | 设计开发其他要求 / 是否充分 / 存在的问题或不足 |
| `apr-g4-r2-result` | 设计开发其他要求 / 是否适宜 / 评审结论 |
| `apr-g4-r2-issue` | 设计开发其他要求 / 是否适宜 / 存在的问题或不足 |
| `apr-g4-r3-result` | 设计开发其他要求 / 要求是否完整 / 评审结论 |
| `apr-g4-r3-issue` | 设计开发其他要求 / 要求是否完整 / 存在的问题或不足 |
| `apr-g4-r4-result` | 设计开发其他要求 / 要求是否清楚 / 评审结论 |
| `apr-g4-r4-issue` | 设计开发其他要求 / 要求是否清楚 / 存在的问题或不足 |
| `apr-g4-r5-result` | 设计开发其他要求 / 所有要求是否没有相矛盾 / 评审结论 |
| `apr-g4-r5-issue` | 设计开发其他要求 / 所有要求是否没有相矛盾 / 存在的问题或不足 |
| `apr-action-advice` | 措施建议 |
| `apr-action-owner` | 措施负责人 |
| `apr-action-due-date` | 预计完成期 |
| `apr-review-summary` | 跟踪情况及评审总结 |
| `apr-lead-reviewer` | 主评人签名 |
| `apr-participants` | 参评人签名 |

## 参考与验证范围

- [Aras 官方文档：Classic Form 的 X/Y 坐标布局](https://docs.aras.com/aras-innovator-platform-33/introduction-to-responsive-forms/0000019f-1799-dcff-a7bf-5f99a4540000)
- [Aras Labs：Field CSS 应用于字段外层容器](https://www.aras.com/community/f/getting-started/3840/how-to-format-field-css/1852)

Chrome 实测外框宽度和内容滚动宽度均为 900px，含边框的表格宽度为 883px；在 900px 内容区内右侧问题栏完整显示。长文本不会撑大表格，53 个空位保留，普通空位高度仍为 30px。详细测量见 validation.json；预览截图为 preview-900.png。

本次为静态布局修正，未连接或修改 Aras 实例；实际 Aras 外层容器裁切需按窗体设置确认。
