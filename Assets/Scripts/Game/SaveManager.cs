using UnityEngine;

namespace Game
{
    public static class SaveManager 
    {
        private const string LevelIndexPref = "LevelIndex";
        
        public static int GetLevelIndex()
        {
            return PlayerPrefs.GetInt(LevelIndexPref, 0);
        }
        
        public static void SetLevelIndex(int levelIndex)
        {
            PlayerPrefs.SetInt(LevelIndexPref, levelIndex);
        }
    }
}