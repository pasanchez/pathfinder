using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Obstacle: MonoBehaviour{
	public int gridX;
	public int gridY;
	public char type;
	public GameObject me;
	public GameController controller;

	public abstract bool doAction (Obstacle o);
	public abstract bool doOverAction(Obstacle o);
}
