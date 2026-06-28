using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLens.Domain.Entities;
using TimeLens.Application.Reflections.Strategies;

namespace TimeLens.Application.Reflections
{
    public class PhilosophyStrategyFactory
    {
        private readonly IEnumerable<IPhilosophyStrategy> _strategies;

        public PhilosophyStrategyFactory(IEnumerable<IPhilosophyStrategy> strategies)
        {
            _strategies = strategies;
        }

        public IPhilosophyStrategy GetStrategy(PhilosophyType philosophy)
            => _strategies.FirstOrDefault(s => s.Philosophy == philosophy)
                ?? throw new ArgumentException(
                    $"Không tìm thấy strategy cho philosophy: {philosophy}");
        
    }
}
