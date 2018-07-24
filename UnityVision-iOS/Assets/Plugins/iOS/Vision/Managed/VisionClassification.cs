using System.Runtime.InteropServices;

namespace Possible.Vision
{
    /// <summary>
    /// Represents an image classification result.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VisionClassification
    {
        public float confidence;
        public string identifier;
    }
}