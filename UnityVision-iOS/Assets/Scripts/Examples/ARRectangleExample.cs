///////////////////////////////////////////////////////////////////////////////
// ARRectangleExample.cs
// 
// Author: Adam Hegedus
// Contact: adam.hegedus@possible.com
// Copyright © 2018 POSSIBLE CEE. Released under the MIT license.
///////////////////////////////////////////////////////////////////////////////

using System.Linq;
using Possible.Vision.Managed;
using UnityEngine;
using UnityEngine.XR.iOS;
using Utils;

namespace Examples
{
	/// <summary>
	/// This example demonstrates how to cast a recognized rectangle from screen to an ARKit surface.
	/// </summary>
	public class ARRectangleExample : MonoBehaviour 
	{
		[SerializeField] private Vision _vision;
		[SerializeField] private RectangleMarker _rectangleMarkerPrefab;
	
		private RectangleMarker _marker;

		private void Awake()
		{
			// We need to tell the Vision plugin what kind of requests do we want it to perform.
			// This call not only prepares the managed wrapper for the specified image requests,
			// but allocates VNRequest objects on the native side. You only need to call this
			// method when you initialize your app, and later if you need to change the type
			// of requests you want to perform. When performing rectangle detection requests,
			// maxObservations refers to the maximum number of rectangles allowed to be recognized at once.
			_vision.SetAndAllocateRequests(VisionRequest.RectangleDetection, maxObservations: 1);
		}

		private void OnEnable()
		{
			// Hook up to the completion event of rectangle detection requests
			_vision.OnRectanglesRecognized += Vision_OnRectanglesRecognized;
		
			// Hook up to ARKit's frame update callback to be able to get a handle to the latest pixel buffer
			UnityARSessionNativeInterface.ARFrameUpdatedEvent += UnityARSessionNativeInterface_ARFrameUpdatedEvent;
		}

		private void OnDisable()
		{
			_vision.OnRectanglesRecognized -= Vision_OnRectanglesRecognized;
			UnityARSessionNativeInterface.ARFrameUpdatedEvent -= UnityARSessionNativeInterface_ARFrameUpdatedEvent;
		}
	
		private void UnityARSessionNativeInterface_ARFrameUpdatedEvent(UnityARCamera unityArCamera)
		{
#if !UNITY_EDITOR && UNITY_IOS
// Evaluate the current state of ARKit's pixel buffer for recognizable objects
// We only classify a new image if no other vision requests are in progress
        if (!_vision.InProgress)
        {
            // This is the call where we pass in the handle to the image data to be analysed
            _vision.EvaluateBuffer(
                
                // This argument is always of type IntPtr, that refers the data buffer
                buffer: unityArCamera.videoParams.cvPixelBufferPtr,
                
                // We need to tell the plugin about the nature of the underlying data.
                // The plugin only supports CVPixelBuffer (CoreVideo) and MTLTexture (Metal).
                // The ARKit plugin uses CoreVideo to capture images from the device camera.
                dataType: ImageDataType.CoreVideoPixelBuffer);
        }
#endif
		}
	
		private void Vision_OnRectanglesRecognized(object sender, RectanglesRecognizedArgs e)
		{
			var rectangles = e.rectangles.OrderByDescending(entry => entry.area).ToList();
			var found = false;

			foreach (var rect in rectangles)
			{
				ARHitTest.CastRectangle(rect, onHit: (topLeft, topRight, bottomRight, bottomLeft) =>
				{
					if (_marker == null)
					{
						_marker = Instantiate(_rectangleMarkerPrefab);
						Debug.Assert(_marker != null, "Could not instantiate rectangle marker prefab.");
					
						// Reset transform
						_marker.transform.position = Vector3.zero;
						_marker.transform.rotation = Quaternion.identity;
						_marker.transform.localScale = Vector3.one;
					}

					// Assign the corners of the marker game object to the surface hit points
					_marker.TopLeft = topLeft;
					_marker.TopRight = topRight;
					_marker.BottomRight = bottomRight;
					_marker.BottomLeft = bottomLeft;

					// Closure is synchronous
					found = true;
				});

				if (found)
				{
					break;
				}
			}

			if (_marker != null)
			{
				// Hide the marker if no rectangles were found
				_marker.gameObject.SetActive(found);
			}
		}
	}
}
