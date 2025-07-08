using System;
using System.Linq;
using Alchemy.Inspector;
using UnityEngine;

public delegate void OnExplodedDelegate();

/// <summary>
/// An object that can explode at its current position applying an explosion force to all rigidbodies in a given radius
/// </summary>
public class Explodable : MonoBehaviour
{
    [SerializeField, Min(0)] private float explosionRadius;
    [SerializeField, Min(0)] private float explosionStrength;
    [SerializeField] private bool showExplosionEffect;
    [SerializeField, ShowIf(nameof(showExplosionEffect))] private GameObject explosionPrefab;
    [SerializeField] private bool showBulletTimeEffect;
    [SerializeField, ShowIf(nameof(showBulletTimeEffect))] private AnimationCurve bulletTimeCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField, ShowIf(nameof(showBulletTimeEffect))] private float bulletTimeDuration = 2;
    
    public OnExplodedDelegate onExploded;

    private float explosionTime = float.MinValue;
    
    [Button]
    public void Explode()
    {
        var colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        colliders.Select(it => it.GetComponent<Rigidbody>())
            .Where(it => it != null)
            .ToList()
            .ForEach(it => it.AddExplosionForce(explosionStrength, transform.position, explosionRadius));
        
        onExploded?.Invoke();
        explosionTime = Time.unscaledTime;
        if(showExplosionEffect)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
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
