using System.Collections.Generic;
using UnityEngine;

public class Sequence : AINode
{
    public Sequence() : base()
    {

    }
    public Sequence(List<AINode> children) : base(children)
    {

    }

    public override NodeState Evaluate()
    {
        bool isAnyChildRunning = false;
        foreach (var node in children)
        {
            switch (node.Evaluate())
            {
                case NodeState.FAILURE:
                    state = NodeState.FAILURE; // Nếu có node con nào thất bại, trả về FAILURE
                    return state;
                case NodeState.SUCCESS:
                    continue; // Nếu có node con nào thành công, tiếp tục kiểm tra node tiếp theo
                case NodeState.RUNNING:
                    isAnyChildRunning = true; // Nếu có node con nào đang chạy, đánh dấu là đang chạy
                    break;
            }
        }
        state = isAnyChildRunning ? NodeState.RUNNING : NodeState.SUCCESS; // Nếu không có node con nào thất bại, trả về RUNNING hoặc SUCCESS
        return state; // Trả về trạng thái cuối cùng    
    }
}
