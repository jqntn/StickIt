using UnityEngine;

[CreateAssetMenu]
public class OptionsSlime : ScriptableObject
{
    public string label;
    public int Id;
    public bool selected;
    public enum OptionTypes
    {
        BOOL,
        SKIN,
        FLOAT,
        INT
    }
    public OptionTypes optionType;
    public bool optionOn;
}
