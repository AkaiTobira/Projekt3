﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLedgeClimb : PlayerBaseState
{

    protected float timeToEnd;    
    protected AnimationTransition m_transition;
    protected Vector2 targetClimbHightPoint   = new Vector2();
    protected Vector2 targetClimbStayPoint    = new Vector2();
    protected Vector2 targetClimbTargetPoint  = new Vector2();
    protected GlobalUtils.Direction forSureDirection;

    protected Vector2 differenceForCameraMove = new Vector2();

    protected float maxOfAnimationToEnd;

    public PlayerLedgeClimb( GameObject controllable, GlobalUtils.Direction dir , float someVariable) : base( controllable ) {
        CommonValues.PlayerVelocity = new Vector2(0,0);
        m_animator.SetBool("LedgeOver", false);
        SetUpVariables(someVariable);

        SetUpDirection();
        SetUpRotation();
        
        PlayerFallOfWallHelper.ResetCounter();

        GUIElements.Camera.EnableMoreSmooth(0.4f);
        GUIElements.Camera.DisableMoreSmooth();
    }

    private Vector2 CalculateHighOfLedge(BoxCollider2D ledgeBox){

        Transform interactable = m_ObjectInteractionDetector.GetClimbableObject();
        Vector2[] Vectors = new Vector2[4];

        Vectors[0] = interactable.TransformPoint(ledgeBox.offset + new Vector2(-ledgeBox.size.x,  ledgeBox.size.y) * 0.5f);
        Vectors[1] = interactable.TransformPoint(ledgeBox.offset + new Vector2( ledgeBox.size.x,  ledgeBox.size.y) * 0.5f);
        Vectors[2] = interactable.TransformPoint(ledgeBox.offset + new Vector2(-ledgeBox.size.x, -ledgeBox.size.y) * 0.5f);
        Vectors[3] = interactable.TransformPoint(ledgeBox.offset + new Vector2( ledgeBox.size.x, -ledgeBox.size.y) * 0.5f);

        Vector2 closestOne  = new Vector2(-10000000,-10000000);
        Vector2 closestOne2 = new Vector2(-10000000,-10000000);

        foreach( Vector2 v in Vectors){
            if( Vector2.Distance( m_FloorDetector.GetComponent<Transform>().position, v) < 
                Vector2.Distance( m_FloorDetector.GetComponent<Transform>().position, closestOne) ){
                    closestOne2 = closestOne;
                    closestOne  = v;
            }else if(
                Vector2.Distance( m_FloorDetector.GetComponent<Transform>().position, v) < 
                Vector2.Distance( m_FloorDetector.GetComponent<Transform>().position, closestOne2) ){
                    closestOne2 = v;
            }
        }

        Vector2 toReturn = new Vector2();
        toReturn.y       = closestOne.y > closestOne2.y ? closestOne.y : closestOne2.y;

        if(  Mathf.Abs( closestOne.x - m_FloorDetector.GetComponent<Transform>().position.x) < Mathf.Abs( closestOne2.x - m_FloorDetector.GetComponent<Transform>().position.x) ){
            toReturn.x = closestOne.x;
        }else{
            toReturn.x = closestOne2.x;
        }

        return toReturn;
    }

    private void SetUpDirection(){
        float ledgeBoxX  = m_ObjectInteractionDetector.GetClimbableObject().GetComponent<Transform>().position.x;
        float playerBoxX = m_FloorDetector.GetComponent<Transform>().position.x;

        if( ledgeBoxX < playerBoxX) m_dir = GlobalUtils.Direction.Left;
        if( ledgeBoxX > playerBoxX) m_dir = GlobalUtils.Direction.Right;
    }

    private void SetUpRotation(){
        rotationAngle = isLeftOriented() ? 180 :0 ; 
        m_controllabledObject.GetComponent<Player>().animationNode.eulerAngles = new Vector3( 0, rotationAngle, slopeAngle);
    }

    int dummycounter = 0 ;
    private void SetUpVariables( float shiftValue ){ //TODO Calculation of target position will require slight update;
        
        BoxCollider2D ledgeBox  = m_ObjectInteractionDetector.GetClimbableObject().GetComponent<BoxCollider2D>();
        BoxCollider2D playerBox = m_FloorDetector.GetComponent<BoxCollider2D>();
        Vector2 pos             = m_FloorDetector.GetComponent<Transform>().position;
        GlobalUtils.Direction obstacleDir = ( playerBox.bounds.max.x < ledgeBox.bounds.max.x ) ? 
                                                    GlobalUtils.Direction.Left : 
                                                    GlobalUtils.Direction.Right;



        targetClimbHightPoint = CalculateHighOfLedge(ledgeBox);
        targetClimbStayPoint.y = targetClimbHightPoint.y;// + 20;
        targetClimbStayPoint.x = targetClimbHightPoint.x + ( playerBox.bounds.extents.x ) * (float) obstacleDir ;

        targetClimbTargetPoint = targetClimbHightPoint + new Vector2( - (float) obstacleDir * shiftValue , playerBox.bounds.extents.y );

        Vector2 velocityS = new Vector2( targetClimbStayPoint.x - pos.x, targetClimbHightPoint.y - pos.y);
        m_FloorDetector.CheatMove( velocityS );

        differenceForCameraMove = targetClimbTargetPoint - targetClimbStayPoint;
    }

    protected float shiftValue;

    protected override void UpdateAnimatorPosition(){
    
        GUIElements.Camera.SetAdditionalModification( differenceForCameraMove * (maxOfAnimationToEnd - timeToEnd)/maxOfAnimationToEnd );
        base.UpdateAnimatorPosition();
    }

    public override void OnExit(){
        GUIElements.Camera.SetAdditionalModification(new Vector3());
    }

    public override void Process(){

        Vector2 pos     = m_FloorDetector.GetComponent<Transform>().position;

        DebugDrawHelper.DrawX( targetClimbTargetPoint, new Color( 0, 1, 0));
        DebugDrawHelper.DrawX( targetClimbStayPoint,   new Color( 1, 1, 0));
        DebugDrawHelper.DrawX( targetClimbHightPoint );

        timeToEnd -= Time.deltaTime;

        m_controllabledObject.GetComponent<Player>().animationNode.position = 
                                m_controllabledObject.transform.position + distanceToFixAnimation;

        if( !m_animator.GetCurrentAnimatorStateInfo(0).IsName("BiesIdle") &&  ! m_animator.GetCurrentAnimatorStateInfo(0).IsName("CatIdle") ){
            

        if( timeToEnd < 0 ){// && ! m_animator.GetCurrentAnimatorStateInfo(0).IsName("BiesLedgeClimb")){
            PlayerFallHelper.FallRequirementsMeet( true );
            PlayerFallHelper.lockTime = 0.2f;
            PlayerAnimatorLock.lockTime = 0.2f;

            m_isOver = true;

            m_FloorDetector.GetComponent<Transform>().position = targetClimbTargetPoint - new Vector2(0, 10f);// + (Vector3)shiftValue;

            m_controllabledObject.GetComponent<Player>().animationNode.position = m_controllabledObject.transform.position + new Vector3(0, 20f);

            rotationAngle = isLeftOriented() ? 0 :180 ; 
            m_controllabledObject.GetComponent<Player>().animationNode.eulerAngles = new Vector3( 0, rotationAngle, slopeAngle);

            m_FloorDetector.Move( new Vector2( 0, -0.01f ) );

        }

        }else{
            m_FloorDetector.GetComponent<Transform>().position = targetClimbStayPoint;
        }

    }

    public override void HandleInput(){}


    public override string GetTutorialAdvice(){
        return "";
    }

    public override string GetCombatAdvice(){
        return "";
    }
}
