using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStatsSo player;

    private void OnEnable()
    {
        player = Resources.Load<PlayerStatsSo>("ScriptableObjects/PlayerStatsSO");
        EndMission.MissionReward += GetReward;
    }

    private void GetReward(int xp, int credit)
    {
        player.ChangeMoneyUp(credit);
        player.AddExperience(xp);
    }

    private void OnDisable()
    {
        EndMission.MissionReward -= GetReward;
    }

}
