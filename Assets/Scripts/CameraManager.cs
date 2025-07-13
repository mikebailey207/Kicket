using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    // pretty clear what this does
    public static CameraManager Instance;
    [SerializeField]
    CinemachineVirtualCamera bowlingCam;
    [SerializeField]
    CinemachineVirtualCamera ballCam;
    [SerializeField]
    CinemachineVirtualCamera fieldCam;

    private void Awake()
    {
       Instance = this;
    }
    public void CutToBallCam()
    {
        ballCam.Priority = 10;
        bowlingCam.Priority = 0;
        fieldCam.Priority = 0;
    }
    public void CutToBowlCam()
    {
        ballCam.Priority = 0;
        bowlingCam.Priority = 10;
        fieldCam.Priority = 0;
    }
    public void CutToFieldCam()
    {
        ballCam.Priority = 0;
        bowlingCam.Priority = 0;
        fieldCam.Priority = 10;
    }
}
