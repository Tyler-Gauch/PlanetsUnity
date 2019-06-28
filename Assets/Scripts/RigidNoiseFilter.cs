using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidNoiseFilter : INoiseFilter {

  Noise noise;
  NoiseSettings.RigidNoiseSettings settings;

  public RigidNoiseFilter(int seed, NoiseSettings.RigidNoiseSettings settings) {
    this.settings = settings;
    noise = new Noise(seed);
  }

  public float Evaluate(Vector3 point) {
    float noiseValue = 0;
    float frequency = settings.baseRoughness;
    float amplitude = 1;
    float weight = 1;

    for (int i = 0; i < settings.numLayers; i++) {
      float v = 1-Mathf.Abs(noise.Evaluate(point * frequency + settings.center));
      v *= v;
      v *= weight;
      weight = Mathf.Clamp01(v * settings.weightMultiplier);

      noiseValue += v * amplitude;
      frequency *= settings.roughness;
      amplitude *= settings.persistance;
    }

    noiseValue = Mathf.Max(0, noiseValue - settings.minValue);
    return noiseValue * settings.strength;
  }
}
