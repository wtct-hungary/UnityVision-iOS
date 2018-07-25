///////////////////////////////////////////////////////////////////////////////
// VisionClassification.cs
// 
// Author: Adam Hegedus
// Contact: adam.hegedus@possible.com
// Copyright © 2018 POSSIBLE CEE. Released under the MIT license.
///////////////////////////////////////////////////////////////////////////////

using System.Runtime.InteropServices;

namespace Possible.Vision
{
    /// <summary>
    /// Represents an image classification result.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VisionClassification
    {
        public float confidence;
        public string identifier;
    }
}