using System.Collections;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public int projectileID;
    public GameObject explosionPrefab;
    private readonly float explosionTimer = 1.0f;

    public void Initialize(int _projectileID)
    {
        projectileID = _projectileID;

        try
        {
            if (Vector3.Distance(ItemActivator.player.transform.position, this.transform.position) < ItemActivator.sightDistance)
                GameMusic.instance.GetComponent<GameMusic>().PlayBasicBombAttackSound();
        }
        catch (System.Exception) { }
    }

    GameObject explosionEffect;
    public void Explode(Vector3 _explosionPosition)
    {
        try
        {
            transform.position = _explosionPosition;
            explosionEffect = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            explosionEffect.transform.parent = this.transform; //MAKE EXPLOSION EFFECT GAME OBJECT A CHILD OF THIS MAIN BOMB OBJECT

            if (this.gameObject.activeSelf == true)
                StartCoroutine(ExplosionDies());
            else
            {
                GameObject.Find("ItemActivatorObject").GetComponent<ItemActivator>().hukoObjects.Add(this.gameObject);
                GameManager.projectileObjects.Remove(projectileID); //DELETE THE BOMB FROM THE DICTIONARY
            }

            GetComponent<MeshRenderer>().enabled = false; //MAKE BOMB INVISIBLE LIKE THIS
        }
        catch (System.Exception) { }
    }

    //EXPLOSIONG EFFECT LASTS FOR X SECONDS
    private IEnumerator ExplosionDies()
    {
        try
        {
            if (Vector3.Distance(ItemActivator.player.transform.position, this.transform.position) < ItemActivator.sightDistance)
                GameMusic.instance.GetComponent<GameMusic>().PlayBasicBombExplosionSound();
        }
        catch (System.Exception) { }

        yield return new WaitForSeconds(explosionTimer);
        GameManager.projectileObjects.Remove(projectileID); //DELETE THE BOMB FROM THE DICTIONARY
        Destroy(gameObject); //DESTROYES BOTH EXPLOSION EFFECT AND BOMB 
    }
}