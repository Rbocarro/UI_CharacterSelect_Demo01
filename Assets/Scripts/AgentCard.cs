using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using PrimeTween;
public class AgentCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Image cardImage;
    public Image CardImageBackground;
    public Color UnselectedColor;
    public Color SelectedColor;
    public GameObject cardVisual;
    public Image gradientImage;
    public Image cardOverlay;
    public TMP_Text cardAgentName;

    public AgentCharacter agent { get; set; }
    private Button button;
    private Camera mainCamera;

    public float scaleMultiplier = 1.2f; // Scale increase when hovered
    public float scaleDuration = 5f; // Speed of scaling
    [SerializeField]
    private bool isHovered = false;
    public bool isSelected = false;
    private Vector2 originalScale;
    private Vector3 originalPosition;
    public Vector3 positionOffset;
    public float positionOffsetSpeed;

    public float rotationSpeed;
    public float tiltAmount = 10f;

    void Awake()
    {
        button = cardVisual.GetComponent<Button>();
        originalScale = cardVisual.transform.localScale;
        originalPosition = cardVisual.transform.localPosition;
        positionOffset = cardVisual.transform.localPosition + positionOffset;
        button.onClick.AddListener(OnButtonClick);// Add click listener
        mainCamera =Camera.main;
        CardImageBackground.color=UnselectedColor;
    }
    void Update()
    {
        if (isHovered) RotateTowardsMouse();
    }
    public void SetPanelAgent(AgentCharacter a)
    {
        agent = a;
        SetCardAgentImage();
        SetCardAgentName();
    }
    private void SetCardAgentImage()
    {
        cardImage.sprite = agent.panelArtwork;
        cardImage.preserveAspect = true;
    }
    private void SetCardAgentName()
    {
        cardAgentName.text = string.Empty;
        cardAgentName.text = agent.name;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isHovered) return;
        isHovered = true;
        Tween.StopAll(cardVisual.transform);
        
        Tween.LocalPosition(cardVisual.transform, positionOffset, .5f, Ease.OutBack);
        Tween.PunchScale(cardVisual.transform, originalScale * scaleMultiplier, scaleDuration, 3, true, Ease.InOutBack);
        AudioManager.instance.Play("UI_CardHoverEnter");
        
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isHovered) return;
        isHovered = false;
        Tween.StopAll(cardVisual.transform);
        Tween.LocalPosition(cardVisual.transform, originalPosition, .3f, Ease.Default);
            if (Vector3.Magnitude(cardVisual.transform.localScale- Vector3.one)>0.1f) Tween.Scale(cardVisual.transform,Vector3.one, 0.2f,Ease.Default);
            if (Quaternion.Angle(cardVisual.transform.localRotation, Quaternion.identity) > 0.1f)
                Tween.LocalRotation(cardVisual.transform, Vector3.zero, 0.2f, Ease.Default);
    }
    private void OnButtonClick()
    {
        UIHandler.SetCurrentAgentFromCard(agent, this);
        AudioManager.instance.Play("UI_CardClickEnter");
        CardImageBackground.color=SelectedColor;
        Tween.ShakeLocalRotation(cardVisual.transform, strength: new Vector3(0, 0, 10), duration: 0.3f, frequency: 15);
    }
    void RotateTowardsMouse()//rotate card to face towards mouse when hovered over;
    {   
        // Get mouse position relative to the card
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            cardVisual.transform.parent as RectTransform,
            Input.mousePosition,
            mainCamera,
            out Vector2 localPoint);

        // Normalize to -1, 1 range based on card size
        RectTransform rect = cardVisual.transform as RectTransform;
        Vector2 normalized = new Vector2(
            Mathf.Clamp(localPoint.x / (rect.rect.width / 2f), -1f, 1f),
            Mathf.Clamp(localPoint.y / (rect.rect.height / 2f), -1f, 1f)
        );

        // Apply tilt angles
        float tiltX = -normalized.y * tiltAmount; // up/down
        float tiltY = normalized.x * tiltAmount;  // left/right

        Quaternion targetRotation = Quaternion.Euler(-tiltX, -tiltY, 0f);

        // Smoothly rotate toward target
        cardVisual.transform.localRotation = Quaternion.Lerp(
            cardVisual.transform.localRotation,
            targetRotation,
            Time.deltaTime * rotationSpeed // speed
        );
    }
    private void OnDestroy()
    {
        if (button != null) button.onClick.RemoveListener(OnButtonClick);
    }
}
