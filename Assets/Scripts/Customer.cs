#region

using UnityEngine;

#endregion

/// <summary>
/// This script handles bejaviour of a customer by utilizing the <see cref="Crushable.OnCrushed"/> delegate.
/// </summary>
[RequireComponent(typeof(Crushable))]
public class Customer : MonoBehaviour
{
    private Crushable crushable;

    public Crushable Crushable => crushable ??= GetComponent<Crushable>();

    private void OnEnable()
    {
        Crushable.OnCrushed += OnCrushed;
    }

    private void OnDisable()
    {
        Crushable.OnCrushed -= OnCrushed;
    }

    private void OnCrushed(float impactForce)
    {
        Destroy(gameObject);
    }
}