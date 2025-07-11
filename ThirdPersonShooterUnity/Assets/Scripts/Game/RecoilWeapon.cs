using UnityEngine;
using Unity.Cinemachine;

public class RecoilWeapon : MonoBehaviour
{
    [SerializeField]
    private CinemachineImpulseSource cameraShake;       //  Camera shake effect

    [SerializeField] 
    private PlayerAiming playerAimingCamera;    //  Player aim component  
    [SerializeField] 
    private float verticalRecoil;   //  How strong is the vertical
    [SerializeField] 
    private float horizontalRecoil;     //  How strong is the horizontal
    [SerializeField] 
    private float recoilDuration;   //  How fast the camera will move following the recoil

    //  Variables
    private float time;
    private float horizontalRandom;

    private void Awake()
    {
        //  Get components
        if(cameraShake == null)
            cameraShake = GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {
        if (time > 0)
        {
            //  Add the values to the input axis camera
            playerAimingCamera.yAxis -= ((verticalRecoil / 10) * Time.deltaTime) / recoilDuration;
            playerAimingCamera.xAxis -= ((horizontalRandom / 10) * Time.deltaTime) / recoilDuration;
            time -= Time.deltaTime;
        }
    }

    public void GenerateRecoil()
    {
        time = recoilDuration;
        //  Make a impulse effect to the main camera when the playerr is shooting
        cameraShake.GenerateImpulse(Camera.main.transform.forward);

        //  Generate a ramdon number with the horizontal recoil
        horizontalRandom = Random.Range(-horizontalRecoil, horizontalRecoil);
    }
}
