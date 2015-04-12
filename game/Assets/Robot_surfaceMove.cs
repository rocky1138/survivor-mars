using UnityEngine;
using System.Collections;

public class Robot_surfaceMove : MonoBehaviour {
	public float moveSpeed = 2.0f; // Units per second
	public float speed = 5;
	public bool moving = false;
	public Transform dest;
	public float jitter = .1f;
	public bool selected = false;
	int tube;
	// Use this for initialization
	void Start () {
	
	}


	// Update is called once per frame
	void Update () {

		if (moving == true && transform.position != dest.position) {
			transform.LookAt(dest);
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, dest.position, step);
								//	jitter = jitter * -1;
								//	Vector3 temp = new Vector3(0,jitter,0);
								//	Debug.Log ("Jitter:" + jitter);
								//	transform.position += temp; 
		}


	/*	if (Input.GetMouseButton (0)) {
			moving = false;
			RaycastHit hitInfo = new RaycastHit ();
			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo)) {
				//print (hitInfo.transform.tag);
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				Physics.Raycast (ray, out hitInfo, Mathf.Infinity);
				Debug.DrawLine (ray.origin, hitInfo.point);
				Debug.Log (hitInfo.transform.tag);
				Debug.Log("XYZ" + hitInfo.transform.position);
				if (hitInfo.collider.tag == "TubeEntrance" || hitInfo.collider.tag == "Building")	{
					moving = true;
					dest = hitInfo.collider.transform;
				}
			}
		}*/
	}

	void OnCollisionEnter(Collision other) {
		Debug.Log ("Collision with  " + other.gameObject.tag);
	}

	void OnTriggerEnter(Collider other) {
		//int tube;
		Debug.Log ("Trigger with  " + other.tag);
		if (other.tag == "TubeEntrance") {
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