using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    //PLAYER ID INFO
    public int playerID;
    public string playerUsername;
    public string ETHAccountAddress;

    //PLAYER HEALTH INFO
    public float playerCurrentHealth;
    public float playerCurrentMana;
    public float playerMaxHPMPLimit = 0f;

    public Text levelText;
    public ParticleSystem levelUpEffect;

    public GameObject ShieldGameObject;

    public void Start()
    {
        if (playerID == Client.instance.myID)
        {
            UpdatePlayerLevel(ExperienceManager.instance.levelText.text, false);
            ClientSend.PlayerLevelUp(levelText.text, false); //UPDATES THE NEW LEVEL AND KILLS LEVELING UP EFFECT
        }
    }

    public void ChangeShieldState(bool isActivated, string shieldType)
    {
        if (shieldType.Equals("Normal") || shieldType.Equals("Revive"))
        {
            ShieldGameObject.SetActive(isActivated);

            //ACTIVATE SHIELD SOUND
            if (Client.instance.myID == playerID)
                GameMusic.instance.isShieldActive = isActivated;
        }
        else if (shieldType.Equals("Protection")|| shieldType.Equals("Deactivate"))
        {
            ShieldGameObject.SetActive(isActivated);

            //ACTIVATE SHIELD SOUND
            if (Client.instance.myID == playerID)
                GameMusic.instance.isShieldActive = isActivated;
        }
    }

    public void Initialize(int _id, string _username, string _playerETHAccountAddress)
    {
        playerID = _id;
        playerUsername = _username;
        ETHAccountAddress = _playerETHAccountAddress;
        this.playerCurrentHealth = playerMaxHPMPLimit;
        this.GetComponentInChildren<HPMPManager>().Initialize(playerUsername, this.tag);
    }

    public void UpdatePlayerLevel(string _levelText, bool _levelEffectActivated)
    {
        levelText.text = _levelText;
        if (_levelEffectActivated == true)
        {
            levelUpEffect.gameObject.SetActive(true);
            levelUpEffect.Play();
        }
        else
        {
            levelUpEffect.gameObject.SetActive(false);
            levelUpEffect.Stop();
        }      
    }

    //SETS THE PLAYER GIVEN VALUE HEALTH
    public void PlayerSetHealth(float _setHealth, float _maxHPMPLimit, int _playerDeathExperience, int _objectIDWhoShootPlayer)
    {
        this.playerMaxHPMPLimit = _maxHPMPLimit;
        this.playerCurrentHealth = _setHealth;
        int getKilledExperience = _playerDeathExperience / 10;

        if (this.playerCurrentHealth <= 0.0f)
        {
            //KILL YOURSELF
            if ((_objectIDWhoShootPlayer == Client.instance.myID) && _objectIDWhoShootPlayer == playerID) 
            {
                GameChat.instance.Announcement("You have killed yourself\nLost " + getKilledExperience + " EXP");
                _ = ExperienceManager.instance.AddExperiencePoints(getKilledExperience, false);
            }
            //KILL SOMEONE ELSE
            else if ((_objectIDWhoShootPlayer == Client.instance.myID) && _objectIDWhoShootPlayer != playerID) 
            {
                GameChat.instance.Announcement("You have slain " + GameManager.playerObjects[playerID].playerUsername + "\nGained " + _playerDeathExperience + " EXP");
                _ = ExperienceManager.instance.AddExperiencePoints(_playerDeathExperience, true);
            }
            //GET KILLED
            else if ((_objectIDWhoShootPlayer != -1) && (_objectIDWhoShootPlayer != Client.instance.myID) && (Client.instance.myID == playerID))
            {
                GameChat.instance.Announcement("You have been slain by " + GameManager.playerObjects[_objectIDWhoShootPlayer].playerUsername + "\nLost " + getKilledExperience + " EXP");
                _ = ExperienceManager.instance.AddExperiencePoints(getKilledExperience, false);
            }
            else if (_objectIDWhoShootPlayer == -1 && (Client.instance.myID == playerID))
            {
                GameChat.instance.Announcement("You have been slain by Enemy AI\nLost " + getKilledExperience + " EXP");
                _ = ExperienceManager.instance.AddExperiencePoints(getKilledExperience, false);
            }

            //PLAY PLAYER DEATH SOUND
            try
            {
                if (Vector3.Distance(ItemActivator.player.transform.position, this.transform.position) < ItemActivator.sightDistance)
                    GameMusic.instance.PlayPlayerDieSound();
            }
            catch (System.Exception) { }
            
            this.playerCurrentHealth = 0;
            PlayerDies();
        }
        this.GetComponentInChildren<HPMPManager>().SetHealth(playerCurrentHealth);
    }

    //SETS THE PLAYER GIVEN VALUE HEALTH
    public void PlayerSetMana(float _setMana, float _maxHPMPLimit)
    {
        this.playerMaxHPMPLimit = _maxHPMPLimit;
        this.playerCurrentMana = _setMana;

        this.GetComponentInChildren<HPMPManager>().SetMana(playerCurrentMana);
    }

    public void PlayerDies()
    {
        foreach(MeshRenderer playerBody in this.gameObject.GetComponentsInChildren<MeshRenderer>())
            playerBody.enabled = false;
        
        this.gameObject.GetComponentInChildren<Canvas>().enabled = false;
    }
    public void PlayerRespawns()
    {
        foreach (MeshRenderer playerBody in this.gameObject.GetComponentsInChildren<MeshRenderer>())
            playerBody.enabled = true;

        PlayerSetHealth(playerMaxHPMPLimit, playerMaxHPMPLimit, 0, playerID);
        PlayerSetMana(playerMaxHPMPLimit, playerMaxHPMPLimit);
        this.gameObject.GetComponentInChildren<Canvas>().enabled = true;
    }
}