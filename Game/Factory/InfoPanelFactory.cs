using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class InfoPanelFactory : IFactory<GameObject>
{
    public string Name => GetType().Name;
    private readonly IContentProvider _contentProvider;
    private readonly TitleFactory _titleFactory;
    private readonly InfoPanelBehaviour _infoPanelBehaviour;
    private readonly NextLabelFactory _nextLabelFactory;
    private readonly ScoreLabelFactory _scoreLabelFactory;
    private readonly ScoreTextFactory _scoreTextFactory;
    private readonly StartButtonFactory _startButtonFactory;
    private readonly RestartButtonFactory _restartButtonFactory;
    private readonly NewTilesHolderBehaviour _newTilesHolderBehaviour;
    private readonly GameOverLabelFactory _gameOverLabelFactory;
    private readonly LeaderboardFactory _leaderboardFactory;

    public InfoPanelFactory(IContentProvider contentProvider,
        TitleFactory titleFactory,
        InfoPanelBehaviour infoPanelBehaviour,
        NextLabelFactory nextLabelFactory,
        ScoreLabelFactory scoreLabelFactory,
        ScoreTextFactory scoreTextFactory,
        StartButtonFactory startButtonFactory,
        RestartButtonFactory restartButtonFactory,
        NewTilesHolderBehaviour newTilesHolderBehaviour,
        GameOverLabelFactory gameOverLabelFactory,
        LeaderboardFactory leaderboardFactory)
    {
        _contentProvider = contentProvider;
        _titleFactory = titleFactory;
        _infoPanelBehaviour = infoPanelBehaviour;
        _nextLabelFactory = nextLabelFactory;
        _scoreLabelFactory = scoreLabelFactory;
        _scoreTextFactory = scoreTextFactory;
        _startButtonFactory = startButtonFactory;
        _restartButtonFactory = restartButtonFactory;
        _newTilesHolderBehaviour = newTilesHolderBehaviour;
        _gameOverLabelFactory = gameOverLabelFactory;
        _leaderboardFactory = leaderboardFactory;
    }
    public GameObject Create()
    {
        GameObject infoPanel = new($"InfoPanel", new Vector2(935, 380), 0, Vector2.One);
        infoPanel
        .AddComponent(new CanvasHandler())
        .AddComponent(_infoPanelBehaviour);

        CreateBackgroundShadow(infoPanel.Transform);
        CreateBackground(infoPanel.Transform);
        CreateTitle(infoPanel.Transform);
        GameObject nextLabel = CreateNextDrawer(infoPanel.Transform);
        CreateNewTilesHolder(nextLabel.Transform);
        CreateScoreLabel(infoPanel.Transform);
        CreateScoreText(infoPanel.Transform);
        CreateRestartButton(infoPanel.Transform);
        CreateStartButton(infoPanel.Transform);
        CreateGameOverLabel(infoPanel.Transform);
        CreateLeaderboard(infoPanel.Transform);
        return infoPanel;
    }

    private void CreateLeaderboard(Transform2D parent)
    {
        GameObject leaderboard = _leaderboardFactory.Create();
        leaderboard.Transform.Parent = parent;
    }

    private void CreateGameOverLabel(Transform2D parent)
    {
        GameObject gameOverLabel = _gameOverLabelFactory.Create();
        gameOverLabel.Transform.Parent = parent;
    }

    private void CreateScoreText(Transform2D parent)
    {
        GameObject scoreText = _scoreTextFactory.Create();
        scoreText.Transform.Parent = parent;
    }

    private void CreateScoreLabel(Transform2D parent)
    {
        GameObject scoreLabel = _scoreLabelFactory.Create();
        scoreLabel.Transform.Parent = parent;
    }

    private GameObject CreateNextDrawer(Transform2D parent)
    {
        GameObject nextLabel = _nextLabelFactory.Create();
        nextLabel.Transform.Parent = parent;
        return nextLabel;
    }

    private void CreateNewTilesHolder(Transform2D parent)
    {
        GameObject newTilesHolder = new("NewTilesHolder", new(0, 120), 0, Vector2.One);
        newTilesHolder.Transform.Parent = parent;
        newTilesHolder
        .AddComponent(new CanvasHandler())
        .AddComponent(new GridLayoutGroup(Settings.RandomTilesCount))
        .AddComponent(_newTilesHolderBehaviour);

    }
    private void CreateBackground(Transform2D parent)
    {
        Texture2D backgroundTexture = _contentProvider.GetTextureByType(TextureTypes.InfoPanel);
        GameObject background = new()
        {
            Name = "InfoPanelImage"
        };

        background.Transform.Parent = parent;
        background
        .AddComponent(new CanvasHandler())
        .AddComponent(new UIImage(new Sprite(backgroundTexture))
        {
            Color = Color.CornflowerBlue
        });

    }
    private void CreateBackgroundShadow(Transform2D parent)
    {
        Texture2D backgroundTexture = _contentProvider.GetTextureByType(TextureTypes.InfoPanel);
        GameObject shadow = new("InfoPanelShadow", new(5, 5), 0, Vector2.One);
        shadow.Transform.Parent = parent;
        
        shadow
        .AddComponent(new CanvasHandler())
        .AddComponent(new UIImage(new Sprite(backgroundTexture), .7f)
        {
            Color = Color.Black
        });

    }

    private void CreateTitle(Transform2D parent)
    {
        GameObject title = _titleFactory.Create();
        title.Transform.Parent = parent;
    }

    private void CreateStartButton(Transform2D parent)
    {
        GameObject startButton = _startButtonFactory.Create();
        startButton.Transform.Parent = parent;
    }

    private void CreateRestartButton(Transform2D parent)
    {
        GameObject restartButton = _restartButtonFactory.Create();
        restartButton.Transform.Parent = parent;
    }
}
