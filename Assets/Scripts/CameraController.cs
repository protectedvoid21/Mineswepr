using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private new Camera _camera;
    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _zoomAction;
    private InputAction _sprintAction;

    public float speed;
    public float sprintFactor = 1.2f;

    public float zoomSpeed;

    [SerializeField]
    private float zoomFactor;

    private float _targetZoom;
    private float _velocity;

    [SerializeField]
    private int minClamp = 200;
    [SerializeField]
    private int maxClamp = 2000;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions.FindAction("MoveCamera");
        _zoomAction = _playerInput.actions.FindAction("Zoom");
        _sprintAction = _playerInput.actions.FindAction("Sprint");
        _targetZoom = _camera.orthographicSize;
    }

    private void Update()
    {
        Vector2 move = _moveAction.ReadValue<Vector2>();
        
        float sprintModifier = _sprintAction.ReadValue<float>() > 0 ? sprintFactor : 1;
        float scroll = _zoomAction.ReadValue<Vector2>().y * zoomSpeed;
        
        transform.Translate(new Vector3(move.x, move.y) * (speed * sprintModifier * Time.deltaTime));

        _targetZoom += scroll * zoomFactor;
        _targetZoom = Mathf.Clamp(_targetZoom, minClamp, maxClamp);
        _camera.orthographicSize =
            Mathf.SmoothDamp(_camera.orthographicSize, _targetZoom, ref _velocity, Time.deltaTime * zoomSpeed);
    }
}