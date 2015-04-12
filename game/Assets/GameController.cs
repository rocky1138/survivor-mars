using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
	public GameObject currentRobot = null;
	public GameObject oldRobot = null;
	public GameObject[] Robots;
	public GameObject[] RobotButtons;
	public GameObject[] SpawnPoints;
	public GameObject CamToggleButton;

	public Camera Surface;
	public Camera Surface_PIP;
	public bool CamToggleState = false;
	Camera currentRobotCam;

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
		if (Input.GetMouseButton (0) && !EventSystem.current.IsPointerOverGameObject()) {
			RaycastHit hitInfo = new RaycastHit ();
			if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo)) {

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
										//DUPLICATE CODE DUE TO LAZINESS SEE FUNCTION BELOW
										if (currentRobot.GetComponent<Robot_surfaceMove>().inTube == true){
											CamToggleButton.SetActive(true);
											currentRobot.transform.GetChild(1).gameObject.SetActive(true);
										}
										currentRobot.transform.GetChild(3).gameObject.SetActive(true);
										//moving = true;
										//dest = hitInfo.collider.transform;
							//}
						//	else {
						//		DeselectRobot();
						//	}

						}
				if ((hitInfo.collider.tag == "TubeEntrance"|| hitInfo.collider.tag == "Building") && currentRobot.GetComponent<Robot_surfaceMove>().inTube == false)	{
						//Debug.Log(currentRobot);
						if (currentRobot != null) {
							//currentRobot.GetComponent<Robot_surfaceMove>().dest = hitInfo.collider.transform;
							currentRobot.GetComponent<Robot_surfaceMove>().target = hitInfo.point;
							currentRobot.GetComponent<Robot_surfaceMove>().moving = true;
						}
						else {
							DeselectRobot();
						}
						//moving = true;
						//dest = hitInfo.collider.transform;
					}
					
					if (hitInfo.collider.tag == "CaveFloor")	{
						//Debug.Log(currentRobot);
						if (currentRobot != null) {
							//currentRobot.GetComponent<Robot_surfaceMove>().dest = hitInfo.collider.transform;
							currentRobot.GetComponent<Robot_surfaceMove>().target = hitInfo.point;
							currentRobot.GetComponent<Robot_surfaceMove>().moving = true;
						}
						else {
							DeselectRobot();
						}
						//moving = true;
						//dest = hitInfo.collider.transform;
						
					} else if (hitInfo.collider.tag == "Mining-Ore") {
						
					}
					
					if (hitInfo.collider.tag == "GamePlane" )	{
							//oldRobot = currentRobot;
							//Debug.Log ("GUI  " + !EventSystem.current.IsPointerOverGameObject());
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
			CamToggleButton.SetActive(true);
			robot.transform.position = SpawnPoints[tube].transform.position;


	}

	public void SelectRobot (int num){
		DeselectRobot ();
		currentRobot = Robots[num];
		if (currentRobot.GetComponent<Robot_surfaceMove>().inTube == true){
			CamToggleButton.SetActive(true);
			currentRobot.transform.GetChild(1).gameObject.SetActive(true);
		}
		currentRobot.transform.GetChild(3).gameObject.SetActive(true);
	}

	public void DeselectRobot(){
		if (currentRobot != null) {
						currentRobot.transform.GetChild (1).gameObject.SetActive (false);
						currentRobot.transform.GetChild (2).gameObject.SetActive (false);
						currentRobot.transform.GetChild (3).gameObject.SetActive (false);
						currentRobot = null;
				}
	}

	public void CamToggler(){
		Debug.Log ("CamToggler");
		CamToggleState = !CamToggleState;
		if (CamToggleState == true) {
				currentRobot.transform.GetChild (2).gameObject.SetActive (true);
				Surface_PIP.gameObject.SetActive(true);
				Surface.tag = "MainCamera_Bak";
				Surface.camera.enabled = false;
				//currentRobot.transform.GetChild (2).tag = "MainCamera";
		} else {
				currentRobot.transform.GetChild (2).gameObject.SetActive (false);
				Surface_PIP.gameObject.SetActive(false);
				Surface.tag = "MainCamera";
				Surface.camera.enabled = true;
		}

	}
}
