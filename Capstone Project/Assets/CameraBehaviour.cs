using UnityEngine;
using System.Collections;

    /// <summary>
    /// Add this class to your main camera to have it zoom in or out, follow the player, etc...
    /// </summary>
    public class CameraBehaviour : MonoBehaviour
    {
        [Header("Zoom level and position")]
        /// the minimum possible position for the camera
        public Vector3 MinimumZoom;
        /// the minimum ortho zoom
        public float MinimumZoomOrthographic = 3f;
        /// the maximum possible position for the camera
        public Vector3 MaximumZoom;
        /// the maximum ortho zoom
        public float MaximumZoomOrthographic = 5f;
        [Space(10)]
        [Header("Following the player")]
        public Transform target;
        /// true if the camera should follow the player on the X axis (not recommended for most infinite runners.
        public bool FollowsPlayerX = false;
        /// true if the camera should follow the player on the y axis
        public bool FollowsPlayerY = true;
        /// true if the camera should follow the player on the z axis
        public bool FollowsPlayerZ = false;
        public float CameraSpeed;

        /// bounds
        [Space(10)]
        [Header("Bounds")]
        /// if true, the camera won't be able to escape its bounding box
        public bool CameraCanOnlyMoveWithinBounds = true;
        /// the position of the bottom left corner of the camera's bounding box
        public Vector2 BottomLeftBounds;
        /// the position of the top right corner of the camera's bounding box
        public Vector2 TopRightBounds;

        /// initial position, size, camera component and player's position
        protected Vector3 _initialPosition;
        protected float _initialSize;
        protected Camera _camera;
        protected Vector3 _playerPosition;

        protected Vector3 _currentZoomOffset;

        protected float _xMin, _xMax, _yMin, _yMax;
        protected Vector3 _newCameraPosition;

        protected Vector2 _topRightBounds;
        protected Vector2 _bottomLeftBounds;

        protected Vector2 _boundsTopRight;
        protected Vector2 _boundsBottomRight;
        protected Vector2 _boundsBottomLeft;
        protected Vector2 _boundsTopLeft;

        protected float _shakeIntensity;
        protected float _shakeDecay;
        protected float _shakeDuration;

        /// <summary>
        /// Triggers initialization.
        /// </summary>
        protected virtual void Start()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the components of the camera and its various private variables
        /// </summary>
        protected virtual void Initialize()
        {
            // we get the camera component, store the camera's initial position and size and player's position.
            _camera = GetComponent<Camera>();
            _initialPosition = transform.position;
            _initialSize = MinimumZoomOrthographic;
            _playerPosition = target.position;

            // we set up the camera's size and position based on the inspector set variables.
            _camera.orthographicSize = _initialSize;
            _camera.transform.position = MinimumZoom;
        }

        /// <summary>
        /// Every frame
        /// </summary>
        protected virtual void Update()
        {
            // pause management, if the game is paused we don't try moving the camera
            if (GameManager.gm != null)
            {
                if (GameManager.gm.paused)
                    return;
            }

            // if the camera's not supposed to follow the player on one of its axis, we reset these values.
            if (!FollowsPlayerX) { _playerPosition.x = 0; }
            if (!FollowsPlayerY) { _playerPosition.y = 0; }
            if (!FollowsPlayerZ) { _playerPosition.z = 0; }

            // we move the camera position and zoom from min to max, proportionately to the level's current speed
            _camera.orthographicSize = MinimumZoomOrthographic + (MaximumZoomOrthographic - MinimumZoomOrthographic) * (target.GetComponent<CharacterController2D>().moveSpeed);
            _currentZoomOffset = MinimumZoom + (MaximumZoom - MinimumZoom) * (target.GetComponent<CharacterController2D>().moveSpeed);

            // we get the bounds 
            GetLevelBounds();
            // we interpolate the camera's position towards the new player's position, taking into account the maximum zoom.
            _newCameraPosition = Vector3.Lerp(transform.position, _currentZoomOffset + _playerPosition, CameraSpeed * Time.deltaTime);

            // we constrain the camera within its bounds if required
            float posX = _newCameraPosition.x;
            float posY = _newCameraPosition.y;
            float posZ = _newCameraPosition.z;
            if (CameraCanOnlyMoveWithinBounds)
            {
                posX = Mathf.Clamp(_newCameraPosition.x, _xMin, _xMax);
                posY = Mathf.Clamp(_newCameraPosition.y, _yMin, _yMax);
            }

            // we handle the potential shake of the camera
            Vector3 shakeFactorPosition = Vector3.zero;
            // If shakeDuration is still running.
            if (_shakeDuration > 0)
            {
                shakeFactorPosition = Random.insideUnitSphere * _shakeIntensity * _shakeDuration;
                _shakeDuration -= _shakeDecay * Time.deltaTime;
            }

            // we apply the new position to the camera, actually moving it
            _camera.transform.position = new Vector3(posX, posY, posZ) + shakeFactorPosition;
        }

        /// <summary>
        /// Use this method to shake the camera, passing in a Vector3 for intensity, duration and decay
        /// </summary>
        /// <param name="shakeParameters">Shake parameters : intensity, duration and decay.</param>
        public virtual void Shake(Vector3 shakeParameters)
        {
            // the shake method just sets a few variables that will be used in the Update(), actually shaking the camera.
            _shakeIntensity = shakeParameters.x;
            _shakeDuration = shakeParameters.y;
            _shakeDecay = shakeParameters.z;
        }

        /// <summary>
        /// Resets the camera to its initial position and size.
        /// </summary>
        public virtual void ResetCamera()
        {
            transform.position = _initialPosition;
            _camera.orthographicSize = _initialSize;
        }

        /// <summary>
        /// Gets the levelbounds coordinates to lock the camera into the level
        /// </summary>
        protected virtual void GetLevelBounds()
        {
            // camera size calculation (orthographicSize is half the height of what the camera sees.
            float cameraHeight = Camera.main.orthographicSize * 2f;
            float cameraWidth = cameraHeight * Camera.main.aspect;
            // we define the coordinates of each corner of the camera's rectangle
            _xMin = BottomLeftBounds.x + (cameraWidth / 2);
            _xMax = TopRightBounds.x - (cameraWidth / 2);
            _yMin = BottomLeftBounds.y + (cameraHeight / 2);
            _yMax = TopRightBounds.y - (cameraHeight / 2);
        }

        /// <summary>
        /// When the camera is selected in scene view
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            _boundsTopRight = new Vector2(TopRightBounds.x, TopRightBounds.y);
            _boundsBottomRight = new Vector2(TopRightBounds.x, BottomLeftBounds.y);
            _boundsBottomLeft = new Vector2(BottomLeftBounds.x, BottomLeftBounds.y);
            _boundsTopLeft = new Vector2(BottomLeftBounds.x, TopRightBounds.y);

            // we draw a blue rectangle to show the camera's bounding box
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_boundsTopRight, _boundsBottomRight);
            Gizmos.DrawLine(_boundsBottomRight, _boundsBottomLeft);
            Gizmos.DrawLine(_boundsBottomLeft, _boundsTopLeft);
            Gizmos.DrawLine(_boundsTopLeft, _boundsTopRight);
        }

    }
