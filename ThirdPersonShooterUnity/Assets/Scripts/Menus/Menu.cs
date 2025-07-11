using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject menuUI;           //  Menu game object to show the menu in screen
    public Text enemiesQuantity;        //  How many enemies you want
    public Slider quantity;             //  1 - 50 - Control how many enemies the player wants
    public PlayerHealth player;         //  Health of the player

    bool isPause;                       //  Control if the player has paused the game
    GameSettings gameSettings;

    private void Awake()
    {
        //  Get the game setting to apply them at the begining of the game
        gameSettings = GameObject.FindObjectOfType<GameSettings>();
        quantity.value = gameSettings.enemies;
        gameSettings.ApplySettings();
        //  Setup the begining of the scene
        Setup();
        //  Get the player health
        player.GetComponent<PlayerHealth>();
    }

    public void Update()
    {
        enemiesQuantity.text = quantity.value.ToString();
        if (!player.PlayerDead)
        {
            //  Pause and unpause the game
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!isPause)
                {
                    Pause();
                }
                else
                {
                    Continue();
                }
            }
        }
        else
        {
            //  If the player is dead then pause the game
            Pause();
        }
    }

    private void Setup()
    {
        //  If enemies is zero then stop the game at the begining
        if (gameSettings.enemies <= 0)
        {
            Pause();
        }
        else
        {
            Continue();
        }
    }

    private void Continue()
    {
        //  Unpause the game and hide the menu
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        menuUI.SetActive(false);
        isPause = false;
    }

    private void Pause()
    {
        //  Pause the game and show the menu
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        menuUI.SetActive(true);
        isPause = true;
    }

    public void Play()
    {
        // Playe the game, reset the same scene
        int number = (int)quantity.value;
        gameSettings.Settings(number);
        ResetGame();
    }

    public void ResetGame()
    {
        //  Get the active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        //  Quit the aplicacion
        Application.Quit();
    }
}
