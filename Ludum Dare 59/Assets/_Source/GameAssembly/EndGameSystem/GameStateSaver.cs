using UnityEngine;

public class GameStateSaver : MonoBehaviour
{
    public static GameStateSaver Instance;

    public bool IsRestarted;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        } 

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
}
