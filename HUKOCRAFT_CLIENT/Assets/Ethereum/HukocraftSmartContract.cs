using System;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.Contracts;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;

public class HukocraftSmartContract : MonoBehaviour
{
    public partial class Item : ItemBase { }

    public class ItemBase
    {
        [Parameter("bool", "isSlotFull", 1)]
        public virtual bool IsSlotFull { get; set; }
        [Parameter("uint256", "itemID", 2)]
        public virtual BigInteger ItemID { get; set; }
        [Parameter("uint8", "itemType", 3)]
        public virtual byte ItemType { get; set; }
        [Parameter("string", "ownerName", 4)]
        public virtual string OwnerName { get; set; }
        [Parameter("address", "ownerAddress", 5)]
        public virtual string OwnerAddress { get; set; }
    }

    public partial class HukocraftDeployment : HukocraftDeploymentBase
    {
        public HukocraftDeployment() : base(BYTECODE) { }
        public HukocraftDeployment(string byteCode) : base(byteCode) { }
    }

    public class HukocraftDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "60806040526000805534801561001457600080fd5b5061201a806100246000396000f3fe6080604052600436106101095760003560e01c8063919167ec11610095578063d29a949611610064578063d29a94961461028c578063ebe0d1aa146102ac578063ee000b82146102cf578063f1812c2d146102f5578063f4d14bc11461031557600080fd5b8063919167ec146102175780639fade3381461022a578063b2d59b4014610257578063d1ffdc591461027957600080fd5b806334713e4e116100dc57806334713e4e1461017d5780634c4bb085146101a057806356958e06146101b55780638c07ca76146101d5578063916d5262146101f757600080fd5b80631b1448991461010e5780631ec5f39d146101395780631fd29fde14610159578063325e088714610177575b600080fd5b34801561011a57600080fd5b50610123610328565b6040516101309190611b27565b60405180910390f35b61014c610147366004611b9f565b61054c565b6040516101309190611c50565b34801561016557600080fd5b5033315b604051908152602001610130565b34610169565b34801561018957600080fd5b506101926105ea565b604051610130929190611cc2565b3480156101ac57600080fd5b5061014c6107ad565b3480156101c157600080fd5b5061014c6101d0366004611b9f565b6108cb565b3480156101e157600080fd5b506101ea610a07565b6040516101309190611d15565b34801561020357600080fd5b506101ea610212366004611d74565b610af4565b610169610225366004611d91565b610bed565b34801561023657600080fd5b5061024a610245366004611d91565b610c87565b6040516101309190611daa565b34801561026357600080fd5b50610277610272366004611dd3565b610da5565b005b34801561028557600080fd5b5047610169565b34801561029857600080fd5b506102776102a7366004611dee565b610fb5565b3480156102b857600080fd5b506102c1611019565b604051610130929190611e23565b3480156102db57600080fd5b503360009081526001602081905260409091200154610169565b34801561030157600080fd5b50610277610310366004611e6d565b611225565b610277610323366004611ea0565b611548565b6060600060015b60005481116103745760008181526003602052604090205460ff16151560011415610362578161035e81611f05565b9250505b8061036c81611f05565b91505061032f565b5060008167ffffffffffffffff81111561039057610390611b89565b6040519080825280602002602001820160405280156103e857816020015b6040805160a08101825260008082526020808301829052928201819052606080830152608082015282526000199092019101816103ae5790505b5060009250905060015b60005481116105455760008181526003602052604090205460ff1615156001141561053357600081815260036020818152604092839020835160a081018552815460ff90811615158252600183015493820193909352600282015490921693820193909352908201805491929160608401919061046e90611f20565b80601f016020809104026020016040519081016040528092919081815260200182805461049a90611f20565b80156104e75780601f106104bc576101008083540402835291602001916104e7565b820191906000526020600020905b8154815290600101906020018083116104ca57829003601f168201915b5050509183525050600491909101546001600160a01b0316602090910152825183908590811061051957610519611f55565b6020026020010181905250828061052f90611f05565b9350505b8061053d81611f05565b9150506103f2565b5092915050565b606034670de0b6b3a7640000146105c35760405162461bcd60e51b815260206004820152603060248201527f596f7520646f6e2774206861766520656e6f75676820657468657220746f206360448201526f3430b733b2903cb7bab9103730b6b29760811b60648201526084015b60405180910390fd5b33600090815260016020908152604090912083516105e392850190611971565b5090919050565b606080600060048054905067ffffffffffffffff81111561060d5761060d611b89565b60405190808252806020026020018201604052801561064057816020015b606081526020019060019003908161062b5790505b50905060005b60045481101561074457600160006004838154811061066757610667611f55565b60009182526020808320909101546001600160a01b031683528201929092526040019020805461069690611f20565b80601f01602080910402602001604051908101604052809291908181526020018280546106c290611f20565b801561070f5780601f106106e45761010080835404028352916020019161070f565b820191906000526020600020905b8154815290600101906020018083116106f257829003601f168201915b505050505082828151811061072657610726611f55565b6020026020010181905250808061073c90611f05565b915050610646565b508060048080548060200260200160405190810160405280929190818152602001828054801561079d57602002820191906000526020600020905b81546001600160a01b0316815260019091019060200180831161077f575b5050505050905092509250509091565b3360009081526001602052604090208054606091906107cb90611f20565b1515905061082e5760405162461bcd60e51b815260206004820152602a60248201527f546865207573657220646f6573206e6f74206578697374206f6e2074686520626044820152693637b1b5b1b430b4b71760b11b60648201526084016105ba565b336000908152600160205260409020805461084890611f20565b80601f016020809104026020016040519081016040528092919081815260200182805461087490611f20565b80156108c15780601f10610896576101008083540402835291602001916108c1565b820191906000526020600020905b8154815290600101906020018083116108a457829003601f168201915b5050505050905090565b3360009081526001602052604090208054606091906108e990611f20565b15905061094b5760405162461bcd60e51b815260206004820152602a60248201527f546865207573657220616c726561647920657869737473206f6e2074686520626044820152693637b1b5b1b430b4b71760b11b60648201526084016105ba565b336000908152600160209081526040909120835161096b92850190611971565b50336000818152600160208190526040822081018290556004805480830182558184527f8a35acfbc15ff81a39ae7d344fd709f28e8600b4aa8c65c6b64bfe7fe36bd19b0180546001600160a01b031916909417909355915490916109cf91611f6b565b33600090815260026020908152604091829020839055815160608101909252602a808352929350909190611fbb908301399392505050565b604080516009808252610140820190925260609160009190602082016101208036833701905050905060005b600960ff82161015610aee5733600090815260016020526040902060020160ff821660098110610a6557610a65611f55565b600502015460ff16151560011415610adc5733600090815260016020526040902060020160ff821660098110610a9d57610a9d611f55565b6005020160020160009054906101000a900460ff16828260ff1681518110610ac757610ac7611f55565b602002602001019060ff16908160ff16815250505b80610ae681611f82565b915050610a33565b50919050565b604080516009808252610140820190925260609160009190602082016101208036833701905050905060005b600960ff82161015610545576001600160a01b038416600090815260016020526040902060020160ff821660098110610b5b57610b5b611f55565b600502015460ff16151560011415610bdb576001600160a01b038416600090815260016020526040902060020160ff821660098110610b9c57610b9c611f55565b6005020160020160009054906101000a900460ff16828260ff1681518110610bc657610bc6611f55565b602002602001019060ff16908160ff16815250505b80610be581611f82565b915050610b20565b600081471015610c525760405162461bcd60e51b815260206004820152602a60248201527f54686520636f6e747261637420686173206e6f7420656e6f756768204574686560448201526939103a379039b2b7321760b11b60648201526084016105ba565b604051339083156108fc029084906000818181858888f19350505050158015610c7f573d6000803e3d6000fd5b503492915050565b6040805160a0810182526000808252602082018190529181018290526060808201526080810191909152600082815260036020818152604092839020835160a081018552815460ff908116151582526001830154938201939093526002820154909216938201939093529082018054919291606084019190610d0890611f20565b80601f0160208091040260200160405190810160405280929190818152602001828054610d3490611f20565b8015610d815780601f10610d5657610100808354040283529160200191610d81565b820191906000526020600020905b815481529060010190602001808311610d6457829003601f168201915b5050509183525050600491909101546001600160a01b031660209091015292915050565b600960ff821610610df35760405162461bcd60e51b815260206004820152601860248201527724b73b30b634b21024b73b32b73a37b93c9024b73232bc1760411b60448201526064016105ba565b33600090815260016020526040902060020160ff821660098110610e1957610e19611f55565b600502015460ff16610e795760405162461bcd60e51b8152602060048201526024808201527f54686520696e76656e746f727920736c6f7420697320616c726561647920656d604482015263383a3c9760e11b60648201526084016105ba565b33600090815260016020526040812060020160ff831660098110610e9f57610e9f611f55565b60050201805460ff191691151591909117905533600090815260016020526040902060020160ff821660098110610ed857610ed8611f55565b600502016003600060016000336001600160a01b03166001600160a01b031681526020019081526020016000206002018460ff1660098110610f1c57610f1c611f55565b60050201600190810154825260208201929092526040016000208254815460ff1990811660ff928316151517835584840154938301939093556002808501549083018054919092169316929092179091556003808301805491830191610f8190611f20565b610f8c9291906119f5565b50600491820154910180546001600160a01b0319166001600160a01b0390921691909117905550565b60018115151415610fed573360009081526001602081905260408220018054849290610fe2908490611fa2565b909155506110159050565b336000908152600160208190526040822001805484929061100f908490611f6b565b90915550505b5050565b606080600060048054905067ffffffffffffffff81111561103c5761103c611b89565b60405190808252806020026020018201604052801561106f57816020015b606081526020019060019003908161105a5790505b5060045490915060009067ffffffffffffffff81111561109157611091611b89565b6040519080825280602002602001820160405280156110ba578160200160208202803683370190505b50905060005b60045481101561121b5760016000600483815481106110e1576110e1611f55565b60009182526020808320909101546001600160a01b031683528201929092526040019020805461111090611f20565b80601f016020809104026020016040519081016040528092919081815260200182805461113c90611f20565b80156111895780601f1061115e57610100808354040283529160200191611189565b820191906000526020600020905b81548152906001019060200180831161116c57829003601f168201915b50505050508382815181106111a0576111a0611f55565b602002602001018190525060016000600483815481106111c2576111c2611f55565b60009182526020808320909101546001600160a01b0316835282019290925260400190206001015482518390839081106111fe576111fe611f55565b60209081029190910101528061121381611f05565b9150506110c0565b5090939092509050565b600960ff8316106112735760405162461bcd60e51b815260206004820152601860248201527724b73b30b634b21024b73b32b73a37b93c9024b73232bc1760411b60448201526064016105ba565b33600090815260016020526040902060020160ff83166009811061129957611299611f55565b600502015460ff16156112ee5760405162461bcd60e51b815260206004820181905260248201527f54686520696e76656e746f727920736c6f74206973206e6f7420656d7074792e60448201526064016105ba565b6000805490806112fd83611f05565b909155505033600090815260016020819052604090912060020160ff84166009811061132b5761132b611f55565b60050201805460ff1916911515919091179055336000908152600160205260409020819060020160ff84166009811061136657611366611f55565b600502016002908101805460ff191660ff938416179055600080543382526001602052604090912090929101908416600981106113a5576113a5611f55565b60050201600190810191909155336000908152602091909152604090206002810160ff8416600981106113da576113da611f55565b600502016003019080546113ed90611f20565b6113f89291906119f5565b5033600081815260016020526040902060020160ff84166009811061141f5761141f611f55565b6005020160040180546001600160a01b0319166001600160a01b039290921691909117905533600090815260016020526040902060020160ff83166009811061146a5761146a611f55565b600502016003600060016000336001600160a01b03166001600160a01b031681526020019081526020016000206002018560ff16600981106114ae576114ae611f55565b60050201600190810154825260208201929092526040016000208254815460ff1990811660ff92831615151783558484015493830193909355600280850154908301805491909216931692909217909155600380830180549183019161151390611f20565b61151e9291906119f5565b50600491820154910180546001600160a01b0319166001600160a01b039092169190911790555050565b82846001600160a01b03163110156115b65760405162461bcd60e51b815260206004820152602b60248201527f546865206f7468657220706c6179657220646f65736e2774206861766520656e60448201526a37bab3b41022ba3432b91760a91b60648201526084016105ba565b60ff81161561196b576000805b600960ff82161015611902576001600160a01b038616600090815260016020526040902060020160ff8216600981106115fe576115fe611f55565b600502015460ff166118f05733600090815260016020526040812060020160ff86166009811061163057611630611f55565b60050201805460ff19169115159190911790556001600160a01b038616600090815260016020819052604090912060020160ff83166009811061167557611675611f55565b60050201805460ff19169115159190911790556001600160a01b0386166000908152600160205260409020839060020160ff8316600981106116b9576116b9611f55565b600502016002908101805460ff191660ff93841617905533600090815260016020526040902001908516600981106116f3576116f3611f55565b600502016001015460016000886001600160a01b03166001600160a01b031681526020019081526020016000206002018260ff166009811061173757611737611f55565b600502016001908101919091556001600160a01b0387166000908152602091909152604090206002810160ff83166009811061177557611775611f55565b6005020160030190805461178890611f20565b6117939291906119f5565b506001600160a01b0386166000908152600160205260409020869060020160ff8316600981106117c5576117c5611f55565b6005020160040180546001600160a01b0319166001600160a01b039283161790558616600090815260016020526040902060020160ff82166009811061180d5761180d611f55565b6005020160036000600160008a6001600160a01b03166001600160a01b031681526020019081526020016000206002018460ff166009811061185157611851611f55565b60050201600190810154825260208201929092526040016000208254815460ff1990811660ff9283161515178355848401549383019390935560028085015490830180549190921693169290921790915560038083018054918301916118b690611f20565b6118c19291906119f5565b50600491820154910180546001600160a01b0319166001600160a01b0390921691909117905560019150611902565b806118fa81611f82565b9150506115c3565b506001811515146119695760405162461bcd60e51b815260206004820152602b60248201527f546865206f7468657220706c6179657220646f65736e2774206861766520656e60448201526a37bab3b41039b830b1b29760a91b60648201526084016105ba565b505b50505050565b82805461197d90611f20565b90600052602060002090601f01602090048101928261199f57600085556119e5565b82601f106119b857805160ff19168380011785556119e5565b828001600101855582156119e5579182015b828111156119e55782518255916020019190600101906119ca565b506119f1929150611a70565b5090565b828054611a0190611f20565b90600052602060002090601f016020900481019282611a2357600085556119e5565b82601f10611a3457805485556119e5565b828001600101855582156119e557600052602060002091601f016020900482015b828111156119e5578254825591600101919060010190611a55565b5b808211156119f15760008155600101611a71565b6000815180845260005b81811015611aab57602081850181015186830182015201611a8f565b81811115611abd576000602083870101525b50601f01601f19169290920160200192915050565b8051151582526020810151602083015260ff60408201511660408301526000606082015160a06060850152611b0a60a0850182611a85565b6080938401516001600160a01b0316949093019390935250919050565b6000602080830181845280855180835260408601915060408160051b870101925083870160005b82811015611b7c57603f19888603018452611b6a858351611ad2565b94509285019290850190600101611b4e565b5092979650505050505050565b634e487b7160e01b600052604160045260246000fd5b600060208284031215611bb157600080fd5b813567ffffffffffffffff80821115611bc957600080fd5b818401915084601f830112611bdd57600080fd5b813581811115611bef57611bef611b89565b604051601f8201601f19908116603f01168101908382118183101715611c1757611c17611b89565b81604052828152876020848701011115611c3057600080fd5b826020860160208301376000928101602001929092525095945050505050565b602081526000611c636020830184611a85565b9392505050565b600082825180855260208086019550808260051b84010181860160005b84811015611cb557601f19868403018952611ca3838351611a85565b98840198925090830190600101611c87565b5090979650505050505050565b604081526000611cd56040830185611c6a565b82810360208481019190915284518083528582019282019060005b81811015611cb55784516001600160a01b031683529383019391830191600101611cf0565b6020808252825182820181905260009190848201906040850190845b81811015611d5057835160ff1683529284019291840191600101611d31565b50909695505050505050565b6001600160a01b0381168114611d7157600080fd5b50565b600060208284031215611d8657600080fd5b8135611c6381611d5c565b600060208284031215611da357600080fd5b5035919050565b602081526000611c636020830184611ad2565b803560ff81168114611dce57600080fd5b919050565b600060208284031215611de557600080fd5b611c6382611dbd565b60008060408385031215611e0157600080fd5b8235915060208301358015158114611e1857600080fd5b809150509250929050565b604081526000611e366040830185611c6a565b82810360208481019190915284518083528582019282019060005b81811015611cb557845183529383019391830191600101611e51565b60008060408385031215611e8057600080fd5b611e8983611dbd565b9150611e9760208401611dbd565b90509250929050565b60008060008060808587031215611eb657600080fd5b8435611ec181611d5c565b935060208501359250611ed660408601611dbd565b9150611ee460608601611dbd565b905092959194509250565b634e487b7160e01b600052601160045260246000fd5b6000600019821415611f1957611f19611eef565b5060010190565b600181811c90821680611f3457607f821691505b60208210811415610aee57634e487b7160e01b600052602260045260246000fd5b634e487b7160e01b600052603260045260246000fd5b600082821015611f7d57611f7d611eef565b500390565b600060ff821660ff811415611f9957611f99611eef565b60010192915050565b60008219821115611fb557611fb5611eef565b50019056fe41206e657720706c617965722069732063726561746564206f6e2074686520626c6f636b636861696e2ea264697066735822122004c551363fe614c6079b30acb16e927fd18863a4089e73ebac1749bb53021fa464736f6c63430008090033";
        public HukocraftDeploymentBase() : base(BYTECODE) { }
        public HukocraftDeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class ChangeUsernameFunction : ChangeUsernameFunctionBase { }

    [Function("ChangeUsername", "string")]
    public class ChangeUsernameFunctionBase : FunctionMessage
    {
        [Parameter("string", "_newUsername", 1)]
        public virtual string NewUsername { get; set; }
    }

    public partial class CharacterInitializationFunction : CharacterInitializationFunctionBase { }

    [Function("CharacterInitialization", "string")]
    public class CharacterInitializationFunctionBase : FunctionMessage
    {
        [Parameter("string", "_username", 1)]
        public virtual string Username { get; set; }
    }

    public partial class ContractEtherTransferFunction : ContractEtherTransferFunctionBase { }

    [Function("ContractEtherTransfer", "uint256")]
    public class ContractEtherTransferFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "_etherAmountInWei", 1)]
        public virtual BigInteger EtherAmountInWei { get; set; }
    }

    public partial class FindItemFunction : FindItemFunctionBase { }

    [Function("FindItem", typeof(FindItemOutputDTO))]
    public class FindItemFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "_itemID", 1)]
        public virtual BigInteger ItemID { get; set; }
    }

    public partial class GetAllPlayersFunction : GetAllPlayersFunctionBase { }

    [Function("GetAllPlayers", typeof(GetAllPlayersOutputDTO))]
    public class GetAllPlayersFunctionBase : FunctionMessage
    {

    }

    public partial class InventoryAddItemFunction : InventoryAddItemFunctionBase { }

    [Function("InventoryAddItem")]
    public class InventoryAddItemFunctionBase : FunctionMessage
    {
        [Parameter("uint8", "_itemIndex", 1)]
        public virtual byte ItemIndex { get; set; }
        [Parameter("uint8", "_itemCode", 2)]
        public virtual byte ItemCode { get; set; }
    }

    public partial class InventoryRemoveItemFunction : InventoryRemoveItemFunctionBase { }

    [Function("InventoryRemoveItem")]
    public class InventoryRemoveItemFunctionBase : FunctionMessage
    {
        [Parameter("uint8", "_itemIndex", 1)]
        public virtual byte ItemIndex { get; set; }
    }

    public partial class ItemExchangeFunction : ItemExchangeFunctionBase { }

    [Function("ItemExchange")]
    public class ItemExchangeFunctionBase : FunctionMessage
    {
        [Parameter("address", "_sendToWallet", 1)]
        public virtual string SendToWallet { get; set; }
        [Parameter("uint256", "_receiveEtherAmount", 2)]
        public virtual BigInteger ReceiveEtherAmount { get; set; }
        [Parameter("uint8", "_myItemIndex", 3)]
        public virtual byte MyItemIndex { get; set; }
        [Parameter("uint8", "_itemCode", 4)]
        public virtual byte ItemCode { get; set; }
    }

    public partial class ObserveInventoryFunction : ObserveInventoryFunctionBase { }

    [Function("ObserveInventory", "uint8[]")]
    public class ObserveInventoryFunctionBase : FunctionMessage
    {
        [Parameter("address", "_userAddress", 1)]
        public virtual string UserAddress { get; set; }
    }

    public partial class ObserveItemsFunction : ObserveItemsFunctionBase { }

    [Function("ObserveItems", typeof(ObserveItemsOutputDTO))]
    public class ObserveItemsFunctionBase : FunctionMessage
    {

    }

    public partial class PlayerEtherTransferFunction : PlayerEtherTransferFunctionBase { }

    [Function("PlayerEtherTransfer", "uint256")]
    public class PlayerEtherTransferFunctionBase : FunctionMessage
    {

    }

    public partial class SetCharacterExperienceFunction : SetCharacterExperienceFunctionBase { }

    [Function("SetCharacterExperience")]
    public class SetCharacterExperienceFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "_expAmount", 1)]
        public virtual BigInteger ExpAmount { get; set; }
        [Parameter("bool", "isAddition", 2)]
        public virtual bool IsAddition { get; set; }
    }

    public partial class ShowCharacterExperienceFunction : ShowCharacterExperienceFunctionBase { }

    [Function("ShowCharacterExperience", "uint256")]
    public class ShowCharacterExperienceFunctionBase : FunctionMessage
    {

    }

    public partial class ShowContractBalanceFunction : ShowContractBalanceFunctionBase { }

    [Function("ShowContractBalance", "uint256")]
    public class ShowContractBalanceFunctionBase : FunctionMessage
    {

    }

    public partial class ShowInventoryFunction : ShowInventoryFunctionBase { }

    [Function("ShowInventory", "uint8[]")]
    public class ShowInventoryFunctionBase : FunctionMessage
    {

    }

    public partial class ShowPlayerBalanceFunction : ShowPlayerBalanceFunctionBase { }

    [Function("ShowPlayerBalance", "uint256")]
    public class ShowPlayerBalanceFunctionBase : FunctionMessage
    {

    }

    public partial class ShowRankingFunction : ShowRankingFunctionBase { }

    [Function("ShowRanking", typeof(ShowRankingOutputDTO))]
    public class ShowRankingFunctionBase : FunctionMessage
    {

    }

    public partial class ShowUsernameFunction : ShowUsernameFunctionBase { }

    [Function("ShowUsername", "string")]
    public class ShowUsernameFunctionBase : FunctionMessage
    {

    }

    public partial class FindItemOutputDTO : FindItemOutputDTOBase { }

    [FunctionOutput]
    public class FindItemOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("tuple", "", 1)]
        public virtual Item ReturnValue1 { get; set; }
    }

    public partial class GetAllPlayersOutputDTO : GetAllPlayersOutputDTOBase { }

    [FunctionOutput]
    public class GetAllPlayersOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("string[]", "", 1)]
        public virtual List<string> ReturnValue1 { get; set; }
        [Parameter("address[]", "", 2)]
        public virtual List<string> ReturnValue2 { get; set; }
    }

    public partial class ObserveInventoryOutputDTO : ObserveInventoryOutputDTOBase { }

    [FunctionOutput]
    public class ObserveInventoryOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint8[]", "", 1)]
        public virtual List<byte> ReturnValue1 { get; set; }
    }

    public partial class ObserveItemsOutputDTO : ObserveItemsOutputDTOBase { }

    [FunctionOutput]
    public class ObserveItemsOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("tuple[]", "", 1)]
        public virtual List<Item> ReturnValue1 { get; set; }
    }

    public partial class ShowCharacterExperienceOutputDTO : ShowCharacterExperienceOutputDTOBase { }

    [FunctionOutput]
    public class ShowCharacterExperienceOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class ShowContractBalanceOutputDTO : ShowContractBalanceOutputDTOBase { }

    [FunctionOutput]
    public class ShowContractBalanceOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class ShowInventoryOutputDTO : ShowInventoryOutputDTOBase { }

    [FunctionOutput]
    public class ShowInventoryOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint8[]", "", 1)]
        public virtual List<byte> ReturnValue1 { get; set; }
    }

    public partial class ShowPlayerBalanceOutputDTO : ShowPlayerBalanceOutputDTOBase { }

    [FunctionOutput]
    public class ShowPlayerBalanceOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class ShowRankingOutputDTO : ShowRankingOutputDTOBase { }

    [FunctionOutput]
    public class ShowRankingOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("string[]", "", 1)]
        public virtual List<string> ReturnValue1 { get; set; }
        [Parameter("uint256[]", "", 2)]
        public virtual List<BigInteger> ReturnValue2 { get; set; }
    }

    public partial class ShowUsernameOutputDTO : ShowUsernameOutputDTOBase { }

    [FunctionOutput]
    public class ShowUsernameOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("string", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }

    //---------------------------------------------------------------------------------------------


    //---------------------------------------------------------------------------------------------


    //---------------------------------------------------------------------------------------------

    public static HukocraftSmartContract instance;
    public void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
        {
            Debug.Log("[Error]: Hukocraft Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    //IF YOU WANNA DEPLOY A NEW CONTRACT, YOU SHOULD FILL THESE VARIABLES BEFORE RUNING THE PROGRAM
    private string ropstenProjectWebAddress = "https://ropsten.infura.io/v3/63aaa845a76948d09e01595fe2092c85";
    private string myAddress = "0xDB57Df571a2f840A679d85cfCeDA176D2c2A916f";
    private string myPrivateKey = "a1390a14751b6eaea8d0ae098537c418fa123e3b58f6178086f9df4361595c28";
    private readonly string contractAddress = "0x96D91B0f497fe8d7604a35b30e9302311c302402"; //THIS IS OUR MAIN CONTRACT
    private Nethereum.Web3.Accounts.Account account;
    private Web3 web3;

    public void Start()
    {
        //IF YOU WANNA RE-CREATE THE CONTRACT, THEN ACTIVATE THIS (AND CHANGE contractAddress MANUALLY)
        // _ = HukocraftCreateAndDeployContract();
    }

    public void Initialize(string _setRopstenProjectWebAddress, string _myAddress, string _setMyPrivateKey)
    {
        ropstenProjectWebAddress = _setRopstenProjectWebAddress;
        myAddress = _myAddress;
        myPrivateKey = _setMyPrivateKey;

        account = new Nethereum.Web3.Accounts.Account(myPrivateKey, 3); //3 STANDS FOR ROPSTEN CHAIN ID [IT MUST BE 3 ALWAYS]
        web3 = new Web3(account, ropstenProjectWebAddress);
    }

    public async Task HukocraftCreateAndDeployContract()
    {
        account = new Nethereum.Web3.Accounts.Account(myPrivateKey, 3); //3 STANDS FOR ROPSTEN CHAIN ID [IT MUST BE 3 ALWAYS]
        web3 = new Web3(account, ropstenProjectWebAddress);
        var deploymentHandler = web3.Eth.GetContractDeploymentHandler<HukocraftDeployment>();
        var deploymentMessage = new HukocraftDeployment
        {

        };

        //TESTING
        //functionParameters.Gas = 1000000; (GAS LIMIT)
        //functionParameters.GasPrice = Nethereum.Web3.Web3.Convert.ToWei(5, Nethereum.Util.UnitConversion.EthUnit.Gwei);

        Debug.Log("[Create and Deploy Smart Contract]: Transaction in process...");
        var transactionReceipt = await deploymentHandler.SendRequestAndWaitForReceiptAsync(deploymentMessage);

        Debug.Log("[Create and Deploy Smart Contract]: Mined Successfully -> " + transactionReceipt.ContractAddress);
        System.Diagnostics.Process.Start("https://ropsten.etherscan.io/address/" + transactionReceipt.ContractAddress);
    }

    public async Task HukocraftCharacterInitialization(string _username)
    {
        var initializationHandler = web3.Eth.GetContractTransactionHandler<CharacterInitializationFunction>();
        var functionParameters = new CharacterInitializationFunction()
        {
            FromAddress = myAddress,
            Username = _username,
        };

        //TESTING
        //functionParameters.Gas = 1000000; (GAS LIMIT)
        //functionParameters.GasPrice = Nethereum.Web3.Web3.Convert.ToWei(5, Nethereum.Util.UnitConversion.EthUnit.Gwei);

        try
        {
            GameChat.instance.SendEthereumMessage("[Character Initialization]: Transaction in process...", true);
            var transaction = await initializationHandler.SendRequestAndWaitForReceiptAsync(contractAddress, functionParameters);

            GameChat.instance.SendEthereumMessage("[Character Initialization]: Mined Successfully -> " + _username, true);
            _ = HukocraftShowPlayerBalance();
        }
        catch (Exception ex) { GameChat.instance.SendEthereumMessage("[CharacterInitialization]: " + ex.Message, false); }
    }

    //THIS FUNCTION DONATES ETHER TO THE SMART CONTRACT, NOT USED BETWEEN PLAYERS
    public async Task HukocraftPlayerEtherTransfer(decimal _etherAmount)
    {
        var transferHandler = web3.Eth.GetContractTransactionHandler<PlayerEtherTransferFunction>();
        var functionParameters = new PlayerEtherTransferFunction()
        {
            FromAddress = myAddress,
            AmountToSend = Nethereum.Util.UnitConversion.Convert.ToWei(_etherAmount)
        };

        //TESTING
        //functionParameters.Gas = 1000000; (GAS LIMIT)
        //functionParameters.GasPrice = Nethereum.Web3.Web3.Convert.ToWei(5, Nethereum.Util.UnitConversion.EthUnit.Gwei);

        try
        {
            GameChat.instance.SendEthereumMessage("[Ether Transfer]: Transaction in process...", true);
            var transaction = await transferHandler.SendRequestAndWaitForReceiptAsync(contractAddress, functionParameters);

            GameChat.instance.SendEthereumMessage("[Ether Transfer]: Mined Successfully -> " + _etherAmount + " Ether", true);
            _ = ETHAccount.instance.ShowAccountBalance();
        }
        catch (Exception ex) { GameChat.instance.SendEthereumMessage("[EtherTransfer]: " + ex.Message, false); }
    }

    public async Task<bool> HukocraftInventoryAddItem(int _itemCode, int _itemIndex)
    {
        var itemAddHandler = web3.Eth.GetContractTransactionHandler<InventoryAddItemFunction>();
        var functionParameters = new InventoryAddItemFunction()
        {
            FromAddress = myAddress,
            ItemCode = (byte)_itemCode,
            ItemIndex = (byte)_itemIndex
        };

        //TESTING
        //functionParameters.Gas = 1000000; (GAS LIMIT)
        //functionParameters.GasPrice = Nethereum.Web3.Web3.Convert.ToWei(5, Nethereum.Util.UnitConversion.EthUnit.Gwei);

        try
        {
            GameChat.instance.SendEthereumMessage("[Add Item]: Transaction in process... -> Slot " + (_itemIndex + 1), true);
            var transaction = await itemAddHandler.SendRequestAndWaitForReceiptAsync(contractAddress, functionParameters);

            GameChat.instance.SendEthereumMessage("[Add Item]: Mined Successfully -> Slot " + (_itemIndex + 1), true);
            return true;
        }
        catch (Exception ex) { GameChat.instance.SendEthereumMessage("[AddItem]: Slot " + (_itemIndex + 1) + " : " + ex.Message, false); return false; }

        

    }

    public async Task<bool> HukocraftInventoryRemoveItem(int _itemIndex)
    {
        var itemRemoveHandler = web3.Eth.GetContractTransactionHandler<InventoryRemoveItemFunction>();
        var functionParameters = new InventoryRemoveItemFunction()
        {
            FromAddress = myAddress,
            ItemIndex = (byte)_itemIndex
        };

        //TESTING
        //functionParameters.Gas = 1000000; (GAS LIMIT)
        //functionParameters.GasPrice = Nethereum.Web3.Web3.Convert.ToWei(5, Nethereum.Util.UnitConversion.EthUnit.Gwei);

        try
        {
            GameChat.instance.SendEthereumMessage("[Remove Item]: Transaction in process... -> Slot " + (_itemIndex + 1), true);
            var transaction = await itemRemoveHandler.SendRequestAndWaitForReceiptAsync(contractAddress, functionParameters);

            GameChat.instance.SendEthereumMessage("[Remove Item]: Mined Successfully -> Slot " + (_itemIndex + 1), true);
            return true;
        }
        catch (Exception ex) { GameChat.instance.SendEthereumMessage("[RemoveItem]: Slot " + (_itemIndex + 1) + " : " + ex.Message, false); return false; }
    }

    public async Task HukocraftChangeUsername(string _newUsername)
    {
        var usernameHandler = web3.Eth.GetContractTransactionHandler<ChangeUsernameFunction>();
        var functionParameters = new ChangeUsernameFunction()
        {
            FromAddress = myAddress,
            NewUsername = _newUsername,
            AmountToSend = Nethereum.Util.UnitConversion.Convert.ToWei(1)
        };

        //TESTING
        //functionParameters.Gas = 1000000; (GAS LIMIT)
        //functionParameters.GasPrice = Nethereum.Web3.Web3.Convert.ToWei(5, Nethereum.Util.UnitConversion.EthUnit.Gwei);

        try
        {
            GameChat.instance.SendEthereumMessage("[Change Username]: Transaction in process...", true);
            var transaction = await usernameHandler.SendRequestAndWaitForReceiptAsync(contractAddress, functionParameters);
            GameChat.instance.SendEthereumMessage("[Change Username]: Mined Successfully -> " + _newUsername, true);
        }
        catch (Exception ex) { GameChat.instance.SendEthereumMessage("[ChangeUsername]: " + ex.Message, false); }
    }
    public async Task<bool> HukocraftSetCharacterExperience(BigInteger _expAmount, bool _isAddition)
    {
        var experienceHandler = web3.Eth.GetContractTransactionHandler<SetCharacterExperienceFunction>();
        var functionParameters = new SetCharacterExperienceFunction()
        {
            FromAddress = myAddress,
            ExpAmount = _expAmount,
            IsAddition = _isAddition
        };

        //TESTING
        //functionParameters.Gas = 1000000; (GAS LIMIT)
        //functionParameters.GasPrice = Nethereum.Web3.Web3.Convert.ToWei(5, Nethereum.Util.UnitConversion.EthUnit.Gwei);

        try
        {
            var transaction = await experienceHandler.SendRequestAndWaitForReceiptAsync(contractAddress, functionParameters);

            if (_isAddition)
                GameChat.instance.SendEthereumMessage("[Experience]: Mined Successfully -> +" + _expAmount + " Exp", true);
            else
                GameChat.instance.SendEthereumMessage("[Experience]: Mined Successfully -> -" + _expAmount + " Exp", true);
            return true;
        }
        catch (Exception ex) { GameChat.instance.SendEthereumMessage("[Experience]: " + ex.Message, false); return false; }
    }

    public async Task<bool> HukocraftItemExchange(String _sendToWallet, decimal _receiveEtherAmount = 0, int _myItemIndex = 0, int _itemCodeToBeSent = 0)
    {
        var itemExchangeHandler = web3.Eth.GetContractTransactionHandler<ItemExchangeFunction>();
        var functionParameters = new ItemExchangeFunction()
        {
            FromAddress = myAddress,
            SendToWallet = _sendToWallet,
            ReceiveEtherAmount = Nethereum.Util.UnitConversion.Convert.ToWei(_receiveEtherAmount),
            MyItemIndex = (byte)_myItemIndex,
            ItemCode = (byte)_itemCodeToBeSent,
        };

        //TESTING
        //functionParameters.Gas = 1000000; (GAS LIMIT)
        //functionParameters.GasPrice = Nethereum.Web3.Web3.Convert.ToWei(5, Nethereum.Util.UnitConversion.EthUnit.Gwei);

        try
        {
            GameChat.instance.SendEthereumMessage("[ItemExchange]: Transaction in process...", true);
            var transaction = await itemExchangeHandler.SendRequestAndWaitForReceiptAsync(contractAddress, functionParameters);

            GameChat.instance.SendEthereumMessage("[ItemExchange]: Mined Successfully -> Slot " + (_myItemIndex + 1), true);

            if (_itemCodeToBeSent == 1)
                GameChat.instance.SendChatMessage("Trade", "You have transferred x1 [Bomb] in the Slot " + (_myItemIndex + 1) + " successfully.");
            else if (_itemCodeToBeSent == 2)
                GameChat.instance.SendChatMessage("Trade", "You have transferred x1 [Shield] in the Slot " + (_myItemIndex + 1) + " successfully.");

            return true;
        }
        catch (Exception ex)
        {
            GameChat.instance.SendEthereumMessage("[ItemExchange]: Slot " + (_myItemIndex + 1) + " : " + ex.Message, false);
            GameChat.instance.SendChatMessage("Trade", ex.Message, true);
            return false;
        }
    }
    //------------------------------------------------------------------------------------------------------------------
    //******************************************** READ ONLY FUNCTIONS *************************************************
    public async Task HukocraftShowPlayerBalance()
    {
        var balanceHandler = web3.Eth.GetContractQueryHandler<ShowPlayerBalanceFunction>();
        var functionParameters = new ShowPlayerBalanceFunction()
        {
            FromAddress = myAddress
        };

        try
        {
            var transaction = await balanceHandler.QueryAsync<BigInteger>(contractAddress, functionParameters);
            GameChat.instance.SendEthereumMessage("[Player Balance]: " + Nethereum.Util.UnitConversion.Convert.FromWei(transaction), true);
        }
        catch (Exception ex) { GameChat.instance.SendEthereumMessage("[PlayerBalance]: " + ex.Message, false); }
    }

    public async Task HukocraftShowContractBalance()
    {
        var balanceHandler = web3.Eth.GetContractQueryHandler<ShowContractBalanceFunction>();
        var functionParameters = new ShowContractBalanceFunction()
        {
            FromAddress = myAddress
        };

        try
        {
            var transaction = await balanceHandler.QueryAsync<BigInteger>(contractAddress, functionParameters);
            GameChat.instance.SendEthereumMessage("[Contract Balance]: " + Nethereum.Util.UnitConversion.Convert.FromWei(transaction), true);
        }
        catch (Exception ex) { GameChat.instance.SendEthereumMessage("[ContractBalance]: " + ex.Message, false); }
    }

    public async Task<int[]> HukocraftShowInventory()
    {
        var inventoryHandler = web3.Eth.GetContractQueryHandler<ShowInventoryFunction>();
        var functionParameters = new ShowInventoryFunction()
        {
            FromAddress = myAddress
        };

        try
        {
            //YOU SHOULD USE QueryDeserializingToObjectAsync<....DTO> FOR ARRAYS AND STRUCTS
            var transaction = await inventoryHandler.QueryDeserializingToObjectAsync<ShowInventoryOutputDTO>(functionParameters, contractAddress);
            int[] arr = transaction.ReturnValue1.Select(c => (int)c).ToArray(); //CONVERT BYTE[] TO INT[]
            return arr;
        }
        catch (Exception ex) { GameChat.instance.SendEthereumMessage("[ShowInventory]: " + ex.Message, false); return null; }
    }

    public async Task<string> HukocraftShowUsername()
    {
        var usernameHandler = web3.Eth.GetContractQueryHandler<ShowUsernameFunction>();
        var functionParameters = new ShowUsernameFunction()
        {
            FromAddress = myAddress
        };

        try
        {
            var transaction = await usernameHandler.QueryAsync<string>(contractAddress, functionParameters);
            GameChat.instance.SendEthereumMessage("[Show Username]: " + transaction, true);
            return transaction;
        }
        catch (Exception ex) { GameChat.instance.SendEthereumMessage("[ShowUsername]: " + ex.Message, false); return ""; }
    }
    public async Task<BigInteger> HukocraftShowCharacterExperience()
    {
        var experienceHandler = web3.Eth.GetContractQueryHandler<ShowCharacterExperienceFunction>();
        var functionParameters = new ShowCharacterExperienceFunction()
        {
            FromAddress = myAddress
        };

        try
        {
            var transaction = await experienceHandler.QueryAsync<BigInteger>(contractAddress, functionParameters);
            return transaction;
        }
        catch (Exception ex) { GameChat.instance.SendEthereumMessage("[ShowExperience]: " + ex.Message, false); return -1; }
    }

    public async Task<Dictionary<string, BigInteger>> HukocraftShowRanking()
    {
        var rankingHandler = web3.Eth.GetContractQueryHandler<ShowRankingFunction>();
        var functionParameters = new ShowRankingFunction()
        {
            FromAddress = myAddress
        };

        try
        {
            
            //YOU SHOULD USE QueryDeserializingToObjectAsync<....DTO> FOR ARRAYS AND STRUCTS
            var transaction = await rankingHandler.QueryDeserializingToObjectAsync<ShowRankingOutputDTO>(functionParameters, contractAddress);

            Dictionary<string, BigInteger> ranking = new Dictionary<string, BigInteger>();
            for (int i = 0; i < transaction.ReturnValue1.Count; i++)
                ranking.Add(transaction.ReturnValue1[i], transaction.ReturnValue2[i]);

            return ranking.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }
        catch (Exception ex) { GameChat.instance.SendEthereumMessage("[ShowRanking]: " + ex.Message, false); return null; }
    }

    public async Task<List<Item>> HukocraftObserveItems()
    {
        var observeItemsHandler = web3.Eth.GetContractQueryHandler<ObserveItemsFunction>();
        var functionParameters = new ObserveItemsFunction()
        {
            FromAddress = myAddress
        };

        try
        {

            //YOU SHOULD USE QueryDeserializingToObjectAsync<....DTO> FOR ARRAYS AND STRUCTS
            var transaction = await observeItemsHandler.QueryDeserializingToObjectAsync<ObserveItemsOutputDTO>(functionParameters, contractAddress);


            List<Item> itemList = new List<Item>();
            for (int i = 0; i < transaction.ReturnValue1.Count; i++)
                itemList.Add(transaction.ReturnValue1[i]);

            return itemList;
        }
        catch (Exception ex) { GameChat.instance.SendEthereumMessage("[ObserveItems]: " + ex.Message, false); return null; }
    }

    public async Task<Item> HukocraftFindItem(BigInteger _itemID)
    {
        var findItemHandler = web3.Eth.GetContractQueryHandler<FindItemFunction>();
        var functionParameters = new FindItemFunction()
        {
            FromAddress = myAddress,
            ItemID = _itemID
        };

        try
        {
            //YOU SHOULD USE QueryDeserializingToObjectAsync<....DTO> FOR ARRAYS AND STRUCTS
            var transaction = await findItemHandler.QueryDeserializingToObjectAsync<FindItemOutputDTO>(functionParameters, contractAddress);

            return transaction.ReturnValue1;
        }
        catch (Exception ex) { GameChat.instance.SendEthereumMessage("[FindItem]: " + ex.Message, false); return null; }
    }

    public async Task<int[]> HukocraftObserveInventory(string _observeAddress) 
    {
        var observeInventoryHandler = web3.Eth.GetContractQueryHandler<ObserveInventoryFunction>();
        var functionParameters = new ObserveInventoryFunction()
        {
            FromAddress = myAddress,
            UserAddress = _observeAddress
        };

        try
        {
            //YOU SHOULD USE QueryDeserializingToObjectAsync<....DTO> FOR ARRAYS AND STRUCTS
            var transaction = await observeInventoryHandler.QueryDeserializingToObjectAsync<ObserveInventoryOutputDTO>(functionParameters, contractAddress);
            int[] arr = transaction.ReturnValue1.Select(c => (int)c).ToArray(); //CONVERT BYTE[] TO INT[]
            return arr;
        }
        catch (Exception ex) { GameChat.instance.SendEthereumMessage("[ObserveInventory]: " + ex.Message, false); return null; }
    }

    public async Task<Dictionary<string, string>> HukocraftGetAllPlayerAddressesAndNames()
    {
        var playerAddressNameHandler = web3.Eth.GetContractQueryHandler<GetAllPlayersFunction>();
        var functionParameters = new GetAllPlayersFunction()
        {
            FromAddress = myAddress
        };

        try
        {
            //YOU SHOULD USE QueryDeserializingToObjectAsync<....DTO> FOR ARRAYS AND STRUCTS
            var transaction = await playerAddressNameHandler.QueryDeserializingToObjectAsync<GetAllPlayersOutputDTO>(functionParameters, contractAddress);
            Dictionary<string, string> playerAddressesAndNames = new Dictionary<string, string>();
            for(int i = 0; i < transaction.ReturnValue1.Count; i++)
                playerAddressesAndNames.Add(transaction.ReturnValue2[i], transaction.ReturnValue1[i] + " (" + transaction.ReturnValue2[i] + ")");

            return playerAddressesAndNames;
        }
        catch (Exception ex) { GameChat.instance.SendEthereumMessage("[GetAllPlayers]: " + ex.Message, false); return null; }
    }
}