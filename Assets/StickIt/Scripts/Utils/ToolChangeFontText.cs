using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class ToolChangeFontText : MonoBehaviour
{
    public TMP_FontAsset font;

    public void ChangeFontText()
    {
        TMP_Text[] textBoxes = Resources.FindObjectsOfTypeAll<TMP_Text>();
        foreach (TMP_Text textBox in textBoxes)
        {
            textBox.font = font;
        }
    }
}
