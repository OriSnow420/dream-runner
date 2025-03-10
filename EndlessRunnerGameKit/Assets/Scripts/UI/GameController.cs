using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Leaderboards;
using System.Threading.Tasks;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject deathMenu;
    [SerializeField] private GameObject victoryMenu;
    [SerializeField] private GameObject timeoutMenu;
    [SerializeField] private GameObject audioTrigger;
    private bool flag = false;

    // Update is called once per frame
    void Update()
    {
        if(NewPlayer.Instance.firstLanded)
        {
            audioTrigger.SetActive(true);
        }

        if(NewPlayer.Instance.health <= 0 && NewPlayer.Instance.currentTime > 0 && NewPlayer.Instance.coins < NewPlayer.Instance.max_coins)
        {
            NewPlayer.Instance.frozen = true;
            deathMenu.SetActive(true);
        }

        if(NewPlayer.Instance.coins == NewPlayer.Instance.max_coins && NewPlayer.Instance.currentTime > 0 && !flag)
        {   
            StartCoroutine(Victory());
            flag = true;
        }

        if(NewPlayer.Instance.currentTime <= 0 && NewPlayer.Instance.coins < NewPlayer.Instance.max_coins)
        {
            NewPlayer.Instance.runRightSpeed = 0;
            timeoutMenu.SetActive(true);
        }
    }

    public IEnumerator Victory()
    {
        NewPlayer.Instance.runRightSpeed = 0;
        NewPlayer.Instance.stopTime = true;
        victoryMenu.SetActive(true);
        float newTime = NewPlayer.Instance.startTime - NewPlayer.Instance.currentTime;
        int newScore = CalculateScore(newTime);
        AddScoreAsync(0);
        
        yield return new WaitForSecondsRealtime(0.3f);
        
        // 使用 StartCoroutine 来等待异步操作
        yield return StartCoroutine(GetPlayerScoreAndUpdate(newScore, newTime));
    }
    
    private IEnumerator GetPlayerScoreAndUpdate(int newScore, float newTime)
    {
        var playerscoreTask = LeaderboardsService.Instance.GetPlayerScoreAsync("dreamrunner2025");
        
        // 等待异步操作完成
        while (!playerscoreTask.IsCompleted)
        {
            yield return null; // 每帧等待
        }
    
        // 获取玩家分数
        var playerscore = playerscoreTask.Result;
        int oldScore = (int)(playerscore.Score);
        if (newScore > oldScore)
        {
            AddScoreAsync(newScore - oldScore);
            oldScore = newScore;
        }
    
        GameObject.Find("VictoryMenu/Victory").GetComponent<Text>().text = "Time: " + newTime.ToString("F2") + "s";
        GameObject.Find("VictoryMenu/ScoreShow").GetComponent<Text>().text = "Score: " + newScore.ToString();
        GameObject.Find("VictoryMenu/ScoreShowHistory").GetComponent<Text>().text = "Record: " + oldScore.ToString(); 
    }

    public async void AddScoreAsync(int score)
    {
        try
        {
            var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync("dreamrunner2025", score);
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
    }

    public int CalculateScore(float time)
    {
        return 30000 - Mathf.FloorToInt(100 * time);
    }

}
