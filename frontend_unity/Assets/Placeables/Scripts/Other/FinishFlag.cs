using System;
using UnityEngine;

public class FinishFlag : MonoBehaviour
{
    public static event Action Reached;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Reached?.Invoke();
        }
    }
}
