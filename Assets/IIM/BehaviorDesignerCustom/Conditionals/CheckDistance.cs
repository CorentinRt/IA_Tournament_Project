using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


namespace IIM
{
	[TaskCategory("IIM")]
	public class CheckDistance : Conditional
	{
		public enum OPERATOR
		{			
			LOWER = 0,
			GREATER = 1,
			LOWER_OR_EQUAL = 2,
			GREATER_OR_EQUAL = 3,
		}

		[Tooltip("The first point")]
		public SharedVector2 A;
		[Tooltip("The second point")]
		public SharedVector2 B;
		[Tooltip("Comparison operator")]
		public OPERATOR op = OPERATOR.LOWER;
		[Tooltip("Distance value")]
		public SharedFloat distance;

		public override TaskStatus OnUpdate()
		{
			float magnitude = (B.Value - A.Value).magnitude;
			bool isTrue = false;
			switch (op)
			{
				case OPERATOR.LOWER: isTrue = magnitude < distance.Value; break;
				case OPERATOR.GREATER: isTrue = magnitude > distance.Value; break;
				case OPERATOR.LOWER_OR_EQUAL: isTrue = magnitude <= distance.Value; break;
				case OPERATOR.GREATER_OR_EQUAL: isTrue = magnitude >= distance.Value; break;
			}
			return isTrue ? TaskStatus.Success : TaskStatus.Failure;
		}
	}
}