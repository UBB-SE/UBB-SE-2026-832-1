using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Models
{
    public class NutritionSyncOptions
    {
        private const string DefaultEndpoint = "http://localhost:5000/api/nutrition/sync";

        public string Endpoint { get; set; } = DefaultEndpoint;
    }
}
