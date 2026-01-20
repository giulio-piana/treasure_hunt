using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ChestUI : MonoBehaviour
{
    [Header("Visual Elements")]
    [SerializeField] private Button chestButton;
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private Animator chestAnimator;
    [SerializeField] private Image WinImage;

    private int chestIndex;
    private GameManager gameManager;

    private Chest chestData;

    public void Initialize(int index, GameManager manager)
    {
        chestIndex = index;
        gameManager = manager;

        if (chestButton != null)
        {
            chestButton.onClick.AddListener(OnChestClicked);
        }

        chestData = gameManager.GetChestAtIndex(chestIndex);

        chestData.StateMachine.OnStateChanged += UpdateChestVisual;

        UpdateVisual(ChestState.Closed, false);
    }

    public void UpdateChestVisual(Chest chest, ChestState state)
    {
         bool isWinning = state == ChestState.Opened && chest.IsWinning;
         UpdateVisual(state, isWinning);       
    }

    private void OnChestClicked()
    {
        if (gameManager != null)
        {
            gameManager.OnChestSelected(chestIndex);
        }
    }

    public void UpdateVisual(ChestState state, bool isWinning = false)
    {
        if (stateText != null)
        {
            stateText.text = state.ToString();
        }

        if (WinImage != null)
        {
            WinImage.gameObject.SetActive(isWinning);
        }

        if (chestAnimator != null)
            chestAnimator.Play(state.ToString(), -1, 0f);      
        
        if (chestButton != null)
        {
            chestButton.interactable = (state == ChestState.Closed);
        }
    }

    void OnDestroy()
    {
        if (chestButton != null)
        {
            chestButton.onClick.RemoveListener(OnChestClicked);
        }
    }
}
