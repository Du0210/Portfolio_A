namespace HDU.Define
{
    using UnityEngine;

    public class CoreDefine
    {
        #region Const
        public const int MAXSTAGECOUNT = 6;
        public const int MAXSELECTUNITCOUNT = 6;
        public const int MAXREADYTIMECOUNT = 60;
        public const int MAXGRADE = 5;
        public const int MAXWAVE = 4;
        #endregion

        #region Enum
        public enum ESoundType
        {
            MasterVolume,
            Bgm,
            FX,
            MaxCount,
        }

        public enum EKeySound
        {
            MaxCount,
        }

        public enum ELanguageType
        {
            EN = 10,
            JA = 22,
            KR = 23,
            ES = 34,
            TW = 41,
            GE = 15,
            PO = 28,
        }

        public enum EUIEventType
        {
            LClickDown,
            LClick,
            LClickUp,
            BeginDrag,
            Drag,
            EndDrag,
            DragDrop,
            RClick,
            RDoubleClick,
            OnPoint,
            PointerEnter,
            PointerExit,
        }

        public enum EEventType
        {
            // Time
            OnChangedSeconds,
            OnChangedMinute,
            OnChangedHour,
            OnSynchronizeTime,

            // UI
            OnSetUnitSlot,
            OnRefreshUnitSlots,
            // Stage

            // Unit

            // Battle

            /// <summary> bool </summary>

        }
        public enum EBattleLoop
        {
            Init,
            Ready,
            Battle,
        }
        public enum ELobbySceneType
        {
            MainLobby,
            Dungeon,
            Stage,
            Deck,
        }
        public enum ESceneType
        {
            None,
            DevScene,
            MainScene,
        }
        public enum ESaveType
        {
            SettingData, //현재하나밖에없음
            MaxCount,
        }
        public enum ECardType
        {
            Cost,
            Unit,
            MaxCount,
        }
        public enum EGoodsType
        {
            StageGoods,
            Golds,
            Dia,
            Star,
            MaxCount,
        }
        public enum EStageType
        {
            First,
            MaxCount,
        }

        public enum EBattleState
        {
            /// <summary> 로비씬일땐 무조건 None </summary>
            None = -1,
            Wait,
            //Move,
            Battle,
        }

        public enum EUnitAttackType
        {
            None,
            Melee,
            Ranged,
            MaxCount,
        }
        //public enum EBaseFSM
        //{
        //    StateStart,
        //    StateUpdate,
        //    StateEnd,
        //}
        public enum EUnitState
        {
            None,
            Idle,
            Move,
            Attack,
            Dead,
            MaxCount,
        }
        public enum EUnitType
        {
            None,
            Slime,
            Spike,
        }

        public enum ESpawnType
        {
            None,
            Slime_R,
            Slime_S,
        }

        // 임시 통합 애니메이션 키값
        public enum EAnimationKey
        {
            IdleBlend,
            AttackBlend,
            Die,
            Hit,
            Move,
        }
        public enum ELayerMask
        {
            Default,
            Obstacle,
        }

        public enum EUnitPrefabKey
        {
            None,
            Unit_Slime_R,
            Unit_Slime_R_B,
            Unit_Slime_R_P,
            Unit_Slime_R_R,
            Unit_Slime_R_Y,
            Unit_Slime_S,
            Unit_Slime_S_B,
            Unit_Slime_S_P,
            Unit_Slime_S_R,
            Unit_Slime_S_Y,
            MaxCount,
        }
        public enum ESpriteKey
        {
            None,
            Sprite_Slime_R, Sprite_Slime_R_B, Sprite_Slime_R_P, Sprite_Slime_R_R, Sprite_Slime_R_Y,
            Sprite_Slime_S, Sprite_Slime_S_B, Sprite_Slime_S_P, Sprite_Slime_S_R, Sprite_Slime_S_Y,
            MaxCount,
        }
        public enum ESoundKey
        {
            None,
            Attack,
            Hit,
            Die,
            MaxCount,
        }
        public enum EAtlasKey
        {
            None,
            SpriteAtlas_Unit,
        }
        public enum ELabelKey
        {
            None,
            local,
            cdn,
        }
        #endregion
    }
}