using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    private static CinemachineVirtualCamera virtualCamera;
    private static float shakeTimer;
    private static float shakeIntensity;

    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public static void ShakeCamera(float intensity, float duration)
    {
        CinemachineBasicMultiChannelPerlin noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        noise.m_AmplitudeGain = intensity;
        shakeIntensity = intensity;
        shakeTimer = duration;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            if (shakeTimer <= 0f)
            {
                CinemachineBasicMultiChannelPerlin noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                noise.m_AmplitudeGain = 0f; // Reset the amplitude when the shake is over
            }
        }
    }
}
