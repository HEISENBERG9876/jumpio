using UnityEngine;

//script doesnt work for now
public class PlatformParent : MonoBehaviour
{
    [SerializeField] private LayerMask passengerMask;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsPassenger(collision))
        {
            Debug.Log("Not a passenger");
            return;
        }

        Debug.Log("Setting parent");
        collision.transform.SetParent(transform, true);

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.parent == transform)
        {
            collision.transform.SetParent(null, true);
        }
    }

    private bool IsPassenger(Collision2D collision)
    {
        return (passengerMask.value & (1 << collision.gameObject.layer)) != 0;
    }
}
