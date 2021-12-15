using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class ToolChangeTextSpacing : MonoBehaviour
{
    public float characterSpacing = 0.0f;

    public void ChangeCharacterSpacing()
    {
        TMP_Text[] textBoxes = Resources.FindObjectsOfTypeAll<TMP_Text>();
        foreach (TMP_Text textBox in textBoxes)
        {
            textBox.characterSpacing = characterSpacing;
        }
    }
}
