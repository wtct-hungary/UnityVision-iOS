///////////////////////////////////////////////////////////////////////////////
// EventArguments.cs
// 
// Author: Adam Hegedus
// Contact: adam.hegedus@possible.com
// Copyright © 2018 POSSIBLE CEE. Released under the MIT license.
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Plugins.iOS.Vision.Managed
{
    /// <summary>
    /// Carries the results of a successful barcode detection request.
    /// </summary>
    public class BarcodesDetectedArgs : EventArgs
    {
        public readonly VisionBarcode[] barcodes;

        public BarcodesDetectedArgs(VisionBarcode[] barcodes)
        {
            this.barcodes = barcodes;
        }
    }
    
    /// <summary>
    /// Carries the results of a successful rectangle detection request.
    /// </summary>
    public class RectanglesRecognizedArgs : EventArgs
    {
        /// <summary>
        /// Rectangles with their respective normalized frame coordinates.
        /// </summary>
        public readonly VisionRectangle[] rectangles;

        public RectanglesRecognizedArgs(IList<Vector2> points)
        {
            var rectCount = points.Count / 4;
            rectangles = new VisionRectangle[rectCount];
            for (var i = 0; i < rectCount; i += 4)
            {
                rectangles[i] = new VisionRectangle(
                    topLeft: points[i], topRight: points[i + 1],
                    bottomRight: points[i + 2], bottomLeft: points[i + 3]);
            }
        }
    }
}