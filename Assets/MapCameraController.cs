using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MapCameraController : MonoBehaviour {

  public Material UnselectedMaterial;
  public Material SelectedMaterial;

  private GameObject _currentSelectedObject;
  public GameObject CurrentSelectedObject {

    get {
      return _currentSelectedObject;
    }
      
    set {
      if (_currentSelectedObject != null) {
        _currentSelectedObject.GetComponent<Renderer>().material = UnselectedMaterial;
      }
      _currentSelectedObject = value;
      _currentSelectedObject.GetComponent<Renderer>().material = SelectedMaterial;

      StaticStorage.CurrentSolarSystemSeed = CurrentSelectedSolarSystem.Seed;
      
    }
  }

  public SolarSystem CurrentSelectedSolarSystem {
    get {
      if (CurrentSelectedObject != null) {
        return _currentSelectedObject.transform.parent.GetComponent<SolarSystem>();
      }

      return null;
    }
  }

  public float rotationSpeed = 100f;
  public float lookAtSpeed = 0.5f;
  public float lookingDistance = 15f;
  public float moveTowardsSpeed = 5f;
	
	// Update is called once per frame
	void Update () {

    float horizontalSpeed = Input.GetAxis("Horizontal") * rotationSpeed;
    float verticalSpeed = Input.GetAxis("Vertical") * rotationSpeed;

    if (CurrentSelectedObject != null) {
      LookAtCurrentSelectedObject();
      MoveTowardCurrentSelectedObject();
      transform.RotateAround(_currentSelectedObject.transform.position, Vector3.up, -horizontalSpeed * Time.deltaTime);
      transform.RotateAround(_currentSelectedObject.transform.position, Vector3.left, -verticalSpeed * Time.deltaTime);
    }

    if (Input.GetMouseButtonDown(0)) {
      RaycastHit hit;
      if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100)) {
        CurrentSelectedObject = hit.collider.gameObject;
      }
    }
  }

  private void LookAtCurrentSelectedObject() {
    Vector3 targetDir = CurrentSelectedObject.transform.position - transform.position;

    // The step size is equal to speed times frame time.
    float step = lookAtSpeed * Time.deltaTime;

    Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);

    // Move our position a step closer to the target.
    transform.rotation = Quaternion.LookRotation(newDir);
  }

  private void MoveTowardCurrentSelectedObject() {
    transform.position = Vector3.Lerp(
        transform.position, 
        (transform.position - CurrentSelectedObject.transform.position).normalized
          * lookingDistance + CurrentSelectedObject.transform.position,
        moveTowardsSpeed * Time.deltaTime
    );
  }

  private void OnGUI() {
    if (CurrentSelectedSolarSystem != null) {
      GUI.Label(new Rect(0, 0, Screen.width, Screen.height), CurrentSelectedSolarSystem.Name + " has " + CurrentSelectedSolarSystem.NumberOfPlanets + " planets");
    }
  }
}
