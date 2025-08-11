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
            /// <summary> ELobbySceneType </summary>
            OnOpenLobbyType,

            // Stage
            /// <summary> EStageType, int </summary>
            OnClickStageTile,

            // Unit
            OnSelectUnit,

            // Battle
            OnChangedBattleLoop,
            OnChangedBattleState,
            OnOpenFirstCard,
            OnOpenCard,
            OnSetFirstUnitSetting,
            OnSelectCard,
            OnDrawBattleTimer,
            OnDrawCost, 
            OnDrawWave,
            OnHideCard,
            OnMoveSpawnerNext,
            OnFollowCamUpdate,
            OnKillEnemy,
            OnKillPlayer,
            OnSpawnEnemy,
            /// <summary> bool </summary>
            OnOpenResultPanel,
            OnOpenBattleMenuPanel,
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
            StageData,
            UnitData,
            GoodsData,
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

        public enum EUnitType
        {
            Player,
            Enemy,
        }

        public enum EPlayerUnitType
        {
            Nurse,
            Knight,
            Panda,
            Elf,
            Student,
            Goth,
            Witch,
            Pirate,
            MaxCount,
        }

        public enum EProjectileType
        {
            None,
            Bullet_0,
        }

        public enum EEnemyUnitType
        {
            Mon_0, Mon_1, Mon_2, Mon_3, Mon_4,
            Mon_5,
            MaxCount,
        }

        public enum EUnitAttackType
        {
            None,
            Melee,
            Ranged,
            MaxCount,
        }
        public enum EBaseFSM
        {
            StateStart,
            StateUpdate,
            StateEnd,
        }
        public enum EUnitState
        {
            Idle,
            Move,
            Attack,
            Skill,
            Dead,
            MaxCount,
        }

        public enum EUnitPosition
        {
            Front,
            Middle,
            Back,
            MaxCount,
        }

        public enum ESpawnType
        {
            Player,
            Enemy,
            UI,
        }

        public enum ESpineSkinType
        {
            Knight,
            Pirate,
            Witch,
            Nerse,
            Regent,
            Panda,
            Goth,
            Elf,
        }

        public enum ESpineWpSlotType
        {
            E_Staff_09, // 칼
            E_Staff_18, // 후크
            E_Staff_19, // 별 지팡이
            E_Staff_07, // 주사기
            E_Staff_20, // 총
            E_Staff_03, // 메로나
            E_Staff_08, // 냥냥펀치
            E_Staff_04, // 전기톱
        }

        public enum EPlayerSpineAnimKey
        {
            /// <summary> 기본 </summary>
            IDLE,
            /// <summary> 근접 </summary>
            ATTACK6,
            /// <summary> 원거리 </summary>
            ATTACK2,
            /// <summary> 이동 </summary>
            FLY,
            /// <summary> 사망 </summary>
            DIE,
        }
        #endregion
    }
}