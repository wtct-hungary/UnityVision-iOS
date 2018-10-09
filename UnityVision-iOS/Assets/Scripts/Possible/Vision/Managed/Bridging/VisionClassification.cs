///////////////////////////////////////////////////////////////////////////////
// VisionClassification.cs
// 
// Author: Adam Hegedus
// Contact: adam.hegedus@possible.com
// Copyright © 2018 POSSIBLE CEE. Released under the MIT license.
///////////////////////////////////////////////////////////////////////////////

using System.Runtime.InteropServices;

namespace Possible.Vision.Managed.Bridging
{
    /// <summary>
    /// Represents a single entry extracted from the native classification result buffer. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VisionClassification
    {
        public float confidence;
        public string identifier;
    }
}