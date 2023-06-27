using System.Collections;

namespace Items
{
    public class RocketVertical : ClickableItem
    {
        public override void DestroyActions()
        {
        }

        public override void ClickAction()
        {
            GameManager.Instance.OnVerticalRocketClicked(x, y);
        }

        public override void OnExplode()
        {
            GameManager.Instance.OnRocketVerticalExplode(x, y);
        }
    }
}