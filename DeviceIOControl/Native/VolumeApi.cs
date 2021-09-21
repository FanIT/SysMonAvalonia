using System;
using System.Runtime.InteropServices;

namespace DeviceIOControl.Native
{
	/// <summary>Volume IOCTL structures</summary>
	public struct VolumeApi
	{
		/// <summary>Represents a physical location on a disk. It is the output buffer for the <see cref="T:Constant.IOCTL_VOLUME.GET_VOLUME_DISK_EXTENTS"/> control code.</summary>
		/// <remarks>http://msdn.microsoft.com/en-us/library/windows/desktop/aa365727%28v=vs.85%29.aspx</remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct VOLUME_DISK_EXTENTS
		{
			/// <summary>Represents a disk extent.</summary>
			[StructLayout(LayoutKind.Sequential)]
			public struct DISK_EXTENT
			{
				/// <summary>The number of the disk that contains this extent.</summary>
				/// <remarks>This is the same number that is used to construct the name of the disk, for example, the X in "\\?\PhysicalDriveX" or "\\?\HarddiskX".</remarks>
				public UInt32 DiskNumber;
				/// <summary>The offset from the beginning of the disk to the extent, in bytes.</summary>
				public UInt64 StartingOffset;
				/// <summary>The number of bytes in this extent.</summary>
				public UInt64 ExtentLength;
			}
			/// <summary>The number of disks in the volume (a volume can span multiple disks).</summary>
			/// <remarks>
			/// An extent is a contiguous run of sectors on one disk.
			/// When the number of extents returned is greater than one (1), the error code ERROR_MORE_DATA is returned.
			/// You should call DeviceIoControl again, allocating enough buffer space based on the value of NumberOfDiskExtents after the first DeviceIoControl call.
			/// </remarks>
			public UInt32 NumberOfDiskExtents;
			/// <summary>An array of DISK_EXTENT structures.</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public DISK_EXTENT[] Extents;//TODO: Если дисков больше 10, то метод DeviceIoControl вернёт ERROR_MORE_DATA.
		}

	}
}
