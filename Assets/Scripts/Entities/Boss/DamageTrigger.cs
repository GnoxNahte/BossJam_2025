using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageTrigger : MonoBehaviour
{
    [field: SerializeField] public int Damage { get; private set; }
    [field: SerializeField] public Vector2 KnockbackSpeed { get; private set; }
}
