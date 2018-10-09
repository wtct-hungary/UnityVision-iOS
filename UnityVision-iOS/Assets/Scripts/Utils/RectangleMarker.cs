///////////////////////////////////////////////////////////////////////////////
// RectangleMarker.cs
// 
// Author: Adam Hegedus
// Contact: adam.hegedus@possible.com
// Copyright © 2018 POSSIBLE CEE. Released under the MIT license.
///////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace Utils
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class RectangleMarker : MonoBehaviour
    {
        [SerializeField] private float _lerpSpeed = 10.0f;
        [SerializeField] private TextMesh[] _sizeTexts;

        private Mesh _mesh;
        private MeshFilter _meshFilter;

        private Vector3 _topLeft, _topRight, _bottomRight, _bottomLeft;
        private Vector3 _targetTopLeft, _targetTopRight, _targetBottomRight, _targetBottomLeft;

        public Vector3 TopLeft 
        {
            set { _targetTopLeft = value; } 
        }

        public Vector3 TopRight
        {
            set { _targetTopRight = value; }
        }

        public Vector3 BottomRight
        {
            set { _targetBottomRight = value; }
        }

        public Vector3 BottomLeft
        {
            set { _targetBottomLeft = value; }
        }

        private void Awake()
        {
            // Acquire required components
            _meshFilter = GetComponent<MeshFilter>();
            _mesh = new Mesh();

            // Defaults
            TopLeft = Vector3.forward * 0.1f + Vector3.left * 0.1f;
            TopRight = Vector3.forward * 0.1f + Vector3.right * 0.1f;
            BottomRight = Vector3.back * 0.1f + Vector3.right * 0.1f;
            BottomLeft = Vector3.back * 0.1f + Vector3.left * 0.1f;
        }

        private void Update()
        {
            _topLeft = Vector3.Lerp(_topLeft, _targetTopLeft, Time.deltaTime * _lerpSpeed);
            _topRight = Vector3.Lerp(_topRight, _targetTopRight, Time.deltaTime * _lerpSpeed);
            _bottomRight = Vector3.Lerp(_bottomRight, _targetBottomRight, Time.deltaTime * _lerpSpeed);
            _bottomLeft = Vector3.Lerp(_bottomLeft, _targetBottomLeft, Time.deltaTime * _lerpSpeed);

            _mesh.Clear();
            _mesh.vertices = new[]
            {
                _topLeft, _bottomLeft, 
                _bottomRight, _topRight,
            };

            _mesh.uv = new[]
            {
                Vector2.up, Vector2.zero,
                Vector2.right, Vector2.up + Vector2.right
            };

            _mesh.triangles = new[]
            {
                2, 1, 0,
                0, 3, 2
            };

            _meshFilter.mesh = _mesh;

            // Top
            _sizeTexts[0].transform.position = (_topLeft + _topRight) / 2.0f + Vector3.up * 0.1f;
            _sizeTexts[0].transform.localRotation = Quaternion.Euler(90.0f, -Vector3.SignedAngle(_topRight - _topLeft, Vector3.right, Vector3.up), 0.0f);
            _sizeTexts[0].text = (_topRight - _topLeft).magnitude * 100.0f + " cm";

            // Bottom
            _sizeTexts[1].transform.position = (_bottomRight + _bottomLeft) / 2.0f + Vector3.up * 0.1f;
            _sizeTexts[1].transform.localRotation = Quaternion.Euler(90.0f, -Vector3.SignedAngle(_bottomRight - _bottomLeft, Vector3.right, Vector3.up), 0.0f);
            _sizeTexts[1].text = (_bottomRight - _bottomLeft).magnitude * 100.0f + " cm";

            // Left
            _sizeTexts[2].transform.position = (_bottomLeft + _topLeft) / 2.0f + Vector3.up * 0.1f;
            _sizeTexts[2].transform.localRotation = Quaternion.Euler(90.0f, -90.0f - Vector3.SignedAngle(_topLeft - _bottomLeft, Vector3.forward, Vector3.up), 0.0f);
            _sizeTexts[2].text = (_topLeft - _bottomLeft).magnitude * 100.0f + " cm";

            // Right
            _sizeTexts[3].transform.position = (_topRight + _bottomRight) / 2.0f + Vector3.up * 0.1f;
            _sizeTexts[3].transform.localRotation = Quaternion.Euler(90.0f, -90.0f - Vector3.SignedAngle(_topRight - _bottomRight, Vector3.forward, Vector3.up), 0.0f);
            _sizeTexts[3].text = (_topRight - _bottomRight).magnitude * 100.0f + " cm";
        }
    }
}