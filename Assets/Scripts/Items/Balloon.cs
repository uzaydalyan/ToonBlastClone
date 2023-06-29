using System;
using DefaultNamespace;
using UnityEngine;

namespace Items
{
    public class Balloon : Obstacle
    {
        public override bool SideExplosionControl()
        {
            return true;
        }

        public override Boolean PowerEffectControl()
        {
            return true;
        }

        public override void PowerEffectAction() 
        {
            GameManager.Instance.DestroyObstacle(x, y);
            AudioManager.Instance.PlayFx("balloon");
            DecreaseGoal();
        }

        public override void SideExplosionAction()
        {
            GameManager.Instance.AddObstacleToMatchedList(x, y);
        }

        public override bool EndOfMoveControl()
        {
            return false;
        }

        public override void EndOfMoveAction()
        {
            
        }

        public override void DestroyActions()
        {
            AudioManager.Instance.PlayFx("balloon");
            DecreaseGoal();
        }
        
    }
}