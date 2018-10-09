using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Possible.Vision.Managed
{
    /// <summary>
    /// A collection of utility and extension methods for common practices of marshalling.
    /// </summary>
    public static class MarshalUtils
    {
        /// <summary>
        /// Copies a native array of structs to the managed heap. The native buffer does not get released by this method.
        /// </summary>
        /// <param name="source">Ptr to the native array.</param>
        /// <param name="count">Length of the native array.</param>
        /// <param name="isLongPtr">Set this argument to false in case of 32 bit systems.</param>
        /// <typeparam name="T">Type of the elements in the produced collection. This type needs to mirror its native counterpart.</typeparam>
        /// <returns>The resulting copy of the native array that can be garbage collected.</returns>
        public static T[] NativeHandleToManagedArrayOfStructs<T>(IntPtr source, int count, bool isLongPtr = true) where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            var sourcePtr = isLongPtr ? source.ToInt64() : source.ToInt32();
            var result = new T[count];
            for (var i = 0; i < count; i++)
            {
                result[i] = (T) Marshal.PtrToStructure(new IntPtr(sourcePtr + i * size), typeof(T));
            }

            return result;
        }
        
        /// <summary>
        /// Allocates and returns the specified collection of struct instances to a byte buffer.
        /// Source: https://stackoverflow.com/questions/25311361/copy-array-to-struct-array-as-fast-as-possible-in-c-sharp
        /// </summary>
        /// <param name="source">Array to copy.</param>
        /// <returns>A newly allocated data buffer containing the byte representation of the source collection.</returns>
        public static byte[] ToByteArray<T>(this ICollection<T> source) where T : struct
        {
            var handle = GCHandle.Alloc(source, GCHandleType.Pinned);
            try
            {
                var pointer = handle.AddrOfPinnedObject();
                var dest = new byte[source.Count * Marshal.SizeOf(typeof(T))];
                Marshal.Copy(pointer, dest, 0, dest.Length);
                
                return dest;
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }

        /// <summary>
        /// Converts a raw data buffer to an array of structs.
        /// Source: https://stackoverflow.com/questions/25311361/copy-array-to-struct-array-as-fast-as-possible-in-c-sharp 
        /// </summary>
        /// <param name="source">Source buffer to convert.</param>
        /// <returns>Resulting array of structs.</returns>
        public static T[] FromByteArray<T>(this byte[] source) where T : struct
        {
            var destination = new T[source.Length / Marshal.SizeOf(typeof(T))];
            var handle = GCHandle.Alloc(destination, GCHandleType.Pinned);
            try
            {
                var pointer = handle.AddrOfPinnedObject();
                Marshal.Copy(source, 0, pointer, source.Length);
                
                return destination;
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }
    }
}
