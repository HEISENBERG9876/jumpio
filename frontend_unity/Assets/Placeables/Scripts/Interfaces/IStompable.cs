using UnityEngine;

public interface IStompable
{
    void OnStomped(PlayerController player, Collision2D collision);
}
