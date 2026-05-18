using System.Linq;
using System.Threading.Tasks;

public class PersistentProgressService : IPersistentProgressService
{
    private readonly IDBService _dBService;
    private readonly ISaveLoadService _saveLoadService;
    private readonly ILeaderboardService _leaderboardService;
    private PersistentData _data = new();
    public PersistentProgressService(IDBService dBService, ISaveLoadService saveLoadService, ILeaderboardService leaderboardService)
    {
        _dBService = dBService;
        _saveLoadService = saveLoadService;
        _leaderboardService = leaderboardService;
    }

    private PersistentData CreateResultsData()
    {
        for (int i = 0; i < Settings.LeaderboardCapacity; i++)
            _data.Results.Add(new Result());
        return _data;
    }
    public async void Load()
    {
        _data = _saveLoadService.Load() ?? CreateResultsData();
        await _dBService.Get(_leaderboardService.Leaders, _data);

        UpdateLeadersData();
    }

    public async void Save(Result result)
    {
        if (result.Value == 0) return;

        bool insert = UpdatePersistentData(result);

        await SaveToDB(result, insert);

        SaveToFile(_data);
    }

    private bool UpdatePersistentData(Result result)
    {
        Result existResult = _data.Results.FirstOrDefault(x => x.PlayerName.Equals(result.PlayerName));
        string playerName = existResult == null ? string.Empty : result.PlayerName;

        if (playerName == string.Empty)
        {
            _data.Results.Add(result);
            UpdateLeadersData();
            return true;
        }
        else
            if (existResult.Value < result.Value)
        {
            existResult.Value = result.Value;
            UpdateLeadersData();
        }

        return false;
    }

    private void UpdateLeadersData()
    {
        _leaderboardService.UpdateLeadersData(_data.Results);
    }

    private void SaveToFile(PersistentData data) =>
        _saveLoadService.Save(data);

    private async Task<bool> SaveToDB(Result result, bool insert)
    {
        return await _dBService.Save(result, insert);
    }

}