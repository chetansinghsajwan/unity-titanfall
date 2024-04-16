using System;
using UnityEngine;

class Health : MonoBehaviour
{
    [SerializeField, Min(0)] protected float _Health;
    public float health
    {
        get => _Health;
        set
        {
            _Health = value;
        }
    }
}