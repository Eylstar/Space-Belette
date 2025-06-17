using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{
    public List<Mission> missions;
    public static event Action OnMissionCompleted;
    public static Mission CurrentMission;
    [SerializeField] RectTransform grid;
    [SerializeField] GameObject missionBtn;
    [SerializeField] TextMeshProUGUI missionTxt;
    [SerializeField] Button startBtn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(RectTransform child in grid)
        {
            Destroy(child.gameObject);
        }
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < missions.Count; i++)
        {
            GameObject btn = Instantiate(missionBtn, grid);
            SetButtons(btn, i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentMission == null)
        {
            missionTxt.text = "No active mission";
            startBtn.interactable = false;
            return;
        }
        else if(CurrentMission != null && CurrentMission.isActive)
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
