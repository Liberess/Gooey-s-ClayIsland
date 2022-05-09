using UnityEngine;

namespace Hun.Entity
{
    public struct DamageMessage
    {
        public GameObject damager;
        public int dmgAmount;

        public Vector3 hitPoint;
        public Vector3 hitNormal;
    }
}