
public class PBonusAtEnd : Skill
{
    public int CurrencyPercentBonus;
    public int XpPercentBonus;
    Pilot pilotToUp;

    public override void Apply(Ship ship, Pilot pilot)
    {
        pilotToUp = pilot;
        MissionManager.OnMissionCompleted += AddBonus;
    }

    public override void Remove(Ship ship, Pilot pilot)
    {
        pilotToUp = null;
        MissionManager.OnMissionCompleted -= AddBonus;
    }
    void AddBonus() 
    {
        var xp = MissionManager.CurrentMission.rewardExperience;
        var currency = MissionManager.CurrentMission.rewardCredits;
        int totalXp = xp + (xp * XpPercentBonus / 100);
        int totalCurrency = currency + (currency * CurrencyPercentBonus / 100);
        pilotToUp.AddExperience(totalXp);
        PlayerStats.player.ChangeMoneyUp(totalCurrency);
    }
}