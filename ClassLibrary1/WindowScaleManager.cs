using System;
using System.Windows;

namespace DpiUtil
{
    /// <summary>
    /// 基于窗口实际尺寸与设计尺寸的比例来缩放
    /// </summary>
    public static class WindowScaleManager
    {
        // 设计基准尺寸 - 默认1920x1080 @ 100% 缩放
        private static double _designWidth = 1920.0;
        private static double _designHeight = 1080.0;
        private static double _designDpiScale = 1.0;

        private static double _scaleX = 1.0;
        private static double _scaleY = 1.0;
        private static double _scale = 1.0;  // 取较小值，保证内容不超出

        /// <summary>
        /// 获取或设置设计宽度
        /// </summary>
        public static double DesignWidth
        {
            get => _designWidth;
            set
            {
                if (value > 0 && _designWidth != value)
                {
                    _designWidth = value;
                    if (IsInitialized)
                        ScaleChanged?.Invoke();
                }
            }
        }

        /// <summary>
        /// 获取或设置设计高度
        /// </summary>
        public static double DesignHeight
        {
            get => _designHeight;
            set
            {
                if (value > 0 && _designHeight != value)
                {
                    _designHeight = value;
                    if (IsInitialized)
                        ScaleChanged?.Invoke();
                }
            }
        }

        /// <summary>
        /// 获取或设置设计DPI缩放比例
        /// </summary>
        public static double DesignDpiScale
        {
            get => _designDpiScale;
            set
            {
                if (value > 0 && _designDpiScale != value)
                {
                    _designDpiScale = value;
                    if (IsInitialized)
                        ScaleChanged?.Invoke();
                }
            }
        }

        public static double ScaleX => _scaleX;
        public static double ScaleY => _scaleY;
        public static double ScaleFactor => _scale;
        public static bool IsInitialized { get; private set; }

        public static event Action ScaleChanged;

        /// <summary>
        /// 设置为720P基准分辨率 (1280x720 @ 100%)
        /// </summary>
        public static void Set720PBase()
        {
            _designWidth = 1280.0;
            _designHeight = 720.0;
            _designDpiScale = 1.0;
            if (IsInitialized)
                ScaleChanged?.Invoke();
        }

        /// <summary>
        /// 设置为1080P基准分辨率 (1920x1080 @ 100%)
        /// </summary>
        public static void Set1080PBase()
        {
            _designWidth = 1920.0;
            _designHeight = 1080.0;
            _designDpiScale = 1.0;
            if (IsInitialized)
                ScaleChanged?.Invoke();
        }

        /// <summary>
        /// 设置为2K基准分辨率 (2560x1440 @ 100%)
        /// </summary>
        public static void Set2KBase()
        {
            _designWidth = 2560.0;
            _designHeight = 1440.0;
            _designDpiScale = 1.0;
            if (IsInitialized)
                ScaleChanged?.Invoke();
        }

        /// <summary>
        /// 设置为4K基准分辨率 (3840x2160 @ 100%)
        /// </summary>
        public static void Set4KBase()
        {
            _designWidth = 3840.0;
            _designHeight = 2160.0;
            _designDpiScale = 1.0;
            if (IsInitialized)
                ScaleChanged?.Invoke();
        }

        /// <summary>
        /// 在主窗口 Loaded 事件中调用
        /// </summary>
        public static void Initialize(Window window)
        {
            UpdateScale(window);
            IsInitialized = true;
            ScaleChanged?.Invoke();

            // 监听窗口大小变化
            window.SizeChanged += (s, e) =>
            {
                UpdateScale(window);
                ScaleChanged?.Invoke();
            };
        }

        /// <summary>
        /// 自定义设计基准尺寸和缩放
        /// </summary>
        public static void SetDesignBase(double width, double height, double dpiScale = 1.0)
        {
            if (width > 0 && height > 0 && dpiScale > 0)
            {
                _designWidth = width;
                _designHeight = height;
                _designDpiScale = dpiScale;
                if (IsInitialized)
                    ScaleChanged?.Invoke();
            }
        }

        private static void UpdateScale(Window window)
        {
            double actualWidth = window.ActualWidth;
            double actualHeight = window.ActualHeight;

            if (actualWidth <= 0 || actualHeight <= 0)
            {
                actualWidth = SystemParameters.WorkArea.Width;
                actualHeight = SystemParameters.WorkArea.Height;
            }

            _scaleX = actualWidth / DesignWidth;
            _scaleY = actualHeight / DesignHeight;

            _scale = Math.Min(_scaleX, _scaleY);
        }

        /// <summary>
        /// 缩放单个值
        /// </summary>
        public static double Scale(double designValue)
        {
            return designValue * _designDpiScale * _scale;
        }

        /// <summary>
        /// 缩放 Thickness
        /// </summary>
        public static Thickness ScaleThickness(Thickness t)
        {
            return new Thickness(
                Scale(t.Left),
                Scale(t.Top),
                Scale(t.Right),
                Scale(t.Bottom));
        }
    }
}