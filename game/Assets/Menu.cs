using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LoadGame(){
		Application.LoadLevel(Application.loadedLevel + 1);
	}

	public void Quit(){
		Application.Quit();
	}
}
