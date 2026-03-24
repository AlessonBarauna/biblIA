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

function Download-Json([string]$url) {
    try {
        Write-Host "  GET $url" -ForegroundColor DarkGray
        return Invoke-RestMethod $url
    } catch {
        Write-Host "  Falhou: $_" -ForegroundColor Yellow
        return $null
    }
}

# --- 1. Baixar JSONs ---
# Tenta thiagobodruk/biblia (branch main, depois master como fallback)
$base1 = "https://raw.githubusercontent.com/thiagobodruk/biblia/main/json"
$base2 = "https://raw.githubusercontent.com/thiagobodruk/biblia/master/json"

Write-Host "[1/4] Baixando KJV..." -ForegroundColor Cyan
$kjv = Download-Json "$base1/en_kjv.json"
if ($null -eq $kjv) { $kjv = Download-Json "$base2/en_kjv.json" }
if ($null -eq $kjv) {
    Write-Error "Nao foi possivel baixar o KJV. Verifique sua conexao com a internet."
    exit 1
}

Write-Host "[2/4] Baixando ACF..." -ForegroundColor Cyan
$acf = Download-Json "$base1/pt_acf.json"
if ($null -eq $acf) { $acf = Download-Json "$base2/pt_acf.json" }
if ($null -eq $acf) {
    Write-Warning "ACF nao encontrado. Versiculos serao importados apenas com KJV."
}

# --- 2. Determinar range ---
switch ($Testament.ToUpper()) {
    "NT"  { $start = 39; $end = 65 }
    "OT"  { $start = 0;  $end = 38 }
    "ALL" { $start = 0;  $end = 65 }
    default { Write-Error "Testament deve ser NT, OT ou ALL"; exit 1 }
}

# --- 3. Construir lista de versiculos ---
Write-Host "[3/4] Construindo lista ($Testament)..." -ForegroundColor Cyan

$allVerses = [System.Collections.Generic.List[hashtable]]::new()

for ($i = $start; $i -le $end; $i++) {
    $orderIndex = $i + 1
    $kjvBook    = $kjv[$i]
    $acfBook    = if ($acf) { $acf[$i] } else { $null }

    if ($null -eq $kjvBook) {
        Write-Warning "Livro indice $i nao encontrado no KJV, pulando."
        continue
    }

    $numChapters = $kjvBook.chapters.Count

    for ($chIdx = 0; $chIdx -lt $numChapters; $chIdx++) {
        $kjvVerses = $kjvBook.chapters[$chIdx]
        $acfVerses = if ($acfBook) { $acfBook.chapters[$chIdx] } else { $null }
        $numVerses = $kjvVerses.Count

        for ($vIdx = 0; $vIdx -lt $numVerses; $vIdx++) {
            $textACF = if ($acfVerses -and $vIdx -lt $acfVerses.Count) { $acfVerses[$vIdx] } else { "" }

            $allVerses.Add(@{
                bookOrderIndex = $orderIndex
                chapter        = $chIdx + 1
                verse          = $vIdx + 1
                textKJV        = $kjvVerses[$vIdx]
                textACF        = $textACF
            })
        }
    }
}

$total   = $allVerses.Count
$batches = [Math]::Ceiling($total / $BatchSize)
Write-Host "  $total versiculos, $batches lotes de $BatchSize." -ForegroundColor Green

# --- 4. Importar em lotes ---
Write-Host "[4/4] Importando..." -ForegroundColor Cyan

$imported = 0
$skipped  = 0

for ($b = 0; $b -lt $batches; $b++) {
    $s     = $b * $BatchSize
    $e     = [Math]::Min($s + $BatchSize - 1, $total - 1)
    $batch = $allVerses[$s..$e]
    $body  = $batch | ConvertTo-Json -Depth 3 -Compress

    try {
        $result = Invoke-RestMethod `
            -Method POST `
            -Uri "$ApiUrl/api/bible/import" `
            -ContentType "application/json; charset=utf-8" `
            -Body ([System.Text.Encoding]::UTF8.GetBytes($body))

        $imported += $result.imported
        $skipped  += $result.skipped
        $pct = [Math]::Round(($b + 1) / $batches * 100)
        Write-Host "  Lote $($b+1)/$batches ($pct%) - ok: $($result.imported), pulados: $($result.skipped)" -ForegroundColor Gray
    }
    catch {
        Write-Error "Erro no lote $($b+1): $_"
    }
}

Write-Host ""
Write-Host "=== Importacao concluida ===" -ForegroundColor Green
Write-Host "  Total    : $total"           -ForegroundColor Green
Write-Host "  Importado: $imported"        -ForegroundColor Green
Write-Host "  Pulados  : $skipped"         -ForegroundColor Green
