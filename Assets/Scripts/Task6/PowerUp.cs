#region

using UnityEngine;

#endregion

public delegate void OnPowerUpCollectedDelegate(Shroom shroom);

[RequireComponent(typeof(Collider))]
public class PowerUp : MonoBehaviour
{
    public OnPowerUpCollectedDelegate onPowerUpCollected;

    private Collider _collider;

    public Collider Collider => _collider ??= GetComponent<Collider>();

    private void Start()
    {
        Collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // This function gets executed whenever any collider gets in contact with this object.
        // Let's check if this is a shroom

        if (!other.TryGetComponent(out Shroom shroom))
            return;

        // this is only reached if there is a shroom component
        onPowerUpCollected?.Invoke(shroom);
        Destroy(gameObject);
    }
}