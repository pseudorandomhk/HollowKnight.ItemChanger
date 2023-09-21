using ItemChanger.Internal;

namespace ItemChanger
{
    /// <summary>
    /// A Deployer which creates a platform at the specified point.
    /// </summary>
    public record SmallPlatform : Deployer
    {
        public override GameObject Instantiate()
        {
            return ObjectCache.SmallPlatform;
        }

        public virtual bool Equals(SmallPlatform other) => ReferenceEquals(this, other) || base.Equals(other);

        public override int GetHashCode() => base.GetHashCode();
    }
}
