using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Nethereum.Web3;
using Nethereum.HdWallet;
using NBitcoin;
using System.Threading.Tasks;

public class ETHAccount : MonoBehaviour
{
	public static ETHAccount instance;
	private string myAddress;
	private string myPrivateKey;
	private Nethereum.Web3.Accounts.Account account;
	private Web3 web3;
	private readonly string ropstenProjectWebAddress = "https://ropsten.infura.io/v3/63aaa845a76948d09e01595fe2092c85";

	public InputField myAddressField, myETHField, sendEtherAmountField;
	public Dropdown AddressSelectBox;
	public Button detailsButton, sendETHButton;

	public static bool isSendEtherAmountFieldActivated = false;

	public static Dictionary<int, string> playersETHAccounts = new Dictionary<int, string>(); //PLAYER ID - ETH ACCOUNT
	public static Dictionary<int, string> playersNamedETHAccounts = new Dictionary<int, string>(); //PLAYER ID - ETH ACCOUNT

	public void Awake()
	{
		if (instance == null)
			instance = this;

		else if (instance != this)
		{
			Debug.Log("[Error]: ETHAccount Instance already exists, destroying object!");
			Destroy(this);
		}
	}

	public void Update()
	{
		if (Inventory.isTradeMenuActivated == true)
		{
			if (sendEtherAmountField.interactable == true || sendETHButton.interactable == true)
			{
				sendEtherAmountField.interactable = sendETHButton.interactable = false;
				isSendEtherAmountFieldActivated = false;
				sendEtherAmountField.text = string.Empty;
			}
		}
		else
		{
			if (sendEtherAmountField.interactable == false || sendETHButton.interactable == false)
				sendEtherAmountField.interactable = sendETHButton.interactable = true;

			if (sendEtherAmountField.isFocused)
				isSendEtherAmountFieldActivated = true;
			else
			{
				isSendEtherAmountFieldActivated = false;
			}
		}
	}

	//IN GAME ETHEREUM WALLET LIST FOR ONLINE PLAYERS
	public void UpdateOnlinePlayersETHAccounts(int _playerID, string _playerETHAccountAddress, bool _addNewPlayer)
	{
		AddressSelectBox.ClearOptions();

		//SAVE NEW PLAYER'S ETHEREUM ACCOUNT
		if (_addNewPlayer == true)
		{
			string playerNamedAccountString = GameManager.playerObjects[_playerID].playerUsername + " (" + _playerETHAccountAddress + ")";

			playersNamedETHAccounts.Add(_playerID, playerNamedAccountString);
			playersETHAccounts.Add(_playerID, _playerETHAccountAddress);
		}
		//DELETE DISCONNECTED PLAYER'S ETHEREUM ACCOUNT
		else
		{
			playersETHAccounts.Remove(_playerID);
			playersNamedETHAccounts.Remove(_playerID);
		}

		AddressSelectBox.AddOptions(new List<string>(playersNamedETHAccounts.Values));
		AddressSelectBox.options.Insert(0, new Dropdown.OptionData("SELECT A WALLET TO TRANSFER ETHER"));
		AddressSelectBox.value = 0;
		AddressSelectBox.captionText.text = AddressSelectBox.options[0].text;
	}

	public void SelectBoxHukoEvent()
	{
		//AddressSelectBox.captionText THIS REFERS TO SELECTED TEXT'S ITSELF
		//AddressSelectBox.value THIS REFERS TO SELECTED OPTION INDEX
		//AddressSelectBox.option[i] THIS REFERS TO SELECTED OPTION
	}

	public void SetAccount(string _myAddress, string _myPrivateKey)
	{
		myAddress = _myAddress;
		myPrivateKey = _myPrivateKey;
		myAddressField.text = _myAddress;

		account = new Nethereum.Web3.Accounts.Account(myPrivateKey, 3); //3 STANDS FOR ROPSTEN CHAIN ID [IT MUST BE 3 ALWAYS]
		web3 = new Web3(account, ropstenProjectWebAddress);

		this.GetComponent<HukocraftSmartContract>().Initialize(ropstenProjectWebAddress, myAddress, myPrivateKey);

		StartCoroutine(GetAccountBalanceCycle());
	}
	public async void EtherTransfer()
	{
		try
		{
			if (AddressSelectBox.value != 0)
			{
				int gasPrice = 2;

				sendEtherAmountField.text = sendEtherAmountField.text.Replace('.', ','); //IF THE USER TYPES ".", CONVERT IT TO ","
				sendEtherAmountField.text = sendEtherAmountField.text.Replace("-", ""); //MAKE IT NON NEGATIVE
				decimal amountToSend = Convert.ToDecimal(sendEtherAmountField.text);
				string addressToSend = AddressSelectBox.captionText.text.Substring(AddressSelectBox.captionText.text.IndexOf("(") + 1, 42);

				sendEtherAmountField.text = string.Empty;

				GameChat.instance.SendEthereumMessage("[Ether Transfer]: Transaction in process...", true);
				var transaction = await web3.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(addressToSend, amountToSend, gasPrice);
				GameChat.instance.SendEthereumMessage("[Ether Transfer]: Mined Successfully -> " + amountToSend + " Ether has been sent.\n[Destination address]: " + addressToSend + "\n[Transaction Hash]: " + transaction.TransactionHash, true);
				_ = ShowAccountBalance();
			}
			else
				GameChat.instance.SendEthereumMessage("[Ether Transfer]: Please Select A Wallet", false);
		}
		catch (Exception ex)
		{
			GameChat.instance.SendEthereumMessage("[Ether Transfer]: " + ex.Message, false);
		}
		sendEtherAmountField.text = string.Empty;
	}

	public async Task<bool> EtherTransfer(string addressToSend, string _amountToSendText)
	{
		try
		{
			int gasPrice = 5;

			_amountToSendText = _amountToSendText.Replace('.', ','); //IF THE USER TYPES ".", CONVERT IT TO ","
			_amountToSendText = _amountToSendText.Replace("-", ""); //MAKE IT NON NEGATIVE
			decimal amountToSend = Convert.ToDecimal(_amountToSendText);

			sendEtherAmountField.text = string.Empty;

			GameChat.instance.SendEthereumMessage("[Ether Transfer]: Transaction in process...", true);
			var transaction = await web3.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(addressToSend, amountToSend, gasPrice);

			GameChat.instance.SendEthereumMessage("[Ether Transfer]: Mined Successfully -> " + amountToSend + " Ether has been sent.\n[Destination address]: " + addressToSend + "\n[Transaction Hash]: " + transaction.TransactionHash, true);
			_ = ShowAccountBalance();
			return true;
		}
		catch (Exception ex)
		{
			GameChat.instance.SendEthereumMessage("[Ether Transfer]: " + ex.Message, false);
			return false;
		}
	}

	public void CreateAccount()
	{
		var password = "!Hukocraft123456789!";
		Mnemonic mnemo = new Mnemonic(Wordlist.English, WordCount.Twelve); //RANDOM 12 SECRET WORDS FOR METAMASK

		account = new Wallet(mnemo.ToString(), password).GetAccount(0);
		web3 = new Web3(account, ropstenProjectWebAddress);

		myAddress = account.Address;
		myPrivateKey = account.PrivateKey;
	}

	public async Task ShowAccountBalance()
	{
		try
		{
			myETHField.text = await GetAccountBalance() + " Ether";
		}
		catch (Exception ex) { GameChat.instance.SendEthereumMessage("[Balance Request]: " + ex.Message, false); }
	}

	public async Task<decimal> GetAccountBalance()
	{
		try
		{
			var balance = await web3.Eth.GetBalance.SendRequestAsync(account.Address);
			return Web3.Convert.FromWei(balance.Value);
		}
		catch (Exception ex) { GameChat.instance.SendEthereumMessage("[Balance Request]: " + ex.Message, false); return 0; }
	}

	public void GoToMyAddress()
	{
		System.Diagnostics.Process.Start("https://ropsten.etherscan.io/address/" + myAddressField.text);
	}

	public string GetMyPrivateKey()
	{
		return myPrivateKey;
	}
	public string GetMyWalletAddress()
	{
		return myAddress;
	}

	private IEnumerator GetAccountBalanceCycle()
	{
		_ = ShowAccountBalance();
		yield return new WaitForSeconds(10.0f);
		StartCoroutine(GetAccountBalanceCycle());
	}
}