using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlockController : MonoBehaviour {

	//COLORS
	Color red = new Color (1f, 0f, 0f, 1f);
	Color green = new Color (0f, 1f, 0f, 1f);
	Color blue = new Color (0f, 0f, 1f, 1f);
	Color yellow = new Color (1f, 1f, 0f, 1f);
	Color cyan = new Color (0f, 1f, 1f, 1f);
	Color magenta = new Color (1f, 0f, 1f, 1f);
	Color[] colors = new Color[6];

	public int column;
	public int row;
	public int targetX;
	public int targetY;

	private GameObject otherBlock;
	private GameObject currentBlock;
	public GameObject neighborLeft;
	public GameObject neighborRight;
	public GameObject neighborUp;
	public GameObject neighborDown;
	private GridGenerator gridGen;

	private Vector2 firstTouchPosition;
	private Vector2 finalTouchPosition;
	private Vector2 tempPosition;

	private bool shaking=false;

	private float shakeAmount=2f;
	public float swipeAngle=0;

	public float startTime;
	public float colorChangeSpeed = 1.0f;

	public Text score;
	public Text swipes;
	public Text combos;

	private int intScore;
	public int intSwipes;
	private int comboCount;

	void Awake () {
		FloatingTextController.Initialize();
		score=GameObject.Find("Score").GetComponent<Text>();
		swipes=GameObject.Find("Swipes").GetComponent<Text>();
		combos=GameObject.Find("Combos").GetComponent<Text>();
		comboCount = 0;

		intScore = int.Parse (score.text);
		intSwipes = int.Parse (swipes.text);

		colors [0] = red;
		colors [1] = green;
		colors [2] = blue;
		colors [3] = yellow;
		colors [4] = cyan;
		colors [5] = magenta;

		gridGen =  FindObjectOfType<GridGenerator> ();
		targetX = (int)transform.position.x;
		targetY = (int)transform.position.y;
		column = targetX;
		row = targetY;
		startTime = Time.time;
	}

	void Update () {
		targetX = column;
		targetY = row;

		if (Mathf.Abs (targetX-transform.position.x)>.1) {
			//Move Towards the target
			tempPosition=new Vector2(targetX,transform.position.y);
			transform.position = Vector2.Lerp (transform.position, tempPosition, .4f);
		}else{
			//Directly set the position
			tempPosition=new Vector2(targetX,transform.position.y);
			transform.position = tempPosition;
			gridGen.allBlocks [column, row] = this.gameObject;
		}
		if (Mathf.Abs (targetY-transform.position.y)>.1) {
			//Move Towards the target
			tempPosition=new Vector2(transform.position.x,targetY);
			transform.position = Vector2.Lerp (transform.position, tempPosition, .4f);
		}else{
			//Directly set the position
			tempPosition=new Vector2(transform.position.x,targetY);
			transform.position = tempPosition;
			gridGen.allBlocks [column, row] = this.gameObject;
		}

		if (shaking) {
			float shake=Time.deltaTime * shakeAmount;
			Vector3 newPosition = transform.position + Random.insideUnitSphere * shake;
			newPosition.z = transform.position.z;
			transform.position=newPosition;
		}
		/*
		if (Input.GetMouseButtonDown (0)) {
			CalculateFirstTouch ();
		}
		if (Input.GetMouseButtonUp (0)) {
			CalculateLastTouch ();
		}
		*/
	}
	/*
	void CalculateFirstTouch(){
		firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		print("Mouse button or screen has been tapped");
	}
	void CalculateLastTouch(){
		finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		CalculateAngle ();
		print("Mouse button or screen has been tapped");
	}
	*/

	void OnMouseDown(){
		firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}
	void OnMouseUp(){
		finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		CalculateAngle ();
	}

	void CalculateAngle(){
		swipeAngle = Mathf.Atan2 (finalTouchPosition.y-firstTouchPosition.y,finalTouchPosition.x-firstTouchPosition.x)*180/Mathf.PI;
		if (!shaking) {
			Move ();
		}
	}

	void Move(){
		currentBlock=gridGen.allBlocks[column,row];

		if (swipeAngle > -45 && swipeAngle <= 45 && column < gridGen.width) {
			//Right swipe
			if (!(column + 1 >= gridGen.width)) {
				otherBlock = gridGen.allBlocks [column + 1, row];

				/*
				if (!(column + 2 >= gridGen.width) && !(row <= gridGen.height)) {
					otherBlockNeighbourRight = gridGen.allBlocks [column + 2, row];
					otherBlockNeighbourLeft = gridGen.allBlocks [column, row];
					otherBlockNeighbourUp = gridGen.allBlocks [column + 1, row + 1];
					otherBlockNeighbourDown = gridGen.allBlocks [column + 1, row - 1];
				}
				*/
				otherBlock.GetComponent<BlockController> ().column -= 1;
				ChangeColor();
				column += 1;

			} else {
				Shake ();
			}
		} else if (swipeAngle > 45 && swipeAngle <= 135 && row < gridGen.height) {
			//Up swipe
			if (!(row + 1 >= gridGen.height)){
				otherBlock = gridGen.allBlocks [column, row + 1];
				otherBlock.GetComponent<BlockController> ().row -= 1;
				ChangeColor();
				row += 1;
			}
			else {
				Shake ();
			}
		} else if (swipeAngle > 135 || swipeAngle <= -135 && column >= 0) {
			//Left swipe
			if (!(column - 1 < 0)) {
				otherBlock = gridGen.allBlocks [column - 1, row];
				otherBlock.GetComponent<BlockController> ().column += 1;
				ChangeColor();
				column -= 1;
			}
			else {
				Shake ();
			}
		} else if (swipeAngle > -135 && swipeAngle <= -45 && row >= 0) {
			//Down swipe
			if (!(row - 1 < 0)) {
				otherBlock = gridGen.allBlocks [column, row - 1];
				otherBlock.GetComponent<BlockController> ().row += 1;
				ChangeColor ();
				row -= 1;
			} else {
				Shake ();
			}
		} 
		//gridGen.CountProbabilities ();
		for(int i=0;i<gridGen.width;i++){
			for(int j=0;j<gridGen.height;j++){

				//Debug.Log (i+","+j+":"+gridGen.grid [i, j]);
			}
		}
	}
	public void Shake(){
		StartCoroutine ("ShakeNow");
	}
	IEnumerator ShakeNow(){
		Vector3 originalPos = transform.position;
		if (!shaking) {
			shaking = true;
		}
		yield return new WaitForSeconds (0.5f);
		shaking=false;
		transform.position=originalPos;
	}

	public void ChangeColor(){
		float t = (Time.time - startTime) * colorChangeSpeed;

		intSwipes = int.Parse (swipes.text);
		intSwipes -= 1;
		swipes.text = intSwipes.ToString ();
		comboCount = int.Parse (combos.text);

		if (currentBlock.GetComponent<Renderer> ().material.color.Equals (red)) {
			if (otherBlock.GetComponent<BlockController> ().neighborLeft != null&&otherBlock.GetComponent<BlockController> ().neighborLeft!= currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color == red) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborLeft.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, red, t);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = red;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color == green) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborLeft.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, yellow, t);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = yellow;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color == blue) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborLeft.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, magenta, t);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = magenta;
				}


				//gridGen.grid [neighborLeft.GetComponent<BlockController> ().column, neighborLeft.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<BlockController> ().neighborRight != null&&otherBlock.GetComponent<BlockController> ().neighborRight != currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color == red) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborRight.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, red, t);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = red;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color == green) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborRight.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, yellow, t);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = yellow;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color == blue) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborRight.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, magenta, t);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = magenta;
				}

				//gridGen.grid [neighborRight.GetComponent<BlockController> ().column, neighborRight.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<BlockController> ().neighborUp != null&&otherBlock.GetComponent<BlockController> ().neighborUp != currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color == red) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborUp.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, red, t);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = red;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color == green) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborUp.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, yellow, t);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = yellow;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color == blue) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborUp.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, magenta, t);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = magenta;
				}
				//gridGen.grid [neighborUp.GetComponent<BlockController> ().column, neighborUp.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<BlockController> ().neighborDown != null&&otherBlock.GetComponent<BlockController> ().neighborDown!= currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color == red) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborDown.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, red, t);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = red;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color == green) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborDown.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, yellow, t);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = yellow;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color == blue) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborDown.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, magenta, t);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = magenta;
				}
				//gridGen.grid [neighborDown.GetComponent<BlockController> ().column, neighborDown.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<Renderer> ().material.color.Equals (red)) {
					
				/*
				if(otherBlock.GetComponent<BlockController>().neighborLeft!=null)
					otherBlock.GetComponent<BlockController>().neighborLeft.GetComponent<Renderer>().material.color=currentBlock.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, colors[Random.Range(0,colors.Length)], t);
				if(otherBlock.GetComponent<BlockController>().neighborRight!=null)
					otherBlock.GetComponent<BlockController>().neighborRight.GetComponent<Renderer>().material.color=currentBlock.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, colors[Random.Range(0,colors.Length)], t);
				if(otherBlock.GetComponent<BlockController>().neighborUp!=null)
					otherBlock.GetComponent<BlockController>().neighborUp.GetComponent<Renderer>().material.color=currentBlock.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, colors[Random.Range(0,colors.Length)], t);
				if(otherBlock.GetComponent<BlockController>().neighborDown!=null)
					otherBlock.GetComponent<BlockController>().neighborDown.GetComponent<Renderer>().material.color=currentBlock.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, colors[Random.Range(0,colors.Length)], t);
				*/



				otherBlock.GetComponent<Renderer>().material.color=red;
				Destroy (otherBlock);
				gridGen.ReplaceCurrentBlock (column, row);
				//gridGen.grid [currentBlock.GetComponent<BlockController> ().column, currentBlock.GetComponent<BlockController> ().row] = 0;

				if (comboCount != 0) {
					comboCount -= 1;
				}
				AddPlus10 (comboCount);
				//AddPlus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (green)) {
				
				otherBlock.GetComponent<Renderer> ().material.color = yellow;
				Destroy (otherBlock);
				currentBlock.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, Color.yellow, t);
				currentBlock.GetComponent<Renderer> ().material.color = yellow;
				gridGen.ReplaceCurrentBlock (column, row);
				//gridGen.grid [currentBlock.GetComponent<BlockController> ().column, currentBlock.GetComponent<BlockController> ().row] = 3;
				comboCount += 1;

				//AddPlus10 (comboCount);
				AddPlus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (blue)) {
				
				otherBlock.GetComponent<Renderer>().material.color=magenta;
				Destroy (otherBlock);
				currentBlock.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, Color.magenta, t);
				currentBlock.GetComponent<Renderer> ().material.color = magenta;
				gridGen.ReplaceCurrentBlock (column, row);
				//gridGen.grid [currentBlock.GetComponent<BlockController> ().column, currentBlock.GetComponent<BlockController> ().row] = 5;

				comboCount += 1;
				//AddPlus10 (comboCount);
				AddPlus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (yellow)) {
				if (comboCount != 0) {
					comboCount -= 1;
					//intSwipes += 2;
				} 
				SubMinus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (cyan)) {
				if (comboCount != 0) {
					comboCount -= 1;
					//intSwipes += 2;
				} 
				SubMinus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (magenta)) {
				if (comboCount != 0) {
					comboCount -= 1;
					//intSwipes += 2;
				} 
				SubMinus5 ();
			}
		} else if (currentBlock.GetComponent<Renderer> ().material.color.Equals (green)) {
			if (otherBlock.GetComponent<BlockController> ().neighborLeft != null&&otherBlock.GetComponent<BlockController> ().neighborLeft!= currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color == red) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborLeft.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, yellow, t);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = yellow;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color == green) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborLeft.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, green, t);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = green;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color == blue) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborLeft.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, cyan, t);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = cyan;
				}


				//gridGen.grid [neighborLeft.GetComponent<BlockController> ().column, neighborLeft.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<BlockController> ().neighborRight != null&&otherBlock.GetComponent<BlockController> ().neighborRight != currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color == red) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborRight.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, yellow, t);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = yellow;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color == green) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborRight.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, green, t);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = green;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color == blue) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborRight.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, cyan, t);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = cyan;
				}

				//gridGen.grid [neighborRight.GetComponent<BlockController> ().column, neighborRight.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<BlockController> ().neighborUp != null&&otherBlock.GetComponent<BlockController> ().neighborUp != currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color == red) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborUp.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, yellow, t);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = yellow;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color == green) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborUp.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, green, t);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = green;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color == blue) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborUp.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, cyan, t);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = cyan;
				}
				//gridGen.grid [neighborUp.GetComponent<BlockController> ().column, neighborUp.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<BlockController> ().neighborDown != null&&otherBlock.GetComponent<BlockController> ().neighborDown!= currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color == red) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborDown.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, yellow, t);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = yellow;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color == green) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborDown.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, green, t);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = green;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color == blue) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborDown.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, cyan, t);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = cyan;
				}
				//gridGen.grid [neighborDown.GetComponent<BlockController> ().column, neighborDown.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<Renderer> ().material.color.Equals (green)) {
				
				otherBlock.GetComponent<Renderer>().material.color=green;
				Destroy (otherBlock);
				gridGen.ReplaceCurrentBlock (column, row);
				//gridGen.grid [currentBlock.GetComponent<BlockController> ().column, currentBlock.GetComponent<BlockController> ().row] = 1;

				if (comboCount != 0) {
					comboCount -= 1;
				}
				AddPlus10 (comboCount);
				//AddPlus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (red)) {
				
				otherBlock.GetComponent<Renderer>().material.color=yellow;
				Destroy (otherBlock);
				currentBlock.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, Color.yellow, t);
				currentBlock.GetComponent<Renderer> ().material.color = yellow;
				gridGen.ReplaceCurrentBlock (column, row);
				//gridGen.grid [currentBlock.GetComponent<BlockController> ().column, currentBlock.GetComponent<BlockController> ().row] = 3;

				comboCount += 1;
				//AddPlus10 (comboCount);
				AddPlus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (blue)) {
				
				otherBlock.GetComponent<Renderer>().material.color=cyan;
				Destroy (otherBlock);
				currentBlock.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, Color.cyan, t);
				currentBlock.GetComponent<Renderer> ().material.color = cyan;
				gridGen.ReplaceCurrentBlock (column, row);
				//gridGen.grid [currentBlock.GetComponent<BlockController> ().column, currentBlock.GetComponent<BlockController> ().row] = 4;

				comboCount += 1;
				//AddPlus10 (comboCount);
				AddPlus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (yellow)) {
				if (comboCount != 0) {
					comboCount -= 1;
					//intSwipes += 2;
				} 
				SubMinus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (cyan)) {
				if (comboCount != 0) {
					comboCount -= 1;
					//intSwipes += 2;
				} 
				SubMinus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (magenta)) {
				if (comboCount != 0) {
					comboCount -= 1;
					//intSwipes += 2;
				} 
				SubMinus5 ();
			}
		} else if (currentBlock.GetComponent<Renderer> ().material.color.Equals (blue)) {
			if (otherBlock.GetComponent<BlockController> ().neighborLeft != null&&otherBlock.GetComponent<BlockController> ().neighborLeft!= currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color == red) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborLeft.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, magenta, t);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = magenta;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color == green) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborLeft.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, cyan, t);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = cyan;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color == blue) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborLeft.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, blue, t);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = blue;
				}


				//gridGen.grid [neighborLeft.GetComponent<BlockController> ().column, neighborLeft.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<BlockController> ().neighborRight != null&&otherBlock.GetComponent<BlockController> ().neighborRight != currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color == red) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborRight.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, magenta, t);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = magenta;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color == green) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborRight.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, cyan, t);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = cyan;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color == blue) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborRight.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, blue, t);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = blue;
				}

				//gridGen.grid [neighborRight.GetComponent<BlockController> ().column, neighborRight.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<BlockController> ().neighborUp != null&&otherBlock.GetComponent<BlockController> ().neighborUp != currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color == red) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborUp.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, magenta, t);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = magenta;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color == green) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborUp.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, cyan, t);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = cyan;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color == blue) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborUp.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, blue, t);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = blue;
				}
				//gridGen.grid [neighborUp.GetComponent<BlockController> ().column, neighborUp.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<BlockController> ().neighborDown != null&&otherBlock.GetComponent<BlockController> ().neighborDown!= currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color == red) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborDown.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, magenta, t);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = magenta;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color == green) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborDown.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, cyan, t);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = cyan;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color == blue) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborDown.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, blue, t);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = blue;
				}
				//gridGen.grid [neighborDown.GetComponent<BlockController> ().column, neighborDown.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<Renderer> ().material.color.Equals (blue)) {
				
				otherBlock.GetComponent<Renderer>().material.color=blue;
				Destroy (otherBlock);
				gridGen.ReplaceCurrentBlock (column, row);
				//gridGen.grid [currentBlock.GetComponent<BlockController> ().column, currentBlock.GetComponent<BlockController> ().row] = 2;

				if (comboCount != 0) {
					comboCount -= 1;
				}
				AddPlus10 (comboCount);
				//AddPlus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (green)) {
				
				otherBlock.GetComponent<Renderer>().material.color=cyan;
				Destroy (otherBlock);
				currentBlock.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, Color.cyan, t);
				currentBlock.GetComponent<Renderer> ().material.color = cyan;
				gridGen.ReplaceCurrentBlock (column, row);
				//gridGen.grid [currentBlock.GetComponent<BlockController> ().column, currentBlock.GetComponent<BlockController> ().row] = 4;

				comboCount += 1;
				//AddPlus10 (comboCount);
				AddPlus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (red)) {
				
				otherBlock.GetComponent<Renderer>().material.color=magenta;
				Destroy (otherBlock);
				currentBlock.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, Color.magenta, t);
				currentBlock.GetComponent<Renderer> ().material.color = magenta;
				gridGen.ReplaceCurrentBlock (column, row);
				//gridGen.grid [currentBlock.GetComponent<BlockController> ().column, currentBlock.GetComponent<BlockController> ().row] = 5;

				comboCount += 1;
				//AddPlus10 (comboCount);
				AddPlus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (yellow)) {
				if (comboCount != 0) {
					comboCount -= 1;
					//intSwipes += 2;
				} 
				SubMinus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (cyan)) {
				if (comboCount != 0) {
					comboCount -= 1;
					//intSwipes += 2;
				} 
				SubMinus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (magenta)) {
				if (comboCount != 0) {
					comboCount -= 1;
					//intSwipes += 2;
				} 
				SubMinus5 ();
			}
		} else if (currentBlock.GetComponent<Renderer> ().material.color.Equals (yellow)) {
			if (otherBlock.GetComponent<BlockController> ().neighborLeft != null&&otherBlock.GetComponent<BlockController> ().neighborLeft!= currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color == yellow) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborLeft.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, yellow, t);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = yellow;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color == magenta) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborLeft.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, red, t);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = red;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color == cyan) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborLeft.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, green, t);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = green;
				}


				//gridGen.grid [neighborLeft.GetComponent<BlockController> ().column, neighborLeft.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<BlockController> ().neighborRight != null&&otherBlock.GetComponent<BlockController> ().neighborRight != currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color == yellow) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborRight.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, yellow, t);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = yellow;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color == magenta) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborRight.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, red, t);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = red;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color == cyan) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborRight.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, green, t);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = green;
				}

				//gridGen.grid [neighborRight.GetComponent<BlockController> ().column, neighborRight.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<BlockController> ().neighborUp != null&&otherBlock.GetComponent<BlockController> ().neighborUp != currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color == yellow) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborUp.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, yellow, t);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = yellow;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color == magenta) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborUp.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, red, t);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = red;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color == cyan) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborUp.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, green, t);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = green;
				}
				//gridGen.grid [neighborUp.GetComponent<BlockController> ().column, neighborUp.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<BlockController> ().neighborDown != null&&otherBlock.GetComponent<BlockController> ().neighborDown!= currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color == yellow) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborDown.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, yellow, t);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = yellow;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color == magenta) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborDown.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, red, t);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = red;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color == cyan) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborDown.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, green, t);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = green;
				}
				//gridGen.grid [neighborDown.GetComponent<BlockController> ().column, neighborDown.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<Renderer> ().material.color.Equals (yellow)) {
				
				otherBlock.GetComponent<Renderer>().material.color=yellow;
				Destroy (otherBlock);
				gridGen.ReplaceCurrentBlock (column, row);
				//gridGen.grid [currentBlock.GetComponent<BlockController> ().column, currentBlock.GetComponent<BlockController> ().row] = 3;

				if (comboCount != 0) {
					comboCount -= 1;
				}
				AddPlus10 (comboCount);
				//AddPlus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (cyan)) {
				
				otherBlock.GetComponent<Renderer>().material.color=green;
				Destroy (otherBlock);
				currentBlock.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, Color.green, t);
				currentBlock.GetComponent<Renderer> ().material.color = green;
				gridGen.ReplaceCurrentBlock (column, row);
				gridGen.grid [currentBlock.GetComponent<BlockController> ().column, currentBlock.GetComponent<BlockController> ().row] = 1;

				comboCount += 1;
				//AddPlus10 (comboCount);
				AddPlus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (magenta)) {
				
				otherBlock.GetComponent<Renderer>().material.color=red;
				Destroy (otherBlock);
				currentBlock.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, Color.red, t);
				currentBlock.GetComponent<Renderer> ().material.color = red;
				gridGen.ReplaceCurrentBlock (column, row);
				//gridGen.grid [currentBlock.GetComponent<BlockController> ().column, currentBlock.GetComponent<BlockController> ().row] = 0;

				comboCount += 1;
				//AddPlus10 (comboCount);
				AddPlus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (red)) {
				if (comboCount != 0) {
					comboCount -= 1;
					//intSwipes += 2;
				} 
				SubMinus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (green)) {
				if (comboCount != 0) {
					comboCount -= 1;
					//intSwipes += 2;
				} 
				SubMinus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (blue)) {
				if (comboCount != 0) {
					comboCount -= 1;
					//intSwipes += 2;
				} 
				SubMinus5 ();
			}
		} else if (currentBlock.GetComponent<Renderer> ().material.color.Equals (cyan)) {
			if (otherBlock.GetComponent<BlockController> ().neighborLeft != null&&otherBlock.GetComponent<BlockController> ().neighborLeft!= currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color == yellow) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborLeft.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, green, t);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = green;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color == magenta) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborLeft.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, blue, t);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = blue;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color == cyan) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborLeft.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, cyan, t);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = cyan;
				}


				//gridGen.grid [neighborLeft.GetComponent<BlockController> ().column, neighborLeft.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<BlockController> ().neighborRight != null&&otherBlock.GetComponent<BlockController> ().neighborRight != currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color == yellow) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborRight.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, green, t);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = green;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color == magenta) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborRight.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, blue, t);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = blue;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color == cyan) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborRight.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, cyan, t);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = cyan;
				}

				//gridGen.grid [neighborRight.GetComponent<BlockController> ().column, neighborRight.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<BlockController> ().neighborUp != null&&otherBlock.GetComponent<BlockController> ().neighborUp != currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color == yellow) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborUp.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, green, t);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = green;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color == magenta) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborUp.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, blue, t);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = blue;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color == cyan) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborUp.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, cyan, t);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = cyan;
				}
				//gridGen.grid [neighborUp.GetComponent<BlockController> ().column, neighborUp.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<BlockController> ().neighborDown != null&&otherBlock.GetComponent<BlockController> ().neighborDown!= currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color == yellow) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborDown.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, green, t);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = green;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color == magenta) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborDown.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, blue, t);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = blue;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color == cyan) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborDown.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, cyan, t);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = cyan;
				}
				//gridGen.grid [neighborDown.GetComponent<BlockController> ().column, neighborDown.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<Renderer> ().material.color.Equals (cyan)) {
				
				otherBlock.GetComponent<Renderer>().material.color=cyan;
				Destroy (otherBlock);
				gridGen.ReplaceCurrentBlock (column, row);
				//gridGen.grid [currentBlock.GetComponent<BlockController> ().column, currentBlock.GetComponent<BlockController> ().row] = 4;

				if (comboCount != 0) {
					comboCount -= 1;
				}
				AddPlus10 (comboCount);
				//AddPlus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (yellow)) {
				
				otherBlock.GetComponent<Renderer>().material.color=green;
				Destroy (otherBlock);
				currentBlock.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, Color.green, t);
				currentBlock.GetComponent<Renderer> ().material.color = green;
				gridGen.ReplaceCurrentBlock (column, row);
				//gridGen.grid [currentBlock.GetComponent<BlockController> ().column, currentBlock.GetComponent<BlockController> ().row] = 1;

				comboCount += 1;
				//AddPlus10 (comboCount);
				AddPlus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (magenta)) {
				
				otherBlock.GetComponent<Renderer>().material.color=blue;
				Destroy (otherBlock);
				currentBlock.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, Color.blue, t);
				currentBlock.GetComponent<Renderer> ().material.color = blue;
				gridGen.ReplaceCurrentBlock (column, row);
				//gridGen.grid [currentBlock.GetComponent<BlockController> ().column, currentBlock.GetComponent<BlockController> ().row] = 2;

				comboCount += 1;
				//AddPlus10 (comboCount);
				AddPlus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (red)) {
				if (comboCount != 0) {
					comboCount -= 1;
					//intSwipes += 2;
				} 
				SubMinus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (green)) {
				if (comboCount != 0) {
					comboCount -= 1;
					//intSwipes += 2;
				} 
				SubMinus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (blue)) {
				if (comboCount != 0) {
					comboCount -= 1;
					//intSwipes += 2;
				} 
				SubMinus5 ();
			}
		} else if (currentBlock.GetComponent<Renderer> ().material.color.Equals (magenta)) {
			if (otherBlock.GetComponent<BlockController> ().neighborLeft != null&&otherBlock.GetComponent<BlockController> ().neighborLeft!= currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color == yellow) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborLeft.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, red, t);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = red;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color == magenta) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborLeft.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, magenta, t);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = magenta;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color == cyan) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborLeft.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, blue, t);
					otherBlock.GetComponent<BlockController> ().neighborLeft.GetComponent<Renderer> ().material.color = blue;
				}


				//gridGen.grid [neighborLeft.GetComponent<BlockController> ().column, neighborLeft.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<BlockController> ().neighborRight != null&&otherBlock.GetComponent<BlockController> ().neighborRight != currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color == yellow) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborRight.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, red, t);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = red;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color == magenta) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborRight.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, magenta, t);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = magenta;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color == cyan) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborRight.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, blue, t);
					otherBlock.GetComponent<BlockController> ().neighborRight.GetComponent<Renderer> ().material.color = blue;
				}

				//gridGen.grid [neighborRight.GetComponent<BlockController> ().column, neighborRight.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<BlockController> ().neighborUp != null&&otherBlock.GetComponent<BlockController> ().neighborUp != currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color == yellow) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborUp.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, red, t);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = red;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color == magenta) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborUp.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, magenta, t);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = magenta;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color == cyan) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborRight.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, blue, t);
					otherBlock.GetComponent<BlockController> ().neighborUp.GetComponent<Renderer> ().material.color = blue;
				}
				//gridGen.grid [neighborUp.GetComponent<BlockController> ().column, neighborUp.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<BlockController> ().neighborDown != null&&otherBlock.GetComponent<BlockController> ().neighborDown!= currentBlock) {
				if (otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color == yellow) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborDown.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, red, t);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = red;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color == magenta) {
					AddPlus10Neighbour (otherBlock.GetComponent<BlockController> ().neighborDown.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, magenta, t);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = magenta;
				}
				if (otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color == cyan) {
					AddPlus5Neighbour (otherBlock.GetComponent<BlockController> ().neighborDown.transform.position);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, blue, t);
					otherBlock.GetComponent<BlockController> ().neighborDown.GetComponent<Renderer> ().material.color = blue;
				}
				//gridGen.grid [neighborDown.GetComponent<BlockController> ().column, neighborDown.GetComponent<BlockController> ().row]=0;
			}
			if (otherBlock.GetComponent<Renderer> ().material.color.Equals (magenta)) {
				
				otherBlock.GetComponent<Renderer>().material.color=magenta;
				Destroy (otherBlock);
				gridGen.ReplaceCurrentBlock (column, row);
				//gridGen.grid [currentBlock.GetComponent<BlockController> ().column, currentBlock.GetComponent<BlockController> ().row] = 5;

				if (comboCount != 0) {
					comboCount -= 1;
				}
				AddPlus10 (comboCount);
				//AddPlus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (cyan)) {
				
				otherBlock.GetComponent<Renderer>().material.color=blue;
				Destroy (otherBlock);
				currentBlock.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, Color.blue, t);
				currentBlock.GetComponent<Renderer> ().material.color = blue;
				gridGen.ReplaceCurrentBlock (column, row);
				//gridGen.grid [currentBlock.GetComponent<BlockController> ().column, currentBlock.GetComponent<BlockController> ().row] = 2;

				comboCount += 1;
				//AddPlus10 (comboCount);
				AddPlus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (yellow)) {
				
				otherBlock.GetComponent<Renderer>().material.color=red;
				Destroy (otherBlock);
				currentBlock.GetComponent<Renderer> ().material.color = Color.Lerp (currentBlock.GetComponent<Renderer> ().material.color, Color.red, t);
				currentBlock.GetComponent<Renderer> ().material.color = red;
				gridGen.ReplaceCurrentBlock (column, row);
				//gridGen.grid [currentBlock.GetComponent<BlockController> ().column, currentBlock.GetComponent<BlockController> ().row] = 0;

				comboCount += 1;
				//AddPlus10 (comboCount);
				AddPlus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (red)) {
				if (comboCount != 0) {
					comboCount -= 1;
					//intSwipes += 2;
				} 
				SubMinus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (green)) {
				if (comboCount != 0) {
					comboCount -= 1;
					//intSwipes += 2;
				} 
				SubMinus5 ();
			} else if (otherBlock.GetComponent<Renderer> ().material.color.Equals (blue)) {
				if (comboCount != 0) {
					comboCount -= 1;
					//intSwipes += 2;
				} 
				SubMinus5 ();
			}
		}
		combos.text = comboCount.ToString ();
		swipes.text = intSwipes.ToString ();

	}

	public void AddPlus10(int comboCount){
		
		intScore = int.Parse (score.text);
		intSwipes = int.Parse (swipes.text);
		StartCoroutine("FloatTenText");
		intScore += 10;

		//intScore += 10*comboCount;
		score.text = intScore.ToString ();
		swipes.text = intSwipes.ToString ();
	}

	public void AddPlus5(){
		intScore = int.Parse (score.text);
		intScore += 5;
		StartCoroutine("FloatFiveText");
		score.text = intScore.ToString ();
	}

	public void SubMinus5(){
		intScore = int.Parse (score.text);
		intScore -= 5;
		StartCoroutine("FloatMinusFiveText");
		score.text = intScore.ToString ();
	}



	IEnumerator FloatTenText(){
		yield return new WaitForSeconds(.1f);
		Vector2 newPosition = new Vector2(currentBlock.GetComponent<BlockController>().targetX,currentBlock.GetComponent<BlockController>().targetY);
		//FloatingTextController.CreateFloatingText ("+"+(10 * comboCount).ToString(), newPosition);
		FloatingTextController.CreateFloatingText ("+"+(10).ToString(), newPosition);
	}
	IEnumerator FloatFiveText(){
		yield return new WaitForSeconds(.1f);
		Vector2 newPosition = new Vector2(currentBlock.GetComponent<BlockController>().targetX,currentBlock.GetComponent<BlockController>().targetY);
		FloatingTextController.CreateFloatingText ("+"+(5).ToString(), newPosition);
	}
	IEnumerator FloatMinusFiveText(){
		yield return new WaitForSeconds(.1f);
		Vector2 newPosition = new Vector2(currentBlock.GetComponent<BlockController>().targetX,currentBlock.GetComponent<BlockController>().targetY);
		FloatingTextController.CreateFloatingText ("-"+(5).ToString(), newPosition);
	}


	public void AddPlus10Neighbour(Vector2 newPosition){
		intScore = int.Parse (score.text);
		intScore += 10;

		/*
		if (comboCount == 0) {
			intScore += 10;
		} else {
			intScore += 10 * comboCount;
		}
		*/
		IEnumerator coroutine = FloatTenNeighbourText (newPosition);
		StartCoroutine(coroutine);
		score.text = intScore.ToString ();
	}
	IEnumerator FloatTenNeighbourText(Vector2 newPosition){
		yield return new WaitForSeconds(.1f);

		FloatingTextController.CreateFloatingText ("+"+(10).ToString(), newPosition);
		/*
		if (comboCount == 0) {
			FloatingTextController.CreateFloatingText ("+"+(10).ToString(), newPosition);
		} else {
			FloatingTextController.CreateFloatingText ("+"+(10*comboCount).ToString(), newPosition);
		}
		*/

	}
	public void AddPlus5Neighbour(Vector2 newPosition){
		intScore = int.Parse (score.text);
		intScore += 5;

		/*
		if (comboCount == 0) {
			intScore += 10;
		} else {
			intScore += 10 * comboCount;
		}
		*/
		IEnumerator coroutine = FloatFiveNeighbourText (newPosition);
		StartCoroutine(coroutine);
		score.text = intScore.ToString ();
	}
	IEnumerator FloatFiveNeighbourText(Vector2 newPosition){
		yield return new WaitForSeconds(.1f);

		FloatingTextController.CreateFloatingText ("+"+(5).ToString(), newPosition);
		/*
		if (comboCount == 0) {
			FloatingTextController.CreateFloatingText ("+"+(10).ToString(), newPosition);
		} else {
			FloatingTextController.CreateFloatingText ("+"+(10*comboCount).ToString(), newPosition);
		}
		*/

	}
}
