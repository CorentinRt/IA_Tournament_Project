using System.Collections.Generic;
using System.Net;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DoNotModify;
using UnityEngine;

namespace Echo
{
    public class CheckBulletComing : EchoConditional
    {
        public SharedFloat checkRadius;
        public SharedFloat tolerance;
        
        public override TaskStatus OnUpdate()
        {
            List<BulletView> bullets = _echoData.GetBullets();
            SpaceShipView ourSpaceship = _echoData.GetOurSpaceship();

            foreach (BulletView bullet in bullets)
            {
                if(Vector2.Distance(bullet.Position, ourSpaceship.Position) > checkRadius.Value) continue;

                //Compute intersection between bullet line and ship line, if no intersection we continue to next bullet
                if(!AimingHelpers.ComputeIntersection(bullet.Position, bullet.Velocity.normalized, ourSpaceship.Position, ourSpaceship.Velocity.normalized, out Vector2 intersection)) continue;

                //Compute Time to intersection point for ship and bullet
                float shipDistanceToIntersection = Vector2.Distance(intersection, ourSpaceship.Position);
                float shipTimeToIntersection = ourSpaceship.Velocity.magnitude / shipDistanceToIntersection;
                
                float bulletDistanceToIntersection = Vector2.Distance(intersection, bullet.Position);
                float bulletTimeToIntersection = bullet.Velocity.magnitude / bulletDistanceToIntersection;

                //Check if time for bullet to arrive to point = time for ship to arrive at same point (with tolerance)
                if (Mathf.Abs(shipTimeToIntersection - bulletTimeToIntersection) <= tolerance.Value)
                    return TaskStatus.Success;
            }
            
            return TaskStatus.Failure;
        }
    }
}

