using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGraph {

	public string player; // ???

	private Node tutorialAdjRoom;
	public Node TutorialAdjRoom {
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
	private List<Edge> doorList;
	public List<Edge> DoorList {
		get{return doorList;}
	}

	public LevelGraph(string player) {
    	this.player = player;
    	roomList = new List<Node>();
    	doorList = new List<Edge>();
    }

    public void AddRoom(Node newRoom) {
    	roomList.Add(newRoom);
    }

    public void AddDoor(Edge newDoor) {
    	doorList.Add(newDoor);
    }

    public void Print() {
    	Debug.Log(player + ": ");
    	Debug.Log("Tutorial Room: " + tutorialAdjRoom.roomNumber);
    	PrintEdges(tutorialAdjRoom);
    	Debug.Log("Switch Room: " + switchAdjRoom.roomNumber);
    	PrintEdges(switchAdjRoom);
    	foreach (Node coreRoom in roomList) {
    		Debug.Log("Room: " + coreRoom.roomNumber);
    		PrintEdges(coreRoom);
    	}
    }

    public void PrintEdges(Node node) {
    	int roomNumber = node.roomNumber;
    	foreach (Edge edge in doorList) {
    		int door1Num = edge.door1.RoomNumber;
    		int door2Num = edge.door2.RoomNumber;
    		if (door1Num == roomNumber) {
    			Debug.Log("   Door: " + door1Num + " to " + door2Num);
    		} else if (door2Num == roomNumber) {
    			Debug.Log("   Door: " + door2Num + " to " + door1Num);
    		}
    	}
    }

}

public class Node {

	public int roomNumber;
	public int difficulty;

	public enum Color {White, Grey, Black}
	public Color color;

	public Node(int roomNumber) {
    	this.roomNumber = roomNumber;
    	this.color = Color.White;
    }

}

public class Edge {

	public Door door1;
	public Door door2;

	public Edge(Door door1, Door door2) {
    	this.door1 = door1;
    	this.door2 = door2;
    }

	public Edge(int roomNumber1, string direction1, int roomNumber2) {
    	Door door1 = new Door(roomNumber1, direction1);

    	string direction2 = "";
    	if (direction1 == "East") {
			direction2 = "West";
		} else if (direction1 == "West") {
			direction2 = "East";
		} else if (direction1 == "North") {
			direction2 = "South";
		} else {
			direction2 = "North";
		}
    	Door door2 = new Door(roomNumber2, direction2);

    	this.door1 = door1;
    	this.door2 = door2;
    }

    public Edge(int roomNumber1, string direction1, int roomNumber2, string direction2) {
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