using BehaviorDesigner.Runtime.Tasks;
using Echo;
using UnityEngine;

[TaskCategory("Echo")]
public class LookAt : Action
{
    [SerializeField] private float _targetOrientation;

    public override void OnStart()
    {
        EchoController controller = GetComponent<EchoController>();
    }
}
