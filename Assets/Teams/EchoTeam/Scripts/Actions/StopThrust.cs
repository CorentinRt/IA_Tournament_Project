using BehaviorDesigner.Runtime.Tasks;

namespace Echo
{
    public class StopThrust : EchoAction
    {
        public override TaskStatus OnUpdate()
        {
            _echoController.GetInputDataByRef().thrust = 0;
            return TaskStatus.Success;
        }
    }
}

