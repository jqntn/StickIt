using UnityEngine;
class Unique : MonoBehaviour
{
    Unique _instance;
    void Awake() { if (_instance == null) { _instance = this; DontDestroyOnLoad(gameObject); } else Destroy(gameObject); }
}