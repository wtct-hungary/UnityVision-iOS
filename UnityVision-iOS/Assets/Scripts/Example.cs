using System.Linq;
using Possible.Vision;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This example shows how to perform real-time image classification using the video feed from the rear camera. 
/// </summary>
public class Example : MonoBehaviour
{
	[SerializeField] private Vision _vision;
	[SerializeField] private RawImage _image;
	[SerializeField] private Text _text;
	
	// We use Unity's WebCamTexture API to access image data from device camera.
	private WebCamTexture _webCamTexture;

	private void Awake()
	{
		// Allocate webcam texture
		_webCamTexture = new WebCamTexture(requestedWidth: 1280, requestedHeight: 720);
		
		// We'll display the camera image on a full screen texture overlay
		_image.texture = _webCamTexture;
		
		// We need to tell the Vision plugin what kind of requests do we want it to perform.
		// This call not only prepares the managed wrapper for the specified image requests,
		// but allocates VisionRequest objects on the native side. You only need to call this
		// method when you initialize your application, and later if you need to change the type
		// of request you want to perform.
		// When performing image image classification requests,
		// maxObservations refers to the number of guesses extracted, ordered by confidence.
		_vision.SetAndAllocateRequests(VisionRequest.Classification, maxObservations: 1);
	}

	private void OnEnable()
	{
		_vision.OnObjectClassified += Vision_OnObjectClassified;
	}

	private void OnDisable()
	{
		_vision.OnObjectClassified -= Vision_OnObjectClassified;
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
			_vision.EvaluateBuffer(
				buffer: _webCamTexture.GetNativeTexturePtr(), 
				dataType: ImageDataType.MetalTexture);
		}
	}
#endif
	
	private void Vision_OnObjectClassified(object sender, ClassificationResultArgs e)
	{
		// Display the top guess for the dominant object on the image
		_text.text = e.observations.First().identifier;
	}
}
