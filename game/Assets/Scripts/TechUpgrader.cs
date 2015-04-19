using UnityEngine;
using UnityEngine.UI;



class TechUpgrader : MonoBehaviour {
  public TechnologyType tech;
  private Technology technology;
	int toast;

  void Start () {
    if (technology == null) {
      technology = GameObject.Find("/Technology").GetComponent<Technology>();
    }
  }

  public void upgrade() {
    technology.Upgrade(tech);

	//Ugly I know
		if (tech.ToString() == "AlgaeFarm") {toast = 3;}
		if (tech.ToString() == "SolarArray") {toast = 0;}
		GameObject.Find ("GameController").GetComponent<ToastNotifications> ().ToastNotification (toast);
		//if (tech == "AlgaeFarm") {toast = 10;}
  }

  public void OnClicked(Button button)
  {
		button.gameObject.SetActive (false);
  }
	
}
