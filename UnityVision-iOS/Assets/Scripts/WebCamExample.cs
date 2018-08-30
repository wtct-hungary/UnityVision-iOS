///////////////////////////////////////////////////////////////////////////////
// WebCamExample.cs
// 
// Author: Adam Hegedus
// Contact: adam.hegedus@possible.com
// Copyright © 2018 POSSIBLE CEE. Released under the MIT license.
///////////////////////////////////////////////////////////////////////////////

using System.Linq;
using Plugins.iOS.Vision.Managed;
using Possible.Vision;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This example shows how to perform real-time image classification, using the video feed from the rear camera. 
/// </summary>
public class WebCamExample : MonoBehaviour
{
	[SerializeField] private Vision _vision;
	[SerializeField] private RawImage _image;
	[SerializeField] private Text _text;
	
	// We use Unity's WebCamTexture API to access image data from device camera.
	private WebCamTexture _webCamTexture;

	private void Awake()
	{
		_webCamTexture = new WebCamTexture(requestedWidth: 1280, requestedHeight: 720);
		_image.texture = _webCamTexture;
		_vision.SetAndAllocateRequests(VisionRequest.BarcodeScanning, maxObservations: 1);
	}

	private void OnEnable()
	{
		_vision.OnBarcodesDetected += Vision_OnBarcodesDetected;
	}

	private void OnDisable()
	{
		_vision.OnBarcodesDetected -= Vision_OnBarcodesDetected;
	}

	private void Start()
	{
		_webCamTexture.Play();
	}

#if !UNITY_EDITOR && UNITY_IOS
	private void Update()
	{
		if (!_vision.InProgress)
		{
			_vision.EvaluateBuffer(_webCamTexture.GetNativeTexturePtr(), ImageDataType.MetalTexture);
		}
	}
#endif
	
	private void Vision_OnBarcodesDetected(object sender, BarcodesDetectedArgs e)
	{
		var barcode = e.barcodes.First();
		Debug.Log("Symbology: " + barcode.symbology);
		Debug.Log("Payload: " + barcode.payload);
	}
}
