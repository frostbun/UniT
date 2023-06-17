namespace ABelieveT
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using UniT.Extensions;
    using UnityEngine;

    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class Entity : MonoBehaviour
    {
        [field: SerializeField] public  int        Team { get; protected set; }
        [SerializeField]        private List<Stat> defaultStats = new();

        private readonly Dictionary<string, Stat>                                 stats     = new();
        private readonly List<Ability>                                            abilities = new();
        private          ReadOnlyDictionary<string, ReadOnlyCollection<StatView>> statViews;

        public void AddStat(Stat stat)
        {
            this.stats.Add(stat.Name, stat);
            this.statViews.GetOrDefault(stat.Name)?.ForEach(statView => statView.Init(stat));
        }

        public Stat GetStat(string statName)
        {
            return this.stats.GetOrDefault(statName);
        }

        public void AddAbility(Ability ability)
        {
            this.abilities.Add(ability);
            ability.OnCreate(this);
        }

        protected virtual void Awake()
        {
            this.statViews = this.GetComponentsInChildren<StatView>()
                                 .GroupBy(statView => statView.StatName)
                                 .ToDictionary(group => group.Key, group => group.ToList().AsReadOnly())
                                 .AsReadOnly();
            this.defaultStats.ForEach(this.AddStat);
        }

        protected virtual void Update()
        {
            this.abilities.ForEach(ability => ability.OnUpdate());
        }
    }
}