///-----------------------------------------------------------------
/// Author : Leonard Fetis
/// Date : 26/01/2021 10:01
///-----------------------------------------------------------------
using System;
using UnityEngine;


   abstract public class StateObjects : MonoBehaviour
    {
        public Action doAction;

        protected virtual void DoActionVoid() { }
        protected virtual void DoActionNormal() { }

        protected virtual  void SetModeVoid()
        {
            doAction = DoActionVoid;
        }

        protected virtual void SetModeNormal()
        {
            doAction = DoActionNormal;
        }
    }

