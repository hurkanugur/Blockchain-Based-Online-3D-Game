using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TradeMenu : MonoBehaviour
{
    public static TradeMenu instance;
    public Button myInventorySlot;
    public Button otherInventorySlot;
    public Button myLockButton, otherLockButton, myConfirmButton, otherConfirmButton;
    public InputField myInputField, otherInputField;
    public Button closeButton;
    public Text otherPlayerName;
    public int otherPlayerID;
    public int itemIndexFromMyInventory;
    public int itemCodeToBeSent;

    public bool isMyLockButtonClicked = false;
    private bool isOtherLockButtonClicked = false;
    private bool isMyConfirmButtonClicked = false;
    private bool isOtherConfirmButtonClicked = false;
    private bool isItemExchangeBegun = false;
    private bool isOtherSucceeded = false;
    private bool amISucceeded = false;
    private bool isOtherFailed = false;
    private bool amIFailed = false;

    public void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
        {
            Debug.Log("[Error]: Trade Menu Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void Start()
    {
        closeButton.gameObject.SetActive(true);
        isItemExchangeBegun = false;

        itemCodeToBeSent = 0;
        itemIndexFromMyInventory = 100;

        myLockButton.interactable = myInputField.interactable = myInventorySlot.interactable = true;
        myInputField.text = otherInputField.text = string.Empty;

        isOtherSucceeded = amISucceeded = isOtherFailed = amIFailed = false;

        myConfirmButton.gameObject.SetActive(false);
        otherConfirmButton.gameObject.SetActive(false);

        myLockButton.GetComponent<Image>().color = new Color(63 / 255f, 63 / 255f, 63 / 255f);
        myConfirmButton.GetComponent<Image>().color = new Color(63 / 255f, 63 / 255f, 63 / 255f);
        otherLockButton.GetComponent<Image>().color = new Color(63 / 255f, 63 / 255f, 63 / 255f);
        otherConfirmButton.GetComponent<Image>().color = new Color(63 / 255f, 63 / 255f, 63 / 255f);

        isMyLockButtonClicked = isOtherLockButtonClicked = isMyConfirmButtonClicked = isOtherConfirmButtonClicked = false;

        myInventorySlot.GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().color = Inventory.instance.PNGBackgroundEmptyColor;
        myInventorySlot.GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().texture = null;
        myInventorySlot.GetComponent<Image>().color = Inventory.instance.emptySlotColor;

        otherInventorySlot.GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().color = Inventory.instance.PNGBackgroundEmptyColor;
        otherInventorySlot.GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().texture = null;
        otherInventorySlot.GetComponent<Image>().color = Inventory.instance.emptySlotColor;

        ClientSend.PlayerShieldActivated("Protection");
        StartCoroutine(CheckTradeMenuConnectionRequirements());
    }

    public IEnumerator CheckTradeMenuConnectionRequirements()
    {
        yield return new WaitForSeconds(2);
        try
        {
            if (GameManager.playerObjects.ContainsKey(otherPlayerID) == false)
            {
                TradeMenuEventHandler("/transfer " + Client.instance.myID + " RECEIVE", "EXIT_BUTTON");
            }
            else if (isOtherFailed == true || amIFailed == true)
            {
                GameChat.instance.SendChatMessage("Trade", "The transaction is failed.", true);
                Inventory.isTradeMenuActivated = false;
                this.gameObject.SetActive(false);
            }
            else if (isOtherSucceeded == true && amISucceeded == true)
            {
                GameChat.instance.SendChatMessage("Trade", "The transaction is completed successfully.");
                Inventory.isTradeMenuActivated = false;
                this.gameObject.SetActive(false);
            }
            else
                StartCoroutine(CheckTradeMenuConnectionRequirements());
        }
        catch (System.Exception) { }
    }

    public void UpdateTradeMenuInventorySlot(bool isMyInventorySlot, int itemID)
    {
        Button tempInventorySlot = isMyInventorySlot == true ? myInventorySlot : otherInventorySlot;
        string itemName = string.Empty;

        if (itemID == 1) //IF IT IS A BOMB
        {
            tempInventorySlot.GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().texture = Inventory.instance.bomb;
            tempInventorySlot.GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().color = Inventory.instance.PNGBackgroundFullColor;
            tempInventorySlot.GetComponent<Image>().color = Inventory.instance.minedAddSlotColor;
            itemName = "Bomb";
        }
        else if (itemID == 2) //IF IT IS A SHIELD
        {
            tempInventorySlot.GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().texture = Inventory.instance.shield;
            tempInventorySlot.GetComponentInChildren<Image>().GetComponentInChildren<RawImage>().color = Inventory.instance.PNGBackgroundFullColor;
            tempInventorySlot.GetComponent<Image>().color = Inventory.instance.minedAddSlotColor;
            itemName = "Shield";
        }

        if (isMyInventorySlot == true)
        {
            myInventorySlot = tempInventorySlot;
            itemCodeToBeSent = itemID;
            GameChat.instance.SendChatMessage("Trade", "You have put x1 [" + itemName + "] item in the trade slot.");
            TradeMenuEventHandler("SEND", "ITEM_SLOT", itemID.ToString());
        }
        else
        {
            otherInventorySlot = tempInventorySlot;
            GameChat.instance.SendChatMessage("Trade", otherPlayerName.text + " has put x1 [" + itemName + "] item in the trade slot.");
        }

    }

    public async void UpdateMyETHAmountField()
    {
        decimal myTotalBalance = await ETHAccount.instance.GetAccountBalance();
        if (myInputField.text != string.Empty)
        {
            myInputField.text = myInputField.text.Replace('.', ','); //IF THE USER TYPES ".", CONVERT IT TO ","
            myInputField.text = myInputField.text.Replace("-", ""); //MAKE IT NON NEGATIVE
            if (System.Convert.ToDecimal(myInputField.text) > myTotalBalance)
                myInputField.text = myTotalBalance + "";
            if (myInputField.text == "0")
                myInputField.text = string.Empty;
        }
        TradeMenuEventHandler("SEND", "INPUT_FIELD", myInputField.text);
    }

    //THE COMMAND TYPE: "/transfer RECEIVER_ID OPERATION COMPONENT VALUE"
    //OPERATION => SEND, RECEIVE
    //COMPONENT => LOCK_BUTTON, CONFIRM_BUTTON, INPUT_FIELD, EXIT_BUTTON, ITEM_SLOT, SUCCESS, FAILURE, INVENTORY_FULL
    public void TradeMenuEventHandler(string operation, string component, string value = "")
    {
        if (operation.Equals("SEND") == true)
        {
            operation = "/transfer " + otherPlayerID + " RECEIVE";
            ClientSend.PlayerSendChatMessage(operation + " " + component + " " + value);
        }
        else if (operation.Equals("/transfer " + Client.instance.myID + " RECEIVE") == true)
        {
            if (component.Equals("LOCK_BUTTON") == true)
                OtherLockButtonOnClicked();
            else if (component.Equals("CONFIRM_BUTTON") == true)
                OtherConfirmButtonOnClicked();
            else if (component.Equals("INPUT_FIELD") == true)
                otherInputField.text = value;
            else if (component.Equals("EXIT_BUTTON") == true)
            {
                GameChat.instance.SendChatMessage("Trade", otherPlayerName.text + " has canceled the trade.", true);
                Inventory.isTradeMenuActivated = false;
                this.gameObject.SetActive(false);
            }
            else if (component.Equals("ITEM_SLOT") == true)
                UpdateTradeMenuInventorySlot(false, int.Parse(value));
            else if (component.Equals("SUCCESS") == true)
            {
                isOtherSucceeded = true;
            }
            else if (component.Equals("FAILURE") == true)
            {
                GameChat.instance.SendChatMessage("Trade", otherPlayerName.text + " has failed on the transaction.", true);
                isOtherFailed = true;
            }
            else if (component.Equals("INVENTORY_FULL") == true)
            {
                GameChat.instance.SendChatMessage("Trade", otherPlayerName.text + " has no empty space in their inventory.", true);
                isOtherFailed = true;
            }
        }
    }

    public void MyLockButtonOnClicked()
    {
        TradeMenuEventHandler("SEND", "LOCK_BUTTON");
        GameChat.instance.SendChatMessage("Trade", "You have locked your trade menu changes.");
        isMyLockButtonClicked = true;
        myLockButton.interactable = myInputField.interactable = myInventorySlot.interactable = false;
        myLockButton.GetComponent<Image>().color = new Color(222 / 255f, 184 / 255f, 135 / 255f);
        if (isOtherLockButtonClicked == true)
        {
            //IF THE OTHER PERSON WILL TRANSFER AN ITEM WHILE MY INVENTORY IS FULL, PREVENT THE TRANSACTION!
            if (Inventory.instance.currentItemAmount == 9 && otherInventorySlot.GetComponent<Image>().color == Inventory.instance.minedAddSlotColor)
            {
                GameChat.instance.SendChatMessage("Trade", "Your inventory is full.", true);
                TradeMenuEventHandler("SEND", "INVENTORY_FULL");
                amIFailed = true;
            }
            myConfirmButton.interactable = true;
            myConfirmButton.gameObject.SetActive(true);
            otherConfirmButton.gameObject.SetActive(true);
        }
    }

    public void OtherLockButtonOnClicked()
    {
        GameChat.instance.SendChatMessage("Trade", otherPlayerName.text + " has locked their trade menu changes.");
        isOtherLockButtonClicked = true;
        otherLockButton.GetComponent<Image>().color = new Color(222 / 255f, 184 / 255f, 135 / 255f);
        if (isMyLockButtonClicked == true)
        {
            //IF THE OTHER PERSON WILL TRANSFER AN ITEM WHILE MY INVENTORY IS FULL, PREVENT THE TRANSACTION!
            if (Inventory.instance.currentItemAmount == 9 && otherInventorySlot.GetComponent<Image>().color == Inventory.instance.minedAddSlotColor)
            {
                GameChat.instance.SendChatMessage("Trade", "Your inventory is full.", true);
                TradeMenuEventHandler("SEND", "INVENTORY_FULL");
                amIFailed = true;
            }

            myLockButton.interactable = false;
            myConfirmButton.interactable = true;
            myConfirmButton.gameObject.SetActive(true);
            otherConfirmButton.gameObject.SetActive(true);
        }
    }

    public void MyConfirmButtonOnClicked()
    {
        TradeMenuEventHandler("SEND", "CONFIRM_BUTTON");
        GameChat.instance.SendChatMessage("Trade", "You have confirmed the transaction to begin.");
        isMyConfirmButtonClicked = true;
        myConfirmButton.interactable = false;
        myConfirmButton.GetComponent<Image>().color = new Color(222 / 255f, 184 / 255f, 135 / 255f);
        if (isOtherConfirmButtonClicked == true)
        {
            //IF THE OTHER PLAYER IS STILL ONLINE (DIDN'T QUIT THE GAME)
            if (GameManager.playerObjects.ContainsKey(otherPlayerID))
                ItemExchangeBegins();
            else
                TradeMenuEventHandler("/transfer " + Client.instance.myID + " RECEIVE", "EXIT_BUTTON");
        }
    }

    public void OtherConfirmButtonOnClicked()
    {
        GameChat.instance.SendChatMessage("Trade", otherPlayerName.text + " has confirmed the transaction to begin.");
        isOtherConfirmButtonClicked = true;
        otherConfirmButton.GetComponent<Image>().color = new Color(222 / 255f, 184 / 255f, 135 / 255f);
        if (isMyConfirmButtonClicked == true)
        {
            //IF THE OTHER PLAYER IS STILL ONLINE (DIDN'T QUIT THE GAME)
            if (GameManager.playerObjects.ContainsKey(otherPlayerID))
                ItemExchangeBegins();
            else
                TradeMenuEventHandler("/transfer " + Client.instance.myID + " RECEIVE", "EXIT_BUTTON");
        }
    }

    private async void ItemExchangeBegins()
    {
        try
        {
            if (isItemExchangeBegun == false)
            {
                //TRANSACTION BEGINS
                closeButton.gameObject.SetActive(false);
                isItemExchangeBegun = true;
                GameChat.instance.SendChatMessage("Trade", "The transaction begins...");

                //IF I PUT AN ITEM TO TRANSFER TO THE OTHER PLAYER
                if (myInventorySlot.GetComponent<Image>().color == Inventory.instance.minedAddSlotColor)
                {
                    decimal receiveEtherAmount = otherInputField.text == string.Empty ? 0 : System.Convert.ToDecimal(otherInputField.text);
                    if (await HukocraftSmartContract.instance.HukocraftItemExchange(GameManager.playerObjects[otherPlayerID].ETHAccountAddress, receiveEtherAmount, itemIndexFromMyInventory, itemCodeToBeSent) == true)
                    {
                        if (myInputField.text != string.Empty)
                        {
                            if (await ETHAccount.instance.EtherTransfer(GameManager.playerObjects[otherPlayerID].ETHAccountAddress, myInputField.text) == true)
                            {
                                GameChat.instance.SendChatMessage("Trade", "The ether transfer is completed successfully.");
                                TradeMenuEventHandler("SEND", "SUCCESS");
                                amISucceeded = true;
                            }
                            else
                            {
                                GameChat.instance.SendChatMessage("Trade", "The ether transfer is failed.", true);
                                TradeMenuEventHandler("SEND", "FAILURE");
                                amIFailed = true;
                            }
                        }
                        else
                        {
                            GameChat.instance.SendChatMessage("Trade", "The item transfer is completed successfully.");
                            TradeMenuEventHandler("SEND", "SUCCESS");
                            amISucceeded = true;
                        }
                    }
                    else
                    {
                        GameChat.instance.SendChatMessage("Trade", "The item transfer is failed.", true);
                        TradeMenuEventHandler("SEND", "FAILURE");
                        amIFailed = true;
                    }
                }
                else if (myInputField.text != string.Empty)
                {
                    if (await ETHAccount.instance.EtherTransfer(GameManager.playerObjects[otherPlayerID].ETHAccountAddress, myInputField.text) == true)
                    {
                        GameChat.instance.SendChatMessage("Trade", "The ether transfer is completed successfully.");
                        TradeMenuEventHandler("SEND", "SUCCESS");
                        amISucceeded = true;
                    }
                    else
                    {
                        GameChat.instance.SendChatMessage("Trade", "The ether transfer is failed.", true);
                        TradeMenuEventHandler("SEND", "FAILURE");
                        amIFailed = true;
                    }
                }
                //I SEND NEITHER AN ITEM NOR ETHER
                else
                {
                    TradeMenuEventHandler("SEND", "SUCCESS");
                    amISucceeded = true;
                }
            }
        }
        catch (System.Exception)
        {
            GameChat.instance.SendChatMessage("Trade", "You have failed on the transfer.", true);
            TradeMenuEventHandler("SEND", "FAILURE");
            amIFailed = true;
        }
    }

    public void CloseButtonOnClicked()
    {
        GameChat.instance.SendChatMessage("Trade", "You have canceled the trade.", true);
        TradeMenuEventHandler("SEND", "EXIT_BUTTON");
        Inventory.isTradeMenuActivated = false;
        this.gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        Start();
    }

    public async void OnDisable()
    {
        if (amISucceeded == true && isOtherSucceeded == true)
        {
            if (otherInventorySlot.GetComponent<Image>().color == Inventory.instance.minedAddSlotColor)
                Inventory.instance.currentItemAmount++;
            if (myInventorySlot.GetComponent<Image>().color == Inventory.instance.minedAddSlotColor)
                Inventory.instance.currentItemAmount--;
        }
        await ETHAccount.instance.ShowAccountBalance();
        await Inventory.instance.CheckBlockchainAndRefreshInventory();
        ClientSend.PlayerSendChatMessage("/inventory_current_item_number " + Inventory.instance.currentItemAmount);
        ClientSend.PlayerShieldActivated("Deactivate");
    }
}