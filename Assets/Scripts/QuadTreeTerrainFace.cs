using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class QuadTreeTerrainFace : MonoBehaviour {

  public int CurrentLevelOfDetail = 0;

  private MeshFilter _meshFilter = null;
  public MeshFilter MeshFilter {
    get {
      return LazyLoadComponent(ref _meshFilter);
    }
  }

  private MeshRenderer _meshRenderer = null;
  public MeshRenderer MeshRenderer {
    get {
      return LazyLoadComponent(ref _meshRenderer);
    }
  }

  private Mesh _mesh = null;
  public Mesh Mesh {
    get {
      if (_mesh == null) {
        _mesh = new Mesh();
        MeshFilter.sharedMesh = _mesh;
      }

      return _mesh;
    }
  }

  public ShapeGenerator shapeGenerator;
  public ColorSettings colorSettings;

  public QuadTreeTerrainFace ParentFace { get; set; }
  public QuadTreeTerrainFace NorthEast { get; private set; }
  public QuadTreeTerrainFace NorthWest { get; private set; }
  public QuadTreeTerrainFace SouthEast { get; private set; }
  public QuadTreeTerrainFace SouthWest { get; private set; }

  public TerrainFace CurrentFace { get; private set; }

  private bool isDivided = false;

  public void Subdivide() {
    if (isDivided) {
      return;
    }

    NorthEast = Create(transform, gameObject.name + " - North East", this);
    NorthWest = Create(transform, gameObject.name + " - North West", this);
    SouthEast = Create(transform, gameObject.name + " - South East", this);
    SouthWest = Create(transform, gameObject.name + " - South West", this);

    Vector3 axisA = CurrentFace.axisA / 2;
    Vector3 axisB = CurrentFace.axisB / 2;

    NorthEast.Initialize(
      shapeGenerator,
      colorSettings,
      CurrentFace.LocalUp + axisA - axisB,
      axisA,
      axisB,
      CurrentFace.Resolution
    );

    NorthWest.Initialize(
      shapeGenerator,
      colorSettings,
      CurrentFace.LocalUp - axisA - axisB,
      axisA,
      axisB,
      CurrentFace.Resolution
    );

    SouthEast.Initialize(
      shapeGenerator,
      colorSettings,
      CurrentFace.LocalUp + axisA + axisB,
      axisA,
      axisB,
      CurrentFace.Resolution
    );

    SouthWest.Initialize(
      shapeGenerator,
      colorSettings,
      CurrentFace.LocalUp - axisA + axisB,
      axisA,
      axisB,
      CurrentFace.Resolution
    );

    NorthEast.CurrentLevelOfDetail = CurrentLevelOfDetail + 1;
    NorthWest.CurrentLevelOfDetail = CurrentLevelOfDetail + 1;
    SouthEast.CurrentLevelOfDetail = CurrentLevelOfDetail + 1;
    SouthWest.CurrentLevelOfDetail = CurrentLevelOfDetail + 1;

    isDivided = true;
    MeshRenderer.enabled = false;
  }

  public void Merge() {
    if (isDivided) {
      Destroy(NorthEast.gameObject);
      Destroy(NorthWest.gameObject);
      Destroy(SouthEast.gameObject);
      Destroy(SouthWest.gameObject);

      MeshRenderer.enabled = true;
      isDivided = false;
    }
  }

  public void Initialize(ShapeGenerator shapeGenerator, ColorSettings colorSettings, Vector3 localUp, int resolution = 2) {
    this.shapeGenerator = shapeGenerator;
    this.colorSettings = colorSettings;
    CurrentFace = new TerrainFace(shapeGenerator, Mesh, resolution, localUp);
    MeshRenderer.sharedMaterial = colorSettings.planetMaterial;
  }

  public void Initialize(ShapeGenerator shapeGenerator, ColorSettings colorSettings, Vector3 localUp, Vector3 axisA, Vector3 axisB, int resolution = 2) {
    this.shapeGenerator = shapeGenerator;
    this.colorSettings = colorSettings;
    CurrentFace = new TerrainFace(shapeGenerator, Mesh, resolution, localUp, axisA, axisB);
    MeshRenderer.sharedMaterial = colorSettings.planetMaterial;
  }

  private T LazyLoadComponent<T>(ref T obj) where T : Component {
    if (obj == null) {
      obj = GetComponent<T>();
      if (obj == null) {
        obj = gameObject.AddComponent<T>();
      }
    }

    return obj;
  }

  private void RemoveIfNull<T>(ref T obj, T value) where T : Component {
    if (value == null) {
      Destroy(obj);
    }

    obj = value;
  }

  public static QuadTreeTerrainFace Create(Transform parentTransform, string name, QuadTreeTerrainFace parent = null) {
    GameObject quadTreeRoot = new GameObject(name);
    quadTreeRoot.transform.parent = parentTransform;
    quadTreeRoot.transform.position = Vector3.zero;

    QuadTreeTerrainFace face = quadTreeRoot.AddComponent<QuadTreeTerrainFace>();
    face.ParentFace = parent;
    return face;
  }

  public void ConstructMesh() {
    if (isDivided) {
      NorthEast.ConstructMesh();
      NorthWest.ConstructMesh();
      SouthEast.ConstructMesh();
      SouthWest.ConstructMesh();
    } else {
      CurrentFace.ConstructMesh();
    }
  }

  public QuadTreeTerrainFace FindClosestFace(Vector3 point) {

    if (isDivided) {
      var neClosestFace = NorthEast.FindClosestFace(point);
      var nwClosestFace = NorthWest.FindClosestFace(point);
      var seClosestFace = SouthEast.FindClosestFace(point);
      var swClosestFace = SouthWest.FindClosestFace(point);

      float neDistance = neClosestFace.DistanceTo(point);
      float nwDistance = nwClosestFace.DistanceTo(point);
      float seDistance = seClosestFace.DistanceTo(point);
      float swDistance = swClosestFace.DistanceTo(point);

      float minDistance = Mathf.Min(neDistance, nwDistance, seDistance, swDistance);

      if (neDistance == minDistance) {
        return neClosestFace;
      } else if (nwDistance == minDistance) {
        return nwClosestFace;
      } else if (seDistance == minDistance) {
        return seClosestFace;
      } else if (swDistance == minDistance) {
        return swClosestFace;
      }
    }

    return this;
  }

  public float DistanceTo(Vector3 point) {
    Bounds bounds = Mesh.bounds;
    return Vector3.Distance(bounds.ClosestPoint(point), point);
  }

  public void UpdateColors(ColorGenerator colorGenerator) {
    if (isDivided) {
      NorthEast.UpdateColors(colorGenerator);
      NorthWest.UpdateColors(colorGenerator);
      SouthEast.UpdateColors(colorGenerator);
      SouthWest.UpdateColors(colorGenerator);
      return;
    }

    CurrentFace.UpdateUVs(colorGenerator);
  }

  private void SetLevelOfDetail(int wantedLevelOfDetail, ref IList<QuadTreeTerrainFace> visitedNodes, int allowedDifference = 0) {
    if (visitedNodes.Contains(this)) {
      return;
    }

    wantedLevelOfDetail = Mathf.Max(0, wantedLevelOfDetail);
    int maxLevel = wantedLevelOfDetail + allowedDifference;
    int minLevel = wantedLevelOfDetail - allowedDifference;

    if (wantedLevelOfDetail > CurrentLevelOfDetail) {
      
      visitedNodes.Add(NorthEast);
      visitedNodes.Add(NorthWest);
      visitedNodes.Add(SouthEast);
      visitedNodes.Add(SouthWest);
    } else if (wantedLevelOfDetail < CurrentLevelOfDetail) {
      ParentFace.Merge();
      visitedNodes.Add(ParentFace);
    }

    visitedNodes.Add(this);

    foreach (QuadTreeTerrainFace neighbor in FindAllNeighbors()) {
      if (visitedNodes.Contains(neighbor)) {
        neighbor.SetLevelOfDetail(wantedLevelOfDetail - 1, ref visitedNodes);
      }
    }
  }

  private class LODNode {
    public QuadTreeTerrainFace Face;
    public int WantedLevelOfDetail;
    public LODNode(QuadTreeTerrainFace face, int wantedLevelOfDetail) {
      Face = face;
      WantedLevelOfDetail = wantedLevelOfDetail;
    }
  }
  public void SetLevelOfDetail(int wantedLevelOfDetail) {
    IList<QuadTreeTerrainFace> visited = new List<QuadTreeTerrainFace>();
    Queue<LODNode> neighbors = new Queue<LODNode>();
    neighbors.Enqueue(new LODNode(this, wantedLevelOfDetail));

    while (neighbors.Count > 0) {
      LODNode node = neighbors.Dequeue();
      if (visited.Contains(node.Face)) {
        continue;
      }
      visited.Add(node.Face);
      visited.Add(node.Face.ParentFace);

      foreach (QuadTreeTerrainFace face in node.Face.FindAllNeighbors()) {
        if (!visited.Contains(face.ParentFace)) {
          neighbors.Enqueue(new LODNode(face, wantedLevelOfDetail - 1));
        }
      }

      if (node.WantedLevelOfDetail > node.Face.CurrentLevelOfDetail) {
        node.Face.Subdivide();
      } else if (node.WantedLevelOfDetail < node.Face.CurrentLevelOfDetail) {
        node.Face.ParentFace.Merge();
      }
    }
  }

  public IList<QuadTreeTerrainFace> FindAllNeighbors() {
    return FindNortherNeighbors()
      .Union(FindSouthernNeighbors())
      .Union(FindEasternNeighbors())
      .Union(FindWesternNeighbors())
      .ToList();
  }

  #region NorthernNeighbors

  public IList<QuadTreeTerrainFace> FindNortherNeighbors() {
    QuadTreeTerrainFace largerNeighbor = FindNorthernNeighborOfGreaterOrEqualSize();
    IList<QuadTreeTerrainFace> neighbors = FindNorthernNeighborsOfSmallerSize(largerNeighbor);
    return neighbors;
  }

  public QuadTreeTerrainFace FindNorthernNeighborOfGreaterOrEqualSize() {
    if (ParentFace == null) {
      return null;
    }

    if (ParentFace.SouthWest == this) {
      return ParentFace.NorthWest;
    }

    if (ParentFace.SouthEast == this) {
      return ParentFace.NorthEast;
    }

    QuadTreeTerrainFace node = ParentFace.FindNorthernNeighborOfGreaterOrEqualSize();
    if (node == null || !node.isDivided) {
      return node;
    }

    return ParentFace.NorthWest == this ? node.SouthWest : node.SouthEast;
  }

  public IList<QuadTreeTerrainFace> FindNorthernNeighborsOfSmallerSize(QuadTreeTerrainFace neighbor) {
    IList<QuadTreeTerrainFace> candidates = new List<QuadTreeTerrainFace>();
    IList<QuadTreeTerrainFace> neighbors = new List<QuadTreeTerrainFace>();
    if (neighbor != null) {
      candidates.Add(neighbor);
    }

    while(candidates.Count > 0) {
      if (!candidates[0].isDivided) {
        neighbors.Add(candidates[0]);
      } else {
        candidates.Add(candidates[0].SouthWest);
        candidates.Add(candidates[0].SouthEast);
      }

      candidates.RemoveAt(0);
    }

    return neighbors;
  }

  #endregion

  #region SouthernNeighbors
  public IList<QuadTreeTerrainFace> FindSouthernNeighbors() {
    QuadTreeTerrainFace largerNeighbor = FindSouthernNeighborOfGreaterOrEqualSize();
    IList<QuadTreeTerrainFace> neighbors = FindSouthernNeighborsOfSmallerSize(largerNeighbor);
    return neighbors;
  }

  public QuadTreeTerrainFace FindSouthernNeighborOfGreaterOrEqualSize() {
    if (ParentFace == null) {
      return null;
    }

    if (ParentFace.NorthWest == this) {
      return ParentFace.SouthWest;
    }

    if (ParentFace.NorthEast == this) {
      return ParentFace.SouthEast;
    }

    QuadTreeTerrainFace node = ParentFace.FindSouthernNeighborOfGreaterOrEqualSize();
    if (node == null || !node.isDivided) {
      return node;
    }

    return ParentFace.SouthWest == this ? node.NorthWest : node.NorthEast;
  }

  public IList<QuadTreeTerrainFace> FindSouthernNeighborsOfSmallerSize(QuadTreeTerrainFace neighbor) {
    IList<QuadTreeTerrainFace> candidates = new List<QuadTreeTerrainFace>();
    IList<QuadTreeTerrainFace> neighbors = new List<QuadTreeTerrainFace>();
    if (neighbor != null) {
      candidates.Add(neighbor);
    }

    while (candidates.Count > 0) {
      if (!candidates[0].isDivided) {
        neighbors.Add(candidates[0]);
      } else {
        candidates.Add(candidates[0].NorthWest);
        candidates.Add(candidates[0].NorthEast);
      }

      candidates.RemoveAt(0);
    }

    return neighbors;
  }

  #endregion

  #region EasternNeighbors
  public IList<QuadTreeTerrainFace> FindEasternNeighbors() {
    QuadTreeTerrainFace largerNeighbor = FindEasternNeighborOfGreaterOrEqualSize();
    IList<QuadTreeTerrainFace> neighbors = FindEasternNeighborsOfSmallerSize(largerNeighbor);
    return neighbors;
  }

  public QuadTreeTerrainFace FindEasternNeighborOfGreaterOrEqualSize() {
    if (ParentFace == null) {
      return null;
    }

    if (ParentFace.NorthWest == this) {
      return ParentFace.NorthEast;
    }

    if (ParentFace.SouthWest == this) {
      return ParentFace.SouthEast;
    }

    QuadTreeTerrainFace node = ParentFace.FindEasternNeighborOfGreaterOrEqualSize();
    if (node == null || !node.isDivided) {
      return node;
    }

    return ParentFace.NorthWest == this ? node.NorthEast : node.SouthEast;
  }

  public IList<QuadTreeTerrainFace> FindEasternNeighborsOfSmallerSize(QuadTreeTerrainFace neighbor) {
    IList<QuadTreeTerrainFace> candidates = new List<QuadTreeTerrainFace>();
    IList<QuadTreeTerrainFace> neighbors = new List<QuadTreeTerrainFace>();
    if (neighbor != null) {
      candidates.Add(neighbor);
    }

    while (candidates.Count > 0) {
      if (!candidates[0].isDivided) {
        neighbors.Add(candidates[0]);
      } else {
        candidates.Add(candidates[0].NorthWest);
        candidates.Add(candidates[0].SouthWest);
      }

      candidates.RemoveAt(0);
    }

    return neighbors;
  }

  #endregion

  #region WesternNeighbors
  public IList<QuadTreeTerrainFace> FindWesternNeighbors() {
    QuadTreeTerrainFace largerNeighbor = FindWesternNeighborOfGreaterOrEqualSize();
    IList<QuadTreeTerrainFace> neighbors = FindWesternNeighborsOfSmallerSize(largerNeighbor);
    return neighbors;
  }

  public QuadTreeTerrainFace FindWesternNeighborOfGreaterOrEqualSize() {
    if (ParentFace == null) {
      return null;
    }

    if (ParentFace.NorthEast == this) {
      return ParentFace.NorthWest;
    }

    if (ParentFace.SouthEast == this) {
      return ParentFace.SouthWest;
    }

    QuadTreeTerrainFace node = ParentFace.FindWesternNeighborOfGreaterOrEqualSize();
    if (node == null || !node.isDivided) {
      return node;
    }

    return ParentFace.NorthEast == this ? node.NorthWest : node.SouthWest;
  }

  public IList<QuadTreeTerrainFace> FindWesternNeighborsOfSmallerSize(QuadTreeTerrainFace neighbor) {
    IList<QuadTreeTerrainFace> candidates = new List<QuadTreeTerrainFace>();
    IList<QuadTreeTerrainFace> neighbors = new List<QuadTreeTerrainFace>();
    if (neighbor != null) {
      candidates.Add(neighbor);
    }

    while (candidates.Count > 0) {
      if (!candidates[0].isDivided) {
        neighbors.Add(candidates[0]);
      } else {
        candidates.Add(candidates[0].NorthEast);
        candidates.Add(candidates[0].SouthEast);
      }

      candidates.RemoveAt(0);
    }

    return neighbors;
  }
  #endregion
}
