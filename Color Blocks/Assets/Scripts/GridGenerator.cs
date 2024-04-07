using UnityEngine;
using System.Collections;

public class GridGenerator : MonoBehaviour {

	//COLORS
	Color red = new Color (1f, 0f, 0f, 1f);
	Color green = new Color (0f, 1f, 0f, 1f);
	Color blue = new Color (0f, 0f, 1f, 1f);
	Color yellow = new Color (1f, 1f, 0f, 1f);
	Color cyan = new Color (0f, 1f, 1f, 1f);
	Color magenta = new Color (1f, 0f, 1f, 1f);

	public int width;
	public int height;
	public int[,] grid;
	public float maxSize;
	public float growFactor;
	public float waitTime;
	public GameObject[,] allBlocks;
	string mode;
	public Renderer rend;
	public bool useRandomSeed;
	public string seed;
	public float currentNumber;
	public float nextNumber;

	public float[] nextProb;
	public float[] currProb;

	GameObject currentBlock;
	GameObject nextBlock;

	public System.Random pseudoRandom;

	void Awake(){
		width = 3;
		height = 3;
		maxSize = 1f;
		growFactor = 3f;
		waitTime = 1f;
		currProb = new float[6] {0.166f,0.166f,0.166f,0.166f,0.166f,0.166f };
		nextProb = new float[6] {0.166f,0.166f,0.166f,0.166f,0.166f,0.166f };
		useRandomSeed = true;
		grid = new int[width, height];
		allBlocks = new GameObject[width, height];
		//pseudoRandom = new System.Random (seed.GetHashCode ());

		GenerateGrid ();
	}
	void Start () {

	}
	void Update () {
		FindNeighbours ();
	}
	public void GenerateGrid(){
		RandomFillGrid ();
		//GameController.gameController.Load ();

	}
	void RandomFillGrid(){
		if (useRandomSeed) {
			seed = System.DateTime.Now.Ticks.ToString ();
		}

		pseudoRandom = new System.Random (seed.GetHashCode ());
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (currentNumber == 0) {
					currentNumber = Random.value;

					nextNumber = Random.value;

				} else {
					currentNumber = nextNumber;
					nextNumber = Random.value;
				}

				//Debug.Log(currentNumber);
				currentBlock=(GameObject)Instantiate (Resources.Load ("White"),new Vector3(x,y,0), Quaternion.Euler (0, 0, 0));
				rend = currentBlock.GetComponent<Renderer> ();
				if (pseudoRandom.Next (0, 100) < 0f) {
					grid [x, y] = 7;
					currentBlock=(GameObject) Instantiate (Resources.Load ("Rainbow"),new Vector3(x,y,0), Quaternion.Euler (0, 0, 0));

				}
				else{
					//Debug.Log (x+","+y+":"+currProb[0]);
					//Debug.Log (x+","+y+":"+(currProb[0]+currProb[1]));
					//Debug.Log (x+","+y+":"+(currProb[0]+currProb[1]+currProb[2]));
					//Debug.Log (x+","+y+":"+(currProb[0]+currProb[1]+currProb[2]+currProb[3]));
					//Debug.Log (x+","+y+":"+(currProb[0]+currProb[1]+currProb[2]+currProb[3]+currProb[4]));
					//Debug.Log (x+","+y+":"+(currProb[0]+currProb[1]+currProb[2]+currProb[3]+currProb[4]+currProb[5]));

					if (currentNumber >= 0 && currentNumber < currProb [0]) {
						//currentBlock=(GameObject)Instantiate (Resources.Load ("White"),new Vector3(x,y,0), Quaternion.Euler (0, 0, 0));
						//rend = currentBlock.GetComponent<Renderer> ();
						rend.material.SetColor ("_Color", red);
						grid [x, y] = 0;
					} else if (currentNumber >= currProb [0] && currentNumber < currProb [0] + currProb [1]) {
						//currentBlock=(GameObject)Instantiate (Resources.Load ("White"),new Vector3(x,y,0), Quaternion.Euler (0, 0, 0));
						//rend = currentBlock.GetComponent<Renderer> ();
						rend.material.SetColor ("_Color", green);
						grid [x, y] = 1;

					} else if (currentNumber >= currProb [0] + currProb [1] && currentNumber < currProb [0] + currProb [1] + currProb [2]) {
						//currentBlock=(GameObject)Instantiate (Resources.Load ("White"),new Vector3(x,y,0), Quaternion.Euler (0, 0, 0));
						//rend = currentBlock.GetComponent<Renderer> ();
						rend.material.SetColor ("_Color", blue);
						grid [x, y] = 2;
					} else if (currentNumber >= currProb [0] + currProb [1] + currProb [2] && currentNumber < currProb [0] + currProb [1] + currProb [2] + currProb [3]) {
						//currentBlock=(GameObject)Instantiate (Resources.Load ("White"),new Vector3(x,y,0), Quaternion.Euler (0, 0, 0));
						//rend = currentBlock.GetComponent<Renderer> ();
						rend.material.SetColor ("_Color", yellow);
						grid [x, y] = 3;
					} else if (currentNumber >= currProb [0] + currProb [1] + currProb [2] + currProb [3] && currentNumber < currProb [0] + currProb [1] + currProb [2] + currProb [3] + currProb [4]) {
						//currentBlock=(GameObject)Instantiate (Resources.Load ("White"),new Vector3(x,y,0), Quaternion.Euler (0, 0, 0));
						//rend = currentBlock.GetComponent<Renderer> ();
						rend.material.SetColor ("_Color", cyan);
						grid [x, y] = 4;
					}else{
					//} else if (currentNumber>=currProb [0] + currProb [1] + currProb [2] + currProb [3] + currProb [4]&&currentNumber < currProb[0]+currProb[1]+currProb[2]+currProb[3]+currProb[4]+currProb[5]) {
						//currentBlock=(GameObject)Instantiate (Resources.Load ("White"),new Vector3(x,y,0), Quaternion.Euler (0, 0, 0));
						//rend = currentBlock.GetComponent<Renderer>();
						rend.material.SetColor("_Color", magenta);
						grid [x, y] = 5;
					} 

				}
				//Debug.Log(x+","+y+":"+grid [x, y]);
				allBlocks [x, y] = currentBlock;
				currentBlock = null;
			}
		}
		nextBlock=(GameObject)Instantiate (Resources.Load ("White"),new Vector3(2f,-0.8f,0), Quaternion.Euler (0, 0, 0));
		nextBlock.transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);
		nextBlock.GetComponent<BlockController> ().enabled = false;
		//SET NEXT BLOCK COLOR
		if (nextNumber < currProb[0]) {
			rend = nextBlock.GetComponent<Renderer>();
			rend.material.SetColor("_Color", red);
		} else if (nextNumber < currProb[0]+currProb[1]) {
			rend = nextBlock.GetComponent<Renderer>();
			rend.material.SetColor("_Color", green);
		} else if (nextNumber < currProb[0]+currProb[1]+currProb[2]) {
			rend = nextBlock.GetComponent<Renderer>();
			rend.material.SetColor("_Color", blue);
		} else if (nextNumber < currProb[0]+currProb[1]+currProb[2]+currProb[3]) {
			rend = nextBlock.GetComponent<Renderer>();
			rend.material.SetColor("_Color", yellow);
		} else if (nextNumber < currProb[0]+currProb[1]+currProb[2]+currProb[3]+currProb[4]) {
			rend = nextBlock.GetComponent<Renderer>();
			rend.material.SetColor("_Color", cyan);
		} else if (nextNumber < currProb[0]+currProb[1]+currProb[2]+currProb[3]+currProb[4]+currProb[5]) {
			rend = nextBlock.GetComponent<Renderer>();
			rend.material.SetColor("_Color", magenta);
		}
		CountProbabilities ();

	}
	public void ReplaceCurrentBlock(int x,int y){
		Destroy (nextBlock);
		currentNumber = nextNumber;
		nextNumber=Random.value;
		if (pseudoRandom.Next (0, 100) < 0f) {
			grid [x, y] = 7;
			currentBlock=(GameObject) Instantiate (Resources.Load ("Rainbow"),new Vector3(x,y,0), Quaternion.Euler (0, 0, 0));
			currentBlock.transform.localScale = new Vector3 (0, 0, 0);
			StartCoroutine(Scale(currentBlock));
			nextBlock=(GameObject)Instantiate (Resources.Load ("White"),new Vector3(2f,-0.8f,0), Quaternion.Euler (0, 0, 0));
			nextBlock.transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);
			nextBlock.GetComponent<BlockController> ().enabled = false;

		}
		else{
			currentBlock=(GameObject)Instantiate (Resources.Load ("White"),new Vector3(x,y,0), Quaternion.Euler (0, 0, 0));
			nextBlock=(GameObject)Instantiate (Resources.Load ("White"),new Vector3(2f,-0.8f,0), Quaternion.Euler (0, 0, 0));
			nextBlock.transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);
			nextBlock.GetComponent<BlockController> ().enabled = false;
			currentBlock.transform.localScale = new Vector3 (0, 0, 0);
			StartCoroutine(Scale(currentBlock));
			if (currentNumber>=0&&currentNumber < currProb[0]) {
				rend = currentBlock.GetComponent<Renderer>();
				rend.material.SetColor("_Color", red);
				grid [x, y] = 0;
			} else if (currentNumber>=currProb[0]&&currentNumber < currProb[0]+currProb[1]) {
				rend = currentBlock.GetComponent<Renderer>();
				rend.material.SetColor("_Color", green);
				grid [x, y] = 1;
			} else if (currentNumber>=currProb[0]+currProb[1]&&currentNumber < currProb[0]+currProb[1]+currProb[2]) {
				rend = currentBlock.GetComponent<Renderer>();
				rend.material.SetColor("_Color", blue);
				grid [x, y] = 2;
			} else if (currentNumber>=currProb[0]+currProb[1]+currProb[2]&&currentNumber < currProb[0]+currProb[1]+currProb[2]+currProb[3]) {
				rend = currentBlock.GetComponent<Renderer>();
				rend.material.SetColor("_Color", yellow);
				grid [x, y] = 3;
			} else if (currentNumber>=currProb[0]+currProb[1]+currProb[2]+currProb[3]&&currentNumber < currProb[0]+currProb[1]+currProb[2]+currProb[3]+currProb[4]) {
				rend = currentBlock.GetComponent<Renderer>();
				rend.material.SetColor("_Color", cyan);
				grid [x, y] = 4;
			} else if (currentNumber>=currProb[0]+currProb[1]+currProb[2]+currProb[3]+currProb[4]&&currentNumber < currProb[0]+currProb[1]+currProb[2]+currProb[3]+currProb[4]+currProb[5]) {
				rend = currentBlock.GetComponent<Renderer>();
				rend.material.SetColor("_Color", magenta);
				grid [x, y] = 5;
			} 
		}
		allBlocks [x, y] = currentBlock;
		currentBlock = null;

		CountProbabilities ();

		//SET NEXT BLOCK COLOR
		if (nextNumber>0&&nextNumber < currProb[0]) {
			rend = nextBlock.GetComponent<Renderer>();
			rend.material.SetColor("_Color", red);
		} else if (nextNumber>currProb[0]&&nextNumber < currProb[0]+currProb[1]) {
			rend = nextBlock.GetComponent<Renderer>();
			rend.material.SetColor("_Color", green);
		} else if (nextNumber>currProb[0]+currProb[1]&&nextNumber < currProb[0]+currProb[1]+currProb[2]) {
			rend = nextBlock.GetComponent<Renderer>();
			rend.material.SetColor("_Color", blue);
		} else if (nextNumber>currProb[0]+currProb[1]+currProb[2]&&nextNumber < currProb[0]+currProb[1]+currProb[2]+currProb[3]) {
			rend = nextBlock.GetComponent<Renderer>();
			rend.material.SetColor("_Color", yellow);
		} else if (nextNumber>currProb[0]+currProb[1]+currProb[2]+currProb[3]&&nextNumber < currProb[0]+currProb[1]+currProb[2]+currProb[3]+currProb[4]) {
			rend = nextBlock.GetComponent<Renderer>();
			rend.material.SetColor("_Color", cyan);
		} else if (nextNumber>currProb[0]+currProb[1]+currProb[2]+currProb[3]+currProb[4]&&nextNumber < currProb[0]+currProb[1]+currProb[2]+currProb[3]+currProb[4]+currProb[5]) {
			rend = nextBlock.GetComponent<Renderer>();
			rend.material.SetColor("_Color", magenta);
		}
		FindNeighbours();
	}

	public IEnumerator Scale(GameObject obj)
	{
		
		float timer = 0;

		while(obj!=null) // this could also be a condition indicating "alive or dead"
		{
			// we scale all axis, so they will have the same value, 
			// so we can work with a float instead of comparing vectors

			while(maxSize > obj.transform.localScale.x)
			{
				timer += Time.deltaTime;
				obj.transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growFactor;
				yield return null;
			}
			// reset the timer

			yield return new WaitForSeconds(waitTime);
			/*
			timer = 0;
			while(1 < obj.transform.localScale.x)
			{
				timer += Time.deltaTime;
				obj.transform.localScale -= new Vector3(1, 1, 1) * Time.deltaTime * growFactor;
				yield return null;
			}

			timer = 0;
			yield return new WaitForSeconds(waitTime);
			*/
		}
	}

	public void CountProbabilities(){
		int[] blockCounts = new int[6] { 0, 0, 0, 0, 0, 0 };
		float[] weight= new float[6];
		float totalWeight = 0f;
		foreach (GameObject b in allBlocks) {
			rend = b.GetComponent<Renderer> ();
			if (rend.material.color == red) {
				blockCounts [0] += 1;
			}else if (rend.material.color == green) {
				blockCounts [1] += 1;
			}else if (rend.material.color == blue) {
				blockCounts [2] += 1;
			}else if (rend.material.color == yellow) {
				blockCounts [3] += 1;
			}else if (rend.material.color == cyan) {
				blockCounts [4] += 1;
			}else if (rend.material.color == magenta) {
				blockCounts [5] += 1;
			}
		}
		/*
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (grid [x, y] == 0) {
					blockCounts [0] += 1;
				}else if (grid [x, y] == 1) {
					blockCounts [1] += 1;
				}else if (grid [x, y] == 2) {
					blockCounts [2] += 1;
				}else if (grid [x, y] == 3) {
					blockCounts [3] += 1;
				}else if (grid [x, y] == 4) {
					blockCounts [4] += 1;
				}else if (grid [x, y] == 5) {
					blockCounts [5] += 1;
				}

			}
		}

		Debug.Log ("red:" + blockCounts [0]);
		Debug.Log ("green:" + blockCounts [1]);
		Debug.Log ("blue:" + blockCounts [2]);
		Debug.Log ("yellow:" + blockCounts [3]);
		Debug.Log ("cyan:" + blockCounts [4]);
		Debug.Log ("magenta:" + blockCounts [5]);
		*/
		for (int i = 0; i < 6; i++) {
			if (blockCounts [i] != 0) {
				weight [i] = (float)1 / blockCounts [i];
			} else if (blockCounts [i] == 0) {
				weight [i] = (float)2;
			}
			totalWeight += weight [i];
		}
		for (int i = 0; i < 6; i++) {
			currProb [i] = nextProb[i];
			nextProb [i] = weight [i] / totalWeight;
		}
	}
	public void FindNeighbours(){
		foreach (GameObject b in allBlocks){
			//LEFT NEIGHBOUR
			if (IsInRange (b.GetComponent<BlockController> ().column - 1,b.GetComponent<BlockController> ().row)) {
				b.GetComponent<BlockController> ().neighborLeft = allBlocks[b.GetComponent<BlockController> ().column - 1,b.GetComponent<BlockController> ().row];

			} else {
				b.GetComponent<BlockController> ().neighborLeft = null;
			}
			//RIGHT NEIGHBOUR
			if (IsInRange (b.GetComponent<BlockController> ().column + 1,b.GetComponent<BlockController> ().row)) {
				b.GetComponent<BlockController> ().neighborRight = allBlocks[b.GetComponent<BlockController> ().column + 1,b.GetComponent<BlockController> ().row];

			} else {
				b.GetComponent<BlockController> ().neighborRight = null;
			}

			//UP NEIGHBOUR
			if (IsInRange (b.GetComponent<BlockController> ().column ,b.GetComponent<BlockController> ().row+1)) {
				b.GetComponent<BlockController> ().neighborUp = allBlocks[b.GetComponent<BlockController> ().column ,b.GetComponent<BlockController> ().row + 1];

			} else {
				b.GetComponent<BlockController> ().neighborUp = null;
			}
			//DOWN NEIGHBOUR
			if (IsInRange (b.GetComponent<BlockController> ().column ,b.GetComponent<BlockController> ().row-1)) {
				b.GetComponent<BlockController> ().neighborDown = allBlocks[b.GetComponent<BlockController> ().column ,b.GetComponent<BlockController> ().row - 1];

			} else {
				b.GetComponent<BlockController> ().neighborDown = null;
			}
		}
	}
	public bool IsInRange(int x, int y){
		if(x>=0 && x<width && y>=0 && y<height){
			return true;
		}else{
			return false;
		}
	}
}
