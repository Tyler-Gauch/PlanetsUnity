using UnityEngine;

[RequireComponent(typeof(SolarSystem))]
public class SolarSystemManager : MonoBehaviour {

  private SolarSystem solarSystem;

	// Use this for initialization
	void Start () {
    solarSystem = GetComponent<SolarSystem>();
    solarSystem.Init(StaticStorage.CurrentSolarSystemSeed);

    solarSystem.CreateStar();
    solarSystem.BuildPlanets();

    Camera.main.transform.position = (Camera.main.transform.position - solarSystem.Star.gameObject.transform.position).normalized
          * 10000 + solarSystem.Star.gameObject.transform.position;
  }

  private void OnGUI() {
    GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "Current Solar System: " + solarSystem.Name);
  }
}
