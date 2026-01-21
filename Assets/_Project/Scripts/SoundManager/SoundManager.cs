using DBD.BaseGame;

public class SoundManager : BaseSoundManager<SoundManager>
{
    public override bool IsMusicOn()
    {
       return true;
    }

    public override bool IsSfxOn()
    {
        return true;
    }

    protected override void UpdateMusic(bool active)
    {
       
    }

    protected override void UpdateSfx(bool active)
    {
       
    }
}