using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class AgentAbilityUI : MonoBehaviour
{
    public Image agentAbilityIcon;
    public TMP_Text agentAbilityTitle;
    public TMP_Text agentAbilityDescription;

    private AbilityData ability;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void setAgentAbility(AbilityData a)
    {
        ability = a;
        SetagentAbilityIcon();
        SetagentAbilityText();
    }

    private void SetagentAbilityIcon()
    {
        agentAbilityIcon.GetComponent<Image>().sprite = ability.icon;
        agentAbilityIcon.GetComponent<Image>().preserveAspect = true;
    }
    private void SetagentAbilityText()
    {
        this.Scramble(agentAbilityTitle.text, ability.abilityName, .20f, (result) =>
        {
            agentAbilityTitle.text = result;
        });

        this.Scramble(agentAbilityDescription.text, ability.description, .20f, (result) =>
        {
            agentAbilityDescription.text = result;
        });
    }

}
