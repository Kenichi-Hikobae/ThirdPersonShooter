using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] 
    private float health = 100.0f;      //  Maximum health of the player
    [SerializeField] 
    private float timeDamageReset = 2.0f;
    [SerializeField] 
    private float damageEffectIntesity = 5.0f;  //  Intensity of effect when the player damages the enemy
    [SerializeField] 
    private float damageEffectDuration = 0.1f;  //  Duration of the effect
    [SerializeField] 
    private HealthInfo healthInfo;  //  UI information of the current health of the player

    public bool PlayerDead { get; set; }    //  Check if the player has dead

    //  Variables
    private float m_CurrentHealth;
    private float m_AcumulateTime;
    private float m_DamageEffectTimer;
    private float intensity;
    private SkinnedMeshRenderer m_Mannequin;    //  Mesh of the object
    private Color m_Color;                      //  Default color of the object, it's required to make the damage effect

    private void Start()
    {
        //  Assign variables
        m_CurrentHealth = health;
        intensity = damageEffectIntesity;
        m_Mannequin = GetComponentInChildren<SkinnedMeshRenderer>();    //  Get the SkinnedMeshRenderer componet
        m_Color = m_Mannequin.material.color;
    }

    private void Update()
    {
        if (m_AcumulateTime > 0)
        {
            //  Controls the time that the player can receive damage again
            m_AcumulateTime -= Time.deltaTime;
        }
        //  Make the damage effect if the player can receive damage again
        DamageEffect();
    }

    public void ReceiveDamage(float enemyDamage)
    {
        if (m_AcumulateTime <= 0)
        {
            //  Apply damage to the player
            m_CurrentHealth -= enemyDamage;
            healthInfo.HealthUpdate(m_CurrentHealth);
            m_AcumulateTime = timeDamageReset;
            m_DamageEffectTimer = damageEffectDuration;
            if (m_CurrentHealth <= 0)
            {
                PlayerDies();
            }
        }
    }

    public void DamageEffect()
    {
        if (m_DamageEffectTimer > 0)
        {
            m_DamageEffectTimer -= Time.deltaTime;
            //  Change the brightness of the mesh material
            m_Mannequin.material.color *= intensity;
        }
        else
        {
            //  Restart the color of the mesh material
            m_Mannequin.material.color = m_Color;
        }
    }

    private void PlayerDies()
    {
        //  The player is dead :C
        PlayerDead = true;
    }
}
