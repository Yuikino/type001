using UnityEngine;

public abstract class SingletonUtil<T> : MonoBehaviour where T : SingletonUtil<T>
{
    public static T Instance { get; private set; }

    protected void Awake()
    {
        Instance = (T) this;
    }
}