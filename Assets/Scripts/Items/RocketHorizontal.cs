using System.Collections;

namespace Items
{
    public class RocketHorizontal : ClickableItem
    {
        public override void ClickAction()
        {
            GameManager.Instance.OnHorizontalRocketClicked(x, y);
        }

        public override void OnExplode()
        {
            GameManager.Instance.OnRocketHorizontalExplode(x, y);
        }

        public override void DestroyActions()
        {
            
        }
    }
}