using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boxScript : Obstacle {

	private bool broken = false;

	override public bool doAction (Obstacle o){
		//break it!
		Transform w = this.transform.FindChild("Cube");
		w.GetComponent<MeshRenderer> ().enabled = false;
		Transform b = this.transform.FindChild("broken");
		MeshRenderer[] renderers = b.GetComponentsInChildren<MeshRenderer> ();
		foreach (MeshRenderer m in renderers) {
			m.enabled = true;
		}
		return false;
	}

	override public bool doOverAction (Obstacle o){
		if (o.GetType () == typeof(playerController)) {
			playerController player = (playerController)o;
			if (!broken) {
				player.moving = false;
				broken = true;
				return true;
			} else {
				return false;
			}
		}
		if (o.GetType () == typeof(barrelScript)) {
			barrelScript barrel = (barrelScript)o;
			if (!broken) {
				barrel.moving = false;
				broken = true;
				return true;
			} else {
				return false;
			}
		}
		return true;
	}
}
