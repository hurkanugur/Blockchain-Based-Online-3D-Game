using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class SearchItemBoard : MonoBehaviour
{
    public GameObject searchItemBoard;

    public GameObject[] gameObjects = new GameObject[4];
    private bool flag = false;

    private readonly List<GameObject> itemList = new List<GameObject>();
    public GameObject itemListContent;
    public Scrollbar itemListScrollBar;
    public GameObject itemListTextObjects;

    public Dropdown ItemTypeSelectBox;

    public Text itemID, itemName, itemOwner, ownerAddress, itemStatus;
    public InputField findItemByIdInputField;

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
                searchItemBoard.gameObject.SetActive(true);
                ItemTypeSelectBox.value = 0;
                ShowItemList();
            }
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (collider.GetComponent<PlayerManager>().playerID == Client.instance.myID)
            {
                ClearFindItemMenu();
                ClearObservingItemMenu();
                searchItemBoard.gameObject.SetActive(false);
            }
        }
    }

    public void SelectBoxHukoEvent()
    {
        if (this.gameObject.activeSelf == false)
            return;

        ClearObservingItemMenu();

        if (ItemTypeSelectBox.value == 0)
            ShowItemList();
        else if(ItemTypeSelectBox.value == 1)
            ShowItemList("Bomb");
        else if (ItemTypeSelectBox.value == 2)
            ShowItemList("Shield");
        //AddressSelectBox.captionText THIS REFERS TO SELECTED TEXT'S ITSELF
        //AddressSelectBox.value THIS REFERS TO SELECTED OPTION INDEX
        //AddressSelectBox.option[i] THIS REFERS TO SELECTED OPTION
    }

    public async void FindItemByIDButtonOnClicked()
    {
        try
        {
            findItemByIdInputField.text = findItemByIdInputField.text.Replace(".", "").Replace(",", "").Replace("-", "");
            BigInteger _itemID = BigInteger.Parse(findItemByIdInputField.text);
            findItemByIdInputField.text = string.Empty;
            HukocraftSmartContract.Item itemProperties = await HukocraftSmartContract.instance.HukocraftFindItem(_itemID);
            itemID.text = _itemID + "";
            itemName.text = itemProperties.ItemType == 0 ? "None" : (itemProperties.ItemType == 1 ? "Bomb" : "Shield");
            itemOwner.text = itemProperties.OwnerName == string.Empty ? "None" : itemProperties.OwnerName;
            ownerAddress.text = itemProperties.OwnerAddress == "0x0000000000000000000000000000000000000000" ? "None" : itemProperties.OwnerAddress;
            itemStatus.text = itemProperties.IsSlotFull == false ? (itemProperties.OwnerAddress == "0x0000000000000000000000000000000000000000" ? "NEVER CREATED" : "DESTROYED") : "AVAILABLE";
        }
        catch (System.Exception) { findItemByIdInputField.text = string.Empty; }
    }

    private void ClearObservingItemMenu()
    {
        if (itemList.Count != 0)
        {
            foreach (GameObject itemObj in itemList)
                Destroy(itemObj.gameObject);
            itemList.Clear();
        }
    }

    private void ClearFindItemMenu()
    {
        itemID.text = itemName.text = itemOwner.text = ownerAddress.text = itemStatus.text = string.Empty;
    }

    private async void ShowItemList(string filter = "default")
    {
        ClearObservingItemMenu();

        List<HukocraftSmartContract.Item> itemListResults = await HukocraftSmartContract.instance.HukocraftObserveItems();

        if (itemListResults != null)
        {
            foreach (HukocraftSmartContract.Item search in itemListResults)
            {
                if (filter.Equals("Bomb") && search.ItemType != 1)
                    continue;
                else if (filter.Equals("Shield") && search.ItemType != 2)
                    continue;

                GameObject textObjects = Instantiate(itemListTextObjects, itemListContent.transform);
                textObjects.transform.Find("ItemIDText").GetComponent<Text>().text = "  " + search.ItemID;
                textObjects.transform.Find("ItemTypeText").GetComponent<Text>().text = search.ItemType == 1 ? "Bomb" : (search.ItemType == 2 ? "Shield" : "Empty");
                textObjects.transform.Find("OwnerText").GetComponent<Text>().text = search.OwnerName;

                itemList.Add(textObjects);
            }
        }
    }
}

