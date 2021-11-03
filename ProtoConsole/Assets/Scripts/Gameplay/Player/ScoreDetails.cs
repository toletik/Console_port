using System.Collections.Generic;

public struct ScoreDetails
{
    public int score;
    public List<DeathType> allDeaths;
    public int numberOfKills;

    public int rank;
}

public enum DeathType { ACCIDENT, KILL };