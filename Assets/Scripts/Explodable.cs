#region

using System.Linq;
using Alchemy.Inspector;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

#endregion

public delegate void OnExplodedDelegate();

/// <summary>
/// An object that can explode at its current position applying an explosion force to all rigidbodies in a given radius
/// </summary>
public class Explodable : MonoBehaviour
{
    [SerializeField] [Min(0)] private float explosionRadius;
    [SerializeField] [Min(0)] private float explosionStrength;
    [SerializeField] private bool showExplosionEffect = true;

    [SerializeField] [ShowIf(nameof(showExplosionEffect))] [Required]
    private GameObject explosionPrefab;

    [SerializeField] private bool showBulletTimeEffect = true;

    [SerializeField] [ShowIf(nameof(showBulletTimeEffect))]
    private AnimationCurve bulletTimeCurve = AnimationCurve.Linear(0, 0.05f, 1, 1);

    [SerializeField] [ShowIf(nameof(showBulletTimeEffect))]
    private float bulletTimeDuration = 2;

    [SerializeField] private bool shakeCamera = true;

    [SerializeField] [ShowIf(nameof(shakeCamera))]
    private AnimationCurve shakeCurve = AnimationCurve.Linear(0, 1, 1, 0);

    [SerializeField] [ShowIf(nameof(shakeCamera))]
    private float shakeStrength = 4;

    [SerializeField] [ShowIf(nameof(shakeCamera))]
    private float shakeDuration = 1.5f;

    public OnExplodedDelegate onExploded;

    private float explosionTime = float.MinValue;

    [Button]
    public void Explode()
    {
        // Finds all colliders in the given explosion radius
        var colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        // selects all colliders that have a rigidbody on it and adds an explosion force
        colliders.Select(it => it.GetComponent<Rigidbody>())
            .Where(it => it != null)
            .ToList()
            .ForEach(it => it.AddExplosionForce(explosionStrength, transform.position, explosionRadius));

        onExploded?.Invoke();
        explosionTime = Time.unscaledTime;

        // instantiate explosion. It destroys itself when it is done
        if (showExplosionEffect)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        if (shakeCamera)
        {
            var vCam = FindAnyObjectByType<CinemachineCamera>();
            var noiseModule = vCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
            DOTween.To(() => noiseModule.AmplitudeGain,
                    x => noiseModule.AmplitudeGain = x,
                    shakeStrength,
                    shakeDuration)
                .SetEase(shakeCurve);
        }
    }

    private void Update()
    {
        if (!showBulletTimeEffect)
            return;

        var timeDiffToExplosion = Time.unscaledTime - explosionTime;
        if (timeDiffToExplosion < 0 || timeDiffToExplosion > bulletTimeDuration)
            return;

        var fac = timeDiffToExplosion / bulletTimeDuration;
        Time.timeScale = bulletTimeCurve.Evaluate(fac);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}