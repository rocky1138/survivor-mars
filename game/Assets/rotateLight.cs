using UnityEngine;
using System.Collections;

public class rotateLight : MonoBehaviour {
		public float speed;
		Quaternion rot;
		// Use this for initialization
		void Start () {
		 
		}
		
		// Update is called once per frame
		void Update () {
			//transform.Rotate(new Vector3(15,30,45) * speed * Time.deltaTime);
			//transform.Rotate(new Vector3(0,5,0) * speed * Time.deltaTime);
			//transform.Rotate(new Vector3(0,5,0) * speed * Time.deltaTime);
			//transform.rotation = Quaternion.AngleAxis(30 * speed * Time.deltaTime, Vector3.up);
			//rot = transform.rotation;
			//transform.rotation = rot * Quaternion.Euler(1, speed * Time.deltaTime, 1);
			transform.Rotate (0, speed, 0);
		}
	}