using UnityEngine;
using System.Collections;

public class LevelGraph {

	public string player;
	public Node tutorialAdjRoom;
	public Node switchAdjRoom;
	public Node[] roomList;
	public Vertex[] doorList;

}

public class Node {

	public int roomNumber;
	public int difficulty;
	// public string color;

}

public class Vertex {

	public Door door1;
	public Door door2;

}

public class Door {

	public int roomNumber;
	public string direction;

}