///////////////////////////////////////////////////////////////////////////////
// VisionRectangle.cs
// 
// Author: Adam Hegedus
// Contact: adam.hegedus@possible.com
// Copyright © 2018 POSSIBLE CEE. Released under the MIT license.
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace Possible.Vision
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

        public VisionRectangle(Vector2 topLeft, Vector2 topRight, Vector2 bottomRight, Vector2 bottomLeft)
        {
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomRight = bottomRight;
            this.bottomLeft = bottomLeft;
        }
    }
}