using System;
using System.Globalization;
using NEI.Data;
using NEI.DTOs;
using NEI.Models;

namespace NEI;

public class CloseApproachService
{

    private readonly AppDbContext _context;

    public CloseApproachService(AppDbContext context)
    {
        _context = context;
    }

    public void SyncCloseApproaches(Asteroid asteroid, NasaAsteroidDto neo)
    {
        if (neo.CloseApproachData == null) return;

        foreach (var approach in neo.CloseApproachData)
        {
            var approachDate = DateTime.Parse(approach.CloseApproachDate);

            if (_context.CloseApproaches.Any(c => c.AsteroidId == asteroid.Id && c.ApproachDate == approachDate))
                continue;

            _context.CloseApproaches.Add(new CloseApproach
            {
                AsteroidId = asteroid.Id,
                ApproachDate = approachDate,
                MissDistanceKm = decimal.Parse(approach.MissDistance.Kilometers, CultureInfo.InvariantCulture),
                RelativeVelocityKm = decimal.Parse(approach.RelativeVelocity.KilometersPerSecond, CultureInfo.InvariantCulture),
                OrbitingBody = approach.OrbitingBody
            });
        }
    }


}
