using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RunnerEndCheckpoint : MonoBehaviour
{
    public RunnerManager runnerManager;
    [Header("-------- DEBUG ---------")]
    [SerializeField] private BoxCollider boxCollider;
    private void Awake()
    {
        if(runnerManager == null)
        {
            runnerManager = FindObjectOfType<RunnerManager>();
        }

        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponentInParent<Player>();
        if (player != null)
        {
            if (!runnerManager.GetOrder().Contains(player))
            {
                runnerManager.AddOrder(player);
            }
        }
    }
}
