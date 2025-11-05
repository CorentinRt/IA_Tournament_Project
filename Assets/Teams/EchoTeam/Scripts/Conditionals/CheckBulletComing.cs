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
        public SharedFloat timingTolerance;

        public override void OnAwake()
        {
            base.OnAwake();
            
            _echoDebug.AddCircle(
                "BulletComingCheckRadius", 
                _echoData.GetOurSpaceship().Position,
                checkRadius.Value,
                Color.yellow);
            _echoDebug.AddCircle("BulletShipIntersection",
                _echoData.GetOurSpaceship().Position,
                0.5f,
                Color.green);
        }

        public override TaskStatus OnUpdate()
        {
            List<BulletView> bullets = _echoData.GetBullets();
            SpaceShipView ourSpaceship = _echoData.GetOurSpaceship();

            _echoDebug.UpdateDebugCirclePosition("BulletComingCheckRadius", ourSpaceship.Position);
            _echoDebug.UpdateDebugCirclePosition("BulletShipIntersection", ourSpaceship.Position);
            
            foreach (BulletView bullet in bullets)
            {
                if(Vector2.Distance(bullet.Position, ourSpaceship.Position) > checkRadius.Value) continue;

                //Compute intersection between bullet line and ship line, if no intersection we continue to next bullet
                if(!ComputeIntersection(ourSpaceship, bullet, out Vector2 intersection)) continue;

                _echoDebug.UpdateDebugCirclePosition("BulletShipIntersection", intersection);
                
                //Compute Time to intersection point for ship and bullet
                if(ComputeCollision(intersection, ourSpaceship, bullet)) return TaskStatus.Success;
            }
            
            return TaskStatus.Failure;
        }

        private bool ComputeIntersection(SpaceShipView ourSpaceship, BulletView bullet, out Vector2 intersection)
        {
            intersection = Vector2.zero;
            Vector2 shipDirection = ourSpaceship.Velocity.normalized;
            Vector2 bulletDirection = bullet.Velocity.normalized;

            // If ship is static, computation is different
            if (shipDirection.magnitude < Mathf.Epsilon)
            {
                    // We get vector from bullet to ship
                    Vector2 bulletToShip =  ourSpaceship.Position - bullet.Position;
                    
                    // We compute the cross product to know if  our spaceShip is on the line of the bullet.
                    // Because the ship is static and is wider than a simple point,
                    // we use a a distance to line tolerance so that if the bullet is gonna hit the ship,
                    // we can detect it. Right now the tolerance is hard coded at 0.5f, it should be radius of collider of spaceShip
                    float cross = bulletToShip.x * bulletDirection.y - bulletToShip.y * bulletDirection.x;
                    float distToLine = Mathf.Abs(cross) / bulletDirection.magnitude;
                    if (distToLine > 0.5f) return false; // Ship not on bullet line
                    
                    // Then we do dot product to check if ship is in front of bullet
                    // and not behind
                    float dot = Vector2.Dot(bulletToShip, bulletDirection);
                    
                    if(dot < 0) return false;
                    
                    intersection = ourSpaceship.Position;
                    return true;
            }
            
            // Cross product of ship direction and bullet direction to see if their line intersect
            float det = shipDirection.x * bulletDirection.y - shipDirection.y * bulletDirection.x;
            if(Mathf.Abs(det) < Mathf.Epsilon) return false; // Parallel Lines 
            
            // This part of the code it to verify is the intersection is in front of both the spaceShip and bullet,
            // and not behind them
            Vector2 diff = bullet.Position - ourSpaceship.Position;

            float t = (diff.x * bulletDirection.y - diff.y * bulletDirection.x) / det;
            float s = (diff.x * shipDirection.y - diff.y * shipDirection.x) / det;
            
            if(t < 0 || s < 0) return false; // This means intersection if behind either ship or bullet

            intersection = ourSpaceship.Position + t * shipDirection;
            return true;
        }

        private bool ComputeCollision(Vector2 intersection, SpaceShipView ourSpaceship, BulletView bullet)
        {
            if (ourSpaceship.Velocity.normalized.magnitude < Mathf.Epsilon)
                return true;
            
            float shipDistanceToIntersection = Vector2.Distance(intersection, ourSpaceship.Position);
            float shipTimeToIntersection = ourSpaceship.Velocity.magnitude / shipDistanceToIntersection;
            
            float bulletDistanceToIntersection = Vector2.Distance(intersection, bullet.Position);
            float bulletTimeToIntersection = bullet.Velocity.magnitude / bulletDistanceToIntersection;

            //Check if time for bullet to arrive to point = time for ship to arrive at same point (with tolerance)
            if (Mathf.Abs(shipTimeToIntersection - bulletTimeToIntersection) <= timingTolerance.Value)
            {
                return true;
            }

            return false;
        }
    }
}

