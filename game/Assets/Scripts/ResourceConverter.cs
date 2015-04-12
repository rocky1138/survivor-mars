using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MarsEndeavour {
  class ResourceConverter : MonoBehaviour {
    public Input[] inputs;
    public Output[] outputs;
    public int cycleTime;
    private float progress;
    private bool ready;
    public bool online;

    public Stockpile stockpile;

    ResourceConverter(int cycleTime) {
      this.inputs = inputs;
      this.outputs = outputs;
      this.cycleTime = cycleTime;
      this.progress = 0;
      this.ready = false;
    }

    void Start() {
      stockpile = GameObject.Find("/ResourceController").GetComponent<Stockpile>();
      inputs = GetComponents<Input>();
      outputs = GetComponents<Output>();
    }

    void Update() {
      if (!online) return;
      if (!ready) {
        ready = true;
        foreach(Input input in inputs) {
          if (stockpile.stocks[input.type] < input.amount) {
            ready = false;
          }
        }
        if (ready) {
          foreach(Input input in inputs) {
            stockpile.updateStockLevel(input, true);
          }
        }
      } else {
        progress += Time.deltaTime;
        if (progress > cycleTime) {
          ready = false;
          progress = 0;

          foreach(Output output in outputs) {
            stockpile.updateStockLevel(output);
          }
        }
      }
    }
  }
}
