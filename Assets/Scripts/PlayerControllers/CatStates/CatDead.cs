﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatDead : BaseState{    
    private bool isMovingLeft = false;
    private float timeToEnd;
    private AnimationTransition m_transition;

    private float velocitXFriction = 0.0f;

    public CatDead( GameObject controllable, GlobalUtils.AttackInfo infoPack) : base( controllable ){
        isMovingLeft = m_detector.GetCurrentDirection() == GlobalUtils.Direction.Left;
        name = "CatDead";
        m_animator.SetTrigger( "CatDead" );
        timeToEnd = getAnimationLenght("CatDead");

        velocity          = infoPack.knockBackValue;
        velocity.x        *= (int)infoPack.fromCameAttack;
        velocitXFriction  = infoPack.knockBackFrictionX * (int)infoPack.fromCameAttack;

        m_transition = m_controllabledObject.
                       GetComponent<Player>().animationNode.
                       GetComponent<AnimationTransition>();
    }

    private float getAnimationLenght(string animationName){
        RuntimeAnimatorController ac = m_animator.runtimeAnimatorController;   
        for (int i = 0; i < ac.animationClips.Length; i++){
            if (ac.animationClips[i].name == animationName)
                return ac.animationClips[i].length;
        }
        return 0.0f;
    }

    private void  ProcessStateEnd(){
        timeToEnd -= Time.deltaTime;
        if( timeToEnd < 0){
 //           m_isOver = true;
            m_animator.ResetTrigger( "CatHurt" );
        }
    }

    private void ProcessMove(){
        PlayerFallHelper.FallRequirementsMeet( true );
        velocity.y += -CatUtils.GravityForce * Time.deltaTime;
        velocity.x = Mathf.Max( velocity.x - velocitXFriction, 0 );
        m_detector.Move(velocity*Time.deltaTime);
    }

    public override void Process(){
        ProcessStateEnd();
        ProcessMove();
    }
    public override void HandleInput(){
    }
}
