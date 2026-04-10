using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    public static ChatUI Instance;

    [Header("UI References")]
    public GameObject chatPanel;
    public TMP_InputField messageInput;
    public TMP_InputField targetPlayerInput; 
    public TextMeshProUGUI chatDisplay;
    public ScrollRect scrollRect;
    
    [Header("Color Settings")]
    public Color publicColor = Color.white;
    public Color privateColor = Color.yellow;
    public Color systemColor = Color.gray;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        if (chatDisplay != null) chatDisplay.text = "";

        if (chatPanel != null)
        {
            chatPanel.SetActive(false); // Tắt chat ngay khi vừa vào game để không bị treo lơ lửng

            Button sendBtn = chatPanel.GetComponentInChildren<Button>();
            if (sendBtn != null)
            {
                sendBtn.onClick.RemoveAllListeners();
                sendBtn.onClick.AddListener(OnSendButtonClicked);
            }
        }
        
        // Mặc định khóa chuột khi chơi
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleChat();
        }

        if (chatPanel != null && chatPanel.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            OnSendButtonClicked();
        }
    }

    public void ToggleChat()
    {
        if (chatPanel == null) return;

        bool isActive = !chatPanel.activeSelf;
        chatPanel.SetActive(isActive);

        if (isActive)
        {
            if (messageInput != null) messageInput.ActivateInputField();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Tắt camera chạy theo chuột khi chat đang bật
        var cinemachineInput = Object.FindAnyObjectByType<Unity.Cinemachine.CinemachineInputAxisController>();
        if (cinemachineInput != null)
        {
            cinemachineInput.enabled = !isActive;
        }
    }

    public void OnSendButtonClicked()
    {
        if (messageInput == null || ChatManager.Instance == null) return;

        string text = messageInput.text.Trim();
        if (string.IsNullOrEmpty(text)) return;

        string target = targetPlayerInput != null ? targetPlayerInput.text.Trim() : "";

        if (string.IsNullOrEmpty(target))
        {
            ChatManager.Instance.SendPublicMessage(text);
        }
        else
        {
            ChatManager.Instance.SendPrivateMessage(target, text);
            OnMessageReceived(ChatManager.Instance.LocalPlayerName, target + " << " + text, true);
        }

        messageInput.text = "";
        messageInput.ActivateInputField(); 
    }

    public void OnMessageReceived(string sender, string message, bool isPrivate)
    {
        if (chatDisplay == null) return;

        string colorHex = ColorUtility.ToHtmlStringRGB(publicColor);
        string prefix = "[Public]";

        if (sender == "System")
        {
            colorHex = ColorUtility.ToHtmlStringRGB(systemColor);
            prefix = "[System]";
        }
        else if (isPrivate)
        {
            colorHex = ColorUtility.ToHtmlStringRGB(privateColor);
            prefix = "[Private]";
        }

        string formattedMsg = string.Format("<color=#{0}>{1} <b>{2}</b>: {3}</color>\n", colorHex, prefix, sender, message);
        chatDisplay.text += formattedMsg;

        Canvas.ForceUpdateCanvases();
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}
