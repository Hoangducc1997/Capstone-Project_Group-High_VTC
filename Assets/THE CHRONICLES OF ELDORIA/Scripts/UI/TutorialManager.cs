using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private Button closeTutorialPanel;

    private bool isPlayerInTrigger = false;

    private void Start()
    {
        tutorialPanel.SetActive(false);
        closeTutorialPanel.onClick.AddListener(CloseTutorialPanel);
    }

    private void Update()
    {
        // Nếu player đang trong trigger và nhấn E thì mở panel
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            OpenTutorialPanel();
        }

        // Nhấn ESC để đóng panel
        if (tutorialPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseTutorialPanel();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
        }
    }

    private void OpenTutorialPanel()
    {
        tutorialPanel.SetActive(true);
    }

    private void CloseTutorialPanel()
    {
        tutorialPanel.SetActive(false);
    }
}
