///////////////////////////////////////////////////////////////////////////////
// CoreVideoExample.cs
// 
// Author: Adam Hegedus
// Contact: adam.hegedus@possible.com
// Copyright © 2018 POSSIBLE CEE. Released under the MIT license.
///////////////////////////////////////////////////////////////////////////////

using System.Linq;
using Possible.Vision.Managed;
using Possible.Vision.Managed.CoreVideo;
using UnityEngine;
using UnityEngine.UI;

namespace Examples
{
	/// <summary>
	/// This example demonstrates how to create native CVPixelBuffer objects from Unity textures.
	/// </summary>
	public class CoreVideoExample : MonoBehaviour 
	{
		[SerializeField] private Vision _vision;
		[SerializeField] private RawImage _image;
		[SerializeField] private Text _text;
		[SerializeField] private Texture2D _imageToClassify;
		
		// Reference to the managed CVPixelBuffer wrapper object.
		// The actual object will be allocated using the appropriate factory method.
		private CVPixelBuffer _cvPixelBuffer;

		private void Awake()
		{
			// Display the target image
			_image.texture = _imageToClassify;
		
			// We need to tell the Vision plugin what kind of requests do we want it to perform.
			// This call not only prepares the Vision instance for the specified image requests,
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
		private void Start()
		{
			var allocationResult = CVPixelBuffer.TryCreate(fromTexture: _imageToClassify, result: out _cvPixelBuffer);
			if (allocationResult == CVReturn.Success)
			{
				_vision.EvaluateBuffer(_cvPixelBuffer.GetNativePtr(), ImageDataType.CoreVideoPixelBuffer);
			}
			else
			{
				Debug.LogError("Could not allocate CVPixelBuffer (" + allocationResult + ")");
			}
		}
#endif

		private void Vision_OnObjectClassified(object sender, ClassificationResultArgs e)
		{
			// Display the top guess for the dominant object on the image
			_text.text = e.observations.First().identifier;
		
			// Release the native object
			_cvPixelBuffer.Dispose();
		}
	}
}
