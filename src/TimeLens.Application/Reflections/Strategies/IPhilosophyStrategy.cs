using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeLens.Domain.Entities;

namespace TimeLens.Application.Reflections.Strategies
{
    public interface IPhilosophyStrategy
    {
        PhilosophyType Philosophy { get; }
        string BuildSystemPrompt(string journalContent);
    }
}
