﻿﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DpiUtil
{
    public static class DpiScaleBehavior
    {
        #region ResponsiveWidth
        public static double GetResponsiveWidth(DependencyObject obj)
            => (double)obj.GetValue(ResponsiveWidthProperty);

        public static void SetResponsiveWidth(DependencyObject obj, double value)
            => obj.SetValue(ResponsiveWidthProperty, value);

        public static readonly DependencyProperty ResponsiveWidthProperty = DependencyProperty.RegisterAttached(
            "ResponsiveWidth", typeof(double), typeof(DpiScaleBehavior),
            new PropertyMetadata(0.0, OnResponsiveWidthChanged));

        private static void OnResponsiveWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window && e.NewValue is double windowWidth && windowWidth > 0)
            {
                // 对于窗口，只在首次加载时设置初始大小，避免循环依赖
                if (!WindowScaleManager.IsInitialized)
                {
                    // 基于工作区大小计算初始缩放因子
                    double workAreaWidth = SystemParameters.WorkArea.Width;
                    double designWidth = WindowScaleManager.DesignWidth;
                    double initialScale = workAreaWidth / designWidth;

                    window.Width = windowWidth * initialScale;
                }
            }
            else if (d is FrameworkElement element && e.NewValue is double elementWidth && elementWidth > 0)
            {
                // Console.WriteLine($"[DpiScaleBehavior] OnResponsiveWidthChanged for {element.GetType().Name}");
                // 对于其他UI元素，继续使用WindowScaleManager的缩放逻辑
                void UpdateWidth()
                {
                    double scaledWidth = WindowScaleManager.Scale(elementWidth);
                    // Console.WriteLine($"[DpiScaleBehavior] UpdateWidth for {element.GetType().Name}: {elementWidth} -> {scaledWidth}");
                    element.Width = scaledWidth;
                }

                void OnLoaded(object s, RoutedEventArgs ev)
                {
                    // Console.WriteLine($"[DpiScaleBehavior] Loaded event for {element.GetType().Name}");
                    // 重新订阅ScaleChanged事件
                    if (!WindowScaleManager.ContainsHandler(UpdateWidth))
                    {
                        // Console.WriteLine($"[DpiScaleBehavior] Re-subscribing to ScaleChanged for {element.GetType().Name}");
                        WindowScaleManager.ScaleChanged += UpdateWidth;
                    }
                    UpdateWidth();
                }

                if (WindowScaleManager.IsInitialized)
                {
                    // Console.WriteLine($"[DpiScaleBehavior] Initializing width for {element.GetType().Name}");
                    UpdateWidth();
                }

                // Console.WriteLine($"[DpiScaleBehavior] Subscribing to ScaleChanged for {element.GetType().Name}");
                WindowScaleManager.ScaleChanged += UpdateWidth;
                element.Loaded += OnLoaded;
                element.Unloaded += (s, ev) =>
                {
                    // Console.WriteLine($"[DpiScaleBehavior] Unloaded event for {element.GetType().Name}");
                    WindowScaleManager.ScaleChanged -= UpdateWidth;
                };
            }
        }
        #endregion

        #region ResponsiveHeight
        public static double GetResponsiveHeight(DependencyObject obj)
            => (double)obj.GetValue(ResponsiveHeightProperty);

        public static void SetResponsiveHeight(DependencyObject obj, double value)
            => obj.SetValue(ResponsiveHeightProperty, value);

        public static readonly DependencyProperty ResponsiveHeightProperty = DependencyProperty.RegisterAttached(
            "ResponsiveHeight", typeof(double), typeof(DpiScaleBehavior),
            new PropertyMetadata(0.0, OnResponsiveHeightChanged));

        private static void OnResponsiveHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window && e.NewValue is double windowHeight && windowHeight > 0)
            {
                // 对于窗口，只在首次加载时设置初始大小，避免循环依赖
                if (!WindowScaleManager.IsInitialized)
                {
                    // 基于工作区大小计算初始缩放因子
                    double workAreaHeight = SystemParameters.WorkArea.Height;
                    double designHeight = WindowScaleManager.DesignHeight;
                    double initialScale = workAreaHeight / designHeight;

                    window.Height = windowHeight * initialScale;
                }
            }
            else if (d is FrameworkElement element && e.NewValue is double elementHeight && elementHeight > 0)
            {
                // Console.WriteLine($"[DpiScaleBehavior] OnResponsiveHeightChanged for {element.GetType().Name}");
                // 对于其他UI元素，继续使用WindowScaleManager的缩放逻辑
                void UpdateHeight()
                {
                    double scaledHeight = WindowScaleManager.Scale(elementHeight);
                    // Console.WriteLine($"[DpiScaleBehavior] UpdateHeight for {element.GetType().Name}: {elementHeight} -> {scaledHeight}");
                    element.Height = scaledHeight;
                }

                void OnLoaded(object s, RoutedEventArgs ev)
                {
                    // Console.WriteLine($"[DpiScaleBehavior] Loaded event for {element.GetType().Name}");
                    // 重新订阅ScaleChanged事件
                    if (!WindowScaleManager.ContainsHandler(UpdateHeight))
                    {
                        // Console.WriteLine($"[DpiScaleBehavior] Re-subscribing to ScaleChanged for {element.GetType().Name}");
                        WindowScaleManager.ScaleChanged += UpdateHeight;
                    }
                    UpdateHeight();
                }

                if (WindowScaleManager.IsInitialized)
                {
                    // Console.WriteLine($"[DpiScaleBehavior] Initializing height for {element.GetType().Name}");
                    UpdateHeight();
                }

                // Console.WriteLine($"[DpiScaleBehavior] Subscribing to ScaleChanged for {element.GetType().Name}");
                WindowScaleManager.ScaleChanged += UpdateHeight;
                element.Loaded += OnLoaded;
                element.Unloaded += (s, ev) =>
                {
                    // Console.WriteLine($"[DpiScaleBehavior] Unloaded event for {element.GetType().Name}");
                    WindowScaleManager.ScaleChanged -= UpdateHeight;
                };
            }
        }
        #endregion

        #region ResponsiveMargin
        public static Thickness GetResponsiveMargin(DependencyObject obj)
            => (Thickness)obj.GetValue(ResponsiveMarginProperty);

        public static void SetResponsiveMargin(DependencyObject obj, Thickness value)
            => obj.SetValue(ResponsiveMarginProperty, value);

        public static readonly DependencyProperty ResponsiveMarginProperty =
            DependencyProperty.RegisterAttached(
                "ResponsiveMargin", typeof(Thickness), typeof(DpiScaleBehavior),
                new PropertyMetadata(new Thickness(0), OnResponsiveMarginChanged));

        private static void OnResponsiveMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element && e.NewValue is Thickness t)
            {
                // Console.WriteLine($"[DpiScaleBehavior] OnResponsiveMarginChanged for {element.GetType().Name}");
                void UpdateMargin()
                {
                    Thickness scaledMargin = WindowScaleManager.ScaleThickness(t);
                    // Console.WriteLine($"[DpiScaleBehavior] UpdateMargin for {element.GetType().Name}: {t.Left},{t.Top},{t.Right},{t.Bottom} -> {scaledMargin.Left},{scaledMargin.Top},{scaledMargin.Right},{scaledMargin.Bottom}");
                    element.Margin = scaledMargin;
                }

                void OnLoaded(object s, RoutedEventArgs ev)
                {
                    // Console.WriteLine($"[DpiScaleBehavior] Loaded event for {element.GetType().Name}");
                    // 重新订阅ScaleChanged事件
                    if (!WindowScaleManager.ContainsHandler(UpdateMargin))
                    {
                        // Console.WriteLine($"[DpiScaleBehavior] Re-subscribing to ScaleChanged for {element.GetType().Name}");
                        WindowScaleManager.ScaleChanged += UpdateMargin;
                    }
                    UpdateMargin();
                }

                if (WindowScaleManager.IsInitialized)
                {
                    // Console.WriteLine($"[DpiScaleBehavior] Initializing margin for {element.GetType().Name}");
                    UpdateMargin();
                }

                // Console.WriteLine($"[DpiScaleBehavior] Subscribing to ScaleChanged for {element.GetType().Name}");
                WindowScaleManager.ScaleChanged += UpdateMargin;
                element.Loaded += OnLoaded;
                element.Unloaded += (s, ev) =>
                {
                    // Console.WriteLine($"[DpiScaleBehavior] Unloaded event for {element.GetType().Name}");
                    WindowScaleManager.ScaleChanged -= UpdateMargin;
                };
            }
        }
        #endregion

        #region ResponsivePadding
        public static Thickness GetResponsivePadding(DependencyObject obj)
            => (Thickness)obj.GetValue(ResponsivePaddingProperty);

        public static void SetResponsivePadding(DependencyObject obj, Thickness value)
            => obj.SetValue(ResponsivePaddingProperty, value);

        public static readonly DependencyProperty ResponsivePaddingProperty =
            DependencyProperty.RegisterAttached(
                "ResponsivePadding", typeof(Thickness), typeof(DpiScaleBehavior),
                new PropertyMetadata(new Thickness(0), OnResponsivePaddingChanged));

        private static void OnResponsivePaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Thickness t)
            {
                // Console.WriteLine($"[DpiScaleBehavior] OnResponsivePaddingChanged for {d.GetType().Name}");
                void UpdatePadding()
                {
                    var scaled = WindowScaleManager.ScaleThickness(t);
                    // Console.WriteLine($"[DpiScaleBehavior] UpdatePadding for {d.GetType().Name}: {t.Left},{t.Top},{t.Right},{t.Bottom} -> {scaled.Left},{scaled.Top},{scaled.Right},{scaled.Bottom}");
                    // 使用反射设置Padding属性，支持更多类型的控件（包括TextBlock、Control和Border等）
                    var paddingProperty = d.GetType().GetProperty("Padding");
                    if (paddingProperty != null && paddingProperty.CanWrite)
                    {
                        paddingProperty.SetValue(d, scaled);
                    }
                }

                void OnLoaded(object s, RoutedEventArgs ev)
                {
                    // Console.WriteLine($"[DpiScaleBehavior] Loaded event for {d.GetType().Name}");
                    // 重新订阅ScaleChanged事件
                    if (!WindowScaleManager.ContainsHandler(UpdatePadding))
                    {
                        // Console.WriteLine($"[DpiScaleBehavior] Re-subscribing to ScaleChanged for {d.GetType().Name}");
                        WindowScaleManager.ScaleChanged += UpdatePadding;
                    }
                    UpdatePadding();
                }

                if (WindowScaleManager.IsInitialized)
                {
                    // Console.WriteLine($"[DpiScaleBehavior] Initializing padding for {d.GetType().Name}");
                    UpdatePadding();
                }

                // Console.WriteLine($"[DpiScaleBehavior] Subscribing to ScaleChanged for {d.GetType().Name}");
                WindowScaleManager.ScaleChanged += UpdatePadding;
                if (d is FrameworkElement fe)
                {
                    fe.Loaded += OnLoaded;
                    fe.Unloaded += (s, ev) =>
                    {
                        // Console.WriteLine($"[DpiScaleBehavior] Unloaded event for {d.GetType().Name}");
                        WindowScaleManager.ScaleChanged -= UpdatePadding;
                    };
                }
            }
        }
        #endregion

        #region ResponsiveCornerRadius
        public static CornerRadius GetResponsiveCornerRadius(DependencyObject obj)
            => (CornerRadius)obj.GetValue(ResponsiveCornerRadiusProperty);

        public static void SetResponsiveCornerRadius(DependencyObject obj, CornerRadius value)
            => obj.SetValue(ResponsiveCornerRadiusProperty, value);

        public static readonly DependencyProperty ResponsiveCornerRadiusProperty =
            DependencyProperty.RegisterAttached(
                "ResponsiveCornerRadius", typeof(CornerRadius), typeof(DpiScaleBehavior),
                new PropertyMetadata(new CornerRadius(0), OnResponsiveCornerRadiusChanged));

        private static void OnResponsiveCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is CornerRadius cornerRadius)
            {
                // Console.WriteLine($"[DpiScaleBehavior] OnResponsiveCornerRadiusChanged for {d.GetType().Name}");
                void UpdateCornerRadius()
                {
                    var scaled = new CornerRadius(
                        WindowScaleManager.Scale(cornerRadius.TopLeft),
                        WindowScaleManager.Scale(cornerRadius.TopRight),
                        WindowScaleManager.Scale(cornerRadius.BottomRight),
                        WindowScaleManager.Scale(cornerRadius.BottomLeft)
                    );
                    // Console.WriteLine($"[DpiScaleBehavior] UpdateCornerRadius for {d.GetType().Name}: {cornerRadius.TopLeft},{cornerRadius.TopRight},{cornerRadius.BottomRight},{cornerRadius.BottomLeft} -> {scaled.TopLeft},{scaled.TopRight},{scaled.BottomRight},{scaled.BottomLeft}");
                    var cornerRadiusProperty = d.GetType().GetProperty("CornerRadius");
                    if (cornerRadiusProperty != null && cornerRadiusProperty.CanWrite)
                    {
                        cornerRadiusProperty.SetValue(d, scaled);
                    }
                }

                void OnLoaded(object s, RoutedEventArgs ev)
                {
                    // Console.WriteLine($"[DpiScaleBehavior] Loaded event for {d.GetType().Name}");
                    // 重新订阅ScaleChanged事件
                    if (!WindowScaleManager.ContainsHandler(UpdateCornerRadius))
                    {
                        // Console.WriteLine($"[DpiScaleBehavior] Re-subscribing to ScaleChanged for {d.GetType().Name}");
                        WindowScaleManager.ScaleChanged += UpdateCornerRadius;
                    }
                    UpdateCornerRadius();
                }

                if (WindowScaleManager.IsInitialized)
                {
                    // Console.WriteLine($"[DpiScaleBehavior] Initializing corner radius for {d.GetType().Name}");
                    UpdateCornerRadius();
                }

                // Console.WriteLine($"[DpiScaleBehavior] Subscribing to ScaleChanged for {d.GetType().Name}");
                WindowScaleManager.ScaleChanged += UpdateCornerRadius;
                if (d is FrameworkElement fe)
                {
                    fe.Loaded += OnLoaded;
                    fe.Unloaded += (s, ev) =>
                    {
                        // Console.WriteLine($"[DpiScaleBehavior] Unloaded event for {d.GetType().Name}");
                        WindowScaleManager.ScaleChanged -= UpdateCornerRadius;
                    };
                }
            }
        }
        #endregion

        #region ResponsiveFontSize
        public static double GetResponsiveFontSize(DependencyObject obj)
            => (double)obj.GetValue(ResponsiveFontSizeProperty);

        public static void SetResponsiveFontSize(DependencyObject obj, double value)
            => obj.SetValue(ResponsiveFontSizeProperty, value);

        public static readonly DependencyProperty ResponsiveFontSizeProperty =
            DependencyProperty.RegisterAttached(
                "ResponsiveFontSize", typeof(double), typeof(DpiScaleBehavior),
                new PropertyMetadata(0.0, OnResponsiveFontSizeChanged));

        private static void OnResponsiveFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is double fontSize && fontSize > 0)
            {
                // Console.WriteLine($"[DpiScaleBehavior] OnResponsiveFontSizeChanged for {d.GetType().Name}");
                void UpdateFontSize()
                {
                    var scaled = WindowScaleManager.Scale(fontSize);
                    // Console.WriteLine($"[DpiScaleBehavior] UpdateFontSize for {d.GetType().Name}: {fontSize} -> {scaled}");
                    if (d is TextBlock tb) tb.FontSize = scaled;
                    else if (d is Control c) c.FontSize = scaled;
                }

                void OnLoaded(object s, RoutedEventArgs ev)
                {
                    // Console.WriteLine($"[DpiScaleBehavior] Loaded event for {d.GetType().Name}");
                    // 重新订阅ScaleChanged事件
                    if (!WindowScaleManager.ContainsHandler(UpdateFontSize))
                    {
                        // Console.WriteLine($"[DpiScaleBehavior] Re-subscribing to ScaleChanged for {d.GetType().Name}");
                        WindowScaleManager.ScaleChanged += UpdateFontSize;
                    }
                    UpdateFontSize();
                }

                if (WindowScaleManager.IsInitialized)
                {
                    // Console.WriteLine($"[DpiScaleBehavior] Initializing font size for {d.GetType().Name}");
                    UpdateFontSize();
                }

                // Console.WriteLine($"[DpiScaleBehavior] Subscribing to ScaleChanged for {d.GetType().Name}");
                WindowScaleManager.ScaleChanged += UpdateFontSize;
                if (d is FrameworkElement fe)
                {
                    fe.Loaded += OnLoaded;
                    fe.Unloaded += (s, ev) =>
                    {
                        // Console.WriteLine($"[DpiScaleBehavior] Unloaded event for {d.GetType().Name}");
                        WindowScaleManager.ScaleChanged -= UpdateFontSize;
                    };
                }
            }
        }
        #endregion
    }
}