using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameController : MonoBehaviour {

	public GameObject floorPrefab;
	public Camera mainCamera;

	public float moveCameraArea = 0.95f;
	public float cameraSpeedMult = 0.3f;
	public float zoomSpeed = 0.5f;
	private int energyleft;
	public int maxEnergy;
	private GUI theGui;
	public GameObject canvas;
	//String to GameObeject representation
	public Dictionary<char,GameObject> obstacleDict;
	public char[] dictkeys;
	public GameObject[] dictObjects;

	public Obstacle[,] mapObstacles;
	public GameObject[,] floors;


	private string nextMap = "init.map"; 
	private string thisMap = "init.map";


	public void decreaseEnergy(){
		energyleft--;
		theGui.RedrawBats (energyleft);
	}

	public bool noMoreEnergy(){
		return (energyleft == 0);
	}

	void Start () {
		theGui = canvas.GetComponent<GUI> ();
		obstacleDict = new Dictionary<char,GameObject> ();
		if (dictObjects.Length == dictkeys.Length) {
			for(int i=0; i <dictkeys.Length;i++){
				obstacleDict.Add(char.ToUpper(dictkeys [i]),dictObjects [i]);
			}
		}
		loadMap (nextMap);
 	}

	void Update () {

		Vector2 pos = mainCamera.ScreenToViewportPoint (Input.mousePosition);
		Vector3 newPos = new Vector3 ();
		newPos = mainCamera.transform.position;
		if (pos.x > moveCameraArea) {
			newPos.x += cameraSpeedMult*(pos.x-moveCameraArea);
		}
		if (pos.x < (1-moveCameraArea)){
			newPos.x -= cameraSpeedMult*((1-moveCameraArea)-pos.x);
		}
		if (pos.y > moveCameraArea){
			newPos.z += cameraSpeedMult*(pos.y-moveCameraArea);
		}
		if (pos.y < (1-moveCameraArea)){
			newPos.z -= cameraSpeedMult*((1-moveCameraArea)-pos.y);
		}

		var wheelAxis = Input.GetAxis("Mouse ScrollWheel");
		if (wheelAxis > 0f){
			newPos.y -= zoomSpeed;
		}
		else if (wheelAxis < 0f){
			newPos.y += zoomSpeed;
		}

		mainCamera.transform.position = newPos;
	}


	private void generateFloor(int rows, int cols){
		mapObstacles = new Obstacle[cols,rows];
		floors = new GameObject[cols,rows];
		GameObject parent = new GameObject ("floor");
		for (float i = 0; i < cols; i++) {
			for (float j = 0; j < rows; j++) {
				Vector3 pos = new Vector3 (i, -0.5f, j);
				GameObject f = Instantiate (floorPrefab, pos, Quaternion.identity,parent.transform);
				floors [(int)i, (int)j] = f;
			}
		}
//		fix outter edge:

		//first row:

		for (int i = 1; i < cols - 1; i++) {
			Transform t = floors [i, 0].transform;
			t.localScale = new Vector3 (t.localScale.x,t.localScale.y,0.7f);
			t.position = new Vector3(t.position.x,t.position.y,t.position.z+0.15f);
		}

		//last row
		for (int i = 1; i < cols - 1; i++) {
			Transform t = floors [i,rows-1].transform;
			t.localScale = new Vector3 (t.localScale.x,t.localScale.y,0.7f);
			t.position = new Vector3(t.position.x,t.position.y,t.position.z-0.15f);
		}

		//first col
		for (int j = 1; j < rows - 1; j++) {
			Transform t = floors [0, j].transform;
			t.localScale = new Vector3 (0.7f,t.localScale.y,t.localScale.z);
			t.position = new Vector3(t.position.x+0.15f,t.position.y,t.position.z);
		}

		//last col
		for (int j = 1; j < rows - 1; j++) {
			Transform t = floors [cols-1,j].transform;
			t.localScale = new Vector3 (0.7f,t.localScale.y,t.localScale.z);
			t.position = new Vector3(t.position.x-0.15f,t.position.y,t.position.z);
		}

		//corners
		Transform trans = floors [0,0].transform;
		trans.localScale = new Vector3 (0.7f,trans.localScale.y,0.7f);
		trans.position = new Vector3(trans.position.x+0.15f,trans.position.y,trans.position.z+0.15f);
		trans = floors [0,rows-1].transform;
		trans.localScale = new Vector3 (0.7f,trans.localScale.y,0.7f);
		trans.position = new Vector3(trans.position.x+0.15f,trans.position.y,trans.position.z-0.15f);
		trans = floors [cols-1,0].transform;
		trans.localScale = new Vector3 (0.7f,trans.localScale.y,0.7f);
		trans.position = new Vector3(trans.position.x-0.15f,trans.position.y,trans.position.z+0.15f);
		trans = floors [cols-1,rows-1].transform;
		trans.localScale = new Vector3 (0.7f,trans.localScale.y,0.7f);
		trans.position = new Vector3(trans.position.x-0.15f,trans.position.y,trans.position.z-0.15f);

	}

	private void generateMap(string[] obstacles){
		GameObject parent = new GameObject ("Map");
		int rows = obstacles.GetLength (0);
		int cols = obstacles[0].Length;
		generateFloor (rows, cols);
		for (float i = 0; i < cols; i++) {
			for (float j = 0; j < rows; j++) {
				char objectChar = char.ToUpper(obstacles [obstacles.GetLength(0)-(int)j-1] [(int)i]);

				GameObject prefab;
				if (obstacleDict.ContainsKey (objectChar)) {
					prefab = obstacleDict [objectChar];
				} else {
					prefab = obstacleDict [' ']; 
				}
				GameObject go = Instantiate (prefab);
				go.transform.SetParent (parent.transform);
				Obstacle obstacle = go.GetComponent<Obstacle> ();
				mapObstacles [(int)i, (int)j] = obstacle;
				go.transform.position = new Vector3 (i, 0,j);
				obstacle.gridX = (int)i;
				obstacle.gridY = (int)j;
				obstacle.type = objectChar;
				obstacle.me = go;
				obstacle.controller = this;
			}
		}
		joinWalls ();
	}


	private void joinWalls() {
		for (float i = 0; i < mapObstacles.GetLength (0); i++) {
			for (float j = 0; j < mapObstacles.GetLength (1); j++) {
				Obstacle obstacle = mapObstacles [(int)i,(int)j];
				if (obstacle != null) {
					if (obstacle.type == 'W') {
						if (i > 0) {
							Obstacle other = mapObstacles [(int)i-1,(int)j];
							if (other.type == 'W') {
								Transform o = obstacle.me.transform.FindChild ("x-");
								o.GetComponent<Renderer>().enabled = true;
							}
						}
						if (i <  mapObstacles.GetLength (0)-1) {
							Obstacle other = mapObstacles [(int)i+1,(int)j];
							if (other.type == 'W') {
								Transform o = obstacle.me.transform.FindChild ("x+");
								o.GetComponent<Renderer>().enabled = true;
							}
						}
						if (j > 0) {
							Obstacle other = mapObstacles [(int)i,(int)j-1];
							if (other.type == 'W') {
								Transform o = obstacle.me.transform.FindChild ("z-");
								o.GetComponent<Renderer>().enabled = true;
							}
						}
						if (j < mapObstacles.GetLength (1)-1) {
							Obstacle other = mapObstacles [(int)i,(int)j+1];
							if (other.type == 'W') {
								Transform o = obstacle.me.transform.FindChild ("z+");
								o.GetComponent<Renderer>().enabled = true;
							}
						}
					}
				}
			}
		}
	
	}

	private void loadMap(string filename){
		thisMap = filename;
		StreamReader reader = new StreamReader ("maps/"+filename);
		List<string> map = new List<string>();
		using (reader) {
			energyleft = int.Parse (reader.ReadLine ());
			maxEnergy = energyleft;
			nextMap = reader.ReadLine ();
			do {
				//string row = reverseString(reader.ReadLine());
				string row = reader.ReadLine();
				Debug.Log(row);
				map.Add(row);			
			} while(!reader.EndOfStream);
			reader.Close ();
		}
		//map.Reverse ();
		string[] output = map.ToArray ();

		theGui.RedrawBats (energyleft);
		generateMap (output);
	}

	private void cleanup(){
		foreach (Obstacle o in mapObstacles) {
			Destroy (o.me);
		}

		foreach (GameObject go in floors) {
			Destroy (go);
		}

		GameObject map = GameObject.Find ("Map");
		Destroy (map);
		if (nextMap == "END") {
			Debug.Log ("YOU WIN");
			return;
		}
	}
	public void nextLevel(){
		cleanup ();
		loadMap (nextMap);
	}
	public void reloadLevel(){
		cleanup ();
		loadMap (thisMap);
	}

	private string reverseString( string s )
	{
		char[] charArray = s.ToCharArray();
		System.Array.Reverse (charArray);
		return new string( charArray );
	}
	void OnDrawGizmos() {
		if (mapObstacles == null)
			return;
		for (int i = 0; i < mapObstacles.GetLength (0); i++) {
			for (int j = 0; j < mapObstacles.GetLength (1); j++) {
				Obstacle o = mapObstacles [i, j];
				if (o.type == ' ')	
					Gizmos.color = new Color (1,1,1,1);
				if (o.type == 'W')	
					Gizmos.color = new Color (0,0,0,1);
				if (o.type == 'B')	
					Gizmos.color = new Color (0,0,1,1);
				if (o.type == 'L')	
					Gizmos.color = new Color (1,0,0,1);
				if (o.type == 'E')	
					Gizmos.color = new Color (0,1,0,1);
				if (o.type == 'P')	
					Gizmos.color = new Color (0,1,1,1);
				Gizmos.DrawCube (new Vector3 (o.gridX, 2, o.gridY), new Vector3 (0.2f, 0.2f, 0.2f));
			}
		}
	}
	
}
