using BehaviorDesigner.Runtime.Tasks;

namespace Echo
{
    [TaskCategory("Echo")]
    public class StopThrust : Action
    {
        private EchoController _echoController;
        
        public override void OnAwake()
        {
            base.OnAwake();
            _echoController = GetComponent<EchoController>();
        }
        
        public override void OnStart()
        {
            _echoController.GetInputDataByRef().thrust = 0;
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}

