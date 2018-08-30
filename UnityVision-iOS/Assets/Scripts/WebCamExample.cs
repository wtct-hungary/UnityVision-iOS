///////////////////////////////////////////////////////////////////////////////
// WebCamExample.cs
// 
// Author: Adam Hegedus
// Contact: adam.hegedus@possible.com
// Copyright © 2018 POSSIBLE CEE. Released under the MIT license.
///////////////////////////////////////////////////////////////////////////////

using System.Linq;
using Plugins.iOS.Vision.Managed;
using UnityEngine;
using UnityEngine.UI;

public class WebCamExample : MonoBehaviour
{
	[SerializeField] private Vision _vision;
	[SerializeField] private RawImage _image;
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
