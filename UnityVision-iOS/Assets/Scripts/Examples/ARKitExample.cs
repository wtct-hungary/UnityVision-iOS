///////////////////////////////////////////////////////////////////////////////
// ARKitExample.cs
// 
// Author: Adam Hegedus
// Contact: adam.hegedus@possible.com
// Copyright © 2018 POSSIBLE CEE. Released under the MIT license.
///////////////////////////////////////////////////////////////////////////////

using System.Linq;
using Possible.Vision.Managed;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.iOS;

namespace Examples
{
    /// <summary>
    /// This example demonstrates how to use the Vision and ARKit plugins together.
    /// </summary>
    public class ARKitExample : MonoBehaviour
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
            // Hook up to ARKit's frame update callback to be able to get a handle to the latest pixel buffer
            UnityARSessionNativeInterface.ARFrameUpdatedEvent += UnityARSessionNativeInterface_ARFrameUpdatedEvent;
        
            // Hook up to the completion event of object classification requests
            _vision.OnObjectClassified += Vision_OnObjectClassified;
        }

        private void OnDisable()
        {
            UnityARSessionNativeInterface.ARFrameUpdatedEvent -= UnityARSessionNativeInterface_ARFrameUpdatedEvent;
            _vision.OnObjectClassified -= Vision_OnObjectClassified;
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
    
        private void Vision_OnObjectClassified(object sender, ClassificationResultArgs e)
        {
            // Display the top guess for the dominant object on the image
            _text.text = e.observations.First().identifier;
        }
    }
}
