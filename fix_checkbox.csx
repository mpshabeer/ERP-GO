using System;
using System.IO;

string file = @"d:\ShabeerWorkingFolder\ERPGO\ERPGoEdition\ERPGoEdition.Shared\Pages\ItemMaster.razor";
string content = File.ReadAllText(file);
string oldStr = "@bind-Value=\"currentItem.IsMultiUnit\" Color=\"Color.Primary\" Dense=\"true\" Size=\"Size.Small\"";
string newStr = "Value=\"currentItem.IsMultiUnit\" ValueChanged=\"@((bool v) => OnIsMultiUnitChanged(v))\" Color=\"Color.Primary\" Dense=\"true\" Size=\"Size.Small\"";
string result = content.Replace(oldStr, newStr);
if (result == content)
{
    Console.WriteLine("WARNING: No replacement made - string not found");
}
else
{
    File.WriteAllText(file, result);
    Console.WriteLine("Done - replacement made successfully");
}
