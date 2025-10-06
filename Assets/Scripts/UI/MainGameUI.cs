using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities.EventManager;

public class MainGameUI : MonoBehaviour, IInitializable
{

    [Header("Victory Components")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private Button victoryRestartBtn;

    [Header("Defeat Components")]
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private TMP_Text deathTypeTxt;
    [SerializeField] private Button defeatRestartBtn;

    #region Subscribes

    private void OnEnable()
    {
        victoryRestartBtn.onClick.AddListener(RestartLevel);
        defeatRestartBtn.onClick.AddListener(RestartLevel);
        EventManager.Subscribe(eEventType.onVictoryPanelEnabled, OpenVictoryPanel);
        EventManager.Subscribe(eEventType.onDefeatPanelEnabled, OpenDefeatPanel);
    }

    private void OnDisable()
    {
        victoryRestartBtn.onClick.RemoveAllListeners();
        defeatRestartBtn.onClick.RemoveAllListeners();
        EventManager.Unsubscribe(eEventType.onVictoryPanelEnabled, OpenVictoryPanel);
        EventManager.Unsubscribe(eEventType.onDefeatPanelEnabled, OpenDefeatPanel);
    }

    #endregion

    public void Initialized()
    {
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
    }

    private void OpenVictoryPanel(object arg0)
    {
        victoryPanel.SetActive(true);
    }

    private void OpenDefeatPanel(object arg0)
    {
        string deathTxt = arg0 as string;
        deathTypeTxt.text = deathTxt;
        defeatPanel.SetActive(true);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
