using System.Collections;

namespace Items
{
    public abstract class ClickableItem : Item
    {
        public abstract void ClickAction();

        private void OnMouseDown()
        {
            ClickAction();
        }

        public abstract void OnExplode();
    }
}