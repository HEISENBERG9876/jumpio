using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ToolbarController : MonoBehaviour
{
    [Header("UI")]
    public Transform buttonContainer;
    public Button buttonPrefab;

    [Header("Placeables")]
    public List<Placeable> placeables;
    private Placeable currentSelection;
    private List<Button> buttons = new List<Button>();

    void Start()
    {
        BuildToolbar();
        if (placeables.Count > 0)
        {
            SelectPlaceable(placeables[0]);
        }

        Debug.Log("buttonContainer: " + buttonContainer);
        Debug.Log("buttonPrefab: " + buttonPrefab);
        Debug.Log("placeables count: " + placeables.Count);
    }

    private void BuildToolbar()
    {
        foreach (Placeable p in placeables)
        {
            var button = Instantiate(buttonPrefab, buttonContainer);
            button.image.sprite = p.icon;
            button.onClick.AddListener(() => SelectPlaceable(p));

            buttons.Add(button);
        }
    }

    private void SelectPlaceable(Placeable p)
    {
        currentSelection = p;
        for (int i = 0; i < buttons.Count; i++)
        {
            var outline = buttons[i].GetComponent<Outline>();
            if (!outline)
            {
                outline = buttons[i].gameObject.AddComponent<Outline>();
                outline.effectColor = Color.yellow;
                outline.effectDistance = new Vector2(3f, -3f);
            }
            outline.enabled = placeables[i] == currentSelection;
        }
    }

    public Placeable GetSelection()
    {
        return currentSelection;
    }
}
