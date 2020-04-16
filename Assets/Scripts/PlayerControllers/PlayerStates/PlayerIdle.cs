﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdle : BaseState
{
    public PlayerIdle( GameObject controllable) : base( controllable ) {}

    public override void HandleInput(){
        if( Input.GetKey(KeyCode.A) ){
            m_nextState = new PlayerMoveGround( m_controllabledObject, PlayerUtils.Direction.Left );
        }else if( Input.GetKey(KeyCode.D) ){
            m_nextState = new PlayerMoveGround( m_controllabledObject, PlayerUtils.Direction.Right );
        }else if( Input.GetKey(KeyCode.Space) && m_controllabledObject.GetComponent<Player>().isOnGrounded()){
            m_nextState = new PlayerJump( m_controllabledObject, PlayerUtils.Direction.Left );  } ;
    }

}