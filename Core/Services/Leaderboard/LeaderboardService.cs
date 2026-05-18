using System.Collections.Generic;
using System.Linq;
using Autofac;

public class LeaderboardService : ILeaderboardService, IStartable
{
    public IReadOnlyList<StringSource> Leaders => _leadersList;
    private List<StringSource> _leadersList = [];

   public void Start() => CreateLeadersData();

    private void CreateLeadersData()
    {
        for (int i = 0; i < Settings.LeaderboardCapacity; i++)
        {
            _leadersList.Add(new()
            {
                Value = "Empty"
            });
        }
    }

    public void UpdateLeadersData(IReadOnlyList<Result> results)
    {
        IReadOnlyList<Result> ordered = [.. results.OrderByDescending(x => x.Value)];
        for(int i = 0; i < Settings.LeaderboardCapacity; i++)
        {
            _leadersList[i].Value = ordered[i].Value != 0 ? $"{ordered[i].PlayerName} {ordered[i].Value}" : "Empty"; 
        }
    }

}