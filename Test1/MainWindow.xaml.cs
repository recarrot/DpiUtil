using DpiUtil;
using System;
using System.Windows;
using System.Windows.Media;

namespace Test1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // 初始化 DPI 管理和窗口缩放管理
            this.Loaded += (s, e) =>
            {
                DpiManager.Initialize(this);
                WindowScaleManager.Initialize(this);
                UpdateDisplayInfo();
            };
            
            // 监听窗口尺寸变化
            this.SizeChanged += (s, e) => UpdateDisplayInfo();
            
            // 监听窗口缩放比例变化
            WindowScaleManager.ScaleChanged += UpdateDisplayInfo;
        }
        
        /// <summary>
        /// 更新显示信息
        /// </summary>
        private void UpdateDisplayInfo()
        {
            // 更新 DPI 信息
            var source = PresentationSource.FromVisual(this);
            if (source?.CompositionTarget != null)
            {
                double dpiScaleX = source.CompositionTarget.TransformToDevice.M11;
                double dpiScaleY = source.CompositionTarget.TransformToDevice.M22;
                int dpiX = (int)(dpiScaleX * 96);
                int dpiY = (int)(dpiScaleY * 96);
                
                DpiInfoText.Text = string.Format("DPI: {0}x{1} (缩放: {2}%, {3}%)", 
                    dpiX, dpiY, 
                    Math.Round(dpiScaleX * 100), 
                    Math.Round(dpiScaleY * 100));
            }
            
            // 更新窗口尺寸信息
            WindowSizeInfoText.Text = string.Format("窗口尺寸: {0}x{1} 像素 | 设计基准: {2}x{3} @ {4}%", 
                Math.Round(this.ActualWidth), 
                Math.Round(this.ActualHeight),
                (int)WindowScaleManager.DesignWidth,
                (int)WindowScaleManager.DesignHeight,
                Math.Round(WindowScaleManager.DesignDpiScale * 100));
            
            // 更新窗口标题
            this.Title = string.Format("DpiUtil 示例窗口 - {0}x{1} px | 基准: {2}x{3} @ {4}%", 
                Math.Round(this.ActualWidth), 
                Math.Round(this.ActualHeight),
                (int)WindowScaleManager.DesignWidth,
                (int)WindowScaleManager.DesignHeight,
                Math.Round(WindowScaleManager.DesignDpiScale * 100));
        }
    }
}
