using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public int enemies { get; set; }    //  How many enemies will be in the scene

    [SerializeField]
    private GameObject enemy;   //  Prefab of the enemy

    public static GameSettings instance = null; //  Instance of the object

    private void Awake()
    {
        //  Check if this object already exists in the scene
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        //  Do not destroy the object the a new scene loaded
        DontDestroyOnLoad(gameObject);
    }

    public void ApplySettings()
    {
        //  Instantiate the number of selected enemies
        for (int i = 0; i <= enemies; i++)
        {
            Instantiate<GameObject>(enemy, Vector3.zero, Quaternion.identity);
        }
    }

    public void Settings(int enemiesQuantity)
    {
        //  Changes the number of enemies
        enemies = enemiesQuantity;
    }
}
