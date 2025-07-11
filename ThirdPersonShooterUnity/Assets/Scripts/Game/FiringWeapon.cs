using System.Collections.Generic;
using UnityEngine;

public class FiringWeapon : MonoBehaviour
{
    private class Bullet
    {
        //  Class to store and get the bullet information
        public float time;
        public Vector3 inicialPosition;
        public Vector3 inicialVelocity;
        public TrailRenderer traicerBullet;
    }

    [SerializeField]
    private WeaponSlot weaponSlot;  //  Slot weapon

    [SerializeField]
    private float fireRate = 10.0f;              //  How fast the wepon can fire
    [SerializeField]
    private float bulletSpeed = 1000.0f;         //  Bullet velocity
    [SerializeField]
    private float bulletDrop = 0.0f;             //  Fall force of the bullet
    [SerializeField]
    private float weaponDamage = 10.0f;          //  Damage of the wepon
    [SerializeField]
    private int ammo = 10;                       //  How many bullet in the magazine
    [SerializeField]
    private int maximumAmmo = 100;               //  Maximun ammo that the player has
    [SerializeField]
    private float bulletForce = 5.0f;            //  Force of the bullet hit
    [SerializeField]
    private Transform raycastOrigin;             //  Origin of the raycast -> the end of the canon of the weapon
    [SerializeField]
    private Transform raycastDestination;        //  Destination of the raycas -> the crosshair
    [SerializeField]
    private TrailRenderer bulletEffect;          //  Effect of the path of the bullet
    [SerializeField]
    private ParticleSystem muzzleEffect;         //  Shoot effect
    [SerializeField]
    private ParticleSystem bulletHitEffect;      //  Bullets hit effect

    public bool isFiring = false;
    public float WeaponDamage => weaponDamage;
    public WeaponSlot WeaponSlot => weaponSlot;

    private Ray m_Ray;                                  //  Raycast
    private RaycastHit m_HitInfo;                       //  Raycasts hit information
    private float m_AccumulatedTime = 1.0f;             //  Time to shoot again
    private List<Bullet> bullets = new List<Bullet>();  //  Bullets
    private float m_MaxBulletLifeTime = 3.0f;           //  Life of the bullet

    //  Variables
    private RecoilWeapon recoil;
    private PlayerWeapon playerWeapon;
    private int m_ClipAmmo;
    private int m_CurrentAmmo;

    private Bullet CreateBullet(Vector3 position, Vector3 velocity)
    {
        //  Get a new bullet
        Bullet bullet = new Bullet();
        bullet.inicialPosition = position;
        bullet.inicialVelocity = velocity;
        bullet.time = 0.0f;
        bullet.traicerBullet = Instantiate(bulletEffect, position, Quaternion.identity);
        bullet.traicerBullet.AddPosition(position);
        return bullet;
    }

    private void Awake()
    {
        //  Get components and initialize variables
        recoil = GetComponent<RecoilWeapon>();
        playerWeapon = GetComponentInParent<PlayerWeapon>();
        m_ClipAmmo = ammo;
        m_CurrentAmmo = maximumAmmo;
    }

    private void Update()
    {
        if (m_AccumulatedTime <= 1)
        {
            //  Wait time to fire again
            m_AccumulatedTime += Time.deltaTime;
        }
    }

    public void StartFiring()
    {
        FiringUpdate();
    }

    public void FiringUpdate()
    {
        if (m_AccumulatedTime >= 1 / fireRate)
        {
            //  If the gun can fire
            WeaponFiring();
            //  Resert the time to wait to fire again
            m_AccumulatedTime = 0;
        }
    }

    private void WeaponFiring()
    {
        if (m_ClipAmmo <= 0)
        {
            //  If the weapon does not have more ammo, then reload
            ReloadWeapon();
        }
        else
        {
            //  When the wepon is firing do the firing effects and the recoil
            muzzleEffect.Emit(1);
            recoil.GenerateRecoil();
            //  Get the velocity of the bullet and create a new one
            Vector3 fireVelocity = (raycastDestination.position - raycastOrigin.position).normalized * bulletSpeed;
            var bullet = CreateBullet(raycastOrigin.position, fireVelocity);
            bullets.Add(bullet);
            //  Decrease the ammo
            m_ClipAmmo--;
        }
    }

    public void BulletUpdate()
    {
        BulletSimulation();
        DestroyBullet();
    }

    private void BulletSimulation()
    {
        bullets.ForEach(bullet =>
        {
            Vector3 p0 = GetPositionBullet(bullet);
            bullet.time += Time.deltaTime;
            Vector3 p1 = GetPositionBullet(bullet);
            RaycastBulletSegment(p0, p1, bullet);
        });
    }

    private void DestroyBullet()
    {
        bullets.RemoveAll(bullet => bullet.time >= m_MaxBulletLifeTime);
    }

    private void RaycastBulletSegment(Vector3 origin, Vector3 end, Bullet bullet)
    {
        m_Ray.origin = origin;
        m_Ray.direction = end - origin;
        float distance = Vector3.Distance(end, origin);

        if (Physics.Raycast(m_Ray, out m_HitInfo, distance))
        {
            bool staticObject = true;
            //  Get the rigibody of the hit object
            Rigidbody body = m_HitInfo.collider.GetComponent<Rigidbody>();
            if (body)
            {
                //  Add a new force to the rigibody
                body.AddForceAtPosition(m_Ray.direction * bulletForce, m_HitInfo.point, ForceMode.Impulse);
                staticObject = false;
            }

            //  Get the hit damage of the enemy
            var enemyBody = m_HitInfo.collider.GetComponent<HitDamage>();
            if (enemyBody)
            {
                //  Do damage to the enemy
                enemyBody.HitRaycast(this, m_Ray.direction);
                staticObject = false;
            }

            //  If the hit object is a static object, then make the hit effect
            if (staticObject)
            {
                bulletHitEffect.transform.position = m_HitInfo.point;
                bulletHitEffect.transform.forward = m_HitInfo.normal;
                bulletHitEffect.Emit(1);
            }

            //  Make the bullet path effect, following the bullet position
            bullet.traicerBullet.transform.position = m_HitInfo.point;
            bullet.time = m_MaxBulletLifeTime;
        }
        else
        {
            bullet.traicerBullet.transform.position = end;
        }
    }

    private Vector3 GetPositionBullet(Bullet bullet)
    {
        //  Get the position of the bullet with the following math function
        //  p * v * t + (g*t^2)/2
        Vector3 gravity = Vector3.down * bulletDrop;
        return (bullet.inicialPosition) + (bullet.inicialVelocity * bullet.time) + (0.5f * gravity * bullet.time * bullet.time);
    }

    public void ReloadWeapon()
    {
        //  Reload the active weapon
        if (m_ClipAmmo != ammo && m_CurrentAmmo > 0)
        {
            int bullets = m_ClipAmmo;
            playerWeapon.Reloading();
            if (m_CurrentAmmo >= ammo)
            {
                //  If can reload a full magazine
                m_ClipAmmo = ammo;
                m_CurrentAmmo = m_CurrentAmmo - (ammo - bullets);
            }
            else
            {
                //  If the remaining bullet are less than a full magazine
                m_ClipAmmo = m_CurrentAmmo;
                m_CurrentAmmo = 0;
            }
        }
    }

    public int GetCurrentClip()
    {
        return m_ClipAmmo;
    }

    public int GetCurrentAmmo()
    {
        return m_CurrentAmmo;
    }
}
