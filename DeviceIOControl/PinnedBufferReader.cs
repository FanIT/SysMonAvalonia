using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace DeviceIOControl
{
	[DefaultProperty("Length")]
	public class PinnedBufferReader : IDisposable
	{
		private GCHandle _gcHandle;

		private IntPtr _gcPointer;

		private readonly byte[] _buffer;

		private byte[] Buffer => _buffer;

		private IntPtr Handle => _gcPointer;

		public byte this[uint index] => Buffer[index];

		public int Length => Buffer.Length;

		//[EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
		public PinnedBufferReader(byte[] buffer)
		{
			_buffer = buffer;
			_gcHandle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
			_gcPointer = _gcHandle.AddrOfPinnedObject();
		}

		//[EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
		public T BytesToStructure<T>(ref uint padding) where T : struct
		{
			int length;
			T result = BytesToStructure<T>(padding, out length);
			padding += (uint)length;
			return result;
		}

		//[EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
		public T BytesToStructure<T>(uint padding) where T : struct
		{
			int length;
			return BytesToStructure<T>(padding, out length);
		}

		//[EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
		public T BytesToStructure<T>(uint padding, out int length) where T : struct
		{
			length = Marshal.SizeOf(typeof(T));
			if (length + padding > Buffer.Length)
			{
				throw new ArgumentOutOfRangeException("padding");
			}
			IntPtr ptr = ((padding == 0) ? Handle : new IntPtr(Handle.ToInt64() + padding));
			return (T)Marshal.PtrToStructure(ptr, typeof(T));
		}

		public string BytesToStringUni(uint padding)
		{
			int length;
			return BytesToStringUni(padding, out length);
		}

		public string BytesToStringUni(ref uint padding)
		{
			int length;
			string result = BytesToStringUni(padding, out length);
			padding += (uint)length;
			return result;
		}

		public string BytesToStringUni(uint padding, out int length)
		{
			if (padding > Buffer.Length)
			{
				throw new ArgumentOutOfRangeException("padding");
			}
			IntPtr ptr = ((padding == 0) ? Handle : new IntPtr(Handle.ToInt64() + padding));
			string text = Marshal.PtrToStringUni(ptr);
			length = (text.Length + 1) * Marshal.SystemDefaultCharSize;
			return text;
		}

		public string BytesToStringAnsi(uint padding)
		{
			int length;
			return BytesToStringAnsi(padding, out length);
		}

		public string BytesToStringAnsi(ref uint padding)
		{
			int length;
			string result = BytesToStringAnsi(padding, out length);
			padding += (uint)length;
			return result;
		}

		public string BytesToStringAnsi(uint padding, out int length)
		{
			if (padding > Buffer.Length)
			{
				throw new ArgumentOutOfRangeException("padding");
			}
			IntPtr ptr = ((padding == 0) ? Handle : new IntPtr(Handle.ToInt64() + padding));
			string text = Marshal.PtrToStringAnsi(ptr);
			length = text.Length + 1;
			return text;
		}

		public byte[] GetBytes(uint padding, uint length)
		{
			if (padding + length > Buffer.Length)
			{
				throw new ArgumentOutOfRangeException("padding");
			}
			byte[] array = new byte[length];
			Array.Copy(Buffer, padding, array, 0L, array.Length);
			return array;
		}

		public static T BytesToStructure<T>(byte[] buffer, ref uint padding) where T : struct
		{
			int length;
			T result = BytesToStructure<T>(buffer, padding, out length);
			padding += (uint)length;
			return result;
		}

		public static T BytesToStructure<T>(byte[] buffer, uint padding) where T : struct
		{
			int length;
			return BytesToStructure<T>(buffer, padding, out length);
		}

		public static T BytesToStructure<T>(byte[] buffer, uint padding, out int length) where T : struct
		{
			using PinnedBufferReader pinnedBufferReader = new PinnedBufferReader(buffer);
			return pinnedBufferReader.BytesToStructure<T>(padding, out length);
		}

		public static string BytesToStringUni(byte[] buffer, uint padding, out int length)
		{
			using PinnedBufferReader pinnedBufferReader = new PinnedBufferReader(buffer);
			return pinnedBufferReader.BytesToStringUni(padding, out length);
		}

		public static string BytesToStringAnsi(byte[] buffer, uint padding, out int length)
		{
			using PinnedBufferReader pinnedBufferReader = new PinnedBufferReader(buffer);
			return pinnedBufferReader.BytesToStringAnsi(padding, out length);
		}

		public static byte[] StructureToArray<T>(T structure) where T : struct
		{
			int num = Marshal.SizeOf((object)structure);
			byte[] array = new byte[num];
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			try
			{
				Marshal.StructureToPtr((object)structure, intPtr, fDeleteOld: true);
				Marshal.Copy(intPtr, array, 0, num);
				return array;
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && _gcHandle.IsAllocated)
			{
				_gcHandle.Free();
			}
		}
	}
}
