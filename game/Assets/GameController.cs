using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	public GameObject currentRobot = null;
	public GameObject[] Robots;

	// Use this for initialization
	void Start () {
	
	}
	
	void Update () {
		if (Input.GetMouseButton (0)) {
			RaycastHit hitInfo = new RaycastHit ();
			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo)) {
				//print (hitInfo.transform.tag);
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				Physics.Raycast (ray, out hitInfo, Mathf.Infinity);
				Debug.DrawLine (ray.origin, hitInfo.point);
				Debug.Log (hitInfo.transform.tag);


						//Debug.Log("XYZ" + hitInfo.transform.position);
						if (hitInfo.collider.tag == "Robot_surface")	{
							currentRobot = hitInfo.transform.gameObject;
							hitInfo.transform.renderer.material.color = Color.green;
							//moving = true;
							//dest = hitInfo.collider.transform;
						}
						if (hitInfo.collider.tag == "TubeEntrance" || hitInfo.collider.tag == "Building")	{
							if (currentRobot != null){
								currentRobot.GetComponent<Robot_surfaceMove>().dest = hitInfo.collider.transform;
								currentRobot.GetComponent<Robot_surfaceMove>().moving = true;
							}
							//moving = true;
							//dest = hitInfo.collider.transform;
						}
						if (hitInfo.collider.tag == "GamePlane" )	{
							if (currentRobot != null){
							//currentRobot.GetComponent<Robot_surfaceMove>().dest = hitInfo.point;
								currentRobot.GetComponent<Robot_surfaceMove>().moving = false;
							}
							//moving = true;
							//dest = hitInfo.collider.transform;
						}


			}
		}
	}

	public void SelectRobot (int num){
		currentRobot = Robots[num];
	}
}
