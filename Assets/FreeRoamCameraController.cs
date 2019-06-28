using UnityEngine;

[RequireComponent(typeof(Camera))]
class FreeRoamCameraController : MonoBehaviour {
  public int speed = 5;
  public int rotationSpeed = 5;
  public float speedMultiplier = 10f;
  public bool invertY = false;
  public bool invertX = false;
  private Vector3 angles;

  void Start() {
    Cursor.visible = false;
    angles = transform.localEulerAngles;
  }

  void Update() {
    if (Input.GetKeyDown(KeyCode.Escape)) {
      Cursor.visible = true;
    } else if (Input.GetMouseButtonDown(0)) {
      Cursor.visible = false;
    }

    float speedMultiplier = 1.0f;
    if (Input.GetKey(KeyCode.LeftShift)) {
      speedMultiplier = this.speedMultiplier;
    }

    float horizontalSpeed = Input.GetAxis("Horizontal") * speed * speedMultiplier;
    float verticalSpeed = Input.GetAxis("Vertical") * speed * speedMultiplier;
    float turnX = Input.GetAxis("Mouse Y") * (invertY ? 1 : -1);
    float turnY = Input.GetAxis("Mouse X") * (invertX ? -1 : 1);

    angles.x = (angles.x + turnX) % 360;
    angles.y = (angles.y + turnY) % 360;
    angles.z = 0;
    transform.localEulerAngles = angles;

    transform.position = transform.position + transform.forward * verticalSpeed + transform.right * horizontalSpeed;


  }
}