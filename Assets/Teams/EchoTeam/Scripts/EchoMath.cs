using UnityEngine;

namespace Echo
{
    public static class EchoMath
    {
        /// <summary>
        /// Returns the angle form by the vector going from origin to target 
        /// </summary>
        public static float OriginToTargetVectorAngle(Vector2 origin,Vector2 target)
        {
            // Get angle formed by vector for our ship to mine
            Vector2 originToTarget = target - origin;
            float targetOrientation = Mathf.Atan2(originToTarget.y, originToTarget.x) * Mathf.Rad2Deg;
            
            // Convert angle from range -180/180 to 0/360
            return (targetOrientation + 360) % 360;;
        }
    }
}

