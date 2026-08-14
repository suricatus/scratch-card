using TMPro;
using Tools.PrizeManager.Models;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class PrizeItemEditorUI : MonoBehaviour
    {
        [Header("UI References")] 
        [SerializeField] private Image prizeIconImage;
        [SerializeField] private TextMeshProUGUI prizeNameText;
        [SerializeField] private TextMeshProUGUI quantityText;
        [SerializeField] private Button decreaseButton;
        [SerializeField] private Button increaseButton;

        private Prize _prize;
        private int _displayedQuantity;

        private void Awake()
        {
            if (decreaseButton != null)
                decreaseButton.onClick.AddListener(OnDecreaseClicked);
            
            if (increaseButton != null)
                increaseButton.onClick.AddListener(OnIncreaseClicked);
        }

        public void Initialize(Prize prizeData)
        {
            _prize = prizeData;
            _displayedQuantity = _prize.RemainingQuantity;
            UpdateUI();
        }

        public int GetCurrentQuantity()
        {
            return _displayedQuantity + _prize.AwardedQuantity;
        }

        public string GetPrizeId()
        {
            return _prize.Id;
        }
        
        private void OnDecreaseClicked()
        {
            if (_displayedQuantity > 0)
            {
                _displayedQuantity--;
                UpdateUI();
            }
        }

        private void OnIncreaseClicked()
        {
            _displayedQuantity++;
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_prize == null)
                return;

            if (prizeIconImage != null)
            {
                try
                {
                    if (_prize.PrizeIcon != null)
                    {
                        prizeIconImage.sprite = _prize.PrizeIcon;
                    }
                }
                catch (MissingReferenceException)
                {
                    prizeIconImage.sprite = null;
                }
            }

            if (prizeNameText != null)
                prizeNameText.text = _prize.PrizeName;

            if (quantityText != null)
                quantityText.text = _displayedQuantity.ToString();
            
            if (decreaseButton != null)
                decreaseButton.interactable = _displayedQuantity > 0;
        }

        private void OnDestroy()
        {
            if (decreaseButton != null)
                decreaseButton.onClick.RemoveListener(OnDecreaseClicked);
            
            if (increaseButton != null)
                increaseButton.onClick.RemoveListener(OnIncreaseClicked);
        }
    }
}
