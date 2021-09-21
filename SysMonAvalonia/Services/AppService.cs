using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.Styling;
using SysMonAvalonia.Views;
using SysMonAvalonia.Data;
using Microsoft.Win32.TaskScheduler;
using SysMonAvalonia.Models;

namespace SysMonAvalonia.Services
{
    public static class AppService
    {
        private static SettingView _settingWindow;

        public static void ShowSettings()
        {
            _settingWindow = new SettingView();
            _settingWindow.Show();
        }

        public static void CloseSettings()
        {
            if (_settingWindow != null) _settingWindow.Close();
        }

        public static bool CreateTaskService(string path)
        {
            bool isActive = false;

            TaskService task = TaskService.Instance;
            LogonTrigger logonTrigger = new();
            TaskDefinition taskDefinition = task.NewTask();

            try
            {
                logonTrigger.Delay = TimeSpan.FromSeconds(15);

                taskDefinition.RegistrationInfo.Description = "Auto start " + Path.GetFileNameWithoutExtension(path);
                taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;
                taskDefinition.Triggers.Add(logonTrigger);
                taskDefinition.Actions.Add(new ExecAction(path));

                isActive = task.RootFolder
                    .RegisterTaskDefinition(Path.GetFileNameWithoutExtension(path), taskDefinition).IsActive;
            }
            catch { }
            finally
            {
                taskDefinition.Dispose();
                logonTrigger.Dispose();
                task.Dispose();
            }

            return isActive;
        }

        public static void DeleteTaskService(string name)
        {
            TaskService task = TaskService.Instance;
            
            task.RootFolder.DeleteFolder(name, false);

            task.Dispose();
        }

        public static bool FindTaskService(string name)
        {
            bool isExist = false;

            TaskService task = TaskService.Instance;

            try
            {
                isExist = task.FindTask(name) != null;
            }
            catch
            {
            }
            finally
            {
                task.Dispose();
            }

            return isExist;
        }

        public static void ShutdownApp()
        {
            if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.Shutdown();
        }

        public static void ChangeStyles()
        {
            string localStyle = SettingData.CurrentSetting.Style == SettingData.Theme.Dark ? "Dark.xaml" : "Light.xaml";
            string fluentStyle = SettingData.CurrentSetting.Style == SettingData.Theme.Dark ? "FluentDark.xaml" : "FluentLight.xaml";

            StyleInclude fluentStyleInclude = new(baseUri: new Uri("avares://Avalonia.Themes.Fluent"));
            fluentStyleInclude.Source = new Uri(fluentStyle, UriKind.Relative);

            App.Current.Styles.Add(fluentStyleInclude);

            StyleInclude localStyleInclude = new(baseUri: new Uri("avares://SysMonAvalonia/Assets/Styles/"));
            localStyleInclude.Source = new Uri(localStyle, UriKind.Relative);

            App.Current.Styles.Add(localStyleInclude);
        }

        public static void ShowWidgets(IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Exit += (sender, args) => SettingModel.Save();

            List<Window> widgetList = new();

            if (SettingData.CurrentSetting.DeviceIoWidget.IsStartup)
            {
                DeviceIoWidgetView deviceIoWidget = new();
                deviceIoWidget.WindowStartupLocation = WindowStartupLocation.Manual;
                deviceIoWidget.Position = new(SettingData.CurrentSetting.DeviceIoWidget.X,
                                                      SettingData.CurrentSetting.DeviceIoWidget.Y);

                widgetList.Add(deviceIoWidget);
            }

            if (SettingData.CurrentSetting.WeatherWidget.IsStartup)
            {
                WeatherWidgetView weatherWidget = new();
                weatherWidget.WindowStartupLocation = WindowStartupLocation.Manual;
                weatherWidget.Position = new(SettingData.CurrentSetting.WeatherWidget.X,
                                                     SettingData.CurrentSetting.WeatherWidget.Y);

                widgetList.Add(weatherWidget);
            }

            if (SettingData.CurrentSetting.ComboWidget.IsStartup)
            {
                ComboWidgetView comboWidget = new();
                comboWidget.WindowStartupLocation = WindowStartupLocation.Manual;
                comboWidget.Position = new(SettingData.CurrentSetting.ComboWidget.X,
                                                   SettingData.CurrentSetting.ComboWidget.Y);

                widgetList.Add(comboWidget);
            }

            foreach (Window widget in widgetList)
            {
                if (desktop.MainWindow == null)
                {
                    desktop.MainWindow = widget;
                    continue;
                }

                widget.Show();
            }

            if (desktop.MainWindow == null) desktop.MainWindow = new DeviceIoWidgetView();
        }
    }
}