using System.Collections;
using UnityEngine;

public class ShieldManager : MonoBehaviour
{
    public GameObject playerShieldGameObject;
    private const int SHIELD_DURATION = 10;
    public int shieldID; //IT IS THE SAME AS PLAYER'S ID
    public bool isShieldActivated = false;
    private string shieldType = "Revive";
    private int prevRandomNumber = 1;

    public void Initialize(int _playerID)
    {
        shieldID = _playerID;
    }

    public void FixedUpdate()
    {
        //TUM GAME OBJELERI TARAR VE ILGILI TAG'LARA SAHIP OLANLARA MANYETIK ALAN UYGULAR
        foreach (GameObject search in FindObjectsOfType<GameObject>())
        {
            if (Vector3.Distance(this.gameObject.transform.position, search.transform.position) < 10)
                if (search.tag == "Enemy")
                    search.GetComponent<ForceManager>().AddHukoImpact(-search.transform.forward, 200);
                else if (search.name == "Player" && search.GetComponent<Player>().playerID != shieldID)
                    search.GetComponent<ForceManager>().AddHukoImpact(-search.transform.forward, 200);
                else if (search.tag == "EnemyBasicAttack" || (search.tag == "PlayerBasicAttack" && search.GetComponent<Attack>().creatorID != shieldID))
                {
                    search.GetComponent<Rigidbody>().AddForce(-search.transform.forward * 3000, ForceMode.Force);
                    if (prevRandomNumber == 0)
                        search.GetComponent<Rigidbody>().AddForce(search.transform.right * 100, ForceMode.Force);
                    else
                        search.GetComponent<Rigidbody>().AddForce(-search.transform.right * 100, ForceMode.Force);
                    prevRandomNumber = prevRandomNumber == 0 ? 1 : 0;
                }
                else if (search.tag == "PlayerBasicBomb")
                {
                    if (search.GetComponent<Projectile>().creatorType.Equals("Enemy") ||
                        (search.GetComponent<Projectile>().creatorType.Equals("Player") && search.GetComponent<Projectile>().creatorID != shieldID))
                    {
                        search.GetComponent<Rigidbody>().AddForce(-search.transform.forward * 3000, ForceMode.Force);
                        if (prevRandomNumber == 0)
                            search.GetComponent<Rigidbody>().AddForce(search.transform.right * 100, ForceMode.Force);
                        else
                            search.GetComponent<Rigidbody>().AddForce(-search.transform.right * 100, ForceMode.Force);
                        prevRandomNumber = prevRandomNumber == 0 ? 1 : 0;
                    }
                }
        }
    }

    public void ActivateShield(string shieldType)
        
    {
        this.shieldType = shieldType;

        if (shieldType.Equals("Normal")) //WHEN A NORMAL ITEM IS USED
        {
            playerShieldGameObject.SetActive(true);
            isShieldActivated = true;
            ServerSend.PlayerShieldOperations(shieldID, true, shieldType);

            GetComponentInParent<Player>().currentItemAmount--;

            try { StopCoroutine(ShieldActiveState(shieldType)); } catch (System.Exception) { }
            StartCoroutine(ShieldActiveState(shieldType));
        }
        else if (shieldType.Equals("Revive")) //WHEN THE PLAYER CONNECTS TO THE GAME OR REVIVES
        {
            playerShieldGameObject.SetActive(true);
            isShieldActivated = true;
            ServerSend.PlayerShieldOperations(shieldID, true, shieldType);

            try { StopCoroutine(ShieldActiveState(shieldType)); } catch (System.Exception) { }
            StartCoroutine(ShieldActiveState(shieldType));
        }
        else if (shieldType.Equals("Protection")) //WHEN THE PLAYER IS HAVING A TRADE (INFINITE PROTECTION)
        {
            playerShieldGameObject.SetActive(true);
            isShieldActivated = true;
            ServerSend.PlayerShieldOperations(shieldID, true, shieldType);

            try { StopCoroutine(ShieldActiveState(shieldType)); } catch (System.Exception) { }
        }
        else if (shieldType.Equals("Deactivate")) //DEACTIVATES THE INFINITE PROTECTION
        {
            playerShieldGameObject.SetActive(false);
            isShieldActivated = false;
            ServerSend.PlayerShieldOperations(shieldID, false, shieldType);

            try { StopCoroutine(ShieldActiveState(shieldType)); } catch (System.Exception) { }
        }
    }
    public IEnumerator ShieldActiveState(string shieldType)
    {
        yield return new WaitForSeconds(SHIELD_DURATION);

        if (this.shieldType.Equals("Protection") == false)
        {
            playerShieldGameObject.SetActive(false);
            isShieldActivated = false;
            ServerSend.PlayerShieldOperations(shieldID, false, shieldType);
        }
    }
}
