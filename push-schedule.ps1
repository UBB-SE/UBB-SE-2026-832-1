$ErrorActionPreference = "Stop"
Set-Location "d:\Anda\UBB-SE-2026-832-1"

function Wait-Until([DateTime]$target) {
    $now = Get-Date
    if ($now -lt $target) {
        $wait = ($target - $now).TotalSeconds
        Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Waiting $([math]::Round($wait))s until $($target.ToString('HH:mm:ss'))..."
        Start-Sleep -Seconds $wait
    }
}

$pr2PushTime = Get-Date -Year 2026 -Month 4 -Day 27 -Hour 20 -Minute 50 -Second 0
Wait-Until $pr2PushTime

Write-Host "`n=== Pushing integrate-nut-repos ===" -ForegroundColor Cyan
git push -u origin integrate-nut-repos
Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Repos branch pushed."

Start-Sleep -Seconds 5

Write-Host "`n=== Creating PR (repos) ===" -ForegroundColor Cyan
$pr2Body = @"
## Summary
- Add generic IRepository interface and NUT-specific repository interfaces
- Implement EF Core repositories for NutUser, Meal, and MealPlan
- Add join entities (Favorite, MealsIngredient, MealPlanMeal) for many-to-many relationships
- Register all NUT repositories in DI container

Closes #49, closes #50
"@
$pr2Body | Out-File -FilePath "pr-body.md" -Encoding utf8
gh pr create --base main --head integrate-nut-repos --title "add NUT repository layer" --body-file pr-body.md
Remove-Item pr-body.md
Write-Host "[$(Get-Date -Format 'HH:mm:ss')] PR created."
Write-Host "`n=== Done ===" -ForegroundColor Green
