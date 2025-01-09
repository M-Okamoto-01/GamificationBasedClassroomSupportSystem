using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LottiePlugin.UI;
using LottiePlugin;
using UnityEngine.UI;

public class LottieAnimationMaster : MonoBehaviour
{
    private RawImage m_rawImage;
    private AnimatedImage m_lottieAnimation;

    // Start is called before the first frame update
    void Awake()
    {
        // Get the RawImage component
        m_rawImage = GetComponent<RawImage>();
        // Get the LottieAnimation component
        m_lottieAnimation = GetComponent<AnimatedImage>();
    }
    
    public void StartLottieAnimation_Once()
    {
        if (m_lottieAnimation == null || m_rawImage == null)
        {
            Debug.LogError("LottieAnimation component is not found!");
            return;
        }
        // Play the Lottie animation
        m_lottieAnimation.Play();
    }

    public void StopLottieAnimation()
    {
        if (m_lottieAnimation == null || m_rawImage == null)
        {
            Debug.LogError("LottieAnimation component is not found!");
            return;
        }
        // Stop the Lottie animation
        m_lottieAnimation.Stop();
    }


}
