using System.Text.Json.Serialization;

namespace ValNet.Objects.Inventory;

public class PlayerInventory
{
    public string Subject { get; set; }
    public int Version { get; set; }
    public List<Gun> Guns { get; set; }
    public List<Spray> Sprays { get; set; }
    [JsonPropertyName("Identity")]
    public Identity PlayerData { get; set; }
    public bool Incognito { get; set; }
    
    public class Identity
    {
        public string PlayerCardID { get; set; }
        public string PlayerTitleID { get; set; }
        public int AccountLevel { get; set; }
        public string PreferredLevelBorderID { get; set; }
        public bool HideAccountLevel { get; set; }
    }
    
    public class Gun
    {
        public string ID { get; set; }
        public string SkinID { get; set; }
        public string SkinLevelID { get; set; }
        public string ChromaID { get; set; }
        public List<object> Attachments { get; set; }
    }

    public class Spray
    {
        public string EquipSlotID { get; set; }
        public string SprayID { get; set; }
        public object SprayLevelID { get; set; }
    }
}