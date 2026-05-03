namespace ClassLibrary.Models
{
    public class NutritionSyncOptions
    {
        private const string DEFAULT_ENDPOINT = "http://localhost:5000/api/nutrition/sync";

        public string Endpoint { get; set; } = DEFAULT_ENDPOINT;
    }
}
