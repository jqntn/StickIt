using UnityEngine;
public class AutoSelectHelp : MonoBehaviour
{
    private void OnEnable()
    { if (MapManager.instance && MapManager.instance.curMod == "MusicalChairs") GetComponent<LayerSwitch>().IncLayer(1); }
}