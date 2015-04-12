using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Randomly disables ore in an attempt to make the game more replayable.
 * Each time you play, the cave is different!
 */
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
