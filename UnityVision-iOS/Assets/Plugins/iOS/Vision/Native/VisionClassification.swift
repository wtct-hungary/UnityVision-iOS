//
//  VisionClassification.swift
//  Vision
//
//  Created by Adam Hegedus on 2018. 06. 15..
//

import Foundation

@objc public class VisionClassification : NSObject
{
    @objc public var identifier : String = ""
    @objc public var confidence : Float = 0
    
    public init(identifier: String, confidence: Float)
    {
        self.identifier = identifier
        self.confidence = confidence
    }
}
