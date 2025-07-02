using UnityEngine;
[CreateAssetMenu(fileName = "PlayerStatsSO", menuName = "ScriptableObjects/PlayerStatsSO", order = 1)]
public class PlayerStatsSo : ScriptableObject
{
    public int Money = 10000;
    public int PlayerLevel = 1;
    public int PlayerExperience = 0;
    int xpNeeded = 1000;

    public void ChangeMoneyUp(int amount)
    {
        Money += amount;
    }
    public void ChangeMoneyDown(int amount)
    {
        Money -= amount;
    }
    void LevelUp()
    {
        PlayerLevel++;
    }
    public void AddExperience(int experience)
    {
        PlayerExperience += experience;

        if (PlayerExperience >= xpNeeded)
        {
            PlayerExperience -= xpNeeded;
            LevelUp();

            if (PlayerLevel <= 100) xpNeeded = Mathf.CeilToInt(xpNeeded * 1.1f);

            if (PlayerExperience >= xpNeeded) AddExperience(0);
        }
    }
}
