using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private Team _team1;
    private Team _team2;
    [SerializeField] private Camera _mainCam;
    private PlayerManager _playerManager;
    private bool _isMatchStarted = false;
    private float _matchTimer;

    [SerializeField] private Canvas _startScreen;
    [SerializeField] private Canvas _endMatchScreen;

    // Start is called before the first frame update
    void Start()
    {
        var teams = GameObject.FindObjectsOfType<Team>();
        if (teams.Length != 2)
        {
            Debug.LogError("You should have exactly 2 teams on scene");
            return;
        }
        _team1 = teams[0];
        _team2 = teams[1];
        if (_mainCam == null)
            _mainCam = Camera.main;
        _playerManager = GetComponent<PlayerManager>();
        _startScreen.enabled = true;
        _endMatchScreen.enabled = false;
    }
    
    void Update()
    {
        if (_isMatchStarted && IsMatchOver())
            EndMatch();
    }

    public void StartMatch()
    {
        _startScreen.enabled = false;
        _endMatchScreen.enabled = false;
        Vector3 leftDown = _mainCam.ViewportToWorldPoint(new Vector3(0, 0, _mainCam.nearClipPlane));
        Vector3 rightUp = _mainCam.ViewportToWorldPoint(new Vector3(1, 1, _mainCam.nearClipPlane));
        _team1.StartMatch(new Vector2(leftDown.x, leftDown.y), new Vector2((leftDown.x + rightUp.x) / 2, rightUp.y));
        _team2.StartMatch(new Vector2((leftDown.x + rightUp.x) / 2, leftDown.y), new Vector2(rightUp.x, rightUp.y));
        _playerManager.StartMatch();
        _isMatchStarted = true;
        _matchTimer = Time.time;
    }

    public void EndMatch()
    {
        _endMatchScreen.enabled = true;
        var matchDuration = Time.time - _matchTimer;
        matchDuration = Mathf.Round(matchDuration * 100.0f) / 100.0f;
        var team = _team1.AlivePlayers.Count > 0 ? _team1 : _team2;
        var matchStat = _endMatchScreen.transform.Find("MatchStats").GetComponent<Text>();
        matchStat.text = $"Match ended in {matchDuration} seconds\n {team.gameObject.name} wins!";
        _isMatchStarted = false;

    }

    private bool IsMatchOver()
    {
        return _team1.AlivePlayers.Count == 0 || _team2.AlivePlayers.Count == 0;
    }

}
