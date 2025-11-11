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
    public Image cardSelectedOverlay;//TO DO
    public TMP_Text cardAgentName;

    private AgentCharacter agent;
    private Button button;
    private Camera camera;

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

    static Material sharedGradientMaterial;
    void Awake()
    {
        SetupGradientImage();
        button = cardVisual.GetComponent<Button>();
        originalScale = cardVisual.transform.localScale;
        originalPosition = cardVisual.transform.localPosition;
        positionOffset = cardVisual.transform.localPosition + positionOffset;
        // Add click listener
        button.onClick.AddListener(OnButtonClick);
        camera=Camera.main;
        CardImageBackground.color=UnselectedColor;
    }

    void Update()
    {
        if (isHovered) HandleCardHoverState();
        else HandleCardNormalState();
    }

    void HandleCardHoverState()
    {
       // cardVisual.transform.localPosition = Vector3.Lerp(cardVisual.transform.localPosition, positionOffset, positionOffsetSpeed * Time.deltaTime);
       // cardVisual.transform.localScale = Vector3.Lerp(cardVisual.transform.localScale, originalScale * scaleMultiplier, scaleSpeed * Time.deltaTime);
        RotateTowardsMouse();
    }

    void HandleCardNormalState()
    {
        // Return to original scale smoothly
        cardVisual.transform.localScale = Vector3.Lerp(cardVisual.transform.localScale, originalScale, scaleDuration * Time.deltaTime);
       cardVisual.transform.localPosition = Vector3.Lerp(cardVisual.transform.localPosition, originalPosition, positionOffsetSpeed * Time.deltaTime);

        // Reset rotation smoothly
        cardVisual.transform.localRotation = Quaternion.Lerp(
            cardVisual.transform.localRotation,
            Quaternion.identity,
            Time.deltaTime * 8f
        );
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
        if (Vector3.Distance(cardVisual.transform.localPosition, originalPosition) < 0.1f)
        {
            isHovered = true;
            Tween.LocalPosition(cardVisual.transform, positionOffset, .5f, Ease.OutBack);
            Tween.PunchScale(cardVisual.transform, originalScale * scaleMultiplier, scaleDuration, 3, true, Ease.InOutBack);
            AudioManager.instance.Play("UI_CardHoverEnter");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {   

        isHovered = false;
       // Tween.LocalPosition(cardVisual.transform, originalPosition, .3f, Ease.Default);
       // Tween.Scale(cardVisual.transform, Vector3.one, 0.3f, Ease.Default);
    }

    public AgentCharacter GetAgent()
    {
        return agent;
    }

    private void OnButtonClick()
    {
        UIHandler.SetCurrentAgentFromCard(agent, this);
        AudioManager.instance.Play("UI_CardClickEnter");
        CardImageBackground.color=SelectedColor;
        Tween.ShakeLocalRotation(cardVisual.transform, strength: new Vector3(0, 0, 10), duration: 0.3f, frequency: 15);
    }
    void RotateTowardsMouse()
    {   
        // Get mouse position relative to the card
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            cardVisual.transform.parent as RectTransform,
            Input.mousePosition,
            camera,
            out Vector2 localPoint);

        // Normalize to [-1, 1] range based on card size
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
    void SetupGradientImage()
    {
        if (sharedGradientMaterial == null)
        {
            sharedGradientMaterial = new Material(Shader.Find("UI/Default"));
            sharedGradientMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
            sharedGradientMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            sharedGradientMaterial.SetInt("_ZWrite", 0);
            sharedGradientMaterial.DisableKeyword("_ALPHATEST_ON");
            sharedGradientMaterial.EnableKeyword("_ALPHABLEND_ON");
            sharedGradientMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            sharedGradientMaterial.renderQueue = 3000;
        }

        gradientImage.material = sharedGradientMaterial;
    }
    private void OnDestroy()
    {
        // Clean up event listeners
        if (button != null)
            button.onClick.RemoveListener(OnButtonClick);
        // Clean up material to prevent memory leaks
        //if (gradientImage != null && gradientImage.material != null)
        //{
        //    if (Application.isEditor)
        //        DestroyImmediate(gradientImage.material);
        //    else
        //        Destroy(gradientImage.material);
        //}
    }
}
