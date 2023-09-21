using ItemChanger.Components;

namespace ItemChanger.Deployers
{
    /// <summary>
    /// Deployer which creates a HintBox, a region which displays a dream dialogue message when the hero enters.
    /// </summary>
    public record HintBoxDeployer : Deployer
    {
        public float Width { get; init; } = 5f;
        public float Height { get; init; } = 5f;
        public IString Text { get; init; }


        public override GameObject Instantiate()
        {
            HintBox box = HintBox.Create(new Vector2(X, Y), new Vector2(Width, Height));
            box.GetDisplayText = Text.GetValue;
            return box.gameObject;
        }

        public virtual bool Equals(HintBoxDeployer other) => ReferenceEquals(this, other) ||
            (base.Equals(other) && this.Width == other.Width
            && this.Height == other.Height && ReferenceEquals(this.Text, other.Text));

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Width.GetHashCode(),
            Height.GetHashCode(), Text?.GetHashCode());
    }
}
