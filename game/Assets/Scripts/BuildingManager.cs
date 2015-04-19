using UnityEngine;

class BuildingManager : MonoBehaviour {
  public ResourceConverter converter;
  private Technology technology;
  public float condition = 1;
  public int maxHP = 100;
  public int HP = 100;
  public int currentLevel = 0;
  public TechnologyType tech;

  public float[] techBonuses;

  void updateConverter() {
    Debug.Log(currentLevel);
    converter.efficiency = (1 + techBonuses[currentLevel]) * condition;
  }

  void checkTech() {
    if (currentLevel != technology.techLevels[tech].current) {
      currentLevel = technology.techLevels[tech].current;
      updateConverter();
    }
  }

  void updateCondition() {
    if (HP > maxHP) {
      HP = maxHP;
    }

    if (HP < 0) {
      HP = 0;
      // BOOM!
    }
    condition = ((float)HP / (float)maxHP);
  }

  public void damage(int magnitude) {
    this.HP -= magnitude;
    updateCondition();
    updateConverter();
  }

  public void repair(int magnitude) {
    this.HP += magnitude;
    updateCondition();
    updateConverter();
  }

  void Start () {
    if (technology == null) {
      technology = GameObject.Find("/Technology").GetComponent<Technology>();
    }

    updateCondition();
    checkTech();
  }

  void Update() {
    updateCondition();
    checkTech();
  }
}
