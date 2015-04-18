using UnityEngine;
using System.Collections;

public class Peril : MonoBehaviour {
	public float likelihood = 0.1f;
	public float frequency = 10f;
	public GameObject dustStorm;
	public GameObject asteroid;
	public AudioClip warning;
	public GameObject[] targets;
	public GameObject GameController;
	int warningNum;

	// Use this for initialization
	void Start () {
		InvokeRepeating("Perils", frequency, frequency);
	}

	
	// Update is called once per frame
	void Perils () {
		int peril = 0;
			if (Random.value > 1.0f - likelihood) {
				peril = Random.Range(0, 5);
				if (peril == 0 && !dustStorm.activeSelf) {
					Debug.Log("Duststorm begins!");
					dustStorm.SetActive(true);
					audio.PlayOneShot(warning);
					warningNum = 0;
				}
				if (peril == 1 && !dustStorm.activeSelf) {
					Debug.Log("Asteroid begins!");
					//asteroid.SetActive(true);
					audio.PlayOneShot(warning);
					//Instantiate(asteroid);
					Debug.Log("Asteroids");
					GameObject target = targets[Random.Range(0, targets.Length)];
					Instantiate(asteroid, new Vector3(target.transform.position.x, target.transform.position.y + 20, target.transform.position.z), transform.rotation);
					warningNum = 1;
				}
			GameController.GetComponent<ToastNotifications>().WarnNotifications(warningNum);
			}
	}
}
