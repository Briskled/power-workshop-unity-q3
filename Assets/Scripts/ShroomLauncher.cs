using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShroomLauncher : MonoBehaviour
{
    [SerializeField] private float pixelToBoostFactor = 0.1f;
    [SerializeField] private GameObject shroom;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField, Min(0)] private float predictionTime = 5;
    [SerializeField] private int predictionDownsample = 10;
    
    private bool isAiming = false;
    private Vector2 mouseStartPosition;
    private Vector2 delta;

    private Vector3 LaunchVelocity => delta * pixelToBoostFactor;

    // Update is called once per frame
    void Update()
    {
        var mouse = Mouse.current;
        var currentMousePosition = mouse.position.ReadValue();
        
        if (mouse.leftButton.isPressed && !isAiming)
        {
            isAiming = true;
            mouseStartPosition = currentMousePosition;
            StartAiming();
        }
        if (!mouse.leftButton.isPressed && isAiming)
        {
            isAiming = false;
            ReleaseShroom();
        }

        if (isAiming)
        {
            delta = mouseStartPosition - currentMousePosition;
            UpdateAiming();
        }
    }

    private void UpdateAiming()
    {
        var points = CalculatePredictionPoints();
        lineRenderer.transform.position = Vector3.zero;
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    private List<Vector3> CalculatePredictionPoints()
    {
        var points = new List<Vector3>();
        var steps = predictionTime / (Time.fixedDeltaTime * predictionDownsample);
        for (var i = 0; i < steps; i++)
        {
            points.Add(GetShroomPositionAtTime(i * predictionDownsample * Time.fixedDeltaTime));
        }

        return points;
    }

    private Vector3 GetShroomPositionAtTime(float t) => shroom.transform.position + LaunchVelocity * t + Physics.gravity * (.5f * t * t);

    private void StartAiming()
    {
        lineRenderer.gameObject.SetActive(true);
    }

    private void ReleaseShroom()
    {
        var shroomRigid = shroom.GetComponent<Rigidbody>();
        shroomRigid.linearVelocity = delta * pixelToBoostFactor;
    }
}
