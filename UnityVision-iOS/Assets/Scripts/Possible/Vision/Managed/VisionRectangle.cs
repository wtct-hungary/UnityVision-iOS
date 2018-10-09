///////////////////////////////////////////////////////////////////////////////
// VisionRectangle.cs
// 
// Author: Adam Hegedus
// Contact: adam.hegedus@possible.com
// Copyright © 2018 POSSIBLE CEE. Released under the MIT license.
///////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using UnityEngine;

namespace Possible.Vision.Managed
{
    /// <summary>
    /// Represents a rectangle detection result.
    /// </summary>
    public struct VisionRectangle
    {
        public readonly Vector2 topLeft;
        public readonly Vector2 topRight;
        public readonly Vector2 bottomRight;
        public readonly Vector2 bottomLeft;
        public readonly float area;

        public VisionRectangle(Vector2 topLeft, Vector2 topRight, Vector2 bottomRight, Vector2 bottomLeft)
        {
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomRight = bottomRight;
            this.bottomLeft = bottomLeft;
            
            area = CalculateArea(vertices: new[] {topLeft, bottomLeft, bottomRight, topRight});
        }

        /// <summary>
        /// Calculates the rectangular polygon's area using the Shoelace formula.
        /// https://en.wikipedia.org/wiki/Shoelace_formula
        /// </summary>
        private static float CalculateArea(IList<Vector2> vertices)
        {
            var numVertices = vertices.Count;
            var previous = numVertices - 1;
            var area = 0.0f;
			
            for (var current = 0; current < numVertices; current++) 
            {
                var previousVertex = vertices[previous];
                var currentVertex = vertices[current];
                area += previousVertex.x * currentVertex.y - currentVertex.x * previousVertex.y;
                previous = current;
            }
			
            return area * 0.5f;
        }
    }
}