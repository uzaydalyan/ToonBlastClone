using UnityEngine;

namespace DefaultNamespace
{
    public static class CubeColors
    {
        public static Color GetCubeColor(ItemType item)
        {
            switch (item)
            {
                case ItemType.b:
                    return new Color(34, 113, 206);
                case ItemType.g:
                    return new Color(57, 167, 8);
                case ItemType.y:
                    return new Color(231, 196, 0);
                case ItemType.r:
                    return new Color(208, 11, 8);
                case ItemType.p:
                    return new Color(150, 13, 165);
                default:
                    return new Color(34, 113, 206);
            }
        }
    }
}