using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu]
public class ShapeSettings : ScriptableObject {

  public float planetRadius = 1;
  public NoiseLayer[] noiseLayers;

  [Serializable]
  public class NoiseLayer {
    public bool enabled = true;
    public bool useFirstLayerAsMask = false;
    public NoiseSettings noiseSettings;
  }
}
