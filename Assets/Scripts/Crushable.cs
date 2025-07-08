#region

using UnityEngine;

#endregion

public delegate void OnCrushedDelegate(float impactForce);

/// <summary>
/// This script listens for collisions and calls <see cref="OnCrushed"/> when a given <see cref="impactThreshold"/> is reached.
/// The impact force is calculated using <see cref="ImpactForceHelper"/>
/// </summary>
[RequireComponent(typeof(Collider))]
public class Crushable : MonoBehaviour
{
    [SerializeField] private float impactThreshold = 10;

    public OnCrushedDelegate OnCrushed;

    /// <summary>
    /// This is a unity event function that is automatically called for each collision with this object
    /// </summary>
    /// <param name="collision">Collision data</param>
    private void OnCollisionEnter(Collision collision)
    {
        var impactForce = ImpactForceHelper.Evaluate(collision);
        if (impactForce >= impactThreshold) OnCrushed?.Invoke(impactForce);
    }
}