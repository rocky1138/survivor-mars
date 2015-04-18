using UnityEngine;
using System.Collections;

public class DestroyByTime : MonoBehaviour {
	public float dtime = 10;
	// Use this for initialization
	void Start () {
		Destroy (gameObject, dtime);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
