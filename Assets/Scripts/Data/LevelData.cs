using Game;
using UnityEngine;

namespace Data
{
    public class LevelData : ScriptableObject
    {
        public int LevelId => _levelId;
        public int BallCount => _ballCount;
        public int BallTargetCount => _ballTargetCount;

        public Tube Tube => _tube;

        [SerializeField] private int _levelId;
        [SerializeField] private int _ballCount;
        [SerializeField] private int _ballTargetCount;
        [SerializeField] private Tube _tube;

        public void SetFields(int levelId, int ballCount, int ballTargetCount, Tube tube)
        {
            _levelId = levelId;
            _ballCount = ballCount;
            _ballTargetCount = ballTargetCount;
            _tube = tube;
        }
    }
}