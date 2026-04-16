using System.Globalization;

namespace Data.Text
{
    public static class UIValueBuilder
    {
        public static string Health(int current, int max) => $"{current} / {max}";
        public static string Progress(int current, int max) => $"{current} / {max}";
        public static string Coin(int value) => $"× {value}";
        public static string Level(int level) => $"Lv.{level}";
        public static string Timer(int sec) => sec.ToString(CultureInfo.InvariantCulture);
        public static string Price(int price) => price.ToString(CultureInfo.InvariantCulture);

        public static string Lock(bool locked) => locked ? "解锁" : "锁定";
    }
}