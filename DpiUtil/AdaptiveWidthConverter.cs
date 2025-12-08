using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DpiUtil
{
    public class AdaptiveWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double windowWidth && parameter is string designWidthStr)
            {
                if (double.TryParse(designWidthStr, out double designWidth))
                {
                    // 计算缩放比例
                    double scale = windowWidth / designWidth;
                    return windowWidth;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 根据窗口宽度自动缩放长度
    /// 用法: Width="{Binding ElementName=MainWnd, Path=ActualWidth, Converter={StaticResource WindowScaleConverter}, ConverterParameter=0.5}"
    /// ConverterParameter是比例（0-1），表示占窗口宽度的百分比
    /// </summary>
    public class WindowScaleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double windowDimension && parameter is string scaleStr)
            {
                if (double.TryParse(scaleStr, out double scale))
                {
                    double result = windowDimension * scale;
                    return Math.Max(result, 10); // 最小值50
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 根据窗口尺寸按比例自动调整宽高和所有内部属性（字体、边距等）
    /// </summary>
    public class ResponsiveScaler : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 && values[0] is double windowWidth && values[1] is double windowHeight)
            {
                // values[0] = 窗口宽度
                // values[1] = 窗口高度
                // parameter = "1920,1080" (设计分辨率)

                if (parameter is string designSizeStr)
                {
                    var parts = designSizeStr.Split(',');
                    if (parts.Length == 2 &&
                        double.TryParse(parts[0], out double designWidth) &&
                        double.TryParse(parts[1], out double designHeight))
                    {
                        // 计算两个方向的缩放比，取较小值确保不溢出
                        double scaleX = windowWidth / designWidth;
                        double scaleY = windowHeight / designHeight;
                        double scale = Math.Min(scaleX, scaleY);

                        return scale;
                    }
                }
            }
            return 1.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // ============ 2. 高级DPI感知转换器 ============
    /// <summary>
    /// 同时处理DPI和窗口缩放
    /// 用法: Width="{Binding ElementName=MainWnd, Path=ActualWidth, Converter={StaticResource DpiWindowScaleConverter}, ConverterParameter=0.5:1920x1080}"
    /// ConverterParameter格式: "比例:设计分辨率宽x高"，如 "0.5:1920x1080"
    /// </summary>
    public class DpiWindowScaleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double windowDimension && parameter is string paramStr)
            {
                var parts = paramStr.Split(':');
                if (parts.Length == 2 &&
                    double.TryParse(parts[0], out double ratio))
                {
                    // 应用窗口缩放
                    double result = windowDimension * ratio;

                    // 应用DPI缩放
                    double dpiScale = DpiManager.DpiScaleX;
                    result = result / dpiScale;

                    return Math.Max(result, 20);
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // ============ 3. 绑定代理（用于XAML中绑定窗口属性） ============
    /// <summary>
    /// 在XAML中直接绑定窗口的ActualWidth和ActualHeight时使用
    /// 例: <local:BindingProxy x:Key="ProxyElement" Data="{Binding RelativeSource={RelativeSource Mode=FindAncestor, AncestorType=Window}}" />
    /// </summary>
    public class BindingProxy : System.Windows.Freezable
    {
        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
    }

    // ============ 4. 增强版DPI缩放行为 ============
    public static class ResponsiveSizeBehavior
    {
        /// <summary>
        /// 根据窗口宽度自动调整大小（保持设计时的宽度比例）
        /// 用法: local:ResponsiveSizeBehavior.ResponsivePercentWidth="0.5"
        /// 表示占窗口宽度的50%
        /// </summary>
        public static double GetResponsivePercentWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(ResponsivePercentWidthProperty);
        }

        public static void SetResponsivePercentWidth(DependencyObject obj, double value)
        {
            obj.SetValue(ResponsivePercentWidthProperty, value);
        }

        public static readonly DependencyProperty ResponsivePercentWidthProperty =
            DependencyProperty.RegisterAttached(
                "ResponsivePercentWidth",
                typeof(double),
                typeof(ResponsiveSizeBehavior),
                new PropertyMetadata(0.0, OnResponsivePercentWidthChanged));

        private static void OnResponsivePercentWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element && e.NewValue is double percent && percent > 0)
            {
                // 获取父窗口
                var window = Window.GetWindow(element);
                if (window != null)
                {
                    element.Width = window.ActualWidth * percent / DpiManager.DpiScaleX;

                    // 监听窗口大小变化
                    window.SizeChanged -= (s, args) => UpdateResponsiveWidth(element, percent);
                    window.SizeChanged += (s, args) => UpdateResponsiveWidth(element, percent);
                }
            }
        }

        private static void UpdateResponsiveWidth(FrameworkElement element, double percent)
        {
            var window = Window.GetWindow(element);
            if (window != null)
            {
                element.Width = window.ActualWidth * percent / DpiManager.DpiScaleX;
            }
        }

        /// <summary>
        /// 根据窗口高度自动调整大小
        /// 用法: local:ResponsiveSizeBehavior.ResponsivePercentHeight="0.5"
        /// 表示占窗口高度的50%
        /// </summary>
        public static double GetResponsivePercentHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(ResponsivePercentHeightProperty);
        }

        public static void SetResponsivePercentHeight(DependencyObject obj, double value)
        {
            obj.SetValue(ResponsivePercentHeightProperty, value);
        }

        public static readonly DependencyProperty ResponsivePercentHeightProperty =
            DependencyProperty.RegisterAttached(
                "ResponsivePercentHeight",
                typeof(double),
                typeof(ResponsiveSizeBehavior),
                new PropertyMetadata(0.0, OnResponsivePercentHeightChanged));

        private static void OnResponsivePercentHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element && e.NewValue is double percent && percent > 0)
            {
                var window = Window.GetWindow(element);
                if (window != null)
                {
                    element.Height = window.ActualHeight * percent / DpiManager.DpiScaleY;

                    window.SizeChanged -= (s, args) => UpdateResponsiveHeight(element, percent);
                    window.SizeChanged += (s, args) => UpdateResponsiveHeight(element, percent);
                }
            }
        }

        private static void UpdateResponsiveHeight(FrameworkElement element, double percent)
        {
            var window = Window.GetWindow(element);
            if (window != null)
            {
                element.Height = window.ActualHeight * percent / DpiManager.DpiScaleY;
            }
        }
    }
}