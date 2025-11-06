using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Echo
{
    [TaskCategory("Echo")]
    public class CheckTimeElapsedSinceLastSuccess : EchoConditional
    {
        // ----- FIELDS ----- //
        [Tooltip("Time minimum to wait since last success")]
        public SharedFloat timeToWait = 10f;

        private float _lastSuccessTime = 0f;
        // ----- FIELDS ----- //

        public override void OnAwake()
        {
            base.OnAwake();

            _lastSuccessTime = UnityEngine.Time.time;
        }

        public override TaskStatus OnUpdate()
        {
            float currentTime = UnityEngine.Time.time;
            bool canDoAction = false;

            if (currentTime - _lastSuccessTime > timeToWait.Value)
            {
                canDoAction = true;
                _lastSuccessTime = currentTime;
            }

            return canDoAction ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
