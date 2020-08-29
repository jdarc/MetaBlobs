using Microsoft.Xna.Framework;

namespace MetaBlobs {
    public sealed class Camera {
        private Vector3 _position;
        private Vector3 _target;
        private Vector3 _upVector;
        private float _fov;
        private float _aspectRatio;
        private float _near;
        private float _far;

        public Vector3 Position {
            get => _position;
            set => _position = value;
        }

        public Vector3 Target {
            get => _target;
            set => _target = value;
        }

        public Vector3 Up {
            get => _upVector;
            set => _upVector = value;
        }

        public float FieldOfView {
            get => _fov;
            set => _fov = value;
        }

        public float AspectRatio {
            get => _aspectRatio;
            set => _aspectRatio = value;
        }

        public float NearPlane {
            get => _near;
            set => _near = value;
        }

        public float FarPlane {
            get => _far;
            set => _far = value;
        }

        public Camera(Vector3 position, Vector3 target, Vector3 upVector, float fov, float aspectRatio, float near, float far) {
            _position = position;
            _target = target;
            _upVector = upVector;
            _fov = fov;
            _aspectRatio = aspectRatio;
            _near = near;
            _far = far;
        }

        public Matrix View => Matrix.CreateLookAt(_position, _target, _upVector);

        public Matrix Projection => Matrix.CreatePerspectiveFieldOfView(_fov, _aspectRatio, _near, _far);
    }
}
