using Data;
using Enums;
using ThirdParty;

public class GameStateChanged : ASignal<GameState, GameState>{} 
public class RequestGameStateChange : ASignal<GameState>{}
public class FakeLoadingFinished : ASignal{}
public class LevelFinished : ASignal<bool>{}
public class LevelRetryRequested : ASignal{}


public class PlayButtonClicked : ASignal{}
public class RequestGameplayInitialize : ASignal<LevelData>{}
public class GameplayInitialized : ASignal<LevelData>{}