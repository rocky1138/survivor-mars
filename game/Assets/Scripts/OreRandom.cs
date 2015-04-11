using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OreRandom : MonoBehaviour {

	public int chanceOfDisablePercent = 10;

	void Start () {
	
		foreach (Transform child in transform) {
			if (Random.Range(1, 101) <= chanceOfDisablePercent &&
				child.gameObject.tag == "Mining-Ore") {
					
				child.gameObject.SetActive(false);
				
			}
		}
	
	}
	
	void Update () {
	
	}
}
