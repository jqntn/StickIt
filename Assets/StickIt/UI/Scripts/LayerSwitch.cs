using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class LayerSwitch : MonoBehaviour
{
    [SerializeField] private GameObject parentLayer;
    [SerializeField] private GameObject activeOnStart;
    [SerializeField] private List<GameObject> layers;
    [SerializeField] private List<Selectable> firstSelected;
    private Dictionary<string, GameObject> layersDictionary;
    private void Start() => layersDictionary = layers.ToDictionary(x => x.name, x => x);
    private void OnEnable()
    { if (activeOnStart) ChangeLayer(activeOnStart.name); }
    public void ChangeLayer(string layer)
    {
        for (var i = 0; i < layers.Count; i++)
        {
            if (layers[i].name == layer)
            {
                if (!layers[i].activeSelf) layers[i].SetActive(true);
                firstSelected[i]?.Select();
                continue;
            }
            layers[i].SetActive(false);
        }
    }
    public void IncLayer(int inc)
    {
        if (parentLayer && parentLayer.activeSelf)
        {
            var i = layers.FindIndex(x => x.activeSelf);
            layers[i].SetActive(false);
            var j = i + inc;
            if (j < 0) j = layers.Count - 1;
            else if (j > layers.Count - 1) j = 0;
            layers[j].SetActive(true);
            firstSelected[j]?.Select();
        }
    }
    public void OnLeftPage(InputAction.CallbackContext context)
    { if (context.performed) IncLayer(-1); }
    public void OnRightPage(InputAction.CallbackContext context)
    { if (context.performed) IncLayer(1); }
}