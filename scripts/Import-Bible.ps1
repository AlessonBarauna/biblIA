<#
.SYNOPSIS
    Importa versículos bíblicos (KJV + ACF) para o banco do BíblIA.

.DESCRIPTION
    Baixa os JSONs públicos de thiagobodruk/biblia (github.com/thiagobodruk/biblia)
    e chama POST /api/bible/import em lotes de 1000 versículos.
    A operação é idempotente — versículos já existentes são pulados.

.PARAMETER ApiUrl
    URL base da API. Padrão: http://localhost:5239

.PARAMETER Testament
    "NT" = apenas Novo Testamento (padrão)
    "OT" = apenas Antigo Testamento
    "ALL" = Bíblia completa

.PARAMETER BatchSize
    Versículos por requisição. Padrão: 1000

.EXAMPLE
    .\Import-Bible.ps1
    .\Import-Bible.ps1 -Testament ALL
    .\Import-Bible.ps1 -ApiUrl http://localhost:5239 -Testament NT
#>
param(
    [string]$ApiUrl    = "http://localhost:5239",
    [string]$Testament = "NT",
    [int]   $BatchSize = 1000
)

$ErrorActionPreference = "Stop"

# ── 1. Baixar JSONs ──────────────────────────────────────────────────────────
$baseUrl = "https://raw.githubusercontent.com/thiagobodruk/biblia/master/json"

Write-Host "`n[BíblIA] Baixando KJV..." -ForegroundColor Cyan
$kjv = Invoke-RestMethod "$baseUrl/en_kjv.json"

Write-Host "[BíblIA] Baixando ACF..." -ForegroundColor Cyan
$acf = Invoke-RestMethod "$baseUrl/pt_acf.json"

# ── 2. Determinar range de livros ────────────────────────────────────────────
# Os JSONs seguem a ordem canônica: índice 0 = Gênesis (orderIndex=1) … índice 65 = Apocalipse (orderIndex=66)
switch ($Testament.ToUpper()) {
    "NT"  { $start = 39; $end = 65 }  # Mateus (40) até Apocalipse (66)
    "OT"  { $start = 0;  $end = 38 }  # Gênesis (1)  até Malaquias (39)
    "ALL" { $start = 0;  $end = 65 }  # Bíblia completa
    default { Write-Error "Testament deve ser NT, OT ou ALL."; exit 1 }
}

# ── 3. Construir lista de versículos ─────────────────────────────────────────
Write-Host "[BíblIA] Construindo lista de versículos ($Testament)..." -ForegroundColor Cyan

$allVerses = [System.Collections.Generic.List[hashtable]]::new()

for ($i = $start; $i -le $end; $i++) {
    $orderIndex = $i + 1
    $kjvBook    = $kjv[$i]
    $acfBook    = $acf[$i]

    if ($null -eq $kjvBook) {
        Write-Warning "Livro índice $i não encontrado no KJV, pulando."
        continue
    }

    $numChapters = $kjvBook.chapters.Count

    for ($chIdx = 0; $chIdx -lt $numChapters; $chIdx++) {
        $chapterNum   = $chIdx + 1
        $kjvVerses    = $kjvBook.chapters[$chIdx]
        $acfVerses    = if ($acfBook) { $acfBook.chapters[$chIdx] } else { $null }
        $numVerses    = $kjvVerses.Count

        for ($vIdx = 0; $vIdx -lt $numVerses; $vIdx++) {
            $verseNum = $vIdx + 1
            $textKJV  = $kjvVerses[$vIdx]
            $textACF  = if ($acfVerses -and $vIdx -lt $acfVerses.Count) { $acfVerses[$vIdx] } else { "" }

            $allVerses.Add(@{
                bookOrderIndex = $orderIndex
                chapter        = $chapterNum
                verse          = $verseNum
                textKJV        = $textKJV
                textACF        = $textACF
            })
        }
    }
}

$total = $allVerses.Count
Write-Host "[BíblIA] $total versículos prontos para importar." -ForegroundColor Green

# ── 4. Importar em lotes ─────────────────────────────────────────────────────
$imported = 0
$skipped  = 0
$batches  = [Math]::Ceiling($total / $BatchSize)

Write-Host "[BíblIA] Importando em $batches lotes de $BatchSize..." -ForegroundColor Cyan

for ($b = 0; $b -lt $batches; $b++) {
    $startIdx = $b * $BatchSize
    $endIdx   = [Math]::Min($startIdx + $BatchSize - 1, $total - 1)
    $batch    = $allVerses[$startIdx..$endIdx]

    $body = $batch | ConvertTo-Json -Depth 3 -Compress

    try {
        $result = Invoke-RestMethod `
            -Method POST `
            -Uri "$ApiUrl/api/bible/import" `
            -ContentType "application/json; charset=utf-8" `
            -Body ([System.Text.Encoding]::UTF8.GetBytes($body))

        $imported += $result.imported
        $skipped  += $result.skipped

        $pct = [Math]::Round(($b + 1) / $batches * 100)
        Write-Host "  Lote $($b+1)/$batches ($pct%) — importados: $($result.imported), pulados: $($result.skipped)" -ForegroundColor Gray
    }
    catch {
        Write-Error "Erro no lote $($b+1): $_"
    }
}

# ── 5. Resumo final ──────────────────────────────────────────────────────────
Write-Host "`n╔══════════════════════════════════════╗" -ForegroundColor Green
Write-Host   "║  Importação concluída!               ║" -ForegroundColor Green
Write-Host   "╠══════════════════════════════════════╣" -ForegroundColor Green
Write-Host   "║  Total processado : $($total.ToString().PadLeft(6))            ║" -ForegroundColor Green
Write-Host   "║  Importados       : $($imported.ToString().PadLeft(6))            ║" -ForegroundColor Green
Write-Host   "║  Já existiam      : $($skipped.ToString().PadLeft(6))            ║" -ForegroundColor Green
Write-Host   "╚══════════════════════════════════════╝" -ForegroundColor Green
