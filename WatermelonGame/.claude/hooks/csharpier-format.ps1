$j = [Console]::In.ReadToEnd() | ConvertFrom-Json
$f = $j.tool_input.file_path
if ($f -match '\.cs$') {
    Set-Location 'C:\Users\hgy41\UnityClass\unity-ai-suika-hgy4152\WatermelonGame'
    dotnet csharpier format $f 2>$null
}
