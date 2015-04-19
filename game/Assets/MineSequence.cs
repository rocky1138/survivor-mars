using UnityEngine;
using System.Collections;

public class MineSequence : MonoBehaviour {
	public bool isMining = false;
	
	Color laserColor1 = new Color(1, 0, 0, 0.5f);
	Color laserColor2 = new Color(1, .17f, .17f, 0.4f);
	LineRenderer lineRenderer;
	public GameObject sparks;
	//public Transform Laser;
	public Material laserMat;
	bool mineAnimationRunning = false;
	public float skew;
	//public AudioClip mineClip;


	// Use this for initialization
	void Start () {
		//Lasers
		lineRenderer = gameObject.AddComponent<LineRenderer>();
		//lineRenderer.material = new Material (Shader.Find("Particles/Additive"));
		lineRenderer.material = laserMat;
		lineRenderer.SetColors(laserColor1, laserColor1);
		lineRenderer.SetWidth(.3f,.1f);
		lineRenderer.SetVertexCount(2);
		//Lasers
	}
	
	// Update is called once per frame
	void Update () {
		if (isMining == false  ) {
//			StopCoroutine (mineAnimation(dest));
		}
	}

	public void Mine(Vector3 dest){
		isMining = true;
		gameObject.GetComponent<Robot_surfaceMove>().moving = false;
		audio.Play ();
		StopCoroutine ("mineAnimation");//if already running  DOESNT SEEM TO BE STOPPING
		StartCoroutine (mineAnimation(dest));
	}


	IEnumerator mineAnimation(Vector3 dest){

		while (true) {
				
				Vector3 destSkew = new Vector3(dest.x + Random.Range(-skew,skew), dest.y + Random.Range(-skew,skew), dest.z + Random.Range(-skew,skew));
				lineRenderer.enabled = true;
				//lineRenderer.SetPosition(0, new Vector3(transform.position.x, transform.position.y - 2, transform.position.z));
				lineRenderer.SetPosition (0, transform.position);
				//lineRenderer.SetPosition (1, dest);
				lineRenderer.SetPosition(1, destSkew);
				Instantiate (sparks, destSkew, Quaternion.identity);
				//StartCoroutine(laser_die());
			yield return new WaitForSeconds (Random.Range(.3f, 1f));
				lineRenderer.enabled = false;
			yield return new WaitForSeconds (Random.Range(.1f, .5f));
		}
	}

}
