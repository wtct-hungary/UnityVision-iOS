///////////////////////////////////////////////////////////////////////////////
// PixelFormat.cs
// 
// Author: Adam Hegedus
// Contact: adam.hegedus@possible.com
// Copyright © 2018 POSSIBLE CEE. Released under the MIT license.
///////////////////////////////////////////////////////////////////////////////

namespace Possible.Vision.Managed.CoreVideo
{
    public enum PixelFormat
    {
        /// <summary>
        /// Each of RGBA color channels is stored as an 8-bit value in [0..255] range.
        /// In memory, the channel data is ordered this way: A, R, G, B bytes one after another.
        /// </summary>
        ARGB32 = 0,
        
        /// <summary>
        /// Each of RGB color channels is stored as an 8-bit value in [0..255] range.
        /// In memory, the channel data is ordered this way: R, G, B bytes one after another.
        /// </summary>
        RGB24 = 1
    }
}
