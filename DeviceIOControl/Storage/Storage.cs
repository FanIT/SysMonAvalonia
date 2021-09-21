using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using DeviceIOControl.Native;

namespace DeviceIOControl.Storage
{
	/// <summary>Storage IO control commands</summary>
	public class Storage
	{
		#region Fields
		private readonly DeviceIoControl _device;
		private Properties _properties;
		private StorageApi.STORAGE_DEVICE_NUMBER? _deviceNumber;
		private StorageApi.GET_MEDIA_TYPES? _mediaTypesEx;
		private StorageApi.MEDIA_SERIAL_NUMBER_DATA? _serialNumber;
		private StorageApi.STORAGE_READ_CAPACITY? _readCapacity;
		#endregion Fields

		/// <summary>Device</summary>
		private DeviceIoControl Device { get { return this._device; } }

		/// <summary>Device descriptors</summary>
		public Properties Properties
		{
			get
			{
				return this._properties == null
					? this._properties = new Properties(this.Device)
					: this._properties;
			}
		}

		/// <summary>Contains information about a device.</summary>
		public StorageApi.STORAGE_DEVICE_NUMBER DeviceNumber
		{
			get
			{
				return (this._deviceNumber == null
					? this._deviceNumber = this.Device.IoControl<StorageApi.STORAGE_DEVICE_NUMBER>(
						Constant.IOCTL_STORAGE.GET_DEVICE_NUMBER,
						null)
					: this._deviceNumber).Value;
			}
		}
		/// <summary>Queries the USB generic parent driver for the serial number of a USB device.</summary>
		/// <remarks>
		/// If a USB device has a CSM-1 content security interface, a USB client driver can query for its serial number using this request.
		/// USB client drivers that help implement a digital rights management (DRM) system can use this information to ensure that only legitimate customers have access to digitized intellectual property.
		/// </remarks>
		public StorageApi.MEDIA_SERIAL_NUMBER_DATA SerialNumber
		{
			get
			{
				return (this._serialNumber == null
					? this._serialNumber = this.Device.IoControl<StorageApi.MEDIA_SERIAL_NUMBER_DATA>(
						Constant.IOCTL_STORAGE.GET_MEDIA_SERIAL_NUMBER,
						null)
					: this._serialNumber).Value;
			}
		}

		/// <summary>Contains information about the media types supported by a device.</summary>
		public StorageApi.GET_MEDIA_TYPES MediaTypesEx
		{
			get
			{
				return (this._mediaTypesEx == null
					? this._mediaTypesEx = this.Device.IoControl<StorageApi.GET_MEDIA_TYPES>(
						Constant.IOCTL_STORAGE.GET_MEDIA_TYPES_EX,
						null)
					: this._mediaTypesEx).Value;
			}
		}

		/// <summary>Retrieves the geometry information for the device.</summary>
		public StorageApi.STORAGE_READ_CAPACITY ReadCapacity
		{
			get
			{
				return (this._readCapacity == null
					? this._readCapacity = this.Device.IoControl<StorageApi.STORAGE_READ_CAPACITY>(
						Constant.IOCTL_STORAGE.READ_CAPACITY, null)
					: this._readCapacity).Value;
			}
		}

		/// <summary>Determines whether the media has changed on a removable-media device that the caller has opened for read or write access.</summary>
		public Boolean CheckVerify
		{
			get
			{
				Boolean result = this.Device.IoControl(Constant.IOCTL_STORAGE.CHECK_VERIFY);
				if (result)
					return true;
				else
				{
					Int32 error = Marshal.GetLastWin32Error();
					if (error == (Int32)Constant.ERROR.NOT_READY)
						return false;
					else throw new Win32Exception(error);
				}
			}
		}
		/// <summary>Determines whether the media has changed on a removable-media device - the caller has opened with FILE_READ_ATTRIBUTES.</summary>
		public Boolean CheckVerify2
		{
			get
			{
				Boolean result = this.Device.IoControl(Constant.IOCTL_STORAGE.CHECK_VERIFY2);
				if (result)
					return true;
				else
				{
					Int32 error = Marshal.GetLastWin32Error();
					if (error == (Int32)Constant.ERROR.NOT_READY)
						return false;
					else throw new Win32Exception(error);
				}
			}
		}

		/// <summary>Create instance of storage IO commands class</summary>
		/// <param name="device">Device</param>
		internal Storage(DeviceIoControl device)
		{
			this._device = device;
		}

		/// <summary>Enables or disables the mechanism that ejects media, for those devices possessing that locking capability.</summary>
		/// <param name="prevent">Prevent device to eject media</param>
		public void PreventMediaRemoval(Boolean prevent)
		{
			this.Device.IoControl<IntPtr>(Constant.IOCTL_STORAGE.MEDIA_REMOVAL, prevent);
		}

		/// <summary>Causes the device to eject the media if the device supports ejection capabilities.</summary>
		/// <exception cref="Win32Exception">Device exception</exception>
		public void EjectMedia()
		{
			this.Device.IoControl<IntPtr>(Constant.IOCTL_STORAGE.EJECT_MEDIA, null);
		}

		/// <summary>Retrieves the hotplug configuration of the specified device.</summary>
		/// <returns>Hotplug information for a device.</returns>
		public StorageApi.STORAGE_HOTPLUG_INFO GetHotPlugInfo()
		{
			return this.Device.IoControl<StorageApi.STORAGE_HOTPLUG_INFO>(
				Constant.IOCTL_STORAGE.GET_HOTPLUG_INFO,
				null);
		}

		/// <summary>Polls for a prediction of device failure.</summary>
		/// <summary>Report whether a device is currently predicting a failure.</summary>
		public StorageApi.STORAGE_PREDICT_FAILURE PredictFailure()
		{
			return this.Device.IoControl<StorageApi.STORAGE_PREDICT_FAILURE>(
				Constant.IOCTL_STORAGE.PREDICT_FAILURE,
				null);
		}
	}
}
