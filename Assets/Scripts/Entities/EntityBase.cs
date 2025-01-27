using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class EntityBase : MonoBehaviour
{
    protected Health Health;

    protected virtual void Awake()
    {
        Health = GetComponent<Health>();
    }
}