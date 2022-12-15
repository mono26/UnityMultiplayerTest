using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MultiplayerTest
{
    [Serializable]
    public class OnTriggerInteractionEvent : UnityEvent<InteractableComponent> { }

    public class InteractableComponent : MonoBehaviour
    {
        [SerializeField]
        private OnTriggerInteractionEvent onTriggerInteraction = new OnTriggerInteractionEvent();

        private List<InteractionComponent> interactors = new List<InteractionComponent>();

        public OnTriggerInteractionEvent OnTriggerInteraction => this.onTriggerInteraction;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out InteractionComponent interactor)) {
                return;
            }

            if (this.interactors.Contains(interactor)) {
                return;
            }

            Log.Info("Interactor entered.");

            this.interactors.Add(interactor);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out InteractionComponent interactor)) {
                return;
            }

            if (!this.interactors.Contains(interactor)) {
                return;
            }

            this.interactors.Remove(interactor);
        }

        public void OnInteractedBy(InteractionComponent interactor)
        {
            if (interactor == null) {
                return;
            }

            if (!this.interactors.Contains(interactor)) {
                return;
            }

            Log.Info("OnInteractedBy");

            this.onTriggerInteraction?.Invoke(this);
        }
    }
}
