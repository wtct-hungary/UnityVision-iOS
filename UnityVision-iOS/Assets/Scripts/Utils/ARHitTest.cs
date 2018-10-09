///////////////////////////////////////////////////////////////////////////////
// ARHitTest.cs
// 
// Author: Adam Hegedus
// Contact: adam.hegedus@possible.com
// Copyright © 2018 POSSIBLE CEE. Released under the MIT license.
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using Possible.Vision.Managed;
using UnityEngine;
using UnityEngine.XR.iOS;

namespace Utils
{
    /// <summary>
    /// Convenience methods for performing hit tests with ARKit.
    /// </summary>
    public static class ARHitTest
    {
        private static bool HitTestWithResultType(ARPoint point, ARHitTestResultType resultTypes, out Matrix4x4 hitTransform)
        {
            var hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, resultTypes);
            if (hitResults.Any())
            {
                hitTransform = hitResults.First().worldTransform;
                return true;
            }

            hitTransform = default(Matrix4x4);
            return false;
        }

        /// <summary>
        /// Casts the provided rectangle onto a previously detected horizontal surface.
        /// </summary>
        /// <param name="rectangle">Rectangle.</param>
        /// <param name="onHit">Called when the specified rectangle is successfully cast to a horizontal plane.</param>
        public static void CastRectangle(VisionRectangle rectangle, Action<Vector3, Vector3, Vector3, Vector3> onHit)
        {
            Matrix4x4 result;
            Vector3 topLeft, topRight, bottomRight, bottomLeft;
            const ARHitTestResultType hitResultType = ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent;

            // Top left
            if (HitTestWithResultType(new ARPoint { x = rectangle.topLeft.x, y = rectangle.topLeft.y }, hitResultType, out result))
            {
                topLeft = UnityARMatrixOps.GetPosition(result);
            }
            else return;

            // Top right
            if (HitTestWithResultType(new ARPoint { x = rectangle.topRight.x, y = rectangle.topRight.y }, hitResultType, out result))
            {
                topRight = UnityARMatrixOps.GetPosition(result);
            }
            else return;

            // Bottom right
            if (HitTestWithResultType(new ARPoint { x = rectangle.bottomRight.x, y = rectangle.bottomRight.y }, hitResultType, out result))
            {
                bottomRight = UnityARMatrixOps.GetPosition(result);
            }
            else return;

            // Bottom left
            if (HitTestWithResultType(new ARPoint { x = rectangle.bottomLeft.x, y = rectangle.bottomLeft.y }, hitResultType, out result))
            {
                bottomLeft = UnityARMatrixOps.GetPosition(result);
            }
            else return;

            if (onHit != null)
            {
                onHit(topLeft, topRight, bottomRight, bottomLeft);
            }
        }
    }
}