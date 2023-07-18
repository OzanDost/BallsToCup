using Data;
using DefaultNamespace;
using Enums;
using ThirdParty;

public class GameStateChanged : ASignal<GameState, GameState>{} 
public class FakeLoadingFinished : ASignal{}
public class SufficientBallCountReached : ASignal{}
public class LevelSuccess : ASignal{}
public class LevelFailed : ASignal{}
public class LevelRetryRequested : ASignal{}
public class LevelQuitRequested : ASignal{}


public class FinishButtonClicked : ASignal{}
public class PlayButtonClicked : ASignal{}
public class RequestGameplayInitialize : ASignal<LevelData>{}
public class GameplayInitialized : ASignal<LevelData>{}
public class BallEnteredCup: ASignal{}
public class BallFellOut : ASignal{}
public class BallReleaseRequested:ASignal<Ball>{}

public class RequestCameraShake:ASignal{}