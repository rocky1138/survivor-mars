using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToastNotifications : MonoBehaviour {
	public AudioClip AchievSound;
	public AudioClip WarnSound;
	public int delay;
	public Sprite[] Achievements;
	public string[] AchievementsText;
	public int[] AchievementsValue;
	public Sprite[] Perils;
	public string[] PerilsText;

	public Text text;
	public GameObject image;
	public GameObject AchievementObject;
	// Use this for initialization
	void Start () {
		AchievementObject.SetActive (false);
	//	ToastNotification (0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ToastNotification(int num){
			audio.PlayOneShot (AchievSound);
			AchievementObject.SetActive (true);
			text.text = AchievementsText [num];
			image.GetComponent<Image>().sprite = Achievements [num];
			Invoke("closeToast", delay);
	}

	public void WarnNotifications(int num){
		audio.PlayOneShot (WarnSound);
		AchievementObject.SetActive (true);
		text.text = PerilsText [num];
		image.GetComponent<Image>().sprite = Perils [num];
		Invoke("closeToast", delay * 2);
	}

	public void closeToast(){
		AchievementObject.SetActive (false);
	}

}
