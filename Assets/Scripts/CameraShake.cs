using UnityEngine;
using Cinemachine;

/// <summary>
/// Controls camera shake effect using Cinemachine.
/// </summary>
public class CameraShake : MonoBehaviour
{
    private static CinemachineVirtualCamera virtualCamera;
    private static float shakeTimer;
    private static float shakeIntensity;

    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    /// <summary>
    /// Initiates a camera shake effect.
    /// </summary>
    /// <param name="intensity">The intensity of the shake.</param>
    /// <param name="duration">The duration of the shake.</param>
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
