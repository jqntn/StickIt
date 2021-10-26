using UnityEngine;
abstract class Unique<T> : MonoBehaviour where T : class
{
    public static T instance;
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = GameObject.FindObjectOfType(typeof(T)) as T;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
}