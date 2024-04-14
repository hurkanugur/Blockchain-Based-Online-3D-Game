using Nethereum.Web3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ETHMenu : MonoBehaviour
{
    public GameObject startMenu;
    public InputField walletAddressField;
    public InputField privateKeyField;
    public Dropdown selectBox;
    public Button createWalletButton;
    public Button saveWalletAttemptButton;
    public Button backButton;
    public Button createAttemptButton;
    public Button saveButton;
    public Button loginButton;
    public Button loginWalletAttemptButton;
    public Text addressAreaName;
    public Text privateKeyAreaName;
    public Image fadingEffect;

    private bool isNewAccountCreated = false;

    private string createdWalletAddress;
    private string createdPrivateKey;
    readonly Dictionary<string, string> CurrentUserETHDictionary = new Dictionary<string, string>();

    public void Start()
    {
        fadingEffect.gameObject.SetActive(true);
        this.gameObject.SetActive(true);

        fadingEffect.raycastTarget = false; //MAKE SCREEN CLICKABLE (CUZ FADING IMAGE IS ABOVE ALL)
        fadingEffect.CrossFadeAlpha(0, 2, false); //FADING ANIMATION
    }

    //SAVES THE NEW ACCOUNT INTO THE FILE
    public void HukoWriteFile()
    {
        try
        {
            if (Web3.IsChecksumAddress(walletAddressField.text.Trim())) //CHECK IF THE ADDRESS IS VALID OR NOT
            {
                StreamWriter hukoFile = new StreamWriter("Hukocraft.txt", true);
                hukoFile.WriteLine(hukoFile.NewLine + walletAddressField.text.Trim() + " " + privateKeyField.text.Trim());  //-----> WRITING TO THE FILE
                hukoFile.Close();
            }
        }
        catch (Exception) { }
    }

    //READS THE SAVED ACCOUNT FROM THE FILE AND WRITES THEM IN SELECTBOX (DROPDOWN)
    public void HukoReadFile()
    {
        CurrentUserETHDictionary.Clear();
        selectBox.ClearOptions();

        if (File.Exists("Hukocraft.txt"))
        {
            StreamReader hukoFile = new StreamReader("Hukocraft.txt");
            while (!hukoFile.EndOfStream)
            {
                try
                {
                    string[] tempArray = hukoFile.ReadLine().Split(' ');  //-----> READING THE FILE
                    if (tempArray[0].Trim().Length != 0 && tempArray[1].Trim().Length != 0)
                        if (Web3.IsChecksumAddress(tempArray[0].Trim())) //---> CHECK IF THE ADDRESS IS VALID
                        {
                            CurrentUserETHDictionary.Add(tempArray[0].Trim(), tempArray[1].Trim());
                        }

                }
                catch (Exception) { }
            }
            selectBox.AddOptions(new List<string>(CurrentUserETHDictionary.Keys));
            hukoFile.Close();
        }
        else
        {
            StreamWriter hukoFile = new StreamWriter("Hukocraft.txt", false);
            hukoFile.Close();
        }

        if (CurrentUserETHDictionary.Count == 0)
        {
            List<string> temp = new List<string>();
            temp.Add("T H E R E  I S  N O  W A L L E T  S A V E D !");
            selectBox.AddOptions(temp);
        }
    }

    public void CreateWalletOnClicked()
    {
        createWalletButton.gameObject.SetActive(false);
        saveWalletAttemptButton.gameObject.SetActive(false);
        loginWalletAttemptButton.gameObject.SetActive(false);

        if (isNewAccountCreated == true)
        {
            backButton.GetComponent<RectTransform>().localPosition = new Vector3(0, -90);

            walletAddressField.text = createdWalletAddress;
            privateKeyField.text = createdPrivateKey;
            walletAddressField.readOnly = true;
            privateKeyField.readOnly = true;
            addressAreaName.gameObject.SetActive(true);
            privateKeyAreaName.gameObject.SetActive(true);
            walletAddressField.gameObject.SetActive(true);
            privateKeyField.gameObject.SetActive(true);
        }
        else
        {
            backButton.GetComponent<RectTransform>().localPosition = new Vector3(0, -30);
            createAttemptButton.gameObject.SetActive(true);
        }

        backButton.gameObject.SetActive(true);
    }

    public void CreateAttemptButtonOnClicked()
    {
        backButton.GetComponent<RectTransform>().localPosition = new Vector3(0, -90);

        backButton.enabled = false;
        createAttemptButton.gameObject.SetActive(false);

        walletAddressField.readOnly = true;
        privateKeyField.readOnly = true;

        ETHAccount.instance.CreateAccount();

        addressAreaName.gameObject.SetActive(true);
        privateKeyAreaName.gameObject.SetActive(true);

        backButton.enabled = true;

        walletAddressField.text = ETHAccount.instance.GetMyWalletAddress();
        createdWalletAddress = walletAddressField.text;
        walletAddressField.gameObject.SetActive(true);

        privateKeyField.text = ETHAccount.instance.GetMyPrivateKey();
        createdPrivateKey = privateKeyField.text;
        privateKeyField.gameObject.SetActive(true);

        HukoWriteFile();

        isNewAccountCreated = true;
    }

    public void LoginAttemptOnClicked()
    {
        createWalletButton.gameObject.SetActive(false);
        saveWalletAttemptButton.gameObject.SetActive(false);
        loginWalletAttemptButton.gameObject.SetActive(false);

        backButton.GetComponent<RectTransform>().localPosition = new Vector3(90, -80);
        backButton.gameObject.SetActive(true);
        selectBox.gameObject.SetActive(true);
        loginButton.gameObject.SetActive(true);

        HukoReadFile();
    }

    public void SaveWalletAttemptOnClicked()
    {
        addressAreaName.gameObject.SetActive(false);
        privateKeyAreaName.gameObject.SetActive(false);
        loginWalletAttemptButton.gameObject.SetActive(false);

        createWalletButton.gameObject.SetActive(false);
        saveWalletAttemptButton.gameObject.SetActive(false);

        backButton.GetComponent<RectTransform>().localPosition = new Vector3(90, -90);

        walletAddressField.gameObject.SetActive(true);
        privateKeyField.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
        saveButton.gameObject.SetActive(true);

        walletAddressField.text = "";
        walletAddressField.readOnly = false;
        privateKeyField.text = "";
        privateKeyField.readOnly = false;
    }

    public void BackButtonOnClicked()
    {
        walletAddressField.gameObject.SetActive(false);
        privateKeyField.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        saveButton.gameObject.SetActive(false);
        createAttemptButton.gameObject.SetActive(false);
        selectBox.gameObject.SetActive(false);
        loginButton.gameObject.SetActive(false);

        createWalletButton.gameObject.SetActive(true);
        saveWalletAttemptButton.gameObject.SetActive(true);
        loginWalletAttemptButton.gameObject.SetActive(true);

        walletAddressField.enabled = true;
    }

    public void SelectBoxHukoEvent()
    {
        //selectBox.captionText THIS REFERS TO SELECTED TEXT'S ITSELF
        //selectBox.value THIS REFERS TO SELECTED OPTION INDEX
        //selectBox.option[i] THIS REFERS TO SELECTED OPTION
    }

    //SAVE THE NEW ACCOUNT IN THE GAME FILE
    public void SaveButtonOnClicked()
    {
        if (walletAddressField.text.Trim().Length != 0 && privateKeyField.text.Trim().Length != 0)
        {
            HukoWriteFile();
            walletAddressField.text = string.Empty;
            privateKeyField.text = string.Empty;
        }
    }

    public void GetDailyEtherButtonOnClicked()
    {
        System.Diagnostics.Process.Start("https://faucet.dimensions.network/");
    }
    public void ShowMyAccountsButtonOnClicked()
    {
        HukoReadFile();
        System.Diagnostics.Process.Start("hukocraft.txt");
    }

    public void ExitButtonOnClicked()
    {
        Application.Quit();
    }

    public async void LoginButtonOnClicked()
    {
        if (selectBox.options[0].text.Equals("T H E R E  I S  N O  W A L L E T  S A V E D !") == false) //IF THERE IS AN ACCOUNT SAVED
        {
            ETHAccount.instance.SetAccount(selectBox.captionText.text, CurrentUserETHDictionary[selectBox.captionText.text]);

            //TRY TO GET USERNAME FROM THE BLOCKCHAIN, IF IT IS EMPTY USER WILL ENTER A NAME, IF NOT, USER CANNOT CHANGE HIS NAME
            MenuManager.instance.usernameField.text = await HukocraftSmartContract.instance.HukocraftShowUsername();
            if (MenuManager.instance.usernameField.text.Length == 0) MenuManager.instance.usernameField.enabled = true;
            else MenuManager.instance.usernameField.enabled = false;

            await Inventory.instance.CheckBlockchainAndRefreshInventory(); //GET THE CORRESPONDING USER'S INVENTORY FROM THE BLOCKCHAIN
            await ExperienceManager.instance.GetExperienceAndLevelFromBlockchain();

            fadingEffect.raycastTarget = true; //MAKE SCREEN UNCLICKABLE (CUZ FADING IMAGE IS ABOVE ALL)
            StartCoroutine(LoginAnimation());
        }
        
    }
    public IEnumerator LoginAnimation()
    {
        fadingEffect.CrossFadeAlpha(1, 1, false); //FADING ANIMATION
        yield return new WaitForSeconds(1.0f);
        this.gameObject.SetActive(false);
        startMenu.SetActive(true);
    }
}