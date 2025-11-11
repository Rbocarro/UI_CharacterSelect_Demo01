using UnityEngine;

[CreateAssetMenu(fileName = "agentAbility", menuName = "Agent/Ability")]
public class AbilityData : ScriptableObject
{
    public string abilityName;
    [TextArea(3, 10)]
    public string description;
    public Sprite icon;
}
