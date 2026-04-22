using System;
using System.Collections.Generic;
using System.Linq;
using TitanExpress.Models;

namespace TitanExpress.Systems
{
    public class StationManager
    {
        private readonly List<BaseStation> _stations = new List<BaseStation>();
        private int _nextId = 1;
        private readonly Random _random = new Random();

        public IReadOnlyList<BaseStation> Stations => _stations.AsReadOnly();

        public int TotalBandwidth => _stations.Where(s => s.IsActive).Sum(s => s.BandwidthProvided);

        public BaseStation DeployStation(string name, int x, int y)
        {
            var station = new BaseStation
            {
                Id = _nextId++,
                Name = name,
                X = x,
                Y = y
            };
            _stations.Add(station);
            return station;
        }

        public BaseStation GetStation(int id)
        {
            return _stations.FirstOrDefault(s => s.Id == id);
        }

        public void DamageStation(int id, int damage)
        {
            var station = _stations.FirstOrDefault(s => s.Id == id);
            if (station != null)
            {
                station.Health -= damage;
                if (station.Health <= 0)
                {
                    station.IsActive = false;
                    station.Health = 0;
                }
            }
        }

        public void RepairStation(int id, int repairAmount)
        {
            var station = _stations.FirstOrDefault(s => s.Id == id);
            if (station != null)
            {
                station.Health = Math.Min(100, station.Health + repairAmount);
                if (station.Health > 0)
                {
                    station.IsActive = true;
                }
            }
        }
    }
}