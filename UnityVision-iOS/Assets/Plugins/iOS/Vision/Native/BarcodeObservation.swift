//
//  BarcodeObservation.swift
//  Unity-iPhone
//
//  Created by Adam Hegedus on 2018. 08. 30..
//

import Foundation

@objc public class BarcodeObservation : NSObject
{
    @objc public var symbology: String = ""
    @objc public var payload: String = ""
    
    public init(symbology: String, payload: String)
    {
        self.symbology = symbology
        self.payload = payload
    }
}
