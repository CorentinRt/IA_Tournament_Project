using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace IIM
{
	[TaskCategory("IIM")]
	public class CompareBool : AConditionalCompare
	{
		[Tooltip("The first variable to compare")]
		public SharedBool variable;
		[Tooltip("The variable to compare to")]
		public SharedBool compareTo;

		public override TaskStatus OnUpdate()
		{
			return Compare(variable.Value, compareTo.Value, OPERATOR.EQUAL);
		}
	}
}
