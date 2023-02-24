namespace TimelineControl
{
    public enum TimelinePlayerError
    {
        None,
        AssetIsNull,
        PlayerDirectorMissing,
    }

    public enum TimePlayerState
    {
        None,
        Error,
        Idle,
        Playing,
    }
}