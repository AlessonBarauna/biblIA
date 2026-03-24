param(
    [string]$ApiUrl    = "http://localhost:5239",
    [string]$Testament = "ALL",
    [int]   $BatchSize = 500
)

# Uso:
#   .\Import-Bible.ps1               # Biblia completa (padrao)
#   .\Import-Bible.ps1 -Testament NT
#   .\Import-Bible.ps1 -Testament OT
#
# Fonte: github.com/aruljohn/Bible-kjv (KJV, dominio publico)
# 66 requisicoes HTTP (1 por livro) -- sem rate limiting severo.

$ErrorActionPreference = "Continue"

# Nomes canonicos dos livros na ordem 1-66 (correspondem aos arquivos do repo)
$bookNames = @(
    "Genesis", "Exodus", "Leviticus", "Numbers", "Deuteronomy",
    "Joshua", "Judges", "Ruth", "1 Samuel", "2 Samuel",
    "1 Kings", "2 Kings", "1 Chronicles", "2 Chronicles", "Ezra",
    "Nehemiah", "Esther", "Job", "Psalms", "Proverbs",
    "Ecclesiastes", "Song of Solomon", "Isaiah", "Jeremiah", "Lamentations",
    "Ezekiel", "Daniel", "Hosea", "Joel", "Amos",
    "Obadiah", "Jonah", "Micah", "Nahum", "Habakkuk",
    "Zephaniah", "Haggai", "Zechariah", "Malachi",
    "Matthew", "Mark", "Luke", "John", "Acts",
    "Romans", "1 Corinthians", "2 Corinthians", "Galatians", "Ephesians",
    "Philippians", "Colossians", "1 Thessalonians", "2 Thessalonians",
    "1 Timothy", "2 Timothy", "Titus", "Philemon", "Hebrews",
    "James", "1 Peter", "2 Peter", "1 John", "2 John",
    "3 John", "Jude", "Revelation"
)

# Filtro de testamento (orderIndex 1-39 = OT, 40-66 = NT)
$minIdx = 0  # 0-based
$maxIdx = 65
switch ($Testament.ToUpper()) {
    "NT"  { $minIdx = 39 }
    "OT"  { $maxIdx = 38 }
    "ALL" { }
    default { Write-Error "Testament deve ser NT, OT ou ALL"; exit 1 }
}

$selectedBooks = $bookNames[$minIdx..$maxIdx]
Write-Host "[BibleIA] $($selectedBooks.Count) livros ($Testament)." -ForegroundColor Cyan

# Verifica se a API esta acessivel
Write-Host "[BibleIA] Verificando API em $ApiUrl..." -ForegroundColor DarkGray
try {
    $null = Invoke-RestMethod "$ApiUrl/api/bible/books" -TimeoutSec 5
    Write-Host "[BibleIA] API ok." -ForegroundColor Green
} catch {
    Write-Error "API nao acessivel em $ApiUrl. Verifique se a API esta rodando (dotnet run)."
    exit 1
}

$baseUrl    = "https://raw.githubusercontent.com/aruljohn/Bible-kjv/master"
$allVerses  = [System.Collections.Generic.List[hashtable]]::new()
$booksDone  = 0
$bookFailed = 0

foreach ($bookName in $selectedBooks) {
    $orderIndex = $bookNames.IndexOf($bookName) + 1
    # Arquivos do repo nao tem espacos: "1 Samuel" -> "1Samuel", "Song of Solomon" -> "SongofSolomon"
    $encoded    = $bookName -replace " ", ""
    $url        = "$baseUrl/$encoded.json"

    try {
        $data = Invoke-RestMethod $url -TimeoutSec 30

        foreach ($ch in $data.chapters) {
            $chNum = [int]$ch.chapter
            foreach ($v in $ch.verses) {
                $allVerses.Add(@{
                    bookOrderIndex = $orderIndex
                    chapter        = $chNum
                    verse          = [int]$v.verse
                    textKJV        = $v.text.Trim()
                    textACF        = ""
                })
            }
        }

        $booksDone++
        $pct = [Math]::Round($booksDone / $selectedBooks.Count * 100)
        Write-Host "  [$booksDone/$($selectedBooks.Count)] $bookName ($($pct)%)" -ForegroundColor DarkGray
    } catch {
        Write-Warning "  FALHOU: $bookName -- $_"
        $bookFailed++
    }
}

$total = $allVerses.Count
$color = if ($bookFailed -eq 0) { "Green" } else { "Yellow" }
Write-Host "[BibleIA] $total versiculos prontos. Livros com falha: $bookFailed" -ForegroundColor $color

if ($total -eq 0) {
    Write-Error "Nenhum versiculo encontrado. Verifique a conexao."
    exit 1
}

# --- Importar em lotes ---
$batches  = [Math]::Ceiling($total / $BatchSize)
$imported = 0
$skipped  = 0
$batchErr = 0

Write-Host "[BibleIA] Importando $total versiculos em $batches lotes..." -ForegroundColor Cyan

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
            -Body ([System.Text.Encoding]::UTF8.GetBytes($body)) `
            -TimeoutSec 120

        $imported += $result.imported
        $skipped  += $result.skipped
        $pct = [Math]::Round(($b + 1) / $batches * 100)
        Write-Host "  Lote $($b+1)/$batches ($($pct)%) - importados:$($result.imported) pulados:$($result.skipped)" -ForegroundColor Gray
    } catch {
        Write-Warning "  Erro no lote $($b+1): $_"
        $batchErr++
    }
}

Write-Host ""
Write-Host "=== Importacao concluida ===" -ForegroundColor Green
Write-Host "  Versiculos no arquivo: $total"
Write-Host "  Importados           : $imported" -ForegroundColor Green
Write-Host "  Ja existiam (pulados): $skipped"
if ($bookFailed -gt 0) { Write-Host "  Livros com falha     : $bookFailed" -ForegroundColor Yellow }
if ($batchErr   -gt 0) { Write-Host "  Lotes com erro       : $batchErr"   -ForegroundColor Yellow }
Write-Host ""
Write-Host "Dica: reexecute o script a qualquer momento -- e idempotente." -ForegroundColor DarkGray
