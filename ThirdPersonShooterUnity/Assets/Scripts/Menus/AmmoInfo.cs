using UnityEngine;
using UnityEngine.UI;

public class AmmoInfo : MonoBehaviour
{
    [SerializeField]
    private Text weaponText;             //  Active weapon
    [SerializeField] 
    private Text ammoText;               //  Ammo quantity of thw active weapon

    public void AmmoUIUpdate(string weapon, int ammo, int currentAmmo)
    {
        //  Update the ammo quatity of the weapon and shows how many bullets in total the player has
        weaponText.text = weapon;
        ammoText.text = ammo.ToString() + " / " + currentAmmo.ToString();
    }
}
