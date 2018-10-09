///////////////////////////////////////////////////////////////////////////////
// ImageDataType.cs
// 
// Author: Adam Hegedus
// Contact: adam.hegedus@possible.com
// Copyright © 2018 POSSIBLE CEE. Released under the MIT license.
///////////////////////////////////////////////////////////////////////////////

namespace Possible.Vision.Managed
{
	public enum ImageDataType 
	{
		/// <summary>
		/// Use this value whenever you need to analyze image data stored in a texture (using Metal graphics api).
		/// https://docs.unity3d.com/ScriptReference/Texture.GetNativeTexturePtr.html
		/// </summary>
		MetalTexture,
		
		/// <summary>
		/// Use this value if need to anlyze image data stored in a native CVPixelBuffer.
		/// https://developer.apple.com/documentation/corevideo/cvpixelbuffer
		/// </summary>
		CoreVideoPixelBuffer
	}
}