using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    
    public static GameManager Instance
    {
        get {  return _instance; }
        set
        {
            if (value != _instance)
            {
                _instance = value;
            }
            else
            {
                Destroy(value);
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
