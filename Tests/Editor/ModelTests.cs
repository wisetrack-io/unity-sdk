using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using NUnit.Framework;

public class ModelTests
{
    [TestCase(WTScreenType.Page, "page")]
    [TestCase(WTScreenType.Dialog, "dialog")]
    [TestCase(WTScreenType.BottomSheet, "bottom_sheet")]
    [TestCase(WTScreenType.Other, "other")]
    public void ScreenTypeMatchesNativeIdentifier(WTScreenType type, string expected)
    {
        var map = new WTScreen("menu", type).ToMap();

        Assert.AreEqual(expected, map["type"]);
    }

    [Test]
    public void ScreenMapContainsParamsAndFlags()
    {
        var screen = new WTScreen(
            name: "shop",
            type: WTScreenType.Page,
            displayName: "Shop",
            parameters: new Dictionary<string, WTParamValue> { { "id", WTParamValue.FromString("sword") } },
            isAuto: true,
            trigger: "scene_load");

        var json = JsonParser.Serialize(screen.ToMap());

        Assert.AreEqual(
            "{\"type\":\"page\",\"name\":\"shop\",\"display_name\":\"Shop\",\"trigger\":\"scene_load\",\"is_auto\":true,\"params\":{\"id\":\"sword\"}}",
            json);
    }

    [Test]
    public void RevenueEventMapUsesInvariantNumbersOnPersianLocale()
    {
        var original = Thread.CurrentThread.CurrentCulture;
        try
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("fa-IR");
            }
            catch (CultureNotFoundException)
            {
                Assert.Ignore("fa-IR is not available in this runtime");
            }

            var wtEvent = WTEvent.RevenueEvent("purchase", WTRevenueCurrency.IRR, 1250.5);
            var json = JsonParser.Serialize(wtEvent.ToMap());

            Assert.AreEqual("{\"type\":\"revenue\",\"name\":\"purchase\",\"revenue\":1250.5,\"currency\":\"IRR\"}", json);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = original;
        }
    }

    [Test]
    public void DefaultEventMapHasNoRevenueFields()
    {
        var map = WTEvent.DefaultEvent("open").ToMap();

        Assert.AreEqual("default", map["type"]);
        Assert.IsFalse(map.ContainsKey("revenue"));
        Assert.IsFalse(map.ContainsKey("currency"));
    }

    [TestCase("PlayStore", "playstore")]
    [TestCase("cafebazaar", "cafebazaar")]
    [TestCase("MYKET", "myket")]
    [TestCase("my-store", "my-store")]
    [TestCase("", "other")]
    [TestCase(null, "other")]
    public void AndroidStoreFromString(string input, string expected)
    {
        Assert.AreEqual(expected, WTAndroidStore.FromString(input).Name);
    }

    [Test]
    public void ConfigMapUsesLowerCaseIdentifiers()
    {
        var map = new WTInitialConfig
        {
            AppToken = "token",
            ClientSecret = "secret",
            UserEnvironment = WTUserEnvironment.Production,
            AndroidStore = WTAndroidStore.CafeBazaar,
        }.ToMap();

        Assert.AreEqual("production", map["user_environment"]);
        Assert.AreEqual("cafebazaar", map["android_store_name"]);
        Assert.AreEqual(WTResources.SdkVersion, map["sdk_version"]);
    }
}
