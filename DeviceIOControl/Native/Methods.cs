using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Security;
using System.Security.Permissions;

namespace DeviceIOControl.Native
{
	/// <summary>Native methods</summary>
	internal static class Methods
	{
		/// <summary>
		/// Creates or opens a file or I/O device.
		/// The most commonly used I/O devices are as follows: file, file stream, directory, physical disk, volume, console buffer, tape drive, communications resource, mailslot, and pipe.
		/// The function returns a handle that can be used to access the file or device for various types of I/O depending on the file or device and the flags and attributes specified.
		/// </summary>
		/// <remarks>http://msdn.microsoft.com/en-us/library/windows/desktop/aa363858%28v=vs.85%29.aspx</remarks>
		/// <param name="lpFileName">The name of the file or device to be created or opened. You may use either forward slashes (/) or backslashes (\) in this name.</param>
		/// <param name="dwDesiredAccess">The requested access to the file or device, which can be summarized as read, write, both or neither zero).</param>
		/// <param name="dwShareMode">
		/// The requested sharing mode of the file or device, which can be read, write, both, delete, all of these, or none (refer to the following table).
		/// Access requests to attributes or extended attributes are not affected by this flag.
		/// </param>
		/// <param name="lpSecurityAttributes">A pointer to a SECURITY_ATTRIBUTES structure that contains two separate but related data members: an optional security descriptor, and a Boolean value that determines whether the returned handle can be inherited by child processes.</param>
		/// <param name="dwCreationDisposition">An action to take on a file or device that exists or does not exist.</param>
		/// <param name="dwFlagsAndAttributes">The file or device attributes and flags, FILE_ATTRIBUTE_NORMAL being the most common default value for files.</param>
		/// <param name="hTemplateFile">A valid handle to a template file with the GENERIC_READ access right. The template file supplies file attributes and extended attributes for the file that is being created.</param>
		/// <returns>If the function succeeds, the return value is an open handle to the specified file, device, named pipe, or mail slot.</returns>
		[DllImport("kernel32.dll", EntryPoint = "CreateFileA", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Ansi)]
		private static extern IntPtr CreateFile(
			[In] String lpFileName,
			WinApi.FILE_ACCESS_FLAGS dwDesiredAccess,
			WinApi.FILE_SHARE dwShareMode,
			IntPtr lpSecurityAttributes,
			WinApi.CreateDisposition dwCreationDisposition,
			UInt32 dwFlagsAndAttributes,
			IntPtr hTemplateFile);

		/// <summary>Opens specified device</summary>
		/// <param name="lpFileName">Device path</param>
		/// <returns>Handle to the opened device</returns>
		public static IntPtr OpenDevice(String lpFileName)
		{
			return Methods.OpenDevice(lpFileName,
				WinApi.FILE_ACCESS_FLAGS.GENERIC_READ | WinApi.FILE_ACCESS_FLAGS.GENERIC_WRITE,
				WinApi.FILE_SHARE.READ | WinApi.FILE_SHARE.WRITE);
		}

		/// <summary>Opens specified device</summary>
		/// <param name="lpFileName">Device path</param>
		/// <param name="dwDesiredAccess">Desired access flage</param>
		/// <param name="dwShareMode">Share mode</param>
		/// <exception cref="ArgumentNullException">lpFileName is null or empty</exception>
		/// <exception cref="Win32Exception">Device does not opened</exception>
		/// <returns>Handle to the opened device</returns>
		public static IntPtr OpenDevice(String lpFileName, WinApi.FILE_ACCESS_FLAGS dwDesiredAccess, WinApi.FILE_SHARE dwShareMode)
		{
			if (String.IsNullOrEmpty(lpFileName))
				throw new ArgumentNullException("lpFileName");

			IntPtr result = Methods.CreateFile(lpFileName,
				dwDesiredAccess,
				dwShareMode,
				IntPtr.Zero,
				WinApi.CreateDisposition.OPEN_EXISTING,
				0,
				IntPtr.Zero);

			if (result == Constant.INVALID_HANDLE_VALUE)
				throw new Win32Exception();
			return result;
		}

		/// <summary>
		/// The LookupAccountName function accepts the name of a system and an account as input.
		/// It retrieves a security identifier (SID) for the account and the name of the domain on which the account was found.
		/// The LsaLookupNames function can also retrieve computer accounts.
		/// </summary>
		/// <param name="lpSystemName">
		/// A pointer to a null-terminated character string that specifies the name of the system.
		/// This string can be the name of a remote computer.
		/// If this string is NULL, the account name translation begins on the local system.
		/// If the name cannot be resolved on the local system, this function will try to resolve the name using domain controllers trusted by the local system.
		/// Generally, specify a value for <c>lpSystemName</c> only when the account is in an untrusted domain and the name of a computer in that domain is known.
		/// </param>
		/// <param name="lpAccountName">
		/// A pointer to a null-terminated string that specifies the account name.
		/// Use a fully qualified string in the <c>domain_name\user_name</c> format to ensure that <c>LookupAccountName</c> finds the account in the desired domain.
		/// </param>
		/// <param name="Sid">
		/// A pointer to a buffer that receives the SID structure that corresponds to the account name pointed to by the <c>lpAccountName</c> parameter.
		/// If this parameter is NULL, <c>cbSid</c> must be zero.
		/// </param>
		/// <param name="cbSid">
		/// A pointer to a variable.
		/// On input, this value specifies the size, in bytes, of the <c>Sid</c> buffer.
		/// If the function fails because the buffer is too small or if <c>cbSid</c> is zero, this variable receives the required buffer size.
		/// </param>
		/// <param name="ReferencedDomainName">
		/// A pointer to a buffer that receives the name of the domain where the account name is found.
		/// For computers that are not joined to a domain, this buffer receives the computer name.
		/// If this parameter is NULL, the function returns the required buffer size.
		/// </param>
		/// <param name="cchReferencedDomainName">
		/// A pointer to a variable.
		/// On input, this value specifies the size, in TCHARs, of the <c>ReferencedDomainName</c> buffer.
		/// If the function fails because the buffer is too small, this variable receives the required buffer size, including the terminating null character.
		/// If the <c>ReferencedDomainName</c> parameter is NULL, this parameter must be zero.
		/// </param>
		/// <param name="peUse">
		/// A pointer to a <see cref="WinApi.SID_NAME_USE"/> enumerated type that indicates the type of the account when the function returns.
		/// </param>
		/// <returns>
		/// If the function succeeds, the function returns nonzero.
		/// If the function fails, it returns zero. For extended error information, call <see cref="Marshal.GetLastWin32Error"/>.
		/// </returns>
		[DllImport("advapi32.dll", EntryPoint = "LookupAccountName", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern Boolean LookupAccountName(
			String lpSystemName,
			String lpAccountName,
			[MarshalAs(UnmanagedType.LPArray)] Byte[] Sid,
			ref UInt32 cbSid,
			StringBuilder ReferencedDomainName,
			ref UInt32 cchReferencedDomainName,
			out WinApi.SID_NAME_USE peUse);

		/// <summary>
		/// The LookupAccountName function accepts the name of a system and an account as input.
		/// It retrieves a security identifier (SID) for the account and the name of the domain on which the account was found.
		/// The LsaLookupNames function can also retrieve computer accounts.
		/// </summary>
		/// <param name="accountName">
		/// A pointer to a null-terminated string that specifies the account name.
		/// Use a fully qualified string in the domain_name\user_name format to ensure that LookupAccountName finds the account in the desired domain.
		/// </param>
		/// <param name="capacity">
		/// A pointer to a variable. On input, this value specifies the size, in bytes, of the Sid buffer.
		/// If the function fails because the buffer is too small or if cbSid is zero, this variable receives the required buffer size.
		/// </param>
		/// <returns>SID structure that corresponds to the account name pointed to by the accountName parameter.</returns>
		public static Byte[] LookupAccountName(String accountName, Int32 capacity = 16)
		{
			Byte[] result = new Byte[capacity];
			UInt32 cbSid = (UInt32)capacity;
			StringBuilder referencedDomainName = new StringBuilder(capacity);
			UInt32 cchReferencedDomainName = (UInt32)referencedDomainName.Capacity;
			WinApi.SID_NAME_USE sidUse;

			Boolean isSuccess = Methods.LookupAccountName(null,
				accountName,
				result,
				ref cbSid,
				referencedDomainName,
				ref cchReferencedDomainName,
				out sidUse);

			if (isSuccess)
				return result;
			else
			{
				Int32 error = Marshal.GetLastWin32Error();
				switch ((Constant.ERROR)error)
				{
					case Constant.ERROR.INSUFFICIENT_BUFFER:
					case Constant.ERROR.INVALID_FLAGS://On Windows Server 2003 this error is/can be returned, but processing can still continue
						return LookupAccountName(accountName, (Int32)cbSid);
					default:
						throw new Win32Exception((Int32)error);
				}
			}
		}

		[DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern Boolean CloseHandle(IntPtr hObject);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("kernel32.dll", EntryPoint = "GetDevicePowerState", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern Boolean GetDevicePowerState(IntPtr hDevice, out Boolean pfOn);

		[DllImport("kernel32.dll", EntryPoint = "RequestDeviceWakeup", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern Boolean RequestDeviceWakeup(IntPtr hDevice);

		[DllImport("kernel32.dll", EntryPoint = "CancelDeviceWakeupRequest", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern Boolean CancelDeviceWakeupRequest(IntPtr hDevice);

		/// <summary>Determines whether a disk drive is a removable, fixed, CD-ROM, RAM disk, or network drive.</summary>
		/// <remarks>To determine whether a drive is a USB-type drive, call SetupDiGetDeviceRegistryProperty and specify the SPDRP_REMOVAL_POLICY property.</remarks>
		/// <param name="lpRootPathName">
		/// The root directory for the drive.
		/// A trailing backslash is required. If this parameter is NULL, the function uses the root of the current directory.
		/// </param>
		/// <returns>The return value specifies the type of drive.</returns>
		[SuppressUnmanagedCodeSecurity]
		[SecuritySafeCritical]
		[DllImport("kernel32.dll", EntryPoint = "GetDriveTypeA", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern WinApi.DRIVE GetDriveTypeA(
			[In] String lpRootPathName);

		/// <summary>Sends a control code directly to a specified device driver, causing the corresponding device to perform the corresponding operation.</summary>
		/// <remarks>http://msdn.microsoft.com/en-us/library/windows/desktop/aa363216%28v=vs.85%29.aspx</remarks>
		/// <param name="hDevice">A handle to the device on which the operation is to be performed. The device is typically a volume, directory, file, or stream.</param>
		/// <param name="dwIoControlCode">The control code for the operation. This value identifies the specific operation to be performed and the type of device on which to perform it.</param>
		/// <param name="lpInBuffer">
		/// A pointer to the input buffer that contains the data required to perform the operation. The format of this data depends on the value of the dwIoControlCode parameter.
		/// This parameter can be NULL if dwIoControlCode specifies an operation that does not require input data.
		/// </param>
		/// <param name="nInBufferSize">The size of the input buffer, in bytes.</param>
		/// <param name="lpOutBuffer">
		/// A pointer to the output buffer that is to receive the data returned by the operation. The format of this data depends on the value of the dwIoControlCode parameter.
		/// This parameter can be NULL if dwIoControlCode specifies an operation that does not return data.
		/// </param>
		/// <param name="nOutBufferSize">The size of the output buffer, in bytes.</param>
		/// <param name="lpBytesReturned">A pointer to a variable that receives the size of the data stored in the output buffer, in bytes.</param>
		/// <param name="lpOverlapped">
		/// A pointer to an OVERLAPPED structure.
		/// If hDevice was opened without specifying FILE_FLAG_OVERLAPPED, lpOverlapped is ignored.
		/// </param>
		/// <returns>If the operation completes successfully, the return value is nonzero.</returns>
		[DllImport("kernel32.dll", EntryPoint = "DeviceIoControl", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern Boolean DeviceIoControl(
			IntPtr hDevice,
			UInt32 dwIoControlCode,
			IntPtr lpInBuffer,
			UInt32 nInBufferSize,
			IntPtr lpOutBuffer,
			UInt32 nOutBufferSize,
			ref UInt32 lpBytesReturned,
			[In] ref NativeOverlapped lpOverlapped);

		public static T DeviceIoControl<T>(
			IntPtr hDevice,
			UInt32 dwIoControlCode,
			Object inParams,
			out UInt32 lpBytesReturned) where T : struct
		{
			T result;
			if (Methods.DeviceIoControl<T>(hDevice, dwIoControlCode, inParams, out lpBytesReturned, out result))
				return result;
			else
				throw new Win32Exception();
		}

		/// <summary>Sends a control code directly to a specified device driver, causing the corresponding device to perform the corresponding operation.</summary>
		/// <remarks>http://msdn.microsoft.com/en-us/library/windows/desktop/aa363216%28v=vs.85%29.aspx</remarks>
		/// <param name="hDevice">A handle to the device on which the operation is to be performed. The device is typically a volume, directory, file, or stream.</param>
		/// <param name="dwIoControlCode">The control code for the operation. This value identifies the specific operation to be performed and the type of device on which to perform it.</param>
		/// <param name="inParams">
		/// A pointer to the input buffer that contains the data required to perform the operation. The format of this data depends on the value of the dwIoControlCode parameter.
		/// This parameter can be NULL if dwIoControlCode specifies an operation that does not require input data.
		/// </param>
		/// <param name="lpBytesReturned">A pointer to a variable that receives the size of the data stored in the output buffer, in bytes.</param>
		/// <param name="outBuffer">
		/// A pointer to the output buffer that is to receive the data returned by the operation. The format of this data depends on the value of the dwIoControlCode parameter.
		/// This parameter can be NULL if dwIoControlCode specifies an operation that does not return data.
		/// </param>
		/// <returns>If the operation completes successfully, the return value is nonzero.</returns>
		//[EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
		public static Boolean DeviceIoControl<T>(
			IntPtr hDevice,
			UInt32 dwIoControlCode,
			Object inParams,
			out UInt32 lpBytesReturned,
			out T outBuffer) where T : struct
		{
			Int32 nInBufferSize = 0;
			IntPtr lpInBuffer = IntPtr.Zero;
			IntPtr lpOutBuffer = IntPtr.Zero;

			try
			{
				if (inParams != null)
				{
					nInBufferSize = Marshal.SizeOf(inParams);
					lpInBuffer = Marshal.AllocHGlobal(nInBufferSize);
					Marshal.StructureToPtr(inParams, lpInBuffer, true);
				}

				outBuffer = new T();
				Int32 nOutBufferSize = Marshal.SizeOf(typeof(T));

				lpOutBuffer = Marshal.AllocHGlobal(nOutBufferSize);
				Marshal.StructureToPtr(outBuffer, lpOutBuffer, true);

				lpBytesReturned = 0;
				NativeOverlapped lpOverlapped = new NativeOverlapped();

				Boolean result = Methods.DeviceIoControl(
					hDevice,
					dwIoControlCode,
					lpInBuffer,
					(UInt32)nInBufferSize,
					lpOutBuffer,
					(UInt32)nOutBufferSize,
					ref lpBytesReturned,
					ref lpOverlapped);
				//if(result) В некоторых случаях даже при отрицательном значении необходимо читать исходящий буфер
				outBuffer = (T)Marshal.PtrToStructure(lpOutBuffer, typeof(T));

				return result;
			}
			finally
			{
				if (lpInBuffer != IntPtr.Zero)
					Marshal.FreeHGlobal(lpInBuffer);
				if (lpOutBuffer != IntPtr.Zero)
					Marshal.FreeHGlobal(lpOutBuffer);
			}
		}
	}
}
