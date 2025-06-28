using System.Collections.Generic;

public interface ILeaderboardService
{
    IReadOnlyList<StringSource> Leaders {get;}
    void Map();
}
