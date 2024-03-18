using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    public enum ControlMode { Joystick, Gyroscope };
    public ControlMode controlMode; // Este será el modo de control asignado desde el editor de Unity

    void Start()
    {
        Button playButton = GetComponent<Button>();

        switch (controlMode)
        {
            case ControlMode.Joystick:
                playButton.onClick.AddListener(GameManager.Instance.LoadGameWithJoystick);
                break;
            case ControlMode.Gyroscope:
                playButton.onClick.AddListener(GameManager.Instance.LoadGameWithGyroscope);
                break;
            default:
                Debug.LogError("Modo de control no válido");
                break;
        }
    }
}