using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class barrelScript : Obstacle {

	private bool hitted = false;
	private Vector3 targetPosition;
	private Vector3 deltaPosition;
	private float movStep = 0.1f;
	public bool moving = false;
	private float rotation = 5;

	void Update () {
		if (moving) {
			if (this.gridX != targetPosition.x || this.gridY != targetPosition.z) {
				if (deltaPosition.x > 0) {
					this.transform.RotateAround (this.transform.position, new Vector3 (0, 0, 1), -rotation);
				}
				if (deltaPosition.x < 0) {
					this.transform.RotateAround (this.transform.position, new Vector3 (0, 0, 1), +rotation);

				}
				if (deltaPosition.z > 0) {
					this.transform.RotateAround (this.transform.position, new Vector3 (1, 0, 0), -rotation);

				}
				if (deltaPosition.z < 0) {
					this.transform.RotateAround (this.transform.position, new Vector3 (1, 0, 0), rotation);

				}
				this.transform.position = this.transform.position + deltaPosition;
				this.gridX = Mathf.RoundToInt (this.transform.position.x);
				this.gridY = Mathf.RoundToInt (this.transform.position.z);
			} else {
				if (this.gridX >= controller.mapObstacles.GetLength (0)-1 ||
				    this.gridX < 0 ||
				    this.gridY >= controller.mapObstacles.GetLength (1)-1 ||
				    this.gridY < 0) {
					Destroy (this.me);
					return;
				}

				this.gridX = Mathf.RoundToInt (this.transform.position.x);
				this.gridY = Mathf.RoundToInt (this.transform.position.z);
				if (controller.mapObstacles [this.gridX, this.gridY].doOverAction (this))
					return;
				fixMe ();
				if (deltaPosition.x > 0) {
					//right
					if (this.gridX + 1 >= controller.mapObstacles.GetLength (0)) {
						targetPosition = new Vector3 (this.gridX + 2, this.transform.position.y, this.gridY);
						deltaPosition = new Vector3 (movStep, 0, 0);
					} else {
						Debug.Log ("do action");
						if (controller.mapObstacles [this.gridX + 1, this.gridY].doAction (this)) {
							Debug.Log ("should stop");
							this.transform.position = new Vector3 (this.gridX, this.transform.position.y, this.gridY);
							moving = false;
							return;
						} else {
							targetPosition = new Vector3 (this.gridX + 1, this.transform.position.y, this.gridY);
							deltaPosition = new Vector3 (movStep, 0, 0);

						}
					}

				}
				if (deltaPosition.x < 0) {
					//left
					if (this.gridX - 1 < 0) {
						targetPosition = new Vector3 (this.gridX - 1, this.transform.position.y, this.gridY);
						deltaPosition = new Vector3 (-movStep, 0, 0);
					} else {
						if (controller.mapObstacles [this.gridX - 1, this.gridY].doAction (this)) {
							this.transform.position = new Vector3 (this.gridX, this.transform.position.y, this.gridY);
							moving = false;
							return;
						} else {
							targetPosition = new Vector3 (this.gridX - 1, this.transform.position.y, this.gridY);
							deltaPosition = new Vector3 (-movStep, 0, 0);
						}
					}
				}
				if (deltaPosition.z > 0) {
					//up
					if (this.gridY + 1 >= controller.mapObstacles.GetLength (1)) {
						targetPosition = new Vector3 (this.gridX, this.transform.position.y, this.gridY + 1);
						deltaPosition = new Vector3 (0, 0, movStep);
					} else {
						if (controller.mapObstacles [this.gridX, this.gridY + 1].doAction (this)) {
							this.transform.position = new Vector3 (this.gridX, this.transform.position.y, this.gridY);
							moving = false;
							return;
						} else {
							targetPosition = new Vector3 (this.gridX, this.transform.position.y, this.gridY + 1);
							deltaPosition = new Vector3 (0, 0, movStep);
						}
					}
				}
				if (deltaPosition.z < 0) {
					//down
					if (this.gridY - 1 < 0) {
						targetPosition = new Vector3 (this.gridX, this.transform.position.y, this.gridY - 1);
						deltaPosition = new Vector3 (0, 0, -movStep);
					} else {
						if (controller.mapObstacles [this.gridX, this.gridY - 1].doAction (this)) {
							this.transform.position = new Vector3 (this.gridX, this.transform.position.y, this.gridY);
							moving = false;
							return;
						} else {
							targetPosition = new Vector3 (this.gridX, this.transform.position.y, this.gridY - 1);
							deltaPosition = new Vector3 (0, 0, -movStep);
						}
					}
				}
				leavePlace ();

			}
		} else {
			fixMe ();
		}
	}


	private void leavePlace(){
		GameObject go = Instantiate (controller.obstacleDict[' ']);
		go.transform.SetParent (GameObject.Find("Map").transform);
		Obstacle obstacle = go.GetComponent<Obstacle> ();
		controller.mapObstacles [this.gridX, this.gridY] = obstacle;
		go.transform.position = new Vector3 (this.gridX, 0,this.gridY);
		obstacle.gridX = (int)this.gridX;
		obstacle.gridY = (int)this.gridY;
		obstacle.type = ' ';
		obstacle.me = go;
		obstacle.controller = this.controller;
	}


	private void fixMe(){
		controller.mapObstacles [this.gridX, this.gridY] = this;
	}


	override public bool doAction(Obstacle o){
		Debug.Log ("Do action");
		if (hitted) {
			Debug.Log ("hitted");
			return true;
		}
		hitted = true;
		if (o.gridX < this.gridX) {
			this.transform.rotation = Quaternion.Euler(90,0,0);
			this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y+0.1f, this.transform.position.z-0.25f);
			if (!controller.mapObstacles [this.gridX + 1, this.gridY].doAction(this)) {
				moving = true;
				targetPosition = new Vector3 (this.gridX + 1,this.transform.position.y, this.gridY);
				deltaPosition = new Vector3 (movStep, 0, 0);
				leavePlace ();
			}
		}
		if (o.gridX > this.gridX) {
			this.transform.rotation = Quaternion.Euler(90,0,0);
			this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y+0.1f, this.transform.position.z-0.25f);
			if (!controller.mapObstacles [this.gridX - 1, this.gridY].doAction(this)) {
				moving = true;
				targetPosition = new Vector3 (this.gridX - 1,this.transform.position.y, this.gridY);
				deltaPosition = new Vector3 (-movStep, 0, 0);
				leavePlace ();
			}
		}
		if (o.gridY < this.gridY) {
			this.transform.rotation = Quaternion.Euler(0,0,90);
			this.transform.position = new Vector3 (this.transform.position.x-0.25f, this.transform.position.y+0.1f, this.transform.position.z);
			if (!controller.mapObstacles [this.gridX, this.gridY + 1].doAction(this)) {
				moving = true;
				targetPosition = new Vector3 (this.gridX,this.transform.position.y, this.gridY + 1);
				deltaPosition = new Vector3 (0, 0, movStep);
				leavePlace ();
			}
		}
		if (o.gridY > this.gridY) {
			this.transform.rotation = Quaternion.Euler(0,0,90);
			this.transform.position = new Vector3 (this.transform.position.x-0.25f, this.transform.position.y+0.1f, this.transform.position.z);
			if (!controller.mapObstacles [this.gridX, this.gridY - 1].doAction(this)) {
				moving = true;
				targetPosition = new Vector3 (this.gridX,this.transform.position.y, this.gridY - 1);
				deltaPosition = new Vector3 (0, 0, -movStep);
				leavePlace ();
			}
		}
		return true;
	}

	override public bool doOverAction (Obstacle o){
		return false;
	}



}
