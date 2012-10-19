using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public enum ProgramEventType
    {
        ProgramStarted,
        ProgramStateChanged,
        WorldMapControlMoved,
        DestructibleKilled,
        PurchasedWeapon,
        StartPlayingMap,
        StopPlayingMap,
        ProfileSaved,
        ScriptStartPerform,
        ScriptEndPerform,
        UserInput,
        CompletedMap,
        MainCharacterRageLevel,
        MainCharacterStrike,
        PistolAmmoChanged,
        MainCharacterTakesDamage,
        MainCharacterSwitchWeapon,
        UnitHit,
        RageLevelChanged,
        RageLevelProgressChanged,
        AchievementEarned,
        StageCompleted,
        UnitJumps,
        CustomMapEvent,
        MainCharacterCaughtInNet
    }

    [Serializable]
    public class ProgramEvent
    {
        public ProgramEventType Type { get; set; }
        public override string ToString()
        {
            return "ProgramEvent " + Type;
        }
    }

    delegate void ProgramEventHandler(ProgramEvent e);

    namespace ProgramEvents
    {
        [Serializable]
        class StartPlayingMap : ProgramEvent
        {
            public StartPlayingMap() { Type = ProgramEventType.StartPlayingMap; }
            public String MapName { get; set; }
            public MeleeWeapons MeleeWeapon { get; set; }
            public RangedWeapons RangedWeapon { get; set; }
            [NonSerialized]
            Game.Map.Map map;
            public Game.Map.Map Map { get { return map; } set { map = value; } }
        }

        [Serializable]
        class CustomMapEvent : ProgramEvent
        {
            public CustomMapEvent() { Type = ProgramEventType.CustomMapEvent; }
            public string EventName { get; set; }
        }

        [Serializable]
        class StopPlayingMap : ProgramEvent
        {
            public StopPlayingMap() { Type = ProgramEventType.StopPlayingMap; }
            public Game.GameState GameState { get; set; }
            public String MapFileName { get; set; }
            public float TimeElapsed { get; set; }
            public int HitsTaken { get; set; }
            public int DamageDone { get; set; }
            public int BulletsUsed { get; set; }
            public int SilverYield { get; set; }
            public int GoldYield { get; set; }
        }

        class DestructibleKilled : ProgramEvent
        {
            public DestructibleKilled() { Type = ProgramEventType.DestructibleKilled; }
            public Game.Map.Unit Perpetrator { get; set; }
            public Game.Map.Destructible Destructible { get; set; }
            public Game.Map.Script Script { get; set; }
        }


        class ScriptStartPerform : ProgramEvent
        {
            public ScriptStartPerform() { Type = ProgramEventType.ScriptStartPerform; }
            public Game.Map.Script Script { get; set; }
        }

        class ScriptEndPerform : ProgramEvent
        {
            public ScriptEndPerform() { Type = ProgramEventType.ScriptEndPerform; }
            public Game.Map.Script Script { get; set; }
        }

        enum UserInputType
        {
            WASD,
            Jump,
            FirePrimary
        }
        class UserInput : ProgramEvent 
        {
            public UserInput() { Type = ProgramEventType.UserInput; }
            public UserInputType InputType { get; set; } 
        }

        class CompletedMap : ProgramEvent 
        {
            public CompletedMap() { Type = ProgramEventType.CompletedMap; }
            public String MapName { get; set; }
            public int GoldEarned { get; set; }
        }

        class MainCharacterCaughtInNet : ProgramEvent
        {
        }

        [Obsolete]
        class MainCharacterRageLevel : ProgramEvent
        {
            public MainCharacterRageLevel() { Type = ProgramEventType.MainCharacterRageLevel; }
            public String MapName { get; set; }
            public int RageLevel { get; set; }
        }

        [Obsolete]
        class MainCharacterStrike : ProgramEvent
        {
            public MainCharacterStrike() { Type = ProgramEventType.MainCharacterStrike; }
            public int DamageDealt { get; set; }
        }

        [Obsolete]
        class MainCharacterTakesDamage : ProgramEvent
        {
            public MainCharacterTakesDamage() { Type = ProgramEventType.MainCharacterTakesDamage; }
            public int DamageTaken { get; set; }
            public Game.Map.Unit Perpetrator { get; set; }
        }

        class UnitHit : ProgramEvent
        {
            public UnitHit() { Type = ProgramEventType.UnitHit; }
            public Game.Map.DamageEventArgs DamageEventArgs { get; set; }
        }

        class PistolAmmoChanged : ProgramEvent
        {
            public PistolAmmoChanged() { Type = ProgramEventType.PistolAmmoChanged; }
            public Game.Map.Unit Unit { get; set; }
            public int OldPistolAmmo { get; set; }
        }

        class RageLevelChanged : ProgramEvent
        {
            public RageLevelChanged() { Type = ProgramEventType.RageLevelChanged; }
            public Game.Map.Unit Unit { get; set; }
            public int OldRageLevel { get; set; }
        }

        class RageLevelProgressChanged : ProgramEvent
        {
            public RageLevelProgressChanged() { Type = ProgramEventType.RageLevelProgressChanged; }
            public Game.Map.Unit Unit { get; set; }
            public float Diff { get; set; }
        }

        class AchievementEarned : ProgramEvent
        {
            public AchievementEarned() { Type = ProgramEventType.AchievementEarned; }
            public Achievement Achievement { get; set; }
        }

        class StageCompleted : ProgramEvent
        {
            public StageCompleted() { Type = ProgramEventType.StageCompleted; }
            public Game.Interface.StageInfo Stage { get; set; }
        }

        class MainCharacterSwitchWeapon : ProgramEvent
        {
            public MainCharacterSwitchWeapon() { Type = ProgramEventType.MainCharacterSwitchWeapon; }
            public int Weapon { get; set; }
        }

        class UnitJumps : ProgramEvent
        {
            public UnitJumps() { Type = ProgramEventType.UnitJumps; }
            public Game.Map.Unit Unit { get; set; }
        }
    }
}
