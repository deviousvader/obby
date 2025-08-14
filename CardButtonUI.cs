using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardButtonUI : MonoBehaviour, IPointerClickHandler
{
    public int slotIndex = 0; // which hand index (0..handSize-1)
    public bool isPlayer = true;
    public Button button;
    public Text costText;
    public Text nameText;
    public Image cooldownOverlay;

    private CardData boundCard;

    void Start()
    {
        button.onClick.AddListener(OnClick);
        Refresh();
    }

    void Update()
    {
        Refresh();
    }

    public void BindCard(CardData card)
    {
        boundCard = card;
        Refresh();
    }

    public void Refresh()
    {
        if (boundCard == null)
        {
            nameText.text = "Empty";
            costText.text = "-";
            button.interactable = false;
            if (cooldownOverlay) cooldownOverlay.fillAmount = 1f;
            return;
        }
        nameText.text = boundCard.cardName;
        costText.text = boundCard.cost.ToString();
        button.interactable = GameManager.I.elixir >= boundCard.cost;
        if (cooldownOverlay) cooldownOverlay.fillAmount = 0f;
    }

    public void OnClick()
    {
        if (boundCard == null) return;
        if (!GameManager.I.TrySpendElixir(boundCard.cost)) return;

        // Simple placement: spawn into left lane if clicked quickly; for better UX use drag-drop and raycasts.
        GameManager.I.SpawnUnit(boundCard, isPlayer, Random.value > 0.5f);
        // remove from player hand
        if (isPlayer) GameManager.I.playerHand.Remove(boundCard);
        // refresh
    }
}
