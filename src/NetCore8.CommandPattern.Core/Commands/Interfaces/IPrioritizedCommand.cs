
namespace NetCore8.CommandPattern.Core.Commands.Interfaces
{
    public enum CommandPriority { Low, Normal, High, Critical }
    
    public interface IPrioritizedCommand 
    {
        CommandPriority Priority { get; }
    }
}