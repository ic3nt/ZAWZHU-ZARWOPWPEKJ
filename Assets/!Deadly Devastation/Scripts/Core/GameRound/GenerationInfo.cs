public struct GenerationInfo
{
    public RoundEvents.GenerationStage Stage;
    public int Floor;

    public GenerationInfo(RoundEvents.GenerationStage stage, int floor)
    {
        Stage = stage;
        Floor = floor;
    }
}
