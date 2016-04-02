using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGraph {

	public string player;

	public Node tutorialAdjRoom;
	private Node TutorialAdjRoom {
		get{return tutorialAdjRoom;}
		set{tutorialAdjRoom = value;}
	}
	private Node switchAdjRoom;
	public Node SwitchAdjRoom {
		get{return switchAdjRoom;}
		set{switchAdjRoom = value;}
	}

	private List<Node> roomList;
	public List<Node> RoomList {
		get{return roomList;}
	}
	private List<Vertex> doorList;
	public List<Vertex> DoorList {
		get{return doorList;}
	}

	public LevelGraph(string player) {
    	this.player = player;
    	roomList = new List<Node>();
    	doorList = new List<Vertex>();
    }

    public void AddRoom(Node newRoom) {
    	roomList.Add(newRoom);
    }

    public void AddDoor(Vertex newDoor) {
    	doorList.Add(newDoor);
    }

}

public class Node {

	public int roomNumber;
	public int difficulty;
	// public string color;

	public Node(int roomNumber) {
    	this.roomNumber = roomNumber;
    }

}

public class Vertex {

	public Door door1;
	public Door door2;

	public Vertex(Door door1, Door door2) {
    	this.door1 = door1;
    	this.door2 = door2;
    }

    public Vertex(int roomNumber1, string direction1, int roomNumber2, string direction2) {
    	Door door1 = new Door(roomNumber1, direction1);
    	Door door2 = new Door(roomNumber2, direction2);
    	this.door1 = door1;
    	this.door2 = door2;
    }

}

public class Door {

	private int roomNumber;
	public int RoomNumber {
		get{return roomNumber;}
	}
	private string direction;
	public string Direction {
		get{return direction;}
	}

	public Door(int roomNumber, string direction) {
    	this.roomNumber = roomNumber;
    	this.direction = direction;
    }

}