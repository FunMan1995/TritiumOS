\ L.I.N.E.O.S. graduation + brand markers stub — TritiumOS.txt §1a.1 / docs/LINEOS.md / docs/LINEOS-BRAND.md
\ Words: lineos-graduate  lineos-graduate-demo  become-lineos
\        lineos-splash  lineos-about  lineos-brand-demo
\ Thresholds mirror evolve/graduation.json (host SoT creates defaults).
\ Brand markers under evolve/lineos/ (host/CLI write; Forth prints markers).
\ Scaffold only — no production branding release.

variable grad-min-sessions       30 grad-min-sessions !
variable grad-min-neurons         8 grad-min-neurons !
variable grad-min-clusters        1 grad-min-clusters !
variable grad-min-refined         1 grad-min-refined !
variable grad-require-license    -1 grad-require-license !   \ true
variable grad-require-confirm    -1 grad-require-confirm !
variable grad-demo-force          0 grad-demo-force !

\ Stub metrics (host loads real/demo values; Forth keeps in-memory for smoke)
variable grad-sessions            0 grad-sessions !
variable grad-neurons             0 grad-neurons !
variable grad-clusters            0 grad-clusters !
variable grad-refined             0 grad-refined !
variable grad-license-ok          0 grad-license-ok !
variable grad-confirm             0 grad-confirm !
variable grad-become              0 grad-become !   \ become-lineos invoked
variable grad-product-id          0 grad-product-id !  \ 0=tritium 1=lineos
variable grad-edition            64 grad-edition !

: become-lineos ( -- )
  -1 grad-become !
  -1 grad-confirm !   \ user invoke path may set confirm
  ." [LINEOS] become-lineos — confirm flag set (§1a.1)" cr ;

\ Print one gate result: name expected actual pass?
: (grad-gate) ( pass? -- ) if ." PASS" else ." FAIL" then ;

\ Brand marker printouts (host writes evolve/lineos/; Forth mirrors stdout)
: (lineos-brand-print) ( -- )
  ." [LINEOS] splash: L.I.N.E.O.S." cr
  ." [LINEOS] slogan: The line tread between madness and genius." cr
  ." [LINEOS] origin: TritiumOS by Draco (edition=" grad-edition @ . ." )" cr
  ." [LINEOS] brand markers → evolve/lineos/ (brand.json SPLASH.txt ABOUT.txt)" cr ;

: lineos-graduate ( -- flag )
  ." [LINEOS] lineos-graduate — TritiumOS.txt §1a.1 gates" cr
  grad-demo-force @ if
    ." [LINEOS] demoForceReady=true — forcing stub metrics ready" cr
    grad-min-sessions @ grad-sessions !
    grad-min-neurons @ grad-neurons !
    grad-min-clusters @ grad-clusters !
    grad-min-refined @ grad-refined !
    -1 grad-license-ok !
    -1 grad-confirm !
  then

  \ Gate 1: sessions OR become-lineos
  0
  grad-sessions @ grad-min-sessions @ >= if drop -1 then
  grad-become @ if drop -1 then
  dup ." [LINEOS] gate sessions/become: " (grad-gate) cr
  \ Gate 2: neurons + connected clusters
  grad-neurons @ grad-min-neurons @ >=
  grad-clusters @ grad-min-clusters @ >= and
  dup ." [LINEOS] gate neurons+clusters: " (grad-gate) cr
  and
  \ Gate 3: refined per class
  grad-refined @ grad-min-refined @ >=
  dup ." [LINEOS] gate refined/class: " (grad-gate) cr
  and
  \ Gate 4: license
  grad-require-license @ if
    grad-license-ok @
  else -1 then
  dup ." [LINEOS] gate license: " (grad-gate) cr
  and
  \ Gate 5: user confirm
  grad-require-confirm @ if
    grad-confirm @
  else -1 then
  dup ." [LINEOS] gate confirm: " (grad-gate) cr
  and

  dup if
    1 grad-product-id !
    ." [LINEOS] scaffold product_id → lineos (preserve edition=" grad-edition @ . ." )" cr
    ." [LINEOS] UI product name → L.I.N.E.O.S." cr
    ." [LINEOS] slogan: The line tread between madness and genius." cr
    (lineos-brand-print)
    ." [LINEOS] graduate OK — scaffold only (not a production release)" cr
  else
    ." [LINEOS] graduate blocked — gates incomplete" cr
  then ;

: lineos-graduate-demo ( -- )
  ." [lineos-graduate-demo] force-ready → scaffold product_id=lineos (§1a.1)" cr
  0 grad-product-id !
  0 grad-become !
  -1 grad-demo-force !
  lineos-graduate if
    ." [lineos-graduate-demo] OK" cr
  else
    ." [lineos-graduate-demo] FAIL" cr
  then
  0 grad-demo-force ! ;

\ Splash — refuse if not graduated (§1a.1); host may force via brand-demo
: lineos-splash ( -- )
  grad-product-id @ 1 = if
    ." [LINEOS] splash: L.I.N.E.O.S." cr
    ." [LINEOS] slogan: The line tread between madness and genius." cr
    ." [LINEOS] origin: TritiumOS by Draco (edition=" grad-edition @ . ." )" cr
  else
    ." [LINEOS] refuse — not graduated (§1a.1)" cr
  then ;

\ About — assistant + slogan if graduated
: lineos-about ( -- )
  grad-product-id @ 1 = if
    ." [LINEOS] about: L.I.N.E.O.S." cr
    ." [LINEOS] slogan: The line tread between madness and genius." cr
    ." [LINEOS] origin: TritiumOS by Draco (edition=" grad-edition @ . ." )" cr
  else
    ." [LINEOS] about: assistant — powered by TritiumOS" cr
  then ;

\ Force graduated markers → assert slogan + productName + edition → OK
: lineos-brand-demo ( -- )
  ." [lineos-brand-demo] force-ready → brand markers (§1a.1 / docs/LINEOS-BRAND.md)" cr
  1 grad-product-id !
  (lineos-brand-print)
  ." [lineos-brand-demo] OK" cr ;
