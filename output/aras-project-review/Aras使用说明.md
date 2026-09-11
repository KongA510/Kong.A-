# 项目立项评审表 · Aras 静态 HTML

按用户提供的图一制作。绿色框是区域标记，成品采用黑色细边框；固定标签保留，数据位置为空。未收到图二，因此未猜测属性名称或字段类型。

## 文件

- `aras-project-review.txt`：用编辑器打开，全选复制到 Aras 的 HTML Code；与 HTML 片段内容完全相同。
- `aras-project-review.html`：同一份粘贴片段，只有 `<style>` 和表格容器，无外部依赖，无 JavaScript。
- `preview.html`：浏览器预览页面。直接打开后调整窗口大小查看效果；不要把预览文件的整页标签贴进字段。

## 在 Aras 中放置表格

1. 在目标 Form 中添加一个 HTML 类型字段，例如命名为 `project_review_layout`，不显示该字段自身的标签。
2. 将 `aras-project-review.txt` 全部内容复制到该字段的 HTML Code。
3. 表格设置了 `width:100%`，会填满所在容器。HTML 字段的外层容器也必须能够变宽；否则表格只能跟随固定大小的容器。
4. 对 Classic Form，可在此 HTML 字段的 **Field CSS** 中设置以下容器样式。顶部位置请按实际窗体调整；这段样式只设置该字段，不要粘到所有字段共用的样式里。

```css
/* 左右各留 12px；不要再给宿主字段设置固定宽度。 */
left: 12px !important;
width: calc(100% - 24px) !important;
max-width: none !important;
min-width: 0 !important;
height: auto !important;
box-sizing: border-box;
```

若 HTML 字段外还嵌套固定宽度容器，须一并解除该容器的固定宽度。不同版本的表单生成结构可能不同，需要在实际窗体确认。Responsive Form 则让承载区域占满可用列宽。

## 放入属性字段

每个空位都是一个独立 `<div>`，例如：

```html
<div id="apr-project-name" class="apr-slot" data-slot="project-name">
  <!-- 项目名称：在此放入对应属性字段 -->
</div>
```

- `id` 和 `data-slot` 是本模板定义的位置编号，**不是 Aras 属性绑定语法**。注释不会显示。
- 你可以在空位内放入自己已有的、经过属性绑定的控件。静态 HTML 自身不读写或保存 Aras 数据。
- 若在 Classic Form 设计器中把原生字段按 X/Y 坐标覆盖到空位上，表格缩放时这些字段不会自动跟随。要让字段与表格一起移动，需通过窗体事件把真实控件接入对应空位，并保留原有绑定、编辑权限和事件；这部分要依据图二以及实际字段 Name/属性名配置。
- 原图“□通过 / □存在问题”的位置也留为空位，可放对应的 List、单选或其他已绑定控件。没有画不能保存数据的假复选框。
- 长内容会让行高自然增加。表格采用百分比列宽、固定表格布局和容器查询，窄窗体内文字换行，不使用整页缩放或绝对坐标。
- 如需加大默认空位，将 CSS 顶部的 `--apr-row-height: 44px` 和 `--apr-field-height: 30px` 调大。较大控件请实际核对其内部最小宽度。
- 表单编号 `JSAB-TD-FR-22016`、版本 `A.0` 按截图抄录，可直接修改。

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

- [Aras 官方文档：Classic Form 使用 X/Y 坐标布局，Responsive Form 自适应布局](https://docs.aras.com/aras-innovator-platform-33/introduction-to-responsive-forms/0000019f-1799-dcff-a7bf-5f99a4540000)
- [Aras Labs：Field CSS 应用于字段外层容器](https://www.aras.com/community/f/getting-started/3840/how-to-format-field-css/1852)

已在本机 Chrome 验证表格容器宽度 1050px、710px、366px：边框对齐，文字换行，无横向溢出；插入长中文及连续英文内容后也未撑破单元格。已检查 53 个空位、唯一 ID、四组跨行合并及每行列数。截图见 preview-1100.png、preview-760.png、preview-390.png。

交付内容为静态布局，浏览器验证不能代替真实 Aras 窗体联调。未连接或修改 Aras 实例。
