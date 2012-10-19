using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace Client.Sound
{
    public enum SFX
    {
        //Interface
        ButtonClickSmall1,
        MenuFadeIn1,
         
        //In-Game
        AchievementEarned1,
        AmmoPickup1,
        BigGrowl1,
        BlasterFire1,
        BlasterFireHitGround1,
        BridgeCollapse1,
        BulletHitFlesh1,
        BulletHitFlesh2,
        BullIdle1,
        BullStomp1,
        CannonFire1,
        Charge1,
        ChestBreak1,
        ClericIdle1,
        ClericIdle2,
        ClericIdle3,
        CloseGate1,
        CommanderBuffShout1,
        DemonGrowl1,
        DMLLogo1,
        DogBite1,
        DogStepsGrass1,
        EventIncoming1,
        EventTension1,
        EventTension2,
        FireBreath1,
        FleshExplode1,
        FootStepsGrass1_3D,
        FootStepsGrass1,
        FootStepsGrass2,
        FootStepsGrassLight1,
        FootStepsGrassLight2,
        FootStepsWater1_3D,
        FootStepsWater1,
        FootStepsWater2,
        GatlingGunFire1,
        GruntAttack1,
        GruntAttack2,
        GruntIdle1,
        GruntIdle2,
        GruntIdle3,
        Horn1,
        HumanDeath1,
        IAmOutOfRage1,
        ScourgedEarthIndicator1,
        InPain1,
        Jump1,
        KeldynLogo1,
        LightningAndThunder1,
        LowHealthBeat1,
        Malaria1,
        MetalMovement1,
        MetalMovement2,
        MetalMovement3,
        MetalMovement4,
        MongrelIdle1,
        MosquitoAmbient1,
        OpenGate1,
        OrgansOnGround1,
        PhysicalHitFlesh1,
        PiranhaAttack1,
        PiranhaSwimming1,
        RaiseDead1,
        ReceiveNet1,
        RifleEmpty1,
        RifleFireReload1,
        ShotgunFire1,
        ShotgunFireReload1,
        SilverShot1,
        SlamImpact1,
        Slither1,
        SwordHitFlesh1,
        SwordHitFlesh2,
        SwordHitFlesh3,
        SwordHitFlesh4,
        SwordHitWood1,
        SwordHitWood2,
        SwordSwish1,
        SwordSwish2,
        SwordSwish3,
        ThrowNet1,
        ThrustCrit1,
        TimeRunningOut1,
        WeaponSwitch1,
        WindBassEerie1,
        WolfBossGrowl1,
        ButtonClickLarge1,
        GatlingGunFire2,
        BuyWeapon1,
        ButtonMapClickLarge1,
        MouseEnterButton1,
        RageLevelGain1,
        RageLevelGain2,
        RageLevelGain3,
        ScourgedEarth1,
        HelpPopUpIndicator1,
        MeteorPassBy1,
        MeteorCrash1,
        UnlockTier1,
        DemonLordAttack1,
        DemonLordAttack2,
        DemonLordNova1,
        DemonLordCharge1,
        DemonLordAura1,
        DemonLordDeath1,
        DemonLordLavaDitch1,
        WolfBossWhine1,
        WolfBossWhine2,
        Land1,
        Trombone1,
        GhostBulletFire1,
        BrutusIntro1,
        WolfIntro1,
        StageCompleted1,
        SwordHitFlesh5,
        VictoryCheer1,
        PotionPickup1,
    }

    public enum Stream
    {
        BirdsAmbient1,
        InGameMusic1,
        InGameMusic2,
        MainMenuMusic1,
        ScoreScreenDefeatMusic1,
        ScoreScreenVictoryMusic1,
        MosquitoAmbient1,
        InGameMusic3,
        EmptyTrack,
        MainMenuMusic2,
        ScoreScreenVictoryMusic2,
        InGameMusic4,
        InGameMusic5,
        Cutscene1,
        CustceneIntro1,
        CutsceneLoop1
    }

    public enum SoundGroups
    {
        Default,   // don't touch this one

        Ambient,
        Music,
        SoundEffects,
        Interface
    }

    public partial class SoundManager
    {
        private void LoadSFX(bool loadFullGameSounds)
        {
            //Interface 2D IN USE
            LoadSound(SFX.ButtonClickSmall1, "ButtonClickSmall1.wav", new SoundResource(false, false)
            {
                SoundGroupEnum = SoundGroups.Interface,
                Volume = 0.6f
            });
            LoadSound(SFX.ButtonClickLarge1, "ButtonClickLarge1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.Interface, Volume = 0.6f });
            LoadSound(SFX.ButtonMapClickLarge1, "ButtonMapClickLarge1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.Interface, Priority = Priority.VeryHigh, PlaybackSpeed = 0.8f, Volume = 0.2f });
            LoadSound(SFX.HelpPopUpIndicator1, "HelpPopUpIndicator1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.Interface, Volume = 0.1f });
            LoadSound(SFX.MouseEnterButton1, "MouseEnterButton1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.Interface, Priority = Priority.VeryHigh });
            LoadSound(SFX.StageCompleted1, "StageCompleted1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.Interface, Priority = Priority.VeryHigh, Volume = 0.65f });
            if(loadFullGameSounds)
                LoadSound(SFX.UnlockTier1, "UnlockTier1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.Interface, Volume = 1 });

            //Interface 2D NOT IN USE
            //LoadSound(SFX.MenuFadeIn1, "MenuFadeIn1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects });

            //SFX 2D IN USE
            if (loadFullGameSounds)
            {
                LoadSound(SFX.AchievementEarned1, "AchievementEarned1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 0.7f });
            }
            LoadSound(SFX.AmmoPickup1, "AmmoPickup1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });
            if(loadFullGameSounds)
                LoadSound(SFX.BigGrowl1, "BigGrowl1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });
            if (loadFullGameSounds)
            {
                LoadSound(SFX.BlasterFire1, "BlasterFire1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 0.5f });
                LoadSound(SFX.BrutusIntro1, "BrutusIntro1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });
                LoadSound(SFX.BuyWeapon1, "BuyWeapon1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 1 });
                LoadSound(SFX.CannonFire1, "CannonFire1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 1.0f });
                LoadSound(SFX.DemonGrowl1, "DemonLordGrowl1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 0.85f });
            }
            LoadSound(SFX.DMLLogo1, "DMLLogo1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 1f });
            if (loadFullGameSounds)
            {
                LoadSound(SFX.EventTension1, "EventTension1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 0.8f });
            }
            LoadSound(SFX.FootStepsGrass1, "FootStepsGrass1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 0.7f });
            LoadSound(SFX.FootStepsGrass2, "FootStepsGrass2.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 0.7f });
            LoadSound(SFX.FootStepsWater1, "FootStepsWater1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 0.8f });
            LoadSound(SFX.FootStepsWater2, "FootStepsWater2.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 0.8f });
            LoadSound(SFX.GhostBulletFire1, "GhostBulletFire1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });
            if (loadFullGameSounds)
            {
                LoadSound(SFX.GatlingGunFire1, "GatlingGunFire1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 0.7f });
            }
            LoadSound(SFX.HumanDeath1, "HumanDeath1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });
            LoadSound(SFX.IAmOutOfRage1, "IAmOutOfRage1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 0.75f });
            LoadSound(SFX.InPain1, "InPain1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.High });
            LoadSound(SFX.Jump1, "Jump1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 0.8f });
            LoadSound(SFX.KeldynLogo1, "KeldynLogo1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 1f });
            LoadSound(SFX.Land1, "Land1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 0.9f });
            if (loadFullGameSounds)
            {
                LoadSound(SFX.LightningAndThunder1, "LightningAndThunder1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });
            }
            LoadSound(SFX.LowHealthBeat1, "LowHealthBeat1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 1.0f });
            //LoadSound(SFX.MetalMovement1, "MetalMovement1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.Music, Priority = Priority.VeryHigh, Volume = 0.25f });
            //LoadSound(SFX.MetalMovement2, "MetalMovement2.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.Music, Priority = Priority.VeryHigh, Volume = 0.25f });
            //LoadSound(SFX.MetalMovement3, "MetalMovement3.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.Music, Priority = Priority.VeryHigh, Volume = 0.25f });
            //LoadSound(SFX.MetalMovement4, "MetalMovement4.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.Music, Priority = Priority.VeryHigh, Volume = 0.25f });

            LoadSound(SFX.PhysicalHitFlesh1, "PhysicalHitFlesh1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });
            LoadSound(SFX.PotionPickup1, "PotionPickup1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 1 });
            LoadSound(SFX.RageLevelGain1, "RageLevelGain1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 1f });
            LoadSound(SFX.RageLevelGain2, "RageLevelGain2.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 1f });
            LoadSound(SFX.RageLevelGain3, "RageLevelGain3.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 1f });
            LoadSound(SFX.ReceiveNet1, "ReceiveNet1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });
            LoadSound(SFX.RifleEmpty1, "RifleEmpty1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });
            if(loadFullGameSounds)
                LoadSound(SFX.ScourgedEarthIndicator1, "ScourgedEarthIndicator1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });
            LoadSound(SFX.ShotgunFire1, "ShotgunFire1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 0.65f });
            LoadSound(SFX.SwordSwish1, "SwordSwish1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 0.8f });
            LoadSound(SFX.SwordSwish2, "SwordSwish2.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 0.8f });
            LoadSound(SFX.SwordSwish3, "SwordSwish3.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 0.8f });
            LoadSound(SFX.ThrustCrit1, "ThrustCrit1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 1f });
            if(loadFullGameSounds)
                LoadSound(SFX.TimeRunningOut1, "TimeRunningOut1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });
            LoadSound(SFX.WeaponSwitch1, "WeaponSwitch1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 0.3f });
            LoadSound(SFX.Trombone1, "Trombone1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 1, });
            LoadSound(SFX.VictoryCheer1, "VictoryCheer1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 1 });
            
            if (loadFullGameSounds)
            {
                LoadSound(SFX.WindBassEerie1, "WindBassEerie1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 1 });
                LoadSound(SFX.WolfBossGrowl1, "WolfBossGrowl1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });
                LoadSound(SFX.WolfIntro1, "WolfIntro1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });
            }
            //SFX 2D NOT IN USE
            LoadSound(SFX.RifleFireReload1, "RifleFireReload1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });
            //LoadSound(SFX.ShotgunFireReload1, "ShotgunFireReload1.wav", new SoundResource(false, false) { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });

            //SFX 3D IN USE
            if (loadFullGameSounds)
            {
                LoadSound(SFX.BlasterFireHitGround1, "BlasterFireHitGround1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 1f });
                LoadSound(SFX.BridgeCollapse1, "BridgeCollapse1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.High, Volume = 0.6f });
            }
            LoadSound(SFX.BulletHitFlesh1, "BulletHitFlesh1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });
            LoadSound(SFX.BulletHitFlesh2, "BulletHitFlesh2.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });
            if (loadFullGameSounds)
            {
                LoadSound(SFX.BullIdle1, "BullIdle1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.Low });
                LoadSound(SFX.BullStomp1, "BullStomp1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryLow });
                LoadSound(SFX.Charge1, "Charge1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.High });
            }
            LoadSound(SFX.ChestBreak1, "ChestBreak1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.High });
            if (loadFullGameSounds)
            {
                LoadSound(SFX.ClericIdle1, "ClericIdle1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.Low });
                LoadSound(SFX.ClericIdle2, "ClericIdle2.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.Low });
                LoadSound(SFX.ClericIdle3, "ClericIdle3.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.Low });
            }
            LoadSound(SFX.CloseGate1, "CloseGate1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 0.7f });
            if (loadFullGameSounds)
            {
                LoadSound(SFX.CommanderBuffShout1, "CommanderBuffShout1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.High });
                LoadSound(SFX.DemonLordAttack1, "DemonLordAttack1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.High, Volume = 0.7f });
                LoadSound(SFX.DemonLordAttack2, "DemonLordAttack2.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.High, Volume = 0.6f });
                LoadSound(SFX.DemonLordAura1, "DemonLordAura1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.High, Volume = 1f });
                LoadSound(SFX.DemonLordCharge1, "DemonLordCharge1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.High, Volume = 0.8f });
                LoadSound(SFX.DemonLordDeath1, "DemonLordDeath1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.High, Volume = 1f });
                LoadSound(SFX.DemonLordLavaDitch1, "DemonLordLavaDitch1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.High, Volume = 0.7f });
                LoadSound(SFX.DemonLordNova1, "DemonLordNova1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.High, Volume = 1f });
                LoadSound(SFX.DogBite1, "DogBite1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.Medium });
                //NOT IN USE
                LoadSound(SFX.FireBreath1, "FireBreath1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.High });
            }
            LoadSound(SFX.FleshExplode1, "FleshExplode1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });
            LoadSound(SFX.FootStepsGrass1_3D, "FootStepsGrass1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.Low });
            if (loadFullGameSounds)
            {
                LoadSound(SFX.FootStepsGrassLight1, "FootStepsGrassLight1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.Medium, Volume = 0.75f });
                LoadSound(SFX.FootStepsGrassLight2, "FootStepsGrassLight2.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.Medium, Volume = 0.75f });
            }
            LoadSound(SFX.FootStepsWater1_3D, "FootStepsWater1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.Low });
            LoadSound(SFX.GruntAttack1, "GruntAttack1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.Medium });
            LoadSound(SFX.GruntAttack2, "GruntAttack2.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.Medium });
            LoadSound(SFX.GruntIdle1, "GruntIdle1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryLow });
            LoadSound(SFX.GruntIdle2, "GruntIdle2.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryLow });
            LoadSound(SFX.GruntIdle3, "GruntIdle3.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryLow });
            if(loadFullGameSounds)
                LoadSound(SFX.Horn1, "Horn1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.High });
            LoadSound(SFX.Malaria1, "Malaria1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });
            if (loadFullGameSounds)
            {
                LoadSound(SFX.MeteorCrash1, "MeteorCrash1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.Medium, Volume = 1f });
                LoadSound(SFX.MeteorPassBy1, "MeteorPassBy1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.Medium, Volume = 0.8f });
                LoadSound(SFX.MongrelIdle1, "MongrelIdle1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.Low });
            }
            LoadSound(SFX.MosquitoAmbient1, "MosquitoAmbient1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.Ambient, Priority = Priority.Medium, Volume = 0.45f });
            LoadSound(SFX.OpenGate1, "OpenGate1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 0.7f });
            LoadSound(SFX.OrgansOnGround1, "OrgansOnGround1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.Medium, InternalCooldown = 0.5f });
            LoadSound(SFX.PiranhaAttack1, "PiranhaAttack1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.Medium });
            LoadSound(SFX.PiranhaSwimming1, "PiranhaSwimming1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.Ambient, Priority = Priority.High });
            if (loadFullGameSounds)
            {
                LoadSound(SFX.RaiseDead1, "RaiseDead1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.High });
                LoadSound(SFX.ScourgedEarth1, "ScourgedEarth1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });
            }
            LoadSound(SFX.SilverShot1, "SilverShot1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });
            LoadSound(SFX.SlamImpact1, "SlamImpact1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });
            if (loadFullGameSounds)
                LoadSound(SFX.Slither1, "Slither1.wav", new SoundResource() { PlaybackSpeed = 0.5f, SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.Low });
            LoadSound(SFX.SwordHitFlesh1, "SwordHitFlesh1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.High });
            LoadSound(SFX.SwordHitFlesh2, "SwordHitFlesh2.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.High });
            LoadSound(SFX.SwordHitFlesh5, "SwordHitFlesh5.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.High });

            if (loadFullGameSounds)
            {
                //NOT IN USE
                LoadSound(SFX.SwordHitFlesh3, "SwordHitFlesh3.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.High });
                //NOT IN USE
                LoadSound(SFX.SwordHitFlesh4, "SwordHitFlesh4.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.High });
                //NOT IN USE
                //LoadSound(SFX.SwordHitWood1, "SwordHitWood1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.High });
            }
            LoadSound(SFX.SwordHitWood1, "SwordHitWood1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.High, Volume = 0.6f });
            LoadSound(SFX.ThrowNet1, "ThrowNet1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });
            if (loadFullGameSounds)
            {
                LoadSound(SFX.WolfBossWhine1, "WolfBossWhine1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 0.1f });
                LoadSound(SFX.WolfBossWhine2, "WolfBossWhine2.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh, Volume = 0.4f });
            }

            //SFX 3D NOT IN USE
            //LoadSound(SFX.DogEating1, "DogEating1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryLow });
            //LoadSound(SFX.RottenTreeHitBySword1, "RottenTreeHitBySword1.wav", new SoundResource() { SoundGroupEnum = SoundGroups.SoundEffects, Priority = Priority.VeryHigh });
        }

        private void LoadStreams(bool loadFullGameSounds)
        {
            //Ambient 2D
            LoadSound(Stream.BirdsAmbient1, "BirdsAmbient1.wav", new SoundResource(false, true) { SoundGroupEnum = SoundGroups.Ambient, Priority = Priority.VeryHigh, Volume = 0.45f });
            
            //Music 2D
            //In-game
            LoadSound(Stream.InGameMusic1, "InGameMusic1.wav", new SoundResource(false, true) { SoundGroupEnum = SoundGroups.Music, Priority = Priority.VeryHigh, Volume = 0.55f });
            LoadSound(Stream.InGameMusic2, "InGameMusic2.wav", new SoundResource(false, true) { SoundGroupEnum = SoundGroups.Music, Priority = Priority.VeryHigh, Volume = 0.55f });
            if (loadFullGameSounds)
            {
                LoadSound(Stream.InGameMusic3, "InGameMusic3.wav", new SoundResource(false, true) { SoundGroupEnum = SoundGroups.Music, Priority = Priority.VeryHigh, Volume = 0.55f });
                LoadSound(Stream.InGameMusic4, "InGameMusic4.wav", new SoundResource(false, true) { SoundGroupEnum = SoundGroups.Music, Priority = Priority.VeryHigh, Volume = 0.55f });
                LoadSound(Stream.InGameMusic5, "InGameMusic5.wav", new SoundResource(false, true) { SoundGroupEnum = SoundGroups.Music, Priority = Priority.VeryHigh, Volume = 0.55f });
                //LoadSound(Stream.Cutscene1, "Cutscene1.wav", new SoundResource(false, true) { SoundGroupEnum = SoundGroups.Music, Priority = Priority.VeryHigh });
                LoadSound(Stream.CustceneIntro1, "CutsceneIntro1.wav", new SoundResource(false, true) { SoundGroupEnum = SoundGroups.Music, Priority = Priority.VeryHigh, Volume = 0.8f });
                LoadSound(Stream.CutsceneLoop1, "CutsceneLoop1.wav", new SoundResource(false, true) { SoundGroupEnum = SoundGroups.Music, Priority = Priority.VeryHigh, Volume = 0.8f });
            }
            //Menu
            LoadSound(Stream.MainMenuMusic1, "MainMenuMusic1.wav", new SoundResource(false, true) { SoundGroupEnum = SoundGroups.Music, Priority = Priority.VeryHigh, Volume = 0.45f });
            //LoadSound(Stream.MainMenuMusic2, "MainMenuMusic2.wav", new SoundResource(false, true) { SoundGroupEnum = SoundGroups.Music, Priority = Priority.VeryHigh, Volume = 0.4f });
            //Score screen
            LoadSound(Stream.ScoreScreenVictoryMusic1, "ScoreScreenVictoryMusic1.wav", new SoundResource(false, true) { SoundGroupEnum = SoundGroups.Music, Priority = Priority.VeryHigh });
            //LoadSound(Stream.ScoreScreenVictoryMusic2, "ScoreScreenVictoryMusic2.wav", new SoundResource(false, true) { SoundGroupEnum = SoundGroups.Music, Priority = Priority.VeryHigh });
        }

        private void SetupSoundGroupSettings()
        {
            //GetSoundGroup(SoundGroups.Ambient).MaxAudible = 10;
            //GetSoundGroup(SoundGroups.Music).MaxAudible = 2;
            //GetSoundGroup(SoundGroups.SoundEffects).MaxAudible = 50;
            //GetSoundGroup(SoundGroups.Interface).MaxAudible = 2;
        }
    }
}