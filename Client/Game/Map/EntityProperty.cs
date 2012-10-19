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
    Editor(typeof(Common.WindowsForms.InstanceSelectTypeEditor<IEntityProperty>), typeof(System.Drawing.Design.UITypeEditor)),
    Serializable]
    public abstract class IEntityProperty : ICloneable
    {
        public IEntityProperty()
        {
        }
        public abstract bool IsActive(GameEntity entity);
        public abstract void Set(GameEntity entity);

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
    public class UnitStateProperty : IEntityProperty
    {
        public UnitState State { get; set; }
        public override bool IsActive(GameEntity entity)
        {
            var d = entity as Destructible;
            if (d == null) return false;
            return d.State == State;
        }
        public override void Set(GameEntity entity)
        {
            var d = entity as Destructible;
            if (d == null) return;
            d.State = State;
        }
    }

    [Serializable]
    public class UnitInCombatProperty : IEntityProperty
    {
        public UnitInCombatProperty()
        {
            Value = true;
        }
        public bool Value { get; set; }
        public override bool IsActive(GameEntity entity)
        {
            var d = entity as Unit;
            if (d == null) return false;
            return d.InCombat == Value;
        }
        public override void Set(GameEntity entity)
        {
            var d = entity as Unit;
            if (d == null) return;
            d.InCombat = Value;
        }
    }
    [Serializable]
    public class IsDestructibleProperty : IEntityProperty
    {
        public IsDestructibleProperty()
        {
            Value = true;
        }
        public bool Value { get; set; }
        public override bool IsActive(GameEntity entity)
        {
            var d = entity as Destructible;
            if (d == null) return false;
            return d.IsDestructible == Value;
        }
        public override void Set(GameEntity entity)
        {
            var d = entity as Destructible;
            if (d == null) return;
            d.IsDestructible = Value;
        }
    }

    public enum ComparisonOperator
    {
        Equals,
        LessThan,
        GreaterThan
    }

    [Serializable]
    public abstract class IntegerProperty : IEntityProperty
    {
        public IntegerProperty()
        {
        }
        public int Value { get; set; }
        public bool Percentage { get; set; }
        public ComparisonOperator Comparison { get; set; }
        protected abstract int GetValue(Destructible d);
        protected virtual int GetMaxValue(Destructible d) { return GetValue(d); }
        protected abstract void SetValue(Destructible d, int value);
        public override bool IsActive(GameEntity entity)
        {
            var d = entity as Destructible;
            if (d == null) return false;
            int val;
            if (Percentage)
                val = (int)(GetValue(d) * GetMaxValue(d) / 100);
            else
                val = (int)GetValue(d);
            switch (Comparison)
            {
                case ComparisonOperator.Equals:
                    return GetValue(d) == val;
                case ComparisonOperator.LessThan:
                    return GetValue(d) < val;
                case ComparisonOperator.GreaterThan:
                    return GetValue(d) > val;
                default:
                    throw new ArgumentException();
            }
        }
        public override void Set(GameEntity entity)
        {
            var d = entity as Destructible;
            if (d == null) return;
            if (Percentage)
                SetValue(d, (int)(GetValue(d) * GetMaxValue(d)));
            else
                SetValue(d, Value);
        }
    }

    [Serializable]
    public class HitPointsProperty : IEntityProperty
    {
        public HitPointsProperty()
        {
            Comparison = ComparisonOperator.Equals;
        }
        public float HitPoints { get; set; }
        public bool Percentage { get; set; }
        public ComparisonOperator Comparison { get; set; }
        public override bool IsActive(GameEntity entity)
        {
            var d = entity as Destructible;
            if (d == null) return false;
            int val;
            if (Percentage)
                val = (int)(HitPoints * d.MaxHitPoints);
            else
                val = (int)HitPoints;
            switch(Comparison)
            {
                case ComparisonOperator.Equals:
                    return d.HitPoints == val;
                case ComparisonOperator.LessThan:
                    return d.HitPoints < val;
                case ComparisonOperator.GreaterThan:
                    return d.HitPoints > val;
                default:
                    throw new ArgumentException();
            }
        }
        public override void Set(GameEntity entity)
        {
            var d = entity as Destructible;
            if (d == null) return;
            if (Percentage)
                d.HitPoints = (int)(HitPoints * d.MaxHitPoints);
            else
                d.HitPoints = (int)HitPoints;
        }
    }

    [Serializable]
    public class RageLevelProperty : IEntityProperty
    {
        public int RageLevel { get; set; }
        public override bool IsActive(GameEntity entity)
        {
            var d = entity as Unit;
            if (d == null) return false;
            return d.RageLevel == (int)RageLevel;
        }
        public override void Set(GameEntity entity)
        {
            var d = entity as Unit;
            if (d == null) return;
            d.RageLevel = RageLevel;
        }
    }


    [Serializable]
    public class AmmoProperty : IEntityProperty
    {
        public int Ammo { get; set; }
        public override bool IsActive(GameEntity entity)
        {
            var d = entity as Unit;
            if (d == null) return false;
            return d.PistolAmmo == (int)Ammo;
        }
        public override void Set(GameEntity entity)
        {
            var d = entity as Unit;
            if (d == null) return;
            d.PistolAmmo = Ammo;
        }
    }


    [Serializable]
    public class CanPerformAbilitiesBlockersProperty : IntegerProperty
    {
        protected override int GetValue(Destructible d)
        {
            var u = d as Unit;
            return u.CanPerformAbilitiesBlockers;
        }
        protected override void SetValue(Destructible d, int value)
        {
            var u = d as Unit;
            u.CanPerformAbilitiesBlockers = value;
        }
    }
    [Serializable]
    public class CanControlMovementBlockersProperty : IntegerProperty
    {
        protected override int GetValue(Destructible d)
        {
            var u = d as Unit;
            return u.CanControlMovementBlockers;
        }
        protected override void SetValue(Destructible d, int value)
        {
            var u = d as Unit;
            u.CanControlMovementBlockers = value;
        }
    }
    [Serializable]
    public class CanControlRotationBlockersProperty : IntegerProperty
    {
        protected override int GetValue(Destructible d)
        {
            var u = d as Unit;
            return u.CanControlRotationBlockers;
        }
        protected override void SetValue(Destructible d, int value)
        {
            var u = d as Unit;
            u.CanControlRotationBlockers = value;
        }
    }
}
