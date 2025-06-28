using System.Linq;
using System.Threading.Tasks;
using Autofac;

public class PersistentProgressService : IPersistentProgressService, IStartable
{
    public Result CurrentResult => _currentResult;
    private readonly IDBService _dBService;
    private readonly ISaveLoadService _saveLoadService;
    private Task<bool> _success;
    private PersistentData _data;
    private Result _currentResult;
    public PersistentProgressService(IDBService dBService, ISaveLoadService saveLoadService)
    {
        _dBService = dBService;
        _saveLoadService = saveLoadService;
        _currentResult = new()
        {
            PlayerName = Settings.PlayerName,
            Game = Settings.GameName
        };
    }
    public PersistentData Load()
    {
        _success = _dBService.Get();

        do { } while (
            false == _success.Status.HasFlag(TaskStatus.Canceled)
            & false == _success.Status.HasFlag(TaskStatus.RanToCompletion)
        );

        if (_success.Status.HasFlag(TaskStatus.RanToCompletion) & _dBService.Results.Count > 0)
        {
            _data = new PersistentData();
            _data.Results.AddRange(_dBService.Results);
        }
        else _data = _saveLoadService.Load() ?? new PersistentData();

        return _data;
    }

    public void Save(Result result)
    {
        if (result.Value == 0) return;

        _currentResult = result;

        bool insert = UpdatePersistentData(result);
        _success = SaveToDB(result, insert);

        do { } while (
            false == _success.Status.HasFlag(TaskStatus.Canceled)
            & false == _success.Status.HasFlag(TaskStatus.RanToCompletion)
        );

        SaveToFile(_data);
    }

    private bool UpdatePersistentData(Result result)
    {
        Result existResult = _data.Results.FirstOrDefault(x => x.PlayerName.Equals(result.PlayerName));
        string playerName = existResult == null ? string.Empty : result.PlayerName;

        if (playerName == string.Empty)
        {
            _data.Results.Add(result);
            return true;
        }
        else
            if (existResult.Value < result.Value)
                existResult.Value = result.Value;

        return false;
    }

    private void SaveToFile(PersistentData data) =>
        _saveLoadService.Save(data);

    private async Task<bool> SaveToDB(Result result, bool insert)
    {
        return await _dBService.Save(result, insert);
    }

    public void Start() => Load();
}