using UnityEngine;
using Utils;

namespace DefaultNamespace
{
    public static class ConfigHelper
    {
        public static GameConfig GameConfig => _gameConfig;
        private static GameConfig _gameConfig;

        public static void Initialize()
        {
            _gameConfig = Resources.Load<GameConfig>(PathHelper.GameConfigPath);
        }
    }
}