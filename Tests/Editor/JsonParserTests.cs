using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using NUnit.Framework;

public class JsonParserTests
{
    private CultureInfo originalCulture;

    [SetUp]
    public void SetUp()
    {
        originalCulture = Thread.CurrentThread.CurrentCulture;
    }

    [TearDown]
    public void TearDown()
    {
        Thread.CurrentThread.CurrentCulture = originalCulture;
    }

    [Test]
    public void EscapesSpecialCharactersInStrings()
    {
        var json = JsonParser.Serialize(new Dictionary<string, object>
        {
            { "text", "He said \"hi\"\\ \n\r\t\b\f\u0001" },
        });

        Assert.AreEqual("{\"text\":\"He said \\\"hi\\\"\\\\ \\n\\r\\t\\b\\f\\u0001\"}", json);
    }

    [Test]
    public void EscapesKeys()
    {
        var json = JsonParser.Serialize(new Dictionary<string, object> { { "a\"b", 1 } });

        Assert.AreEqual("{\"a\\\"b\":1}", json);
    }

    [Test]
    public void KeepsUnicodeCharacters()
    {
        var json = JsonParser.Serialize(new Dictionary<string, object> { { "name", "خرید" } });

        Assert.AreEqual("{\"name\":\"خرید\"}", json);
    }

    [TestCase("fa-IR")]
    [TestCase("de-DE")]
    [TestCase("ru-RU")]
    [TestCase("tr-TR")]
    public void WritesNumbersWithInvariantCulture(string cultureName)
    {
        SetCultureOrIgnore(cultureName);

        var json = JsonParser.Serialize(new Dictionary<string, object>
        {
            { "d", 9.99 },
            { "f", 1.5f },
            { "m", 12.25m },
            { "i", 1200000 },
            { "l", 5000000000L },
        });

        Assert.AreEqual("{\"d\":9.99,\"f\":1.5,\"m\":12.25,\"i\":1200000,\"l\":5000000000}", json);
    }

    [Test]
    public void WritesNonFiniteNumbersAsNull()
    {
        var json = JsonParser.Serialize(new Dictionary<string, object>
        {
            { "nan", double.NaN },
            { "inf", float.PositiveInfinity },
        });

        Assert.AreEqual("{\"nan\":null,\"inf\":null}", json);
    }

    [Test]
    public void OmitsNullValues()
    {
        var json = JsonParser.Serialize(new Dictionary<string, object>
        {
            { "a", null },
            { "b", true },
        });

        Assert.AreEqual("{\"b\":true}", json);
    }

    [Test]
    public void SerializesNestedObjectsAndLists()
    {
        var json = JsonParser.Serialize(new Dictionary<string, object>
        {
            { "obj", new Dictionary<string, object> { { "x", false } } },
            { "list", new List<object> { 1, "two", null } },
        });

        Assert.AreEqual("{\"obj\":{\"x\":false},\"list\":[1,\"two\",null]}", json);
    }

    [Test]
    public void SerializesNotificationPayload()
    {
        var json = WiseTrack.BuildNotificationPayloadJson(new Dictionary<string, string>
        {
            { "wt_type", "uninstall" },
            { "body", "{\"nested\":\"json\"}" },
            { "empty", null },
        });

        Assert.AreEqual("{\"wt_type\":\"uninstall\",\"body\":\"{\\\"nested\\\":\\\"json\\\"}\",\"empty\":\"\"}", json);
    }

    private static void SetCultureOrIgnore(string cultureName)
    {
        try
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);
        }
        catch (CultureNotFoundException)
        {
            Assert.Ignore("Culture " + cultureName + " is not available in this runtime");
        }
    }
}
