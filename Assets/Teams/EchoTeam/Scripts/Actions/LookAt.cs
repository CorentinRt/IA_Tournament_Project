using BehaviorDesigner.Runtime.Tasks;
using DoNotModify;
using Echo;
using UnityEngine;

[TaskCategory("Echo")]
public class LookAt : Action
{
    [SerializeField, Range(0.0f, 360.0f)] private float _targetOrientation;
    private EchoController _echoController;

    public override void OnAwake()
    {
        base.OnAwake();
        _echoController = GetComponent<EchoController>();
        
    }

    public override void OnStart()
    {
        _echoController.GetInputDataByRef().targetOrientation  = _targetOrientation;
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}
