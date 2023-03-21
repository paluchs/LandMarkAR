using System;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    [Serializable]
    public class Experiment 
    {
        // Experiment Information
        [SerializeField] private string name;
        public string Name => name;

        public DateTime Created => _created;

        public List<string> createdAnchorIDs;
        private readonly DateTime _created;

        public Experiment(string name)
        {
            this.name = name;
            _created = DateTime.Now;
            createdAnchorIDs = new List<string>();
        }
    }
}
