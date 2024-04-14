using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public int itemSpawnerObjectId; //ITEM SPAWNER FIELD ID
    public bool itemSpawnerHasItem; //REPRESENTS IF THE ITEM SPAWNER HAS AN ITEM ON IT OR NOT

    public GameObject itemSpawner_BasicBombModel; //ITEM SPAWNER FIELD RENDERER
    public GameObject itemSpawner_HealModel; //ITEM SPAWNER FIELD RENDERER
    public GameObject itemSpawner_ShieldModel; //ITEM SPAWNER FIELD RENDERER

    public float itemRotationSpeed = 300.0f; //ITEM ROTATION SPEED
    public float itemMotionSpeed = 5.0f; //ITEM UP-DOWN SPEED
    private UnityEngine.Vector3 itemPosition;
    private string itemType;

    public void Update() 
    {
        if (itemSpawnerHasItem == true) //IF THE ITEM SPAWNER HAS AN ITEM
        {
            //ROTATE VERTICAL AXIS
            transform.Rotate(UnityEngine.Vector3.up, itemRotationSpeed * Time.deltaTime, Space.World);
            //GOES UP AND DOWN 0.5F
            transform.position = itemPosition + new UnityEngine.Vector3(0f, 0.5f * Mathf.Sin(Time.time * itemMotionSpeed), 0f);
        }
    }

    public void Initialize(int _itemSpawnerObjectId, bool _itemSpawnerHasItem, string _itemType)
    {
        itemSpawnerObjectId = _itemSpawnerObjectId;
        itemSpawnerHasItem = _itemSpawnerHasItem;

        itemType = _itemType;

        itemPosition = transform.position;

        if (_itemSpawnerHasItem == true)
            ItemSpawned(itemType); 
    }

    public void ItemSpawned(string _itemType)
    {
        itemType = _itemType;

        if (_itemType.Equals("BasicBomb"))
        {
            itemSpawner_HealModel.gameObject.SetActive(false);
            itemSpawner_ShieldModel.gameObject.SetActive(false);
            itemSpawner_BasicBombModel.gameObject.SetActive(true);
        }
        else if(_itemType.Equals("Heal"))
        {
            itemSpawner_BasicBombModel.gameObject.SetActive(false);
            itemSpawner_ShieldModel.gameObject.SetActive(false);
            itemSpawner_HealModel.gameObject.SetActive(true);
        }
        else if(_itemType.Equals("Shield"))
        {
            itemSpawner_BasicBombModel.gameObject.SetActive(false);
            itemSpawner_HealModel.gameObject.SetActive(false);
            itemSpawner_ShieldModel.gameObject.SetActive(true);
        }

        itemSpawnerHasItem = true;
    }

    public void ItemPickedUp(int playerIDWhoTookItem)
    {
        //PLAY ITEM PICK UP SOUND
        try
        {
            if (Vector3.Distance(ItemActivator.player.transform.position, this.transform.position) < ItemActivator.sightDistance)
                GameMusic.instance.PlayItemPickUpSound();
        }
        catch (System.Exception) { }
        
        itemSpawnerHasItem = false;

        itemSpawner_BasicBombModel.gameObject.SetActive(false);
        itemSpawner_HealModel.gameObject.SetActive(false);
        itemSpawner_ShieldModel.gameObject.SetActive(false);


        if (playerIDWhoTookItem == Client.instance.myID) //IF IT IS ME, THEN ADD THE ITEM INTO MY INVENTORY
        {
            if(itemType.Equals("BasicBomb"))
            {
                _ = Inventory.instance.InventoryAttemptToAddItem(1);
            }
            else if(itemType.Equals("Heal"))
            {

            }
            else if(itemType.Equals("Shield"))
            {
                _ = Inventory.instance.InventoryAttemptToAddItem(2);
            }
        }
    }
}