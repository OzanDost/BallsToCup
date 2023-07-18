using System;
using System.Collections.Generic;
using Data;
using ThirdParty;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private List<LevelData> _levelList;

        private const string LevelSaveDataPath = "LevelDatas";
        private LevelData _lastActiveLevel;
        private int _levelIndex;
        private bool _finishedAllLevels;


        private void Awake()
        {
            AddListeners();
        }

        private void AddListeners()
        {
            Signals.Get<LevelSuccess>().AddListener(OnLevelSuccess);
        }

        private void OnLevelSuccess()
        {
            _levelIndex++;
            SaveManager.SetLevelIndex(_levelIndex);
        }

        public void Initialize()
        {
            _levelIndex = SaveManager.GetLevelIndex();
        }

        public void CreateLevel(bool retry = false)
        {
            if (_levelIndex >= _levelList.Count)
            {
                _finishedAllLevels = true;
            }

            var levelIndex = GetLevelIndex();
            var data = _levelList[levelIndex];

            if (retry)
            {
                data = _lastActiveLevel;
            }

            Signals.Get<RequestGameplayInitialize>().Dispatch(data);
            _lastActiveLevel = data;
        }


        private int GetLevelIndex()
        {
            if (_finishedAllLevels)
            {
                return _levelIndex = Random.Range(0, _levelList.Count);
            }

            return _levelIndex;
        }
    }
}