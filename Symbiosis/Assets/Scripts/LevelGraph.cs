using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum NodeColor {White, Grey, Black};

public class LevelGraph {

	// public static enum Color {White, Grey, Black};

	public string player; // ???
	public int maxDifficulty;

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

    public Node GetRoomByNumber(int roomNumber) {
		foreach (Node room in RoomList) {
			if (room.RoomNumber == roomNumber) {
				return room;
			}
		}
		return null;
    }

    public void AddDoor(Edge newDoor) {
    	doorList.Add(newDoor);
    }

    public void Print() {
    	Debug.Log(player + ": ");
		Debug.Log("Tutorial Room: " + tutorialAdjRoom.RoomNumber);
		Debug.Log("Switch Room: " + switchAdjRoom.RoomNumber);
    	foreach (Node coreRoom in roomList) {
			Debug.Log("-- Room: " + coreRoom.RoomNumber);
    		PrintEdges(coreRoom);
    	}
		Debug.Log("------------------------------------------------");
    }
    public void PrintEdges(Node node) {
		foreach (Node adj in node.adjacentRooms) {
			Debug.Log("      - Door to: " + adj.RoomNumber);
		}
    }

    public void AddAllAdjacentRooms() {
		foreach (Edge edge in doorList) {
			Node room1 = edge.door1.RoomInside;
			Node room2 = edge.door2.RoomInside;
			room1.AddAdjacentRoom(room2);
			room2.AddAdjacentRoom(room1);
		}
	}

}

public class Node {

	public List<Node> adjacentRooms;

	private int roomNumber;
	public int RoomNumber {
		get{return roomNumber;}
	}
	private int difficulty;
	public int Difficulty {
		get{return difficulty;}
		set{difficulty = value;}
	}

	public NodeColor color;

	public Node(int roomNumber) {
		this.roomNumber = roomNumber;
		this.color = NodeColor.White;
		adjacentRooms = new List<Node>();
    }

	public void AddAdjacentRoom(Node newRoom) {
		adjacentRooms.Add(newRoom);
    }

}

public class Edge {

	public Door door1;
	public Door door2;

	public Edge(Door door1, Door door2) {
    	this.door1 = door1;
    	this.door2 = door2;
    }

	public Edge(Node roomNode1, string direction1, Node roomNumber2) {
		Door door1 = new Door(roomNode1, direction1);

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

    public Edge(Node roomNode1, string direction1, Node roomNode2, string direction2) {
		Door door1 = new Door(roomNode1, direction1);
		Door door2 = new Door(roomNode2, direction2);
    	this.door1 = door1;
    	this.door2 = door2;
    }

}

public class Door {

	private Node roomInside;
	public Node RoomInside {
		get{return roomInside;}
	}
	private string direction;
	public string Direction {
		get{return direction;}
	}

	public Door(Node roomInside, string direction) {
		this.roomInside = roomInside;
    	this.direction = direction;
    }

}