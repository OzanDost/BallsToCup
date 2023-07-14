using System.Collections.Generic;
using Data;
using ThirdParty;
using UnityEngine;

namespace Game
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private List<LevelData> _levelList;

        private const string LevelSaveDataPath = "LevelDatas";
        private LevelData _lastActiveLevel;
        private int _levelIndex;
        private bool _finishedAllLevels;

        public void Initialize()
        {
            _levelIndex = SaveManager.GetLevelIndex();

            if (_levelIndex >= _levelList.Count)
            {
                _finishedAllLevels = true;
            }
        }

        public void CreateLevel()
        {
            var levelIndex = GetLevelIndex();
            var data = _levelList[levelIndex];

            Signals.Get<RequestGameplayInitialize>().Dispatch(data);
        }


        private int GetLevelIndex()
        {
            if (_finishedAllLevels)
            {
                return Random.Range(0, _levelList.Count);
            }

            return _levelIndex;
        }
    }
}