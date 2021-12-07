using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
public class ControllerSwitch : MonoBehaviour
{
    [SerializeField]
    private GameObject Controller_X;
    [SerializeField]
    private GameObject Controller_PS;
    private void Start()
    {
        if (!(Gamepad.current is DualShockGamepad))
        {
            Controller_X.SetActive(true);
            Controller_PS.SetActive(false);
        }
        else
        {
            Controller_X.SetActive(false);
            Controller_PS.SetActive(true);
        }
    }
}