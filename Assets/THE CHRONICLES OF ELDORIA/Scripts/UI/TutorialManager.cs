using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private Button closeTutorialPanel;
    private void Start()
    {
        tutorialPanel.SetActive(false);
    }
    //Khi player cham vao vung tutorial se hien thi panel
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialPanel.SetActive(true);
            closeTutorialPanel.onClick.AddListener(CloseTutorialPanel);
        }
    }

    private void CloseTutorialPanel()
    {
        tutorialPanel.SetActive(false);
    }
}
