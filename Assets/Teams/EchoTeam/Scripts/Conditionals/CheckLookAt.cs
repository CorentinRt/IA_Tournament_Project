using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Echo;
using UnityEngine;

namespace Echo
{
    public class CheckLookAt : EchoConditional
    {
        public SharedFloat tolerance;
        
        public override TaskStatus OnUpdate()
        {
            if(Mathf.Abs(_echoController.GetInputDataByRef().targetOrientation - _echoData.GetOurSpaceship().Orientation) <= tolerance.Value) 
                return TaskStatus.Success;
            
            return TaskStatus.Failure;
        }
    }
}

