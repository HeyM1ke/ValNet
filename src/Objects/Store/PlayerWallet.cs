using System.Text.Json.Serialization;

namespace ValNet.Objects.Store;

public class PlayerWallet
{
    public BalanceSheet Balances { get; set; }
    public class BalanceSheet
    {
        [JsonPropertyName("85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741")]
        public int VP { get; set; }

        [JsonPropertyName("e59aa87c-4cbf-517a-5983-6e81511be9b7")]
        public int RP { get; set; }

        [JsonPropertyName("f08d4ae3-939c-4576-ab26-09ce1f23bb37")]
        public int FREE_AGENT_TOKENS { get; set; }
    }

}