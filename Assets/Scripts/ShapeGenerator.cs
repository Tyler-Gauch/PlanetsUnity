using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ShapeGenerator {
  ShapeSettings settings;
  INoiseFilter[] noiseFilters;
  public MinMax elevationMinMax;

  public void UpdateSettings(int seed, ShapeSettings settings) {
    this.settings = settings;
    elevationMinMax = new MinMax();
    noiseFilters = new INoiseFilter[settings.noiseLayers.Length];
    for (int i = 0; i < noiseFilters.Length; i++) {
      noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings, seed);
    }
  }

  public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere) {
    float elevation = 0;
    float firstLayerValue = 0;

    if (noiseFilters.Length > 0) {
      firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere);
      if (settings.noiseLayers[0].enabled) {
        elevation = firstLayerValue;
      }
    }

    for (int i = 0; i < noiseFilters.Length; i++) {
      if (settings.noiseLayers[i].enabled) {
        float mask = (settings.noiseLayers[i].useFirstLayerAsMask) ? firstLayerValue : 1;
        elevation += noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
      }
    }

    elevation = settings.planetRadius * (1 + elevation);
    elevationMinMax.AddValue(elevation);
    return pointOnUnitSphere * elevation;
  }
}
