using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplatter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GGJ2022.EnemyAI.PlayerState.OnDamaged += TakeDamage;
    }

    void TakeDamage(int _incomingDamage)
    {
        if(this.enabled)
        {
            Deactivate();
        }
        gameObject.SetActive(true);
        Invoke("Deactivate", 3);
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
