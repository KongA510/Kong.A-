/* The outer page owns input. Untrusted form HTML/CSS lives in a sandbox without scripts.
   The only host messages are explicit editor operations, never AML or executable content. */
(() => {
  'use strict';
  const $ = id => document.getElementById(id);
  const frame = $('preview'), overlay = $('overlay'), scene = $('scene'), viewport = $('viewport');
  let state = null, selection = [], handles = new Map(), drag = null, scale = 1, ready = false;
  let revision = 0, viewportOrigin = { x: 50, y: 50 }, pendingState = null;
  const num = (value, fallback = 0) => Number.isFinite(Number(value)) && value !== '' && value != null ? Number(value) : fallback;
  const post = (kind, data = {}) => {
    const message = { kind, revision, ...data };
    if (window.chrome?.webview) window.chrome.webview.postMessage(message);
    else window.dispatchEvent(new CustomEvent('editor-message', { detail: message }));
  };
  const value = (item, key, fallback = '') => item?.values?.[key] ?? fallback;
  const escape = text => String(text).replace(/[&<>"']/g, c => ({ '&':'&amp;', '<':'&lt;', '>':'&gt;', '"':'&quot;', "'":'&#39;' }[c]));
  const validPosition = field => ['', 'absolute'].includes(value(field, 'positioning', 'absolute').toLowerCase());
  const container = field => state.containerProperty ? value(field, state.containerProperty) : '';
  function insertionContext(target) {
    const containers = [...handles.values()].filter(entry => value(entry.field,'field_type') === 'groupbox').map(entry => {
      const element = entry.node.querySelector('fieldset'), rect = element.getBoundingClientRect();
      return { name:value(entry.field,'name'), x:rect.left + element.clientLeft, y:rect.top + element.clientTop };
    });
    const selected = handles.get(target?.closest('.handle')?.dataset.id || selection[0]);
    const containerName = selected ? value(selected.field,'field_type') === 'groupbox' ? value(selected.field,'name') : container(selected.field) : '';
    return {containers,containerName};
  }
  function resourceUrl(source) {
    if (!source || !state?.serverUrl) return source;
    try {
      const root = new URL(state.serverUrl.replace(/\/$/, '') + '/');
      const resolved = new URL(source, new URL('Client/Forms/', root));
      if (resolved.origin === root.origin && resolved.pathname.startsWith(root.pathname + 'Client/'))
        return 'https://form-resources.local/' + resolved.pathname.slice(root.pathname.length) + resolved.search;
    } catch { /* Invalid or external URLs remain blocked by the preview CSP. */ }
    return source;
  }
  function resourceCss(css) {
    return css.replace(/url\(\s*(['"]?)([^)'"\s]+)\1\s*\)/gi, (_, quote, url) => `url("${resourceUrl(url).replace(/"/g,'%22')}")`)
      .replace(/@import\s+(['"])(.*?)\1/gi, (_, quote, url) => `@import "${resourceUrl(url).replace(/"/g,'%22')}"`);
  }
  function logical(event) {
    const rect = scene.getBoundingClientRect();
    return { x: (event.clientX - rect.left) / scale, y: (event.clientY - rect.top) / scale };
  }
  function setZoom(zoom) {
    scale = Math.max(.5, Math.min(2, num(zoom, 100) / 100));
    scene.style.transform = `scale(${scale})`;
    $('extent').style.width = `${parseFloat(scene.style.width || 1000) * scale}px`;
    $('extent').style.height = `${parseFloat(scene.style.height || 600) * scale}px`;
  }
  function setSelection(ids, notify = false) {
    selection = [...new Set(ids)].filter(id => handles.has(id));
    for (const [id, entry] of handles) {
      entry.handle.classList.toggle('selected', selection.includes(id));
      entry.handle.classList.toggle('anchor', selection[0] === id);
    }
    if (selection.length) {
      $('guide').style.top = handles.get(selection[0]).handle.style.top;
      $('guide').hidden = false;
    } else $('guide').hidden = true;
    if (notify) post('selection', { ids: selection });
  }
  // No HTML sanitizer is relied on as the isolation boundary: iframe sandbox + CSP enforce it.
  // Removing active elements also gives predictable static layout and useful diagnostics.
  function inertHtml(doc, html) {
    const template = doc.createElement('template'); template.innerHTML = html;
    let dynamic = false;
    template.content.querySelectorAll('script,iframe,object,embed,base,meta,link,form').forEach(element => {
      dynamic = true;
      if (element.tagName === 'FORM') element.replaceWith(...element.childNodes); else element.remove();
    });
    template.content.querySelectorAll('*').forEach(element => {
      [...element.attributes].forEach(attribute => {
        if (/^on/i.test(attribute.name) || ['srcdoc', 'action', 'formaction'].includes(attribute.name)) {
          element.removeAttribute(attribute.name); dynamic = true;
        }
        if (['src','href','xlink:href'].includes(attribute.name) && /^\s*javascript:/i.test(attribute.value)) element.removeAttribute(attribute.name);
      });
      if (element.tagName === 'A') element.removeAttribute('href');
      for (const name of ['src','href','xlink:href']) if (element.hasAttribute(name)) element.setAttribute(name, resourceUrl(element.getAttribute(name)));
      if (element.hasAttribute('style')) element.setAttribute('style', resourceCss(element.getAttribute('style')));
      if (element.tagName === 'STYLE') element.textContent = resourceCss(element.textContent);
    });
    return { fragment: template.content, dynamic };
  }
  function placeholder(doc, label) {
    const box = doc.createElement('div'); box.textContent = label;
    box.style.cssText = 'padding:8px;border:1px dashed #999;background:#f5f5f5;color:#777;font:12px Segoe UI;min-width:100px;min-height:30px';
    return box;
  }
  function renderForm(doc, definition, destination, prefix, ancestors, editable) {
    const fields = definition.fields || [], nodes = new Map();
    const warnings = [];
    for (const field of fields) {
      const v = field.values, type = value(field, 'field_type', 'text'), name = value(field, 'name', field.id);
      const outer = doc.createElement('div');
      outer.id = prefix + field.id; outer.className = `aras-field sys_f_container sys_ft_${type.replace(/[^a-z0-9_-]/g, '_')} sys_fn_${name} ${type.replace(/[^a-z0-9_-]/g, '-')}`;
      outer.setAttribute('name', name); outer.dataset.fieldId = editable ? field.id : '';
      const rawCss = value(field, 'css') || value(field, 'field_css');
      if (rawCss) {
        const css = resourceCss(rawCss).replaceAll('{$this field rule}', `#${CSS.escape(outer.id)}`);
        if (css.includes('{')) { const style = doc.createElement('style'); style.textContent = css; destination.append(style); }
        else outer.style.cssText = css;
      }
      const position = value(field, 'positioning', 'absolute') || 'absolute';
      outer.style.position = ['absolute','relative','static'].includes(position) ? position : 'absolute';
      if (position !== 'static') { outer.style.left = `${num(v.x)}px`; outer.style.top = `${num(v.y)}px`; }
      outer.style.zIndex = String(num(v.z_index));
      outer.style.fontFamily = value(field, 'font_family', 'Arial, Helvetica, sans-serif');
      outer.style.fontSize = value(field, 'font_size', '8pt');
      outer.style.fontWeight = value(field, 'font_weight', 'normal');
      outer.style.fontStyle = value(field, 'font_style', 'normal');
      outer.style.color = value(field, 'font_color', '#333');
      if (v.bg_color || v.background_color) outer.style.backgroundColor = v.bg_color || v.background_color;
      if (v.border_width) { outer.style.borderWidth = `${num(v.border_width)}px`; outer.style.borderStyle = value(field, 'border_style', 'solid'); outer.style.borderColor = value(field, 'border_color', '#888'); }
      if (v.width) outer.style.width = `${num(v.width, 150)}px`;
      if (v.height) outer.style.height = `${num(v.height, 30)}px`;
      if (v.is_visible === '0') outer.style.opacity = '.42';
      const label = doc.createElement('label'); label.className = 'aras-field__label sys_f_label';
      label.textContent = value(field, 'label', name); label.style.display = 'block';
      label.style.textAlign = value(field, 'text_align', 'left');
      const input = doc.createElement('div'); input.className = 'aras-field__value sys_f_value';
      const length = num(v.display_length, 150);
      const unit = ['px','em','%','in','cm','mm','ex','pt','pc'].includes(v.display_length_unit) ? v.display_length_unit : 'px';
      input.style.width = `${length}${unit}`;
      const property = (definition.properties || []).find(property => property.id === v.propertytype_id);
      let control;
      switch (type) {
        case 'label': control = doc.createElement('span'); control.textContent = value(field, 'label', name); label.textContent = ''; break;
        case 'button': control = doc.createElement('button'); control.textContent = value(field, 'label', name); label.textContent = ''; break;
        case 'checkbox': control = doc.createElement('input'); control.type = 'checkbox'; break;
        case 'textarea': control = doc.createElement('textarea'); control.style.width = `${num(v.textarea_cols, length)}px`; control.style.height = `${num(v.textarea_rows, 100)}px`; control.placeholder = property?.label || name; break;
        case 'dropdown': case 'color list': case 'listbox single select': case 'listbox multi select':
          control = doc.createElement('select');
          if (type.startsWith('listbox')) control.size = 4;
          if (type === 'listbox multi select') control.multiple = true;
          for (const option of property?.options?.length ? property.options : [{label:'选择…',value:''}]) {
            const element = doc.createElement('option'); element.textContent = option.label || option.value; control.append(element);
          } break;
        case 'radio button list': case 'checkbox list':
          control = doc.createElement('div');
          for (const option of property?.options?.length ? property.options : [{label:'选项',value:''}]) {
            const row = doc.createElement('label'), check = doc.createElement('input');
            check.type = type === 'checkbox list' ? 'checkbox' : 'radio'; row.append(check, doc.createTextNode(option.label || option.value)); control.append(row);
          } break;
        case 'html': {
          control = doc.createElement('div');
          const html = inertHtml(doc, value(field, 'html_code'));
          control.append(html.fragment);
          if (html.dynamic) warnings.push(`${name} 含动态内容，仅显示静态部分`);
          if (!control.childNodes.length) control.append(placeholder(doc, 'HTML · 空内容'));
          break;
        }
        case 'groupbox':
          control = doc.createElement('fieldset'); control.style.minWidth = `${num(v.width, length)}px`; control.style.minHeight = `${num(v.height, 90)}px`;
          control.style.position = 'relative';
          const legend = doc.createElement('legend'); legend.textContent = value(field, 'legend', value(field, 'label', name));
          control.append(legend); label.textContent = ''; break;
        case 'nested form': {
          const id = state.nestedFormProperty ? value(field, state.nestedFormProperty) : state.nestedPreviewIds?.[field.id] || '';
          const nested = state.nestedForms?.[id];
          if (ancestors.has(id)) control = placeholder(doc, '嵌套窗体 · 循环引用');
          else if (!nested || nested.responsive) control = placeholder(doc, '嵌套窗体不可预览');
          else {
            control = doc.createElement('div'); control.style.cssText = `position:relative;width:${num(nested.form?.width,400)}px;height:${num(nested.form?.height,200)}px;overflow:hidden;border:1px dashed #aaa`;
            renderForm(doc, nested, control, prefix + id + '-', new Set([...ancestors,id]), false);
          } break;
        }
        case 'image': {
          const source = state.imageProperty ? value(field, state.imageProperty) : property?.defaultValue || '';
          if (!source) control = placeholder(doc, '图片 · 数据绑定预览');
          else {
            control = doc.createElement('img'); control.src = resourceUrl(source); control.alt = '图片无法加载';
            control.style.maxWidth = `${num(v.width, length)}px`; if (v.height) control.style.height = `${num(v.height)}px`;
          } break;
        }
        case 'formatted text': control = placeholder(doc, '富文本 · 静态预览'); control.style.width = `${num(v.textarea_cols, length)}px`; control.style.height = `${num(v.textarea_rows, 100)}px`; break;
        case 'class structure': control = placeholder(doc, '分类结构 · 选择分类…'); break;
        case 'item': case 'file item': case 'date': case 'color': case 'ml_string':
          control = doc.createElement('div'); control.className = 'compound';
          const text = doc.createElement('input'); text.type = 'text'; text.placeholder = property?.label || name;
          const picker = doc.createElement('button'); picker.textContent = type === 'date' ? '▦' : type === 'color' ? '■' : '…';
          control.append(text,picker); break;
        default:
          if (!['text','password',''].includes(type)) { control = placeholder(doc, `控件 ${type} · 静态占位`); warnings.push(`控件 ${type} 仅显示占位`); }
          else { control = doc.createElement('input'); control.type = type === 'password' ? 'password' : 'text'; control.placeholder = property?.label || name; }
      }
      if (v.is_disabled === '1') { if ('disabled' in control) control.disabled = true; control.querySelectorAll('input,button,select,textarea').forEach(child => child.disabled = true); }
      input.append(control);
      const labelPosition = value(field, 'label_position', 'top');
      if (['left','right'].includes(labelPosition)) { outer.style.display = 'flex'; outer.style.alignItems = 'center'; outer.style.gap = '6px'; }
      if (['right','bottom'].includes(labelPosition)) outer.append(input,label); else outer.append(label,input);
      if (labelPosition === 'none' || labelPosition === 'hidden') label.style.display = 'none';
      nodes.set(name, { outer, control, field }); destination.append(outer);
    }
    // Group children only within the current Body; malformed cycles remain visible at root.
    for (const entry of nodes.values()) {
      const parentName = container(entry.field); if (!parentName) continue;
      const visited = new Set([value(entry.field,'name')]); let next = parentName, valid = true;
      while (next) { if (visited.has(next)) { valid = false; break; } visited.add(next); next = nodes.has(next) ? container(nodes.get(next).field) : ''; }
      const parent = nodes.get(parentName);
      if (valid && parent && value(parent.field,'field_type') === 'groupbox') parent.control.append(entry.outer);
    }
    return warnings;
  }
  function render() {
    if (!state || !ready) return;
    const doc = frame.contentDocument;
    if (!doc?.body) return;
    doc.body.replaceChildren();
    doc.querySelectorAll('[data-config-style]').forEach(style => style.remove());
    const body = state.body || {};
    const stylesheet = state.form?.stylesheet || '';
    if (stylesheet && !/[{}<>]/.test(stylesheet)) {
      const link = doc.createElement('link'); link.rel = 'stylesheet'; link.href = resourceUrl(stylesheet); link.dataset.configStyle = '';
      link.addEventListener('load', rebuildHandles);
      link.addEventListener('error', () => { $('notice').textContent = '窗体样式表无法加载，来源配置已保留。'; });
      doc.head.append(link);
    }
    for (const css of [state.form?.css, body.css || body.body_css, /[{}]/.test(stylesheet) ? stylesheet : '']) {
      if (!css) continue;
      const style = doc.createElement('style'); style.dataset.configStyle = '';
      style.textContent = resourceCss(css); doc.head.append(style);
    }
    doc.body.style.backgroundColor = body.bg_color || body.background_color || 'white';
    if (body.style) doc.body.style.cssText += body.style;
    const warnings = renderForm(doc, state, doc.body, '', new Set([state.formId]), true);
    const width = Math.max(1,num(state.form?.width,850)), height = Math.max(1,num(state.form?.height,350));
    const maxX = Math.max(width,...(state.fields || []).map(field => num(value(field,'x')) + num(value(field,'width'),num(value(field,'display_length'),150)) + 200));
    const maxY = Math.max(height,...(state.fields || []).map(field => num(value(field,'y')) + num(value(field,'height'),200)));
    scene.style.width = `${maxX + 100}px`; scene.style.height = `${maxY + 100}px`;
    frame.style.width = scene.style.width; frame.style.height = scene.style.height;
    $('boundary').style.width = `${width}px`; $('boundary').style.height = `${height}px`;
    $('boundary').querySelector('span').textContent = `${width} × ${height} px`;
    setZoom(state.zoom); rebuildHandles(); setSelection(state.selected || []);
    $('notice').textContent = state.responsive ? '响应式窗体不可编辑' :
      warnings.length ? [...new Set(warnings)].join('；') : '静态 HTML/CSS 预览 · 业务脚本不运行 · 拖动后点击「保存修改」写回';
    doc.querySelectorAll('img').forEach(img => img.addEventListener('error', () => { $('notice').textContent = '部分图片资源无法加载，来源配置已保留。'; }));
    requestAnimationFrame(rebuildHandles);
  }
  function rebuildHandles() {
    const doc = frame.contentDocument;
    if (!doc || !state) return;
    overlay.querySelectorAll('.handle').forEach(handle => handle.remove()); handles.clear();
    for (const field of state.fields || []) {
      const node = doc.getElementById(field.id); if (!node) continue;
      const rect = node.getBoundingClientRect(); const handle = document.createElement('div');
      handle.dataset.id = field.id; handle.className = 'handle';
      handle.style.cssText = `left:${rect.left}px;top:${rect.top}px;width:${Math.max(rect.width,24)}px;height:${Math.max(rect.height,22)}px;z-index:${Math.max(0,num(value(field,'z_index')))+2}`;
      // Keep decoration/group backgrounds behind field hit targets regardless of DOM order.
      if (['html','groupbox'].includes(value(field,'field_type'))) handle.style.zIndex = '1';
      handle.classList.toggle('hidden-field', value(field,'is_visible','1') === '0');
      handle.classList.toggle('flow', !validPosition(field)); handle.classList.toggle('readonly', !state.editable);
      handle.title = `${value(field,'label',value(field,'name'))} · X=${value(field,'x','0')} Y=${value(field,'y','0')}`;
      overlay.append(handle); handles.set(field.id, {field,node,handle});
    }
    setSelection(selection.length ? selection : state.selected || []);
  }
  function canDrag() {
    const entries = selection.map(id => handles.get(id));
    return state?.editable && entries.length && entries.every(entry => validPosition(entry.field)) && new Set(entries.map(entry => container(entry.field))).size === 1;
  }
  overlay.addEventListener('pointerdown', event => {
    if (event.button !== 0 || !state) return;
    event.preventDefault(); // Read-only/static selection must not select the iframe as browser text.
    const point = logical(event); const id = event.target.closest('.handle')?.dataset.id;
    viewport.focus({preventScroll:true}); viewportOrigin = { x: Math.max(0,Math.round(point.x)), y: Math.max(0,Math.round(point.y)) };
    if (id) {
      if (event.ctrlKey) { setSelection(selection.includes(id) ? selection.filter(value => value !== id) : [...selection,id], true); return; }
      if (!selection.includes(id)) setSelection([id], true);
      if (!canDrag()) return;
      drag = { kind:'move', start:point, moved:false, entries:selection.map(id => {
        const entry = handles.get(id); return {id, x:num(value(entry.field,'x')),y:num(value(entry.field,'y'))};
      })};
      post('dragStart');
    } else {
      const initial = event.ctrlKey ? [...selection] : [];
      setSelection(initial,true); drag = {kind:'select',start:point,initial}; $('marquee').hidden = false;
    }
    overlay.setPointerCapture(event.pointerId); event.preventDefault();
  });
  overlay.addEventListener('pointermove', event => {
    if (!drag) return;
    const point = logical(event), dx = point.x-drag.start.x, dy = point.y-drag.start.y;
    if (drag.kind === 'select') {
      const x = Math.min(point.x,drag.start.x), y = Math.min(point.y,drag.start.y), w=Math.abs(dx), h=Math.abs(dy);
      $('marquee').style.cssText=`left:${x}px;top:${y}px;width:${w}px;height:${h}px`;
      const ids = [...handles].filter(([,entry]) => {
        const r=entry.node.getBoundingClientRect();return r.left>=x&&r.top>=y&&r.right<=x+w&&r.bottom<=y+h;
      }).map(([id])=>id);
      setSelection([...drag.initial,...ids]); return;
    }
    const minX = Math.min(...drag.entries.map(entry => entry.x)), minY=Math.min(...drag.entries.map(entry=>entry.y));
    const deltaX=Math.max(-minX,Math.round(dx)),deltaY=Math.max(-minY,Math.round(dy));
    const points=drag.entries.map(entry=>({id:entry.id,x:entry.x+deltaX,y:entry.y+deltaY}));
    for (const point of points) { const entry=handles.get(point.id);entry.node.style.left=`${point.x}px`;entry.node.style.top=`${point.y}px`; }
    drag.moved = true; drag.points=points;
    // Include the displayed positions of descendants when a group moves.
    for (const entry of handles.values()) { const rect=entry.node.getBoundingClientRect();entry.handle.style.left=`${rect.left}px`;entry.handle.style.top=`${rect.top}px`; }
    setSelection(selection); post('move', {points});
  });
  function finish(cancel=false) {
    if (!drag) return;
    const previous=drag;drag=null;$('marquee').hidden=true;
    if(previous.kind==='move') post('dragEnd',{cancel});else setSelection(selection,true);
    if (pendingState) { const next=pendingState;pendingState=null;setState(next); }
  }
  overlay.addEventListener('pointerup',()=>finish());
  overlay.addEventListener('pointercancel',()=>finish(true));
  overlay.addEventListener('lostpointercapture',()=>finish(true));
  viewport.addEventListener('keydown',event=>{
    if(event.key==='Escape'){finish(true);event.preventDefault();}
    if(event.key==='Delete'){post('delete');event.preventDefault();}
    if(event.ctrlKey&&['z','y'].includes(event.key.toLowerCase())){post(event.key.toLowerCase()==='z'&&!event.shiftKey?'undo':'redo');event.preventDefault();}
  });
  viewport.addEventListener('scroll',()=>{
    viewportOrigin={x:Math.max(0,Math.round(viewport.scrollLeft/scale+40)),y:Math.max(0,Math.round(viewport.scrollTop/scale+40))};
  });
  overlay.addEventListener('dragover', event => {
    if (!state?.editable) return;
    event.preventDefault(); event.dataTransfer.dropEffect = 'copy';
  });
  overlay.addEventListener('drop', event => {
    if (!state?.editable) return;
    event.preventDefault();
    try {
      const data = JSON.parse(event.dataTransfer.getData('text/plain'));
      if (data.formEditor !== true) return;
      const point = logical(event);
      post('addDrop', { propertyId: typeof data.propertyId === 'string' ? data.propertyId : '',
        fieldType: typeof data.fieldType === 'string' ? data.fieldType : '',
        x: Math.max(0, Math.round(point.x)), y: Math.max(0, Math.round(point.y)), ...insertionContext(event.target) });
    } catch { /* Ignore unrelated clipboard data. */ }
  });
  function setState(next) {
    if(drag){pendingState=next;return;}
    state=next;revision=next.revision;selection=next.selected||[];
    if(ready)render();
  }
  frame.addEventListener('load',()=>{ready=true;render();});
  frame.srcdoc=`<!doctype html><html><head><meta charset="utf-8"><meta http-equiv="Content-Security-Policy" content="default-src 'none'; style-src 'unsafe-inline' https://form-resources.local; img-src data: https://form-resources.local; font-src https://form-resources.local; script-src 'none'; form-action 'none'; base-uri https://form-resources.local"><base href="https://form-resources.local/Client/Forms/"><style>
    html,body{margin:0;padding:0;position:relative;font:12px Arial;background:white;min-width:100%;min-height:100%}*{box-sizing:border-box}input:not([type=checkbox]):not([type=radio]),select,textarea{width:100%;min-height:23px;border:1px solid #a0a0a0;border-radius:2px;padding:2px 5px;font:12px Arial;color:#333;background:#fff}input[type=checkbox],input[type=radio]{width:14px;height:14px}button{min-height:23px;border:1px solid #aaa;background:#f3f3f3;border-radius:2px;color:#333}textarea{resize:none}.compound{display:flex}.compound input{flex:1;min-width:20px}.compound button{width:25px}.aras-field__label{min-height:14px}.aras-field__value>label{display:block}fieldset{margin:0;border:1px solid #aaa;padding:12px;position:relative}legend{padding:0 4px}
  </style></head><body></body></html>`;
  window.formEditor={setState,setZoom,setSelection,requestInsert:()=>post('insertPoint',{...viewportOrigin,...insertionContext()}),
    clear:()=>{state=null;handles.clear();overlay.querySelectorAll('.handle').forEach(handle=>handle.remove());if(frame.contentDocument?.body)frame.contentDocument.body.replaceChildren();$('notice').textContent='选择已有窗体后在此预览';},
    getPoint:()=>viewportOrigin};
  post('ready');
})();
