using UnityEngine;

public class GameData : MonoBehaviour
{
    [System.Serializable]   
    public class Data
    {
        public bool isFirstRun = true;
        public bool isPlayerAgreedPlay = false;
        public string language;
        public int frameRateIndex = 1;
        public int windowModeIndex = 0;
        public float gammaValue = 1.0f;
        public bool isVisualMoverEnabled = true;
    }

    public Data gameData = new Data();
}
