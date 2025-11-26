using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public enum PlayerStates
    {
        Idle,
        Moving,
        Attacking,
        Dodging,
        Blocking,
        Parrying,
        Stunned,
        Dead
    }

    public enum CombatState
    {
        Normal,
        Hurt,
        Dead
    }
}
