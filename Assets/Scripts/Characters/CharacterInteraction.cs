﻿using System;
using UnityEngine;

[DisallowMultipleComponent]
public class CharacterInteraction : MonoBehaviour
{
    //////////////////////////////////////////////////////////////////
    /// Variables
    //////////////////////////////////////////////////////////////////

    public Character Character { get; protected set; }
    protected CharacterCapsule CharacterCapsule => Character.CharacterCapsule;

    [SerializeField, Space] protected InteractionBound m_InteractionBound;
    public InteractionBound InteractionBound
    {
        get => m_InteractionBound;
        set
        {
            if (value != null)
            {
                value.OnInteractableFound = this.OnInteractableFound;
                value.OnInteractableLost = this.OnInteractableLost;
            }

            m_InteractionBound = value;
        }
    }

    [SerializeField] protected InteractionRay m_InteractionRay;
    public InteractionRay InteractionRay
    {
        get => m_InteractionRay;
        set => m_InteractionRay = value;
    }

    //////////////////////////////////////////////////////////////////
    /// Constructors
    //////////////////////////////////////////////////////////////////

    public CharacterInteraction()
    {
    }

    //////////////////////////////////////////////////////////////////
    /// UpdateLoop
    //////////////////////////////////////////////////////////////////

    public void Init(Character character)
    {
        this.Character = character;

        // this sets up the events
        InteractionBound = InteractionBound;
    }

    public void UpdateImpl()
    {
        if (m_InteractionRay)
        {
            OnInteractableFound(m_InteractionRay.FindInteractable());
        }
    }

    //////////////////////////////////////////////////////////////////
    /// Interactables
    //////////////////////////////////////////////////////////////////

    protected void OnInteractableFound(Interactable interactable)
    {
    }

    protected void OnInteractableLost(Interactable interactable)
    {
    }
}