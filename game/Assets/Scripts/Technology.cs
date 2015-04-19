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
  public void upgrade() {
    if (current < max) {
      current += 1 ;
    } else {
      current = max;
    }
  }
}

class Technology : MonoBehaviour {
  public Dictionary<TechnologyType, TechLevel> techLevels = new Dictionary<TechnologyType, TechLevel> {
    {TechnologyType.AlgaeFarm, new TechLevel(TechnologyType.AlgaeFarm, 0, 1)},
    {TechnologyType.SolarArray, new TechLevel(TechnologyType.SolarArray, 0, 1)},
    {TechnologyType.MiningLaser, new TechLevel(TechnologyType.MiningLaser, 0, 1)}
  };

  public void Upgrade(TechnologyType tech) {
    techLevels[tech].upgrade();
  }

  void Start() {

  }

  void Update() {

  }
}
