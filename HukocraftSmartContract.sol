// SPDX-License-Identifier: MIT
pragma solidity >=0.0.0;

contract Hukocraft {
    uint8 constant INV_LENGTH = 9;
    uint256 BLOCKCHAIN_ITEMID_COUNTER = 0;
    
    struct Item {
        bool isSlotFull;
        uint256 itemID;
        uint8 itemType; // 0: EMPTY, 1: BOMB, 2: SHIELD
        string ownerName;
        address ownerAddress;
    }
    
    struct Player {
        string username;
        uint256 experience;
        Item[INV_LENGTH] inventory;
    }
    
    mapping(address => Player) private currentPlayer;
    mapping(address => uint256) private playerIndex;
    mapping(uint256 => Item) private currentItem;
    address[] private playerAddresses;
    
    function CharacterInitialization(string memory _username) external returns (string memory) {
        require(bytes(currentPlayer[msg.sender].username).length == 0, 'The user already exists on the blockchain.');
        
        currentPlayer[msg.sender].username = _username;
        currentPlayer[msg.sender].experience = 0;
        
        playerAddresses.push(msg.sender);
        
        uint256 id = playerAddresses.length - 1;
        playerIndex[msg.sender] = id;
        
        return "A new player is created on the blockchain.";
    }

    function InventoryAddItem(uint8 _itemIndex, uint8 _itemCode) external {
        require(_itemIndex >= 0 && _itemIndex < INV_LENGTH, 'Invalid Inventory Index.');
        require(currentPlayer[msg.sender].inventory[_itemIndex].isSlotFull == false, 'The inventory slot is not empty.');
        
        // SAVE THE NEW ITEM
        BLOCKCHAIN_ITEMID_COUNTER++;
        currentPlayer[msg.sender].inventory[_itemIndex].isSlotFull = true;
        currentPlayer[msg.sender].inventory[_itemIndex].itemType = _itemCode;
        currentPlayer[msg.sender].inventory[_itemIndex].itemID = BLOCKCHAIN_ITEMID_COUNTER;
        currentPlayer[msg.sender].inventory[_itemIndex].ownerName = currentPlayer[msg.sender].username;
        currentPlayer[msg.sender].inventory[_itemIndex].ownerAddress = msg.sender;
        
        // UPDATE CURRENTITEM MAP
        currentItem[currentPlayer[msg.sender].inventory[_itemIndex].itemID] = currentPlayer[msg.sender].inventory[_itemIndex];
    }

    function InventoryRemoveItem(uint8 _itemIndex) external {
        require(_itemIndex >= 0 && _itemIndex < INV_LENGTH, 'Invalid Inventory Index.');
        require(currentPlayer[msg.sender].inventory[_itemIndex].isSlotFull != false, 'The inventory slot is already empty.');
        
        currentPlayer[msg.sender].inventory[_itemIndex].isSlotFull = false;
        currentItem[currentPlayer[msg.sender].inventory[_itemIndex].itemID] = currentPlayer[msg.sender].inventory[_itemIndex];
    }

    function ShowInventory() external view returns (uint8[] memory) {
        uint8;
        for (uint8 i = 0; i < INV_LENGTH; i++) {
            if (currentPlayer[msg.sender].inventory[i].isSlotFull == true) {
                inventorySlots[i] = currentPlayer[msg.sender].inventory[i].itemType;
            }
        }
        return inventorySlots;
    }

    function ContractEtherTransfer(uint256 _etherAmountInWei) external payable returns (uint256) {
        require(address(this).balance >= _etherAmountInWei, 'The contract has not enough Ether to send.');
        (payable(msg.sender)).transfer(_etherAmountInWei);
        return msg.value;
    }

    function PlayerEtherTransfer() external payable returns (uint256) {
        return msg.value;
    }

    function ShowPlayerBalance() external view returns (uint256) {
        return msg.sender.balance;
    }

    function ShowContractBalance() external view returns (uint256) {
        return address(this).balance;
    }

    function ItemExchange(address payable _sendToWallet, uint256 _receiveEtherAmount, uint8 _myItemIndex, uint8 _itemCode) external payable {
        // CHECK THE OTHER USER'S ACCOUNT WHETHER HE HAS ENOUGH ETHERS TO SEND ME OR NOT
        require(address(_sendToWallet).balance >= _receiveEtherAmount, 'The other player doesn\'t have enough Ether.');
        
        // IF I WILL SEND AN ITEM
        if (_itemCode != 0) {
            bool isSucceeded = false;
            for (uint8 i = 0; i < INV_LENGTH; i++) {
                if (currentPlayer[_sendToWallet].inventory[i].isSlotFull == false) {
                    // REMOVE THE ITEM FROM ME
                    currentPlayer[msg.sender].inventory[_myItemIndex].isSlotFull = false;
                    
                    // UPDATE THE INVENTORY ITEM OF THE RECEIVER
                    currentPlayer[_sendToWallet].inventory[i].isSlotFull = true;
                    currentPlayer[_sendToWallet].inventory[i].itemType = _itemCode;
                    currentPlayer[_sendToWallet].inventory[i].itemID = currentPlayer[msg.sender].inventory[_myItemIndex].itemID;
                    currentPlayer[_sendToWallet].inventory[i].ownerName = currentPlayer[_sendToWallet].username;
                    currentPlayer[_sendToWallet].inventory[i].ownerAddress = _sendToWallet;
                    
                    // UPDATE THE CURRENTITEM MAP
                    currentItem[currentPlayer[_sendToWallet].inventory[i].itemID] = currentPlayer[_sendToWallet].inventory[i];
                    
                    isSucceeded = true;
                    break;
                }
            }
            require(isSucceeded == true, 'The other player doesn\'t have enough space.');
        }
    }

    function ObserveItems() external view returns (Item[] memory) {
        uint256 counter = 0;
        for (uint256 i = 1; i <= BLOCKCHAIN_ITEMID_COUNTER; i++) {
            if (currentItem[i].isSlotFull == true)
                counter++;
        }
        
        Item[] memory items = new Item[](counter);
        
        counter = 0;
        for (uint256 i = 1; i <= BLOCKCHAIN_ITEMID_COUNTER; i++) {
            if (currentItem[i].isSlotFull == true) {
                items[counter] = currentItem[i];
                counter++;
            }
        }

        return items;
    }

    function FindItem(uint256 _itemID) external view returns (Item memory) {
        return currentItem[_itemID];
    }

    function ObserveInventory(address _userAddress) external view returns (uint8[] memory) {
        uint8;
        for (uint8 i = 0; i < INV_LENGTH; i++) {
            if (currentPlayer[_userAddress].inventory[i].isSlotFull == true) {
                inventorySlots[i] = currentPlayer[_userAddress].inventory[i].itemType;
            }
        }
        return inventorySlots;
    }

    function GetAllPlayers() external view returns (string[] memory, address[] memory) {
        string[] memory playerNames = new string[](playerAddresses.length);
        for (uint256 i = 0; i < playerAddresses.length; i++) {
            playerNames[i] = currentPlayer[playerAddresses[i]].username;
        }

        return (playerNames, playerAddresses);
    }

    function ShowUsername() external view returns (string memory) {
        require(bytes(currentPlayer[msg.sender].username).length != 0, 'The user does not exist on the blockchain.');
        return currentPlayer[msg.sender].username;
    }

    function ChangeUsername(string memory _newUsername) external payable returns (string memory) {
        require(msg.value == 1 ether, 'You don\'t have enough ether to change your name.');
        currentPlayer[msg.sender].username = _newUsername;
        return _newUsername;
    }

    function SetCharacterExperience(uint256 _expAmount, bool isAddition) external {
        if (isAddition == true)
            currentPlayer[msg.sender].experience += _expAmount;
        else
            currentPlayer[msg.sender].experience -= _expAmount;
    }

    function ShowCharacterExperience() external view returns (uint256) {
        return currentPlayer[msg.sender].experience;
    }

    function ShowRanking() view external returns (string[] memory, uint256[] memory) {
        string[] memory names = new string[](playerAddresses.length);
        uint256[] memory experiences = new uint256[](playerAddresses.length);
        
        for (uint256 i = 0; i < playerAddresses.length; i++) {
            names[i] = currentPlayer[playerAddresses[i]].username;
            experiences[i] = currentPlayer[playerAddresses[i]].experience;
        }
        
        return (names, experiences);
    }
}