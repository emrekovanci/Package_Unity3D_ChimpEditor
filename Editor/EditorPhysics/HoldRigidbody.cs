using System;

using UnityEngine;

namespace Chimp.Editor.EditorPhysics
{
    internal class HoldRigidbody : IEquatable<Rigidbody>
    {
        public readonly RigidbodyConstraints Constraints;
        public readonly bool IsKinematic;
        public readonly Rigidbody Target;

        public HoldRigidbody(Rigidbody rb)
        {
            Target = rb;
            IsKinematic = rb.isKinematic;
            Constraints = rb.constraints;
        }

        public void Freeze()
        {
            Target.isKinematic = true;
            Target.constraints = RigidbodyConstraints.FreezeAll;
        }

        public void Revert()
        {
            Target.velocity = Vector3.zero;
            Target.angularVelocity = Vector3.zero;
            Target.isKinematic = IsKinematic;
            Target.constraints = Constraints;
        }

        public bool Equals(Rigidbody other) => Target == other;
    }
}