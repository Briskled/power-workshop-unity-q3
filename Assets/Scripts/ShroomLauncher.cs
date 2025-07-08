#region

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

#endregion

public class ShroomLauncher : MonoBehaviour
{
    [SerializeField] private float pixelToBoostFactor = 0.1f;
    [SerializeField] private LineRenderer predictionLineRenderer;
    [SerializeField] [Min(0)] private float predictionTime = 5;
    [SerializeField] private int predictionDownsample = 10;
    [SerializeField] private float maxVelocity = 20;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform shroomSpawnPoint;
    [SerializeField] private List<GameObject> shroomPrefabs;

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
            var velocity = delta * pixelToBoostFactor;
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
        var prefab = shroomPrefabs[randomIndex];
        var newShroomObj = Instantiate(prefab, shroomSpawnPoint.position, Quaternion.identity);
        currentShroom = newShroomObj;
        var shroomRigid = currentShroom.GetComponent<Rigidbody>();
        shroomRigid.isKinematic = true;
    }
}