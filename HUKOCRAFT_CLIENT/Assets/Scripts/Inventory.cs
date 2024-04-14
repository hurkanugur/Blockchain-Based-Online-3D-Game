using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public Button[] itemSlotButton = new Button[9];
    public Texture bomb, shield;

    public int currentItemAmount = 0;

    public Color emptySlotColor;
    public Color minedAddSlotColor;
    public Color minedRemoveSlotColor;
    public Color transactionSlotColor;
    public Color PNGBackgroundFullColor;
    public Color PNGBackgroundEmptyColor;

    public GameObject tradeMenu; 
    public static bool isTradeMenuActivated = false;

    public void Awake()
    { 
        if (instance == null)
            instance = this;

        else if (instance != this)
        {
            Debug.Log("[Error]: Inventory Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void Start()
    {
        transactionSlotColor = Color.red;
        minedRemoveSlotColor = Color.blue;
        minedAddSlotColor = new Color(39 / 255f, 39 / 255f, 39 / 255f, 255 / 255f);
        emptySlotColor = new Color(40 / 255f, 40 / 255f, 40 / 255f, 255 / 255f);
        PNGBackgroundEmptyColor = new Color(60 / 255f, 60 / 255f, 60 / 255f, 255 / 255f);
        PNGBackgroundFullColor = new Color(1, 1, 1, 1); //MUST BE WHITE WHEN THERE IS AN ITEM SO THAT PNG COLOR CAN BE ORIGINAL COLOR
    }

    public void Update()
    {
        if (Cursor.visible == false && GameChat.isChatActivated == false
            && ETHAccount.isSendEtherAmountFieldActivated == false && Inventory.isTradeMenuActivated == false)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                _ = InventoryAttemptToRemoveItem(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                _ = InventoryAttemptToRemoveItem(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                _ = InventoryAttemptToRemoveItem(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                _ = InventoryAttemptToRemoveItem(3);
            else if (Input.GetKeyDown(KeyCode.Alpha5))
                _ = InventoryAttemptToRemoveItem(4);
            else if (Input.GetKeyDown(KeyCode.Alpha6))
                _ = InventoryAttemptToRemoveItem(5);
            else if (Input.GetKeyDown(KeyCode.Alpha7))
                _ = InventoryAttemptToRemoveItem(6);
            else if (Input.GetKeyDown(KeyCode.Alpha8))
                _ = InventoryAttemptToRemoveItem(7);
            else if (Input.GetKeyDown(KeyCode.Alpha9))
                _ = InventoryAttemptToRemoveItem(8);
        }
        else if(isTradeMenuActivated == true)
        {
            //KEEP THE SHIELD ON
            if(GameManager.playerObjects[Client.instance.myID].ShieldGameObject.activeSelf == false)
                ClientSend.PlayerShieldActivated("Protection");

            //IF THE TRADE MENU'S INPUT FIELD IS NOT FOCUSED AND GAME CHAT IS NOT ACTIVATED
            if (TradeMenu.instance.myInputField.isFocused == false && GameChat.isChatActivated == false) 
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    TradeMenuItemSelection(0);
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                    TradeMenuItemSelection(1);
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                    TradeMenuItemSelection(2);
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                    TradeMenuItemSelection(3);
                else if (Input.GetKeyDown(KeyCode.Alpha5))
                    TradeMenuItemSelection(4);
                else if (Input.GetKeyDown(KeyCode.Alpha6))
                    TradeMenuItemSelection(5);
                else if (Input.GetKeyDown(KeyCode.Alpha7))
                    TradeMenuItemSelection(6);
                else if (Input.GetKeyDown(KeyCode.Alpha8))
                    TradeMenuItemSelection(7);
                else if (Input.GetKeyDown(KeyCode.Alpha9))
                    TradeMenuItemSelection(8);
            }
        }
    }

    private async void TradeMenuItemSelection(int _index)
    {
        //IF THE ITEMS ARE NOT LOCKED, ALLOW TO CHANGE THE ITEM
        if (TradeMenu.instance.isMyLockButtonClicked == false)
        {
            await CheckBlockchainAndRefreshInventory();

            //IF THE ITEM IS MINED AND NOT USED
            if (itemSlotButton[_index].GetComponent<Image>().color == minedAddSlotColor)
            {
                TradeMenu.instance.itemIndexFromMyInventory = _index;
                if (itemSlotButton[_index].GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().texture == bomb)
                {
                        TradeMenu.instance.UpdateTradeMenuInventorySlot(true, 1);
                }
                else if (itemSlotButton[_index].GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().texture == shield)
                {
                        TradeMenu.instance.UpdateTradeMenuInventorySlot(true, 2);
                }
            }
            //IF THE ITEM IS IN USE
            else
            {
                GameChat.instance.Announcement("Inventory slot " + (_index + 1) + " is not valid");
            }
        }
        //IF THE ITEMS ARE LOCKED, DON'T ALLOW TO CHANGE THE ITEM
        else
        {
            GameChat.instance.Announcement("You cannot attempt to change the trade slot");
        }
    }

    public async Task CheckBlockchainAndRefreshInventory()
    {
        int[] inventory = await HukocraftSmartContract.instance.HukocraftShowInventory();

        if (inventory != null)
        {
            currentItemAmount = 0;
            for (int i = 0; i < 9; i++)
            {
                if (inventory[i] != 0) //IF THE CORRESPONDING INDEX IS NOT EMPTY
                {
                    //IF IT IS NOT EMPTY BUT IT LOOKS LIKE EMPTY
                    if(itemSlotButton[i].GetComponent<Image>().color == emptySlotColor 
                        || itemSlotButton[i].GetComponent<Image>().color == minedAddSlotColor)
                    {
                        if (inventory[i] == 1) //IF IT IS A BOMB
                        {
                            itemSlotButton[i].GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().texture = bomb;
                            itemSlotButton[i].GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().color = PNGBackgroundFullColor;
                            itemSlotButton[i].GetComponent<Image>().color = minedAddSlotColor;
                        }
                        else if (inventory[i] == 2) //IF IT IS A SHIELD
                        {
                            itemSlotButton[i].GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().texture = shield;
                            itemSlotButton[i].GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().color = PNGBackgroundFullColor;
                            itemSlotButton[i].GetComponent<Image>().color = minedAddSlotColor;
                        }
                    }
                }
                else
                {
                    //IF IT IS EMPTY BUT IT LOOKS LIKE FULL
                    if (itemSlotButton[i].GetComponent<Image>().color == minedAddSlotColor)
                    {
                        itemSlotButton[i].GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().color = PNGBackgroundEmptyColor;
                        itemSlotButton[i].GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().texture = null;
                        itemSlotButton[i].GetComponent<Image>().color = emptySlotColor;
                    }
                }

                //TO CALCULATE HOW MANY ITEMS DO WE HAVE, WE COUNT ACCORDING TO THE COLOR
                if (itemSlotButton[i].GetComponent<Image>().color != emptySlotColor)
                    currentItemAmount++;
            }
        }
    }

    public async Task InventoryAttemptToAddItem(int _itemCode)
    {
        await CheckBlockchainAndRefreshInventory(); //CHECK THE BLOCKCHAIN AND THEN REFRESH THE INVENTORY
        int index = -1;

        for (int i = 0; i < 9; i++) //FIND THE MOST CONVENIENT INDEX FOR THE NEW ITEM
        {
            if (itemSlotButton[i].GetComponentInChildren<Image>().color == emptySlotColor)
            {
                index = i;
                break;
            }
        }

        if (index == -1) return; //MEANING THAT THE INVENTORY IS FULL

        if (itemSlotButton[index].GetComponent<Image>().color == emptySlotColor) //IF THE INVENTORY EMPTY
        {
            itemSlotButton[index].GetComponent<Image>().color = transactionSlotColor;
            currentItemAmount++;
            if (await HukocraftSmartContract.instance.HukocraftInventoryAddItem(_itemCode, index) == true)
            {
                itemSlotButton[index].GetComponent<Image>().color = minedAddSlotColor; //MEANING: IT IS MINED ON THE BLOCKCHAIN

                //IF IT IS BOMB
                if (_itemCode == 1)
                    itemSlotButton[index].GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().texture = bomb;
                //IF IT IS SHIELD
                else if (_itemCode == 2)
                    itemSlotButton[index].GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().texture = shield;

                itemSlotButton[index].GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().color = PNGBackgroundFullColor;

            }
            else
            {
                currentItemAmount--;
                itemSlotButton[index].GetComponent<Image>().color = emptySlotColor;
            }
                
        }
    }

    public async Task InventoryAttemptToRemoveItem(int _index = 0)
    {
        if (itemSlotButton[_index].GetComponent<Image>().color == minedAddSlotColor) //MEANING: IF THE INVENTORY NOT EMPTY
        {
            itemSlotButton[_index].GetComponent<Image>().color = transactionSlotColor;

            if (await HukocraftSmartContract.instance.HukocraftInventoryRemoveItem(_index) == true)
            {
                itemSlotButton[_index].GetComponent<Image>().color = minedRemoveSlotColor; //MEANING: ITEM IS READY TO BE USED (REMOVED ON THE BLOCKCHAIN)
                itemSlotButton[_index].GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().color = PNGBackgroundFullColor;
            }
            else
            {
                itemSlotButton[_index].GetComponent<Image>().color = minedAddSlotColor; //MAKE IT LIKE FULL, SO THAT IT WILL GET REMOVED
                await CheckBlockchainAndRefreshInventory(); //CHECK THE BLOCKCHAIN AND THEN REFRESH THE INVENTORY
            }
                
        }
        else if (itemSlotButton[_index].GetComponent<Image>().color == minedRemoveSlotColor) //MEANING: ITEM IS READY TO BE USED (REMOVED ON THE BLOCKCHAIN)
        {
            if (GameManager.playerObjects[Client.instance.myID].playerCurrentHealth > 0.0f)
            {
                bool useItem = false;

                //IF IT IS BOMB
                if (itemSlotButton[_index].GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().texture == bomb)
                {
                    useItem = true;
                    ClientSend.PlayerThrowItem(FindObjectOfType<PlayerController>().shootingPlace.position, FindObjectOfType<PlayerController>().shootingPlace.rotation);
                }
                 
                //IF IT IS SHIELD
                else if (itemSlotButton[_index].GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().texture == shield)
                {
                    //IF THE SHIELD IS OFF, THEN CONSUME THE ITEM
                    if (GameManager.playerObjects[Client.instance.myID].ShieldGameObject.activeSelf == false)
                    {
                        useItem = true;
                        ClientSend.PlayerShieldActivated("Normal");
                        GameManager.playerObjects[Client.instance.myID].ChangeShieldState(true, "Normal");
                    }
                    else
                        GameChat.instance.Announcement("The shield is already activated");
                }

                //IF SAFELY-USE MODE IS CONFIRMED
                if(useItem == true)
                {
                    itemSlotButton[_index].GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().color = PNGBackgroundEmptyColor;
                    itemSlotButton[_index].GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().texture = null;
                    itemSlotButton[_index].GetComponent<Image>().color = emptySlotColor; //RESET THE COLOR
                }
                
            }
        }
    }
}
