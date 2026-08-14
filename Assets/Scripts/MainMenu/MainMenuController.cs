using TMPro;
using Tools.B2B.PlayerRegistration.Models;
using Tools.B2B.PlayerRegistration.Services;
using Tools.PrizeManager.Models;
using Tools.PrizeManager.Services;
using Tools.SoundManager.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Input Fields")]
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private TMP_InputField emailInputField;
        [SerializeField] private TMP_InputField cellphoneInputField;
        
        [Header("Sound Settings")]
        [SerializeField] private AudioClip backgroundMusic;
        [SerializeField] private AudioClip buttonClick;

        [Header("UI Elements")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button soundButton;
        [SerializeField] private TMP_Text errorMessageText;
        [SerializeField] private TMP_Text qtyPrizesText;
        [SerializeField] private Image soundButtonIcon;
        [SerializeField] private Sprite soundOnSprite;
        [SerializeField] private Sprite soundMutedSprite;

        [Header("Keyboard Avoidance")]
        [SerializeField] private KeyboardAvoidance keyboardAvoidance;

        [Header("Scene Settings")]
        [SerializeField] private string gameplaySceneName = "GameplayScene";

        private const string ErrorMessageFillFields = "Por favor, preencha todos os campos.";
        private const string ErrorMessageInvalidEmail = "Por favor, insira um email válido com @.";
        private const string ErrorMessageInvalidPhone = "O telefone deve conter apenas números.";
        private IPlayerRegistrationService _playerRegistrationService;
        private GoogleSheetsService _googleSheetsService;
        
        [Inject] private ISoundManager _soundManager;
        [Inject] private IPrizeManagerService _prizeManagerService;

        private void OnEnable()
        {
            if (_prizeManagerService != null)
            {
                UpdatePrizeDisplay();
            }
        }

        private async void Start()
        {
            SetupGoogleSheetsService();
            SetupPlayerRegistrationService();
            SetupInputFields();
            SetupKeyboardAvoidance();
            SubscribeToPrizeEvents();

            await _soundManager.PlayMusic(backgroundMusic);

            if (startButton != null)
            {
                startButton.onClick.AddListener(OnStartButtonClicked);
            }
            
            if (soundButton != null)
            {
                soundButton.onClick.AddListener(OnSoundButtonClicked);
                UpdateSoundButtonVisual(_soundManager.IsMuted());
            }

            HideErrorMessage();
            UpdatePrizeDisplay();
        }

        private void SubscribeToPrizeEvents()
        {
            PrizeEvents.OnPrizeAwarded += HandlePrizeAwarded;
            PrizeEvents.OnPrizeReset += HandlePrizesReset;
            PrizeEvents.OnPrizeQuantityUpdated += HandlePrizeQuantityUpdated;
        }

        private void UnsubscribeFromPrizeEvents()
        {
            PrizeEvents.OnPrizeAwarded -= HandlePrizeAwarded;
            PrizeEvents.OnPrizeReset -= HandlePrizesReset;
            PrizeEvents.OnPrizeQuantityUpdated -= HandlePrizeQuantityUpdated;
        }
        
        private void HandlePrizeQuantityUpdated()
        {
            UpdatePrizeDisplay();
        }

        private void HandlePrizeAwarded(Prize prize)
        {
            UpdatePrizeDisplay();
        }

        private void HandlePrizesReset()
        {
            UpdatePrizeDisplay();
        }

        private void UpdatePrizeDisplay()
        {
            if (qtyPrizesText != null && _prizeManagerService != null)
            {
                var allPrizes = _prizeManagerService.GetAllPrizes();
                var totalPrizes = 0;
                var totalRemaining = 0;
                
                foreach (var prize in allPrizes)
                {
                    totalPrizes += prize.TotalQuantity;
                    totalRemaining += prize.RemainingQuantity;
                }
                
                qtyPrizesText.text = $"Prêmios Disponíveis: {totalRemaining}/{totalPrizes}";
            }
        }

        private void SetupGoogleSheetsService()
        {
            _googleSheetsService = FindObjectOfType<GoogleSheetsService>();
        
            if (_googleSheetsService == null)
            {
                var serviceObj = new GameObject("GoogleSheetsService");
                _googleSheetsService = serviceObj.AddComponent<GoogleSheetsService>();
                DontDestroyOnLoad(serviceObj);
            }
        }

        private void SetupPlayerRegistrationService()
        {
            _playerRegistrationService = new PlayerRegistrationService();
        }

        public void OnSoundButtonClicked()
        {
            var currentMuteState = _soundManager.IsMuted();
            var newMuteState = !currentMuteState;
    
            _soundManager.Mute(newMuteState);
            _soundManager.SaveSettings();
    
            UpdateSoundButtonVisual(newMuteState);
        }
        
        private void UpdateSoundButtonVisual(bool isMuted)
        {
            if (soundButtonIcon != null)
            {
                soundButtonIcon.sprite = isMuted ? soundMutedSprite : soundOnSprite;
            }
        }

        private void SetupInputFields()
        {
            if (cellphoneInputField != null)
            {
                cellphoneInputField.contentType = TMP_InputField.ContentType.IntegerNumber;
                cellphoneInputField.characterValidation = TMP_InputField.CharacterValidation.Integer;
            }

            if (emailInputField != null)
            {
                emailInputField.contentType = TMP_InputField.ContentType.EmailAddress;
            }
        }

        private void SetupKeyboardAvoidance()
        {
            if (keyboardAvoidance != null)
            {
                if (nameInputField != null)
                    keyboardAvoidance.RegisterInputField(nameInputField);
            
                if (emailInputField != null)
                    keyboardAvoidance.RegisterInputField(emailInputField);
            
                if (cellphoneInputField != null)
                    keyboardAvoidance.RegisterInputField(cellphoneInputField);
            }
        }

        private async void OnStartButtonClicked()
        {
            var playerName = nameInputField.text;
            var playerEmail = emailInputField.text;
            var playerCellphone = cellphoneInputField.text;

            var result = await _playerRegistrationService.RegisterPlayerAsync(
                playerName, 
                playerEmail,
                playerCellphone,
                consent: true);

            if (result.Success)
            {
                HideErrorMessage();
                SavePlayerData(result.PlayerData);
                LoadGameplayScene();
            }
            else
            {
                ShowErrorMessage(result.ErrorMessage);
            }
        }

        private void ShowErrorMessage(string message)
        {
            if (errorMessageText != null)
            {
                errorMessageText.text = message;
                errorMessageText.gameObject.SetActive(true);
            }
        }

        private void HideErrorMessage()
        {
            if (errorMessageText != null)
                errorMessageText.gameObject.SetActive(false);
        }

        private void SavePlayerData(PlayerRegistrationData playerData)
        {
            PlayerPrefs.SetString("PlayerName", playerData.Name);
            PlayerPrefs.SetString("PlayerEmail", playerData.Email);
            PlayerPrefs.SetString("PlayerCellphone", playerData.PhoneNumber);
            PlayerPrefs.Save();
        }

        private void LoadGameplayScene()
        {
            SceneManager.LoadScene(gameplaySceneName);
        }

        private void OnDestroy()
        {
            UnsubscribeFromPrizeEvents();
            
            if (startButton != null)
            {
                startButton.onClick.RemoveListener(OnStartButtonClicked);
            }
            
            if (soundButton != null)
            {
                soundButton.onClick.RemoveListener(OnSoundButtonClicked);
            }
        }
    }
}
