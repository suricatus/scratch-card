using System.Collections.Generic;
using Tools.PrizeManager.Models;
using Tools.PrizeManager.Services;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace MainMenu
{
    public class PrizeEditorPanelUI : MonoBehaviour
    {
        [Header("Panel References")]
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Transform prizeItemsContainer;
        [SerializeField] private PrizeItemEditorUI prizeItemPrefab;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button saveButton;

        private IPrizeManagerService _prizeManagerService;
        private readonly List<PrizeItemEditorUI> _prizeItems = new List<PrizeItemEditorUI>();
        private bool _isPanelOpen = false;

        [Inject]
        public void Construct(IPrizeManagerService prizeService)
        {
            _prizeManagerService = prizeService;
        }

        private void Awake()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseClicked);
            }

            if (saveButton != null)
            {
                saveButton.onClick.AddListener(OnSaveClicked);
            }

            if (panelRoot != null)
            {
                panelRoot.SetActive(false);
            }
        }

        private void OnEnable()
        {
            PrizeEvents.OnPrizeAwarded += OnPrizeAwarded;
            PrizeEvents.OnPrizeReset += OnPrizeReset;
        }

        private void OnDisable()
        {
            PrizeEvents.OnPrizeAwarded -= OnPrizeAwarded;
            PrizeEvents.OnPrizeReset -= OnPrizeReset;
        }

        public void OpenPanel()
        {
            if (panelRoot != null)
            {
                panelRoot.SetActive(true);
            }

            _isPanelOpen = true;
            RefreshPrizeItems();
        }

        public void ClosePanel()
        {
            if (panelRoot != null)
            {
                panelRoot.SetActive(false);
            }

            _isPanelOpen = false;
            ClearPrizeItems();
        }

        private void OnPrizeAwarded(Prize prize)
        {
            if (_isPanelOpen)
            {
                RefreshPrizeItems();
            }
        }

        private void OnPrizeReset()
        {
            if (_isPanelOpen)
            {
                RefreshPrizeItems();
            }
        }

        private void RefreshPrizeItems()
        {
            ClearPrizeItems();
            PopulatePrizeItems();
        }

        private void PopulatePrizeItems()
        {
            var prizes = _prizeManagerService.GetAllPrizes();

            foreach (var prize in prizes)
            {
                var prizeItem = Instantiate(prizeItemPrefab, prizeItemsContainer);
                prizeItem.Initialize(prize);
                _prizeItems.Add(prizeItem);
            }
        }

        private void ClearPrizeItems()
        {
            foreach (var item in _prizeItems)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }

            _prizeItems.Clear();
        }

        private void OnCloseClicked()
        {
            ClosePanel();
        }

        private void OnSaveClicked()
        {
            foreach (var prizeItem in _prizeItems)
            {
                string prizeId = prizeItem.GetPrizeId();
                int newQuantity = prizeItem.GetCurrentQuantity();
                _prizeManagerService.UpdatePrizeQuantity(prizeId, newQuantity);
            }

            PrizeEvents.TriggerPrizeQuantityUpdated();

            ClosePanel();
        }

        private void OnDestroy()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(OnCloseClicked);
            }

            if (saveButton != null)
            {
                saveButton.onClick.RemoveListener(OnSaveClicked);
            }
        }
    }
}
