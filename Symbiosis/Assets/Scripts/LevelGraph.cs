using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum NodeColor {White, Grey, Black};

public class LevelGraph {

	public string player; // ???
	private int maxDistance;
	public int MaxDistance {
		get{return maxDistance;}
	}

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
	private List<Edge> removedDoorList;
	public List<Edge> RemovedDoorList {
		get{return removedDoorList;}
	}


	public LevelGraph(string player) {
    	this.player = player;
    	roomList = new List<Node>();
    	doorList = new List<Edge>();
		removedDoorList = new List<Edge>();
    }

	public void RemoveEdge(Edge edge) {
		doorList.Remove(edge);
		removedDoorList.Add(edge);
		Node room1 = edge.door1.RoomInside;
		Node room2 = edge.door2.RoomInside;
		room1.AdjacentRooms.Remove(room2);
		room2.AdjacentRooms.Remove(room1);
	}

    public Node GetRoomByNumber(int roomNumber) {
		foreach (Node room in RoomList) {
			if (room.RoomNumber == roomNumber) {
				return room;
			}
		}
		return null;
    }

    public Edge GetEdgeBetweenRooms(Node room1, Node room2) {
		foreach (Edge edge in doorList) {
			Node connectingRoom1 = edge.door1.RoomInside;
			Node connectingRoom2 = edge.door2.RoomInside;
			if ((connectingRoom1 == room1 && connectingRoom2 == room2) 
					|| (connectingRoom1 == room2 && connectingRoom2 == room1)) {
				return edge;
			}
		}
		return null;
    }

    public void AddAllAdjacentRooms() {
		foreach (Edge edge in doorList) {
			Node room1 = edge.door1.RoomInside;
			Node room2 = edge.door2.RoomInside;
			room1.AddAdjacentRoom(room2);
			room2.AddAdjacentRoom(room1);
		}
	}

	public void OldCreateCriticalPath(System.Random pseudoRandom, bool levelOnRightSide) {
		List<Edge> criticalEdges = new List<Edge>();
		string restrictedDirection = "West";
		if (levelOnRightSide) {
			restrictedDirection = "East";
		}

		tutorialAdjRoom.Color = NodeColor.Black;
		Queue<Node> nodeQueue = new Queue<Node>();
		nodeQueue.Enqueue(tutorialAdjRoom);
		while (nodeQueue.Count != 0) {
			Node exploringNode = nodeQueue.Dequeue();
			exploringNode.Color = NodeColor.Black;

			// Switch to 2 restricted directions
			if (false && exploringNode != switchAdjRoom) {
				// int randomNextRoomIndex;
				// Node randomNextRoom;
				// Edge connectingNode;
				// string doorDirection;
				// do {
				// 	randomNextRoomIndex = pseudoRandom.Next(exploringNode.AdjacentRooms.Count);
				// 	randomNextRoom = exploringNode.AdjacentRooms[randomNextRoomIndex];

				// 	connectingNode = GetEdgeBetweenRooms(exploringNode, randomNextRoom);
				// 	doorDirection = connectingNode.GetDoorDirection(exploringNode);
				// } while (randomNextRoom.Color == NodeColor.Black || doorDirection == restrictedDirection);

				// criticalEdges.Add(connectingNode);
				// nodeQueue.Enqueue(randomNextRoom);
			}

		}

		removedDoorList = doorList.Except(criticalEdges).ToList();
		doorList = criticalEdges;
	}

	public void GenerateLevel(System.Random pseudoRandom) {

	}

	public bool CanReachSwitchRoom() {
		tutorialAdjRoom.Color = NodeColor.Grey;
		bool reached = CanReachSwitchRoomHelper(tutorialAdjRoom);
		ResetNodeColors();
		return reached;
	}

	public bool CanReachSwitchRoomHelper(Node rootNode) {
		bool reached = false;
		rootNode.Color = NodeColor.Grey;
		foreach (Node node in rootNode.AdjacentRooms) {
			if (reached) break;
			if (node.Color == NodeColor.White) {
				if (node == switchAdjRoom) {
					reached = true;
				} else {
					reached = (reached || CanReachSwitchRoomHelper(node));
				}
			}
		}
		return reached;
	}

	public void SingleRoomIsolate(int roomNum) {
		Node room = GetRoomByNumber(roomNum);
		SingleRoomIsolate(room);
	}

	public void SingleRoomIsolate(Node room) {
		foreach (Edge edge in ReturnAllRoomEdges(room)) {
			RemoveEdge(edge);
		}
	}

	public void RandomSingleRoomIsolate(System.Random pseudoRandom) {
		Node isolatedNode;
		int indexToRemove;
		do {
			indexToRemove = pseudoRandom.Next(roomList.Count);
			isolatedNode = roomList[indexToRemove];
		} while (isolatedNode == SwitchAdjRoom || isolatedNode == TutorialAdjRoom);

		foreach (Edge edge in ReturnAllRoomEdges(isolatedNode)) {
			RemoveEdge(edge);
		}
	}

	public List<Edge> ReturnAllRoomEdges(Node room) {
		List<Edge> roomEdges = new List<Edge>();
		foreach (Edge edge in doorList) {
			if (edge.door1.RoomInside == room || edge.door2.RoomInside == room) {
				roomEdges.Add(edge);
			}
		}
		return roomEdges;
	}

	public List<Node> GetIsolatedRooms() {
		List<Node> reachableRooms = new List<Node>();

		tutorialAdjRoom.Color = NodeColor.Black;
		Queue<Node> nodeQueue = new Queue<Node>();
		nodeQueue.Enqueue(tutorialAdjRoom);

		while (nodeQueue.Count != 0) {
			Node exploringNode = nodeQueue.Dequeue();
			reachableRooms.Add(exploringNode);
			foreach (Node adjNode in exploringNode.AdjacentRooms) {
				if (adjNode.Color == NodeColor.White) {
					adjNode.Color = NodeColor.Grey;
					nodeQueue.Enqueue(adjNode);
				}
			}
			exploringNode.Color = NodeColor.Black;
		}

		ResetNodeColors();

		return roomList.Except(reachableRooms).ToList();
	}

	public void ResetNodeColors() {
		foreach (Node node in roomList) {
			node.Color = NodeColor.White;
		}
	}

	public void CalculateRoomDistances() {
		switchAdjRoom.Color = NodeColor.Black;
		switchAdjRoom.Distance = 1;
		maxDistance = 1;

		Queue<Node> nodeQueue = new Queue<Node>();
		nodeQueue.Enqueue(switchAdjRoom);

		while (nodeQueue.Count != 0) {
			Node exploringNode = nodeQueue.Dequeue();
			maxDistance = (exploringNode.Distance > maxDistance ? exploringNode.Distance : maxDistance);
			foreach (Node adjNode in exploringNode.AdjacentRooms) {
				if (adjNode.Color == NodeColor.White) {
					adjNode.Color = NodeColor.Grey;
					adjNode.Distance = exploringNode.Distance + 1;
					nodeQueue.Enqueue(adjNode);
				}
			}
			exploringNode.Color = NodeColor.Black;
		}
		maxDistance++;
	}


	public void Print() {
		Debug.Log(player + ": ");
		Debug.Log("Farthest room is " + maxDistance + " rooms away from the switch room");
		Debug.Log("Tutorial Room: " + tutorialAdjRoom.RoomNumber + "  (" + tutorialAdjRoom.Distance + " rooms away from switch room)");
		Debug.Log("Switch Room: " + switchAdjRoom.RoomNumber + "  (" + switchAdjRoom.Distance + " rooms away from switch room)");
		foreach (Node coreRoom in roomList) {
			Debug.Log("-- Room: " + coreRoom.RoomNumber + "  (" + coreRoom.Distance + " rooms away from switch room)");
			PrintEdges(coreRoom);
		}
		Debug.Log("------------------------------------------------");
	}
	public void PrintEdges(Node node) {
		foreach (Node adj in node.AdjacentRooms) {
			Debug.Log("      - Door to: " + adj.RoomNumber);
		}
	}
}

public class Node {

	private List<Node> adjacentRooms;
	public List<Node> AdjacentRooms {
		get{return adjacentRooms;}
	}

	private GameObject roomObject;
	public GameObject RoomObject {
		get{return roomObject;}
		set{roomObject = value;}
	}
	private int roomNumber;
	public int RoomNumber {
		get{return roomNumber;}
	}
	private int distance;
	public int Distance {
		get{return distance;}
		set{distance = value;}
	}

	private NodeColor color;
	public NodeColor Color {
		get{return color;}
		set{color = value;}
	}

	public Node(int roomNumber) {
		this.roomNumber = roomNumber;
		this.distance = 0;
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

	public string GetDoorDirection(Node room) {
		if (room == door1.RoomInside) {
			return door1.Direction;
		} else if (room == door2.RoomInside) {
			return door2.Direction;
		}
		return null;
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