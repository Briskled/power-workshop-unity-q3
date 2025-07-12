#region

using UnityEngine;

#endregion

[RequireComponent(typeof(PowerUp))]
public class PowerUpDuplicate : MonoBehaviour
{
    [SerializeField] [Min(1)] private int duplicateCount = 3;
    [SerializeField] private float spreadAngle = 30;
    [SerializeField] private GameObject collectionEffectPrefab;

    private PowerUp _powerUp;

    public PowerUp PowerUp => _powerUp ??= GetComponent<PowerUp>();

    private void OnEnable()
    {
        PowerUp.onPowerUpCollected += OnCollected;
    }

    private void OnDisable()
    {
        PowerUp.onPowerUpCollected -= OnCollected;
    }

    private void OnCollected(Shroom shroom)
    {
        Instantiate(collectionEffectPrefab, transform.position, Quaternion.identity);
        if (duplicateCount == 1) return;

        var layer = LayerMask.NameToLayer("IgnoreCollision");
        shroom.gameObject.layer = layer;
        var rigid = shroom.GetComponent<Rigidbody>();
        var velocity = rigid.linearVelocity;

        var anglePerDuplicate = spreadAngle / (duplicateCount - 1);
        var halfAngle = spreadAngle / 2f;

        var startAngle = -halfAngle;
        for (var i = 0; i < duplicateCount; i++)
        {
            var targetVelocity = Quaternion.Euler(0, 0, startAngle + anglePerDuplicate * i) * velocity;
            var newShroom = Instantiate(shroom.gameObject, shroom.transform.position, shroom.transform.rotation);
            newShroom.GetComponent<Rigidbody>().linearVelocity = targetVelocity;
            newShroom.layer = layer;
        }

        Destroy(shroom.gameObject);
    }
}