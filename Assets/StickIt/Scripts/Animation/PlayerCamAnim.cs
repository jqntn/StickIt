using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamAnim : MonoBehaviour
{
    public GameObject canvasCameraPlayer;
    public GameObject[] playerStartPos;
    public bool hasFinish = false;
    public float depthOffset = -0.03f;
    public Vector3 offset = new Vector3(.0f, .0f, .0f);
    [Header("DEBUG____________________")]
    [SerializeField] private Vector3 pos2Go = new Vector3(.0f, .0f, .0f);
    //private void Awake()
    //{
    //    canvasCameraPlayer.SetActive(false);
    //    playerStartPos = GameObject.FindGameObjectsWithTag("StartPos");
    //}

    //private IEnumerator Start()
    //{
    //    hasFinish = false;
    //    canvasCameraPlayer.SetActive(true);
    //    yield return new WaitForSeconds(2.0f);
    //    hasFinish = true;
    //}

    //private void Update()
    //{
    //    if (!hasFinish) return;

    //    // how to calculate position it should go to ?
    //    // Dependence : 
    //    //      Player Pos
    //    //      Camera Viewport = frustum
    //    //      
    //    // if player outside of viewport + margin 
    //    //  
    //    pos2Go.y = playerStartPos[0].transform.position.y;
    //    pos2Go.z = playerStartPos[0].transform.position.x;
    //    pos2Go.x = depthOffset;
        
    //    canvasCameraPlayer.GetComponent<Canvas>().GetComponent<RectTransform>().localPosition = pos2Go;
    //}

    //private void OnDrawGizmos()
    //{
        
    //}
}
