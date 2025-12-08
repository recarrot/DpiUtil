using DpiUtil;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Test1
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DpiManager.EnablePerMonitorV2DpiAwareness();

            // 示例：设置不同的设计基准分辨率
            // 默认基准：1920x1080 @ 100%
            // WindowScaleManager.Set1080PBase();

            // 720P基准 (1280x720 @ 100%)
            // WindowScaleManager.Set720PBase();

            // 2K基准 (2560x1440 @ 100%)
            // WindowScaleManager.Set2KBase();

            // 4K基准 (3840x2160 @ 100%)
             //WindowScaleManager.Set4KBase();

            // 自定义基准尺寸和缩放
            // WindowScaleManager.SetDesignBase(1366, 768, 1.25);
        }
    }
}
