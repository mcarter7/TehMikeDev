using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerData
{
    public string playerName;
    public float playerRound;
    public float playerScore;

    public PlayerData()
    {

    }

    public PlayerData(string playerName, float playerRound, float playerScore)
    {
        this.playerName = playerName;
        this.playerRound = playerRound;
        this.playerScore = playerScore;
    }

    public string GetFormat()
    {
        return playerName + "~S~" + playerRound + "~S~" + playerScore;
    }
}

public class ScoreBoard : MonoBehaviour {

    public int scoreCount = 9;
    public static List<PlayerData> playerList;

    static ScoreBoard scoreBoard;
    static string separator = "~S~";
    ScoreManager scoreManager;

    // Use this for initialization
    void Start ()
    {
        scoreBoard = this;
        playerList = GetScores();
        scoreManager = GameObject.FindObjectOfType<ScoreManager>();

        if (playerList != null && playerList.Count > 0)
            scoreManager.LoadPlayersScores(playerList);
    }

    public static void SaveScore(string name, float time, float score)
    {
        List<PlayerData> playerData = new List<PlayerData>();
        time = (float)System.Math.Round((double)time, 2);

        for (int i = 0; i < scoreBoard.scoreCount; i++)
        {
            if(PlayerPrefs.HasKey("Score" + i))
            {
                string [] scoreFormat = PlayerPrefs.GetString("Score" + i).Split(new string[] { separator }, System.StringSplitOptions.RemoveEmptyEntries);
                playerData.Add(new PlayerData(scoreFormat[0], float.Parse(scoreFormat[1]), float.Parse(scoreFormat[2])));
            }
            else
            {
                break;
            }
        }

        if(playerData.Count < 1)
        {
            PlayerPrefs.SetString("Score0", name + separator + time + separator + score);
            return;
        }

        playerData.Add(new PlayerData(name, time, score));
        playerData = playerData.OrderByDescending(n => n.playerScore).ToList();

        for(int i = 0; i < scoreBoard.scoreCount; i++)
        {
            if(i >= playerData.Count)
            {
                break;
            }
            PlayerPrefs.SetString("Score" + i, playerData[i].GetFormat());
        }
    }

    public static List<PlayerData> GetScores()
    {
        List<PlayerData> playerData = new List<PlayerData>();

        for(int i = 0; i < scoreBoard.scoreCount; i++)
        {
            if(PlayerPrefs.HasKey("Score" + i))
            {
                string[] scoreFormat = PlayerPrefs.GetString("Score" + i).Split(new string[] { separator }, System.StringSplitOptions.RemoveEmptyEntries);
                playerData.Add(new PlayerData(scoreFormat[0], float.Parse(scoreFormat[1]), float.Parse(scoreFormat[2])));
            }
            else
            {
                break;
            }
        }

        return playerData;
    }
}
