using UnityEngine;
using System.Collections;

public class Robot_surfaceMove : MonoBehaviour {
	public float moveSpeed = 2.0f; // Units per second
	public float speed = 5;
	public bool moving = false;
	//public Transform dest;
	public Vector3 target;
	public float jitter = .1f;
	public bool selected = false;
	public bool inTube = false;
	int tube;
	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void LateUpdate(){
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target - transform.position), Time.deltaTime * 6);
	}

	void Update () {

		//if (moving == true && transform.position != dest.position) {
			
		if (moving == true && transform.position != target) {
			
			//transform.LookAt(dest);
	//		transform.LookAt(target);

		 	
		

			float step = speed * Time.deltaTime;
			
			//transform.position = Vector3.MoveTowards (transform.position, dest.position, step);
			transform.position = Vector3.MoveTowards (transform.position, target, step);
								//	jitter = jitter * -1;
								//	Vector3 temp = new Vector3(0,jitter,0);
								//	Debug.Log ("Jitter:" + jitter);
								//	transform.position += temp; 
		}
	}

	void OnCollisionEnter(Collision other) {
		Debug.Log ("Collision with  " + other.gameObject.tag);
	}

	void OnTriggerEnter(Collider other) {
		
		Debug.Log ("Trigger with  " + other.tag);
		
		if (other.tag == "TubeEntrance") {
				inTube = true;
				gameObject.transform.GetChild (1).gameObject.SetActive (true);
				moving = false;
				if (other.name == "LavaTube1") {
						tube = 0;
				}
				if (other.name == "LavaTube2") {
						tube = 1;
				}
				if (other.name == "LavaTube3") {
						tube = 2;
				}
				GameObject.Find ("GameController").GetComponent<GameController> ().EnterTube (tube, gameObject);
		}
	}

}