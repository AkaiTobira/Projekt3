﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatHurt : PlayerBaseState{    
    private float timeToEnd;
    private AnimationTransition m_transition;
    private float velocitXFriction = 0.0f;

    private int knocBackDirection = 0;

    private bool isFaceLocked = false;
    private GlobalUtils.Direction savedDir;

    public CatHurt( GameObject controllable, GlobalUtils.AttackInfo infoPack) : base( controllable ){
        name = "CatHurt";

        savedDir     = m_FloorDetector.GetCurrentDirection();
        isFaceLocked = infoPack.lockFaceDirectionDuringKnockback;
        if( !isFaceLocked ) m_dir = infoPack.fromCameAttack;
        knocBackDirection = (int)infoPack.fromCameAttack;

        fillKnockbackInfo( infoPack );
    }


 //   protected override void UpdateDirection(){
//
  //  }

    private void fillKnockbackInfo( GlobalUtils.AttackInfo infoPack ){
        CommonValues.PlayerVelocity          = infoPack.knockBackValue * 1.75f;
        CommonValues.PlayerVelocity.x        *= (int)infoPack.fromCameAttack;
        velocitXFriction  = infoPack.knockBackFrictionX;

   ///     if(Mathf.Abs( CommonValues.PlayerVelocity.x ) > infoPack.knockBackValue.x ) 
 //           velocity.x = (int)infoPack.fromCameAttack * Mathf.Abs( CommonValues.PlayerVelocity.x );
 

        if( velocitXFriction > 0){
            m_FloorDetector.CheatMove( new Vector2(0,40.0f));
        }
    }


    protected override void UpdateDirection(){
        if( !isFaceLocked ) base.UpdateDirection();
    }

    protected override void SetUpAnimation(){
        m_animator.SetTrigger( "CatHurt" );
        timeToEnd = getAnimationLenght("CatHurt");

        m_transition = m_controllabledObject.
               GetComponent<Player>().animationNode.
               GetComponent<AnimationTransition>();
    }

    private void  ProcessStateEnd(){
        timeToEnd -= Time.deltaTime;
        if( timeToEnd < 0){
            m_isOver = true;
            m_animator.ResetTrigger( "CatHurt" );

            if( isFaceLocked ) {
                CommonValues.PlayerVelocity = new Vector2();
                m_FloorDetector.Move( new Vector2( 0.001f, 0) * (int)savedDir);
            }

            if( !m_FloorDetector.isOnGround() ){
                m_nextState = new CatFall( m_controllabledObject, m_dir);
            }

        }
    }

    private void ProcessMove(){
        m_animator.SetFloat( "FallVelocity", velocity.y);
        m_animator.SetBool("isGrounded", m_FloorDetector.isOnGround());
        PlayerFallHelper.FallRequirementsMeet( true );

        CommonValues.PlayerVelocity.y += -CatUtils.GravityForce * Time.deltaTime;

        if( knocBackDirection == -1 ) {
            CommonValues.PlayerVelocity.x = CommonValues.PlayerVelocity.x - velocitXFriction * Time.deltaTime;
        }else{
            CommonValues.PlayerVelocity.x = CommonValues.PlayerVelocity.x + velocitXFriction * Time.deltaTime;
        }

        m_FloorDetector.Move(CommonValues.PlayerVelocity*Time.deltaTime);
    }

    public override void Process(){
        ProcessStateEnd();
        ProcessMove();
    }

    public override void HandleInput(){}
}