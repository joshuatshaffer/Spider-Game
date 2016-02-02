using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class ResetProgress : MonoBehaviour {

	public void Reset () {
		PlayerPrefs.DeleteKey("CampaignProgress");
		SceneManager.LoadScene("Main Menu");
	}
}
