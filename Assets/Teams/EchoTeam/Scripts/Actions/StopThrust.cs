using BehaviorDesigner.Runtime.Tasks;

namespace Echo
{
    public class StopThrust : EchoAction
    {
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

