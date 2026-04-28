using UnityEngine;
using System;
using System.Collections.Generic;

namespace Turn_Based_Game
{
    public class MonsterSpawner : MonoBehaviour
    {
        [SerializeField] private List<GameObject> monsterPrefabs = new List<GameObject>();
        [SerializeField] private Transform spawnParent;
        [SerializeField] private int minMonsterCount = 2;
        [SerializeField] private int maxMonsterCount = 4;

        private List<Func<Monster>> monsterTemplates;

        private void Awake()
        {
            InitializeMonsterTemplates();
        }

        private void InitializeMonsterTemplates()
        {
            monsterTemplates = new List<Func<Monster>>()
            {
                () => new Slime("슬라임", 50, 10, 5),
                () => new Orc("오크", 80, 15, 10),
                () => new Dragon("드래곤", 120, 20, 15)
                // 새로운 몬스터 추가 시 아래에 계속 입력
            };
        }

        public Monster[] SpawnEnemies()
        {
            int monsterCount = UnityEngine.Random.Range(minMonsterCount, maxMonsterCount + 1);

            Monster[] monsters = new Monster[monsterCount];

            for (int i = 0; i < monsterCount; i++)
            {
                monsters[i] = CreateRandomEnemy(i);
            }

            return monsters;
        }

        public Monster CreateRandomEnemy(int index)
        {
            int type = UnityEngine.Random.Range(0, monsterTemplates.Count);
            Monster monster = monsterTemplates[type]();
            monster.Index = index;

            // 프리팹에 해당하는 MonsterView 인스턴스화 및 데이터 연결
            if (index < monsterPrefabs.Count && monsterPrefabs[index] != null)
            {
                GameObject monsterGo = Instantiate(monsterPrefabs[index], spawnParent);
                MonsterView monsterView = monsterGo.GetComponent<MonsterView>();

                if (monsterView != null)
                {
                    monsterView.Init(monster);
                }
            }

            return monster;
        }
    }
}