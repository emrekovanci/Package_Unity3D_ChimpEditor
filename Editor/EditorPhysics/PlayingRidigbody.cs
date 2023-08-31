using UnityEngine;

namespace Chimp.Editor.EditorPhysics
{
    internal class PlayingRigidbody
    {
        public GameObject ActualGameObject;
        public bool ActuallyHadCollider;
        public bool ActuallyHadRigidbody;
        public CollisionDetectionMode OldDetectionMode;
        public Rigidbody TargetRigidbody;

        public PlayingRigidbody(GameObject obj)
        {
            ActualGameObject = obj;
        }

        public void Start()
        {
            var collider = ActualGameObject.GetComponent<Collider>();
            ActuallyHadCollider = collider;

            if (!ActuallyHadCollider)
            {
                collider = ActualGameObject.AddComponent<MeshCollider>();
                (collider as MeshCollider).convex = true;
            }

            TargetRigidbody = ActualGameObject.GetComponent<Rigidbody>();
            ActuallyHadRigidbody = TargetRigidbody;

            if (!ActuallyHadRigidbody)
            {
                TargetRigidbody = ActualGameObject.AddComponent<Rigidbody>();
            }

            OldDetectionMode = TargetRigidbody.collisionDetectionMode;
            TargetRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            TargetRigidbody.velocity = Vector3.zero;
            TargetRigidbody.angularVelocity = Vector3.zero;
        }

        public void Stop()
        {
            if (ActuallyHadRigidbody)
            {
                TargetRigidbody.collisionDetectionMode = OldDetectionMode;
                TargetRigidbody.velocity = Vector3.zero;
                TargetRigidbody.angularVelocity = Vector3.zero;
            }
            else
            {
                Object.DestroyImmediate(TargetRigidbody);
                TargetRigidbody = null;
            }

            if (!ActuallyHadCollider)
            {
                var collider = ActualGameObject.GetComponent<Collider>();
                Object.DestroyImmediate(collider);
                collider = null;
            }
        }
    }
}