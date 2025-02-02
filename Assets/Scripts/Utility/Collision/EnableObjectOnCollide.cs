using System;
using UnityEngine;

public class EnableObjectOnCollide : MonoBehaviour
{
    [SerializeField] private GameObject obj;

    private void OnTriggerEnter2D(Collider2D other)
    {
        obj.SetActive(true);
    }
}
