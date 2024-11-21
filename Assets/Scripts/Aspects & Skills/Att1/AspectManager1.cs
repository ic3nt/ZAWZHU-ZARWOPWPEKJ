using System.Collections.Generic;
using UnityEngine;

public class AspectManager1 : MonoBehaviour
{
    public static AspectManager1 Instance;

    [System.Serializable]
    public class PlayerAspectData
    {
        public string PlayerName;
        public Aspect1 SelectedAspect;
        public List<Skill1> PlayerSkills;
    }

    public List<PlayerAspectData> Players = new List<PlayerAspectData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AssignAspectToPlayer(string playerName, Aspect1 aspect)
    {
        var playerData = Players.Find(p => p.PlayerName == playerName);
        if (playerData != null)
        {
            playerData.SelectedAspect = aspect;
            playerData.PlayerSkills = new List<Skill1>(aspect.Skills);
        }
        else
        {
            Players.Add(new PlayerAspectData
            {
                PlayerName = playerName,
                SelectedAspect = aspect,
                PlayerSkills = new List<Skill1>(aspect.Skills)
            });
        }
    }
}
