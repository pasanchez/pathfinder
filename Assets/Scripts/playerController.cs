using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : Obstacle {

	private Vector3 targetPosition;
	private Vector3 deltaPosition;
	private float movStep = 0.1f;
	public bool moving = false;
	void Update () {
		if (moving) {
			if (this.transform.position != targetPosition) {
				this.transform.position = this.transform.position + deltaPosition;
				this.gridX = Mathf.RoundToInt(this.transform.position.x);
				this.gridY = Mathf.RoundToInt(this.transform.position.z);
			} else {
				if (this.gridX >= controller.mapObstacles.GetLength (0) ||
				    this.gridX < 0 ||
				    this.gridY >= controller.mapObstacles.GetLength (1) ||
				    this.gridY < 0) {
					controller.reloadLevel ();
					return;
				}
						
				this.gridX = Mathf.RoundToInt(this.transform.position.x);
				this.gridY = Mathf.RoundToInt(this.transform.position.z);
				if(controller.mapObstacles [this.gridX, this.gridY].doOverAction (this)) return;

				if (deltaPosition.x > 0) {
					//right
					if (this.gridX + 1 >= controller.mapObstacles.GetLength (0)) {
						targetPosition = new Vector3 (this.gridX + 1,this.transform.position.y, this.gridY);
						deltaPosition = new Vector3 (movStep, 0, 0);
					} else {
						Debug.Log (controller.mapObstacles [this.gridX + 1, this.gridY].type);
						if (controller.mapObstacles [this.gridX + 1, this.gridY].doAction(this)) {
							moving = false;
						} else {
							targetPosition = new Vector3 (this.gridX + 1,this.transform.position.y, this.gridY);
							deltaPosition = new Vector3 (movStep, 0, 0);

						}
					}

				}
				if (deltaPosition.x < 0) {
					//left
					if (this.gridX - 1 < 0) {
						targetPosition = new Vector3 (this.gridX - 1,this.transform.position.y, this.gridY);
						deltaPosition = new Vector3 (-movStep, 0, 0);
					} else {
						if (controller.mapObstacles [this.gridX - 1, this.gridY].doAction(this)) {
							moving = false;
						} else {
							targetPosition = new Vector3 (this.gridX - 1,this.transform.position.y, this.gridY);
							deltaPosition = new Vector3 (-movStep, 0, 0);
						}
					}
				}
				if (deltaPosition.z > 0) {
					//up
					if (this.gridY + 1 >= controller.mapObstacles.GetLength (1)) {
						targetPosition = new Vector3 (this.gridX,this.transform.position.y, this.gridY + 1);
						deltaPosition = new Vector3 (0, 0, movStep);
					} else {
						if (controller.mapObstacles [this.gridX, this.gridY + 1].doAction(this)) {
							moving = false;
						} else {
							targetPosition = new Vector3 (this.gridX,this.transform.position.y, this.gridY + 1);
							deltaPosition = new Vector3 (0, 0, movStep);
						}
					}
				}
				if (deltaPosition.z < 0) {
					//down
					if (this.gridY - 1 < 0) {
						targetPosition = new Vector3 (this.gridX,this.transform.position.y, this.gridY - 1);
						deltaPosition = new Vector3 (0, 0, -movStep);
					} else {
						if (controller.mapObstacles [this.gridX, this.gridY - 1].doAction(this)) {
							moving = false;
						} else {
							targetPosition = new Vector3 (this.gridX,this.transform.position.y, this.gridY - 1);
							deltaPosition = new Vector3 (0, 0, -movStep);
						}
					}
				}
			}
		} else {
			if (controller.noMoreEnergy())
				controller.reloadLevel ();
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				if (!controller.mapObstacles [this.gridX + 1, this.gridY].doAction(this)) {
					moving = true;
					controller.decreaseEnergy();
					this.transform.rotation = Quaternion.Euler (0, 180, 0);
					targetPosition = new Vector3 (this.gridX + 1,this.transform.position.y, this.gridY);
					deltaPosition = new Vector3 (movStep, 0, 0);
				}
			}
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				if (!controller.mapObstacles [this.gridX - 1, this.gridY].doAction(this)) {
					moving = true;
					controller.decreaseEnergy();
					this.transform.rotation = Quaternion.Euler (0, 0, 0);
					targetPosition = new Vector3 (this.gridX - 1,this.transform.position.y, this.gridY);
					deltaPosition = new Vector3 (-movStep, 0, 0);
				}
			}
			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				if (!controller.mapObstacles [this.gridX, this.gridY + 1].doAction(this)) {
					moving = true;
					controller.decreaseEnergy();
					this.transform.rotation = Quaternion.Euler (0, 90, 0);
					targetPosition = new Vector3 (this.gridX,this.transform.position.y, this.gridY + 1);
					deltaPosition = new Vector3 (0, 0, movStep);
				}
			}
			if (Input.GetKeyDown (KeyCode.DownArrow)) {
				if (!controller.mapObstacles [this.gridX, this.gridY - 1].doAction(this)) {
					moving = true;
					controller.decreaseEnergy();
					this.transform.rotation = Quaternion.Euler (0, 270, 0);
					targetPosition = new Vector3 (this.gridX,this.transform.position.y, this.gridY - 1);
					deltaPosition = new Vector3 (0, 0, -movStep);
				}
			}
		}
	}

	override public bool doAction(Obstacle o){
		return false;
	}

	override public bool doOverAction (Obstacle o){
		return false;
	}
}
