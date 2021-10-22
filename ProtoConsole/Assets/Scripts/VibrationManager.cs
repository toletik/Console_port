using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using nn.hid;

public class VibrationManager 
{
    //static VibrationManager instance;
    //public static VibrationManager Instance { get { return instance == null ? (instance = new VibrationManager()) : instance; } }

    //Vibration
    int vibrationDeviceCount;
    const int VibrationDeviceCountMax = 8;
    VibrationDeviceHandle[] vibrationDevices = new VibrationDeviceHandle[VibrationDeviceCountMax];
    VibrationDeviceInfo info;

    public VibrationManager(NpadId npadId, NpadStyle style)
    {
        // Npads should be initialized when this function is executed

        // Get the vibration device handle of NpadId::No1.
        vibrationDeviceCount = Vibration.GetDeviceHandles(vibrationDevices, VibrationDeviceCountMax, npadId, style);

        // Init devices
        for (int i = 0; i < vibrationDeviceCount; i++)
        {
            // Initialize the vibration devices.
            Vibration.InitializeDevice(vibrationDevices[i]);
        }
    }

    void VibrateForAllDuring1Frame(VibrationValue vibration)

    {
        // Set the left vibration device to vibrate at an amplitude of 0.5 and a frequency of 160 Hz.
        for (int i = 0; i < vibrationDeviceCount; i++)
        {
            Vibration.GetDeviceInfo(ref info, vibrationDevices[i]);
            //if (info.position == VibrationDevicePosition_Left)
            {
                Vibration.SendValue(vibrationDevices[i], vibration);
            }
        }
    }

    void VibrateForOneDuring1Frame(VibrationValue vibration, int vibrationDeviceID)

    {
        Vibration.SendValue(vibrationDevices[vibrationDeviceID], vibration);
    }

    void StopAllVibrations()
    {
        // Set all vibration devices to stop vibrating.
        VibrationValue v0 = VibrationValue.Make();
        for (int i = 0; i < vibrationDeviceCount; i++)
        {
            Vibration.SendValue(vibrationDevices[i], v0);
        }
    }

    void StopOneVibration(int vibrationDeviceID)
    {
        Vibration.SendValue(vibrationDevices[vibrationDeviceID], VibrationValue.Make());
    }

    ///**
    // * Run a vibration for all controllers.
    // * Usage : Coroutine c = StartCoroutine(VibrateForAll(vibration))
    // * To stop the vibration : StopCoroutine(c)
    // * 
    // * VibrationValue good values : 0.40f, 160.0f, 0.55f, 320.0f
    // */
    //public IEnumerator VibrateForAll(VibrationValue vibration)
    //{
    //    for (;;)
    //    {
    //        VibrateForAllDuring1Frame(vibration);
    //        yield return null;
    //    }
    //}

    /**
     * Run a vibration for all controllers for duration time.
     * duration : Duration of the vibration in seconds
     * Usage : Coroutine c = StartCoroutine(VibrateForAllDuringSeconds(vibration, 0.3f))
     * You can also stop the vibration with StopCoroutine(c)
     * 
     * VibrationValue good values : 0.40f, 160.0f, 0.55f, 320.0f
     */
    public IEnumerator VibrateForAllDuringSeconds(VibrationValue vibration, float duration)
    {
        for (float startTime = Time.time; Time.time < startTime + duration;)
        {
            VibrateForAllDuring1Frame(vibration);
            yield return null;
        }

        StopAllVibrations();
    }

    /**
     * Run a vibration for all controllers for duration time.
     * duration : Duration of the vibration in seconds
     * Usage : Coroutine c = StartCoroutine(VibrateForAllDuringSeconds(vibration, 0.3f))
     * You can also stop the vibration with StopCoroutine(c)
     * 
     * VibrationValue good values : 0.40f, 160.0f, 0.55f, 320.0f
     */
    public IEnumerator VibrateForOneDuringSeconds(VibrationValue vibration, int vibrationDeviceID, float duration)
    {
        for (float startTime = Time.time; Time.time < startTime + duration;)
        {
            VibrateForOneDuring1Frame(vibration, vibrationDeviceID);
            yield return null;
        }

        StopOneVibration(vibrationDeviceID);
    }
}
