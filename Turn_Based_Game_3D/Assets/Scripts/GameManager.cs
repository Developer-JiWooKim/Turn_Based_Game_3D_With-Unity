using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (Instance == null)
        {
            Instance = this;
            Initialize();
        }
    }
}
