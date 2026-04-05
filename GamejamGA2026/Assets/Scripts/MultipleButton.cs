using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MultipleButton : GameButton
{
    [SerializeField]
    private GameObject WaitModel;

    [SerializeField]
    private List<MultipleButton> otherButtons;

    public void Start()
    {
        WaitModel.SetActive(false);
    }

    public override void Entered()
    {
        if (!active)
        {
            pressAudioSrc.PlayOneShot(pressAudioSrc.clip);
        }

        active = true;

        OffModel.SetActive(false);
        WaitModel.SetActive(true);
        OnModel.SetActive(false);

        if (otherButtons.Where(b => !b.active).Count() == 0)
        {
            OffModel.SetActive(false);
            WaitModel.SetActive(false);
            OnModel.SetActive(true);

            foreach (MultipleButton button in otherButtons)
            {
                button.OffModel.SetActive(false);
                button.WaitModel.SetActive(false);
                button.OnModel.SetActive(true);
            }
        }
    }

    public override void Exited()
    {
        if (otherButtons.Where(b => !b.active).Count() != 0)
        {
            active = false;

            unpressAudioSrc.PlayOneShot(unpressAudioSrc.clip);

            OffModel.SetActive(true);
            WaitModel.SetActive(false);
            OnModel.SetActive(false);
        }
    }
}
