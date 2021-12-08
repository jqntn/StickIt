using UnityEngine;
using UnityEngine.EventSystems;
public class EnableOnSelect : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private GameObject toEnable;
    private void Start() => toEnable.SetActive(false);
    public void OnDeselect(BaseEventData eventData) => toEnable.SetActive(false);
    public void OnSelect(BaseEventData eventData) => toEnable.SetActive(true);
}