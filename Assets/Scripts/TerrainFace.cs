using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TerrainFace {

  public Mesh Mesh { get; private set; }
  public int Resolution { get; set; }
  public Vector3 LocalUp { get; private set; }
  
  public Vector3 axisA { get; private set; }
  public Vector3 axisB { get; private set; }
  public ShapeGenerator ShapeGenerator { get; private set; }

  public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp) {
    Mesh = mesh;
    Resolution = resolution;
    LocalUp = localUp;
    ShapeGenerator = shapeGenerator;

    axisA = new Vector3(LocalUp.y, LocalUp.z, LocalUp.x);
    axisB = Vector3.Cross(LocalUp, axisA);
  }

  public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp, Vector3 axisA, Vector3 axisB)
    : this(shapeGenerator, mesh, resolution, localUp)
  {
    this.axisA = axisA;
    this.axisB = axisB;
  }

  public void ConstructMesh() {
    Vector3[] verticies = new Vector3[Resolution * Resolution];
    int[] triangles = new int[(Resolution - 1) * (Resolution - 1) * 6];
    int triIndex = 0;
    Vector2[] uv = Mesh.uv;

    for (int y = 0; y < Resolution; y++) {
      for (int x = 0; x < Resolution; x++) {

        int i = x + y * Resolution;
        Vector2 percent = new Vector2(x, y) / (Resolution - 1);
        Vector3 pointOnUnitCube = (LocalUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB);
        Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
        verticies[i] = ShapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);

        if (x != Resolution-1 && y != Resolution-1) {
          triangles[triIndex] = i;
          triangles[triIndex+1] = i + Resolution + 1;
          triangles[triIndex+2] = i + Resolution;

          triangles[triIndex+3] = i;
          triangles[triIndex + 4] = i + 1;
          triangles[triIndex + 5] = i + Resolution + 1;
          triIndex += 6;
        }
      }
    }

    Mesh.Clear();
    Mesh.vertices = verticies;
    Mesh.triangles = triangles;
    Mesh.RecalculateNormals();
    Mesh.uv = uv;
  }

  public void UpdateUVs(ColorGenerator colorGenerator) {
    Vector2[] uv = new Vector2[Resolution * Resolution];

    for (int y = 0; y < Resolution; y++) {
      for (int x = 0; x < Resolution; x++) {
        int i = x + y * Resolution;
        Vector2 percent = new Vector2(x, y) / (Resolution - 1);
        Vector3 pointOnUnitCube = LocalUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB;
        Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

        uv[i] = new Vector2(colorGenerator.BiomePercentFromPoint(pointOnUnitSphere), 0);
      }
    }

    Mesh.uv = uv;
  }
}
