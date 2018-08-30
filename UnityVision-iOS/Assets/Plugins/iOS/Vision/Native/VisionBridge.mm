//
//  VisionBridge.mm
//  Vision
//
//  Created by Adam Hegedus on 2018. 05. 23..
//

#include "Vision-Swift.h"

#pragma mark - C interface

struct BarcodeObservationType {
    
    char* symbology;
    char* payload;
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
    
    int _vision_acquireBarcodeBuffer(BarcodeObservationType* pBarcodeBuffer) {
        
        // Get a handle to the buffer
        NSArray * array = [[VisionNative shared] barcodeBuffer];
        
        // Cache count
        unsigned long count = [array count];
        
        // Extract buffer contents
        for (int i = 0; i < count; i++) {
            BarcodeObservation* observation = (BarcodeObservation*) [array objectAtIndex: i];
            pBarcodeBuffer[i].symbology = safeStringCopy([observation.symbology UTF8String]);
            pBarcodeBuffer[i].payload = safeStringCopy([observation.payload UTF8String]);
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
}
