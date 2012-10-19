using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.ComponentModel;
using Graphics;
using Graphics.Content;

namespace Client.Game.Map
{
    [TypeConverter(typeof(ExpandableObjectConverter)),
    Editor(typeof(Common.WindowsForms.InstanceSelectTypeEditor<IUnitGroup>), typeof(System.Drawing.Design.UITypeEditor)),
    Serializable]
    public abstract class IUnitGroup : ICloneable
    {
        public IUnitGroup()
        {
            TeamFilter = Team.Neutral | Team.Player | Team.Zombie;
        }
        public Team TeamFilter { get; set; }
        [Editor(typeof(Common.WindowsForms.TypeSelectTypeEditor<Destructible>), typeof(System.Drawing.Design.UITypeEditor))]
        public Type TypeFilter { get; set; }
        public IEnumerable<Destructible> GetDestructibles()
        {
            foreach (var v in InternalGetDestructibles())
            {
                if ((v.Team & TeamFilter) == 0) continue;
                if (TypeFilter != null && !v.GetType().IsAssignableFrom(TypeFilter)) continue;
                yield return v;
            }
        }
        public IEnumerable<Unit> GetUnits()
        {
            foreach (var v in GetDestructibles())
            {
                var u = v as Unit;
                if (u != null)
                    yield return u;
            }
        }
        public virtual bool IsInGroup(Destructible destructible) 
        {
            foreach (var v in GetDestructibles())
                if (v == destructible) return true;
            return false;
        }
        protected abstract IEnumerable<Destructible> InternalGetDestructibles();

        public object Clone()
        {
            var t = GetType();
            var clone = Activator.CreateInstance(t);

            foreach (var v in t.GetProperties())
            {
                if (v.CanWrite)
                {
                    var val = v.GetValue(this, null);
                    var c = val as ICloneable;
                    if (c != null)
                        val = c.Clone();
                    v.SetValue(clone, val, null);
                }
            }

            return clone;
        }
    }


    [Serializable]
    public class SingleUnitGroup : IUnitGroup
    {
        public string Unit { get; set; }
        protected override IEnumerable<Destructible> InternalGetDestructibles()
        {
            var u = Game.Instance.Scene.GetByName(Unit) as Destructible;
            if (u != null) yield return u;
        }
    }

    [Serializable]
    public class ListUnitGroup : IUnitGroup
    {
        public ListUnitGroup()
        {
            Units = new string[0];
        }
        public string[] Units { get; set; }
        protected override IEnumerable<Destructible> InternalGetDestructibles()
        {
            if (units == null)
            {
                units = new List<Destructible>();
                foreach (var v in Units)
                {
                    var e = Game.Instance.Scene.GetByName(v) as Destructible;
                    if (e != null)
                        units.Add(e);
                }
            }
            return units;
        }
        [NonSerialized]
        List<Destructible> units;
    }

    [Serializable]
    public class RegionUnitGroup : IUnitGroup
    {
        public string Region { get; set; }
        protected override IEnumerable<Destructible> InternalGetDestructibles()
        {
            var region = Game.Instance.Map.GetRegion(Region);
            foreach (var v in Game.Instance.Scene.AllEntities)
            {
                var d = v as Destructible;
                if (d != null && region.BoundingRegion.GetNodeAt(v.Translation) != null)
                    yield return d;
            }
        }
        public override bool IsInGroup(Destructible destructible)
        {
            var region = Game.Instance.Map.GetRegion(Region);
            return region.BoundingRegion.GetNodeAt(destructible.Translation) != null;
        }
    }


    [Serializable]
    public class AllUnitsUnitGroup : IUnitGroup
    {
        protected override IEnumerable<Destructible> InternalGetDestructibles()
        {
            foreach (var v in Game.Instance.Scene.AllEntities)
            {
                var d = v as Destructible;
                if (d != null)
                    yield return d;
            }
        }
    }
}
