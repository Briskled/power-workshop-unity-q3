#region

using UnityEngine;

#endregion

[RequireComponent(typeof(PowerUp))]
public class PowerUpBoost : MonoBehaviour
{
    [SerializeField] private float velocityMultiplier = 5;
    [SerializeField] private GameObject collectionEffectPrefab;
    [SerializeField] private GameObject shroomParticlesPrefab;

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
        var rigid = shroom.GetComponent<Rigidbody>();
        rigid.linearVelocity *= velocityMultiplier;
        Instantiate(collectionEffectPrefab, transform.position, Quaternion.identity);
        var shroomFire = Instantiate(shroomParticlesPrefab, shroom.transform);
        shroomFire.transform.localPosition = Vector3.zero;
        shroomFire.transform.localScale = Vector3.one;
    }
}