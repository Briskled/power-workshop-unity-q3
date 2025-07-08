#region

using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

/// <summary>
/// This script handles bejaviour of a customer by utilizing the <see cref="Crushable.OnCrushed"/> delegate.
/// </summary>
[RequireComponent(typeof(Crushable))]
public class Customer : MonoBehaviour
{
    [SerializeField] private List<GameObject> customerLookPrefabs;

    [MinMaxRangeSlider(0, 20)] [SerializeField]
    private Vector2 crushableThresholdRange;

    private Crushable crushable;

    public Crushable Crushable => crushable ??= GetComponent<Crushable>();

    private void Start()
    {
        Destroy(transform.GetChild(0).gameObject);

        var crushThreshold = Random.Range(crushableThresholdRange.x, crushableThresholdRange.y);
        Crushable.ImpactThreshold = crushThreshold;

        var randomIndex = Random.Range(0, customerLookPrefabs.Count);
        var randomPrefab = customerLookPrefabs[randomIndex];
        var newLook = Instantiate(randomPrefab, transform);
        newLook.transform.localPosition = Vector3.zero;
        newLook.transform.localRotation = Quaternion.Euler(0, 180, 0);
        newLook.transform.localScale = Vector3.one * 4;
    }

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