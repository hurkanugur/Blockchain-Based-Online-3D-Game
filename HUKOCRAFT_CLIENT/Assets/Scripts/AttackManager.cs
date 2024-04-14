using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public int attackObjectID;

    public void Initialize(int _attackObjectID)
    {
        try
        {
            if (Vector3.Distance(ItemActivator.player.transform.position, this.transform.position) < ItemActivator.sightDistance)
                GameMusic.instance.GetComponent<GameMusic>().PlayBasicAttackSound();
        }
        catch (System.Exception) { }
        attackObjectID = _attackObjectID;
    }

    public void DestroyAttackObject()
    {
         GameManager.attackObjects.Remove(attackObjectID);
         Destroy(gameObject);
    }
}
