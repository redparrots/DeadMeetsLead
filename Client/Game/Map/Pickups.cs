using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graphics;
using SlimDX;
using Graphics.Content;

namespace Client.Game.Map
{
    [Serializable]
    public abstract class Pickup : Destructible
    {
        public Pickup()
        {
            IsDestructible = false;
        }
        protected override void OnAddedToScene()
        {
            base.OnAddedToScene();
            if(IsInGame)
                Game.Instance.Mechanics.Pickups.Insert(this, VisibilityWorldBounding);

            Glitter = new Client.Game.Map.Effects.Glitter();
            Scene.Add(Glitter);
            Glitter.Translation = Position + Vector3.UnitZ * (0.2f);
        }
        protected override void UpdateMotionObject()
        {
        }
        protected override void OnMoved()
        {
            base.OnMoved();
            if(IsInGame && !IsRemoved)
                Game.Instance.Mechanics.Pickups.Move(this, VisibilityWorldBounding);

            if(Glitter != null)
                Glitter.Translation = Position + Vector3.UnitZ * (0.2f);
        }
        protected override void OnRemovedFromScene()
        {
            base.OnRemovedFromScene();
            if (IsInGame)
                Game.Instance.Mechanics.Pickups.Remove(this);

            Glitter.Stop();
        }

        public virtual void DoPickup(Unit unit) 
        {
            Remove(); 
        }

        [NonSerialized]
        private Effects.Glitter Glitter;
    }

    [Serializable, EditorDeployable(Group = "Pickups")]
    public class AmmoBox : Pickup
    {
        public AmmoBox()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Ammobox1.x"),
                Texture = new TextureFromFile("Models/Props/Ammobox1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/AmmoboxSpecular1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                ReceivesSpecular = Priority.Medium,
            };
            PickingLocalBounding = VisibilityLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)MainGraphic).XMesh,
                Transformation = ((MetaModel)MainGraphic).World
            };
            Ammo = 10;
        }
        public int Ammo { get; set; }
        public override void DoPickup(Unit unit)
        {
            unit.PistolAmmo += Ammo;

            Interface.ScrollingCombatText s;
            Game.Instance.Interface.AddChild(s = new Interface.ScrollingCombatText
            {
                Text = "+" + Ammo + Locale.Resource.GenLCBullets,
                WorldPosition = Translation + Vector3.UnitZ * 1
            });
            s.Font.Color = System.Drawing.Color.Green;

            var sm = Program.Instance.SoundManager;
            sm.GetSFX(global::Client.Sound.SFX.AmmoPickup1).Play(new Sound.PlayArgs());

            base.DoPickup(unit);
        }
    }

    [Serializable, EditorDeployable(Group = "Pickups")]
    public class Talisman : Pickup
    {
        public Talisman()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Ammobox1.x"),
                Texture = new TextureFromFile("Models/Props/Stone3.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
            };
            PickingLocalBounding = VisibilityLocalBounding = new MetaBoundingBox
            {
                Mesh = ((MetaModel)MainGraphic).XMesh,
                Transformation = ((MetaModel)MainGraphic).World
            };
        }
        public override void DoPickup(Unit unit)
        {
            ((Units.MainCharacter)unit).TalismansCollected++;

            Interface.ScrollingCombatText s;
            Game.Instance.Interface.AddChild(s = new Interface.ScrollingCombatText
            {
                Text = "+1 talisman",
                WorldPosition = Translation + Vector3.UnitZ * 1
            });
            s.Font.Color = System.Drawing.Color.Green;

            base.DoPickup(unit);
        }
    }


    [Serializable, EditorDeployable(Group = "Pickups")]
    public class HPPotion : Pickup
    {
        public HPPotion()
        {
            HealPerc = 0.5f;
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Bottle1.x"),
                Texture = new TextureFromFile("Models/Props/HpBottle1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/HPBottleSpecular1.png"),
                World = Matrix.Scaling(0.09f, 0.09f, 0.09f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = global::Graphics.Content.Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 12,
            };
            PickingLocalBounding = VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
        }
        public override void DoPickup(Unit unit)
        {
            if (unit.HitPoints >= unit.MaxHitPoints) return;

            //Sound and visual effect
            Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.PotionPickup1).Play(new Client.Sound.PlayArgs { });
            unit.AddChild(new Client.Game.Map.Effects.RageSac { Translation = Vector3.UnitZ * 1.5f });

            unit.Heal(null, (int)(HealPerc * unit.MaxHitPoints));
            
            Interface.ScrollingCombatText s;
            Game.Instance.Interface.AddChild(s = new Interface.ScrollingCombatText
            {
                Text = "+" + (int)(100 * HealPerc) + "% " + Locale.Resource.GenLCHP,
                WorldPosition = Translation + Vector3.UnitZ * 1
            });
            s.Font.Color = System.Drawing.Color.Green;

            base.DoPickup(unit);
        }
        [NonSerialized]
        public float HealPerc;

    }

    [Serializable, EditorDeployable(Group = "Pickups")]
    public class RagePotion : Pickup
    {
        public RagePotion()
        {
            Rage = 1.5f;
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Bottle1.x"),
                Texture = new TextureFromFile("Models/Props/RageBottle1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/HPBottleSpecular1.png"),
                World = Matrix.Scaling(0.09f, 0.09f, 0.09f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = global::Graphics.Content.Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 12,
            };
            PickingLocalBounding = VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
        }
        public override void DoPickup(Unit unit)
        {
            ((Units.MainCharacter)unit).AddRageLevelProgress(Rage);

            //Sound and visual effect
            Program.Instance.SoundManager.GetSFX(Client.Sound.SFX.PotionPickup1).Play(new Client.Sound.PlayArgs { });
            unit.AddChild(new Client.Game.Map.Effects.HpSac { Translation = Vector3.UnitZ * 1.5f });

            Interface.ScrollingCombatText s;
            Game.Instance.Interface.AddChild(s = new Interface.ScrollingCombatText
            {
                Text = "+" + Rage + " " + Locale.Resource.GenLCRage,
                WorldPosition = Translation + Vector3.UnitZ * 1
            });
            s.Font.Color = System.Drawing.Color.Orange;

            base.DoPickup(unit);
        }
        [NonSerialized]
        public float Rage;
    }


    [Serializable, EditorDeployable(Group = "Pickups")]
    public class AntidotePotion : Pickup
    {
        public AntidotePotion()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Bottle1.x"),
                Texture = new TextureFromFile("Models/Props/AntidoteBottle1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/HPBottleSpecular1.png"),
                World = Matrix.Scaling(0.1f, 0.1f, 0.1f) * SkinnedMesh.InitSkinnedMeshFromMaya,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = global::Graphics.Content.Priority.High,
                ReceivesSpecular = Priority.Medium,
                SpecularExponent = 12,
            };
            PickingLocalBounding = VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
        }
        public override void DoPickup(Unit unit)
        {
            new FinishScript
            {
                InitDelay = 0.2f,
                State = GameState.Won,
                Reason = "Finished map"
            }.TryStartPerform();
            base.DoPickup(unit);
        }
    }

    [Serializable, EditorDeployable(Group = "Pickups")]
    public class HammerPickup : Pickup
    {
        public HammerPickup()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/MayaHammer1.x"),
                Texture = new TextureFromFile("Models/Props/Sword1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/SwordSpecular1.png"),
                World = Matrix.Scaling(0.2f, 0.2f, 0.2f) * SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Translation(-0.5f, 0, 0),
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High
            };
            PickingLocalBounding = VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
        }
        public override void DoPickup(Unit unit)
        {
            Program.Instance.Profile.AvailableMeleeWeapons |= MeleeWeapons.MayaHammer;
            base.DoPickup(unit);
            new DialogScript
            {
                Title = "Hammer!",
                Text = "You've picked up a hammer! You can select this weapon the next time you start a map.",
            }.TryStartPerform();
        }
    }


    [Serializable, EditorDeployable(Group = "Pickups")]
    public class SpearPickup : Pickup
    {
        public SpearPickup()
        {
            MainGraphic = new MetaModel
            {
                XMesh = new MeshFromFile("Models/Props/Spear1.x"),
                Texture = new TextureFromFile("Models/Props/Sword1.png"),
                SpecularTexture = new TextureFromFile("Models/Props/SwordSpecular1.png"),
                World = Matrix.Scaling(0.2f, 0.2f, 0.2f) * SkinnedMesh.InitSkinnedMeshFromMaya * Matrix.Translation(-1, 0, 0),
                Visible = Priority.High,
                CastShadows = global::Graphics.Content.Priority.High,
                ReceivesShadows = Priority.High,
                ReceivesSpecular = Priority.High
            };
            PickingLocalBounding = VisibilityLocalBounding = CreateBoundingBoxFromModel((MetaModel)MainGraphic);
        }
        public override void DoPickup(Unit unit)
        {
            Program.Instance.Profile.AvailableMeleeWeapons |= MeleeWeapons.Spear;
            base.DoPickup(unit);
            new DialogScript
            {
                Title = "Spear!",
                Text = "You've picked up a spear! You can select this weapon the next time you start a map.",
            }.TryStartPerform();
        }
    }
}
