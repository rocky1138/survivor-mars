using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum TechnologyType {
  AlgaeFarm, SolarArray, MiningLaser
}

class TechLevel {
  public TechnologyType type;
  public int current;
  public int max;
  public TechLevel(TechnologyType t, int c, int m) {
    type = t;
    current = c;
    max = m;
  }
  public void Upgrade(int level) {
    if (level <= max) {
      current = level;
    } else {
      current = max;
    }
  }
}

class Technology : MonoBehaviour {
  public Dictionary<TechnologyType, TechLevel> techLevels = new Dictionary<TechnologyType, TechLevel> {
    {TechnologyType.AlgaeFarm, new TechLevel(TechnologyType.AlgaeFarm, 1, 2)},
    {TechnologyType.SolarArray, new TechLevel(TechnologyType.SolarArray, 1, 2)},
    {TechnologyType.MiningLaser, new TechLevel(TechnologyType.MiningLaser, 1, 2)}
  };

  public void Upgrade(TechnologyType tech, int level) {
    techLevels[tech].Upgrade(level);
  }

  void Start() {

  }

  void Update() {

  }
}
