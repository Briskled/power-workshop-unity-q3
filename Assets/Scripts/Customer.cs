using System;
using UnityEngine;

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