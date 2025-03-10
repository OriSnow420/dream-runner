using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class MainMenu : Panel
{

    [SerializeField] public TextMeshProUGUI nameText = null;
    [SerializeField] private Button logoutButton = null;
    [SerializeField] private Button gameButton = null;
    [SerializeField] private Button leaderboardsButton = null;
    [SerializeField] private Button renameButton = null;
    [SerializeField] private Button quitButton = null;
    [SerializeField] private Button helpButton = null;
    [SerializeField] private string whichScene;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        logoutButton.onClick.AddListener(SignOut);
        gameButton.onClick.AddListener(StartGame);
        leaderboardsButton.onClick.AddListener(Leaderboards);
        renameButton.onClick.AddListener(RenamePlayer);
        quitButton.onClick.AddListener(Quit);
        helpButton.onClick.AddListener(Help);
        base.Initialize();
    }
    
    public override void Open()
    {
        UpdatePlayerNameUI();
        base.Open();
    }
    
    private void SignOut()
    {
        MenuManager.Singleton.SignOut();
    }

    private void Quit()
    {
        Application.Quit();
    }

    private void Help()
    {
        PanelManager.Open("help");
    }
    
    private void UpdatePlayerNameUI()
    {
        nameText.text = AuthenticationService.Instance.PlayerName;
    }

    private void StartGame()
    {
        SceneManager.LoadScene(whichScene);
    }
    
    private void Leaderboards()
    {
        PanelManager.Open("leaderboards");
    }

    private void RenamePlayer()
    {
        GetInputMenu panel = (GetInputMenu)PanelManager.GetSingleton("input");
        panel.Open(RenamePlayerConfirm, GetInputMenu.Type.String, 14, "Enter a new name:", "Send", "Cancel");
        // // 添加以下代码 
        // if (Application.platform == RuntimePlatform.WebGLPlayer) 
        // { 
        //     // 获取面板中的InputField组件 
        //     InputField inputField = panel.GetComponentInChildren<InputField>(); 
        //     if (inputField != null) 
        //     { 
        //         // 激活输入框并聚焦 
        //         inputField.ActivateInputField(); inputField.Select(); 
        //         // 对于移动端网页，需要设置这个属性来显示虚拟键盘 
        //         inputField.shouldHideMobileInput = false; 
        //     } 
        // }
    }

    private async void RenamePlayerConfirm(string input)
    {
        renameButton.interactable = false;
        try
        {
            string pattern = @"^[^\s]+\d{2}$";
            if(Regex.IsMatch(input, pattern))
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(input);
                UpdatePlayerNameUI();
            }
            else
            {
                ErrorMenu panel = (ErrorMenu)PanelManager.GetSingleton("error");
                panel.Open(ErrorMenu.Action.None, "Fail to change the name. The name must be in the format of \"name\" + \"class\" without space and no longer than 14. E.g.\"ZhangSan21\".", "OK");
            }
        }
        catch
        {
            ErrorMenu panel = (ErrorMenu)PanelManager.GetSingleton("error");
            panel.Open(ErrorMenu.Action.None, "Fail to change the name. The name must be in the format of \"name\" + \"class\" without space and no longer than 14. E.g.\"ZhangSan21\".", "OK");
        }
        renameButton.interactable = true;
    }
}