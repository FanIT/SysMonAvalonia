using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;
using DeviceIOControl.Native;

namespace DeviceIOControl.FileSystem
{
	/// <summary>File system control commands</summary>
	public class FileSystem
	{
		private readonly DeviceIoControl _device;
		private FsctlApi.NTFS_VOLUME_DATA_BUFFER? _volumeData;

		/// <summary>Device</summary>
		private DeviceIoControl Device { get { return this._device; } }

		/// <summary>Represents volume data.</summary>
		public FsctlApi.NTFS_VOLUME_DATA_BUFFER VolumeData
		{
			get
			{
				return (this._volumeData == null
					? this._volumeData = this.Device.IoControl<FsctlApi.NTFS_VOLUME_DATA_BUFFER>(
						Constant.FSCTL.GET_NTFS_VOLUME_DATA, null)
					: this._volumeData).Value;
			}
		}

		/// <summary>Determines whether the specified volume is mounted</summary>
		public Boolean IsVolumeMounted
		{
			get { return this.Device.IoControl(Constant.FSCTL.IS_VOLUME_MOUNTED); }
		}

		/// <summary>Create instance of file system IO commands class</summary>
		/// <param name="device">Device</param>
		public FileSystem(DeviceIoControl device)
		{
			this._device = device;
		}

		/// <summary>Retrieves a bitmap of occupied and available clusters on a volume.</summary>
		/// <returns>Represents the occupied and available clusters on a disk.</returns>
		public IEnumerable<FsctlApi.VOLUME_BITMAP_BUFFER> GetVolumeBitmap()
		{
			UInt64 startingLcn = 0;
			UInt32 bytesReturned;
			Boolean moreDataAvailable = false;

			do
			{
				FsctlApi.VOLUME_BITMAP_BUFFER part = this.GetVolumeBitmap(startingLcn, out bytesReturned, out moreDataAvailable);
				if (moreDataAvailable)
					startingLcn = bytesReturned;
				yield return part;
			} while (moreDataAvailable);
		}

		/// <summary>Метод не работает и вываливается по: 0x00000057 - The parameter is incorrect</summary>
		/// <param name="accountName"></param>
		/// <returns></returns>
		public FsctlApi.FIND_BY_SID_OUTPUT FindFilesBySid(String accountName)
		{
			if (String.IsNullOrEmpty(accountName))
				throw new ArgumentNullException("accountName");

			FsctlApi.FIND_BY_SID_DATA fsInParams = new FsctlApi.FIND_BY_SID_DATA();
			fsInParams.Restart = 1;
			fsInParams.Sid = Methods.LookupAccountName(accountName);

			//var currentIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();
			//System.Security.Principal.SecurityIdentifier managedSid = new System.Security.Principal.SecurityIdentifier(fsInParams.Sid, 0);

			UInt32 bytesReturned;
			FsctlApi.FIND_BY_SID_OUTPUT result;
			Boolean resultCode = this.Device.IoControl<FsctlApi.FIND_BY_SID_OUTPUT>(
				Constant.FSCTL.FIND_FILES_BY_SID,
				fsInParams,
				out bytesReturned,
				out result);

			if (resultCode)
				return result;
			else
			{
				Int32 error = Marshal.GetLastWin32Error();
				throw new Win32Exception(error);
			}
		}

		/// <summary>Retrieves a bitmap of occupied and available clusters on a volume.</summary>
		/// <param name="startingLcn">
		/// The LCN from which the operation should start when describing a bitmap.
		/// This member will be rounded down to a file-system-dependent rounding boundary, and that value will be returned.
		/// Its value should be an integral multiple of eight.
		/// </param>
		/// <param name="bytesReturned">Returned length of bytes</param>
		/// <param name="moreDataAvailable">Not all data readed</param>
		/// <exception cref="Win32Exception">Win32 Exception occured. See winerror.h for details</exception>
		/// <returns>Represents the occupied and available clusters on a disk.</returns>
		public FsctlApi.VOLUME_BITMAP_BUFFER GetVolumeBitmap(UInt64 startingLcn, out UInt32 bytesReturned, out Boolean moreDataAvailable)
		{
			FsctlApi.STARTING_LCN_INPUT_BUFFER fsInParams = new FsctlApi.STARTING_LCN_INPUT_BUFFER();
			fsInParams.StartingLcn = startingLcn;//0xA000;

			FsctlApi.VOLUME_BITMAP_BUFFER result;
			Boolean resultCode = this.Device.IoControl<FsctlApi.VOLUME_BITMAP_BUFFER>(
				Constant.FSCTL.GET_VOLUME_BITMAP,
				fsInParams,
				out bytesReturned,
				out result);

			if (resultCode)
				moreDataAvailable = false;
			else
			{
				Int32 error = Marshal.GetLastWin32Error();
				if (error == (Int32)Constant.ERROR.MORE_DATA)
					moreDataAvailable = true;
				else
					throw new Win32Exception(error);
			}
			return result;
		}

		/// <summary>Retrieves the information from various file system performance counters.</summary>
		/// <returns>Contains statistical information from the file system.</returns>
		public FsctlApi.FILESYSTEM_STATISTICS GetStatistics()
		{
			UInt32 bytesReturned;
			FsctlApi.FILESYSTEM_STATISTICS result = this.Device.IoControl<FsctlApi.FILESYSTEM_STATISTICS>(
				Constant.FSCTL.FILESYSTEM_GET_STATISTICS,
				null,
				out bytesReturned);

			/*const UInt32 SizeOfFileSystemStat = 56;
			using(BytesReader reader = new BytesReader(result.Data))
			{
				UInt32 padding = 0;
				switch(result.FileSystemType)
				{
				case FsctlAPI.FILESYSTEM_STATISTICS.FILESYSTEM_STATISTICS_TYPE.NTFS:
					FsctlAPI.NTFS_STATISTICS ntfsStat = reader.BytesToStructure<FsctlAPI.NTFS_STATISTICS>(ref padding);
					break;
				case FsctlAPI.FILESYSTEM_STATISTICS.FILESYSTEM_STATISTICS_TYPE.FAT:
					FsctlAPI.FAT_STATISTICS fatStat = reader.BytesToStructure<FsctlAPI.FAT_STATISTICS>(ref padding);
					break;
				case FsctlAPI.FILESYSTEM_STATISTICS.FILESYSTEM_STATISTICS_TYPE.EXFAT:
					FsctlAPI.EXFAT_STATISTICS exFatStat = reader.BytesToStructure<FsctlAPI.EXFAT_STATISTICS>(ref padding);
					break;
				}
			}*/
			return result;
		}

		/// <summary>Retrieves the information from various file system performance counters</summary>
		/// <remarks>Support for this structure started with Windows 10</remarks>
		/// <returns>Contains statistical information from the file system</returns>
		public FsctlApi.FILESYSTEM_STATISTICS_EX GetStatisticsEx()
		{
			UInt32 bytesReturned;
			FsctlApi.FILESYSTEM_STATISTICS_EX result = this.Device.IoControl<FsctlApi.FILESYSTEM_STATISTICS_EX>(
				Constant.FSCTL.FILESYSTEM_GET_STATISTICS,
				null,
				out bytesReturned);

			return result;
		}

		/// <summary>
		/// Locks a volume if it is not in use.
		/// A locked volume can be accessed only through handles to the file object (*hDevice) that locks the volume.
		/// </summary>
		/// <exception cref="Win32Exception">Device exception</exception>
		/// <returns>Lock action status</returns>
		public void LockVolume()
		{
			this.Device.IoControl<IntPtr>(Constant.FSCTL.LOCK_VOLUME, null);
		}

		/// <summary>Unlocks a volume.</summary>
		/// <exception cref="Win32Exception">Device exception</exception>
		/// <returns>Unlock action status</returns>
		public void UnlockVolume()
		{
			this.Device.IoControl<IntPtr>(Constant.FSCTL.UNLOCK_VOLUME, null);
		}

		/// <summary>Dismounts a volume regardless of whether or not the volume is currently in use.</summary>
		/// <exception cref="Win32Exception">Device exception</exception>
		/// <returns>Dismount action status</returns>
		public void DismountVolume()
		{
			this.Device.IoControl<IntPtr>(Constant.FSCTL.DISMOUNT_VOLUME, null);
		}
	}
}
