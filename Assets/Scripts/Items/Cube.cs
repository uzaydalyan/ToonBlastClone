using DefaultNamespace;

namespace Items
{
    public class Cube : ClickableItem
    {
        public override void ClickAction()
        {
            GameManager.Instance.onCubeClicked(x, y);
        }

        public override void OnExplode()
        {
            GameManager.Instance.DestroyCube(x, y);
        }
        
        public override void DestroyActions()
        {
            DecreaseGoal();
            StartCoroutine(AnimationManager.Instance.CubeSpreadParticles(transform.position, type));
        }
        
    }
}