using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystem : MonoBehaviour {

  public Star Star { get; private set; }

  public List<GameObject> PlanetPrefabs;

  public List<GameObject> StarPrefabs;

  public int MaxNumberOfPlanets = 10;

  public Vector2 DistanceBetweenPlanets = new Vector2(1f, 4f);

  public int NumberOfPlanets;

  public int Seed;

  private IList<GameObject> _planets;

  public string Name;

  public List<string> Adjectives;
  public List<string> Nouns;

  public void Init(int seed) {
    Seed = seed;
    Random.InitState(seed);
    NumberOfPlanets = Random.Range(1, MaxNumberOfPlanets);
    Name = Adjectives[Random.Range(0, Adjectives.Count)] + " " + Nouns[Random.Range(0, Nouns.Count)];
  }

  public void BuildPlanets() {
    _planets = new List<GameObject>(NumberOfPlanets);
    float distanceFromPreviousObject;
    Vector3 lastLocation = Star.gameObject.transform.position;
    for (int i = 0; i < NumberOfPlanets; i++) {

      distanceFromPreviousObject = Random.Range(DistanceBetweenPlanets.x, DistanceBetweenPlanets.y);

      lastLocation += (Vector3.right * distanceFromPreviousObject);

      GameObject planet = Instantiate(
        PlanetPrefabs[Random.Range(0, PlanetPrefabs.Count)],
        lastLocation,
        Quaternion.identity,
        transform
      );

      Planet planetComponent = planet.GetComponent<Planet>();
      planet.transform.RotateAround(Star.transform.position, Star.transform.up, planetComponent.Speed);
      planetComponent.Star = Star.gameObject;

      _planets.Add(planet);
    }
  }

  public void CreateStar() {
    GameObject star = Instantiate(
      StarPrefabs[Random.Range(0, StarPrefabs.Count)],
      transform.position,
      Quaternion.identity,
      transform
    );

    Star = star.GetComponent<Star>();
  }
}
