using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Echo
{
    public class LookAt : EchoAction
    {
        public SharedFloat targetOrientation;
        public override void OnStart()
        {
            _echoController.GetInputDataByRef().targetOrientation  = targetOrientation.Value;
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    } 
}

