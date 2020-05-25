﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState
{
    protected GameObject m_controllabledObject;
    protected CollisionDetectorPlayer m_detector;
    protected BaseState  m_nextState = null;
    protected PlayerUtils.Direction m_dir;

    public string name = ""; 
    protected Vector2    velocity = new Vector2(0,0); 

    protected float slopeAngle   = 0.0f;
    protected float rotationAngle = 0;

    public BaseState( GameObject controllableObject ){
        m_controllabledObject = controllableObject;
        m_detector            = controllableObject.GetComponent<CollisionDetectorPlayer>();
    }

    protected bool m_isOver = false;

    public bool isOver(){
        return m_isOver;
    }

    public virtual void HandleInput(){
    }

    public BaseState NextState(){
        BaseState temp = m_nextState;
        m_nextState = null;
        return temp;
    }

    protected Vector3 animationVel = Vector3.zero;
    protected float m_smoothTime = 0.03f;
    public virtual void UpdateDirection(){

        m_controllabledObject.GetComponent<Player>().animationNode.position = 
            Vector3.SmoothDamp( m_controllabledObject.GetComponent<Player>().animationNode.position, 
                                m_controllabledObject.transform.position, ref animationVel, m_smoothTime);

    //    slopeAngle =  (( m_dir == PlayerUtils.Direction.Right) ? 180.0f - m_detector.GetSlopeAngle()  : m_detector.GetSlopeAngle()  );

        if( velocity.x != 0){
            PlayerUtils.Direction c_dir = Mathf.Sign( velocity.x ) == -1 ? 
                                               PlayerUtils.Direction.Left : 
                                               PlayerUtils.Direction.Right;

            if( m_dir == c_dir) return;

            m_dir = c_dir;
            
            rotationAngle = ( m_dir == PlayerUtils.Direction.Left) ? 180 :0 ; 
            m_controllabledObject.GetComponent<Player>().animationNode.eulerAngles = new Vector3( 0, rotationAngle, slopeAngle);
            //m_controllabledObject.transform.GetChild(0).position    = m_controllabledObject.transform.position;
        }
    }

    public PlayerUtils.Direction GetDirection(){
        return m_dir;
    }

    public  virtual void Process()
    {
    }

    public virtual void OnExit(){
        velocity = new Vector2();
    }

}
