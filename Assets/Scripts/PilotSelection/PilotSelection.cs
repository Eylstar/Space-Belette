using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PilotSelection : MonoBehaviour
{
    [SerializeField] List<Pilot> Pilots;
    [SerializeField] GameObject PilotCardPrefab;
    [SerializeField] RectTransform Content, capsule;
    [SerializeField] Transform Spawner;
    [SerializeField] TextMeshProUGUI ActiveTxt, PassiveTxt;
    Pilot SelectedPilot;
    public static event Action<Pilot> OnPilotSelected;
    
    void Start()
    {
        for(int i = 0; i < Pilots.Count; i++)
        {
            GameObject card = Instantiate(PilotCardPrefab, Content);
            PilotCardContent cardContent = card.GetComponent<PilotCardContent>();
            cardContent.SetName(Pilots[i].pilotName);
            cardContent.SetIcon(Pilots[i].pilotImage);
            cardContent.SetLevel(Pilots[i].pilotLevel);
            cardContent.Pilot = Pilots[i];
            if (Pilots[i].IsUnlocked)
            {
                cardContent.Unlock();
            }
            cardContent.GetComponent<Button>().onClick.AddListener(() => SelectPilot(cardContent.Pilot));
        }
    }

    private void SelectPilot(Pilot pilot)
    {
        Cleanard();
        ResetCapsuleTop();
        Instantiate(pilot.pilotPrefab, Spawner.position, Spawner.rotation, Spawner);
        SelectedPilot = pilot;
        ActiveTxt.text = $"<color=#FF0000>{pilot.ActiveSkill.SkillName}</color>\n<color=#AA0000>{pilot.ActiveSkill.Description}</color>";
        PassiveTxt.text = $"<color=#00FF00>{pilot.PassiveSkill.SkillName}</color>\n<color=#008000>{pilot.PassiveSkill.Description}</color>";
    }
    void Cleanard()
    {
        foreach (Transform child in Spawner)
        {
            Destroy(child.gameObject);
        }
    }
    public void ValidPilot()
    {
        if (SelectedPilot.IsUnlocked)
        {
            float targetY = -capsule.parent.GetComponent<RectTransform>().rect.height;
            StartCoroutine(AnimateCapsuleTop(targetY, 2f));
        }
    }

    private IEnumerator AnimateCapsuleTop(float targetTop, float duration)
    {
        RectTransform rectTransform = capsule;
        float initialTop = rectTransform.offsetMax.y;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newTop = Mathf.Lerp(initialTop, targetTop, elapsedTime / duration);
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, newTop);
            yield return null;
        }

        rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, targetTop);

        if (SelectedPilot.IsUnlocked) 
        {
            Ship.MainPilot = SelectedPilot;
            SceneManager.LoadScene("MissionSelection");
        }
        else
            ResetCapsuleTop();
    }
    void ResetCapsuleTop()
    {
        RectTransform rectTransform = capsule;
        rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, 4f);
    }
}
