#region

using System.Linq;
using UnityEngine;

#endregion

public static class ImpactForceHelper
{
    public static float Evaluate(Collision collision)
    {
        return collision.contacts.Sum(contact =>
        {
            var myRigidbody = contact.thisCollider.GetComponent<Rigidbody>();
            var otherRigidbody = contact.otherCollider.GetComponent<Rigidbody>();

            var myVelocity = myRigidbody?.linearVelocity ?? Vector3.zero;
            var otherVelocity = contact.otherCollider.GetComponent<Rigidbody>()?.linearVelocity ?? Vector3.zero;

            var myMass = myRigidbody?.mass ?? float.PositiveInfinity;
            var otherMass = otherRigidbody?.mass ?? float.PositiveInfinity;

            var velocityDifference = myVelocity - otherVelocity;
            var meanMass = (otherMass + myMass) / 2f;
            var impactFactor = Mathf.Abs(Vector3.Dot(contact.normal, velocityDifference));
            return (impactFactor * velocityDifference * meanMass).magnitude;
        });
    }
}