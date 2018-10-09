///////////////////////////////////////////////////////////////////////////////
// CGPoint.cs
// 
// Author: Adam Hegedus
// Contact: adam.hegedus@possible.com
// Copyright © 2018 POSSIBLE CEE. Released under the MIT license.
///////////////////////////////////////////////////////////////////////////////

using System.Runtime.InteropServices;
using UnityEngine;

namespace Possible.Vision.Managed.Bridging
{
    /// <summary>
    /// Managed counterpart for the CGPoint type on iOS platform.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CGPoint
    {
        public double x;
        public double y;

        public CGPoint(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Converts CGPoint to Unity's Vector2 type.
        /// </summary>
        public static implicit operator Vector2(CGPoint point)
        {
            return new Vector2((float)point.x, (float)point.y);
        }
    }
}