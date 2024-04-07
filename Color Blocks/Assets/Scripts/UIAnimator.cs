using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIAnimator : MonoBehaviour {

	Animator anim;
	public Text swipes;
	public Text score;
	public Text highScore;

	public int intSwipes;
	private bool shaking=false;
	private float shakeAmount=20f;
	GameObject[] blocks;
	// Use this for initialization
	void Awake(){

	}

	void Start () {
		swipes=GameObject.Find("Swipes").GetComponent<Text>();
		score=GameObject.Find("Score").GetComponent<Text>();
		highScore=GameObject.Find("HighScore").GetComponent<Text>();
		intSwipes = int.Parse (swipes.text);
		anim=GetComponent<Animator> ();
		//GameController.gameController.Load ();
	}
	
	// Update is called once per frame
	void Update () {
		intSwipes = int.Parse (swipes.text);
		anim.SetInteger ("Swipes", intSwipes);
		if (intSwipes == 15) {
			swipes.color=new Color (1f, 1f, 0f, 1f);
		}
		if (intSwipes == 10) {
			swipes.color=new Color (1f, 0.5f, 0f, 1f);
		}
		if (intSwipes == 5) {
			swipes.color=new Color (1f, 0f, 0f, 1f);
		}
		/*
		if (intSwipes <= 20) {
			//swipes.color=new Color (0f, 1f, 0f, 1f);
			Shake ();
		}
		*/
		if (intSwipes == 0) {
			blocks=GameObject.FindGameObjectsWithTag ("Block");
			foreach (GameObject block in blocks) {
				block.GetComponent<BoxCollider2D> ().enabled = false;
			}


			//GameController.gameController.Save ();
		}

		//SAVE HIGHSCORE
		if (int.Parse(score.text) > int.Parse(highScore.text)) {
			highScore.text = score.text;
			highScore.color = new Color (0f, 1f, 0f, 1f);
			PlayerPrefs.SetInt ("HighScore", int.Parse (highScore.text));
			PlayerPrefs.Save ();
			//GameController.gameController.Save ();
		}
		/*
		PlayerPrefs.SetInt ("HighScore", 0);
		PlayerPrefs.Save ();
		*/

		//Debug.Log (PlayerPrefs.GetInt ("HighScore"));
		/*
		if (shaking) {
			float shake=Time.deltaTime * shakeAmount;
			Vector3 newPosition = swipes.rectTransform.position + Random.insideUnitSphere * shake;
			newPosition.z = swipes.rectTransform.position.z;
			swipes.rectTransform.position=newPosition;
		}
		*/
	}
	/*
	public void Shake(){
		StartCoroutine ("ShakeNow");
	}
	IEnumerator ShakeNow(){
		Vector3 originalPos = swipes.rectTransform.position;
		if (!shaking) {
			shaking = true;
			Debug.Log("shaking");
		}
		yield return new WaitForSeconds (0.5f);
		shaking=false;
		swipes.rectTransform.position=originalPos;
	}
	*/
}
