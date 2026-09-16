/* Run with NODE_PATH pointing to a Playwright installation. Uses the installed Edge browser. */
const { chromium } = require('playwright');
const assert = require('node:assert/strict');
const http = require('node:http');
const fs = require('node:fs');
const path = require('node:path');
const root = path.resolve(__dirname, '../../src/ArasToolkit.App.WinUI/Assets/FormEditor');
const output = path.resolve(__dirname, '../../.codex/screenshots');
const server = http.createServer((req,res) => {
  const name = req.url.split('?')[0].slice(1) || 'index.html';
  if (!['index.html','editor.css','editor.js'].includes(name)) {res.writeHead(404).end();return;}
  res.setHeader('Content-Type', name.endsWith('.js')?'text/javascript':name.endsWith('.css')?'text/css':'text/html');
  res.end(fs.readFileSync(path.join(root,name)));
});
const field = (id,type,x,y,extra={}) => ({id,values:{name:id,label:id,field_type:type,x:String(x),y:String(y),display_length:'150',display_length_unit:'px',...extra}});
let passed=0;
const pass=name=>{passed++;console.log('PASS '+name);};
(async()=>{
  await new Promise(resolve=>server.listen(0,'127.0.0.1',resolve));
  const browser=await chromium.launch({channel:'msedge',headless:true});
  try {
    const page=await browser.newPage({viewport:{width:1180,height:750}});
    await page.addInitScript(()=>{
      window.__messages=[];
      window.addEventListener('editor-message',event=>window.__messages.push(event.detail));
    });
    await page.route('https://form-resources.local/**',route=>route.fulfill({status:404,body:''}));
    await page.goto(`http://127.0.0.1:${server.address().port}/index.html`);
    const fixture={revision:1,editable:true,zoom:100,formId:'root',form:{width:'850',height:'350'},body:{css:'.special{color:rgb(255,0,0)}'},
      containerProperty:'container',nestedFormProperty:'form',imageProperty:'image',selected:[],properties:[],nestedForms:{},
      fields:[field('编号','text',50,50,{font_color:'#123abc'}),field('名称','text',250,120),field('隐藏字段','label',450,50,{is_visible:'0'}),
        field('width_HTML','html',10,20,{z_index:'-1',label:'基础信息',html_code:'<div style="width:820px;height:290px;border:1px solid #aaa"></div>'}),
        field('安全预览','html',50,220,{html_code:'<div class="special">静态内容</div><script>parent.__compromised=true</script><img src="missing.png" onerror="parent.__compromised=true">'})]};
    await page.evaluate(f=>window.formEditor.setState(f),fixture);
    await page.waitForFunction(()=>document.querySelectorAll('.handle').length===5);
    const frame=page.frames().find(frame=>frame.parentFrame());
    assert.equal(await frame.locator('[id="编号"]').evaluate(node=>getComputedStyle(node).color),'rgb(18, 58, 188)');
    assert.equal(await frame.locator('.special').evaluate(node=>getComputedStyle(node).color),'rgb(255, 0, 0)');
    assert.equal(await page.locator('.hidden-field').count(),1);
    assert.equal(await frame.locator('#width_HTML').innerText(),'基础信息');
    assert.equal(await page.evaluate(()=>!!window.__compromised),false);
    assert.equal(await frame.locator('script,[onerror]').count(),0);
    pass('HTML、Body CSS、自定义颜色与隐藏控件呈现，原脚本隔离');
    assert.equal(await page.locator('#boundary').evaluate(node=>node.style.height),'350px');
    const a=await page.locator('.handle[data-id="编号"]').evaluate(node=>{const r=node.getBoundingClientRect();return {x:r.x,y:r.y,width:r.width,height:r.height};});
    await page.mouse.click(a.x+20,a.y+15);
    assert.equal(await page.locator('.handle.anchor').getAttribute('data-id'),'编号');
    const b=await page.locator('.handle[data-id="名称"]').evaluate(node=>{const r=node.getBoundingClientRect();return {x:r.x,y:r.y,width:r.width,height:r.height};});
    await page.keyboard.down('Control');await page.mouse.click(b.x+20,b.y+15);await page.keyboard.up('Control');
    assert.equal(await page.locator('.selected').count(),2);
    assert.equal(await page.locator('.anchor').getAttribute('data-id'),'编号');
    pass('Ctrl 多选保留首选基准');
    await page.evaluate(()=>window.__messages=[]);
    await page.mouse.move(a.x+20,a.y+15);await page.mouse.down();await page.mouse.move(a.x+80,a.y+55,{steps:5});await page.mouse.up();
    let messages=await page.evaluate(()=>window.__messages);
    const movement=messages.filter(message=>message.kind==='move').at(-1);
    assert.deepEqual(movement.points.map(point=>[point.id,point.x,point.y]),[['编号',110,90],['名称',310,160]]);
    assert.equal(messages.filter(message=>message.kind==='dragStart').length,1);
    assert.equal(messages.filter(message=>message.kind==='dragEnd').length,1);
    pass('组合拖动按逻辑像素移动，单次手势起止正确');
    for(const point of movement.points){const item=fixture.fields.find(field=>field.id===point.id);item.values.x=String(point.x);item.values.y=String(point.y);}
    fixture.revision++;fixture.zoom=150;fixture.selected=['编号'];
    await page.evaluate(f=>window.formEditor.setState(f),fixture);
    await page.evaluate(()=>{document.getElementById('viewport').scrollLeft=40;document.getElementById('viewport').scrollTop=35;window.__messages=[];});
    await page.evaluate(()=>new Promise(resolve=>requestAnimationFrame(()=>requestAnimationFrame(resolve))));
    const scaled=await page.locator('.handle[data-id="编号"]').evaluate(node=>{const r=node.getBoundingClientRect();return {x:r.x,y:r.y,width:r.width,height:r.height};});
    await page.mouse.move(scaled.x+15,scaled.y+15);await page.mouse.down();await page.mouse.move(scaled.x+60,scaled.y+45,{steps:3});await page.mouse.up();
    messages=await page.evaluate(()=>window.__messages);
    const scaledMovement=messages.filter(message=>message.kind==='move').at(-1);
    assert.equal(scaledMovement.points[0].x,140);assert.equal(scaledMovement.points[0].y,110);
    assert.equal(await page.locator('#boundary').evaluate(node=>node.style.height),'350px');
    pass('150% 缩放并滚动后拖动坐标无偏差，窗体高度保持不变');
    fixture.zoom=100;fixture.selected=[];fixture.revision++;
    await page.evaluate(f=>{window.formEditor.setState(f);document.getElementById('viewport').scrollTop=0;document.getElementById('viewport').scrollLeft=0;},fixture);
    const scene=await page.locator('#scene').evaluate(node=>{const r=node.getBoundingClientRect();return {x:r.x,y:r.y,width:r.width,height:r.height};});
    await page.mouse.move(scene.x+1,scene.y+1);await page.mouse.down();await page.mouse.move(scene.x+480,scene.y+215,{steps:3});await page.mouse.up();
    assert.equal(await page.locator('.selected').count(),2);
    pass('空白区域框选控件');
    fixture.fields.push(field('静态字段','text',0,0,{positioning:'static'}));fixture.revision++;
    await page.evaluate(f=>window.formEditor.setState(f),fixture);
    await page.evaluate(()=>window.__messages=[]);
    const flow=await page.locator('.handle[data-id="静态字段"]').evaluate(node=>{const r=node.getBoundingClientRect();return {x:r.x,y:r.y,width:r.width,height:r.height};});
    await page.mouse.move(flow.x+20,flow.y+15);await page.mouse.down();await page.mouse.move(flow.x+70,flow.y+55);await page.mouse.up();
    assert.equal(await page.evaluate(()=>window.__messages.filter(message=>message.kind==='dragStart').length),0);
    pass('静态定位控件不被转换为绝对定位');
    fixture.fields.push(field('分组','groupbox',50,410,{width:'300',height:'130',legend:'扩展属性'}));
    fixture.fields.push(field('分组子项','text',15,25,{container:'分组'}));
    fixture.fields.push(field('嵌套','nested form',450,400,{form:'nested'}));
    fixture.nestedForms.nested={formId:'nested',form:{width:'250',height:'140'},fields:[field('内层字段','text',20,25),field('循环','nested form',20,70,{form:'nested'})]};
    fixture.revision++;await page.evaluate(f=>window.formEditor.setState(f),fixture);
    assert.equal(await frame.locator('[id="分组"] fieldset [id="分组子项"]').count(),1);
    assert.equal(await page.locator('.handle[data-id="内层字段"]').count(),0);
    assert.match(await frame.locator('[id="嵌套"]').innerText(),/循环引用/);
    pass('分组坐标与只读嵌套呈现，循环引用显示占位');
    fixture.nestedFormProperty='';fixture.nestedPreviewIds={'嵌套':'nested','循环':'nested'};
    fixture.fields[0].values.css='{$this field rule} .sys_f_label { text-decoration: underline; }';
    fixture.fields[0].values.bg_color='#abcdef';fixture.revision++;
    await page.evaluate(f=>window.formEditor.setState(f),fixture);
    assert.match(await frame.locator('[id="嵌套"]').innerText(),/循环引用/);
    assert.equal(await frame.locator('[id="编号"]').evaluate(node=>getComputedStyle(node).backgroundColor),'rgb(171, 205, 239)');
    assert.equal(await frame.locator('[id="编号"] .sys_f_label').evaluate(node=>getComputedStyle(node).textDecorationLine),'underline');
    pass('BL 背景色、官方 Field CSS 宏及绑定嵌套预览映射');
    await page.route('https://form-resources.local/**/theme.css',route=>route.fulfill({status:200,contentType:'text/css',body:'.sys_f_label { letter-spacing: 2px; }'}));
    fixture.serverUrl='https://example.invalid/Innovator';fixture.form.stylesheet='../styles/theme.css';fixture.form.css='.sys_f_value { opacity: .9; }';fixture.revision++;
    await page.evaluate(f=>window.formEditor.setState(f),fixture);
    await frame.waitForFunction(()=>getComputedStyle(document.querySelector('.sys_f_label')).letterSpacing==='2px');
    assert.equal(await frame.locator('[id="编号"] .sys_f_value').evaluate(node=>getComputedStyle(node).opacity),'0.9');
    pass('Form 外部样式表与内联 CSS 同时加载');
    await page.evaluate(()=>window.__messages=[]);
    await page.locator('#overlay').dispatchEvent('drop',{clientX:200,clientY:200,dataTransfer:await page.evaluateHandle(()=>{
      const data=new DataTransfer();data.setData('text/plain',JSON.stringify({formEditor:true,fieldType:'label'}));return data;
    })});
    assert.equal(await page.evaluate(()=>window.__messages.at(-1)?.kind),'addDrop');
    pass('工具箱拖入产生新增意图与逻辑坐标');
    fixture.editable=false;fixture.revision++;await page.evaluate(f=>window.formEditor.setState(f),fixture);
    await page.evaluate(()=>window.__messages=[]);
    const readonly=await page.locator('.handle[data-id="编号"]').evaluate(node=>{const r=node.getBoundingClientRect();return {x:r.x,y:r.y,width:r.width,height:r.height};});
    await page.mouse.move(readonly.x+10,readonly.y+10);await page.mouse.down();await page.mouse.move(readonly.x+50,readonly.y+50);await page.mouse.up();
    assert.equal(await page.evaluate(()=>window.__messages.filter(message=>message.kind==='dragStart').length),0);
    pass('忙碌、断线或只读状态禁止画布拖动');
    assert.equal(await page.evaluate(()=>window.getSelection().toString()),'');
    fixture.editable=true;fixture.selected=['编号','名称'];fixture.revision++;
    await page.evaluate(f=>window.formEditor.setState(f),fixture);
    fs.mkdirSync(output,{recursive:true});await page.screenshot({path:path.join(output,'form-editor-canvas.png')});
    fixture.fields.push(field('富文本','formatted text',40,340,{textarea_rows:'180',textarea_cols:'460',font_color:'#ff0000'}));
    fixture.revision++;await page.evaluate(f=>window.formEditor.setState(f),fixture);
    const rich = frame.locator('[id="富文本"] .sys_f_value > *');
    assert.equal(await rich.evaluate(node=>getComputedStyle(node).width),'460px');
    assert.equal(await rich.evaluate(node=>getComputedStyle(node).height),'180px');
    assert.equal(await frame.locator('[id="富文本"] .sys_f_label').evaluate(node=>getComputedStyle(node).color),'rgb(255, 0, 0)');
    pass('FormattedText 预览同步行列尺寸与标题颜色');
    console.log(`All ${passed} canvas regression cases passed.`);
  } finally {await browser.close();server.close();}
})().catch(error=>{console.error(error);server.close();process.exitCode=1;});
