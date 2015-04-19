using UnityEngine;

class TechUpgrader : MonoBehaviour {
  public TechnologyType tech;
  private Technology technology;

  void Start () {
    if (technology == null) {
      technology = GameObject.Find("/Technology").GetComponent<Technology>();
    }
  }

  public void upgrade() {
    technology.Upgrade(tech);
  }
}
