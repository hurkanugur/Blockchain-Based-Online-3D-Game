using System;
using System.Collections;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager instance;
    public Slider experienceBar;
    public Text experienceText, levelText; //ONLY FOR SCREEN TEXT
    public BigInteger level = 1;

    private BigInteger characterTotalExperience = 0;
    private BigInteger characterCurrentLevelExp = 0;
    private BigInteger currentLevelRequiredExp = 0;
    public bool isExp100 = false;

    public void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
        {
            Debug.Log("[Error]: ExperienceManager Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public BigInteger LevelCalculation(BigInteger _characterTotalExperience)
    {
        BigInteger nextLevelRequiredExp = 5;
        BigInteger level = 1;

        while (nextLevelRequiredExp <= _characterTotalExperience)
        {
            level++;
            nextLevelRequiredExp += 5 * level;
        }
        return level;
    }

    public void ExperienceAndLevelCalculation()
    {
        BigInteger prevLevelRequiredExp = 0;
        BigInteger nextLevelRequiredExp = 5;
        BigInteger level = 1;

        while (nextLevelRequiredExp <= characterTotalExperience)
        {
            prevLevelRequiredExp = nextLevelRequiredExp;
            level++;
            nextLevelRequiredExp += 5 * level;
        }

        this.level = level;
        characterCurrentLevelExp = characterTotalExperience - prevLevelRequiredExp;
        currentLevelRequiredExp = nextLevelRequiredExp - prevLevelRequiredExp;
        float ExpBarPercentage = ((float)characterCurrentLevelExp * 100) / ((float)currentLevelRequiredExp);

        experienceText.text = "EXP " + ExpBarPercentage + "%";
        levelText.text = "LVL " + level;
        try { GameManager.playerObjects[Client.instance.myID].UpdatePlayerLevel(levelText.text, false); } catch (Exception) { }
        experienceBar.value = (float)ExpBarPercentage;
    }

    public async Task GetExperienceAndLevelFromBlockchain()
    {
        characterTotalExperience = await HukocraftSmartContract.instance.HukocraftShowCharacterExperience();

        if(characterTotalExperience != -1)
            ExperienceAndLevelCalculation();
    }

    public void UpdateExpBar()
    {
        float ExpBarPercentage = ((float)characterCurrentLevelExp * 100) / ((float)currentLevelRequiredExp);
        experienceText.text = "EXP " + ExpBarPercentage + "%";
        experienceBar.value = (float)ExpBarPercentage;
    }

    
    private IEnumerator WaitForAllExperienceTransactionsDone()
    {
        yield return new WaitForSeconds(30f);
        LevelUpProcessIsDone();
    }
    public async void LevelUpProcessIsDone()
    {
        await GetExperienceAndLevelFromBlockchain(); //CHECK BLOCKCHAIN TO GET PLAYER'S REAL TOTAL EXP (IF IT IS LOWER, PLAYER WON'T LEVEL UP)
        ClientSend.PlayerLevelUp(levelText.text, false); //UPDATES THE NEW LEVEL AND KILLS LEVELING UP EFFECT
        isExp100 = false;
    }

    public async Task AddExperiencePoints(BigInteger _expAmount, bool isAddition)
    {  
        if(isExp100 == false && !ETHAccount.instance.myETHField.text.Equals("0 Ether"))
        {
            if (isAddition == false && (characterCurrentLevelExp - _expAmount <= 0))
            {
                characterCurrentLevelExp = 0;
                _ = HukocraftSmartContract.instance.HukocraftSetCharacterExperience(characterCurrentLevelExp, false);
            }
            else if (isAddition == false)
            {
                characterCurrentLevelExp -= _expAmount;
                _ = HukocraftSmartContract.instance.HukocraftSetCharacterExperience(_expAmount, false);
            }
            else if (isAddition == true && (characterCurrentLevelExp + _expAmount >= currentLevelRequiredExp)) //LEVELING UP
            {
                characterCurrentLevelExp = currentLevelRequiredExp;
                UpdateExpBar();
                isExp100 = true;
                ClientSend.PlayerLevelUp(levelText.text, true); //STARTS THE LEVELING UP EFFECT ONLY
                if (await HukocraftSmartContract.instance.HukocraftSetCharacterExperience(_expAmount, true) == true) //IF SUCCESS
                {
                    StartCoroutine(WaitForAllExperienceTransactionsDone());
                }
                else
                {
                    _ = GetExperienceAndLevelFromBlockchain();
                    isExp100 = false;
                    ClientSend.PlayerLevelUp(levelText.text, false); //KILLS LEVELING UP EFFECT
                }
            }
            else
            {
                characterCurrentLevelExp += _expAmount;
                _ = HukocraftSmartContract.instance.HukocraftSetCharacterExperience(_expAmount, true);
            }

            UpdateExpBar();
        }
        else
        {
            if(ETHAccount.instance.myETHField.text.Equals("0 Ether"))
            {
                GameChat.instance.Announcement("You need to have Ether to gain or lose EXP");
            }
            else if(isExp100 == true)
            {
                GameChat.instance.Announcement("You cannot gain or lose EXP while leveling up");
            }
        }
    }
}
