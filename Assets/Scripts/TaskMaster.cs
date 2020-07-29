﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskMaster : MonoBehaviour
{

    public Vector2 lastCheckPoint = new Vector2();
    public Transform PlayerPrefab;

    public int triggeredEnemies = 0;

    public bool IsPlayerInCombat() { return triggeredEnemies != 0 ; }

    public void EnemyTriggered(){
    //    triggeredEnemies += 1;
    }

    public void EnemyIsOutOfCombat(){
        triggeredEnemies = Mathf.Max(0, triggeredEnemies - 1);
    }

    void Start()
    {
        GlobalUtils.TaskMaster = GetComponent<TaskMaster>();
        if( GlobalUtils.PlayerObject ) lastCheckPoint = GlobalUtils.PlayerObject.transform.position;
    }


    public void SetPlayerAtLastCheckpoint(){
        GlobalUtils.PlayerObject.position = lastCheckPoint;
        GlobalUtils.PlayerObject.GetComponent<Player>().ResetPlayer();
        
        //var newPlayer = Instantiate( PlayerPrefab, (Vector3)lastCheckPoint, Quaternion.identity );
        //GlobalUtils.Camera.SetNewFollowable( newPlayer );
        //Destroy(GlobalUtils.PlayerObject.gameObject, 1.0f);
        //GlobalUtils.PlayerObject = newPlayer;
    }

    public void UpdateCheckpoint( Vector2 checkpointPosition){
        lastCheckPoint = checkpointPosition;
    }

}
