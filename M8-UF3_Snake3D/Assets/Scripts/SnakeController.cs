using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SnakeController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float bodySpeed;
    [SerializeField] private float steerSpeed;
    [SerializeField] private GameObject bodyPrefab;
    [SerializeField] private GameObject applePrefab;
    [SerializeField] private StickController stickController;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;
    public static bool UseGyroscope = false;

    private List<GameObject> bodyParts = new List<GameObject>();
    private List<Vector3> positionHistory = new List<Vector3>();
    private int applesEaten = 0;

    private Vector2 moveDirection = Vector2.zero;

    private Gyroscope gyro;
    private bool gyroEnabled;

    void Start()
    {
        bodyParts.Add(gameObject);
        InvokeRepeating("UpdatePositionHistory", 0f, 0.01f);
        SpawnApple();

        stickController.StickChanged += OnStickChanged;

        gyroEnabled = EnableGyro() && UseGyroscope;

        // Desactivar el joystick si el modo de control del giroscopio estÃ¡ habilitado
        if (UseGyroscope)
        {
            if (stickController != null)
            {
                stickController.gameObject.SetActive(false);
            }
        }
    }


    void Update()
    {
        if (gyroEnabled)
        {
            float steerDirection = -gyro.rotationRateUnbiased.z; // Use z-axis rotation for steering
            transform.Rotate(Vector3.up * steerDirection * steerSpeed * Time.deltaTime);
        }
        else
        {
            // Use joystick if gyroscope is not available
            float steerDirection = moveDirection.x;
            transform.Rotate(Vector3.up * steerDirection * steerSpeed * Time.deltaTime);
        }

        MoveBody();
    }

    void OnStickChanged(object sender, StickEventArgs e)
    {
        moveDirection = e.Position;
    }

    void MoveBody()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        for (int i = 1; i < bodyParts.Count; i++)
        {
            Vector3 targetPosition = bodyParts[i - 1].transform.position;
            Vector3 moveDirection = targetPosition - bodyParts[i].transform.position;
            bodyParts[i].transform.position += moveDirection.normalized * bodySpeed * Time.deltaTime;
            bodyParts[i].transform.LookAt(targetPosition);
        }
    }

    void UpdatePositionHistory()
    {
        positionHistory.Insert(0, transform.position);

        if (positionHistory.Count > 500)
        {
            positionHistory.RemoveAt(positionHistory.Count - 1);
        }
    }

    void GrowSnake()
    {
        Vector3 newPosition = bodyParts[bodyParts.Count - 1].transform.position - transform.forward * 1.0f;
        GameObject newBodyPart = Instantiate(bodyPrefab, newPosition, Quaternion.identity);
        bodyParts.Add(newBodyPart);
    }

    void SpawnApple()
    {
        Vector3 randomPos = new Vector3(Random.Range(-10f, 10f), 1f, Random.Range(-10f, 10f));
        GameObject apple = Instantiate(applePrefab, randomPos, Quaternion.Euler(-90, 0, 0));
        apple.transform.position = new Vector3(apple.transform.position.x, 1f, apple.transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Apple"))
        {
            Destroy(other.gameObject);
            GrowSnake();
            SpawnApple();
            UpdateScore();
        }
        else if (other.CompareTag("Die"))
        {
            UpdateBestScore();
            SceneManager.LoadScene("Menu");
        }
    }

    public void UpdateScore()
    {
        applesEaten++;
        if (scoreText != null)
        {
            scoreText.text = "Score: " + applesEaten.ToString();
        }
    }

    void UpdateBestScore()
    {
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);

        if (applesEaten > bestScore)
        {
            bestScore = applesEaten;
            PlayerPrefs.SetInt("BestScore", bestScore);
            bestScoreText.text = "Best Score: " + bestScore.ToString();
        }
    }

    private bool EnableGyro()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
            return true;
        }
        return false;
    }
}
