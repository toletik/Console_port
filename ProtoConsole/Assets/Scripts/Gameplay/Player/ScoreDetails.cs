using System.Collections.Generic;

public struct ScoreDetails
{
    public int score;
    public List<DeathType> allDeaths;
    public int numberOfKills;
}

public enum DeathType { ACCIDENT, KILL };