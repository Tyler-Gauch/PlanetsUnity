using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu]
public class ColorSettings : ScriptableObject {

  public Material planetMaterial;
  public BiomeColorSettings biomeColorSettings;

  [Serializable]
  public class BiomeColorSettings {

    public Biome[] biomes;
    public NoiseSettings noiseSettings;
    public float noiseOffset;
    public float noiseStrength;
    [Range(0, 1)]
    public float blendAmount;

    [Serializable]
    public class Biome {
      public Gradient gradient;
      public Color tint;
      [Range(0,1)]
      public float startHeight;
      [Range(0,1)]
      public float tintPercent;
    }
  }
}
