using UnityEngine;
using System.Collections;

public class DustStorm : MonoBehaviour {
	
	public GameObject buildingSettlement;
	public GameObject buildingSolarArray;
	public GameObject buildingAlgaeFarm;
	
	public float duration = 5.0f;
	public int damage = 1;

	private float start;
	private float finish;
	
	// Use this for initialization
	void OnEnable () {
		start = Time.time;
		finish = Time.time + duration;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > finish) {
			buildingSettlement.GetComponent<BuildingManager>().damage(damage);
			buildingSolarArray.GetComponent<BuildingManager>().damage(damage);
			buildingAlgaeFarm.GetComponent<BuildingManager>().damage(damage);
			gameObject.SetActive(false);
		}
	}
}
