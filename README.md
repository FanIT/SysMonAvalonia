# SysMonAvalonia
------
SysMonAvalonia - 3 desktop widgets for Windows 10.
- Disks widgets - Showing a list of logical disks in Windows with informations about total/used size, model, speed read/write and health, total write for ssd.
- Combo widgets - Showing system informations cpu, ram, video and network.
- Weather widgets - Showing a current, forecast hours and forecast days weather. The widget have 4 weather providers.

## What use for the GUI?
------
The widgets use Avalonia project for the GUI.

## What side projects does the project use?
- [LibreHardwareMonitorLib](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) for showing cpu, gpu load and temperatures.
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) for work with json.
- [DeviceIoControl assembly](https://github.com/DKorablin/DeviceIoControl#deviceiocontrol-assembly) for getting SMART date and speed read/write a disks.