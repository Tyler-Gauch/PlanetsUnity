using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class PlanetLevelOfDetail {

  [Range(0, float.MaxValue)]
  public float startDistance;
  [Range(2, 256)]
  public int resolution;

}
