using System.Collections.Generic;
using UnityEngine;

public class PathfindingNode {
    public int DistanceFromGoal = int.MaxValue;
    public int DistanceFromOrigin = int.MaxValue;
    public int DistanceValue = int.MaxValue;

    public PathfindingNode PreviousNode = null;

    public Vector3Int Position;

    private const int distanceValueMultiplier = 100;

    public static readonly Vector3Int[] ValidDirections = {
        new Vector3Int(-1, -1),
        new Vector3Int(0, -1),
        new Vector3Int(1, -1),

        new Vector3Int(-1, 0),
        new Vector3Int(1, 0),

        new Vector3Int(-1, 1),
        new Vector3Int(0, 1),
        new Vector3Int(1, 1),
    };

    //--------------------------------[[ Constructors ]]--------------------------------
    public PathfindingNode (Vector3 Goal, Vector3 Position) {
        Setup(Goal, Position);

        DistanceFromOrigin = 0;
    }
    public PathfindingNode(Vector3 Goal, Vector3 Position, PathfindingNode PreviousNode) {
        Setup(Goal, Position);
        this.PreviousNode = PreviousNode;

        DistanceFromOrigin = PreviousNode.DistanceFromOrigin + distanceValueMultiplier;
    }

    //--------------------------------[[ Functions ]]--------------------------------
    public override string ToString () {
        return $"PathfindingNode: (Distance from goal: {DistanceFromGoal}, Distance from origin: {DistanceFromOrigin}, Distance value: {GetDistanceValue()})";
    }

    private void Setup(Vector3 Goal, Vector3 Position) {
        DistanceFromGoal = (int) (Vector3.Distance(Goal, Position) * 100);
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
}