using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Astar2DPathFinding.Mika {

    public class Node : IHeapItem<Node> {
        public NodeType walkable;
        public Vector3 worldPosition;
        public int gridX;
        public int gridY;
        public int movementPenalty;

        public bool inClosedList;
        public bool inOpenSet;


        public int gCost;
        public int hCost;
        public int fCost {
            get {
                return gCost + hCost;
            }
        }

        public Node parent;
        private int heapIndex;
        public int HeapIndex {
            get {
                return heapIndex;
            }
            set {
                heapIndex = value;
            }
        }
        public Node[] neighbours = new Node[8];

        public Node(NodeType _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _penalty) {
            walkable = _walkable;
            worldPosition = _worldPos;
            gridX = _gridX;
            gridY = _gridY;
            movementPenalty = _penalty;
        }

        public int CompareTo(Node nodeToCompare) {
            int compare = fCost.CompareTo(nodeToCompare.fCost);
            if (compare == 0) {
                compare = hCost.CompareTo(nodeToCompare.hCost);
            }
            return -compare;
        }
    }

    public enum NodeType {
        obstacle,
        walkable
    }
}

