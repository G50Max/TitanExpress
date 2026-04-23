# 泰坦快递 (Titan Express) 开发对话记录

## 项目概述

泰坦快递是一款末世物流策略游戏，采用 MonoGame 框架开发，包含 GUI 界面、资源管理系统、基站部署、订单处理等核心功能。

---

## 开发过程

### 1. 初期设置与编译问题

- 用户请求将游戏改为全 GUI 界面，不要命令窗口
- 解决 .NET SDK 和 MonoGame Content Builder (mgcb) 路径问题
- 使用完整路径调用 dotnet 和 mgcb 命令

### 2. 字体问题

- 初始字体加载失败，报错 `ContentLoadException`
- 用户反馈："游戏没有退出只是文字全是问号"
- 解决方案：将字体从 Consolas 改为 SimHei 以支持中文字符

### 3. UI 布局问题

- 用户反馈："显示分类的区域被挤没了"
- 解决方案：实现屏幕自动缩放，根据屏幕宽度调整 UI 元素大小
- 用户反馈："电池，信誉，跟钱显示错位了，重叠到可视化状态条了"
- 解决方案：添加独立的资源条显示区域

### 4. 按钮文字问题

- 用户反馈日志旁边的 4 个按钮没有文字
- 原因：文字绘制顺序错误，被矩形背景覆盖
- 解决方案：先绘制背景矩形，再绘制文字
- 调整文字位置以居中显示

---

## 关键代码片段

### Game1.cs - 按钮绘制

```csharp
private void DrawButtons(float scale, int screenHeight)
{
    int btnY = screenHeight - 80;
    
    Color deployColor = _resources.Battery >= 10 ? Color.Green : Color.Red;
    _spriteBatch.Draw(_pixel, new Rectangle((int)(80 * scale), btnY, (int)(100 * scale), 25), deployColor);
    DrawBorder((int)(80 * scale), btnY, (int)(100 * scale), 25, 2, Color.White);
    _spriteBatch.DrawString(_font, "部署基站", new Vector2(85 * scale, btnY + 4), Color.White);

    bool hasPendingOrder = _orderManager.GetPendingOrders().Count > 0;
    Color acceptColor = hasPendingOrder ? Color.Green : Color.Gray;
    _spriteBatch.Draw(_pixel, new Rectangle((int)(200 * scale), btnY, (int)(100 * scale), 25), acceptColor);
    DrawBorder((int)(200 * scale), btnY, (int)(100 * scale), 25, 2, Color.White);
    _spriteBatch.DrawString(_font, "接受订单", new Vector2(205 * scale, btnY + 4), Color.White);

    _spriteBatch.Draw(_pixel, new Rectangle((int)(320 * scale), btnY, (int)(100 * scale), 25), Color.Blue);
    DrawBorder((int)(320 * scale), btnY, (int)(100 * scale), 25, 2, Color.White);
    _spriteBatch.DrawString(_font, "系统状态", new Vector2(325 * scale, btnY + 4), Color.White);

    _spriteBatch.Draw(_pixel, new Rectangle((int)(440 * scale), btnY, (int)(80 * scale), 25), Color.Purple);
    DrawBorder((int)(440 * scale), btnY, (int)(80 * scale), 25, 2, Color.White);
    _spriteBatch.DrawString(_font, "清空", new Vector2(450 * scale, btnY + 4), Color.White);
}
```

### 字体配置 - TerminalFont.spritefont

```xml
<?xml version="1.0" encoding="utf-8"?>
<XnaContent xmlns:Graphics="Microsoft.Xna.Framework.Content.Pipeline.Graphics">
  <Asset Type="Graphics:FontDescription">
    <FontName>SimHei</FontName>
    <Size>14</Size>
    <Spacing>0</Spacing>
    <UseKerning>true</UseKerning>
    <Style>Regular</Style>
    <CharacterRegions>
      <CharacterRegion>
        <Start>&#32;</Start>
        <End>&#126;</End>
      </CharacterRegion>
      <CharacterRegion>
        <Start>&#19968;</Start>
        <End>&#40869;</End>
      </CharacterRegion>
    </CharacterRegions>
  </Asset>
</XnaContent>
```

---

## 文件结构

```
TitanExpress/
├── .config/
│   └── dotnet-tools.json
├── .idea/
├── .vscode/
│   └── launch.json
├── Content/
│   ├── Content.mgcb
│   ├── TerminalFont.spritefont
│   ├── bin/
│   └── obj/
├── Models/
│   ├── BaseStation.cs
│   ├── GameResources.cs
│   └── Order.cs
├── Systems/
│   ├── OrderManager.cs
│   ├── StationManager.cs
│   └── Terminal.cs
├── UI/
│   ├── Button.cs
│   ├── MapView.cs
│   ├── OrderPanel.cs
│   ├── Panel.cs
│   ├── ResourceBar.cs
│   ├── ResourceDisplay.cs
│   ├── StationPanel.cs
│   └── TerminalView.cs
├── Game1.cs
├── Program.cs
├── TitanExpress.csproj
├── .gitignore
└── README.md
```

---

## GitHub 上传

- 初始化 Git 仓库
- 创建 .gitignore 文件排除编译产物
- 提交所有源代码文件
- 推送到 https://github.com/G50Max/TitanExpress

---

## 注意事项

### 开发环境设置

1. **测试前先确认游戏是否成功启动**
   - 不要一直盯着错误代码死循环修改
   - 先运行游戏，确认是否成功打开窗口
   - 如果游戏能运行但有 UI 问题，再针对性修复

2. **编译命令**
   - 使用完整路径：`& "C:\Program Files\dotnet\dotnet.exe" build`
   - 运行时：`& "C:\Program Files\dotnet\dotnet.exe" run`

3. **字体问题排查**
   - 如果文字显示为方块/问号，检查字体是否支持中文（改用 SimHei）
   - 确保 Content.mgcb 正确配置了字体文件

4. **UI 布局调试**
   - 先用小幅度数值调整，不要一次性改太多
   - 每次修改后运行游戏确认效果

---

## 用户反馈汇总

1. "不能用像素，我感觉很难看" - 改用 GUI
2. "ok你现在最先要解决的是没有文字" - 解决字体问题
3. "游戏没有退出只是文字全是问号" - 改用 SimHei 字体
4. "显示分类的区域被挤没了" - 实现屏幕自适应
5. "电池，信誉，跟钱显示错位了" - 调整布局
6. "还是跟上面一样错位了，就是文字在该呆的位置偏左" - 修复文字位置
7. "不是错位是你压根没写字" - 修复按钮文字绘制顺序
8. "往右下偏了几十个像素" - 调整按钮文字位置

---

*对话时间: 2026年4月*
