using TMPro;
using UnityEngine;
public class ChangeColorIfLayerIs : MonoBehaviour
{
    [SerializeField] private GameObject layer;
    [SerializeField] private Color newColor;
    private Color baseColor;
    private TextMeshProUGUI text;
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        baseColor = text.color;
    }
    private void Update() => text.color = layer.activeSelf ? newColor : baseColor;
}