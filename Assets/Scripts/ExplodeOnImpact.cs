#region

using UnityEngine;

#endregion

[RequireComponent(typeof(Explodable))]
[RequireComponent(typeof(Crushable))]
public class ExplodeOnImpact : MonoBehaviour
{
    private Explodable _explodable;
    private Crushable _crushable;

    public Explodable Explodable => _explodable ??= GetComponent<Explodable>();
    public Crushable Crushable => _crushable ??= GetComponent<Crushable>();

    private void OnEnable()
    {
        Crushable.OnCrushed += OnCrushed;
    }

    private void OnDisable()
    {
        Crushable.OnCrushed -= OnCrushed;
    }

    private void OnCrushed(float impactforce)
    {
        Explodable.Explode();
        Destroy(gameObject);
    }
}