using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class NoiseFilterFactory {

  public static INoiseFilter CreateNoiseFilter(NoiseSettings settings, int seed = 0) {
    switch (settings.filterType) {
      case NoiseSettings.FilterType.Simple:
        return new SimpleNoiseFilter(seed, settings.simpleNoiseSettings);
      case NoiseSettings.FilterType.Rigid:
        return new RigidNoiseFilter(seed, settings.rigidNoiseSettings);
      default:
        return null;
    }
  }
}
