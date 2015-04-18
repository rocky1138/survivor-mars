using UnityEngine;
using System.Collections;

public class DustStorm : MonoBehaviour {
	public float duration = 5.0f;
	float start;
	float finish;

	// Use this for initialization
	void OnEnable () {
		Debug.LogError("Starting Dust Storm..............");
		start = Time.time;
		finish = Time.time + duration;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > finish) {
			gameObject.SetActive(false);
			Debug.LogError("Duststorm ends!");
		}
	}
}
