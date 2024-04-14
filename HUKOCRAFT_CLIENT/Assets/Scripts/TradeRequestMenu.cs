using UnityEngine;
using UnityEngine.UI;

public class TradeRequestMenu : MonoBehaviour
{
    public static TradeRequestMenu instance;
    public string otherPlayerName;
    public int otherPlayerID;
    public Text requestText;
    public void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
        {
            Debug.Log("[Error]: TradeRequestMenu Instance already exists, destroying object!");
            Destroy(this);
        }
    }
    public void Initialize(string _otherPlayerName, int _otherPlayerID)
    {
        otherPlayerName = _otherPlayerName;
        otherPlayerID = _otherPlayerID;
        requestText.text = otherPlayerName + " has sent you a trade request";
    }

    public void AcceptButtonOnClicked()
    {
        //IF THE OTHER PLAYER IS STILL ONLINE
        if (GameManager.playerObjects.ContainsKey(otherPlayerID))
        {
            Inventory.instance.tradeMenu.gameObject.SetActive(true);
            Inventory.isTradeMenuActivated = true;
            TradeMenu.instance.otherPlayerName.text = otherPlayerName;
            TradeMenu.instance.otherPlayerID = otherPlayerID;
            this.gameObject.SetActive(false);
            GameChat.instance.SendTradeRequest("ME", "/trade " + otherPlayerName, "ACCEPT");
        }
        else
        {
            GameChat.instance.SendTradeRequest("OTHER", "/trade " + otherPlayerName, "REFUSE");
            this.gameObject.SetActive(false);
        }
    }
    public void RefuseButtonOnClicked()
    {
        GameChat.instance.SendTradeRequest("ME", "/trade " + otherPlayerName, "REFUSE");
        this.gameObject.SetActive(false);
    }
}
