using System.Collections;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public const int MAX_PLAYER_NUMBER = 30;
    public static NetworkManager instance;
    public GameObject playerPrefab;
    public static Vector3 playerInitialSpawnPosition = new Vector3(0.0f, 50.0f, 0.0f);

    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Debug.Log("[Error]: Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void ExitButtonOnClicked()
    {
        OnApplicationQuit();
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public void Start()
    {
        //PROVIDES LOWER CPU 
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        Server.Start(MAX_PLAYER_NUMBER, 26000);
    }

    public Player InstantiatePlayer() //RETURNS THE REFERANCE TO THE PLAYER
    {
        return Instantiate(playerPrefab, playerInitialSpawnPosition, Quaternion.identity).GetComponent<Player>();
    }
}
