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
# Fontes:
#   KJV : github.com/aruljohn/Bible-kjv      (66 arquivos, 1 por livro)
#   AA/ACF/NVI : github.com/thiagobodruk/biblia (3 arquivos, 1 por traducao)

$ErrorActionPreference = "Continue"

# Nomes canonicos na ordem 1-66 (indices 0-65)
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

$minIdx = 0
$maxIdx = 65
switch ($Testament.ToUpper()) {
    "NT"  { $minIdx = 39 }
    "OT"  { $maxIdx = 38 }
    "ALL" { }
    default { Write-Error "Testament deve ser NT, OT ou ALL"; exit 1 }
}

Write-Host "[BibleIA] Testament=$Testament | batch=$BatchSize" -ForegroundColor Cyan

# Verifica API
try {
    $null = Invoke-RestMethod "$ApiUrl/api/bible/books" -TimeoutSec 5
    Write-Host "[BibleIA] API ok em $ApiUrl" -ForegroundColor Green
} catch {
    Write-Error "API nao acessivel em $ApiUrl."
    exit 1
}

# ────────────────────────────────────────────────────────────────────────────
# 1. KJV (aruljohn/Bible-kjv) — um arquivo por livro
#    Formato: { book, chapters: [ { chapter, verses: [{verse, text}] } ] }
# ────────────────────────────────────────────────────────────────────────────
Write-Host "[BibleIA] Baixando KJV (66 arquivos)..." -ForegroundColor Cyan
$kjvBase  = "https://raw.githubusercontent.com/aruljohn/Bible-kjv/master"
# Dicionario (orderIndex, chapter, verse) -> textKJV
$kjvMap = @{}

for ($i = $minIdx; $i -le $maxIdx; $i++) {
    $name    = $bookNames[$i]
    $encoded = $name -replace " ", ""  # "1 Samuel" -> "1Samuel"
    $url     = "$kjvBase/$encoded.json"
    try {
        $data = Invoke-RestMethod $url -TimeoutSec 30
        foreach ($ch in $data.chapters) {
            $chNum = [int]$ch.chapter
            foreach ($v in $ch.verses) {
                $key = "$($i+1)_$($chNum)_$([int]$v.verse)"
                $kjvMap[$key] = $v.text.Trim()
            }
        }
        Write-Host "  KJV [$($i+1)/66] $name" -ForegroundColor DarkGray
    } catch {
        Write-Warning "  KJV FALHOU: $name -- $_"
    }
}
Write-Host "[BibleIA] KJV: $($kjvMap.Count) versiculos." -ForegroundColor Green

# ────────────────────────────────────────────────────────────────────────────
# 2. PT-BR (thiagobodruk/biblia) — um arquivo por traducao
#    Formato: [ { abbrev, chapters: [ ["v1","v2",...], [...] ] } ]
#    Indice 0-based: chapters[chIdx][vIdx]
# ────────────────────────────────────────────────────────────────────────────
$ptBase = "https://raw.githubusercontent.com/thiagobodruk/biblia/master/json"

function Get-PtMap($url, $label) {
    $map = @{}
    try {
        Write-Host "[BibleIA] Baixando $label..." -ForegroundColor Cyan
        $data = Invoke-RestMethod $url -TimeoutSec 60
        # Array de 66 livros na ordem canonica (indice 0 = Genesis)
        for ($bi = $minIdx; $bi -le $maxIdx; $bi++) {
            $book    = $data[$bi]
            $orderIdx = $bi + 1
            for ($chIdx = 0; $chIdx -lt $book.chapters.Count; $chIdx++) {
                $chNum   = $chIdx + 1
                $chapter = $book.chapters[$chIdx]
                for ($vIdx = 0; $vIdx -lt $chapter.Count; $vIdx++) {
                    $vNum = $vIdx + 1
                    $key  = "${orderIdx}_${chNum}_${vNum}"
                    $map[$key] = $chapter[$vIdx]
                }
            }
        }
        Write-Host "[BibleIA] $($label): $($map.Count) versiculos." -ForegroundColor Green
    } catch {
        Write-Warning "[BibleIA] Falha ao baixar $($label): $_"
    }
    return $map
}

$aaMap  = Get-PtMap "$ptBase/aa.json"  "AA  (Almeida Revisada)"
$acfMap = Get-PtMap "$ptBase/acf.json" "ACF (Almeida Corrigida e Fiel)"
$nviMap = Get-PtMap "$ptBase/nvi.json" "NVI (Nova Versao Internacional)"

# ────────────────────────────────────────────────────────────────────────────
# 3. Monta lista unificada de ImportVerseDto
# ────────────────────────────────────────────────────────────────────────────
# A uniao de chaves de todas as traducoes garante que nenhum versiculo seja perdido.
$allKeys = [System.Collections.Generic.HashSet[string]]::new()
foreach ($k in $kjvMap.Keys)  { $null = $allKeys.Add($k) }
foreach ($k in $aaMap.Keys)   { $null = $allKeys.Add($k) }
foreach ($k in $acfMap.Keys)  { $null = $allKeys.Add($k) }
foreach ($k in $nviMap.Keys)  { $null = $allKeys.Add($k) }

$allVerses = [System.Collections.Generic.List[hashtable]]::new()

foreach ($key in $allKeys) {
    $parts = $key -split "_"
    $allVerses.Add(@{
        bookOrderIndex = [int]$parts[0]
        chapter        = [int]$parts[1]
        verse          = [int]$parts[2]
        textKJV        = if ($kjvMap.ContainsKey($key))  { $kjvMap[$key]  } else { "" }
        textAA         = if ($aaMap.ContainsKey($key))   { $aaMap[$key]   } else { "" }
        textACF        = if ($acfMap.ContainsKey($key))  { $acfMap[$key]  } else { "" }
        textNVI        = if ($nviMap.ContainsKey($key))  { $nviMap[$key]  } else { "" }
    })
}

# Ordena para inserir na sequencia canonica
$allVerses = $allVerses | Sort-Object { [int]$_.bookOrderIndex * 1000000 + [int]$_.chapter * 1000 + [int]$_.verse }

$total = $allVerses.Count
Write-Host "[BibleIA] $total versiculos combinados. Iniciando importacao..." -ForegroundColor Cyan

# ────────────────────────────────────────────────────────────────────────────
# 4. Importar em lotes
# ────────────────────────────────────────────────────────────────────────────
$batches  = [Math]::Ceiling($total / $BatchSize)
$imported = 0
$skipped  = 0
$batchErr = 0

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
Write-Host "  Total de versiculos  : $total"
Write-Host "  Importados           : $imported" -ForegroundColor Green
Write-Host "  Ja existiam (pulados): $skipped"
if ($batchErr -gt 0) { Write-Host "  Lotes com erro       : $batchErr" -ForegroundColor Yellow }
Write-Host ""
Write-Host "Traducoes importadas: KJV, AA, ACF, NVI" -ForegroundColor Cyan
Write-Host "Dica: reexecute o script a qualquer momento -- e idempotente." -ForegroundColor DarkGray
