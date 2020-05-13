﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : BaseState
{    private bool isMovingLeft = false;
    private PlayerUtils.Direction m_dir;
    private PlayerUtils.Direction m_swipe;

    private bool swipeOn = false;

    public PlayerJump( GameObject controllable, PlayerUtils.Direction dir) : base( controllable ) {
        isMovingLeft = dir == PlayerUtils.Direction.Left;
        velocity.y = PlayerUtils.PlayerJumpForce;
        name = "Jump";
    }


    private void checkIfShouldBeOver(){

        if( m_detector.isOnCelling()){
            velocity = new Vector2();
            m_isOver = true;
        }

//        if( m_detector.isOnGround() || m_detector.isOnCelling() ){
//            velocity = new Vector2();
//            m_isOver   = true;
//        }

        if( PlayerFallHelper.FallRequirementsMeet( m_detector.isOnGround()) && velocity.y < 0 ){ 
            m_isOver = true;
        }

    }

    public override void Process(){
        
        checkIfShouldBeOver();

        velocity.y += -PlayerUtils.GravityForce * Time.deltaTime;
        if( swipeOn ){
            velocity.x = ( m_swipe == PlayerUtils.Direction.Left ) ? 
                            -PlayerUtils.PlayerSpeedInAir : 
                             PlayerUtils.PlayerSpeedInAir;

            // if velocity.x > 0 => m_direction = Direction.Left
            // else velocity.x < 0 => m_direction = Direction.Right czy jakoś tak.
        }
        m_detector.Move(velocity * Time.deltaTime);
    }

    public override void HandleInput(){

        if( PlayerUtils.isMoveLeftKeyHold() ){
            swipeOn = true;
            m_swipe = PlayerUtils.Direction.Left;
        }else if( PlayerUtils.isMoveRightKeyHold() ){
            swipeOn = true;
            m_swipe = PlayerUtils.Direction.Right;
        }else{
            swipeOn = false;
        }
    }
}
