using System;
using System.Runtime.InteropServices;

namespace DeviceIOControl.Native
{
	/// <summary>File System control API</summary>
	public struct FsctlApi
	{
		/// <summary>Represents volume data. This structure is passed to the <see cref="T:Constant.FSCTL.GET_NTFS_VOLUME_DATA"/> control code.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct NTFS_VOLUME_DATA_BUFFER
		{
			/// <summary>The serial number of the volume. This is a unique number assigned to the volume media by the operating system.</summary>
			public UInt64 VolumeSerialNumber;
			/// <summary>The number of sectors in the specified volume.</summary>
			public UInt64 NumberSectors;
			/// <summary>The number of used and free clusters in the specified volume.</summary>
			public UInt64 TotalClusters;
			/// <summary>The number of free clusters in the specified volume.</summary>
			public UInt64 FreeClusters;
			/// <summary>The number of reserved clusters in the specified volume.</summary>
			public UInt64 TotalReserved;
			/// <summary>The number of bytes in a sector on the specified volume.</summary>
			public UInt32 BytesPerSector;
			/// <summary>The number of bytes in a cluster on the specified volume. This value is also known as the cluster factor.</summary>
			public UInt32 BytesPerCluster;
			/// <summary>The number of bytes in a file record segment.</summary>
			public UInt32 BytesPerFileRecordSegment;
			/// <summary>The number of clusters in a file record segment.</summary>
			public UInt32 ClustersPerFileRecordSegment;
			/// <summary>The length of the master file table, in bytes.</summary>
			public UInt64 MftValidDataLength;
			/// <summary>The starting logical cluster number of the master file table.</summary>
			public UInt64 MftStartLcn;
			/// <summary>The starting logical cluster number of the master file table mirror.</summary>
			public UInt64 Mft2StartLcn;
			/// <summary>The starting logical cluster number of the master file table zone.</summary>
			public UInt64 MftZoneStart;
			/// <summary>The ending logical cluster number of the master file table zone.</summary>
			public UInt64 MftZoneEnd;
		}

		/// <summary>Contains the starting LCN to the<see cref="T:Constant.FSCTL.GET_VOLUME_BITMAP"/> control code.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct STARTING_LCN_INPUT_BUFFER
		{
			/// <summary>
			/// The LCN from which the operation should start when describing a bitmap.
			/// This member will be rounded down to a file-system-dependent rounding boundary, and that value will be returned.
			/// </summary>
			/// <remarks>Its value should be an integral multiple of eight.</remarks>
			public UInt64 StartingLcn;
		}

		/// <summary>Represents the occupied and available clusters on a disk. This structure is the output buffer for the <see cref="T:Constant.FSCTL.GET_VOLUME_BITMAP"/> control code.</summary>
		/// <remarks>http://msdn.microsoft.com/en-us/library/windows/desktop/aa365726%28v=vs.85%29.aspx</remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct VOLUME_BITMAP_BUFFER
		{
			/// <summary>Starting LCN requested as an input to the operation.</summary>
			public UInt64 StartingLcn;
			/// <summary>The number of clusters on the volume, starting from the starting LCN returned in the StartingLcn member of this structure.</summary>
			public UInt64 BitmapSize;
			/// <summary>
			/// Array of bytes containing the bitmap that the operation returns.
			/// The bitmap is bitwise from bit zero of the bitmap to the end.
			/// Thus, starting at the requested cluster, the bitmap goes from bit 0 of byte 0, bit 1 of byte 0 ... bit 7 of byte 0, bit 0 of byte 1, and so on.
			/// The value 1 indicates that the cluster is allocated (in use).
			/// The value 0 indicates that the cluster is not allocated (free).
			/// </summary>
			/// <remarks>
			/// Необходимо возвращать массив байт, а на массив байт накладывать первую структуру, а затем читать буфер.
			/// Но если разбить на куски, скажем, по 512 байт, то чтение всего массива может занять несколько минут. Проще сразу передать достаточно большой массив.
			/// </remarks>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 100000000)]
			//[MarshalAs(UnmanagedType.ByValArray, SizeConst = Constant.BUFFER_SIZE)]
			public Byte[] Buffer;
		}

		/// <summary>Contains data for the FSCTL_FIND_FILES_BY_SID control code.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct FIND_BY_SID_DATA
		{
			/// <summary>
			/// Indicates whether to restart the search.
			/// This member should be 1 on first call, so the search will start from the root.
			/// For subsequent calls, this member should be zero so the search will resume at the point where it stopped.
			/// </summary>
			public UInt32 Restart;
			/// <summary>A SID structure that specifies the desired creator owner.</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)]//TODO: Пока не понял как передать точно фиксированный массив...
			public Byte[] Sid;
		}

		/// <summary>Represents a file name</summary>
		/// <remarks>This size does not include the NULL character.</remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct FIND_BY_SID_OUTPUT
		{
			/// <summary>The size of the file name, in bytes.</summary>
			public UInt32 FileNameLength;
			/// <summary>A pointer to a null-terminated string that specifies the file name.</summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = Constant.BUFFER_SIZE)]
			public Byte[] FileName;
		}

		/// <summary>Contains statistical information from the file system.</summary>
		/// <remarks>
		/// There are two types of files: user and metadata. User files are available for the user.
		/// Metadata files are system files that contain information, which the file system uses for its internal organization.
		/// The number of read and write operations measured is the number of paging operations.
		/// </remarks>
		[StructLayout(LayoutKind.Explicit)]
		public struct FILESYSTEM_STATISTICS
		{
			/// <summary>The type of file system.</summary>
			public enum FILESYSTEM_STATISTICS_TYPE : ushort
			{
				/// <summary>
				/// The file system is the NTFS file system.
				/// If this value is set, this structure is followed by an <see cref="NTFS_STATISTICS"/> structure.
				/// </summary>
				NTFS = 1,
				/// <summary>
				/// The file system is a FAT file system.
				/// If this value is set, this structure is followed by a <see cref="FAT_STATISTICS"/> structure.
				/// </summary>
				FAT = 2,
				/// <summary>
				/// The file system is an exFAT file system.
				/// If this value is set, this structure is followed by an <see cref="EXFAT_STATISTICS"/> structure.
				/// </summary>
				/// <remarks>This value is not supported until Windows Vista with SP1.</remarks>
				EXFAT = 3,
			}
			/// <summary>The type of file system.</summary>
			[FieldOffset(0)]
			public FILESYSTEM_STATISTICS_TYPE FileSystemType;
			/// <summary>This member is set to 1 (one).</summary>
			[FieldOffset(2)]
			public UInt16 Version;
			/// <summary>The size of this structure plus the size of the file system-specific structure that follows this structure, multiplied by the number of processors.</summary>
			/// <remarks>
			/// This value must be a multiple of 64. For example, if the size of FILESYSTEM_STATISTICS is 0x38, the size of NTFS_STATISTICS is 0xd4, and if there are 2 processors, the buffer allocated must be 0x280.
			/// sizeof(FILESYSTEM_STATISTICS) = 0x38
			/// sizeof(NTFS_STATISTICS) = 0xd4
			/// Total Size = 0x10c
			/// size of the complete structure = 0x140 (which is the aligned length, a multiple of 64)
			/// multiplied by 2 (the number of processors) = 0x280
			/// </remarks>
			[FieldOffset(4)]
			public UInt32 SizeOfCompleteStructure;
			/// <summary>The number of read operations on user files.</summary>
			[FieldOffset(8)]
			public UInt32 UserFileReads;
			/// <summary>The number of bytes read from user files.</summary>
			[FieldOffset(12)]
			public UInt32 UserFileReadBytes;
			/// <summary>
			/// The number of read operations on user files.
			/// This value includes sub-read operations.
			/// </summary>
			[FieldOffset(16)]
			public UInt32 UserDiskReads;
			/// <summary>The number of write operations on user files.</summary>
			[FieldOffset(20)]
			public UInt32 UserFileWrites;
			/// <summary>The number of bytes written to user files.</summary>
			[FieldOffset(24)]
			public UInt32 UserFileWriteBytes;
			/// <summary>
			/// The number of write operations on user files.
			/// This value includes sub-write operations.
			/// </summary>
			[FieldOffset(28)]
			public UInt32 UserDiskWrites;
			/// <summary>The number of read operations on metadata files.</summary>
			[FieldOffset(32)]
			public UInt32 MetaDataReads;
			/// <summary>The number of bytes read from metadata files.</summary>
			[FieldOffset(36)]
			public UInt32 MetaDataReadBytes;
			/// <summary>
			/// The number of read operations on metadata files.
			/// This value includes sub-read operations.
			/// </summary>
			[FieldOffset(40)]
			public UInt32 MetaDataDiskReads;
			/// <summary>The number of write operations on metadata files.</summary>
			[FieldOffset(44)]
			public UInt32 MetaDataWrites;
			/// <summary>The number of bytes written to metadata files.</summary>
			[FieldOffset(48)]
			public UInt32 MetaDataWriteBytes;
			/// <summary>
			/// The number of write operations on metadata files.
			/// This value includes sub-write operations.
			/// </summary>
			[FieldOffset(52)]
			public UInt32 MetaDataDiskWrites;

			//Win32Exception: More data is available
			/// <summary>Contains statistical information from the NTFS file system.</summary>
			[FieldOffset(56)]
			public NTFS_STATISTICS Ntfs;
			/// <summary>Contains statistical information from the FAT file system.</summary>
			[FieldOffset(56)]
			public FAT_STATISTICS Fat;
			/// <summary>Contains statistical information from the exFAT file system.</summary>
			[FieldOffset(56)]
			public EXFAT_STATISTICS ExFat;

			//[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2024)]
			//[FieldOffset(56)]
			//public Byte[] Data;
		}

		/// <summary>Contains statistical information from the file system.</summary>
		/// <remarks>
		/// There are two types of files: user and metadata. User files are available for the user.
		/// Metadata files are system files that contain information, which the file system uses for its internal organization.
		/// The number of read and write operations measured is the number of paging operations.
		/// Support for this structure started with Windows 10.
		/// </remarks>
		[StructLayout(LayoutKind.Explicit)]
		public struct FILESYSTEM_STATISTICS_EX
		{
			/// <summary>The type of file system.</summary>
			[FieldOffset(0)]
			public FILESYSTEM_STATISTICS.FILESYSTEM_STATISTICS_TYPE FileSystemType;
			/// <summary>This member is set to 1 (one).</summary>
			[FieldOffset(2)]
			public UInt16 Version;
			/// <summary>The size of this structure plus the size of the file system-specific structure that follows this structure, multiplied by the number of processors.</summary>
			/// <remarks>
			/// This value must be a multiple of 64. For example, if the size of FILESYSTEM_STATISTICS is 0x38, the size of NTFS_STATISTICS is 0xd4, and if there are 2 processors, the buffer allocated must be 0x280.
			/// sizeof(FILESYSTEM_STATISTICS) = 0x38
			/// sizeof(NTFS_STATISTICS) = 0xd4
			/// Total Size = 0x10c
			/// size of the complete structure = 0x140 (which is the aligned length, a multiple of 64)
			/// multiplied by 2 (the number of processors) = 0x280
			/// </remarks>
			[FieldOffset(4)]
			public UInt32 SizeOfCompleteStructure;
			/// <summary>The number of read operations on user files.</summary>
			[FieldOffset(8)]
			public UInt64 UserFileReads;
			/// <summary>The number of bytes read from user files.</summary>
			[FieldOffset(16)]
			public UInt64 UserFileReadBytes;
			/// <summary>
			/// The number of read operations on user files.
			/// This value includes sub-read operations.
			/// </summary>
			[FieldOffset(24)]
			public UInt64 UserDiskReads;
			/// <summary>The number of write operations on user files.</summary>
			[FieldOffset(32)]
			public UInt64 UserFileWrites;
			/// <summary>The number of bytes written to user files.</summary>
			[FieldOffset(40)]
			public UInt64 UserFileWriteBytes;
			/// <summary>
			/// The number of write operations on user files.
			/// This value includes sub-write operations.
			/// </summary>
			[FieldOffset(48)]
			public UInt64 UserDiskWrites;
			/// <summary>The number of read operations on metadata files.</summary>
			[FieldOffset(56)]
			public UInt64 MetaDataReads;
			/// <summary>The number of bytes read from metadata files.</summary>
			[FieldOffset(64)]
			public UInt64 MetaDataReadBytes;
			/// <summary>
			/// The number of read operations on metadata files.
			/// This value includes sub-read operations.
			/// </summary>
			[FieldOffset(72)]
			public UInt64 MetaDataDiskReads;
			/// <summary>The number of write operations on metadata files.</summary>
			[FieldOffset(80)]
			public UInt64 MetaDataWrites;
			/// <summary>The number of bytes written to metadata files.</summary>
			[FieldOffset(88)]
			public UInt64 MetaDataWriteBytes;
			/// <summary>
			/// The number of write operations on metadata files.
			/// This value includes sub-write operations.
			/// </summary>
			[FieldOffset(96)]
			public UInt64 MetaDataDiskWrites;

			//Win32Exception: More data is available
			/// <summary>Contains statistical information from the NTFS file system.</summary>
			[FieldOffset(104)]
			public NTFS_STATISTICS_EX Ntfs;
			/// <summary>Contains statistical information from the FAT file system.</summary>
			[FieldOffset(104)]
			public FAT_STATISTICS Fat;
			/// <summary>Contains statistical information from the exFAT file system.</summary>
			[FieldOffset(104)]
			public EXFAT_STATISTICS ExFat;

			//[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2024)]
			//[FieldOffset(104)]
			//public Byte[] Data;
		}

		/// <summary>Contains statistical information from the FAT file system.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct FAT_STATISTICS
		{
			/// <summary>The number of create operations.</summary>
			public UInt32 CreateHits;
			/// <summary>The number of successful create operations.</summary>
			public UInt32 SuccessfulCreates;
			/// <summary>The number of failed create operations.</summary>
			public UInt32 FailedCreates;
			/// <summary>The number of read operations that were not cached.</summary>
			public UInt32 NonCachedReads;
			/// <summary>The number of bytes read from a file that were not cached.</summary>
			public UInt32 NonCachedReadBytes;
			/// <summary>The number of write operations that were not cached.</summary>
			public UInt32 NonCachedWrites;
			/// <summary>The number of bytes written to a file that were not cached.</summary>
			public UInt32 NonCachedWriteBytes;
			/// <summary>The number of read operations that were not cached. This value includes sub-read operations.</summary>
			public UInt32 NonCachedDiskReads;
			/// <summary>The number of write operations that were not cached. This value includes sub-write operations.</summary>
			public UInt32 NonCachedDiskWrites;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = Constant.BUFFER_SIZE)]
			private Byte[] Data;
		}
		/// <summary>Contains statistical information from the exFAT file system.</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct EXFAT_STATISTICS
		{
			/// <summary>The number of create operations.</summary>
			public UInt32 CreateHits;
			/// <summary>The number of successful create operations.</summary>
			public UInt32 SuccessfulCreates;
			/// <summary>The number of failed create operations.</summary>
			public UInt32 FailedCreates;
			/// <summary>The number of read operations that were not cached.</summary>
			public UInt32 NonCachedReads;
			/// <summary>The number of bytes read from a file that were not cached.</summary>
			public UInt32 NonCachedReadBytes;
			/// <summary>The number of write operations that were not cached.</summary>
			public UInt32 NonCachedWrites;
			/// <summary>The number of bytes written to a file that were not cached.</summary>
			public UInt32 NonCachedWriteBytes;
			/// <summary>The number of read operations that were not cached. This value includes sub-read operations.</summary>
			public UInt32 NonCachedDiskReads;
			/// <summary>The number of write operations that were not cached. This value includes sub-write operations.</summary>
			public UInt32 NonCachedDiskWrites;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = Constant.BUFFER_SIZE)]
			private Byte[] Data;
		}
		/// <summary>Contains statistical information from the NTFS file system.</summary>
		/// <remarks>
		/// The MFT, MFT mirror, root index, user index, bitmap, and MFT bitmap are counted as metadata files.
		/// The log file is not counted as a metadata file.
		/// The number of read and write operations measured is the number of paging operations.
		/// https://msdn.microsoft.com/en-us/library/windows/desktop/aa365255%28v=vs.85%29.aspx
		/// </remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct NTFS_STATISTICS
		{
			/// <summary>Number of writes</summary>
			[StructLayout(LayoutKind.Sequential)]
			public struct MftWrites
			{
				/// <summary>
				/// The number of MFT writes due to a write operation.
				/// The number of MFT mirror writes due to a write operation.
				/// The number of MFT bitmap writes due to a write operation.
				/// </summary>
				public UInt16 Write;
				/// <summary>
				/// The number of MFT writes due to a create operation.
				/// The number of MFT mirror writes due to a create operation.
				/// The number of bitmap writes due to a create operation.
				/// </summary>
				public UInt16 Create;
				/// <summary>
				/// The number of MFT writes due to setting file information.
				/// The number of MFT mirror writes due to setting file information.
				/// The number of bitmap writes due to setting file information.
				/// </summary>
				public UInt16 SetInfo;
				/// <summary>
				/// The number of MFT writes due to a flush operation.
				/// The number of MFT mirror writes due to a flush operation.
				/// The number of bitmap writes due to a flush operation.
				/// </summary>
				public UInt16 Flush;
			}
			/// <summary>The number of bitmap writes</summary>
			[StructLayout(LayoutKind.Sequential)]
			public struct BitmapWrites
			{
				/// <summary>The number of bitmap writes due to a write operation.</summary>
				public UInt16 Write;
				/// <summary>The number of bitmap writes due to a create operation.</summary>
				public UInt16 Create;
				/// <summary>The number of bitmap writes due to setting file information.</summary>
				public UInt16 SetInfo;
			}
			/// <summary>Allocate clusters</summary>
			[StructLayout(LayoutKind.Sequential)]
			public struct AllocateStruct
			{
				/// <summary>The number of individual calls to allocate clusters.</summary>
				public UInt32 Calls;
				/// <summary>The number of clusters allocated.</summary>
				public UInt32 Clusters;
				/// <summary>The number of times a hint was specified.</summary>
				public UInt32 Hints;
				/// <summary>The number of runs used to satisfy all the requests.</summary>
				public UInt32 RunsReturned;
				/// <summary>The number of times the hint was useful.</summary>
				public UInt32 HintsHonored;
				/// <summary>The number of clusters allocated through the hint.</summary>
				public UInt32 HintsClusters;
				/// <summary>The number of times the cache was useful other than the hint.</summary>
				public UInt32 Cache;
				/// <summary>The number of clusters allocated through the cache other than the hint.</summary>
				public UInt32 CacheClusters;
				/// <summary>The number of times the cache was not useful.</summary>
				public UInt32 CacheMiss;
				/// <summary>The number of clusters allocated without the cache.</summary>
				public UInt32 CacheMissClusters;
			}
			/// <summary>The number of exceptions generated due to the log file being full.</summary>
			public UInt32 LogFileFullExceptions;
			/// <summary>The number of other exceptions generated.</summary>
			public UInt32 OtherExceptions;
			/// <summary>The number of read operations on the master file table (MFT).</summary>
			public UInt32 MftReads;
			/// <summary>The number of bytes read from the MFT.</summary>
			public UInt32 MftReadBytes;
			/// <summary>The number of write operations on the MFT.</summary>
			public UInt32 MftWrites1;
			/// <summary>The number of bytes written to the MFT.</summary>
			public UInt32 MftWriteBytes;
			/// <summary>The number of MFT writes</summary>
			public MftWrites MftWritesUserLevel;
			/// <summary>The number of flushes of the MFT performed because the log file was full.</summary>
			public UInt16 MftWritesFlushForLogFileFull;
			/// <summary>The number of MFT write operations performed by the lazy writer thread.</summary>
			public UInt16 MftWritesLazyWriter;
			/// <summary>Reserved.</summary>
			public UInt16 MftWritesUserRequest;
			/// <summary>The number of write operations on the MFT mirror.</summary>
			public UInt32 Mft2Writes;
			/// <summary>The number of bytes written to the MFT mirror.</summary>
			public UInt32 Mft2WriteBytes;
			/// <summary>The number of MFT mirror writes</summary>
			public MftWrites Mft2WritesUserLevel;
			/// <summary>The number of flushes of the MFT mirror performed because the log file was full.</summary>
			public UInt16 Mft2WritesFlushForLogFileFull;
			/// <summary>The number of MFT mirror write operations performed by the lazy writer thread.</summary>
			public UInt16 Mft2WritesLazyWriter;
			/// <summary>Reserved.</summary>
			public UInt16 Mft2WritesUserRequest;
			/// <summary>The number of read operations on the root index.</summary>
			public UInt32 RootIndexReads;
			/// <summary>The number of bytes read from the root index.</summary>
			public UInt32 RootIndexReadBytes;
			/// <summary>The number of write operations on the root index.</summary>
			public UInt32 RootIndexWrites;
			/// <summary>The number of bytes written to the root index.</summary>
			public UInt32 RootIndexWriteBytes;
			/// <summary>The number of read operations on the cluster allocation bitmap.</summary>
			public UInt32 BitmapReads;
			/// <summary>The number of bytes read from the cluster allocation bitmap.</summary>
			public UInt32 BitmapReadBytes;
			/// <summary>The number of write operations on the cluster allocation bitmap.</summary>
			public UInt32 BitmapWrites1;
			/// <summary>The number of bytes written to the cluster allocation bitmap.</summary>
			public UInt32 BitmapWriteBytes;
			/// <summary>The number of flushes of the bitmap performed because the log file was full.</summary>
			public UInt16 BitmapWritesFlushForLogFileFull;
			/// <summary>The number of bitmap write operations performed by the lazy writer thread.</summary>
			public UInt16 BitmapWritesLazyWriter;
			/// <summary>Reserved.</summary>
			public UInt16 BitmapWritesUserRequest;
			/// <summary>The number of bitmap writes</summary>
			public BitmapWrites BitmapWritesUserLevel;
			/// <summary>The number of read operations on the MFT bitmap.</summary>
			public UInt32 MftBitmapReads;
			/// <summary>The number of bytes read from the MFT bitmap.</summary>
			public UInt32 MftBitmapReadBytes;
			/// <summary>The number of write operations on the MFT bitmap.</summary>
			public UInt32 MftBitmapWrites;
			/// <summary>The number of bytes written to the MFT bitmap.</summary>
			public UInt32 MftBitmapWriteBytes;
			/// <summary>The number of flushes of the MFT bitmap performed because the log file was full.</summary>
			public UInt16 MftBitmapWritesFlushForLogFileFull;
			/// <summary>The number of MFT bitmap write operations performed by the lazy writer thread.</summary>
			public UInt16 MftBitmapWritesLazyWriter;
			/// <summary>Reserved.</summary>
			public UInt16 MftBitmapWritesUserRequest;
			/// <summary>The number of MFT bitmap writes</summary>
			public MftWrites MftBitmapWritesUserLevel;
			/// <summary>The number of read operations on the user index.</summary>
			public UInt32 UserIndexReads;
			/// <summary>The number of bytes read from the user index.</summary>
			public UInt32 UserIndexReadBytes;
			/// <summary>The number of write operations on the user index.</summary>
			public UInt32 UserIndexWrites;
			/// <summary>The number of bytes written to the user index.</summary>
			public UInt32 UserIndexWriteBytes;
			/// <summary>The number of read operations on the log file.</summary>
			public UInt32 LogFileReads;
			/// <summary>The number of bytes read from the log file.</summary>
			public UInt32 LogFileReadBytes;
			/// <summary>The number of write operations on the log file.</summary>
			public UInt32 LogFileWrites;
			/// <summary>The number of bytes written to the log file.</summary>
			public UInt32 LogFileWriteBytes;
			/// <summary>Allocate clusters</summary>
			public AllocateStruct Allocate;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = Constant.BUFFER_SIZE)]
			private Byte[] Data;
		}

		/// <summary>Contains statistical information from the NTFS file system.</summary>
		/// <remarks>
		/// The MFT, MFT mirror, root index, user index, bitmap, and MFT bitmap are counted as metadata files.
		/// The log file is not counted as a metadata file.
		/// The number of read and write operations measured is the number of paging operations.
		/// https://msdn.microsoft.com/en-us/library/windows/desktop/aa365255%28v=vs.85%29.aspx
		/// Support for this structure started with Windows 10
		/// </remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct NTFS_STATISTICS_EX
		{
			/// <summary>Number of writes</summary>
			[StructLayout(LayoutKind.Sequential)]
			public struct MftWrites
			{
				/// <summary>
				/// The number of MFT writes due to a write operation.
				/// The number of MFT mirror writes due to a write operation.
				/// The number of MFT bitmap writes due to a write operation.
				/// </summary>
				public UInt32 Write;
				/// <summary>
				/// The number of MFT writes due to a create operation.
				/// The number of MFT mirror writes due to a create operation.
				/// The number of bitmap writes due to a create operation.
				/// </summary>
				public UInt32 Create;
				/// <summary>
				/// The number of MFT writes due to setting file information.
				/// The number of MFT mirror writes due to setting file information.
				/// The number of bitmap writes due to setting file information.
				/// </summary>
				public UInt32 SetInfo;
				/// <summary>
				/// The number of MFT writes due to a flush operation.
				/// The number of MFT mirror writes due to a flush operation.
				/// The number of bitmap writes due to a flush operation.
				/// </summary>
				public UInt32 Flush;
			}
			/// <summary>The number of bitmap writes</summary>
			[StructLayout(LayoutKind.Sequential)]
			public struct BitmapWrites
			{
				/// <summary>The number of bitmap writes due to a write operation.</summary>
				public UInt32 Write;
				/// <summary>The number of bitmap writes due to a create operation.</summary>
				public UInt32 Create;
				/// <summary>The number of bitmap writes due to setting file information.</summary>
				public UInt32 SetInfo;
			}
			/// <summary>Allocate clusters</summary>
			[StructLayout(LayoutKind.Sequential)]
			public struct AllocateStruct
			{
				/// <summary>The number of individual calls to allocate clusters.</summary>
				public UInt32 Calls;
				/// <summary>The number of runs used to satisfy all the requests.</summary>
				public UInt32 RunsReturned;
				/// <summary>The number of times a hint was specified.</summary>
				public UInt32 Hints;
				/// <summary>The number of times the hint was useful.</summary>
				public UInt32 HintsHonored;
				/// <summary>The number of times the cache was useful other than the hint.</summary>
				public UInt32 Cache;
				/// <summary>The number of times the cache was not useful.</summary>
				public UInt32 CacheMiss;
				/// <summary>The number of clusters allocated.</summary>
				public UInt64 Clusters;
				/// <summary>The number of clusters allocated through the hint.</summary>
				public UInt64 HintsClusters;
				/// <summary>The number of clusters allocated through the cache other than the hint.</summary>
				public UInt64 CacheClusters;
				/// <summary>The number of clusters allocated without the cache.</summary>
				public UInt64 CacheMissClusters;
			}
			/// <summary>The number of exceptions generated due to the log file being full.</summary>
			public UInt32 LogFileFullExceptions;
			/// <summary>The number of other exceptions generated.</summary>
			public UInt32 OtherExceptions;
			/// <summary>The number of read operations on the master file table (MFT).</summary>
			public UInt64 MftReads;
			/// <summary>The number of bytes read from the MFT.</summary>
			public UInt64 MftReadBytes;
			/// <summary>The number of write operations on the MFT.</summary>
			public UInt64 MftWrites1;
			/// <summary>The number of bytes written to the MFT.</summary>
			public UInt64 MftWriteBytes;
			/// <summary>The number of MFT writes</summary>
			public MftWrites MftWritesUserLevel;
			/// <summary>The number of flushes of the MFT performed because the log file was full.</summary>
			public UInt32 MftWritesFlushForLogFileFull;
			/// <summary>The number of MFT write operations performed by the lazy writer thread.</summary>
			public UInt32 MftWritesLazyWriter;
			/// <summary>Reserved.</summary>
			public UInt32 MftWritesUserRequest;
			/// <summary>The number of write operations on the MFT mirror.</summary>
			public UInt64 Mft2Writes;
			/// <summary>The number of bytes written to the MFT mirror.</summary>
			public UInt64 Mft2WriteBytes;
			/// <summary>The number of MFT mirror writes</summary>
			public MftWrites Mft2WritesUserLevel;
			/// <summary>The number of flushes of the MFT mirror performed because the log file was full.</summary>
			public UInt32 Mft2WritesFlushForLogFileFull;
			/// <summary>The number of MFT mirror write operations performed by the lazy writer thread.</summary>
			public UInt32 Mft2WritesLazyWriter;
			/// <summary>Reserved.</summary>
			public UInt32 Mft2WritesUserRequest;
			/// <summary>The number of read operations on the root index.</summary>
			public UInt64 RootIndexReads;
			/// <summary>The number of bytes read from the root index.</summary>
			public UInt64 RootIndexReadBytes;
			/// <summary>The number of write operations on the root index.</summary>
			public UInt64 RootIndexWrites;
			/// <summary>The number of bytes written to the root index.</summary>
			public UInt64 RootIndexWriteBytes;
			/// <summary>The number of read operations on the cluster allocation bitmap.</summary>
			public UInt64 BitmapReads;
			/// <summary>The number of bytes read from the cluster allocation bitmap.</summary>
			public UInt64 BitmapReadBytes;
			/// <summary>The number of write operations on the cluster allocation bitmap.</summary>
			public UInt64 BitmapWrites1;
			/// <summary>The number of bytes written to the cluster allocation bitmap.</summary>
			public UInt64 BitmapWriteBytes;
			/// <summary>The number of flushes of the bitmap performed because the log file was full.</summary>
			public UInt32 BitmapWritesFlushForLogFileFull;
			/// <summary>The number of bitmap write operations performed by the lazy writer thread.</summary>
			public UInt32 BitmapWritesLazyWriter;
			/// <summary>Reserved.</summary>
			public UInt32 BitmapWritesUserRequest;
			/// <summary>The number of bitmap writes</summary>
			public BitmapWrites BitmapWritesUserLevel;
			/// <summary>The number of read operations on the MFT bitmap.</summary>
			public UInt64 MftBitmapReads;
			/// <summary>The number of bytes read from the MFT bitmap.</summary>
			public UInt64 MftBitmapReadBytes;
			/// <summary>The number of write operations on the MFT bitmap.</summary>
			public UInt64 MftBitmapWrites;
			/// <summary>The number of bytes written to the MFT bitmap.</summary>
			public UInt64 MftBitmapWriteBytes;
			/// <summary>The number of flushes of the MFT bitmap performed because the log file was full.</summary>
			public UInt32 MftBitmapWritesFlushForLogFileFull;
			/// <summary>The number of MFT bitmap write operations performed by the lazy writer thread.</summary>
			public UInt32 MftBitmapWritesLazyWriter;
			/// <summary>Reserved.</summary>
			public UInt32 MftBitmapWritesUserRequest;
			/// <summary>The number of MFT bitmap writes</summary>
			public MftWrites MftBitmapWritesUserLevel;
			/// <summary>The number of read operations on the user index.</summary>
			public UInt64 UserIndexReads;
			/// <summary>The number of bytes read from the user index.</summary>
			public UInt64 UserIndexReadBytes;
			/// <summary>The number of write operations on the user index.</summary>
			public UInt64 UserIndexWrites;
			/// <summary>The number of bytes written to the user index.</summary>
			public UInt64 UserIndexWriteBytes;
			/// <summary>The number of read operations on the log file.</summary>
			public UInt64 LogFileReads;
			/// <summary>The number of bytes read from the log file.</summary>
			public UInt64 LogFileReadBytes;
			/// <summary>The number of write operations on the log file.</summary>
			public UInt64 LogFileWrites;
			/// <summary>The number of bytes written to the log file.</summary>
			public UInt64 LogFileWriteBytes;
			/// <summary>Allocate clusters</summary>
			public AllocateStruct Allocate;
			/// <summary>The number of failed attempts made to acquire a slab of storage for use on the current thinly provisioned volume</summary>
			public UInt64 DiskResourcesExhausted;
			/// <summary>The number of volume level trim operations issued</summary>
			public UInt64 VolumeTrimCount;
			/// <summary>The total time elapsed during all volume level trim operations</summary>
			/// <remarks>This value, divided by the frequency value from QueryPerformanceFrequency or KeQueryPerformanceCounter, will give the time in seconds</remarks>
			public UInt64 VolumeTrimTime;
			/// <summary>The total number of bytes issued by all volume level trim operations</summary>
			public UInt64 VolumeTrimByteCount;
			/// <summary>The number of file level trim operations issued</summary>
			public UInt64 FileLevelTrimCount;
			/// <summary>The total time elapsed during all file level trim operations</summary>
			/// <remarks>This value, divided by the frequency value from QueryPerformanceFrequency or KeQueryPerformanceCounter, will give the time in seconds</remarks>
			public UInt64 FileLevelTrimTime;
			/// <summary>The total number of bytes issued by all file level trim operations</summary>
			public UInt64 FileLevelTrimByteCount;
			/// <summary>The number of times a volume level trim operation was aborted before being sent down through the storage stack</summary>
			public UInt64 VolumeTrimSkippedCount;
			/// <summary>The number of bytes that were not sent through a volume level trim operation because they were skipped</summary>
			public UInt64 VolumeTrimSkippedByteCount;
			/// <summary>banana banana banana</summary>
			public UInt64 NtfsFillStatInfoFromMftRecordCalledCount;
			/// <summary>banana banana banana</summary>
			public UInt64 NtfsFillStatInfoFromMftRecordBailedBecauseOfAttributeListCount;
			/// <summary>banana banana banana</summary>
			public UInt64 NtfsFillStatInfoFromMftRecordBailedBecauseOfNonResReparsePointCount;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = Constant.BUFFER_SIZE)]
			private Byte[] Data;
		}
	}
}
