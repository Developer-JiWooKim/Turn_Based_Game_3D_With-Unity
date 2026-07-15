using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.MyAssets.Scripts.Scriptable;

namespace Assets.MyAssets.Scripts.Manager
{

    /// <summary>
    /// 파티 로스터의 슬롯 단위 래퍼. 동일한 PlayerData(캐릭터)가 로스터에 중복으로 들어갈 수 있어
    /// (파티 시너지/영입 중복 허용), PlayerData 자체를 키로 상태(HP)를 추적할 수 없어 슬롯을 개별 객체로 감싼다.
    /// </summary>
    public class PartyMember
    {
        public PlayerData Data;
        public int CurrentHp;

        public PartyMember(PlayerData data)
        {
            Data = data;
            CurrentHp = data.playerStat.Hp;
        }
    }

    public class PlayerDataManager : Singleton<PlayerDataManager>
    {

        private const int MaxPartySize = 4;

        [SerializeField] private List<PlayerData> _partyRoster = new List<PlayerData>(); // SettingScene 없이 BattleScene을 바로 테스트할 때의 기본 구성

        private List<PlayerData>  _originalRoster;   // 최초 구성 스냅샷 (게임오버 시 복원용)
        private List<PartyMember> _runtimeRoster;    // 실제 런타임 로스터 (슬롯별 HP 등 상태 보유)

        public List<PartyMember> PartyRoster => _runtimeRoster;

        protected override void Awake()
        {
            base.Awake();
            if (!IsValidInstance) return;

            DontDestroyOnLoad(gameObject);
            Initialize();
        }

        protected override void OnDestroy()
        {
            // base.OnDestroy();
        }

        private void Initialize()
        {
            if (_partyRoster.Count > MaxPartySize)
            {
                Debug.LogWarning($"파티 로스터가 최대 인원({MaxPartySize})을 초과해 잘라냅니다.");
                _partyRoster.RemoveRange(MaxPartySize, _partyRoster.Count - MaxPartySize);
            }

            _originalRoster = new List<PlayerData>(_partyRoster);
            _runtimeRoster  = _partyRoster.ConvertAll(data => new PartyMember(data));
        }

        // SettingScene에서 시작 캐릭터를 고른 뒤 파티 로스터를 그 1명으로 확정
        public void SetStartingCharacter(PlayerData data)
        {
            _partyRoster = new List<PlayerData> { data };
            Initialize();
        }

        // 캐릭터가 전투 중 사망하면 파티에서 영구 추방
        public void RemoveFromRoster(PartyMember member)
        {
            _runtimeRoster.Remove(member);
        }

        // 게임 오버(전멸) 시 다음 런을 위해 원래 구성 + 풀피로 복원
        public void ResetRoster()
        {
            _runtimeRoster = _originalRoster.ConvertAll(data => new PartyMember(data));
        }

        // 로그라이크 "파티원 영입" 선택지 - 파티가 여유 있을 때 즉시 합류
        public void AddToRoster(PlayerData data)
        {
            if (_runtimeRoster.Count >= MaxPartySize)
            {
                Debug.LogWarning("파티가 이미 가득 찼습니다.");
                return;
            }

            _runtimeRoster.Add(new PartyMember(data));
        }

        // 로그라이크 "파티원 영입" 선택지 - 파티가 꽉 찼을 때 교체 대상을 지정해 교체
        public void ReplaceInRoster(PartyMember oldMember, PlayerData newData)
        {
            int index = _runtimeRoster.IndexOf(oldMember);
            if (index < 0) return;

            _runtimeRoster[index] = new PartyMember(newData);
        }

        // 로그라이크 "회복" 선택지 - 파티 전체 HP를 퍼센트만큼 즉시 회복
        public void HealAllByPercent(int percent)
        {
            foreach (var member in _runtimeRoster)
            {
                int maxHp = member.Data.playerStat.Hp;
                member.CurrentHp = Math.Clamp(member.CurrentHp + maxHp * percent / 100, 0, maxHp);
            }
        }

        // 스테이지 승리 후 생존자의 HP를 다음 스테이지로 이어받기 위해 기록.
        // 런 버프로 실제 최대체력이 base playerStat.Hp보다 커져 있을 수 있어 여기서는 상한 클램프를 하지 않는다
        // (다음 전투 시작 시 BattleUnit.SetCurrentHp가 그 시점의 실제 최대체력 기준으로 다시 클램프한다).
        public void SetCurrentHp(PartyMember member, int hp)
        {
            member.CurrentHp = Math.Max(hp, 0);
        }
    }

}
