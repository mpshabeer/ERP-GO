$filePath = "d:\ShabeerWorkingFolder\ERPGO\ERPGoEdition\ERPGoEdition.Shared\Pages\ItemMaster.razor"
$text = [System.IO.File]::ReadAllText($filePath)
$from = '@bind-Value="currentItem.IsMultiUnit"'
$to = 'Value="currentItem.IsMultiUnit" ValueChanged="@((bool v) => OnIsMultiUnitChanged(v))"'
$newText = $text.Replace($from, $to)
if ($text -eq $newText) {
    Write-Host "NOT FOUND - no change made"
}
else {
    [System.IO.File]::WriteAllText($filePath, $newText)
    Write-Host "SUCCESS - replacement done"
}
