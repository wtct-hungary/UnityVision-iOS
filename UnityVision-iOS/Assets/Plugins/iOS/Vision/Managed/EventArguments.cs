using System;
using System.Collections.Generic;
using UnityEngine;

namespace Possible.Vision
{
    /// <summary>
    /// Carries the results of a succesful image classification request.
    /// </summary>
    public class ClassificationResultArgs : EventArgs
    {
        public readonly VisionClassification[] observations;

        public ClassificationResultArgs(VisionClassification[] observations)
        {
            this.observations = observations;
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