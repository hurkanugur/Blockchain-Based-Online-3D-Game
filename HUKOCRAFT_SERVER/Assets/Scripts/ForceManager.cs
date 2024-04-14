using UnityEngine;

public class ForceManager : MonoBehaviour
{
    private readonly float objectMass = 3.0F; //CREATURE'S MASS
    private Vector3 impactForce = Vector3.zero; //THIS FORCE WILL PUSH THE TARGET (INITIALLY IT IS SET TO ZERO)

    public void FixedUpdate()
    {
        //PUSH AWAY THE TARGET
        if (GetComponent<CharacterController>().enabled && impactForce.magnitude >= 0.2F)
            GetComponent<CharacterController>().Move(impactForce * Time.deltaTime);
        else
            impactForce = Vector3.zero;

        //CONSUMES THE IMPACT'S ENERGY IN EACH CYCLE
        impactForce = Vector3.Lerp(impactForce, Vector3.zero, 10 * Time.deltaTime);
    }

    public void AddHukoImpact(Vector3 _hitDirectionVector, float _force)
    {
        _hitDirectionVector.Normalize();
        _hitDirectionVector.y = 0; //IF YOU WANNA APPLY FORCE ON ONLY 2D, DO THIS (DEACTIVATE VERTICAL FORCE)
        impactForce += _hitDirectionVector.normalized * _force / objectMass;
    }
}
