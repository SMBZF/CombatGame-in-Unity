using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public MeeleFighter fighter;

    public void Init(MeeleFighter fighter)
    {
        this.fighter = fighter; 
    }

    public MeeleFighter GetFighter()
    {
        return fighter;
    }
}
