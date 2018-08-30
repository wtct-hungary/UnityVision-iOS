///////////////////////////////////////////////////////////////////////////////
// VisionBarcode.cs
// 
// Author: Adam Hegedus
// Contact: adam.hegedus@possible.com
// Copyright © 2018 POSSIBLE CEE. Released under the MIT license.
///////////////////////////////////////////////////////////////////////////////

using System.Runtime.InteropServices;

namespace Plugins.iOS.Vision.Managed
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VisionBarcode
    {
        public string symbology;
        public string payload;
    }
}
