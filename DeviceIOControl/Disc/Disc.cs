using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using DeviceIOControl.Native;
using DeviceIOControl.Disc.Smart;

namespace DeviceIOControl.Disc
{
    public class Disc
    {
		private readonly DeviceIoControl _device;
		private SmartInfoCollection _smart;
		private DiscApi.GETVERSIONINPARAMS? _version;
		private DiscApi.DISK_GEOMETRY_EX? _driveGeometryEx;
		private Boolean? _isWritable;
		/// <summary>Device</summary>
		private DeviceIoControl Device { get { return this._device; } }

		/// <summary>Returns version information, a capabilities mask, and a bitmask for the device.</summary>
		/// <remarks>This IOCTL must be handled by drivers that support Self-Monitoring Analysis and Reporting Technology (SMART).</remarks>
		public DiscApi.GETVERSIONINPARAMS? Version
		{
			get
			{
				if (this._version == null)
				{
					DiscApi.GETVERSIONINPARAMS prms;
					if (this.Device.IoControl<DiscApi.GETVERSIONINPARAMS>(Constant.IOCTL_DISC.SMART_GET_VERSION, null, out prms))
						this._version = prms;
					else
						this._version = prms;
				}
				return this._version.Value.Equals(default(DiscApi.GETVERSIONINPARAMS)) ? (DiscApi.GETVERSIONINPARAMS?)null : this._version.Value;
			}
		}

		/// <summary>Returns information about the physical disk's geometry (media type, number of cylinders, tracks per cylinder, sectors per track, and bytes per sector).</summary>
		public DiscApi.DISK_GEOMETRY_EX DriveGeometryEx
		{
			get
			{
				if (this._driveGeometryEx == null)
					this._driveGeometryEx = this.Device.IoControl<DiscApi.DISK_GEOMETRY_EX>(
						Constant.IOCTL_DISC.GET_DRIVE_GEOMETRY_EX,
						null);
				return this._driveGeometryEx.Value;
			}
		}

		/// <summary>Self-Monitoring Analysis and Reporting Technology info</summary>
		/// <remarks>Smart can be null if SAMRT commands not supported</remarks>
		public SmartInfoCollection Smart
		{
			get
			{
				if (this._smart == null && this.Version != null && this.Version.Value.IsSmartSupported)
					this._smart = new SmartInfoCollection(this.Device);
				return this._smart;
			}
		}

		/// <summary>Determines whether the specified disk is writable.</summary>
		/// <exception cref="Win32Exception">WinAPI operation fails.</exception>
		public Boolean IsWritable
		{
			get
			{
				if (this._isWritable == null)
					if (this.Device.IoControl(Constant.IOCTL_DISC.IS_WRITABLE))
						this._isWritable = true;
					else
					{
						Int32 error = Marshal.GetLastWin32Error();
						if (error == (Int32)Constant.ERROR.WRITE_PROTECT)
							this._isWritable = false;
						else
							throw new Win32Exception(error);
					}
				return this._isWritable.Value;
			}
		}

		/// <summary>Create instance of disc IO control commands class</summary>
		/// <param name="device">Device</param>
		internal Disc(DeviceIoControl device)
		{
			this._device = device;
		}

		/// <summary>Get disc performance managed</summary>
		/// <returns>Perfomance manager</returns>
		public Performance GetDiscPerformance()
		{
			return new Performance(this.Device);
		}
	}
}
