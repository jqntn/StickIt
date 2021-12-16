using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
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
                var c0 = firstSelected[i]?.GetComponents<EnableOnSelect>();
                if (c0 != null) foreach (var item in c0) item.OnSelect(null);
                var c1 = firstSelected[i]?.GetComponents<AnimateOnSelect>();
                if (c1 != null) foreach (var item in c1) item.OnSelect(null);
                var c2 = firstSelected[i]?.GetComponents<TextColorOnSelect>();
                if (c2 != null) foreach (var item in c2) item.OnSelect(null);
                continue;
            }
            layers[i].SetActive(false);
        }
    }
    public void IncLayer(int inc)
    {
        if (parentLayer && parentLayer.activeSelf)
        {
            AkSoundEngine.PostEvent("Play_SFX_UI_Move", gameObject);
            var a = EventSystem.current.currentSelectedGameObject.GetComponents<IDeselectHandler>();
            if (a != null) foreach (var item in a) item.OnDeselect(null);
            var i = layers.FindIndex(x => x.activeSelf);
            layers[i].SetActive(false);
            var j = i + inc;
            if (j < 0) j = layers.Count - 1;
            else if (j > layers.Count - 1) j = 0;
            layers[j].SetActive(true);
            firstSelected[j]?.Select();
            var b = firstSelected[j]?.GetComponents<ISelectHandler>();
            if (b != null) foreach (var item in b) item.OnSelect(null);
        }
    }
    public void OnLeftPage(InputAction.CallbackContext context)
    { if (context.performed) IncLayer(-1); }
    public void OnRightPage(InputAction.CallbackContext context)
    { if (context.performed) IncLayer(1); }
}