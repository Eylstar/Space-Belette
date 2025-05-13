using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    public List<Bloc> ShootBlocs = new();
    public List<Bloc> ShipBlocs = new();
    public int MaxLife;
    public int CurrentLife;
}
