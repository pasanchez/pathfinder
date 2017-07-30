using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wall :Obstacle {

	override public bool doAction (Obstacle o){
		return true;
	}

	override public bool doOverAction (Obstacle o){
		return false;
	}
}
