public class InfoPanelBehaviour : Component, IStart, ICanvasComponent
{
    private readonly LinesModel _lines;
    private readonly StartButtonBehaviour _startButtonBehaviour;
    private readonly RestartButtonBehaviour _restartButtonBehaviour;
    private readonly GameOverLabelBehaviour _gameOverLabelBehaviour;
    private readonly IPersistentProgressService _persistentProgressService;
    private readonly ILeaderboardService _leaderboardService;
    private readonly GameModel _gameModel;
    private Button _startButton;
    private Button _restartButton;
    private GameObject _gameOverLabel;

    public InfoPanelBehaviour
    (
        LinesModel lines,
        StartButtonBehaviour startButtonBehaviour,
        RestartButtonBehaviour restartButtonBehaviour,
        GameOverLabelBehaviour gameOverLabelBehaviour,
        GameModel gameModel,
        IPersistentProgressService persistentProgressService,
        ILeaderboardService leaderboardService
    )
    {
        _lines = lines;
        _startButtonBehaviour = startButtonBehaviour;
        _restartButtonBehaviour = restartButtonBehaviour;
        _gameOverLabelBehaviour = gameOverLabelBehaviour;
        _gameModel = gameModel;
        _persistentProgressService = persistentProgressService;
        _leaderboardService = leaderboardService;
    }

    public override void Start()
    {
        _startButton = _startButtonBehaviour.gameObject.GetComponent<Button>();
        _restartButton = _restartButtonBehaviour.gameObject.GetComponent<Button>();
        _gameOverLabel = _gameOverLabelBehaviour.gameObject;
        _startButton.OnClick += OnStartButtonClick;
        _restartButton.OnClick += OnRestartButtonClick;
        _lines.GameOver += OnGameOver;
        //_gameModel.StartGame();
    }

    private void OnGameOver()
    {
        _restartButton.gameObject.SetActive(false);
        _gameOverLabel.SetActive(true);
        _startButton.Interactable = true;
        _persistentProgressService.Save(_persistentProgressService.CurrentResult);
        _leaderboardService.Map();
    }

    private void OnRestartButtonClick(UIEvent uIEvent)
    {
        _gameModel.RestartGame();
        _restartButton.Interactable = false;
        _startButton.Interactable = true;
    }

    private void OnStartButtonClick(UIEvent uIEvent)
    {
        _gameModel.StartGame();
        _restartButton.Interactable = true;
        _startButton.Interactable = false;
        if (_restartButton.gameObject.Active == false)
        {
            _restartButton.gameObject.SetActive(true);
            _gameOverLabel.SetActive(false);
        }
    }

    public override void Destroy()
    {
        _startButton.OnClick -= OnStartButtonClick;
        _restartButton.OnClick -= OnRestartButtonClick;
        _lines.GameOver -= OnGameOver;
    }
}
