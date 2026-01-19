using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private SoundType soundType;
    [SerializeField, Range(0, 1)] private float volume = 1;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public void PlaySoundInAnimation()
    {
        SoundManager.PlayRandomSoundFromType(soundType, volume);
    }
}
