using UnityEngine;
using System.Collections;

public class AsteroidImpact : MonoBehaviour {

	public float speed;
	void Start(){

		
		rigidbody.velocity = transform.forward * speed;
	}
}

