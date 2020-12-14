using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Team _team1;
    private Team _team2;

    [SerializeField] private GameObject LaserPrefab;
    [SerializeField] private Transform LasersPool;
    private List<LineRenderer> lasers = new List<LineRenderer>();

    private bool _isMatchInProcess = false;
    
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

        var damageColliders = GameObject.FindObjectsOfType<DamageCollider>();
        foreach (var collider in damageColliders)
        {
            collider.TriggerEnter = this.TriggerEnter;
            collider.TriggerStay = this.TriggerStay;
            collider.TriggerExit = this.TriggerExit;
        }

    }

    public void StartMatch()
    {
        AddLineRenderers(_team1, _team2, _team1.LaserColor, 0);
        AddLineRenderers(_team2, _team1, _team2.LaserColor, _team1.AlivePlayers.Count * _team2.AlivePlayers.Count );
        _isMatchInProcess = true;
    }
    
    void FixedUpdate()
    {
        if (_isMatchInProcess)
        {
            CheckDeadSystem(_team1, _team2);
            CheckDeadSystem(_team2, _team1);

            AssignTargets(_team1, _team2);
            AssignTargets(_team2, _team1);
        }


    }

    private void TriggerEnter(Collider2D attackerCollider, Collider2D damagedCollider)
    {
        var attacker = attackerCollider.GetComponentInParent<Player>();
        if (!_isMatchInProcess || !attacker.IsAlive())
            return;
        var damaged = damagedCollider.GetComponentInParent<Player>();
        if (PlayerInWhichTeam(attacker) != PlayerInWhichTeam(damaged))
        {
            attacker.DrawShoot(damaged);
            damaged.ReceiveDamage(0);
        }
    }

    private void TriggerStay(Collider2D attackerCollider, Collider2D damagedCollider)
    {
        var attacker = attackerCollider.GetComponentInParent<Player>();
        if (!_isMatchInProcess || !attacker.IsAlive())
            return;
        var damaged = damagedCollider.GetComponentInParent<Player>();
        if (PlayerInWhichTeam(attacker) != PlayerInWhichTeam(damaged))
        {
            attacker.DrawShoot(damaged);
            var damage = Time.fixedDeltaTime * attacker.DamagePerSecond;
            damaged.ReceiveDamage(damage);
        }
    }

    private void TriggerExit(Collider2D attackerCollider, Collider2D damagedCollider)
    {
        var attacker = attackerCollider.GetComponentInParent<Player>();
        if (!_isMatchInProcess || !attacker.IsAlive())
            return;
        var damaged = damagedCollider.GetComponentInParent<Player>();
        if (PlayerInWhichTeam(attacker) != PlayerInWhichTeam(damaged))
        {
            attacker.UndrawShoot(damaged);
            var damage = Time.fixedDeltaTime * attacker.DamagePerSecond;
            damaged.ReceiveDamage(damage);
        }
    }

    private Team PlayerInWhichTeam(Player player)
    {
        return _team1.IsInTeam(player) ? _team1 : _team2;
    }
    

    private void CheckDeadSystem(Team team, Team enemyTeam)
    {
        for (var i = team.AlivePlayers.Count - 1; i >= 0; --i)
        {
            if (!team.AlivePlayers[i].IsAlive())
            {
                var deadPlayer = team.AlivePlayers[i];
                deadPlayer.Die();
                team.DeadPlayers.Add(deadPlayer);
                foreach (var enemy in enemyTeam.AlivePlayers)
                    enemy.UndrawShoot(deadPlayer);
                foreach (var enemy in enemyTeam.DeadPlayers)
                    enemy.UndrawShoot(deadPlayer);
                team.AlivePlayers.RemoveAt(i);
            }
        }
    }

    private void AssignTargets(Team team, Team enemyTeam)
    {
        foreach (var player in team.AlivePlayers)
            if (player.CurrentTargetToFollow == null || !player.CurrentTargetToFollow.IsAlive())
                player.CurrentTargetToFollow = GetNearestEnemy(player, enemyTeam);
    }

    private Player GetNearestEnemy(Player player, Team enemyTeam)
    {
        Player nearestEnemy = null;
        var minSqrDistance = System.Single.MaxValue;
        foreach (var enemy in enemyTeam.AlivePlayers)
        {
            var sqrDistance = ((Vector2)enemy.transform.position - (Vector2)player.transform.position).sqrMagnitude;
            if (sqrDistance < minSqrDistance)
            {
                nearestEnemy = enemy;
                minSqrDistance = sqrDistance;
            }
        }
        return nearestEnemy;
    }

    private void AddLineRenderers(Team team, Team enemyTeam, Color color, int index)
    {
        foreach (var player in team.AlivePlayers)
            foreach (var enemy in enemyTeam.AlivePlayers)
            {
                if (index >= lasers.Count)
                {
                    var lineRenderer = Instantiate(LaserPrefab, LasersPool).GetComponent<LineRenderer>();
                    lasers.Add(lineRenderer);
                    lineRenderer.startWidth = 0.05f;
                    lineRenderer.endWidth = 0.3f;
                    
                }
                
                player.AssignLineRenderer(enemy, lasers[index]);
                lasers[index].startColor = color;
                lasers[index].endColor = color;
                index++;
            }
               
    }

    public void Stop()
    {
        _isMatchInProcess = false;
    }

    public void Continue()
    {
        _isMatchInProcess = true;
    }
}
