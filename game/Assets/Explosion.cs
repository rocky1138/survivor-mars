using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
	public GameObject explosion;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		Instantiate (explosion, transform.position, transform.rotation);
		Destroy (gameObject);
	}

	void OnCollisionEnter(Collision other){
		Instantiate (explosion, transform.position, transform.rotation);
		Destroy (gameObject);
	}
}
