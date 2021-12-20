using System.Text.Json.Serialization;

namespace ValNet.Objects.Store;

public class PlayerStore
{
    public FeaturedBundle FeaturedBundle { get; set; }
    public DailySkinCarousel SkinsPanelLayout { get; set; }
    public NightMarket BonusStore { get; set; }
}

public class FeaturedBundle
{
    public Bundle Bundle { get; set; }
    public List<Bundle> Bundles { get; set; }
    public int BundleRemainingDurationInSeconds { get; set; }
}

public class Bundle
{
    public string ID { get; set; }
    public string DataAssetID { get; set; }
    public string CurrencyID { get; set; }
    public List<BundleItem> Items { get; set; }
    public int DurationRemainingInSeconds { get; set; }
    public bool WholesaleOnly { get; set; }
}

public class BundleItem
{
    [JsonPropertyName("Item")]
    public ItemOffer ItemData { get; set; }
    public int BasePrice { get; set; }
    public string CurrencyID { get; set; }
    public int DiscountPercent { get; set; }
    public int DiscountedPrice { get; set; }
    public bool IsPromoItem { get; set; }
}

public class DailySkinCarousel
{
    public List<string> SingleItemOffers { get; set; }
    public int SingleItemOffersRemainingDurationInSeconds { get; set; }
}

public class NightMarket
{
    [JsonPropertyName("BonusStoreOffers")]
    List<NightMarketOffer> NightMarketOffers { get; set; }
    
    [JsonPropertyName("BonusStoreRemainingDurationInSeconds")]
    public int NightMarketTimeRemainingInSeconds { get; set; }
    
    public class NightMarketOffer
    {
        public string BonusOfferID { get; set; }
        public StoreOffer Offer { get; set; }
        public int DiscountPercent { get; set; }
        public Cost DiscountCosts { get; set; }
        public bool IsSeen { get; set; }
    }
}

public class NightMarketOffer
{
    public string OfferID { get; set; }
    public bool IsDirectPurchase { get; set; }
    public string StartDate { get; set; }
    
    [JsonPropertyName("Cost")]
    public Cost OriginalCost { get; set; }
    public List<ItemOffer> Rewards { get; set; }

}

public class ItemOffer
{
    public string ItemTypeID { get; set; }
    public string ItemID { get; set; }
    public int Quantity { get; set; }
}

public class StoreOffer
{
    public string OfferID { get; set; }
    public bool IsDirectPurchase { get; set; }
    public string StartDate { get; set; }
    public Cost Cost { get; set; }
    public List<ItemOffer> Rewards { get; set; }
}

public class Cost
{
    [JsonPropertyName("85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741")]
    public int ValorantPointCost { get; set; }
}