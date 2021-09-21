using System;
using DeviceIOControl.Native;

namespace DeviceIOControl.Disc
{
	/// <summary>Collect perfomance counters from HDD</summary>
	public class Performance : IDisposable
	{
		private readonly DeviceIoControl _device;
		private Boolean _counterIsActive = false;

		/// <summary>Device</summary>
		private DeviceIoControl Device { get { return this._device; } }

		/// <summary>Create instance of deviceperfomance counters</summary>
		/// <param name="device">device</param>
		internal Performance(DeviceIoControl device)
		{
			this._device = device;
		}
		/// <summary>Query performance counters</summary>
		/// <returns>Performance counters</returns>
		public DiscApi.DISK_PERFORMANCE QueryPerformanceInfo()
		{
			DiscApi.DISK_PERFORMANCE result = this.Device.IoControl<DiscApi.DISK_PERFORMANCE>(
				Constant.IOCTL_DISC.PERFORMANCE,
				null);
			this._counterIsActive = true;
			return result;
		}
		/// <summary>Close perfomace counters manager</summary>
		public void Dispose()
		{
			if (this._counterIsActive)
			{
				this.Device.IoControl<DiscApi.DISK_PERFORMANCE>(
					Constant.IOCTL_DISC.PERFORMANCE_OFF,
					null);
				this._counterIsActive = false;
			}
		}
	}
}
