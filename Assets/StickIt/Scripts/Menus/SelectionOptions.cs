using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
public class SelectionOptions : MonoBehaviour
{
    public List<List<OptionsSlime>> doubleList;
    public List<OptionsSlime> optionsDirection;
    public List<OptionsSlime> optionsSkin;
    public GameObject selectOptions;
    public TMP_Text textOption;
    public int indexPlayer;
    int indexX = 0, indexY = 0;
    bool playerAdded = false;
    private void Start()
    {
        doubleList = new List<List<OptionsSlime>>();
        doubleList.Add(optionsDirection);
        doubleList.Add(optionsSkin);
        for(int i = 0; i < doubleList.Count; i++)
        {
            for(int j = 0; j < doubleList[i].Count; j++)
            {
                doubleList[i][j].Id = j;
            }
        }
        selectOptions.SetActive(false);
    }
    private void Update()
    {

        if (MultiplayerManager.instance.players.Count - 1 >= indexPlayer && MultiplayerManager.instance.players[indexPlayer])
        {
           /* ChangeOption();
            ChangeTypeOption();*/
            if (!playerAdded)
            {
                selectOptions.SetActive(true);
                UpdateDisplay();
                playerAdded = true;
            }
        }

    }
    void UpdateDisplay()
    {
        textOption.text = doubleList[indexY][indexX].label;
    }
    public void ChangeOption()
    {
        if (Gamepad.all[MultiplayerManager.instance.players[indexPlayer].myDatas.deviceID].dpad.right.isPressed)
        {
            doubleList[indexY][indexX].selected = false;
            indexX++;
            if (indexX >= doubleList[indexY].Count)
            {
                indexX = 0;
            }
            if(doubleList[indexY][indexX].optionType == OptionsSlime.OptionTypes.BOOL)
            {
                if(indexX == 0 ? MultiplayerManager.instance.players[indexPlayer].myMouvementScript.isReversedDirection = false : MultiplayerManager.instance.players[indexPlayer].myMouvementScript.isReversedDirection = true);
            }else if(doubleList[indexY][indexX].optionType == OptionsSlime.OptionTypes.SKIN)
            {
                Debug.Log("Skin : " + doubleList[indexY][indexX].label);
            }
            doubleList[indexY][indexX].selected = true;
            UpdateDisplay();
        }
        else if (Gamepad.all[MultiplayerManager.instance.players[indexPlayer].myDatas.deviceID].dpad.left.isPressed)
        {
            doubleList[indexY][indexX].selected = false;
            indexX--;
            if (indexX <= 0)
            {
                indexX = doubleList[indexY].Count - 1;
            }
            doubleList[indexY][indexX].selected = true;
            UpdateDisplay();
        }
    }
    public void ChangeTypeOption()
    {
        if (Gamepad.all[MultiplayerManager.instance.players[indexPlayer].myDatas.deviceID].dpad.down.isPressed)
        {

            indexY++;
            if (indexY >= doubleList.Count)
            {
                indexY = 0;
            }
            indexX = doubleList[indexY].Find(x => x.selected == true).Id;
            UpdateDisplay();
        }
        else if (Gamepad.all[MultiplayerManager.instance.players[indexPlayer].myDatas.deviceID].dpad.up.isPressed)
        {
            indexY--;
            if (indexY <= 0)
            {
                indexY = doubleList.Count - 1;
            }
            indexX = doubleList[indexY].Find(x => x.selected == true).Id;
            UpdateDisplay();
        }
    }
}