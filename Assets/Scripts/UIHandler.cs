using PrimeTween;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public Image agentCutout;
    private CanvasGroup canvasGroup;
    public float cutoutFadeDuration = 1f;
    public float cutoutMoveDistanceX = 200f; // Distance to move from the left
    private Vector2 cutoutOriginalPosition;

    public AgentCharacter[] Agents;
    public static AgentCharacter currentAgent;
    public static UIHandler Instance { get; private set; }

    public GameObject agentCardPanel;
    public GameObject agentCardPrefab;
    public GameObject agentInfoPanel;
    public GameObject agentAbilityPanel;
    public GameObject agentAbilityCardPrefab;

    [Space]
    [Header("Info Panel")]
    public TMP_Text currentAgentName;
    public TMP_Text currentAgentDescription;

    [Space]
    [Header("BG Elements")]
    public GameObject BG_ScrollingTextcurrentAgentName;

    public Button SelectAgentButton;

    void Awake()
    {
        //canvasGroup = agentCutout.GetComponent<CanvasGroup>();
        //if (canvasGroup == null)    canvasGroup = agentCutout.gameObject.AddComponent<CanvasGroup>();

        //if (Instance != null && Instance != this)
        //{
        //    Destroy(gameObject);
        //    return;
        //}
        Instance = this;

    }
    void Start()
    {
        SetCurrentAgent(Agents[Random.Range(0,Agents.Length)]);
        cutoutOriginalPosition =agentCutout.rectTransform.anchoredPosition;

        //INSTANTIATE CARDS
        for (int i=0;i< Agents.Length;i++)
        {
            GameObject card=Instantiate(agentCardPrefab, agentCardPanel.transform);
            card.name = Agents[i].name; //set the gameobject name in the hierachy not the actual agent name
            card.GetComponent<AgentCard>().SetPanelAgent(Agents[i]);
            //card.GetComponentInChildren<AgentCard>().setPanelAgent(Agents[i]);
        }

        AudioManager.instance.Play("UI_ambient");

        SelectAgentButton.onClick.AddListener(PlayMatchReadyAnimation);

    }

    public void StartFadeIn()
    {
        //StartCoroutine(FadeInAndMove());
    }

    IEnumerator FadeInAndMove()
    {
        float elapsedTime = 0f;
        Vector2 startPos = new Vector2(cutoutOriginalPosition.x - cutoutMoveDistanceX, cutoutOriginalPosition.y);
        Vector2 targetPos = cutoutOriginalPosition;

        //agentCutout.rectTransform.anchoredPosition = startPos;
        agentCutout.rectTransform.position = startPos;
        canvasGroup.alpha = 0f;

        while (elapsedTime < cutoutFadeDuration)
        {
            float t = elapsedTime / cutoutFadeDuration;
            //agentCutout.rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            agentCutout.rectTransform.position = Vector2.Lerp(startPos, targetPos, t);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //agentCutout.rectTransform.anchoredPosition = targetPos;
        agentCutout.rectTransform.position = targetPos;
        canvasGroup.alpha = 1f;
    }
    public void SetCurrentAgent(AgentCharacter agent)
    {   
        currentAgent = agent;
        agentCutout.GetComponent<Image>().sprite = currentAgent.panelTransparentArtwork;
        //agentCutout.GetComponent<Image>().preserveAspect = true;

        BG_ScrollingTextcurrentAgentName.GetComponent<ScrollingText>().Text= currentAgent.name;
        this.Scramble(currentAgentDescription.text, currentAgent.description, .10f, (result) =>
        {
            currentAgentDescription.text = result;
        });
        this.Scramble(currentAgentName.text, currentAgent.name, .10f, (result) =>
        {
            currentAgentName.text = result;
        });

        this.Scramble(BG_ScrollingTextcurrentAgentName.GetComponent<ScrollingText>().Text, currentAgent.name, .10f, (result) =>
        {
            BG_ScrollingTextcurrentAgentName.GetComponent<ScrollingText>().Text = result;
        });

        SetupAgentAbilitesUI(agent);

        StartFadeIn();
        for (int i = 0; i < agentCardPanel.transform.childCount; i++) 
        {
            Transform card = agentCardPanel.transform.GetChild(i);
            if (card.GetComponent<AgentCard>().GetAgent().name !=currentAgent.name)
            {
                card.GetComponent<AgentCard>().CardImageBackground.color = card.GetComponent<AgentCard>().UnselectedColor;
            }
        }
    }
    public static void SetCurrentAgentFromCard(AgentCharacter agent, AgentCard card)
    {
        if (Instance == null)
        {
            Debug.LogWarning("UIHandler instance not found.");
            return;
        }
        if (currentAgent.name != agent.name)
        {
            Instance.SetCurrentAgent(agent);
            card.isSelected = true;
        }
    }

    void SetupAgentAbilitesUI(AgentCharacter a)
    {   
        //Clear all displayed Abilities
        for (int i = 0; i < agentAbilityPanel.transform.childCount; i++)
        {
            Destroy(agentAbilityPanel.transform.GetChild(i).gameObject);
        }

        //Instantiate all abilities UI attached to agent
        for (int i = 0; i < a.abilities.Length; i++)
        {
            GameObject ability = Instantiate(agentAbilityCardPrefab, agentAbilityPanel.transform);
            ability.GetComponent<AgentAbilityUI>().setAgentAbility(a.abilities[i]);
        }
    }

    //void //OnGUI()
    //{
    //    Event currentEvent = Event.current;
    //    Vector2 mousePos = new Vector2();
    //    mousePos.x = currentEvent.mousePosition.x;
    //    mousePos.y = Camera.main.pixelHeight - currentEvent.mousePosition.y;
    //    GUILayout.BeginArea(new Rect(20, 20, 250, 120));
    //    GUI.contentColor = Color.red;
    //    GUILayout.Label("Screen pixels: " + Camera.main.pixelWidth + ":" + Camera.main.pixelHeight);
    //    GUILayout.Label("Mouse Screen position: " + mousePos);
    //    GUILayout.EndArea();
    //}

    void PlayMatchReadyAnimation()
    {
        AudioManager.instance.Play("UI_CardClickEnter");
        var seq = PrimeTween.Sequence.Create();

        seq.Chain(Tween.PositionY(agentCardPanel.transform, -200f, 0.3f, Ease.OutBounce));
        seq.Chain(Tween.PositionX(agentInfoPanel.transform, 200f, 0.3f, Ease.OutBounce));
        seq.Chain(Tween.PositionX(SelectAgentButton.transform, -200f, 0.4f, Ease.OutBounce));
        seq.Chain(Tween.PositionY(agentCutout.transform, 75f, 0.1f, Ease.OutCubic));
        seq.Chain(Tween.Delay(0.5f));

        seq.ChainCallback(() => { 
            Application.OpenURL("https://www.linkedin.com/in/rbocarro/");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }); 
    }

}
