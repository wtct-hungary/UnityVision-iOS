///////////////////////////////////////////////////////////////////////////////
// CVReturn.cs
// 
// Author: Adam Hegedus
// Contact: adam.hegedus@possible.com
// Copyright © 2018 POSSIBLE CEE. Released under the MIT license.
///////////////////////////////////////////////////////////////////////////////

namespace Possible.Vision.Managed.CoreVideo
{
    /// <summary>
    /// An incomplete counterpart for the native CVReturn type on iOS platform.
    /// </summary>
    public enum CVReturn
    {
        // Common
        Success = 0,
        First = -6660,
        InvalidArgument = -6661,
        AllocationFailed = -6662,
        Unsupported = -6663,
        Last = -6699,
        
        // CVPixelBuffer
        InvalidPixelBufferAttributes = -6682,
        InvalidPixelFormat = -6680,
        InvalidSize = -6681,
        PixelBufferNotMetalCompatible = -6684,
        PixelBufferNotOpenGLCompatible = -6683
    }
}
