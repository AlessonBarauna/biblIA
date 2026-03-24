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

function Try-Download([string[]]$urls) {
    foreach ($url in $urls) {
        try {
            Write-Host "  GET $url" -ForegroundColor DarkGray
            return Invoke-RestMethod $url
        } catch {
            Write-Host "  Falhou ($($_.Exception.Message.Split([char]10)[0]))" -ForegroundColor Yellow
        }
    }
    return $null
}

# =============================================================================
# 1. Baixar KJV
# Fonte: scrollmapper/bible_databases
# Formato: {"resultset":{"row":[{"field":[book,chapter,verse,text]},...]}}
# book 1=Genesis ... 66=Apocalipse (igual ao nosso orderIndex)
# =============================================================================
Write-Host "[1/4] Baixando KJV (scrollmapper)..." -ForegroundColor Cyan
$kjvRaw = Try-Download @(
    "https://raw.githubusercontent.com/scrollmapper/bible_databases/master/json/t_kjv.json"
)
if ($null -eq $kjvRaw) {
    Write-Error "Nao foi possivel baixar o KJV. Verifique sua conexao com a internet."
    exit 1
}

# Indexar KJV por (book, chapter, verse) para lookup O(1) ao cruzar com PT
$kjvIndex = @{}
foreach ($row in $kjvRaw.resultset.row) {
    $f = $row.field
    $key = "$($f[0])_$($f[1])_$($f[2])"
    $kjvIndex[$key] = $f[3]
}
Write-Host "  $($kjvIndex.Count) versiculos KJV carregados." -ForegroundColor Green

# =============================================================================
# 2. Baixar Portugues (ACF / AA)
# Tenta varias fontes conhecidas. Se falhar, importa so KJV.
# =============================================================================
Write-Host "[2/4] Baixando portugues (ACF/AA)..." -ForegroundColor Cyan
$ptRaw = Try-Download @(
    "https://raw.githubusercontent.com/thiagobodruk/biblia/main/json/pt_acf.json",
    "https://raw.githubusercontent.com/thiagobodruk/biblia/master/json/pt_acf.json",
    "https://raw.githubusercontent.com/thiagobodruk/biblia/main/json/pt_aa.json",
    "https://raw.githubusercontent.com/thiagobodruk/biblia/master/json/pt_aa.json"
)

# Indexar PT por (bookOrderIndex, chapter, verse) se disponivel
# Formato thiagobodruk: array de livros -> chapters (array de arrays de strings)
$ptIndex = @{}
if ($null -ne $ptRaw) {
    for ($bi = 0; $bi -lt $ptRaw.Count; $bi++) {
        $book     = $ptRaw[$bi]
        $orderIdx = $bi + 1
        for ($ci = 0; $ci -lt $book.chapters.Count; $ci++) {
            $ch = $ci + 1
            for ($vi = 0; $vi -lt $book.chapters[$ci].Count; $vi++) {
                $key = "${orderIdx}_${ch}_$($vi+1)"
                $ptIndex[$key] = $book.chapters[$ci][$vi]
            }
        }
    }
    Write-Host "  $($ptIndex.Count) versiculos PT carregados." -ForegroundColor Green
} else {
    Write-Warning "Portugues nao disponivel. Importando apenas KJV (campo textACF ficara vazio)."
}

# =============================================================================
# 3. Determinar range de livros
# =============================================================================
switch ($Testament.ToUpper()) {
    "NT"  { $bookStart = 40; $bookEnd = 66 }
    "OT"  { $bookStart = 1;  $bookEnd = 39 }
    "ALL" { $bookStart = 1;  $bookEnd = 66 }
    default { Write-Error "Testament deve ser NT, OT ou ALL"; exit 1 }
}

# =============================================================================
# 4. Montar lista de versiculos
# =============================================================================
Write-Host "[3/4] Construindo lista ($Testament, livros $bookStart-$bookEnd)..." -ForegroundColor Cyan

$allVerses = [System.Collections.Generic.List[hashtable]]::new()

foreach ($row in $kjvRaw.resultset.row) {
    $f   = $row.field
    $b   = [int]$f[0]
    $ch  = [int]$f[1]
    $v   = [int]$f[2]
    $txt = $f[3]

    if ($b -lt $bookStart -or $b -gt $bookEnd) { continue }

    $ptKey  = "${b}_${ch}_${v}"
    $textPT = if ($ptIndex.ContainsKey($ptKey)) { $ptIndex[$ptKey] } else { "" }

    $allVerses.Add(@{
        bookOrderIndex = $b
        chapter        = $ch
        verse          = $v
        textKJV        = $txt
        textACF        = $textPT
    })
}

$total   = $allVerses.Count
$batches = [Math]::Ceiling($total / $BatchSize)
Write-Host "  $total versiculos, $batches lotes de $BatchSize." -ForegroundColor Green

# =============================================================================
# 5. Importar em lotes
# =============================================================================
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
        Write-Host "  Lote $($b+1)/$batches ($pct%) - ok:$($result.imported) pulados:$($result.skipped)" -ForegroundColor Gray
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
