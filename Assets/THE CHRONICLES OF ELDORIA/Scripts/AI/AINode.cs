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

public abstract class AINode
{
    protected NodeState state; //Trạng thái của node
    public AINode parent;
    protected List<AINode> children = new List<AINode>();

    public AINode()
    {

    }

    public AINode(List<AINode> children)
    {
        foreach (var child in children)
        {
            Attack(child); //Thêm node con vào danh sách
        }
    }
    public void Attack(AINode child)
    {
        this.children.Add(child);
        child.parent = this;
    }
    public abstract NodeState Evaluate(); // Phương thức đánh giá trạng thái của node
}

