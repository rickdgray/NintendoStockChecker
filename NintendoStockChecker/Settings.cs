using System.ComponentModel.DataAnnotations;

namespace NintendoStockChecker
{
    internal class Settings
    {
        [Required] public string PushoverUserKey { get; set; } = string.Empty;
        [Required] public string PushoverAppKey { get; set; } = string.Empty;
        public int PollRateInSeconds { get; set; } = 1;
    }
}
