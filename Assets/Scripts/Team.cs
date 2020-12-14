using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    [SerializeField] private List<GameObject> _playerPrefabs;

    private List<Player> _players = new List<Player>();
    public List<Player> AlivePlayers { get; set; } = new List<Player>();
    public List<Player> DeadPlayers { get; set; } = new List<Player>();

    [SerializeField] private Color _laserColor;
    public Color LaserColor { get => _laserColor; set => _laserColor = value; }

    void Awake()
    {
        foreach (var prefab in _playerPrefabs)
            _players.Add(Instantiate(prefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Player>());

    }

    public void StartMatch(Vector2 leftDownCornerForRandomRect, Vector2 rightUpCornerForRandomRect)
    {
        AlivePlayers.Clear();
        foreach (var player in _players)
        {
            player.transform.position = new Vector3(Random.Range(leftDownCornerForRandomRect.x + player.PlayerRadius, rightUpCornerForRandomRect.x - player.PlayerRadius), 
                Random.Range(leftDownCornerForRandomRect.y + player.PlayerRadius, rightUpCornerForRandomRect.y - player.PlayerRadius), transform.position.z);
            player.gameObject.SetActive(true);
            player.Restart();
            AlivePlayers.Add(player);
        }
        DeadPlayers.Clear();
    }

    public bool IsInTeam(Player player)
    {
        return _players.Contains(player);
    }
}
