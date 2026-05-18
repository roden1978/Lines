using System.Collections.Generic;

public interface ILeaderboardService
{
    IReadOnlyList<StringSource> Leaders {get;}
    void UpdateLeadersData(IReadOnlyList<Result> results);
    //void LoadLeaders();
}
