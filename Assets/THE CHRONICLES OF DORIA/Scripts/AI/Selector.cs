using System.Collections.Generic;
using UnityEngine;

public class Selector : AINode
{
    public Selector() : base()
    {

    }

    public Selector(List<AINode> children) : base(children)
    {

    }
    public override NodeState Evaluate()
    {
        foreach ( var node in children)
        {
            switch (node.Evaluate())
            {
                case NodeState.FAILURE: continue;
                case NodeState.SUCCESS: 
                    state = NodeState.SUCCESS; // Nếu có node con nào thành công, trả về SUCCESS
                    return state;
                case NodeState.RUNNING:
                    state = NodeState.RUNNING; // Nếu có node con nào đang chạy, trả về RUNNING
                    return state;
            }
        }
        state = NodeState.FAILURE; // Nếu không có node con nào thành công, trả về FAILURE
        return state; 
    }
}
