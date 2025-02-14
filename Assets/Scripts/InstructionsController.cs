using UnityEngine;
using UnityEngine.UI;

public class InstructionsController : MonoBehaviour
{
    public GameObject InstructPanel;
    public GameObject BackgroundOverlay;
    public Button Instructions;
    public Button CloseButton;

    void Start()
    {
        if (InstructPanel != null)
            InstructPanel.SetActive(false);

        if (BackgroundOverlay != null)
            BackgroundOverlay.SetActive(false);

        if (CloseButton != null)
            CloseButton.gameObject.SetActive(false);

        if (Instructions != null)
            Instructions.onClick.AddListener(ShowInstructions);

        if (CloseButton != null)
            CloseButton.onClick.AddListener(HideInstructions);
    }

    void ShowInstructions()
    {
        if (InstructPanel != null)
            InstructPanel.SetActive(true);

        if (BackgroundOverlay != null)
            BackgroundOverlay.SetActive(true);

        if (CloseButton != null)
            CloseButton.gameObject.SetActive(true);
    }

    void HideInstructions()
    {
        if (InstructPanel != null)
            InstructPanel.SetActive(false);

        if (BackgroundOverlay != null)
            BackgroundOverlay.SetActive(false);

        if (CloseButton != null)
            CloseButton.gameObject.SetActive(false);
    }
}
