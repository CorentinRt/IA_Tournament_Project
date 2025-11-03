using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace IIM
{
	[TaskCategory("IIM")]
	public class CompareInt : AConditionalCompare
	{
		[Tooltip("The first variable to compare")]
		public SharedInt variable;
		[Tooltip("The comparison operator")]
		public OPERATOR op = OPERATOR.EQUAL;
		[Tooltip("The variable to compare to")]
		public SharedInt compareTo;

		public override TaskStatus OnUpdate()
		{
			return Compare(variable.Value, compareTo.Value, op);
		}
	}
}
