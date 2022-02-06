using DigitalSkynet.Boilerplate.Data.Interfaces;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalSkynet.Boilerplate.Data.Repositories
{
    public class DebugRepository : IDebugRepository
    {
        private readonly IStringLocalizer<DebugRepository> _stringLocalizer;
        public DebugRepository(IStringLocalizer<DebugRepository> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }

        public DebugModel GetDebugModel()
        {
            var debugModel = new DebugModel()
            {
                Question = _stringLocalizer["Question"].Value,
                Answer = _stringLocalizer["Answer"].Value
            };
            return debugModel;
        }
    }

    public class DebugModel
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
