using System.Collections.Generic;
using UnityEngine;

public interface ICollidable
{
    CollidableType GetCollidableType();
    List<CollidableType> GetFilter();
    
    GameObject GetGameObject();
}

public enum CollidableType
{
    Enemy,
    Obstacle,
    Player,
    PlayerBullet,
    EnemyBullet
}