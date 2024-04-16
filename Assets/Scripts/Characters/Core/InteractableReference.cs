using UnityEngine;

[RequireComponent(typeof(Collider))]
sealed class InteractableReference : MonoBehaviour
{
    [SerializeField] private Interactable _interactable;
    public Interactable getInteractable => _interactable;
}