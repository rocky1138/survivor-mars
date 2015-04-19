using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


class ResourceConverter : MonoBehaviour {
    public ResourceInput[] inputs;
    public ResourceOutput[] outputs;
    public float cycleTime;
    private float progress;
    private bool ready;
    public bool online;
    public float efficiency = 1;

    public Stockpile stockpile;

    public ResourceConverter(int cycleTime) {
      this.cycleTime = cycleTime;
      this.progress = 0;
      this.ready = false;
    }
    public ResourceConverter() {
       this.cycleTime = 0;
      this.progress = 0;
      this.ready = false;
    }

    void Start() {
      if (stockpile == null) {
        stockpile = GameObject.Find("/Stockpile").GetComponent<Stockpile>();
      }
      inputs = GetComponents<ResourceInput>();
      outputs = GetComponents<ResourceOutput>();
    }

    void Update() {
      if (!online) return;
      if (!ready) {
        ready = true;
        foreach(ResourceInput input in inputs) {
          if (stockpile.stocks[input.type] < input.amount) {
            ready = false;
          }
        }
        if (ready) {
          foreach(ResourceInput input in inputs) {
            stockpile.updateStockLevel(
              new ResourceAmount(input.type, input.amount), true);
          }
        }
      } else {
        progress += efficiency * Time.deltaTime;
        if (progress > cycleTime) {
          ready = false;
          progress = 0;

          foreach(ResourceOutput output in outputs) {
            stockpile.updateStockLevel(new ResourceAmount(output.type, output.amount));
          }
        }
      }
    }
  }

