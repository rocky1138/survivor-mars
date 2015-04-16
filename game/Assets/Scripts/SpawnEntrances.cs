using UnityEngine;
using System.Collections;

public class SpawnEntrances : MonoBehaviour {
	public GameObject[] Entrances;

	// Use this for initialization
	void Start () {
		Entrances [Random.Range (0, Entrances.Length)].SetActive (true);;
	}
	
	// Update is called once per frame
	void Update () {

	}


}
