using UnityEngine;
public class SwitchOnSelected : MonoBehaviour
{
    [SerializeField]
    private GameObject toSwitch;
    private Animator ar;
    private void Start() => ar = GetComponent<Animator>();
    private void Update()
    {
        toSwitch.SetActive(
            ar.GetCurrentAnimatorStateInfo(0).IsName("Highlighted") ||
            ar.GetCurrentAnimatorStateInfo(0).IsName("Pressed") ||
            ar.GetCurrentAnimatorStateInfo(0).IsName("Selected")
            ? true : false);
    }
}