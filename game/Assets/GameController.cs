using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	public GameObject currentRobot = null;
	public GameObject oldRobot = null;
	public GameObject[] Robots;
	public GameObject[] RobotButtons;
	public GameObject[] SpawnPoints;

	// Use this for initialization
	void Start () {
		for(int i = 0; i < Robots.Length; i++)
		{
			if (Robots[i].gameObject.activeSelf == false){
				if (i < RobotButtons.Length && RobotButtons[i] != null) {
					RobotButtons[i].SetActive(false);
				}
			}
		}
	}
	
	void Update () {
		if (Input.GetMouseButton (0)) {
			RaycastHit hitInfo = new RaycastHit ();
			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo)) {
				//print (hitInfo.transform.tag);
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				Physics.Raycast (ray, out hitInfo, Mathf.Infinity);
				Debug.DrawLine (ray.origin, hitInfo.point);
				Debug.Log (hitInfo.collider.tag);


						//Debug.Log("XYZ" + hitInfo.transform.position);
						if (hitInfo.collider.tag == "Robot_surface")	{

							if (hitInfo.collider.gameObject != currentRobot && currentRobot != null){
								DeselectRobot();
							}
										currentRobot = hitInfo.transform.gameObject;
										currentRobot.transform.GetChild(3).gameObject.SetActive(true);
										//moving = true;
										//dest = hitInfo.collider.transform;
							//}
						//	else {
						//		DeselectRobot();
						//	}

						}
						if (hitInfo.collider.tag == "TubeEntrance" || hitInfo.collider.tag == "CaveFloor" || hitInfo.collider.tag == "Building")	{
							Debug.Log(currentRobot);
							if (currentRobot != null) {
								currentRobot.GetComponent<Robot_surfaceMove>().dest = hitInfo.collider.transform;
								currentRobot.GetComponent<Robot_surfaceMove>().moving = true;
							}
							else {
								DeselectRobot();
							}
							//moving = true;
							//dest = hitInfo.collider.transform;
						}
						if (hitInfo.collider.tag == "GamePlane" )	{
							//oldRobot = currentRobot;
							DeselectRobot();

							
					//		if (currentRobot != null){
							//currentRobot.GetComponent<Robot_surfaceMove>().dest = hitInfo.point;
					//			currentRobot.GetComponent<Robot_surfaceMove>().moving = false;
					//		}
							//moving = true;
							//dest = hitInfo.collider.transform;
						}
						

			}
		}
	}

	public void EnterTube(int tube, GameObject robot){
	
			robot.transform.position = SpawnPoints[tube].transform.position;


	}

	public void SelectRobot (int num){
		currentRobot = Robots[num];
	}

	public void DeselectRobot(){
		if (currentRobot != null) {
						currentRobot.transform.GetChild (3).gameObject.SetActive (false);
						currentRobot = null;
				}
	}
}
