﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : Entity
{
    protected int ap;
    protected int maxAP;
    protected int health;
    protected int maxHealth;
    public GameObject myTurnUI;

    protected Vector2Int targetPosition;
    protected Vector2Int mapPosition;
    protected bool moving = false;

    protected override void Awake()
    {
        base.Awake();
        Init();
        maxAP = ap;
        maxHealth = health;
        targetPosition = MyPosition();
        mapPosition = MyPosition();
    }

    private void FixedUpdate()
    {
        MoveAnimation();
    }

    protected virtual void Update()
    {
        GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(-transform.position.y) + 1;
    }

    private void MoveAnimation()
    {
        if(WorldController.GetTile(targetPosition) != null)
        {
            targetPosition = MyPosition();
            moving = false;
            return;
        }
        if (targetPosition != (Vector2)transform.position) moving = true;
        if (targetPosition == (Vector2)transform.position && moving)
        {
            Move();
            moving = false;
        }
        
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, Settings.moveSpeed);
    }

    protected void SetDirection(Vector2Int direction)
    {
        targetPosition = MyPosition() + direction;
    }

    public void SetPosition(Vector2Int position)
    {
        targetPosition = position;
    }

    protected virtual void Move()
    {
        WorldController.MoveToWorldPoint(transform, mapPosition, targetPosition);
        mapPosition = targetPosition;
        ChangeAP(-1);
    }

    public virtual void ChangeAP(int change)
    {
        ap += change;
    }

    public int Health()
    {
        return health;
    }

    public int MaxHealth()
    {
        return maxHealth;
    }

    public virtual void StartTurn()
    {
        player.GetComponent<PlayerController>().myCamera.SetPosition(transform);
        ap = maxAP;
    }
    public abstract bool StartConcurrentTurn();

    protected abstract void Init();
}
