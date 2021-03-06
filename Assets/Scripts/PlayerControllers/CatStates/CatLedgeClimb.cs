﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatLedgeClimb : PlayerLedgeClimb
{

    public CatLedgeClimb( GameObject controllable, GlobalUtils.Direction dir) : base( controllable, dir, 15f ) {
        name = "CatLedgeClimb";
        distanceToFixAnimation = new Vector3( (isLeftOriented())? -20 : 20, -3.5f , 0);
    }

    protected override void  SetUpAnimation(){
        m_animator.SetTrigger("CatClimb");
        timeToEnd = getAnimationLenght("CatLedgeClimb");
        maxOfAnimationToEnd = timeToEnd;
        
        m_transition = m_controllabledObject.
                       GetComponent<Player>().animationNode.
                       GetComponent<AnimationTransition>();
    }
}
