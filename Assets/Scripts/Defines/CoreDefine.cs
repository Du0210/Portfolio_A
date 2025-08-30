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
            LobbyScene,
            BattleScene,
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
        #endregion
    }
}