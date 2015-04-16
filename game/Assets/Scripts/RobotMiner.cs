using UnityEngine;
using System.Collections;

public class RobotMiner : MonoBehaviour {
	
	public float moveSpeed = 2.0f; // Units per second
	public float speed = 5;
	public bool moving = false;
	public Vector3 target;
	public float jitter = .1f;
	public bool selected = false;
	private AudioSource movement = null;
	int tube;

	// Use this for initialization
	void Start () {
		movement = gameObject.AddComponent<AudioSource> ();
		movement.clip = Resources.Load ("Assets/Sounds/Robot/95119__robinhood76__01636-robotics-move.wav") as AudioClip;
	}

	// Update is called once per frame
	void Update () {

		if (moving == true && transform.position != target) {
			float step = speed * Time.deltaTime;

			transform.LookAt(target);
			transform.position = Vector3.MoveTowards (transform.position, target, step);
								//	jitter = jitter * -1;
								//	Vector3 temp = new Vector3(0,jitter,0);
								//	Debug.Log ("Jitter:" + jitter);
								//	transform.position += temp; 

			if (!movement.isPlaying) {
				movement.Play();
			}
		}
	}

	void OnCollisionEnter(Collision other) {
		Debug.Log ("Collision with  " + other.gameObject.tag);
	}

	void OnTriggerEnter(Collider other) {
		
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