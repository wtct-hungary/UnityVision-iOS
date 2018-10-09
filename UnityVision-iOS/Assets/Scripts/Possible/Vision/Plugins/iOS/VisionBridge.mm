//
//  VisionBridge.mm
//  Vision
//
//  Created by Adam Hegedus on 2018. 05. 23..
//

#include "Vision-Swift.h"

#pragma mark - C interface

struct VisionClassificationType {
    
    float confidence;
    char* identifier;
};

char* safeStringCopy(const char* string) {
    
    if (string == NULL) return NULL;
    char* result = (char*)malloc(strlen(string)+1);
    strcpy(result, string);
    return result;
}

extern "C" {
    
    void _vision_allocateVisionRequests(int requestType, int maxObservations) {
        [[VisionNative shared] allocateVisionRequestsWithRequestType: requestType maxObservations: maxObservations];
    }
    
    void _vision_setCallbackTarget(const char* target) {
        [[VisionNative shared] setCallbackTargetWithTarget: [NSString stringWithUTF8String: target]];
    }
    
    int _vision_evaluateWithBuffer(CVPixelBufferRef buffer) {
        
        // In case of invalid buffer ref
        if (!buffer) return 0;
        
        // Forward message to the swift api
        return [[VisionNative shared] evaluateWithBuffer: buffer] ? 1 : 0;
    }
    
    int _vision_evaluateWithTexture(MTLTextureRef texture) {
        
        // In case of invalid texture ref
        if (!texture) return 0;
        
        // Forward message to the swift api
        return [[VisionNative shared] evaluateWithTexture: texture] ? 1 : 0;
    }
    
    int _vision_acquireClassificationBuffer(VisionClassificationType* pClassificationBuffer, int maxObservations) {
        
        // Get a handle to the buffer
        NSArray * array = [[VisionNative shared] classificationBuffer];
        
        // Cache count
        unsigned long count = [array count];
        if (maxObservations < count) count = maxObservations;
        
        // Extract buffer contents
        for (int i = 0; i < count; i++) {
            VisionClassification* observation = (VisionClassification*) [array objectAtIndex:i];
            pClassificationBuffer[i].confidence = observation.confidence;
            pClassificationBuffer[i].identifier = safeStringCopy([observation.identifier UTF8String]);
        }
        
        // Return with the number of extracted data
        return (int)count;
    }
    
    int _vision_acquirePointBuffer(CGPoint* pPointBuffer) {
        
        // Get a handle to the buffer
        NSArray * array = [[VisionNative shared] pointBuffer];
        
        // Cache count
        unsigned long count = [array count];
        
        // Extract buffer contents
        for (int i = 0; i < count; i++) {
            pPointBuffer[i] = [[array objectAtIndex:i] CGPointValue];
        }
        
        // Return with the number of extracted data
        return (int)count;
    }
    
    int _vision_allocateCVPixelBuffer(char* address, int width, int height,
                                      int format, CVPixelBufferRef* outPixelBufferPtr) {
        
        OSType pixelFormat = kCVPixelFormatType_32ARGB;
        int bytesPerRow = width * 4;
        
        if (format == 1) {
            pixelFormat = kCVPixelFormatType_24RGB;
            bytesPerRow = width * 3;
        }
        
        return CVPixelBufferCreateWithBytes(kCFAllocatorDefault, width, height, pixelFormat, address, bytesPerRow, NULL, NULL, NULL, outPixelBufferPtr);
    }
    
    void _vision_releaseCVPixelBuffer(CVPixelBufferRef pixelBuffer) {
        CVPixelBufferRelease(pixelBuffer);
    }
    
    void _vision_release(void* p) {
        free(p);
    }
}
