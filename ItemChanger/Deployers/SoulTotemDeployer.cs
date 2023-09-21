using ItemChanger.Internal;

namespace ItemChanger.Deployers
{
    /// <summary>
    /// Deployer for placing a simple soul totem.
    /// <br/>Use SoulTotemContainer instead to create a soul totem which can handle items.
    /// </summary>
    public record SoulTotemDeployer : Deployer
    {
        public SoulTotemSubtype SoulTotemSubtype { get; init; } = SoulTotemSubtype.PathOfPain;

        public override GameObject Instantiate()
        {
            SoulTotemSubtype e = SoulTotemSubtype;
            return ObjectCache.SoulTotem(ref e);
        }

        public virtual bool Equals(SoulTotemDeployer other) => ReferenceEquals(this, other) ||
            (base.Equals(other) && this.SoulTotemSubtype == other.SoulTotemSubtype);

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), SoulTotemSubtype.GetHashCode());
    }
}
