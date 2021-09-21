using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SysMonAvalonia.Localization;
using SysMonAvalonia.Data;
using SysMonAvalonia.Models;
using SysMonAvalonia.Services;

namespace SysMonAvalonia
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            SettingModel.Load();

            Language.Load(SettingData.CurrentSetting.Culture);

            AppService.ChangeStyles();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Exit += (sender, args) => SettingModel.Save();

                AppService.ShowWidgets(desktop);
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
