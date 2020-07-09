﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BiesDead : PlayerBaseState{    

    private float timeToEnd;
    private AnimationTransition m_transition;
    private float velocitXFriction = 0.0f;

    public BiesDead( GameObject controllable, GlobalUtils.AttackInfo infoPack) : base( controllable ){
        name = "BiesDead";
    }

    protected override void  SetUpAnimation(){
        m_animator.SetTrigger( "BiesDead" );
        timeToEnd = getAnimationLenght("BiesDead") + 3f;

        m_transition = m_controllabledObject.
                       GetComponent<Player>().animationNode.
                       GetComponent<AnimationTransition>();
    }

    private void fillKnockbackInfo( GlobalUtils.AttackInfo infoPack ){
        velocity          = infoPack.knockBackValue;
        velocity.x        *= (int)infoPack.fromCameAttack;
        velocitXFriction  = infoPack.knockBackFrictionX * (int)infoPack.fromCameAttack;
    }

    private void  ProcessStateEnd(){
        timeToEnd -= Time.deltaTime;
        if( timeToEnd < 0){
            //TODO ... reload form chekcpoint
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void ProcessMove(){
        PlayerFallHelper.FallRequirementsMeet( true );
        velocity.y += -CatUtils.GravityForce * Time.deltaTime;
        velocity.x = Mathf.Max( velocity.x - velocitXFriction, 0 );
        m_FloorDetector.Move(velocity*Time.deltaTime);
    }

    public override void Process(){
        ProcessStateEnd();
        ProcessMove();
    }
    public override void HandleInput(){}
}