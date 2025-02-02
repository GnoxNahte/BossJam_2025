using System;
using System.Collections;
using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField] private float flyDuration;
    
    private Transform _gemTarget;
    private bool _isFlying = false;
    private void Awake()
    {
        Player player = FindFirstObjectByType<Player>();
        _gemTarget = player.GemTarget;
    }

    public void FlyToPlayer()
    {
        if (_isFlying)
        {
            Debug.LogError("Gem is already flying");
            return;
        }
        StartCoroutine(FlyAnim());
    }

    private IEnumerator FlyAnim()
    {
        _isFlying = true;
        
        float animTime = 0f;
        Vector2 startPos = transform.position;
        
        while (animTime < flyDuration)
        {
            animTime += Time.deltaTime;
            // transform.position = Vector2.Lerp(startPos, _gemTarget.position, animTime / flyDuration);
            transform.position = Vector3.Slerp(startPos, _gemTarget.position, animTime / flyDuration);
            yield return null;
        }
        print("DONE");
    }
}
