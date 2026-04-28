using System.Reflection;

namespace TelegramBot.Domain.Constants;

public class IconsStrings
{
    public const string Robot = "🤖";
    public const string Spanner = "🔧";
    public const string Ok = "✅";
    public const string Ko = "❌";
    public const string CashBag = "💰";
    public const string System = "🟢";

    private static FieldInfo[] FieldInfos => typeof(IconsStrings).GetFields( 
            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
        .Where(fi => fi is { IsLiteral: true, IsInitOnly: false } && fi.FieldType == typeof(string))
        .ToArray();
    
    public static readonly string[] Names = FieldInfos
        .Select(fi => fi.Name)
        .ToArray();

    public string this[string name]
    {
        get
        {
            var field = FieldInfos.Single(e => e.Name == name);

            if(field.GetValue(this) is not string value)
                throw new KeyNotFoundException(name);
            return value;
        }
    }
}