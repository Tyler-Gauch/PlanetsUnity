using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class SimpleNoiseFilter : INoiseFilter {

  Noise noise;
  NoiseSettings.SimpleNoiseSettings settings;

  public SimpleNoiseFilter(int seed, NoiseSettings.SimpleNoiseSettings settings) {
    this.settings = settings;
    noise = new Noise(seed);
  }

  public float Evaluate(Vector3 point) {
    float noiseValue = 0;
    float frequency = settings.baseRoughness;
    float amplitude = 1;

    for (int i = 0; i < settings.numLayers; i++) {
      float v = noise.Evaluate(point * frequency + settings.center);
      noiseValue += (v + 1) * .5f * amplitude;
      frequency *= settings.roughness;
      amplitude *= settings.persistance;
    }

    noiseValue = Mathf.Max(0, noiseValue - settings.minValue);
    return noiseValue * settings.strength;
  }

}
