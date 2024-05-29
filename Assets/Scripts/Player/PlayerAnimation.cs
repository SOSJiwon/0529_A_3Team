using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public PlayerAnimation Instance;
    public Animator animator;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        animator = GetComponent<Animator>();
    }

}
