using UnityEngine;

public class PlayerData :   MonoBehaviour 
{
    public string playerName = "Player"; 
    public static PlayerData Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
           
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
