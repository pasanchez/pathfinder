using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoObstacleScript : Obstacle {

	override public bool doAction (Obstacle o){
		return false;
	}

	override public bool doOverAction (Obstacle o){
		return false;
	}
}
