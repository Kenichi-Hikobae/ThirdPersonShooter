using UnityEngine;

public class CrossHairPosition : MonoBehaviour
{
    private Camera m_MainCamera;    //  Current main camera

    private Ray m_Ray;
    private RaycastHit m_HitInfo;

    private void Start()
    {
        m_MainCamera = Camera.main;
    }

    private void Update()
    {
        //  Make a raycast from the camera origin and detect if there is an object
        m_Ray.origin = m_MainCamera.transform.position;
        m_Ray.direction = m_MainCamera.transform.forward;
        Physics.Raycast(m_Ray, out m_HitInfo);

        //  Changes the position of the crosshair to where the player is going to shoot
        transform.position = m_HitInfo.point;
    }
}
