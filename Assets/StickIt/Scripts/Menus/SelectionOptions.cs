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

    bool inputInstantiated;
    PlayerInput pi;
    private InputAction m_ArrowDown;
    private InputAction m_ArrowUp;
    private InputAction m_ArrowLeft;
    private InputAction m_ArrowRight;
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
            if (!inputInstantiated)
            {
                InstantiateInputs();
                inputInstantiated = true;
            }
            ChangeOption();
            //ChangeTypeOption();
            if (!playerAdded)
            {
                selectOptions.SetActive(true);
                UpdateDisplay();
                playerAdded = true;
            }
        }
    }
    public void InstantiateInputs()
    {
        pi = MultiplayerManager.instance.players[indexPlayer].GetComponent<PlayerInput>();
        m_ArrowDown = pi.actions["ArrowDown"];
        m_ArrowLeft = pi.actions["ArrowLeft"];
        m_ArrowRight = pi.actions["ArrowRight"];
        m_ArrowUp = pi.actions["ArrowUp"];
    }
    void UpdateDisplay()
    {
        textOption.text = doubleList[indexY][indexX].label;
    }
    public void ChangeOption()
    {
        if (m_ArrowRight.triggered)
        {
            doubleList[indexY][indexX].selected = false;
            indexX++;
            if (indexX >= doubleList[indexY].Count)
            {
                indexX = 0;
            }
            if(doubleList[indexY][indexX].optionType == OptionsSlime.OptionTypes.BOOL)
            {
                MultiplayerManager.instance.players[indexPlayer].MyMouvementScript.isReversedDirection = indexX == 0 ?  false : true;
            }else if(doubleList[indexY][indexX].optionType == OptionsSlime.OptionTypes.SKIN)
            {
                //Debug.Log("Skin : " + doubleList[indexY][indexX].label);
            }
            doubleList[indexY][indexX].selected = true;
            UpdateDisplay();
        }
        else if (m_ArrowLeft.triggered)
        {
            doubleList[indexY][indexX].selected = false;
            indexX--;
            if (indexX < 0)
            {
                indexX = doubleList[indexY].Count - 1;
            }
            if (doubleList[indexY][indexX].optionType == OptionsSlime.OptionTypes.BOOL)
            {
                MultiplayerManager.instance.players[indexPlayer].MyMouvementScript.isReversedDirection = indexX == 0 ? false : true;
            }
            else if (doubleList[indexY][indexX].optionType == OptionsSlime.OptionTypes.SKIN)
            {
                Debug.Log("Skin : " + doubleList[indexY][indexX].label);
            }
            doubleList[indexY][indexX].selected = true;
            UpdateDisplay();
        }
    }
    public void ChangeTypeOption()
    {
        if (m_ArrowDown.triggered)
        {

            indexY++;
            if (indexY >= doubleList.Count)
            {
                indexY = 0;
            }
            if(doubleList[indexY].Find(x => x.selected == true))
                indexX = doubleList[indexY].Find(x => x.selected == true).Id;
            UpdateDisplay();
        }
        else if (m_ArrowUp.triggered)
        {
            indexY--;
            if (indexY < 0)
            {
                indexY = doubleList.Count - 1;
            }
            if (doubleList[indexY].Find(x => x.selected == true))
                indexX = doubleList[indexY].Find(x => x.selected == true).Id;
            UpdateDisplay();
        }
    }
}