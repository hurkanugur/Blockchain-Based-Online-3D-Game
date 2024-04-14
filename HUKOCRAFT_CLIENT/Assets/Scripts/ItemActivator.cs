using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemActivator : MonoBehaviour
{
    public static float sightDistance = 100f;
    public static GameObject player;
    public List<GameObject> hukoObjects = new List<GameObject>();

    public GameObject portalAsylumObj;
    
    public void Start()
    {
        hukoObjects.Add(portalAsylumObj);
        portalAsylumObj.SetActive(false);
    }

    public void Update() 
    {
        try
        {
            if (player == null)
            {
                player = GameManager.playerObjects[Client.instance.myID].gameObject;
                StartCoroutine(CheckActivation());
            }
        }
        catch (System.Exception) { }
    }

    private IEnumerator CheckActivation()
    {
        try
        {
            foreach (GameObject search in hukoObjects)
            {
                if (search == null)
                    hukoObjects.Remove(search);
                else
                {
                    if (search.CompareTag("PlayerBasicBomb"))
                    {
                        if (Vector3.Distance(player.transform.position, search.transform.position) > sightDistance)
                        {
                            try
                            {
                                if (search.transform.Find("Explosion(Clone)").gameObject.activeSelf == true)
                                {
                                    Destroy(search);
                                    hukoObjects.Remove(search);
                                }
                            }
                            catch (System.Exception) { search.SetActive(false); }
                        }
                        else
                            search.SetActive(true);
                    }
                    else
                    {
                        if (Vector3.Distance(player.transform.position, search.transform.position) > sightDistance)
                            search.gameObject.SetActive(false);
                        else
                            search.gameObject.SetActive(true);
                    }
                }
            }
        }
        catch (System.Exception) {}
        yield return new WaitForSeconds(0.01f);
        StartCoroutine(CheckActivation());
    }
}
