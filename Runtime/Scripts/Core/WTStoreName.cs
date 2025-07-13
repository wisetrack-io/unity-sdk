using System;
using System.Collections.Generic;

namespace WiseTrack.Runtime
{
    public class WTAndroidStore
    {
        public static readonly WTAndroidStore PlayStore = new("playstore");
        public static readonly WTAndroidStore CafeBazaar = new("cafebazaar");
        public static readonly WTAndroidStore Myket = new("myket");
        public static readonly WTAndroidStore Other = new("other");

        public string Name { get; }

        private WTAndroidStore(string name)
        {
            Name = name;
        }

        public static WTAndroidStore Custom(string name) => new(name);

        public static WTAndroidStore FromString(string value)
        {
            value = value.ToLower();
            if (value == PlayStore.Name) return PlayStore;
            if (value == CafeBazaar.Name) return CafeBazaar;
            if (value == Myket.Name) return Myket;
            if (value == Other.Name) return Other;
            return Custom(value);
        }

        public static List<WTAndroidStore> Values => new List<WTAndroidStore>
        {
            PlayStore, CafeBazaar, Myket, Other
        };

        public override bool Equals(object obj)
        {
            return obj is WTAndroidStore other && Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class WTIOSStore
    {
        public static readonly WTIOSStore AppStore = new("appstore");
        public static readonly WTIOSStore Sibche = new("sibche");
        public static readonly WTIOSStore Sibapp = new("sibapp");
        public static readonly WTIOSStore Anardoni = new("anardoni");
        public static readonly WTIOSStore Sibirani = new("sibirani");
        public static readonly WTIOSStore Sibjo = new("sibjo");
        public static readonly WTIOSStore Other = new("other");

        public string Name { get; }

        private WTIOSStore(string name)
        {
            Name = name;
        }

        public static WTIOSStore Custom(string name) => new(name);

        public static WTIOSStore FromString(string value)
        {
            value = value.ToLower();
            if (value == AppStore.Name) return AppStore;
            if (value == Sibche.Name) return Sibche;
            if (value == Sibapp.Name) return Sibapp;
            if (value == Anardoni.Name) return Anardoni;
            if (value == Sibirani.Name) return Sibirani;
            if (value == Sibjo.Name) return Sibjo;
            if (value == Other.Name) return Other;
            return Custom(value);
        }

        public static List<WTIOSStore> Values => new List<WTIOSStore>
        {
            AppStore, Sibche, Sibapp, Anardoni, Sibirani, Sibjo, Other
        };

        public override bool Equals(object obj)
        {
            return obj is WTIOSStore other && Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}