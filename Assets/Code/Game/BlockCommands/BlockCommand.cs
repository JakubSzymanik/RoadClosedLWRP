public class BlockCommand
{
    public CommandType Type { get; private set; }
    public SpecialField CommandingField { get; private set; }

    public BlockCommand(CommandType type, SpecialField commandingField)
    {
        this.Type = type;
        this.CommandingField = commandingField;
    }
}
