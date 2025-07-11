using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [SerializeField]
    private Transform hipsBone;          //  Get the Hips bone to be able to apply physics when the enemy is dead
    [SerializeField]
    private SkinnedMeshRenderer mannequin;

    private Rigidbody[] m_Rigidbodies;          //  Get the all rigibodies of the bone hierachy (Ragdoll)
    private Animator m_Animator;                //  Animator component

    private void Start()
    {
        m_Rigidbodies = GetComponentsInChildren<Rigidbody>();
        m_Animator = GetComponent<Animator>();
        //  At the beginning desactivate the ragdoll to avoid conflict with the animator and physics of the gameobject
        DeactivateRagdoll();
    }

    public void ActivateRagdoll()
    {
        /*
         *  For each object in the bone scheme, the kinematic property is disabled 
         *  and the animator component is disabled to be able to get the physics properties of the ragdoll.
         * */
        foreach (var rigibody in m_Rigidbodies)
        {
            rigibody.isKinematic = false;
        }
        m_Animator.enabled = false;
        //  Activate that the mesh also renders when it is off screen
        mannequin.updateWhenOffscreen = true;
    }

    public void DeactivateRagdoll()
    {
        /*
         *  For each object in the bone scheme, the kinematic property is enabled 
         *  and the animator component is enabled.
         * */
        foreach (var rigibody in m_Rigidbodies)
        {
            rigibody.isKinematic = true;
        }
        m_Animator.enabled = true;
    }

    public void ApplyForce(Vector3 force)
    {
        //  Apply force to the ragdoll when the enemy is dead
        var rigibody = hipsBone.GetComponent<Rigidbody>();
        rigibody.AddForce(force, ForceMode.VelocityChange);
    }
}
