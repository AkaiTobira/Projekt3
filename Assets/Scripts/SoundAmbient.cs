﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundAmbient : MonoBehaviour {


     public static SoundAmbient instance;

     [FMODUnity.EventRef]
     public string[] Sounds;
     public List<FMOD.Studio.EventInstance> soundevents = new List<FMOD.Studio.EventInstance>();

     void Awake() {
          instance = GetComponent<SoundAmbient>();

          foreach( string sound in Sounds){
               soundevents.Add( FMODUnity.RuntimeManager.CreateInstance (sound) );
          }

          PlayAmbient( 0 );
     }


     void Start() {
          PlayAmbient( 0 );
     }

     public void PlayAmbient( int index ){

          FMODUnity.RuntimeManager.AttachInstanceToGameObject(soundevents[index], GetComponent<Transform>(), GetComponent<Rigidbody2D>());

          FMOD.Studio.PLAYBACK_STATE fmodPbState;
          soundevents[index].getPlaybackState(out fmodPbState);
          if (fmodPbState != FMOD.Studio.PLAYBACK_STATE.PLAYING) {
              soundevents[index].start ();
          }
     }


     void StopSoud(int index){

          FMOD.Studio.PLAYBACK_STATE fmodPbState;
          soundevents[index].getPlaybackState(out fmodPbState);
          if (fmodPbState == FMOD.Studio.PLAYBACK_STATE.PLAYING) {
              soundevents[index].stop (FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
          }
     }

}