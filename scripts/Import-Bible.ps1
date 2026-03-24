param(
    [string]$ApiUrl    = "http://localhost:5239",
    [string]$Testament = "NT",
    [int]   $BatchSize = 500,
    [int]   $DelayMs   = 300,
    [int]   $MaxRetry  = 3
)

# Uso:
#   .\Import-Bible.ps1               # NT (padrao, ~260 capitulos, ~3 min)
#   .\Import-Bible.ps1 -Testament ALL  # Biblia completa (~1189 capitulos, ~7 min)
#   .\Import-Bible.ps1 -Testament OT

# Nao usar Stop — queremos continuar em erros individuais e reportar no final
$ErrorActionPreference = "Continue"

# --- Mapa canonico de livros ---
# orderIndex (1-66), nome para a bible-api.com, contagem de capitulos
$books = @(
    @{i=1;  n="genesis";           c=50},
    @{i=2;  n="exodus";            c=40},
    @{i=3;  n="leviticus";         c=27},
    @{i=4;  n="numbers";           c=36},
    @{i=5;  n="deuteronomy";       c=34},
    @{i=6;  n="joshua";            c=24},
    @{i=7;  n="judges";            c=21},
    @{i=8;  n="ruth";              c=4},
    @{i=9;  n="1+samuel";          c=31},
    @{i=10; n="2+samuel";          c=24},
    @{i=11; n="1+kings";           c=22},
    @{i=12; n="2+kings";           c=25},
    @{i=13; n="1+chronicles";      c=29},
    @{i=14; n="2+chronicles";      c=36},
    @{i=15; n="ezra";              c=10},
    @{i=16; n="nehemiah";          c=13},
    @{i=17; n="esther";            c=10},
    @{i=18; n="job";               c=42},
    @{i=19; n="psalms";            c=150},
    @{i=20; n="proverbs";          c=31},
    @{i=21; n="ecclesiastes";      c=12},
    @{i=22; n="song+of+solomon";   c=8},
    @{i=23; n="isaiah";            c=66},
    @{i=24; n="jeremiah";          c=52},
    @{i=25; n="lamentations";      c=5},
    @{i=26; n="ezekiel";           c=48},
    @{i=27; n="daniel";            c=12},
    @{i=28; n="hosea";             c=14},
    @{i=29; n="joel";              c=3},
    @{i=30; n="amos";              c=9},
    @{i=31; n="obadiah";           c=1},
    @{i=32; n="jonah";             c=4},
    @{i=33; n="micah";             c=7},
    @{i=34; n="nahum";             c=3},
    @{i=35; n="habakkuk";          c=3},
    @{i=36; n="zephaniah";         c=3},
    @{i=37; n="haggai";            c=2},
    @{i=38; n="zechariah";         c=14},
    @{i=39; n="malachi";           c=4},
    @{i=40; n="matthew";           c=28},
    @{i=41; n="mark";              c=16},
    @{i=42; n="luke";              c=24},
    @{i=43; n="john";              c=21},
    @{i=44; n="acts";              c=28},
    @{i=45; n="romans";            c=16},
    @{i=46; n="1+corinthians";     c=16},
    @{i=47; n="2+corinthians";     c=13},
    @{i=48; n="galatians";         c=6},
    @{i=49; n="ephesians";         c=6},
    @{i=50; n="philippians";       c=4},
    @{i=51; n="colossians";        c=4},
    @{i=52; n="1+thessalonians";   c=5},
    @{i=53; n="2+thessalonians";   c=3},
    @{i=54; n="1+timothy";         c=6},
    @{i=55; n="2+timothy";         c=4},
    @{i=56; n="titus";             c=3},
    @{i=57; n="philemon";          c=1},
    @{i=58; n="hebrews";           c=13},
    @{i=59; n="james";             c=5},
    @{i=60; n="1+peter";           c=5},
    @{i=61; n="2+peter";           c=3},
    @{i=62; n="1+john";            c=5},
    @{i=63; n="2+john";            c=1},
    @{i=64; n="3+john";            c=1},
    @{i=65; n="jude";              c=1},
    @{i=66; n="revelation";        c=22}
)

# Filtrar por testamento
switch ($Testament.ToUpper()) {
    "NT"  { $books = $books | Where-Object { $_.i -ge 40 } }
    "OT"  { $books = $books | Where-Object { $_.i -le 39 } }
    "ALL" { }
    default { Write-Error "Testament deve ser NT, OT ou ALL"; exit 1 }
}

$totalChapters = ($books | ForEach-Object { $_.c } | Measure-Object -Sum).Sum
Write-Host "[BibleIA] $($books.Count) livros, $totalChapters capitulos ($Testament)." -ForegroundColor Cyan
Write-Host "[BibleIA] Fonte: bible-api.com (KJV) | delay: ${DelayMs}ms | batch: $BatchSize" -ForegroundColor DarkGray

# Verifica se a API esta acessivel
try {
    $null = Invoke-RestMethod "$ApiUrl/api/bible/books" -TimeoutSec 5
    Write-Host "[BibleIA] API respondendo em $ApiUrl" -ForegroundColor Green
} catch {
    Write-Error "API nao acessivel em $ApiUrl. Certifique-se de que a API esta rodando."
    exit 1
}

# --- Baixar e acumular versiculos ---
$allVerses  = [System.Collections.Generic.List[hashtable]]::new()
$chapDone   = 0
$chapFailed = 0

foreach ($book in $books) {
    for ($ch = 1; $ch -le $book.c; $ch++) {
        $url     = "https://bible-api.com/$($book.n)+$($ch)?translation=kjv"
        $success = $false

        for ($try = 1; $try -le $MaxRetry; $try++) {
            try {
                $data = Invoke-RestMethod $url -TimeoutSec 30
                foreach ($v in $data.verses) {
                    $allVerses.Add(@{
                        bookOrderIndex = $book.i
                        chapter        = $v.chapter
                        verse          = $v.verse
                        textKJV        = $v.text.Trim()
                        textACF        = ""
                    })
                }
                $success = $true
                break
            } catch {
                if ($try -lt $MaxRetry) {
                    $wait = $try * 1000  # backoff: 1s, 2s
                    Write-Warning "  Tentativa $try/$MaxRetry falhou para $($book.n) $ch. Aguardando ${wait}ms..."
                    Start-Sleep -Milliseconds $wait
                } else {
                    Write-Warning "  FALHOU (todas as tentativas): $($book.n) $ch — $_"
                    $chapFailed++
                }
            }
        }

        if ($success) {
            $chapDone++
            if ($chapDone % 20 -eq 0) {
                $pct = [Math]::Round($chapDone / $totalChapters * 100)
                Write-Host "  Baixando... $chapDone/$totalChapters capitulos ($pct%)" -ForegroundColor DarkGray
            }
        }

        if ($DelayMs -gt 0) { Start-Sleep -Milliseconds $DelayMs }
    }
}

$total = $allVerses.Count
Write-Host "[BibleIA] $total versiculos baixados | $chapFailed capitulos com falha." -ForegroundColor $(if ($chapFailed -eq 0) { "Green" } else { "Yellow" })

if ($total -eq 0) {
    Write-Error "Nenhum versiculo baixado. Verifique a conexao com bible-api.com."
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
        Write-Host "  Lote $($b+1)/$batches ($pct%) — importados:$($result.imported) pulados:$($result.skipped)" -ForegroundColor Gray
    } catch {
        Write-Warning "  Erro no lote $($b+1): $_"
        $batchErr++
    }
}

Write-Host ""
Write-Host "=== Importacao concluida ===" -ForegroundColor Green
Write-Host "  Versiculos baixados : $total"
Write-Host "  Importados          : $imported" -ForegroundColor Green
Write-Host "  Ja existiam (pulados): $skipped"
if ($chapFailed -gt 0)  { Write-Host "  Capitulos com falha  : $chapFailed"  -ForegroundColor Yellow }
if ($batchErr   -gt 0)  { Write-Host "  Lotes com erro       : $batchErr"    -ForegroundColor Yellow }
Write-Host ""
Write-Host "Dica: reexecute o script para preencher eventuais falhas (e idempotente)." -ForegroundColor DarkGray
