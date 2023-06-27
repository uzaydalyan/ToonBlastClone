using System;

namespace DefaultNamespace
{
    public struct ElementLocation : IComparable
    {
        public int x;
        public int y;

        public ElementLocation(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int CompareTo(Object obj)
        {
            ElementLocation other = (ElementLocation) obj;

            if (this.x > other.x)
            {
                return 1;
            }
            else if (this.x < other.x)
            {
                return -1;
            }
            else
            {
                if (this.y > other.y)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        public void SetX(int x)
        {
            this.x = x;
        }
    }
}