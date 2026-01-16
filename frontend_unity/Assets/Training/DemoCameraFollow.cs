using Unity.MLAgents;
using UnityEngine;

public class DemoCameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10);

    void LateUpdate()
    {
        if (!Academy.Instance.IsCommunicatorOn)
        {
            if (target == null)
            {
                var agent = FindFirstObjectByType<PlayerAgent>();
                if (agent != null)
                    target = agent.transform;
            }

            if (target != null)
                transform.position = target.position + offset;
        }
    }
}
