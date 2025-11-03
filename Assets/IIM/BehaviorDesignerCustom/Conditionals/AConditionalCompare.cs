using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System;

namespace IIM
{
	[TaskDescription("Returns success if the comparison is true.")]
	public abstract class AConditionalCompare : Conditional
	{
		public enum OPERATOR
		{
			EQUAL = 0,
			LOWER = 1,
			GREATER = 2,
			LOWER_OR_EQUAL = 3,
			GREATER_OR_EQUAL = 4,
		}

		public TaskStatus Compare(IComparable a, IComparable b, OPERATOR op)
		{
			if (a == null || b == null)
				return a == b ? TaskStatus.Success : TaskStatus.Failure;

			int result = a.CompareTo(b);
			bool isTrue = false;
			switch (op)
			{
				case OPERATOR.EQUAL: isTrue = result == 0; break;
				case OPERATOR.LOWER: isTrue = result < 0; break;
				case OPERATOR.GREATER: isTrue = result > 0; break;
				case OPERATOR.LOWER_OR_EQUAL: isTrue = result <= 0; break;
				case OPERATOR.GREATER_OR_EQUAL: isTrue = result >= 0; break;
			}
			return isTrue ? TaskStatus.Success : TaskStatus.Failure;
		}
	}
}