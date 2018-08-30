//
//  VisionNative.swift
//  Vision
//
//  Created by Adam Hegedus on 2018. 05. 23..
//

import Foundation
import AVFoundation
import Vision

@objc public class VisionNative: NSObject {
    
    // Shared instance
    @objc static let shared = VisionNative()
    
    // Used to cache vision requests to be performed
    private lazy var visionRequests = [VNRequest]()
    
    // Unique serial queue reserved for vision requests
    private let visionRequestQueue = DispatchQueue(label: "com.possible.unity.visionqueue")
    
    // Id of the managed Unity game object to forward messages to
    private var callbackTarget: String = "Vision"
    
    // Exposed buffer for caching the results of a barcode detection request
    @objc public var barcodeBuffer: [BarcodeObservation] = []
    private var maxBarcodeObservations: Int = 10
    
    // Exposed buffer for caching the results of a rectangle recognition request
    @objc public var pointBuffer: [CGPoint] = []
    
    @objc func allocateVisionRequests(requestType: Int, maxObservations: Int)
    {
        // Empty request buffer
        visionRequests.removeAll()
        
        if (requestType == 0) {
            print("[VisionNative] No requests specified.")
            return
        }
        
        let barcodeScanningEnabled = requestType != 2;
        let rectangleRecognitionEnabled = requestType != 1
        
        if barcodeScanningEnabled {
            
            // Set up barcode detection request
            let barcodeRequest = VNDetectBarcodesRequest(completionHandler:
                barcodeDetectionCompleteHandler)
            
            self.maxBarcodeObservations = maxObservations
            
            // Register request
            visionRequests.append(barcodeRequest)
        }
        
        if rectangleRecognitionEnabled {
            
            // Set up rectangle detection request
            let rectangleRequest = VNDetectRectanglesRequest(completionHandler: rectangleRecognitionCompleteHandler)
            rectangleRequest.maximumObservations = maxObservations
            rectangleRequest.quadratureTolerance = 15
            
            // Register request
            visionRequests.append(rectangleRequest)
            
            print("[VisionNative] Rectangle detection request allocated.")
        }
    }
    
    @objc func evaluate(texture: MTLTexture) -> Bool {
        
        // Create an image from the current state of the buffer
        guard let image = CIImage(mtlTexture: texture, options: nil) else { return false }
        
        // Perform vision request
        performVisionRequest(for: image)
        
        return true
    }
    
    @objc func evaluate(buffer: CVPixelBuffer) -> Bool {
        
        // Lock the buffer
        CVPixelBufferLockBaseAddress(buffer, CVPixelBufferLockFlags.readOnly)
        
        // Create an image from the current state of the buffer
        let image = CIImage(cvPixelBuffer: buffer)
        
        // Unlock the buffer
        CVPixelBufferUnlockBaseAddress(buffer, CVPixelBufferLockFlags.readOnly)
        
        // Perform vision request
        performVisionRequest(for: image)
        
        return true
    }
    
    private func performVisionRequest(for image: CIImage) {
        
        visionRequestQueue.async {
            // Prepare image request
            let imageRequestHandler = VNImageRequestHandler(ciImage: image, options: [:])
            
            // Run image request
            do {
                try imageRequestHandler.perform(self.visionRequests)
            } catch {
                print(error)
            }
        }
    }
    
    @objc func setCallbackTarget(target: String) {
        
        // Set the target for unity messaging
        self.callbackTarget = target
    }
    
    private func barcodeDetectionCompleteHandler(request: VNRequest, error: Error?) {
        
        // Fall back to main thread
        DispatchQueue.main.async {
            
            // Catch errors
            if error != nil {
                let error = "[VisionNative] Error: " + (error?.localizedDescription)!
                UnitySendMessage(self.callbackTarget, "OnBarcodeDetectionComplete", error)
                return
            }
            
            guard let observations = request.results as? [VNBarcodeObservation],
                let _ = observations.first else {
                    UnitySendMessage(self.callbackTarget, "OnBarcodeDetectionComplete", "No results")
                    return
            }
            
            // Cache results
            self.barcodeBuffer.removeAll()
            for observation in observations.prefix(self.maxBarcodeObservations) {
                if observation.payloadStringValue == nil {
                    continue
                }
                
                self.barcodeBuffer.append(
                    BarcodeObservation(
                        symbology: observation.symbology.rawValue,
                        payload: observation.payloadStringValue!))
            }
            
            // Call unity object with no errors
            UnitySendMessage(self.callbackTarget, "OnBarcodeDetectionComplete", "")
        }
    }
    
    private func rectangleRecognitionCompleteHandler(request: VNRequest, error: Error?) {
        
        // Fall back to main thread
        DispatchQueue.main.async {
            
            // Catch errors
            if error != nil {
                let error = "[VisionNative] Error: " + (error?.localizedDescription)!
                UnitySendMessage(self.callbackTarget, "OnRectangleRecognitionComplete", error)
                return
            }
            
            guard let observations = request.results as? [VNRectangleObservation],
                let _ = observations.first else {
                    UnitySendMessage(self.callbackTarget, "OnRectangleRecognitionComplete", "No results")
                    return
            }
            
            // Cache points
            self.pointBuffer.removeAll()
            for observation in observations {
                self.pointBuffer.append(contentsOf: [
                    observation.topLeft,
                    observation.topRight,
                    observation.bottomRight,
                    observation.bottomLeft])
            }
            
            // Call unity object with no errors
            UnitySendMessage(self.callbackTarget, "OnRectangleRecognitionComplete", "")
        }
    }
}
