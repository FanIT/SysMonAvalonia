using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using DeviceIOControl.Native;

namespace DeviceIOControl.Volume
{
	/// <summary>Volume IO control commands</summary>
	public class Volume
	{
		#region Fields
		private readonly DeviceIoControl _device;
		private Boolean? _isClustered;
		private VolumeApi.VOLUME_DISK_EXTENTS? _diskExtents;
		#endregion Fields

		/// <summary>Device</summary>
		private DeviceIoControl Device { get { return this._device; } }

		/// <summary>Represents a physical location on a disk.</summary>
		/// <remarks>Вызов не работает в Windows 7</remarks>
		public VolumeApi.VOLUME_DISK_EXTENTS DiskExtents
		{
			get
			{
				if (this._diskExtents == null)
					this._diskExtents = this.Device.IoControl<VolumeApi.VOLUME_DISK_EXTENTS>(
						Constant.IOCTL_VOLUME.GET_VOLUME_DISK_EXTENTS,
						null);
				return this._diskExtents.Value;
			}
		}

		/// <summary>Determines whether the specified volume is clustered.</summary>
		/// <exception cref="Win32Exception">The volume is offline or Cluster service is not installed.</exception>
		public Boolean IsClustered
		{
			get
			{
				if (this._isClustered == null)
					if (this.Device.IoControl(Constant.IOCTL_VOLUME.IS_CLUSTERED)) this._isClustered = true;
					else
					{
						Int32 error = Marshal.GetLastWin32Error();
						if (error == (Int32)Constant.ERROR.NO_ERROR)
							this._isClustered = false;
						else
							throw new Win32Exception(error);
					}
				return this._isClustered.Value;
			}
		}

		/// <summary>Create instance of Volume IO control commands</summary>
		/// <param name="device">Device</param>
		internal Volume(DeviceIoControl device)
		{
			this._device = device;
		}
	}
}
