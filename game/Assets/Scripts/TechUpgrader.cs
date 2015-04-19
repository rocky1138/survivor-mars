using UnityEngine;
using UnityEngine.UI;

class TechUpgrader : MonoBehaviour {
  public TechnologyType tech;
  private Stockpile stockpile;
  private Technology technology;
  public UpgradeCost[] costs;
  public bool payable;
	int toast;

  void setStatus(bool active) {
    if (active) {
      // enable the button fully
    } else  {
      // disable the button such that it is not visible or clickable, but it's Update() still gets run
    }
  }

  void Start () {
    technology = GameObject.Find("/Technology").GetComponent<Technology>();
    stockpile = GameObject.Find("/Stockpile").GetComponent<Stockpile>();
    costs = GetComponents<UpgradeCost>();
    setStatus(false);
  }

  void Update () {
    payable = true;
    foreach (UpgradeCost cost in costs) {
      if (stockpile.stocks[cost.type] < cost.amount) {
        payable = false;
      }
    }

    setStatus(payable);
  }

  public void upgrade() {
    if (payable) {
      foreach (UpgradeCost cost in costs) {
        stockpile.updateStockLevel(cost.toResourceAmount(), true);
      }

      technology.Upgrade(tech);

      if (tech == TechnologyType.AlgaeFarm) {toast = 3;}
      if (tech == TechnologyType.SolarArray) {toast = 0;}
      GameObject.Find ("GameController").GetComponent<ToastNotifications> ().ToastNotification (toast);

    }
  }

  public void OnClicked(Button button)
  {
		button.gameObject.SetActive (false);
  }

}
