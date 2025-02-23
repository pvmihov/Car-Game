using Dan.Main;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndMenuManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI congrMess, timeGiver;
    int level;
    int score;
    private string[] publicLeaderboardKeys = { "088931034f3c1259426dc47478f3d19b797ebc9c75f4ed86e866e3769d2e1c73" , "ba4d3f71fd450c4885e72b29ba2b9fa57cf917378ac5c4fe71b6a8f59b91d14f" };

    [SerializeField] private TextMeshProUGUI[] lNames;
    [SerializeField] private TextMeshProUGUI[] lScores;
    [SerializeField] private TextMeshProUGUI LeadName;
    [SerializeField] private TextMeshProUGUI textButton;
    bool veche = false;
    [SerializeField] private GameObject butonche;
    [SerializeField] private TMP_InputField inputche;
    public Image img;
    public Sprite[] sprites;

    private void Start() {
        AudioManager.Instance.StopLoopingSound("Drifting");
        img.sprite = sprites[PlayerPrefs.GetInt("skin")];
        level = PlayerPrefs.GetInt("level");
        score = PlayerPrefs.GetInt("score");
        string textche = score.ToString();
        timeGiver.text = "Your score was " + textche;
        if (level == 1 && PlayerPrefs.GetInt("10") == 0 && score > 10000) { congrMess.text = "Congratulations! You unlocked Gold skin!"; PlayerPrefs.SetInt("10", 1); }
        if (level == 2 && PlayerPrefs.GetInt("11") == 0 && score > 10000) { congrMess.text = "Congratulations! You unlocked LTF skin!"; PlayerPrefs.SetInt("11", 1); }
        UpdateLeaderBoard();

    }

    void UpdateLeaderBoard() {
        for (int q = 0; q < lNames.Length; q++) {
            lNames[q].text = "-:-";
            lScores[q].text = "-:-";
        }

        LeadName.text = "Loading";

        LeaderboardCreator.GetLeaderboard(publicLeaderboardKeys[level - 1], ((msg) => {
            if (msg == null || msg.Length == 0) {
                LeadName.text = "Can't Load";
            }
            for (int q = 0; q < lNames.Length; q++) {
                if (q >= msg.Length) {
                    lNames[q].text = "-:-";
                    lScores[q].text = "-:-";
                } else {
                    lNames[q].text = msg[q].Username;
                    int score = msg[q].Score;
                 
                    string vreme = score.ToString();
                    lScores[q].text = vreme;
                }
            }
            LeadName.text = "Map " + (level).ToString() + " Leaderboard";
        }));
    }

    public void Submit() {
        string name = textButton.text;
        if (name.Length == 0 || name.Length > 10) {
            return;
        }
        if (veche) return;
        veche = true;
        int score1 = score;
        for (int q = 0; q < lNames.Length; q++) {
            lNames[q].text = "-:-";
            lScores[q].text = "-:-";
        }

        LeadName.text = "Loading";
        LeaderboardCreator.UploadNewEntry(publicLeaderboardKeys[level - 1], name, score1, ((msg) => {  
            
            UpdateLeaderBoard();
        }));
        //butonche.SetActive(false);
    }

    private void Update() {
        if (Input.GetKeyDown("o") && !inputche.isFocused) { SceneManager.LoadScene(0); }
        if (Input.GetKeyDown("p") && !inputche.isFocused) { SceneManager.LoadScene(level); }
        if (Input.GetKeyDown(KeyCode.Return)) Submit();
    }

    public void Back() {
        SceneManager.LoadScene(0);
    }
}
