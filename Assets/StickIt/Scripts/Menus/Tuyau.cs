using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class Tuyau : MonoBehaviour
{




    private Animator anim;
    public MenuSelection menuSelection;

    public Gamepad gamepad;
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
