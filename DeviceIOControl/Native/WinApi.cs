using System;

namespace DeviceIOControl.Native
{
	/// <summary>Native structures</summary>
	public struct WinApi
	{
		/// <summary>The MEDIA_TYPE enumerators are used in conjunction with the IOCTL_DISK_FORMAT_TRACKS request to format the specified set of contiguous tracks on the disk.</summary>
		/// <remarks>Removable disks include zip drivers, jaz drives, magneto-optical (MO) drives, and LS-120 floppies as well as regular floppies.</remarks>
		public enum MEDIA_TYPE
		{
			/// <summary>Format is unknown</summary>
			Unknown,
			/// <summary>5.25", 1.2MB,  512 bytes/sector</summary>
			F5_1Pt2_512,
			/// <summary>3.5",  1.44MB, 512 bytes/sector</summary>
			F3_1Pt44_512,
			/// <summary>3.5",  2.88MB, 512 bytes/sector</summary>
			F3_2Pt88_512,
			/// <summary>3.5",  20.8MB, 512 bytes/sector</summary>
			F3_20Pt8_512,
			/// <summary>3.5",  720KB,  512 bytes/sector</summary>
			F3_720_512,
			/// <summary>5.25", 360KB,  512 bytes/sector</summary>
			F5_360_512,
			/// <summary>5.25", 320KB,  512 bytes/sector</summary>
			F5_320_512,
			/// <summary>5.25", 320KB,  1024 bytes/sector</summary>
			F5_320_1024,
			/// <summary>5.25", 180KB,  512 bytes/sector</summary>
			F5_180_512,
			/// <summary>5.25", 160KB,  512 bytes/sector</summary>
			F5_160_512,
			/// <summary>Removable media other than floppy</summary>
			RemovableMedia,
			/// <summary>Fixed hard disk media</summary>
			FixedMedia,
			/// <summary>3.5", 120M Floppy</summary>
			F3_120M_512,
			/// <summary>3.5" ,  640KB,  512 bytes/sector</summary>
			F3_640_512,
			/// <summary>5.25",  640KB,  512 bytes/sector</summary>
			F5_640_512,
			/// <summary>5.25",  720KB,  512 bytes/sector</summary>
			F5_720_512,
			/// <summary>3.5" ,  1.2Mb,  512 bytes/sector</summary>
			F3_1Pt2_512,
			/// <summary>3.5" ,  1.23Mb, 1024 bytes/sector</summary>
			F3_1Pt23_1024,
			/// <summary>5.25",  1.23MB, 1024 bytes/sector</summary>
			F5_1Pt23_1024,
			/// <summary>3.5" MO 128Mb   512 bytes/sector</summary>
			F3_128Mb_512,
			/// <summary>3.5" MO 230Mb   512 bytes/sector</summary>
			F3_230Mb_512,
			/// <summary>8",     256KB,  128 bytes/sector</summary>
			F8_256_128,
			/// <summary>3.5",   200M Floppy (HiFD)</summary>
			F3_200Mb_512,
			/// <summary>3.5",   240Mb Floppy (HiFD)</summary>
			F3_240M_512,
			/// <summary>3.5",   32Mb Floppy</summary>
			F3_32M_512,
		}

		/// <summary>Define the method codes for how buffers are passed for I/O and FS controls</summary>
		public enum METHOD : byte
		{
			/// <summary>Specifies the buffered I/O method, which is typically used for transferring small amounts of data per request.</summary>
			BUFFERED = 0,
			/// <summary>Specifies the direct I/O method, which is typically used for reading or writing large amounts of data, using DMA or PIO, that must be transferred quickly.</summary>
			IN_DIRECT = 1,
			/// <summary>Specifies the direct I/O method, which is typically used for reading or writing large amounts of data, using DMA or PIO, that must be transferred quickly.</summary>
			OUT_DIRECT = 2,
			/// <summary>
			/// Specifies neither buffered nor direct I/O.
			/// The I/O manager does not provide any system buffers or MDLs.
			/// The IRP supplies the user-mode virtual addresses of the input and output buffers that were specified to DeviceIoControl or IoBuildDeviceIoControlRequest, without validating or mapping them.
			/// </summary>
			NEITHER = 3,
		}

		/// <summary>The SID_NAME_USE enumeration contains values that specify the type of a security identifier (SID).</summary>
		public enum SID_NAME_USE
		{
			/// <summary>A user SID.</summary>
			SidTypeUser = 1,
			/// <summary>A group SID.</summary>
			SidTypeGroup,
			/// <summary>A domain SID.</summary>
			SidTypeDomain,
			/// <summary>An alias SID.</summary>
			SidTypeAlias,
			/// <summary>A SID for a well-known group.</summary>
			SidTypeWellKnownGroup,
			/// <summary>A SID for a deleted account.</summary>
			SidTypeDeletedAccount,
			/// <summary>A SID that is not valid.</summary>
			SidTypeInvalid,
			/// <summary>A SID of unknown type.</summary>
			SidTypeUnknown,
			/// <summary>A SID for a computer.</summary>
			SidTypeComputer,
			/// <summary>A mandatory integrity label SID.</summary>
			SidTypeLabel,
		}

		/// <summary>Access flags</summary>
		[Flags]
		public enum FILE_ACCESS_FLAGS : uint
		{
			/// <summary>Read</summary>
			GENERIC_READ = 0x80000000,
			/// <summary>Write</summary>
			GENERIC_WRITE = 0x40000000,
			/// <summary>Execute</summary>
			GENERIC_EXECUTE = 0x20000000,
			/// <summary>All</summary>
			GENERIC_ALL = 0x10000000,
		}

		/// <summary>Share</summary>
		[Flags]
		public enum FILE_SHARE : uint
		{
			/// <summary>
			/// Enables subsequent open operations on a file or device to request read access.
			/// Otherwise, other processes cannot open the file or device if they request read access.
			/// </summary>
			READ = 0x00000001,
			/// <summary>
			/// Enables subsequent open operations on a file or device to request write access.
			/// Otherwise, other processes cannot open the file or device if they request write access.
			/// </summary>
			WRITE = 0x00000002,
			/// <summary>
			/// Enables subsequent open operations on a file or device to request delete access.
			/// Otherwise, other processes cannot open the file or device if they request delete access.
			/// If this flag is not specified, but the file or device has been opened for delete access, the function fails.
			/// </summary>
			DELETE = 0x00000004,
		}

		/// <summary>Defines the access check value for any access. </summary>
		[Flags]
		public enum FILE_ACCESS : ushort
		{
			/// <summary>Request all access.</summary>
			ANY_ACCESS = 0,
			/// <summary>Request read access.</summary>
			/// <remarks>Can be used with FILE_WRITE_ACCESS.</remarks>
			READ_ACCESS = 0x0001,
			/// <summary>Request write access.</summary>
			/// <remarks>Can be used with FILE_READ_ACCESS.</remarks>
			WRITE_ACCESS = 0x0002,
		}

		/// <summary>Disposition</summary>
		public enum CreateDisposition : uint
		{
			/// <summary>Create new</summary>
			CREATE_NEW = 1,
			/// <summary>Create always</summary>
			CREATE_ALWAYS = 2,
			/// <summary>Open exising</summary>
			OPEN_EXISTING = 3,
			/// <summary>Open always</summary>
			OPEN_ALWAYS = 4,
			/// <summary>Truncate existing</summary>
			TRUNCATE_EXISTING = 5,
		}

		/// <summary>Drive types</summary>
		public enum DRIVE : uint
		{
			/// <summary>The drive type cannot be determined.</summary>
			UNKNOWN = 0,
			/// <summary>The root directory does not exist.</summary>
			NO_ROOT_DIR = 1,
			/// <summary>The drive can be removed from the drive.</summary>
			REMOVABLE = 2,
			/// <summary>The disk cannot be removed from the drive.</summary>
			FIXED = 3,
			/// <summary>The drive is a remote (network) drive.</summary>
			REMOTE = 4,
			/// <summary>The drive is a CD-ROM drive.</summary>
			CDROM = 5,
			/// <summary>The drive is a RAM disk.</summary>
			RAMDISK = 6,
		}
	}
}
