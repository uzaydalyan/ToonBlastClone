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
            ParticleCreator.Instance.CubeDestroyAction(transform.position, type);
        }
    }
}