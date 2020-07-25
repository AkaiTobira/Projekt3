﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalactitController :IEntity
{

    new CollisionDetector m_FloorDetector;
    private Vector3 animationVel = new Vector3();
    public float m_smoothTime = 0.01f;


    void Start() {
        m_FloorDetector = transform.Find("Detector").GetComponent<CollisionDetector>();
        m_animator      = transform.Find("Animator").GetComponent<Animator>();
    }

    bool hasBeenHit = false;

    public override void OnHit(GlobalUtils.AttackInfo infoPack){
        if( !infoPack.isValid ) return;
        if( infoPack.stateName.Contains("2")){ 
            hasBeenHit = true;
            m_FloorDetector.Move( new Vector2(0, 500f));
            m_animator.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public float damage = 3;

    public override GlobalUtils.AttackInfo GetAttackInfo(){
        GlobalUtils.AttackInfo infoPack = new GlobalUtils.AttackInfo();
        infoPack.isValid = true;

        if( Vector3.Distance(GlobalUtils.PlayerObject.position, m_FloorDetector.transform.position) < 150 ){ 
            infoPack.attackDamage = damage;
        }else{
            infoPack.attackDamage = 10000;
        }

        return infoPack;
    }

    public override string GetCurrentState(){
        if ( hasBeenHit ) return "Falling";
        return "Waiting";
    }

    protected virtual void UpdateAnimatorPosition(){
        m_animator.transform.position = 
            Vector3.SmoothDamp( m_animator.transform.position, 
                                m_FloorDetector.GetComponent<Transform>().position, 
                                ref animationVel, 
                                m_smoothTime);
    }

    private float Gravity = 0;
    public float GravityForce = 100;
    private bool onFloor = false;
    void Update()
    {   

        UpdateAnimatorPosition();
        UpdateMoveDown();
        UpdateBreak();

    }

    void UpdateMoveDown(){
        if( onFloor ) return;
        if( hasBeenHit && !m_FloorDetector.isOnGround() ){
            Gravity += GravityForce * Time.deltaTime;
            m_FloorDetector.Move( new Vector2(0, Gravity )* Time.deltaTime);
        }else{
            Gravity = 0;
        }
    }
    void UpdateBreak(){
        if( onFloor ) return;
        if( hasBeenHit && m_FloorDetector.isOnGround()){
 
            m_FloorDetector.enabled = false;
            m_FloorDetector.GetComponent<BoxCollider2D>().enabled = false;
            m_animator.SetTrigger("OnDestroy");
            Destroy( gameObject, getAnimationLenght( "StalactitBreak") );
            onFloor = true;
        }
    }

    protected float getAnimationLenght(string animationName){
        RuntimeAnimatorController ac = m_animator.runtimeAnimatorController;   
        for (int i = 0; i < ac.animationClips.Length; i++){
            if (ac.animationClips[i].name == animationName)
                return ac.animationClips[i].length;
        }
        return 0.0f;
    }


}