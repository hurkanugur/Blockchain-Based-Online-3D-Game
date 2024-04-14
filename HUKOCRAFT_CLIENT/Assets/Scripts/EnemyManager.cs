using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int enemyID;
    public float enemyCurrentHealth;
    public float enemyMaxHealth;
    private string enemyName;
    private string enemyWorld;

    public void Initialize(int _enemyID, float _enemyMaxHP, string _enemyWorld)
    {
        enemyID = _enemyID;
        enemyMaxHealth = _enemyMaxHP;
        enemyWorld = _enemyWorld;

        enemyCurrentHealth = enemyMaxHealth;

        if (enemyWorld.Equals("Earth"))
            enemyName = "Sorcerer";
        else if (enemyWorld.Equals("Nightmare"))
            enemyName = "Machine Sorcerer";
        else if (enemyWorld.Equals("Mirror"))
            enemyName = "Exiled Sorcerer";
        else if (enemyWorld.Equals("Asylum"))
            enemyName = "Forgotten Soldier";

            this.GetComponentInChildren<HPMPManager>().Initialize(enemyName, this.tag, enemyMaxHealth);
    }

    public void EnemySetHealth(float _setHealth, int enemyExperience, int _playerIDWhoShoot)
    {
        enemyCurrentHealth = _setHealth;

        if (enemyCurrentHealth <= 0f)
        {
            if (_playerIDWhoShoot == Client.instance.myID)
            {
                GameChat.instance.Announcement("You have slain an enemy\nGained " + enemyExperience + " EXP");
                _ = ExperienceManager.instance.AddExperiencePoints(enemyExperience, true);
            }

            try
            {
                if (Vector3.Distance(ItemActivator.player.transform.position, this.transform.position) < ItemActivator.sightDistance)
                    GameMusic.instance.PlayEnemyDieSound();
            }
            catch (System.Exception) { }
            
            GameManager.enemyObjects.Remove(enemyID);
            Destroy(gameObject);
        }
        else
            this.GetComponentInChildren<HPMPManager>().SetHealth(enemyCurrentHealth);
    }
}
