using System;

namespace Data
{
    [Serializable]
    public class LevelData
    {
        public int Id { get; private set; }
        public int BallCount { get; private set; }
        public int BallTargetCount { get; private set; }
        
        public LevelData(int id, int ballCount, int ballTargetCount)
        {
            Id = id;
            BallCount = ballCount;
            BallTargetCount = ballTargetCount;
        }
    }
}