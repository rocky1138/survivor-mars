using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


class ResourceConverter : MonoBehaviour {
    public ResourceInput[] inputs;
    public ResourceOutput[] outputs;
    public int cycleTime;
    private float progress;
    private bool ready;
    public bool online;

    public Stockpile stockpile;

    ResourceConverter(int cycleTime) {
      this.cycleTime = cycleTime;
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
            stockpile.updateStockLevel(input, true);
          }
        }
      } else {
        progress += Time.deltaTime;
        if (progress > cycleTime) {
          ready = false;
          progress = 0;

          foreach(ResourceOutput output in outputs) {
            stockpile.updateStockLevel(output);
          }
        }
      }
    }
  }

