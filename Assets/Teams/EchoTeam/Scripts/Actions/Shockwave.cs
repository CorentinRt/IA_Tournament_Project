using BehaviorDesigner.Runtime.Tasks;

namespace Echo
{
    [TaskCategory("Echo")]
    public class Shockwave : EchoAction
    {
        public override TaskStatus OnUpdate()
        {
            if (_echoController == null)
            {
                UnityEngine.Debug.LogError("Couldn't find echo controller in shockwave.");
                return TaskStatus.Failure;
            }

            _echoController.GetInputDataByRef().fireShockwave = true;

            return TaskStatus.Success;
        }
    }
}
