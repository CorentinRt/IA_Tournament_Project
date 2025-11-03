using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace IIM
{
	[TaskCategory("IIM")]
	public class CompareGameObject : AConditionalCompare
	{
		[Tooltip("The first variable to compare")]
		public SharedInt variable;
		[Tooltip("The variable to compare to")]
		public SharedInt compareTo;

		public override TaskStatus OnUpdate()
		{
			return Compare(variable.Value, compareTo.Value, OPERATOR.EQUAL);
		}
	}
}
