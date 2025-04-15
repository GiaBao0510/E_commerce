function Show-ColorTree {
    param(
        [string]$Path = ".",
        [int]$Indent = 0
    )
    
    Get-ChildItem -Path $Path | Where-Object { $_.FullName -notmatch "\\(bin|obj)(\\|$)" } | ForEach-Object {
        $indentString = " " * $Indent
        if ($_.PSIsContainer) {
            Write-Host ($indentString + "|-- ") -NoNewline
            Write-Host $_.Name -ForegroundColor Blue
            Show-ColorTree -Path $_.FullName -Indent ($Indent + 4)
        } else {
            Write-Host ($indentString + "|-- ") -NoNewline
            Write-Host $_.Name -ForegroundColor Green
        }
    }
}

Show-ColorTree