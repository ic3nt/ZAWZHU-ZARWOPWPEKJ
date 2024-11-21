using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAspect", menuName = "Game/Aspect")]
public class Aspect1 : ScriptableObject
{
    public string AspectName; // Название аспекта
    public List<Skill1> Skills; // Список скиллов аспекта
    public int Strength;       // Сила аспекта
}
