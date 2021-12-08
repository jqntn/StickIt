using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
public class TextColorOnSelect : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TextMeshProUGUI toChange;
    [SerializeField] private Color newColor;
    private Color baseColor;
    private void Awake() => baseColor = toChange.color;
    public void OnDeselect(BaseEventData eventData) => toChange.color = baseColor;
    public void OnSelect(BaseEventData eventData) => toChange.color = newColor;
}