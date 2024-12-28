using UnityEngine;

public class GameData : MonoBehaviour
{
    [System.Serializable]   
    public class Data
    {
        public bool isFirstRun;
        public bool isPlayerAgreedPlay;
        public string language;
        public int frameRateIndex;
        public int windowModeIndex;
        public float gammaValue;
        public bool isVisualMoverEnabled;
    }

    public Data settingsData = new Data();
}
