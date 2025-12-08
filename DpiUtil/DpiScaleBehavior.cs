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

        public static readonly DependencyProperty ResponsiveWidthProperty =
            DependencyProperty.RegisterAttached(
                "ResponsiveWidth", typeof(double), typeof(DpiScaleBehavior),
                new PropertyMetadata(0.0, OnResponsiveWidthChanged));

        private static void OnResponsiveWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element && e.NewValue is double width && width > 0)
            {
                void UpdateWidth() => element.Width = WindowScaleManager.Scale(width);

                if (WindowScaleManager.IsInitialized)
                    UpdateWidth();

                // 监听缩放比例变化（窗口大小改变时）
                WindowScaleManager.ScaleChanged += UpdateWidth;
                element.Unloaded += (s, ev) => WindowScaleManager.ScaleChanged -= UpdateWidth;
            }
        }
        #endregion

        #region ResponsiveHeight
        public static double GetResponsiveHeight(DependencyObject obj)
            => (double)obj.GetValue(ResponsiveHeightProperty);

        public static void SetResponsiveHeight(DependencyObject obj, double value)
            => obj.SetValue(ResponsiveHeightProperty, value);

        public static readonly DependencyProperty ResponsiveHeightProperty =
            DependencyProperty.RegisterAttached(
                "ResponsiveHeight", typeof(double), typeof(DpiScaleBehavior),
                new PropertyMetadata(0.0, OnResponsiveHeightChanged));

        private static void OnResponsiveHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element && e.NewValue is double height && height > 0)
            {
                void UpdateHeight() => element.Height = WindowScaleManager.Scale(height);

                if (WindowScaleManager.IsInitialized)
                    UpdateHeight();

                WindowScaleManager.ScaleChanged += UpdateHeight;
                element.Unloaded += (s, ev) => WindowScaleManager.ScaleChanged -= UpdateHeight;
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
                void UpdateMargin() => element.Margin = WindowScaleManager.ScaleThickness(t);

                if (WindowScaleManager.IsInitialized)
                    UpdateMargin();

                WindowScaleManager.ScaleChanged += UpdateMargin;
                element.Unloaded += (s, ev) => WindowScaleManager.ScaleChanged -= UpdateMargin;
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
                void UpdatePadding()
                {
                    var scaled = WindowScaleManager.ScaleThickness(t);
                    // 使用反射设置Padding属性，支持更多类型的控件（包括TextBlock、Control和Border等）
                    var paddingProperty = d.GetType().GetProperty("Padding");
                    if (paddingProperty != null && paddingProperty.CanWrite)
                    {
                        paddingProperty.SetValue(d, scaled);
                    }
                }

                if (WindowScaleManager.IsInitialized)
                    UpdatePadding();

                WindowScaleManager.ScaleChanged += UpdatePadding;
                if (d is FrameworkElement fe)
                    fe.Unloaded += (s, ev) => WindowScaleManager.ScaleChanged -= UpdatePadding;
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
                void UpdateCornerRadius()
                {
                    // 使用反射设置CornerRadius属性，支持所有具有该属性的控件（如Border、Button等）
                    var cornerRadiusProperty = d.GetType().GetProperty("CornerRadius");
                    if (cornerRadiusProperty != null && cornerRadiusProperty.CanWrite)
                    {
                        // 对CornerRadius的每个值进行缩放
                        var scaled = new CornerRadius(
                            WindowScaleManager.Scale(cornerRadius.TopLeft),
                            WindowScaleManager.Scale(cornerRadius.TopRight),
                            WindowScaleManager.Scale(cornerRadius.BottomRight),
                            WindowScaleManager.Scale(cornerRadius.BottomLeft)
                        );
                        cornerRadiusProperty.SetValue(d, scaled);
                    }
                }

                if (WindowScaleManager.IsInitialized)
                    UpdateCornerRadius();

                WindowScaleManager.ScaleChanged += UpdateCornerRadius;
                if (d is FrameworkElement fe)
                    fe.Unloaded += (s, ev) => WindowScaleManager.ScaleChanged -= UpdateCornerRadius;
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
                void UpdateFontSize()
                {
                    var scaled = WindowScaleManager.Scale(fontSize);
                    if (d is TextBlock tb) tb.FontSize = scaled;
                    else if (d is Control c) c.FontSize = scaled;
                }

                if (WindowScaleManager.IsInitialized)
                    UpdateFontSize();

                WindowScaleManager.ScaleChanged += UpdateFontSize;
                if (d is FrameworkElement fe)
                    fe.Unloaded += (s, ev) => WindowScaleManager.ScaleChanged -= UpdateFontSize;
            }
        }
        #endregion
    }
}