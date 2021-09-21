using DeviceIOControl.Native;

namespace DeviceIOControl
{
	public struct LogicalDrive
	{
		private readonly string _name;

		private readonly WinApi.DRIVE _type;

		public string Name => _name;

		public WinApi.DRIVE Type => _type;

		internal LogicalDrive(string name, WinApi.DRIVE type)
		{
			this = default(LogicalDrive);
			_name = name;
			_type = type;
		}
	}
}
