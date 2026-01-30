# DpiUtil

DpiUtil 是一个专为 WPF 应用程序开发的 DPI 感知与响应式布局库

## 功能特性

### DPI 感知支持
- 启用 PerMonitorV2 DPI 感知模式
- 实时获取当前屏幕 DPI 信息
- 自动适应不同显示器的 DPI 设置

### 响应式布局
- **响应式尺寸**：`ResponsiveWidth`、`ResponsiveHeight`
- **响应式字体大小**：`ResponsiveFontSize`
- **响应式边距**：`ResponsiveMargin`
- **响应式内边距**：`ResponsivePadding`
- **响应式圆角**：`ResponsiveCornerRadius`

### 灵活的设计基准
- 默认以 1080P (1920×1080 @ 100%) 为设计基准
- 支持预设基准分辨率：720P、1080P、2K、4K
- 支持自定义设计尺寸与 DPI 缩放比例

### 转换器支持
- `DpiScaleConverter`：基于 DPI 缩放的绑定转换器
- `WindowScaleConverter`：基于窗口尺寸比例的绑定转换器


## 快速上手

### 1. 启用 DPI 感知

在 `App.xaml.cs` 中启用 PerMonitorV2 DPI 感知模式：

```csharp
protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);
    DpiManager.EnablePerMonitorV2DpiAwareness();
}
```

### 2. 设置设计基准（可选）

在 `App.xaml.cs` 中设置设计基准，默认为 1080P：

```csharp
protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);
    DpiManager.EnablePerMonitorV2DpiAwareness();

    // 使用预设基准
    WindowScaleManager.Set1080PBase(); // 默认 1920×1080 @ 100%
    // WindowScaleManager.Set720PBase();
    // WindowScaleManager.Set2KBase();
    // WindowScaleManager.Set4KBase();

    // 或自定义基准
    // WindowScaleManager.SetDesignBase(1366, 768, 1.25);
}
```

### 3. 初始化窗口管理

在窗口的 `Loaded` 事件中进行初始化：

```csharp
this.Loaded += (s, e) =>
{
    DpiManager.Initialize(this);
    WindowScaleManager.Initialize(this);
};
```

### 4. 在 XAML 中使用响应式属性

```xaml
<Window xmlns:dpi="clr-namespace:DpiUtil;assembly=DpiUtil">
    <Grid>
        <!-- 响应式尺寸 -->
        <Border
            dpi:DpiScaleBehavior.ResponsiveWidth="300"
            dpi:DpiScaleBehavior.ResponsiveHeight="150"
            Background="#3498db">
            
            <!-- 响应式字体 -->
            <TextBlock
                dpi:DpiScaleBehavior.ResponsiveFontSize="14"
                Text="响应式布局示例" />
        </Border>

        <!-- 响应式边距与内边距 -->
        <Button
            dpi:DpiScaleBehavior.ResponsiveMargin="20,10,20,10"
            dpi:DpiScaleBehavior.ResponsivePadding="15,8"
            dpi:DpiScaleBehavior.ResponsiveFontSize="16"
            Content="响应式按钮" />

        <!-- 响应式圆角 -->
        <Border
            dpi:DpiScaleBehavior.ResponsiveCornerRadius="15"
            dpi:DpiScaleBehavior.ResponsivePadding="20"
            Background="#2ecc71">
            <TextBlock Text="带圆角的响应式边框" />
        </Border>
    </Grid>
</Window>
```

## 使用示例

### 基本响应式元素

```xaml
<Border
    dpi:DpiScaleBehavior.ResponsiveWidth="200"
    dpi:DpiScaleBehavior.ResponsiveHeight="100"
    Background="#3498db">
    <TextBlock
        dpi:DpiScaleBehavior.ResponsiveFontSize="14"
        Text="200×100 响应式边框" />
</Border>
```

### 带圆角的响应式边框

```xaml
<Border
    dpi:DpiScaleBehavior.ResponsiveMargin="20,10,20,10"
    dpi:DpiScaleBehavior.ResponsivePadding="30"
    dpi:DpiScaleBehavior.ResponsiveCornerRadius="15"
    Background="#2ecc71">
    <TextBlock
        dpi:DpiScaleBehavior.ResponsiveFontSize="16"
        Text="响应式边距、内边距和圆角" />
</Border>
```

### 使用绑定转换器

```xaml
<Window.Resources>
    <dpi:WindowScaleConverter x:Key="WindowScaleConverter" />
    <dpi:ResponsiveScaler x:Key="ResponsiveScaler" />
    <dpi:DpiWindowScaleConverter x:Key="DpiWindowScaleConverter" />
    <dpi:DpiScaleConverter x:Key="DpiScaleConverter" />
</Window.Resources>

<Button
    Width="{Binding ElementName=gdMain, Path=ActualWidth, Converter={StaticResource WindowScaleConverter}, ConverterParameter=0.5}"
    Height="{Binding ElementName=gdMain, Path=ActualHeight, Converter={StaticResource WindowScaleConverter}, ConverterParameter=0.1}"
    Content="占父容器50%宽，10%高的按钮" />
```

## API 概览

### DpiManager

```csharp
// 启用 PerMonitorV2 DPI 感知
DpiManager.EnablePerMonitorV2DpiAwareness();

// 初始化 DPI 管理
DpiManager.Initialize(Window window);
```

### WindowScaleManager

```csharp
// 初始化窗口缩放管理
WindowScaleManager.Initialize(Window window);

// 设置预设基准分辨率
WindowScaleManager.Set720PBase();    // 1280×720 @ 100%
WindowScaleManager.Set1080PBase();   // 1920×1080 @ 100% (默认)
WindowScaleManager.Set2KBase();      // 2560×1440 @ 100%
WindowScaleManager.Set4KBase();      // 3840×2160 @ 100%

// 自定义设计基准
WindowScaleManager.SetDesignBase(double width, double height, double dpiScale = 1.0);

// 获取当前缩放比例
double scale = WindowScaleManager.ScaleFactor;

// 按设计基准缩放数值
double scaledValue = WindowScaleManager.Scale(double designValue);
```

### DpiScaleBehavior 附加属性

- `ResponsiveWidth`：响应式宽度
- `ResponsiveHeight`：响应式高度
- `ResponsiveFontSize`：响应式字体大小
- `ResponsiveMargin`：响应式边距
- `ResponsivePadding`：响应式内边距
- `ResponsiveCornerRadius`：响应式圆角

### 内置转换器

- `DpiScaleConverter`：基于 DPI 缩放值的转换器
- `WindowScaleConverter`：基于窗口尺寸比例的转换器


## 系统要求

- .NET Framework 4.6 及以上版本
- WPF 应用程序
- Windows 10 及以上系统（支持 PerMonitorV2 DPI 感知）

## 注意事项

- 在应用启动时调用 `DpiManager.EnablePerMonitorV2DpiAwareness()`
- 使用响应式属性时，避免同时设置对应的固定属性（如同时设置 Width 与 ResponsiveWidth）

## 示例

项目附带了 `Test1` 示例应用，演示了 DpiUtil 的主要功能：

- 响应式尺寸、字体与间距
- 实时 DPI 与窗口尺寸信息展示
- 不同基准分辨率的适配效果
- 多种布局元素的响应式实现

## 演示效果
![screenshot_2025-12-08_15-11-48](https://github.com/user-attachments/assets/db45dfd0-46d1-4feb-9608-d1e80ca85dd4)

