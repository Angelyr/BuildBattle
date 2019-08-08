﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Character
{
    int enterTurnDistance = 7;

    protected void EnterTurnOrder()
    {
        if (WorldController.GetDistanceFromPlayer(MyPosition()) < enterTurnDistance)
        {
            TurnOrder.AddTurn(gameObject);
        }
        else TurnOrder.AddConcurrentTurn(gameObject);
    }

    public override bool StartConcurrentTurn()
    {
        if (PlayerWithInRange(enterTurnDistance))
        {
            TurnOrder.AddTurn(gameObject);
            player.GetComponent<PlayerUI>().SetMessage("Enemy In Range");
            StartCoroutine(player.GetComponent<PlayerController>().myCamera.PointCamera(transform));
            return false;
        }
        PathToPlayer();
        return true;
    }

    protected bool PathToPlayer()
    {
        Vector2Int closest = WorldController.GetClosestTileToPlayer(MyPosition());
        if (closest == MyPosition()) return false;
        MoveTo(closest);
        return true;
    }

    protected virtual string Attack()
    {
        if (PlayerWithInRange(1) == false) return "fail";
        ConsumeAP();
        if (PlayerWithInRange(1)) player.GetComponent<PlayerController>().Attacked();
        return "success";
    }

    protected void MoveRandomly()
    {
        int xMove = Random.Range(-1, 2);
        int yMove = Random.Range(-1, 2);
        Move(xMove, yMove);
    }

    protected void Move(int xMove, int yMove)
    {
        ConsumeAP();
        WorldController.MoveWorldLocation(transform, xMove, yMove);
    }

    protected void MoveTo(Vector2Int target)
    {
        ConsumeAP();
        WorldController.MoveToWorldPoint(transform, target);
    }

    protected void ConsumeAP()
    {
        if (ap < 1)
        {
            TurnOrder.EndTurn(gameObject);
            return;
        }
        ap -= 1;
    }

    protected bool PathToDistance(int distance)
    {
        if (WorldController.GetDistanceFromPlayer(MyPosition()) > distance) PathToPlayer();
        else return false;

        return true;
    }

    protected bool LineUp(Vector2Int target)
    {
        int distX = Mathf.Abs(MyPosition().x - target.x);
        int distY = Mathf.Abs(MyPosition().y - target.y);

        if (distX < distY/2 && MyPosition().x > target.x) Move(-1, 0);
        else if (distX < distY/2 && MyPosition().x < target.x) Move(1, 0);
        else if (distX/2 > distY && MyPosition().y > target.y) Move(0, -1);
        else if (distX/2 > distY && MyPosition().y < target.y) Move(0, 1);
        else return false;

        return true;
    }

    protected Vector2Int FindTarget()
    {
        return PlayerPosition();
    }

    protected bool RunAway()
    {
        Vector2Int farthest = WorldController.FarthestTileFromPlayer(MyPosition());
        if (farthest == MyPosition()) return false;
        MoveTo(farthest);
        return true;
    }

    public override void Attacked()
    {
        health -= 1;
        SetHealthUI();
        if(health == 0) WorldController.Kill(gameObject);
    }

    protected abstract override void Init();
}
