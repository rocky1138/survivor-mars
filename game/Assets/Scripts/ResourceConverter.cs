using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MarsEndeavour {
  class ResourceConverter : MonoBehaviour {
    public Dictionary<ResourceType, int> inputs;
    public Dictionary<ResourceType, int> outputs;
    public int cycleTime;
    private float progress;
    private bool ready;

    public Stockpile stockpile;

    ResourceConverter(Dictionary<ResourceType, int> inputs, Dictionary<ResourceType, int> outputs, int cycleTime) {
      this.inputs = inputs;
      this.outputs = outputs;
      this.cycleTime = cycleTime;
      this.progress = 0;
      this.ready = false;
    }

    void Start() {
      stockpile = GameObject.Find("/ResourceController").GetComponent<Stockpile>();
    }

    void Update() {
      // check that stockpile has enough inputs
      if (!ready) {
        ready = true;
        foreach(KeyValuePair<ResourceType, int> entry in inputs) {
          if (stockpile.stocks[entry.Key] < inputs[entry.Key]) {
            ready = false;
          }
        }
        if (ready) {
          foreach(KeyValuePair<ResourceType, int> entry in inputs) {
            stockpile.updateStockLevel(entry);
          }
        }

      } else {
        progress += Time.deltaTime;
        if (progress > cycleTime) {
          ready = false;
          progress = 0;

          foreach(KeyValuePair<ResourceType, int> entry in outputs) {
            stockpile.updateStockLevel(entry);
          }
        }
      }
    }
  }
}
