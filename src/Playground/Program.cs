using PlatynUI.Runtime;

foreach (var v in Finder.Evaluate(null, "app:Application/@RuntimeId"))
{
    if (v is Array array)
    {
        foreach (var item in array)
        {
            Console.WriteLine(item);
        }
    }
    else
    {
        Console.WriteLine(v);
    }
}
