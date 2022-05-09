using UnityEngine;

namespace Hun.Entity
{
    public interface IDamageable
    {
        void ApplyDamage(DamageMessage dmgMsg);
    }
}