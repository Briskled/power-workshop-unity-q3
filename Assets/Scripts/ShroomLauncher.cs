#region

using System.Collections.Generic;
using Alchemy.Inspector;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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

    [BoxGroup("Prediction Settings")] [SerializeField]
    private Gradient predictionLineGradient;

    [BoxGroup("Prediction Settings")] [SerializeField]
    private AnimationCurve predictionLineThickness = AnimationCurve.Constant(0, 1, 1);

    [BoxGroup("Camera Effect Settings")] [SerializeField]
    private CinemachineCamera virtualCamera;

    [BoxGroup("Camera Effect Settings")] [SerializeField] [MinMaxRangeSlider(50, 80)]
    private Vector2 fieldOfViewRange = new(60, 69);

    [BoxGroup("Camera Effect Settings")] [SerializeField]
    private AnimationCurve fieldOfViewLerpCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [BoxGroup("Camera Effect Settings")] [SerializeField]
    private float fieldOfViewEndLerpDuration = 0.7f;

    [BoxGroup("Camera Effect Settings")] [SerializeField]
    private Ease fieldOfViewEndLerpEase = Ease.OutBack;

    [BoxGroup("Particle Effect Settings")] [SerializeField]
    private ParticleSystem particleSystem;

    [BoxGroup("Particle Effect Settings")] [SerializeField] [MinMaxRangeSlider(0, 50)]
    private Vector2 emissionRange = new(0, 35);

    [BoxGroup("Post Processing Settings")] [SerializeField]
    private Volume volume;

    [BoxGroup("Post Processing Settings")] [SerializeField] [MinMaxRangeSlider(0, 1)]
    private Vector2 chromaticAberration;

    [BoxGroup("Post Processing Settings")] [SerializeField]
    private AnimationCurve chromaticAberrationCurve = AnimationCurve.Linear(0, 0, 1, 1);

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

    private void Start()
    {
        ResetAimEffects();
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
            ResetAimEffects();
        }

        if (isAiming)
        {
            delta = mouseStartPosition - currentMousePosition;
            UpdateAiming();
            UpdateAimEffects();
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

    private void UpdateAimEffects()
    {
        var velocityPercentage = LaunchVelocity.magnitude / maxVelocity;

        var color = predictionLineGradient.Evaluate(velocityPercentage);
        var thickness = predictionLineThickness.Evaluate(velocityPercentage);

        // line
        predictionLineRenderer.startWidth = thickness;
        predictionLineRenderer.endWidth = thickness;
        predictionLineRenderer.material.color = color;

        // camera
        virtualCamera.Lens.FieldOfView = Mathf.Lerp(
            fieldOfViewRange.x,
            fieldOfViewRange.y,
            fieldOfViewLerpCurve.Evaluate(velocityPercentage)
        );

        // particles
        var emission = particleSystem.emission;
        emission.rateOverTime = Mathf.Lerp(emissionRange.x, emissionRange.y, velocityPercentage);

        // post processing
        if (volume.profile.TryGet<ChromaticAberration>(out var chroma))
            chroma.intensity.value = Mathf.Lerp(chromaticAberration.x, chromaticAberration.y,
                chromaticAberrationCurve.Evaluate(velocityPercentage));
    }

    private void ResetAimEffects()
    {
        // camera
        DOTween.To(
                () => virtualCamera.Lens.FieldOfView,
                x => virtualCamera.Lens.FieldOfView = x,
                fieldOfViewRange.x, fieldOfViewEndLerpDuration)
            .SetEase(fieldOfViewEndLerpEase);

        // partciels
        var emission = particleSystem.emission;
        emission.rateOverTime = emissionRange.x;

        // post processing
        if (volume.profile.TryGet<ChromaticAberration>(out var chroma))
            chroma.intensity.value = chromaticAberration.x;
        ;
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