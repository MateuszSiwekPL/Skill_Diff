using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBehaviour : MonoBehaviour, IKillable
{
  public void Kill()
  {
      Debug.Log("I Was Killed");
  }
}
