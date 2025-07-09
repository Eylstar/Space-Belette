using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndMission : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI EndLabel, PilotXp, Currency;
    [SerializeField] GameObject EndPanel;
    [SerializeField] GameObject EndButton;
    [SerializeField] RectTransform btnContainer;
    Mission mission;
    Pilot pilot;
    int BonusReward;
    PlayerStatsSo playerStats;
    public static event Action NextMission;
    public static event Action<int, int> MissionReward;
    private void OnEnable()
    {

        pilot = Ship.MainPilot;
        PilotXp.text = string.Empty;
        Currency.text = string.Empty;
        EndPanel.SetActive(false);
        mission = MissionManager.CurrentMission;
        ShipMove.PlayerDeath += MissionFail;
        Spawner.EndMission += MissionComplete;
    }
    private void OnDisable()
    {
        //ShipManager.PlayerDeath -= MissionFail;
        Spawner.EndMission -= MissionComplete;
    }
    private void BackToMenu()
    {
        SceneManager.LoadScene("ShipBuilder");
    }
    void MissionFail()
    {
        EndPanel.SetActive(true);
        SetBtns(false);
        EndLabel.text = "Mission Failed";
        PilotXp.text = "0";
        Currency.text = "0";
    }
    
    void MissionComplete()
    {
        Ship ship = FindFirstObjectByType<Ship>();
        BonusReward = ship.ShipCost;
        EndPanel.SetActive(true);
        SetBtns(true);
        EndLabel.text = "Mission Complete";
        PilotXp.text = mission.rewardExperience.ToString();
        Currency.text = $"{mission.rewardCredits.ToString()} + {BonusReward}";

        pilot.AddExperience(mission.rewardExperience);
        MissionReward?.Invoke(mission.rewardExperience, mission.rewardCredits+BonusReward);

    }
    
    void SetBtns(bool Success)
    {
        foreach (Transform child in btnContainer)
        {
            Destroy(child.gameObject);
        }

        if (!Success)
        {
            GameObject btn = Instantiate(EndButton, btnContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = "Menu";
            btn.GetComponent<Button>().onClick.AddListener(BackToMenu);
        }
        else
        {
            GameObject saveBtn = Instantiate(EndButton, btnContainer);
            saveBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Backup";
            saveBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                // Appeller la sauvegarde ici
                // à voir quelle methode on va utiliser
                BackToMenu();
            });

            GameObject nextBtn = Instantiate(EndButton, btnContainer);
            nextBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Next Mission";
            nextBtn.GetComponent<Button>().onClick.AddListener(()=>NextMission?.Invoke());
        }
    }
}
