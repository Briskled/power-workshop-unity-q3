#region

using UnityEngine;

#endregion

public delegate void OnCrushedDelegate(float impactForce);

[RequireComponent(typeof(Collider))]
public class Crushable : MonoBehaviour
{
    [SerializeField] private float impactThreshold = 10;

    public OnCrushedDelegate OnCrushed;

    private void OnCollisionEnter(Collision collision)
    {
        var impactForce = ImpactForceHelper.Evaluate(collision);
        if (impactForce >= impactThreshold)
        {
            Debug.Log($"Impact {impactForce} on crushable {gameObject.name}. Invoking callback");
            OnCrushed?.Invoke(impactForce);
        }
    }
}