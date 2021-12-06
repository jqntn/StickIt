using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class Tuyau : MonoBehaviour
{




    private Animator anim;
    [HideInInspector]
    public MenuSelection menuSelection;
    [HideInInspector]
    public Gamepad gamepad;
    [HideInInspector]
    public int id;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }


    public void PlayAnimation()
    {
        anim.SetTrigger("Activate");
    }



    public void CallEvent()
    {
        menuSelection.SpawnPlayer(gamepad, id);
    }

}
