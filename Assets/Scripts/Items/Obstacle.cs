using System;

namespace Items
{
    public abstract class Obstacle : Item
    {
        public abstract Boolean SideExplosionControl();
        public abstract void SideExplosionAction();
        public abstract Boolean PowerEffectControl();
        public abstract void PowerEffectAction();
        public abstract Boolean EndOfMoveControl();
        public abstract void EndOfMoveAction();
    }
}