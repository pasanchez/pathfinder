using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exitScript : Obstacle {

	override public bool doAction (Obstacle o){
		return false;
	}

	override public bool doOverAction (Obstacle o){
		controller.nextLevel ();
		if (o.GetType() == typeof(playerController))
			((playerController)o).moving = false;
		return true;
	}
}
