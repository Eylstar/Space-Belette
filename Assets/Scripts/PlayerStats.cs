using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStatsSo player;

    private void OnEnable()
    {
        player = Resources.Load<PlayerStatsSo>("ScriptableObjects/PlayerStatsSO");
        Bloc.OnFloorPlaced.AddListener(player.ChangeMoneyDown);
        Bloc.OnFloorRemoved.AddListener(player.ChangeMoneyUp);
        EndMission.MissionReward += GetReward;
    }

    private void GetReward(int xp, int credit)
    {
        player.ChangeMoneyUp(credit);
        player.AddExperience(xp);
    }

    private void OnDisable()
    {
        Bloc.OnFloorPlaced.RemoveListener(player.ChangeMoneyDown);
        Bloc.OnFloorRemoved.RemoveListener(player.ChangeMoneyUp);
    }

}
