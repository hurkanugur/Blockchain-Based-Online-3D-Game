using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //PLAYER SHOOTING
    public Transform shootingPlace;
    public string mainPlayerName;

    private readonly float playerBasicAttackManaCost = 2.0f;

    public void Start()
    {
        mainPlayerName = GameManager.playerObjects[Client.instance.myID].playerUsername;
    }
    public void Update()
    {
        if (Cursor.visible == false && GameChat.isChatActivated == false
            && ETHAccount.isSendEtherAmountFieldActivated == false && Inventory.isTradeMenuActivated == false)
        {
            //IF PLAYER IS NOT DEAD (EVEN IF IT IS HACKED, PLAYER STILL CAN'T ATTACK, SINCE IT IS A SERVER SIDE ATTRIBUTE)

            if (this.GetComponent<PlayerManager>().playerCurrentHealth > 0.0f)
            {
                //CHECK IF THE LEFT MOUSE BUTTON IS PRESSED (FOR SHOOTING)
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (GameManager.playerObjects[Client.instance.myID].playerCurrentMana > playerBasicAttackManaCost)
                        ClientSend.PlayerShoot(this.GetComponent<PlayerManager>().playerID, shootingPlace.position, shootingPlace.rotation);
                }

                //IF PLAYER IS NOT DEAD (EVEN IF IT IS HACKED, PLAYER STILL CAN'T MOVE, SINCE IT IS A SERVER SIDE ATTRIBUTE)
                PlayerMovementInputs();
            }
        }
    }

    //SENDS PLAYER KEYBOARD INPUTS TO THE SERVER
    private void PlayerMovementInputs()
    {
        bool[] movementKeys = new bool[]
        {
            Input.GetKey(KeyCode.W), //WHENEVER PLAYER CLICKS ON THE KEYS, THE KEY VALUE IS AS TRUE IF IT IS "W"
            Input.GetKey(KeyCode.S), //THE KEY VALUE IS AS TRUE IF IT IS "S"
            Input.GetKey(KeyCode.A), //THE KEY VALUE IS AS TRUE IF IT IS "A"
            Input.GetKey(KeyCode.D), //THE KEY VALUE IS AS TRUE IF IT IS "D"
            Input.GetKey(KeyCode.Space) //THE KEY VALUE IS AS TRUE IF IT IS "Space"
        };

        //WALKING SOUND DISABLED
       /* if (transform.position.y >= 1f && transform.position.y <= 2f)
            if ((movementKeys[0] || movementKeys[1] || movementKeys[2] || movementKeys[3]) && (movementKeys[4] == false))
                GameMusic.instance.PlayWalkingSound();*/

        ClientSend.PlayerMovement(movementKeys); //SEND SERVER WHICH KEY THE PLAYER PRESSED
    }
}