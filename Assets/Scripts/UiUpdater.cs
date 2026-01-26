using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UiUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI spinsRemainingText;
    [SerializeField] private TextMeshProUGUI chipsText;
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private Button spinButton;

    [SerializeField] private SpinnerController sc;

    private void Update()
    {
        spinsRemainingText.text = "Spins Remaining: " + PlayerStats.spinsRemaining.ToString();
        chipsText.text = PlayerStats.chips.ToString();
        roundText.text = "Round: " + PlayerStats.currentRound.ToString();
        spinButton.gameObject.SetActive(sc.confirmButtonActive);
    }
}
