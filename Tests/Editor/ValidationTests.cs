using System.Collections.Generic;
using NUnit.Framework;

public class ValidationTests
{
    [Test]
    public void ConfigRequiresAppTokenAndClientSecret()
    {
        Assert.AreEqual(2, WTValidation.ValidateConfig(new WTInitialConfig()).Count);
        Assert.AreEqual(1, WTValidation.ValidateConfig(new WTInitialConfig { AppToken = "a" }).Count);
        Assert.IsEmpty(WTValidation.ValidateConfig(new WTInitialConfig { AppToken = "a", ClientSecret = "b" }));
        Assert.IsNotEmpty(WTValidation.ValidateConfig(null));
    }

    [Test]
    public void EventNameLimit()
    {
        Assert.IsEmpty(WTValidation.ValidateEvent(WTEvent.DefaultEvent(new string('a', 50))));
        Assert.IsNotEmpty(WTValidation.ValidateEvent(WTEvent.DefaultEvent(new string('a', 51))));
        Assert.IsNotEmpty(WTValidation.ValidateEvent(WTEvent.DefaultEvent("")));
    }

    [Test]
    public void ScreenNameLimit()
    {
        Assert.IsEmpty(WTValidation.ValidateScreen(new WTScreen(new string('a', 150))));
        Assert.IsNotEmpty(WTValidation.ValidateScreen(new WTScreen(new string('a', 151))));
    }

    [Test]
    public void ParameterLimits()
    {
        var valid = new Dictionary<string, WTParamValue>
        {
            { new string('k', 50), WTParamValue.FromString(new string('v', 100)) },
            { "number", WTParamValue.FromNumber(1) },
        };
        Assert.IsEmpty(WTValidation.ValidateEvent(WTEvent.DefaultEvent("e", valid)));

        var longKey = new Dictionary<string, WTParamValue> { { new string('k', 51), WTParamValue.FromBoolean(true) } };
        Assert.IsNotEmpty(WTValidation.ValidateEvent(WTEvent.DefaultEvent("e", longKey)));

        var longValue = new Dictionary<string, WTParamValue> { { "k", WTParamValue.FromString(new string('v', 101)) } };
        Assert.IsNotEmpty(WTValidation.ValidateScreen(new WTScreen("s", parameters: longValue)));
    }
}
