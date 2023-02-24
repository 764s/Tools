namespace TimelineControl
{
    public enum TimelineControllerError
    {
        None,
        AssetIsNull,
        PlayerDirectorMissing,
    }

    public enum TimelineControllerState
    {
        None,
        Error,
        Idle,
        Playing,
    }
}