using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LevelSelect : MonoBehaviour {

	public GameObject levelButtonPrefab;
	public GameObject lockedLevelPrefab;
	public Transform placeToPutLevels;
	public int numberOfLevels;

	void Awake () {
		int campaignProgress = PlayerPrefs.GetInt("CampaignProgress")+1;
		for (int i=1; i<=numberOfLevels; ++i) {
			if (i > campaignProgress) {
				GameObject button = (GameObject)Instantiate(lockedLevelPrefab);
				button.transform.SetParent(placeToPutLevels, false);
				button.GetComponentInChildren<Text>().text = "Level "+i;
			} else {
				GameObject button = (GameObject)Instantiate(levelButtonPrefab);
				button.transform.SetParent(placeToPutLevels, false);
				int _i = i;
				button.GetComponent<Button>().onClick.AddListener(() => LoadLevel(_i));
				button.GetComponentInChildren<Text>().text = "Level "+i;
			}
		}
	}

	void LoadLevel (int level) {
		SceneManager.LoadScene("Level "+level);
	}

	public static void WonLevel (string levelName) {
		int levelnum = int.Parse(levelName.Split(' ')[1]);
		if (PlayerPrefs.GetInt("CampaignProgress") < levelnum) {
			PlayerPrefs.SetInt("CampaignProgress", levelnum);
		}
	}
}