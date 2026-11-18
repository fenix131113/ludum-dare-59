using UnityEngine;
using UnityEngine.UI;

public class Monitor : MonoBehaviour
{
    [SerializeField] private GameObject gameGO;

    public void ShowGame()
    {
        gameGO.SetActive(true);
    }
}
