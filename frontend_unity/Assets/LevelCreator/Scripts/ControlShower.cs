using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlShower : MonoBehaviour
{
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private TMP_Text buttonText;

    [SerializeField] private string showText = "Show Controls";
    [SerializeField] private string hideText = "Hide Controls";

    private void Start()
    {
        HideControls();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !IsTyping())
        {
            ToggleControls();
        }
    }

    public void ShowControls()
    {
        controlsPanel.SetActive(true);
        UpdateButtonText();
    }

    public void HideControls()
    {
        controlsPanel.SetActive(false);
        UpdateButtonText();
    }

    public void ToggleControls()
    {
        controlsPanel.SetActive(!controlsPanel.activeSelf);
        UpdateButtonText();
    }

    private void UpdateButtonText()
    {
        buttonText.text = controlsPanel.activeSelf ? hideText : showText;
    }

    private bool IsTyping()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;
        if (current == null)
        {
            return false;
        }

        return current.GetComponent<TMP_InputField>() != null;
    }

}
