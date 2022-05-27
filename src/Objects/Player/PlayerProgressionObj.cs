namespace ValNet.Objects.Player;

    public class Progress
    {
        public int Level { get; set; }
        public int XP { get; set; }
    }

    public class StartProgress
    {
        public int Level { get; set; }
        public int XP { get; set; }
    }

    public class EndProgress
    {
        public int Level { get; set; }
        public int XP { get; set; }
    }

    public class XPSource
    {
        public string ID { get; set; }
        public int Amount { get; set; }
    }

    public class XPMultiplier
    {
        public string ID { get; set; }
        public int Value { get; set; }
    }

    public class History
    {
        public string ID { get; set; }
        public DateTime MatchStart { get; set; }
        public StartProgress StartProgress { get; set; }
        public EndProgress EndProgress { get; set; }
        public int XPDelta { get; set; }
        public List<XPSource> XPSources { get; set; }
        public List<XPMultiplier> XPMultipliers { get; set; }
    }

    public class PlayerProgressionObj
    {
        public int Version { get; set; }
        public string Subject { get; set; }
        public Progress Progress { get; set; }
        public List<History> History { get; set; }
        public string LastTimeGrantedFirstWin { get; set; }
        public string NextTimeFirstWinAvailable { get; set; }
    }
