using UnityEngine;
using System.Collections;

public class ItemDisabler : MonoBehaviour
{
    public void Start()
    {
        StartCoroutine(AddToList());
    }

    private IEnumerator AddToList()
    {
        try
        {
            GameObject.Find("ItemActivatorObject").GetComponent<ItemActivator>().hukoObjects.Add(this.gameObject);
        }
        catch (System.Exception) { StartCoroutine(AddToList()); }
        yield break;
    }
}