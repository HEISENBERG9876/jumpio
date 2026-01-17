using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAgent : Agent
{
    [Header("Refs")]
    public Rigidbody2D rb;
    public ArenaEpisodeController arena;
    public PlayerMovement movement;
    public PlayerController player;

    [Header("Raycast observations")]
    public LayerMask standableMask;
    public LayerMask enemyMask;
    public LayerMask spikeMask;
    public LayerMask bulletMask;
    public LayerMask finishMask;

    public float frontScanDistance = 10f;
    public float shortScanDistance = 5f;
    public float frontRayOffsetX = 0f;
    public float[] frontRayHeights = new float[] { -2f, -1.2f, -0.4f, 0.4f, 1.2f, 2f};
    public float[] downProbeForwardOffsets = new float[] { 0.5f, 1.4f, 2.3f, 3.2f, 4.1f, 5f };
    public Vector2 rayBehindOffset = new Vector2(-0.4f, 0f);
    public Vector2 rayAboveOffset = new Vector2(0f, 0.4f);
    public Vector2 rayBelowOffset = new Vector2(-0.5f, -0.4f);

    //rewards
    public float stepPenalty = 0.0002f;
    public float timeoutPenalty = 0.15f;
    public float forwardRewardScale = 0.02f;
    public float unnecesaryJumpPenalty = 0.01f;

    private float prevX;


    protected override void Awake()
    {
        base.Awake();
        if (!rb)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        if (player != null)
        {
            player.Died += OnDied;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector2 v = rb.linearVelocity;
        sensor.AddObservation(Mathf.Clamp(v.x / 6f, -1f, 1f));
        sensor.AddObservation(Mathf.Clamp(v.y / 15f, -1f, 1f));

        sensor.AddObservation(movement.IsGroundedPublic ? 1f : 0f);
        sensor.AddObservation(movement.IsGroundedPublic ? 1f : movement.CoyoteTime01);


        AddRayObservations(sensor);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int moveA = actions.DiscreteActions[0]; // 0=left,1=idle,2=right | left got removed so 0=idle, 1 = right
        int jumpA = actions.DiscreteActions[1]; // 0=off,1=held

        float x = moveA == 1 ? 1f : 0f;
        movement.SetMove(x);
        movement.SetJumpHeld(jumpA == 1);

        //rewards
        float currentX = transform.position.x;
        float differenceX = currentX - prevX;

        if (differenceX > 0f)
        {
            AddReward(differenceX * forwardRewardScale);
        }

        prevX = currentX;

        if (stepPenalty > 0f)
        {
            AddReward(-stepPenalty);
        }

        //timeout penalty
        if (MaxStep > 0 && StepCount >= MaxStep - 1)
        {
            AddReward(-timeoutPenalty);
            Academy.Instance.StatsRecorder.Add("TimeoutRate", 1f, StatAggregationMethod.Average);
            Academy.Instance.StatsRecorder.Add("SuccessRate", 0f, StatAggregationMethod.Average);
            Academy.Instance.StatsRecorder.Add("DeathRate", 0f, StatAggregationMethod.Average);

        }
    }

    //for moving manually
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var d = actionsOut.DiscreteActions;
        d[0] = (Input.GetAxisRaw("Horizontal") > 0f) ? 1 : 0;
        d[1] = (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W)) ? 1 : 0;
    }

    public override void OnEpisodeBegin() => arena.StartEpisode();

    public void ResetForNewEpisode(Vector2 spawnLocal)
    {
        transform.localPosition = spawnLocal;
        transform.localRotation = Quaternion.identity;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        prevX = transform.position.x;
    }

    public void Win()
    {
        Academy.Instance.StatsRecorder.Add("SuccessRate", 1f, StatAggregationMethod.Average);
        Academy.Instance.StatsRecorder.Add("TimeoutRate", 0f, StatAggregationMethod.Average);
        Academy.Instance.StatsRecorder.Add("DeathRate", 0f, StatAggregationMethod.Average);
        AddReward(1f);
        EndEpisode();
    }

    private void OnDied()
    {
        Academy.Instance.StatsRecorder.Add("SuccessRate", 0f, StatAggregationMethod.Average);
        Academy.Instance.StatsRecorder.Add("DeathRate", 1f, StatAggregationMethod.Average);
        Academy.Instance.StatsRecorder.Add("TimeoutRate", 0f, StatAggregationMethod.Average);
        AddReward(-1f);
        EndEpisode();
    }

    // rays

    void AddRayObservations(VectorSensor sensor)
    {
        Vector2 right = Vector2.right;
        Vector2 origin = transform.position;

        int envMask = standableMask | enemyMask | spikeMask | bulletMask | finishMask;


        // rays forward
        int hazardMask = enemyMask | spikeMask | bulletMask;
        for (int heightIndex = 0; heightIndex < frontRayHeights.Length; heightIndex++)
        {
            float height = frontRayHeights[heightIndex];
            Vector2 orig = origin + new Vector2(frontRayOffsetX, height);
                AddSingleRayObservation(sensor, orig, right, frontScanDistance, envMask);

        }
        

        //gap detection
        for (int i = 0; i < downProbeForwardOffsets.Length; i++)
        {
            float fx = downProbeForwardOffsets[i];
            Vector2 orig = origin + new Vector2(fx, 0f) + new Vector2(rayBelowOffset.x, rayBelowOffset.y);
            AddSingleRayObservation(sensor, orig, Vector2.down, shortScanDistance, envMask);
        }
    }

    void AddSingleRayObservation(VectorSensor sensor, Vector2 origin, Vector2 direction, float distance, int mask)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, mask);

        float standable = 0f, enemy = 0f, spike = 0f, bullet = 0f, finish = 0f, none = 0f;
        float distance01 = 1f;
        Vector2 platformVelocity = Vector2.zero;

        if (hit.collider == null)
        {
            none = 1f;
        }
        else
        {
            distance01 = Mathf.Clamp01(hit.distance / distance);
            Vector2 hitVelocity = Vector2.zero;
            int layerBit = 1 << hit.collider.gameObject.layer;


            if ((layerBit & standableMask.value) != 0)
            {
                standable = 1f;
                Rigidbody2D hitRb = hit.collider.attachedRigidbody;
                if (hitRb != null)
                {
                    platformVelocity = hitRb.linearVelocity; //moving platforms have velocity
                }
            }
            else if ((layerBit & enemyMask.value) != 0)
            {
                enemy = 1f;
            }
            else if ((layerBit & spikeMask.value) != 0)
            {
                spike = 1f;
            }
            else if ((layerBit & bulletMask.value) != 0)
            {
                bullet = 1f;
            }
            else if ((layerBit & finishMask.value) != 0)
            {
                finish = 1f;
            }
            else none = 1f;
        }

        sensor.AddObservation(standable);
        sensor.AddObservation(enemy);
        sensor.AddObservation(spike);
        sensor.AddObservation(bullet);
        sensor.AddObservation(finish);
        sensor.AddObservation(none);
        sensor.AddObservation(distance01);
        sensor.AddObservation(Mathf.Clamp(platformVelocity.x / 4f, -1f, 1f));
        sensor.AddObservation(Mathf.Clamp(platformVelocity.y / 4f, -1f, 1f));
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying && !rb)
        {
            return;
        }

        Vector2 origin = transform.position;

        int envMask = standableMask | enemyMask | spikeMask | bulletMask | finishMask;

        // forward
        for (int i = 0; i < frontRayHeights.Length; i++)
        {
            float height = frontRayHeights[i];
            Vector2 orig = origin + new Vector2(frontRayOffsetX, height);

                DrawGizmoRay(orig, Vector2.right, frontScanDistance, envMask);
        }

        // gap detection
        for (int i = 0; i < downProbeForwardOffsets.Length; i++)
        {
            float offsetX = downProbeForwardOffsets[i];
            Vector2 orig = origin + new Vector2(offsetX, 0) + new Vector2(rayBelowOffset.x, rayBelowOffset.y);
            DrawGizmoRay(orig, Vector2.down, shortScanDistance, envMask);
        }
    }

    void DrawGizmoRay(Vector2 origin, Vector2 dir, float dist, int mask)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, dist, mask);

        Gizmos.color = hit.collider ? Color.red : Color.green;
        Gizmos.DrawLine(origin, origin + dir.normalized * dist);

        if (hit.collider)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(hit.point, 0.05f);
        }
    }

}
