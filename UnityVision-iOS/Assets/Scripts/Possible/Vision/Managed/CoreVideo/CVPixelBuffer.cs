///////////////////////////////////////////////////////////////////////////////
// CVPixelBuffer.cs
// 
// Author: Adam Hegedus
// Contact: adam.hegedus@possible.com
// Copyright © 2018 POSSIBLE CEE. Released under the MIT license.
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Possible.Vision.Managed.CoreVideo
{
	/// <summary>
	/// Managed incomplete wrapper for the native CVPixelBuffer type on iOS platform.
	/// </summary>
	public class CVPixelBuffer : IDisposable
	{
		private GCHandle _managedBufferHandle;
		private IntPtr _nativePtr;

		private CVPixelBuffer()
		{
			_nativePtr = IntPtr.Zero;
		}

		/// <summary>
		/// Returns the address of the native CVPixelBuffer object.
		/// </summary>
		public IntPtr GetNativePtr()
		{
			return _nativePtr;
		}

		/// <summary>
		/// Allocates a new native CVPixelBuffer object using the pixels of the specified texture. This method throws.
		/// Note: The provided texture must be readable!
		/// </summary>
		/// <param name="fromTexture">A managed byte buffer containing the pixels of this texture will get extracted and retained. This texture must be readable.</param>
		/// <param name="alignPixelFormat">Set this argument to true, if the resulting pixel buffer needs to be color accurate.</param>
		/// <returns>A managed wrapper instance of the native CVPixelBuffer object.</returns>
		public static CVPixelBuffer Create(Texture2D fromTexture, bool alignPixelFormat = true)
		{
			var buffer = fromTexture.GetPixels32().ToByteArray();
			
			if (alignPixelFormat)
			{
				ShiftRGBAToARGB(ref buffer);	
			}
			
			return Create(buffer, fromTexture.width, fromTexture.height, PixelFormat.ARGB32);
		}
		
		/// <summary>
		/// Allocates a new native CVPixelBuffer object using the pixels of the specified WebCamTexture. This method throws.
		/// </summary>
		/// <param name="fromWebCamTexture">A managed byte buffer containing the pixels of this texture will get extracted and retained.</param>
		/// <param name="alignPixelFormat">Set this argument to true, if the resulting pixel buffer needs to be color accurate.</param>
		/// <returns>A managed wrapper instance of the native CVPixelBuffer object.</returns>
		public static CVPixelBuffer Create(WebCamTexture fromWebCamTexture, bool alignPixelFormat = true)
		{
			var buffer = fromWebCamTexture.GetPixels32().ToByteArray();
			
			if (alignPixelFormat)
			{
				ShiftRGBAToARGB(ref buffer);	
			}
			
			return Create(buffer, fromWebCamTexture.width, fromWebCamTexture.height, PixelFormat.ARGB32);
		}
		
		/// <summary>
		/// Allocates a new native CVPixelBuffer object that will reference the specified buffer.
		/// The specified array is retained and will not be released until the managed wrapper object is not disposed. 
		/// </summary>
		/// <param name="fromBuffer">A flattened 2D array. Each 4 values represent a single pixel. Pixels are laid out left to right, bottom to top (i.e. row after row).</param>
		/// <param name="width">The width of the image that the specified buffer represents.</param>
		/// <param name="height">The height of the image that the specified buffer represents.</param>
		/// <param name="format">The color layout of an individual pixel.</param>
		/// <returns>A managed wrapper instance of the native CVPixelBuffer object.</returns>
		/// <exception cref="CVOperationFailedException">Thrown in case the native buffer fails to be allocated.</exception>
		public static CVPixelBuffer Create(byte[] fromBuffer, int width, int height, PixelFormat format)
		{
			// Capture the managed buffer object
			var handle = GCHandle.Alloc(fromBuffer, GCHandleType.Pinned);
			
			// This will contain the address of the native CVPixelBuffer object, if created
			IntPtr pixelBufferPtr;
			
			// Attempt to create the native buffer, using data from the captured array
			var operationResult = (CVReturn)_vision_allocateCVPixelBuffer(
				address: handle.AddrOfPinnedObject(), 
				width: width, height: height, format: (int) format, 
				outPixelBufferPtr: out pixelBufferPtr);

			if (operationResult == CVReturn.Success)
			{
				// In case of success, preserve the captured array and return with a managed wrapper instance 
				return new CVPixelBuffer
				{
					_nativePtr = pixelBufferPtr,
					_managedBufferHandle = handle
				};
			}

			// Release the captured array, since the native buffer failed to allocate
			if (handle.IsAllocated)
			{
				handle.Free();
			}
				
			// Throw an exception propagating the reason of failure
			throw new CVOperationFailedException(operationResult);
		}
		
		/// <summary>
		/// Allocates a new native CVPixelBuffer object using the pixels of the specified texture.
		/// This factory method will not throw, and instead returns a value indicating whether the allocation was successful.
		/// Note: The provided texture must be readable!
		/// </summary>
		/// <param name="fromTexture">A managed byte buffer containing the pixels of this texture will get extracted and retained. This texture must be readable.</param>
		/// <param name="result">A managed wrapper instance of the native CVPixelBuffer object.</param>
		/// <param name="alignPixelFormat">Set this argument to true, if the resulting pixel buffer needs to be color accurate.</param>
		/// <returns>The result of the Core Video operation.</returns>
		public static CVReturn TryCreate(Texture2D fromTexture, out CVPixelBuffer result, bool alignPixelFormat = true)
		{
			var buffer = fromTexture.GetPixels32().ToByteArray();
			
			if (alignPixelFormat)
			{
				ShiftRGBAToARGB(ref buffer);
			}
			
			return TryCreate(buffer, fromTexture.width, fromTexture.height, PixelFormat.ARGB32, out result);
		}
		
		/// <summary>
		/// Allocates a new native CVPixelBuffer object using the pixels of the specified WebCamTexture.
		/// This factory method will not throw, and instead returns a value indicating whether the allocation was successful.
		/// </summary>
		/// <param name="fromWebCamTexture">A managed byte buffer containing the pixels of this texture will get extracted and retained.</param>
		/// <param name="result">A managed wrapper instance of the native CVPixelBuffer object.</param>
		/// <param name="alignPixelFormat">Set this argument to true, if the resulting pixel buffer needs to be color accurate.</param>
		/// <returns>The result of the Core Video operation.</returns>
		public static CVReturn TryCreate(WebCamTexture fromWebCamTexture, out CVPixelBuffer result, bool alignPixelFormat = true)
		{
			var buffer = fromWebCamTexture.GetPixels32().ToByteArray();
			
			if (alignPixelFormat)
			{
				ShiftRGBAToARGB(ref buffer);
			}
			
			return TryCreate(buffer, fromWebCamTexture.width, fromWebCamTexture.height, PixelFormat.ARGB32, out result);
		}
		
		/// <summary>
		/// Allocates a new native CVPixelBuffer object that will reference the specified buffer.
		/// The specified array is retained and will not be released until the managed wrapper object is not disposed.
		/// This factory method will not throw, and instead returns a value indicating whether the allocation was successful.
		/// </summary>
		/// <param name="fromBuffer">A flattened 2D array. Each 4 values represent a single pixel. Pixels are laid out left to right, bottom to top (i.e. row after row).</param>
		/// <param name="width">The width of the image that the specified buffer represents.</param>
		/// <param name="height">The height of the image that the specified buffer represents.</param>
		/// <param name="format">The color layout of an individual pixel.</param>
		/// <param name="result">A managed wrapper instance of the native CVPixelBuffer object.</param>
		/// <returns>The result of the Core Video operation.</returns>
		public static CVReturn TryCreate(byte[] fromBuffer, int width, int height, PixelFormat format, out CVPixelBuffer result)
		{
			// Capture the managed buffer object
			var handle = GCHandle.Alloc(fromBuffer, GCHandleType.Pinned);
			
			// This will contain the address of the native CVPixelBuffer object, if created
			IntPtr pixelBufferPtr;
			
			// Attempt to create the native buffer, using texture data from the captured array
			var operationResult = (CVReturn)_vision_allocateCVPixelBuffer(
				address: handle.AddrOfPinnedObject(), 
				width: width, height: height, format: (int) format, 
				outPixelBufferPtr: out pixelBufferPtr);
			
			result = new CVPixelBuffer
			{
				_nativePtr = pixelBufferPtr,
				_managedBufferHandle = handle
			};

			return operationResult;
		}

		/// <summary>
		/// Converts the specified image buffer from RGBA to ARGB pixel format.
		/// </summary>
		private static void ShiftRGBAToARGB(ref byte[] buffer)
		{
			for (var i = 0; i < buffer.Length; i += 4)
			{
				var last = buffer[i + 3];
				buffer[i + 3] = buffer[i + 2];
				buffer[i + 2] = buffer[i + 1];
				buffer[i + 1] = buffer[i];
				buffer[i] = last;
			}
		}
		
		private void ReleaseUnmanagedResources()
		{
			// Release the handle to the managed pixel array
			if (_managedBufferHandle.IsAllocated)
			{
				_managedBufferHandle.Free();
			}
			
			// Release the native CVPixelBuffer object
			_vision_releaseCVPixelBuffer(_nativePtr);
		}

		public void Dispose()
		{
			// Release resources
			ReleaseUnmanagedResources();
			
			// Bypass finalizer
			GC.SuppressFinalize(this);
		}

		~CVPixelBuffer()
		{
			// Release resources in finalizer in case the object was not disposed properly
			ReleaseUnmanagedResources();
		}
		
		#region Native Bindings
		
#if UNITY_IOS && !UNITY_EDITOR

		[DllImport("__Internal")]
		private static extern int _vision_allocateCVPixelBuffer(IntPtr address, int width, int height, int format, out IntPtr outPixelBufferPtr);
	
		[DllImport("__Internal")]
		private static extern void _vision_releaseCVPixelBuffer(IntPtr pixelBufferPtr);
#else
		
		private static int _vision_allocateCVPixelBuffer(IntPtr address, int width, int height, int format, out IntPtr outPixelBufferPtr)
		{
			outPixelBufferPtr = IntPtr.Zero;
			return 0;
		}

		private static void _vision_releaseCVPixelBuffer(IntPtr pixelBufferPtr)
		{
			// ...
		}
		
#endif
		
		#endregion
	}
}
