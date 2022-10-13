using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Animator animator;
    PlayerControl playerControl;
    public float timeHoldOpen = 0.5f;
    float timer = 0;


    // Start is called before the first frame update
    void Start()
    {
        playerControl = FindObjectOfType<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        if (playerControl.interiorObjectWillFly && timer <= 0)
        {
            timer = timeHoldOpen;
        }
        
        animator.SetBool("ObjectFlyInside", playerControl.interiorObjectWillFly || timer > 0);
    }
}
