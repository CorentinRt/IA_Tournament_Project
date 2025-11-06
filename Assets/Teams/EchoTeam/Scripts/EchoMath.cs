using DoNotModify;
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

        public static bool CanHit(float orientation, Vector2 originPosition, Vector2 targetPosition, Vector2 targetVelocity, float hitTimeTolerance)
        {
            if (hitTimeTolerance <= 0)
            {
                //Debug.LogError("Hit time tolerence must be greater than 0");
                return false;
            }

            float shootAngle = Mathf.Deg2Rad * orientation;
            Vector2 shootDirection = new Vector2(Mathf.Cos(shootAngle), Mathf.Sin(shootAngle));

            Vector2 intersection;
            bool canIntersect = AimingHelpers.ComputeIntersection(originPosition, shootDirection, targetPosition, targetVelocity, out intersection);
            if (!canIntersect)
            { // Cannot shoot if directions never cross eachother (parallel)
                //Debug.Log($"Can intersect = {canIntersect}");
                return false;
            }

            Vector2 spaceshipToIntersection = intersection - originPosition;
            if (Vector2.Dot(spaceshipToIntersection, shootDirection) <= 0) // Cannot shoot if target is behind
            {
                //Debug.Log($"First Dot return = {Vector2.Dot(spaceshipToIntersection, shootDirection)}");
                return false;
            }

            Vector2 targetToIntersection = intersection - targetPosition;
            float bulletTimeToIntersection = spaceshipToIntersection.magnitude / Bullet.Speed;
            float targetTimeToIntersection = targetToIntersection.magnitude / targetVelocity.magnitude;
            targetTimeToIntersection *= Vector2.Dot(targetToIntersection, targetVelocity) > 0 ? 1 : -1;

            float timeDiff = bulletTimeToIntersection - targetTimeToIntersection;

            //Debug.Log($"Abs return need = {Mathf.Abs(timeDiff)} < {hitTimeTolerance}");
            return Mathf.Abs(timeDiff) < hitTimeTolerance;
        }
    }
}

