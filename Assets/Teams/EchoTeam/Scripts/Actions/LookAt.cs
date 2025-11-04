using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Echo
{
    [TaskCategory("Echo")]
    public class LookAt : Action
    {
        public SharedFloat targetOrientation;
        private EchoController _echoController;

        public override void OnAwake()
        {
            base.OnAwake();
            _echoController = GetComponent<EchoController>();
        
        }

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

