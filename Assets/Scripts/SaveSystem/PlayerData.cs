[System.Serializable]
public class PlayerData
{
    public int coins = 0;
    public int level = 1;

    public int maxHpStatus = 1;
    public int maxPowerStatus = 1;

    public PlayerData(Player player)
    {
        coins = player.coins;
        level = player.level;

        maxHpStatus = player.maxHpStatus;
        maxPowerStatus = player.maxPowerStatus;
    }

    public PlayerData() { }
}