using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    public List<Mission> missions;
    public static event Action OnMissionCompleted;
    public static Mission CurrentMission;
    [SerializeField] RectTransform grid;
    [SerializeField] GameObject missionBtn;
    [SerializeField] TextMeshProUGUI missionTxt;
    [SerializeField] Button startBtn;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        SetupMissionSelection();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MissionSelection")
        {
            SetupMissionSelection();
        }
    }

    private void SetupMissionSelection()
    {
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<RectTransform>();
        startBtn = GameObject.FindGameObjectWithTag("ValidBtn").GetComponent<Button>();
        missionTxt = GameObject.FindGameObjectWithTag("Text").GetComponent<TextMeshProUGUI>();

        foreach (RectTransform child in grid)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < missions.Count; i++)
        {
            GameObject btn = Instantiate(missionBtn, grid);
            SetButtons(btn, i);
        }
        startBtn.onClick.RemoveAllListeners();
        startBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Space");
        });
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "MissionSelection")
        {
            return;
        }
        if (CurrentMission == null)
        {
            missionTxt.text = "No active mission";
            startBtn.interactable = false;
            return;
        }
        else if (CurrentMission != null && CurrentMission.isActive)
        {
            startBtn.interactable = true;
            return;
        }
        else if (CurrentMission != null && !CurrentMission.isActive)
        {
            startBtn.interactable = false;
        }

        if (CurrentMission != null && CurrentMission.isCompleted)
        {
            OnMissionCompleted?.Invoke();
            CurrentMission.isActive = false;
            if (CurrentMission.missionID + 1 < missions.Count)
                missions[CurrentMission.missionID + 1].isActive = true;
        }
    }
    void SetButtons(GameObject btn, int index)
    {
        btn.GetComponentInChildren<TextMeshProUGUI>().text = missions[index].missionName;
        btn.GetComponent<Button>().onClick.AddListener(() =>
        {
            CurrentMission = missions[index];
            missionTxt.text = CurrentMission.description;
            Debug.Log("Current Mission: " + CurrentMission.missionName);
        });
    }
}
