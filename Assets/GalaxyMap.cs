using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GalaxyMap : MonoBehaviour {

  public int NumberOfSolarSystems = 10;

  public int MinDistanceBetweenSystems = 4;

  private int Seed = CurrentTimeEpoch();

  public float GalaxySize = 100;

  public List<GameObject> SolarSystemPrefabs;

  private IList<GameObject> _solarSystems = new List<GameObject>();

  protected void Start() {
    PlaceSolarSystems(GetSolarSystemLocations());
    MapCameraController mainCamera = Camera.main.GetComponent<MapCameraController>();
    mainCamera.CurrentSelectedObject = _solarSystems.First().GetComponent<SolarSystem>().Star.gameObject;
  }

  private IList<Vector3> GetSolarSystemLocations() {
    IList<Vector3> currentLocations = new List<Vector3>();
    Random.InitState(Seed);

    while (currentLocations.Count < NumberOfSolarSystems) {
      Vector3 newPosition = new Vector3(
        Random.Range(0.0f, GalaxySize),
        Random.Range(0.0f, GalaxySize),
        Random.Range(0.0f, GalaxySize)
      );
      
      if (CanPlaceSolarSystem(newPosition, currentLocations)) {
        currentLocations.Add(newPosition);
      }
    }

    return currentLocations;
  } 

  private bool CanPlaceSolarSystem(Vector3 location, IList<Vector3> currentLocations) {
    foreach (Vector3 currentLocation in currentLocations) {
      if (Vector3.Distance(currentLocation, location) < MinDistanceBetweenSystems) {
        return false;
      }
    }

    return true;
  }

  private void PlaceSolarSystems(IList<Vector3> starLocations) {
    foreach (Vector3 location in starLocations) {
      GameObject solarSystem = Instantiate(
          SolarSystemPrefabs[Random.Range(0, SolarSystemPrefabs.Count)],
          location,
          Quaternion.identity,
          transform
      );

      SolarSystem solarSystemComponent = solarSystem.GetComponent<SolarSystem>();
      solarSystemComponent.Seed = Seed + (_solarSystems.Count + 1 * NumberOfSolarSystems);
      solarSystemComponent.CreateStar();

      _solarSystems.Add(solarSystem);
    }
  }

  private static int CurrentTimeEpoch() {
    System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
    return (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
  }
}
