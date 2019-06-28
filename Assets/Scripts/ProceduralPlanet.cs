using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralPlanet : MonoBehaviour {

  [Range(2, 256)]
  public int resolution = 10;
  public bool autoupdate = true;
  public enum FaceRenderMask { All, Top, Bottom, Left, Right, Front, Back};
  public FaceRenderMask faceRenderMask;
  public int seed;
  public float subdivisionDistance = 7000f;
  public float consecutiveSubdivisionDistance = 1000f;

  public PlanetLevelOfDetail[] levelsOfDetail;

  public ShapeSettings shapeSettings;
  public ColorSettings colorSettings;

  [HideInInspector]
  public bool shapeSettingsFoldout;
  [HideInInspector]
  public bool colorSettingsFoldout;

  ShapeGenerator shapeGenerator = new ShapeGenerator();
  ColorGenerator colorGenerator = new ColorGenerator();

  [SerializeField, HideInInspector]
  QuadTreeTerrainFace[] quadTreeTerrainFaces;

  private float distanceSinceLastSubdivision = float.MaxValue;
  private QuadTreeTerrainFace lastClosestFace = null;
  private int currentResolution = 16;
  private int lastLevelOfDetail = -1;

  private void Start() {
    GeneratePlanet();
  }

  private void Update() {
    // find the face that the camera is closest to
    float closestDistance = float.MaxValue;
    QuadTreeTerrainFace closestFace = null;
    QuadTreeTerrainFace parentFace = null;
    Vector3 position = Camera.main.transform.position;
    foreach (QuadTreeTerrainFace face in quadTreeTerrainFaces) {
      QuadTreeTerrainFace currentClosestFace = face.FindClosestFace(position);
      float faceDistance = currentClosestFace.DistanceTo(position);
      if (faceDistance <= closestDistance) {
        closestFace = currentClosestFace;
        parentFace = face;
        closestDistance = faceDistance;
      }
    }

    //foreach (QuadTreeTerrainFace face in quadTreeTerrainFaces) {
    //  face.gameObject.SetActive(face == closestFace);
    //}

    Debug.Log(closestDistance);
    Debug.DrawLine(Camera.main.transform.position, closestFace.Mesh.bounds.ClosestPoint(Camera.main.transform.position), Color.red);

    int currentLevelOfDetail = 0;
    for (int i = 0; i < levelsOfDetail.Length; i++) {
      if (closestDistance < levelsOfDetail[i].startDistance) {
        currentLevelOfDetail = i;
      }
    }

    closestFace.SetLevelOfDetail(currentLevelOfDetail);

    GenerateMesh();
    GenerateColors();
  }

  public void Initialize() {
    shapeGenerator.UpdateSettings(seed, shapeSettings);
    colorGenerator.UpdateSettings(seed, colorSettings);

    if (quadTreeTerrainFaces == null || quadTreeTerrainFaces.Length == 0) {
      quadTreeTerrainFaces = new QuadTreeTerrainFace[6];
    }

    Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

    for (int i = 0; i < 6; i++) {
      if (quadTreeTerrainFaces[i] == null) {
        quadTreeTerrainFaces[i] = QuadTreeTerrainFace.Create(transform, "Mesh - " + i);
      }
      quadTreeTerrainFaces[i].Initialize(shapeGenerator, colorSettings, directions[i], resolution);

      bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i;
      quadTreeTerrainFaces[i].gameObject.SetActive(renderFace);
    }
  }

  public void GeneratePlanet() {
    Initialize();
    GenerateMesh();
    GenerateColors();
  }

  public void OnShapeSettingsUpdated() {
    if (autoupdate) {
      Initialize();
      GenerateMesh();
    }
  }

  public void OnColorSettisngUpdated() {
    if (autoupdate) {
      Initialize();
      GenerateColors();
    }
  }

  void GenerateMesh() {
    for (int i = 0; i < quadTreeTerrainFaces.Length; i++) {
      if (quadTreeTerrainFaces[i].gameObject.activeSelf) {
        quadTreeTerrainFaces[i].ConstructMesh();
      }
    }

    colorGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
  }

  void GenerateColors() {
    colorGenerator.UpdateColors();

    for (int i = 0; i < quadTreeTerrainFaces.Length; i++) {
      if (quadTreeTerrainFaces[i].gameObject.activeSelf) {
        quadTreeTerrainFaces[i].UpdateColors(colorGenerator);
      }
    }
  }
}
