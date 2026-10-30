using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;
using UnityEngine.XR.Interaction.Toolkit.Transformers;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Pooling;

namespace UnityEngine.XR.Interaction.Toolkit
{
    public class SliceObject : MonoBehaviour
    {
        public Transform startSlicePoint;
        public Transform endSlicePoint;
        public VelocityEstimator velocityEstimator;
        public LayerMask sliceableLayer;
        public Material crossSectionMaterial;
        public float cutForce = 10;
        //[SerializeField] public XRGrabInteractable _xrGr;

        void FixedUpdate()
        {
            bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer);
            if (hasHit)
            {
                GameObject target = hit.transform.gameObject;
                Slice(target);
            }
        }

        public void Slice(GameObject target)
        {
            Vector3 velocity = velocityEstimator.GetVelocityEstimate();
            Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocity);
            planeNormal.Normalize();
            SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);
            if (hull != null)
            {
                GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
                SetupSlicedComponent(upperHull, target);
                GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);
                SetupSlicedComponent(lowerHull, target);
                Destroy(target);

                Destroy(upperHull, 30);
                Destroy(lowerHull, 30);
            }
        }

        public void SetupSlicedComponent(GameObject slicedObject, GameObject target)
        {
            Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
            MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
            XRGrabInteractable xrgi = slicedObject.AddComponent<XRGrabInteractable>();
            collider.convex = true;
            slicedObject.transform.position = target.transform.position;
        }
    }
}