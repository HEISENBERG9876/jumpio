using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabNavigation : MonoBehaviour
{
    [SerializeField] private Selectable first;
    [SerializeField] private Selectable last;

    private void OnEnable()
    {
        //if (EventSystem.current == null)
        //{
        //    return;
        //}

        //GameObject current = EventSystem.current.currentSelectedGameObject;

        //if (current == null || !current.activeInHierarchy)
        //{
        //    SelectFirstValid();
        //}
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Tab))
        {
            return;
        }

        if (EventSystem.current == null)
        {
            return;
        }

        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        GameObject currentGO = EventSystem.current.currentSelectedGameObject;

        if (currentGO == null || !currentGO.activeInHierarchy)
        {
            SelectFirstValid();
            return;
        }

        Selectable current = currentGO.GetComponent<Selectable>();

        if (current == null || !current.IsActive() || !current.IsInteractable())
        {
            SelectFirstValid();
            return;
        }

        Selectable next;

        if (shift)
        {
            next = current.FindSelectableOnUp();
            if (next == null)
            {
                next = current.FindSelectableOnLeft();
            }
        }
        else
        {
            next = current.FindSelectableOnDown();
            if (next == null)
            {
                next = current.FindSelectableOnRight();
            }
        }

        if (next != null && next.IsActive() && next.IsInteractable())
        {
            next.Select();
            return;
        }

        if (shift)
        {
            SelectLastValid();
        }
        else
        {
            SelectFirstValid();
        }
    }

    private void SelectFirstValid()
    {
        if (first != null && first.IsActive() && first.IsInteractable())
        {
            first.Select();
        }
    }

    private void SelectLastValid()
    {
        if (last != null && last.IsActive() && last.IsInteractable())
        {
            last.Select();
        }
    }
}
