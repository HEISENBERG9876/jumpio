using UnityEngine;

public class EditorModeToggle : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private MonoBehaviour[] gameplayScripts;

    public void Apply(bool isEditor)
    {
        if (rb != null)
        {
            rb.bodyType = isEditor
                ? RigidbodyType2D.Kinematic
                : RigidbodyType2D.Dynamic;

            rb.simulated = !isEditor;
            rb.linearVelocity = Vector2.zero;
        }

        foreach (var script in gameplayScripts)
        {
            if (script != null)
                script.enabled = !isEditor;
        }

        if (animator != null)
            animator.enabled = !isEditor;
    }
}
