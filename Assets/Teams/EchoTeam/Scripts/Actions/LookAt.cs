using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Echo
{
    public class LookAt : EchoAction
    {
        public SharedFloat targetOrientation;

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

