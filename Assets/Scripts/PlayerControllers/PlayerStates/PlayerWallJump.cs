﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJump : BaseState
{
    private bool isMovingLeft = false;
    private PlayerUtils.Direction m_dir;

    private bool isJumpIncreasing = true;
    private float jumpTime = 0.0f;

    private enum Swipe{ Left, None, Right };

    private Swipe m_swipe;

    public PlayerWallJump( GameObject controllable, PlayerUtils.Direction dir) : base( controllable ) {
        // play change direction animation;
        // at end of animation call :
        // TEMP
        controllable.transform.GetComponent<Player>().changeDirection(dir);
        isMovingLeft = dir == PlayerUtils.Direction.Left;
        m_dir = dir;
    }

    private float ProcessSwipe( float curr_velocity ){
        if( m_swipe == Swipe.None ) return curr_velocity;
        if( m_swipe == Swipe.Left ){
                m_controllabledObject.transform.GetComponent<Player>().changeDirection(PlayerUtils.Direction.Left);
                return PlayerUtils.PlayerSpeedInAir * (float)PlayerUtils.Direction.Left;
            }
        if( m_swipe == Swipe.Right ){
                m_controllabledObject.transform.GetComponent<Player>().changeDirection(PlayerUtils.Direction.Right);
                return PlayerUtils.PlayerSpeedInAir * (float)PlayerUtils.Direction.Right;
            }
        return curr_velocity;
    }

    public override void Process(){
        Rigidbody2D m_rb      = m_controllabledObject.GetComponent<Rigidbody2D>();
        Vector2 curr_velocity = m_rb.velocity;
        curr_velocity.x = ProcessSwipe(curr_velocity.x);

        if( isJumpIncreasing ){
            curr_velocity.y       = PlayerUtils.PlayerJumpForce;
        }
/*        }else if( m_controllabledObject.GetComponent<Player>().isOnGrounded() ){
            m_isOver = true;
        }else if( m_controllabledObject.GetComponent<Player>().isHittingWall()){
            m_isOver = true;
        }
*/
        //m_rb.velocity         = curr_velocity;
        m_rb.AddForce( curr_velocity  * 10);
    }

    public override void HandleInput(){
        if( Input.GetKey(KeyCode.Space) && jumpTime < PlayerUtils.JumpMaxTime ){
            jumpTime += Time.deltaTime;
        }else{
            isJumpIncreasing = false;
        }

        if( Input.GetKey(KeyCode.A)      ) { m_swipe = Swipe.Left;  }
        else if( Input.GetKey(KeyCode.D) ) { m_swipe = Swipe.Right; }
        else { m_swipe = Swipe.None; }
    }

}