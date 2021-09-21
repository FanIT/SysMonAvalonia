using System;
using System.Runtime.InteropServices;

namespace DeviceIOControl.Native
{
	/// <summary>Storage IOCTL structures</summary>
	public struct StorageApi
	{
		/// <summary>Indicates the properties of a storage device or adapter to retrieve as the input buffer passed to the <see cref="T:Constant.IOCTL_STORAGE.QUERY_PROPERTY"/> control code.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct STORAGE_PROPERTY_QUERY
		{
			/// <summary>Enumerates the possible values of the PropertyId member of the <see cref="T:STORAGE_PROPERTY_QUERY"/> structure passed as input to the <see cref="T:Constant.IOCTL_STORAGE.QUERY_PROPERTY"/> request to retrieve the properties of a storage device or adapter.</summary>
			public enum STORAGE_PROPERTY_ID : int
			{
				/// <summary>Indicates that the caller is querying for the device descriptor.</summary>
				StorageDeviceProperty = 0,
				/// <summary>Indicates that the caller is querying for the adapter descriptor.</summary>
				StorageAdapterProperty = 1,
				/// <summary>Indicates that the caller is querying for the device identifiers provided with the SCSI vital product data pages.</summary>
				StorageDeviceIdProperty = 2,
				/// <summary>Indicates that the caller is querying for the unique device identifiers.</summary>
				StorageDeviceUniqueIdProperty = 3,
				/// <summary>Indicates that the caller is querying for the write cache property.</summary>
				StorageDeviceWriteCacheProperty = 4,
				/// <summary>Indicates that the caller is querying for the miniport driver descriptor.</summary>
				StorageMiniportProperty = 5,
				/// <summary>Indicates that the caller is querying for the access alignment descriptor.</summary>
				StorageAccessAlignmentProperty = 6,
				/// <summary>Indicates that the caller is querying for the seek penalty descriptor.</summary>
				StorageDeviceSeekPenaltyProperty = 7,
				/// <summary>Indicates that the caller is querying for the trim descriptor.</summary>
				StorageDeviceTrimProperty = 8,
				/// <summary>Indicates that the caller is querying for the write aggregation property.</summary>
				StorageDeviceWriteAggregationProperty = 9,
				/// <summary>This value is reserved.</summary>
				StorageDeviceDeviceTelemetryProperty = 0xA,
				/// <summary>Indicates that the caller is querying for the logical block provisioning descriptor, usually to detect whether the storage system uses thin provisioning.</summary>
				StorageDeviceLBProvisioningProperty = 0xB,
				/// <summary>Indicates that the caller is querying for the power optical disk drive descriptor.</summary>
				StorageDeviceZeroPowerProperty = 0xC,
				/// <summary>Indicates that the caller is querying for the write offload descriptor.</summary>
				StorageDeviceCopyOffloadProperty = 0xD,
				/// <summary>Indicates that the caller is querying for the device resiliency descriptor.</summary>
				StorageDeviceResiliencyProperty = 0xE,
			}
			/// <summary>Types of queries</summary>
			public enum STORAGE_QUERY_TYPE : int
			{
				/// <summary>Retrieves the descriptor</summary>
				PropertyStandardQuery = 0,
				/// <summary>Used to test whether the descriptor is supported</summary>
				PropertyExistsQuery = 1,
				/// <summary>Used to retrieve a mask of writeable fields in the descriptor</summary>
				PropertyMaskQuery = 2,
				/// <summary>use to validate the value</summary>
				PropertyQueryMaxDefined = 3,
			}

			/// <summary>Indicates whether the caller is requesting a device descriptor, an adapter descriptor, a write cache property, a device unique ID (DUID), or the device identifiers provided in the device's SCSI vital product data (VPD) page.</summary>
			public STORAGE_PROPERTY_ID PropertyId;
			/// <summary>Contains flags indicating the type of query to be performed as enumerated by the <see cref="T:STORAGE_QUERY_TYPE"/> enumeration.</summary>
			public STORAGE_QUERY_TYPE QueryType;
			/// <summary>Contains an array of bytes that can be used to retrieve additional parameters for specific queries.</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
			public Byte[] AdditionalParameters;
		}

		/// <summary>Contains information about the size of a device.</summary>
		/// <remarks>The header file Ntddstor.h is available in the Windows Driver Kit (WDK).</remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct STORAGE_READ_CAPACITY
		{
			/// <summary>The version of this structure.</summary>
			/// <remarks>The caller must set this member to sizeof(STORAGE_READ_CAPACITY).</remarks>
			public UInt32 Version;
			/// <summary>The size of the data returned.</summary>
			public UInt32 Size;
			/// <summary>The number of bytes per block.</summary>
			public UInt32 BlockLength;
			/// <summary>The total number of blocks on the disk.</summary>
			public UInt64 NumberOfBlocks;
			/// <summary>The disk size in bytes.</summary>
			public UInt64 DiskLength;

			/// <summary>Contains string representation of disc size</summary>
			/// <returns>Size with dimention</returns>
			public String DiskLengthString
			{
				get { return Utils.FileSizeToString(this.DiskLength); }
			}
		}

		/// <summary>The STORAGE_HOTPLUG_INFO structure provides hotplug information for a device.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct STORAGE_HOTPLUG_INFO
		{
			/// <summary>Indicates the size, in bytes, of this structure.</summary>
			public UInt32 Size;
			/// <summary>Specifies whether the media is removable.</summary>
			/// <remarks>
			/// If set to a nonzero value, the device media is removable.
			/// If set to zero, the device media is not removable.
			/// </remarks>
			public Byte MediaRemovable;
			/// <summary>Specifies whether the media is lockable.</summary>
			/// <remarks>
			/// If set to a nonzero value, the device media is not lockable.
			/// If set to zero, the device media is lockable.
			/// </remarks>
			public Byte MediaHotplug;
			/// <summary>Specifies whether the device is a hotplug device.</summary>
			/// <remarks>
			/// If set to a nonzero value, the device is a hotplug device.
			/// If set to zero, the device is not a hotplug device.
			/// </remarks>
			public Byte DeviceHotplug;
			/// <summary>Do not use; set the value to NULL.</summary>
			public Byte WriteCacheEnableOverride;
		}
		/// <summary>Device property descriptor - this is really just a rehash of the inquiry data retrieved from a scsi device.</summary>
		/// <remarks>This may only be retrieved from a target device. Sending this to the bus will result in an error.</remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct STORAGE_DEVICE_DESCRIPTOR
		{
			/// <summary>Contains the size of this structure, in bytes. The value of this member will change as members are added to the structure.</summary>
			public UInt32 Version;
			/// <summary>Specifies the total size of the descriptor, in bytes, which may include vendor ID, product ID, product revision, device serial number strings and bus-specific data which are appended to the structure.</summary>
			public UInt32 Size;
			/// <summary>Specifies the device type as defined by the Small Computer Systems Interface (SCSI) specification.</summary>
			public Byte DeviceType;
			/// <summary>Specifies the device type modifier, if any, as defined by the SCSI specification.</summary>
			/// <remarks>If no device type modifier exists, this member is zero.</remarks>
			public Byte DeviceTypeModifier;
			/// <summary>Indicates when TRUE that the device's media (if any) is removable. If the device has no media, this member should be ignored. When FALSE the device's media is not removable.</summary>
			/// <remarks>This field should be ignored for media-less devices.</remarks>
			public Byte RemovableMedia;
			/// <summary>Indicates when TRUE that the device supports multiple outstanding commands (SCSI tagged queuing or equivalent). When FALSE, the device does not support SCSI-tagged queuing or the equivalent.</summary>
			/// <remarks>The actual synchronization in this case is the responsibility of the port driver.</remarks>
			public Byte CommandQueueing;
			/// <summary>Specifies the byte offset from the beginning of the structure to a null-terminated ASCII string that contains the device's vendor ID.</summary>
			/// <remarks>If the device has no vendor ID, this member is zero.</remarks>
			public UInt32 VendorIdOffset;
			/// <summary>Specifies the byte offset from the beginning of the structure to a null-terminated ASCII string that contains the device's product ID.</summary>
			/// <remarks>If the device has no product ID, this member is zero.</remarks>
			public UInt32 ProductIdOffset;
			/// <summary>Specifies the byte offset from the beginning of the structure to a null-terminated ASCII string that contains the device's product revision string.</summary>
			/// <remarks>If the device has no product revision string, this member is zero.</remarks>
			public UInt32 ProductRevisionOffset;
			/// <summary>Specifies the byte offset from the beginning of the structure to a null-terminated ASCII string that contains the device's serial number.</summary>
			/// <remarks>If the device has no serial number, this member is zero.</remarks>
			public UInt32 SerialNumberOffset;
			/// <summary>Specifies an enumerator value of type <see cref="T:STORAGE_BUS_TYPE"/> that indicates the type of bus to which the device is connected.</summary>
			/// <remarks>It should be used to interpret the raw device properties at the end of this structure (if any)</remarks>
			public STORAGE_BUS_TYPE BusType;
			/// <summary>Indicates the number of bytes of bus-specific data that have been appended to this descriptor.</summary>
			public UInt32 RawPropertiesLength;
			/// <summary>Contains an array of length one that serves as a place holder for the first byte of the bus specific property data.</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = Constant.BUFFER_SIZE)]
			public Byte[] RawDeviceProperties;
		}
		/// <summary>The STORAGE_PREDICT_FAILURE structure is used in conjunction with <see cref="Constant.IOCTL_STORAGE.PREDICT_FAILURE"/> to report whether a device is currently predicting a failure.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct STORAGE_PREDICT_FAILURE
		{
			/// <summary>Indicates when nonzero that the device is currently predicting an imminent failure.</summary>
			public UInt32 PredictFailure;
			/// <summary>Contains an array that holds 512 bytes of vendor-specific information if the device supports failure prediction.</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = Constant.BUFFER_SIZE)]
			public Byte[] VendorSpecific;
			/// <summary>Device does not predict any failures at this time</summary>
			public Boolean IsDeviceHealthy { get { return this.PredictFailure == 0; } }
		}
		/// <summary>Provides a symbolic means of representing storage bus types.</summary>
		/// <remarks>Bus types below 128 (0x80) are reserved for Microsoft use</remarks>
		public enum STORAGE_BUS_TYPE : int
		{
			/// <summary>Indicates an unknown bus type.</summary>
			BusTypeUnknown = 0x00,
			/// <summary>Indicates a SCSI bus type.</summary>
			BusTypeScsi = 0x1,
			/// <summary>Indicates an ATAPI bus type.</summary>
			BusTypeAtapi = 0x2,
			/// <summary>Indicates an ATA bus type.</summary>
			BusTypeAta = 0x3,
			/// <summary>Indicates an IEEE 1394 bus type.</summary>
			BusType1394 = 0x4,
			/// <summary>Indicates an SSA bus type.</summary>
			BusTypeSsa = 0x5,
			/// <summary>Indicates a fiber channel bus type.</summary>
			BusTypeFibre = 0x6,
			/// <summary>Indicates a USB bus type.</summary>
			BusTypeUsb = 0x7,
			/// <summary>Indicates a RAID bus type.</summary>
			BusTypeRAID = 0x8,
			/// <summary>Indicates an iSCSI bus type.</summary>
			BusTypeiScsi = 0x9,
			/// <summary>Indicates a serial-attached SCSI (SAS) bus type.</summary>
			BusTypeSas = 0xA,
			/// <summary>Indicates a SATA bus type.</summary>
			BusTypeSata = 0xB,
			/// <summary>Indicates a secure digital (SD) bus type.</summary>
			BusTypeSd = 0xC,
			/// <summary>Indicates a multimedia card (MMC) bus type.</summary>
			BusTypeMmc = 0xD,
			/// <summary>Indicates a virtual bus type.</summary>
			BusTypeVirtual = 0xE,
			/// <summary>Indicates a file-backed virtual bus type.</summary>
			BusTypeFileBackedVirtual = 0xF,
			/// <summary>Indicates the maximum value for this value.</summary>
			BusTypeMax = 0x10,
			/// <summary>The maximum value of the storage bus type range.</summary>
			BusTypeMaxReserved = 0x7F,
		}
		/// <summary>
		/// The STORAGE_DEVICE_RESILIENCY_DESCRIPTOR structure is one of the query result structures returned from an <see cref="T:Constant.IOCTL_STORAGE.QUERY_PROPERTY"/> request.
		/// This structure contains the resiliency capabilities for a storage device.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct STORAGE_DEVICE_RESILIENCY_DESCRIPTOR
		{
			/// <summary>Contains the size of this structure, in bytes. The value of this member will change as members are added to the structure. Set to sizeof(STORAGE_DEVICE_RESILIENCY_DESCRIPTOR).</summary>
			public UInt32 Version;
			/// <summary>Specifies the total size of the data returned, in bytes. This may include data that follows this structure.</summary>
			public UInt32 Size;
			/// <summary>
			/// Byte offset to the null-terminated ASCII string containing the resiliency properties Name.
			/// For devices with no Name property, this will be zero.
			/// </summary>
			public UInt32 NameOffset;
			/// <summary>Number of logical copies of data that are available.</summary>
			public UInt32 NumberOfLogicalCopies;
			/// <summary>Number of complete copies of data that are stored.</summary>
			public UInt32 NumberOfPhysicalCopies;
			/// <summary>Number of disks that can fail without leading to data loss.</summary>
			public UInt32 PhysicalDiskRedundancy;
			/// <summary>Number of columns in the storage device.</summary>
			public UInt32 NumberOfColumns;
			/// <summary>
			/// Size of a stripe unit of the storage device, in bytes.
			/// This is also referred to as the stripe width or interleave of the storage device.
			/// </summary>
			public UInt32 Interleave;
		}
		/// <summary>
		/// The DEVICE_COPY_OFFLOAD_DESCRIPTOR structure is one of the query result structures returned from an <see cref="T:Constant.IOCTL_STORAGE.QUERY_PROPERTY"/> request.
		/// This structure contains the copy offload capabilities for a storage device.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct DEVICE_COPY_OFFLOAD_DESCRIPTOR
		{
			/// <summary>Contains the size of this structure, in bytes. The value of this member will change as members are added to the structure.</summary>
			public UInt32 Version;
			/// <summary>Specifies the total size of the data returned, in bytes. This may include data that follows this structure.</summary>
			public UInt32 Size;
			/// <summary>The maximum lifetime of the token, in seconds.</summary>
			public UInt32 MaximumTokenLifetime;
			/// <summary>The default lifetime of the token, in seconds.</summary>
			public UInt32 DefaultTokenLifetime;
			/// <summary>The maximum transfer size, in bytes.</summary>
			public UInt64 MaximumTransferSize;
			/// <summary>The optimal transfer size, in bytes.</summary>
			public UInt64 OptimalTransferCount;
			/// <summary>The maximum number of data descriptors.</summary>
			public UInt32 MaximumDataDescriptors;
			/// <summary>The maximum transfer length, in blocks, per descriptor.</summary>
			public UInt32 MaximumTransferLengthPerDescriptor;
			/// <summary>The optimal transfer length per descriptor.</summary>
			public UInt32 OptimalTransferLengthPerDescriptor;
			/// <summary>
			/// The granularity of the optimal transfer length, in blocks.
			/// Transfer lengths that are not an even multiple of this length may be delayed.
			/// </summary>
			public UInt16 OptimalTransferLengthGranularity;
			/// <summary>Reserved.</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public Byte[] Reserved;
		}
		/// <summary>Used in conjunction with the <see cref="T:Constant.IOCTL_STORAGE.QUERY_PROPERTY"/> control code to retrieve the write aggregation data for a device.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct DEVICE_WRITE_AGGREGATION_DESCRIPTOR
		{
			/// <summary>Contains the size, in bytes, of this structure. The value of this member will change as members are added to the structure.</summary>
			public UInt32 Version;
			/// <summary>Specifies the total size of the descriptor, in bytes.</summary>
			public UInt32 Size;
			/// <summary>TRUE if the device benefits from write aggregation.</summary>
			public Byte BenefitsFromWriteAggregation;
		}
		/// <summary>Used in conjunction with the <see cref="T:Constant.IOCTL_STORAGE.QUERY_PROPERTY"/> request to retrieve the trim descriptor data for a device.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct DEVICE_TRIM_DESCRIPTOR
		{
			/// <summary>Contains the size of this structure, in bytes. The value of this member will change as members are added to the structure.</summary>
			public UInt32 Version;
			/// <summary>Specifies the total size of the data returned, in bytes. This may include data that follows this structure.</summary>
			public UInt32 Size;
			/// <summary>Specifies whether trim is enabled for the device.</summary>
			public Byte TrimEnabled;
		}
		/// <summary>Used in conjunction with the <see cref="T:Constant.IOCTL_STORAGE.QUERY_PROPERTY"/> request to retrieve the seek penalty descriptor data for a device.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct DEVICE_SEEK_PENALTY_DESCRIPTOR
		{
			/// <summary>Contains the size of this structure, in bytes. The value of this member will change as members are added to the structure.</summary>
			public UInt32 Version;
			/// <summary>Specifies the total size of the data returned, in bytes. This may include data that follows this structure.</summary>
			public UInt32 Size;
			/// <summary>Specifies whether the device incurs a seek penalty.</summary>
			public Byte IncursSeekPenalty;
		}
		/// <summary>Used in conjunction with the <see cref="T:Constant.IOCTL_STORAGE.QUERY_PROPERTY"/> control code to retrieve the storage access alignment descriptor data for a device.</summary>
		/// <remarks>http://msdn.microsoft.com/en-us/library/windows/desktop/ff800831%28v=vs.85%29.aspx</remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct STORAGE_ACCESS_ALIGNMENT_DESCRIPTOR
		{
			/// <summary>Contains the size of this structure, in bytes. The value of this member will change as members are added to the structure.</summary>
			public UInt32 Version;
			/// <summary>Specifies the total size of the data returned, in bytes. This may include data that follows this structure.</summary>
			public UInt32 Size;
			/// <summary>The number of bytes in a cache line of the device.</summary>
			public UInt32 BytesPerCacheLine;
			/// <summary>The address offset necessary for proper cache access alignment, in bytes.</summary>
			public UInt32 BytesOffsetForCacheAlignment;
			/// <summary>The number of bytes in a logical sector of the device.</summary>
			public UInt32 BytesPerLogicalSector;
			/// <summary>The number of bytes in a physical sector of the device.</summary>
			public UInt32 BytesPerPhysicalSector;
			/// <summary>The logical sector offset within the first physical sector where the first logical sector is placed, in bytes.</summary>
			public UInt32 BytesOffsetForSectorAlignment;
		}
		/// <summary>Used in conjunction with the <see cref="T:Constant.IOCTL_STORAGE.QUERY_PROPERTY"/> request to retrieve the storage adapter miniport driver descriptor data for a device.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct STORAGE_MINIPORT_DESCRIPTOR
		{
			/// <summary>Indicates whether the storage adapter driver is a Storport-miniport driver or a SCSI Port-miniport driver.</summary>
			public enum STORAGE_PORT_CODE_SET
			{
				/// <summary>Indicates an unknown storage adapter driver type.</summary>
				StoragePortCodeSetReserved = 0,
				/// <summary>Storage adapter driver is a Storport-miniport driver.</summary>
				StoragePortCodeSetStorport = 1,
				/// <summary>Storage adapter driver is a SCSI Port-miniport driver.</summary>
				StoragePortCodeSetSCSIport = 2,
			}
			/// <summary>Contains the size of this structure, in bytes. The value of this member will change as members are added to the structure.</summary>
			public UInt32 Version;
			/// <summary>Specifies the total size of the data returned, in bytes. This may include data that follows this structure.</summary>
			public UInt32 Size;
			/// <summary>Type of port driver as enumerated by the STORAGE_PORT_CODE_SET enumeration.</summary>
			public STORAGE_PORT_CODE_SET Portdriver;
			/// <summary>Indicates whether a LUN reset is supported.</summary>
			public Byte LUNResetSupported;
			/// <summary>Indicates whether a target reset is supported.</summary>
			public Byte TargetResetSupported;
		}
		/// <summary>The DEVICE_POWER_DESCRIPTOR structure describes the power capabilities of a storage device.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct DEVICE_POWER_DESCRIPTOR
		{
			/// <summary>Contains the size of this structure, in bytes. The value of this member will change as members are added to the structure.</summary>
			public UInt32 Version;
			/// <summary>Specifies the total size of the data returned, in bytes. This may include data that follows this structure.</summary>
			public UInt32 Size;
			/// <summary>True if device attention is supported. Otherwise, false.</summary>
			public Byte DeviceAttentionSupported;
			/// <summary>True if the device supports asynchronous notifications, delivered via <see cref="T:Constant.IOCTL_STORAGE.EVENT_NOTIFICATION"/>. Otherwise, false.</summary>
			public Byte AsynchronousNotificationSupported;
			/// <summary>True if the device has been registered for runtime idle power management. Otherwise, false.</summary>
			public Byte IdlePowerManagementEnabled;
			/// <summary>True if the device will be powered off when put into D3 power state. Otherwise, false.</summary>
			public Byte D3ColdEnabled;
			/// <summary>True if the platform supports D3ColdEnabled for this device. Otherwise, false.</summary>
			public Byte D3ColdSupported;
			/// <summary>Reserved.</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
			public Byte[] Reserved;
			/// <summary>The idle timeout value in milliseconds. This member is ignored unless IdlePowerManagementEnabled is true.</summary>
			public UInt32 IdleTimeoutInMS;
		}
		/// <summary>Used with the <see cref="T:Constant.IOCTL_STORAGE.QUERY_PROPERTY"/> control code to retrieve information about a device's write cache property.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct STORAGE_WRITE_CACHE_PROPERTY
		{
			/// <summary>Specifies the cache type.</summary>
			public enum WRITE_CACHE_TYPE
			{
				/// <summary>The system cannot report the type of the write cache.</summary>
				WriteCacheTypeUnknown = 0,
				/// <summary>The device does not have a write cache.</summary>
				WriteCacheTypeNone = 1,
				/// <summary>The device has a write-back cache.</summary>
				WriteCacheTypeWriteBack = 2,
				/// <summary>The device has a write-through cache.</summary>
				WriteCacheTypeWriteThrough = 3,
			}
			/// <summary>Indicates whether the write cache is enabled or disabled.</summary>
			public enum WRITE_CACHE_ENABLE
			{
				/// <summary>The system cannot report whether the device's write cache is enabled or disabled.</summary>
				WriteCacheEnableUnknown = 0,
				/// <summary>The device's write cache is disabled.</summary>
				WriteCacheDisabled = 1,
				/// <summary>The device's write cache is enabled.</summary>
				WriteCacheEnabled = 2,
			}
			/// <summary>Indicates whether the write cache features of a device are changeable.</summary>
			public enum WRITE_CACHE_CHANGE
			{
				/// <summary>The system cannot report the write cache change capability of the device.</summary>
				WriteCacheChangeUnknown = 0,
				/// <summary>Host software cannot change the characteristics of the device's write cache.</summary>
				WriteCacheNotChangeable = 1,
				/// <summary>Host software can change the characteristics of the device's write cache.</summary>
				WriteCacheChangeable = 2,
			}
			/// <summary>Specifies whether a storage device supports write-through caching.</summary>
			public enum WRITE_THROUGH
			{
				/// <summary>Indicates that no information is available about the write-through capabilities of the device.</summary>
				WriteThroughUnknown = 0,
				/// <summary>Indicates that the device does not support write-through caching.</summary>
				WriteThroughNotSupported = 1,
				/// <summary>Indicates that the device supports write-through caching.</summary>
				WriteThroughSupported = 2,
			}
			/// <summary>
			/// Contains the size of this structure, in bytes.
			/// The value of this member will change as members are added to the structure.
			/// </summary>
			public UInt32 Version;
			/// <summary>
			/// Specifies the total size of the data returned, in bytes.
			/// This may include data that follows this structure.
			/// </summary>
			public UInt32 Size;
			/// <summary>A value from the WRITE_CACHE_TYPE enumeration that indicates the current write cache type.</summary>
			public WRITE_CACHE_TYPE WriteCacheType;
			/// <summary>A value from the WRITE_CACHE_ENABLE enumeration that indicates whether the write cache is enabled.</summary>
			public WRITE_CACHE_ENABLE WriteCacheEnabled;
			/// <summary>A value from the WRITE_CACHE_CHANGE enumeration that indicates whether if the host can change the write cache characteristics.</summary>
			public WRITE_CACHE_CHANGE WriteCacheChangeable;
			/// <summary>A value from the WRITE_THROUGH enumeration that indicates whether the device supports write-through caching.</summary>
			public WRITE_THROUGH WriteThroughSupported;
			/// <summary>
			/// A BOOLEAN value that indicates whether the device allows host software to flush the device cache.
			/// If TRUE, the device allows host software to flush the device cache.
			/// If FALSE, host software cannot flush the device cache.
			/// </summary>
			public Byte FlushCacheSupported;
			/// <summary>
			/// A BOOLEAN value that indicates whether a user can configure the device's power protection characteristics in the registry.
			/// If TRUE, a user can configure the device's power protection characteristics in the registry.
			/// If FALSE, the user cannot configure the device's power protection characteristics in the registry.
			/// </summary>
			public Byte UserDefinedPowerProtection;
			/// <summary>
			/// A BOOLEAN value that indicates whether the device has a battery backup for the write cache.
			/// If TRUE, the device has a battery backup for the write cache.
			/// If FALSE, the device does not have a battery backup for the writer cache.
			/// </summary>
			public Byte NVCacheEnabled;
		}
		/// <summary>Used with the <see cref="T:Constant.IOCTL_STORAGE.QUERY_PROPERTY"/> control code request to retrieve the device ID descriptor data for a device.</summary>
		/// <remarks>The device ID descriptor consists of an array of device IDs taken from the SCSI-3 vital product data (VPD) page 0x83 that was retrieved during discovery.</remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct STORAGE_DEVICE_ID_DESCRIPTOR
		{
			/// <summary>Contains the size of this structure, in bytes. The value of this member will change as members are added to the structure.</summary>
			public UInt32 Version;
			/// <summary>Specifies the total size of the data returned, in bytes. This may include data that follows this structure.</summary>
			public UInt32 Size;
			/// <summary>Contains the number of identifiers reported by the device in the Identifiers array.</summary>
			public UInt32 NumberOfIdentifiers;
			/// <summary>Contains a variable-length array of identification descriptors.</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = Constant.BUFFER_SIZE)]
			public Byte[] Identifiers;
		}
		/// <summary>Used with the <see cref="T:Constant.IOCTL_STORAGE.QUERY_PROPERTY"/> control code to retrieve the storage adapter descriptor data for a device.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct STORAGE_ADAPTER_DESCRIPTOR
		{
			/// <summary>Contains the size of this structure, in bytes. The value of this member will change as members are added to the structure.</summary>
			public UInt32 Version;
			/// <summary>Specifies the total size of the data returned, in bytes. This may include data that follows this structure.</summary>
			public UInt32 Size;
			/// <summary>Specifies the maximum number of bytes the storage adapter can transfer in a single operation.</summary>
			public UInt32 MaximumTransferLength;
			/// <summary>Specifies the maximum number of discontinuous physical pages the storage adapter can manage in a single transfer (in other words, the extent of its scatter/gather support).</summary>
			public UInt32 MaximumPhysicalPages;
			/// <summary>Specifies the storage adapter's alignment requirements for transfers.</summary>
			/// <remarks>The alignment mask indicates alignment restrictions for buffers required by the storage adapter for transfer operations. Valid mask values are also restricted by characteristics of the memory managers on different versions of Windows.</remarks>
			/// <value>
			/// 0 - Buffers must be aligned on BYTE boundaries.
			/// 1 - Buffers must be aligned on WORD boundaries.
			/// 3 - Buffers must be aligned on DWORD32 boundaries.
			/// 7 - Buffers must be aligned on DWORD64 boundaries.
			/// </value>
			public UInt32 AlignmentMask;
			/// <summary>If this member is TRUE, the storage adapter uses programmed I/O (PIO) and requires the use of system-space virtual addresses mapped to physical memory for data buffers. When this member is FALSE, the storage adapter does not use PIO.</summary>
			public Byte AdapterUsesPio;
			/// <summary>If this member is TRUE, the storage adapter scans down for BIOS devices, that is, the storage adapter begins scanning with the highest device number rather than the lowest. When this member is FALSE, the storage adapter begins scanning with the lowest device number.</summary>
			/// <remarks>This member is reserved for legacy miniport drivers.</remarks>
			public Byte AdapterScansDown;
			/// <summary>If this member is TRUE, the storage adapter supports SCSI tagged queuing and/or per-logical-unit internal queues, or the non-SCSI equivalent. When this member is FALSE, the storage adapter neither supports SCSI-tagged queuing nor per-logical-unit internal queues.</summary>
			public Byte CommandQueueing;
			/// <summary>If this member is TRUE, the storage adapter supports synchronous transfers as a way of speeding up I/O. When this member is FALSE, the storage adapter does not support synchronous transfers as a way of speeding up I/O. </summary>
			public Byte AcceleratedTransfer;
			/// <summary>Specifies a value of type STORAGE_BUS_TYPE that indicates the type of the bus to which the device is connected.</summary>
			public Byte BusType;
			/// <summary>Specifies the major version number, if any, of the storage adapter. </summary>
			public UInt16 BusMajorVersion;
			/// <summary>Specifies the minor version number, if any, of the storage adapter.</summary>
			public UInt16 BusMinorVersion;
			/// <summary>Specifies the version number, if any, of the storage adapter.</summary>
			public Version BusVersion { get { return new Version(this.BusMajorVersion, this.BusMinorVersion); } }
			/// <summary>С большого перепугу, тут структура типа Byte, а в STORAGE_DEVICE_DESCRIPTOR - Int32...</summary>
			public STORAGE_BUS_TYPE BusTypeReal { get { return (STORAGE_BUS_TYPE)this.BusType; } }
		}
		/// <summary>Contains the serial number of a USB device. It is used by the <see cref="T:Constant.IOCTL_STORAGE.GET_MEDIA_SERIAL_NUMBER"/> control code.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct MEDIA_SERIAL_NUMBER_DATA
		{
			/// <summary>The size of the SerialNumberData string, in bytes.</summary>
			public UInt32 SerialNumberLength;
			/// <summary>The status of the request.</summary>
			public UInt32 Result;
			/// <summary>Reserved.</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
			public UInt32[] Reserved;
			/// <summary>The serial number of the device.</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = Constant.BUFFER_SIZE)]
			public Byte[] SerialNumberData;
		}
		/// <summary>Contains information about the media types supported by a device.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct GET_MEDIA_TYPES
		{
			/// <summary>Provides information about the media supported by a device.</summary>
			[StructLayout(LayoutKind.Explicit)]
			public struct DEVICE_MEDIA_INFO
			{
				/// <summary>The characteristics of the media.</summary>
				[Flags]
				public enum MEDIA : uint
				{
					/// <summary>Media currently mounted</summary>
					CURRENTLY_MOUNTED = 0x80000000,
					/// <summary>Eraseable media</summary>
					ERASEABLE = 0x00000001,
					/// <summary>Read only media</summary>
					READ_ONLY = 0x00000004,
					/// <summary>Read/Write media</summary>
					READ_WRITE = 0x00000008,
					/// <summary>Write once</summary>
					WRITE_ONCE = 0x00000002,
					/// <summary>Write protected</summary>
					WRITE_PROTECTED = 0x00000100,
				}
				/// <summary>Disc info</summary>
				[StructLayout(LayoutKind.Sequential, Pack = 1)]
				public struct DiskInfoStruct
				{
					/// <summary>The number of cylinders on this disk.</summary>
					public UInt64 Cylinders;
					/// <summary>The media type.</summary>
					public WinApi.MEDIA_TYPE MediaType;
					/// <summary>The number of tracks per cylinder.</summary>
					public UInt32 TracksPerCylinder;
					/// <summary>The number of sectors per track.</summary>
					public UInt32 SectorsPerTrack;
					/// <summary>The number of bytes per sector.</summary>
					public UInt32 BytesPerSector;
					/// <summary>The number of sides of the disk that can contain data.</summary>
					/// <remarks>This member is 1 for one-sided media or 2 for two-sided media.</remarks>
					public UInt32 NumberMediaSides;
					/// <summary>The characteristics of the media.</summary>
					public MEDIA MediaCharacteristics;
				}
				/// <summary>Removable disc info</summary>
				[StructLayout(LayoutKind.Sequential, Pack = 1)]
				public struct RemovableDiskInfoStruct
				{
					/// <summary>The number of cylinders on this disk.</summary>
					public UInt64 Cylinders;
					/// <summary>The media type.</summary>
					public WinApi.MEDIA_TYPE MediaType;
					/// <summary>The number of tracks per cylinder.</summary>
					public UInt32 TracksPerCylinder;
					/// <summary>The number of sectors per track.</summary>
					public UInt32 SectorsPerTrack;
					/// <summary>The number of bytes per sector.</summary>
					public UInt32 BytesPerSector;
					/// <summary>The number of sides of the disk that can contain data.</summary>
					/// <remarks>This member is 1 for one-sided media or 2 for two-sided media.</remarks>
					public UInt32 NumberMediaSides;
					/// <summary>The characteristics of the media.</summary>
					public MEDIA MediaCharacteristics;
				}
				/// <summary>Tape info</summary>
				[StructLayout(LayoutKind.Sequential, Pack = 1)]
				public struct TapeInfoStruct
				{
					/// <summary>The media type.</summary>
					public WinApi.MEDIA_TYPE MediaType;
					/// <summary>The characteristics of the media.</summary>
					public UInt32 MediaCharacteristics;
					/// <summary>The current block size, in bytes.</summary>
					public UInt32 CurrentBlockSize;
					/// <summary>The type of bus to which the tape drive is connected.</summary>
					public STORAGE_BUS_TYPE BusType;
					/// <summary>The SCSI-specific medium type.</summary>
					public Byte MediumType;
					/// <summary>The SCSI-specific current operating density for read/write operations.</summary>
					public Byte DensityCode;
				}
				/// <summary>A disc info structure</summary>
				[FieldOffset(0)]
				public DiskInfoStruct DiskInfo;
				/// <summary>A removable media structure</summary>
				[FieldOffset(0)]
				public RemovableDiskInfoStruct RemovableDiskInfo;
				/// <summary>A tape info structure</summary>
				[FieldOffset(0)]
				public TapeInfoStruct TapeInfo;
			}
			/// <summary>The type of device.</summary>
			/// <remarks>Values from 0 through 32,767 are reserved for use by Microsoft Corporation. Values from 32,768 through 65,535 are reserved for use by other vendors.</remarks>
			public STORAGE_DEVICE_NUMBER.FileDevice DeviceType;
			/// <summary>The number of elements in the MediaInfo array.</summary>
			public UInt32 MediaInfoCount;
			/// <summary>A pointer to the first DEVICE_MEDIA_INFO structure in the array. There is one structure for each media type supported by the device.</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32 * 10)]//32-sizeof(DEVICE_MEDIA_INFO)
			public Byte[] MediaInfo;
			/// <summary>Gets media info structure by index</summary>
			/// <param name="index">index</param>
			/// <returns>DEVICE_MEDIA_INFO</returns>
			public DEVICE_MEDIA_INFO this[UInt32 index]
			{
				get
				{
					if (index >= this.MediaInfoCount)
						throw new ArgumentOutOfRangeException("index");
					else
						using (PinnedBufferReader reader = new PinnedBufferReader(this.MediaInfo))
							return reader.BytesToStructure<DEVICE_MEDIA_INFO>(index * (UInt32)Marshal.SizeOf(typeof(DEVICE_MEDIA_INFO)));
				}
			}
		}
		/// <summary>Contains information about a device.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct STORAGE_DEVICE_NUMBER
		{
			/// <summary>Known device types</summary>
			/// <remarks>http://msdn.microsoft.com/en-us/library/windows/hardware/ff563821%28v=vs.85%29.aspx</remarks>
			public enum FileDevice : uint
			{
				/// <summary></summary>
				BEEP = 0x00000001,
				/// <summary></summary>
				CD_ROM = 0x00000002,
				/// <summary></summary>
				CD_ROM_FILE_SYSTEM = 0x00000003,
				/// <summary></summary>
				CONTROLLER = 0x00000004,
				/// <summary></summary>
				DATALINK = 0x00000005,
				/// <summary></summary>
				DFS = 0x00000006,
				/// <summary></summary>
				DISK = 0x00000007,
				/// <summary></summary>
				DISK_FILE_SYSTEM = 0x00000008,
				/// <summary></summary>
				FILE_SYSTEM = 0x00000009,
				/// <summary></summary>
				INPORT_PORT = 0x0000000a,
				/// <summary></summary>
				KEYBOARD = 0x0000000b,
				/// <summary></summary>
				MAILSLOT = 0x0000000c,
				/// <summary></summary>
				MIDI_IN = 0x0000000d,
				/// <summary></summary>
				MIDI_OUT = 0x0000000e,
				/// <summary></summary>
				MOUSE = 0x0000000f,
				/// <summary></summary>
				MULTI_UNC_PROVIDER = 0x00000010,
				/// <summary></summary>
				NAMED_PIPE = 0x00000011,
				/// <summary></summary>
				NETWORK = 0x00000012,
				/// <summary></summary>
				NETWORK_BROWSER = 0x00000013,
				/// <summary></summary>
				NETWORK_FILE_SYSTEM = 0x00000014,
				/// <summary></summary>
				NULL = 0x00000015,
				/// <summary></summary>
				PARALLEL_PORT = 0x00000016,
				/// <summary></summary>
				PHYSICAL_NETCARD = 0x00000017,
				/// <summary></summary>
				PRINTER = 0x00000018,
				/// <summary></summary>
				SCANNER = 0x00000019,
				/// <summary></summary>
				SERIAL_MOUSE_PORT = 0x0000001a,
				/// <summary></summary>
				SERIAL_PORT = 0x0000001b,
				/// <summary></summary>
				SCREEN = 0x0000001c,
				/// <summary></summary>
				SOUND = 0x0000001d,
				/// <summary></summary>
				STREAMS = 0x0000001e,
				/// <summary></summary>
				TAPE = 0x0000001f,
				/// <summary></summary>
				TAPE_FILE_SYSTEM = 0x00000020,
				/// <summary></summary>
				TRANSPORT = 0x00000021,
				/// <summary></summary>
				UNKNOWN = 0x00000022,
				/// <summary></summary>
				VIDEO = 0x00000023,
				/// <summary></summary>
				VIRTUAL_DISK = 0x00000024,
				/// <summary></summary>
				WAVE_IN = 0x00000025,
				/// <summary></summary>
				WAVE_OUT = 0x00000026,
				/// <summary></summary>
				_8042_PORT = 0x00000027,
				/// <summary></summary>
				NETWORK_REDIRECTOR = 0x00000028,
				/// <summary></summary>
				BATTERY = 0x00000029,
				/// <summary></summary>
				BUS_EXTENDER = 0x0000002a,
				/// <summary></summary>
				MODEM = 0x0000002b,
				/// <summary></summary>
				VDM = 0x0000002c,
				/// <summary></summary>
				MASS_STORAGE = 0x0000002d,
				/// <summary></summary>
				SMB = 0x0000002e,
				/// <summary></summary>
				KS = 0x0000002f,
				/// <summary></summary>
				CHANGER = 0x00000030,
				/// <summary></summary>
				SMARTCARD = 0x00000031,
				/// <summary></summary>
				ACPI = 0x00000032,
				/// <summary></summary>
				DVD = 0x00000033,
				/// <summary></summary>
				FULLSCREEN_VIDEO = 0x00000034,
				/// <summary></summary>
				DFS_FILE_SYSTEM = 0x00000035,
				/// <summary></summary>
				DFS_VOLUME = 0x00000036,
				/// <summary></summary>
				SERENUM = 0x00000037,
				/// <summary></summary>
				TERMSRV = 0x00000038,
				/// <summary></summary>
				KSEC = 0x00000039,
				/// <summary></summary>
				FIPS = 0x0000003A,
				/// <summary></summary>
				INFINIBAND = 0x0000003B,
			}
			/// <summary>The type of device.</summary>
			/// <remarks>Values from 0 through 32,767 are reserved for use by Microsoft. Values from 32,768 through 65,535 are reserved for use by other vendors.</remarks>
			public FileDevice DeviceType;
			/// <summary>The number of this device.</summary>
			public UInt32 DeviceNumber;
			/// <summary>The partition number of the device, if the device can be partitioned. Otherwise, this member is –1.</summary>
			public UInt32 PartitionNumber;
		}
	}
}
