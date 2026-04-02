using UnityEngine;
using Unity.Cinemachine;
using Fusion;

public class AutoAssignCamera : NetworkBehaviour
{
    public Transform cameraTarget;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            var vcam = FindAnyObjectByType<CinemachineCamera>();
            if (vcam != null)
            {
                vcam.Target.TrackingTarget = cameraTarget;
            }
        }
    }
}