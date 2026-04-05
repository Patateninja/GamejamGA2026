using UnityEngine;

public class HoldButton : GameButton
{
    public override void Entered()
    {
        active = true;

        OffModel.SetActive(false);
        OnModel.SetActive(true);

        pressAudioSrc.PlayOneShot(pressAudioSrc.clip);
    }

    public override void Exited()
    {
        active = false;

        unpressAudioSrc.PlayOneShot(unpressAudioSrc.clip);

        OffModel.SetActive(true);
        OnModel.SetActive(false);
    }
}