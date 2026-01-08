using Mono.Cecil;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using UnityEngine;

public class PathfindingNode {
    public int DistanceFromGoal = int.MaxValue;
    public int DistanceFromOrigin = int.MaxValue;
    public int DistanceValue = int.MaxValue;

    public PathfindingNode PreviousNode = null;

    public Vector3Int Position;

    private const int distanceFromOriginMultiplier = 100;

    public static readonly Vector3Int[] ValidDirections = {
        //new Vector3Int(-1, -1),
        new Vector3Int(0, -1),
        //new Vector3Int(1, -1),

        new Vector3Int(-1, 0),
        new Vector3Int(1, 0),

        //new Vector3Int(-1, 1),
        new Vector3Int(0, 1),
        //new Vector3Int(1, 1),
    };

    //--------------------------------[[ Constructors ]]--------------------------------
    public PathfindingNode (Vector3 Goal, Vector3 Position) {
        DistanceFromOrigin = 0;

        Setup(Goal, Position);
    }
    public PathfindingNode(Vector3 Goal, Vector3 Position, PathfindingNode PreviousNode) {
        DistanceFromOrigin = PreviousNode.DistanceFromOrigin + distanceFromOriginMultiplier;

        Setup(Goal, Position);

        this.PreviousNode = PreviousNode;

    }

    //--------------------------------[[ Functions ]]--------------------------------

    private void Setup(Vector3 Goal, Vector3 Position) {
        DistanceFromGoal = (int) ((math.abs(Goal.x - Position.x) + math.abs(Goal.y - Position.y)) * 100);
        DistanceValue = DistanceFromGoal + DistanceFromOrigin;

        this.Position = Vector3Int.FloorToInt(Position);
    }

    public Stack<PathfindingNode> CollapseNodes () {
        Stack<PathfindingNode> stack = new();

        CollapseNodes(stack);

        return stack;
    }

    private Stack<PathfindingNode> CollapseNodes (Stack<PathfindingNode> stack) {
        stack.Push(this);

        if (PreviousNode != null) {
            PreviousNode.CollapseNodes(stack);
        }

        return stack;
    }

    //--------------------------------[[ Overrides ]]--------------------------------

    public override bool Equals (object obj) {
        return Position == ((PathfindingNode) obj).Position;
    }

    public override string ToString () {
        return $"PathfindingNode: (Position: {Position}, Distance from goal: {DistanceFromGoal}, Distance from origin: {DistanceFromOrigin}, Distance value: {DistanceValue})";
    }

    public override int GetHashCode () {
        int hash = 13;
        
        hash = (hash * 7) + Position.x;
        hash = (hash * 7) + Position.y;
        
        return hash;
    }
}

//public class PathfindingNodeComparer : IComparer<PathfindingNode> {
//    public int Compare(PathfindingNode node1, PathfindingNode node2) {
//        return node1.DistanceValue.CompareTo(node2.DistanceValue);
//    }
//}