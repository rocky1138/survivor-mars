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

	Color laserColor1 = new Color(1, 0, 0, 0.5f);
	LineRenderer lineRenderer;
	public Material laserMat;
	
	//public Transform target;

	// Use this for initialization
	void Start () {

		
		// Mining Lasers
		lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.material = laserMat;
		lineRenderer.SetColors(laserColor1, laserColor1);
		lineRenderer.SetWidth(.3f,.1f);
		lineRenderer.SetVertexCount(2);

		for(int i = 0; i < Robots.Length; i++) {
			if (Robots[i] != null && Robots[i].gameObject.activeSelf == false) {
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

				if (hitInfo.collider.tag == "Robot_surface")	{

					if (hitInfo.collider.gameObject != currentRobot && currentRobot != null){
						DeselectRobot();
					}
					
					currentRobot = hitInfo.transform.gameObject;
					
					// DUPLICATE CODE DUE TO LAZINESS SEE FUNCTION BELOW
					/*
					if (currentRobot.GetComponent<RobotMiner>().inTube == true){
						CamToggleButton.SetActive(true);
						currentRobot.transform.GetChild(1).gameObject.SetActive(true);
					}
					*/
					currentRobot.transform.GetChild(3).gameObject.SetActive(true);
				}
				
				if ((hitInfo.collider.tag == "TubeEntrance"|| hitInfo.collider.tag == "Building")) {

					if (currentRobot != null) {
						currentRobot.GetComponent<RobotMiner>().target = hitInfo.point;
						currentRobot.GetComponent<RobotMiner>().moving = true;
					}
					else {
						DeselectRobot();
					}
				}
					
				if (hitInfo.collider.tag == "CaveFloor")	{

					if (currentRobot != null) {
						currentRobot.GetComponent<RobotMiner>().target = hitInfo.point;
						currentRobot.GetComponent<RobotMiner>().moving = true;
					}
					else {
						DeselectRobot();
					}
					
				} else if (hitInfo.collider.tag == "Mining-Ore") {
					if (currentRobot != null) {

						currentRobot.GetComponent<ResourceConverter>().online = true;

						// Mining Laser
						lineRenderer.enabled=true;
						lineRenderer.SetPosition(0, currentRobot.transform.position);
						lineRenderer.SetPosition(1, hitInfo.point);
						StartCoroutine(laser_die());
					}
				}
				
				if (hitInfo.collider.tag == "GamePlane" )	{
					if (currentRobot != null) {
						currentRobot.GetComponent<RobotMiner>().target = hitInfo.point;
						currentRobot.GetComponent<RobotMiner>().moving = true;
					}
					else {
						DeselectRobot();
					}
					//	DeselectRobot();
				}
			}
		}
	}

	public void EnterTube(int tube, GameObject robot) {
		CamToggleButton.SetActive(true);
		robot.transform.position = SpawnPoints[tube].transform.position;
	}

	public void SelectRobot (int num) {
		DeselectRobot ();
		currentRobot = Robots[num];
		/*
		if (currentRobot.GetComponent<RobotMiner>().inTube == true){
			CamToggleButton.SetActive(true);
			currentRobot.transform.GetChild(1).gameObject.SetActive(true);
		}
		*/
		currentRobot.transform.GetChild(3).gameObject.SetActive(true);
	}

	public void DeselectRobot() {
		if (currentRobot != null) {
			currentRobot.transform.GetChild (1).gameObject.SetActive (false);
			currentRobot.transform.GetChild (2).gameObject.SetActive (false);
			currentRobot.transform.GetChild (3).gameObject.SetActive (false);
			currentRobot = null;
		}
	}

	public void CamToggler() {
		
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

	IEnumerator laser_die(){
		yield return new WaitForSeconds(5f);
		lineRenderer.enabled = false;
		
	}

}
