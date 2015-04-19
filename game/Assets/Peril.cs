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
	public int asteroidDamage = 30;
	
	private GameObject target;
	
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
				GameController.GetComponent<ToastNotifications>().WarnNotifications(warningNum);
			}
			if (peril == 1 && !dustStorm.activeSelf) {
				
				Debug.Log("Asteroid begins!");
				
				audio.PlayOneShot(warning);
				
				target = targets[Random.Range(0, targets.Length - 1)];
				
				Debug.Log("Peril target: " + target);
				
				Instantiate(asteroid, new Vector3(target.transform.position.x, target.transform.position.y + 20, target.transform.position.z), transform.rotation);
				
				warningNum = 1;
				
				GameController.GetComponent<ToastNotifications>().WarnNotifications(warningNum);
				
				Invoke("AsteroidDamageTarget", 5.0f);
			}
		}
	}
	
	void AsteroidDamageTarget () {
		target.GetComponent<BuildingManager>().damage(asteroidDamage);
	}
}
