using System;
using System.Runtime.InteropServices;

namespace DeviceIOControl.Native
{
	/// <summary>ATA IOCTL structures</summary>
	public struct AtaApi
	{
		/// <summary>The ATA_PASS_THROUGH_EX structure is used in conjunction with an <see cref="T:Constant.IOCTL_ATA.PASS_THROUGH"/> request to instruct the port driver to send an embedded ATA command to the target device.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct ATA_PASS_THROUGH_EX
		{
			/// <summary>Indicates the direction of data transfer and specifies the kind of operation to be performed.</summary>
			[Flags]
			public enum AtaPassThroughFlags : ushort
			{
				/// <summary>Wait for DRDY status from the device before sending the command to the device.</summary>
				DRDY_REQUIRED = 1 << 0,
				/// <summary>Read data from the device.</summary>
				DATA_IN = 1 << 1,
				/// <summary>Write data to the device.</summary>
				DATA_OUT = 1 << 2,
				/// <summary>The ATA command to be sent uses the 48-bit logical block address (LBA) feature set.</summary>
				/// <remarks>When this flag is set, the contents of the PreviousTaskFile member in the ATA_PASS_THROUGH_EX structure should be valid.</remarks>
				_48BIT_COMMAND = 1 << 3,
				/// <summary>Set the transfer mode to DMA.</summary>
				USE_DMA = 1 << 4,
				/// <summary>Read single sector only.</summary>
				NO_MULTIPLE = 1 << 5,
			}
			/// <summary>Specifies the length in bytes of the ATA_PASS_THROUGH_EX structure.</summary>
			public UInt16 Length;
			/// <summary>Indicates the direction of data transfer and specifies the kind of operation to be performed.</summary>
			public AtaPassThroughFlags AtaFlags;
			/// <summary>Contains an integer that indicates the IDE port or bus for the request. This value is set by the port driver.</summary>
			public Byte PathId;
			/// <summary>Contains an integer that indicates the target device on the bus. This value is set by the port driver.</summary>
			public Byte TargetId;
			/// <summary>Indicates the logical unit number of the device. This value is set by the port driver.</summary>
			public Byte Lun;
			/// <summary>Reserved for future use.</summary>
			public Byte ReservedAsUchar;
			/// <summary>
			/// Indicates the size, in bytes, of the data buffer.
			/// If an underrun occurs, the miniport driver must update this member to the number of bytes that were actually transferred.
			/// </summary>
			public UInt32 DataTransferLength;
			/// <summary>Indicates the number of seconds that are allowed for the request to execute before the OS-specific port driver determines that the request has timed out.</summary>
			public UInt32 TimeOutValue;
			/// <summary>Reserved for future use.</summary>
			public UInt32 ReservedAsUlong;
			/// <summary>Specifies the offset, in bytes, from the beginning of this structure to the data buffer.</summary>
			public UInt32 DataBufferOffset;
			/// <summary>Specifies the contents of the task file input registers prior to the current pass-through command.</summary>
			/// <remarks>This member is not used when the ATA_FLAGS_48BIT_COMMAND flag is not set.</remarks>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			public Byte[] PreviousTaskFile;
			/// <summary>Specifies the content of the task file register on both input and output.</summary>
			/// <remarks>http://msdn.microsoft.com/en-us/library/windows/hardware/ff551323%28v=vs.85%29.aspx</remarks>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
			public Byte[] CurrentTaskFile;
		}
	}
}
