using UnityEngine;
using System.Collections;

public class Peril : MonoBehaviour {
	public float likelihood = 0.1f;
	public GameObject dustStorm;
	public GameObject asteroid;
	public AudioClip warning;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		int peril = 0;
		if (Random.value > 1.0f - likelihood) {
			peril = Random.Range(0, 5);
			if (peril == 0 && !dustStorm.activeSelf) {
				Debug.Log("Duststorm begins!");
				dustStorm.SetActive(true);
				audio.PlayOneShot(warning);
			}
			if (peril == 1 && !dustStorm.activeSelf) {
				Debug.Log("Asteroid begins!");
				//asteroid.SetActive(true);
				audio.PlayOneShot(warning);
				Instantiate(asteroid);
			}
		}
	}
}
