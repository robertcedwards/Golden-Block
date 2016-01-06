﻿using UnityEngine;
using System.Collections;

namespace NewtonVR
{
    public abstract class NVRInteractable : MonoBehaviour
    {
        public Rigidbody Rigidbody;
        public bool CanAttach = true;
        public NVRHand AttachedHand = null;

        protected Collider[] Colliders;
        protected virtual float DropDistance { get { return 3f; } }
        protected Vector3 ClosestHeldPoint;

        public virtual bool IsAttached
        {
            get
            {
                return AttachedHand != null;
            }
        }

        protected virtual void Awake()
        {   
            if (Rigidbody == null)
                Rigidbody = this.GetComponent<Rigidbody>();

            if (Rigidbody == null)
            {
                Debug.LogError("There is no rigidbody attached to this interactable.");
            }

            Colliders = this.GetComponentsInChildren<Collider>();
        }

        protected virtual void Start()
        {
            NVRInteractables.Register(this, Colliders);
        }

        protected virtual void FixedUpdate()
        {
            if (IsAttached == true)
            {
                float shortestDistance = float.MaxValue;

                for (int index = 0; index < Colliders.Length; index++)
                {
                    //todo: this does not do what I think it does.
                    Vector3 closest = Colliders[index].ClosestPointOnBounds(AttachedHand.transform.position);
                    float distance = Vector3.Distance(AttachedHand.transform.position, closest);

                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        ClosestHeldPoint = closest;
                    }
                }

                if (shortestDistance > DropDistance)
                {
                    DroppedBecauseOfDistance();
                }
            }
        }

        //Remove items that go too high or too low.
        protected virtual void Update()
        {
            if (this.transform.position.y > 10000 || this.transform.position.y < -10000)
            {
                if (AttachedHand != null)
                    AttachedHand.EndInteraction(this);

                Destroy(this.gameObject);
            }
        }

        public virtual void BeginInteraction(NVRHand hand)
        {
            AttachedHand = hand;
        }

        public virtual void InteractingUpdate(NVRHand hand)
        {
            if (hand.UseButtonUp == true)
            {
                UseButtonUp();
            }
        }

        public void ForceDetach()
        {
            if (AttachedHand != null)
                AttachedHand.EndInteraction(this);

            if (AttachedHand != null)
                EndInteraction();
        }

        public virtual void EndInteraction()
        {
            AttachedHand = null;
            ClosestHeldPoint = Vector3.zero;
        }

        protected virtual void DroppedBecauseOfDistance()
        {
            AttachedHand.EndInteraction(this);

            Debug.Log("Dropped");
        }

        public virtual void UseButtonUp()
        {

        }
    }
}