param(
    [string]$ApiUrl    = "http://localhost:5239",
    [string]$Testament = "NT",
    [int]   $BatchSize = 1000
)

# Uso:
#   .\Import-Bible.ps1               # NT (padrao)
#   .\Import-Bible.ps1 -Testament ALL
#   .\Import-Bible.ps1 -Testament OT

$ErrorActionPreference = "Stop"

# --- 1. Baixar JSONs ---
$baseUrl = "https://raw.githubusercontent.com/thiagobodruk/biblia/master/json"

Write-Host "[BibleIA] Baixando KJV..." -ForegroundColor Cyan
$kjv = Invoke-RestMethod "$baseUrl/en_kjv.json"

Write-Host "[BibleIA] Baixando ACF..." -ForegroundColor Cyan
$acf = Invoke-RestMethod "$baseUrl/pt_acf.json"

# --- 2. Determinar range ---
# JSONs em ordem canonica: indice 0 = Genesis (orderIndex=1) ... indice 65 = Apocalipse (orderIndex=66)
switch ($Testament.ToUpper()) {
    "NT"  { $start = 39; $end = 65 }
    "OT"  { $start = 0;  $end = 38 }
    "ALL" { $start = 0;  $end = 65 }
    default { Write-Error "Testament deve ser NT, OT ou ALL"; exit 1 }
}

# --- 3. Construir lista de versiculos ---
Write-Host "[BibleIA] Construindo lista ($Testament)..." -ForegroundColor Cyan

$allVerses = [System.Collections.Generic.List[hashtable]]::new()

for ($i = $start; $i -le $end; $i++) {
    $orderIndex = $i + 1
    $kjvBook    = $kjv[$i]
    $acfBook    = $acf[$i]

    if ($null -eq $kjvBook) {
        Write-Warning "Livro indice $i nao encontrado, pulando."
        continue
    }

    $numChapters = $kjvBook.chapters.Count

    for ($chIdx = 0; $chIdx -lt $numChapters; $chIdx++) {
        $chapterNum = $chIdx + 1
        $kjvVerses  = $kjvBook.chapters[$chIdx]
        $acfVerses  = if ($acfBook) { $acfBook.chapters[$chIdx] } else { $null }
        $numVerses  = $kjvVerses.Count

        for ($vIdx = 0; $vIdx -lt $numVerses; $vIdx++) {
            $textACF = if ($acfVerses -and $vIdx -lt $acfVerses.Count) { $acfVerses[$vIdx] } else { "" }

            $allVerses.Add(@{
                bookOrderIndex = $orderIndex
                chapter        = $chapterNum
                verse          = $vIdx + 1
                textKJV        = $kjvVerses[$vIdx]
                textACF        = $textACF
            })
        }
    }
}

$total   = $allVerses.Count
$batches = [Math]::Ceiling($total / $BatchSize)
Write-Host "[BibleIA] $total versiculos em $batches lotes." -ForegroundColor Green

# --- 4. Importar em lotes ---
$imported = 0
$skipped  = 0

for ($b = 0; $b -lt $batches; $b++) {
    $startIdx = $b * $BatchSize
    $endIdx   = [Math]::Min($startIdx + $BatchSize - 1, $total - 1)
    $batch    = $allVerses[$startIdx..$endIdx]
    $body     = $batch | ConvertTo-Json -Depth 3 -Compress

    try {
        $result = Invoke-RestMethod `
            -Method POST `
            -Uri "$ApiUrl/api/bible/import" `
            -ContentType "application/json; charset=utf-8" `
            -Body ([System.Text.Encoding]::UTF8.GetBytes($body))

        $imported += $result.imported
        $skipped  += $result.skipped
        $pct = [Math]::Round(($b + 1) / $batches * 100)
        Write-Host "  Lote $($b+1)/$batches ($pct%) - importados: $($result.imported), pulados: $($result.skipped)" -ForegroundColor Gray
    }
    catch {
        Write-Error "Erro no lote $($b+1): $_"
    }
}

# --- 5. Resumo ---
Write-Host ""
Write-Host "=== Importacao concluida ===" -ForegroundColor Green
Write-Host "  Total    : $total"          -ForegroundColor Green
Write-Host "  Importado: $imported"       -ForegroundColor Green
Write-Host "  Pulados  : $skipped"        -ForegroundColor Green
