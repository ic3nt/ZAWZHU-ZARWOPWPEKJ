using UnityEngine;

public enum AspectType 
{ 
    Hacker, 
    Exterminator, 
    Researcher, 
    Mechanic
}

[CreateAssetMenu(fileName = "NewAspect", menuName = "ScriptableObjects/Aspect")]
public class Aspect : ScriptableObject
{
    public AspectType aspectType;
    public Skill[] Skills;
}
