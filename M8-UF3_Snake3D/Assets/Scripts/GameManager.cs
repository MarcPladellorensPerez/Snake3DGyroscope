using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject gameManagerObject = new GameObject("GameManager");
                    instance = gameManagerObject.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void LoadSnake3DScene()
    {
        SceneManager.LoadScene("Snake3D");
    }
    
    public void LoadGameWithJoystick()
    {
        SceneManager.LoadScene("Snake3D");
        SnakeController.UseGyroscope = false; // Usar el joystick para el control
    }

    public void LoadGameWithGyroscope()
    {
        SceneManager.LoadScene("Snake3D");
        SnakeController.UseGyroscope = true; // Usar el giroscopio para el control
    }

}
