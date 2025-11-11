using UnityEngine;



public enum AgentClass { DPS, TANK, FLANK };
[CreateAssetMenu(fileName = "agentCharacter", menuName = "Agent/Character")]
public class AgentCharacter : ScriptableObject
{

    public new string name;
    [TextArea(3, 10)]
    public string description;
    public AgentClass agentClass=AgentClass.DPS;
    public Sprite panelArtwork;
    public Sprite panelTransparentArtwork;
    public Color primaryColor;
    public Color secondaryColor;
    public AbilityData[] abilities;

    public void Print()
    {
        Debug.Log(name + ": " + description );
    }
}
