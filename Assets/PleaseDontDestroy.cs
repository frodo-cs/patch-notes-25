using UnityEngine;

public class PleaseDontDestroy : MonoBehaviour
{
    private static PleaseDontDestroy instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
