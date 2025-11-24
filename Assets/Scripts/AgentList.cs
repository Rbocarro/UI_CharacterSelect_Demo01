using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AgentList", menuName = "Agent/AgentList")]
public class AgentList : ScriptableObject
{
    public List<AgentCharacter> list;
}
