using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Animator animator;
    PlayerControl playerControl;
    public float speedCloseNormal = 1.2f;
    public float speedCloseForced = 0.2f;


    // Start is called before the first frame update
    void Start()
    {
        playerControl = FindObjectOfType<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("ObjectFlyInside", playerControl.interiorObjectWillFly);
        //animator.SetFloat("DoorCloseSpeed", playerControl.interiorObjectWillFly? speedCloseNormal : speedCloseForced);
    }
}
