using UnityEngine;

public class HitDamage : MonoBehaviour
{
    [SerializeField]
    public EnemyHealth health; //  Enemy health class component

    public void HitRaycast(FiringWeapon weapon, Vector3 direction)
    {
        // Damage the player with the weapon damage and give the direction of the force
        health.TakeDamage(weapon.WeaponDamage, direction);
    }
}
