using BehaviorDesigner.Runtime.Tasks;

namespace Echo
{
    [TaskCategory("Echo")]
    public class Shoot : EchoAction
    {
        public override TaskStatus OnUpdate()
        {
            if (_echoController == null)
            {
                UnityEngine.Debug.LogError("Couldn't find echo controller in shoot.");
                return TaskStatus.Failure;
            }

            _echoController.GetInputDataByRef().shoot = true;

            return TaskStatus.Success;
        }
    }
}
