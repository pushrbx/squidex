namespace Squidex.Extensibility
{
    public interface ISquidexPlugin
    {
        string Name { get; }

        string TargetSchema { get; }

        string TargetAppName { get; }

        IContentOperation Operation { get; }
    }
}
