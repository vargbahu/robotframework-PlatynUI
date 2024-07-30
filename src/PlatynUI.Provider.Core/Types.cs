namespace PlatynUI.Provider.Core
{
    public struct NodeReference(int[] runtimeId)
    {
        public readonly int[] RuntimeId = runtimeId;

        public override readonly string ToString()
        {
            return $"NodeReference(RuntimeId: {string.Join(", ", RuntimeId)})";
        }

        public static bool operator ==(NodeReference left, NodeReference right)
        {
            return left.RuntimeId.SequenceEqual(right.RuntimeId);
        }

        public static bool operator !=(NodeReference left, NodeReference right)
        {
            return !left.RuntimeId.SequenceEqual(right.RuntimeId);
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is NodeReference reference && this == reference;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(RuntimeId);
        }
    }

    public interface IApplicationInfoAsync
    {
        Task<string> GetTechnology();
        Task<IList<NodeReference>> GetChildren();
    }

    public interface INodeInfoAsync
    {
        Task<bool> IsValid(NodeReference node);

        Task<IList<NodeReference>> GetChildren(NodeReference parent);
        Task<string[]> GetAttributeNames(NodeReference node);
        Task<object?> GetAttributeValue(NodeReference node, string attributeName);
    }
}
