﻿using Newtonsoft.Json;

namespace ItemChanger
{
    public interface IBool
    {
        bool Value { get; }
        IBool Clone();
    }

    public class BoxedBool : IBool
    {
        public bool Value { get; set; }

        public BoxedBool(bool Value) => this.Value = Value;

        public IBool Clone() => (IBool)MemberwiseClone();
    }

    public class PDBool : IBool
    {
        public string boolName;

        public PDBool(string boolName) => this.boolName = boolName;

        [JsonIgnore]
        public bool Value => PlayerData.instance.GetBool(boolName);
        public IBool Clone() => (IBool)MemberwiseClone();
    }

    public class SDBool : IBool
    {
        public string id;
        public string sceneName;

        public SDBool(string id, string sceneName)
        {
            this.id = id;
            this.sceneName = sceneName;
        }

        [JsonIgnore]
        public bool Value => SceneData.instance.FindMyState(new PersistentBoolData
        {
            id = id,
            sceneName = sceneName,
        })?.activated ?? false;

        public IBool Clone() => (IBool)MemberwiseClone();
    }

    public class PlacementAllObtainedBool : IBool
    {
        public PlacementAllObtainedBool(string placementName, IBool missingPlacementTest)
        {
            this.placementName = placementName;
            this.missingPlacementTest = missingPlacementTest;
        }

        public string placementName;
        public IBool missingPlacementTest;

        [JsonIgnore]
        public bool Value
        {
            get
            {
                if (placementName != null && Internal.Ref.Settings is Settings s && s.Placements != null && s.Placements.TryGetValue(placementName, out var p) && p != null)
                {
                    return p.AllObtained();
                }
                return missingPlacementTest?.Value ?? true;
            }
        }

        public IBool Clone() => (IBool)MemberwiseClone();
    }

    public class Disjunction : IBool
    {
        public List<IBool> bools = new();
        public Disjunction() { }
        public Disjunction(IEnumerable<IBool> bools)
        {
            this.bools.AddRange(bools);
        }
        public Disjunction(params IBool[] bools)
        {
            this.bools.AddRange(bools);
        }

        [JsonIgnore]
        public bool Value => bools.Any(b => b.Value);

        public IBool Clone()
        {
            return new Disjunction(bools.Select(b => b.Clone()));
        }

        public void OrWith(IBool b) => bools.Add(b);
    }

    public class Conjunction : IBool
    {
        public List<IBool> bools = new();
        public Conjunction() { }
        public Conjunction(IEnumerable<IBool> bools)
        {
            this.bools.AddRange(bools);
        }
        public Conjunction(params IBool[] bools)
        {
            this.bools.AddRange(bools);
        }

        [JsonIgnore]
        public bool Value => bools.All(b => b.Value);

        public IBool Clone()
        {
            return new Conjunction(bools.Select(b => b.Clone()));
        }

        public void AndWith(IBool b) => bools.Add(b);
    }

    public class Negation : IBool
    {
        public IBool Bool;

        [JsonConstructor]
        public Negation(IBool Bool)
        {
            this.Bool = Bool;
        }

        [JsonIgnore]
        public bool Value => !Bool.Value;

        public IBool Clone()
        {
            return new Negation(Bool.Clone());
        }
    }
}
