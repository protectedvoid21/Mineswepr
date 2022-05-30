using UnityEngine;

public class CameraController : MonoBehaviour {
    private new Camera camera;
    
    public float speed;
    public float sprintFactor = 1.2f;
    [SerializeField] private KeyCode sprintKeyCode = KeyCode.LeftShift;
    public float zoomSpeed;
    [SerializeField] private float zoomFactor;
    private float targetZoom;
    private float velocity;

    [SerializeField] private int minClamp = 200;
    [SerializeField] private int maxClamp = 2000;

    private void Awake() {
        camera = GetComponent<Camera>();
        targetZoom = camera.orthographicSize;
    }

    private void Update() {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float sprintModifier = Input.GetKey(sprintKeyCode) ? sprintFactor : 1;
        float scroll = -Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

        transform.Translate(new Vector3(x, y) * (speed * sprintModifier * Time.deltaTime));

        targetZoom += scroll * zoomFactor;
        targetZoom = Mathf.Clamp(targetZoom, minClamp, maxClamp);
        camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, targetZoom, ref velocity, Time.deltaTime * zoomSpeed);
    }
}