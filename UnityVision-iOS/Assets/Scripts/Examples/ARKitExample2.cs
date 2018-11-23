///////////////////////////////////////////////////////////////////////////////
// ARKitExample2.cs
// 
// Author: Adam Hegedus
// Contact: adam.hegedus@possible.com
// Copyright © 2018 POSSIBLE CEE. Released under the MIT license.
///////////////////////////////////////////////////////////////////////////////

using System.Linq;
using Possible.Vision.Managed;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable once RedundantUsingDirective
using UnityEngine.XR.iOS;

namespace Examples
{
	/// <summary>
	/// This example demonstrates how to use the Vision and ARKit plugins together.
	/// </summary>
	public class ARKitExample2 : MonoBehaviour 
	{
		[SerializeField] private Vision _vision;
		[SerializeField] private Text _text;

		private void Awake()
		{
			// We need to tell the Vision plugin what kind of requests do we want it to perform.
			// This call not only prepares the managed wrapper for the specified image requests,
			// but allocates VNRequest objects on the native side. You only need to call this
			// method when you initialize your app, and later if you need to change the type
			// of requests you want to perform. When performing image classification requests,
			// maxObservations refers to the number of returned guesses, ordered by confidence.
			_vision.SetAndAllocateRequests(VisionRequest.Classification, maxObservations: 1);
		}

		private void OnEnable()
		{
			// Hook up to the completion event of object classification requests
			_vision.OnObjectClassified += Vision_OnObjectClassified;
		}

		private void OnDisable()
		{
			_vision.OnObjectClassified -= Vision_OnObjectClassified;
		}

#if !UNITY_EDITOR && UNITY_IOS
        private void Update()
        {
	        // We only classify a new image if no other vision requests are in progress
            if (_vision.InProgress)
            {
                return;
            }
            
	        // Instead of using ARKit's capturedImage (CVPixelBuffer), like we did in the first example,
	        // this time we use the Y plane of the YCbCr texture Unity uses to render the current camera frame.
            ARTextureHandles handles = UnityARSessionNativeInterface.GetARSessionNativeInterface().GetARVideoTextureHandles();
            if (handles.textureY != System.IntPtr.Zero)
            {
	            // This is the call where we pass in the handle to the metal texture to be analysed
	            _vision.EvaluateBuffer(handles.textureY, ImageDataType.MetalTexture);
            }   
        }
#endif
    
		private void Vision_OnObjectClassified(object sender, ClassificationResultArgs e)
		{
			// Display the top guess for the dominant object on the image
			_text.text = e.observations.First().identifier;
		}
	}
}