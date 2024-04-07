using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {

	public Button restartBtn;
	public Camera camera;
	public GameObject swipesText;
	public GameObject swipes;
	public GameObject scoreText;
	public GameObject score;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//restartBtn.onClick.AddListener(Restart);
		if (Input.GetKeyDown (KeyCode.Backspace)) {
			Restart ();
		
		}
		if(Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit();
		}
		/*
		if(Input.GetKeyDown(KeyCode.Space)){
			if(Time.timeScale==1){
				Time.timeScale = 0;
			}else{
				Time.timeScale = 1;
			}
		}
		*/
		//Debug.LogError (Input.deviceOrientation);
		if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft||Input.deviceOrientation == DeviceOrientation.LandscapeRight) {
			Debug.LogError (camera.GetComponent<Camera> ().orthographicSize);
			Debug.Log (camera.GetComponent<Camera> ().orthographicSize);
			//camera.GetComponent<Camera> ().orthographicSize=2;
			camera.orthographicSize=2;
			/*
			swipesText.GetComponent<Text> ().fontSize = 20;
			swipes.GetComponent<Text> ().fontSize = 20;
			scoreText.GetComponent<Text> ().fontSize = 20;
			score.GetComponent<Text> ().fontSize = 20;

			swipesText.GetComponent<RectTransform> ().sizeDelta = new Vector2 (80f, 30f);
			swipes.GetComponent<RectTransform> ().sizeDelta = new Vector2 (40f, 30f);
			scoreText.GetComponent<RectTransform> ().sizeDelta = new Vector2 (65f, 30f);
			score.GetComponent<RectTransform> ().sizeDelta = new Vector2 (45f, 30f);
			*/
		}else if (Input.deviceOrientation == DeviceOrientation.Portrait||Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown) {
			Debug.LogError (camera.GetComponent<Camera> ().orthographicSize);
			Debug.Log (camera.GetComponent<Camera> ().orthographicSize);
			//camera.GetComponent<Camera> ().orthographicSize=3;
			camera.orthographicSize=3;

			/*
			swipesText.GetComponent<Text> ().fontSize = 40;
			swipes.GetComponent<Text> ().fontSize = 40;
			scoreText.GetComponent<Text> ().fontSize = 40;
			score.GetComponent<Text> ().fontSize = 40;

			swipesText.GetComponent<RectTransform> ().sizeDelta = new Vector2 (168f, 62f);
			swipes.GetComponent<RectTransform> ().sizeDelta = new Vector2 (86f, 62f);
			scoreText.GetComponent<RectTransform> ().sizeDelta = new Vector2 (134f, 62f);
			score.GetComponent<RectTransform> ().sizeDelta = new Vector2 (94f, 62f);
			*/
		}
	}
	void OnMouseDown(){
		
	}
	void OnMouseDrag()
	{
		

	}
	public void Restart(){
		SceneManager.LoadScene(0);
	}
}
