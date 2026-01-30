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

        // Windows 7兼容的DPI感知API
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetProcessDPIAware();

        // Windows 8.1+ 的DPI感知API
        [DllImport("shcore.dll", SetLastError = true)]
        private static extern int SetProcessDpiAwareness(int value);

        // Windows 10 1703+ 的DPI感知API
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetProcessDpiAwarenessContext(IntPtr dpiAwarenessContext);

        private enum DPI_AWARENESS
        {
            UNAWARE = 0,
            SYSTEM_AWARE = 1,
            PER_MONITOR_AWARE = 2
        }

        private static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = new IntPtr(-4);

        public static void EnablePerMonitorV2DpiAwareness()
        {
            // 按操作系统版本使用不同的API
            var osVersion = Environment.OSVersion;

            if (osVersion.Platform == PlatformID.Win32NT)
            {
                // Windows 10 1607+ (Build 14393+)
                if (osVersion.Version.Major == 10 && osVersion.Version.Build >= 14393)
                {
                    try
                    {
                        // Windows 10 1703+ (Build 15063+)
                        if (osVersion.Version.Build >= 15063)
                        {
                            SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
                        }
                        // Windows 10 1607
                        else
                        {
                            SetProcessDpiAwareness((int)DPI_AWARENESS.PER_MONITOR_AWARE);
                        }
                    }
                    catch (DllNotFoundException)
                    {
                        // API不存在，回退到旧版本
                        FallbackDpiAwareness();
                    }
                    catch (EntryPointNotFoundException)
                    {
                        // 函数不存在，回退
                        FallbackDpiAwareness();
                    }
                }
                // Windows 8.1
                else if (osVersion.Version.Major == 6 && osVersion.Version.Minor == 3)
                {
                    try
                    {
                        SetProcessDpiAwareness((int)DPI_AWARENESS.PER_MONITOR_AWARE);
                    }
                    catch (DllNotFoundException)
                    {
                        // 如果shcore.dll不存在（某些Windows 8.1版本）
                        SetProcessDPIAware();
                    }
                }
                // Windows 7, Windows 8
                else
                {
                    SetProcessDPIAware(); // 最基本的DPI感知
                }
            }
        }

        private static void FallbackDpiAwareness()
        {
            try
            {
                // 尝试Windows 8.1 API
                SetProcessDpiAwareness((int)DPI_AWARENESS.PER_MONITOR_AWARE);
            }
            catch
            {
                // 最终回退到Windows Vista/7 API
                SetProcessDPIAware();
            }
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
