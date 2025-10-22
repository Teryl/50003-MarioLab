using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/SetupInvincibility")]
public class InvincibleAction : Action
{
    public AudioClip invincibilityStart;

    public override void Act(StateController controller)
    {
        MarioStateController m = (MarioStateController)controller;
        AudioSource audioSource = m.gameObject.GetComponent<AudioSource>();
        if (audioSource != null && invincibilityStart != null)
        {
            audioSource.PlayOneShot(invincibilityStart);
        }

        m.SetRendererToFlicker();
    }
}