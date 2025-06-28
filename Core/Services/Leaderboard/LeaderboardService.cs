using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Autofac;

public class LeaderboardService : ILeaderboardService, IStartable
{
    public IReadOnlyList<StringSource> Leaders => _leadersList;
    private readonly IPersistentProgressService _persistentProgressService;
    private List<Result> _leaders = [];
    private ImmutableList<StringSource> _leadersList;
    private ImmutableList<StringSource>.Builder _builder;

    public LeaderboardService(IPersistentProgressService persistentProgressService)
    {
        _persistentProgressService = persistentProgressService;
        _builder = ImmutableList.CreateBuilder<StringSource>();
    }

    private IReadOnlyList<Result> GetLeaders()
    {
        LoadLeaders();
        return _leaders.OrderByDescending(x => x.Value).Take(Settings.LeaderboardCapacity).ToList();
    }

    public void Start()
    {
        IReadOnlyList<Result> values = GetLeaders();
        int count = values.Count;
        for(int i = 0; i < count; i++)
        {
            _builder.Add(new StringSource()
            {Str = $"{values[i].PlayerName} {values[i].Value}"});
        }
        
        for(int i = 0; i < Settings.LeaderboardCapacity - count; i++)
        {
            _builder.Add(new StringSource());
        }

        _leadersList = _builder.ToImmutableList();
    }

    private void LoadLeaders() => _leaders = _persistentProgressService.Load().Results;

    public void Map()
    {
        IReadOnlyList<Result> leaders = GetLeaders();
        
        for(int i = 0; i < leaders.Count; i++)
        {
            _leadersList[i].Str = $"{leaders[i].PlayerName} {leaders[i].Value}";
        }
    }
}