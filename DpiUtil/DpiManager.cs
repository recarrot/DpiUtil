using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DpiUtil
{
    public static class DpiManager
    {
        private static double _dpiScaleX = 1.0;
        private static double _dpiScaleY = 1.0;

        public static double DpiScaleX => _dpiScaleX;
        public static double DpiScaleY => _dpiScaleY;

        [DllImport("user32.dll", SetLastError = true)] 
        private static extern IntPtr SetProcessDpiAwarenessContext(IntPtr dpiAwarenessContext);

        private static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = new IntPtr(-4);

        // 在 App.xaml.cs 的构造函数中最先调用此方法
        public static void EnablePerMonitorV2DpiAwareness()
        {
            try
            {
                SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
            }
            catch { }
        }

        public static void Initialize(Window window)
        {
            var source = PresentationSource.FromVisual(window);
            if (source?.CompositionTarget != null)
            {
                _dpiScaleX = source.CompositionTarget.TransformToDevice.M11;
                _dpiScaleY = source.CompositionTarget.TransformToDevice.M22;
            }
        }

        // 将设计时尺寸转换为实际显示尺寸
        public static double Scale(double designValue)
        {
            return designValue * _dpiScaleX;
        }

        public static Thickness ScaleThickness(double top, double right, double bottom, double left)
        {
            return new Thickness(
                Scale(left),
                Scale(top),
                Scale(right),
                Scale(bottom));
        }
    }

}
