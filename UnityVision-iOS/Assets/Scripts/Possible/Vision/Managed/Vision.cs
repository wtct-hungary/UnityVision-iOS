///////////////////////////////////////////////////////////////////////////////
// Vision.cs
// 
// Author: Adam Hegedus
// Contact: adam.hegedus@possible.com
// Copyright © 2018 POSSIBLE CEE. Released under the MIT license.
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Possible.Vision.Managed.Bridging;
using UnityEngine;

namespace Possible.Vision.Managed
{
    /// <summary>
    /// Managed wrapper for using specific features of iOS Vision Framework and CoreML.
    /// </summary>
    public class Vision : MonoBehaviour
    {
        #region Native Bindings

#if UNITY_IOS && !UNITY_EDITOR

        [DllImport("__Internal")]
        private static extern void _vision_setCallbackTarget(string target); 

        [DllImport("__Internal")]
        private static extern void _vision_allocateVisionRequests(int requestType, int maxObservations); 

        [DllImport("__Internal")]
        private static extern int _vision_evaluateWithBuffer(IntPtr buffer); 
    
        [DllImport("__Internal")]
        private static extern int _vision_evaluateWithTexture(IntPtr texture); 

        [DllImport("__Internal")]
        private static extern int _vision_acquirePointBuffer([In, Out] CGPoint[] pointBuffer); 

        [DllImport("__Internal")]
        private static extern int _vision_acquireClassificationBuffer([In, Out] VisionClassification[] classificationBuffer, int maxObservations);

#else

        private static void _vision_setCallbackTarget(string target)
        {
            // ...
        }

        private static void _vision_allocateVisionRequests(int requestType, int maxObservations)
        {
            // ...
        }

        private static int _vision_evaluateWithBuffer(IntPtr buffer)
        {
            return 1;
        }
        
        private static int _vision_evaluateWithTexture(IntPtr texture)
        {
            return 1;
        }

        private static int _vision_acquirePointBuffer([In, Out] CGPoint[] pointBuffer)
        {
            return 0;
        }

        private static int _vision_acquireClassificationBuffer([In, Out] VisionClassification[] classificationBuffer, int maxObservations)
        {
            return 0;
        }

#endif

		#endregion

        /// <summary>
        /// Callback for when an object gets classified.
        /// </summary>
        public event EventHandler<ClassificationResultArgs> OnObjectClassified;

        /// <summary>
        /// Callback for when rectangles get recognized.
        /// </summary>
        public event EventHandler<RectanglesRecognizedArgs> OnRectanglesRecognized;

        /// <summary>
        /// Vision requests to perform when evaluating a pixel buffer.
        /// </summary>
        private VisionRequest _requestsToPerform = VisionRequest.None;

        /// <summary>
        /// Indicates what type of requests are currently being in progress.
        /// </summary>
        private VisionRequest _requestsInProgress = VisionRequest.None;

        /// <summary>
        /// Buffer used to copy rectangle detection results from the native buffer.
        /// </summary>
        private CGPoint[] _pointBuffer = new CGPoint[40];   // Default number of rectangles per frame: 10

        /// <summary>
        /// Buffer used to copy image classification results from the native buffer.
        /// </summary>
        private VisionClassification[] _classificationBuffer = new VisionClassification[10];

        /// <summary>
        /// Number of maximum observation results.
        /// </summary>
        private int _maxObservations = 10;

        /// <summary>
        /// Indicates if there are any vision requests in progress.
        /// </summary>
        public bool InProgress 
        { 
            get { return _requestsInProgress != VisionRequest.None; }
        }

		private void Awake()
		{
            // Set native callback target to this GameObject
            _vision_setCallbackTarget(gameObject.name);
		}

        /// <summary>
        /// Note: Heavy call!
        /// Allocates VNRequest objects for iOS Vision Framework based on the specified flags.
        /// </summary>
        /// <param name="requests">Requests.</param>
        /// <param name="maxObservations"></param>
        public void SetAndAllocateRequests(VisionRequest requests, int maxObservations)
        {
            // Cache the required vision requests
            _requestsToPerform = requests;

            // Allocate vision requests
            _vision_allocateVisionRequests((int)_requestsToPerform, maxObservations);

            // Re-allocate copy buffers
            _pointBuffer = new CGPoint[maxObservations * 4];
            _classificationBuffer = new VisionClassification[maxObservations];
            _maxObservations = maxObservations;
        }

        /// <summary>
        /// Evaluates the provided pixel buffer for recognizable objects.
        /// </summary>
        /// <param name="buffer">Native pointer to the image data to evaluate.</param>
        /// <param name="dataType">The nature of the data buffer.</param>
        public void EvaluateBuffer(IntPtr buffer, ImageDataType dataType)
        {
            if (_requestsToPerform == VisionRequest.None)
            {
                Debug.LogError("[Vision] Unspecified vision request.");
                return;
            }

            if (_requestsInProgress != VisionRequest.None)
            {
                Debug.LogError("[Vision] One or more vision requests are still in progress.");
                return;
            }

            if (buffer == IntPtr.Zero)
            {
                Debug.LogError("[Vision] Pointer to buffer is null.");
                return;
            }

            bool success;
            switch (dataType)
            {
                case ImageDataType.MetalTexture:
                    success = _vision_evaluateWithTexture(buffer) > 0;
                    break;
                case ImageDataType.CoreVideoPixelBuffer:
                    success = _vision_evaluateWithBuffer(buffer) > 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("dataType", dataType, null);
            }

            if (success)
            {
                // Store requests in progress
                _requestsInProgress = _requestsToPerform;
            }
            else
            {
                Debug.LogError("[Vision] Unable to perform vision request. Pointer to buffer is not in expected type or is no longer accessible.");
            }
        }

        /// <summary>
        /// Invoked from native component when an object gets classified.
        /// </summary>
        /// <param name="error">Error message sent from the native component.</param>
        private void OnClassificationComplete(string error)
        {
            // Remove classification from the ongoing requests indicator
            _requestsInProgress &= ~VisionRequest.Classification;

            // Handle errors
            if (!string.IsNullOrEmpty(error))
            {
                if (error.Contains("Error") || error.Contains("error"))
                {
                    Debug.LogError(error);
                }
                else
                {
                    Debug.LogWarning(error);
                }

                // Since the message only represents errors, return if its not empty
                return;
            }

            // If anyone is interested in the results
            if (OnObjectClassified != null)
            {
                var length = _vision_acquireClassificationBuffer(_classificationBuffer, _maxObservations);
                if (length < 1) return;

                // Notify listeners
                OnObjectClassified(this, new ClassificationResultArgs(_classificationBuffer.Take(length).ToArray()));
            }
        }

        /// <summary>
        /// Invoked from native component when rectangles get recognized.
        /// </summary>
        /// <param name="error">Error message sent from the native component.</param>
        private void OnRectangleRecognitionComplete(string error)
        {
            // Remove rectangle detection from the ongoing requests indicator
            _requestsInProgress &= ~VisionRequest.RectangleDetection;

            // Handle errors
            if (!string.IsNullOrEmpty(error))
            {
                if (error.Contains("Error") || error.Contains("error"))
                {
                    Debug.LogError(error);
                }
                else
                {
                    Debug.LogWarning(error);
                }

                // Since the message only represents errors, return if its not empty
                return;
            }

            // If anyone is interested in the results
            if (OnRectanglesRecognized != null)
            {
                // Acquire resulting points
                var length = _vision_acquirePointBuffer(_pointBuffer);
                if (length < 4) return;

                var coordinates = _pointBuffer.Take(length).Select(point => (Vector2)point).ToArray();
                AlignScreenCoordinates(coordinates);
                
                // Notify listeners about the resulting points of the recognized rectangles
                OnRectanglesRecognized(this, new RectanglesRecognizedArgs(coordinates));
            }
        }
        
        /// <summary>
        /// Aligns the specified normalized screen coordinates to device orientation.
        /// The reference orientation is LandscapeLeft.
        /// </summary>
        private static void AlignScreenCoordinates(IList<Vector2> coordinates)
        {
            if (Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                for (var i = 0; i < coordinates.Count; i++)
                {
                    coordinates[i] = Vector2.one - coordinates[i];
                }
            }
            else if (Screen.orientation != ScreenOrientation.LandscapeLeft)
            {
                Debug.LogWarning("[Vision] Normalized screen coordinate alignment is not yet implemented for the current orientation.");
            }
        }
	}
}