using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MarsEndeavour {
  public class Stockpile : MonoBehaviour {
    public Dictionary<ResourceType, int> stocks;
    public Dictionary<ResourceType, int> maxima;
    public Text allStocks;

    Stockpile() {
      stocks = new Dictionary<ResourceType, int>();
      maxima = new Dictionary<ResourceType, int>();
    }

    private void initializeResource(ResourceType type, int stock, int max) {
      maxima.Add(type, max);
      stocks.Add(type, stock);
    }

    // Use this for initialization
    void Start () {
    //  allStocks = new Text("");
      initializeResource(ResourceType.Food, 1000, 1000);
      initializeResource(ResourceType.BioWaste, 0, 100);
      initializeResource(ResourceType.H2O, 250, 250);
      initializeResource(ResourceType.WasteWater, 0, 150);
      initializeResource(ResourceType.O2, 200, 200);
      initializeResource(ResourceType.CO2, 0, 200);
      initializeResource(ResourceType.H2, 0, 500);
      initializeResource(ResourceType.Power, 1500, 1500);
      initializeResource(ResourceType.Ore, 0, 200);
      initializeResource(ResourceType.Metal, 0, 100);
      initializeResource(ResourceType.Silicates, 0, 100);
      initializeResource(ResourceType.Ice, 0, 200);
    }

    void Update() {
      string stockString = "";
      foreach (ResourceType value in Enum.GetValues(typeof(ResourceType))) {
        Debug.Log("this" + value);
        stockString += " " + Resource.niceNames[value] + ":" + stocks[value];
      }
      allStocks.text = stockString;
    }

    public int updateStockLevel(KeyValuePair<ResourceType, int> delta) {
      int newStock = stocks[delta.Key] + delta.Value;
      if (newStock >= 0) {
        if (newStock <= maxima[delta.Key]) {
          stocks[delta.Key] = newStock;
        } else {
          stocks[delta.Key] = maxima[delta.Key];
        }
      }

      return newStock;
    }
  }
}
