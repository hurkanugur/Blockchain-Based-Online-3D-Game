using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    public GameObject scoreBoard;

    public GameObject[] gameObjects = new GameObject[4];
    private bool flag = false;

    private readonly List<GameObject> rankingList = new List<GameObject>();
    public GameObject rankingContent;
    public Scrollbar rankingScrollBar;
    public GameObject rankingTextObjects;

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
        if (collider.CompareTag("Player"))
        {
            if (collider.GetComponent<PlayerManager>().playerID == Client.instance.myID)
            {
                scoreBoard.gameObject.SetActive(true);
                ShowRankingList();
            }
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (collider.GetComponent<PlayerManager>().playerID == Client.instance.myID)
            {
                foreach (GameObject rankingObj in rankingList)
                    Destroy(rankingObj.gameObject);
                rankingList.Clear();

                scoreBoard.gameObject.SetActive(false);
            }
        }
    }

    //IT IS FOR RANKING TEXT FIELD
    public async void ShowRankingList()
    {
        if (rankingList.Count != 0)
        {
            foreach (GameObject rankingObj in rankingList)
                Destroy(rankingObj.gameObject);
            rankingList.Clear();
        }

        Dictionary<string, BigInteger> rankingResults = await HukocraftSmartContract.instance.HukocraftShowRanking();

        if(rankingResults != null)
        {
            int rank = 1;
            foreach (KeyValuePair<string, BigInteger> search in rankingResults)
            {
                GameObject textObjects = Instantiate(rankingTextObjects, rankingContent.transform);
                textObjects.transform.Find("rankText").GetComponent<Text>().text = "  #" + rank;
                textObjects.transform.Find("nameText").GetComponent<Text>().text = search.Key;
                textObjects.transform.Find("levelText").GetComponent<Text>().text = ExperienceManager.instance.LevelCalculation(search.Value).ToString();
                textObjects.transform.Find("expText").GetComponent<Text>().text = search.Value.ToString();

                rank++;
                rankingList.Add(textObjects);
            }
        }
    }
}
