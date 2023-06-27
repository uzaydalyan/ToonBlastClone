using DefaultNamespace;
using UnityEngine;

namespace Items
{
    public abstract class Item : MonoBehaviour
    {
        public int x;
        public int y;
        public ItemType type;
        public Rigidbody2D rigidBody;

        public void InitItem(int x, int y)
        {
            this.x = x;
            this.y = y;
            GameManager.Instance.Grid[x, y] = gameObject;
            rigidBody = GetComponent<Rigidbody2D>();
        }

        public void DecreaseGoal()
        {
            ProgressManager.Instance.DecreaseGoal(this);
        }

        public abstract void DestroyActions();

    }
}