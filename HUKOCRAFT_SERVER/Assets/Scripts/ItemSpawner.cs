using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    //TRACK ALL THE ITEMSPAWNERS BY USING THEIR ID
    public static Dictionary<int, ItemSpawner> itemSpawnerObjects = new Dictionary<int, ItemSpawner>();
    public int itemSpawnerObjectID;
    private static int nextItemObjectID = 1;
    public string itemType = "Heal";
    private int randomValue;

    public bool itemSpawnerHasItem = false; //IF CURRENT ITEMSPAWNER HAS AN ITEM OR NOT
    private readonly float itemRespawnTime = 10.0f;

    public void Start()
    {
        itemSpawnerHasItem = false;
        itemSpawnerObjectID = nextItemObjectID;
        nextItemObjectID++;
        itemSpawnerObjects.Add(itemSpawnerObjectID, this);

        StartCoroutine(ChangeItemType(0));
        StartCoroutine(SpawnItem(0));
    }

    private IEnumerator ChangeItemType(float _itemRespawnTime)
    {
        yield return new WaitForSeconds(_itemRespawnTime);

        randomValue = Random.Range(0, 4);
        if (randomValue == 0)
            itemType = "BasicBomb";
        else if (randomValue == 1 || randomValue == 2)
            itemType = "Heal";
        else if (randomValue == 3)
            itemType = "Shield";

        if (itemSpawnerHasItem == true)
            ServerSend.ItemSpawned(itemSpawnerObjectID, itemType);

        StartCoroutine(ChangeItemType(itemRespawnTime));
    }

    //ITEM WILL BE SPAWN ON ITEMSPAWNER OBJECT FIELD
    private IEnumerator SpawnItem(float _itemRespawnTime)
    {
        yield return new WaitForSeconds(_itemRespawnTime); //ITEMS WILL BE SPAWNED EVERY X SECONDS

        itemSpawnerHasItem = true;
        ServerSend.ItemSpawned(itemSpawnerObjectID, itemType);
    }

    //IF PLAYER HITS THE SPAWNED ITEM, IT WILL TRIGGER THIS FUNC
    public void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") == true && itemSpawnerHasItem == true)
        {
            if (itemType.Equals("BasicBomb"))
            {
                if (collider.GetComponent<Player>().AttemptPickupItem() == true)
                    ItemPickedUp(collider.GetComponent<Player>().playerID);
            }
            else if (itemType.Equals("Heal"))
            {
                float recoveryAmount = collider.GetComponent<Player>().playerMaxHPMPLimit / 4.0f; //25% HP MP RECOVERY
                collider.GetComponent<Player>().Recovery(recoveryAmount, recoveryAmount);
                ItemPickedUp(collider.GetComponent<Player>().playerID);
            }
            else if (itemType.Equals("Shield"))
            {
                if (collider.GetComponent<Player>().AttemptPickupItem() == true)
                    ItemPickedUp(collider.GetComponent<Player>().playerID);
            }
        }
    }

    //ITEM WILL BE PICKED UP BY A PLAYER
    private void ItemPickedUp(int _playerIDWhoTookTheItem)
    {
        itemSpawnerHasItem = false;
        ServerSend.ItemPickedUp(_playerIDWhoTookTheItem, itemSpawnerObjectID);
        StartCoroutine(SpawnItem(itemRespawnTime));
    }
}
