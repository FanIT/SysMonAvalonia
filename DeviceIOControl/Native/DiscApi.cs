using System;
using System.Runtime.InteropServices;
using System.Collections;

namespace DeviceIOControl.Native
{
	/// <summary>Disc IOCTL structures</summary>
	public struct DiscApi
	{
		/// <summary>IDE registers</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct IDEREGS
		{
			/// <summary>Feature register defines for SMART "sub commands"</summary>
			public enum SMART : byte
			{
				/// <summary>Retrieve the device attributes</summary>
				READ_ATTRIBUTES = 0xD0,
				/// <summary>Retrieve threshold values that indicate when a drive is about to fail.</summary>
				READ_THRESHOLDS = 0xD1,
				/// <summary>Enables the optional attribute autosave feature of the device when set to 1. Disables this feature when set to 0.</summary>
				ENABLE_DISABLE_AUTOSAVE = 0xD2,
				/// <summary>Instructs the device to save its attribute values to the device's non-volatile memory.</summary>
				SAVE_ATTRIBUTE_VALUES = 0xD3,
				/// <summary>Causes the device to begin collecting SMART data in off-line mode or execute a self-diagnostic test routine in either captive or off-line mode.</summary>
				EXECUTE_OFFLINE_DIAGS = 0xD4,
				/// <summary>Retrieves the indicated log.</summary>
				SMART_READ_LOG = 0xD5,
				/// <summary>Writes the indicated number of 512-byte data sectors to the indicated log.</summary>
				SMART_WRITE_LOG = 0xD6,
				/// <summary>Enables SMART functionality on the device.</summary>
				ENABLE_SMART = 0xD8,
				/// <summary>Disables SMART functionality on the device.</summary>
				DISABLE_SMART = 0xD9,
				/// <summary>Retrieves the reliability status of the device.</summary>
				RETURN_SMART_STATUS = 0xDA,
				/// <summary>Enables offline mode when set to 1. Disables offline mode when 0.</summary>
				ENABLE_DISABLE_AUTO_OFFLINE = 0xDB,
			}
			/// <summary>Valid values for the bCommandReg member of <see cref="T:IDEREGS"/>.</summary>
			public enum IDE : byte
			{
				/// <summary>Returns ID sector for ATAPI.</summary>
				ATAPI_ID_CMD = 0xA1,
				/// <summary>Returns ID sector for ATA.</summary>
				ID_CMD = 0xEC,
				/// <summary>Performs SMART cmd.</summary>
				/// <remarks>Requires valid bFeaturesReg, bCylLowReg, and bCylHighReg.</remarks>
				SMART_CMD = 0xB0,
			}
			/// <summary>Cylinder register defines for SMART command</summary>
			public enum SMART_CYL : byte
			{
				/// <summary>Low culinder register</summary>
				LOW = 0x4F,
				/// <summary>Hi cylinder register</summary>
				HI = 0xC2,
			}
			/// <summary>
			/// Holds the contents of the Features register.
			/// This register is used to specify Self-Monitoring Analysis and Reporting Technology (SMART) commands.
			/// </summary>
			public SMART bFeaturesReg;
			/// <summary>Holds the contents of the sector count register. IDE sector count register.</summary>
			public Byte bSectorCountReg;
			/// <summary>Holds the contents of the sector number register. </summary>
			public Byte bSectorNumberReg;
			/// <summary>Holds the contents of the IDE low-order cylinder register.</summary>
			public SMART_CYL bCylLowReg;
			/// <summary>Holds the contents of the IDE high-order cylinder register.</summary>
			public SMART_CYL bCylHighReg;
			/// <summary>Holds the contents of the IDE drive/head register. </summary>
			public Byte bDriveHeadReg;
			/// <summary>Holds the contents of the IDE command register.</summary>
			public IDE bCommandReg;
			/// <summary>Reserved for future use. Should always be zero.</summary>
			public Byte bReserved;
		}
		/// <summary>The SENDCMDINPARAMS structure contains the input parameters for the <see cref="T:Constant.IOCTL_DISC.SMART_SEND_DRIVE_COMMAND"/> request.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct SENDCMDINPARAMS
		{
			/// <summary>Contains the buffer size, in bytes.</summary>
			public UInt32 cBufferSize;
			/// <summary>Contains a IDEREGS structure used to report the contents of the IDE controller registers.</summary>
			public DiscApi.IDEREGS irDriveRegs;
			/// <summary>
			/// The bDriveNumber member is opaque. Do not use it.
			/// The operating system ignores this member, because the physical drive that receives the request depends on the handle that the caller uses when making the request.
			/// </summary>
			public Byte bDriveNumber;
			/// <summary>Reserved.</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			public Byte[] bReserved;
			/// <summary>Reserved.</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			public UInt32[] dwReserved;
			/// <summary>Pointer to the input buffer.</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
			public Byte[] bBuffer;
			/// <summary>Create in params structure</summary>
			/// <param name="isSmart">Create for S.M.A.R.T. request</param>
			/// <returns></returns>
			public static SENDCMDINPARAMS Create(Boolean isSmart)
			{
				DiscApi.SENDCMDINPARAMS result = new DiscApi.SENDCMDINPARAMS();
				result.irDriveRegs.bSectorCountReg = 1;
				result.irDriveRegs.bSectorNumberReg = 1;
				/*if(deviceId.HasValue)
					result.irDriveRegs.bDriveHeadReg = (Byte)(0xA0 | ((deviceId & 1) << 4));*/

				if (isSmart)
				{
					result.irDriveRegs.bCylLowReg = DiscApi.IDEREGS.SMART_CYL.LOW;
					result.irDriveRegs.bCylHighReg = DiscApi.IDEREGS.SMART_CYL.HI;
					result.irDriveRegs.bCommandReg = DiscApi.IDEREGS.IDE.SMART_CMD;
				}
				return result;
			}
		}
		/// <summary>The DRIVERSTATUS structure is used in conjunction with the SENDCMDOUTPARAMS structure and the <see cref="T:Constant.IOCTL_STORAGE.SMART_SEND_DRIVE_COMMAND"/> request to retrieve data returned by a Self-Monitoring Analysis and Reporting Technology (SMART) command.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct DRIVERSTATUS
		{
			/// <summary>SMART error codes</summary>
			public enum SMART_ERROR : byte
			{
				/// <summary>Command executed successfully</summary>
				NO_ERROR = 0,
				/// <summary>Error from IDE controller</summary>
				IDE_ERROR = 1,
				/// <summary>Invalid command flag</summary>
				INVALID_FLAG = 2,
				/// <summary>Invalid command byte</summary>
				INVALID_COMMAND = 3,
				/// <summary>Bad buffer (null, invalid addr..)</summary>
				INVALID_BUFFER = 4,
				/// <summary>Drive number not valid</summary>
				INVALID_DRIVE = 5,
				/// <summary>Invalid IOCTL</summary>
				INVALID_IOCTL = 6,
				/// <summary>Could not lock user's buffer</summary>
				ERROR_NO_MEM = 7,
				/// <summary>Some IDE Register not valid</summary>
				INVALID_REGISTER = 8,
				/// <summary>Invalid cmd flag set</summary>
				NOT_SUPPORTED = 9,
				/// <summary>Cmd issued to device not present although drive number is valid</summary>
				NO_IDE_DEVICE = 10,
			}

			/// <summary>Error code from driver, or 0 if no error.</summary>
			public SMART_ERROR bDriverError;
			/// <summary>Contents of IDE Error register. Only valid when bDriverError is SMART_IDE_ERROR.</summary>
			public Byte bIDEError;
			/// <summary>Reserved.</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public Byte[] bReserved;
			/// <summary>Reserved.</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public UInt32[] dwReserved;
		}
		/// <summary>The SENDCMDOUTPARAMS structure is used in conjunction with the <see cref="T:Constant.IOCTL_DISC.SMART_SEND_DRIVE_COMMAND"/> request to retrieve data returned by a Self-Monitoring Analysis and Reporting Technology (SMART) command.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct SENDCMDOUTPARAMS
		{
			/// <summary>Contains the size in bytes of the buffer pointed to by bBuffer.</summary>
			public UInt32 cBufferSize;
			/// <summary>Contains a DRIVERSTATUS structure that indicates the driver status.</summary>
			public DiscApi.DRIVERSTATUS DriverStatus;
			/// <summary>Pointer to a buffer in which to store the data read from the drive.</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = Constant.BUFFER_SIZE)]
			public Byte[] bBuffer;
		}
		/// <summary>Sector identity</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct IDSECTOR
		{
			/// <summary>Device type</summary>
			public enum DeviceType
			{
				/// <summary>Unknown device type</summary>
				Unknown,
				/// <summary>Non-magnetic</summary>
				NonMagnetic,
				/// <summary>Removable media device</summary>
				Removable,
				/// <summary>Fixed drive</summary>
				Fixed,
			}
			/// <summary>banana</summary>
			public UInt16 wGenConfig;
			/// <summary>Number of cylinders</summary>
			public UInt16 wNumCyls;
			/// <summary>banana</summary>
			public UInt16 wReserved;
			/// <summary>Number of heads</summary>
			public UInt16 wNumHeads;
			/// <summary>Bytes per track</summary>
			public UInt16 wBytesPerTrack;
			/// <summary>Bytes per sector</summary>
			public UInt16 wBytesPerSector;
			/// <summary>Sectors per track</summary>
			public UInt16 wSectorsPerTrack;//14
			/// <summary>banana</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			public UInt16[] wVendorUnique;//20
			/// <summary>Serial number</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
			public Char[] sSerialNumber;
			/// <summary>banana</summary>
			public UInt16 wBufferType;
			/// <summary>banana</summary>
			public UInt16 wBufferSize;
			/// <summary>banana</summary>
			public UInt16 wECCSize;//66
			/// <summary>Firmware version</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			public Char[] sFirmwareRev;
			/// <summary>Model number</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
			public Char[] sModelNumber;//162
			/// <summary>banana</summary>
			public UInt16 wMoreVendorUnique;
			/// <summary>banana</summary>
			public UInt16 wDoubleWordIO;
			/// <summary>Capabilities</summary>
			public UInt16 wCapabilities;
			/// <summary>banana</summary>
			public UInt16 wReserved1;
			/// <summary>Device PIO mode</summary>
			public UInt16 wPIOTiming;
			/// <summary>banana</summary>
			public UInt16 wDMATiming;
			/// <summary>banana</summary>
			public UInt16 wBS;//176
			/// <summary>Current number of cylinders</summary>
			public UInt16 wNumCurrentCyls;
			/// <summary>Current number of heads</summary>
			public UInt16 wNumCurrentHeads;
			/// <summary>Current sectors per track</summary>
			public UInt16 wNumCurrentSectorsPerTrack;
			/// <summary>Current sector capacity</summary>
			public UInt32 ulCurrentSectorCapacity;
			/// <summary>banana</summary>
			public UInt16 wMultSectorStuff;
			/// <summary>User addressable space</summary>
			public UInt32 ulTotalAddressableSectors;
			/// <summary>banana</summary>
			public UInt16 wSingleWordDMA;
			/// <summary>banana</summary>
			public UInt16 wMultiWordDMA;
			/// <summary>banana</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			public Byte[] bReserved;

			/// <summary>Serial number</summary>
			public String SerialNumber { get { return Utils.SwapChars(this.sSerialNumber); } }
			/// <summary>Firmware version</summary>
			public String FirmwareRev { get { return Utils.SwapChars(this.sFirmwareRev); } }
			/// <summary>Model number</summary>
			public String ModelNumber { get { return Utils.SwapChars(this.sModelNumber); } }
			/// <summary>Device type</summary>
			public DeviceType Type
			{
				get
				{
					DeviceType result = DeviceType.Unknown;
					if ((this.wGenConfig & 0x80) == 0x40)
						result = DeviceType.Removable;
					else if ((this.wGenConfig & 0x40) == 0x40)
						result = DeviceType.Fixed;
					/*if((this.wGenConfig & 0x80C0) > 0)
					{//TODO: Тип устройства определяется неправильно
						if((this.wGenConfig & 0x8000) > 0)
							result = DeviceType.NonMagnetic;
						if((this.wGenConfig & 0x80) > 0)
							result = DeviceType.Removable;
						if((this.wGenConfig & 0x40) > 0)
							result = DeviceType.Fixed;
					}*/
					return result;
				}
			}
		}
		/// <summary>The following structure defines the structure of a Drive Attribute</summary>
		[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
		public struct DRIVEATTRIBUTE
		{
			/// <summary>Identifies which attribute</summary>
			public Byte bAttrID;
			/// <summary>see bit definitions below</summary>
			public UInt16 wStatusFlags;
			/// <summary>Current normalized value</summary>
			public Byte bAttrValue;
			/// <summary>How bad has it ever been?</summary>
			public Byte bWorstValue;
			/// <summary>Attribute raw value</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
			public Byte[] bRawValue;
			/// <summary>Reserved for future use</summary>
			public Byte bReserved;
			/// <summary>Attribute name</summary>
			public String AttributeName
			{
				get
				{

					String value;
					return Constant.AttributeNames.TryGetValue(this.bAttrID, out value) ? value : Constant.AttributeNames[0];
				}
			}
			/// <summary>Преобразованное значение</summary>
			public Int64 RawValue
			{
				get
				{
					Byte[] rawValue = new Byte[sizeof(Int64)];
					Array.Copy(this.bRawValue, rawValue, this.bRawValue.Length);
					return BitConverter.ToInt64(rawValue, 0);
				}
			}
		}

		/// <summary>The following structure defines the structure of a Warranty Threshold (Obsoleted in ATA4!)</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct ATTRTHRESHOLD
		{
			/// <summary>Identifies which attribute</summary>
			public Byte bAttrID;
			/// <summary>Triggering value</summary>
			public Byte bWarrantyThreshold;
			/// <summary>Reserved for future use</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
			public Byte[] bReserved;
			/// <summary>Attribute name</summary>
			public String AttributeName
			{
				get
				{
					String value;
					return Constant.AttributeNames.TryGetValue(this.bAttrID, out value) ? value : Constant.AttributeNames[0];
				}
			}
		}
		/// <summary>The GETVERSIONINPARAMS structure is used in conjunction with the SMART_GET_VERSION request to retrieve version information, a capabilities mask, and a bitmask for the indicated device.</summary>
		/// <remarks>http://msdn.microsoft.com/en-us/library/windows/hardware/ff554977%28v=vs.85%29.aspx</remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct GETVERSIONINPARAMS
		{
			/// <summary>Binary driver version.</summary>
			public Byte bVersion;
			/// <summary>Binary driver revision.</summary>
			public Byte bRevision;
			/// <summary>Not used.</summary>
			public Byte bReserved;
			/// <summary>Bit map of IDE devices.</summary>
			public Byte bIDEDeviceMap;
			/// <summary>Bit mask of driver capabilities.</summary>
			public CAP fCapabilities;
			/// <summary>For future use.</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
			public UInt32[] dwReserved;
			/// <summary>The device is an IDE drive</summary>
			public Boolean IsIDEDrive
			{
				get
				{
					BitArray arr = new BitArray(new Byte[] { this.bIDEDeviceMap, });
					return arr[0] || arr[1] || arr[2] || arr[3];
				}
			}
			/// <summary>The device is an ATAPI drive</summary>
			public Boolean IsAtapiDrive
			{
				get
				{
					BitArray arr = new BitArray(new Byte[] { this.bIDEDeviceMap, });
					return arr[4] || arr[5] || arr[6] || arr[7];
				}
			}
			/// <summary>The device is on primary channel. If False than on secondary channel.</summary>
			public Boolean IsPrimaryChannel
			{
				get
				{
					BitArray arr = new BitArray(new Byte[] { this.bIDEDeviceMap, });
					return arr[0] || arr[1] || arr[4] || arr[5];
				}
			}
			/// <summary>The device is master on channel. If False than subordinate.</summary>
			public Boolean IsMaster
			{
				get
				{
					BitArray arr = new BitArray(new Byte[] { this.bIDEDeviceMap, });
					return arr[0] || arr[2] || arr[4] || arr[6];
				}
			}
			/// <summary>ATA commands supported</summary>
			public Boolean IsAtaSupported { get { return (this.fCapabilities & CAP.ATA_ID_CMD) == CAP.ATA_ID_CMD; } }
			/// <summary>ATAPI commands supported</summary>
			public Boolean IsAtapiSupported { get { return (this.fCapabilities & CAP.ATAPI_ID_CMD) == CAP.ATAPI_ID_CMD; } }
			/// <summary>SMART commands supported</summary>
			public Boolean IsSmartSupported { get { return (this.fCapabilities & CAP.SMART_CMD) == CAP.SMART_CMD; } }
		}
		/// <summary>Bits returned in the fCapabilities member of GETVERSIONINPARAMS</summary>
		[Flags]
		public enum CAP : uint
		{
			/// <summary>ATA ID command supported</summary>
			ATA_ID_CMD = 1,
			/// <summary>ATAPI ID command supported</summary>
			ATAPI_ID_CMD = 2,
			/// <summary>SMART commannds supported</summary>
			SMART_CMD = 4,
		}
		/// <summary>DISK_GEOMETRY is used in conjunction with the <see cref="T:Constant.IOCTL_DISK.GET_DRIVE_GEOMETRY"/> and the <see cref="T:Constant.IOCTL_DISK.GET_MEDIA_TYPES"/> requests, in order to retrieve information about the geometry of a physical disk.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct DISK_GEOMETRY
		{
			/// <summary>Indicates the number of cylinders on the disk device.</summary>
			public UInt64 Cylinders;
			/// <summary>Indicates the type of disk.</summary>
			public WinApi.MEDIA_TYPE MediaType;
			/// <summary>Indicates the number of tracks in a cylinder.</summary>
			public UInt32 TracksPerCylinder;
			/// <summary>Indicates the number of sectors in each track.</summary>
			public UInt32 SectorsPerTrack;
			/// <summary>Indicates the number of bytes in a disk sector.</summary>
			public UInt32 BytesPerSector;
		}
		/// <summary>The DISK_GEOMETRY_EX structure is a variable-length structure composed of a DISK_GEOMETRY structure followed by a DISK_PARTITION_INFO structure followed, in turn, by a DISK_DETECTION_INFO structure.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct DISK_GEOMETRY_EX
		{
			/// <summary>information about the geometry of a physical disk.</summary>
			public DISK_GEOMETRY Geometry;

			/// <summary>Contains the size in bytes of the disk.</summary>
			public UInt64 DiskSize;

			/// <summary>Pointer to a variable length area containing a DISK_PARTITION_INFO structure followed by a DISK_DETECTION_INFO structure.</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = Constant.BUFFER_SIZE)]
			public Byte[] Data;

			/// <summary>Contains string representation of disc size</summary>
			/// <returns>Size with dimention</returns>
			public String DiskSizeString
			{
				get { return Utils.FileSizeToString(this.DiskSize); }
			}

			/// <summary>Information about the disk's partition table.</summary>
			/// <returns>Partition table info</returns>
			public DISK_PARTITION_INFO GetDiscPartitionInfo()
			{
				using (PinnedBufferReader reader = new PinnedBufferReader(this.Data))
					return reader.BytesToStructure<DISK_PARTITION_INFO>(0);
			}

			/// <summary>Detected drive parameters that are supplied by an x86 PC BIOS on boot.</summary>
			/// <returns>Detected drive parameters</returns>
			public DISK_DETECTION_INFO GetDiscDetectionInfo()
			{
				using (PinnedBufferReader reader = new PinnedBufferReader(this.Data))
					return reader.BytesToStructure<DISK_DETECTION_INFO>(this.GetDiscPartitionInfo().SizeOfPartitionInfo);
			}
		}
		/// <summary>The DISK_DETECTION_INFO structure contains the detected drive parameters that are supplied by an x86 PC BIOS on boot.</summary>
		[StructLayout(LayoutKind.Explicit)]
		public struct DISK_DETECTION_INFO
		{
			/// <summary>Contains the quantity, in bytes, of retrieved detect information.</summary>
			[FieldOffset(0)]
			public UInt32 SizeOfDetectInfo;
			/// <summary>Indicates one of three possible detection types</summary>
			[FieldOffset(4)]
			public DETECTION_TYPE DetectionType;
			/// <summary>
			/// Contains DISK_INT13_INFO structure with the disk parameters for INT 13 type partitions.
			/// This member is used if DetectionType == DetectInt13.
			/// </summary>
			[FieldOffset(8)]
			public DISK_INT13_INFO Int13;
			/// <summary>
			/// Contains a DISK_EX_INT13_INFO structure with the disk parameters for extended INT 13 type partitions.
			/// This member is used if DetectionType == DetectExInt13.
			/// </summary>
			[FieldOffset(8)]
			public DISK_EX_INT13_INFO ExInt13;
		}
		/// <summary>The DETECTION_TYPE enumeration type is used in conjunction with the IOCTL_DISK_GET_DRIVE_GEOMETRY_EX request and the DISK_GEOMETRY_EX structure to determine the type of formatting used by the BIOS to record the disk geometry.</summary>
		/// <remarks>Possible formatting types are the standard INT 13h partition format or the extended INT 13h partition format.</remarks>
		public enum DETECTION_TYPE
		{
			/// <summary>Indicates that the disk contains neither an INT 13h partition nor an extended INT 13h partition.</summary>
			DetectNone = 0,
			/// <summary>Indicates that the disk has a standard INT 13h partition.</summary>
			DetectInt13 = 1,
			/// <summary>Indicates that the disk contains an extended INT 13h partition. </summary>
			DetectExInt13 = 2,
		}
		/// <summary>The DISK_INT13_INFO structure is used by the BIOS to report disk detection data for a partition with an INT13 format.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct DISK_INT13_INFO
		{
			/// <summary>
			/// Corresponds to the Device/Head register defined in the AT Attachment (ATA) specification.
			/// When zero, the fourth bit of this register indicates that drive zero is selected.
			/// When 1, it indicates that drive one is selected.
			/// The values of bits 0, 1, 2, 3, and 6 depend on the command in the command register.
			/// Bits 5 and 7 are no longer used.
			/// For more information about the values that the Device/Head register can hold, see the ATA specification.
			/// </summary>
			public UInt16 DriveSelect;
			/// <summary>Indicates the maximum number of cylinders on the disk.</summary>
			public UInt32 MaxCylinders;
			/// <summary>Indicates the number of sectors per track.</summary>
			public UInt16 SectorsPerTrack;
			/// <summary>Indicates the maximum number of disk heads.</summary>
			public UInt16 MaxHeads;
			/// <summary>Indicates the number of drives.</summary>
			public UInt16 NumberDrives;
		}
		/// <summary>The DISK_EX_INT13_INFO structure is used by the BIOS to report disk detection data for a partition with an extended INT13 format.</summary>
		/// <remarks>http://msdn.microsoft.com/en-us/library/windows/hardware/ff552610%28v=vs.85%29.aspx</remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct DISK_EX_INT13_INFO
		{
			/// <summary>
			/// Indicates the size of the buffer that the caller provides to the BIOS in which to return the requested drive data. ExBufferSize must be 26 or greater.
			/// If ExBufferSize is less than 26, the BIOS returns an error.
			/// If ExBufferSize is between 30 and 66, the BIOS sets it to exactly 30 on exit.
			/// If ExBufferSize is 66 or greater, the BIOS sets it to exactly 66 on exit.
			/// </summary>
			public UInt16 ExBufferSize;
			/// <summary>
			/// Provides information about the drive.
			/// The following table describes the significance of each bit, where bit 0 is the least significant bit and bit 15 the most significant bit.
			/// A value of one in the indicated bit means that the feature described in the "Meaning" column is available.
			/// A value of zero in the indicated bit means that the feature is not available with this drive.
			/// </summary>
			public UInt16 ExFlags;
			/// <summary>Indicates the number of physical cylinders. This is one greater than the maximum cylinder number.</summary>
			public UInt32 ExCylinders;
			/// <summary>Indicates the number of physical heads. This is one greater than the maximum head number.</summary>
			public UInt32 ExHeads;
			/// <summary>Indicates the number of physical sectors per track. This number is the same as the maximum sector number.</summary>
			public UInt32 ExSectorsPerTrack;
			/// <summary>Indicates the total count of sectors on the disk. This is one greater than the maximum logical block address.</summary>
			public UInt64 ExSectorsPerDrive;
			/// <summary>Indicates the sector size in bytes.</summary>
			public UInt16 ExSectorSize;
			/// <summary>Reserved</summary>
			public UInt16 ExReserved;
		}
		/// <summary>The DISK_PARTITION_INFO structure is used to report information about the disk's partition table.</summary>
		[StructLayout(LayoutKind.Explicit, Size = 32)]//MBR- 24, GPT - 32
		public struct DISK_PARTITION_INFO
		{
			/// <summary>The PARTITION_STYLE enumeration type indicates the type of partition table for a disk.</summary>
			public enum PARTITION_STYLE
			{
				/// <summary>Specifies the traditional, AT-style Master Boot Record, type of partition table.</summary>
				MBR = 0,
				/// <summary>Specifies the GUID Partition Table type of partition table.</summary>
				GPT = 1,
				/// <summary></summary>
				RAW = 2,
			}
			/// <summary>Size of this structure in bytes. Set to sizeof(DISK_PARTITION_INFO).</summary>
			[FieldOffset(0)]
			public UInt32 SizeOfPartitionInfo;
			/// <summary>Takes a PARTITION_STYLE enumerated value that specifies the type of partition table the disk contains.</summary>
			[FieldOffset(4)]
			public PARTITION_STYLE PartitionStyle;
			/// <summary>MBR</summary>
			[FieldOffset(8)]
			public MbrType Mbr;
			/// <summary>GPT</summary>
			[FieldOffset(8)]
			public GptType Gpt;
		}
		/// <summary>Mbr</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct MbrType
		{
			/// <summary>
			/// Specifies the signature value, which uniquely identifies the disk.
			/// The Mbr member of the union is used to specify the disk signature data for a disk formatted with a Master Boot Record (MBR) format partition table.
			/// Any other value indicates that the partition is not a boot partition.
			/// </summary>
			/// <remarks>This member is valid when PartitionStyle is PARTITION_STYLE.MBR.</remarks>
			public UInt32 Signature;
			/// <summary>
			/// Specifies the checksum for the master boot record.
			/// The Mbr member of the union is used to specify the disk signature data for a disk formatted with a Master Boot Record (MBR) format partition table.
			/// </summary>
			/// <remarks>This member is valid when PartitionStyle is PARTITION_STYLE.MBR.</remarks>
			public UInt32 CheckSum;
		}
		/// <summary>Gpt</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct GptType
		{
			/// <summary>
			/// Specifies the GUID that uniquely identifies the disk.
			/// The Gpt member of the union is used to specify the disk signature data for a disk that is formatted with a GUID Partition Table (GPT) format partition table.
			/// </summary>
			/// <remarks>This member is valid when PartitionStyle is PARTITION_STYLE_GPT.</remarks>
			public Guid DiskId;
		}
		/// <summary>Contains information used to verify a disk extent.</summary>
		/// <remarks>It is the output buffer for the IOCTL_DISK_VERIFY control code.</remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct VERIFY_INFORMATION
		{
			/// <summary>The starting offset of the disk extent.</summary>
			public UInt64 StartingOffset;
			/// <summary>The length of the disk extent, in bytes.</summary>
			public UInt32 Length;
		}
		/// <summary>The DISK_PERFORMANCE structure is used in conjunction with the IOCTL_DISK_PERFORMANCE request to collect summary disk statistics for purposes of measuring disk performance.</summary>
		/// <remarks>Counting halts whenever the performance counters are disabled, but the counters are not reset, so the cumulative values assigned to the structure members might potentially reflect disk activity across several enablings and disablings of the counters.</remarks>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DISK_PERFORMANCE
		{
			/// <summary>Contains a cumulative count of bytes read from the disk since the performance counters were enabled.</summary>
			public UInt64 BytesRead;
			/// <summary>Contains a cumulative count of bytes written to the disk since the performance counters were enabled.</summary>
			public UInt64 BytesWritten;
			/// <summary>Contains a cumulative time, expressed in increments of 100 nanoseconds, spent on disk reads since the performance counters were enabled.</summary>
			public UInt64 ReadTime;
			/// <summary>Contains a cumulative time, expressed in increments of 100 nanoseconds, spent on disk reads since the performance counters were enabled.</summary>
			public UInt64 WriteTime;
			/// <summary>Contains a cumulative time, expressed in increments of 100 nanoseconds, since the performance counters were enabled in which there was no disk activity.</summary>
			public UInt64 IdleTime;
			/// <summary>Contains the number of disk accesses for reads since the performance counters were enabled.</summary>
			public UInt32 ReadCount;
			/// <summary>Contains the number of disk accesses for writes since the performance counters were enabled.</summary>
			public UInt32 WriteCount;
			/// <summary>Contains a snapshot of the number of queued disk I/O requests at the time that the query for performance statistics was performed.</summary>
			public Int32 QueueDepth;
			/// <summary>Contains the number of disk accesses by means of an associated IRP since the performance counters were enabled.</summary>
			public UInt32 SplitCount;
			/// <summary>
			/// Contains a timestamp indicating the system time at the moment that the query took place.
			/// System time is a count of 100-nanosecond intervals since January 1, 1601.
			/// System time is typically updated approximately every ten milliseconds.
			/// </summary>
			public UInt64 QueryTime;
			/// <summary>Contains a unique number assigned to every disk or volume across a particular storage type. The storage types are disk.sys, ftdisk.sys, and dmio.sys.</summary>
			public UInt32 StorageDeviceNumber;
			/// <summary>
			/// Contains an 8-character string that indicates which device driver provided the performance statistics.
			/// In Windows 2000, this can be either "LogiDisk" for the driver logidisk.sys or "PhysDisk" for the driver physdisk.sys.
			/// These drivers collect performance statistics for devices and physical disks respectively.
			/// In Windows XP and later operating systems, this can be any of the following three strings: "FTDISK" for the driver ftdisk.sys, "DMIO" for the driver dmio.sys, or PARTMGR" for the driver partmgr.sys.
			/// These three drivers collect performance statistics for basic disk volumes, dynamic disk volumes, and physical disks respectively.
			/// Note that these strings are 8-character case-sensitive strings with blank fill.
			/// For example, in the case of the string "FTDISK", the StorageManagerName character array should contain two trailing blanks ("FTDISK&lt;b&gt;&lt;b&gt;"), and in the case of the string "DMIO", the array should contain four trailing blanks ("DMIO&lt;b&gt;&lt;b&gt;&lt;b&gt;&lt;b&gt;").
			/// </summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
			public String StorageManagerName;
		}
	}
}
