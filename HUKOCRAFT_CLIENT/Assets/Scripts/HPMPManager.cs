using UnityEngine;
using UnityEngine.UI;

public class HPMPManager : MonoBehaviour
{
    public Slider hpBarSlider;
    public Slider mpBarSlider;
    public Text username;
    public Text hpText; //ONLY FOR SCREEN TEXT 
    public Text mpText; //ONLY FOR SCREEN TEXT

    private string creatureType = "Menu"; //DEFAULT
    private float maxHP;
    private float maxMP;
    private Color prevHPColor, prevMPColor;

    public void Initialize(string _username, string _type, float _enemyMaxHP = 0.0f)
    {
        username.text = _username;

        if (_type.Equals("Player"))
        {
            maxHP = maxMP = 100;
            SetHealth(maxHP);
            SetMana(maxMP);
            username.color = Color.green;
        }
        else if (_type.Equals("Enemy"))
        {
            maxHP = _enemyMaxHP;
            hpBarSlider.maxValue = _enemyMaxHP;
            SetHealth(maxHP);
            username.color = Color.white;
        }
        creatureType = _type;
    }

    public void FixedUpdate()
    {
        if (this.CompareTag("Menu")) //IF IT IS SCREEN'S HP/MP BAR, ONLY TRANSFER PLAYER'S HP&MP IN HERE (DO NOT ROTATE ANYTHING)
        {
            try
            {
                if (username.text.Equals("U S E R N A M E"))
                {
                    username.text = GameManager.playerObjects[Client.instance.myID].playerUsername;
                    maxHP = maxMP = 100.0f;
                }
                maxHP = maxMP = hpBarSlider.maxValue = mpBarSlider.maxValue = GameManager.playerObjects[Client.instance.myID].playerMaxHPMPLimit;
                hpBarSlider.value = GameManager.playerObjects[Client.instance.myID].playerCurrentHealth;
                mpBarSlider.value = GameManager.playerObjects[Client.instance.myID].playerCurrentMana;
                hpText.text = (int)hpBarSlider.value + " / " + maxHP;
                mpText.text = (int)mpBarSlider.value + " / " + maxMP;
            }
            catch (System.Exception) { /*That means Main Player is not created yet*/ }
        }
        else //ELSE MAKE EVERY HP/MP BAR LOOK AT THE CAMERA
            this.transform.LookAt(this.transform.position + CameraController.instance.transform.forward);


        //IF PLAYER LEVELS UP, UPDATE HP MP BAR OVER THE PLAYER
        if (creatureType.Equals("Player"))
        {
            if (this.gameObject.GetComponentInParent<PlayerManager>().playerMaxHPMPLimit != maxHP ||
                this.gameObject.GetComponentInParent<PlayerManager>().playerMaxHPMPLimit != maxMP)
            {
                maxHP = maxMP = this.gameObject.GetComponentInParent<PlayerManager>().playerMaxHPMPLimit;
                hpBarSlider.maxValue = mpBarSlider.maxValue = this.gameObject.GetComponentInParent<PlayerManager>().playerMaxHPMPLimit;
                hpBarSlider.value = maxHP; //SetHealth(maxHP);
                mpBarSlider.value = maxMP; //SetMana(maxMP);
            }
        }

        //HP & MP BAR COLOR EVENTS WHEN THE VALUE DROPS BELOW THE CRITICAL RATE (40% OF MP/HP)
        if (hpBarSlider != null)
        {
            if (hpBarSlider.value > 0.0f)//IF THE ONE IS ALIVE
            {
                if (hpBarSlider.value <= (maxHP * 40.0f / 100.0f)) //---> KRITIK LIMITE ULASINCA ISIKLAR YANIP SONCEK
                {
                    if (prevHPColor == Color.black)
                        hpBarSlider.GetComponentInChildren<Image>().color = Color.white;
                    else
                        hpBarSlider.GetComponentInChildren<Image>().color = Color.black;

                    prevHPColor = hpBarSlider.GetComponentInChildren<Image>().color;
                }
                else
                    hpBarSlider.GetComponentInChildren<Image>().color = new Color(218 / 255f, 13 / 255f, 26 / 255f); //RED
            }
            else
                hpBarSlider.GetComponentInChildren<Image>().color = new Color(218 / 255f, 13 / 255f, 26 / 255f); //RED
        }
        if (mpBarSlider != null)
        {
            if (hpBarSlider.value > 0.0f) //IF THE ONE IS ALIVE
            {
                if (mpBarSlider.value <= (maxMP * 40.0f / 100.0f)) //---> KRITIK LIMITE ULASINCA ISIKLAR YANIP SONCEK
                {
                    if (prevMPColor == Color.black)
                        mpBarSlider.GetComponentInChildren<Image>().color = Color.white;
                    else
                        mpBarSlider.GetComponentInChildren<Image>().color = Color.black;

                    prevMPColor = mpBarSlider.GetComponentInChildren<Image>().color;
                }
                else
                    mpBarSlider.GetComponentInChildren<Image>().color = new Color(50 / 255f, 31 / 255f, 245 / 255f); //BLUE
            }
            else
                mpBarSlider.GetComponentInChildren<Image>().color = new Color(50 / 255f, 31 / 255f, 245 / 255f); //BLUE
        }
    }

    public void SetHealth(float _setHealth)
    {
        hpBarSlider.value = _setHealth;
    }

    public void SetMana(float _setMana)
    {
        mpBarSlider.value = _setMana;
    }
}