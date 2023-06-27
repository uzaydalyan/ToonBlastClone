using DefaultNamespace;

namespace Items
{
    public class Duck : Obstacle
    {
        public override bool SideExplosionControl()
        {
            return false;
        }

        public override bool PowerEffectControl()
        {
            return false;
        }

        public override void PowerEffectAction()
        {

        }

        public override void SideExplosionAction()
        {

        }

        public override bool EndOfMoveControl()
        {
            if (x == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void EndOfMoveAction()
        {
            GameManager.Instance.DestroyObstacle(x, y);
            DestroyActions();
        }

        public override void DestroyActions()
        {
            AudioManager.Instance.PlayFx("duck");
            DecreaseGoal();
        }
    }
}