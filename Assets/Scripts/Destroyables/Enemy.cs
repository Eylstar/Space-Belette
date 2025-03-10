using UnityEngine;

public class Enemy : Destroyable
{
    protected override void Start()
    {
        base.Start();
        collisionType = CollidableType.Enemy;
        collisionFilter.Add(CollidableType.PlayerBullet);
    }
}
