using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public GameObject[] gameObjects = new GameObject[4];
    private bool flag = false;

    public void Start()
    {
        gameObjects[0].SetActive(false);
        gameObjects[1].SetActive(false);
        gameObjects[2].SetActive(false);
        gameObjects[3].SetActive(false);
    }
    
    public void FixedUpdate()
    {
        if (flag == false && FindObjectOfType<PlayerController>() != null)
        {
            gameObjects[0].SetActive(true);
            gameObjects[1].SetActive(true);
            gameObjects[2].SetActive(true);
            gameObjects[3].SetActive(true);
            flag = true;
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player"))
        {
            if (collider.GetComponent<PlayerManager>().playerID == Client.instance.myID)
            {
                Application.Quit();
            }
        }
    }
}
