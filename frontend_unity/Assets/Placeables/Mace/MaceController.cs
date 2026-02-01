using UnityEngine;

public class MaceController : MonoBehaviour, IKillsPlayer
{
    private enum State { IdleAtTop, Falling, WaitingAtBottom, Rising }

    [SerializeField] private float fallSpeed = 20f;
    [SerializeField] private float riseSpeed = 6f;
    [SerializeField] private float maxFallDistance = 30f;
    [SerializeField] private float waitAtBottomSeconds = 1f;

    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float detectWidth = 1.2f;
    [SerializeField] private float detectHeight = 8.0f;

    [SerializeField] private LayerMask standableLayer;

    private Collider2D maceCol;

    private State state = State.IdleAtTop;
    private Vector3 startPos;

    private float bottomWaitTimer;

    private void Awake()
    {
        startPos = transform.position;
        maceCol = GetComponent<Collider2D>();
    }

    private void Update()
    {
        switch (state)
        {
            case State.IdleAtTop:
                if (PlayerBelowStart())
                    state = State.Falling;
                break;

            case State.Falling:
                TickFalling();
                break;

            case State.WaitingAtBottom:
                bottomWaitTimer -= Time.deltaTime;
                if (bottomWaitTimer <= 0f)
                    state = State.Rising;
                break;

            case State.Rising:
                TickRising();
                break;
        }
    }

    public void KillPlayer(PlayerController player)
    {
        player.Die();
    }

    private void TickFalling()
    {
        float move = fallSpeed * Time.deltaTime;

        RaycastHit2D hit = Physics2D.Raycast(
            origin: transform.position,
            direction: Vector2.down,
            distance: move,
            layerMask: standableLayer
        );

        if (hit.collider != null)
        {
            transform.position = new Vector3(
                transform.position.x,
                hit.point.y + 0.5f,
                transform.position.z
            );

            state = State.WaitingAtBottom;
            bottomWaitTimer = waitAtBottomSeconds;
            return;
        }

        transform.position += Vector3.down * move;

        if (transform.position.y <= startPos.y - maxFallDistance)
        {
            Destroy(gameObject);
        }
    }

    private void TickRising()
    {
        transform.position = Vector3.MoveTowards(transform.position, startPos, riseSpeed * Time.deltaTime);

        if ((transform.position - startPos).sqrMagnitude <= 0.0001f)
        {
            transform.position = startPos;
            state = State.IdleAtTop;
        }
    }

    private bool PlayerBelowStart()
    {
        Vector2 origin = startPos;
        Vector2 center = origin + Vector2.down * (detectHeight * 0.5f);
        Vector2 size = new Vector2(detectWidth * 2f, detectHeight);

        Collider2D hit = Physics2D.OverlapBox(center, size, 0f, playerLayer);
        return hit != null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 s = Application.isPlaying ? startPos : transform.position;
        Vector3 center = s + Vector3.down * (detectHeight * 0.5f);
        Gizmos.DrawWireCube(center, new Vector3(detectWidth * 2f, detectHeight, 0f));
    }
}
