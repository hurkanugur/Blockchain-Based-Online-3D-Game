using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ObserveInventoryBoard : MonoBehaviour
{
    public GameObject observeInventoryBoard;
    public Dropdown AddressSelectBox;

    public GameObject[] gameObjects = new GameObject[4];
    private bool flag = false;

    //KEY: "ADDRESS" | VALUE: "PLAYERNAME (ADDRESS)"
    private Dictionary<string, string> playerAddressesAndNames = new Dictionary<string, string>();
    public Button[] inventorySlots = new Button[9];

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

    public async void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (collider.GetComponent<PlayerManager>().playerID == Client.instance.myID)
            {
                observeInventoryBoard.gameObject.SetActive(true);
                await UpdatePlayerAccountAddresses();

            }
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (collider.GetComponent<PlayerManager>().playerID == Client.instance.myID)
            {
                ClearObservingInventory();
                AddressSelectBox.ClearOptions();
                observeInventoryBoard.gameObject.SetActive(false);
            }
        }
    }

    //IN GAME ETHEREUM WALLET LIST FOR ONLINE PLAYERS
    private async Task UpdatePlayerAccountAddresses()
    {
        AddressSelectBox.ClearOptions();
        playerAddressesAndNames.Clear();
        playerAddressesAndNames = await HukocraftSmartContract.instance.HukocraftGetAllPlayerAddressesAndNames();
        AddressSelectBox.AddOptions(new List<string>(playerAddressesAndNames.Values));
        AddressSelectBox.options.Insert(0, new Dropdown.OptionData("SELECT A WALLET TO OBSERVE AN INVENTORY"));
        AddressSelectBox.value = 0;
        AddressSelectBox.captionText.text = AddressSelectBox.options[0].text;
    }

    private void ClearObservingInventory()
    {
        for (int i = 0; i < 9; i++)
        {
            if (inventorySlots[i] != null)
            {
                inventorySlots[i].GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().texture = null;
                inventorySlots[i].GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().color = Inventory.instance.PNGBackgroundEmptyColor;
                inventorySlots[i].GetComponent<Image>().color = Inventory.instance.emptySlotColor;
            }
        }
    }
    public void SelectBoxHukoEvent()
    {
        if (this.gameObject.activeSelf == false)
            return;

        ClearObservingInventory();

        if (AddressSelectBox.value != 0)
        {
            string keyOfValue = playerAddressesAndNames.FirstOrDefault(element => element.Value == AddressSelectBox.captionText.text).Key;
            FillObservingInventory(keyOfValue);

        }
        //AddressSelectBox.captionText THIS REFERS TO SELECTED TEXT'S ITSELF
        //AddressSelectBox.value THIS REFERS TO SELECTED OPTION INDEX
        //AddressSelectBox.option[i] THIS REFERS TO SELECTED OPTION
    }
    private async void FillObservingInventory(string _playerAddress)
    {
        int[] slots = await HukocraftSmartContract.instance.HukocraftObserveInventory(_playerAddress);

        if (slots != null)
        {
            for (int i = 0; i < 9; i++)
            {
                //IF IT IS A BOMB
                if (slots[i] == 1)
                {
                    inventorySlots[i].GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().texture = Inventory.instance.bomb;
                    inventorySlots[i].GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().color = Inventory.instance.PNGBackgroundFullColor;
                    inventorySlots[i].GetComponent<Image>().color = Inventory.instance.minedAddSlotColor;
                }
                else if (slots[i] == 2)
                {
                    inventorySlots[i].GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().texture = Inventory.instance.shield;
                    inventorySlots[i].GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().color = Inventory.instance.PNGBackgroundFullColor;
                    inventorySlots[i].GetComponent<Image>().color = Inventory.instance.minedAddSlotColor;
                }
            }
        }
    }
}
