using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Echo
{
    public class LookAt : EchoAction
    {
        public SharedFloat targetOrientation;

        public override TaskStatus OnUpdate()
        {
            _echoController.GetInputDataByRef().targetOrientation  = targetOrientation.Value;
            return TaskStatus.Success;
        }
    } 
}

