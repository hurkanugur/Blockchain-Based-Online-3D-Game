# Blockchain Based Online 3D Game

## 🎮 Overview
A blockchain-based online 3D MMORPG developed using **Unity** and the **Ethereum Ropsten test network** as an undergraduate graduation project.  

This game allows players to:  
- Endlessly **level up**  
- Query **inventory and items** stored on the blockchain  
- Securely **trade items and Ether** with other players through smart contracts  

---

## 📹 Project Demo
[![Video](https://img.youtube.com/vi/gCaed4vt2Qo/0.jpg)](https://www.youtube.com/watch?v=gCaed4vt2Qo)

---

## ✨ Features
- **Unity-powered MMORPG** with immersive 3D gameplay  
- **Blockchain-backed inventory** — all items stored on-chain  
- **Player progression system** with continuous leveling  
- **Decentralized trading** — items & Ether exchanged securely via smart contracts  
- **Transparent and trustless logic** handled by Solidity contracts  

---

## 🛠 Tech Stack
- **Unity (C#)** for 3D client-side game development  
- **Solidity** for smart contracts on Ethereum Ropsten testnet  
- **MetaMask / Web3** for blockchain interaction  

---

## 📂 Project Structure
```
/ (root)
├── HUKOCRAFT_CLIENT/                  		# Unity client project
├── HUKOCRAFT_SERVER/                  		# Backend server for the game
├── HukocraftSmartContract.sol         		# Solidity smart contract
├── Blockchain Based Online 3D Game.pdf   	# Project documentation overview
├── Hurkan_Ugur_Graduation_Project.pdf    	# Graduation thesis/report
└── README.md                          		# This documentation
```

---

## 🚀 Getting Started

### Prerequisites
- [Unity](https://unity.com/) (supported version)  
- [MetaMask](https://metamask.io/) wallet connected to Ropsten  
- [Infura](https://infura.io/) or another Ethereum RPC provider  

### Setup
1. Clone the repository:
   ```bash
   git clone https://github.com/hurkanugur/Blockchain-Based-Online-3D-Game.git
   cd Blockchain-Based-Online-3D-Game
   ```
2. Deploy or connect to the **Solidity contract** (`HukocraftSmartContract.sol`) on Ropsten.  
3. Update Unity client with the deployed contract **address** and **ABI**.  
4. Open the Unity project (`HUKOCRAFT_CLIENT/`), configure RPC/wallet settings, and run.  

---

## ⚙️ How It Works
1. Player actions trigger Unity client functions.  
2. The client queries the smart contract for player levels and inventory.  
3. Trades and transfers are executed as Ethereum transactions.  
4. The smart contract ensures trustless validation and persistence.  

---

## 🔮 Future Improvements
- Migration from **Ropsten** to Ethereum mainnet or L2 (Polygon, Arbitrum, etc.)  
- Expanded gameplay: quests, combat, multiplayer zones  
- Integration with **IPFS** for decentralized storage  
- Cross-platform support (WebGL, mobile)  

---

## 🙏 Acknowledgments
- Academic supervisors and mentors  
- Unity and Ethereum developer communities  
- Open-source Web3 libraries and tools  

---

## 📜 License
This project is licensed under the **MIT License**.  
See the [LICENSE](./LICENSE) file for details.
