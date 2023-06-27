using UnityEngine;

namespace DefaultNamespace
{
    public class Goal
    {
        public ItemType itemType;
        public int goal;

        public Goal(ItemType itemType, int count)
        {
            goal = count;
            this.itemType = itemType;
        }
    }
}