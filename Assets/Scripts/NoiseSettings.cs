using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class NoiseSettings {

  public enum FilterType { Simple, Rigid };
  public FilterType filterType;

  [ConditionalHide("filterType", 0)]
  public SimpleNoiseSettings simpleNoiseSettings;
  [ConditionalHide("filterType", 1)]
  public RigidNoiseSettings rigidNoiseSettings;

  [Serializable]
  public class SimpleNoiseSettings {
    public float strength = 1;
    public float roughness = 2;
    public Vector3 center;
    [Range(1, 8)]
    public int numLayers;
    public float persistance = 0.5f;
    public float baseRoughness = 1;
    public float minValue = 1;
  }

  [Serializable]
  public class RigidNoiseSettings : SimpleNoiseSettings {
    public float weightMultiplier = 0.8f;
  }
}
