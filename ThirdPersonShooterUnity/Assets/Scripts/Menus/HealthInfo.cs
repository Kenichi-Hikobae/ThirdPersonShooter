using UnityEngine;
using UnityEngine.UI;

public class HealthInfo : MonoBehaviour
{
    [SerializeField]
    private Text textHealth;     //  Updating text of the current health of the player
    [SerializeField]
    private Slider health;   //  Health of the player

    public void HealthUpdate(float healthValue)
    {
        //  Update the current health of the player
        health.value = healthValue;
        textHealth.text = health.value.ToString();
    }
}
