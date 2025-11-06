using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Echo
{
    public class CheckIsWinning : EchoConditional
    {
        // ----- FIELDS ----- //
        [Tooltip("Point d'écarts nécessaires pour renvoyer success")]
        public SharedInt pointDifferenceForSuccess = 2;

        [Tooltip("Success ou failed si égalité (si points d'écarts = 0)")]
        public SharedBool returnSuccessForEquality = false;
        // ----- FIELDS ----- //

        public override TaskStatus OnUpdate()
        {
            int ourSpaceshipScore = _echoData.GetOurSpaceshipScore();
            int enemySpaceshipScore = _echoData.GetEnemySpaceshipScore();

            bool isWinning = false;

            if (pointDifferenceForSuccess.Value == 0)
            {
                if (returnSuccessForEquality.Value)
                {
                    isWinning = ourSpaceshipScore >= enemySpaceshipScore;
                }
                else
                {
                    isWinning = ourSpaceshipScore > enemySpaceshipScore;
                }
            }
            else
            {
                isWinning = ourSpaceshipScore - enemySpaceshipScore >= pointDifferenceForSuccess.Value;
            }

            return isWinning ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}