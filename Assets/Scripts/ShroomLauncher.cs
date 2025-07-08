#region

using System.Collections.Generic;
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.InputSystem;

#endregion

/// <summary>
/// This component is responsible for launching shooting a shroom.
/// It also handles resetting shrooms and resetting missed throws 
/// </summary>
public class ShroomLauncher : MonoBehaviour
{
    [BoxGroup("Throwing Settings")] [SerializeField]
    private float pixelToVelocityFactor = 0.1f;

    [BoxGroup("Throwing Settings")] [SerializeField]
    private float maxVelocity = 35;

    [BoxGroup("Spawn Settings")] [SerializeField]
    private Transform shroomSpawnPoint;

    [BoxGroup("Spawn Settings")] [SerializeField]
    private List<GameObject> shroomPrefabs;

    [BoxGroup("Prediction Settings")] [SerializeField]
    private LineRenderer predictionLineRenderer;

    [BoxGroup("Prediction Settings")] [SerializeField] [Min(0)]
    private float predictionTime = 5;

    [BoxGroup("Prediction Settings")] [SerializeField]
    private int predictionDownsample = 10;


    private GameObject currentShroom;
    private bool isAiming;
    private Vector2 mouseStartPosition;
    private Vector2 delta;
    private Rigidbody shroomRigidbody;

    private Rigidbody ShroomRigidbody => currentShroom.GetComponent<Rigidbody>();


    private Vector3 LaunchVelocity
    {
        get
        {
            var velocity = delta * pixelToVelocityFactor;
            return velocity.normalized * Mathf.Min(velocity.magnitude, maxVelocity);
        }
    }

    private void Update()
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
        predictionLineRenderer.transform.position = Vector3.zero;
        predictionLineRenderer.positionCount = points.Count;
        predictionLineRenderer.SetPositions(points.ToArray());
    }

    private void StartAiming()
    {
        ShroomRigidbody.linearVelocity = Vector3.zero;
        ShroomRigidbody.angularVelocity = Vector3.zero;
        predictionLineRenderer.gameObject.SetActive(true);
    }

    private void ReleaseShroom()
    {
        ShroomRigidbody.isKinematic = false;
        ShroomRigidbody.linearVelocity = LaunchVelocity;
        predictionLineRenderer.gameObject.SetActive(false);
    }

    private List<Vector3> CalculatePredictionPoints()
    {
        var points = new List<Vector3>();
        var steps = predictionTime / (Time.fixedDeltaTime * predictionDownsample);
        for (var i = 0; i < steps; i++)
            points.Add(GetShroomPositionAtTime(i * predictionDownsample * Time.fixedDeltaTime));

        return points;
    }

    private Vector3 GetShroomPositionAtTime(float t)
    {
        return currentShroom.transform.position + LaunchVelocity * t + Physics.gravity * (.5f * t * t);
    }

    public void ResetState()
    {
        if (!currentShroom)
            return;

        currentShroom.transform.position = shroomSpawnPoint.position;
        var shroomRigid = currentShroom.GetComponent<Rigidbody>();
        shroomRigid.isKinematic = true;
    }

    public void NextShroom()
    {
        var randomIndex = Random.Range(0, shroomPrefabs.Count);
        var shroomPrefab = shroomPrefabs[randomIndex];

        var newShroomObj = Instantiate(shroomPrefab, shroomSpawnPoint.position, Quaternion.identity);
        currentShroom = newShroomObj;
        var shroomRigid = currentShroom.GetComponent<Rigidbody>();
        shroomRigid.isKinematic = true;
    }
}