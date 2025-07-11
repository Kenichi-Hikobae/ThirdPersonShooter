using UnityEngine;

public enum WeaponSlot      //  Identify every weapon
{
    AK = 0,
    Pistol = 1,
    Shotgun = 2
}

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField]
    private FiringWeapon[] weapons;      //  Weapons
    [SerializeField]
    private AmmoInfo ammoUIInfo;         //  UI Ammo information

    //  State variables
    public bool isShooting{ get; set; }
    public bool isReloading{ get; set; }
    public bool isChanging{ get; set; }
    public bool canShoot { get; set; }

    //  Variables
    private PlayerAiming playerAiming;
    private Animator m_Animator;
    private bool m_IsAK;
    private bool m_IsPistol;
    private bool m_IsShotgun;
    private bool m_ActiveWeapon;

    private int indexWeapon = 0;            //  Get the active weapon to use the different methods of the active weapon

    private void Start()
    {
        //  Get components
        m_Animator = GetComponent<Animator>();
        playerAiming = GetComponent<PlayerAiming>();
        //  The player starts without any weapon active, the IU info only appears when the player has a weapon
        ammoUIInfo.gameObject.SetActive(false);
    }

    private void Update()
    {
        ActiveWeapon();
        Shooting();
        ReloadWeapon();
    }

    private void Shooting()
    {
        if (Input.GetButton("Fire"))
        {
            if (m_ActiveWeapon && !isChanging && !isReloading)
            {
                //  If the player shoots and he is not doing another action, the make the aim animations
                isShooting = true;
                playerAiming.isAiming = true;
                m_Animator.SetBool("IsAiming", true);
                AmmoInfo();
                if (canShoot)
                {
                    //  The player can shoot when the aim animation has finished
                    weapons[indexWeapon].StartFiring();     //  Firing method of the active weapon
                }
            }
        }

        //  Update the bullets simulations
        weapons[indexWeapon].BulletUpdate();

        if (Input.GetButtonUp("Fire"))
        {
            //  Stop firing
            isShooting = false;
            canShoot = false;
        }
    }

    public void AmmoInfo()
    {
        //  Update the ammo information
        ammoUIInfo.AmmoUIUpdate(weapons[indexWeapon].WeaponSlot.ToString(), weapons[indexWeapon].GetCurrentClip(), weapons[indexWeapon].GetCurrentAmmo());
    }

    private void ReloadWeapon()
    {
        if (Input.GetButtonDown("Reload"))
        {
            if (!isReloading && m_ActiveWeapon)
            {
                //  If the player reload, then reaload the active weapon
                weapons[indexWeapon].ReloadWeapon();
            }
        }
    }

    public void Reloading()
    {
        //  State and animation of reloading
        isReloading = true;
        m_Animator.SetBool("IsAiming", false);
        m_Animator.SetTrigger("IsReload");
    }

    private void ActiveWeapon()
    {
        //  Change the weapon and get the active (AK, pistol or shotgun in this case)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            indexWeapon = 0;
            WeaponEquipped(weapons[indexWeapon]);
            AmmoInfo();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            indexWeapon = 1;
            WeaponEquipped(weapons[indexWeapon]);
            AmmoInfo();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            indexWeapon = 2;
            WeaponEquipped(weapons[indexWeapon]);
            AmmoInfo();
        }
        //  Set the different animation in the animator to what the weapon is active
        m_Animator.SetBool("AK", m_IsAK);
        m_Animator.SetBool("Pistol", m_IsPistol);
        m_Animator.SetBool("Shotgun", m_IsShotgun);
    }

    private void ChangingWeapon(bool weaponA, bool weaponB)
    {
        //  Control when the player is changing a weapon to another
        if (weaponA || weaponB)
        {
            isChanging = true;
        }
    }

    private void WeaponEquipped(FiringWeapon weapon)
    {
        //  For each weapon compare the weaponslot and identify which weapon is active
        if (weapon.WeaponSlot.Equals(WeaponSlot.AK))
        {
            ChangingWeapon(m_IsPistol, m_IsShotgun);
            m_IsPistol = false;
            m_IsShotgun = false;

            if (!m_IsAK)
            {
                //  Equip the weapon
                m_IsAK = true;
                m_ActiveWeapon = true;
                ammoUIInfo.gameObject.SetActive(true);
            }
            else
            {
                //  Unequip the weapon
                m_IsAK = false;
                m_ActiveWeapon = false;
                ammoUIInfo.gameObject.SetActive(false);
            }
        }
        else if (weapon.WeaponSlot.Equals(WeaponSlot.Pistol))
        {
            ChangingWeapon(m_IsAK, m_IsShotgun);
            m_IsAK = false;
            m_IsShotgun = false;

            if (!m_IsPistol)
            {
                //  Equip the weapon
                m_IsPistol = true;
                m_ActiveWeapon = true;
                ammoUIInfo.gameObject.SetActive(true);
            }
            else
            {
                //  Unequip the weapon
                m_IsPistol = false;
                m_ActiveWeapon = false;
                ammoUIInfo.gameObject.SetActive(false);
            }
        }
        else if (weapon.WeaponSlot.Equals(WeaponSlot.Shotgun))
        {
            ChangingWeapon(m_IsAK, m_IsPistol);
            m_IsAK = false;
            m_IsPistol = false;

            if (!m_IsShotgun)
            {
                //  Equip the weapon
                m_IsShotgun = true;
                m_ActiveWeapon = true;
                ammoUIInfo.gameObject.SetActive(true);
            }
            else
            {
                //  Unequip the weapon
                m_IsShotgun = false;
                m_ActiveWeapon = false;
                ammoUIInfo.gameObject.SetActive(false);
            }
        }
    }

    public void ShootEvent()
    {
        canShoot = true;
    }
}
