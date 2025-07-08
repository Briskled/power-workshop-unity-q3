using System;
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
    public class RigidbodyAutoMass : MonoBehaviour
    {
        [SerializeField, Min(0)] private float weightPerCubicMeter = 1;
        
        private Rigidbody rigid;
        private Collider collider;
        
        public Rigidbody Rigidbody => rigid ??= GetComponent<Rigidbody>();
        public Collider Collider => collider ??= GetComponent<Collider>();

        private void OnValidate()
        {
            UpdateMass();
        }

        private void Start()
        {
            UpdateMass();
        }

        [Button]
        private void UpdateMass()
        {
            var bounds = Collider.bounds;
            var volume = bounds.extents.magnitude;
            
            Rigidbody.mass = volume * weightPerCubicMeter;
        }
    }