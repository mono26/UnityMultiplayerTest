using Fusion;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerTest
{
    public class InteractionComponent : SLGNetworkBehaviour
    {
        private const float INTERACTIOM_COOLDOWN = 0.25f;

        private float currentCooldown = 0.0f;

        private GameCharacterInput characterInput = null;

        private List<InteractableComponent> interactables = new List<InteractableComponent>();

        private void Awake()
        {
            this.Initialize();
        }

        private void LateUpdate()
        {
            if (this.characterInput == null) {
                return;
            }

            if (this.currentCooldown > 0) {
                this.currentCooldown -= Time.deltaTime;
                return;
            }

            bool isInteracting = this.characterInput.action;
            if (isInteracting) {
                this.TriggerInteractionWith(this.GetClosesInteractable());
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this.characterInput = this.GetComponent<GameCharacterInput>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out InteractableComponent interactable)) {
                return;
            }

            if (this.interactables.Contains(interactable)) {
                return;
            }

            Log.Info("Interactable entered.");

            this.interactables.Add(interactable);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out InteractableComponent interactable)) {
                return;
            }

            if (!this.interactables.Contains(interactable)) {
                return;
            }

            this.interactables.Remove(interactable);
        }

        private InteractableComponent GetClosesInteractable()
        {
            if (this.interactables.Count == 0) {
                return null;
            }

            float closestSqrDist = (this.transform.position - this.interactables[0].transform.position).sqrMagnitude;
            InteractableComponent closestInteractable = this.interactables[0];
            for (int i = 0; i < this.interactables.Count; i++) {
                InteractableComponent interactable = this.interactables[i];
                float sqrDist = (this.transform.position - interactable.transform.position).sqrMagnitude;
                if (sqrDist < closestSqrDist) {
                    closestSqrDist = sqrDist;
                    closestInteractable = interactable;
                }
            }

            return closestInteractable;
        }

        private void TriggerInteractionWith(InteractableComponent interactable)
        {
            if (interactable == null) {
                return;
            }

            this.currentCooldown = INTERACTIOM_COOLDOWN;

            interactable.OnInteractedBy(this);
        }
    }
}