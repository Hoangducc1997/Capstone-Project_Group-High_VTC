using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


// Node Status
public enum NodeState
{
    SUCCESS, //Trạng thái thành công
    FAILURE,
    RUNNING
}

public abstract class Node
{
    protected NodeState state; //Trạng thái của node
    public Node parent;
    protected List<Node> children = new List<Node>();

    public Node()
    {

    }

    public Node(List<Node> children)
    {
        foreach (var child in children)
        {
            Attack(child); //Thêm node con vào danh sách
        }
    }
    public void Attack(Node child)
    {
        this.children.Add(child);
        child.parent = this;
    }
    public abstract NodeState Evaluate(); // Phương thức đánh giá trạng thái của node
}

