using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(Crushable))]
public class SpawnOnDeath : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabsToSpawn;

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

    private void OnCrushed(float impactforce)
    {
        prefabsToSpawn.ForEach(prefab => Instantiate(prefab, transform.position, Quaternion.identity));
    }
}