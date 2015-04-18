using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public enum ResourceType {
  /*Food, BioWaste, H2O, WasteWater,*/ O2, CO2, /*H2, Power,*/ Ore, Metal, Silicates, Ice
}

public class ResourceAmount {
  public ResourceType type;
  public int amount;
  public ResourceAmount(ResourceType t, int a) {
    type = t;
    amount = a;
  }
}


public class Resource : MonoBehaviour {
  public static Dictionary<ResourceType, string> niceNames = new Dictionary<ResourceType, string> {
    //{ResourceType.Food, "Food"},
    //{ResourceType.BioWaste, "Bio Waste"},
    //{ResourceType.H2O, "H2O"},
    //{ResourceType.WasteWater, "Waste Water"},
    {ResourceType.O2, "O2"},
    {ResourceType.CO2, "CO2"},
    //{ResourceType.H2, "H2"},
    //{ResourceType.Power, "Power"},
    {ResourceType.Ore, "Ore"},
    {ResourceType.Metal, "Metal"},
    {ResourceType.Silicates, "Silicates"},
    {ResourceType.Ice, "Ice"}};
  public ResourceType type;
  public int amount;
}

