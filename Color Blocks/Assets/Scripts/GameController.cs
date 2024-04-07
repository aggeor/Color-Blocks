using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameController : MonoBehaviour {

	public static GameController gameController;
	public int highScoreInt;
	public Text highScore;

	void Awake () {
		//highScore = GameObject.Find ("HighScore").GetComponent<Text>();
		/*
		if (gameController==null) {
			DontDestroyOnLoad (gameObject);
			gameController = this;
		}else if(gameController!=this){
			Destroy (gameObject);
		}
		*/

	}
	void Start(){

		//LOAD HIGHSCORE
		highScoreInt=PlayerPrefs.GetInt ("HighScore");
		if (highScoreInt == null) {
			//highScoreInt = 0;
		} 
		//Debug.Log (highScoreInt);
		highScore.text = highScoreInt.ToString ();
	}
	void OnLevelWasLoaded(){
		

		//Load ();
	}

	public void Save(){
		if (File.Exists (Application.persistentDataPath + "/playerInfo.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = new FileStream(Application.persistentDataPath+"/playerInfo.dat",FileMode.Open);
			PlayerData data = (PlayerData)bf.Deserialize (file);

			highScoreInt = int.Parse (highScore.text);
			data.highScore = highScoreInt;
			bf.Serialize (file, data);
			file.Close ();
			Debug.Log ("Edited Saved Game");
		} else {

			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = new FileStream (Application.persistentDataPath + "/playerInfo.dat", FileMode.Create);
			//FileStream file = File.Create (Application.persistentDataPath+"/playerInfo.dat");
			//Debug.Log (Application.persistentDataPath);
			PlayerData data = new PlayerData ();
			highScoreInt = int.Parse (highScore.text);
			data.highScore = highScoreInt;

			bf.Serialize (file, data);
			file.Close ();
			Debug.Log ("Saved Game");
		}
	}
	public void Load(){
		if (File.Exists (Application.persistentDataPath + "/playerInfo.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = new FileStream(Application.persistentDataPath+"/playerInfo.dat",FileMode.Open);
			//FileStream file = File.Open (Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
			PlayerData data = (PlayerData)bf.Deserialize (file);
			file.Close ();
			highScoreInt = data.highScore;
			highScore.text = highScoreInt.ToString ();
			Debug.Log ("Loaded Game");
		} else {
			highScoreInt = 0;
			highScore.text = highScoreInt.ToString ();
			Debug.Log("No such file exists");
		}
	}

}

[Serializable]
class PlayerData{
	public int highScore;
}