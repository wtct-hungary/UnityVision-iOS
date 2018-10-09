///////////////////////////////////////////////////////////////////////////////
// VisionRequest.cs
// 
// Author: Adam Hegedus
// Contact: adam.hegedus@possible.com
// Copyright © 2018 POSSIBLE CEE. Released under the MIT license.
///////////////////////////////////////////////////////////////////////////////

using System;

namespace Possible.Vision.Managed
{
    /// <summary>
    /// Used to specify the type of vision request to perform.
    /// </summary>
    [Flags]
    public enum VisionRequest
    {
        /// <summary>
        /// Do nothing.
        /// </summary>
        None = 0,

        /// <summary>
        /// Classify the dominant object on the frame.
        /// </summary>
        Classification = 1,

        /// <summary>
        /// Detect rectangles on the frame.
        /// </summary>
        RectangleDetection = 2,

        /// <summary>
        /// Both classify the dominant object on the frame and detect rectangles.
        /// </summary>
        ClassificationAndRectangleDetection = Classification | RectangleDetection
    }

    public static class VisionRequestExtensions
    {
        public static bool HasFlag(this VisionRequest request, VisionRequest flag)
        {
            return (request & flag) == flag;
        }
    }
}

