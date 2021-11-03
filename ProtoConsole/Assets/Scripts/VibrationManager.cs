using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using nn.hid;

public sealed class VibrationManager 
{
    public bool enableVibrations = true;

    List<VibrationDeviceHandle> vibrationDevices = new List<VibrationDeviceHandle>();
    VibrationDeviceInfo info;

    private static VibrationManager instance = null;
    public static void MakeNewSingleton()
    {
        if (instance == null)
        {
            instance = new VibrationManager();
        }
    }
    public static VibrationManager GetSingleton()
    {
        return instance;
    }

    private VibrationManager() { }

    public void InitJoycons()
    {
        // Npads should be initialized when this function is executed.
        NpadId[] npadIds = { NpadId.No1, NpadId.No2, NpadId.No3, NpadId.No4, NpadId.No5, NpadId.No6, NpadId.No7, NpadId.No8 };

        // Get the vibration device handle of all NpadIds.
        foreach (NpadId id in npadIds)
        {
            NpadStyle currentStyle = Npad.GetStyleSet(id);

            // If player is invalid
            if (currentStyle == NpadStyle.None)
                continue;

            // Add the Device corresponding to the player (single joycon)
            VibrationDeviceHandle[] vibrationDevicesLeft = new VibrationDeviceHandle[1];
            Vibration.GetDeviceHandles(vibrationDevicesLeft, 1, id, currentStyle);
            vibrationDevices.Add(vibrationDevicesLeft[0]);
            
        }

        // Initialize all vibration devices.
        for (int i = 0; i < vibrationDevices.Count; i++)
        {
            Vibration.InitializeDevice(vibrationDevices[i]);
        }
    }

    void VibrateForAllDuring1Frame(VibrationValue vibration)
    {
        if (!enableVibrations) 
            return;

        // Set the left vibration device to vibrate at an amplitude of 0.5 and a frequency of 160 Hz.
        for (int i = 0; i < vibrationDevices.Count; i++)
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
        if ((!enableVibrations) || vibrationDeviceID < 0)
            return;

        Vibration.SendValue(vibrationDevices[vibrationDeviceID], vibration);
    }

    void StopAllVibrations()
    {
        // Set all vibration devices to stop vibrating.
        VibrationValue v0 = VibrationValue.Make();
        for (int i = 0; i < vibrationDevices.Count; i++)
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
