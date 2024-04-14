using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class GameChat : MonoBehaviour
{
    public static GameChat instance;
    public GameObject tradeRequestMenuGameObject;

    public GameObject textPrefab;
    public InputField textWritingField;

    public Text announcement;

    private readonly List<GameObject> chatMessageList = new List<GameObject>();
    public GameObject chatContent;
    public Scrollbar chatScrollBar;


    private readonly List<GameObject> ethereumMessageList = new List<GameObject>();
    public GameObject ethereumContent;
    public Scrollbar ethereumScrollBar;
    

    public static bool isChatActivated = false;

    public void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
        {
            Debug.Log("[Error]: GameChat Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (textWritingField.interactable == false)
            {
                textWritingField.Select();
                textWritingField.interactable = true;
                textWritingField.ActivateInputField();
                isChatActivated = true;
            }
            else
            {
                if (textWritingField.text.Trim().Length != 0)
                {
                    //SEND ALL PLAYERS YOUR MESSAGE
                    if (textWritingField.text.Trim().StartsWith("/trade"))
                        SendTradeRequest("ME", textWritingField.text.Trim(), "REQUEST");
                    else
                    {
                        ClientSend.PlayerSendChatMessage(textWritingField.text.Trim());
                        SendChatMessage(GameManager.playerObjects[Client.instance.myID].playerUsername, textWritingField.text.Trim());
                    }
                }
                textWritingField.text = string.Empty;
                textWritingField.DeactivateInputField();
                textWritingField.interactable = false;
                isChatActivated = false;
            }
        }
        else
        {
            if (textWritingField.isFocused == false && textWritingField.interactable == true)
            {
                textWritingField.interactable = false;
                isChatActivated = false;
                textWritingField.text = string.Empty;
            }
        }
    }

    //IT IS FOR HOW MUCH EXP YOU GAINED OR LOST (TOP CENTER TEXT AREA)
    private bool isAnimationVisible;
    private float A = 0;
    private IEnumerator AnnouncementAnimation()
    {
        yield return new WaitForSeconds(0.001f);

        if (isAnimationVisible == false && A < 255f)
        {
            A += 15;
            announcement.color = new Color(222 / 255.0f, 184 / 255.0f, 135 / 255.0f, A / 255.0f);

            if (A == 255)
            {
                yield return new WaitForSeconds(2f); //TEXT WILL BE SHOWN FOR 5 SECONDS
                isAnimationVisible = true;
            }
            StartCoroutine(AnnouncementAnimation());
        }
        else if (isAnimationVisible == true && A > 0f)
        {
            A -= 15;
            announcement.color = new Color(222 / 255.0f, 184 / 255.0f, 135 / 255.0f, A / 255.0f);
            if (A > 0)
            {
                StartCoroutine(AnnouncementAnimation());
            }
            else
                isAnimationVisible = false;
        }
    }

    //IT IS FOR HOW MUCH EXP YOU GAINED OR LOST (TOP CENTER TEXT AREA)
    public void Announcement(string _announcementText)
    {
        StopCoroutine(AnnouncementAnimation());
        isAnimationVisible = false;
        A = 0f;
        announcement.color = new Color(222 / 255.0f, 184 / 255.0f, 135 / 255.0f, A);
        announcement.text = _announcementText;
        StartCoroutine(AnnouncementAnimation());
    }


    //REQUEST FORMAT:  /trade USER_NAME_WHO_WE_WANNA_MAKE_TRADE OPERATION_NAME
    //OPERATIONS: REQUEST ACCEPT REFUSE
    public void SendTradeRequest(string sender, string tradeMessage, string operation)
    {
        bool sendTradeMessageToOtherPlayer = true;

        if (chatMessageList.Count > 10)
        {
            Destroy(chatMessageList[0].gameObject);
            chatMessageList.RemoveAt(0);
        }
        GameObject textObj = Instantiate(textPrefab, chatContent.transform);
        textObj.GetComponent<Text>().color = Color.blue;

        try
        {
            if (tradeMessage.StartsWith("/trade ") == false)
            {
                textObj.GetComponent<Text>().color = Color.red;
                textObj.GetComponent<Text>().text = "You have mistyped the /trade command.\nDid you mean /trade PlayerName ?";
                sendTradeMessageToOtherPlayer = false;
            }
            else
            {
                //MAKE SURE THERE WILL BE NO ERROR CAUSED BY TURKISH CHARACTERS
                if (tradeMessage.Contains("ý"))
                    tradeMessage = tradeMessage.Replace("ý", "i");
                if (tradeMessage.Contains("Ý"))
                    tradeMessage = tradeMessage.Replace("Ý", "i");
                tradeMessage = tradeMessage.ToUpper(new System.Globalization.CultureInfo("en-US", false));

                if (sender.Equals("ME") == true)
                {
                    string command = "/trade ";
                    tradeMessage = tradeMessage.Substring(7);
                    string userName;
                    try { userName = tradeMessage.Substring(0, tradeMessage.IndexOf(" ")); }
                    catch (System.Exception) { userName = tradeMessage; }
                    int userID = GameManager.playerObjects.FirstOrDefault(x => x.Value.playerUsername.Equals(userName)).Key;

                    if (userID == 0)
                    {
                        throw new System.Exception("The player \"" + userName + "\" is not online.");
                    }
                    else if(userID == Client.instance.myID)
                    {
                        throw new System.Exception("You cannot send a trade request to yourself.");
                    }


                    //I SEND TRADE REQUEST TO SOMEONE
                    if (operation.Equals("REQUEST"))
                    {
                        textObj.GetComponent<Text>().text = "You have sent a trade request to " + userName + ".";
                        tradeMessage = command + userID + " REQUEST";
                    }
                    //I NOTIFY SOMEONE THAT I HAVE ACCEPTED HIS TRADE REQUEST (AND OPEN THE TRADE MENU)
                    else if (operation.Equals("ACCEPT"))
                    {
                        textObj.GetComponent<Text>().text = "You have accepted the trade request of " + userName + ".";
                        tradeMessage = command + userID + " ACCEPT";
                    }
                    //I NOTIFY SOMEONE THAT I HAVE REFUSED HIS TRADE REQUEST
                    else if (operation.Equals("REFUSE"))
                    {
                        textObj.GetComponent<Text>().color = Color.red;
                        textObj.GetComponent<Text>().text = "You have refused the trade request of " + userName + ".";
                        tradeMessage = command + userID + " REFUSE";
                    }
                }
                else if (sender.Equals("OTHER") == true)
                {
                    tradeMessage = tradeMessage.Substring(7);
                    string userID = tradeMessage.Substring(0, tradeMessage.IndexOf(" "));
                    string userName = GameManager.playerObjects[int.Parse(userID)].playerUsername;
                    operation = tradeMessage.Substring(tradeMessage.IndexOf(" ") + 1);

                    //SOMEONE SEND ME A TRADE REQUEST (OPEN TRADE OFFER MENU)
                    if (operation.Equals("REQUEST"))
                    {
                        //IF YOU ARE ALREADY IN A TRADE, REFUSE THE REQUEST
                        if (tradeRequestMenuGameObject.gameObject.activeSelf == true || Inventory.instance.tradeMenu.gameObject.activeSelf == true)
                        {
                            tradeMessage = "/trade " + userName;
                            SendTradeRequest("ME", tradeMessage, "REFUSE");
                            return;
                        }
                        else
                        {
                            textObj.GetComponent<Text>().text = userName + " has sent you a trade request.";

                            //ACTIVATE TRADE REQUEST MENU
                            tradeRequestMenuGameObject.gameObject.SetActive(true);
                            TradeRequestMenu.instance.Initialize(userName, int.Parse(userID));
                        }
                    }
                    //SOMEONE NOTIFIES ME THAT HE ACCEPTED MY TRADE REQUEST (OPEN TRADE OFFER MENU)
                    else if (operation.Equals("ACCEPT"))
                    {
                        textObj.GetComponent<Text>().text = userName + " has accepted your trade request.";
                        Inventory.instance.tradeMenu.gameObject.SetActive(true);
                        Inventory.isTradeMenuActivated = true;
                        TradeMenu.instance.otherPlayerName.text = userName;
                        TradeMenu.instance.otherPlayerID = int.Parse(userID);
                    }
                    //I NOTIFY SOMEONE THAT I HAVE REFUSED HIS TRADE REQUEST
                    else if (operation.Equals("REFUSE"))
                    {
                        textObj.GetComponent<Text>().color = Color.red;
                        textObj.GetComponent<Text>().text = userName + " has refused your trade request.";
                    }

                    sendTradeMessageToOtherPlayer = false;
                }
            }
        }
        catch (System.Exception ex)
        {
            textObj.GetComponent<Text>().color = Color.red;
            textObj.GetComponent<Text>().text = ex.Message;
            tradeRequestMenuGameObject.gameObject.SetActive(false);
            sendTradeMessageToOtherPlayer = false;
        }

        chatScrollBar.value = 0; //SCROLLBAR'S POSITION WILL BE SET TO BOTTOM WHEN A NEW MESSAGE ARRIVES
        chatMessageList.Add(textObj);

        if (sendTradeMessageToOtherPlayer)
            ClientSend.PlayerSendChatMessage(tradeMessage);
    }

    //IT IS FOR LEFT BOTTOM TEXT AREA (GAME CHATTING)
    public void SendChatMessage(string _senderName, string _message, bool failure = false)
    {
        if (chatMessageList.Count > 10)
        {
            Destroy(chatMessageList[0].gameObject);
            chatMessageList.RemoveAt(0);
        }
        GameObject textObj = Instantiate(textPrefab, chatContent.transform);

        if (_senderName.Equals("Server"))
        {
            textObj.GetComponent<Text>().text = _message;
            textObj.GetComponent<Text>().color = Color.cyan;
        }
        else if (_senderName.Equals("Trade"))
        {
            if(failure == true)
                textObj.GetComponent<Text>().color = Color.red;
            else
                textObj.GetComponent<Text>().color = Color.blue;
            textObj.GetComponent<Text>().text = _message;
        }
        else
        {
            textObj.GetComponent<Text>().text = _senderName + ": " + _message;
            textObj.GetComponent<Text>().color = new Color(222 / 255f, 184 / 255f, 135 / 255f, 1f);
        }
        chatScrollBar.value = 0; //SCROLLBAR'S POSITION WILL BE SET TO BOTTOM WHEN A NEW MESSAGE ARRIVES
        chatMessageList.Add(textObj);
    }

    //IT IS FOR RIGHT BOTTOM TEXT AREA (ETHEREUM INFORMATION FIELD)
    public void SendEthereumMessage(string _message, bool istransactionSucceeded)
    {
        if (ethereumMessageList.Count > 10)
        {
            Destroy(ethereumMessageList[0].gameObject);
            ethereumMessageList.RemoveAt(0);
        }
        GameObject textObj = Instantiate(textPrefab, ethereumContent.transform);

        textObj.GetComponent<Text>().text = _message;
        if (istransactionSucceeded == true)
            textObj.GetComponent<Text>().color = Color.cyan;
        else
            textObj.GetComponent<Text>().color = Color.red;
        ethereumScrollBar.value = 0; //SCROLLBAR'S POSITION WILL BE SET TO BOTTOM WHEN A NEW MESSAGE ARRIVES
        ethereumMessageList.Add(textObj);
    }



}