using UnityEngine;

public class TestNormal : MonoBehaviour
{
    [Range(0, 5)]
    [SerializeField] private int index = 0;
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (index > other.contacts.Length - 1)
            return;
        
        var contact = other.contacts[0];
        // Debug.DrawRay(contact.point, contact.normal, Color.red, 1f);
    }
}
